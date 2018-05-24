using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuUnitLink : MainMenuSeq
{
    private readonly float MATERIAL_ICON_SIZE = 48.0f;      //!< 素材アイコンサイズ
    private readonly float MATERIAL_ICON_SPACE_SIZE = 10.0f;  //!< 素材アイコン間隔サイズ

    public enum LinkType
    {
        CREATE,
        DELETE,
    }

    private UnitBGPanel m_UnitBGPanel = null;
    private UnitStatusPanel m_UnitStatusPanel = null;
    private UnitMaterialPanel m_UnitMaterialPanel = null;
    private ExpandWindow m_ExpandWindow = null;
    private UnitGridComplex m_UnitGrid = null;
    private UnitLinkPanel m_UnitLinkPanel = null;

    private UnitGridParam m_BaseUnit = null;
    private MasterDataParamChara m_BaseCharaMaster = null;
    private UnitGridParam m_TargetUnit = null;
    private MasterDataParamChara m_TargetCharaMaster = null;

    private LinkType m_LinkType = LinkType.CREATE;
    private bool m_Validate = false;

    private UnitResult m_UnitResult = null;

    private bool m_ExpandButtonOpen = false;

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
            makeUnitList();
            m_UnitGrid.UpdateList();
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        initLinkParts();

        updateLinkStatus(true);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.UNIT;
    }

    private void initLinkParts()
    {
        //ページ初期化処理
        if (m_UnitBGPanel == null)
        {
            m_UnitBGPanel = m_CanvasObj.GetComponentInChildren<UnitBGPanel>();
            m_UnitBGPanel.SetPositionAjustStatusBar(new Vector2(0, 40), new Vector2(0, -345));
            m_UnitBGPanel.IsViewResetButton = false;
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;
            m_UnitBGPanel.IsViewEvolve = true;
            m_UnitBGPanel.IsActiveLink = true;
            m_UnitBGPanel.IsLinkStatus = true;
            m_UnitBGPanel.Title = GameTextUtil.GetText("unit_function_link");

            m_UnitBGPanel.DidSelect = SelectLink;
            m_UnitBGPanel.DidReturn = SelectReturn;
            m_UnitBGPanel.DidSelectIcon = SelectBaseUnitIcon;
            m_UnitBGPanel.DidSelectIconLongpress = SelectBaseUnitIconLongpress;
            m_UnitBGPanel.DidSelectEvolveIcon = SelectTargetUnitIcon;
            m_UnitBGPanel.DidSelectEvolveIconLongpress = SelectTargetUnitIconLongpress;
            m_UnitBGPanel.OnClickLinkStatusAction = OnClickLinkStatus;
            m_UnitBGPanel.OnClickLinkSkillAction = OnClickLinkSkill;

        }

        if (m_ExpandWindow == null)
        {
            m_ExpandWindow = m_CanvasObj.GetComponentInChildren<ExpandWindow>();
            m_ExpandWindow.SetPositionAjustStatusBar(new Vector2(0, -232));
            m_ExpandWindow.ViewHeightSize = 260.0f;
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

        if (m_UnitLinkPanel == null)
        {
            m_UnitLinkPanel = m_CanvasObj.GetComponentInChildren<UnitLinkPanel>();
            if (m_ExpandWindow != null) m_UnitLinkPanel.SetParent(m_ExpandWindow.Content);
            m_UnitLinkPanel.SetPositionAjustStatusBar(new Vector2(0, 35));
            m_UnitLinkPanel.SetSize(new Vector2(592, 245), true);
            m_UnitLinkPanel.IsSkillOnly = true;
            m_UnitLinkPanel.setupSkill(null);
        }

        m_ExpandButtonOpen = false;
    }

    /// <summary>
    /// リンクステータス初期化
    /// </summary>
    private void updateLinkStatus(bool bRenew = false, bool bMaterialReset = false)
    {
        if (bRenew)
        {
            makeUnitList();
            //
            m_ExpandWindow.Close(true);
            m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
            //
            m_Validate = false;
            //
            m_UnitBGPanel.IsViewPanel = false;
            //
            m_UnitGrid.IsActiveSortButton = true;

        }

        m_BaseUnit = UserDataAdmin.Instance.SearchUnitGridParam(MainMenuParam.m_LinkBaseUnitUniqueId);
        m_TargetUnit = UserDataAdmin.Instance.SearchUnitGridParam(MainMenuParam.m_LinkTargetUnitUniqueId);

        if (m_BaseUnit == null)
        {
            //ベース未選択状態

            //表示リセット
            m_UnitBGPanel.Money = 0;
            m_UnitBGPanel.resetBaseUnit();
            m_UnitStatusPanel.reset();

            //ボタン関連OFF
            m_UnitBGPanel.IsViewExecButton = false;
            m_UnitBGPanel.IsViewReturnButton = false;

            if (m_TargetUnit == null)
            {
                m_UnitBGPanel.resetEvolveUnit();
                m_UnitMaterialPanel.MaterialList.Clear();
                if (bMaterialReset == false)
                {
                    StartCoroutine(WaitClearMaterial());
                }
                else
                {
                    UnityUtil.SetObjectEnabledOnce(m_UnitMaterialPanel.gameObject, false);
                }
            }

            //ベース選択リスト更新
            updateBaseUnitList();

            MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un86p_description"));
            m_UnitBGPanel.LinkStatusImage = ResourceManager.Instance.Load("btn_status_off", ResourceType.Common);
            m_UnitBGPanel.LinkSkillImage = ResourceManager.Instance.Load("btn_skill_off", ResourceType.Common);
        }
        else
        {
            switch (m_BaseUnit.unit.link_info)
            {
                case (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE:
                    //リンクなし
                    m_LinkType = LinkType.CREATE;
                    break;
                case (int)CHARALINK_TYPE.CHARALINK_TYPE_BASE:
                    //リンクベース
                    {
                        MainMenuParam.m_LinkTargetUnitUniqueId = m_BaseUnit.unit.link_unique_id;
                        m_TargetUnit = UserDataAdmin.Instance.SearchUnitGridParam(MainMenuParam.m_LinkTargetUnitUniqueId);
                        setUnitSelectFlag(MainMenuParam.m_LinkTargetUnitUniqueId, true);
                        m_LinkType = LinkType.DELETE;
                    }
                    break;
                case (int)CHARALINK_TYPE.CHARALINK_TYPE_LINK:
                    //リンクターゲット
                    {
                        MainMenuParam.m_LinkBaseUnitUniqueId = m_BaseUnit.unit.link_unique_id;
                        MainMenuParam.m_LinkTargetUnitUniqueId = m_BaseUnit.unit.unique_id;
                        m_BaseUnit = UserDataAdmin.Instance.SearchUnitGridParam(MainMenuParam.m_LinkBaseUnitUniqueId);
                        m_TargetUnit = UserDataAdmin.Instance.SearchUnitGridParam(MainMenuParam.m_LinkTargetUnitUniqueId);
                        setUnitSelectFlag(MainMenuParam.m_LinkBaseUnitUniqueId, true);
                        m_LinkType = LinkType.DELETE;
                    }
                    break;
                default:
                    Debug.LogError("baseUnit link_info Error!");
                    return;
            }
            //ベース情報
            m_BaseCharaMaster = m_BaseUnit.master;
            m_UnitBGPanel.setupBaseUnit(m_BaseCharaMaster, m_BaseUnit.unit);
            m_UnitStatusPanel.setup(m_BaseCharaMaster, m_BaseUnit.unit);
            if (m_ExpandWindow.isOpen == false)
            {
                m_UnitBGPanel.LinkStatusImage = ResourceManager.Instance.Load("btn_status", ResourceType.Common);
                m_UnitBGPanel.LinkSkillImage = ResourceManager.Instance.Load("btn_skill", ResourceType.Common);
            }

            //ターゲット情報
            if (m_TargetUnit == null)
            {
                //ターゲット未選択状態
                m_UnitBGPanel.resetEvolveUnit();
                m_UnitMaterialPanel.MaterialList.Clear();

                updateTargetUnitList();

                //ボタン関連ON
                m_UnitBGPanel.IsViewExecButton = true;
                m_UnitBGPanel.IsViewReturnButton = true;
                m_UnitBGPanel.ExecButtonImage = ResourceManager.Instance.Load("confirm_button");

                m_UnitBGPanel.IsActiveExecButton = IsLinkStart();

                resetTarget();
                m_TargetCharaMaster = null;

                StartCoroutine(WaitClearMaterial());
                //
                SetupLinkStatus();

                MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un87p_description"));
            }
            else
            {
                //素材選択状態
                m_TargetCharaMaster = m_TargetUnit.master;

                m_UnitBGPanel.setupEvolveUnit(m_TargetCharaMaster, m_TargetUnit.unit);

                //if (!UnityUtil.ChkObjectEnabled(m_UnitMaterialPanel.gameObject))
                {
                    UnityUtil.SetObjectEnabledOnce(m_UnitMaterialPanel.gameObject, true);
                    m_UnitMaterialPanel.MaterialList.Clear();
                    m_UnitMaterialPanel.PanelColor = Color.clear;
                    if (m_LinkType == LinkType.CREATE)
                    {
                        int _count = 0;
                        if (m_TargetCharaMaster.link_unit_id_parts1 != 0)
                        {
                            m_UnitMaterialPanel.addItem(_count++, m_TargetCharaMaster.link_unit_id_parts1, SelectMaterialUnit, true);
                        }
                        if (m_TargetCharaMaster.link_unit_id_parts2 != 0)
                        {
                            m_UnitMaterialPanel.addItem(_count++, m_TargetCharaMaster.link_unit_id_parts2, SelectMaterialUnit, true);
                        }
                        if (m_TargetCharaMaster.link_unit_id_parts3 != 0)
                        {
                            m_UnitMaterialPanel.addItem(_count++, m_TargetCharaMaster.link_unit_id_parts3, SelectMaterialUnit, true);
                        }
                    }
                    else
                    {
                        int _count = 0;
                        if (m_TargetCharaMaster.link_del_unit_id_parts1 != 0)
                        {
                            m_UnitMaterialPanel.addItem(_count++, m_TargetCharaMaster.link_del_unit_id_parts1, SelectMaterialUnit, true);
                        }
                        if (m_TargetCharaMaster.link_del_unit_id_parts2 != 0)
                        {
                            m_UnitMaterialPanel.addItem(_count++, m_TargetCharaMaster.link_del_unit_id_parts2, SelectMaterialUnit, true);
                        }
                        if (m_TargetCharaMaster.link_del_unit_id_parts3 != 0)
                        {
                            m_UnitMaterialPanel.addItem(_count++, m_TargetCharaMaster.link_del_unit_id_parts3, SelectMaterialUnit, true);
                        }
                    }

                    if (m_UnitMaterialPanel.MaterialList.Count != 0)
                    {
                        StartCoroutine(WaitSetMaterial());

                        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un88p_description"));
                    }
                    else
                    {
                        //素材いらない
                        UnityUtil.SetObjectEnabledOnce(m_UnitMaterialPanel.gameObject, false);

                        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("un92p_description"));
                    }
                }

                //ボタン関連ON
                m_UnitBGPanel.IsViewExecButton = true;
                m_UnitBGPanel.IsViewReturnButton = true;
                m_UnitBGPanel.ExecButtonImage = ResourceManager.Instance.Load("confirm_button");

                m_UnitBGPanel.IsActiveExecButton = IsLinkStart();

                updateMaterialList();
                //
                SetupLinkStatus();
            }
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

        updateMaterialList();
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
    /// ベースユニット解除
    /// </summary>
    private void unsetBaseUnit()
    {
        setUnitSelectFlag(MainMenuParam.m_LinkBaseUnitUniqueId, false);
        MainMenuParam.m_LinkBaseUnitUniqueId = 0;
        //リンク解除のときはターゲットもはずす
        if (m_LinkType == LinkType.DELETE)
        {
            setUnitSelectFlag(MainMenuParam.m_LinkTargetUnitUniqueId, false);
            MainMenuParam.m_LinkTargetUnitUniqueId = 0;
            unsetMaterialUnitAll();
        }
        if (m_Validate) setValidate(false);
        updateLinkStatus();
    }

    /// <summary>
    /// ターゲットユニット解除
    /// </summary>
    private void unsetTargetUnit()
    {
        setUnitSelectFlag(MainMenuParam.m_LinkTargetUnitUniqueId, false);
        MainMenuParam.m_LinkTargetUnitUniqueId = 0;
        unsetMaterialUnitAll();
        //リンク解除のときはベースもはずす
        if (m_LinkType == LinkType.DELETE)
        {
            setUnitSelectFlag(MainMenuParam.m_LinkBaseUnitUniqueId, false);
            MainMenuParam.m_LinkBaseUnitUniqueId = 0;
        }
        if (m_Validate) setValidate(false);
        updateLinkStatus();
    }

    /// <summary>
    /// リンク実行選択
    /// </summary>
    private void SelectLink()
    {
        {
            //--------------------------
            // 素材アサインチェック
            //--------------------------
            bool bLessMaterial = false;
            if (!IsCompleteMaterialUnit())
            {
                bLessMaterial = true;
            }
            //----------------------------------------
            // 所持金チェック
            //----------------------------------------
            bool bLessMoney = false;
            if (UserDataAdmin.Instance.m_StructPlayer.have_money < m_UnitBGPanel.Money)
            {
                bLessMoney = true;
            }

            if (bLessMaterial || bLessMoney)
            {
                Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "link_impossible_title");
                string mainText = "";
                if (bLessMaterial)
                {
                    mainText += GameTextUtil.GetText("link_impossible__content1") + "\n";
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
                if (bLessMoney)
                {
                    if (m_LinkType == LinkType.DELETE)
                    {
                        mainText += GameTextUtil.GetText("link_impossible__content3");
                    }
                    else
                    {
                        mainText += GameTextUtil.GetText("link_impossible__content2");
                    }
                }
                newDialog.SetDialogText(DialogTextType.MainText, mainText);
                newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                newDialog.EnableFadePanel();
                newDialog.Show();

                SoundUtil.PlaySE(SEID.SE_MENU_OK);
                return;
            }
        }
        if (!m_Validate)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);

            setValidate(true);
            return;
        }

        if (m_LinkType == LinkType.CREATE)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);

            LinkCreateUnit();
        }
        else
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);

            Dialog _newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "un93q_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "un93q_content");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
            _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
            {
                LinkDeleteUnit();
            });
            _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
            _newDialog.Show();
        }
    }

    /// <summary>
    /// リンク実行
    /// </summary>
	private void LinkCreateUnit()
    {
        AndroidBackKeyManager.Instance.StackPop(SceneCommon.Instance.gameObject);
        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        m_ExpandWindow.SetBackKey(false);
        AndroidBackKeyManager.Instance.DisableBackKey();
        //----------------------------------------
        // 演出用に通信処理を行う前の情報を保持しておく
        //----------------------------------------
        MainMenuParam.m_LinkBaseBefore = m_BaseUnit.unit;
        MainMenuParam.m_LinkUnit = m_TargetUnit.unit;

        MainMenuParam.m_LinkParts.Release();
        for (int num = 0; num < m_UnitMaterialPanel.MaterialList.Count; ++num)
        {
            if (m_UnitMaterialPanel.MaterialList[num].m_UniqueId == 0)
            {
                continue;
            }

            PacketStructUnit materialUnit = UserDataAdmin.Instance.SearchChara(m_UnitMaterialPanel.MaterialList[num].m_UniqueId);
            if (materialUnit == null)
            {
                continue;
            }

            PacketStructUnit cUnit = new PacketStructUnit();
            cUnit.Copy(materialUnit);
            MainMenuParam.m_LinkParts.Add(cUnit);
        }

        ButtonBlocker.Instance.Block(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
        ServerDataUtilSend.SendPacketAPI_UnitLinkCreate(
                                                          m_BaseUnit.unique_id
                                                        , m_TargetUnit.unique_id
                                                        , getPartsArray()
                                                        , (MainMenuParam.m_BeginnerBoost != null) ? (int)MainMenuParam.m_BeginnerBoost.fix_id : 0)
        .setSuccessAction(_data =>
        {
            resultSuccess(_data);
        })
        .setErrorAction(_data =>
        {
            resultError(_data);
        })
        .SendStart();

    }

    /// <summary>
    /// リンク解除実行
    /// </summary>
	private void LinkDeleteUnit()
    {
        AndroidBackKeyManager.Instance.StackPop(SceneCommon.Instance.gameObject);
        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        m_ExpandWindow.SetBackKey(false);
        AndroidBackKeyManager.Instance.DisableBackKey();

        ButtonBlocker.Instance.Block(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
        ServerDataUtilSend.SendPacketAPI_UnitLinkDelete(
                                                          m_BaseUnit.unique_id
                                                        , getPartsArray()
                                                        , (MainMenuParam.m_BeginnerBoost != null) ? (int)MainMenuParam.m_BeginnerBoost.fix_id : 0)
        .setSuccessAction(_data =>
        {
            resultSuccess(_data);
        })
        .setErrorAction(_data =>
        {
            resultError(_data);
        })
        .SendStart();
    }

    /// <summary>
    /// パーツ配列取得
    /// </summary>
    /// <returns></returns>
    private long[] getPartsArray()
    {
        List<long> Parts = new List<long>();
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId != 0)
            {
                Parts.Add(m_UnitMaterialPanel.MaterialList[i].m_UniqueId);
            }
        }

        long[] partsArray = new long[0];
        if (Parts.Count != 0)
        {
            partsArray = Parts.ToArray();
        }
        else
        {
            partsArray = new long[0];
        }
        return partsArray;
    }

    /// <summary>
    /// 成功シーケンス
    /// </summary>
    /// <param name="_data"></param>
    private void resultSuccess(ServerApi.ResultData _data)
    {
        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvUnitLink>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
        UserDataAdmin.Instance.ConvertPartyAssing();

        if (m_LinkType == LinkType.CREATE)
        {
            //----------------------------------------
            // リンク実行後のユニット情報を引き渡し
            //----------------------------------------
            PacketStructUnit cAfterBaceUnit = UserDataAdmin.Instance.SearchChara(m_BaseUnit.unique_id);
            if (cAfterBaceUnit != null)
            {
                MainMenuParam.m_LinkBaseAfter = cAfterBaceUnit;
            }
            else
            {
                Debug.LogError("Link Create After None!");
            }

            SoundUtil.PlaySE(SEID.SE_MAINMENU_BLEND_EXEC);


            m_UnitResult = UnitResult.Create(UnitResult.ResultType.Link);
            if (m_UnitResult != null)
            {
                UnitResultLink resultLink = m_UnitResult.Parts.GetComponent<UnitResultLink>();
                resultLink.Initialize(m_BaseUnit.unit.id, m_TargetUnit.unit.id, () =>
                {
                    buttonUnlock();
                    resultLink.Show(HideUnitResult);
                });

                ResetAll(true);
                return;
            }

            AndroidBackKeyManager.Instance.EnableBackKey();
            ResetAll(true);
        }
        else
        {
            buttonUnlock();
            AndroidBackKeyManager.Instance.EnableBackKey();
            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "un94q_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "un94q_content");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
                ResetAll(false);
            });
            _newDialog.Show();
        }

    }

    /// <summary>
    /// 失敗シーケンス
    /// </summary>
    /// <param name="_data"></param>
	private void resultError(ServerApi.ResultData _data)
    {
        buttonUnlock();
        AndroidBackKeyManager.Instance.EnableBackKey();
    }

    private void HideUnitResult()
    {
        if (m_UnitResult != null)
        {
            m_UnitResult.Hide();
            m_UnitResult = null;
        }
        AndroidBackKeyManager.Instance.EnableBackKey();
    }

    private void resetTarget()
    {
        if (m_TargetCharaMaster != null)
        {
            if (m_LinkType == LinkType.CREATE)
            {
                UnitIconImageProvider.Instance.Reset(m_TargetCharaMaster.link_unit_id_parts1);
                UnitIconImageProvider.Instance.Reset(m_TargetCharaMaster.link_unit_id_parts2);
                UnitIconImageProvider.Instance.Reset(m_TargetCharaMaster.link_unit_id_parts3);
            }
            else
            {
                UnitIconImageProvider.Instance.Reset(m_TargetCharaMaster.link_del_unit_id_parts1);
                UnitIconImageProvider.Instance.Reset(m_TargetCharaMaster.link_del_unit_id_parts2);
                UnitIconImageProvider.Instance.Reset(m_TargetCharaMaster.link_del_unit_id_parts3);
            }
        }
    }

    private void ResetAll(bool bCharaDetail)
    {
        MainMenuParam.m_LinkBaseUnitUniqueId = 0;
        MainMenuParam.m_LinkTargetUnitUniqueId = 0;
        resetTarget();
        makeUnitList();
        //
        m_Validate = false;
        //
        m_UnitBGPanel.IsViewPanel = false;
        //
        m_UnitGrid.IsActiveSortButton = true;
        updateLinkStatus(false, bCharaDetail);
        if (bCharaDetail)
        {
            if (MainMenuManager.HasInstance)
            {
                UnitDetailInfo info = MainMenuManager.Instance.OpenUnitDetailInfoPlayer(MainMenuParam.m_LinkBaseAfter);
                info.SetCloseAction(() =>
                {
                    //ウインドウ閉じる
                    m_ExpandWindow.Close(true);
                    m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                    StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
                });
            }
        }
        else
        {
            //ウインドウ閉じる
            m_ExpandWindow.Close(true);
            m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
        }
    }

    /// <summary>
    /// リンクベース選択
    /// </summary>
    private void SelectBaseUnitIcon()
    {
        if (m_BaseUnit == null)
        {
            return;
        }

        OnSelectBaseBackKey();
    }

    private void SelectBaseUnitIconLongpress()
    {
        if (m_BaseUnit == null)
        {
            return;
        }
    }

    /// <summary>
    /// リンクターゲット選択
    /// </summary>
    private void SelectTargetUnitIcon()
    {
        if (m_TargetUnit == null)
        {
            return;
        }

        OnSelectLinkBackKey();
    }

    private void SelectTargetUnitIconLongpress()
    {
        if (m_TargetUnit == null)
        {
            return;
        }
    }

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
        else if (m_TargetUnit != null)
        {
            OnSelectLinkBackKey();
        }
        else if (m_BaseUnit != null)
        {
            OnSelectBaseBackKey();
        }
    }

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
        if (m_Validate) setValidate(false);
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

        m_UnitGrid.SetUpSortData(LocalSaveManager.Instance.LoadFuncSortFilterLinkUnit());
        m_UnitGrid.CreateList(unitList);
    }

    /// <summary>
    /// 素材リスト更新
    /// </summary>
    public void updateMaterialList()
    {
        m_UnitGrid.SetupUnitIconType = SetupMaterialUnitIconType;
        m_UnitGrid.UpdateList();
    }

    /// <summary>
    /// ベースリスト更新
    /// </summary>
    public void updateBaseUnitList()
    {
        m_UnitGrid.SetupUnitIconType = SetupBaseUnitIconType;
        m_UnitGrid.UpdateList();
    }

    /// <summary>
    /// ターゲットリスト更新
    /// </summary>
    public void updateTargetUnitList()
    {
        m_UnitGrid.SetupUnitIconType = SetupTargetUnitIconType;
        m_UnitGrid.UpdateList();
    }

    /// <summary>
    /// 素材変更選択
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
            if (IsSelectUnit(_unit.UnitData.unique_id))
            {
                if (m_BaseUnit != null &&
                    m_BaseUnit.unique_id == _unit.UnitData.unique_id)
                {
                    //ベース選択解除
                    OnSelectBaseBackKey();
                }
                else if (m_TargetUnit != null &&
                          m_TargetUnit.unique_id == _unit.UnitData.unique_id)
                {
                    //ターゲット選択解除
                    OnSelectLinkBackKey();
                }
                else
                {
                    SoundUtil.PlaySE(SEID.SE_MENU_RET);
                    //解除
                    unsetMaterialUnit(_unit.UnitData.unique_id);
                    _unit.IsSelectedUnit = false;
                }

            }
            else if (m_BaseUnit == null)
            {
                //ベース選択
                if (!CheckBaseUnit(_unit.UnitData))
                {
                    return;
                }

                SoundUtil.PlaySE(SEID.SE_MENU_OK);
                MainMenuParam.m_LinkBaseUnitUniqueId = _unit.UnitData.unique_id;
                _unit.IsSelectedUnit = true;
                updateLinkStatus();
                AndroidBackKeyManager.Instance.StackPush(m_CanvasObj.gameObject, OnSelectBaseBackKey);
            }
            else if (m_TargetUnit == null)
            {
                //ターゲット選択
                if (!CheckTargetUnit(_unit))
                {
                    return;
                }

                SoundUtil.PlaySE(SEID.SE_MENU_OK);
                MainMenuParam.m_LinkTargetUnitUniqueId = _unit.UnitData.unique_id;
                _unit.IsSelectedUnit = true;
                updateLinkStatus();
                AndroidBackKeyManager.Instance.StackPush(SceneCommon.Instance.gameObject, OnSelectLinkBackKey);
            }
            else
            {
                //素材
                if (!CheckMaterialUnit(_unit.UnitData))
                {
                    return;
                }

                if (setMaterialUnit(_unit.UnitData.id, _unit.UnitData.unique_id))
                {
                    SoundUtil.PlaySE(SEID.SE_MENU_OK);
                }

                m_UnitGrid.UpdateList();
            }
            m_UnitBGPanel.IsActiveExecButton = IsLinkStart();
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

    private void OnSelectBaseBackKey()
    {
        if (IsBusy() == true)
        {
            return;
        }

        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        unsetBaseUnit();
        if (m_ExpandWindow.isOpen == true)
        {
            m_ExpandWindow.SetBackKey(false);
            m_ExpandWindow.Close();
            m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
            m_ExpandButtonOpen = false;
        }
    }

    private void OnSelectLinkBackKey()
    {
        if (IsBusy() == true)
        {
            return;
        }

        AndroidBackKeyManager.Instance.StackPop(SceneCommon.Instance.gameObject);
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        unsetTargetUnit();
    }

    /// <summary>
    ///
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
                //ボタン制御
                m_UnitBGPanel.IsActiveExecButton = IsLinkStart();

                SetupUnitSelected(_unit);
            }

            //素材選択シーケンスの場合はIconTypeを更新する
            if (m_BaseUnit != null &&
                m_TargetUnit != null)
            {
                SetupMaterialUnitIconType(_unit);
            }

            //更新データ反映
            m_UnitGrid.UpdateBaseItem(_unit);
        });
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

            //選択されていない
            if (!IsSelectUnit(unitBox[i].unique_id)) return unitBox[i].unique_id;
        }
        return 0;
    }

    /// <summary>
    /// 選択されている
    /// </summary>
    /// <param name="_unique_id"></param>
    /// <returns></returns>
    private bool IsSelectUnit(long _unique_id)
    {
        if (m_BaseUnit != null &&
            m_BaseUnit.unique_id == _unique_id) return true;
        if (m_TargetUnit != null &&
            m_TargetUnit.unique_id == _unique_id) return true;
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

    private bool IsLinkStart()
    {
        //--------------------------
        // ユニットアサインチェック
        //--------------------------
        if (m_BaseUnit == null
        || m_TargetUnit == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ベースユニットチェック
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckBaseUnit(PacketStructUnit _unit)
    {
        if (IsSelectUnit(_unit.unique_id)) return true;
        //ターゲットと同じユニットID（同一ユニット除く）
        if (m_TargetUnit != null &&
            m_TargetUnit.unique_id != _unit.unique_id &&
            m_TargetUnit.unit.id == _unit.id) return false;

        return true;
    }

    /// <summary>
    /// ターゲットユニットチェック
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckTargetUnit(UnitGridContext _unit)
    {
        if (IsSelectUnit(_unit.UnitData.unique_id)) return true;
        //リンクできないキャラクタ
        if (_unit.CharaMasterData.link_enable != MasterDataDefineLabel.BoolType.ENABLE) return false;
        //ベースと同じユニットID
        if (m_BaseUnit != null &&
            m_BaseUnit.unit.id == _unit.UnitData.id) return false;
        // パーティチェック
        if (MainMenuUtil.ChkUnitPartyAssign(_unit.UnitData.unique_id)) return false;
        //リンクしている・されている
        if (_unit.UnitData.link_info != (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE) return false;


        return true;
    }

    /// <summary>
    /// 素材ユニットチェック
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckMaterialUnit(PacketStructUnit _unit)
    {
        // 選択されている
        if (IsSelectUnit(_unit.unique_id)) return true;
        // 素材じゃない
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
    /// リンク素材が設定されているか？
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
    /// 確認フラグ設定
    /// </summary>
    /// <param name="bFlag"></param>
    private void setValidate(bool bFlag)
    {
        if (bFlag)
        {
            if (m_LinkType == LinkType.CREATE)
            {
                m_UnitBGPanel.ExecButtonImage = ResourceManager.Instance.Load("l_button");
                m_UnitBGPanel.Message = GameTextUtil.GetText("link_text");
            }
            else
            {
                m_UnitBGPanel.ExecButtonImage = ResourceManager.Instance.Load("d_button");
                m_UnitBGPanel.Message = GameTextUtil.GetText("linkout_text");
            }
            m_ExpandWindow.SetBackKey(false);
            m_ExpandWindow.SetBackKey(true);
            if (m_ExpandButtonOpen == false)
            {
                UnityUtil.SetObjectEnabledOnce(m_UnitStatusPanel.gameObject, true);
                UnityUtil.SetObjectEnabledOnce(m_UnitLinkPanel.gameObject, false);
                m_ExpandWindow.Open();
                m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                m_UnitBGPanel.LinkStatusImage = ResourceManager.Instance.Load("btn_status_down", ResourceType.Common);
            }
        }
        else
        {
            m_UnitBGPanel.ExecButtonImage = ResourceManager.Instance.Load("confirm_button");
            m_ExpandWindow.SetBackKey(false);
            if (m_ExpandButtonOpen == false)
            {
                m_ExpandWindow.Close();
                m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                m_UnitBGPanel.LinkStatusImage = ResourceManager.Instance.Load("btn_status", ResourceType.Common);
                m_UnitBGPanel.LinkSkillImage = ResourceManager.Instance.Load("btn_skill", ResourceType.Common);
            }
        }
        m_UnitBGPanel.IsViewPanel = bFlag;
        m_UnitGrid.IsActiveSortButton = !bFlag;
        m_Validate = bFlag;
    }

    private void OnBackKeyValidate()
    {
        AndroidBackKeyManager.Instance.StackPop(gameObject);
        setValidate(false);
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
    /// 素材対象かどうか？
    /// </summary>
    /// <param name="_chara_no"></param>
    /// <returns></returns>
    private bool IsMaterialUnit(uint _chara_no)
    {
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_CharaId == _chara_no) return true;
        }
        return false;
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
    /// 素材ユニット全解除
    /// </summary>
    /// <param name="_unique_id"></param>
    private void unsetMaterialUnitAll()
    {
        for (int i = 0; i < m_UnitMaterialPanel.MaterialList.Count; i++)
        {
            if (m_UnitMaterialPanel.MaterialList[i].m_UniqueId == 0)
            {
                continue;
            }

            //選択フラグ解除
            setUnitSelectFlag(m_UnitMaterialPanel.MaterialList[i].m_UniqueId, false);

            m_UnitMaterialPanel.MaterialList[i].setUnit(0);
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
    //----------------------------------------------------------------------------
    /*!
		@brief	ステータス更新
		@note
	*/
    //----------------------------------------------------------------------------
    void SetupLinkStatus()
    {
        uint iLinkMoney = 0;
        //-----------------------
        // リンク設定
        //-----------------------
        if (m_BaseUnit != null
        && m_TargetUnit != null)
        {
            //-----------------------
            // リンク実行費用
            // ※ベースユニットの設定値を参照する
            //-----------------------
            if (m_BaseUnit.unit.link_info == (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE
            && m_TargetUnit.unit.link_info == (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
            {
                iLinkMoney = m_BaseCharaMaster.link_money;
            }
            //-----------------------
            // リンク解除費用
            // ※ベースユニットの設定値を参照する
            //-----------------------
            else if (m_BaseUnit.unit.link_info != (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE
            && m_TargetUnit.unit.link_info != (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
            {
                iLinkMoney = m_BaseCharaMaster.link_del_money;
            }
            bool bFake = (m_LinkType == LinkType.CREATE) ? true : false;
            m_UnitStatusPanel.setupUnit(m_BaseUnit.unit, m_TargetUnit.unit, bFake, true);

            //----------------------------------------
            // 初心者ブースト適用
            // 表示用の値を計算、補正値を適用
            // ※費用倍率が1倍の場合は表示反映しない
            //----------------------------------------
            if (MainMenuParam.m_BeginnerBoost != null
            && MainMenuParam.m_BeginnerBoost.boost_build_money != 100)
            {
                iLinkMoney = MasterDataUtil.ConvertBeginnerBoostBuildMoney(ref MainMenuParam.m_BeginnerBoost, iLinkMoney);
            }
        }
        else if (m_BaseUnit != null)
        {
            m_UnitStatusPanel.setupUnit(m_BaseUnit.unit);
        }

        m_UnitBGPanel.Money = (int)iLinkMoney;

        if (m_TargetUnit != null)
        {
            m_UnitLinkPanel.setupSkill(m_TargetUnit.master, m_BaseUnit.unit);
        }
        else
        {
            m_UnitLinkPanel.setupSkill(null);
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
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterLinkUnit());
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
        LocalSaveManager.Instance.SaveFuncSortFilterLinkUnit(sortInfo);

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
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
        }
        else
        {
            _unit.UnitIconType = MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON;
        }
    }

    void SetupTargetUnitIconType(UnitGridContext _unit)
    {
        //ベースとして選択できるか？
        if (CheckTargetUnit(_unit))
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
        return false;
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        base.PageSwitchEventDisableAfter(eNextMainMenuSeq);
        AndroidBackKeyManager.Instance.StackPop(SceneCommon.Instance.gameObject);
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

    private void OnClickLinkStatus()
    {
        if (m_BaseUnit == null)
        {
            return;
        }
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (UnityUtil.ChkObjectEnabled(m_UnitStatusPanel.gameObject) == false)
        {
            UnityUtil.SetObjectEnabledOnce(m_UnitStatusPanel.gameObject, true);
            UnityUtil.SetObjectEnabledOnce(m_UnitLinkPanel.gameObject, false);
            m_UnitBGPanel.LinkStatusImage = ResourceManager.Instance.Load("btn_status_down", ResourceType.Common);
            m_UnitBGPanel.LinkSkillImage = ResourceManager.Instance.Load("btn_skill", ResourceType.Common);
            if (m_ExpandWindow.isOpen == false)
            {
                m_ExpandWindow.Open();
                m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                m_ExpandButtonOpen = true;
            }
        }
        else
        {
            if (m_ExpandWindow.isOpen == false)
            {
                UnityUtil.SetObjectEnabledOnce(m_UnitLinkPanel.gameObject, false);
                m_ExpandWindow.Open();
                m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                m_ExpandButtonOpen = true;
                m_UnitBGPanel.LinkStatusImage = ResourceManager.Instance.Load("btn_status_down", ResourceType.Common);
            }
            else
            {
                m_ExpandWindow.Close();
                m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                m_ExpandButtonOpen = false;
                m_UnitBGPanel.LinkStatusImage = ResourceManager.Instance.Load("btn_status", ResourceType.Common);
            }
        }
    }

    private void OnClickLinkSkill()
    {
        if (m_BaseUnit == null)
        {
            return;
        }
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (UnityUtil.ChkObjectEnabled(m_UnitLinkPanel.gameObject) == false)
        {
            UnityUtil.SetObjectEnabledOnce(m_UnitStatusPanel.gameObject, false);
            UnityUtil.SetObjectEnabledOnce(m_UnitLinkPanel.gameObject, true);
            m_UnitBGPanel.LinkStatusImage = ResourceManager.Instance.Load("btn_status", ResourceType.Common);
            m_UnitBGPanel.LinkSkillImage = ResourceManager.Instance.Load("btn_skill_down", ResourceType.Common);
            if (m_ExpandWindow.isOpen == false)
            {
                m_ExpandWindow.Open();
                m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                m_ExpandButtonOpen = true;
            }
        }
        else
        {
            if (m_ExpandWindow.isOpen == false)
            {
                UnityUtil.SetObjectEnabledOnce(m_UnitStatusPanel.gameObject, false);
                m_ExpandWindow.Open();
                m_UnitGrid.changeGridWindowSize(true, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                m_ExpandButtonOpen = true;
                m_UnitBGPanel.LinkSkillImage = ResourceManager.Instance.Load("btn_skill_down", ResourceType.Common);
            }
            else
            {
                m_ExpandWindow.Close();
                m_UnitGrid.changeGridWindowSize(false, (m_ExpandWindow != null) ? m_ExpandWindow.ViewHeightSize : 0);
                m_ExpandButtonOpen = false;
                m_UnitBGPanel.LinkSkillImage = ResourceManager.Instance.Load("btn_skill", ResourceType.Common);
            }
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
