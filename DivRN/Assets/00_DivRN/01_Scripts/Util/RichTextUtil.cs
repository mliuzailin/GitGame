/**
 * 	@file	RichTextUtil.cs
 *	@brief	リッチテキストのタグの付加
 *	@author Developer
 *	@date	2016/11/15
 */

using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;

public static class RichTextUtil
{
    /// <summary>
    /// 色のタグを付加する
    /// </summary>
    /// <param name="str"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorTag(this string str, Color color)
    {
        return string.Format("<color={0}>{1}</color>", HexColor.FormColorString(color), str);
    }

    /// <summary>
    /// 太字のタグを付加する
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string BoldTag(this string str)
    {
        return string.Format("<b>{0}</b>", str);
    }

    /// <summary>
    /// イタリックのタグを付加する
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ItalicTag(this string str)
    {
        return string.Format("<i>{0}</i>", str);
    }

    /// <summary>
    /// サイズのタグを付加する
    /// </summary>
    /// <param name="str"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string SizeTag(this string str, int size)
    {
        return string.Format("<size={0}>{1}</size>", size, str);
    }

    /// <summary>
    /// フォント変更のタグを付加する
    /// </summary>
    /// <param name="str"></param>
    /// <param name="font_name"></param>
    /// <returns></returns>
    public static string FontTag(this string str, string font_name)
    {
        return string.Format("<font=\"{1}\">{0}</font>", str, font_name);
    }

    /// <summary>
    /// フォント変更のタグを付加する
    /// </summary>
    /// <param name="str"></param>
    /// <param name="font_name"></param>
    /// <param name="material_name"></param>
    /// <returns></returns>
    public static string FontTag(this string str, string font_name, string material_name)
    {
        return string.Format("<font=\"{1}\" material=\"{2}\">{0}</font>", str, font_name, material_name);
    }

    /// <summary>
    /// 禁則処理無効のタグを付加する
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    static public string NoLineBreakTag(this string str)
    {
        return string.Format("<nobr>{0}</nobr>", str);
    }

    /// <summary>
    /// タグを除去する
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static public string NoRichText(this string str)
    {
        return Regex.Replace(str, "<.*?>", string.Empty, RegexOptions.Singleline);
    }

    /// <summary>
    /// 全角スペースをspaceタグに置き換える
    /// </summary>
    /// <param name="str"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    static public string ReplaceSpaceTag(this string str, int size)
    {
        if (str == null)
        {
            return null;
        }

        string tag = string.Format("<space={0}em>", size);
        return str.Replace("　", tag);
    }
}
