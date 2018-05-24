using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening.Plugins;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialArrow : MonoBehaviour
{
    private bool m_IsDestroy = false;	// 削除されたフラグ(OnEnable,OnDisable 中は Destroy できないのでフラグを見て削除)

    void Awake()
    {
        //        Hide();
    }

    private void Update()
    {
        if (m_IsDestroy)
        {
            m_IsDestroy = false;
            Hide();
        }
    }

    private void OnDisable()
    {
        if (disableAction != null)
        {
            disableAction();
            disableAction = null;

            m_IsDestroy = true;
        }
    }

    public static TutorialArrow Create(string n)
    {
        GameObject go = GameObject.Find(n);
        if (go == null)
        {
            Debug.LogError("TutorialArrow is not found:" + n);
            return null;
        }

        GameObject arrowPrefab = Resources.Load("Prefab/TutorialArrow") as GameObject;
        TutorialArrow ar = NOUtil.AddChild(go, arrowPrefab).GetComponent<TutorialArrow>();
        ar.setupAction();
        return ar;
    }

    private Button targetButton;
    private UGUILongPress targetLongPress;

    private void setupAction()
    {
        targetButton = transform.GetComponentInParent<Button>();
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(ClickAction);
            return;
        }

        targetLongPress = transform.GetComponentInParent<UGUILongPress>();
        if (targetLongPress != null)
        {
            targetLongPress.m_OnClick.AddListener(ClickAction);
            return;
        }
    }

    public TutorialArrow SetLocalPosition(Vector3 v)
    {
        GetComponent<RectTransform>().localPosition = v;
        return this;
    }

    public TutorialArrow SetLocalEulerAngles(Vector3 v)
    {
        GetComponent<RectTransform>().localEulerAngles = v;
        return this;
    }

    public TutorialArrow SwitchRootCanvas()
    {
        Canvas root = gameObject.GetComponentInParent<Canvas>();
        gameObject.transform.SetParent(root.transform, true);
        return this;
    }

    private Action pushAction = null;	// 対象ボタンが押された時のアクション
    private Action disableAction = null;	// 対象ボタンが押されることなく消えた場合のアクション（対象ボタンが押された場合はこちらは実行されない）

    private void ClickAction()
    {
        if (pushAction != null)
        {
            pushAction();
            pushAction = null;
        }

        disableAction = null;

        if (targetButton != null)
        {
            targetButton.onClick.RemoveListener(ClickAction);
        }

        if (targetLongPress != null)
        {
            targetLongPress.m_OnClick.RemoveListener(ClickAction);
        }

        Hide();
    }


    public void Show(Action push_action, Action disable_action = null)
    {
        this.pushAction = push_action;
        this.disableAction = disable_action;
    }

    public void Hide()
    {
        DestroyImmediate(gameObject);
    }
}
