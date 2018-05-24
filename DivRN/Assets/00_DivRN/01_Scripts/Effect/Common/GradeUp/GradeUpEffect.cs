using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GradeUpEffect : View
{
    private static readonly string PrefabPath = "Prefab/Effect/Common/GradeUp/GradeUpEffect";
    private static readonly string AppearAnimationName = "grade_up_effect";
    private static readonly string DefaultAnimationName = "grade_up_effect_loop";

    private bool m_isFinished = false;

    public static GradeUpEffect Attach(GameObject parent)
    {
        return View.Attach<GradeUpEffect>(PrefabPath, parent);
    }

    public GradeUpEffect Show(System.Action callback = null)
    {
        m_isFinished = false;

        PlayAnimation(AppearAnimationName,
            () =>
            {
                m_isFinished = true;

                if (callback != null)
                    callback();
            });

        return this;
    }

    public void SkipShowAnimation()
    {
        if (m_isFinished)
            return;

        m_isFinished = true;
        PlayAnimation(DefaultAnimationName);
    }

    public bool isFinished { get { return m_isFinished; } }
}
