using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@brief		マスターデータ実体：クエスト参加条件
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class MasterDataQuestRequirement : Master
{
    public uint timing_public { get; set; }                                           //!< 一般公開タイミング

    public MasterDataDefineLabel.BoolType elem_fire { get; set; }                     //!< 属性：炎
    public MasterDataDefineLabel.BoolType elem_water { get; set; }                    //!< 属性：水
    public MasterDataDefineLabel.BoolType elem_wind { get; set; }                     //!< 属性：風
    public MasterDataDefineLabel.BoolType elem_light { get; set; }                    //!< 属性：光
    public MasterDataDefineLabel.BoolType elem_dark { get; set; }                     //!< 属性：闇
    public MasterDataDefineLabel.BoolType elem_naught { get; set; }                   //!< 属性：無

    public MasterDataDefineLabel.BoolType kind_human { get; set; }                    //!< 種族：人間
    public MasterDataDefineLabel.BoolType kind_fairy { get; set; }                    //!< 種族：妖精
    public MasterDataDefineLabel.BoolType kind_demon { get; set; }                    //!< 種族：魔物
    public MasterDataDefineLabel.BoolType kind_dragon { get; set; }                   //!< 種族：竜
    public MasterDataDefineLabel.BoolType kind_machine { get; set; }                  //!< 種族：機械
    public MasterDataDefineLabel.BoolType kind_beast { get; set; }                    //!< 種族：獣
    public MasterDataDefineLabel.BoolType kind_god { get; set; }                      //!< 種族：神
    public MasterDataDefineLabel.BoolType kind_egg { get; set; }                      //!< 種族：強化合成

    public int num_elem { get; set; }                                                 //!< 属性数
    public int num_kind { get; set; }                                                 //!< 種族数
    public int num_unit { get; set; }                                                 //!< ユニット数

    public MasterDataDefineLabel.BoolType much_name { get; set; }                     //!< 同名禁止

    public uint require_unit_00 { get; set; }                                         //!< 必須ユニットID
    public uint require_unit_01 { get; set; }                                         //!< 必須ユニットID
    public uint require_unit_02 { get; set; }                                         //!< 必須ユニットID
    public uint require_unit_03 { get; set; }                                         //!< 必須ユニットID
    public uint require_unit_04 { get; set; }                                         //!< 必須ユニットID

    public MasterDataDefineLabel.RarityType limit_rare { get; set; }                  //!< 制限レア度
    public int limit_cost { get; set; }                                               //!< 制限コスト
    public int limit_cost_total { get; set; }                                         //!< 制限コスト合計
    public int limit_unit_lv { get; set; }                                            //!< 制限ユニットレベル
    public int limit_unit_lv_total { get; set; }                                      //!< 制限ユニットレベル合計
    public int limit_rank { get; set; }                                               //!< 制限ランク

    public int rule_disable_as { get; set; }                                          //!< AS禁止
    public int rule_disable_ls { get; set; }                                          //!< LS禁止
    public int rule_heal_half { get; set; }                                           //!< NS半減
    public int rule_disable_affinity { get; set; }                                    //!< 属性相性無し

    public MasterDataDefineLabel.BoolType fix_unit_00_enable { get; set; }            //!< 固定パーティ：リーダー：有効無効
    public uint fix_unit_00_id { get; set; }                                          //!< 固定パーティ：リーダー：ユニットID
    public int fix_unit_00_lv { get; set; }                                           //!< 固定パーティ：リーダー：ユニットレベル
    public int fix_unit_00_lv_lbs { get; set; }                                       //!< 固定パーティ：リーダー：スキルレベル
    public int fix_unit_00_lv_lo { get; set; }                                        //!< 固定パーティ：リーダー：限界突破レベル
    public int fix_unit_00_plus_hp { get; set; }                                      //!< 固定パーティ：リーダー：プラス値：HP
    public int fix_unit_00_plus_atk { get; set; }                                     //!< 固定パーティ：リーダー：プラス値：ATK
    public MasterDataDefineLabel.BoolType fix_unit_00_link_enable { get; set; }       //!< 固定パーティ：リーダー：リンク：有効無効
    public uint fix_unit_00_link_id { get; set; }                                     //!< 固定パーティ：リーダー：リンク：ユニットID
    public int fix_unit_00_link_lv { get; set; }                                      //!< 固定パーティ：リーダー：リンク：ユニットレベル
    public int fix_unit_00_link_plus_hp { get; set; }                                 //!< 固定パーティ：リーダー：リンク：プラス値：HP
    public int fix_unit_00_link_plus_atk { get; set; }                                //!< 固定パーティ：リーダー：リンク：プラス値：ATK
    public int fix_unit_00_link_point { get; set; }                                   //!< 固定パーティ：リーダー：リンク：ポイント
    public int fix_unit_00_link_lv_lo { get; set; }                                   //!< 固定パーティ：リーダー：リンク：限界突破レベル

    public MasterDataDefineLabel.BoolType fix_unit_01_enable { get; set; }            //!< 固定パーティ：サブ０１：有効無効
    public uint fix_unit_01_id { get; set; }                                          //!< 固定パーティ：サブ０１：ユニットID
    public int fix_unit_01_lv { get; set; }                                           //!< 固定パーティ：サブ０１：ユニットレベル
    public int fix_unit_01_lv_lbs { get; set; }                                       //!< 固定パーティ：サブ０１：スキルレベル
    public int fix_unit_01_lv_lo { get; set; }                                        //!< 固定パーティ：サブ０１：限界突破レベル
    public int fix_unit_01_plus_hp { get; set; }                                      //!< 固定パーティ：サブ０１：プラス値：HP
    public int fix_unit_01_plus_atk { get; set; }                                     //!< 固定パーティ：サブ０１：プラス値：ATK
    public MasterDataDefineLabel.BoolType fix_unit_01_link_enable { get; set; }       //!< 固定パーティ：サブ０１：リンク：有効無効
    public uint fix_unit_01_link_id { get; set; }                                     //!< 固定パーティ：サブ０１：リンク：ユニットID
    public int fix_unit_01_link_lv { get; set; }                                      //!< 固定パーティ：サブ０１：リンク：ユニットレベル
    public int fix_unit_01_link_plus_hp { get; set; }                                 //!< 固定パーティ：サブ０１：リンク：プラス値：HP
    public int fix_unit_01_link_plus_atk { get; set; }                                //!< 固定パーティ：サブ０１：リンク：プラス値：ATK
    public int fix_unit_01_link_point { get; set; }                                   //!< 固定パーティ：サブ０１：リンク：ポイント
    public int fix_unit_01_link_lv_lo { get; set; }                                   //!< 固定パーティ：リーダー：リンク：限界突破レベル

    public MasterDataDefineLabel.BoolType fix_unit_02_enable { get; set; }            //!< 固定パーティ：サブ０２：有効無効
    public uint fix_unit_02_id { get; set; }                                          //!< 固定パーティ：サブ０２：ユニットID
    public int fix_unit_02_lv { get; set; }                                           //!< 固定パーティ：サブ０２：ユニットレベル
    public int fix_unit_02_lv_lbs { get; set; }                                       //!< 固定パーティ：サブ０２：スキルレベル
    public int fix_unit_02_lv_lo { get; set; }                                        //!< 固定パーティ：サブ０２：限界突破レベル
    public int fix_unit_02_plus_hp { get; set; }                                      //!< 固定パーティ：サブ０２：プラス値：HP
    public int fix_unit_02_plus_atk { get; set; }                                     //!< 固定パーティ：サブ０２：プラス値：ATK
    public MasterDataDefineLabel.BoolType fix_unit_02_link_enable { get; set; }       //!< 固定パーティ：サブ０２：リンク：有効無効
    public uint fix_unit_02_link_id { get; set; }                                     //!< 固定パーティ：サブ０２：リンク：ユニットID
    public int fix_unit_02_link_lv { get; set; }                                      //!< 固定パーティ：サブ０２：リンク：ユニットレベル
    public int fix_unit_02_link_plus_hp { get; set; }                                 //!< 固定パーティ：サブ０２：リンク：プラス値：HP
    public int fix_unit_02_link_plus_atk { get; set; }                                //!< 固定パーティ：サブ０２：リンク：プラス値：ATK
    public int fix_unit_02_link_point { get; set; }                                   //!< 固定パーティ：サブ０２：リンク：ポイント
    public int fix_unit_02_link_lv_lo { get; set; }                                   //!< 固定パーティ：リーダー：リンク：限界突破レベル

    public MasterDataDefineLabel.BoolType fix_unit_03_enable { get; set; }            //!< 固定パーティ：サブ０３：有効無効
    public uint fix_unit_03_id { get; set; }                                          //!< 固定パーティ：サブ０３：ユニットID
    public int fix_unit_03_lv { get; set; }                                           //!< 固定パーティ：サブ０３：ユニットレベル
    public int fix_unit_03_lv_lbs { get; set; }                                       //!< 固定パーティ：サブ０３：スキルレベル
    public int fix_unit_03_lv_lo { get; set; }                                        //!< 固定パーティ：サブ０３：限界突破レベル
    public int fix_unit_03_plus_hp { get; set; }                                      //!< 固定パーティ：サブ０３：プラス値：HP
    public int fix_unit_03_plus_atk { get; set; }                                     //!< 固定パーティ：サブ０３：プラス値：ATK
    public MasterDataDefineLabel.BoolType fix_unit_03_link_enable { get; set; }       //!< 固定パーティ：サブ０３：リンク：有効無効
    public uint fix_unit_03_link_id { get; set; }                                     //!< 固定パーティ：サブ０３：リンク：ユニットID
    public int fix_unit_03_link_lv { get; set; }                                      //!< 固定パーティ：サブ０３：リンク：ユニットレベル
    public int fix_unit_03_link_plus_hp { get; set; }                                 //!< 固定パーティ：サブ０３：リンク：プラス値：HP
    public int fix_unit_03_link_plus_atk { get; set; }                                //!< 固定パーティ：サブ０３：リンク：プラス値：ATK
    public int fix_unit_03_link_point { get; set; }                                   //!< 固定パーティ：サブ０３：リンク：ポイント
    public int fix_unit_03_link_lv_lo { get; set; }                                   //!< 固定パーティ：リーダー：リンク：限界突破レベル

    public MasterDataDefineLabel.BoolType fix_unit_04_enable { get; set; }            //!< 固定パーティ：フレンド：有効無効
    public uint fix_unit_04_id { get; set; }                                          //!< 固定パーティ：フレンド：ユニットID
    public int fix_unit_04_lv { get; set; }                                           //!< 固定パーティ：フレンド：ユニットレベル
    public int fix_unit_04_lv_lbs { get; set; }                                       //!< 固定パーティ：フレンド：スキルレベル
    public int fix_unit_04_lv_lo { get; set; }                                        //!< 固定パーティ：フレンド：限界突破レベル
    public int fix_unit_04_plus_hp { get; set; }                                      //!< 固定パーティ：フレンド：プラス値：HP
    public int fix_unit_04_plus_atk { get; set; }                                     //!< 固定パーティ：フレンド：プラス値：ATK
    public MasterDataDefineLabel.BoolType fix_unit_04_link_enable { get; set; }       //!< 固定パーティ：フレンド：リンク：有効無効
    public uint fix_unit_04_link_id { get; set; }                                     //!< 固定パーティ：フレンド：リンク：ユニットID
    public int fix_unit_04_link_lv { get; set; }                                      //!< 固定パーティ：フレンド：リンク：ユニットレベル
    public int fix_unit_04_link_plus_hp { get; set; }                                 //!< 固定パーティ：フレンド：リンク：プラス値：HP
    public int fix_unit_04_link_plus_atk { get; set; }                                //!< 固定パーティ：フレンド：リンク：プラス値：ATK
    public int fix_unit_04_link_point { get; set; }                                   //!< 固定パーティ：フレンド：リンク：ポイント
    public int fix_unit_04_link_lv_lo { get; set; }                                   //!< 固定パーティ：リーダー：リンク：限界突破レベル

};

