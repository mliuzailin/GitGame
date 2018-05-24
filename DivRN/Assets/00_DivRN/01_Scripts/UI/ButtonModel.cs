using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ButtonModel
{
    protected bool m_isEnabled = true;
    protected bool m_isReady = false;

    public bool isEnabled
    {
        get { return m_isEnabled; }
        set
        {
            m_isEnabled = value;

            if (OnUpdated != null)
                OnUpdated();
        }
    }
    public bool isReady
    {
        get { return m_isReady; }
    }

    public delegate void EventHandler();
    public event EventHandler OnAppearingBegan;
    public event EventHandler OnAppeared;
    public event EventHandler OnDisappearingBegan;
    public event EventHandler OnDisappeared;

    public event EventHandler OnUpdated;
    public event EventHandler OnClicked;


    public void FinishAppearingAnimation()
    {
        m_isReady = true;

        if (OnAppeared != null)
        {
            OnAppeared();
        }
    }

    public void FinishDisappearingAnimation()
    {
        if (OnDisappeared != null)
        {
            OnDisappeared();
        }
    }



    virtual public void Click()
    {
        if (!m_isReady
            || !m_isEnabled)
        {
            return;
        }

        if (OnClicked != null)
        {
            OnClicked();
        }
    }

    virtual public void Appear()
    {
        if (m_isReady)
        {
            return;
        }

        if (OnAppearingBegan != null)
        {
            OnAppearingBegan();
        }
    }

    virtual public void SkipAppearing()
    {
        if (m_isReady)
        {
            return;
        }

        FinishAppearingAnimation();
    }

    virtual public void Close()
    {
        m_isReady = false;

        if (OnDisappearingBegan != null)
        {
            OnDisappearingBegan();
        }
    }

    public bool isAppearingBegan()
    {
        if (OnAppearingBegan != null)
        {
            return true;
        }


        return false;
    }
}
