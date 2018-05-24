using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static class MasterExtensions
{
    //マスタクラス名からテーブル名を導出する
    public static string GetTableName(this Type t)
    {
        return DivRNUtil.GetTableName(t.Name);
    }
}

public class MasterFinder<T>
    where T : Master, new()
{
    private Dictionary<string, Dictionary<int, T>> singleCache = new Dictionary<string, Dictionary<int, T>>();
    private Dictionary<string, List<T>> allCache = new Dictionary<string, List<T>>();
    private static MasterFinder<T> instance = new MasterFinder<T>();

    public static MasterFinder<T> Instance
    {
        get
        {
            return instance;
        }
    }

    public T[] GetAll()
    {
        return FindAll().ToArray();
    }

    public bool IsCache()
    {
        return allCache.ContainsKey(TableName);
    }

    public List<T> FindAll()
    {
        //キャッシュがなければ、キャッシュを作る
        if (!allCache.ContainsKey(TableName))
        {
#if BUILD_TYPE_DEBUG
            //UnityEngine.Debug.Log("RemoveCache create allCache TableName: " + TableName);
#endif
            List<T> list = SQLiteClient.Instance.SelectAll<T>(TableName);
            if (list == null)
            {
                return new List<T>();
            }
            else
            {
                allCache.Add(TableName, list);
            }
        }

        return allCache[TableName];
    }

    public void RemoveCache()
    {
        if (singleCache.Count() > 0)
        {
#if BUILD_TYPE_DEBUG
            //UnityEngine.Debug.Log("RemoveCache singleCache TableName: " + TableName);
#endif
            singleCache[TableName].Clear();
            singleCache.Clear();
        }

        if (allCache.Count() > 0)
        {
#if BUILD_TYPE_DEBUG
            //UnityEngine.Debug.Log("RemoveCache allCache TableName: " + TableName);
#endif
            allCache[TableName].Clear();
            allCache.Clear();
        }
    }

    public List<T> SelectWhere(String WhereText, params object[] args)
    {
        List<T> result = null;
        result = SQLiteClient.Instance.SelectWhere<T>(TableName, WhereText, args);
        return result;
    }

    public T SelectFirstWhere(String WhereText, params object[] args)
    {
        T result = null;
        result = SQLiteClient.Instance.SelectFirstWhere<T>(TableName, WhereText, args);
        return result;
    }

    public SimpleSQL.SimpleDataTable QueryGeneric(string QueryText, params object[] args)
    {
        SimpleSQL.SimpleDataTable result = SQLiteClient.Instance.QueryGeneric(QueryText, args);
        return result;
    }
    public SimpleSQL.SimpleDataTable SelectCountWhere(string WhereText, params object[] args)
    {
        SimpleSQL.SimpleDataTable result = SQLiteClient.Instance.QueryGeneric(string.Format("select count(*) as count from {0} {1}", TableName, WhereText), args);
        return result;
    }
    public SimpleSQL.SimpleDataTable SelectColumnWhere(string ColumnText, string WhereText, params object[] args)
    {
        SimpleSQL.SimpleDataTable result = SQLiteClient.Instance.QueryGeneric(string.Format("select{0} from {1} {2}", ColumnText, TableName, WhereText), args);
        return result;
    }
    public T FindCache(int fix_id)
    {
        T result = null;

        if (allCache.ContainsKey(TableName))
        {
            return FindAll().FirstOrDefault(t => t.fix_id == fix_id);
        }

        if (singleCache.ContainsKey(TableName))
        {
            singleCache[TableName].TryGetValue(fix_id, out result);
        }

        return result;
    }

    public T Find(int fix_id)
    {
        T result = FindCache(fix_id);

        if (result != null)
        {
            return result;
        }

        result = SQLiteClient.Instance.SelectFirst<T>(TableName, (uint)fix_id);

        if (result == null)
        {
            return null;
        }
#if BUILD_TYPE_DEBUG
        //UnityEngine.Debug.Log("RemoveCache create singleCache TableName: " + TableName);
#endif
        singleCache.AddIfNotExists(TableName, new Dictionary<int, T>());
        singleCache[TableName].Add(fix_id, result);
        return result;
    }

    public T Find(string first)
    {
        return Find(int.Parse(first));
    }

    private string TableName
    {
        get
        {
            return typeof(T).GetTableName();
        }
    }
}
