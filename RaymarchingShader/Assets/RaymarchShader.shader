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

            //setUp            
            sampler2D _CameraDepthTexture;
            uniform float4x4 _CamFrustum,_CamToWorld;
            uniform int _MaxIter;
            uniform float _Accuracy,_MaxDis;

            //color
            uniform fixed4 _mainColor;

            //light
            uniform float3 _LightDir,_LightCol;
            uniform float _LightIntensity;           
            
            //shadow
            uniform float2 _ShadowDis;
            uniform float _ShadowIntensity,_ShadowPenumbra;

            //SDF
            uniform float4 _sphere;
            uniform float _sphereSmooth;
            uniform float _degreeRotate;

            //reflection
            uniform int _ReflectionCnt;
            uniform float _ReflactionIntensity;
            uniform float _EnvReflIntensity;
            uniform samplerCUBE _ReflactionCube;


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

            float3 rotateY(float3 v,float degree)
            {
                float rad = 0.0174532925*degree;
                float cosY = cos(rad);
                float sinY = sin(rad);
                return float3(cosY*v.x-sinY*v.z,v.y,sinY*v.x+cosY*v.z);
            }


            float disField(float3 camWPos)
            {
                float ground = sdPlane(camWPos,float4(0,1,0,0));
                float sphere = sdSphere(camWPos-_sphere.xyz,_sphere.w);
                for(int i = 1;i<8;i++){
                      float sphereTemp = sdSphere(rotateY(camWPos,_degreeRotate*i) -_sphere.xyz,_sphere.w);  
                      sphere = opUS(sphere,sphereTemp,_sphereSmooth);
                }
                return opU(sphere,ground);
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

            float hardShadow(float3 ro,float3 rd,float mint,float maxt)
            {
                for(float t = mint;t<maxt;)
                {
                   float h = disField(ro+rd*t);
                   if(h<0.001){
                       return 0.0;
                   }
                   t+=h;
                }
                return 1.0;
            }

            float softShadow(float3 ro,float3 rd,float mint,float maxt,float k)
            {
                float result = 1;
                for(float t = mint;t<maxt;)
                {
                   float h = disField(ro+rd*t);
                   if(h<0.001){
                       return 0.0;
                   }
                   result = min(result,k*h/t);
                   t+=h;
                }
                return result;
            }

            uniform float _AoStepsize,_AoIntesity;
            uniform int _AoIter;
            float AmbientOcculusion(float3 p,float3 n){
                float step = _AoStepsize;
                float ao = 0.0;
                float dis;
                for(int i = 1;i<=_AoStepsize;i++){
                    dis = step*i;
                    ao += max(0.0,(dis-disField(p+n*dis))/dis);
                }
                return (1-ao*_AoIntesity);
            }

            float3 shading(float3 p,float3 n)
            {
                float3 result;
                //diffuse Color;
                float3 color = _mainColor.rgb;
                //DirL
                float light = (_LightCol* dot(-_LightDir,n)*0.5+0.5)*_LightIntensity;
                //shadows
                //float shadow = hardShadow(p,-_LightDir,_ShadowDis.x,_ShadowDis.y)*0.5+0.5;
                float shadow = softShadow(p,-_LightDir,_ShadowDis.x,_ShadowDis.y,_ShadowPenumbra)*0.5+0.5;
                shadow = max(0.0,pow(shadow,_ShadowIntensity));

                float ao = AmbientOcculusion(p,n);
                result = color*light*shadow*ao;

                return result;
            }


            fixed4 raymarching(float3 ro,float3 rd,float depth,float maxDis,float maxIter,inout float3 p)
            {
                bool hit;
                float t = 0;
                for (int i = 0;i < maxIter;i++)
                {

                    if(t>maxDis || t>=depth){
                        hit = false;
                        break;
                    } 
                    
                    p = ro+rd*t;
                    float dis = disField(p);
                    if(dis<_Accuracy){
                       hit = true;
                       break;
                    }
                    t += dis;
                }
                return hit;
            }
                        

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture,i.uv).r);
                depth *= length(i.ray);
                fixed3 col = tex2D(_MainTex,i.uv);
                float3 rayDir = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;

                fixed4 result;
                float3 hitPos;
                bool hit = raymarching(rayOrigin,rayDir,depth,_MaxDis,_MaxIter,hitPos);
                if(hit){
                    float3 n = getNormal(hitPos);
                    float3 s = shading(hitPos,n);
                    result = fixed4(s,1);
                    result += fixed4(texCUBE(_ReflactionCube,n).rgb*_EnvReflIntensity*_ReflactionIntensity,0);
                    
                    //Reflection
                    if(_ReflectionCnt>0)
                    {
                        rayDir = normalize(reflect(rayDir,n));
                        rayOrigin = hitPos+(rayDir*0.01);
                        hit = raymarching(rayOrigin,rayDir,_MaxDis,_MaxDis*0.5,_MaxIter/2,hitPos);
                        if(hit)
                        {
                            float3 n = getNormal(hitPos);
                            float3 s = shading(hitPos,n);
                            result += fixed4(s*_ReflactionIntensity,0);
                            
                            if(_ReflectionCnt>1){
                                rayDir = normalize(reflect(rayDir,n));
                                rayOrigin = hitPos+(rayDir*0.01);
                                hit = raymarching(rayOrigin,rayDir,_MaxDis,_MaxDis*0.25,_MaxIter/4,hitPos);
                                if(hit)
                                {
                                    float3 n = getNormal(hitPos);
                                    float3 s = shading(hitPos,n);
                                    result += fixed4(s*_ReflactionIntensity*0.5,0);
                                }
                            }
                            
                        }
                    }
                    
                 }else{
                     result = fixed4(0,0,0,0);
                 }

                return fixed4(col*(1.0-result.w)+result.xyz*result.w,1.0);
            }
            ENDCG
        }
    }
}
