Shader "Custom/GrassShader"
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
            #pragma multi_compile_instancing
            #pragma target 5

            #define PI 3.14159265359

            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl" 
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            
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

            //https://gist.github.com/mattatz/86fff4b32d198d0928d0fa4ff32cf6fa
            float4x4 axis_matrix(float3 right, float3 up, float3 forward)
            {
                float3 xaxis = right;
                float3 yaxis = up;
                float3 zaxis = forward;
                return float4x4(
		            xaxis.x, yaxis.x, zaxis.x, 0,
		            xaxis.y, yaxis.y, zaxis.y, 0,
		            xaxis.z, yaxis.z, zaxis.z, 0,
		            0, 0, 0, 1
	            );
            }

            // http://stackoverflow.com/questions/349050/calculating-a-lookat-matrix
            float4x4 look_at_matrix(float3 forward, float3 up)
            {
                float3 xaxis = normalize(cross(forward, up));
                float3 yaxis = up;
                float3 zaxis = forward;
                return axis_matrix(xaxis, yaxis, zaxis);
            }

            struct instanceProperties 
            {
                float3 position;
                float3 direction;
            };

            StructuredBuffer<instanceProperties> _InstanceProperties;

            v2f vert (appdata v, uint instanceID : SV_INSTANCEID)
            {
                v2f o;
                
                uint id = instanceID;
                float3 cameraPos = _WorldSpaceCameraPos;
                float3 instancePos = _InstanceProperties[id].position;
                float3 direction = _InstanceProperties[id].direction;

                float3 forward = instancePos - cameraPos;
                
                float4x4 lookMatrix = look_at_matrix(forward, direction);
                float3 localSpaceVert = mul(lookMatrix, v.vertex);

                o.vertex = TransformObjectToHClip(localSpaceVert + instancePos.xyz);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                return col * _Color;//float4(173.0 / 255.0 ,1 ,47.0 / 255.0,1);
            }
            ENDHLSL
        }
    }
}
