/**
 *  @file   StoryViewCharaPanelContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/29
 */

using UnityEngine;
using System.Collections;
using M4u;

public class StoryViewCharaPanelContext : M4uContext
{
    /// <summary>キャラカットインが隠れる位置</summary>
    public Vector2 m_CharaHidePosition;

    M4uProperty<bool> isActiveCharaImage = new M4uProperty<bool>(true);
    /// <summary>キャラクター画像の表示・非表示</summary>
    public bool IsActiveCharaImage
    {
        get
        {
            return isActiveCharaImage.Value;
        }
        set
        {
            isActiveCharaImage.Value = value;
        }
    }

    M4uProperty<Texture2D> charaTexture = new M4uProperty<Texture2D>();
    /// <summary>キャラクター画像</summary>
    public Texture2D CharaTexture
    {
        get
        {
            return charaTexture.Value;
        }
        set
        {
            charaTexture.Value = value;
        }
    }

    M4uProperty<Texture2D> charaTexture_mask = new M4uProperty<Texture2D>();
    /// <summary>キャラクター画像</summary>
    public Texture2D CharaTexture_mask
    {
        get
        {
            return charaTexture_mask.Value;
        }
        set
        {
            charaTexture_mask.Value = value;
        }
    }

    M4uProperty<Sprite> charaBackGround = new M4uProperty<Sprite>();
    /// <summary>キャラ背景</summary>
    public Sprite CharaBackGround
    {
        get
        {
            return charaBackGround.Value;
        }
        set
        {
            charaBackGround.Value = value;
        }
    }

    M4uProperty<bool> isActiveCharaBackGround = new M4uProperty<bool>(false);
    /// <summary>背景枠の表示・非表示</summary>
    public bool IsActiveCharaBackGround
    {
        get
        {
            return isActiveCharaBackGround.Value;
        }
        set
        {
            isActiveCharaBackGround.Value = value;
        }
    }

    M4uProperty<Rect> charaUV = new M4uProperty<Rect>(new Rect(0, 0, 1, 1));
    /// <summary>キャラクターのUV座標</summary>
    public Rect CharaUV
    {
        get
        {
            return charaUV.Value;
        }
        set
        {
            charaUV.Value = value;

        }
    }

    M4uProperty<Color> charaImageColor = new M4uProperty<Color>(Color.white);
    /// <summary>キャラクター画像の色</summary>
    public Color CharaImageColor
    {
        get
        {
            return charaImageColor.Value;
        }
        set
        {
            charaImageColor.Value = value;
        }
    }

    M4uProperty<float> charaImagePosX = new M4uProperty<float>();
    /// <summary>キャラクターのスライドイン用座標</summary>
    public float CharaImagePosX
    {
        get
        {
            return charaImagePosX.Value;
        }
        set
        {
            charaImagePosX.Value = value;
        }
    }

    M4uProperty<string> charaNameText = new M4uProperty<string>();
    /// <summary>名前</summary>
    public string CharaNameText
    {
        get
        {
            return charaNameText.Value;
        }
        set
        {
            charaNameText.Value = value;
        }
    }

    M4uProperty<bool> isActiveCharaName = new M4uProperty<bool>(true);
    /// <summary>名前の表示・非表示</summary>
    public bool IsActiveCharaName
    {
        get
        {
            return isActiveCharaName.Value;
        }
        set
        {
            isActiveCharaName.Value = value;
        }
    }

    M4uProperty<bool> isActiveFocus = new M4uProperty<bool>(true);
    /// <summary>フォーカス</summary>
    public bool IsActiveFocus
    {
        get
        {
            return isActiveFocus.Value;
        }
        set
        {
            if (value)
            {
                CharaImageColor = Color.white;

            }
            else
            {
                CharaImageColor = Color.gray;
            }
            isActiveFocus.Value = value;
        }
    }

}
