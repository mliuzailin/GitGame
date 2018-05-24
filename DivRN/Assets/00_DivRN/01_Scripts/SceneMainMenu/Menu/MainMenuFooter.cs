using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;


public class MainMenuFooter : View
{
    [SerializeField]
    private GameObject[] m_buttonRoots = null;

    [SerializeField]
    private GameObject m_particleRoot = null;

    /*==========================================================================*/
    /*		define																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	フッターのメニュータイプ
	*/
    //----------------------------------------------------------------------------
    public enum FOOTER_MENU_TYPE : int
    {
        NONE = -1,      //!< 無し

        HOME,       //!< 「ホーム」
        UNITS,      //!< 「ユニット」
        SCRATCH,    //!< 「スクラッチ」
        SHOP,       //!< 「ショップ」
        QUEST,      //!< 「クエスト」

        MAX,        //!< 制御用
    }

    public GameObject[] ButtonList = null;
    public GameObject subRoot = null;
    public GameObject returnButton = null;

    private M4uProperty<bool> homeenable = new M4uProperty<bool>();
    public bool Homeenable { get { return homeenable.Value; } set { homeenable.Value = value; } }

    private M4uProperty<bool> unitenable = new M4uProperty<bool>();
    public bool Unitenable { get { return unitenable.Value; } set { unitenable.Value = value; } }

    private M4uProperty<bool> scratchenable = new M4uProperty<bool>();
    public bool Scratchenable { get { return scratchenable.Value; } set { scratchenable.Value = value; } }

    private M4uProperty<bool> shopenable = new M4uProperty<bool>();
    public bool Shopenable { get { return shopenable.Value; } set { shopenable.Value = value; } }

    private M4uProperty<bool> helpenable = new M4uProperty<bool>();
    public bool Helpenable { get { return helpenable.Value; } set { helpenable.Value = value; } }

    private M4uProperty<bool> isActiveReturn = new M4uProperty<bool>();
    public bool IsActiveReturn { get { return isActiveReturn.Value; } set { isActiveReturn.Value = value; } }

    private M4uProperty<Sprite> frameImage = new M4uProperty<Sprite>();
    public Sprite FrameImage { get { return frameImage.Value; } set { frameImage.Value = value; } }

    M4uProperty<bool> isHelpBufEvent = new M4uProperty<bool>();
    public bool IsHelpBufEvent { get { return isHelpBufEvent.Value; } set { isHelpBufEvent.Value = value; } }

    M4uProperty<bool> isUnitBufEvent = new M4uProperty<bool>();
    public bool IsUnitBufEvent { get { return isUnitBufEvent.Value; } set { isUnitBufEvent.Value = value; } }

    public System.Action ReturnAction = null;

    private MAINMENU_SEQ[] seqList = {
        MAINMENU_SEQ.SEQ_HOME_MENU,
        MAINMENU_SEQ.SEQ_UNIT_MENU,
        MAINMENU_SEQ.SEQ_GACHA_MAIN,
        MAINMENU_SEQ.SEQ_SHOP_MENU,
        MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY
    };

    public static readonly string AppearAnimationName = "mainmenu_footer_appear";
    public static readonly string DisappearAnimationName = "mainmenu_footer_disappear";
    public static readonly string DefaultAnimationName = "mainmenu_footer_loop";

    private bool m_ActiveFooterTouch = true;
    private FOOTER_MENU_TYPE m_CurrentMenuType = FOOTER_MENU_TYPE.NONE;
    private GameObject m_SubMenuObject = null;
    private FooterSubMenu m_FooterSubMenu = null;
    private bool m_ActiveReturnBuffer = false;

    private MainMenuFooterModel m_footer = null;
    private List<MainMenuFooterButtonModel> m_buttons = new List<MainMenuFooterButtonModel>();
    private bool m_helpBufEvent;
    private bool m_unitBufEvent;
    private bool m_isFooterAppeared = false;
    public bool isFooterAppeared { get { return m_isFooterAppeared; } }
    private MainMenuFooterHelpButton m_helpButton = null;
    private MainMenuFooterUnitsButton m_unitsButton = null;
    private MainMenuFooterGachaButton m_gachaButton = null;


    void Awake()
    {
        m_isFooterAppeared = false;
    }

    // Use this for initialization
    void Start()
    {
        setMenuType(FOOTER_MENU_TYPE.HOME);

        FrameImage = ResourceManager.Instance.Load("big_footer", ResourceType.Menu);

        MainMenuManager.Instance.RegisterOnCategoryChangedCallback((MAINMENU_CATEGORY category) =>
        {
            FOOTER_MENU_TYPE type = ConvertCategoryToFooterMenuType(category);
            if (type != m_CurrentMenuType && type != FOOTER_MENU_TYPE.NONE && type != FOOTER_MENU_TYPE.MAX)
            {
                setMenuType(type);
            }

            UpdateButtonSelected(category);
            UpdateBufEvent();
        });

        UpdateButtonSelected(MainMenuManager.Instance.currentCategory);
        IsHelpBufEvent = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize()
    {
        GetComponent<M4uContextRoot>().Context = this;

        SetModel();
        AddButtons();
        SetUpAppearAnimation();
    }

    // TODO : Mainmenumanagerを解体したらここも整理
    public MainMenuFooterModel GetModel()
    {
        return m_footer;
    }

    override public void FinishAnimation(string animationName)
    {
        var defaultAnimationEventMap = new Dictionary<string, System.Action>
        {
            {
                AppearAnimationName ,
                ()=>
                {
                    m_footer.FinishAppearingAnimation();
                    PlayAnimation(DefaultAnimationName);
                }
            },
            {
                DisappearAnimationName ,
                ()=>
                {
                    m_footer.FinishDisappearingAnimation();
                }
            }
        };

        if (defaultAnimationEventMap.ContainsKey(animationName))
        {
            defaultAnimationEventMap[animationName]();
        }

        base.FinishAnimation(animationName);
    }


    public void ShowButton(int index)
    {
        m_buttons[index].Appear();
    }

    public void ShowParticle()
    {
        while (m_particleRoot.transform.childCount > 0)
        {
            var child = m_particleRoot.transform.GetChild(0);
            GameObject.Destroy(child);
        }
        SplashParticleView.Attach(m_particleRoot);
    }

    private void SetModel()
    {
        m_helpBufEvent = MainMenuUtil.checkHelpBufEvent();
        CheckSkillUpEvent();
        m_unitBufEvent = (MainMenuParam.m_BlendBuildEventSLV != 0);
        m_footer = new MainMenuFooterModel();

        m_footer.OnDisappearingBegan += () =>
        {
            PlayAnimation(DisappearAnimationName);
        };

        m_footer.OnAppeared += () =>
        {
            m_isFooterAppeared = true;
            m_helpButton = ButtonList[(int)FOOTER_MENU_TYPE.QUEST].GetComponentInChildren<MainMenuFooterHelpButton>();
            if (m_helpButton != null)
            {
                if (MainMenuManager.Instance != null
               && MainMenuManager.Instance.currentCategory == MAINMENU_CATEGORY.QUEST)
                {
                    m_helpButton.setBufEvent(false);
                }
                else
                {
                    if (MainMenuParam.m_PartyAssignPrevPage == MAINMENU_SEQ.SEQ_QUEST_SELECT_PARTY)
                    {
                        m_helpButton.setBufEvent(false);
                    }
                    else
                    {
                        m_helpButton.setBufEvent(true);
                    }
                }
            }
            m_unitsButton = ButtonList[(int)FOOTER_MENU_TYPE.UNITS].GetComponentInChildren<MainMenuFooterUnitsButton>();
            if (m_unitsButton != null)
            {
                if (MainMenuManager.Instance != null
               && MainMenuManager.Instance.currentCategory == MAINMENU_CATEGORY.UNIT)
                {
                    m_unitsButton.setBufEvent(false);
                }
                else
                {
                    m_unitsButton.setBufEvent(true);
                }
            }
            m_gachaButton = ButtonList[(int)FOOTER_MENU_TYPE.SCRATCH].GetComponentInChildren<MainMenuFooterGachaButton>();
            if (m_gachaButton != null)
            {
                bool bFlag = MasterDataUtil.CheckFirstTimeFree();
                m_gachaButton.SetFlag(bFlag);
            }
        };
    }

    private void AddButtons()
    {
        var createViewMap = new List<System.Func<GameObject, MainMenuFooterButtonModel, ButtonView>>
        {
            (GameObject parent, MainMenuFooterButtonModel model)=>
            {
                return MainMenuFooterHomeButton.
                                Attach(parent).
                                SetModel(model.AddCategory(MAINMENU_CATEGORY.HOME));
            },
            (GameObject parent, MainMenuFooterButtonModel model)=>
            {
                return MainMenuFooterUnitsButton.
                                Attach(parent).
                                SetModel(model.AddCategory(MAINMENU_CATEGORY.UNIT), m_unitBufEvent);
            },
            (GameObject parent, MainMenuFooterButtonModel model)=>
            {
                return MainMenuFooterGachaButton.
                                Attach(parent).
                                SetModel(model.AddCategory(MAINMENU_CATEGORY.GACHA));
            },
            (GameObject parent, MainMenuFooterButtonModel model)=>
            {
                return MainMenuFooterShopButton.
                                Attach(parent).
                                SetModel(model.AddCategory(MAINMENU_CATEGORY.SHOP));
            },
            (GameObject parent, MainMenuFooterButtonModel model)=>
            {
                return MainMenuFooterHelpButton.
                                Attach(parent).
                                SetModel(model.AddCategory(MAINMENU_CATEGORY.QUEST), m_helpBufEvent);
            }
        };

        int size = m_buttonRoots.Length;

        UnityEngine.Debug.Assert(createViewMap.Count == size, "The count of footer button gameObjects is invalid.");

        for (int i = 0; i < size; i++)
        {
            int index = i;
            var button = new MainMenuFooterButtonModel();
            button.OnClicked += () =>
            {
                // TODO : OnTouchMenuが整理されたらここも新しい処理に切り替える
                OnTouchMenu((FOOTER_MENU_TYPE)index);
            };

            ButtonList[index] = createViewMap[index](m_buttonRoots[index], button).GetRoot();

            m_buttons.Add(button);
        }

        m_buttons[0].isSelected = true;

        //戻るボタン
        {
            var retBtnModel = new ButtonModel();
            retBtnModel.OnClicked += () =>
            {
                OnSelectReturn();
            };

            MainMenuFooterReturnButton.Attach(returnButton).SetModel(retBtnModel);

            retBtnModel.Appear();
        }
    }

    // TODO : 整理
    private static bool s_isAlreadyAppear = false;
    private void SetUpAppearAnimation()
    {
        EffectProcessor.Instance.Register("MainMenuFooter", (System.Action finish) =>
        {
            System.Action skipFunc = () =>
            {
                foreach (var buttonRoot in m_buttonRoots)
                {
                    buttonRoot.SetActive(true);
                }

                foreach (var button in m_buttons)
                {
                    button.SkipAppearing();
                }

                FinishAnimation(AppearAnimationName);
            };

            bool isReady = false;

            // TODO : 整理
            if (s_isAlreadyAppear)
            {
                skipFunc();
                finish();
                return;
            }

            PlayAnimation(AppearAnimationName, () =>
            {
                isReady = true;
            });

            InputLayer.Instance.OnAnyTouchBeganCallbackOnce = (Vector3 touchPosition) =>
            {
                if (isReady)
                {
                    return;
                }

                skipFunc();
            };

            s_isAlreadyAppear = true;

            finish();
        });
    }


    /// <summary>
    /// メニュータイプ設定
    /// </summary>
    /// <param name="type"></param>
	public void setMenuType(FOOTER_MENU_TYPE type)
    {
        var enableFlagReferenceMap = new Dictionary<int, M4uProperty<bool>>
        {
            { (int)FOOTER_MENU_TYPE.HOME, homeenable},
            { (int)FOOTER_MENU_TYPE.UNITS, unitenable},
            { (int)FOOTER_MENU_TYPE.SCRATCH, scratchenable},
            { (int)FOOTER_MENU_TYPE.SHOP, shopenable},
            { (int)FOOTER_MENU_TYPE.QUEST, helpenable}
        };

        m_CurrentMenuType = type;

        for (int _index = 0; _index < (int)FOOTER_MENU_TYPE.MAX; _index++)
        {
            enableFlagReferenceMap[_index].Value = m_ActiveFooterTouch;
        }
    }


    /// <summary>
    /// アクティブ設定
    /// </summary>
    /// <param name="_flag"></param>
    public void setActiveFlag(bool _flag)
    {
        if (m_ActiveFooterTouch == _flag)
        {
            return;
        }

        m_ActiveFooterTouch = _flag;
        if (IsActiveReturn ||
            m_ActiveReturnBuffer)
        {
            m_ActiveReturnBuffer = IsActiveReturn;
            IsActiveReturn = _flag;
        }

        foreach (var button in m_buttons)
        {
            button.isEnabled = m_ActiveFooterTouch;
        }

        setMenuType(m_CurrentMenuType);
        UpdateImages();
    }

    /// <summary>
    /// メニュー選択
    /// </summary>
    /// <param name="type"></param>
	public void OnTouchMenu(FOOTER_MENU_TYPE type)
    {
        bool bChange = false;

        if (MainMenuManager.HasInstance)
        {
            if (m_CurrentMenuType == type &&
                MainMenuManager.Instance.CheckPageActive(seqList[(int)type]))
            {
                if (IsSubMenuOpened())
                {
                    closeSubMenu();
                }

                return;
            }

            switch (type)
            {
                case FOOTER_MENU_TYPE.UNITS:
                    if (m_FooterSubMenu != null
                        && m_FooterSubMenu.Category == MAINMENU_CATEGORY.UNIT)
                    {
                        SoundUtil.PlaySE(SEID.SE_MENU_RET);
                    }
                    else
                    {
                        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_FOOT_UNIT);
                    }

                    openSubMenu(type, MAINMENU_CATEGORY.UNIT);
                    break;

                case FOOTER_MENU_TYPE.SHOP:
                    if (m_FooterSubMenu != null
                        && m_FooterSubMenu.Category == MAINMENU_CATEGORY.SHOP)
                    {
                        SoundUtil.PlaySE(SEID.SE_MENU_RET);
                    }
                    else
                    {
                        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_FOOT_SHOP);
                    }

                    openSubMenu(type, MAINMENU_CATEGORY.SHOP);
                    break;

                case FOOTER_MENU_TYPE.QUEST:
                    {
                        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_FOOT_QUEST);

                        //クエスト選択画面と成長ボス選択画面以外は選択保存情報へ遷移
                        if (MainMenuManager.Instance.WorkSwitchPageNow != MAINMENU_SEQ.SEQ_QUEST_SELECT &&
                            MainMenuManager.Instance.WorkSwitchPageNow != MAINMENU_SEQ.SEQ_CHALLENGE_SELECT)
                        {
                            //選択保存情報へ遷移
                            bChange = MainMenuParam.SetupSaveSelect();
                        }

                        if (bChange == false)
                        {
                            //失敗した場合はデフォルト位置へ
                            MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY.STORY);
                            bChange = MainMenuManager.Instance.AddSwitchRequest(seqList[(int)type], false, false);
                        }
                        closeSubMenu();
                    }
                    break;
                case FOOTER_MENU_TYPE.SCRATCH:
                    {
                        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_FOOT_SCRATCH);

                        MainMenuParam.m_GachaMaster = null;
                        MasterDataGacha[] gachaArray = MasterDataUtil.GetActiveGachaMaster();
                        if (gachaArray != null && gachaArray.Length > 0)
                        {
                            MainMenuParam.m_GachaMaster = gachaArray[0];
                        }

                        bChange = MainMenuManager.Instance.AddSwitchRequest(seqList[(int)type], false, false);
                        closeSubMenu();
                    }
                    break;
                case FOOTER_MENU_TYPE.HOME:
                    {
                        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_HOME);
                        bChange = MainMenuManager.Instance.AddSwitchRequest(seqList[(int)type], false, false);
                        closeSubMenu();
                    }
                    break;
            }
        }
        else
        {
            if (m_CurrentMenuType == type)
            {
                if (IsSubMenuOpened())
                {
                    closeSubMenu();
                }

                return;
            }
            bChange = true;
        }

        if (bChange)
        {
            setMenuType(type);
        }
    }


    private void openSubMenu(FOOTER_MENU_TYPE _type, MAINMENU_CATEGORY _category)
    {
        if (m_FooterSubMenu != null)
        {
            MAINMENU_CATEGORY _prevCategory = m_FooterSubMenu.Category;
            closeSubMenu();
            if (_prevCategory == _category)
            {
                return;
            }
        }
        if (m_SubMenuObject == null)
        {
            if (_type != FOOTER_MENU_TYPE.UNITS)
            {
                m_SubMenuObject = Resources.Load("Prefab/MainMenu/FooterSubMenu") as GameObject;
            }
            else
            {
                m_SubMenuObject = Resources.Load("Prefab/MainMenu/FooterSubMenuDouble") as GameObject;
            }
            if (m_SubMenuObject == null)
            {
                return;
            }
        }

        //ユニットのときはスキルアップイベントのチェック
        if (_category == MAINMENU_CATEGORY.UNIT)
        {
            CheckSkillUpEvent();
        }

        GameObject _insObj = Instantiate(m_SubMenuObject);
        if (_insObj == null)
        {
            return;
        }
        _insObj.transform.SetParent(ButtonList[(int)_type].transform, false);
        _insObj.transform.SetParent(subRoot.transform, true);

        m_FooterSubMenu = _insObj.GetComponent<FooterSubMenu>();
        m_FooterSubMenu.setup(_category);
        m_FooterSubMenu.DidSelectClose = () =>
        {
            closeSubMenu();
        };
        m_FooterSubMenu.DidSelectCancel = () =>
        {
            closeSubMenu();
        };

        m_FooterSubMenu.Show();

        foreach (var eachButton in m_buttons)
        {
            eachButton.isSelected = eachButton.IsCategoryOf(_category);
        }

        AndroidBackKeyManager.Instance.StackPush(m_FooterSubMenu.gameObject, OnSelectBackKey);
    }

    public void closeSubMenu()
    {
        if (m_FooterSubMenu == null)
        {
            return;
        }

        AndroidBackKeyManager.Instance.StackPop(m_FooterSubMenu.gameObject);

        m_FooterSubMenu.close();
        m_FooterSubMenu = null;
        m_SubMenuObject = null;


        UpdateButtonSelected(MainMenuManager.Instance.currentCategory);
        UpdateBufEvent();
    }

    private bool IsSubMenuOpened()
    {
        return m_FooterSubMenu != null;
    }

    private void OnSelectBackKey()
    {
        closeSubMenu();
    }

    private void UpdateButtonSelected(MAINMENU_CATEGORY category)
    {
        bool anyButtonsNotSelected = true;
        foreach (var eachButton in m_buttons)
        {
            eachButton.isSelected = eachButton.IsCategoryOf(category);

            if (eachButton.isSelected)
            {
                anyButtonsNotSelected = false;
            }
        }

        if (anyButtonsNotSelected)
        {
            m_buttons[0].isSelected = true;
        }
    }

    /// <summary>
    /// 戻るボタン選択
    /// </summary>
    public void OnSelectReturn()
    {
        bool bSE = false;
        try
        {
            MainMenuSeq pageNow = MainMenuManager.Instance.MainMenuSeqPageNow;
            if (MainMenuManager.Instance.IsPageSwitch() ||          //ページ切り替え中
                ServerApi.IsExists ||                               //通信中
                (pageNow != null && pageNow.IsSuspendReturn))     //戻るボタン抑制中
            {
                return;
            }

            if (ReturnAction != null)
            {
                ReturnAction();
                bSE = true;
            }

            if (MainMenuParam.m_PageBack.Count == 0)
            {
                return;
            }


            if (MainMenuManager.HasInstance)
            {
                MAINMENU_SEQ eNextPage = MainMenuParam.m_PageBack.Pop();
                if (MainMenuManager.Instance.AddSwitchRequest(eNextPage, false, true) == false)
                {
                    MainMenuParam.m_PageBack.Push(eNextPage);
                }
                else
                {
                    bSE = true;
                }
            }
            return;
        }
        finally
        {
            if (bSE)
            {
                SoundUtil.PlaySE(SEID.SE_MENU_RET);
            }
        }
    }

    private void UpdateImages()
    {
        FrameImage = m_ActiveFooterTouch
                    ? ResourceManager.Instance.Load("big_footer", ResourceType.Menu)
                    : ResourceManager.Instance.Load("big_footer_off", ResourceType.Menu);
    }

    private void CheckSkillUpEvent()
    {
        //----------------------------------------
        // スキルレベルアップ確率上昇イベントの発生判定
        //----------------------------------------
        MasterDataEvent cHitEventMaster = null;
        MainMenuParam.m_BlendBuildEventSLV = 0;

        //--------------------------------
        // スキルレベルアップ確率上昇イベントIDをリスト化
        // @change Developer 2016/08/04 v360
        //--------------------------------
        uint[] aunEventSLVList = {
                                    GlobalDefine.SLV_EVENT_ID_x1000,	// スキルレベルアップ確率増加イベントID：10.0倍
									GlobalDefine.SLV_EVENT_ID_x0500,	// スキルレベルアップ確率増加イベントID：5.0倍
									GlobalDefine.SLV_EVENT_ID_x0400,	// スキルレベルアップ確率増加イベントID：4.0倍
									GlobalDefine.SLV_EVENT_ID_x0300,	// スキルレベルアップ確率増加イベントID：3.0倍
									GlobalDefine.SLV_EVENT_ID_x0250,	// スキルレベルアップ確率増加イベントID：2.5倍
									GlobalDefine.SLV_EVENT_ID_x0200,	// スキルレベルアップ確率増加イベントID：2.0倍
									GlobalDefine.SLV_EVENT_ID_x0150	// スキルレベルアップ確率増加イベントID：1.5倍
								};

        //--------------------------------
        // イベント期間判定
        // @add Developer 2016/08/04 v360
        //--------------------------------
        MasterDataEvent cTempEventMaster;
        uint unTimingStart = 0;
        uint unTimingEnd = 0;
        uint unFixEndTime = 0;
        for (int num = 0; num < aunEventSLVList.Length; ++num)
        {
            cTempEventMaster = MasterDataUtil.GetMasterDataEventFromID(aunEventSLVList[num]);
            if (cTempEventMaster == null)
            {
                continue;
            }

            //--------------------------------
            // 期間指定タイプによる分岐
            //--------------------------------
            switch (cTempEventMaster.period_type)
            {
                // 指定(従来通り)
                default:
                case MasterDataDefineLabel.PeriodType.DESIGNATION:
                    unTimingStart = cTempEventMaster.timing_start;
                    unTimingEnd = cTempEventMaster.timing_end;
                    break;

                // サイクル
                case MasterDataDefineLabel.PeriodType.CYCLE:
                    if (cHitEventMaster != null
                    || TimeEventManager.Instance == null)
                    {
                        continue;
                    }

                    // 開催期間を取得
                    CycleParam cCycleParam = TimeEventManager.Instance.GetEventCycleParam(cTempEventMaster.event_id);
                    if (cCycleParam == null)
                    {
                        continue;
                    }

                    unTimingStart = cCycleParam.timingStart;
                    unTimingEnd = cCycleParam.timingEnd;
                    break;
            }

            //--------------------------------
            // イベント期間判定
            //--------------------------------
            bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(unTimingStart, unTimingEnd);
            if (bCheckWithinTime == false)
            {
                continue;
            }

            // 各種情報を設定
            unFixEndTime = unTimingEnd;         // イベント終了時間
            cHitEventMaster = cTempEventMaster;     // イベントマスター

            // 従来通りならイベント判定終了(指定優先)
            if (cTempEventMaster.period_type != MasterDataDefineLabel.PeriodType.CYCLE)
            {
                break;
            }
        }

        //--------------------------------
        // イベント終了時のフロー戻しがキツイので、
        // やっぱりチュートリアル中はイベント開催を検知しない
        //--------------------------------
        if (TutorialManager.IsExists)
        {
            cHitEventMaster = null;
        }
        //-------------------------------
        // イベント開催中か否かで表示分岐
        // サイクル対応、無期限対応
        // @change Developer 2016/08/04 v360
        //-------------------------------
        if (cHitEventMaster != null)
        {
            MainMenuParam.m_BlendBuildEventSLV = cHitEventMaster.event_id;
        }
    }

    FOOTER_MENU_TYPE ConvertCategoryToFooterMenuType(MAINMENU_CATEGORY category)
    {
        FOOTER_MENU_TYPE type = FOOTER_MENU_TYPE.NONE;
        switch (category)
        {
            case MAINMENU_CATEGORY.UNIT:
                //type = FOOTER_MENU_TYPE.UNITS;
                break;
            case MAINMENU_CATEGORY.SHOP:
                //type = FOOTER_MENU_TYPE.SHOP;
                break;
            case MAINMENU_CATEGORY.HOME:
                type = FOOTER_MENU_TYPE.HOME;
                break;
            case MAINMENU_CATEGORY.GACHA:
                type = FOOTER_MENU_TYPE.SCRATCH;
                break;
            case MAINMENU_CATEGORY.QUEST:
                type = FOOTER_MENU_TYPE.QUEST;
                break;
        }
        return type;
    }

    public float getHelpButtanAnimationTime()
    {
        if (m_helpButton == null)
        {
            return 0;
        }

        return m_helpButton.getAnimationTime();
    }

    public float getUnitsButtanAnimationTime()
    {
        if (m_unitsButton == null)
        {
            return 0;
        }

        return m_unitsButton.getAnimationTime();
    }

    public float getFooterBufAnimationTime()
    {
        float ret = 0;

        ret = getHelpButtanAnimationTime();
        if (ret != 0)
        {
            return ret;
        }

        ret = getUnitsButtanAnimationTime();

        return ret;
    }

    private void UpdateBufEvent()
    {
        m_helpBufEvent = MainMenuUtil.checkHelpBufEvent();
        CheckSkillUpEvent();
        m_unitBufEvent = (MainMenuParam.m_BlendBuildEventSLV != 0);
        if (m_helpButton != null)
        {
            m_helpButton.IsBufEvent = m_helpBufEvent;
            if (MainMenuManager.Instance != null
           && MainMenuManager.Instance.currentCategory == MAINMENU_CATEGORY.QUEST)
            {
                m_helpButton.setBufEvent(false);
            }
            else
            {
                if (MainMenuParam.m_PartyAssignPrevPage == MAINMENU_SEQ.SEQ_QUEST_SELECT_PARTY)
                {
                    m_helpButton.setBufEvent(false);
                }
                else
                {
                    m_helpButton.setBufEvent(true);
                }
            }
        }
        if (m_unitsButton != null)
        {
            m_unitsButton.IsBufEvent = m_unitBufEvent;
            if (MainMenuManager.Instance != null
           && MainMenuManager.Instance.currentCategory == MAINMENU_CATEGORY.UNIT)
            {
                m_unitsButton.setBufEvent(false);
            }
            else
            {
                m_unitsButton.setBufEvent(true);
            }
        }
        if (m_gachaButton != null)
        {
            bool bFlag = MasterDataUtil.CheckFirstTimeFree();
            m_gachaButton.SetFlag(bFlag);
        }
    }
}
