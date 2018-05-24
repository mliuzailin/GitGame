
/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	SceneObjReferGameMain.cs
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
using UnityEngine.UI;
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
public class SceneObjReferGameMain : SingletonComponent<SceneObjReferGameMain>
{
    //------------------------------------------------------------------------
    // フィールドUI
    //------------------------------------------------------------------------
    public GameObject m_UIInstanceTargetWindowPosition; //!< 参照インスタンス：UI：

    //------------------------------------------------------------------------
    // エフェクト関連
    //------------------------------------------------------------------------
    private SceneObjReferGameMainEffect _m_EffectPrefab;

    public SceneObjReferGameMainEffect m_EffectPrefab
    {
        get
        {
            return _m_EffectPrefab;
        }
    }

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    override protected void Awake()
    {
        base.Awake();

        if (m_EffectPrefab != null)
        {
            m_EffectPrefab.init();
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
    */
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();
    }


    protected override void OnDestroy()
    {
        resetEffectAssignObj();

        base.OnDestroy();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="effect_assign">実体化されたゲームオブジェクトのコンポーネント渡す</param>
    public void setEffectAssignObj(SceneObjReferGameMainEffect effect_assign)
    {
        resetEffectAssignObj();

        if (effect_assign != null)
        {
            _m_EffectPrefab = effect_assign;
            _m_EffectPrefab.transform.SetParent(transform, false);

            m_EffectPrefab.init();
        }
    }

    private void resetEffectAssignObj()
    {
        if (_m_EffectPrefab != null)
        {
            _m_EffectPrefab.destroy();
            Destroy(_m_EffectPrefab.gameObject);
        }
        _m_EffectPrefab = null;
    }
}

