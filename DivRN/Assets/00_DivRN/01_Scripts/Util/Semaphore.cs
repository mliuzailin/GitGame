public class Semaphore
{
    private bool m_locked = false;
    private System.Action m_onUnlocked = null;

    public void Lock(System.Action callback)
    {
        UnityEngine.Debug.Assert(!m_locked, "This semaphore was already locked.");

        m_onUnlocked = callback;
        m_locked = true;
    }

    public void Unlock()
    {
        m_locked = false;
    }

    public void Tick()
    {
        if (m_locked)
            return;

        if (m_onUnlocked != null)
        {
            m_onUnlocked();
            m_onUnlocked = null;
        }
    }
}
