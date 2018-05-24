using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitEvolveTouchArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public System.Action DidPointerEnter = delegate { };
    public System.Action DidPointerExit = delegate { };

    public void OnPointerEnter(PointerEventData eventData)
    {
        DidPointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DidPointerExit();
    }
}
