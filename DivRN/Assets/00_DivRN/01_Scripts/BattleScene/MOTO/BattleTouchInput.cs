using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトル用のタッチ入力
/// </summary>
public class BattleTouchInput : SingletonComponent<BattleTouchInput>
{
    private Camera m_Camera = null;

    private bool m_IsTouching;
    private bool m_IsTouchTriger;
    private bool m_IsTouchRelease;
    private float m_TouchingTime;
    private Vector2 m_CurrentPosition = new Vector2();
    private Vector2 m_TouchStartPosition = new Vector2();
    private Vector2 m_OldPosition = new Vector2();
    private float m_DeltaTime;
    private bool m_IsWorking = false;
    private bool m_IsDrag = false;

    // タッチ入力乗っ取り関連
    private int m_IsOverrideTouchMode_Counter = 0;     // 乗っ取り情報：乗っ取り中かどうか
    private int m_OverrideTouchMode_HandAreaTouchIndex = -1;  // 乗っ取り情報：コリジョン判定
    private int m_OverrideTouchMode_FieldAreaTouchIndex = -1; // 乗っ取り情報：コリジョン判定

    private bool m_DeviceTouching;  // 乗っ取り中でも入力デバイス素のままのタッチ情報
    private bool m_DeviceTouchTriger;  // 乗っ取り中でも入力デバイス素のままのタッチ情報
    private bool m_DeviceTouchRelease;  // 乗っ取り中でも入力デバイス素のままのタッチ情報

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        m_IsTouching = false;
        m_IsTouchTriger = false;
        m_IsTouchRelease = false;
        m_TouchingTime = 0.0f;
        m_IsDrag = false;
    }

    public void Update()
    {
        bool input_touch = false;
        Vector2 input_position = m_CurrentPosition;

        if (Input.touchSupported)
        {
            if (Input.touchCount > 0)
            {
                input_touch = true;
                input_position = Input.GetTouch(0).position;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                input_touch = true;
                input_position = Input.mousePosition;
            }
        }

        {
            bool is_touching = false;
            if (m_IsWorking)
            {
                is_touching = input_touch;
            }
            m_DeviceTouchTriger = is_touching && !m_DeviceTouching;
            m_DeviceTouchRelease = !is_touching && m_DeviceTouching;
            m_DeviceTouching = is_touching;
        }

        if (m_IsOverrideTouchMode_Counter > 0)
        {
            m_IsOverrideTouchMode_Counter--;

            // タッチ操作の乗っ取り中はタッチを無視
            return;
        }

        _updateTouchInfo(input_touch, input_position);
    }

    private void _updateTouchInfo(bool input_touch, Vector2 input_position)
    {
        // 画面外をタッチしたらタッチしていないことにする
        //  タッチパネルではなくマウスを使用するプラットフォームへの対策。
        //  アスペクト比によっては出現する左右の黒帯上はタッチパネルでもタッチできないようにする。
        {
            int screen_left;
            int screen_right;
            if (Screen.width > Screen.height * BattleSceneUtil.MAX_ASPECT)
            {
                // 左右に黒帯が出るアスペクト比の場合
                int screen_width = (int)(Screen.height * BattleSceneUtil.MAX_ASPECT);
                screen_left = (Screen.width - screen_width) / 2;
                screen_right = (Screen.width + screen_width) / 2;
            }
            else
            {
                screen_left = 0;
                screen_right = Screen.width;
            }

            if (input_position.x < screen_left)
            {
                input_position.x = screen_left;
                input_touch = false;
            }
            else if (input_position.x > screen_right - 1)
            {
                input_position.x = screen_right - 1;
                input_touch = false;
            }

            if (input_position.y < 0)
            {
                input_position.y = 0;
                input_touch = false;
            }
            else if (input_position.y > Screen.height - 1)
            {
                input_position.y = Screen.height - 1;
                input_touch = false;
            }
        }

        bool is_touching = false;
        if (m_IsWorking)
        {
            is_touching = input_touch;
        }

        m_IsTouchTriger = is_touching && !m_IsTouching;
        m_IsTouchRelease = !is_touching && m_IsTouching;
        m_IsTouching = is_touching;

        if (m_IsTouching)
        {
            m_DeltaTime = Time.deltaTime;

            m_OldPosition.x = m_CurrentPosition.x;
            m_OldPosition.y = m_CurrentPosition.y;
            m_CurrentPosition.x = input_position.x;
            m_CurrentPosition.y = input_position.y;

            if (m_IsTouchTriger)
            {
                m_TouchingTime = m_DeltaTime;

                m_TouchStartPosition.x = m_CurrentPosition.x;
                m_TouchStartPosition.y = m_CurrentPosition.y;

                m_IsDrag = false;
            }
            else
            {
                m_TouchingTime += m_DeltaTime;

                if (m_IsDrag == false)
                {
                    Vector2 delta = m_CurrentPosition - m_TouchStartPosition;
                    if (delta.sqrMagnitude > 20.0f * 20.0f)
                    {
                        m_IsDrag = true;
                    }
                }
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }


    public void setWorking(bool is_working)
    {
        if (m_IsWorking != is_working)
        {
            m_IsWorking = is_working;

            m_IsTouching = false;
            m_IsTouchTriger = false;
            m_IsTouchRelease = false;
            m_TouchingTime = 0.0f;
            m_IsDrag = false;
        }
    }

    public bool isWorking()
    {
        return m_IsWorking;
    }

    public bool isTouching()
    {
        return m_IsTouching;
    }

    public bool isTouchTriger()
    {
        return m_IsTouchTriger;
    }

    public bool isTouchRelease()
    {
        return m_IsTouchRelease;
    }

    public Vector2 getPosition()
    {
        return m_CurrentPosition;
    }

    public bool isDrag()
    {
        return m_IsDrag;
    }

    public float getTouchingTime()
    {
        return m_TouchingTime;
    }

    public bool isTapped()
    {
        if (m_IsTouchRelease && m_IsDrag == false && m_TouchingTime < 0.5f)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// マウスカーソルのワールド座標を取得（カメラからの奥行z地点での）
    /// </summary>
    /// <param name="z"></param>
    /// <returns></returns>
    public Vector3 getWorldPosition(float z)
    {
        Vector3 wrk_position = new Vector3(m_CurrentPosition.x, m_CurrentPosition.y, z);
        Vector3 world_position = m_Camera.ScreenToWorldPoint(wrk_position);
        return world_position;
    }

    /// <summary>
    /// スクリーン座標（キャンバス座標ではない）をワールド座標へ変換する
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 getWorldPosition(Vector3 position)
    {
        Vector3 world_position = m_Camera.ScreenToWorldPoint(position);
        return world_position;
    }

    /// <summary>
    /// ワールド座標をスクリーン座標（キャンバス座標ではない）へ変換する
    /// ※キャンバス座標にするには scrrenPosition / Canvas.scaleFactor
    /// </summary>
    /// <param name="world_position"></param>
    /// <returns></returns>
    public Vector3 getScreenPosition(Vector3 world_position)
    {
        Vector3 screen_position = m_Camera.WorldToScreenPoint(world_position);
        return screen_position;
    }

    public void setCamera(Camera camera)
    {
        m_Camera = camera;
    }

    public Camera getCamera()
    {
        return m_Camera;
    }

    /// <summary>
    /// タッチ入力を乗っ取る
    /// </summary>
    /// <param name="touch_position"></param>
    /// <param name="is_touch"></param>
    public void setOverrideTouchMode(Vector2 touch_position, bool is_touch, int hand_area_touch_index, int field_area_touch_index)
    {
        const int COUNTER_START = 2;

        if (m_IsOverrideTouchMode_Counter == COUNTER_START)
        {
            // 同一フレーム内では２回目以降は処理しない
            return;
        }

        if (m_IsOverrideTouchMode_Counter == 0)
        {
            // ユーザーによるタッチ入力を消去
            if (m_IsTouching)
            {
                m_IsTouchTriger = false;
                m_IsTouchRelease = true;
                m_IsTouching = false;
            }
        }

        m_IsOverrideTouchMode_Counter = COUNTER_START;

        m_OverrideTouchMode_HandAreaTouchIndex = hand_area_touch_index;
        m_OverrideTouchMode_FieldAreaTouchIndex = field_area_touch_index;

        _updateTouchInfo(is_touch, touch_position);
    }

    public bool isOverrideTouchMode()
    {
        bool ret_val = (m_IsOverrideTouchMode_Counter > 0);
        return ret_val;
    }

    public int getOverrideTouchMode_HandAreaTouchIndex()
    {
        return m_OverrideTouchMode_HandAreaTouchIndex;
    }

    public int getOverrideTouchMode_FieldAreaTouchIndex()
    {
        return m_OverrideTouchMode_FieldAreaTouchIndex;
    }

    public bool isDeviceTouching()
    {
        return m_DeviceTouching;
    }

    public bool isDeviceTouchTriger()
    {
        return m_DeviceTouchTriger;
    }

    public bool isDeviceTouchRelease()
    {
        return m_DeviceTouchRelease;
    }
}
