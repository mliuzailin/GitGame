using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class MenuBannerContext : M4uContext
{
    M4uProperty<Sprite> itemImage = new M4uProperty<Sprite>();
    public Sprite ItemImage { get { return itemImage.Value; } set { itemImage.Value = value; } }

}
