using UnityEngine;
using System;
using System.Collections;

public class SceneEventSchedule : Scene<SceneEventSchedule>
{
	public EventSchedule Bind;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnInitialized()
    {
        base.OnInitialized();

        Bind.PostSceneStart();
    }
}
