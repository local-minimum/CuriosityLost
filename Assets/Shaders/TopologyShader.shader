Shader "Custom/TopologyShader" {
	Properties{
		_ColorLow("Color Low", Color) = (1,1,1,1)
		_ColorZero("Color Zero", Color) = (1,1,1,1)
		_ColorHigh("Color High", Color) = (1,1,1,1)
		_HeightColorMin("Color height low", Float) = -1
		_HeightColorMax("Color height high", Float) = 5

		_TexMixing("Texture Mixing", Range(0, 1)) = 1

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		#pragma vertex vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 localPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _ColorLow;
		fixed4 _ColorHigh;
		fixed4 _ColorZero;
		half _HeightColorMin;
		half _HeightColorMax;
		half _TexMixing;

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = v.vertex.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c;
			if (IN.localPos.y < 0) {
				c = lerp(_ColorLow, _ColorZero, IN.localPos.y / _HeightColorMin);
			}
			else {
				c = lerp(_ColorZero, _ColorHigh, IN.localPos.y / _HeightColorMax);
			}
			
			c = saturate(lerp(c, tex2D(_MainTex, IN.uv_MainTex) * c, _TexMixing));

			o.Albedo = c.rgb;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
