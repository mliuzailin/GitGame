using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using ServerDataDefine;

public class QuestMissionContext : M4uContext
{
    public uint fix_id = 0;

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<string> itemName = new M4uProperty<string>();
    public string ItemName { get { return itemName.Value; } set { itemName.Value = value; } }

    M4uProperty<bool> isViewCount = new M4uProperty<bool>();
    public bool IsViewCount { get { return isViewCount.Value; } set { isViewCount.Value = value; } }

    M4uProperty<int> count = new M4uProperty<int>();
    public int Count { get { return count.Value; } set { count.Value = value; calcCountRatio(); } }

    M4uProperty<int> countMax = new M4uProperty<int>();
    public int CountMax { get { return countMax.Value; } set { countMax.Value = value; calcCountRatio(); } }

    M4uProperty<float> countRatio = new M4uProperty<float>();
    public float CountRatio { get { return countRatio.Value; } set { countRatio.Value = value; } }

    M4uProperty<bool> isActiveLeftTime = new M4uProperty<bool>();
    public bool IsActiveLeftTime { get { return isActiveLeftTime.Value; } set { isActiveLeftTime.Value = value; } }

    M4uProperty<string> leftValue = new M4uProperty<string>();
    public string LeftValue { get { return leftValue.Value; } set { leftValue.Value = value; } }

    M4uProperty<string> itemValue = new M4uProperty<string>();
    /// <summary>アイテムのプレゼント個数</summary>
    public string ItemValue { get { return itemValue.Value; } set { itemValue.Value = value; } }

    private void calcCountRatio()
    {
        if (CountMax == 0)
        {
            return;
        }

        CountRatio = (float)Count / (float)CountMax;
        if (Count == CountMax)
        {
            IsViewCount = false;
        }
        else
        {
            IsViewCount = true;
        }
    }

    public QuestMissionContext()
    {

    }

    public QuestMissionContext(PacketAchievement clearAchievement)
    {
        MasterDataPresent presentData = (!clearAchievement.present_ids.IsNullOrEmpty()) ? MasterDataUtil.GetPresentParamFromID(clearAchievement.present_ids[0]) : null;
        if (presentData != null)
        {
            //TODO MasterDataAchievementConverted →　PresentMasterCount とまとめたい
            fix_id = clearAchievement.fix_id;
            int mastercount = 1;
            uint presentid = clearAchievement.present_ids[0];
            for (int j = 1; j < clearAchievement.present_ids.Length; j++)
            {
                if (presentid == clearAchievement.present_ids[j])
                {
                    mastercount += 1;
                }
            }

            int itemValue = MasterDataUtil.GetPresentCount(presentData) * mastercount;

            Title = clearAchievement.draw_msg;
            ItemName = MasterDataUtil.GetPresentName(presentData);
            MainMenuUtil.GetPresentIcon(
                presentData,
                sprite =>
                {
                    IconImage = sprite;
                });
            IsViewCount = false;
            ItemValue = (itemValue > 0) ? itemValue.ToString() : "";
        }
    }
}
