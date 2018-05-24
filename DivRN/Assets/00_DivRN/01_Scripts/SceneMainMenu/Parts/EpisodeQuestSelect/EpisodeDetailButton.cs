using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodeDetailButton : ButtonView
{

    static readonly string PrefabPath = "Prefab/EpisodeQuestSelect/EpisodeDetailButton";

    public static EpisodeDetailButton Attach(GameObject parent)
    {
        return View.Attach<EpisodeDetailButton>(PrefabPath, parent);
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
