using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：クエストキー
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataQuestKey : Master
{
    public uint timing_public { get; set; }      //!< 一般公開タイミング

    public string key_name { get; set; }         //!< クエストキー名称
    public uint key_area_category_id { get; set; }        //!< 対応エリアID

    public uint timing_end { get; set; }         //!< クエストキー期限（終了タイミング）
}
