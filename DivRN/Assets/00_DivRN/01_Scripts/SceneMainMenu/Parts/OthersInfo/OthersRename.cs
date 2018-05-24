/**
 *  @file   OthersRename.cs
 *  @brief
 *  @author Developer
 *  @date   2017/03/09
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using M4u;
using TMPro;

public class OthersRename : MenuPartsBase
{
    public Action<string> OnClickAction = delegate { };
    public Action<string> OnEndEditAction = delegate { };

    M4uProperty<string> nameDetailText = new M4uProperty<string>();
    public string NameDetailText { get { return nameDetailText.Value; } set { nameDetailText.Value = value; } }

    M4uProperty<string> detailText = new M4uProperty<string>();
    public string DetailText { get { return detailText.Value; } set { detailText.Value = value; } }

    M4uProperty<string> buttonText = new M4uProperty<string>();
    public string ButtonText { get { return buttonText.Value; } set { buttonText.Value = value; } }

    M4uProperty<string> labelText = new M4uProperty<string>();
    public string LabelText { get { return labelText.Value; } set { labelText.Value = value; } }

    private TMP_InputField field;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        field = GetComponentInChildren<TMP_InputField>();
        field.enabled = false;
    }

    void Start()
    {
        DetailText = GameTextUtil.GetText("he174p_text3");
        ButtonText = GameTextUtil.GetText("common_button7");
        field.enabled = true;
    }


    public void SetName(string name)
    {
        TMP_InputField field = GetComponentInChildren<TMP_InputField>();
        if (field != null)
        {
            field.text = name;
        }
    }

    public void OnClick()
    {
        TMP_InputField field = GetComponentInChildren<TMP_InputField>();
        if (field != null && OnClickAction != null)
        {
            TMP_TextInfo generatr = field.textComponent.textInfo;
            for (int i = 0; i < generatr.characterCount; i++)
            {
                TMP_CharacterInfo info = generatr.characterInfo[i];
#if BUILD_TYPE_DEBUG
                Debug.Log(i + " / " + "characterCount = " + info.pointSize);
#endif
                if (info.pointSize == 0)
                {
                    //文字幅がないものは表示できないフォントなのでエラー表示
                    Dialog newDialog = newDialog = Dialog.Create(DialogType.DialogOK);
                    newDialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText("common_button1"));
                    newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("error_response_title69"));
                    newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "error_response_content69");
                    newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() => { }));
                    newDialog.DisableCancelButton();
                    newDialog.Show();

                    return;
                }
            }

            OnClickAction(field.text);
        }
    }

    public void OnEndEdit(string text)
    {
        int max_length = text.Length >= GlobalDefine.USER_NAME_MAX_LENGTH ? GlobalDefine.USER_NAME_MAX_LENGTH : text.Length;
        text = text.Substring(0, max_length);
        field.text = text;

        if (OnEndEditAction != null)
        {
            OnEndEditAction(text);
        }
    }


    public void OnValueChange()
    {
        if (field != null)
        {
            string inStr = field.text;

            if (inStr.Length != 0)
            {
                System.Text.StringBuilder retStr = new System.Text.StringBuilder();
                System.Globalization.TextElementEnumerator tee =
                    System.Globalization.StringInfo.GetTextElementEnumerator(inStr);
                tee.Reset();

                while (tee.MoveNext())
                {
                    // 1文字取得
                    var te = tee.GetTextElement();
                    // 1文字が2つ以上のcharからなる場合は、サロゲートペアと判断
                    if (1 < te.Length)
                    {
                        // 文字列から除去
                    }
                    else
                    {
                        retStr = retStr.Append(te);
                    }
                }
                // InputFieldに返す
                field.text = retStr.ToString();
            }
        }
    }
}
