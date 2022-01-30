Shader "Custom/HiddenShader"
{
	Properties
	{
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		ZWrite Off
		Blend Zero One

		CGPROGRAM
			#pragma surface surfaceFunction Standard noshadow
			#pragma target 3.0

			struct Input
			{
				float a;
			};

			void surfaceFunction(Input IN, inout SurfaceOutputStandard OUT)
			{
				fixed4 color = (0,0,0,0);
				OUT.Albedo = color.rgb;
				OUT.Alpha = color.a;
			}
		ENDCG
	}
}
