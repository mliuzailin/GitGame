using UnityEngine;
using System.Collections;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：AssetBundle管理関連：各種ファイルパス
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataAssetBundlePath : Master
{

    public MasterDataAssetBundlePath()
    {
    }

    public string name
    {
        get;
        set;
    }

    public MasterDataDefineLabel.ASSETBUNDLE_CATEGORY category
    {
        get;
        set;
    }

    public uint version
    {
        get;
        set;
    }

    public MasterDataDefineLabel.ASSETBUNDLE_TITLE_DL title_dl
    {
        get;
        set;
    }

    [Ignore]
    public bool IsIconPackUnit
    {
        get
        {
            return name.ToLower().Contains("iconpackunit");
        }
    }

    [Ignore]
    public string LowerName
    {
        get;
        set;
    }

};
