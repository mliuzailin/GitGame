using UnityEngine;
using System.Collections;

public class DropIconListItem : ListItem<DropIconContex>
{
    void Awake()
    {
        AppearAnimationName = "drop_icon_appear";
        DefaultAnimationName = "drop_icon_loop";
    }

    void Start()
    {
        var model = Context.model;
        SetModel(model);

        model.OnLoopStarted += () =>
        {
            PlayAnimation(DefaultAnimationName);
        };

        model.OnDisappeared += () =>
        {
            GetRoot().SetActive(false);
        };

        RegisterKeyEventCallback("next", () =>
        {
            model.ShowNext();
        });

        RegisterKeyEventCallback("FinishAnimation", () =>
        {
            Debug.Log("");
        });

        Context.m_GettingUnit = GetComponentInChildren<AnimationClipGettingUnit>();
        Context.m_bReady = true;
    }

    void Update()
    {
    }
}
