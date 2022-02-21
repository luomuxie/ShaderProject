Shader "Custom/FogShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (1,1,1,1)
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
            sampler2D _CameraDepthTexture;
            fixed4 _FogColor;

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
            float _DepthStart,_DepthDis;

            fixed4 frag (v2f i) : COLOR
            {
                //UNITY_PROJ_COORD(a)	给定一个 4 分量矢量，此宏返回一个适合投影纹理读取的纹理坐标。在大多数平台上，它直接返回给定值。
                //Linear01Depth(i)：通过深度纹理 i 给出高精度值时，返回相应的线性深度，范围在 0 到 1 之间。
               float depthVal = Linear01Depth(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.scrPos)).r)*_ProjectionParams.z;
               depthVal = saturate((depthVal-_DepthStart)/_DepthDis);
               //return  _FogColor*depthVal;
                
               fixed4 fogColor = _FogColor*depthVal;
               fixed4 col = tex2Dproj(_MainTex,i.scrPos);
               return lerp(col,fogColor,depthVal);                               
            }
            ENDCG
        }
    }
}
    