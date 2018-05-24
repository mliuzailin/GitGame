using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class OmakeDataContext : M4uContext
{
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<int> count = new M4uProperty<int>();
    public int Count { get { return count.Value; } set { count.Value = value; } }
}
