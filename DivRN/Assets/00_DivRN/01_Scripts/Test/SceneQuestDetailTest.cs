using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneQuestDetailTest : Scene<SceneQuestDetailTest>
{
    public QuestDetailBG questDetailBG = null;
    public QuestDetailInfo questDetailInfo = null;
    public QuestDetailTab questDetailTab = null;
    public QuestDetailMessage questDetailMessage = null;
    public QuestDetailMission questDetailMission = null;
    public uint charaId = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        questDetailBG.QuestId = 1;
        questDetailBG.QuestIdLabel = "Quest.";
        questDetailBG.QuestTitle = "サンプルクエスト名";
        questDetailBG.AreaCategoryTitle = "エリアカテゴリ名";
        questDetailBG.AreaTitle = "エリア名";
        questDetailBG.ButtonTitle = "出撃";

        questDetailBG.setupChara(charaId);

        questDetailInfo.CountLabel = "BATTLE";
        questDetailInfo.CountValue = "5";
        questDetailInfo.ExpLabel = "EXP";
        questDetailInfo.ExpValue = "2000";
        questDetailInfo.CoinLabel = "COIN";
        questDetailInfo.CoinValue = "1500";
        questDetailInfo.BossLabel = "BOSS";
        questDetailInfo.BossName = "ぼすのなまえ";

        //Sprite _tmpSprite = Resources.Load<Sprite>("UIData/element/s_hi");

        questDetailTab.DidTabChenged = ChengeTab;

        for (int i = 0; i < 3; i++)
        {
            questDetailTab.AddTab("タブ" + (i + 1).ToString(), (QuestDetailModel.TabType)i);
        }

        questDetailMessage.Title = "タイトルだよ";
        questDetailMessage.Message = "さんぷるめっせーじだよ";

        UnityUtil.SetObjectEnabledOnce(questDetailMessage.gameObject, false);

        questDetailMission.Title = "MISSION";
        questDetailMission.Count = 0;
        questDetailMission.CountMax = 5;

        for(int i = 0; i < 5; i++)
        {
            QuestMissionContext newMission = new QuestMissionContext();
            newMission.Title = "ミッションタイトル" + (i + 1).ToString();
            newMission.Count = i;
            newMission.CountMax = 4;
            newMission.IconImage = null;
            newMission.IsActiveLeftTime = true;
            newMission.LeftValue = "あと"+(i+1).ToString() + "日";
            questDetailMission.MissionList.Add(newMission);
        }
        UnityUtil.SetObjectEnabledOnce(questDetailMission.gameObject, false);
    }

    private void ChengeTab( QuestDetailTabContext _tab )
    {
        switch(questDetailTab.currentIndex)
        {
            case 0:
                UnityUtil.SetObjectEnabledOnce(questDetailInfo.gameObject, true);
                UnityUtil.SetObjectEnabledOnce(questDetailMessage.gameObject, false);
                UnityUtil.SetObjectEnabledOnce(questDetailMission.gameObject, false);
                break;
            case 1:
                UnityUtil.SetObjectEnabledOnce(questDetailInfo.gameObject, false);
                UnityUtil.SetObjectEnabledOnce(questDetailMessage.gameObject, true);
                UnityUtil.SetObjectEnabledOnce(questDetailMission.gameObject, false);
                break;
            case 2:
                UnityUtil.SetObjectEnabledOnce(questDetailInfo.gameObject, false);
                UnityUtil.SetObjectEnabledOnce(questDetailMessage.gameObject, false);
                UnityUtil.SetObjectEnabledOnce(questDetailMission.gameObject, true);
                break;
        }
    }

}
