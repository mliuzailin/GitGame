/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	InGameUtil.cs
	@brief	インゲーム関連ユーティリティ
	@author Developer
	@date 	2012/10/09
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

using ServerDataDefine;


/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@class		InGameUtil
	@brief		インゲーム関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class InGameUtil
{
    static private bool m_Quest2 = false;

    //------------------------------------------------------------------------
    /*!
		@brief		バックアタックを回避するパッシブスキル
		@return		int		[-1 or 効果値]
		@change		Developer 2015/09/12 ver300	リンクパッシブ対応
	*/
    //------------------------------------------------------------------------
    static public int PassiveChkpassBackAttack(int pass_rate)
    {
        CharaOnce[] charaParty;
        MasterDataParamChara charaParam = null;
        MasterDataSkillPassive passiveParam = null;

        //------------------------------
        // パーティメンバー取得
        //------------------------------
        charaParty = SceneModeContinuousBattle.Instance.m_PlayerPartyChara;
        if (charaParty == null)
        {
            return (pass_rate);
        }

        //----------------------------------------
        // 最も有効な値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < charaParty.Length; ++num)
        {
            //------------------------------
            // エラーチェック
            //------------------------------
            if (charaParty[num] == null)
            {
                continue;
            }

            // キャラ情報取得
            charaParam = charaParty[num].m_CharaMasterDataParam;
            if (charaParam == null)
            {
                continue;
            }

            // パッシブスキル取得
            passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 有効無効を確認(該当スキルかチェック)
            //------------------------------
            MasterDataSkillPassive.BackAttackInfo back_attack_info = passiveParam.Get_BackAttackInfo();
            if (back_attack_info == null)
            {
                continue;
            }

            //------------------------------
            // 最も小さい値を取得
            //------------------------------
            if (pass_rate <= back_attack_info.m_BackAttackPercent)
            {
                continue;
            }

            pass_rate = back_attack_info.m_BackAttackPercent;
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < charaParty.Length; ++num)
        {
            //------------------------------
            // エラーチェック
            //------------------------------
            if (charaParty[num] == null
            || charaParty[num].m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            charaParam = BattleParam.m_MasterDataCache.useCharaParam(charaParty[num].m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブスキル取得
            passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            //------------------------------
            // 有効無効を確認(該当スキルかチェック)
            //------------------------------
            MasterDataSkillPassive.BackAttackInfo back_attack_info = passiveParam.Get_BackAttackInfo();
            if (back_attack_info == null)
            {
                continue;
            }

            //------------------------------
            // 最も小さい値を取得
            //------------------------------
            if (pass_rate <= back_attack_info.m_BackAttackPercent)
            {
                continue;
            }

            pass_rate = back_attack_info.m_BackAttackPercent;
        }
        #endregion

        return (pass_rate);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		指定色以上のスキルを成立させて攻撃すると、ダメージが指定倍率分UP
		@param[in]	int			(charaIdx)		キャラインデックス
		@return		float		[攻撃力倍率]
	*/
    //----------------------------------------------------------------------------
    static public float GetLeaderSkillDamageUPColor(GlobalDefine.PartyCharaIndex charaIdx)
    {
        float rate = 1.0f;

        if (BattleParam.isActiveBattle() == false
        || SceneModeContinuousBattle.Instance == null)
        {
            return rate;
        }

        if (SceneModeContinuousBattle.Instance.m_PlayerPartyChara == null)
        {
            return rate;
        }

        if (charaIdx < 0 || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return rate;
        }

        // フレンドであればフレンド登録状態のチェック
        if (ChkFriendRegisterStatus(charaIdx) == false)
        {
            return rate;
        }

        // スキルの所持者を取得
        CharaOnce chara = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[(int)charaIdx];
        if (chara == null)
        {
            return rate;
        }

        MasterDataParamChara charaParam = chara.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return rate;
        }

        // スキル情報の取得
        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(chara.m_CharaMasterDataParam.skill_leader);
        if (skillLeader == null)
        {
            return rate;
        }

        if (skillLeader.Is_skill_damageup_color_active() == false)
        {
            return rate;
        }

        int count = BattleSceneManager.Instance.getActiveSkillElementCount();
        if (count < skillLeader.skill_damageup_color_count)
        {
            return rate;
        }

        // DBから倍率を取得して倍率を計算
        float val = InGameUtilBattle.GetDBRevisionValue(skillLeader.skill_damageup_color_rate);
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, val);

        return rate;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		指定HANDS以上でスキルを成立させて攻撃すると、ダメージが指定倍率分UP
		@param[in]	int			(charaIdx)		キャラインデックス
		@return		float		[攻撃力倍率]
	*/
    //----------------------------------------------------------------------------
    static public float GetLeaderSkillDamageUPHands(GlobalDefine.PartyCharaIndex charaIdx)
    {
        float rate = 1.0f;

        if (BattleParam.isActiveBattle() == false
        || SceneModeContinuousBattle.Instance == null)
        {
            return rate;
        }

        if (SceneModeContinuousBattle.Instance.m_PlayerPartyChara == null)
        {
            return rate;
        }

        if (charaIdx < 0 || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return rate;
        }

        // フレンドであればフレンド登録状態のチェック
        if (ChkFriendRegisterStatus(charaIdx) == false)
        {
            return rate;
        }

        // スキルの所持者を取得
        CharaOnce chara = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[(int)charaIdx];
        if (chara == null)
        {
            return rate;
        }

        MasterDataParamChara charaParam = chara.m_CharaMasterDataParam;
        if (charaParam == null)
        {
            return rate;
        }

        // スキル情報の取得
        MasterDataSkillLeader skillLeader = BattleParam.m_MasterDataCache.useSkillLeader(chara.m_CharaMasterDataParam.skill_leader);
        if (skillLeader == null)
        {
            return rate;
        }

        if (skillLeader.Is_skill_damageup_hands_active() == false)
        {
            return rate;
        }

        int count = BattleSceneManager.Instance.getActiveSkillComboCount();
        if (count < skillLeader.skill_damageup_hands_count)
        {
            return rate;
        }

        // DBから倍率を取得して倍率を計算
        float val = InGameUtilBattle.GetDBRevisionValue(skillLeader.skill_damageup_hands_rate);
        rate = InGameUtilBattle.AvoidErrorMultiple(rate, val);

        return rate;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		フレンド登録状態でリーダースキルが発動できるかチェック
		@param[in]	int		(charaIdx)		キャラインデックス
		@retval		[発動可能/発動不能]
	*/
    //----------------------------------------------------------------------------
    static public bool ChkFriendRegisterStatus(GlobalDefine.PartyCharaIndex charaIdx)
    {
        // フレンドであればフレンド登録状態のチェック
        bool enable = true;

        // Ver260：フレンド制限廃止のため、フレンド枠のリーダースキルを無条件発動
        //if ( charaIdx == GlobalDefine.PartyCharaIndex.FRIEND ) {
        //	switch ( InGameManager.Instance.m_FriendRegisterState ) {
        //	case (int)ServerDataDefine.FRIEND_STATE.FRIEND_STATE_SUCCESS:   enable = true;  break;
        //	case (int)ServerDataDefine.FRIEND_STATE.FRIEND_STATE_WAIT_ME:   enable = false; break;
        //	case (int)ServerDataDefine.FRIEND_STATE.FRIEND_STATE_WAIT_HIM:  enable = false; break;
        //	case (int)ServerDataDefine.FRIEND_STATE.FRIEND_STATE_UNRELATED: enable = false; break;
        //	case (int)ServerDataDefine.FRIEND_STATE.FRIEND_STATE_PREMIUM:   enable = true;  break;
        //	default: break;
        //	}
        //}

        return enable;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		パッシブスキル：移動リジェネ	<static>
		@param[out]	ref int			(owner)			スキル発動者
		@return		float			[回復：割合]
		@note		InGame関連以外からの呼出しは行わないでください。
		@change		Developer 2015/09/11 ver300	リンクパッシブ対応
	*/
    //----------------------------------------------------------------------------
    static public float PassiveChkHealHPMove(ref GlobalDefine.PartyCharaIndex owner)
    {
        float rate = 0.0f;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (SceneGoesParam.Instance == null
        || SceneModeContinuousBattle.Instance == null)
        {
            return (rate);
        }


        //------------------------------
        // パーティメンバー取得
        //------------------------------
        CharaOnce[] charaParty;
        charaParty = SceneModeContinuousBattle.Instance.m_PlayerPartyChara;
        if (charaParty == null)
        {
            return (rate);
        }

        MasterDataParamChara charaParam = null;
        MasterDataSkillPassive passiveParam = null;
        float tempRate = 0.0f;

        //----------------------------------------
        // 最も有効な値を取得
        // ※パッシブとリンクパッシブは、念のため切り分けておく
        //----------------------------------------
        //------------------------------
        // パッシブスキル：全キャラチェック
        //------------------------------
        #region ==== パッシブスキル処理 ====
        for (int num = 0; num < charaParty.Length; ++num)
        {
            // メンバーが設定されているかチェック
            if (charaParty[num] == null)
            {
                continue;
            }

            // キャラ情報取得
            charaParam = charaParty[num].m_CharaMasterDataParam;
            if (charaParam == null)
            {
                continue;
            }

            // パッシブスキル取得
            passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            // 発動者を仮設定(フロア開始時カットイン用：初期値でなければよい)
            if (owner == GlobalDefine.PartyCharaIndex.MAX)
            {
                owner = (GlobalDefine.PartyCharaIndex)Enum.ToObject(typeof(GlobalDefine.PartyCharaIndex), num);
            }

            //----------------------------------------
            // 移動リジェネ
            //----------------------------------------
            tempRate = PassiveHealHPMove(passiveParam);

            //----------------------------------------
            // 移動時回復選定(割合)：重複対策
            //----------------------------------------
            if (rate >= tempRate)
            {
                continue;
            }

            rate = tempRate;
            owner = (GlobalDefine.PartyCharaIndex)Enum.ToObject(typeof(GlobalDefine.PartyCharaIndex), num);
        }
        #endregion

        //------------------------------
        // リンクパッシブ：全リンクキャラチェック
        //------------------------------
        #region ==== リンクパッシブ処理 ====
        for (int num = 0; num < charaParty.Length; ++num)
        {
            // メンバーが設定されているかチェック
            if (charaParty[num] == null
            || charaParty[num].m_LinkParam == null)
            {
                continue;
            }

            // リンクキャラ情報取得
            charaParam = BattleParam.m_MasterDataCache.useCharaParam(charaParty[num].m_LinkParam.m_CharaID);
            if (charaParam == null)
            {
                continue;
            }

            // リンクパッシブ取得
            passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
            if (passiveParam == null)
            {
                continue;
            }

            // 発動者を仮設定(フロア開始時カットイン用：初期値でなければよい)
            if (owner == GlobalDefine.PartyCharaIndex.MAX)
            {
                owner = (GlobalDefine.PartyCharaIndex)Enum.ToObject(typeof(GlobalDefine.PartyCharaIndex), num);
            }

            //----------------------------------------
            // 移動リジェネ
            //----------------------------------------
            tempRate = PassiveHealHPMove(passiveParam);

            //----------------------------------------
            // 移動時回復選定(割合)：重複対策
            //----------------------------------------
            if (rate >= tempRate)
            {
                continue;
            }

            rate = tempRate;
            owner = (GlobalDefine.PartyCharaIndex)Enum.ToObject(typeof(GlobalDefine.PartyCharaIndex), num);
        }
        #endregion

        return (rate);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		パッシブスキル：移動リジェネ	<static>
		@param[in]	MasterDataSkillPassive	(passiveParam)	スキルマスター
		@return		float					(rate)			[回復：割合]
		@note		InGame関連以外からの呼出しは行わないでください。
					Developer 2015/09/11 ver300 根本の処理部分を分離
	*/
    //----------------------------------------------------------------------------
    static public float PassiveHealHPMove(MasterDataSkillPassive passiveParam)
    {
        float rate = 0.0f;

        //------------------------------
        // 該当スキルかどうかチェック
        //------------------------------
        if (passiveParam == null
        || passiveParam.skill_type != MasterDataDefineLabel.PassiveSkillCategory.HEAL_HP_MOVE)
        {
            return (rate);
        }

        //----------------------------------------
        // 割合取得
        //----------------------------------------
        rate = InGameUtilBattle.GetDBRevisionValue(passiveParam.Get_HEAL_HP_MOVE_RATE());

        return (rate);
    }

    //------------------------------------------------------------------------
    /*!
		@brief		パッシブスキル発動チェック(クエスト開始時のカットイン)<static>
		@param[out]	ref uint[]	(aPassiveID)	発動判定フラグ兼スキルID
		@param[in]	ESKILLTYPE	(skillType)		スキルタイプ
		@retval		bool		[発動/発動しない]
	*/
    //------------------------------------------------------------------------
    static public bool CheckPrePassiveSkill(ref uint[] aPassiveID, ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE)
    {
        bool result = false;

        //------------------------------
        // エラーチェック
        //------------------------------
        if (SceneModeContinuousBattle.Instance == null
        || SceneModeContinuousBattle.Instance.m_PlayerPartyChara == null)
        {
            return (result);
        }


        CharaOnce[] aCharaParty;
        MasterDataParamChara charaParam = null;
        MasterDataSkillPassive passiveParam = null;
        GlobalDefine.PartyCharaIndex[] aOwnerIdx = new GlobalDefine.PartyCharaIndex[(int)GlobalDefine.PartyCharaIndex.MAX];

        //------------------------------
        // パーティメンバー取得
        //------------------------------
        aCharaParty = SceneModeContinuousBattle.Instance.m_PlayerPartyChara;
        if (aCharaParty == null)
        {
            return (result);
        }

        //------------------------------
        // 全てのキャラのパッシブスキルをチェック
        //------------------------------
        for (int num = 0; num < aCharaParty.Length; ++num)
        {
            // 発動者をパーティ外で初期化
            aOwnerIdx[num] = GlobalDefine.PartyCharaIndex.MAX;

            // メンバーが設定されているかチェック
            if (aCharaParty[num] == null
            || !aCharaParty[num].m_bHasCharaMasterDataParam)
            {
                continue;
            }

            //------------------------------
            // スキルタイプによる分岐
            //------------------------------
            switch (skillType)
            {
                //------------------------------
                // パッシブスキル
                //------------------------------
                #region ==== パッシブスキル処理 ====
                case ESKILLTYPE.ePASSIVE:
                    // キャラ情報取得
                    charaParam = aCharaParty[num].m_CharaMasterDataParam;
                    if (charaParam == null)
                    {
                        continue;
                    }

                    // パッシブスキル取得
                    passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.skill_passive);
                    if (passiveParam == null)
                    {
                        continue;
                    }

                    // スキルタイプによる分岐
                    switch (passiveParam.skill_type)
                    {
                        // 各スキルで優先される発動者を取得する
                        case MasterDataDefineLabel.PassiveSkillCategory.HEAL_HP_MOVE: PassiveChkHealHPMove(ref aOwnerIdx[num]); break;      // 移動リジェネ
                        case MasterDataDefineLabel.PassiveSkillCategory.HEAL_HP_BATTLE: InGameUtilBattle.PassiveChkHealHPBattle(ref aOwnerIdx[num]); break;     // バトルリジェネ
                        case MasterDataDefineLabel.PassiveSkillCategory.CNG_HAND_ELEM: InGameUtilBattle.PassiveChkChangeHandElem(0, ref aOwnerIdx[num]); break;     // カード変換
                                                                                                                                                                    //						case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_FLOOR_PNL:	InGameUtilBattle.PassiveChkDamageUPFloorPanel( ref aOwnerIdx[ num ] );	break;		// めくりパネル数に応じた攻撃力補正
                                                                                                                                                                    //						case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_ELEM_NUM:		InGameUtilBattle.PassiveChkDamageUPElemNum(	  ref aOwnerIdx[ num ] );	break;		// 指定色以上で攻撃をした場合、攻撃力が指定倍率UP
                                                                                                                                                                    //						case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_HANDS_NUM:	InGameUtilBattle.PassiveChkDamageUPHandsNum(	  ref aOwnerIdx[ num ] );	break;		// 指定HANDS以上で攻撃をした場合、攻撃力が指定倍率UP
                        default: aOwnerIdx[num] = GlobalDefine.PartyCharaIndex.MAX; break;
                    }
                    break;
                #endregion

                //------------------------------
                // リンクパッシブ
                //------------------------------
                #region ==== リンクパッシブ処理 ====
                case ESKILLTYPE.eLINKPASSIVE:

                    // メンバーが設定されているかチェック
                    if (aCharaParty[num].m_LinkParam == null)
                    {
                        continue;
                    }

                    // リンクキャラ情報取得
                    charaParam = BattleParam.m_MasterDataCache.useCharaParam(aCharaParty[num].m_LinkParam.m_CharaID);
                    if (charaParam == null)
                    {
                        continue;
                    }

                    // リンクパッシブ取得
                    passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
                    if (passiveParam == null)
                    {
                        continue;
                    }

                    // スキルタイプによる分岐
                    switch (passiveParam.skill_type)
                    {
                        // 各スキルの発動者を取得する
                        case MasterDataDefineLabel.PassiveSkillCategory.HEAL_HP_MOVE: PassiveChkHealHPMove(ref aOwnerIdx[num]); break;      // 移動リジェネ
                        case MasterDataDefineLabel.PassiveSkillCategory.HEAL_HP_BATTLE: InGameUtilBattle.PassiveChkHealHPBattle(ref aOwnerIdx[num]); break;     // バトルリジェネ
                        case MasterDataDefineLabel.PassiveSkillCategory.CNG_HAND_ELEM: InGameUtilBattle.PassiveChkChangeHandElem(0, ref aOwnerIdx[num]); break;     // カード変換
                                                                                                                                                                    //						case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_FLOOR_PNL:	InGameUtilBattle.PassiveChkDamageUPFloorPanel( ref aOwnerIdx[ num ], ESKILLTYPE.eLINKPASSIVE, charaParam.fix_id );	break;		// めくりパネル数に応じた攻撃力補正
                                                                                                                                                                    //						case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_ELEM_NUM:		InGameUtilBattle.PassiveChkDamageUPElemNum(	  ref aOwnerIdx[ num ], ESKILLTYPE.eLINKPASSIVE, charaParam.fix_id );	break;		// 指定色以上で攻撃をした場合、攻撃力が指定倍率UP
                                                                                                                                                                    //						case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_HANDS_NUM:	InGameUtilBattle.PassiveChkDamageUPHandsNum(	  ref aOwnerIdx[ num ], ESKILLTYPE.eLINKPASSIVE, charaParam.fix_id );	break;		// 指定HANDS以上で攻撃をした場合、攻撃力が指定倍率UP
                        default: aOwnerIdx[num] = GlobalDefine.PartyCharaIndex.MAX; break;
                    }
                    break;
                    #endregion
            }

            // カットインの方法による分岐
            switch (passiveParam.skill_type)
            {
                // 優先発動者以外、カットインを表示させないスキル（一番効果値が高い一人だけを表示）
                case MasterDataDefineLabel.PassiveSkillCategory.HEAL_HP_MOVE:
                case MasterDataDefineLabel.PassiveSkillCategory.HEAL_HP_BATTLE:
                case MasterDataDefineLabel.PassiveSkillCategory.CNG_HAND_ELEM:
                    if ((int)aOwnerIdx[num] != num)
                    {
                        aOwnerIdx[num] = GlobalDefine.PartyCharaIndex.MAX;
                    }
                    break;

                // 所持していたら、カットインを表示させるスキル
                case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_FLOOR_PNL:
                case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_ELEM_NUM:
                //				case MasterDataDefineLabel.PassiveSkillCategory.DMGUP_HANDS_NUM:
                case MasterDataDefineLabel.PassiveSkillCategory.BATTLEFIELD_PANEL:
                    if ((int)aOwnerIdx[num] != num)
                    {
                        aOwnerIdx[num] = (GlobalDefine.PartyCharaIndex)Enum.ToObject(typeof(GlobalDefine.PartyCharaIndex), num);
                    }
                    break;
            }

            // 所持していたら、カットインを表示させるスキル
            if (passiveParam.Get_HandsAttackUpInfo() != null)
            {
                aOwnerIdx[num] = (GlobalDefine.PartyCharaIndex)num;
            }
        }

        //------------------------------
        // 発動者/発動情報の整理
        //------------------------------
        for (int num = 0; num < aOwnerIdx.Length; ++num)
        {
            // いちお初期化
            aPassiveID[num] = 0;

            // パーティ外の場合、処理しない
            if (aOwnerIdx[num] == GlobalDefine.PartyCharaIndex.MAX)
            {
                continue;
            }

            // メンバーが設定されているかチェック
            if (aCharaParty[(int)aOwnerIdx[num]] == null
            || !aCharaParty[(int)aOwnerIdx[num]].m_bHasCharaMasterDataParam)
            {
                continue;
            }

            //------------------------------
            // スキルタイプによる分岐
            //------------------------------
            switch (skillType)
            {
                //------------------------------
                // パッシブスキル
                //------------------------------
                #region ==== パッシブスキル処理 ====
                case ESKILLTYPE.ePASSIVE:
                    // キャラ情報取得
                    charaParam = aCharaParty[(int)aOwnerIdx[num]].m_CharaMasterDataParam;
                    if (charaParam == null)
                    {
                        continue;
                    }

                    // パッシブスキル取得
                    passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.skill_passive);
                    if (passiveParam == null)
                    {
                        continue;
                    }
                    break;
                #endregion

                //------------------------------
                // リンクパッシブ
                //------------------------------
                #region ==== リンクパッシブ処理 ====
                case ESKILLTYPE.eLINKPASSIVE:
                    // リンクキャラ情報取得
                    charaParam = BattleParam.m_MasterDataCache.useCharaParam(aCharaParty[num].m_LinkParam.m_CharaID);
                    if (charaParam == null)
                    {
                        continue;
                    }

                    // リンクパッシブ取得
                    passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
                    if (passiveParam == null)
                    {
                        continue;
                    }
                    break;
                    #endregion
            }

            aPassiveID[num] = passiveParam.fix_id;
            result = true;
        }

        return (result);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		セル中心位置取得			<static>
		@param[in]	int		(nCellX)		X座標
		@param[in]	int		(nCellY)		Y座標
		@return		Vector3	[セル座標]
	*/
    //----------------------------------------------------------------------------
    static public Vector3 GetCellCenter(int nCellX, int nCellZ)
    {
        Vector3 vRet;
        vRet.x = (float)nCellX * GlobalDefine.CELL_SIZE_X - (GlobalDefine.CELL_SIZE_X * (float)GlobalDefine.CELL_MAX_X * 0.5f);
        vRet.y = 0.0f;
        vRet.z = (float)nCellZ * GlobalDefine.CELL_SIZE_Z - (GlobalDefine.CELL_SIZE_Z * (float)GlobalDefine.CELL_MAX_Z * 0.5f);

        //------------------------------
        // 奇数の場合は中心を半分ずらす
        //------------------------------
        if (GlobalDefine.CELL_MAX_X % 2 == 1)
        {
            vRet.x += GlobalDefine.CELL_SIZE_X * 0.5f;
        }
        if (GlobalDefine.CELL_MAX_Z % 2 == 1)
        {
            vRet.z += GlobalDefine.CELL_SIZE_Z * 0.5f;
        }

#if true
        //------------------------------
        // 奥から手前に数えるように補正
        //------------------------------
        vRet.z *= -1.0f;
#endif

        return vRet;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		リーダースキル発動チェック			<static>
		@param[in]	int			(charaID)		キャラID
		@retval		bool		[発動/発動しない]
	*/
    //------------------------------------------------------------------------
    static public bool CheckPreLeaderSkill(GlobalDefine.PartyCharaIndex nCharaID, CharaOnce[] PlayerPartyChara)
    {
        if (nCharaID < 0 || nCharaID >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return false;
        }

        // フレンドであればフレンド登録状態のチェック
        if (ChkFriendRegisterStatus(nCharaID) == false)
        {
            return false;
        }

        if (PlayerPartyChara == null)
        {
            return false;
        }

        CharaOnce chara = PlayerPartyChara[(int)nCharaID];
        if (chara == null)
        {
            return false;
        }

        if (!chara.m_bHasCharaMasterDataParam)
        {
            return false;
        }

        MasterDataSkillLeader skill = BattleParam.m_MasterDataCache.useSkillLeader(chara.m_CharaMasterDataParam.skill_leader);
        if (skill == null)
        {
            return false;
        }

        bool ret = false;
        if (skill.Is_skill_decline_dmg_active())
        {
            // 被ダメ軽減
            ret = true;
        }
        else if (skill.Is_skill_hpdown_powup_active())
        {
            // 瀕死時攻撃力アップ
            ret = true;
        }
        else if (skill.Is_skill_hpfull_guard_active())
        {
            // 瀕死時ダメージ軽減
            ret = true;
        }
        else if (skill.Is_skill_hpfull_powup_active())
        {
            // 体力最大時に攻撃力アップ
            ret = true;
        }
        else if (skill.Is_skill_initiative_atk_active())
        {
            // 先制攻撃発動割合変動
            ret = true;
        }
        else if (skill.Is_skill_quick_time_active())
        {
            // 戦闘時秒数補正
            ret = true;
        }
        else if (skill.Is_skill_recovery_battle_active())
        {
            // 戦闘時回復
            ret = true;
        }
        else if (skill.Is_skill_recovery_support_active())
        {
            // 回復量アップ
            ret = true;
        }
        else if (skill.Is_skill_recovery_atk_active())
        {
            // 攻撃時回復効果
            ret = true;
        }
        else if (skill.Is_skill_powup_kind_active())
        {
            // 該当種族のステータスアップ
            ret = true;
        }
        else if (skill.Is_skill_powup_elem_active())
        {
            // 該当属性のステータスアップ
            ret = true;
        }
        else if (skill.skill_mekuri_powup_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            // めくり枚数比例攻撃力アップ
            ret = true;
        }
        else if (skill.Is_skill_recovery_move_active())
        {
            // 移動リジェネ
            ret = true;
        }
        else if (skill.Is_skill_recovery_battle_active())
        {
            // バトルリジェネ
            ret = true;
        }
        else if (skill.Is_skill_transform_card_active())
        {
            // カード変換
            ret = true;
        }
        else if (skill.Is_skill_damageup_color_active())
        {
            // 指定色以上のエナジーパネルで攻撃をした場合、ダメージが指定倍率UP
            ret = true;
        }
        else if (skill.Is_skill_damageup_hands_active())
        {
            // 指定HANDS以上で攻撃をした場合、ダメージが指定倍率UP
            ret = true;
        }
        else if (skill.skill_type == MasterDataDefineLabel.LeaderSkillCategory.HP_ATK_POWUP)
        {
            // バラシフト
            ret = true;
        }
        else if (skill.skill_type == MasterDataDefineLabel.LeaderSkillCategory.DMGUP_CONDITION_COST)
        {
            // ダメ補正：コスト条件(属性指定＆パネル数指定)
            ret = true;
        }
        else if (skill.skill_type == MasterDataDefineLabel.LeaderSkillCategory.POINTUP_EXP_RANK)
        {
            // 取得値補正：ランク経験値
            ret = true;
        }
        else if (skill.skill_type == MasterDataDefineLabel.LeaderSkillCategory.BATTLEFIELD_PANEL)
        {
            // 場にパネルを配置
            ret = true;
        }
        else if (skill.skill_type == MasterDataDefineLabel.LeaderSkillCategory.DMG_ENEMY_ELEM)
        {
            // 被ダメ補正：エネミー属性
            ret = true;
        }
        return ret;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		プレイヤーパーティキャラがリーダースキルの効果対象になるかを調べる	<static>
		@param[in]	int		(charaID)		リーダースキル所持者
		@return		int		[対象になるかどうかを入力したビットフラグ]
	*/
    //------------------------------------------------------------------------
    public static int ChkLeaderSkillTarget(GlobalDefine.PartyCharaIndex charaID)
    {
        int flag = 0;
        if (charaID < 0 || charaID >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return flag;
        }

        if (SceneModeContinuousBattle.Instance == null)
        {
            return flag;
        }

        if (SceneModeContinuousBattle.Instance.m_PlayerPartyChara == null)
        {
            return flag;
        }

        CharaOnce chara = SceneModeContinuousBattle.Instance.m_PlayerPartyChara[(int)charaID];
        if (chara == null)
        {
            return flag;
        }

        if (!chara.m_bHasCharaMasterDataParam)
        {
            return flag;
        }

        MasterDataSkillLeader skill = BattleParam.m_MasterDataCache.useSkillLeader(chara.m_CharaMasterDataParam.skill_leader);
        if (skill == null)
        {
            return flag;
        }

        CharaOnce[] charaArray = SceneModeContinuousBattle.Instance.m_PlayerPartyChara;

        //------------------------------
        // スキル検索：リーダースキル複合化
        // @Developer 2015/07/31 ver290
        //------------------------------
        for (; skill != null;)
        {
            // 追撃
            if (skill.Is_skill_follow_atk_active())
            {
                flag |= 1 << (int)charaID;
            }

            // 移動リジェネ
            if (skill.Is_skill_recovery_move_active())
            {
                flag |= 1 << (int)charaID;
            }

            // 戦闘リジェネ
            if (skill.Is_skill_recovery_battle_active())
            {
                flag |= 1 << (int)charaID;
            }

            // カウントダウン時間変化
            if (skill.Is_skill_quick_time_active())
            {
                flag |= 1 << (int)charaID;
            }

            // 属性ステータスアップ
            if (skill.Is_skill_powup_elem_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null)
                    {
                        continue;
                    }

                    if (!charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    if (charaArray[i].m_CharaMasterDataParam.element != skill.skill_powup_elem_type)
                    {
                        continue;
                    }
                    flag |= 1 << i;
                }
            }

            // 種族ステータスアップ
            if (skill.Is_skill_powup_kind_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null)
                    {
                        continue;
                    }

                    if (!charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    if (charaArray[i].kind != skill.skill_powup_kind_type
                    && charaArray[i].kind_sub != skill.skill_powup_kind_type)
                    {
                        continue;
                    }
                    flag |= 1 << i;
                }
            }

            // 被ダメージ軽減
            if (skill.Is_skill_decline_dmg_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null)
                    {
                        continue;
                    }

                    if (!charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }
                    flag |= 1 << i;
                }
            }

            // 回復量アップ
            if (skill.Is_skill_recovery_support_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null
                    || !charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << i;
                }
            }

            // 攻撃時回復効果
            if (skill.Is_skill_recovery_atk_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null
                    || !charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << i;
                }
            }

            // 体力最大時にのみ演出を入れる？
            if (skill.Is_skill_hpfull_powup_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null
                    || !charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << i;
                }
            }

            // 体力減少時にのみ演出を入れる？
            if (skill.Is_skill_hpdown_powup_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null
                    || !charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << i;
                }
            }

            // めくったパネルの枚数に応じてパワーアップ
            if (skill.skill_mekuri_powup_active == MasterDataDefineLabel.BoolType.ENABLE)
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null
                    || !charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << i;
                }
            }

            // 体力最大時にのみ演出を入れる？
            if (skill.Is_skill_hpfull_guard_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null
                    || !charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << i;
                }
            }


            // 指定色以上のエナジーパネルで攻撃をした場合、ダメージが指定倍率UP
            if (skill.Is_skill_damageup_color_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null
                    || !charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << i;
                }
            }

            // 指定HANDS以上で攻撃をした場合、ダメージが指定倍率UP
            if (skill.Is_skill_damageup_hands_active())
            {
                for (int i = 0; i < charaArray.Length; i++)
                {
                    if (charaArray[i] == null
                    || !charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << i;
                }
            }

            // バラシフト
            if (skill.skill_type == MasterDataDefineLabel.LeaderSkillCategory.HP_ATK_POWUP)
            {

                bool skill_result = true;

                for (int i = 0; i < charaArray.Length; i++)
                {

                    if (charaArray[i] == null)
                    {
                        continue;
                    }

                    if (!charaArray[i].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }


                    bool chk_and = skill.Get_HP_ATK_POWUP_AND_CHK();
                    if (chk_and)
                    {

                        skill_result = true;

                        //----------------------------------------
                        //	ANDチェック
                        //----------------------------------------
                        //	種族チェック
                        MasterDataDefineLabel.KindType[] HP_ATK_POWUP_KIND_array = skill.GetArray_HP_ATK_POWUP_KIND();
                        for (int n = 0; n < HP_ATK_POWUP_KIND_array.Length; n++)
                        {
                            MasterDataDefineLabel.KindType kind_type = HP_ATK_POWUP_KIND_array[n];

                            if (kind_type == MasterDataDefineLabel.KindType.NONE)
                            {
                                continue;
                            }

                            if (kind_type == charaArray[i].kind
                            || kind_type == charaArray[i].kind_sub)
                            {
                                continue;
                            }

                            skill_result = false;
                            break;
                        }

                        //	属性チェック
                        MasterDataDefineLabel.ElementType[] HP_ATK_POWUP_ELEM_array = skill.GetArray_HP_ATK_POWUP_ELEM();
                        for (int n = 0; n < HP_ATK_POWUP_ELEM_array.Length; n++)
                        {
                            MasterDataDefineLabel.ElementType element_type = HP_ATK_POWUP_ELEM_array[n];

                            if (element_type == MasterDataDefineLabel.ElementType.NONE)
                            {
                                continue;
                            }

                            if (element_type == charaArray[i].m_CharaMasterDataParam.element)
                            {
                                continue;
                            }

                            skill_result = false;
                            break;
                        }

                    }
                    else
                    {

                        skill_result = false;

                        //----------------------------------------
                        //	ORチェック
                        //----------------------------------------
                        //	種族チェック
                        MasterDataDefineLabel.KindType[] HP_ATK_POWUP_KIND_array = skill.GetArray_HP_ATK_POWUP_KIND();
                        for (int n = 0; n < HP_ATK_POWUP_KIND_array.Length; n++)
                        {
                            MasterDataDefineLabel.KindType kind_type = HP_ATK_POWUP_KIND_array[n];

                            if (kind_type == MasterDataDefineLabel.KindType.NONE)
                            {
                                continue;
                            }

                            if (kind_type != charaArray[i].kind
                            && kind_type != charaArray[i].kind_sub)
                            {
                                continue;
                            }

                            skill_result = true;
                            break;
                        }

                        //	属性チェック
                        MasterDataDefineLabel.ElementType[] HP_ATK_POWUP_ELEM_array = skill.GetArray_HP_ATK_POWUP_ELEM();
                        for (int n = 0; n < HP_ATK_POWUP_ELEM_array.Length; n++)
                        {
                            MasterDataDefineLabel.ElementType element_type = HP_ATK_POWUP_ELEM_array[n];

                            if (element_type == MasterDataDefineLabel.ElementType.NONE)
                            {
                                continue;
                            }

                            if (element_type != charaArray[i].m_CharaMasterDataParam.element)
                            {
                                continue;
                            }

                            skill_result = true;
                            break;
                        }

                    }

                    if (skill_result == true)
                    {
                        flag |= 1 << i;
                    }
                }
            }

            // ダメ補正：コスト条件(属性指定＆パネル数指定)
            if (skill.skill_type == MasterDataDefineLabel.LeaderSkillCategory.DMGUP_CONDITION_COST)
            {
                for (int num = 0; num < charaArray.Length; ++num)
                {
                    if (charaArray[num] == null
                    || !charaArray[num].m_bHasCharaMasterDataParam)
                    {
                        continue;
                    }

                    flag |= 1 << num;
                }
            }


            //------------------------------
            // 追加リーダースキルを取得
            //------------------------------
            skill = BattleParam.m_MasterDataCache.useSkillLeader(skill.add_fix_id);
        }

        return flag;
    }

    //-----------------------------------------------------------------------
    /*!
		@brief		お金増加				<static>
		@param[in]	int		(floor)		階層
		@param[in]	int		(add)		追加量
		@param[in]	bool	(draw)		描画あり/なし
	*/
    //-----------------------------------------------------------------------
    static public void AddMoney(int floor, int add, bool draw)
    {

        // 獲得データにお金を追加
        if (InGameQuestData.Instance != null)
        {
            InGameQuestData.Instance.AddAcquireMoneyParam((int)add, floor);
        }
    }


    //-----------------------------------------------------------------------
    /*!
		@brief		お金減らし				<static>
		@param[in]	int		(floor)		階層
		@param[in]	int		(del)		減らす分のお金
	*/
    //-----------------------------------------------------------------------
    static public void DelMoney(int floor, int del)
    {
        if (InGameQuestData.Instance != null)
        {
            InGameQuestData.Instance.AddAcquireMoneyParam(-del, floor);
        }
    }

    //-----------------------------------------------------------------------
    /*!
		@brief		お金減らし				<static>
		@param[in]	int		(floor)		階層
		@param[in]	int		(rate)		割合(%)
	*/
    //-----------------------------------------------------------------------
    static public void DelMoneyRate(int floor, int rate)
    {
        if (InGameQuestData.Instance != null)
        {
            uint total = InGameQuestData.Instance.GetAcquireMoneyTotalFloor(floor);
            float revision = InGameUtilBattle.GetDBRevisionValue(rate);
            float temp;
            temp = InGameUtilBattle.AvoidErrorMultiple(-1.0f, total);
            temp = InGameUtilBattle.AvoidErrorMultiple(temp, revision);

            int del_money = (int)temp;
            InGameQuestData.Instance.AddAcquireMoneyParam(del_money, floor);
        }
    }


    //-----------------------------------------------------------------------
    /*!
		@brief		チケット増加
	*/
    //-----------------------------------------------------------------------
    static public bool AddTicket(int floor, int value)
    {
        //------------------------
        //	エラーチェック
        //------------------------
        if (InGameQuestData.Instance == null)
        {
            return false;
        }


        //------------------------
        //	チケット取得情報の追加
        //------------------------
        bool result = InGameQuestData.Instance.AddAcquireTicket(floor, value);
        if (result == false)
        {
            return false;
        }

        return true;
    }


    //-----------------------------------------------------------------------
    /*!
		@brief		チケット削除
	*/
    //-----------------------------------------------------------------------
    static public void DelTicket(int floor)
    {
        //------------------------
        //	エラーチェック
        //------------------------
        if (InGameQuestData.Instance == null)
        {
            return;
        }


        //------------------------
        //	指定階層のチケット取得情報の削除
        //------------------------
        InGameQuestData.Instance.DelAcquireTicketFloor(floor);

    }

    //-----------------------------------------------------------------------
    /*!
		@brief		ローカルデータセーブ		<static>
	*/
    //-----------------------------------------------------------------------
    public static void SaveLocalData()
    {
        if (TutorialManager.Instance != null
        && TutorialManager.Instance.TutorialChkAllFinish() == false)
        {
            return;
        }
        if (LocalSaveManager.Instance == null)
        {
            return;
        }

        LocalSaveManager.Instance.SaveFuncRestoreQuest2();

        //--------------------------------
        // コンティニュー課金後に一度ローカルセーブを更新したなら
        // コンティニュー処理の結果が定着したと判断して次のコンティニュー情報を構築
        //--------------------------------
        LocalSaveContinue cContinueParam = LocalSaveManager.Instance.LoadFuncInGameContinue();
        if (cContinueParam.nContinueNext != cContinueParam.nContinueCt)
        {
            cContinueParam.nContinueCt = cContinueParam.nContinueNext;
            LocalSaveManager.Instance.SaveFuncInGameContinue(cContinueParam);
        }
        //--------------------------------
        // コンティニュー課金後に一度ローカルセーブを更新したなら
        // コンティニュー処理の結果が定着したと判断して次のコンティニュー情報を構築
        //--------------------------------
        LocalSaveReset cResetParam = LocalSaveManager.Instance.LoadFuncInGameReset();
        if (cResetParam.nResetNext != cResetParam.nResetCt)
        {
            cResetParam.nResetCt = cResetParam.nResetNext;
            LocalSaveManager.Instance.SaveFuncInGameReset(cResetParam);
        }
    }

    //-----------------------------------------------------------------------
    /*!
		@brief		ローカルセーブデータの削除
	*/
    //-----------------------------------------------------------------------
    public static void RemoveLocalData()
    {
        if (SceneGoesParam.Instance == null)
        {
            return;
        }

        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = null;
    }


    //-----------------------------------------------------------------------
    /*!
		@brief		ローカルセーブデータの設定
	*/
    //-----------------------------------------------------------------------
    public static void SetLocalData()
    {
        if (SceneGoesParam.Instance == null)
        {
            return;
        }

        SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore = LocalSaveManager.Instance.LoadFuncGoesToQuest2Restore(); ;
    }


    //-----------------------------------------------------------------------
    /*!
		@brief		ローカルセーブデータの有無
	*/
    //-----------------------------------------------------------------------
    public static bool IsLocalData()
    {
        if (SceneGoesParam.Instance != null)
        {
            if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore != null) return true;
        }
        return false;
    }

    //-----------------------------------------------------------------------
    /*!
		@brief		ローカルセーブデータの戦闘情報取得
	*/
    //-----------------------------------------------------------------------
    public static RestoreBattle GetRestoreBattle()
    {
        if (SceneGoesParam.Instance != null)
        {
            if (SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore != null) return SceneGoesParam.Instance.m_SceneGoesParamToQuest2Restore.m_BattleRestore;
        }
        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		各種ウィンドウのオープンリクエストのタイミングチェック
	*/
    //----------------------------------------------------------------------------
    static public bool ChkInGameOpenWindowTiming()
    {
        if (BattleSceneManager.Instance != null
        && BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleActive == true)
        {
            //----------------------------------------
            // 戦闘中の入力フェイズ以外はメニューは開けない
            //----------------------------------------
            if (BattleParam.getBattlePhase() != BattleParam.BattlePhase.INPUT)
            {
                return false;
            }

            // オートプレイ中は開けない（オートプレイ中はユーザー入力乗っ取り処理が行われているがそれだけでは不十分）
            if (BattleSceneManager.Instance.AutoPlay.isPlaying())
            {
                return false;
            }
        }
        else
        {
            {
                return false;
            }
        }

        return true;
    }


    //------------------------------------------------------------------------
    /*!
		@brief		コンティニューが不可能なクエストかどうか確認
		@param[in]	uint	(missionID)		クエストID
		@retval		bool	[コンティニュー不可/可]
	*/
    //------------------------------------------------------------------------
    static public bool ChkNoContinueQuest(uint missionID)
    {
        return MasterDataUtil.GetQuestContiunueEnable(missionID);
    }
#if false// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
    //------------------------------------------------------------------------
    /*!
		@brief		リトライが不可能なクエストかどうか確認
		@param[in]	uint	(missionID)		クエストID
		@retval		bool	[リトライ可/不可]
	*/
    //------------------------------------------------------------------------
    static public bool ChkNoRetryQuest( uint missionID )
	{

		return MasterDataUtil.GetQuestRetryEnable( missionID );
	}
#endif
    //------------------------------------------------------------------------
    /*!
		@brief		HANDS系実績の解除
		@param[in]	int			(combo)		hands数
	*/
    //------------------------------------------------------------------------
    static public void UnlockAchievement_Hnads(int combo)
    {

        if (combo >= 5)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eHANDS_05);
        }

        if (combo >= 10)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eHANDS_10);
        }

        if (combo >= 15)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eHANDS_15);
        }

        if (combo >= 20)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eHANDS_20);
        }

        if (combo >= 25)
        {
            PlayGameServiceUtil.UnlockAchievement(EACHIEVEMENT.eHANDS_25);
        }
    }

    //------------------------------------------------------------------------
    /*!
		@brief		ダメージランキング用に最大ダメージを記録
		@param[in]	int			(damage)		ダメージ量
	*/
    //------------------------------------------------------------------------
    static public void SubmitDamageRanking(int damage)
    {
    }

    //------------------------------------------------------------------------
    /*!
		@brief		ダメージランキング登録
	*/
    //------------------------------------------------------------------------
    static public void SubmitDamageRanking()
    {
        // 登録
        System.Int64 nVal = (System.Int64)BattleParam.m_AchievementTotalingInBattle.getMaxDamageToEnemy();
        PlayGameServiceUtil.SubmitScore(ELEADERBORAD.eTOTAL_DAMAGE, nVal);
    }

    //------------------------------------------------------------------------
    /*!
		@brief		総合ターン数のカウントアップ
	*/
    //------------------------------------------------------------------------
    static public void IncrementTotalTurn()
    {
    }

    //------------------------------------------------------------------------
    /*!
		@brief		総合ターン数の登録
	*/
    //------------------------------------------------------------------------
    static public void SubmitTotalTurn()
    {
#if false
		if ( InGameManager.Instance == null ) {
			return;
		}

		if ( InGameManager.Instance.m_InGameClear == false ) {
			return;
		}

		int eID = -1;
		switch ( InGameManager.Instance.m_QuestMissionID ) {
		case GlobalDefine.INFINITY_DUNGEON_QUEST_001:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_CLEAR_TURN_LV0;
			break;

		case GlobalDefine.INFINITY_DUNGEON_QUEST_002:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_CLEAR_TURN_LV1;
			break;
		case GlobalDefine.INFINITY_DUNGEON_QUEST_003:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_CLEAR_TURN_LV2;
			break;

		case GlobalDefine.INFINITY_DUNGEON_QUEST_004:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_CLEAR_TURN_LV3;
			break;

		case GlobalDefine.INFINITY_DUNGEON_QUEST_005:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_CLEAR_TURN_LV4;
			break;
		default:
			break;
		}

		if ( eID == -1 ) {
			return;
		}

		// 登録
		System.Int64 nVal = (System.Int64)InGameManager.Instance.m_QuestTotalTurn;
		PlayGameServiceUtil.SubmitScore( (ELEADERBORAD)eID, nVal );

		// 登録したらクリア
		InGameManager.Instance.m_QuestTotalTurn = 0;
#endif
    }

    //------------------------------------------------------------------------
    /*!
		@brief		到達階数
	*/
    //------------------------------------------------------------------------
    static public void SubmitTotalFloor()
    {
#if false
		if ( InGameManager.Instance == null ) {
			return;
		}

		int eID = -1;
		switch ( InGameManager.Instance.m_QuestMissionID ) {
		case GlobalDefine.INFINITY_DUNGEON_QUEST_001:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_LV0;
			break;

		case GlobalDefine.INFINITY_DUNGEON_QUEST_002:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_LV1;
			break;
		case GlobalDefine.INFINITY_DUNGEON_QUEST_003:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_LV2;
			break;

		case GlobalDefine.INFINITY_DUNGEON_QUEST_004:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_LV3;
			break;

		case GlobalDefine.INFINITY_DUNGEON_QUEST_005:
			eID = (int)ELEADERBORAD.eINFINITY_DUNGEON_LV4;
			break;
		default:
			break;
		}

		if ( eID == -1 ) {
			return;
		}

		// 登録
		int floor = InGameManager.Instance.m_QuestFloor;
		if ( floor >= 0 ) {
			floor = floor + 1;
		}

		System.Int64 nVal = (System.Int64)floor;
		PlayGameServiceUtil.SubmitScore( (ELEADERBORAD)eID, nVal );
#endif
    }


    //------------------------------------------------------------------------
    //	@brief		クエスト構築情報から敵情報を検索
    //	@param[in]	PacketStructQuestBuild		(quest_build_param)		クエスト情報
    //	@param[in]	int							(unique_id)				敵ユニークID
    //------------------------------------------------------------------------
    public static PacketStructQuestBuildBattle GetQuestBuildBattle(PacketStructQuestBuild quest_build_param, int unique_id)
    {

        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (quest_build_param == null
        || quest_build_param.list_battle == null)
        {
            return null;
        }

        if (unique_id == 0)
        {
            return null;
        }


        PacketStructQuestBuildBattle battle_param = null;
        PacketStructQuestBuildBattle battle_param_temp = null;
        for (int i = 0; i < quest_build_param.list_battle.Length; i++)
        {

            //----------------------------------------
            //	エラーチェック
            //----------------------------------------
            battle_param_temp = quest_build_param.list_battle[i];
            if (battle_param_temp == null)
            {
                continue;
            }


            //----------------------------------------
            //	ユニークID検索
            //----------------------------------------
            if (battle_param_temp.unique_id != unique_id)
            {
                continue;
            }


            battle_param = battle_param_temp;
            break;
        }


        return battle_param;

    }

    //------------------------------------------------------------------------
    //	@brief		クエスト構築情報から敵情報を検索
    //	@param[in]	PacketStructQuestBuild		(quest_build_param)		クエスト情報
    //	@param[in]	int							(unique_id)				敵ユニークID
    //------------------------------------------------------------------------
    public static PacketStructQuest2BuildBattle GetQuest2BuildBattle(PacketStructQuest2Build quest_build_param, int unique_id)
    {

        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (quest_build_param == null
        || quest_build_param.list_battle == null)
        {
            return null;
        }

        if (unique_id == 0)
        {
            return null;
        }


        PacketStructQuest2BuildBattle battle_param = null;
        PacketStructQuest2BuildBattle battle_param_temp = null;
        for (int i = 0; i < quest_build_param.list_battle.Length; i++)
        {

            //----------------------------------------
            //	エラーチェック
            //----------------------------------------
            battle_param_temp = quest_build_param.list_battle[i];
            if (battle_param_temp == null)
            {
                continue;
            }


            //----------------------------------------
            //	ユニークID検索
            //----------------------------------------
            if (battle_param_temp.unique_id != unique_id)
            {
                continue;
            }


            battle_param = battle_param_temp;
            break;
        }


        return battle_param;

    }

    //------------------------------------------------------------------------
    //	@brief		クエスト構築情報からアイテム情報を検索
    //	@param[in]	PacketStructQuestBuild		(quest_build_param)		クエスト情報
    //	@param[in]	int							(unique_id)				アイテムユニークID
    //------------------------------------------------------------------------
    public static PacketStructQuestBuildItem GetQuestBuildItem(PacketStructQuestBuild quest_build_param, int unique_id)
    {

        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (quest_build_param == null
        || quest_build_param.list_item == null)
        {
            return null;
        }

        if (unique_id == 0)
        {
            return null;
        }


        PacketStructQuestBuildItem item_param = null;
        PacketStructQuestBuildItem item_param_temp = null;
        for (int i = 0; i < quest_build_param.list_item.Length; i++)
        {

            //----------------------------------------
            //	エラーチェック
            //----------------------------------------
            item_param_temp = quest_build_param.list_item[i];
            if (item_param_temp == null)
            {
                continue;
            }


            //----------------------------------------
            //	ユニークID検索
            //----------------------------------------
            if (item_param_temp.unique_id != unique_id)
            {
                continue;
            }


            item_param = item_param_temp;
            break;
        }


        return item_param;

    }


    //------------------------------------------------------------------------
    //	@brief		クエスト構築情報からトラップ情報を検索
    //	@param[in]	PacketStructQuestBuild		(quest_build_param)		クエスト情報
    //	@param[in]	int							(unique_id)				トラップユニークID
    //------------------------------------------------------------------------
    public static PacketStructQuestBuildTrap GetQuestBuildTrap(PacketStructQuestBuild quest_build_param, int unique_id)
    {

        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (quest_build_param == null
        || quest_build_param.list_trap == null)
        {
            return null;
        }

        if (unique_id == 0)
        {
            return null;
        }


        PacketStructQuestBuildTrap trap_param = null;
        PacketStructQuestBuildTrap trap_param_temp = null;
        for (int i = 0; i < quest_build_param.list_trap.Length; i++)
        {

            //----------------------------------------
            //	エラーチェック
            //----------------------------------------
            trap_param_temp = quest_build_param.list_trap[i];
            if (trap_param_temp == null)
            {
                continue;
            }


            //----------------------------------------
            //	ユニークID検索
            //----------------------------------------
            if (trap_param_temp.unique_id != unique_id)
            {
                continue;
            }


            trap_param = trap_param_temp;
            break;
        }


        return trap_param;

    }


    //------------------------------------------------------------------------
    /*!
		@brief		LBSのコスト消費共通処理
	*/
    //------------------------------------------------------------------------
    public static void DelLimitBreakSkillCost(CharaParty param_party, GlobalDefine.PartyCharaIndex chara_index)
    {
        uint lbs_skill = 0;


        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (chara_index < GlobalDefine.PartyCharaIndex.LEADER
        || chara_index >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return;
        }

        //		if ( InGameManager.Instance == null ) {
        //			return;
        //		}

        if (param_party == null)
        {
            return;
        }


        //----------------------------------------
        //	キャラデータ取得
        //----------------------------------------
        CharaOnce param_chara_once = param_party.getPartyMember(chara_index, CharaParty.CharaCondition.SKILL_TURN1);
        if (param_chara_once == null)
        {
            return;
        }


        if (!param_chara_once.m_bHasCharaMasterDataParam)
        {
            return;
        }


        //----------------------------------------
        //	LBSパラメータ取得
        //----------------------------------------
        lbs_skill = param_chara_once.m_CharaMasterDataParam.skill_limitbreak;


        MasterDataSkillLimitBreak param_lbs = BattleParam.m_MasterDataCache.useSkillLimitBreak(lbs_skill);
        if (param_lbs == null)
        {
            return;
        }


        //----------------------------------------
        //	コスト消費
        //----------------------------------------
        // SP消費
        param_party.DamageSP(param_lbs.use_sp);

        // ターンコスト消費
        // param_chara_once.DelCharaLimitBreak( param_lbs.use_turn );
        param_chara_once.ClrCharaLimitBreak();

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "SPが" + param_lbs.use_sp.ToString() + "消費されました");
    }

    //---------------------------------------------------------------------
    /*!
		@brief		インゲーム中の速度調整関数
	*/
    //---------------------------------------------------------------------
    static public void SetGameSpeed()
    {

        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();

        if (cOption == null)
        {
            return;
        }

        if (cOption.m_OptionSpeed == (int)LocalSaveDefine.OptionSpeed.ON)
        {
            if (SceneModeContinuousBattle.Instance.m_GameSpeedAmend != GlobalDefine.GAME_SPEED_UP_ON)
            {
                //倍速
                SceneModeContinuousBattle.Instance.m_GameSpeedAmend = GlobalDefine.GAME_SPEED_UP_ON;
            }
        }
        else
        {
            if (SceneModeContinuousBattle.Instance.m_GameSpeedAmend != GlobalDefine.GAME_SPEED_UP_OFF)
            {
                //通常
                SceneModeContinuousBattle.Instance.m_GameSpeedAmend = GlobalDefine.GAME_SPEED_UP_OFF;
            }
        }
    }




    /// <summary>
    /// キャラの攻撃力倍率を取得する
    /// </summary>
    /// <param name="chara"></param>
    /// <param name="ailmentID"></param>
    /// <returns></returns>
    public static float getCharaAttakPowScale(CharaOnce chara, StatusAilmentChara ailmentID)
    {
        float ret_val = 1.0f;

        //------------------------------
        // リーダースキル補正(とりあえずver)
        //------------------------------
        float leaderRate;
        {
            leaderRate = InGameUtilBattle.GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.LEADER, chara);
            leaderRate = InGameUtilBattle.AvoidErrorMultiple(leaderRate, InGameUtilBattle.GetLeaderSkillDamageRate(GlobalDefine.PartyCharaIndex.FRIEND, chara));
            leaderRate = InGameUtilBattle.AvoidErrorMultiple(leaderRate, InGameUtil.GetLeaderSkillDamageUPColor(GlobalDefine.PartyCharaIndex.LEADER));
            leaderRate = InGameUtilBattle.AvoidErrorMultiple(leaderRate, InGameUtil.GetLeaderSkillDamageUPColor(GlobalDefine.PartyCharaIndex.FRIEND));
            leaderRate = InGameUtilBattle.AvoidErrorMultiple(leaderRate, InGameUtil.GetLeaderSkillDamageUPHands(GlobalDefine.PartyCharaIndex.LEADER));
            leaderRate = InGameUtilBattle.AvoidErrorMultiple(leaderRate, InGameUtil.GetLeaderSkillDamageUPHands(GlobalDefine.PartyCharaIndex.FRIEND));
        }
        ret_val = InGameUtilBattle.AvoidErrorMultiple(ret_val, leaderRate);

        //------------------------------
        // パッシブスキル補正(とりあえずver)
        //------------------------------
        float passiveRate;
        {
            GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;  // ダミー（未使用）
            passiveRate = InGameUtilBattle.PassiveChkSkillDamageRate(chara);
            passiveRate = InGameUtilBattle.AvoidErrorMultiple(passiveRate, InGameUtilBattle.PassiveChkKindKillerAtk(chara));
            passiveRate = InGameUtilBattle.AvoidErrorMultiple(passiveRate, InGameUtilBattle.PassiveChkDamageUPFloorPanel(ref owner));
            passiveRate = InGameUtilBattle.AvoidErrorMultiple(passiveRate, InGameUtilBattle.PassiveChkDamageUPElemNum(ref owner));
            passiveRate = InGameUtilBattle.AvoidErrorMultiple(passiveRate, InGameUtilBattle.PassiveChkDamageUPHandsNum(ref owner));
        }
        ret_val = InGameUtilBattle.AvoidErrorMultiple(ret_val, passiveRate);

        //------------------------------
        // リンクパッシブスキル補正
        //------------------------------
        float linkPassiveRate;
        {
            uint linkUnitID = 0;
            if (chara.m_LinkParam != null)
            {
                linkUnitID = chara.m_LinkParam.m_CharaID;
            }
            GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;  // ダミー（未使用）
            linkPassiveRate = InGameUtilBattle.PassiveChkSkillDamageRate(chara, ESKILLTYPE.eLINKPASSIVE);
            linkPassiveRate = InGameUtilBattle.AvoidErrorMultiple(linkPassiveRate, InGameUtilBattle.PassiveChkKindKillerAtk(chara, ESKILLTYPE.eLINKPASSIVE));
            linkPassiveRate = InGameUtilBattle.AvoidErrorMultiple(linkPassiveRate, InGameUtilBattle.PassiveChkDamageUPFloorPanel(ref owner, ESKILLTYPE.eLINKPASSIVE, linkUnitID));
            linkPassiveRate = InGameUtilBattle.AvoidErrorMultiple(linkPassiveRate, InGameUtilBattle.PassiveChkDamageUPElemNum(ref owner, ESKILLTYPE.eLINKPASSIVE, linkUnitID));
            linkPassiveRate = InGameUtilBattle.AvoidErrorMultiple(linkPassiveRate, InGameUtilBattle.PassiveChkDamageUPHandsNum(ref owner, ESKILLTYPE.eLINKPASSIVE, linkUnitID));
        }
        ret_val = InGameUtilBattle.AvoidErrorMultiple(ret_val, linkPassiveRate);

        //------------------------------
        // 状態異常によるダメージ増減
        //------------------------------
        float ailmentRate;
        {
            // 攻撃力倍率
            ailmentRate = ailmentID.GetOffenceRate();
            // 属性攻撃力倍率
            // ※属性エンハンス系、属性逆エンハンス系の計算処理。属性参照はユニット単位で行うこと！
            ailmentRate = InGameUtilBattle.AvoidErrorMultiple(ailmentRate, ailmentID.GetOffenceElementRate(chara.m_CharaMasterDataParam.element));
            // 種族攻撃力倍率(メイン)
            ailmentRate = InGameUtilBattle.AvoidErrorMultiple(ailmentRate, ailmentID.GetOffenceKindRate(chara.kind));
            // 種族攻撃力倍率(サブ)
            ailmentRate = InGameUtilBattle.AvoidErrorMultiple(ailmentRate, ailmentID.GetOffenceKindRate(chara.kind_sub));
        }
        ret_val = InGameUtilBattle.AvoidErrorMultiple(ret_val, ailmentRate);

        return ret_val;
    }

    public static void setQuest2(bool sw)
    {
        m_Quest2 = sw;
    }

    public static bool isQuest2()
    {
        return m_Quest2;
    }

}; // class InGameUtil

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
