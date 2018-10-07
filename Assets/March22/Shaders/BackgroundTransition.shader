Shader "Custom/BackgroundTransitionShader" {
	Properties{
		_MainTex("Texture", 2D) = "black" {}
		_SecondaryTex("Texture", 2D) = "white" {}
		_Progress("Progress", Range(-1, 1)) = -1
		_InOrOut("InOrOut", Range(0, 1)) = 0
		_Alpha("Alpha", Range(0, 1)) = 1
		_AmbientLighting("AmbientLighting", COLOR) = (1,1,1,1)
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
#pragma surface surf NoLighting alpha
		sampler2D _MainTex;
	sampler2D _SecondaryTex;
	sampler2D _TertiaryTex;
	float _Progress;
	float3 _AmbientLighting;
	float _InOrOut; // 0 == in, 1 == out
	float _Alpha;

	struct Input {
		float2 uv_MainTex;
		float2 uv_SecondaryTex;
		float2 uv_TertiaryTex;
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		float4 col = tex2D(_MainTex, IN.uv_MainTex);
		float4 destCol = tex2D(_SecondaryTex, IN.uv_SecondaryTex);

		float temp;
		if (_InOrOut == 0)
			temp = clamp(1 - (destCol.r - _Progress), 0, 1);
		else
			temp = clamp(1 - ((1 + (destCol.r * -1)) - _Progress), 0, 1);

		// 1 - (0.33f - 0.5f) == 1 - (-0.17f) == 1.17f
		// 1 - (0.33f - 0.2f) == 1 - (0.13f)  == 0.87f
		// 1 - ( ( 1 + ( 0.33f * -1 ) ) - 0.2f ) 
		// 1 - ( ( 1 + -0.33f ) - 0.2f )
		// 1 - ( (   0.66f    ) - 0.2f )
		// 1 - ( 0.46f )
		// 0.54f

		//dest.r = clamp(dest.r, 0, 1);
		//dest.g = clamp(dest.g, 0, 1);
		//dest.b = clamp(dest.b, 0, 1);
		//dest.a = clamp(dest.a, 0, 1);
		o.Albedo = col.rgb;
		o.Alpha = temp * _Alpha;
	}

	fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.r = s.Albedo.r * (1 - _AmbientLighting.r);
		c.g = s.Albedo.g * (1 - _AmbientLighting.g);
		c.b = s.Albedo.b * (1 - _AmbientLighting.b);
		c.a = s.Alpha;
		return c;
	}

	ENDCG
	}
		FallBack "Diffuse"
}


