using System.Collections;
using System.Collections.Generic;


//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
    @class		SceneGoesParamToQuest2Restore
    @brief		ゲーム復帰用パラメータ
*/
//----------------------------------------------------------------------------

public class SceneGoesParamToQuest2Restore
{

    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------

    public InGameAcquireMoney[] m_AcquireMoney = new InGameAcquireMoney[GlobalDefine.INGAME_MONEY_MAX];
    public InGameAcquireUnit[] m_AcquireUnit = new InGameAcquireUnit[GlobalDefine.INGAME_UNIT_MAX];
    public InGameAcquireTicket[] m_AcquireTicket = new InGameAcquireTicket[GlobalDefine.INGAME_TICKET_MAX];
    public int[] m_AcquireUnitRareNum = new int[(int)MasterDataDefineLabel.RarityType.MAX];

    public uint m_QuestAreaID = 0;                                      //!< エリアID
    public uint m_QuestMissionID = 0;                                       //!< クエストID
    public int m_QuestEnemyGroupID = 0;                                     //!< 敵グループID

    public int m_QuestDamageMax = 0;                                        //!< 瞬間最大ダメージ
    public int m_QuestTotalTurn = 0;                                        //!< クリアまでのターン数

    public uint m_QuestRandSeed = 0;                                       //!< 乱数シード
    public bool m_IsUsedAutoPlay = false;

    public uint m_NextAreaCleard = 0;

    //----------------------------------------
    //	プレイヤー情報
    //----------------------------------------
    public RestorePlayerParty m_PlayerPartyRestore = new RestorePlayerParty();

    //----------------------------------------
    //	戦闘情報
    //----------------------------------------
    public RestoreBattle m_BattleRestore = new RestoreBattle();

    public int m_BattleCount = 0;
    public bool[] m_Balloon_active = new bool[(int)GlobalDefine.PartyCharaIndex.MAX];
    public int m_BossChainCount = 0;
    public SceneGoesParamToQuest2Restore()
    {

        //----------------------------------------
        //	パーティ情報の初期化
        //----------------------------------------
        for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
        {
            m_PlayerPartyRestore.m_PartyCharaID[i] = 0;
            m_PlayerPartyRestore.m_PartyCharaLevel[i] = 0;
            m_PlayerPartyRestore.m_PartyCharaLBSLv[i] = 0;
            m_PlayerPartyRestore.m_PartyCharaPlusPow[i] = 0;
            m_PlayerPartyRestore.m_PartyCharaPlusDef[i] = 0;
            m_PlayerPartyRestore.m_PartyCharaPlusHP[i] = 0;
            m_PlayerPartyRestore.m_PartyCharaLimitBreak[i] = 0;
            m_PlayerPartyRestore.m_PartyCharaLimitOver[i] = 0;
        }


        //----------------------------------------
        //	敵行動テーブルランダム行動時の乱数シード初期化
        //----------------------------------------
        for (int i = 0; i < m_BattleRestore.m_BattleEnemyRandSeed.Length; i++)
        {
            m_BattleRestore.m_BattleEnemyRandSeed[i] = 0;
        }


        //----------------------------------------
        //	敵行動テーブル進行度初期化
        //----------------------------------------
        for (int i = 0; i < m_BattleRestore.m_BattleEnemyActStep.Length; i++)
        {
            m_BattleRestore.m_BattleEnemyActStep[i] = -1;
        }


        m_BattleRestore.m_UpdateStatusAilmentPlayer = false;


        //----------------------------------------
        //	敵リアクション：判定スキルID
        //----------------------------------------
        m_BattleRestore.m_EnemyReactChkSkillID = 0;
        m_BattleRestore.m_NextLimitBreakSkillCaster = GlobalDefine.PartyCharaIndex.ERROR;
        m_BattleRestore.m_NextLimitBreakSkillFixID = 0;

#if true // @Change Developer 2015/08/21 v300状態異常の遅延発動対応。
        //----------------------------------------
        // 状態異常遅延発動バッファ。
        //----------------------------------------
        for (int i = 0; i < m_BattleRestore.m_StatusAilmentDelay.Length; ++i)
        {
            m_BattleRestore.m_StatusAilmentDelay[i] = new RestoreDelayAilment();

            for (int j = 0; j < m_BattleRestore.m_StatusAilmentDelay[i].m_acSkillParamTarget.Length; ++j)
            {
                m_BattleRestore.m_StatusAilmentDelay[i].m_acSkillParamTarget[j] = new BattleSkillTarget();
            }
        }
#endif

        //----------------------------------------
        // 戦闘時カード管理：場：パネル属性
        // @add Developer 2015/10/23 v300新スキル対応
        //----------------------------------------
        for (int num = 0; num < m_BattleRestore.m_BattleFieldPanelElem.Length; ++num)
        {
            m_BattleRestore.m_BattleFieldPanelElem[num] = MasterDataDefineLabel.ElementType.NONE;
        }

        //----------------------------------------
        // 敵テーブル切り替え時行動：フラグ
        // @add Developer 2015/12/28 v300新スキル対応
        //----------------------------------------
        for (int num = 0; num < InGameDefine.BATTLE_ENEMY_MAX; ++num)
        {
            m_BattleRestore.m_EnemyActionSwitch[num] = false;
        }

        //----------------------------------------
        // パーティーユニットスキル発動アイコン：フラグ
        //----------------------------------------
        for (int num = 0; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
        {
            m_Balloon_active[num] = true;
        }

        m_BossChainCount = 0;
    }
}


#if true // @Change Developer 2015/08/21 v300状態異常の遅延発動対応。
//----------------------------------------------------------------------------
/*!
    @brief		状態異常遅延発動用情報クラス。
*/
//----------------------------------------------------------------------------
public class RestoreDelayAilment
{
    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------

    public GlobalDefine.PartyCharaIndex m_nSkillParamOwnerNum = GlobalDefine.PartyCharaIndex.ERROR;                                                     //!< スキル発動キャラ。※InGameManager.m_PlayerPartyCharaのインデックス。
    public BattleSkillTarget[] m_acSkillParamTarget = new BattleSkillTarget[InGameDefine.BATTLE_ENEMY_MAX]; //!< スキルターゲット。※BattleManager.m_acSkillActivityDelayAilment.m_SkillParamTargetの参照を登録する。
    public MasterDataDefineLabel.TargetType m_nStatusAilmentTarget = MasterDataDefineLabel.TargetType.NONE; //!< 状態異常ターゲット種類。※MasterDataDefineLabel.TARGET_***の値。
    public int[] m_anStatusAilment = new int[StatusAilmentChara.get_STATUSAILMENT_MAX()];     //!< 状態異常ID。※MasterDataUtil.GetMasterDataStatusAilmentParamに渡す値。	[SerializeField] private int[]				m_anStatusAilment			= new int[ StatusAilmentsManager.STATUSAILMENT_MAX ];		//!< 状態異常ID。※MasterDataUtil.GetMasterDataStatusAilmentParamに渡す値。


    public void SetAilment(int index, int ailment_id)
    {
        m_anStatusAilment[index] = ailment_id;
    }
    public int[] GetAilmentIDArray()
    {
        int[] ret_val = new int[m_anStatusAilment.Length];

        for (int idx = 0; idx < m_anStatusAilment.Length; idx++)
        {
            ret_val[idx] = m_anStatusAilment[idx];
        }

        return ret_val;
    }
    public void SetAilmentIDArray(int[] ailment_type_array)
    {
        for (int idx = 0; idx < m_anStatusAilment.Length; idx++)
        {
            m_anStatusAilment[idx] = ailment_type_array[idx];
        }
    }
};
#endif

//----------------------------------------------------------------------------
/*!
    @brief		復帰用フロア情報クラス
*/
//----------------------------------------------------------------------------
public class RestoreFloor
{

    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------
    public bool[] m_PanelEnable = new bool[GlobalDefine.CELL_MAX_TOTAL];        //!< 有効無効
    public bool m_BossKilled = false;                                       //!< ボス殺害フラグ
    public bool m_BossStart = false;
    public bool m_GetKey = false;                                       //!< 鍵取得フラグ
    public int[] m_cautionLevel = new int[GlobalDefine.CELL_MAX_TOTAL];

} // class RestoreFloor


/// <summary>
/// 復帰用パーティクラス
/// </summary>
public class RestorePlayerParty
{
    public bool m_IsKobetsuHP = false;

    public BattleSceneUtil.MultiInt m_QuestHP = new BattleSceneUtil.MultiInt(0);                                        //!< 残りHP
    public BattleSceneUtil.MultiInt m_QuestHPMax = new BattleSceneUtil.MultiInt(0);                                     //!< 最大HP
    public int m_QuestSP = 0;                                       //!< 残りSP
    public int m_QuestSPMax = 0;                                        //!< 最大SP
    public BattleSceneUtil.MultiInt m_Hate = new BattleSceneUtil.MultiInt(0);
    public BattleSceneUtil.MultiInt m_Hate_ProvokeTurn = new BattleSceneUtil.MultiInt(0);
    public BattleSceneUtil.MultiInt m_Hate_ProvokeOrder = new BattleSceneUtil.MultiInt(0);
    public BattleSceneUtil.MultiAilment m_PartyAilments = new BattleSceneUtil.MultiAilment();   //!< パーティ状態異常
    public int m_HeroSkillTurn = 0; // 主人公スキルターン数
    public CharaParty.BattleAchive m_BattleAchive = new CharaParty.BattleAchive();

    //----------------------------------------
    //	プレイヤー情報
    //----------------------------------------
    public uint[] m_PartyCharaID = new uint[(int)GlobalDefine.PartyCharaIndex.MAX];                     //!< パーティキャラID
    public int[] m_PartyCharaLevel = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                        //!< パーティキャラレベル
    public int[] m_PartyCharaLBSLv = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                        //!< パーティキャラLBSLv
    public int[] m_PartyCharaPlusPow = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                      //!< パーティキャラ：＋値：POW
    public int[] m_PartyCharaPlusDef = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                      //!< パーティキャラ：＋値：DEF
    public int[] m_PartyCharaPlusHP = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                       //!< パーティキャラ：＋値：HP
    public int[] m_PartyCharaLimitBreak = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                       //!< パーティキャラLBSコスト
    public int[] m_PartyCharaLimitOver = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                        //!< パーティキャラ：限界突破レベル
    public uint[] m_PartyCharaLinkID = new uint[(int)GlobalDefine.PartyCharaIndex.MAX];                     //!< パーティキャラ：リンク：キャラID
    public int[] m_PartyCharaLinkLv = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                       //!< パーティキャラ：リンク：キャラレベル
    public int[] m_PartyCharaLinkPlusPow = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                      //!< パーティキャラ：リンク：＋値：POW
    public int[] m_PartyCharaLinkPlusHP = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                       //!< パーティキャラ：リンク：＋値：HP
    public int[] m_PartyCharaLinkPoint = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                        //!< パーティキャラ：リンク：ポイント(リンクスキル発動率に影響)
    public int[] m_PartyCharaLinkLimitOver = new int[(int)GlobalDefine.PartyCharaIndex.MAX];                        //!< パーティキャラ：リンク：限界突破
}


//----------------------------------------------------------------------------
/*!
    @class		RestoreBattle
    @brief		復帰用戦闘情報クラス
*/
//----------------------------------------------------------------------------
public class RestoreBattle
{

    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------
    public int m_BattleUniqueID = 0;            //!< 戦闘構築情報：戦闘情報：ユニークID
    public int m_BattleTurnOffset = 0;          //!< ターンオフセット
    public bool m_Chain = false;        //!< この戦闘が派生戦闘かどうか
    public bool m_Boss = false;     //!< この戦闘がボスかどうか

    /*
        public bool BattleRequestAdd(	PacketStructQuestBuildBattle battle_param,
                                        bool boss, bool chain, int turn_offset ) {
    */
    public int m_BattleTotalTurn = 0;                                                             //!< 戦闘情報：経過ターン数

    public bool m_UpdateStatusAilmentPlayer = false;                            //!< プレイヤー状態異常更新済みフラグ

#if true // @Change Developer 2015/08/21 v300状態異常の遅延発動対応。
    public RestoreDelayAilment[] m_StatusAilmentDelay = new RestoreDelayAilment[BattleLogic.DELAYAILMENT_MAX];  //!< プレイヤーターン開始時の状態異常遅延発動予約バッファ。
#endif

    public BattleEnemy[] m_BattleEnemy = new BattleEnemy[InGameDefine.BATTLE_ENEMY_MAX];              //!< 敵情報

    // @add Developer 2015/12/28 v320敵テーブル切り替え時行動対応
    public bool[] m_EnemyActionSwitch = new bool[InGameDefine.BATTLE_ENEMY_MAX];                    //!< 戦闘情報：敵テーブル切り替え時行動フラグ

    public int[] m_BattleEnemyActStep = new int[InGameDefine.BATTLE_ENEMY_MAX];                     //!< 戦闘情報：敵行動テーブル進行度
    public uint[] m_BattleEnemyRandSeed = new uint[InGameDefine.BATTLE_ENEMY_MAX];                    //!< 戦闘情報：敵行動テーブルランダム行動時の乱数シード

    public uint m_EnemyReactChkSkillID = 0;                                                             //!< 戦闘情報：敵リアクション：判定スキルID
    public GlobalDefine.PartyCharaIndex m_NextLimitBreakSkillCaster = GlobalDefine.PartyCharaIndex.ERROR;
    public uint m_NextLimitBreakSkillFixID = 0; //続きのリミブレスキルのfix_id

    public MasterDataDefineLabel.ElementType[] m_BattleHandCardElem = new MasterDataDefineLabel.ElementType[BattleLogic.BATTLE_FIELD_MAX];
    public MasterDataDefineLabel.ElementType[] m_BattleNextCardElem = new MasterDataDefineLabel.ElementType[BattleLogic.BATTLE_FIELD_MAX];

    // @add Developer 2015/10/23 v300新スキル対応
    public MasterDataDefineLabel.ElementType[] m_BattleFieldPanelElem = new MasterDataDefineLabel.ElementType[BattleLogic.BATTLE_FIELD_MAX * BattleLogic.BATTLE_FIELD_COST_MAX]; //!< 戦闘時カード管理：場：パネル属性

    public bool[] m_BattleBoost = null;

    public int m_ProcStep = (int)EBATTLE_UPDATE_STEP.eSTART;            //!< 戦闘突入後の更新状態
    public int m_ProcStep_player = (int)EBATTLE_UPDATE_STEP.eSTART;         //!< 戦闘突入後の更新状態(プレイヤー用)

    public AchievementTotalingInBattle m_AchievementTotaling = null;	//!< 実績の集計情報
}
