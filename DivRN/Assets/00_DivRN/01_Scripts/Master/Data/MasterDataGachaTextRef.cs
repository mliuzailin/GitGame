using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataGachaTextRef : Master
{
    public int master_type { get; set; }
    public int master_fix_id { get; set; }
    public int guideline_text_id { get; set; }
    public int detail_text_id { get; set; }
    public int normal1_rate_url_text_id { get; set; }
    public int normal2_rate_url_text_id { get; set; }
    public int special_rate_url_text_id { get; set; }
    public MasterDataDefineLabel.BoolType show_rate_flag { get; set; }
}
