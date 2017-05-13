Shader "Custom/BackgroundShader" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Alpha("Alpha", Range(0, 1)) = 1
	}
	SubShader{
	Tags{
		"Queue" = "Transparent"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"IgnoreProjector" = "True"
	}
	ZWrite Off
	Blend One OneMinusSrcAlpha

	CGPROGRAM
	#pragma surface surf NoLighting
	sampler2D _MainTex;
	float _Alpha;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		float4 col = tex2D(_MainTex, IN.uv_MainTex);
		o.Alpha = _Alpha;
	}

	fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}

	ENDCG
	}
	FallBack "Diffuse"
}