Shader "UnityVolume/Volume Slice"
{
    Properties
    {
        _DataTex("Texture", 2D) = "white" {}
        _WindowMin ("Window Minimum", Float) = -1000
        _WindowMax ("Window Maximum", Float) = 1000
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vertex
            #pragma fragment fragment

            #include "UnityCG.cginc"

            struct VertexData
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct FragmentData
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _DataTex;
            float4 _DataTex_ST;
            float _WindowMin;
            float _WindowMax;

            FragmentData vertex(VertexData vertData)
            {
                FragmentData fragData;
                fragData.position = UnityObjectToClipPos(vertData.position);
                fragData.uv = TRANSFORM_TEX(vertData.uv, _DataTex);
                return fragData;
            }

            fixed4 fragment(FragmentData fragData) : SV_Target
            {
                // Calculate the normalised value at the current position
                const float density = tex2D(_DataTex, fragData.uv);
                const float value = clamp((density - _WindowMin) / (_WindowMax - _WindowMin), 0, 1);

                fixed4 col;
                col.r = value;
                col.g = value;
                col.b = value;
                col.a = 1;

                return col;
            }
            ENDCG
        }
    }
}
