using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TipsModel
{
    public delegate void EventHandler();
    public event EventHandler OnUpdated;

    private List<TipSet> m_tips = new List<TipSet>();
    private int m_currentTipsIndex = 0;

    public TipsModel AddRandomTips(int count = 1)
    {
        for (int i = 0; i < count; i++)
            m_tips.Add(new TipSet());

        return this;
    }

    public void Next()
    {
        Debug.Assert(m_tips.Count > 0, "The TipSet must set.");

        m_currentTipsIndex++;
        while (m_currentTipsIndex >= m_tips.Count)
            m_currentTipsIndex -= m_tips.Count;

        if (OnUpdated != null)
            OnUpdated();
    }

    // ============================= accessor
    public TipSet tipSet
    {
        get
        {
            Debug.Assert(m_tips.Count > 0, "The TipSet must set.");

            return m_tips[m_currentTipsIndex].AsReadOnly();
        }
    }
}
