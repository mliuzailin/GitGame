﻿/**
 *  @file   MainMenuOthersRename.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/10
 */

using UnityEngine;
using System.Collections;
using ServerDataDefine;

public class MainMenuTutorialName : MainMenuSeq
{
    OthersRename m_OthersRename;
    string m_InitName = "";
    string m_RenameStr;
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

        m_OthersRename = GetComponentInChildren<OthersRename>();
        m_OthersRename.SetPositionAjustStatusBar(new Vector2(0, -180));
        m_OthersRename.SetName(m_InitName);
        m_OthersRename.OnClickAction = OnClickButton;
        m_OthersRename.OnEndEditAction = OnEndEdit;
        m_OthersRename.LabelText = GameTextUtil.GetText("name_title");
        m_OthersRename.NameDetailText = GameTextUtil.GetText("he174p_text2");
    }

    void OnClickButton(string name)
    {
        if (m_InitName == name)
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "he174q_title");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "he174q_content");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.EnableFadePanel();
            newDialog.DisableCancelButton();
            newDialog.Show();
        }
        else
        {
            m_RenameStr = name;
            SendRenameUser();
        }
    }

    void OnEndEdit(string text)
    {
        if (text.IsNullOrEmpty())
        {
            // 入力を戻す
            m_OthersRename.SetName(m_InitName);
        }
    }

    /// <summary>
    /// API送信：ユーザー名変更
    /// </summary>
    void SendRenameUser()
    {
        ServerDataUtilSend.SendPacketAPI_RenameUser(m_RenameStr)
        .setSuccessAction(_data =>
        {
            if (UserDataAdmin.Instance.m_StructPlayer != null)
            {
                UserDataAdmin.Instance.m_StructPlayer.user.user_name = _data.GetResult<RecvRenameUser>().result.user_name;
                UserDataAdmin.Instance.Player.User_name = UserDataAdmin.Instance.m_StructPlayer.user.user_name;
                Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "he175q_title");
                newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "he175q_content");
                newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
                {

                    if (TutorialManager.IsExists)
                    {
                        TutorialFSM.Instance.SendFsmNextEvent();
                    }

                }));
                newDialog.EnableFadePanel();
                newDialog.DisableCancelButton();
                newDialog.Show();
            }
        })
        .setErrorAction(SendRenameUserError)
        .SendStart();
    }

    /// <summary>
    /// API結果：失敗：ユーザー名変更
    /// </summary>
    void SendRenameUserError(ServerApi.ResultData _data)
    {
        if (_data.m_PacketCode != API_CODE.API_CODE_USER_RENAME_NG_WORD)
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "he176q_title");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "he176q_content");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
            }));
            newDialog.EnableFadePanel();
            newDialog.DisableCancelButton();
            newDialog.Show();
            SoundUtil.PlaySE(SEID.SE_MENU_NG);
        }

        m_OthersRename.SetName(m_InitName); // 入力を戻す
    }
}
