using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class MissionDialog : M4uContextMonoBehaviour
{
    // 文字列
    M4uProperty<string> title = new M4uProperty<string>("");
    M4uProperty<string> under_text = new M4uProperty<string>("");
    M4uProperty<string> yes_text = new M4uProperty<string>("");

    public string Title { get { return title.Value; } set { title.Value = value; } }
    public string Under_text { get { return under_text.Value; } set { under_text.Value = value; } }
    public string Yes_text { get { return yes_text.Value; } set { yes_text.Value = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {
    }

    // ダイアログのメッセージテキストをセット
    public void SetMessage(string text1, string text2, string text3)
    {
        Title = text1 + "\n";
        Under_text = text2 + "\n";
        Yes_text = text3 + "\n";
    }

    // クリック時のフィードバック
    public void OnClickButton()
    {
        Destroy(gameObject);
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClick MissionDialogButton:");
#endif
    }
}
