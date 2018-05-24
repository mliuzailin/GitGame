using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
    @brief  マスターデータ実体：ヒーロー関連：レベル情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataHeroLevel : Master
{
    public int exp_next { get; set; }

    public int exp_next_total { get; set; }
}
