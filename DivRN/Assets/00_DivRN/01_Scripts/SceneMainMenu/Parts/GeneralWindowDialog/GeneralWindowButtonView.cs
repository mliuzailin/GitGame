using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class GeneralWindowButtonView : ButtonView
{
    [SerializeField]
    TextMeshProUGUI m_Text;
    [SerializeField]
    Image m_Image;

    M4uProperty<bool> isView = new M4uProperty<bool>(false);
    public bool IsView { get { return isView.Value; } set { isView.Value = value; } }

    string m_DefaultText = "";

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        SetText("");
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDefaultText(string text)
    {
        m_DefaultText = text;
        SetText(text);
    }

    public void SetText(string text)
    {
        if (m_Text != null)
        {
            if (text.IsNullOrEmpty() == true)
            {
                m_Text.text = m_DefaultText;
            }
            else
            {
                m_Text.text = text;
            }
        }
    }

    public void SetSprite(Sprite sprite, bool isNativeSize = false)
    {
        if (m_Image != null)
        {
            m_Image.sprite = sprite;
            if (isNativeSize == true)
            {
                m_Image.SetNativeSize();
            }
        }

    }

}
