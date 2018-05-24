/**
 *  @file   SubTabGroupListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/07/01
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SubTabGroupListItem : ListItem<SubTabGroupContext>
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
        MainMenuSubTab mainMenuSubTab = GetComponentInParent<MainMenuSubTab>();
        if (mainMenuSubTab != null)
        {
            Context.Toggle.onValueChanged.AddListener(mainMenuSubTab.OnChangedGroup);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
