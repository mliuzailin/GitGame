using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuUnitPointEvolve : MainMenuSeq
{
    private UnitBGPanel m_UnitBGPanel = null;
    private UnitStatusPanel m_UnitStatusPanel = null;
    private UnitGridComplex m_UnitGrid = null;
    private ExpandWindow m_ExpandWindow = null;


    private PacketStructUnit m_BaseUnit = null;
    private MasterDataParamChara m_BaseCharaMaster = null;
    private MasterDataParamCharaEvol m_CharaEvol = null;
    private MasterDataParamChara m_AfterCharaMaster = null;
    private uint m_BlendPoint = 0;
    private bool m_Validate = false;
    private bool m_IsProductID = false;
    private UnitResult m_UnitResult = null;

    private Sprite m_ConfirmSprite = null;
    private Sprite m_DecideSprite = null;

    private Dialog m_MaxDialog = null;

    private int m_UnitListCount = 0;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_ConfirmSprite = ResourceManager.Instance.Load("confirm_button");
        m_DecideSprite = ResourceManager.Instance.Load("s_button");
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
                if (shop_product[id].product_type == MasterDataDefineLabel.PointShopType.EVOL)
                {
                    MainMenuParam.m_PointShopEvolProductID = shop_product[id].fix_id;
                    break;
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

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        if (m_UnitBGPanel == null)
        {
            m_UnitBGPanel = m_CanvasObj.GetComponentInChildren<UnitBGPanel>();
            m_UnitBGPanel.SetPositionAjustStatusBar(new Vector2(0, 40), new Vector2(0, -345));
            m_UnitBGPanel.IsViewPointEvolve = true;
            m_UnitBGPanel.IsViewEvolve = true;
            m_UnitBGPanel.IsViewResetButton = false;
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;
            m_UnitBGPanel.Title = GameTextUtil.GetText("unit_function_evolve");
            m_UnitBGPanel.TotalTitle = GameTextUtil.GetText("unit_function_point");
            m_UnitBGPanel.Evolve_arrow = GameTextUtil.GetText("unit_function_evolve_arrow");
            m_UnitBGPanel.TotalPoint = (int)UserDataAdmin.Instance.m_StructPlayer.have_unit_point;

            m_UnitBGPanel.DidSelect = SelectEvolve;
            m_UnitBGPanel.DidReturn = SelectReturn;
            m_UnitBGPanel.DidSelectIcon = SelectUnitIcon;
            m_UnitBGPanel.DidSelectEvolveIcon = SelectEvolveUnitIcon;
            m_UnitBGPanel.DidSelectEvolveIconLongpress = SelectEvolveUnitIcon;
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

        updateEvolveStatus(true);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.UNIT;
    }

    /// <summary>
    /// 進化画面更新
    /// </summary>
    public void updateEvolveStatus(bool bRenew = false)
    {
        if (bRenew)
        {
            //ユニットリスト作成
            makeUnitList();
            //ウインドウ閉じる
            m_ExpandWindow.Close(true);
            m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
            //パネルを消す
            m_UnitBGPanel.IsViewPanel = false;
            //ソートボタン有効化
            m_UnitGrid.IsActiveSortButton = true;
            //確認OFF
            m_Validate = false;
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

        m_BaseUnit = UserDataAdmin.Instance.SearchChara(MainMenuParam.m_EvolveBaseUnitUniqueId);
        if (m_BaseUnit != null)
        {
            m_BaseCharaMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)m_BaseUnit.id);
            //進化前アイコン
            m_UnitBGPanel.setupBaseUnit(m_BaseCharaMaster, m_BaseUnit);

            m_CharaEvol = MasterDataUtil.GetCharaEvolParamFromCharaID(m_BaseUnit.id);
            m_AfterCharaMaster = MasterFinder<MasterDataParamChara>.Instance.Find((int)m_CharaEvol.unit_id_after);
            //進化後アイコン
            m_UnitBGPanel.setupEvolveUnit(m_AfterCharaMaster, m_BaseUnit);
            //進化後名前
            m_UnitStatusPanel.setup(m_AfterCharaMaster, m_BaseUnit);

            //ボタン関連ON
            m_UnitBGPanel.IsViewExecButton = true;
            m_UnitBGPanel.IsViewReturnButton = true;
            m_UnitBGPanel.ExecButtonImage = m_ConfirmSprite;

            m_UnitBGPanel.IsActiveExecButton = IsEvolveStart();

            //ベース選択後リスト更新
            updateBaseUnitAfterList();

            SetupBlendAfterStatus();
        }
        else
        {
            //表示リセット
            m_UnitBGPanel.Point = 0;
            m_UnitBGPanel.resetBaseUnit();
            m_UnitBGPanel.resetEvolveUnit();
            m_UnitStatusPanel.reset();

            //ボタン関連OFF
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;

            //ベース選択リスト更新
            updateBaseUnitList();
        }

    }

    /// <summary>
    /// ユニットリスト生成
    /// </summary>
    private void makeUnitList()
    {
        List<UnitGridContext> unitList = MainMenuUtil.MakeUnitGridContextListUnitPoint();
        if (unitList == null)
        {
            Debug.LogError("unitlist is null");
            return;
        }

        m_UnitGrid.OnClickSortButtonAction = OnClockSortButton;
        m_UnitGrid.ClickUnitAction = SelectUnit;
        m_UnitGrid.LongPressUnitAction = SelectUnitLongPress;
        m_UnitGrid.SetupUnitSelected = SetupUintSelected;

        m_UnitGrid.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterUnitPointEvolve());
        m_UnitGrid.CreateList(unitList);

        m_UnitListCount = unitList.Count;
    }

    /// <summary>
    /// ベースユニット選択後更新
    /// </summary>
    public void updateBaseUnitAfterList()
    {
        m_UnitGrid.SetupUnitIconType = SetupBaseUnitAfterIconType;
        m_UnitGrid.UpdateList();
    }

    /// <summary>
    /// ベースユニット選択更新
    /// </summary>
    public void updateBaseUnitList()
    {
        m_UnitGrid.SetupUnitIconType = SetupBaseUnitIconType;
        m_UnitGrid.UpdateList();
    }

    /// <summary>
    /// ベースユニット解除
    /// </summary>
    private void unsetBaseUnit()
    {
        MainMenuParam.m_EvolveBaseUnitUniqueId = 0;
        updateEvolveStatus();
        //ウインドウ閉じる
        m_ExpandWindow.Close();
        m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
    }

    /// <summary>
    /// ユニット選択フラグ設定
    /// </summary>
    /// <param name="_unique_id"></param>
    /// <param name="bFlag"></param>
    private void setUnitSelectFlag(long _unique_id, bool bFlag)
    {
        for (int i = 0; i < m_UnitGrid.Units.Count; i++)
        {
            if (m_UnitGrid.Units[i].UnitData.unique_id == _unique_id)
            {
                m_UnitGrid.Units[i].IsSelectedUnit = bFlag;
            }
        }
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
        m_UnitBGPanel.Message = string.Format(GameTextUtil.GetText("collaboevo_text"), m_BlendPoint);
        m_UnitBGPanel.IsViewPanel = bFlag;
        m_UnitBGPanel.ExecButtonImage = bFlag ? m_DecideSprite : m_ConfirmSprite;
        m_UnitGrid.IsActiveSortButton = !bFlag;
        m_Validate = bFlag;
    }

    private void backKeyValidate()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        setValidate(false);
    }

    /// <summary>
    /// ベース選択で選択できるユニットかどうか
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckBaseUnit(PacketStructUnit _unit)
    {
        //リンクしている・されている
        if (_unit.link_info != (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE) return false;
        //進化先があるか？
        UnitGridParam unitGridParam = UserDataAdmin.Instance.SearchUnitGridParam(_unit.unique_id);
        if (!unitGridParam.evolve) return false;

        return true;
    }

    /// <summary>
    /// ベース選択で進化条件がそろっているユニットかどうか
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckBaseUnitEvolve(PacketStructUnit _unit)
    {
        MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit.id);
        if (_master == null) return false;
        // レベルチェック
        if (_unit.level != _master.level_max) return false;
        int blendPoint = _master.evol_unitpoint;
        // ユニットポイントチェック
        if (blendPoint > UserDataAdmin.Instance.m_StructPlayer.have_unit_point) return false;

        return true;
    }

    private bool IsEvolveStart()
    {
        //ベース設定されていない
        if (m_BaseUnit == null) return false;

        return true;
    }

    /// <summary>
    /// 決定ボタン選択
    /// </summary>
    private void SelectEvolve()
    {
        SoundUtil.PlaySE(m_Validate
            ? SEID.SE_MENU_OK2
            : SEID.SE_MENU_OK);
        {
            //--------------------------
            // 合成可能レベルチェック
            //--------------------------
            bool bLessLevel = false;
            if (m_BaseUnit.level != m_BaseCharaMaster.level_max)
            {
                bLessLevel = true;
            }
            //--------------------------
            // 所持ポイントチェック
            //--------------------------
            bool bLessPoint = false;
            if (m_BlendPoint > UserDataAdmin.Instance.m_StructPlayer.have_unit_point)
            {
                bLessPoint = true;
            }

            if (bLessLevel || bLessPoint)
            {
                Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "shinka_impossible_title");
                string mainText = "";
                if (bLessLevel) mainText += GameTextUtil.GetText("shinka_impossible_content2") + "\n";
                if (bLessPoint) mainText += GameTextUtil.GetText("unitpoint_impossible_content");
                newDialog.SetDialogText(DialogTextType.MainText, mainText);
                newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                newDialog.EnableFadePanel();
                newDialog.Show();
                return;
            }
        }
        if (!m_Validate)
        {
            setValidate(true);
            return;
        }
        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        m_ExpandWindow.SetBackKey(false);
        AndroidBackKeyManager.Instance.DisableBackKey();

        EvolveUnit();
    }

    /// <summary>
    /// ユニット進化実行
    /// </summary>
    private void EvolveUnit()
    {
        //----------------------------------------
        // 演出用に通信処理を行う前の情報を保持しておく
        //----------------------------------------
        {
            MainMenuParam.m_EvolveBaseBefore = new PacketStructUnit();
            MainMenuParam.m_EvolveBaseBefore.Copy(m_BaseUnit);

            MainMenuParam.m_EvolveParts.Release();
        }

        ButtonBlocker.Instance.Block(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
        //ユニット進化送信
        ServerDataUtilSend.SendPacketAPI_PointShopEvol(
                                                         MainMenuParam.m_PointShopEvolProductID
                                                        , m_BaseUnit.unique_id
                                                        , m_CharaEvol.unit_id_after
                                                        , m_CharaEvol.fix_id)
        .setSuccessAction(_data =>
        {
            resultSuccess(_data);
        })
        .setErrorAction(_data =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("request Error : " + _data.m_PacketCode.ToString());
#endif
            AndroidBackKeyManager.Instance.EnableBackKey();
            buttonUnlock();
        })
        .SendStart();
    }

    /// <summary>
    /// 進化成功
    /// </summary>
    /// <param name="_data"></param>
    private void resultSuccess(ServerApi.ResultData _data)
    {
        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvPointShopEvol>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
        UserDataAdmin.Instance.ConvertPartyAssing();

        //----------------------------------------
        // 進化後のユニット情報を引き渡し
        //----------------------------------------
        PacketStructUnit cAfterUnit = UserDataAdmin.Instance.SearchChara(m_BaseUnit.unique_id);
        if (cAfterUnit != null)
        {
            MainMenuParam.m_EvolveBaseAfter = new PacketStructUnit();
            MainMenuParam.m_EvolveBaseAfter.Copy(cAfterUnit);
        }
        else
        {
            Debug.LogError("Blend Unit After None!");
        }

        //Camera mainCamera = SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>();
        m_UnitResult = UnitResult.Create(/*mainCamera,*/ UnitResult.ResultType.Evolve);
        if (m_UnitResult != null)
        {
            UnitResultEvolve resultEvolve = m_UnitResult.Parts.GetComponent<UnitResultEvolve>();
            resultEvolve.Initialize(
                MainMenuParam.m_EvolveBaseBefore.id,
                MainMenuParam.m_EvolveBaseAfter.id,
                new uint[]
                {
                    0,0,0
                },
                () =>
                {
                    resultEvolve.Show(HideUnitResult);
                    buttonUnlock();
                });
            ResetAll();
            return;
        }

        //リザルト表示できなかった（こちらに来ることはない）

        AndroidBackKeyManager.Instance.EnableBackKey();

        ResetAll();
    }

    private void HideUnitResult()
    {
        if (m_UnitResult != null)
        {
            m_UnitResult.Hide();
            m_UnitResult = null;
        }
        AndroidBackKeyManager.Instance.EnableBackKey();
        buttonUnlock();
    }

    private void ResetAll()
    {
        MainMenuParam.m_EvolveBaseUnitUniqueId = 0;
        //ユニットリスト作成
        makeUnitList();
        //ウインドウ閉じる
        m_ExpandWindow.Close();
        m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
        //パネルを消す
        m_UnitBGPanel.IsViewPanel = false;
        //ソートボタン有効化
        m_UnitGrid.IsActiveSortButton = true;
        //確認OFF
        m_Validate = false;

        updateEvolveStatus();

        m_UnitBGPanel.TotalPoint = (int)UserDataAdmin.Instance.m_StructPlayer.have_unit_point;

        //ユニット詳細へ
        if (MainMenuManager.HasInstance)
        {
            UnitDetailInfo info = MainMenuManager.Instance.OpenUnitDetailInfoPlayer(MainMenuParam.m_EvolveBaseAfter);
            info.SetCloseAction(() =>
            {
                StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
            });
        }
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
                //ベース未選択時
                if (!CheckBaseUnit(_unit.UnitData))
                {
                    return;
                }

                SoundUtil.PlaySE(SEID.SE_MENU_OK);
                MainMenuParam.m_EvolveBaseUnitUniqueId = _unit.UnitData.unique_id;
                _unit.IsSelectedUnit = true;
                updateEvolveStatus();
                m_ExpandWindow.Open();
                m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                AndroidBackKeyManager.Instance.StackPush(m_CanvasObj.gameObject, OnBackKeyBaseSelect);
            }
            else if (m_BaseUnit.unique_id == _unit.UnitData.unique_id)
            {
                OnBackKeyBaseSelect();
            }
            else
            {
                SetupBlendAfterStatus();
                m_UnitGrid.UpdateList();
            }
            m_UnitBGPanel.IsActiveExecButton = IsEvolveStart();
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

    private void OnBackKeyBaseSelect()
    {
        if (IsBusy() == true)
        {
            return;
        }

        backKeyBaseSelect(true);
    }

    private void backKeyBaseSelect(bool isSe)
    {
        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        if (isSe == true)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        unsetBaseUnit();
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
            OnBackKeyBaseSelect();
        }
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
        if (MainMenuParam.m_EvolveBaseUnitUniqueId == 0)
        {
            return;
        }

        if (IsBusy() == true)
        {
            return;
        }

        backKeyBaseSelect(false);
        backKeyValidate();
    }

    /// <summary>
    /// 進化後ユニットアイコン選択
    /// </summary>
    private void SelectEvolveUnitIcon()
    {
        if (MainMenuParam.m_EvolveBaseUnitUniqueId == 0 || m_AfterCharaMaster == null)
        {
            return;
        }

        if (IsBusy() == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.OpenUnitDetailInfoCatalog(m_AfterCharaMaster.fix_id);
        }
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

        if (m_Validate)
        {
            backKeyValidate();
        }
        else
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
        if (m_ExpandWindow.isOpen == true)
        {
            m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
        }
        else
        {
            m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
        }
    }

    /// <summary>
    /// 選択されている
    /// </summary>
    /// <param name="_unique_id"></param>
    /// <returns></returns>
    private bool IsSelectUnit(long _unique_id)
    {
        if (m_BaseUnit != null && m_BaseUnit.unique_id == _unique_id) return true;
        return false;
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
        // 必要ポイントの算出
        //-----------------------
        m_BlendPoint = 0;
        m_BlendPoint += (m_BaseCharaMaster != null) ? (uint)m_BaseCharaMaster.evol_unitpoint : 0;

        //-----------------------
        // 合成時のパラメータ数値表示部分を更新
        //-----------------------
        if (m_BaseUnit != null
        && m_CharaEvol != null)
        {
            MasterDataParamChara cCharaMasterData = MasterDataUtil.GetCharaParamFromID(m_BaseUnit.id);
            MasterDataParamChara cCharaMasterDataAfter = MasterDataUtil.GetCharaParamFromID(m_CharaEvol.unit_id_after);
            if (cCharaMasterData != null
            && cCharaMasterDataAfter != null
            )
            {
                m_UnitBGPanel.Point = (int)m_BlendPoint;
            }

            uint plus_pow = m_BaseUnit.add_pow;
            uint plus_hp = m_BaseUnit.add_hp;

            //プラス値最大チェック
            if (plus_pow > GlobalDefine.PLUS_MAX)
            {
                plus_pow = GlobalDefine.PLUS_MAX;
            }
            if (plus_hp > GlobalDefine.PLUS_MAX)
            {
                plus_hp = GlobalDefine.PLUS_MAX;
            }

            //進化後パラメータ
            m_UnitStatusPanel.setupChara(m_AfterCharaMaster.fix_id, UnitStatusPanel.StatusType.LV_1, m_BaseUnit.limitover_lv, plus_pow, plus_hp);

            // 進化後パラメータはLv1のステータスが設定されるため、ActiveSkillLvをベースユニットから引き継いで設定する.
            uint activeSkillLv = 0;
            int activeSkillLvMax = 0;
            if (cCharaMasterDataAfter.skill_limitbreak != 0)
            {
                if (cCharaMasterData.skill_limitbreak == cCharaMasterDataAfter.skill_limitbreak)
                {
                    //進化後も同じスキルの場合はLVを引き継ぐ
                    activeSkillLv = m_BaseUnit.limitbreak_lv + 1;
                }
                else
                {
                    //進化後のスキルが違う場合は１にする。
                    activeSkillLv = 1;
                }
                MasterDataSkillLimitBreak cMasterSkillLimitBreak = MasterDataUtil.GetLimitBreakSkillParamFromID(cCharaMasterDataAfter.skill_limitbreak);
                activeSkillLvMax = cMasterSkillLimitBreak.level_max + 1;
            }
            m_UnitStatusPanel.Aslv = string.Format(GameTextUtil.GetText("unit_status15"), activeSkillLv);
            m_UnitStatusPanel.AslvMax = string.Format(GameTextUtil.GetText("unit_status15"), activeSkillLvMax);
        }
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

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        SortDialog dialog = SortDialog.Create();
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.UNIT);
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterUnitPointEvolve());
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
        LocalSaveManager.Instance.SaveFuncSortFilterUnitPointEvolve(sortInfo);

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
        if (IsSelectUnit(_unit.UnitData.unique_id))
        {
            _unit.IsSelectedUnit = true;
        }
        else
        {
            _unit.IsSelectedUnit = false;
        }
    }

    void SetupBaseUnitIconType(UnitGridContext _unit)
    {
        //ベースとして選択できるか？
        if (CheckBaseUnit(_unit.UnitData))
        {
            if (CheckBaseUnitEvolve(_unit.UnitData))
            {
                _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
            }
            else
            {
                _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.ALPHA_HALF_DISABLE_BUTTON;
            }
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

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        base.PageSwitchEventDisableAfter(eNextMainMenuSeq);
        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        m_ExpandWindow.SetBackKey(false);
        AndroidBackKeyManager.Instance.EnableBackKey();

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
