using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using M4u;

public class InputLayer : SingletonComponent<InputLayer>
{
    [SerializeField]
    private Camera m_camera = null;
    public Camera layerCamera { get { return m_camera; } }
    private static InputLayer m_instance = null;
    private InputLayer() { }

    public static readonly string PrefabPath = "Prefab/Input/InputLayer";

    public static InputLayer Attach()
    {
        if (m_instance != null)
            return m_instance;

        var prefab = Resources.Load<GameObject>(PrefabPath);
        var entity = Instantiate<GameObject>(prefab);
        Debug.Assert(entity != null, "The prefab " + PrefabPath + " not found.");
        m_instance = entity.GetComponent<InputLayer>();
        Debug.Assert(m_instance != null, "The component InputLayer not found.");

        return m_instance.Initialize();
    }


    private System.Action<Vector3> _onAnyTouchBegan = delegate { };
    private System.Action<Vector3> _onAnyTouchFinished = delegate { };

    private Queue<System.Action<Vector3>> _onAnyTouchBeganCallbackOnce = new Queue<System.Action<Vector3>>();
    public System.Action<Vector3> OnAnyTouchBeganCallbackOnce
    {
        set
        {
            _onAnyTouchBeganCallbackOnce.Enqueue(value);
        }
    }

    private Queue<System.Action<Vector3>> _onAnyTouchFinishedCallbackOnce = new Queue<System.Action<Vector3>>();
    public System.Action<Vector3> OnAnyTouchFinishedCallbackOnce
    {
        set
        {
            _onAnyTouchFinishedCallbackOnce.Enqueue(value);
        }
    }

    public void CancelAnyTouchCallback()
    {
        _onAnyTouchBeganCallbackOnce.Clear();
        _onAnyTouchFinishedCallbackOnce.Clear();
    }

    private System.Action _updateInput = delegate { };


    void Update()
    {
        _updateInput();
    }

    public InputLayer Initialize()
    {
#if UNITY_EDITOR
        InitializeMouse();
        _updateInput = UpdateMouse;
#else
        _updateInput = UpdateTouch;
#endif

        _onAnyTouchBegan = (Vector3 touchPosition) =>
        {
            while (_onAnyTouchBeganCallbackOnce.Count > 0)
                _onAnyTouchBeganCallbackOnce.Dequeue()(touchPosition);
        };

        _onAnyTouchFinished = (Vector3 touchPosition) =>
        {
            while (_onAnyTouchFinishedCallbackOnce.Count > 0)
                _onAnyTouchFinishedCallbackOnce.Dequeue()(touchPosition);
        };

        return this;
    }

    // TODO : Unityの定数があれば差し替え
    private enum MouseInput
    {
        Left = 0,
        Max,
    }
    private List<bool> _oldMouseInputs = new List<bool>();
    private void InitializeMouse()
    {
        for (int i = 0; i < (int)MouseInput.Max; i++)
            _oldMouseInputs.Add(false);
    }
    private void UpdateMouse()
    {
        if (m_camera == null)
            return;

        for (int i = 0; i < (int)MouseInput.Max; i++)
        {
            if (Input.GetMouseButtonDown(i)
                && !_oldMouseInputs[i])
                _onAnyTouchBegan(m_camera.ScreenToWorldPoint(Input.mousePosition));
            else if (!Input.GetMouseButtonDown(i)
                && _oldMouseInputs[i])
                _onAnyTouchFinished(m_camera.ScreenToWorldPoint(Input.mousePosition));
            _oldMouseInputs[i] = Input.GetMouseButtonDown(i);
        }
    }

    private void UpdateTouch()
    {
        if (m_camera == null)
            return;

        for (int i = 0; i < Input.touchCount; ++i)
        {
            switch (Input.GetTouch(i).phase)
            {
                case TouchPhase.Began:
                    _onAnyTouchBegan(m_camera.ScreenToWorldPoint(Input.GetTouch(i).position));
                    break;
                case TouchPhase.Ended:
                    _onAnyTouchFinished(m_camera.ScreenToWorldPoint(Input.GetTouch(i).position));
                    break;
                default:
                    break;
            }
        }
    }
}