using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMenuSeq : MonoBehaviour
{
    protected int m_PageCurrentCt = 0;
    private CanvasGroup m_CanvasGroup = null;
    private Tweener m_Fade = null;
    public bool IsStart { get; private set; }

    private System.Action m_onFadeOutFinished = null;
    public void RegisterOnFadeOutFinishedCallback(System.Action callback) { m_onFadeOutFinished = callback; }
    public void RunOnFadeOutFinishedCallback()
    {
        if (m_onFadeOutFinished != null)
        {
            m_onFadeOutFinished();
        }
    }
    private void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        if (m_CanvasGroup != null)
        {
            m_CanvasGroup.alpha = 0.0f;
        }
        IsStart = false;
    }

    // Use this for initialization
    protected virtual void Start()
    {
        IsStart = true;
    }

    // Update is called once per frame
    public void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

    }

    protected bool PageSwitchUpdate()
    {
        if (SceneCommon.Instance == null
        || SceneCommon.Instance.IsLoadingScene == true)
        {
            return false;
        }
        return true;
    }

    public void ForceActive()
    {
        if (m_CanvasGroup != null)
        {
            m_CanvasGroup.alpha = 1.0f;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替えメッセージ処理
		@note	ページ切り替えの際にSendMessageで呼び出される関数。
	*/
    //----------------------------------------------------------------------------
    public void PageSwitchTriger(bool bActive, bool bFast, bool bBack)
    {
        //--------------------------------
        // レイアウト再構築関数呼び出し
        //--------------------------------
        if (bActive)
        {
            PageSwitchSetting(true, bBack);
        }
        //--------------------------------
        // 初期化中のレイアウトを見られたくないので
        // 一時的に表示しないレイヤーに設定している。
        //
        // そのままだと表示されないのでGUIレイヤーに書き換えて描画有効化
        //--------------------------------
        if (bActive == true)
        {
            //if (gameObject.layer == LayerMask.NameToLayer("DRAW_CLIP")) {
            //    UnityUtil.SetObjectLayer(gameObject, LayerMask.NameToLayer("GUI"));
            //    m_CanvasGroup.alpha = 1.0f;
            //}

            StartFadeIn(bFast);
        }
        else
        {
            StartFadeOut();
        }
    }

    /// <summary>
    /// 基底継承：GlobalMenuSeq：ページ切り替えにより有効化された際に呼ばれる関数
    /// <para>ページのレイアウト再構築を兼ねる</para>
    /// </summary>
    /// <param name="bActive"></param>
    /// <param name="bBack"></param>
    protected virtual void PageSwitchSetting(bool bActive, bool bBack)
    {
        if (bActive == true)
        {
            m_PageCurrentCt++;
        }
    }
    /// <summary>
    /// 基底継承：GlobalMenuSeq：ページ無効化直前に走るイベント
    /// <para>処理中を返す間中はページを次のページが移行せずに処理を続ける</para>
    /// </summary>
    /// <returns>[ true:処理中 / false:処理完遂 ]</returns>
    public virtual bool PageSwitchEventDisableBefore()
    {
        return false;
    }

    /// <summary>
    /// 基底継承：GlobalMenuSeq：ページ無効化直後に走るイベント
    /// <para>処理中を返す間中はページを次のページが移行せずに処理を続ける</para>
    /// </summary>
    /// <param name="eNextGlobalMenuSeq"></param>
    /// <returns>[ true:処理中 / false:処理完遂 ]</returns>
    public virtual bool PageSwitchEventDisableAfter(GLOBALMENU_SEQ eNextGlobalMenuSeq)
    {
        return false;
    }

    /// <summary>
    /// 基底継承：GlobalMenuSeq：ページ有効化直前に走るイベント
    /// <para>処理中を返す間中はページを次のページが移行せずに処理を続ける</para>
    /// </summary>
    /// <returns>[ true:処理中 / false:処理完遂 ]</returns>
    public virtual bool PageSwitchEventEnableBefore()
    {
        return false;
    }

    /// <summary>
    /// 基底継承：GlobalMenuSeq：ページ有効化直後に走るイベント
    /// <para>処理中を返す間中はページを次のページが移行せずに処理を続ける</para>
    /// </summary>
    /// <returns>[ true:処理中 / false:処理完遂 ]</returns>
    public virtual bool PageSwitchEventEnableAfter()
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
        if (m_CanvasGroup == null) { return; }
        m_CanvasGroup.alpha = 1.0f;
        //m_CanvasGroup.alpha = 0.0f;
        //if (bFast) {
        //    m_Fade = m_CanvasGroup.DOFade(1f, 0.0f);
        //} else {
        //    m_Fade = m_CanvasGroup.DOFade(1f, 0.3f);
        //}
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

        if (m_CanvasGroup == null) { return; }
        m_CanvasGroup.alpha = 0.0f;
        //m_CanvasGroup.alpha = 1.0f;
        //m_Fade = m_CanvasGroup.DOFade(0f, 0.0f);
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

}
