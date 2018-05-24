using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListIconButton : ButtonView
{
    private static readonly string IconButtonPrefabPath = "Prefab/FriendList/FriendListIconButton";

    public Image IconImage;
    public Image ElementImage;
    public Image BGImage;

    public static FriendListIconButton Attach(GameObject parent)
    {
        return ButtonView.Attach<FriendListIconButton>(IconButtonPrefabPath, parent);
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
