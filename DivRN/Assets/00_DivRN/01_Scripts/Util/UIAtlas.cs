using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class UIAtlas
{
    public Material material = null;
    public Sprite[] sprites = null;

    public string Name
    {
        get;
        set;
    }

    public Material spriteMaterial
    {
        get
        {
            return material;
        }
        set
        {
            material = value;
        }
    }

    public Sprite[] spriteList
    {
        get
        {
            return sprites;
        }
        set
        {
            sprites = value;
        }
    }

    public Texture texture
    {
        get
        {
            if (material != null)
            {
                return material.mainTexture;
            }
            else if (!sprites.IsNullOrEmpty())
            {
                return sprites[0].texture;
            }
            return null;
        }
    }

    public Sprite GetSprite(string name)
    {
        return sprites.FirstOrDefault(s => s.name.Equals(name));
    }
}
