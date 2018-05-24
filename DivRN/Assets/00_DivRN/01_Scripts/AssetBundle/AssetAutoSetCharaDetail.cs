using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetAutoSetCharaDetail : AssetAutoSetCharaMesh
{
    private float m_ImageScale = 1.0f;
    public float ImageScale { get { return m_ImageScale; } set { m_ImageScale = value; } }

    protected override void setMainTexture(Texture2D cTexture, Vector3 scale)
    {
        m_CharaImage.sprite = Sprite.Create(cTexture, new Rect(0, 0, cTexture.width, cTexture.height), Vector2.zero);
        m_CharaImage.color = new Color(1, 1, 1, 1);
        m_CharaImage.enabled = true;
        m_CharaRect.sizeDelta = new Vector2(cTexture.GetUnitTextureWidth() * m_ImageScale, m_CharaRect.sizeDelta.y);
    }

    protected override void setShadowTexture(Texture2D cTexture, Vector3 scale, Material material)
    {
        m_ShadowImage.sprite = Sprite.Create(cTexture, new Rect(0, 0, cTexture.width, cTexture.height), Vector2.zero);
        m_ShadowImage.color = new Color(1, 1, 1, 1);
        m_ShadowImage.material = Instantiate(material) as Material;
        m_ShadowImage.enabled = true;
        m_ShadowRect.sizeDelta = new Vector2(cTexture.GetUnitTextureWidth() * m_ImageScale, m_ShadowRect.sizeDelta.y);
    }

    public void setAlpha(bool sw)
    {
        if (sw == true)
        {
            m_CharaImage.color = Color.white;
            m_ShadowImage.color = Color.white;
        }
        else
        {
            m_CharaImage.color = Color.clear;
            m_ShadowImage.color = Color.clear;
        }
    }
}
