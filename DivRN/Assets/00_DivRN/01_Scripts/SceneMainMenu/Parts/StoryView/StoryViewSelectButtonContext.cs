/**
 *  @file   StoryViewSelectButtonContext.cs
 *  @brief
 *  @author Developer
 *  @date   2018/02/01
 */

using UnityEngine;
using System.Collections;
using M4u;

public class StoryViewSelectButtonContext : M4uContext
{
    M4uProperty<string> buttonText = new M4uProperty<string>();
    /// <summary>テキスト</summary>
    public string ButtonText
    {
        get
        {
            return buttonText.Value;
        }
        set
        {
            buttonText.Value = value;
        }
    }

    M4uProperty<bool> isEnableSelectButton = new M4uProperty<bool>(false);
    /// <summary>ボタンの選択状態</summary>
    public bool IsEnableSelectButton { get { return isEnableSelectButton.Value; } set { isEnableSelectButton.Value = value; } }

    M4uProperty<Sprite> buttonType = new M4uProperty<Sprite>();
    /// <summary>枠のタイプ</summary>
    public Sprite ButtonType { get { return buttonType.Value; } set { buttonType.Value = value; } }

}
