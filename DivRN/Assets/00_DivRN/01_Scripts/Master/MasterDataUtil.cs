/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MasterDataUtil.cs
	@brief	マスターデータ関連ユーティリティ
	@author Developer
	@date 	2012/10/03
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
using System.Linq;
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
	@brief	マスターデータ関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class MasterDataUtil
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    //----------------------------------------------------------------------------
    /*!
		@brief	プレゼントマスター取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataPresent GetPresentParamFromID(uint unPresentID)
    {
        MasterDataPresent resMasterDataPresent = MasterFinder<MasterDataPresent>.Instance.Find((int)unPresentID);
        if (resMasterDataPresent == null)
        {
            return null;
        }

        return resMasterDataPresent;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		指定したガチャが有効か判定する
		@note		チュートリアルガチャ・サブガチャ・イベント判定・ランチタイムガチャの時間制限
					に引っかかったものはfalseを返す
	*/
    //----------------------------------------------------------------------------
    static public bool CheckActiveGachaMaster(MasterDataGacha gachaMaster)
    {
        //--------------------------------
        // チュートリガチャは普通には選ばれない。
        // ※チュートリは例外的に別関数で直接マスターデータを参照して取得する
        //--------------------------------
        if (gachaMaster.fix_id == GlobalDefine.TUTORIAL_SCRATCH)
        {
            return false;
        }

        //--------------------------------
        // サブガチャ、チケットガチャは選ばれない
        //--------------------------------
        if (gachaMaster.type == MasterDataDefineLabel.GachaType.SUB ||
            gachaMaster.type == MasterDataDefineLabel.GachaType.TICKET)
        {
            return false;
        }

        //--------------------------------
        // イベントタイプが設定されているなら
        // イベントの有効チェック
        //--------------------------------
        if (gachaMaster.event_id != 0)
        {
            if (TimeEventManager.Instance.ChkEventActive(gachaMaster.event_id) == false)
            {
                return false;
            }
        }
        else
        {
            //--------------------------------
            // イベント期間判定
            // @add Developer 2016/07/25 ver360
            // イベントIDが指定されてない場合(値が0の場合)、
            // ガチャ定義で指定されている開始と終了タイミングを見るように設定
            //--------------------------------
            bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(
                                                                gachaMaster.timing_start
                                                            , gachaMaster.timing_end
                                                            );
            if (bCheckWithinTime == false)
            {
                return false;
            }
        }

        //--------------------------------
        // ステップアップガチャ
        //--------------------------------
        if (gachaMaster.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            MasterDataStepUpGacha stepUpGachaMaster = MasterDataUtil.GetMasterDataStepUpGachaFromGachaID(gachaMaster.fix_id);
            if (stepUpGachaMaster == null)
            {
                return false;
            }

            // 現在のステップがあるかどうか
            MasterDataStepUpGachaManage stepUpGachaManageMaster = MasterDataUtil.GetCurrentStepUpGachaManageMaster(stepUpGachaMaster.gacha_id);
            if (stepUpGachaManageMaster == null)
            {
                return false;
            }

            return true;
        }

        //--------------------------------
        // ガチャグループマスターが存在しない
        //--------------------------------
        MasterDataGachaGroup gachaGroup = MasterFinder<MasterDataGachaGroup>.Instance.Find((int)gachaMaster.gacha_group_id);
        if (gachaGroup == null)
        {
            return false;
        }

        //--------------------------------
        // ランチタイムガチャの時間制限
        //--------------------------------
        if (gachaMaster.type == MasterDataDefineLabel.GachaType.LUNCH)
        {
            if (MainMenuUtil.CheckLunchTimeGacha() == false)
            {
                return false;
            }
        }

        //--------------------------------
        // 有料チップガチャが有効か
        //--------------------------------
        if (gachaMaster.paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
        {
            // 保持データがある場合は、ガチャ引き情報を持ってくる
            PacketStructGachaStatus GachaStatus = UserDataAdmin.Instance.GetGachaStatus(gachaMaster.fix_id);

            if (GachaStatus != null)
            {
                //--------------------------------
                // ガチャカウントチェック
                //--------------------------------
                if (MainMenuUtil.CheckPaidTipOnlyGacha(gachaMaster) == false)
                {
                    if (MasterDataUtil.GetGachaCountPossible(GachaStatus.fix_id, GachaStatus.paid_tip_only_count) == false)
                    {
                        return false;
                    }
                }
            }
        }

        //--------------------------------
        // アイテムポイントガチャチェック
        //--------------------------------
        if (gachaMaster.type == MasterDataDefineLabel.GachaType.ITEM_POINT)
        {
            MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find((int)gachaMaster.cost_item_id);
            if (itemMaster == null)
            {
                return false;
            }

            MasterDataEvent eventMaster = GetMasterDataEventFromID(itemMaster.gacha_event_id);
            if (eventMaster == null)
            {
                return false;
            }

            if (eventMaster.timing_end != 0)
            {
                //有限イベントは期間チェック
                if (TimeEventManager.Instance.ChkEventActive(eventMaster.event_id) == false)
                {
                    return false;
                }
            }
            else
            {
                //無限イベントはポイント所持とプライオリティチェック
                if (UserDataAdmin.Instance.GetItemPoint(gachaMaster.cost_item_id) == 0 &&
                    gachaMaster.priority == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 確定枠のガチャかどうか
    /// </summary>
    /// <param name="fix_id"></param>
    /// <returns></returns>
    public static bool CheckRainbowGacha(uint fix_id)
    {
        MasterDataGacha master = MasterFinder<MasterDataGacha>.Instance.SelectFirstWhere("where rainbow_decide = ?", fix_id);
        if (master != null)
        {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		有効なガチャ情報を取得
		@note		選択したタブの優先度の高い方から降順に並べたデータを取得する
	*/
    //----------------------------------------------------------------------------
    static public MasterDataGacha[] GetActiveGachaMaster()
    {
        if (TutorialManager.IsExists == true)
        {
            Debug.LogError("GetActiveGachaMaster:Tutorial in progress");
            return null;
        }

        TemplateList<MasterDataGacha> acGachaMasterData = new TemplateList<MasterDataGacha>();
        //--------------------------------
        // 有効になるガチャを選定
        //--------------------------------
        MasterDataGacha[] gachas = MasterFinder<MasterDataGacha>.Instance.GetAll();
        for (int i = 0; i < gachas.Length; i++)
        {
            MasterDataGacha cGachaMasterData = gachas[i];
            if (cGachaMasterData == null)
            {
                continue;
            }

            //fix_idが100のガチャは非表示
            if (cGachaMasterData.fix_id == GlobalDefine.TUTORIAL_SCRATCH)
            {
                continue;
            }

            //確定枠ガチャは無視
            if (CheckRainbowGacha(cGachaMasterData.fix_id))
            {
                continue;
            }

            //--------------------------------
            // ガチャ有効チェック
            //--------------------------------

            if (!CheckActiveGachaMaster(cGachaMasterData))
            {
                continue;
            }

            //--------------------------------
            // ガチャ有効！
            //--------------------------------
            acGachaMasterData.Add(cGachaMasterData);
        }

        //--------------------------------
        // ソート処理 (表示優先度が高い物を選択)
        //--------------------------------
        acGachaMasterData.Sort(GachaMasterSortExec);

        return acGachaMasterData.ToArray();
    }

    /// <summary>
    /// フレンドポイントガチャ取得
    /// </summary>
    /// <returns></returns>
    static public MasterDataGacha GetActiveFriendGachaMaster()
    {
        MasterDataGacha[] _activeList = GetActiveGachaMaster();

        if (_activeList == null)
        {
            return null;
        }

        for (int i = 0; i < _activeList.Length; i++)
        {
            if (_activeList[i].type == MasterDataDefineLabel.GachaType.NORMAL) return _activeList[i];
        }

        return null;
    }

    /// <summary>
    /// アイテムポイントガチャ取得
    /// </summary>
    /// <param name="event_id"></param>
    /// <returns></returns>
    static public MasterDataGacha GetActiveItemPointGachaMaster(uint item_id)
    {
        MasterDataGacha[] _activeList = GetActiveGachaMaster();
        if (_activeList == null)
        {
            return null;
        }

        for (int i = 0; i < _activeList.Length; i++)
        {
            if (_activeList[i].type == MasterDataDefineLabel.GachaType.ITEM_POINT &&
                _activeList[i].cost_item_id == item_id)
            {
                return _activeList[i];
            }
        }

        return null;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="_group_id"></param>
    /// <param name="_pass_id"></param>
    /// <returns></returns>
    static public MasterDataGacha GetGroupGachaMaster(MasterDataGacha _master)
    {
        //無効
        if (_master.gacha_group_id == 0)
        {
            return null;
        }

        MasterDataGacha[] _gachaList = MasterFinder<MasterDataGacha>.Instance.SelectWhere("where gacha_group_id = ?", _master.gacha_group_id).ToArray();
        if (_gachaList == null)
        {
            return null;
        }

        for (int i = 0; i < _gachaList.Length; i++)
        {
            //同じガチャ
            if (_gachaList[i].fix_id == _master.fix_id)
            {
                continue;
            }

            //確定枠
            if (_master.rainbow_decide == _gachaList[i].fix_id)
            {
                continue;
            }

            return _gachaList[i];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		マスターデータからガチャの引ける回数を取得
    */
    //----------------------------------------------------------------------------
    static public uint GetGachaCountFromMaster(MasterDataGacha cMasterGacha, uint max = GlobalDefine.GACHA_PLAY_MAX, bool free = false)
    {
        uint unRetVal = 0;

        uint price = 0;
        MasterDataDefineLabel.BoolType paid_tip_only = MasterDataDefineLabel.BoolType.NONE;
        MasterDataStepUpGachaManage stepManageMaster = null;

        if (cMasterGacha.type != MasterDataDefineLabel.GachaType.STEP_UP)
        {
            price = cMasterGacha.price;
            paid_tip_only = cMasterGacha.paid_tip_only;
        }
        else
        {
            MasterDataStepUpGacha stepMaster = MasterDataUtil.GetMasterDataStepUpGachaFromGachaID(cMasterGacha.fix_id);
            stepManageMaster = MasterDataUtil.GetCurrentStepUpGachaManageMaster(stepMaster.gacha_id);
            price = stepManageMaster.price;
            paid_tip_only = stepMaster.paid_tip_only;
        }

        //価格が０の時は最大数を返す
        if (price <= 0)
        {
            return max;
        }

        //--------------------------------
        // ガチャタイプにあわせて引ける回数を計算
        //--------------------------------
        switch (cMasterGacha.type)
        {
            // 友情ガチャ
            case MasterDataDefineLabel.GachaType.NORMAL:
                {
                    unRetVal = UserDataAdmin.Instance.m_StructPlayer.have_friend_pt / price;
                }
                break;
            // レア
            case MasterDataDefineLabel.GachaType.RARE:
            // イベント(コラボ)
            case MasterDataDefineLabel.GachaType.EVENT:
            // チュートリアル
            case MasterDataDefineLabel.GachaType.TUTORIAL:
            // イベント2
            case MasterDataDefineLabel.GachaType.EVENT_2:
                {
                    if (free == false)
                    {
                        //初回無料チェック
                        if (IsFirstTimeFree(cMasterGacha))
                        {
                            //初回無料は１回固定
                            unRetVal = 1;
                        }
                        else
                        {
                            unRetVal = UserDataAdmin.Instance.m_StructPlayer.have_stone / price;
                            // 有料チップ限定フラグが立っていたら引ける回数は最大1になる
                            if (paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                            {
                                unRetVal = UserDataAdmin.Instance.m_StructPlayer.have_stone_pay / price;
                                if (unRetVal > 1)
                                {
                                    unRetVal = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        unRetVal = 0;
                    }
                }
                break;
            // ランチ
            case MasterDataDefineLabel.GachaType.LUNCH:
                {
                    unRetVal = 1;
                }
                break;
            case MasterDataDefineLabel.GachaType.EVENT_POINT:
                {
                    unRetVal = 1;
                }
                break;
            case MasterDataDefineLabel.GachaType.ITEM_POINT:
                {
                    uint _ticketNum = UserDataAdmin.Instance.GetItemPoint(cMasterGacha.cost_item_id);
                    unRetVal = _ticketNum / price;
                }
                break;
            case MasterDataDefineLabel.GachaType.STEP_UP:
                {
                    if (free == false)
                    {

                        unRetVal = UserDataAdmin.Instance.m_StructPlayer.have_stone / price;
                        // 有料チップ限定フラグの場合
                        if (paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                        {
                            unRetVal = UserDataAdmin.Instance.m_StructPlayer.have_stone_pay / price;
                        }

                        // 引ける場合は指定回数にする
                        if (unRetVal != 0)
                        {
                            unRetVal = stepManageMaster.total_lot_exec;
                        }
                    }
                    else
                    {
                        unRetVal = 0;
                    }
                }
                break;
        }

        if (unRetVal > max)
        {
            unRetVal = max;
        }

        return unRetVal;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		マスターデータから初回無料ひけるかチェック
    */
    //----------------------------------------------------------------------------
    static public bool IsFirstTimeFree(MasterDataGacha master)
    {
        //チップを使用するガチャ以外は無効
        if (master.type != MasterDataDefineLabel.GachaType.RARE &&
            master.type != MasterDataDefineLabel.GachaType.EVENT)
        {
            return false;
        }

        //初回無料フラグ
        if (master.first_time_free_enable == MasterDataDefineLabel.BoolType.DISABLE)
        {
            return false;
        }

        //ガチャステータスチェック
        PacketStructGachaStatus status = UserDataAdmin.Instance.GetGachaStatus(master.fix_id);
        if (status != null)
        {
            if (status.is_first_time != 0)
            {
                return false;
            }
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		初回無料ガチャが存在するかチェック
    */
    //----------------------------------------------------------------------------
    static public bool CheckFirstTimeFree()
    {
        MasterDataGacha[] activeGacha = GetActiveGachaMaster();
        if (activeGacha == null ||
            activeGacha.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < activeGacha.Length; i++)
        {
            if (IsFirstTimeFree(activeGacha[i]))
            {
                return true;
            }
        }

        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ソート処理：ガチャマスターデータ
		@note	priorityで降順になるように設定
	*/
    //----------------------------------------------------------------------------
    static public int GachaMasterSortExec(MasterDataGacha cGachaA, MasterDataGacha cGachaB)
    {
        return cGachaB.priority - cGachaA.priority;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ガチャ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataGacha GetGachaParamFromID(uint unGachaID)
    {
        if (MasterFinder<MasterDataGacha>.Instance == null)
        {
            return null;
        }

        MasterDataGacha[] gachas = MasterFinder<MasterDataGacha>.Instance.GetAll();

        for (int i = 0; i < gachas.Length; i++)
        {
            if (gachas[i].fix_id != unGachaID)
            {
                continue;
            }

            return gachas[i];
        }

        return null;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ガチャ取得：ガチャが引けるか
	*/
    //----------------------------------------------------------------------------
    static public bool GetGachaCountPossible(uint unGachaID, int nCount)
    {
        MasterDataGacha gachaMaster = MasterFinder<MasterDataGacha>.Instance.Find((int)unGachaID);
        if (gachaMaster == null)
        {
            return false;
        }

        if (gachaMaster.view_count > nCount)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 現在のスッテップ数のMasterDataStepUpGachaManageを取得する
    /// </summary>
    /// <param name="gacha_id"></param>
    /// <returns></returns>
    static public MasterDataStepUpGachaManage GetCurrentStepUpGachaManageMaster(uint gacha_id)
    {
        List<MasterDataStepUpGachaManage> gachaManageMasterList = MasterFinder<MasterDataStepUpGachaManage>.Instance.SelectWhere("where gacha_id = ? ORDER BY step_num ASC", gacha_id);
        if (gachaManageMasterList == null)
        {
            return null;
        }

        MasterDataStepUpGachaManage gachaManageMaster = null;

        // 現在のステップを取得
        PacketStructGachaStatus gachaStatus = UserDataAdmin.Instance.GetGachaStatus(gacha_id);
        if (gachaStatus != null)
        {
            gachaManageMaster = gachaManageMasterList.Find((v) => v.step_num == gachaStatus.step_num);
        }
        else
        {
            // 初回ステップ
            gachaManageMaster = gachaManageMasterList.First();
        }

        if (gachaManageMaster == null)
        {
            Debug.LogError("No MasterDataStepUpGachaManage::gacha_id: " + gacha_id);
        }

        return gachaManageMaster;
    }

    static public MasterDataStepUpGacha GetMasterDataStepUpGachaFromGachaID(uint gacha_id)
    {
        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataStepUpGacha>.Instance.SelectFirstWhere("where gacha_id = ?", gacha_id);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataParamChara GetCharaParamFromID(uint unID)
    {
        if (MasterFinder<MasterDataParamChara>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataParamChara>.Instance.Find(unID.ToString());
    }

    /// <summary>
    /// キャラパラメータ取得：表示ID
    /// </summary>
    /// <param name="drawID"></param>
    /// <returns></returns>
    static public MasterDataParamChara GetCharaParamFromDrawID(uint drawID)
    {
        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (drawID == 0)
        {
            return null;
        }
        MasterDataParamChara MasterDataParamCharaTmp = MasterFinder<MasterDataParamChara>.Instance.SelectFirstWhere(" where draw_id = ? ", drawID);
        return MasterDataParamCharaTmp;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラ進化情報取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataParamCharaEvol GetCharaEvolParamFromCharaID(uint unID)
    {
        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        List<MasterDataParamCharaEvol> evolArray = MasterFinder<MasterDataParamCharaEvol>.Instance.FindAll();
        MasterDataParamCharaEvol evol = evolArray.Find((v) => v.unit_id_pre == unID);

        return evol;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キャラ進化前情報をリストで取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataParamCharaEvol[] GetCharaEvolParamFromCharaListID(uint unID, bool bAfter)
    {
        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        MasterDataParamCharaEvol[] retMasterDataParamCharaEvol = null;
        if (bAfter)
        {
            retMasterDataParamCharaEvol = MasterFinder<MasterDataParamCharaEvol>.Instance.SelectWhere("where unit_id_pre = ? ", unID).ToArray();
        }
        else
        {
            retMasterDataParamCharaEvol = MasterFinder<MasterDataParamCharaEvol>.Instance.SelectWhere("where unit_id_after = ? ", unID).ToArray();
        }

        if (retMasterDataParamCharaEvol == null)
        {
            return null;
        }

        return retMasterDataParamCharaEvol;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	パッシブスキルパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataSkillPassive GetPassiveSkillParamFromID(uint unID)
    {
        if (MasterFinder<MasterDataSkillPassive>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataSkillPassive>.Instance.Find(unID.ToString());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リミットブレイクスキルパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataSkillLimitBreak GetLimitBreakSkillParamFromID(uint unID)
    {
        if (MasterFinder<MasterDataSkillLimitBreak>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataSkillLimitBreak>.Instance.Find(unID.ToString());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リーダースキルパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataSkillLeader GetLeaderSkillParamFromID(uint unID)
    {
        //InGameUtilで使用しているが
        //if falseなので使ってはいなそう
        if (MasterFinder<MasterDataSkillLeader>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataSkillLeader>.Instance.Find(unID.ToString());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	スキルパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataSkillActive GetActiveSkillParamFromID(uint unID)
    {
        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        return MasterFinder<MasterDataSkillActive>.Instance.SelectFirstWhere("where fix_id = ? ", unID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	エリアパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataArea GetAreaParamFromID(uint unID)
    {
        if (MasterFinder<MasterDataArea>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataArea>.Instance.Find(unID.ToString());
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	エリア補正パラメータ取得：エリアID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataAreaAmend GetAreaAmendParamFromAreaID(uint unAreaID, MasterDataDefineLabel.AmendType nAmendType)
    {
        //------------------
        //
        //------------------
        MasterDataDefineLabel.BelongType nUserGroup = SystemUtil.GetUserGroup();
        MasterDataAreaAmend[] areaAmendArray = MasterFinder<MasterDataAreaAmend>.Instance.SelectWhere("where area_id = ?", unAreaID).ToArray();
        int uAreaAmendListMax = areaAmendArray.Length - 1;
        for (int nChkAreaAmend = uAreaAmendListMax; nChkAreaAmend >= 0; nChkAreaAmend--)
        {
            //------------------
            //
            //------------------
            if (areaAmendArray[nChkAreaAmend] == null)
            {
                continue;
            }

            if (areaAmendArray[nChkAreaAmend].active != MasterDataDefineLabel.BoolType.ENABLE)
            {
                continue;
            }

            //------------------
            // ユーザーグループの指定があるなら自分のグループと一致判定
            //------------------
            if (areaAmendArray[nChkAreaAmend].user_group != MasterDataDefineLabel.BelongType.NONE
            && areaAmendArray[nChkAreaAmend].user_group != nUserGroup
            ) continue;

            //------------------
            // エリア補正効果が求めているタイプでないならスルー
            //------------------
            if (nAmendType != MasterDataDefineLabel.AmendType.NONE)
            {
                if (areaAmendArray[nChkAreaAmend].amend != nAmendType)
                {
                    continue;
                }
            }

            //------------------
            // 効果時間内チェック
            //------------------
            bool bWithinTime = TimeManager.Instance.CheckWithinTime(areaAmendArray[nChkAreaAmend].timing_start
                                                                , areaAmendArray[nChkAreaAmend].timing_end
                                                                );

            // @add Developer 2016/08/12 v360 イベント終了時間が0だった場合表示しない(セーフティ処理)
            if (areaAmendArray[nChkAreaAmend].timing_end <= 0)
            {
                continue;
            }

            if (bWithinTime == false)
            {
                continue;
            }

            //------------------
            // 補正データ確定
            //------------------
            return areaAmendArray[nChkAreaAmend];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：クエストコンティニューが可能かどうか
	*/
    //----------------------------------------------------------------------------
    static public bool GetQuestContiunueEnable(uint unID)
    {
        bool ret = true;

        MasterDataQuest2 quest_param = GetQuest2ParamFromID(unID);
        if (quest_param == null)
        {
            return ret;
        }

        //	設定値DISABLE意外は可能とする
        if (quest_param.enable_continue == MasterDataDefineLabel.BoolType.DISABLE)
        {
            ret = false;
        }

        return ret;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：ゲリラボス情報
	*/
    //----------------------------------------------------------------------------
    static public MasterDataGuerrillaBoss GetGuerrillaBossParamFromQuestID(uint unQuestID)
    {
        //----------------------------------------
        // 対象クエストにとって有効なゲリラボスを選定
        //----------------------------------------
        MasterDataDefineLabel.BelongType nUserGroup = SystemUtil.GetUserGroup();
        MasterDataGuerrillaBoss[] bossArray = MasterFinder<MasterDataGuerrillaBoss>.Instance.SelectWhere("where quest_id = ?", unQuestID).ToArray();
        for (int i = 0; i < bossArray.Length; ++i)
        {
            //----------------------------------------
            // 有効チェック
            //----------------------------------------
            MasterDataGuerrillaBoss cGuerrillaBossChk = bossArray[i];
            if (cGuerrillaBossChk == null)
            {
                continue;
            }

            if (cGuerrillaBossChk.active != MasterDataDefineLabel.BoolType.ENABLE)
            {
                continue;
            }

            if (cGuerrillaBossChk.user_group != MasterDataDefineLabel.BelongType.NONE
            && cGuerrillaBossChk.user_group != nUserGroup
            ) continue;

            //----------------------------------------
            // 期間内チェック
            //----------------------------------------
            if (TimeManager.Instance.CheckWithinTime(cGuerrillaBossChk.timing_start, cGuerrillaBossChk.timing_end) == false)
            {
                continue;
            }

            //----------------------------------------
            // 特定クエストクリアが出現条件として指定されている場合
            // 出現判定を行い間引く
            //----------------------------------------
            if (cGuerrillaBossChk.quest_id_must != 0)
            {
                if (ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_clear, cGuerrillaBossChk.quest_id_must) == false)
                {
                    continue;
                }
            }

            //----------------------------------------
            // 選んだクエストを対象とした期間内のゲリラクエストを発見
            //----------------------------------------
            return cGuerrillaBossChk;
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	敵特性パラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataEnemyAbility GetEnemyAbilityParamFromID(int nAbilityID)
    {
        return MasterFinder<MasterDataEnemyAbility>.Instance.Find(nAbilityID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：状態異常整理用定義
	*/
    //----------------------------------------------------------------------------
    static public MasterDataStatusAilmentParam GetMasterDataStatusAilmentParam(int unStatusAilmentID)
    {
        MasterDataStatusAilmentParam retMasterDataStatusAilmentParam = MasterFinder<MasterDataStatusAilmentParam>.Instance.SelectFirstWhere("where fix_id = ? ", unStatusAilmentID);
        if (retMasterDataStatusAilmentParam == null)
        {
            return null;
        }

        return retMasterDataStatusAilmentParam;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：エリア中のクエスト通し番号
	*/
    //----------------------------------------------------------------------------
    static public bool GetQuestSerialNumber(uint unQuestID, uint unAreaID, ref uint unGetSerialNum, ref uint unGetSerialTotal)
    {

        unGetSerialNum = 0;
        unGetSerialTotal = 0;

        SimpleSQL.SimpleDataTable quest_count = MasterFinder<MasterDataQuest2>.Instance.SelectCountWhere(" where area_id = ? and fix_id < ? ", unAreaID, unQuestID);

        if (quest_count != null)
        {
            unGetSerialNum = Convert.ToUInt32(quest_count.rows[0][0]);
        }

        SimpleSQL.SimpleDataTable quest_total_count = MasterFinder<MasterDataQuest2>.Instance.SelectCountWhere(" where area_id = ?  ", unAreaID);

        if (quest_total_count != null)
        {
            unGetSerialTotal = Convert.ToUInt32(quest_total_count.rows[0][0]);
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：運営のお知らせ文言を取得
	*/
    //----------------------------------------------------------------------------
    static public string GetInformationMessage(MasterDataDefineLabel.InfomationType _type, bool bCheckIgnoer = true)
    {
        string ignoreKeyName = "";
        TemplateList<uint> ignoreList = new TemplateList<uint>();
        if (bCheckIgnoer)
        {
            //日付が変更されるまで保持される無視リストを取得
            LocalSaveManager.Instance.CheckInfoTime(TimeManager.Instance.m_TimeNow);
            switch (_type)
            {
                case MasterDataDefineLabel.InfomationType.NONE:
                    return "";
                case MasterDataDefineLabel.InfomationType.NORMAL:
                    ignoreKeyName = LocalSaveManager.LOCALSAVE_INFO_NORMAL_LIST;
                    break;
                case MasterDataDefineLabel.InfomationType.EMERGENCY:
                    ignoreKeyName = LocalSaveManager.LOCALSAVE_INFO_EMERGENCY_LIST;
                    break;
            }
            ignoreList = LocalSaveManager.Instance.LoadFuncUintList(ignoreKeyName);
        }

        //----------------------------------------
        // 現在時で有効な運営通知を選定
        //----------------------------------------
        MasterDataInformation[] infoList = MasterFinder<MasterDataInformation>.Instance.GetAll();

        string strInformation = "";
        List<MasterDataInformation> tmpInfoList = new List<MasterDataInformation>();
        for (int i = infoList.Length - 1; i >= 0; i--)
        {
            MasterDataInformation cInformation = infoList[i];
            //有効フラグ
            if (cInformation.active != MasterDataDefineLabel.BoolType.ENABLE)
            {
                continue;
            }

            //タイプチェック
            if (cInformation.type != _type)
            {
                continue;
            }

#if UNITY_ANDROID
            if (cInformation.platform != 1 && cInformation.platform != 3)
            {
                continue;
            }
#elif UNITY_IPHONE
            if (cInformation.platform != 0 && cInformation.platform != 3)
            {
                continue;
            }
#elif UNITY_STANDALONE_WIN
            if (cInformation.platform != 2 && cInformation.platform != 3)
			{
				continue;
			}
#endif
            bool bIgnore = false;
            for (int j = 0; j < ignoreList.m_BufferSize; j++)
            {
                if (ignoreList[j] == cInformation.fix_id)
                {
                    bIgnore = true;
                    break;
                }
            }

            if (bIgnore)
            {
                continue;
            }

            //
            bool bTimeCheck = false;
            if (cInformation.timing_start == 0 &&
               cInformation.timing_end == 0)
            {
                bTimeCheck = true;
            }
            else if (cInformation.timing_start == 0)
            {
                //----------------------------------------
                // 期間内チェック
                // 終了期限のみの指定時用
                //----------------------------------------
                bTimeCheck = TimeManager.Instance.CheckWithinTime(cInformation.timing_end);
            }
            else
            {
                //----------------------------------------
                // 期間内チェック
                //----------------------------------------
                bTimeCheck = TimeManager.Instance.CheckWithinTime(cInformation.timing_start, cInformation.timing_end);
            }

            if (!bTimeCheck)
            {
                continue;
            }

            //----------------------------------------
            // 運営通知を有効なものと見なして追加登録
            //----------------------------------------
            tmpInfoList.Add(cInformation);
        }

        //ソート
        tmpInfoList.Sort((a, b) => a.priority - b.priority);

        for (int i = 0; i < tmpInfoList.Count; i++)
        {
            if (strInformation == string.Empty)
            {
                strInformation += tmpInfoList[i].message;
            }
            else
            {
                strInformation += ("\n" + tmpInfoList[i].message);
            }

            //一度表示したら無視リストにいれて日付が変更されるまで表示しない
            ignoreList.Add(tmpInfoList[i].fix_id);
        }

        //無視リスト更新
        if (bCheckIgnoer && tmpInfoList.Count != 0)
        {
            LocalSaveManager.Instance.SaveFuncUintList(ignoreKeyName, ignoreList);
        }

        return strInformation;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：AssetBundleデータ
	*/
    //----------------------------------------------------------------------------
    static private Dictionary<string, MasterDataAssetBundlePath> AssetBundlePaths = null;

    static public void ResetAssetBundlePaths()
    {
        if (AssetBundlePaths != null)
        {
            AssetBundlePaths.Clear();
            AssetBundlePaths = null;
        }
    }

    static public MasterDataAssetBundlePath GetMasterDataAssetBundlePath(string strAssetBundleName, bool autoLower = true)
    {
        //テキスト定義マスタを参照する
        if (MasterFinder<MasterDataAssetBundlePath>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 対象外
        //----------------------------------------
        if (strAssetBundleName == null ||
            strAssetBundleName.Length <= 0)
        {
            return null;
        }

        string filename = strAssetBundleName;
        if (autoLower == true)
        {
            filename = strAssetBundleName.ToLower();
        }


        //----------------------------------------
        // キー値の一致するテキストを取得
        //----------------------------------------
        MasterDataAssetBundlePath assetbundlepath = null;
        bool iscache = MasterFinder<MasterDataAssetBundlePath>.Instance.IsCache();

        List<MasterDataAssetBundlePath> list = MasterFinder<MasterDataAssetBundlePath>.Instance.FindAll();
        if (iscache == false)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].LowerName = list[i].name.ToLower();
            }
        }

        if (AssetBundlePaths == null)
        {
            AssetBundlePaths = new Dictionary<string, MasterDataAssetBundlePath>();

            for (int i = 0; i < list.Count; i++)
            {
                MasterDataAssetBundlePath master = list[i];
                AssetBundlePaths[list[i].LowerName] = master;
            }
        }

        return AssetBundlePaths.ContainsKey(filename) ? AssetBundlePaths[filename] : null;
    }

    static public string GetMasterDataAreamapBackgroundName(int background)
    {
        string assetbundleName = "Areamap_" + background;

        if (GetMasterDataAssetBundlePath(assetbundleName) == null)
        {
            assetbundleName = "Areamap_default";
        }

        return assetbundleName;
    }

    static public string GetMasterDataQuestBackgroundName(int background)
    {
        string assetbundleName = "Quest_" + background.ToString("0000");
        // アセットマスターに存在するか
        if (GetMasterDataAssetBundlePath(assetbundleName) == null)
        {
#if BUILD_TYPE_DEBUG
            Debug.LogError("QUEST MasterDataAssetBundlePath BackGround Name Not Found [" + assetbundleName + "]");
#endif
            assetbundleName = "Quest_default";
        }

        return assetbundleName;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：有効なイベント取得
	*/
    //----------------------------------------------------------------------------
    static public MasterDataEvent GetMasterDataEventFromID(uint unEventID)
    {
        if (unEventID == 0)
        {
            return null;
        }
        //----------------------------------------
        //
        //----------------------------------------
        MasterDataDefineLabel.BelongType nUserGroup = SystemUtil.GetUserGroup();
        //MasterDataEvent[] eventArray = MasterFinder<MasterDataEvent>.Instance.GetAll();
        object[] whereArg = new object[4];
        String whereText = " where active = ? ";
        whereArg[0] = MasterDataDefineLabel.BoolType.ENABLE;
        whereText += " and (user_group = ? OR user_group = ? )";
        whereArg[1] = MasterDataDefineLabel.BelongType.NONE;
        whereArg[2] = nUserGroup;
        whereText += " and ( event_id = ? )";
        whereArg[3] = unEventID;

        List<MasterDataEvent> eventList = MasterFinder<MasterDataEvent>.Instance.SelectWhere(whereText, whereArg);
        if (eventList == null)
        {
            return null;
        }

        MasterDataEvent[] eventArray = eventList.ToArray();
        for (int nChk = 0; nChk < eventArray.Length; ++nChk)
        {
            MasterDataEvent cEvent = eventArray[nChk];

            //--------------------------------
            // @change Developer 2016/07/29 無期限対応
            //--------------------------------
            if (cEvent.timing_end > 0)
            {
                uint unTimingStart = 0;
                uint unTimingEnd = 0;

                //--------------------------------
                // 期間指定タイプによる分岐
                // @add Developer 2016/07/26 v360
                //--------------------------------
                switch (cEvent.period_type)
                {
                    // 指定(従来通り)
                    default:
                    case MasterDataDefineLabel.PeriodType.DESIGNATION:
                        unTimingStart = cEvent.timing_start;
                        unTimingEnd = cEvent.timing_end;
                        break;

                    // サイクル
                    case MasterDataDefineLabel.PeriodType.CYCLE:
                        if (TimeEventManager.Instance == null)
                        {
                            continue;
                        }

                        // エリアの表示期間のカウントダウンを算出
                        CycleParam cCycleParam = TimeEventManager.Instance.GetEventCycleParam(cEvent.event_id);
                        if (cCycleParam == null)
                        {
                            continue;
                        }

                        unTimingStart = cCycleParam.timingStart;
                        unTimingEnd = cCycleParam.timingEnd;
                        break;
                }

                bool bCheckWithinTime = TimeManager.Instance.CheckWithinTime(unTimingStart, unTimingEnd);
                if (bCheckWithinTime == false)
                {
                    continue;
                }
            }

            return cEvent;
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：未来日のイベント取得
	*/
    //----------------------------------------------------------------------------
    static public MasterDataEvent GetMasterDataFurtureEventFromID(uint unEventID)
    {
        MasterDataEvent cResultEvent = null;

        if (unEventID == 0)
        {
            return (cResultEvent);
        }

        //----------------------------------------
        //
        //----------------------------------------
        MasterDataDefineLabel.BelongType nUserGroup = SystemUtil.GetUserGroup();
        //MasterDataEvent[] eventArray = MasterFinder<MasterDataEvent>.Instance.GetAll();
        object[] whereArg = new object[4];
        String whereText = " where active = ? ";
        whereArg[0] = MasterDataDefineLabel.BoolType.ENABLE;
        whereText += " and (user_group = ? OR user_group = ? )";
        whereArg[1] = MasterDataDefineLabel.BelongType.NONE;
        whereArg[2] = nUserGroup;
        whereText += " and ( event_id = ? )";
        whereArg[3] = unEventID;

        List<MasterDataEvent> eventList = MasterFinder<MasterDataEvent>.Instance.SelectWhere(whereText, whereArg);
        if (eventList == null)
        {
            return null;
        }

        MasterDataEvent[] eventArray = eventList.ToArray();
        for (int nChk = 0; nChk < eventArray.Length; ++nChk)
        {
            MasterDataEvent cEvent = eventArray[nChk];

            //--------------------------------
            // 期間指定タイプによる分岐
            // @add Developer 2016/07/26 v360
            //--------------------------------
            uint unTimingStart = 0;
            uint unTimingEnd = 0;
            switch (cEvent.period_type)
            {
                // 指定(従来通り)
                default:
                case MasterDataDefineLabel.PeriodType.DESIGNATION:
                    unTimingStart = cEvent.timing_start;
                    unTimingEnd = cEvent.timing_end;
                    break;

                // サイクル
                case MasterDataDefineLabel.PeriodType.CYCLE:
                    if (TimeEventManager.Instance == null)
                    {
                        continue;
                    }

                    // エリアの表示期間のカウントダウンを算出
                    CycleParam cCycleParam = TimeEventManager.Instance.GetEventCycleFurtureParam(cEvent.event_id);
                    if (cCycleParam == null)
                    {
                        continue;
                    }

                    unTimingStart = cCycleParam.timingStart;
                    unTimingEnd = cCycleParam.timingEnd;
                    break;
            }

            //--------------------------------
            // @change Developer 2016/07/29 無期限対応
            //--------------------------------
            if (cEvent.timing_end > 0)
            {
                bool bCheckFurtureTime = TimeManager.Instance.CheckFurtureTime(unTimingStart, unTimingEnd);
                if (bCheckFurtureTime == false)
                {
                    continue;
                }
            }

            // 最も直近のイベントを探す
            if (cResultEvent == null
            || cResultEvent.timing_start > unTimingStart)
            {
                cResultEvent = cEvent;
            }
        }

        return (cResultEvent);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：イベントマスターデータ取得(FixID指定版)
	*/
    //----------------------------------------------------------------------------
    static public MasterDataEvent GetMasterDataEventFromFixID(uint unFixID)
    {
        MasterDataEvent resMasterDataEvent = MasterFinder<MasterDataEvent>.Instance.SelectFirstWhere("where fix_id = ? ", unFixID);
        if (resMasterDataEvent != null)
        {
            return resMasterDataEvent;
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：通知用マスターデータ取得
	*/
    //----------------------------------------------------------------------------
    static public bool IsMasterDataNotification()
    {
        return MasterFinder<MasterDataNotification>.Instance.IsCache();
    }

    static public MasterDataNotification[] GetMasterDataNotification()
    {
        return MasterFinder<MasterDataNotification>.Instance.GetAll();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：通知用マスターデータ取得(ID指定版)
	*/
    //----------------------------------------------------------------------------
    static public MasterDataNotification GetMasterDataNotification(uint unFixID, ServerDataDefine.NOTIFICATION_TYPE type)
    {
        // notification_masterはfix_idとtypeが同一のものが存在する可能性がある
        // 同一の場合は、timing_startのみがかわる（notification_title、text、notification_type、event_schedule_show、tag_idは同一）
        // とのこと
        var resMasterDataNotificationt = MasterFinder<MasterDataNotification>.Instance.SelectFirstWhere("where fix_id = ? and type = ? ", unFixID, (int)type);
        if (resMasterDataNotificationt != null)
        {
            return resMasterDataNotificationt;
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：有効な初心者ブースト取得
	*/
    //----------------------------------------------------------------------------
    static public MasterDataBeginnerBoost GetMasterDataBeginnerBoost()
    {
        //----------------------------------------
        //
        //----------------------------------------
        MasterDataBeginnerBoost[] masterBoost = MasterFinder<MasterDataBeginnerBoost>.Instance.GetAll();
        for (int i = 0; i < masterBoost.Length; i++)
        {
            if (masterBoost[i] == null)
                continue;

            //----------------------------------------
            // ランク帯が範囲内かチェック
            //----------------------------------------
            if (masterBoost[i].rank_min > UserDataAdmin.Instance.m_StructPlayer.rank
            || masterBoost[i].rank_max < UserDataAdmin.Instance.m_StructPlayer.rank
            ) continue;


            //----------------------------------------
            // イベント開催チェック
            // ※イベントIDが０の場合は無条件適用可能
            //----------------------------------------
            if (masterBoost[i].event_id != 0)
            {
                if (TimeEventManager.Instance.ChkEventActive(masterBoost[i].event_id) == false)
                {
                    continue;
                }
            }

            //----------------------------------------
            // ブースト一致！
            //----------------------------------------
            return masterBoost[i];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ブースト補正：合成金額に補正値を掛け合わせたものを取得
	*/
    //----------------------------------------------------------------------------
    static public uint ConvertBeginnerBoostBuildMoney(ref MasterDataBeginnerBoost cBeginnerMasterData, uint nMoney)
    {
        //----------------------------------------
        // ブースト発動してないなら元のまま
        //----------------------------------------
        if (cBeginnerMasterData == null)
        {
            return nMoney;
        }

        //----------------------------------------
        // 初期値が０なら０のまま
        //----------------------------------------
        if (nMoney == 0)
        {
            return 0;
        }

        //----------------------------------------
        // 倍率が完全に０なら０のまま
        //----------------------------------------
        if (cBeginnerMasterData.boost_build_money == 0)
        {
            return 0;
        }

        //----------------------------------------
        // マスターデータの割合を掛け合わせ
        //----------------------------------------
        float fRate = (float)(cBeginnerMasterData.boost_build_money * 0.01f + 0.000001f);
        nMoney = (uint)(nMoney * fRate);

        return nMoney;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	浮動少数を文字列型へ変換
		@note	誤差によって表示が狂うことがあるため補正を含む
	*/
    //----------------------------------------------------------------------------
    static public string FloatToString(float fValue, int nKeta)
    {
        //----------------------------------
        // １.整数型に変換
        //		桁倍率 = 3 = 1000
        //		0.5 → 500
        // ２.ケタ倍率で割った値を文字追加してもとから引く
        //		500/1000 = 0
        //		500 - 0
        // ３.残数が0じゃないなら次の桁チェック
        //		500/1000 = 0
        //----------------------------------
        int nKetaMult = 1;
        for (int i = 0; i < nKeta; i++)
        {
            nKetaMult *= 10;
        }

        int nValueTotal = (int)(fValue * nKetaMult);

        int nVallueChk = (nValueTotal / nKetaMult);
        string strAnswer = "";
        strAnswer += nVallueChk;

        nValueTotal -= (nVallueChk * nKetaMult);
        if (nValueTotal > 0)
        {
            strAnswer += ".";
            for (; ; )
            {
                nKetaMult /= 10;

                nVallueChk = (nValueTotal / nKetaMult);
                nValueTotal -= (nVallueChk * nKetaMult);
                strAnswer += nVallueChk;

                if (nValueTotal <= 0
                || nKetaMult == 1
                )
                {
                    break;
                }
            }
        }

        return strAnswer;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	クエスト入場条件
	*/
    //----------------------------------------------------------------------------
    static public MasterDataQuestRequirement GetMasterDataQuestRequirementFromID(uint fix_id)
    {
        return MasterFinder<MasterDataQuestRequirement>.Instance.Find((int)fix_id);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	エリア取得：イベントID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataArea GetAreaParamFromEventID(uint unEventID)
    {
        //----------------------------------------
        // 0番は対象外
        //----------------------------------------
        if (unEventID == 0)
        {
            return null;
        }

        MasterDataArea retMasterDataArea = MasterFinder<MasterDataArea>.Instance.SelectFirstWhere("where event_id = ? ", unEventID);
        if (retMasterDataArea == null)
        {
            return null;
        }

        return retMasterDataArea;
    }

    //----------------------------------------------------------------------------
    /*!
	    @brief	エリアカテゴリパラメータ取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataAreaCategory GetAreaCategoryParamFromID(uint unID)
    {
        return MasterFinder<MasterDataAreaCategory>.Instance.Find((int)unID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	テキストデータ取得：キー値指定
	*/
    //----------------------------------------------------------------------------

    static private Dictionary<string, string> TextDefinition = null;

    static public void ResetTextDefinition()
    {
        if (TextDefinition != null)
        {
            TextDefinition.Clear();
            TextDefinition = null;
        }
    }

    static public string GetTextDefinitionTextFromKey(string strKey)
    {
        //テキスト定義マスタを参照する
        if (MasterFinder<MasterDataTextDefinition>.Instance == null)
        {
            return "";
        }

        //----------------------------------------
        // 対象外
        //----------------------------------------
        if (strKey == null ||
            strKey.Length <= 0)
        {
            return "";
        }

        //----------------------------------------
        // キー値の一致するテキストを取得
        //----------------------------------------

        if (TextDefinition.IsNullOrEmpty() == true)
        {
            if (TextDefinition == null)
            {
                TextDefinition = new Dictionary<string, string>();
            }

            List<MasterDataTextDefinition> textDefinition = MasterFinder<MasterDataTextDefinition>.Instance.FindAll();
            for (int i = 0; i < textDefinition.Count; i++)
            {
                MasterDataTextDefinition master = textDefinition[i];
                TextDefinition[master.text_key] = master.text;
            }
        }

        if (TextDefinition.ContainsKey(strKey))
        {
            return TextDefinition[strKey];
        }
        else
        {
            return "";
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		マスターデータ取得：オーディオ再生情報マスター取得
		@param[in]	fix_id		指定のID
	*/
    //----------------------------------------------------------------------------
    static public MasterDataAudioData GetMasterDataAudioDataFromID(uint fix_id)
    {
        MasterDataAudioData retMasterDataAudioData = MasterFinder<MasterDataAudioData>.Instance.SelectFirstWhere(" where fix_id = ? ", fix_id);
        if (retMasterDataAudioData == null)
        {
            return null;
        }

        return retMasterDataAudioData;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	消費アイテムマスター取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataUseItem GetMasterDataUseItemFromID(uint unID)
    {
        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        return MasterFinder<MasterDataUseItem>.Instance.Find((int)unID);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	消費アイテム：アイテム名取得：ガチャマスター指定
	*/
    //----------------------------------------------------------------------------
    static public string GetItemNameFromGachaMaster(MasterDataGacha _master)
    {
        MasterDataUseItem retMasterDataUseItem = GetMasterDataUseItemFromID(_master.cost_item_id);
        if (retMasterDataUseItem == null)
        {
            return "";
        }

        return retMasterDataUseItem.item_name;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ランク：ランク最大時ユニットボックス最大数
		@note	一度もチップによる拡張を行っていない場合の値
	*/
    //----------------------------------------------------------------------------
    static public uint GetUnitMaxRankMax()
    {
        SimpleSQL.SimpleDataTable resMasterDataUserRank = MasterFinder<MasterDataUserRank>.Instance.SelectColumnWhere(" MAX(unit_max) as unit_max ", "");
        if (resMasterDataUserRank == null)
        {
            return 0;
        }

        return Convert.ToUInt32(resMasterDataUserRank.rows[0][0]);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ランク：ランク最大時フレンド拡張最大数
		@note	一度もチップによる拡張を行っていない場合の値
	*/
    //----------------------------------------------------------------------------
    static public uint GetFriendMaxRankMax()
    {
        SimpleSQL.SimpleDataTable resMasterDataUserRank = MasterFinder<MasterDataUserRank>.Instance.SelectColumnWhere(" MAX(friend_max) as friend_max ", "");
        if (resMasterDataUserRank == null)
        {
            return 0;
        }

        return Convert.ToUInt32(resMasterDataUserRank.rows[0][0]);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	共通定義取得(Value)：ID指定
	*/
    //----------------------------------------------------------------------------
    static public int GetMasterDataGlobalParamFromID(uint unID)
    {
        MasterDataGlobalParams retMasterDataGlobalParams = MasterFinder<MasterDataGlobalParams>.Instance.SelectFirstWhere(" where fix_id = ? ", unID);
        if (retMasterDataGlobalParams == null)
        {
            return 0;
        }
        return retMasterDataGlobalParams.value;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	共通定義取得(Text)：ID指定
	*/
    //----------------------------------------------------------------------------
    static public string GetMasterDataGlobalParamTextFromID(uint unID)
    {
        MasterDataGlobalParams retMasterDataGlobalParams = MasterFinder<MasterDataGlobalParams>.Instance.SelectFirstWhere(" where fix_id = ? ", unID);
        if (retMasterDataGlobalParams == null)
        {
            return "";
        }
        return retMasterDataGlobalParams.text;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ユニット所持枠最大数取得
		@note	ランクマスターと共通定義から算出する
				ランクアップによる増加量+チップ消費による購入の増加量
	*/
    //----------------------------------------------------------------------------
    static public uint GetUserUnitMax()
    {
        uint UnitExtend = (uint)(GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_UNIT_MAX_EXTEND) * GlobalDefine.SHOP_BUY_UNIT_ADD);
        return GetUnitMaxRankMax() + UnitExtend;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	フレンド枠最大数取得
		@note	ランクマスターと共通定義から算出する
				ランクアップによる増加量+チップ消費による購入の増加量
	*/
    //----------------------------------------------------------------------------
    static public uint GetUserFriendMax()
    {
        uint FriendExtend = (uint)(GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_FRIEND_MAX_EXTEND) * GlobalDefine.SHOP_BUY_FRIEND_ADD);
        return GetFriendMaxRankMax() + FriendExtend;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化合成時の限界突破ボーナス値取得
		@note
	*/
    //----------------------------------------------------------------------------
    static public double GetBuildUpLimitOverBonus()
    {
        double dLimitOverBouns = ((double)GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_LIMITOVER_BOUNS_BUILDUP) / 100);
        return dLimitOverBouns;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	売却時の限界突破ボーナス値取得
		@note
	*/
    //----------------------------------------------------------------------------
    static public double GetSaleLimitOverBonus()
    {
        double dLimitOverBouns = ((double)GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_LIMITOVER_BOUNS_SALE) / 100);
        return dLimitOverBouns;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	限界突破タイプから限界突破情報取得
		@note
	*/
    //----------------------------------------------------------------------------
    static public MasterDataLimitOver GetLimitOverFromID(int limit_over_type)
    {
        MasterDataLimitOver MasterDataLimitOver = MasterFinder<MasterDataLimitOver>.Instance.Find(limit_over_type);
        if (MasterDataLimitOver == null)
        {
            return null;
        }

        return MasterDataLimitOver;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	クエストキーマスター取得：ID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataQuestKey GetMasterDataQuestKeyFromID(uint unID)
    {
        if (MasterFinder<MasterDataQuestKey>.Instance == null)
        {
            return null;
        }

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        MasterDataQuestKey[] questkeys = MasterFinder<MasterDataQuestKey>.Instance.GetAll();

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        for (int i = 0; i < questkeys.Length; i++)
        {
            if (questkeys[i].fix_id != unID)
            {
                continue;
            }

            return questkeys[i];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	クエストキーマスター取得：AreaID指定
	*/
    //----------------------------------------------------------------------------
    static public MasterDataQuestKey GetMasterDataQuestKeyFromAreaCategoryID(uint unAreaCategoryID)
    {
        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unAreaCategoryID == 0)
        {
            return null;
        }

        List<MasterDataQuestKey> keyList = MasterFinder<MasterDataQuestKey>.Instance.SelectWhere("where key_area_category_id = ?", unAreaCategoryID);
        if (keyList.Count == 0)
        {
            return null;
        }

        return keyList[0];
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：新クエスト情報
	*/
    //----------------------------------------------------------------------------
    static public MasterDataQuest2 GetQuest2ParamFromID(uint unID)
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (DebugOptionInGame.Instance != null
        && DebugOptionInGame.Instance.m_Quest2fromJson == true)
        {
            return DebugOptionInGame.Instance.GetQuest2ParamFromID(unID);
        }
#endif

        //----------------------------------------
        // 0番は不整合調整用データなので無視
        //----------------------------------------
        if (unID == 0)
        {
            return null;
        }

        //----------------------------------------
        // 指定IDのデータを返す
        //----------------------------------------
        switch (GetQuestType(unID))
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                return MasterFinder<MasterDataQuest2>.Instance.Find((int)unID);
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                return MasterFinder<MasterDataChallengeQuest>.Instance.Find((int)unID);
            default:
                break;
        }
        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：新クエストフレンドポイント取得可能かどうか
	*/
    //----------------------------------------------------------------------------
    static public bool GetQuest2FriendPointEnabel(uint unID)
    {
        bool ret = true;

        MasterDataQuest2 quest_param = GetQuest2ParamFromID(unID);
        if (quest_param == null)
        {
            return ret;
        }

        if (quest_param.enable_friendpoint == MasterDataDefineLabel.BoolType.DISABLE)
        {
            ret = false;
        }

        return ret;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：新クエストバトル回数(ボス以外)
	*/
    //----------------------------------------------------------------------------
    static public int GetQuest2BattleNum(uint unID)
    {
        int ret = 0;

        MasterDataQuest2 quest_param = GetQuest2ParamFromID(unID);
        if (quest_param == null)
        {
            return ret;
        }

        uint[] enemy_group_tbl =
        {
            quest_param.enemy_group_id_1,
            quest_param.enemy_group_id_2,
            quest_param.enemy_group_id_3,
            quest_param.enemy_group_id_4,
            quest_param.enemy_group_id_5,
            quest_param.enemy_group_id_6,
            quest_param.enemy_group_id_7,
            quest_param.enemy_group_id_8,
            quest_param.enemy_group_id_9,
            quest_param.enemy_group_id_10,
            quest_param.enemy_group_id_11,
            quest_param.enemy_group_id_12,
            quest_param.enemy_group_id_13,
            quest_param.enemy_group_id_14,
            quest_param.enemy_group_id_15,
            quest_param.enemy_group_id_16,
            quest_param.enemy_group_id_17,
            quest_param.enemy_group_id_18,
            quest_param.enemy_group_id_19,
        };

        for (int i = 0; i < enemy_group_tbl.Length; ++i)
        {
            if (enemy_group_tbl[i] == 0) break;
            ++ret;
        }

        return ret;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	マスターデータ取得：新クエストバトル回数(ボス以外)
	*/
    //----------------------------------------------------------------------------
    static public uint GetQuest2EnemyGroup(uint unID, int nIndex)
    {
        if (nIndex >= 19)
        {
            return 0;
        }

        MasterDataQuest2 quest_param = GetQuest2ParamFromID(unID);
        if (quest_param == null)
        {
            return 0;
        }

        uint[] enemy_group_tbl =
        {
            quest_param.enemy_group_id_1,
            quest_param.enemy_group_id_2,
            quest_param.enemy_group_id_3,
            quest_param.enemy_group_id_4,
            quest_param.enemy_group_id_5,
            quest_param.enemy_group_id_6,
            quest_param.enemy_group_id_7,
            quest_param.enemy_group_id_8,
            quest_param.enemy_group_id_9,
            quest_param.enemy_group_id_10,
            quest_param.enemy_group_id_11,
            quest_param.enemy_group_id_12,
            quest_param.enemy_group_id_13,
            quest_param.enemy_group_id_14,
            quest_param.enemy_group_id_15,
            quest_param.enemy_group_id_16,
            quest_param.enemy_group_id_17,
            quest_param.enemy_group_id_18,
            quest_param.enemy_group_id_19,
        };

        return enemy_group_tbl[nIndex];
    }

    /// <summary>
    /// 選択中のheroのfix_idを取得する
    /// </summary>
    /// <returns></returns>
    static public uint GetCurrentHeroID()
    {
        MasterDataHero[] heros = MasterFinder<MasterDataHero>.Instance.GetAll();
        uint def_id = heros[0].fix_id;//デフォルト

        if (UserDataAdmin.Instance.m_StructHeroList == null)
        {
            return def_id;
        }

        int index = Array.FindIndex(UserDataAdmin.Instance.m_StructHeroList, v => v.unique_id == UserDataAdmin.Instance.m_StructPlayer.current_hero_id);
        if (index < 0)
        {
            return def_id;
        }

        return (uint)UserDataAdmin.Instance.m_StructHeroList[index].hero_id;
    }


    /// <summary>
    /// プレゼントの付与数を取得する
    /// </summary>
    /// <param name="presentData"></param>
    /// <returns></returns>
    static public int GetPresentCount(MasterDataPresent presentData)
    {
        int count = 0;
        if (presentData != null)
        {
            switch (presentData.present_type)
            {
                case MasterDataDefineLabel.PresentType.NONE:
                    break;
                case MasterDataDefineLabel.PresentType.MONEY:
                case MasterDataDefineLabel.PresentType.FP:
                case MasterDataDefineLabel.PresentType.STONE:
                case MasterDataDefineLabel.PresentType.STONE_PAY:
                    count = presentData.present_param1;
                    break;
                case MasterDataDefineLabel.PresentType.UNIT:
                    // 旧仕様でPacketStructPresent.present_value2に0が設定される場合があるので
                    // 1をデフォルトにしてpresent_value2が2以上だったときに反映する
                    count = 1;
                    if (presentData.present_param8 > 1)
                    {
                        count = presentData.present_param8;
                    }
                    break;
                case MasterDataDefineLabel.PresentType.TICKET:
                case MasterDataDefineLabel.PresentType.PARTY_PRESET:
                    count = presentData.present_param1;
                    break;
                case MasterDataDefineLabel.PresentType.SCRATCH:
                    count = 1; // 付与数の値がないのでとりあえず1と表示
                    break;
                case MasterDataDefineLabel.PresentType.ITEM:
                case MasterDataDefineLabel.PresentType.QUEST_KEY:
                    count = presentData.present_param2;
                    break;
                case MasterDataDefineLabel.PresentType.UNIT_POINT:
                    count = presentData.present_param1;
                    break;
                case MasterDataDefineLabel.PresentType.EVENT_POINT:
                    count = presentData.present_param2;
                    break;
                case MasterDataDefineLabel.PresentType.MSG:
                case MasterDataDefineLabel.PresentType.NOTICE:
                    count = 1;
                    break;
                default:
#if BUILD_TYPE_DEBUG
                    Debug.Log("GetPresentCount::presentData.present_type: " + presentData.present_type);
#endif
                    break;
            }
        }

        return count;
    }

    static public string GetPresentName(MasterDataPresent present_master)
    {
        string result = "";

        // present_type 毎に引き数の意味合いが異なる
        switch (present_master.present_type)
        {
            case MasterDataDefineLabel.PresentType.MONEY:
                result = GameTextUtil.GetText("common_text11");
                break;
            case MasterDataDefineLabel.PresentType.FP:
                result = GameTextUtil.GetText("common_text1");
                break;
            case MasterDataDefineLabel.PresentType.STONE:
            case MasterDataDefineLabel.PresentType.STONE_PAY:
                result = GameTextUtil.GetText("common_text6");
                break;
            case MasterDataDefineLabel.PresentType.UNIT:
                {
                    MasterDataParamChara charaMaster = MasterFinder<MasterDataParamChara>.Instance.Find(present_master.present_param1);
                    if (charaMaster != null)
                    {
                        result = charaMaster.name;
                    }
                }
                break;
            case MasterDataDefineLabel.PresentType.TICKET:
                result = GameTextUtil.GetText("common_text10");
                break;
            case MasterDataDefineLabel.PresentType.SCRATCH:
                {
                    MasterDataGachaTicket gachaMaster = MasterFinder<MasterDataGachaTicket>.Instance.Find(present_master.present_param1);
                    if (gachaMaster != null)
                    {
                        result = gachaMaster.gacha_tk_name;
                    }
                }
                break;
            case MasterDataDefineLabel.PresentType.ITEM:
                {
                    MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find(present_master.present_param1);
                    if (itemMaster != null)
                    {
                        result = itemMaster.item_name;
                    }
                }
                break;
            case MasterDataDefineLabel.PresentType.QUEST_KEY:
                {
                    MasterDataQuestKey questKeyMaster = MasterFinder<MasterDataQuestKey>.Instance.Find(present_master.present_param1);
                    if (questKeyMaster != null)
                    {
                        result = questKeyMaster.key_name;
                    }
                }
                break;
            case MasterDataDefineLabel.PresentType.UNIT_POINT:
                result = GameTextUtil.GetText("common_text2");
                break;
            case MasterDataDefineLabel.PresentType.PARTY_PRESET:
                result = GameTextUtil.GetText("common_text12");
                break;
            case MasterDataDefineLabel.PresentType.MSG:
            case MasterDataDefineLabel.PresentType.NOTICE:
                result = GameTextUtil.GetText("loginbonus_operation_info");
                break;
            default:
#if BUILD_TYPE_DEBUG
                Debug.Log("GetPresentName:: present_master.present_type: " + present_master.present_type);
#endif
                // リザーブ
                // パーティプリセット が入る可能性があります
                break;
        }
        return result;
    }

    static public MasterDataPresent ConvertStructPresentToMasterData(PacketStructPresent present_data)
    {
        // HACK: とりあえずfix_idと個数のみ対応

        if (present_data == null)
        {
            return null;
        }

        MasterDataPresent master = new MasterDataPresent();
        master.present_type = (MasterDataDefineLabel.PresentType)present_data.present_type;
        master.present_param1 = (int)present_data.present_value0;
        master.present_param2 = (int)present_data.present_value1;
        //master.present_param3 = (int)present_data.present_value2;
        //master.present_param4 = (int)present_data.present_value3;
        //master.present_param5 = (int)present_data.present_value4;
        //master.present_param6 = (int)present_data.present_value5;
        //master.present_param7 = (int)present_data.present_value6;
        //master.present_param8 = (int)present_data.present_value7;
        //master.present_param9 = (int)present_data.present_value8;
        //master.present_param10 = (int)present_data.present_value9;

        if (master.present_type == MasterDataDefineLabel.PresentType.UNIT)
        {
            //master.present_param3 = 0;
            master.present_param8 = (int)present_data.present_value2;
        }

        return master;
    }

    /// <summary>
    /// プレゼントの中に受け取り設定にしたタイプがあるかチェックする
    /// </summary>
    /// <param name="present_ids"></param>
    /// <param name="receive_type"></param>
    /// <param name="is_once">true:1件目のみチェックする false:全件チェックする</param>
    /// <returns>true:ある false:ない</returns>
    static public bool CheckReceivePresentType(uint[] present_ids, MasterDataDefineLabel.AchievementReceiveType receive_type, bool is_once = false)
    {
        if (receive_type == MasterDataDefineLabel.AchievementReceiveType.NONE)
        {
            return true;
        }

        if (present_ids.IsNullOrEmpty() == true)
        {
            return false;
        }

        uint[] tmp_present_ids;
        if (is_once == true)
        {
            tmp_present_ids = new uint[] { present_ids[0] };
        }
        else
        {
            tmp_present_ids = present_ids;
        }

        bool isCheck = false;

        for (int i = 0; i < tmp_present_ids.Length; ++i)
        {
            MasterDataPresent presentMaster = MasterDataUtil.GetPresentParamFromID(tmp_present_ids[i]);
            switch (presentMaster.present_type)
            {
                case MasterDataDefineLabel.PresentType.MONEY:
                    isCheck = (receive_type == MasterDataDefineLabel.AchievementReceiveType.COIN);
                    break;
                case MasterDataDefineLabel.PresentType.STONE:
                case MasterDataDefineLabel.PresentType.STONE_PAY:
                    isCheck = (receive_type == MasterDataDefineLabel.AchievementReceiveType.CHIP);
                    break;
                case MasterDataDefineLabel.PresentType.UNIT:
                    isCheck = (receive_type == MasterDataDefineLabel.AchievementReceiveType.UNIT);
                    break;
                default:
                    isCheck = (receive_type == MasterDataDefineLabel.AchievementReceiveType.OTHER);
                    break;
            }

            if (isCheck == true)
            {
                break;
            }
        }

        return isCheck;
    }

    /// <summary>
    /// スタミナ回復アイテムかどうかチェックする
    /// </summary>
    /// <param name="item_master"></param>
    /// <returns></returns>
    static public bool ChkUseItemTypeStaminaRecovery(MasterDataUseItem item_master)
    {
        return (item_master.stamina_recovery != 0);
    }

    /// <summary>
    /// 報酬アップアイテムかどうかチェックする
    /// </summary>
    /// <param name="item_master"></param>
    /// <returns></returns>
    static public bool ChkUseItemTypeAmend(MasterDataUseItem item_master)
    {
        return (item_master.exp_amend != 0 ||
                item_master.coin_amend != 0 ||
                item_master.fp_amend != 0 ||
                item_master.link_amend != 0 ||
                item_master.tk_amend != 0);
    }

    /// <summary>
    /// リージョンID取得（カテゴリートップ）
    /// </summary>
    /// <param name="category"></param>
    /// <param name="isPrevRegion">前回表示したリージョンを取得するかどうか true:する false:しない</param>
    /// <returns></returns>
    static public uint GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY category, bool isPrevRegion = true)
    {
        uint ret = 0;

        List<MasterDataRegion> list = MainMenuUtil.CreateRegionList(category);
        if (list != null && list.Count > 0)
        {
            if (isPrevRegion == true)
            {
                uint selectedRegionID = ResidentParam.m_RegionIds[(int)category];
                int index = list.FindIndex((v) => v.fix_id == selectedRegionID);
                if (index >= 0)
                {
                    ret = list[index].fix_id;
                }
                else
                {
                    // 開催期間が切れて有効ではない場合
                    ret = list[0].fix_id;
                }

            }
            else
            {
                ret = list[0].fix_id;
            }

        }

        return ret;
    }

    public static readonly string DEF_ITEM_ICON_NAME = "divine_icon";

    /// <summary>
    /// アイテムアイコン取得（ID指定）
    /// </summary>
    /// <param name="item_id"></param>
    /// <param name="action"></param>
    static public void GetItemIcon(uint item_id, System.Action<Sprite> action)
    {
        MasterDataUseItem cItemMaster = MasterDataUtil.GetMasterDataUseItemFromID(item_id);
        if (cItemMaster == null)
        {
            //デフォルト設定
            action(ResourceManager.Instance.Load(DEF_ITEM_ICON_NAME, ResourceType.Common));
        }

        GetItemIcon(cItemMaster, action);
    }

    /// <summary>
    /// アイテムアイコン取得(マスター指定)
    /// </summary>
    /// <param name="itemMaster"></param>
    /// <param name="action"></param>
    static public void GetItemIcon(MasterDataUseItem itemMaster, System.Action<Sprite> action)
    {
        if (itemMaster == null)
        {
            //デフォルト設定
            action(ResourceManager.Instance.Load(DEF_ITEM_ICON_NAME, ResourceType.Common));
        }

        if (itemMaster.gacha_event_id == 0)
        {
            //通常アイテム(内部リソース)
            action(ResourceManager.Instance.Load(itemMaster.item_icon, ResourceType.Common));
        }
        else
        {
            //ガチャチケットアイテム(AssetBundle)
            UnitIconImageProvider.Instance.GetEtc(itemMaster.item_icon, action);
        }
    }

    static public List<MasterDataScoreEvent> GetActiveScoreEvent()
    {
        List<MasterDataScoreEvent> eventList = new List<MasterDataScoreEvent>();
        List<MasterDataScoreEvent> closeList = new List<MasterDataScoreEvent>();

        MasterDataScoreEvent[] scoreEvents = MasterFinder<MasterDataScoreEvent>.Instance.GetAll();
        if (scoreEvents != null &&
            scoreEvents.Length != 0)
        {
            for (int i = 0; i < scoreEvents.Length; i++)
            {
                if (scoreEvents[i].event_id == 0 ||
                    scoreEvents[i].timing_start == 0)
                {
                    continue;
                }

                if (TimeEventManager.Instance.ChkEventActive(scoreEvents[i].event_id) == true)
                {
                    //開催中
                    eventList.Add(scoreEvents[i]);
                }
                else if (TimeManager.Instance.CheckWithinTime(scoreEvents[i].timing_start, scoreEvents[i].receiving_end) == true)
                {
                    //報酬受取期間中
                    closeList.Add(scoreEvents[i]);
                }
            }

            if (eventList.Count != 0)
            {
                eventList.Sort((a, b) => a.priority - b.priority);
            }

            eventList.AddRange(closeList);
        }
        return eventList;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    static public List<MasterDataChallengeEvent> GetActiveChallengeEvent()
    {
        List<MasterDataChallengeEvent> eventList = new List<MasterDataChallengeEvent>();
        List<MasterDataChallengeEvent> closeList = new List<MasterDataChallengeEvent>();

        MasterDataChallengeEvent[] challengeEvents = MasterFinder<MasterDataChallengeEvent>.Instance.GetAll();
        if (challengeEvents != null &&
            challengeEvents.Length != 0)
        {
            for (int i = 0; i < challengeEvents.Length; i++)
            {
                if (challengeEvents[i].event_id == 0 ||
                    challengeEvents[i].timing_start == 0)
                {
                    continue;
                }

                //表示期間判定
                if (TimeManager.Instance.CheckWithinTime(challengeEvents[i].timing_start, challengeEvents[i].receiving_end) == false)
                {
                    continue;
                }

                //開催期間判定
                if (TimeEventManager.Instance.ChkEventActive(challengeEvents[i].event_id) == true)
                {
                    //開催中
                    eventList.Add(challengeEvents[i]);
                }
                else
                {
                    //報酬受取期間中
                    closeList.Add(challengeEvents[i]);
                }
            }

            eventList.AddRange(closeList);
        }
        return eventList;
    }

    /// <summary>
    /// 成長ボスクエストIDか判定
    /// </summary>
    /// <param name="quest_id"></param>
    /// <returns></returns>
    static public MasterDataDefineLabel.QuestType GetQuestType(uint quest_id)
    {
        if (quest_id >= GlobalDefine.CHALLENGE_QUEST_OFFSET)
        {
            return MasterDataDefineLabel.QuestType.CHALLENGE;
        }
        else if (quest_id >= GlobalDefine.RENEW_QUEST_OFFSET)
        {
            return MasterDataDefineLabel.QuestType.NORMAL;
        }
        //旧クエストのIDが来ることはないはず
        return MasterDataDefineLabel.QuestType.OLD;
    }

    /// <summary>
    /// ボス特性文字列取得
    /// </summary>
    /// <param name="master"></param>
    /// <returns></returns>
    static public string GetBossAbilityText(MasterDataQuest2 master)
    {
        //特性なし
        if (master.boss_ability_1 == 0 &&
            master.boss_ability_2 == 0 &&
            master.boss_ability_3 == 0 &&
            master.boss_ability_4 == 0)
        {
            return "";
        }

        // 特性を配列化
        uint[] enemyAbility = new uint[4];
        {
            enemyAbility[0] = master.boss_ability_1;
            enemyAbility[1] = master.boss_ability_2;
            enemyAbility[2] = master.boss_ability_3;
            enemyAbility[3] = master.boss_ability_4;
        }
        string abilityLabel = "";

        //--------------------------------
        // 特性の内容を設定
        //--------------------------------
        for (int num = 0; num < enemyAbility.Length; ++num)
        {
            if (enemyAbility[num] == 0)
            {
                continue;
            }

            // 特性マスターを取得
            MasterDataEnemyAbility cAbilityMaster = MasterDataUtil.GetEnemyAbilityParamFromID((int)enemyAbility[num]);
            if (cAbilityMaster == null)
            {
                continue;
            }

            // テキストを取得
            abilityLabel += cAbilityMaster.name + "\r\n";
            abilityLabel += cAbilityMaster.detail + "\r\n";
        }

        // 末尾の改行コードを削除
        abilityLabel = abilityLabel.Substring(0, (abilityLabel.Length - 2));
        return abilityLabel;
    }

    static public string GetGachaText(EMASTERDATA_SERVER mastertype, uint fix_id, MasterDataDefineLabel.GachaTextRefType reftype)
    {
        var gachatextrefs = MasterFinder<MasterDataGachaTextRef>.Instance.SelectFirstWhere(" where master_type = ? and master_fix_id = ?",
                                                                                            (int)mastertype,
                                                                                             fix_id);
        if (gachatextrefs == null)
        {
            return "";
        }

        int text_id = 0;
        switch (reftype)
        {
            case MasterDataDefineLabel.GachaTextRefType.GUIDLINE:
                text_id = gachatextrefs.guideline_text_id;
                break;

            case MasterDataDefineLabel.GachaTextRefType.DETAIL:
                text_id = gachatextrefs.detail_text_id;
                break;

            case MasterDataDefineLabel.GachaTextRefType.NOMAL1_RATE_URL:
                text_id = gachatextrefs.normal1_rate_url_text_id;
                break;

            case MasterDataDefineLabel.GachaTextRefType.NOMAL2_RATE_URL:
                text_id = gachatextrefs.normal2_rate_url_text_id;
                break;

            case MasterDataDefineLabel.GachaTextRefType.SPECIAL_RATE_URL:
                text_id = gachatextrefs.special_rate_url_text_id;
                break;
        }

        var gachatext = MasterFinder<MasterDataGachaText>.Instance.SelectFirstWhere(" where fix_id = ?", text_id);

        if (gachatext == null)
        {
            return "";
        }

        return gachatext.text;
    }

    static public MasterDataPresent[] GetPresentMasterFromGroupID(uint group_id)
    {
        List<MasterDataPresentGroup> presentGroupList = MasterFinder<MasterDataPresentGroup>.Instance.SelectWhere("where group_id = ? ORDER BY fix_id ASC", group_id);
        if (presentGroupList == null)
        {
            return null;
        }

        List<MasterDataPresent> presentList = new List<MasterDataPresent>();
        for (int i = 0; i < presentGroupList.Count; ++i)
        {
            MasterDataPresentGroup groupMaster = presentGroupList[i];

            MasterDataPresent presentMaster = MasterFinder<MasterDataPresent>.Instance.Find((int)groupMaster.present_id);
            if (presentMaster == null)
            {
                continue;
            }

            for (int present_num = 0; present_num < groupMaster.present_num; ++present_num)
            {
                MasterDataPresent master = new MasterDataPresent();
                master.Copy(presentMaster);
                presentList.Add(master);
            }
        }

        return presentList.ToArray();
    }

    /// <summary>
    /// 成長ボスイベントマスター取得
    /// </summary>
    /// <param name="quest_id"></param>
    /// <returns></returns>
    static public MasterDataChallengeEvent GetChallengeEventFromQuestID(uint quest_id)
    {
        if (GetQuestType(quest_id) != MasterDataDefineLabel.QuestType.CHALLENGE)
        {
            return null;
        }

        MasterDataChallengeQuest questMaster = MasterFinder<MasterDataChallengeQuest>.Instance.Find((int)quest_id);
        if (questMaster == null)
        {
            return null;
        }

        return MasterFinder<MasterDataChallengeEvent>.Instance.SelectFirstWhere("where event_id = ?", questMaster.event_id);
    }

    /// <summary>
    /// 成長ボスクエストマスター取得
    /// </summary>
    /// <param name="event_id"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    static public MasterDataChallengeQuest GetChallengeQuestMaster(uint event_id, int level)
    {
        MasterDataChallengeQuest[] questMasterAll = MasterFinder<MasterDataChallengeQuest>.Instance.GetAll();

        List<MasterDataChallengeQuest> questList = new List<MasterDataChallengeQuest>(Array.FindAll(questMasterAll, (a) => a.event_id == event_id));

        if (questList.Count != 0)
        {
            questList.Sort((a, b) => a.level_max - b.level_max);
            for (int i = 0; i < questList.Count; i++)
            {
                if (level <= questList[i].level_max)
                {
                    return questList[i];
                }
            }
        }

        return null;
    }

    static public MasterDataAreaCategory GetChallengeAreaCategoryMaster(uint event_id)
    {
        MasterDataChallengeQuest questMaster = GetChallengeQuestMaster(event_id, 1);
        if (questMaster != null)
        {
            MasterDataArea areaMaster = MasterFinder<MasterDataArea>.Instance.Find((int)questMaster.area_id);
            if (areaMaster != null)
            {
                return MasterFinder<MasterDataAreaCategory>.Instance.Find((int)areaMaster.area_cate_id);
            }
        }
        return null;
    }
};

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
