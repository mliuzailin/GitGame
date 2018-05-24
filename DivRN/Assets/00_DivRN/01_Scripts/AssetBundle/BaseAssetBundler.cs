using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAssetBundler : MonoBehaviour
{
    protected virtual void Awake()
    {
        IsAutoDestroyOnSuccess = true;
        IsAutoDestroyOnFail = true;
    }

    public bool IsEnd
    {
        get
        {
            return IsError || IsFail || IsSuccess;
        }
    }

    public bool IsReady
    {
        get
        {
            return ActiveStateName.Equals("Ready");
        }
    }
    public bool IsWait
    {
        get
        {
            return ActiveStateName.Equals("Wait");
        }
    }

    public bool IsError
    {
        get
        {
            return ActiveStateName.Equals("Error");
        }
    }

    public bool IsFail
    {
        get
        {
            return ActiveStateName.Equals("Fail");
        }
    }

    public bool IsSuccess
    {
        get
        {
            return ActiveStateName.Equals("Success");
        }
    }

    public virtual string ActiveStateName
    {
        get
        {
            return null;
        }
    }

    protected bool IsAutoDestroyOnFail
    {
        get;
        set;
    }

    protected bool IsAutoDestroyOnSuccess
    {
        get;
        set;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public static bool HasAssetBundler()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("AssetBundler");
        BaseAssetBundler assetBundler = null;
        for (int i = 0; i < objArray.Length; i++)
        {
            assetBundler = objArray[i].GetComponent<BaseAssetBundler>();
            if (assetBundler != null)
            {
                break;
            }

        }

        return (assetBundler != null);
    }

}
