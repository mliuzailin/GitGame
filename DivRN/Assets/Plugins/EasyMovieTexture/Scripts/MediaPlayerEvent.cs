using UnityEngine;
using System.Collections;

public class MediaPlayerEvent : MonoBehaviour {


	public MediaPlayerCtrl m_srcVideo;

	// Use this for initialization
	void Start () {
		m_srcVideo.OnReady += OnReady;
		m_srcVideo.OnVideoFirstFrameReady += OnFirstFrameReady;
		m_srcVideo.OnVideoError += OnError;
		m_srcVideo.OnEnd += OnEnd;
		m_srcVideo.OnResize += OnResize;

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnReady() {

#if BUILD_TYPE_DEBUG
		Debug.Log ("OnReady");
#endif
	}

	void OnFirstFrameReady() {
#if BUILD_TYPE_DEBUG
		Debug.Log ("OnFirstFrameReady");
#endif
	}

	void OnEnd() {
#if BUILD_TYPE_DEBUG
		Debug.Log ("OnEnd");
#endif
	}

	void OnResize()
	{
#if BUILD_TYPE_DEBUG
		Debug.Log ("OnResize");
#endif
	}

	void OnError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra){
#if BUILD_TYPE_DEBUG
		Debug.Log ("OnError");
#endif
	}
}
