/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreUseTipFriendExt.cs
	@brief	ストア画面でのフレンド枠増加周り
	@author Developer
	@date 	2015/09/11
 
	新UIへの変更にともない、MainMenuSeqShopUseTipFriendではなく
	MainMenuSeqShopRoot画面上に出るようになった為追加
	MainMenuSeqShopUseTipFriendの中身を持ってきて修正する
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
	@brief	ストア画面でのフレンド枠増加周り
*/
//----------------------------------------------------------------------------
public class StoreUseTipFriendExt : StoreUseTipBase
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private MainMenuFriendsList m_FriendList = null;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    public void SetFriendList(MainMenuFriendsList friendList )
    {
        m_FriendList = friendList;
    }

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
        // チップの保持数及びフレンド枠の状態を取得
        //--------------------------------
        uint unHaveTip = 0;
        bool bFriendFull = true;
        if (UserDataAdmin.Instance != null
        && UserDataAdmin.Instance.m_StructPlayer != null)
        {
            // チップ
            unHaveTip = UserDataAdmin.Instance.m_StructPlayer.have_stone;
            // フレンド枠数
            bFriendFull = (UserDataAdmin.Instance.m_StructPlayer.total_friend >= MasterDataUtil.GetUserFriendMax());
            if (bFriendFull == false)
            {
                // 有料拡張分も見る
                bFriendFull = (UserDataAdmin.Instance.m_StructPlayer.extend_friend >= MasterDataUtil.GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_FRIEND_MAX_EXTEND));
            }
        }

        //--------------------------------
        // テキストにフレンド現在数/最大数を表示するため取得
        //--------------------------------
        // 現在フレンド数
        uint unFriendNow = 0;
        if (UserDataAdmin.Instance.m_StructFriendList != null)
        {
            for (int i = 0; i < UserDataAdmin.Instance.m_StructFriendList.Length; i++)
            {
                if (UserDataAdmin.Instance.m_StructFriendList[i] == null)
                    continue;

                // 成立している物のみ
                if (UserDataAdmin.Instance.m_StructFriendList[i].friend_state == (int)FRIEND_STATE.FRIEND_STATE_SUCCESS)
                {
                    unFriendNow++;
                }
            }
        }
        // 文字列化
        string stFriendNow = unFriendNow.ToString();
        if (unFriendNow > UserDataAdmin.Instance.m_StructPlayer.total_friend)
        {
            // フレンド枠最大数よりフレンド数の方が大きければ赤文字
            stFriendNow = "<color=#ff0000>" + unFriendNow.ToString() + "</color>";
        }
        uint unRestExtensionNum = (uint)(MasterDataUtil.GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_FRIEND_MAX_EXTEND) - UserDataAdmin.Instance.m_StructPlayer.extend_friend);
        //--------------------------------
        // 現状のフレンド枠数をテキストとして表示
        //--------------------------------
        string strMainMsg = string.Format(UnityUtil.GetText("sh137q_content"), stFriendNow, UserDataAdmin.Instance.m_StructPlayer.total_friend, unRestExtensionNum);
        // 最大値ではない場合「後何回追加できるか」を表示する
        if (bFriendFull == false)
        {
            //strMainMsg += "\n" + string.Format(UnityUtil.GetText("SHOP_DIALOG_FRIEND_REMAINING"), unRestExtensionNum);
        }

        //--------------------------------
        // チップの有無、フレンド拡張可能かによって表示物を変える
        //--------------------------------
        if (bFriendFull == true)
        {
            //--------------------------------
            // 既に拡張枠最大に到達している
            // →これ以上拡張できないことを示す
            //--------------------------------
            // エラーメッセージを付与して出す
            strMainMsg = UnityUtil.GetText("sh138q_content");
            // ダイアログ表示
            m_DialogChoice = DialogManager.Open1B_Direct(GameTextUtil.GetText("sh138q_title"), strMainMsg, "common_button6", true, true);
            // エラー対策に移動
            m_BuyStep = STORE_TIP_SEQUENCE.ERROR;
        }
        else if (unHaveTip == 0)
        {
            //--------------------------------
            // チップを所持していない場合
            // →魔法石購入を促してそのまま終了
            //--------------------------------
            // エラーメッセージを付与して出す
            strMainMsg = UnityUtil.GetText("sh140q_content");
            // ダイアログ表示
            m_DialogChoice = DialogManager.Open2B_Direct(GameTextUtil.GetText("sh140q_title"), strMainMsg, "common_button4", "common_button5", true, true).SetStrongYes();
            // エラー対策に移動
            m_BuyStep = STORE_TIP_SEQUENCE.NEXT_BUY_TIP;
        }
        else
        {
            //--------------------------------
            // チップがある
            // →チップ使用判定へ移動
            //--------------------------------
            m_DialogChoice = DialogManager.Open2B_Direct(GameTextUtil.GetText("sh137q_title"), strMainMsg, "common_button4", "common_button5", true, true).SetStrongYes();
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
        // フレンド枠拡張通信リクエスト
        ServerDataUtilSend.SendPacketAPI_StoneUsedFriend()
        .setSuccessAction(_data =>
        {
            //-----------------------
            // 通信完遂したんで結果を反映
            //-----------------------
            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvStoneUsedFriend>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();

            //-----------------------
            // ダイアログで結果を通知
            //-----------------------
            string strMainMsg = string.Format(GameTextUtil.GetText("sh139q_content"), UserDataAdmin.Instance.m_StructPlayer.total_friend);
            m_FinishDialog = DialogManager.Open1B_Direct(GameTextUtil.GetText("sh139q_title"), strMainMsg, "common_button1", true, true);

            //フレンド最大数更新
            if (m_FriendList != null) m_FriendList.updateFriendCount();

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
