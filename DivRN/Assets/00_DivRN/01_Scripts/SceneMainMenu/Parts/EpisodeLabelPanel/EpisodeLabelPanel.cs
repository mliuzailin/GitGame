/**
 *  @file   EpisodeLabelPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/14
 */

using UnityEngine;
using System.Collections;
using M4u;

public class EpisodeLabelPanel : MenuPartsBase
{

    M4uProperty<string> episodeText = new M4uProperty<string>();
    /// <summary></summary>
    public string EpisodeText
    {
        get
        {
            return episodeText.Value;
        }
        set
        {
            episodeText.Value = value;
        }
    }

    M4uProperty<string> storyText = new M4uProperty<string>();
    public string StoryText
    {
        get
        {
            return storyText.Value;
        }
        set
        {
            storyText.Value = value;
        }
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
