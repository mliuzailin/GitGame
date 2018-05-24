using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class PP
{
    public PP()
    { }

    public PP(int masterId)
    {
        MasterId = masterId;
    }

    public JsonDAO GetJsonDAO(string key)
    {
        return GetStr(key).ToJsonDAO();
    }

    public List<JsonDAO> GetJsonDAOList(string key)
    {
        List<JsonDAO> list = new List<JsonDAO>();
        string str = GetStr(key);
        if (str.IsNullOrEmpty())
        {
            return list;
        }

#if BUILD_TYPE_DEBUG
        Debug.Log("KEY:" + key + " STR:" + str);
#endif
        foreach (object obj in MiniJSON.Json.Deserialize(str) as List<object>)
        {
            JsonDAO jdao = JsonDAO.Create(obj);
            list.Add(jdao);
        }

        return list;
    }

    public void SetJsonDAOList(string key, List<JsonDAO> list)
    {
        string json = "[" + string.Join(",", list.Select(jdao => jdao.ToJson()).ToArray()) + "]";
#if BUILD_TYPE_DEBUG
        Debug.Log("JDAOLIST:" + json);
#endif
        SetStr(key, json);
    }

    public void SetJsonDAO(string key, JsonDAO jdao)
    {
        SetStr(key, jdao.ToJson());
    }


    public string ConvertToJson()
    {
        return MiniJSON.Json.Serialize(ConvertToDict());
    }

    public virtual Dictionary<string, object> ConvertToDict()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        foreach (string key in AllKey)
        {
            dict.Add(key, PlayerPrefs.GetString(key));
        }

        return dict;
    }

    public bool IsExits
    {
        get
        {
            return AllKey.Any(key => PlayerPrefs.HasKey(key));
        }
    }

    public int MasterId
    {
        get;
        set;
    }

    public bool GetBool(string key, bool defaultV = false)
    {
        int d = (defaultV) ? 1 : 0;
        return GetInt(key, d) == 1;
    }

    public void SetBool(string key, bool v)
    {
        SetInt(key, (v) ? 1 : 0);
    }

    public string GetStr(string key, string defaultV = null)
    {
        return PlayerPrefs.GetString(key, defaultV);
    }

    public DateTime GetDateTime(string key)
    {
        //		Debug.LogError ("HIT_CA " );
        string def = DateTime.Now.ToString();
        //		Debug.LogError ("HIT_CB:"+def );

        string getd = GetStr(key, def);
        //		Debug.LogError ("HIT_CD:"+getd );

        DateTime result = getd.ConvertToDateTime();

        return result;
    }

    public void SetDateTime(string key, DateTime v)
    {
        SetStr(key, v.Format());
    }

    public void SetStr(string key, string v)
    {
        PlayerPrefs.SetString(key, v);
    }

    public float GetFloat(string key, float defaultV = 0)
    {
        return float.Parse(PlayerPrefs.GetString(GetKey(key, MasterId), defaultV.ToString()));
    }

    public void SetFloat(string key, float v)
    {
        PlayerPrefs.SetString(GetKey(key, MasterId), v.ToString());
    }


    public int GetInt(string key, int defaultV = 0)
    {
        return PlayerPrefs.GetInt(GetKey(key, MasterId), defaultV);
    }

    public void SetInt(string key, int v)
    {
        PlayerPrefs.SetInt(GetKey(key, MasterId), v);
    }

    protected string GetKey(string keyFormat)
    {
        if (!keyFormat.Contains("{0}"))
        {
            return keyFormat;
        }

        return string.Format(keyFormat, MasterId);
    }

    protected string GetKey(string keyFormat, int masterId)
    {
        if (!keyFormat.Contains("{0}"))
        {
            return keyFormat;
        }

        return string.Format(keyFormat, masterId);
    }

    public void SetVector3(string key, Vector3 v)
    {
        string str = string.Format("{0},{1},{2}", v.x, v.y, v.z);
        PlayerPrefs.SetString(key, str);
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public Vector3 GetVecto3(string key)
    {
        //		string str = string.Format("{0},{1},{2}",v.x,v.y,v.z);
        if (!PlayerPrefs.HasKey(key))
        {
            return Vector3.zero;
        }
        string[] strs = PlayerPrefs.GetString(key).Split(',');

        return new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[2]));
    }


    public void SetStrList(string key, List<string> list)
    {
        string str = string.Join(",", list.ToArray());
        PlayerPrefs.SetString(key, str);
    }

    public List<string> GetStrList(string key)
    {
        string v = PlayerPrefs.GetString(key, "");
        if (v.Length == 0)
        {
            return new List<string>();
        }
        return v.Split(',').ToList();
    }


    public void AddStrList(string key, string v, bool distinct = false)
    {
        List<string> list = GetStrList(key);
        if (!distinct ||
            !list.Contains(v))
        {
            list.Add(v);
        }
        SetStrList(key, list);
    }


    public void AddIntList(string key, int v, bool distinct = false)
    {
        List<int> list = GetIntList(key);
        if (!distinct ||
            !list.Contains(v))
        {
            list.Add(v);
        }
        SetIntList(key, list);
    }

    public int PopIntList(string key)
    {
        List<int> list = GetIntList(key);
        if (list.Count == 0)
        {
            return -1;
        }

        SetIntList(key, list.GetRange(1, list.Count - 1));
        return list[0];
    }

    public void RemoveIntList(string key, int v)
    {
        List<int> list = GetIntList(key);
        if (list.Contains(v))
        {
            list.Remove(v);
        }
        SetIntList(key, list);
    }

    public void SetIntList(string key, List<int> list)
    {
        string str = String.Join(",", list.Select(i => i.ToString()).ToArray());
        PlayerPrefs.SetString(key, str);
    }

    public List<int> GetIntList(string key)
    {
        string v = PlayerPrefs.GetString(key, "");
        if (v.Length == 0)
        {
            return new List<int>();
        }
        return v.Split(',').Select(istr => int.Parse(istr)).ToList();
    }

    public virtual void Save()
    {
        PlayerPrefs.Save();
    }

    public void DeleteAll()
    {
        foreach (string key in AllKey)
        {
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
    }

    public virtual List<string> AllKey
    {
        get
        {
            return AllKeyFormat.Select(keyFormat => string.Format(keyFormat, MasterId)).ToList();
        }
    }

    public virtual List<string> AllKeyFormat
    {
        get
        {
            // Debug.LogError("T");
            // return this.GetType().GetFields().Where(fi => fi.IsStatic).Where(fi => fi.Name.EndsWith("_KEY") || fi.Name.EndsWith("_KEYFORMAT")) .Select(fi => fi.Name).ToList();
            return this.GetType().GetFields().Where(fi => fi.IsStatic).Where(fi => fi.Name.EndsWith("_KEY") || fi.Name.EndsWith("_KEYFORMAT")).Select(fi => fi.GetValue(this).ToString()).ToList();
            // return new List<string>();
        }
    }
}
