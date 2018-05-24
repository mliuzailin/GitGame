using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListSelectButton : ButtonView
{
    private static readonly string SelectButtonPrefabPath = "Prefab/FriendList/FriendListSelectButton";

    [SerializeField]
    Button SelectButton;

    public static FriendListSelectButton Attach(GameObject parent)
    {
        return ButtonView.Attach<FriendListSelectButton>(SelectButtonPrefabPath, parent);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetButtonTarget(Image targetImage)
    {
        if (SelectButton != null)
        {
            SelectButton.targetGraphic = targetImage;
        }
    }
}
