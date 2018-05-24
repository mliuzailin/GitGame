/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	SceneModeContinuousBattle.cs
	@brief	シーン：ゲームメイン
	@author Developer
	@date 	2012/10/04
*/
/*==========================================================================*/
/*==========================================================================*/

/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

using ServerDataDefine;
using UnityEngine.UI;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	シーン：ゲームメイン
*/
//----------------------------------------------------------------------------
public class SceneModeContinuousBattle : SceneMode<SceneModeContinuousBattle>
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public GameObject m_BackGround = null;                                  //!< 背景オブジェクト
    public Animation m_BackGroundAnimation = null;                                  //!< 背景拡大アニメーション
    public GameObject m_EnemyIconList = null;                                 //!< 敵アイコンルートオブジェクト
    public GameObject m_BackGround_L = null;                                    //!< 背景扉用オブジェクト(左)
    public Animation m_BackGround_LAnimation = null;                                 //!< 背景扉アニメーション(左)
    public GameObject m_BackGround_R = null;                                    //!< 背景扉用オブジェクト(右)
    public Animation m_BackGround_RAnimation = null;                                 //!< 背景扉アニメーション(右)
    public GameObject m_BattleWave = null;                                 //!< BattleWaveオブジェクト
    public Animation m_ContinuousBattleWaveAnim = null;                                 //!< BattleWaveアニメーション
    public GameObject m_FadeScreen = null;                                 //!< フェード画面オブジェクト
    public Animation m_FadeScreen_Animation = null;                                 //!< フェード画面アニメーション
    public ContinuousBattleManager m_ContinuousBattleManager = null;                                 //!< 新バトル遷移UI制御クラス
    public Camera m_SubCamera = null;
    public RenderTexture m_RenderTexture = null;
    public GameObject m_StringCutInRoot = null;
    public GameObject m_SkillWindowRoot = null;
    public Animation m_ReadyAnimation = null;
    public Sprite m_NewBattleBg = null;

    [HideInInspector]
    public BattleReqManager m_ManagerBattleReq = null;                                    //!< マネージャ管理：戦闘リクエストマネージャ
    [HideInInspector]
    private InGameQuestData m_ManagerUserDataQuest = null;                                    //!< マネージャ管理：クエスト中取得物管理
    [HideInInspector]
    public InGameSkillWindowQuest2 m_InGameSkillWindowQuest2 = null;                                  //!< マネージャ管理：スキルウィンドウ
    [HideInInspector]
    public InGameShopManager m_InGameShopManager = null;                                  //!< マネージャ管理：課金関連管理

    [HideInInspector]
    public bool m_InGameStartDisplayed = false;                               //!< ゲーム開始演出表示済みフラグ
    [HideInInspector]
    public float m_GameSpeedAmend = GlobalDefine.GAME_SPEED_UP_OFF;       //!< ゲーム速度補正 v380add
    [HideInInspector]
    public bool m_InGameStepTriger = false;                               //!< インゲーム処理ステップ切り替わりフラグ
    [HideInInspector]
    public bool m_InGameStartOK = false;                              //!< インゲーム開始許可
    [HideInInspector]
    public CharaParty m_PlayerParty = null;                                   //!< プレイヤーパーティー関連：パーティー情報
    [HideInInspector]
    public CharaOnce[] m_PlayerPartyChara = null;                                 //!< プレイヤーパーティー関連：キャラ情報
    [HideInInspector]
    public bool m_InGameBattleInitOK = false;                             //!< ゲーム開始演出表示済みフラグ

    //----------------------------------------
    // クエスト情報
    //----------------------------------------
    [HideInInspector]
    public uint m_QuestAreaID = 0;                                    //!< クエスト情報：エリアID
    [HideInInspector]
    public uint m_QuestMissionID = 0;                                    //!< クエスト情報：クエストID
    [HideInInspector]
    public int m_QuestFloor = 0;                                  //!< クエスト情報：現在のフロア
    [HideInInspector]
    public uint m_NextAreaCleard = 0;                                    //!< クエスト情報：次のエリアID

    //----------------------------------------
    // プレイヤー取得物関連情報定義
    //----------------------------------------
    [HideInInspector]
    public bool m_AcquireKey = false;                             //!< プレイヤー取得物：鍵入力

    //----------------------------------------
    // エフェクト管理
    //----------------------------------------
    [HideInInspector]
    public GlobalDefine.PartyCharaIndex m_LBSCharaIdx = GlobalDefine.PartyCharaIndex.ERROR;   //!< リミットブレイクスキル発動キャラINDEX
    [HideInInspector]
    public SkillRequestParam m_SkillRequestLimitBreak = new SkillRequestParam(BattleParam.SKILL_LIMIT_BREAK_ADD_MAX);

    [HideInInspector]
    public int m_QuestTotalTurn = 0;                                    //!< クリアまでのターン数

    //----------------------------------------
    // ゲームリソース関連定義
    //----------------------------------------
    [HideInInspector]
    public GameObject m_QuestClear = null;                                    //!< ゲーム中オブジェクト：クエストクリア
    [HideInInspector]
    public Animation m_QuestClearAnim = null;                                 //!< ゲーム中オブジェクト：クエストクリアアニメーション
    [HideInInspector]
    public static readonly string m_GameObjEffLimitBreak = "m_GameObjEffLimitBreak";              //!< ゲーム中オブジェクト：リミットブレイクスキル
    [HideInInspector]
    public bool[] m_Balloon_active = null;
    [HideInInspector]
    public int m_BossChainCount = 0;
    [HideInInspector]
    public uint m_QuestRandSeed = 0;                                  //!< クエストの乱数シード

    private int m_BattleCount;                                                          //!< 現在のバトル回数

    private int m_EnemyCount;
    private ContinuousBattleWave m_ContinuousBattleWave = null;
    private bool m_IsSkillWindowUpdata = false;

    private GameObject m_StringCutIn = null;
    private Animation m_StringCutInAnimation = null;
    private GameObject m_SkillWindowObject = null;

    private bool m_BossStart = false;
    private bool m_IsOkPrefabPreload = false;   //!< プレハブの先行読み込み完了

    public bool isUsedAutoPlay
    {
        get
        {
            return BattleParam.isUsedAutoPlay();
        }
        set
        {
            BattleParam.initUsedAutoPlay(value);
        }
    }

    public int battleCount                                                          //!< 現在のバトル回数
    {
        get { return m_BattleCount; }
    }
    public bool isSkillWindowUpdate
    {
        get { return m_IsSkillWindowUpdata; }
    }

    private float m_WaiTime = 0;

    private int m_CurrentBGM = 0;

    private string m_BackGroundAssetBundleName;

    private List<string> m_BgmAssetNameList = new List<string>();   // BGMアセット名リスト（ロード時間短縮用の情報）

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※インスタンス生成時呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Awake()
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        DebugOptionInGame.Instance.calcDebugTime("ChangeScene->Awake");
#endif
        base.Awake();

        //----------------------------------------
        //	AssetBundle読み込みの管理用の配列生成
        //----------------------------------------

        m_ManagerBattleReq = gameObject.AddComponent<BattleReqManager>();   // マネージャ管理：バトルマネージャ
        m_ManagerUserDataQuest = gameObject.AddComponent<InGameQuestData>();    // マネージャ管理：クエスト中取得物管理
        m_InGameSkillWindowQuest2 = null;
        m_InGameShopManager = gameObject.AddComponent<InGameShopManager>(); // マネージャ管理：課金関連

        m_InGameStartOK = false;
        m_BossStart = false;
        m_Balloon_active = new bool[(int)GlobalDefine.PartyCharaIndex.MAX];
        for (int num = 0; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
        {
            m_Balloon_active[num] = true;
        }
        m_InGameBattleInitOK = false;

        m_ReadyAnimation.wrapMode = WrapMode.Loop;
        m_ReadyAnimation.Play("Ready");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        m_IsSkillWindowUpdata = false;
        InGameUtil.setQuest2(true);

        // m_SceneGoesParamToQuest2が設定されてないとき(単体起動時など)のダミー設定
        if ((SceneGoesParam.Instance != null
        && SceneGoesParam.Instance.m_SceneGoesParamToQuest2 == null)
        )
        {
            MasterDataQuest2 masterDataQuest = null;
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
            if (DebugOptionInGame.Instance != null
            && DebugOptionInGame.Instance.inGameDebugDO.m_InGameDebug == true
            )
            {
                // 探索単体起動か全クエストオートプレイ時
                masterDataQuest = DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataQuest2;
            }
            else
#endif
            {
                masterDataQuest = MasterDataUtil.GetQuest2ParamFromID(UserDataAdmin.Instance.m_StructQuest.quest_id);
            }
            //----------------------------------------
            // 中断復帰情報を反映。
            // 本来はタイトルシーンとかで反映される。あくまでセーフティ処理
            //----------------------------------------
            if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore == null)
            {
                if (LocalSaveManager.Instance != null)
                {
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = LocalSaveManager.Instance.LoadFuncGoesToQuest2Restore();
                }
            }
            if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2 == null)
            {
                if (LocalSaveManager.Instance != null)
                {
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = LocalSaveManager.Instance.LoadFuncGoesToQuest2Start();
                }
            }
            //----------------------------------------
            // ダミーパラメータを設定
            //----------------------------------------
            if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2 == null)
            {
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = new SceneGoesParamToQuest2();
                if (masterDataQuest == null)
                {
                    Debug.LogError("masterDataQuest is null");
                    return;
                }

                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestAreaID = masterDataQuest.area_id;
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID = masterDataQuest.fix_id;
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestRandSeed = RandManager.GetRand();
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_IsUsedAutoPlay = false;

                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara0Param = new UserDataUnitParam();
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara1Param = new UserDataUnitParam();
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara2Param = new UserDataUnitParam();
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara3Param = new UserDataUnitParam();
                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara4Param = new UserDataUnitParam();

                if (
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
                    DebugOptionInGame.Instance.inGameDebugDO.m_DebugParty == false &&
#endif
                    UserDataAdmin.Instance != null &&
                    UserDataAdmin.Instance.m_StructHelperList.IsNullOrEmpty() != true &&
                    UserDataAdmin.Instance.m_StructPartyAssign.IsNullOrEmpty() != true)
                {
                    PacketStructFriend cHelper = UserDataAdmin.Instance.m_StructHelperList[3];
                    int nPartyCurrent = UserDataAdmin.Instance.m_StructPlayer.unit_party_current;
                    PacketStructUnit[] acUnitStruct = {
                                    UserDataAdmin.Instance.m_StructPartyAssign[ nPartyCurrent ][0]
                                ,   UserDataAdmin.Instance.m_StructPartyAssign[ nPartyCurrent ][1]
                                ,   UserDataAdmin.Instance.m_StructPartyAssign[ nPartyCurrent ][2]
                                ,   UserDataAdmin.Instance.m_StructPartyAssign[ nPartyCurrent ][3]
                                ,   cHelper.unit
                                };
                    UserDataUnitParam[] acUnitParam = {
                        SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara0Param,
                        SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara1Param,
                        SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara2Param,
                        SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara3Param,
                        SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara4Param,
                    };
                    for (int i = 0; i < acUnitStruct.Length; i++)
                    {
                        acUnitParam[i].m_UnitDataID = acUnitStruct[i].id;
                        acUnitParam[i].m_UnitParamLevel = (int)acUnitStruct[i].level;
                        acUnitParam[i].m_UnitParamEXP = (int)acUnitStruct[i].exp;
                        acUnitParam[i].m_UnitParamUniqueID = acUnitStruct[i].unique_id;
                        acUnitParam[i].m_UnitParamLimitBreakLV = (int)acUnitStruct[i].limitbreak_lv;
                        acUnitParam[i].m_UnitParamLimitOverLV = (int)acUnitStruct[i].limitover_lv;
                        acUnitParam[i].m_UnitParamPlusPow = (int)acUnitStruct[i].add_pow;
                        acUnitParam[i].m_UnitParamPlusHP = (int)acUnitStruct[i].add_hp;
                    }
                }
                else
                {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara0Param.m_UnitDataID = DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara0_id;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara1Param.m_UnitDataID = DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara1_id;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara2Param.m_UnitDataID = DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara2_id;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara3Param.m_UnitDataID = DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara3_id;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara4Param.m_UnitDataID = DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara4_id;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara0Param.m_UnitParamLevel = (int)DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara0_level;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara1Param.m_UnitParamLevel = (int)DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara1_level;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara2Param.m_UnitParamLevel = (int)DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara2_level;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara3Param.m_UnitParamLevel = (int)DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara3_level;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara4Param.m_UnitParamLevel = (int)DebugOptionInGame.Instance.inGameDebugDO.m_MasterDataDefaultParty.party_chara4_level;
#endif
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara0Param.m_UnitParamEXP = 100;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara1Param.m_UnitParamEXP = 100;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara2Param.m_UnitParamEXP = 100;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara3Param.m_UnitParamEXP = 100;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara4Param.m_UnitParamEXP = 100;

                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara0Param.m_UnitParamUniqueID = 1;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara1Param.m_UnitParamUniqueID = 2;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara2Param.m_UnitParamUniqueID = 3;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara3Param.m_UnitParamUniqueID = 4;
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara4Param.m_UnitParamUniqueID = 5;
                }
            }
        }

        //--------------------------------
        // 中断復帰データがあれば復旧
        //--------------------------------
        if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore != null)
        {
            //------------------------------
            // クエスト情報の復帰
            //------------------------------
            // クエスト情報
            m_QuestAreaID = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_QuestAreaID;
            m_QuestMissionID = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_QuestMissionID;
            m_NextAreaCleard = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_NextAreaCleard;
            // クリアターン数
            m_QuestTotalTurn = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_QuestTotalTurn;
            m_QuestRandSeed = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_QuestRandSeed;
            isUsedAutoPlay = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_IsUsedAutoPlay;
            m_BattleCount = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_BattleCount;
            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {
                m_Balloon_active[i] = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_Balloon_active[i];
            }
            m_BossChainCount = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_BossChainCount;
        }
        else if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2 != null)
        {

            //------------------------------
            // 通常起動
            //------------------------------
            // クエスト情報
            m_QuestAreaID = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestAreaID;
            m_QuestMissionID = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID;
            m_NextAreaCleard = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_NextAreaCleard;
            // クリアターン数
            m_QuestTotalTurn = 0;
            m_QuestRandSeed = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestRandSeed;
            isUsedAutoPlay = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_IsUsedAutoPlay;
            m_BattleCount = 0;
#if BUILD_TYPE_DEBUG
            m_BattleCount = MainMenuParam.m_DebugBattleNumber;
            MainMenuParam.m_DebugBattleNumber = 0;
#endif //BUILD_TYPE_DEBUG
            m_BossChainCount = 0;
        }

        AssetBundleLoad().Load(() =>
        {
            m_InGameStartOK = true;
        },
        () =>
        {
            Debug.LogError("AssetBundleLoad NG:");
            m_InGameStartOK = true;
        });

        // プレハブを先行読み込み
        m_IsOkPrefabPreload = false;
        StartCoroutine(preloadPrefabs());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：インスタンス制御関連：インスタンス破棄時に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void OnDestroy()
    {
        BattleParam.unloadBattlePrefab();
        if (ReplaceAssetManager.HasInstance)
        {
            ReplaceAssetManager.Instance.endReplaceMode();
        }

        m_ManagerBattleReq = null;
        m_ManagerUserDataQuest = null;
        m_InGameSkillWindowQuest2 = null;
        m_InGameShopManager = null;

        //----------------------------------------
        // 基底呼び出し。
        //----------------------------------------
        base.OnDestroy();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        SkillWindowClose();
    }

    public override void OnInitialized()
    {
        base.OnInitialized();
        AndroidBackKeyManager.Instance.EnableBackKey();
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnSelectBackKey);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	事前に読み込めるプレハブを読み込み
		@note	アセットバンドルの読み込み待ちの間に読み込む
	*/
    //----------------------------------------------------------------------------
    IEnumerator preloadPrefabs()
    {
        //WAVE
        GameObject _tmpObj = Resources.Load("Prefab/InGame/InGameLogo/BattleWave") as GameObject;
        yield return null;
        m_BattleWave = Instantiate(_tmpObj) as GameObject;
        yield return null;
        m_BattleWave.transform.SetParent(m_StringCutInRoot.transform, false);
        m_ContinuousBattleWaveAnim = m_BattleWave.transform.GetChild(0).gameObject.GetComponent<Animation>();
        m_ContinuousBattleWave = m_BattleWave.GetComponent<ContinuousBattleWave>();
        yield return null;

        //バトル
        BattleParam.loadBattlePrefab();
        yield return null;

        //リソース差し替え開始（ここで一度startReplaceModeByQuestID()を呼んでおかないとバトル開始時の待ち時間が長くなる）
        ReplaceAssetManager.Instance.startReplaceMode(ReplaceAssetManager.getAreaCategoryIDFromQuestID(m_QuestMissionID), ReplaceAssetReference.ChangeTimingType.BATTLE_NORMAL);
        while (ReplaceAssetManager.Instance.getStatus() == ReplaceAssetManager.Status.LOADING)
        {
            yield return null;
        }

        m_IsOkPrefabPreload = true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：初期化：諸々のしがらみ待ち
		@note	シーン初期化や他マネージャの正常起動を待ってからゲームを開始する
	*/
    //----------------------------------------------------------------------------
    public void OnReady()
    {
        //------------------------------
        // 諸々のしがらみが解消されたら初期化開始
        //------------------------------
        if (m_InGameStartOK == false
            || m_IsOkPrefabPreload == false
        )
        {
            return;
        }
        if (m_ReadyAnimation["Ready"].time <= 0)
        {
            return;
        }
        // クリアターン数
        m_QuestTotalTurn = 0;

        //------------------------------
        // 敵リスト情報構築
        //------------------------------
        m_EnemyCount = m_ContinuousBattleManager.setupEnemyMap(m_QuestMissionID, m_BattleCount, -1);

        m_ContinuousBattleWave.setupWave(m_BattleCount + 1, m_EnemyCount - 1);

        //----------------------------------------
        // 入手情報を破棄
        //----------------------------------------
        if (InGameQuestData.Instance != null)
        {
            InGameQuestData.Instance.ClrAcquireUnitParam();
            InGameQuestData.Instance.ClrAcquireMoneyParam();
            if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2 != null)
            {
                InGameQuestData.Instance.Quest2Setup(m_QuestMissionID);
            }
        }

        stopBattleBGM(true);

        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_READY_CREATE_PARTY");
    }


    public Image backgroundImageEffect;
    public Image backgroundImage;


    public void OnAssetBundoleRequest()
    {
        //		SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID
        //			SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestAreaID = masterDataQuest.area_id;
        //			SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID = masterDataQuest.fix_id;

        //		if ( UserDataAdmin.Instance != null )
        //		{
        //			int unique_id = UserDataAdmin.Instance.m_StructPlayer.current_hero_id;
        //			for(int i = 0;i < UserDataAdmin.Instance.m_StructHeroList.Length;++i)
        //			{
        //				if(UserDataAdmin.Instance.m_StructHeroList[i].unique_id == unique_id)
        //				{
        //					Hero_face = InGamePlayerParty.Instance.m_HeroSprite;
        //					return;
        //				}
        //			}
        //		}

        SceneModeContinuousBattleFSM.Instance.SendFsmNextEvent();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：初期化：パーティー生成
	*/
    //----------------------------------------------------------------------------
    void OnReadyCreateParty()
    {
        //------------------------------
        // インスタンス再構築
        //------------------------------
        m_PlayerParty = new CharaParty();
        m_PlayerPartyChara = new CharaOnce[(int)GlobalDefine.PartyCharaIndex.MAX];
        for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
        {
            m_PlayerPartyChara[i] = new CharaOnce();
        }

        bool is_kobetsu_hp = true;  // 個別ＨＰ
        bool bRet = true;


        //------------------------------
        // 名称を元にキャラを再構築
        //------------------------------
        SceneGoesParamToQuest2Restore restore = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore;
        if (restore != null)
        {
            if (restore.m_PlayerPartyRestore.m_IsKobetsuHP != is_kobetsu_hp)
            {
                bRet = false;
            }

            //------------------------------
            // 各キャラの情報復帰
            //------------------------------
            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {

                if (restore.m_PlayerPartyRestore.m_PartyCharaID[i] == 0)
                {
                    continue;
                }

                //------------------------------
                //	キャラ情報設定
                //------------------------------
                m_PlayerPartyChara[i].CharaSetupFromID(restore.m_PlayerPartyRestore.m_PartyCharaID[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLevel[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLBSLv[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLimitOver[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaPlusPow[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaPlusHP[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLinkID[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLinkLv[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLinkPlusPow[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLinkPlusHP[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLinkPoint[i],
                                                          restore.m_PlayerPartyRestore.m_PartyCharaLinkLimitOver[i]);

                //------------------------------
                // リミットブレイクコスト復帰
                //------------------------------
                m_PlayerPartyChara[i].m_CharaLimitBreak = restore.m_PlayerPartyRestore.m_PartyCharaLimitBreak[i];
            }
        }
        else
        {

            UserDataUnitParam[] acUnitParam =
            {   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara0Param
            ,   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara1Param
            ,   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara2Param
            ,   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara3Param
            ,   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara4Param
            };


            //------------------------------
            // クエスト開始時に受けとった情報からパーティを構築
            //------------------------------
            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {

                if (acUnitParam[i] == null)
                {
                    continue;
                }

                if (acUnitParam[i].m_UnitDataID == 0)
                {
                    continue;
                }

                int nLimitOverLevel = 0;

                MasterDataParamChara cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(acUnitParam[i].m_UnitDataID);

                // 限界突破の値を反映
                if (cCharaMaster != null)
                {
                    nLimitOverLevel = acUnitParam[i].m_UnitParamLimitOverLV;
                }
                cCharaMaster = BattleParam.m_MasterDataCache.useCharaParam(acUnitParam[i].m_UnitParamLinkID);

                int nLimitOverLinkLevel = 0;

                if (cCharaMaster != null)
                {
                    // 限界突破の値を反映
                    nLimitOverLinkLevel = acUnitParam[i].m_UnitParamLinkLimitOverLV;
                }

                //------------------------------
                //	キャラ情報設定
                //------------------------------
                bRet &= m_PlayerPartyChara[i].CharaSetupFromID(acUnitParam[i].m_UnitDataID,
                                                                  acUnitParam[i].m_UnitParamLevel,
                                                                  acUnitParam[i].m_UnitParamLimitBreakLV,
                                                                  nLimitOverLevel,
                                                                  acUnitParam[i].m_UnitParamPlusPow,
                                                                  acUnitParam[i].m_UnitParamPlusHP,
                                                                  acUnitParam[i].m_UnitParamLinkID,
                                                                  acUnitParam[i].m_UnitParamLinkLv,
                                                                  acUnitParam[i].m_UnitParamLinkPlusPow,
                                                                  acUnitParam[i].m_UnitParamLinkPlusHP,
                                                                  acUnitParam[i].m_UnitParamLinkPoint,
                                                                  nLimitOverLinkLevel);
            }

        }

        if (bRet == false)
        {
            return;
        }

        //------------------------------
        // キャラを元にパーティーを再構築
        //------------------------------
        bRet &= m_PlayerParty.PartySetup(m_PlayerPartyChara, is_kobetsu_hp);
        if (bRet == false)
        {
            return;
        }
        if (restore != null)
        {
            //------------------------------
            // パーティ状態異常復帰（CharaParty.PartySetup() より後に復帰）
            //------------------------------
            m_PlayerParty.m_Ailments = restore.m_PlayerPartyRestore.m_PartyAilments;
            m_PlayerParty.m_Ailments.restoreFromSaveData();
        }
        if (SceneGoesParam.Instance != null
        && SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore != null)
        {
            m_PlayerParty.m_HPCurrent.setValue(GlobalDefine.PartyCharaIndex.MAX, SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_QuestHP);
            m_PlayerParty.m_PartyTotalSP = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_QuestSP;
            m_PlayerParty.m_HPMax.setValue(GlobalDefine.PartyCharaIndex.MAX, SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_QuestHPMax);
            m_PlayerParty.m_PartyTotalSPMax = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_QuestSPMax;
            m_PlayerParty.m_Hate = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_Hate;
            m_PlayerParty.m_Hate_ProvokeTurn = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_Hate_ProvokeTurn;
            m_PlayerParty.m_Hate_ProvokeOrder = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_Hate_ProvokeOrder;

            m_PlayerParty.m_BattleHero.restoreSkillTurn(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_HeroSkillTurn);
            m_PlayerParty.m_BattleAchive = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_PlayerPartyRestore.m_BattleAchive;

            m_PlayerParty.updateDispHp(0.0f);
        }

        InGamePlayerParty.Instance.setup(m_PlayerParty, m_PlayerPartyChara, true, m_Balloon_active);
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_READY_CREATE_BATTLE");
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：初期化：バトル関連生成
	*/
    //----------------------------------------------------------------------------
    void OnReadyCreateBattle()
    {
        //------------------------------
        // バトル関連の常駐インスタンス周りを準備
        //------------------------------
        BattleParam.QuestInitialize(this);
        m_InGameBattleInitOK = true;

        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_INIT");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		インゲーム処理ステップ：初期化：
		@returrn	EINGAME_STEP		[次回実行ステップ]
	*/
    //----------------------------------------------------------------------------
    void OnInit()
    {
        //フェードイン
        SceneCommon.Instance.StartFadeIn();

        m_InGameStartDisplayed = false;
        m_InGameStepTriger = true;

        //--------------------------------
        // 中断復帰データがあれば復旧
        //--------------------------------
        if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore != null)
        {

            //--------------------------------
            // 取得金の状態復帰
            //--------------------------------
            for (int i = 0; i < SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_AcquireMoney.Length; i++)
            {
                InGameQuestData.Instance.m_AcquireMoney[i] = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_AcquireMoney[i];
            }


            //--------------------------------
            // 取得ユニットの状態復帰
            //--------------------------------
            for (int i = 0; i < SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_AcquireUnit.Length; i++)
            {
                InGameQuestData.Instance.m_AcquireUnit[i] = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_AcquireUnit[i];
            }


            //--------------------------------
            //	取得チケットの状態復帰
            //--------------------------------
            for (int i = 0; i < SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_AcquireTicket.Length; i++)
            {
                InGameQuestData.Instance.m_AcquireTicket[i] = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_AcquireTicket[i];
            }

            //--------------------------------
            // 取得ユニット数の状態復帰
            //--------------------------------
            for (int i = 0; i < (int)MasterDataDefineLabel.RarityType.MAX; i++)
            {
                InGameQuestData.Instance.m_AcquireUnitRareNum[i] = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_AcquireUnitRareNum[i];
            }
        }

        if (TutorialManager.HasInstance)
        {
            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
            cOption.m_OptionBattleSkillTurn = (int)LocalSaveDefine.OptionBattleSkillTurn.OFF;
            cOption.m_OptionConfirmAS = (int)LocalSaveDefine.OptionConfirmAS.ON;
            cOption.m_OptionBattleSkillCost = (int)LocalSaveDefine.OptionBattleSkillCost.OFF;
            cOption.m_OptionBattleAchieve = (int)LocalSaveDefine.OptionBattleAchieve.OFF;

            if (InGamePlayerParty.HasInstance)
            {
                InGamePlayerParty.Instance.ASTurnInfo(false);
                InGamePlayerParty.Instance.NormalSkillInfo(false);
            }
        }

        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_INIT_GAMESTART");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：初期化：ゲームスタート演出
	*/
    //----------------------------------------------------------------------------
    void OnInitGameStart()
    {
        if (m_InGameStepTriger == true)
        {
            GameObject _tmpObj = Resources.Load("Prefab/InGame/InGameLogo/QuestStart") as GameObject;
            m_StringCutIn = Instantiate(_tmpObj) as GameObject;
            m_StringCutIn.transform.SetParent(m_StringCutInRoot.transform, false);
            m_StringCutIn.transform.localPosition = new Vector3(0, 200);
            UnityUtil.SetObjectEnabledOnce(m_StringCutIn, true);
            m_StringCutInAnimation = m_StringCutIn.transform.GetChild(0).gameObject.GetComponent<Animation>();
            m_InGameStepTriger = false;
            m_WaiTime = 1;
            return;
        }
        m_WaiTime -= Time.deltaTime;
        if (m_WaiTime > 0)
        {
            return;
        }

        //--------------------------------
        // クエスト開始演出開始
        //--------------------------------
        if (m_InGameStartDisplayed == false)
        {
            m_StringCutInAnimation["QuestStart"].speed = m_GameSpeedAmend;
            m_StringCutInAnimation.Play("QuestStart");
        }

        bool is_boss_battle = m_ContinuousBattleManager.enemyList[m_BattleCount].m_Boss;
        if (is_boss_battle == false)
        {
            playBattleBGM();
        }

        //--------------------------------
        // 次のフローへ進む
        //--------------------------------
        m_InGameStepTriger = true;
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_INIT_GAMESTART_WAIT");
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：初期化：ゲームスタート演出終了待ち
	*/
    //----------------------------------------------------------------------------
    void OnInitGameStartWait()
    {
        //--------------------------------
        // 演出終了待ち
        //--------------------------------
        if (m_InGameStartDisplayed == false)
        {
            if (m_StringCutInAnimation.isPlaying == true)
            {
                return;
            }
        }

        //--------------------------------
        // 次のフローへ進む
        //--------------------------------
        m_InGameStepTriger = true;
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_LEADERSKILL");
    }


    //----------------------------------------------------------------------------
    /*!
		@brief		インゲーム処理ステップ：ゲーム：リーダースキルによるパワーアップ演出
		@return		EINGAME_STEP		[次に実行する戦闘処理ステップ]
	*/
    //----------------------------------------------------------------------------
    void OnGameLeaderSkill()
    {
        //--------------------------------
        // 初回処理
        //--------------------------------
        if (m_InGameStepTriger == true)
        {
            m_InGameStepTriger = false;

            // 実行可能なリーダースキルが存在するかチェック
            bool leader = InGameUtil.CheckPreLeaderSkill(GlobalDefine.PartyCharaIndex.LEADER, m_PlayerPartyChara);
            bool friend = InGameUtil.CheckPreLeaderSkill(GlobalDefine.PartyCharaIndex.FRIEND, m_PlayerPartyChara);
            if (leader == false
            && friend == false)
            {
                m_InGameStepTriger = true;
                SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_PASSIVESKILL");
                return;
            }
            BattleSkillCutinManager.Instance.ClrSkillCutin();
            if (leader == true)
            {
                BattleSkillCutinManager.Instance.SkillCutinLeader(GlobalDefine.PartyCharaIndex.LEADER, m_PlayerParty);    // リーダー
            }

            if (friend == true)
            {
                BattleSkillCutinManager.Instance.SkillCutinLeader(GlobalDefine.PartyCharaIndex.FRIEND, m_PlayerParty);    // フレンド
            }
            BattleSkillCutinManager.Instance.CutinStart2(false, true);

            return;
        }

        //--------------------------------
        // 各アニメーションが終了するのを待つ
        //--------------------------------
        if (BattleSkillCutinManager.Instance.isRunning() == true)
        {
            return;
        }

        m_InGameStepTriger = true;
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_PASSIVESKILL");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		インゲーム処理ステップ：ゲーム：パッシブスキルによるパワーアップ演出
		@return		EINGAME_STEP		[次に実行する戦闘処理ステップ]
	*/
    //----------------------------------------------------------------------------
    void OnGamePassiveSkill()
    {

        //--------------------------------
        // 初回処理
        //--------------------------------
        if (m_InGameStepTriger == true)
        {
            m_InGameStepTriger = false;
            // 実行可能なパッシブスキルが存在するかチェック
            uint[] aPassiveID = new uint[(int)GlobalDefine.PartyCharaIndex.MAX];
            int skillCnt = 0;
            bool passive = InGameUtil.CheckPrePassiveSkill(ref aPassiveID);

            if (passive == false)
            {
                m_InGameStepTriger = true;
                SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_LINKPASSIVE");
                return;
            }

            // カットイン表示
            BattleSkillCutinManager.Instance.ClrSkillCutin();

            // パーティ1人、1パッシブとして処理
            for (int partyChara = 0; partyChara < (int)GlobalDefine.PartyCharaIndex.MAX; ++partyChara)
            {
                // 発動するスキルがある場合
                if (aPassiveID[partyChara] != 0)
                {
                    // カットイン情報を、前詰めで格納
                    BattleSkillCutinManager.Instance.SkillCutinPassive((GlobalDefine.PartyCharaIndex)partyChara, aPassiveID[partyChara]);
                    ++skillCnt;
                }
            }

            BattleSkillCutinManager.Instance.CutinStart2(false, true);


            return;
        }
        //--------------------------------
        // 各アニメーションが終了するのを待つ
        //--------------------------------
        if (BattleSkillCutinManager.Instance.isRunning() == true)
        {
            return;
        }

        m_InGameStepTriger = true;
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_LINKPASSIVE");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		インゲーム処理ステップ：ゲーム：リンクパッシブによるパワーアップ演出
		@return		EINGAME_STEP		[次に実行する戦闘処理ステップ]
	*/
    //----------------------------------------------------------------------------
    void OnGameLinkPassive()
    {

        //--------------------------------
        // 初回処理
        //--------------------------------
        if (m_InGameStepTriger == true)
        {
            m_InGameStepTriger = false;
            // 実行可能なリンクパッシブが存在するかチェック
            uint[] aPassiveID = new uint[(int)GlobalDefine.PartyCharaIndex.MAX];
            int skillCnt = 0;
            bool passive = InGameUtil.CheckPrePassiveSkill(ref aPassiveID, ESKILLTYPE.eLINKPASSIVE);

            if (passive == false)
            {
                m_InGameStepTriger = true;
                SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_PRE_PRODUCTION_WAIT");
                return;
            }

            // カットイン表示
            BattleSkillCutinManager.Instance.ClrSkillCutin();

            // パーティ1人、1パッシブとして処理
            for (int partyChara = 0; partyChara < (int)GlobalDefine.PartyCharaIndex.MAX; ++partyChara)
            {
                // 発動するスキルがある場合
                if (aPassiveID[partyChara] != 0)
                {
                    // カットイン情報を、前詰めで格納
                    BattleSkillCutinManager.Instance.SkillCutinRequest((GlobalDefine.PartyCharaIndex)Enum.ToObject(typeof(GlobalDefine.PartyCharaIndex), partyChara), aPassiveID[partyChara], ESKILLTYPE.eLINKPASSIVE);
                    ++skillCnt;
                }
            }

            BattleSkillCutinManager.Instance.CutinStart2(false, true);


            return;
        }

        //--------------------------------
        // 各アニメーションが終了するのを待つ
        //--------------------------------
        if (BattleSkillCutinManager.Instance.isRunning() == true)
        {
            return;
        }

        m_InGameStepTriger = true;
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_PRE_PRODUCTION_WAIT");
    }

    public void OnBattlePreProductionWait()
    {
        if (m_InGameStepTriger == true)
        {
            m_InGameStepTriger = false;
            m_FadeScreen_Animation.Play("FadeScreen");

            // リソース差し替え（演出と同時進行で差し替えてみる）
            {
                bool is_boss_battle = m_ContinuousBattleManager.enemyList[m_BattleCount].m_Boss;
                ReplaceAssetReference.ChangeTimingType timing = is_boss_battle ? ReplaceAssetReference.ChangeTimingType.BATTLE_BOSS : ReplaceAssetReference.ChangeTimingType.BATTLE_NORMAL;
                ReplaceAssetManager.Instance.startReplaceMode(ReplaceAssetManager.getAreaCategoryIDFromQuestID(m_QuestMissionID), timing);
            }
        }
        if (ReplaceAssetManager.Instance.getStatus() == ReplaceAssetManager.Status.LOADING)
        {
            return;
        }
        if (m_FadeScreen_Animation.isPlaying == true)
        {
            return;
        }
        m_ContinuousBattleManager.changeBackgroundColor(true);
        m_ContinuousBattleManager.setEnemyMapStart(m_BattleCount);
        UnityUtil.SetObjectEnabled(m_FadeScreen, false);
        int turn_offset = 0;
        int battle_unique = m_ContinuousBattleManager.enemyList[m_BattleCount].m_BattleUnique;
        PacketStructQuest2BuildBattle battle_param = null;
        battle_param = InGameUtil.GetQuest2BuildBattle(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild, battle_unique);
        m_InGameStepTriger = true;
        m_IsSkillWindowUpdata = true;
        if (m_ContinuousBattleManager.enemyList[m_BattleCount].m_Boss == true)
        {
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_CLEAR_BOSS_BATTLE");
        }
        else
        {
            RestoreBattle restore_battle = BattleParam.getRestoreData();
            if (restore_battle == null
                || restore_battle.m_BattleUniqueID != battle_unique
            )
            {
                const int MAGIC_NUMBER = 123456;    // 適当な固定数値
                uint rand_value = BattleParam.GetRandFix(battle_unique + MAGIC_NUMBER, 0, 100);
                MasterDataQuest2 master = MasterDataUtil.GetQuest2ParamFromID(m_QuestMissionID);
                if (master == null)
                {
                    Debug.LogError("master is null");
                    return;
                }

                if (master.first_attack > 100 ||
                    master.back_attack > 100 ||
                    (master.first_attack + master.back_attack) > 100)
                {
                    // 確立パラメーターエラー
                    Debug.LogError("first or back attack rate error:(first attack rate=" + master.first_attack + ") (back attack rate=" + master.back_attack + ")");
                    turn_offset = 0;
                }
                else
                {
                    if (rand_value <= master.first_attack)
                    {
                        // 先制攻撃
                        turn_offset = InGameDefine.TURN_OFFSET_00;
                    }
                    else
                    {
                        int turn_offset_rate = InGameUtil.PassiveChkpassBackAttack((int)master.back_attack);
                        if ((rand_value - master.first_attack) <= turn_offset_rate)
                        {
                            // 不意打ち攻撃
                            turn_offset = InGameDefine.TURN_OFFSET_01;
                        }
                    }
                }

#if BUILD_TYPE_DEBUG
                if (BattleParam.m_DebugBattleStartType == BattleParam.DebugBattleStartType.FORCE_FIRST_ATTACK)
                {
                    // デバッグ機能：強制先制攻撃（ファーストアタック）
                    turn_offset = InGameDefine.TURN_OFFSET_00;
                }

                if (BattleParam.m_DebugBattleStartType == BattleParam.DebugBattleStartType.FORCE_BACK_ATTACK)
                {
                    // デバッグ機能：強制不意打ち攻撃（バックアタック）
                    turn_offset = InGameDefine.TURN_OFFSET_01;
                }
#endif //BUILD_TYPE_DEBUG
            }

            playBattleBGM();

            BattleReqManager.Instance.BattleRequestAdd(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild, battle_param, false, false, turn_offset);
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_START");
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：バトル：バトル開始
	*/
    //----------------------------------------------------------------------------
    void OnBattleStartFunc()
    {
        if (m_InGameStepTriger == true)
        {
            m_InGameStepTriger = false;
            m_ContinuousBattleWave.setWaveNum(m_BattleCount + 1);
            UnityUtil.SetObjectEnabled(m_BattleWave, true);
            return;
        }
        if (m_ContinuousBattleWaveAnim.isPlaying == true)
        {
            return;
        }
        if (m_ContinuousBattleManager.cbm_state != ContinuousBattleManager.State.NONE)
        {
            return;
        }

        if (BattleReqManager.Instance == null)
        {
            Debug.LogError("BattleManager Is  Null!!");
            return;
        }

        //--------------------------------
        // リクエスト情報取得
        //--------------------------------
        BattleReq cBattleReq = BattleReqManager.Instance.GetBattleRequest();
        if (cBattleReq == null)
        {
            Debug.LogError("BattleRequest Is Null!!");
            return;
        }

        //--------------------------------
        // リクエスト情報をバトル管理クラスに発行
        //--------------------------------
        bool is_init_battle = BattleParam.BattleInitialize(cBattleReq, this);
        //is_init_battle = false;
        if (is_init_battle == false)
        {
            Debug.LogError("BattleRequest Is Error!!");
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_ERROR_GO_TITLE");
            return;
        }

        //--------------------------------
        // 戦闘開始指示出し
        //--------------------------------
        bool is_start_battle = BattleParam.BattleStart();
        if (is_start_battle == false)
        {
            Debug.LogError("BattleRequest Is Error!!");
            return;
        }

        //--------------------------------
        // 消去時のエラーチェックのためにリクエストの発行済みフラグを立てておく
        //--------------------------------
        cBattleReq.m_BattleRequestAttached = true;


        //--------------------------------
        // 戦闘リクエストが受理された
        // →戦闘待ちステップへ遷移
        //--------------------------------
        UnityUtil.SetObjectEnabled(m_BattleWave, false);
        m_InGameStepTriger = true;
        m_ContinuousBattleManager.changeBackgroundColor(false);
        InGamePlayerParty.Instance.m_HandsView = true;
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE");
    }

    /// <summary>
    /// バトル初期化失敗。エラーダイアログを表示後タイトルへ
    /// </summary>
    void OnErrorGoTitle()
    {
        Dialog dialog = Dialog.Create(DialogType.DialogOK);
        dialog.Title = GameTextUtil.GetText("error_reject_common_title");
        dialog.Main_text = GameTextUtil.GetText("battle_error_01");
        dialog.SetOkEvent(() => SceneCommon.Instance.ChangeScene(SceneType.SceneTitle));
        dialog.Show();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：バトル：バトル中
	*/
    //----------------------------------------------------------------------------
    void OnBattle()
    {
        //--------------------------------
        // リタイアリクエスト処理
        //--------------------------------
        if (InGameQuestData.Instance && InGameQuestData.Instance.m_InGameRetire)
        {
            m_IsSkillWindowUpdata = false;
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_RETIRE");
            InGamePlayerParty.Instance.m_HandsView = false;
            return;
        }


        //--------------------------------
        // 戦闘管理クラスがリタイアに入っているならゲームオーバーへ移行
        //--------------------------------
        if (BattleParam.getBattlePhase() == BattleParam.BattlePhase.RETIRE)
        {
            m_InGameStepTriger = true;
            m_IsSkillWindowUpdata = false;
            BattleParam.endBattleScene();
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_GAMEOVER");
            InGamePlayerParty.Instance.m_HandsView = false;
            return;
        }

        //--------------------------------
        // 戦闘管理クラスが戦闘状況を終えるまで待機
        //--------------------------------
        if (BattleParam.isActiveBattle() == true)
        {
            return;
        }


        //--------------------------------
        // 戦闘終了
        // →バトル終了フローへ遷移
        //--------------------------------
        m_IsSkillWindowUpdata = false;
        InGamePlayerParty.Instance.m_HandsView = false;
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_END");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：バトル：バトルゲームオーバー
	*/
    //----------------------------------------------------------------------------
    void OnBattleGameOver()
    {
        //--------------------------------
        // ゲームオーバーUIのアニメーション開始
        //--------------------------------
        if (m_InGameStepTriger == true)
        {
            m_InGameStepTriger = false;

            //--------------------------------
            // 常駐フラグの中断復帰時コンティニュー
            // 中断復帰でサーバー側が先行して成立しているケースになる。
            //
            // 例外的にそのまま課金と消費なしでコンティニューを成立させる
            //--------------------------------
            if (ResidentParam.m_QuestRestoreContinue == true)
            {
                ResidentParam.m_QuestRestoreContinue = false;

                //--------------------------------
                // ココの処理は課金後のコンティニュー遷移と同じ処理を持ってきただけ
                //--------------------------------
                {
                    // 次のコンティニュー情報を構築
                    LocalSaveContinue cContinueParam = LocalSaveManager.Instance.LoadFuncInGameContinue();
                    cContinueParam.nContinueNext = cContinueParam.nContinueCt + 1;
                    LocalSaveManager.Instance.SaveFuncInGameContinue(cContinueParam);
                }
                SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_CONTINUE");
                return;
            }

            //--------------------------------
            //
            //--------------------------------
            {
                LocalSaveContinue cContinueParam = LocalSaveManager.Instance.LoadFuncInGameContinue();
            }

            // ゲームオーバー演出開始
            GameObject _tmpObj = Resources.Load("Prefab/InGame/InGameLogo/GameOver") as GameObject;
            m_StringCutIn = Instantiate(_tmpObj) as GameObject;
            m_StringCutIn.transform.SetParent(m_StringCutInRoot.transform, false);
            m_StringCutIn.transform.localPosition = new Vector3(0, 250);
            UnityUtil.SetObjectEnabledOnce(m_StringCutIn, true);
            m_StringCutInAnimation = m_StringCutIn.transform.GetChild(0).gameObject.GetComponent<Animation>();
            string strAnim = m_StringCutInAnimation.clip.name;
            m_StringCutInAnimation[strAnim].speed = m_GameSpeedAmend;
            return;
        }

        if (m_StringCutIn != null)
        {
            //--------------------------------
            // ゲームオーバー演出終了待機
            //--------------------------------
            if (m_StringCutInAnimation.isPlaying == true)
            {
                return;
            }

            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
            if (cOption.m_OptionQuestEndTips == (int)LocalSaveDefine.OptionQuestEndTips.ON)
            {
                // TIPS表示するときは演出を消す
                UnityUtil.SetObjectEnabledOnce(m_StringCutIn, false);
                Destroy(m_StringCutIn);
            }
            m_StringCutIn = null;
            m_StringCutInAnimation = null;

            // クエスト終了時TIPS表示へ
            SceneModeContinuousBattleFSM.Instance.SendFsmNextEvent();
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：バトル：バトルゲームオーバー
	*/
    //----------------------------------------------------------------------------
    void OnHelpDialog()
    {
        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        if (cOption.m_OptionQuestEndTips == (int)LocalSaveDefine.OptionQuestEndTips.OFF)
        {
            // TIPS表示せずに画面タッチ待ち
            InGameMenuManagerQuest2.Instance.m_InGameMenuQuest2.QuestEndButtonActive = true;
            return;
        }

        string[] help_dialog_message =
        {
                "he169p_button2title1", "he170q_content1",
                "he169p_button2title2", "he170q_content2",
                "he169p_button2title3", "he170q_content3",
                "he169p_button2title4", "he170q_content4",
                "he169p_button2title5", "he170q_content5",
                "he169p_button2title6", "he170q_content6",
                "he169p_button2title7", "he170q_content7",
                "he169p_button2title8", "he170q_content8",
                "he169p_button2title9", "he170q_content9",
                "he169p_button2title10", "he170q_content10",
                "he169p_button2title11", "he170q_content11",
                "he169p_button2title12", "he170q_content12",
                "he169p_button2title13", "he170q_content13",
                "he169p_button2title14", "he170q_content14",
                "he169p_button2title15", "he170q_content15",
                "he169p_button2title16", "he170q_content16",
                "he169p_button2title17", "he170q_content17",
                "he169p_button2title18", "he170q_content18",
            };
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        uint index = RandManager.GetRand() % (uint)(help_dialog_message.Length / 2);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, help_dialog_message[index * 2]);
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, help_dialog_message[index * 2 + 1]);
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.EnableCancel();
        newDialog.EnableFadePanel();
        newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            //--------------------------------
            // ダイアログチェックはバトル管理側でやっているので
            // コンティニューせずにそのままリタイアする
            //--------------------------------
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GO_RESULT");
        });
        newDialog.SetDialogEvent(DialogButtonEventType.CANCEL, () =>
        {
            //--------------------------------
            // ダイアログチェックはバトル管理側でやっているので
            // コンティニューせずにそのままリタイアする
            //--------------------------------
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GO_RESULT");
        });
        newDialog.Show();
    }

    public void OnQuestButtonPush()
    {
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GO_RESULT");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		インゲーム処理ステップ：ゲーム：リタイア
		@retval		EINGAME_STEP		[次のステップ]
	*/
    //----------------------------------------------------------------------------
    void OnGameRetire()
    {
        stopBattleBGM(false);

        // 中断復帰情報の削除
        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = null;

        // 獲得品情報の破棄
        InGameQuestData.Instance.ClrAcquireQuest();

        // リザルトへ
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GO_RESULT");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：バトル：バトル終了
	*/
    //----------------------------------------------------------------------------
    void OnBattleEndFunc()
    {
        //--------------------------------
        //	エラーチェック
        //--------------------------------
        if (BattleReqManager.Instance == null)
        {
            Debug.LogError("BattleManager Is  Null!!");
            return;
        }


        //--------------------------------
        //	前回のリクエストを一旦取得
        //--------------------------------
        BattleReq battleReq = BattleReqManager.Instance.GetBattleRequest();
        if (battleReq == null)
        {
            Debug.LogError("GetBattleRequest Failed!!");
            return;
        }

        //		uint EnemyGroupID    = battleReq.m_BattleEnemyGroupID;
        bool bossFlag = battleReq.isBoss;


        //--------------------------------
        //	リクエスト情報破棄
        //--------------------------------
        bool bRet = BattleReqManager.Instance.ErsBattleRequest();
        if (bRet == false)
        {
            Debug.LogError("BattleRequest Delete Failed!!");
            return;
        }

        //--------------------------------
        // 戦闘が終了したので次ステップへ遷移
        //--------------------------------
        if (m_BossStart == true)
        {
            ++m_BossChainCount;
            if (m_ContinuousBattleManager.checkBoosBattleCount() <= m_BossChainCount)
            {
                // ボス戦であれば終了
                SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_CLEAR_STAGE");
            }
            else
            {
                //--------------------------------
                // ボス情報の取得
                //--------------------------------
                int bossUniqueID = m_ContinuousBattleManager.enemyList[m_BattleCount].m_BattleUnique + m_BossChainCount;
                PacketStructQuest2BuildBattle battle_param = InGameUtil.GetQuest2BuildBattle(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild, bossUniqueID);
                //--------------------------------
                // 戦闘リクエスト
                //--------------------------------
                BattleReqManager.Instance.BattleRequestAdd(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild, battle_param, true, true, 0);
                m_InGameStepTriger = false;
                m_IsSkillWindowUpdata = true;
                SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_START");
            }
        }
        else
        {
            m_ContinuousBattleManager.changeBackgroundColor(false);
            m_ContinuousBattleManager.enemyList[m_BattleCount].changeIcon(ContinuousBattleEnemyIcon.ICON_TYPE.TYPE_CLEAR);
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_NEXT");
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：バトル：バトル終了
	*/
    //----------------------------------------------------------------------------
    void OnBattleEndWait()
    {
        if (m_BackGround_LAnimation.isPlaying == true
        || m_BackGround_RAnimation.isPlaying == true)
        {
            return;
        }
        UnityUtil.SetObjectEnabled(m_BackGround_L, false);
        UnityUtil.SetObjectEnabled(m_BackGround_R, false);
        Vector2 sizeDelta = m_BackGround_L.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = 400;
        m_BackGround_L.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        sizeDelta = m_BackGround_R.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = 400;
        m_BackGround_R.GetComponent<RectTransform>().sizeDelta = sizeDelta;

        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_NEXT");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：バトル：バトル終了
	*/
    //----------------------------------------------------------------------------
    void OnBattleNext()
    {
        m_BattleCount += 1;
        SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_PRE_PRODUCTION_WAIT");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：クリア：ボス戦
	*/
    //----------------------------------------------------------------------------
    void OnClearBossBattle()
    {
        if (m_InGameStepTriger == true)
        {
            m_InGameStepTriger = false;
            // ボス出現演出
            GameObject _tmpObj = Resources.Load("Prefab/InGame/InGameLogo/BossAppear") as GameObject;
            m_StringCutIn = Instantiate(_tmpObj) as GameObject;
            m_StringCutIn.transform.SetParent(m_StringCutInRoot.transform, false);
            m_StringCutIn.transform.localPosition = new Vector3(0, 120);
            UnityUtil.SetObjectEnabledOnce(m_StringCutIn, true);
            m_StringCutInAnimation = m_StringCutIn.transform.GetChild(0).gameObject.GetComponent<Animation>();
            // BGM再生情報のリセット
            //			m_BGMPlayed = false;

            // BGM処理
            stopBattleBGM_BossStart(1.0f);

            // ボスアラートSE
            SoundUtil.PlaySE(SEID.SE_BATTLE_BOSS_ALERT);
        }

        //--------------------------------
        // ボス登場演出終了待ち
        //--------------------------------
        //v380 add 倍速対応[BOSS APPEARS]
        string strAnim = m_StringCutInAnimation.clip.name;
        m_StringCutInAnimation[strAnim].speed = m_GameSpeedAmend;

        if (m_StringCutInAnimation.isPlaying == true)
        {
            return;
        }
        UnityUtil.SetObjectEnabledOnce(m_StringCutIn, false);
        Destroy(m_StringCutIn);


        int bossUniqueID = 0;
        PacketStructQuest2BuildBattle battle_param = null;


        //--------------------------------
        // ボス情報の取得
        //--------------------------------
        bossUniqueID = m_ContinuousBattleManager.enemyList[m_BattleCount].m_BattleUnique + m_BossChainCount;
        battle_param = InGameUtil.GetQuest2BuildBattle(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild, bossUniqueID);


        //--------------------------------
        // ボス戦開始済みか確認
        //--------------------------------
        if (battle_param != null)
        {

            //--------------------------------
            // ボスBGM再生
            //--------------------------------
            playBattleBGM();

            //--------------------------------
            // ボス戦が始まっていない状態、尚且つ敵が死んでいない
            //--------------------------------
            {


                //--------------------------------
                // 戦闘リクエスト
                //--------------------------------
                BattleReqManager.Instance.BattleRequestAdd(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild, battle_param, true, false, 0);


                //--------------------------------
                // ボス戦開始フラグ
                //--------------------------------
                m_BossStart = true;

            }
            m_InGameStepTriger = false;
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_BATTLE_START");

        }
        else
        {
            m_InGameStepTriger = true;
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_CLEAR_STAGE");
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	インゲーム処理ステップ：クリア：ステージクリア
	*/
    //----------------------------------------------------------------------------
    void OnClearStage()
    {
        if (SceneGoesParam.Instance != null)
        {
            SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = null;
        }

        //--------------------------------
        // クエストクリア
        //--------------------------------
        {
            if (m_InGameStepTriger == true)
            {
                m_InGameStepTriger = false;

                InGameQuestData.Instance.QuestComplete();

                stopBattleBGM(false);
                SoundUtil.PlaySE(SEID.SE_STARGE_CLEAR);

                //--------------------------------
                // クエストクリア演出
                //--------------------------------
                // UI表示
                {
                    GameObject _tmpObj = Resources.Load("Prefab/InGame/InGameLogo/QuestClear") as GameObject;
                    m_QuestClear = Instantiate(_tmpObj) as GameObject;
                    m_QuestClear.transform.SetParent(m_StringCutInRoot.transform, false);
                    m_QuestClear.transform.localPosition = new Vector3(0, 200);
                    m_QuestClearAnim = m_QuestClear.transform.GetChild(0).gameObject.GetComponent<Animation>();
                    UnityUtil.SetObjectEnabledOnce(m_QuestClear, true);
                }
                // エフェクト表示
                if (EffectManager.Instance != null)
                {
                    EffectManager.Instance.playEffect2(SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DGameClear, new Vector3(0, 0.5f), Vector3.zero, null, null, 1.0f, "m_GameObjEffStageClear");
                }
            }

            if (m_QuestClear != null && m_QuestClearAnim != null)
            {
                string strAnim = m_QuestClearAnim.clip.name;
                m_QuestClearAnim[strAnim].speed = m_GameSpeedAmend;

                //--------------------------------
                // クリア演出終了待機
                //--------------------------------
                if (m_QuestClearAnim.isPlaying)
                {
                    return;
                }

                LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
                if (cOption.m_OptionQuestEndTips == (int)LocalSaveDefine.OptionQuestEndTips.ON)
                {
                    // TIPS表示するときは演出を消す演出を消す
                    UnityUtil.SetObjectEnabledOnce(m_QuestClear, false);
                    if (EffectManager.HasInstance)
                    {
                        EffectManager.Instance.stopEffect("m_GameObjEffStageClear");
                    }
                }
                m_QuestClear = null;
                m_QuestClearAnim = null;

                if (TutorialManager.IsExists)
                {
                    // チュートリアル中は即時終了
                    SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GO_RESULT");
                }

                // クエスト終了時TIPS表示へ
                SceneModeContinuousBattleFSM.Instance.SendFsmNextEvent();
            }
        }
    }

    public void OnGoResult()
    {

        // 中断復帰データの削除
        InGameUtil.RemoveLocalData();

        // 通常戦闘時のBGM停止
        stopBattleBGM(false);

        if (TutorialManager.HasInstance)
        {
            //チュートリアル中
            TutorialFSM.Instance.SendEvent_FinishBattle();
        }
        else
        {
            //--------------------------------
            // ランキング情報登録
            //--------------------------------
            // ダメージランキング
            InGameUtil.SubmitDamageRanking();

            //----------------------------------------
            // リザルトへの遷移を期待されているならメインメニューへ
            // リザルトはメインメニュー上で表示される
            //----------------------------------------
#if DEBUG_EXPORT_BATTLE_LOG && BUILD_TYPE_DEBUG
            uploadBattleLog();
            System.GC.Collect();
#else
            AndroidBackKeyManager.Instance.DisableBackKey();
            SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
#endif
        }
        if (InGameQuestData.Instance.m_InGameClear == true)
        {
            //----------------------------------------
            // クエストリザルト情報を受け渡しクラスへ入力
            //----------------------------------------
            uint unQuestID = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID;
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu = new SceneGoesParamToMainMenu();
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_QuestID = unQuestID;
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_AreaID = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestAreaID;
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_PartyFriend = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyFriend;

            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_EnemyKillCountList = BattleParam.m_AchievementTotalingInBattle.getEnemyKillCount();
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_LimitBreakUseList = BattleParam.m_AchievementTotalingInBattle.getLimitBreakSkillUse();
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_NoDamagePlayer = BattleParam.m_AchievementTotalingInBattle.isNoDamagePlayer();
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_MaxDamageToEnemy = BattleParam.m_AchievementTotalingInBattle.getMaxDamageToEnemy();
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_HeroSkillUseCount = BattleParam.m_AchievementTotalingInBattle.getHeroSkillUseCount();
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_NextAreaCleard = m_NextAreaCleard;
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_PlayScoreInfo = BattleParam.m_PlayScoreInfo;
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_IsUsedAutoPlay = BattleParam.isUsedAutoPlay();

            SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_Quest2 = true;

            //----------------------------------------
            // リザルト情報をローカルセーブ。
            // リザルトシーンに行くまでにハングした場合に復旧できるようにしておく
            //----------------------------------------
            LocalSaveManager.Instance.SaveFuncGoesToQuest2Restore(null);
            LocalSaveManager.Instance.SaveFuncGoesToQuest2Start(null);
            if (TutorialManager.Instance == null
            || (TutorialManager.Instance != null &&
                 TutorialManager.IsExists == false))
                LocalSaveManager.Instance.SaveFuncGoesToMenuResult(SceneGoesParam.Instance.m_SceneGoesParamToMainMenu);
            else
                LocalSaveManager.Instance.SaveFuncGoesToMenuResult(null);
            LocalSaveManager.Instance.SaveFuncGoesToMenuRetire(null);


        }
        else
        {
            uint unQuestID = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID;
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire = new SceneGoesParamToMainMenuRetire();
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire.m_QuestID = (uint)unQuestID;
            SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire.m_IsUsedAutoPlay = BattleParam.isUsedAutoPlay();

            //----------------------------------------
            // リタイア情報をローカルセーブ。
            // リザルトシーンに行くまでにハングした場合に復旧できるようにしておく
            //----------------------------------------
            LocalSaveManager.Instance.SaveFuncGoesToQuest2Restore(null);
            LocalSaveManager.Instance.SaveFuncGoesToQuest2Start(null);
            LocalSaveManager.Instance.SaveFuncGoesToMenuResult(null);
            LocalSaveManager.Instance.SaveFuncGoesToMenuRetire(SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラステータスウィンドウOPEN
		@param[in]	GlobalDefine.PartyCharaIndex	(id)			キャラパーティインデックス
	*/
    //----------------------------------------------------------------------------
    public void SkillWindowOpen(GlobalDefine.PartyCharaIndex id)
    {
        if (m_InGameSkillWindowQuest2 == null)
        {
            if (InGameMenuManagerQuest2.Instance.isSkillMenuActive == false)
            {
                GameObject _tmpObj = Resources.Load("Prefab/InGame/InGameUI/Menu/Quest2SkillWindow") as GameObject;
                m_SkillWindowObject = Instantiate(_tmpObj) as GameObject;
                m_SkillWindowObject.transform.SetParent(m_SkillWindowRoot.transform, false);
                m_InGameSkillWindowQuest2 = m_SkillWindowObject.GetComponent<InGameSkillWindowQuest2>();
                m_InGameSkillWindowQuest2.Open(id);
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラステータスウィンドウCLOSE
	*/
    //----------------------------------------------------------------------------
    public void SkillWindowClose()
    {
        if (m_InGameSkillWindowQuest2 != null)
        {
            if (m_InGameSkillWindowQuest2.isClose == true)
            {
                m_InGameSkillWindowQuest2 = null;
                Destroy(m_SkillWindowObject);
                m_SkillWindowObject = null;
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	主人公ステータスウィンドウOPEN
	*/
    //----------------------------------------------------------------------------
    public void HeroWindowOpen()
    {
        if (m_InGameSkillWindowQuest2 == null)
        {
            GameObject _tmpObj = Resources.Load("Prefab/InGame/InGameUI/Menu/Quest2SkillWindow") as GameObject;
            m_SkillWindowObject = Instantiate(_tmpObj) as GameObject;
            m_SkillWindowObject.transform.SetParent(m_SkillWindowRoot.transform, false);
            m_InGameSkillWindowQuest2 = m_SkillWindowObject.GetComponent<InGameSkillWindowQuest2>();
            m_InGameSkillWindowQuest2.OpenHero();
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	AssetBundle関連処理：ゲーム内で使用するリソースの先読み
	*/
    //----------------------------------------------------------------------------
    private AssetBundlerMultiplier AssetBundleLoad()
    {
        LoadingManager.Instance.RequestLoadingFinish();
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.DATA_DOWNLOAD);

        //----------------------------------------
        // AssetBundleManagerが処理しやすいように、
        // リソースが必要なキャラのIDをリスト化する
        // @change Developer 2016/02/19 v330 板ポリ対応
        //----------------------------------------
        TemplateList<uint> acUnitIDPreloadTexture = new TemplateList<uint>();

        //----------------------------------------
        // プレイヤーパーティのリソースを先読み実行
        //----------------------------------------
        if (SceneGoesParam.Instance != null
        && SceneGoesParam.Instance.m_SceneGoesParamToQuest2 != null
        )
        {
            UserDataUnitParam[] acUnitParam = {
                                                SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara0Param
                                            ,   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara1Param
                                            ,   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara2Param
                                            ,   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara3Param
                                            ,   SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyChara4Param
                                            };
            for (int i = 0; i < acUnitParam.Length; i++)
            {
                if (acUnitParam[i] == null)
                {
                    continue;
                }
                if (acUnitParam[i].m_UnitDataID == 0)
                {
                    continue;
                }

                //----------------------------------------
                // プレイヤー側はメッシュ使わないのでテクスチャだけ参照
                //----------------------------------------
                acUnitIDPreloadTexture.Add(acUnitParam[i].m_UnitDataID);

                //----------------------------------------
                // リンクキャラ：テクスチャ参照
                // @add Developer 2015/10/16
                //----------------------------------------
                if (acUnitParam[i].m_UnitParamLinkID != 0)
                {
                    acUnitIDPreloadTexture.Add(acUnitParam[i].m_UnitParamLinkID);
                }
            }
        }


        //----------------------------------------
        // 敵キャラ系のリソースを先読み実行
        //----------------------------------------
        // クエストID確定
        uint unQuestID = 0;
        if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2 != null)
        {
            unQuestID = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID;
        }
        else if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore != null)
        {
            unQuestID = (uint)SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_QuestMissionID;
        }
        else
        {
            Debug.LogError("Quest2 AssetBundle Preload Error!");
            return null;
        }

        //	出現する敵のマスターを元に事前読み込み登録
        MasterDataParamEnemy[] e_param_array = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_e_param;
        for (int i = 0; i < e_param_array.Length; i++)
        {
            if (e_param_array[i].chara_id == 0)
            {
                continue;
            }
            acUnitIDPreloadTexture.Add(e_param_array[i].chara_id);
        }
        //----------------------------------------
        // リストをソート。
        //
        // AssetBundleはキャラの塊でファイルが分かれており、
        // 近いIDのキャラは一度の読み込みで複数解決できる可能性があるため、
        // 可能な限りソートして順番を並べている
        // @change Developer 2016/02/19 v330 板ポリ対応
        //----------------------------------------
        acUnitIDPreloadTexture.Sort(TemplateListSort.SortUint);

        AssetBundlerMultiplier assetLoader = AssetBundlerMultiplier.Create();
        //----------------------------------------
        // AssetBundleのキャラテクスチャ読み込みリクエスト発行
        //----------------------------------------
        for (int i = 0; i < acUnitIDPreloadTexture.m_BufferSize; i++)
        {
            //----------------------------------------
            // 同じキャラを複数リクエストかける意味はないので
            // 同一キャラIDが連続した場合は弾く
            //----------------------------------------
            if (i > 0)
            {
                if (acUnitIDPreloadTexture[i] == acUnitIDPreloadTexture[i - 1])
                    continue;
            }

            assetLoader.Add(AssetBundler.Create().SetAsUnitTexture(acUnitIDPreloadTexture[i]));
        }

        MasterDataQuest2 master = MasterDataUtil.GetQuest2ParamFromID(m_QuestMissionID);
        int background = (master == null) ? 0 : master.background;
        m_BackGroundAssetBundleName = MasterDataUtil.GetMasterDataQuestBackgroundName(background);
        assetLoader.Add(AssetBundler.Create().
            Set(m_BackGroundAssetBundleName, m_BackGroundAssetBundleName,
                (o) =>
                {
                    m_ContinuousBattleManager.Battle_bg = o.AssetBundle.LoadAsset<Sprite>(m_BackGroundAssetBundleName);
                    if (m_ContinuousBattleManager.Battle_bg == null)
                    {
#if BUILD_TYPE_DEBUG
                        Debug.LogError("QUEST AssetBundle BackGround Sprite Not Found [" + m_BackGroundAssetBundleName + "]");
#endif
                        m_ContinuousBattleManager.Battle_bg = m_NewBattleBg;
                    }
                },
                (s) =>
                {
#if BUILD_TYPE_DEBUG
                    Debug.LogError("QUEST AssetBundle BackGround Data Not Found [" + m_BackGroundAssetBundleName + "]");
#endif
                    m_ContinuousBattleManager.Battle_bg = m_NewBattleBg;
                })
            );

#if true
        // BGMデータの事前ダウンロード
        {
            m_BgmAssetNameList.Clear();

            List<string> battle_bgm_asset_bundle_name_list = new List<string>();
            for (int idx = 0; idx < SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_battle.Length; idx++)
            {
                PacketStructQuest2BuildBattle b = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_battle[idx];
                if (b != null
                    && b.bgm_id > 0
                )
                {
                    string asset_bundle_name = BGMManager.getBattleBgmAssetBundleName(b.bgm_id);

                    if (battle_bgm_asset_bundle_name_list.Contains(asset_bundle_name) == false)
                    {
                        battle_bgm_asset_bundle_name_list.Add(asset_bundle_name);

                        assetLoader.Add(
                            AssetBundler.Create().Set(asset_bundle_name,
                            (o) =>
                            {
                                string[] bgm_list = o.AssetBundle.GetAllAssetNames();
                                for (int idx_bgm = 0; idx_bgm < bgm_list.Length; idx_bgm++)
                                {
                                    string bgm_asset_name = Path.GetFileNameWithoutExtension(bgm_list[idx_bgm]);
                                    m_BgmAssetNameList.Add(bgm_asset_name);
                                }
                            }
                            )
                        );
                    }
                }
            }
        }
#endif

        // 主人公
        int currentHeroID = UserDataAdmin.Instance.HeroId;
#if BUILD_TYPE_DEBUG
        Debug.Log("QUEST_ID:" + SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID);
        Debug.Log("AREA_ID:" + SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestAreaID);
        Debug.Log("CURRENT_HERO_ID:" + currentHeroID);
#endif
        assetLoader.Add(
            AssetBundler.Create().Set(string.Format("hero_{0:D4}", currentHeroID),
                (o) =>
                {
                    InGamePlayerParty.Instance.m_HeroSprite = o.GetHeroThumbnailSprite(currentHeroID);
                    InGamePlayerParty.Instance.m_HeroSprite_mask = o.GetHeroThumbnailSprliteMask(currentHeroID);
                },
                (str) =>
                {
                    Debug.LogError("ERROR:" + str);
                }));

        assetLoader.RegisterProgressFilesAction(
            (float count, float max) =>
            {
                LoadingManager.Instance.ProgressFiles(count, max);
            });

        return assetLoader;
    }

    /// <summary>
    /// バトルＢＧＭ再生
    /// </summary>
    private void playBattleBGM()
    {
        int bgm_id = 0;
        {
            int battle_unique = m_ContinuousBattleManager.enemyList[m_BattleCount].m_BattleUnique;
            PacketStructQuest2BuildBattle battle_param = InGameUtil.GetQuest2BuildBattle(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild, battle_unique);
            if (battle_param != null)
            {
                bgm_id = battle_param.bgm_id;
            }
        }

        // 同じ曲なら再生し直ししない
        if (bgm_id != m_CurrentBGM)
        {
            SoundUtil.StopBGM(true);

            string bgm_asset_name = BGMManager.getBattleBgmName(bgm_id);

            BGMManager.PlayBattleBGM(bgm_asset_name, (m_CurrentBGM != 0), m_BgmAssetNameList);
            m_CurrentBGM = bgm_id;
        }
    }

    /// <summary>
    /// バトルＢＧＭ停止
    /// </summary>
    /// <param name="is_fadeout"></param>
    private void stopBattleBGM(bool is_fadeout)
    {
        SoundUtil.StopBGM(is_fadeout);
        m_CurrentBGM = 0;
    }

    /// <summary>
    /// ボス戦開始時のバトルＢＧＭ処理
    /// ザコ戦とボス戦のＢＧＭが同じときは一時的に音量を下げる、異なるときは停止
    /// </summary>
    /// <param name="duration">音量を下げる時間（秒）</param>
    private void stopBattleBGM_BossStart(float duration)
    {
        if (m_CurrentBGM != 0)
        {
            int bgm_id = 0;
            {
                int battle_unique = m_ContinuousBattleManager.enemyList[m_BattleCount].m_BattleUnique;
                PacketStructQuest2BuildBattle battle_param = InGameUtil.GetQuest2BuildBattle(SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild, battle_unique);
                if (battle_param != null)
                {
                    bgm_id = battle_param.bgm_id;
                }
            }

            if (bgm_id == m_CurrentBGM)
            {
                // BGMが変わらない場合は音量を下げるだけ.
                BGMManager.Instance.Ducking(duration);
            }
            else
            {
                // BGMが変わる場合は停止.
                stopBattleBGM(true);
            }
        }
    }

    private void OnSelectBackKey()
    {
        // メニューオープンタイミングのチェック
        bool isOpen = InGameUtil.ChkInGameOpenWindowTiming();
        if (isOpen == false)
        {
            return;
        }

        if (m_SkillWindowObject != null)
        {
            if (m_InGameSkillWindowQuest2.isOpened() == true)
            {
                m_InGameSkillWindowQuest2.OnClose();
            }
        }
        else if (InGameMenuManagerQuest2.Instance.m_InGameMenuQuest2.Menu_active == true)
        {
            InGameMenuManagerQuest2.Instance.OnBack();
        }
        else if (InGameMenuManagerQuest2.Instance.isSkillMenuActive == true)
        {
            InGameMenuManagerQuest2.Instance.OnSkillNo();
        }
        else if (BattleSceneManager.Instance.isTargetWindowOpen() == true)
        {
            BattleSceneManager.Instance.TargetWindowClose();
        }
        else
        {
            if (TutorialManager.Instance == null
            || (TutorialManager.Instance != null &&
                 TutorialManager.IsExists == false))
            {
                InGameMenuManagerQuest2.Instance.OpenInGameMenu(InGameMenuID.GAMEMENU_RETIRE, true);
            }
        }
    }

    public override SceneType GetSceneType()
    {
        return SceneType.SceneQuest2;
    }

    public string GetWaveCount()
    {
        if (m_ContinuousBattleManager.enemyList[m_BattleCount].m_Boss == true)
        {
            return GameTextUtil.GetText("battle_wave_02");
        }
        return string.Format(GameTextUtil.GetText("battle_wave_01"), m_BattleCount + 1, m_EnemyCount - 1);
    }

#if DEBUG_EXPORT_BATTLE_LOG && BUILD_TYPE_DEBUG
    /// <summary>
    /// バトルログの送信
    /// </summary>
    private void uploadBattleLog()
    {
        string filename = DebugBattleLog.getFileName();
        string filepath = DebugBattleLog.getSavePath() + "/" + filename;

        if (DebugBattleLog.getEneble() == false)
        {
            finishBattleLog("uploadBattleLog disable log error");
            return;
        }

        ApiEnv env = GlobalDefine.GetApiEnv();
        if (ApiEnv.API_TEST_ADDRESS_ONLINE == env ||
           ApiEnv.API_TEST_ADDRESS_LOCAL_GOE == env ||
           ApiEnv.NONE == env)
        {

            finishBattleLog("uploadBattleLog not support server error");
            return;
        }

        if (filename == null || filename.Length <= 0)
        {
            finishBattleLog("uploadBattleLog not filename error");
            return;
        }

        if (System.IO.File.Exists(filepath) == false)
        {
            finishBattleLog("uploadBattleLog not file path error: " + filepath);
            return;
        }

        string logtext = System.IO.File.ReadAllText(filepath);

        if (logtext.Length <= 0)
        {
            finishBattleLog("uploadBattleLog not file size zero error: " + filepath);
            return;
        }

        sendBattleLog(logtext);
    }

    private void sendBattleLog(string str)
    {
        ServerDataUtilSend.SendPacketAPI_DebugBattleLog(1, str)
        .setSuccessAction(_data =>
        {
            var url = _data.GetResult<RecvDebugBattleLog>().result.url;
            finishBattleLog("uploadBattleLog SendPacketAPI_DebugBattleLog RecvData.result.url: " + url);
        })
        .setErrorAction(_data =>
        {
            if (_data.m_PacketCode != API_CODE.API_CODE_WIDE_ERROR_MENTENANCE ||
                _data.m_PacketCode != API_CODE.API_CODE_POST_LOG_UPLOAD_ERROR)
            {
                var RecvData = _data.GetResult<RecvDebugBattleLog>();
                if (RecvData != null)
                {
                    finishBattleLog("uploadBattleLog SendPacketAPI_DebugBattleLog error: " + RecvData.header.code);
                }
                else
                {
                    finishBattleLog("uploadBattleLog SendPacketAPI_DebugBattleLog error: RecvData NULL");
                }
            }
        })
        .SendStart();
    }

    private void finishBattleLog(string logtext)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log(logtext);
#endif
        AndroidBackKeyManager.Instance.DisableBackKey();
        SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
    }
#endif
}
