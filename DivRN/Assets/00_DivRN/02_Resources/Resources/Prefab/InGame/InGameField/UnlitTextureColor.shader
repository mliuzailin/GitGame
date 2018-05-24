Shader "Unlit/Texture Color" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass{
			Lighting off
			SetTexture[_MainTex]{ 
				constantColor[_Color]
				combine constant lerp(texture) previous
			}
			SetTexture[_MainTex]{
				combine previous * texture
			}
		}
	}
}
