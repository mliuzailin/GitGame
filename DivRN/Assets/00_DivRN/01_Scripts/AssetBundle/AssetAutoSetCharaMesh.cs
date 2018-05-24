/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	AssetAutoSetCharaMesh.cs
	@brief	AssetBundle操作：リソース自動セットクラス：キャラメッシュ
	@author Developer
	@date 	2013/04/30

	AssetBundleの読み込み待ちとかメッシュ複製とかを色んなとこでやるのは面倒なのでクラス化。
	以下の処理を行う。

	１．（外部処理）GameObjectに対してAddComponentされる
	２．（外部処理）キャラIDを指定される

	３．（内部処理）AssetBundleManagerに対してキャラIDに対応したメッシュの読み込み指示
	４．（内部処理）AssetBundleの読み込み待ち
	５．（内部処理）読み込みが完了してキャラメッシュのPrefabが取得できたら複製してgameObjectの子として登録
	６．（内部処理）キャラのマスターデータを取得してサイズを補正

*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*==========================================================================*/
/*		script easy to find 												*/
/*==========================================================================*/
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
	@brief	AssetBundle操作：リソース自動セットクラス：キャラメッシュ
*/
//----------------------------------------------------------------------------
public class AssetAutoSetCharaMesh : MonoBehaviour
{
    const float DEF_SCALE_UP_RATE = 0.1f; //!< 自動サイズ補正：割合
    const float DEF_SCALE_UP_SIZE = 512.0f; //!< 自動サイズ補正：最大サイズ

    const uint CHARAMESH_BODY = (1 << 0);
    const uint CHARAMESH_SHADOW = (1 << 1);

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private uint m_AutoSetCharaID = 0; //!< 自動設定情報：キャラID

    private uint m_AutoSetCharaType = CHARAMESH_BODY; //!< 自動設定情報：キャラ影有無

    private AssetBundler m_AssetBundleResource = null;

    public bool m_AssetBundleAttachPause = false;

    public bool m_AssetBundleMeshScaleUp = false; // 自動で拡大する設定
    public bool m_AssetBundleMeshPosition = false; // 自動で位置調整

    private bool m_AssetBundleResourceGetFlg = false; // v310 bugweb4370対応 20151207add

    protected Coroutine m_LoadCoroutine = null;
    protected Texture2D m_Texture = null;
    protected Image m_CharaImage = null;
    protected Image m_ShadowImage = null;
    protected RectTransform m_CharaRect = null;
    protected RectTransform m_ShadowRect = null;
    protected MasterDataParamChara cMasterDataChara;

    private bool m_Ready = false;
    public bool Ready { get { return m_Ready; } }

    private bool m_UseUncompressed = false;
    public bool UseUncompressed { get { return m_UseUncompressed; } set { m_UseUncompressed = value; } }

    public Image charaImage { get { return m_CharaImage; } }
    public Image shadowImage { get { return m_ShadowImage; } }

    private AssetBundler m_AssetBundler = null;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief	キャラIDを指定してメッシュ構築リクエスト
        @note
        @param[in]	uint	( unCharaID				) キャラID
        @param[in]	bool	( bShadowActive			) 影メッシュ有無
        @param[in]	---		( ---------------------	) ---------------------
        @param[out]	void	( ---------------------	) ---------------------
    */
    //----------------------------------------------------------------------------
    public void SetCharaID(uint unCharaID, bool bShadowActive)
    {
        m_Ready = false;
        m_AutoSetCharaID = unCharaID;
        if (bShadowActive == true)
        {
            m_AutoSetCharaType = CHARAMESH_BODY | CHARAMESH_SHADOW;
        }
        else
        {
            m_AutoSetCharaType = CHARAMESH_BODY;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	キャラIDを指定してメッシュ構築リクエスト
        @note
        @param[in]	uint	( unCharaID				) キャラID
        @param[in]	bool	( bShadowActive			) 影メッシュ有無
        @param[in]	bool	( bBodyActive			) 本体メッシュ有無
        @param[in]	---		( ---------------------	) ---------------------
        @param[out]	void	( ---------------------	) ---------------------
    */
    //----------------------------------------------------------------------------
    public void SetCharaID(uint unCharaID, bool bShadowActive, bool bBodyActive)
    {
        m_Ready = false;
        m_AutoSetCharaID = unCharaID;
        m_AutoSetCharaType = 0;

        if (bBodyActive == true)
        {
            m_AutoSetCharaType |= CHARAMESH_BODY;
        }
        if (bShadowActive == true)
        {
            m_AutoSetCharaType |= CHARAMESH_SHADOW;
        }
    }

    /// <summary>
    /// テクスチャを直接設定する
    /// </summary>
    /// <param name="charaTexture"></param>
    /// <param name="bShadowActive"></param>
    public void SetCharaTexture(MasterDataParamChara charaMaster, Texture2D charaTexture, bool bShadowActive)
    {
        m_Ready = false;
        cMasterDataChara = charaMaster;

        if (bShadowActive == true)
        {
            m_AutoSetCharaType = CHARAMESH_BODY | CHARAMESH_SHADOW;
        }
        else
        {
            m_AutoSetCharaType = CHARAMESH_BODY;
        }

        if (m_LoadCoroutine != null)
        {
            StopCoroutine(m_LoadCoroutine);
            m_LoadCoroutine = null;
        }

        Set(charaTexture);
    }


    //----------------------------------------------------------------------------
    /*!
        @brief	キャラメッシュ破棄
    */
    //----------------------------------------------------------------------------
    public bool DestroyCharaMesh()
    {
        return true;
    }


    private void Awake()
    {
        GameObject CharaImageObj = UnityUtil.GetChildNode(gameObject, "CharaImage");
        if (CharaImageObj != null)
        {
            m_CharaImage = CharaImageObj.GetComponent<Image>();
            m_CharaImage.enabled = false;
            m_CharaRect = CharaImageObj.GetComponent<RectTransform>();
        }
        GameObject ShadowImageObj = UnityUtil.GetChildNode(gameObject, "ShadowImage");
        if (ShadowImageObj != null)
        {
            m_ShadowImage = ShadowImageObj.GetComponent<Image>();
            m_ShadowImage.enabled = false;
            m_ShadowRect = ShadowImageObj.GetComponent<RectTransform>();
        }
    }
    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
    */
    //----------------------------------------------------------------------------
    void Start()
    {
        if (m_Ready == false)
        {
            m_LoadCoroutine = StartCoroutine(RepeatGet());
        }
    }

    private IEnumerator RepeatGet()
    {
        while (cMasterDataChara == null)
        {
            yield return new WaitForEndOfFrame();
            //----------------------------------------
            // マスターデータ取得
            //----------------------------------------
            cMasterDataChara = MasterDataUtil.GetCharaParamFromID(m_AutoSetCharaID);

            if (cMasterDataChara == null)
            {
                Debug.LogError("CharaID Error! - " + m_AutoSetCharaID);
            }
            else
            {
                m_AssetBundler = AssetBundler.Create().
                    SetAsUnitTexture(
                        m_AutoSetCharaID,
                        (o) =>
                        {
                            if (m_UseUncompressed)
                            {
                                Set(o.GetUncompressedTexture2D());
                            }
                            else
                            {
                                Set(o.GetTexture2D(TextureWrapMode.Clamp));
                            }
                        },
                        (str) =>
                        {
                            m_Ready = true;
                        }).
                    Load();
            }
        }
    }

    private void Set(Texture2D cAssetBundleTexture)
    {
        //--------------------------------
        // Pivotによって高さ補正
        //--------------------------------
        Vector3 vEnemyBaseScale = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 vEnemyBasePosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (m_AssetBundleMeshPosition == true)
        {
            //ユニット詳細などで中心位置がずれる際の対応。ベースのy座標をずらす
            vEnemyBasePosition.y += 155.0f;
        }

        if (cMasterDataChara != null)
        {
            vEnemyBaseScale = new Vector3(cMasterDataChara.size_width, cMasterDataChara.size_height, 1.0f);

            //--------------------------------
            // 小さい子を自動で大きくする設定の場合、
            // サイズに合わせて適当な大きさに補正をかける。
            //--------------------------------
            if (m_AssetBundleMeshScaleUp == true)
            {
                float fScaleRate = DEF_SCALE_UP_RATE;
                float fScaleRateInv = 1.0f - fScaleRate;
                vEnemyBaseScale.x = (vEnemyBaseScale.x * fScaleRateInv) + (DEF_SCALE_UP_SIZE * fScaleRate);
                vEnemyBaseScale.y = (vEnemyBaseScale.y * fScaleRateInv) + (DEF_SCALE_UP_SIZE * fScaleRate);
            }

            vEnemyBaseScale = new Vector3(vEnemyBaseScale.x / cMasterDataChara.size_width, vEnemyBaseScale.y / cMasterDataChara.size_height, 1.0f);

            Vector3 vEnemyUnderPos = new Vector3(0.0f, -155.0f, 0.0f);

            //--------------------------------
            //
            //--------------------------------
            //		float fCharaWidth	= cCharaMasterData.size_width;
            float fCharaHeight = vEnemyBaseScale.y;

            //--------------------------------
            // メッシュの原点によって位置補正
            //--------------------------------
            if (cMasterDataChara.pivot == MasterDataDefineLabel.PivotType.BOTTOM)
            {
                //--------------------------------
                // 下原点のキャラなら固定座標分下に送る
                //--------------------------------
                vEnemyBasePosition.y += vEnemyUnderPos.y + (fCharaHeight * 0.5f);
            }
            else if (cMasterDataChara.pivot == MasterDataDefineLabel.PivotType.CENTER)
            {
                //--------------------------------
                // 中央原点なら補正せずに縦方向は0.0で出す
                //--------------------------------
            }
        }
        if (m_CharaImage != null && cAssetBundleTexture != null)
        {
            setMainTexture(cAssetBundleTexture, vEnemyBaseScale);
        }
        //----------------------------------------
        // 影部分を作成
        //----------------------------------------
        if ((m_AutoSetCharaType & CHARAMESH_SHADOW) != 0)
        {
            Material cMaterialEnemyShadow = null;
            if (SceneObjReferMainMenu.Instance != null)
            {
                switch (cMasterDataChara.element)
                {
                    case MasterDataDefineLabel.ElementType.NAUGHT:
                        cMaterialEnemyShadow = SceneObjReferMainMenu.Instance.m_MaterialEnemyShadowNaught;
                        break; // スキルカットイン：無
                    case MasterDataDefineLabel.ElementType.FIRE:
                        cMaterialEnemyShadow = SceneObjReferMainMenu.Instance.m_MaterialEnemyShadowFire;
                        break; // スキルカットイン：炎
                    case MasterDataDefineLabel.ElementType.WATER:
                        cMaterialEnemyShadow = SceneObjReferMainMenu.Instance.m_MaterialEnemyShadowWater;
                        break; // スキルカットイン：水
                    case MasterDataDefineLabel.ElementType.LIGHT:
                        cMaterialEnemyShadow = SceneObjReferMainMenu.Instance.m_MaterialEnemyShadowLight;
                        break; // スキルカットイン：光
                    case MasterDataDefineLabel.ElementType.DARK:
                        cMaterialEnemyShadow = SceneObjReferMainMenu.Instance.m_MaterialEnemyShadowDark;
                        break; // スキルカットイン：闇
                    case MasterDataDefineLabel.ElementType.WIND:
                        cMaterialEnemyShadow = SceneObjReferMainMenu.Instance.m_MaterialEnemyShadowWind;
                        break; // スキルカットイン：緑
                    default:
                        Debug.LogError("Material NG!! - " + cMasterDataChara.element);
                        break;
                }
            }
            //--------------------------------
            // @change Developer 2016/03/07 v330 板ポリ対応
            // @note   描画順に問題があるためNGUIオブジェ化
            //--------------------------------
            if (m_ShadowImage != null && cAssetBundleTexture != null)
            {
                setShadowTexture(cAssetBundleTexture, vEnemyBaseScale, cMaterialEnemyShadow);
            }
        }

        m_Texture = cAssetBundleTexture;
        m_Ready = true;
    }

    protected virtual void setMainTexture(Texture2D cTexture, Vector3 scale)
    {
        m_CharaImage.sprite = Sprite.Create(cTexture, new Rect(0, 0, cTexture.width, cTexture.height), Vector2.zero);
        m_CharaImage.color = new Color(1, 1, 1, 1);
        m_CharaImage.SetNativeSize();
        m_CharaImage.enabled = true;
        if (m_AssetBundleMeshScaleUp) m_CharaImage.transform.localScale = new Vector3(scale.x, scale.y, 1.0f);
        m_CharaRect.sizeDelta = new Vector2(cTexture.GetUnitTextureWidth(), cTexture.GetUnitTextureHeight());
    }

    protected virtual void setShadowTexture(Texture2D cTexture, Vector3 scale, Material material)
    {
        m_ShadowImage.sprite = Sprite.Create(cTexture, new Rect(0, 0, cTexture.width, cTexture.height), Vector2.zero);
        m_ShadowImage.material = Instantiate(material) as Material;
        m_ShadowImage.color = new Color(1, 1, 1, 1);
        m_ShadowImage.SetNativeSize();
        m_ShadowImage.enabled = true;
        if (m_AssetBundleMeshScaleUp) m_ShadowImage.transform.localScale = new Vector3(scale.x, scale.y, 1.0f);
        m_ShadowRect.sizeDelta = new Vector2(cTexture.GetUnitTextureWidth(), cTexture.GetUnitTextureHeight());
    }

    private void OnDestroy()
    {
        if (m_AssetBundler != null &&
            m_Ready == false)
        {
            m_AssetBundler.Destroy();
        }

        //非圧縮テクスチャの場合は明示的にアンロードする。
        if (m_Texture != null && m_UseUncompressed)
        {
            Resources.UnloadAsset(m_Texture);
        }
    }
}


/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
