using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//----------------------------------------------------------------------------
/*!
	@brief	通信中タイプ
*/
//----------------------------------------------------------------------------
public enum LOADING_TYPE
{
    NONE,
    ASSETBUNDLE,        //!< 通信中タイプ：AssetBundle読み込み中
    SERVER_API,         //!< 通信中タイプ：サーバー通信中
    PATCH,              //!< 通信中タイプ：パッチ
    PLAYGAME_SERVICES,  //!< 通信中タイプ：PlayGameServices周り(SavedGamesなど)
    DATA_DOWNLOAD,      //!< そこそこ大きいデータやファイルのダウンロード
    TO_HOME,        	//!< ホーム画面へ遷移時の読み込み
    TO_BATTLE,        	//!< クエスト(バトル)画面へ遷移時の読み込み
    WITH_TIPS,        //!< Tips表示付きローディング演出汎用
    RETRY_WAIT,        	//!< リトライ待ちの処理
    GUARD,              //!< 通信表示なしのガードのみ
};
//----------------------------------------------------------------------------
/*!
	@brief	Loading表示管理クラス
*/
//----------------------------------------------------------------------------
public class LoadingManager : SingletonComponent<LoadingManager>
{
    [SerializeField]
    private GameObject m_effectRoot;
    [SerializeField]
    private GameObject m_LoadingStatsBar;

    [SerializeField]
    private GameObject m_TutorlalMask;

    private bool lastEffectMask = false;
    private bool lastStatusbarMask = false;

    private LoadingEffect m_currentEffect = null;
    private LOADING_TYPE m_currentType = LOADING_TYPE.NONE;


    //----------------------------------------------------------------------------
    /*!
		@brief	Loading表示有効化
	*/
    //----------------------------------------------------------------------------
    public void RequestLoadingStart(LOADING_TYPE eLoadingType = LOADING_TYPE.NONE)
    {
        if (MovieManager.HasInstance)
        {
            return;
        }

        if (m_currentEffect != null)
        {
            return;
        }

        m_currentType = eLoadingType;

        switch (eLoadingType)
        {
            case LOADING_TYPE.DATA_DOWNLOAD:
                m_currentEffect = LoadingEffectDownload.Attach(m_effectRoot);
                setMask(true, true);
                break;
            case LOADING_TYPE.TO_BATTLE:
            case LOADING_TYPE.TO_HOME:
                m_currentEffect = LoadingEffectSimple.Attach(m_effectRoot);
                setMask(true, true);
                break;
            case LOADING_TYPE.WITH_TIPS:
                m_currentEffect = LoadingEffectWithTips.Attach(m_effectRoot);
                break;
            case LOADING_TYPE.GUARD:
                m_currentEffect = LoadingEffectHTTPConnect.Attach(m_effectRoot);
                setMask(true, false);
                break;
            default:
                m_currentEffect = LoadingEffectHTTPConnect.Attach(m_effectRoot);
                setMask(true, false);
                break;
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	Loading表示無効化
	*/
    //----------------------------------------------------------------------------
    public void RequestLoadingFinish(LOADING_TYPE eLoadingType = LOADING_TYPE.NONE)
    {
        if (MovieManager.HasInstance)
        {
            return;
        }

        if (m_currentEffect == null)
        {
            return;
        }

        if (m_currentType != eLoadingType &&
            eLoadingType != LOADING_TYPE.NONE)
        {
            return;
        }

        setMask(false, false);

        m_currentEffect.Detach();
        m_currentEffect = null;
        m_currentType = LOADING_TYPE.NONE;
    }

    private void setMask(bool isEffect, Boolean isStatusbar)
    {
        if (m_effectRoot != null)
        {
            if (lastEffectMask != isEffect)
            {
                m_effectRoot.SetActive(isEffect);
                lastEffectMask = isEffect;
            }
        }

        if (m_LoadingStatsBar != null)
        {
            if (lastStatusbarMask != isStatusbar)
            {
                m_LoadingStatsBar.SetActive(isStatusbar);
                lastStatusbarMask = isStatusbar;
            }
        }
    }

    public void setOverLayMask(bool yes)
    {
        m_TutorlalMask.SetActive(yes);
    }


    // ======================== 暫定処理
    // TODO : このあたりのはローディング処理用のモデルを作ってそっちに移す
    public void Progress(float percent)
    {
        if (m_currentEffect == null)
        {
            return;
        }

        m_currentEffect.Progress(percent);
    }

    public void ProgressFiles(float current, float max)
    {
        if (m_currentEffect == null)
        {
            return;
        }

        m_currentEffect.ProgressFiles(current, max);
    }

    public void SetText(string text)
    {
        if (m_currentEffect == null)
        {
            return;
        }

        m_currentEffect.SetText(text);
    }

    public void SetTips(TipsModel model)
    {
        if (m_currentEffect == null)
        {
            return;
        }

        m_currentEffect.SetTips(model);
    }
}