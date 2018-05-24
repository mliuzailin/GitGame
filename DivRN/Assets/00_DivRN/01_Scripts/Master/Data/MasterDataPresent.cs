using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：ログインボーナス関連：プレゼント定義
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataPresent : Master
{
    public MasterDataDefineLabel.PresentType present_type { get; set; }       //!< プレゼント定義：タイプ
    public int present_param1 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param2 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param3 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param4 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param5 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param6 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param7 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param8 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param9 { get; set; }      //!< プレゼント定義：パラメータ
    public int present_param10 { get; set; }	//!< プレゼント定義：パラメータ

    /// <summary>
    /// コピー
    /// </summary>
    /// <param name="cSrc"></param>
    public void Copy(MasterDataPresent src)
    {
        base.Copy(src);

        present_type = src.present_type;
        present_param1 = src.present_param1;
        present_param2 = src.present_param2;
        present_param3 = src.present_param3;
        present_param4 = src.present_param4;
        present_param5 = src.present_param5;
        present_param6 = src.present_param6;
        present_param7 = src.present_param7;
        present_param8 = src.present_param8;
        present_param9 = src.present_param9;
        present_param10 = src.present_param10;
    }
};
