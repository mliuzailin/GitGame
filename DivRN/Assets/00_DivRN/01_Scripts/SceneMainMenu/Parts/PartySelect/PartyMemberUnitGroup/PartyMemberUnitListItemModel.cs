using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PartyMemberUnitListItemModel : ListItemModel
{
    public event EventHandler OnShowedNext;
    public event EventHandler OnShowedStatus;

    public PartyMemberUnitListItemModel(uint index) : base(index)
    {

    }

    public void ShowNext()
    {
        if (OnShowedNext != null)
            OnShowedNext();
    }

    public void ShowStatus()
    {
        if (OnShowedStatus != null)
            OnShowedStatus();
    }
}
