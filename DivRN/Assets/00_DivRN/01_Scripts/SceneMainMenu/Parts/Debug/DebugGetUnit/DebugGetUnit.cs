/**
 *  @file   DebugGetUnit.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/14
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;
using M4u;
using System;

public class DebugGetUnit : M4uContextMonoBehaviour
{
    const int UNIT_NUM = 10;

    M4uProperty<List<DebugGetUnitListItemContext>> units = new M4uProperty<List<DebugGetUnitListItemContext>>(new List<DebugGetUnitListItemContext>());
    public List<DebugGetUnitListItemContext> Units
    {
        get
        {
            return units.Value;
        }
        set
        {
            units.Value = value;
        }
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        //CreateList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateList()
    {
        Units.Clear();

        DebugGetUnitListItemContext unit;
        for (int i = 0; i < UNIT_NUM; ++i)
        {
            unit = new DebugGetUnitListItemContext();
            unit.UnitGetData.level = 1;
            Units.Add(unit);
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

        for (int i = 0; i < Units.Count; i++)
        {

            // 選択ユニット追加
            if (Units[i].UnitGetData.id == 0)
            {
                continue;
            }

            cUnitGet = Units[i].UnitGetData;
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
            SendGetDebugUnit(cUnitGetList.ToArray()); // リクエスト
        }
    }

    /// <summary>
    /// リセットボタンを押したとき
    /// </summary>
    public void OnClickReset()
    {
        CreateList();
    }

    /// <summary>
    /// 追加ボタンを押したとき
    /// </summary>
    public void OnClickAdd()
    {
        CreateSendUnitGetDatas(false, false);
    }

    /// <summary>
    /// 進化ユニットも追加ボタンを押したとき
    /// </summary>
    public void OnClickAddEvol()
    {
        CreateSendUnitGetDatas(false, true);
    }

    /// <summary>
    /// リンクユニットも追加ボタンを押したとき
    /// </summary>
    public void OnClickAddLink()
    {
        CreateSendUnitGetDatas(true, false);
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
            newDialog.SetDialogText(DialogTextType.Title, "GetUnit");
            newDialog.SetDialogText(DialogTextType.MainText, "ユニットを追加しました");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
                CreateList();
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
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_DEBUG_PERMISSION_TITLE");
            newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("ERROR_DEBUG_PERMISSION_DETAIL"), strUserID));
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
            }));
            newDialog.Show();
        }

        CreateList();
    }
    #endregion
}
