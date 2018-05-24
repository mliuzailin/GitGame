/**
 *  @file   MainMenuOthersUser.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/06
 */

using UnityEngine;
using System.Collections;
using ServerDataDefine;

public class MainMenuOthersUser : MainMenuSeq
{
    OthersUser m_OthersUser;
    string m_RenewUUID;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        uint userID = LocalSaveManager.Instance.LoadFuncUserID();

        m_OthersUser = GetComponentInChildren<OthersUser>();
        m_OthersUser.SetPositionAjustStatusBar(new Vector2(0, -180));

        m_OthersUser.OnRenameClickAction = OnRename;
        m_OthersUser.OnPasswordClickAction = OnPassword;
        m_OthersUser.OnGameDataDeleteClickAction = OnGameDataDelete;
        m_OthersUser.OnAchievementClickAction = OnAchievement;
        m_OthersUser.OnLeaderboardClickAction = OnLeaderboard;
        m_OthersUser.UserNameText = UserDataAdmin.Instance.m_StructPlayer.user.user_name;
        m_OthersUser.UserIDText = UnityUtil.CreateDrawUserID(userID);
        m_OthersUser.TollTipNum = UserDataAdmin.Instance.m_StructPlayer.have_stone_pay.ToString();
        m_OthersUser.FreeTipNum = UserDataAdmin.Instance.m_StructPlayer.have_stone_free.ToString();
        if (LocalSaveManager.Instance.LoadFuncTransferPasswordChk() == false)
        {
            m_OthersUser.PasswordText = GameTextUtil.GetText("he173p_button1");
        }
        else
        {
            m_OthersUser.PasswordText = GameTextUtil.GetText("he173p_button2_1");
        }

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.NONE;
    }

    void OnRename()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_OTHERS_USER_RENAME, false, false);
        }
    }

    void OnPassword()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (LocalSaveManager.Instance.LoadFuncTransferPasswordChk() == true)
        {
            PasswordDialog(false);
        }
        else
        {
            ServerDataUtilSend.SendPacketAPI_TransferOrder()
                .setSuccessAction(_data =>
                {
                    LocalSaveTransferPassword TransferPassword = new LocalSaveTransferPassword();
                    RecvTransferOrderValue TransferOrder = _data.GetResult<RecvTransferOrder>().result;

                    TransferPassword.m_Password = TransferOrder.password;
                    TransferPassword.m_TimeLimit = TransferOrder.remaining_time;
                    LocalSaveManager.Instance.SaveFuncTransferPassword(TransferPassword);

                    PasswordDialog(true);
                })
                .setErrorAction(data =>
                {
                    if (data.m_PacketCode == API_CODE.API_CODE_MAKEPASSWORD_INVALID_RANK_ERROR)
                    {
                        //--------------
                        //不正ランクの場合
                        //--------------
                        DialogManager.Open1B("OTHERS_TRANSFER_ERR_RANK_TITLE",
                                             "OTHERS_TRANSFER_ERR_RANK",
                                            "common_button1", true, true).
                        SetOkEvent(() =>
                        {
                        });
                        SoundUtil.PlaySE(SEID.SE_MENU_NG);
                    }
                    else
                    {
                        DialogManager.Open1B("ERROR_MSG_WIDEUSE_TITLE",
                                             "ERROR_MSG_WIDEUSE",
                                             "common_button1", true, true).
                        SetOkEvent(() =>
                        {
                        });
                        SoundUtil.PlaySE(SEID.SE_MENU_NG);

                    }
                })
                .SendStart();
        }
    }

    void PasswordDialog(bool isFirst)
    {
        uint uid = LocalSaveManager.Instance.LoadFuncUserID();
        LocalSaveTransferPassword TransferPassword = LocalSaveManager.Instance.LoadFuncTransferPassword();

        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "he180q_title");
        string detail = "";
        if (isFirst == true)
        {
            detail = GameTextUtil.GetText("he180q_content1") + "\n";
        }
        int dd = TransferPassword.m_TimeLimit % 100;
        int mm = (TransferPassword.m_TimeLimit / 100) % 100;
        int yy = TransferPassword.m_TimeLimit / 10000;
        string yymmdd = yy.ToString("0000") + "/" + mm.ToString("00") + "/" + dd.ToString("00");
        detail = detail + string.Format(GameTextUtil.GetText("he180q_content2"), UnityUtil.CreateDrawUserID(uid), TransferPassword.m_Password, yymmdd);
        newDialog.SetDialogText(DialogTextType.MainText, detail);
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            m_OthersUser.PasswordText = GameTextUtil.GetText("he173p_button2_1");
        });
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    void OnGameDataDelete()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "he177q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "he177q_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            Dialog nextDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
            nextDialog.SetDialogTextFromTextkey(DialogTextType.Title, "he178q_title");
            nextDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "he178q_content");
            nextDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
            nextDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
            nextDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
            {
                SendUserRenew();
            });
            nextDialog.EnableFadePanel();
            nextDialog.DisableCancelButton();
            nextDialog.Show();
        });
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    /// <summary>
    /// API送信：ユーザー削除
    /// </summary>
    void SendUserRenew()
    {
        ServerDataUtilSend.SendPacketAPI_UserRenew(ref m_RenewUUID)
        .setSuccessAction(SendUserRenewSuccess)
        .setErrorAction(SendUserRenewError)
        .SendStart();
    }

    /// <summary>
    /// API結果：成功：ユーザー削除
    /// </summary>
    void SendUserRenewSuccess(ServerApi.ResultData _data)
    {
        //----------------------------------------
        // 情報反映
        //----------------------------------------
#if BUILD_TYPE_DEBUG
        Debug.LogError("UUID Renew : " + m_RenewUUID);
#endif

        //----------------------------------------
        // ローカルセーブを破棄して再構築
        //----------------------------------------
        LocalSaveManager.LocalSaveRenew(false, false);

        //----------------------------------------
        // FoxSDKの仕様による進行不可回避
        //----------------------------------------
#if BUILD_TYPE_DEBUG
        Debug.Log("FoxSDK Safety");
#endif
        LocalSaveManager.Instance.SaveFuncInformationOK(LocalSaveManager.AGREEMENT.FOX_CALLED);

        //----------------------------------------
        // UUID記憶
        //----------------------------------------
        LocalSaveManager.Instance.SaveFuncUUID(m_RenewUUID);
        LocalSaveManager.Instance.SaveFuncTitleUUID();

        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvRenewUser>(UserDataAdmin.Instance.m_StructPlayer);
        UserDataAdmin.Instance.m_StructSystem = _data.GetResult<RecvRenewUser>().result.system;
        UserDataAdmin.Instance.ConvertPartyAssing();

        //----------------------------------------
        // メインメニューパラメータクリア
        //----------------------------------------
        MainMenuParam.ParamReset();
        MainMenuHeader.ParamReset();
        ResidentParam.ParamResetUserRenew();

        //----------------------------------------
        // 共有パラメータクリア
        //----------------------------------------
        if (UserDataAdmin.Instance != null)
        {
            UserDataAdmin.Instance.ParamReset();
        }

        MainMenuManager.s_LastLoginTime = 0;

        // 完了ダイアログ
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "he179q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "he179q_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.GUARD);

            StartCoroutine(SQLiteClient.Instance.LocalSqlite3ClearExec(() =>
            {
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.GUARD);
                //----------------------------------------
                // 初回起動時の動作を想定してタイトルに戻る
                //----------------------------------------
                SceneCommon.Instance.GameToTitle();
            }));
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    /// <summary>
    /// API結果：失敗：ユーザー削除
    /// </summary>
    void SendUserRenewError(ServerApi.ResultData _data)
    {
        if (_data.m_PacketCode == API_CODE.API_CODE_USER_CREATE_UUID_ERR)
        {
            //----------------------------------------
            // UUIDが既に使われている
            // →UUIDの再構築は内部的に行われるので、再度トライしてみる
            //----------------------------------------
            SendUserRenew();

        }
        else
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_MSG_WIDEUSE_TITLE");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_MSG_WIDEUSE");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {

            }));
            newDialog.EnableFadePanel();
            newDialog.DisableCancelButton();
            newDialog.Show();
            SoundUtil.PlaySE(SEID.SE_MENU_NG);
        }
    }

    void OnAchievement()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("Open LeaderBoard");
#endif

        bool bSignedIn = PlayGameServiceUtil.isSignedIn();
        if (bSignedIn == false)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "OTHERS_ACHIEVEMENT_SIGNOUT_TITLE");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "OTHERS_ACHIEVEMENT_SIGNOUT_DETAIL");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.EnableFadePanel();
            newDialog.DisableCancelButton();
            newDialog.Show();
        }
        else
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
            PlayGameServiceUtil.ShowAchievements();
        }
    }

    void OnLeaderboard()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("Open Trophy");
#endif

        bool bSignedIn = PlayGameServiceUtil.isSignedIn();
        if (bSignedIn == false)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "OTHERS_ACHIEVEMENT_SIGNOUT_TITLE");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "OTHERS_ACHIEVEMENT_SIGNOUT_DETAIL");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.EnableFadePanel();
            newDialog.DisableCancelButton();
            newDialog.Show();
        }
        else
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
            PlayGameServiceUtil.ShowAllLeaderBoard();
        }
    }
}
