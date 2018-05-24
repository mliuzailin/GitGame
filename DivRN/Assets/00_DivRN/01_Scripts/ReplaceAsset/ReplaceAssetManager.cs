//#define TEST_LOCAL_ASSETBUNDLE	//StreamingAssets に置いたアセットバンドルでテスト

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リソース差し替え
/// アセットバンドルから読み込んだアセットで既存のリソースを一時的に差し替える。
/// </summary>
public class ReplaceAssetManager : SingletonComponent<ReplaceAssetManager>
{
    // スプライト(SpriteRenderer)を差し替えた時用のマテリアル
    public Material m_SpriteDefaultMaterial = null;

    public enum Status
    {
        NONE,   // 差し替えモード中ではない
        LOADING,    // 差し替えモードへ切り替え中
        REPLACE_MODE,       // 差し替えモード中
    }

    private Status m_Status = Status.NONE;
    private uint m_AreaCategoryID = 0;
    private ReplaceAssetReference.ChangeTimingType m_TimingType = ReplaceAssetReference.ChangeTimingType.NONE;
    private bool m_IsUseEffect = false;

    private AssetBundle m_AssetBundle = null;
    // リソース差し替え情報
    private ReplaceAssetReference m_ReplaceAssetReference = null;

    private int m_SerialNo = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public IEnumerator downloadReplaceAssetbundle(AssetBundlerMultiplier multiplier)
    {
#if TEST_LOCAL_ASSETBUNDLE
        multiplier = null;
#endif //TEST_LOCAL_ASSETBUNDLE

        // ここではダウンロードするだけ
        if (multiplier != null)
        {
            MasterDataQuestAppearance[] quest_appearances = MasterFinder<MasterDataQuestAppearance>.Instance.GetAll();

            for (int idx = 0; idx < quest_appearances.Length; idx++)
            {
                MasterDataQuestAppearance quest_appearance = quest_appearances[idx];

                MasterDataAssetBundlePath assetbundle_path = MasterFinder<MasterDataAssetBundlePath>.Instance.Find((int)quest_appearance.asset_bundle_id);
                if (assetbundle_path != null)
                {
                    multiplier.Add(
                        AssetBundler.Create().Set(assetbundle_path.name, (result) =>
                        {
#if BUILD_TYPE_DEBUG
                            if (result.AssetBundle != null)
                            {
                                Debug.Log("ReplaseAssetManager:asset download success:" + result.AssetBundle.name);
                            }
#endif //BUILD_TYPE_DEBUG
                        }
                        , null
                        )
                    );
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// 差し替えモード中かどうか
    /// </summary>
    /// <returns></returns>
    public Status getStatus()
    {
        return m_Status;
    }

    public int getSerialNo()
    {
        return m_SerialNo;
    }

    /// <summary>
    /// クエストＩＤからリージョンＩＤを探す
    /// </summary>
    /// <param name="quest_id"></param>
    /// <returns></returns>
    public static uint getAreaCategoryIDFromQuestID(uint quest_id)
    {
        uint area_category_id = 0;

        MasterDataQuest2 master_data_quest = MasterDataUtil.GetQuest2ParamFromID(quest_id);
        if (master_data_quest != null)
        {
            MasterDataArea master_data_area = MasterFinder<MasterDataArea>.Instance.Find((int)master_data_quest.area_id);
            if (master_data_area != null)
            {
                MasterDataAreaCategory master_data_area_category = MasterFinder<MasterDataAreaCategory>.Instance.Find((int)master_data_area.area_cate_id);
                if (master_data_area_category != null)
                {
                    area_category_id = master_data_area_category.fix_id;
                }
            }
        }

        return area_category_id;
    }

    /// <summary>
    /// 差し替えモードを開始する
    /// 差し替えモードになるまでは少し時間がかかります（getStatus()で調べる）
    /// </summary>
    /// <param name="area_category_id"></param>
    /// <param name="timing_type"></param>
    public void startReplaceMode(uint area_category_id, ReplaceAssetReference.ChangeTimingType timing_type)
    {
        endReplaceMode();

        m_Status = Status.LOADING;
        m_AreaCategoryID = area_category_id;
        m_TimingType = timing_type;
        m_IsUseEffect = (m_TimingType == ReplaceAssetReference.ChangeTimingType.BATTLE_NORMAL || m_TimingType == ReplaceAssetReference.ChangeTimingType.BATTLE_BOSS);

        StartCoroutine(loadReplaceAssetInfo());
    }

    /// <summary>
    /// 差し替えモードを終了する
    /// </summary>
    public void endReplaceMode()
    {
        StopCoroutine(loadReplaceAssetInfo());

        if (SoundManager.HasInstance)
        {
            SoundManager.Instance.clearReplaceSE();
        }

        if (m_AssetBundle != null)
        {
            m_AssetBundle.Unload(false);
            m_AssetBundle = null;
        }
        m_ReplaceAssetReference = null;
        m_AreaCategoryID = 0;
        m_TimingType = ReplaceAssetReference.ChangeTimingType.NONE;
        m_Status = Status.NONE;
    }

    private IEnumerator loadReplaceAssetInfo()
    {
        // 差し替え情報を探す
        MasterDataQuestAppearance[] quest_appearances = MasterFinder<MasterDataQuestAppearance>.Instance.GetAll();
        MasterDataQuestAppearance quest_appearance = null;
        if (m_AreaCategoryID > 0)
        {
            for (int idx = 0; idx < quest_appearances.Length; idx++)
            {
                MasterDataQuestAppearance wrk_quest_appearance = quest_appearances[idx];

                if (wrk_quest_appearance.area_category_id == m_AreaCategoryID)
                {
                    quest_appearance = wrk_quest_appearance;
                    break;
                }
            }
        }

        bool is_finish_download = false;

#if !TEST_LOCAL_ASSETBUNDLE
        // 差し替え情報を元にアセットバンドルをダウンロード
        if (quest_appearance != null)
        {
            MasterDataAssetBundlePath assetbundle_path = MasterFinder<MasterDataAssetBundlePath>.Instance.Find((int)quest_appearance.asset_bundle_id);
            if (assetbundle_path != null)
            {
                AssetBundler.Create().Set(assetbundle_path.name, (result) =>
                {
                    if (result.AssetBundle != null)
                    {
                        m_ReplaceAssetReference = ReplaceAssetReference.getReplaceAssetReference(result.AssetBundle, m_TimingType);
                    }

                    is_finish_download = true;
                },
                (o) =>
                {
                    is_finish_download = true;
                }
                ).Load();
            }
            else
            {
                is_finish_download = true;
            }
        }
        else
        {
            is_finish_download = true;
        }
#else //TEST_LOCAL_ASSETBUNDLE
        // StreamingAssetsフォルダ内のデータでテスト
        WWW www = debug_LoadLocalAssetBundle("kensyou");
        while (www.isDone == false)
        {
            yield return null;
        }

        m_AssetBundle = www.assetBundle;
        if (m_AssetBundle != null)
        {
            m_ReplaceAssetReference = ReplaceAssetReference.getReplaceAssetReference(m_AssetBundle, m_TimingType);
            //m_AssetBundle.Unload(false);
        }
        is_finish_download = true;
#endif //TEST_LOCAL_ASSETBUNDLE

        SceneObjReferGameMainEffect effect_assign = null;
        if (m_IsUseEffect)
        {
            GameObject effect_prefab = Resources.Load<GameObject>("Prefab/BattleScene/InGameEffectPrefab");
            GameObject effect_obj = Instantiate(effect_prefab); // effect_prefabのコンポーネントを書き換えると以前に書き換えた情報が残っているので別オブジェクトを一回生成
            effect_assign = effect_obj.GetComponent<SceneObjReferGameMainEffect>();
        }

        while (is_finish_download == false)
        {
            yield return null;
        }

        if (m_ReplaceAssetReference != null)
        {
            // エフェクトを差し替え
            if (m_IsUseEffect)
            {
                replaceEffect(ref effect_assign);
            }

            // ＳＥを差し替え
            replaceSound();
        }

        if (m_IsUseEffect)
        {
            SceneObjReferGameMain.Instance.setEffectAssignObj(effect_assign);
        }

        yield return null;

        m_SerialNo++;

        if (m_ReplaceAssetReference != null)
        {
            m_Status = Status.REPLACE_MODE;
        }
        else
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(false);
                m_AssetBundle = null;
            }

            m_Status = Status.NONE;
        }
    }

    /// <summary>
    /// エフェクトデータを差し替える
    /// </summary>
    /// <param name="effect_assign"></param>
    private SceneObjReferGameMainEffect replaceEffect(ref SceneObjReferGameMainEffect effect_assign)
    {
        if (m_ReplaceAssetReference == null)
        {
            return effect_assign;
        }

        if (effect_assign == null)
        {
            return effect_assign;
        }

        ReplaceAssetReferenceEffect effect_replace_info = m_ReplaceAssetReference.m_Effect;

        // エフェクトを差し替え
        if (effect_replace_info.m_NAUGHT_MA_00 != null)
        {
            effect_assign.m_Naught_MA_00 = effect_replace_info.m_NAUGHT_MA_00;
            effect_assign.m_Naught_MM_00 = effect_replace_info.m_NAUGHT_MA_00;
        }

        if (effect_replace_info.m_NAUGHT_MA_01 != null)
        {
            effect_assign.m_Naught_MA_01 = effect_replace_info.m_NAUGHT_MA_01;
            effect_assign.m_Naught_MM_01 = effect_replace_info.m_NAUGHT_MA_01;
        }

        if (effect_replace_info.m_NAUGHT_SA_00 != null)
        {
            effect_assign.m_Naught_SA_00 = effect_replace_info.m_NAUGHT_SA_00;
            effect_assign.m_Naught_SM_00 = effect_replace_info.m_NAUGHT_SA_00;
        }

        if (effect_replace_info.m_NAUGHT_SA_01 != null)
        {
            effect_assign.m_Naught_SA_01 = effect_replace_info.m_NAUGHT_SA_01;
            effect_assign.m_Naught_SM_01 = effect_replace_info.m_NAUGHT_SA_01;
        }

        if (effect_replace_info.m_NAUGHT_SA_02 != null)
        {
            effect_assign.m_Naught_SA_02 = effect_replace_info.m_NAUGHT_SA_02;
            effect_assign.m_Naught_SM_02 = effect_replace_info.m_NAUGHT_SA_02;
        }

        if (effect_replace_info.m_FIRE_MA_00 != null)
        {
            effect_assign.m_Fire_MA_00 = effect_replace_info.m_FIRE_MA_00;
            effect_assign.m_Fire_MM_00 = effect_replace_info.m_FIRE_MA_00;
        }

        if (effect_replace_info.m_FIRE_MA_01 != null)
        {
            effect_assign.m_Fire_MA_01 = effect_replace_info.m_FIRE_MA_01;
            effect_assign.m_Fire_MM_01 = effect_replace_info.m_FIRE_MA_01;
        }

        if (effect_replace_info.m_FIRE_SA_00 != null)
        {
            effect_assign.m_Fire_SA_00 = effect_replace_info.m_FIRE_SA_00;
            effect_assign.m_Fire_SM_00 = effect_replace_info.m_FIRE_SA_00;
        }

        if (effect_replace_info.m_FIRE_SA_01 != null)
        {
            effect_assign.m_Fire_SA_01 = effect_replace_info.m_FIRE_SA_01;
            effect_assign.m_Fire_SM_01 = effect_replace_info.m_FIRE_SA_01;
        }

        if (effect_replace_info.m_FIRE_SA_02 != null)
        {
            effect_assign.m_Fire_SA_02 = effect_replace_info.m_FIRE_SA_02;
            effect_assign.m_Fire_SM_02 = effect_replace_info.m_FIRE_SA_02;
        }

        if (effect_replace_info.m_WATER_MA_00 != null)
        {
            effect_assign.m_Water_MA_00 = effect_replace_info.m_WATER_MA_00;
            effect_assign.m_Water_MM_00 = effect_replace_info.m_WATER_MA_00;
        }

        if (effect_replace_info.m_WATER_MA_01 != null)
        {
            effect_assign.m_Water_MA_01 = effect_replace_info.m_WATER_MA_01;
            effect_assign.m_Water_MM_01 = effect_replace_info.m_WATER_MA_01;
        }

        if (effect_replace_info.m_WATER_SA_00 != null)
        {
            effect_assign.m_Water_SA_00 = effect_replace_info.m_WATER_SA_00;
            effect_assign.m_Water_SM_00 = effect_replace_info.m_WATER_SA_00;
        }

        if (effect_replace_info.m_WATER_SA_01 != null)
        {
            effect_assign.m_Water_SA_01 = effect_replace_info.m_WATER_SA_01;
            effect_assign.m_Water_SM_01 = effect_replace_info.m_WATER_SA_01;
        }

        if (effect_replace_info.m_WATER_SA_02 != null)
        {
            effect_assign.m_Water_SA_02 = effect_replace_info.m_WATER_SA_02;
            effect_assign.m_Water_SM_02 = effect_replace_info.m_WATER_SA_02;
        }

        if (effect_replace_info.m_LIGHT_MA_00 != null)
        {
            effect_assign.m_Light_MA_00 = effect_replace_info.m_LIGHT_MA_00;
            effect_assign.m_Light_MM_00 = effect_replace_info.m_LIGHT_MA_00;
        }

        if (effect_replace_info.m_LIGHT_MA_01 != null)
        {
            effect_assign.m_Light_MA_01 = effect_replace_info.m_LIGHT_MA_01;
            effect_assign.m_Light_MM_01 = effect_replace_info.m_LIGHT_MA_01;
        }

        if (effect_replace_info.m_LIGHT_SA_00 != null)
        {
            effect_assign.m_Light_SA_00 = effect_replace_info.m_LIGHT_SA_00;
            effect_assign.m_Light_SM_00 = effect_replace_info.m_LIGHT_SA_00;
        }

        if (effect_replace_info.m_LIGHT_SA_01 != null)
        {
            effect_assign.m_Light_SA_01 = effect_replace_info.m_LIGHT_SA_01;
            effect_assign.m_Light_SM_01 = effect_replace_info.m_LIGHT_SA_01;
        }

        if (effect_replace_info.m_LIGHT_SA_02 != null)
        {
            effect_assign.m_Light_SA_02 = effect_replace_info.m_LIGHT_SA_02;
            effect_assign.m_Light_SM_02 = effect_replace_info.m_LIGHT_SA_02;
        }

        if (effect_replace_info.m_DARK_MA_00 != null)
        {
            effect_assign.m_Dark_MA_00 = effect_replace_info.m_DARK_MA_00;
            effect_assign.m_Dark_MM_00 = effect_replace_info.m_DARK_MA_00;
        }

        if (effect_replace_info.m_DARK_MA_01 != null)
        {
            effect_assign.m_Dark_MA_01 = effect_replace_info.m_DARK_MA_01;
            effect_assign.m_Dark_MM_01 = effect_replace_info.m_DARK_MA_01;
        }

        if (effect_replace_info.m_DARK_SA_00 != null)
        {
            effect_assign.m_Dark_SA_00 = effect_replace_info.m_DARK_SA_00;
            effect_assign.m_Dark_SM_00 = effect_replace_info.m_DARK_SA_00;
        }

        if (effect_replace_info.m_DARK_SA_01 != null)
        {
            effect_assign.m_Dark_SA_01 = effect_replace_info.m_DARK_SA_01;
            effect_assign.m_Dark_SM_01 = effect_replace_info.m_DARK_SA_01;
        }

        if (effect_replace_info.m_DARK_SA_02 != null)
        {
            effect_assign.m_Dark_SA_02 = effect_replace_info.m_DARK_SA_02;
            effect_assign.m_Dark_SM_02 = effect_replace_info.m_DARK_SA_02;
        }

        if (effect_replace_info.m_WIND_MA_00 != null)
        {
            effect_assign.m_Wind_MA_00 = effect_replace_info.m_WIND_MA_00;
            effect_assign.m_Wind_MM_00 = effect_replace_info.m_WIND_MA_00;
        }

        if (effect_replace_info.m_WIND_MA_01 != null)
        {
            effect_assign.m_Wind_MA_01 = effect_replace_info.m_WIND_MA_01;
            effect_assign.m_Wind_MM_01 = effect_replace_info.m_WIND_MA_01;
        }

        if (effect_replace_info.m_WIND_SA_00 != null)
        {
            effect_assign.m_Wind_SA_00 = effect_replace_info.m_WIND_SA_00;
            effect_assign.m_Wind_SM_00 = effect_replace_info.m_WIND_SA_00;
        }

        if (effect_replace_info.m_WIND_SA_01 != null)
        {
            effect_assign.m_Wind_SA_01 = effect_replace_info.m_WIND_SA_01;
            effect_assign.m_Wind_SM_01 = effect_replace_info.m_WIND_SA_01;
        }

        if (effect_replace_info.m_WIND_SA_02 != null)
        {
            effect_assign.m_Wind_SA_02 = effect_replace_info.m_WIND_SA_02;
            effect_assign.m_Wind_SM_02 = effect_replace_info.m_WIND_SA_02;
        }

        if (effect_replace_info.m_HM_M_FIRE != null)
        {
            effect_assign.m_HM_M_Fire = effect_replace_info.m_HM_M_FIRE;
        }

        if (effect_replace_info.m_HM_M_WATER != null)
        {
            effect_assign.m_HM_M_Water = effect_replace_info.m_HM_M_WATER;
        }

        if (effect_replace_info.m_HM_M_WIND != null)
        {
            effect_assign.m_HM_M_Wind = effect_replace_info.m_HM_M_WIND;
        }

        if (effect_replace_info.m_HM_M_LIGHT != null)
        {
            effect_assign.m_HM_M_Light = effect_replace_info.m_HM_M_LIGHT;
        }

        if (effect_replace_info.m_HM_M_DARK != null)
        {
            effect_assign.m_HM_M_Dark = effect_replace_info.m_HM_M_DARK;
        }

        if (effect_replace_info.m_HM_M_NAUGHT != null)
        {
            effect_assign.m_HM_M_Naught = effect_replace_info.m_HM_M_NAUGHT;
        }

        if (effect_replace_info.m_HM_S_FIRE != null)
        {
            effect_assign.m_HM_S_Fire = effect_replace_info.m_HM_S_FIRE;
        }

        if (effect_replace_info.m_HM_S_WATER != null)
        {
            effect_assign.m_HM_S_Water = effect_replace_info.m_HM_S_WATER;
        }

        if (effect_replace_info.m_HM_S_WIND != null)
        {
            effect_assign.m_HM_S_Wind = effect_replace_info.m_HM_S_WIND;
        }

        if (effect_replace_info.m_HM_S_LIGHT != null)
        {
            effect_assign.m_HM_S_Light = effect_replace_info.m_HM_S_LIGHT;
        }

        if (effect_replace_info.m_HM_S_DARK != null)
        {
            effect_assign.m_HM_S_Dark = effect_replace_info.m_HM_S_DARK;
        }

        if (effect_replace_info.m_HM_S_NAUGHT != null)
        {
            effect_assign.m_HM_S_Naught = effect_replace_info.m_HM_S_NAUGHT;
        }

        if (effect_replace_info.m_HEAL_1 != null)
        {
            effect_assign.m_Heal_00 = effect_replace_info.m_HEAL_1;
            effect_assign.m_Heal_01 = effect_replace_info.m_HEAL_1;
            effect_assign.m_Heal_02 = effect_replace_info.m_HEAL_1;
            effect_assign.m_Heal_03 = effect_replace_info.m_HEAL_1;
        }

        if (effect_replace_info.m_SP_HEAL != null)
        {
            effect_assign.m_Heal_SP = effect_replace_info.m_SP_HEAL;
        }

        if (effect_replace_info.m_BLOOD != null)
        {
            effect_assign.m_Blood = effect_replace_info.m_BLOOD;
        }

        if (effect_replace_info.m_POISON != null)
        {
            effect_assign.m_Poison = effect_replace_info.m_POISON;
        }

        if (effect_replace_info.m_BUFF != null)
        {
            effect_assign.m_Buff = effect_replace_info.m_BUFF;
        }

        if (effect_replace_info.m_DEBUFF != null)
        {
            effect_assign.m_Debuff = effect_replace_info.m_DEBUFF;
        }

        if (effect_replace_info.m_ENEMY_SKILL_BUFF != null)
        {
            effect_assign.m_EnemySkillBuff = effect_replace_info.m_ENEMY_SKILL_BUFF;
        }

        if (effect_replace_info.m_ENEMY_SKILL_DEBUFF != null)
        {
            effect_assign.m_EnemySkillDebuff = effect_replace_info.m_ENEMY_SKILL_DEBUFF;
        }

        if (effect_replace_info.m_ENEMY_SKILL_FIRE != null)
        {
            effect_assign.m_EnemySkillFire00 = effect_replace_info.m_ENEMY_SKILL_FIRE;
        }

        if (effect_replace_info.m_ENEMY_SKILL_WIND != null)
        {
            effect_assign.m_EnemySkillWind00 = effect_replace_info.m_ENEMY_SKILL_WIND;
        }

        if (effect_replace_info.m_ENEMY_SKILL_WATER != null)
        {
            effect_assign.m_EnemySkillWater00 = effect_replace_info.m_ENEMY_SKILL_WATER;
        }

        if (effect_replace_info.m_ENEMY_SKILL_LIGHT != null)
        {
            effect_assign.m_EnemySkillLight00 = effect_replace_info.m_ENEMY_SKILL_LIGHT;
        }

        if (effect_replace_info.m_ENEMY_SKILL_DARK != null)
        {
            effect_assign.m_EnemySkillDark00 = effect_replace_info.m_ENEMY_SKILL_DARK;
        }

        if (effect_replace_info.m_ENEMY_SKILL_NAUGHT != null)
        {
            effect_assign.m_EnemySkillNaught00 = effect_replace_info.m_ENEMY_SKILL_NAUGHT;
        }

        if (effect_replace_info.m_ENEMY_SKILL_HEAL_S != null)
        {
            effect_assign.m_EnemySkillHeal_S = effect_replace_info.m_ENEMY_SKILL_HEAL_S;
        }

        if (effect_replace_info.m_ENEMY_SKILL_HEAL_M != null)
        {
            effect_assign.m_EnemySkillHeal_M = effect_replace_info.m_ENEMY_SKILL_HEAL_M;
        }

        if (effect_replace_info.m_PLAYER_SKILL_BUFF != null)
        {
            effect_assign.m_PlayerSkillBuff = effect_replace_info.m_PLAYER_SKILL_BUFF;
        }

        if (effect_replace_info.m_PLAYER_SKILL_DEBUFF != null)
        {
            effect_assign.m_PlayerSkillDebuff = effect_replace_info.m_PLAYER_SKILL_DEBUFF;
        }

        if (effect_replace_info.m_BATTLE_COST_PLUS != null)
        {
            effect_assign.m_2DBattleCostPlus = effect_replace_info.m_BATTLE_COST_PLUS;
        }

        if (effect_replace_info.m_UNIT_DROP != null)
        {
            effect_assign.m_2DUnitDrop = effect_replace_info.m_UNIT_DROP;
        }

        if (effect_replace_info.m_PLAYER_DAMAGE_NAUGHT != null)
        {
            effect_assign.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.NAUGHT] = effect_replace_info.m_PLAYER_DAMAGE_NAUGHT;
        }

        if (effect_replace_info.m_PLAYER_DAMAGE_FIRE != null)
        {
            effect_assign.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.FIRE] = effect_replace_info.m_PLAYER_DAMAGE_FIRE;
        }

        if (effect_replace_info.m_PLAYER_DAMAGE_WATER != null)
        {
            effect_assign.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.WATER] = effect_replace_info.m_PLAYER_DAMAGE_WATER;
        }

        if (effect_replace_info.m_PLAYER_DAMAGE_LIGHT != null)
        {
            effect_assign.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.LIGHT] = effect_replace_info.m_PLAYER_DAMAGE_LIGHT;
        }

        if (effect_replace_info.m_PLAYER_DAMAGE_DARK != null)
        {
            effect_assign.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.DARK] = effect_replace_info.m_PLAYER_DAMAGE_DARK;
        }

        if (effect_replace_info.m_PLAYER_DAMAGE_WIND != null)
        {
            effect_assign.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.WIND] = effect_replace_info.m_PLAYER_DAMAGE_WIND;
        }

        if (effect_replace_info.m_SKILL_COMP != null)
        {
            effect_assign.m_2DSkill = effect_replace_info.m_SKILL_COMP;
        }

        if (effect_replace_info.m_SKILL_SETUP_NAUGHT != null)
        {
            effect_assign.m_SkillSetupNaught = effect_replace_info.m_SKILL_SETUP_NAUGHT;
        }

        if (effect_replace_info.m_TURN_CHANGE != null)
        {
            effect_assign.m_2DTurnChange = effect_replace_info.m_TURN_CHANGE;
        }

        if (effect_replace_info.m_GAME_CLEAR != null)
        {
            effect_assign.m_2DGameClear = effect_replace_info.m_GAME_CLEAR;
        }

        if (effect_replace_info.m_ENEMY_EVOL1 != null)
        {
            effect_assign.m_2DEnemyEvol = effect_replace_info.m_ENEMY_EVOL1;
        }

        if (effect_replace_info.m_ENEMY_EVOL2 != null)
        {
            effect_assign.m_2DEnemyEvol2 = effect_replace_info.m_ENEMY_EVOL2;
        }

        if (effect_replace_info.m_ENEMY_DEATH != null)
        {
            effect_assign.m_EnemyDeath = effect_replace_info.m_ENEMY_DEATH;
        }

        if (effect_replace_info.m_ENEMT_DEATH_BOSS != null)
        {
            effect_assign.m_EnemyDeath_BOSS = effect_replace_info.m_ENEMT_DEATH_BOSS;
        }

        if (effect_replace_info.m_HAND_CARD_CHANGE != null)
        {
            effect_assign.m_HandCardTransform = effect_replace_info.m_HAND_CARD_CHANGE;
        }

        if (effect_replace_info.m_HAND_CARD_DESTROY != null)
        {
            effect_assign.m_HandCardDestroy = effect_replace_info.m_HAND_CARD_DESTROY;
        }

        return effect_assign;
    }

    /// <summary>
    /// SEを差し替え
    /// </summary>
    /// <param name="asset_bundle"></param>
    private void replaceSound()
    {
        if (m_ReplaceAssetReference == null)
        {
            return;
        }

        if (SoundManager.HasInstance)
        {
            SoundManager.Instance.clearReplaceSE();
        }

        AudioClip[][] replace_infos = new AudioClip[3][];
        for (int idx = 0; idx < 3; idx++)
        {
            ReplaceAssetReferenceSE replace_info = null;
            switch (idx)
            {
                case 0:
                    replace_info = m_ReplaceAssetReference.m_SE1;
                    break;

                case 1:
                    replace_info = m_ReplaceAssetReference.m_SE2;
                    break;

                case 2:
                    replace_info = m_ReplaceAssetReference.m_SE3;
                    break;
            }

            if (replace_info != null)
            {
                replace_infos[idx] = new AudioClip[(int)SEID.SE_MAX]
                {
                    null,
                    replace_info.m_SE_STARGE_START, //!< SE：システムSE：ジングル：ゲーム開始
                    replace_info.m_SE_STARGE_CLEAR, //!< SE：システムSE：ジングル：ゲームクリア
                    replace_info.m_SE_STAGE_CLEAR_UI,   //!< SE：システムSE：ジングル：ゲームクリアUI
                    replace_info.m_SE_STARGE_GAMEOVER,    //!< SE：システムSE：ジングル：ゲームオーバー
                    replace_info.m_SE_BATLE_WINDOW_OPEN,        //!< SE：バトルSE：バトルウィンドウ：開く
                    replace_info.m_SE_BATLE_WINDOW_CLOSE,      //!< SE：バトルSE：バトルウィンドウ：閉じる
                    replace_info.m_SE_BATLE_COST_PUT,      //!< SE：バトルSE：コストを追加成立
                    replace_info.m_SE_BATLE_SKILL_EXEC,      //!< SE：バトルSE：スキルが発動
                    replace_info.m_SE_BATLE_SKILL_REPLACE,    //!< SE：バトルSE：スキルが置き換えられた
                    replace_info.m_SE_BATLE_ATTACK_EN_NORMAL,      //!< SE：バトルSE：攻撃音：敵側：デフォルト
                    replace_info.m_SE_BATLE_ATTACK_PC_NORMAL,      //!< SE：バトルSE：攻撃音：PC側：デフォルト
                    replace_info.m_SE_BATLE_COUNTDOWN_1,        //!< SE：システムSE：カウントダウン：1
                    replace_info.m_SE_BATLE_COUNTDOWN_2,        //!< SE：システムSE：カウントダウン：2
                    replace_info.m_SE_BATLE_COUNTDOWN_3,        //!< SE：システムSE：カウントダウン：3
                    replace_info.m_SE_BATLE_COUNTDOWN_4,        //!< SE：システムSE：カウントダウン：4
                    replace_info.m_SE_BATLE_COUNTDOWN_5,        //!< SE：システムSE：カウントダウン：5
                    replace_info.m_SE_BATLE_COUNTDOWN_6,        //!< SE：システムSE：カウントダウン：6
                    replace_info.m_SE_BATLE_COUNTDOWN_7,        //!< SE：システムSE：カウントダウン：7
                    replace_info.m_SE_BATLE_COUNTDOWN_8,        //!< SE：システムSE：カウントダウン：8
                    replace_info.m_SE_BATLE_COUNTDOWN_9,        //!< SE：システムSE：カウントダウン：9
                    replace_info.m_SE_BATLE_COST_PLUS_1,        //!< SE：システムSE：コスト吸着
                    replace_info.m_SE_BATLE_COST_PLUS_2,        //!< SE：システムSE：コスト吸着
                    replace_info.m_SE_BATLE_COST_IN,        //!< SE：システムSE：コスト配り
                    replace_info.m_SE_BATLE_SKILL_HANDS,        //!< SE：システムSE：～HANDSの音
                    replace_info.m_SE_BATLE_SKILL_CUTIN,        //!< SE：システムSE：スキルカットイン
                    replace_info.m_SE_BATLE_SKILL_CAPTION,    //!< SE：システムSE：スキルキャプション出現音
                    replace_info.m_SE_BATLE_SKILL_LIMITBREAK_CUTIN,      //!< SE：システムSE：LBSカットイン
                    replace_info.m_SE_BATLE_SKILL_LIMITBREAK_IMPACT,        //!< SE：システムSE：LBSインパクト
                    replace_info.m_SE_BATLE_ENEMY_TURN,      //!< SE：システムSE：敵ターン経過
                    replace_info.m_SE_BATLE_UI_OPEN,            //!< SE：システムSE：戦闘ウィンドウ開く
                    replace_info.m_SE_INGAME_PANEL_SELECT,    //!< SE：インゲームSE：パネル操作：選択音
                    replace_info.m_SE_INGAME_PANEL_MEKURI,    //!< SE：インゲームSE：パネル操作：めくり
                    replace_info.m_SE_INGAME_PANEL_MEKURI_NORMAL,      //!< SE:インゲームSE：パネル操作：めくり経過通常
                    replace_info.m_SE_INGAME_PANEL_MEKURI_SPECIAL,    //!< SE:インゲームSE：パネル操作：めくり経過特殊
                    replace_info.m_SE_INGAME_PANEL_SHOCK,      //!< SE：インゲームSE：パネル操作：叩きつけ音
                    replace_info.m_SE_INGAME_PANEL_MEKURI_S,        //!< SE：インゲームSE：パネル操作：めくり（無地パネル用）
                    replace_info.m_SE_INGAME_ACTIVITY_KEY,    //!< SE：インゲームSE：パネル発動音：鍵
                    replace_info.m_SE_INGAME_ACTIVITY_ITEM,      //!< SE：インゲームSE：パネル発動音：宝箱
                    replace_info.m_SE_INGAME_ACTIVITY_TRAP,      //!< SE：インゲームSE：パネル発動音：虎ばさみ
                    replace_info.m_SE_INGAME_ACTIVITY_BOMB,      //!< SE：インゲームSE：パネル発動音：地雷
                    replace_info.m_SE_INGAME_ACTIVITY_PITFALL,    //!< SE：インゲームSE：パネル発動音：落とし穴
                    replace_info.m_SE_INGAME_ACTIVITY_ENEMY,        //!< SE：インゲームSE：パネル発動音：敵
                    replace_info.m_SE_INGAME_ACTIVITY_TICKET,      //!< SE：インゲームSE：パネル発動音：チケット
                    replace_info.m_SE_INGAME_DOOR_OPEN,      //!< SE：インゲームSE：ドア作動音：開く
                    replace_info.m_SE_INGAME_DOOR_BOSS_TATAKI,    //!< SE：インゲームSE：ドア作動音：ボスドア：叩く
                    replace_info.m_SE_INGAME_DOOR_BOSS_OPEN,        //!< SE：インゲームSE：ドア作動音：ボスドア：開く
                    replace_info.m_SE_INGAME_PATH_PLUS,      //!< SE：インゲームSE：パス追加音
                    replace_info.m_SE_INGAME_QUEST_START_00,        //!< SE:インゲームSE：ReadyTo
                    replace_info.m_SE_INGAME_QUEST_START_01,        //!< SE:インゲームSE：MoveOn
                    replace_info.m_SE_INGAME_QUEST_START_02,        //!< SE:インゲームSE:UI退場音
                    replace_info.m_SE_MENU_OK,    //!< SE：メニューSE：肯定
                    replace_info.m_SE_MENU_OK2,      //!< SE：メニューSE：肯定（強）
                    replace_info.m_SE_MENU_NG,    //!< SE：メニューSE：否定
                    replace_info.m_SE_MENU_RET,      //!< SE：メニューSE：戻る
                    replace_info.m_SE_MAINMENU_BLEND_SELECT,        //!< SE：メインメニューSE：合成選択
                    replace_info.m_SE_MAINMENU_BLEND_SELECT_NG,      //!< SE：メインメニューSE：合成選択失敗
                    replace_info.m_SE_MAINMENU_BLEND_EXEC,    //!< SE：メインメニューSE：合成実行
                    replace_info.m_SE_MAINMENU_BLEND_CLEAR,      //!< SE：メインメニューSE：合成選択クリア
                    replace_info.m_SE_BATTLE_ENEMYDEATH,        //!< SE：バトルSE：敵死亡
                    replace_info.m_SE_BATTLE_SKILL_HEAL,        //!< SE：バトルSE：スキルによるHP回復
                    replace_info.m_SE_TITLE_CALL_W,      //!< SE：タイトルコール：女
                    replace_info.m_SE_TITLE_CALL_M,      //!< SE：タイトルコール：男 4.0.0ファイルがない // 番号がずれると問題があるので復活させました
                    replace_info.m_SE_SHUTTER_OPEN,      //!< SE：シャッター：開く
                    replace_info.m_SE_SHUTTER_CLOSE,        //!< SE：シャッター：閉じる
                    replace_info.m_SE_TRAP_CANCEL,    //!< SE：罠解除
                    replace_info.m_SE_TRAP_LUCK,        //!< SE：よい効果
                    replace_info.m_SE_TRAP_BAD,      //!< SE：わるい効果
                    replace_info.m_SE_BATTLE_ATTACK_FIRE,      //!< SE：炎属性攻撃
                    replace_info.m_SE_BATTLE_ATTACK_WATER,    //!< SE：水属性攻撃
                    replace_info.m_SE_BATTLE_ATTACK_WIND,      //!< SE：風属性攻撃
                    replace_info.m_SE_BATTLE_ATTACK_NAUGHT,      //!< SE：無属性攻撃
                    replace_info.m_SE_BATTLE_ATTACK_LIGHT,    //!< SE：光属性攻撃
                    replace_info.m_SE_BATTLE_ATTACK_DARK,      //!< SE：闇属性攻撃
                    replace_info.m_SE_BATTLE_ATTACK_HEAL,      //!< SE：回復属性攻撃
                    replace_info.m_SE_BATTLE_BOSS_ALERT,        //!< SE：ボスアラート
                    replace_info.m_SE_BATTLE_BOSS_APPEAR,      //!< SE：ボス登場
                    replace_info.m_SE_BATTLE_ATTACK_FIRST,    //!< SE：先制攻撃
                    replace_info.m_SE_BATTLE_ATTACK_BACK,      //!< SE：不意打ち攻撃
                    replace_info.m_SE_BATTLE_BUFF,    //!< SE：BUFFスキル
                    replace_info.m_SE_BATTLE_DEBUFF,        //!< SE：DEBUFFスキル
                    replace_info.m_SE_INGAME_LEADERSKILL,      //!< SE：リーダースキルパワーアップ
                    replace_info.m_SE_SKILL_COMBO_00,      //!< SE:スキルコンボ：00
                    replace_info.m_SE_SKILL_COMBO_01,      //!< SE:スキルコンボ：01
                    replace_info.m_SE_SKILL_COMBO_02,      //!< SE:スキルコンボ：02
                    replace_info.m_SE_SKILL_COMBO_03,      //!< SE:スキルコンボ：03
                    replace_info.m_SE_SKILL_COMBO_04,      //!< SE:スキルコンボ：04
                    replace_info.m_SE_SKILL_COMBO_05,      //!< SE:スキルコンボ：05
                    replace_info.m_SE_SKILL_COMBO_06,      //!< SE:スキルコンボ：06
                    replace_info.m_SE_SKILL_COMBO_07,      //!< SE:スキルコンボ：07
                    replace_info.m_SE_SKILL_COMBO_08,      //!< SE:スキルコンボ：08
                    replace_info.m_SE_SKILL_COMBO_MORE_THAN_08,    //!< SE:スキルコンボ：09
                    replace_info.m_SE_SKILL_COMBO_FINISH_WORD,    //!< SE：スキルコンボ：フィニッシュ
                    replace_info.m_SE_CHESS_FALL,      //!< SE:チェス駒：落下音
                    replace_info.m_SE_CHESS_MOVE,      //!< SE:チェス駒：移動音
                    replace_info.m_SE_DOOR_OPEN_NORMAL,      //!< SE:チェス駒：ドア開き：ノーマル
                    replace_info.m_SE_DOOR_OPEN_BOSS,      //!< SE:チェス駒：ドア開き：ボス
                    replace_info.m_SE_SPLIMITOVER,    //!< SE:SP切れUI

                    replace_info.m_SE_MM_A01_CHECK,      //!< SE:全体リソース：決定音
                    replace_info.m_SE_MM_A02_CHECK2,        //!< SE:全体リソース：決定音（大）
                    replace_info.m_SE_MM_A03_TAB,      //!< SE:全体リソース：タブ切り替え
                    replace_info.m_SE_MM_A04_BACK,    //!< SE:全体リソース：キャンセル(バック)音
                    replace_info.m_SE_MM_B01_EXP_GAUGE,      //!< SE:クエストリザルト：ゲージが伸びる音
                    replace_info.m_SE_MM_B02_RANKUP,        //!< SE:クエストリザルト：ランクアップ
                    replace_info.m_SE_MM_B04_RARE_START,        //!< SE:クエストリザルト：ユニットゲットレア度のUI
                    replace_info.m_SE_MM_B05_RARE_STAR_PUT,      //!< SE:クエストリザルト：星のはまる音
                    replace_info.m_SE_MM_B06_RARE_END,    //!< SE:クエストリザルト：レア度UIの最後に
                    replace_info.m_SE_MM_C01_SCRATCH_1_3,      //!< SE:スクラッチ：☆１～３ゲット
                    replace_info.m_SE_MM_C02_SCRATCH_4,      //!< SE:スクラッチ：☆４ゲット
                    replace_info.m_SE_MM_C03_SCRATCH_5_6,      //!< SE:スクラッチ：☆５～６ゲット
                    replace_info.m_SE_MM_C04_SCRATCH_RARE,    //!< SE:スクラッチ：レアめくり
                    replace_info.m_SE_MM_D01_FRIEND_UNIT,      //!< SE:強化合成：フレンドユニット
                    replace_info.m_SE_MM_D02_MATERIAL_UNIT,      //!< SE:強化合成：マテリアルユニット
                    replace_info.m_SE_MM_D04_LEVEL_UP,    //!< SE:強化合成：レベルアップ
                    replace_info.m_SE_MM_D09_EVOLVE_ROLL,      //!< SE:進化合成：演出回転
                    replace_info.m_SE_MM_D10_EVOLVE_COMP,      //!< SE:進化合成：進化後遷移
                    replace_info.m_SE_MM_D08_SALE,    //!< SE:売却：売却演出音

                    replace_info.m_VOICE_INGAME_QUEST_READYTO,    //!< Voice:A:ReadyTo
                    replace_info.m_VOICE_INGAME_QUEST_MOVEON,      //!< Voice:A:MoveOn
                    replace_info.m_VOICE_INGAME_QUEST_BOSSAPPEAR,      //!< Voice:A:BossAppear
                    replace_info.m_VOICE_INGAME_QUEST_QUESTCLEAR,      //!< Voice:A:QuestClear
                    replace_info.m_VOICE_INGAME_QUEST_GAMEOVER,      //!< Voice:A:GameOver
                    replace_info.m_VOICE_INGAME_QUEST_GETKEY,      //!< Voice:A:GETKEY
                    replace_info.m_VOICE_INGAME_QUEST_NICE,      //!< Voice:A:戦闘評価：NICE
                    replace_info.m_VOICE_INGAME_QUEST_GREAT,        //!< Voice:A:戦闘評価：GREAT
                    replace_info.m_VOICE_INGAME_QUEST_BEAUTY,      //!< Voice:A:戦闘評価：BEAUTY
                    replace_info.m_VOICE_INGAME_QUEST_EXCELLENT,        //!< Voice:A:戦闘評価：EXCELLENT
                    replace_info.m_VOICE_INGAME_QUEST_COOL,      //!< Voice:A:戦闘評価：COOL
                    replace_info.m_VOICE_INGAME_QUEST_UNBELIEVABLE,      //!< Voice:A:戦闘評価：UNBELIEVABLE
                    replace_info.m_VOICE_INGAME_QUEST_MARVELOUS,        //!< Voice:A:戦闘評価：MARVELOUS
                    replace_info.m_VOICE_INGAME_QUEST_DIVINE,      //!< Voice:A:戦闘評価：DIVINE
                    replace_info.m_VOICE_INGAME_QUEST_FIRSTATTACK,    //!< Voice:A：FIRSTATTACK
                    replace_info.m_VOICE_INGAME_QUEST_BACKATTACK,      //!< Voice:A：BACKATTACK
                    replace_info.m_VOICE_INGAME_QUEST_HANDCARD_SET,      //!< Voice:A:手札配り
                    replace_info.m_VOICE_INGAME_QUEST_STANDREADY,      //!< Voice:A:LBS発動
                    replace_info.m_VOICE_INGAME_QUEST_SPLIMIT,    //!< Voice:A:SPLimit

                    replace_info.m_VOICE_INGAME_MM_EVOLVE,    //!< Voice:A:進化
                    replace_info.m_VOICE_INGAME_MM_FOOT_FRIEND,      //!< Voice:A:フッタ：フレンド
                    replace_info.m_VOICE_INGAME_MM_FOOT_OTHERS,      //!< Voice:A:フッタ：その他
                    replace_info.m_VOICE_INGAME_MM_FOOT_QUEST,    //!< Voice:A:フッタ：クエスト
                    replace_info.m_VOICE_INGAME_MM_FOOT_SCRATCH,        //!< Voice:A:フッタ：スクラッチ
                    replace_info.m_VOICE_INGAME_MM_FOOT_SHOP,      //!< Voice:A:フッタ：ショップ
                    replace_info.m_VOICE_INGAME_MM_FOOT_UNIT,      //!< Voice:A:フッタ：ユニット
                    replace_info.m_VOICE_INGAME_MM_LEVELUP,      //!< Voice:A:レベルアップ
                    replace_info.m_VOICE_INGAME_MM_RANKUP,    //!< Voice:A:ランクアップ
                    replace_info.m_VOICE_INGAME_MM_SKILLUP,      //!< Voice:A:スキルアップ
                    replace_info.m_VOICE_INGAME_MM_UNIT_GET,        //!< Voice:A:ユニット取得：
                    replace_info.m_VOICE_INGAME_MM_UNIT_GET_1,    //!< Voice:A:ユニット取得：レア１
                    replace_info.m_VOICE_INGAME_MM_UNIT_GET_2,    //!< Voice:A:ユニット取得：レア２
                    replace_info.m_VOICE_INGAME_MM_UNIT_GET_3,    //!< Voice:A:ユニット取得：レア３
                    replace_info.m_VOICE_INGAME_MM_UNIT_GET_4,    //!< Voice:A:ユニット取得：レア４
                    replace_info.m_VOICE_INGAME_MM_UNIT_GET_5,    //!< Voice:A:ユニット取得：レア５
                    replace_info.m_VOICE_INGAME_MM_UNIT_GET_6,    //!< Voice:A:ユニット取得：レア６
                    replace_info.m_VOICE_INGAME_MM_UNIT_GET_7,    //!< Voice:A:ユニット取得：レア７

                    replace_info.m_VOICE_INGAME_MM_LINK_ON,      //!< Voice:A:リンクオン
                    replace_info.m_VOICE_INGAME_MM_HOME,        //!< Voice:A:ホーム
                    replace_info.m_VOICE_INGAME_MM_LIMITOVER,      //!< Voice:A:リミットオーバー
                };
            }
            else
            {
                replace_infos[idx] = new AudioClip[(int)SEID.SE_MAX];
            }
        }

        for (int se_idx = 0; se_idx < (int)SEID.SE_MAX; se_idx++)
        {
            SEID se_id = (SEID)se_idx;

            AudioClip[] audio_clips = new AudioClip[replace_infos.Length];
            int count = 0;
            for (int idx = 0; idx < replace_infos.Length; idx++)
            {
                AudioClip audio_clip = replace_infos[idx][(int)se_id];
                if (audio_clip != null)
                {
                    audio_clips[count] = audio_clip;
                    count++;
                }
            }

            if (count > 0)
            {
                AudioClip[] audio_clips2 = new AudioClip[count];
                for (int idx = 0; idx < audio_clips2.Length; idx++)
                {
                    audio_clips2[idx] = audio_clips[idx];
                }

                SoundManager.Instance.setReplaceSE(se_id, audio_clips2);
            }
        }
    }

    /// <summary>
    /// 差し替えスプライトを取得する
    /// </summary>
    /// <param name="sprite_name"></param>
    /// <param name="dest_sprite"></param>
    /// <returns></returns>
    public bool getReplaceSprite(string sprite_name, out Sprite dest_sprite)
    {
        dest_sprite = null;
        if (m_Status == Status.REPLACE_MODE
            && m_ReplaceAssetReference != null
            && m_ReplaceAssetReference.m_Sprite != null
            && m_ReplaceAssetReference.m_Sprite.m_SpriteReplaceInfo.IsNullOrEmpty() == false
        )
        {
            ReplaceAssetReferenceSprite.SpriteReplaceInfo[] infos = m_ReplaceAssetReference.m_Sprite.m_SpriteReplaceInfo;
            for (int idx = 0; idx < infos.Length; idx++)
            {
                ReplaceAssetReferenceSprite.SpriteReplaceInfo info = infos[idx];
                if (info != null)
                {
                    if (sprite_name == info.m_BaseName)
                    {
                        dest_sprite = info.m_Sprite;
                        break;
                    }
                }
            }
        }

        return (dest_sprite != null);
    }

#if BUILD_TYPE_DEBUG
    /// <summary>
    /// ローカルのStreamingAssetsに置いたアセットバンドルを読む（テスト用）
    /// </summary>
    /// <param name="asset_bundle_name"></param>
    /// <returns></returns>
    public WWW debug_LoadLocalAssetBundle(string asset_bundle_name)
    {
        string scheme = "";
        string platform = "";

#if UNITY_EDITOR
        platform = "Android";
#if UNITY_IOS
        platform = "iOS";
#endif //UNITY_IOS
        scheme = "file:///";
#elif UNITY_ANDROID
        platform = "Android";
        scheme = "";
#elif UNITY_IOS
        platform = "iOS";
        scheme = "file:///";
#else
        platform = "Android";
        scheme = "file:///";
#endif

        string url = scheme + Application.streamingAssetsPath + "/" + platform.ToString() + "/" + asset_bundle_name;

        WWW www = new WWW(url);

        return www;
    }
#endif
}
