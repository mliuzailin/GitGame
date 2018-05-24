//このシーンを使用するには
//  MasterDataCache.cs の先頭にある USE_LOCAL_ENEMY_MASTER,USE_DEBUG_JSON_MASTER_DATA
//  BattleParam.cs の先頭にある TEST_CONTINUE_DIALOG
// を有効にする必要があるかもしれない

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class AilmentData
{
    [Tooltip("状態異常ＩＤ")]
    public int m_AilmentID;

    [Tooltip("この状態異常を発動したキャラの基本攻撃力")]
    public int m_AttackerBaseAttack;
}

[Serializable]
public class PlayerCharaData
{
    [Tooltip("キャラＩＤ")]
    public int m_CharaID;
    [Tooltip("キャラレベル")]
    public int m_CharaLevel;
    [Tooltip("キャラスキルレベル")]
    public int m_SkillLevel;
    [Tooltip("キャラ限界突破レベル")]
    public int m_LimitOverLevel;
    [Tooltip("キャラ攻撃力プラスレベル")]
    public int m_PlusPower;
    [Tooltip("キャラＨＰプラスレベル")]
    public int m_PlusHP;

    [Tooltip("リンクキャラＩＤ")]
    public int m_LinkCharaID;
    [Tooltip("リンクキャラレベル")]
    public int m_LinkCharaLevel;
    [Tooltip("リンクキャラ限界突破レベル")]
    public int m_LinkLimitOverLevel;
    [Tooltip("リンクキャラ攻撃力プラスレベル")]
    public int m_LinkPlusPower;
    [Tooltip("リンクキャラＨＰプラスレベル")]
    public int m_LinkPlusHP;
    [Tooltip("リンクキャラ親密度(0～10000 10000で100%)")]
    public int m_LinkFriendry;  // 親密度.

    [Tooltip("アクティブスキル発動までのターン数")]
    public int m_SkillTurn;	// スキル発動までの残りターン数.
}

[Serializable]
public class PlayerPartyData
{
    [Tooltip("パーティＨＰ（-1で最大値に自動設定）")]
    public int m_HP = -1;
    [Tooltip("パーティＳＰ")]
    public int m_SP = 10;
    [Tooltip("パーティ状態異常")]
    public AilmentData[] m_Ailments;
    [Tooltip("パーティメンバー")]
    public PlayerCharaData[] m_PlayerCharas = new PlayerCharaData[5];
}

[Serializable]
public class EnemyCharaData
{
    [Tooltip("敵定義ＩＤ（キャラＩＤではない）")]
    public int m_EnemyDefineID;
    [Tooltip("ドロップするキャラＩＤ（ドロップしない場合はゼロを設定）")]
    public int m_DropCharaID;
    public int m_DropPowPlus;
    public int m_DropHpPlus;
    [Tooltip("状態異常")]
    public AilmentData[] m_Ailments;
}


[Serializable]
public class AreaData
{
    [Tooltip("エリア情報ID")]
    public int m_AreaID;
}

[Serializable]
public class BattleData
{
    [Tooltip("エリア情報")]
    public AreaData m_AreaData;
    [Tooltip("プレイヤーパーティ情報")]
    public PlayerPartyData m_PlayerParty;
}


public class BattleSceneStart : MonoBehaviour
{
    [Tooltip("バトルで使用するマスターデータはJsonから取得する（検証用）")]
    public bool m_UseJsonMasterData = false;

    [Tooltip("バトル条件設定")]
    public BattleData m_BattleData = new BattleData();
    [Tooltip("スキルターン短縮条件")]
    public SkillTurnCondition m_SkillTurnCondition = new SkillTurnCondition();
    [Tooltip("主人公")]
    public BattleHeroParam m_HeroParam = new BattleHeroParam();

    private int[] m_EnemyPartyIDs = null;   // 中身は MasterDataEnemyGroup.fix_id
    private int m_EnemyPartyIndex = 0;
    private ServerDataDefine.PacketStructQuest2Build m_QuestBuild = null;
    private int m_NextChainPartyIndex = 0;
    private int m_NextChainTurnOffset = 0;

    private MasterDataCache m_MasterDataCache = new MasterDataCache();

    private bool m_IsEnableEnemyUI = false; // 敵情報を設定できるようにするかどうか（MasterDataEnemyGroup がないと設定できないのでその時は無効にする）

    private bool m_IsKobetsuHP = false;
    private bool m_KobetsuHPEnemyAttackAll = true;
    private bool m_KobetsuHPEnemyTargetHate = true;
    private float m_KobetsuHP_EnemyAtkRate = 1.0f;

    // Use this for initialization
    void Start()
    {
#if BUILD_TYPE_DEBUG
        BattleParam.m_IsUseDebugJsonMasterData = m_UseJsonMasterData;
#endif
        if (BattleSceneManager.Instance != null)
        {
            BattleSceneManager.Instance.setBattleScenePhase(BattleSceneManager.BattleScenePhase.NOT_BATTLE);
        }

        DebugBattleLog.setEnable(true);

        BattleSceneManager.Instance.PRIVATE_FIELD.CreateBattleGameObject();
    }

    void initPlayerParty1()
    {
        if (m_BattleData.m_PlayerParty != null)
        {
            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                if (m_BattleData.m_PlayerParty.m_PlayerCharas[idx] != null)
                {
                    int chara_id = m_BattleData.m_PlayerParty.m_PlayerCharas[idx].m_CharaID;
                    int chara_level = m_BattleData.m_PlayerParty.m_PlayerCharas[idx].m_CharaLevel;
                    int chara_id_link = m_BattleData.m_PlayerParty.m_PlayerCharas[idx].m_LinkCharaID;
                    int chara_level_link = m_BattleData.m_PlayerParty.m_PlayerCharas[idx].m_LinkCharaLevel;

                    BattleSceneStartCharaUI chara_ui = gameObject.transform.Find("SetupUI/" + menber_ids[idx] + "/Main/BattleDebugCharaSettingUI").GetComponent<BattleSceneStartCharaUI>();
                    BattleSceneStartCharaUI chara_ui_link = gameObject.transform.Find("SetupUI/" + menber_ids[idx] + "/Link/BattleDebugCharaSettingUI").GetComponent<BattleSceneStartCharaUI>();

                    chara_ui.setup(chara_id, chara_level);
                    chara_ui_link.setup(chara_id_link, chara_level_link);
                }
            }
        }
    }

    void initPlayerParty2()
    {
        // マスターーデータ
        BattleParam.m_MasterDataCache = m_MasterDataCache;

        // ステージ情報
        BattleParam.m_QuestAreaID = 0;
        BattleParam.m_QuestMissionID = 0;
        BattleParam.m_QuestFloor = 0;
        BattleParam.m_AcquireKey = false;

        //
        BattleParam.ClrLBS();
        BattleParam.m_SkillRequestLimitBreak = new SkillRequestParam(BattleParam.SKILL_LIMIT_BREAK_ADD_MAX);

#if UNITY_EDITOR
        BattleParam.m_IsDamageOutput = false;
#endif

        // プレイヤーパーティメンバー
        if (m_BattleData.m_PlayerParty.m_PlayerCharas != null)
        {
            CharaOnce[] player_party_chara = new CharaOnce[m_BattleData.m_PlayerParty.m_PlayerCharas.Length];
            for (int idx = 0; idx < m_BattleData.m_PlayerParty.m_PlayerCharas.Length; idx++)
            {
                PlayerCharaData player_chara_data = m_BattleData.m_PlayerParty.m_PlayerCharas[idx];
                if (player_chara_data != null)
                {
                    MasterDataParamChara master_data = m_MasterDataCache.useCharaParam((uint)player_chara_data.m_CharaID);
                    if (master_data != null)
                    {
                        CharaOnce chara_once = new CharaOnce();
                        if (player_chara_data.m_CharaLevel < 1)
                        {
                            player_chara_data.m_CharaLevel = 1;
                        }
                        if (player_chara_data.m_CharaLevel > master_data.level_max)
                        {
                            player_chara_data.m_CharaLevel = master_data.level_max;
                        }

                        chara_once.CharaSetupFromID((uint)player_chara_data.m_CharaID,
                            player_chara_data.m_CharaLevel,
                            player_chara_data.m_SkillLevel,
                            player_chara_data.m_LimitOverLevel,
                            player_chara_data.m_PlusPower,
                            player_chara_data.m_PlusHP,

                            (uint)player_chara_data.m_LinkCharaID,
                            player_chara_data.m_LinkCharaLevel,
                            player_chara_data.m_LinkPlusPower,
                            player_chara_data.m_LinkPlusHP,
                            player_chara_data.m_LinkFriendry,
                            player_chara_data.m_LinkLimitOverLevel
                        );

                        player_party_chara[idx] = chara_once;
                    }
                }
            }

            // プレイヤーパーティ
            BattleParam.m_PlayerParty = new CharaParty();
            BattleParam.m_PlayerParty.setHero(m_HeroParam.m_Level, (uint)m_HeroParam.m_HeroSkillID);
            BattleParam.m_PlayerParty.PartySetup(player_party_chara, m_IsKobetsuHP);
            BattleParam.m_PlayerParty.m_PartyTotalSP = m_BattleData.m_PlayerParty.m_SP;

            // ステージ情報
            BattleParam.m_QuestAreaID = (uint)m_BattleData.m_AreaData.m_AreaID;
            BattleParam.m_QuestMissionID = 0;
            BattleParam.m_QuestFloor = 0;
            BattleParam.m_AcquireKey = false;
        }

        if (InGameUtil.IsLocalData())
        {
            SceneGoesParamToQuest2Restore restore = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore;
            RestorePlayerParty restore_player_party = restore.m_PlayerPartyRestore;
            if (restore_player_party != null)
            {
                CharaOnce[] player_party_chara = new CharaOnce[(int)GlobalDefine.PartyCharaIndex.MAX];
                for (int idx = 0; idx < player_party_chara.Length; idx++)
                {
                    CharaOnce chara_once = new CharaOnce();
                    chara_once.CharaSetupFromID(restore_player_party.m_PartyCharaID[idx],
                        restore_player_party.m_PartyCharaLevel[idx],
                        restore_player_party.m_PartyCharaLBSLv[idx],
                        restore_player_party.m_PartyCharaLimitOver[idx],
                        restore_player_party.m_PartyCharaPlusPow[idx],
                        restore_player_party.m_PartyCharaPlusHP[idx],

                        restore_player_party.m_PartyCharaLinkID[idx],
                        restore_player_party.m_PartyCharaLinkLv[idx],
                        restore_player_party.m_PartyCharaLinkPlusPow[idx],
                        restore_player_party.m_PartyCharaLinkPlusHP[idx],
                        restore_player_party.m_PartyCharaLinkPoint[idx],
                        restore_player_party.m_PartyCharaLinkLimitOver[idx]
                    );

                    //------------------------------
                    // リミットブレイクコスト復帰
                    //------------------------------
                    chara_once.m_CharaLimitBreak = restore.m_PlayerPartyRestore.m_PartyCharaLimitBreak[idx];

                    player_party_chara[idx] = chara_once;
                }

                // プレイヤーパーティ
                BattleParam.m_PlayerParty = new CharaParty();
                BattleParam.m_PlayerParty.setHero(0/*m_HeroParam.m_Level*/, 0/*(uint)m_HeroParam.m_HeroSkillID*/);
                BattleParam.m_PlayerParty.PartySetup(player_party_chara, restore_player_party.m_IsKobetsuHP);

                //------------------------------
                // パーティ状態異常復帰（CharaParty.PartySetup() より後に復帰）
                //------------------------------
                BattleParam.m_PlayerParty.m_Ailments = restore_player_party.m_PartyAilments;
                BattleParam.m_PlayerParty.m_Ailments.restoreFromSaveData();

                BattleParam.m_PlayerParty.m_HPCurrent.setValue(GlobalDefine.PartyCharaIndex.MAX, restore_player_party.m_QuestHP);
                BattleParam.m_PlayerParty.m_PartyTotalSP = restore_player_party.m_QuestSP;
                BattleParam.m_PlayerParty.m_HPMax.setValue(GlobalDefine.PartyCharaIndex.MAX, restore_player_party.m_QuestHPMax);
                BattleParam.m_PlayerParty.m_PartyTotalSPMax = restore_player_party.m_QuestSPMax;
                BattleParam.m_PlayerParty.m_Hate = restore_player_party.m_Hate;
                BattleParam.m_PlayerParty.m_Hate_ProvokeTurn = restore_player_party.m_Hate_ProvokeTurn;
                BattleParam.m_PlayerParty.m_Hate_ProvokeOrder = restore_player_party.m_Hate_ProvokeOrder;

                BattleParam.m_PlayerParty.m_BattleHero.restoreSkillTurn(restore_player_party.m_HeroSkillTurn);
                BattleParam.m_PlayerParty.m_BattleAchive = restore_player_party.m_BattleAchive;
            }
        }
    }

    static private int CreateQuestBuildBattle(
        int nFloor
        , ref TemplateList<ServerDataDefine.PacketStructQuest2BuildBattle> rcBattleList
        , ref TemplateList<ServerDataDefine.PacketStructQuest2BuildDrop> rcDropList
        , MasterDataEnemyGroup cEnemyGroup)
    {
        if (cEnemyGroup == null)
        {
            return 0;
        }

        //----------------------------------------
        // グループに内包されるエネミー一覧を生成
        //----------------------------------------
        TemplateList<MasterDataParamEnemy> cEnemyList = new TemplateList<MasterDataParamEnemy>();
        MasterDataParamEnemy cEnemyData1 = BattleParam.m_MasterDataCache.useEnemyParam(cEnemyGroup.enemy_id_1);
        MasterDataParamEnemy cEnemyData2 = BattleParam.m_MasterDataCache.useEnemyParam(cEnemyGroup.enemy_id_2);
        MasterDataParamEnemy cEnemyData3 = BattleParam.m_MasterDataCache.useEnemyParam(cEnemyGroup.enemy_id_3);
        MasterDataParamEnemy cEnemyData4 = BattleParam.m_MasterDataCache.useEnemyParam(cEnemyGroup.enemy_id_4);
        MasterDataParamEnemy cEnemyData5 = BattleParam.m_MasterDataCache.useEnemyParam(cEnemyGroup.enemy_id_5);
        MasterDataParamEnemy cEnemyData6 = BattleParam.m_MasterDataCache.useEnemyParam(cEnemyGroup.enemy_id_6);
        MasterDataParamEnemy cEnemyData7 = BattleParam.m_MasterDataCache.useEnemyParam(cEnemyGroup.enemy_id_7);
        if (cEnemyData1 != null) { cEnemyList.Add(cEnemyData1); }
        if (cEnemyData2 != null) { cEnemyList.Add(cEnemyData2); }
        if (cEnemyData3 != null) { cEnemyList.Add(cEnemyData3); }
        if (cEnemyData4 != null) { cEnemyList.Add(cEnemyData4); }
        if (cEnemyData5 != null) { cEnemyList.Add(cEnemyData5); }
        if (cEnemyData6 != null) { cEnemyList.Add(cEnemyData6); }
        if (cEnemyData7 != null) { cEnemyList.Add(cEnemyData7); }


        //----------------------------------------
        // グループ内のエネミーリストから実際に出現するエネミーを並べる
        //----------------------------------------
        TemplateList<int> cEnemyFixAccessList = new TemplateList<int>();
        ServerDataDefine.PacketStructQuest2BuildBattle cBattle = new ServerDataDefine.PacketStructQuest2BuildBattle();
        if (cEnemyGroup.fix == MasterDataDefineLabel.BoolType.ENABLE)
        {
            //----------------------------------------
            // 完全固定で並べる場合
            //----------------------------------------
            cEnemyFixAccessList.Alloc(cEnemyList.m_BufferSize);
            for (int i = 0; i < cEnemyList.m_BufferSize; i++)
            {
                cEnemyFixAccessList.Add(i);
            }
        }
        else
        {
            //----------------------------------------
            // ランダムで並べる場合
            //----------------------------------------
            int nTotalEnemyCt = (int)RandManager.GetRand((uint)cEnemyGroup.num_min, (uint)cEnemyGroup.num_max);
            cEnemyFixAccessList.Alloc(nTotalEnemyCt);
            for (int i = 0; i < nTotalEnemyCt; i++)
            {
                int nEnemyListAccess = (int)RandManager.GetRand(0, (uint)cEnemyList.m_BufferSize);
                cEnemyFixAccessList.Add(nEnemyListAccess);
            }
        }

        //----------------------------------------
        // ドロップ判定
        //----------------------------------------
        cBattle.floor = nFloor;
        cBattle.enemy_list = new uint[cEnemyFixAccessList.m_BufferSize];
        cBattle.drop_list = new int[cEnemyFixAccessList.m_BufferSize];
        bool bDropFixed = false;
        for (int i = 0; i < cEnemyFixAccessList.m_BufferSize; i++)
        {
            //----------------------------------------
            // 基本情報入力
            //----------------------------------------
            cBattle.enemy_list[i] = cEnemyList[cEnemyFixAccessList[i]].fix_id;
            cBattle.drop_list[i] = 0;

#if BUILD_TYPE_DEBUG
            Debug.Log("EnemyBattle - " + cEnemyList[cEnemyFixAccessList[i]].fix_id);
#endif
            //----------------------------------------
            // ドロップ判定
            //----------------------------------------
            if (bDropFixed == true)
            {
                continue;
            }
            MasterDataParamEnemy cEnemyFixParam = cEnemyList[cEnemyFixAccessList[i]];
            uint unDropRand = RandManager.GetRand(0, 10000);
            if (cEnemyFixParam.drop_unit_rate < unDropRand)
            {
                //----------------------------------------
                // ドロップ確定！
                // パラメータを保持しておく
                //----------------------------------------
                ServerDataDefine.PacketStructQuest2BuildDrop cDropData = new ServerDataDefine.PacketStructQuest2BuildDrop();
                cDropData.item_id = (int)cEnemyFixParam.drop_unit_id;
                if (cDropData.item_id == 0)
                {
                    cDropData.setKindType(ServerDataDefine.PacketStructQuest2BuildDrop.KindType.NONE);
                }
                else
                {
                    cDropData.setKindType(ServerDataDefine.PacketStructQuest2BuildDrop.KindType.UNIT);
                }
                cDropData.plus_hp = 0;
                cDropData.plus_pow = 0;
                cDropData.unique_id = (rcDropList.m_BufferSize + 1);
                cDropData.floor = nFloor;

                rcDropList.Add(cDropData);

                cBattle.drop_list[i] = cDropData.unique_id;

                bDropFixed = true;
            }
        }

        //----------------------------------------
        // 戦闘連鎖があるなら連鎖も加味
        //----------------------------------------
        if (cEnemyGroup.chain_id > 0)
        {
            MasterDataEnemyGroup cChainEnemyGroup = BattleParam.m_MasterDataCache.useEnemyGroup(cEnemyGroup.chain_id);
            cBattle.chain = CreateQuestBuildBattle(nFloor, ref rcBattleList, ref rcDropList, cChainEnemyGroup);
        }

        //----------------------------------------
        // 情報を加算して管理番号を返す
        //----------------------------------------
        cBattle.unique_id = rcBattleList.m_BufferSize + 1;
        rcBattleList.Add(cBattle);


        return cBattle.unique_id;
    }

    private void resetBattleRequest()
    {
        m_QuestBuild = null;
        m_EnemyPartyIndex = 0;
        m_NextChainPartyIndex = 0;
        m_NextChainTurnOffset = 0;
    }

    private bool createBattleRequest()
    {
        if (m_QuestBuild != null)
        {
            if (m_NextChainPartyIndex != 0)
            {
                //敵進化あり
                BattleParam.m_BattleRequest = new BattleReq();
                ServerDataDefine.PacketStructQuest2BuildBattle battle_param = InGameUtil.GetQuest2BuildBattle(m_QuestBuild, m_NextChainPartyIndex);

                BattleParam.m_BattleRequest.SetupBattleReq(0,
                    false,
                    m_NextChainTurnOffset,
                    false,
                    true,
                    m_QuestBuild,
                    battle_param);

                m_NextChainPartyIndex = BattleParam.m_BattleRequest.m_QuestBuildBattle.chain;
                m_NextChainTurnOffset = BattleParam.m_BattleRequest.m_QuestBuildBattle.chain_turn_offset;
            }
            else
            {
                m_QuestBuild = null;
                m_EnemyPartyIndex++;
            }
        }

        if (m_QuestBuild == null && m_EnemyPartyIndex < m_EnemyPartyIDs.Length)
        {
            TemplateList<ServerDataDefine.PacketStructQuest2BuildBattle> acQuestBuildBattle = new TemplateList<ServerDataDefine.PacketStructQuest2BuildBattle>();
            TemplateList<ServerDataDefine.PacketStructQuest2BuildDrop> acQuestBuildDrop = new TemplateList<ServerDataDefine.PacketStructQuest2BuildDrop>();
            acQuestBuildBattle.Alloc(64);

            MasterDataEnemyGroup enemy_group = BattleParam.m_MasterDataCache.useEnemyGroup((uint)m_EnemyPartyIDs[m_EnemyPartyIndex]);

            int battle_id = CreateQuestBuildBattle(0, ref acQuestBuildBattle, ref acQuestBuildDrop, enemy_group);

            // 敵情報.
            m_QuestBuild = new ServerDataDefine.PacketStructQuest2Build();
            m_QuestBuild.list_drop = acQuestBuildDrop.ToArray();
            m_QuestBuild.list_battle = acQuestBuildBattle.ToArray();

            // ヘイト情報付加
            for (int idx = 0; idx < m_QuestBuild.list_battle.Length; idx++)
            {
                ServerDataDefine.PacketStructQuest2BuildBattle battle_build = m_QuestBuild.list_battle[idx];
                if (battle_build != null)
                {
                    battle_build.hate = new ServerDataDefine.PacketStructQuest2Hate();
                    battle_build.hate.hate_initial = 1000;
                    battle_build.hate.hate_given_damage1 = 1000;
                    battle_build.hate.hate_given_damage2 = 800;
                    battle_build.hate.hate_given_damage3 = 600;
                    battle_build.hate.hate_given_damage4 = 400;
                    battle_build.hate.hate_given_damage5 = 200;
                    battle_build.hate.hate_heal1 = 500;
                    battle_build.hate.hate_heal2 = 400;
                    battle_build.hate.hate_heal3 = 300;
                    battle_build.hate.hate_heal4 = 200;
                    battle_build.hate.hate_heal5 = 100;
                    battle_build.hate.hate_rate_fire = 100;
                    battle_build.hate.hate_rate_water = 100;
                    battle_build.hate.hate_rate_wind = 100;
                    battle_build.hate.hate_rate_light = 100;
                    battle_build.hate.hate_rate_dark = 100;
                    battle_build.hate.hate_rate_naught = 100;

                    battle_build.hate.hate_rate_race1 = 100;
                    battle_build.hate.hate_rate_race2 = 100;
                    battle_build.hate.hate_rate_race3 = 100;
                    battle_build.hate.hate_rate_race4 = 100;
                    battle_build.hate.hate_rate_race5 = 100;
                    battle_build.hate.hate_rate_race6 = 100;
                    battle_build.hate.hate_rate_race7 = 100;
                    battle_build.hate.hate_rate_race8 = 100;
                    battle_build.hate.hate_rate_race9 = 100;
                    battle_build.hate.hate_rate_race10 = 100;
                }
            }

            ServerDataDefine.PacketStructQuest2BuildBattle battle_param = InGameUtil.GetQuest2BuildBattle(m_QuestBuild, battle_id);

            BattleParam.m_BattleRequest = new BattleReq();
            BattleParam.m_BattleRequest.SetupBattleReq(0,
                false,
                0,
                false,
                false,
                m_QuestBuild,
                battle_param);

            m_NextChainPartyIndex = BattleParam.m_BattleRequest.m_QuestBuildBattle.chain;
            m_NextChainTurnOffset = BattleParam.m_BattleRequest.m_QuestBuildBattle.chain_turn_offset;
        }

        return (m_QuestBuild != null);
    }

    private string[] menber_ids = { "Leader", "Sub1", "Sub2", "Sub3", "Friend" };
    private string[] enemy_gropus = { "Enemy1", "Enemy2", "Enemy3", "Enemy4", "Enemy5" };
    /// <summary>
    /// パーティキャラ設定ＵＩへ値を反映
    /// </summary>
    private void setup_for_screen()
    {
        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            BattleSceneStartCharaUI chara_ui = gameObject.transform.Find("SetupUI/" + menber_ids[idx] + "/Main/BattleDebugCharaSettingUI").GetComponent<BattleSceneStartCharaUI>();
            BattleSceneStartCharaUI chara_ui_link = gameObject.transform.Find("SetupUI/" + menber_ids[idx] + "/Link/BattleDebugCharaSettingUI").GetComponent<BattleSceneStartCharaUI>();

            int chara_id = chara_ui.getCharaID();
            int chara_level = chara_ui.getCharaLevel();

            int chara_id_link = chara_ui_link.getCharaID();
            int chara_level_link = chara_ui_link.getCharaLevel();

            if (BattleParam.m_PlayerParty != null)
            {
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
                if (chara_once != null)
                {
                    chara_id = (int)chara_once.m_CharaMasterDataParam.fix_id;

                    if (chara_once.m_LinkParam != null && chara_once.m_LinkParam.m_CharaID != 0)
                    {
                        MasterDataParamChara master_chara_link = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
                        chara_id_link = (int)master_chara_link.fix_id;
                    }
                }
            }

            if (chara_id != 0)
            {
                chara_ui.setOn(true);
                chara_ui.setup(chara_id, chara_level);
            }
            else
            {
                chara_ui.setOn(false);
            }

            if (chara_id_link != 0)
            {
                chara_ui_link.setOn(true);
                chara_ui_link.setup(chara_id_link, chara_level_link);
            }
            else
            {
                chara_ui_link.setOn(false);
            }
        }

        for (int idx = 0; idx < enemy_gropus.Length; idx++)
        {
            GameObject enemy_ui_obj = gameObject.transform.Find("SetupUI/" + enemy_gropus[idx]).gameObject;
            if (enemy_ui_obj != null)
            {
                enemy_ui_obj.SetActive(m_IsEnableEnemyUI);
            }
        }

        gameObject.transform.Find("SetupUI").gameObject.SetActive(true);
    }

    private void update_setup_ui()
    {
        Slider slider = gameObject.transform.Find("SetupUI/SliderEnemyAtkRate").GetComponent<Slider>();
        if (slider != null)
        {
            Text text = gameObject.transform.Find("SetupUI/TextEnemyAtkRate").GetComponent<Text>();
            if (text != null)
            {
                float enemy_atk_rate = slider.value / (float)slider.maxValue;
                text.text = "個別ＨＰ敵攻撃補正 x" + enemy_atk_rate.ToString();
            }
        }
    }

    /// <summary>
    /// パーティキャラ設定ＵＩから値を反映
    /// </summary>
    private void setup_from_screen()
    {
        gameObject.transform.Find("SetupUI").gameObject.SetActive(false);

        // バトルの動作モード
        int game_mode = gameObject.transform.Find("SetupUI/DropdownBattleMode").GetComponent<Dropdown>().value;
        m_IsKobetsuHP = (game_mode >= 1);
        m_KobetsuHPEnemyAttackAll = (game_mode == 1 || game_mode == 2);
        m_KobetsuHPEnemyTargetHate = (game_mode == 2 || game_mode == 4);
        float enemy_atk_rate = 1.0f;
        if (m_IsKobetsuHP)
        {
            Slider slider = gameObject.transform.Find("SetupUI/SliderEnemyAtkRate").GetComponent<Slider>();
            if (slider != null)
            {
                enemy_atk_rate = slider.value / (float)slider.maxValue;
            }
        }
        m_KobetsuHP_EnemyAtkRate = enemy_atk_rate;

        // プレイヤーパーティ
        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            BattleSceneStartCharaUI chara_ui = gameObject.transform.Find("SetupUI/" + menber_ids[idx] + "/Main/BattleDebugCharaSettingUI").GetComponent<BattleSceneStartCharaUI>();
            BattleSceneStartCharaUI chara_ui_link = gameObject.transform.Find("SetupUI/" + menber_ids[idx] + "/Link/BattleDebugCharaSettingUI").GetComponent<BattleSceneStartCharaUI>();

            PlayerCharaData player_chara_data = m_BattleData.m_PlayerParty.m_PlayerCharas[idx];
            if (player_chara_data != null)
            {
                player_chara_data.m_CharaID = 0;
                player_chara_data.m_LinkCharaID = 0;
                if (chara_ui.isOn())
                {
                    int chara_id = chara_ui.getCharaID();
                    MasterDataParamChara param_chara = BattleParam.m_MasterDataCache.useCharaParam((uint)chara_id);
                    if (param_chara != null)
                    {
                        player_chara_data.m_CharaID = chara_id;
                        player_chara_data.m_CharaLevel = chara_ui.getCharaLevel();

                        if (chara_ui_link.isOn())
                        {
                            int chara_id_link = chara_ui_link.getCharaID();
                            MasterDataParamChara param_chara_link = BattleParam.m_MasterDataCache.useCharaParam((uint)chara_id_link);
                            if (param_chara_link != null)
                            {
                                player_chara_data.m_LinkCharaID = chara_id_link;
                                player_chara_data.m_LinkCharaLevel = chara_ui_link.getCharaLevel();
                            }
                        }
                    }
                }
            }
        }

        // 敵設定
        if (m_IsEnableEnemyUI == false)
        {
            return;
        }

        int[] wrk_enemy_group_ids = new int[enemy_gropus.Length];
        int enemy_group_count = 0;
        for (int idx = 0; idx < enemy_gropus.Length; idx++)
        {
            BattleSceneStartEnemyUI enemy_ui = gameObject.transform.Find("SetupUI/" + enemy_gropus[idx]).GetComponent<BattleSceneStartEnemyUI>();
            if (enemy_ui != null)
            {
                int enemy_group_id = enemy_ui.getEnemyGroupID();
                if (enemy_group_id != 0)
                {
                    wrk_enemy_group_ids[enemy_group_count] = enemy_group_id;
                    enemy_group_count++;
                }
            }
        }

        m_EnemyPartyIDs = new int[enemy_group_count];
        for (int idx = 0; idx < m_EnemyPartyIDs.Length; idx++)
        {
            m_EnemyPartyIDs[idx] = wrk_enemy_group_ids[idx];
        }
    }

    enum ScenePhase
    {
        START,
        INIT,
        PLAYER_SETUP,
        PLAYER_SETUP_WAIT,
        INIT_PLAYER,
        INIT_ENEMY,
        INIT_BATTLE_MANAGER,
        BATTILING,
    }
    private ScenePhase m_ScenePhase = ScenePhase.START;

    // Update is called once per frame
    void Update()
    {
        switch (m_ScenePhase)
        {
            case ScenePhase.START:
                MasterDataEnemyGroup[] master_data_array = BattleParam.m_MasterDataCache.getAllEnemyGroup();
                if (master_data_array != null)
                {
                    m_IsEnableEnemyUI = true;
                }
                initPlayerParty1();

                m_ScenePhase = ScenePhase.INIT;
                break;

            case ScenePhase.INIT:
                m_ScenePhase = ScenePhase.PLAYER_SETUP;
                break;

            case ScenePhase.PLAYER_SETUP:
                setup_for_screen();
                m_ScenePhase = ScenePhase.PLAYER_SETUP_WAIT;
                break;

            case ScenePhase.PLAYER_SETUP_WAIT:
                // OnPushButtonStart() が呼ばれれば進む.
                update_setup_ui();
                break;

            case ScenePhase.INIT_PLAYER:
                BattleParam.m_MasterDataCache.clearCachePlayerAll();
                BattleParam.m_MasterDataCache.clearCacheEnemyAll();
                setup_from_screen();
                initPlayerParty2();

                BattleParam.QuestInitialize(null);

                m_ScenePhase = ScenePhase.INIT_ENEMY;
                break;

            case ScenePhase.INIT_ENEMY:
                resetBattleRequest();
                createBattleRequest();
                m_ScenePhase = ScenePhase.INIT_BATTLE_MANAGER;
                break;

            case ScenePhase.INIT_BATTLE_MANAGER:
                {
                    GameObject effect_prefab = Resources.Load<GameObject>("Prefab/BattleScene/InGameEffectPrefab");
                    GameObject effect_obj = Instantiate(effect_prefab); // effect_prefabのコンポーネントを書き換えると以前に書き換えた情報が残っているので別オブジェクトを一回生成
                    SceneObjReferGameMainEffect effect_assign = effect_obj.GetComponent<SceneObjReferGameMainEffect>();
                    SceneObjReferGameMain.Instance.setEffectAssignObj(effect_assign);
                }
                BattleParam.BattleInitialize(BattleParam.m_BattleRequest, null);
                BattleParam.setKensyoParam(m_SkillTurnCondition, m_KobetsuHP_EnemyAtkRate, m_KobetsuHPEnemyAttackAll, m_KobetsuHPEnemyTargetHate);
                BattleParam.BattleStart();
                m_ScenePhase = ScenePhase.BATTILING;
                break;

            case ScenePhase.BATTILING:
                if (BattleParam.getBattlePhase() == BattleParam.BattlePhase.NOT_BATTLE)
                {
                    // クリアした
                    bool is_next_battle = createBattleRequest();
                    if (is_next_battle)
                    {
                        // 次の戦闘がある
                        m_ScenePhase = ScenePhase.INIT_BATTLE_MANAGER;
                    }
                    else
                    {
                        // 終了
                        BattleParam.endBattleScene();
                        m_ScenePhase = ScenePhase.INIT;
                    }
                }
                if (BattleParam.getBattlePhase() == BattleParam.BattlePhase.RETIRE)
                {
                    // ゲームオーバー
                    BattleParam.endBattleScene();
                    m_ScenePhase = ScenePhase.INIT;
                }
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// バトルスタート
    /// </summary>
    public void OnPushButtonStart()
    {
        if (m_ScenePhase == ScenePhase.PLAYER_SETUP_WAIT)
        {
            InGameUtil.RemoveLocalData();

            m_ScenePhase = ScenePhase.INIT_PLAYER;
        }
    }

    /// <summary>
    /// 中断復帰でバトルスタート
    /// </summary>
    public void OnPushButtonStartRestore()
    {
        if (m_ScenePhase == ScenePhase.PLAYER_SETUP_WAIT)
        {
            InGameUtil.SetLocalData();

            m_ScenePhase = ScenePhase.INIT_PLAYER;
        }
    }

    public void OnPushButtonRetire()
    {
        if (m_ScenePhase == ScenePhase.BATTILING)
        {
            BattleParam.endBattleScene();
            m_ScenePhase = ScenePhase.INIT;
        }
    }

    public void OnPushButtonDebugLog()
    {
        // クリップボードへコピー
        DebugBattleLog.exportLog();
    }

    public void OnPushButtonDebugLogClear()
    {
        DebugBattleLog.clearLog();
    }
}
