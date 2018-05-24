using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIconListItem : ListItem<BossIconContext>
{
    private void Start()
    {
        SetModel(Context.model);
    }
}
