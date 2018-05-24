using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMenuManager : SingletonComponent<GlobalMenuManager>
{
    public GlobalMenu m_GlobalMenu = null;

    public void ShowWait()
    {
    }

    public void CloseWait()
    {
    }

    public IEnumerator OnPageOutBefor()
    {
        while (m_GlobalMenu.getCurrentPageSeq().PageSwitchEventDisableBefore())
        {
            yield return null;
        }

        m_GlobalMenu.getCurrentPageSeq().PageSwitchTriger(false, false, m_GlobalMenu.Back);

        GlobalMenuManagerFSM.Instance.SendFsmEvent("DO_NEXT");
    }

    public IEnumerator OnPageOut()
    {
        while (!m_GlobalMenu.getNextPageSeq().CheckFade())
        {
            yield return null;
        }

        //
        UnityUtil.SetObjectEnabledOnce(m_GlobalMenu.getCurrentPageSeq().gameObject, false);
        m_GlobalMenu.getCurrentPageSeq().RunOnFadeOutFinishedCallback();

        GlobalMenuManagerFSM.Instance.SendFsmEvent("DO_NEXT");
    }

    public IEnumerator OnPageOutAfter()
    {
        while (m_GlobalMenu.getCurrentPageSeq().PageSwitchEventDisableAfter(m_GlobalMenu.NextSeq))
        {
            yield return null;
        }

        GlobalMenuManagerFSM.Instance.SendFsmEvent("DO_NEXT");
    }

    public IEnumerator OnPageInBefor()
    {
        while (m_GlobalMenu.getNextPageSeq().PageSwitchEventEnableBefore())
        {
            yield return null;
        }

        // 機能チュートリアルの表示
        TutorialDialog.FLAG_TYPE flagType = m_GlobalMenu.NextSeq.ConvertToTutorialFlagType();
        if (flagType != TutorialDialog.FLAG_TYPE.NONE && flagType != TutorialDialog.FLAG_TYPE.ALL)
        {
            bool isTutorial = (LocalSaveManagerRN.Instance.GetIsShowTutorialDialog(flagType) == false);
            if (isTutorial)
            {
#if BUILD_TYPE_DEBUG
                Debug.LogError(string.Format("チュートリアルを表示する FLAG_TYPE:{0}", flagType.ToString()));
#endif
                TutorialDialog.Create().SetTutorialType(flagType).Show(() =>
                {
                    isTutorial = false;
                });

            }
            while (isTutorial)
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("IN_TUTORIAL");
#endif
                yield return null;
            }
        }

        while (!m_GlobalMenu.getNextPageSeq().IsStart)
        {
            yield return null;
        }

        m_GlobalMenu.getNextPageSeq().PageSwitchTriger(true, false, m_GlobalMenu.Back);

        GlobalMenuManagerFSM.Instance.SendFsmEvent("DO_NEXT");
    }

    public IEnumerator OnPageIn()
    {
        m_GlobalMenu.setPanelSize(m_GlobalMenu.NextSeq);
        //
        UnityUtil.SetObjectEnabledOnce(m_GlobalMenu.getNextPageSeq().gameObject, true);

        m_GlobalMenu.SetActiveReturn(m_GlobalMenu.getNextMaster().Return);

        while (!m_GlobalMenu.getNextPageSeq().CheckFade())
        {
            yield return null;
        }

        GlobalMenuManagerFSM.Instance.SendFsmEvent("DO_NEXT");
    }

    public IEnumerator OnPageInAfter()
    {
        while (m_GlobalMenu.getNextPageSeq().PageSwitchEventEnableAfter())
        {
            yield return null;
        }

        m_GlobalMenu.PageSwitchEnd();

        GlobalMenuManagerFSM.Instance.SendFsmEvent("DO_NEXT");
    }

    public bool IsPageWait()
    {
        return GlobalMenuManagerFSM.Instance.ActiveStateName.Equals("PageWait");
    }

    public bool IsBusy()
    {
        if (ButtonBlocker.Instance.IsActive()
            || MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch()
            || ServerApi.IsExists
            || !IsPageWait())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// ページの切り替え中もしくは、グローバルメニューが閉じる最中かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsPageClosing()
    {
        if (m_GlobalMenu.isShowed == false
            || IsPageWait() == false
            || MainMenuManager.Instance.CheckMenuControlNG()
            || MainMenuManager.Instance.IsPageSwitch()
            )
        {
            return true;
        }

        return false;
    }

    public void OnSelectReturn()
    {
        if (IsBusy()) return;

        if (AndroidBackKeyManager.Instance.isTopItem(m_GlobalMenu.gameObject.name))
        {
            m_GlobalMenu.OnPushReturn();
        }
        else
        {
            AndroidBackKeyManager.Instance.StackAction();
        }
    }

    public void OnSelectClose()
    {
        if (IsBusy()) return;

        m_GlobalMenu.OnPushClose();
    }

    public bool IsCangeTime()
    {
        // public MAINMENU_SEQ getReplaceNextPage(MAINMENU_SEQ eNextPage)
        // copy
        if (MainMenuParam.m_ReturnTitleTime != DateTime.MaxValue &&
            TimeManager.Instance != null &&
            TimeManager.Instance.m_TimeNow != null)
        {
            //--------------------------------
            // タイトルログイン時に設定された時間をすぎたらタイトルに戻す
            //--------------------------------
            if (TimeManager.Instance.m_TimeNow >= MainMenuParam.m_ReturnTitleTime)
            {
                DialogManager.Open1B("CHANGE_DAY", "CHANGE_DAY_DETAIL", "common_button1", true, false).
                SetOkEvent(() =>
                {
                    SceneCommon.Instance.GameToTitle();
                    SoundUtil.PlaySE(SEID.SE_MENU_OK2);
                });

                return true;
            }
        }

        return false;
    }
}
