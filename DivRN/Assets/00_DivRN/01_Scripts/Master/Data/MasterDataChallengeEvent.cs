using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataChallengeEvent : Master
{
    public uint event_id { get; set; }           //!< MasterDataEventのfix_id

    public uint timing_start { get; set; }
    public uint receiving_end { get; set; }

    public string title { get; set; }

    public int level_cap { get; set; }

    public int boss_cap_level { get; set; }
    public int boss_cap_hp { get; set; }
    public int boss_hp_curve_coefficient_num { get; set; }
    public int boss_hp_growth_coefficient_num { get; set; }
    public int boss_cap_attack { get; set; }
    public int boss_atk_curve_coefficient_num { get; set; }
    public int boss_atk_growth_coefficient_num { get; set; }

    public int side_cap_level { get; set; }
    public int side_cap_hp { get; set; }
    public int side_hp_curve_coefficient_num { get; set; }
    public int side_hp_growth_coefficient_num { get; set; }
    public int side_cap_attack { get; set; }
    public int side_atk_curve_coefficient_num { get; set; }
    public int side_atk_growth_coefficient_num { get; set; }

    public string bg_assetbundle_name { get; set; }

    public int skip_level { get; set; }
    public int skip_base_ticket_num { get; set; }
    public int skip_stepup_ticket_num { get; set; }
    public int skip_stepup_max { get; set; }
    public int skip_max { get; set; }

    public int open_count { get; set; }
}
