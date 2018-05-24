using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using ServerDataDefine;

public class ScoreRewardContext : M4uContext
{
    public static readonly int REWARD_SHOW = 0;         // 未達成
    public static readonly int REWARD_ACHIIEVE = 1;     // 達成済み
    public static readonly int REWARD_GET = 2;          // 報酬受け取り済み
    public static readonly int REWARD_CLEAR = 3;        // クエストリザルトでの表示状態

    M4uProperty<bool> isViewEventName = new M4uProperty<bool>();
    public bool IsViewEventName { get { return isViewEventName.Value; } set { isViewEventName.Value = value; } }

    M4uProperty<string> eventName = new M4uProperty<string>();
    public string EventName { get { return eventName.Value; } set { eventName.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> scoreLabel = new M4uProperty<string>();
    public string ScoreLabel { get { return scoreLabel.Value; } set { scoreLabel.Value = value; } }

    M4uProperty<string> score = new M4uProperty<string>();
    public string Score { get { return score.Value; } set { score.Value = value; } }

    M4uProperty<string> itemName = new M4uProperty<string>();
    public string ItemName { get { return itemName.Value; } set { itemName.Value = value; } }

    M4uProperty<string> itemNum = new M4uProperty<string>();
    public string ItemNum { get { return itemNum.Value; } set { itemNum.Value = value; } }

    M4uProperty<int> rewardType = new M4uProperty<int>();
    public int RewardType { get { return rewardType.Value; } set { rewardType.Value = value; } }

    public System.Action<ScoreRewardContext> DidSelectItem = delegate { };

    public PacketStructUserScoreReward Reward = null;
    private string m_SpriteName = string.Empty;

    public void setData(PacketStructUserScoreReward reward, int type)
    {
        Reward = reward;
        RewardType = type;
    }

    public void setup(PacketStructUserScoreReward reward, int type)
    {
        Reward = reward;

        switch (reward.type)
        {
            case 1:
                ScoreLabel = GameTextUtil.GetText("scorereward_list_01");
                break;
            case 2:
                ScoreLabel = GameTextUtil.GetText("scorereward_list_02");
                break;
        }

        Score = string.Format(GameTextUtil.GetText("scorereward_list_03"), reward.score);
        RewardType = type;

        MasterDataPresent present = MasterDataUtil.GetPresentParamFromID((uint)reward.present_ids[0]);

        m_SpriteName = string.Empty;
        MainMenuUtil.GetPresentIcon(present, ref m_SpriteName, (sprite) =>
        {
            if (MainMenuUtil.IsWriteIcon(ref m_SpriteName, sprite))
            {
                IconImage = sprite;
            }
        });

        ItemName = MasterDataUtil.GetPresentName(present);
        int num = MasterDataUtil.GetPresentCount(present);
        int count = 0;
        for (int i = 0; i < reward.present_ids.Length; i++)
        {
            if (reward.present_ids[0] == reward.present_ids[i]) count++;
        }

        ItemNum = string.Format("{0}", num * count);

        IsViewEventName = false;
        EventName = "";
    }

}
