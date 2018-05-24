using UnityEngine;
using System;

/// <summary>
/// バトル中の状態異常処理をまとめたもの
/// </summary>
public class BattleLogicAilment
{
    public BattleLogicAilment()
    {
    }


    public BattleSkillActivity[] m_acDelayAilment = new BattleSkillActivity[StatusAilmentChara.get_STATUSAILMENT_MAX()];    //!< 遅延発動予約。
    public bool m_bDelayAilment = false;                                                                //!< 遅延発動予約があるかどうか。


    //---------------------------------------------------------------------
    /**
     *	@brief		状態異常の遅延発動予約。
     *	@param[in]	nCharaIndex		: スキルを発動したキャラ。	※BattleSkillActivity.m_SkillParamOwnerNum
     *	@param[in]	acTarget		: スキルのターゲット情報。	※BattleSkillActivity.m_SkillParamTarget
     *	@param[in]	nAilmentTarget	: 状態異常ターゲット。		※BattleSkillActivity.m_statusAilment_target
     *	@param[in]	anAilment		: 状態異常パラメータ配列。	※BattleSkillActivity.m_statusAilment
     */
    //---------------------------------------------------------------------
    public void ReserveDelayAilment(GlobalDefine.PartyCharaIndex nCharaIndex, ref BattleSkillTarget[] acTarget, MasterDataDefineLabel.TargetType nAilmentTarget, ref int[] anAilmentID)
    {
        if (null == acTarget)
        {
#if BUILD_TYPE_DEBUG
            Debug.LogWarning("null == acTarget");
#endif // BUILD_TYPE_DEBUG
            return;
        }
        if (null == anAilmentID)
        {
#if BUILD_TYPE_DEBUG
            Debug.LogWarning("null == anAilment");
#endif // BUILD_TYPE_DEBUG
            return;
        }
        if (GlobalDefine.PartyCharaIndex.ERROR == nCharaIndex)
        {
#if BUILD_TYPE_DEBUG
            // ( -1 != m_SkillParamOwnerNum )を遅延発動の有無判定に使用するので、-1指定で予約されるのはエラー扱いとする。
            Debug.LogError("-1 == nCharaIndex");
#endif // BUILD_TYPE_DEBUG
            return;
        }


        //-----------------------------------------------------------------
        // 遅延発動登録可能なバッファを探す。
        //-----------------------------------------------------------------
        int nIndex = -1;

        int nLen = m_acDelayAilment.Length;
        for (int i = 0; i < nLen; ++i)
        {
            if (null == m_acDelayAilment[i])
            {
                nIndex = i;
                break;
            }
        }

        if (nIndex < 0)
        {
#if BUILD_TYPE_DEBUG
            // 登録場所が見つからなかった。ターン毎にバッファがクリアされていないか、１ターンの総定数以上の遅延発動リクエストが来ているか。
            Debug.LogWarning("ReserveDelayAilment failed. nIndex < 0");
#endif // BUILD_TYPE_DEBUG
            return;
        }


        //-----------------------------------------------------------------
        // １件でも予約が入っている場合、発動させる状態異常が重複していないかチェック。
        // ※同じ種類の状態異常は先着優先となっているため、新しい状態異常がanAilmentに存在しない場合、この予約はキャンセルする。
        //-----------------------------------------------------------------
        if (m_bDelayAilment)
        {
            bool bNew = false;

            nLen = m_acDelayAilment.Length;
            for (int i = 0; i < nLen; ++i)
            {
                if (null == m_acDelayAilment[i])
                {
                    continue;
                }

                if (!CheckAilmentDif(m_acDelayAilment[i].m_statusAilment,
                                        m_acDelayAilment[i].m_statusAilment_target,
                                        anAilmentID,
                                        nAilmentTarget))
                {
                    continue;
                }

                // 新しい状態異常がanAilmentに含まれている。予約処理に進む。
                bNew = true;
                break;
            }

            if (!bNew)
            {
                // 新しい状態異常が無い。予約不要。
                return;
            }
        }


        //-----------------------------------------------------------------
        // 必要な情報を登録。
        //-----------------------------------------------------------------
        m_bDelayAilment = true;

        m_acDelayAilment[nIndex] = new BattleSkillActivity();
        m_acDelayAilment[nIndex].m_SkillParamTarget = new BattleSkillTarget[acTarget.Length];
        m_acDelayAilment[nIndex].m_statusAilment = new int[anAilmentID.Length];
        m_acDelayAilment[nIndex].m_SkillParamOwnerNum = nCharaIndex;
        m_acDelayAilment[nIndex].m_statusAilment_target = nAilmentTarget;

        nLen = m_acDelayAilment[nIndex].m_SkillParamTarget.Length;
        for (int i = 0; i < nLen; ++i)
        {
            if (null != acTarget[i])
            {
                m_acDelayAilment[nIndex].m_SkillParamTarget[i] = new BattleSkillTarget(acTarget[i]);
            }
        }
        nLen = m_acDelayAilment[nIndex].m_statusAilment.Length;
        for (int i = 0; i < nLen; ++i)
        {
            m_acDelayAilment[nIndex].m_statusAilment[i] = anAilmentID[i];
        }
    }

    //---------------------------------------------------------------------
    /**
     *	@brief		状態異常の遅延発動予約クリア。
     */
    //---------------------------------------------------------------------
    public void ClearDelayAilment()
    {
        m_bDelayAilment = false;

        int nLen = m_acDelayAilment.Length;
        for (int i = 0; i < nLen; ++i)
        {
            if (null == m_acDelayAilment[i])
            {
                continue;
            }
            m_acDelayAilment[i].m_SkillParamTarget = null;
            m_acDelayAilment[i].m_statusAilment = null;
            m_acDelayAilment[i] = null;
        }
    }


    private bool CheckAilmentDif(int[] anAilmentIDLH, MasterDataDefineLabel.TargetType nAilmentTargetLH, int[] anAilmentIDRH, MasterDataDefineLabel.TargetType nAilmentTargetRH)
    {
        if (null == anAilmentIDLH || null == anAilmentIDRH)
        {
            return false;
        }

        MasterDataStatusAilmentParam cAilmentParamLH = null;
        MasterDataStatusAilmentParam cAilmentParamRH = null;

        for (int i = 0; i < anAilmentIDLH.Length; ++i)
        {
            if (0 == anAilmentIDLH[i])  // ← 2016/11/04 Developer 修正。元のプログラムでは状態異常のIDと種類を比較していた	//if( MasterDataDefineLabel.AilmentType.NONE == anAilmentIDLH[i] )
            {
                continue;
            }
            cAilmentParamLH = BattleParam.m_MasterDataCache.useAilmentParam((uint)anAilmentIDLH[i]);
            if (null == cAilmentParamLH)
            {
                continue;
            }


            for (int j = 0; j < anAilmentIDRH.Length; ++j)
            {
                if (0 == anAilmentIDRH[j])  // ← 2016/11/04 Developer 修正。元のプログラムでは状態異常のIDと種類を比較していた	//if( MasterDataDefineLabel.AilmentType.NONE == anAilmentIDRH[j] )
                {
                    continue;
                }
                cAilmentParamRH = BattleParam.m_MasterDataCache.useAilmentParam((uint)anAilmentIDRH[j]);
                if (null == cAilmentParamRH)
                {
                    continue;
                }

                if ((cAilmentParamLH.category != cAilmentParamRH.category) ||
                    (nAilmentTargetLH != nAilmentTargetRH))
                {
                    // 状態異常の効果が違う or 状態異常の対象が違う　場合、新規と見なす。
                    // ※同じ状態異常効果でも「敵にかかる」「味方にかかる」と異なる場合があるため、対象の相違をOR判定に加えている。
                    return true;
                }
            }
        }

        return false;
    }

    //----------------------------------------------------------------------------
    //	@brief		状態異常処理
    //----------------------------------------------------------------------------
    public void SkillUpdate_StatusAilment(BattleSkillActivity activity, int nAtk, BattleSceneUtil.MultiInt nHPMax, BattleEnemy[] enemy_param)
    {

        if (activity == null
            || activity.m_SkillParamTarget == null
            || activity.m_statusAilment == null)
        {
            return;
        }


        bool clearOnAttack = true;
        int statusAilment_data_id = 0;
        MasterDataStatusAilmentParam statusAilmentParam = null;
        BattleSkillTarget cBattleTarget = null;
        BattleSceneManager battleMgr = BattleSceneManager.Instance;

        if (battleMgr == null)
        {
            return;
        }


        //	発行された状態異常を全て処理する
        for (int j = 0; j < activity.m_statusAilment.Length; j++)
        {

            statusAilment_data_id = activity.m_statusAilment[j];
            if (statusAilment_data_id == 0)
            {
                continue;
            }


            statusAilmentParam = BattleParam.m_MasterDataCache.useAilmentParam((uint)statusAilment_data_id);
            if (statusAilmentParam == null)
            {
                Debug.LogError("statusAilment Param not found.");
                continue;
            }

            //	遅延のスキルは全員の効果がきれるまで再度この効果をかけることが出来ない
            if (statusAilmentParam.category == MasterDataDefineLabel.AilmentType.FEAR)
            {
                for (int k = 0; k < enemy_param.Length; k++)
                {
                    if (enemy_param[k] == null)
                    {
                        continue;
                    }

                    if (enemy_param[k].isDead() == true)
                    {
                        continue;
                    }

                    if (enemy_param[k].getAttackFlag() == true)
                    {
                        continue;
                    }

                    clearOnAttack = false;
                }
            }

            //	誰かがまだ攻撃をしていない
            if (clearOnAttack == false)
            {
                continue;
            }


            //	状態異常に設定されたターゲットをみて処理
            switch (activity.m_statusAilment_target)
            {
                case MasterDataDefineLabel.TargetType.NONE:
                default:
                    break;

                case MasterDataDefineLabel.TargetType.FRIEND:
                case MasterDataDefineLabel.TargetType.SELF:
                    //----------------------------------------
                    //	プレイヤー側状態異常処理
                    //----------------------------------------
                    {
                        GlobalDefine.PartyCharaIndex target_player = GlobalDefine.PartyCharaIndex.MAX;
                        BattleParam.m_PlayerParty.m_Ailments.AddStatusAilmentToPlayerParty(target_player, statusAilment_data_id, nAtk, nHPMax);
                    }
                    break;

                case MasterDataDefineLabel.TargetType.OTHER:
                    //----------------------------------------
                    //	敵単体(スキル側で指定されたターゲット)
                    //----------------------------------------
                    {
                        bool ailmentAddResult = false;

                        for (int i = 0; i < activity.m_SkillParamTarget.Length; i++)
                        {
                            cBattleTarget = activity.m_SkillParamTarget[i];
                            if (cBattleTarget == null)
                            {
                                continue;
                            }

                            BattleEnemy enemyParam = enemy_param[cBattleTarget.m_TargetNum];
                            if (enemyParam == null)
                            {
                                continue;
                            }

                            if (enemyParam.isDead() == true)
                            {
                                continue;
                            }


                            // 状態異常処理
                            // @change Developer 2016/02/23 v330 毒[最大HP割合]対応
                            ailmentAddResult = enemyParam.m_StatusAilmentChara.AddStatusAilment(statusAilment_data_id, nAtk, enemyParam.m_EnemyHPMax, enemyParam.getMasterDataParamChara());

                            // NEXTのターンを加算(特殊処理)
                            // 上限数以上の場合、遅延効果が発動しないように修正		@20150512 Developer
                            if (statusAilmentParam.category == MasterDataDefineLabel.AilmentType.FEAR
                                && ailmentAddResult == true)
                            {
                                int nTurn = (int)RandManager.GetRand((uint)statusAilmentParam.Get_ABSTATE_LATE_TURN_MIN(),
                                                                     (uint)statusAilmentParam.Get_ABSTATE_LATE_TURN_MAX());
                                enemyParam.AddTurn(nTurn);
                            }
                        }
                    }
                    break;

                case MasterDataDefineLabel.TargetType.ENEMY:
                case MasterDataDefineLabel.TargetType.ENE_N_1:
                case MasterDataDefineLabel.TargetType.ENE_1N_1:
                case MasterDataDefineLabel.TargetType.ENE_R_N:
                case MasterDataDefineLabel.TargetType.ENE_1_N:
                    //----------------------------------------
                    //	敵全体
                    //----------------------------------------
                    {
                        BattleEnemy enemyParam = null;
                        bool ailmentAddResult = false;

                        for (int k = 0; k < enemy_param.Length; k++)
                        {
                            enemyParam = enemy_param[k];
                            if (enemyParam == null)
                            {
                                continue;
                            }

                            if (enemyParam.isDead() == true)
                            {
                                continue;
                            }


                            // 状態異常処理
                            // @change Developer 2016/02/23 v330 毒[最大HP割合]対応
                            ailmentAddResult = enemyParam.m_StatusAilmentChara.AddStatusAilment(statusAilment_data_id, nAtk, enemyParam.m_EnemyHPMax, enemyParam.getMasterDataParamChara());

                            // NEXTのターンを加算(特殊処理)
                            // 上限数以上の場合、遅延効果が発動しないように修正		@20150512 Developer
                            if (statusAilmentParam.category == MasterDataDefineLabel.AilmentType.FEAR
                                && ailmentAddResult == true)
                            {
                                int nTurn = (int)RandManager.GetRand((uint)statusAilmentParam.Get_ABSTATE_LATE_TURN_MIN(),
                                                                     (uint)statusAilmentParam.Get_ABSTATE_LATE_TURN_MAX());
                                enemyParam.AddTurn(nTurn);
                            }
                        }
                    }
                    break;

                case MasterDataDefineLabel.TargetType.ALL:
                    //----------------------------------------
                    //	全員
                    //----------------------------------------
                    {
                        //	プレイヤー側状態異常処理
                        GlobalDefine.PartyCharaIndex target_player = GlobalDefine.PartyCharaIndex.MAX;
                        BattleParam.m_PlayerParty.m_Ailments.AddStatusAilmentToPlayerParty(target_player, statusAilment_data_id, nAtk, nHPMax);

                        BattleEnemy enemyParam = null;
                        bool ailmentAddResult = false;

                        //	敵全体
                        for (int k = 0; k < enemy_param.Length; k++)
                        {
                            enemyParam = enemy_param[k];
                            if (enemyParam == null)
                            {
                                continue;
                            }

                            if (enemyParam.isDead() == true)
                            {
                                continue;
                            }


                            // 状態異常処理
                            // @change Developer 2016/02/23 v330 毒[最大HP割合]対応
                            ailmentAddResult = enemyParam.m_StatusAilmentChara.AddStatusAilment(statusAilment_data_id, nAtk, enemyParam.m_EnemyHPMax, enemyParam.getMasterDataParamChara());

                            // NEXTのターンを加算(特殊処理)
                            // 上限数以上の場合、遅延効果が発動しないように修正		@20150512 Developer
                            if (statusAilmentParam.category == MasterDataDefineLabel.AilmentType.FEAR
                                && ailmentAddResult == true)
                            {
                                int nTurn = (int)RandManager.GetRand((uint)statusAilmentParam.Get_ABSTATE_LATE_TURN_MIN(),
                                                                     (uint)statusAilmentParam.Get_ABSTATE_LATE_TURN_MAX());
                                enemyParam.AddTurn(nTurn);
                            }
                        }
                    }
                    break;
            }
        }

    }

    /// <summary>
    /// 状態異常の遅延発動
    /// </summary>
    public void updateDelaydSkill()
    {
        //-----------------------------------------------------------------
        // 状態異常の遅延発動。プレイヤーのターン開始時に状態異常を発動させる。
        //-----------------------------------------------------------------
        if (m_bDelayAilment)
        {
            CharaOnce cChara = null;

            for (int i = 0; i < m_acDelayAilment.Length; ++i)
            {
                if (null == m_acDelayAilment[i])
                {
                    break;
                }


                //---------------------------------------------------------
                // 状態異常付きスキルを発動したキャラを取得。
                //---------------------------------------------------------
                if (m_acDelayAilment[i].m_SkillParamOwnerNum < 0 ||
                    m_acDelayAilment[i].m_SkillParamOwnerNum >= GlobalDefine.PartyCharaIndex.MAX)
                {
                    continue;
                }
                cChara = BattleParam.m_PlayerParty.getPartyMember(m_acDelayAilment[i].m_SkillParamOwnerNum, CharaParty.CharaCondition.EXIST);
                if (null == cChara)
                {
                    continue;
                }


                //---------------------------------------------------------
                // 状態異常付与。
                // @change Developer 201603/14 v330 キャラの最大HPではなく、パーティの最大HPに変更
                //---------------------------------------------------------
                //				SkillUpdate_StatusAilment( m_acDelayAilment[i], cChara.m_CharaPow, cChara.m_CharaHP );
                SkillUpdate_StatusAilment(m_acDelayAilment[i], cChara.m_CharaPow, BattleParam.m_PlayerParty.m_HPMax, BattleParam.m_EnemyParam);
            }


            //-------------------------------------------------------------
            // 状態異常遅延発動バッファをクリア。
            //-------------------------------------------------------------
            ClearDelayAilment();
        }
    }
}
