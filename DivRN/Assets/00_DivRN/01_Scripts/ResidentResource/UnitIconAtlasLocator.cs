using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitIconAtlasLocator
{
    public UnitIconAtlasLocator(string name)
    {
        m_name = name;
    }

    private string m_name = "";
    public string name
    {
        get { return m_name; }
    }

    public bool InRange(int index)
    {
        return (index >= FromIndex) && (index <= ToIndex);
    }

    public int ToIndex
    {
        get
        {
            if (m_name.IsNullOrEmpty() ||
                m_name.Split('_').Length < 3)
            {
                return -1;
            }

            return NameIndex(2);
        }
    }

    public int FromIndex
    {
        get
        {
            if (m_name.IsNullOrEmpty() ||
                m_name.Split('_').Length < 2)
            {
                return -1;
            }

            return NameIndex(1);
        }
    }

    private int NameIndex(int index)
    {
        string[] array = m_name.Split('_');
        if (array.Length > index)
        {
            return array[index].ToInt(-1);
        }
        else
        {
            return -1;
        }
    }
}
