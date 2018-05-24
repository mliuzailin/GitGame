using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マスターデータ実体：ストーリー
/// </summary>
[System.Serializable]
public class MasterDataStoryChara : Master
{

    public MasterDataDefineLabel.CharaType chara_type { get; set; }
    public uint chara_id { get; set; }
    public int img_tiling { get; set; }
    public int img_offset_x { get; set; }
    public int img_offset_y { get; set; }
}

