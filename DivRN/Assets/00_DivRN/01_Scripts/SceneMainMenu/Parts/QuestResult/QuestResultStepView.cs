using UnityEngine;
using System.Collections;

public class QuestResultStepView : View
{
    private static readonly string PrefaPath = "Prefab/QuestResult/QuestResultStepView";

    private QuestResultStepModel m_model = null;


    public static QuestResultStepView Attach(GameObject parent, QuestResultStepModel model)
    {
        return View.Attach<QuestResultStepView>(PrefaPath, parent).SetModel(model);
    }

    public QuestResultStepView SetModel(QuestResultStepModel model)
    {
        Debug.Assert(m_model == null, "QuestResultStepModel was set twice.");

        m_model = model;

        return this;
    }

    public void ClickNextButton()
    {
        m_model.Next();
    }
}
