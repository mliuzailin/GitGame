using UnityEngine;
using System;

/// <summary>
/// バトル中のプレイヤースキル処理をまとめたもの
/// </summary>
public class BattleLogicSkill
{
    private BattleLogic m_Owner;
    public BattleLogicSkill(BattleLogic owner_class)
    {
        m_Owner = owner_class;

        m_SkillRequestActive.clearRequest();
        m_SkillRequestLeader.clearRequest();
        m_SkillRequestPassive.clearRequest();
        m_SkillRequestLinkPassive.clearRequest();

        //--------------------------------
        // 攻撃リクエスト領域を確保
        //--------------------------------
        m_AttackFixReq = new BattleSkillReq[BattleLogic.BATTLE_SKILL_TOTAL_MAX];
    }

    public SkillRequestParam m_SkillRequestActive = new SkillRequestParam(BattleLogic.BATTLE_SKILL_TOTAL_MAX + SkillLink.SKILL_LINK_MAX);                                       //!< スキル発動リクエスト管理：NS
    public SkillRequestParam m_SkillRequestLeader = new SkillRequestParam(2);                                       //!< スキル発動リクエスト管理：PS
    public SkillRequestParam m_SkillRequestPassive = new SkillRequestParam(35);                                     //!< スキル発動リクエスト管理：LBS
    public SkillRequestParam m_SkillRequestLinkPassive = new SkillRequestParam(35);                                     //!< スキル発動リクエスト管理：LPS(カウンター処理を切り分けるため、PS配列を流用できない)※追撃のみなら流用可能
    public SkillLink m_SkillLink = new SkillLink();                                             //!< スキルリンク情報
    public SkillBoost m_SkillBoost = new SkillBoost();                                              //!< スキルブースト情報

    public BattleSkillReq[] m_AttackFixReq = null;                                          //!< 攻撃リクエスト：スキル確定用：リクエスト情報
    public int m_AttackFixReqInputNum = 0;                                          //!< 攻撃リクエスト：スキル確定用：リクエスト情報アクセス番号

    public int m_SkillComboCountCalc = 0;                                           //!< スキルコンボ：計算用（以前はm_AttackFixReqInputNumを使用していたが復活スキルはコンボ数にカウントされないのでこの変数を用意）
    public int m_SkillComboCountDisp = 0;                                           //!< スキルコンボ：表示用

    public bool[] m_ActiveSkillElement = new bool[(int)MasterDataDefineLabel.ElementType.MAX];              //!< スキル発動情報：各属性のスキルが発動しているかどうか
    public int[] m_ActiveSkillCostCount = new int[5];   //!< コスト別のアクティブスキル発動数

    private int m_ResurrectBitFlag = 0;

    public void clearUsingElementInfo()
    {
        //----------------------------------------
        //	各属性のスキル成立フラグをOFFにしておく
        //----------------------------------------
        for (int i = 0; i < m_ActiveSkillElement.Length; i++)
        {
            m_ActiveSkillElement[i] = false;
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		リーダースキル追撃発動リクエスト
        @param[in]	int		(nChara)		キャラID
        @retval		bool	[発動/非発動]
    */
    //------------------------------------------------------------------------
    public BattleSkillActivity RequestLeaderSkillFollowAttack(GlobalDefine.PartyCharaIndex nCharaID)
    {

        if (nCharaID < 0 || nCharaID >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return null;
        }

        CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(nCharaID, CharaParty.CharaCondition.SKILL_LEADER);
        if (chara == null)
        {
            return null;
        }

        if (!chara.m_bHasCharaMasterDataParam)
        {
            return null;
        }

        MasterDataSkillLeader skill = BattleParam.m_MasterDataCache.useSkillLeader(chara.m_CharaMasterDataParam.skill_leader);
        if (skill == null)
        {
            return null;
        }

        //----------------------------------------
        // スキル発動情報設定
        //----------------------------------------
        BattleSkillActivity skillActivity = new BattleSkillActivity();

        skillActivity.m_SkillParamOwnerNum = nCharaID;
        skillActivity.m_SkillParamFieldID = -1;
        skillActivity.m_SkillParamSkillID = chara.m_CharaMasterDataParam.skill_leader;
        skillActivity.m_SkillType = ESKILLTYPE.eLEADER;
        skillActivity.m_Effect = skill.skill_follow_atk_effect;
        skillActivity.m_Element = skill.skill_follow_atk_element;

        BattleSkillTarget target = null;
        BattleEnemy enemy_param = null;
        BattleSkillTarget[] targetArray = null;

        //----------------------------------------
        // 攻撃関係のスキルはターゲット情報を作成
        //----------------------------------------
        if (skill.Is_skill_follow_atk_active())
        {

            targetArray = new BattleSkillTarget[BattleParam.m_EnemyParam.Length];


            // 対象情報作成
            for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
            {

                enemy_param = BattleParam.m_EnemyParam[i];
                if (enemy_param == null)
                {
                    continue;
                }

                target = new BattleSkillTarget();

                // 敵毎にダメージ情報を作成
                CharaOnce activity_target = new CharaOnce();
                activity_target.CharaSetupFromParamEnemy(enemy_param.getMasterDataParamEnemy());

                target.m_TargetType = BattleSkillTarget.TargetType.ENEMY;
                target.m_TargetNum = i;
                target.m_SkillValueToEnemy = InGameUtilBattle.GetDamageFollow(BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.LEADER, CharaParty.CharaCondition.SKILL_LEADER),
                                                                  BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.FRIEND, CharaParty.CharaCondition.SKILL_LEADER),
                                                                  chara,
                                                                  activity_target,
                                                                  enemy_param,
                                                                  skillActivity);


                targetArray[i] = target;
            }

            skillActivity.m_SkillParamTarget = targetArray;

        }

        return skillActivity;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル追撃発動リクエスト
        @retval		bool	[発動/非発動]
        @change		Developer 2015/09/11 ver300	リンクパッシブ対応
    */
    //------------------------------------------------------------------------
    public bool RequestPassiveSkillFollowAttack(ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        bool result = false;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (m_Owner.m_BattleActive == false)
        {
            return (result);
        }

        //------------------------------
        // 全てのキャラのパッシブスキルをチェック
        //------------------------------
        for (int num = 0; num < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            if (PassiveSkillFollowAttack(chara_once, (GlobalDefine.PartyCharaIndex)num, skillType) == false)
            {
                continue;
            }

            result = true;
        }

        return (result);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		パッシブスキル追撃発動リクエスト
        @retval		bool	[発動/非発動]
        @note		Developer 2015/09/08 ver300 根本の処理部分を分離
    */
    //------------------------------------------------------------------------
    private bool PassiveSkillFollowAttack(CharaOnce chara, GlobalDefine.PartyCharaIndex owner, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        MasterDataParamChara charaParam = null;
        MasterDataSkillPassive passiveParam = null;
        BattleEnemy enemy_param = null;
        BattleSkillActivity skillActivity = null;       // スキル発動情報用
        BattleSkillTarget target = null;        // ターゲット情報用
        BattleSkillTarget[] targetArray = null;
        SkillRequestParam requestPram = null;

        bool result = false;

        //------------------------------
        // メンバーが設定されているかチェック
        //------------------------------
        if (chara == null)
        {
            return (result);
        }

        //------------------------------
        // スキルタイプによる分岐
        //------------------------------
        switch (skillType)
        {
            //------------------------------
            // パッシブスキル
            //------------------------------
            case ESKILLTYPE.ePASSIVE:
                // キャラ情報取得
                charaParam = chara.m_CharaMasterDataParam;
                if (charaParam == null)
                {
                    return (result);
                }

                // パッシブスキル取得
                passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.skill_passive);
                if (passiveParam == null)
                {
                    return (result);
                }

                // スキルに対応したリクエスト情報を取得
                requestPram = m_SkillRequestPassive;
                break;

            //------------------------------
            // リンクパッシブ
            //------------------------------
            case ESKILLTYPE.eLINKPASSIVE:
                // リンクキャラ情報取得
                charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara.m_LinkParam.m_CharaID);
                if (charaParam == null)
                {
                    return (result);
                }

                // リンクパッシブ取得
                passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
                if (passiveParam == null)
                {
                    return (result);
                }

                // スキルに対応した配列を取得
                requestPram = m_SkillRequestLinkPassive;
                break;
        }

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        MasterDataSkillPassive.FollowAttackInfo follow_attack_info = passiveParam.Get_FollowAttackInfo();
        if (follow_attack_info == null)
        {
            return (result);
        }

        //----------------------------------------
        // スキル発動情報設定
        //----------------------------------------
        skillActivity = new BattleSkillActivity();
        skillActivity.m_SkillParamOwnerNum = owner;
        skillActivity.m_SkillParamFieldID = -1;
        skillActivity.m_SkillParamSkillID = passiveParam.fix_id;
        skillActivity.m_SkillType = skillType;
        skillActivity.m_Effect = follow_attack_info.m_EffectType;
        skillActivity.m_Element = follow_attack_info.m_ElementType;
        skillActivity.m_Type = MasterDataDefineLabel.SkillType.ATK_ALL;


        //----------------------------------------
        // ターゲット情報を作成
        //----------------------------------------
        targetArray = new BattleSkillTarget[BattleParam.m_EnemyParam.Length];

        // 対象情報作成
        for (int num = 0; num < BattleParam.m_EnemyParam.Length; ++num)
        {
            enemy_param = BattleParam.m_EnemyParam[num];
            if (enemy_param == null)
            {
                continue;
            }

            target = new BattleSkillTarget();

            // 敵毎にダメージ情報を作成
            CharaOnce activity_target = new CharaOnce();
            activity_target.CharaSetupFromParamEnemy(enemy_param.getMasterDataParamEnemy());

            target.m_TargetType = BattleSkillTarget.TargetType.ENEMY;
            target.m_TargetNum = num;
            target.m_SkillValueToEnemy = InGameUtilBattle.GetPassiveDamageFollow(chara, activity_target, enemy_param, skillActivity);

            targetArray[num] = target;
        }

        skillActivity.m_SkillParamTarget = targetArray;

        requestPram.addSkillRequest(skillActivity);

        // SE再生：リーダースキルパワーアップ
        SoundUtil.PlaySE(SEID.SE_INGAME_LEADERSKILL);

        result = true;

        return (result);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		リンクパッシブ発行情報リセット
        @note
        @add		Developer 2015/09/15 ver300
    */
    //----------------------------------------------------------------------------
    public void ResetSkillRequestLinkPassive()
    {
        m_SkillRequestLinkPassive.clearRequest();
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		パネル配置スキル
        @note		リーダースキル、パッシブスキル、リンクパッシブの発動管理
        @add		Developer 2015/12/25 ver320
    */
    //----------------------------------------------------------------------------
    public void SkillBattlefieldPanel()
    {
        // 戦闘開始時の場合(プレイヤー操作のステップでは、ターン数1が初回)
        bool bBtlStart = false;
        if (m_Owner.m_BattleTotalTurn == 1)
        {
            bBtlStart = true;
        }

        // スキル発動判定用
        bool bLeader = false;
        bool bFriend = false;
        bool bPassive = false;
        bool bLinkPassive = false;

        //---------------------------
        // リーダースキル
        //---------------------------
        CharaOnce leader_chara = BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.LEADER, CharaParty.CharaCondition.SKILL_LEADER);
        if (leader_chara != null)
        {
            bLeader = InGameUtilBattle.SkillBattlefieldPanel(leader_chara.m_CharaMasterDataParam.skill_leader, ESKILLTYPE.eLEADER, bBtlStart);
        }

        CharaOnce friend_chara = BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.FRIEND, CharaParty.CharaCondition.SKILL_LEADER);
        if (friend_chara != null)
        {
            bFriend = InGameUtilBattle.SkillBattlefieldPanel(friend_chara.m_CharaMasterDataParam.skill_leader, ESKILLTYPE.eLEADER, bBtlStart);
        }

        //---------------------------
        // パッシブスキル
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //---------------------------
        for (int num = (int)GlobalDefine.PartyCharaIndex.LEADER; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            if (chara_once == null)
            {
                continue;
            }

            if (InGameUtilBattle.SkillBattlefieldPanel(chara_once.m_CharaMasterDataParam.skill_passive, ESKILLTYPE.ePASSIVE, bBtlStart) == true)
            {
                bPassive = true;
            }
        }

        //---------------------------
        // リンクパッシブスキル
        //---------------------------
        for (int num = (int)GlobalDefine.PartyCharaIndex.LEADER; num < (int)GlobalDefine.PartyCharaIndex.MAX; ++num)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)num, CharaParty.CharaCondition.SKILL_PASSIVE);
            if (chara_once == null
            || chara_once.m_LinkParam == null
            || chara_once.m_LinkParam.m_cCharaMasterDataParam == null)
            {
                continue;
            }

            if (InGameUtilBattle.SkillBattlefieldPanel(chara_once.m_LinkParam.m_cCharaMasterDataParam.link_skill_passive, ESKILLTYPE.eLINKPASSIVE, bBtlStart) == true)
            {
                bLinkPassive = true;
            }
        }

        // スキルが発動していた場合
        if (bLeader == true
        || bFriend == true
        || bPassive == true
        || bLinkPassive == true)
        {
            SoundUtil.PlaySE(SEID.SE_BATTLE_BUFF);
        }
    }

    //------------------------------------------------------------------------
    /*!
        @brief		プレイヤー側のスキル処理
    */
    //------------------------------------------------------------------------
    public bool SkillUpdate(SkillRequestParam request_param)
    {

        //----------------------------------------
        // スキル表示の更新
        //----------------------------------------
        BattleEnemy[] enemy_params = BattleParam.m_EnemyParam;

        bool is_zenmatsu = true;
        for (int idx = 0; idx < enemy_params.Length; idx++)
        {
            if (enemy_params[idx].m_EnemyHP > 0)
            {
                is_zenmatsu = false;
                break;
            }
        }
        if (is_zenmatsu)
        {
            BattleSkillCutinManager.Instance.setSpeedUp(true);
        }


        //----------------------------------------
        // カットインから効果発動指示が出てるなら発動
        //----------------------------------------
        int cutin_id = BattleSkillCutinManager.Instance.GetPlayingCutinID();
        bool is_wait_next_cutin = BattleSkillCutinManager.Instance.GetPlayingCutinAnimEnd(true);

        BattleSkillActivity activity = request_param.getCurrentSkillRequest();
        if (cutin_id >= 0
            && activity != null
            && activity.m_CutinID == cutin_id
        )
        {
            // タイトル切り替え		@add Developer 2015/09/10 ver300
            if (activity.m_SkillType == ESKILLTYPE.eACTIVE
            && m_Owner.m_DispCaption.getCurrentCaption() == BattleCaptionControl.CaptionType.LINK_SKILL)
            {
                m_Owner.m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.ACTIVE_SKILL);
            }
            else if (activity.m_SkillType == ESKILLTYPE.eLINK
            && m_Owner.m_DispCaption.getCurrentCaption() == BattleCaptionControl.CaptionType.ACTIVE_SKILL)
            {
                m_Owner.m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LINK_SKILL);
            }

            switch (activity.m_SkillType)
            {
                case ESKILLTYPE.eLIMITBREAK:
                    {
                        //	特定スキルタイプはカットイン終了を待つ
                        if (is_wait_next_cutin == false)
                        {
                            return false;
                        }
                    }
                    break;

                case ESKILLTYPE.eLEADER:
                case ESKILLTYPE.ePASSIVE:
                case ESKILLTYPE.eBOOST:
                case ESKILLTYPE.eLINK:
                case ESKILLTYPE.eLINKPASSIVE:
                    {
                    }
                    break;

                default:
                    {
                    }
                    break;
            }


            // 攻撃力などを確定する(プレイヤーパーティ・敵パーティの現状の影響を受ける)
            switch (activity.m_SkillType)
            {
                case ESKILLTYPE.ePASSIVE:
                case ESKILLTYPE.eLINKPASSIVE:
                    // PassiveSkillFollowAttack()で攻撃力が計算済みなのでここでは計算しない.
                    break;

                default:
                    SkillRequestParam.calcAttackValue(activity, m_SkillComboCountCalc, m_Owner.m_abBoostField);
                    break;
            }

            //-------------------------------
            // ブーストスキルの発動情報作成
            //-------------------------------
            m_SkillBoost.AddActivityBoostSkill(activity);


            //----------------------------------------
            //	スキル実行済みフラグ
            //----------------------------------------
            request_param.nextProgress();


            //----------------------------------------
            //	効果発動時にコンボ数を加算
            //	※評価表示に使用されるコンボ数はノーマルスキルでのみカウント
            //----------------------------------------
            if (activity.m_SkillType == ESKILLTYPE.eACTIVE
                && activity.getMasterDataSkillActive().isAlwaysResurrectSkill() == false
            )
            {
                m_SkillComboCountDisp = m_SkillComboCountDisp + 1;

                BattleParam.plusPartyMemberHands(activity.m_SkillParamOwnerNum, -1);

                //ヒーロースキルターン数消化
                BattleParam.m_PlayerParty.m_BattleHero.addSkillTrun(1, true);

                m_Owner.m_BattleComboArea.setHitCount(m_SkillComboCountDisp);

                if (BattleParam.m_PlayScoreInfo != null)
                {
                    BattleParam.m_PlayScoreInfo.addHand(
                        m_Owner.m_BattleLogicSkill.m_ActiveSkillCostCount[0] > 0,
                        m_Owner.m_BattleLogicSkill.m_ActiveSkillCostCount[1] > 0,
                        m_Owner.m_BattleLogicSkill.m_ActiveSkillCostCount[2] > 0,
                        m_Owner.m_BattleLogicSkill.m_ActiveSkillCostCount[3] > 0,
                        m_Owner.m_BattleLogicSkill.m_ActiveSkillCostCount[4] > 0
                    );
                }
            }


            //----------------------------------------
            // 指定番号のカットインから効果発動指示が出ている
            // →スキル発動情報から効果を発動する
            //----------------------------------------
            //----------------------------------------
            // スキルに合わせてエフェクト生成
            //----------------------------------------
            m_Owner.AddSkillEffect(activity);


            //----------------------------------------
            // リーダースキルは強制的に全体を対象とする
            //----------------------------------------
            MasterDataDefineLabel.SkillType type = activity.m_Type;
            if (activity.m_SkillType == ESKILLTYPE.eLEADER)
            {
                type = MasterDataDefineLabel.SkillType.ATK_ALL;
            }


            //----------------------------------------
            // 攻撃タイプ
            // →敵に対してダメージを発行
            //----------------------------------------

            bool ignoreDef = (activity.m_skill_chk_def_defence != MasterDataDefineLabel.BoolType.ENABLE) ? true : false;
            bool ignoreDefAilment = (activity.m_skill_chk_def_ailment != MasterDataDefineLabel.BoolType.ENABLE) ? true : false;
            bool ignoreDefBarrier = (activity.m_skill_chk_def_barrier != MasterDataDefineLabel.BoolType.ENABLE) ? true : false;
            bool damage_enable = true;
            int damage = 0;
            MasterDataDefineLabel.ElementType elem = 0;
            BattleSceneUtil.MultiInt kickback = new BattleSceneUtil.MultiInt();
            BattleSceneUtil.MultiInt kickback_rate = new BattleSceneUtil.MultiInt();
            BattleSkillTarget cBattleTarget = null;
            EDAMAGE_TYPE eDamageType = EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL;

            //-------------------------------
            //	攻撃力を参照し、動作する物があるため、攻撃力を入力
            //-------------------------------
            int nAtk = 0;
            GlobalDefine.PartyCharaIndex nOwner = GlobalDefine.PartyCharaIndex.ERROR;

            //----------------------------------------
            // 即死攻撃用フラグ
            //----------------------------------------
            bool bDeath = false;

            //----------------------------------------
            // スキル効果発動フラグ
            // @add		Developer 32016/06/01 v350
            // @note	攻撃対象(エネミー)分、処理が行われる。(事前シミュレートで、スキルタイプによりターゲットが決まる)
            //			全体攻撃+ユニーク効果の場合、1スキルの発動で、ユニーク効果がエネミー分発動する問題があるのでフラグ管理する
            //			Developer 2017/09/14 [DG0-2509]リミブレスキルにもこの問題があったので対応
            //----------------------------------------
            bool is_worked_skill_effect = false;        // ユニーク効果処理(カテゴリ分岐)の発動有無

            //----------------------------------------
            // 手札を再構築用フラグ
            // @change	Developer 2016/03/28 v330 コメントアウト
            // @note	パネル配置後のパネル出現率操作スキル無効から回復した場合、場に置かれたパネルが消えるバグ修正対応
            //----------------------------------------

            nOwner = activity.m_SkillParamOwnerNum;
            if (nOwner >= 0 && (int)nOwner < BattleParam.m_PlayerParty.getPartyMemberMaxCount())
            {
                CharaOnce chara = BattleParam.m_PlayerParty.getPartyMember(activity.m_SkillParamOwnerNum, CharaParty.CharaCondition.SKILL_ACTIVE);
                if (chara != null)
                {
                    nAtk = chara.m_CharaPow;
                }
            }
            else if (nOwner == GlobalDefine.PartyCharaIndex.HERO)
            {
                nAtk = BattleParam.m_PlayerParty.m_BattleHero.getAtk();
            }

            if (activity.m_SkillParamTarget == null)
            {
                Debug.LogError("Skill Target Error");
                return false;
            }

            //----------------------------------------
            //	すべての攻撃対象に対してダメージ計算を行っていく
            //----------------------------------------
            for (int nTargetAccess = 0; nTargetAccess < (activity.m_SkillParamTarget.Length); nTargetAccess++)
            {


                cBattleTarget = activity.m_SkillParamTarget[nTargetAccess];
                if (cBattleTarget != null)
                {

                    // ダメージ値入力(基本的にInGameUtil.GetDamageValueにてダメージ計算が行われる)
                    damage = (int)(cBattleTarget.m_SkillValueToEnemy);

                    damage_enable = activity.is_skill_damage_enable() || (damage > 0);
                    elem = activity.m_Element;
                    eDamageType = EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL;

                    bDeath = false;

                    switch (activity.m_SkillType)
                    {
                        default:
                        case ESKILLTYPE.eACTIVE:
                        case ESKILLTYPE.eLINK:
                            //----------------------------------------
                            //	通常スキルダメージ計算
                            //----------------------------------------
                            {
                                switch (type)
                                {
                                    default:
                                        //	通常攻撃
                                        {
                                            if (enemy_params[cBattleTarget.m_TargetNum] == null)
                                            {
                                                break;
                                            }

                                            // クリティカルチェック
                                            if (activity.m_bCritical == true)
                                            {
                                                eDamageType = EDAMAGE_TYPE.eDAMAGE_TYPE_CRITICAL;
                                            }
                                            else
                                            {
                                                // 弱点チェック
                                                eDamageType = InGameUtilBattle.GetSkillElementAffinity(elem,
                                                                                                 enemy_params[cBattleTarget.m_TargetNum].getMasterDataParamChara().element);
                                            }
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillType.HEAL:
                                        //	通常回復
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            InGameUtilBattle.HealHPSkillProc(activity);

                                            // 音再生
                                            SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);

                                            // 情報がないためプログラムでダメージOFFを入力
                                            damage_enable = false;
                                        }
                                        break;
                                }
                            }
                            break;

                        case ESKILLTYPE.eLEADER:
                            //----------------------------------------
                            //	リーダースキルダメージ計算
                            //----------------------------------------
                            {
                                if (enemy_params[cBattleTarget.m_TargetNum] == null)
                                {
                                    break;
                                }

                                if (activity.m_SkillParamOwnerNum == GlobalDefine.PartyCharaIndex.LEADER
                                    || activity.m_SkillParamOwnerNum == GlobalDefine.PartyCharaIndex.FRIEND)
                                {
                                    eDamageType = InGameUtilBattle.GetSkillElementAffinity(elem,
                                                                                     enemy_params[cBattleTarget.m_TargetNum].getMasterDataParamChara().element);
                                }
                            }
                            break;

                        case ESKILLTYPE.ePASSIVE:
                        case ESKILLTYPE.eLINKPASSIVE:
                            //----------------------------------------
                            //	パッシブスキルダメージ計算
                            //----------------------------------------
                            switch (type)
                            {
                                default:
                                    {
                                        if (enemy_params[cBattleTarget.m_TargetNum] == null)
                                        {
                                            break;
                                        }

                                        eDamageType = InGameUtilBattle.GetSkillElementAffinity(elem,
                                                                                         enemy_params[cBattleTarget.m_TargetNum].getMasterDataParamChara().element);
                                    }
                                    break;

                                case MasterDataDefineLabel.SkillType.HEAL:
                                    if (is_worked_skill_effect == false)
                                    {
                                        is_worked_skill_effect = true;
                                        InGameUtilBattle.HealHPSkillProc(activity);

                                        // 音再生
                                        SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);

                                        // 情報がないためプログラムでダメージOFFを入力
                                        damage_enable = false;
                                    }
                                    break;
                            }
                            break;

                        case ESKILLTYPE.eLIMITBREAK:
                            //----------------------------------------
                            //	リミットブレイクスキルダメージ計算
                            //----------------------------------------
                            {

                                //----------------------------------------
                                //	共通処理
                                //----------------------------------------
                                //	反動ダメージ量(パーティ全体で受ける)
                                kickback.setValue(GlobalDefine.PartyCharaIndex.MAX, activity.m_skill_kickback_fix);
                                kickback_rate.setValue(GlobalDefine.PartyCharaIndex.MAX, activity.m_skill_kickback);

                                //	弱点チェック
                                if (activity.m_skill_chk_atk_affinity == MasterDataDefineLabel.BoolType.ENABLE)
                                {
                                    if (enemy_params[cBattleTarget.m_TargetNum] != null)
                                    {

                                        eDamageType = InGameUtilBattle.GetSkillElementAffinity(activity.m_Element, enemy_params[cBattleTarget.m_TargetNum].getMasterDataParamChara().element);

                                    }
                                }


                                //----------------------------------------
                                //	ユニーク効果処理
                                //----------------------------------------
                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　リミブレスキル効果種類:" + activity.m_Category_SkillCategory_PROPERTY.ToString());
                                switch (activity.m_Category_SkillCategory_PROPERTY)
                                {
                                    case MasterDataDefineLabel.SkillCategory.ATK_DEATH:
                                        //----------------------------------------
                                        //	[確率で即死]
                                        //----------------------------------------
                                        {
                                            if (enemy_params[cBattleTarget.m_TargetNum] == null)
                                            {
                                                break;
                                            }

                                            // 敵特性：即死無効化
                                            if (EnemyAbility.ChkAbilityInvalid(enemy_params[cBattleTarget.m_TargetNum], MasterDataDefineLabel.SkillCategory.ATK_DEATH) == true)
                                            {
                                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：即死効果:無効");
                                                break;
                                            }

                                            // 即死のみダメージを入力
                                            int rate = activity.Get_ATK_DEATH_RATE();
                                            int rand = (int)RandManager.GetRand(0, 100);
                                            if (rand <= rate)
                                            {
                                                damage = enemy_params[cBattleTarget.m_TargetNum].m_EnemyHP;

                                                bDeath = true;
                                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：即死効果:発動(" + rate.ToString() + "%)");
                                            }
                                            else
                                            {
                                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：即死効果:不発(" + rate.ToString() + "%)");
                                            }
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.COST_SHUFFLE:
                                        //-------------------------------
                                        //	全カード入れ替え
                                        //-------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            for (int n = 0; n < m_Owner.m_BattleCardManager.m_HandArea.getCardMaxCount(); n++)
                                            {

                                                BattleScene.BattleCard battle_card = m_Owner.m_BattleCardManager.m_HandArea.getCard(n);
                                                if (battle_card == null)
                                                {
                                                    continue;
                                                }


                                                // 属性変更
                                                battle_card.setElementType(m_Owner.GetRandomCardElement(), BattleScene.BattleCard.ChangeCause.SKILL);

                                                //	手札変化エフェクト
                                                m_Owner.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, n, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);
                                            }
                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：手札変化(全カードランダム)");
                                            DebugBattleLog.outputCard(m_Owner.m_BattleCardManager);
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.COST_CHANGE_ELEM:
                                        //-------------------------------
                                        //	属性変化
                                        //-------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            MasterDataDefineLabel.ElementType prev = activity.Get_COST_CHANGE_PREV();
                                            MasterDataDefineLabel.ElementType next = activity.Get_COST_CHANGE_AFTER();
                                            for (int n = 0; n < m_Owner.m_BattleCardManager.m_HandArea.getCardMaxCount(); n++)
                                            {
                                                BattleScene.BattleCard battle_card = m_Owner.m_BattleCardManager.m_HandArea.getCard(n);
                                                // 該当属性のカードのみ変化
                                                if (battle_card.getElementType() != prev)
                                                {
                                                    continue;
                                                }

                                                battle_card.setElementType(next, BattleScene.BattleCard.ChangeCause.SKILL);

                                                //	手札変化エフェクト
                                                m_Owner.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, n, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);
                                            }
                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：手札変化(" + prev.ToString() + "→" + next.ToString() + ")");
                                            DebugBattleLog.outputCard(m_Owner.m_BattleCardManager);
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.SUPPORT_COST_FIX:
                                        //----------------------------------------
                                        // 手札属性固定
                                        //----------------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            // 手札を変化させるか選択
                                            bool handcard_enable = activity.Get_SUPPORT_C_FIX_HANDCARD();
                                            if (handcard_enable)
                                            {
                                                // 手札交換
                                                for (int n = 0; n < m_Owner.m_BattleCardManager.m_HandArea.getCardMaxCount(); n++)
                                                {
                                                    BattleScene.BattleCard battle_card = m_Owner.m_BattleCardManager.m_HandArea.getCard(n);
                                                    // 属性変更
                                                    battle_card.setElementType(activity.Get_SUPPORT_C_FIX_ELEM(), BattleScene.BattleCard.ChangeCause.NONE);
                                                }
                                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：手札変化(属性固定:" + activity.Get_SUPPORT_C_FIX_ELEM().ToString() + ")");
                                                DebugBattleLog.outputCard(m_Owner.m_BattleCardManager);
                                            }
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.SUPPORT_ALL_BOOST:
                                        //----------------------------------------
                                        //	場をすべてブーストに変換
                                        //----------------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            m_Owner.SwitchBoostField(BattleLogic.EBOOST_CLEAR_MODE.eALL);
                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：場をすべてブーストに変換");
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.SUPPORT_LBS_TURN_FAST:
                                        //----------------------------------------
                                        //	LBS必要ターン短縮
                                        //----------------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            InGameUtilBattle.LBSCostSkillProc(activity);
                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：LBS必要ターン短縮");
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.MOVE_TELEPORT:
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.MOVE_UNLOCK_DOOR:
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.SUPPORT_COST_CHANGE_ALL:
                                        //----------------------------------------
                                        //	手札の属性を書き換え
                                        //----------------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            MasterDataDefineLabel.ElementType[] elem_array = activity.Get_COST_CHANGE_ALL_ELEMENT();

                                            for (int j = 0; j < elem_array.Length; j++)
                                            {

                                                if (elem_array[j] == MasterDataDefineLabel.ElementType.NONE
                                                    || elem_array[j] == MasterDataDefineLabel.ElementType.MAX)
                                                {
                                                    continue;
                                                }

                                                BattleScene.BattleCard battle_card = m_Owner.m_BattleCardManager.m_HandArea.getCard(j);
                                                if (battle_card.getElementType() == elem_array[j])
                                                {
                                                    continue;
                                                }

                                                // 属性変更
                                                battle_card.setElementType(elem_array[j], BattleScene.BattleCard.ChangeCause.SKILL);

                                                //	手札変化エフェクト
                                                m_Owner.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, j, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_CHANGE);
                                            }
                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：手札変化");
                                            DebugBattleLog.outputCard(m_Owner.m_BattleCardManager);
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.SUPPORT_ABSTATE_CLEAR:
                                        //----------------------------------------
                                        //	状態異常全クリア&HP回復
                                        //----------------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            // 更新前：手札に影響がある状態異常を取得
                                            // @change	Developer 2016/03/28 v330 コメントアウト
                                            // @note	パネル配置後のパネル出現率操作スキル無効から回復した場合、場に置かれたパネルが消えるバグ修正対応
                                            //ailmentHandCardPrev = StatusAilmentUtil.GetHandCardDefault( ingameMgr.m_PlayerParty.m_StatusAilmentChara );

                                            InGameUtilBattle.ClearAilmentStatusSkillProc(activity);
                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：状態異常全クリア&HP回復");
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.RECOVERY_HP:
                                    case MasterDataDefineLabel.SkillCategory.RECOVERY_HP_RATE:
                                        //----------------------------------------
                                        //	HP回復
                                        //----------------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            InGameUtilBattle.HealHPSkillProc(activity);

                                            // 音再生
                                            SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);

                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：HP回復");
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.RECOVERY_SP:
                                        //-------------------------------
                                        //	SP回復タイプ
                                        //-------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            InGameUtilBattle.HealSPSkillProc(activity);

                                            // 音再生
                                            SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);

                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：SP回復");
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.SUPPORT_SPESTATE_CLEAR:
                                        //-------------------------------
                                        //	指定都合状態異常をクリア
                                        //-------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            // 更新前：手札に影響がある状態異常を取得
                                            // @change	Developer 2016/03/28 v330 コメントアウト
                                            // @note	パネル配置後のパネル出現率操作スキル無効から回復した場合、場に置かれたパネルが消えるバグ修正対応
                                            //ailmentHandCardPrev = StatusAilmentUtil.GetHandCardDefault( ingameMgr.m_PlayerParty.m_StatusAilmentChara );

                                            InGameUtilBattle.ClearAilmentStatusCondition(activity);
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.SUPPORT_BATTLEFIELD_PANEL:
                                        //-------------------------------
                                        //	場にパネルを配置
                                        //-------------------------------
                                        if (is_worked_skill_effect == false)
                                        {
                                            is_worked_skill_effect = true;
                                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：場にパネルを配置");
                                            InGameUtilBattle.SkillBattlefieldPanel(activity);
                                        }
                                        break;

                                    case MasterDataDefineLabel.SkillCategory.STATE_DARK:
                                        //@add Developer 2016/09/06 v370 状態異常：暗闇
                                        //-------------------------------
                                        //	暗闇
                                        //-------------------------------
                                        {
                                            //ここでは処理なし
                                            //InGameManagerにて状態異常ステータスをみてフィールドパネルが更新される
                                        }
                                        break;

                                    default:
                                        break;
                                }

                            }
                            break;

                        case ESKILLTYPE.eBOOST:
                            //----------------------------------------
                            //	ブーストスキルダメージ計算
                            //----------------------------------------
                            {
                                //----------------------------------------
                                //	共通処理
                                //----------------------------------------
                                //	弱点チェック
                                if (activity.m_skill_chk_atk_affinity == MasterDataDefineLabel.BoolType.ENABLE)
                                {
                                    if (enemy_params[cBattleTarget.m_TargetNum] != null)
                                    {

                                        eDamageType = InGameUtilBattle.GetSkillElementAffinity(activity.m_Element, enemy_params[cBattleTarget.m_TargetNum].getMasterDataParamChara().element);

                                    }
                                }

                                //----------------------------------------
                                // スキル効果発動フラグ
                                // @change Developer 32016/06/01 v350 bugweb：5460、5464対応
                                //----------------------------------------
                                if (is_worked_skill_effect == false)
                                {
                                    is_worked_skill_effect = true;

                                    //----------------------------------------
                                    //	ユニーク効果処理(カテゴリ分岐)
                                    //----------------------------------------
                                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　ブーストスキル効果種類:" + activity.m_Category_BoostSkillCategory_PROPERTY.ToString());
                                    switch (activity.m_Category_BoostSkillCategory_PROPERTY)
                                    {
                                        case MasterDataDefineLabel.BoostSkillCategory.HEAL_HP_RATE:
                                        case MasterDataDefineLabel.BoostSkillCategory.HEAL_HP_FIX:
                                            InGameUtilBattle.HealHPSkillProc(activity);             // HP回復
                                            SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);        // 音再生
                                            break;

                                        case MasterDataDefineLabel.BoostSkillCategory.HEAL_SP:
                                            InGameUtilBattle.HealSPSkillProc(activity);             // SP回復
                                            SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);        // 音再生
                                            break;

                                        case MasterDataDefineLabel.BoostSkillCategory.CNG_HAND_PANEL: m_SkillBoost.ChangeHandPanel(activity); break;
                                        case MasterDataDefineLabel.BoostSkillCategory.CNG_HAND_ELEM: m_SkillBoost.ChangeHandElement(activity); break;
                                        case MasterDataDefineLabel.BoostSkillCategory.CNG_FIELD: m_SkillBoost.ChangeField(); break;
                                        case MasterDataDefineLabel.BoostSkillCategory.ATK_ELEM_TARGET: m_SkillBoost.AttackElementTarget(); break;
                                        case MasterDataDefineLabel.BoostSkillCategory.REDUCE_LBS_TURN: m_SkillBoost.ReduceLBSTurn(activity); break;
                                        case MasterDataDefineLabel.BoostSkillCategory.CLEAR_ABSTATE: m_SkillBoost.ClearAbstate(); break;

                                        case MasterDataDefineLabel.BoostSkillCategory.BATTLEFIELD_PANEL:
                                            InGameUtilBattle.SkillBattlefieldPanel(activity);
                                            SoundUtil.PlaySE(SEID.SE_BATTLE_BUFF);
                                            break;

                                        case MasterDataDefineLabel.BoostSkillCategory.NONE:
                                        default:
                                            break;
                                    }
                                }
                            }
                            break;

                    } // switch ( activity.m_SkillType )


                    //----------------------------------------
                    //	ダメージ処理
                    //----------------------------------------
                    if (damage_enable == true)
                    {

                        if (enemy_params != null)
                        {

                            BattleEnemy enemy_param = enemy_params[cBattleTarget.m_TargetNum];
                            if (enemy_param != null)
                            {

                                //----------------------------------------
                                // ダメージ軽減
                                //----------------------------------------
                                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　敵" + cBattleTarget.m_TargetNum.ToString() + "ダメージ軽減");
                                damage = InGameUtilBattle.DamageReduce(damage, elem, enemy_param.m_StatusAilmentChara,
                                                                 enemy_param.m_EnemyDefense, ignoreDef, ignoreDefAilment, ignoreDefBarrier);


                                //----------------------------------------
                                // HPから減算
                                //----------------------------------------
                                m_Owner.m_BattleLogicEnemy.AddDamageEnemyHP(cBattleTarget.m_TargetNum, damage, eDamageType, activity.m_SkillParamOwnerNum);
#if BUILD_TYPE_DEBUG
                                BattleDebugMenu.DispDamageEnemy(activity, enemy_param, damage);
#endif

                                //----------------------------------------
                                // 即死効果
                                //----------------------------------------
                                if (bDeath == true)
                                {
                                    // 強制的にHPを0に設定
                                    enemy_param.SetHP(0);

                                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　即死効果"
                                        + "  敵" + cBattleTarget.m_TargetNum.ToString() + "HP:→" + enemy_param.m_EnemyHP.ToString()
                                    );
                                }

                                //----------------------------------------
                                // 吸血効果
                                //----------------------------------------
                                if (activity.m_skill_absorb != 0)
                                {

                                    // 死亡している場合
                                    if (enemy_param.isDead() == true)
                                    {
                                        // 既に非表示化されていれば処理しない
                                        // @change	Developer 2016/04/28 v340 bugweb5644対応
                                        // @note	v240で修正したが、v300で変数の扱いが変更され再発。判定方法をBattleManagerに合わせて修正。
                                        if (enemy_param.isShow() == false)
                                        {
                                            continue;
                                        }
                                    }


                                    float absorb_rate = InGameUtilBattle.GetDBRevisionValue(activity.m_skill_absorb);
                                    int absorb_total = (int)InGameUtilBattle.AvoidErrorMultiple(damage, absorb_rate);
                                    BattleSceneUtil.MultiInt absorb = new BattleSceneUtil.MultiInt();
                                    if (BattleParam.IsKobetsuHP)
                                    {
                                        // 吸血分を生存者で山分け
                                        CharaOnce[] alive_members = BattleParam.m_PlayerParty.getPartyMembers(CharaParty.CharaCondition.ALIVE);
                                        int alive_member_count = alive_members.Length;
                                        if (alive_member_count > 0)
                                        {
                                            absorb_total /= alive_member_count;
                                            absorb.setValue(GlobalDefine.PartyCharaIndex.MAX, absorb_total);
                                        }
                                    }
                                    else
                                    {
                                        absorb.setValue(nOwner, absorb_total);
                                    }


                                    //----------------------------------------
                                    // 状態異常確認
                                    // @add Developer 2016/05/30 v350 回復不可[全]対応
                                    //----------------------------------------
                                    // 状態異常：回復不可[全]の場合
                                    for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                                    {
                                        StatusAilmentChara ailmnet_chara = BattleParam.m_PlayerParty.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx);
                                        if (ailmnet_chara != null
                                            && ailmnet_chara.GetNonRecoveryAll()
                                        )
                                        {
                                            // 回復量を0へ
                                            absorb.setValue((GlobalDefine.PartyCharaIndex)idx, 0);
                                        }
                                    }


                                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　吸血効果発動 ", false);
                                    BattleParam.m_PlayerParty.RecoveryHP(absorb, true, true);

                                    // 音再生
                                    SoundUtil.PlaySE(SEID.SE_BATTLE_SKILL_HEAL);
                                }
                            }
                        }

                    }
                }
            }

            if (BattleParam.IsKobetsuHP)
            {
                // 復活処理
                ResurrectInfo resurrect_info = activity.getResurrectInfo();
                if (resurrect_info != null)
                {
                    BattleSceneUtil.MultiInt old_hp = new BattleSceneUtil.MultiInt(BattleParam.m_PlayerParty.m_HPCurrent);
                    BattleParam.m_PlayerParty.Resurrect(resurrect_info.m_FixCount, 100, resurrect_info.m_HpPercent, resurrect_info.m_FixSpUse);
                    BattleParam.m_PlayerParty.Resurrect(resurrect_info.m_AddCount, resurrect_info.m_AddChancePercent, resurrect_info.m_HpPercent, resurrect_info.m_AddSpUse);
                    BattleSceneUtil.MultiInt current_hp = new BattleSceneUtil.MultiInt(BattleParam.m_PlayerParty.m_HPCurrent);

                    for (int idx = 0; idx < old_hp.getMemberCount(); idx++)
                    {
                        if (old_hp.getValue((GlobalDefine.PartyCharaIndex)idx) == 0
                            && current_hp.getValue((GlobalDefine.PartyCharaIndex)idx) > 0
                            )
                        {
                            m_ResurrectBitFlag |= 1 << idx;
                        }
                    }

                }

                // 復活ユニット表示
                if (activity.m_SkillType != ESKILLTYPE.eACTIVE)
                {
                    BattleSkillCutinManager.Instance.setResurrectInfo(m_ResurrectBitFlag);
                    m_ResurrectBitFlag = 0;
                }
                else
                {
                    // ノーマルスキルの時は最後にまとめて
                    int skill_progress = request_param.getCurrentProgress() - 1;    // nextProgress()で一つ進んでいるので戻す.
                    int skill_count = request_param.getSkillActivityCount();
                    if (skill_progress >= skill_count - 1)
                    {
                        BattleSkillCutinManager.Instance.setResurrectInfo(m_ResurrectBitFlag);
                        m_ResurrectBitFlag = 0;
                    }
                }

                // ヘイト処理
                {
                    int hate_value = activity.getHateValue();
                    BattleParam.m_PlayerParty.addHate(activity.m_SkillParamOwnerNum, hate_value);
                }

                //挑発発動
                {
                    int provoke_turn = activity.getProvokeTurn();
                    if (provoke_turn > 0)
                    {
                        BattleParam.m_PlayerParty.provoke(activity.m_SkillParamOwnerNum, provoke_turn);
                    }
                }
            }

            //----------------------------------------
            //	反動ダメージ
            //----------------------------------------
            {

                // 固定量
                if (kickback.getValue(GlobalDefine.PartyCharaIndex.MAX) != 0)
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "反動ダメージ（固定量） ["
                        + "LEADER:" + kickback.getValue(GlobalDefine.PartyCharaIndex.LEADER)
                        + " MOB_1:" + kickback.getValue(GlobalDefine.PartyCharaIndex.MOB_1)
                        + " MOB_2:" + kickback.getValue(GlobalDefine.PartyCharaIndex.MOB_2)
                        + " MOB_3:" + kickback.getValue(GlobalDefine.PartyCharaIndex.MOB_3)
                        + " FRIEND:" + kickback.getValue(GlobalDefine.PartyCharaIndex.FRIEND)
                        + "]"
                    );

                    kickback.addValue(GlobalDefine.PartyCharaIndex.MAX, 1);
                    kickback.minValue(GlobalDefine.PartyCharaIndex.MAX, BattleParam.m_PlayerParty.m_HPCurrent);
                    kickback.subValue(GlobalDefine.PartyCharaIndex.MAX, 1);
                    kickback.maxValue(GlobalDefine.PartyCharaIndex.MAX, 0);
                    BattleParam.m_PlayerParty.DamageHP(kickback, new BattleSceneUtil.MultiInt(1), true, true, MasterDataDefineLabel.ElementType.NONE);
                    kickback.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);
                }

                // 割合
                if (kickback_rate.getValue(GlobalDefine.PartyCharaIndex.MAX) != 0)
                {

                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "反動ダメージ（割合） LEADER:"
                        + (kickback_rate.getValue(GlobalDefine.PartyCharaIndex.LEADER) * BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.LEADER) / 100).ToString()
                        + " = " + BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "(HpMax)"
                        + " x" + kickback_rate.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "%(反動割合)"
                    );
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "反動ダメージ（割合） MOB_1:"
                        + (kickback_rate.getValue(GlobalDefine.PartyCharaIndex.MOB_1) * BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MOB_1) / 100).ToString()
                        + " = " + BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MOB_1).ToString() + "(HpMax)"
                        + " x" + kickback_rate.getValue(GlobalDefine.PartyCharaIndex.MOB_1).ToString() + "%(反動割合)"
                    );
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "反動ダメージ（割合） MOB_2:"
                        + (kickback_rate.getValue(GlobalDefine.PartyCharaIndex.MOB_2) * BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MOB_2) / 100).ToString()
                        + " = " + BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MOB_2).ToString() + "(HpMax)"
                        + " x" + kickback_rate.getValue(GlobalDefine.PartyCharaIndex.MOB_2).ToString() + "%(反動割合)"
                    );
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "反動ダメージ（割合） MOB_3:"
                        + (kickback_rate.getValue(GlobalDefine.PartyCharaIndex.MOB_3) * BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MOB_3) / 100).ToString()
                        + " = " + BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MOB_3).ToString() + "(HpMax)"
                        + " x" + kickback_rate.getValue(GlobalDefine.PartyCharaIndex.MOB_3).ToString() + "%(反動割合)"
                    );
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "反動ダメージ（割合） FRIEND:"
                        + (kickback_rate.getValue(GlobalDefine.PartyCharaIndex.FRIEND) * BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.FRIEND) / 100).ToString()
                        + " = " + BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.FRIEND).ToString() + "(HpMax)"
                        + " x" + kickback_rate.getValue(GlobalDefine.PartyCharaIndex.FRIEND).ToString() + "%(反動割合)"
                    );

                    BattleSceneUtil.MultiFloat val = BattleParam.m_PlayerParty.m_HPMax.toMultiFloat();
                    val.mulValue(GlobalDefine.PartyCharaIndex.MAX, InGameUtilBattle.GetDBRevisionValue(1.0f));
                    kickback_rate.mulValueF(GlobalDefine.PartyCharaIndex.MAX, val);

                    kickback_rate.addValue(GlobalDefine.PartyCharaIndex.MAX, 1);
                    kickback_rate.minValue(GlobalDefine.PartyCharaIndex.MAX, BattleParam.m_PlayerParty.m_HPCurrent);
                    kickback_rate.subValue(GlobalDefine.PartyCharaIndex.MAX, 1);
                    kickback_rate.maxValue(GlobalDefine.PartyCharaIndex.MAX, 0);
                    BattleParam.m_PlayerParty.DamageHP(kickback_rate, new BattleSceneUtil.MultiInt(1), true, true, MasterDataDefineLabel.ElementType.NONE);
                    kickback_rate.setValue(GlobalDefine.PartyCharaIndex.MAX, 0);

                }

            }


            //----------------------------------------
            //	状態異常
            //----------------------------------------
            if (activity.m_nStatusAilmentDelay <= 0)
            {
                // 即時発動。
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：即時発動状態変化");
                m_Owner.m_BattleLogicAilment.SkillUpdate_StatusAilment(activity, nAtk, BattleParam.m_PlayerParty.m_HPMax, enemy_params);
            }
            else
            {
                // 次プレイヤーターン開始時発動。
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "スキル効果：遅延発動状態変化");
                m_Owner.m_BattleLogicAilment.ReserveDelayAilment(activity.m_SkillParamOwnerNum, ref activity.m_SkillParamTarget, activity.m_statusAilment_target, ref activity.m_statusAilment);
            }

            if (activity.m_SkillType == ESKILLTYPE.eLIMITBREAK)
            {
                // 実績を集計
                if (activity.m_SkillParamOwnerNum != GlobalDefine.PartyCharaIndex.HERO)
                {
                    BattleParam.m_AchievementTotalingInBattle.useLimitBreakSkill((int)activity.m_SkillParamSkillID);
                }
                else
                {
                    BattleParam.m_AchievementTotalingInBattle.useHeroSkill();
                }

                //----------------------------------------
                //	リミットブレイクスキルを使用した際には中断復帰情報を作成
                //	※中断復帰を利用して即死攻撃の判定を何度も行える問題への対応
                //----------------------------------------
                BattleParam.SaveLocalData();
            }
        }


        //----------------------------------------
        // スキルカットインの完遂を待つ
        //----------------------------------------
        if (BattleSkillCutinManager.Instance.isRunning() == true)
        {
            return false;
        }

        m_ResurrectBitFlag = 0;

        return true;
    }


    //----------------------------------------------------------------------------
    //	@brief		ノーマルスキル発行情報リセット
    //----------------------------------------------------------------------------
    public void ResetSkillRequestActive()
    {

        if (BattleSceneManager.Instance == null)
        {
            return;
        }

        if (m_SkillRequestActive == null)
        {
            return;
        }

        m_SkillRequestActive.clearRequest();
    }


    //----------------------------------------------------------------------------
    //	@brief		リーダースキル発行情報リセット
    //----------------------------------------------------------------------------
    public void ResetSkillRequestLeader()
    {

        if (BattleSceneManager.Instance == null)
        {
            return;
        }

        SkillRequestParam request_param = m_SkillRequestLeader;
        if (request_param == null)
        {
            return;
        }

        request_param.clearRequest();
    }


    //----------------------------------------------------------------------------
    //	@brief		パッシブスキル発行情報リセット
    //----------------------------------------------------------------------------
    public void ResetSkillRequestPassive()
    {

        if (BattleSceneManager.Instance == null)
        {
            return;
        }

        SkillRequestParam request_param = m_SkillRequestPassive;
        if (request_param == null)
        {
            return;
        }


        request_param.clearRequest();
    }


    //----------------------------------------------------------------------------
    //	@brief	LBSリクエスト情報のクリア
    //----------------------------------------------------------------------------
    public void ResetSkillRequestLimitBreak()
    {

        SkillRequestParam request_param = BattleParam.m_SkillRequestLimitBreak;
        if (request_param == null)
        {
            return;
        }

        request_param.clearRequest();

    }

    //----------------------------------------------------------------------------
    /*!
        @brief		敵ユニット攻撃更新：カウンター
        @param[in]	int		(damageValue)		受けるダメージの量
    */
    //----------------------------------------------------------------------------
    public void EnemyAttackAllCounter(int enemyIndex, BattleSceneUtil.MultiInt damageValue, BattleSceneUtil.MultiInt player_HP_before_damage)
    {
        // カウンターはパーティ全体で受けたダメージ合計に対して反撃するようにする（そうしないとカットインが大量に出るため）
        int party_damage = damageValue.getValue(GlobalDefine.PartyCharaIndex.MAX);

        //----------------------------------------
        //	ダメージがない場合はカウンターを発動させない
        //----------------------------------------
        if (party_damage <= 0)
        {
            return;
        }

        for (int member_idx = (int)GlobalDefine.PartyCharaIndex.LEADER; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
        {
            // 攻撃カウンターチェック
            if (InGameUtilBattle.PassiveChkSkillCounter(m_SkillRequestPassive, (GlobalDefine.PartyCharaIndex)member_idx, party_damage, enemyIndex) == true)
            {
                m_Owner.m_SkillCounter = true;
                m_Owner.m_PassiveSkillCounter = true;
            }

            // 回復カウンターチェック
            if (InGameUtilBattle.PassiveChkSkillCounterHeal(m_SkillRequestPassive, (GlobalDefine.PartyCharaIndex)member_idx, party_damage, enemyIndex) == true)
            {
                m_Owner.m_SkillCounter = true;
                m_Owner.m_PassiveSkillCounter = true;
            }
        }

        // リンクパッシブ処理
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        for (int member_idx = (int)GlobalDefine.PartyCharaIndex.LEADER; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; ++member_idx)
        {
            // 攻撃カウンターチェック
            if (InGameUtilBattle.PassiveChkSkillCounter(m_SkillRequestLinkPassive, (GlobalDefine.PartyCharaIndex)member_idx, party_damage, enemyIndex, ESKILLTYPE.eLINKPASSIVE) == true)
            {
                m_Owner.m_SkillCounter = true;
                m_Owner.m_LinkPassiveCounter = true;
            }

            // 回復カウンターチェック
            if (InGameUtilBattle.PassiveChkSkillCounterHeal(m_SkillRequestLinkPassive, (GlobalDefine.PartyCharaIndex)member_idx, party_damage, enemyIndex, ESKILLTYPE.eLINKPASSIVE) == true)
            {
                m_Owner.m_SkillCounter = true;
                m_Owner.m_LinkPassiveCounter = true;
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	スキル確定情報管理：情報クリア
    */
    //----------------------------------------------------------------------------
    public void ClrAttackFix()
    {
        for (int i = 0; i < m_AttackFixReq.Length; i++)
        {
            m_AttackFixReq[i] = null;
        }
        m_AttackFixReqInputNum = 0;
        m_SkillComboCountCalc = 0;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	スキル確定情報管理：情報構築
        @note	スキル作業領域からコピーする。作業領域側がFIXしていない状態でも取れるので外部的に同期対応が必要
    */
    //----------------------------------------------------------------------------

    public void SetAttackFix(BattleSkillReq[] skill_request, int skill_request_count, SkillRequestParam.SkillFilterType filter_type)
    {
        ClrAttackFix();

        m_AttackFixReqInputNum = 0;
        switch (filter_type)
        {
            case SkillRequestParam.SkillFilterType.ALL:
                m_SkillComboCountCalc = 0;
                m_SkillComboCountDisp = 0;
                break;

            case SkillRequestParam.SkillFilterType.FIRST_HALF:
                m_SkillComboCountCalc = 0;
                m_SkillComboCountDisp = 0;
                break;

            case SkillRequestParam.SkillFilterType.LAST_HALF:
                m_SkillComboCountCalc = 0;
                break;
        }
        for (int idx = 0; idx < skill_request_count; idx++)
        {
            m_AttackFixReq[m_AttackFixReqInputNum] = skill_request[idx];
            m_AttackFixReqInputNum++;

            uint skillID = m_AttackFixReq[idx].m_SkillParamSkillID;
            MasterDataSkillActive skillParam = BattleParam.m_MasterDataCache.useSkillActive(skillID);
            // 復活スキルはコンボ数に含めない
            if (skillParam != null && skillParam.isAlwaysResurrectSkill() == false)
            {
                m_SkillComboCountCalc++;
            }
        }

        for (int idx = 0; idx < m_ActiveSkillCostCount.Length; idx++)
        {
            m_ActiveSkillCostCount[idx] = 0;
        }

        {
            //----------------------------------------
            // ノーマルスキル発動判定
            //----------------------------------------
            int linkSkillNum = 0;
            if (m_AttackFixReqInputNum > 0)
            {
                // リンクスキル分の要素数を格納
                linkSkillNum = SkillLink.SKILL_LINK_MAX;
            }

            //----------------------------------------
            // スキル発動情報領域確保
            //----------------------------------------
            BattleSkillActivity[] skillActivityArray = new BattleSkillActivity[m_AttackFixReqInputNum + linkSkillNum];

            //----------------------------------------
            // とりあえずソート度外視でスキル発動時情報を埋める
            //----------------------------------------
            for (int i = 0; i < m_AttackFixReqInputNum; i++)
            {

                uint skillID = m_AttackFixReq[i].m_SkillParamSkillID;

                MasterDataSkillActive skillParam = BattleParam.m_MasterDataCache.useSkillActive(skillID);
                if (skillParam == null)
                {
                    continue;
                }

                BattleSkillActivity skillActivity = new BattleSkillActivity();
                if (skillActivity != null)
                {
                    skillActivity._setParam(skillParam);

                    skillActivity.m_SkillParamOwnerNum = m_AttackFixReq[i].m_SkillParamCharaNum;
                    skillActivity.m_SkillParamFieldID = m_AttackFixReq[i].m_SkillParamFieldNum;
                    skillActivity.m_SkillParamSkillID = m_AttackFixReq[i].m_SkillParamSkillID;

                    skillActivity.m_Effect = skillParam.effect;
                    skillActivity.m_Element = skillParam.skill_element;
                    skillActivity.m_Type = skillParam.skill_type;
                    skillActivity.m_SkillType = ESKILLTYPE.eACTIVE;
                    {
                        skillActivity.m_SkillIndex = 0;
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(skillActivity.m_SkillParamOwnerNum, CharaParty.CharaCondition.EXIST);
                        if (chara_once != null)
                        {
                            if (chara_once.m_CharaMasterDataParam.skill_active1 == skillActivity.m_SkillParamSkillID)
                            {
                                skillActivity.m_SkillIndex = 1;
                            }
                        }
                    }

                    skillActivity.m_skill_chk_atk_combo = MasterDataDefineLabel.BoolType.ENABLE;

                    skillActivity.m_Category_SkillCategory_PROPERTY = 0;
                    skillActivity.m_SkillParamTarget = null;

                    // @change Developer v320 ノーマルスキル振れ幅対応
                    // @change Developer	v330 ノーマルスキル特性対応
                    int nCriticalOdds = 0;
                    if (skillParam.Is_skill_active())
                    {
                        //----------------------------------------
                        // 特性判定
                        //----------------------------------------
                        int nSkillValue = 0;
                        int nSkillValueRand = 0;

                        if (InGameUtilBattle.ChkSkillAbility(skillParam, m_SkillComboCountCalc) == false)
                        {
                            // 通常値を設定
                            nSkillValue = skillParam.skill_value;
                            nSkillValueRand = skillParam.skill_value_rand;
                            nCriticalOdds = skillParam.skill_critical_odds;
                        }
                        else
                        {
                            // 特性値で設定
                            skillActivity.m_Effect = skillParam.ability_effect;     // スキルエフェクト
                            skillActivity.m_Type = skillParam.ability_type;         // スキルタイプ
                            nSkillValue = skillParam.ability_value;         // スキル効果値
                            nSkillValueRand = skillParam.ability_value_rand;    // スキル効果値振れ幅
                            nCriticalOdds = skillParam.ability_critical;        // クリティカル発動率
                        }

                        //----------------------------------------
                        // 振れ幅が設定してある場合
                        //----------------------------------------
                        if (nSkillValueRand != 0)
                        {
                            int nSkillPower = nSkillValue + nSkillValueRand;
                            if (nSkillPower < 0)
                            {
                                nSkillPower = 0;
                            }

                            // 最小値と最大値を確定：基準値より高い場合
                            uint unRandMin = 0;
                            uint unRandMax = 0;
                            if (nSkillPower > nSkillValue)
                            {
                                unRandMin = (uint)nSkillValue;
                                unRandMax = (uint)nSkillPower;
                            }
                            else
                            {
                                unRandMin = (uint)nSkillPower;
                                unRandMax = (uint)nSkillValue;
                            }

                            // スキル威力確定：振れ幅算出
                            skillActivity.m_skill_power = (int)RandManager.GetRand(unRandMin, unRandMax + 1);

                            // 効果値によるエフェクトの切り替え
                            skillActivity.m_Effect = InGameUtilBattle.SetSkillEffectToValue(skillActivity.m_Effect, skillActivity.m_Type, skillActivity.m_skill_power);
                        }
                        else
                        {
                            skillActivity.m_skill_power = nSkillValue;
                        }
                    }

                    // クリティカル判定
                    if (BattleSceneUtil.checkChancePercent(nCriticalOdds))
                    {
                        skillActivity.m_bCritical = true;
                    }
                    else
                    {
                        skillActivity.m_bCritical = false;
                    }
                    skillActivityArray[i] = skillActivity;

                }
                else
                {
                    skillActivityArray[i] = null;
                }

                // 各種属性のスキルが発動したフラグをたてる
                MasterDataDefineLabel.ElementType elem = skillParam.skill_element;
                if (elem > MasterDataDefineLabel.ElementType.NONE
                && elem < MasterDataDefineLabel.ElementType.MAX)
                {
                    m_ActiveSkillElement[(int)elem] = true;
                }

                {
                    int skill_cost = 0;
                    skill_cost += (skillParam.cost1 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0;
                    skill_cost += (skillParam.cost2 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0;
                    skill_cost += (skillParam.cost3 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0;
                    skill_cost += (skillParam.cost4 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0;
                    skill_cost += (skillParam.cost5 != MasterDataDefineLabel.ElementType.NONE) ? 1 : 0;

                    m_ActiveSkillCostCount[skill_cost - 1]++;
                }
            }

            //----------------------------------------
            // スキル発行情報を仮保存
            // SkillActivitySort内のGetDamageValueで、
            // 発動したノーマルスキルを判定するリーダースキルがある
            // 全チェックするため、発動順は問題ない
            // @Developer 2015/07/31 ver290
            //----------------------------------------
            m_SkillRequestActive.clearRequest();
            m_SkillRequestActive.addSkillRequest(skillActivityArray, skillActivityArray.Length);

            //-------------------------------
            // リンクスキルの発動情報作成
            //-------------------------------
            m_SkillLink.ActivityLinkSkill(m_SkillRequestActive);

            //-------------------------------
            // ノーマルスキルの配列に、リンクスキルを組み込む
            //-------------------------------
            m_SkillRequestActive.addSkillRequest(m_SkillLink.m_SkillRequestLink);

            // ソート
            m_SkillRequestActive.sortSkillRequest(filter_type);
        }
    }
}
