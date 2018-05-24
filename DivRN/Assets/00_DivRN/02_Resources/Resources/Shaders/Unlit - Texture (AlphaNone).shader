Shader "Unlit/Texture (AlphaNone)"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Tags
		{
			"RenderType"="Opaque"
		}
		
		Pass
		{
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			SetTexture [_MainTex] { combine texture } 
		}
	}
}


