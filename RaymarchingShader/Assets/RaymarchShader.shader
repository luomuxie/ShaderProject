Shader "Comstom/RaymarchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma enable_d3d11_debug_symbols

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            uniform float4x4 _CamFrustum,_CamToWorld;
            uniform float _MaxDis;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ray:TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                half index = v.vertex.z;
                v.vertex.z = 0;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.ray = _CamFrustum[(int) index].xyz;
                o.ray /= abs(o.ray.z);
                o.ray = mul(_CamToWorld,o.ray);
                return o;
            }

            /*
            *圆的计算参考圆锥体
            */

            float sdSphere(float3 p,float s){
                return length(p)-s;
             }

            float disField(float3 camWPos)
            {
                float sphere = sdSphere(camWPos-float3(0,0,0),2.0);
                return sphere;
                  
            }

            fixed4 raymarching(float3 ro,float3 rd)
            {
                fixed4 result = fixed4(1,1,1,1);
                const int max_iter = 64;
                float t = 0;
                for (int i = 0;i < max_iter;i++)
                {
                    if(t>_MaxDis){
                        result = fixed4(rd,1);//在范围外的话赋值环境色
                        break;
                    } 
                    
                    float3 p = ro+rd*t;
                    float dis = disField(p);
                    if(dis<0.01){

                       result = fixed4(1,1,1,1);
                       break;
                    }
                    t += dis;
                    //检查射击范围
                }
                return result;
            }
                        

            fixed4 frag (v2f i) : SV_Target
            {
                float3 rayDir = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;
                fixed4 result = raymarching(rayOrigin,rayDir);
                return result;
            }
            ENDCG
        }
    }
}
