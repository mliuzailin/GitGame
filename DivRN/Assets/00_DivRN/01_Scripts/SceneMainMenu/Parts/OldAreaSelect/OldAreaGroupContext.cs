using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class OldAreaGroupContext : M4uContext
{
    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<List<OldAreaContext>> oldAreaList = new M4uProperty<List<OldAreaContext>>();
    public List<OldAreaContext> OldAreaList { get { return oldAreaList.Value; } set { oldAreaList.Value = value; } }

}
