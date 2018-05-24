using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using TMPro;

public class DialogButtonView : ButtonView
{
    [SerializeField]
    private TextMeshProUGUI m_text;
    [SerializeField]
    private Image m_switch;
    [SerializeField]
    private Sprite[] m_switchSprite;

    new public static T Attach<T>(string prefabPath, GameObject parent) where T : ButtonView
    {
        return ButtonView.Attach<T>(prefabPath, parent);
    }


    protected DialogButtonModel m_model = null;

    public void SetText(string text)
    {
        if (m_text != null)
            m_text.text = text;
    }

    public void SetSwitch(bool _sw)
    {
        if (m_switch != null)
        {
            if (_sw == true)
            {
                m_switch.sprite = m_switchSprite[0];
            }
            else
            {
                m_switch.sprite = m_switchSprite[1];
            }
        }
    }

    public bool IsCloseButton()
    {
        if (m_text == null)
            return false;

        return m_text.text == "閉じる"
            || m_text.text == "戻る";
    }
}
