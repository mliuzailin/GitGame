using UnityEngine;
using System.Collections.Generic;
using M4u;

public class DialogManager
{
    static public Dialog Open1B(string strTitleKey, string strMsgKey, string strBtnKey, bool bWall, bool bSE)
    {
        return Open1B_Direct(GameTextUtil.GetText(strTitleKey), GameTextUtil.GetText(strMsgKey), strBtnKey, bWall, bSE);
    }

    static public Dialog Open1B_Direct(string strTitle, string strMsg, string strBtnKey, bool bWall, bool bSE)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, strTitle);
        newDialog.SetDialogText(DialogTextType.MainText, strMsg);
        newDialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText(strBtnKey));
        if (bWall) newDialog.EnableFadePanel();
        newDialog.DisableAutoHide();
        newDialog.Show();

        return newDialog;
    }

    static public Dialog Open2B(string strTitleKey, string strMsgKey, string strBtnKey1, string strBtnKey2, bool bWall, bool bSE)
    {
        return Open2B_Direct(GameTextUtil.GetText(strTitleKey), GameTextUtil.GetText(strMsgKey), strBtnKey1, strBtnKey2, bWall, bSE);
    }

    static public Dialog Open2B_Direct(string strTitle, string strMsg, string strBtnKey1, string strBtnKey2, bool bWall, bool bSE)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogText(DialogTextType.Title, strTitle);
        newDialog.SetDialogText(DialogTextType.MainText, strMsg);
        newDialog.SetDialogText(DialogTextType.YesText, GameTextUtil.GetText(strBtnKey1));
        newDialog.SetDialogText(DialogTextType.NoText, GameTextUtil.GetText(strBtnKey2));
        if (bWall) newDialog.EnableFadePanel();
        newDialog.DisableAutoHide();
        newDialog.Show();

        return newDialog;
    }

    static public Dialog Open2B2(string strTitleKey, string strMsgKey, string strBtnKey1, string strBtnKey2, bool bWall, bool bSE)
    {
        return Open2B2_Direct(GameTextUtil.GetText(strTitleKey), GameTextUtil.GetText(strMsgKey), strBtnKey1, strBtnKey2, bWall, bSE);
    }

    static public Dialog Open2B2_Direct(string strTitle, string strMsg, string strBtnKey1, string strBtnKey2, bool bWall, bool bSE)
    {
        return Open2B_Direct(strTitle, strMsg, strBtnKey1, strBtnKey2, bWall, bSE);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenInGameContinue(int nStoneCt)
    {
        //--------------------------------
        // ダイアログ発行依頼
        //--------------------------------
        string strDialogTitle = GameTextUtil.GetText("INGAME_CONTINUE_TITLE");
        string strDialogMessage = "";
        string strDialogBtn1Key = "";
        string strDialogBtn2Key = "";

        strDialogMessage = GameTextUtil.GetText("INGAME_CONTINUE_MSG");
        strDialogBtn1Key = "common_button4";
        strDialogBtn2Key = "common_button5";

        strDialogMessage = string.Format(strDialogMessage, nStoneCt);

        return Open2B_Direct(strDialogTitle, strDialogMessage, strDialogBtn1Key, strDialogBtn2Key, false, true);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenInGameReset(int nFloorNum, int nFloorMax, int nFloorDisplay, int nStoneCt)
    {
        //--------------------------------
        // ダイアログ発行依頼
        //--------------------------------
        string strDialogTitle = "";
        string strDialogMessage = "";
        string strDialogBtn1Key = "";
        string strDialogBtn2Key = "";
        if (nFloorDisplay == nFloorMax)
        {
            strDialogTitle = GameTextUtil.GetText("INGAME_RESET_FINAL_TITLE");
            strDialogMessage = GameTextUtil.GetText("INGAME_RESET_FINAL_MSG");
            strDialogBtn1Key = "INGAME_RESET_FINAL_BTN_1";
            strDialogBtn2Key = "INGAME_RESET_FINAL_BTN_2";

            strDialogMessage = string.Format(strDialogMessage, nStoneCt);
        }
        else
        {
            strDialogTitle = GameTextUtil.GetText("INGAME_RESET_TITLE");
            strDialogMessage = GameTextUtil.GetText("INGAME_RESET_MSG");
            strDialogBtn1Key = "INGAME_RESET_BTN_1";
            strDialogBtn2Key = "INGAME_RESET_BTN_2";

            string strFloor = "";
            if (nFloorDisplay < 0)
            {
                strFloor = "B" + Mathf.Abs(nFloorDisplay);
            }
            else
            {
                strFloor = nFloorDisplay.ToString();
            }

            strDialogTitle = string.Format(strDialogTitle, strFloor);
            strDialogMessage = string.Format(strDialogMessage, nStoneCt);
        }

        Dialog dialog = Open2B_Direct(strDialogTitle, strDialogMessage, strDialogBtn1Key, strDialogBtn2Key, false, true);
        dialog.DisableCancelButton();
        return dialog;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenInGameResetFix(int nStoneCt)
    {
        //--------------------------------
        // ダイアログ発行依頼
        //--------------------------------
        string strDialogTitle = GameTextUtil.GetText("INGAME_RESET_AGREE_TITLE");
        string strDialogMessage = "";
        string strDialogBtn1Key = "";
        string strDialogBtn2Key = "";
        if (nStoneCt == 0)
        {
            strDialogMessage = GameTextUtil.GetText("INGAME_RESET_AGREE_NOTIP_MSG");
            strDialogBtn1Key = "INGAME_RESET_AGREE_NOTIP_BTN";
            strDialogBtn2Key = "common_button5";

            strDialogMessage = string.Format(strDialogMessage, nStoneCt);
        }
        else
        {
            strDialogMessage = GameTextUtil.GetText("INGAME_RESET_AGREE_MSG");
            strDialogBtn1Key = "INGAME_RESET_FIX_BTN_1";
            strDialogBtn2Key = "common_button5";

            strDialogMessage = string.Format(strDialogMessage, nStoneCt);
        }

        Dialog dialog = Open2B_Direct(strDialogTitle, strDialogMessage, strDialogBtn1Key, strDialogBtn2Key, false, true);
        dialog.DisableCancelButton();
        return dialog;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：ショップ特化：「魔法石を購入しますか？」
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenShopShort()
    {
        //--------------------------------
        // ダイアログ発行依頼
        //--------------------------------
        return Open2B("CONTINUE_tip_title", "CONTINUE_tip_content", "common_button4", "common_button5", true, true);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：ショップ特化：「年齢いくつですか？」
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenAgeVerification(System.Action<DialogButtonEventType> ageAction = null, System.Action okAction = null)
    {
        // プレハブを追加
        Dialog dialog = Dialog.Create(DialogType.DialogShopAgeVerification);
        dialog.SetDialogTextFromTextkey(DialogTextType.MainText, "sh118q_content1");
        List<DialogAgeItem> ageList = new List<DialogAgeItem>();
        ageList.Add(new DialogAgeItem(GameTextUtil.GetText("sh118q_content4"), "", DialogButtonEventType.MULTI01));
        ageList.Add(new DialogAgeItem(GameTextUtil.GetText("sh118q_content3"), "", DialogButtonEventType.MULTI02));
        ageList.Add(new DialogAgeItem(GameTextUtil.GetText("sh118q_content2"), "", DialogButtonEventType.MULTI03));
        dialog.SetAgeEvent(ageList, ageAction);
        dialog.SetOkEvent(okAction, true);

        dialog.SetDialogTextFromTextkey(DialogTextType.Title, "sh118q_title");
        dialog.EnableFadePanel();
        dialog.Show();
        return dialog;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：ショップ特化：「おゆるしもらいましたか？」
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenAgeVerificationUnder(System.Action yesAction, System.Action noAction)
    {
        Dialog dialog = Dialog.Create(DialogType.DialogYesNo);
        dialog.SetDialogTextFromTextkey(DialogTextType.Title, "sh121q_title");
        dialog.SetDialogTextFromTextkey(DialogTextType.MainText, "sh121q_content");
        dialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        dialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        dialog.SetYesEvent(yesAction);
        dialog.SetNoEvent(noAction);
        dialog.DisableAutoHide();
        dialog.EnableFadePanel();
        dialog.Show();
        return dialog;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：ショップ特化：「どれ買いますか？」
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenShopBuySelect(System.Action<DialogTipItem> tipAction = null, System.Action okAction = null)
    {
        // プレハブを追加
        Dialog dialog = Dialog.Create(DialogType.DialogShopSelect);
        dialog.SetDialogTextFromTextkey(DialogTextType.Title, "sh119q_title");

        List<DialogTipItem> tipList = new List<DialogTipItem>();
        StoreProduct[] productList = new StoreProduct[6];
        for (int i = 0; i < 6; i++)
        {
            productList[i] = StoreParam.GetStoreProductChanged(i);
            if (productList[i] == null) productList[i] = StoreParam.GetStoreProductBase(i);
        }
        tipList.Add(new DialogTipItem(productList[0], DialogButtonEventType.MULTI01));
        tipList.Add(new DialogTipItem(productList[1], DialogButtonEventType.MULTI02));
        tipList.Add(new DialogTipItem(productList[2], DialogButtonEventType.MULTI03));
        tipList.Add(new DialogTipItem(productList[3], DialogButtonEventType.MULTI04));
        tipList.Add(new DialogTipItem(productList[4], DialogButtonEventType.MULTI05));
        tipList.Add(new DialogTipItem(productList[5], DialogButtonEventType.MULTI06));
        dialog.SetTipEvent(tipList, tipAction, true);

        dialog.SetOkEvent(okAction, true);

        dialog.SetDialogObjectEnabled(DialogObjectType.Title, false);
        dialog.IsActiveTitleBorder = false;

        dialog.setupUnderKiyaku();

        dialog.EnableFadePanel();
        dialog.Show();
        return dialog;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：タイトルに戻るダイアログ
		@note	MultiDialogを使用する
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenGotoTitle()
    {
        //--------------------------------
        // ダイアログ表示データ、元ダイアログと合わせとく
        //--------------------------------
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("BACKBTN_GOTO_TITLE"));
        newDialog.SetDialogText(DialogTextType.MainText, GameTextUtil.GetText("BACKBTN_GOTO_TITLE_DETAIL"));
        newDialog.SetDialogText(DialogTextType.YesText, GameTextUtil.GetText("common_button4"));
        newDialog.SetDialogText(DialogTextType.NoText, GameTextUtil.GetText("common_button5"));
        newDialog.DisableAutoHide();
        newDialog.Show();
        //--------------------------------
        // ダイアログ発行依頼
        //--------------------------------
        return newDialog;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ダイアログオープン：致命的なエラー発生
	*/
    //----------------------------------------------------------------------------
    static public Dialog OpenError(uint unErrorCode)
    {
        //--------------------------------
        // ダイアログ発行依頼
        //--------------------------------
        string strDialogMsg = GameTextUtil.GetText("ERROR_MSG_CANCEL");
        strDialogMsg = string.Format(strDialogMsg, unErrorCode);
        //--------------------------------
        // ダイアログ表示データ、元ダイアログと合わせとく
        //--------------------------------
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogObjectEnabled(DialogObjectType.Title, false);//タイトルなし
        newDialog.SetDialogText(DialogTextType.MainText, strDialogMsg);
        newDialog.SetDialogText(DialogTextType.YesText, GameTextUtil.GetText("common_button4"));
        newDialog.SetDialogText(DialogTextType.NoText, GameTextUtil.GetText("common_button5"));
        newDialog.DisableAutoHide();
        newDialog.Show();
        //--------------------------------
        // ダイアログ発行依頼
        //--------------------------------
        return newDialog;
    }
}
