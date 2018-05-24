using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SafeAreaControl : SingletonComponent<SafeAreaControl>
{
    [SerializeField]
    private GameObject goStatusBarMask = null;
    [SerializeField]
    private GameObject goBottomMask = null;

    public int disp_width { get; set; }
    public int disp_height { get; set; }
    public int view_top { get; set; }
    public int view_width { get; set; }
    public float view_density { get; set; }
    public float view_scale { get; set; }
    public int view_height { get; set; }
    public int bar_height { get; set; }
    public int ios_bar_width { get; set; }
    public int ios_bar_height { get; set; }
    public float ios_scale { get; set; }
    public int api_level { get; set; }

    public int bottom_space_height { get; set; }
    public bool safe_area_mask { get; set; }

    private bool checkViewTop = true;
    private bool isFixedViewTop = false;  //view_topが確定したかどうか

#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")] private static extern int iOS_GetStatusBarWidth();
	[DllImport("__Internal")] private static extern int iOS_GetStatusBarHeight();
	[DllImport("__Internal")] private static extern float iOS_ScreenScaleFactor();
#endif

    private Queue<System.Action> m_onBarHeightFixed = new Queue<Action>();
    public void AddOnBarHeightFixedCallback(System.Action callback)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (checkViewTop)
        {
            m_onBarHeightFixed.Enqueue(callback);
        }
        else
        {
            callback();
        }
#else
        callback();
#endif
    }

    protected override void Awake()
    {
        base.Awake();

        //一度分割表示にしてステータスバーの高さを取得する。
        // Android: (ステータスバー表示(ゲーム画面と分割表示されるタイプ)
        ApplicationChrome.States _statusBarState = ApplicationChrome.States.Visible;
#if UNITY_ANDROID && !UNITY_EDITOR
        //API Level 取得
        var cls = new AndroidJavaClass("android.os.Build$VERSION");
        api_level = cls.GetStatic<int>("SDK_INT");
        if (api_level >= 24)
        {
            // Android 7.0 以降
            // Android: (ステータスバー表示(ゲーム画面の上にオーバーライドされるタイプ)
            _statusBarState = ApplicationChrome.States.VisibleOverContent;
        }
#endif
        ApplicationChrome.statusBarState = _statusBarState;
    }

    protected override void Start()
    {
        base.Start();

#if UNITY_IOS && !UNITY_EDITOR
        ios_bar_width = iOS_GetStatusBarWidth();
        ios_bar_height = iOS_GetStatusBarHeight();
        ios_scale = iOS_ScreenScaleFactor();
        disp_width = Screen.width;
        disp_height = Screen.height;
        view_top = (int)((float)ios_bar_height * ((float)disp_width / (float)ios_bar_width));
        isFixedViewTop = true;

        //画面下のSafeArea計算 iOS用
        if (ios_bar_height == 44)
        {
            //iPhoneX
            bottom_space_height = (int)(Screen.height - Screen.safeArea.y - Screen.safeArea.height);
        }
        else
        {
            //通常のステータスバー
            bottom_space_height = 0;
        }
#else
        bottom_space_height = 0;
#endif

#if UNITY_EDITOR
        int sizeX = Screen.width;
        int sizeY = Screen.height;

        int sizeGcd = sizeX > sizeY ? sizeX % sizeY : sizeY % sizeX;
        if (sizeGcd <= 0)
        {
            sizeGcd = 1;
        }

        int aspectX = sizeX / sizeGcd;
        int aspectY = sizeY / sizeGcd;

        Debug.Log(string.Format("aspectX:{0} aspectY:{1} sizeGcd:{2}", aspectX, aspectY, sizeGcd));

        //Unityエディタでテストしたいときはここに数値を入れる
        bottom_space_height = 0;
        bar_height = 0;

        if (aspectX == 9 && aspectY == 19)
        {
            bottom_space_height = 132;
            bar_height = 50;
        }
        else if (sizeX == 640 && sizeY == 960)
        {
            bottom_space_height = 0;
            bar_height = 40;
        }
        else if (sizeX == 1125 && sizeY == 2436)
        {
            //iPhone Xと同じ表示になるはず
            bottom_space_height = 132;
            bar_height = 107;
        }

        if (bar_height > 0)
        {
            // ステータスバーのサイズをCanvasサイズに合わせる
            RectTransform rect = GetComponentInChildren<RectTransform>();
            if (rect != null)
            {
                bar_height = (int)(bar_height * (960.0f / rect.rect.height));
            }
        }

        checkViewTop = false;
        isFixedViewTop = true;
#endif

        setMask(false);

        statusDisp();
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void runOnAndroidUiThread(Action target)
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                activity.Call("runOnUiThread", new AndroidJavaRunnable(target));
            }
        }
    }

	private void updateStatusInThread() {
		using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
    			using (var wm = activity.Call<AndroidJavaObject>("getWindowManager")) {
        			using (var disp = wm.Call<AndroidJavaObject>("getDefaultDisplay")) {
                        using(AndroidJavaObject Point = new AndroidJavaObject("android.graphics.Point"))
                        {
                            disp.Call("getRealSize",Point);
                            disp_width = Point.Get<int>("x");
                            disp_height = Point.Get<int>("y");
                        }
                        using(AndroidJavaObject Metrics = new AndroidJavaObject("android.util.DisplayMetrics"))
                        {
                            disp.Call("getMetrics",Metrics);
                            view_width = Metrics.Get<int>("widthPixels");
                            view_height = Metrics.Get<int>("heightPixels");
                            view_density = Metrics.Get<int>("density");
                            view_scale = Metrics.Get<int>("scaledDensity");
                        }
                    }
                }
			}
		}
	}

	private void updateViewTopInThread() {
		using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
				using (var window = activity.Call<AndroidJavaObject>("getWindow")) {
					using (var view = window.Call<AndroidJavaObject>("getDecorView")) {
                        using(AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                        {
                            view.Call("getWindowVisibleDisplayFrame", Rct);
                            view_top = Rct.Get<int>("top");
                            isFixedViewTop = true;
                        }
                    }
				}
            }
        }
    }
#endif

    public void updateStatus()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    	runOnAndroidUiThread(updateStatusInThread);
#endif
    }

    public void updateViewTop()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    	runOnAndroidUiThread(updateViewTopInThread);
#endif
    }

    private void Update()
    {
        if (checkViewTop == false)
        {
            return;
        }

        if (isFixedViewTop &&
            disp_width != 0 &&
            disp_height != 0)
        {
            //高さが取得できたら
            // Android: (ステータスバー表示(ゲーム画面の上にオーバーライドされるタイプ)
            ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;

            calcStatusBarHeight();

            checkViewTop = false;

            while (m_onBarHeightFixed.Count > 0)
            {
                m_onBarHeightFixed.Dequeue()();
            }
        }
        else
        {
            updateViewTop();
            updateStatus();
        }
    }

    private void calcStatusBarHeight()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Android 7.0 以降はステータスバーの高さを０に固定
        if(api_level >= 24 )
        {
            bar_height = 0;
            return;
        }
#endif

        float base_aspect = 640.0f / 960.0f;
        float disp_aspect = (float)disp_width / (float)disp_height;
        if (disp_aspect <= base_aspect)
        {
            bar_height = (int)((float)view_top * (640.0f / (float)disp_width));
        }
        else
        {
            bar_height = (int)((float)view_top * (960.0f / (float)disp_height));
        }

        statusDisp();
    }

    public int getNativeTop()
    {
#if UNITY_EDITOR
        return bar_height;
#elif UNITY_ANDROID
        return view_top;
#elif UNITY_IOS
        return ios_bar_height;
#else
        return bar_height;
#endif
    }

    private void statusDisp()
    {
#if BUILD_TYPE_DEBUG
        var log = string.Format("disp: Screen.widt:{0} / Screen.widt:{1}", Screen.width, Screen.height);
        UnityEngine.Debug.Log(log);

        log = string.Format("disp: Screen.safeArea.x:{0} / Screen.safeArea.y:{1}", Screen.safeArea.x, Screen.safeArea.y);
        UnityEngine.Debug.Log(log);

        log = string.Format("disp: Screen.safeArea.width:{0} / Screen.safeArea.height:{1}", Screen.safeArea.width, Screen.safeArea.height);
        UnityEngine.Debug.Log(log);

        log = string.Format("disp: bar_height:{0} / bottom_space_height:{1}", bar_height, bottom_space_height);
        UnityEngine.Debug.Log(log);

        log = string.Format("disp: ios_bar_width:{0} / ios_bar_height:{1} / ios_scale:{2}", ios_bar_width, ios_bar_height, ios_scale);
        UnityEngine.Debug.Log(log);
#endif
    }

    public void setMask(bool yes)
    {
        safe_area_mask = yes;

        if (bottom_space_height > 0)
        {
            if (goStatusBarMask != null)
            {
                goStatusBarMask.SetActive(safe_area_mask);
            }

            if (goBottomMask != null)
            {
                goBottomMask.SetActive(safe_area_mask);
            }
        }
    }

    public void adjustanchoredPosition(RectTransform rect)
    {
        if (rect == null)
        {
            return;
        }
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,
                                            rect.anchoredPosition.y + bottom_space_height);
    }

    public void fitTopAndBottom(RectTransform rect)
    {
        if (rect == null)
        {
            return;
        }

        float bar_height = (float)(this.bar_height * -1);
        float bar_height_half = bar_height * 0.5f;
        float bottom_height = (float)(bottom_space_height * -1);
        float bottom_height_half = bottom_height * 0.5f;

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,
                                            rect.anchoredPosition.y + bar_height_half - bottom_height_half);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x,
                                     rect.sizeDelta.y + bar_height + bottom_height);
    }

    public void addLocalYPos(Transform transform, bool reverse = false)
    {
        if (bottom_space_height > 0)
        {
            int height = bottom_space_height * (reverse ? -1 : 1);
            transform.AddLocalPositionY(height);
        }
    }

    public void enebleMask(GameObject top, GameObject bottom)
    {
        if (bottom_space_height <= 0)
        {
            return;
        }

        if (bottom != null)
        {
            bottom.SetActive(true);
        }

        if (bar_height > 0)
        {
            if (top != null)
            {
                top.SetActive(true);
            }
        }
    }
}
