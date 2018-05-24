using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AjustBottomMask : MonoBehaviour
{
    public bool Half = false;
    public bool Invert = true;

    private RectTransform rectTrans = null;

    private Vector2 defaultSizeDelta = new Vector2(0, 0);

    // Use this for initialization
    void Start()
    {
        rectTrans = GetComponent<RectTransform>();

        defaultSizeDelta = new Vector2(rectTrans.sizeDelta.x, rectTrans.sizeDelta.y);

        var children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }
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

        float ysize = defaultSizeDelta.y + SafeAreaControl.Instance.bottom_space_height;
        if (rectTrans.sizeDelta.y != ysize)
        {
            rectTrans.sizeDelta = new Vector2(defaultSizeDelta.x, ysize);
        }
    }
}
