using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRewardListItem : ListItem<ScoreRewardContext>
{
    public GameObject Offset = null;

    public void OnSelect()
    {
        Context.DidSelectItem(Context);
    }
}
