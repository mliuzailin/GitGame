using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：リージョン関連：リージョン情報
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataRegion : Master
{
    public string name
    {
        get;
        set;
    }

    public MasterDataDefineLabel.REGION_CATEGORY category
    {
        get;
        set;
    }

    public uint sort
    {
        get;
        set;
    }
}
