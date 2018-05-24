using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Configuration;
using System.Xml.Schema;

public class AssetBundlerMultiplier : BaseAssetBundler
{
    private AssetBundlerMultiplierFSM fsm;
    private Action errorAction;
    private Action failAction;
    private Action finishAction;
    private List<AssetBundler> children = new List<AssetBundler>();

    private Action<float> progressAction = null;
    private Action<float, float> progressfilesAction = null;

    private static int MAX_PARALLEL_DOWNLOAD = 2;


    public void RegisterProgressAction(Action<float> action)
    {
        progressAction = action;
    }

    public void RegisterProgressFilesAction(Action<float, float> action)
    {
        progressfilesAction = action;
    }

    protected override void Awake()
    {
        base.Awake();
        fsm = GetComponent<AssetBundlerMultiplierFSM>();
        Retryable();
    }

    void OnDestroy()
    {
        if (children != null)
        {
            children.Clear();
            children = null;
        }
    }

    void Update()
    {
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (progressAction == null
        && progressfilesAction == null)
        {
            return;
        }

        float all = 0;
        float amount = 0;

        foreach (var child in children)
        {
            all += 1.0f;

            if (child.IsSuccess ||
                child.IsError)
            {
                amount += 1.0f;
            }
            else if (child.Response != null &&
                     child.WWW != null)
            {
                amount += child.WWW.progress;
            }
        }

        if (all == 0)
        {
            return;
        }

        if (progressAction != null)
        {
            progressAction(amount / all);
        }
        if (progressfilesAction != null)
        {
            progressfilesAction(amount, all);
        }
    }

    public static bool IsExists
    {
        get
        {
            return GameObject.FindObjectOfType<AssetBundlerMultiplier>() != null;
        }
    }

    public AssetBundlerMultiplier SetName(string n)
    {
        gameObject.name = "AssetBundlerMultiplier::" + n;
        return this;
    }

    public AssetBundlerMultiplier Retryable()
    {
        DisableAutoDestoryOnFail();
        SetOnFail(() =>
        {
            DivRNUtil.ShowRetryDialog(
                () =>
                {
                    fsm.SendFsmEvent("RETRY");
                });
        });

        return this;
    }

    public AssetBundlerMultiplier DisableAutoDestoryOnSuccess()
    {
        IsAutoDestroyOnSuccess = false;
        return this;
    }

    public AssetBundlerMultiplier DisableAutoDestoryOnFail()
    {
        IsAutoDestroyOnFail = false;
        return this;
    }

    public int ChildCount
    {
        get
        {
            return transform.childCount;
        }
    }

    public List<AssetBundler> Children
    {
        get
        {
            return children;
        }
    }

    public static AssetBundlerMultiplier Create(string name = null)
    {
        GameObject go = Instantiate(Resources.Load("Prefab/AssetBundlerMultiplier") as GameObject);
        AssetBundlerMultiplier result = go.GetComponent<AssetBundlerMultiplier>();
        if (name != null)
        {
            result.SetName(name);
        }
        return result;
    }

    public AssetBundlerMultiplier Add(AssetBundler assetBundler)
    {
        assetBundler.DisableAutoDestoryOnSuccess();
        assetBundler.transform.parent = transform;
        return this;
    }

    public AssetBundlerMultiplier SetOnError(Action onError)
    {
        this.errorAction = onError;
        return this;
    }

    public AssetBundlerMultiplier SetOnFail(Action onFail)
    {
        this.failAction = onFail;
        return this;
    }

    public AssetBundlerMultiplier SetOnFinish(Action onFinish)
    {
        this.finishAction = onFinish;
        return this;
    }

    public AssetBundlerMultiplier Load(Action onFinish = null, Action errorAciton = null/*, Action failAciton = null */)
    {
        if (onFinish != null)
        {
            SetOnFinish(onFinish);
        }

        if (errorAciton != null)
        {
            SetOnError(errorAciton);
        }

        // failはリトライ処理。
        // 通常使う必要性はないので使えないようにする
        // 状況に応じて調整
        /*
        if (failAciton != null)
        {
            SetOnFail(failAciton);
        }
        */

        fsm.SendFsmEvent("DO_LOAD");

        return this;
    }

    void OnLoad()
    {
        children = transform.GetComponentsInChildren<AssetBundler>().ToList();

        for (int i = 0; i < children.Count; i++)
        {
            AssetBundler ab = children[i];
            ab.disableRetryDialog();
            ab.setAction(
                    (o) =>
                    {
                        NextDownlaod();
                    },
                    (str) =>
                    {
                        NextDownlaod();
                    },
                    (str) =>
                    {
                        fsm.SendFsmEvent("FAIL");
                    });
        }

        fsm.SendFsmEvent("DO_WAIT");
    }

    IEnumerator OnWait()
    {
        //　ダウンロード開始
        for (int i = 0; i < MAX_PARALLEL_DOWNLOAD; i++)
        {
            NextDownlaod();
        }

        //ダウンロード待ち
        while (children.Any(ab => !ab.IsEnd))
        {
            yield return null;
        }

        if (children.Any(ab => ab.IsFail))
        {
            fsm.SendFsmEvent("FAIL");
        }
        else if (children.Any(ab => ab.IsError))
        {
            fsm.SendFsmEvent("ERROR");
        }
        else
        {
            fsm.SendFsmEvent("SUCCESS");
        }
    }

    void OnSuccess()
    {
        if (finishAction != null)
        {
            finishAction();
        }

        if (IsAutoDestroyOnSuccess)
        {
            Destroy(gameObject);
        }
    }

    void OnError()
    {
        Debug.LogError("ERROR");

        if (errorAction != null)
        {
            errorAction();
        }

        if (IsAutoDestroyOnSuccess)
        {
            Destroy(gameObject);
        }
    }

    void OnFail()
    {
        Debug.LogError("Fail");

        if (failAction != null)
        {
            failAction();
        }

        if (IsAutoDestroyOnFail)
        {
            Destroy(gameObject);
        }
    }

    private void NextDownlaod()
    {
        int run_cunt = 0;

        for (int i = 0; i < children.Count; i++)
        {
            if (MAX_PARALLEL_DOWNLOAD <= run_cunt)
            {
                return;
            }

            AssetBundler ab = children[i];

            if (ab.IsSuccess)
            {
                continue;
            }

            if (ab.IsError)
            {
                continue;
            }

            if (ab.IsWait)
            {
                run_cunt++;
                continue;
            }

            if (ab.IsFail)
            {
                ab.Retry();
                run_cunt++;
                continue;
            }

            if (ab.IsReady)
            {
                ab.Load();
                run_cunt++;
                continue;
            }
        }
    }

    public override string ActiveStateName
    {
        get
        {
            return fsm.ActiveStateName;
        }
    }
}
