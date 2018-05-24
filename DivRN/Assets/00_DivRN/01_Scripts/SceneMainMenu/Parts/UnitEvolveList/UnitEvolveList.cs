using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitEvolveList : MenuPartsBase
{
    private M4uProperty<List<UnitEvolveContext>> evolveList = new M4uProperty<List<UnitEvolveContext>>(new List<UnitEvolveContext>());
    public List<UnitEvolveContext> EvolveList { get { return evolveList.Value; } set { evolveList.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }
}
