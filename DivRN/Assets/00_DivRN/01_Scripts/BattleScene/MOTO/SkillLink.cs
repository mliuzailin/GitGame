/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	SkillLink.cs
    @brief	リンクスキルクラス
    @author Developer
    @date 	2015/08/28
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
    @brief	リンクシステムクラス
*/
//----------------------------------------------------------------------------
public class SkillLink
{
    public const int SKILL_LINK_MAX = (5);      // リンクスキル最大発動数

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public SkillRequestParam m_SkillRequestLink = new SkillRequestParam(SKILL_LINK_MAX);                        //!< スキル発動リクエスト管理
    private bool[] m_SkillOwner = new bool[(int)GlobalDefine.PartyCharaIndex.MAX];   //!< リンクスキル

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    public int linkReqInputNum
    {
        get
        {
            return (m_SkillRequestLink.getRequestCount());
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		リンクスキル発行情報リセット
    */
    //----------------------------------------------------------------------------
    public void ResetSkillRequestLink()
    {
        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (m_SkillRequestLink == null)
        {
            return;
        }

        m_SkillRequestLink.clearRequest();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	戦闘処理ステップ：ゲーム：リンクスキルのカットイン処理
    */
    //----------------------------------------------------------------------------
    public void CutinStart()
    {
        if (m_SkillRequestLink == null
        || m_SkillRequestLink.getRequestCount() <= 0
        )
        {
            Debug.LogError("LinkSkillActivity Is None!");
            return;
        }

        BattleSkillCutinManager.Instance.ClrSkillCutin();
        BattleSkillCutinManager.Instance.SetSkillCutin(m_SkillRequestLink);
        BattleSkillCutinManager.Instance.CutinStart2(true); //リンクスキル
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		リンクスキルの最大発動数をチェック
        @param[in]	BattleSkillActivity[]	(activityArray)	ノーマルスキル発動情報
    */
    //----------------------------------------------------------------------------
    private static int CheckLinkSkillNum(SkillRequestParam skill_requeset_param, bool[] dest_skill_owner)
    {
        int ret_val = 0;
        for (int idx = 0; idx < dest_skill_owner.Length; idx++)
        {
            dest_skill_owner[idx] = false;
        }

        //--------------------------------
        // ノーマルスキル発動者を取得
        //--------------------------------
        for (int num = 0; num < skill_requeset_param.getRequestCount(); ++num)
        {
            BattleSkillActivity normalActivity = skill_requeset_param.getSkillRequestByIndex(num);
            if (normalActivity == null
            || normalActivity.m_SkillType != ESKILLTYPE.eACTIVE
            || normalActivity.m_SkillParamOwnerNum == GlobalDefine.PartyCharaIndex.GENERAL)
            {
                continue;
            }

            // 発動者の登録処理
            for (int ownerNum = 0; ownerNum < dest_skill_owner.Length; ++ownerNum)
            {
                if ((GlobalDefine.PartyCharaIndex)ownerNum == normalActivity.m_SkillParamOwnerNum
                && dest_skill_owner[ownerNum] == false)
                {
                    dest_skill_owner[ownerNum] = true;
                    break;
                }
            }

        }


        //--------------------------------
        // リンクスキル発動判定
        //--------------------------------
        for (int num = 0; num < dest_skill_owner.Length; ++num)
        {
            if (dest_skill_owner[num] == false)
            {
                continue;
            }

            // リンクキャラを取得
            CharaOnce partyChara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_ACTIVE);
            if (partyChara == null)
            {
                continue;
            }

            MasterDataParamChara charaMaster = BattleParam.m_MasterDataCache.useCharaParam(partyChara.m_LinkParam.m_CharaID);
            if (charaMaster == null)
            {
                dest_skill_owner[num] = false;
                continue;
            }

            // リンクスキル発動判定(基本発動率 + リンクポイント)
            uint odds = CharaLinkUtil.GetLinkSkillOdds(charaMaster, partyChara.m_LinkParam.m_CharaLinkPoint);
            if (BattleSceneUtil.checkChancePercentSkill((int)odds, (int)CharaLinkUtil.SKILL_LINK_ODDS_MAX))
            {
                ret_val++;
            }
            else
            {
                dest_skill_owner[num] = false;
            }
        }

        return ret_val;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		アクティブスキルの発動情報からリンクスキルの発動情報作成
    */
    //----------------------------------------------------------------------------
    public void ActivityLinkSkill(SkillRequestParam active_skill_request_param)
    {
        m_SkillRequestLink.clearRequest();

        //--------------------------------
        // スキル情報の選定完了
        //--------------------------------
        int link_skill_num = CheckLinkSkillNum(active_skill_request_param, m_SkillOwner);
        if (link_skill_num <= 0)
        {
            return;
        }

        //--------------------------------
        // リンクスキル発動情報設定
        //--------------------------------
        int nSkillPower = 0;
        uint unRandMin = 0;
        uint unRandMax = 0;

        for (int num = 0; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
        {
            if (m_SkillOwner[num] == false)
            {
                continue;
            }

            // リンクキャラを取得
            CharaOnce baseChara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_ACTIVE);
            MasterDataParamChara linkCharaParam = BattleParam.m_MasterDataCache.useCharaParam(baseChara.m_LinkParam.m_CharaID);
            if (linkCharaParam == null)
            {
                continue;
            }

            // リンクキャラのリンクスキルを取得
            MasterDataSkillActive skillActiveParam = BattleParam.m_MasterDataCache.useSkillActive(linkCharaParam.link_skill_active);
            if (skillActiveParam == null)
            {
                continue;
            }

            // リンクスキルを構築
            BattleSkillActivity skillActivity = new BattleSkillActivity();
            if (skillActivity != null)
            {
                // 共通情報を設定
                skillActivity.m_SkillParamOwnerNum = (GlobalDefine.PartyCharaIndex)num;                                 // 発動者(パーティキャラ)
                skillActivity.m_SkillParamFieldID = InGameDefine.SELECT_NONE;               // フィールドID(ブーストパネルの影響は受けない)
                skillActivity.m_SkillParamSkillID = skillActiveParam.fix_id;                // ノーマルスキルID
                skillActivity.m_SkillType = ESKILLTYPE.eLINK;                       // 発動スキルの種類

                skillActivity.m_Element = skillActiveParam.skill_element;       // 基本情報：属性
                skillActivity.m_Type = skillActiveParam.skill_type;         // 基本情報：スキルタイプ
                skillActivity.m_Effect = skillActiveParam.effect;               // 基本情報：エフェクト

                skillActivity.m_skill_chk_atk_combo = MasterDataDefineLabel.BoolType.ENABLE;    // 攻撃情報：攻撃側：コンボレートの影響

                // @change Developer v320 リンクスキル振れ幅対応
                if (skillActiveParam.Is_skill_active())
                {
                    // 振れ幅が設定してある場合
                    if (skillActiveParam.skill_value_rand != 0)
                    {
                        nSkillPower = skillActiveParam.skill_value + skillActiveParam.skill_value_rand;
                        if (nSkillPower < 0)
                        {
                            nSkillPower = 0;
                        }

                        // 最小値と最大値を確定：基準値より高い場合
                        if (nSkillPower > skillActiveParam.skill_value)
                        {
                            unRandMin = (uint)skillActiveParam.skill_value;
                            unRandMax = (uint)nSkillPower;
                        }
                        else
                        {
                            unRandMin = (uint)nSkillPower;
                            unRandMax = (uint)skillActiveParam.skill_value;
                        }

                        // スキル威力確定：振れ幅算出
                        skillActivity.m_skill_power = (int)RandManager.GetRand(unRandMin, unRandMax + 1);

                        // 効果値によるエフェクトの切り替え
                        skillActivity.m_Effect = InGameUtilBattle.SetSkillEffectToValue(skillActivity.m_Effect, skillActivity.m_Type, skillActivity.m_skill_power);
                    }
                    else
                    {
                        skillActivity.m_skill_power = skillActiveParam.skill_value;
                    }
                }

                // クリティカル判定
                if (BattleSceneUtil.checkChancePercent(skillActiveParam.skill_critical_odds))
                {
                    skillActivity.m_bCritical = true;
                }
                else
                {
                    skillActivity.m_bCritical = false;
                }

                m_SkillRequestLink.addSkillRequest(skillActivity);
            }
        }
    }

}
///////////////////////////////////////EOF///////////////////////////////////////