using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerDataDefine;

public class ScratchListItem : ListItem<ScratchListItemContext>
{
    [SerializeField]
    Image m_BoadImage = null;
    [SerializeField]
    ScratchStepButton m_StepButton = null;

    public enum ButtonId : int
    {
        Close = 0,
        LineUp_A = 10,  // ラインナップ 前半
        LineUp_B = 11,  // ラインナップ 後半
        LineUp_Rainbow = 12,  // ラインナップ 確定
        LineUp_Normal = 13,  // ラインナップ 通常
        ScratchOne = 20,  // 1回ガチャ
        ScratchMax = 21, // 最大9回ガチャ
        ScratchStepUp = 22, // ステップアップガチャ
        //GideLine = 500, // ガイドライン
        Info = 999,     // 詳細
        End
    }

    private void Awake()
    {
        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
        // ページ切り替え用トグルの設定
        Context.Toggle = GetComponent<Toggle>();
        ToggleGroup toggleGroup = GetComponentInParent<ToggleGroup>();
        if (toggleGroup != null)
        {
            Context.Toggle.group = toggleGroup;
        }

        Context.m_StepButton = m_StepButton;

        // コールバック設定
        Scratch scratch = GetComponentInParent<Scratch>();
        if (scratch != null)
        {
            Context.Toggle.onValueChanged.AddListener(scratch.OnChangedForm);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Context.Toggle.isOn == false)
        {
            return;
        }

        if (!UserDataAdmin.HasInstance)
        {
            return;
        }

        if (UserDataAdmin.Instance.IsUpdateUserData &&
            Context.m_IsMoveScratchResult == false)
        {
            Context.updatePointText();

            Context.updateScratchButton();
        }
    }

    void SetUpButtons()
    {
        var stepButtonModel = new ButtonModel();
        m_StepButton.SetModel(stepButtonModel);
        stepButtonModel.OnClicked += () =>
        {
            OnClickedButton((int)ButtonId.ScratchStepUp);
        };

        stepButtonModel.Appear();
        stepButtonModel.SkipAppearing();
    }

    // クリック時のフィードバック
    public void OnClickedButton(int buttonId)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClickedButton" + buttonId);
#endif

        Context.uodateGachaRainbowDecide();

        switch ((ButtonId)buttonId)
        {
            case ButtonId.Close:
                //Destroy(gameObject);
                break;

            case ButtonId.LineUp_A:
                OnLineUp(Scratch.LineUp.A);   // LineUp 1　前半ラインナップ
                break;
            case ButtonId.LineUp_B:
                OnLineUp(Scratch.LineUp.B);   // LineUp 2　後半ラインナップ
                break;
            case ButtonId.LineUp_Rainbow:
                OnLineUp(Scratch.LineUp.Rainbow);   // LineUp 3　確定ラインナップ
                break;
            case ButtonId.LineUp_Normal:
                OnLineUp(Scratch.LineUp.Normal);   // LineUp 3　通常ラインナップ
                break;

            case ButtonId.ScratchOne:
                OnScratch(ButtonId.ScratchOne);
                break;
            case ButtonId.ScratchMax:
                OnScratch(ButtonId.ScratchMax);
                break;
            case ButtonId.ScratchStepUp:
                OnScratch(ButtonId.ScratchStepUp);
                break;
            case ButtonId.Info:
                openInfomationDialog();
                break;

            default:
                break;
        }
    }

    // ダイアログ（特殊） ガチャラインナップ
    private void OnLineUp(Scratch.LineUp _type)
    {
        if (TutorialManager.IsExists)
        {
            MasterDataGacha isMaster = MasterFinder<MasterDataGacha>.Instance.Find((int)Context.lineupMaster[(int)_type]);
            if (isMaster == null)
            {
                return;
            }

            if (isMaster.fix_id != GlobalDefine.TUTORIAL_SCRATCH ||
                isMaster.gacha_group_id == 0)
            {
                return;
            }
        }

        string title = "";
        switch (_type)
        {
            case Scratch.LineUp.A:
                title = GameTextUtil.GetText("gacha_lineup_title1") + GameTextUtil.GetText("gacha_lineup_half1");
                break;
            case Scratch.LineUp.B:
                title = GameTextUtil.GetText("gacha_lineup_title1") + GameTextUtil.GetText("gacha_lineup_half2");
                break;
            case Scratch.LineUp.Normal:
                title = GameTextUtil.GetText("gacha_lineup_title1");
                break;
            case Scratch.LineUp.Rainbow:
                title = GameTextUtil.GetText("gacha_lineup_title2");
                break;
            default:
                break;
        }

        MasterDataStepUpGachaManage stepUpGachaManage = null;
        MasterDataGacha master = null;

        if (Context.gachaMaster.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            stepUpGachaManage = MasterDataUtil.GetCurrentStepUpGachaManageMaster(Context.gachaMaster.fix_id);
            master = Context.gachaMaster;
            // タイトルにステップ数追加
            if (stepUpGachaManage.step_num == 0)
            {
                // 初回ステップ
                title += GameTextUtil.GetText("gacha_lineup_step1");
            }
            else
            {
                title += string.Format(GameTextUtil.GetText("gacha_lineup_step2"), stepUpGachaManage.step_num);
            }
        }
        else
        {
            master = MasterFinder<MasterDataGacha>.Instance.Find((int)Context.lineupMaster[(int)_type]);
        }

        if (master == null)
        {
            return;
        }

        ScratchLineUpDialog lineupdialog = ScratchLineUpDialog.Create();
        lineupdialog.SetCamera(SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>());
        lineupdialog.setup(master, stepUpGachaManage, Context.lineupMaster[(int)_type], Context.IsUsedTip, title);

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    /// <summary>
    /// ダイアログ　ガチャ詳細
    /// </summary>
    public void openInfomationDialog()
    {
        if (TutorialManager.IsExists)
        {
            if (Context.detailText.IsNullOrEmpty())
            {
                return;
            }
        }

        if (Context.gachaMaster.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            StepUpScratchDialog newdialog = StepUpScratchDialog.Create();
            newdialog.SetGachaID(Context.gachaMaster.fix_id);
            newdialog.Show();
        }
        else
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
            newDialog.SetDialogText(DialogTextType.Title, "詳細");
            newDialog.SetDialogText(DialogTextType.MainText, Context.detailText);
            newDialog.SetDialogObjectEnabled(DialogObjectType.OneButton, true);
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.DisableCancelButton();
            newDialog.Show();
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    public void OnScratch(ButtonId _buttonId)
    {
        if (MainMenuManager.Instance.IsPageSwitch() ||
            ServerApi.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (TutorialManager.IsExists ||
            MasterDataUtil.IsFirstTimeFree(Context.gachaMaster))
        {
            // チュートリアル＆
            // 初回オタメシは確認ダイアログなし
            SendScratch(1);
            return;
        }

        //期間チェック
        if (!MasterDataUtil.CheckActiveGachaMaster(Context.gachaMaster))
        {
            openWarningTermDialog();
            return;
        }

        //引ける回数チェック
        int _count = (int)MasterDataUtil.GetGachaCountFromMaster(Context.gachaMaster);
        if (_count == 0)
        {
            openWarningLowDialog();
            return;
        }

        if (_buttonId == ButtonId.ScratchOne)
        {
            //単発
            openScratchDialog(1);
        }
        else
        {
            //連続
            openScratchDialog(_count);
        }
    }

#if BUILD_TYPE_DEBUG
    private void openDebugScrachDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        newDialog.SetDialogText(DialogTextType.Title, "DebugScrach");
        newDialog.SetDialogText(DialogTextType.MainText, "デバッグ用の機能です\n" +
                                                         "１枚引くを連続で２回送信します。");

        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");

        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            int count = 1;
            int tutorial = (TutorialManager.IsExists) ? 1 : 0;

            SendScratch(count);

#if BUILD_TYPE_DEBUG
            Debug.Log("Scratch Debug Send");
#endif
            var serverApi = ServerDataUtilSend.SendPacketAPI_GachaPlay(Context.gachaMaster.fix_id, (uint)count, (uint)tutorial);

            // SendStartの成功時の振る舞い
            serverApi.setSuccessAction(_data =>
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("Scratch Debug Send Success");
#endif
                UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvGachaPlay>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                UserDataAdmin.Instance.ConvertPartyAssing();
            });
            // SendStartの失敗時の振る舞い
            serverApi.setErrorAction(_date =>
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("Scratch Debug Send ng");
#endif
            });

            serverApi.SendStart();

        }));
        newDialog.DisableCancelButton();
        newDialog.Show();
    }
#endif

    /// <summary>
    /// ダイアログ　ガチャ実行
    /// </summary>
    public void openScratchDialog(int count)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        newDialog.SetDialogText(DialogTextType.Title, Context.gachaMaster.name);
        string mainText = GameTextUtil.GetGachaPlayText(count, Context.gachaMaster);
        newDialog.SetDialogText(DialogTextType.MainText, mainText);
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");

        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            SendScratch(count);   // ガチャ(最大)9回実行
        }));
#if BUILD_TYPE_DEBUG
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            openDebugScrachDialog();
        }));
#endif
        //チップを使用するガチャのみ規約関連のボタン表示
        if (Context.gachaMaster.type == MasterDataDefineLabel.GachaType.RARE ||
            Context.gachaMaster.type == MasterDataDefineLabel.GachaType.EVENT)
        {
            newDialog.setupUnderKiyaku();
        }

        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    /// <summary>
    /// ダイアログ　不足警告
    /// </summary>
    public void openWarningLowDialog()
    {
        if (Context.IsUsedTip)
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "sc148q_title");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "sc148q_content");
            newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
            newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
            {
                //チップ購入ダイアログ
                StoreDialogManager.Instance.OpenBuyStone();
            });
            newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
            newDialog.DisableCancelButton();
            newDialog.Show();
        }
        else
        {
            string pname = "";
            switch (Context.gachaMaster.type)
            {
                case MasterDataDefineLabel.GachaType.NORMAL:
                    pname = GameTextUtil.GetText("common_text1");
                    break;
                case MasterDataDefineLabel.GachaType.ITEM_POINT:
                    //pname = GameTextUtil.GetText("common_text5");
                    pname = MasterDataUtil.GetItemNameFromGachaMaster(Context.gachaMaster);
                    break;
            }
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            string titleFormat = GameTextUtil.GetText("sc148q_title2");
            newDialog.SetDialogText(DialogTextType.Title, string.Format(titleFormat, pname));
            string mainFormat = GameTextUtil.GetText("sc148q_content2");
            newDialog.SetDialogText(DialogTextType.MainText, string.Format(mainFormat, pname));
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.Show();
        }
    }

    /// <summary>
    /// ダイアログ　期限警告
    /// </summary>
    public void openWarningTermDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_GETBOX_GACHA_STATUS_DISABLE_TITLE");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_GETBOX_GACHA_STATUS_DISABLE");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            //Home画面へ
            if (MainMenuManager.HasInstance)
            {
                MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
            }
        });
        newDialog.Show();
    }

    // ガチャ実行
    // fix_id:  count:回数   tutorial:チュートリアルフラグ
    private void SendScratch(int count)
    {
        //連打防止
        if (ServerApi.IsExists)
        {
            return;
        }

        //スクラッチのリザルトが終わるまでHomeに戻るバックキー無効
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.DisableBackKey();
        }

        //ユニット取得フラグ保存
        MainMenuParam.m_GachaPrevUnitGetFlag = UserDataAdmin.Instance.m_StructPlayer.flag_unit_get;

#if BUILD_TYPE_DEBUG
        Debug.Log("Scratch Send");
#endif
        int tutorial = (TutorialManager.IsExists) ? 1 : 0;
        var serverApi = ServerDataUtilSend.SendPacketAPI_GachaPlay(Context.gachaMaster.fix_id, (uint)count, (uint)tutorial);

        // SendStartの成功時の振る舞い
        serverApi.setSuccessAction(_data =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("Scratch Send Success");
#endif
            // DG0-2733 Tutorial時、StructPlayer.renew_tutorial_step == 602 に更新される
            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvGachaPlay>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();

            RecvGachaPlayValue result = _data.GetResult<RecvGachaPlay>().result;
            //----------------------------------------
            // ガチャの結果表示画面で使う情報入力
            //----------------------------------------
            MainMenuParam.m_GachaUnitUniqueID = result.unit_unique_id;
            MainMenuParam.m_GachaBlankUnitID = result.blank_unit_id;
            MainMenuParam.m_GachaOmakeID = result.gacha_bonus_data;
            MainMenuParam.m_GachaGetUnitNum = MainMenuParam.m_GachaUnitUniqueID.Length;

            if (result.gacha_status != null)
            {
                UserDataAdmin.Instance.UpdateGachaStatusList(result.gacha_status);
            }

            if (result.result_present != null)
            {
                UserDataAdmin.Instance.m_StructPresentList = UserDataAdmin.PresentListClipTimeLimit(result.result_present.present);
            }

            Context.m_IsMoveScratchResult = true;
            if (MainMenuManager.HasInstance) MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_GACHA_RESULT, false, false);
        });

        // SendStartの失敗時の振る舞い
        serverApi.setErrorAction(_data =>
        {
            // ステップが更新された
            if (_data.m_PacketCode == API_CODE.API_CODE_STEP_UP_GACHA_REST_TIME_NOW)
            {
                RecvGachaPlayValue result = _data.GetResult<RecvGachaPlay>().result;
                if (result.gacha_status != null)
                {
                    UserDataAdmin.Instance.UpdateGachaStatusList(result.gacha_status);
                }
                MainMenuManager.Instance.SetResetSubTabFlag();
                MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_GACHA_MAIN, false, false);
            }

#if BUILD_TYPE_DEBUG
            Debug.Log("Scratch Send Error");
#endif
        });

        serverApi.SendStart();
    }
}
