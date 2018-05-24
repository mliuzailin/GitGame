[System.Serializable]
public class MasterDataHero : Master
{
    public string name { get; set; } //!< 名前

    public string detail { get; set; } //!< 詳細テキスト

    public int element { get; set; } //!< 属性

    public int kind { get; set; } //!< 主人公の種類

    public int gender { get; set; } //!< 性別

    public int default_skill_id { get; set; } //!< 初期必殺技ID
    public int default_party_id { get; set; } //!< 

    public int img_story_tiling { get; set; } //!< ストーリ画面表示時の画像補正
    public int img_story_offset_x { get; set; } //!< ストーリ画面表示時の画像補正
    public int img_story_offset_y { get; set; } //!< ストーリ画面表示時の画像補正

    public uint illustrator_id { get; set; } //!< イラストレータID


    public MasterDataDefineLabel.ElementType getElement()
    {
        return (MasterDataDefineLabel.ElementType)element;
    }

    public enum HeroKind
    {
        NONE,
        STUDENT,
        TEACHER,
    }

    public HeroKind getKind()
    {
        return (HeroKind)kind;
    }

    public enum GenderType
    {
        NONE,
        HUMAN_MALE,
        HUMAN_FEMALE,
    }

    public GenderType getGender()
    {
        return (GenderType)gender;
    }
}
