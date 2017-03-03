// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WobblyTest" {
	Properties
	{
		_MainTex("Tex (RGB)", 2D) = "white" {}
		_OutlineTex("Highlight (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_WobbleAmp("Wobble Amplitude", float) = 0.1
		_WobbleFreq("Wobble Frequency", float) = 1
		_HighlightColor("Highlight Color", Color) = (1, 1, 1, 1)
		_HighlightBlending("Highlight Blending", Range(0, 1)) = 0.2
		_HighlightDuration("Highlight Duration", float) = 2
		_HighlightTime("Highlight Time", float) = 0
		_ScanOrigin("Scan Origin", Vector) = (0, 0, 0, 0)
		_ScanLength("Scan Length", float) = 1
	}
	SubShader 
	{	

		Pass
		{
						
			Lighting Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex	: POSITION;
				float2 uv		: TEXCOORD0;
			};

			struct v2f {
				float4 vertex	: SV_POSITION;
				float2 uv		: TEXCOORD0;
				float3 wPos		: TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _OutlineTex;
			float4 _Color;
			float4 _HighlightColor;
			float _HighlightBlending;
			float _HighlightDuration;
			float _HighlightTime;
			float _WobbleAmp;
			float _WobbleFreq;
			
			float4 _ScanOrigin;
			float _ScanLength;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.x += sin(_WobbleFreq * (_Time.y + v.vertex.y)) * _WobbleAmp;
				o.vertex.y += cos(_WobbleFreq * (_Time.y + v.vertex.z)) * _WobbleAmp;
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.uv;
				return o;
			};

			float4 frag(v2f i) : SV_Target
			{
				float4 baseColor = tex2D(_MainTex, i.uv);				
				float4 color = saturate(baseColor * lerp(_Color, _HighlightColor, _HighlightBlending * saturate(1 - (_Time.y - _HighlightTime) / _HighlightDuration)));

				float4 outline = tex2D(_OutlineTex, i.uv);

				float progress = _ScanOrigin.w - distance(_ScanOrigin.xyz, i.wPos);
				float trailing = saturate(1 - progress / _ScanLength);

				if (progress > 0) {
					outline.a = outline.a * pow(trailing, 2);
					outline.rgb = outline.rbg * outline.a;
					color = saturate(color + outline);
				}
				return color;
			};
			ENDCG
		}
	}
	FallBack "Diffuse"
}
