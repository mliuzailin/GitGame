/**
 *  @file   TutorialDialogPointListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/01
 */

using UnityEngine;
using System.Collections;

public class TutorialDialogPointListItem : ListItem<TutorialDialogPointContext>
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        Context.DidSelectItem(Context);
    }

}
