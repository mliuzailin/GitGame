using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class GlobalMenuContext : M4uContext
{
    /// <summary>
    /// タイトル
    /// </summary>
    private M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    /// <summary>
    /// メニュー関連
    /// </summary>

    //ユーザー機能
    private M4uProperty<List<GlobalMenuItem>> userMenuList = new M4uProperty<List<GlobalMenuItem>>();
    public List<GlobalMenuItem> UserMenuList { get { return userMenuList.Value; } set { userMenuList.Value = value; } }

    //ゲーム機能
    private M4uProperty<List<GlobalMenuItem>> gameMenuList = new M4uProperty<List<GlobalMenuItem>>();
    public List<GlobalMenuItem> GameMenuList { get { return gameMenuList.Value; } set { gameMenuList.Value = value; } }

    //その他
    private M4uProperty<List<GlobalMenuItem>> otherMenuList = new M4uProperty<List<GlobalMenuItem>>();
    public List<GlobalMenuItem> OtherMenuList { get { return otherMenuList.Value; } set { otherMenuList.Value = value; } }

    private M4uProperty<bool> isActiveReturn = new M4uProperty<bool>();
    public bool IsActiveReturn { get { return isActiveReturn.Value; } set { isActiveReturn.Value = value; } }

    private M4uProperty<bool> isActiveFlag = new M4uProperty<bool>();
    public bool IsActiveFlag { get { return isActiveFlag.Value; } set { isActiveFlag.Value = value; } }

    private M4uProperty<bool> isActiveTopMenu = new M4uProperty<bool>();
    public bool IsActiveTopMenu { get { return isActiveTopMenu.Value; } set { isActiveTopMenu.Value = value; } }



    public GlobalMenuContext()
    {
        Title = "MENU";
        UserMenuList = new List<GlobalMenuItem>();
        GameMenuList = new List<GlobalMenuItem>();
        OtherMenuList = new List<GlobalMenuItem>();
        IsActiveReturn = false;
        IsActiveFlag = false;
        IsActiveTopMenu = true;
    }
}
