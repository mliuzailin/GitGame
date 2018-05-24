using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSpriteRendererMaterialAssigner : MonoBehaviour
{

    public ImageMaterialAssigner assigner;
    public ImageMaterialAssigner assigner02;

    private SpriteRenderer image;

    private bool assigned;

    public bool Assined
    {
        set { assigned = value; }
    }

    void Awake()
    {
        image = GetComponent<SpriteRenderer>();
        image.sprite = null;
    }

    void Update()
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
