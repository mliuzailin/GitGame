using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMaster2er
{
    private List<List<EMASTERDATA>> targetList;
    public int TargetListCount
    {
        get
        {
            if (targetList == null)
            {
                return 0;
            }
            return targetList.Count;
        }
    }
    private Dictionary<EMASTERDATA, uint> eMasterDataDict;

    public GetMaster2er()
    {
        initalize(new List<List<EMASTERDATA>>
                {
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_ENEMY_GROUP,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_PARAM_CHARA,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_SKILL_ACTIVE,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_ENEMY_ACTION_PARAM,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_ENEMY_ACTION_TABLE,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_SKILL_LEADER,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_STORY,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_SKILL_LIMITBREAK,
                        EMASTERDATA.eMASTERDATA_SKILL_PASSIVE,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_STATUS_AILMENT,
                        EMASTERDATA.eMASTERDATA_ASSET_BUNDLE_PATH,
                        EMASTERDATA.eMASTERDATA_TEXT_DEFINITION,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_PARAM_CHARA_EVOL,
                        EMASTERDATA.eMASTERDATA_PRESENT,
                        EMASTERDATA.eMASTERDATA_RENEW_QUEST,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_RENEW_QUEST_SCORE,
                        EMASTERDATA.eMASTERDATA_PLAY_SCORE,
                        EMASTERDATA.eMASTERDATA_SCORE_EVENT,
                        EMASTERDATA.eMASTERDATA_SCORE_REWARD,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_GACHA,
                        EMASTERDATA.eMASTERDATA_GACHA_GROUP,
                        EMASTERDATA.eMASTERDATA_GACHA_TICKET,
                        EMASTERDATA.eMASTERDATA_STEP_UP_GACHA,
                        EMASTERDATA.eMASTERDATA_STEP_UP_GACHA_MANAGE,
                        EMASTERDATA.eMASTERDATA_GACHA_TEXT,
                        EMASTERDATA.eMASTERDATA_GACHA_TEXT_REF,
                        EMASTERDATA.eMASTERDATA_PRESENT_GROUP,
                    },
                    new List<EMASTERDATA>
                    {
                        EMASTERDATA.eMASTERDATA_CHALLENGE_QUEST,
                        EMASTERDATA.eMASTERDATA_CHALLENGE_EVENT,
                        EMASTERDATA.eMASTERDATA_CHALLENGE_REWARD,
                    },
            });
    }

    public GetMaster2er(List<List<EMASTERDATA>> targetList)
    {
        initalize(targetList);
    }

    public void initalize(List<List<EMASTERDATA>> targetList)
    {
        this.targetList = targetList;
        updateMasterMaxDic();
    }

    public void updateMasterMaxDic()
    {
        this.eMasterDataDict = SQLiteClient.Instance.GetMaxTagIdDict();
    }

    public Dictionary<EMASTERDATA, uint> Next()
    {

        Dictionary<EMASTERDATA, uint> result = new Dictionary<EMASTERDATA, uint>();

        foreach (List<EMASTERDATA> list in targetList)
        {
            if (eMasterDataDict.ContainsKey(list[0]))
            {
                foreach (EMASTERDATA emasterData in list)
                {
                    result.Add(emasterData, eMasterDataDict[emasterData]);
                    eMasterDataDict.Remove(emasterData);
                }
                return result;
            }
        }

        foreach (EMASTERDATA key in eMasterDataDict.Keys)
        {
            result.Add(key, eMasterDataDict[key]);
        }
        eMasterDataDict.Clear();
        return result;
    }

    public bool IsDone
    {
        get
        {
            return eMasterDataDict.IsNullOrEmpty();
        }
    }
}
