using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 下部セーフエリア分上へずらす
/// </summary>
public class AdjustBottomAnchor : MonoBehaviour
{
    private RectTransform rectTrans = null;

    private Vector2 defaultPos = new Vector2(0, 0);

    // Use this for initialization
    void Start()
    {
        rectTrans = GetComponent<RectTransform>();

        defaultPos = new Vector2(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (SafeAreaControl.HasInstance)
        {
            updatePosition();
        }
    }

    private void updatePosition()
    {
        float ypos = defaultPos.y + SafeAreaControl.Instance.bottom_space_height;
        if (rectTrans.anchoredPosition.y != ypos)
        {
            rectTrans.anchoredPosition = new Vector2(defaultPos.x, ypos);
        }
    }
}
