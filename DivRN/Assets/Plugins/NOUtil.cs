using UnityEngine;
using UnityEngine.UI;
using UniExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// using HutongGames.PlayMaker;
using System.Reflection;
using System.Linq;
using System.Text;

//using PathologicalGames;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class NOUtil
{
    public static void AppendCode(string path, string t)
    {
        string allText = File.ReadAllText(path);

        if (allText.Contains(t))
        {
            return;
        }

        int index = allText.LastIndexOf("}");

#if BUILD_TYPE_DEBUG
//        Debug.Log("INDE:" + index);
#endif
        string after = allText.Remove(index, 1);
#if BUILD_TYPE_DEBUG
//        Debug.Log("after:" + after);
#endif
        after += t;
//            after += "void OnReady(){}";
        after += "}";

        File.WriteAllText(path, after);
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(path);
#endif
    }


    public static string FindScriptPath(string scriptName)
    {
 #if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:script " + scriptName.Replace(".cs", ""));
        foreach (string gu in guids)
        {
            string p = AssetDatabase.GUIDToAssetPath(gu);
            if (p.Contains(scriptName + ".cs"))
            {
                return p;
            }

            Debug.Log("P:" + p);
        }
#endif
        return null;
    }

    public static string Platform
    {
        get
        {
            string platform = null;
 #if UNITY_ANDROID
            platform = "Android";
 #elif UNITY_IOS || UNITY_STANDALONE_OSX 
            platform = "iOS";
 #elif UNITY_STANDALONE_WIN
            platform = "Windows";
            //切り替えた場合はコミット時に戻すのを忘れないこと
            //platform = "Android";
 #else
            platform = "Android";
 #endif
            return platform;
        }
    }

    public static bool IsIOS
    {
        get
        {
            return Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }

    public static bool IsIPAD
    {
        get
        {
            #if !UNITY_IOS
            return false;
            #else
            return (bool)SystemInfo.deviceModel.ToLower().Contains("ipad");
            #endif
        }
    }

    static public string GetTypeName<T>()
    {
        string s = typeof(T).ToString();
        if (s.StartsWith("UI"))
            s = s.Substring(2);
        else if (s.StartsWith("UnityEngine."))
            s = s.Substring(12);
        return s;
    }

    static public GameObject AddChild(GameObject parent)
    {
        return AddChild(parent, true);
    }

    /// <summary>
    /// Add a new child game object.
    /// </summary>

    static public GameObject AddChild(GameObject parent, bool undo)
    {
        GameObject go = new GameObject();
        #if UNITY_EDITOR
        if (undo)
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
        #endif
        if (parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }

    /// <summary>
    /// Instantiate an object and add it to the specified parent.
    /// </summary>

    static public GameObject AddChild(GameObject parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        #if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
        #endif
        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }


    /// <summary>
    /// Add a child object to the specified parent and attaches the specified script to it.
    /// </summary>

    static public T AddChild<T>(GameObject parent) where T : Component
    {
        GameObject go = AddChild(parent);
        go.name = GetTypeName<T>();
        return go.AddComponent<T>();
    }

    /// <summary>
    /// Add a child object to the specified parent and attaches the specified script to it.
    /// </summary>

    static public T AddChild<T>(GameObject parent, bool undo) where T : Component
    {
        GameObject go = AddChild(parent, undo);
        go.name = GetTypeName<T>();
        return go.AddComponent<T>();
    }


    public static bool IsJailBroken()
    {
        #if UNITY_IOS && !UNITY_EDITOR
		string[] paths = new string[10] {
			"/Applications/Cydia.app",
			"/private/var/lib/cydia",
			"/private/var/tmp/cydia.log",
			"/System/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist",
			"/usr/libexec/sftp-server",
			"/usr/bin/sshd",
			"/usr/sbin/sshd",
			"/Applications/FakeCarrier.app",
			"/Applications/SBSettings.app",
			"/Applications/WinterBoard.app"
		};
		int i;
		bool jailbroken = false;
		for (i = 0; i < paths.Length; i++) {
			if (System.IO.File.Exists (paths [i])) {
				jailbroken = true;
			}            
		}
		return jailbroken;
        #else
        return false;
        #endif
    }


    #if UNITY_EDITOR
    static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

    public static bool isZenkaku(string str)
    {
        int num = sjisEnc.GetByteCount(str);
        return num > str.Length;
    }

    public static bool isHankaku(string str)
    {
        int num = sjisEnc.GetByteCount(str);
        return num == str.Length;
    }
    #endif

    public static IEnumerator TakeScreenShot(Action<Texture2D> action)
    {
        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        action(tex);	
    }


    public static void RemoveClone(this GameObject gameObject)
    {

        gameObject.name = gameObject.name.Replace("(Clone)", "");
    }

    //	public static void MoveIn (this List<GEAnim> list)
    //	{
    //		foreach (GEAnim anim in list) {
    //			anim.enabled = true;
    //			anim.MoveIn ();
    //		}
    //		// list.ForEach(anim => anim.MoveIn());
    //	}
    //
    //	public static void MoveOut (this List<GEAnim> list)
    //	{
    //		list.ForEach (anim => anim.MoveOut ());
    //	}

    public static int ConvertToMB(this long bytenum)
    {
        return (int)(bytenum / 1024 / 1024);
    }

    public static List<T> Flatten<T>(this List<List<T>> list)
    {
        List<T> result = new List<T>();
        foreach (List<T> l  in list)
        {
            result.AddRange(l);
        }
        return result;
    }

    //	public static void Reset (this UIScrollView sv)
    //	{
    //		sv.transform.SetLocalPositionY (0);
    //		sv.GetComponent<UIPanel> ().clipOffset = Vector2.zero;
    //		sv.UpdateScrollbars ();
    ////      sv.bounds = new Bounds(
    //	}
    //
    //	public static GameObject AddChild (GameObject parentGO, string prefabPath)
    //	{
    //
    //		GameObject prefab = Resources.Load (prefabPath) as GameObject;
    //		GameObject go = NGUITools.AddChild (parentGO, prefab);
    //		return go;
    //	}


    public static bool IsExists(Type t)
    {
        return GameObject.FindObjectOfType(t) != null;
    }

    //public static string PasteFromClipboard()
    //{
    //    TextEditor te = new TextEditor();
    //    te.content.text = "";
    //    te.Paste();
    //    return te.content.text;
    //}

    //     public static void CopyToClipboard(string data)
    //     {
    // #if UNITY_EDITOR
    //         UniPasteBoard.SetClipBoardString(data);
    // #elif UNITY_WEBPLAYER
    //         TextEditor te = new TextEditor();
    //         te.content = new GUIContent();
    //         te.SelectAll();
    //         te.Copy();
    // #endif
    //     }


    //	public static void AddTweenScale (GameObject go, Vector3 from, Vector3 to, float duration, UITweener.Style style = UITweener.Style.Once)
    //	{
    //		TweenScale ta = go.AddComponent<TweenScale> ();
    //		ta.from = from;
    //		ta.to = to;
    //		ta.duration = duration;
    //		ta.style = style;
    //		ta.AddOnFinished (() => {
    //
    //			GameObject.Destroy (ta);
    //		});
    ////      ta.onFinished += ()=>{
    ////      };
    //	}

    //	public static void AddTweenAlpha (GameObject go, float from, float to, float duration)
    //	{
    //		TweenAlpha ta = go.AddComponent<TweenAlpha> ();
    //		ta.from = from;
    //		ta.to = to;
    //		ta.duration = duration;
    //		ta.AddOnFinished (() => {
    //
    //			GameObject.Destroy (ta);
    //		});
    ////      ta.onFinished += ()=>{
    ////      };
    //	}
    //
    public static List<int> Range(int start, int end)
    {
        List<int> result = new List<int>();
        if (start < end)
        {

            for (int i = start; i <= end; ++i)
            {
                result.Add(i);
            }

        }
        else
        {

            for (int i = start; i >= end; --i)
            {
                result.Add(i);
            }
        }
        return result;
    }

    //public static string GenerateUniqueID()
    //{
    //    string key = "ID";
    //
    //    var random = new System.Random();
    //    DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
    //    double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;

    //    return Application.systemLanguage//Language
    //    + "-" + Application.platform//Device
    //    + "-" + String.Format("{0:X}", ((int)timestamp)).ToString()//Time
    //    + "-" + String.Format("{0:X}", (int)(Time.time * 1000000)).ToString()//Time in game
    //    + "-" + String.Format("{0:X}", random.Next(1000000000));          //random number
    //}



    public static void ReplacePrefab(GameObject go)
    {
#if UNITY_EDITOR
//プレハブを更新
        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go), ReplacePrefabOptions.ConnectToPrefab);
#endif
    }

    public static string ConvertId_D2(int id)
    {
        return string.Format("{0:D2}", id);
    }

    public static string ConvertId_D4(int id)
    {
        return string.Format("{0:D4}", id);
    }

    //  public static int GetStageId (string goName)
    //  {
    //      return int.Parse (GetStageIdStr (goName));
    //  }
    //
    //public static string GetStageIdStr (string goName)
    //{
    //    return goName.Substring ("Stage".Length, 4);
    //}
    //

    //        public static Vector3 GetLocalEulerAngles (PlayerDirection playerDirection, string radical)
    //        {
    //            switch (playerDirection) {
    //            case PlayerDirection.FORWARD:
    //                return GetForwardLocalEulerAngles (radical);
    //                break;
    //            case PlayerDirection.BACKWARD:
    //                return GetForwardLocalEulerAngles (radical) + new Vector3 (0, 180.0f, 0);
    //                break;
    //            case PlayerDirection.RIGHT:
    //                return GetForwardLocalEulerAngles (radical) + new Vector3 (0, 90.0f, 0);
    //                break;
    //            case PlayerDirection.LEFT:
    //                return GetForwardLocalEulerAngles (radical) + new Vector3 (0, -90.0f, 0);
    //                break;
    //            }
    //            return Vector3.zero;
    //        }
    //
    //        private static Vector3 GetForwardLocalEulerAngles (string radical)
    //        {
    //            switch (radical) {
    //            case "TOP":
    //                return new Vector3 (0, 0, -180);
    //                break;
    //            case "LEFT":
    //                return new Vector3 (0, 0, -90);
    //                break;
    //            case "BOTTOM":
    //                return new Vector3 (0, 0, 0);
    //                break;
    //            case "RIGHT":
    //                return new Vector3 (0, 0, 90);
    //                break;
    //            }
    //            return Vector3.zero;
    //        }

    //public static int CalExp (int level)
    //{
    //    return level * 100;
    //}
    //
    //public static string SerializePoints (List<Vector2> points)
    //{
    //    string result = MiniJSON.Json.Serialize (points);
    //    result = result.Replace (",\"", "|");
    //    result = result.Replace ("(", "");
    //    result = result.Replace (")", "");
    //    result = result.Replace ("0.", "");
    //    result = result.Replace ("[", "");
    //    result = result.Replace ("]", "");
    //    result = result.Replace ("\"", "");
    //    result = result.Replace (" ", "");
    //    result = result.Replace (",", "_");
    //    return result;
    //}
    //
    //public static List<Vector2> DeserializePoints (string pointsStr)
    //{
    //    List<Vector2> result = new List<Vector2> ();
    //    Log.Minor ("POINT_STR:" + pointsStr);
    //    foreach (string pointStr in pointsStr.Split('|')) {
    //        string[] points = pointStr.Split ('_');
    //        result.Add (new Vector2 ((float)Convert.ToDouble (points [0]) / 10.0f, (float)Convert.ToDouble (points [1]) / 10.0f));
    //    }
    //    return result;
    //}
    //
    //public static List<int> SplitIntList (string str)
    //{
    //    return SplitIntList (str, ',');
    //}
    //
    //public static List<int> SplitIntList (string str, char delimiter)
    //{
    ////          Log.Minor ("CALL Util#SplitIntList: " + str);
    //    if (!str.Contains (delimiter)) {
    //        return new List<int> (){Convert.ToInt32 (str)};
    //    }
    //    return str.Split (delimiter).Select (s => Convert.ToInt32 (s)).ToList ();
    //}
    //
    //public static string JoinIntList (List<int> list)
    //{
    //    string result = null;
    //    foreach (int i in list) {
    //        if (result == null) {
    //            result = i.ToString ();
    //        } else {
    //            result += "," + i.ToString ();
    //        }
    //    }
    //    return result;
    //}
    //
    public static bool CanConnectInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    //
    //public static DateTime Now {
    //    get {
    //        return NOUtil.Now;
    //    }
    //}
    //
    ////      public int ConvertToInt
    //public static float DiffPlayerPosition (Transform transform)
    //{
    //    return transform.position.z - Player.Instance.transform.position.z;
    //}
    //
    //
    public static T TakeByWeight<T>(this List<T> list) where T : IWeightable
    {
        List<int> keys = new List<int>();
        List<T> values = new List<T>();

        int sum = 0;

        foreach (T t in list)
        {
            if (t.GetWeight() <= 0)
            {
//                  Log.MinorWarning ("CAN NOT ADD PROB:" + t.GetWeight ());
                continue;
            }
            sum += t.GetWeight();
            keys.Add(sum);
            values.Add(t);
        }
        int random = UnityEngine.Random.Range(0, sum);
        for (int i = 0; i < keys.Count; ++i)
        {
            if (random < keys[i])
            {
                return values[i];
            }
        }
        return default(T);
    }
    //
    //public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    //{
    //    HashSet<TKey> seenKeys = new HashSet<TKey> ();
    //    foreach (TSource element in source) {
    //        if (seenKeys.Add (keySelector (element))) {
    //            yield return element;
    //        }
    //    }
    //}
    //
    //public static Vector3 SetX (Vector3 t, float x)
    //{
    //    return new Vector3 (x, t.y, t.z);
    //}
    //
    //public static Vector3 SetY (Vector3 t, float y)
    //{
    //    return new Vector3 (t.x, y, t.z);
    //}
    //
    //public static Vector3 SetZ (Vector3 t, float z)
    //{
    //    return new Vector3 (t.x, t.y, z);
    //}
    //
    //public static Vector3 Round (Vector3 t)
    //{
    //    return new Vector3 ((float)Math.Round (t.x), (float)Math.Round (t.y), (float)Math.Round (t.z));
    ////      return t;
    //}
    //
    //private static Dictionary<int,string> _textDict;
    ///// <summary>
    ///// Start the tweening process.
    ///// </summary>
    //
    //public static GameObject FindActiveGameObjectWithTag (string tag)
    //{
    //    return GameObject.FindGameObjectsWithTag (tag).ToList ().Find (g => g.tag.Equals (tag));
    //}
    //
    //public static Dictionary<int,string> textDict {
    //    get {
    //        if (_textDict == null) {
    //            // テキスト読込
    //            TextAsset text = Resources.Load ("Text/op12_text", typeof(TextAsset)) as TextAsset;
    //            if (text == null) {
    //                Log.Minor ("Text init: TextAsset load error!");
    //            } else {
    //                // テキストをパース
    //                _textDict = new Dictionary<int,string> ();
    //                string[] text_pairs = text.text.Split ('|');
    //                foreach (string text_pair in text_pairs) {
    //                    string[] pair = text_pair.Split (',');
    //                    _textDict.Add (int.Parse (pair [0]), pair [1]);
    //                }
    //            }
    //        }
    //        return _textDict;
    //    }
    //
    //}
    //
    //public static int getTextIdByText (string text)
    //{
    //    // 有効な番号かチェック
    //    if (!textDict.ContainsValue (text)) {
    //        Log.Minor ("Text getText: Invalid text!:" + text);
    //        return -1;
    //    }
    //
    //    foreach (int key in textDict.Keys) {
    //        if (text.Equals (textDict [key])) {
    //            return key;
    //        }
    //    }
    //
    //    return -1;
    //}
    //
    //
    //// 取得
    //public static string getTextByTextId (int textId)
    //{
    //    // 有効な番号かチェック
    //    if (!textDict.ContainsKey (textId)) {
    //        Log.Minor ("Text getText: Invalid number! textId:" + textId);
    //        return "";
    //    }
    //
    //    return textDict [textId];
    //}
    //
    //public static List<int> HalfOfRandomInts (int max)
    //{
    //    List<int> list = new List<int> ();
    //
    //    for (int i=0; i<max; ++i) {
    //        list.Add (i);
    //    }
    //
    //    return new List<int> (Shuffle<int> (list.ToArray ())).GetRange (0, (int)Math.Floor (max / 2.0d));
    //}
    //
    //public static List<int> HalfOfRandomInt (int size)
    //{
    //    List<int> result = new List<int> ();
    //
    //    for (int i=0; i<size; ++i) {
    //        result.Add (i);
    //    }
    //
    //    result = new List<int> (Shuffle<int> (result.ToArray ())).GetRange (0, (int)Math.Ceiling (size / 2.0d));
    //    result.Sort ();
    //    return result;
    //}
    //
    //private static int seed = Environment.TickCount;
    ////  static System.Random _random = new System.Random ();
    //
    //
    //public static UIAtlas InstantiateUIAtlas (string atlasName)
    //{
    //    Log.Minor ("ATLAS_NAME:" + atlasName);
    //    return (GameObject.Instantiate (Resources.Load (atlasName)) as GameObject).GetComponent<UIAtlas> ();
    //}
    //
    //public static IList<T> Shuffle<T> (this IList<T> list)
    //{
    //    System.Random rng = new System.Random ();
    //    int n = list.Count;
    //    while (n > 1) {
    //        n--;
    //        int k = rng.Next (n + 1);
    //        T value = list [k];
    //        list [k] = list [n];
    //        list [n] = value;
    //    }
    //    return list;
    //}
    //
    //public static T[] Shuffle<T> (T[] array)
    //{
    //    var random = new System.Random (seed++);
    //    for (int i = array.Length; i > 1; i--) {
    //        // Pick random element to swap.
    //        int j = random.Next (i); // 0 <= j <= i-1
    //        // Swap.
    //        T tmp = array [j];
    //        array [j] = array [i - 1];
    //        array [i - 1] = tmp;
    //    }
    //    return array;
    //}
    //
    //public static GameObject FindChild (GameObject parent, string childName)
    //{
    //    foreach (Transform transform in parent.transform) {
    //        if (transform.gameObject.name == childName) {
    //            return transform.gameObject;
    //        }
    //    }
    //    return null;
    //
    //}
    //
    //public static int CountString (string target, string countTarget)
    //{
    //    return target.Length - target.Replace (countTarget, "").Length;
    //}
    //
    //public static bool IsNumeric (string target)
    //{
    //    try {
    //        double.Parse (target);
    //    } catch {
    //        return false;
    //    }
    //
    //    return true;
    //}
    /// Author: Daniele Giardini - http://www.demigiant.com/

    public static float linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    public static float clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else
            retval = start + (end - start) * value;
        return retval;
    }

    public static float spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static float easeInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static float easeOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    public static float easeInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1)
            return end / 2 * value * value + start;
        value--;
        return -end / 2 * (value * (value - 2) - 1) + start;
    }

    public static float easeInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float easeOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    public static float easeInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1)
            return end / 2 * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value + 2) + start;
    }

    public static float easeInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    public static float easeOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }

    public static float easeInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1)
            return end / 2 * value * value * value * value + start;
        value -= 2;
        return -end / 2 * (value * value * value * value - 2) + start;
    }

    public static float easeInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public static float easeOutQuint(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value * value * value + 1) + start;
    }

    public static float easeInOutQuint(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1)
            return end / 2 * value * value * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value * value * value + 2) + start;
    }

    public static float easeInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
    }

    public static float easeOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
    }

    public static float easeInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
    }

    public static float easeInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
    }

    public static float easeOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
    }

    public static float easeInOutExpo(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1)
            return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    public static float easeInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
    }

    public static float easeOutCirc(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * Mathf.Sqrt(1 - value * value) + start;
    }

    public static float easeInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1)
            return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }

    /* GFX47 MOD START */
    public static float easeInBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        return end - easeOutBounce(0, end, d - value) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //public static float bounce(float start, float end, float value){
    public static float easeOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < (1 / 2.75f))
        {
            return end * (7.5625f * value * value) + start;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return end * (7.5625f * (value) * value + .75f) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return end * (7.5625f * (value) * value + .9375f) + start;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    public static float easeInOutBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        if (value < d / 2)
            return easeInBounce(0, end, value * 2) * 0.5f + start;
        else
            return easeOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
    }
    /* GFX47 MOD END */

    public static float easeInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1;
        float s = 1.70158f;
        return end * (value) * value * ((s + 1) * value - s) + start;
    }

    public static float easeOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value = (value / 1) - 1;
        return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
    }

    public static float easeInOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;
        if ((value) < 1)
        {
            s *= (1.525f);
            return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
        }
        value -= 2;
        s *= (1.525f);
        return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
    }

    public static float punch(float amplitude, float value)
    {
        float s = 9;
        if (value == 0)
        {
            return 0;
        }
        if (value == 1)
        {
            return 0;
        }
        float period = 1 * 0.3f;
        s = period / (2 * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }

    /* GFX47 MOD START */
    public static float easeInElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0)
            return start;

        if ((value /= d) == 1)
            return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //public static float elastic(float start, float end, float value){
    public static float easeOutElastic(float start, float end, float value)
    {
        /* GFX47 MOD END */
        //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0)
            return start;

        if ((value /= d) == 1)
            return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
    }

    /* GFX47 MOD START */
    public static float easeInOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0)
            return start;

        if ((value /= d / 2) == 2)
            return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        if (value < 1)
            return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
    }
    /* GFX47 MOD END */

}
