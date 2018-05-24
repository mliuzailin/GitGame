using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerDataDefine;

public class MainMenuUnitPointLimitOver : MainMenuSeq
{
    private UnitBGPanel m_UnitBGPanel = null;
    private ExpandWindow m_ExpandWindow = null;
    private UnitStatusPanel m_UnitStatusPanel = null;
    private UnitGridComplex m_UnitGrid = null;

    private UnitGridParam m_BaseUnit = null;

    private int m_BlendLimitOverResult = 0;
    private bool m_BlendLimitOverWarning = false;
    private bool m_BlendLinkPointUpFlag = false;
    private bool m_BlendLinkPointMaxFlag = false;
    private bool m_BlendRarityWarning = false;
    private bool m_BlendLevelMaxWarning = false;
    private bool m_BlendPartsSameCharacter = false;
    private bool m_BlendPartsSameSkill = false;
    private bool m_Premium = false;
    private bool m_Validate = false;
    private bool m_IsProductID = false;
    private uint m_BlendPoint = 0;

    private UnitResult m_UnitResult = null;

    private Sprite m_ConfirmSprite = null;
    private Sprite m_DecideSprite = null;

    UnitResultBuildupModel m_unitResultBuildupModel = new UnitResultBuildupModel();

    private Dialog m_MaxDialog = null;
    private int m_UnitListCount = 0;


    protected override void Start()
    {
        base.Start();
        m_ConfirmSprite = ResourceManager.Instance.Load("confirm_button");
        m_DecideSprite = ResourceManager.Instance.Load("limitover_button");

        m_IsProductID = false;
        ServerDataUtilSend.SendPacketAPI_GetPointShopProduct()
        .setSuccessAction(_data =>
        {
            var shop_product = _data.GetResult<ServerDataDefine.RecvGetPointShopProduct>().result.shop_product.ToList();
            for (int id = 0; id < shop_product.Count; id++)
            {
                if (shop_product[id].fix_id == 0)
                {
                    continue;
                }
                if (shop_product[id].product_type == MasterDataDefineLabel.PointShopType.LIMITOVER)
                {
                    MainMenuParam.m_PointShopLimitOverProductID = shop_product[id].fix_id;
                }
            }
            m_IsProductID = true;
        })
        // SendStartの失敗時の振る舞い
        .setErrorAction(_date =>
        {
        })
        .SendStart();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

        if (ChkUserDataUpdate())
        {
            makeUnitList();
            m_UnitGrid.UpdateList();
        }
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        base.PageSwitchEventDisableAfter(eNextMainMenuSeq);

        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        m_ExpandWindow.SetBackKey(false);
        AndroidBackKeyManager.Instance.EnableBackKey();

        return false;
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        if (m_UnitBGPanel == null)
        {
            m_UnitBGPanel = m_CanvasObj.GetComponentInChildren<UnitBGPanel>();
            m_UnitBGPanel.SetPositionAjustStatusBar(new Vector2(0, 40), new Vector2(0, -345));
            m_UnitBGPanel.IsViewPointEvolve = true;
            m_UnitBGPanel.IsViewResetButton = false;
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;
            m_UnitBGPanel.Title = GameTextUtil.GetText("unit_status10");
            m_UnitBGPanel.TotalTitle = GameTextUtil.GetText("unit_function_point");
            m_UnitBGPanel.TotalPoint = (int)UserDataAdmin.Instance.m_StructPlayer.have_unit_point;
            m_UnitBGPanel.DidSelect = SelectBuildup;
            m_UnitBGPanel.DidReset = SelectReset;
            m_UnitBGPanel.DidReturn = SelectReturn;
            m_UnitBGPanel.DidSelectIcon = SelectUnitIcon;
        }

        if (m_ExpandWindow == null)
        {
            m_ExpandWindow = m_CanvasObj.GetComponentInChildren<ExpandWindow>();
            m_ExpandWindow.SetPositionAjustStatusBar(new Vector2(0, -232));
            m_ExpandWindow.ViewHeightSize = 210.0f;
            m_ExpandWindow.DidSelectButton = SelectWindowButton;
        }

        if (m_UnitStatusPanel == null)
        {
            m_UnitStatusPanel = m_CanvasObj.GetComponentInChildren<UnitStatusPanel>();
            m_UnitStatusPanel.DidSelectPremium = SelectPremium;
            if (m_ExpandWindow != null) m_UnitStatusPanel.SetParent(m_ExpandWindow.Content);
        }


        if (m_UnitGrid == null)
        {
            //ユニットグリッド取得
            m_UnitGrid = m_CanvasObj.GetComponentInChildren<UnitGridComplex>();
            //サイズ設定
            m_UnitGrid.SetPositionAjustStatusBar(new Vector2(0, -35), new Vector2(-48, -315));

            m_UnitGrid.AttchUnitGrid<UnitGridView>(UnitGridView.Create());
        }

        updateBuildupStatus(true);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.UNIT;
    }

    /// <summary>
    /// 強化画面更新
    /// </summary>
    /// <param name="bRenew"></param>
    public void updateBuildupStatus(bool bRenew = false)
    {
        if (bRenew)
        {
            makeUnitList();
            m_ExpandWindow.Close(true);
            m_Validate = false;
            m_UnitBGPanel.IsViewPanel = false;
            m_UnitGrid.IsActiveSortButton = true;
            m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
        }

        if (m_UnitListCount <= 0)
        {
            if (m_MaxDialog == null)
            {
                m_MaxDialog = Dialog.Create(DialogType.DialogOK);
                m_MaxDialog.SetDialogTextFromTextkey(DialogTextType.Title, "error_reject_common_title");
                m_MaxDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "unit_list_Unowned");
                m_MaxDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                m_MaxDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
                {
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
                });
                m_MaxDialog.DisableCancelButton();
                m_MaxDialog.Show();
            }
        }

        m_BaseUnit = UserDataAdmin.Instance.SearchUnitGridParam(MainMenuParam.m_BuildupBaseUnitUniqueId);
        if (m_BaseUnit != null)
        {
            MasterDataParamChara _master = m_BaseUnit.master;
            m_UnitStatusPanel.setupUnit(m_BaseUnit.unit);

            m_UnitBGPanel.setupBaseUnit(_master, m_BaseUnit.unit);
            m_UnitBGPanel.IsViewExecButton = true;
            m_UnitBGPanel.IsViewReturnButton = true;
            m_UnitBGPanel.ExecButtonImage = m_ConfirmSprite;

            m_UnitBGPanel.IsActiveExecButton = IsBuildupStart();

            // 限界突破対応リストを作成
            CharaLimitOver.SetEvolBaseUnitIdList(_master.fix_id);

            updateUnitBaseAfterList();

            SetupBlendAfterStatus();
        }
        else
        {
            m_UnitBGPanel.resetBaseUnit();
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;
            m_UnitBGPanel.Point = 0;
            m_UnitStatusPanel.reset();
            updateUnitBaseList();
        }
    }

    /// <summary>
    /// ユニットリスト生成
    /// </summary>
    private void makeUnitList()
    {
        List<UnitGridContext> unitList = MainMenuUtil.MakeUnitGridContextListLimitOver();
        if (unitList == null)
        {
            Debug.LogError("unitlist is null");
            return;
        }

        m_UnitGrid.OnClickSortButtonAction = OnClockSortButton;
        m_UnitGrid.ClickUnitAction = SelectUnit;
        m_UnitGrid.LongPressUnitAction = SelectUnitLongPress;
        m_UnitGrid.SetupUnitSelected = SetupUintSelected;

        m_UnitGrid.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterUnitPointLO());
        m_UnitGrid.CreateList(unitList);

        m_UnitListCount = unitList.Count;
    }

    /// <summary>
    /// ベースユニット選択後更新
    /// </summary>
    /// <param name="bRenew"></param>
    public void updateUnitBaseAfterList()
    {
        m_UnitGrid.SetupUnitIconType = SetupBaseUnitAfterIconType;
        m_UnitGrid.UpdateList();
    }

    /// <summary>
    /// ベースユニット選択更新
    /// </summary>
    /// <param name="bRenew"></param>
    public void updateUnitBaseList()
    {
        m_UnitGrid.SetupUnitIconType = SetupBaseUnitIconType;
        m_UnitGrid.UpdateList();
    }

    /// <summary>
    /// ベースユニット解除
    /// </summary>
    private void unsetBaseUnit()
    {
        if (m_BaseUnit == null)
        {
            return;
        }

        //ベース設定解除
        UnitGridContext unit = m_UnitGrid.Units.Find((v) => v.UnitData.unique_id == m_BaseUnit.unique_id);
        if (unit != null)
        {
            unit.IsSelectedUnit = false;
        }

        MainMenuParam.m_BuildupBaseUnitUniqueId = 0;
        m_BaseUnit = null;
        updateBuildupStatus();

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        m_ExpandWindow.Close();
        m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
    }

    /// <summary>
    /// ベースユニットとして選択できるか
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckBaseUnit(PacketStructUnit _unit)
    {
        UnitGridParam unit = UserDataAdmin.Instance.SearchUnitGridParam(_unit.unique_id);
        if (unit == null)
        {
            return false;
        }
        if (unit.master == null)
        {
            return false;
        }
        MasterDataLimitOver cLimitOver = MasterDataUtil.GetLimitOverFromID(unit.master.limit_over_type);
        if (cLimitOver == null)
        {
            return false;
        }
        if (unit.limitover_lv >= cLimitOver.limit_over_max)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ユニット選択
    /// </summary>
    /// <param name="_unit"></param>
    private void SelectUnit(UnitGridContext _unit)
    {
        if (IsBusy() == true)
        {
            return;
        }

#if BUILD_TYPE_DEBUG
        // 現在の経過時間を取得
        float check_time = Time.realtimeSinceStartup;
        try
#endif
        {
            if (m_BaseUnit == null)
            {
                if (!CheckBaseUnit(_unit.UnitData))
                {
                    return;
                }

                SoundUtil.PlaySE(SEID.SE_MENU_OK);
                //ベース設定
                MainMenuParam.m_BuildupBaseUnitUniqueId = _unit.UnitData.unique_id;
                _unit.IsSelectedUnit = true;
                updateBuildupStatus();
                m_ExpandWindow.Open();
                m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                AndroidBackKeyManager.Instance.StackPush(m_CanvasObj.gameObject, OnSelectBackKey);
            }
            else if (m_BaseUnit.unique_id == _unit.UnitData.unique_id)
            {
                //ベース解除
                PopBaseUnit();
                m_ExpandWindow.Close();
                m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
            }
            m_UnitBGPanel.IsActiveExecButton = IsBuildupStart();
        }
#if BUILD_TYPE_DEBUG
        finally
        {
            // 処理完了後の経過時間から、保存していた経過時間を引く＝処理時間
            check_time = Time.realtimeSinceStartup - check_time;
            Debug.Log("SelectUnit Time : " + check_time.ToString("0.00000"));
        }
#endif
    }

    private void PopBaseUnit()
    {
        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        unsetBaseUnit();
    }

    private void OnSelectBackKey()
    {
        if (IsBusy() == true)
        {
            return;
        }

        PopBaseUnit();
    }

    /// <summary>
    /// ユニット長押し
    /// </summary>
    /// <param name="_unit"></param>
    private void SelectUnitLongPress(UnitGridContext _unit)
    {
        if (IsBusy() == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        if (MainMenuManager.HasInstance)
        {
            UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo();
            if (_info == null) return;
            PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit.UnitData);
            _info.SetUnitFavorite(_unit.UnitData, _subUnit, _unit);
            _info.SetCloseAction(() =>
            {
                // 更新データ反映
                m_UnitGrid.UpdateBaseItem(_unit);
            });
        }
    }

    /// <summary>
    /// ベースユニットアイコン選択
    /// </summary>
    private void SelectUnitIcon()
    {
        if (IsBusy() == true)
        {
            return;
        }

        //ベース解除
        OnSelectBackKey();
        setValidate(false);
    }

    /// <summary>
    /// 戻るボタン選択
    /// </summary>
    private void SelectReturn()
    {
        if (IsBusy() == true)
        {
            return;
        }

        if (m_Validate)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            setValidate(false);
        }
        else
        {
            OnSelectBackKey();
        }
    }

    /// <summary>
    /// プレミアムボタン選択
    /// </summary>
    /// <param name="bFlag"></param>
    private void SelectPremium(bool bFlag)
    {
        if (IsBusy() == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        m_Premium = bFlag;
        SetupBlendAfterStatus();
    }

    /// <summary>
    /// ウインドウボタン選択
    /// </summary>
    private void SelectWindowButton()
    {
        if (IsBusy() == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        if (m_ExpandWindow.isOpen == true)
        {
            m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
        }
        else
        {
            m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
            if (m_Validate == true)
            {
                AndroidBackKeyManager.Instance.StackPop(gameObject);
            }
        }
        if (m_Validate)
        {
            setValidate(false);
        }
    }

    private void SelectReset()
    {
        if (IsBusy() == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        if (m_BaseUnit == null)
        {
            updateUnitBaseList();
        }
    }

    private bool IsBuildupStart()
    {
        if (m_BaseUnit == null)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CALL IsBuildupStart:01");
#endif
            return false;
        }

        return true;
    }

    /// <summary>
    /// 決定ボタン選択
    /// </summary>
    private void SelectBuildup()
    {

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        {
            //----------------------------------------
            // 所持ポイントチェック
            //----------------------------------------
            bool bLessPoint = false;
            if (UserDataAdmin.Instance.m_StructPlayer.have_unit_point < m_UnitBGPanel.Point)
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("CALL IsBuildupStart03:" + UserDataAdmin.Instance.m_StructPlayer.have_unit_point + " MON:" + m_UnitBGPanel.Point);
#endif
                bLessPoint = true;
            }

            if (bLessPoint)
            {
                Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "unitpoint_impossible_title");
                newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "unitpoint_impossible_content");
                newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                newDialog.EnableFadePanel();
                newDialog.Show();
                return;
            }

        }

        if (!m_Validate)
        {
            Debug.LogError("m_VALIDATE");
            setValidate(true);
            return;
        }

        if (ServerApi.IsExists)
        {
            return;
        }

        if (m_Premium)
        {
            openWarningPremiumDialog();
        }
        else if (checkBuildupWarning())
        {
            openWarningBuildupDialog();
        }
        else
        {
            BuildupUnit();
        }
    }

    private void openWarningPremiumDialog()
    {
        Dialog _newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "un77q_title");
        string mainFormat = GameTextUtil.GetText("un77q_content");
        _newDialog.SetDialogText(DialogTextType.MainText, string.Format(mainFormat, UserDataAdmin.Instance.m_StructPlayer.have_stone));
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            if (UserDataAdmin.Instance.m_StructPlayer.have_stone == 0)
            {
                //チップ不足
                Dialog nextDialog = Dialog.Create(DialogType.DialogYesNo);
                nextDialog.SetDialogTextFromTextkey(DialogTextType.Title, "sh131q_title");
                nextDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "premium_boost_notchip");
                nextDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
                nextDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
                nextDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
                {
                    StoreDialogManager.Instance.OpenBuyStone();
                });
                nextDialog.EnableFadePanel();
                nextDialog.Show();

            }
            else if (checkBuildupWarning())
            {
                openWarningBuildupDialog();
            }
            else
            {
                BuildupUnit();
            }
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();
    }

    private void openWarningBuildupDialog()
    {
        Dialog _newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "un78q_title");

        string mainText = "";
        if (m_BlendLevelMaxWarning) mainText += GameTextUtil.GetText("un78q_content1") + "\n";
        if (m_BlendRarityWarning) mainText += GameTextUtil.GetText("un78q_content2") + "\n";
        if (m_BlendPartsSameCharacter) mainText += GameTextUtil.GetText("un78q_content3") + "\n";
        if (m_BlendPartsSameSkill) mainText += GameTextUtil.GetText("un78q_content4") + "\n";
        if (m_BlendLinkPointUpFlag && m_BlendLinkPointMaxFlag) mainText += GameTextUtil.GetText("un78q_content6") + "\n";
        mainText += GameTextUtil.GetText("un78q_content5");
        _newDialog.SetDialogText(DialogTextType.MainText, mainText);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            BuildupUnit();
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();
    }

    private bool checkBuildupWarning()
    {
        if (m_BlendLimitOverResult == (int)CharaLimitOver.RESULT_TYPE.eValueOver) return true;
        if (m_BlendLimitOverWarning) return true;
        if (m_BlendRarityWarning) return true;
        if (m_BlendPartsSameCharacter) return true;
        if (m_BlendPartsSameSkill) return true;
        return false;
    }

    /// <summary>
    /// ユニット強化実行
    /// </summary>
    private void BuildupUnit()
    {
        m_ExpandWindow.SetBackKey(false);
        AndroidBackKeyManager.Instance.DisableBackKey();
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL MainMenuUnitPointLimitOver#BuildupUnit");
#endif

        //----------------------------------------
        // 演出用に通信処理を行う前の情報を保持しておく
        //----------------------------------------
        {
            //ベースユニット
            MainMenuParam.m_BlendBuildUpUnitPrev = new PacketStructUnit();
            MainMenuParam.m_BlendBuildUpUnitPrev.Copy(m_BaseUnit.unit);
        }

        //----------------------------------------
        // チュートリアル判定
        //----------------------------------------
        bool is_premium = m_Premium;

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL MainMenuUnitPointLimitOver#BuildupUnit SendPacketAPI_UnitBlendBuildUp");
#endif


        Action buildUpAction = () =>
        {
            ButtonBlocker.Instance.Block(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
            ServerDataUtilSend.SendPacketAPI_PointShopLimitOver(
                                                              MainMenuParam.m_PointShopLimitOverProductID
                                                            , m_BaseUnit.unique_id
                                                            , 1)

            .setSuccessAction(_data =>
            {
                Debug.LogError("SUCCESS");
                resultSuccess(_data);
            })
            .setErrorAction(_data =>
            {

                Debug.LogError("ERROR:" + _data.m_PacketCode);
                buttonUnlock();
                AndroidBackKeyManager.Instance.EnableBackKey();
                resultError(_data);
            })
            .SendStart();
        };

        buildUpAction();
    }

    /// <summary>
    /// 通信成功
    /// </summary>
    /// <param name="_data"></param>
    private void resultSuccess(ServerApi.ResultData _data)
    {

        buttonUnlock();
        //----------------------------------------
        // 情報反映
        //----------------------------------------
        long unBuildUpCharaUniqueID = 0;
        //uint unBuildUpType = (uint)BUILDUP_TYPE.BUILDUP_TYPE_RATE_1_00;

        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvPointShopLimitOver>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
        UserDataAdmin.Instance.ConvertPartyAssing();

        //unBuildUpType = _data.GetResult<RecvUnitBlendBuildUp>().result.blend_pattern;
        unBuildUpCharaUniqueID = m_BaseUnit.unique_id;

        //----------------------------------------
        // 合成後のユニット情報を引き渡し
        //----------------------------------------
        PacketStructUnit cAfterUnit = UserDataAdmin.Instance.SearchChara(unBuildUpCharaUniqueID);
        if (cAfterUnit != null)
        {
            MainMenuParam.m_BlendBuildUpUnitAfter = new PacketStructUnit();
            MainMenuParam.m_BlendBuildUpUnitAfter.Copy(cAfterUnit);
        }
        else
        {
            Debug.LogError("Blend Unit After None!");
        }

        //----------------------------------------
        // 強化合成演出ページへ移行
        //----------------------------------------
        SoundUtil.PlaySE(SEID.SE_MAINMENU_BLEND_EXEC);

        //Camera mainCamera = SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>();
        m_UnitResult = UnitResult.Create(/*mainCamera,*/ UnitResult.ResultType.Builup);
        if (m_UnitResult != null)
        {
            UnitResultBuildup buildup = m_UnitResult.Parts.GetComponent<UnitResultBuildup>();
            buildup.setup(m_UnitStatusPanel, m_unitResultBuildupModel, ResetAll);
            return;
        }

        ResetAll();
    }

    /// <summary>
    /// 通信エラー
    /// </summary>
    /// <param name="_data"></param>
    private void resultError(ServerApi.ResultData _data)
    {
        if (_data.m_PacketCode == API_CODE.API_CODE_WIDE_API_EVENT_ERR_FP)
        {
            //----------------------------------------
            // フレンドポイント関連のイベントが終了している
            //----------------------------------------

            //----------------------------------------
            // サーバー時間とクライアント時間のズレで再表示されないように、
            // エラー発生時には端末側の時間情報を更新することで確実にイベント終了タイミングまで進める
            //----------------------------------------
            if (ServerDataParam.m_ServerTime != 0)
            {
                if (TimeManager.Instance != null)
                {
                    TimeManager.Instance.SetupServerTime(ServerDataParam.m_ServerTime);
                }
                if (UserDataAdmin.Instance != null
                && UserDataAdmin.Instance.m_StructSystem != null
                )
                {
                    UserDataAdmin.Instance.m_StructSystem.server_time = ServerDataParam.m_ServerTime;
                }
            }

            return;
        }

        if (_data.m_PacketCode == API_CODE.API_CODE_WIDE_API_EVENT_ERR_SLV)
        {
            //----------------------------------------
            // スキルレベルアップ関連のイベントが終了している
            //----------------------------------------

            //----------------------------------------
            // サーバー時間とクライアント時間のズレで再表示されないように、
            // エラー発生時には端末側の時間情報を更新することで確実にイベント終了タイミングまで進める
            //----------------------------------------
            if (ServerDataParam.m_ServerTime != 0)
            {
                if (TimeManager.Instance != null)
                {
                    TimeManager.Instance.SetupServerTime(ServerDataParam.m_ServerTime);
                }
                if (UserDataAdmin.Instance != null
                && UserDataAdmin.Instance.m_StructSystem != null
                )
                {
                    UserDataAdmin.Instance.m_StructSystem.server_time = ServerDataParam.m_ServerTime;
                }
            }

            return;
        }
        return;
    }

    private void ResetAll()
    {
        AndroidBackKeyManager.Instance.EnableBackKey();

        if (m_UnitResult != null)
        {
            m_UnitResult.Hide();
            m_UnitResult = null;
        }

        makeUnitList();
        PacketStructUnit unit = UserDataAdmin.Instance.SearchChara(MainMenuParam.m_BuildupBaseUnitUniqueId);
        if (CheckBaseUnit(unit) == false)
        {
            unsetBaseUnit();
        }

        m_Validate = false;
        m_UnitBGPanel.IsViewPanel = false;
        m_UnitGrid.IsActiveSortButton = true;

        updateBuildupStatus();
    }

    /// <summary>
    /// 確認フラグ設定
    /// </summary>
    /// <param name="bFlag"></param>
    private void setValidate(bool bFlag)
    {
        if (bFlag)
        {
            m_ExpandWindow.SetBackKey(false);
            m_ExpandWindow.SetBackKey(true);
            m_ExpandWindow.Open();
            m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
        }
        else
        {
            m_ExpandWindow.SetBackKey(false);
        }
        m_UnitBGPanel.Message = string.Format(GameTextUtil.GetText("limitover_text"), m_BlendPoint);
        m_UnitBGPanel.IsViewPanel = bFlag;
        m_UnitBGPanel.ExecButtonImage = bFlag ? m_DecideSprite : m_ConfirmSprite;
        m_UnitGrid.IsActiveSortButton = !bFlag;
        m_Validate = bFlag;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ステータス更新
		@note	
	*/
    //----------------------------------------------------------------------------
    void SetupBlendAfterStatus()
    {
        //-----------------------
        // 合成時のパラメータ数値表示部分を更新
        //-----------------------

        if (m_BaseUnit != null)
        {
            MasterDataParamChara cBaseMasterData = m_BaseUnit.master;
            if (cBaseMasterData != null)
            {
                CharaOnce cCharaParam = new CharaOnce();

                PacketStructUnit cBaseUnit = m_BaseUnit.unit;

                // リンクの限界突破
                int nLinkLimitOverLV = 0;

                PacketStructUnit cLinkUnit = CharaLinkUtil.GetLinkUnit(cBaseUnit.link_unique_id);

                if (cLinkUnit != null)
                {
                    nLinkLimitOverLV = (int)cLinkUnit.limitover_lv;
                }

                if (cBaseUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE)
                {
                    cCharaParam.CharaSetupFromID(cBaseUnit.id
                                            , (int)cBaseUnit.level
                                            , (int)cBaseUnit.limitbreak_lv
                                            , 0
                                            , (int)cBaseUnit.add_pow
                                            , (int)cBaseUnit.add_hp
                                            , cLinkUnit.id
                                            , (int)cLinkUnit.level
                                            , (int)cLinkUnit.add_pow
                                            , (int)cLinkUnit.add_hp
                                            , (int)cBaseUnit.link_point
                                            , nLinkLimitOverLV
                                            );
                }
                else
                {
                    cCharaParam.CharaSetupFromID(cBaseUnit.id
                                            , (int)cBaseUnit.level
                                            , (int)cBaseUnit.limitbreak_lv
                                            , 0
                                            , (int)cBaseUnit.add_pow
                                            , (int)cBaseUnit.add_hp
                                            , 0
                                            , 0
                                            , 0
                                            , 0
                                            , 0
                                            , nLinkLimitOverLV
                                            );
                }

                m_BlendLevelMaxWarning = false;
                if (m_BaseUnit.level >= cBaseMasterData.level_max)
                {
                    m_BlendLevelMaxWarning = true;
                }


                uint unBaseAtk = (uint)cCharaParam.m_CharaPow;
                uint unBaseHP = (uint)cCharaParam.m_CharaHP;

                //-----------------------
                // 合成費用を算出
                // 合成費用 = ( ベースキャラレベル * 100 * 素材数 ) + ( 関連キャラのプラス値合計 * 1000 )
                //-----------------------
                m_BlendPoint = 0;
                m_BlendPoint += (cBaseMasterData != null) ? (uint)cBaseMasterData.limit_over_unitpoint : 0;

                //-----------------------
                // プラス値を算出
                //-----------------------
                uint unUnitPlusHP = (uint)cCharaParam.m_CharaPlusHP;
                uint unUnitPlusAtk = (uint)cCharaParam.m_CharaPlusPow;

                //-----------------------
                // ユニットが持ってる総合経験値を算出
                // ※今回の合成で得られる経験値含む
                //-----------------------
                int nTotalEXP = 0;// GetTotalEXP(ref Parts, fBonusRateTotal, cBaseMasterData.element);
                int nTotalEXPAfter = nTotalEXP + (int)cBaseUnit.exp;
                //-----------------------
                // レベルを算出
                //-----------------------
                uint unUnitLevelMax = (uint)cBaseMasterData.level_max;
                uint unUnitLevel = (uint)CharaUtil.GetLevelFromExp(cBaseMasterData, nTotalEXPAfter);
                //-----------------------
                // HPとATKを算出
                //-----------------------
                uint unBuildUpHP = 0;
                uint unBuildUpAtk = 0;

                if (cBaseUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE)
                {
                    //リンク親の場合はリンクキャラの値を反映しておく
                    CharaOnce cCharaParamBuildUp = new CharaOnce();
                    cCharaParamBuildUp.CharaSetupFromID(cBaseUnit.id
                                                    , (int)unUnitLevel
                                                    , (int)cBaseUnit.limitbreak_lv
                                                    , 0
                                                    , (int)unUnitPlusAtk
                                                    , (int)unUnitPlusHP
                                                    , cLinkUnit.id
                                                    , (int)cLinkUnit.level
                                                    , (int)cLinkUnit.add_pow
                                                    , (int)cLinkUnit.add_hp
                                                    , (int)cBaseUnit.link_point
                                                    , nLinkLimitOverLV
                                                    );
                    unBuildUpHP = (uint)cCharaParamBuildUp.m_CharaHP;
                    unBuildUpAtk = (uint)cCharaParamBuildUp.m_CharaPow;
                }
                else
                {
                    unBuildUpHP = (uint)CharaUtil.GetStatusValue(cBaseMasterData, (int)unUnitLevel, CharaUtil.VALUE.HP);
                    unBuildUpAtk = (uint)CharaUtil.GetStatusValue(cBaseMasterData, (int)unUnitLevel, CharaUtil.VALUE.POW);
                    //プラス値より合成後のHPとATK値を求める
                    unBuildUpHP += unUnitPlusHP * GlobalDefine.PLUS_RATE_HP;
                    unBuildUpAtk += unUnitPlusAtk * GlobalDefine.PLUS_RATE_POW;
                }

                //-----------------------
                // HPとATKの表示形式とカラーの設定
                // 強化後のHP/ATK、プラス値更新があったら色つける
                //-----------------------
                //フォーマット
                string strFormatUp = GameTextUtil.GetText("kyouka_text1");          //値上昇時
                string strFormatMax = "{0}";            //値MAX
                string strFormat = "{0}";            //値変更なし
                string strFormatPlus = "[{0}]";
                string strFormatPlusNum = "+{0}";

                string strUnitPlusHP = "";
                string strUnitPlusHPNum = string.Format(strFormatPlusNum, unUnitPlusHP);
                if (unUnitPlusHP > (uint)cCharaParam.m_CharaPlusHP)
                {
                    strUnitPlusHP = string.Format(strFormatUp, strUnitPlusHPNum);
                }
                else if (unUnitPlusHP == GlobalDefine.PLUS_MAX)
                {
                    strUnitPlusHP = string.Format(strFormatMax, strUnitPlusHPNum);
                }
                else if (unUnitPlusHP == 0)
                {
                    strUnitPlusHP = "";
                }
                else
                {
                    strUnitPlusHP = string.Format(strFormat, strUnitPlusHPNum);
                }
                if (strUnitPlusHP != string.Empty)
                {
                    strUnitPlusHP = string.Format(strFormatPlus, strUnitPlusHP);
                }

                string strUnitPlusAtk = "";
                string strUnitPlusAtkum = string.Format(strFormatPlusNum, unUnitPlusAtk);
                if (unUnitPlusAtk > (uint)cCharaParam.m_CharaPlusPow)
                {
                    strUnitPlusAtk = string.Format(strFormatUp, strUnitPlusAtkum);
                }
                else if (unUnitPlusAtk == GlobalDefine.PLUS_MAX)
                {
                    strUnitPlusAtk = string.Format(strFormatMax, strUnitPlusAtkum);
                }
                else if (unUnitPlusAtk == 0)
                {
                    strUnitPlusAtk = "";
                }
                else
                {
                    strUnitPlusAtk = string.Format(strFormat, strUnitPlusAtkum);
                }
                if (strUnitPlusAtk != string.Empty)
                {
                    strUnitPlusAtk = string.Format(strFormatPlus, strUnitPlusAtk);
                }

                m_BlendLimitOverResult = 0;
                m_BlendLimitOverWarning = false;
                m_BlendLinkPointUpFlag = false;
                m_BlendLinkPointMaxFlag = false;
                m_BlendRarityWarning = false;
                m_BlendPartsSameCharacter = false;
                m_BlendPartsSameSkill = false;


                // 限界突破レベル
                int nLimitOverCount = 1;

                // ベースユニットの限界突破MAXレベル
                int nLimitOverMaxLevel = (int)CharaLimitOver.GetParam(0, cBaseMasterData.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_LIMITOVER_MAX);

                // 
                uint unBaseLinkPoint = 0;

                //リンクの子をベースにしている場合は親のリンクポイントを参照する
                if (m_BaseUnit.unit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK)
                {
                    PacketStructUnit cTmpLinkUnit = CharaLinkUtil.GetLinkUnit(m_BaseUnit.unit.link_unique_id);
                    unBaseLinkPoint = cTmpLinkUnit.link_point;
                }
                else if (m_BaseUnit.unit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE)
                {
                    unBaseLinkPoint = m_BaseUnit.unit.link_point;
                }

                //リンクポイントが最大かどうか
                if (unBaseLinkPoint >= CharaLinkUtil.LINK_POINT_MAX)
                {
                    m_BlendLinkPointMaxFlag = true;
                }

                // ベースユニットの限界突破レベル
                uint nLimitOverLevel = m_BaseUnit.unit.limitover_lv + (uint)nLimitOverCount;

                string strBuilupLO = "";
                string strBuilupCharm = "";
                bool bChangeLO = false;
                if (nLimitOverCount > 0 && nLimitOverMaxLevel > 0)
                {
                    bChangeLO = true;
                    // 限界突破が元々MAXの場合
                    if (nLimitOverMaxLevel == m_BaseUnit.limitover_lv)
                    {
                        m_BlendLimitOverResult = (int)CharaLimitOver.RESULT_TYPE.eValueMax;
                    }
                    // 限界突破の上限値を超える場合
                    else if (nLimitOverMaxLevel < nLimitOverLevel)
                    {
                        m_BlendLimitOverResult = (int)CharaLimitOver.RESULT_TYPE.eValueOver;
                    }
                    else
                    {
                        m_BlendLimitOverResult = (int)CharaLimitOver.RESULT_TYPE.ePossible;
                    }
                }

                //限界突破LV最大チェック
                if (nLimitOverMaxLevel < nLimitOverLevel)
                {
                    nLimitOverLevel = (uint)nLimitOverMaxLevel;
                }

                // 限界突破の加算値
                float fLoHp = ((float)CharaLimitOver.GetParam((uint)nLimitOverLevel, cBaseMasterData.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_HP) / 100);
                float fLoAtk = ((float)CharaLimitOver.GetParam((uint)nLimitOverLevel, cBaseMasterData.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_ATK) / 100);

                // ベースユニットの元々のHPと攻撃力を算出
                int nCharaHP = CharaUtil.GetStatusValue(cBaseMasterData, (int)unUnitLevel, CharaUtil.VALUE.HP);
                int nCharaPow = CharaUtil.GetStatusValue(cBaseMasterData, (int)unUnitLevel, CharaUtil.VALUE.POW);

                // 限界突破のパラメーターを反映
                uint unLO_HP = (uint)(nCharaHP * fLoHp);
                uint unLO_Atk = (uint)(nCharaPow * fLoAtk);

                double dLimitOverCharm = CharaLimitOver.GetParamCharm(nLimitOverLevel, cBaseMasterData.limit_over_type);
                //int nLimitOverCost = (int)CharaLimitOver.GetParam(nLimitOverLevel, cBaseMasterData.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_COST);

                string strBuildUpHP = (unBuildUpHP + unLO_HP).ToString();
                string strBuildUpAtk = (unBuildUpAtk + unLO_Atk).ToString();


                if (unBuildUpHP > unBaseHP || nLimitOverLevel != m_BaseUnit.limitover_lv)
                {
                    strBuildUpHP = string.Format(strFormatUp, strBuildUpHP);
                }
                else if (unBuildUpHP == GlobalDefine.VALUE_MAX_HP)
                {
                    strBuildUpHP = string.Format(strFormatMax, strBuildUpHP);
                }
                else
                {
                    strBuildUpHP = string.Format(strFormat, strBuildUpHP);
                }

                if (unBuildUpAtk > unBaseAtk || nLimitOverLevel != m_BaseUnit.limitover_lv)
                {
                    strBuildUpAtk = string.Format(strFormatUp, strBuildUpAtk);
                }
                else if (unBuildUpAtk == GlobalDefine.VALUE_MAX_POW)
                {
                    strBuildUpAtk = string.Format(strFormatMax, strBuildUpAtk);
                }
                else
                {
                    strBuildUpAtk = string.Format(strFormat, strBuildUpAtk);
                }

                if (bChangeLO && nLimitOverLevel != m_BaseUnit.limitover_lv)
                {
                    strBuilupLO = string.Format(strFormatUp, nLimitOverLevel);
                    strBuilupCharm = string.Format(strFormatUp, dLimitOverCharm.ToString("F1"));
                }
                else
                {
                    strBuilupLO = nLimitOverLevel.ToString();
                    strBuilupCharm = dLimitOverCharm.ToString("F1");
                }

                //----------------------------------------
                // LVカラー
                //----------------------------------------

                string strBuildUpLevel = "";
                if (unUnitLevel == cCharaParam.m_CharaMasterDataParam.level_max)
                {
                    strBuildUpLevel = string.Format(strFormatUp, GameTextUtil.GetText("unit_status18"));
                }
                else
                {
                    if (unUnitLevel != (uint)cCharaParam.m_CharaLevel)
                    {
                        strBuildUpLevel = string.Format("{0}><color=#f90974>{1}</color>/{2}", cCharaParam.m_CharaLevel, unUnitLevel, cCharaParam.m_CharaMasterDataParam.level_max);
                    }
                    else
                    {
                        strBuildUpLevel = string.Format(GameTextUtil.GetText("unit_status17"), cCharaParam.m_CharaLevel, cCharaParam.m_CharaMasterDataParam.level_max);
                    }
                }

                m_UnitStatusPanel.Level = strBuildUpLevel;
                m_UnitStatusPanel.Lo = strBuilupLO;
                m_UnitStatusPanel.Hp = strBuildUpHP + strUnitPlusHP;
                m_UnitStatusPanel.Atk = strBuildUpAtk + strUnitPlusAtk;
                m_UnitStatusPanel.Charm = strBuilupCharm;
                m_UnitBGPanel.Point = (int)m_BlendPoint;
            }
            else
            {
#if BUILD_TYPE_DEBUG
                Debug.LogError("MasterData None!");
#endif
            }
        }
        m_UnitBGPanel.TotalPoint = (int)UserDataAdmin.Instance.m_StructPlayer.have_unit_point;
    }

    /// <summary>
    /// ソートダイアログを開く
    /// </summary>
    void OnClockSortButton()
    {
        if (IsBusy() == true)
        {
            return;
        }

        if (SortDialog.IsExists == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        SortDialog dialog = SortDialog.Create();
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.UNIT);
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterUnitPointLO());
        dialog.OnCloseThreadAction = OnClickSortThread;
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
        LocalSaveManager.Instance.SaveFuncSortFilterUnitPointLO(sortInfo);

        m_UnitGrid.ExecSortBuild(sortInfo);
    }

    void OnClickSortThread(LocalSaveSortInfo sortInfo)
    {
        m_UnitGrid.ExecSortOnly(sortInfo);
    }


    /// <summary>
    /// 選択されているユニットかどうか
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    bool IsSelectedUint(PacketStructUnit _unit)
    {
        //ベースに選択されている
        if (m_BaseUnit != null && m_BaseUnit.unique_id == _unit.unique_id)
        {
            return true;
        }

        //選択されていない
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    void SetupUintSelected(UnitGridContext _unit)
    {
        _unit.IsSelectedUnit = IsSelectedUint(_unit.UnitData);
    }

    void SetupBaseUnitIconType(UnitGridContext _unit)
    {
        //ベースとして選択できるか？
        if (CheckBaseUnit(_unit.UnitData))
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
        }
        else
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON;
        }
    }

    void SetupBaseUnitAfterIconType(UnitGridContext _unit)
    {
        //ベースか？
        if (m_BaseUnit.unique_id == _unit.UnitData.unique_id)
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
        }
        else
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bBack"></param>
    /// <returns></returns>
    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        //ユニットパラメータが作成されるまで待つ
        if (UserDataAdmin.Instance.m_bThreadUnitParam)
        {
            return true;
        }
        if (m_IsProductID == false)
        {
            return true;
        }

        return false;
    }

    private void buttonUnlock()
    {
        if (ButtonBlocker.Instance.IsActive(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG) == true)
        {
            ButtonBlocker.Instance.Unblock(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
        }
    }

    private bool IsBusy()
    {
        if (Dialog.HasDialog() ||
            ServerApi.IsExists ||
            m_UnitResult != null)
        {
            return true;
        }
        return false;
    }
}
