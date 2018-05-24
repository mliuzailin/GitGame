using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SortDialogTextButtonView : ButtonView
{
    [SerializeField]
    TextMeshProUGUI m_Text;
    [SerializeField]
    Button m_Button;

    public string m_OnText;
    public string m_OffText;

    public bool IsSelect { get; private set; }

    void Awake()
    {
        IsSelect = true;
    }

    // Use this for initialization
    void Start()
    {
        SetSelect(IsSelect);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSelect(bool selected)
    {
        IsSelect = selected;
        if (selected)
        {
            m_Text.text = m_OnText;
        }
        else
        {
            m_Text.text = m_OffText;
        }
    }
}
