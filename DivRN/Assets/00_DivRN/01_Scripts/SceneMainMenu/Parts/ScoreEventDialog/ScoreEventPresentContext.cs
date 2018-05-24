using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ScoreEventPresentContext : M4uContext
{
    M4uProperty<string> message = new M4uProperty<string>();
    public string Message { get { return message.Value; } set { message.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }
}
