using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public static class NOExtension
{
    public static uint ConvertToTimingUInt(this string timing)
    {
        return (uint)timing.Split(':')[0].Replace("-", "").Replace(" ", "").ToInt(0);
    }

    public static string SnakeCase(this string word)
    {
        var snake = Regex.Replace(word, "([A-Za-z])([0-9]+)", "$1_$2");
        snake = Regex.Replace(snake, "([0-9]+)([A-Za-z])", "$1_$2");
        snake = Regex.Replace(snake, "([A-Z]+)([A-Z][a-z])", "$1_$2");
        snake = Regex.Replace(snake, "([a-z]+)([A-Z])", "$1_$2");
        return snake.ToLower();

    }

    /// <summary>
    /// スネークケースをアッパーキャメル(パスカル)ケースに変換します
    /// 例) quoted_printable_encode → QuotedPrintableEncode
    /// </summary>
    public static string SnakeToUpperCamel(this string self)
    {
        if (string.IsNullOrEmpty(self))
        {
            return self;
        }

        return self
            .Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
            .Aggregate(string.Empty, (s1, s2) => s1 + s2);
    }

    /// <summary>
    /// スネークケースをローワーキャメル(キャメル)ケースに変換します
    /// 例) quoted_printable_encode → quotedPrintableEncode
    /// </summary>
    public static string SnakeToLowerCamel(this string self)
    {
        if (string.IsNullOrEmpty(self))
        {
            return self;
        }

        return self.SnakeToUpperCamel().Insert(0, char.ToLowerInvariant(self[0]).ToString()).Remove(1, 1);
    }
	public readonly static DateTime UnixEpoch = new System.DateTime (1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);

	public static bool IsImplement (this PropertyInfo p, Type type)
	{
		return type.IsAssignableFrom (p.PropertyType);
	}

	public static JsonDAO ToJsonDAO (this string json)
	{
		return JsonDAO.Create (json);
	}


	public static long ConvertUT (this DateTime dateTime)
	{
		DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc (dateTime);
		return (long)(utcDateTime - UnixEpoch).TotalSeconds;
	}

	public static DateTime ConvertDateTime (this long unixTime)
	{
		return UnixEpoch.AddSeconds (unixTime);
	}


	public static Uri ToUri (this string url)
	{
		return new Uri (url);
	}


	public static void Reset (this Type type, System.Object obj)
	{
		foreach (FieldInfo fi in type.GetFields()) {
			if (fi.FieldType == typeof(int)) {
				fi.SetValue (obj, 0);
			} else if (fi.FieldType == typeof(bool)) {
				fi.SetValue (obj, false);
			} else if (fi.FieldType == typeof(string)) {
				fi.SetValue (obj, "");
			}
		}
	}

	public static Vector2 LocalPosition2D (this Transform t)
	{
		return new Vector2 (t.localPosition.x, t.localPosition.y);
	}

	/// <summary>
	/// 最小値を持つ要素を返します
	/// </summary>
	public static TSource FindMin<TSource, TResult> (
		this IEnumerable<TSource> self, 
		Func<TSource, TResult> selector)
	{
		return self.FirstOrDefault (c => selector (c).Equals (self.Min (selector)));
	}

	/// <summary>
	/// 最大値を持つ要素を返します
	/// </summary>
	public static TSource FindMax<TSource, TResult> (
		this IEnumerable<TSource> self, 
		Func<TSource, TResult> selector)
	{
		return self.First (c => selector (c).Equals (self.Max (selector)));
	}



	public static int CompareToMonth (this DateTime dateTime, DateTime dateTime2)
	{
		if (dateTime.Year < dateTime2.Year) {
			return -1;	
		}
		if (dateTime.Year > dateTime2.Year) {
			return 1;	
		}
		int diff = dateTime.Month - dateTime2.Month;

//		Debug.LogError("DD:"+dateTime +" DD2:"+dateTime2 +" DIF:"+diff);

		if (diff < 0) {
			return -1;
		} else if (diff > 0) {
			return 1;
		}
		return 0;
	}


	public static int CompareToDay (this DateTime dateTime, DateTime dateTime2)
	{
		if (dateTime.Year < dateTime2.Year) {
			return -1;	
		}
		if (dateTime.Year > dateTime2.Year) {
			return 1;	
		}
		int diff = dateTime.DayOfYear - dateTime2.DayOfYear;

//		Debug.LogError("DD:"+dateTime +" DD2:"+dateTime2 +" DIF:"+diff);

		if (diff < 0) {
			return -1;
		} else if (diff > 0) {
			return 1;
		}
		return 0;
	}

	public static float ToRadian (this float degree)
	{
		return degree / 180f * Mathf.PI;	
	}

	public static float ToDegree (this float radian)
	{
		return radian * 180f / Mathf.PI;
	}

	//	public static string Localize (this DayOfWeek dow)
	//	{
	//		return Localization.Get(dow.ToString().ToLower());
	//	}


	/// <summary>
	/// 文字が全角なら true を、半角なら false を返します
	/// </summary>
	public static bool IsChar2Byte (char c)
	{
		return !((c >= 0x0 && c < 0x81) || (c == 0xf8f0) || (c >= 0xff61 && c < 0xffa0) || (c >= 0xf8f1 && c < 0xf8f4));
	}

	/// <summary>
	/// バイト数を計算して返します
	/// </summary>
	public static int GetByteCount (this string self)
	{
		int count = 0;
		for (int i = 0; i < self.Length; i++) {
			if (IsChar2Byte (self [i])) {
				count++;
			}
			count++;
		}
		return count;
	}

	/// <summary>
	/// インスタンスからバイト単位で部分文字列を取得します
	/// </summary>
	public static string SubstringInByte (this string self, int byteCount)
	{
		string tmp = "";
		int count = 0;
		for (int i = 0; i < self.Length; i++) {
			char c = self [i];
			if (IsChar2Byte (c)) {
				count++;
			}
			count++;
			if (count > byteCount) {
				break;
			}
			tmp += c;
		}
		return tmp;
	}

	/// <summary>
	/// <para>インスタンスからバイト単位で部分文字列を取得します</para>
	/// <para>文字数が指定されたバイト数以内の場合はインスタンスをそのまま返します</para>
	/// </summary>
	public static string SafeSubstringInByte (this string self, int byteCount)
	{
		return byteCount < self.GetByteCount () ? self.SubstringInByte (byteCount) : self;
	}

	//	public static void MoveIn (this GameObject go)
	//	{
	//		MoveIn (go.GetComponentsInChildren<GEAnim> ());
	//	}
	//
	//	public static void MoveOut (this GameObject go)
	//	{
	//		MoveOut (go.GetComponentsInChildren<GEAnim> ());
	//	}
	//
	//	public static void MoveIn (this GEAnim[] anims)
	//	{
	//
	//		foreach (GEAnim anim in anims) {
	//			if (!anim.tag.Equals ("DisableAutoMoveIn")) {
	//				anim.MoveIn ();
	//			}
	//		}
	//	}
	//
	//	public static void MoveOut (this GEAnim[] anims)
	//	{
	//
	//		foreach (GEAnim anim in anims) {
	//			anim.MoveOut ();
	//		}
	//	}

	public static GameObject FindGameObject (this string n)
	{
#if BUILD_TYPE_DEBUG
		Debug.Log ("CALL FindGameObject:" + n);
#endif
		List<string> list = n.Split ('#').ToList ();
		GameObject result = null;

		foreach (string ch in list) {
			if (result == null) {
				result = GameObject.Find (ch);
			} else {
#if BUILD_TYPE_DEBUG
				Debug.Log ("FindGameObject:" + ch + " result:" + result.name);
#endif
				result = result.transform.Find (ch).gameObject;
			}
		}

		return result;
	}

	//	public static void AddTweenAlpha (this GameObject go, float f, float t, float duration)
	//	{
	//		if (go == null) {
	//			return;
	//		}
	//		UIWidget widget = go.GetComponent<UIWidget> ();
	//		if (widget == null) {
	//			Debug.LogError ("UI_WIDGET_IS_NULL:" + go.name);
	//			return;
	//		}
	//		TweenAlpha ta = widget.gameObject.AddComponent<TweenAlpha> ();
	//		ta.style = TweenAlpha.Style.Once;
	//		ta.duration = duration;
	//		ta.from = f;
	//		ta.to = t;
	//	}
	//
	//	public static void AddPingPongTweenAlpha (this GameObject go)
	//	{
	//		if (go == null) {
	//			return;
	//		}
	//		UIWidget widget = go.GetComponent<UIWidget> ();
	//		if (widget == null) {
	//			Debug.LogError ("UI_WIDGET_IS_NULL:" + go.name);
	//			return;
	//		}
	//		TweenAlpha ta = widget.gameObject.AddComponent<TweenAlpha> ();
	//		ta.style = TweenAlpha.Style.PingPong;
	//		ta.duration = 1f;
	//		ta.from = 0.6f;
	//		ta.to = 1.0f;
	//	}
	//
	//	public static void RemoveTweenAlpha (this GameObject go)
	//	{
	//		if (go == null) {
	//			return;
	//		}
	//		TweenAlpha ta = go.GetComponent<TweenAlpha> ();
	//		if (ta == null) {
	//			return;
	//		}
	//		UnityEngine.MonoBehaviour.Destrotr (ta);
	//	}
	//


	//TODO GENERICS
	public static Queue<Texture> ToQueue (this List<Texture> l)
	{
		return new Queue<Texture> (l);
	}

	public static Queue<string> ToQueue (this string strs)
	{
		Queue<string> queue = new Queue<string> ();
		foreach (string str in strs.Split(',')) {
			queue.Enqueue (str);
		}
		return queue;
	}

	public static  string ConvertToString (this List<string> s)
	{
		return string.Join (",", s.ToArray ());
	}


	public static  List<string> ConvertToStringList (this string s)
	{
		return s.Split (',').ToList ();
	}

	public static  string ConvertToString (this List<int> s)
	{
		return string.Join (",", s.Select (i => i.ToString ()).ToArray ());
	}


	public static  List<int> ConvertToIntList (this string s)
	{
		return s.Split (',').Select (str => str.ToInt (0)).ToList ();
	}


	public static string ConvertToString (this Vector3 v)
	{
		return  v.x + "|" + v.y + "|" + v.z + "|";
	}

	public static Vector3 ConvertToVector3 (this string s)
	{
		string[] tmp = s.Split ("|" [0]);
		Vector3 myVector = new Vector3 (float.Parse (tmp [0]), float.Parse (tmp [1]), float.Parse (tmp [2]));
		return myVector;
	}



	public static bool IsPlaying (this Animator anim, string animName)
	{
		return anim.GetCurrentAnimatorStateInfo (0).IsName (animName);
	}

	public static string ToTimeLabel (this int seconds)
	{

		int sec = seconds % 60;
		int min = (seconds / 60) % 60;
		int hour = (seconds / 60 / 60) % 24;

		return string.Format ("{0:D2}:{1:D2}:{2:D2}", hour, min, sec);
	}

	public static string UppercaseFirst (this string s)
	{
		// Check for empty string.
		if (string.IsNullOrEmpty (s)) {
			return string.Empty;
		}
		// Return char and concat substring.
		string str = s.ToLower ();

		return char.ToUpper (str [0]) + str.Substring (1);
	}

	public static bool Play (this Animation animation, AnimationClip clip)
	{
		Animator a = animation.GetComponent<Animator> ();
		if (a != null) {
			a.enabled = false;
		}
#if BUILD_TYPE_DEBUG
		// Debug.Log("CALL Play:"+ clip.name);
#endif
		animation.enabled = true;
		animation.AddClip (clip, clip.name);
		return animation.Play (clip.name);
	}

	public static string FullPath (this GameObject obj)
	{
		string path = "/" + obj.name;
		while (obj.transform.parent != null) {
			obj = obj.transform.parent.gameObject;
			path = "/" + obj.name + path;
		}
		return path;
	}

	public static string UnescapeXML (this string s)
	{
		if (string.IsNullOrEmpty (s))
			return s;

		string returnString = s;
		returnString = returnString.Replace ("&apos;", "'");
		returnString = returnString.Replace ("&quot;", "\"");
		returnString = returnString.Replace ("&gt;", ">");
		returnString = returnString.Replace ("&lt;", "<");
		returnString = returnString.Replace ("&amp;", "&");

		return returnString;
	}


	public static string D2 (this int v)
	{
		return string.Format ("{0:D2}", v);
	}

	public static string D3 (this int v)
	{
		return string.Format ("{0:D3}", v);
	}

	public static string D4 (this int v)
	{
		return string.Format ("{0:D4}", v);
	}


	public static T GetAndCacheComponentInParent<T> (this GameObject behaviour, ref T t) where T : Component
	{
		if (t == null) {
			t = behaviour.GetComponentInParent<T> ();
		}
		return t;
	}


	public static List<T> GetAndCacheComponentsInParent<T> (this GameObject behaviour, ref List<T> list) where T : Component
	{
		if (list == null) {
			list = behaviour.GetComponentsInParent<T> ().ToList ();
		}
		return list;
	}


	public static T GetAndCacheComponentInChildren<T> (this GameObject behaviour, ref T t) where T : Component
	{
		if (t == null) {
			t = behaviour.GetComponentInChildren<T> ();
		}
		return t;
	}

	public static T GetAndCacheComponent<T> (this GameObject behaviour, ref T t) where T : Component
	{
		if (t == null) {
			t = behaviour.GetComponent<T> ();
		}
		return t;
	}


	public static List<T> GetAndCacheComponentsInChildren<T> (this GameObject behaviour, ref List<T> list) where T : Component
	{
		if (list == null) {
			list = behaviour.GetComponentsInChildren<T> ().ToList ();
		}
		return list;
	}

	public static List<T> GetAndCacheComponents<T> (this GameObject behaviour, ref List<T> list) where T : Component
	{
		if (list == null) {
			list = behaviour.GetComponents<T> ().ToList ();
		}
		return list;
	}



	public static T GetComponentInSelfAndChildren<T> (this GameObject behaviour) where T : Component
	{
		T result = behaviour.GetComponent<T> ();
		if (result != null) {
			return result;
		}
		return behaviour.GetComponentInChildren<T> ();
	}


	public static T Next<T> (this List<T> list, T t)
	{
		if (list.IsLast (t)) {
			return list.First ();
		}
		return list [list.IndexOf (t) + 1];
	}

	public  static T Prev<T> (this List<T> list, T t)
	{
		if (list.IsFirst (t)) {
			return list.Last ();
		}
		return list [list.IndexOf (t) - 1];
	}

	public static T First<T> (this List<T> list)
	{
		if (list.Count () == 0) {
			return default(T);
		}
		return list [0];
	}

	public static T Last<T> (this List<T> list)
	{
		if (list.Count () == 0) {
			return default(T);
		}
		return list [list.Count () - 1];
	}

	public static bool IsFirst<T> (this List<T> list, T t)
	{
		if (list.Count () == 0) {
			return false;
		}
		return list.First ().Equals (t);
	}


	public static bool IsLast<T> (this List<T> list, T t)
	{
		if (list.Count () == 0) {
			return false;
		}
		return list.Last ().Equals (t);
	}

	public static string JsonPrettyFormat(this string json)
	{

		LitJson.JsonWriter writer1 = new LitJson.JsonWriter();
		writer1.PrettyPrint = true;
		writer1.IndentValue = 4;
		LitJson.JsonMapper.ToJson(MiniJSON.Json.Deserialize(json), writer1);
		return writer1.ToString();
	}


	// public static bool IsNullOrEmpty<T>(this List<T> list)
	// {
	//     return list == null || list.Count == 0;
	// }

	// public static bool IsNotNullOrEmpty<T>(this List<T> list)
	// {
	//     return !list.IsNullOrEmpty<T>();
	// }

	//    public static IEnumerable<T> RemoveDuplicates<T>(this ICollection<T> list, Func<T, int> Predicate)
	//    {
	//        var dict = new Dictionary<int, T>();
	//
	//        foreach (var item in list)
	//        {
	//            if (!dict.ContainsKey(Predicate(item)))
	//            {
	//                dict.Add(Predicate(item), item);
	//            }
	//        }
	//
	//        return dict.Values.AsEnumerable();
	//    }


	public static List<T> FindChildren<T> (this GameObject gameObject) where T : Component
	{
		List<T> result = new List<T> ();
		foreach (Transform transform in gameObject.transform) {
			T component = transform.GetComponent<T> ();
			if (component != null) {
				result.Add (component);
			}
		}
		return result;
	}


	public static List<Transform> FindChildrenTransform<T> (this GameObject gameObject) where T : Component
	{
		List<Transform> result = new List<Transform> ();
		foreach (Transform transform in gameObject.transform) {
			if (transform.GetComponent<T> () != null) {
				result.Add (transform);
			}
		}
		return result;
	}


	//	public static void SetButtonAction (this GameObject g, Action action)
	//	{
	//		UIButton button = g.GetComponent<UIButton> ();
	//		if (button == null) {
	//			button = g.AddComponent<UIButton> ();
	//		}
	//		button.onClick.Add (new EventDelegate (delegate() {
	//			action ();
	//		}));
	//	}

	public static int Number (this GameObject g)
	{
		return int.Parse (g.name.Substring (g.name.Length - 2, 2));
	}


	public static void SetActivateRecursive (this GameObject g, bool a)
	{

		g.SetActive (a);

		foreach (Transform child in g.transform) {
			child.gameObject.SetActivateRecursive (a);
		}
	}


	public static void SetActiveChildren (this GameObject g, bool active)
	{
		foreach (Transform t in g.transform) {
			t.gameObject.SetActivateRecursive (active);
		}
	}


	public static T TakeRandom<T> (this List<T> list)
	{
		if (list.Count == 0) {
			return default(T);
		}
		return list [UnityEngine.Random.Range (0, list.Count)];
	}


	public static List<int> ToIntList (this string str)
	{
		return str.Split (',').Select (s => s.ToInt (0)).ToList ();
	}

	public static string TrimEndNL (this string text)
	{
		return text.TrimEnd ('\r', '\n');
	}

	public static int ExtractInt (this string text)
	{
		return int.Parse (ExtractIntStr (text));
	}

	public static float ExtractFloat (this string text)
	{
		return float.Parse (ExtractFloatStr (text));
	}

	public static string ExtractFloatStr (this string text)
	{
		return Regex.Replace (text, @"[^\d.]", string.Empty);
	}

	public static string ExtractIntStr (this string text)
	{
		if (text.IsNullOrEmpty ()) {
			return "0";
		}
		return Regex.Replace (text.Replace (",", ""), @"[^\d]", string.Empty);
	}


	public static string Path_Platform (this string str, string platform)
	{
		return str.Path_Platform (platform, null);
	}

	public static string Path_Platform (this string str, string platform, string extension)
	{
		if (extension == null) {
			extension = "unity3d";
		}

		Debug.LogError ("SS:" + str);
		string[] strs = str.Split ('/');


		return string.Format ("AssetBundle/{0}/{1}", platform, strs [strs.Length - 1]).ConvertExtensionToAssetBundle (extension);
	}

	public static string AbsPath_Platform (this string str, string platform)
	{
		return AbsPath_Platform (str, platform, null);
	}

	public static string AbsPath_Platform (this string str, string platform, string extension)
	{
		return Application.persistentDataPath + "/" + str.Path_Platform (platform, extension);
	}

	public static string CutOffAB (this string str)
	{
		return str.Substring ("Assets/101_AssetBundleMaker/AssetBundle/".Length);
	}

	public static string CategoryStr (this string str)
	{
		return str.Split ('/') [3];
	}

	public static string SplitLast (this string str)
	{
		string[] strs = str.Split ('/');
		return strs [strs.Length - 1];
	}

	public static string ConvertExtensionToAssetBundle (this string str, string extension)
	{
		return str.Split ('.') [0] + "." + extension;
	}


	private static string DATE_FORMAT = "yyyy/MM/dd HH:mm:ss";

	public static DateTime ConvertToDateTime (this string dtStr)
	{
//		return DateTime.ParseExact(dtStr,DATE_FORMAT,null);
//		Debug.LogError ("CONVERT_TO_DATETIME:" + dtStr);
		return DateTime.Parse (dtStr);
	}

	public static string Format (this DateTime dt)
	{
		return dt.ToString (DATE_FORMAT);
	}

	//	public static string FormatJA (this DateTime dt)
	//	{
	//		return dt.ToString ("yyyy年MM月dd日");
	//	}
	//
	public static Vector3 Deserialize (this Vector3 vector3, string data)
	{
		data = data.Remove (0, 1);
		data = data.Remove (data.Length - 1, 1);
		float[] v = data.Split (',').Select (d => float.Parse (d)).ToArray ();
		vector3.x = v [0];
		vector3.y = v [1];
		vector3.z = v [2];
		return vector3;
	}



	public static Vector3 Round (this Vector3 vector3, int digits)
	{
		float x = (float)Math.Round ((double)vector3.x, digits);
		float y = (float)Math.Round ((double)vector3.y, digits);
		float z = (float)Math.Round ((double)vector3.z, digits);

		return new Vector3 (x, y, z);

	}

	public static Vector3 SetX (this Vector3 vector3, float x)
	{
		return new Vector3 (x, vector3.y, vector3.z);
	}

	public static Vector3 SetY (this Vector3 vector3, float y)
	{
		return new Vector3 (vector3.x, y, vector3.z);
	}

	public static Vector3 SetZ (this Vector3 vector3, float z)
	{
		return new Vector3 (vector3.x, vector3.y, z);
	}

	public static void BiyonXY (this Transform self, float rate = 0.5f)
	{
		DOTween.Sequence ().
            Append (self.DOScale (1f + rate, 0.1f)).
            Append (self.DOScale (1f, 0.2f).SetEase (Ease.OutBack));
	}

	public static void Biyon (this Transform self, float rate = 0.5f)
	{
		DOTween.Sequence ().
            Append (self.DOScaleY (1f + rate, 0.1f)).
            Append (self.DOScaleY (1f, 0.2f).SetEase (Ease.OutBack));
	}


	public static Vector2 Position2D (this Transform self)
	{
		return new Vector2 (self.position.x, self.position.y);
	}

	/// <summary>
	/// 指定のオブジェクトの方向に回転する
	/// </summary>
	/// <param name="self">Self.</param>
	/// <param name="target">Target.</param>
	/// <param name="forward">正面方向</param>
	public static void LookAt2D (this Transform self, Transform target, Vector2 forward)
	{
//      Debug.LogError("T:"+target);
		LookAt2D (self, target.position, forward);
	}


	public static void LookAt2D (this Transform self, Vector3 target, Vector2 forward)
	{
		var forwardDiff = GetForwardDiffPoint (forward);
		Vector3 direction = target - self.position;
		float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
		self.rotation = Quaternion.AngleAxis (angle - forwardDiff, Vector3.forward);
	}

	public static Vector3 LookAt2DEulerAngles (this Transform self, Transform target, Vector2 forward)
	{
		var forwardDiff = GetForwardDiffPoint (forward);
		Vector3 direction = target.position - self.position;
		float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
		return Quaternion.AngleAxis (angle - forwardDiff, Vector3.forward).eulerAngles;
	}

	public static Dictionary<string,int> Convert (this Dictionary<string, object> dict)
	{
		Dictionary<string,int> result = new Dictionary<string,int> ();
		foreach (string key in dict.Keys) {
			result.Add (key, int.Parse (dict [key].ToString ()));
		}
		return result;
	}

	public static bool ContainsKeyAndData<K, V> (this Dictionary<K, V> dict, K key)
	{
		if (dict.ContainsKey (key) && (dict [key] != null)) {
			if (dict [key] is ICollection) {
				return ((ICollection)dict [key]).Count > 0;
			} else {
				return true;
			}
		} else {
			return false;
		}
	}

	public static V GetValueOrDefault<K, V> (this Dictionary<K, V> dic, K key, V defaultVal)
	{
		V ret;
		bool found = dic.TryGetValue (key, out ret);
		if (found) {
			return ret;
		}
		return defaultVal;
	}

	public static int GetValueOrDefaultToInt<K, V> (this Dictionary<K, V> dic, K key, V defaultVal)
	{
		V ret = dic.GetValueOrDefault<K,V> (key, defaultVal);
		return System.Convert.ToInt32 (ret);
	}

	public static string GetValueOrDefaultToString<K, V> (this Dictionary<K, V> dic, K key, V defaultVal)
	{
		V ret = dic.GetValueOrDefault<K,V> (key, defaultVal);
		return System.Convert.ToString (ret);
	}

	public static void Dump<K, V> (this Dictionary<K, V> dic)
	{
		foreach (K k in dic.Keys) {
			Debug.Log ("KEY:" + k.ToString () + " VALUE:" + dic [k].ToString ());
		}
//      V ret = dic.GetValueOrDefault<K,V>(key,defaultVal);
//      return System.Convert.ToString(ret);
	}



	/// <summary>
	/// 正面の方向の差分を算出する
	/// </summary>
	/// <returns>The forward diff point.</returns>
	/// <param name="forward">Forward.</param>
	static private float GetForwardDiffPoint (Vector2 forward)
	{
		if (Equals (forward, Vector2.up))
			return 90;
		if (Equals (forward, Vector2.right))
			return 0;
		return 0;
	}

	//	public static void DestroyChildren (this Transform transform)
	//	{
	//		foreach (Transform childT in transform.Children().ToList()) {
	//			UnityEngine.Object.Destroy (childT.gameObject);
	//		}
	//	}

	public static void DestroyChildrenImmediate (this Transform transform)
	{
		foreach (Transform childT in transform.Children().ToList()) {
			UnityEngine.Object.DestroyImmediate (childT.gameObject);
		}
	}



	public static Transform[] Children (this Transform tx)
	{
		var list = new List<Transform> ();
		foreach (Transform t in tx) {
			list.Add (t);
		}
		return list.ToArray ();
	}

	public static void AddEulerAnglesY (this Transform transform, float y)
	{
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y + y, transform.eulerAngles.z);
	}

	public static void AddEulerAnglesZ (this Transform transform, float z)
	{
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + z);
	}

	public static void AddLocalEulerAnglesY (this Transform transform, float y)
	{
		transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y + y, transform.localEulerAngles.z);
	}

	public static void AddLocalEulerAnglesZ (this Transform transform, float z)
	{
		transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + z);
	}


	public static void AddPositionXandY (this Transform transform, float x, float y)
	{
		transform.position = new Vector3 (transform.position.x + x, transform.position.y + y, transform.localPosition.z);
	}

	public static void AddLocalPositionXandY (this Transform transform, float x, float y)
	{
		transform.localPosition = new Vector3 (transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
	}

	public static void AddLocalPositionX (this Transform transform, float x)
	{
		transform.localPosition = new Vector3 (transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z);
	}

	public static void AddLocalPositionY (this Transform transform, float y)
	{
		transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y + y, transform.localPosition.z);
	}

	public static void AddLocalPositionZ (this Transform transform, float z)
	{
		transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + z);
	}


	public static void AddPositionX (this Transform transform, float x)
	{
		transform.position = new Vector3 (transform.position.x + x, transform.position.y, transform.position.z);
	}

	public static void AddPositionY (this Transform transform, float y)
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y + y, transform.position.z);
	}

	public static void AddPositionZ (this Transform transform, float z)
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + z);
	}


	public static Vector3 AddAndGetPositionX (this Transform transform, float x)
	{
		return new Vector3 (transform.position.x + x, transform.position.y, transform.position.z);
	}

	public static Vector3 AddAndGetPositionY (this Transform transform, float y)
	{
		return new Vector3 (transform.position.x, transform.position.y + y, transform.position.z);
	}

	public static Vector3 AddAndGetPositionZ (this Transform transform, float z)
	{
		return new Vector3 (transform.position.x, transform.position.y, transform.position.z + z);
	}

	public static Vector3 SetAndGetPositionX (this Transform transform, float x)
	{
		return new Vector3 (x, transform.position.y, transform.position.z);
	}

	public static Vector3 SetAndGetPositionY (this Transform transform, float y)
	{
		return new Vector3 (transform.position.x, y, transform.position.z);
	}

	public static Vector3 SetAndGetPositionZ (this Transform transform, float z)
	{
		return new Vector3 (transform.position.x, transform.position.y, z);
	}

	public static void SetEulerAnglesZ (this Transform transform, float z)
	{
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, z);
	}

	public static void SetLocalEulerAnglesX (this Transform transform, float x)
	{
		transform.localEulerAngles = new Vector3 (x, transform.localEulerAngles.y, transform.localEulerAngles.x);
	}

	public static void SetLocalEulerAnglesY (this Transform transform, float y)
	{
		transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, y, transform.localEulerAngles.x);
	}

	public static void SetLocalEulerAnglesZ (this Transform transform, float z)
	{
		transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y, z);
	}

	public static void SetPositionXandY (this Transform transform, float x, float y)
	{
		transform.position = new Vector3 (x, y, transform.position.z);
	}

	public static void SetPositionZandY (this Transform transform, float z, float y)
	{
		transform.position = new Vector3 (transform.position.x, y, z);
	}

	public static void SetPositionXandZ (this Transform transform, float x, float z)
	{
		transform.position = new Vector3 (x, transform.position.y, z);
	}

	public static void SetPositionYandZ (this Transform transform, float y, float z)
	{
		transform.position = new Vector3 (transform.position.x, y, z);
	}

	public static void SetLocalPositionXandY (this Transform transform, float x, float y)
	{
		transform.localPosition = new Vector3 (x, y, transform.localPosition.z);
	}

	public static void SetLocalPositionYandZ (this Transform transform, float y, float z)
	{
		transform.localPosition = new Vector3 (transform.localPosition.x, y, z);
	}


	public static void SetLocalPosition (this Transform transform, float x, float y, float z)
	{
		transform.localPosition = new Vector3 (x, y, z);
	}

	/// <summary>
	/// ローカルポジションXを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalPositionX (this Transform t, float value)
	{
		t.localPosition = new Vector3 (value, t.localPosition.y, t.localPosition.z);
		return t;
	}

	/// <summary>
	/// ローカルポジションYを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalPositionY (this Transform t, float value)
	{
		t.localPosition = new Vector3 (t.localPosition.x, value, t.localPosition.z);
		return t;
	}

	/// <summary>
	/// ローカルポジションZを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalPositionZ (this Transform t, float value)
	{
		t.localPosition = new Vector3 (t.localPosition.x, t.localPosition.y, value);
		return t;
	}

	/// <summary>
	/// ローカルスケールXを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalScaleX (this Transform t, float value)
	{
		t.localScale = new Vector3 (value, t.localScale.y, t.localScale.z);
		return t;
	}

	/// <summary>
	/// ローカルスケールYを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalScaleY (this Transform t, float value)
	{
		t.localScale = new Vector3 (t.localScale.x, value, t.localScale.z);
		return t;
	}

	/// <summary>
	/// ローカルスケールZを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalScaleZ (this Transform t, float value)
	{
		t.localScale = new Vector3 (t.localScale.x, t.localScale.y, value);
		return t;
	}

	/// <summary>
	/// ローカルローテーションXを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalRotationX (this Transform t, float value)
	{
		t.localRotation = Quaternion.Euler (value, t.localRotation.eulerAngles.y, t.localRotation.eulerAngles.z);
		return t;
	}

	/// <summary>
	/// ローカルローテーションYを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalRotationY (this Transform t, float value)
	{
		t.localRotation = Quaternion.Euler (t.localRotation.eulerAngles.x, value, t.localRotation.eulerAngles.z);
		return t;
	}

	/// <summary>
	/// ローカルローテーションZを設定
	/// </summary>
	/// <param name='t'>トランスフォーム</param>
	/// <param name='value'>値</param>
	public static Transform SetLocalRotationZ (this Transform t, float value)
	{
		t.localRotation = Quaternion.Euler (t.localRotation.eulerAngles.x, t.localRotation.eulerAngles.y, value);
		return t;
	}

	public static Vector3 AddX (this Vector3 v, float x)
	{
		return new Vector3 (v.x + x, v.y, v.z);
	}

	public static Vector3 AddY (this Vector3 v, float y)
	{
		return new Vector3 (v.x, v.y + y, v.z);
	}


	public static Vector3 AddZ (this Vector3 v, float z)
	{
		return new Vector3 (v.x, v.y, v.z + z);
	}


	public static Vector3 AddXandZ (this Vector3 v, float x, float z)
	{
		return new Vector3 (v.x + x, v.y, v.z + z);
	}

	//    public static Vector3 SetX(this Vector3 vector3, float x){
	//        return new Vector3(x,vector3.y,vector3.z);
	//    }
	//
	//    public static Vector3 SetY(this Vector3 vector3, float y){
	//        return new Vector3(vector3.x,y,vector3.z);
	//    }
	//
	//    public static Vector3 SetZ(this Vector3 vector3, float z){
	//        return new Vector3(vector3.x,vector3.y,z);
	//    }
	public static string Last (this string str)
	{
		return str.Substring (str.Length - 1, 1);
	}

}
