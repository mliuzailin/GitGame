using UnityEngine;
using System;
using System.Collections;

public class ScenePresent : Scene<ScenePresent>
{
	public Present Bind;

    protected override void Start()
    {
        base.Start();

        // データの更新
        Bind.initPresent();

        // シーンの最後に呼び出す
        //Bind.PostSceneStart();
    }
}
