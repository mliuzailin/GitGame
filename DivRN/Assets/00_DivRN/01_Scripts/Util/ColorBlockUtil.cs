/**
 *  @file   ColorBlockUtil.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/19
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class ColorBlockUtil
{
    public static ColorBlock BUTTON_YELLOW = SetColor(ColorUtil.COLOR_YELLOW, ColorUtil.COLOR_YELLOW, ColorUtil.COLOR_GRAY, ColorUtil.COLOR_GRAY);
    public static ColorBlock BUTTON_PURPLE = SetColor(ColorUtil.COLOR_PURPLE, ColorUtil.COLOR_PURPLE, ColorUtil.COLOR_GRAY, ColorUtil.COLOR_GRAY);
    public static ColorBlock BUTTON_WHITE = SetColor(ColorUtil.COLOR_WHITE, ColorUtil.COLOR_WHITE, ColorUtil.COLOR_GRAY, ColorUtil.COLOR_GRAY);
    public static ColorBlock BUTTON_RED = SetColor(ColorUtil.COLOR_RED, ColorUtil.COLOR_RED, ColorUtil.COLOR_GRAY, ColorUtil.COLOR_GRAY);

    /// <summary>
    /// 選択カラーの設定
    /// </summary>
    /// <param name="n">通常のカラー</param>
    /// <param name="h">ハイライトカラー</param>
    /// <param name="d">無効時の色</param>
    /// <param name="p">（ボタンなど）押されたときのカラー</param>
    /// <returns></returns>
    public static ColorBlock SetColor(Color n, Color h, Color d, Color p)
    {
        ColorBlock block = ColorBlock.defaultColorBlock;
        block.normalColor = n;
        block.highlightedColor = h;
        block.disabledColor = d;
        block.pressedColor = p;

        return block;
    }

}
