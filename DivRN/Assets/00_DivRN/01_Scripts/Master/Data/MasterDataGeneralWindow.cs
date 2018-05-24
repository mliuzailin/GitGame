using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MasterDataGeneralWindow : Master
{
    public uint group_id { get; set; }                                                //!< グループID

    public string title { get; set; }                                                 //!< 見出し
    public MasterDataDefineLabel.GeneralWindowMessageType message_type { get; set; }  //!< 表示タイプ 0:文章/1:画像
    public string message { get; set; }                                               //!< 表示リソース 文章 or アセットID

    public string char_img { get; set; }                                              //!< 表示キャラ アセットID
    public MasterDataDefineLabel.GeneralWindowCharaType char_type { get; set; }       //!< キャラ表示位置 0:左/1:中央/2:右
    public int char_offset_x { get; set; }                                            //!< キャラ表示調整値x
    public int char_offset_y { get; set; }                                            //!< キャラ表示調整値x

    public MasterDataDefineLabel.GeneralWindowButtonType button_type { get; set; }    //!< ボタン表示タイプ 0:閉じる,次へ/1:はい,いいえ
    public string button_01 { get; set; }                                             //!< ボタンテキストキー
    public string button_02 { get; set; }                                             //!< ボタンテキストキー
}
