﻿Shader "Dicom/Dicom Volume Shader"
{
    Properties
    {
        _DataTex("Data Texture (Generated)", 3D) = "" {}
        _GradientTex("Gradient Texture (Generated)", 3D) = "" {}
        _NoiseTex("Noise Texture (Generated)", 2D) = "white" {}
        _TFTex("Transfer Function Texture (Generated)", 2D) = "" {}
        _WindowMin ("Window Minimum", Float) = -1000
        _WindowMax ("Window Maximum", Float) = 1000
        _CutMin("Min val", Float) = -1000
        _CutMax("Max val", Float) = 1000

            // culling box rotation offset
            // depth for each culling plane
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100
            Cull Front
            ZTest LEqual
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma multi_compile MODE_DVR MODE_SURF

                #pragma vertex vertex
                #pragma fragment fragment

                #include "UnityCG.cginc"

                struct vertex_in
                {
                    float4 position : POSITION;
                    float4 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct fragment_in
                {
                    float4 screenSpacePosition : SV_POSITION;
                    float3 objectSpacePosition : TEXCOORD1;
                    float3 vertexToCamera : TEXCOORD2;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct fragment_out
                {
                    float4 colour : SV_TARGET;
                    float depth : SV_DEPTH;
                };

                struct ray
                {
                    float3 origin : TEXCOORD0;
                    float3 direction : TEXCOORD1;
                };

                sampler3D _DataTex;
                sampler3D _GradientTex;
                sampler2D _NoiseTex;
                sampler2D _TFTex;

                float _WindowMin;
                float _WindowMax;
                float _CutMin;
                float _CutMax;

                sampler2D _MainTex;
                float4 _MainTex_ST;

                #if defined (MODE_DVR)
                    #define NUM_STEPS 512
                #endif

                #if defined (MODE_SURF)
                    #define NUM_STEPS 1024
                #endif

                #define SIZE_STEP 1.732f / NUM_STEPS // 1.732 is the longest straight line that can be drawn in a unit cube

                #include "DicomVolumeRenderUtilities.cginc"

                /* ###################################################### */
                /* ------------------ RENDER FUNCTIONS ------------------ */

                // ------------------------------------------------------
                // Locates the fragment corresponding to the input vertex
                // ------------------------------------------------------
                fragment_in vertexToInputFragment(vertex_in input)
                {
                    fragment_in output;
                    output.screenSpacePosition = UnityObjectToClipPos(input.position);
                    output.objectSpacePosition = input.position;
                    output.vertexToCamera = ObjSpaceViewDir(input.position);
                    output.normal = UnityObjectToWorldNormal(input.normal);
                    output.uv = input.uv;
                    return output;
                }

                // ------------------------------------------------------
                // Raymarches towards the camera using the input fragment
                // and returns the output fragment to be rendered
                // ------------------------------------------------------
                fragment_out fragment_directVolumeRendering(fragment_in input)
                {
                    ray ray = getVertexToFragmentRay(input);

                    // March along the ray
                    float4 colour = float4(0.0f, 0.0f, 0.0f, 0.0f);
                    float distanceToCamera = length(input.vertexToCamera);
                    uint depth = 0;
                    for (uint step = 0; step < NUM_STEPS; step++)
                    {
                        const float distance = step * SIZE_STEP;
                        const float3 currentPosition = ray.origin + (ray.direction * distance);

                        // Break if marching outside the render cube
                        if (currentPosition.x < 0.0f
                        || currentPosition.x > 1.0f
                        || currentPosition.y < 0.0f
                        || currentPosition.y > 1.0f
                        || currentPosition.z < 0.0f
                        || currentPosition.z > 1.0f)
                            break;

                        // Break if viewing from inside the volume and the raycast has gone behind the camera
                        if (distance > distanceToCamera)
                            break;

                        // TODO: Box culling here

                        const float density = getDensity(currentPosition);
                        const float value = windowDensity(density);
                        float4 baseColour = getTransferFunctionColor(value);

                        // Threshold the density above and below
                        if (density < _CutMin || density > _CutMax)
                            baseColour.a = 0.0f;

                        // Combine this step's RGB with the previous step's RGB
                        colour.rgb = baseColour.a * baseColour.rgb + (1.0f - baseColour.a) * colour.rgb;

                        // Combine this step's alpha with the previous step's alpha
                        colour.a = (baseColour.a + (1.0f - baseColour.a) * colour.a);

                        // If this step's alpha is above a certain threshold, add to the depth buffer
                        if (baseColour.a > 0.15f)
                            depth = step;

                        // If this step is completely opaque there's no need to continue stepping
                        if (colour.a > 1.0f)
                            break;
                    }

                    // Write the fragment output
                    fragment_out output;
                    output.colour = colour;

                    if (depth != 0)
                        output.depth = localToDepth(ray.origin + ray.direction * (depth * SIZE_STEP) - float3(0.5f, 0.5f, 0.5f));
                    else
                        output.depth = 0;

                    return output;
                }

                // ------------------------------------------------------
                // Renders the first point (closest to camera) with a density within thresholds
                // ------------------------------------------------------
                fragment_out fragment_surfaceRendering(fragment_in input)
                {
                    ray ray = getVertexToFragmentRay(input);

                    // Reverse the ray to trace from fragment to vertex
                    ray.origin += ray.direction * SIZE_STEP * NUM_STEPS;
                    ray.direction = -ray.direction;

                    // Step toward the vertex until we encounter a density within thresholds
                    float4 colour = float4(0,0,0,0);
                    for (uint step = 0; step < NUM_STEPS; step++)
                    {
                        const float distance = step * SIZE_STEP;
                        const float3 currentPosition = ray.origin + (ray.direction * distance);

                        if (currentPosition.x < 0.0f
                        || currentPosition.x > 1.0f
                        || currentPosition.y < 0.0f
                        || currentPosition.y > 1.0f
                        || currentPosition.z < 0.0f
                        || currentPosition.z > 1.0f)
                            continue;

                        const float density = getDensity(currentPosition);
                        if (density > _CutMin && density < _CutMax)
                        {
                            float3 normal = getGradient(currentPosition);
                            colour = getTransferFunctionColor(density);
                            colour.rgb = calculateLighting(colour.rgb, normal, -ray.direction, -ray.direction, 0.15f);
                            colour.a = 1.0f;
                            break;
                        }
                    }

                    fragment_out output;
                    output.colour = colour;
                    output.depth = localToDepth(ray.origin + ray.direction * (step * SIZE_STEP) - float3(0.5f, 0.5f, 0.5f));
                    return output;
                }


                // ------------------------------------------------------
                // Called procedurally by ShaderLab
                // ------------------------------------------------------
                fragment_in vertex(vertex_in input)
                {
                    return vertexToInputFragment(input);
                }

                // ------------------------------------------------------
                // Called procedurally by ShaderLab
                // ------------------------------------------------------
                fragment_out fragment(fragment_in input)
                {
                    #if defined (MODE_DVR)
                        return fragment_directVolumeRendering(input);
                    #endif

                    #if defined (MODE_SURF)
                        return fragment_surfaceRendering(input);
                    #endif
                }

                ENDCG
            }
        }
}