using UnityEngine;

public class PartySelectGroupUnitListItemNameView : PartySelectGroupUnitListItemPartsView
{
    void Awake()
    {
        AppearAnimationName = "party_select_name_appear";
        DefaultAnimationName = "party_select_name_loop";
        SelectAnimationName = "party_select_name_select";
    }
}
