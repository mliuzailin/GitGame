/**
 *  @file   SceneWebViewTest.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/20
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneWebViewTest : SceneTest<SceneWebViewTest>
{
    public InputField m_InputField;

    protected override void Awake()
    {
        base.Awake();
        DestroyObject(GetComponentInChildren<WebView>().gameObject);
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_InputField.text = "http://divine.example.com/member/";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnInitialized()
    {
        base.OnInitialized();

    }

    public void OnClickToBasic()
    {
        SceneManager.LoadScene("1.Basic");
    }

    public void OnClickToUseWithCode()
    {
        SceneManager.LoadScene("2.UseWithCode");
    }

    public void OnClickToLocalHTML()
    {
        SceneManager.LoadScene("3.LocalHTML");
    }

    public void OnClickToSizeAndTransition()
    {
        SceneManager.LoadScene("4.SizeAndTransition");
    }

    public void OnClickToCallbackFromWeb()
    {
        SceneManager.LoadScene("5.CallbackFromWeb");
    }

    public void OnClickToRunJavaScriptInWeb()
    {
        SceneManager.LoadScene("6.RunJavaScriptInWeb");
    }

    public void OnClickCreateWebView()
    {
        WebView.OpenWebView(m_InputField.text);
    }

    void ClosedWebView()
    {
        Debug.Log("ClosedWebView");
    }

    public void OnClickOpenBrowser()
    {
        Application.OpenURL(m_InputField.text);
    }
}
