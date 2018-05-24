/**
 * 	@file	UGUILongPress.cs
 *	@brief	UIの長押し判定
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UGUILongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent m_OnClick = new UnityEvent(); //!< ボタンクリックで呼び出すイベント
    public UnityEvent m_OnLongPress = new UnityEvent(); //!< 長押しで呼び出すイベント
    public float m_IntervalLongPress = 0.5f;//!< 長押しイベントを呼び出す間隔
    float m_PressTime = 0.0f;  //!< 次の押下判定時間

    /// <summary>
    /// 押した状態
    /// </summary>
    public bool m_Pressed
    {
        get;
        private set;
    }

    bool m_IsLongPress = false; //!< 長押ししたかどうか

    void Awake()
    {

    }

    void Update()
    {
        if (m_Pressed)
        {
            m_PressTime += Time.deltaTime;
#if BUILD_TYPE_DEBUG
            //Debug.Log("Press Time = " + m_PressTime.ToString());
#endif
            if (m_IntervalLongPress < m_PressTime)
            {
                m_Pressed = false;
                LongPress();
                m_PressTime = 0;
            }
        }
    }

    /// <summary>
    /// ボタンを押したとき
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        m_Pressed = true;
        m_IsLongPress = false;
        m_PressTime = 0;
    }

    /// <summary>
    /// ボタンを離したとき
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        m_Pressed = false;
        if (!(m_IsLongPress || eventData.dragging))
        {
            m_OnClick.Invoke();
        }
    }

    /// <summary>
    /// 長押しのイベントを送る
    /// </summary>
    void LongPress()
    {
        m_IsLongPress = true;
        m_OnLongPress.Invoke();
    }

    private void OnDisable()
    {
        m_Pressed = false;
        m_IsLongPress = false;
        m_PressTime = 0;
    }
}