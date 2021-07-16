Shader "Dicom/DicomImageRendering"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
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

            struct vertexData
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragmentData
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fragmentData vertex(vertexData vertData)
            {
                fragmentData fragData;
                fragData.position = UnityObjectToClipPos(vertData.position);
                fragData.uv = TRANSFORM_TEX(vertData.uv, _MainTex);
                return fragData;
            }

            fixed4 fragment(fragmentData fragData) : SV_Target
            {
                float value = tex2D(_MainTex, fragData.uv);

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
