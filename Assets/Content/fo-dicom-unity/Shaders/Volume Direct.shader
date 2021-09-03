Shader "UnityVolume/Volume Direct"
{
    Properties
    {
        _DataTex("Data Texture (Generated)", 3D) = "" {}
        _GradientTex("Gradient Texture (Generated)", 3D) = "" {}
        _NoiseTex("Noise Texture (Generated)", 2D) = "white" {}
        _TFTex("Transfer Function Texture (Generated)", 2D) = "" {}
        _WindowMin ("Window Minimum", Float) = -1000
        _WindowMax ("Window Maximum", Float) = 1000
        _CutMin("Cutoff Minimum", Float) = -1000
        _CutMax("Cutoff Maximum", Float) = 1000
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

                #pragma vertex vertex
                #pragma fragment fragment

                #include "UnityCG.cginc"

                struct VertexData
                {
                    float4 position : POSITION;
                    float4 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct FragmentInputData
                {
                    float4 screenSpacePosition : SV_POSITION;
                    float3 objectSpacePosition : TEXCOORD1;
                    float3 vertexToCamera : TEXCOORD2;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct FragmentOutputData
                {
                    float4 colour : SV_TARGET;
                    float depth : SV_DEPTH;
                };

                sampler3D _DataTex;
                sampler3D _GradientTex;
                sampler2D _NoiseTex;
                sampler2D _TFTex;

                float _WindowMin;
                float _WindowMax;
                float _CutMin;
                float _CutMax;

                #define STEP_COUNT 512

                #define STEP_SIZE 1.732f / STEP_COUNT // 1.732 is the longest straight line that can be drawn in a unit cube

                #include "Volume Utilities.cginc"

                // ------------------------------------------------------
                // Locates the fragment corresponding to the input vertex
                // ------------------------------------------------------
                FragmentInputData VertexToInputFragment(VertexData vertIn)
                {
                    FragmentInputData fragOut;

                    fragOut.screenSpacePosition = UnityObjectToClipPos(vertIn.position);
                    fragOut.objectSpacePosition = vertIn.position;
                    fragOut.vertexToCamera = ObjSpaceViewDir(vertIn.position);
                    fragOut.normal = UnityObjectToWorldNormal(vertIn.normal);
                    fragOut.uv = vertIn.uv;

                    return fragOut;
                }

                // ------------------------------------------------------
                // Raymarches towards the camera using the input fragment
                // and returns the output fragment to be rendered
                // ------------------------------------------------------
                FragmentOutputData DirectVolumeRendering(FragmentInputData fragIn)
                {
                    // Get a ray pointing from the vertex to the fragment
                    float3 origin = fragIn.objectSpacePosition + float3(0.5f, 0.5f, 0.5f);
                    float3 direction = normalize(fragIn.vertexToCamera);

                    // Create a small random offset in order to remove artifacts
                    origin = origin + (2.0f * direction / STEP_COUNT) * tex2D(_NoiseTex, float2(fragIn.uv.x, fragIn.uv.y)).r;

                    // March along the ray
                    float4 colour = float4(0.0f, 0.0f, 0.0f, 0.0f);
                    float distanceToCamera = length(fragIn.vertexToCamera);
                    uint depth = 0;
                    for (uint step = 0; step < STEP_COUNT; step++)
                    {
                        const float distance = step * STEP_SIZE;
                        const float3 currentPosition = origin + (direction * distance);

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

                        // Calculate the normalised value at the current position
                        const float density = tex3Dlod(_DataTex, float4(currentPosition, 0.0f));
                        const float value = clamp((density - _WindowMin) / (_WindowMax - _WindowMin), 0, 1);
                        
                        // Convert this value to a colour using the transfer function
                        float4 baseColour = tex2Dlod(_TFTex, float4(value, 0.0f, 0.0f, 0.0f));

                        // Hide any values outside the cutoff range
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
                    FragmentOutputData fragOut;
                    fragOut.colour = colour;

                    if (depth != 0)
                    {
                        float3 localPosition = origin + direction * (depth * STEP_SIZE) - float3(0.5f, 0.5f, 0.5f);
                        float4 clipPosition = UnityObjectToClipPos(float4(localPosition, 1.0f));
                        
                        #if defined(SHADER_API_GLCORE) || defined(SHADER_API_OPENGL) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
                            fragOut.depth = (clipPosition.z / clipPosition.w) * 0.5 + 0.5;
                        #else
                            fragOut.depth = clipPosition.z / clipPosition.w;
                        #endif
                    }
                    else
                        fragOut.depth = 0;

                    return fragOut;
                }

                // ------------------------------------------------------
                // Called procedurally by ShaderLab
                // ------------------------------------------------------
                FragmentInputData vertex(VertexData vertIn)
                {
                    return VertexToInputFragment(vertIn);
                }

                // ------------------------------------------------------
                // Called procedurally by ShaderLab
                // ------------------------------------------------------
                FragmentOutputData fragment(FragmentInputData fragIn)
                {
                    return DirectVolumeRendering(fragIn);
                }

                ENDCG
            }
        }
}
