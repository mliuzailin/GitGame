using UnityEngine;
using System.Collections;

public class MainMenuQuestCollabo : MainMenuSeq
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }
}
