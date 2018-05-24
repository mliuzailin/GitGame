using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuFriendsListWaitMe : MainMenuSeq
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
    /// フレンドリスト更新（申請受けリスト）
    /// </summary>
	private void updateFriendList()
    {
        MainMenuUtil.CreateFriendList(ref m_FriendList, FRIEND_STATE.FRIEND_STATE_WAIT_ME, SelectIcon, SelectFriend);
        m_FriendList.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterFriendWaitMe());
        m_FriendList.OnClickSortButtonAction = OnClockSortButton;
        m_FriendList.Init();

        updateCount();
    }

    private void updateCount()
    {
        string valueFormat = GameTextUtil.GetText("fr151p_text");
        uint total = GlobalDefine.FRIEND_MAX_WAIT_ME;
        int now = m_FriendList.FriendBaseList.Body.Count;
        m_FriendList.ParamValue = string.Format(valueFormat, now, total);
    }

    /// <summary>
    /// キャラクタ詳細画面
    /// </summary>
    /// <param name="_friend"></param>
    private void SelectIcon(FriendDataItem _friend)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        if (MainMenuManager.HasInstance) MainMenuManager.Instance.OpenUnitDetailInfoFriend(_friend.FriendData);
    }

    /// <summary>
    /// フレンド申請選択ダイアログ
    /// </summary>
    /// <param name="_friend"></param>
    private void SelectFriend(FriendDataItem _friend)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        m_SelectFriend = _friend;

        Dialog _newDialog = Dialog.Create(DialogType.DialogFriend).SetStrongYes();
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr156q_title");
        _newDialog.SetFriendInfo(m_SelectFriend.FriendData);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
        string underFormat = GameTextUtil.GetText("fr156q_content");
        _newDialog.SetDialogText(DialogTextType.UnderText, string.Format(underFormat, m_SelectFriend.FriendData.user_name));
        _newDialog.SetMenuInButton(true);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.InButtonText, "fr156q_button2");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            sendFriendConsent();
        });
        _newDialog.SetDialogEvent(DialogButtonEventType.INBUTTON, () =>
        {
            sendFriendRequestCancel();
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();
    }

    /// <summary>
    /// フレンド申請キャンセル通信
    /// </summary>
	private void sendFriendRequestCancel()
    {
        uint[] selectFriendArray = new uint[1];
        selectFriendArray[0] = m_SelectFriend.FriendData.user_id;
        ServerDataUtilSend.SendPacketAPI_FriendRefusal(selectFriendArray)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendRefusal>().result.friend);

            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr159q_title");
            string mainFormat = GameTextUtil.GetText("fr159q_content");
            _newDialog.SetDialogText(DialogTextType.MainText, string.Format(mainFormat, m_SelectFriend.FriendData.user_name));
            _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
                MainMenuManager.Instance.SubTab.updateTabItem();

                updateFriendList();
            });
            _newDialog.Show();

        })
        .setErrorAction(_data =>
        {
            MainMenuUtil.openFriendRequestErrorDialog(_data.m_PacketCode);
        })
        .SendStart();
    }

    /// <summary>
    /// フレンド申請受諾通信
    /// </summary>
	private void sendFriendConsent()
    {
        uint[] selectFriendArray = new uint[1];
        selectFriendArray[0] = m_SelectFriend.FriendData.user_id;

        ServerDataUtilSend.SendPacketAPI_FriendConsent(selectFriendArray)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendConsent>().result.friend);

            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr157q_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "fr157q_content");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
                MainMenuManager.Instance.SubTab.updateTabItem();

                updateFriendList();

                StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
            });
            _newDialog.Show();
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
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterFriendWaitMe());
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
        LocalSaveManager.Instance.SaveFuncSortFilterFriendWaitMe(sortInfo);
        m_FriendList.ExecSort(sortInfo);
    }
}
