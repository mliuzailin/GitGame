using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class DialogInfoIconContext : M4uContext
{
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    public uint m_CharaId = 0;

    public System.Action<uint> DidSelectIcon = delegate { };

    public void setup(uint _chara_id)
    {
        m_CharaId = _chara_id;
        UnitIconImageProvider.Instance.Get(
            m_CharaId,
            sprite => { IconImage = sprite; });
    }
}
