using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEventListItem : ListItem<ScoreEventContext>
{
    public void OnSelect()
    {
        Context.DidSelectItem(Context);
    }
}
