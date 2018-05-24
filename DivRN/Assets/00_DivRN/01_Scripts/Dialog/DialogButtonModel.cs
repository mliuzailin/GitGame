using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DialogButtonModel : ButtonModel
{
    new public event ButtonModel.EventHandler OnUpdated;

    public DialogButtonModel()
    {
        base.OnUpdated += () =>
        {
            if (OnUpdated != null)
                OnUpdated();
        };
    }

    private string m_text = "";
    public string text
    {
        get { return m_text; }
        set
        {
            m_text = value;
            if (OnUpdated != null)
                OnUpdated();
        }
    }

    private bool m_switch = true;
    public bool _switch
    {
        get { return m_switch; }
        set
        {
            m_switch = value;
            if (OnUpdated != null)
                OnUpdated();
        }
    }
}
