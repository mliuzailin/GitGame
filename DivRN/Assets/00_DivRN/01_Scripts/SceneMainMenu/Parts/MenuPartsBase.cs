using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class MenuPartsBase : View
{
    public void SetPosition(Vector2 _pos)
    {
        RectTransform trans = GetComponent<RectTransform>();
        if (trans == null)
        {
            return;
        }
        trans.anchoredPosition = new Vector2(_pos.x, _pos.y);
    }

    public void SetPositionAjustStatusBar(Vector2 _pos)
    {
        RectTransform trans = GetComponent<RectTransform>();
        if (trans == null)
        {
            return;
        }

        float bar_height = 0.0f;
        if (SafeAreaControl.HasInstance)
        {
            bar_height = (float)(SafeAreaControl.Instance.bar_height * -1);
        }

        trans.anchoredPosition = new Vector2(_pos.x, _pos.y + bar_height);
    }

    public void SetPosition(Vector2 _pos, Vector2 _size, bool Ignore = false)
    {
        RectTransform trans = GetComponent<RectTransform>();
        if (trans == null)
        {
            return;
        }
        trans.anchoredPosition = new Vector2(_pos.x, _pos.y);

        CanvasSetting _setting = GetComponentInParent<CanvasSetting>();
        if (_setting != null &&
             Ignore == false)
        {
            trans.sizeDelta = new Vector2(_size.x + _setting.GetAddWidth(), _size.y);
        }
        else
        {
            trans.sizeDelta = new Vector2(_size.x, _size.y);
        }
    }

    /// <summary>
    /// Pivotが中央でないとうまく調整されない
    /// </summary>
    /// <param name="_pos">Anchored Position</param>
    /// <param name="_size">Size Delta</param>
    public void SetPositionAjustStatusBar(Vector2 _pos, Vector2 _size)
    {
        RectTransform trans = GetComponent<RectTransform>();
        if (trans == null)
        {
            return;
        }

        float bar_height = 0.0f;
        float bar_height_half = 0.0f;
        float bottom_height = 0.0f;
        float bottom_height_half = 0.0f;
        if (SafeAreaControl.HasInstance)
        {
            float bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
            bar_height = (float)(SafeAreaControl.Instance.bar_height * -1);
            bar_height_half = bar_height * 0.5f;
            bottom_height = (float)(bottom_space_height * -1);
            bottom_height_half = bottom_height * 0.5f;
        }

        if (trans.pivot.x == 1.0f &&
            trans.pivot.y == 1.0f)
        {
            CanvasSetting _setting = GetComponentInParent<CanvasSetting>();
            if (_setting != null)
            {
                trans.anchoredPosition = new Vector2(_pos.x + _setting.GetAddWidth() / 2.0f, _pos.y + bar_height);
                trans.sizeDelta = new Vector2(_size.x + _setting.GetAddWidth(), _size.y + bar_height + bottom_height);
            }
            else
            {
                trans.anchoredPosition = new Vector2(_pos.x, _pos.y + bar_height);
                trans.sizeDelta = new Vector2(_size.x, _size.y + bar_height + bottom_height);
            }

        }
        else
        {
            trans.anchoredPosition = new Vector2(_pos.x, _pos.y + bar_height_half - bottom_height_half);

            CanvasSetting _setting = GetComponentInParent<CanvasSetting>();
            if (_setting != null)
            {
                trans.sizeDelta = new Vector2(_size.x + _setting.GetAddWidth(), _size.y + bar_height + bottom_height);
            }
            else
            {
                trans.sizeDelta = new Vector2(_size.x, _size.y + bar_height + bottom_height);
            }
        }

    }

    public void SetSize(Vector2 _size, bool notUseCanvasSetting = false)
    {
        RectTransform trans = GetComponent<RectTransform>();
        if (trans == null)
        {
            return;
        }
        trans.sizeDelta = new Vector2(_size.x, _size.y);

        CanvasSetting _setting = GetComponentInParent<CanvasSetting>();
        if (_setting != null && notUseCanvasSetting == false)
        {
            trans.sizeDelta = new Vector2(_size.x + _setting.GetAddWidth(), _size.y);
        }
        else
        {
            trans.sizeDelta = new Vector2(_size.x, _size.y);
        }
    }

    public void SetSizeParfect(Vector2 _size)
    {
        RectTransform trans = GetComponent<RectTransform>();
        if (trans == null)
        {
            return;
        }
        CanvasSetting _setting = GetComponentInParent<CanvasSetting>();
        if (_setting != null)
        {
            trans.sizeDelta = new Vector2(_size.x + _setting.GetAddWidth(), _size.y);
        }
        else
        {
            trans.sizeDelta = new Vector2(_size.x, _size.y);
        }
    }

    public void SetParent(GameObject _parent)
    {
        transform.SetParent(_parent.transform, false);
    }

    /// <summary>
    /// トップとボトムの位置設定
    /// </summary>
    /// <param name="pos">(Anchoerd Position Y, SizeDelta Y)</param>
    public virtual void SetTopAndBottomAjustStatusBar(Vector2 pos)
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }

        float bar_height = 0.0f;
        float bar_height_half = 0.0f;
        float bottom_height = 0.0f;
        float bottom_height_half = 0.0f;
        if (SafeAreaControl.HasInstance)
        {
            float bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
            bar_height = (float)(SafeAreaControl.Instance.bar_height * -1);
            bar_height_half = bar_height * 0.5f;
            bottom_height = (float)(bottom_space_height * -1);
            bottom_height_half = bottom_height * 0.5f;
        }

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, pos.x + bar_height_half - bottom_height_half);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, pos.y + bar_height + bottom_height);
    }

    /// <summary>
    /// レイアウト更新
    /// </summary>
    public void updateLayout()
    {
        LayoutGroup[] layoutGroups = GetComponentsInChildren<LayoutGroup>();
        for (int i = 0; i < layoutGroups.Length; i++)
        {
            LayoutRebuilder.MarkLayoutForRebuild(layoutGroups[i].transform as RectTransform);
        }
    }
}
