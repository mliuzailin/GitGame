
#if UNITY_EDITOR
#define SAVE_TYPE_UNITY
#elif UNITY_STANDALONE_WIN
#define SAVE_TYPE_WINDOWS
#elif UNITY_IPHONE
#define SAVE_TYPE_IOS
#elif UNITY_ANDROID
#define SAVE_TYPE_ANDROID
#else
#define SAVE_TYPE_WINDOWS
#endif


/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	LocalSaveUtilToInstallFolder.cs
	@brief	端末セーブのラップクラス：アプリインストールフォルダへのファイル入出力
	@author Developer
	@date 	2014/08/13
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ServerDataDefine;
using System.Text;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	端末セーブのラップクラス：アプリインストールフォルダへのファイル入出力
*/
//----------------------------------------------------------------------------
static public class LocalSaveUtilToInstallFolder
{
    private static string SUFFIX_BYTES = ".bytes";
    private static string SUFFIX_TEXT = ".txt";

#if SAVE_TYPE_ANDROID
	public static AndroidJavaClass		m_AndroidClassUnityPlayer;
	public static AndroidJavaObject		m_AndroidObjectActivity;
#elif SAVE_TYPE_IOS
	[DllImport("__Internal")] private static extern int iOS_StorageAvailableMb();
#endif

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Java関連クラスの取得初期化
	*/
    //----------------------------------------------------------------------------
    static public void InitializeJava()
    {
#if SAVE_TYPE_ANDROID
		//--------------------------------
		// Androidの場合、パスを求めるためにActivityが必要になる。
		// 毎回取るのも負荷になるので、最初に一度だけ取得を行うようにする
		//--------------------------------
		if( m_AndroidClassUnityPlayer == null )
		{
			m_AndroidClassUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		}
		if( m_AndroidClassUnityPlayer != null 
		&&	m_AndroidObjectActivity == null
		)
		{
			m_AndroidObjectActivity = m_AndroidClassUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		}
#endif
    }

    public static string GetSavePathAssetBundle()
    {
        return GetSavePathWithCreateFolderIfNotExists("Assetbundle");
    }

    public static string GetSavePathSqlite()
    {
        return GetSavePathWithCreateFolderIfNotExists("db");
    }

    public static string GetSavePathBattle()
    {
#if SAVE_TYPE_IOS
        return GetSavePathWithCreateFolderIfNotExists("../battle");
#else
        return GetSavePathWithCreateFolderIfNotExists("battle");
#endif
    }

    public static string GetSavePathPreference()
    {
        return GetSavePathWithCreateFolderIfNotExists("preference");
    }

    public static string GetSavePathWebResource()
    {
        return GetSavePathWithCreateFolderIfNotExists("webresource");
    }

    private static string GetSavePathWithCreateFolderIfNotExists(string addPath = null)
    {
        string strSavePath = GetSavePath();
        if (strSavePath == null
        || strSavePath.Length <= 0
        )
        {
            Debug.LogError("InstallFolder Save Error! Path None ... ");
            return null;
        }

        if (addPath != null)
        {
            strSavePath = strSavePath + "/" + addPath;
        }

        //--------------------------------
        // フォルダが無いならフォルダを作る
        //--------------------------------
        if (System.IO.Directory.Exists(strSavePath) == false)
        {
            System.IO.Directory.CreateDirectory(strSavePath);
        }

        return strSavePath;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ対象となる絶対パスの取得
	*/
    //----------------------------------------------------------------------------

#if SAVE_TYPE_ANDROID
	static string Android_Save_PATH = null;
#endif

    static private string GetSavePath()
    {
#if SAVE_TYPE_UNITY
        //--------------------------------
        // Unityのフォルダは実行ファイル置き場所から取得
        //--------------------------------
        return Application.dataPath + "/../LocalDownload";
#endif
#if SAVE_TYPE_WINDOWS
		//--------------------------------
		// Windows版のアプリインストールフォルダは実行ファイル置き場所から取得
		//--------------------------------
		return Application.dataPath + "/Local";
#endif
#if SAVE_TYPE_IOS
		//--------------------------------
		// iOSのアプリインストールフォルダはApplicationから直接取得
		//--------------------------------
		return Application.temporaryCachePath;
#endif
#if SAVE_TYPE_ANDROID

		if(Android_Save_PATH != null){
			return Android_Save_PATH;
		}

		//--------------------------------
		// Androidのアプリインストールフォルダはプラグイン経由で取得
		//
		// ※プラグインで以下の処理を行うのと同等の処理を実行している
		// File Activity.getFilesDir()
		// string File.getCanonicalPath()
		//--------------------------------
		if( m_AndroidObjectActivity == null )
		{
			InitializeJava();
		}
		if( m_AndroidObjectActivity == null )
		{
			Debug.LogError( "Android Activity None..." );
			return "";
		}

		Android_Save_PATH = m_AndroidObjectActivity.Call<AndroidJavaObject>("getFilesDir").Call<string>("getCanonicalPath");

		return Android_Save_PATH;
#endif

#if !SAVE_TYPE_UNITY && !SAVE_TYPE_WINDOWS && !SAVE_TYPE_IOS && !SAVE_TYPE_ANDROID
		Debug.LogError( "LocalSave Platform Error!" );
		return "";
#endif // !SAVE_TYPE_WINDOWS && !SAVE_TYPE_IOS && !SAVE_TYPE_ANDROID
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	対象パスへのセーブが可能かチェック
	*/
    //----------------------------------------------------------------------------
    static public bool ChkSavePossible()
    {
        long nDataSize = GetRequestFreeSpace();
        long nBufferSize = GetSaveDiskFreeSpace();

        if (nBufferSize <= nDataSize)
        {
            return false;
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	空き容量エラー要求容量
	*/
    //----------------------------------------------------------------------------
    static public long GetRequestFreeSpace()
    {
        long nDataSize = 0;

        TutorialPart part = TutorialManager.GetNextTutorialPart();
        if (part == TutorialPart.LAST ||
            part == TutorialPart.NONE)
        {
            nDataSize = Patcher.Instance.GetDiskSizeLimitExistingUser();
        }
        else
        {
            nDataSize = Patcher.Instance.GetDiskSizeLimitNewUser();
        }

        return nDataSize;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ローカルセーブ保存領域の空き容量
	*/
    //----------------------------------------------------------------------------
    static public long GetSaveDiskFreeSpace()
    {
        const int TEMP_DISK_SIZE = 1000000;
        long megaByte = 0;
        int api_level = 0;

        // ストレージの空き容量を確認する - Qiita
        // https://qiita.com/snaka/items/35a6389d35447c2db272
#if SAVE_TYPE_ANDROID
        var cls = new AndroidJavaClass("android.os.Build$VERSION");
        api_level = cls.GetStatic<int>("SDK_INT");

        var statFs = new AndroidJavaObject("android.os.StatFs", GetSavePath());
        long availableBlocks = 0;
        long blockSize = 0;

        if(18 <= api_level)
        {
            availableBlocks = statFs.Call<long>("getAvailableBlocksLong");
            blockSize = statFs.Call<long>("getBlockSizeLong");
        }
        else
        {
            availableBlocks = statFs.Call<int>("getAvailableBlocks");
            blockSize = statFs.Call<int>("getBlockSize");
        }

        var freeBytes = availableBlocks * blockSize;
        if(freeBytes > 0)
        {
            megaByte = freeBytes / 1024 / 1024;
        }
        else
        {
            megaByte = TEMP_DISK_SIZE;
        }

#elif SAVE_TYPE_IOS
        megaByte = iOS_StorageAvailableMb();
        if(megaByte == -1)
        {
            megaByte = TEMP_DISK_SIZE;
        }
#else
        megaByte = TEMP_DISK_SIZE;
#endif

#if BUILD_TYPE_DEBUG
        string text = String.Format("API level {0} / GetSaveDiskFreeSpace: {1}MB", api_level, megaByte);
        UnityEngine.Debug.Log(text);
#endif

        return megaByte;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	セーブ実行：
	*/
    //----------------------------------------------------------------------------
    static private bool Save(string strSavePath, string strSaveKey, string strSaveData)
    {
        if (strSavePath == null
        || strSavePath.Length <= 0
        )
        {
            Debug.LogError("InstallFolder Save Error! Path None ... ");
            return false;
        }

        //--------------------------------
        // ファイル書き込みクラスの生成
        // 
        // 第2引数は末尾追加の有無。falseなら既に存在している場合は上書きする
        // 第3引数はエンコード。一応Unicode指定
        //--------------------------------
        string path = strSavePath + "/" + strSaveKey + SUFFIX_TEXT;
        try
        {
            System.IO.StreamWriter cStreamWriter = new System.IO.StreamWriter(path, false, System.Text.Encoding.Unicode);

            //--------------------------------
            // 書き込みクラスに対してデータを指定して上書きして閉じる
            //--------------------------------
            try
            {
                cStreamWriter.Write(strSaveData);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(string.Format("MasterData LocalSave Failure source : {0} {1}", strSaveKey, ex));
                return false;
            }
            finally
            {
                cStreamWriter.Close();
            }
        }
        catch (System.Exception ex2)
        {
            //----------------------------------------------------------------------------------------
            // 一応上記以外のエラーも拾うようにしたが、公式で確認するとStreamWriter.Write メソッドのエラーパラメータは上記3つのみ
            //----------------------------------------------------------------------------------------
            Debug.LogWarning(string.Format("MasterData LocalSave Failure source : {0} ", strSaveKey));
            return false;
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("MasterData LocalSave - " + strSaveKey);
#endif
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ロード実行：
	*/
    //----------------------------------------------------------------------------
    static private string Load(string strLoadPath, string strSaveKey)
    {
        if (strLoadPath == null
        || strLoadPath.Length <= 0
        )
        {
            Debug.LogError("InstallFolder Load Error! Path None ... ");
            return null;
        }

        string strRetValue = "";

        //--------------------------------
        // ファイルの保存先を算出
        //--------------------------------
        string strLoadFile = strLoadPath + "/" + strSaveKey + SUFFIX_TEXT;

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL LocalSaveUtilToInstallFolder#Load:" + strLoadFile);
#endif

        //--------------------------------
        // ファイルの存在チェック
        //--------------------------------
        if (System.IO.File.Exists(strLoadFile) == false)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log(">File None! : " + strLoadFile);
#endif
            return "";
        }

        //--------------------------------
        // ストリームリーダーを生成して読み込み実行
        // 
        // エラーが発生する可能性を考慮して try catch を使用。
        // try catch どちらのフローに入っても解放がちゃんと走るようにfinallyで解放
        //--------------------------------
        System.IO.StreamReader cStreanmReader = null;
        try
        {
            cStreanmReader = new System.IO.StreamReader(strLoadFile, System.Text.Encoding.Unicode);
            strRetValue = cStreanmReader.ReadToEnd();
        }
        catch (System.Exception cException)
        {
            Debug.LogError(">Exception : " + cException);
        }
        finally
        {
            if (cStreanmReader != null)
            {
                cStreanmReader.Close();
            }
        }

        return strRetValue;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	セーブ実行：
    */
    //----------------------------------------------------------------------------
    static private bool SaveByte(string strSavePath, string strSaveKey, byte[] bSaveData)
    {
        if (strSavePath == null
        || strSavePath.Length <= 0
        )
        {
            Debug.LogError("InstallFolder Save Byte Error! Path None ... ");
            return false;
        }

        //--------------------------------
        // ファイルの保存先を算出
        //--------------------------------
        string strSaveFile = strSavePath + "/" + strSaveKey + SUFFIX_BYTES;

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL LocalSaveUtilToInstallFolder#SaveByte:" + strSavePath);
#endif
        //--------------------------------
        // ファイル書き込みクラスの生成
        //--------------------------------
        try
        {
            //--------------------------------
            // 書き込みクラスに対してデータを指定して上書きして閉じる
            //--------------------------------
            System.IO.File.WriteAllBytes(strSaveFile, bSaveData);
        }
        catch (System.Exception err)
        {
            Debug.LogWarning(string.Format("MasterData LocalByteSave Failure source : {0} by {1}", strSaveKey, err.Message));
            return false;
        }
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	ロード実行：
    */
    //----------------------------------------------------------------------------
    static private byte[] LoadByte(string strLoadPath, string strSaveKey)
    {
        if (strLoadPath == null
        || strLoadPath.Length <= 0
        )
        {
            Debug.LogError("InstallFolder Load Byte Error! Path None ... ");
            return new byte[0];
        }

        byte[] bMasterData;

        //--------------------------------
        // ファイルの読み込み
        //--------------------------------
        string strLoadFile = strLoadPath + "/" + strSaveKey + SUFFIX_BYTES;

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL LocalSaveUtilToInstallFolder#LoadByte:" + strLoadFile);
#endif

        //--------------------------------
        // ファイルの存在チェック
        //--------------------------------
        if (System.IO.File.Exists(strLoadFile) == false)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log(">File None! : " + strLoadFile);
#endif
            return new byte[0];
        }

        //--------------------------------
        // ストリームリーダーを生成して読み込み実行
        // 
        // エラーが発生する可能性を考慮して try catch を使用。
        // try catch どちらのフローに入っても解放がちゃんと走るようにfinallyで解放
        //--------------------------------
        try
        {
            bMasterData = System.IO.File.ReadAllBytes(strLoadFile); // 読み込み
        }
        catch (System.Exception cException)
        {
            Debug.LogError(">Exception : " + cException);
            bMasterData = new byte[0];
        }

        return bMasterData;
    }

    /*
		AssetBundle関連のロードトセーブ
	 */
    static public bool SaveAsssetBundle(string strSaveKey, byte[] bSaveData)
    {
        return SaveByte(GetSavePathAssetBundle(), strSaveKey, bSaveData);
    }
    static public byte[] LoadAsssetBundle(string strSaveKey)
    {
        return LoadByte(GetSavePathAssetBundle(), strSaveKey);
    }

    static public string LoadAsssetBundlePath(string strSaveKey)
    {
        string strLoadPath = GetSavePathAssetBundle();

        if (strLoadPath == null ||
            strLoadPath.Length <= 0)
        {
            Debug.LogError("InstallFolder Load path Error! Path None ... ");
            return null;
        }

        return strLoadPath + "/" + strSaveKey + SUFFIX_BYTES;
    }

    static public void RemoveAsssetBundle(string strSaveKey)
    {
        string strRemoveFile = GetSavePathAssetBundle() + "/" + strSaveKey + SUFFIX_BYTES;
        //--------------------------------
        // ファイルの存在チェック
        //--------------------------------
        if (System.IO.File.Exists(strRemoveFile) == false)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log(">File None! : " + strRemoveFile);
#endif
            return;
        }

        System.IO.File.Delete(strRemoveFile);
    }

    private static string ASEETBUNDLE_VERSION_FILENAME = "assetbundleversionscache" + SUFFIX_BYTES;
    private static string ASEETBUNDLE_VERSION_PATH = "";

    static public string getAssetbundleVersionPath()
    {
        if (ASEETBUNDLE_VERSION_PATH.Length > 0)
        {
            return ASEETBUNDLE_VERSION_PATH;
        }

        ASEETBUNDLE_VERSION_PATH = GetSavePathAssetBundle() + "/" + ASEETBUNDLE_VERSION_FILENAME;

        return ASEETBUNDLE_VERSION_PATH;
    }

    static public void SaveAssetBundleVersions(Dictionary<string, uint> values)
    {
        string path = getAssetbundleVersionPath();
        try
        {
            ES2.Save(values, path);
        }
        catch (Exception e)
        {
        }
    }

    static public Dictionary<string, uint> LoadAssetBundleVersions()
    {
        string path = getAssetbundleVersionPath();

        if (ES2.Exists(path))
        {
            Dictionary<string, uint> dic = new Dictionary<string, uint>();
            try
            {
                dic = ES2.LoadDictionary<string, uint>(path);
            }
            catch (Exception e)
            {
                dic = new Dictionary<string, uint>();
            }

            return dic;
        }
        else
        {
            return new Dictionary<string, uint>();
        }
    }

    /*
       バトル関連のロードトセーブ
    */
    private static string BATTLE_SETTING_FILENAME = "_battlesetting" + SUFFIX_BYTES;
    private static string BATTLE_SETTING_PATH = "";

    static public string getBattleSettingPath(string prefix)
    {
        BATTLE_SETTING_PATH = GetSavePathBattle() + "/" + prefix + BATTLE_SETTING_FILENAME;

        return BATTLE_SETTING_PATH;
    }

    static private ES2Settings Es2ButtleSetting(string prefix)
    {
        // memo
        // UUIDだけだと他と同じなので適当なキーを追加
        // キーを変えると復号できなくなるので注意
        ES2Settings setting = new ES2Settings();
        setting.encrypt = true;
        setting.encryptionPassword = prefix + "000000" + LocalSaveManager.Instance.LoadFuncUUID();

        return setting;
    }

    static private string Sha1BattleSetting(string values)
    {
        System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
        var sha1Bs = sha1.ComputeHash(new UTF8Encoding().GetBytes(values));
        return BitConverter.ToString(sha1Bs).ToLower().Replace("-", "");
    }

    static public void SaveBattleSetting(string prefix, string values)
    {
        if (prefix.IsNullOrEmpty())
        {
            return;
        }

        if (values.IsNullOrEmpty())
        {
            RemoveBattleSetting(prefix);
            return;
        }

        string shar1 = Sha1BattleSetting(values);
        Dictionary<string, string> savedata = new Dictionary<string, string>();

        savedata.Add(shar1, values);
        string path = getBattleSettingPath(prefix);

        try
        {
            ES2.Save(savedata, path, Es2ButtleSetting(prefix));
        }
        catch (Exception e)
        {
        }
    }

    static public string LoadBattleSetting(string prefix)
    {
        if (prefix.IsNullOrEmpty())
        {
            return "";
        }

        string path = getBattleSettingPath(prefix);

        if (ES2.Exists(path) == false)
        {
            return "";
        }

        Dictionary<string, string> savedata = null;
        try
        {
            savedata = ES2.LoadDictionary<string, string>(path, Es2ButtleSetting(prefix));
        }
        catch (Exception e)
        {
            return "";
        }

        if (savedata.Count != 1)
        {
            return "";
        }

        foreach (KeyValuePair<string, string> kvp in savedata)
        {
            string shar1 = Sha1BattleSetting(kvp.Value);
            if (kvp.Key == shar1)
            {
                return kvp.Value;
            }
            else
            {
                return "";
            }
        }

        return "";
    }

    static public void RemoveBattleSetting(string prefix)
    {
        if (prefix.IsNullOrEmpty())
        {
            return;
        }

        string path = getBattleSettingPath(prefix);

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }

    /*
       ポップアップWebView関連のロードトセーブ
    */
    private static string WEBVIEW_CHECK_FILENAME = "webviewcheck" + SUFFIX_BYTES;
    private static string WEBVIEW_CHECK_PATH = "";

    static public string getWebviewCheckPath()
    {
        WEBVIEW_CHECK_PATH = GetSavePathPreference() + "/" + WEBVIEW_CHECK_FILENAME;

        return WEBVIEW_CHECK_PATH;
    }

    static public void SaveWebviewCheck(Dictionary<uint, uint> values)
    {
        string path = getWebviewCheckPath();
        try
        {
            ES2.Save(values, path);
        }
        catch (Exception e)
        {
        }
    }

    static public Dictionary<uint, uint> LoadWebviewCheck()
    {
        string path = getWebviewCheckPath();

        if (ES2.Exists(path))
        {
            Dictionary<uint, uint> dic;
            try
            {
                dic = ES2.LoadDictionary<uint, uint>(path);
            }
            catch (Exception e)
            {
                dic = new Dictionary<uint, uint>();
            }

            return dic;
        }
        else
        {
            RemoveWebviewCheck();
            return new Dictionary<uint, uint>();
        }
    }

    static public void RemoveWebviewCheck()
    {
        string path = getWebviewCheckPath();

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }

    /*
       WebResource関連のロードトセーブ
    */
    static public byte[] LoadWebResource(string key)
    {
        string path = GetSavePathWebResource();

        return LoadByte(path, key);
    }

    static public void SaveWebResource(string key, byte[] data)
    {
        string path = GetSavePathWebResource();

        SaveByte(path, key, data);
    }

    static public void RemoveWebResource()
    {
        string path = GetSavePathWebResource();

        if (System.IO.Directory.Exists(path))
        {
            try
            {
                //ディレクトリ内にあるファイル削除
                string[] files = System.IO.Directory.GetFiles(path);
                for (int i = 0; i < files.Length; i++)
                {
                    System.IO.File.Delete(files[i]);
                }

                System.IO.Directory.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
