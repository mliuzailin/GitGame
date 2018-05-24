using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerDataDefine;

public class SceneTestGameMain : SceneMode<SceneTestGameMain>
{
    enum eStatus : int
    {
        NONE = -1,
        API_QUEST_START,
        DATA_CREATE,
        DATA_RESTORE,
    };

    public Sprite[] m_Button;
    public Text[] m_Text;
    public TestGameManager m_TestGameManager = null;
    public InputField[] m_InputField = null;

    private bool m_Switch;
    private bool m_Start;
    private eStatus m_Status;
    private uint m_HelperIndex;

#if UNITY_EDITOR && BUILD_TYPE_DEBUG
    public override void OnInitialized()
    {
        base.OnInitialized();
        m_Start = true;
        m_TestGameManager.Message_text = "";
    }

    protected override void Awake()
    {
        base.Awake();
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_Status = eStatus.NONE;
        m_Start = false;
        if (DebugOptionInGame.Instance != null)
        {
            if (m_TestGameManager != null)
            {
                if (DebugOptionInGame.Instance.m_QuestIndex < 0) DebugOptionInGame.Instance.m_QuestIndex = 0;
                uint area_id = 0;
                {
                    m_TestGameManager.Quest2_switch = m_Button[1];
                    MasterDataQuest2[] master = null;
                    if (DebugOptionInGame.Instance.m_Quest2fromJson == true)
                        master = DebugOptionInGame.Instance.masterDataQuest2;
                    else
                        master = MasterFinder<MasterDataQuest2>.Instance.GetAll();
                    if (master != null)
                    {
                        m_TestGameManager.Quest_name = master[DebugOptionInGame.Instance.m_QuestIndex].quest_name;
                        area_id = master[DebugOptionInGame.Instance.m_QuestIndex].area_id;
                        DebugOptionInGame.Instance.inGameDebugDO.m_QuestId = master[DebugOptionInGame.Instance.m_QuestIndex].fix_id;
                    }
                }
                MasterDataArea amaster = MasterFinder<MasterDataArea>.Instance.Find((int)area_id);
                if (amaster != null)
                {
                    m_TestGameManager.Area_name = amaster.area_name;
                    m_TestGameManager.Area_detail = amaster.area_detail;
                }
                m_Text[0].text = DebugOptionInGame.Instance.m_QuestIndex.ToString();
                m_Text[1].text = DebugOptionInGame.Instance.inGameDebugDO.m_QuestId.ToString();
                if (DebugOptionInGame.Instance.inGameDebugDO.m_Restore == false)
                {
                    m_TestGameManager.Restore_switch = m_Button[0];
                }
                else
                {
                    m_TestGameManager.Restore_switch = m_Button[1];
                }
                if (DebugOptionInGame.Instance.inGameDebugDO.m_UseAPI == false)
                {
                    m_TestGameManager.Api_switch = m_Button[0];
                }
                else
                {
                    m_TestGameManager.Api_switch = m_Button[1];
                }
            }
            DebugOptionInGame.Instance.m_TestMode = true;
        }
        if (LocalSaveManager.Instance.LoadFuncResultVersionCheck() == false)
        {
            LocalSaveManager.Instance.SaveFuncGoesToMenuResult(null);
            LocalSaveManager.Instance.SaveFuncResultVersion();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (DebugOptionInGame.Instance == null)
        {
            return;
        }

        if (m_TestGameManager == null)
        {
            return;
        }

        switch (m_Status)
        {
            case eStatus.API_QUEST_START:
                {
                    if (DebugOptionInGame.Instance.inGameDebugDO.m_UseAPI == true)
                    {
                        m_HelperIndex = RandManager.GetRand(0, (uint)(UserDataAdmin.Instance.m_StructHelperList.Length - 1));
                        PacketStructFriend cHelper = null;
                        if (UserDataAdmin.Instance.m_StructFriendList.Length > 0
                        && UserDataAdmin.Instance.m_StructFriendList[0] != null)
                        {
                            cHelper = UserDataAdmin.Instance.m_StructFriendList[0];
                        }
                        else if (UserDataAdmin.Instance.m_StructHelperList.Length > 0)
                        {
                            cHelper = UserDataAdmin.Instance.m_StructHelperList[m_HelperIndex];
                        }
                        {
                            ServerDataUtilSend.SendPacketAPI_Quest2Start(
                                DebugOptionInGame.Instance.inGameDebugDO.m_QuestId,
                                0,
                                cHelper.user_id,
                                cHelper.unit,
                                false,
                                UserDataAdmin.Instance.m_StructPlayer.unit_party_current,
                                0,
                                0,
                                null,
                                false
                                )
                            .setSuccessAction(_data =>
                            {
                                if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build == null)
                                {
                                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build = new SceneGoesParamToQuest2Build();
                                }
                                SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild = _data.GetResult<RecvQuest2Start>().result.quest;
                                m_Status = eStatus.DATA_CREATE;
                            })
                            .setErrorAction(data =>
                            {
                                Debug.LogErrorFormat("[TestGameMain] Quest2Start API Error [{0}] : QuestId = {1}", data.m_PacketCode.ToString(), DebugOptionInGame.Instance.inGameDebugDO.m_QuestId.ToString());
                                m_TestGameManager.Message_text = "API Error\r\n [" + data.m_PacketCode.ToString() + "]";
                            })
                            .SendStart();
                            m_Status = eStatus.NONE;
                        }
                    }
                    else
                    {
                        if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build == null)
                        {
                            SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build = new SceneGoesParamToQuest2Build();
                        }
                        {
                            MasterDataQuest2 masterDataQuest2 = MasterDataUtil.GetQuest2ParamFromID(DebugOptionInGame.Instance.inGameDebugDO.m_QuestId);
                            PacketStructQuest2Build cQuestBuild = new PacketStructQuest2Build();
                            int floor_size = 2;
                            //----------------------------------------
                            // 最終的な構築情報を格納する領域を確保
                            //----------------------------------------
                            TemplateList<PacketStructQuest2BuildBattle> acQuestBuildBattle = new TemplateList<PacketStructQuest2BuildBattle>();
                            TemplateList<PacketStructQuest2BuildDrop> acQuestBuildDrop = new TemplateList<PacketStructQuest2BuildDrop>();
                            acQuestBuildBattle.Alloc(64);

                            int nFloorDataAccess = 0;
                            cQuestBuild.boss = new int[floor_size];

                            for (int i = 0; i < floor_size; i++)
                            {
                                nFloorDataAccess = (i - 1);
                                if (nFloorDataAccess < 0)
                                {
                                    continue;
                                }
                                //----------------------------------------
                                //	0番目要素ダミーデータの入力
                                //----------------------------------------
                                PacketStructQuest2BuildBattle build_param_battle = new PacketStructQuest2BuildBattle();
                                if (build_param_battle != null)
                                {
                                    build_param_battle.unique_id = 0;
                                    build_param_battle.enemy_list = null;
                                    build_param_battle.drop_list = null;
                                    build_param_battle.chain = 0;
                                    build_param_battle.chain_turn_offset = 0;
                                    build_param_battle.bgm_id = 0;

                                    acQuestBuildBattle.Add(build_param_battle);
                                }

                                //----------------------------------------
                                // 戦闘情報を設定
                                //----------------------------------------
                                int battle_num = MasterDataUtil.GetQuest2BattleNum(masterDataQuest2.fix_id);
                                for (int j = 0; j < battle_num; j++)
                                {
                                    uint enemy_group_id = MasterDataUtil.GetQuest2EnemyGroup(masterDataQuest2.fix_id, j);
                                    if (enemy_group_id == 0)
                                    {
                                        continue;
                                    }

                                    MasterDataEnemyGroup acMasterGroupEnemy = ServerFogery_GetEnemyGroupFromID(enemy_group_id);
                                    if (acMasterGroupEnemy == null)
                                    {
                                        Debug.LogError("EnemyGroup not found id = " + enemy_group_id);
                                        continue;
                                    }
                                    CreateQuestBuildBattle(
                                                i
                                            , ref acQuestBuildBattle
                                            , ref acQuestBuildDrop
                                            , acMasterGroupEnemy
                                            );
                                }
                                //----------------------------------------
                                // ボス戦闘情報を設定
                                //----------------------------------------
                                cQuestBuild.boss[i] = 0;
                                if (masterDataQuest2.boss_group_id > 0)
                                {
                                    MasterDataEnemyGroup cBossEnemyGroup = ServerFogery_GetEnemyGroupFromID(masterDataQuest2.boss_group_id);
                                    if (cBossEnemyGroup != null)
                                    {
                                        cQuestBuild.boss[i] = CreateQuestBuildBattle(
                                                                                i
                                                                            , ref acQuestBuildBattle
                                                                            , ref acQuestBuildDrop
                                                                            , cBossEnemyGroup
                                                                            );
                                    }
                                    else
                                    {
                                        Debug.LogError("EnemyGroup not found Boss id = " + masterDataQuest2.boss_group_id);
                                    }
                                }
                            }

                            TemplateList<MasterDataParamEnemy> e_param_list = new TemplateList<MasterDataParamEnemy>();
                            TemplateList<MasterDataEnemyActionParam> e_act_param_list = new TemplateList<MasterDataEnemyActionParam>();
                            TemplateList<MasterDataEnemyActionTable> e_act_table_list = new TemplateList<MasterDataEnemyActionTable>();

                            for (int i = 0; i < acQuestBuildBattle.m_BufferSize; i++)
                            {
                                if (acQuestBuildBattle[i] == null)
                                {
                                    continue;
                                }

                                if (acQuestBuildBattle[i].enemy_list == null)
                                {
                                    continue;
                                }

                                for (int j = 0; j < acQuestBuildBattle[i].enemy_list.Length; j++)
                                {
                                    if (acQuestBuildBattle[i].enemy_list[j] == 0)
                                    {
                                        continue;
                                    }

                                    MasterDataParamEnemy enemy_param = ServerForgery_GetEnemyParamFromID(acQuestBuildBattle[i].enemy_list[j]);
                                    e_param_list.Add(enemy_param);

                                    int[] table_id = {  enemy_param.act_table1,
                                        enemy_param.act_table2,
                                        enemy_param.act_table3,
                                        enemy_param.act_table4,
                                        enemy_param.act_table5,
                                        enemy_param.act_table6,
                                        enemy_param.act_table7,
                                        enemy_param.act_table8  };

                                    for (int k = 0; k < table_id.Length; k++)
                                    {
                                        if (table_id[k] == 0)
                                        {
                                            continue;
                                        }

                                        MasterDataEnemyActionTable table = ServerFogery_GetEnemyActionTable(table_id[k]);
                                        e_act_table_list.Add(table);

                                        int[] action_id = { table.action_param_id1,
                                            table.action_param_id2,
                                            table.action_param_id3,
                                            table.action_param_id4,
                                            table.action_param_id5,
                                            table.action_param_id6,
                                            table.action_param_id7,
                                            table.action_param_id8 };

                                        for (int l = 0; l < action_id.Length; l++)
                                        {
                                            e_act_param_list.Add(GetEnemyActionParam(action_id[l]));
                                        }
                                    }
                                }
                            }


                            //----------------------------------------
                            // 構築した動的要素を配列化して受け渡し
                            //----------------------------------------
                            cQuestBuild.list_drop = acQuestBuildDrop.ToArray();
                            cQuestBuild.list_battle = acQuestBuildBattle.ToArray();
                            cQuestBuild.list_e_param = e_param_list.ToArray();
                            cQuestBuild.list_e_actparam = e_act_param_list.ToArray();
                            cQuestBuild.list_e_acttable = e_act_table_list.ToArray();

                            SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild = cQuestBuild;
                        }
                        m_Status = eStatus.DATA_CREATE;
                    }
                }
                break;
            case eStatus.DATA_CREATE:
                {

                    {
                        MasterDataQuest2 masterDataQuest = MasterDataUtil.GetQuest2ParamFromID(DebugOptionInGame.Instance.inGameDebugDO.m_QuestId);
                        //----------------------------------------
                        // ダミーパラメータを設定
                        //----------------------------------------
                        if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2 == null)
                        {
                            SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = new SceneGoesParamToQuest2();
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

                        if (DebugOptionInGame.Instance.inGameDebugDO.m_DebugParty == false
                        && UserDataAdmin.Instance != null
                        && UserDataAdmin.Instance.m_StructHelperList.IsNullOrEmpty() != true
                        && UserDataAdmin.Instance.m_StructPartyAssign.IsNullOrEmpty() != true)
                        {
                            PacketStructFriend cHelper = UserDataAdmin.Instance.m_StructHelperList[m_HelperIndex];
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
                                if (acUnitStruct[i] != null)
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
                        }
                        else
                        {
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

                        LocalSaveManager.Instance.SaveFuncGoesToQuest2Start(SceneGoesParam.Instance.m_SceneGoesParamToQuest2);
                        LocalSaveManager.Instance.SaveFuncGoesToQuest2Restore(null);
                        DebugOptionInGame.Instance.m_Quest2Build = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild;
                        SceneCommon.Instance.ChangeScene(SceneType.SceneQuest2);
                    }
                    m_Status = eStatus.NONE;
                }
                break;
            case eStatus.DATA_RESTORE:
                {
                    {
                        SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = LocalSaveManager.Instance.LoadFuncGoesToQuest2Start();
                        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = LocalSaveManager.Instance.LoadFuncGoesToQuest2Restore();
                        if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build == null)
                        {
                            SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build = new SceneGoesParamToQuest2Build();
                        }
                        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild = DebugOptionInGame.Instance.m_Quest2Build;
                        SceneCommon.Instance.ChangeScene(SceneType.SceneQuest2);
                    }
                    m_Status = eStatus.NONE;
                }
                break;
            default:
                break;
        }
    }

    public void OnStart()
    {
        if (m_Start == false)
        {
            return;
        }

        if (DebugOptionInGame.Instance.inGameDebugDO.m_Restore == true)
        {
            if (LocalSaveManager.Instance != null)
            {
                if (LocalSaveManager.Instance.LoadFuncGoesToQuest2RestoreChk() ||
                    LocalSaveManager.Instance.LoadFuncGoesToQuest2StartChk())
                {
                    m_Status = eStatus.DATA_RESTORE;
                    return;
                }
            }
        }

        m_Status = eStatus.API_QUEST_START;
    }

    public void OnRestoreButton()
    {
        if (DebugOptionInGame.Instance == null)
        {
            return;
        }

        if (m_TestGameManager == null)
        {
            return;
        }

        if (m_Start == false)
        {
            return;
        }

        if (DebugOptionInGame.Instance.inGameDebugDO.m_Restore == false)
        {
            DebugOptionInGame.Instance.inGameDebugDO.m_Restore = true;
            m_TestGameManager.Restore_switch = m_Button[1];
        }
        else
        {
            DebugOptionInGame.Instance.inGameDebugDO.m_Restore = false;
            m_TestGameManager.Restore_switch = m_Button[0];
        }
    }

    public void OnAPIButton()
    {
        if (DebugOptionInGame.Instance == null)
        {
            return;
        }

        if (m_TestGameManager == null)
        {
            return;
        }

        if (m_Start == false)
        {
            return;
        }

        if (DebugOptionInGame.Instance.inGameDebugDO.m_UseAPI == false)
        {
            DebugOptionInGame.Instance.inGameDebugDO.m_UseAPI = true;
            m_TestGameManager.Api_switch = m_Button[1];
        }
        else
        {
            DebugOptionInGame.Instance.inGameDebugDO.m_UseAPI = false;
            m_TestGameManager.Api_switch = m_Button[0];
        }
    }

    public void OnQuestIndex(int num)
    {
        if (DebugOptionInGame.Instance == null)
        {
            return;
        }

        if (m_TestGameManager == null)
        {
            return;
        }

        if (m_Start == false)
        {
            return;
        }

        int index = DebugOptionInGame.Instance.m_QuestIndex;
        index += num;
        int max = 0;
        {
            MasterDataQuest2[] master = null;
            if (DebugOptionInGame.Instance.m_Quest2fromJson == true)
                master = DebugOptionInGame.Instance.masterDataQuest2;
            else
                master = MasterFinder<MasterDataQuest2>.Instance.GetAll();

            if (master.IsNullOrEmpty() == true)
            {
                return;
            }

            max = master.Length;
        }
        if (index < 0)
        {
            if (DebugOptionInGame.Instance.m_QuestIndex != 0)
            {
                index = 0;
            }
            else
            {
                index = max - 1;
            }
        }
        if (index >= max)
        {
            if (DebugOptionInGame.Instance.m_QuestIndex != max - 1)
            {
                index = max - 1;
            }
            else
            {
                index = 0;
            }
        }
        DebugOptionInGame.Instance.m_QuestIndex = index;

        changeText();
    }

    private void changeText()
    {
        uint area_id = 0;
        {
            MasterDataQuest2[] master = null;
            if (DebugOptionInGame.Instance.m_Quest2fromJson == true)
                master = DebugOptionInGame.Instance.masterDataQuest2;
            else
                master = MasterFinder<MasterDataQuest2>.Instance.GetAll();
            if (master != null)
            {
                if (DebugOptionInGame.Instance.m_QuestIndex >= master.Length) DebugOptionInGame.Instance.m_QuestIndex = master.Length - 1;
                m_TestGameManager.Quest_name = master[DebugOptionInGame.Instance.m_QuestIndex].quest_name;
                area_id = master[DebugOptionInGame.Instance.m_QuestIndex].area_id;
                DebugOptionInGame.Instance.inGameDebugDO.m_QuestId = master[DebugOptionInGame.Instance.m_QuestIndex].fix_id;
            }
        }
        MasterDataArea amaster = MasterFinder<MasterDataArea>.Instance.Find((int)area_id);
        if (amaster != null)
        {
            m_TestGameManager.Area_name = amaster.area_name;
            m_TestGameManager.Area_detail = amaster.area_detail;
        }
        m_Text[0].text = DebugOptionInGame.Instance.m_QuestIndex.ToString();
        m_Text[1].text = DebugOptionInGame.Instance.inGameDebugDO.m_QuestId.ToString();
    }

    public void OnQuestIndex()
    {
        if (m_Start == false)
        {
            return;
        }

        int index = m_InputField[0].text.ToInt(-1);
        if (index < 0)
        {
            return;
        }

        m_InputField[0].text = null;
        DebugOptionInGame.Instance.m_QuestIndex = index;

        changeText();
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	エネミーグループパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static private MasterDataEnemyGroup ServerFogery_GetEnemyGroupFromID(uint unID)
    {
        if (MasterFinder<MasterDataEnemyGroup>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataEnemyGroup>.Instance.Find(unID.ToString());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	敵アクションテーブル取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static private MasterDataEnemyActionTable ServerFogery_GetEnemyActionTable(int unID)
    {
        if (MasterFinder<MasterDataEnemyActionTable>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataEnemyActionTable>.Instance.Find(unID.ToString());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	敵アクションパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static private MasterDataEnemyActionParam GetEnemyActionParam(int unID)
    {
        if (MasterFinder<MasterDataEnemyActionParam>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataEnemyActionParam>.Instance.Find(unID.ToString());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	エネミーパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataParamEnemy ServerForgery_GetEnemyParamFromID(uint unID)
    {
        if (MasterFinder<MasterDataParamEnemy>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataParamEnemy>.Instance.Find(unID.ToString());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	クエスト構成情報を捏造：戦闘情報
	*/
    //----------------------------------------------------------------------------
    static private int CreateQuestBuildBattle(
        int nFloor
    , ref TemplateList<PacketStructQuest2BuildBattle> rcBattleList
    , ref TemplateList<PacketStructQuest2BuildDrop> rcDropList
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
        MasterDataParamEnemy cEnemyData1 = ServerForgery_GetEnemyParamFromID(cEnemyGroup.enemy_id_1);
        MasterDataParamEnemy cEnemyData2 = ServerForgery_GetEnemyParamFromID(cEnemyGroup.enemy_id_2);
        MasterDataParamEnemy cEnemyData3 = ServerForgery_GetEnemyParamFromID(cEnemyGroup.enemy_id_3);
        MasterDataParamEnemy cEnemyData4 = ServerForgery_GetEnemyParamFromID(cEnemyGroup.enemy_id_4);
        MasterDataParamEnemy cEnemyData5 = ServerForgery_GetEnemyParamFromID(cEnemyGroup.enemy_id_5);
        MasterDataParamEnemy cEnemyData6 = ServerForgery_GetEnemyParamFromID(cEnemyGroup.enemy_id_6);
        MasterDataParamEnemy cEnemyData7 = ServerForgery_GetEnemyParamFromID(cEnemyGroup.enemy_id_7);
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
        PacketStructQuest2BuildBattle cBattle = new PacketStructQuest2BuildBattle();
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
                PacketStructQuest2BuildDrop cDropData = new PacketStructQuest2BuildDrop();
                cDropData.unit_id = cEnemyFixParam.drop_unit_id;
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
            MasterDataEnemyGroup cChainEnemyGroup = ServerFogery_GetEnemyGroupFromID(cEnemyGroup.chain_id);
            cBattle.chain = CreateQuestBuildBattle(nFloor, ref rcBattleList, ref rcDropList, cChainEnemyGroup);
        }

        //----------------------------------------
        // 情報を加算して管理番号を返す
        //----------------------------------------
        cBattle.unique_id = rcBattleList.m_BufferSize;
        rcBattleList.Add(cBattle);


        return cBattle.unique_id;
    }
#endif
}
