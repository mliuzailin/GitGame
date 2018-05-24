using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MasterDataStepUpGacha : Master
{

    public uint timing_public { get; set; }                           //!< 一般公開タイミング
    public uint timing_close { get; set; }                            //!< 一般公開終了タイミング

    public uint gacha_id { get; set; }                                //!< ガチャマスタのfix_id

    public MasterDataDefineLabel.BoolType paid_tip_only { get; set; } //!< 有償チップ専用フラグ

    public MasterDataDefineLabel.BoolType reset_flg { get; set; }     //!< ステップ進行状況のリセットフラグ
    public int reset_date { get; set; }                              //!< リセット(年月日時): 時間指定
    public int reset_time { get; set; }                              //!< リセット(時分): 時間指定

    public string guideline_text_key { get; set; }                    //!< ガチャガイドライン使用テキストキー
}

