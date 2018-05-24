using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;
using System;
using AsPerSpec;
using System.Linq;

public class MainMenuSubTab : View
{
    //----------------------------------------------------------------------------
    /*!
        @brief サブタブメニュータイプ
    */
    //----------------------------------------------------------------------------
    public enum SUB_TAB_MENU_TYPE
    {
        NONE = -1,

        FRIEND,
        SCRATCH,

        MAX,
    }

    public class SubTabData
    {
        public MAINMENU_SEQ seq { get; set; }
        public string icon { get; set; }
        public string text { get; set; }

        public SubTabData(MAINMENU_SEQ _seq, string _icon, string _text)
        {
            seq = _seq;
            icon = _icon;
            text = _text;
        }
    }

    private SubTabData[] friendSubTabData = new SubTabData[4]
    {
        new SubTabData(MAINMENU_SEQ.SEQ_FRIEND_LIST,"btn_furendoichiran","furendoichiran"),
        new SubTabData(MAINMENU_SEQ.SEQ_FRIEND_LIST_WAIT_HIM,"btn_shinsei","shinsei"),
        new SubTabData(MAINMENU_SEQ.SEQ_FRIEND_LIST_WAIT_ME,"btn_syounini","syounin"),
        new SubTabData(MAINMENU_SEQ.SEQ_FRIEND_SEARCH,"btn_kensaku","kensaku")
    };

    public ScrollRect scrollRect = null;

    M4uProperty<List<SubTabGroupContext>> tabGroupList = new M4uProperty<List<SubTabGroupContext>>(new List<SubTabGroupContext>());
    public List<SubTabGroupContext> TabGroupList { get { return tabGroupList.Value; } set { tabGroupList.Value = value; } }

    List<GameObject> tabGroupObjectList = new List<GameObject>();
    public List<GameObject> TabGroupObjectList { get { return tabGroupObjectList; } set { tabGroupObjectList = value; } }

    M4uProperty<bool> isViewTabList = new M4uProperty<bool>();
    public bool IsViewTabList { get { return isViewTabList.Value; } set { isViewTabList.Value = value; } }

    M4uProperty<bool> isViewLeftArrow = new M4uProperty<bool>();
    public bool IsViewLeftArrow { get { return isViewLeftArrow.Value; } set { isViewLeftArrow.Value = value; } }

    M4uProperty<bool> isViewRightArrow = new M4uProperty<bool>();
    public bool IsViewRightArrow { get { return isViewRightArrow.Value; } set { isViewRightArrow.Value = value; } }

    M4uProperty<bool> isScrollMoving = new M4uProperty<bool>();
    public bool IsScrollMoving { get { return isScrollMoving.Value; } set { isScrollMoving.Value = value; } }

    private SUB_TAB_MENU_TYPE m_SubMenuType = SUB_TAB_MENU_TYPE.NONE;

    public CarouselRotator m_CarouselRotator = null;
    public CarouselToggler m_CarouselToggler = null;

    private bool m_IsRestTabs = false; // 同じページ遷移でタブを作り直すかどうか

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        IsViewLeftArrow = false;
        IsViewRightArrow = false;

        scrollRect = GetComponentInChildren<ScrollRect>();
        m_CarouselRotator = scrollRect.GetComponent<CarouselRotator>();
        m_CarouselToggler = scrollRect.GetComponent<CarouselToggler>();

    }

    void LateUpdate()
    {
        if (IsScrollMoving != m_CarouselToggler.moving)
        {
            IsScrollMoving = m_CarouselToggler.moving;
        }
    }

    /// <summary>
    /// サブタブ設定
    /// </summary>
    /// <param name="_seq"></param>
    public void SetSubTab(MAINMENU_SEQ _seq)
    {
        SUB_TAB_MENU_TYPE _next = getSubTabType(_seq);
        if (_next == SUB_TAB_MENU_TYPE.NONE)
        {
            TabGroupList.Clear();
            m_SubMenuType = SUB_TAB_MENU_TYPE.NONE;
            StartCoroutine(TabClearWait());
        }
        else if (m_SubMenuType != _next || m_IsRestTabs == true)
        {
            m_SubMenuType = _next;
            setupSubTab(_seq);
            m_IsRestTabs = false;
        }
        else
        {
            changeSubTab(_seq);
        }
    }

    IEnumerator TabClearWait()
    {
        while (TabGroupObjectList.Count != 0)
        {
            yield return null;
        }
        IsViewTabList = false;
    }

    /// <summary>
    /// サブタブタイプ取得
    /// </summary>
    /// <param name="_seq"></param>
    /// <returns></returns>
    private SUB_TAB_MENU_TYPE getSubTabType(MAINMENU_SEQ _seq)
    {
        SUB_TAB_MENU_TYPE _ret = SUB_TAB_MENU_TYPE.NONE;
        switch (_seq)
        {
            case MAINMENU_SEQ.SEQ_FRIEND_LIST:
            case MAINMENU_SEQ.SEQ_FRIEND_LIST_WAIT_HIM:
            case MAINMENU_SEQ.SEQ_FRIEND_LIST_WAIT_ME:
            case MAINMENU_SEQ.SEQ_FRIEND_SEARCH:
                _ret = SUB_TAB_MENU_TYPE.FRIEND;
                break;
            case MAINMENU_SEQ.SEQ_GACHA_MAIN:
                _ret = SUB_TAB_MENU_TYPE.SCRATCH;
                break;
        }
        return _ret;
    }

    /// <summary>
    /// サブタブ構築
    /// </summary>
    /// <param name="_seq"></param>
    private void setupSubTab(MAINMENU_SEQ _seq)
    {
        IsViewTabList = true;
        TabGroupList.Clear();
        scrollRect.content.anchoredPosition = new Vector2(0.0f, scrollRect.content.anchoredPosition.y);
        switch (m_SubMenuType)
        {
            case SUB_TAB_MENU_TYPE.FRIEND:
                //case SUB_TAB_MENU_TYPE.HELP:
                setupSubTabFriend(_seq);
                break;
            case SUB_TAB_MENU_TYPE.SCRATCH:
                setupSubTabScratch(_seq);
                break;
        }
    }

    public void AddData(SubTabContext subtab)
    {
        if (TabGroupList.Count == 0)
        {
            SubTabGroupContext addGroup = new SubTabGroupContext();
            addGroup.SubTabList.Add(subtab);
            TabGroupList.Add(addGroup);
        }
        else
        {
            SubTabGroupContext group = TabGroupList[TabGroupList.Count - 1];
            if (group.SubTabList.Count < 5)
            {
                group.SubTabList.Add(subtab);
            }
            else
            {
                SubTabGroupContext addGroup = new SubTabGroupContext();
                addGroup = new SubTabGroupContext();
                addGroup.SubTabList.Add(subtab);
                TabGroupList.Add(addGroup);
            }
        }
    }


    /// <summary>
    /// サブタブ構築（その他用）
    /// </summary>
    /// <param name="_seq"></param>
    private void setupSubTabFriend(MAINMENU_SEQ _seq)
    {
        uint totalWaitMe = 0;
        uint totalWaitHim = 0;
        UserDataAdmin.Instance.GetFriendInfo(ref totalWaitMe, ref totalWaitHim);

        for (int j = 0; j < friendSubTabData.Length; j++)
        {
            int index = j;
            var model = new ListItemModel((uint)index);

            SubTabContext _newTab = new SubTabContext(model);
            _newTab.m_MenuSeq = friendSubTabData[j].seq;
            _newTab.IconImage = ResourceManager.Instance.Load(friendSubTabData[j].icon);
            _newTab.IsViewTxtImage = true;
            _newTab.TxtImage = ResourceManager.Instance.Load(friendSubTabData[j].text);
            _newTab.GachaMaster = null;
            _newTab.IsViewFlag = false;
            switch (friendSubTabData[j].seq)
            {
                case MAINMENU_SEQ.SEQ_FRIEND_LIST_WAIT_ME:
                    _newTab.AlertCount = (int)totalWaitMe;
                    break;
                case MAINMENU_SEQ.SEQ_FRIEND_LIST_WAIT_HIM:
                    _newTab.AlertCount = 0;
                    break;
                default:
                    _newTab.AlertCount = 0;
                    break;
            }
            _newTab.setFlag(_newTab.m_MenuSeq == _seq);
            AddData(_newTab);

            model.OnClicked += () =>
            {
                if (OnSelectTab(_newTab) == true)
                {
                    SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
                }
            };
        }
    }

    /// <summary>
    /// サブタブ構築（スクラッチ用）
    /// </summary>
    /// <param name="_seq"></param>
    private void setupSubTabScratch(MAINMENU_SEQ _seq)
    {
        if (TutorialManager.IsExists == true)
        {
            return;
        }
        MasterDataGacha[] gachaArray = MasterDataUtil.GetActiveGachaMaster();
        if (gachaArray == null || gachaArray.Length == 0)
        {
            return;
        }

        int index = 0;

        for (int j = 0; j < gachaArray.Length; j++)
        {
            if (gachaArray[j] == null)
            {
                continue;
            }

            var model = new ListItemModel((uint)index++);

            SubTabContext _newTab = new SubTabContext(model);
            _newTab.m_MenuSeq = MAINMENU_SEQ.SEQ_GACHA_MAIN;
            _newTab.IconImage = null;
            _newTab.IsViewTxtImage = false;
            _newTab.TabName = "";
            _newTab.GachaMaster = gachaArray[j];
            _newTab.AlertCount = (int)MasterDataUtil.GetGachaCountFromMaster(gachaArray[j], GlobalDefine.VALUE_VIEW_MAX_INFO, true);
            _newTab.setFlag(MainMenuParam.m_GachaMaster == gachaArray[j]);
            _newTab.IsViewFlag = MasterDataUtil.IsFirstTimeFree(gachaArray[j]);
            AddData(_newTab);

            model.OnClicked += () =>
            {
                if (OnSelectTab(_newTab) == true)
                {
                    SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
                }
            };
        }

        LoadIconResource();
    }

    /// <summary>
    /// サブタブ変更
    /// </summary>
    /// <param name="_seq"></param>
    private void changeSubTab(MAINMENU_SEQ _seq)
    {
        for (int i = 0; i < TabGroupList.Count; ++i)
        {
            for (int j = 0; j < TabGroupList[i].SubTabList.Count; ++j)
            {
                SubTabContext subTab = TabGroupList[i].SubTabList[j];
                if (m_SubMenuType == SUB_TAB_MENU_TYPE.FRIEND)
                {
                    subTab.setFlag(subTab.m_MenuSeq == _seq);
                }
                else if (m_SubMenuType == SUB_TAB_MENU_TYPE.SCRATCH)
                {
                    subTab.setFlag(MainMenuParam.m_GachaMaster == subTab.GachaMaster);
                }

                if (subTab.IsSelect == true)
                {
                    Toggle toggle = TabGroupObjectList[i].GetComponentInChildren<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = true;
                    }
                }
            }
        }

        m_CarouselToggler.CenterOnToggled();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="_tab"></param>
    /// <return>true:切り替えた false:切り替えてない</return>
    private bool OnSelectTab(SubTabContext _tab)
    {
        if (!MainMenuManager.HasInstance ||
            MainMenuManager.Instance.IsPageSwitch())
        {
            return false;
        }

        if (m_SubMenuType == SUB_TAB_MENU_TYPE.SCRATCH)
        {
            //同じガチャには遷移しない
            if (MainMenuParam.m_GachaMaster.fix_id == _tab.GachaMaster.fix_id)
            {
                return false;
            }

            MainMenuParam.m_GachaMaster = _tab.GachaMaster;
            ((MainMenuScratch)MainMenuManager.Instance.MainMenuSeqPageNow).changeScratch(_tab.Index);
            changeSubTab(MAINMENU_SEQ.SEQ_GACHA_MAIN);
        }
        else
        {
            MainMenuManager.Instance.AddSwitchRequest(_tab.m_MenuSeq, false, false);
        }

        return true;
    }

    private void LoadIconResource()
    {

        for (int i = 0; i < TabGroupList.Count; ++i)
        {
            for (int j = 0; j < TabGroupList[i].SubTabList.Count; ++j)
            {
                SubTabContext subTab = TabGroupList[i].SubTabList[j];

                if (subTab.GachaMaster == null)
                {
                    continue;
                }

                string url = "";
                if (subTab.GachaMaster.url_img == string.Empty)
                {
                    url = string.Format(GlobalDefine.GetScratchIconhUrl(), subTab.GachaMaster.fix_id);
                }
                else
                {
                    url = string.Format(GlobalDefine.GetScratchIconhUrl(), subTab.GachaMaster.url_img);
                }

                WebResource.Instance.GetSprite(url,
                            (Sprite sprite) =>
                            {
                                subTab.IconImage = sprite;
                            },
                            () =>
                            {
                                subTab.IconImage = ResourceManager.Instance.Load("dummy_scratch_icon", ResourceType.Common);
                            });
            }
        }
    }

    public void updateTabItem()
    {
        switch (m_SubMenuType)
        {
            case SUB_TAB_MENU_TYPE.FRIEND:
                {
                    uint totalWaitMe = 0;
                    uint totalWaitHim = 0;
                    UserDataAdmin.Instance.GetFriendInfo(ref totalWaitMe, ref totalWaitHim);
                    GetSubTab(1).AlertCount = 0;
                    GetSubTab(2).AlertCount = (int)totalWaitMe;
                }
                break;
            case SUB_TAB_MENU_TYPE.SCRATCH:
                break;
        }
    }

    /// <summary>
    /// 指定したインデックスのパーティ情報を取得する
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public SubTabContext GetSubTab(int index)
    {
        for (int i = 0; i < TabGroupList.Count; ++i)
        {
            for (int j = 0; j < TabGroupList[i].SubTabList.Count; ++j)
            {
                SubTabContext subTab = TabGroupList[i].SubTabList[j];
                if (subTab.Index == index)
                {
                    return subTab;
                }
            }
        }

        return null;
    }

    public void SetResetFlag()
    {
        m_IsRestTabs = true;
    }

    /// <summary>
    /// 矢印ボタンの表示・非表示状態を変更する
    /// </summary>
    void ChangeArrowButtonActive()
    {
        int index = -1;
        for (int i = 0; i < TabGroupObjectList.Count; i++)
        {
            Toggle toggle = TabGroupObjectList[i].GetComponentInChildren<Toggle>();
            if (toggle == null) { continue; }

            if (toggle.isOn == true)
            {
                index = i;
                break;
            }
        }

        if (TabGroupObjectList.Count <= 1)
        {
            // リストが1個以下のとき
            IsViewRightArrow = false;
            IsViewLeftArrow = false;
        }
        else if (index == 0)
        {
            // リストが先頭の場合
            IsViewRightArrow = true;
            IsViewLeftArrow = false;
        }
        else if (index == TabGroupObjectList.Count - 1)
        {
            // リストが最後尾の場合
            IsViewRightArrow = false;
            IsViewLeftArrow = true;
        }
        else
        {
            // その他の場合
            IsViewRightArrow = true;
            IsViewLeftArrow = true;
        }
    }

    /// <summary>
    /// ページが切り替わったときに呼ばれる
    /// </summary>
    /// <param name="isOn"></param>
    public void OnChangedGroup(bool isOn)
    {
        if (isOn == true)
        {
            ChangeArrowButtonActive();
        }
    }

    /// <summary>
    /// リストのオブジェクトが生成された
    /// </summary>
    void OnChangedCollection()
    {
        if (TabGroupObjectList.Count > 0)
        {
            //ScrollContentが機能した後に実行する
            StartCoroutine(DelayScrollContent(() =>
            {
                // リストの表示位置設定
                for (int i = 0; i < TabGroupList.Count; ++i)
                {
                    for (int j = 0; j < TabGroupList[i].SubTabList.Count; ++j)
                    {
                        SubTabContext subTab = TabGroupList[i].SubTabList[j];
                        if (subTab.IsSelect)
                        {
                            Toggle toggle = TabGroupObjectList[i].GetComponentInChildren<Toggle>();
                            if (toggle != null)
                            {
                                toggle.isOn = true;
                                m_CarouselToggler.CenterOnToggled();
                            }

                            return;
                        }
                    }
                }
            }));
        }
    }

    /// <summary>
    /// ScrollRect.contentが機能した後に実行する
    /// </summary>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayScrollContent(Action action)
    {
        ScrollRect scrollRect = m_CarouselToggler.GetComponentInChildren<ScrollRect>();
        while (scrollRect.content.rect.width == 0 || TabGroupList.Any(q => q.Toggle == null))
        {
            yield return null;
        }

        action();
    }

    public void OnClickPrevButton()
    {
        if (m_CarouselRotator != null && m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            m_CarouselRotator.Step(false);
        }
    }

    public void OnClickNextButton()
    {
        if (m_CarouselRotator != null && m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            m_CarouselRotator.Step(true);
        }
    }

    public void OnChangedSnapCarousel(bool isChange)
    {
        if (isChange == true)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
        }
    }

    public void changeSelectTub(int index)
    {
        int mainIndex = index / 5;
        int subIndex = index % 5;

        OnSelectTab(TabGroupList[mainIndex].SubTabList[subIndex]);
    }
}
