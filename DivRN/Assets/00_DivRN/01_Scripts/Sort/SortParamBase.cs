using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortParamBase
{
    public SORT_PARAM m_Type;
}

public class SortParam<T> : SortParamBase
{
    public T m_Param;
}

public class SortParamInt : SortParam<int>
{
    public int CompareTo(int _param)
    {
        if (m_Param < _param)
        {
            return 1;
        }

        if (m_Param > _param)
        {
            return -1;
        }

        return 0;
    }
}

public class SortParamLong : SortParam<long>
{
    public int CompareTo(long _param)
    {
        if (m_Param < _param)
        {
            return 1;
        }

        if (m_Param > _param)
        {
            return -1;
        }

        return 0;
    }
}

public class SortParamDouble : SortParam<double>
{
    public int CompareTo(double _param)
    {
        if (m_Param < _param)
        {
            return 1;
        }

        if (m_Param > _param)
        {
            return -1;
        }

        return 0;
    }
}

public class SortParamBool : SortParam<bool>
{
    public bool Param { get { return m_Param; } }
    public int CompareTo(bool _param)
    {
        if (Param == true && _param == false)
        {
            return 1;
        }

        if (Param == false && _param == true)
        {
            return -1;
        }

        return 0;
    }
}
