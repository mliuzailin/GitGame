/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ColorUtil.cs
	@brief	アプリ内のカラー統一関連ユーティリティ
	@author Developer
	@date 	2013/07/05
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	カラー定義
	
	@note	インスペクタから参照している可能性があるので、追加の際には後付けを厳守してください。
 *			[2015/9/18 v300使用カラー]
 *			TODO: 現在はTextのリッチテキストのタグを使ってMasterDataTextDefinitionに設定しているラベルが多いので、整理が必要 Developer 2017/08/23
 *			
 *			LABEL_WHITE
 *			LABEL_YELLOW（リンク、ノーマルスキル等）
 *			LABEL_YELLOW2（スタミナ、パッシブスキル等）
 *			LABEL_YELLOW3（リンク以外、ID表示など）
 *			LABEL_PURPLE（トップ見出し等）
 *			LABEL_PURPLE2（リーダースキル等）
 *			LABEL_LIGHT_BLUE（ステ上昇系）
 *			LABEL_PINK（アクティブスキル等）
 *			LABEL_RED
 *			LABEL_RED2（EXP）
 *			LABEL_GREEN2（サポート等）
*/
//----------------------------------------------------------------------------
public enum APP_COLOR
{

    LABEL_WHITE,        //!< カラータイプ：ラベル系：白
    LABEL_YELLOW,       //!< カラータイプ：ラベル系：黄
    LABEL_YELLOW2,      //!< カラータイプ：ラベル系：黄（若干黒い）
    LABEL_PURPLE,       //!< カラータイプ：ラベル系：紫
    LABEL_GREEN,        //!< カラータイプ：ラベル系：緑
    LABEL_GREEN2,       //!< カラータイプ：ラベル系：緑（若干黒い）
    LABEL_PINK,         //!< カラータイプ：ラベル系：ピンク
    LABEL_RED,          //!< カラータイプ：ラベル系：赤
    LABEL_RED2,         //!< カラータイプ：ラベル系：赤
    LABEL_GRAY,         //!< カラータイプ：ラベル系：灰

    LABEL_ORANGE,       //!< カラータイプ：ラベル系：オレンジ
    LABEL_LIGHT_YELLOW, //!< カラータイプ：ラベル系：ライトイエロー

    LABEL_PURPLE_DARK,  //!< カラータイプ：ラベル系：黒紫

    LABEL_LIGHT_BLUE,   //!< カラータイプ：ラベル系：ライトブルー

    LABEL_YELLOW3,      //!< カラータイプ：ラベル系：黄３[v300追加]
    LABEL_PURPLE2,      //!< カラータイプ：ラベル系：紫[v300追加]

    LABEL_GRAY_TAB,     //!< カラータイプ：ラベル系：灰(タブ用)[v310追加]
    MAX,
};
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	アプリ内のカラー統一関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class ColorUtil
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public static Color COLOR_WHITE = RGB255Color(255, 255, 255);    // カラータイプ：ラベル系：白
    public static Color COLOR_YELLOW = RGB255Color(255, 255, 0); // カラータイプ：ラベル系：黄
    public static Color COLOR_YELLOW2 = RGB255Color(255, 192, 0);    // カラータイプ：ラベル系：黄（若干黒い）
    public static Color COLOR_YELLOW3 = RGB255Color(255, 162, 0);    // カラータイプ：ラベル系：黄 [v300追加]
    //public static Color COLOR_PURPLE = RGB255Color( 153 , 50 , 204);	// カラータイプ：ラベル系：紫
    public static Color COLOR_PURPLE = RGB255Color(193, 29, 217);    // カラータイプ：ラベル系：紫 [v300値変更]
    public static Color COLOR_PURPLE2 = RGB255Color(180, 0, 255);  // カラータイプ：ラベル系：紫2 [v300追加]
    public static Color COLOR_LIGHT_BLUE = RGB255Color(0, 255, 255); // カラータイプ：ラベル系：ライトブルー [v300値変更]
    public static Color COLOR_PINK = RGB255Color(255, 0, 228);   // カラータイプ：ラベル系：ピンク
    public static Color COLOR_RED = RGB255Color(255, 0, 0);  // カラータイプ：ラベル系：赤
    public static Color COLOR_RED2 = RGB255Color(255, 35, 73);   // カラータイプ：ラベル系：赤
    public static Color COLOR_GREEN2 = RGB255Color(33, 180, 68); // カラータイプ：ラベル系：緑（若干黒い）

    public static Color COLOR_GREEN = RGB255Color(66, 255, 0);   // カラータイプ：ラベル系：緑
    public static Color COLOR_GRAY = RGB255Color(128, 128, 128); // カラータイプ：ラベル系：灰
    public static Color COLOR_ORANGE = RGB255Color(255, 120, 0); // カラータイプ：ラベル系：オレンジ
    public static Color COLOR_LIGHT_YELLOW = RGB255Color(255, 255, 155); // カラータイプ：ラベル系：ライトイエロー
    public static Color COLOR_PURPLE_DARK = RGB255Color(90, 0, 128); // カラータイプ：ラベル系：黒紫

    public static Color COLOR_GRAY_TAB = RGB255Color(82, 80, 82);    // カラータイプ：ラベル系：灰(タブ用)

    public static Color COLOR_INVISIBLE = Color.clear; ///< 透明色

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    /// <summary>
    /// ステータス値取得
    /// </summary>
    static public Color GetColor(APP_COLOR eColor)
    {
        //--------------------------------
        // カラータイプに対応するカラーを返す
        //--------------------------------
        Color cColor = Color.white;
        switch (eColor)
        {
            case APP_COLOR.LABEL_WHITE: cColor = COLOR_WHITE; break;        // カラータイプ：ラベル系：白
            case APP_COLOR.LABEL_YELLOW: cColor = COLOR_YELLOW; break;      // カラータイプ：ラベル系：黄
            case APP_COLOR.LABEL_YELLOW2: cColor = COLOR_YELLOW2; break;        // カラータイプ：ラベル系：黄（若干黒い）
            case APP_COLOR.LABEL_PURPLE: cColor = COLOR_PURPLE; break;      // カラータイプ：ラベル系：紫
            case APP_COLOR.LABEL_GREEN: cColor = COLOR_GREEN; break;        // カラータイプ：ラベル系：緑
            case APP_COLOR.LABEL_GREEN2: cColor = COLOR_GREEN2; break;      // カラータイプ：ラベル系：緑（若干黒い）
            case APP_COLOR.LABEL_PINK: cColor = COLOR_PINK; break;      // カラータイプ：ラベル系：ピンク
            case APP_COLOR.LABEL_RED: cColor = COLOR_RED; break;        // カラータイプ：ラベル系：赤
            case APP_COLOR.LABEL_RED2: cColor = COLOR_RED2; break;      // カラータイプ：ラベル系：赤
            case APP_COLOR.LABEL_GRAY: cColor = COLOR_GRAY; break;      // カラータイプ：ラベル系：灰
            case APP_COLOR.LABEL_ORANGE: cColor = COLOR_ORANGE; break;      // カラータイプ：ラベル系：オレンジ
            case APP_COLOR.LABEL_LIGHT_YELLOW: cColor = COLOR_LIGHT_YELLOW; break;      // カラータイプ：ラベル系：ライトイエロー
            case APP_COLOR.LABEL_PURPLE_DARK: cColor = COLOR_PURPLE_DARK; break;        // カラータイプ：ラベル系：黒紫
            case APP_COLOR.LABEL_LIGHT_BLUE: cColor = COLOR_LIGHT_BLUE; break;      // カラータイプ：ラベル系：ライトブルー
            //v300追加
            case APP_COLOR.LABEL_YELLOW3: cColor = COLOR_YELLOW3; break;        // カラータイプ：ラベル系：黄３
            case APP_COLOR.LABEL_PURPLE2: cColor = COLOR_PURPLE2; break;        // カラータイプ：ラベル系：紫
            // v310追加
            case APP_COLOR.LABEL_GRAY_TAB: cColor = COLOR_GRAY_TAB; break;      // カラータイプ：ラベル系：灰(タブ用)
            default: Debug.LogError("Color Error! - " + eColor); break;     // 補間タイプ：__
        }
        return cColor;
    }

    /// <summary>
    /// RGBの値を255で計算する
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Color RGB255Color(float r, float g, float b, float a)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }

    /// <summary>
    /// RGBの値を255で計算する
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Color RGB255Color(float r, float g, float b)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    /// <summary>
    /// 属性ごとの色を取得する
    /// </summary>
    /// <param name="element_type"></param>
    /// <returns></returns>
    public static Color GetElementLabelColor(MasterDataDefineLabel.ElementType element_type)
    {
        string hex = "";
        switch (element_type)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                hex = "#c6c6c6";
                break;
            case MasterDataDefineLabel.ElementType.FIRE:
                hex = "#ff372c";
                break;
            case MasterDataDefineLabel.ElementType.WATER:
                hex = "#41b6ff";
                break;
            case MasterDataDefineLabel.ElementType.LIGHT:
                hex = "#ffff00";
                break;
            case MasterDataDefineLabel.ElementType.DARK:
                hex = "#b400ff";
                break;
            case MasterDataDefineLabel.ElementType.WIND:
                hex = "#25b300";
                break;
            case MasterDataDefineLabel.ElementType.HEAL:
                hex = "#ff8c00";
                break;
        }

        if (hex.IsNullOrEmpty()) { return Color.white; }

        return HexColor.ToColor(hex);
    }
};

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
