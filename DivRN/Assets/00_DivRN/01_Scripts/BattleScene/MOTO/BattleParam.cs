using UnityEngine;
using System.Collections;

/// <summary>
/// バトル処理と探索処理をつなぐためのクラス。バトル・探索間のやり取りはこのクラスを経由して行うようにする。
/// バトル処理を単独で実行できるようにするため、バトルと他の処理を切り離すのが目的のクラス。
/// </summary>
public static class BattleParam
{
    // ・バトルシーンが参照している外部変数
    // ・バトルシーン以外からよく参照されるバトルシーンの変数
    // これらを集めたもの.
    // 検証中なのでアクセスしやすい static にしています.


    // InGameManager に返す値
    // 勝/敗/リタイア
    // パーティ体力・パーティＳＰ・パーティ状態異常
    // キャラスキルターン
    // 鍵取得・ドロップアイテム番号


    //----- InGameManager側にあった変数.
    public static uint m_QuestAreaID = 0;
    public static uint m_QuestMissionID = 0;
    public static CharaParty m_PlayerParty = null;
    public static MasterDataCache m_MasterDataCache = new MasterDataCache();

    public static int m_QuestFloor;
    public static bool m_AcquireKey = false;    // 鍵を取得済みかどうか（BattleManager側のスキルで取得できる場合がある）

    private static GlobalDefine.PartyCharaIndex m_LBSCharaIdx = GlobalDefine.PartyCharaIndex.ERROR; // マップ側からのスキル発動要求（発動したキャラのパーティ内番号）
    public const int SKILL_LIMIT_BREAK_ADD_MAX = 1; // リミブレスキルを連結できる最大数
    public static SkillRequestParam m_SkillRequestLimitBreak = new SkillRequestParam(SKILL_LIMIT_BREAK_ADD_MAX);    // マップ側からのスキル発動要求（スキル種類）

    public static int m_QuestTotalTurn = 0;                      //!< クリアまでのターン数

#if UNITY_EDITOR
    public static bool m_IsDamageOutput = false;                    // ダメージログ出力フラグ
#endif


    //----- BattleManager側にあった変数.
    public static BattleReq m_BattleRequest = null;
    public static ServerDataDefine.PacketStructQuest2Build m_QuestBuild = null;
    public static BattleEnemy[] m_EnemyParam = null;                //!< 敵情報：敵パラメータ

    public static int m_TargetEnemyCurrent = InGameDefine.SELECT_NONE;              //!< 現在のターゲット対象
    public static int m_TargetEnemyWindow = InGameDefine.SELECT_NONE;


    //----- InGameManager側にあった関数.

    //-----	その他の変数・新設した変数
    public static ServerDataDefine.PlayScoreInfo m_PlayScoreInfo = null;

    public static AchievementTotalingInBattle m_AchievementTotalingInBattle = null;

    private static bool m_IsKobetsuHP = false;
    public static bool IsKobetsuHP
    {
        get { return m_IsKobetsuHP; }
    }
    public static void setKobetsuMode(bool is_kobetsu_hp)
    {
        m_IsKobetsuHP = is_kobetsu_hp;
    }

    private static int m_BattleRound = 0;   //何戦目か（新クエスト用）
    public static int BattleRound
    {
        get { return m_BattleRound; }
    }

    private static uint m_QuestRandSeed = 0;

    /// <summary>
    /// チュートリアル中かどうか
    /// </summary>
    public static bool IsTutorial()
    {
        //クエストＩＤが「10001」の時はチュートリアル.
        bool ret_val = (m_QuestMissionID == GlobalDefine.TUTORIAL_QUEST_1);
        return ret_val;
    }

    /// <summary>
    /// チュートリアル：右下の「オプション」ボタンの使用が許可されているか
    /// </summary>
    /// <returns></returns>
    public static bool isEnbaleOptionButton()
    {
        return BattleSceneManager.Instance.isTutorialEnableOptionButton();
    }

    public static BattleTutorialManager.TutorialOptionMenuPhase getTutorialOptionMenuPhase()
    {
        return BattleSceneManager.Instance.getTutorialOptionMenuPhase();
    }

    private static int[/*member_idx*/] m_PartyMemberHands = new int[(int)GlobalDefine.PartyCharaIndex.MAX];
    private static int[/*field_idx*/] m_FieldSkillCounts = new int[(int)GlobalDefine.PartyCharaIndex.MAX];
    private static int[/*skill_idx*/, /*member_idx*/] m_FormedSkillCounts = new int[3, (int)GlobalDefine.PartyCharaIndex.MAX];
    /// <summary>
    /// 各メンバーのスキル成立数を取得
    /// </summary>
    /// <param name="member_index"></param>
    /// <returns></returns>
    public static int getPartyMemberHands(GlobalDefine.PartyCharaIndex member_index)
    {
        if (member_index == GlobalDefine.PartyCharaIndex.GENERAL)
        {
            member_index = m_PlayerParty.getGeneralPartyMember();
        }

        if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
        {
            int ret_val = m_PartyMemberHands[(int)member_index];

            if (ret_val > 0)
            {
                return ret_val;
            }
        }

        return 0;
    }

    public static void plusPartyMemberHands(GlobalDefine.PartyCharaIndex member_index, int value)
    {
        if (member_index == GlobalDefine.PartyCharaIndex.GENERAL)
        {
            member_index = m_PlayerParty.getGeneralPartyMember();
        }

        if (member_index >= GlobalDefine.PartyCharaIndex.LEADER && member_index < GlobalDefine.PartyCharaIndex.MAX)
        {
            m_PartyMemberHands[(int)member_index] += value;
            if (m_PartyMemberHands[(int)member_index] < 0)
            {
                m_PartyMemberHands[(int)member_index] = 0;
            }
        }
    }

    public static int getFieldSkillCount(int field_index)
    {
        int ret_val = m_FieldSkillCounts[field_index];

        return ret_val;
    }

    public static void plusFieldSkillCount(int field_index, int value)
    {
        m_FieldSkillCounts[field_index] += value;
        if (m_FieldSkillCounts[field_index] < 0)
        {
            m_FieldSkillCounts[field_index] = 0;
        }
    }

    /// <summary>
    /// 各メンバー、各スキルの成立数を取得
    /// </summary>
    /// <param name="skill_index">0:ノーマルスキル１ 1:ノーマルスキル２ 2:汎用スキル</param>
    /// <param name="member_index"></param>
    /// <returns></returns>
    public static int getFormedSkillCounts(int skill_index, GlobalDefine.PartyCharaIndex member_index)
    {
        return m_FormedSkillCounts[skill_index, (int)member_index];
    }
    public static void plusFormedSkillCounts(int skill_index, GlobalDefine.PartyCharaIndex member_index, int value)
    {
        int new_value = m_FormedSkillCounts[skill_index, (int)member_index] + value;
        if (new_value < 0)
        {
            new_value = 0;
        }
        m_FormedSkillCounts[skill_index, (int)member_index] = new_value;
    }

    /// <summary>
    /// パーティインターフェイスのスキルターン数表示状態を取得
    /// </summary>
    public static bool isShowPartyInterfaceSkillTurn()
    {
        bool ret_val = false;
        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        if (cOption != null)
        {
            ret_val = ((LocalSaveDefine.OptionBattleSkillTurn)cOption.m_OptionBattleSkillTurn == LocalSaveDefine.OptionBattleSkillTurn.ON);
        }
        return ret_val;
    }

    /// <summary>
    /// パーティインターフェイスのノーマルスキル発動条件の表示状態を取得
    /// </summary>
    public static bool isShowPartyInterfaceSkillCost()
    {
        bool ret_val = false;
        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        if (cOption != null)
        {
            ret_val = ((LocalSaveDefine.OptionBattleSkillCost)cOption.m_OptionBattleSkillCost == LocalSaveDefine.OptionBattleSkillCost.ON);
        }
        return ret_val;
    }

    public static GlobalDefine.PartyCharaIndex m_EnemyToPlayerTarget = GlobalDefine.PartyCharaIndex.ERROR;  // 敵がプレイヤーを攻撃する際のターゲット

    public enum AutoPlayState
    {
        NONE,   // オートプレイなし
        OFF,    // オートプレイ：非稼働
        ON,     // オートプレイ：稼働中
        CANCEL, // オートプレイ：キャンセル中
    }
    private static AutoPlayState m_AutoPlayState = AutoPlayState.NONE;   // オートプレイボタンの状態
    private static bool m_IsUsedAutoPlay = false;   // クエスト中にオートプレイを使用したかどうか

    public static AutoPlayState getAutoPlayState()
    {
        return m_AutoPlayState;
    }

    public static void setAutoPlayState(AutoPlayState auto_play_state)
    {
        if (m_AutoPlayState != auto_play_state)
        {
            m_AutoPlayState = auto_play_state;

            bool is_on_auto_play = (m_AutoPlayState == AutoPlayState.ON);

            if (is_on_auto_play)
            {
                m_IsUsedAutoPlay = true;
            }

            LocalSaveDefine.OptionAutoPlayEnable option_on_off = (is_on_auto_play) ? LocalSaveDefine.OptionAutoPlayEnable.ON : LocalSaveDefine.OptionAutoPlayEnable.OFF;

            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
            if (cOption.m_OptionAutoPlayEnable != (int)option_on_off)
            {
                cOption.m_OptionAutoPlayEnable = (int)option_on_off;
                LocalSaveManager.Instance.SaveFuncOption(cOption);  // ここでオプションのセーブは処理落ちの原因になるかも
            }
        }
    }

    // クエスト開始時にオートプレイ使用状況を初期化
    public static void initUsedAutoPlay(bool init_value)
    {
        m_IsUsedAutoPlay = init_value;
    }
    // オートプレイ使用状況を取得
    public static bool isUsedAutoPlay()
    {
        return m_IsUsedAutoPlay;
    }

    /// <summary>
    /// 変数の内容を初期化（クエスト終了時に初期化しておく）
    /// </summary>
    public static void clearValues()
    {
        m_QuestAreaID = 0;
        m_QuestMissionID = 0;
        m_PlayerParty = null;
        m_MasterDataCache.clearCachePlayerAll();
        m_MasterDataCache.clearCacheEnemyAll();
        m_QuestFloor = 0;
        m_AcquireKey = false;
        m_LBSCharaIdx = GlobalDefine.PartyCharaIndex.ERROR;
        m_SkillRequestLimitBreak.clearRequest();
        m_QuestTotalTurn = 0;
        m_BattleRequest = null;
        m_QuestBuild = null;
        m_EnemyParam = null;
        m_TargetEnemyCurrent = InGameDefine.SELECT_NONE;
        m_TargetEnemyWindow = InGameDefine.SELECT_NONE;
        m_PlayScoreInfo = null;
        m_AchievementTotalingInBattle = null;
        m_IsKobetsuHP = true;
        m_BattleRound = 0;
        m_QuestRandSeed = 0;
        for (int idx = 0; idx < m_PartyMemberHands.Length; idx++)
        {
            m_PartyMemberHands[idx] = 0;
        }
        for (int idx = 0; idx < m_FieldSkillCounts.Length; idx++)
        {
            m_FieldSkillCounts[idx] = 0;
        }
        for (int skill_idx = 0; skill_idx < m_FormedSkillCounts.GetLength(0); skill_idx++)
        {
            for (int member_idx = 0; member_idx < m_FormedSkillCounts.GetLength(1); member_idx++)
            {
                m_FormedSkillCounts[skill_idx, member_idx] = 0;
            }
        }
        m_EnemyToPlayerTarget = GlobalDefine.PartyCharaIndex.ERROR;
        m_AutoPlayState = AutoPlayState.OFF;
        m_IsUsedAutoPlay = false;
    }

#if BUILD_TYPE_DEBUG
    // バトル関連のマスターデータを読む際ローカルのJsonファイルから読むかどうか（スキルや敵の検証作業用）
    public static bool m_IsUseDebugJsonMasterData = false;

    // デバッグ機能
    public enum DebugBattleStartType
    {
        NORMAL,     // 通常
        FORCE_FIRST_ATTACK, // 強制ファーストアタック
        FORCE_BACK_ATTACK, // 強制バックアタック
    }
    public static DebugBattleStartType m_DebugBattleStartType = DebugBattleStartType.NORMAL;

    // 確率発動のスキルの確率を強制的に１００％にする
    public static bool m_DebugForce100PercentSkill = false;

    // 敵の被ダメージを１０００倍に
    public static bool m_DebugEnemyDamage1000 = false;
#endif

    // リミットブレイク技はマップシーンのインターフェイスのボタンが押されたら発動し、バトルシーンへリクエストが発生する.

    /// <summary>
    /// リミットブレイクスキル発動できるかをチェック
    /// </summary>
    /// <param name="player_index"></param>
    /// <returns></returns>
    public static bool IsEnableLBS(GlobalDefine.PartyCharaIndex player_index)
    {
        bool is_enable = BattleSceneUtil.checkLimitBreak(m_PlayerParty, player_index, m_EnemyParam, m_TargetEnemyCurrent);

        return is_enable;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		LBSリクエストチェック
        @retval		bool		[リクエストあり/なし]
    */
    //----------------------------------------------------------------------------
    public static bool ChkRequestLBS()
    {
        return (m_LBSCharaIdx != GlobalDefine.PartyCharaIndex.ERROR) ? true : false;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		LBSリクエスト設定
        @param[in]	int		(nIdx)		キャラインデックス
    */
    //------------------------------------------------------------------------
    public static void RequestLBS(GlobalDefine.PartyCharaIndex nIdx)
    {
        if (nIdx >= 0 && nIdx < GlobalDefine.PartyCharaIndex.MAX)
        {
            m_LBSCharaIdx = nIdx;
        }
        else if (nIdx == GlobalDefine.PartyCharaIndex.HERO
            && IsKobetsuHP
        )
        {
            m_LBSCharaIdx = nIdx;
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		LBSリクエストクリア
    */
    //------------------------------------------------------------------------
    public static void ClrLBS()
    {
        m_LBSCharaIdx = GlobalDefine.PartyCharaIndex.ERROR;
    }

    public static GlobalDefine.PartyCharaIndex _GetLBS()
    {
        return m_LBSCharaIdx;
    }

    // ヒーロースキル発動要求かどうか
    public static bool _IsHeroSkillLBS()
    {
        return (m_LBSCharaIdx == GlobalDefine.PartyCharaIndex.HERO);
    }

    public static void IncrementTotalTurn()
    {
        InGameUtil.IncrementTotalTurn();
    }

    /// <summary>
    /// 中断データを保存
    /// チュートリアル中はローカルセーブロードしない
    /// </summary>
    public static void SaveLocalData()
    {
        if (IsTutorial() == false)
        {
            InGameUtil.SaveLocalData();
        }
    }

    /// <summary>
    /// 中断データを取得
    /// チュートリアル中はローカルセーブロードしない
    /// </summary>
    /// <returns></returns>
    public static RestoreBattle getRestoreData()
    {
        RestoreBattle restore_battle = null;
        if (IsTutorial() == false)
        {
            restore_battle = InGameUtil.GetRestoreBattle();
        }
        return restore_battle;
    }

    public static bool ChkNoContinueQuest(uint missionID)
    {
        return InGameUtil.ChkNoContinueQuest(missionID);
    }

    private static void initBattleParam(SceneModeContinuousBattle continuous_battle)
    {
        if (continuous_battle != null)
        {
            m_QuestAreaID = continuous_battle.m_QuestAreaID;
            m_QuestMissionID = continuous_battle.m_QuestMissionID;
            m_PlayerParty = continuous_battle.m_PlayerParty;

            m_QuestFloor = continuous_battle.m_QuestFloor;
            m_AcquireKey = continuous_battle.m_AcquireKey;

            m_LBSCharaIdx = continuous_battle.m_LBSCharaIdx;
            m_SkillRequestLimitBreak = continuous_battle.m_SkillRequestLimitBreak;

            m_QuestTotalTurn = continuous_battle.m_QuestTotalTurn;

            m_BattleRound = continuous_battle.battleCount;
            m_QuestRandSeed = continuous_battle.m_QuestRandSeed;
        }
    }

    public static void loadBattlePrefab()
    {
        if (BattleSceneManager.Instance == null)
        {
            GameObject prefab = Resources.Load("Prefab/BattleScene/BattleScenePrefab", typeof(GameObject)) as GameObject;   //TODO:とりあえず決め打ちでファイル指定
            GameObject obj = GameObject.Instantiate(prefab);
            if (obj != null)
            {
                obj.AddComponent<DontDestroy>();    // シーン切り替え中に削除されないようにする
            }

            BattleSceneManager.Instance.setBattleScenePhase(BattleSceneManager.BattleScenePhase.NOT_BATTLE);
            BattleSceneManager.Instance.PRIVATE_FIELD.CreateBattleGameObject();
        }

        BattleSceneManager.Instance.setBattleScenePhase(BattleSceneManager.BattleScenePhase.NOT_BATTLE);
    }

    public static void unloadBattlePrefab()
    {
        if (BattleSceneManager.Instance != null)
        {
            GameObject.Destroy(BattleSceneManager.Instance.gameObject);
        }
    }

    public static void QuestInitialize(SceneModeContinuousBattle continuous_battle)
    {
        if (SceneGoesParam.HasInstance
            && SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build != null
            && SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild != null)
        {
            m_QuestBuild = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild;
        }

        loadBattlePrefab();

        m_AchievementTotalingInBattle = new AchievementTotalingInBattle();
        m_AchievementTotalingInBattle.initQuset();

        initBattleParam(continuous_battle);

        for (int idx = 0; idx < m_PlayerParty.getPartyMemberMaxCount(); idx++)
        {
            CharaOnce chara_once = m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
            if (chara_once != null)
            {
                chara_once.m_PartyCharaIndex = (GlobalDefine.PartyCharaIndex)idx;
            }
        }

        // プレイスコア
        m_PlayScoreInfo = new ServerDataDefine.PlayScoreInfo();
#if false //v5.1.0ではプレイスコアの機能を無効にする
        {
            // クエストスコア補正値
            int quest_base_score = 0;
            int quest_play_score_rate = 0;

            MasterDataQuest2 master_data_quest = MasterDataUtil.GetQuest2ParamFromID(continuous_battle.m_QuestMissionID);

            // プレイスコア計算対象クエストかどうかを調べる
            if (master_data_quest != null)
            {
                uint quest_score_fix_id = master_data_quest.quest_score_id;
                MasterDataRenewQuestScore master_data_quest_score = MasterFinder<MasterDataRenewQuestScore>.Instance.Find((int)quest_score_fix_id);
                if (master_data_quest_score != null)
                {
                    quest_base_score = master_data_quest_score.base_score;
                    quest_play_score_rate = master_data_quest_score.play_score_rate;
                }

                if (quest_play_score_rate > 0)
                {
                    if (master_data_quest.consume_type == 1)
                    {
                        if (master_data_quest.consume_value > 0)
                        {
                            m_PlayScoreInfo.setPlayScoreQuestFlag(true);
                        }
                    }
                }
            }

            // 中断復帰していない時だけ有効（なので中断復帰用のローカル保存もしていない）
            if (getRestoreData() == null
                && m_PlayScoreInfo.isPlayScoreQuest()
            )
            {
                // プレイスコア設定値取得
                MasterDataPlayScore[] master_data_play_score_array = MasterFinder<MasterDataPlayScore>.Instance.GetAll();

                if (master_data_play_score_array.IsNullOrEmpty() == false
                    && quest_base_score > 0
                    && quest_play_score_rate > 0
                )
                {
                    m_PlayScoreInfo.init(master_data_play_score_array, quest_base_score, quest_play_score_rate);
                }
            }
        }
#endif

        BattleParam.m_MasterDataCache.CachePlayerMasterData(BattleParam.m_PlayerParty);
        BattleSceneManager.Instance.setShowHandAreaAlways(true);

        BattleSceneManager.Instance.PRIVATE_FIELD.QuestInitialize();

        return;
    }

    public static bool BattleInitialize(BattleReq cBattleReq, SceneModeContinuousBattle continuous_battle)
    {
        initBattleParam(continuous_battle);

        m_BattleRequest = cBattleReq;
        if (cBattleReq != null)
        {
            m_QuestBuild = cBattleReq.m_QuestBuild;
        }
        else
        {
            m_QuestBuild = null;
        }

        m_EnemyToPlayerTarget = GlobalDefine.PartyCharaIndex.ERROR;
        return BattleSceneManager.Instance.PRIVATE_FIELD.BattleInitialize();
    }

    public static void setKensyoParam(
        SkillTurnCondition skill_turn_condition,
        float kobetsu_hp_enemy_attack_rate,
        bool is_kobetsu_hp_enemy_attack_all,
        bool is_kobetsu_hp_enemy_target_hate
    )
    {
        BattleSceneManager.Instance.PRIVATE_FIELD._setKensyoParam(skill_turn_condition, kobetsu_hp_enemy_attack_rate, is_kobetsu_hp_enemy_attack_all, is_kobetsu_hp_enemy_target_hate);
    }

    public static bool BattleStart()
    {
        BattleSceneManager.Instance.setBattleScenePhase(BattleSceneManager.BattleScenePhase.IN_BATTLE);
        return BattleSceneManager.Instance.PRIVATE_FIELD.BattleStart();
    }

    public static bool isInitilaizedBattle()
    {
        if (BattleSceneManager.Instance != null)
        {
            return true;
        }
        return false;
    }

    public static bool isActiveBattle()
    {
        if (BattleSceneManager.Instance != null && BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleActive)
        {
            return true;
        }
        return false;
    }

    public enum BattlePhase
    {
        NOT_BATTLE = 0, // 戦闘中ではない
        OPENING,        // 開始演出中.
        INPUT,          // 入力受付中（プレイヤーはカード操作していない）
        INPUT_HANDLING, // 入力受付中（プレイヤーがカード操作中）
        BATTLING,       // 戦闘中
        RETIRE,         // リタイア
    }
    public static BattlePhase getBattlePhase()
    {
        if (isActiveBattle())
        {
            BattleLogic.EBATTLE_STEP current_step = BattleLogicFSM.Instance.getCurrentStep();

            if (current_step < BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT)
            {
                return BattlePhase.OPENING;
            }

            if (current_step == BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT)
            {
                if (BattleSceneManager.Instance.PRIVATE_FIELD.isHandlingCard() == false)
                {
                    return BattlePhase.INPUT;
                }
                else
                {
                    return BattlePhase.INPUT_HANDLING;
                }
            }

            if (current_step == BattleLogic.EBATTLE_STEP.eBATTLE_STEP_DEAD_RETIRE)
            {
                return BattlePhase.RETIRE;
            }

            return BattlePhase.BATTLING;
        }

        return BattlePhase.NOT_BATTLE;
    }

    /// <summary>
    ///  カウントダウン中かどうかを調べる
    /// </summary>
    /// <returns></returns>
    public static bool isCountDown()
    {
        if (isActiveBattle())
        {
            if (BattleSceneManager.Instance.PRIVATE_FIELD.m_InputCountDownStart
                && BattleSceneManager.Instance.PRIVATE_FIELD.m_InputCountDown > 0.0f
            )
            {
                return true;
            }
        }

        return false;
    }


    public static void endBattleScene()
    {
        //playBGM(null);
        if (BattleSceneManager.Instance != null)
        {
            BattleSceneManager.Instance.PRIVATE_FIELD.endBattle();
        }
    }

    //-----	その他の関数・新設した関数
    public static bool isBoostField(int field_index)
    {
        bool ret_val = false;
        if (isActiveBattle()
            && BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField != null
            && field_index >= 0
            && field_index < BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField.Length
        )
        {
            ret_val = BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[field_index];
        }
        return ret_val;
    }


    public enum ContinueDialogResult
    {
        NONE,
        WAIT,   // コンティニューダイアログ表示中（ユーザーの入力待ち）
        GAMEOVER,   // ユーザーがコンティニューしないを選択した
        CONTINUE,   // ユーザーがコンティニューするを選択した
    }
    private static ContinueDialogResult m_ContinueDialogResult = ContinueDialogResult.NONE;

    /// <summary>
    /// コンティニューダイアログを表示
    /// </summary>
    /// <returns></returns>
    public static void showContinueDialog()
    {
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
            m_ContinueDialogResult = ContinueDialogResult.CONTINUE;
        }
        else
        {
            // 課金ダイアログの表示リクエスト
            LocalSaveContinue cContinueParam = LocalSaveManager.Instance.LoadFuncInGameContinue();
            bool is_auto_play = isUsedAutoPlay();
            InGameShopManager.Instance.RequestShopWorkContinue(cContinueParam.nContinueCt, is_auto_play);
            m_ContinueDialogResult = ContinueDialogResult.WAIT;
        }
    }

    public static void setContinueDialogResult(ContinueDialogResult continue_dialog_result)
    {
        m_ContinueDialogResult = continue_dialog_result;
    }

    /// <summary>
    /// コンティニューダイアログの結果受け取り
    /// </summary>
    /// <returns></returns>
    public static ContinueDialogResult getContinueDialogResult()
    {
        if (m_ContinueDialogResult == ContinueDialogResult.WAIT)
        {
            switch (InGameShopManager.Instance.m_ShopResult)
            {
                case InGameShopManager.INGAME_SHOP_RESAULT.RESULT_OK:

                    // 課金が成立
                    {
                        // 次のコンティニュー情報を構築
                        LocalSaveContinue cContinueParam = LocalSaveManager.Instance.LoadFuncInGameContinue();
                        cContinueParam.nContinueNext = cContinueParam.nContinueCt + 1;
                        LocalSaveManager.Instance.SaveFuncInGameContinue(cContinueParam);
                    }
                    m_ContinueDialogResult = ContinueDialogResult.CONTINUE;
                    break;

                case InGameShopManager.INGAME_SHOP_RESAULT.RESULT_NG:
                    // 課金せずに終了演出へ
                    m_ContinueDialogResult = ContinueDialogResult.GAMEOVER;
                    break;

                default:
                    break;
            }
        }

        return m_ContinueDialogResult;
    }


    /// <summary>
    /// 固定乱数値を取得。
    /// クエスト内で offset 値が同じなら同じ乱数値を返す。
    /// クエストが別の物になればその時点で返す乱数値が変わる。
    /// 使用目的：中断復帰時に乱数で値を決めるがセーブされない情報が、復帰のたびに値が異なるのを防ぐ
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static uint GetRandFix(int offset)
    {
        Rand rand = new Rand();
        rand.SetRandSeed((uint)(m_QuestRandSeed + offset));
        const int RAND_LOOP = 50;   // SetRandSeed() 直後は値が偏るっぽいので何回か余計にループさせる
        for (int idx = 0; idx < RAND_LOOP; idx++)
        {
            rand.GetRand();
        }

        return rand.GetRand();
    }

    /// <summary>
    /// 固定乱数値を取得。
    /// クエスト内で offset 値が同じなら同じ乱数値を返す。
    /// クエストが別の物になればその時点で返す乱数値が変わる。
    /// 使用目的：中断復帰時に乱数で値を決めるがセーブされない情報が、復帰のたびに値が異なるのを防ぐ
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static uint GetRandFix(int offset, uint min, uint max)
    {
        if (max <= min)
        {
            return min;
        }

        uint rand_value = GetRandFix(offset);
        return ((rand_value % (max - min)) + min);
    }
}
