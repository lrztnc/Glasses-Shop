"TextMeshPro/Distance Field Clean" {
Properties {
	_MainTex("Font Atlas", 2D) = "white" {}
	_FaceColor("Face Color", Color) = (1,1,1,1)
	_OutlineColor("Outline Color", Color) = (0,0,0,1)
	_OutlineWidth("Outline Thickness", Range(0, 1)) = 0
	_OutlineSoftness("Outline Softness", Range(0,1)) = 0
	_FaceDilate("Face Dilate", Range(-1,1)) = 0
	_GradientScale("Gradient Scale", float) = 5.0
	_Sharpness("Sharpness", Range(-1,1)) = 0
	_ScaleRatioA("Scale RatioA", float) = 1
}

SubShader {
	Tags {
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Cull Off
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	Blend One OneMinusSrcAlpha
	ColorMask RGBA

	Pass {
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _FaceColor;
		float4 _OutlineColor;
		float _OutlineWidth;
		float _OutlineSoftness;
		float _FaceDilate;
		float _GradientScale;
		float _Sharpness;
		float _ScaleRatioA;

		struct appdata {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float4 color : COLOR;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 color : COLOR;
		};

		v2f vert(appdata v) {
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			o.color = v.color;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target {
			float d = tex2D(_MainTex, i.uv).a;

			float scale = _GradientScale * (_Sharpness + 1);
			float weight = (_FaceDilate) * _ScaleRatioA * 0.5;
			float bias = 0.5 - weight + (0.5 / scale);
			float sd = (bias - d) * scale;

			float outline = (_OutlineWidth * _ScaleRatioA) * scale;
			float softness = (_OutlineSoftness * _ScaleRatioA) * scale;

			// Step blending
			float alpha = saturate(sd + outline * 0.5);
			float softnessFactor = smoothstep(0.0, softness, sd + outline * 0.5);

			fixed4 face = _FaceColor * i.color;
			fixed4 outline = _OutlineColor * i.color;

			fixed4 color = lerp(outline, face, softnessFactor);
			color.a *= i.color.a;

			// Elimina riquadri: forza opacità piena
			color.a = 1;

			// Clipping alpha (facoltativo)
			clip(color.a - 0.001);

			return color;
		}
		ENDCG
	}
}

Fallback "TextMeshPro/Mobile/Distance Field"
CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}