Shader "Hidden/cat"
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // https://arxiv.org/pdf/1711.10662.pdf
                // https://en.wikipedia.org/wiki/LMS_color_space
                // RGB to LMS
                float3x3 RGBtoLMS = float3x3(17.8824, 3.45570, 0.0300,
                                             43.5161, 27.1554, 0.1843,
                                             4.11940, 3.8671, 1.4671);

                float3 LMS = mul(RGBtoLMS, col.rgb);
                LMS.x *= 0.7;


                float3x3 LMStoRGB = float3x3(0.0809446, -0.0102483, -0.000367778,
                               -0.1305040, 0.0540190, -0.004117350,
                               0.1167140, -0.1136120, 0.693502000);


                float3 rgbOut = mul(LMStoRGB, LMS);

                float3 grey = dot(col, float3(0.0, 0.8, 0.2));
                rgbOut = lerp(rgbOut, grey, 0.5);


                //float gray = dot(col, float3(0.0, 0.8, 0.2));
                // float3 catColor = float3(col.r * 0.4, col.g, col.b);


                return float4(rgbOut, 1);
            }
            ENDCG
        }
    }
}