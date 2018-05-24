using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortFilterBase
{
    public SORT_PARAM[] m_Types;
}

public class SortFilter<T> : SortFilterBase
{
    public T[] m_Filters;

    public SortFilter(SORT_PARAM[] _types, T[] _data)
    {
        m_Types = _types;
        m_Filters = _data;
    }

    public bool FilterTo<T1>(T1 param)
    {
        int _ret = System.Array.IndexOf(m_Filters, param);
        return (_ret != -1) ? true : false;
    }

    public bool FilterTo<T1>(SortParam<T1> param)
    {
        int _ret = System.Array.IndexOf(m_Filters, param.m_Param);
        return (_ret != -1) ? true : false;
    }
}
