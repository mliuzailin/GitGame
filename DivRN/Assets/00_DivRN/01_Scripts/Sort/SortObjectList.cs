using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortList<T> where T : SortObjectBase
{

    private List<T> body = new List<T>();
    public List<T> Body { get { return body; } set { body = value; } }

    private List<SortInfo> sortInfoList = new List<SortInfo>();
    private List<SortFilterBase> filterList = new List<SortFilterBase>();

    /// <summary>
    /// ソート条件クリア
    /// </summary>
    public void ClearSortInfo()
    {
        sortInfoList.Clear();
    }

    /// <summary>
    /// ソート条件追加
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_order"></param>
    public void AddSortInfo(SORT_PARAM _type, SORT_ORDER _order)
    {
        //特定のソートでは順序を反転する
        switch (_type)
        {
            case SORT_PARAM.FAVORITE:
                _order = (_order == SORT_ORDER.ASCENDING) ? SORT_ORDER.DESCENDING : SORT_ORDER.ASCENDING;
                break;
        }

        sortInfoList.Add(new SortInfo(_type, _order));
    }

    /// <summary>
    /// 実行
    /// </summary>
    /// <returns></returns>
    public List<T> Exec(SORT_OBJECT_TYPE type)
    {
        List<T> _ret = null;

        if (checkActiveFilter())
        {
            switch (type)
            {
                case SORT_OBJECT_TYPE.UNIT_LIST:
                    _ret = ExecUnitFilter();
                    break;
                case SORT_OBJECT_TYPE.LINUP_LIST:
                    _ret = ExecUnitFilter();
                    break;
                case SORT_OBJECT_TYPE.FRIEND_LIST:
                    _ret = ExecUnitFilter();
                    break;
                default:
                    Debug.Assert(true, "Exec Filter Type Error!");
                    break;
            }
        }
        else
        {
            _ret = Body;
        }

        if (checkActiveSortInfo())
        {
            switch (type)
            {
                case SORT_OBJECT_TYPE.UNIT_LIST:
                    _ret.Sort(SortUnitCompare);
                    break;
                case SORT_OBJECT_TYPE.LINUP_LIST:
                    _ret.Sort(SortUnitCompare);
                    break;
                case SORT_OBJECT_TYPE.FRIEND_LIST:
                    _ret.Sort(SortUnitCompare);
                    break;
                default:
                    Debug.Assert(true, "Exec Sort Type Error!");
                    break;
            }
        }

        return _ret;
    }


    /// <summary>
    /// フィルタクリア
    /// </summary>
    public void ClearFilter()
    {
        filterList.Clear();
    }

    /// <summary>
    /// フィルタ追加
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="_type"></param>
    /// <param name="_data"></param>
    public void AddFilter<T1>(SORT_PARAM _type, T1[] _data)
    {
        SORT_PARAM[] _tmp = new SORT_PARAM[1];
        _tmp[0] = _type;
        filterList.Add(new SortFilter<T1>(_tmp, _data));
    }

    /// <summary>
    /// フィルタ追加
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="_types"></param>
    /// <param name="_data"></param>
    public void AddFilter<T1>(SORT_PARAM[] _types, T1[] _data)
    {
        filterList.Add(new SortFilter<T1>(_types, _data));
    }

    /// <summary>
    /// フィルタ実行（ユニット）
    /// </summary>
    /// <returns></returns>
    private List<T> ExecUnitFilter()
    {
        List<T> tmpList = new List<T>();
        for (int i = 0; i < body.Count; i++)
        {
            body[i].IsActive = true;
            for (int j = 0; j < filterList.Count; j++)
            {
                if (!body[i].IsActive)
                {
                    continue;
                }

                body[i].FilterUnitTo(filterList[j]);
            }
            if (body[i].IsActive)
            {
                tmpList.Add(body[i]);
            }

        }
        return tmpList;
    }

    private bool checkActiveFilter()
    {
        bool bActive = false;
        for (int i = 0; i < filterList.Count; i++)
        {
            if (filterList[i].m_Types == null)
            {
                continue;
            }

            if (filterList[i].m_Types.Length == 0)
            {
                continue;
            }

            if (filterList[i].m_Types[0] == SORT_PARAM.NONE)
            {
                continue;
            }

            bActive = true;
        }

        return bActive;
    }

    /// <summary>
    /// 比較関数
    /// </summary>
    /// <param name="_objA"></param>
    /// <param name="_objB"></param>
    /// <returns></returns>

    private int SortUnitCompare(SortObjectBase _objA, SortObjectBase _objB)
    {
        int _ret = 0;
        for (int i = 0; i < sortInfoList.Count; i++)
        {
            _ret = _objA.CompareUnitTo(sortInfoList[i], _objB);

            if (_ret != 0)
            {
                return _ret;
            }
        }

        return _ret;
    }

    private bool checkActiveSortInfo()
    {
        bool bActive = false;
        for (int i = 0; i < sortInfoList.Count; i++)
        {
            if (sortInfoList[i].m_Type == SORT_PARAM.NONE)
            {
                continue;
            }

            bActive = true;
        }
        return bActive;
    }


    /// <summary>
    /// ソート＆フィルタ情報の設定
    /// </summary>
    /// <param name="sortInfo"></param>
    public void SetUpSortData(LocalSaveSortInfo sortInfo)
    {
        ClearSortInfo();
        ClearFilter();

        //--------------------------------
        // ソートの設定
        //--------------------------------
        if (sortInfo != null)
        {
            if (sortInfo.m_SortType == (int)MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE_SORT)
            {
                // お好みソート
                if (sortInfo != null && sortInfo.m_FavoriteSorts != null)
                {
                    // 使用中優先
                    if (sortInfo.m_FavoriteSortIsUsePriority)
                    {
                        AddSortInfo(SORT_PARAM.PARTY, SORT_ORDER.ASCENDING);
                    }

                    // ソート
                    for (int i = 0; i < sortInfo.m_FavoriteSorts.Length; i++)
                    {
                        SORT_PARAM sortparam = SortUtil.GetSortParam((MAINMENU_SORT_SEQ)sortInfo.m_FavoriteSorts[i].m_SortType);
                        if (sortparam == SORT_PARAM.NONE)
                        {
                            continue;
                        }

                        AddSortInfo(sortparam,
                                    (sortInfo.m_FavoriteSorts[i].m_IsAscOrder) ? SORT_ORDER.ASCENDING : SORT_ORDER.DESCENDING);
                    }
                }
            }
            else if (sortInfo.m_SortType != (int)MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_DEFAULT)
            {
                // 通常ソート
                SORT_PARAM sortparam = SortUtil.GetSortParam((MAINMENU_SORT_SEQ)sortInfo.m_SortType);
                if (sortparam != SORT_PARAM.NONE)
                {
                    AddSortInfo(sortparam, (sortInfo.m_SortIsAscOrder) ? SORT_ORDER.ASCENDING : SORT_ORDER.DESCENDING);
                }
            }
        }

        //--------------------------------
        // フィルタの設定
        //--------------------------------
        if (sortInfo != null)
        {
            // レア度
            if (SortUtil.CheckFilterWorking(sortInfo.m_FilterRares))
            {
                AddFilter<int>(SORT_PARAM.RARITY, sortInfo.m_FilterRares);
            }

            // 属性
            if (SortUtil.CheckFilterWorking(sortInfo.m_FilterElements))
            {
                AddFilter<int>(SORT_PARAM.ELEMENT, SortUtil.GetFilterNumElements(sortInfo.m_FilterElements));
            }

            // 種族
            if (SortUtil.CheckFilterWorking(sortInfo.m_FilterKinds))
            {
                if (sortInfo.m_FilterIsIncludeKindsSub)
                {
                    AddFilter<int>(new SORT_PARAM[] { SORT_PARAM.KIND, SORT_PARAM.SUB_KIND }, SortUtil.GetFilterNumKindss(sortInfo.m_FilterKinds));
                }
                else
                {
                    AddFilter<int>(SORT_PARAM.KIND, SortUtil.GetFilterNumKindss(sortInfo.m_FilterKinds));
                }
            }
        }
    }
}
