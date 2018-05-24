using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：キャラ関連：パラメータ
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataParamChara : Master
{
    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------

    public int rarity { get; set; }                  //!< キャラパラメータ：種類

    public uint timing_public { get; set; }          //!< 一般公開タイミング

    public string name { get; set; }                 //!< キャラパラメータ：名前
    public string detail { get; set; }                   //!< キャラパラメータ：詳細テキスト

    public uint draw_id { get; set; }                //!< 表示用ID

    public MasterDataDefineLabel.RarityType rare { get; set; }                    //!< キャラパラメータ：レア度タイプ

    public string res_chara_id_str { get; set; }     //!< キャラパラメータ：リソースID	※fix_idを3ケタの文字列に変換したもの。リソースのアクセスに使用 → 4桁の子が出てきて扱い難しくなってきたので基本的に使わないように！

    public MasterDataDefineLabel.ElementType element { get; set; }             //!< キャラパラメータ：キャラ性質：属性
    public MasterDataDefineLabel.KindType kind { get; set; }                    //!< キャラパラメータ：キャラ性質：種族
    public MasterDataDefineLabel.KindType sub_kind { get; set; }                //!< キャラパラメータ：キャラ性質：副種族

    public uint skill_leader { get; set; }           //!< キャラパラメータ：スキル：リーダースキル			※[ MasterDataSkillLeader.fix_id		]と一致
    public uint skill_limitbreak { get; set; }       //!< キャラパラメータ：スキル：リミットブレイクスキル	※[ MasterDataSkillLimitBreak.fix_id	]と一致
    public uint skill_passive { get; set; }          //!< キャラパラメータ：スキル：パッシブスキル			※[ MasterDataSkillPassive.fix_id		]と一致
    public uint skill_active0 { get; set; }          //!< キャラパラメータ：スキル：アクティブスキル			※[ MasterDataSkillActive.fix_id		]と一致
    public uint skill_active1 { get; set; }          //!< キャラパラメータ：スキル：アクティブスキル			※[ MasterDataSkillActive.fix_id		]と一致

    public MasterDataDefineLabel.BoolType link_enable { get; set; }         //!< キャラパラメータ：リンク：可否
    public uint link_skill_active { get; set; }      //!< キャラパラメータ：リンク：スキル：アクティブ		※[ MasterDataSkillActive.fix_id		]と一致
    public uint link_skill_passive { get; set; }     //!< キャラパラメータ：リンク：スキル：パッシブ			※[ MasterDataSkillPassive.fix_id		]と一致
    public uint link_unit_id_parts1 { get; set; }    //!< キャラパラメータ：リンク：実行：必要ユニット1		※[ MasterDataParamChara.fix_id			]と一致
    public uint link_unit_id_parts2 { get; set; }    //!< キャラパラメータ：リンク：実行：必要ユニット2		※[ MasterDataParamChara.fix_id			]と一致
    public uint link_unit_id_parts3 { get; set; }    //!< キャラパラメータ：リンク：実行：必要ユニット3		※[ MasterDataParamChara.fix_id			]と一致
    public uint link_money { get; set; }             //!< キャラパラメータ：リンク：実行：必要経費
    public uint link_del_unit_id_parts1 { get; set; }//!< キャラパラメータ：リンク：解除：必要ユニット1		※[ MasterDataParamChara.fix_id			]と一致
    public uint link_del_unit_id_parts2 { get; set; }//!< キャラパラメータ：リンク：解除：必要ユニット2		※[ MasterDataParamChara.fix_id			]と一致
    public uint link_del_unit_id_parts3 { get; set; }//!< キャラパラメータ：リンク：解除：必要ユニット3		※[ MasterDataParamChara.fix_id			]と一致
    public uint link_del_money { get; set; }         //!< キャラパラメータ：リンク：解除：必要経費

    public int party_cost { get; set; }              //!< キャラパラメータ：パーティ編成コスト
    public int level_min { get; set; }               //!< キャラパラメータ：レベル：下限
    public int level_max { get; set; }               //!< キャラパラメータ：レベル：上限

    public int limit_over_type { get; set; }         //!< キャラパラメータ：限界突破タイプ
    public MasterDataDefineLabel.LimitOverSynthesisType limit_over_synthesis_type { get; set; }  //!< キャラパラメータ：限突エッグタイプ
    public int limit_over_param1 { get; set; }      //!< キャラパラメータ：限突エッグパラメータ１
    public int limit_over_value { get; set; }            //!< キャラパラメータ：限突エッグ増加量
    public MasterDataDefineLabel.ElementType limit_over_attribute { get; set; }       //!< キャラパラメータ：未使用
    public int limit_over_unitpoint { get; set; }       //!< キャラパラメータ：限界突破時消費ユニットポイント
    public int evol_unitpoint { get; set; }              //!< キャラパラメータ：進化時消費ユニットポイント
    public int exp_total { get; set; }                   //!< キャラパラメータ：最大レベルまでの総合経験値
    public MasterDataDefineLabel.CurveType exp_total_curve { get; set; }         //!< キャラパラメータ：最大レベルまでの総合経験値補間タイプ

    public int base_hp_min { get; set; }         //!< キャラパラメータ：体力：下限
    public int base_hp_max { get; set; }         //!< キャラパラメータ：体力：上限
    public MasterDataDefineLabel.CurveType base_hp_curve { get; set; }           //!< キャラパラメータ：体力：補間タイプ
    public int base_attack_min { get; set; }     //!< キャラパラメータ：攻撃力：下限
    public int base_attack_max { get; set; }     //!< キャラパラメータ：攻撃力：上限
    public MasterDataDefineLabel.CurveType base_attack_curve { get; set; }       //!< キャラパラメータ：攻撃力：補間タイプ
    public int base_defense_min { get; set; }        //!< キャラパラメータ：防御力：下限
    public int base_defense_max { get; set; }        //!< キャラパラメータ：防御力：上限
    public MasterDataDefineLabel.CurveType base_defense_curve { get; set; }      //!< キャラパラメータ：防御力：補間タイプ
    public int blend_exp_min { get; set; }           //!< キャラパラメータ：素材時経験値：下限
    public int blend_exp_max { get; set; }           //!< キャラパラメータ：素材時経験値：上限
    public MasterDataDefineLabel.CurveType blend_exp_curve { get; set; }     //!< キャラパラメータ：素材時経験値：補間タイプ
    public int skill_plus { get; set; }              //!< キャラパラメータ：スキルアップ確率
    public MasterDataDefineLabel.SkillLevelElementType skill_plus_element { get; set; }      //!< キャラパラメータ：対応属性
    public int sales_min { get; set; }               //!< キャラパラメータ：売却価格：下限
    public int sales_max { get; set; }               //!< キャラパラメータ：売却価格：上限
    public MasterDataDefineLabel.CurveType sales_curve { get; set; }         //!< キャラパラメータ：売却価格：補間タイプ
    public int material_link_point { get; set; } //!< キャラパラメータ：素材時リンクポイント
    public int sales_unitpoint { get; set; }     //!< キャラパラメータ：売却時付与ユニットポイント
    public MasterDataDefineLabel.BoolType wild_egg_flg { get; set; }            //!< キャラパラメータ：ワイルドエッグ対応フラグ

    public int img_0_tiling { get; set; }            //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_0_offsetX { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_0_offsetY { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_1_tiling { get; set; }            //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_1_offsetX { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_1_offsetY { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_2_tiling { get; set; }            //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_2_offsetX { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_2_offsetY { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_3_tiling { get; set; }            //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_3_offsetX { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_3_offsetY { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_4_tiling { get; set; }            //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_4_offsetX { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_4_offsetY { get; set; }           //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_0_link_tiling { get; set; }       //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_0_link_offsetX { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_0_link_offsetY { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_1_link_tiling { get; set; }       //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_1_link_offsetX { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_1_link_offsetY { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_2_link_tiling { get; set; }       //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_2_link_offsetX { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_2_link_offsetY { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_3_link_tiling { get; set; }       //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_3_link_offsetX { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_3_link_offsetY { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_4_link_tiling { get; set; }       //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_4_link_offsetX { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正
    public int img_4_link_offsetY { get; set; }      //!< キャラパラメータ：パーティ表示時の画像補正

    public int img_cutin_tiling { get; set; }        //!< キャラパラメータ：カットイン表示時の画像補正
    public int img_cutin_offsetX { get; set; }       //!< キャラパラメータ：カットイン表示時の画像補正
    public int img_cutin_offsetY { get; set; }       //!< キャラパラメータ：カットイン表示時の画像補正

    public int img_cutin_link_tiling { get; set; }   //!< キャラパラメータ：リンクカットイン表示時の画像補正
    public int img_cutin_link_offsetX { get; set; }  //!< キャラパラメータ：リンクカットイン表示時の画像補正
    public int img_cutin_link_offsetY { get; set; }  //!< キャラパラメータ：リンクカットイン表示時の画像補正

    public int img_cutin_mm_tiling { get; set; } //!< キャラパラメータ：メインメニューUIカットイン表示時の画像補正
    public int img_cutin_mm_offsetX { get; set; }    //!< キャラパラメータ：メインメニューUIカットイン表示時の画像補正
    public int img_cutin_mm_offsetY { get; set; }    //!< キャラパラメータ：メインメニューUIカットイン表示時の画像補正

    public int size_width { get; set; }              //!< キャラパラメータ：メッシュ表示時のサイズ：横幅
    public int size_height { get; set; }         //!< キャラパラメータ：メッシュ表示時のサイズ：縦幅
    public MasterDataDefineLabel.PivotType pivot { get; set; }                   //!< キャラパラメータ：メッシュ表示時の原点タイプ
    public int side_offset { get; set; }         //!< キャラパラメータ：メッシュ表示時の隣とのオフセット

    public int illustrator_id { get; set; }    //!< キャラパラメータ：MasterDataIllustrator.fix_id

};
