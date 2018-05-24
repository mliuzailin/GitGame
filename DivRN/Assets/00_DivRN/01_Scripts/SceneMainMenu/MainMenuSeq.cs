/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuSeq.cs
	@brief	メインメニューシーケンス基底クラス
	@author Developer
	@date 	2012/11/27

	メインメニューの各ページの、フェード処理周りの隠蔽に特化したクラス。

	[ AnimationClipFade ] のスクリプトを使って動く。
	[ AnimationClipFade ] は「 フェードイン , フェードアウト , 待機 , ランダム待機 」のアニメーションを保持するだけのクラスになっており
	インスペクタ上でAnimationClipFadeコンポーネントを付けてアニメーションをアサインしている階層を探し出し、
	そこに対してアニメーションを発行、監視することに特化する。

	アニメーションの作り方的に、なるべくパーツごとにバラバラにアニメーションを入れたいらしいので、
	階層以下のアニメーションコンポーネントアサインオブジェクトを選出しておき、
	フェードの際にはそれらに同時タイミングでアニメーションを発行する。

	発行後は階層以下のオブジェクトの全アニメーションが完遂したことを確認してフェード完遂と見なす。
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	メインメニューシーケンス基底クラス
*/
//----------------------------------------------------------------------------
public class MainMenuSeq : MonoBehaviour
{
    /*==========================================================================*/
    /*		var and accessor													*/
    /*==========================================================================*/
    AnimationClipFadeTop m_AnimationClipFadeTop = null;

    public bool m_MainMenuSeqStartOK = false;

    public bool m_PageEventEnableBefore = false;

    public bool m_BackEventPageUpdate = false;

    protected GameObject m_CanvasObj = null;
    private CanvasGroup m_CanvasGroup = null;
    private Tweener m_Fade = null;

    private bool m_SuspendReturn = false;
    public bool IsSuspendReturn { get { return m_SuspendReturn; } }

    private int m_UpdateDelayCount = 0;

    private System.Action m_onFadeOutFinished = null;
    public void RegisterOnFadeOutFinishedCallback(System.Action callback) { m_onFadeOutFinished = callback; }
    public void RunOnFadeOutFinishedCallback()
    {
        if (m_onFadeOutFinished != null)
        {
            m_onFadeOutFinished();
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
    */
    //----------------------------------------------------------------------------
    protected virtual void Start()
    {
        //--------------------------------
        // アニメーションフェード管理クラスを追加
        //--------------------------------
        m_AnimationClipFadeTop = gameObject.GetComponent<AnimationClipFadeTop>();
        if (m_AnimationClipFadeTop == null)
        {
            m_AnimationClipFadeTop = gameObject.AddComponent<AnimationClipFadeTop>();
        }

        m_CanvasObj = UnityUtil.GetChildNode(gameObject, "Canvas");
        if (m_CanvasObj != null)
        {
            m_CanvasGroup = m_CanvasObj.GetComponent<CanvasGroup>();
            m_CanvasGroup.alpha = 0.0f;
            m_CanvasGroup.blocksRaycasts = false;

            //カメラ設定
            m_CanvasObj.GetComponent<Canvas>().worldCamera = SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>();
        }
        MainMenuManager.Instance.m_ResumePatchUpdateRequest = true;
    }

    public virtual void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

        if (m_UpdateDelayCount > 0)
        {
            m_UpdateDelayCount--;
        }
    }

    public virtual void OnDestroy()
    {
        if (m_Fade != null)
        {
            m_Fade.Kill();
            m_Fade = null;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	フェードアニメーション処理	※定期処理
    */
    //----------------------------------------------------------------------------
    protected bool PageSwitchUpdate()
    {
        if (SceneCommon.Instance == null ||
            SceneCommon.Instance.IsLoadingScene == true)
        {
            return false;
        }


        //--------------------------------
        // 準備完了待ち
        //--------------------------------
        if (m_MainMenuSeqStartOK == false)
        {
            if (m_AnimationClipFadeTop != null &&
                m_AnimationClipFadeTop.m_AnimationSeq != AnimationClipFadeTop.ANIM_FADE_NONE
            )
            {
                Debug.LogError("[Menu:o] MainMenuSeq Ready! - " + gameObject.name);
                m_MainMenuSeqStartOK = true;
            }
            else
            {
                return false;
            }
        }

        if (!CheckFade())
        {
            return false;
        }

        //----------------------------------------
        // 子供のフェード処理更新
        //----------------------------------------
        if (m_AnimationClipFadeTop != null)
        {
            m_AnimationClipFadeTop.AnimationUpdate();
        }

        //----------------------------------------
        // 管理側から操作許可が出ないならスルー
        //----------------------------------------
        if (MainMenuManager.HasInstance &&
            MainMenuManager.Instance.CheckMenuControlNG() == true)
        {
            return false;
        }

        //--------------------------------
        // リモート通知：更新
        // @add Developer 2016/10/31
        //--------------------------------
        RemoteNotificationManager.UpdateProcess();

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替え完遂チェック
    */
    //----------------------------------------------------------------------------
    public bool PageSwitchFinishCheck()
    {
        if (!CheckFade())
        {
            return false;
        }
        if (m_AnimationClipFadeTop != null)
        {
            return m_AnimationClipFadeTop.FadeFinishCheck();
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替えメッセージ処理
        @note	ページ切り替えの際にSendMessageで呼び出される関数。
    */
    //----------------------------------------------------------------------------
    public void PageSwitchTrigerDisable()
    {
        PageSwitchSetting(false);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ページ切り替えメッセージ処理
        @note	ページ切り替えの際にSendMessageで呼び出される関数。
    */
    //----------------------------------------------------------------------------
    public void PageSwitchTriger(bool bFast = false)
    {
        if (m_MainMenuSeqStartOK == false)
        {
            Debug.LogError("Page NG!");
        }

        bool animationSkip = false;
        if (gameObject.layer == LayerMask.NameToLayer("GUI"))
        {
            animationSkip = true;
        }

        //--------------------------------
        // アニメーション再生指示
        //--------------------------------
        if (animationSkip == true)
        {
            m_CanvasGroup.alpha = 1.0f;

            if (m_BackEventPageUpdate == true)
            {
                PageSwitchSetting(false);
            }
        }
        else
        {
            PageSwitchSetting(true);

            //--------------------------------
            // 初期化中のレイアウトを見られたくないので
            // 一時的に表示しないレイヤーに設定している。
            //
            // そのままだと表示されないのでGUIレイヤーに書き換えて描画有効化
            //--------------------------------
            UnityUtil.SetObjectLayer(gameObject, LayerMask.NameToLayer("GUI"));

            m_AnimationClipFadeTop.AnimationTriger(true, bFast);
            StartFadeIn(bFast);
        }
    }

    public void ClosePage(bool bFast = false)
    {
        m_AnimationClipFadeTop.AnimationTriger(false, bFast);
        StartFadeOut();
    }

    // ページ離脱時に呼ばれる
    virtual public void OnPageFinished()
    { }

    /// <summary>
    /// 基底継承：MainMenuSeq：ページ切り替えにより有効化された際に呼ばれる関数
    /// <para>ページのレイアウト再構築を兼ねる</para>
    /// </summary>
    /// <param name="bActive"></param>
    /// <param name="bBack"></param>
    protected virtual void PageSwitchSetting(bool initalize)
    {
    }

    /// <summary>
    /// 基底継承：MainMenuSeq：ページ無効化直前に走るイベント
    /// <para>処理中を返す間中はページを次のページが移行せずに処理を続ける</para>
    /// </summary>
    /// <returns>[ true:処理中 / false:処理完遂 ]</returns>
    public virtual bool PageSwitchEventDisableBefore()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.blocksRaycasts = false;
        return false;
    }

    /// <summary>
    /// 基底継承：MainMenuSeq：ページ無効化直後に走るイベント
    /// <para>処理中を返す間中はページを次のページが移行せずに処理を続ける</para>
    /// </summary>
    /// <param name="eNextMainMenuSeq"></param>
    /// <returns>[ true:処理中 / false:処理完遂 ]</returns>
    public virtual bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        //戻るボタン抑制解除
        SetSuspendReturn(false);
        return false;
    }

    /// <summary>
    /// 基底継承：MainMenuSeq：ページ有効化直前に走るイベント
    /// <para>処理中を返す間中はページを次のページが移行せずに処理を続ける</para>
    /// </summary>
    /// <returns>[ true:処理中 / false:処理完遂 ]</returns>
    public virtual bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        m_PageEventEnableBefore = true;
        return false;
    }

    /// <summary>
    /// 基底継承：MainMenuSeq：ページ有効化直後に走るイベント
    /// <para>処理中を返す間中はページを次のページが移行せずに処理を続ける</para>
    /// </summary>
    /// <returns>[ true:処理中 / false:処理完遂 ]</returns>
    public virtual bool PageSwitchEventEnableAfter()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.blocksRaycasts = true;
        return false;
    }

    public virtual void PageUpdateStatusFromMainMenu(GLOBALMENU_SEQ seq)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("MainMenuSeq UpdateUserStatusFromGlobalMenu:" + seq);
#endif
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		バックボタンを押した際のタイトル遷移共通処理
        @param[in]	bool		(pause)		処理を行った場合true
    */
    //----------------------------------------------------------------------------
    public bool BackToTitleCommon()
    {
        return false;
    }

    public void StartFadeIn(bool bFast)
    {
        if (m_Fade != null)
        {
            m_Fade.Kill();
            m_Fade = null;
        }
        m_CanvasGroup.alpha = 0.0f;
        if (bFast)
        {
            m_Fade = m_CanvasGroup.DOFade(1f, 0.0f);
        }
        else
        {
            m_Fade = m_CanvasGroup.DOFade(1f, 0.5f);
        }
    }

    public void StartFadeOut()
    {
        if (!UnityUtil.ChkObjectEnabled(gameObject))
        {
            return;
        }

        if (m_Fade != null)
        {
            m_Fade.Kill();
            m_Fade = null;
        }
        m_CanvasGroup.alpha = 1.0f;
        m_Fade = m_CanvasGroup.DOFade(0f, 0.5f);
    }

    public bool CheckFade()
    {
        if (m_Fade != null &&
            m_Fade.IsPlaying())
        {
            return false;
        }
        return true;
    }

    protected void SetSuspendReturn(bool flag)
    {
        m_SuspendReturn = flag;
    }


    protected bool ChkUserDataUpdate()
    {
        if (UserDataAdmin.Instance.IsUpdateUserData &&
            m_UpdateDelayCount == 0)
        {
            m_UpdateDelayCount = 5;
            return true;
        }
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		アプリケーションポーズ時のイベント処理
		@param[in]	bool		(pause)		アプリの状態ポーズ/レジューム
	*/
    //----------------------------------------------------------------------------
    void OnApplicationPause(bool pause)
    {
        if (pause == false)
        {
            // チュートリアル中ならレジューム時のリクエストを行わない.
            if (TutorialManager.IsExists == false)
            {
                MainMenuManager.Instance.RequestPatchUpdate();
                //--------------------------------
                // リモート通知：デバイスIDリクエスト
                // @add Developer 2016/10/31
                //--------------------------------
                RemoteNotificationManager.RequestDeviceID();
            }
        }
    }
}
