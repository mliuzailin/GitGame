using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class OldAreaContext : M4uContext
{
    M4uProperty<Sprite> areaImage = new M4uProperty<Sprite>();
    public Sprite AreaImage { get { return areaImage.Value; } set { areaImage.Value = value; } }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    public uint m_AreaId = 0;
    public System.Action<uint> DidSelectArea = delegate { };
}
