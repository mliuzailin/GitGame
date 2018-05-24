using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuUnitList : MainMenuSeq
{
    private UnitGridComplex m_UnitGrid = null;

    PageTitle m_PageTitle = null;

    // Use this for initialization
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


    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        //ユニットパラメータが作成されるまで待つ
        if (UserDataAdmin.Instance.m_bThreadUnitParam)
        {
            return true;
        }

        return false;
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        m_PageTitle = m_CanvasObj.GetComponentInChildren<PageTitle>();
        if (m_PageTitle != null)
        {
            m_PageTitle.Title = GameTextUtil.GetText("unit_shojititle");
            m_PageTitle.SetPositionAjustStatusBar(new Vector2(0, -152));
        }

        //ページ初期化処理
        if (m_UnitGrid == null)
        {
            //ユニットグリッド取得
            m_UnitGrid = m_CanvasObj.GetComponentInChildren<UnitGridComplex>();
            //サイズ設定
            m_UnitGrid.SetPositionAjustStatusBar(new Vector2(0, -25), new Vector2(-48, -295));

            m_UnitGrid.AttchUnitGrid<UnitGridView>(UnitGridView.Create());
        }

        updateUnitList();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.UNIT;
    }

    /// <summary>
    /// ユニットリスト更新
    /// </summary>
    private void updateUnitList()
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

        m_UnitGrid.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterUnitList());
        m_UnitGrid.CreateList(unitList);
    }

    /// <summary>
    /// ユニット選択
    /// </summary>
    /// <param name="_unit"></param>
    private void SelectUnit(UnitGridContext _unit)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        //ユニット詳細画面へ
        openUnitDetailInfo(_unit);
    }

    /// <summary>
    /// ユニット長押し
    /// </summary>
    /// <param name="_unit"></param>
    private void SelectUnitLongPress(UnitGridContext _unit)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        //ユニット詳細画面へ
        openUnitDetailInfo(_unit);
    }

    /// <summary>
    /// ユニット詳細画面（お気に入りあり）
    /// </summary>
    /// <param name="_unit"></param>
    private void openUnitDetailInfo(UnitGridContext _unit)
    {
        if (MainMenuManager.HasInstance)
        {
            UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo();
            if (_info == null) return;
            PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit.UnitData);
            _info.IsViewCharaCount = true;
            _info.SetUnitFavorite(_unit.UnitData, _subUnit, _unit);
            _info.SetCloseAction(() =>
            {
                // 更新データ反映
                m_UnitGrid.UpdateBaseItem(_unit);
            });
        }
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
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.UNIT);
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterUnitList());
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
        LocalSaveManager.Instance.SaveFuncSortFilterUnitList(sortInfo);

        m_UnitGrid.ExecSortBuild(sortInfo);
    }

    void OnClickSortThread(LocalSaveSortInfo sortInfo)
    {
        m_UnitGrid.ExecSortOnly(sortInfo);
    }
}
