using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMenuForMainMenu : GlobalMenu
{
    protected override void SetupTopMenu(Camera _camera)
    {
        base.SetupTopMenu(_camera);

        addMenuList(GlobalMenuButtonCategory.User, GlobalMenuButtonType.Infomation, "インフォメーション", "GlobalMenu_info");
        addMenuList(GlobalMenuButtonCategory.User, GlobalMenuButtonType.UserInfo, "ユーザー情報", "yu-zajyouhou");
        addMenuList(GlobalMenuButtonCategory.User, GlobalMenuButtonType.Friend, "フレンド", "friend");

        addMenuList(GlobalMenuButtonCategory.Game, GlobalMenuButtonType.Item, "アイテム", "item");
        addMenuList(GlobalMenuButtonCategory.Game, GlobalMenuButtonType.EventSchedule, "イベントスケジュール", "event");
        addMenuList(GlobalMenuButtonCategory.Game, GlobalMenuButtonType.Mission, "ミッション", "mission");
        addMenuList(GlobalMenuButtonCategory.Game, GlobalMenuButtonType.Present, "プレゼント", "present");

        addMenuList(GlobalMenuButtonCategory.Other, GlobalMenuButtonType.GameHelp, "お助け情報", "otasuke");
        addMenuList(GlobalMenuButtonCategory.Other, GlobalMenuButtonType.Catalog, "図鑑", "zukan");
        addMenuList(GlobalMenuButtonCategory.Other, GlobalMenuButtonType.Web, "WEB", "web");
        addMenuList(GlobalMenuButtonCategory.Other, GlobalMenuButtonType.TOS, "利用規約", "riyoukiyaku");
        addMenuList(GlobalMenuButtonCategory.Other, GlobalMenuButtonType.Movie, "ムービー", "mu-bi-");
        addMenuList(GlobalMenuButtonCategory.Other, GlobalMenuButtonType.Support, "お問い合わせ", "otoiawase");
        addMenuList(GlobalMenuButtonCategory.Other, GlobalMenuButtonType.Option, "オプション", "option");
        addMenuList(GlobalMenuButtonCategory.Other, GlobalMenuButtonType.Logout, "ログアウト", "power");

        updateMenuItem();
    }

    public void updateMenuItem()
    {
        bool bFriend = UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalFriend);
        getMenuItem(GlobalMenuButtonCategory.User, GlobalMenuButtonType.Friend).IsActiveFlag = bFriend;

        bool bPresent = UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalPresent);
        getMenuItem(GlobalMenuButtonCategory.Game, GlobalMenuButtonType.Present).IsActiveFlag = bPresent;

        bool bMissionFlag = false;
        if (UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionDaily) ||
            UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionEvent) ||
            UserDataAdmin.Instance.GetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionNormal))
        {
            bMissionFlag = true;
        }
        getMenuItem(GlobalMenuButtonCategory.Game, GlobalMenuButtonType.Mission).IsActiveFlag = bMissionFlag;
        if (bFriend || bPresent || bMissionFlag) m_Context.IsActiveFlag = true;
    }

    public override void OnPushMenu(GlobalMenuButtonType _buttonType)
    {
        base.OnPushMenu(_buttonType);

        switch (_buttonType)
        {
            case GlobalMenuButtonType.Infomation:
                openInfomationDialog();
                break;
            case GlobalMenuButtonType.UserInfo:
                PageSwitchMain(MAINMENU_SEQ.SEQ_OTHERS_USER);
                break;
            case GlobalMenuButtonType.Friend:
                PageSwitchMain(MAINMENU_SEQ.SEQ_FRIEND_LIST);
                break;
            case GlobalMenuButtonType.Item:
                PageSwitch(GLOBALMENU_SEQ.ITEM);
                break;
            case GlobalMenuButtonType.EventSchedule:
                PageSwitch(GLOBALMENU_SEQ.EVENTSCHEDULE);
                break;
            case GlobalMenuButtonType.Mission:
                PageSwitch(GLOBALMENU_SEQ.MISSION);
                break;
            case GlobalMenuButtonType.Present:
                PageSwitch(GLOBALMENU_SEQ.PRESENT);
                break;
            case GlobalMenuButtonType.GameHelp:
                PageSwitchMain(MAINMENU_SEQ.SEQ_OTHERS_HELP);
                break;
            case GlobalMenuButtonType.Catalog:
                PageSwitchMain(MAINMENU_SEQ.SEQ_UNIT_CATALOG);
                break;
            case GlobalMenuButtonType.Web:
                PageSwitchMain(MAINMENU_SEQ.SEQ_OTHERS_WEB);
                break;
            case GlobalMenuButtonType.TOS:
                PageSwitchMain(MAINMENU_SEQ.SEQ_OTHERS_KIYAKU);
                break;
            case GlobalMenuButtonType.Movie:
                PageSwitchMain(MAINMENU_SEQ.SEQ_OTHERS_MOVIE);
                break;
            case GlobalMenuButtonType.Support:
                openSupportDialog();
                break;
            case GlobalMenuButtonType.Option:
                PageSwitch(GLOBALMENU_SEQ.OPTION);
                break;
            case GlobalMenuButtonType.Logout:
                {
                    openReturnTitleDialog();
                }
                break;
            default:
                break;
        }
    }

    public override void OnPushClose()
    {
        base.OnPushClose();

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        if (MainMenuManager.HasInstance) MainMenuManager.Instance.CloseGlobalMenu();
    }

    public override void OnPushReturn()
    {
        base.OnPushReturn();

        updateMenuItem();

        BackToTop();

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
    }

    private void BackToTop()
    {
        m_UIView.BackToTop(() =>
        {
            ResetButtons();
            ShowButtons();
        });
    }


    private void openInfomationDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogScrollInfo);
        newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("mm31q_title"));
        string _message = MasterDataUtil.GetInformationMessage(MasterDataDefineLabel.InfomationType.NORMAL, false);
        newDialog.AddScrollInfoText(_message);
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.DisableCancelButton();
        newDialog.Show();

        newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            ResetButtons();
        });
    }

    private void openSupportDialog()
    {
        uint uid = LocalSaveManager.Instance.LoadFuncUserID();
        string userIdText = string.Format(GameTextUtil.GetText("mt21q_content"), UnityUtil.CreateDrawUserID(uid));
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mt21q_title");
        newDialog.SetDialogText(DialogTextType.MainText, userIdText);
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            ResetButtons();
            //DG0-1944　お問い合わせは強制的に外部ブラウザーで表示する
            string support_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_SUPPORT);
            URLManager.OpenURL(support_url, true);
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();

        newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
        {
            ResetButtons();
        });
    }

    private void openReturnTitleDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mm55q_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "mm55q_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            MainMenuManager.Instance.UpdateUserStatusFromGlobalMenu(GLOBALMENU_SEQ.TITLE);

            AndroidBackKeyManager.Instance.StackClear();
            SceneCommon.Instance.GameToTitle();
        });
        newDialog.DisableCancelButton();
        newDialog.Show();

        newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
        {
            ResetButtons();
        });
    }

    private void PageSwitchMain(MAINMENU_SEQ _seq)
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.AddSwitchRequest(_seq, false, false);
            MainMenuManager.Instance.CloseGlobalMenu();
        }
    }
}
