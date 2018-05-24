using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitCatalog : MenuPartsBase, IRecyclableItemsScrollContentDataProvider
{
    [SerializeField]
    RecyclableItemsScrollContent scrollContent = null;
    [SerializeField]
    GameObject itemPrefab = null;

    private List<UnitCatalogItemContext> masterCatalogList = new List<UnitCatalogItemContext>();
    public List<UnitCatalogItemContext> MasterCatalogList { get { return masterCatalogList; } set { masterCatalogList = value; } }

    M4uProperty<string> labelText = new M4uProperty<string>();
    public string LabelText { get { return labelText.Value; } set { labelText.Value = value; } }

    M4uProperty<bool> isActiveLabel = new M4uProperty<bool>(true);
    public bool IsActiveLabel { get { return isActiveLabel.Value; } set { isActiveLabel.Value = value; } }

    M4uProperty<string> countText = new M4uProperty<string>("");
    public string CountText { get { return countText.Value; } set { countText.Value = value; } }

    private int m_Count = 0;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        LabelText = GameTextUtil.GetText("he184p_title");
    }

    public void Init()
    {
        masterCatalogList.Sort((a, b) => (int)a.master.draw_id - (int)b.master.draw_id);
        if (scrollContent != null)
        {
            scrollContent.Initialize(this);
        }
    }

    #region IRecyclableItemsScrollContentDataProvider methods.

    public int DataCount
    {
        get
        {
            int _ret = MasterCatalogList.Count / 5;
            if (MasterCatalogList.Count % 5 != 0) _ret++;
            return _ret;
        }
    }

    public float GetItemScale(int index)
    {
        // セルの高さを返す
        return itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    public RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        // indexの位置にあるセルを読み込む処理

        if (null == recyclableItem)
        {
            // 初回ロード時はinstantateItemCountで指定した回数分だけitemがnullで来るので、ここで生成してあげる
            // 以降はitemが再利用されるため、Reflesh()しない限りnullは来ない
            recyclableItem = Instantiate(itemPrefab).GetComponent<RectTransform>();
        }

        // セルの内容書き換え
        {
            UnitCatalogLine _line = recyclableItem.GetComponent<UnitCatalogLine>();
            _line.ItemList.Clear();
            int start_id = index * 5;
            for (int i = 0; i < 5; i++)
            {
                if (start_id + i >= MasterCatalogList.Count)
                {
                    continue;
                }

                UnitCatalogItemContext copyContext = new UnitCatalogItemContext();
                MasterCatalogList[start_id + i].Copy(copyContext);
                copyContext.requestId = UnitIconImageProvider.Instance.Get(
                    copyContext.master.fix_id,
                    sprite =>
                    {
                        copyContext.IconImage = sprite;
                    });

                _line.ItemList.Add(copyContext);
            }
            m_Count++;
            if (m_Count >= 100)
            {
                System.GC.Collect();
                m_Count = 0;
            }
        }

        return recyclableItem;
    }

    #endregion
}
