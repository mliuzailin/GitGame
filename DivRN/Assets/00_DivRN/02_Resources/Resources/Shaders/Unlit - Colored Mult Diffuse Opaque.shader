Shader "Unlit/Colored Mult Diffuse Opaque"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 100
		
		Tags
		{
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite On
			Fog { Mode Off }
			Offset -1, -1
			
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}

			SetTexture [_MainTex]
			{
				constantColor [_Color]
				Combine previous * constant
			}
		}
	}
}