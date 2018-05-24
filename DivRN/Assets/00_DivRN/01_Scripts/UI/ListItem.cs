using UnityEngine;
using System.Collections;
using M4u;

public class ListItem<T> : ListItemView where T : M4uContext
{
    public T Context
    {
        get
        {
            return gameObject.GetComponent<M4uContextRoot>().Context as T;
        }
    }

    public int Index
    {
        get
        {
            return int.Parse(gameObject.name);
        }
    }
}
