using UnityEngine;
using System.Collections.Generic;
using M4u;

public class UnitGridViewModelContainer : M4uContextMonoBehaviour
{
    private M4uProperty<List<UnitGridContext>> items = new M4uProperty<List<UnitGridContext>>(new List<UnitGridContext>());
    public List<UnitGridContext> Items
    {
        get
        {
            return items.Value;
        }
        set
        {
            items.Value = value;
        }
    }

    public void SetHeight(float height)
    {
        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    public void UpdateHeight(float height)
    {
        var rectTransform = gameObject.GetComponent<RectTransform>();
        SetHeight(height);
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }
}
