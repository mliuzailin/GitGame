/**
 *  @file   UGUIHoldPress.cs
 *  @brief  ボタンの押しっぱなしで呼ばれ続ける
 *  @author Developer
 *  @date   2017/05/03
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UGUIHoldPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent m_OnClick = new UnityEvent(); //!< ボタンクリックで呼び出すイベント
    public UnityEvent m_OnHoldPress = new UnityEvent(); //!< 長押しで呼び出すイベント
    public float m_IntervalHoldPress = 0.1f;//!< 長押しイベントを呼び出す間隔
    float m_PressTime = 0.0f;  //!< 次の押下判定時間

    /// <summary>
    /// 押した状態
    /// </summary>
    public bool m_Pressed
    {
        get;
        private set;
    }

    void Awake()
    {

    }

    void Update()
    {
        if (m_Pressed)
        {
            m_PressTime += Time.deltaTime;
            if (m_IntervalHoldPress < m_PressTime)
            {
                HoldPress();
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
        m_PressTime = 0;
    }

    /// <summary>
    /// ボタンを離したとき
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        m_Pressed = false;
        if (!eventData.dragging)
        {
            m_OnClick.Invoke();
        }
    }

    /// <summary>
    /// 長押しのイベントを送る
    /// </summary>
    void HoldPress()
    {
        m_OnHoldPress.Invoke();
    }
}
