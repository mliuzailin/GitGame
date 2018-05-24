using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySelectMemberButton : ButtonView
{
    [SerializeField]
    Button m_Button;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public PartySelectMemberButton SetModel(ButtonModel model)
    {
        model.OnUpdated += () =>
        {
            m_Button.interactable = model.isEnabled;
            SetTextures();
        };

        base.SetModel<ButtonModel>(model);

        return this;
    }

    void SetTextures()
    {

    }
}
