using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginBonusCloseButton : ButtonView
{
    [SerializeField]
    Text m_Text;

    // Use this for initialization
    void Start()
    {
        if (m_Text != null)
        {
            m_Text.text = GameTextUtil.GetText("common_button1");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
