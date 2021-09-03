Shader "Unlit/Surface Dynamic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

/*
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



// Gets the gradient at the specified position
float3 getGradient(float3 pos)
{
    return tex3Dlod(_GradientTex, float4(pos.x, pos.y, pos.z, 0.0f)).rgb;
}

// Performs lighting calculations, and returns a modified colour.
float3 calculateLighting(float3 col, float3 normal, float3 lightDir, float3 eyeDir, float specularIntensity)
{
    float ndotl = max(lerp(0.0f, 1.0f, dot(normal, lightDir)), 0.5f); // modified, to avoid volume becoming too dark
    float3 diffuse = ndotl * col;
    float3 v = eyeDir;
    float3 r = normalize(reflect(-lightDir, normal));
    float rdotv = max(dot(r, v), 0.0);
    float3 specular = pow(rdotv, 32.0f) * float3(1.0f, 1.0f, 1.0f) * specularIntensity;
    return diffuse + specular;
}

*/


            ENDCG
        }
    }
}
