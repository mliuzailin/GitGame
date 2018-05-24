using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class TitleMenuButton : M4uContextMonoBehaviour
{
    M4uProperty<string> menu_text = new M4uProperty<string>();
    public string Menu_text { get { return menu_text.Value; } set { menu_text.Value = value; } }

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        gameObject.GetComponent<M4uContextRoot>().Context = this;
    }

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        //		Menu_text = GameTextUtil.GetText("");
        Menu_text = "MENU";
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {

    }
}
