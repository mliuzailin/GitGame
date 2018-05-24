﻿/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	SceneMode.cs
	@brief	シーン基底クラス
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
using DG.Tweening;

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
	@brief	シーン基底クラス
*/
//----------------------------------------------------------------------------
public abstract class SceneMode<T> : Scene<T> where T : Component
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public Camera m_TouchCheckCamera3D;         //!< カメラ情報：タッチ判定用カメラ
    public Camera m_TouchCheckCamera2D;         //!< カメラ情報：タッチ判定用カメラ

    public Camera m_ShakeCamera3D;              //!< カメラ情報：振動カメラ
    public Camera m_ShakeCamera2D;              //!< カメラ情報：振動カメラ

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※インスタンス生成時呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        InputParam.m_Camera2D = m_TouchCheckCamera2D;
        InputParam.m_Camera3D = m_TouchCheckCamera3D;

    }


    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：インスタンス制御関連：インスタンス破棄時に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void OnDestroy()
    {
        base.OnDestroy();
        //
        DOTween.Clear();
        //----------------------------------------
        //
        //----------------------------------------
        InputParam.m_Camera2D = null;
        InputParam.m_Camera3D = null;
        InputParam.m_CameraDialog = null;
        InputParam.m_CameraTutorial = null;
    }
}
