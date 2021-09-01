// ------------------------------------------------------
// Gets the density at the specified position
// ------------------------------------------------------
float getDensity(float3 pos)
{
    return tex3Dlod(_DataTex, float4(pos.x, pos.y, pos.z, 0.0f));
}


// ------------------------------------------------------
// Converts local position to depth value
// ------------------------------------------------------
float localToDepth(float3 localPos)
{
    float4 clipPos = UnityObjectToClipPos(float4(localPos, 1.0f));

    #if defined(SHADER_API_GLCORE) || defined(SHADER_API_OPENGL) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
        return (clipPos.z / clipPos.w) * 0.5 + 0.5;
    #else
        return clipPos.z / clipPos.w;
    #endif
}

// ------------------------------------------------------
// Get a ray pointing from the vertex to the fragment
// ------------------------------------------------------
ray getVertexToFragmentRay(fragment_in input)
{
    ray output;

    output.origin = input.objectSpacePosition + float3(0.5f, 0.5f, 0.5f);
    output.direction = normalize(input.vertexToCamera);

    // Create a small random offset in order to remove artifacts
    output.origin = output.origin + (2.0f * output.direction / NUM_STEPS) * tex2D(_NoiseTex, float2(input.uv.x, input.uv.y)).r;

    return output;
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