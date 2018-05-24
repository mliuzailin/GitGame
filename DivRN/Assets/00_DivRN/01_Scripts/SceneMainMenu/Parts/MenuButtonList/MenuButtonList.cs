using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class MenuButtonList : M4uContextMonoBehaviour
{
    M4uProperty<string> menuTitle = new M4uProperty<string>();
    public string MenuTitle { get { return menuTitle.Value; } set { menuTitle.Value = value; } }

    M4uProperty<List<MenuButtonContext>> menulist = new M4uProperty<List<MenuButtonContext>>();
    public List<MenuButtonContext> Menulist { get { return menulist.Value; } set { menulist.Value = value; } }

    void Awake()
    {
        Menulist = new List<MenuButtonContext>();
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        //setupMenuList(MAINMENU_CATEGORY.UNIT);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setupMenuList(MAINMENU_CATEGORY _category)
    {
        MenuTitle = _category.ToString();

        MasterMenuButtonItem[] masterArray = Resources.LoadAll<MasterMenuButtonItem>("MasterData/MenuButtonItem");

        //List<MenuButtonContext> newList = new List<MenuButtonContext>();
        Menulist.Clear();
        foreach (MasterMenuButtonItem _item in masterArray)
        {
            if (_item.categoryType == _category)
            {
                MenuButtonContext newContext = new MenuButtonContext();
                newContext.Dto = _item;
                Menulist.Add(newContext);
            }
        }
        //menuList = newList;
    }

    public void setMenuAction(MAINMENU_BUTTON _buttonType, System.Action _action)
    {
        foreach (MenuButtonContext _menu in Menulist)
        {
            if (_menu.Dto.buttonType == _buttonType)
            {
                _menu.SelectAction = _action;
            }
        }
    }

}
