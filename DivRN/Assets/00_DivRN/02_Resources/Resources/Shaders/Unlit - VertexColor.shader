// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Vertex Color"
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags 
		{
			"Queue"="Geometry"
			"RenderType"="Opaque"
			"IgnoreProjector"="True"
		}
		
		Pass
		{
			
			Lighting Off
			ZWrite On
			Fog { Mode Off }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"

			fixed4 _Color;
			
			struct v2f {
				half4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};
			
			v2f vert (appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.color = _Color;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR {
				 return i.color;
			}
			ENDCG
			
			
			
		}
	}
}


