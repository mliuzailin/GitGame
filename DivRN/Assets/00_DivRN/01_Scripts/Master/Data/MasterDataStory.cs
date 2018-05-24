using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マスターデータ実体：ストーリー
/// </summary>
[System.Serializable]
public class MasterDataStory : Master
{
    //public uint fix_id;                                                        //!< 情報固有固定ID
    public uint story_id { get; set; } //!< ストーリーID
    // キャラ1
    public MasterDataDefineLabel.BoolType show_character_01_active { get; set; } //!< 表示キャラon/off
    public MasterDataDefineLabel.BoolType show_character_01_slide { get; set; } //!< スライドイン・アウト
    public uint show_character_01 { get; set; } //!< 表示キャラID ※[ MasterDataParamChara.fix_id]と一致
    public uint show_hero_01 { get; set; } //!< 表示キャラID（主人公）
    public uint show_npc_01 { get; set; } //!< 表示キャラID（その他）
    public uint show_chara_01_background { get; set; } //!< 背景枠
    public MasterDataDefineLabel.BoolType name_01_active { get; set; } //!< 名前のon/off
    public string name_01 { get; set; } //!< 名前

    // キャラ2
    public MasterDataDefineLabel.BoolType show_character_02_active { get; set; } //!< 表示キャラon/off
    public MasterDataDefineLabel.BoolType show_character_02_slide { get; set; } //!< スライドイン・アウト
    public uint show_character_02 { get; set; } //!< 表示キャラID ※[ MasterDataParamChara.fix_id]と一致
    public uint show_hero_02 { get; set; } //!< 表示キャラID（主人公）
    public uint show_npc_02 { get; set; } //!< 表示キャラID（その他）
    public uint show_chara_02_background { get; set; } //!< 背景枠
    public MasterDataDefineLabel.BoolType name_02_active { get; set; } //!< 名前のon/off
    public string name_02 { get; set; } //!< 名前

    // キャラ3
    public MasterDataDefineLabel.BoolType show_character_03_active { get; set; } //!< 表示キャラon/off
    public MasterDataDefineLabel.BoolType show_character_03_slide { get; set; } //!< スライドイン・アウト
    public uint show_character_03 { get; set; } //!< 表示キャラID ※[ MasterDataParamChara.fix_id]と一致
    public uint show_hero_03 { get; set; } //!< 表示キャラID（主人公）
    public uint show_npc_03 { get; set; } //!< 表示キャラID（その他）
    public uint show_chara_03_background { get; set; } //!< 背景枠
    public MasterDataDefineLabel.BoolType name_03_active { get; set; } //!< 名前のon/off
    public string name_03 { get; set; } //!< 名前

    // キャラ4
    public MasterDataDefineLabel.BoolType show_character_04_active { get; set; } //!< 表示キャラon/off
    public MasterDataDefineLabel.BoolType show_character_04_slide { get; set; } //!< スライドイン・アウト
    public uint show_character_04 { get; set; } //!< 表示キャラID ※[ MasterDataParamChara.fix_id]と一致
    public uint show_hero_04 { get; set; } //!< 表示キャラID（主人公）
    public uint show_npc_04 { get; set; } //!< 表示キャラID（その他）
    public uint show_chara_04_background { get; set; } //!< 背景枠
    public MasterDataDefineLabel.BoolType name_04_active { get; set; } //!< 名前のon/off
    public string name_04 { get; set; } //!< 名前

    //public string name                                           { get; set; } //!< 名前

    public MasterDataDefineLabel.StoryCharFocus focus { get; set; } //!< フォーカス
    public string text { get; set; } //!< テキスト
    public uint balloon_type { get; set; } //!< 吹き出しの種類

    public uint back_ground_res { get; set; } //!< 背景

    public SEID se_res { get; set; } //!< SEリソース
    public MasterDataDefineLabel.BoolType bgm_active { get; set; } //!< BGMのon/off
    public BGMManager.EBGM_ID bgm_res { get; set; } //!< bgmリソース

    public MasterDataDefineLabel.BoolType effect_fade_view { get; set; } //!< 画面のフェードイン・アウト

    public string label { get; set; } //!< ラベル
    public string command { get; set; } //!< コマンド
}

