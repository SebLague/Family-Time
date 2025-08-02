Shader "Custom/Planet"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,0,0,1)
        _Color2 ("Color 2", Color) = (0,1,0,1)
        _Color3 ("Color 3", Color) = (0,0,1,1)
        _HMin ("Min y", Float) = 0
        _HMax ("Max y", Float) = 0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        float _HMin;
        float _HMax;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float h =length(IN.worldPos);
            float y = saturate((h - _HMin) / (_HMax - _HMin));
            fixed4 c1 = _Color1;
            fixed4 c2 = _Color2;
            fixed4 c3 = _Color3;
            
            float blend1 = saturate(1.0 - y * 2.0);
            float blend3 = saturate(y * 2.0 - 1.0);
            float blend2 = 1.0 - blend1 - blend3;

            fixed4 col = c1 * blend1 + c2 * blend2 + c3 * blend3;
            o.Albedo = col;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}