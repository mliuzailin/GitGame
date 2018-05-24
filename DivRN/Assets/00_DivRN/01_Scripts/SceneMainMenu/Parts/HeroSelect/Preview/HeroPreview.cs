/**
 *  @file   HeroPreview.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/20
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class HeroPreview : MenuPartsBase
{
    public Action OnClickViewAction = delegate { };
    M4uProperty<Sprite> unitImage = new M4uProperty<Sprite>();
    public Sprite UnitImage
    {
        get
        {
            return unitImage.Value;
        }
        set
        {
            unitImage.Value = value;
        }
    }

    M4uProperty<Texture> unitImage_mask = new M4uProperty<Texture>();
    public Texture UnitImage_mask
    {
        get
        {
            return unitImage_mask.Value;
        }
        set
        {
            unitImage_mask.Value = value;
        }
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickView()
    {
        if (OnClickViewAction != null)
        {
            OnClickViewAction();
        }

    }

}
