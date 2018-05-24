using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaPlayer : SingletonComponent<MediaPlayer>
{
    private MediaPlayerCtrl ctrl;
    private Action finishPlayAction;

    protected override void Awake()
    {
        ctrl = GetComponentInChildren<MediaPlayerCtrl>();
    }

    protected override void Start()
    {
        //最初のフレームのロードが完了
        ctrl.OnVideoFirstFrameReady += () =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CALL MediaPlayer#OnVideoFirstFrameReady");
#endif
            ctrl.Play();
        };
        //エラー
        ctrl.OnVideoError += (errorCode, errorExtra) =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CALL MediaPlayer#OnVideoError errorCode:" + errorCode.ToString() + " errorExtra:" + errorExtra.ToString());
#endif
        };
        //再生終了
        ctrl.OnEnd += () =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CALL MediaPlayer#OnEnd");
#endif
            OnEnd();
        };
        //OnReadyを有効にすると動画が再生されない
        //        ctrl.OnReady += () =>
        //        {
#if BUILD_TYPE_DEBUG
        //            Debug.Log("CALL MediaPlayer#OnReady");
#endif
        //        };
        //        ctrl.OnResize += () =>
        //        {
#if BUILD_TYPE_DEBUG
        //            Debug.Log("CALL MediaPlayer#OnResize");
#endif
        //        };
    }

    void OnEnd()
    {
        if (finishPlayAction != null)
        {
            finishPlayAction();
        }
        Destroy(gameObject);
    }

    public static MediaPlayer Create()
    {
        string prefabPath = "Prefab/MediaPlayer";
        GameObject prefab = Resources.Load(prefabPath) as GameObject;
        GameObject go = Instantiate(prefab) as GameObject;
        MediaPlayer result = go.GetComponent<MediaPlayer>();
        return result;
    }

    public void Play()
    {
        ctrl.Play();
    }

    public void LoadAndPlay(string fileName, Action finish)
    {
        Load(fileName, finish, true);
    }


    public void Load(string fileName, Action finish, bool autoPlay = false)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL MediaPlayer#Play:" + fileName);
#endif
        this.finishPlayAction = finish;
        //entry以外はダウンロードしてきて保存されている前提
        //        if (!fileName.StartsWith("entry"))
        //        {
        //            fileName = "file://" + Application.persistentDataPath + "/" + fileName;
        //        }

        ctrl.m_bAutoPlay = autoPlay;
        ctrl.Load(fileName);
    }


    public void OnTouch()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL MediaPlayer#OnTouch:" + ctrl.GetCurrentState());
#endif
        if (ctrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
        {
            return;
        }
        if (ctrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY)
        {
            return;
        }
        Stop();
    }


    void Update()
    {
#if BUILD_TYPE_DEBUG
        //        Debug.Log("CTRL_STATE:" + ctrl.GetCurrentState());
#endif
    }

    public void Stop()
    {
        ctrl.Stop();
        OnEnd();
    }
}
