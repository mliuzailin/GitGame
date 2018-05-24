﻿#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class UniWebViewPlugin {
    private static AndroidJavaClass webView;

    public static void Init(string name, int top, int left, int bottom, int right) {
#if BUILD_TYPE_DEBUG
        Debug.Log("Unity Init");
#endif
        if (Application.platform == RuntimePlatform.Android) {
            webView = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");

            webView.CallStatic("_UniWebViewInit", name, top, left, bottom, right);
        }
    }

    public static void ChangeInsets(string name, int top, int left, int bottom, int right) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewChangeInsets", name, top, left, bottom, right);
        }
    }

    public static void Load(string name, string url) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewLoad", name, url);
        }
    }

    public static void LoadHTMLString(string name, string htmlString, string baseUrl) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewLoadHTMLString", name, htmlString, baseUrl);
        }
    }

    public static void Reload(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewReload", name);
        }
    }

    public static void Stop(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewStop", name);
        }
    }

    public static void EvaluatingJavaScript(string name, string javaScript) {
        if (Application.platform == RuntimePlatform.Android) {
#if BUILD_TYPE_DEBUG
            Debug.Log("calling eval js " + javaScript);
#endif
            webView.CallStatic("_UniWebViewEvaluatingJavaScript", name, javaScript);
        }
    }

    public static void AddJavaScript(string name, string javaScript) {
        if (Application.platform == RuntimePlatform.Android) {
#if BUILD_TYPE_DEBUG
            Debug.Log("adding js " + javaScript);
#endif
            webView.CallStatic("_UniWebViewAddJavaScript", name, javaScript);
        }
    }

    public static void Show(string name, bool fade, int direction, float duration) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewShow", name, fade, direction, duration);
        }
    }

    public static void Hide(string name, bool fade, int direction, float duration) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewHide", name, fade, direction, duration);
        }
    }

    public static void CleanCache(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewCleanCache", name);
        }
    }

    public static void CleanCookie(string name, string key) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewCleanCookie", name, key);
        }
    }

    public static void Destroy(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewDestroy", name);
        }
    }

    public static void SetSpinnerShowWhenLoading(string name, bool show) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetSpinnerShowWhenLoading", name, show);
        }
    }

    public static void SetSpinnerText(string name, string text) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetSpinnerText", name, text);
        }
    }

    public static void TransparentBackground(string name, bool transparent) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewTransparentBackground", name, transparent);
        }
    }

    public static void SetBackgroundColor(string name, float r, float g, float b, float a) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetBackgroundColor", name, r, g, b, a);
        }
    }

    public static bool CanGoBack(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            return webView.CallStatic<bool>("_UniWebViewCanGoBack", name);
        }
        return false;
    }

    public static bool CanGoForward(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            return webView.CallStatic<bool>("_UniWebViewCanGoForward", name);
        }
        return false;
    }

    public static void GoBack(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewGoBack", name);
        }
    }

    public static void GoForward(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewGoForward", name);
        }
    }

    public static string GetCurrentUrl(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            return webView.CallStatic<string>("_UniWebViewGetCurrentUrl", name);
        }
        return "";
    }

    public static void SetBackButtonEnable(string name, bool enable) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetBackButtonEnable", name, enable);
        }
    }

    public static void SetBounces(string name, bool enable) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetBounces", name, enable);
        }
    }

    public static void SetZoomEnable(string name, bool enable) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetZoomEnable", name, enable);
        }
    }

    public static void AddUrlScheme(string name, string scheme) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewAddUrlScheme", name, scheme);
        }
    }

    public static void RemoveUrlScheme(string name, string scheme) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewRemoveUrlScheme", name, scheme);
        }
    }

    public static void SetUseWideViewPort(string name, bool use) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewUseWideViewPort", name, use);
        }
    }

    public static void LoadWithOverviewMode(string name, bool overview) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewLoadWithOverviewMode", name, overview);
        }
    }

    public static void SetUserAgent(string userAgent) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetUserAgent", userAgent);
        }
    }

    public static string GetUserAgent(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            return webView.CallStatic<string>("_UniWebViewGetUserAgent", name);
        }
        return "";
    }

    public static float GetAlpha(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            return webView.CallStatic<float>("_UniWebViewGetAlpha", name);
        }
        return 0.0f;
    }

    public static void SetAlpha(string name, float alpha) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetAlpha", name, alpha);
        }
    }

    public static void SetImmersiveModeEnabled(string name, bool enabled) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetImmersiveModeEnabled", name, enabled);
        }
    }

    public static void AddPermissionRequestTrustSite(string name, string url) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewAddPermissionRequestTrustSite", name, url);
        }
    }

    public static void SetHeaderField(string name, string key, string value) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetHeaderField", name, key, value);
        }
    }
    
    public static void SetVerticalScrollBarShow(string name, bool show) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetVerticalScrollBarShow", name, show);
        }
    }
    
    public static void SetHorizontalScrollBarShow(string name, bool show) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetHorizontalScrollBarShow", name, show);
        }
    }
    
    public static bool GetOpenLinksInExternalBrowser(string name) {
        if (Application.platform == RuntimePlatform.Android) {
            return webView.CallStatic<bool>("_UniWebViewGetOpenLinksInExternalBrowser", name);
        }
        return false;
    }

    public static void SetOpenLinksInExternalBrowser(string name, bool value) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetOpenLinksInExternalBrowser", name, value);
        }
    }

    public static void SetWebContentsDebuggingEnabled(bool enabled) {
        if (Application.platform == RuntimePlatform.Android) {
            webView.CallStatic("_UniWebViewSetWebContentsDebuggingEnabled", enabled);
        }
    }
}
#endif
