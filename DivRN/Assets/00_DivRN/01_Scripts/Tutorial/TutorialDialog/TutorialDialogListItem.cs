/**
 *  @file   TutorialDialogListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/01
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TutorialDialogListItem : ListItem<TutorialDialogListContext>
{
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
        TutorialDialog carousel = GetComponentInParent<TutorialDialog>();
        if (carousel != null)
        {
            Context.Toggle.onValueChanged.AddListener(carousel.OnChangedCarousel);
        }
    }
}
