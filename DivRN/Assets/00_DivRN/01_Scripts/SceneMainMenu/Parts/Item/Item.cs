using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class Item : MenuPartsBase
{
    public enum CategoryType
    {
        Item,
        Point,
        Key,
        Max
    }
    public enum ItemType
    {
        Stamina = 0,
        RewardUp,
        Other,
        Max
    }
    // ダイアログ
    public GameObject Dialog;
    public GameObject DialogRoot;

    // ナビゲーションメニュー文字列
    M4uProperty<string> naviText0 = new M4uProperty<string>("");
    M4uProperty<string> naviText1 = new M4uProperty<string>("");
    M4uProperty<string> naviText2 = new M4uProperty<string>("");

    public string NaviText0 { get { return naviText0.Value; } set { naviText0.Value = value; } }
    public string NaviText1 { get { return naviText1.Value; } set { naviText1.Value = value; } }
    public string NaviText2 { get { return naviText2.Value; } set { naviText2.Value = value; } }

    // レコードの追加先
    M4uProperty<List<ItemDataContext>> records0 = new M4uProperty<List<ItemDataContext>>(new List<ItemDataContext>());
    M4uProperty<List<ItemDataContext>> records1 = new M4uProperty<List<ItemDataContext>>(new List<ItemDataContext>());
    M4uProperty<List<ItemDataContext>> records2 = new M4uProperty<List<ItemDataContext>>(new List<ItemDataContext>());
    public List<ItemDataContext> Records0 { get { return records0.Value; } set { records0.Value = value; } }
    public List<ItemDataContext> Records1 { get { return records1.Value; } set { records1.Value = value; } }
    public List<ItemDataContext> Records2 { get { return records2.Value; } set { records2.Value = value; } }

    M4uProperty<bool> isViewRecords0 = new M4uProperty<bool>();
    public bool IsViewRecords0 { get { return isViewRecords0.Value; } set { isViewRecords0.Value = value; } }

    M4uProperty<bool> isViewRecords1 = new M4uProperty<bool>();
    public bool IsViewRecords1 { get { return isViewRecords1.Value; } set { isViewRecords1.Value = value; } }

    M4uProperty<bool> isViewRecords2 = new M4uProperty<bool>();
    public bool IsViewRecords2 { get { return isViewRecords2.Value; } set { isViewRecords2.Value = value; } }

    M4uProperty<bool> isViewCategory0 = new M4uProperty<bool>();
    public bool IsViewCategory0 { get { return isViewCategory0.Value; } set { isViewCategory0.Value = value; } }

    M4uProperty<bool> isViewCategory1 = new M4uProperty<bool>();
    public bool IsViewCategory1 { get { return isViewCategory1.Value; } set { isViewCategory1.Value = value; } }

    M4uProperty<bool> isViewCategory2 = new M4uProperty<bool>();
    public bool IsViewCategory2 { get { return isViewCategory2.Value; } set { isViewCategory2.Value = value; } }


    M4uProperty<List<ItemKeyContext>> pointList = new M4uProperty<List<ItemKeyContext>>(new List<ItemKeyContext>());
    public List<ItemKeyContext> PointList { get { return pointList.Value; } set { pointList.Value = value; } }

    M4uProperty<string> pointLabel = new M4uProperty<string>();
    public string PointLabel { get { return pointLabel.Value; } set { pointLabel.Value = value; } }

    M4uProperty<bool> isViewPoint = new M4uProperty<bool>();
    public bool IsViewPoint { get { return isViewPoint.Value; } set { isViewPoint.Value = value; } }

    M4uProperty<List<ItemKeyContext>> ticketList = new M4uProperty<List<ItemKeyContext>>(new List<ItemKeyContext>());
    public List<ItemKeyContext> TicketList { get { return ticketList.Value; } set { ticketList.Value = value; } }

    M4uProperty<string> ticketLabel = new M4uProperty<string>();
    public string TicketLabel { get { return ticketLabel.Value; } set { ticketLabel.Value = value; } }

    M4uProperty<bool> isViewTicket = new M4uProperty<bool>();
    public bool IsViewTicket { get { return isViewTicket.Value; } set { isViewTicket.Value = value; } }

    M4uProperty<List<ItemKeyContext>> keyList = new M4uProperty<List<ItemKeyContext>>(new List<ItemKeyContext>());
    public List<ItemKeyContext> KeyList { get { return keyList.Value; } set { keyList.Value = value; } }

    M4uProperty<string> keyLabel = new M4uProperty<string>();
    public string KeyLabel { get { return keyLabel.Value; } set { keyLabel.Value = value; } }

    M4uProperty<bool> isViewKey = new M4uProperty<bool>();
    public bool IsViewKey { get { return isViewKey.Value; } set { isViewKey.Value = value; } }

    M4uProperty<bool> isViewCategoryEmpty0 = new M4uProperty<bool>();
    public bool IsViewCategoryEmpty0 { get { return isViewCategoryEmpty0.Value; } set { isViewCategoryEmpty0.Value = value; } }

    M4uProperty<bool> isViewCategoryEmpty1 = new M4uProperty<bool>();
    public bool IsViewCategoryEmpty1 { get { return isViewCategoryEmpty1.Value; } set { isViewCategoryEmpty1.Value = value; } }

    M4uProperty<bool> isViewCategoryEmpty2 = new M4uProperty<bool>();
    public bool IsViewCategoryEmpty2 { get { return isViewCategoryEmpty2.Value; } set { isViewCategoryEmpty2.Value = value; } }

    M4uProperty<string> emptyLabel = new M4uProperty<string>();
    public string EmptyLabel { get { return emptyLabel.Value; } set { emptyLabel.Value = value; } }

    [SerializeField]
    private Toggle[] m_toggles;

    private CategoryType m_SelectCategoryType = CategoryType.Item;

    public System.Action DidSelectTab = delegate { };

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        IsViewCategory0 = true;
        IsViewCategory1 = false;
        IsViewCategory2 = false;
    }

    void Start()
    {
    }

    public void OnSelectTab(int _tabId)
    {
        if (!m_toggles[_tabId].isOn
            || m_SelectCategoryType == (CategoryType)_tabId)
            return;

        m_SelectCategoryType = (CategoryType)_tabId;
        switch (m_SelectCategoryType)
        {
            case CategoryType.Item:
                IsViewCategory0 = true;
                IsViewCategory1 = false;
                IsViewCategory2 = false;
                break;
            case CategoryType.Point:
                IsViewCategory0 = false;
                IsViewCategory1 = true;
                IsViewCategory2 = false;
                break;
            case CategoryType.Key:
                IsViewCategory0 = false;
                IsViewCategory1 = false;
                IsViewCategory2 = true;
                break;
        }
        DidSelectTab();
    }

    public void AddRecord(
                ItemType type,
                MasterDataUseItem item_master,
                int item_count,
                long use_timing,
                System.Action<ItemDataContext> selectAction)
    {
        ItemDataContext contex = new ItemDataContext();

        contex.setupIcon(item_master);
        contex.ItemMaster = item_master;
        contex.Count = item_count;  // アイテムの所持数
        contex.use_timing = use_timing; //使用開始した時間

        contex.DidSelectItem += selectAction;

        // レコードの追加先を指定
        switch (type)
        {
            case ItemType.Stamina:
                Records0.Add(contex);
                break;
            case ItemType.RewardUp:
                Records1.Add(contex);
                break;
            case ItemType.Other:
                Records2.Add(contex);
                break;
        }
    }

    public void ResetRecordAll()
    {
        Records0.Clear();
        Records1.Clear();
        Records2.Clear();
        PointList.Clear();
        TicketList.Clear();
        KeyList.Clear();
    }

    public void setup()
    {
        IsViewRecords0 = Records0.Count != 0 ? true : false;
        IsViewRecords1 = Records1.Count != 0 ? true : false;
        IsViewRecords2 = Records2.Count != 0 ? true : false;
        IsViewCategoryEmpty0 = (!IsViewRecords0 && !IsViewRecords1 && !IsViewRecords2) ? true : false;

        IsViewPoint = PointList.Count != 0 ? true : false;
        IsViewTicket = TicketList.Count != 0 ? true : false;
        IsViewCategoryEmpty1 = (!IsViewPoint && !IsViewTicket) ? true : false;

        IsViewKey = KeyList.Count != 0 ? true : false;
        IsViewCategoryEmpty2 = (!IsViewKey) ? true : false;

    }

}
