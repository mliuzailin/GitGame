/**
 *  @file   StoryViewContext.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/07
 */

using UnityEngine;
using System.Collections;
using M4u;

public class StoryViewContext : M4uContext
{
    M4uProperty<float> mainContentAlpha = new M4uProperty<float>();
    public float MainContentAlpha
    {
        get
        {
            return mainContentAlpha.Value;
        }
        set
        {
            mainContentAlpha.Value = value;
        }
    }

    M4uProperty<float> scenerioContentAlpha = new M4uProperty<float>();
    public float ScenerioContentAlpha
    {
        get
        {
            return scenerioContentAlpha.Value;
        }
        set
        {
            scenerioContentAlpha.Value = value;
        }
    }

    M4uProperty<Sprite> backGroundImage = new M4uProperty<Sprite>();
    /// <summary>背景</summary>
    public Sprite BackGroundImage
    {
        get
        {
            return backGroundImage.Value;
        }
        set
        {
            backGroundImage.Value = value;
            IsActiveBackGroundImage = (value != null);
        }
    }

    M4uProperty<bool> isActiveBackGroundImage = new M4uProperty<bool>();
    /// <summary>背景の表示・非表示</summary>
    public bool IsActiveBackGroundImage
    {
        get
        {
            return isActiveBackGroundImage.Value;
        }
        set
        {
            isActiveBackGroundImage.Value = value;
        }
    }

    M4uProperty<Color> pauseCursorColor = new M4uProperty<Color>(Color.white);
    /// <summary>カーソルの色</summary>
    public Color PauseCursorColor
    {
        get
        {
            return pauseCursorColor.Value;
        }
        set
        {
            pauseCursorColor.Value = value;
        }
    }


    M4uProperty<int> textBoxNum = new M4uProperty<int>(0);
    public int TextBoxNum
    {
        get
        {
            return textBoxNum.Value;
        }
        set
        {
            textBoxNum.Value = value;
        }
    }


    M4uProperty<MasterDataDefineLabel.StoryCharFocus> textBoxFocus = new M4uProperty<MasterDataDefineLabel.StoryCharFocus>();
    public MasterDataDefineLabel.StoryCharFocus TextBoxFocus
    {
        get
        {
            return textBoxFocus.Value;
        }
        set
        {
            textBoxFocus.Value = value;
        }
    }

    M4uProperty<bool> isEnableSelectButton = new M4uProperty<bool>(false);
    /// <summary>ボタンの選択状態</summary>
    public bool IsEnableSelectButton { get { return isEnableSelectButton.Value; } set { isEnableSelectButton.Value = value; } }

    M4uProperty<bool> isEnableSelectMode = new M4uProperty<bool>(false);
    /// <summary>ボタンの選択状態</summary>
    public bool IsEnableSelectMode { get { return isEnableSelectMode.Value; } set { isEnableSelectMode.Value = value; } }

    M4uProperty<float> backGroundAlpha = new M4uProperty<float>();
    public float BackGroundAlpha { get { return backGroundAlpha.Value; } set { backGroundAlpha.Value = value; } }

    M4uProperty<float> topMaskAlpha = new M4uProperty<float>();
    public float TopMaskAlpha { get { return topMaskAlpha.Value; } set { topMaskAlpha.Value = value; } }

    M4uProperty<float> bottomMaskAlpha = new M4uProperty<float>();
    public float BottomMaskAlpha { get { return bottomMaskAlpha.Value; } set { bottomMaskAlpha.Value = value; } }
}
