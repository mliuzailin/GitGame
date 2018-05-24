#define DEF_OBJECT_ENABLE_QUICK			//!< オブジェクト有効無効切り替え処理の高速化対応

/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	UnityUtil.cs
	@brief	Unity関連ユーティリティ
	@author Developer
	@date 	2012/12/05
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
	@brief	Unity関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class UnityUtil
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private static bool m_ResizeScaleCalc = false;  //!< 計算処理を行ったかフラグ
    private static float m_ResizeAddHeight = 0.0f;     //!< 増加した実数
    private static float m_StatusbarHeight = 0.0f;      //!< ステータスバー高さ計算結果保持用

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトを親に接続する
		@note	姿勢の逆行列計算とか走ってposition , scale の値が変化してしまうので「セットアップ→姿勢差し戻し」の処理を行う
		@param[in]	GameObject	( cChildObj				) オブジェクト子供
		@param[in]	GameObject	( cParentObj			) オブジェクト親
		@param[in]	Vector3		( rvLocalPosOffset		) ローカル位置オフセット
		@param[out]	bool		( ---------------------	) 成否
	*/
    //----------------------------------------------------------------------------
    static public bool SetParentNode(GameObject cObjChild, GameObject cObjParent, ref Vector3 rvLocalPosOffset)
    {
        //------------------------------
        //
        //------------------------------
        if (cObjChild == null
        || cObjParent == null
        ) return false;

        //--------------------------------
        // 親を接続すると、子の姿勢がワールド的に変化しないように勝手に補正が入ってくる。
        //
        // 接続を切り替えるようなオブジェクトの姿勢は相対的なものになっていることが多く、
        // 補正が余計なお世話になるため、姿勢の差し戻しも合わせて行う。
        //--------------------------------
        Vector3 vOriginLocalPos = cObjChild.transform.localPosition;
        Vector3 vOriginLocalScale = cObjChild.transform.localScale;

        cObjChild.transform.parent = cObjParent.transform;
        cObjChild.transform.localScale = vOriginLocalScale;
        cObjChild.transform.localPosition = vOriginLocalPos + rvLocalPosOffset;

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトを親に接続する
		@note	姿勢の逆行列計算とか走ってposition , scale の値が変化してしまうので「セットアップ→姿勢差し戻し」の処理を行う
		@param[in]	GameObject	( cChildObj				) オブジェクト子供
		@param[in]	GameObject	( cParentObj			) オブジェクト親
		@param[out]	bool		( ---------------------	) 成否
	*/
    //----------------------------------------------------------------------------
    static public bool SetParentNode(GameObject cObjChild, GameObject cObjParent)
    {
        //------------------------------
        //
        //------------------------------
        if (cObjChild == null
        || cObjParent == null
        ) return false;

        //--------------------------------
        // 親を接続すると、子の姿勢がワールド的に変化しないように勝手に補正が入ってくる。
        //
        // 接続を切り替えるようなオブジェクトの姿勢は相対的なものになっていることが多く、
        // 補正が余計なお世話になるため、姿勢の差し戻しも合わせて行う。
        //--------------------------------
        Vector3 vOriginLocalPos = cObjChild.transform.localPosition;
        Vector3 vOriginLocalScale = cObjChild.transform.localScale;

        cObjChild.transform.parent = cObjParent.transform;
        cObjChild.transform.localScale = vOriginLocalScale;
        cObjChild.transform.localPosition = vOriginLocalPos;

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	階層以下のオブジェクトのレイヤーを強制的に置き換え
	*/
    //----------------------------------------------------------------------------
    static public void SetObjectLayer(GameObject cRootObj, int nLayer)
    {
        //------------------------------
        // オブジェクトのレイヤーを書き換え
        //------------------------------
        if (cRootObj == null)
            return;
        cRootObj.layer = nLayer;

        //--------------------------------
        // 再帰処理で子供を全て書き換え
        //--------------------------------
#if true // @Change Developer 2015/11/04 warning除去。
        int nChildCount = cRootObj.transform.childCount;
        for (int i = 0; i < nChildCount; i++)
#else
		for( int i = 0;i < cRootObj.transform.GetChildCount(); i++ )
#endif
        {
            SetObjectLayer(cRootObj.transform.GetChild(i).gameObject, nLayer);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ローカルポジション(X)変更
	*/
    //----------------------------------------------------------------------------
    public static void SetLocalPositionX(GameObject cObject, float fPosX)
    {
        if (cObject == null)
            return;

        // 表示位置修正
        Vector3 pos = cObject.transform.localPosition;
        pos.x = fPosX;
        cObject.transform.localPosition = pos;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ローカルポジション(Y)変更
	*/
    //----------------------------------------------------------------------------
    public static void SetLocalPositionY(GameObject cObject, float fPosY)
    {
        if (cObject == null)
            return;

        // 表示位置修正
        Vector3 pos = cObject.transform.localPosition;
        pos.y = fPosY;
        cObject.transform.localPosition = pos;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	名称指定で子ノード探索
		@note	似た処理にFind関数があるが、そちらは非アクティブなオブジェクトが対象外になるので再帰で対応
	*/
    //----------------------------------------------------------------------------
    static public GameObject GetChildNode(GameObject cRootObj, string strNodeName)
    {
        //------------------------------
        //
        //------------------------------
        if (cRootObj == null)
            return null;
        if (cRootObj.name == strNodeName)
            return cRootObj;

        //--------------------------------
        // 再帰処理で子供を全てチェック
        //--------------------------------
#if true // @Change Developer 2015/11/04 warning除去。
        int nChildCount = cRootObj.transform.childCount;
        for (int i = 0; i < nChildCount; i++)
#else
		for( int i = 0;i < cRootObj.transform.GetChildCount(); i++ )
#endif
        {
            GameObject cChildObj = cRootObj.transform.GetChild(i).gameObject;
            if (cChildObj == null)
                continue;

            GameObject cRetObj = GetChildNode(cChildObj, strNodeName);
            if (cRetObj == null)
                continue;

            return cRetObj;
        }
        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	名称指定で子ノード探索
		@note	似た処理にFind関数があるが、そちらは非アクティブなオブジェクトが対象外になるので再帰で対応
	*/
    //----------------------------------------------------------------------------
    static public GameObject GetChildNode(GameObject cRootObj, string strNodeName, string strNodeName2)
    {
        return GetChildNode(GetChildNode(cRootObj, strNodeName), strNodeName2);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	コンポーネントを保持するオブジェクトの探索
	*/
    //----------------------------------------------------------------------------
    static public int SearchComponentObj(GameObject cRootObj, string strComponentName, ref GameObject[] acSearchObj, ref int nSearchObjTotal)
    {
        //------------------------------
        //
        //------------------------------
        if (cRootObj == null)
            return nSearchObjTotal;

        //------------------------------
        // コンポーネント保持チェック
        //------------------------------
        if (cRootObj.GetComponent(strComponentName) != null)
        {
            if (nSearchObjTotal < acSearchObj.Length)
            {
                acSearchObj[nSearchObjTotal++] = cRootObj;
            }
            else
            {
                Debug.LogError("SearchComponentObj Buffer Over! - " + cRootObj.name);
            }
        }

        //--------------------------------
        // 再帰処理で子供を全てチェック
        //--------------------------------
#if true // @Change Developer 2015/11/04 warning除去。
        int nChildCount = cRootObj.transform.childCount;
        for (int i = 0; i < nChildCount; i++)
#else
		for( int i = 0;i < cRootObj.transform.GetChildCount(); i++ )
#endif
        {
            GameObject cChildObj = cRootObj.transform.GetChild(i).gameObject;
            if (cChildObj == null)
                continue;

            SearchComponentObj(cChildObj, strComponentName, ref acSearchObj, ref nSearchObjTotal);
        }

        return nSearchObjTotal;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからDraggablePanelを探して要素が画面内に収まるレベルならロックして動かないようにする(縦)
	*/
    //----------------------------------------------------------------------------
    static public float CalcResizeAddHeight()
    {
        // UIリサイズ用の値計算 何回計算しても意味がないので一回だけ
        if (m_ResizeScaleCalc == false)
        {
            float h = (float)Screen.height;
            float w = (float)Screen.width;

            // Andriod版のみステータスバー分の高さを含まない値をScreen.heightが返しているため、
            // ステータスバーサイズを取得して加算する
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect");
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");	// クラス参照
			AndroidJavaObject window = unity.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getWindow");
			window.Call<AndroidJavaObject>("getDecorView").Call("getWindowVisibleDisplayFrame", rect);
			h += (float)rect.Get<int>("top");
#endif

            float screen_ratio = h / w;
            // 比率が変化し、尚且つ縦幅が大きい時のみ計算を適用
            if (h > w && screen_ratio >= 1.5f)
            {
                // 高さ増加量 = 現在の画面のアス比 * デフォルト画面幅 / デフォルト画面高さ
                m_ResizeAddHeight = screen_ratio * GlobalDefine.SCREEN_SIZE_W - GlobalDefine.SCREEN_SIZE_H;
            }
            else
            {

                m_ResizeAddHeight = 0.0f;
            }

            m_ResizeScaleCalc = true;
        }

        return m_ResizeAddHeight;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからDraggablePanelを探して位置関係をリセット
	*/
    //----------------------------------------------------------------------------
    //	static	public void AddComponent( GameObject cObj , string strComponentName )
    static public void AddComponent<T>(GameObject cObj) where T : Component
    {
        if (cObj == null)
            return;

        T cComponent = cObj.GetComponent<T>();
        if (cComponent != null)
            return;

        cComponent = cObj.AddComponent<T>();
        if (cComponent == null)
        {
            Debug.LogError("Component None! - " + typeof(T).Name);
            return;
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUILabelを探して引数を入力
	*/
    //----------------------------------------------------------------------------
    static public void SetUILabelValue(GameObject cObj, uint unValue)
    {
        if (cObj == null)
            return;
        Text cLabel = cObj.GetComponent<Text>();
        if (cLabel == null)
            return;

        cLabel.text = unValue.ToString();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUILabelを探して引数を入力
	*/
    //----------------------------------------------------------------------------
    static public void SetUILabelValue(GameObject cObj, int nValue)
    {
        if (cObj == null)
            return;
        Text cLabel = cObj.GetComponent<Text>();
        if (cLabel == null)
            return;

        cLabel.text = nValue.ToString();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUILabelを探して引数を入力
	*/
    //----------------------------------------------------------------------------
    static public void SetUILabelValue(GameObject cObj, string strValue)
    {
        if (cObj == null)
            return;
        Text cLabel = cObj.GetComponent<Text>();
        if (cLabel == null)
            return;

        cLabel.text = strValue;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	文字数取得（半角考慮）
	*/
    //----------------------------------------------------------------------------
    static public int GetTextSize(string text)
    {
        double dTextSizeCnt = 0; //文字数カウント用（半角来た時は+0.5する）
        int nTextNum = 0; //文字数(半角文字は２文字で「1」とする)

        if (text != null)
        {
            text = text.Replace("\\n", "\n");

            for (int i = 0, imax = text.Length; i < imax;)
            {
                if (IsHankakuChk(text[i].ToString()))
                {
                    //半角文字の場合
                    dTextSizeCnt += GetHankakuSize(text[i].ToString());
                }
                else
                {
                    ++dTextSizeCnt;
                }

                i++;
            }

            nTextNum = (int)Math.Ceiling(dTextSizeCnt);
        }

        return nTextNum;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	半角チェック
	*/
    //----------------------------------------------------------------------------
    public static bool IsHankakuChk(string str)
    {
        Encoding sjisEnc = Encoding.GetEncoding("utf-8");
        int num = sjisEnc.GetByteCount(str);
        return num == str.Length;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	半角英数字かをチェック
	*/
    //----------------------------------------------------------------------------
    public static bool IsEnglishOrNumChk(string str)
    {
        return (Regex.Match(str, "^[a-zA-Z0-9]+$")).Success;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	半角文字文字サイズ取得
	*/
    //----------------------------------------------------------------------------
    public static float GetHankakuSize(string str)
    {
        float fHankaku = 0.6f;
        if (IsEnglishOrNumChk(str))
        {
            fHankaku = 0.7f;
        }
        return fHankaku;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ユーザーIDから表示用IDを算出
	*/
    //----------------------------------------------------------------------------
    static public uint CreateFriendUserID(string strUserIDBase)
    {
        //--------------------------------
        // 入力された文字列を数値型に変換
        //--------------------------------
        string strUserIDConv = strUserIDBase.Replace(".", "");
        int nGetUserID = 0;
        bool bGetUserIDOK = int.TryParse(strUserIDConv, out nGetUserID);
        if (bGetUserIDOK == false)
        {
            return 0;
        }

        //--------------------------------
        // 表示用のIDを作るのと逆順で変換
        //--------------------------------
        uint unUserID = (uint)nGetUserID;
        unUserID = SwapNumber(unUserID, 7, 0);
        unUserID = SwapNumber(unUserID, 4, 2);
        unUserID = SwapNumber(unUserID, 6, 3);

        return unUserID;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ユーザーIDから表示用IDを算出
	*/
    //----------------------------------------------------------------------------
    static public string CreateDrawUserID(uint unUserIDBase)
    {
        string strUserID = "";
        uint unUserID = unUserIDBase;

        //--------------------------------
        // 適当な数値スワップ
        //--------------------------------
        unUserID = SwapNumber(unUserID, 0, 7);
        unUserID = SwapNumber(unUserID, 2, 4);
        unUserID = SwapNumber(unUserID, 3, 6);

#if BUILD_TYPE_DEBUG
        //		Debug.Log( "DrawNumber - \n" + unUserIDBase + "\n" + unUserID );
#endif

        //--------------------------------
        // 表示用に文字列成形
        //--------------------------------
        uint unUserID0 = (unUserID / 1000000) % 1000;
        uint unUserID1 = (unUserID / 1000) % 1000;
        uint unUserID2 = (unUserID / 1) % 1000;

        strUserID += (unUserID0 / 100) % 10;
        strUserID += (unUserID0 / 10) % 10;
        strUserID += (unUserID0 / 1) % 10 + ".";
        strUserID += (unUserID1 / 100) % 10;
        strUserID += (unUserID1 / 10) % 10;
        strUserID += (unUserID1 / 1) % 10 + ".";
        strUserID += (unUserID2 / 100) % 10;
        strUserID += (unUserID2 / 10) % 10;
        strUserID += (unUserID2 / 1) % 10;

        return strUserID;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	数値の入れ替え
	*/
    //----------------------------------------------------------------------------
    static public uint SwapNumber(
        uint unNum
    , uint unNumAccess0
    , uint unNumAccess1
    )
    {
        uint unNumKetaRate0 = 1;
        uint unNumKetaRate1 = 1;
        for (int i = 0; i < unNumAccess0; i++) { unNumKetaRate0 *= 10; }
        for (int i = 0; i < unNumAccess1; i++) { unNumKetaRate1 *= 10; }

        uint unNumKeta0 = (unNum / unNumKetaRate0) % 10;
        uint unNumKeta1 = (unNum / unNumKetaRate1) % 10;

        //--------------------------------
        // その桁の数を一旦削除
        //--------------------------------
        unNum -= (unNumKeta0 * unNumKetaRate0);
        unNum -= (unNumKeta1 * unNumKetaRate1);

        //--------------------------------
        // 桁を入れ替えて数を入力
        //--------------------------------
        unNum += (unNumKeta0 * unNumKetaRate1);
        unNum += (unNumKeta1 * unNumKetaRate0);

        return unNum;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUILabelを探してカラーを変更
	*/
    //----------------------------------------------------------------------------
    static public void SetUILabelColor(GameObject cObj, Color cColor)
    {
        if (cObj == null)
            return;
        Text cLabel = cObj.GetComponent<Text>();
        if (cLabel == null)
            return;
        cLabel.color = cColor;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUISpriteを探してカラーを変更
	*/
    //----------------------------------------------------------------------------
    static public void SetUISpriteColor(GameObject cObj, Color cColor)
    {
        if (cObj == null)
            return;
        Image cSprite = cObj.GetComponent<Image>();
        if (cSprite == null)
            return;
        cSprite.color = cColor;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUILabelを探して取得
	*/
    //----------------------------------------------------------------------------
    //static public UILabel GetUILabel(GameObject cObj)
    static public Text GetUILabel(GameObject cObj)
    {
        if (cObj == null)
            return null;
        Text cLabel = cObj.GetComponent<Text>();
        return cLabel;

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUILabelを探して入力文字列を返す
	*/
    //----------------------------------------------------------------------------
    static public string GetUILabelValue(GameObject cObj)
    {

        if (cObj == null)
            return "";
        //UILabel cLabel = cObj.GetComponent< UILabel >();
        Text cLabel = cObj.GetComponent<Text>();
        if (cLabel == null)
            return "";

        return cLabel.text;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUILabelを探して入力文字列を返す
	*/
    //----------------------------------------------------------------------------
    static public int GetUILabelValueInt(GameObject cObj, int nSafetyNum)
    {
        string strLabelText = UnityUtil.GetUILabelValue(cObj);
        strLabelText = strLabelText.Replace(".", "");
        int nLabelInt = 0;
        bool bIntOK = int.TryParse(strLabelText, out nLabelInt);
        if (bIntOK == false)
        {
            return nSafetyNum;
        }

        return nLabelInt;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトからUISpriteを探してテクスチャを切り替え
	*/
    //----------------------------------------------------------------------------
    static public bool SetUIAtlasSprite(GameObject cObj, UIAtlas[] acAtlas, string strSpriteName)
    {
        if (acAtlas == null)
        {
            Debug.LogError("Error - AtlasList None!");
            return false;
        }

        for (int i = 0; i < acAtlas.Length; i++)
        {
            if (acAtlas[i] == null)
                continue;
            if (acAtlas[i].GetSprite(strSpriteName) == null)
                continue;

            return SetUIAtlasSprite(cObj, acAtlas[i], strSpriteName);
        }
        return false;
    }
    //----------------------------------------------------------------------------
    /*!
        @brief	オブジェクトからUISpriteを探してテクスチャを切り替え
    */
    //----------------------------------------------------------------------------
    static public bool SetUIAtlasSprite(GameObject cObj, UIAtlas cAtlas, string strSpriteName)
    {
        if (cObj == null)
        {
            Debug.LogError("Error - Object None!");
            return false;
        }
        Image cSprite = cObj.GetComponent<Image>();
        if (cSprite == null)
        {
            Debug.LogError("Error - SpriteComponent None!");
            return false;
        }

#if UNITY_EDITOR
        //------------------------------
        // Atlas内包チェック
        //------------------------------
        if (cAtlas == null)
        {
            return false;
        }
        if (FindSpriteInAtlas(cAtlas, strSpriteName) == false)
        {
            Debug.LogError("Atlas Find Not Sprite!! - " + strSpriteName);
            return false;
        }
#endif
        {
#if fasle
			if( cSprite.atlas.spriteMaterial.shader.GetInstanceID() != cAtlas.spriteMaterial.shader.GetInstanceID() )
			{
				//-----------------------------
				// シェーダーが一致していない場合、
				// Android機器で不具合が出ることがある。
				//
				//
				//-----------------------------
				Debug.LogError( "Atlas Change Fail!! Shader Discord!!!" + cSprite.atlas.spriteMaterial.shader.name + " -> " + cAtlas.spriteMaterial.shader.name );
				return false;
			}
#endif

            Vector3 cLocalScale = cObj.transform.localScale;
            Vector3 cLocalPosition = cObj.transform.localPosition;

            cSprite.sprite = cAtlas.GetSprite(strSpriteName);
            cSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(cSprite.sprite.textureRect.width, cSprite.sprite.textureRect.height);

            cSprite.transform.localScale = cLocalScale;
            cSprite.transform.localPosition = cLocalPosition;
            return true;
        }

    }
    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクトの有効無効を指定。
		@note	子供階層はたどらない。
	*/
    //----------------------------------------------------------------------------
    static public void SetObjectEnabledOnce(GameObject cObj, bool bEnabled)
    {
        //------------------------------
        //
        //------------------------------
        if (cObj == null)
        {
            return;
        }

#if DEF_OBJECT_ENABLE_QUICK
        //------------------------------
        // オブジェクトの状態を設定。
        //------------------------------
        if (cObj.activeSelf != bEnabled)
        {
            cObj.SetActive(bEnabled);
        }
#else
#if false
		//------------------------------
		// オブジェクトの状態を強制的に有効化。
		// 有効無効を切り替えるとオブジェクトの再生成が走ったりしてメモリが荒れるらしい。
		// オブジェクト自体は常時有効化しておき、コンポーネントのみを排除することで切り替え付加を軽減する
		//------------------------------
		if( cObj.activeSelf == false )
		{
			cObj.SetActive( true );
		}
#else
		//------------------------------
		// オブジェクトの状態を設定。
		//
		// 本来は固定で有効化してコンポーネントだけ切り替えたいが、
		// Unity4になる以前に組んでいた部分で、Animationのデフォルト再生処理を期待している部分があるため一旦こちらで対応。
		// アニメーションの再生指示の調整が完了したら常時有効に切り替える
		//------------------------------
		if( cObj.activeSelf != bEnabled )
		{
			cObj.SetActive( bEnabled );
		}
#endif

		//------------------------------
		// コンポーネント有効無効切り替え
		//------------------------------
		Behaviour[] acComponent = cObj.GetComponents< Behaviour >();
		for( int i = 0;i < acComponent.Length;i++ )
		{
			acComponent[i].enabled = bEnabled;
		}
		Renderer[] acRenderer = cObj.GetComponents< Renderer >();
		for( int i = 0;i < acRenderer.Length;i++ )
		{
			acRenderer[i].enabled = bEnabled;
		}
		Collider[] acCollider = cObj.GetComponents< Collider >();
		for( int i = 0;i < acCollider.Length;i++ )
		{
			acCollider[i].enabled = bEnabled;
		}

		//------------------------------
		// オブジェクトの有効無効状態を保持
		//------------------------------
		{
			ObjectEnabled cComponentEnabled = cObj.GetComponent< ObjectEnabled >();
			if( cComponentEnabled == null )
			{
				cComponentEnabled = cObj.AddComponent< ObjectEnabled >();
			}
			cComponentEnabled.m_ObjEnabled = bEnabled;
		}
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	階層以下のオブジェクトの有効無効を総当たりで指定。
		@note	子供階層も込で全てを有効化。
		@note	Unityはバージョンアップでこの辺の仕様がちょこちょこ変化するため、ユーティリティ化して影響を最小限に抑える
	*/
    //----------------------------------------------------------------------------
    static public void SetObjectEnabled(GameObject cObj, bool bEnabled)
    {
        //------------------------------
        //
        //------------------------------
        if (cObj == null)
        {
            return;
        }

#if DEF_OBJECT_ENABLE_QUICK
        //------------------------------
        // オブジェクトの状態を設定。
        //------------------------------
        if (cObj.activeSelf != bEnabled)
        {
            cObj.SetActive(bEnabled);
        }
#else

		//------------------------------
		// オブジェクトの状態を設定。
		//
		// 本来は固定で有効化してコンポーネントだけ切り替えたいが、
		// Unity4になる以前に組んでいた部分で、Animationのデフォルト再生処理を期待している部分があるため一旦こちらで対応。
		// アニメーションの再生指示の調整が完了したら常時有効に切り替える
		//------------------------------
		if( cObj.activeSelf != bEnabled )
		{
			cObj.SetActive( bEnabled );
		}


		//------------------------------
		// オブジェクトの有効無効フラグを切り替え
		// 若干重い処理なので、フラグが変わってないならコンポーネントの更新を省く
		//------------------------------
		bool bComponentUpdate = true;
		{
			ObjectEnabled cComponentEnabled = cObj.GetComponent< ObjectEnabled >();
			if( cComponentEnabled == null )
			{
				cComponentEnabled = cObj.AddComponent< ObjectEnabled >();
			}
			else
			if( cComponentEnabled.m_ObjEnabled == bEnabled )
			{
				bComponentUpdate = false;
			}
			cComponentEnabled.m_ObjEnabled = bEnabled;
		}

		//------------------------------
		// コンポーネント有効無効切り替え
		//------------------------------
		if( bComponentUpdate == true )
		{
			Behaviour[] acComponent = cObj.GetComponents< Behaviour >();
			for( int i = 0;i < acComponent.Length;i++ )
			{

				if ( acComponent[ i ] == null )
				{
					continue;
				}

				//------------------------------
				// NGUIで使用しているスクリプトの一部は
				// 強制的に有効化すると挙動に不具合が生じるのでスルー
				//------------------------------
				if( acComponent[i].GetType() == typeof(SpringPanel) )
				{
					continue;
				}

				acComponent[i].enabled = bEnabled;
			}
			Renderer[] acRenderer = cObj.GetComponents< Renderer >();
			for( int i = 0;i < acRenderer.Length;i++ )
			{
				acRenderer[i].enabled = bEnabled;
			}
			Collider[] acCollider = cObj.GetComponents< Collider >();
			for( int i = 0;i < acCollider.Length;i++ )
			{
				acCollider[i].enabled = bEnabled;
			}
		}
#endif

        //------------------------------
        // 無効化はトップノードだけオフったら終わりでOK
        //------------------------------
        if (bEnabled == false)
        {
            return;
        }


        //--------------------------------
        // 再帰処理で子供を全てチェック
        //--------------------------------
#if true // @Change Developer 2015/11/04 warning除去。
        int nChildCount = cObj.transform.childCount;
        for (int i = 0; i < nChildCount; i++)
#else
		for( int i = 0;i < cObj.transform.GetChildCount(); i++ )
#endif
        {
            GameObject cChildObj = cObj.transform.GetChild(i).gameObject;
            if (cChildObj == null)
                continue;

            SetObjectEnabled(cChildObj, bEnabled);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	階層以下のオブジェクトの有効無効を総当たりで指定。
		@note	子供階層も込で全てを有効化。
		@note	Unityはバージョンアップでこの辺の仕様がちょこちょこ変化するため、ユーティリティ化して影響を最小限に抑える
	*/
    //----------------------------------------------------------------------------
    static public void SetObjectEnabledFix(GameObject cObj, bool bEnabled)
    {

#if !DEF_OBJECT_ENABLE_QUICK
		//------------------------------
		// 高速化モードでないなら通常処理がMAX判定なのでそちらへ回す
		//------------------------------
		SetObjectEnabled( cObj , bEnabled );
#else

        //------------------------------
        //
        //------------------------------
        if (cObj == null)
        {
            return;
        }

        //------------------------------
        // オブジェクトの状態を設定。
        //------------------------------
        //		bool bComponentUpdate = false;
        if (cObj.activeSelf != bEnabled)
        {
            cObj.SetActive(bEnabled);
        }

        //------------------------------
        // 無効化はトップノードだけオフったら終わりでOK
        //------------------------------
        if (bEnabled == false)
        {
            return;
        }

        //------------------------------
        // コンポーネント有効無効切り替え
        //------------------------------
        //		if( bEnabled == true )
        //		{
        //			bComponentUpdate = true;
        //		}
        //		if( bComponentUpdate == true )
        {
            Behaviour[] acComponent = cObj.GetComponents<Behaviour>();
            for (int i = 0; i < acComponent.Length; i++)
            {

                if (acComponent[i] == null)
                {
                    continue;
                }

                acComponent[i].enabled = bEnabled;
            }
            Renderer[] acRenderer = cObj.GetComponents<Renderer>();
            for (int i = 0; i < acRenderer.Length; i++)
            {
                acRenderer[i].enabled = bEnabled;
            }
            Collider[] acCollider = cObj.GetComponents<Collider>();
            for (int i = 0; i < acCollider.Length; i++)
            {
                acCollider[i].enabled = bEnabled;
            }

        }

        //--------------------------------
        // 再帰処理で子供を全てチェック
        //--------------------------------
#if true // @Change Developer 2015/11/04 warning除去。
        int nChildCount = cObj.transform.childCount;
        for (int i = 0; i < nChildCount; i++)
#else
		for( int i = 0;i < cObj.transform.GetChildCount(); i++ )
#endif
        {
            GameObject cChildObj = cObj.transform.GetChild(i).gameObject;
            if (cChildObj == null)
                continue;

            SetObjectEnabledFix(cChildObj, bEnabled);
        }
#endif

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オブジェクト以下のコンポーネントの有効無効を取得
		@note	Unityはバージョンアップでこの辺の仕様がちょこちょこ変化するため、ユーティリティ化して影響を最小限に抑える
	*/
    //----------------------------------------------------------------------------
    static public bool ChkObjectEnabled(GameObject cObj)
    {
#if DEF_OBJECT_ENABLE_QUICK
        //------------------------------
        //
        //------------------------------
        if (cObj == null)
        {
            return false;
        }
        //		return cObj.activeSelf;
        return cObj.activeInHierarchy;
#else
		//------------------------------
		//
		//------------------------------
		if( cObj == null )
		{
			return false;
		}

		//------------------------------
		// オブジェクトの有効無効状態を取得
		//------------------------------
		ObjectEnabled cComponentEnabled = cObj.GetComponent< ObjectEnabled >();
		if( cComponentEnabled == null )
		{
			//------------------------------
			// オブジェクトのコンポーネントが存在しない場合、
			// 一度も無効化処理を呼んでいないことが保証できるため、オブジェクトの状態を参照
			//------------------------------
			return cObj.activeSelf;
//			return true;
		}

		return cComponentEnabled.m_ObjEnabled;
#endif
    }
    //#if false
    /// <summary>
    ///
    /// </summary>
    /// <param name="cObj"></param>
    /// <param name="bEnable"></param>
    static public void SetCanvasEnable(GameObject cObj, bool bEnable)
    {
        Canvas _tmp = cObj.GetComponentInChildren<Canvas>();
        if (_tmp == null)
        {
            return;
        }
        _tmp.enabled = bEnable;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="cObj"></param>
    /// <returns></returns>
    static public bool ChkCanvasEnable(GameObject cObj)
    {
        Canvas _tmp = cObj.GetComponentInChildren<Canvas>();
        if (_tmp == null) return false;
        return _tmp.enabled;
    }
    //#endif
    static public void SetSortingOrder(GameObject _obj, string layerName)
    {
        Renderer[] renderArray = _obj.GetComponentsInChildren<Renderer>();
        if (renderArray != null)
        {
            for (int i = 0; i < renderArray.Length; i++)
            {
                renderArray[i].sortingLayerName = layerName;
            }
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief		スプライト検索
		@param[in]	UIAtlas		(atlas)				アトラス
		@param[in]	string		(sprite_name)		スプライト名
		@retval		bool		[ある/ない]
		@note		文字列比較なので検索時間がきになる
	*/
    //----------------------------------------------------------------------------
    static public bool FindSpriteInAtlas(UIAtlas atlas, string sprite_name)
    {
        if (atlas == null)
        {
            return false;
        }

        if (atlas.GetSprite(sprite_name) == null)
        {
            return false;
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リソース破棄
		@note		Unityは完全に参照が切れていても、一度読んだリソースは破棄を呼ぶまでは保持してるらしい。
					定期的にリフレッシュさせることでメモリの蓄積を防ぐ。

					シーン切り替えの際には同じ処理をUnityが内部的に呼び出しているらしいので呼ぶ必要はない。
					同一シーン内で、リソースの読み替えが頻発する際には明示的に呼び出すように対応する。
	*/
    //----------------------------------------------------------------------------
    static public void ResourceRefresh()
    {
#if UNITY_EDITOR // @Change Developer 2015/10/28 処理負荷対策。実機ではログを出さないようにする。
        Debug.Log("Resource UnloadUnusedAssets");
#endif

        //--------------------------------
        // リソース破棄
        //--------------------------------
        Resources.UnloadUnusedAssets();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		リソース破棄＋ガベージコレクション
		@note		Unityは完全に参照が切れていても、一度読んだリソースは破棄を呼ぶまでは保持してるらしい。
					定期的にリフレッシュさせることでメモリの蓄積を防ぐ
	*/
    //----------------------------------------------------------------------------
    static public void ResourceRefreshAndGC()
    {
#if BUILD_TYPE_DEBUG && DEBUG_LOG
		Debug.Log( "Resource UnloadUnusedAssets And GC" );
#endif
        //--------------------------------
        // リソース破棄
        //--------------------------------
        Resources.UnloadUnusedAssets();

        //--------------------------------
        // ガベージコレクション
        //--------------------------------
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief		ガベージコレクション強制実行
		@note
	*/
    //----------------------------------------------------------------------------
    static public void ExecGC()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("Exec GC");
#endif
        //--------------------------------
        // ガベージコレクション明示呼び出し
        //--------------------------------
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// フレーム遅延実行
    /// </summary>
    /// <param name="iCount"></param>
    /// <param name="_action"></param>
    /// <returns></returns>
    public static IEnumerator DelayFrameAction(int iCount, System.Action _action)
    {
        for (int i = 0; i < iCount; i++)
        {
            yield return null;
        }
        _action();
    }

    public static T SetupPrefab<T>(string _prefabName, GameObject _parent)
    {
        GameObject _tmpObj = Resources.Load(_prefabName) as GameObject;
        return SetupPrefab<T>(_tmpObj, _parent);
    }

    public static T SetupPrefab<T>(GameObject _prefabObj, GameObject _parent)
    {
        if (_prefabObj != null)
        {
            GameObject _newObj = GameObject.Instantiate(_prefabObj);
            if (_newObj != null)
            {
                _newObj.transform.SetParent(_parent.transform, false);
                return _newObj.GetComponent<T>();
            }
        }
        return default(T);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キーに割り当てられたテキストを返す
	*/
    //----------------------------------------------------------------------------
    public static string GetText(string strKey)
    {
        if (strKey == null ||
            strKey.Length <= 0)
        {
            return " ";
        }

        //マスターからテキスト取得
        string strGetValue = MasterDataUtil.GetTextDefinitionTextFromKey(strKey);

        if (strGetValue == null || strGetValue.Length <= 0)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("TextManager.GetText Key Search NG!! - [ " + strKey + " ] ");
#endif
            //			return " ";
            return "NOT_FOUND_KEY:" + strKey;
        }

        return strGetValue;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	キーに割り当てられたテキストを返す
	*/
    //----------------------------------------------------------------------------
    public static bool GetTextTry(string strKey, ref string strGetMsg)
    {
        strGetMsg = " ";

        //マスターからテキスト取得
        strGetMsg = MasterDataUtil.GetTextDefinitionTextFromKey(strKey);

        if (strGetMsg == null || strGetMsg.Length <= 0)
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("TextManager.GetText Key Search NG!! - [ " + strKey + " ] ");
#endif
            return false;
        }

        return true;
    }

};

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
