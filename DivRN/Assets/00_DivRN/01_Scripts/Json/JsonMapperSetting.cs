using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonMapperSetting
{
    private static bool bInit = false;
    /// <summary>
    /// JsonMapper拡張
    /// </summary>
    static public void Setup()
    {
        if (bInit)
        {
            return;
        }
        bInit = true;

        //int => long
        LitJson.JsonMapper.RegisterImporter<System.Int32, long>((input) => { return System.Convert.ToInt64(input); });
        LitJson.JsonMapper.RegisterImporter<System.Int32, ulong>((input) => { return System.Convert.ToUInt64(input); });

        //int => Enum 変換
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.BoolType>((input) => { return (MasterDataDefineLabel.BoolType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.TrapType>((input) => { return (MasterDataDefineLabel.TrapType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.ElementType>((input) => { return (MasterDataDefineLabel.ElementType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.CurveType>((input) => { return (MasterDataDefineLabel.CurveType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.PivotType>((input) => { return (MasterDataDefineLabel.PivotType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.RarityType>((input) => { return (MasterDataDefineLabel.RarityType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.KindType>((input) => { return (MasterDataDefineLabel.KindType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.GachaType>((input) => { return (MasterDataDefineLabel.GachaType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.AreaCategory>((input) => { return (MasterDataDefineLabel.AreaCategory)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.BelongType>((input) => { return (MasterDataDefineLabel.BelongType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.AmendType>((input) => { return (MasterDataDefineLabel.AmendType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.StatusType>((input) => { return (MasterDataDefineLabel.StatusType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.SkillType>((input) => { return (MasterDataDefineLabel.SkillType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.UIEffectType>((input) => { return (MasterDataDefineLabel.UIEffectType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.SkillCategory>((input) => { return (MasterDataDefineLabel.SkillCategory)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.LBSkillPhase>((input) => { return (MasterDataDefineLabel.LBSkillPhase)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.AilmentType>((input) => { return (MasterDataDefineLabel.AilmentType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.EnemyACTSelectType>((input) => { return (MasterDataDefineLabel.EnemyACTSelectType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.EnemyACTPatternSelectType>((input) => { return (MasterDataDefineLabel.EnemyACTPatternSelectType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.EnemyMotionType>((input) => { return (MasterDataDefineLabel.EnemyMotionType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.EnemyATKShakeType>((input) => { return (MasterDataDefineLabel.EnemyATKShakeType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.EnemySkillCategory>((input) => { return (MasterDataDefineLabel.EnemySkillCategory)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.TargetType>((input) => { return (MasterDataDefineLabel.TargetType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.LeaderSkillCategory>((input) => { return (MasterDataDefineLabel.LeaderSkillCategory)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.PassiveSkillCategory>((input) => { return (MasterDataDefineLabel.PassiveSkillCategory)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.PresentType>((input) => { return (MasterDataDefineLabel.PresentType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.AchievementType>((input) => { return (MasterDataDefineLabel.AchievementType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.AchievementCategory>((input) => { return (MasterDataDefineLabel.AchievementCategory)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.GachaTabIndex>((input) => { return (MasterDataDefineLabel.GachaTabIndex)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.BoostSkillCategory>((input) => { return (MasterDataDefineLabel.BoostSkillCategory)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.NotificationType>((input) => { return (MasterDataDefineLabel.NotificationType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.SkillLevelElementType>((input) => { return (MasterDataDefineLabel.SkillLevelElementType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.EvoluveEffectType>((input) => { return (MasterDataDefineLabel.EvoluveEffectType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.EnemyAbilityType>((input) => { return (MasterDataDefineLabel.EnemyAbilityType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.AilmentGroup>((input) => { return (MasterDataDefineLabel.AilmentGroup)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.StoreType>((input) => { return (MasterDataDefineLabel.StoreType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.EventType>((input) => { return (MasterDataDefineLabel.EventType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.ConditionType>((input) => { return (MasterDataDefineLabel.ConditionType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.SkillCostType>((input) => { return (MasterDataDefineLabel.SkillCostType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.HPGaugeType>((input) => { return (MasterDataDefineLabel.HPGaugeType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.PanelType>((input) => { return (MasterDataDefineLabel.PanelType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.FieldType>((input) => { return (MasterDataDefineLabel.FieldType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.MethodType>((input) => { return (MasterDataDefineLabel.MethodType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.ActiveAbilityType>((input) => { return (MasterDataDefineLabel.ActiveAbilityType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.PointShopType>((input) => { return (MasterDataDefineLabel.PointShopType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.LimitOverCurveType>((input) => { return (MasterDataDefineLabel.LimitOverCurveType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.FloorBonusType>((input) => { return (MasterDataDefineLabel.FloorBonusType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.TopPageJump>((input) => { return (MasterDataDefineLabel.TopPageJump)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.PeriodType>((input) => { return (MasterDataDefineLabel.PeriodType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.UnitSkillType>((input) => { return (MasterDataDefineLabel.UnitSkillType)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.AchievementState>((input) => { return (MasterDataDefineLabel.AchievementState)input; });
        LitJson.JsonMapper.RegisterImporter<int, MasterDataDefineLabel.UnitIconType>((input) => { return (MasterDataDefineLabel.UnitIconType)input; });
    }

    static public void SetupNew()
    {
        if (bInit)
        {
            return;
        }
        bInit = true;

        //int => long
        LitJson.JsonMapper.RegisterImporter<System.Int32, long>((input) => { return System.Convert.ToInt64(input); });
        LitJson.JsonMapper.RegisterImporter<System.Int32, ulong>((input) => { return System.Convert.ToUInt64(input); });

        //string => Enum 変換
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.BoolType>((input) => { return (MasterDataDefineLabel.BoolType)Enum.Parse(typeof(MasterDataDefineLabel.BoolType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.TrapType>((input) => { return (MasterDataDefineLabel.TrapType)Enum.Parse(typeof(MasterDataDefineLabel.TrapType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.ElementType>((input) => { return (MasterDataDefineLabel.ElementType)Enum.Parse(typeof(MasterDataDefineLabel.ElementType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.CurveType>((input) => { return (MasterDataDefineLabel.CurveType)Enum.Parse(typeof(MasterDataDefineLabel.CurveType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.PivotType>((input) => { return (MasterDataDefineLabel.PivotType)Enum.Parse(typeof(MasterDataDefineLabel.PivotType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.RarityType>((input) => { return (MasterDataDefineLabel.RarityType)Enum.Parse(typeof(MasterDataDefineLabel.RarityType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.KindType>((input) => { return (MasterDataDefineLabel.KindType)Enum.Parse(typeof(MasterDataDefineLabel.KindType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.GachaType>((input) => { return (MasterDataDefineLabel.GachaType)Enum.Parse(typeof(MasterDataDefineLabel.GachaType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.AreaCategory>((input) => { return (MasterDataDefineLabel.AreaCategory)Enum.Parse(typeof(MasterDataDefineLabel.AreaCategory), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.BelongType>((input) => { return (MasterDataDefineLabel.BelongType)Enum.Parse(typeof(MasterDataDefineLabel.BelongType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.AmendType>((input) => { return (MasterDataDefineLabel.AmendType)Enum.Parse(typeof(MasterDataDefineLabel.AmendType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.StatusType>((input) => { return (MasterDataDefineLabel.StatusType)Enum.Parse(typeof(MasterDataDefineLabel.StatusType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.SkillType>((input) => { return (MasterDataDefineLabel.SkillType)Enum.Parse(typeof(MasterDataDefineLabel.SkillType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.UIEffectType>((input) => { return (MasterDataDefineLabel.UIEffectType)Enum.Parse(typeof(MasterDataDefineLabel.UIEffectType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.SkillCategory>((input) => { return (MasterDataDefineLabel.SkillCategory)Enum.Parse(typeof(MasterDataDefineLabel.SkillCategory), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.LBSkillPhase>((input) => { return (MasterDataDefineLabel.LBSkillPhase)Enum.Parse(typeof(MasterDataDefineLabel.LBSkillPhase), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.AilmentType>((input) => { return (MasterDataDefineLabel.AilmentType)Enum.Parse(typeof(MasterDataDefineLabel.AilmentType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.EnemyACTSelectType>((input) => { return (MasterDataDefineLabel.EnemyACTSelectType)Enum.Parse(typeof(MasterDataDefineLabel.EnemyACTSelectType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.EnemyACTPatternSelectType>((input) => { return (MasterDataDefineLabel.EnemyACTPatternSelectType)Enum.Parse(typeof(MasterDataDefineLabel.EnemyACTPatternSelectType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.EnemyMotionType>((input) => { return (MasterDataDefineLabel.EnemyMotionType)Enum.Parse(typeof(MasterDataDefineLabel.EnemyMotionType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.EnemyATKShakeType>((input) => { return (MasterDataDefineLabel.EnemyATKShakeType)Enum.Parse(typeof(MasterDataDefineLabel.EnemyATKShakeType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.EnemySkillCategory>((input) => { return (MasterDataDefineLabel.EnemySkillCategory)Enum.Parse(typeof(MasterDataDefineLabel.EnemySkillCategory), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.TargetType>((input) => { return (MasterDataDefineLabel.TargetType)Enum.Parse(typeof(MasterDataDefineLabel.TargetType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.LeaderSkillCategory>((input) => { return (MasterDataDefineLabel.LeaderSkillCategory)Enum.Parse(typeof(MasterDataDefineLabel.LeaderSkillCategory), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.PassiveSkillCategory>((input) => { return (MasterDataDefineLabel.PassiveSkillCategory)Enum.Parse(typeof(MasterDataDefineLabel.PassiveSkillCategory), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.PresentType>((input) => { return (MasterDataDefineLabel.PresentType)Enum.Parse(typeof(MasterDataDefineLabel.PresentType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.AchievementType>((input) => { return (MasterDataDefineLabel.AchievementType)Enum.Parse(typeof(MasterDataDefineLabel.AchievementType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.AchievementCategory>((input) => { return (MasterDataDefineLabel.AchievementCategory)Enum.Parse(typeof(MasterDataDefineLabel.AchievementCategory), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.GachaTabIndex>((input) => { return (MasterDataDefineLabel.GachaTabIndex)Enum.Parse(typeof(MasterDataDefineLabel.GachaTabIndex), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.BoostSkillCategory>((input) => { return (MasterDataDefineLabel.BoostSkillCategory)Enum.Parse(typeof(MasterDataDefineLabel.BoostSkillCategory), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.NotificationType>((input) => { return (MasterDataDefineLabel.NotificationType)Enum.Parse(typeof(MasterDataDefineLabel.NotificationType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.SkillLevelElementType>((input) => { return (MasterDataDefineLabel.SkillLevelElementType)Enum.Parse(typeof(MasterDataDefineLabel.SkillLevelElementType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.EvoluveEffectType>((input) => { return (MasterDataDefineLabel.EvoluveEffectType)Enum.Parse(typeof(MasterDataDefineLabel.EvoluveEffectType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.EnemyAbilityType>((input) => { return (MasterDataDefineLabel.EnemyAbilityType)Enum.Parse(typeof(MasterDataDefineLabel.EnemyAbilityType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.AilmentGroup>((input) => { return (MasterDataDefineLabel.AilmentGroup)Enum.Parse(typeof(MasterDataDefineLabel.AilmentGroup), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.StoreType>((input) => { return (MasterDataDefineLabel.StoreType)Enum.Parse(typeof(MasterDataDefineLabel.StoreType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.EventType>((input) => { return (MasterDataDefineLabel.EventType)Enum.Parse(typeof(MasterDataDefineLabel.EventType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.ConditionType>((input) => { return (MasterDataDefineLabel.ConditionType)Enum.Parse(typeof(MasterDataDefineLabel.ConditionType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.SkillCostType>((input) => { return (MasterDataDefineLabel.SkillCostType)Enum.Parse(typeof(MasterDataDefineLabel.SkillCostType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.HPGaugeType>((input) => { return (MasterDataDefineLabel.HPGaugeType)Enum.Parse(typeof(MasterDataDefineLabel.HPGaugeType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.PanelType>((input) => { return (MasterDataDefineLabel.PanelType)Enum.Parse(typeof(MasterDataDefineLabel.PanelType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.FieldType>((input) => { return (MasterDataDefineLabel.FieldType)Enum.Parse(typeof(MasterDataDefineLabel.FieldType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.MethodType>((input) => { return (MasterDataDefineLabel.MethodType)Enum.Parse(typeof(MasterDataDefineLabel.MethodType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.ActiveAbilityType>((input) => { return (MasterDataDefineLabel.ActiveAbilityType)Enum.Parse(typeof(MasterDataDefineLabel.ActiveAbilityType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.PointShopType>((input) => { return (MasterDataDefineLabel.PointShopType)Enum.Parse(typeof(MasterDataDefineLabel.PointShopType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.LimitOverCurveType>((input) => { return (MasterDataDefineLabel.LimitOverCurveType)Enum.Parse(typeof(MasterDataDefineLabel.LimitOverCurveType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.FloorBonusType>((input) => { return (MasterDataDefineLabel.FloorBonusType)Enum.Parse(typeof(MasterDataDefineLabel.FloorBonusType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.TopPageJump>((input) => { return (MasterDataDefineLabel.TopPageJump)Enum.Parse(typeof(MasterDataDefineLabel.TopPageJump), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.PeriodType>((input) => { return (MasterDataDefineLabel.PeriodType)Enum.Parse(typeof(MasterDataDefineLabel.PeriodType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.UnitSkillType>((input) => { return (MasterDataDefineLabel.UnitSkillType)Enum.Parse(typeof(MasterDataDefineLabel.UnitSkillType), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.AchievementState>((input) => { return (MasterDataDefineLabel.AchievementState)Enum.Parse(typeof(MasterDataDefineLabel.AchievementState), input); });
        LitJson.JsonMapper.RegisterImporter<string, MasterDataDefineLabel.UnitIconType>((input) => { return (MasterDataDefineLabel.UnitIconType)Enum.Parse(typeof(MasterDataDefineLabel.UnitIconType), input); });
        LitJson.JsonMapper.RegisterImporter<string, SEID>((input) => { return (SEID)Enum.Parse(typeof(SEID), input); });
        LitJson.JsonMapper.RegisterImporter<string, BGMManager.EBGM_ID>((input) => { return (BGMManager.EBGM_ID)Enum.Parse(typeof(BGMManager.EBGM_ID), input); });
    }

}
/*
	BoolType
	TrapType
	ElementType
	CurveType
	PivotType
	RarityType
	KindType
	GachaType
	AreaCategory
	BelongType
	AmendType
	StatusType
	SkillType
	UIEffectType
	SkillCategory
	LBSkillPhase
	AilmentType
	EnemyACTSelectType
	EnemyACTPatternSelectType
	EnemyMotionType
	EnemyATKShakeType
	EnemySkillCategory
	TargetType
	LeaderSkillCategory
	PassiveSkillCategory
	PresentType
	AchievementType
	AchievementCategory
	GachaTabIndex
	BoostSkillCategory
	NotificationType
	SkillLevelElementType
	EvoluveEffectType
	EnemyAbilityType
	AilmentGroup
	StoreType
	EventType
	ConditionType
	SkillCostType
	HPGaugeType
	PanelType
	FieldType
	MethodType
	ActiveAbilityType
	PointShopType
	LimitOverCurveType
	FloorBonusType
	TopPageJump
	PeriodType
	UnitSkillType
	AchievementState
	StatusDisplayType
	UnitIconType
*/
