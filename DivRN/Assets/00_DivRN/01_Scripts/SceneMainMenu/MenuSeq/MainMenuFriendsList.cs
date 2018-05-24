/**
 *  @file   MainMenuFriendsList.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/07
 */

using UnityEngine;
using System;
using System.Collections;
using ServerDataDefine;
using System.Collections.Generic;

public class MainMenuFriendsList : MainMenuSeq
{
    const int NAME_LENGTH = 4;

    FriendList m_FriendList;
    FriendDataItem m_SelectFriend = null;

    protected override void Start()
    {
        base.Start();
    }

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

        // TODO: ページ解説テキスト設定

        // TODO: タイトル設定

        if (m_FriendList == null)
        {
            m_FriendList = m_CanvasObj.GetComponentInChildren<FriendList>();
            m_FriendList.SetPositionAjustStatusBar(new Vector2(0, -30), new Vector2(-60, -456));
            m_FriendList.IsViewParam = true;
            m_FriendList.CheckLock = true;//お気に入りチェックON
        }

        CreateFriendList(); // リストの作成

        updateFriendCount(); // フレンド数・最大数更新

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.FRIEND;
    }

    /// <summary>
    /// フレンドリストの情報作成
    /// </summary>
    void CreateFriendList()
    {
        MainMenuUtil.CreateFriendList(ref m_FriendList, FRIEND_STATE.FRIEND_STATE_SUCCESS, SelectIcon, SelectFriend);
        m_FriendList.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterFriendList());
        m_FriendList.OnClickSortButtonAction = OnClockSortButton;
        m_FriendList.Init();

        updateFriendCount();
    }

    public void updateFriendCount()
    {
        string valueFormat = GameTextUtil.GetText("fr151p_text");
        uint total = UserDataAdmin.Instance.m_StructPlayer.total_friend;
        int now = m_FriendList.FriendBaseList.Body.Count;
        m_FriendList.ParamValue = string.Format(valueFormat, now, total);
    }

    #region API通信
    /// <summary>
    /// API開始：フレンド一覧取得
    /// </summary>
    void SendGetFriendList()
    {
        ServerDataUtilSend.SendPacketAPI_FriendListGet().setSuccessAction(ReceiveGetFriendListSuccess).setErrorAction(ReceiveGetFriendListError).SendStart();
    }

    /// <summary>
    /// API終了：フレンド一覧取得失敗
    /// </summary>
    void ReceiveGetFriendListError(ServerApi.ResultData data)
    {
        DialogManager.Open1B("ERROR_MSG_WIDEUSE_TITLE", "ERROR_MSG_WIDEUSE", "common_button7", true, true);
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_MSG_WIDEUSE_TITLE");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_MSG_WIDEUSE");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
    }

    /// <summary>
    /// API終了：フレンド一覧取得成功
    /// </summary>
    void ReceiveGetFriendListSuccess(ServerApi.ResultData data)
    {
        PacketStructFriend[] friendList = data.GetResult<RecvFriendListGet>().result.friend;
        if (!friendList.IsNullOrEmpty())
        {
            // ユーザーデータ情報を更新
            UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(friendList);
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_FRIEND_LIST, false, false); // ページ再表示
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        }
    }
    #endregion

    public void OnClick()
    {
        SendGetFriendList();
    }


    /// <summary>
    /// ソートボタンが押されたとき
    /// </summary>
    public void OnClickSortButton()
    {
        Debug.Assert(false, "This function is deprecated.");
    }

    /// <summary>
    /// UPDATEボタンが押されたとき
    /// </summary>
    public void OnClickFixButton()
    {

    }

    /// <summary>
    /// ダイアログのソートボタンが押されたとき
    /// </summary>
    /// <param name="sortType"></param>
    public void OnClickSortItem(MAINMENU_SORT_SEQ sortType)
    {
    }
    private void SelectIcon(FriendDataItem _friend)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        if (MainMenuManager.HasInstance) MainMenuManager.Instance.OpenUnitDetailInfoFriend(_friend.FriendData);
    }

    /// <summary>
    /// フレンドダイアログ
    /// </summary>
    /// <param name="_friend"></param>
    private void SelectFriend(FriendDataItem _friend)
    {
        m_SelectFriend = _friend;

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        Dialog _newDialog = Dialog.Create(DialogType.DialogFriend).SetStrongYes();
        _newDialog.SetFriendInfo(m_SelectFriend.FriendData, true);
        _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
        if (MainMenuUtil.ChkFavoridFriend(m_SelectFriend.FriendData.user_id))
        {
            _newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("fr153q_title"));
            string underFormat = GameTextUtil.GetText("fr153q_content");
            _newDialog.SetDialogText(DialogTextType.UnderText, string.Format(underFormat, m_SelectFriend.FriendData.user_name));
        }
        else
        {
            _newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("fr152q_title"));
            string underFormat = GameTextUtil.GetText("fr152q_content");
            _newDialog.SetDialogText(DialogTextType.UnderText, string.Format(underFormat, m_SelectFriend.FriendData.user_name));
            _newDialog.SetMenuInButton(true);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.InButtonText, "fr_button");
        }
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            //お気に入り変更
            if (MainMenuUtil.ChkFavoridFriend(m_SelectFriend.FriendData.user_id))
            {
                //----------------------------------------
                // お気に入りから除外
                //----------------------------------------
                LocalSaveManager.Instance.SaveFuncAddFavoriteFriend(m_SelectFriend.FriendData.user_id, false, true);
                m_SelectFriend.IsActiveLock = false;
            }
            else
            {
                //----------------------------------------
                // お気に入りに追加登録
                //----------------------------------------
                LocalSaveManager.Instance.SaveFuncAddFavoriteFriend(m_SelectFriend.FriendData.user_id, true, false);
                m_SelectFriend.IsActiveLock = true;
            }
        });
        _newDialog.SetDialogEvent(DialogButtonEventType.INBUTTON, () =>
        {
            // 
            //if (MainMenuUtil.ChkFavoridFriend(m_SelectFriend.FriendData.user_id))
            //{
            //	//お気に入りフレンドを削除しているむねを表示
            //	openWarningDialog();
            //}else
            {
                //削除申請
                openWarningDialog();
            }
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();

    }

    /// <summary>
    /// 警告ダイアログ
    /// </summary>
	private void openWarningDialog()
    {
        Dialog _warningDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        _warningDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr155q_title");
        string underFormat = GameTextUtil.GetText("fr155q_content");
        _warningDialog.SetDialogText(DialogTextType.MainText, string.Format(underFormat, m_SelectFriend.FriendData.user_name));
        _warningDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _warningDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _warningDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            requestFriendRefusal();
        });
        _warningDialog.EnableFadePanel();
        _warningDialog.Show();
    }

    /// <summary>
    /// フレンド解除通信
    /// </summary>
	private void requestFriendRefusal()
    {
        uint[] aunFriendIDList = new uint[1];
        aunFriendIDList[0] = m_SelectFriend.FriendData.user_id;

        ServerDataUtilSend.SendPacketAPI_FriendRefusal(aunFriendIDList)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendRefusal>().result.friend);

            CreateFriendList();
        })
        .setErrorAction(_data =>
        {
            MainMenuUtil.openFriendRequestErrorDialog(_data.m_PacketCode);
        })
        .SendStart();
    }

    /// <summary>
    /// ソートダイアログを開く
    /// </summary>
    void OnClockSortButton()
    {
        if (SortDialog.IsExists == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        SortDialog dialog = SortDialog.Create();
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.FRIEND);
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterFriendList());
        dialog.OnCloseAction = OnClickSortCloseButton;
    }

    /// <summary>
    /// ソートダイアログを閉じたとき
    /// </summary>
    void OnClickSortCloseButton(LocalSaveSortInfo sortInfo)
    {
        //--------------------------------
        // データ保存
        //--------------------------------
        LocalSaveManager.Instance.SaveFuncSortFilterFriendList(sortInfo);
        m_FriendList.ExecSort(sortInfo);
    }
}