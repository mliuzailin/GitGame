using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

/// <summary>
/// 
/// </summary>
public class MainMenuFriendsListWaitHim : MainMenuSeq
{

    private FriendList m_FriendList = null;
    private FriendDataItem m_SelectFriend = null;

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

        if (m_FriendList == null)
        {
            //フレンドリスト取得
            m_FriendList = m_CanvasObj.GetComponentInChildren<FriendList>();
            m_FriendList.SetPositionAjustStatusBar(new Vector2(0, -30), new Vector2(-60, -456));
            m_FriendList.IsViewParam = true;
        }

        updateFriendList();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.FRIEND;
    }

    /// <summary>
    /// フレンドリスト更新（フレンド申し込みリスト）
    /// </summary>
	private void updateFriendList()
    {
        MainMenuUtil.CreateFriendList(ref m_FriendList, FRIEND_STATE.FRIEND_STATE_WAIT_HIM, SelectIcon, SelectFriend);
        m_FriendList.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterFriendWaitHim());
        m_FriendList.OnClickSortButtonAction = OnClockSortButton;
        m_FriendList.Init();

        //
        updateCount();
    }

    private void updateCount()
    {
        string valueFormat = GameTextUtil.GetText("fr151p_text");
        uint total = GlobalDefine.FRIEND_MAX_WAIT_HIM;
        int now = m_FriendList.FriendBaseList.Body.Count;
        m_FriendList.ParamValue = string.Format(valueFormat, now, total);
    }

    /// <summary>
    /// フレンド申請申し込みキャンセルダイアログ
    /// </summary>
    /// <param name="_friend"></param>
	public void SelectFriend(FriendDataItem _friend)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        m_SelectFriend = _friend;

        Dialog _newDialog = Dialog.Create(DialogType.DialogFriend).SetStrongYes();
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr161q_title");
        _newDialog.SetFriendInfo(m_SelectFriend.FriendData);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.UnderText, "fr161q_content");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            sendFriendRequestCancel();
        });
        _newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
        {
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();

    }

    /// <summary>
    /// キャラクタ詳細画面
    /// </summary>
    /// <param name="_friend"></param>
	public void SelectIcon(FriendDataItem _friend)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        if (MainMenuManager.HasInstance) MainMenuManager.Instance.OpenUnitDetailInfoFriend(_friend.FriendData);
    }

    /// <summary>
    /// フレンド申請申し込みキャンセル通信
    /// </summary>
	private void sendFriendRequestCancel()
    {
        uint[] selectFriendArray = new uint[1];
        selectFriendArray[0] = m_SelectFriend.FriendData.user_id;
        ServerDataUtilSend.SendPacketAPI_FriendRefusal(selectFriendArray)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendRefusal>().result.friend);

            MainMenuManager.Instance.SubTab.updateTabItem();

            updateFriendList();
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
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterFriendWaitHim());
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
        LocalSaveManager.Instance.SaveFuncSortFilterFriendWaitHim(sortInfo);
        m_FriendList.ExecSort(sortInfo);
    }
}
