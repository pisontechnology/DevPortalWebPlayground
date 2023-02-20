/// <summary>
/// 
/// Very basic grid shader to put on any object
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
Shader "Unlit/GridShader"
{
  Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_GridColor ("Grid Color", Color) = (255,.0,.0,1)
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
            float4 _GridColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float GridTest(float2 r) {
                float result;

                for (float i = 0.0; i <= 1; i += 0.1f) {
                    for (int j = 0; j < 2; j++) {
                        result += 1.0 - smoothstep(0.0, 0.003,abs(r[j] - i));
                    }
                }

                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 gridColour = (_GridColor * GridTest(i.uv)) + tex2D(_MainTex, i.uv);
                gridColour = float4(gridColour.r, gridColour.g, gridColour.b, 1);
                return float4(gridColour);
            }
            ENDCG
        }
    }
}
