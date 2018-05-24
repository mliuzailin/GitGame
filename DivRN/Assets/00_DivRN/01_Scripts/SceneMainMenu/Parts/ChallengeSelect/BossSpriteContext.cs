using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class BossSpriteContext : M4uContext
{
    M4uProperty<Sprite> spriteImage = new M4uProperty<Sprite>();
    public Sprite SpriteImage { get { return spriteImage.Value; } set { spriteImage.Value = value; } }

    M4uProperty<Color> spriteColor = new M4uProperty<Color>();
    public Color SpriteColor { get { return spriteColor.Value; } set { spriteColor.Value = value; } }

    M4uProperty<float> spriteSizeX = new M4uProperty<float>();
    public float SpriteSizeX { get { return spriteSizeX.Value; } set { spriteSizeX.Value = value; } }

    M4uProperty<float> spriteSizeY = new M4uProperty<float>();
    public float SpriteSizeY { get { return spriteSizeY.Value; } set { spriteSizeY.Value = value; } }

    private bool m_bSetup = false;
    public bool IsSetup { get { return m_bSetup; } }

    public BossSpriteContext()
    {
        SpriteImage = null;
        SpriteColor = new Color(1, 1, 1, 0);
    }

    public AssetBundler setup(uint boss_id)
    {
        m_bSetup = false;
        AssetBundler assetBundler = AssetBundler.Create();
        assetBundler.SetAsUnitTexture(
            boss_id,
            (asset) =>
            {
                var tex2D = asset.GetUncompressedTexture2D();
                if (tex2D != null)
                {
                    SpriteImage = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
                    SpriteColor = new Color(1, 1, 1, 1);
                    SpriteSizeX = tex2D.GetUnitTextureWidth();
                    SpriteSizeY = tex2D.GetUnitTextureHeight();
                }
                m_bSetup = true;
            });
        return assetBundler;
    }
}
