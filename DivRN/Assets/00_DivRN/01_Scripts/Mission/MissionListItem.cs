using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerDataDefine;
using TMPro;

public class MissionListItem : ListItem<MasterDataAchievementConverted>
{
    public Image icon;
    public TextMeshProUGUI count;
    public GameObject limitParentGO;
    public TextMeshProUGUI limit;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI itemName;
    public Image progressFill;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI progressMaxText;
    public Button ItemButton;
    public Button ReceiveButton;

    public Image iconSprite;
    public Image frameSprite;

    public GameObject stateProgressGO;
    public GameObject stateAchieveGO;
    public GameObject stateRewardedGO;
    public GameObject hasProgressCountGO;
    public GameObject hasNotProgressCountGO;

    private MissionGroup MissionGroup
    {
        get
        {
            return transform.GetComponentInParent<MissionGroup>();
        }
    }

    private MasterDataAchievementConverted Achievement
    {
        get
        {
            return Context;
        }
    }


    private uint RestCount
    {
        get
        {
            return Achievement.RestCount;
        }
    }


    void Start()
    {
        gameObject.name += "::" + Achievement.fix_id.ToString() + "::" + Achievement.server_state;

        if (!Achievement.IsState_Appear)
        {
            Debug.LogError("Why i am here?");
            return;
        }

        stateProgressGO.SetActive(false);
        stateAchieveGO.SetActive(false);
        stateRewardedGO.SetActive(false);

#if BUILD_TYPE_DEBUG
        //Debug.Log("MissionListItem:" + gameObject.name + ":" + Achievement.AchievementState);
#endif

        //解説
        desc.text = Achievement.Desc;
        // アイテム名
        itemName.text = Achievement.PresentName;
        //プレゼントアイコン
        Achievement.GetPresentIcon(sprite => { icon.sprite = sprite; });
        //プレゼント数
        count.text = Achievement.PresentCount.ToString();

        limitParentGO.SetActive(false);

        // リストを押せるかどうか
        if (Achievement.achievement_category_id == (uint)ACHIEVEMENT_CATEGORY_TYPE.QUEST)
        {
            ItemButton.interactable = true;
        }
        else
        {
            ItemButton.interactable = false;
        }

        //状態：報酬獲得済み
        if (Achievement.IsState_Rewarded)
        {
            stateRewardedGO.SetActive(true);
            return;
        }

        //状態：条件達成済み
        if (Achievement.IsState_Achieve ||
            Achievement.IsState_AchieveEffect)
        {
            stateAchieveGO.SetActive(true);
            //期限
            if (Achievement.HasExpira)
            {
                limitParentGO.SetActive(true);
                limit.text = DivRNUtil.DiffTimePresentLabel(Achievement.ExpiraDate, TimeManager.Instance.m_TimeNow) + "\n";
            }

            return;
        }

        //以降、状態：出現
        stateProgressGO.SetActive(true);

        //期限
        if (Achievement.HasExpira)
        {
            limitParentGO.SetActive(true);
            limit.text = DivRNUtil.DiffTimeMissonLabel(Achievement.ExpiraDate, TimeManager.Instance.m_TimeNow) + "\n";
        }


        //現在の進行度
        hasProgressCountGO.SetActive(false);
        hasNotProgressCountGO.SetActive(false);

        if (Achievement.TotalCount > 0)
        {
            hasProgressCountGO.SetActive(true);
            progressFill.fillAmount = Achievement.ProgressRate;
            //progressText.text = string.Format("{0}/{1}", Achievement.ProgressCount, Achievement.TotalCount);
            progressText.text = string.Format("{0}", Achievement.ProgressCount);
            progressMaxText.text = string.Format("{0}", Achievement.TotalCount);
        }
        else
        {
            hasNotProgressCountGO.SetActive(true);
        }
    }

    public void OnClickReceiveButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnClickReceiveButton:" + gameObject.name);
#endif
        //連打防止
        if (ServerApi.IsExists)
        {
            return;
        }

        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

        if (GlobalMenuManager.Instance.IsCangeTime())
        {
            return;
        }
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        ServerDataUtilSend.SendPacketAPI_AchievementOpen(
                new uint[]
                {
                    Achievement.fix_id
                },
                null).
            setSuccessAction(
                (_data) =>
                {
                    //ユーザー情報更新
                    UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvOpenAchievement>(UserDataAdmin.Instance.m_StructPlayer);
                    UserDataAdmin.Instance.ConvertPartyAssing();

                    RecvOpenAchievementValue result = _data.GetResult<RecvOpenAchievement>().result;
                    MissionOneOpenResultMessage(result);
                    MissionGroup.SetEnaleReceiveButtonAll(false);
                    MissionGroup.Activate();

                    // 開封した新規達成アチーブメントを削除
                    ResidentParam.DelAchievementClear(result.achievement_opened);
#if BUILD_TYPE_DEBUG
                    Debug.LogError("FINISH:" + ((result.achievement_opened != null) ? result.achievement_opened.Length : 0));
#endif
                    MainMenuManager.Instance.UpdateUserStatusFromGlobalMenu();
                }).
            setErrorAction(
                (_data) =>
                {
                    Debug.LogError("ERROR:" + _data.m_PacketCode);
                    MissionGroup.SetEnaleReceiveButtonAll(true);
                }).
            SendStart();
    }

    /// <summary>
    /// リストが選択された時
    /// </summary>
    public void OnClickItem()
    {
        if (GlobalMenuManager.Instance.IsPageClosing() == true)
        {
            return;
        }

        if (Achievement.quest_id == 0)
        {
            return;
        }

        MasterDataQuest2 questMaster = MasterDataUtil.GetQuest2ParamFromID(Achievement.quest_id);
        if (questMaster == null)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);


        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mission_questtub_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "mission_questtub_text");
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "mission_questtub_button");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            //---------------------------------
            // クエスト詳細に遷移する
            //---------------------------------
            MainMenuParam.MoveQuestDetail(questMaster);
            // メニュー閉じる
            MainMenuManager.Instance.CloseGlobalMenu();
        });
        newDialog.SetDialogEvent(DialogButtonEventType.NO, () =>
        {
        });
        newDialog.Show();
    }

    /// <summary>
    /// 単品受取したときのメーッセージ
    /// </summary>
    private void MissionOneOpenResultMessage(RecvOpenAchievementValue result)
    {
        if (result == null) { return; }

        string result_msg = "";

        //--------------------------------------------------------------
        // エラーテキストの取得
        // 1: 受け取りアイテム（ユニット）が上限に到達　2: クエストキー期限切れ　３：開封有効期限切れ
        //--------------------------------------------------------------
        if (result.error != null)
        {
            for (int i = 0; i < result.error.Length; ++i)
            {
                switch ((PRESENT_OPEN_ERROR_TYPE)result.error[i])
                {
                    case PRESENT_OPEN_ERROR_TYPE.COUNT_LIMIT:
                        result_msg += Environment.NewLine + GameTextUtil.GetText("mt37q_content1");
                        break;
                    case PRESENT_OPEN_ERROR_TYPE.QUEST_KEY_EXPIRED:
                        result_msg += Environment.NewLine + GameTextUtil.GetText("mt37q_content2");
                        break;
                    case PRESENT_OPEN_ERROR_TYPE.RECEIVE_EXPIRED:
                        result_msg += Environment.NewLine + GameTextUtil.GetText("mt37q_content3");
                        break;
                }
            }
        }

        int rewardOpenCount = (result.achievement_opened != null) ? result.achievement_opened.Length : 0; // 開封個数
        if (rewardOpenCount > 0)
        {
            // 先頭に受け取りメッセージを追加
            result_msg = GameTextUtil.GetText("mt37q_content0") + Environment.NewLine + result_msg;
        }
        else
        {
            // 先頭の改行を消す
            if (!result_msg.IsNullOrEmpty())
            {
                result_msg = result_msg.Remove(0, Environment.NewLine.Length);
            }
        }

        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("mm36q_title"));
        newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("mt37q_content"), result_msg));
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.Show();
    }

}
