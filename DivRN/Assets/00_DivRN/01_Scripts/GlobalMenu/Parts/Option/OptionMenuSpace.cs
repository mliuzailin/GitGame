using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuSpace : View
{

    public void SetHight(float hight)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, hight);
    }
}
