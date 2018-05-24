/**
 *  @file   URLManager.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/09
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class URLManager
{
#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")] private static extern float iOS_GetiOSVersion();
#endif

    /// <summary>
    /// Webview表示かブラウザ表示か判断する
    /// </summary>
    /// <param name="url"></param>
    /// <returns>true:内部　false:外部</returns>
    static public bool CheckLoadURL(Uri uri)
    {
        // UniWebView.mm 下記のメソッドでも同一判定をチェックしている
        // -(BOOL)webView:(UniWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType {
        List<string> inBrowserUrls = Patcher.Instance.GetInBrowserUrls();
        string url = uri.AbsoluteUri;
        //Debug.Log("CheckLoadURL: url " + url);

        //https以外の場合は外部
        if (uri.Scheme.StartsWith("https") == false)
        {
            //Debug.Log("CheckLoadURL: false uri.Scheme " + uri.Scheme);
            return false;
        }

        //example.comドメインは内部
        if (0 <= uri.Host.IndexOf(GlobalDefine.IN_BROWSER_DOMAIN))
        {
            //Debug.Log("CheckLoadURL: true uri.Host " + uri.Host);
            return true;
        }

        //指定の文字列がある場合は内部
        for (int i = 0; i < inBrowserUrls.Count; i++)
        {
            if (0 <= url.IndexOf(inBrowserUrls[i]))
            {
                //Debug.Log("CheckLoadURL: true inBrowserUrls " + inBrowserUrls[i]);

                return true;
            }
        }

        //Debug.Log("CheckLoadURL: end false");

        return false;
    }

    /// <summary>
    /// URLを開く
    /// </summary>
    /// <param name="url"></param>
    /// <param name="isInline"></param>
    public static bool OpenURL(string url, bool openUrl = false)
    {
        //URLが空の場合は処理しない
        if (url == null || url.Length <= 0)
        {
            return false;
        }

        Uri uri = null;
        try
        {
            uri = new Uri(url);
        }
        catch (Exception e)
        {
            return false;
        }

        bool isInline = CheckLoadURL(uri);

        if (openUrl == true)
        {
            isInline = false;
        }

#if UNITY_IOS && !UNITY_EDITOR
#if BUILD_TYPE_DEBUG
		Debug.Log("iOS_GetiOSVersion: " +  iOS_GetiOSVersion());
#endif
		if(iOS_GetiOSVersion() < 8.0)
		{
			isInline = false;
		}
#endif

        if (isInline)
        {
            //webview表示
            MainMenuParam.m_DialogWebViewURL = url;
            WebView.OpenWebView(url, () => { });
        }
        else
        {
            // 外部ブラウザで開く
            Application.OpenURL(url);
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("OpenURL: " + url);
#endif
        return true;
    }
}
