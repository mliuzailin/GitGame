// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//----------------------------------------------------------------------------
//	Unlit/Vertex Color Transparent
//	�^�C�g����ʂŎg�p���Ă���B
//	�w��J���[�ŕ`��B�A���t�@�̓e�N�X�`�����玝���Ă���B
//----------------------------------------------------------------------------
Shader "Unlit/Vertex Color Transparent"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags 
		{
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
		}

		Pass
		{
			Lighting Off
			ZWrite On
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha 
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;

			struct v2f
			{
				half4  pos   : SV_POSITION;
				float2 uv    : TEXCOORD0;
				fixed4 color : COLOR;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv  = v.texcoord;
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				//	�w��F
				half4 c = _Color;
				//	�e�N�X�`���A���t�@���g�p
				c.a = tex2D( _MainTex, i.uv ).a;

				return c;
			}
			ENDCG
		}
	}
}

