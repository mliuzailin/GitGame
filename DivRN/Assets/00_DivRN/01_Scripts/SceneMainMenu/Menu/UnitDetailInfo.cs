using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class UnitDetailInfo : M4uContextMonoBehaviour
{
    private readonly int UPDATE_LAYOUT_COUNT = 5;
    public enum StatusType
    {
        None = -1,
        Unit,
        Catalog,
        Max,
    }

    public enum ToggleType
    {
        None = -1,
        Status = 0,
        Skill,
        Link,
        Evolve,
        //Equip,
        Max,
    }

    public class EvolveInfo
    {
        public MasterDataParamChara charaMaster;
        public MasterDataParamCharaEvol evolveMaster;
        public Texture2D charaTexture;
        public bool nameView;
    };


    public AssetAutoSetCharaDetail CharaImage = null;
    public GameObject MainCanvas = null;
    public CanvasGroup WindowRoot = null;

    public MenuPartsBase BGWindow = null;
    public MenuPartsBase CharaImageParts = null;
    public MenuPartsBase ShadowImageParts = null;

    public UnitDetailSkillDialog SkillDialog = null;
    public UnitDetailLinkDialog LinkDialog = null;

    //public GameObject[] ButtonPosList = null;

    private M4uProperty<bool> isViewInfo = new M4uProperty<bool>();
    public bool IsViewInfo { get { return isViewInfo.Value; } set { isViewInfo.Value = value; } }

    private M4uProperty<string> favoriteLabel = new M4uProperty<string>();
    public string FavoriteLabel { get { return favoriteLabel.Value; } set { favoriteLabel.Value = value; } }

    private M4uProperty<bool> isViewEvolveNone = new M4uProperty<bool>();
    public bool IsViewEvolveNone { get { return isViewEvolveNone.Value; } set { isViewEvolveNone.Value = value; } }

    private M4uProperty<string> evolveNoneText = new M4uProperty<string>();
    public string EvolveNoneText { get { return evolveNoneText.Value; } set { evolveNoneText.Value = value; } }

    private M4uProperty<List<UnitDetailToggleContext>> toggleList = new M4uProperty<List<UnitDetailToggleContext>>();
    public List<UnitDetailToggleContext> ToggleList { get { return toggleList.Value; } set { toggleList.Value = value; } }

    private M4uProperty<bool> isViewImage = new M4uProperty<bool>();
    public bool IsViewImage { get { return isViewImage.Value; } set { isViewImage.Value = value; } }

    private M4uProperty<string> illustratorName = new M4uProperty<string>();
    public string IllustratorName { get { return illustratorName.Value; } set { illustratorName.Value = value; } }

    private M4uProperty<Sprite> returnImage = new M4uProperty<Sprite>();
    public Sprite ReturnImage { get { return returnImage.Value; } set { returnImage.Value = value; } }

    public bool IsViewCharaCount
    {
        get { return m_UnitDetailPanel.IsViewCharaCount; }
        set { m_UnitDetailPanel.IsViewCharaCount = value; }
    }

    private UnitNamePanel m_UnitNamePanel = null;
    //Info
    private UnitParamPanel m_UnitParamPanel = null;
    private UnitStoryPanel m_UnitStoryPanel = null;
    //Skill
    private UnitSkillPanel m_UnitSkillPanel = null;
    //Link
    private UnitLinkPanel m_UnitLinkPanel = null;
    //Evolve
    private UnitEvolveList m_UnitEvolvePanel = null;

    private UnitDetailPanel m_UnitDetailPanel = null;

    private Canvas m_MainCanvas = null;
    private StatusType m_StatusType = StatusType.None;
    private ToggleType m_CurrentType = ToggleType.None;
    private UnitLinkPanel.LinkParamType m_LinkType = UnitLinkPanel.LinkParamType.Link;
    private bool m_bSetting = false;
    private uint m_UnitId = 0;
    private bool m_bEvolve = false;
    private bool m_bFavorite = false;
    private PacketStructUnit m_MainUnit = null;
    private MasterDataParamChara m_MainUnitMaster = null;
    private MasterDataParamCharaEvol m_MainUnitEvolMaster = null;
    private PacketStructUnit m_SubUnit = null;
    private UnitGridContext m_MainUnitContext = null;
    private int m_LastUpdateCount = 0;
    private bool backkey = true;
    private List<EvolveInfo> m_EvolveList = new List<EvolveInfo>();

    private bool[] m_Setup = new bool[(int)ToggleType.Max];

    private System.Action m_CloseAction = delegate { };
    private System.Action m_ReadyAction = delegate { };

    private bool m_CharaScreen;
    public bool charaScreen { set { m_CharaScreen = value; } }

    private void Awake()
    {
        AndroidBackKeyManager.Instance.DisableBackKey();

        GetComponent<M4uContextRoot>().Context = this;

        GameObject rootObject = WindowRoot.gameObject;

        if (SceneObjReferMainMenu.HasInstance)
        {
            m_UnitNamePanel = UnityUtil.SetupPrefab<UnitNamePanel>(SceneObjReferMainMenu.Instance.m_UnitNamePanel, MainCanvas);
            m_UnitParamPanel = UnityUtil.SetupPrefab<UnitParamPanel>(SceneObjReferMainMenu.Instance.m_UnitParamPanel, rootObject);
            m_UnitStoryPanel = UnityUtil.SetupPrefab<UnitStoryPanel>(SceneObjReferMainMenu.Instance.m_UnitStoryPanel, m_UnitParamPanel.gameObject);
            m_UnitSkillPanel = UnityUtil.SetupPrefab<UnitSkillPanel>(SceneObjReferMainMenu.Instance.m_UnitSkillPanel, rootObject);
            m_UnitLinkPanel = UnityUtil.SetupPrefab<UnitLinkPanel>(SceneObjReferMainMenu.Instance.m_UnitLinkPanel, rootObject);
            m_UnitEvolvePanel = UnityUtil.SetupPrefab<UnitEvolveList>(SceneObjReferMainMenu.Instance.m_UnitEvolveListPanel, rootObject);
            m_UnitDetailPanel = UnityUtil.SetupPrefab<UnitDetailPanel>(SceneObjReferMainMenu.Instance.m_UnitDetailPanel, rootObject);
        }
        else
        {
            m_UnitNamePanel = UnityUtil.SetupPrefab<UnitNamePanel>("Prefab/UnitNamePanel/UnitNamePanel", MainCanvas);
            m_UnitParamPanel = UnityUtil.SetupPrefab<UnitParamPanel>("Prefab/UnitParamPanel/UnitParamPanel", rootObject);
            m_UnitStoryPanel = UnityUtil.SetupPrefab<UnitStoryPanel>("Prefab/UnitStoryPanel/UnitStoryPanel", m_UnitParamPanel.gameObject);
            m_UnitSkillPanel = UnityUtil.SetupPrefab<UnitSkillPanel>("Prefab/UnitSkillPanel/UnitSkillPanel", rootObject);
            m_UnitLinkPanel = UnityUtil.SetupPrefab<UnitLinkPanel>("Prefab/UnitLinkPanel/UnitLinkPanel", rootObject);
            m_UnitEvolvePanel = UnityUtil.SetupPrefab<UnitEvolveList>("Prefab/UnitEvolveList/UnitEvolveList", rootObject);
            m_UnitDetailPanel = UnityUtil.SetupPrefab<UnitDetailPanel>("Prefab/UnitDetailPanel/UnitDetailPanel", rootObject);
        }

        //
        LayoutElement element = m_UnitStoryPanel.gameObject.AddComponent<LayoutElement>();
        element.ignoreLayout = true;

        m_MainCanvas = MainCanvas.GetComponent<Canvas>();

        IsViewInfo = true;
        ToggleList = new List<UnitDetailToggleContext>();
        ToggleList.Add(new UnitDetailToggleContext(ToggleType.Status, new Vector2(-128, -8), "status", OnSelectToggle, 1));
        ToggleList.Add(new UnitDetailToggleContext(ToggleType.Skill, new Vector2(0, -17), "skill", OnSelectToggle, 10));
        ToggleList.Add(new UnitDetailToggleContext(ToggleType.Link, new Vector2(128, -8), "link", OnSelectToggle, 1));
        ToggleList.Add(new UnitDetailToggleContext(ToggleType.Evolve, new Vector2(256, 13), "evol", OnSelectToggle, -20));
        m_UnitDetailPanel.selectTogglr = OnSelectToggle;
        m_UnitDetailPanel.selectSkill = OnSelectMainUnitSkillButton;
        m_UnitDetailPanel.selectLoupe = OnSelectCharaScreen;
        m_UnitDetailPanel.selectLink = OnSelectLinkButton;
        m_UnitDetailPanel.selectFavorite = OnSelectFavorite;

        m_UnitDetailPanel.IsViewFavorite = false;
        FavoriteLabel = "お気に入り";
        IllustratorName = "";

        ReturnImage = ResourceManager.Instance.Load("back2");

        if (SafeAreaControl.HasInstance)
        {
            Transform canvasTransform = gameObject.transform.Find("Canvas");
            Transform transform = canvasTransform.Find("ReturnButton");
            SafeAreaControl.Instance.addLocalYPos(transform);
        }

        m_MainCanvas.enabled = false;
        //UnityUtil.SetObjectLayer( gameObject, LayerMask.NameToLayer("DRAW_CLIP"));

        AndroidBackKeyManager.Instance.StackPush(gameObject, OnSelectReturn);
    }

    public void setting()
    {
        CharaImage.ImageScale = 1.25f;
        BGWindow.SetPositionAjustStatusBar(new Vector2(0, -67), new Vector2(-8, -134));
        CharaImageParts.SetTopAndBottomAjustStatusBar(new Vector2(-53, -444));
        ShadowImageParts.SetTopAndBottomAjustStatusBar(new Vector2(-53, -444));

        //
        m_UnitNamePanel.SetPositionAjustStatusBar(new Vector2(-16, -135));

        //
        m_UnitParamPanel.SetPositionAjustStatusBar(new Vector2(0, -236));
        m_UnitParamPanel.IsActiveBG = true;

        //
        m_UnitStoryPanel.SetPosition(new Vector2(0, -8));

        //
        m_UnitSkillPanel.SetPositionAjustStatusBar(new Vector2(0, -29), new Vector2(-40, -413));

        //
        m_UnitLinkPanel.SetPositionAjustStatusBar(new Vector2(0, -29), new Vector2(-40, -413));

        //
        m_UnitEvolvePanel.SetPositionAjustStatusBar(new Vector2(0, -120.75f), new Vector2(-16, -238.5f));

        //
        m_UnitDetailPanel.SetPositionAjustStatusBar(new Vector2(0, -117.75f), new Vector2(-16, -235.5f));

        //
        IsViewEvolveNone = false;
        EvolveNoneText = GameTextUtil.GetText("unit_evolution1");

        UnityUtil.SetObjectEnabledOnce(m_UnitParamPanel.gameObject, false);
        UnityUtil.SetObjectEnabledOnce(m_UnitStoryPanel.gameObject, false);
        UnityUtil.SetObjectEnabledOnce(m_UnitSkillPanel.gameObject, false);
        UnityUtil.SetObjectEnabledOnce(m_UnitLinkPanel.gameObject, false);
        UnityUtil.SetObjectEnabledOnce(m_UnitEvolvePanel.gameObject, false);
        UnityUtil.SetObjectEnabledOnce(m_UnitDetailPanel.gameObject, false);

        for (int i = 0; i < (int)ToggleType.Max; i++) m_Setup[i] = false;

        m_bSetting = true;
    }

    public void setCamera(Camera camera)
    {
        GetComponentInChildren<Canvas>().worldCamera = camera;
        GetComponentInChildren<Canvas>().sortingOrder = 16;
    }

    // Use this for initialization
    void Start()
    {
        m_CurrentType = ToggleType.Status;
        setting();
    }

    // Update is called once per frame
    void Update()
    {
        //キャラクタの準備ができたら表示
        if (IsReady())
        {
            if (m_MainCanvas.enabled == false)
            {
                m_MainCanvas.enabled = true;
                //
                m_ReadyAction();
                //ローディング表示終了
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.GUARD);
                CharaImage.setAlpha(true);
                IsViewImage = false;
                m_UnitDetailPanel.setup(m_UnitId, m_MainUnit, m_SubUnit, CharaImage.charaImage, CharaImage.shadowImage);
                UnityUtil.SetObjectEnabledOnce(m_UnitDetailPanel.gameObject, true);
                AndroidBackKeyManager.Instance.EnableBackKey();
                if (m_CharaScreen == true)
                {
                    setActiveStatus(m_CurrentType, false);
                    m_CurrentType = ToggleType.None;
                    //キャラ画像
                    IsViewImage = true;
                }
            }
        }


        //ボタン位置更新
        //updateTogglePos();
    }

    private void LateUpdate()
    {
        if (m_LastUpdateCount != 0)
        {
            m_UnitParamPanel.updateLayout();

            m_UnitEvolvePanel.updateLayout();

            m_LastUpdateCount--;
            if (m_LastUpdateCount <= 0)
            {
                WindowRoot.alpha = 1.0f;
                m_LastUpdateCount = 0;
            }
        }
    }

    private bool IsReady()
    {
        return m_LastUpdateCount == 0
            && !MainMenuManager.Instance.CheckMenuControlNG()
            && !MainMenuManager.Instance.IsPageSwitch()//
            && CharaImage.Ready;//キャラ画像の準備が完了するまで
    }

    /// <summary>
    /// キャラクタID設定
    /// </summary>
    /// <param name="_id"></param>
    public void SetCharaID(uint _id)
    {
        if (_id == 0)
        {
            return;
        }

        m_MainUnitMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)_id);
        if (m_MainUnitMaster == null)
        {
            return;
        }

        m_UnitId = _id;
        m_MainUnit = null;
        m_SubUnit = null;

        m_StatusType = StatusType.Catalog;

        //キャラクタ画像読み込み設定
        CharaImage.UseUncompressed = true;
        CharaImage.SetCharaID(m_UnitId, true);

        //名前パネル
        m_UnitNamePanel.setup(m_MainUnitMaster);

        //進化
        m_bEvolve = true;

        // 所持数
        setupCharaCount();
        //イラストレーター名
        setupIllustrator();

        //キャラ画像
        IsViewImage = true;
        CharaImage.setAlpha(false);

        //お気に入り
        m_bFavorite = false;
        m_UnitDetailPanel.IsViewFavorite = false;

        //
        //OnSelectToggle(ToggleType.Status);
        //ローディング表示開始
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.GUARD);
    }

    /// <summary>
    /// ユニット設定
    /// </summary>
    /// <param name="_mainUnit"></param>
    /// <param name="_subUnit"></param>
    public void SetUnit(PacketStructUnit _mainUnit, PacketStructUnit _subUnit)
    {
        if (_mainUnit == null)
        {
            return;
        }

        m_MainUnitMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)_mainUnit.id);
        if (m_MainUnitMaster == null)
        {
            return;
        }

        m_UnitId = _mainUnit.id;
        m_MainUnit = _mainUnit;
        m_SubUnit = _subUnit;

        m_StatusType = StatusType.Unit;

        //キャラクタ画像読み込み設定
        CharaImage.UseUncompressed = true;
        CharaImage.SetCharaID(m_UnitId, true);

        //名前パネル
        m_UnitNamePanel.setup(m_MainUnitMaster);

        //進化
        m_bEvolve = true;

        //お気に入り
        m_bFavorite = false;
        m_UnitDetailPanel.IsViewFavorite = false;

        // 所持数
        setupCharaCount();
        //イラストレーター名
        setupIllustrator();

        //キャラ画像
        IsViewImage = true;
        CharaImage.setAlpha(false);
        //
        //OnSelectToggle(ToggleType.Status);
        //ローディング表示開始
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.GUARD);
    }

    /// <summary>
    /// ユニット設定(お気に入り登録あり)
    /// </summary>
    /// <param name="_mainUnit"></param>
    /// <param name="_subUnit"></param>
    /// <param name="_unitContext"></param>
    public void SetUnitFavorite(PacketStructUnit _mainUnit, PacketStructUnit _subUnit, UnitGridContext _unitContext)
    {
        SetUnit(_mainUnit, _subUnit);

        m_bFavorite = true;
        m_UnitDetailPanel.IsViewFavorite = true;

        m_MainUnitContext = _unitContext;
        m_UnitDetailPanel.IsFavorite = MainMenuUtil.ChkFavoritedUnit(_mainUnit.unique_id);
    }

    private void setupCharaCount()
    {
        UnitGridParam[] unit_list = UserDataAdmin.Instance.m_UnitGridParamList;

        int count = 0;
        for (int i = 0; i < unit_list.Length; ++i)
        {
            if (unit_list[i].master == null)
            {
                continue;
            }

            if (unit_list[i].master.fix_id == m_UnitId)
            {
                ++count;
            }
        }

        // 所持数の表示
        if (count > 0 && count <= GlobalDefine.VALUE_VIEW_MAX_SAME_UNIT)
        {
            m_UnitDetailPanel.CharaCountText = string.Format(GameTextUtil.GetText("possession_01"), count);
        }
        else if (count > GlobalDefine.VALUE_VIEW_MAX_SAME_UNIT)
        {
            // カンスト時
            m_UnitDetailPanel.CharaCountText = string.Format(GameTextUtil.GetText("possession_02"), GlobalDefine.VALUE_VIEW_MAX_SAME_UNIT);
        }
        else
        {
            m_UnitDetailPanel.CharaCountText = "";
        }

#if BUILD_TYPE_DEBUG
        Debug.Log(string.Format("UnitDetailInfo ユニット所持数: {0} DrawID: {1}", count, m_MainUnitMaster.draw_id));
#endif
    }

    private void setupIllustrator()
    {
        IllustratorName = "";
        if (m_MainUnitMaster.illustrator_id != 0)
        {
            MasterDataIllustrator illustrator = MasterFinder<MasterDataIllustrator>.Instance.Find(m_MainUnitMaster.illustrator_id);
            if (illustrator != null)
            {
                IllustratorName = illustrator.name;
            }
        }
    }

    private void setupStatus()
    {
        if (m_StatusType == StatusType.Catalog)
        {
            //パラメータパネル
            m_UnitParamPanel.setupChara(m_UnitId, UnitParamPanel.StatusType.LV_1);

            //ストーリーパネル
            m_UnitStoryPanel.setup(m_UnitId);
        }
        else if (m_StatusType == StatusType.Unit)
        {
            //パラメータパネル
            m_UnitParamPanel.setupUnit(m_MainUnit, m_SubUnit);

            //ストーリーパネル
            m_UnitStoryPanel.setup(m_UnitId);
        }
        m_Setup[(int)ToggleType.Status] = true;
    }

    private void setupSkill()
    {
        if (m_StatusType == StatusType.Catalog)
        {
            //スキルパネル
            m_UnitSkillPanel.AddLeaderSkill(m_MainUnitMaster.skill_leader);
            m_UnitSkillPanel.AddLimitBreakSkill(m_MainUnitMaster.skill_limitbreak, 0);
            m_UnitSkillPanel.AddActiveSkill(m_MainUnitMaster.skill_active0);
            if (m_MainUnitMaster.skill_active1 != 0) m_UnitSkillPanel.AddActiveSkill(m_MainUnitMaster.skill_active1);
            if (m_MainUnitMaster.skill_passive != 0) m_UnitSkillPanel.AddPassiveSkill(m_MainUnitMaster.skill_passive);
        }
        else if (m_StatusType == StatusType.Unit)
        {
            //スキルパネル
            m_UnitSkillPanel.AddLeaderSkill(m_MainUnitMaster.skill_leader);
            m_UnitSkillPanel.AddLimitBreakSkill(m_MainUnitMaster.skill_limitbreak, (int)m_MainUnit.limitbreak_lv);
            m_UnitSkillPanel.AddActiveSkill(m_MainUnitMaster.skill_active0);
            if (m_MainUnitMaster.skill_active1 != 0) m_UnitSkillPanel.AddActiveSkill(m_MainUnitMaster.skill_active1);
            if (m_MainUnitMaster.skill_passive != 0) m_UnitSkillPanel.AddPassiveSkill(m_MainUnitMaster.skill_passive);
        }
        m_Setup[(int)ToggleType.Skill] = true;

    }

    private void setupEvol()
    {
        if (m_bEvolve)
        {
            m_EvolveList.Clear();
            m_UnitEvolvePanel.EvolveList.Clear();

            AndroidBackKeyManager.Instance.DisableBackKey();

            new SerialProcess().Add(
                (System.Action next) =>
                {
                    //最初の進化先
                    int nextUnitId = (int)m_UnitId;
                    MasterDataParamChara _masterAfter = m_MainUnitMaster;
                    bool nameView = false;

                    //進化先がなくなるまで追加処理
                    do
                    {
                        MasterDataParamCharaEvol _evolAfter = MasterDataUtil.GetCharaEvolParamFromCharaID((uint)nextUnitId);

                        EvolveInfo evolveInfo = new EvolveInfo();
                        evolveInfo.charaMaster = _masterAfter;
                        evolveInfo.evolveMaster = _evolAfter;
                        evolveInfo.charaTexture = null;
                        evolveInfo.nameView = nameView;
                        m_EvolveList.Add(evolveInfo);
                        nameView = true;

                        nextUnitId = -1;

                        if (_evolAfter != null)
                        {
                            _masterAfter = MasterFinder<MasterDataParamChara>.Instance.Find((int)_evolAfter.unit_id_after);
                            if (_masterAfter != null) nextUnitId = (int)_evolAfter.unit_id_after;
                        }

                        //同一キャラがすでにリストに登録されていたら検索をやめる
                        if (nextUnitId != -1 &&
                            checkEvolveCharaID((uint)nextUnitId) == false)
                        {
                            nextUnitId = -1;
                        }
                    } while (nextUnitId != -1);

                    //ローディング表示開始
                    LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.GUARD);

                    next();
                })
                .Add((System.Action next) =>
                {
                    AssetBundlerMultiplier multi = AssetBundlerMultiplier.Create();
                    for (int i = 0; i < m_EvolveList.Count; i++)
                    {
                        int no = i;
                        AssetBundler assetBundler = AssetBundler.Create()
                        .SetAsUnitTexture(m_EvolveList[i].charaMaster.fix_id,
                        (o) =>
                        {
                            m_EvolveList[no].charaTexture = o.GetTexture2D(TextureWrapMode.Clamp);
                        });
                        multi.Add(assetBundler);
                    }
                    multi.Load(
                    () =>//Success
                    {
                        next();
                    },
                    () =>//Error
                    {
                        next();
                    });
                })
                .Add((System.Action next) =>
                {
                    for (int i = 0; i < m_EvolveList.Count; i++)
                    {
                        UnitEvolveContext evolveContext = new UnitEvolveContext();
                        evolveContext.setup(m_EvolveList[i].charaMaster, m_EvolveList[i].evolveMaster, m_EvolveList[i].charaTexture, m_EvolveList[i].nameView);
                        evolveContext.DidSelectItem = OnSelectSkillButton;
                        m_UnitEvolvePanel.EvolveList.Add(evolveContext);
                    }

                    //ローディング表示終了
                    LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.GUARD);

                    AndroidBackKeyManager.Instance.EnableBackKey();

                    m_LastUpdateCount = UPDATE_LAYOUT_COUNT;

                    next();
                })
                .Flush();
        }

        m_Setup[(int)ToggleType.Evolve] = true;
    }

    /// <summary>
    /// 同一のキャラクタが進化リストに入っているか？
    /// </summary>
    /// <param name="chara_id"></param>
    /// <returns></returns>
    private bool checkEvolveCharaID(uint chara_id)
    {
        for (int i = 0; i < m_EvolveList.Count; i++)
        {
            if (m_EvolveList[i].charaMaster.fix_id == chara_id) return false;
        }
        return true;
    }

    public void SetCloseAction(System.Action closeAction)
    {
        m_CloseAction += closeAction;
    }

    public void SetReadyAction(System.Action readyAction)
    {
        m_ReadyAction += readyAction;
    }

    public void OnSelectReturn()
    {
        if (!IsReady())
            return;

        if (!backkey)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        // [DG0-4795] 【5.4.0】「チュートリアル」中の「ユニット詳細」画面＞「ユニット画像」画面にて「もどる」ボタンをタップした際、適切な画面に戻らない場合がある
        // DG0-4231でチュートリアルガチャ後の名前入力時に詳細画面表示が残らない対応が追加されたが,
        // マスター選択のユニット詳細にも影響しており、拡大表示の後に閉じる挙動になっていた.
        // 戻るか、閉じるかの判定条件としてチュートリアル中でチュートリアルステップ番号(スクラッチ後の名前入力画面の603かどうか)を見るようにした.
        if ((TutorialManager.IsExists == false
        || (TutorialManager.IsExists == true
        && UserDataAdmin.Instance.m_StructPlayer.renew_tutorial_step != 603))
        && (m_CurrentType == ToggleType.None
        || m_CurrentType == ToggleType.Evolve)
        )
        {
            OnSelectToggle(ToggleType.Status, false);
        }
        else
        {
            m_CloseAction();
            Hide();
        }

    }

    public void OnSelectToggle(ToggleType _type, bool bSE = true)
    {
        if (_type == ToggleType.None
            || !IsReady())
        {
            return;
        }

        if (bSE == true)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
        }
        ToggleType _oldType = m_CurrentType;
        m_CurrentType = _type;
        setActiveStatus(_oldType, false);
        setActiveStatus(m_CurrentType, true);
        IsViewImage = false;

    }

    public void OnSelectCharaScreen()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        setActiveStatus(m_CurrentType, false);
        m_CurrentType = ToggleType.None;
        //キャラ画像
        IsViewImage = true;
    }

    private void openStatus()
    {
        if (m_CurrentType == ToggleType.None)
        {
            m_CurrentType = ToggleType.Status;
        }

        setActiveStatus(m_CurrentType, true);
    }

    private void closeStatus()
    {
        setActiveStatus(m_CurrentType, false);
    }

    private void setActiveStatus(ToggleType _type, bool bActive)
    {
        IsViewEvolveNone = false;
        IsViewInfo = bActive;
        m_UnitDetailPanel.IsViewFavorite = false;

        int updateCount = UPDATE_LAYOUT_COUNT;

        switch (_type)
        {
            case ToggleType.None:
                break;
            case ToggleType.Status:
                UnityUtil.SetObjectEnabledOnce(m_UnitDetailPanel.gameObject, bActive);
                if (m_bFavorite) m_UnitDetailPanel.IsViewFavorite = bActive;

                MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un63f_description"));
                break;
            case ToggleType.Evolve:
                if (m_bEvolve)
                {
                    if (!m_Setup[(int)_type])
                    {
                        setupEvol();
                        updateCount = 0;
                    }
                    UnityUtil.SetObjectEnabledOnce(m_UnitEvolvePanel.gameObject, bActive);
                }
                else
                {
                    IsViewEvolveNone = bActive;
                }

                MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un69f_description"));
                break;
        }

        WindowRoot.alpha = 0.0f;
        m_LastUpdateCount = updateCount;

        if (IsViewInfo && bActive) setToggle(_type);
    }

    private void setToggle(ToggleType _type)
    {
        for (int i = 0; i < ToggleList.Count; i++)
        {
            if (ToggleList[i].m_Type == _type)
            {
                ToggleList[i].Flag = true;
            }
            else
            {
                ToggleList[i].Flag = false;
            }
        }
    }
    public void Hide()
    {
        AndroidBackKeyManager.Instance.StackPop(gameObject);

        DestroyObject(gameObject);
    }

    public void OnSelectFavorite()
    {
        if (m_MainUnit == null)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        m_UnitDetailPanel.IsFavorite = !m_UnitDetailPanel.IsFavorite;

        if (m_UnitDetailPanel.IsFavorite)
        {
            //--------------------------
            // お気に入り登録実行
            //--------------------------
            LocalSaveManager.Instance.SaveFuncAddFavoriteUnit(m_MainUnit.unique_id, true, false);

            //表示データ
            if (m_MainUnitContext != null)
            {
                m_MainUnitContext.IsActiveFavoriteImage = true;
            }

            //原本データ
            UnitGridParam gridParam = UserDataAdmin.Instance.SearchUnitGridParam(m_MainUnit.unique_id);
            if (gridParam != null) gridParam.favorite = true;
        }
        else
        {
            //--------------------------
            // お気に入り解除実行
            //--------------------------
            LocalSaveManager.Instance.SaveFuncAddFavoriteUnit(m_MainUnit.unique_id, false, true);

            //表示データ
            if (m_MainUnitContext != null)
            {
                m_MainUnitContext.IsActiveFavoriteImage = false;
            }

            //原本データ
            UnitGridParam gridParam = UserDataAdmin.Instance.SearchUnitGridParam(m_MainUnit.unique_id);
            if (gridParam != null) gridParam.favorite = false;
        }
    }

    public void DisableBackKey()
    {
        backkey = false;
    }

    public void EnableBackKey()
    {
        backkey = true;
    }

    public void OnSelectSkillButton(UnitEvolveContext context)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        SkillDialog.Show(context.charaMaster);
    }

    public void OnSelectMainUnitSkillButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        SkillDialog.Show(m_MainUnitMaster, m_MainUnit, m_SubUnit);
    }

    public void OnSelectLinkButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        LinkDialog.Show(m_UnitId, m_MainUnit, m_SubUnit);
    }

    //-------------------------------------------------------------------------------------------------
    //
    //
    //
    //-------------------------------------------------------------------------------------------------
    public static UnitDetailInfo Create(Camera camera, bool charaScreen = false)
    {
        //ユニット詳細は１つしか開かない
        if (GetUnitDetailInfo() != null) return null;

        GameObject _tmpObj = Resources.Load("Prefab/UnitDetailInfo/UnitDetailInfo") as GameObject;
        if (_tmpObj == null) return null;
        GameObject _insObj = Instantiate(_tmpObj) as GameObject;
        if (_insObj == null) return null;
        UnityUtil.SetObjectEnabledOnce(_insObj, true);

        UnitDetailInfo info = _insObj.GetComponent<UnitDetailInfo>();
        info.setCamera(camera);
        info.charaScreen = charaScreen;

        return info;
    }

    public static UnitDetailInfo GetUnitDetailInfo()
    {
        GameObject[] infoArray = GameObject.FindGameObjectsWithTag("UnitDetailInfo");
        if (infoArray.Length == 0) return null;
        return infoArray[0].GetComponent<UnitDetailInfo>();
    }

}
