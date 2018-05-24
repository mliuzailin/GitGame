using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScenePointShop : Scene<ScenePointShop>
{
	public PointShop Bind;

    public override void OnInitialized()
    {
        base.OnInitialized();
        Bind.SceneStart();
    }

    protected override void Start()
    {
        base.Start();
    }
}
