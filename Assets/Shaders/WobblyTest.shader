Shader "Custom/WobblyTest" {
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_WobbleAmp("Wobble Amplitude", float) = 0.1
		_WobbleFreq("Wobble Frequency", float) = 1
		_HighlightColor("Highlight Color", Color) = (1, 1, 1, 1)
		_HighlightBlending("Highlight Blending", Range(0, 1)) = 0.2
		_HighlightDuration("Highlight Duration", float) = 2
		_HighlightTime("Highlight Time", float) = 0
	}
	SubShader 
	{	

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4	vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			float4 _HighlightColor;
			float _HighlightBlending;
			float _HighlightDuration;
			float _HighlightTime;
			float _WobbleAmp;
			float _WobbleFreq;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.x += sin(_WobbleFreq * (_Time.y + v.vertex.y)) * _WobbleAmp;
				o.vertex.y += cos(_WobbleFreq * (_Time.y + v.vertex.z)) * _WobbleAmp;
				return o;
			};

			float4 frag(v2f i) : SV_Target
			{
				return lerp(_Color, _HighlightColor, _HighlightBlending * saturate(1 - (_Time.y - _HighlightTime) / _HighlightDuration));
			};
			ENDCG
		}
	}
	FallBack "Diffuse"
}
