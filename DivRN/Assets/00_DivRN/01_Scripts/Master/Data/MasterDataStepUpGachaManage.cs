using SQLite.Attribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MasterDataStepUpGachaManage : Master
{
    [Ignore]
    public override uint tag_id { get; set; }
    [Ignore]
    public uint total_lot_exec
    {
        get
        {
            return normal1_lot_exec + normal2_lot_exec + special_lot_exec;
        }
    }

    public uint timing_public { get; set; }     //!< 一般公開タイミング
    public uint gacha_id { get; set; }          //!< ガチャマスタのfix_id

    public uint step_num { get; set; }          //!< 現在のステップ数
    public uint next_step_num { get; set; }     //!< 次ステップ数を指定

    public uint normal1_assign_id { get; set; } //!< ユニット抽選：通常枠1 アサインID
    public uint normal1_lot_exec { get; set; }  //!< ユニット抽選：通常枠1 事項回数

    public uint normal2_assign_id { get; set; } //!< ユニット抽選：通常枠2 アサインID
    public uint normal2_lot_exec { get; set; }  //!< ユニット抽選：通常枠3 事項回数

    public uint special_assign_id { get; set; } //!< ユニット抽選：確定枠 アサインID
    public uint special_lot_exec { get; set; }  //!< ユニット抽選：確定枠 事項回数

    public uint price { get; set; }             //!< リソース総消費量

    public string url_img { get; set; }         //!< 宣伝用画像 S3の格納先指定
    public string detail_text { get; set; }     //!< 詳細説明文字 テキストキー指定

    public MasterDataDefineLabel.BoolType present_enable { get; set; }    //!< ガチャおまけ：有効化フラグ
    public uint present_group_id { get; set; }  //!< ガチャおまけ：付与プレゼント グループID
    public string present_message { get; set; } //!< ガチャおまけ：付与プレゼントメッセージ
}
