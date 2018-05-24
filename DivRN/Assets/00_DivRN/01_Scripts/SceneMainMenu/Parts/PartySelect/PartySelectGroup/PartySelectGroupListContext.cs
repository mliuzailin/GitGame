/**
 *  @file   PartySelectGroupListContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/12
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class PartySelectGroupListContext : M4uContext
{
    public Toggle Toggle = null;

    M4uProperty<List<PartySelectGroupUnitContext>> parties = new M4uProperty<List<PartySelectGroupUnitContext>>(new List<PartySelectGroupUnitContext>());
    public List<PartySelectGroupUnitContext> Parties
    {
        get
        {
            return parties.Value;
        }
        set
        {
            parties.Value = value;
        }
    }

	List<GameObject> partyList = new List<GameObject>();
	public List<GameObject> PartyList { get { return partyList; } set { partyList = value; } }
}
