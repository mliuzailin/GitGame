using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerDataDefine;

public class MainMenuUnitBuildup : MainMenuSeq
{
    private readonly uint BILDUP_MATERIAL_MAX = 10;    //!< 強化素材最大数
    private readonly float MATERIAL_ICON_SIZE = 40.0f;  //!< 素材アイコンサイズ

    private UnitBGPanel m_UnitBGPanel = null;
    private ExpandWindow m_ExpandWindow = null;
    private UnitGridComplex m_UnitGrid = null;
    private UnitMaterialPanel m_UnitMaterialPanel = null;
    private UnitStatusPanel m_UnitStatusPanel = null;

    private UnitGridParam m_BaseUnit = null;

    private int m_BlendLimitOverResult = 0;
    private bool m_BlendMaterialLOverWarning = false;
    private bool m_BlendRarityWarning = false;
    private bool m_BlendLevelMaxWarning = false;
    private bool m_BlendLBSLvMaxWarning = false;
    private bool m_BlendLOverMaxWarning = false;
    private bool m_BlendPlusMaxWarning = false;
    private bool m_BlendLinkPointMaxWarning = false;
    private bool m_BlendOverLOverWarning = false;
    private bool m_BlendOverPlusWarning = false;

    private bool m_BlendLimitEgg = false;
    private bool m_BlendLimitEggWarning = false;
    private bool m_BlendRarityOverEggWarning = false;
    private bool m_BlendNoLOverEggWarning = false;
    private bool m_BlendLimitEggMaxWarning = false;
    private bool m_BlendOverLimitEggWarning = false;

    private bool m_Premium = false;

    private UnitResult m_UnitResult = null;

    UnitResultBuildupModel m_unitResultBuildupModel = new UnitResultBuildupModel();


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
            m_UnitBGPanel.Title = GameTextUtil.GetText("unit_function_levelup");
            m_UnitBGPanel.ExecButtonImage = ResourceManager.Instance.Load("k_button");
            m_UnitBGPanel.DidSelect = SelectBuildup;
            m_UnitBGPanel.DidReset = SelectReset;
            m_UnitBGPanel.DidReturn = SelectReturn;
            m_UnitBGPanel.DidSelectIcon = SelectUnitIcon;
            m_UnitBGPanel.DidSelectIconLongpress = SelectUnitLongPress;
        }

        if (m_ExpandWindow == null)
        {
            m_ExpandWindow = m_CanvasObj.GetComponentInChildren<ExpandWindow>();
            m_ExpandWindow.SetPositionAjustStatusBar(new Vector2(0, -232));
            m_ExpandWindow.ViewHeightSize = 210.0f;
        }

        if (m_UnitStatusPanel == null)
        {
            m_UnitStatusPanel = m_CanvasObj.GetComponentInChildren<UnitStatusPanel>();
            m_UnitStatusPanel.IsViewPremiumButton = true;
            m_UnitStatusPanel.DidSelectPremium = SelectPremium;
            m_UnitStatusPanel.IsViewExp = true;
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

        if (m_UnitMaterialPanel == null)
        {
            m_UnitMaterialPanel = m_CanvasObj.GetComponentInChildren<UnitMaterialPanel>();
            m_UnitMaterialPanel.SetPositionAjustStatusBar(new Vector2(-205, -177));
            m_UnitMaterialPanel.setIconSize(MATERIAL_ICON_SIZE);
            m_UnitMaterialPanel.SetModel(m_unitResultBuildupModel);
            for (int i = 0; i < BILDUP_MATERIAL_MAX; i++)
            {
                m_UnitMaterialPanel.addItem(i, 0);
            }
        }

        //チュートリアル中はスクロールできなくする
        if (TutorialManager.IsExists)
        {
            ScrollRect scrollRect = m_UnitGrid.gameObject.GetComponentInChildren<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.vertical = false;
            }
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
            for (int i = 0; i < BILDUP_MATERIAL_MAX; i++)
            {
                var unit = m_UnitMaterialPanel.MaterialList[i];
                UnitIconImageProvider.Instance.Reset(unit.m_CharaId);
                unit.reset();
            }
            m_ExpandWindow.Close(true);
            m_UnitBGPanel.IsViewPanel = false;
            m_UnitGrid.IsActiveSortButton = true;
            m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
        }

        m_BaseUnit = UserDataAdmin.Instance.SearchUnitGridParam(MainMenuParam.m_BuildupBaseUnitUniqueId);
        if (m_BaseUnit != null)
        {
            MasterDataParamChara _master = m_BaseUnit.master;
            m_UnitStatusPanel.setupUnit(m_BaseUnit.unit);

            m_UnitBGPanel.setupBaseUnit(_master, m_BaseUnit.unit);
            m_UnitBGPanel.IsViewExecButton = true;
            m_UnitBGPanel.IsViewReturnButton = true;

            m_UnitBGPanel.IsActiveExecButton = IsBuildupStart();

            updateMaterialUnitList();

            // 限界突破対応リストを作成
            CharaLimitOver.SetEvolBaseUnitIdList(_master.fix_id);

            SetupBlendAfterStatus();

            MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un75p_description"));
        }
        else
        {
            m_UnitBGPanel.resetBaseUnit();
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;
            m_UnitBGPanel.Money = 0;
            m_UnitStatusPanel.reset();
            updateUnitBaseList();
        }

        if (TutorialManager.IsExists)
        {
            m_UnitBGPanel.IsViewReturnButton = false;
        }

        m_UnitBGPanel.IsEnableResetButton = (GetBuildupMaterialCount() == 0) ? false : true;
        m_ExpandWindow.AutoWindow = !TutorialManager.IsExists;
        m_UnitStatusPanel.togglePremium.interactable = !TutorialManager.IsExists;
    }

    /// <summary>
    /// ユニットリスト生成
    /// </summary>
    private void makeUnitList()
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

        if (TutorialManager.IsExists)
        {
            m_UnitGrid.SetUpTutorialSortData();
        }
        else
        {
            m_UnitGrid.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterBuildUnit());
        }
        m_UnitGrid.CreateList(unitList);
    }

    /// <summary>
    /// 素材ユニット選択更新
    /// </summary>
    public void updateMaterialUnitList()
    {
        m_UnitGrid.SetupUnitIconType = SetupMaterialUnitIconType;
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
    /// 素材設定
    /// </summary>
    /// <param name="_unit"></param>
    public void setMaterialUnit(UnitGridContext _unit)
    {
        _unit.IsSelectedUnit = (_unit.IsSelectedUnit) ? false : true;
        if (_unit.IsSelectedUnit)
        {
            for (int i = 0; i < BILDUP_MATERIAL_MAX; i++)
            {
                if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == _unit.UnitData.unique_id)
                {
                    return;
                }
            }

            for (int i = 0; i < BILDUP_MATERIAL_MAX; i++)
            {
                if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0)
                {
                    m_UnitMaterialPanel.MaterialList[i].IsViewIcon = true;
                    m_UnitMaterialPanel.MaterialList[i].m_CharaId = _unit.UnitData.id;
                    m_UnitMaterialPanel.MaterialList[i].m_UniqueId = _unit.UnitData.unique_id;
                    m_UnitMaterialPanel.SetViewIcon(i);

                    UnitIconImageProvider.Instance.Get(
                        _unit.UnitData.id,
                        sprite =>
                        {
                            m_UnitMaterialPanel.MaterialList[i].IconImage = sprite;
                        },
                        true);

                    SoundUtil.PlaySE(SEID.SE_MENU_OK);
                    return;
                }
            }
        }
        else
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
            sortMaterialList(_unit);
        }
    }

    /// <summary>
    /// 素材整理
    /// </summary>
    private void sortMaterialList(UnitGridContext _unit)
    {
        for (int i = 0; i < BILDUP_MATERIAL_MAX; i++)
        {
            var unit = m_UnitMaterialPanel.MaterialList[i];
            if (unit.m_UniqueId == _unit.UnitData.unique_id)
            {
                UnitIconImageProvider.Instance.Reset(unit.m_CharaId);
                unit.reset();
                m_UnitMaterialPanel.UnsetViewIcon(i);
            }
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        /// 次に設定されている素材取得
        System.Func<int, MaterialDataContext> getnextmaterial = (int _start) =>
        {
            for (int i = _start + 1; i < BILDUP_MATERIAL_MAX; i++)
            {
                if (m_UnitMaterialPanel.MaterialList[i].IsViewIcon)
                {
                    return m_UnitMaterialPanel.MaterialList[i];
                }
            }

            return null;
        };

        // 素材を詰める
        for (int i = 0; i < (BILDUP_MATERIAL_MAX - 1); i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].IsViewIcon)
            {
                continue;
            }

            MaterialDataContext _next = getnextmaterial(i);
            if (_next == null)
            {
                return;
            }

            m_UnitMaterialPanel.MaterialList[i].IsViewIcon = true;
            m_UnitMaterialPanel.MaterialList[i].m_CharaId = _next.m_CharaId;
            m_UnitMaterialPanel.MaterialList[i].m_UniqueId = _next.m_UniqueId;

            UnitIconImageProvider.Instance.Get(
                _next.m_CharaId,
                sprite =>
                {
                    m_UnitMaterialPanel.MaterialList[i].IconImage = sprite;
                },
                true);

            _next.reset();
        }
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

        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un74p_description"));

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        m_ExpandWindow.Close();
        m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
    }

    /// <summary>
    /// 素材として選択できるか
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckMaterialUnit(PacketStructUnit _unit)
    {
        // 強化元
        if (m_BaseUnit != null && m_BaseUnit.unique_id == _unit.unique_id)
        {
            return true;
        }

        if (TutorialManager.IsExists && _unit.unique_id != TutorialManager.MaterialUnitUniqueId)
        {
            return false;
        }

        // リンクチェック
        if (_unit.link_info != (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
        {
            return false;
        }

        UnitGridParam unitGridParam = UserDataAdmin.Instance.SearchUnitGridParam(_unit.unique_id);
        if (unitGridParam == null)
        {
            Debug.LogError("unique_id=" + _unit.unique_id + "  is failed!!!");
            return false;
        }

        // パーティチェック
        if (unitGridParam.party_assign)
        {
            return false;
        }

        // お気に入り
        if (unitGridParam.favorite)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ベースユニットとして選択できるか
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckBaseUnit(PacketStructUnit _unit)
    {
        if (TutorialManager.IsExists && _unit.unique_id != TutorialManager.BaseUnitUniqueId)
        {
            return false;
        }

        if (IsSelectMaterialUnit(_unit.unique_id))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 素材で選択されている？
    /// </summary>
    /// <param name="_unique_id"></param>
    /// <returns></returns>
    private bool IsSelectMaterialUnit(long _unique_id)
    {
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == _unique_id)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 素材設定数取得
    /// </summary>
    /// <returns></returns>
    private uint GetBuildupMaterialCount()
    {
        uint count = 0;
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0)
            {
                continue;
            }
            count++;
        }
        return count;
    }

    /// <summary>
    /// ユニット選択
    /// </summary>
    /// <param name="_unit"></param>
	private void SelectUnit(UnitGridContext _unit)
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
            if (m_BaseUnit == null)
            {
                if (!CheckBaseUnit(_unit.UnitData))
                {
                    if (TutorialManager.IsExists)
                    {
                        return;
                    }

                    selectMaterialUnit(_unit);

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
                //チュートリアル中は反応しない
                if (TutorialManager.IsExists)
                {
                    return;
                }

                //ベース解除
                PopBaseUnit();
                m_ExpandWindow.Close();
                m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
            }
            else
            {
                if (selectMaterialUnit(_unit) == false)
                {
                    return;
                }
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
        //チュートリアル中は反応しない
        if (TutorialManager.IsExists)
        {
            return;
        }

        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        unsetBaseUnit();
    }

    private void OnSelectBackKey()
    {
        PopBaseUnit();
    }

    private bool selectMaterialUnit(UnitGridContext _unit)
    {
        //素材設定
        if (!CheckMaterialUnit(_unit.UnitData))
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("cancel");
#endif
            return false;
        }

        if (!_unit.IsSelectedUnit && GetBuildupMaterialCount() >= BILDUP_MATERIAL_MAX)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("cancel");
#endif
            return false;
        }

        //チュートリアル中は一度選択されたものは解除しない
        if (TutorialManager.IsExists &&
            _unit.IsSelectedUnit)
        {
            return false;
        }

        //[DG0-2610] 【本番】強化素材の選択の際に、選択しているユニットに表示される枠の整合性が合わない
        // _unit.UnitData.unique_idがm_UnitMaterialPanel.MaterialList登録されているが。
        // IsSelectedUnitがfalseのケースだと発生しそうなので対処
        // 実際の発生プロセスは現状不明
        if (IsSelectMaterialUnit(_unit.UnitData.unique_id) &&
            _unit.IsSelectedUnit == false)
        {
            _unit.IsSelectedUnit = true;
        }

        setMaterialUnit(_unit);
        SetupBlendAfterStatus();

        m_UnitBGPanel.IsEnableResetButton = (GetBuildupMaterialCount() == 0) ? false : true;

        return true;
    }

    /// <summary>
    /// ユニット長押し
    /// </summary>
    /// <param name="_unit"></param>
    private void SelectUnitLongPress(UnitGridContext _unit)
    {
        //チュートリアル中はユニット詳細に遷移しない
        if (TutorialManager.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        if (MainMenuManager.HasInstance)
        {
            CreateUnitDetailInfo(_unit);
        }
    }

    /// <summary>
    /// ベースユニットアイコン選択
    /// </summary>
    private void SelectUnitIcon()
    {
        //チュートリアル中は反応しない
        if (TutorialManager.IsExists)
        {
            return;
        }
        //
        if (IsBusy())
        {
            return;
        }

        OnSelectBackKey();
    }


    private void SelectUnitLongPress()
    {
        if (MainMenuManager.HasInstance)
        {
            if (m_BaseUnit == null || m_BaseUnit.unit == null)
            {
                return;
            }

            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            CreateUnitDetailInfo(m_BaseUnit.unit);

        }
    }

    /// <summary>
    /// ユニット詳細情報を表示する.
    /// </summary>
    /// <param name="_unit">対象ユニットのコンテキスト情報</param>
    private void CreateUnitDetailInfo(UnitGridContext _unit)
    {
        UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo();
        if (_info == null) return;

        PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit.UnitData);
        _info.SetUnitFavorite(_unit.UnitData, _subUnit, _unit);
        _info.SetCloseAction(() =>
        {
            //素材選択されてるユニットがお気に入り登録されたら素材選択を解除する
            if (IsSelectMaterialUnit(_unit.UnitData.unique_id) &&
                _unit.IsActiveFavoriteImage)
            {
                //選択解除
                setMaterialUnit(_unit);
                //ステータス更新
                SetupBlendAfterStatus();
                //ボタン制御
                m_UnitBGPanel.IsEnableResetButton = (GetBuildupMaterialCount() == 0) ? false : true;
                m_UnitBGPanel.IsActiveExecButton = IsBuildupStart();
            }

            //素材選択シーケンスの場合はIconTypeを更新する
            if (m_BaseUnit != null)
            {
                SetupMaterialUnitIconType(_unit);
            }

            //更新データ反映
            m_UnitGrid.UpdateBaseItem(_unit);
        });
    }

    /// <summary>
    /// ユニット詳細情報を表示する(ベースユニット用).
    /// </summary>
    /// <param name="_unitData">対象ユニットの情報</param>
    private void CreateUnitDetailInfo(PacketStructUnit _unitData)
    {
        UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo();
        if (_info == null)
        {
            return;
        }

        UnitGridContext _unit = m_UnitGrid.SearchUnitBaseItem(_unitData.unique_id);
        PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unitData);
        _info.SetUnitFavorite(_unitData, _subUnit, _unit);
        _info.SetCloseAction(() =>
        {
            // 変更が画面に反映されないので、全更新
            m_UnitGrid.UpdateList();
        });
    }

    /// <summary>
    /// 戻るボタン選択
    /// </summary>
    private void SelectReturn()
    {
        //チュートリアル中は反応しない
        if (TutorialManager.IsExists)
        {
            return;
        }
        //
        if (IsBusy())
        {
            return;
        }

        OnSelectBackKey();
    }

    /// <summary>
    /// プレミアムボタン選択
    /// </summary>
    /// <param name="bFlag"></param>
    private void SelectPremium(bool bFlag)
    {
        //チュートリアル中はプレミアムボタン無効
        if (TutorialManager.IsExists)
        {
            return;
        }
        //
        if (IsBusy())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        m_Premium = bFlag;
        SetupBlendAfterStatus();
    }

    private void SelectReset()
    {
        //チュートリアル中はリセットボタン無効
        if (TutorialManager.IsExists)
        {
            return;
        }
        //
        if (IsBusy())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        ResetMaterialAll();
        if (m_BaseUnit != null)
        {
            updateMaterialUnitList();
        }
        else
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

        if (GetBuildupMaterialCount() == 0)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CALL IsBuildupStart:02");
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
            // 所持金チェック
            //----------------------------------------
            bool bLessMoney = false;
            if (!TutorialManager.IsExists && UserDataAdmin.Instance.m_StructPlayer.have_money < m_UnitBGPanel.Money)
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("CALL IsBuildupStart03:" + UserDataAdmin.Instance.m_StructPlayer.have_money + " MON:" + m_UnitBGPanel.Money);
#endif
                bLessMoney = true;
            }

            if (bLessMoney)
            {
                Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kyouka_impossible_title");
                newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "kyouka_impossible_content");
                newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                newDialog.EnableFadePanel();
                newDialog.Show();
                return;
            }

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
        else if (checkLimitEggWarning())
        {
            openWarningLimitEggDialog();
        }
        else
        {
            openBuildupDialog();
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
            else if (checkLimitEggWarning())
            {
                openWarningLimitEggDialog();
            }
            else
            {
                openBuildupDialog();
            }
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();
    }

    private void openWarningBuildupDialog()
    {
        Dialog _newDialog = Dialog.Create(DialogType.DialogIconList).SetStrongYes();
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "un78q_title");

        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0 ||
                m_UnitMaterialPanel.MaterialList[i].m_bWarning == false)
            {
                continue;
            }

            DialogIconItem iconItem = makeIconItem(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);
            if (iconItem == null)
            {
                continue;
            }

            _newDialog.IconList.Add(iconItem);
        }

        string mainText = "";
        if (m_BlendLevelMaxWarning)
        {
            mainText += GameTextUtil.GetText("un78q_content1") + "\n";
        }
        if (m_BlendPlusMaxWarning)
        {
            mainText += GameTextUtil.GetText("warning_limitover_08") + "\n";
        }
        if (m_BlendOverPlusWarning)
        {
            mainText += GameTextUtil.GetText("kyouka_text4") + "\n";
        }
        if (m_BlendLOverMaxWarning)
        {
            mainText += GameTextUtil.GetText("warning_limitover_06") + "\n";
        }
        if (m_BlendOverLOverWarning)
        {
            mainText += GameTextUtil.GetText("kyouka_text5") + "\n";
        }
        if (m_BlendLBSLvMaxWarning)
        {
            mainText += GameTextUtil.GetText("warning_limitover_07") + "\n";
        }
        if (m_BlendLinkPointMaxWarning)
        {
            mainText += GameTextUtil.GetText("un78q_content6") + "\n";
        }
        if (m_BlendRarityWarning)
        {
            mainText += GameTextUtil.GetText("un78q_content2") + "\n";
        }
        if (m_BlendMaterialLOverWarning)
        {
            mainText += GameTextUtil.GetText("warning_limitover_09") + "\n";
        }

        mainText += GameTextUtil.GetText("un78q_content5");
        _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
        _newDialog.SetDialogText(DialogTextType.UnderText, mainText);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            if (checkLimitEggWarning())
            {
                openWarningLimitEggDialog();
            }
            else
            {
                BuildupUnit();
            }
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();
    }

    private bool checkBuildupWarning()
    {
        if (m_BlendRarityWarning ||
            m_BlendLevelMaxWarning ||
            m_BlendLBSLvMaxWarning ||
            m_BlendLOverMaxWarning ||
            m_BlendPlusMaxWarning ||
            m_BlendLinkPointMaxWarning ||
            m_BlendMaterialLOverWarning ||
            m_BlendOverPlusWarning ||
            m_BlendOverLOverWarning)
        {
            return true;
        }

        return false;
    }

    private void openWarningLimitEggDialog()
    {
        Dialog _newDialog = Dialog.Create(DialogType.DialogIconList).SetStrongYes();

        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "warning_limitover_title");

        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0 ||
                m_UnitMaterialPanel.MaterialList[i].m_bEggWarning == false)
            {
                continue;
            }

            DialogIconItem iconItem = makeIconItem(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);
            if (iconItem == null)
            {
                continue;
            }

            _newDialog.IconList.Add(iconItem);
        }

        string mainText = "";
        if (m_BlendNoLOverEggWarning)
        {
            mainText = GameTextUtil.GetText("warning_limitover_02");
        }
        else
        {
            if (m_BlendLimitEggMaxWarning)
            {
                mainText = GameTextUtil.GetText("warning_limitover_01");
            }
            else if (m_BlendOverLimitEggWarning)
            {
                mainText = GameTextUtil.GetText("kyouka_text5");
            }
            else
            {
                if (m_BlendLimitEggWarning) mainText += GameTextUtil.GetText("warning_limitover_03");
                if (m_BlendRarityOverEggWarning) mainText += GameTextUtil.GetText("warning_limitover_04");
            }
        }

        mainText += "\n" + GameTextUtil.GetText("warning_limitover_05");

        _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
        _newDialog.SetDialogText(DialogTextType.UnderText, mainText);

        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            BuildupUnit();
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();

    }

    private bool checkLimitEggWarning()
    {
        if (m_BlendLimitEgg == false) return false;

        if (m_BlendNoLOverEggWarning) return true;
        if (m_BlendLimitEggWarning) return true;
        if (m_BlendRarityOverEggWarning) return true;
        if (m_BlendLimitEggMaxWarning) return true;
        if (m_BlendOverLimitEggWarning) return true;

        return false;
    }

    private void openBuildupDialog()
    {

        Dialog _newDialog = Dialog.Create(DialogType.DialogIconList).SetStrongYes();

        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "warning_limitover_10");

        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0)
            {
                continue;
            }

            DialogIconItem iconItem = makeIconItem(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);
            if (iconItem == null)
            {
                continue;
            }

            _newDialog.IconList.Add(iconItem);
        }

        _newDialog.SetDialogObjectEnabled(DialogObjectType.UnderText, true);
        _newDialog.SetDialogText(DialogTextType.UnderText, GameTextUtil.GetText("kyouka_text2"));

        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            BuildupUnit();
        });
        _newDialog.EnableFadePanel();
        _newDialog.Show();
    }

    private DialogIconItem makeIconItem(long unique_id)
    {
        PacketStructUnit partsUnit = UserDataAdmin.Instance.SearchChara(unique_id);
        if (partsUnit == null)
        {
            return null;
        }
        MasterDataParamChara partsMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)partsUnit.id);
        if (partsMaster == null)
        {
            return null;
        }

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
        return iconItem;
    }

    /// <summary>
    /// ユニット強化実行
    /// </summary>
	private void BuildupUnit()
    {
        if (ServerApi.IsExists)
        {
            return;
        }

        m_ExpandWindow.SetBackKey(false);
        AndroidBackKeyManager.Instance.DisableBackKey();
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL MainMenuUnitBuildUp#BuildupUnit");
#endif
        //素材
        long[] partsList = new long[BILDUP_MATERIAL_MAX];
        PacketStructUnit[] partsUnitList = new PacketStructUnit[BILDUP_MATERIAL_MAX];
        int _count = 0;
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId != 0 &&
                m_UnitMaterialPanel.MaterialList[i].IsViewIcon)
            {
                partsList[_count] = m_UnitMaterialPanel.MaterialList[i].m_UniqueId;
                partsUnitList[_count] = UserDataAdmin.Instance.SearchChara(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);
                _count++;
            }
        }

        //----------------------------------------
        // 演出用に通信処理を行う前の情報を保持しておく
        //----------------------------------------
        {
            //ベースユニット
            MainMenuParam.m_BlendBuildUpUnitPrev = new PacketStructUnit();
            MainMenuParam.m_BlendBuildUpUnitPrev.Copy(m_BaseUnit.unit);

            //素材ユニット
            MainMenuParam.m_BlendBuildUpParts.Release();
            for (int i = 0; i < _count; i++)
            {
                if (partsUnitList[i] == null)
                {
                    continue;
                }

                PacketStructUnit cUnit = new PacketStructUnit();
                cUnit.Copy(partsUnitList[i]);
                MainMenuParam.m_BlendBuildUpParts.Add(cUnit);
            }
        }

        //----------------------------------------
        // チュートリアル判定
        //----------------------------------------
        bool is_premium = m_Premium;

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL MainMenuUnitBuildUp#BuildupUnit SendPacketAPI_UnitBlendBuildUp");
#endif


        Action buildUpAction = () =>
        {
            ButtonBlocker.Instance.Block(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
            ServerDataUtilSend.SendPacketAPI_UnitBlendBuildUp(
                                                              m_BaseUnit.unique_id
                                                            , partsList
                                                            , MainMenuParam.m_BlendBuildEventSLV
                                                            , (MainMenuParam.m_BeginnerBoost != null) ? (int)MainMenuParam.m_BeginnerBoost.fix_id : 0
                                                            , (TutorialManager.IsExists) ? 1 : 0
                                                            , is_premium)
            .setSuccessAction(_data =>
            {
                resultSuccess(_data);
            })
            .setErrorAction(_data =>
            {

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
        // DG0-2733 Tutorial時、StructPlayer.renew_tutorial_step == 406 に更新される BUILDUP_PART6
        //----------------------------------------
        if (TutorialManager.IsExists)
        {
            UserDataAdmin.Instance.m_StructPlayer.renew_tutorial_step = (int)TutorialStep.BUILDUP_PART6;
        }

        long unBuildUpCharaUniqueID = 0;
        //uint unBuildUpType = (uint)BUILDUP_TYPE.BUILDUP_TYPE_RATE_1_00;

        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvUnitBlendBuildUp>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
        UserDataAdmin.Instance.ConvertPartyAssing();

        //unBuildUpType = _data.GetResult<RecvUnitBlendBuildUp>().result.blend_pattern;
        unBuildUpCharaUniqueID = _data.GetResult<RecvUnitBlendBuildUp>().result.blend_unit_unique;

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
        ResetMaterialAll();

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

        if (TutorialManager.IsExists)
        {
            TutorialManager.Instance.FinishBuildUp();
        }

        if (m_UnitResult != null)
        {
            m_UnitResult.Hide();
            m_UnitResult = null;
        }

        makeUnitList();
        for (int i = 0; i < BILDUP_MATERIAL_MAX; i++)
        {
            var unit = m_UnitMaterialPanel.MaterialList[i];
            UnitIconImageProvider.Instance.Reset(unit.m_CharaId);
            unit.reset();
        }

        m_UnitBGPanel.IsViewPanel = false;
        m_UnitGrid.IsActiveSortButton = true;

        updateBuildupStatus();
    }

    /// <summary>
    /// 全素材解除
    /// </summary>
	private void ResetMaterialAll()
    {
        for (int i = 0; i < m_UnitGrid.Units.Count; i++)
        {
            if (m_BaseUnit != null &&
                m_BaseUnit.unique_id == m_UnitGrid.Units[i].UnitData.unique_id)
            {
                continue;
            }
            m_UnitGrid.Units[i].IsSelectedUnit = false;
        }

        for (int i = m_UnitMaterialPanel.MaterialList.Count - 1; i >= 0; i--)
        {
            var unit = m_UnitMaterialPanel.MaterialList[i];
            UnitIconImageProvider.Instance.Reset(unit.m_CharaId);

            unit.reset();
            m_UnitMaterialPanel.UnsetViewIcon(i);
        }

        m_UnitBGPanel.IsActiveExecButton = false;
        m_UnitBGPanel.IsEnableResetButton = false;
        SetupBlendAfterStatus();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ステータス更新
		@note
	*/
    //----------------------------------------------------------------------------
    void SetupBlendAfterStatus()
    {
        float fBonusRateTotal = 1.0f;
        if (m_Premium) fBonusRateTotal = 3.0f;

        //int totalExp = 0;
        //-----------------------
        // 合成時のパラメータ数値表示部分を更新
        //-----------------------

        //パーツユニット設定
        PacketStructUnit[] Parts = new PacketStructUnit[GetBuildupMaterialCount()];
        {
            int _partsCount = 0;
            for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
            {
                if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0)
                {
                    continue;
                }
                Parts[_partsCount] = UserDataAdmin.Instance.SearchChara(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);
                _partsCount++;
            }
        }
        m_UnitMaterialPanel.resetWarning();

        MasterDataSkillLimitBreak cSkillLimitBreak;
        if (m_BaseUnit == null)
        {
            //-----------------------
            // 得られる経験値を算出
            //-----------------------
            //totalExp = GetTotalEXP( ref Parts , fBonusRateTotal , MasterDataDefineLabel.ElementType.NONE );
        }
        else
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

                // ベースユニットのLVMAXチェック
                bool bBaseUnitLevelMax = false;
                if (m_BaseUnit.level >= cBaseMasterData.level_max)
                {
                    bBaseUnitLevelMax = true;
                }
                // ベースユニットのLBSLvMAXチェック
                cSkillLimitBreak = BattleParam.m_MasterDataCache.useSkillLimitBreak(cBaseMasterData.skill_limitbreak);
                bool bBaseUnitLBSLvMax = false;
                if (cSkillLimitBreak != null &&
                    cBaseUnit.limitbreak_lv >= cSkillLimitBreak.level_max)
                {
                    bBaseUnitLBSLvMax = true;
                }
                // ベースユニットの限界突破MAXチェック
                int nLimitOverMaxLevel = (int)CharaLimitOver.GetParam(0, cBaseMasterData.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_LIMITOVER_MAX);
                bool bBaseUnitLOverMax = false;
                if (nLimitOverMaxLevel == cBaseUnit.limitover_lv)
                {
                    bBaseUnitLOverMax = true;
                }

                uint unBaseAtk = (uint)cCharaParam.m_CharaPow;
                uint unBaseHP = (uint)cCharaParam.m_CharaHP;

                //-----------------------
                // 合成費用を算出
                // 合成費用 = ( ベースキャラレベル * 100 * 素材数 ) + ( 関連キャラのプラス値合計 * 1000 )
                //-----------------------
                uint unPartsTotal = (uint)Parts.Length;
                uint unPartsPlus = 0;
                for (int i = 0; i < unPartsTotal; i++)
                {
                    unPartsPlus += (Parts[i].add_pow + Parts[i].add_hp);
                }

                if (unPartsTotal > 0)
                {
                    unPartsPlus += (cBaseUnit.add_hp + cBaseUnit.add_pow);
                }
                uint unBlendMoney = (cBaseUnit.level * 100 * unPartsTotal) + (unPartsPlus * GlobalDefine.PLUS_BUILD_COIN);

                //-----------------------
                // プラス値を算出
                //-----------------------
                uint unUnitAddPlusHp = 0;
                uint unUnitAddPlusAtk = 0;
                uint unUnitPlusHP = (uint)cCharaParam.m_CharaPlusHP;
                uint unUnitPlusAtk = (uint)cCharaParam.m_CharaPlusPow;

                bool bPlusHPMax = false;
                bool bPlusAtkMax = false;
                List<long> PartsPlusHP = new List<long>();
                List<long> PartsPlusAtk = new List<long>();
                m_BlendPlusMaxWarning = false;
                m_BlendOverPlusWarning = false;

                //プラス最大値チェック
                if (unUnitPlusHP == GlobalDefine.PLUS_MAX)
                {
                    bPlusHPMax = true;
                }
                if (unUnitPlusAtk == GlobalDefine.PLUS_MAX)
                {
                    bPlusAtkMax = true;
                }

                //プラス加算値の算出
                for (int i = 0; i < Parts.Length; ++i)
                {
                    if (Parts[i] == null)
                    {
                        continue;
                    }

                    unUnitAddPlusHp += Parts[i].add_hp;
                    //HPプラス加算素材に登録
                    if (Parts[i].add_hp != 0)
                    {
                        PartsPlusHP.Add(Parts[i].unique_id);
                    }

                    unUnitAddPlusAtk += Parts[i].add_pow;
                    //Atkプラス加算素材に登録
                    if (Parts[i].add_pow != 0)
                    {
                        PartsPlusAtk.Add(Parts[i].unique_id);
                    }
                }

                //プラス値の加算
                //HP
                if (unUnitAddPlusHp != 0)
                {
                    unUnitPlusHP += unUnitAddPlusHp;
                    if (unUnitPlusHP > GlobalDefine.PLUS_MAX)
                    {
                        unUnitPlusHP = GlobalDefine.PLUS_MAX;
                        if (bPlusHPMax)
                        {
                            //加算前にプラス値が最大値だった場合、警告する。
                            m_BlendPlusMaxWarning = true;
                        }
                        else
                        {
                            //加算後のプラス値が最大値を超えていた場合、警告する。
                            m_BlendOverPlusWarning = true;
                        }
                        //警告対象のユニットに登録
                        setMaterialWarningUnit(PartsPlusHP);
                    }
                }
                //Atk
                if (unUnitAddPlusAtk != 0)
                {
                    unUnitPlusAtk += unUnitAddPlusAtk;
                    if (unUnitPlusAtk > GlobalDefine.PLUS_MAX)
                    {
                        unUnitPlusAtk = GlobalDefine.PLUS_MAX;
                        if (bPlusAtkMax)
                        {
                            //加算前にプラス値が最大値だった場合、警告する。
                            m_BlendPlusMaxWarning = true;
                        }
                        else
                        {
                            //加算後のプラス値が最大値を超えていた場合、警告する。
                            m_BlendOverPlusWarning = true;
                        }
                        //警告対象のユニットに登録
                        setMaterialWarningUnit(PartsPlusAtk);
                    }
                }

                //-----------------------
                // ユニットが持ってる総合経験値を算出
                // ※今回の合成で得られる経験値含む
                //-----------------------
                int nTotalEXP = GetTotalEXP(ref Parts, fBonusRateTotal, cBaseMasterData.element);
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
                // 次のレベルまでの経験値を算出
                //-----------------------
                float expRatio = 0.0f;
                float buildupExpRatio = 0.0f;
                int nNextEXP = 0;
                {
                    int nNowLevelExp = CharaUtil.GetStatusValue(cBaseMasterData, (int)unUnitLevel, CharaUtil.VALUE.EXP);
                    int nNextLevelExp = CharaUtil.GetStatusValue(cBaseMasterData, (int)unUnitLevel + 1, CharaUtil.VALUE.EXP);
                    int nLevelupExp = nNextLevelExp - nNowLevelExp;
                    int nNextBuildupEXP = 0;
                    if (nNextLevelExp > nTotalEXPAfter)
                    {
                        nNextBuildupEXP = nNextLevelExp - nTotalEXPAfter;
                        nNextEXP = nNextLevelExp - nTotalEXPAfter;
                    }
                    if (m_BaseUnit.level == unUnitLevel && nLevelupExp != 0)
                    {
                        expRatio = (float)(nLevelupExp - nNextEXP) / (float)nLevelupExp;
                    }
                    if (nNextBuildupEXP != 0 && nLevelupExp != 0)
                    {
                        buildupExpRatio = (float)(nLevelupExp - nNextBuildupEXP) / (float)nLevelupExp;
                    }
                }

                //----------------------------------------
                // 初心者ブースト適用
                // 表示用の値を計算、補正値を適用
                //----------------------------------------
                if (MainMenuParam.m_BeginnerBoost != null
                && MainMenuParam.m_BeginnerBoost.boost_build_money != 100
                && unBlendMoney != 0)
                {
                    unBlendMoney = MasterDataUtil.ConvertBeginnerBoostBuildMoney(ref MainMenuParam.m_BeginnerBoost, unBlendMoney);
                }

                //-----------------------
                // HPとATKの表示形式とカラーの設定
                // 強化後のHP/ATK、プラス値更新があったら色つける
                //-----------------------
                //フォーマット
                string strFormatUp = GameTextUtil.GetText("kyouka_text1");          //値上昇時
                string strFormatMax = GameTextUtil.GetText("unit_status15");            //値MAX
                string strFormat = GameTextUtil.GetText("unit_status15");            //値変更なし
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
                m_BlendLevelMaxWarning = false;
                m_BlendLBSLvMaxWarning = false;
                m_BlendLOverMaxWarning = false;
                m_BlendOverLOverWarning = false;
                m_BlendLinkPointMaxWarning = false;
                m_BlendRarityWarning = false;

                m_BlendMaterialLOverWarning = false;

                m_BlendLimitEgg = false;
                m_BlendLimitEggWarning = false;
                m_BlendRarityOverEggWarning = false;
                m_BlendNoLOverEggWarning = false;
                m_BlendLimitEggMaxWarning = false;
                m_BlendOverLimitEggWarning = false;

                // 限界突破レベル
                int nLimitOverCount = 0;
                List<long> PartsLOver = new List<long>();
                List<long> PartsLEgg = new List<long>();

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
                bool bLinkPointMax = false;
                if (unBaseLinkPoint >= CharaLinkUtil.LINK_POINT_MAX)
                {
                    bLinkPointMax = true;
                }

                uint limitbreakLv = cBaseUnit.limitbreak_lv;
                int limitbreakLvMax = 0;
                if (cSkillLimitBreak != null)
                {
                    limitbreakLvMax = cSkillLimitBreak.level_max;
                }
                for (int i = 0; i < Parts.Length; i++)
                {
                    if (Parts[i] == null)
                    {
                        continue;
                    }

                    MasterDataParamChara cPartsMasterData = MasterFinder<MasterDataParamChara>.Instance.Find((int)Parts[i].id);

                    // 限界突破の増加量設定
                    int nAddValue = 1;

                    //同一スキル判定
                    bool bSkillUpParts = false;
                    bool bLimitSkillLvUp = false;
                    if (cPartsMasterData.skill_limitbreak != 0 &&
                        cPartsMasterData.skill_limitbreak == cBaseMasterData.skill_limitbreak)
                    {
                        bSkillUpParts = true;
                        //スキルLV最大警告
                        if (bBaseUnitLBSLvMax)
                        {
                            m_BlendLBSLvMaxWarning = true;
                            m_UnitMaterialPanel.setMaterialWarning(Parts[i].unique_id, true);
                        }

                        /*
                         * 同一スキルのActiveSkillLevel増加値算出
                         * 1.ユニットのスキルレベルに応じたレート作成
                         * スキルレベルアップレート　＝　基本値　＊イベント補正値（1.5、2.0、2.5、3.0、4.0、5.0、10.0）
                         * ※スキルアップレートの最大値は100まで（100分率）
                         *　基本値について
                         *　該当スキルのレベルMAX値と、対象ユニットの現在のスキルレベルの差に応じて変わる
                         *　レベル差が・・
                         *　　1~10:  10
                         *　　11~16: 20
                         *　　17~18: 30
                         *　　19~99: 40
                         */

                        // 基本値を算出する.
                        var subLevel = cSkillLimitBreak.level_max - limitbreakLv;
                        var limitSkillUpBaseRate = 0;
                        if (subLevel >= GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL1_LOW &&
                            subLevel <= GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL1_HIGH)
                        {
                            limitSkillUpBaseRate = GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_RATE_1;
                        }
                        else if (subLevel >= GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL2_LOW &&
                                 subLevel <= GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL2_HIGH)
                        {
                            limitSkillUpBaseRate = GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_RATE_2;
                        }
                        else if (subLevel >= GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL3_LOW &&
                                 subLevel <= GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL3_HIGH)
                        {
                            limitSkillUpBaseRate = GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_RATE_3;
                        }
                        else if (subLevel >= GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL4_LOW &&
                                 subLevel <= GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_SUBLEVEL4_HIGH)
                        {
                            limitSkillUpBaseRate = GlobalDefine.BUILDUP_LIMITBREAKSKILL_BASE_RATE_4;
                        }

                        // 基本値とイベント補正値を掛け合わせてスキルアップレートの一定値を超えるか判定する.
                        if (limitSkillUpBaseRate * MainMenuUtil.GetBuildUpEventRate() >= GlobalDefine.BUILDUP_SAMESKILL_SKILL_RATE_MAX)
                        {
                            bLimitSkillLvUp = true;
                            limitbreakLv = (uint)Mathf.Min(limitbreakLv + 1, cSkillLimitBreak.level_max);
                        }
                    }

                    //Sエッグ判定
                    if (cPartsMasterData.skill_plus != 0 &&
                        ((int)cBaseMasterData.element == (int)cPartsMasterData.skill_plus_element ||
                        cPartsMasterData.skill_plus_element == MasterDataDefineLabel.SkillLevelElementType.ALL))
                    {
                        bSkillUpParts = true;
                        //スキルLV最大警告
                        if (bBaseUnitLBSLvMax)
                        {
                            m_BlendLBSLvMaxWarning = true;
                            m_UnitMaterialPanel.setMaterialWarning(Parts[i].unique_id, true);
                        }

                        /*
                         * ActiveSkillLevelの増加値算出.
                         *
                         * ■スキルエッグを利用
                         * 1.スキルエッグに応じたレート作成
                         * スキルレベルアップレート＝　スキルエッグの基本値＊イベント補正値（1.5、2.0、2.5、3.0、4.0、5.0、10.0）
                         * ※スキルアップレートの最大値は10000まで（10000分率）
                         * 　スキルエッグの基本値について
                         * 該当スキルエッグのchara_master:skill_plusの値を参照
                         * 　　○○スキルエッグ: 400
                         * ハイ○○スキルエッグ: 2000
                         * キング○○スキルエッグ: 10000
                         *
                         */
                        if (bLimitSkillLvUp == false &&
                            cSkillLimitBreak != null &&
                            cBaseMasterData.skill_limitbreak > 0)
                        {
                            if (cPartsMasterData.skill_plus * MainMenuUtil.GetBuildUpEventRate() >= GlobalDefine.BUILDUP_SKILLEGG_RATE_MAX)
                            {
                                limitbreakLv = (uint)Mathf.Min(limitbreakLv + 1, cSkillLimitBreak.level_max);
                            }

                        }

                    }

                    //限突エッグを使用したフラグ
                    bool bLimitEgg = false;
                    if (cPartsMasterData.limit_over_synthesis_type != MasterDataDefineLabel.LimitOverSynthesisType.NONE)
                    {
                        bLimitEgg = true;

                        m_BlendLimitEgg = true;

                    }

                    //限界突破可能か？
                    bool bLOverParts = false;
                    if (nLimitOverMaxLevel > 0)
                    {
                        // 限界突破対応のユニットか検索
                        bool bCheckLimitOver = CharaLimitOver.CheckEvolUnit(cPartsMasterData.fix_id);

                        // 限突エッグのチェック
                        bool bLimitOverEggElement = false;
                        switch (cPartsMasterData.limit_over_synthesis_type)
                        {
                            case MasterDataDefineLabel.LimitOverSynthesisType.NONE:
                                break;
                            case MasterDataDefineLabel.LimitOverSynthesisType.ALL:
                                {
                                    //無制限
                                    bLimitOverEggElement = true;
                                }
                                break;
                            case MasterDataDefineLabel.LimitOverSynthesisType.RARITY:
                                {
                                    //レアリティ制限
                                    if ((int)cBaseMasterData.rare <= cPartsMasterData.limit_over_param1)
                                    {
                                        bLimitOverEggElement = true;
                                        if ((int)cBaseMasterData.rare < cPartsMasterData.limit_over_param1)
                                        {
                                            //レアリティオーバー警告
                                            m_BlendRarityOverEggWarning = true;
                                            m_UnitMaterialPanel.setEggWarning(Parts[i].unique_id, true);
                                        }
                                    }
                                    else
                                    {
                                        //条件未達警告
                                        m_BlendLimitEggWarning = true;
                                        m_UnitMaterialPanel.setEggWarning(Parts[i].unique_id, true);
                                    }
                                }
                                break;
                            case MasterDataDefineLabel.LimitOverSynthesisType.UNIT:
                                {
                                    //ユニット制限
                                    if (cBaseMasterData.fix_id == cPartsMasterData.limit_over_param1 ||
                                        CharaLimitOver.CheckEvolUnit((uint)cPartsMasterData.limit_over_param1))
                                    {
                                        bLimitOverEggElement = true;
                                    }
                                    else
                                    {
                                        //条件未達警告
                                        m_BlendLimitEggWarning = true;
                                        m_UnitMaterialPanel.setEggWarning(Parts[i].unique_id, true);
                                    }

                                }
                                break;
                        }

                        //限突エッグの場合は増加量を設定値に
                        if (bLimitOverEggElement)
                        {
                            // 設定した増加量を加算
                            nAddValue = cPartsMasterData.limit_over_value;

                            //限界Egg素材として登録
                            PartsLEgg.Add(Parts[i].unique_id);
                        }

                        // 条件のいずれかが一致していたら限界突破のカウントをする
                        if (bLimitOverEggElement ||
                            bCheckLimitOver ||
                            cPartsMasterData.fix_id == cBaseMasterData.fix_id)
                        {
                            // 限界突破パーツフラグ
                            bLOverParts = true;

                            // 増加量を加算
                            nLimitOverCount += nAddValue;

                            // 素材が限界突破済みキャラだった場合はその数値分加算
                            nLimitOverCount += (int)Parts[i].limitover_lv;

                            if (bLimitEgg == false)
                            {
                                //限界突破素材として登録
                                PartsLOver.Add(Parts[i].unique_id);
                            }
                        }
                    }
                    else
                    {
                        //限界突破しないユニットに限突エッグを餌にした場合は警告する。
                        if (cPartsMasterData.limit_over_synthesis_type != MasterDataDefineLabel.LimitOverSynthesisType.NONE)
                        {
                            m_BlendNoLOverEggWarning = true;
                            m_UnitMaterialPanel.setEggWarning(Parts[i].unique_id, true);
                        }
                    }

                    //限界突破しない素材なのに、素材が限界突破している警告
                    if (bLOverParts == false &&
                        Parts[i].limitover_lv > 0)
                    {
                        m_UnitMaterialPanel.setMaterialWarning(Parts[i].unique_id, true);
                        m_BlendMaterialLOverWarning = true;
                    }

                    //ついでにリンクポイント素材か判定
                    bool bLinkPointParts = false;
                    if (cPartsMasterData.material_link_point > 0)
                    {
                        bLinkPointParts = true;
                        //リンクポイントが最大の場合は警告する。
                        if (bLinkPointMax)
                        {
                            m_BlendLinkPointMaxWarning = true;
                            m_UnitMaterialPanel.setMaterialWarning(Parts[i].unique_id, true);
                        }
                    }

                    //ベースユニットが最大LVでスキル・限界突破・リンクポイントも変化しない場合は警告する。(限突エッグ除外)
                    if (bBaseUnitLevelMax &&
                        bSkillUpParts == false &&
                        bLOverParts == false &&
                        bLinkPointParts == false &&
                        bLimitEgg == false)
                    {
                        m_BlendLevelMaxWarning = true;
                        m_UnitMaterialPanel.setMaterialWarning(Parts[i].unique_id, true);
                    }

                    //レアリティ４以上の場合は警告する。
                    if ((int)cPartsMasterData.rare >= (int)MasterDataDefineLabel.RarityType.STAR_4)
                    {
                        m_BlendRarityWarning = true;
                        m_UnitMaterialPanel.setMaterialWarning(Parts[i].unique_id, true);
                    }

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

                        //限界突破素材が使用されている場合、警告
                        if (PartsLOver.Count != 0)
                        {
                            m_BlendLOverMaxWarning = true;
                            setMaterialWarningUnit(PartsLOver);
                        }
                        //限突Eggが使用されている場合、警告
                        if (PartsLEgg.Count != 0)
                        {
                            m_BlendLimitEggMaxWarning = true;
                            setEggWarningUnit(PartsLEgg);
                        }
                    }
                    // 限界突破の上限値を超える場合
                    else if (nLimitOverMaxLevel < nLimitOverLevel)
                    {
                        m_BlendLimitOverResult = (int)CharaLimitOver.RESULT_TYPE.eValueOver;

                        //限界突破素材が使用されている場合、警告
                        if (PartsLOver.Count != 0)
                        {
                            m_BlendOverLOverWarning = true;
                            setMaterialWarningUnit(PartsLOver);
                        }
                        //限突Eggが使用されている場合、警告
                        if (PartsLEgg.Count != 0)
                        {
                            m_BlendOverLimitEggWarning = true;
                            setEggWarningUnit(PartsLEgg);
                        }
                    }
                    // 問題なし
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


                if (unBuildUpHP != unBaseHP ||
                     nLimitOverLevel != m_BaseUnit.limitover_lv)
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

                if (unBuildUpAtk != unBaseAtk ||
                     nLimitOverLevel != m_BaseUnit.limitover_lv)
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
                        strBuildUpLevel = string.Format(GameTextUtil.GetText("kyouka_text3"), cCharaParam.m_CharaLevel, unUnitLevel, cCharaParam.m_CharaMasterDataParam.level_max);
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
                m_UnitStatusPanel.NextExp = nNextEXP;
                m_UnitStatusPanel.ExpRate = expRatio;
                m_UnitStatusPanel.BuildupExpRate = buildupExpRatio;
                m_UnitStatusPanel.Charm = strBuilupCharm;
                m_UnitBGPanel.Money = (int)unBlendMoney;

                // ActiveSkillLevelの表示
                var strFormatAslv = "";
                if (limitbreakLv > cBaseUnit.limitbreak_lv)
                {
                    strFormatAslv = GameTextUtil.GetText("kyouka_text1");
                }
                else
                {
                    strFormatAslv = GameTextUtil.GetText("unit_status15");
                }

                // ActiveSkillLevelが設定されている場合のみ表示レベルを設定する.
                if (cBaseMasterData.skill_limitbreak > 0)
                {
                    limitbreakLv += 1;
                }
                else
                {
                    limitbreakLv = 0;
                }
                m_UnitStatusPanel.Aslv = string.Format(strFormatAslv, limitbreakLv);

#if BUILD_TYPE_DEBUG
                Debug.Log("exp:" + expRatio.ToString() + " buildupExp:" + buildupExpRatio.ToString());
#endif
            }
            else
            {
#if BUILD_TYPE_DEBUG
                Debug.LogError("MasterData None!");
#endif
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ユニット詳細表示
		@note
	*/
    //----------------------------------------------------------------------------
    static private int GetTotalEXP(ref PacketStructUnit[] racUnitList, float fBonusRate, MasterDataDefineLabel.ElementType nBaseUnitElement)
    {
        //-----------------------
        // 普通に合成した際にから得られる経験値を算出
        //-----------------------
        int nTotalEXP = 0;
        for (int j = 0; j < racUnitList.Length; j++)
        {
            if (racUnitList[j] == null) continue;

            MasterDataParamChara cMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)racUnitList[j].id);

            int nDefalutUnitExp = CharaUtil.GetStatusValue(cMaster
                                        , (int)racUnitList[j].level
                                        , CharaUtil.VALUE.EXP_PARTS);

            int nUnitExp = nDefalutUnitExp;

            //-----------------------
            // 属性一致かワイルドエッグで経験値を1.5倍
            //-----------------------
            if (nBaseUnitElement != MasterDataDefineLabel.ElementType.NONE
            && nBaseUnitElement == cMaster.element
            || cMaster.wild_egg_flg == MasterDataDefineLabel.BoolType.ENABLE
            )
            {
                nUnitExp = (int)(nUnitExp * 1.5f);
            }

            nTotalEXP += nUnitExp;

            if (racUnitList[j].limitover_lv > 0)
            {
                //-----------------------
                // 限界突破最大レベル
                //-----------------------
                double dLimitOverMaxLevel = CharaLimitOver.GetParam(0, cMaster.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_LIMITOVER_MAX);

                //-----------------------
                // 現在の限界突破レベル
                //-----------------------
                double dLimitOverLevel = (double)racUnitList[j].limitover_lv;

                //-----------------------
                // 限界突破ボーナス分の経験値を追加
                //-----------------------
                double dLoBounsExp = CharaLimitOver.GetParamBuildUpLimitOverBouns(dLimitOverLevel, dLimitOverMaxLevel, nDefalutUnitExp);

                nTotalEXP += (int)dLoBounsExp;
            }
        }

        //-----------------------
        // ボーナス倍率をかける
        //-----------------------
        nTotalEXP = (int)(nTotalEXP * fBonusRate);

        return nTotalEXP;
    }

    /// <summary>
    /// ソートダイアログを開く
    /// </summary>
    void OnClockSortButton()
    {
        //チュートリアル中は開かない
        if (TutorialManager.IsExists)
        {
            return;
        }

        //
        if (IsBusy())
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
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterBuildUnit());
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
        LocalSaveManager.Instance.SaveFuncSortFilterBuildUnit(sortInfo);

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

        //素材に選択されている
        if (IsSelectMaterialUnit(_unit.unique_id))
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

    void SetupMaterialUnitIconType(UnitGridContext _unit)
    {
        //素材として選択できるか？
        if (CheckMaterialUnit(_unit.UnitData))
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
        }
        else
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON;
        }
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="bBack"></param>
    /// <returns></returns>
    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        //ユニットパラメータが作成されるまで待つ
        if (UserDataAdmin.Instance.m_bThreadUnitParam) return true;

        // インジケーターを閉じる(チュートリアルの時)
        if (TutorialManager.Instance != null &&
             TutorialManager.IsExists == true)
        {
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingFinish();
            }
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

    private void setMaterialWarningUnit(List<long> Parts)
    {
        for (int i = 0; i < Parts.Count; ++i)
        {
            if (Parts[i] == 0)
            {
                continue;
            }

            m_UnitMaterialPanel.setMaterialWarning(Parts[i], true);
        }

    }

    private void setEggWarningUnit(List<long> Parts)
    {
        for (int i = 0; i < Parts.Count; ++i)
        {
            if (Parts[i] == 0)
            {
                continue;
            }

            m_UnitMaterialPanel.setEggWarning(Parts[i], true);
        }

    }
}
