using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class OldAreaSelect : MenuPartsBase
{
    M4uProperty<List<OldAreaGroupContext>> oldAreaGroupList = new M4uProperty<List<OldAreaGroupContext>>();
    public List<OldAreaGroupContext> OldAreaGroupList { get { return oldAreaGroupList.Value; } set { oldAreaGroupList.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        OldAreaGroupList = new List<OldAreaGroupContext>();
    }
}
