using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Custom/RasterScroll")]
	public class RasterScroll : ImageEffectBase
	{
		[Range(0.0f, 100.0f)]
		public float frequency;    // 周波数
		[Range(0.0f, 1.0f)]
		public float power;        // ラスタースクロールの強さ
		[Range(0.0f, 100.0f)]
		public float speed;        // スクロールの速さ

		// シェーダープロパティID
		private int propertyIDFreq;
		private int propertyIDPower;
		private int propertyIDSpeed;

		protected override void Start()
		{
			base.Start();

			// シェーダープロパティID取得
			propertyIDFreq = Shader.PropertyToID("_Freq");
			propertyIDPower = Shader.PropertyToID("_Power");
			propertyIDSpeed = Shader.PropertyToID("_Speed");
		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			material.SetFloat(propertyIDFreq, frequency);
			material.SetFloat(propertyIDPower, power);
			material.SetFloat(propertyIDSpeed, speed);
			Graphics.Blit(source, destination, material);
		}
	}
}
