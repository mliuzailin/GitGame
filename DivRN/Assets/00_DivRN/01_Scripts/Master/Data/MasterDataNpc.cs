using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マスターデータ実体：NPC
/// </summary>
[System.Serializable]
public class MasterDataNpc : Master
{
    //public uint fix_id;                            //!< 情報固有固定ID
    public string name { get; set; }      //!< 名前
    public string detail { get; set; }      //!< 詳細テキスト
    public int img_story_tiling { get; set; }      //!< ストーリ画面表示時の画像補正
    public int img_story_offset_x { get; set; }      //!< ストーリ画面表示時の画像補正
    public int img_story_offset_y { get; set; }      //!< ストーリ画面表示時の画像補正
}

