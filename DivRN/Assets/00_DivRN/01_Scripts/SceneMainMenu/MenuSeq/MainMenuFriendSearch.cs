using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuFriendSearch : MainMenuSeq
{

    private FriendSearch m_FriendSearch = null;
    private uint m_SearchUserId = 0;

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

        if (m_FriendSearch == null)
        {
            m_FriendSearch = m_CanvasObj.GetComponentInChildren<FriendSearch>();
            m_FriendSearch.SetPositionAjustStatusBar(new Vector2(0, -258));

            //Self UserId
            uint userId = LocalSaveManager.Instance.LoadFuncUserID();
            m_FriendSearch.SelfIDText = UnityUtil.CreateDrawUserID(userId);

            //
            m_FriendSearch.DidSearchFriend = SearchFriend;

        }

        resetSearchUserId();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.FRIEND;
    }

    public void resetSearchUserId()
    {
        m_SearchUserId = 0;
        m_FriendSearch.resetInputField();
    }

    public void SearchFriend(string _userId)
    {
        //----------------------------------------
        // 入力されているIDを算出。
        // IDが数値でないならエラー
        //----------------------------------------
        m_SearchUserId = UnityUtil.CreateFriendUserID(_userId);
        if (m_SearchUserId != 0)
        {
            if (m_SearchUserId == UserDataAdmin.Instance.m_StructPlayer.user.user_id)
            {

                Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
                _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr163q_title");
                _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "fr163q_content4");
                _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
                _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
                 {
                     resetSearchUserId();
                 });
                _newDialog.EnableFadePanel();
                _newDialog.Show();

                SoundUtil.PlaySE(SEID.SE_MENU_OK);
            }
            else
            {
                ServerDataUtilSend.SendPacketAPI_FriendSearch(m_SearchUserId)
                .setSuccessAction(_data =>
                {
                    requestSuccess(_data.GetResult<RecvFriendSearch>().result.friend);
                })
                .setErrorAction(_data =>
                {
                    MainMenuUtil.openFriendRequestErrorDialog(_data.m_PacketCode, resetSearchUserId);
                })
                .SendStart();
                SoundUtil.PlaySE(SEID.SE_MENU_OK);
            }
        }
        else
        {
            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr163q_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "fr163q_content3");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button6");
            _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
                {
                });
            _newDialog.EnableFadePanel();
            _newDialog.Show();

            SoundUtil.PlaySE(SEID.SE_MENU_OK);
        }
    }

    private void sendFriendRequest()
    {
        uint[] useridArray = new uint[1];
        useridArray[0] = m_SearchUserId;
        ServerDataUtilSend.SendPacketAPI_FriendRequest(useridArray)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructFriendList = UserDataAdmin.FriendListClipNotExist(_data.GetResult<RecvFriendRequest>().result.friend);
            // すでにお気に入り登録しているフレンドの可能性があるので削除しておく
            LocalSaveManager.Instance.SaveFuncAddFavoriteFriend(useridArray[0], false, true);

            MainMenuManager.Instance.SubTab.updateTabItem();

            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr164q_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "fr164q_content");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            _newDialog.EnableFadePanel();
            _newDialog.Show();

            resetSearchUserId();
        })
        .setErrorAction(_data =>
        {
            MainMenuUtil.openFriendRequestErrorDialog(_data.m_PacketCode, resetSearchUserId);
        })
        .SendStart();
    }

    private void requestSuccess(PacketStructFriend friend)
    {
        Dialog _newDialog = Dialog.Create(DialogType.DialogFriend).SetStrongYes();
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "fr163q_title");
        _newDialog.SetFriendInfo(friend);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.UnderText, "fr163q_content");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            sendFriendRequest();
        });
        _newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
        {
            resetSearchUserId();
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();
    }

}
