/**
 *  @file   PartySelectGroupListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/12
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using M4u;

public class PartySelectGroupListItem : ListItem<PartySelectGroupListContext>
{

    // Use this for initialization
    void Start()
    {
        // ページ切り替え用トグルの設定
        Context.Toggle = GetComponent<Toggle>();
        ToggleGroup toggleGroup = GetComponentInParent<ToggleGroup>();
        if (toggleGroup != null)
        {
            Context.Toggle.group = toggleGroup;
        }

        // コールバック設定
        PartySelectGroup partySelectGroup = GetComponentInParent<PartySelectGroup>();
        if (partySelectGroup != null)
        {
            Context.Toggle.onValueChanged.AddListener(partySelectGroup.OnChangedPartGroup);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
