using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class BossIconContext : M4uContext
{
    private M4uProperty<bool> isSelect = new M4uProperty<bool>();
    public bool IsSelect { get { return isSelect.Value; } set { isSelect.Value = value; } }

    public ButtonModel model = null;
    public uint boss_id = 0;
}
