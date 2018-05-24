using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerPartyViewControl : MonoBehaviour
{
    public GameObject m_MemberDetailPrefab = null;
    public GameObject[] m_MemberLocates = new GameObject[5];
    public GameObject m_PartyEffect = null;
    public GameObject m_PartySP = null;

    private Transform[] m_MemberHPTrans = new Transform[5];
    private Transform[] m_MemberEffectTrans = new Transform[5];

    private int m_SP = 0;
    private bool m_IsSpChangeEffect = false;

    private const float INTERFACE_EXTENTION_DAMAGE_VALUE_OFFSET_Y = 0.79f;  // インターフェイス拡張時のダメージ数値表示位置の調整量

    // Use this for initialization
    void Start()
    {
        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            Transform member_trans = m_MemberLocates[idx].transform;
            GameObject obj = Instantiate(m_MemberDetailPrefab);
            obj.transform.SetParent(member_trans, false);
            m_MemberHPTrans[idx] = obj.transform.Find("HP");
            m_MemberEffectTrans[idx] = obj.transform.Find("Effect");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (BattleParam.IsKobetsuHP
            && BattleParam.m_PlayerParty != null)
        {
            BattleSceneUtil.MultiInt damage_value = BattleParam.m_PlayerParty.getDispDamageValue();
            BattleSceneUtil.MultiInt damage_target = BattleParam.m_PlayerParty.getDispDamageTarget();
            MasterDataDefineLabel.ElementType damage_element = BattleParam.m_PlayerParty.getDispDamageElement();
            BattleSceneUtil.MultiInt heal_value = BattleParam.m_PlayerParty.getDispRecoveryValue();

            // ダメージ表示
            setDamageValueMember(damage_element, damage_value, damage_target);

            // SP回復or消費
            if (m_SP != BattleParam.m_PlayerParty.m_PartyTotalSP)
            {
                m_IsSpChangeEffect = true;
            }
            m_SP = BattleParam.m_PlayerParty.m_PartyTotalSP;

            if (m_IsSpChangeEffect)
            {
                if (BattleSkillCutinManager.Instance.isRunning() == false)
                {
                    EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_SP, Vector3.zero, Vector3.zero, m_PartySP.transform, null, 1.0f);
                    m_IsSpChangeEffect = false;
                }
            }

            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                // HP回復
                if (heal_value != null)
                {
                    Transform chara_hp_trans = m_MemberHPTrans[idx];
                    int hp_delta = heal_value.getValue((GlobalDefine.PartyCharaIndex)idx);
                    if (hp_delta > 0)
                    {
                        DrawDamageManager.showDamage(chara_hp_trans, hp_delta, EDAMAGE_TYPE.eDAMAGE_TYPE_HEAL, 3.0f, getInterfaceExtentionRate() * INTERFACE_EXTENTION_DAMAGE_VALUE_OFFSET_Y - 0.65f);
                        setEffectHeal((GlobalDefine.PartyCharaIndex)idx);
                    }
                }
            }
        }
    }

    /// <summary>
    /// パーティメンバー個別へのダメージ演出（ダメージ数値）
    /// </summary>
    /// <param name="member_index"></param>
    /// <param name="effect_prefab"></param>
    public void setDamageValueMember(MasterDataDefineLabel.ElementType damage_element, BattleSceneUtil.MultiInt damage_value, BattleSceneUtil.MultiInt damage_target)
    {
        if (BattleParam.IsKobetsuHP == false)
        {
            return;
        }

        for (int idx = 0; idx < damage_value.getMemberCount(); idx++)
        {
            int dmg_target = damage_target.getValue((GlobalDefine.PartyCharaIndex)idx);
            if (dmg_target > 0)
            {
                int dmg_value = damage_value.getValue((GlobalDefine.PartyCharaIndex)idx);
                EDAMAGE_TYPE damage_type = EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL;
                if (damage_element != MasterDataDefineLabel.ElementType.NONE)
                {
                    CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
                    if (chara_once != null)
                    {
                        damage_type = InGameUtilBattle.GetSkillElementAffinity(damage_element, chara_once.m_CharaMasterDataParam.element);
                    }
                }

                DrawDamageManager.showDamage(m_MemberHPTrans[idx], dmg_value, damage_type, 3.0f, getInterfaceExtentionRate() * INTERFACE_EXTENTION_DAMAGE_VALUE_OFFSET_Y + 0.14f, 2.0f);
            }
        }
    }

    /// <summary>
    /// パーティメンバー個別へのダメージ演出（エフェクト再生）
    /// </summary>
    /// <param name="member_index"></param>
    /// <param name="effect_prefab"></param>
    public void setDamageEffectMember(GameObject effect_prefab, BattleSceneUtil.MultiInt damage_target)
    {
        if (effect_prefab == null)
        {
            return;
        }

        if (BattleParam.IsKobetsuHP == false)
        {
            if (damage_target.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0)
            {
                setDamageEffectParty(effect_prefab);
            }
            return;
        }

        for (int idx = 0; idx < damage_target.getMemberCount(); idx++)
        {
            int dmg_target = damage_target.getValue((GlobalDefine.PartyCharaIndex)idx);
            if (dmg_target > 0)
            {
                EffectManager.Instance.playEffect(effect_prefab, Vector3.zero, Vector3.zero, m_MemberEffectTrans[idx], null, 1.0f);
            }
        }
    }

    /// <summary>
    /// パーティ全体へのダメージ演出
    /// </summary>
    /// <param name="effect_prefab"></param>
    public void setDamageEffectParty(GameObject effect_prefab)
    {
        if (effect_prefab != null)
        {
            EffectManager.Instance.playEffect(effect_prefab, new Vector3(-2.6f, 1.6f, 0.0f), Vector3.zero, m_PartyEffect.transform, null, 12.0f);
        }
    }

    public void setEffectParty(GameObject effect_prefab)
    {
        EffectManager.Instance.playEffect(effect_prefab, Vector3.zero, Vector3.zero, m_PartyEffect.transform, null, 8.0f);
    }

    public void setEffectHeal(GlobalDefine.PartyCharaIndex player_index, int heal_level = 3)
    {
        Transform chara_hp_trans = m_MemberHPTrans[(int)player_index];
        GameObject effect_prefab = null;
        switch (heal_level)
        {
            case 0:
                effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_00;
                break;

            case 1:
                effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_01;
                break;

            case 2:
                effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_02;
                break;

            case 3:
                effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_03;
                break;
        }

        if (effect_prefab != null)
        {
            EffectManager.Instance.playEffect(effect_prefab, new Vector3(1.0f, getInterfaceExtentionRate() * 0.7f - 2.7f, 0.0f), Vector3.zero, chara_hp_trans, null, 3.0f);
        }
    }

    public Transform getEffectTransform(GlobalDefine.PartyCharaIndex index)
    {
        return m_MemberEffectTrans[(int)index];
    }

    /// <summary>
    /// パーティインターフェイスが拡張中かどうか
    /// </summary>
    /// <returns></returns>
    private float getInterfaceExtentionRate()
    {
        return BattleParam.isShowPartyInterfaceSkillCost() ? 1.0f : 0.0f;
    }
}
