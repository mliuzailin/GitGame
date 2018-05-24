using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class DebugQuestAllClear : M4uContextMonoBehaviour
{
    [SerializeField]
    private Toggle m_FeatureQuestToggle = null;

    private bool m_isSend = false;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
#if BUILD_TYPE_DEBUG
        if (m_FeatureQuestToggle != null)
        {
            m_FeatureQuestToggle.isOn = DebugOption.Instance.featureQuest;
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClearOn(int _clear)
    {
        SendDebugEditUser(_clear);
    }

    public void OnChangeFeatureQuestToggle()
    {
        if (m_FeatureQuestToggle != null)
        {
#if BUILD_TYPE_DEBUG
            if (DebugOption.Instance.featureQuest != m_FeatureQuestToggle.isOn)
            {
                DebugOption.Instance.featureQuest = m_FeatureQuestToggle.isOn;
                TimeEventManager.Instance.TimeEventUpdateRequest();
            }
#endif
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	デバッグ処理：クエストクリアフラグ制御：
		@param[in]	int		(_clear)		クエストクリアフラグ
											0 : 全クエストのクリアフラグを折る
											1 : 全クエストのクリアフラグを立てる
											2 : 全ミッションコンプリートのフラグを折る
											3 : 全ミッションコンプリートのフラグを立てる
	*/
    //----------------------------------------------------------------------------
    private void SendDebugEditUser(int _claer)
    {
        ServerDataUtilSend.SendPacketAPI_DebugEditUser(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, _claer, 0).
            setSuccessAction(_data =>
            {
                UserDataAdmin.Instance.m_StructPlayer = _data.GetResult<RecvDebugEditUser>().result.player;
                UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvDebugEditUser>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                UserDataAdmin.Instance.ConvertPartyAssing();

                m_isSend = false;
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
}
