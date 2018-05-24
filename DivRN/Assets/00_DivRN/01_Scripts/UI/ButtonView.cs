using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class ButtonView : View
{
    new public static T Attach<T>(string prefabPath, GameObject parent) where T : ButtonView
    {
        return View.Attach<T>(prefabPath, parent);
    }


    protected ButtonModel m_modelBase = null;

    public void SetModel<T>(T model) where T : ButtonModel
    {
        m_modelBase = model;

        m_modelBase.OnAppearingBegan += () =>
        {
            PlayAnimation(AppearAnimationName, () => { m_modelBase.FinishAppearingAnimation(); });
        };

        m_modelBase.OnAppeared += () =>
        {
            PlayAnimation(DefaultAnimationName);
        };

        m_modelBase.OnDisappearingBegan += () =>
        {
            PlayAnimation(DisappearAnimationName, () => { m_modelBase.FinishDisappearingAnimation(); });
        };

        m_modelBase.OnUpdated += () =>
        {

        };
    }

    public void Show()
    {
        m_modelBase.Appear();
    }

    public void Click()
    {
        if (!m_modelBase.isReady
            || ButtonBlocker.Instance.IsActive())
            return;

        ButtonBlocker.Instance.Block();

        PlayAnimation(ClickAnimationName,
            () =>
            {
                m_modelBase.Click();
                ButtonBlocker.Instance.Unblock();
            });
    }

    public void ClickWithoutAnimation()
    {
        m_modelBase.Click();
    }

    // ------------------------------------- animation ------------------------------
    public static readonly string AppearAnimation = "appear";
    public static readonly string DisappearAnimation = "disappear";
    public static readonly string DefaultAnimation = "loop";
    public static readonly string ClickAnimation = "click";
    protected Dictionary<string, string> m_animationNameMap = new Dictionary<string, string>
    {
        { AppearAnimation       , "button_view_appear" },
        { DisappearAnimation    , "button_view_disappear" },
        { DefaultAnimation      , "button_view_loop" },
        { ClickAnimation        , "button_view_click" }
    };


    public string AppearAnimationName
    {
        get
        {
            return m_animationNameMap[AppearAnimation];
        }
        protected set
        {
            m_animationNameMap[AppearAnimation] = value;
        }
    }

    public string DisappearAnimationName
    {
        get
        {
            return m_animationNameMap[DisappearAnimation];
        }
        protected set
        {
            m_animationNameMap[DisappearAnimation] = value;
        }
    }

    public string DefaultAnimationName
    {
        get
        {
            return m_animationNameMap[DefaultAnimation];
        }
        protected set
        {
            m_animationNameMap[DefaultAnimation] = value;
        }
    }

    public string ClickAnimationName
    {
        get
        {
            return m_animationNameMap[ClickAnimation];
        }
        protected set
        {
            m_animationNameMap[ClickAnimation] = value;
        }
    }
}
