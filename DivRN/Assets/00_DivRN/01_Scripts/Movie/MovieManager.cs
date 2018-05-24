using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovieManager : SingletonComponent<MovieManager>
{
    [SerializeField]
    private MediaPlayerCtrl m_ScrMedia = null;                          //!<
    [SerializeField]
    private TitleMovie m_TitleMovie = null;

    [SerializeField]
    private GameObject m_ImagePanel = null;

    [SerializeField]
    private GameObject m_SkipObject = null;
    [SerializeField]
    private GameObject m_LodingObject = null;
    [SerializeField]
    private GameObject m_PercentObject = null;
    [SerializeField]
    private Image m_percentIcon = null;
    [SerializeField]
    private TextMeshProUGUI m_PercentNum = null;
    [SerializeField]
    private GameObject m_kabe = null;

    private bool m_loop = false;
    private bool m_clear = false;
    private bool m_destroy = false;
    private BGMManager.EBGM_ID m_bgmid = BGMManager.EBGM_ID.eBGM_INIT;

    public bool isMoviePlay { get { return m_TitleMovie.m_bPlayMovie; } }
    public System.Action skipButtonAction = delegate { };

    public void play(string _name, bool _skip, bool _loop, bool _loding = false, bool _clear = true, BGMManager.EBGM_ID _bgm = BGMManager.EBGM_ID.eBGM_INIT, bool _destroy = false)
    {
        //BGM停止
        SoundUtil.StopBGM(false);

        m_loop = _loop;

        UnityUtil.SetObjectEnabledOnce(gameObject, true);

        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.adjustanchoredPosition(m_ImagePanel.GetComponent<RectTransform>());
        }

        m_TitleMovie.m_bPlayMovie = true;

        m_ScrMedia.DeleteVideoTexture();
        m_ScrMedia.Load(_name);
        m_ScrMedia.Play();

        m_ScrMedia.OnEnd += () =>
        {
            if (m_loop == true)
            {
                m_ScrMedia.Play();
            }
            else
            {
                finishMovie();
            }
        };

        m_ScrMedia.OnVideoError += (MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra) =>
        {
            finishMovie();
        };

        setSkip(_skip);

        if (_loding == false)
        {
            UnityUtil.SetObjectEnabled(m_LodingObject, false);
        }
        UnityUtil.SetObjectEnabled(m_PercentObject, false);

        m_clear = _clear;
        m_bgmid = _bgm;
        m_destroy = _destroy;
    }

    public void setSkip(bool _skip)
    {
        if (_skip == true)
        {
            m_TitleMovie.onClick += () =>
            {
                SoundManager.Instance.PlaySE(SEID.SE_MENU_OK);
                finishMovie();

                if (skipButtonAction != null)
                {
                    skipButtonAction();
                }
            };
            UnityUtil.SetObjectEnabled(m_SkipObject, true);
            UnityUtil.SetObjectEnabled(m_LodingObject, false);
            UnityUtil.SetObjectEnabled(m_PercentObject, false);
        }
        else
        {
            if (m_TitleMovie.onClick != null)
            {
                m_TitleMovie.onClick = null;
            }
            UnityUtil.SetObjectEnabled(m_SkipObject, false);
        }
    }

    public void setLoop(bool _loop)
    {
        m_loop = _loop;
    }

    public void setPercent(float percent)
    {
        if (UnityUtil.ChkObjectEnabled(m_PercentObject) == false)
        {
            UnityUtil.SetObjectEnabled(m_PercentObject, true);
        }

        if (m_percentIcon.enabled == false)
        {
            m_percentIcon.enabled = true;
        }

        m_PercentNum.text = Mathf.FloorToInt(percent).ToString();
    }

    public void setProgressFiles(float current, float max)
    {
        if (UnityUtil.ChkObjectEnabled(m_PercentObject) == false)
        {
            UnityUtil.SetObjectEnabled(m_PercentObject, true);
        }

        if (m_percentIcon.enabled == true)
        {
            m_percentIcon.enabled = false;
        }

        m_PercentNum.text = String.Format("{0,2}/{1,2}", current, max);
    }

    public void finishMovie()
    {
        m_TitleMovie.m_bPlayMovie = false;
        if (m_ScrMedia.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
        {
            m_ScrMedia.Stop();
        }
        m_ScrMedia.DeleteVideoTexture();
        if (m_clear == true)
        {
            UnityUtil.SetObjectEnabledOnce(gameObject, false);
            if (m_destroy == true)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            UnityUtil.SetObjectEnabled(m_kabe, true);
        }
        if (m_bgmid != BGMManager.EBGM_ID.eBGM_INIT)
        {
            SoundUtil.PlayBGM(m_bgmid, false);
        }
    }
}
