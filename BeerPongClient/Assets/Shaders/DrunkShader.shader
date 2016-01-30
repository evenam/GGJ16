// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "Hidden/DrunkShader" {
		Properties{
			_MainTex("Base (RGB)", 2D) = "white" {}
		}

			SubShader{
			Pass{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_TexelSize;
		uniform float _Drunk; 

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f vert(appdata_img v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		float4 frag(v2f i) : SV_Target
		{
			float offx = sin(i.uv.y * _Drunk + _Time * _Drunk / 1.5) * .0045;
			float offy = sin(i.uv.x * _Drunk + _Time * _Drunk / 1.5) * .0045;
			float2 offset = i.uv;
			offset.x += offx;
			offset.x += offy;
			float4 color = tex2D(_MainTex, offset);
			return color;
		}
			ENDCG

		}
		}

			Fallback off

	}
