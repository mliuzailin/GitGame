/**
 *  @file   SubTabGroupContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/07/01
 */

using UnityEngine;
using System.Collections;
using M4u;
using UnityEngine.UI;
using System.Collections.Generic;

public class SubTabGroupContext : M4uContext
{
    public Toggle Toggle = null;


    M4uProperty<List<SubTabContext>> subTabList = new M4uProperty<List<SubTabContext>>(new List<SubTabContext>());
    public List<SubTabContext> SubTabList { get { return subTabList.Value; } set { subTabList.Value = value; } }

}
