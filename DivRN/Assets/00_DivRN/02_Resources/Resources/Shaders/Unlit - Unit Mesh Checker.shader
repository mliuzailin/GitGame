Shader "Unlit/Unit Mesh Checker"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
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
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			SetTexture [_MainTex] { combine texture } 
		}
	}
}


