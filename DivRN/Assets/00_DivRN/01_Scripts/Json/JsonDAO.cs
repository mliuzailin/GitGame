using UnityEngine;
using System.Collections;
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;

public class JsonDAO
{

    public T Create<T>() where T : new()
    {
        T t = new T();
        //		foreach (string k in Dict.Keys) {
        //
        //			Debug.Log ("k:" + k + " v:" + Dict [k]);
        //		}
        //
        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            //			Debug.Log ("pr:" + prop.Name);
            if (!Dict.ContainsKey(prop.Name))
            {
                Debug.Log("NOT_CONTAINS:" + prop.Name);
                continue;
            }
            //			Debug.Log ("V:" + Dict [prop.Name]);
            //			switch (Type.GetTypeCode (prop.PropertyType)) {
            //			case TypeCode.Int16:
            //			case TypeCode.Int32:
            //			case TypeCode.Int64:
            //				Debug.Log ("int");
            //				break;
            //			default:
            //				prop.SetValue (t, Dict [prop.Name], null);
            //				break;
            //			}
            //			prop.SetValue (t, Dict [prop.Name], null);
            //			Debug.Log ("PROPERTY_TYPE:" + prop.PropertyType);
            //			try{
            //				
            try
            {
                prop.SetValue(t, System.Convert.ChangeType(Dict[prop.Name], prop.PropertyType), null);
            }
            catch (Exception e)
            {
                Debug.Log("CANNNOT_CHANGE_TYPE:" + Dict[prop.Name].GetType().ToString());
                Debug.Log("CANNNOT_CHANGE_TYPE:" + Dict[prop.Name]);
                Debug.Log("Exception:" + e.ToString());
                //				prop.SetValue (t, System.Convert.ChangeType (Dict [prop.Name], prop.PropertyType), null);
            }
        }

        return t;
    }

    public void Compact()
    {
        List<string> list = new List<string>();

        foreach (string key in Dict.Keys)
        {
            if (Dict[key].ToString().IsNullOrEmpty())
            {
                list.Add(key);
            }
        }

        list.ForEach(key => Dict.Remove(key));
    }

    public string ToJson()
    {
        return MiniJSON.Json.Serialize(Dict);
    }

    public Dictionary<string, object> Dict
    {
        get;
        set;
    }

    public List<string> Keys
    {
        get
        {
#if BUILD_TYPE_DEBUG
            //			Debug.Log("CHCH");
#endif
            return Dict.Keys.ToList();
        }
    }

    public void PrintKeys()
    {
        foreach (string k in Dict.Keys)
        {
            Debug.Log("KEY:" + k + " V:" + Dict[k].ToString());
        }
    }


    public static JsonDAO Create(string json)
    {
        return Create(MiniJSON.Json.Deserialize(json) as Dictionary<string, object>);
    }

    public static JsonDAO Create(object obj)
    {
        return Create(obj as Dictionary<string, object>);
    }

    public JsonDAO GetJsonDAO(string key)
    {
        if (!HasKey(key))
        {
            return null;
        }

        return Create(GetDict(key));
    }

    public static JsonDAO Create(Dictionary<string, object> d)
    {
        JsonDAO jdao = new JsonDAO();
        jdao.Dict = d;
        return jdao;
    }

    public static JsonDAO Parse(string json)
    {
        // Debug.LogError("PARSE:" +json);
        JsonDAO jdao = new JsonDAO();
        jdao.Dict = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
        return jdao;
    }

    public List<string> GetStrList(string key)
    {
        if (!HasKey(key))
        {
            return new List<string>();
        }
        return GetList(key).Select(o => o.ToString()).ToList();
    }

    public List<object> GetList(string key)
    {
        if (!HasKey(key))
        {
            return new List<object>();
        }

        return Dict[key] as List<object>;
    }

    public Dictionary<string, object> GetDict(string key)
    {
        if (!HasKey(key))
        {
            return null;
        }

        return Dict[key] as Dictionary<string, object>;
    }

    public bool HasKey(string key)
    {
        if (key == null)
        {
            return false;
        }
        if (Dict == null)
        {
            return false;
        }

        if (!Dict.ContainsKey(key))
        {
            return false;
        }

        object v = Dict[key];

        if (v == null)
        {
            return false;
        }

        if (v.ToString().IsNullOrEmpty())
        {
            return false;
        }

        return true;
    }

    public DateTime GetDateTime(string key)
    {
        return DateTime.Parse(GetStr(key));
    }

    public float GetFloat(string key)
    {
        if (!HasKey(key))
        {
            return 0;
        }
        return float.Parse(Dict[key].ToString());
    }

    public bool GetBool(string key)
    {

        if (!HasKey(key))
        {
            return false;
        }

        string str = GetStr(key).ToLower();

        if (str.Equals("true"))
        {

            return true;

        }
        else if (str.Equals("false"))
        {

            return false;
        }

        return GetInt(key) == 1;
    }

    public string GetStrOrFourDigit(string key)
    {
        if (!HasKey(key))
        {
            return null;
        }
        string str = GetStr(key);
        if (str.IsInt())
        {
            return string.Format("{0:D4}", int.Parse(str));
        }
        return str.ToUpper();
    }

    public int GetInt(string key, int defaultV = 0)
    {
        if (!HasKey(key))
        {
            return defaultV;
        }
        string str = Dict[key].ToString();
        if (!str.IsInt())
        {
            return defaultV;
        }

        if (str.Length == 0)
        {
            return defaultV;
        }
#if BUILD_TYPE_DEBUG
        //		Debug.Log("STR:"+str);
#endif
        return int.Parse(str);
    }

    public string GetStr(string key)
    {
        if (!HasKey(key))
        {
            return "";
        }
        return Dict[key].ToString();
    }

    public List<string> GetListFromStrs(string key)
    {
        if (!HasKey(key))
        {
            return new List<string>();
        }

        string str = GetStr(key);
        if (str == null || str.Length == 0)
        {
            return new List<string>();
        }

        return str.Split(',').ToList();
    }

    public List<int> GetListFromIds(string key)
    {
        return GetListFromStrs(key).Select(str => int.Parse(str)).ToList();
    }
}
