using UnityEngine;
using System.Collections;
using M4u;

public class MasterFooterSubMenuItem : M4uContextScriptableObject
{
    //private readonly int FOOTER_SUB_MENU_COUNT_MAX = 99;

    public string title = "テストデータ";
    public MainMenuFooter.FOOTER_MENU_TYPE footermenutype = MainMenuFooter.FOOTER_MENU_TYPE.NONE;
    public MAINMENU_SEQ mainmenuseqtype = MAINMENU_SEQ.SEQ_NONE;
    public int sortindex = 0;


    private M4uProperty<string> m4u_title = new M4uProperty<string>();
    public string M4u_title { get { return m4u_title.Value; } set { m4u_title.Value = value; } }

    void Awake()
    {
        M4u_title = title;
    }
}
