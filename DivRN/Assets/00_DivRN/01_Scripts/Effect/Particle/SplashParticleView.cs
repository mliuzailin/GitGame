using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class SplashParticleView : View
{
    private static readonly string PrefabPath = "Prefab/Effect/Particle/SplashParticle";

    public static SplashParticleView Attach(GameObject parent)
    {
        return View.Attach<SplashParticleView>(PrefabPath, parent);
    }
}
