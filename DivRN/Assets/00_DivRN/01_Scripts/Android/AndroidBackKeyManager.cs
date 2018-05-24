using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidBackKeyManager : SingletonComponent<AndroidBackKeyManager>
{
    private bool m_Disable = false;

    public class ActionData
    {
        public GameObject m_Object;
        public System.Action m_Action;

        public ActionData(GameObject _obj, System.Action _action)
        {
            m_Object = _obj;
            m_Action = _action;
        }
    }

    private Stack<ActionData> m_Stack = new Stack<ActionData>();

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (m_Stack.Count == 0)
        {
            return;
        }

#if UNITY_ANDROID || UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StackAction();
        }
#endif
    }

    public void StackClear()
    {
        m_Stack.Clear();
    }

    public bool StackAction()
    {
        if (m_Disable)
        {
            // BackKeyが無効化されている
#if BUILD_TYPE_DEBUG
            Debug.Log("AndroidBackKeyManager#Update cancel: Disable");
#endif
            return false;
        }

        if (TutorialManager.IsExists)
        {
            // チュートリアル中はMainMenuManagerのActionを実行しない
            if (isTopItem("MainMenuManager"))
            {
                return false;
            }
        }

        if (ServerApi.IsExists &&
            isTopItem("Dialog(Clone)") == false)
        {
            //通信中はActionを実行しない
#if BUILD_TYPE_DEBUG
            Debug.Log("AndroidBackKeyManager#Update cancel: Network");
#endif
            return false;
        }

        if (MainMenuManager.HasInstance &&
            MainMenuManager.Instance.IsPageSwitch())
        {
            if (isTopItem("TutorialDialog(Clone)") == false)
            {
                //Menuでページ遷移中はActionを実行しない
#if BUILD_TYPE_DEBUG
                Debug.Log("AndroidBackKeyManager#Update cancel: PageSwitch");
#endif
                return false;
            }
            else
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("AndroidBackKeyManager#Update cancel: PageSwitch skip");
#endif
            }
        }

        if (ButtonBlocker.Instance.IsActive())
        {
            //ButtonBlockerがロックされている
#if BUILD_TYPE_DEBUG
            Debug.Log("AndroidBackKeyManager#Update cancel: ButtonBlock");
#endif
            return false;
        }

        ActionData _data = m_Stack.Peek();
        if (_data.m_Object != null)
        {
            if (_data.m_Action != null)
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("AndroidBackKeyManager#Update Action:" + _data.m_Object.name);
#endif
                _data.m_Action();
                return true;
            }
        }
        else
        {
#if BUILD_TYPE_DEBUG
            Debug.LogError("AndroidBackKeyManager#Update Action: null _obj");
#endif
        }

        return false;
    }

    public bool isTopItem(string name)
    {
#if BUILD_TYPE_DEBUG
        //        Debug.Log("AndroidBackKeyManager#isTopItem: start");
#endif
        ActionData[] actionArray = m_Stack.ToArray();

        if (actionArray.Length < 1)
        {
            return false;
        }

        ActionData action = actionArray[0];
#if BUILD_TYPE_DEBUG
        Debug.Log("AndroidBackKeyManager#isTopItem: action.m_Object.name " + action.m_Object.name);
#endif

        if (action.m_Object.name == name)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("AndroidBackKeyManager#isTopItem: " + name);
#endif
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StackPush(GameObject _obj, System.Action _action)
    {
        if (_obj == null)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("AndroidBackKeyManager#push: null _obj");
#endif
        }

        ActionData _newData = new ActionData(_obj, _action);
        m_Stack.Push(_newData);
#if BUILD_TYPE_DEBUG
        //        Debug.Log("AndroidBackKeyManager#push: " + _newData.m_Object.name);
#endif
    }

    public void StackPop(GameObject _obj)
    {
        if (m_Stack.Count == 0)
        {
            return;
        }

        ActionData[] actionArray = m_Stack.ToArray();
        m_Stack.Clear();
        for (int i = actionArray.Length - 1; i >= 0; i--)
        {
            ActionData action = actionArray[i];

            if (action.m_Object == null)
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("AndroidBackKeyManager#pop: null _obj");
#endif
                continue;
            }
            if (action.m_Object == _obj)
            {
#if BUILD_TYPE_DEBUG
                //                Debug.Log("AndroidBackKeyManager#pop: " + action.m_Object.name);
#endif
                continue;
            }
            m_Stack.Push(action);
        }
    }

    public void DisableBackKey()
    {
        m_Disable = true;
    }

    public void EnableBackKey()
    {
        m_Disable = false;
    }
}
