using UnityEngine;
using System;
using System.Collections;

public class SceneLoginBonus : Scene<LoginBonus>
{
    public LoginBonus Bind;

    public override void OnInitialized()
    {
        base.OnInitialized();

        // シーンの最後に呼び出す
        Bind.PostSceneStart();
    }
}
