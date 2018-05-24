using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class ListItemView : ButtonView
{
    public static readonly string LongPressAnimation = "long_press";
    public static readonly string LongPressAnimationName = "button_view_click"; // TODO : 専用の演出にするならそのアニメーション名に変える

    protected ListItemModel m_listItemModel = null;

    new public static T Attach<T>(string prefabPath, GameObject parent) where T : ButtonView
    {
        return ButtonView.Attach<T>(prefabPath, parent);
    }


    void Awake()
    {
        m_animationNameMap[LongPressAnimation] = LongPressAnimationName;
    }

    public void SetModel(ListItemModel model)
    {
        m_listItemModel = model;

        base.SetModel<ListItemModel>(m_listItemModel);
    }

    new public void Click()
    {
        base.Click();
    }
    public void LongPress()
    {
        if (!m_modelBase.isReady)
            return;

        PlayAnimation(ClickAnimationName, () => { m_listItemModel.LongPress(); });
    }
}
