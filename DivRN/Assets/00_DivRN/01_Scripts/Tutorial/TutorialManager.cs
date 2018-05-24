using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using ServerDataDefine;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialPart
{
    NONE,
    NORMAL01, //通常パート1
    BATTLE, //戦闘パート
    BUILDUP, //強化パート
    EDIT, //編成パート
    NORMAL02, //通常パート2
    LAST
}

public enum TutorialPartState
{
    NONE,
    YET,
    SKIP,
    DONE
}

public enum TutorialUserType
{
    NONE,
    ALREADY, //既存ユーザー
    NEW //新規ユーザー
}
public enum TutorialStep
{
    NONE,
    HERO_SELECT = 5,
    NOMAL01_END = 100,
    BUILDUP_START = 301,
    BUILDUP_PART1 = 401,
    BUILDUP_PART2 = 402,
    BUILDUP_PART3 = 403,
    BUILDUP_PART4 = 404,
    BUILDUP_PART5 = 405,
    BUILDUP_PART6 = 406,
    BUILDUP_PART7 = 407,
    SCRATCH_START = 600,
    LAST
}

public static class TutorialExtension
{
    public static bool IsSkipOrDone(this TutorialPartState s)
    {
        switch (s)
        {
            case TutorialPartState.SKIP:
            case TutorialPartState.DONE:
                return true;

            default:
                return false;
        }
    }
}

public class TutorialPP : PP
{
    public static string USER_TYPE_KEY = "UserTypeKey";

    public static string PART_STATE_KEY_FORMAT = "TutorialPP_PART_{0}";

    public bool IsNewUser
    {
        get
        {
            return TutorialUserType == TutorialUserType.NEW;
        }
    }

    public TutorialUserType TutorialUserType
    {
        get
        {
            return (TutorialUserType)GetInt(USER_TYPE_KEY);
        }
        set
        {
            SetInt(USER_TYPE_KEY, (int)value);
        }
    }


    public TutorialPartState GetPartState(TutorialPart part)
    {
        return GetStr(string.Format(PART_STATE_KEY_FORMAT, part.ToString()), TutorialPartState.YET.ToString()).ToEnum<TutorialPartState>();
    }

    public void SetPartState(TutorialPart part, TutorialPartState state)
    {
        SetStr(string.Format(PART_STATE_KEY_FORMAT, part.ToString()), state.ToString());
    }
}

public class TutorialManager : SingletonComponent<TutorialManager>
{
    public static int FirstSelectNone = -1;

    public GameObject Panel = null;

    private static TutorialPP pp = new TutorialPP();
    private UnitDetailInfo m_Info = null;

    protected override void Start()
    {
        MainMenuManager.Instance.DisableBackKey();
        InputLock(false);
    }

    public void Skip()
    {
        PP.SetPartState(TutorialPart.BATTLE, TutorialPartState.SKIP);
        PP.SetPartState(TutorialPart.BUILDUP, TutorialPartState.SKIP);
        PP.SetPartState(TutorialPart.EDIT, TutorialPartState.SKIP);
        PP.Save();
        TutorialFSM.Instance.SendFsmNextEvent();
    }

    public static TutorialManager Create()
    {
        GameObject prefab = Resources.Load("Prefab/TutorialManager") as GameObject;
        GameObject go = Instantiate(prefab) as GameObject;

        TutorialManager result = go.GetComponent<TutorialManager>();
        return result;
    }

    public static TutorialPart GetNextTutorialPart()
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG 
        if (DebugOption.Instance.tutorialDO.forceTutorialPart != TutorialPart.NONE)
        {
            TutorialPart forceP = DebugOption.Instance.tutorialDO.forceTutorialPart;
            //            DebugOption.Instance.tutorialDO.forceTutorialPart = TutorialPart.NONE;

            Debug.Log("CALL GetNextTutorialPart force:" + forceP);
            return forceP;
        }
#endif

        foreach (TutorialPart part in Enum.GetValues(typeof(TutorialPart)))
        {
            if (part == TutorialPart.NONE)
            {
                continue;
            }
#if BUILD_TYPE_DEBUG
            Debug.Log("CALL GetNextTutorialPart :" + part);
#endif
            if (!PP.GetPartState(part).IsSkipOrDone())
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("CALL GetNextTutorialPart go:" + part);
#endif
                return part;
            }
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL GetNextTutorialPart NONE");
#endif
        return TutorialPart.NONE;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tutorialStep"></param>
    public static void CheckTutorialStep()
    {
        PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;
        int tutorialStep = player.renew_tutorial_step;

        //旧チュートリアルでパーティ選択しているか
        if (player.first_select_num == FirstSelectNone)
        {
            //選択していない　[新規ユーザー]
            PP.TutorialUserType = TutorialUserType.NEW;
        }
        else
        {
            //選択している　[既存ユーザー]
            PP.TutorialUserType = TutorialUserType.ALREADY;
        }
        PP.Save();

        //一旦初期化
        PP.SetPartState(TutorialPart.NORMAL01, TutorialPartState.NONE);
        PP.SetPartState(TutorialPart.BATTLE, TutorialPartState.NONE);
        PP.SetPartState(TutorialPart.BUILDUP, TutorialPartState.NONE);
        PP.SetPartState(TutorialPart.EDIT, TutorialPartState.NONE);
        PP.SetPartState(TutorialPart.NORMAL02, TutorialPartState.NONE);
        PP.Save();

        if (tutorialStep < 200)
        {
            //通常１から
        }
        else if (tutorialStep < 300)
        {
            //戦闘から
            PP.SetPartState(TutorialPart.NORMAL01, TutorialPartState.DONE);
            PP.Save();
        }
        else if (tutorialStep < 500)
        {
            //強化から
            PP.SetPartState(TutorialPart.NORMAL01, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.BATTLE, TutorialPartState.DONE);
            PP.Save();
        }
        else if (tutorialStep < 600)
        {
            //編成から
            PP.SetPartState(TutorialPart.NORMAL01, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.BATTLE, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.BUILDUP, TutorialPartState.DONE);
            PP.Save();
        }
        else if (tutorialStep < 605)
        {
            //通常２から
            PP.SetPartState(TutorialPart.NORMAL01, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.BATTLE, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.BUILDUP, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.EDIT, TutorialPartState.DONE);
            PP.Save();
        }
        else
        {
            //チュートリアル終了している
            PP.SetPartState(TutorialPart.NORMAL01, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.BATTLE, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.BUILDUP, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.EDIT, TutorialPartState.DONE);
            PP.SetPartState(TutorialPart.NORMAL02, TutorialPartState.DONE);
            PP.Save();
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public static TutorialPP PP
    {
        get
        {
            return pp;
        }
    }

    public static void SendStep(int tutorialStep, Action successAction, Action failAction = null)
    {

        PacketStructPlayer StructPlayer = UserDataAdmin.Instance.m_StructPlayer;

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL TutorialManager#SendStep: " + tutorialStep + " / renew_tutorial_step: " + StructPlayer.renew_tutorial_step);
#endif

        if (tutorialStep <= StructPlayer.renew_tutorial_step)
        {
            Debug.Log("TutorialManager#SendStep CANCEL: " + tutorialStep + " <= renew_tutorial_step: " + StructPlayer.renew_tutorial_step);
            successAction();
            return;
        }

        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.GUARD);

        ServerApi api = ServerDataUtilSend.SendPacketAPI_RenewTutorial((uint)tutorialStep);
        api.setSuccessAction(
                (_data) =>
                {

#if BUILD_TYPE_DEBUG
                    Debug.Log("TutorialManager#SendStep Success SendStep:" + tutorialStep + " / renew_tutorial_step: " + StructPlayer.renew_tutorial_step);
#endif
                    LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.GUARD);

                    if (successAction != null)
                    {
                        successAction();
                    }
                }).
            setErrorAction(
                (_data) =>
                {

                    Debug.LogError("TutorialManager#SendStep ERROR:" + _data.m_PacketCode + " / renew_tutorial_step: " + StructPlayer.renew_tutorial_step);

                    LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.GUARD);

                    if (successAction != null)
                    {
                        successAction();
                    }
                }).
            SendStart();

        //送信後にtutorialStepを更新
        StructPlayer.renew_tutorial_step = tutorialStep;
    }

    private bool ShouldWait(MAINMENU_SEQ seq)
    {
        if (!MainMenuManagerFSM.Instance.IsPageWaitLoop &&
            !MainMenuManagerFSM.Instance.IsTutorialNext)
        {
#if BUILD_TYPE_DEBUG
            //Debug.Log("Wait:" + MainMenuManagerFSM.Instance.ActiveStateName);
#endif
            return true;
        }
#if BUILD_TYPE_DEBUG
        //Debug.Log("m_WorkSwitchPageNow:" + MainMenuManager.Instance.WorkSwitchPageNow + " SEQ:" + seq);
#endif
        if (seq != MainMenuManager.Instance.WorkSwitchPageNow)
        {
#if BUILD_TYPE_DEBUG
            //Debug.Log("HIT");
#endif
            return true;
        }

        return false;
    }

    private IEnumerator WaitForPageWaitLoop(MAINMENU_SEQ seq, bool request = true, bool sendNext = true, bool blanch = false)
    {
#if BUILD_TYPE_DEBUG
        //        Debug.Log("HIT_a :" + request);
#endif
        if (request)
        {
#if BUILD_TYPE_DEBUG
            //            Debug.Log("HIT_b:" + seq);
#endif
            MainMenuManager.Instance.AddSwitchRequest(seq, false, false);
        }

        yield return null;

        while (ShouldWait(seq))
        {
            yield return null;
        }

        if (sendNext)
        {
            if (blanch == false)
            {
                TutorialFSM.Instance.SendFsmEvent("GONE");
            }
            else
            {
                if (UserDataAdmin.Instance.m_StructPlayer.renew_tutorial_step >= (int)TutorialStep.BUILDUP_PART6)
                {
                    InputLock(false);
                    FinishBuildUp();
                }
                else
                {
                    TutorialFSM.Instance.SendFsmEvent("GONE");
                }
            }
        }
    }

    void OnReady()
    {
        TutorialFSM.Instance.SendFsmNextEvent();
    }

    void OnSwitchPart()
    {
        TutorialPart part = GetNextTutorialPart();

#if UNITY_EDITOR && BUILD_TYPE_DEBUG 
        Debug.Log("CALL TutorialManager#OnSwitchPart:" + part);

        DebugOption.Instance.tutorialDO.forceTutorialPart = TutorialPart.NONE;
#endif
        //メニューBGM再生
        switch (part)
        {
            case TutorialPart.BUILDUP:
            case TutorialPart.EDIT:
            case TutorialPart.NORMAL02:
                SoundUtil.PlayBGM(BGMManager.EBGM_ID.eBGM_2_1, false);
                break;
        }

        TutorialFSM.Instance.SendFsmEvent(part.ToString());
    }

    IEnumerator OnGoMainMenu()
    {
        //ログインパック取得遷移へ
        MainMenuParam.m_DateChangeType = DATE_CHANGE_TYPE.LOGIN;
        yield return WaitForPageWaitLoop(MAINMENU_SEQ.SEQ_DATE_CHANGE);
        Destroy();
    }

    ///////////////////
    /// 通常パート01 ///
    //////////////////
    IEnumerator OnEnterNormal01()
    {
        UnitIconImageProvider.Instance.hiddenCanvas();

        {
            yield return null;
        }
        if (UserDataAdmin.Instance.m_StructPlayer.renew_first_select_num >= 0)
        {
            LoadingManager.Instance.setOverLayMask(false);

            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.TO_BATTLE);
            TutorialFSM.Instance.SendEvent_FinishStory();
        }
        else
        {
            TutorialFSM.Instance.SendFsmEvent("GONE");
        }
    }

    private StoryView storyView;

    private void DestroyStoryView()
    {
        Debug.Log("CALL TutorialManager#DestroyStoryView");
        if (storyView == null)
        {
            Debug.Log("StoryView is null");
            return;
        }

        DestroyObject(storyView.gameObject);
        storyView = null;
    }

    private StoryView PlayStory(uint storyId, Action action)
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (TutorialDebugOption.Instance.skipMovie)
        {
            action();
            return null;
        }
#endif

        StoryView sv = StoryView.Create().DisableAutoDestroy();
        sv.SetScenario(storyId);
        sv.Show(
            () =>
            {
                Debug.Log("StoryView Completed");
                action();
            });

        return sv;
    }

    void OnPlayOpeningMovie()
    {
        SendStep(
            1,
            () =>
            {
                storyView = PlayStory(
                    GlobalDefine.TUTORIAL_STORY01,
                    () =>
                    {
                        SendStep(
                            2,
                            () =>
                            {
                                TutorialFSM.Instance.SendEvent_FinishStory();
                            });
                    });
                storyView.EnableLoadingMask();
            });
    }

    IEnumerator OnGoTutorialHeroSelect()
    {
        DestroyStoryView();
        yield return null;

        yield return Resources.UnloadUnusedAssets();
        yield return null;

        //メニューBGM再生開始
        SoundUtil.PlayBGM(BGMManager.EBGM_ID.eBGM_2_1, false);

        yield return WaitForPageWaitLoop(MAINMENU_SEQ.SEQ_TUTORIAL_HERO_SELECT);
    }

    void OnExitNormal01()
    {
        PP.SetPartState(TutorialPart.NORMAL01, TutorialPartState.DONE);
        PP.Save();
        TutorialFSM.Instance.SendFsmNextEvent();
    }

    ///////////////////
    /// 通常パート02 ///
    //////////////////
    void OnEnterNormal02()
    {
        if (UserDataAdmin.Instance.m_StructPlayer.renew_tutorial_step >= 602)
        {
            TutorialFSM.Instance.SendFsmNegativeEvent();
            return;
        }

        //スクラッチの前にはStep番号の送信保証を行う
        TutorialManager.SendStep(
            (int)TutorialStep.SCRATCH_START,
            () =>
            {
                TutorialFSM.Instance.SendFsmPositiveEvent();
            });
    }

    IEnumerator OnGoScratch()
    {
        MainMenuParam.m_GachaMaster = MasterFinder<MasterDataGacha>.Instance.Find((int)GlobalDefine.TUTORIAL_SCRATCH);
        yield return WaitForPageWaitLoop(MAINMENU_SEQ.SEQ_GACHA_MAIN);
    }

    void OnShowDialog_ScratchInitial()
    {
        new SerialProcess().Add(
            (System.Action next) =>
            {
                TutorialDialog.Create().SetTutorialType(TutorialDialog.FLAG_TYPE.GACHA).Show(() =>
                {
                    next();
                });
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_SCRATCH_RESULT01_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_SCRATCH_RESULT01_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_SCRATCH_RESULT02_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_SCRATCH_RESULT02_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            })
            .Add(next =>
            {
                ServerDataUtilSend.SendPacketAPI_UserAuthentication().
                setSuccessAction(_data =>
                {
                    next();
                }).
                setErrorAction(_data =>
                {
                    //authError(_data.m_PacketCode, _data.m_PacketUniqueNum);
                    next();
                }).
                SendStart();
            })
            .Add(
            (System.Action next) =>
            {
                InputLock(true);
                TutorialManager.SendStep(
                    601,
                    () =>
                    {
                        InputLock(false);
                        TutorialFSM.Instance.SendFsmNextEvent();

                        next();
                    });
            }).Flush();
    }

    void OnShowArrow_Scratch()
    {
        TutorialArrow.Create("ScratchOne").
            SetLocalEulerAngles(new Vector3(0, 0, 180)).
            Show(
                () =>
                {
                    TutorialFSM.Instance.SendFsmNextEvent();
                });
    }

    IEnumerator OnWaitFor_ScratchResult()
    {
        yield return WaitForPageWaitLoop(MAINMENU_SEQ.SEQ_GACHA_RESULT, false, false);

        m_Info = GameObject.FindObjectOfType<UnitDetailInfo>();
        while (m_Info == null)
        {
            yield return null;
            m_Info = GameObject.FindObjectOfType<UnitDetailInfo>();
        }

        InputLock(true);
        m_Info.DisableBackKey();

        while (m_Info.CharaImage == null)
        {
            yield return null;
        }

        while (!m_Info.CharaImage.Ready)
        {
            yield return null;
        }

        TutorialFSM.Instance.SendFsmNextEvent();
    }

    void OnShowDialog_ScratchResult()
    {
        InputLock(false);
        new SerialProcess().Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_SCRATCH_RESULT03_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_SCRATCH_RESULT03_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_SCRATCH_RESULT04_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_SCRATCH_RESULT04_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_SCRATCH_RESULT05_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_SCRATCH_RESULT05_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_SCRATCH_RESULT06_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_SCRATCH_RESULT06_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                InputLock(true);
                TutorialManager.SendStep(
                    603,
                    () =>
                    {
                        InputLock(false);
                        TutorialFSM.Instance.SendFsmNextEvent();

                        next();
                    });
            }).Flush();
    }

    public void OnShowCarousel_Scratch()
    {
        TutorialDialog.Create().
            SetTutorialType(TutorialPart.NORMAL02.ConvertToTutorialFlagType()).
            Show(
                () =>
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("TutorialCarousel Completed");
#endif
                    m_Info.EnableBackKey();
                    TutorialFSM.Instance.SendFsmNextEvent();
                });
    }

    void OnShouldGoInputNickname()
    {
        InputLock(true);
        if (GameObject.Find("ReturnButton") != null)
        {
            //ユニット詳細画面を閉じる
            GameObject.Find("ReturnButton").GetComponent<Button>().onClick.Invoke();
        }

        //新規ユーザーの場合は名前入力画面へ遷移する
        if (PP.IsNewUser)
        {
            TutorialManager.SendStep(
            604,
            () =>
            {
                InputLock(false);
                TutorialFSM.Instance.SendFsmPositiveEvent();
            });
            return;
        }

        InputLock(false);
        TutorialFSM.Instance.SendFsmNegativeEvent();
    }

    IEnumerator OnGoInputNickname()
    {
        yield return WaitForPageWaitLoop(MAINMENU_SEQ.SEQ_TUTORIAL_NAME);
    }


    void OnExitNormal02()
    {
        TutorialManager.SendStep(
            605,
            () =>
            {
                PP.SetPartState(TutorialPart.NORMAL02, TutorialPartState.DONE);
                PP.Save();
                TutorialFSM.Instance.SendFsmNextEvent();
            });
    }


    ///////////////////
    /// バトルパート ///
    //////////////////
    IEnumerator OnEnterBattle()
    {
        {
            yield return null;
        }
        if (UserDataAdmin.Instance.m_StructPlayer.renew_tutorial_step >= 217)
        {
            TutorialFSM.Instance.SendFsmNegativeEvent();
        }
        else
        {
            TutorialFSM.Instance.SendFsmPositiveEvent();
        }
    }

    void OnPlayStoryOpeningMovie()
    {
        storyView = PlayStory(
            GlobalDefine.TUTORIAL_STORY02,
            () =>
            {
                SendStep(
                    101,
                    () =>
                    {
                        TutorialFSM.Instance.SendEvent_FinishStory();
                    });
            });
        storyView.EnableLoadingMask();
        storyView.DisableReturnableBGM();
    }


    void OnGoBattle()
    {
        DestroyStoryView();
        uint unAreaID = GlobalDefine.TUTORIAL_AREA_1;
        uint unQuestID = GlobalDefine.TUTORIAL_QUEST_1;
        uint unQuestState = 0;
        uint unHelperUserID = 0;
        PacketStructUnit sHelperUnit = null;
        bool bHelperPointActive = false;
        int nBeginnerBoostID = 0;
        uint[] aunareaAmendID = null;

        Debug.LogError("USER:" + UserDataAdmin.Instance.getCurrentHero());

        MasterDataDefaultParty masterParty = UserDataAdmin.Instance.getCurrentHeroDefaultParty();

        SceneGoesParam.Instance.m_SceneGoesParamToQuest2 = new SceneGoesParamToQuest2();
        SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestAreaID = unAreaID;
        SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestMissionID = unQuestID;

        int index = 0;

        PacketStructUnit friendUnit = null;

        foreach (uint id in masterParty.PartyCharaDict.Keys.ToList().GetRange(0, 4))
        {
            if (id == 0)
                continue;
            UserDataUnitParam p = new UserDataUnitParam();

            PacketStructUnit unit = UserDataAdmin.Instance.m_StructPlayer.unit_list.FirstOrDefault(u => u.id == id);
            //フレンドユニットはリーダーと同一ユニットを指定する。
            if (friendUnit == null)
                friendUnit = unit;

            p.m_UnitDataID = unit.id;
            p.m_UnitParamLevel = (int)unit.level;
            p.m_UnitParamEXP = (int)unit.exp;
            p.m_UnitParamUniqueID = unit.unique_id;
            p.m_UnitParamLimitBreakLV = (int)unit.limitbreak_lv;
            p.m_UnitParamLimitOverLV = (int)unit.limitover_lv;
            p.m_UnitParamPlusPow = (int)unit.add_pow;
            p.m_UnitParamPlusHP = (int)unit.add_hp;

            SceneGoesParam.Instance.m_SceneGoesParamToQuest2.SetParam(index, p);
            ++index;
        }

        {
            //フレンド枠のユニットを設定
            UserDataUnitParam p = new UserDataUnitParam();
            p.m_UnitDataID = friendUnit.id;
            p.m_UnitParamLevel = (int)friendUnit.level;
            p.m_UnitParamEXP = (int)friendUnit.exp;
            p.m_UnitParamUniqueID = friendUnit.unique_id;
            p.m_UnitParamLimitBreakLV = (int)friendUnit.limitbreak_lv;
            p.m_UnitParamLimitOverLV = (int)friendUnit.limitover_lv;
            p.m_UnitParamPlusPow = (int)friendUnit.add_pow;
            p.m_UnitParamPlusHP = (int)friendUnit.add_hp;
            SceneGoesParam.Instance.m_SceneGoesParamToQuest2.SetParam(4, p);

            //フレンド生成
            PacketStructFriend friend = new PacketStructFriend();
            friend.unit = friendUnit;
            friend.user_id = 0;
            friend.user_name = "guest";
            friend.user_rank = 999;
            friend.last_play = friendUnit.get_time;
            friend.friend_point = 0;
            friend.friend_state = (uint)FRIEND_STATE.FRIEND_STATE_SUCCESS;
            SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyFriend = friend;
        }

        ServerDataUtilSend.SendPacketAPI_Quest2StartTutorial(
                unQuestID,
                unQuestState,
                unHelperUserID,
                sHelperUnit,
                bHelperPointActive,
                UserDataAdmin.Instance.m_StructPlayer.unit_party_current,
                MainMenuParam.m_QuestEventFP,
                (MainMenuParam.m_BeginnerBoost != null) ? (int)MainMenuParam.m_BeginnerBoost.fix_id : 0,
                null //aunAreaAmendID
            ).
            setSuccessAction(
                _data =>
                {
                    //----------------------------------------
                    // サーバーで求めたクエスト開始情報を反映
                    //----------------------------------------
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build = new SceneGoesParamToQuest2Build();
                    SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild = _data.GetResult<RecvQuest2Start>().result.quest;

                    //----------------------------------------
                    // BGM停止処理：暫定対応
                    //----------------------------------------
                    SoundUtil.StopBGM(false);

                    //----------------------------------------
                    // サーバーに受理されたのでゲームへ移行
                    //----------------------------------------
                    SoundUtil.PlaySE(SEID.SE_MENU_OK2);
                    SceneCommon.Instance.ChangeScene(SceneType.SceneQuest2, false);

                    TutorialFSM.Instance.SendFsmNextEvent();
                }).
            setErrorAction(
                _data =>
                {
                    Debug.LogError("ERROR:" + _data);
                }).
            SendStart();
    }

    void OnWaitForFinishQuestResult()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnWaitForFinishQuestResult");
#endif
        // クエストリザルト情報を受け渡しクラスへ入力
        //----------------------------------------
        uint unQuestID = GlobalDefine.TUTORIAL_QUEST_1;
        SceneGoesParam.Instance.m_SceneGoesParamToMainMenu = new SceneGoesParamToMainMenu();
        SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_QuestID = unQuestID;
        SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_PartyFriend = SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_PartyFriend;

        SceneGoesParam.Instance.m_SceneGoesParamToMainMenu.m_Quest2 = true;

        SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
    }

    void OnPlayStoryEndingMovie()
    {
        storyView = PlayStory(
            GlobalDefine.TUTORIAL_STORY03,
            () =>
            {
                TutorialFSM.Instance.SendEvent_FinishStory();
            });
        storyView.EnableLoadingMask();
    }


    IEnumerator OnExitBattle()
    {
        DestroyStoryView();
        yield return null;

        yield return Resources.UnloadUnusedAssets();
        yield return null;

        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false);

        PP.SetPartState(TutorialPart.BATTLE, TutorialPartState.DONE);
        PP.Save();

        TutorialFSM.Instance.SendFsmNextEvent();
    }

    ///////////////////
    //// 強化パート ////
    //////////////////
    ///
    void OnEnterBuildUp()
    {
        BaseUnitPush = false;
        MaterialUnitPush = false;

        InputLock(true);
        SendStep(
            (int)TutorialStep.BUILDUP_START,
            () =>
            {
                TutorialFSM.Instance.SendFsmNextEvent();
            });
    }

    IEnumerator OnGoUnitBuildUp()
    {
        Debug.LogError("HIT");

        while (MainMenuManager.Instance.WorkSwitchPageNow == MAINMENU_SEQ.SEQ_MAX)
        {
            Debug.LogError("HIT:" + MainMenuManager.Instance.WorkSwitchPageNow);
            yield return WaitForPageWaitLoop(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, true, true, true);
        }
        while (!MainMenuManagerFSM.Instance.IsPageWaitLoop)
        {
            Debug.LogError("HIT_:" + MainMenuManager.Instance.WorkSwitchPageNow);
            yield return null;
        }
        TutorialFSM.Instance.SendFsmEvent("GONE");
    }

    public static long BaseUnitUniqueId { get; set; }

    public static uint BaseUnitCharaId
    {
        get
        {
            MasterDataDefaultParty masterParty = UserDataAdmin.Instance.getCurrentHeroDefaultParty();
            return masterParty.party_chara0_id;
        }
    }

    public static string BaseUnitListItemGameObjectName
    {
        get
        {
            return string.Format(UnitGridListItem.UnitListItemGameObjectNameFormat,
                                 BaseUnitCharaId,
                                 BaseUnitUniqueId);
        }
    }


    public static long MaterialUnitUniqueId { get; set; }

    public static uint MaterialUnitCharaId
    {
        get
        {
            MasterDataDefaultParty masterParty = UserDataAdmin.Instance.getCurrentHeroDefaultParty();
            return masterParty.material_chara0_id;
        }
    }

    public static string MaterialUnitListItemGameObjectName
    {
        get
        {
            return string.Format(UnitGridListItem.UnitListItemGameObjectNameFormat,
                                 MaterialUnitCharaId,
                                 MaterialUnitUniqueId);
        }
    }

    void OnCheckBuildUpUnit()
    {
        BaseUnitUniqueId = 0;
        MaterialUnitUniqueId = 0;
        try
        {

            uint base_unit_charaid = BaseUnitCharaId;
            uint material_unit_charaid = MaterialUnitCharaId;
            UnitGridListItem[] gridlistitems = FindObjectsOfType(typeof(UnitGridListItem)) as UnitGridListItem[];
            for (int i = 0; i < gridlistitems.Length; i++)
            {
                UnitGridListItem item = gridlistitems[i];
                string[] strItems = item.name.Split(':');
                if (strItems.Length == 3)
                {
                    uint chara_id = Convert.ToUInt32(strItems[1]);
                    long unique_id = Convert.ToInt64(strItems[2]);
                    //ベースと素材が存在するかチェック
                    if (chara_id == base_unit_charaid)
                    {
                        if (BaseUnitUniqueId < unique_id)
                        {
                            BaseUnitUniqueId = unique_id;
                        }
                    }
                    else if (chara_id == material_unit_charaid)
                    {
                        if (MaterialUnitUniqueId < unique_id)
                        {
                            MaterialUnitUniqueId = unique_id;
                        }
                    }
                }
            }

            GameObject baseObj = GameObject.Find(BaseUnitListItemGameObjectName);

            if (baseObj != null)
            {
                UnitGridComplex unitGrid = baseObj.GetComponentInParent<UnitGridComplex>();
                unitGrid.UpdateList();
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            if (BaseUnitUniqueId == 0 ||
                MaterialUnitUniqueId == 0)
            {
                TutorialFSM.Instance.SendFsmEvent("FINISH_BUILD_UP");
            }
            else
            {
                TutorialFSM.Instance.SendFsmNextEvent();
            }
        }
    }

    void OnShowDialog_UnitBuildUpInitial()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL TutorialManager#OnShowDialog_UnitBuildUpInitial");
#endif
        InputLock(false);
        Dialog.Create(DialogType.DialogOK).
            SetTitleFromTextKey(("TUTORIAL_BUILDUP_INITIAL01_TITLE").ToLower()).
            SetMainFromTextKey(("TUTORIAL_BUILDUP_INITIAL01_MAIN").ToLower()).
            SetOkEvent(
                () =>
                {
                    Dialog.Create(DialogType.DialogOK).
                        SetTitleFromTextKey(("TUTORIAL_BUILDUP_INITIAL02_TITLE").ToLower()).
                        SetMainFromTextKey(("TUTORIAL_BUILDUP_INITIAL02_MAIN").ToLower()).
                        SetOkEvent(
                            () =>
                            {
                                InputLock(true);
                                TutorialFSM.Instance.SendFsmNextEvent();
                            }).
                        Show();
                }).
            Show();
    }

    static bool BaseUnitPush = false;
    void OnShowArrow_BaseUnit()
    {
        SendStep(
            (int)TutorialStep.BUILDUP_PART1,
            () =>
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("CALL OnShowArrow_BaseUnit:" + BaseUnitListItemGameObjectName);
#endif
                InputLock(false);
                TutorialArrow.Create(BaseUnitListItemGameObjectName + "/TouchSize").
                    SwitchRootCanvas().
                    Show(
                        () =>
                        {
                            if (BaseUnitPush == false)
                            {
                                BaseUnitPush = true;
                                InputLock(true);
                                TutorialFSM.Instance.SendFsmNextEvent();
                            }
                        });
            });
    }

    void OnShowDialog_BaseUnitSelected()
    {
        InputLock(false);
        new SerialProcess().Add(
            (System.Action next) =>
            {
                InputLock(true);
                //すぐにダイアログを開くと選択判定がキャンセルされるのでボタンブロックが解除されるまで待つ
                StartCoroutine(WaitButtonBlock(() =>
                {
                    next();
                    InputLock(false);
                }));
            }).Add(
            (System.Action next) =>
            {
                SendStep(
                    (int)TutorialStep.BUILDUP_PART2,
                    () =>
                    {
                        next();
                    });
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_BUILDUP_BASE_SELECTED01_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_BUILDUP_BASE_SELECTED01_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_BUILDUP_BASE_SELECTED02_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_BUILDUP_BASE_SELECTED02_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_BUILDUP_BASE_SELECTED03_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_BUILDUP_BASE_SELECTED03_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_BUILDUP_BASE_SELECTED04_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_BUILDUP_BASE_SELECTED04_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                InputLock(true);
                TutorialFSM.Instance.SendFsmNextEvent();
            }).Flush();

    }

    static bool MaterialUnitPush = false;
    void OnShowArrow_MaterialUnit()
    {
        SendStep(
            (int)TutorialStep.BUILDUP_PART3,
            () =>
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("CALL OnShowArrow_MaterialUnit:" + MaterialUnitListItemGameObjectName);
#endif
                InputLock(false);
                TutorialArrow.Create(MaterialUnitListItemGameObjectName + "/TouchSize").
                    SwitchRootCanvas().
                    Show(
                        () =>
                        {
                            if (MaterialUnitPush == false)
                            {
                                MaterialUnitPush = true;
                                InputLock(true);
                                TutorialFSM.Instance.SendFsmNextEvent();
                            }
                        });
            });
    }


    void OnShowDialog_MaterialUnitSelected()
    {
        InputLock(false);
        new SerialProcess().Add(
            (System.Action next) =>
            {
                InputLock(true);
                //すぐにダイアログを開くと選択判定がキャンセルされるのでボタンブロックが解除されるまで待つ
                StartCoroutine(WaitButtonBlock(() =>
                {
                    next();
                    InputLock(false);
                }));
            }).Add(
            (System.Action next) =>
            {
                SendStep(
                    (int)TutorialStep.BUILDUP_PART4,
                    () =>
                    {
                        next();
                    });
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_BUILDUP_MATERIAL_SELECTED01_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_BUILDUP_MATERIAL_SELECTED01_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).
                    Show();
            }).Add(
            (System.Action next) =>
            {
                InputLock(true);
                SendStep(
                    (int)TutorialStep.BUILDUP_PART5,
                    () =>
                    {
                        next();
                    });
            }).Add(
            (System.Action next) =>
            {
                InputLock(false);
                TutorialArrow.Create("UnitPanelExecuteButton(Clone)/Button").SetLocalEulerAngles(new Vector3(0, 0, 180)). //SetLocalPosition(new Vector3(0,50f,0)).
                    Show(
                        () =>
                        {
                            TutorialFSM.Instance.SendFsmNextEvent();
                        });
            }).Flush();
    }

    public void FinishBuildUp()
    {
        TutorialFSM.Instance.SendFsmEvent("FINISH_BUILD_UP");
    }

    void OnShowDialog_BuildUpResult()
    {
        SendStep(
            (int)TutorialStep.BUILDUP_PART7,
            () =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_BUILDUP_RESULT01_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_BUILDUP_RESULT01_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            Dialog.Create(DialogType.DialogOK).
                                SetTitleFromTextKey(("TUTORIAL_BUILDUP_RESULT02_TITLE").ToLower()).
                                SetMainFromTextKey(("TUTORIAL_BUILDUP_RESULT02_MAIN").ToLower()).
                                SetOkEvent(
                                    () =>
                                    {
                                        TutorialFSM.Instance.SendFsmNextEvent();
                                    }).
                                Show();
                        }).
                    Show();
            });
    }

    public void OnShowCarousel_UnitBuildUp()
    {
        SendStep(
            408,
            () =>
            {
                TutorialDialog.Create().
                    SetTutorialType(TutorialPart.BUILDUP.ConvertToTutorialFlagType()).
                    Show(
                        () =>
                        {
#if BUILD_TYPE_DEBUG
                            Debug.Log("TutorialCarousel Completed");
#endif
                            TutorialFSM.Instance.SendFsmNextEvent();
                        });
            });
    }


    void OnShowDialog_EditUnitBuildUp()
    {
        SendStep(
            409,
            () =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_BUILDUP_LETEDIT01_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_BUILDUP_LETEDIT01_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            TutorialFSM.Instance.SendFsmNextEvent();
                        }).
                    Show();
            });
    }

    void OnExitBuildUp()
    {
        SendStep(
            410,
            () =>
            {
                PP.SetPartState(TutorialPart.BUILDUP, TutorialPartState.DONE);
                PP.Save();
                TutorialFSM.Instance.SendFsmNextEvent();
            });
    }

    ///////////////////
    //// 編成パート ////
    //////////////////

    void OnEnterEdit()
    {
        SendStep(
            411,
            () =>
            {
                TutorialFSM.Instance.SendFsmNextEvent();
            });
    }

    IEnumerator OnGoUnitEdit()
    {
        yield return WaitForPageWaitLoop(MAINMENU_SEQ.SEQ_UNIT_PARTY_FORM);
    }

    void OnShowDialog_UnitEditEnter()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL TutorialManager#ShowDialog_UnitEditEnter");
#endif

        new SerialProcess().Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_EDIT_ENTER01_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_EDIT_ENTER01_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_EDIT_ENTER02_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_EDIT_ENTER02_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                Dialog.Create(DialogType.DialogOK).
                    SetTitleFromTextKey(("TUTORIAL_EDIT_ENTER03_TITLE").ToLower()).
                    SetMainFromTextKey(("TUTORIAL_EDIT_ENTER03_MAIN").ToLower()).
                    SetOkEvent(
                        () =>
                        {
                            next();
                        }).Show();
            }).Add(
            (System.Action next) =>
            {
                TutorialFSM.Instance.SendFsmNextEvent();
            }).Flush();
    }

    public void OnShowCarousel_UnitEdit()
    {
        SendStep(
            501,
            () =>
            {
                TutorialDialog.Create().
                    SetTutorialType(TutorialPart.EDIT.ConvertToTutorialFlagType()).
                    Show(
                        () =>
                        {
#if BUILD_TYPE_DEBUG
                            Debug.Log("TutorialCarousel Completed");
#endif
                            SendStep(
                                502,
                                () =>
                                {
                                    TutorialFSM.Instance.SendFsmNextEvent();
                                });
                        });
            });
    }

    void OnShowDialog_UnitEditExit()
    {
        Dialog.Create(DialogType.DialogOK).
            SetTitleFromTextKey(("TUTORIAL_EDIT_EXIT01_TITLE").ToLower()).
            SetMainFromTextKey(("TUTORIAL_EDIT_EXIT01_MAIN").ToLower()).
            SetOkEvent(
                () =>
                {
                    SendStep(
                        503,
                        () =>
                        {
                            TutorialFSM.Instance.SendFsmNextEvent();
                        });
                }).
            Show();
    }

    void OnExitEdit()
    {
        /*
           ここでは次のシーケンスですぐにチュートリアルステップAPIが
           実行されるとサーバー側でエラーになってしまうので
           通信が終了してから次に進む
        */
        SendStep(
            504,
            () =>
            {
                TutorialFSM.Instance.SendFsmNextEvent();
            },
            () =>
            {
                TutorialFSM.Instance.SendFsmNextEvent();
            }
        );
        PP.SetPartState(TutorialPart.EDIT, TutorialPartState.DONE);
        PP.Save();
    }

    public bool TutorialChkAllFinish()
    {
        bool ret = false;
        TutorialPart part = GetNextTutorialPart();
        if (part == TutorialPart.LAST)
        {
            ret = true;
        }
        return ret;
    }


    bool lastInputLock = false;

    public bool LastInputLock
    {
        get { return lastInputLock; }
        set { }
    }

    /// <summary>
    /// 入力禁止設定
    /// </summary>
    /// <param name="bFlag"></param>
    public void InputLock(bool bFlag)
    {
        lastInputLock = bFlag;
        UnityUtil.SetObjectEnabledOnce(Panel, bFlag);
    }

    /// <summary>
    /// ボタンブロックが解除されるまで待つ
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public IEnumerator WaitButtonBlock(System.Action action)
    {
        while (ButtonBlocker.Instance.IsActive())
        {
            yield return null;
        }

        while (ServerApi.IsExists)
        {
            yield return null;
        }

        action();
    }

}
