public class WaitTimer
{
    private float m_elaspedTime = 0;
    private float m_limitTime = 0;

    private System.Action m_onFinished;

    public WaitTimer(float limitTime, System.Action callback)
    {
        m_limitTime = limitTime;
        m_onFinished = callback;
    }

    public void Tick(float delta)
    {
        if (m_elaspedTime >= m_limitTime)
            return;

        m_elaspedTime += delta;
        if (m_elaspedTime >= m_limitTime)
            if (m_onFinished != null)
                m_onFinished();
    }
}
