using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public static class TextUtil
{
    /**
     * @brief 文字数を指定し、そこから表示が切れる場合は「...」表示
     * @return 整形した文字列
     */
    static public string TrimSymbolText(string strValue, int unValueNum)
    {

        //指定されたテキスト内の色エンコード・シンボル除外
        string strValueEdit = strValue;

        //半角考慮したテキストサイズを取得
        int nTextSize = GetTextSize(strValueEdit);

        if (unValueNum > 0)
        {
            if (nTextSize > unValueNum)
            {
                //文字数に差分があれば、カラーコード周りの表示設定を調整
                if (strValueEdit.Length != strValue.Length)
                {
                    strValueEdit = GetTextSymbolText(strValue, unValueNum - 1);
                }
                else
                {
                    double dTextSizeCnt = 0;
                    int nTextNum = 0;

                    for (int i = 0; i < strValue.Length; i++)
                    {
                        if (IsHankakuChk(strValue[i].ToString()))
                        {
                            //半角文字の場合
                            dTextSizeCnt += GetHankakuSize(strValue[i].ToString());
                        }
                        else
                        {
                            ++dTextSizeCnt;
                        }

                        if (unValueNum < dTextSizeCnt)
                        {
                            nTextNum = i;
                            break;
                        }
                    }

                    //指定文字列までカット
                    strValueEdit = strValue.Substring(0, nTextNum - 1);
                    strValueEdit += "...";
                }
            }
            else
            {
                strValueEdit = strValue;
            }
        }
        else
        {
            strValueEdit = strValue;
        }

        return strValueEdit;
    }

    /**
     * @brief 文字数取得（半角考慮）
     */
    static public int GetTextSize(string text)
    {
        double dTextSizeCnt = 0; //文字数カウント用（半角来た時は+0.5する）
        int nTextNum = 0; //文字数(半角文字は２文字で「1」とする)

        if (text != null)
        {
            text = text.Replace("\\n", "\n");

            for (int i = 0, imax = text.Length; i < imax;)
            {
                if (IsHankakuChk(text[i].ToString()))
                {
                    //半角文字の場合
                    dTextSizeCnt += GetHankakuSize(text[i].ToString());
                }
                else
                {
                    ++dTextSizeCnt;
                }

                i++;
            }

            nTextNum = (int)Math.Ceiling(dTextSizeCnt);
        }

        return nTextNum;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	指定文字数分の文字列を成形して返却
		@note	末尾に「...」を付与。カラーコードなど途中で切れる場合を考慮。
	*/
    //----------------------------------------------------------------------------
    static public string GetTextSymbolText(string strTextBase, int nTextNum)
    {
        string strText = "";
        double dTextSizeCutCnt = 0; //文字数カウント用（半角来た時は+0.5する）
        double dTextSizeCnt = 0; //文字数カウント用（半角来た時は+0.5する）
        int nTextCutNum = 0; //最終的に切り取る文字数

        //カラーコードの指定があったか
        bool bColorStart = false;
        bool bColorEnd = false;

        string strChkText = strTextBase;

        if (strChkText != null)
        {
            strChkText = strChkText.Replace("\\n", "\n");

            for (int i = 0, imax = strChkText.Length; i < imax;)
            {
                char c = strChkText[i];

                if (c == '[')
                {
                    int retVal = ParseSymbol(strChkText, i, null, false);

                    if (retVal > 0)
                    {
                        if (retVal == 8) { bColorStart = true; bColorEnd = false; } else if (retVal == 3) { bColorEnd = true; }

                        strChkText = strChkText.Remove(i, retVal);
                        imax = strChkText.Length;

                        //端折られた分をカウントアップしとく
                        dTextSizeCutCnt += retVal;

                        continue;
                    }
                }

                //今何文字目のチェックかをカウント
                if (IsHankakuChk(strChkText[i].ToString()))
                {
                    //半角文字の場合
                    dTextSizeCutCnt += GetHankakuSize(strChkText[i].ToString());
                    dTextSizeCnt += GetHankakuSize(strChkText[i].ToString());
                }
                else
                {
                    ++dTextSizeCutCnt;
                    ++dTextSizeCnt;
                }

                //指定文字数オーバーしたら終了
                if (nTextNum <= dTextSizeCnt)
                {
                    //nTextCutNum = (int)Math.Ceiling(dTextSizeCutCnt);
                    nTextCutNum = (int)Math.Floor(dTextSizeCutCnt);
                    break;
                }

                ++i;
            }
        }

        strText = strTextBase.Substring(0, nTextCutNum);

        //カラーコード指定あるけど終わってない場合
        if (bColorStart == true && bColorEnd == false)
        {
            strText += "[-]";
        }

        strText += "...";

        return strText;
    }

    /**
     * @brief 半角チェック
     */
    public static bool IsHankakuChk(string str)
    {
        Encoding sjisEnc = Encoding.GetEncoding("utf-8");
        int num = sjisEnc.GetByteCount(str);
        return num == str.Length;
    }

    /**
     * @brief 半角英数字かをチェック
     */
    public static bool IsEnglishOrNumChk(string str)
    {
        return (Regex.Match(str, "^[a-zA-Z0-9]+$")).Success;
    }

    /**
     * @brief 半角文字文字サイズ取得
     */
    public static float GetHankakuSize(string str)
    {
        float fHankaku = 0.6f;
        if (IsEnglishOrNumChk(str))
        {
            fHankaku = 0.7f;
        }
        return fHankaku;
    }

    /**
     * @brief NGUIのカラータグの文字数を返す
     * return カラータグの文字数 [FFAA00]=8 [-]=3 それ以外は0
     */
    static public int ParseSymbol(string text, int index, List<Color> colors, bool premultiply)
    {
        int length = text.Length;

        if (index + 2 < length)
        {
            if (text[index + 1] == '-')
            {
                if (text[index + 2] == ']')
                {
                    if (colors != null && colors.Count > 1) colors.RemoveAt(colors.Count - 1);
                    return 3;
                }
            }
            else if (index + 7 < length)
            {
                if (text[index + 7] == ']')
                {
                    if (colors != null)
                    {
                        Color c = HexColor.ToColor(text.Substring(index + 1, 6));

                        if (HexColor.FormColorString(c).Substring(1, 6) != text.Substring(index + 1, 6))
                            return 0;

                        c.a = colors[colors.Count - 1].a;
                        if (premultiply && c.a != 1f)
                            c = Color.Lerp(ColorUtil.COLOR_INVISIBLE, c, c.a);

                        colors.Add(c);
                    }
                    return 8;
                }
            }
        }
        return 0;
    }

    /// <summary>
    /// 文字数を指定し、そこから表示が切れる場合は「...」表示
    /// </summary>
    /// <param name="count">文字数</param>
    /// <returns>文字数 + ...のテキスト</returns>
    static public string CliLengthText(string str, int length)
    {
        string cnvStr = str;
        if (length != 0)
        {
            if (cnvStr != null
            && cnvStr.Length > length
            )
            {
                cnvStr = cnvStr.Substring(0, length);
                cnvStr += "...";
            }
        }

        return cnvStr;
    }

    /// <summary>
    /// 数値を文字配列に変換する
    /// </summary>
    /// <param name="num">数値</param>
    /// <returns>要素数0には１桁目の値が格納</returns>
    static public List<uint> GetNumberList(int num)
    {
        int digit = num;
        List<uint> number = new List<uint>();
        if (digit > 0)
        {
            while (digit != 0)
            {
                number.Add((uint)(digit % 10));
                digit = digit / 10;
            }
        }
        else
        {
            number.Add(0);
        }

        return number;
    }

    /// <summary>
    /// 改行コードを削除する
    /// </summary>
    /// <returns></returns>
    static public string RemoveNewLine(string _str)
    {
        if (_str == null)
        {
            return null;
        }

        return _str.Replace("\r", "").Replace("\n", "");
    }
}
