Shader "Custom/16"
{
	Properties
	{
		_Texture1("Texture1", 2D) = "white" {}
		_Texture2("Texture2", 2D) = "white" {}
		_Emission("Emission", 2D) = "black" {}
		_Color("Color", Color) = (1,1,1,1)
		_AlphaCut("Alpha Cutout", Range(0,1)) = 0.0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Int) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Int) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling", Int) = 0
		[ToggleOff] _DepthTest("Depth Test", Float) = 1.0
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 0.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 0.0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "TransparentCutout" }
		LOD 200
		ZWrite[_DepthTest]
		Blend[_SrcBlend][_DstBlend]
		Cull[_Cull]

		CGPROGRAM
			#pragma surface surfaceFunction Standard fullforwardshadows alphatest:_AlphaCut
			#pragma target 3.0
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF

			struct Input
			{
				float2 uv_Texture1;
			};

			sampler2D _Texture1;
			fixed4 _Color;
			int _SrcBlend;
			int _DstBlend;

			void surfaceFunction(Input IN, inout SurfaceOutputStandard OUT)
			{
				fixed4 color = tex2D(_Texture1, IN.uv_Texture1) * _Color;
				if (_SrcBlend == 5 && _DstBlend == 1)
				{
					float alpha = color.r * 0.299f + color.g * 0.587f + color.b * 0.114f;
					color.a *= alpha;
				}
				OUT.Albedo = color.rgb;
				OUT.Alpha = color.a;
				OUT.Metallic = 0;
				OUT.Smoothness = 0;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
