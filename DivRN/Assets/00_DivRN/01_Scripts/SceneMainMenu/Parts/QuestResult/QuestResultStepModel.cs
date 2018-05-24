using UnityEngine;
using System.Collections;

public class QuestResultStepModel
{
    public delegate void EventHandler();
    public event EventHandler OnStepChanged;
    public event EventHandler OnStepStarted;
    public event EventHandler OnStepEnded;

    private bool m_isProgressing = false;
    public bool IsProgressing { get { return m_isProgressing; } }

    public void Start()
    {
        m_isProgressing = true;

        if (OnStepStarted != null)
            OnStepStarted();
    }


    public void Next()
    {
        if (m_isProgressing)
            return;

        m_isProgressing = true;

        if (OnStepChanged != null)
            OnStepChanged();
    }

    public void End()
    {
        if (!m_isProgressing)
            return;

        m_isProgressing = false;

        if (OnStepEnded != null)
            OnStepEnded();
    }

    public void ForceNext()
    {
        m_isProgressing = false;

        if (OnStepEnded != null)
            OnStepEnded();

        Next();
    }
}
