using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class DebugUserRankUp : MenuPartsBase
{
    public InputField[] m_InputField = null;
    public Toggle m_ToggleObj = null;
    public Toggle m_ToggleObj2 = null;

    enum INPUT_INDEX : int
    {
        INPUT_INDEX_RANK = 0,
        INPUT_INDEX_COIN,
        INPUT_INDEX_FREE_CHIP,
        INPUT_INDEX_FRIEND_POINT,
        INPUT_INDEX_BUY_UNIT,
        INPUT_INDEX_BUY_FRIEND,
        INPUT_INDEX_TICKET,
        INPUT_INDEX_UNIT_POINT,
        INPUT_INDEX_EVENT_P_ID,
        INPUT_INDEX_EVENT_P_VALUE,
        INPUT_INDEX_RESET_GACHA_ID,
        INPUT_INDEX_MAX,
    };

    private int[] m_InputValue = null;
    private int m_QuestClear = 2;
    private bool m_bQuestClear = false;


    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_InputValue = new int[(int)INPUT_INDEX.INPUT_INDEX_MAX];
        for (int i = 0; i < (int)INPUT_INDEX.INPUT_INDEX_MAX; ++i)
        {
            m_InputValue[i] = 0;
            m_InputField[i].text = "0";
        }
    }

    // Use this for initialization
    void Start()
    {
        SetTopAndBottomAjustStatusBar(new Vector2(0, -270));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnReset()
    {
        for (int i = 0; i < (int)INPUT_INDEX.INPUT_INDEX_MAX; ++i)
        {
            m_InputValue[i] = 0;
            m_InputField[i].text = "0";
        }
        m_QuestClear = 1;
        m_bQuestClear = false;
        m_ToggleObj.isOn = false;
        m_ToggleObj2.isOn = true;
    }

    public void OnAdd()
    {
        if (m_bQuestClear == false) m_QuestClear = 2;
        ServerDataUtilSend.SendPacketAPI_DebugEditUser(m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_COIN],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_FREE_CHIP],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_FRIEND_POINT],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_RANK],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_BUY_UNIT],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_BUY_FRIEND],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_TICKET],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_UNIT_POINT],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_EVENT_P_ID],
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_EVENT_P_VALUE],
                                                        m_QuestClear,
                                                        m_InputValue[(int)INPUT_INDEX.INPUT_INDEX_RESET_GACHA_ID]
                                                        ).
            setSuccessAction(_data =>
            {
                RecvDebugEditUserValue result = _data.GetResult<RecvDebugEditUser>().result;
                UserDataAdmin.Instance.m_StructPlayer = result.player;
                UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvDebugEditUser>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                UserDataAdmin.Instance.ConvertPartyAssing();
                UserDataAdmin.Instance.ResetGachaStatus();
                if (result.gacha_status != null)
                {
                    UserDataAdmin.Instance.UpdateGachaStatusList(result.gacha_status);
                }

                SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            }).
            setErrorAction(data =>
            {
                if (data.m_PacketCode == API_CODE.API_CODE_DEBUG_ERROR_PERMISSION)
                {
                    uint unUserID = LocalSaveManager.Instance.LoadFuncUserID();
                    string strUserID = UnityUtil.CreateDrawUserID(unUserID);
                    Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                    newDialog.SetDialogText(DialogTextType.Title, "パーミッションエラー");
                    newDialog.SetDialogText(DialogTextType.MainText, string.Format("アカウントのステータスが開発ユーザではありません。\n管理ツールからユーザーのステータスを「開発ユーザー」に変更してください。\n\nID：{0}", strUserID));
                    newDialog.SetDialogText(DialogTextType.OKText, Dialog.CONFIRM_BUTTON_TITLE);
                    newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
                    {
                    }));
                    newDialog.EnableFadePanel();
                    newDialog.DisableCancelButton();
                    newDialog.Show();
                    SoundUtil.PlaySE(SEID.SE_MENU_NG);
                }
            }).
            SendStart();
    }

    public void OnInputEnd(int index)
    {
        if (index < 0 || index >= (int)INPUT_INDEX.INPUT_INDEX_MAX)
        {
            return;
        }
        m_InputValue[index] = m_InputField[index].text.ToInt(0);
    }

    public void OnToggle(bool sw)
    {
        m_bQuestClear = sw;
    }

    public void OnToggle2(bool sw)
    {
        if (sw == true)
        {
            m_QuestClear = 1;
        }
        else
        {
            m_QuestClear = 0;
        }
    }
}
