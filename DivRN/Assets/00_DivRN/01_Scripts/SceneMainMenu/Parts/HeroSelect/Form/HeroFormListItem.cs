/**
 *  @file   HeroFormListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/24
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class HeroFormListItem : ListItem<HeroFormListContext>
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
        HeroForm heroForm = GetComponentInParent<HeroForm>();
        if (heroForm != null)
        {
            Context.Toggle.onValueChanged.AddListener(heroForm.OnChangedForm);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 顔写真が押された
    /// </summary>
    public void OnClickFaceImage()
    {
        if (Context.OnClickFaceImageAction != null)
        {
            Context.OnClickFaceImageAction(Context);
        }
    }


}
