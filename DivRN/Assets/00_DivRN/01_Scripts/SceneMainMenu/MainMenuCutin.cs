/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuCutin.cs
	@brief	メインメニュー：カットイン処理
	@author Developer
	@date 	2015/10/14
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
	@brief	カットイン演出
*/
//----------------------------------------------------------------------------
public class MainMenuCutin
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private GameObject m_CutinObj = null;
    private GameObject[] m_BaseObj = null;
    private AnimationClipShot m_BaseAnim = null;
    private GameObject[] m_UnitObj = null;
    private Animation m_UnitAnim = null;
    //	private AssetBundleResource	 m_Resource = null;
    private MasterDataParamChara m_Master = null;
    private Texture2D m_Texture = null;


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    public GameObject cutinObj
    {
        get
        {
            return m_CutinObj;
        }
    }

    //	public AssetBundleResource resource
    //	{
    //		get
    //		{
    //			return m_Resource;
    //		}
    //	}

    public bool isTexture
    {
        get
        {
            return (m_Texture != null);
        }
    }

    public bool isBaseAnimPlay
    {
        get
        {
            if (m_BaseAnim == null)
            {
                return false;
            }

            return m_BaseAnim.ChkAnimationPlaying();
        }
    }

    public bool isUnitAnimPlay
    {
        get
        {
            if (m_UnitAnim == null)
            {
                return false;
            }

            return m_UnitAnim.isPlaying;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セットアップ
		@note	
	*/
    //----------------------------------------------------------------------------
    public bool Setup(GameObject cCutinRoot, uint unUnitID, int nObjType)
    {
        if (cCutinRoot == null)
        {
            return false;
        }

        //--------------------------------
        // オブジェクトタイプによる分岐
        //--------------------------------
        switch (nObjType)
        {
            // 固定ユニット
            case MainMenuDefine.CUTIN_OBJ_TYPE_FIX:
                m_CutinObj = MonoBehaviour.Instantiate(SceneObjReferMainMenu.Instance.m_OriginObjEvolCutinFriend) as GameObject;
                break;

            // 消費ユニット
            case MainMenuDefine.CUTIN_OBJ_TYPE_COST:
                m_CutinObj = MonoBehaviour.Instantiate(SceneObjReferMainMenu.Instance.m_OriginObjEvolCutinParts) as GameObject;
                break;
        }
        if (m_CutinObj == null)
        {
            return false;
        }

        //--------------------------------
        // 基本姿勢設定
        //--------------------------------
        m_CutinObj.transform.parent = cCutinRoot.transform;
        m_CutinObj.transform.localPosition = GlobalDefine.VECTOR_ZERO;
        m_CutinObj.transform.localScale = GlobalDefine.VECTOR_ONE;
        m_CutinObj.transform.localRotation = new Quaternion();

        //--------------------------------
        // アニメーションルートを取得
        //--------------------------------
        GameObject cBaseObjAnim = UnityUtil.GetChildNode(m_CutinObj, "CutinBaseAnim");
        GameObject cUnitObjAnim = UnityUtil.GetChildNode(m_CutinObj, "CutinUnitAnim");

        //--------------------------------
        // 各種設定
        //--------------------------------
        #region ==== 帯オブジェクト ====
        if (cBaseObjAnim != null)
        {
            // NONE定義分、回復属性分「-2」する(キャラ属性に、回復はないため)
            m_BaseObj = new GameObject[(int)MasterDataDefineLabel.ElementType.MAX - 2];

            // 帯オブジェクトを取得
            m_BaseObj[(int)MasterDataDefineLabel.ElementType.FIRE - 1] = UnityUtil.GetChildNode(cBaseObjAnim, "BaseFire");
            m_BaseObj[(int)MasterDataDefineLabel.ElementType.WATER - 1] = UnityUtil.GetChildNode(cBaseObjAnim, "BaseWater");
            m_BaseObj[(int)MasterDataDefineLabel.ElementType.WIND - 1] = UnityUtil.GetChildNode(cBaseObjAnim, "BaseWind");
            m_BaseObj[(int)MasterDataDefineLabel.ElementType.LIGHT - 1] = UnityUtil.GetChildNode(cBaseObjAnim, "BaseLight");
            m_BaseObj[(int)MasterDataDefineLabel.ElementType.DARK - 1] = UnityUtil.GetChildNode(cBaseObjAnim, "BaseDark");
            m_BaseObj[(int)MasterDataDefineLabel.ElementType.NAUGHT - 1] = UnityUtil.GetChildNode(cBaseObjAnim, "BaseNaught");

            // アニメーション
            m_BaseAnim = cBaseObjAnim.GetComponent<AnimationClipShot>();
        }
        #endregion

        #region ==== ユニットオブジェクト ====
        if (cUnitObjAnim != null)
        {
            // NONE定義分、回復属性分「-2」する(キャラ属性に、回復はないため)
            m_UnitObj = new GameObject[(int)MasterDataDefineLabel.ElementType.MAX - 2];

            // ユニットオブジェクトを取得
            m_UnitObj[(int)MasterDataDefineLabel.ElementType.FIRE - 1] = UnityUtil.GetChildNode(cUnitObjAnim, "UnitFire");
            m_UnitObj[(int)MasterDataDefineLabel.ElementType.WATER - 1] = UnityUtil.GetChildNode(cUnitObjAnim, "UnitWater");
            m_UnitObj[(int)MasterDataDefineLabel.ElementType.WIND - 1] = UnityUtil.GetChildNode(cUnitObjAnim, "UnitWind");
            m_UnitObj[(int)MasterDataDefineLabel.ElementType.LIGHT - 1] = UnityUtil.GetChildNode(cUnitObjAnim, "UnitLight");
            m_UnitObj[(int)MasterDataDefineLabel.ElementType.DARK - 1] = UnityUtil.GetChildNode(cUnitObjAnim, "UnitDark");
            m_UnitObj[(int)MasterDataDefineLabel.ElementType.NAUGHT - 1] = UnityUtil.GetChildNode(cUnitObjAnim, "UnitNaught");

            // アニメーション
            m_UnitAnim = cUnitObjAnim.GetComponent<Animation>();

            // ユニット情報を取得
            m_Master = MasterDataUtil.GetCharaParamFromID(unUnitID);
            //			m_Resource = AssetBundleUtil.RequestCharaTexture( unUnitID );
        }
        #endregion


        // 非表示設定
        for (int num = 0; num < (int)MasterDataDefineLabel.ElementType.MAX - 2; ++num)
        {
            UnityUtil.SetObjectEnabled(m_BaseObj[num], false);
            UnityUtil.SetObjectEnabled(m_UnitObj[num], false);
        }

        UnityUtil.SetObjectEnabledOnce(m_CutinObj, false);

        m_Texture = null;
        AssetBundler.Create().
            SetAsUnitTexture(
                unUnitID,
                (o) =>
                {
                    m_Texture = o.GetTexture2D(TextureWrapMode.Clamp);
                }).
            Load();

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セットアップ：カットイン
		@note	
	*/
    //----------------------------------------------------------------------------
    public bool SetupCutin(bool bCharaMaterial = false)
    {
        //----------------------------------------
        // エラーチェック
        //----------------------------------------
        if (m_CutinObj == null
            || m_Master == null
            || m_Texture == null)
        {
            return false;
        }

        //----------------------------------------
        // AssetBundle反映
        //----------------------------------------
        GameObject cUnitMesh = UnityUtil.GetChildNode(m_CutinObj, "CutinUnit");
        if (cUnitMesh != null
        && cUnitMesh.GetComponent<Renderer>().material != null)
        {
            Vector2 cAdjustOffset;
            Vector2 cAdjustTiling;

            if (bCharaMaterial == false)
            {
                // ベースオフセット(左のカットイン用：合成ではフレンドなど)
                cAdjustOffset.x = m_Master.img_cutin_offsetX * 0.001f;
                cAdjustOffset.y = m_Master.img_cutin_offsetY * 0.001f;
                cAdjustTiling.x = m_Master.img_cutin_tiling * 0.001f;
                cAdjustTiling.y = cAdjustTiling.x;
            }
            else
            {
                // リンクオフセット(右のカットイン用：合成では素材枠)
                cAdjustOffset.x = m_Master.img_cutin_link_offsetX * 0.001f;
                cAdjustOffset.y = m_Master.img_cutin_link_offsetY * 0.001f;
                cAdjustTiling.x = m_Master.img_cutin_link_tiling * 0.001f;
                cAdjustTiling.y = cAdjustTiling.x;
            }

            cUnitMesh.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", cAdjustOffset);
            cUnitMesh.GetComponent<Renderer>().material.SetTextureScale("_MainTex", cAdjustTiling);
            cUnitMesh.GetComponent<Renderer>().material.SetTexture("_MainTex", m_Texture);
        }
        //----------------------------------------
        // カットイン背景を表示
        // NONE定義分がないので「-1」をする
        //----------------------------------------
        int nElem = (int)m_Master.element - 1;
        UnityUtil.SetObjectEnabled(m_BaseObj[nElem], true);
        UnityUtil.SetObjectEnabled(m_UnitObj[nElem], true);

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション：スタート：全体
		@note	
	*/
    //----------------------------------------------------------------------------
    public bool StartAnimAll(string strAnimName)
    {
        //----------------------------------------
        // エラーチェック
        //----------------------------------------
        if (m_BaseAnim == null
        || m_UnitAnim == null)
        {
            return false;
        }

        UnityUtil.SetObjectEnabledOnce(m_CutinObj, true);
        m_BaseAnim.PlayAnimation(AnimationClipShot.SHOT_ANIM.PLAY);
        m_UnitAnim.GetClip(strAnimName).wrapMode = WrapMode.Once;
        m_UnitAnim.Play(strAnimName);

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション：スタート：ユニット
		@note	
	*/
    //----------------------------------------------------------------------------
    public bool StartAnimUnit(string strAnimName)
    {
        //----------------------------------------
        // エラーチェック
        //----------------------------------------
        if (m_UnitAnim == null)
        {
            return false;
        }

        m_UnitAnim.GetClip(strAnimName).wrapMode = WrapMode.Once;
        m_UnitAnim.Play(strAnimName);

        return true;
    }

}
///////////////////////////////////////EOF///////////////////////////////////////
