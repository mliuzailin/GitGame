using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class QuestDetailMessage : MenuPartsBase
{
    private QuestDetailModel m_model = null;
    public void SetModel(QuestDetailModel model)
    {
        m_model = model;
        SetUpLayout();
    }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<string> message = new M4uProperty<string>();
    public string Message { get { return message.Value; } set { message.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    private void Start()
    {
        if (SafeAreaControl.Instance)
        {
            SafeAreaControl.Instance.addLocalYPos(transform);
        }
    }

    private void SetUpLayout()
    {
        Debug.Assert(m_model != null, "model not set.");
        var rectTransform = GetComponent<RectTransform>();
        Debug.Assert(rectTransform != null, "RectTransform not set.");
        m_model.SetMessagePanelHeight(rectTransform.sizeDelta.y);
    }
}
