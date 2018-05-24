//----------------------------------------------
// MVVM 4 uGUI
// © 2015 yedo-factory
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace M4u
{
    /// <summary>
    /// M4uCollectionBinding. Bind Collection
    /// </summary>
    [AddComponentMenu("M4u/CollectionBinding")]
    public class M4uCollectionBinding : M4uBindingSingle
    {
        public GameObject Data;
        public string SavePath;
        public string OnChanged;
        public bool namedIndex;
        /// <summary>GridLayutGroupのCellSizeをプレハブのサイズに合わせる</summary>
        public bool SetGridCellSize = false;

        private bool isChange = false;
        private Action onChanged = null;
        private ICollection saveCollection = null;
        private List<GameObject> saveGos = new List<GameObject>();
        private List<object> saveObjs = new List<object>();

        private GameObject m_ListItemPool = null;

        public override void Awake()
        {
            base.Awake();

            //----------------------------------------------------
            // GridLayutGroupのCellSizeをプレハブのサイズに合わせる
            //----------------------------------------------------
            if (SetGridCellSize == true)
            {
                RectTransform rect = Data.GetComponent<RectTransform>();
                GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
                if (rect != null && grid != null)
                {
                    grid.cellSize = new Vector2(rect.rect.width, rect.rect.height);
                }
            }

        }

        public override void Start()
        {
            base.Start();
            m_ListItemPool = GameObject.Find("/ListItemPool");

            if (!string.IsNullOrEmpty(SavePath))
            {
                string[] names = SavePath.Split('.');
                object parent = Root.Context;
                object value = null;
                object obj = null;
                PropertyInfo pi = null;
                FieldInfo fi = null;
                for (int i = 0; i < names.Length; i++)
                {
                    bool isLast = (i == names.Length - 1);
                    string name = names[i];
                    ParseMember(isLast, ref name, ref parent, ref value, ref obj, ref pi, ref fi);
                }
                saveCollection = GetMember<ICollection>(obj, pi, fi);
            }

            if (!string.IsNullOrEmpty(OnChanged))
            {
                string[] names = OnChanged.Split('.');
                string name = "";
                object parent = Root.Context;
                object value = null;
                object obj = null;
                PropertyInfo pi = null;
                FieldInfo fi = null;
                for (int i = 0; i < names.Length; i++)
                {
                    bool isLast = (i == names.Length - 1);
                    name = names[i];
                    ParseMember(isLast, ref name, ref parent, ref value, ref obj, ref pi, ref fi);
                }
                onChanged = (Action)Delegate.CreateDelegate(typeof(Action), parent, name);
            }
            OnChange();
        }

        public override void Update()
        {
            base.Update();

            object value = Values[0];
            if (value != null)
            {
                Type type = value.GetType();
                int count = 0;
                ICollection collection = null;
                if (type.IsPrimitive)
                {
                    if (int.TryParse(value.ToString(), out count) && saveObjs.Count != count)
                    {
                        isChange = true;
                    }
                }
                else
                {
                    collection = value as ICollection;
                    if (collection != null)
                    {
                        if (saveObjs.Count != collection.Count)
                        {
                            isChange = true;
                        }
                        else if (type.IsArray)
                        {
                            var data = (Array)value;
                            for (int i = saveObjs.Count - 1; i >= 0; i--)
                            {
                                if (saveObjs[i] != data.GetValue(i)) { isChange = true; break; }
                            }
                        }
                        else if (value is IList)
                        {
                            var data = (IList)value;
                            for (int i = saveObjs.Count - 1; i >= 0; i--)
                            {
                                if (saveObjs[i] != data[i]) { isChange = true; break; }
                            }
                        }
                    }
                }

                if (isChange)
                {
                    isChange = false;

                    foreach (var go in saveGos)
                    {
                        Destroy(go);
                    }
                    saveGos.Clear();
                    saveObjs.Clear();

                    var saveList = (saveCollection != null) ? (saveCollection as IList) : null;
                    var saveDic = (saveCollection != null && saveList == null) ? (saveCollection as IDictionary) : null;
                    if (saveList != null) { saveList.Clear(); }
                    if (saveDic != null) { saveDic.Clear(); }
                    if (type.IsPrimitive)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            CreateData(i, saveList, saveDic);
                        }
                    }
                    else if (collection != null)
                    {
                        foreach (var obj in collection)
                        {
                            CreateData(obj, saveList, saveDic);
                        }
                    }

                    if (onChanged != null) { onChanged(); }
                }
            }
        }

        private GameObject CreateData(object obj, IList saveList, IDictionary saveDic)
        {
            bool isFromPool = false;
            GameObject go = null;
            if (m_ListItemPool != null && Data.name == "UnitListItem")
            {
                if (m_ListItemPool.transform.childCount > 0)
                {
                    go = m_ListItemPool.transform.GetChild(0).gameObject;
                    var rt = go.GetComponent<M4uContextRoot>();
                    rt.Context = (M4uContextInterface)obj;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.eulerAngles = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    isFromPool = true;
                }
                else
                {
                    go = (GameObject)Instantiate(Data);
                    go.name = Data.name;
                }
            }
            else
            {
                go = (GameObject)Instantiate(Data);
            }
            var root = go.GetComponent<M4uContextRoot>();
            var pos = go.transform.localPosition;
            var rot = go.transform.eulerAngles;
            var scale = go.transform.localScale;
            go.transform.SetParent(transform);
            go.transform.localPosition = pos;
            go.transform.eulerAngles = rot;
            go.transform.localScale = scale;
            if (root != null && root.Context == null && (obj is M4uContextInterface))
            {
                root.Context = (M4uContextInterface)obj;
            }
            if (namedIndex)
            {
                go.name = string.Format("{0:D2}", (transform.childCount));
            }
            if (isFromPool == true)
            {
                go.SendMessage("Refresh");
            }

            saveGos.Add(go);
            saveObjs.Add(obj);
            if (saveList != null) { saveList.Add(go); }
            else if (saveDic != null) { saveDic.Add(obj, go); }
            return go;
        }

        public override void OnChange()
        {
            base.OnChange();
            isChange = true;
        }

        public override string ToString()
        {
            return "Collection=" + GetBindStr(Path) + "/" + ((Data != null) ? Data.name : "None");
        }
    }
}
