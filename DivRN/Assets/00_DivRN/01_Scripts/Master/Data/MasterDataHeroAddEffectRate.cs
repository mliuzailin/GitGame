//----------------------------------------------------------------------------
/*!
    @brief  マスターデータ実体：ヒーロー関連：
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataHeroAddEffectRate : Master
{
    public int hero_id { get; set; }

    public int start_level { get; set; }

    public int end_level { get; set; }

    public int additional_effect_value { get; set; }
}
