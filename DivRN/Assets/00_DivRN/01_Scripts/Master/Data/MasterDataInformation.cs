using System;
using UnityEngine;
using System.Collections;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：運営お知らせ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataInformation : Master
{
    [Ignore]
    public override uint tag_id
    {
        get;
        set;
    }

    public MasterDataDefineLabel.BoolType active
    {
        get;
        set;
    } //!< データ使用フラグ	※このデータは一部領域を使いまわす。Excelの行ごとに無効化してデータを作るのでフラグを持つ

    public MasterDataDefineLabel.InfomationType type
    {
        get;
        set;
    } //!< タイプ

    public int platform
    {
        get;
        set;
    } //!< プラットフォーム

    public int priority
    {
        get;
        set;
    } //!< プライオリティ

    public uint timing_start
    {
        get;
        set;
    } //!< 表示タイミング：開始

    public uint timing_end
    {
        get;
        set;
    } //!< 表示タイミング：終了

    public string message
    {
        get;
        set;
    } //!< 表示メッセージ

    public override string ToString()
    {
        return "MASTER_DATA_INFORMAITAN:" + fix_id;
    }
};
