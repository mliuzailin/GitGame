using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitResultBase : MenuPartsBase
{
    protected bool m_Ready = false;
    public bool Ready { get { return m_Ready; } }
    protected System.Action m_FinishCallAction = delegate { };

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    protected IEnumerator DelayExec(System.Action _action, float _delay)
    {
        yield return new WaitForSeconds(_delay);

        _action();
    }
}
