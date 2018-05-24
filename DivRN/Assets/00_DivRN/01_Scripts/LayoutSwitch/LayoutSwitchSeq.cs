/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	LayoutSwitchSeq.cs
	@brief	レイアウト切り替えオブジェクトクラス
	@author Developer
	@date 	2012/11/27
	
	LayoutSwitchManagerによって操作されることに特化したクラス。
	レイアウト１つごとにGameObjectが対応しており、
	そのGameObject以下の階層に対してフェードイン,フェードアウトを発行して監視することに特化する。
	
	
	[ AnimationClipFade ] のスクリプトを使って動く。
	[ AnimationClipFade ] は「 フェードイン , フェードアウト , 待機 , ランダム待機 」のアニメーションを保持するだけのクラスになっており
	インスペクタ上でAnimationClipFadeコンポーネントを付けてアニメーションをアサインしている階層を探し出し、
	そこに対してアニメーションを発行、監視することに特化する。
	
	アニメーションの作り方的に、なるべくパーツごとにバラバラにアニメーションを入れたいらしいので、
	階層以下のアニメーションコンポーネントアサインオブジェクトを選出しておき、
	フェードの際にはそれらに同時タイミングでアニメーションを発行する。

	発行後は階層以下のオブジェクトの全アニメーションが完遂したことを確認してフェード完遂と見なす。
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
	@brief	タイトルメニューシーケンス基底クラス
*/
//----------------------------------------------------------------------------
public class LayoutSwitchSeq : MonoBehaviour
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public LayoutSwitchManager m_LayoutSwitchManager = null;

    public bool m_LayoutStartOK = false;
    public bool m_LayoutExecOK = false;

    public bool m_AnimationReady = false;
    protected AnimationClipFadeTop m_AnimationClipFadeTop = null;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected virtual void Awake()
    {
        UnityUtil.SetObjectEnabled(gameObject, true);
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected virtual void Start()
    {
        //--------------------------------
        // アニメーションフェード管理クラスを追加
        //--------------------------------
        m_AnimationClipFadeTop = gameObject.GetComponent<AnimationClipFadeTop>();
        if (m_AnimationClipFadeTop == null)
        {
            m_AnimationClipFadeTop = gameObject.AddComponent<AnimationClipFadeTop>();
        }

        //--------------------------------
        // 自分自身を無効化する
        //--------------------------------
        UnityUtil.SetObjectEnabledOnce(gameObject, false);

        //--------------------------------
        // スタート関数を通ったことを明示
        //--------------------------------
        m_LayoutStartOK = true;
        m_LayoutExecOK = false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    protected bool LayoutSwitchUpdate()
    {
        //--------------------------------
        // 準備完了待ち
        //--------------------------------
        if (m_AnimationReady == false)
        {
            if (m_AnimationClipFadeTop != null
            && m_AnimationClipFadeTop.m_AnimationSeq != AnimationClipFadeTop.ANIM_FADE_NONE
            )
            {
                m_AnimationReady = true;
            }
            else
            {
                return false;
            }
        }

        //----------------------------------------
        // 子供のフェード処理更新
        //----------------------------------------
        if (m_AnimationClipFadeTop != null)
        {
            bool bUpdateOK = m_AnimationClipFadeTop.AnimationUpdate();
            if (bUpdateOK == false)
            {
                return false;
            }
        }

        //----------------------------------------
        // 管理クラス側が更新を不許可
        //----------------------------------------
        if (m_LayoutSwitchManager != null
        && m_LayoutSwitchManager.ChkLayoutRequestNG() == true
        )
        {
            return false;
        }

        //----------------------------------------
        // 管理クラス側が更新を不許可
        //----------------------------------------
        if (m_LayoutExecOK == false)
        {
            return false;
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト切り替え完遂チェック
	*/
    //----------------------------------------------------------------------------
    public bool LayoutSwitchFinishCheck()
    {
        if (UnityUtil.ChkObjectEnabled(gameObject) == false)
        {
            return true;
        }

        if (m_AnimationClipFadeTop != null)
        {
            return m_AnimationClipFadeTop.FadeFinishCheck();
        }
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト切り替えメッセージ処理
	*/
    //----------------------------------------------------------------------------
    public void LayoutSwitchTriger(bool bActive, bool bFast)
    {
        //--------------------------------
        // フェードアウト処理で現在が既にフェードアウト状態ならスルー
        //--------------------------------
        if (bActive == false)
        {
            if (UnityUtil.ChkObjectEnabled(gameObject) == false)
            {
                return;
            }
        }

        //--------------------------------
        // 関与するオブジェクトを全て有効化
        //--------------------------------
        if (bActive)
        {
            m_AnimationClipFadeTop.AnimationObjectActive();
        }

        //--------------------------------
        // レイアウト再構築関数呼び出し
        //--------------------------------
        if (bActive)
        {
            LayoutSwitchSetting(true);
        }

        //--------------------------------
        // アニメーション再生指示
        //--------------------------------
        bool bFadeIn = (bActive == true) ? true : false;
        m_AnimationClipFadeTop.AnimationTriger(bFadeIn, bFast);
    }

#if true
    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト切り替えメッセージ処理
	*/
    //----------------------------------------------------------------------------
    public void LayoutSwitchTrigerDisable()
    {
        LayoutSwitchSetting(false);
    }
#endif

    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：LayoutSwitchSeq：ページ切り替えにより有効化された際に呼ばれる関数
		@note	ページのレイアウト再構築を兼ねる
	*/
    //----------------------------------------------------------------------------
    protected virtual void LayoutSwitchSetting(bool bActive)
    {

    }



    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：MainMenuSeq：ページ無効化直前に走るイベント
		@note	処理中を返す間中はページを次のページが移行せずに処理を続ける
		@retval	[ true:処理中 / false:処理完遂 ]
	*/
    //----------------------------------------------------------------------------
    public virtual bool LayoutSwitchEventDisableBefore()
    {
        //		Debug.LogError( "PageSwitch Event Disable Before - " + gameObject.name );
        return false;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：MainMenuSeq：ページ無効化直後に走るイベント
		@note	処理中を返す間中はページを次のページが移行せずに処理を続ける
		@retval	[ true:処理中 / false:処理完遂 ]
	*/
    //----------------------------------------------------------------------------
    public virtual bool LayoutSwitchEventDisableAfter()
    {
        //		Debug.LogError( "PageSwitch Event Disable After - " + gameObject.name );
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：MainMenuSeq：ページ有効化直前に走るイベント
		@note	処理中を返す間中はページを次のページが移行せずに処理を続ける
		@retval	[ true:処理中 / false:処理完遂 ]
	*/
    //----------------------------------------------------------------------------
    public virtual bool LayoutSwitchEventEnableBefore()
    {
        //		Debug.LogError( "PageSwitch Event Enable Before - " + gameObject.name );
        return false;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：MainMenuSeq：ページ有効化直後に走るイベント
		@note	処理中を返す間中はページを次のページが移行せずに処理を続ける
		@retval	[ true:処理中 / false:処理完遂 ]
	*/
    //----------------------------------------------------------------------------
    public virtual bool LayoutSwitchEventEnableAfter()
    {
        //		Debug.LogError( "PageSwitch Event Enable After - " + gameObject.name );
        return false;
    }
}
