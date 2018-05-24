using UnityEngine;
using System.Collections;
using M4u;

public class MenuButtonContext : M4uContext
{
    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    private MasterMenuButtonItem dto = null;
    public MasterMenuButtonItem Dto
    {
        get { return dto; }
        set
        {
            dto = value;
            Title = dto.title;
        }
    }

    public System.Action SelectAction = delegate { };

}