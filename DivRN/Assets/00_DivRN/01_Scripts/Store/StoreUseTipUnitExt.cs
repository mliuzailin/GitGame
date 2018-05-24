/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreUseTipUnitExt.cs
	@brief	ストア画面での所持ユニット枠増加周り
	@author Developer
	@date 	2015/09/11
 
	新UIへの変更にともない、MainMenuSeqShopUseTipUnitではなく
	MainMenuSeqShopRoot画面上に出るようになった為追加
	MainMenuSeqShopUseTipUnitの中身を持ってきて修正する
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
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
	@brief	ストア画面での所持ユニット枠増加周り
*/
//----------------------------------------------------------------------------
public class StoreUseTipUnitExt : StoreUseTipBase
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	処理開始
	*/
    //----------------------------------------------------------------------------
    public override void StartProcess(MainMenuShop cMenuSeqShopRoot)
    {
        //--------------------------------
        // ステップがNONE以外の場合は何もしない
        //--------------------------------
        if (m_BuyStep != STORE_TIP_SEQUENCE.NONE)
        {
            return;
        }
        m_DialogChoice = null;
        m_bNextBuyTip = false;

        //--------------------------------
        // パッチ取得用
        // @add Developer 016/04/20 v340
        //--------------------------------
        m_MenuSeqShopRoot = cMenuSeqShopRoot;

        //--------------------------------
        // チップの保持数及び所持ユニット枠の状態を取得
        //--------------------------------
        uint unHaveTip = 0;
        bool bUnitFull = true;
        if (UserDataAdmin.Instance != null
        && UserDataAdmin.Instance.m_StructPlayer != null)
        {
            // チップ
            unHaveTip = UserDataAdmin.Instance.m_StructPlayer.have_stone;
            // 所持ユニット枠数
            bUnitFull = (UserDataAdmin.Instance.m_StructPlayer.total_unit >= MasterDataUtil.GetUserUnitMax());
            if (bUnitFull == false)
            {
                // 有料拡張分も見る
                bUnitFull = (UserDataAdmin.Instance.m_StructPlayer.extend_unit >= MasterDataUtil.GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_UNIT_MAX_EXTEND));
            }
        }

        //--------------------------------
        // テキストにユニット獲得枠現在数/最大数を表示するため取得
        //--------------------------------
        // 現在フレンド数
        // 所持ユニット
        string stUnitNow = UserDataAdmin.Instance.m_StructPlayer.unit_list.Length.ToString();
        if (UserDataAdmin.Instance.m_StructPlayer.unit_list.Length > UserDataAdmin.Instance.m_StructPlayer.total_unit)
        {
            // 所持ユニット枠最大数より所持ユニット数の方が大きければ赤文字
            stUnitNow = "<color=#ff0000>" + UserDataAdmin.Instance.m_StructPlayer.unit_list.Length.ToString() + "</color>";
        }
        uint unRestExtensionNum = (uint)(MasterDataUtil.GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_UNIT_MAX_EXTEND) - UserDataAdmin.Instance.m_StructPlayer.extend_unit);
        //--------------------------------
        // 現状のユニット枠数をテキストとして表示
        //--------------------------------
        string strMainMsg = string.Format(GameTextUtil.GetText("sh128q_content"), stUnitNow, UserDataAdmin.Instance.m_StructPlayer.total_unit, unRestExtensionNum);
        // 最大値ではない場合「後何回追加できるか」を表示する
        if (bUnitFull == false)
        {
            //strMainMsg += "\n" + string.Format(UnityUtil.GetText("SHOP_DIALOG_UNIT_REMAINING"), unRestExtensionNum);
        }

        //--------------------------------
        // チップの有無、所持ユニット枠拡張可能かによって表示物を変える
        //--------------------------------
        if (bUnitFull == true)
        {
            //--------------------------------
            // 既に拡張枠最大に到達している
            // →これ以上拡張できないことを示す
            //--------------------------------
            // エラーメッセージを付与して出す
            strMainMsg = GameTextUtil.GetText("sh129q_content");
            // ダイアログ表示
            m_DialogChoice = DialogManager.Open1B_Direct(GameTextUtil.GetText("sh129q_title"), strMainMsg, "common_button1", true, true);
            // エラー対策に移動
            m_BuyStep = STORE_TIP_SEQUENCE.ERROR;
        }
        else if (unHaveTip == 0)
        {
            //--------------------------------
            // チップを所持していない場合
            // →魔法石購入へ遷移するか確認
            //--------------------------------
            // エラーメッセージを付与して出す
            strMainMsg = GameTextUtil.GetText("sh131q_content");
            // ダイアログ表示
            m_DialogChoice = DialogManager.Open2B_Direct(GameTextUtil.GetText("sh131q_title"), strMainMsg, "common_button4", "common_button5", true, true).SetStrongYes();
            // チップ購入確認に移動
            m_BuyStep = STORE_TIP_SEQUENCE.NEXT_BUY_TIP;
        }
        else
        {
            //--------------------------------
            // チップがある
            // →チップ使用判定へ移動
            //--------------------------------
            m_DialogChoice = DialogManager.Open2B_Direct(GameTextUtil.GetText("sh128q_title"), strMainMsg, "common_button4", "common_button5", true, true).SetStrongYes();
            // 使用意思確認に移動
            m_BuyStep = STORE_TIP_SEQUENCE.CHOICE;
        }

        m_FinishDialog = null;
    }
 
    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：使用リクエスト
		@note	
		@add	Developer 2016/04/21 v340
	*/
    //----------------------------------------------------------------------------
    protected override bool ExecStepRequest()
    {
        // 所持ユニット枠拡張通信リクエスト
        ServerDataUtilSend.SendPacketAPI_StoneUsedUnit()
        .setSuccessAction(_data =>
        {
            //-----------------------
            // 通信完遂したんで結果を反映
            //-----------------------
            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvStoneUsedUnit>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();

            //-----------------------
            // ダイアログで結果を通知
            //-----------------------
            string strMainMsg = string.Format(UnityUtil.GetText("sh130q_content"), UserDataAdmin.Instance.m_StructPlayer.total_unit);
            m_FinishDialog = DialogManager.Open1B_Direct(GameTextUtil.GetText("sh130q_title"), strMainMsg, "common_button1", true, true);

            //-----------------------
            // ユニットの最大数更新
            //-----------------------
            UpdateUnitGridCount();

            //-----------------------
            // 処理終わったんで戻る
            //-----------------------
            m_BuyStep = STORE_TIP_SEQUENCE.FINISH;
        })
        .setErrorAction(_data =>
        {
            m_BuyStep = STORE_TIP_SEQUENCE.FINISH;
        })
        .SendStart();
        // 通信処理へ
        m_BuyStep = STORE_TIP_SEQUENCE.REQUEST_WAIT;

        return true;
    }

    /// <summary>
    /// ユニットリストのユニット最大数更新
    /// </summary>
    void UpdateUnitGridCount() {
        if (MainMenuManager.HasInstance) {
            UnitGridComplex unit_grid = MainMenuManager.Instance.MainMenuSeqPageNow.GetComponentInChildren<UnitGridComplex>();
            if (unit_grid != null) {
                unit_grid.UpdateUnitCount();
            }
        }
    }
}
