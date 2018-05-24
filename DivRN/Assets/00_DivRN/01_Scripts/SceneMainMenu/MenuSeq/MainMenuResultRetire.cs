using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuResultRetire : MainMenuSeq
{
    private uint m_QuestId = 0;
    private bool m_PageSwitchNext = false;
    private Dialog m_ErrorDialog = null;

    private MasterDataQuest2 m_QuestMaster = null;
    private MasterDataArea m_AreaMaster = null;
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

        if (m_ErrorDialog != null)
        {
            if (m_ErrorDialog.PushButton != DialogButtonEventType.NONE)
            {
                m_PageSwitchNext = true;

                m_ErrorDialog.Hide();
                m_ErrorDialog = null;
            }
        }

        if (m_PageSwitchNext)
        {
            if (MainMenuManager.s_LastLoginTime == 0)
            {
                //クエスト中断から開始してログインパックを取得していないときは
                //ログインパック取得遷移へ
                MainMenuParam.m_DateChangeType = DATE_CHANGE_TYPE.LOGIN;
                MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_DATE_CHANGE, false, false);
            }
            else if (m_AreaMaster != null)
            {
                switch (MasterDataUtil.GetQuestType(m_QuestId))
                {
                    case MasterDataDefineLabel.QuestType.NORMAL:
                        {
                            //クエスト選択に戻る
                            MainMenuParam.SetQuestSelectParam(m_AreaMaster.area_cate_id, m_AreaMaster.fix_id);
                            if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false, true))
                            {
                                m_PageSwitchNext = false;
                            }
                        }
                        break;
                    case MasterDataDefineLabel.QuestType.CHALLENGE:
                        {
                            //成長ボス選択に戻る
                            MainMenuParam.SetChallengeSelectParam(m_QuestId);
                            if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_CHALLENGE_SELECT, false, false))
                            {
                                m_PageSwitchNext = false;
                            }
                        }
                        break;
                    default:
                        {
                            //ホームに戻る
                            if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false))
                            {
                                m_PageSwitchNext = false;
                            }
                        }
                        break;
                }
            }
            else
            {
                //ホームに戻る
                if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false))
                {
                    m_PageSwitchNext = false;
                }
            }

        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        sendQuestRetire();

        AndroidBackKeyManager.Instance.EnableBackKey();
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnSelectBackKey);
    }

    public override bool PageSwitchEventDisableBefore()
    {
        base.PageSwitchEventDisableBefore();
        AndroidBackKeyManager.Instance.StackPop(gameObject);
        return false;
    }

    private void sendQuestRetire()
    {
        m_PageSwitchNext = false;
        m_QuestId = SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire.m_QuestID;
        bool is_auto_play = SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire.m_IsUsedAutoPlay;
        m_QuestMaster = MasterDataUtil.GetQuest2ParamFromID(m_QuestId);
        if (m_QuestMaster != null)
        {
            m_AreaMaster = MasterFinder<MasterDataArea>.Instance.Find((int)m_QuestMaster.area_id);
        }

        //--------------------------------
        // ローカルセーブにあるリザルト情報を破棄
        //--------------------------------
        LocalSaveManager.Instance.SaveFuncGoesToMenuRetire(null);

        switch (MasterDataUtil.GetQuestType(m_QuestId))
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {
                    ServerDataUtilSend.SendPacketAPI_QuestRetire(m_QuestId, is_auto_play)
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
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    ServerDataUtilSend.SendPacketAPI_ChallengeQuestRetire(m_QuestId, is_auto_play)
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
                break;
            default:
                //Home画面へ
                m_PageSwitchNext = true;
                break;
        }
    }

    private void resultSuccess(ServerApi.ResultData data)
    {
        UserDataAdmin.Instance.m_StructPlayer = data.UpdateStructPlayer<RecvQuestRetire>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
        UserDataAdmin.Instance.ConvertPartyAssing();

        //Home画面へ
        m_PageSwitchNext = true;
    }

    private void resultError(ServerApi.ResultData data)
    {
        string strInvalidTitle = GameTextUtil.GetText("ERROR_MSG_IRREGULAR_TITLE");
        string strInvalidMsg = string.Format(GameTextUtil.GetText("ERROR_MSG_IRREGULAR"), (int)data.m_PacketCode);
        m_ErrorDialog = DialogManager.Open1B_Direct(strInvalidTitle, strInvalidMsg, "common_button7", true, true);
    }

    private void OnSelectBackKey()
    {

    }

}
