/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	AnimationClipScratch.cs
	@brief	アニメーションクリップ記憶再生クラス：スクラッチパネル特化
	@author Developer
	@date 	2013/03/26
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
	@brief	アニメーションクリップ記憶再生クラス：スクラッチパネル特化
*/
//----------------------------------------------------------------------------
public class AnimationClipScratch : AnimationClipPlayer
{

    //----------------------------------------------------------------------------
    /*!
		@brief	スクラッチアニメーションID
	*/
    //----------------------------------------------------------------------------
    public enum SCRATCH_ANIM
    {
        SELECT_TO_PIECE_1,      //!< アニメーションラベル：任意選択：選択した際の動作：
        SELECT_TO_PIECE_2,      //!< アニメーションラベル：任意選択：選択した際の動作：
        SELECT_TO_PIECE_3,      //!< アニメーションラベル：任意選択：選択した際の動作：
        SELECT_TO_PIECE_4,      //!< アニメーションラベル：任意選択：選択した際の動作：
        SELECT_TO_PIECE_5,      //!< アニメーションラベル：任意選択：選択した際の動作：
        SELECT_TO_PIECE_6,      //!< アニメーションラベル：任意選択：選択した際の動作：
        SELECT_TO_CHARA_1,      //!< アニメーションラベル：任意選択：駒がキャラに成る時の動作：
        SELECT_TO_CHARA_2,      //!< アニメーションラベル：任意選択：駒がキャラに成る時の動作：
        SELECT_TO_CHARA_3,      //!< アニメーションラベル：任意選択：駒がキャラに成る時の動作：
        SELECT_TO_CHARA_4,      //!< アニメーションラベル：任意選択：駒がキャラに成る時の動作：
        SELECT_TO_CHARA_5,      //!< アニメーションラベル：任意選択：駒がキャラに成る時の動作：
        SELECT_TO_CHARA_6,      //!< アニメーションラベル：任意選択：駒がキャラに成る時の動作：
        ANSWER_TO_PIECE_1,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_PIECE_2,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_PIECE_3,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_PIECE_4,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_PIECE_5,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_PIECE_6,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_CHARA_1,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_CHARA_2,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_CHARA_3,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_CHARA_4,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_CHARA_5,      //!< アニメーションラベル：答え合わせ：
        ANSWER_TO_CHARA_6,      //!< アニメーションラベル：答え合わせ：
        MAX,                    //!< アニメーションラベル：
    };

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public AnimationClip m_ScratchSelectToPiece_1;          //!< アニメーション実体：任意選択：選択した際の動作：
    public AnimationClip m_ScratchSelectToPiece_2;          //!< アニメーション実体：任意選択：選択した際の動作：
    public AnimationClip m_ScratchSelectToPiece_3;          //!< アニメーション実体：任意選択：選択した際の動作：
    public AnimationClip m_ScratchSelectToPiece_4;          //!< アニメーション実体：任意選択：選択した際の動作：
    public AnimationClip m_ScratchSelectToPiece_5;          //!< アニメーション実体：任意選択：選択した際の動作：
    public AnimationClip m_ScratchSelectToPiece_6;          //!< アニメーション実体：任意選択：選択した際の動作：

    public AnimationClip m_ScratchSelectToChara_1;          //!< アニメーション実体：任意選択：駒がキャラに成る時の動作：
    public AnimationClip m_ScratchSelectToChara_2;          //!< アニメーション実体：任意選択：駒がキャラに成る時の動作：
    public AnimationClip m_ScratchSelectToChara_3;          //!< アニメーション実体：任意選択：駒がキャラに成る時の動作：
    public AnimationClip m_ScratchSelectToChara_4;          //!< アニメーション実体：任意選択：駒がキャラに成る時の動作：
    public AnimationClip m_ScratchSelectToChara_5;          //!< アニメーション実体：任意選択：駒がキャラに成る時の動作：
    public AnimationClip m_ScratchSelectToChara_6;          //!< アニメーション実体：任意選択：駒がキャラに成る時の動作：

    public AnimationClip m_ScratchAnswerToPiece_1;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToPiece_2;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToPiece_3;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToPiece_4;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToPiece_5;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToPiece_6;          //!< アニメーション実体：答え合わせ：

    public AnimationClip m_ScratchAnswerToChara_1;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToChara_2;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToChara_3;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToChara_4;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToChara_5;          //!< アニメーション実体：答え合わせ：
    public AnimationClip m_ScratchAnswerToChara_6;          //!< アニメーション実体：答え合わせ：

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
        SetAnimationClipMax((int)SCRATCH_ANIM.MAX);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_PIECE_1, m_ScratchSelectToPiece_1);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_PIECE_2, m_ScratchSelectToPiece_2);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_PIECE_3, m_ScratchSelectToPiece_3);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_PIECE_4, m_ScratchSelectToPiece_4);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_PIECE_5, m_ScratchSelectToPiece_5);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_PIECE_6, m_ScratchSelectToPiece_6);

        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_CHARA_1, m_ScratchSelectToChara_1);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_CHARA_2, m_ScratchSelectToChara_2);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_CHARA_3, m_ScratchSelectToChara_3);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_CHARA_4, m_ScratchSelectToChara_4);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_CHARA_5, m_ScratchSelectToChara_5);
        SetAnimationClip((int)SCRATCH_ANIM.SELECT_TO_CHARA_6, m_ScratchSelectToChara_6);

        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_PIECE_1, m_ScratchAnswerToPiece_1);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_PIECE_2, m_ScratchAnswerToPiece_2);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_PIECE_3, m_ScratchAnswerToPiece_3);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_PIECE_4, m_ScratchAnswerToPiece_4);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_PIECE_5, m_ScratchAnswerToPiece_5);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_PIECE_6, m_ScratchAnswerToPiece_6);

        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_CHARA_1, m_ScratchAnswerToChara_1);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_CHARA_2, m_ScratchAnswerToChara_2);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_CHARA_3, m_ScratchAnswerToChara_3);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_CHARA_4, m_ScratchAnswerToChara_4);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_CHARA_5, m_ScratchAnswerToChara_5);
        SetAnimationClip((int)SCRATCH_ANIM.ANSWER_TO_CHARA_6, m_ScratchAnswerToChara_6);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーションクリップ再生
	*/
    //----------------------------------------------------------------------------
    public bool PlayScratchAnimation(SCRATCH_ANIM eScratchAnim)
    {
        //--------------------------------
        // 不具合チェック
        //--------------------------------
        if (eScratchAnim < 0
        || eScratchAnim >= SCRATCH_ANIM.MAX
        ) return false;

        //--------------------------------
        // 再生指示
        //--------------------------------
        return PlayAnimationClip((int)eScratchAnim, true);
    }

}

