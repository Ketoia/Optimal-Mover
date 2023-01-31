Shader "Custom/UVShower"
{
    Properties
    {
        _MainTex ("_MainTex (RGBA)", 2D) = "white" {}
        _Color ("_MainTex (RGBA)", Color) = (.25, .5, .5, 1)
    }

    SubShader
    {        
        Tags { "RenderType"="Opaque" "DisableBatching" = "True" }

        Pass
        {            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _Color;
            float4 _MainTex_ST;


            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                return float4(0, 0, 0, 1);
                //return float4(0, i.uv.y, 0, 1);
                //return float4(i.uv.x, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}
