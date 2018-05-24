using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuUnitEvolve : MainMenuSeq
{
    private readonly float MATERIAL_ICON_SIZE = 48.0f;      //!< 素材アイコンサイズ
    private readonly float MATERIAL_ICON_SPACE_SIZE = 10.0f;  //!< 素材アイコン間隔サイズ

    private UnitBGPanel m_UnitBGPanel = null;
    private UnitStatusPanel m_UnitStatusPanel = null;
    private UnitMaterialPanel m_UnitMaterialPanel = null;
    private UnitGridComplex m_UnitGrid = null;
    private ExpandWindow m_ExpandWindow = null;


    private PacketStructUnit m_BaseUnit = null;
    private MasterDataParamChara m_BaseCharaMaster = null;
    private MasterDataParamCharaEvol m_CharaEvol = null;
    private MasterDataParamChara m_AfterCharaMaster = null;
    private uint m_BlendMoney = 0;
    private bool m_Validate = false;
    private UnitResult m_UnitResult = null;

    private Sprite m_ConfirmSprite = null;
    private Sprite m_DecideSprite = null;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_ConfirmSprite = ResourceManager.Instance.Load("confirm_button");
        m_DecideSprite = ResourceManager.Instance.Load("s_button");
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
            m_UnitBGPanel.IsViewEvolve = true;
            m_UnitBGPanel.IsViewResetButton = false;
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;
            m_UnitBGPanel.Title = GameTextUtil.GetText("unit_function_evolve");
            m_UnitBGPanel.Evolve_arrow = GameTextUtil.GetText("unit_function_evolve_arrow");

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

        if (m_UnitMaterialPanel == null)
        {
            m_UnitMaterialPanel = m_CanvasObj.GetComponentInChildren<UnitMaterialPanel>();
            m_UnitMaterialPanel.SetPositionAjustStatusBar(new Vector2(-93, -177));
            m_UnitMaterialPanel.setIconSize(MATERIAL_ICON_SIZE);
            m_UnitMaterialPanel.setIconSpaceSize(MATERIAL_ICON_SPACE_SIZE);
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
	public void updateEvolveStatus(bool bRenew = false, bool bMaterialReset = false)
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

            //素材設定
            {
                //素材表示ON
                UnityUtil.SetObjectEnabledOnce(m_UnitMaterialPanel.gameObject, true);
                m_UnitMaterialPanel.MaterialList.Clear();
                m_UnitMaterialPanel.ObjectList.Clear();
                m_UnitMaterialPanel.PanelColor = Color.clear;
                int _materialCount = 0;
                //進化素材情報設定
                if (m_CharaEvol.unit_id_parts1 != 0)
                {
                    m_UnitMaterialPanel.addItem(_materialCount++, m_CharaEvol.unit_id_parts1, SelectMaterialUnit, true);
                }
                if (m_CharaEvol.unit_id_parts2 != 0)
                {
                    m_UnitMaterialPanel.addItem(_materialCount++, m_CharaEvol.unit_id_parts2, SelectMaterialUnit, true);
                }
                if (m_CharaEvol.unit_id_parts3 != 0)
                {
                    m_UnitMaterialPanel.addItem(_materialCount++, m_CharaEvol.unit_id_parts3, SelectMaterialUnit, true);
                }

                StartCoroutine(WaitSetMaterial());
            }

            //ボタン関連ON
            m_UnitBGPanel.IsViewExecButton = true;
            m_UnitBGPanel.IsViewReturnButton = true;
            m_UnitBGPanel.ExecButtonImage = m_ConfirmSprite;

            m_UnitBGPanel.IsActiveExecButton = IsEvolveStart();

            MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un82p_description"));
        }
        else
        {
            //表示リセット
            m_UnitBGPanel.Money = 0;
            m_UnitBGPanel.resetBaseUnit();
            m_UnitBGPanel.resetEvolveUnit();
            m_UnitStatusPanel.reset();
            m_UnitMaterialPanel.MaterialList.Clear();

            //
            m_AfterCharaMaster = null;
            if (m_CharaEvol != null)
            {
                UnitIconImageProvider.Instance.Reset(m_CharaEvol.unit_id_parts1);
                UnitIconImageProvider.Instance.Reset(m_CharaEvol.unit_id_parts2);
                UnitIconImageProvider.Instance.Reset(m_CharaEvol.unit_id_parts3);

                m_CharaEvol = null;
            }

            //ボタン関連OFF
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;

            if (bMaterialReset)
            {
                UnityUtil.SetObjectEnabledOnce(m_UnitMaterialPanel.gameObject, false);
            }
            else
            {
                StartCoroutine(WaitClearMaterial());
            }

            //ベース選択リスト更新
            updateBaseUnitList();
        }

    }

    public IEnumerator WaitSetMaterial()
    {
        while (m_UnitMaterialPanel.ObjectList.Count != m_UnitMaterialPanel.MaterialList.Count)
        {
            yield return null;
        }
        m_UnitMaterialPanel.PanelColor = Color.white;
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            MaterialDataContext material = m_UnitMaterialPanel.MaterialList[i];
            material.MaterialColor = Color.white;

            //進化素材を探して設定する
            long unique_id = SearchMaterialUnit(material.m_CharaId, i);
            material.setUnit(unique_id);
        }

        //素材選択リスト更新
        updateMaterialUnitList();

        //進化後ステータス更新
        SetupBlendAfterStatus();
    }

    public IEnumerator WaitClearMaterial()
    {
        while (m_UnitMaterialPanel.ObjectList.Count != 0)
        {
            yield return null;
        }
        UnityUtil.SetObjectEnabledOnce(m_UnitMaterialPanel.gameObject, false);
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
        m_UnitGrid.SetupUnitSelected = SetupUnitSelected;

        m_UnitGrid.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterEvolveUnit());
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

        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un81p_description"));
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
    /// 素材ユニット解除
    /// </summary>
    /// <param name="_unique_id"></param>
    private void unsetMaterialUnit(long _unique_id)
    {
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == _unique_id)
            {
                m_UnitMaterialPanel.MaterialList[i].setUnit(0);
            }
        }
    }

    /// <summary>
    /// 素材ユニット設定
    /// </summary>
    /// <param name="_chara_id"></param>
    /// <param name="_unique_id"></param>
    /// <returns></returns>
    private bool setMaterialUnit(uint _chara_id, long _unique_id)
    {
        //空きがある時はそのまま設定
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_CharaId == _chara_id &&
                m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0)
            {
                m_UnitMaterialPanel.MaterialList[i].setUnit(_unique_id);
                return true;
            }
        }
        //空きがない時は入れ替え
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_CharaId == _chara_id &&
                m_UnitMaterialPanel.MaterialList[i].m_UniqueId != 0)
            {
                //選択フラグ解除
                setUnitSelectFlag(m_UnitMaterialPanel.MaterialList[i].m_UniqueId, false);
                //解除処理
                unsetMaterialUnit(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);

                m_UnitMaterialPanel.MaterialList[i].setUnit(_unique_id);
                return true;
            }
        }
        return false;
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
        m_UnitBGPanel.Message = GameTextUtil.GetText("evo_text");
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
    /// 素材選択で選択できるユニットかどうか
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckMaterialUnit(PacketStructUnit _unit)
    {
        //ベース素材
        if (m_BaseUnit != null &&
            m_BaseUnit.unique_id == _unit.unique_id) return true;
        //素材対象
        if (!IsMaterialUnit(_unit.id)) return false;
        // リンクチェック
        if (_unit.link_info != (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE) return false;
        // パーティチェック
        if (MainMenuUtil.ChkUnitPartyAssign(_unit.unique_id)) return false;
        // お気に入り
        if (MainMenuUtil.ChkUnitFavorite(_unit.unique_id)) return false;

        return true;
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
        MasterDataParamCharaEvol _masterEvolve = MasterDataUtil.GetCharaEvolParamFromCharaID(_unit.id);
        if (_masterEvolve == null) return false;
        uint blendMoney = _masterEvolve.money;
        if (MainMenuParam.m_BeginnerBoost != null
        && MainMenuParam.m_BeginnerBoost.boost_build_money != 100
        )
        {
            blendMoney = MasterDataUtil.ConvertBeginnerBoostBuildMoney(ref MainMenuParam.m_BeginnerBoost, blendMoney);
        }
        // 所持金チェック
        if (blendMoney > UserDataAdmin.Instance.m_StructPlayer.have_money) return false;
        PacketStructUnit tempUnit = m_BaseUnit;
        List<MaterialDataContext> tempList = m_UnitMaterialPanel.MaterialList;
        m_UnitMaterialPanel.MaterialList.Clear();
        bool ret = true;
        m_BaseUnit = _unit;
        long unique_id;
        // 素材チェック
        if (_masterEvolve.unit_id_parts1 != 0)
        {
            unique_id = SearchMaterialUnit(_masterEvolve.unit_id_parts1, 0);
            if (unique_id == 0) ret = false;
            m_UnitMaterialPanel.addItem(0, _masterEvolve.unit_id_parts1);
            m_UnitMaterialPanel.MaterialList[0].setUnit(unique_id);
        }
        if (_masterEvolve.unit_id_parts2 != 0)
        {
            unique_id = SearchMaterialUnit(_masterEvolve.unit_id_parts2, 0);
            if (unique_id == 0) ret = false;
            m_UnitMaterialPanel.addItem(1, _masterEvolve.unit_id_parts2);
            m_UnitMaterialPanel.MaterialList[1].setUnit(unique_id);
        }
        if (_masterEvolve.unit_id_parts3 != 0)
        {
            if (SearchMaterialUnit(_masterEvolve.unit_id_parts3, 2) == 0) ret = false;
        }
        m_BaseUnit = tempUnit;
        m_UnitMaterialPanel.MaterialList.Clear();
        m_UnitMaterialPanel.MaterialList = tempList;

        return ret;
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
        if (IsBusy() == true)
        {
            return;
        }

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
            // 素材アサインチェック
            //--------------------------
            bool bLessMaterial = false;
            if (!IsCompleteMaterialUnit())
            {
                bLessMaterial = true;
            }
            //--------------------------
            // 所持金チェック
            //--------------------------
            bool bLessMoney = false;
            if (m_BlendMoney > UserDataAdmin.Instance.m_StructPlayer.have_money)
            {
                bLessMoney = true;
            }

            if (bLessLevel || bLessMaterial || bLessMoney)
            {
                Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "shinka_impossible_title");
                string mainText = "";
                if (bLessMaterial)
                {
                    mainText += GameTextUtil.GetText("shinka_impossible_content1") + "\n";
                    for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; ++i)
                    {
                        if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0)
                        {
                            MasterDataParamChara master = MasterFinder<MasterDataParamChara>.Instance.Find((int)m_UnitMaterialPanel.MaterialList[i].m_CharaId);
                            if (master != null)
                            {
                                mainText += string.Format("<color=#ffd700>{0}</color>\n", master.name);
                            }
                        }
                    }
                }
                if (bLessLevel) mainText += GameTextUtil.GetText("shinka_impossible_content2") + "\n";
                if (bLessMoney) mainText += GameTextUtil.GetText("shinka_impossible_content3");
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
        //素材
        List<long> partsList = new List<long>();
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId != 0)
            {
                partsList.Add(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);
            }
        }

        //----------------------------------------
        // 演出用に通信処理を行う前の情報を保持しておく
        //----------------------------------------
        {
            MainMenuParam.m_EvolveBaseBefore = new PacketStructUnit();
            MainMenuParam.m_EvolveBaseBefore.Copy(m_BaseUnit);

            MainMenuParam.m_EvolveParts.Release();
            for (int i = 0; i < partsList.Count; i++)
            {
                if (partsList[i] == 0)
                {
                    continue;
                }

                PacketStructUnit _unit = UserDataAdmin.Instance.SearchChara(partsList[i]);
                if (_unit == null)
                {
                    continue;
                }

                PacketStructUnit cUnit = new PacketStructUnit();
                cUnit.Copy(_unit);
                MainMenuParam.m_EvolveParts.Add(cUnit);
            }
        }

        ButtonBlocker.Instance.Block(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
        //ユニット進化送信
        ServerDataUtilSend.SendPacketAPI_Evolve_Unit(
                                                         m_BaseUnit.unique_id
                                                        , partsList.ToArray()
                                                        , m_CharaEvol.fix_id
                                                        , m_CharaEvol.unit_id_after
                                                        , (MainMenuParam.m_BeginnerBoost != null) ? (int)MainMenuParam.m_BeginnerBoost.fix_id : 0)
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
        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvEvolveUnit>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
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
                    m_CharaEvol.unit_id_parts1,
                    m_CharaEvol.unit_id_parts2,
                    m_CharaEvol.unit_id_parts3
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
        //パネルを消す
        m_UnitBGPanel.IsViewPanel = false;
        //ソートボタン有効化
        m_UnitGrid.IsActiveSortButton = true;
        //確認OFF
        m_Validate = false;

        updateEvolveStatus(false, true);

        //ユニット詳細へ
        if (MainMenuManager.HasInstance)
        {
            UnitDetailInfo info = MainMenuManager.Instance.OpenUnitDetailInfoPlayer(MainMenuParam.m_EvolveBaseAfter);
            info.SetCloseAction(() =>
            {
                //ウインドウ閉じる
                m_ExpandWindow.Close();
                m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
            });
        }
    }

    /// <summary>
    /// 進化素材が設定されているか？
    /// </summary>
    /// <returns></returns>
    private bool IsCompleteMaterialUnit()
    {
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0) return false;
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
                //進化素材を選択
                if (!CheckMaterialUnit(_unit.UnitData))
                {
                    return;
                }

                if (IsSelectMaterialUnit(_unit.UnitData.unique_id))
                {
                    SoundUtil.PlaySE(SEID.SE_MENU_RET);
                    //解除
                    unsetMaterialUnit(_unit.UnitData.unique_id);
                }
                else
                {
                    if (setMaterialUnit(_unit.UnitData.id, _unit.UnitData.unique_id))
                    {
                        SoundUtil.PlaySE(SEID.SE_MENU_OK);
                    }
                }
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
            backKeyValidate();
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
            openUnitDetailInfo(_unit);
        }
    }

    /// <summary>
    /// 素材選択
    /// </summary>
    /// <param name="_material"></param>
    private void SelectMaterialUnit(MaterialDataContext _material)
    {
        if (IsBusy() == true)
        {
            return;
        }

        if (MainMenuManager.HasInstance == false)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        MainMenuManager.Instance.OpenUnitDetailInfoCatalog(_material.m_CharaId);
    }

    /// <summary>
    /// ユニット詳細を開く
    /// </summary>
    /// <param name="_unit"></param>
    private void openUnitDetailInfo(UnitGridContext _unit)
    {
        UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo();
        if (_info == null) return;
        PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit.UnitData);
        _info.SetUnitFavorite(_unit.UnitData, _subUnit, _unit);
        _info.SetCloseAction(() =>
        {
            //選択されているユニットが素材に選択されていたら解除する。
            if (IsSelectMaterialUnit(_unit.UnitData.unique_id) &&
                _unit.IsActiveFavoriteImage)
            {
                //解除
                unsetMaterialUnit(_unit.UnitData.unique_id);
                //ステータス更新
                SetupBlendAfterStatus();
                //ボタン制御
                m_UnitBGPanel.IsActiveExecButton = IsEvolveStart();

                SetupUnitSelected(_unit);
            }

            //更新データ反映
            m_UnitGrid.UpdateBaseItem(_unit);

            if (m_BaseUnit != null)
            {
                //素材選択シーケンスの場合はIconTypeを更新する（更新ユニットのみ）
                SetupMaterialUnitIconType(_unit);
            }
            else
            {
                //ベース選択シーケンスの場合はすべて更新
                m_UnitGrid.UpdateList();
            }

        });
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

        // SEが二重にならないようにする対応
        //OnBackKeyBaseSelect();
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
    /// 素材がボックスにあったら取得する
    /// </summary>
    /// <param name="unit_id"></param>
    /// <returns></returns>
    private long SearchMaterialUnit(uint unit_id, int material_id)
    {
        UnitGridParam[] unitBox = System.Array.FindAll(UserDataAdmin.Instance.m_UnitGridParamList, (v) => v.unit_id == unit_id);
        //Level低い順にソート
        System.Array.Sort(unitBox, (a, b) => (int)a.level - (int)b.level);

        for (int i = 0; i < unitBox.Length; i++)
        {
            //お気に入り
            if (unitBox[i].favorite)
            {
                continue;
            }

            //パーティーに入っている
            if (unitBox[i].party_assign)
            {
                continue;
            }

            //リンクしている・されている
            if (unitBox[i].unit.link_info != (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
            {
                continue;
            }

            //ベースユニットは対象外
            if (m_BaseUnit.unique_id == unitBox[i].unique_id)
            {
                continue;
            }

            //素材として選択されていない
            if (!IsSelectMaterialUnit(unitBox[i].unique_id)) return unitBox[i].unique_id;
        }
        //持ってない
        return 0;
    }

    /// <summary>
    /// 選択されている
    /// </summary>
    /// <param name="_unique_id"></param>
    /// <returns></returns>
    private bool IsSelectUnit(long _unique_id)
    {
        if (m_BaseUnit != null && m_BaseUnit.unique_id == _unique_id) return true;
        if (IsSelectMaterialUnit(_unique_id)) return true;
        return false;
    }

    /// <summary>
    /// 素材として選択されている
    /// </summary>
    /// <param name="_unique_id"></param>
    /// <returns></returns>
    private bool IsSelectMaterialUnit(long _unique_id)
    {
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == _unique_id) return true;
        }
        return false;
    }

    /// <summary>
    /// 素材対象かどうか？
    /// </summary>
    /// <param name="_chara_no"></param>
    /// <returns></returns>
    private bool IsMaterialUnit(uint _chara_no)
    {
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_CharaId == _chara_no)
            {
                return true;
            }
        }
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
        // 必要資金の算出
        //-----------------------
        m_BlendMoney = 0;
        m_BlendMoney += (m_CharaEvol != null) ? m_CharaEvol.money : 0;

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

                //----------------------------------------
                // 初心者ブースト適用
                // 表示用の値を計算、補正値を適用
                //
                // ※費用倍率が１倍の場合は表示反映しない
                //----------------------------------------
                if (MainMenuParam.m_BeginnerBoost != null
                && MainMenuParam.m_BeginnerBoost.boost_build_money != 100
                )
                {
                    m_BlendMoney = MasterDataUtil.ConvertBeginnerBoostBuildMoney(ref MainMenuParam.m_BeginnerBoost, m_BlendMoney);

                }

                m_UnitBGPanel.Money = (int)m_BlendMoney;
            }

            uint plus_pow = m_BaseUnit.add_pow;
            uint plus_hp = m_BaseUnit.add_hp;
            for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
            {
                if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0) continue;
                PacketStructUnit unit = UserDataAdmin.Instance.SearchChara(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);
                if (unit == null) continue;
                plus_pow += unit.add_pow;
                plus_hp += unit.add_hp;
            }

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
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterEvolveUnit());
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
        LocalSaveManager.Instance.SaveFuncSortFilterEvolveUnit(sortInfo);

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
    void SetupUnitSelected(UnitGridContext _unit)
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="bBack"></param>
    /// <returns></returns>
    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        //ユニットパラメータが作成されるまで待つ
        if (UserDataAdmin.Instance.m_bThreadUnitParam) return true;
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
