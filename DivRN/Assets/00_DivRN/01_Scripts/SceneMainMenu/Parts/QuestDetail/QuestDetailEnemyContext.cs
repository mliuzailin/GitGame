using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class QuestDetailEnemyContext : M4uContext
{
    M4uProperty<Sprite> elementImage = new M4uProperty<Sprite>();
    public Sprite ElementImage { get { return elementImage.Value; } set { elementImage.Value = value; } }
}
