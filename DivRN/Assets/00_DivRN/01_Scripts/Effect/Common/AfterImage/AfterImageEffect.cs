using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AfterImageEffect : View
{
    [SerializeField]
    private Image m_image;
    [SerializeField]
    private RectTransform m_imageRectTransform;

    private static readonly string PrefabPath = "Prefab/Effect/Common/AfterImage/AfterImageEffect";
    private static readonly string AnimationName = "after_image_effect";

    public static AfterImageEffect Attach(GameObject parent)
    {
        return View.Attach<AfterImageEffect>(PrefabPath, parent);
    }

    public AfterImageEffect SetImage(Sprite sprite, Material material = null)
    {
        m_image.sprite = sprite;

        if (material != null)
            m_image.material = material;

        if (sprite != null)
            m_imageRectTransform.sizeDelta = new Vector2(
                sprite.bounds.size.x * sprite.pixelsPerUnit,
                sprite.bounds.size.y * sprite.pixelsPerUnit);

        return this;
    }

    public AfterImageEffect SetPosition(Vector3 position)
    {
        GetRoot().transform.localPosition = position;

        return this;
    }

    public AfterImageEffect SetSize(Vector2 size)
    {
        m_imageRectTransform.sizeDelta = size;

        return this;
    }

    public AfterImageEffect Show(System.Action callback = null)
    {
        PlayAnimation(AnimationName, () =>
        {
            if (callback != null)
                callback();

            Detach();
        });
        return this;
    }
}
