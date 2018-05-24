using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDataListItem : ListItem<QuestDataContext>
{
    public AssetAutoSetEpisodeBackgroundTexture assetAutoSetEpisodeBackgroundTexture;

    void Awake()
    {
        AppearAnimationName = "quest_select_button_appear";
        DefaultAnimationName = "quest_select_button_loop";
    }

    void Start()
    {
        SetModel(Context.model);
    }

    public AssetBundler CreateBg()
    {
        return assetAutoSetEpisodeBackgroundTexture.Create(Context.area_category_id);
    }

    public void OnSelectQuest()
    {
        base.Click();
    }
}
