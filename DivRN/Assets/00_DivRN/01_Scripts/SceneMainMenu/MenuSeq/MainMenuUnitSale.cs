using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuUnitSale : MainMenuSeq
{
    private readonly int MAX_SALE_UNIT_COUNT = 16;

    private UnitBGPanel m_UnitBGPanel = null;
    private ExpandWindow m_ExpandWindow = null;
    private UnitSaleList m_UnitSale = null;
    private UnitGridComplex m_UnitGrid = null;

    private bool m_Validate = false;
    private bool m_WarningRarity = false;
    private bool m_WarningBuildup = false;

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

        if (ChkUserDataUpdate())
        {
            updateUnitList();
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        if (m_UnitBGPanel == null)
        {
            m_UnitBGPanel = m_CanvasObj.GetComponentInChildren<UnitBGPanel>();
            m_UnitBGPanel.SetPositionAjustStatusBar(new Vector2(0, 40), new Vector2(0, -345));
            m_UnitBGPanel.IsViewIcon = false;
            m_UnitBGPanel.IsViewSale = true;
            m_UnitBGPanel.IsViewResetButton = false;
            m_UnitBGPanel.IsViewExecButton = true;
            m_UnitBGPanel.ExecButtonImage = ResourceManager.Instance.Load("confirm_button");
            m_UnitBGPanel.IsViewReturnButton = false;
            m_UnitBGPanel.IsViewResetButton = true;
            m_UnitBGPanel.Title = GameTextUtil.GetText("unit_function_sell");
            m_UnitBGPanel.IsDetailButton = true;


            m_UnitBGPanel.SaleCountMax = MAX_SALE_UNIT_COUNT;

            m_UnitBGPanel.DidReset = SelectReset;
            m_UnitBGPanel.DidSelect = OpenSaleDialog;
            m_UnitBGPanel.DidReturn = SelectReturn;
            m_UnitBGPanel.OnClickDetailAction = OnDetailWindow;

        }
        if (m_ExpandWindow == null)
        {
            m_ExpandWindow = m_CanvasObj.GetComponentInChildren<ExpandWindow>();
            m_ExpandWindow.SetPositionAjustStatusBar(new Vector2(0, -232));
            m_ExpandWindow.ViewHeightSize = 182.0f;
            m_ExpandWindow.DidSelectButton = SelectWindowButton;
        }

        if (m_UnitSale == null)
        {
            m_UnitSale = m_CanvasObj.GetComponentInChildren<UnitSaleList>();
            for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
            {
                m_UnitSale.addItem(i, 0);
            }

            if (m_ExpandWindow != null)
            {
                m_UnitSale.SetParent(m_ExpandWindow.Content);
            }
        }

        if (m_UnitGrid == null)
        {
            //ユニットグリッド取得
            m_UnitGrid = m_CanvasObj.GetComponentInChildren<UnitGridComplex>();
            //サイズ設定
            m_UnitGrid.SetPositionAjustStatusBar(new Vector2(0, -35), new Vector2(-48, -315));

            m_UnitGrid.AttchUnitGrid<UnitGridView>(UnitGridView.Create());
        }

        m_Validate = false;
        m_ExpandWindow.Close(true);
        m_UnitBGPanel.IsViewReturnButton = false;
        m_UnitBGPanel.IsViewPanel = false;
        m_UnitBGPanel.ExecButtonImage = ResourceManager.Instance.Load("confirm_button");
        m_UnitBGPanel.IsActiveExecButton = false;
        m_UnitGrid.IsActiveSortButton = true;

        //
        resetSaleUnit();

        //ユニット情報構築
        updateUnitList();

        //
        SetupSaleStatusValue();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.UNIT;
    }

    public void resetSaleUnit()
    {
        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            var unit = m_UnitSale.UnitList[i];
            UnitIconImageProvider.Instance.Reset(unit.m_CharaId);
            unit.reset();
        }

        SetupSaleStatusValue();
    }

    public void updateUnitList()
    {
        List<UnitGridContext> unitList = MainMenuUtil.MakeUnitGridContextList();
        if (unitList == null)
        {
            Debug.LogError("unitlist is null");
            return;
        }

        m_UnitGrid.OnClickSortButtonAction = OnClockSortButton;
        m_UnitGrid.ClickUnitAction = SelectUnit;
        m_UnitGrid.LongPressUnitAction = SelectUnitLongPress;
        m_UnitGrid.SetupUnitSelected = SetupUintSelected;
        m_UnitGrid.SetupUnitIconType = SetupUnitIconType;

        m_UnitGrid.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterUnitSale());
        m_UnitGrid.CreateList(unitList);
    }

    private bool CheckSaleUnit(PacketStructUnit _unit)
    {
        //パーティチェック
        if (MainMenuUtil.ChkUnitPartyAssign(_unit.unique_id))
        {
            return false;
        }

        // お気に入り
        if (MainMenuUtil.ChkUnitFavorite(_unit.unique_id))
        {
            return false;
        }

        //リンクチェック
        if (_unit.link_info != (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
        {
            return false;
        }

        return true;
    }

    public void SelectUnit(UnitGridContext _unit)
    {
        //
        if (IsBusy())
        {
            return;
        }
#if BUILD_TYPE_DEBUG
        // 現在の経過時間を取得
        float check_time = Time.realtimeSinceStartup;
        try
#endif
        {
            if (IsSelectSaleUnit(_unit.UnitData.unique_id))
            {
                SetUnitUnselected(_unit);
            }
            else
            {
                SetUnitSelected(_unit);
            }

            SetupSaleStatusValue();
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

    private void SetUnitSelected(UnitGridContext unit)
    {
        if (IsSelectSaleMax())
        {
            return;
        }

        if (!CheckSaleUnit(unit.UnitData))
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        AddSaleUnit(unit.UnitData.unique_id);
        unit.IsSelectedUnit = true;

        m_UnitGrid.UpdateItem(unit);
    }
    private void SetUnitUnselected(UnitGridContext unit)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        DelSaleUnit(unit.UnitData.unique_id);
        unit.IsSelectedUnit = false;

        m_UnitGrid.UpdateItem(unit);
    }

    public void SelectUnitLongPress(UnitGridContext _unit)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        //ユニット詳細画面へ
        if (MainMenuManager.HasInstance)
        {
            UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo();
            if (_info == null) return;
            PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit.UnitData);
            _info.SetUnitFavorite(_unit.UnitData, _subUnit, _unit);
            _info.SetCloseAction(() =>
            {
                //選択されてるユニットがお気に入り登録されたら選択を解除する
                if (IsSelectSaleUnit(_unit.UnitData.unique_id) &&
                    _unit.IsActiveFavoriteImage)
                {
                    //選択解除
                    SetUnitUnselected(_unit);
                    //ステータス更新
                    SetupSaleStatusValue();
                }

                //IconType更新
                SetupUnitIconType(_unit);

                //更新データ反映
                m_UnitGrid.UpdateBaseItem(_unit);
            });
        }
    }

    private void SelectReturn()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
    }

    private void SelectReset()
    {
        //
        if (IsBusy())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        resetSaleUnit();
        m_UnitGrid.UpdateList();
        SetupSaleStatusValue();
    }

    private void SelectWindowButton()
    {
        if (m_ExpandWindow == null)
        {
            return;
        }

        if (m_ExpandWindow.isOpen == true)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            m_ExpandWindow.Close();
            m_UnitGrid.changeGridWindowSize(false, m_ExpandWindow.ViewHeightSize);
        }
    }

    private void AddSaleUnit(long unique_unit_id)
    {
        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            if (m_UnitSale.UnitList[i].m_UniqueId == unique_unit_id)
            {
                return;
            }
        }

        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            if (m_UnitSale.UnitList[i].m_UniqueId == 0)
            {
                PacketStructUnit addUnit = GetUnitData(unique_unit_id);
                if (addUnit == null)
                {
                    return;
                }

                m_UnitSale.UnitList[i].IsViewIcon = true;
                m_UnitSale.UnitList[i].m_UniqueId = addUnit.unique_id;
                m_UnitSale.UnitList[i].m_CharaId = addUnit.id;

                UnitIconImageProvider.Instance.Get(
                    addUnit.id,
                    sprite =>
                    {
                        m_UnitSale.UnitList[i].IconImage = sprite;
                    },
                    true);

                return;
            }
        }
    }

    private void DelSaleUnit(long unique_unit_id)
    {
        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            var unit = m_UnitSale.UnitList[i];
            if (unit.m_UniqueId == unique_unit_id)
            {
                UnitIconImageProvider.Instance.Reset(unit.m_CharaId);
                unit.reset();
            }
        }

        /// 次に設定されている素材取得
        System.Func<int, MaterialDataContext> getnextmaterial = (int _start) =>
        {
            for (int i = _start + 1; i < MAX_SALE_UNIT_COUNT; i++)
            {
                if (m_UnitSale.UnitList[i].IsViewIcon)
                {
                    return m_UnitSale.UnitList[i];
                }
            }

            return null;
        };

        // 素材を詰める
        for (int i = 0; i < (MAX_SALE_UNIT_COUNT - 1); i++)
        {
            if (m_UnitSale.UnitList[i].IsViewIcon)
            {
                continue;
            }

            MaterialDataContext _next = getnextmaterial(i);
            if (_next == null)
            {
                return;
            }

            m_UnitSale.UnitList[i].IsViewIcon = true;
            m_UnitSale.UnitList[i].m_CharaId = _next.m_CharaId;
            m_UnitSale.UnitList[i].m_UniqueId = _next.m_UniqueId;

            UnitIconImageProvider.Instance.Get(
                _next.m_CharaId,
                sprite =>
                {
                    m_UnitSale.UnitList[i].IconImage = sprite;
                },
                true);

            _next.reset();
        }
    }

    private PacketStructUnit GetUnitData(long unique_unit_id)
    {
        PacketStructUnit[] unitlist = UserDataAdmin.Instance.m_StructPlayer.unit_list;

        for (int i = 0; i < unitlist.Length; i++)
        {
            if (unitlist[i].unique_id == unique_unit_id)
            {
                return unitlist[i];
            }
        }
        return null;
    }

    private bool IsSelectSaleUnit(long unique_unit_id)
    {
        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            if (m_UnitSale.UnitList[i].m_UniqueId == unique_unit_id)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsSelectSaleMax()
    {
        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            if (m_UnitSale.UnitList[i].m_UniqueId == 0)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsSaleUnit()
    {
        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            if (m_UnitSale.UnitList[i].m_UniqueId != 0)
            {
                return true;
            }
        }
        return false;
    }

    public void OpenSaleDialog()
    {
        if (!IsSaleUnit())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        //----------------------------------------
        // パラメータリミットチェック
        //----------------------------------------
        //チェック対象：コイン・ユニットポイント
        PRM_LIMIT_ERROR_TYPE ret = MainMenuUtil.ChkPrmLimit((uint)m_UnitBGPanel.Money, 0, 0, 0, (uint)m_UnitBGPanel.Point);
        Dialog warningDialog = null;
        switch (ret)
        {
            case PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_COIN:
                // コインが最大数オーバー
                warningDialog = Dialog.Create(DialogType.DialogYesNo);
                warningDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_SALE_LIMIT_COIN_TITLE");
                warningDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_SALE_LIMIT_COIN");
                break;
            case PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_UNITPOINT:
                // ユニットポイントが最大数オーバー
                warningDialog = Dialog.Create(DialogType.DialogYesNo);
                warningDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_SALE_LIMIT_UNITPOINT_TITLE");
                warningDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_SALE_LIMIT_UNITPOINT");
                break;
            case PRM_LIMIT_ERROR_TYPE.PRM_LIMIT_ERR_OTHER:
                // コインとユニットポイントが最大数オーバー
                warningDialog = Dialog.Create(DialogType.DialogYesNo);
                warningDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_SALE_LIMIT_OTHER_TITLE");
                warningDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_SALE_LIMIT_OTHER");
                break;
        }

        if (warningDialog != null)
        {
            warningDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
            warningDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
            warningDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
            {
                openSaleWarningDialog();
            });
            warningDialog.Show();
            return;
        }

        openSaleWarningDialog();
    }

    private void openSaleWarningDialog()
    {
        Dialog _newDialog = Dialog.Create(DialogType.DialogIconList).SetStrongYes();

        for (int i = 0; i < m_UnitSale.UnitList.Count; i++)
        {
            if (m_UnitSale.UnitList[i].m_UniqueId == 0)
            {
                continue;
            }

            PacketStructUnit partsUnit = UserDataAdmin.Instance.SearchChara(m_UnitSale.UnitList[i].m_UniqueId);
            MasterDataParamChara partsMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)partsUnit.id);

            DialogIconItem iconItem = new DialogIconItem();

            //アイコン
            UnitIconImageProvider.Instance.Get(
                partsUnit.id,
                (sprite) =>
                {
                    iconItem.IconImage = sprite;
                });


            //LV+PLUS表示
            iconItem.ParamValue = (partsUnit.level >= partsMaster.level_max) ? GameTextUtil.GetText("unit_status18")
                                : string.Format(GameTextUtil.GetText("uniticon_flag2"), partsUnit.level); // レベル
            uint plus = partsUnit.add_hp + partsUnit.add_pow;
            if (plus != 0)
            {
                string format = GameTextUtil.GetText("uniticon_flag3");
                iconItem.ParamValue += string.Format(format, plus);
            }

            _newDialog.IconList.Add(iconItem);
        }

        string mainText = "";
        if (m_WarningRarity ||
            m_WarningBuildup)
        {
            if (m_WarningBuildup)
            {
                mainText += GameTextUtil.GetText("unit_sale_01") + "\n";
            }
            if (m_WarningRarity)
            {
                mainText += GameTextUtil.GetText("unit_sale_02") + "\n";
            }
            mainText += GameTextUtil.GetText("unit_sale_03");

            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "unit_sale_title");
        }
        else
        {
            mainText = GameTextUtil.GetText("unitsold_text2");

            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "unit_sale_04");
        }


        _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
        _newDialog.SetDialogText(DialogTextType.UnderText, mainText);

        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            SaleUnit();
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();
    }

    public void SaleUnit()
    {
        if (ServerApi.IsExists)
        {
            return;
        }

        long[] sale_unit = new long[MAX_SALE_UNIT_COUNT];
        int sale_count = 0;
        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            var unit = m_UnitSale.UnitList[i];
            if (unit.m_UniqueId != 0 &&
                unit.IsViewIcon)
            {
                sale_unit[sale_count] = unit.m_UniqueId;
                sale_count++;
            }
        }

        ButtonBlocker.Instance.Block(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);

        ServerDataUtilSend.SendPacketAPI_UnitSale(sale_unit)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvUnitSale>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();
            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "un97q_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "un97q_content");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
                resetSaleUnit();
                updateUnitList();
                SetupSaleStatusValue();
                StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
            });
            _newDialog.Show();

            ButtonBlocker.Instance.Unblock(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
        })
        .setErrorAction(_data =>
        {
            ButtonBlocker.Instance.Unblock(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
        })
        .SendStart();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ステータス更新
		@note	売却のパラメータ表示部分のテキスト表記を更新
	*/
    //----------------------------------------------------------------------------
    void SetupSaleStatusValue()
    {
        //-----------------------
        // 選択してるユニットから合計金額と合計ユニットポイントを求める
        //-----------------------
        uint unTotalMoney = 0;
        uint unTotalUnitPoint = 0;
        uint unSelectCount = 0;

        m_WarningRarity = false;
        m_WarningBuildup = false;

        for (int i = 0; i < MAX_SALE_UNIT_COUNT; i++)
        {
            if (m_UnitSale.UnitList[i].m_UniqueId == 0)
            {
                continue;
            }

            PacketStructUnit _saleUnit = UserDataAdmin.Instance.SearchChara(m_UnitSale.UnitList[i].m_UniqueId);
            MasterDataParamChara _saleMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)_saleUnit.id);

            if (_saleUnit == null ||
                _saleMaster == null)
            {
                continue;
            }

            unSelectCount++;

            //レアリティ★４以上のユニットが選択されていたら警告する。
            if ((int)_saleMaster.rare >= (int)MasterDataDefineLabel.RarityType.STAR_4)
            {
                m_WarningRarity = true;
            }

            //ユニットが強化されていたら警告する。
            if (_saleUnit.level > 1 ||
                (_saleMaster.skill_limitbreak != 0 && _saleUnit.limitbreak_lv > 0) ||
                _saleUnit.limitover_lv > 0 ||
                _saleUnit.add_hp > 0 ||
                _saleUnit.add_pow > 0)
            {
                m_WarningBuildup = true;
            }

            // コイン
            uint unUnitMoney = (uint)CharaUtil.GetStatusValue(_saleMaster, (int)_saleUnit.level, CharaUtil.VALUE.SALE);

            unTotalMoney += unUnitMoney;

            unTotalMoney += _saleUnit.add_pow * GlobalDefine.PLUS_SALE_COIN;
            unTotalMoney += _saleUnit.add_hp * GlobalDefine.PLUS_SALE_COIN;

            // ユニットポイント
            unTotalUnitPoint += (uint)_saleMaster.sales_unitpoint;

            // 限界突破によるコインとユニットポイントの計算
            if (_saleUnit.limitover_lv > 0)
            {
                // 限界突破によるボーナスコイン
                double dLimitOverMaxLevel = CharaLimitOver.GetParam(0, _saleMaster.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_LIMITOVER_MAX);
                double dLimitOverLevel = (double)_saleUnit.limitover_lv;
                //unTotalMoney += (uint)( unUnitMoney * nLimitOverLevel + unUnitMoney * ( nLimitOverLevel + 1 ) * ( GlobalDefine.LIMITOVER_BONUS * nLimitOverLevel / nLimitOverMaxLevel ) );
                double dLoBounsMoney = CharaLimitOver.GetParamSaleLimitOverBouns(dLimitOverLevel, dLimitOverMaxLevel, unUnitMoney);
                unTotalMoney += (uint)dLoBounsMoney;

                // 限界突破によるボーナスユニットポイント
                unTotalUnitPoint += (uint)(_saleMaster.sales_unitpoint * dLimitOverLevel);
            }
        }

        m_UnitBGPanel.Money = (int)unTotalMoney;
        m_UnitBGPanel.Point = (int)unTotalUnitPoint;
        m_UnitBGPanel.SaleCount = (int)unSelectCount;
        m_UnitBGPanel.IsEnableResetButton =
            m_UnitBGPanel.IsActiveExecButton = IsSaleUnit();
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

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        SortDialog dialog = SortDialog.Create();
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.UNIT);
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterUnitSale());
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
        LocalSaveManager.Instance.SaveFuncSortFilterUnitSale(sortInfo);

        m_UnitGrid.ExecSortBuild(sortInfo);
    }

    void OnClickSortThread(LocalSaveSortInfo sortInfo)
    {
        m_UnitGrid.ExecSortOnly(sortInfo);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    void SetupUintSelected(UnitGridContext _unit)
    {
        _unit.IsSelectedUnit = IsSelectSaleUnit(_unit.UnitData.unique_id);
    }

    void SetupUnitIconType(UnitGridContext _unit)
    {
        //売却できるか？
        if (CheckSaleUnit(_unit.UnitData))
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
        }
        else
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON;
        }

        m_UnitGrid.UpdateItem(_unit);
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

        return false;
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        base.PageSwitchEventDisableAfter(eNextMainMenuSeq);
        m_ExpandWindow.SetBackKey(false);
        AndroidBackKeyManager.Instance.EnableBackKey();

        return false;

    }

    void OnDetailWindow()
    {
        if (m_ExpandWindow == null)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        if (m_ExpandWindow.isOpen == false)
        {
            m_ExpandWindow.SetBackKey(true);
            m_ExpandWindow.Open();
            m_UnitGrid.changeGridWindowSize(true, m_ExpandWindow.ViewHeightSize);
        }
        else
        {
            m_ExpandWindow.SetBackKey(false);
            m_ExpandWindow.Close();
            m_UnitGrid.changeGridWindowSize(false, m_ExpandWindow.ViewHeightSize);
        }
    }

    private bool IsBusy()
    {
        if (Dialog.HasDialog() ||
             ServerApi.IsExists)
        {
            return true;
        }
        return false;
    }
}
