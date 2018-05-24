//#define USE_LOCAL_ENEMY_MASTER	// ローカルに置いた敵マスターを使用する
//#define USE_DEBUG_JSON_MASTER_DATA	// バトル関連のマスターデータを読む際ローカルのJsonファイルから読むかどうか（スキルや敵の検証作業用）

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// バトルで使用するマスターデータを取得しキャッシュするクラス
/// マスターデータは巨大なので毎回全検索して取得すると重いので、その時のバトルで使用するデータのみをキャッシュしておく.
/// </summary>
public class MasterDataCache
{
    private MasterDataCache2<MasterDataArea> m_AreaParam = new MasterDataCache2<MasterDataArea>();
    private MasterDataCache2<MasterDataStatusAilmentParam> m_AilmentParam = new MasterDataCache2<MasterDataStatusAilmentParam>();

    private MasterDataCache2<MasterDataParamChara> m_CharaParam = new MasterDataCache2<MasterDataParamChara>();
    private MasterDataCache2<MasterDataSkillPassive> m_SkillPassive = new MasterDataCache2<MasterDataSkillPassive>();                                                           //!< スキル発動判定：パッシブスキル一覧
    private MasterDataCache2<MasterDataSkillBoost> m_SkillBoost = new MasterDataCache2<MasterDataSkillBoost>();                                                         //!< スキル発動判定：ブーストスキル一覧
    private MasterDataCache2<MasterDataSkillLeader> m_SkillLeader = new MasterDataCache2<MasterDataSkillLeader>();                                                          //!< スキル発動判定：リーダースキル一覧
    private MasterDataCache2<MasterDataSkillLimitBreak> m_SkillLimitBreak = new MasterDataCache2<MasterDataSkillLimitBreak>();                                                          //!< スキル発動判定：リミブレスキル一覧
    private MasterDataCache2<MasterDataSkillActive> m_SkillActive = new MasterDataCache2<MasterDataSkillActive>();                                                            //!< スキル発動判定：アクティブスキル一覧

    // 現在は移植中なためローカルのデータを使用しています.
    private MasterDataCache2<MasterDataParamEnemy> m_EnemyParam = new MasterDataCache2<MasterDataParamEnemy>();                                                         //!<
    private MasterDataCache2<MasterDataEnemyActionTable> m_EnemyActionTable = new MasterDataCache2<MasterDataEnemyActionTable>();                                                            //!< スキル発動判定：アクティブスキル一覧
    private MasterDataCache2<MasterDataEnemyActionParam> m_EnemyActionParam = new MasterDataCache2<MasterDataEnemyActionParam>();                                                            //!< スキル発動判定：アクティブスキル一覧
    private MasterDataCache2<MasterDataEnemyAbility> m_EnemyAbility = new MasterDataCache2<MasterDataEnemyAbility>();                                                            //!< スキル発動判定：アクティブスキル一覧
#if USE_LOCAL_ENEMY_MASTER
    private MasterDataCache2<MasterDataEnemyGroup> m_EnemyGroup = new MasterDataCache2<MasterDataEnemyGroup>();                                                            //!< スキル発動判定：アクティブスキル一覧
#endif //USE_LOCAL_ENEMY_MASTER

    public void clearCachePlayerAll()
    {
        clearCacheAreaParam();
        clearCacheAilmentParam();
        clearCacheCharaParam();
        clearCacheSkillPassive();
        clearCacheSkillBoost();
        clearCacheSkillLeader();
        clearCacheSkillLimitBreak();
        clearCacheSkillActive();
    }

    public void clearCacheEnemyAll()
    {
        clearCacheEnemyParam();
        clearCacheEnemyActionTable();
        clearCacheEnemyActionParam();
        clearCacheEnemyAbility();

#if USE_LOCAL_ENEMY_MASTER
        m_EnemyGroup.clearCache();
#endif //USE_LOCAL_ENEMY_MASTER
    }

    //-------------
    public void clearCacheAreaParam()
    {
        m_AreaParam.clearCache();
    }

    public MasterDataArea useAreaParam(uint id)
    {
        return m_AreaParam.use(id);
    }

    //-------------
    public void clearCacheAilmentParam()
    {
        m_AilmentParam.clearCache();
    }

    public MasterDataStatusAilmentParam useAilmentParam(uint id)
    {
        return m_AilmentParam.use(id);
    }
    //-------------
    public void clearCacheCharaParam()
    {
        m_CharaParam.clearCache();
    }

    public MasterDataParamChara useCharaParam(uint id)
    {
        return m_CharaParam.use(id);
    }

    public MasterDataParamChara addCharaParam(MasterDataParamChara master_data)
    {
        return m_CharaParam.add(master_data);
    }


    //-------------
    public void clearCacheSkillPassive()
    {
        m_SkillPassive.clearCache();
    }

    public MasterDataSkillPassive useSkillPassive(uint id)
    {
        return m_SkillPassive.use(id);
    }

    //-------------
    public void clearCacheSkillBoost()
    {
        m_SkillBoost.clearCache();
    }

    public MasterDataSkillBoost useSkillBoost(uint id)
    {
        return m_SkillBoost.use(id);
    }

    //-------------
    public void clearCacheSkillLeader()
    {
        m_SkillLeader.clearCache();
    }

    public MasterDataSkillLeader useSkillLeader(uint id)
    {
        return m_SkillLeader.use(id);
    }

    //-------------
    public void clearCacheSkillLimitBreak()
    {
        m_SkillLimitBreak.clearCache();
    }

    public MasterDataSkillLimitBreak useSkillLimitBreak(uint id)
    {
        return m_SkillLimitBreak.use(id);
    }

    public void addSkillLimitBreak(MasterDataSkillLimitBreak master_data_skill_limit_break)
    {
        m_SkillLimitBreak.add(master_data_skill_limit_break);
    }

#if UNITY_EDITOR
    public string getSkillLimitBreakName(uint id)
    {
        MasterDataSkillLimitBreak master_data = useSkillLimitBreak(id);
        if (master_data != null)
        {
            return master_data.name;
        }

        return null;
    }
#endif

    //-------------
    public void clearCacheSkillActive()
    {
        m_SkillActive.clearCache();
    }

    public MasterDataSkillActive useSkillActive(uint id)
    {
        return m_SkillActive.use(id);
    }

    public int _getSkillActiveCount()
    {
        return m_SkillActive.getCachedDataCount();
    }

    public MasterDataSkillActive _getSkillActiveByIndex(int idx)
    {
        return m_SkillActive.getCachedDataByIndex(idx);
    }

#if UNITY_EDITOR
    public string getSkillActiveName(uint id)
    {
        MasterDataSkillActive master_data = useSkillActive(id);
        if (master_data != null)
        {
            return master_data.name;
        }

        return null;
    }
#endif
    //-------------
    public void clearCacheEnemyParam()
    {
        m_EnemyParam.clearCache();
    }

    public MasterDataParamEnemy useEnemyParam(uint id)
    {
        return m_EnemyParam.use(id);
    }

    public MasterDataParamEnemy[] getAllEnemyParam()
    {
#if USE_LOCAL_ENEMY_MASTER
        return m_EnemyParam._getMasterDataAll();
#else
        return null;
#endif
    }

    //-------------
    public void clearCacheEnemyActionTable()
    {
        m_EnemyActionTable.clearCache();
    }

    public MasterDataEnemyActionTable useEnemyActionTable(uint id)
    {
        return m_EnemyActionTable.use(id);
    }

#if BUILD_TYPE_DEBUG
    public void setDebugAllDataEnemyActionTable(MasterDataEnemyActionTable[] all_master_data_array)
    {
        m_EnemyActionTable.setDebugAllData(all_master_data_array);
    }
#endif

    //-------------
    public void clearCacheEnemyActionParam()
    {
        m_EnemyActionParam.clearCache();
    }

    public MasterDataEnemyActionParam useEnemyActionParam(uint id)
    {
        return m_EnemyActionParam.use(id);
    }

#if BUILD_TYPE_DEBUG
    public void setDebugAllDataEnemyActionParam(MasterDataEnemyActionParam[] all_master_data_array)
    {
        m_EnemyActionParam.setDebugAllData(all_master_data_array);
    }
#endif

    //-------------
    public void clearCacheEnemyAbility()
    {
        m_EnemyAbility.clearCache();
    }

    public MasterDataEnemyAbility useEnemyAbility(uint id)
    {
        return m_EnemyAbility.use(id);
    }

    public MasterDataEnemyGroup useEnemyGroup(uint id)
    {
#if USE_LOCAL_ENEMY_MASTER
        return m_EnemyGroup.use(id);
#else
        return null;
#endif //USE_LOCAL_ENEMY_MASTER
    }
    public MasterDataEnemyGroup[] getAllEnemyGroup()
    {
#if USE_LOCAL_ENEMY_MASTER
        return m_EnemyGroup._getMasterDataAll();
#else
        return null;
#endif //USE_LOCAL_ENEMY_MASTER
    }




    /**
     * プレイヤー側で使用しているマスターデータをキャッシュ.
     */
    public void CachePlayerMasterData(CharaParty player_party)
    {
        clearCachePlayerAll();

        // 常駐スキル.
        MasterDataSkillActive[] master_data_skill_active_array = null;
#if BUILD_TYPE_DEBUG && USE_DEBUG_JSON_MASTER_DATA
        if (BattleParam.m_IsUseDebugJsonMasterData)
        {
            master_data_skill_active_array = BattleMasterDataFromJson.Instance.getMasterDataAll<MasterDataSkillActive>();
            for (int idx = 0; idx < master_data_skill_active_array.Length; idx++)
            {
                MasterDataSkillActive master_data_skill_active = master_data_skill_active_array[idx];
                if (master_data_skill_active.always == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    useSkillActive(master_data_skill_active.fix_id);
                }
            }
        }
        else
#endif    //USE_DEBUG_JSON_MASTER_DATA
        {
            MasterDataSkillActive[] resMasterDataSkillActiveArray = MasterFinder<MasterDataSkillActive>.Instance.SelectWhere("where always = ? ", MasterDataDefineLabel.BoolType.ENABLE).ToArray();
            if (resMasterDataSkillActiveArray != null)
            {
                foreach (MasterDataSkillActive resMasterDataSkillActive in resMasterDataSkillActiveArray)
                {
                    useSkillActive(resMasterDataSkillActive.fix_id);
                }
            }
        }

        for (int i = 0; i < player_party.getPartyMemberMaxCount(); i++)
        {
            CharaOnce chara = player_party.getPartyMember((GlobalDefine.PartyCharaIndex)i, CharaParty.CharaCondition.EXIST);
            if (chara == null)
            {
                continue;
            }

            if (!chara.m_bHasCharaMasterDataParam)
            {
                continue;
            }

            // キャラ情報を保存
            MasterDataParamChara master_data_param_chara = chara.m_CharaMasterDataParam;
            addCharaParam(master_data_param_chara);

            // リンクキャラを保存
            MasterDataParamChara master_data_param_link_chara = null;
            if (chara.m_LinkParam != null)
            {
                master_data_param_link_chara = useCharaParam(chara.m_LinkParam.m_CharaID);
            }

            //--------------------------------
            // スキル情報を保存		<アクティブ１>
            //--------------------------------
            {
                MasterDataSkillActive master_data_skill_active = useSkillActive(master_data_param_chara.skill_active0);
                if (master_data_skill_active != null)
                {
                    useSkillBoost(master_data_skill_active.skill_boost_id);
                }
            }

            //--------------------------------
            // スキル情報を保存		<アクティブ２>
            //--------------------------------
            {
                MasterDataSkillActive master_data_skill_active = useSkillActive(master_data_param_chara.skill_active1);
                if (master_data_skill_active != null)
                {
                    useSkillBoost(master_data_skill_active.skill_boost_id);
                }
            }

            //--------------------------------
            // スキル情報を保存		<リーダー>
            //--------------------------------
            {
                int loop_counter = 0;
                MasterDataSkillLeader master_data_skill_leader = useSkillLeader(master_data_param_chara.skill_leader);
                while (master_data_skill_leader != null)
                {
                    // 無限ループ検出
                    if (_checkinfiniteLoop(ref loop_counter))
                    {
                        master_data_skill_leader.add_fix_id = 0;
                        break;
                    }

                    master_data_skill_leader = useSkillLeader(master_data_skill_leader.add_fix_id);
                }
            }

            //--------------------------------
            // スキル情報を保存		<パッシブ>
            //--------------------------------
            {
                int loop_counter = 0;
                MasterDataSkillPassive master_data_skill_passive = useSkillPassive(master_data_param_chara.skill_passive);
                while (master_data_skill_passive != null)
                {
                    // 無限ループ検出
                    if (_checkinfiniteLoop(ref loop_counter))
                    {
                        master_data_skill_passive.add_fix_id = 0;
                        break;
                    }

                    master_data_skill_passive = useSkillPassive((uint)master_data_skill_passive.add_fix_id);
                }
            }

            //--------------------------------
            // スキル情報を保存		<リンクパッシブ>
            //--------------------------------
            // バトル中の判定には使わないが、ダイアログの表示に使われるので保存.
            useSkillPassive(master_data_param_chara.link_skill_passive);

            //--------------------------------
            // スキル情報を保存		<リミブレ>
            //--------------------------------
            {
                int loop_counter = 0;
                MasterDataSkillLimitBreak master_data_skill_limit_break = useSkillLimitBreak(master_data_param_chara.skill_limitbreak);
                while (master_data_skill_limit_break != null)
                {
                    // 無限ループ検出
                    if (_checkinfiniteLoop(ref loop_counter))
                    {
                        master_data_skill_limit_break.add_fix_id = 0;
                        break;
                    }

                    master_data_skill_limit_break = useSkillLimitBreak((uint)master_data_skill_limit_break.add_fix_id);
                }
            }

            //--------------------------------
            // スキル情報を保存		<リンク>
            //--------------------------------
            if (master_data_param_link_chara != null)
            {
                //--------------------------------
                // スキル情報を保存		<リンクスキル>
                //--------------------------------
                useSkillActive(master_data_param_link_chara.skill_active0);

                //--------------------------------
                // スキル情報を保存		<リンクパッシブ>
                //--------------------------------
                {
                    int loop_counter = 0;
                    MasterDataSkillPassive master_data_skill_passive = useSkillPassive(master_data_param_link_chara.link_skill_passive);
                    while (master_data_skill_passive != null)
                    {
                        // 無限ループ検出
                        if (_checkinfiniteLoop(ref loop_counter))
                        {
                            master_data_skill_passive.add_fix_id = 0;
                            break;
                        }

                        master_data_skill_passive = useSkillPassive((uint)master_data_skill_passive.add_fix_id);
                    }
                }
            }
        }
    }

    // 無限ループを検出
    private bool _checkinfiniteLoop(ref int counter)
    {
        const int ADD_SKILL_COUNT_LIMIT = 50;   // スキル連結数の上限
        counter++;
        if (counter >= ADD_SKILL_COUNT_LIMIT)
        {
#if BUILD_TYPE_DEBUG
            Dialog dloalog = DialogManager.Open1B_Direct(
                "スキルデータエラー",
                "リーダースキル・リミブレスキル・パッシブスキル\nのいずれかでスキル連結に\n無限ループが検出されました。\n\n現状ではスキルが正常に動作しません。",
                "common_button7",
                true, true
            ).SetOkEvent(() => { });
#endif //BUILD_TYPE_DEBUG

            return true;
        }
        return false;
    }
}


/// <summary>
/// バトルで使用するマスターデータを取得しキャッシュするクラス２
/// マスターデータは巨大なので毎回全検索して取得すると重いので、その時のバトルで使用するデータのみをキャッシュしておく.
/// ※本当は MasterDataCache ではなくこちらのクラスを使おうと思っていたが、今から直すと修正個所が多すぎるのでこのまま
/// </summary>
/// <typeparam name="T"></typeparam>
public class MasterDataCache2<T> where T : Master, new()
{
    private Dictionary<uint, T> m_Dictionary = null;
#if BUILD_TYPE_DEBUG
    private T[] m_DebugAllData = null;  // デバッグ用の一時的なマスターデータ。通常のマスターデータは取得せずこちらの中から取得
#endif

    public MasterDataCache2()
    {
        m_Dictionary = new Dictionary<uint, T>();
    }

    // キャッシュをクリア.
    public void clearCache()
    {
        m_Dictionary.Clear();
#if BUILD_TYPE_DEBUG
        m_DebugAllData = null;
#endif
    }

    // マスターデータを取得（同時にキャッシュもされます）
    public T use(uint id)
    {
        if (id == 0)
        {
            return null;
        }

        if (m_Dictionary == null)
        {
            m_Dictionary = new Dictionary<uint, T>();
        }

        if (m_Dictionary.ContainsKey(id))
        {
            return m_Dictionary[id];
        }

        T master_data = null;

#if BUILD_TYPE_DEBUG
        // デバッグ用の一時的なマスターデータがあればそこから取得
        if (m_DebugAllData != null)
        {
            for (int idx = 0; idx < m_DebugAllData.Length; idx++)
            {
                T wrk_master_data = m_DebugAllData[idx];
                if (wrk_master_data != null
                    && wrk_master_data.fix_id == id
                )
                {
                    master_data = wrk_master_data;
                    break;
                }
            }

            m_Dictionary[id] = master_data;
            return master_data;
        }
#endif

        if (typeof(T) == typeof(MasterDataParamEnemy)
            || typeof(T) == typeof(MasterDataEnemyActionTable)
            || typeof(T) == typeof(MasterDataEnemyActionParam)
        )
        {
            // クエストのたびにサーバからもらうもの
            //敵のマスターデータのうち MasterDataParamEnemy・MasterDataEnemyActionTable・MasterDataEnemyActionParam は PacketStructQuestBuildから毎回取得しているようです（おそらくチート対策）
            if (BattleParam.m_QuestBuild != null
            )
            {
                if (typeof(T) == typeof(MasterDataParamEnemy)
                    && BattleParam.m_QuestBuild.list_e_param != null
                )
                {
                    for (int idx = 0; idx < BattleParam.m_QuestBuild.list_e_param.Length; idx++)
                    {
                        MasterDataParamEnemy wrk_master_data = BattleParam.m_QuestBuild.list_e_param[idx];
                        if (wrk_master_data != null
                            && wrk_master_data.fix_id == id
                            )
                        {
                            master_data = wrk_master_data as T;
                            break;
                        }
                    }
                }
                else
                if (typeof(T) == typeof(MasterDataEnemyActionTable)
                    && BattleParam.m_QuestBuild.list_e_acttable != null
                )
                {
                    for (int idx = 0; idx < BattleParam.m_QuestBuild.list_e_acttable.Length; idx++)
                    {
                        MasterDataEnemyActionTable wrk_master_data = BattleParam.m_QuestBuild.list_e_acttable[idx];
                        if (wrk_master_data != null
                            && wrk_master_data.fix_id == id
                            )
                        {
                            master_data = wrk_master_data as T;
                            break;
                        }
                    }
                }
                else
                if (typeof(T) == typeof(MasterDataEnemyActionParam)
                    && BattleParam.m_QuestBuild.list_e_actparam != null
                )
                {
                    for (int idx = 0; idx < BattleParam.m_QuestBuild.list_e_actparam.Length; idx++)
                    {
                        MasterDataEnemyActionParam wrk_master_data = BattleParam.m_QuestBuild.list_e_actparam[idx];
                        if (wrk_master_data != null
                            && wrk_master_data.fix_id == id
                            )
                        {
                            master_data = wrk_master_data as T;
                            break;
                        }
                    }
                }
            }

#if BUILD_TYPE_DEBUG && USE_LOCAL_ENEMY_MASTER
            if (master_data == null)
            {
                master_data = _getMasterData(id);
            }
#endif //USE_LOCAL_ENEMY_MASTER
        }
        else
        {
            master_data = _getMasterData(id);
        }

        m_Dictionary[id] = master_data;

        return master_data;
    }

    /// <summary>
    /// マスターデータにない独自のデータを追加する
    /// </summary>
    /// <param name="master_data"></param>
    public T add(T master_data)
    {
        if (master_data != null && m_Dictionary.ContainsKey(master_data.fix_id) == false)
        {
            m_Dictionary[master_data.fix_id] = master_data;
        }

        return master_data;
    }

    /// <summary>
    /// キャッシュされているデータの個数を取得
    /// </summary>
    /// <returns></returns>
    public int getCachedDataCount()
    {
        if (m_Dictionary != null)
        {
            return m_Dictionary.Count;
        }
        return 0;
    }

    /// <summary>
    /// キャッシュされているデータを取得（インデックス指定）
    /// </summary>
    /// <returns></returns>
    public T getCachedDataByIndex(int index)
    {
        foreach (T master_data in m_Dictionary.Values)
        {
            if (index <= 0)
            {
                return master_data;
            }

            index--;
        }

        return null;
    }

    /// <summary>
    /// データベースからマスターデータを取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private T _getMasterData(uint id)
    {
        T master_data = null;

#if BUILD_TYPE_DEBUG && USE_DEBUG_JSON_MASTER_DATA
        if (BattleParam.m_IsUseDebugJsonMasterData)
        {
            master_data = BattleMasterDataFromJson.Instance.getMasterData<T>(id);
            return master_data;
        }
#endif //USE_DEBUG_JSON_MASTER_DATA

        try
        {
            master_data = MasterFinder<T>.Instance.Find((int)id);
        }
        catch (Exception e)
        {
            Debug.LogError("MasterDataCache.use() get error :" + e.ToString());
        }

        return master_data;
    }

    public T[] _getMasterDataAll()
    {
        T[] master_data_array = null;

#if BUILD_TYPE_DEBUG && USE_DEBUG_JSON_MASTER_DATA
        if (BattleParam.m_IsUseDebugJsonMasterData)
        {
            master_data_array = BattleMasterDataFromJson.Instance.getMasterDataAll<T>();
            return master_data_array;
        }
#endif //USE_DEBUG_JSON_MASTER_DATA

        try
        {
            master_data_array = MasterFinder<T>.Instance.GetAll();
        }
        catch (Exception e)
        {
            Debug.LogError("MasterDataCache.use() get error :" + e.ToString());
        }

        return master_data_array;
    }

#if BUILD_TYPE_DEBUG
    public void setDebugAllData(T[] master_data_array)
    {
        m_DebugAllData = master_data_array;
    }
#endif
}

#if BUILD_TYPE_DEBUG && USE_DEBUG_JSON_MASTER_DATA
public class BattleMasterDataFromJson
{
    //private string JSON_DATA_PATH = "MasterData/00_Json/";
    private static string JSON_DATA_PATH = "MasterData/";

    private MasterDataArea[] m_MasterDataArea = null;
    private MasterDataStatusAilmentParam[] m_MasterDataStatusAilmentParam = null;
    private MasterDataParamChara[] m_MasterDataParamChara = null;
    private MasterDataSkillPassive[] m_MasterDataSkillPassive = null;
    private MasterDataSkillBoost[] m_MasterDataSkillBoost = null;
    private MasterDataSkillLeader[] m_MasterDataSkillLeader = null;
    private MasterDataSkillLimitBreak[] m_MasterDataSkillLimitBreak = null;
    private MasterDataSkillActive[] m_MasterDataSkillActive = null;
    private MasterDataParamEnemy[] m_MasterDataParamEnemy = null;
    private MasterDataEnemyActionTable[] m_MasterDataEnemyActionTable = null;
    private MasterDataEnemyActionParam[] m_MasterDataEnemyActionParam = null;
    private MasterDataEnemyAbility[] m_MasterDataEnemyAbility = null;
    private MasterDataEnemyGroup[] m_MasterDataEnemyGroup = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BattleMasterDataFromJson()
    {
        Debug.LogWarning("検証用マスターデータとしてJsonデータを使用するモードで実行中です.");

        try
        {
            m_MasterDataArea = LitJson.JsonMapper.ToObject<MasterDataArea[]>((Resources.Load(JSON_DATA_PATH + "MasterDataArea") as TextAsset).text);
            m_MasterDataStatusAilmentParam = LitJson.JsonMapper.ToObject<MasterDataStatusAilmentParam[]>((Resources.Load(JSON_DATA_PATH + "MasterDataStatusAilmentParam") as TextAsset).text);
            m_MasterDataParamChara = LitJson.JsonMapper.ToObject<MasterDataParamChara[]>((Resources.Load(JSON_DATA_PATH + "MasterDataParamChara") as TextAsset).text);
            m_MasterDataSkillPassive = LitJson.JsonMapper.ToObject<MasterDataSkillPassive[]>((Resources.Load(JSON_DATA_PATH + "MasterDataSkillPassive") as TextAsset).text);
            m_MasterDataSkillBoost = LitJson.JsonMapper.ToObject<MasterDataSkillBoost[]>((Resources.Load(JSON_DATA_PATH + "MasterDataSkillBoost") as TextAsset).text);
            m_MasterDataSkillLeader = LitJson.JsonMapper.ToObject<MasterDataSkillLeader[]>((Resources.Load(JSON_DATA_PATH + "MasterDataSkillLeader") as TextAsset).text);
            m_MasterDataSkillLimitBreak = LitJson.JsonMapper.ToObject<MasterDataSkillLimitBreak[]>((Resources.Load(JSON_DATA_PATH + "MasterDataSkillLimitBreak") as TextAsset).text);
            m_MasterDataSkillActive = LitJson.JsonMapper.ToObject<MasterDataSkillActive[]>((Resources.Load(JSON_DATA_PATH + "MasterDataSkillActive") as TextAsset).text);
            m_MasterDataParamEnemy = LitJson.JsonMapper.ToObject<MasterDataParamEnemy[]>((Resources.Load(JSON_DATA_PATH + "MasterDataParamEnemy") as TextAsset).text);
            m_MasterDataEnemyActionTable = LitJson.JsonMapper.ToObject<MasterDataEnemyActionTable[]>((Resources.Load(JSON_DATA_PATH + "MasterDataEnemyActionTable") as TextAsset).text);
            m_MasterDataEnemyActionParam = LitJson.JsonMapper.ToObject<MasterDataEnemyActionParam[]>((Resources.Load(JSON_DATA_PATH + "MasterDataEnemyActionParam") as TextAsset).text);
            m_MasterDataEnemyAbility = LitJson.JsonMapper.ToObject<MasterDataEnemyAbility[]>((Resources.Load(JSON_DATA_PATH + "MasterDataEnemyAbility") as TextAsset).text);
            m_MasterDataEnemyGroup = LitJson.JsonMapper.ToObject<MasterDataEnemyGroup[]>((Resources.Load(JSON_DATA_PATH + "MasterDataEnemyGroup") as TextAsset).text);
        }
        catch (Exception e)
        {
            Debug.LogError("検証用マスターデータのJsonが存在しないかJsonにエラーがあります！！！\n" + e.ToString());
        }
    }

    // シングルトンのインスタンス
    private static BattleMasterDataFromJson s_MasterDataFromJson = null;

    public static BattleMasterDataFromJson Instance
    {
        get
        {
            if (s_MasterDataFromJson == null)
            {
                s_MasterDataFromJson = new BattleMasterDataFromJson();
            }

            return s_MasterDataFromJson;
        }
    }

    public T getMasterData<T>(uint id) where T : Master
    {
        T[] master_data_array = getMasterDataAll<T>();
        if (master_data_array != null)
        {
            for (int idx = 0; idx < master_data_array.Length; idx++)
            {
                T master_data = master_data_array[idx];
                if (master_data != null
                    && master_data.fix_id == id
                )
                {
                    return master_data;
                }
            }
        }

        return null;
    }

    public T[] getMasterDataAll<T>() where T : Master
    {
        T[] master_data_array = null;

        if (typeof(T) == typeof(MasterDataArea))
        {
            master_data_array = m_MasterDataArea as T[];
        }
        else if (typeof(T) == typeof(MasterDataStatusAilmentParam))
        {
            master_data_array = m_MasterDataStatusAilmentParam as T[];
        }
        else if (typeof(T) == typeof(MasterDataParamChara))
        {
            master_data_array = m_MasterDataParamChara as T[];
        }
        else if (typeof(T) == typeof(MasterDataSkillPassive))
        {
            master_data_array = m_MasterDataSkillPassive as T[];
        }
        else if (typeof(T) == typeof(MasterDataSkillBoost))
        {
            master_data_array = m_MasterDataSkillBoost as T[];
        }
        else if (typeof(T) == typeof(MasterDataSkillLeader))
        {
            master_data_array = m_MasterDataSkillLeader as T[];
        }
        else if (typeof(T) == typeof(MasterDataSkillLimitBreak))
        {
            master_data_array = m_MasterDataSkillLimitBreak as T[];
        }
        else if (typeof(T) == typeof(MasterDataSkillActive))
        {
            master_data_array = m_MasterDataSkillActive as T[];
        }
        else if (typeof(T) == typeof(MasterDataParamEnemy))
        {
            master_data_array = m_MasterDataParamEnemy as T[];
        }
        else if (typeof(T) == typeof(MasterDataEnemyActionTable))
        {
            master_data_array = m_MasterDataEnemyActionTable as T[];
        }
        else if (typeof(T) == typeof(MasterDataEnemyActionParam))
        {
            master_data_array = m_MasterDataEnemyActionParam as T[];
        }
        else if (typeof(T) == typeof(MasterDataEnemyAbility))
        {
            master_data_array = m_MasterDataEnemyAbility as T[];
        }
        else if (typeof(T) == typeof(MasterDataEnemyGroup))
        {
            master_data_array = m_MasterDataEnemyGroup as T[];
        }

        return master_data_array;
    }
}
#endif	//USE_DEBUG_JSON_MASTER_DATA

