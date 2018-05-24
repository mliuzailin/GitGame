using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeSkipListItem : ListItem<ChallengeSkipContext>
{
    public int itemIndex = 0;
    public RectTransform Offset = null;
    public void OnSelect()
    {
        Context.DidSelectItem(Context);
    }
}
