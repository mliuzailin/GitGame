using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HiScoreEffect : View
{
    private static readonly string PrefabPath = "Prefab/Effect/Common/HiScore/HiScoreEffect";
    private static readonly string AppearAnimationName = "hi_score_effect";
    private static readonly string DefaultAnimationName = "hi_score_effect_loop";

    private bool m_isFinished = false;

    public static HiScoreEffect Attach(GameObject parent)
    {
        return View.Attach<HiScoreEffect>(PrefabPath, parent);
    }

    public HiScoreEffect Show(System.Action callback = null)
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
