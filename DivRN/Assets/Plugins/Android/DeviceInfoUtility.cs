#if UNITY_ANDROID && !UNITY_EDITOR

/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	DeviceInfoUtility.cs
	@brief	Android用アプリ外領域へのデータ保存
	@author Developer
	@date 	2015/06/29
*/
/*==========================================================================*/
/*==========================================================================*/

using UnityEngine;
using System.Collections;

public class DeviceInfoUtility
{
	public static AndroidJavaClass UnityPlayerObject;
	public static AndroidJavaObject UnityActivity;
	
	public static AndroidJavaClass 	ms_AndroidPlugin_2_Class = null;
	public static AndroidJavaObject ms_AndroidPlugin_2_Obj = null;
	//----------------------------------------------------------------------------
	/*!
		@brief	初期化
	*/
	//----------------------------------------------------------------------------
	public static void Init()
	{
		UnityPlayerObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		UnityActivity = UnityPlayerObject.GetStatic<AndroidJavaObject>("currentActivity");

		// DeviceInfoUtility_2 のインスタンスの取得(シングルトンクラスなのでインスタンス化はできない)
		ms_AndroidPlugin_2_Class = new AndroidJavaClass("jp.example.DeviceInfoUtil.DeviceInfoUtility_2");
		ms_AndroidPlugin_2_Obj = ms_AndroidPlugin_2_Class.CallStatic<AndroidJavaObject>("GetInstance");
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	外部ストレージのマウント状態取得
	*/
	//----------------------------------------------------------------------------
	public static string GetExStorageState()
	{
		string retVal = "This function is AndroidPlatform only.";
		if( ms_AndroidPlugin_2_Obj != null)
		{
			retVal = ms_AndroidPlugin_2_Obj.Call<string>("GetExStorageState");
		}
		return retVal;
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	外部ストレージパス取得
	*/
	//----------------------------------------------------------------------------
	public static string GetExDirPath()
	{
		string retVal = "This function is AndroidPlatform only.";
		if( ms_AndroidPlugin_2_Obj != null)
		{
			retVal = ms_AndroidPlugin_2_Obj.Call<string>("GetExDirPath");
		}
		return retVal;
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	システムの設定ファイルから外部ストレージのパスリストを取得
	*/
	//----------------------------------------------------------------------------
	public static string[] GetExStoragePaths()
	{
		string[] retVal = null;
		if( ms_AndroidPlugin_2_Obj != null)
		{
			retVal = ms_AndroidPlugin_2_Obj.Call<string[]>("GetExStoragePaths");
		}
		return retVal;
	}
	public static string[] GetExStoragePaths_Active()
	{
		string[] retVal = null;
		if( ms_AndroidPlugin_2_Obj != null)
		{
			retVal = ms_AndroidPlugin_2_Obj.Call<string[]>("GetExStoragePaths");
			if( retVal != null)
			{
				for(int i = 0; i < retVal.Length; i++)
				{
					retVal[i] = string.Empty;
				}
			}
		}
		return retVal;
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	指定されたパスがマウントされているかチェックする
	 	@note	USB接続で外部ストレージモードにしている場合などにはマウント扱いされるずに false となる
		@param	path : チェックするパス
		@return	true : マウントされている
				false: マウントされていない
	*/
	//----------------------------------------------------------------------------
	public static bool IsMounted( string path )
	{
		bool retVal = false;
		if( ms_AndroidPlugin_2_Obj != null)
		{
			retVal = ms_AndroidPlugin_2_Obj.Call<bool>("isMounted", path);
		}
		return retVal;
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	指定したパスにディレクトリを作成
		@param	path : 作成するパス
	*/
	//----------------------------------------------------------------------------
	public static bool CreateDir( string path )
	{
		bool retVal = false;
		if( ms_AndroidPlugin_2_Obj != null)
		{
			retVal = ms_AndroidPlugin_2_Obj.Call<bool>("CreateDir", path);
		}
		return retVal;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	指定したディレクトリを削除
		@param	path : 削除するパス
	*/
	//----------------------------------------------------------------------------
	public static bool DeleteDir( string path )
	{
		bool retVal = false;
		if( ms_AndroidPlugin_2_Obj != null )
		{
			ms_AndroidPlugin_2_Obj.Call<bool>("DeleteDir", path);
		}
		return retVal;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	SetFileNameとSetFilePathを纏めて行う関数
		@param	fileName : ファイル名
		@param	path	 : パス
	*/
	//----------------------------------------------------------------------------
	public static void FileInit( string fileName, string path )
	{
		if( ms_AndroidPlugin_2_Obj != null )
		{
			ms_AndroidPlugin_2_Obj.Call("FileInit", fileName, path);
		}
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	保存するxmlファイル名を設定する
		@param	name	: ファイル名
	*/
	//----------------------------------------------------------------------------
	public static void SetFileName( string name )
	{
		if( ms_AndroidPlugin_2_Obj != null )
		{
			ms_AndroidPlugin_2_Obj.Call("SetFileName", name);
		}
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	xmlファイルの保存位置のパスを設定する
		@param	path	: ファイル名
	*/
	//----------------------------------------------------------------------------
	public static void SetFilePath( string path )
	{
		if( ms_AndroidPlugin_2_Obj != null )
		{
			ms_AndroidPlugin_2_Obj.Call("SetFilePath", path);
		}
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	keyと要素を対にして保存
		@param	key	: キー
		@param	item: 保存データ
	*/
	//----------------------------------------------------------------------------
	public static void SetKeyChain(string key, string item)
	{
		if( ms_AndroidPlugin_2_Obj != null )
		{
			ms_AndroidPlugin_2_Obj.Call("SetKeyChainItem", key,item);
		}
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	指定keyの要素を取得
		@param	key	: キー
	*/
	//----------------------------------------------------------------------------
	public static string GetKeyChain(string key)
	{
		string retVal = "This function is AndroidPlatform only.";
		if( ms_AndroidPlugin_2_Obj != null )
		{
			retVal = ms_AndroidPlugin_2_Obj.Call<string>("GetKeyChainItem", key);
		}
		return retVal;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	指定keyの要素を削除
		@param	key	: キー
	*/
	//----------------------------------------------------------------------------
	public static void DeleteKeyChain(string key)
	{
		if( ms_AndroidPlugin_2_Obj != null )
		{
			ms_AndroidPlugin_2_Obj.Call("DeleteKeyChainItem", key);
		}
	}
}
#endif
