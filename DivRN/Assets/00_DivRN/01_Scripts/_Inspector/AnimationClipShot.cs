﻿/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	AnimationClipShot.cs
	@brief	アニメーションクリップ記憶再生クラス：単発再生
	@author Developer
	@date 	2013/07/03
 
	任意のアニメーションを単発で指定して再生指示を出して終了を監視するだけに特化したクラス
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
	@brief	アニメーションクリップ記憶再生クラス：単発再生
*/
//----------------------------------------------------------------------------
public class AnimationClipShot : AnimationClipPlayer
{
    //----------------------------------------------------------------------------
    /*!
		@brief	ユニット取得アニメーションID
	*/
    //----------------------------------------------------------------------------
    public enum SHOT_ANIM
    {
        PLAY,           //!< アニメーションラベル：
        MAX,            //!< アニメーションラベル：
    };

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public AnimationClip m_ShotAnim;        //!< アニメーション実体：

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：AnimationClipPlayer：関連するアニメーションをAnimationクリップに追加する
		@note	Start関数で登録しようとすると、一度もアクティブ化されていないオブジェクトに再生処理を発行した際に破綻する。
				基底クラスのAwakeで呼び出すことで登録を保証する
	*/
    //----------------------------------------------------------------------------
    protected override void AnimationSetup()
    {
        //--------------------------------
        // 基底クラスの初期化実行
        //--------------------------------
        base.AnimationSetup();

        //--------------------------------
        // アニメーションコンポーネントに要素を追加登録
        //--------------------------------
        SetAnimationClipMax((int)SHOT_ANIM.MAX);
        SetAnimationClip((int)SHOT_ANIM.PLAY, m_ShotAnim);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーションクリップ再生
	*/
    //----------------------------------------------------------------------------
    public bool PlayAnimation(SHOT_ANIM eAnimID)
    {
        //--------------------------------
        // 不具合チェック
        //--------------------------------
        if (eAnimID < 0
        || eAnimID >= SHOT_ANIM.MAX
        ) return false;

        //--------------------------------
        // 再生指示
        //--------------------------------
        return PlayAnimationClip((int)eAnimID, true);
    }

}

