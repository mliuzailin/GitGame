//	用語メモ：一般に「ノーマルスキル」と呼ばれているものはプログラム内では AvticeSkill, SkillActive 等と記述されています.
//	用語メモ：一般に「アクティブスキル」と呼ばれているものはプログラム内では LimitBreakSkill, SkillLimitBreak, LBS 等と記述されています.
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;

//----------------------------------------------------------------------------
/**
 *	バトル管理クラス
 *	こちらのクラスにはバトル中の処理ロジックを集める.
 *  こちらのクラスから描画クラスへ描画命令を発行するような感じor
 *  描画クラスがこちらのクラスを監視して適切に描画する？
 *  プレイヤーによるカード操作など分割が難しいものは後で考える.
 */
//----------------------------------------------------------------------------
public class BattleLogic : MonoBehaviour
{
    public BattleScene._BattleCardManager m_BattleCardManager = null;
    public SkillIconArea m_SkillIconArea = null;
    public CountDownArea m_CountDownArea = null;
    public CountFinishArea m_CountFinishArea = null;
    public BattleCaptionControl m_DispCaption = null;
    public TextCutinViewControl m_TextCutinArea = null;
    public BattleDispEnemy m_BattleDispEnemy = null;
    public BattleComboArea m_BattleComboArea = null;
    public BattleComboFinishArea m_BattleComboFinishArea = null;
    public BattlePlayerPartyViewControl m_BattlePlayerPartyViewControl = null;

    public void init(
        BattleScene._BattleCardManager battle_card_manager,
        SkillIconArea skill_icon_area,
        CountDownArea count_down_area,
        CountFinishArea count_finish_area,
        BattleCaptionControl caption_view_control,
        TextCutinViewControl text_cutin_area,
        BattleDispEnemy battle_disp_enemy,
        BattleComboArea combo_area,
        BattleComboFinishArea combo_finish_area,
        BattlePlayerPartyViewControl party_view
    )
    {
        m_BattleCardManager = battle_card_manager;
        m_SkillIconArea = skill_icon_area;
        m_CountDownArea = count_down_area;
        m_CountFinishArea = count_finish_area;
        m_DispCaption = caption_view_control;
        m_TextCutinArea = text_cutin_area;
        m_BattleDispEnemy = battle_disp_enemy;
        m_BattleComboArea = combo_area;
        m_BattleComboFinishArea = combo_finish_area;
        m_BattlePlayerPartyViewControl = party_view;

        m_BattleDispEnemy.setPlayerPartyViewContorl(party_view);
    }

    public const int BATTLE_DAMAGE_MIN = 1; //!< 戦闘ダメージの最低値
    public const int BATTLE_FIELD_MAX = 5;                                          //!< 戦闘フィールドのセル数
    public const int BATTLE_SKILL_FIELD_MAX = 25 + 5/*50*/;                                 //!< 同時可能攻撃最大数 // @Change Developer 2015/10/14 処理負荷対策。１パネ攻撃＊５人＝２５個と、回復パネルでの発動をバッファ（＋５）として、５０->３０に修正。
    public const int BATTLE_SKILL_TOTAL_MAX = BATTLE_SKILL_FIELD_MAX * BATTLE_FIELD_MAX;    //!< 同時可能攻撃最大数
    public const int BATTLE_SKILL_REACH_MAX = BATTLE_SKILL_TOTAL_MAX;                       //!< 同時可能リーチ最大数
    public const int BATTLE_FIELD_COST_MAX = 5;                                         //!< 戦闘フィールドの１セルあたりのコスト最大量


    public enum EBATTLE_STEP
    {
        eBATTLE_STEP_WAIT_UPDATE,

        eBATTLE_STEP_INIT,                                      //!< 戦闘処理ステップ：初期化：
        eBATTLE_STEP_INIT_FADEIN,                               //!< 戦闘処理ステップ：初期化：フェードイン

        eBATTLE_STEP_GAME_EVOL_START,                           //!< 戦闘処理ステップ：ゲーム：進化開始
        eBATTLE_STEP_GAME_EVOL_END,                             //!< 戦闘処理ステップ：ゲーム：進化終了

        eBATTLE_STEP_INIT_START,                                //!< 戦闘処理ステップ：初期化：スタート

        eBATTLE_STEP_GAME_SELECT_ACTION,                        //!< 戦闘処理ステップ：ゲーム：行動選択フェイズ
        eBATTLE_STEP_GAME_ENEMY_TURN_FIRST,                     //!< 戦闘処理ステップ：ゲーム：敵初回殴り
        eBATTLE_STEP_GAME_ENEMY_TURN_FIRST_END,                 //!< 戦闘処理ステップ：ゲーム：敵初回殴り終了

        eBATTLE_STEP_GAME_INITIATIVE_TITLE,                     //!< 戦闘処理ステップ：ゲーム：先制不意打ちのタイトル表示
        eBATTLE_STEP_GAME_INITIATIVE,                           //!< 戦闘処理ステップ：ゲーム：先制不意打ちによるターン変化
        eBATTLE_STEP_GAME_INITIATIVE_ATTACK,                    //!< 戦闘処理ステップ：ゲーム：先制不意打ちによる攻撃
        eBATTLE_STEP_GAME_INITIATIVE_END,                       //!< 戦闘処理ステップ：ゲーム：先制不意打ちの終了

        eBATTLE_STEP_GAME_PHASE_PLAYER_START,                   //!< 戦闘処理ステップ：ゲーム：プレイヤーフェイズ開始
        eBATTLE_STEP_GAME_THINK,                                //!< 戦闘処理ステップ：ゲーム：入力前考え中
        eBATTLE_STEP_GAME_INPUT,                                //!< 戦闘処理ステップ：ゲーム：入力実行

        eBATTLE_STEP_GAME_SKILL_SORT,                           //!< 戦闘処理ステップ：ゲーム：スキルソート（復活スキルまで）
        eBATTLE_STEP_GAME_COUNT_FADE_IN,                        //!< 戦闘処理ステップ：ゲーム：スキルカウントフェードイン
        eBATTLE_STEP_GAME_COUNT_FADE_OUT,                       //!< 戦闘処理ステップ：ゲーム：スキルカウントフェードアウト
        eBATTLE_STEP_GAME_ACTION_TITLE_IN,                      //!< 戦闘処理ステップ：ゲーム：入力結果アクションタイトル表示
        eBATTLE_STEP_GAME_ACTION,                               //!< 戦闘処理ステップ：ゲーム：入力結果アクション（復活スキルまで）
        eBATTLE_STEP_GAME_SKILL_SORT2,                               //!< 戦闘処理ステップ：ゲーム：スキルソート２（復活スキル後）
        eBATTLE_STEP_GAME_ACTION2,                               //!< 戦闘処理ステップ：ゲーム：入力結果アクション（復活スキル後）
        eBATTLE_STEP_GAME_ACTION_FINISH,                        //!< 戦闘処理ステップ：ゲーム：入力結果アクション終了
        eBATTLE_STEP_GAME_ACTION_TITLE_OUT,                     //!< 戦闘処理ステップ：ゲーム：入力結果アクションタイトル非表示

        eBATTLE_STEP_GAME_BOOST_SKILL_TITLE,                    //!< 戦闘処理ステップ：ゲーム：ブーストスキルタイトル表示
        eBATTLE_STEP_GAME_BOOST_SKILL,                          //!< 戦闘処理ステップ：ゲーム：ブーストスキルによるアクション
        eBATTLE_STEP_GAME_BOOST_SKILL_FINISH,                   //!< 戦闘処理ステップ：ゲーム：ブーストスキルによるアクション終了

        eBATTLE_STEP_GAME_ACTION_SKILL_TITLE,                   //!< 戦闘処理ステップ：ゲーム：リーダースキルタイトル表示
        eBATTLE_STEP_GAME_ACTION_SKILL,                         //!< 戦闘処理ステップ：ゲーム：スキルによる追撃
        eBATTLE_STEP_GAME_ACTION_SKILL_FINISH,                  //!< 戦闘処理ステップ：ゲーム：スキルによる追撃終了

        eBATTLE_STEP_GAME_PASSIVE_SKILL_TITLE,                  //!< 戦闘処理ステップ：ゲーム：パッシブスキルタイトル表示
        eBATTLE_STEP_GAME_PASSIVE_SKILL,                        //!< 戦闘処理ステップ：ゲーム：パッシブスキルによる追撃
        eBATTLE_STEP_GAME_PASSIVE_SKILL_FINISH,                 //!< 戦闘処理ステップ：ゲーム：パッシブスキルによる追撃終了

        eBATTLE_STEP_GAME_LINK_PASSIVE_TITLE,                   //!< 戦闘処理ステップ：ゲーム：リンクパッシブタイトル表示
        eBATTLE_STEP_GAME_LINK_PASSIVE,                         //!< 戦闘処理ステップ：ゲーム：リンクパッシブによる追撃
        eBATTLE_STEP_GAME_LINK_PASSIVE_FINISH,                  //!< 戦闘処理ステップ：ゲーム：リンクパッシブによる追撃終了

        eBATTLE_STEP_GAME_ACTION_END,                           //!< 戦闘処理ステップ：ゲーム：評価表示

        eBATTLE_STEP_GAME_PHASE_ENEMY_START,                    //!<　戦闘処理ステップ：ゲーム：敵フェイズ開始
        eBATTLE_STEP_GAME_AILMENTUPDATE,                        //!< 戦闘処理ステップ：ゲーム：敵状態異常処理
        eBATTLE_STEP_GAME_ENEMY_TURNUPDATE,                     //!< 戦闘処理ステップ：ゲーム：敵ターン更新
        eBATTLE_STEP_GAME_ENEMY_TURN,                           //!< 戦闘処理ステップ：ゲーム：敵ターン
        eBATTLE_STEP_GAME_PHASE_ENEMY_END,                      //!<　戦闘処理ステップ：ゲーム：敵フェイズ終了

        eBATTLE_STEP_GAME_COUNTER_PLAYER,                       //!< 戦闘処理ステップ：ゲーム：カウンター

        eBATTLE_STEP_GAME_RESULT,                               //!< 戦闘処理ステップ：ゲーム：１ターン終了
        eBATTLE_STEP_GAME_CLEAR,                                //!< 戦闘処理ステップ：ゲーム：ゲームクリア

        eBATTLE_STEP_DEAD_GAMEOVER,                             //!< 戦闘処理ステップ：死亡時：ゲームオーバー演出待ち
        eBATTLE_STEP_DEAD_GAMEOVER_WAIT,                        //!< 戦闘処理ステップ：死亡時：ゲームオーバー
        eBATTLE_STEP_DEAD_CONTINUE,                             //!< 戦闘処理ステップ：死亡時：バトルコンティニュー
        eBATTLE_STEP_DEAD_RETIRE,                               //!< 戦闘処理ステップ：死亡時：バトルリタイア

        eBATTLE_STEP_LIMITBREAK,                                //!< 戦闘処理ステップ：リミットブレイク：演出
        eBATTLE_STEP_LIMITBREAK_WAIT,                           //!< 戦闘処理ステップ：リミットブレイク：演出終了待ち
        eBATTLE_STEP_LIMITBREAK_END,                            //!< 戦闘処理ステップ：リミットブレイク：演出終了

        eBATTLE_STEP_GAME_ENEMY_REACTION_START,                 //!< 戦闘処理ステップ：ゲーム：敵リアクション開始
        eBATTLE_STEP_GAME_ENEMY_REACTION,                       //!< 戦闘処理ステップ：ゲーム：敵リアクション
        eBATTLE_STEP_GAME_ENEMY_REACTION_END,                   //!< 戦闘処理ステップ：ゲーム：敵リアクション終了

        eBATTLE_STEP_ERROR,                                     //!< 戦闘処理ステップ：エラー
        eBATTLE_STEP_MAX,                                       //!< 戦闘処理ステップ：
    };

    //----------------------------------------------------------------------------
    /*!
        @brief		場のブーストパネル表示処理タイプ定義
    */
    //----------------------------------------------------------------------------
    public enum EBOOST_CLEAR_MODE : int
    {
        eNORMAL,                                                //!< 通常処理(ランダム)
        eCLEAR,                                                 //!< 表示のクリア
        eALL,                                                   //!< すべてブースト化
    }


    public const int DELAYAILMENT_MAX = (int)GlobalDefine.PartyCharaIndex.MAX * 2;                              //!< ブースト状態異常予約バッファ最大数。１キャラ当りブースト最大２個所持*キャラ数分がMAX。

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public bool m_BattleActive = false;                                                     //!< 戦闘有無フラグ

    public enum InitialPhase
    {
        NOT_INIT,       // 初期化されていない
        CREATE_OBJ,     // CreateBattleGameObject() まで呼ばれた
        QUEST_INIT,     // QuestInitialize() まで呼ばれた
        BATTLE_INIT,    // BattleInitialize()まで呼ばれた
        BATTLE_START,   // BattleStart()まで呼ばれた
        WAIT_START1,    // 子コンポーネントのStart()終了待ち１
        WAIT_START2,    // 子コンポーネントのStart()終了待ち２
        WAIT_START3,    // 子コンポーネントのStart()終了待ち３
        IN_BATTLE,      // バトル中
    }
    public InitialPhase m_InitialPhase = InitialPhase.NOT_INIT;


    public int m_BattleTotalTurn = 0;                                                           //!< 戦闘ターン数

    public float m_InputCountDown = 0.0f;                                                           //!< 入力カウントダウン：カウントダウン現在値
    public bool m_InputCountDownStart = false;                                                        //!< 入力カウントダウン：カウントダウン開始フラグ

    public const string EFFECT_HANDLE_ENEMY_DEATH = "ENEMY_DEATH";                                          //!< ゲーム中オブジェクト：敵死亡エフェクト終了監視用
    public const string EFFECT_HANDLE_UNIT_DROP = "UNIT_DROP";                                          //!< ゲーム中オブジェクト：ユニットドロップエフェクト終了監視用
    public GameObject m_GameObjEffLimitBreak = null;

    public Animation m_TitleAnimInitiative = null;

    public GameObject m_SkillCountGameObjSkillRate = null;                                          //!< スキルカウント：オブジェクト：スキル効果倍率

    private uint m_SkillComboCountSound = 0;                                            //!< スキルコンボ：コンボカウント：サウンド再生用

    private BattleSkillFormingCheck m_BattleSkillFormingCheck = null;   // ノーマルスキル成立判定処理クラス
    private SkillRequestParam.SkillFilterType[] m_BattleSkillForimngCheckFilters;

    public BattleSkillReachInfo m_BattleSkillReachInfo = null;  // スキルリーチ情報

    public MasterDataSkillActive[] m_ActiveSkillOrder = null;                                                           //!< スキル発動判定：アクティブスキル発動順番
    private int m_AlwaysResurrectSkillSP = 0;               // 汎用復活スキルの消費ＳＰ量

    public bool m_ActiveSkillSuccess = false;                                                       //!< スキル発動チェックフラグ
    public bool m_FirstStep = true;                                                         //!< 初回更新フラグ
    public bool m_FirstStep_player = true;                                                          //!< 初回更新フラグ_プレイヤー
    public EBATTLE_UPDATE_STEP m_ProcStep = 0;                                                          //!< 戦闘突入後の更新状態：中断復帰時の敵の状態異常ターン経過の整合性を取る
    public EBATTLE_UPDATE_STEP m_ProcStep_player = 0;                                                           //!< 戦闘突入後の更新状態：中断復帰時の味方の状態異常ターン経過の整合性を取る
    public bool m_UpdatedStatusAilmentPlayer = false;                                                       //!< プレイヤーの状態異常更新済みフラグ

    public Rand m_EnemySkillHandCardRemake = new Rand();                                                    //!< 敵攻撃用乱数：手札変換
    public Rand m_ObjectCardRand = new Rand();                                                  //!< 属性カード関連：変動乱数

    // リミブレスキル連続発動対応
    public GlobalDefine.PartyCharaIndex m_NextLimitBreakSkillCaster = GlobalDefine.PartyCharaIndex.ERROR;
    public uint m_NextLimitBreakSkillFixID = 0; //続きのリミブレスキルのfix_id

    //----------------------------------------
    // 特殊フィールド関連
    //----------------------------------------
    public bool[] m_abBoostField = new bool[BATTLE_FIELD_MAX];
#if BUILD_TYPE_DEBUG
    public bool[] m_abBoostFieldDebugKeep = new bool[BATTLE_FIELD_MAX];
#endif

    public bool m_SkillCounter = false;     //!< カウンター

    public uint m_EnemyReactChkSkillID = 0;         //!< 敵リアクション：判定スキルID

    public bool m_PassiveSkillCounter = false;
    public bool m_LinkPassiveCounter = false;

    // @add Developer 2015/10/19 新スキル対応
    private bool m_bPutSuccess = false;     //!< 手札から場にカードを置いたかフラグ

    // @add Developer 2016/03/11 v330 同ターン中に、複数ダメージがある場合の対応(ダメージ描画をずらすため)
    private BattleSceneUtil.MultiInt[] m_afAilmentDamagePlayer = new BattleSceneUtil.MultiInt[(int)StatusAilmentChara.PoisonType.MAX];                                  //!< 状態異常ダメージ：毒
    private float[] m_afAilmentDamageEnemy = new float[InGameDefine.BATTLE_ENEMY_MAX * (int)StatusAilmentChara.PoisonType.MAX]; //!< 敵情報：状態異常ダメージ：毒

    // バトルの統計情報（スキルターン短縮の判定に使用する）
    private int[] m_Stats_NPanelSkillCount = new int[5];    //Ｎパネルスキルの発動回数
    private int[] m_Stats_NHandsCount = new int[6 * 5 * 5]; //Ｎハンズ数の発動回数
    private int m_Stats_FullFieldCount = 0; //場が「FULL」になった回数
    private int m_Stats_BoostSkillCount = 0;    //ブースト場でスキルが発動した回数.
    private bool m_Stats_LimitBreakWin = false;

    private BattleAutoPlayAI m_AutoPlayAI = null;

    #region ==== 検証用パラメータ（将来的には固定値になるはず） ====
    public static float m_KobetsuHP_EnemyAtkRate = 1.0f;    //個別ＨＰ時の敵の攻撃補正
    public static bool m_KobetsuHPEnemyAttackAll = false;   //敵のデフォルト攻撃が全体攻撃か単体攻撃か（全体攻撃が通常の動作）
    public static bool m_KobetsuHPEnemyTargetHate = true;   //敵のターゲット基準がヘイト管理かランダムか（ヘイトが通常の動作）
    private SkillTurnCondition m_SkillTurnCondition = new SkillTurnCondition(); // スキルターン短縮検証パラメータ
    public void _setKensyoParam(
        SkillTurnCondition skill_turn_condition,
        float kobetsu_hp_enemy_attack_rate,
        bool is_kobetsu_hp_enemy_attack_all,
        bool is_kobetsu_hp_enemy_target_hate)
    {
        m_SkillTurnCondition = skill_turn_condition;
        m_KobetsuHP_EnemyAtkRate = kobetsu_hp_enemy_attack_rate;
        m_KobetsuHPEnemyAttackAll = is_kobetsu_hp_enemy_attack_all;
        m_KobetsuHPEnemyTargetHate = is_kobetsu_hp_enemy_target_hate;
    }
    #endregion

    public BattleLogicAilment m_BattleLogicAilment = null;
    public BattleLogicEnemy m_BattleLogicEnemy = null;
    public BattleLogicSkill m_BattleLogicSkill = null;


    // 毎回 new するとガベコレが発生しやすいので一度だけ確保するもの
    private MasterDataDefineLabel.ElementType[] m_WorkHandAreaElements = null;

    private float m_GcTimer = 0.01f;    // 前回にGCを実行してからの経過時間
    private int m_GcWait = 0;   // GC実行待ち
                                /// <summary>
                                /// ガベージコレクションを実行
                                /// ガベコレをする場合は見た目やプレイヤーの操作やに影響のないタイミングで行うこと
                                /// </summary>
                                /// <param name="interval">GC実行間隔（秒）。前回GCが実行されてからこの時間以上経過いなければGCは実行されない</param>
    private void execGC(float interval, int wait_frame = 0)
    {
        if (m_GcTimer >= interval)
        {
            m_GcTimer = 0.0f;
            m_GcWait = wait_frame + 1;
            if (m_GcWait < 1)
            {
                m_GcWait = 1;
            }
        }
    }
    private void cancelGC()
    {
        m_GcWait = 0;
    }

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/


    /// <summary>
    /// カードが操作されているかどうか（カード操作中はメニューなどを開けないようにする必要がある）
    /// </summary>
    /// <returns></returns>
    public bool isHandlingCard()
    {
        bool is_touch_card = m_BattleCardManager.isCatchingCardByPlayer();
        bool ret_val = is_touch_card
            || m_InputCountDownStart
            || BattleTouchInput.Instance.isOverrideTouchMode();

        return ret_val;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：起動時呼出し
    */
    //----------------------------------------------------------------------------
    public/*protected override*/ void Awake()
    {
        m_BattleLogicAilment = new BattleLogicAilment();
        m_BattleLogicEnemy = new BattleLogicEnemy(this);
        m_BattleLogicSkill = new BattleLogicSkill(this);

        //base.Awake();

#if DEBUG_EXPORT_BATTLE_LOG && BUILD_TYPE_DEBUG
        DebugBattleLog.setEnable(true);
#endif
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
    */
    //----------------------------------------------------------------------------
    public/*protected override*/ void Start()
    {
        //base.Start();
    }


    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：破棄
    */
    //----------------------------------------------------------------------------
    public/*protected override*/ void OnDestroy()
    {


        //--------------------------------
        //	基底呼び出し
        //--------------------------------
        //base.OnDestroy();

        //--------------------------------
        //	ドロップユニット駒オブジェクト削除
        //--------------------------------
        m_BattleDispEnemy.hideDropObject();


        //--------------------------------
        //	敵オブジェクト破棄
        //--------------------------------
        if (BattleParam.m_EnemyParam != null)
        {
            for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
            {
                BattleParam.m_EnemyParam[i] = null;
            }
            BattleParam.m_EnemyParam = null;
        }

#if BUILD_TYPE_DEBUG
        DebugBattleLog.init();
#endif
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		スキルリストの作成
    */
    //----------------------------------------------------------------------------
    public void CreateSkillList()
    {
        //--------------------------------
        // 優先度込みで順番を決めておくことで、発動の際の判定等を簡略化
        //--------------------------------
        m_BattleCardManager.resetSkillCardInfo();

        // 回復は汎用属性のため無条件にTRUE
        m_BattleCardManager.setSkillCardInfo(MasterDataDefineLabel.ElementType.HEAL);

        // スキルを高コスト順に並び替え ＆ スキル発動に必要な属性を設定.
        MasterDataSkillActive[] order_array = new MasterDataSkillActive[BattleParam.m_MasterDataCache._getSkillActiveCount()];
        int order_index = 0;
        for (int cost = BATTLE_FIELD_COST_MAX; cost >= 0; cost--)
        {
            for (int idx = 0; idx < order_array.Length; idx++)
            {
                MasterDataSkillActive master_data_skill_active = BattleParam.m_MasterDataCache._getSkillActiveByIndex(idx);
                if (master_data_skill_active == null)
                {
                    continue;
                }

                int skill_cost = ((master_data_skill_active.cost1 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0)
                    + ((master_data_skill_active.cost2 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0)
                    + ((master_data_skill_active.cost3 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0)
                    + ((master_data_skill_active.cost4 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0)
                    + ((master_data_skill_active.cost5 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0);

                if (cost != skill_cost)
                {
                    continue;
                }

                // 場カードによるスキル発動判定はメインキャラのスキルのみなので、リンクキャラのみが持っているスキルは除外する.
                bool is_main_chara_skill = false;
                for (int player_idx = 0; player_idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); player_idx++)
                {
                    CharaOnce player_chara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)player_idx, CharaParty.CharaCondition.SKILL_ACTIVE);
                    if (player_chara != null && player_chara.m_CharaMasterDataParam != null)
                    {
                        if (player_chara.m_CharaMasterDataParam.skill_active0 == master_data_skill_active.fix_id
                        || player_chara.m_CharaMasterDataParam.skill_active1 == master_data_skill_active.fix_id)
                        {
                            is_main_chara_skill = true;
                            break;
                        }
                    }
                }

                if (is_main_chara_skill == false)
                {
                    // デフォルトのヒールスキルは除外しない.
                    if ((master_data_skill_active.cost1 == MasterDataDefineLabel.ElementType.HEAL)
                    && (master_data_skill_active.cost2 == MasterDataDefineLabel.ElementType.HEAL)
                    && (master_data_skill_active.cost3 == MasterDataDefineLabel.ElementType.NONE || master_data_skill_active.cost3 == MasterDataDefineLabel.ElementType.HEAL)
                    && (master_data_skill_active.cost4 == MasterDataDefineLabel.ElementType.NONE || master_data_skill_active.cost4 == MasterDataDefineLabel.ElementType.HEAL)
                    && (master_data_skill_active.cost5 == MasterDataDefineLabel.ElementType.NONE || master_data_skill_active.cost5 == MasterDataDefineLabel.ElementType.HEAL)
                    )
                    {
                        is_main_chara_skill = true;
                    }
                }

                if (is_main_chara_skill == false)
                {
                    continue;
                }

                m_BattleCardManager.setSkillCardInfo(master_data_skill_active.cost1);
                m_BattleCardManager.setSkillCardInfo(master_data_skill_active.cost2);
                m_BattleCardManager.setSkillCardInfo(master_data_skill_active.cost3);
                m_BattleCardManager.setSkillCardInfo(master_data_skill_active.cost4);
                m_BattleCardManager.setSkillCardInfo(master_data_skill_active.cost5);

                order_array[order_index] = master_data_skill_active;
                order_index++;
            }
        }

        m_ActiveSkillOrder = new MasterDataSkillActive[order_index];
        for (int idx = 0; idx < order_index; idx++)
        {
            m_ActiveSkillOrder[idx] = order_array[idx];
        }

        // ヒーロースキルを作成（リミットブレイクを間借り）
        {
            BattleParam.m_PlayerParty.m_BattleHero.createHeroSkill();
        }
    }

    /// <summary>
    /// 汎用復活スキルの最小消費ＳＰを計算
    /// </summary>
    private void calcAlwaysResurrectSkillSp()
    {
        m_AlwaysResurrectSkillSP = 999;

        int active_skill_count = BattleParam.m_MasterDataCache._getSkillActiveCount();
        for (int idx = 0; idx < active_skill_count; idx++)
        {
            MasterDataSkillActive master_data_skill_active = BattleParam.m_MasterDataCache._getSkillActiveByIndex(idx);
            if (master_data_skill_active != null)
            {
                if (master_data_skill_active.isAlwaysResurrectSkill())
                {
                    ResurrectInfo resurrect_info = master_data_skill_active.getResurrectInfo();
                    if (resurrect_info.m_AddSpUse < m_AlwaysResurrectSkillSP)
                    {
                        m_AlwaysResurrectSkillSP = resurrect_info.m_AddSpUse;
                    }
                    if (resurrect_info.m_FixSpUse < m_AlwaysResurrectSkillSP)
                    {
                        m_AlwaysResurrectSkillSP = resurrect_info.m_FixSpUse;
                    }
                }
            }
        }
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		ゲームオブジェクト生成
        @return		bool		[正常終了/エラー]
     */
    //----------------------------------------------------------------------------
    public bool CreateBattleGameObject()
    {
        DebugBattleLog.setFileName("BattleLog_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");

        // ステージ先頭のバトルなので手札をリセット.
        m_BattleCardManager.reset(false);

        if (m_InitialPhase != InitialPhase.NOT_INIT)
        {
            return false;
        }
        m_InitialPhase = InitialPhase.CREATE_OBJ;

        //--------------------------------
        // 発動条件を満たしたスキルの領域を確保
        //--------------------------------
        m_BattleSkillFormingCheck = new BattleSkillFormingCheck(BATTLE_SKILL_TOTAL_MAX);

        ClrSkillRequest();

        //--------------------------------
        // リーチ情報を準備
        //--------------------------------
        m_BattleSkillReachInfo = new BattleSkillReachInfo(m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax(), m_BattleCardManager.m_HandArea.getCardMaxCount());

        BattleParam.m_TargetEnemyCurrent = InGameDefine.SELECT_NONE;

        //----------------------------------------
        // 敵メッシュ。
        //----------------------------------------
        m_BattleDispEnemy.initObject();

        return true;
    }

    public bool QuestInitialize()
    {
        if (m_InitialPhase != InitialPhase.CREATE_OBJ
            && m_InitialPhase != InitialPhase.QUEST_INIT
        )
        {
            return false;
        }
        m_InitialPhase = InitialPhase.QUEST_INIT;

        CreateSkillList();
        calcAlwaysResurrectSkillSp();

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief			戦闘操作：戦闘前初期化
        @param[in]		BattleReq		(cBattleReq)		戦闘情報
        @return			bool			[正常終了/エラー]
     */
    //----------------------------------------------------------------------------
    public bool BattleInitialize()
    {
        if (m_InitialPhase != InitialPhase.QUEST_INIT)
        {
            return false;
        }
        m_InitialPhase = InitialPhase.BATTLE_INIT;

        //--------------------------------
        // 既に起動中なら発行受理しない
        //--------------------------------
        if (m_BattleActive == true)
        {
            Debug.LogError("BattleInitialize Failed!! Battle Active Now");
            return false;
        }

        initValue();
        m_SkillIconArea.setSkillInfos(null, 0); // スキルアイコンをクリア
        setPartyHandsInfo(null, 0, SkillRequestParam.SkillFilterType.ALL);

        //--------------------------------
        // 敵インスタンスを破棄
        //--------------------------------
        m_BattleDispEnemy.destroyEnemy();

        //--------------------------------
        //	乱数シード更新
        //--------------------------------
        m_EnemySkillHandCardRemake.SetRandSeed(BattleParam.GetRandFix(888 + BattleParam.BattleRound * 1000 + m_BattleTotalTurn));
        m_ObjectCardRand.SetRandSeed(BattleParam.GetRandFix(99999 + BattleParam.BattleRound * 1000 + m_BattleTotalTurn));


        //--------------------------------
        //	累計ターン数の初期化
        //--------------------------------
        m_BattleTotalTurn = 0;


        //--------------------------------
        // 中断復帰データがあればデータを復帰
        //--------------------------------
        RestoreBattle restore_battle = BattleParam.getRestoreData();
        if (restore_battle != null)
        {
            //----------------------------------------
            //	経過ターン数復帰
            //----------------------------------------
            m_BattleTotalTurn = restore_battle.m_BattleTotalTurn;
            restore_battle.m_BattleTotalTurn = 0;

            //--------------------------------
            // 敵リアクション：判定スキルIDの復帰
            //--------------------------------
            m_EnemyReactChkSkillID = restore_battle.m_EnemyReactChkSkillID;
            restore_battle.m_EnemyReactChkSkillID = 0;

            m_NextLimitBreakSkillCaster = restore_battle.m_NextLimitBreakSkillCaster;
            restore_battle.m_NextLimitBreakSkillCaster = GlobalDefine.PartyCharaIndex.ERROR;

            m_NextLimitBreakSkillFixID = restore_battle.m_NextLimitBreakSkillFixID;
            restore_battle.m_NextLimitBreakSkillFixID = 0;
        }

        m_BattleLogicEnemy.init(BattleParam.m_BattleRequest.m_QuestBuildBattle.enemy_list, BattleParam.m_BattleRequest.m_QuestBuildBattle.drop_list);

        //--------------------------------
        // 敵が一人でもいれば戦闘成立。
        // 敵が一人もいないなら戦闘不成立
        //--------------------------------
        if (BattleParam.m_EnemyParam.Length == 0)
        {
            Debug.LogError("Enemy Total Num == 0!!");
            return false;
        }

        // マスターデータの整合性チェック
        for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
        {
            BattleEnemy enemy = BattleParam.m_EnemyParam[idx];
            if (enemy == null)
            {
                return false;
            }

            if (enemy.getMasterDataParamChara() == null)
            {
                return false;
            }
        }

        //--------------------------------
        // 敵インスタンスの基準位置を算出
        //--------------------------------
        m_BattleDispEnemy.instanceObject(BattleParam.m_EnemyParam);

        return true;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘操作：戦闘開始
     */
    //----------------------------------------------------------------------------
    public bool BattleStart()
    {
        if (m_InitialPhase != InitialPhase.BATTLE_INIT)
        {
            return false;
        }
        m_InitialPhase = InitialPhase.BATTLE_START;

        //--------------------------------
        // 戦闘が成立するかチェック
        //--------------------------------
        if (BattleParam.m_BattleRequest == null)
        {
            Debug.LogError("BattleRequest Is Null!!");
            return false;
        }

        if (BattleParam.m_EnemyParam.Length <= 0)
        {
            Debug.LogError("BattleRequest Enemy Zero!!");
            return false;
        }

        // バトルで使用するユニット画像をキャッシュ
        BattleUnitTextureCache.Instance.clearCache();
        BattleUnitTextureCache.cacheBattleUnitTexture();

        // 未使用なリソースを削除してメモリを整理
        Resources.UnloadUnusedAssets();
        execGC(0.0f);

        m_BattleActive = true;

        // バトルの統計情報をクリア（スキルターン短縮の判定に使用する）
        for (int idx = 0; idx < m_Stats_NPanelSkillCount.Length; idx++)
        {
            m_Stats_NPanelSkillCount[idx] = 0;
        }
        for (int idx = 0; idx < m_Stats_NHandsCount.Length; idx++)
        {
            m_Stats_NHandsCount[idx] = 0;
        }
        m_Stats_FullFieldCount = 0;
        m_Stats_BoostSkillCount = 0;

        m_DispCaption.updateAchieve();

#if BUILD_TYPE_DEBUG
        DebugBattleLog.newLine();
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ ユーザーデータ ]]]]]");
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "ServerName : " + ServerDataUtil.GetServerName());
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "productName: " + Application.productName);
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "Bundle Id  : " + Application.identifier);
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "");

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ クエストデータ ]]]]]");
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
        if (UserDataAdmin.HasInstance)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "クエストＩＤ　: " + UserDataAdmin.Instance.m_StructQuest.quest_id.ToString());
            MasterDataQuest2 quest_data = MasterDataUtil.GetQuest2ParamFromID(BattleParam.m_QuestMissionID);
            if (quest_data != null)
            {
                uint enemy_group_id = 0;
                if (BattleParam.BattleRound + 1 == quest_data.battle_count)
                {
                    enemy_group_id = quest_data.boss_group_id;
                }
                else
                {
                    uint[] enemy_group_ids =
                    {
                        quest_data.enemy_group_id_1,
                        quest_data.enemy_group_id_2,
                        quest_data.enemy_group_id_3,
                        quest_data.enemy_group_id_4,
                        quest_data.enemy_group_id_5,
                        quest_data.enemy_group_id_6,
                        quest_data.enemy_group_id_7,
                        quest_data.enemy_group_id_8,
                        quest_data.enemy_group_id_9,
                        quest_data.enemy_group_id_10,
                        quest_data.enemy_group_id_11,
                        quest_data.enemy_group_id_12,
                        quest_data.enemy_group_id_13,
                        quest_data.enemy_group_id_14,
                        quest_data.enemy_group_id_15,
                        quest_data.enemy_group_id_16,
                        quest_data.enemy_group_id_17,
                        quest_data.enemy_group_id_18,
                        quest_data.enemy_group_id_19,
                    };
                    enemy_group_id = enemy_group_ids[BattleParam.BattleRound];
                }

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "クエスト名　　: " + quest_data.quest_name);
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "エリアＩＤ　　: " + quest_data.area_id.ToString());
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "バトル　　　　: " + (BattleParam.BattleRound + 1).ToString() + "/" + quest_data.battle_count);
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵グループＩＤ: " + enemy_group_id.ToString());
            }
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ バトル開始 ]]]]]");
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
#endif

        return true;
    }

    /// <summary>
    /// メンバー変数を初期化
    /// </summary>
    private void initValue()
    {
        BattleParam.m_TargetEnemyCurrent = InGameDefine.SELECT_NONE;
        BattleParam.m_TargetEnemyWindow = InGameDefine.SELECT_NONE;

        m_BattleTotalTurn = 0;
        m_InputCountDown = 0.0f;
        m_InputCountDownStart = false;
        m_GameObjEffLimitBreak = null;

        //		m_BattleEnemyAnim = null;
        m_TitleAnimInitiative = null;

        m_SkillCountGameObjSkillRate = null;

        m_SkillComboCountSound = 0;

        m_BattleSkillFormingCheck.reset();

        m_ActiveSkillOrder = null;

        m_ActiveSkillSuccess = false;
        m_FirstStep = true;
        m_FirstStep_player = true;
        m_ProcStep = 0;
        m_ProcStep_player = 0;
        m_UpdatedStatusAilmentPlayer = false;

        //		m_EnemySkillHandCardRemake = new Rand();
        //		m_ObjectCardRand = new Rand();

        m_abBoostField = new bool[BATTLE_FIELD_MAX];

        m_SkillCounter = false;

        m_EnemyReactChkSkillID = 0;

        m_PassiveSkillCounter = false;
        m_LinkPassiveCounter = false;

        m_bPutSuccess = false;


        m_afAilmentDamagePlayer = new BattleSceneUtil.MultiInt[(int)StatusAilmentChara.PoisonType.MAX];
        for (int idx = 0; idx < m_afAilmentDamagePlayer.Length; idx++)
        {
            m_afAilmentDamagePlayer[idx] = new BattleSceneUtil.MultiInt(0);
        }

        m_afAilmentDamageEnemy = new float[InGameDefine.BATTLE_ENEMY_MAX * (int)StatusAilmentChara.PoisonType.MAX];

        //		m_IsUpdating = false;

        m_Stats_NPanelSkillCount = new int[5];
        m_Stats_NHandsCount = new int[6 * 5 * 5];
        m_Stats_FullFieldCount = 0;
        m_Stats_BoostSkillCount = 0;

        m_AutoPlayAI = null;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：更新処理	※定期処理
    */
    //----------------------------------------------------------------------------
    public void Update()
    {
        if (m_GcWait > 0)
        {
            m_GcWait--;
            if (m_GcWait <= 0)
            {
                m_GcTimer = 0.0f;
                m_GcWait = 0;
                System.GC.Collect();
            }
        }
        m_GcTimer += Time.deltaTime;

        //--------------------------------
        // 起動していなければ終了
        //--------------------------------
        if (m_BattleActive == false)
        {
            return;
        }

        if (m_InitialPhase != InitialPhase.IN_BATTLE)
        {
            switch (m_InitialPhase)
            {
                case InitialPhase.BATTLE_START:
                    m_InitialPhase = InitialPhase.WAIT_START1;
                    break;

                case InitialPhase.WAIT_START1:
                    m_InitialPhase = InitialPhase.WAIT_START2;
                    break;

                case InitialPhase.WAIT_START2:
                    m_InitialPhase = InitialPhase.WAIT_START3;
                    break;

                case InitialPhase.WAIT_START3:
                    m_InitialPhase = InitialPhase.IN_BATTLE;
                    break;
            }
            return;
        }

        m_BattleLogicEnemy.update(Time.deltaTime);

        switch (BattleLogicFSM.Instance.getCurrentStep())
        {
            case EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT:
                MonoUpdate_OnGameInput();
                break;
        }

        //----------------------------------------
        // キャプションコントロール
        //----------------------------------------
        if (m_DispCaption.isUpdating())
        {
            return;
        }

        bool is_show_battle_achieve = BattleParam.IsKobetsuHP;
        if (is_show_battle_achieve)
        {
            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
            if (cOption != null)
            {
                is_show_battle_achieve = ((LocalSaveDefine.OptionBattleAchieve)cOption.m_OptionBattleAchieve == LocalSaveDefine.OptionBattleAchieve.ON);
            }
        }
        m_DispCaption.setShowAchieve(is_show_battle_achieve);

        {
            //----------------------------------------
            // UI更新
            //----------------------------------------
            UIUpdate();
        }
    }

    /// <summary>
    /// Update()が動き始めるまではここで待つ.
    /// PlayMaker で行う処理はもともとは Update() 関数内で実行されていたものなので、Update()が動き始める前に PlayMaker で処理が流れないようにここで待つ.
    /// </summary>
    void OnWaitUpdate()
    {
        if (m_InitialPhase == InitialPhase.IN_BATTLE)
        {
            if (BattleUnitTextureCache.Instance.isLoading() == false)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_INIT);
            }
            return;
        }
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：初期化：
        @return		EBATTLE_STEP		[次の処理ステップ]
    */
    //----------------------------------------------------------------------------
    void OnBattleInit()
    {

        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // リーチ線のクリア用に最初に呼出し
            //----------------------------------------
            ClrSkillRequest();
            initSkillReachInfo();

            // デバッグログ：プレイヤーパーティ
            DebugBattleLog.outputPlayerParty(BattleParam.m_PlayerParty);

            // デバッグログ：敵パーティ
            DebugBattleLog.outputEnemyParty(BattleParam.m_EnemyParam);

#if BUILD_TYPE_DEBUG
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ パネル出現率 ]]]]]");
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");

                int[] element_appear_rates = new int[(int)MasterDataDefineLabel.ElementType.MAX];
                int total_rate = _getElementAppearRates(ref element_appear_rates);
                if (total_rate > 0)
                {
                    uint unAreaID = BattleParam.m_QuestAreaID;
                    MasterDataArea cMasterDataArea = BattleParam.m_MasterDataCache.useAreaParam(unAreaID);
                    int[] area_costs = {
                        0,
                        cMasterDataArea.cost0,
                        cMasterDataArea.cost1,
                        cMasterDataArea.cost2,
                        cMasterDataArea.cost4,  //←MasterDataArea の cost の並び順は MasterDataDefineLabel.ElementType とは異なる
                        cMasterDataArea.cost5,  //←MasterDataArea の cost の並び順は MasterDataDefineLabel.ElementType とは異なる
                        cMasterDataArea.cost3,  //←MasterDataArea の cost の並び順は MasterDataDefineLabel.ElementType とは異なる
                        cMasterDataArea.cost6,
                    };

                    float div_total_rate = 100.0f / total_rate;

                    for (int idx = (int)MasterDataDefineLabel.ElementType.NAUGHT; idx < (int)MasterDataDefineLabel.ElementType.MAX; idx++)
                    {
                        MasterDataDefineLabel.ElementType element_type = (MasterDataDefineLabel.ElementType)idx;
                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + String.Format("   {0}:{1}(基本値)×{2}%(スキル)={3} ({4:F2}%)",
                            element_type.ToString(),
                            area_costs[idx],
                            (int)(InGameUtilBattle.PassiveChkHandCardChanceUPElement(element_type) * 100.0f),
                            element_appear_rates[(int)element_type],
                            (element_appear_rates[(int)element_type] * div_total_rate)
                            ));

                    }
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + String.Format("   合計:{0}", total_rate));
                }
                else
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "    エラー");
                }

                DebugBattleLog.setValue("PnlAprNAUGHT", 0);
                DebugBattleLog.setValue("PnlAprFIRE", 0);
                DebugBattleLog.setValue("PnlAprWATER", 0);
                DebugBattleLog.setValue("PnlAprLIGHT", 0);
                DebugBattleLog.setValue("PnlAprDARK", 0);
                DebugBattleLog.setValue("PnlAprWIND", 0);
                DebugBattleLog.setValue("PnlAprHEAL", 0);

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
            }
#endif

            //----------------------------------------
            // フィールドをブースト無しの表示にリセット
            //----------------------------------------
            SwitchBoostField(EBOOST_CLEAR_MODE.eCLEAR);

            m_DispCaption.hideCaption();

            m_AutoPlayAI = new BattleAutoPlayAI();
        }


        //----------------------------------------
        // 次のステップへ移行
        //----------------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_INIT_FADEIN);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：初期化：フェードイン
        @return		EBATTLE_STEP		[次の処理ステップ]
    */
    //----------------------------------------------------------------------------
    void OnInitFadeIn()
    {

        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // 手札と場を再構築
            //----------------------------------------
            BattleClearField();
            BattleClearHand(false);

            RestoreBattle restore_battle = BattleParam.getRestoreData();
            if (restore_battle != null)
            {

                //----------------------------------------
                // 手札属性変更
                //----------------------------------------
                MasterDataDefineLabel.ElementType[] handCardElem = restore_battle.m_BattleHandCardElem;
                MasterDataDefineLabel.ElementType[] nextCardElem = restore_battle.m_BattleNextCardElem;
                if (handCardElem != null)
                {
                    m_BattleCardManager.m_HandArea.reset();
                    m_BattleCardManager.m_NextArea.reset();
                    for (int n = 0; n < m_BattleCardManager.m_HandArea.getCardMaxCount(); n++)
                    {
                        if (handCardElem[n] != MasterDataDefineLabel.ElementType.NONE)
                        {
                            m_BattleCardManager.addNewCardInHandArea(n, handCardElem[n]);
                        }

                        if (nextCardElem[n] != MasterDataDefineLabel.ElementType.NONE)
                        {
                            m_BattleCardManager.addNewCardInHandArea(n, nextCardElem[n]);
                        }
                    }
                }
                restore_battle.m_BattleHandCardElem = null;
                restore_battle.m_BattleNextCardElem = null;

                //----------------------------------------
                // 場の属性変更
                // @add Developer 2015/10/26 v300 新スキル対応
                //----------------------------------------
                MasterDataDefineLabel.ElementType[] anBattleFieldPanelElem = restore_battle.m_BattleFieldPanelElem;
                if (anBattleFieldPanelElem != null)
                {
                    int field_count = m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax();
                    int card_count = m_BattleCardManager.m_FieldAreas.getFieldArea(0).getCardMaxCount();

                    if (field_count * card_count == anBattleFieldPanelElem.Length)
                    {
                        m_BattleCardManager.m_FieldAreas.reset();

                        for (int nFieldNum = 0; nFieldNum < field_count; nFieldNum++)
                        {
                            for (int nPanelNum = 0; nPanelNum < card_count; nPanelNum++)
                            {
                                int idx = nFieldNum * card_count + nPanelNum;
                                if (anBattleFieldPanelElem[idx] != MasterDataDefineLabel.ElementType.NONE)
                                {
                                    BattleScene.BattleCard battle_card = m_BattleCardManager.getUnusedCard();
                                    if (battle_card == null)
                                    {
#if UNITY_EDITOR
                                        Debug.LogError("FreeCard Is None!");
#endif // #if UNITY_EDITOR
                                        continue;
                                    }

                                    // カード設定変更＆デザイン変更
                                    battle_card.setElementType(anBattleFieldPanelElem[idx], BattleScene.BattleCard.ChangeCause.NONE);
                                    m_BattleCardManager.m_FieldAreas.addCard(nFieldNum, battle_card);
                                }
                            }
                        }
                    }

                    restore_battle.m_BattleFieldPanelElem = null;
                }

                //----------------------------------------
                // 初回ターンの更新処理
                //----------------------------------------
                m_ProcStep = (EBATTLE_UPDATE_STEP)restore_battle.m_ProcStep;
                m_ProcStep_player = (EBATTLE_UPDATE_STEP)restore_battle.m_ProcStep_player;
                restore_battle.m_ProcStep = (int)EBATTLE_UPDATE_STEP.eSTART;
                restore_battle.m_ProcStep_player = (int)EBATTLE_UPDATE_STEP.eSTART;

                //----------------------------------------
                // 実績集計情報を更新
                //----------------------------------------
                if (restore_battle.m_AchievementTotaling != null)
                {
                    BattleParam.m_AchievementTotalingInBattle = restore_battle.m_AchievementTotaling;
                    restore_battle.m_AchievementTotaling = null;
                }

                //----------------------------------------
                // プレイヤー状態異常更新済みフラグ
                //----------------------------------------
                m_UpdatedStatusAilmentPlayer = restore_battle.m_UpdateStatusAilmentPlayer;
                restore_battle.m_UpdateStatusAilmentPlayer = false;
            }

            if (BattleParam.IsKobetsuHP
                && restore_battle == null
            )
            {
                //スキルターン更新（中断復帰時はすでに加算後の数値が入っているのでここでは加算しない）
                {
                    int skill_turn = 0;
                    if (BattleParam.m_BattleRequest.isChain == false)
                    {
                        skill_turn += m_SkillTurnCondition.m_BattleStart;
                    }
                    else
                    {
                        skill_turn += m_SkillTurnCondition.m_EnemyEvol;
                    }

                    for (int i = 0; i < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); i++)
                    {
                        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)i, CharaParty.CharaCondition.SKILL_TURN2);
                        if (chara != null)
                        {
                            chara.AddCharaLimitBreak(skill_turn);
                        }
                    }
                }

                //ヘイト初期化（中断復帰時はすでに値が入っているので初期化しない）
                BattleParam.m_PlayerParty.initHate(BattleParam.m_BattleRequest.isChain);
            }

            BattleSceneManager.Instance.setTutorialPhase(BattleTutorialManager.TutorialBattlePhase.CARD_INIT);
        }

        //--------------------------------
        // UIが表示されきってない場合は待つ
        //--------------------------------
        if (BattleSceneManager.Instance.isFadeing())
        {
            return;
        }

        //--------------------------------
        // 次のステップへ移行
        //--------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_EVOL_START);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：進化演出：開始
    */
    //----------------------------------------------------------------------------
    void OnGameEvolStart()
    {
        //--------------------------------
        //	進化演出不要
        //--------------------------------
        if (BattleParam.m_BattleRequest.isChain == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_EVOL_END);
            return;
        }

        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //--------------------------------
            // 進化演出2
            //--------------------------------
            m_BattleDispEnemy.showEvolve2(BattleParam.m_BattleRequest.m_QuestBuildBattle.evol_direction, m_TextCutinArea);

            return;
        }

        //--------------------------------
        // 進化演出終了待ち
        //--------------------------------
        if (m_BattleDispEnemy.isShowEvolve())
        {
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_EVOL_END);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：進化演出：終了
    */
    //----------------------------------------------------------------------------
    void OnGameEvolEnd()
    {

        //--------------------------------
        //	進化演出不要
        //--------------------------------
        if (BattleParam.m_BattleRequest.isChain == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_INIT_START);
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_INIT_START);
        return;
    }


    //------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：初期化：スタート
        @return		EBATTLE_STEP		[次の処理ステップ]
    */
    //------------------------------------------------------------------------
    void OnInitStart()
    {

        //--------------------------------
        // 手札が配られるモーションの再生リクエスト
        //--------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            for (int idx = 0; idx < m_BattleCardManager.m_HandArea.getCardMaxCount(); idx++)
            {
                m_BattleCardManager.m_HandArea.getCard(idx).m_ViewControl.m_AnimWait = 0.3f + idx * 0.03f;
            }

            //--------------------------------
            // カード配りボイス再生
            //--------------------------------
            SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_HANDCARD_SET);

            return;
        }


        //--------------------------------
        // 再生完了待ち
        //--------------------------------
        if (m_BattleCardManager.m_HandArea.getCard(m_BattleCardManager.m_HandArea.getCardMaxCount() - 1).m_ViewControl.m_AnimWait > 0.0f)
        {
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_SELECT_ACTION);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：行動選択
    */
    //----------------------------------------------------------------------------
    void OnGameSelectAction()
    {

        //--------------------------------
        //	指定のフェイズに行動する敵がいた場合、行動テーブル処理フェイズへ移動
        //	※初回行動が設定されていた場合、オフセット設定を無視し、
        //	※行動テーブルに記入された行動を優先する。（平位確認済み）
        //--------------------------------
        if (m_BattleLogicEnemy.EnemyActionTableCheckFirstAttack() == true
        && m_BattleTotalTurn == 0
        && m_ProcStep_player == EBATTLE_UPDATE_STEP.eSTART)
        {

            //--------------------------------
            //	何もなかったことにする
            //--------------------------------
            BattleParam.m_BattleRequest.m_BattleTurnOffset = 0;


            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURN_FIRST);
            return;
        }


        //--------------------------------
        //	オフセット設定による先攻後攻処理
        //--------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE_TITLE);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：敵初回殴り
    */
    //----------------------------------------------------------------------------
    void OnGameEnemyTurnFirst()
    {

        //----------------------------------------
        // カウンター処理から、戻ってきた場合
        // @add Developer 2015/12/16 v320
        //----------------------------------------
        if (BattleLogicFSM.Instance.getPrevStep() == EBATTLE_STEP.eBATTLE_STEP_GAME_COUNTER_PLAYER)
        {
            // 初回処理防止
            BattleLogicFSM.Instance.isChangeStepTriger(true);
        }

        //----------------------------------------
        //	初回処理
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {

            //----------------------------------------
            // 敵攻撃関連処理：事前処理
            //----------------------------------------
            m_BattleLogicEnemy.EnemyAttackAllPreProc();

        }

        //----------------------------------------
        // カウンター処理が必要であればカウンター処理へ移行
        // @add Developer 2015/12/16 v320
        //----------------------------------------
        if (m_SkillCounter == true)
        {
            //----------------------------------------
            //	動きが速すぎるので、適当に待つ
            //----------------------------------------
            if (m_BattleLogicEnemy.isWaitingActionTimer())
            {
                return;
            }

            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_COUNTER_PLAYER);
            return;
        }

        bool updateResult;

        //----------------------------------------
        //	敵攻撃関連処理：メイン処理(カウンターあり)
        //----------------------------------------
        updateResult = m_BattleLogicEnemy.EnemyAttackAllMainProc(true, 0);
        if (updateResult == false)
        {
            return;
        }


        //----------------------------------------
        //	敵攻撃関連処理：後処理
        //----------------------------------------
        updateResult = m_BattleLogicEnemy.EnemyAttackAllPostProc();
        if (updateResult == false)
        {
            return;
        }


        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURN_FIRST_END);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		先制不意打ちのタイトル表示
        @return		EBATTLE_STEP		[次の処理ステップ]
    */
    //----------------------------------------------------------------------------
    void OnGameTitleInitiative()
    {

        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {

            //--------------------------------
            // 派生バトルでは演出をカット
            //--------------------------------
            if (BattleParam.m_BattleRequest.isChain == true)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE);
                return;
            }


            //--------------------------------
            // オフセット設定がなければ次へ
            //--------------------------------
            int offset = BattleParam.m_BattleRequest.m_BattleTurnOffset;
            if (offset == 0)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE);
                return;
            }

            //--------------------------------
            // 先制、不意打ち発生
            //--------------------------------
            if (offset < 0)
            {
                // 不意打ち
                m_TextCutinArea.startAnim(TextCutinViewControl.TitleType.BACK_ATTACK);
                SoundUtil.PlaySE(SEID.SE_BATTLE_ATTACK_BACK);
                SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_BACKATTACK);
            }
            else
            {
                // 先制
                m_TextCutinArea.startAnim(TextCutinViewControl.TitleType.FIRST_ATTACK);
                SoundUtil.PlaySE(SEID.SE_BATTLE_ATTACK_FIRST);
                SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_FIRSTATTACK);
            }
        }

        // アニメーション終了待ち
        if (m_TextCutinArea.isPlaying())
        {
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：先制不意打ちによるターンの変化
        @return		EBATTLE_STEP		[次の処理ステップ]
    */
    //----------------------------------------------------------------------------
    void OnGameInitiative()
    {

        int turn;


        //--------------------------------
        // 初回更新
        //--------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {

            //--------------------------------
            // 先制、不意打ちによるターンの変化処理
            //--------------------------------
            for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
            {


                if (BattleParam.m_EnemyParam[i] == null)
                {
                    continue;
                }

                turn = (BattleParam.m_EnemyParam[i].m_EnemyTurn + BattleParam.m_BattleRequest.m_BattleTurnOffset);
                if (turn <= 0)
                {
                    turn = 0;
                }

                //--------------------------------
                // ターン数の文字書き換え
                //--------------------------------
                BattleParam.m_EnemyParam[i].m_EnemyTurn = turn;

                if (BattleParam.m_BattleRequest.m_BattleTurnOffset != 0)
                {
                    m_BattleDispEnemy.playTurnChangeEffect(i);
                }
            }

        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE_ATTACK);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：Caption:PlayerPhase
        @return		EBATTLE_STEP		[次の処理ステップ]
    */
    //----------------------------------------------------------------------------
    void OnPhasePlayerStart()
    {

        int dummy;
        checkStats(false, out dummy);

        //----------------------------------------
        //	キャプション表示
        //----------------------------------------
        m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.PLAYER_PHASE);


        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_THINK);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：入力前考え中
        @return		EBATTLE_STEP		[次の処理ステップ]
    */
    //----------------------------------------------------------------------------
    void OnGameThink()
    {

        // 初回処理
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            m_BattleLogicSkill.clearUsingElementInfo();

            //----------------------------------------
            //	ブーストフィールドを再配置
            //----------------------------------------
            SwitchBoostField(EBOOST_CLEAR_MODE.eNORMAL);

            //----------------------------------------
            //	カウントダウン時間設定
            //----------------------------------------
            m_InputCountDown = InGameDefine.COUNTDOWN_TIME_DEFAULT;

            //----------------------------------------
            //	プレイヤー側状態異常効果反映
            //----------------------------------------
            if (m_UpdatedStatusAilmentPlayer == false)
            {


                //----------------------------------------
                //	リジェネ
                //----------------------------------------
                StatusAilmentChara ailment = BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX);
                bool badCondition = ailment.ChkStatusAilmentCondition(StatusAilment.GoodOrBad.BAD);
                if (badCondition == false)
                {

                    BattleSceneUtil.MultiInt healVal = ailment.GetHealValue();
                    if (healVal.getValue(GlobalDefine.PartyCharaIndex.MAX) != 0)
                    {
                        BattleParam.m_PlayerParty.RecoveryHP(healVal, true, true);
                    }

                }

                //----------------------------------------
                //	毒ダメージ：取得
                //	@change Developer 2016/03/11 v330 毒複数対応
                //----------------------------------------
                for (int num = 0; num < (int)StatusAilmentChara.PoisonType.MAX; ++num)
                {
                    m_afAilmentDamagePlayer[num] = new BattleSceneUtil.MultiInt();
                    for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.ALIVE);
                        if (chara_once != null)
                        {
                            m_afAilmentDamagePlayer[num].setValue((GlobalDefine.PartyCharaIndex)idx, (int)BattleParam.m_PlayerParty.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx).GetPoisonDamage((StatusAilmentChara.PoisonType)num));
                        }
                    }
                }

                //----------------------------------------
                // 更新済みフラグ
                //----------------------------------------
                m_UpdatedStatusAilmentPlayer = true;

            }
        }


        //----------------------------------------
        //	毒ダメージ：描画
        //	@add Developer 2016/03/11 v330 ダメージ複数対応
        //----------------------------------------
        bool bDamageDraw = false;
        for (int num = 0; num < (int)StatusAilmentChara.PoisonType.MAX; ++num)
        {
            if (m_afAilmentDamagePlayer[num].getValue(GlobalDefine.PartyCharaIndex.MAX) <= 0)
            {
                continue;
            }

            BattleParam.m_PlayerParty.DamageHP(m_afAilmentDamagePlayer[num], m_afAilmentDamagePlayer[num], true, true, MasterDataDefineLabel.ElementType.NONE);
            m_afAilmentDamagePlayer[num].setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
            bDamageDraw = true;
            break;
        }
        //----------------------------------------
        //	ダメージ描画がある場合進まない
        //----------------------------------------
        if (bDamageDraw == true)
        {
            return;
        }

        // ダメージ描画終了を待つ
        if (BattleParam.m_PlayerParty.isShowingDamageNumber())
        {
            return;
        }

        //----------------------------------------
        //	中断復帰時に更新しない
        //----------------------------------------
        if ((m_ProcStep_player == EBATTLE_UPDATE_STEP.eSTART)
        || (m_ProcStep_player == EBATTLE_UPDATE_STEP.eUPDATE && m_FirstStep_player == false))
        {

            //----------------------------------------
            //	プレイヤー側状態異常ターン更新
            //----------------------------------------
            {
                BattleParam.m_PlayerParty.m_Ailments.UpdateTurnOnce(StatusAilmentChara.UpdateTiming.eBATTLE);
                BattleParam.m_PlayerParty.updateProvokeTurn();
            }

            //----------------------------------------
            // 場にパネルを配置
            // @add Developer 2015/12/25 v320：リーダー、パッシブ対応
            //----------------------------------------
            m_BattleLogicSkill.SkillBattlefieldPanel();

            //----------------------------------------
            // @add Developer 2016/01/04 v320 テーブル切り替え時：中断復帰対応
            //----------------------------------------
            m_BattleLogicEnemy.CheckEnemyActionSwitchTable();

            // 二回目以降の更新
            m_ProcStep_player = EBATTLE_UPDATE_STEP.eUPDATE;
        }

        m_FirstStep_player = false;


        //-----------------------------------------------------------------
        // 状態異常の遅延発動。プレイヤーのターン開始時に状態異常を発動させる。
        //-----------------------------------------------------------------
        m_BattleLogicAilment.updateDelaydSkill();

        //----------------------------------------
        // 敵リアクションが必要であれば、敵リアクション開始へ移行
        //----------------------------------------
        if (m_EnemyReactChkSkillID != 0)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION_START);
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：入力実行
        @return		EBATTLE_STEP		[次の処理ステップ]
    */
    //----------------------------------------------------------------------------
    void OnGameInput()
    {
    }

    void MonoUpdate_OnGameInput()
    {
        // リミブレスキル連続発動
        if (m_NextLimitBreakSkillFixID != 0)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK);
            return;
        }

        BattleTouchInput battle_touch_input = BattleTouchInput.Instance;
        //--------------------------------
        // 初回更新
        //--------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            m_ActiveSkillSuccess = false;
            m_bPutSuccess = false;

            //--------------------------------
            //	中断復帰データ保存
            //--------------------------------
            BattleParam.SaveLocalData();

            // 敵の死亡処理
            m_BattleLogicEnemy.DeadEnemy();

            //--------------------------------
            // 敵が全滅してるならクリア
            //--------------------------------
            if (m_BattleLogicEnemy.IsEnemyDestroyAll() == true)
            {
                // 例外処理:PlayerPhaseCaption非表示化
                m_DispCaption.outForce();
                // UIの非表示
                DisableAllWindow();

                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_CLEAR);
                return;
            }

            //----------------------------------------
            // 死亡しているプレイヤーパーティメンバーの状態異常を解除
            //----------------------------------------
            BattleParam.m_PlayerParty.clearAilmentDeadMember();

            //----------------------------------------
            // プレイヤーパーティが全滅してるならゲームオーバー処理へ遷移
            //----------------------------------------
            if (BattleParam.m_PlayerParty.m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MAX) <= 0)
            {
                // 例外処理:PlayerPhaseCaption非表示化
                m_DispCaption.outForce();
                // UIの非表示
                DisableAllWindow();

                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_DEAD_GAMEOVER);
                return;
            }

            // 個別ＨＰで追加
            if (BattleParam.IsKobetsuHP)
            {
                // プレイヤーパーティの最大ＨＰ再計算
                BattleParam.m_PlayerParty.calcPartyHP();

                // スキルリストの更新.
                CreateSkillList();
            }
            BattleParam.m_PlayerParty.updateGeneralPartyMember();

            m_BattleSkillForimngCheckFilters = new SkillRequestParam.SkillFilterType[BattleParam.m_PlayerParty.getPartyMemberMaxCount()];
            for (int idx = 0; idx < m_BattleSkillForimngCheckFilters.Length; idx++)
            {
                SkillRequestParam.SkillFilterType filter_type = SkillRequestParam.SkillFilterType.ALL;
                if (BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx) <= 0)
                {
                    // ターン開始時に死亡しているメンバーは前半スキルの成立判定をしない。
                    filter_type = SkillRequestParam.SkillFilterType.LAST_HALF;
                }
                m_BattleSkillForimngCheckFilters[idx] = filter_type;
            }

            m_BattleSkillFormingCheck.reset();
            initSkillReachInfo();   // リーチ線情報を初期化

            m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.PLAYER_PHASE);

            //----------------------------------------
            //	BOOST表示の更新
            //----------------------------------------
            for (int i = 0; i < m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax(); i++)
            {
                m_BattleCardManager.m_FieldAreas.getFieldArea(i).m_IsBoost = m_abBoostField[i];
            }

            // 汎用復活スキルを使用可能かどうか
            updateResurrectInfoView();

            // デバッグログ出力
            {
                DebugBattleLog.newLine();
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ プレイヤー入力受付開始"
                    + "(Trun:" + m_BattleTotalTurn.ToString()
                    + " SP:" + BattleParam.m_PlayerParty.m_PartyTotalSP.ToString()
                    + "/" + BattleParam.m_PlayerParty.m_PartyTotalSPMax.ToString()
                    + ") ]]]]]"
                );

                // 札
                DebugBattleLog.outputCard(m_BattleCardManager);

                //主人公、バトルアチーブ、ターゲット

                DebugBattleLog.flush();
            }

            BattleSceneManager.Instance.setTutorialPhase(BattleTutorialManager.TutorialBattlePhase.INPUT);

            // GC
            execGC(0.0f);
        }

        //--------------------------------
        // 攻撃リクエストの判定処理
        //--------------------------------
        m_BattleSkillFormingCheck.setCheck();   // 常にチェック
        m_BattleSkillFormingCheck.checkAsync(m_BattleCardManager, m_ActiveSkillOrder, m_abBoostField, m_BattleSkillForimngCheckFilters);

        if (m_BattleSkillFormingCheck.isUpdateResult())
        {
            // スキルアイコンを更新
            int new_skill_forming = m_SkillIconArea.setSkillInfos(m_BattleSkillFormingCheck.skills, m_BattleSkillFormingCheck.skills_count);   // スキルアイコンへ情報をセット
            setPartyHandsInfo(m_BattleSkillFormingCheck.skills, m_BattleSkillFormingCheck.skills_count, SkillRequestParam.SkillFilterType.ALL);

            // リーチ線情報を更新
            initSkillReachInfo();
            for (int idx = 0; idx < m_BattleSkillFormingCheck.reach_infos_count; idx++)
            {
                addSkillReachInfo(m_BattleSkillFormingCheck.reach_infos[idx].m_FieldIndex, m_BattleSkillFormingCheck.reach_infos[idx].m_Element);
            }

            // 新たに成立したスキルのエフェクト
            if (new_skill_forming >= 0)
            {
                m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.FIELD_AREA, new_skill_forming, BattleScene._BattleCardManager.EffectInfo.EffectType.SKILL_FORMED);
            }
        }

        //--------------------------------
        // 戦闘チュートリアル開始
        //--------------------------------
        if (BattleParam.IsTutorial())
        {
            if (BattleSceneManager.Instance.isTutorialAllDeadEnemy(true))
            {
                // 例外処理:PlayerPhaseCaption非表示化
                m_DispCaption.outForce();
                // UIの非表示
                DisableAllWindow();

                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_CLEAR);
                return;
            }
        }

        //--------------------------------
        // カウントダウン更新
        //--------------------------------
        if (m_InputCountDownStart == false)
        {
            battle_touch_input.setWorking(
                (BattleParam.m_TargetEnemyWindow == InGameDefine.SELECT_NONE)
                && (BattleSceneManager.Instance.isWaitTutorial() == false)
            );

            //--------------------------------
            // LBSリクエストがあれば専用処理ステップへ
            //--------------------------------
            if (BattleParam.ChkRequestLBS())
            {
                battle_touch_input.setWorking(false);

                if (BattleParam._IsHeroSkillLBS())
                {
                    if (BattleParam.m_PlayerParty.m_BattleHero.checkHeroSkill())
                    {
                        m_NextLimitBreakSkillCaster = GlobalDefine.PartyCharaIndex.HERO;
                        m_NextLimitBreakSkillFixID = 0;
                        BattleParam.ClrLBS();
                        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK);
                        return;
                    }
                }
                else
                {
                    //発動条件チェックをここでもしておく
                    if (BattleParam.IsEnableLBS(BattleParam._GetLBS()))
                    {
                        m_NextLimitBreakSkillCaster = BattleParam._GetLBS();
                        m_NextLimitBreakSkillFixID = 0;
                        BattleParam.ClrLBS();
                        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK);
                        return;
                    }
                }

                // リクエストされたリミブレスキルが発動条件を満たしていないのでキャンセル
                BattleParam.ClrLBS();
            }

            //--------------------------------
            // プレイヤーがカードを場に置いたらカウント開始
            // @change 新スキル対応：手札から場にカードを置いたかチェック	Developer 2015/10/19 ver300
            //--------------------------------
            if (m_bPutSuccess == true)
            {
                m_InputCountDownStart = true;
                m_InputCountDown = calcCountDownTime();

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "カウントダウン開始 " + m_InputCountDown.ToString() + "秒");

                // UI表示開始。
                m_CountDownArea.initTime(m_InputCountDown);

                m_InputCountDown = m_InputCountDown - Time.deltaTime;

                //break;
            }

            // カウントダウが始まる前は適度なタイミングでＧＣを実行する（カウントダウン中にＧＣが動作しないようにするため）
            {
                if (battle_touch_input.isTouchRelease())
                {
                    // ユーザーが指を離した直後にGCを実行（ただし場にパネルを置いた場合はＧＣが動作しないように）
                    execGC(0.5f, 4);
                }
                else
                if (battle_touch_input.isTouching() == false)
                {
                    // ユーザーが指を離している間は定期的にＧＣを実行
                    execGC(1.0f, 1);
                }
                else
                {
                    execGC(5.0f, 1);
                }
            }
        }
        else
        {
            //--------------------------------
            // カウントダウン起動済み
            //--------------------------------
            m_InputCountDown -= Time.deltaTime;
            if (m_InputCountDown < InGameDefine.COUNTDOWN_TIME_MIN)
            {
                m_InputCountDown = InGameDefine.COUNTDOWN_TIME_MIN;
            }
            m_CountDownArea.setTime(m_InputCountDown);

            // カウントダウン中はＧＣはしない
            cancelGC();
        }

        // オートプレイ切り替え
        switch (BattleParam.getAutoPlayState())
        {
            case BattleParam.AutoPlayState.OFF:
                {
                    if (BattleSceneManager.Instance.AutoPlay.isPlaying())
                    {
                        // カウントダウン始まった後はオートプレイボタンをオフにしてもオートプレイは止まらない。
                        if (m_InputCountDownStart == false)
                        {
                            BattleSceneManager.Instance.AutoPlay.stopAutoPlay();
                        }
                    }
                }
                break;

            case BattleParam.AutoPlayState.ON:
                {
                    if (BattleSceneManager.Instance.isWaitTutorial() == false)
                    {
                        if (BattleSceneManager.Instance.AutoPlay.isPlaying() == false)
                        {
                            BattleSceneManager.Instance.AutoPlay.startAutoPlay(m_AutoPlayAI);
                        }
                    }
                }
                break;
        }

        //--------------------------------
        // カウントダウンチェック
        //--------------------------------
        if (m_InputCountDown <= InGameDefine.COUNTDOWN_TIME_MIN)
        {
            // カード操作部分の入力受付停止
            battle_touch_input.setWorking(false);

            //--------------------------------
            // カウントダウンが完遂している場合
            // →入力結果を実行するフローへ遷移
            //--------------------------------
            m_InputCountDownStart = false;

            m_CountDownArea.setTime(0.0f);

            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_SKILL_SORT);
            return;
        }

        if (m_BattleCardManager.m_IsStartCountDown)
        {
            m_bPutSuccess = true;

            // カウントダウン中はＧＣはしない
            cancelGC();
        }

        if (m_BattleCardManager.isCatchingCardByPlayer() == false)
        {
            // 手札の空きを新カードで埋める.
            FillHandCard();
        }

        //-------------------------------------------------------------
        // PLAYER PHASEを出したり引っ込めたりする処理。
        //-------------------------------------------------------------
        {
            //---------------------------------------------------------
            // カードタッチ状態によってin/outアニメーション発行。
            //---------------------------------------------------------
            // プレイヤーがカードタッチしているorカウントダウン始まっている
            if (m_BattleCardManager.isCatchingCardByPlayer()
                || m_InputCountDownStart
            )
            {
                if (m_DispCaption.getCurrentCaption() == BattleCaptionControl.CaptionType.PLAYER_PHASE)
                {
                    m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.PLAYER_HOLD_CARD_PHASE);
                }
            }
            // プレイヤーがカードをタッチしていない＆カウントダウン始まっていない
            else
            {
                if (m_bPutSuccess == false)
                {
                    if (m_DispCaption.getCurrentCaption() == BattleCaptionControl.CaptionType.PLAYER_HOLD_CARD_PHASE)
                    {
                        m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.PLAYER_PHASE);
                    }
                }
            }
        }

        return;
    }

    public float calcCountDownTime()
    {
        float ret_val = InGameDefine.COUNTDOWN_TIME_DEFAULT;

        // 状態変化によるカウントダウンの変化
        ret_val += (float)BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX).GetCountDown();

        // リーダースキルによるカウントダウンの変化
        ret_val += InGameUtilBattle.GetLeaderSkillCountDown(GlobalDefine.PartyCharaIndex.LEADER);
        ret_val += InGameUtilBattle.GetLeaderSkillCountDown(GlobalDefine.PartyCharaIndex.FRIEND);

        // パッシブスキルによるカウントダウンの変化
        ret_val += InGameUtilBattle.PassiveChkCountDown();

        // カウントダウンの下限上限のチェック
        if (ret_val > InGameDefine.COUNTDOWN_TIME_MAX)
        {
            ret_val = InGameDefine.COUNTDOWN_TIME_MAX;
        }
        if (ret_val < InGameDefine.COUNTDOWN_PLAY_TIME_MIN)
        {
            ret_val = InGameDefine.COUNTDOWN_PLAY_TIME_MIN;
        }

        return ret_val;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：スキルソート
    */
    //----------------------------------------------------------------------------
    void OnGameSkillSort()
    {
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ 手札操作完了 ]]]]]");
        DebugBattleLog.outputCard(m_BattleCardManager);
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "成立スキル数：" + m_BattleSkillFormingCheck.skills_count.ToString());
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "成立スキル数：メンバー別：[LEADER:" + BattleParam.getPartyMemberHands(GlobalDefine.PartyCharaIndex.LEADER).ToString()
            + " MOB_1:" + BattleParam.getPartyMemberHands(GlobalDefine.PartyCharaIndex.MOB_1).ToString()
            + " MOB_2:" + BattleParam.getPartyMemberHands(GlobalDefine.PartyCharaIndex.MOB_2).ToString()
            + " MOB_3:" + BattleParam.getPartyMemberHands(GlobalDefine.PartyCharaIndex.MOB_3).ToString()
            + " FRIEND:" + BattleParam.getPartyMemberHands(GlobalDefine.PartyCharaIndex.FRIEND).ToString()
            + "]"
        );
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "成立スキル数：場単位"
            + "[" + BattleParam.getFieldSkillCount(0) + "]"
            + "[" + BattleParam.getFieldSkillCount(1) + "]"
            + "[" + BattleParam.getFieldSkillCount(2) + "]"
            + "[" + BattleParam.getFieldSkillCount(3) + "]"
            + "[" + BattleParam.getFieldSkillCount(4) + "]"
        );
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ ノーマルスキル発動1 ]]]]]");

        // スキル成立判定を確定する
        m_BattleSkillFormingCheck.check(m_BattleCardManager, m_ActiveSkillOrder, m_abBoostField, m_BattleSkillForimngCheckFilters);

        // リーチ線情報を初期化
        initSkillReachInfo();

        // スキルアイコンを更新
        int new_skill_forming = m_SkillIconArea.setSkillInfos(m_BattleSkillFormingCheck.skills, m_BattleSkillFormingCheck.skills_count);   // スキルアイコンへ情報をセット
        setPartyHandsInfo(m_BattleSkillFormingCheck.skills, m_BattleSkillFormingCheck.skills_count, SkillRequestParam.SkillFilterType.ALL);

        // 新たに成立したスキルのエフェクト
        if (new_skill_forming >= 0)
        {
            m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.FIELD_AREA, new_skill_forming, BattleScene._BattleCardManager.EffectInfo.EffectType.SKILL_FORMED);
        }

        // 最後に攻撃した敵をリセット
        SkillRequestParam.resetAttackTarget();

        // 主人公スキルの最大消化数をリセット
        BattleParam.m_PlayerParty.m_BattleHero.resetBattleTurn(4);

        // スキルの選択・ソート・攻撃情報付加
        m_BattleLogicSkill.m_SkillBoost.ResetSkillRequestBoost();
        m_BattleLogicSkill.SetAttackFix(m_BattleSkillFormingCheck.skills, m_BattleSkillFormingCheck.skills_count, SkillRequestParam.SkillFilterType.FIRST_HALF);

        //----------------------------------------
        // スキル発動確認
        //----------------------------------------
        int active_skill_count = m_BattleLogicSkill.m_SkillRequestActive.getRequestCount();
        if (active_skill_count > 0)
        {
            m_ActiveSkillSuccess = true;
        }

        // 統計情報集計
        {
            for (int idx = 0; idx < m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax(); idx++)
            {
                if (m_BattleCardManager.m_FieldAreas.getFieldArea(idx).getCardMaxCount() == m_BattleCardManager.m_FieldAreas.getFieldArea(idx).getCardCount())
                {
                    m_Stats_FullFieldCount++;
                }
            }
        }

        // 敵の「根性」を設定
        m_BattleLogicEnemy.setKonjo();

        // GCを実行
        execGC(0.0f);

        //----------------------------------------
        // 次ステップへ移行
        //----------------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_COUNT_FADE_IN);
        return;
    }

    void OnGameSkillSort2()
    {
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ ノーマルスキル発動2 ]]]]]");

        // 復活したキャラを含めてスキル判定
        // 汎用回復発動者を更新
        BattleParam.m_PlayerParty.updateGeneralPartyMember();
        // スキルリストを構築
        CreateSkillList();
        // スキル成立判定開始
        m_BattleSkillFormingCheck.setCheck();
        // スキル成立判定を確定する
        m_BattleSkillFormingCheck.check(m_BattleCardManager, m_ActiveSkillOrder, m_abBoostField, m_BattleSkillForimngCheckFilters);

        // スキルアイコンへ情報をセット
        m_SkillIconArea.setSkillInfos(m_BattleSkillFormingCheck.skills, m_BattleSkillFormingCheck.skills_count);
        setPartyHandsInfo(m_BattleSkillFormingCheck.skills, m_BattleSkillFormingCheck.skills_count, SkillRequestParam.SkillFilterType.LAST_HALF);

        // リーチ線情報を初期化
        initSkillReachInfo();

        // スキルの選択・ソート・攻撃情報付加
        m_BattleLogicSkill.SetAttackFix(m_BattleSkillFormingCheck.skills, m_BattleSkillFormingCheck.skills_count, SkillRequestParam.SkillFilterType.LAST_HALF);

        //----------------------------------------
        // スキル発動確認
        //----------------------------------------
        int active_skill_count = m_BattleLogicSkill.m_SkillRequestActive.getRequestCount();
        if (active_skill_count > 0)
        {
            m_ActiveSkillSuccess = true;
        }

        // 統計情報集計
        {
            for (int idx = 0; idx < m_Stats_NPanelSkillCount.Length; idx++)
            {
                m_Stats_NPanelSkillCount[idx] += m_BattleLogicSkill.m_ActiveSkillCostCount[idx];
            }

            int hands_count = m_BattleLogicSkill.m_SkillComboCountCalc;
            if (hands_count > m_Stats_NHandsCount.Length - 1)
            {
                hands_count = m_Stats_NHandsCount.Length - 1;
            }
            m_Stats_NHandsCount[hands_count]++;
        }

        // GCを実行
        execGC(0.0f);

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION2);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：スキルカウントフェードイン
    */
    //----------------------------------------------------------------------------
    void OnGameSkillCountIn()
    {
        //----------------------------------------
        // 初回更新
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true))
        {
            //----------------------------------------
            // スキルカウント関連の有効化
            //----------------------------------------
            m_CountFinishArea.setHands(m_BattleLogicSkill.m_SkillComboCountCalc);
            m_CountFinishArea.play();
        }


        // アニメーション終了待ち
        if (m_CountFinishArea.isPlaying())
        {
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_COUNT_FADE_OUT);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：スキルカウントフェードアウト
    */
    //----------------------------------------------------------------------------
    void OnGameSkillCountOut()
    {
        //----------------------------------------
        // 初回更新
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true))
        {
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_TITLE_IN);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：入力結果アクションタイトル表示
    */
    //----------------------------------------------------------------------------
    void OnGameActionTitleIn()
    {
        // 攻撃が成功していない場合はスキップ
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION);
            return;
        }

        // 次のステップへ
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：入力結果アクションタイトル非表示
    */
    //----------------------------------------------------------------------------
    void OnGameActionTitleOut()
    {
        // 攻撃が成功していない場合はスキップ
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL_TITLE);
            return;
        }

        // キャプション更新
        m_DispCaption.hideCaption();

        m_BattleComboArea.setEnable(false);

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL_TITLE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：入力結果アクション
        @return	EBATTLE_STEP			[次の処理ステップ]
    */
    //----------------------------------------------------------------------------
    void OnGameAction()
    {
        //----------------------------------------
        // スキル発動初期化
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // リーチ線のクリア用に最初に呼出し
            //----------------------------------------
            ClrSkillRequest();
            initSkillReachInfo();

            if (m_ActiveSkillSuccess)
            {
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.ACTIVE_SKILL);

                m_BattleComboArea.setHitCount(m_BattleLogicSkill.m_SkillComboCountDisp);
                m_BattleComboArea.setHands(m_BattleLogicSkill.m_SkillComboCountCalc);
                m_BattleComboArea.setEnable(true);
            }

            //----------------------------------------
            // カットイン主導で動くのでカットイン側にスキル起動情報を入力
            // 最初のカットインへの連鎖を許可することで処理スタート
            //----------------------------------------
            BattleSkillCutinManager.Instance.ClrSkillCutin();
            BattleSkillCutinManager.Instance.SetSkillCutin(m_BattleLogicSkill.m_SkillRequestActive);
            BattleSkillCutinManager.Instance.CutinStart2(true); //回復系ノーマルスキル

            if (BattleParam.m_PlayScoreInfo != null)
            {
                BattleParam.m_PlayScoreInfo.startTurn();
            }

            return;
        }

        //----------------------------------------
        //	スキル更新処理
        //----------------------------------------
        if (m_BattleLogicSkill.SkillUpdate(m_BattleLogicSkill.m_SkillRequestActive) == false)
        {
            return;
        }

        //----------------------------------------
        // スキルリクエストを破棄
        //----------------------------------------
        ClrSkillRequest();

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_SKILL_SORT2);
        return;
    }

    void OnGameAction2()
    {
        //----------------------------------------
        // スキル発動初期化
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // リーチ線のクリア用に最初に呼出し
            //----------------------------------------
            ClrSkillRequest();
            initSkillReachInfo();

            //----------------------------------------
            // 手札と場を再構築
            //----------------------------------------
            BattleClearField();
            BattleClearHand(false);

            //----------------------------------------
            // ハンズ数更新
            //----------------------------------------
            if (m_ActiveSkillSuccess)
            {
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.ACTIVE_SKILL);

                m_BattleComboArea.setHitCount(m_BattleLogicSkill.m_SkillComboCountDisp);
                m_BattleComboArea.setHands(m_BattleLogicSkill.m_SkillComboCountCalc);
                m_BattleComboArea.setEnable(true);
            }

            //----------------------------------------
            // カットイン主導で動くのでカットイン側にスキル起動情報を入力
            // 最初のカットインへの連鎖を許可することで処理スタート
            //----------------------------------------
            BattleSkillCutinManager.Instance.ClrSkillCutin();
            BattleSkillCutinManager.Instance.SetSkillCutin(m_BattleLogicSkill.m_SkillRequestActive);
            BattleSkillCutinManager.Instance.CutinStart2(true); //攻撃系ノーマルスキル

            return;
        }

        //----------------------------------------
        //	スキル更新処理
        //----------------------------------------
        if (m_BattleLogicSkill.SkillUpdate(m_BattleLogicSkill.m_SkillRequestActive) == false)
        {
            return;
        }

        //----------------------------------------
        // スキルリクエストを破棄
        //----------------------------------------
        ClrSkillRequest();

        //----------------------------------------
        // 状態異常によるターンの更新
        // ※※※ここの更新は他へ移動する
        //----------------------------------------
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {
            if (BattleParam.m_EnemyParam[i] == null)
            {
                continue;
            }

            // ターンを加算
            BattleParam.m_EnemyParam[i].m_EnemyTurnMax = BattleParam.m_EnemyParam[i].getMasterDataParamEnemy().status_turn;
        }

        // スキルアイコンをクリア
        m_SkillIconArea.setSkillInfos(null, 0);

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_FINISH);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：入力結果アクション終了
    */
    //----------------------------------------------------------------------------
    void OnGameActionFinish()
    {
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // 指定HANDS(以上)だったらプレイヤーのHPを回復する
            //----------------------------------------
            {
                SkillRequestParam skill_requset_param = new SkillRequestParam(((int)GlobalDefine.PartyCharaIndex.MAX) * 2);
                InGameUtilBattle.PassiveChkHandsHealHP(ref skill_requset_param, m_BattleLogicSkill.m_SkillComboCountCalc);

                BattleSkillCutinManager.Instance.ClrSkillCutin();
                BattleSkillCutinManager.Instance.SetSkillCutin(skill_requset_param);
                BattleSkillCutinManager.Instance.CutinStart2(true, true);   //パッシブスキル・リンクパッシブ（ハンズ数で回復）
            }

            return;
        }

        //----------------------------------------
        //	スキル更新処理
        //----------------------------------------
        if (m_BattleLogicSkill.SkillUpdate(BattleSkillCutinManager.Instance.getSkillRequestParam()) == false)
        {
            return;
        }

        //----------------------------------------
        // プレイヤー状態異常更新済みフラグをオフ
        //----------------------------------------
        m_UpdatedStatusAilmentPlayer = false;


        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_TITLE_OUT);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：ブーストスキルによるアクションタイトル表示
    */
    //----------------------------------------------------------------------------
    void OnGameBoostSkillTitle()
    {
        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // 攻撃が成功していない場合はスキップ
            //----------------------------------------
            if (m_ActiveSkillSuccess == false)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL);
                return;
            }

            //----------------------------------------
            // ブーストスキルが発動していない場合はスキップ
            //----------------------------------------
            if (m_BattleLogicSkill.m_SkillBoost.BoostReqInputNum <= 0)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL);
                return;
            }

            // タイトルを表示
            m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.BOOST_SKILL);

            m_Stats_BoostSkillCount += m_BattleLogicSkill.m_SkillBoost.BoostReqInputNum;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：ブーストスキルによるアクション
    */
    //----------------------------------------------------------------------------
    void OnGameBoostSkill()
    {
        //----------------------------------------
        // 攻撃が成功していない場合はスキップ
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL_FINISH);
            return;
        }

        //----------------------------------------
        // ブーストスキルが発動していない場合はスキップ
        //----------------------------------------
        if (m_BattleLogicSkill.m_SkillBoost.BoostReqInputNum <= 0)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL_FINISH);
            return;
        }

        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ ブーストスキル発動 ]]]]]");

            // スキル発行情報のソート(ターゲット情報作成)
            m_BattleLogicSkill.m_SkillBoost.SkillRequestBoost.sortSkillRequest(SkillRequestParam.SkillFilterType.ALL);

            //----------------------------------------
            // カットイン主導で動くのでカットイン側にスキル起動情報を入力
            // 最初のカットインへの連鎖を許可することで処理スタート
            //----------------------------------------
            m_BattleLogicSkill.m_SkillBoost.CutinStart();

            return;
        }

        //----------------------------------------
        // スキル更新処理
        //----------------------------------------
        if (m_BattleLogicSkill.SkillUpdate(m_BattleLogicSkill.m_SkillBoost.SkillRequestBoost) == false)
        {
            return;
        }

        //----------------------------------------
        // スキル発行情報クリア
        //----------------------------------------
        m_BattleLogicSkill.m_SkillBoost.ResetSkillRequestBoost();


        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL_FINISH);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	// 戦闘処理ステップ：ゲーム：ブーストスキルによるアクション終了
    */
    //----------------------------------------------------------------------------
    void OnGameBoostSkillFinish()
    {
        //----------------------------------------
        // 攻撃が成功していない場合はスキップ
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL_TITLE);
            return;
        }

        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // ブーストスキルが発動している場合
            //----------------------------------------
            if (m_BattleLogicSkill.m_SkillBoost.BoostReqInputNum > 0)
            {
                // タイトルを非表示(現表示中のを指定することで、退場する)
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.BOOST_SKILL);
            }

            return;
        }


        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL_TITLE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：スキルによる追撃タイトル表示
    */
    //----------------------------------------------------------------------------
    void OnGameActionSkillTitle()
    {
        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // 攻撃が成功していない場合はスキップ
            //----------------------------------------
            if (m_ActiveSkillSuccess == false)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL);
                return;
            }

            //----------------------------------------
            // 発動可能なリーダースキルを所持しているか
            //----------------------------------------
            if (!InGameUtilBattle.CheckLaeaderSkill(GlobalDefine.PartyCharaIndex.LEADER)
            && !InGameUtilBattle.CheckLaeaderSkill(GlobalDefine.PartyCharaIndex.FRIEND))
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL);
                return;
            }

            m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LEADER_SKILL);
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：スキルによる追撃
    */
    //----------------------------------------------------------------------------
    void OnGameActionSkill()
    {
        //----------------------------------------
        // 攻撃が成功していなければ終了
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL_FINISH);
            return;
        }

        //----------------------------------------
        // 発動可能なリーダースキルを所持しているか
        //----------------------------------------
        bool leader = InGameUtilBattle.CheckLaeaderSkill(GlobalDefine.PartyCharaIndex.LEADER);
        bool friend = InGameUtilBattle.CheckLaeaderSkill(GlobalDefine.PartyCharaIndex.FRIEND);

        if (!leader && !friend)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL_FINISH);
            return;
        }

        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ リーダースキル追撃発動 ]]]]]");

            m_BattleLogicSkill.ResetSkillRequestLeader();

            // リーダースキル発動
            BattleSkillCutinManager.Instance.ClrSkillCutin();

            m_BattleLogicSkill.m_SkillRequestLeader.clearRequest();

            // リーダースキル
            if (leader)
            {
                BattleSkillActivity leader_skill_activity = m_BattleLogicSkill.RequestLeaderSkillFollowAttack(GlobalDefine.PartyCharaIndex.LEADER);
                m_BattleLogicSkill.m_SkillRequestLeader.addSkillRequest(leader_skill_activity);
            }

            // リーダースキル[フレンド]
            if (friend)
            {
                BattleSkillActivity leader_skill_activity = m_BattleLogicSkill.RequestLeaderSkillFollowAttack(GlobalDefine.PartyCharaIndex.FRIEND);
                m_BattleLogicSkill.m_SkillRequestLeader.addSkillRequest(leader_skill_activity);
            }

            // リーダースキル発動確認
            if (m_BattleLogicSkill.m_SkillRequestLeader.getRequestCount() > 0)
            {
                BattleSkillCutinManager.Instance.SetSkillCutin(m_BattleLogicSkill.m_SkillRequestLeader);
                BattleSkillCutinManager.Instance.CutinStart2(true, true);   //追撃

                // 発動していれば、スキルの更新処理へ
                return;
            }

            // 何も発動していないため、次のステップへ
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL_FINISH);
            return;
        }


        //----------------------------------------
        //	スキルの更新処理
        //----------------------------------------
        if (m_BattleLogicSkill.SkillUpdate(m_BattleLogicSkill.m_SkillRequestLeader) == false)
        {
            return;
        }


        //----------------------------------------
        //	スキル発行情報クリア
        //----------------------------------------
        m_BattleLogicSkill.ResetSkillRequestLeader();


        // 処理が完了したら次のステップへ
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL_FINISH);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘ステップ：ゲーム：スキルによる追撃終了
    */
    //----------------------------------------------------------------------------
    void OnGameActionSkillFinish()
    {
        //----------------------------------------
        // 攻撃が成功していなければ終了
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL_TITLE);
            return;
        }

        //----------------------------------------
        // スキル総評表示初期化
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {

            //----------------------------------------
            // 発動可能なリーダースキルを所持しているか
            //----------------------------------------
            if (InGameUtilBattle.CheckLaeaderSkill(GlobalDefine.PartyCharaIndex.LEADER)
            || InGameUtilBattle.CheckLaeaderSkill(GlobalDefine.PartyCharaIndex.FRIEND))
            {
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LEADER_SKILL);
            }

            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL_TITLE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：パッシブスキルによる追撃タイトル表示
    */
    //----------------------------------------------------------------------------
    void OnGamePassiveSkillTitle()
    {
        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // 攻撃が成功していない場合はスキップ
            //----------------------------------------
            if (m_ActiveSkillSuccess == false)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL);
                return;
            }

            //----------------------------------------
            // 発動可能なパッシブスキルを所持しているか
            //----------------------------------------
            if (InGameUtilBattle.PassiveChkFollowAttack() == false)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL);
                return;
            }

            m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.PASSIVE_SKILL);
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：パッシブスキルによる追撃
    */
    //----------------------------------------------------------------------------
    void OnGamePassiveSkill()
    {
        //----------------------------------------
        // 攻撃が成功していなければ終了
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL_FINISH);
            return;
        }

        //----------------------------------------
        // 発動可能なパッシブスキルを所持しているか
        //----------------------------------------
        bool passive = InGameUtilBattle.PassiveChkFollowAttack();
        if (passive == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL_FINISH);
            return;
        }

        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ パッシブスキル発動 ]]]]]");

            m_BattleLogicSkill.ResetSkillRequestPassive();

            // パッシブスキル追撃リクエスト
            bool bRet = m_BattleLogicSkill.RequestPassiveSkillFollowAttack();

            // パッシブスキル発動確認
            if (bRet == true)
            {

                // パッシブスキル発動
                BattleSkillCutinManager.Instance.ClrSkillCutin();
                BattleSkillCutinManager.Instance.SetSkillCutin(m_BattleLogicSkill.m_SkillRequestPassive);
                BattleSkillCutinManager.Instance.CutinStart2(true, true);   //パッシブスキル

                // 発動していれば、スキルの更新処理へ
                return;
            }

            // 何も発動していないため、次のステップへ
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL_FINISH);
            return;
        }


        //----------------------------------------
        //	パッシブスキルの更新処理
        //----------------------------------------
        if (m_BattleLogicSkill.SkillUpdate(m_BattleLogicSkill.m_SkillRequestPassive) == false)
        {
            return;
        }

        //----------------------------------------
        //	パッシブスキル発動情報クリア
        //----------------------------------------
        m_BattleLogicSkill.ResetSkillRequestPassive();


        // 処理が完了したら次のステップへ
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL_FINISH);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘ステップ：ゲーム：パッシブスキルによる追撃終了
    */
    //----------------------------------------------------------------------------
    void OnGamePassiveSkillFinish()
    {
        //----------------------------------------
        // 攻撃が成功していなければ終了
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE_TITLE);
            return;
        }

        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {

            //----------------------------------------
            // 発動可能なパッシブスキルを所持しているか
            //----------------------------------------
            if (InGameUtilBattle.PassiveChkFollowAttack() == true)
            {
                // タイトルを非表示(現表示中のを指定することで、退場する)
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.PASSIVE_SKILL);
            }

            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE_TITLE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：リンクパッシブによる追撃タイトル表示
    */
    //----------------------------------------------------------------------------
    void OnGameLinkPassiveTitle()
    {
        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // 攻撃が成功していない場合はスキップ
            //----------------------------------------
            if (m_ActiveSkillSuccess == false)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE);
                return;
            }

            //----------------------------------------
            // 発動可能なリンクパッシブを所持しているか
            //----------------------------------------
            if (InGameUtilBattle.PassiveChkFollowAttack(ESKILLTYPE.eLINKPASSIVE) == false)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE);
                return;
            }

            m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LINK_PASSIVE);
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：リンクパッシブによる追撃
    */
    //----------------------------------------------------------------------------
    void OnGameLinkPassive()
    {
        //----------------------------------------
        // 攻撃が成功していなければ終了
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE_FINISH);
            return;
        }

        //----------------------------------------
        // 発動可能なリンクパッシブを所持しているか
        //----------------------------------------
        bool passive = InGameUtilBattle.PassiveChkFollowAttack(ESKILLTYPE.eLINKPASSIVE);
        if (passive == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE_FINISH);
            return;
        }

        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ リンクパッシブスキル発動 ]]]]]");

            m_BattleLogicSkill.ResetSkillRequestLinkPassive();

            // リンクパッシブ追撃リクエスト
            bool bRet = m_BattleLogicSkill.RequestPassiveSkillFollowAttack(ESKILLTYPE.eLINKPASSIVE);

            // リンクパッシブ発動確認
            if (bRet == true)
            {
                // リンクパッシブ発動
                BattleSkillCutinManager.Instance.ClrSkillCutin();
                BattleSkillCutinManager.Instance.SetSkillCutin(m_BattleLogicSkill.m_SkillRequestLinkPassive);
                BattleSkillCutinManager.Instance.CutinStart2(true, true);   // リンクパッシブ

                // 発動していれば、スキルの更新処理へ
                return;
            }

            // 何も発動していないため、次のステップへ
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE_FINISH);
            return;
        }


        //----------------------------------------
        // リンクパッシブの更新処理
        //----------------------------------------
        if (m_BattleLogicSkill.SkillUpdate(m_BattleLogicSkill.m_SkillRequestLinkPassive) == false)
        {
            return;
        }

        //----------------------------------------
        // リンクパッシブ発動情報クリア
        //----------------------------------------
        m_BattleLogicSkill.ResetSkillRequestLinkPassive();


        // 処理が完了したら次のステップへ
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE_FINISH);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘ステップ：ゲーム：リンクパッシブよる追撃終了
    */
    //----------------------------------------------------------------------------
    void OnGameLinkPassiveFinish()
    {
        //----------------------------------------
        // 攻撃が成功していなければ終了
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_END);
            return;
        }

        //----------------------------------------
        // 初回実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            //----------------------------------------
            // 発動可能なリンクパッシブを所持しているか
            //----------------------------------------
            if (InGameUtilBattle.PassiveChkFollowAttack(ESKILLTYPE.eLINKPASSIVE) == true)
            {
                // タイトルを非表示(現表示中のを指定することで、退場する)
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LINK_PASSIVE);
            }

            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_END);
        return;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		評価の表示
        @return		EBATTLE_STEP		[次のステップ]
    */
    //------------------------------------------------------------------------
    void OnGameActionEnd()
    {

        //----------------------------------------
        // 攻撃が成功していなければスキップ
        //----------------------------------------
        if (m_ActiveSkillSuccess == false)
        {
            // 敵の「根性」を実行
            m_BattleLogicEnemy.execKonjo();

            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_ENEMY_START);
            return;
        }

        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            // 敵の「根性」を実行
            m_BattleLogicEnemy.execKonjo();

            m_BattleComboArea.setEnable(false);
            //----------------------------------------
            // アニメーション開始
            //----------------------------------------
            bool ret = m_BattleComboFinishArea.setComboFinish(m_BattleLogicSkill.m_SkillComboCountCalc);
            if (ret == false)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_ENEMY_START);
                return;
            }
        }

        //----------------------------------------
        // アニメーション完遂判定
        //----------------------------------------
        if (m_BattleComboFinishArea.isUpdating())
        {
            return;
        }

        //----------------------------------------
        // 実績解除
        //----------------------------------------
        InGameUtil.UnlockAchievement_Hnads(m_BattleLogicSkill.m_SkillComboCountCalc);


        //----------------------------------------
        // 次ステップへ
        //----------------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_ENEMY_START);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：caption:敵フェイズ
    */
    //----------------------------------------------------------------------------
    void OnPhaseEnemyStart()
    {
        // 敵が全滅していたらキャプション表示フローを飛ばす
        if (m_BattleLogicEnemy.IsEnemyDestroyAll() == true)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_AILMENTUPDATE);
            return;
        }

        m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.ENEMY_PHASE);

        //ヘイト計算
        BattleParam.m_PlayerParty.updateHate();

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_AILMENTUPDATE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：状態異常処理
    */
    //----------------------------------------------------------------------------
    void OnGameAilmentUpdate()
    {

        int nDmgIdx;

        // 初回処理
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ 状態異常更新 ]]]]]");


            BattleSceneUtil.MultiInt healVal = new BattleSceneUtil.MultiInt();
            bool badCondition;

            for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
            {
                if (BattleParam.m_EnemyParam[i] == null)
                {
                    continue;
                }

                // 死亡していない場合
                if (BattleParam.m_EnemyParam[i].isDead() == false)
                {
                    //----------------------------------------
                    //	リジェネ
                    //	※都合の悪い状態異常がかかっている場合回復が作動しない
                    //----------------------------------------
                    badCondition = BattleParam.m_EnemyParam[i].m_StatusAilmentChara.ChkStatusAilmentCondition(StatusAilment.GoodOrBad.BAD);
                    if (badCondition == false)
                    {

                        healVal = BattleParam.m_EnemyParam[i].m_StatusAilmentChara.GetHealValue();
                        if (healVal.getValue(GlobalDefine.PartyCharaIndex.MAX) != 0)
                        {
                            m_BattleLogicEnemy.AddHealEnemyHP(i, healVal.getValue(GlobalDefine.PartyCharaIndex.LEADER));
                        }

                    }

                    //----------------------------------------
                    //	毒ダメージ：取得
                    //	@change Developer 2016/03/11 v330 毒複数対応
                    //----------------------------------------
                    nDmgIdx = i * (int)StatusAilmentChara.PoisonType.MAX;
                    for (int num = 0; num < (int)StatusAilmentChara.PoisonType.MAX; ++num)
                    {
                        m_afAilmentDamageEnemy[nDmgIdx + num] = BattleParam.m_EnemyParam[i].m_StatusAilmentChara.GetPoisonDamage((StatusAilmentChara.PoisonType)num);
                    }

                    //----------------------------------------
                    //	毒ダメージ：描画
                    //	@change Developer 2016/03/11 v330 毒複数対応
                    //----------------------------------------
                    for (int num = 0; num < (int)StatusAilmentChara.PoisonType.MAX; ++num)
                    {
                        nDmgIdx += num;
                        if (m_afAilmentDamageEnemy[nDmgIdx] == 0.0f)
                        {
                            continue;
                        }

                        m_BattleLogicEnemy.AddDamageEnemyHP(i, (int)m_afAilmentDamageEnemy[nDmgIdx], EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL, GlobalDefine.PartyCharaIndex.MAX);
                        m_afAilmentDamageEnemy[nDmgIdx] = 0.0f;
                        break;
                    }
                }
            }

            return;
        }


        //----------------------------------------
        //	ダメージが複数ある場合
        //	@add Developer 2016/03/11 v330 ダメージ複数対応
        //----------------------------------------
        bool bDamageDraw = false;
        for (int enemyIdx = 0; enemyIdx < BattleParam.m_EnemyParam.Length; ++enemyIdx)
        {
            if (BattleParam.m_EnemyParam[enemyIdx] == null
            || BattleParam.m_EnemyParam[enemyIdx].isDead() == true)
            {
                continue;
            }

            //----------------------------------------
            //	毒ダメージ：描画
            //----------------------------------------
            nDmgIdx = enemyIdx * (int)StatusAilmentChara.PoisonType.MAX;
            for (int num = 0; num < (int)StatusAilmentChara.PoisonType.MAX; ++num)
            {
                nDmgIdx += num;
                if (m_afAilmentDamageEnemy[nDmgIdx] == 0.0f)
                {
                    continue;
                }

                m_BattleLogicEnemy.AddDamageEnemyHP(enemyIdx, (int)m_afAilmentDamageEnemy[nDmgIdx], EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL, GlobalDefine.PartyCharaIndex.MAX);
                m_afAilmentDamageEnemy[nDmgIdx] = 0.0f;
                bDamageDraw = true;
                break;
            }
        }
        //----------------------------------------
        //	ダメージ描画がある場合進まない
        //----------------------------------------
        if (bDamageDraw == true)
        {
            return;
        }

        // ダメージ描画中は進まない
        if (DrawDamageManager.isShowing())
        {
            return;
        }

        // 敵の死亡処理
        m_BattleLogicEnemy.DeadEnemy();

        // 敵死亡エフェクト描画中は進まない
        if (EffectManager.Instance.isPlayingEffect(EFFECT_HANDLE_ENEMY_DEATH))
        {
            return;
        }

        // ドロップアニメーション中は進まない
        if (EffectManager.Instance.isPlayingEffect(EFFECT_HANDLE_UNIT_DROP) == true)
        {
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURNUPDATE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：敵ターン更新
    */
    //----------------------------------------------------------------------------
    void OnGameEnemyTurnUpdate()
    {
        bool bPlaySE = false;
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {
            BattleEnemy battle_enemy = BattleParam.m_EnemyParam[i];
            if (battle_enemy != null)
            {
                bool is_update = battle_enemy.updateTurn();

                if (is_update)
                {
                    bPlaySE = true;

                    // ターン変化エフェクト
                    m_BattleDispEnemy.playTurnChangeEffect(i);
                }
            }
        }

        //----------------------------------------
        // ターン更新があればSEを再生
        //----------------------------------------
        if (bPlaySE == true)
        {
            // ターン経過SE
            SoundUtil.PlaySE(SEID.SE_BATLE_ENEMY_TURN);
        }


        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURN);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：敵ターン
    */
    //----------------------------------------------------------------------------
    void OnGameEnemyTurn()
    {

        //----------------------------------------
        // カウンター処理から、戻ってきた場合
        // @add Developer 2015/12/16 v320
        //----------------------------------------
        if (BattleLogicFSM.Instance.getPrevStep() == EBATTLE_STEP.eBATTLE_STEP_GAME_COUNTER_PLAYER)
        {
            // 初回処理防止
            BattleLogicFSM.Instance.isChangeStepTriger(true);
        }

        //----------------------------------------
        //	初回処理
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ 敵行動 ]]]]]");

            //----------------------------------------
            //	行動テーブル選択処理
            //----------------------------------------
            m_BattleLogicEnemy.EnemyActionTableSelectAll();


            //----------------------------------------
            // 敵攻撃関連処理：事前処理
            //----------------------------------------
            m_BattleLogicEnemy.EnemyAttackAllPreProc();

        }

        //----------------------------------------
        // カウンター処理が必要であればカウンター処理へ移行
        // @add Developer 2015/12/16 v320
        //----------------------------------------
        if (m_SkillCounter == true)
        {
            //----------------------------------------
            //	動きが速すぎるので、適当に待つ
            //----------------------------------------
            if (m_BattleLogicEnemy.isWaitingActionTimer())
            {
                return;
            }

            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_COUNTER_PLAYER);
            return;
        }

        bool updateResult;

        //----------------------------------------
        //	敵攻撃関連処理：メイン処理(カウンターあり)
        //----------------------------------------
        updateResult = m_BattleLogicEnemy.EnemyAttackAllMainProc(true, 1);
        if (updateResult == false)
        {
            return;
        }


        //----------------------------------------
        //	敵攻撃関連処理：後処理
        //----------------------------------------
        updateResult = m_BattleLogicEnemy.EnemyAttackAllPostProc();
        if (updateResult == false)
        {
            return;
        }


        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_ENEMY_END);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：caption:EnemyPhase:End
    */
    //----------------------------------------------------------------------------
    void OnPhaseEnemyEnd()
    {

        //----------------------------------------
        //	動きが速すぎるので、適当に待つ
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            m_BattleLogicEnemy.clearActionTimer();

            //----------------------------------------
            // @add Developer 2016/01/04 v320 テーブル切り替え時：中断復帰対応
            //----------------------------------------
            m_BattleLogicEnemy.CheckEnemyActionSwitchTable();
        }

        if (m_BattleLogicEnemy.isWaitingActionTimer())
        {
            return;
        }

        if (m_DispCaption.isDisplayCaption() == true)
        {
            m_DispCaption.hideCaption();
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_RESULT);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：カウンター処理
        @chenge		Developer 2015/09/15 ver300	リンクパッシブ対応
                    色々煩雑で気持ち悪いが、諸々の事情によりこの形
                    敵リアクションに対するカウンター処理を見直せば、綺麗にできる
    */
    //----------------------------------------------------------------------------
    void OnGameCounterPlayer()
    {

        //----------------------------------------
        //	カットイン実行
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            // 敵の「根性」を設定
            m_BattleLogicEnemy.setKonjo();

            BattleSkillCutinManager.Instance.ClrSkillCutin();
            if (m_PassiveSkillCounter == true)
            {
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.PASSIVE_SKILL);
                BattleSkillCutinManager.Instance.SetSkillCutin(m_BattleLogicSkill.m_SkillRequestPassive);
            }
            else if (m_LinkPassiveCounter == true)
            {
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LINK_PASSIVE);
                BattleSkillCutinManager.Instance.SetSkillCutin(m_BattleLogicSkill.m_SkillRequestLinkPassive);
            }
            BattleSkillCutinManager.Instance.CutinStart2(true, true);   //カウンター

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ プレイヤーカウンター ]]]]]");

            return;
        }


        //----------------------------------------
        //	パッシブスキル実行処理
        //----------------------------------------
        #region ==== パッシブスキル処理 ====
        if (m_PassiveSkillCounter == true)
        {
            if (m_BattleLogicSkill.SkillUpdate(m_BattleLogicSkill.m_SkillRequestPassive) == false)
            {
                return;
            }

            //----------------------------------------
            //	パッシブスキル発動情報クリア
            //----------------------------------------
            m_BattleLogicSkill.ResetSkillRequestPassive();
            m_PassiveSkillCounter = false;

            //----------------------------------------
            //	リンクパッシブ実行準備
            //----------------------------------------
            if (m_LinkPassiveCounter == true)
            {
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LINK_PASSIVE);
                BattleSkillCutinManager.Instance.ClrSkillCutin();
                BattleSkillCutinManager.Instance.SetSkillCutin(m_BattleLogicSkill.m_SkillRequestLinkPassive);
                BattleSkillCutinManager.Instance.CutinStart2(true, true);   //カウンターパッシブ
                return;
            }
        }
        #endregion
        //----------------------------------------
        //	リンクパッシブ実行処理
        //----------------------------------------
        #region ==== リンクパッシブ処理 ====
        else if (m_LinkPassiveCounter == true)
        {
            if (m_BattleLogicSkill.SkillUpdate(m_BattleLogicSkill.m_SkillRequestLinkPassive) == false)
            {
                return;
            }

            //----------------------------------------
            //	リンクパッシブ発動情報クリア
            //----------------------------------------
            m_BattleLogicSkill.ResetSkillRequestLinkPassive();
            m_LinkPassiveCounter = false;
        }
        #endregion


        //----------------------------------------
        //	カウンターフラグ初期化
        //----------------------------------------
        m_SkillCounter = false;

        // 敵の「根性」を実行
        m_BattleLogicEnemy.execKonjo();

        //----------------------------------------
        //	敵の死亡処理
        //----------------------------------------
        m_BattleLogicEnemy.DeadEnemy();

        //----------------------------------------
        //	1つ前のステップが、敵行動中の場合
        //	@change Developer 2015/12/16 v320
        //	@note	カウンター発動タイミング変更に伴い、switchより下の処理は通らないが念のため残しておく
        //			※switchを通らない場合、戦闘の流れや表示がおかしくなる(switchをなくして、常に1つ前のステップに戻してもいいかも)
        //----------------------------------------
        switch (BattleLogicFSM.Instance.getPrevStep())
        {
            case EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE_ATTACK:      // バックアタック
            case EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURN_FIRST:       // 先制攻撃
            case EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURN:             // 通常攻撃
            case EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION:         // 敵特性反撃
                                                                        //			case EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION_END:
                                                                        //--------------------------------
                                                                        //　敵全滅確認
                                                                        //--------------------------------
                if (m_BattleLogicEnemy.IsEnemyDestroyAll() == true)
                {
                    //----------------------------------------
                    //	キャプション非表示
                    //----------------------------------------
                    m_DispCaption.hideCaption();
                }
                else
                {
                    //----------------------------------------
                    //	キャプション設定
                    //----------------------------------------
                    m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.ENEMY_PHASE);
                }

                BattleLogicFSM.Instance.step(BattleLogicFSM.Instance.getPrevStep());
                return;
        }

        //----------------------------------------
        //	キャプション非表示
        //----------------------------------------
        m_DispCaption.hideCaption();

        //----------------------------------------
        //	ターン終了処理へ移行
        //----------------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_RESULT);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：：ゲーム：ターン終了処理
    */
    //----------------------------------------------------------------------------
    void OnGameResult()
    {
        // ダメージ描画中は進まない
        if (DrawDamageManager.isShowing())
        {
            return;
        }

        if (m_NextLimitBreakSkillFixID == 0)    //リミブレスキル連続発動中は全滅判定をしない
        {
            m_BattleLogicEnemy.DeadEnemy();
        }

        // 敵死亡エフェクト描画中は進まない
        if (EffectManager.Instance.isPlayingEffect(EFFECT_HANDLE_ENEMY_DEATH))
        {
            return;
        }

        // ドロップアニメーション中は進まない
        if (EffectManager.Instance.isPlayingEffect(EFFECT_HANDLE_UNIT_DROP) == true)
        {
            return;
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ ターン結果 ]]]]]");
        DebugBattleLog.flush();

        //----------------------------------------
        // ノーマルスキル発行情報クリア
        // (各スキルでNSの発動コストをチェックするLSがあるため、StepFuncGameAction内からここに変更)
        // @Developer 2015/08/10 ver290
        //----------------------------------------
        m_BattleLogicSkill.ResetSkillRequestActive();

        //----------------------------------------
        // 死亡しているプレイヤーパーティメンバーの状態異常を解除
        //----------------------------------------
        BattleParam.m_PlayerParty.clearAilmentDeadMember();

        bool is_alive_player = (BattleParam.m_PlayerParty.m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0);
        if (m_NextLimitBreakSkillFixID != 0)    //リミブレスキル連続発動中は全滅判定をしない
        {
            //ここに来るのは中断復帰した場合のみ
            is_alive_player = true;
        }


        //----------------------------------------
        // スキルを発動していればリミットブレイクスキルのコスト更新
        //----------------------------------------
        if (m_ActiveSkillSuccess == true)
        {

            for (int i = 0; i < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); i++)
            {
                CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)i, CharaParty.CharaCondition.SKILL_TURN1);
                if (chara != null)
                {
                    chara.AddCharaLimitBreak(GlobalDefine.SKILL_LIMITBREAK_TURN_ADD);
                }
            }

        }


        // 発動スキル情報削除
        m_BattleLogicSkill.ClrAttackFix();


        //--------------------------------
        // 全員が攻撃した後にクリアされるフラグの情報更新
        //--------------------------------
        bool clearOnAttack = true;
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {
            if (BattleParam.m_EnemyParam[i] == null)
            {
                continue;
            }

            if (BattleParam.m_EnemyParam[i].getAttackFlag() == true)
            {
                continue;
            }

            clearOnAttack = false;
        }

        if (clearOnAttack == true)
        {
            for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
            {
                if (BattleParam.m_EnemyParam[i] == null)
                {
                    continue;
                }

                BattleParam.m_EnemyParam[i].ClearOnAttack();
            }
        }


        //--------------------------------
        // 敵全滅確認
        //--------------------------------
        bool bEnemyDestroy = false;
        if (m_NextLimitBreakSkillFixID == 0)    //リミブレスキル連続発動中は全滅判定をしない
        {
            bEnemyDestroy = m_BattleLogicEnemy.IsEnemyDestroyAll();
        }


        //--------------------------------
        // 中断復帰で初回更新処理が何度も呼ばれてしまうため、
        // それを防止するために書いたif文。
        //--------------------------------
        if ((m_ProcStep == EBATTLE_UPDATE_STEP.eSTART)
        || (m_ProcStep == EBATTLE_UPDATE_STEP.eUPDATE && m_FirstStep == false))
        {

            if (is_alive_player)
            {
                //--------------------------------
                // リーダースキルによる回復
                //--------------------------------
                float fLeaderRate = 0.0f;
                float fFriendRate = 0.0f;
                BattleSceneUtil.MultiInt heal = new BattleSceneUtil.MultiInt();
                fLeaderRate = InGameUtilBattle.GetLeaderSkillHealBattle(GlobalDefine.PartyCharaIndex.LEADER);
                heal.addValue(GlobalDefine.PartyCharaIndex.MAX, InGameUtilBattle.GetHealValue(BattleParam.m_PlayerParty, fLeaderRate, false).toMultiInt());

                fFriendRate = InGameUtilBattle.GetLeaderSkillHealBattle(GlobalDefine.PartyCharaIndex.FRIEND);
                heal.addValue(GlobalDefine.PartyCharaIndex.MAX, InGameUtilBattle.GetHealValue(BattleParam.m_PlayerParty, fFriendRate, false).toMultiInt());

                //--------------------------------
                // パッシブスキルによる回復
                //--------------------------------
                float fPassiveRate = 0.0f;
                GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;  // ダミー（未使用）
                fPassiveRate = InGameUtilBattle.PassiveChkHealHPBattle(ref owner);
                heal.addValue(GlobalDefine.PartyCharaIndex.MAX, InGameUtilBattle.GetHealValue(BattleParam.m_PlayerParty, fPassiveRate, false).toMultiInt());

                //----------------------------------------
                // 回復処理
                // @change	Developer 2016/05/31 v350 回復不可[全]対応
                // @note	回復量が0だと処理されないので、0でも処理するように改修
                //			スキルが発動したかで判断する(発動していれば、rateに値が入っている)
                //----------------------------------------
                if (heal.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0
                || fLeaderRate > 0.0f
                || fFriendRate > 0.0f
                || fPassiveRate > 0.0f)
                {
                    if (BattleParam.m_BattleRequest.isBoss == true
                        && BattleParam.m_BattleRequest.isChainNextBattle == false
                        && bEnemyDestroy
                    )
                    {
                        // クエストクリア時はリジェネの回復をしない
                    }
                    else
                    {
                        BattleParam.m_PlayerParty.RecoveryHP(heal, true, true);
                    }

                    // 音再生
                    SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);
                }
            }


            //--------------------------------
            // 状態異常のターン更新処理
            //--------------------------------
            bool bUpdateStatusAilment = true;

            //--------------------------------
            // 派生戦闘の初回ターンは状態異常を更新しない
            //--------------------------------
            if (bEnemyDestroy == true && BattleParam.m_BattleRequest.isChainNextBattle == true)
            {

                bUpdateStatusAilment = false;

            }

#if true // @Change Developer 2015/08/21 v300状態異常の遅延発動対応。
            //--------------------------------
            // 敵が全滅したら状態異常遅延発動のクリア。
            // 　・派生戦闘（ボス進化）の場合はクリアしない。
            // 　・連続戦闘（パネルめくれるやつ）の場合はクリアする。
            //--------------------------------
            if (bEnemyDestroy && !BattleParam.m_BattleRequest.isChainNextBattle)
            {
                m_BattleLogicAilment.ClearDelayAilment();
            }
#endif

            //--------------------------------
            // 敵状態異常ターン更新
            //--------------------------------
            if (bUpdateStatusAilment == true)
            {

                {
                    for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
                    {

                        if (BattleParam.m_EnemyParam[i] == null)
                        {
                            continue;
                        }

                        BattleParam.m_EnemyParam[i].m_StatusAilmentChara.UpdateTurnOnce(StatusAilmentChara.UpdateTiming.eBATTLE);
                    }
                }

            }



            //----------------------------------------
            //	ターン経過数
            //----------------------------------------
            m_BattleTotalTurn = m_BattleTotalTurn + 1;

            //--------------------------------
            // トータルターン数の更新
            //--------------------------------
            BattleParam.IncrementTotalTurn();


            //--------------------------------
            // 二回目以降の更新
            //--------------------------------
            m_ProcStep = EBATTLE_UPDATE_STEP.eUPDATE;
        }

        // 初回更新フラグをおろす
        m_FirstStep = false;

        //--------------------------------
        //	乱数シード更新
        //--------------------------------
        m_EnemySkillHandCardRemake.SetRandSeed(BattleParam.GetRandFix(8888 + BattleParam.BattleRound * 1000 + m_BattleTotalTurn));
        m_ObjectCardRand.SetRandSeed(BattleParam.GetRandFix(999999 + BattleParam.BattleRound * 1000 + m_BattleTotalTurn));

        //----------------------------------------
        // プレイヤーパーティが全滅してるならゲームオーバー処理へ遷移
        // ※注意：ゲームオーバー画面でコンティニューした場合はこの後の続きから処理されるのではなく、プレイヤーフェイズ(eBATTLE_STEP_GAME_PHASE_PLAYER_START)から処理されます。
        //----------------------------------------
        if (is_alive_player == false)
        {
            // 発動スキル情報削除
            m_BattleLogicSkill.ClrAttackFix();
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_DEAD_GAMEOVER);
            return;
        }

        //--------------------------------
        // 中断復帰情報保存
        //--------------------------------
        BattleParam.SaveLocalData();

        //--------------------------------
        // チュートリアル
        //--------------------------------
        BattleSceneManager.Instance.setTutorialPhase(BattleTutorialManager.TutorialBattlePhase.RESULT);


        SwitchBoostField(EBOOST_CLEAR_MODE.eCLEAR);

        //----------------------------------------
        // 敵が生きていれば戦闘続行
        //----------------------------------------
        if (bEnemyDestroy == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_PLAYER_START);
            return;
        }


        // UIの非表示
        DisableAllWindow();

        //----------------------------------------
        // ゲームクリア
        //----------------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_CLEAR);
        return;
    }

    /// <summary>
    /// バトルアチーブのチェック
    /// </summary>
    /// <param name="is_add_skill_turn"></param>
    private void checkStats(bool is_add_skill_turn, out int play_score_achieve_count)
    {
        play_score_achieve_count = 0;
        if (BattleParam.IsKobetsuHP)
        {
            int skill_turn = 0;

            if (m_SkillTurnCondition.m_1CardSkill > 0
                && m_Stats_NPanelSkillCount[0] > 0)
            {
                BattleParam.m_PlayerParty.m_BattleAchive.m_Panel1Skill = true;
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_Panel1Skill)
            {
                skill_turn += m_SkillTurnCondition.m_1CardSkill;
                play_score_achieve_count++;
            }

            if (m_SkillTurnCondition.m_2CardSkill > 0
                && m_Stats_NPanelSkillCount[1] > 0)
            {
                BattleParam.m_PlayerParty.m_BattleAchive.m_Panel2Skill = true;
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_Panel2Skill)
            {
                skill_turn += m_SkillTurnCondition.m_2CardSkill;
                play_score_achieve_count++;
            }

            if (m_SkillTurnCondition.m_3CardSkill > 0
                && m_Stats_NPanelSkillCount[2] > 0)
            {
                BattleParam.m_PlayerParty.m_BattleAchive.m_Panel3Skill = true;
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_Panel3Skill)
            {
                skill_turn += m_SkillTurnCondition.m_3CardSkill;
                play_score_achieve_count++;
            }

            if (m_SkillTurnCondition.m_4CardSkill > 0
                && m_Stats_NPanelSkillCount[3] > 0)
            {
                BattleParam.m_PlayerParty.m_BattleAchive.m_Panel4Skill = true;
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_Panel4Skill)
            {
                skill_turn += m_SkillTurnCondition.m_4CardSkill;
                play_score_achieve_count++;
            }

            if (m_SkillTurnCondition.m_5CardSkill > 0
                && m_Stats_NPanelSkillCount[4] > 0)
            {
                BattleParam.m_PlayerParty.m_BattleAchive.m_Panel5Skill = true;
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_Panel5Skill)
            {
                skill_turn += m_SkillTurnCondition.m_5CardSkill;
                play_score_achieve_count++;
            }

            for (int idx = m_SkillTurnCondition.m_ComboCount1; idx < m_Stats_NHandsCount.Length; idx++)
            {
                if (m_Stats_NHandsCount[idx] > 0)
                {
                    BattleParam.m_PlayerParty.m_BattleAchive.m_Combo5 = true;
                    break;
                }
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_Combo5)
            {
                skill_turn++;
                play_score_achieve_count++;
            }

            for (int idx = m_SkillTurnCondition.m_ComboCount2; idx < m_Stats_NHandsCount.Length; idx++)
            {
                if (m_Stats_NHandsCount[idx] > 0)
                {
                    BattleParam.m_PlayerParty.m_BattleAchive.m_Combo10 = true;
                    break;
                }
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_Combo10)
            {
                skill_turn++;
                play_score_achieve_count++;
            }

            if (m_SkillTurnCondition.m_FullField && m_Stats_FullFieldCount > 0)
            {
                BattleParam.m_PlayerParty.m_BattleAchive.m_FullField = true;
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_FullField)
            {
                skill_turn++;
                play_score_achieve_count++;
            }

            if (m_SkillTurnCondition.m_BoostSkill && m_Stats_BoostSkillCount > 0)
            {
                BattleParam.m_PlayerParty.m_BattleAchive.m_Boost = true;
            }
            if (BattleParam.m_PlayerParty.m_BattleAchive.m_Boost)
            {
                skill_turn++;
                play_score_achieve_count++;
            }

            if (m_Stats_LimitBreakWin)
            {
                skill_turn += m_SkillTurnCondition.m_LimitBreakWin;
            }

            skill_turn += m_SkillTurnCondition.m_BattleEnd;

            if (is_add_skill_turn)
            {
                for (int i = 0; i < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); i++)
                {
                    CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)i, CharaParty.CharaCondition.SKILL_TURN2);
                    if (chara != null)
                    {
                        chara.AddCharaLimitBreak(skill_turn);
                    }
                }

                BattleParam.m_PlayerParty.m_BattleAchive.resetAll();
            }
            else
            {
                play_score_achieve_count = 0;
            }

            m_DispCaption.updateAchieve();
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：バトルクリア
    */
    //----------------------------------------------------------------------------
    void OnGameClear()
    {
        // ダメージ描画終了を待つ
        if (BattleParam.m_PlayerParty.isShowingDamageNumber())
        {
            return;
        }

        Animation cAnimationEnemy = null;
        Animation cAnimationField = null;

        //--------------------------------
        // バトル関連UIの無効化
        //--------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ 敵パーティ撃破 ]]]]]");

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ パネル出現数 ]]]]]  ※出現パネル変換スキル適用後の出現数です");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.NAUGHT.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprNAUGHT");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.FIRE.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprFIRE");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.WATER.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprWATER");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.LIGHT.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprLIGHT");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.DARK.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprDARK");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.WIND.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprWIND");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.HEAL.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprHEAL");

            if (BattleParam.m_BattleRequest.isChainNextBattle == true)
            {
                //--------------------------------
                // 進化演出1
                //--------------------------------
                m_BattleDispEnemy.showEvolve1();
            }

            if (BattleParam.m_BattleRequest.isBoss == true)
            {
                //--------------------------------
                // ボス戦専用処理
                //--------------------------------
                if (BattleParam.m_BattleRequest.isChainNextBattle == true)
                {
                }
                else
                {
                }


                // 中断復帰情報を破棄
                BattleParam.SaveLocalData();

            }
            else
            {
                //--------------------------------
                // 雑魚戦専用処理
                //--------------------------------
                if (BattleParam.m_BattleRequest.isChainNextBattle == true)
                {
                }
            }

            //--------------------------------
            // 敵リアクション：判定スキルIDをクリア
            //--------------------------------
            m_EnemyReactChkSkillID = 0;

            //--------------------------------
            // 敵連続行動：行動IDをクリア
            // 敵テーブル切り替え時：行動フラグをクリア
            //--------------------------------
            m_BattleLogicEnemy.resetActionTableProgress();

            //----------------------------------------
            // 統計情報を反映（進化するときはスキルターンの短縮はしない）
            //----------------------------------------
            bool is_add_skill_turn = (BattleParam.m_BattleRequest.isChainNextBattle == false);
            int play_score_achive_count = 0;
            checkStats(is_add_skill_turn, out play_score_achive_count);

            // プレイスコア関連
            if (BattleParam.m_PlayScoreInfo != null)
            {
                // 敵進化時は play_score_achive_count はゼロだが、プレイスコアの計算では敵進化も１バトルとカウントするので endBattle() を呼ぶ.
                BattleParam.m_PlayScoreInfo.endBattle(play_score_achive_count);
            }

        }

        //--------------------------------
        // モーション終了判定
        //--------------------------------
        if (cAnimationEnemy
        && cAnimationEnemy.isPlaying
        )
        {
            return;
        }
        if (cAnimationField
        && cAnimationField.isPlaying
        )
        {
            return;
        }

        if (BattleParam.IsTutorial())
        {
            BattleSceneManager.Instance.setTutorialPhase(BattleTutorialManager.TutorialBattlePhase.CREAR);

            if (BattleSceneManager.Instance.isWaitTutorial())
            {
                return;
            }
        }

        endBattle();

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_WAIT_UPDATE);
        return;
    }

    public void endBattle()
    {
        DebugBattleLog.flush();

        BattleSceneManager.Instance.setBattleScenePhase(BattleSceneManager.BattleScenePhase.NOT_BATTLE);

        m_ProcStep = EBATTLE_UPDATE_STEP.eSTART;
        m_ProcStep_player = EBATTLE_UPDATE_STEP.eSTART;
        m_FirstStep = true;
        m_FirstStep_player = true;

        //--------------------------------
        // バトル終了
        //--------------------------------
        m_BattleActive = false;
        m_InitialPhase = InitialPhase.QUEST_INIT;
        BattleParam.m_BattleRequest = null;

        // ドロップユニットアイコンの削除
        m_BattleDispEnemy.hideDropObject();

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_WAIT_UPDATE);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：死亡時：ゲームオーバー
    */
    //----------------------------------------------------------------------------
    void OnDeadGameOver()
    {
        //----------------------------------------
        // チュートリアル
        //----------------------------------------
        if (BattleParam.IsTutorial())
        {
            BattleSceneManager.Instance.setTutorialPhase(BattleTutorialManager.TutorialBattlePhase.GAME_OVER);
            if (BattleSceneManager.Instance.isWaitTutorial())
            {
                return;
            }

            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_DEAD_CONTINUE);
            return;
        }

        //--------------------------------
        // 無限ダンジョンであればリザルトへ
        //--------------------------------
        if (BattleParam.ChkNoContinueQuest(BattleParam.m_QuestMissionID) == false)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_DEAD_GAMEOVER_WAIT);
            return;
        }

        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ 味方全滅 ]]]]]");

            // オートプレイ停止
            BattleParam.setAutoPlayState(BattleParam.AutoPlayState.OFF);

            //--------------------------------
            // 中断復帰データの作成
            //--------------------------------
            BattleParam.SaveLocalData();

            // コンティニューダイアログを表示
            BattleParam.showContinueDialog();
        }

        // コンティニューダイアログ結果待ち
        BattleParam.ContinueDialogResult continue_dialog_result = BattleParam.getContinueDialogResult();
        switch (continue_dialog_result)
        {
            case BattleParam.ContinueDialogResult.CONTINUE:
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_DEAD_CONTINUE);
                return;

            case BattleParam.ContinueDialogResult.GAMEOVER:
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_DEAD_GAMEOVER_WAIT);
                return;
        }

        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：死亡時：ゲームオーバー演出待ち
    */
    //----------------------------------------------------------------------------
    void OnDeadGameOverWait()
    {
        //--------------------------------
        // ゲームオーバーUIのアニメーション開始
        //--------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ ゲームオーバー ]]]]]");

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ パネル出現数 ]]]]]  ※出現パネル変換スキル適用後の出現数です");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "===========================================================");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.NAUGHT.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprNAUGHT");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.FIRE.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprFIRE");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.WATER.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprWATER");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.LIGHT.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprLIGHT");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.DARK.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprDARK");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.WIND.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprWIND");
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "   " + MasterDataDefineLabel.ElementType.HEAL.ToString() + ":", false);
            DebugBattleLog.writeValue("PnlAprHEAL");

            // SE再生：ゲームオーバー
            SoundUtil.PlaySE(SEID.SE_STARGE_GAMEOVER);

            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_DEAD_RETIRE);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：死亡時：バトルコンティニュー
    */
    //----------------------------------------------------------------------------
    void OnDeadContinue()
    {
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ コンティニュー ]]]]]");

        //--------------------------------
        // 回復して再開
        //--------------------------------
        if (BattleParam.m_PlayerParty != null)
        {
            BattleParam.m_PlayerParty.ContinueRecovery();

            // ヒーロースキルターン回復
            BattleParam.m_PlayerParty.m_BattleHero.addSkillTrun(9999999, false);

            //--------------------------------
            // LBSコストMAX対応
            // @add Developer v330 2016/02/22
            //--------------------------------
            for (int num = 0; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
            {
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.EXIST);
                if (chara_once == null)
                {
                    continue;
                }

                chara_once.MaxCharaLimitBreak();
            }
        }

        //--------------------------------
        // 状態異常強制回復
        //--------------------------------
        BattleParam.m_PlayerParty.m_Ailments.DelAllStatusAilment(GlobalDefine.PartyCharaIndex.MAX);

        BattleClearField();
        BattleClearHand(false);

        m_FirstStep_player = false;

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_PLAYER_START);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：死亡時：バトルリタイア
    */
    //----------------------------------------------------------------------------
    void OnDeadRetire()
    {

        //--------------------------------
        // 外部からチェックしての移行を期待してバトルステップは遷移しない
        //--------------------------------
        return;

    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：リミットブレイクスキル：演出開始処理
    */
    //----------------------------------------------------------------------------
    void OnLimitBreak()
    {
        if (m_NextLimitBreakSkillFixID != 0)
        {
            // リミブレ連続発動中は演出なし
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK_WAIT);
            return;
        }


        //--------------------------------
        //	初回処理
        //--------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            // 敵の「根性」を設定
            m_BattleLogicEnemy.setKonjo();

            // タイトル表示
            m_TextCutinArea.startAnim(TextCutinViewControl.TitleType.STAND_READY);

            // SE再生：リーダースキルパワーアップ
            SoundUtil.PlaySE(SEID.SE_INGAME_LEADERSKILL);
            SoundUtil.PlaySE(SEID.VOICE_INGAME_QUEST_STANDREADY);

            // リーチ線情報を初期化
            initSkillReachInfo();

            return;
        }


        //--------------------------------
        // アニメーション
        //--------------------------------
        if (m_TextCutinArea.isPlaying())
        {
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK_WAIT);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：リミットブレイクスキル：演出待機処理
    */
    //----------------------------------------------------------------------------
    void OnLimitBreakWait()
    {

        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            BattleSkillActivity skill_activity = null;
            bool is_show_cutin = true;

            if (m_NextLimitBreakSkillFixID == 0)
            {
                // 先頭のスキル

                // タイトル切り替え
                m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LBS_SKILL);

                if (m_NextLimitBreakSkillCaster != GlobalDefine.PartyCharaIndex.HERO)
                {
                    // リミブレスキル
                    CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(m_NextLimitBreakSkillCaster, CharaParty.CharaCondition.SKILL_LIMITBREAK);
                    if (chara == null)
                    {
                        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK_END);
                        return;
                    }
                    skill_activity = InGameUtilBattle.ActivityLimitBreakSkill(m_NextLimitBreakSkillCaster, -1, chara.m_CharaMasterDataParam.skill_limitbreak);

                    //	コスト消費
                    InGameUtil.DelLimitBreakSkillCost(BattleParam.m_PlayerParty, m_NextLimitBreakSkillCaster);

                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ リミットブレイクスキル発動"
                        + " 発動者:" + m_NextLimitBreakSkillCaster.ToString()
                        + " スキルFixID:" + chara.m_CharaMasterDataParam.skill_limitbreak.ToString()
                        + " ]]]]]"
                    );
                }
                else
                {
                    // ヒーロースキル
                    skill_activity = BattleParam.m_PlayerParty.m_BattleHero.getHeroSkillActivity();

                    //	コスト消費
                    BattleParam.m_PlayerParty.m_BattleHero.resetSkillTurn();

                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ 主人公スキル発動 ]]]]]");
                }

                is_show_cutin = true;
            }
            else
            {
                // ２番目以降のスキル
                skill_activity = InGameUtilBattle.ActivityLimitBreakSkill(m_NextLimitBreakSkillCaster, -1, m_NextLimitBreakSkillFixID);
                is_show_cutin = false;

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ 連結リミットブレイクスキル発動"
                    + " スキルFixID:" + m_NextLimitBreakSkillFixID.ToString()
                    + " ]]]]]"
                );
            }

            BattleParam.m_SkillRequestLimitBreak.clearRequest();
            BattleParam.m_SkillRequestLimitBreak.addSkillRequest(skill_activity);

            //----------------------------------------
            // 敵リアクション：判定スキルIDを保存
            //----------------------------------------
            if (m_NextLimitBreakSkillCaster != GlobalDefine.PartyCharaIndex.HERO)
            {
                m_EnemyReactChkSkillID = skill_activity.getMasterDataSkillLimitBreak().fix_id;
            }
            else
            {
                m_EnemyReactChkSkillID = 0;
            }

            // 連結スキル
            m_NextLimitBreakSkillFixID = (uint)skill_activity.getMasterDataSkillLimitBreak().add_fix_id;

            // カットイン発行
            BattleSkillCutinManager.Instance.ClrSkillCutin();
            BattleSkillCutinManager.Instance.SetSkillCutin(BattleParam.m_SkillRequestLimitBreak);
            BattleSkillCutinManager.Instance.CutinStart2(true, false, !is_show_cutin);  //リミットブレイク

            return;
        }

        if (m_BattleLogicSkill.SkillUpdate(BattleParam.m_SkillRequestLimitBreak) == false)
        {
            return;
        }


        // カットイン終了待ち
        if (BattleSkillCutinManager.Instance.isRunning() == true)
        {
            return;
        }

        // ダメージ描画中は進まない
        if (DrawDamageManager.isShowing())
        {
            return;
        }

        // 敵の「根性」を実行
        m_BattleLogicEnemy.execKonjo();

        //----------------------------------------
        // 敵の死亡処理
        //----------------------------------------
        if (m_NextLimitBreakSkillFixID == 0)
        {
            m_BattleLogicEnemy.DeadEnemy();
        }

        //----------------------------------------
        //	スキル発行情報リセット
        //----------------------------------------
        m_BattleLogicSkill.ResetSkillRequestLimitBreak();

        // 敵死亡エフェクト描画中は進まない
        if (EffectManager.Instance.isPlayingEffect(EFFECT_HANDLE_ENEMY_DEATH))
        {
            return;
        }

        // ドロップアニメーション中は進まない
        if (EffectManager.Instance.isPlayingEffect(EFFECT_HANDLE_UNIT_DROP) == true)
        {
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK_END);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：リミットブレイクスキル：演出終了処理
    */
    //----------------------------------------------------------------------------
    void OnLimitBreakEnd()
    {

        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            m_DispCaption.hideCaption();

            return;
        }

        // リミブレ連続発動中は敵の全滅判定をしない
        if (m_NextLimitBreakSkillFixID != 0)
        {
            //----------------------------------------
            // 敵リアクション開始へ移行
            //----------------------------------------
            if (m_EnemyReactChkSkillID != 0)
            {
                BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION_START);
                return;
            }

            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT);
            return;
        }

        //----------------------------------------
        // 敵が全滅してるならクリア。全滅してないなら入力画面へ遷移
        //----------------------------------------
        bool bActiveEnemy = false;
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {
            if (BattleParam.m_EnemyParam[i] == null)
            {
                continue;
            }

            if (BattleParam.m_EnemyParam[i].m_EnemyHP <= 0)
            {
                continue;
            }

            bActiveEnemy = true;
            break;
        }

        if (bActiveEnemy == false)
        {
            if (BattleParam.m_BattleRequest.isChainNextBattle == false)
            {
                m_Stats_LimitBreakWin = true;
            }
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_CLEAR);
            return;
        }

        //----------------------------------------
        // 敵リアクション開始へ移行
        //----------------------------------------
        if (m_EnemyReactChkSkillID != 0)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION_START);
            return;
        }

        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT);
        return;
    }




    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：敵リアクション開始
    */
    //----------------------------------------------------------------------------
    void OnGameEnemyReactionStart()
    {
        //----------------------------------------
        // 敵が全滅していた場合、処理しない
        //----------------------------------------
        if (m_BattleLogicEnemy.IsEnemyDestroyAll() == true)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT);
            return;
        }

        //----------------------------------------
        // 敵特性チェック
        //----------------------------------------
        bool enemyReaction = EnemyAbility.AbilitySpecificActionReaction(m_EnemyReactChkSkillID, BattleParam.m_EnemyParam);

        //----------------------------------------
        // 敵リアクションがない場合
        //----------------------------------------
        if (enemyReaction == false)
        {
            // 判定スキルIDをクリア
            m_EnemyReactChkSkillID = 0;

            // プレイヤー入力実行処理へ移行
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT);
            return;
        }

        //----------------------------------------
        // キャプション：エネミーフェーズ
        //----------------------------------------
        m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.ENEMY_PHASE);

        //----------------------------------------
        // 敵リアクション処理へ移行
        //----------------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION);
        return;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：敵リアクション処理
    */
    //----------------------------------------------------------------------------
    void OnGameEnemyReaction()
    {
        //----------------------------------------
        // カウンター処理から、戻ってきた場合
        // @add Developer 2015/12/16 v320
        //----------------------------------------
        if (BattleLogicFSM.Instance.getPrevStep() == EBATTLE_STEP.eBATTLE_STEP_GAME_COUNTER_PLAYER)
        {
            // 初回処理防止
            BattleLogicFSM.Instance.isChangeStepTriger(true);
        }

        //----------------------------------------
        // 初回処理
        //----------------------------------------
        if (BattleLogicFSM.Instance.isChangeStepTriger(true) == true)
        {
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "[[[[[ 敵リアクション ]]]]]");

            //----------------------------------------
            // 敵攻撃関連処理：事前処理
            //----------------------------------------
            m_BattleLogicEnemy.EnemyAttackAllPreProc();
        }


        //----------------------------------------
        // カウンター処理が必要であればカウンター処理へ移行
        // @add Developer 2015/12/16 v320
        //----------------------------------------
        if (m_SkillCounter == true)
        {
            //----------------------------------------
            //	動きが速すぎるので、適当に待つ
            //----------------------------------------
            if (m_BattleLogicEnemy.isWaitingActionTimer())
            {
                return;
            }

            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_COUNTER_PLAYER);
            return;
        }

        //----------------------------------------
        // 敵リアクション行動処理：メイン処理
        //----------------------------------------
        if (m_BattleLogicEnemy.EnemyAttackReaction(true) == false)
        {
            return;
        }

        //----------------------------------------
        // 敵リアクション行動処理：後処理
        //----------------------------------------
        if (m_BattleLogicEnemy.EnemyAttackAllPostProc() == false)
        {
            return;
        }

        //----------------------------------------
        // 敵リアクション用変数初期化
        //----------------------------------------
        EnemyAbility.InitReaction();

        //----------------------------------------
        // 敵リアクション終了へ移行
        //----------------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION_END);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		戦闘処理ステップ：ゲーム：敵リアクション終了
    */
    //----------------------------------------------------------------------------
    void OnGameEnemyReactionEnd()
    {
        //----------------------------------------
        // 判定スキルIDをクリア
        //----------------------------------------
        m_EnemyReactChkSkillID = 0;

        //----------------------------------------
        // 死亡しているプレイヤーパーティメンバーの状態異常を解除
        //----------------------------------------
        BattleParam.m_PlayerParty.clearAilmentDeadMember();

        //リミブレ連続は出し切るまでゲームオーバー・ゲームクリアへ遷移しない
        if (m_NextLimitBreakSkillFixID != 0)
        {
            //----------------------------------------
            // プレイヤー入力実行処理へ移行
            //----------------------------------------
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT);
            return;
        }

        //----------------------------------------
        // @add Developer 2015/10/30 v300 bugweb 3214対応
        // プレイヤーパーティが全滅しているならゲームオーバー処理へ遷移
        // ここを通る場合ノーマルスキルは発動されていないためスキル情報削除は行わない
        // StepFuncGameInput内の判定ではクリア処理が先行しているので、
        // カウンターで相打ちした場合、他と挙動が変わるためここで判定を行う
        //----------------------------------------
        if (BattleParam.m_PlayerParty.m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MAX) <= 0)
        {
            BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_DEAD_GAMEOVER);
            return;
        }

        //----------------------------------------
        // プレイヤー入力実行処理へ移行
        //----------------------------------------
        BattleLogicFSM.Instance.step(EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT);
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：エラー
    */
    //----------------------------------------------------------------------------
    void OnError()
    {
        return;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	UI更新
    */
    //----------------------------------------------------------------------------
    private void UIUpdate()
    {

        //----------------------------------------
        // ターゲットしている相手にカーソルを付ける
        //----------------------------------------
        UIUpdateTargetWindow();
    }

    /// <summary>
    /// 敵エリアをタッチした時の処理
    /// </summary>
    private void touchEnemyArea()
    {
        if (m_InputCountDownStart == false)
        {
            // カウントダウン中でなければ敵ターゲット

            if (BattleParam.m_TargetEnemyWindow == InGameDefine.SELECT_NONE)
            {
                // 長押し(ウィンドウ表示)
                int touch_enemy_index = m_BattleDispEnemy.checkTouchEnemy(true);
                if (touch_enemy_index >= 0)
                {
                    BattleParam.m_TargetEnemyWindow = touch_enemy_index;
                }
            }

            if (BattleParam.m_TargetEnemyWindow == InGameDefine.SELECT_NONE)
            {
                // タップ(ターゲット切り替え)
                int touch_enemy_index = m_BattleDispEnemy.checkTouchEnemy(false);
                if (touch_enemy_index >= 0)
                {
                    if (touch_enemy_index != BattleParam.m_TargetEnemyCurrent)
                    {
                        BattleParam.m_TargetEnemyCurrent = touch_enemy_index;
                    }
                    else
                    {
                        // ターゲット中の敵をタッチしたら解除
                        BattleParam.m_TargetEnemyCurrent = -1;
                    }
                    m_BattleDispEnemy.showTargetCursor(BattleParam.m_TargetEnemyCurrent);
                }
            }
        }
        else
        {
            // カウントダウン中はカウントダウン短縮
            if (BattleParam.IsTutorial() == false)  //チュートリアル中は短縮できないようにする
            {
                if (m_BattleDispEnemy.isTappedEnemyArea())
                {
                    skipCountDown();
                }
            }
        }
    }

    public void skipCountDown()
    {
        m_InputCountDown = InGameDefine.COUNTDOWN_TIME_MIN;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		ターゲットウィンドウの更新
    */
    //----------------------------------------------------------------------------
    private void UIUpdateTargetWindow()
    {
        //----------------------------------------
        // ターゲット済みで、ターゲット対象が死亡している
        //----------------------------------------
        if (BattleParam.m_TargetEnemyCurrent != InGameDefine.SELECT_NONE)
        {
            if (BattleParam.m_EnemyParam.Length <= BattleParam.m_TargetEnemyCurrent
            || BattleParam.m_EnemyParam[BattleParam.m_TargetEnemyCurrent].isDead() == true)
            {
                BattleParam.m_TargetEnemyCurrent = InGameDefine.SELECT_NONE;
                m_BattleDispEnemy.hideTargetCursor();
            }
        }

        // 敵エリアをタッチしたときの処理
        touchEnemyArea();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘情報クリア：場再構築
    */
    //----------------------------------------------------------------------------
    private void BattleClearField()
    {
        m_BattleCardManager.reset(true);
    }

    public void FillHandCard()
    {
        // 手札分と次カード分の２回をカード補給
        for (int idx2 = 0; idx2 < 2; idx2++)
        {
            for (int idx = 0; idx < m_BattleCardManager.m_HandArea.getCardMaxCount(); idx++)
            {
                BattleScene.BattleCard battle_card = m_BattleCardManager.m_HandArea.getCard(idx);
                if (battle_card == null)
                {
                    MasterDataDefineLabel.ElementType element_type = getRandomAndSkilledCardElement();
                    m_BattleCardManager.addNewCardInHandArea(idx, element_type);
                }
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘情報クリア：手札再構築
    */
    //----------------------------------------------------------------------------
    public void BattleClearHand(bool bHandClear)
    {
        m_BattleCardManager.reset(!bHandClear);
        FillHandCard();
    }

    //　スキルリーチ情報を初期化
    private void initSkillReachInfo()
    {
        if (m_WorkHandAreaElements == null)
        {
            m_WorkHandAreaElements = new MasterDataDefineLabel.ElementType[m_BattleCardManager.m_HandArea.getCardMaxCount()];
        }
        for (int hand_area_index = 0; hand_area_index < m_WorkHandAreaElements.Length; hand_area_index++)
        {
            BattleScene.BattleCard battle_card = m_BattleCardManager.m_HandArea.getCard(hand_area_index);
            if (battle_card != null)
            {
                m_WorkHandAreaElements[hand_area_index] = battle_card.getElementType();
            }
            else
            {
                m_WorkHandAreaElements[hand_area_index] = MasterDataDefineLabel.ElementType.NONE;
            }
        }

        m_BattleSkillReachInfo.resetReachInfo(m_WorkHandAreaElements);

        for (int hand_card_index = 0; hand_card_index < m_BattleCardManager.m_InHandArea.getCardCount(); hand_card_index++)
        {
            BattleScene.BattleCard battle_card = m_BattleCardManager.m_InHandArea.getCard(hand_card_index);
            if (battle_card != null)
            {
                m_WorkHandAreaElements[battle_card.m_HandCardIndex] = battle_card.getElementType();
            }
        }

        m_BattleCardManager.setReachInfo(m_BattleSkillReachInfo);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	攻撃リクエスト管理：リクエストクリア
    */
    //----------------------------------------------------------------------------
    private void ClrSkillRequest()
    {
        //--------------------------------
        // リンクスキル
        //--------------------------------
        if (m_BattleLogicSkill.m_SkillLink.linkReqInputNum > 0)
        {
            m_BattleLogicSkill.m_SkillLink.ResetSkillRequestLink();
        }
    }

    // スキルリーチ情報を追加
    private void addSkillReachInfo(int field_index, MasterDataDefineLabel.ElementType element_type)
    {
        m_BattleSkillReachInfo.addReachInfo(field_index, element_type);
    }



    //----------------------------------------------------------------------------
    /*!
        @brief		スキルエフェクト操作：エフェクト更新
        @param[in]	BattleSkillActivity		(cSkillActivity)		スキル発動情報
    */
    //----------------------------------------------------------------------------
    public bool AddSkillEffect(BattleSkillActivity cSkillActivity)
    {
        MasterDataDefineLabel.UIEffectType effectId = MasterDataDefineLabel.UIEffectType.NONE;
        MasterDataDefineLabel.SkillType type = MasterDataDefineLabel.SkillType.ERROR;
        GlobalDefine.PartyCharaIndex charaID = cSkillActivity.m_SkillParamOwnerNum;

        //----------------------------------------
        // 通常時と追撃時で属性の参照先を変更
        //----------------------------------------
        if (cSkillActivity.m_SkillType == ESKILLTYPE.eLEADER
        && (charaID == GlobalDefine.PartyCharaIndex.LEADER || charaID == GlobalDefine.PartyCharaIndex.FRIEND))
        {
            //----------------------------------------
            // 追撃処理
            //----------------------------------------
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(charaID, CharaParty.CharaCondition.SKILL_LEADER);
            if (chara_once != null)
            {

                uint unSkillID = chara_once.m_CharaMasterDataParam.skill_leader;
                MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(unSkillID);
                if (skillLeader != null && skillLeader.Is_skill_follow_atk_active())
                {

                    effectId = skillLeader.skill_follow_atk_effect;

                    // SE再生：暫定
                    InGameUtilBattle.PlayActiveSkillSE(MasterDataDefineLabel.SkillType.ATK_ALL, cSkillActivity.m_Element);
                }
            }

            type = MasterDataDefineLabel.SkillType.ATK_ALL;
        }
        else
        {
            //----------------------------------------
            // 通常攻撃
            //----------------------------------------
            effectId = cSkillActivity.m_Effect;

            bool is_play_se = true;
            {
                MasterDataSkillLimitBreak master_data_skill_limit_break = cSkillActivity.getMasterDataSkillLimitBreak();
                if (master_data_skill_limit_break != null)
                {
                    {
                        // 何の効果もない連結リミブレスキル(連結スキルの始動用スキル)の場合はＳＥを鳴らさない
                        if (master_data_skill_limit_break.add_fix_id != 0
                            && master_data_skill_limit_break.skill_cate == MasterDataDefineLabel.SkillCategory.ATK
                            && master_data_skill_limit_break.skill_effect == MasterDataDefineLabel.UIEffectType.NONE
                            && master_data_skill_limit_break.Is_skill_damage_enable() == false
                            && master_data_skill_limit_break.status_ailment_target == MasterDataDefineLabel.TargetType.NONE
                        )
                        {
                            is_play_se = false;
                        }
                    }

                    if (is_play_se)
                    {
                        // 蘇生効果リミブレスキルで効果対象がない場合はＳＥを鳴らさない
                        if (master_data_skill_limit_break.skill_type == MasterDataDefineLabel.SkillType.RESURRECT)
                        {
                            // 死亡メンバーの有無を調べる（リミブレスキル効果発揮前なのでこの調べ方で大丈夫）
                            CharaOnce[] dead_party_member = BattleParam.m_PlayerParty.getPartyMembers(CharaParty.CharaCondition.DEAD);
                            if (dead_party_member.IsNullOrEmpty())
                            {
                                is_play_se = false;
                            }
                        }
                    }
                }
            }

            // SE再生：暫定
            if (is_play_se)
            {
                InGameUtilBattle.PlayActiveSkillSE(cSkillActivity.m_Type, cSkillActivity.m_Element);
            }

            type = cSkillActivity.m_Type;
        }

        if (effectId == MasterDataDefineLabel.UIEffectType.NONE)
        {
#if UNITY_EDITOR
            Debug.LogError("BattleAttack Effect Object None!");
#endif // #if UNITY_EDITOR
            return false;
        }

        bool bEnemySide = true;

        //----------------------------------------
        // エフェクト表示位置
        //----------------------------------------
        switch (type)
        {
            case MasterDataDefineLabel.SkillType.ATK_ONCE: bEnemySide = true; break;    // スキルタイプ：単体攻撃
            case MasterDataDefineLabel.SkillType.ATK_ALL: bEnemySide = true; break; // スキルタイプ：全体攻撃
            case MasterDataDefineLabel.SkillType.CURSE_ONCE: bEnemySide = true; break;
            case MasterDataDefineLabel.SkillType.CURSE_ALL: bEnemySide = true; break;
            case MasterDataDefineLabel.SkillType.HEAL: bEnemySide = false; break;   // スキルタイプ：回復
            case MasterDataDefineLabel.SkillType.HEAL_SP: bEnemySide = false; break;    // スキルタイプ：回復SP
            case MasterDataDefineLabel.SkillType.SUPPORT: bEnemySide = false; break;    // スキルタイプ：補助
            case MasterDataDefineLabel.SkillType.RESURRECT: bEnemySide = false; break;  // スキルタイプ：復活
            default:
                bEnemySide = true;
                break;
        }

        //----------------------------------------
        // 敵メッシュ座標を取得
        //----------------------------------------
        int nEnemyIdx = -1;
        if (bEnemySide == true)
        {

            //----------------------------------------
            //	敵ターゲット情報からエフェクト表示位置を決める
            //----------------------------------------
            if (cSkillActivity != null)
            {
                if (cSkillActivity.m_SkillParamTarget != null)
                {
                    if (cSkillActivity.m_SkillParamTarget[0] != null)
                    {
                        for (int i = 0; i < cSkillActivity.m_SkillParamTarget.Length; i++)
                        {

                            if (cSkillActivity.m_SkillParamTarget[i] == null)
                            {
                                continue;
                            }

                            nEnemyIdx = cSkillActivity.m_SkillParamTarget[i].m_TargetNum;
                            if (nEnemyIdx < 0)
                            {
                                nEnemyIdx = 0;
                            }
                            break;
                        }
                    }
                }
            }
        }

        //----------------------------------------
        // スキルに合わせてエフェクト生成
        //----------------------------------------
        if (type == MasterDataDefineLabel.SkillType.HEAL)
        {
        }
        else if (type == MasterDataDefineLabel.SkillType.HEAL_SP)
        {
        }
        else if (type == MasterDataDefineLabel.SkillType.RESURRECT)
        {
        }
        else
        {

            //----------------------------------------
            //	指定位置への表示
            //----------------------------------------
            m_BattleDispEnemy.playDamageEffect(effectId, nEnemyIdx, bEnemySide);
        }

        return false;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief			ブーストフィールドの表示切り換え
        @param[in]		bool		(clear)		表示のクリア/通常処理
    */
    //----------------------------------------------------------------------------
    public void SwitchBoostField(EBOOST_CLEAR_MODE mode)
    {

        //----------------------------------------
        //	全ブーストパネルのフラグをクリア
        //----------------------------------------
        for (int i = 0; i < m_abBoostField.Length; i++)
        {
            m_abBoostField[i] = false;
        }

        int nRandBoostNum;
        int nBoostChance = 1;


        //----------------------------------------
        //	パッシブスキルによるブーストパネル抽選回数増加
        //----------------------------------------
        nBoostChance = nBoostChance + InGameUtilBattle.PassiveChkAddBoostChance();

        //----------------------------------------
        //	動作モード毎に処理
        //----------------------------------------
        switch (mode)
        {
            case EBOOST_CLEAR_MODE.eNORMAL:
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "場のブースト抽選", false);

                //----------------------------------------
                //	通常のブースト抽選
                //----------------------------------------
                for (int n = 0; n < nBoostChance; n++)
                {

                    //----------------------------------------
                    //	ブーストパネル抽選
                    //----------------------------------------
                    nRandBoostNum = (int)RandManager.GetRand(0, 10);

                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + " 場:" + nRandBoostNum + "をBOOST ", false);

                    if (nRandBoostNum < 0
                    || nRandBoostNum >= m_abBoostField.Length)
                    {
                        continue;
                    }

                    m_abBoostField[nRandBoostNum] = true;
                }
                DebugBattleLog.newLine();
                break;

            case EBOOST_CLEAR_MODE.eALL:
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "全ての場をBOOSTに");
                //----------------------------------------
                //	すべてブースト
                //----------------------------------------
                for (int i = 0; i < m_abBoostField.Length; i++)
                {
                    m_abBoostField[i] = true;
                }
                break;

            case EBOOST_CLEAR_MODE.eCLEAR:
            default:
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "場のBOOSTをクリア");
                //----------------------------------------
                //	すべてノーマル
                //----------------------------------------
                break;
        }


        //----------------------------------------
        // ブーストパネルの復帰
        //----------------------------------------
        if (mode == EBOOST_CLEAR_MODE.eNORMAL)
        {

            RestoreBattle restore_battle = BattleParam.getRestoreData();
            if (restore_battle != null)
            {

                if (restore_battle.m_BattleBoost != null)
                {
                    bool[] boostPanel = restore_battle.m_BattleBoost;
                    if (boostPanel != null)
                    {
                        for (int n = 0; n < boostPanel.Length; n++)
                        {
                            m_abBoostField[n] = boostPanel[n];
                        }
                        restore_battle.m_BattleBoost = null;
                    }

                }
            }
        }

#if BUILD_TYPE_DEBUG
        // デバッグ機能(BOOST固定)
        for (int idx = 0; idx < m_abBoostField.Length; idx++)
        {
            m_abBoostField[idx] |= m_abBoostFieldDebugKeep[idx];
        }
#endif

        //----------------------------------------
        //	表示の更新
        //----------------------------------------
        for (int i = 0; i < m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax(); i++)
        {

            m_BattleCardManager.m_FieldAreas.getFieldArea(i).m_IsBoost = m_abBoostField[i];
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "場のBOOST状況 "
            + "[" + (m_abBoostField[0] ? "B" : " ") + "]"
            + "[" + (m_abBoostField[1] ? "B" : " ") + "]"
            + "[" + (m_abBoostField[2] ? "B" : " ") + "]"
            + "[" + (m_abBoostField[3] ? "B" : " ") + "]"
            + "[" + (m_abBoostField[4] ? "B" : " ") + "]"
        );

    }



    //----------------------------------------------------------------------------
    /*!
        @brief		ランダムに抽選したパネル属性の取得（出現率の影響を受けます）
        @return		int		[抽選結果の属性]
    */
    //----------------------------------------------------------------------------
    public MasterDataDefineLabel.ElementType GetRandomCardElement()
    {
        if (BattleParam.IsTutorial())
        {
            MasterDataDefineLabel.ElementType element_type = BattleSceneManager.Instance.getTutorialCard();
            if (element_type != MasterDataDefineLabel.ElementType.NONE)
            {
                return element_type;
            }
        }

        int[] element_appear_rates = new int[(int)MasterDataDefineLabel.ElementType.MAX];
        int total_rate = _getElementAppearRates(ref element_appear_rates);
        if (total_rate <= 0)
        {
            return MasterDataDefineLabel.ElementType.NONE;
        }

        //--------------------------------
        //	ランダム抽選
        //--------------------------------
        int nRandElement = (int)m_ObjectCardRand.GetRand(0, (uint)total_rate);
        MasterDataDefineLabel.ElementType nElement = MasterDataDefineLabel.ElementType.FIRE;


        for (int i = 0; i < (int)MasterDataDefineLabel.ElementType.MAX; i++)
        {

            if (element_appear_rates[i] == 0)
            {
                continue;
            }

            nRandElement -= element_appear_rates[i];
            if (nRandElement >= 0)
            {
                continue;
            }

            nElement = (MasterDataDefineLabel.ElementType)i;
            break;
        }

        return nElement;
    }

    private int _getElementAppearRates(ref int[] dest_element_appear_rates)
    {
        uint unAreaID = BattleParam.m_QuestAreaID;
        MasterDataArea cMasterDataArea = BattleParam.m_MasterDataCache.useAreaParam(unAreaID);
        if (cMasterDataArea == null)
        {
            Debug.LogError("AreaID Error! - " + unAreaID);
            return 0;
        }

        //--------------------------------
        //	各属性の出現率を格納
        //--------------------------------
        dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.NONE] = 0;
        dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.NAUGHT] = cMasterDataArea.cost0;
        dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.FIRE] = cMasterDataArea.cost1;
        dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.WATER] = cMasterDataArea.cost2;
        dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.WIND] = cMasterDataArea.cost3;
        dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.LIGHT] = cMasterDataArea.cost4;
        dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.DARK] = cMasterDataArea.cost5;
        dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.HEAL] = cMasterDataArea.cost6;


        //--------------------------------
        //	各属性の出現率を調整する
        //--------------------------------
        float rate_val = 1.0f;
        float base_val = 0.0f;
        MasterDataDefineLabel.ElementType elem = MasterDataDefineLabel.ElementType.NONE;
        bool defaultCard = false;


        // 状態異常情報を取得
        {
            StatusAilmentChara statusAilment = BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX);

            // 状態異常：出現率デフォルト状態かどうかを取得
            defaultCard = statusAilment.GetHandCardDefault();
        }


        // 状態異常：出現率デフォルトが付与されていない場合
        if (defaultCard == false)
        {
            for (int i = (int)MasterDataDefineLabel.ElementType.NONE; i < (int)MasterDataDefineLabel.ElementType.MAX; i++)
            {

                elem = (MasterDataDefineLabel.ElementType)i;

                base_val = dest_element_appear_rates[(int)elem];
                rate_val = InGameUtilBattle.PassiveChkHandCardChanceUPElement(elem);
                base_val = InGameUtilBattle.AvoidErrorMultiple(base_val, rate_val);
                if (base_val >= 0)
                {
                    dest_element_appear_rates[(int)elem] = (int)base_val;
                }
                else
                {
                    dest_element_appear_rates[(int)elem] = 0;
                }

            }
        }

        int total_rate = dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.NAUGHT]
                            + dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.FIRE]
                            + dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.WATER]
                            + dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.WIND]
                            + dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.LIGHT]
                            + dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.DARK]
                            + dest_element_appear_rates[(int)MasterDataDefineLabel.ElementType.HEAL];

        return total_rate;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief			属性へスキルによる変換を適用
        @param[in]		int		(nCardAccessNum)		コストインスタンスのインデックス
        @retval	bool			[正常終了/エラー]
     */
    //----------------------------------------------------------------------------
    public MasterDataDefineLabel.ElementType changeCardElementBySkill(MasterDataDefineLabel.ElementType nElement)
    {
        //--------------------------------
        // パネルの属性を決めて
        // 属性に合わせてメッシュを再構築する
        //--------------------------------
        //--------------------------------
        // パネルの属性選定
        //--------------------------------
        if (nElement == MasterDataDefineLabel.ElementType.NONE)
        {
            return MasterDataDefineLabel.ElementType.NONE;
        }


        //--------------------------------
        // リーダースキル：指定属性の手札を指定属性に変換する
        //--------------------------------
        nElement = InGameUtilBattle.GetLeaderSkillTransformCard(GlobalDefine.PartyCharaIndex.LEADER, nElement);
        nElement = InGameUtilBattle.GetLeaderSkillTransformCard(GlobalDefine.PartyCharaIndex.FRIEND, nElement);

        //--------------------------------
        // パッシブスキル：指定属性の手札を指定属性に変換する
        //--------------------------------
        GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;  // ダミー（未使用）
        nElement = InGameUtilBattle.PassiveChkChangeHandElem(nElement, ref owner);

        //--------------------------------
        // リミットブレイクスキル：属性固定
        //--------------------------------
        MasterDataDefineLabel.ElementType elem = BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX).GetPanelElemFix();
        if (elem != MasterDataDefineLabel.ElementType.NONE)
        {
            nElement = elem;
        }

        return nElement;
    }

    public MasterDataDefineLabel.ElementType getRandomAndSkilledCardElement()
    {
        MasterDataDefineLabel.ElementType element_type = GetRandomCardElement();
        element_type = changeCardElementBySkill(element_type);

#if BUILD_TYPE_DEBUG
        // パネル出現数を集計
        switch (element_type)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                DebugBattleLog.addValue("PnlAprNAUGHT", 1);
                break;

            case MasterDataDefineLabel.ElementType.FIRE:
                DebugBattleLog.addValue("PnlAprFIRE", 1);
                break;

            case MasterDataDefineLabel.ElementType.WATER:
                DebugBattleLog.addValue("PnlAprWATER", 1);
                break;

            case MasterDataDefineLabel.ElementType.LIGHT:
                DebugBattleLog.addValue("PnlAprLIGHT", 1);
                break;

            case MasterDataDefineLabel.ElementType.DARK:
                DebugBattleLog.addValue("PnlAprDARK", 1);
                break;

            case MasterDataDefineLabel.ElementType.WIND:
                DebugBattleLog.addValue("PnlAprWIND", 1);
                break;

            case MasterDataDefineLabel.ElementType.HEAL:
                DebugBattleLog.addValue("PnlAprHEAL", 1);
                break;
        }
#endif

        return element_type;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief			コストの属性ランダム変化
        @param[in]		int		(nCardAccessNum)		コストインスタンスのインデックス
        @retval	bool			[正常終了/エラー]
     */
    //----------------------------------------------------------------------------
    public void ChangeCard(ref BattleScene.BattleCard battle_card)
    {
        if (battle_card != null)
        {
            MasterDataDefineLabel.ElementType element_type = getRandomAndSkilledCardElement();
            battle_card.setElementType(element_type, BattleScene.BattleCard.ChangeCause.NEW_CARD);
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		スキル成立音再生（成立回数で音が変わる）
    */
    //----------------------------------------------------------------------------
    public void PlaySE_SkillCombo()
    {
        m_SkillComboCountSound++;
    }



    //----------------------------------------------------------------------------
    /*!
        @brief		UIの非表示対応
    */
    //----------------------------------------------------------------------------
    private void DisableAllWindow()
    {

        m_BattleDispEnemy.hideTargetCursor();
        BattleParam.m_TargetEnemyCurrent = InGameDefine.SELECT_NONE;

    }

    /// <summary>
    /// 復活可能表示を更新
    /// </summary>
    public void updateResurrectInfoView()
    {
        bool is_enable_resurrect = false;
        if (BattleParam.m_PlayerParty.m_PartyTotalSP >= m_AlwaysResurrectSkillSP)
        {
            for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
            {
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.DEAD);
                if (chara_once != null)
                {
                    is_enable_resurrect = true;
                    break;
                }
            }
        }

        m_BattleCardManager.m_FieldAreas.setResurrectMode(is_enable_resurrect);
    }


    /// <summary>
    /// パーティメンバーごとのハンズ数を計算・設定
    /// </summary>
    /// <param name="skill_infos"></param>
    /// <param name="skill_info_count"></param>
    /// <param name="filter_type"></param>
    private void setPartyHandsInfo(BattleSkillReq[] skill_infos, int skill_info_count, SkillRequestParam.SkillFilterType filter_type)
    {
        // ハンズ数をゼロクリア
        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            BattleParam.plusPartyMemberHands((GlobalDefine.PartyCharaIndex)idx, -999);
        }
        for (int idx = 0; idx < m_BattleCardManager.m_FieldAreas.getFieldAreaCountMax(); idx++)
        {
            BattleParam.plusFieldSkillCount(idx, -999);
        }
        for (int skill_idx = 0; skill_idx < 3; skill_idx++)
        {
            for (GlobalDefine.PartyCharaIndex member_idx = GlobalDefine.PartyCharaIndex.LEADER; member_idx < GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                BattleParam.plusFormedSkillCounts(skill_idx, member_idx, -999);
            }
        }

        // ハンズ数を加算していく
        for (int req_idx = 0; req_idx < skill_info_count; req_idx++)
        {
            BattleSkillReq battle_skill_req = skill_infos[req_idx];
            if (battle_skill_req != null && battle_skill_req.m_SkillReqState != BattleSkillReq.State.NONE && battle_skill_req.m_SkillParamSkillID != 0)
            {
                uint skill_id = battle_skill_req.m_SkillParamSkillID;
                int field_idx = battle_skill_req.m_SkillParamFieldNum;
                GlobalDefine.PartyCharaIndex caster_idx = battle_skill_req.m_SkillParamCharaNum;

                if (caster_idx == GlobalDefine.PartyCharaIndex.GENERAL)
                {
                    caster_idx = BattleParam.m_PlayerParty.getGeneralPartyMember();
                }

                if (caster_idx >= GlobalDefine.PartyCharaIndex.LEADER && caster_idx < GlobalDefine.PartyCharaIndex.MAX)
                {
                    MasterDataSkillActive master_data_skill_active = BattleParam.m_MasterDataCache.useSkillActive(skill_id);
                    if (master_data_skill_active != null)
                    {
                        bool is_target = SkillRequestParam.filterSkill(master_data_skill_active, filter_type);
                        if (is_target)
                        {
                            if (master_data_skill_active.isAlwaysResurrectSkill() == false)
                            {
                                BattleParam.plusPartyMemberHands(caster_idx, 1);
                                BattleParam.plusFieldSkillCount(field_idx, 1);

                                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(caster_idx, CharaParty.CharaCondition.SKILL_ACTIVE);
                                if (chara_once != null)
                                {
                                    int skill_idx = 2;
                                    if (chara_once.m_CharaMasterDataParam.skill_active0 == skill_id)
                                    {
                                        skill_idx = 0;
                                    }
                                    else if (chara_once.m_CharaMasterDataParam.skill_active1 == skill_id)
                                    {
                                        skill_idx = 1;
                                    }

                                    BattleParam.plusFormedSkillCounts(skill_idx, caster_idx, 1);
                                }
                            }
                            else
                            {
                                // 汎用復活の場合はスキルカウントに含めない
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 場に置かれたパネルで発動条件を満たしたアクティブスキル数
    /// </summary>
    /// <returns></returns>
    public int getFormedActiveSkillCount()
    {
        return m_BattleSkillFormingCheck.skills_count;
    }

    /// <summary>
    /// 場に置かれたパネルで発動条件を満たしたアクティブスキルのfix_idを取得
    /// </summary>
    /// <returns></returns>
    public uint getFormedActiveSkillID(int index)
    {
        return m_BattleSkillFormingCheck.skills[index].m_SkillParamSkillID;
    }

#if BUILD_TYPE_DEBUG
    /// <summary>
    /// 全敵を殺す
    /// </summary>
    public void DebugKillAllEnemy()
    {
        EBATTLE_STEP battle_step = BattleLogicFSM.Instance.getCurrentStep();
        if (battle_step == EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT)
        {
            for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
            {
                BattleEnemy battle_enemy = BattleParam.m_EnemyParam[idx];
                battle_enemy.m_EnemyHP = 0;
            }

            m_InputCountDownStart = true;
            m_InputCountDown = InGameDefine.COUNTDOWN_TIME_MIN;

            // UIの非表示
            DisableAllWindow();
        }
    }
#endif

}
