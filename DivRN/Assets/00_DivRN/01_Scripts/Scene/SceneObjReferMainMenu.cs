/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	SceneObjReferMainMenu.cs
	@brief	非アクティブオブジェクト一元化クラス
	@author Developer
	@date 	2012/10/04
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
	@brief	非アクティブオブジェクト一元化クラス
*/
//----------------------------------------------------------------------------
public class SceneObjReferMainMenu : SingletonComponent<SceneObjReferMainMenu>
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    [Header("参照オブジェクト")]
    public GameObject m_MainMenuRoot;                           //!< 参照オブジェクト：ルートオブジェクト
    public GameObject m_MainMenuGroupCamera;                    //!< 参照オブジェクト：NGUI階層：Camera
    public GameObject m_MainMenuTopAnchor;                  //!< 参照オブジェクト：TopAnchor
    public GameObject m_MainMenuMiddleAnchor;                   //!< 参照オブジェクト：MiddleAnchor
    public GameObject m_MainMenuBottomAnchor;                   //!< 参照オブジェクト：BottomAnchor

    public GameObject m_MainMenuHeader;                     //!< 参照オブジェクト：固定レイヤー：プレイヤーステータス
    public GameObject m_MainMenuSubTab;                     //!< 参照オブジェクト：固定レイヤー：タブリスト
    public GameObject m_MainMenuFooter;                     //!< 参照オブジェクト：固定レイヤー：下部リンク
    public GameObject m_MainMenuSeqObj;                     //!< 
    public GameObject m_MainMenuBackground;                 //!< 

    public GameObject m_TopDecoNoMenu;                      //!< 
    public GameObject m_TopDeco;								//!< 

    [Header("参照マテリアル")]
    public Material m_MaterialChara;                    //!< 参照マテリアル：キャラクター

    public Material m_MaterialEnemyShadowFire;          //!< 参照マテリアル：エネミー影：火
    public Material m_MaterialEnemyShadowWater;         //!< 参照マテリアル：エネミー影：水
    public Material m_MaterialEnemyShadowWind;          //!< 参照マテリアル：エネミー影：風
    public Material m_MaterialEnemyShadowDark;          //!< 参照マテリアル：エネミー影：闇
    public Material m_MaterialEnemyShadowLight;         //!< 参照マテリアル：エネミー影：光
    public Material m_MaterialEnemyShadowNaught;        //!< 参照マテリアル：エネミー影：無


    [Header("メインメニューアトラス")]
    public UIAtlas m_MainMenuAtlasArea;             //!< エリアアイコンアトラス：

    public UIAtlas m_MainMenuAtlasGacha;                //!< ガチャアトラス：

    private UIAtlas mainMenuAtlas = null;               //!< メインメニューアトラス：
    public UIAtlas m_MainMenuAtlas
    {
        get
        {
            if (mainMenuAtlas == null)
            {
                mainMenuAtlas = new UIAtlas();
                mainMenuAtlas.spriteList = ResourceManager.Instance.LoadAll(ResourceType.Menu).ToArray();
            }
            return mainMenuAtlas;
        }
    }
    //v300新UI対応用 end

    [Header("参照オブジェクト")]

    public GameObject m_OriginObjEvolCutinFriend;           //!< 参照オブジェクト：進化演出カットイン：フレンド
    public GameObject m_OriginObjEvolCutinParts;            //!< 参照オブジェクト：進化演出カットイン：素材

    public GameObject m_EffectEvolCutinFriend;          //!< 参照オブジェクト：進化演出：カットインエフェクト（フレンド用）
    public GameObject m_EffectEvolCutinParts;               //!< 参照オブジェクト：進化演出：カットインエフェクト（素材用）
    public GameObject m_EffectEvolFix;                  //!< 参照オブジェクト：進化演出：進化成立エフェクト
    public GameObject m_EffectLevelUp;                  //!< 参照オブジェクト：強化演出：レベルアップエフェクト

    public GameObject m_EffectSkillLevelUp;             //!< 参照オブジェクト：強化演出：スキルレベルアップエフェクト
    public GameObject m_EffectLinkOn;                       //!< 参照オブジェクト：リンク演出：リンクONエフェクト
    public GameObject m_EffectRankUp;						//!< 参照オブジェクト：ランクアップ演出：ランクアップエフェクト


    [Header("参照オブジェクトv5.0")]
    public GameObject m_UnitNamePanel;                    //!< 
    public GameObject m_UnitParamPanel;                   //!<
    public GameObject m_UnitStoryPanel;                   //!< 
    public GameObject m_UnitSkillPanel;                   //!< 
    public GameObject m_UnitLinkPanel;                    //!< 
    public GameObject m_UnitMaterialPanel;                //!< 
    public GameObject m_UnitEvolveListPanel;              //!< 
    public GameObject m_UnitDetailPanel;					//!<


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
}

