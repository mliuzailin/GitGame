/**
 *  @file   HeroStoryListItemContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/20
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class HeroStoryListItemContext : M4uContext
{
    public HeroStoryListItemContext(ListItemModel listItemModel)
    {
        m_model = listItemModel;
    }
    private ListItemModel m_model;
    public ListItemModel model { get { return m_model; } }


    M4uProperty<string> buttonText = new M4uProperty<string>();
    /// <summary>ボタンの文字</summary>
    public string ButtonText
    {
        get
        {
            return buttonText.Value;
        }
        set
        {
            buttonText.Value = value;
        }
    }

    M4uProperty<string> storyTitle = new M4uProperty<string>();
    /// <summary>タイトル</summary>
    public string StoryTitle
    {
        get
        {
            return storyTitle.Value;
        }
        set
        {
            storyTitle.Value = value;
        }
    }

    M4uProperty<string> contentText = new M4uProperty<string>();
    /// <summary>内容</summary>
    public string ContentText
    {
        get
        {
            return contentText.Value;
        }
        set
        {
            contentText.Value = value;
        }
    }

    M4uProperty<bool> isOpenStory = new M4uProperty<bool>();
    /// <summary>ストーリーが開放されているかどうか</summary>
    public bool IsOpenStory
    {
        get
        {
            return isOpenStory.Value;
        }
        set
        {
            isOpenStory.Value = value;
        }
    }

}
