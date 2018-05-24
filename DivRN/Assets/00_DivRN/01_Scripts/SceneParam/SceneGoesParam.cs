﻿/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	SceneGoesParam.cs
    @brief	シーン間のパラメータ受け渡しクラス
    @author Developer
    @date 	2013/02/08
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

//----------------------------------------------------------------------------
/*!
    @brief	シーン間のパラメータ受け渡しクラス
*/
//----------------------------------------------------------------------------
public class SceneGoesParam : SingletonComponent<SceneGoesParam>
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public SceneGoesParamToMainMenu m_SceneGoesParamToMainMenu = null; //!< 各シーン受け渡しパラメータ：ゲームメイン → メインメニュー

    public SceneGoesParamToMainMenuRetire m_SceneGoesParamToMainMenuRetire = null; //!< 各シーン受け渡しパラメータ：ゲームメイン → メインメニュー

    public SceneGoesParamToQuest2 m_SceneGoesParamToQuest2 = null; //!< 各シーン受け渡しパラメータ：メインメニュー → 新探索
    public SceneGoesParamToQuest2Restore m_SceneGoesParamToQuest2Restore = null; //!< 各シーン受け渡しパラメータ：メインメニュー → ゲームメイン（中断復帰）
    public SceneGoesParamToQuest2Build m_SceneGoesParamToQuest2Build = null; //!< 各シーン受け渡しパラメータ：メインメニュー → ゲームメイン（クエスト構成情報）

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
}