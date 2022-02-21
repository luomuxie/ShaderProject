Shader "Custom/UnderwaterImg"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseScale("Noise Scale",float) = 1
        _NoiseFrequency("Noise Frequency",float) = 1
        _NoiseSpeed("Noise Speed",float) = 1
        _PixelOffest("Noise Offest",float) = 0.005
        _DepthDis("Depth Dis",float) = 1
        _DepthStart("Depth Start",float) = 1

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

            #include "UnityCG.cginc"
            #include "noiseSimplex.cginc"
            #define M_PI 3.14159265359
            sampler2D _CameraDepthTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 scrPos:TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _NoiseScale,_NoiseFrequency,_NoiseSpeed,_PixelOffest;
            float _DepthStart,_DepthDis;

            fixed4 frag (v2f i) : COLOR
            {
               float depthVal = Linear01Depth(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.scrPos)).r)*_ProjectionParams.z;
               depthVal = 1-saturate((depthVal-_DepthStart)/_DepthDis);

                float3 spos = float3(i.scrPos.x,i.scrPos.y,0)*_NoiseFrequency;
                spos.z += _Time.x*_NoiseSpeed;
                float noise = _NoiseScale*((snoise(spos)+1)/2);
                float4 noiseDir = float4(cos(noise*M_PI*2),sin(noise*M_PI*2),0,0);
                fixed4 col = tex2Dproj(_MainTex,i.scrPos+(normalize(noiseDir))*_PixelOffest*depthVal);
                return col;
            }
            ENDCG
        }
    }
}
    