using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class SceneChallengeSelectTest : Scene<SceneChallengeSelectTest>
{
    public ChallengeSelect challengeSelect = null;
    private List<MasterDataChallengeEvent> m_Events = new List<MasterDataChallengeEvent>();

    protected override void Start()
    {
        base.Start();

        //テストデータ
        uint[] boss_ids = new uint[4] { 2440, 2445, 2450, 2455 };
        string[] asset_names = new string[4] { "Areamap_1101000", "Areamap_1101001", "Areamap_1101002", "Areamap_1101000" };
        for (int i = 0; i < 4; i++)
        {
            MasterDataChallengeEvent newData = new MasterDataChallengeEvent();
            newData.bg_assetbundle_name = asset_names[i];
            MasterDataChallengeQuest newQuest = new MasterDataChallengeQuest();
            newQuest.boss_chara_id = boss_ids[i];
            PacketStructChallengeInfo newInfo = new PacketStructChallengeInfo();
            challengeSelect.AddEventData(newData, newInfo, newQuest);
        }

        StartCoroutine(LoadIcon(
            () =>
            {
                UnitIconImageProvider.Instance.MakeAtlus(()=>
                {
                    if (challengeSelect != null)
                    {
                        challengeSelect.setup();
                    }
                });
            }));
    }

    private IEnumerator LoadIcon( System.Action finish )
    {
        AssetBundlerMultiplier multi = AssetBundlerMultiplier.Create();
        yield return UnitIconImageProvider.Instance.LoadIconPacks(multi);

        multi.Load(finish);
    }
}
