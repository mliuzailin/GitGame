/**
 *  @file   ScrollbarAttacher.cs
 *  @brief  スクロールバーのプレハブをはり付けるためのクラス
 *  @author Developer
 *  @date   2017/06/23
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(ScrollRect))]
public class ScrollbarAttacher : MonoBehaviour
{
    /// <summary>スクロール感度</summary>
    const float SCROLL_SENSITIVITY = 40f;

    [SerializeField]
    GameObject ScrollBarPrefab = null;
    [SerializeField]
    Scrollbar.Direction Direction = Scrollbar.Direction.BottomToTop;

    /// <summary>始点位置を変える</summary>
    [SerializeField]
    float StartOffset = 0f;
    /// <summary>終点位置を変える</summary>
    [SerializeField]
    float EndOffset = 0f;
    /// <summary>スクロールバーの位置を変える</summary>
    [SerializeField]
    float Offset = 0f;

    [SerializeField]
    bool SetActive = true;

    void Awake()
    {
        ScrollRect scroll = GetComponent<ScrollRect>();
        if (scroll == null) { return; }

        scroll.scrollSensitivity = SCROLL_SENSITIVITY;

        if (ScrollBarPrefab != null)
        {
            GameObject scrollbarObj = Instantiate(ScrollBarPrefab);
            Scrollbar scrollbar = scrollbarObj.GetComponent<Scrollbar>();
            if (scrollbar == null) { return; }
            scrollbar.gameObject.transform.SetParent(scroll.transform, false);

            RectTransform scrollbarRect = scrollbar.GetComponent<RectTransform>();

            if (Direction == Scrollbar.Direction.BottomToTop || Direction == Scrollbar.Direction.TopToBottom)
            {
                scroll.verticalScrollbar = scrollbar;

                scrollbarRect.offsetMax = new Vector2(scrollbarRect.offsetMax.x, scrollbarRect.offsetMax.y - StartOffset);
                scrollbarRect.offsetMin = new Vector2(scrollbarRect.offsetMin.x, scrollbarRect.offsetMin.y + EndOffset);
                scrollbarRect.anchoredPosition = new Vector2(scrollbarRect.anchoredPosition.x + Offset, scrollbarRect.anchoredPosition.y);

            }
            else
            {
                scroll.horizontalScrollbar = scrollbar;

                scrollbarRect.offsetMax = new Vector2(scrollbarRect.offsetMax.x - StartOffset, scrollbarRect.offsetMax.y);
                scrollbarRect.offsetMin = new Vector2(scrollbarRect.offsetMin.x + EndOffset, scrollbarRect.offsetMin.y);
                scrollbarRect.anchoredPosition = new Vector2(scrollbarRect.anchoredPosition.x, scrollbarRect.anchoredPosition.y + Offset);
            }


            scrollbar.direction = Direction;
            scrollbar.gameObject.SetActive(SetActive);
        }

    }

}
