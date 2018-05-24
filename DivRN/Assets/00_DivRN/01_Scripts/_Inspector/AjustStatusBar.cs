using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AjustStatusBar : MonoBehaviour
{
    public Text debugText = null;
    public bool Half = false;
    public bool Invert = true;
    public bool HideBeforePositionFix = false;

    private RectTransform rectTrans = null;
    private Vector2 defaultPos = new Vector2(0, 0);
#if BUILD_TYPE_DEBUG
    private RectTransform canvasRect = null;
#endif

    // Use this for initialization
    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        defaultPos = new Vector2(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y);

        var children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        if (HideBeforePositionFix)
        {
            SafeAreaControl.Instance.AddOnBarHeightFixedCallback(() =>
            {
                foreach (var child in children)
                {
                    child.gameObject.SetActive(true);
                }

                updatePosition();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
#if BUILD_TYPE_DEBUG
        if (debugText != null &&
            SafeAreaControl.HasInstance)
        {
            debugText.text = "";
            if (canvasRect == null)
            {
                Canvas canvas = GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    canvasRect = canvas.GetComponent<RectTransform>();
                }
            }

            if (canvasRect != null)
            {
                debugText.text += "canvas:[" + (int)canvasRect.rect.width + ", " + (int)canvasRect.rect.height + "]\n";
            }

            debugText.text += "disp:[" + SafeAreaControl.Instance.disp_width + ", " + SafeAreaControl.Instance.disp_height + "]\n";
#if UNITY_EDITOR
            debugText.text += "bar_height:" + SafeAreaControl.Instance.bar_height + "\n";
#elif UNITY_ANDROID
            debugText.text += "api_level:" + SafeAreaControl.Instance.api_level + "\n";
            debugText.text += "view_top:" + SafeAreaControl.Instance.view_top + "\n";
            debugText.text += "bar_height:" + SafeAreaControl.Instance.bar_height + "\n";
#elif UNITY_IOS
            debugText.text += "ios_bar:" + SafeAreaControl.Instance.ios_bar_width +", " + SafeAreaControl.Instance.ios_bar_height + "\n";
            debugText.text += "view_top:" + SafeAreaControl.Instance.view_top + "\n";
            debugText.text += "bar_height:" + SafeAreaControl.Instance.bar_height + "\n";
#endif
        }
#endif

        if (SafeAreaControl.HasInstance)
        {
            updatePosition();
        }
    }

    private void updatePosition()
    {
        int div = 1;
        if (Half)
        {
            div = 2;
        }

        int invert = 1;
        if (Invert)
        {
            invert = -1;
        }

        float ypos = defaultPos.y + ((SafeAreaControl.Instance.bar_height / div) * invert);
        if (rectTrans.anchoredPosition.y != ypos)
        {
            rectTrans.anchoredPosition = new Vector2(defaultPos.x, ypos);
        }
    }
}
