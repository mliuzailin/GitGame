/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreUseTipStamina.cs
	@brief	ストア画面でのスタミナ回復処理周り
	@author Developer
	@date 	2015/09/11
 
	新UIへの変更にともない、MainMenuSeqShopUseTipStaminaではなく
	MainMenuSeqShopRoot画面上に出るようになった為追加
	MainMenuSeqShopUseTipStaminaの中身を持ってきて修正する
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
	@brief	ストア画面でのスタミナ回復処理クラス
*/
//----------------------------------------------------------------------------
public class StoreUseTipStamina : StoreUseTipBase
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
        // チップの保持数及びスタミナの状態を取得
        //--------------------------------
        uint unHaveTip = 0;
        bool bStaminaFull = true;
        if (UserDataAdmin.Instance != null
        && UserDataAdmin.Instance.m_StructPlayer != null)
        {
            // チップ
            unHaveTip = UserDataAdmin.Instance.m_StructPlayer.have_stone;
            // スタミナ状態
            bStaminaFull = (UserDataAdmin.Instance.m_StructPlayer.stamina_max <= UserDataAdmin.Instance.m_StructPlayer.stamina_now);
        }

        //--------------------------------
        // チップの有無、スタミナの状態によって表示物を変える
        //--------------------------------
        if (bStaminaFull == true)
        {
            //--------------------------------
            // スタミナが満タンの場合
            // →回復する意味がないことを表示してそのまま終了
            //--------------------------------
            m_DialogChoice = DialogManager.Open1B_Direct(GameTextUtil.GetText("sh132q_title"), GameTextUtil.GetText("sh132q_content2"), "common_button1", true, true);
            // エラー対策に移動
            m_BuyStep = STORE_TIP_SEQUENCE.ERROR;
        }
        else if (unHaveTip == 0)
        {
            //--------------------------------
            // チップを所持していない場合
            // →魔法石購入を促してそのまま終了
            //--------------------------------
            m_DialogChoice = DialogManager.Open2B_Direct(GameTextUtil.GetText("sh136q_title"), GameTextUtil.GetText("sh136q_content"), "common_button4", "common_button5", true, true).SetStrongYes();
            // エラー対策に移動
            m_BuyStep = STORE_TIP_SEQUENCE.NEXT_BUY_TIP;
        }
        else
        {
            //--------------------------------
            // チップがある
            // →チップ使用判定へ移動
            //--------------------------------
            string mainText = "";
            if (UserDataAdmin.Instance.m_StructPlayer.stamina_now == 0)
            {
                mainText = GameTextUtil.GetText("sh132q_content1") + "\n\r";
                mainText += string.Format(GameTextUtil.GetText("sh132q_content3"), UserDataAdmin.Instance.m_StructPlayer.stamina_max, UserDataAdmin.Instance.m_StructPlayer.stamina_max);
                m_DialogChoice = DialogManager.Open2B_Direct(GameTextUtil.GetText("sh132q_title"), mainText, "common_button4", "common_button5", true, true).SetStrongYes();
            }
            else
            {
                uint stamina = UserDataAdmin.Instance.m_StructPlayer.stamina_now + UserDataAdmin.Instance.m_StructPlayer.stamina_max;
                mainText = string.Format(GameTextUtil.GetText("sh134q_content"), stamina, UserDataAdmin.Instance.m_StructPlayer.stamina_max);
                m_DialogChoice = DialogManager.Open2B_Direct(GameTextUtil.GetText("sh132q_title"), mainText, "common_button4", "common_button5", true, true).SetStrongYes();
            }
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
        // スタミナ回復通信リクエスト
        ServerDataUtilSend.SendPacketAPI_StoneUsedStamina()
        .setSuccessAction(_data =>
        {

            //-----------------------
            // 通信完遂したんで結果を反映
            //-----------------------
            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvStoneUsedStamina>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();

			// @add Developer 2015/12/02 v310 ローカル通知用に保存
			LocalNotificationUtil.m_StaminaNow = UserDataAdmin.Instance.m_StructPlayer.stamina_now;

            //-----------------------
            // ダイアログで結果を通知
            //-----------------------
            m_FinishDialog = DialogManager.Open1B("sh135q_title", "sh135q_content", "common_button1", true, true);

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
}
