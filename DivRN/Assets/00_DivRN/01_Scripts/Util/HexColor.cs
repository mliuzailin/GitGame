/**
 * 	@file	HexColor.cs
 *	@brief	Hexカラーを扱う
 *	@author Developer
 *	@date	2016/02/22
 */

using UnityEngine;

[System.Serializable]
public struct HexColor
{
    /// <summary>16進数の色情報 #RGBA.</summary>
    public string hex;

    public HexColor(string hex)
    {
        this.hex = FromatHexString(hex);
    }

    public HexColor(Color color)
    {
        HexColor tmp = FromColor(color);
        this.hex = tmp.hex;
    }

    /**
     * <summary>16進数の文字列を整形する.</summary>
     */
    static string FromatHexString(string hex)
    {
        hex = hex.TrimStart('#');
        string tmp = hex.ToUpper();

        if (tmp == null || tmp.Length < 6)
        {
            tmp = "FFFFFFFF";
        }
        else if (tmp.Length < 8)
        {
            tmp = tmp.Insert(6, "FF");
        }
        tmp = tmp.Substring(0, 8);
        tmp = tmp.Insert(0, "#");

        return tmp;
    }

    /**
     * <summary>16進数の文字列をRGBカラーに変換する.</summary>
     */
    public static Color ToColor(string hex)
    {
#if true
        Color color = Color.white;

        if (!ColorUtility.TryParseHtmlString(FromatHexString(hex), out color))
        {
            return Color.white;
        }

        return color;
#else
        float r, g, b, a;
        r = g = b = a = 0;
        hex = hex.TrimStart('#');

        if (hex == null || hex.Length < 6) {
            return new Color();
        } else if (hex.Length < 8) {
            r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            a = 255.0f;
        } else {
            r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            a = int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }

        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
#endif
    }

    /**
     * <summary>HexカラーをRGBカラーに変換する.</summary>
     */
    public static Color ToColor(HexColor hexColor)
    {
        return ToColor(hexColor.hex);
    }

    /**
     * <summary>RGBカラーに変換する.</summary>
     */
    public Color ToColor()
    {
        return ToColor(this.hex);
    }

    /**
     * <summary>RGBカラーをHexカラーに変換する.</summary>
     */
    public static HexColor FromColor(Color color)
    {
        HexColor hexcolor = new HexColor();
        string hex = FormColorString(color);
        hexcolor.hex = hex;

        return hexcolor;
    }

    /**
     * <summary>RGBカラーをHexカラーの文字列に変換する.</summary>
     * <returns>#FFAA11FFのような文字列</returns>
     */
    public static string FormColorString(Color color)
    {
        string hex = string.Format("#{0}{1}{2}{3}",
         ((int)(color.r * 255)).ToString("X2"),
         ((int)(color.g * 255)).ToString("X2"),
         ((int)(color.b * 255)).ToString("X2"),
         ((int)(color.a * 255)).ToString("X2"));

        return hex;
    }

}