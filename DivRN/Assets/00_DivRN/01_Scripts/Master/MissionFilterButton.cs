using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionFilterButton : ButtonView
{

    static readonly string PrefabPath = "Prefab/Mission/MissionFilterButton";

    public static MissionFilterButton Attach(GameObject parent)
    {
        return View.Attach<MissionFilterButton>(PrefabPath, parent);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
