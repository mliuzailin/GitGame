using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using M4u;
using SQLite.Attribute;

[System.Serializable]
public class Master : M4uContext
{
    [SerializeField]
    [PrimaryKey]
    public uint fix_id
    {
        get;
        set;
    }

    public virtual uint tag_id
    {
        get;
        set;
    }

    public void Copy(Master src)
    {
        fix_id = src.fix_id;
        tag_id = src.tag_id;
    }

    public string ConvertToJson()
    {
        return MiniJSON.Json.Serialize(ConvertToDict());
    }


    public virtual Dictionary<string, object> ConvertToDict()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        //		dict.Add ("master_id", MasterId);
        foreach (FieldInfo fi in this.GetType().GetFields())
        {
            // Debug.LogError("FI:" + fi.Name + " V:" + fi.GetValue(this));
            if (dict.ContainsKey(fi.Name))
            {
                continue;
            }
            object obj = fi.GetValue(this);

            if (obj == null)
            {
                continue;
            }

            dict.Add(fi.Name, obj.ToString());
        }
        return dict;
    }


    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void FromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }

    //objと自分自身が等価のときはtrueを返す
    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }
        Master c = (Master)obj;

        return (this.fix_id.Equals(c.fix_id));
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
