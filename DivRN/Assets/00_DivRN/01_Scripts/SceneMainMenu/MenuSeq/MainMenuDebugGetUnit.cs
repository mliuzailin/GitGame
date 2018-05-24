/**
 *  @file   MainMenuDebugGetUnit.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/15
 */

using UnityEngine;
using System.Collections;
using System;
using ServerDataDefine;
using System.Collections.Generic;

public class MainMenuDebugGetUnit : MainMenuSeq
{
    const int UNIT_NUM = 12;

    DebugGetUnitSelectPanel m_DebugGetUnitSelectPanel = null;
    UnitCatalog m_UnitCatalog = null;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
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

        // TODO: ページ解説テキスト設定

        // TODO: タイトル設定

        m_DebugGetUnitSelectPanel = m_CanvasObj.GetComponentInChildren<DebugGetUnitSelectPanel>();
        m_UnitCatalog = m_CanvasObj.GetComponentInChildren<UnitCatalog>();


        if (m_DebugGetUnitSelectPanel != null)
        {
            m_DebugGetUnitSelectPanel.Units.Clear();
            for (int i = 0; i < UNIT_NUM; i++)
            {
                DebugGetUnitListItemContext unit = new DebugGetUnitListItemContext();
                unit.ClickAction = OnClickSelectedUnit;
                unit.LongPressAction = OnLongPressSelectedUnit;
                m_DebugGetUnitSelectPanel.Units.Add(unit);
            }
            m_DebugGetUnitSelectPanel.ClickFixButtonAction = OnClickFixButton;
            m_DebugGetUnitSelectPanel.ClickResetButtonAction = OnClickResetButton;
            m_DebugGetUnitSelectPanel.ClickSearchAddButtonAction = OnClickSearchAddButton;
            m_DebugGetUnitSelectPanel.ResetParam();

            if (SafeAreaControl.HasInstance)
            {
                float bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
                if (bottom_space_height > 0)
                {
                    float height = (bottom_space_height / 2) * -1;
                    m_DebugGetUnitSelectPanel.transform.AddLocalPositionY(height);
                }
            }
        }

        if (m_UnitCatalog != null)
        {
            m_UnitCatalog.SetPositionAjustStatusBar(new Vector2(0, -112f), new Vector2(0, -495f));
            m_UnitCatalog.LabelText = "";
            m_UnitCatalog.IsActiveLabel = false;

            if (m_UnitCatalog.MasterCatalogList.Count == 0)
            {
                MasterDataParamChara[] charamsterArray = MasterFinder<MasterDataParamChara>.Instance.GetAll();
                for (int i = 0; i < charamsterArray.Length; ++i)
                {
                    if (charamsterArray[i] == null) { continue; }
                    if (charamsterArray[i].fix_id == 0) { continue; }

                    UnitCatalogItemContext unit = new UnitCatalogItemContext();
                    unit.master = charamsterArray[i];
                    unit.Index = (int)unit.master.draw_id;
                    unit.DidSelectItem = OnClickUnit;
                    unit.DidLongPressItem = OnLongPressUnit;
                    /*
                                        UnitIconImageProvider.Instance.Get(
                                            unit.master.fix_id,
                                            sprite => 
                                            {
                                                unit.IconImage = sprite;
                                            });
                    */
                    m_UnitCatalog.MasterCatalogList.Add(unit);
                }

                m_UnitCatalog.Init();
            }
        }
    }

    void CreateSendUnitGetDatas(bool isLink, bool isEvol)
    {
        List<PacketStructUnitGetDebug> cUnitGetList = new List<PacketStructUnitGetDebug>();
        MasterDataParamChara cCharaMasterData; // ユニットのマスターデータ
        PacketStructUnitGetDebug cUnitGet; // ユニットデータ
        MasterDataParamCharaEvol cEvolMasterData;// 進化素材のマスターデータ
        MasterDataParamChara cCLinkharaMasterData; // リンク素材のマスターデータ
        int nLimitOverMax; // 限界突破レベルの最大値

        for (int i = 0; i < m_DebugGetUnitSelectPanel.Units.Count; i++)
        {

            // 選択ユニット追加
            if (m_DebugGetUnitSelectPanel.Units[i].unitID == 0)
            {
                continue;
            }
            cUnitGet = new PacketStructUnitGetDebug();
            cUnitGet.id = m_DebugGetUnitSelectPanel.Units[i].unitID;
            cUnitGet.limitbreak_lv = m_DebugGetUnitSelectPanel.m_UnitGetData.limitbreak_lv;
            cUnitGet.limitover_lv = m_DebugGetUnitSelectPanel.m_UnitGetData.limitover_lv;
            cUnitGet.level = m_DebugGetUnitSelectPanel.m_UnitGetData.level;
            cUnitGet.add_pow = m_DebugGetUnitSelectPanel.m_UnitGetData.add_pow;
            cUnitGet.add_hp = m_DebugGetUnitSelectPanel.m_UnitGetData.add_hp;

            cCharaMasterData = MasterDataUtil.GetCharaParamFromID(cUnitGet.id);
            nLimitOverMax = (int)CharaLimitOver.GetParam(0, cCharaMasterData.limit_over_type, (int)CharaLimitOver.EGET.ePARAM_LIMITOVER_MAX);
            cUnitGet.limitover_lv = (nLimitOverMax < (int)cUnitGet.limitover_lv) ? (uint)nLimitOverMax : cUnitGet.limitover_lv;

            cUnitGetList.Add(cUnitGet);


            // 進化ユニット追加
            if (isEvol)
            {
                cEvolMasterData = MasterDataUtil.GetCharaEvolParamFromCharaID(cUnitGet.id);
                if (cEvolMasterData != null)
                {
                    if (cEvolMasterData.unit_id_parts1 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGetEvol1 = new PacketStructUnitGetDebug();
                        cUnitGetEvol1.id = cEvolMasterData.unit_id_parts1;
                        cUnitGetEvol1.limitbreak_lv = cUnitGet.limitbreak_lv;
                        cUnitGetEvol1.level = cUnitGet.level;
                        cUnitGetEvol1.add_hp = cUnitGet.add_hp;
                        cUnitGetEvol1.add_pow = cUnitGet.add_pow;

                        cUnitGetList.Add(cUnitGetEvol1);
                    }
                    if (cEvolMasterData.unit_id_parts2 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGetEvol2 = new PacketStructUnitGetDebug();
                        cUnitGetEvol2.id = cEvolMasterData.unit_id_parts2;
                        cUnitGetEvol2.limitbreak_lv = cUnitGet.limitbreak_lv;
                        cUnitGetEvol2.level = cUnitGet.level;
                        cUnitGetEvol2.add_hp = cUnitGet.add_hp;
                        cUnitGetEvol2.add_pow = cUnitGet.add_pow;

                        cUnitGetList.Add(cUnitGetEvol2);
                    }
                    if (cEvolMasterData.unit_id_parts3 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGetEvol3 = new PacketStructUnitGetDebug();
                        cUnitGetEvol3.id = cEvolMasterData.unit_id_parts3;
                        cUnitGetEvol3.limitbreak_lv = cUnitGet.limitbreak_lv;
                        cUnitGetEvol3.level = cUnitGet.level;
                        cUnitGetEvol3.add_hp = cUnitGet.add_hp;
                        cUnitGetEvol3.add_pow = cUnitGet.add_pow;

                        cUnitGetList.Add(cUnitGetEvol3);
                    }
                }
            }

            // リンクユニット追加
            if (isLink)
            {
                cCLinkharaMasterData = MasterDataUtil.GetCharaParamFromID(cUnitGet.id);
                if (cCLinkharaMasterData != null)
                {
                    if (cCLinkharaMasterData.link_unit_id_parts1 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGetLink1 = new PacketStructUnitGetDebug();
                        cUnitGetLink1.id = cCLinkharaMasterData.link_unit_id_parts1;
                        cUnitGetLink1.limitbreak_lv = cUnitGet.limitbreak_lv;
                        cUnitGetLink1.level = cUnitGet.level;
                        cUnitGetLink1.add_hp = cUnitGet.add_hp;
                        cUnitGetLink1.add_pow = cUnitGet.add_pow;

                        cUnitGetList.Add(cUnitGetLink1);
                    }
                    if (cCLinkharaMasterData.link_unit_id_parts2 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGetLink2 = new PacketStructUnitGetDebug();
                        cUnitGetLink2.id = cCLinkharaMasterData.link_unit_id_parts2;
                        cUnitGetLink2.limitbreak_lv = cUnitGet.limitbreak_lv;
                        cUnitGetLink2.level = cUnitGet.level;
                        cUnitGetLink2.add_hp = cUnitGet.add_hp;
                        cUnitGetLink2.add_pow = cUnitGet.add_pow;

                        cUnitGetList.Add(cUnitGetLink2);
                    }
                    if (cCLinkharaMasterData.link_unit_id_parts3 != 0)
                    {
                        PacketStructUnitGetDebug cUnitGetLink3 = new PacketStructUnitGetDebug();
                        cUnitGetLink3.id = cCLinkharaMasterData.link_unit_id_parts3;
                        cUnitGetLink3.limitbreak_lv = cUnitGet.limitbreak_lv;
                        cUnitGetLink3.level = cUnitGet.level;
                        cUnitGetLink3.add_hp = cUnitGet.add_hp;
                        cUnitGetLink3.add_pow = cUnitGet.add_pow;

                        cUnitGetList.Add(cUnitGetLink3);
                    }
                }
            }
        }

        if (cUnitGetList.Count > 0)
        {
            //----------------------------------------
            // 追加ユニット数を10倍にする
            //----------------------------------------
            if (m_DebugGetUnitSelectPanel.IsMultiplyUnit)
            {
                List<PacketStructUnitGetDebug> unitGetMultiplyList = new List<PacketStructUnitGetDebug>();
                foreach (PacketStructUnitGetDebug unit in cUnitGetList)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        PacketStructUnitGetDebug copyUnit = new PacketStructUnitGetDebug();
                        copyUnit.Copy(unit);
                        unitGetMultiplyList.Add(copyUnit);
                    }
                }
                cUnitGetList = unitGetMultiplyList;
            }

            //----------------------------------------
            // リクエスト開始
            //----------------------------------------
            if (cUnitGetList.Count > 100 || m_DebugGetUnitSelectPanel.IsMultiplyUnit)
            {
                // 10倍すると、かなり数が多くなるので警告を出しておく
                Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
                newDialog.SetDialogText(DialogTextType.Title, "ユニット取得");
                newDialog.SetDialogText(DialogTextType.MainText, string.Format("{0}体のユニットを取得しようとしています。\nよろしいですか。", cUnitGetList.Count));
                newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
                newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
                newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
                {
                    SendGetDebugUnit(cUnitGetList.ToArray()); // リクエスト
                });
                newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
                {
                });
                newDialog.Show();
            }
            else
            {
                SendGetDebugUnit(cUnitGetList.ToArray()); // リクエスト
            }
        }
    }

    #region API通信
    void SendGetDebugUnit(PacketStructUnitGetDebug[] unitGetDatas)
    {
        ServerApi api = ServerDataUtilSend.SendPacketAPI_DebugUnitGet(unitGetDatas);
        api.setSuccessAction(ReceiveGetUnitSuccess);
        api.setErrorAction(ReceiveGetUnitError);
        api.SendStart();
    }

    void ReceiveGetUnitSuccess(ServerApi.ResultData data)
    {
        PacketStructPlayer player = data.GetResult<RecvDebugUnitGet>().result.player;
        if (player != null)
        {
            // ユーザーデータ情報を更新
            UserDataAdmin.Instance.m_StructPlayer = data.UpdateStructPlayer<RecvDebugUnitGet>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();

            // 成功ダイアログ
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogText(DialogTextType.Title, "ユニット取得");
            newDialog.SetDialogText(DialogTextType.MainText, "ユニットを追加しました");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
                m_DebugGetUnitSelectPanel.ResetParam();
            }));
            newDialog.Show();
        }
    }

    void ReceiveGetUnitError(ServerApi.ResultData data)
    {
        // 失敗ダイアログ
        if (data == null || data.m_PacketCode != API_CODE.API_CODE_SUCCESS)
        {
            uint unUserID = LocalSaveManager.Instance.LoadFuncUserID();
            string strUserID = UnityUtil.CreateDrawUserID(unUserID);

            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "パーミッションエラー");
            newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("アカウントのステータスが開発ユーザーではありません。\n管理ツールからユーザーのステータスを「」開発ユーザーに変更してください。\n\nID:{0}"), strUserID));
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
            }));
            newDialog.Show();
        }

        m_DebugGetUnitSelectPanel.ResetParam();
    }
    #endregion

    void OnClickFixButton()
    {
        CreateSendUnitGetDatas(m_DebugGetUnitSelectPanel.IsLinkMaterial, m_DebugGetUnitSelectPanel.IsEvolMaterial);
    }

    void OnClickResetButton()
    {
        m_DebugGetUnitSelectPanel.ResetParam();
    }

    void OnClickSearchAddButton(uint unit_id)
    {
        m_DebugGetUnitSelectPanel.AddUnit(unit_id);
    }

    /// <summary>
    /// 追加候補のユニットを選択したとき
    /// </summary>
    /// <param name="unit"></param>
    void OnClickSelectedUnit(DebugGetUnitListItemContext unit)
    {
        m_DebugGetUnitSelectPanel.DeleteUnit(unit);
    }

    /// <summary>
    /// 追加候補のユニットを長押ししたとき
    /// </summary>
    /// <param name="unit"></param>
    void OnLongPressSelectedUnit(DebugGetUnitListItemContext unit)
    {
        if (unit.unitID == 0) { return; }

        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.OpenUnitDetailInfoCatalog(unit.unitID);
        }
    }


    /// <summary>
    /// カタログリストのユニットを選択したとき
    /// </summary>
    /// <param name="unit_id"></param>
    void OnClickUnit(uint unit_id)
    {
        m_DebugGetUnitSelectPanel.AddUnit(unit_id);
    }

    /// <summary>
    /// カタログリストのユニットを長押ししたとき
    /// </summary>
    /// <param name="unit_id"></param>
    void OnLongPressUnit(uint unit_id)
    {
        if (unit_id == 0) { return; }

        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.OpenUnitDetailInfoCatalog(unit_id);
        }
    }

}
