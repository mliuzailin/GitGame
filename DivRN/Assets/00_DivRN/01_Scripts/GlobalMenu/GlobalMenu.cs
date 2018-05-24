using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M4u;

public enum GLOBALMENU_TYPE : int
{
    MAIN_MENU = 0,
    TITLE,
    IN_GAME,

    MAX,
}

public enum GLOBALMENU_SEQ : int
{
    NONE = -1,

    TOP_MENU = 0,

    //MainMenu
    OPTION,
    MISSION,
    PRESENT,
    ITEM,
    EVENTSCHEDULE,
    TITLE,

    //Battle

    MAX,
}

public enum GlobalMenuButtonCategory : int
{
    User = 0,
    Game,
    Other,

    Max,
}
public enum GlobalMenuButtonType : int
{
    //MainMenu
    Infomation = 0,
    Option,
    Mission,
    Present,
    Item,
    Friend,
    EventSchedule,
    Logout,
    UserInfo,
    GameHelp,
    Catalog,
    Web,
    TOS,
    Movie,
    Support,

    Max,
}

public class GlobalMenu : View
{
    private static readonly string AppearAnimationName = "global_menu_appear";
    private static readonly string AppearPageAnimationName = "global_menu_page_appear";
    private static readonly string DisappearAnimationName = "global_menu_disappear";

    protected GlobalMenuContext m_Context = null;
    public GlobalMenuContext Context { get { return m_Context; } }

    protected GLOBALMENU_SEQ m_CurrentSeq = GLOBALMENU_SEQ.NONE;
    public GLOBALMENU_SEQ CurrentSeq { get { return m_CurrentSeq; } }
    protected GLOBALMENU_SEQ m_NextSeq = GLOBALMENU_SEQ.NONE;
    public GLOBALMENU_SEQ NextSeq { get { return m_NextSeq; } }
    protected GLOBALMENU_SEQ m_LastSeq = GLOBALMENU_SEQ.NONE;
    public GLOBALMENU_SEQ LastSeq { get { return m_LastSeq; } }

    protected Camera m_Camera = null;
    protected MenuPartsBase m_Panel = null;

    protected bool m_Back = false;
    public bool Back { get { return m_Back; } }

    public class SeqData
    {
        public GlobalMenuSeq m_MenuSeq = null;
        public MasterGlobalMenuSeq m_Master = null;
    };

    protected SeqData[] m_SeqArray = null;

    private GameObject m_UIRoot;
    protected GlobalMenuUIView m_UIView;

    private List<GlobalMenuListItemModel> m_buttons = new List<GlobalMenuListItemModel>();


    public void Awake()
    {
        m_Context = new GlobalMenuContext();
        gameObject.GetComponentInChildren<Canvas>().enabled = false;
        GetComponent<M4uContextRoot>().Context = m_Context;

        GameObject _panel = UnityUtil.GetChildNode(gameObject, "PagePanel");
        m_Panel = _panel.GetComponent<MenuPartsBase>();

        m_SeqArray = new SeqData[(int)GLOBALMENU_SEQ.MAX];
        MasterGlobalMenuSeq[] _masters = Resources.LoadAll<MasterGlobalMenuSeq>("MasterData/GlobalMenuSeq");
        for (int i = 0; i < _masters.Length; i++)
        {
            if (_masters[i].Sequence == GLOBALMENU_SEQ.NONE)
            {
                continue;
            }

            m_SeqArray[(int)_masters[i].Sequence] = new SeqData();
            m_SeqArray[(int)_masters[i].Sequence].m_Master = _masters[i];
        }

        // TODO : レイアウトデータとしてprefabを用意したら消す
        m_UIRoot = UnityUtil.GetChildNode(gameObject, "UIRoot");
        Debug.Assert(m_UIRoot != null, "The UIRoot node not found in GlobalMenu prefab");
        Transform transform = m_UIRoot.transform.Find("RetuenButton");
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.addLocalYPos(transform);
        }

        m_UIView = m_UIRoot.GetComponent<GlobalMenuUIView>();
        Debug.Assert(m_UIView != null, "The GlobalMenuUIView node not found in m_UIRoot node");

        AndroidBackKeyManager.Instance.StackPush(gameObject, OnPushBackKey);
    }

    protected virtual void SetupTopMenu(Camera _camera)
    {
        m_Camera = _camera;

        Canvas[] canvasList = gameObject.GetComponentsInChildren<Canvas>();
        for (int i = 0; i < canvasList.Length; i++)
        {
            canvasList[i].worldCamera = m_Camera;
        }

        GlobalTopMenu _topMenu = gameObject.GetComponentInChildren<GlobalTopMenu>();
        m_SeqArray[(int)GLOBALMENU_SEQ.TOP_MENU].m_MenuSeq = _topMenu;

        _topMenu.ForceActive();
        _topMenu.UpdateCount = 5;
        m_CurrentSeq = GLOBALMENU_SEQ.TOP_MENU;

        StartCoroutine(UnityUtil.DelayFrameAction(1, () =>
        {
            setPanelSize(GLOBALMENU_SEQ.TOP_MENU);
        }));

        GlobalMenuManager.Instance.m_GlobalMenu = this;
    }

    public void addMenuList(GlobalMenuButtonCategory _buttonCategory, GlobalMenuButtonType _buttonType, string title_name, string icon_name, bool bFlag = false)
    {
        Sprite icon = ResourceManager.Instance.Load("btn_" + icon_name);
        Sprite title = ResourceManager.Instance.Load("txt_" + icon_name);

        int index = GetAvailableIndex();
        var buttonType = _buttonType;

        var model = new GlobalMenuListItemModel((uint)index);

        model.OnClicked += () =>
        {
            model.isSelected = true;
            SelectMenu(buttonType);
        };

        m_buttons.Add(model);

        switch (_buttonCategory)
        {
            case GlobalMenuButtonCategory.User:
                m_Context.UserMenuList.Add(new GlobalMenuItem(_buttonType, title, icon, OnPushMenu, bFlag).SetModel(model));
                break;
            case GlobalMenuButtonCategory.Game:
                m_Context.GameMenuList.Add(new GlobalMenuItem(_buttonType, title, icon, OnPushMenu, bFlag).SetModel(model));
                break;
            case GlobalMenuButtonCategory.Other:
                m_Context.OtherMenuList.Add(new GlobalMenuItem(_buttonType, title, icon, OnPushMenu, bFlag).SetModel(model));
                break;
        }
    }

    // 次にm_buttonsに追加する要素のindexを取得する
    private int GetAvailableIndex()
    {
        return m_buttons.Count;
    }

    public GlobalMenuItem getMenuItem(GlobalMenuButtonCategory _buttonCategory, GlobalMenuButtonType _buttonType)
    {
        List<GlobalMenuItem> itemList = null;

        switch (_buttonCategory)
        {
            case GlobalMenuButtonCategory.User:
                itemList = m_Context.UserMenuList;
                break;
            case GlobalMenuButtonCategory.Game:
                itemList = m_Context.GameMenuList;
                break;
            case GlobalMenuButtonCategory.Other:
                itemList = m_Context.OtherMenuList;
                break;
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].Type == _buttonType)
            {
                return itemList[i];
            }
        }

        return null;
    }

    public void SelectMenu(GlobalMenuButtonType buttonType)
    {
        // TODO : 整理
        switch (buttonType)
        {
            case GlobalMenuButtonType.Infomation:
            case GlobalMenuButtonType.Support:
            case GlobalMenuButtonType.Logout:
                OnPushMenu(buttonType);
                break;
            case GlobalMenuButtonType.Item:
            case GlobalMenuButtonType.EventSchedule:
            case GlobalMenuButtonType.Mission:
            case GlobalMenuButtonType.Present:
            case GlobalMenuButtonType.Option:
                m_UIView.ChangeToItem(() =>
                {
                    OnPushMenu(buttonType);
                });

                foreach (var button in m_buttons)
                {
                    button.Close();
                }
                break;
            default:
                m_UIView.Close(() =>
                {
                    OnPushMenu(buttonType);
                });

                foreach (var button in m_buttons)
                {
                    button.Close();
                }
                break;
        }
    }

    public virtual void OnPushMenu(GlobalMenuButtonType _buttonType)
    {
    }

    public virtual void OnPushClose()
    {
    }

    public virtual void OnPushReturn()
    {
        PageSwitch(LastSeq);
    }

    public virtual void OnPushBackKey()
    {
        if (GlobalMenuManager.Instance.IsBusy())
        {
            return;
        }

        if (m_Context.IsActiveReturn)
        {
            OnPushReturn();
        }
        else
        {
            OnPushClose();
        }
    }

    private bool m_isShowed = false;
    public bool isShowed { get { return m_isShowed; } }
    public void Show(System.Action callback = null)
    {
        gameObject.GetComponentInChildren<Canvas>().enabled = true;

        GlobalMenuManagerFSM.Instance.SendFsmEvent("SHOW");

        PlayAnimation(AppearAnimationName, () =>
        {
            if (callback != null)
                callback();

            // The FSM status is ShowWait until here.
            GlobalMenuManagerFSM.Instance.SendFsmNextEvent();

            m_isShowed = true;
        });

        RegisterKeyEventCallback("show_buttons", () =>
        {
            ShowButtons();
        });
    }

    public void ShowPage(GLOBALMENU_SEQ _seq, System.Action callback = null)
    {
        gameObject.GetComponentInChildren<Canvas>().enabled = true;
        GlobalMenuManagerFSM.Instance.SendFsmEvent("SHOW_PAGE");

        setPanelSize(_seq);

        PlayAnimation(AppearPageAnimationName, () =>
        {
            if (callback != null)
            {
                callback();
            }

            PageSwitch(_seq);

            m_isShowed = true;
        });
    }

    public void Hide(System.Action callback = null)
    {
        GlobalMenuManagerFSM.Instance.SendFsmEvent("CLOSE");

        m_isShowed = false;

        var tag = "GlobalMenuHide";
        ButtonBlocker.Instance.Block(tag);

        PlayAnimation(DisappearAnimationName, () =>
        {
            if (callback != null)
            {
                callback();
            }

            ButtonBlocker.Instance.Unblock(tag);

            // The FSM status is CloseWait until here.
            Finish();
        });
    }

    public void Finish()
    {
        m_buttons.Clear();

        AndroidBackKeyManager.Instance.StackPop(gameObject);

        DestroyObject(gameObject);
    }

    public void ShowButtons()
    {
        foreach (var button in m_buttons)
        {
            button.Appear();
        }
    }

    public void ResetButtons()
    {
        foreach (var button in m_buttons)
        {
            button.isSelected = false;
        }
    }

    public void PageSwitch(GLOBALMENU_SEQ _seq, bool bBack = false)
    {
        if (m_NextSeq != GLOBALMENU_SEQ.NONE)
        {
            return;
        }

        SeqData _seqData = m_SeqArray[(int)_seq];

        if (_seqData.m_MenuSeq == null)
        {
            //オブジェクト生成
            GameObject _tmp = Resources.Load("Prefab/GlobalMenu/GlobalMenuSeq") as GameObject;
            if (_tmp == null)
            {
                return;
            }

            GameObject _pageObj = Instantiate(_tmp) as GameObject;
            if (_pageObj == null)
            {
                return;
            }
            _pageObj.transform.SetParent(m_UIRoot.transform, false);

            foreach (MasterGlobalMenuSeq.SequenceObj _obj in _seqData.m_Master.SequenceObjArray)
            {
                if (_obj.gameObj == null)
                {
                    string object_path = "Prefab/" + _obj.object_name;
                    GameObject originObj = Resources.Load(object_path) as GameObject;
                    if (originObj != null)
                    {
                        _obj.gameObj = Instantiate(originObj) as GameObject;
                        _obj.gameObj.transform.SetParent(_pageObj.transform, false);

                    }
                }
            }

            //クラス生成
            m_SeqArray[(int)_seq].m_MenuSeq = _pageObj.AddComponent(Type.GetType(_seqData.m_Master.SequenceName)) as GlobalMenuSeq;
            if (m_SeqArray[(int)_seq].m_MenuSeq == null)
            {
                DestroyObject(_pageObj);
                return;
            }

            //名前変更
            _pageObj.name = m_SeqArray[(int)_seq].m_Master.SequenceName;

            //--------------------------------
            // 初期化中のレイアウトを見られたくないので
            // 一時的に表示しないレイヤーに設定する
            //--------------------------------
            //UnityUtil.SetObjectLayer( _pageObj , LayerMask.NameToLayer( "DRAW_CLIP" ) );

            UnityUtil.SetObjectEnabledOnce(_pageObj, true);

            if (_seq != GLOBALMENU_SEQ.TOP_MENU)
            {
                _seqData.m_MenuSeq.RegisterOnFadeOutFinishedCallback(() =>
                {
                    Destroy(_pageObj);
                });
            }
        }

        m_NextSeq = _seq;
        m_Back = bBack;
        GlobalMenuManagerFSM.Instance.SendFsmEvent("SWITCH_PAGE");
    }

    public void setPanelSize(GLOBALMENU_SEQ _seq)
    {
        if (_seq == GLOBALMENU_SEQ.TOP_MENU)
        {
            m_Context.IsActiveTopMenu = true;
        }
        else
        {
            m_Context.IsActiveTopMenu = false;
            m_Panel.SetPositionAjustStatusBar(new Vector2(-8, -106), new Vector2(-16, -226));
        }
    }

    public GlobalMenuSeq getCurrentPageSeq()
    {
        return m_SeqArray[(int)m_CurrentSeq].m_MenuSeq;
    }

    public GlobalMenuSeq getNextPageSeq()
    {
        return m_SeqArray[(int)m_NextSeq].m_MenuSeq;
    }

    public MasterGlobalMenuSeq getNextMaster()
    {
        return m_SeqArray[(int)m_NextSeq].m_Master;
    }

    public void PageSwitchEnd()
    {
        m_LastSeq = m_CurrentSeq;
        m_CurrentSeq = m_NextSeq;
        m_NextSeq = GLOBALMENU_SEQ.NONE;
    }

    public void SetActiveReturn(bool bFlag)
    {
        m_Context.IsActiveReturn = bFlag;
    }

    /*-------------------------------------------------------------------------------------*/
    /*                                                                                     */
    /*                                                                                     */
    /*                                                                                     */
    /*-------------------------------------------------------------------------------------*/
    public static GlobalMenu Create(GLOBALMENU_TYPE menuType, Camera _camera)
    {
        GlobalMenu newGlobalMenu = null;

        //グローバルメニューは２個同時に開けない
        if (GameObject.FindGameObjectWithTag("GlobalMenu") != null) return null;

        GameObject _tmpObj = Resources.Load("Prefab/GlobalMenu/GlobalMenu") as GameObject;
        if (_tmpObj == null)
        {
            return null;
        }

        GameObject _newObj = Instantiate(_tmpObj) as GameObject;
        if (_newObj == null)
        {
            return null;
        }

        switch (menuType)
        {
            case GLOBALMENU_TYPE.MAIN_MENU:
                newGlobalMenu = _newObj.AddComponent<GlobalMenuForMainMenu>();
                _newObj.name = "GlobalMenuForMainMenu";
                break;
            default:
                newGlobalMenu = _newObj.AddComponent<GlobalMenu>();
                _newObj.name = "GlobalMenu";
                break;
        }

        if (newGlobalMenu == null)
        {
            return null;
        }

        newGlobalMenu.AssignAnimationComponent();


        UnityUtil.SetObjectEnabledOnce(_newObj, true);

        newGlobalMenu.SetupTopMenu(_camera);

        return newGlobalMenu;
    }

    public static GlobalMenuForMainMenu GetGlobalMainMenu()
    {
        GameObject _obj = GameObject.FindGameObjectWithTag("GlobalMenu");
        if (_obj == null)
        {
            return null;
        }

        return _obj.GetComponent<GlobalMenuForMainMenu>();
    }

    // Create()内からのみ呼ぶ
    // TODO : 生成回り整理するときに消す
    public void AssignAnimationComponent()
    {
        m_animation = gameObject.GetComponent<Animation>();
    }

    /// <summary>
    /// ボタンのアニメーションが終了しているかチェックする
    /// </summary>
    /// <returns></returns>
    public bool CheckFinishButtonAnim()
    {
        if (Context.UserMenuList.IsNullOrEmpty() == true ||
            Context.UserMenuList.Any((v) => v.model.isReady == false))
        {
            return false;
        }

        if (Context.GameMenuList.IsNullOrEmpty() == true ||
            Context.GameMenuList.Any((v) => v.model.isReady == false))
        {
            return false;
        }

        if (Context.OtherMenuList.IsNullOrEmpty() == true ||
            Context.OtherMenuList.Any((v) => v.model.isReady == false))
        {
            return false;
        }

        return true;
    }
}
