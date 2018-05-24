using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using ServerDataDefine;
using SimpleSQL;
using SimpleSQLManager = SqlCipher4Unity3D.SimpleSQLManager;

public enum TermType
{
    NONE,
    ALL,
    DAY,
    WEEK,
    MONTH
}


public enum OrderBy
{
    ASC,
    DESC
}

public class SQLiteClient : SingletonComponent<SQLiteClient>
{
    [HideInInspector]
    public SimpleSQLManager dbManager;

    SimpleDataRow rowClientVersion = null;

    protected override void Awake()
    {
        base.Awake();
        dbManager = GetComponent<SimpleSQLManager>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void UpdateDbClientVersion()
    {
        rowClientVersion = null;
        try
        {
            List<SimpleDataRow> rows = dbManager.QueryGeneric("select create_type, create_time from client_cleate_type_master").rows;

            if (rows.Count == 0)
            {
                return;
            }

            rowClientVersion = rows[0];
        }
        catch (Exception e)
        {
            Debug.Log("Sqlite3: Exception: " + e.ToString());
            return;
        }
    }

    public GlobalDefine.SERVER_TYPE GetDbServerType()
    {
        string key = "create_type";

        if (rowClientVersion == null ||
            rowClientVersion[key] == null)
        {
            return GlobalDefine.SERVER_TYPE.none;
        }

        var servertype = (GlobalDefine.SERVER_TYPE)rowClientVersion[key].ToString().ToInt(0);
        return servertype;
    }

    public double GetDbCreateTime()
    {
        string key = "create_time";

        if (rowClientVersion == null ||
            rowClientVersion[key] == null)
        {
            return 0;
        }

        double unixTime = rowClientVersion[key].ToString().ToDouble(0);
        return unixTime;
    }

    public static readonly int EmptyDbUnixTime = 0;

    /// <summary>
    /// タイトルでMasterダウンロードを行わないようにする
    /// </summary>
    public void zerotoOneClientVersion()
    {
        UpdateDbClientVersion();
        double unixTime = GetDbCreateTime();
        if (unixTime == EmptyDbUnixTime)
        {
            dbManager.Execute("update client_cleate_type_master  set create_time = 1");
        }
        else
        {
            Debug.LogError("NG zerotoOneClientVersion: " + unixTime);
        }
    }

    public void RemoveDbFiles()
    {
        dbManager.RemoveDbFiles();
    }

    public string GetFilename()
    {
        return dbManager.getFilename();
    }

    public string GetDbPath()
    {
        return dbManager.getPath();
    }

    public void OpenDB()
    {
        dbManager.Initialize(false);
    }

    public void CloseDb()
    {
        dbManager.Dispose();
    }

    public bool BrokenDb()
    {
        return dbManager.BrokenDb();
    }

    public IEnumerator LocalSqlite3ClearExec(Action action)
    {
        WaitForSeconds waitseconds = new WaitForSeconds(0.2f);

        try
        {
            // ファイルのクローズ
            CloseDb();
        }
        catch (Exception e)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("Sqlite3: Exception: " + e.ToString());
#endif
        }

        yield return waitseconds;

        // ファイルの削除
        RemoveDbFiles();

        yield return waitseconds;

        // ファイルのコピー
        OpenDB();

        yield return waitseconds;

        action();

        yield return null;
    }

    public Dictionary<EMASTERDATA, uint> GetMaxTagIdDict()
    {
        Dictionary<EMASTERDATA, uint> dict = new Dictionary<EMASTERDATA, uint>();

        // テーブルを空の状態にして再取得する必要なマスターを設定する
        List<EMASTERDATA> zeroTagList = new List<EMASTERDATA>
        {
            EMASTERDATA.eMASTERDATA_INFORMATION,
            EMASTERDATA.eMASTERDATA_GACHA,
            EMASTERDATA.eMASTERDATA_NOTIFICATION,
            EMASTERDATA.eMASTERDATA_EVENT,
            EMASTERDATA.eMASTERDATA_QUEST_APPEARANCE,
            EMASTERDATA.eMASTERDATA_STEP_UP_GACHA_MANAGE,
            EMASTERDATA.eMASTERDATA_PRESENT_GROUP,
        };

        foreach (var d in MasterDataDefine.SQLiteHoldList())
        {
            if (zeroTagList.Contains(d))
            {
                dbManager.Execute("delete from " + MasterDataDefine.GetTableName(d));
                dict.Add(d, (uint)0);
                continue;
            }

            try
            {
                SimpleSQL.SimpleDataTable t = dbManager.QueryGeneric("select max(tag_id) as max_tag_id from  " + d.GetTableName());

                if (t == null)
                {
                    dict.Add(d, (uint)0);
                    continue;
                }

                List<SimpleSQL.SimpleDataRow> rows = t.rows;
                if (rows.Count > 0)
                {
                    object obj = rows[0]["max_tag_id"];
                    if (obj != null)
                    {
                        int maxTagId = obj.ToString().ToInt(0);

                        dict.Add(d, (uint)maxTagId);
                    }
                    else
                    {
                        dict.Add(d, (uint)0);
                    }
                }
                else
                {
                    dict.Add(d, (uint)0);
                }

            }
            catch (Exception e)
            {
                Debug.LogError("E:" + e.StackTrace + " ::" + d.ToString());
                dict.Add(d, (uint)0);
            }
        }

        return dict;
    }

    public void Delete<T>(int[] idList) where T : Master, new()
    {
        if (idList.IsNullOrEmpty())
        {
            return;
        }

        string TableName = typeof(T).GetTableName();
        string[] ids = idList.Select(i => i.ToString()).ToArray();

        dbManager.Query<T>(string.Format("delete from {0} where fix_id in ({1})", TableName, string.Join(",", ids)));
    }

    public void InsertOrUpdate<T>(T[] upd_list) where T : Master, new()
    {
        if (upd_list.IsNullOrEmpty())
        {
            return;
        }

        dbManager.InsertAll(upd_list, "OR REPLACE");
    }

    public T SelectFirst<T>(string TableName, uint fix_id) where T : Master, new()
    {
        bool recordExists = false;
        T result = null;

        try
        {
            result = dbManager.QueryFirstRecord<T>(out recordExists, string.Format("select * from {0} where fix_id = {1}", TableName, fix_id));
            if (!recordExists)
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Sqlite3: Exception: " + e.ToString());
            return null;
        }

        return result;
    }

    public List<T> SelectAll<T>(string TableName) where T : Master, new()
    {
        List<T> list = null;

        try
        {
            list = dbManager.Query<T>(string.Format("select * from {0}", TableName));
        }
        catch (Exception e)
        {
            Debug.Log("Sqlite3: Exception: " + e.ToString());
            return null;
        }

        return list;
    }

    public List<int> Reflect<T>(ServerDataDefine.BaseMasterDiff<T> d) where T : Master, new()
    {
        List<int> result = new List<int>();

        Delete<T>(d.del_list);
        InsertOrUpdate<T>(d.upd_list);

        result.Add(d.upd_list.Length);
        result.Add(d.del_list.Length);

        return result;
    }

    public List<int> Reflect(object obj)
    {
        if (obj == null)
        {
            return new List<int>();
        }

        Type type = obj.GetType();
        if (type == typeof(MasterDataDefaultPartyDiff))
        {
            List<int> ret = Reflect(obj as MasterDataDefaultPartyDiff);
            MasterFinder<MasterDataDefaultParty>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataAssetBundlePathDiff))
        {
            List<int> ret = Reflect(obj as MasterDataAssetBundlePathDiff);
            MasterFinder<MasterDataAssetBundlePath>.Instance.RemoveCache();
            MasterDataUtil.ResetAssetBundlePaths();

            return ret;
        }
        if (type == typeof(MasterDataInformationDiff))
        {
            List<int> ret = Reflect(obj as MasterDataInformationDiff);
            MasterFinder<MasterDataInformation>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGachaDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGachaDiff);
            MasterFinder<MasterDataGacha>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGachaGroupDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGachaGroupDiff);
            MasterFinder<MasterDataGachaGroup>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataPresentDiff))
        {
            List<int> ret = Reflect(obj as MasterDataPresentDiff);
            MasterFinder<MasterDataPresent>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataAchievementConvertedDiff))
        {
            List<int> ret = Reflect(obj as MasterDataAchievementConvertedDiff);
            MasterFinder<MasterDataAchievementConverted>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataEventDiff))
        {
            List<int> ret = Reflect(obj as MasterDataEventDiff);
            MasterFinder<MasterDataEvent>.Instance.RemoveCache();
            TimeEventManager.Instance.updateEvents();
            return ret;
        }
        if (type == typeof(MasterDataNotificationDiff))
        {
            List<int> ret = Reflect(obj as MasterDataNotificationDiff);
            MasterFinder<MasterDataNotification>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataUserRankDiff))
        {
            List<int> ret = Reflect(obj as MasterDataUserRankDiff);
            MasterFinder<MasterDataUserRank>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataStoreProductDiff))
        {
            List<int> ret = Reflect(obj as MasterDataStoreProductDiff);
            MasterFinder<MasterDataStoreProduct>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataAreaAmendDiff))
        {
            List<int> ret = Reflect(obj as MasterDataAreaAmendDiff);
            MasterFinder<MasterDataAreaAmend>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataBeginnerBoostDiff))
        {
            List<int> ret = Reflect(obj as MasterDataBeginnerBoostDiff);
            MasterFinder<MasterDataBeginnerBoost>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGuerrillaBossDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGuerrillaBossDiff);
            MasterFinder<MasterDataGuerrillaBoss>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataStatusAilmentParamDiff))
        {
            List<int> ret = Reflect(obj as MasterDataStatusAilmentParamDiff);
            MasterFinder<MasterDataStatusAilmentParam>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataEnemyGroupDiff))
        {
            List<int> ret = Reflect(obj as MasterDataEnemyGroupDiff);
            MasterFinder<MasterDataEnemyGroup>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataAreaCategoryDiff))
        {
            List<int> ret = Reflect(obj as MasterDataAreaCategoryDiff);
            MasterFinder<MasterDataAreaCategory>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataLinkSystemDiff))
        {
            List<int> ret = Reflect(obj as MasterDataLinkSystemDiff);
            MasterFinder<MasterDataLinkSystem>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataLimitOverDiff))
        {
            List<int> ret = Reflect(obj as MasterDataLimitOverDiff);
            MasterFinder<MasterDataLimitOver>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataUseItemDiff))
        {
            List<int> ret = Reflect(obj as MasterDataUseItemDiff);
            MasterFinder<MasterDataUseItem>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGlobalParamsDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGlobalParamsDiff);
            MasterFinder<MasterDataGlobalParams>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataAreaDiff))
        {
            List<int> ret = Reflect(obj as MasterDataAreaDiff);
            MasterFinder<MasterDataArea>.Instance.RemoveCache();
            return ret;
        }
#if false// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
        if (type == typeof(MasterDataQuestDiff))
        {
            //TODO
            List<int> ret = Reflect(obj as MasterDataQuestDiff);
            MasterFinder<MasterDataQuest>.Instance.RemoveCache();
            return ret;
        }
#endif
        if (type == typeof(MasterDataParamCharaDiff))
        {
            List<int> ret = Reflect(obj as MasterDataParamCharaDiff);
            MasterFinder<MasterDataParamChara>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataParamCharaEvolDiff))
        {
            List<int> ret = Reflect(obj as MasterDataParamCharaEvolDiff);
            MasterFinder<MasterDataParamCharaEvol>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataSkillActiveDiff))
        {
            List<int> ret = Reflect(obj as MasterDataSkillActiveDiff);
            MasterFinder<MasterDataSkillActive>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataSkillPassiveDiff))
        {
            List<int> ret = Reflect(obj as MasterDataSkillPassiveDiff);
            MasterFinder<MasterDataSkillPassive>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataSkillLimitBreakDiff))
        {
            List<int> ret = Reflect(obj as MasterDataSkillLimitBreakDiff);
            MasterFinder<MasterDataSkillLimitBreak>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataSkillLeaderDiff))
        {
            List<int> ret = Reflect(obj as MasterDataSkillLeaderDiff);
            MasterFinder<MasterDataSkillLeader>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataSkillBoostDiff))
        {
            List<int> ret = Reflect(obj as MasterDataSkillBoostDiff);
            MasterFinder<MasterDataSkillBoost>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataParamEnemyDiff))
        {
            List<int> ret = Reflect(obj as MasterDataParamEnemyDiff);
            MasterFinder<MasterDataParamEnemy>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataEnemyAbilityDiff))
        {
            List<int> ret = Reflect(obj as MasterDataEnemyAbilityDiff);
            MasterFinder<MasterDataEnemyAbility>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataEnemyActionTableDiff))
        {
            List<int> ret = Reflect(obj as MasterDataEnemyActionTableDiff);
            MasterFinder<MasterDataEnemyActionTable>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataEnemyActionParamDiff))
        {
            List<int> ret = Reflect(obj as MasterDataEnemyActionParamDiff);
            MasterFinder<MasterDataEnemyActionParam>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataQuestRequirementDiff))
        {
            List<int> ret = Reflect(obj as MasterDataQuestRequirementDiff);
            MasterFinder<MasterDataQuestRequirement>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataTextDefinitionDiff))
        {
            List<int> ret = Reflect(obj as MasterDataTextDefinitionDiff);
            MasterFinder<MasterDataTextDefinition>.Instance.RemoveCache();
            UnitGridContext.ResetTextValues();
            MasterDataUtil.ResetTextDefinition();

            return ret;
        }
        if (type == typeof(MasterDataTopPageDiff))
        {
            List<int> ret = Reflect(obj as MasterDataTopPageDiff);
            MasterFinder<MasterDataTopPage>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataAudioDataDiff))
        {
            List<int> ret = Reflect(obj as MasterDataAudioDataDiff);
            MasterFinder<MasterDataAudioData>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGachaTicketDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGachaTicketDiff);
            MasterFinder<MasterDataGachaTicket>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataQuestKeyDiff))
        {
            List<int> ret = Reflect(obj as MasterDataQuestKeyDiff);
            MasterFinder<MasterDataQuestKey>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataStoreProductEventDiff))
        {
            List<int> ret = Reflect(obj as MasterDataStoreProductEventDiff);
            MasterFinder<MasterDataStoreProductEvent>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataHeroDiff))
        {
            List<int> ret = Reflect(obj as MasterDataHeroDiff);
            MasterFinder<MasterDataHero>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataHeroAddEffectRateDiff))
        {
            List<int> ret = Reflect(obj as MasterDataHeroAddEffectRateDiff);
            MasterFinder<MasterDataHeroAddEffectRate>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataHeroLevelDiff))
        {
            List<int> ret = Reflect(obj as MasterDataHeroLevelDiff);
            MasterFinder<MasterDataHeroLevel>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataLoginMonthlyDiff))
        {
            List<int> ret = Reflect(obj as MasterDataLoginMonthlyDiff);
            MasterFinder<MasterDataLoginMonthly>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGachaAssignDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGachaAssignDiff);
            MasterFinder<MasterDataGachaAssign>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataLoginEventDiff))
        {
            List<int> ret = Reflect(obj as MasterDataLoginEventDiff);
            MasterFinder<MasterDataLoginEvent>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataCategoryPatternDiff))
        {
            List<int> ret = Reflect(obj as MasterDataCategoryPatternDiff);
            MasterFinder<MasterDataCategoryPattern>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataExpectPatternDiff))
        {
            List<int> ret = Reflect(obj as MasterDataExpectPatternDiff);
            MasterFinder<MasterDataExpectPattern>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataLoginChainDiff))
        {
            List<int> ret = Reflect(obj as MasterDataLoginChainDiff);
            MasterFinder<MasterDataLoginChain>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataLoginTotalDiff))
        {
            List<int> ret = Reflect(obj as MasterDataLoginTotalDiff);
            MasterFinder<MasterDataLoginTotal>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataQuest2Diff))
        {
            List<int> ret = Reflect(obj as MasterDataQuest2Diff);
            MasterFinder<MasterDataQuest2>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataStoryDiff))
        {
            List<int> ret = Reflect(obj as MasterDataStoryDiff);
            MasterFinder<MasterDataStory>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataNpcDiff))
        {
            List<int> ret = Reflect(obj as MasterDataNpcDiff);
            MasterFinder<MasterDataNpc>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataStoryCharaDiff))
        {
            List<int> ret = Reflect(obj as MasterDataStoryCharaDiff);
            MasterFinder<MasterDataStoryChara>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataIllustratorDiff))
        {
            List<int> ret = Reflect(obj as MasterDataIllustratorDiff);
            MasterFinder<MasterDataIllustrator>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataWebViewDiff))
        {
            List<int> ret = Reflect(obj as MasterDataWebViewDiff);
            MasterFinder<MasterDataWebView>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataRegionDiff))
        {
            List<int> ret = Reflect(obj as MasterDataRegionDiff);
            MasterFinder<MasterDataRegion>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataRenewQuestScoreDiff))
        {
            List<int> ret = Reflect(obj as MasterDataRenewQuestScoreDiff);
            MasterFinder<MasterDataRenewQuestScore>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataPlayScoreDiff))
        {
            List<int> ret = Reflect(obj as MasterDataPlayScoreDiff);
            MasterFinder<MasterDataPlayScore>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataScoreEventDiff))
        {
            List<int> ret = Reflect(obj as MasterDataScoreEventDiff);
            MasterFinder<MasterDataScoreEvent>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataScoreRewardDiff))
        {
            List<int> ret = Reflect(obj as MasterDataScoreRewardDiff);
            MasterFinder<MasterDataScoreReward>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataQuestAppearanceDiff))
        {
            List<int> ret = Reflect(obj as MasterDataQuestAppearanceDiff);
            MasterFinder<MasterDataQuestAppearance>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataStepUpGachaDiff))
        {
            List<int> ret = Reflect(obj as MasterDataStepUpGachaDiff);
            MasterFinder<MasterDataStepUpGacha>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataStepUpGachaManageDiff))
        {
            List<int> ret = Reflect(obj as MasterDataStepUpGachaManageDiff);
            MasterFinder<MasterDataStepUpGachaManage>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataChallengeQuestDiff))
        {
            List<int> ret = Reflect(obj as MasterDataChallengeQuestDiff);
            MasterFinder<MasterDataChallengeQuest>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataChallengeEventDiff))
        {
            List<int> ret = Reflect(obj as MasterDataChallengeEventDiff);
            MasterFinder<MasterDataChallengeEvent>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataChallengeRewardDiff))
        {
            List<int> ret = Reflect(obj as MasterDataChallengeRewardDiff);
            MasterFinder<MasterDataChallengeReward>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGachaTextDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGachaTextDiff);
            MasterFinder<MasterDataGachaText>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGachaTextRefDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGachaTextRefDiff);
            MasterFinder<MasterDataGachaTextRef>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataPresentGroupDiff))
        {
            List<int> ret = Reflect(obj as MasterDataPresentGroupDiff);
            MasterFinder<MasterDataPresentGroup>.Instance.RemoveCache();
            return ret;
        }
        if (type == typeof(MasterDataGeneralWindowDiff))
        {
            List<int> ret = Reflect(obj as MasterDataGeneralWindowDiff);
            MasterFinder<MasterDataGeneralWindow>.Instance.RemoveCache();
            return ret;
        }
        return new List<int>();
    }

    public List<T> SelectWhere<T>(string TableName, string WhereText, params object[] args)
    where T : Master, new()
    {
        List<T> list = null;
        try
        {
            list = dbManager.Query<T>(string.Format("select * from {0} {1}", TableName, WhereText), args);
        }
        catch (Exception e)
        {
            Debug.Log("Sqlite3: Exception: " + e.ToString());
            return null;
        }

        return list;
    }

    public T SelectFirstWhere<T>(string TableName, string WhereText, params object[] args)
    where T : Master, new()
    {
        bool recordExists = false;
        T result = null;

        try
        {
            result = dbManager.QueryFirstRecord<T>(out recordExists, string.Format("select * from {0} {1}", TableName, WhereText), args);
            if (!recordExists)
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Sqlite3: Exception: " + e.ToString());
            return null;
        }

        return result;
    }

    public SimpleDataTable QueryGeneric(string QueryText, params object[] args)
    {
        SimpleDataTable result = null;
        try
        {
            result = dbManager.QueryGeneric(QueryText, args);
        }
        catch (Exception e)
        {
            Debug.Log("Sqlite3: Exception: " + e.ToString());
            return null;
        }

        return result;
    }
}
