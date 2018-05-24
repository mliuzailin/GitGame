using UnityEngine;
using System;
using System.Collections;

public class SceneMission : Scene<SceneMission>
{
    public Mission mission;

    public override void OnInitialized()
    {
        base.OnInitialized();
        mission.Initialize();

    }
    protected override void Start()
    {
        base.Start();
    }
}
