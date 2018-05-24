using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkillListItemAtParty : ListItem<UnitSkillAtPartyContext>
{
    void Awake()
    {
        AppearAnimationName = "unit_skill_list_item_appear";
        DefaultAnimationName = "unit_skill_list_item_default";
    }

    void Start()
    {
        SetModel(Context.model);
    }
}
