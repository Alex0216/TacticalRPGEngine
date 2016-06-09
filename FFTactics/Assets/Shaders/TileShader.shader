Shader "Custom/TileShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Blink ("Freq", Float ) = 0
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 _Color;
		float _Blink;

		struct Input {
			float2 uv_MainTex;
		};


		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
