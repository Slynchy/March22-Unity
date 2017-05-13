Shader "Custom/TransitionShader" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_SecondaryTex("Texture", 2D) = "white" {}
		_TertiaryTex("Texture", 2D) = "white" {}
		_Progress("Progress", Range(-1, 1)) = -1
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
		#pragma surface surf NoLighting
		sampler2D _MainTex;
		sampler2D _SecondaryTex;
		sampler2D _TertiaryTex;
		float _Progress;
		float3 _AmbientLighting;

		struct Input {
			float2 uv_MainTex;
			float2 uv_SecondaryTex;
			float2 uv_TertiaryTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			float4 col = tex2D(_MainTex, IN.uv_MainTex);
			float4 destCol = tex2D(_SecondaryTex, IN.uv_SecondaryTex);
			float4 trCol = tex2D(_TertiaryTex, IN.uv_TertiaryTex);

			float temp = clamp(1 - (trCol.r - _Progress), 0, 1);
			float3 dest = lerp(col.rgb, destCol.rgb, temp);
			dest.r = clamp(dest.r, 0, 1);
			dest.g = clamp(dest.g, 0, 1);
			dest.b = clamp(dest.b, 0, 1);
			o.Albedo = dest;
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