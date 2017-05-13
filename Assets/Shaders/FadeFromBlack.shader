Shader "Custom/FadeFromBlack" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Progress("Progress", Range(0, 2)) = 0
	}
	SubShader{
	Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
	LOD 200

	CGPROGRAM
	#pragma surface surf Standard fullforwardshadows alpha:fade
	#pragma target 3.0
	sampler2D _MainTex;
	float _Progress;
	fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) {
		half4 col = tex2D(_MainTex, IN.uv_MainTex);
		half3 white = half3(1, 1, 1);
		half3 black = half3(0, 0, 0);

		//int tempAlpha = alpha - speed * 5;
		o.Albedo = black;
		o.Alpha = 1 - (col.r - _Progress);
		
	}
	ENDCG
		}
			Fallback off
}