Shader "Unlit/AlphaGrad"
{
	Properties
	{
		_Color ("Colour", Color) = (1,1,1,1)
		_POW("POW", Float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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

			float _POW;
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float a = pow(abs(i.uv.y), _POW);
				return float4(_Color.rgb, _Color.a * a);
			}
			ENDCG
		}
	}
}