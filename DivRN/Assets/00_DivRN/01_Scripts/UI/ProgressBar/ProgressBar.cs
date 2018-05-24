using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProgressBar : View
{
    [SerializeField]
    private RectTransform m_bar;

    [SerializeField]
    private TextMeshProUGUI m_percentNum;
    [SerializeField]
    private Image m_percentIcon;


    private static readonly string PrefabPath = "Prefab/Common/ProgressBar/ProgressBar";

    private static float MaskWidth = 400.0f;
    private static float MaskHeight = 40.0f;



    public static ProgressBar Attach(GameObject parent = null)
    {
        var bar = View.Attach<ProgressBar>(PrefabPath, parent);
        UnityEngine.Debug.Assert(bar != null, "View.Attach<ProgressBar>() failed.");
        return bar;
    }


    public void Progress(float percent)
    {
        float ratio = percent / 100;
        m_bar.sizeDelta = new Vector2(MaskWidth * ratio, MaskHeight);
        m_percentNum.text = Mathf.FloorToInt(percent).ToString();
    }

    public void ProgressFiles(float current, float max)
    {
        m_percentIcon.enabled = false;
        float ratio = current / max;
        m_bar.sizeDelta = new Vector2(MaskWidth * ratio, MaskHeight);
        m_percentNum.text = String.Format("{0,2}/{1,2}", (int)current, (int)max);
    }
}
