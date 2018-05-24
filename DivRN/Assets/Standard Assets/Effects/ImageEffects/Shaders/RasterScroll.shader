// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/RasterScroll"
{
	Properties{
		_MainTex("Source", 2D) = "white" {}
		_Freq("Frequency", Float) = 0
		_Power("Power", Float) = 0
		_Speed("Speed", Float) = 0
	}
	SubShader{
		ZTest Always
		Cull Off
		ZWrite Off
		Fog{ Mode Off }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct v2f {
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed2 spos : TEXCOORD1;
			};

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				o.spos = ComputeScreenPos(o.pos);   // モデルビュープロジェクション変換された座標を画面上に変換する
				return o;
			}

			sampler2D _MainTex;
			fixed _Freq;
			fixed _Power;
			fixed _Speed;

			fixed4 frag(v2f i) : SV_TARGET{
				fixed2 uv = i.uv;
				uv.x = fmod(abs(uv.x + sin(i.spos.y * _Freq + _Time.y * _Speed) * _Power), 1);  // uvの横方向だけ画面のY座標に合わせて一定周期でずらしていく
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
	FallBack Off
}
