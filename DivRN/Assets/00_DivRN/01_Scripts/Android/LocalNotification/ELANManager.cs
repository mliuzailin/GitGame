using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ELANManager
{
#if UNITY_ANDROID && !UNITY_EDITOR
	private static AndroidJavaObject playerActivityContext = null;
	private static List<int> currentNotificationIDs = new List<int>();
#endif

    public static int SendNotification(string title, string message, string packageName, string activityName, long delay)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
Debug.LogError( "SendNotification ---" );
		//if(currentNotificationIDs.Count == 0) currentNotificationIDs.AddRange(GetIntArray());

		int result_nID = -1;

		CreatePlayerActivityContext();

		AndroidJNI.AttachCurrentThread();
		AndroidJNI.PushLocalFrame(0);
		try {
			// Set notification within java plugin
			using ( AndroidJavaClass pluginClass = new AndroidJavaClass("com.CFM.ELAN.ELANManager") ) {
				int nID = (int)(Time.time*1000) + (int)Random.Range (0,int.MaxValue/2);
				if (pluginClass != null) {
					pluginClass.CallStatic("FireNotification", playerActivityContext, title, message, packageName, activityName, delay, nID);
					if(delay > 0) {
						currentNotificationIDs.Add (nID);
						SetIntArray(currentNotificationIDs.ToArray ());
					}
				}
				result_nID = nID;
			}
		}
		catch (System.Exception ex) {
#if BUILD_TYPE_DEBUG
			Debug.Log( ex.Message );
#endif
		}
		finally {
			AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
		}

		return result_nID;
#else
        return -1;
#endif
    }

    public static void ScheduleRepeatingNotification(string title, string message, string packageName, string activityName, long delay, long rep)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		CreatePlayerActivityContext();
		
		AndroidJNI.AttachCurrentThread();
		AndroidJNI.PushLocalFrame(0);
		try {
			// Set notification within java plugin
			using ( AndroidJavaClass pluginClass = new AndroidJavaClass("com.CFM.ELAN.ELANManager") ) {
				if (pluginClass != null) {
					pluginClass.CallStatic( "ScheduleRepeatingNotification", playerActivityContext, title, message, packageName, activityName, delay, rep );
				}
			}
		}
		catch (System.Exception ex) {
#if BUILD_TYPE_DEBUG
			Debug.Log( ex.Message );
#endif
		}
		finally {
			AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
		}
#endif
    }

    private static void CreatePlayerActivityContext()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

		AndroidJNI.AttachCurrentThread();
		AndroidJNI.PushLocalFrame(0);
		try {
			if ( playerActivityContext == null ) {
				// using( AndroidJavaClass actClass = new AndroidJavaClass("com.CFM.ELAN.PQDMCurrentActivity") ) {
				// 	playerActivityContext = actClass.CallStatic<AndroidJavaObject>( "getCurrentActivity" );
				// 	if ( playerActivityContext == null ) {
				// 		Debug.LogError( "null ---------------------" );
				// 	}
				// }
				using( AndroidJavaClass actClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) ) {
					playerActivityContext = actClass.GetStatic<AndroidJavaObject>( "currentActivity" );
				}
			}
		}
		catch (System.Exception ex)
		{
#if BUILD_TYPE_DEBUG
			Debug.Log( ex.Message );
#endif
		}
		finally
		{
			AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
		}
#endif
    }

    public static void CancelAllNotifications()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		//Cancel Notification
		currentNotificationIDs.Clear();
		currentNotificationIDs.AddRange(GetIntArray());
		if(currentNotificationIDs.Count > 0) {
			List<int> copy = new List<int>();
			copy.AddRange (currentNotificationIDs);
			foreach(int nID in copy) {
				CancelLocalNotification(nID);
			}
			SetIntArray(currentNotificationIDs.ToArray());
		}
		
		//Cancel RepeatingNotification
		CancelRepeatingNotification();
#endif
    }

    private static void CancelRepeatingNotification()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		CreatePlayerActivityContext();
		
		AndroidJNI.AttachCurrentThread();
		AndroidJNI.PushLocalFrame(0);
		try {
			// Set notification within java plugin
			using ( AndroidJavaClass pluginClass = new AndroidJavaClass("com.CFM.ELAN.ELANManager") ) {
				if (pluginClass != null) {
					pluginClass.CallStatic("CancelRepeatingNotification", playerActivityContext);
				}
			}
		}
		catch (System.Exception ex) {
#if BUILD_TYPE_DEBUG
			Debug.Log( ex.Message );
#endif
		}
		finally {
			AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
		}
#endif
    }

    private static void CancelLocalNotification(int nID)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		CreatePlayerActivityContext();

		AndroidJNI.AttachCurrentThread();
		AndroidJNI.PushLocalFrame(0);
		try {
			// Set notification within java plugin
			using ( AndroidJavaClass pluginClass = new AndroidJavaClass("com.CFM.ELAN.ELANManager") ) {
				if (pluginClass != null) {
					pluginClass.CallStatic("CancelNotification", playerActivityContext, nID);
					currentNotificationIDs.Remove (nID);
				}
			}
		}
		catch (System.Exception ex) {
#if BUILD_TYPE_DEBUG
			Debug.Log( ex.Message );
#endif
		}
		finally {
			AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
		}
#endif
    }

    private static string PREF_KEY = "ELANPlayerPrefsKey";
    public static void SetIntArray(int[] intArray)
    {
        if (intArray.Length == 0)
        {
            PlayerPrefs.DeleteKey(PREF_KEY);
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < intArray.Length - 1; i++)
        {
            sb.Append(intArray[i]).Append("|");
        }
        sb.Append(intArray[intArray.Length - 1]);

        try
        {
            PlayerPrefs.SetString(PREF_KEY, sb.ToString());
        }
        catch (System.Exception err)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log(err.ToString());
#endif
        }
    }

    public static int[] GetIntArray()
    {
        if (PlayerPrefs.HasKey(PREF_KEY))
        {
            string[] stringArray = PlayerPrefs.GetString(PREF_KEY).Split("|"[0]);
            Debug.Log(PlayerPrefs.GetString(PREF_KEY));
            int[] intArray = new int[stringArray.Length];
            Debug.Log(intArray.Length);
            for (int i = 0; i < stringArray.Length; i++)
            {
                intArray[i] = int.Parse(stringArray[i]);
            }
            return intArray;
        }
        return new int[0];
    }
}
