using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DynamicImageMaterialAssigner : MonoBehaviour
{
    public ImageMaterialAssigner assigner;
    public ImageMaterialAssigner assigner02;

    private Image image;

    private bool assigned;

    public bool Assined
    {
        set { assigned = value; }
    }

    void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = null;
    }

    void LateUpdate()
    {
        if (image.sprite == null)
        {
            return;
        }

        if (assigned)
        {
            return;
        }

        assigned = true;
        Material mat = assigner.FindMaterial(image.sprite);
        if (mat == null && assigner02 != null)
        {
            mat = assigner02.FindMaterial(image.sprite);
        }
        image.material = mat;
    }
}
