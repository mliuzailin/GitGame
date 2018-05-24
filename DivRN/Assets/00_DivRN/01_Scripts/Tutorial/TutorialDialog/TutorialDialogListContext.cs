/**
 *  @file   TutorialDialogListContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/01
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using M4u;

public class TutorialDialogListContext : M4uContext
{
    public Toggle Toggle;

    M4uProperty<Sprite> image = new M4uProperty<Sprite>();
    public Sprite Image
    {
        get
        {
            return image.Value;
        }
        set
        {
            image.Value = value;
        }
    }
}
