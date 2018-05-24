using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ListItemModel : ButtonModel
{
    public delegate void PositionEventhandler(Vector2 position);

    public event PositionEventhandler OnPositionUpdated;

    public event EventHandler OnLongPressed;
    public event EventHandler OnStarted;


    private bool m_isStarted = false;
    public bool isStarted
    {
        get
        {
            return m_isStarted;
        }
    }
    public void Start()
    {
        m_isStarted = true;

        if (OnStarted != null)
        {
            OnStarted();
        }
    }

    public ListItemModel(uint index)
    {
        m_index = index;
    }

    protected uint m_index = 0;

    public uint index { get { return m_index; } }

    public void setIndex(uint _index)
    {
        m_index = _index;
    }

    public void LongPress()
    {
        if (!m_isReady ||
            !m_isEnabled)
        {
            return;
        }

        if (OnLongPressed != null)
        {
            OnLongPressed();
        }
    }

    private Vector2 m_position = Vector2.zero;
    public void SetPosition(Vector2 position)
    {
        m_position = position;
#if BUILD_TYPE_DEBUG
        //Debug.Log(System.String.Format("pos:S {0} / x:{1} / y:{2}", m_index, m_position.x, m_position.y));
#endif
        if (OnPositionUpdated != null)
        {
            OnPositionUpdated(m_position);
        }
    }

    public void MoveBy(Vector2 delta)
    {
        m_position += delta;
#if BUILD_TYPE_DEBUG
        //Debug.Log(System.String.Format("pos:M {0} / x:{1} / y:{2}", m_index, m_position.x, m_position.y));
#endif
        if (OnPositionUpdated != null)
        {
            OnPositionUpdated(m_position);
        }
    }
}
