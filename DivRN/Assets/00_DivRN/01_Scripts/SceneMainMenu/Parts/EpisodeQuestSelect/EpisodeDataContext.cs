using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class EpisodeDataContext : M4uContext
{
    private EpisodeDataListItemModel m_model;
    public EpisodeDataListItemModel model { get { return m_model; } }

    public EpisodeDataContext(EpisodeDataListItemModel listItemModel)
    {
        m_model = listItemModel;
    }


    public bool IsSelected
    {
        get { return m_model.isSelected; }
        set { m_model.isSelected = value; }
    }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<Sprite> selectImage = new M4uProperty<Sprite>();
    public Sprite SelectImage { get { return selectImage.Value; } set { selectImage.Value = value; } }

    M4uProperty<bool> isActiveFlag = new M4uProperty<bool>();
    public bool IsActiveFlag { get { return isActiveFlag.Value; } set { isActiveFlag.Value = value; } }

    M4uProperty<Sprite> flagImage = new M4uProperty<Sprite>();
    public Sprite FlagImage { get { return flagImage.Value; } set { flagImage.Value = value; } }

	M4uProperty<string> flagRate = new M4uProperty<string>();
	public string FlagRate { get { return flagRate.Value; } set { flagRate.Value = value; } }

	M4uProperty<string> time = new M4uProperty<string>();
    public string Time { get { return time.Value; } set { time.Value = value; } }

    public System.Action<uint> DidSelected = delegate { };

    public MasterDataAreaCategory masterDataAreaCategory = null;
    public MasterDataArea master = null;
    public MainMenuQuestSelect.AreaAmendParam amend = null;
    public List<Sprite> flagImageList = new List<Sprite>();
	public List<string> flagTextList = new List<string>();
	public int flagCounter = 0;

    public uint m_EpisodeId = 0;

    public AssetBundler CreatetIcon()
    {
        Sprite defaultSpite = IconImage;
        IconImage = null;

        string assetName = string.Format("icon{0:D2}", m_EpisodeId);

#if BUILD_TYPE_DEBUG
        Debug.Log("ASSETNAME:" + assetName + " FIX:" + masterDataAreaCategory.fix_id);
#endif

        string assetbundleName = MasterDataUtil.GetMasterDataAreamapBackgroundName(masterDataAreaCategory.background);

        return AssetBundler.Create().Set(assetbundleName, assetName, (o) =>
            {
                Sprite sprite = o.AssetBundle.LoadAsset<Sprite>(assetName);
                if (sprite != null)
                {
                    IconImage = sprite;
                }
                else
                {
                    IconImage = defaultSpite;
                }
            },
            (error) =>
            {
                IconImage = defaultSpite;
            });
    }
}
