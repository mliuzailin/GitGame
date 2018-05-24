/**
 *  @file   StoryViewSelectButton.cs
 *  @brief
 *  @author Developer
 *  @date   2018/02/01
 */

using UnityEngine;
using System.Collections.Generic;
using M4u;
using DG.Tweening;

public class StoryViewSelectButton : MenuPartsBase
{
    public StoryViewSelectButtonContext Context = new StoryViewSelectButtonContext();
    StoryView.ADVselect.ADVbranch m_param;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = Context;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetParam(StoryView.ADVselect.ADVbranch param)
    {
        m_param = param;
        Context.ButtonText = m_param.text;
        if (param.wakuType == 1)
        {
            Context.ButtonType = ResourceManager.Instance.Load("answer_A", ResourceType.Menu);
        }
        else
        {
            Context.ButtonType = ResourceManager.Instance.Load("answer_B", ResourceType.Menu);
        }
        Context.IsEnableSelectButton = true;
    }

    public StoryView.ADVselect.ADVbranch GetParam()
    {
        return m_param;
    }

    public void Hide()
    {
        Context.IsEnableSelectButton = false;
    }
}
