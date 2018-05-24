using System.Collections;
using System.Collections.Generic;


public class EffectProcessor
{
    private static EffectProcessor _instance = null;
    public static EffectProcessor Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EffectProcessor();
            return _instance;
        }
    }
    private EffectProcessor() { }

    public void TearDown()
    {
        _instance = null;
    }

    private Dictionary<string, System.Action<System.Action>> m_effects = new Dictionary<string, System.Action<System.Action>>();
    private bool m_isProcessing = false;

    public void Register(string tag, System.Action<System.Action> playEffect)
    {
        m_effects[tag] = playEffect;
    }

    public void PlayOrderly(List<string> orderedTags)
    {
        if (m_isProcessing
            || m_effects.Count == 0)
            return;

        foreach (var tag in orderedTags)
            if (!m_effects.ContainsKey(tag))
                return;

        m_isProcessing = true;

        var process = new SerialProcess();

        foreach (var tag in orderedTags)
            process.Add(m_effects[tag]);

        process.Add((System.Action finish) =>
        {
            m_isProcessing = false;
            Clear();
        });

        process.Flush();
    }

    public void Clear()
    {
        if (m_isProcessing)
            return;

        m_effects.Clear();
    }
}
