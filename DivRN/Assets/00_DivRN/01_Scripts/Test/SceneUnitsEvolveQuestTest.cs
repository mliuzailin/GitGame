using UnityEngine;
using System.Collections;

public class SceneUnitsEvolveQuestTest : Scene<SceneUnitsEvolveQuestTest>
{
    public UnitsEvolveQuest Bind;

    protected override void Start()
    {
        base.Start();

        Bind.TitleText = "TITILE";
    }
}
