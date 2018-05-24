using UnityEngine;
using System;
using System.Collections;

public class SceneBeginnerBoost : Scene<BeginnerBoost>
{
    public BeginnerBoost Bind;

    public override void OnInitialized()
    {
        // シーンの最後に呼び出す
        Bind.PostSceneStart();
    }
}
