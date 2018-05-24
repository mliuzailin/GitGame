using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataChallengeReward : Master
{
    public int event_id { get; set; }

    public int type { get; set; }

    public int clear_param { get; set; }
    public int clear_loop_reward_enable { get; set; }
    public int clear_loop_reward_count { get; set; }

    public int present_group_id { get; set; }
    public string present_message { get; set; }
}
