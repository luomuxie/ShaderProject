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
            #include "DistanceFunctions.cginc"

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            uniform float4x4 _CamFrustum,_CamToWorld;
            uniform float _MaxDis;
            uniform float4 _shpere1;
            uniform float3 _LightDir;
            uniform fixed4 _mainColor;

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

            float disField(float3 camWPos)
            {
                float sphere = sdSphere(camWPos - _shpere1.xyz,_shpere1.w);
                return sphere;
                  
            }

            //法线的计算可以了解一下gradient 与 normal的关系
            float3 getNormal( float3 p)
            {
                const float2 offset = float2(0.001,0.0);
                float3 n = float3(disField(p+offset.xyy)-disField(p-offset.xyy),
                            disField(p+offset.yxy)-disField(p-offset.yxy),
                            disField(p+offset.yyx)-disField(p-offset.yyx)
                );
                return normalize(n);
            }

            fixed4 raymarching(float3 ro,float3 rd,float depth)
            {
                fixed4 result = fixed4(1,1,1,1);
                const int max_iter = 164;
                float t = 0;
                for (int i = 0;i < max_iter;i++)
                {

                    if(t>_MaxDis || t>=depth){
                        result = fixed4(rd,0);//在范围外的话赋值环境色
                        break;
                    } 
                    
                    float3 p = ro+rd*t;
                    float dis = disField(p);
                    if(dis<0.01){
                       float3 n = getNormal(p);
                       float light = dot(-_LightDir,n);
                       result = fixed4(_mainColor.rgb*light,1);
                       break;
                    }
                    t += dis;
                }
                return result;
            }
                        

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture,i.uv).r);
                depth *= length(i.ray);
                fixed3 col = tex2D(_MainTex,i.uv);
                float3 rayDir = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;
                fixed4 result = raymarching(rayOrigin,rayDir,depth);
                return fixed4(col*(1.0-result.w)+result.xyz*result.w,1.0);
            }
            ENDCG
        }
    }
}
