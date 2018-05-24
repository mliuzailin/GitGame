using UnityEngine;
using System.Collections;
using M4u;

public class MasterMenuButtonItem : M4uContextScriptableObject
{
    public string title = "テストデータ";
    public MAINMENU_CATEGORY categoryType = MAINMENU_CATEGORY.NONE;
    public MAINMENU_BUTTON buttonType = MAINMENU_BUTTON.NONE;
    public MAINMENU_SEQ switchSeqType = MAINMENU_SEQ.SEQ_NONE;
    public int sortindex = 0;
    public string iconImageName = "";
    public string textImageName = "";
}
