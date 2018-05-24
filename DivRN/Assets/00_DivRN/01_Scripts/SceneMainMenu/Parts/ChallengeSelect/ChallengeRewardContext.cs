using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using ServerDataDefine;

public class ChallengeRewardContext : M4uContext
{
    public static readonly int REWARD_SHOW = 0;         // 未達成
    public static readonly int REWARD_ACHIIEVE = 1;     // 達成済み
    public static readonly int REWARD_GET = 2;          // 報酬受け取り済み
    public static readonly int REWARD_CLEAR = 3;        // クリア状態
    public static readonly int REWARD_NONE = 4;         // 未達成(表示なし)

    public static readonly int REWARD_STATUS_APPEAR = 0;    // 出現
    public static readonly int REWARD_STATUS_CLEAR = 1;     // クリア
    public static readonly int REWARD_STATUS_GET = 2;       // 取得済み


    enum ChallengeRewardType : int
    {
        TYPE_NONE = 0,
        TYPE_SP_CHALLENGE = 1,
        TYPE_SP_CLEAR = 2,
        TYPE_SP_LEVEL = 3,
        TYPE_NM_LEVEL = 4
    };


    M4uProperty<string> message = new M4uProperty<string>();
    public string Message { get { return message.Value; } set { message.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> itemName = new M4uProperty<string>();
    public string ItemName { get { return itemName.Value; } set { itemName.Value = value; } }

    M4uProperty<string> itemNum = new M4uProperty<string>();
    public string ItemNum { get { return itemNum.Value; } set { itemNum.Value = value; } }

    M4uProperty<int> rewardType = new M4uProperty<int>();
    public int RewardType { get { return rewardType.Value; } set { rewardType.Value = value; } }

    M4uProperty<float> fillAmount = new M4uProperty<float>();
    public float FillAmount { get { return fillAmount.Value; } set { fillAmount.Value = value; } }

    M4uProperty<int> rewardCount = new M4uProperty<int>();
    public int RewardCount { get { return rewardCount.Value; } set { rewardCount.Value = value; } }

    M4uProperty<int> rewardCountMax = new M4uProperty<int>();
    public int RewardCountMax { get { return rewardCountMax.Value; } set { rewardCountMax.Value = value; } }

    M4uProperty<bool> isActiveTitleBar = new M4uProperty<bool>();
    public bool IsActiveTitleBar { get { return isActiveTitleBar.Value; } set { isActiveTitleBar.Value = value; } }

    M4uProperty<string> titleBarText = new M4uProperty<string>();
    public string TitleBarText { get { return titleBarText.Value; } set { titleBarText.Value = value; } }

    private PacketStructChallengeGetReward m_GetReward = null;
    public PacketStructChallengeGetReward GetReward { get { return m_GetReward; } }

    private MasterDataChallengeReward m_GetMaster = null;
    public MasterDataChallengeReward GetMaster { get { return m_GetMaster; } }

    private PacketStructChallengeInfoReward m_InfoReward = null;
    public PacketStructChallengeInfoReward InfoReward { get { return m_InfoReward; } }

    public System.Action<ChallengeRewardContext> DidSelectItem = delegate { };

    private string m_SpriteName = string.Empty;

    public void SetData(PacketStructChallengeGetReward reward, MasterDataChallengeReward master)
    {
        m_GetReward = reward;
        m_GetMaster = master;
        m_InfoReward = null;
    }
    public void SetData(PacketStructChallengeInfoReward reward)
    {
        m_GetReward = null;
        m_GetMaster = null;
        m_InfoReward = reward;
    }

    public void SetTitleBar(string titleValue)
    {
        IsActiveTitleBar = true;
        TitleBarText = titleValue;
    }

    public void CopyData(ChallengeRewardContext context)
    {
        m_GetReward = context.GetReward;
        m_GetMaster = context.GetMaster;
        m_InfoReward = context.InfoReward;

        IsActiveTitleBar = context.IsActiveTitleBar;
        TitleBarText = context.TitleBarText;
    }

    public void setupGet(int type)
    {
        PacketStructChallengeGetReward reward = m_GetReward;
        MasterDataChallengeReward master = m_GetMaster;

        Message = getRewardMessage(master.type, master.clear_param, master.clear_param, (master.clear_loop_reward_enable == 1 ? master.clear_loop_reward_count : 0));
        RewardType = type;

        if (reward.present_ids == null ||
            reward.present_ids.Length == 0)
        {
            return;
        }

        setupPresent(reward.present_ids);
    }

    public void setupInfo(PacketStructChallengeInfo info)
    {
        PacketStructChallengeInfoReward reward = m_InfoReward;
        Message = "";
        switch (reward.type)
        {
            case (int)ChallengeRewardType.TYPE_SP_CHALLENGE:
                {
                    Message = getRewardMessage(reward.type, reward.clear_param);
                    setCount(info.reward_challenge_cnt, reward.clear_param, reward.status);
                }
                break;
            case (int)ChallengeRewardType.TYPE_SP_CLEAR:
                {
                    Message = getRewardMessage(reward.type, reward.clear_param);
                    setCount(info.reward_clear_cnt, reward.clear_param, reward.status);
                }
                break;
            case (int)ChallengeRewardType.TYPE_SP_LEVEL:
                {
                    Message = getRewardMessage(reward.type, reward.clear_param);
                    if (reward.status == REWARD_STATUS_APPEAR)
                    {
                        RewardType = REWARD_NONE;
                    }
                    else
                    {
                        RewardType = REWARD_CLEAR;
                    }
                }
                break;
            case (int)ChallengeRewardType.TYPE_NM_LEVEL:
                {
                    if (reward.loop_cnt == 0)
                    {
                        Message = getRewardMessage(reward.type, reward.start, reward.end);
                    }
                    else
                    {
                        Message = getRewardMessage(reward.type, reward.start, reward.end, reward.loop_cnt);
                    }
                }
                RewardType = REWARD_NONE;
                break;
        }

        if (reward.present_ids == null ||
            reward.present_ids.Length == 0)
        {
            return;
        }

        setupPresent(reward.present_ids);

    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="count"></param>
    /// <param name="max"></param>
    private void setCount(int count, int max, int status)
    {
        if (status == REWARD_STATUS_APPEAR)
        {
            RewardCount = count;
            RewardCountMax = max;
            RewardType = REWARD_SHOW;
            if (max > 0)
            {
                FillAmount = (float)count / max;
                if (FillAmount >= 1.0f)
                {
                    FillAmount = 1.0f;
                    RewardType = REWARD_CLEAR;
                }
            }
        }
        else
        {
            RewardType = REWARD_CLEAR;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="loop_cnt"></param>
    /// <returns></returns>
    private string getRewardMessage(int type, int param1, int param2 = 0, int loop_cnt = 0)
    {
        string message = "";
        switch (type)
        {
            case (int)ChallengeRewardType.TYPE_SP_CHALLENGE:
                {
                    message = string.Format(GameTextUtil.GetText("growth_boss_22"), param1);
                }
                break;
            case (int)ChallengeRewardType.TYPE_SP_CLEAR:
                {
                    message = string.Format(GameTextUtil.GetText("growth_boss_23"), param1);
                }
                break;
            case (int)ChallengeRewardType.TYPE_SP_LEVEL:
                {
                    message = string.Format(GameTextUtil.GetText("growth_boss_24"), param1);
                }
                break;
            case (int)ChallengeRewardType.TYPE_NM_LEVEL:
                {
                    if (param1 != param2)
                    {
                        message = string.Format(GameTextUtil.GetText("growth_boss_25"), param1, param2);
                    }
                    else if (loop_cnt == 0)
                    {
                        message = string.Format(GameTextUtil.GetText("growth_boss_26"), param1);
                    }
                    else
                    {
                        message = string.Format(GameTextUtil.GetText("growth_boss_27"), param1, loop_cnt);
                    }
                }
                break;
        }
        return message;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="present_ids"></param>
    private void setupPresent(int[] present_ids)
    {
        MasterDataPresent present = MasterDataUtil.GetPresentParamFromID((uint)present_ids[0]);

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
        for (int i = 0; i < present_ids.Length; i++)
        {
            if (present_ids[0] == present_ids[i]) count++;
        }

        ItemNum = string.Format("{0}", num * count);

    }
}
