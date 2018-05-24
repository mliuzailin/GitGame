using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ChallengeSkipContext : M4uContext
{
    M4uProperty<string> level = new M4uProperty<string>();
    public string Level { get { return level.Value; } set { level.Value = value; } }

    public System.Action<ChallengeSkipContext> DidSelectItem = delegate { };

    private int m_SkipLevel = 0;
    public int SkipLevel { get { return m_SkipLevel; } }

    public void SetData(int skipLevel)
    {
        m_SkipLevel = skipLevel;
    }

    public void Copy(ChallengeSkipContext context)
    {
        m_SkipLevel = context.SkipLevel;

    }

    public void setup()
    {
        Level = string.Format(GameTextUtil.GetText("growth_boss_31"), m_SkipLevel);
    }
}
