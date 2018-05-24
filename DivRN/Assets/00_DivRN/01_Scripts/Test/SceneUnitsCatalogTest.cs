using UnityEngine;
using System.Collections;

public class SceneUnitsCatalogTest : Scene<SceneUnitsCatalogTest>
{
    public UnitsCatalog Bind;

    protected override void Start()
    {
        base.Start();

        Bind.TitleText = "TITILE";
    }
}
