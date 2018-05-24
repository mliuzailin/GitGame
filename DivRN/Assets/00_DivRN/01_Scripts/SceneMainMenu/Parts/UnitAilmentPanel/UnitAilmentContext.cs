using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitAilmentContext : M4uContext
{
    M4uProperty<string> unit_ailment_label = new M4uProperty<string>();
    public string Unit_ailment_label { get { return unit_ailment_label.Value; } set { unit_ailment_label.Value = value; } }

    M4uProperty<string> unit_ailment_text = new M4uProperty<string>();
    public string Unit_ailment_text { get { return unit_ailment_text.Value; } set { unit_ailment_text.Value = value; } }

    public void setupAilmentInfo(string detail, int turn)
    {
        Unit_ailment_label = detail;
        if (turn <= 0)
        {
            Unit_ailment_text = "";
        }
        else
        {
            Unit_ailment_text = turn + "ターン";
        }
    }

    public void setupAbilityInfo(string detail, int turn)
    {
        Unit_ailment_label = detail;
        Unit_ailment_text = "";
    }
}
