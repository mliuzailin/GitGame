using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;


public class MainMenuFooterModel
{
    public delegate void EventHandler();
    public event EventHandler OnAppeared;
    public event EventHandler OnDisappearingBegan;
    public event EventHandler OnDisappeared;

    private bool m_isAppeared = false;


    public void FinishAppearingAnimation()
    {
        m_isAppeared = true;

        if (OnAppeared != null)
            OnAppeared();
    }

    public void FinishDisappearingAnimation()
    {
        if (OnDisappeared != null)
            OnDisappeared();
    }

    public void SkipAppearing()
    {
        if (m_isAppeared)
            return;

        FinishAppearingAnimation();
    }


    public void Close()
    {
        m_isAppeared = false;

        if (OnDisappearingBegan != null)
            OnDisappearingBegan();
    }



    public bool isAppeared
    {
        get { return m_isAppeared; }
    }
}
