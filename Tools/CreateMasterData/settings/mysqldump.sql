-- MySQL dump 10.13  Distrib 5.5.54, for Linux (x86_64)
--
-- Host: goe-stg1-aurora-master-cluster.cluster-cwqorxbdzwm6.ap-northeast-1.rds.amazonaws.com    Database: stg3a_pqdm
-- ------------------------------------------------------
-- Server version	5.6.10

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `user_rank_master`
--

DROP TABLE IF EXISTS `user_rank_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user_rank_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `exp_next` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '経験値：次までの数値',
  `exp_next_total` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '経験値：次までの合計値',
  `stamina` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'スタミナ値',
  `friend_max` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'フレンド保持数上限',
  `unit_max` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット保持数上限',
  `party_cost` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'パーティコスト上限',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `default_party_master`
--

DROP TABLE IF EXISTS `default_party_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `default_party_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有固定ID',
  `party_name` varchar(32) NOT NULL DEFAULT '' COMMENT 'パーティー名称',
  `party_chara0_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ0：ID',
  `party_chara0_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ0：レベル',
  `party_chara1_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ1：ID',
  `party_chara1_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ1：レベル',
  `party_chara2_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ2：ID',
  `party_chara2_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ2：レベル',
  `party_chara3_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ3：ID',
  `party_chara3_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ3：レベル',
  `party_chara4_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ4：ID',
  `party_chara4_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ4：レベル',
  `material_chara0_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ0：ID',
  `material_chara0_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ0：レベル',
  `material_chara1_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ1：ID',
  `material_chara1_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ1：レベル',
  `material_chara2_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ2：ID',
  `material_chara2_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ2：レベル',
  `material_chara3_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ3：ID',
  `material_chara3_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ3：レベル',
  `material_chara4_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ4：ID',
  `material_chara4_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '素材キャラ4：レベル',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `chara_master`
--

DROP TABLE IF EXISTS `chara_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chara_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `name` varchar(64) NOT NULL DEFAULT '' COMMENT '名前',
  `detail` text NOT NULL COMMENT '詳細テキスト',
  `draw_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラパラメータ：表示用ID',
  `rare` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'レア度タイプ',
  `rarity` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラパラメータ：フェス限フラグ',
  `res_chara_id_str` varchar(6) NOT NULL DEFAULT '' COMMENT 'リソースID',
  `element` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ性質：属性',
  `kind` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ性質：種族',
  `sub_kind` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラ性質：副種族',
  `skill_leader` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル：リーダースキル',
  `skill_limitbreak` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル：リミットブレイクスキル',
  `skill_passive` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル：パッシブスキル',
  `skill_active0` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル：アクティブスキル',
  `skill_active1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル：アクティブスキル',
  `link_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：可否',
  `link_skill_active` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク：スキル：アクティブ',
  `link_skill_passive` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク：スキル：パッシブ',
  `link_unit_id_parts1` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク実行必要ユニット1',
  `link_unit_id_parts2` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク実行必要ユニット2',
  `link_unit_id_parts3` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク実行必要ユニット3',
  `link_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク実行必要経費',
  `link_del_unit_id_parts1` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク解除必要ユニット1',
  `link_del_unit_id_parts2` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク解除必要ユニット2',
  `link_del_unit_id_parts3` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク解除必要ユニット3',
  `link_del_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク解除必要経費',
  `party_cost` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パーティ編成コスト',
  `level_min` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'レベル：下限',
  `level_max` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'レベル：上限',
  `limit_over_type` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラパラメーター：限界突破タイプ',
  `limit_over_value` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラパラメーター：限界突破増加量',
  `limit_over_synthesis_type` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラパラメーター：限界突破合成タイプ',
  `limit_over_attribute` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラパラメーター：限界突破対応属性',
  `limit_over_param1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラパラメーター：限界突破対応汎用カラム１',
  `limit_over_unitpoint` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラパラメーター：ポイントショップから限界突破を行う際の消費ユニットポイント',
  `evol_unitpoint` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラパラメーター：ポイントショップから進化を行う際の消費ユニットポイント',
  `exp_total` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '最大レベルまでの総合経験値',
  `exp_total_curve` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '最大レベルまでの総合経験値補間タイプ',
  `base_hp_min` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '体力：下限',
  `base_hp_max` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '体力：上限',
  `base_hp_curve` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '体力補間タイプ',
  `base_attack_min` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '攻撃力：下限',
  `base_attack_max` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '攻撃力：上限',
  `base_attack_curve` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '攻撃力：補間タイプ',
  `base_defense_min` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '防御力：下限',
  `base_defense_max` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '防御力：上限',
  `base_defense_curve` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '防御力：補間タイプ',
  `blend_exp_min` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '素材経験値：下限',
  `blend_exp_max` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '素材経験値：上限',
  `blend_exp_curve` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '素材経験値：補間タイプ',
  `skill_plus` int(10) NOT NULL DEFAULT '0' COMMENT 'スキルレベルアップ値',
  `skill_plus_element` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラパラメータ：対応属性',
  `sales_min` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '売却価格：下限',
  `sales_max` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '売却価格：上限',
  `sales_curve` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '売却価格：補間タイプ',
  `material_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラパラメータ：素材時リンクポイント',
  `sales_unitpoint` int(10) NOT NULL DEFAULT '0' COMMENT '売却時付与ユニットポイント',
  `wild_egg_flg` int(10) NOT NULL DEFAULT '0' COMMENT 'ワイルドエッグ対応属性',
  `img_0_tiling` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_0_offsetX` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_0_offsetY` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_1_tiling` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_1_offsetX` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_1_offsetY` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_2_tiling` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_2_offsetX` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_2_offsetY` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_3_tiling` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_3_offsetX` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_3_offsetY` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_4_tiling` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_4_offsetX` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_4_offsetY` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_0_link_tiling` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_0_link_offsetX` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_0_link_offsetY` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_1_link_tiling` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_1_link_offsetX` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_1_link_offsetY` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_2_link_tiling` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_2_link_offsetX` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_2_link_offsetY` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_3_link_tiling` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_3_link_offsetX` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_3_link_offsetY` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_4_link_tiling` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_4_link_offsetX` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_4_link_offsetY` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクキャラ',
  `img_cutin_tiling` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_cutin_offsetX` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_cutin_offsetY` smallint(5) NOT NULL DEFAULT '0' COMMENT 'パーティー表示時の画像補整',
  `img_cutin_link_tiling` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクカットイン',
  `img_cutin_link_offsetX` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクカットイン',
  `img_cutin_link_offsetY` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクカットイン',
  `img_cutin_mm_tiling` int(10) NOT NULL DEFAULT '0' COMMENT 'メインメニューUIユニットカットイン',
  `img_cutin_mm_offsetX` int(10) NOT NULL DEFAULT '0' COMMENT 'メインメニューUIユニットカットイン',
  `img_cutin_mm_offsetY` int(10) NOT NULL DEFAULT '0' COMMENT 'メインメニューUIユニットカットイン',
  `size_width` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'メッシュ表示時のサイズ：横幅',
  `size_height` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'メッシュ表示時のサイズ：縦幅',
  `pivot` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'メッシュ表示時の原点タイプ',
  `side_offset` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'メッシュ表示時の隣とのオフセット',
  `illustrator_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '絵師ID illustrator_master.fix_id',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `chara_evol_master`
--

DROP TABLE IF EXISTS `chara_evol_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chara_evol_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `unit_id_pre` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID：進化元（ベース素材）',
  `unit_id_after` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID：進化後',
  `unit_id_parts1` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID：素材１',
  `unit_id_parts2` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID：素材２',
  `unit_id_parts3` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID：素材３',
  `unit_id_parts4` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID：素材４',
  `friend_elem` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'フレンド制限：属性',
  `friend_kind` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'フレンド制限：種族',
  `friend_level` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'フレンド制限：レベル',
  `money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '必要経費',
  `quest_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '進化クエストID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  UNIQUE KEY `unit_id_pre` (`unit_id_pre`),
  KEY `idx_unit_id_after` (`unit_id_after`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `skill_leader_master`
--

DROP TABLE IF EXISTS `skill_leader_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `skill_leader_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `name` varchar(64) NOT NULL DEFAULT '' COMMENT '名称和名',
  `detail` text NOT NULL COMMENT '詳細説明',
  `add_fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リーダースキル情報：追加リーダースキル',
  `skill_powup_elem_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '味方属性別攻撃力倍率効果：効果有無',
  `skill_powup_elem_type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '味方属性別攻撃力倍率効果：属性',
  `skill_powup_elem_status` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '味方属性別攻撃力倍率効果：ステータスタイプ',
  `skill_powup_elem_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '味方属性別攻撃力倍率効果：倍率',
  `skill_powup_kind_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '味方種族別攻撃力倍率効果：効果有無',
  `skill_powup_kind_type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '味方種族別攻撃力倍率効果：属性',
  `skill_powup_kind_status` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '味方種族別攻撃力倍率効果：ステータスタイプ',
  `skill_powup_kind_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '味方種族別攻撃力倍率効果：倍率',
  `skill_follow_atk_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '追い打ち効果：効果有無',
  `skill_follow_atk_element` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '追い打ち効果：属性',
  `skill_follow_atk_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '追い打ち効果：倍率',
  `skill_follow_atk_effect` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '追い打ち効果：エフェクトタイプ',
  `skill_decline_dmg_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '味方属性別攻撃力倍率効果：効果有無',
  `skill_decline_dmg_element` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '味方属性別攻撃力倍率効果：属性',
  `skill_decline_dmg_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '味方属性別攻撃力倍率効果：倍率',
  `skill_recovery_move_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '移動リジェネ：効果有無',
  `skill_recovery_move_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '移動リジェネ：倍率',
  `skill_recovery_battle_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'バトルリジェネ：効果有無',
  `skill_recovery_battle_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'バトルリジェネ：倍率',
  `skill_quick_time_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '戦闘時秒数補正効果：効果有無',
  `skill_quick_time_second` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '戦闘時秒数補正効果：加算秒数',
  `skill_recovery_support_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '回復量アップ効果：効果有無',
  `skill_recovery_support_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '回復量アップ効果：倍率',
  `skill_recovery_atk_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '攻撃時回復効果：効果有無',
  `skill_recovery_atk_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '攻撃時回復効果：倍率',
  `skill_hpfull_powup_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '体力最大時攻撃力倍化効果：効果有無',
  `skill_hpfull_powup_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '体力最大時攻撃力倍化効果：倍率',
  `skill_hpdown_powup_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '体力減少時攻撃力倍化効果：効果有無',
  `skill_hpdown_powup_border` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '体力減少時攻撃力倍化効果：体力しきい値',
  `skill_hpdown_powup_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '体力減少時攻撃力倍化効果：倍率',
  `skill_mekuri_powup_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パネルめくり数比例攻撃力倍化効果：効果有無',
  `skill_funbari_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ふんばり効果：効果有無',
  `skill_funbari_border` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ふんばり効果：体力しきい値',
  `skill_hpfull_guard_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '体力最大時被ダメ減衰効果：効果有無',
  `skill_hpfull_guard_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '体力最大時被ダメ減衰効果：倍率',
  `skill_initiative_atk_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：効果有無',
  `skill_initiative_atk_b_0` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：青パネル：先制',
  `skill_initiative_atk_b_1` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：青パネル：通常',
  `skill_initiative_atk_b_2` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：青パネル：不意打ち',
  `skill_initiative_atk_y_0` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：黄パネル：先制',
  `skill_initiative_atk_y_1` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：黄パネル：通常',
  `skill_initiative_atk_y_2` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：黄パネル：不意打ち',
  `skill_initiative_atk_r_0` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：赤パネル：先制',
  `skill_initiative_atk_r_1` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：赤パネル：通常',
  `skill_initiative_atk_r_2` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '先制攻撃発動割合変動効果：赤パネル：不意打ち',
  `skill_transform_card_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '指定属性の手札を指定属性のカードに変換：効果有無',
  `skill_transform_card_root` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '指定属性の手札を指定属性のカードに変換：変換前',
  `skill_transform_card_dest` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '指定属性の手札を指定属性のカードに変換：変換後',
  `skill_damageup_color_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '指定色以上で攻撃をした際にダメージUP：効果有無',
  `skill_damageup_color_count` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '指定色以上で攻撃をした際にダメージUP：色数',
  `skill_damageup_color_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '指定色以上で攻撃をした際にダメージUP：ダメージ倍率',
  `skill_damageup_hands_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '指定HANDS以上で攻撃をした際にダメージUP：効果有無',
  `skill_damageup_hands_count` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '指定HANDS以上で攻撃をした際にダメージUP：HANDS数',
  `skill_damageup_hands_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '指定HANDS以上で攻撃をした際にダメージUP：ダメージ倍率',
  `skill_type` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル効果',
  `skill_value_00` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_00',
  `skill_value_01` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_01',
  `skill_value_02` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_02',
  `skill_value_03` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_03',
  `skill_value_04` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_04',
  `skill_value_05` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_05',
  `skill_value_06` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_06',
  `skill_value_07` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_07',
  `skill_value_08` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_08',
  `skill_value_09` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_09',
  `skill_value_10` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_10',
  `skill_value_11` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_11',
  `skill_value_12` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_12',
  `skill_value_13` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_13',
  `skill_value_14` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_14',
  `skill_value_15` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ_15',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `skill_active_master`
--

DROP TABLE IF EXISTS `skill_active_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `skill_active_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `name` varchar(64) NOT NULL DEFAULT '' COMMENT '名前',
  `detail` text NOT NULL COMMENT '詳細説明',
  `always` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '常時発動フラグ',
  `skill_element` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '属性',
  `cost1` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト1',
  `cost2` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト2',
  `cost3` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト3',
  `cost4` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト4',
  `cost5` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト5',
  `effect` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'エフェクト番号',
  `skill_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '汎用スキル効果：有効無効',
  `skill_type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '汎用スキル効果：タイプ',
  `skill_value` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '汎用スキル効果：効果値',
  `skill_value_rand` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '汎用スキル効果：効果値振れ幅',
  `skill_poison_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '毒スキル効果：有効無効',
  `skill_poison_turn_min` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '毒スキル効果：持続ターン最小',
  `skill_poison_turn_max` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '毒スキル効果：持続ターン最大',
  `skill_poison_scale` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '毒スキル効果：効果倍率',
  `skill_coerce_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '威圧スキル効果：有効無効',
  `skill_coerce_turn_min` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '威圧スキル効果：付加ターン最小',
  `skill_coerce_turn_max` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '威圧スキル効果：付加ターン最大',
  `skill_guard_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '被ダメ軽減スキル効果：有効無効',
  `skill_guard_turn_min` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '被ダメ軽減スキル効果：持続ターン最小',
  `skill_guard_turn_max` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '被ダメ軽減スキル効果：持続ターン最大',
  `skill_guard_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '被ダメ軽減スキル効果：軽減倍率',
  `skill_week_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '敵キャラ軟化スキル効果：有効無効',
  `skill_week_turn_min` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '敵キャラ軟化スキル効果：持続ターン最小',
  `skill_week_turn_max` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '敵キャラ軟化スキル効果：持続ターン最大',
  `skill_week_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '敵キャラ軟化スキル効果：与ダメ倍率',
  `skill_change_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト変化スキル効果：有効無効',
  `skill_change_elem_prev` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト変化スキル効果：変化前コスト属性',
  `skill_change_elem_after` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト変化スキル効果：変化後コスト属性',
  `skill_ct_add_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'カウントダウン秒数増加スキル効果：有効無効',
  `skill_ct_add_second` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'カウントダウン秒数増加スキル効果：付加秒数',
  `skill_drain_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '吸血スキル効果：有効無効',
  `skill_drain_scale` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '吸血スキル効果：攻撃倍率',
  `skill_critical_odds` int(10) NOT NULL DEFAULT '0' COMMENT '発動率（％）',
  `skill_boost_name` varchar(255) NOT NULL DEFAULT '' COMMENT 'ブーストスキル表示名',
  `skill_boost_element` tinyint(2) unsigned NOT NULL DEFAULT '0' COMMENT 'ブーストスキル属性',
  `skill_boost_effect` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ブーストスキルエフェクト',
  `skill_boost_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ブーストスキル識別名',
  `skill_link_name` text NOT NULL COMMENT 'リンクスキル：表示名',
  `skill_link_detail` text NOT NULL COMMENT 'リンクスキル：詳細テキスト',
  `skill_link_odds` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクスキル：発動率',
  `ability` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：特性タイプ',
  `ability_effect` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：エフェクトタイプ',
  `ability_type` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：スキルタイプ',
  `ability_value` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：スキル効果値',
  `ability_value_rand` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：スキル効果値振れ幅',
  `ability_critical` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：クリティカル発動率',
  `param_00` int(10) NOT NULL DEFAULT '0' COMMENT '特性汎用パラメータ',
  `param_01` int(10) NOT NULL DEFAULT '0' COMMENT '特性汎用パラメータ',
  `param_02` int(10) NOT NULL DEFAULT '0' COMMENT '特性汎用パラメータ',
  `param_03` int(10) NOT NULL DEFAULT '0' COMMENT '特性汎用パラメータ',
  `param_04` int(10) NOT NULL DEFAULT '0' COMMENT '特性汎用パラメータ',
  `param_05` int(10) NOT NULL DEFAULT '0' COMMENT '特性汎用パラメータ',
  `param_06` int(10) NOT NULL DEFAULT '0' COMMENT '特性汎用パラメータ',
  `param_07` int(10) NOT NULL DEFAULT '0' COMMENT '特性汎用パラメータ',
  `hate_value` int(10) NOT NULL DEFAULT '0' COMMENT 'ヘイト増減値',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `skill_passive_master`
--

DROP TABLE IF EXISTS `skill_passive_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `skill_passive_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `name` varchar(64) NOT NULL DEFAULT '' COMMENT '名称和名',
  `detail` text NOT NULL COMMENT '詳細説明',
  `add_fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '追加パッシブfix_id',
  `skill_trap_pass_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '移動中トラップ無効効果：効果有無',
  `skill_trap_pass_type` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '移動中トラップ無効効果：トラップタイプ',
  `skill_powup_kind_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '敵種族別ステータス倍率効果：効果有無',
  `skill_powup_kind_type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '敵種族別ステータス倍率効果：種族',
  `skill_powup_kind_status` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '敵種族別ステータス倍率効果：変動ステータス',
  `skill_powup_kind_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '敵種族別ステータス倍率効果：倍率',
  `skill_counter_atk_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'カウンター効果：効果有無',
  `skill_counter_atk_element` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'カウンター効果：属性',
  `skill_counter_atk_odds` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'カウンター効果：発動確率',
  `skill_counter_atk_scale` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'カウンター効果：与ダメ倍率',
  `skill_counter_atk_effect` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'カウンター効果：エフェクトタイプ',
  `skill_damage_recovery_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '被ダメ時回復効果：効果有無',
  `skill_damage_recovery_odds` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '被ダメ時回復効果：発動確率',
  `skill_damage_recovery_rate` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '被ダメ時回復効果：回復割合',
  `skill_hp_full_powup_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '体力最大時攻撃力アップ効果：効果有無',
  `skill_hp_full_powup_scale` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '体力最大時攻撃力アップ効果：倍率',
  `skill_dying_powup_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '瀕死時攻撃力アップ効果：効果有無',
  `skill_dying_powup_border` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '瀕死時攻撃力アップ効果：体力しきい値',
  `skill_dying_powup_scale` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '瀕死時攻撃力アップ効果：倍率',
  `skill_backatk_pass_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パッシブスキル情報：バックアタック発生確率上書き：効果有無',
  `skill_backatk_pass_rate` tinyint(3) NOT NULL DEFAULT '0' COMMENT 'パッシブスキル情報：バックアタック発生確率上書き：確率',
  `skill_decline_dmg_elem_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パッシブスキル情報：属性ダメージ軽減：効果有無',
  `skill_decline_dmg_elem_elem` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パッシブスキル情報：属性ダメージ軽減：属性',
  `skill_decline_dmg_elem_rate` int(10) NOT NULL DEFAULT '0' COMMENT '属性ダメージ軽減：軽減倍率',
  `skill_decline_dmg_kind_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パッシブスキル情報：種族ダメージ軽減：効果有無',
  `skill_decline_dmg_kind_kind` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パッシブスキル情報：種族ダメージ軽減：種族',
  `skill_decline_dmg_kind_rate` int(10) NOT NULL DEFAULT '0' COMMENT '種族ダメージ軽減：軽減倍率',
  `skill_boost_chance_active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パッシブスキル情報：ブーストパネル抽選回数増加：効果有無',
  `skill_boost_chance_count` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'パッシブスキル情報：ブースとパネル抽選回数増加：回数',
  `skill_type` int(10) NOT NULL DEFAULT '0' COMMENT 'スキルタイプ',
  `skill_param_00` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_00',
  `skill_param_01` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_01',
  `skill_param_02` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_02',
  `skill_param_03` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_03',
  `skill_param_04` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_04',
  `skill_param_05` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_05',
  `skill_param_06` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_06',
  `skill_param_07` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_07',
  `skill_param_08` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_08',
  `skill_param_09` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_09',
  `skill_param_10` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_10',
  `skill_param_11` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_11',
  `skill_param_12` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_12',
  `skill_param_13` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_13',
  `skill_param_14` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_14',
  `skill_param_15` int(10) NOT NULL DEFAULT '0' COMMENT '汎用パラメータ_15',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `skill_limitbreak_master`
--

DROP TABLE IF EXISTS `skill_limitbreak_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `skill_limitbreak_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `name` varchar(64) NOT NULL DEFAULT '' COMMENT 'スキル名称',
  `detail` text NOT NULL COMMENT 'スキル詳細',
  `add_fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '追加リミブレfix_id',
  `use_turn` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '対価：ターン数',
  `use_sp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '対価：SP',
  `level_max` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキルレベル：最大',
  `level_up_rate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキルレベル：レベルアップ確率',
  `phase` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'フェーズ',
  `subject_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '条件タイプ',
  `subject_value` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '条件値',
  `skill_elem` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル属性',
  `skill_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキルタイプ',
  `skill_cate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル効果タイプ',
  `skill_effect` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキルエフェクト',
  `skill_damage_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'ダメージ有効無効',
  `skill_power` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル攻撃力（％）',
  `skill_power_fix` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル攻撃力（固定）',
  `skill_power_hp_rate` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル攻撃力（対象のHPの割合）',
  `skill_absorb` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル吸収力（％）',
  `skill_kickback` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル反動ダメージ（％）',
  `skill_kickback_fix` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル反動ダメージ（固定）',
  `skill_chk_atk_affinity` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃側：属性相性チェック',
  `skill_chk_atk_leader` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃側：リーダーチェック',
  `skill_chk_atk_passive` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃側：パッシブチェック',
  `skill_chk_atk_ailment` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃側：状態異常チェック',
  `skill_chk_def_defence` int(10) NOT NULL DEFAULT '0' COMMENT '防御側：防御力チェック',
  `skill_chk_def_ailment` int(10) NOT NULL DEFAULT '0' COMMENT '防御側：状態異常チェック',
  `skill_chk_def_barrier` int(10) NOT NULL DEFAULT '0' COMMENT '防御側：状態バリアチェック',
  `status_ailment_target` int(10) NOT NULL DEFAULT '0' COMMENT '状態異常効果対象',
  `status_ailment1` int(10) NOT NULL DEFAULT '0' COMMENT '状態異常01',
  `status_ailment2` int(10) NOT NULL DEFAULT '0' COMMENT '状態異常02',
  `status_ailment3` int(10) NOT NULL DEFAULT '0' COMMENT '状態異常03',
  `status_ailment4` int(10) NOT NULL DEFAULT '0' COMMENT '状態異常04',
  `value0` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value1` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value2` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value3` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value4` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value5` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value6` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value7` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value8` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value9` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value10` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value11` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value12` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value13` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value14` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `value15` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル汎用領域',
  `hate_value` int(10) NOT NULL DEFAULT '0' COMMENT 'ヘイト増減値',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `skill_boost_master`
--

DROP TABLE IF EXISTS `skill_boost_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `skill_boost_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `skill_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキルタイプ',
  `skill_cate` int(10) NOT NULL DEFAULT '0' COMMENT '基本情報：効果カテゴリ',
  `skill_damage_enable` int(1) unsigned NOT NULL DEFAULT '0' COMMENT 'ダメージ有効無効',
  `skill_power` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃情報：攻撃力（％）',
  `skill_power_fix` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃情報：攻撃力（固定）',
  `skill_power_hp_rate` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃情報：攻撃力（対象のHPの割合）',
  `skill_absorb` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃情報：吸収量（％）',
  `skill_chk_atk_affinity` smallint(3) NOT NULL DEFAULT '0' COMMENT '効果情報：攻撃側：属性相性チェック',
  `skill_chk_atk_leader` smallint(3) NOT NULL DEFAULT '0' COMMENT '効果情報：攻撃側：リーダーチェック',
  `skill_chk_atk_passive` smallint(3) NOT NULL DEFAULT '0' COMMENT '効果情報：攻撃側：パッシブチェック',
  `skill_chk_atk_ailment` smallint(3) NOT NULL DEFAULT '0' COMMENT '効果情報：攻撃側：状態変化チェック',
  `skill_chk_def_defence` smallint(3) NOT NULL DEFAULT '0' COMMENT '効果情報：防御側：防御力チェック',
  `skill_chk_def_ailment` smallint(3) NOT NULL DEFAULT '0' COMMENT '効果情報：防御側：状態変化チェック	',
  `skill_chk_def_barrier` smallint(3) NOT NULL DEFAULT '0' COMMENT '効果情報：防御側：状態バリアチェック',
  `status_ailment_target` int(10) NOT NULL DEFAULT '0' COMMENT '状態変化対象',
  `status_ailment_delay` int(10) NOT NULL DEFAULT '0' COMMENT '状態変化遅延',
  `status_ailment1` int(10) NOT NULL DEFAULT '0' COMMENT '状態変化01	',
  `status_ailment2` int(10) NOT NULL DEFAULT '0' COMMENT '状態変化02	',
  `status_ailment3` int(10) NOT NULL DEFAULT '0' COMMENT '状態変化03	',
  `status_ailment4` int(10) NOT NULL DEFAULT '0' COMMENT '状態変化04	',
  `skill_param_00` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ00',
  `skill_param_01` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ01',
  `skill_param_02` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ02',
  `skill_param_03` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ03',
  `skill_param_04` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ04',
  `skill_param_05` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ05',
  `skill_param_06` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ06',
  `skill_param_07` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ07',
  `skill_param_08` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ08',
  `skill_param_09` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ09',
  `skill_param_10` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ10',
  `skill_param_11` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ11',
  `skill_param_12` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ12',
  `skill_param_13` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ13',
  `skill_param_14` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ14',
  `skill_param_15` int(10) NOT NULL DEFAULT '0' COMMENT 'スキル情報：汎用パラメータ15',
  `hate_value` int(10) NOT NULL DEFAULT '0' COMMENT 'ヘイト増減値',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enemy_master`
--

DROP TABLE IF EXISTS `enemy_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enemy_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `chara_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラID',
  `status_hp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ステータス：体力',
  `status_pow` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ステータス：攻撃',
  `status_def` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ステータス：防御',
  `status_turn` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ステータス：ターン',
  `acquire_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '撃破時固定報酬：お金',
  `acquire_exp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '撃破時固定報酬：経験値',
  `drop_item_kind` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ種別',
  `drop_item_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップアイテムID',
  `drop_item_amount` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ数',
  `drop_item_rate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ確率',
  `drop_unit_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ：ユニット：ID',
  `drop_unit_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ：ユニット：レベル',
  `drop_unit_rate` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ：ユニット：確率',
  `drop_plus_pow` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ時プラス値確率：攻撃',
  `drop_plus_hp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ時プラス値確率：体力',
  `drop_plus_none` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ時プラス値確率：なし',
  `drop_money_value` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ：お金：金額',
  `drop_money_rate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ：お金：確率',
  `act_table1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：行動パターン１',
  `act_table2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：行動パターン２',
  `act_table3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：行動パターン３',
  `act_table4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：行動パターン４',
  `act_table5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：行動パターン５',
  `act_table6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：行動パターン６',
  `act_table7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：行動パターン７',
  `act_table8` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：行動パターン８',
  `act_first` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：初回行動',
  `act_dead` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：死亡行動',
  `ability1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：特性1',
  `ability2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：特性2',
  `ability3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：特性3',
  `ability4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：特性4',
  `ability5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：特性5',
  `ability6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：特性6',
  `ability7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：特性7',
  `ability8` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エネミーパラメータ：特性8',
  `posz_value` int(10) NOT NULL DEFAULT '0' COMMENT '描画優先度',
  `pos_absolute` int(10) NOT NULL DEFAULT '0' COMMENT 'メッシュ表示時の絶対座標フラグ',
  `posx_offset` int(10) NOT NULL DEFAULT '0' COMMENT 'メッシュ表示時の位置オフセットX',
  `posy_offset` int(10) NOT NULL DEFAULT '0' COMMENT 'メッシュ表示時の位置オフセットY',
  `hp_gauge_type` int(10) NOT NULL DEFAULT '0' COMMENT 'HPゲージの種類',
  `hp_posx_offset` int(10) NOT NULL DEFAULT '0' COMMENT 'HPゲージの位置オフセットX',
  `hp_posy_offset` int(10) NOT NULL DEFAULT '0' COMMENT 'HPゲージの位置オフセットY',
  `target_pos_absolute` int(10) NOT NULL DEFAULT '0' COMMENT 'ターゲットカーソル位置設定有効',
  `target_posx_offset` int(10) NOT NULL DEFAULT '0' COMMENT 'ターゲットカーソル位置オフセットX',
  `target_posy_offset` int(10) NOT NULL DEFAULT '0' COMMENT 'ターゲットカーソル位置オフセットY',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enemy_group_master`
--

DROP TABLE IF EXISTS `enemy_group_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enemy_group_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `fix` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '完全固定フラグ',
  `num_min` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ数下限',
  `num_max` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ数上限',
  `enemy_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_8` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_9` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_10` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_11` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_12` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_13` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_14` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_15` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_16` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_17` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_18` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_19` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `enemy_id_20` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '選択肢キャラ',
  `chain_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '連鎖するエネミーグループ',
  `chain_turn_offset` int(10) NOT NULL DEFAULT '0' COMMENT '連鎖した際のターンオフセット',
  `drop_type` int(10) NOT NULL DEFAULT '0' COMMENT 'ドロップタイプ',
  `evol_direct_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '進化演出タイプ',
  `hate_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ヘイト管理ID',
  `bgm_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'BGMID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `guerrilla_boss_master`
--

DROP TABLE IF EXISTS `guerrilla_boss_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `guerrilla_boss_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベント開始タイミング',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベント終了タイミング',
  `user_group` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユーザーグループ',
  `enemy_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '敵キャラID',
  `enemy_rate` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '敵キャラ出現確率',
  `quest_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現クエストID',
  `quest_id_must` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現 条件クエストID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_timing_end` (`timing_end`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enemy_action_table_master`
--

DROP TABLE IF EXISTS `enemy_action_table_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enemy_action_table_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `change_msg` text NOT NULL COMMENT '起動タイミング表示メッセージ',
  `change_action_id` int(10) NOT NULL DEFAULT '0' COMMENT '起動タイミングアクション定義ID',
  `action_select_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション選択タイプ(巡回、ループ)',
  `action_param_id1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション定義IDパターン',
  `action_param_id2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション定義IDパターン',
  `action_param_id3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション定義IDパターン',
  `action_param_id4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション定義IDパターン',
  `action_param_id5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション定義IDパターン',
  `action_param_id6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション定義IDパターン',
  `action_param_id7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション定義IDパターン',
  `action_param_id8` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アクション定義IDパターン',
  `timing_priority` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '判定優先度',
  `timing_type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミング(初回、HP割合)',
  `timing_param1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミングパラメータ',
  `timing_param2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミングパラメータ',
  `timing_param3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミングパラメータ',
  `timing_param4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミングパラメータ',
  `timing_param5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミングパラメータ',
  `timing_param6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミングパラメータ',
  `timing_param7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミングパラメータ',
  `timing_param8` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '起動タイミングパラメータ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enemy_action_param_master`
--

DROP TABLE IF EXISTS `enemy_action_param_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enemy_action_param_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `skill_name` text NOT NULL COMMENT '表示用のテキスト',
  `add_fix_id` int(10) NOT NULL DEFAULT '0' COMMENT '追加行動',
  `attack_target` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃対象',
  `se_id` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'SEID(ラベルID)',
  `effect_id` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'エフェクトID(名前?タイプ?)',
  `attack_motion` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '攻撃モーション(ON/OFF)',
  `shake_screen` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '画面揺れ(ON/OFF)',
  `damage_effect` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ダメージエフェクト(ON/OFF)',
  `damage_draw` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ダメージ数値(ON/OFF)',
  `wait_time` int(10) NOT NULL DEFAULT '0' COMMENT '待機時間[msecs] 0:デフォ待機時間',
  `status_ailment_target` int(10) NOT NULL DEFAULT '0' COMMENT '状態変化ターゲット',
  `status_ailment1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態変化１',
  `status_ailment2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態変化２',
  `status_ailment3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態変化３',
  `status_ailment4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態変化４',
  `skill_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキルタイプ',
  `skill_param1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param8` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param9` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param10` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param11` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param12` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param13` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param14` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param15` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `skill_param16` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スキル汎用パラメータ',
  `audio_data_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボイスID',
  `attack_target_num` int(10) NOT NULL DEFAULT '0' COMMENT '攻撃対象数',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `status_ailment_master`
--

DROP TABLE IF EXISTS `status_ailment_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `status_ailment_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `good_or_bad` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '良い、悪い',
  `clear_type` int(10) NOT NULL DEFAULT '0' COMMENT 'クリアできる種類',
  `category` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の種類',
  `duration` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '継続ターン数',
  `icon` text NOT NULL COMMENT '状態アイコンリソース名',
  `name` text NOT NULL COMMENT '名前',
  `detail` text NOT NULL COMMENT '詳細テキスト',
  `update_move` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '移動中ターン経過',
  `update_battle` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '戦闘中ターン経過',
  `param01` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param02` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param03` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param04` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param05` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param06` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param07` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param08` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param09` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param10` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param11` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `param12` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '状態異常の汎用パラメータ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `area_master`
--

DROP TABLE IF EXISTS `area_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `area_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `view_id` int(10) NOT NULL DEFAULT '0' COMMENT '表示順番ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `area_name` varchar(64) NOT NULL DEFAULT '' COMMENT 'エリア名称',
  `area_name_eng` varchar(64) NOT NULL DEFAULT '' COMMENT 'エリア名称英字名',
  `area_detail` text NOT NULL COMMENT 'エリア詳細',
  `area_cate_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エリアカテゴリ',
  `questlist_sort` int(10) NOT NULL DEFAULT '0' COMMENT 'クエスト図鑑ソート順',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '公開条件イベント',
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'エリアタイプ',
  `pre_area` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア必須エリア',
  `cost0` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト枚数分布：無',
  `cost1` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト枚数分布：火',
  `cost2` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト枚数分布：水',
  `cost3` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト枚数分布：風',
  `cost4` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト枚数分布：光',
  `cost5` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト枚数分布：闇',
  `cost6` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'コスト枚数分布：回復',
  `area_element` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '属性',
  `area_effect` text NOT NULL COMMENT 'エリアエフェクト名',
  `mesh_map` text NOT NULL COMMENT 'リソース：メッシュ名：マップ',
  `mesh_door` text NOT NULL COMMENT 'リソース：メッシュ名：扉',
  `mesh_door_boss` text NOT NULL COMMENT 'リソース：メッシュ名：扉(ボス)',
  `res_map` text NOT NULL COMMENT 'リソース：テクスチャ名：マップ',
  `res_map_icon` text NOT NULL COMMENT 'リソース：テクスチャ名：マップアイコン',
  `res_icon_key` text NOT NULL COMMENT 'リソース：テクスチャ名：パネルアイコン：鍵',
  `res_icon_box` text NOT NULL COMMENT 'リソース：テクスチャ名：パネルアイコン：宝箱',
  `packname_se` text NOT NULL COMMENT 'サウンドパック：SE',
  `packname_bgm` text NOT NULL COMMENT 'サウンドパック：BGM',
  `area_url` text NOT NULL COMMENT 'イベント概要URL',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quest_master`
--

DROP TABLE IF EXISTS `quest_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quest_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `area_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エリア通し番号',
  `active` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `quest_name` text NOT NULL COMMENT 'クエスト名称',
  `difficulty_name` text NOT NULL COMMENT '難易度名称',
  `quest_stamina` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'クエスト消費スタミナ',
  `quest_ticket` smallint(5) NOT NULL DEFAULT '0' COMMENT 'クエスト受諾に必要なチケット枚数',
  `quest_key` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエスト消費キー数',
  `quest_floor_bonus_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'フロアボーナス報酬タイプ',
  `clear_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：お金',
  `clear_exp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：経験値',
  `clear_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'クリア報酬：リンクポイント',
  `clear_stone` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：魔法石（初回限定）',
  `item_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '宝箱基準額',
  `limit_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'チート対策上限：お金',
  `limit_exp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'チート対策上限：経験値',
  `story` text NOT NULL COMMENT 'クエストごとのストーリー',
  `clear_unit` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：ユニットID',
  `clear_unit_lv` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：ユニットレベル',
  `clear_unit_msg` text NOT NULL COMMENT 'クリア報酬：ユニット取得時文言',
  `clear_item` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：アイテム',
  `clear_item_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：アイテム個数',
  `clear_item_msg` text COMMENT 'クリア報酬：アイテム取得時文言',
  `clear_key` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：クエストキー',
  `clear_key_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：クエストキー個数',
  `clear_key_msg` text COMMENT 'クリア報酬：クエストキー取得時文言',
  `once` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '一度クリアしたら二度と出現しないクエスト',
  `battle_chain` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '先頭連鎖が発生するフロアか',
  `quest_requirement_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエスト入場条件',
  `quest_requirement_text` text NOT NULL COMMENT 'クエスト入場条件テキスト',
  `enable_continue` int(10) NOT NULL DEFAULT '0' COMMENT 'コンティニュー有無',
  `enable_retry` int(10) NOT NULL DEFAULT '0' COMMENT 'リトライ有無',
  `enable_friendpoint` int(10) NOT NULL DEFAULT '0' COMMENT 'フレンドポイント有無',
  `voice_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボイスグループID',
  `packname_voice` text NOT NULL COMMENT 'サウンドパック：VOICE',
  `movie_name` text NOT NULL COMMENT 'ムービー：リソース名',
  `boss_icon_hide` int(10) NOT NULL DEFAULT '0' COMMENT 'ボスアイコンを隠すフラグ',
  `boss_icon_unit` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボスアイコン情報',
  `boss_ability_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_1',
  `boss_ability_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_2',
  `boss_ability_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_3',
  `boss_ability_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_4',
  `boss_chara_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現ボスのキャラID',
  `e_chara_id_0` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_1',
  `e_chara_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_2',
  `e_chara_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_3',
  `e_chara_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_4',
  `e_chara_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_5',
  `floor_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quest_floor_master`
--

DROP TABLE IF EXISTS `quest_floor_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quest_floor_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `quest_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエスト通し番号',
  `under` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '地下フラグ',
  `boss_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボスエネミーグループ番号',
  `evol_direct_type` int(10) NOT NULL DEFAULT '0' COMMENT '進化演出タイプ',
  `pattern_expect` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '期待値パターン',
  `pattern_category` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'カテゴリパターン',
  `question_ct` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '「？」の出現数',
  `ticket_rate` smallint(5) NOT NULL DEFAULT '0' COMMENT 'チケット置き換え：確率',
  `ticket_effective` int(10) NOT NULL DEFAULT '0' COMMENT 'チケット置き換え：パネル効果定義FixID',
  `enemy_group_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現エネミーグループ：☆１',
  `enemy_group_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現エネミーグループ：☆２',
  `enemy_group_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現エネミーグループ：☆３',
  `enemy_group_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現エネミーグループ：☆４',
  `enemy_group_id_5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現エネミーグループ：☆５',
  `enemy_group_id_6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現エネミーグループ：☆６',
  `enemy_group_id_7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現エネミーグループ：☆７',
  `trap_group_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現トラップグループ：☆１',
  `trap_group_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現トラップグループ：☆２',
  `trap_group_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現トラップグループ：☆３',
  `trap_group_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現トラップグループ：☆４',
  `trap_group_id_5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現トラップグループ：☆５',
  `trap_group_id_6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現トラップグループ：☆６',
  `trap_group_id_7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現トラップグループ：☆７',
  `item_group_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現アイテムグループ：☆１',
  `item_group_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現アイテムグループ：☆２',
  `item_group_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現アイテムグループ：☆３',
  `item_group_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現アイテムグループ：☆４',
  `item_group_id_5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現アイテムグループ：☆５',
  `item_group_id_6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現アイテムグループ：☆６',
  `item_group_id_7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現アイテムグループ：☆７',
  `heal_group_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現回復グループ：☆１',
  `heal_group_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現回復グループ：☆２',
  `heal_group_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現回復グループ：☆３',
  `heal_group_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現回復グループ：☆４',
  `heal_group_id_5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現回復グループ：☆５',
  `heal_group_id_6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現回復グループ：☆６',
  `heal_group_id_7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現回復グループ：☆７',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `category_pattern_master`
--

DROP TABLE IF EXISTS `category_pattern_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `category_pattern_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `cate0` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '出現割合：空',
  `cate1` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '出現割合：敵キャラ',
  `cate2` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '出現割合：アイテム',
  `cate3` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '出現割合：トラップ',
  `cate4` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '出現割合：SP回復',
  `cate5` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '出現割合：鍵',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `expect_pattern_master`
--

DROP TABLE IF EXISTS `expect_pattern_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `expect_pattern_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `level1` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '期待値固定数：☆１',
  `level2` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '期待値固定数：☆２',
  `level3` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '期待値固定数：☆３',
  `level4` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '期待値固定数：☆４',
  `level5` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '期待値固定数：☆５',
  `level6` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '期待値固定数：☆６',
  `level7` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '期待値固定数：☆７',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `panel_master`
--

DROP TABLE IF EXISTS `panel_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `panel_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `name` text NOT NULL COMMENT '名前',
  `detail` text NOT NULL COMMENT '詳細テキスト',
  `res_panel` text NOT NULL COMMENT 'リソース名称：アイコン',
  `effect_type` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'エフェクトタイプ',
  `effective_type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '効果タイプ',
  `effective_value` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '効果値',
  `ailment_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パネル効果パラメータ：状態異常ID',
  `trap_type` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'トラップタイプ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `panel_group_master`
--

DROP TABLE IF EXISTS `panel_group_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `panel_group_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `panel_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パネル分岐選択肢',
  `panel_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パネル分岐選択肢',
  `panel_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パネル分岐選択肢',
  `panel_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パネル分岐選択肢',
  `panel_id_5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パネル分岐選択肢',
  `panel_id_6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パネル分岐選択肢',
  `panel_id_7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パネル分岐選択肢',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quest_requirement`
--

DROP TABLE IF EXISTS `quest_requirement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quest_requirement` (
  `fix_id` int(10) NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `elem_fire` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '属性：炎',
  `elem_water` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '属性：水',
  `elem_wind` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '属性：風',
  `elem_light` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '属性：光',
  `elem_dark` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '属性：闇',
  `elem_naught` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '属性：無',
  `kind_human` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '種族：人間',
  `kind_fairy` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '種族：妖精',
  `kind_demon` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '種族：悪魔',
  `kind_dragon` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '種族：竜',
  `kind_machine` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '種族：機械',
  `kind_beast` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '種族：獣',
  `kind_god` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '種族：神',
  `kind_egg` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '種族：強化合成',
  `num_elem` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '属性数',
  `num_kind` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '種族数',
  `num_unit` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット数',
  `much_name` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '同名禁止',
  `require_unit_00` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '必須ユニットID00',
  `require_unit_01` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '必須ユニットID01',
  `require_unit_02` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '必須ユニットID02',
  `require_unit_03` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '必須ユニットID03',
  `require_unit_04` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '必須ユニットID04',
  `limit_rare` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '制限レア度',
  `limit_cost` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '制限コスト',
  `limit_cost_total` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '制限コスト合計',
  `limit_unit_lv` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '制限ユニットレベル',
  `limit_unit_lv_total` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '制限ユニットレベル合計',
  `limit_rank` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '制限ランク',
  `rule_disable_as` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'AS禁止',
  `rule_disable_ls` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'LS禁止',
  `rule_heal_half` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'NS半減',
  `rule_disable_affinity` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '属性相性無し',
  `fix_unit_00_enable` int(10) NOT NULL DEFAULT '0' COMMENT '有効無効フラグ',
  `fix_unit_00_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID',
  `fix_unit_00_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'レベル',
  `fix_unit_00_lv_lbs` int(10) NOT NULL DEFAULT '0' COMMENT 'スキルレベル',
  `fix_unit_00_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破レベル',
  `fix_unit_00_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：HP',
  `fix_unit_00_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：ATK',
  `fix_unit_00_link_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：有効無効フラグ',
  `fix_unit_00_link_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットID',
  `fix_unit_00_link_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットレベル',
  `fix_unit_00_link_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：限界突破レベル',
  `fix_unit_00_link_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：HP',
  `fix_unit_00_link_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：ATK',
  `fix_unit_00_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ポイント     ',
  `fix_unit_01_enable` int(10) NOT NULL DEFAULT '0' COMMENT '有効無効フラグ',
  `fix_unit_01_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID',
  `fix_unit_01_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'レベル',
  `fix_unit_01_lv_lbs` int(10) NOT NULL DEFAULT '0' COMMENT 'スキルレベル',
  `fix_unit_01_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破レベル',
  `fix_unit_01_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：HP',
  `fix_unit_01_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：ATK',
  `fix_unit_01_link_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：有効無効フラグ',
  `fix_unit_01_link_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットID',
  `fix_unit_01_link_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットレベル',
  `fix_unit_01_link_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：限界突破レベル',
  `fix_unit_01_link_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：HP',
  `fix_unit_01_link_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：ATK',
  `fix_unit_01_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ポイント     ',
  `fix_unit_02_enable` int(10) NOT NULL DEFAULT '0' COMMENT '有効無効フラグ',
  `fix_unit_02_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID',
  `fix_unit_02_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'レベル',
  `fix_unit_02_lv_lbs` int(10) NOT NULL DEFAULT '0' COMMENT 'スキルレベル',
  `fix_unit_02_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破レベル',
  `fix_unit_02_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：HP',
  `fix_unit_02_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：ATK',
  `fix_unit_02_link_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：有効無効フラグ',
  `fix_unit_02_link_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットID',
  `fix_unit_02_link_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットレベル',
  `fix_unit_02_link_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：限界突破レベル',
  `fix_unit_02_link_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：HP',
  `fix_unit_02_link_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：ATK',
  `fix_unit_02_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ポイント     ',
  `fix_unit_03_enable` int(10) NOT NULL DEFAULT '0' COMMENT '有効無効フラグ',
  `fix_unit_03_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID',
  `fix_unit_03_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'レベル',
  `fix_unit_03_lv_lbs` int(10) NOT NULL DEFAULT '0' COMMENT 'スキルレベル',
  `fix_unit_03_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破レベル',
  `fix_unit_03_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：HP',
  `fix_unit_03_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：ATK',
  `fix_unit_03_link_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：有効無効フラグ',
  `fix_unit_03_link_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットID',
  `fix_unit_03_link_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットレベル',
  `fix_unit_03_link_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：限界突破レベル',
  `fix_unit_03_link_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：HP',
  `fix_unit_03_link_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：ATK',
  `fix_unit_03_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ポイント     ',
  `fix_unit_04_enable` int(10) NOT NULL DEFAULT '0' COMMENT '有効無効フラグ',
  `fix_unit_04_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニットID',
  `fix_unit_04_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'レベル',
  `fix_unit_04_lv_lbs` int(10) NOT NULL DEFAULT '0' COMMENT 'スキルレベル',
  `fix_unit_04_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破レベル',
  `fix_unit_04_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：HP',
  `fix_unit_04_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'プラス値：ATK',
  `fix_unit_04_link_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：有効無効フラグ',
  `fix_unit_04_link_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットID',
  `fix_unit_04_link_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ユニットレベル',
  `fix_unit_04_link_lv_lo` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク：限界突破レベル',
  `fix_unit_04_link_plus_hp` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：HP',
  `fix_unit_04_link_plus_atk` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:プラス値：ATK',
  `fix_unit_04_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'リンク:ポイント     ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gacha_master`
--

DROP TABLE IF EXISTS `gacha_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gacha_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャタイミング：開始',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャタイミング：終了',
  `name` text NOT NULL COMMENT 'ガチャ名称',
  `detail` text NOT NULL COMMENT 'ガチャ詳細',
  `detail_2` text NOT NULL COMMENT 'ガチャ詳細(統合表示用)',
  `priority` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '優先度',
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャタイプ',
  `price` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '対価',
  `cost_item_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '消費アイテムID',
  `view_count` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャを引ける回数',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現条件イベントID',
  `hint` text NOT NULL COMMENT 'ガチャアイコン上部のヒント文言',
  `url_img` text NOT NULL COMMENT '宣伝用画像URL',
  `debug_url_img` text NOT NULL COMMENT '宣伝用画像URL(デバッグ用)',
  `url_web` text NOT NULL COMMENT '外部詳細ページURL',
  `priority_show` int(10) NOT NULL DEFAULT '0' COMMENT '表示優先度',
  `tab_num` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャタブ番号',
  `tab_name` text NOT NULL COMMENT 'ガチャタブ名称',
  `rainbow_decide` int(10) NOT NULL DEFAULT '0' COMMENT '虹確定イベント',
  `change_title` text NOT NULL COMMENT 'ガチャ表示名称変更',
  `change_odds` int(10) NOT NULL DEFAULT '0' COMMENT '名称変更確率',
  `caption_subtext` text NOT NULL COMMENT 'キャプションサブテキスト',
  `notification_title` text NOT NULL COMMENT '通知文言：タイトル',
  `notification_body` text NOT NULL COMMENT '通知文言：本文',
  `not_have_priority` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '未所持ユニット優先排出',
  `box_output` int(10) NOT NULL DEFAULT '0' COMMENT 'BOXガチャフラグ',
  `event_point_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ポイント定義名',
  `grossing_present_enable` int(10) NOT NULL DEFAULT '0' COMMENT '総付けプレゼント情報：有効化フラグ',
  `grossing_present_id` int(10) NOT NULL DEFAULT '0' COMMENT '総付けプレゼント情報：ID',
  `grossing_present_detail` text NOT NULL COMMENT '総付けプレゼント情報：説明文',
  `present_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャプレゼント情報：有効化フラグ',
  `present_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャプレゼント情報：付与プレゼント',
  `present_message` text COMMENT 'ガチャプレゼント情報：付与プレゼントメッセージ',
  `present_count` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャプレゼント情報：付与回数',
  `present_gacha_count` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャプレゼント情報：付与条件：連続引き回数',
  `first_time_free_enable` int(10) NOT NULL DEFAULT '0' COMMENT '初回無料ガチャ：有効化フラグ',
  `paid_tip_only` int(10) NOT NULL DEFAULT '0' COMMENT '有料チップ限定ガチャフラグ',
  `reset_type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'リセットタイプ',
  `reset_month` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リセット：月',
  `reset_day` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リセット：日',
  `reset_time` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一日一回ガチャリセット時間',
  `reset_week` smallint(6) unsigned NOT NULL DEFAULT '0' COMMENT '曜日指定',
  `gacha_group_id` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャグループマスタID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gacha_assign_master`
--

DROP TABLE IF EXISTS `gacha_assign_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gacha_assign_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `gacha_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャID',
  `assign_chara_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'アサインキャラ情報：出現キャラ',
  `assign_rate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アサインキャラ情報：出現確率',
  `assign_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'アサインキャラ情報：出現レベル',
  `plus_pow` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プラス値確率：攻撃',
  `plus_hp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プラス値確率：体力',
  `plus_none` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プラス値確率：なし',
  `limitover_lv` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '限界突破',
  `box_stock` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'BOXガチャ在庫数',
  `limit_icon` tinyint(3) NOT NULL DEFAULT '0' COMMENT 'ラインナップ画面 限定表記の有無設定',
  `assign_rate_up` tinyint(3) NOT NULL DEFAULT '0' COMMENT '超絶UP設定 0: なし(1倍), 1: 1.5倍, 2：2倍, 3: 3倍, 4: 4倍, 5: 5倍, 6: 6倍, 7: 7倍, 8: 8倍, 9: 9倍, 10: 10倍,',
  `lineup_sort_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ラインナップ画面 並び順指定',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gacha_group_master`
--

DROP TABLE IF EXISTS `gacha_group_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gacha_group_master` (
  `fix_id` int(10) unsigned NOT NULL COMMENT 'ガチャグループID',
  `timing_public` int(10) NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `sale_period_start` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャイベント全体の販売期間を設定（開始日時）',
  `sale_period_end` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャイベント全体の販売期間を設定（終了日時）',
  `event_detail` text NOT NULL COMMENT '販売期間全体で使用する詳細説明',
  `guideline_text_key` text COMMENT 'ガチャガイドライン使用テキストキー',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `login_total_master`
--

DROP TABLE IF EXISTS `login_total_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `login_total_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `login_ct` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ログイン回数',
  `message` text NOT NULL COMMENT '表示テキスト',
  `acquire_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：お金',
  `acquire_fp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：フレンドポイント',
  `acquire_stone` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：魔法石',
  `acquire_unit_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：ユニット',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `login_chain_master`
--

DROP TABLE IF EXISTS `login_chain_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `login_chain_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `login_ct` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ログイン回数',
  `message` text NOT NULL COMMENT '表示テキスト',
  `acquire_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：お金',
  `acquire_fp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：フレンドポイント',
  `acquire_stone` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：魔法石',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `login_event_master`
--

DROP TABLE IF EXISTS `login_event_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `login_event_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `timing` int(10) NOT NULL DEFAULT '0',
  `message` text NOT NULL COMMENT '表示テキスト',
  `acquire_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：お金',
  `acquire_fp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：フレンドポイント',
  `acquire_stone` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：魔法石',
  `acquire_unit_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼント：ユニット',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_master`
--

DROP TABLE IF EXISTS `event_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `period_type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '期間指定タイプ',
  `cycle_date_type` smallint(6) unsigned NOT NULL DEFAULT '0' COMMENT 'サイクル：開催日指定：開始',
  `cycle_timing_start` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'サイクル：イベント開始時間',
  `cycle_active_hour` smallint(6) unsigned NOT NULL DEFAULT '0' COMMENT 'サイクル：イベント有効時間',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベント開始タイミング',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベント終了タイミング',
  `receiving_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '達成アチーブメント受取り期限',
  `user_group` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ユーザーグループ',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントラベル',
  `notification_title` text NOT NULL COMMENT '通知文言：タイトル',
  `notification_body` text NOT NULL COMMENT '通知文言：本文',
  `notification_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '通知文言：タイプ',
  `event_schedule_show` int(10) NOT NULL DEFAULT '0' COMMENT 'イベントスケジュール表示有無',
  `work_note` text NOT NULL COMMENT '作業メモ',
  `area_id` int(10) DEFAULT '-1' COMMENT 'クエストエリアID指定',
  `head_titel_1` varchar(255) NOT NULL COMMENT '項目タイトル１（出現ユニット）',
  `head_titel_2` varchar(255) NOT NULL COMMENT '項目タイトル２（イベント概要）',
  `head_text` text NOT NULL COMMENT '概要テキスト',
  `banner_img_url` text NOT NULL COMMENT 'イベント詳細用バナー画像URL',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `area_amend_master`
--

DROP TABLE IF EXISTS `area_amend_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `area_amend_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `timing_start` int(10) NOT NULL DEFAULT '0' COMMENT 'イベント開始タイミング',
  `timing_end` int(10) NOT NULL DEFAULT '0' COMMENT 'イベント終了タイミング',
  `user_group` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ユーザーグループ',
  `area_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '補正対象エリア番号',
  `amend` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '補正タイプ',
  `amend_value` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '補正値',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_timing_end` (`timing_end`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `present_master`
--

DROP TABLE IF EXISTS `present_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `present_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `present_type` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼントタイプ',
  `present_param1` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼントパラメータ１',
  `present_param2` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼントパラメータ２',
  `present_param3` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼントパラメータ３',
  `present_param4` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼントパラメータ４',
  `present_param5` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼントパラメータ５',
  `present_param6` int(10) NOT NULL DEFAULT '0' COMMENT 'スキルレベルアップ',
  `present_param7` int(10) NOT NULL DEFAULT '0' COMMENT 'バッファ',
  `present_param8` int(10) NOT NULL DEFAULT '0' COMMENT 'バッファ',
  `present_param9` int(10) NOT NULL DEFAULT '0' COMMENT 'バッファ',
  `present_param10` int(10) NOT NULL DEFAULT '0' COMMENT 'バッファ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_fix_id` (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `achievement_master`
--

DROP TABLE IF EXISTS `achievement_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `achievement_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `draw_id` text NOT NULL COMMENT 'アチーブメント名',
  `category` int(10) NOT NULL DEFAULT '0' COMMENT 'カテゴリ',
  `achievement_group` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントグループ',
  `draw_msg` text NOT NULL COMMENT 'アチーブメント名',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現条件イベントID',
  `timing_start` int(10) NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `achievement_category_id` tinyint(3) unsigned NOT NULL DEFAULT '1' COMMENT 'アチーブメントカテゴリID(1: ノーマル、2: イベント、3: デイリー、4: クエスト)',
  `achievement_type` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ',
  `achievement_param_1` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントパラメータ１',
  `achievement_param_2` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `achievement_param_3` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `achievement_param_4` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `achievement_param_5` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `achievement_param_6` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `achievement_param_7` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `achievement_param_8` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `achievement_param_9` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `achievement_param_10` int(10) NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ処理用の汎用パラメータ',
  `present_group_id` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼントグループ',
  `open_type` int(10) NOT NULL DEFAULT '0' COMMENT '解放条件タイプ',
  `open_param1` int(10) NOT NULL DEFAULT '0' COMMENT '解放条件パラメータ１',
  `show_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '表示条件タイプ',
  `show_param1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '表示条件パラメータ１',
  `quest_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '移動先クエストID',
  `check_evol_unit_flag` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット条件の進化前後許容フラグ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_achievement_group` (`achievement_group`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `present_group_master`
--

DROP TABLE IF EXISTS `present_group_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `present_group_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `active` int(10) NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `group_id` int(10) NOT NULL DEFAULT '0' COMMENT 'グループID',
  `present_id` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼントID',
  `present_num` int(10) NOT NULL DEFAULT '0' COMMENT 'プレゼント個数',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_fix_id` (`fix_id`),
  KEY `idx_group_id` (`group_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `store_product_master`
--

DROP TABLE IF EXISTS `store_product_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `store_product_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有固定ID',
  `name` varchar(64) NOT NULL DEFAULT '' COMMENT '商品和名',
  `platform` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '商品販売プラットフォーム',
  `id` varchar(64) NOT NULL DEFAULT '' COMMENT '商品ID',
  `price` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '商品価格',
  `point` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '商品内包個数',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `asset_bundle_path_master`
--

DROP TABLE IF EXISTS `asset_bundle_path_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `asset_bundle_path_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有固定ID',
  `path_type` int(10) NOT NULL DEFAULT '0' COMMENT '置き場所タイプ',
  `category` int(10) NOT NULL DEFAULT '0' COMMENT 'カテゴリ',
  `name` varchar(64) NOT NULL DEFAULT '' COMMENT 'AssetBundle名称',
  `version` int(10) NOT NULL DEFAULT '0' COMMENT 'AssetBundle更新番号',
  `title_dl` tinyint(3) NOT NULL DEFAULT '0' COMMENT '強制ダウンロードフラグ',
  `memo` text NOT NULL COMMENT 'memo',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `information_master`
--

DROP TABLE IF EXISTS `information_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `information_master` (
  `fix_id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '情報固有固定ID',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '1' COMMENT 'データ使用フラグ 0:NONE, 1:無効, 2:有効',
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'お知らせタイプ 0: 通常, 1: 緊急',
  `priority` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '優先度 通常のお知らせのみで有効 小さいほうが優先',
  `platform` tinyint(3) unsigned NOT NULL DEFAULT '3' COMMENT 'お知らせ対象OS 0:iOS, 1:And, 2:Win, 3:all',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントタイミング：開始',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントタイミング：終了',
  `message` text NOT NULL COMMENT '表示メッセージ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB AUTO_INCREMENT=62 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `store_product_event_master`
--

DROP TABLE IF EXISTS `store_product_event_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `store_product_event_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベント開始タイミング',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベント終了タイミング',
  `item_before` text NOT NULL COMMENT '切り替わる対象のアイテム：変化前',
  `item_after` text NOT NULL COMMENT '切り替わる対象のアイテム：変化後',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントID',
  `add_chip` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '加算チップ枚数',
  `event_chip_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントでのチップ購入回数',
  `platform` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'プラットフォーム',
  `count_draw` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'カウントダウン表示有無',
  `kind` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントタイプ',
  `event_text` text NOT NULL COMMENT 'イベント文言',
  `timing_st_m` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ストアイベントの分単位指定：開始時刻 分',
  `timing_ed_m` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ストアイベントの分単位指定：終了時刻 分',
  `present_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購入時付与プレゼント',
  `present_message` text NOT NULL COMMENT '購入者付与プレゼントメッセージ',
  `event_caption_text` text NOT NULL COMMENT 'イベント文言',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_event_id` (`event_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `login_monthly_master`
--

DROP TABLE IF EXISTS `login_monthly_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `login_monthly_master` (
  `fix_id` int(10) unsigned NOT NULL,
  `active` int(11) NOT NULL,
  `date` int(10) unsigned NOT NULL,
  `receive_limit` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '受取期限（0: 受け取り日時から30日、1: 無期限、日時指定も可）',
  `present_group_id` int(10) unsigned NOT NULL,
  `message` text NOT NULL COMMENT 'ダイアログメッセージ',
  `notification1_timing` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プッシュ通知1：時間',
  `notification1_title` text NOT NULL COMMENT 'プッシュ通知1：タイトル',
  `notification1_body` text NOT NULL COMMENT 'プッシュ通知1：メッセージ',
  `notification2_timing` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プッシュ通知2：時間',
  `notification2_title` text NOT NULL COMMENT 'プッシュ通知2：タイトル',
  `notification2_body` text NOT NULL COMMENT 'プッシュ通知2：メッセージ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_group_id` (`present_group_id`),
  KEY `idx_date_and_active` (`date`,`active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `beginner_boost_master`
--

DROP TABLE IF EXISTS `beginner_boost_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `beginner_boost_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '公開条件イベント',
  `rank_min` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '適用ランク下限（以上）',
  `rank_max` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '適用ランク下限（以下）',
  `boost_exp` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ボーナス倍率：クエストの経験値倍率（％）',
  `boost_money` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ボーナス倍率：クエストの取得金額倍率（％）',
  `boost_stamina_use` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ボーナス倍率：クエストのスタミナ消費倍率（％）',
  `boost_build_money` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ボーナス倍率：合成の消費金額倍率（％）',
  `boost_build_great` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'ボーナス倍率：合成の大成功発生確率倍率（％）',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `area_category_master`
--

DROP TABLE IF EXISTS `area_category_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `area_category_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `area_cate_name` text NOT NULL COMMENT 'エリアカテゴリ名称',
  `area_cate_type` int(10) NOT NULL DEFAULT '0' COMMENT 'エリアカテゴリタイプ',
  `quest_tab_type` int(10) NOT NULL DEFAULT '0' COMMENT 'クエストタブタイプ',
  `questlist_sort` int(10) NOT NULL DEFAULT '0' COMMENT 'クエスト図鑑表示順',
  `region_id` int(10) NOT NULL DEFAULT '0' COMMENT 'リージョンID',
  `btn_posx_offset` int(10) NOT NULL DEFAULT '0' COMMENT 'ボタン表示位置 x',
  `btn_posy_offset` int(10) NOT NULL DEFAULT '0' COMMENT 'ボタン表示位置 y',
  `background` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エリア背景',
  `area_cate_detail` text NOT NULL COMMENT '備考',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `invitation_master`
--

DROP TABLE IF EXISTS `invitation_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `invitation_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有固定ID',
  `enable` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '有効フラグ',
  `send_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '招待元のポストグループID',
  `recv_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '招待先のポストグループID',
  `receive_max` int(10) NOT NULL DEFAULT '0' COMMENT '招待報酬受け取り数上限',
  `title` text NOT NULL COMMENT 'ダイアログタイトル',
  `message` text NOT NULL COMMENT 'ダイアログメッセージ',
  `send_present_message` text NOT NULL COMMENT '招待元プレゼントメッセージ',
  `recv_present_message` text NOT NULL COMMENT '招待先プレゼントメッセージ',
  `start_date` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '開始日時',
  `end_date` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '終了日時',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enemy_ability_master`
--

DROP TABLE IF EXISTS `enemy_ability_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enemy_ability_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `name` text NOT NULL COMMENT '基本情報：名前',
  `detail` text NOT NULL COMMENT '基本情報：詳細テキスト',
  `icon` text NOT NULL COMMENT '基本情報：使用アイコンリソース名',
  `category` int(10) NOT NULL DEFAULT '0' COMMENT '基本情報：効果カテゴリ',
  `param_00` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ00',
  `param_01` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ01',
  `param_02` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ02',
  `param_03` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ03',
  `param_04` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ04',
  `param_05` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ05',
  `param_06` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ06',
  `param_07` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ07',
  `param_08` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ08',
  `param_09` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ09',
  `param_10` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ10',
  `param_11` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ11',
  `param_12` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ12',
  `param_13` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ13',
  `param_14` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ14',
  `param_15` int(10) NOT NULL DEFAULT '0' COMMENT '特性情報：汎用パラメータ15',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `text_definition_master`
--

DROP TABLE IF EXISTS `text_definition_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `text_definition_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `text_key` text NOT NULL COMMENT 'キー',
  `text` text NOT NULL COMMENT 'テキスト',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `link_system_master`
--

DROP TABLE IF EXISTS `link_system_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `link_system_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `elem` int(10) NOT NULL DEFAULT '0' COMMENT '基本情報：属性',
  `kind` int(10) NOT NULL DEFAULT '0' COMMENT '基本情報：種族',
  `rare` int(10) NOT NULL DEFAULT '0' COMMENT '基本情報：レア度',
  `hp` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクボーナス：体力',
  `atk` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクボーナス：攻撃力',
  `crt` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクボーナス：クリティカル威力',
  `bst` int(10) NOT NULL DEFAULT '0' COMMENT 'リンクボーナス：BOOSTパネル威力',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `top_page_master`
--

DROP TABLE IF EXISTS `top_page_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `top_page_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントタイミング：開始',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントタイミング：終了',
  `img_bg` text NOT NULL COMMENT '画像リソース：背景',
  `banner_img` text NOT NULL COMMENT 'バナー：画像リソース名',
  `debug_banner_img` text NOT NULL COMMENT 'バナー：画像リソース名(デバッグ用)',
  `banner_gacha_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'バナー：ガチャID',
  `banner_quest_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'バナー：クエストID',
  `banner_achievement_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ',
  `banner_achievement_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アチーブメントグループID',
  `banner_shop_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ショップタイプ',
  `a_cap` text NOT NULL COMMENT 'テキストエリア上：タイトル',
  `a_txt` text NOT NULL COMMENT 'テキストエリア上：内容',
  `a_gacha_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'テキストエリア上：ガチャID',
  `a_quest_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'テキストエリア上：クエストID',
  `a_achievement_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ',
  `a_achievement_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アチーブメントグループID',
  `a_shop_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ショップタイプ',
  `b_cap` text NOT NULL COMMENT 'テキストエリア下：タイトル',
  `b_txt` text NOT NULL COMMENT 'テキストエリア下：内容',
  `b_gacha_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'テキストエリア下：ガチャID',
  `b_quest_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'テキストエリア下：クエストID',
  `b_achievement_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アチーブメントタイプ',
  `b_achievement_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アチーブメントグループID',
  `b_shop_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ショップタイプ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `audio_data_master`
--

DROP TABLE IF EXISTS `audio_data_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `audio_data_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '所属グループID',
  `ducking_disable` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ダッキング無効',
  `res_name` text NOT NULL COMMENT 'オーディオファイル名',
  `vol_lv` int(10) NOT NULL DEFAULT '0' COMMENT 'ボリュームレベル',
  `rand_id_00` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ランダム再生SE_FiXID',
  `rand_id_01` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ランダム再生SE_FiXID',
  `rand_id_02` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ランダム再生SE_FiXID',
  `rand_id_03` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ランダム再生SE_FiXID',
  `rand_id_04` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ランダム再生SE_FiXID',
  `rand_id_05` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ランダム再生SE_FiXID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `point_shop_product_master`
--

DROP TABLE IF EXISTS `point_shop_product_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `point_shop_product_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `product_name` text NOT NULL COMMENT '商品表示名',
  `product_type` int(10) NOT NULL DEFAULT '0' COMMENT '商品種別',
  `product_param1` int(10) NOT NULL DEFAULT '0' COMMENT '商品パラメータ１',
  `product_param2` int(10) NOT NULL DEFAULT '0' COMMENT '商品パラメータ２',
  `product_param3` int(10) NOT NULL DEFAULT '0' COMMENT '商品パラメータ３',
  `product_param4` int(10) NOT NULL DEFAULT '0' COMMENT '商品パラメータ４',
  `product_param5` int(10) NOT NULL DEFAULT '0' COMMENT '商品パラメータ５',
  `product_param6` int(10) NOT NULL DEFAULT '0' COMMENT '商品パラメータ６',
  `product_param7` int(10) NOT NULL DEFAULT '0' COMMENT '商品パラメータ７',
  `priority` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '商品並び順',
  `price` int(10) NOT NULL DEFAULT '0' COMMENT '価格（ユニットポイント）',
  `show_limit` int(10) NOT NULL DEFAULT '0' COMMENT '残り時間表示',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '商品有効期間：開始',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '商品有効期間：終了',
  `timing_st_m` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ストアイベントの分単位指定：開始時刻 分',
  `timing_ed_m` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ストアイベントの分単位指定：終了時間 分',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gacha_ticket_master`
--

DROP TABLE IF EXISTS `gacha_ticket_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gacha_ticket_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `gacha_ticket_id` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャチケット定義の該当番号',
  `gacha_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャ定義ID',
  `gacha_ct` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャ連続回数',
  `gacha_tk_name` text NOT NULL COMMENT 'ガチャチケット名',
  `gacha_tk_msg` text NOT NULL COMMENT 'ガチャ詳細メッセージ',
  `exchange_item_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '変換先アイテムID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_gacha_ticket_id` (`gacha_ticket_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `limit_over_master`
--

DROP TABLE IF EXISTS `limit_over_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `limit_over_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `limit_over_max_hp` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破HP最大値',
  `limit_over_max_atk` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破ATK最大値',
  `limit_over_max_cost` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破COST最大値',
  `limit_over_max_charm` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破CHARM最大値',
  `limit_grow` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破成長タイプ',
  `limit_over_max` int(10) NOT NULL DEFAULT '0' COMMENT '限界突破最大値',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `item_master`
--

DROP TABLE IF EXISTS `item_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `item_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `item_name` varchar(190) NOT NULL DEFAULT '' COMMENT '消費アイテム名称',
  `item_icon` varchar(190) NOT NULL DEFAULT '' COMMENT '消費アイテムアイコン',
  `item_effect` text NOT NULL COMMENT '使用効果概要',
  `item_effect_detail` text NOT NULL COMMENT '使用効果詳細',
  `stamina_recovery` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '効果：スタミナ回復量割合',
  `exp_amend` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '効果：経験値補正',
  `coin_amend` int(10) NOT NULL DEFAULT '0' COMMENT '効果：コイン補正',
  `fp_amend` int(10) NOT NULL DEFAULT '0' COMMENT '効果：友情ポイント補正',
  `link_amend` int(10) NOT NULL DEFAULT '0' COMMENT '効果：リンクポイント補正',
  `tk_amend` int(10) NOT NULL DEFAULT '0' COMMENT '効果：チケット補正',
  `enemy_avoid` int(10) NOT NULL DEFAULT '0' COMMENT 'エネミー連鎖回避有無',
  `effect_span_m` int(10) NOT NULL DEFAULT '0' COMMENT '効果時間：分',
  `quest_use` int(10) NOT NULL DEFAULT '0' COMMENT 'クエスト入場時使用可否',
  `gacha_event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャに関連するイベントID指定',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `global_params_master`
--

DROP TABLE IF EXISTS `global_params_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `global_params_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `value` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'パラメータ',
  `text` text NOT NULL COMMENT 'テキスト',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quest_floor_bonus_master`
--

DROP TABLE IF EXISTS `quest_floor_bonus_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quest_floor_bonus_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `floor_bonus_questid` int(10) NOT NULL DEFAULT '0' COMMENT 'クエストID',
  `floor_bonus_type` int(10) NOT NULL DEFAULT '0' COMMENT '報酬タイプ',
  `floor_bonus_odds` int(10) NOT NULL DEFAULT '0' COMMENT '確率',
  `floor_bonus_param1` int(10) NOT NULL DEFAULT '0' COMMENT '報酬パラメータ１',
  `floor_bonus_param2` int(10) NOT NULL DEFAULT '0' COMMENT '報酬パラメータ２',
  `floor_bonus_param3` int(10) NOT NULL DEFAULT '0' COMMENT '報酬パラメータ３',
  `floor_bonus_param4` int(10) NOT NULL DEFAULT '0' COMMENT '報酬パラメータ４',
  `floor_bonus_param5` int(10) NOT NULL DEFAULT '0' COMMENT '報酬パラメータ５',
  `floor_bonus_param6` int(10) NOT NULL DEFAULT '0' COMMENT '報酬パラメータ６',
  `floor_bonus_param7` int(10) NOT NULL DEFAULT '0' COMMENT '報酬パラメータ７',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_questid` (`floor_bonus_questid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quest_key_master`
--

DROP TABLE IF EXISTS `quest_key_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quest_key_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントタイミング：終了',
  `key_name` varchar(190) NOT NULL DEFAULT '' COMMENT 'クエストキー名称',
  `key_area_category_id` int(11) NOT NULL DEFAULT '0' COMMENT '対応エリアカテゴリID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_timing_end` (`timing_end`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `achievement_group_master`
--

DROP TABLE IF EXISTS `achievement_group_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `achievement_group_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `category` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'カテゴリ',
  `draw_msg` varchar(190) NOT NULL DEFAULT '' COMMENT '表示文言',
  `list_sort_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '表示優先順位',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_timing_public` (`timing_public`),
  KEY `category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `web_view_master`
--

DROP TABLE IF EXISTS `web_view_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `web_view_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '開始タイミング',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '終了タイミング',
  `url_web` text NOT NULL COMMENT 'リンク先URL(prod,stg1,stg0,review)',
  `debug_url_web` text NOT NULL COMMENT 'リンク先URL(デバッグ環境)',
  `webview_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示条件',
  `show_every_time` int(10) NOT NULL DEFAULT '0' COMMENT '繰り返し表示',
  `webview_param_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_5` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_6` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_7` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_8` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_9` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `webview_param_10` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'webview表示処理用の汎用パラメータ ※表示判定用',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_timing_end` (`timing_public`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hero_master`
--

DROP TABLE IF EXISTS `hero_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `hero_master` (
  `fix_id` bigint(20) unsigned NOT NULL COMMENT '主人公ID',
  `timing_public` int(10) NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `name` varchar(32) NOT NULL COMMENT '主人公の名前',
  `detail` text NOT NULL COMMENT '詳細テキスト',
  `element` tinyint(3) unsigned NOT NULL COMMENT '主人公の属性',
  `kind` tinyint(3) unsigned NOT NULL COMMENT '主人公の種別 1: 生徒 2：先生',
  `gender` tinyint(3) unsigned NOT NULL COMMENT '主人公の性別 1:人間男性、2；人間女性',
  `default_skill_id` bigint(20) unsigned NOT NULL COMMENT '初期の必殺技ID',
  `default_party_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '付与パーティID',
  `img_story_tiling` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'タイリング',
  `img_story_offset_x` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'x座標',
  `img_story_offset_y` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'y座標',
  `illustrator_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '絵師ID illustrator_master.fix_id',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hero_level_master`
--

DROP TABLE IF EXISTS `hero_level_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `hero_level_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `exp_next` bigint(20) unsigned NOT NULL COMMENT '経験値：次までの数値',
  `exp_next_total` bigint(20) unsigned NOT NULL COMMENT '経験値：次までの合計値',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hero_add_effect_rate_master`
--

DROP TABLE IF EXISTS `hero_add_effect_rate_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `hero_add_effect_rate_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0',
  `hero_id` bigint(20) unsigned NOT NULL COMMENT '主人公ID',
  `start_level` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'レベル：該当範囲（始点）',
  `end_level` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'レベル：該当範囲（始点）',
  `additional_effect_value` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '追加効果発動率',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `topic_information_master`
--

DROP TABLE IF EXISTS `topic_information_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `topic_information_master` (
  `fix_id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '情報固有固定ID',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '1' COMMENT 'データ使用フラグ 0:NONE, 1:無効, 2:有効',
  `timing_start` int(10) NOT NULL DEFAULT '0' COMMENT '開始',
  `timing_end` int(10) NOT NULL DEFAULT '0' COMMENT '終了',
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '情報タイプ(1:お得、2:ゲリラ、3:新ユニット)',
  `kind` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ種類(1:画像、2:テキスト、3:アイコン)',
  `priority` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '優先度 同(type,kind)グループでの表示優先度',
  `data` text NOT NULL COMMENT 'データ JSON形式',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB AUTO_INCREMENT=382 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `story_master`
--

DROP TABLE IF EXISTS `story_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `story_master` (
  `fix_id` int(10) unsigned NOT NULL COMMENT 'ID',
  `story_id` int(10) unsigned NOT NULL COMMENT 'ストーリーID',
  `show_character_01_active` tinyint(3) unsigned NOT NULL COMMENT 'キャラ1表示',
  `show_character_01_slide` tinyint(3) unsigned NOT NULL COMMENT 'キャラ1スライド',
  `show_character_01` int(10) unsigned NOT NULL COMMENT 'キャラ1キャラID',
  `show_hero_01` bigint(20) unsigned NOT NULL COMMENT 'キャラ1主人公ID',
  `show_npc_01` bigint(20) unsigned NOT NULL COMMENT 'キャラ1NPCID',
  `show_chara_01_background` int(10) unsigned NOT NULL COMMENT 'キャラ1枠画像',
  `name_01_active` tinyint(3) unsigned NOT NULL COMMENT 'キャラ1名前表示',
  `name_01` varchar(64) NOT NULL COMMENT 'キャラ1名前',
  `show_character_02_active` tinyint(3) unsigned NOT NULL COMMENT 'キャラ2表示',
  `show_character_02_slide` tinyint(3) unsigned NOT NULL COMMENT 'キャラ2スライド',
  `show_character_02` int(10) unsigned NOT NULL COMMENT 'キャラ2キャラID',
  `show_hero_02` bigint(20) unsigned NOT NULL COMMENT 'キャラ2主人公ID',
  `show_npc_02` bigint(20) unsigned NOT NULL COMMENT 'キャラ2NPCID',
  `show_chara_02_background` int(10) unsigned NOT NULL COMMENT 'キャラ2枠画像',
  `name_02_active` tinyint(3) unsigned NOT NULL COMMENT 'キャラ2名前表示',
  `name_02` varchar(64) NOT NULL COMMENT 'キャラ2名前',
  `show_character_03_active` tinyint(3) unsigned NOT NULL COMMENT 'キャラ3表示',
  `show_character_03_slide` tinyint(3) unsigned NOT NULL COMMENT 'キャラ3スライド',
  `show_character_03` int(10) unsigned NOT NULL COMMENT 'キャラ3キャラID',
  `show_hero_03` bigint(20) unsigned NOT NULL COMMENT 'キャラ3主人公ID',
  `show_npc_03` bigint(20) unsigned NOT NULL COMMENT 'キャラ3NPCID',
  `show_chara_03_background` int(10) unsigned NOT NULL COMMENT 'キャラ3枠画像',
  `name_03_active` tinyint(3) unsigned NOT NULL COMMENT 'キャラ3名前表示',
  `name_03` varchar(64) NOT NULL COMMENT 'キャラ3名前',
  `show_character_04_active` tinyint(3) unsigned NOT NULL COMMENT 'キャラ4表示',
  `show_character_04_slide` tinyint(3) unsigned NOT NULL COMMENT 'キャラ4スライド',
  `show_character_04` int(10) unsigned NOT NULL COMMENT 'キャラ4キャラID',
  `show_hero_04` bigint(20) unsigned NOT NULL COMMENT 'キャラ4主人公ID',
  `show_npc_04` bigint(20) unsigned NOT NULL COMMENT 'キャラ4NPCID',
  `show_chara_04_background` int(10) unsigned NOT NULL COMMENT 'キャラ4枠画像',
  `name_04_active` tinyint(3) unsigned NOT NULL COMMENT 'キャラ4名前表示',
  `name_04` varchar(64) NOT NULL COMMENT 'キャラ4名前',
  `focus` int(10) unsigned NOT NULL COMMENT 'フォーカス',
  `label` text NOT NULL COMMENT '飛び先ラベル',
  `command` text NOT NULL COMMENT '挙動定義',
  `text` text NOT NULL COMMENT 'テキスト',
  `balloon_type` int(10) unsigned NOT NULL COMMENT '吹き出し',
  `back_ground_res` int(10) unsigned NOT NULL COMMENT '背景',
  `se_res` int(10) unsigned NOT NULL COMMENT 'SE',
  `bgm_active` tinyint(3) unsigned NOT NULL COMMENT 'BGM再生',
  `bgm_res` int(10) unsigned NOT NULL COMMENT 'BGM',
  `effect_fade_view` tinyint(3) unsigned NOT NULL COMMENT 'フェード',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_point_master`
--

DROP TABLE IF EXISTS `event_point_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_point_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `event_point_name` text NOT NULL COMMENT 'イベントポイント表示名',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_timing_public` (`timing_public`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `grossing_present_master`
--

DROP TABLE IF EXISTS `grossing_present_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `grossing_present_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `grossing_id` int(10) NOT NULL DEFAULT '0' COMMENT '報酬グループ',
  `grossing_type` int(10) NOT NULL DEFAULT '0' COMMENT '商品タイプ',
  `setting_type` int(10) NOT NULL DEFAULT '0' COMMENT '指定タイプ',
  `grossing_count` int(10) NOT NULL DEFAULT '0' COMMENT '景品が貰えるまでの回数',
  `grossing_max_count` int(10) NOT NULL DEFAULT '0' COMMENT '景品が貰えるまでの最大回数',
  `grossing_param1` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ１',
  `grossing_param2` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ２',
  `grossing_param3` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ３',
  `grossing_param4` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ４',
  `grossing_param5` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ５',
  `grossing_param6` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ６',
  `grossing_param7` int(10) NOT NULL DEFAULT '0' COMMENT 'パラメータ７',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_grossingid` (`grossing_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `enemy_hate_master`
--

DROP TABLE IF EXISTS `enemy_hate_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enemy_hate_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `hate_initial` int(10) NOT NULL DEFAULT '0' COMMENT '初期ヘイト値',
  `hate_given_damage1` int(10) NOT NULL DEFAULT '0' COMMENT '与ダメ1位が上昇するヘイト値',
  `hate_given_damage2` int(10) NOT NULL DEFAULT '0' COMMENT '与ダメ2位が上昇するヘイト値',
  `hate_given_damage3` int(10) NOT NULL DEFAULT '0' COMMENT '与ダメ3位が上昇するヘイト値',
  `hate_given_damage4` int(10) NOT NULL DEFAULT '0' COMMENT '与ダメ4位が上昇するヘイト値',
  `hate_given_damage5` int(10) NOT NULL DEFAULT '0' COMMENT '与ダメ5位が上昇するヘイト値',
  `hate_heal1` int(10) NOT NULL DEFAULT '0' COMMENT '回復1位が上昇するヘイト値',
  `hate_heal2` int(10) NOT NULL DEFAULT '0' COMMENT '回復2位が上昇するヘイト値',
  `hate_heal3` int(10) NOT NULL DEFAULT '0' COMMENT '回復3位が上昇するヘイト値',
  `hate_heal4` int(10) NOT NULL DEFAULT '0' COMMENT '回復4位が上昇するヘイト値',
  `hate_heal5` int(10) NOT NULL DEFAULT '0' COMMENT '回復5位が上昇するヘイト値',
  `hate_rate_fire` int(10) NOT NULL DEFAULT '0' COMMENT '火属性ヘイト上昇％',
  `hate_rate_water` int(10) NOT NULL DEFAULT '0' COMMENT '水属性ヘイト上昇％',
  `hate_rate_wind` int(10) NOT NULL DEFAULT '0' COMMENT '風属性ヘイト上昇％',
  `hate_rate_light` int(10) NOT NULL DEFAULT '0' COMMENT '光属性ヘイト上昇％',
  `hate_rate_dark` int(10) NOT NULL DEFAULT '0' COMMENT '闇属性ヘイト上昇％',
  `hate_rate_naught` int(10) NOT NULL DEFAULT '0' COMMENT '無属性ヘイト上昇％',
  `hate_rate_race1` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race2` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race3` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race4` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race5` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race6` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race7` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race8` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race9` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `hate_rate_race10` int(10) NOT NULL DEFAULT '0' COMMENT '種族ごとヘイト上昇管理',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `renew_quest_master`
--

DROP TABLE IF EXISTS `renew_quest_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `renew_quest_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `area_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エリア通し番号',
  `active` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `quest_name` varchar(255) NOT NULL DEFAULT '' COMMENT 'クエスト名称',
  `quest_score_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエストスコアID',
  `difficulty_name` varchar(64) NOT NULL DEFAULT '' COMMENT '難易度名称',
  `once` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '一度クリアしたら二度と出現しないクエスト',
  `enable_continue` int(10) NOT NULL DEFAULT '0' COMMENT 'コンティニュー有無',
  `enable_friendpoint` int(10) NOT NULL DEFAULT '0' COMMENT 'フレンドポイント有無',
  `enable_autoplay` int(10) NOT NULL DEFAULT '0' COMMENT 'オートプレイ有無',
  `consume_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '消費タイプ',
  `consume_value` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '消費量',
  `quest_floor_bonus_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'フロアボーナス報酬タイプ',
  `clear_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：お金',
  `clear_exp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：経験値',
  `clear_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'クリア報酬：リンクポイント',
  `clear_stone` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：魔法石（初回限定）',
  `clear_unit` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：ユニットID',
  `clear_unit_lv` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：ユニットレベル',
  `clear_unit_msg` text NOT NULL COMMENT 'クリア報酬：ユニット取得時文言',
  `clear_item` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：アイテム',
  `clear_item_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：アイテム個数',
  `clear_item_msg` varchar(64) NOT NULL DEFAULT '' COMMENT 'クリア報酬：アイテム',
  `clear_key` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：クエストキー',
  `clear_key_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：クエストキー個数',
  `clear_key_msg` varchar(64) NOT NULL DEFAULT '' COMMENT 'クリア報酬：クエストキー取得時文言',
  `limit_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'チート対策上限：お金',
  `limit_exp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'チート対策上限：経験値',
  `drop_max_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'バトル毎最大ドロップ制限数',
  `next_drop_rate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ率の減少値',
  `story` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ストーリーID クエストごとに紐づくstory_master.story_idを指定',
  `quest_requirement_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエスト入場条件',
  `quest_requirement_text` text NOT NULL COMMENT 'クエスト入場条件テキスト',
  `voice_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボイスグループID',
  `packname_voice` varchar(64) NOT NULL DEFAULT '' COMMENT 'サウンドパック：VOICE',
  `boss_icon_hide` int(10) NOT NULL DEFAULT '0' COMMENT 'ボスアイコンを隠すフラグ',
  `boss_icon_unit` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボスアイコン情報',
  `boss_ability_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_1',
  `boss_ability_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_2',
  `boss_ability_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_3',
  `boss_ability_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_4',
  `boss_chara_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現ボスのキャラID',
  `e_chara_id_0` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_1',
  `e_chara_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_2',
  `e_chara_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_3',
  `e_chara_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_4',
  `e_chara_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_5',
  `battle_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '戦闘回数',
  `enemy_group_id_1` int(10) NOT NULL DEFAULT '0' COMMENT '1戦目エネミーグループID',
  `enemy_group_id_2` int(10) NOT NULL DEFAULT '0' COMMENT '2戦目エネミーグループID',
  `enemy_group_id_3` int(10) NOT NULL DEFAULT '0' COMMENT '3戦目エネミーグループID',
  `enemy_group_id_4` int(10) NOT NULL DEFAULT '0' COMMENT '4戦目エネミーグループID',
  `enemy_group_id_5` int(10) NOT NULL DEFAULT '0' COMMENT '5戦目エネミーグループID',
  `enemy_group_id_6` int(10) NOT NULL DEFAULT '0' COMMENT '6戦目エネミーグループID',
  `enemy_group_id_7` int(10) NOT NULL DEFAULT '0' COMMENT '7戦目エネミーグループID',
  `enemy_group_id_8` int(10) NOT NULL DEFAULT '0' COMMENT '8戦目エネミーグループID',
  `enemy_group_id_9` int(10) NOT NULL DEFAULT '0' COMMENT '9戦目エネミーグループID',
  `enemy_group_id_10` int(10) NOT NULL DEFAULT '0' COMMENT '10戦目エネミーグループID',
  `enemy_group_id_11` int(10) NOT NULL DEFAULT '0' COMMENT '11戦目エネミーグループID',
  `enemy_group_id_12` int(10) NOT NULL DEFAULT '0' COMMENT '12戦目エネミーグループID',
  `enemy_group_id_13` int(10) NOT NULL DEFAULT '0' COMMENT '13戦目エネミーグループID',
  `enemy_group_id_14` int(10) NOT NULL DEFAULT '0' COMMENT '14戦目エネミーグループID',
  `enemy_group_id_15` int(10) NOT NULL DEFAULT '0' COMMENT '15戦目エネミーグループID',
  `enemy_group_id_16` int(10) NOT NULL DEFAULT '0' COMMENT '16戦目エネミーグループID',
  `enemy_group_id_17` int(10) NOT NULL DEFAULT '0' COMMENT '17戦目エネミーグループID',
  `enemy_group_id_18` int(10) NOT NULL DEFAULT '0' COMMENT '18戦目エネミーグループID',
  `enemy_group_id_19` int(10) NOT NULL DEFAULT '0' COMMENT '19戦目エネミーグループID',
  `boss_group_id` int(10) NOT NULL DEFAULT '0' COMMENT 'ボスグループID',
  `call_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '追加召喚するエネミーグループID',
  `first_attack` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ファーストアタック発生確率',
  `back_attack` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'バックアタック発生確率',
  `background` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエスト背景',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_area_id` (`area_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `npc_master`
--

DROP TABLE IF EXISTS `npc_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `npc_master` (
  `fix_id` bigint(20) unsigned NOT NULL COMMENT '主人公ID',
  `timing_public` int(10) NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `name` varchar(32) NOT NULL COMMENT '主人公の名前',
  `detail` text NOT NULL COMMENT '詳細テキスト',
  `img_story_tiling` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'タイリング',
  `img_story_offset_x` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'x座標',
  `img_story_offset_y` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'y座標',
  `illustrator_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '絵師ID illustrator_master.fix_id',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `story_chara_master`
--

DROP TABLE IF EXISTS `story_chara_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `story_chara_master` (
  `fix_id` int(10) unsigned NOT NULL COMMENT '情報固有ID',
  `chara_type` int(10) unsigned NOT NULL COMMENT 'キャラタイプ chara, hero, npc',
  `chara_id` int(10) unsigned NOT NULL COMMENT 'キャラID',
  `img_tiling` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'タイリング',
  `img_offset_x` int(10) NOT NULL DEFAULT '0' COMMENT 'x座標',
  `img_offset_y` int(10) NOT NULL DEFAULT '0' COMMENT 'y座標',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `illustrator_master`
--

DROP TABLE IF EXISTS `illustrator_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `illustrator_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `name` varchar(255) NOT NULL COMMENT '絵師名',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='絵師マスタ';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `period_login_event_master`
--

DROP TABLE IF EXISTS `period_login_event_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `period_login_event_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントタイミング：開始',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントタイミング：終了',
  `event_name` text NOT NULL COMMENT 'ログインイベント表示名',
  `group_id` int(10) NOT NULL DEFAULT '0' COMMENT '報酬グループ',
  `create_user_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '有効ユーザー作成日時：開始',
  `create_user_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '有効ユーザー作成日時：終了',
  `present_message` text NOT NULL COMMENT 'プレゼントダイアログメッセージ',
  `resource_store_name` text NOT NULL COMMENT '素材格納フォルダ名',
  `effect` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エフェクト番号',
  `sound_effect` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '効果音番号',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_timing_end` (`timing_public`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `period_login_event_group_master`
--

DROP TABLE IF EXISTS `period_login_event_group_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `period_login_event_group_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `group_id` int(10) NOT NULL DEFAULT '0' COMMENT '報酬グループ',
  `login_day_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '指定ログイン日数',
  `present_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼントグループ',
  `message` text NOT NULL COMMENT 'ダイアログメッセージ',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_group_id` (`group_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `region_master`
--

DROP TABLE IF EXISTS `region_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `region_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `name` text NOT NULL COMMENT 'リージョン名称',
  `category` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'カテゴリ',
  `sort` int(10) NOT NULL DEFAULT '0' COMMENT '図鑑表示順',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `renew_quest_score_master`
--

DROP TABLE IF EXISTS `renew_quest_score_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `renew_quest_score_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一意ID',
  `base_score` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '基礎スコア',
  `play_score_rate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレイスコア補正',
  `turn_penalty` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ペナルティ発生開始ターン数',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `play_score_master`
--

DROP TABLE IF EXISTS `play_score_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `play_score_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一意ID',
  `score_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スコアタイプ',
  `score_param` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '効果値',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `score_event_master`
--

DROP TABLE IF EXISTS `score_event_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `score_event_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一意ID',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントID',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スコアの報酬が受け取れる期間（開始）',
  `receiving_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'スコアの報酬が受け取れる期間（終了）',
  `image_present_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イメージID',
  `title` text NOT NULL COMMENT 'イベント名',
  `banner_url` text NOT NULL COMMENT 'バナーURL',
  `priority` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '表示優先度',
  `area_category_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '対象エリアカテゴリID',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  UNIQUE KEY `uq_event_id` (`event_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `score_reward_master`
--

DROP TABLE IF EXISTS `score_reward_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `score_reward_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一意ID',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントID',
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '報酬タイプ',
  `score` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '達成スコア',
  `present_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼントグループID',
  `present_message` text NOT NULL COMMENT 'プレゼントメッセージ',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_event_id_and_type` (`event_id`,`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quest_appearance_master`
--

DROP TABLE IF EXISTS `quest_appearance_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quest_appearance_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開日時',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '開始日時',
  `timing_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '終了日時',
  `area_category_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '対象エリアカテゴリID',
  `boss_text_key` text NOT NULL COMMENT '"BOSS"テキストキー',
  `battle_text_key` text NOT NULL COMMENT '"BATTLE"テキストキー',
  `enemy_info_text_key` text NOT NULL COMMENT '"ENEMY INFO"テキストキー',
  `asset_bundle_id` int(10) unsigned NOT NULL COMMENT 'アセットバンドルID',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `manage_banner_list`
--

DROP TABLE IF EXISTS `manage_banner_list`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `manage_banner_list` (
  `fix_id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '情報固有ID',
  `type` int(10) unsigned NOT NULL COMMENT 'バナータイプ',
  `banner` text NOT NULL COMMENT 'バナーURL',
  `name` varchar(255) NOT NULL COMMENT '名前',
  `platform` tinyint(3) NOT NULL COMMENT 'プラットフォーム',
  `timing_start` int(10) NOT NULL DEFAULT '0' COMMENT '開始日時',
  `timing_end` int(10) NOT NULL DEFAULT '0' COMMENT '終了日時',
  `data` text NOT NULL COMMENT 'パラメータ JSON文字列',
  `priority` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '優先度 小さい方が優先度高',
  `active` tinyint(3) NOT NULL COMMENT '利用フラグ 0: none, 1:無効, 2:有効',
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00' COMMENT '作成日時',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00' COMMENT '更新日時',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB AUTO_INCREMENT=164 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `step_up_gacha_master`
--

DROP TABLE IF EXISTS `step_up_gacha_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `step_up_gacha_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `timing_close` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開終了タイミング',
  `gacha_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャマスタID',
  `paid_tip_only` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '有償チップ専用フラグ',
  `reset_flg` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ステップ進行状況のリセットフラグ',
  `reset_date` int(10) NOT NULL DEFAULT '-1' COMMENT 'リセット(年月日時): 時間指定',
  `reset_time` int(10) NOT NULL DEFAULT '-1' COMMENT 'リセット(時分): 時間指定',
  `guideline_text_key` text COMMENT 'ガチャガイドライン使用テキストキー',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `step_up_gacha_manage_master`
--

DROP TABLE IF EXISTS `step_up_gacha_manage_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `step_up_gacha_manage_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `gacha_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャマスタID',
  `step_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '現在のステップ数',
  `next_step_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '次ステップ数を指定',
  `normal1_assign_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット抽選：通常枠1 アサインID',
  `normal1_lot_exec` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット抽選：通常枠1 実行回数',
  `normal2_assign_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット抽選：通常枠2 アサインID',
  `normal2_lot_exec` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット抽選：通常枠2 実行回数',
  `special_assign_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット抽選：確定枠 アサインID',
  `special_lot_exec` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ユニット抽選：確定枠 実行回数',
  `price` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'リソース総消費量',
  `url_img` text NOT NULL COMMENT '宣伝用画像 S3の格納先指定',
  `detail_text` text NOT NULL COMMENT '詳細説明文字 テキストキー指定',
  `present_enable` int(10) NOT NULL DEFAULT '0' COMMENT 'ガチャおまけ：有効化フラグ',
  `present_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ガチャおまけ：付与プレゼント グループID',
  `present_message` text COMMENT 'ガチャおまけ：付与プレゼントメッセージ',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_gacha_id` (`gacha_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `challenge_quest_master`
--

DROP TABLE IF EXISTS `challenge_quest_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `challenge_quest_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `timing_public` int(10) NOT NULL DEFAULT '0' COMMENT '一般公開タイミング',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントID',
  `area_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'エリア通し番号',
  `level_min` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '対象レベル（開始）',
  `level_max` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '対象レベル（終了）',
  `active` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `quest_name` varchar(255) NOT NULL DEFAULT '' COMMENT 'クエスト名称',
  `quest_score_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエストスコアID',
  `difficulty_name` varchar(64) NOT NULL DEFAULT '' COMMENT '難易度名称',
  `once` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '一度クリアしたら二度と出現しないクエスト',
  `enable_continue` int(10) NOT NULL DEFAULT '0' COMMENT 'コンティニュー有無',
  `enable_friendpoint` int(10) NOT NULL DEFAULT '0' COMMENT 'フレンドポイント有無',
  `enable_autoplay` int(10) NOT NULL DEFAULT '0' COMMENT 'オートプレイ有無',
  `consume_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '消費タイプ',
  `consume_value` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '消費量',
  `quest_floor_bonus_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'フロアボーナス報酬タイプ',
  `clear_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：お金',
  `clear_exp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：経験値',
  `clear_link_point` int(10) NOT NULL DEFAULT '0' COMMENT 'クリア報酬：リンクポイント',
  `clear_stone` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：魔法石（初回限定）',
  `clear_unit` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：ユニットID',
  `clear_unit_lv` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：ユニットレベル',
  `clear_unit_msg` text NOT NULL COMMENT 'クリア報酬：ユニット取得時文言',
  `clear_item` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：アイテム',
  `clear_item_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：アイテム個数',
  `clear_item_msg` varchar(64) NOT NULL DEFAULT '' COMMENT 'クリア報酬：アイテム',
  `clear_key` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：クエストキー',
  `clear_key_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア報酬：クエストキー個数',
  `clear_key_msg` varchar(64) NOT NULL DEFAULT '' COMMENT 'クリア報酬：クエストキー取得時文言',
  `limit_money` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'チート対策上限：お金',
  `limit_exp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'チート対策上限：経験値',
  `drop_max_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'バトル毎最大ドロップ制限数',
  `next_drop_rate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ドロップ率の減少値',
  `story` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ストーリーID クエストごとに紐づくstory_master.story_idを指定',
  `quest_requirement_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエスト入場条件',
  `quest_requirement_text` text NOT NULL COMMENT 'クエスト入場条件テキスト',
  `voice_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボイスグループID',
  `packname_voice` varchar(64) NOT NULL DEFAULT '' COMMENT 'サウンドパック：VOICE',
  `boss_icon_hide` int(10) NOT NULL DEFAULT '0' COMMENT 'ボスアイコンを隠すフラグ',
  `boss_icon_unit` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボスアイコン情報',
  `boss_ability_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_1',
  `boss_ability_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_2',
  `boss_ability_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_3',
  `boss_ability_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボス特性FixID_4',
  `boss_chara_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現ボスのキャラID',
  `e_chara_id_0` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_1',
  `e_chara_id_1` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_2',
  `e_chara_id_2` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_3',
  `e_chara_id_3` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_4',
  `e_chara_id_4` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '出現する雑魚敵のキャラID_5',
  `battle_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '戦闘回数',
  `enemy_group_id_1` int(10) NOT NULL DEFAULT '0' COMMENT '1戦目エネミーグループID',
  `enemy_group_id_2` int(10) NOT NULL DEFAULT '0' COMMENT '2戦目エネミーグループID',
  `enemy_group_id_3` int(10) NOT NULL DEFAULT '0' COMMENT '3戦目エネミーグループID',
  `enemy_group_id_4` int(10) NOT NULL DEFAULT '0' COMMENT '4戦目エネミーグループID',
  `enemy_group_id_5` int(10) NOT NULL DEFAULT '0' COMMENT '5戦目エネミーグループID',
  `enemy_group_id_6` int(10) NOT NULL DEFAULT '0' COMMENT '6戦目エネミーグループID',
  `enemy_group_id_7` int(10) NOT NULL DEFAULT '0' COMMENT '7戦目エネミーグループID',
  `enemy_group_id_8` int(10) NOT NULL DEFAULT '0' COMMENT '8戦目エネミーグループID',
  `enemy_group_id_9` int(10) NOT NULL DEFAULT '0' COMMENT '9戦目エネミーグループID',
  `enemy_group_id_10` int(10) NOT NULL DEFAULT '0' COMMENT '10戦目エネミーグループID',
  `enemy_group_id_11` int(10) NOT NULL DEFAULT '0' COMMENT '11戦目エネミーグループID',
  `enemy_group_id_12` int(10) NOT NULL DEFAULT '0' COMMENT '12戦目エネミーグループID',
  `enemy_group_id_13` int(10) NOT NULL DEFAULT '0' COMMENT '13戦目エネミーグループID',
  `enemy_group_id_14` int(10) NOT NULL DEFAULT '0' COMMENT '14戦目エネミーグループID',
  `enemy_group_id_15` int(10) NOT NULL DEFAULT '0' COMMENT '15戦目エネミーグループID',
  `enemy_group_id_16` int(10) NOT NULL DEFAULT '0' COMMENT '16戦目エネミーグループID',
  `enemy_group_id_17` int(10) NOT NULL DEFAULT '0' COMMENT '17戦目エネミーグループID',
  `enemy_group_id_18` int(10) NOT NULL DEFAULT '0' COMMENT '18戦目エネミーグループID',
  `enemy_group_id_19` int(10) NOT NULL DEFAULT '0' COMMENT '19戦目エネミーグループID',
  `boss_group_id` int(10) NOT NULL DEFAULT '0' COMMENT 'ボスグループID',
  `call_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '追加召喚するエネミーグループID',
  `first_attack` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ファーストアタック発生確率',
  `back_attack` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'バックアタック発生確率',
  `background` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'クエスト背景',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_event_id` (`event_id`),
  KEY `idx_area_id` (`area_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `challenge_event_master`
--

DROP TABLE IF EXISTS `challenge_event_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `challenge_event_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ID',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ID',
  `timing_start` int(10) unsigned NOT NULL DEFAULT '0',
  `receiving_end` int(10) unsigned NOT NULL DEFAULT '0',
  `title` text NOT NULL,
  `level_cap` smallint(5) unsigned NOT NULL DEFAULT '0',
  `boss_cap_level` smallint(5) unsigned NOT NULL DEFAULT '0',
  `boss_cap_hp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'HP',
  `boss_hp_curve_coefficient_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'HP',
  `boss_hp_growth_coefficient_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'HP',
  `boss_cap_attack` int(10) unsigned NOT NULL DEFAULT '0',
  `boss_atk_curve_coefficient_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ATK',
  `boss_atk_growth_coefficient_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ATK',
  `side_cap_level` smallint(5) unsigned NOT NULL DEFAULT '0',
  `side_cap_hp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'HP',
  `side_hp_curve_coefficient_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'HP',
  `side_hp_growth_coefficient_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'HP',
  `side_cap_attack` int(10) unsigned NOT NULL DEFAULT '0',
  `side_atk_curve_coefficient_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ATK',
  `side_atk_growth_coefficient_num` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ATK',
  `bg_assetbundle_name` text NOT NULL,
  `skip_level` smallint(5) unsigned NOT NULL DEFAULT '0',
  `skip_base_ticket_num` int(10) unsigned NOT NULL DEFAULT '0',
  `skip_stepup_ticket_num` int(10) unsigned NOT NULL DEFAULT '0',
  `skip_stepup_max` smallint(5) unsigned NOT NULL DEFAULT '0',
  `skip_max` smallint(5) unsigned NOT NULL DEFAULT '0',
  `open_count` int(10) unsigned NOT NULL DEFAULT '0',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`fix_id`),
  KEY `idx_event_id` (`event_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `challenge_reward_master`
--

DROP TABLE IF EXISTS `challenge_reward_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `challenge_reward_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '報酬ID',
  `event_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'イベントID',
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '報酬タイプ',
  `clear_param` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'クリア値',
  `clear_loop_reward_enable` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'ループ報酬の有無',
  `clear_loop_reward_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ループ報酬の獲得に必要なのクリア回数',
  `present_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プレゼントグループID',
  `present_message` text NOT NULL COMMENT 'プレゼントメッセージ',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`),
  KEY `idx_event_id` (`event_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `step_up_gacha_assign_master`
--

DROP TABLE IF EXISTS `step_up_gacha_assign_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `step_up_gacha_assign_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `active` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ使用フラグ',
  `assign_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アサインID',
  `assign_chara_id` smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT 'アサインキャラ情報：出現キャラ',
  `assign_rate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'アサインキャラ情報：出現確率',
  `assign_level` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'アサインキャラ情報：出現レベル',
  `plus_pow` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プラス値確率：攻撃',
  `plus_hp` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'プラス値確率：体力',
  `limitover_lv` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '限界突破',
  `limit_icon` tinyint(3) NOT NULL DEFAULT '0' COMMENT 'ラインナップ画面 限定表記の有無設定',
  `assign_rate_up` tinyint(3) NOT NULL DEFAULT '0' COMMENT '超絶UP設定 0: なし, 1: 1.5倍, 3: 3倍, 4: 4倍, 5: 5倍, 10: 10倍,',
  `lineup_sort_group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ラインナップ画面 ソート用グループID',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gacha_text_master`
--

DROP TABLE IF EXISTS `gacha_text_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gacha_text_master` (
  `fix_id` int(10) unsigned NOT NULL COMMENT '情報固有ID',
  `kind` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'データ種 1:詳細, 2:ガイドライン, 3:ラインナップURL',
  `label` varchar(64) NOT NULL COMMENT '管理用ラベル',
  `text` text NOT NULL COMMENT '内容',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gacha_text_ref_master`
--

DROP TABLE IF EXISTS `gacha_text_ref_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gacha_text_ref_master` (
  `fix_id` int(10) unsigned NOT NULL COMMENT '情報固有ID',
  `master_type` int(10) unsigned NOT NULL COMMENT 'マスタタイプ 62:group, 125:step_up, 126:step_up_manage',
  `master_fix_id` int(10) unsigned NOT NULL COMMENT 'マスタタイプfix_id',
  `guideline_text_id` int(10) unsigned NOT NULL COMMENT 'ガイドラインID gacha_text_master.fix_id',
  `detail_text_id` int(10) unsigned NOT NULL COMMENT '詳細テキストID gacha_text_master.fix_id',
  `normal1_rate_url_text_id` int(10) unsigned NOT NULL COMMENT 'normal1排出率URLID gacha_text_master.fix_id',
  `normal2_rate_url_text_id` int(10) unsigned NOT NULL COMMENT 'normal2排出率URLID gacha_text_master.fix_id',
  `special_rate_url_text_id` int(10) unsigned NOT NULL COMMENT 'special排出率URLID gacha_text_master.fix_id',
  `show_rate_flag` tinyint(3) unsigned NOT NULL COMMENT '排出率表示フラグ 0:無効, 1:非表示, 2:表示',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `general_window_master`
--

DROP TABLE IF EXISTS `general_window_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `general_window_master` (
  `fix_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '情報固有ID',
  `group_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'グループID',
  `title` varchar(64) NOT NULL DEFAULT '' COMMENT '見出し',
  `message_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '表示タイプ 0:文章/1:画像',
  `message` text NOT NULL COMMENT '表示リソース 文章 or アセットID',
  `char_img` text NOT NULL COMMENT '表示キャラ アセットID',
  `char_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'キャラ表示位置 0:左/1:中央/2:右',
  `char_offset_x` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラ表示調整値x',
  `char_offset_y` int(10) NOT NULL DEFAULT '0' COMMENT 'キャラ表示調整値y',
  `button_type` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ボタン表示タイプ 0:閉じる,次へ/1:はい,いいえ',
  `button_01` text NOT NULL COMMENT 'ボタン1テキストキー',
  `button_02` text NOT NULL COMMENT 'ボタン2テキストキー',
  `tag_id` int(10) unsigned NOT NULL COMMENT '更新タグ',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `master_hash`
--

DROP TABLE IF EXISTS `master_hash`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `master_hash` (
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT 'マスタデータタイプ',
  `hash` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ハッシュ値',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '1' COMMENT '更新タグ',
  PRIMARY KEY (`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `master_hash_log`
--

DROP TABLE IF EXISTS `master_hash_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `master_hash_log` (
  `date` datetime NOT NULL COMMENT '日時',
  `type` tinyint(3) unsigned NOT NULL COMMENT 'マスターデータタイプ',
  `hash` int(10) unsigned NOT NULL DEFAULT '0',
  `tag_id` int(10) unsigned NOT NULL DEFAULT '1' COMMENT '更新タグ',
  `tag_id_real` int(10) unsigned NOT NULL DEFAULT '1' COMMENT '更新タグ(当時)',
  `manage_tag_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '更新管理タグ',
  `delete_count` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '削除レコード数',
  PRIMARY KEY (`type`,`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `master_delete_info`
--

DROP TABLE IF EXISTS `master_delete_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `master_delete_info` (
  `fix_id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '情報固有ID',
  `type` int(10) unsigned NOT NULL COMMENT '削除レコードのテーブル名',
  `tag_id` int(10) unsigned NOT NULL COMMENT 'レコードが削除された時の更新タグ',
  `pkey` varchar(255) NOT NULL COMMENT '削除レコードのprimary key値',
  `delete_date` datetime NOT NULL COMMENT '削除日時',
  PRIMARY KEY (`fix_id`)
) ENGINE=InnoDB AUTO_INCREMENT=121946 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-05-10 20:51:52