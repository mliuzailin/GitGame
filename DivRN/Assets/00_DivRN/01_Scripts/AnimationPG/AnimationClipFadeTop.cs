/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	AnimationClipFadeTop.cs
	@brief	アニメーションクリップ記憶クラス：フェード特化管理側
	@author Developer
	@date 	2012/11/27
	
	[ AnimationClipFade ]のコンポーネントが付いたオブジェクトとペアで機能。
	自分の接続されているオブジェクト以下の、 [ AnimationClipFade ] の付いたオブジェクトを選定し、
	一括のフェード関連のアニメーション再生指示出し,完遂監視を行う
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
	@brief	アニメーションクリップ記憶クラス：フェード特化管理側
*/
//----------------------------------------------------------------------------
public class AnimationClipFadeTop : MonoBehaviour
{
    const float DEF_SAFETY_TIMELIMIT = 120.0f;      //!< セーフティのためのアニメーション限界時間

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーションクリップ記憶クラス：フェード特化管理側
	*/
    //----------------------------------------------------------------------------
    public class AnimationClipObj
    {
        public GameObject m_AnimationObject = null;
        public AnimationClipFade m_AnimationClip = null;
    }


    const int DEF_CHILD_OBJ_MAX = 30;           //!< 子供オブジェクト最大想定数

    public const int ANIM_FADE_NONE = 0;            //!< アニメーション通し番号：
    public const int ANIM_FADE_READY = 1;           //!< アニメーション通し番号：準備完了
    public const int ANIM_FADE_IN_REQ = 2;          //!< アニメーション通し番号：フェードイン
    public const int ANIM_FADE_IN = 3;          //!< アニメーション通し番号：フェードイン
    public const int ANIM_FADE_OUT = 4;         //!< アニメーション通し番号：フェードアウト
    public const int ANIM_FADE_MAX = 5;         //!< アニメーション通し番号：

    /*==========================================================================*/
    /*		var	and accessor													*/
    /*==========================================================================*/
    bool m_SetupAnimationClipFade = false;
    int m_SetupPrevRequest = ANIM_FADE_NONE;

    TemplateList<AnimationClipObj> m_ChildObject = new TemplateList<AnimationClipObj>();

    public int m_AnimationSeq = ANIM_FADE_NONE;
    public float m_AnimationDelta = 0.0f;


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected virtual void Start()
    {
        //--------------------------------
        // コンポーネントの付いたオブジェクトをリスト化
        //--------------------------------
        if (m_SetupAnimationClipFade == false)
        {
            SetupAnimationClipFade();
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション制御準備
	*/
    //----------------------------------------------------------------------------
    public void SetupAnimationClipFade()
    {
        //--------------------------------
        // コンポーネントの付いたオブジェクトをリスト化
        // AnimationClipFadeを取得しておく
        //--------------------------------
        m_ChildObject.Release();
        m_ChildObject.Alloc(32);
        SearchAnimationObj(gameObject, true, ref m_ChildObject);

        m_AnimationSeq = ANIM_FADE_READY;
        m_SetupAnimationClipFade = true;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	コンポーネントを保持するオブジェクトの探索
	*/
    //----------------------------------------------------------------------------
    static public void SearchAnimationObj(GameObject cRootObj, bool bRoot, ref TemplateList<AnimationClipObj> cSearchObjList)
    {
        //------------------------------
        // 
        //------------------------------
        if (cRootObj == null)
        {
            return;
        }

        //------------------------------
        // コンポーネント取得
        //------------------------------
        AnimationClipFade cAnimationClipFade = cRootObj.GetComponent<AnimationClipFade>();

        //------------------------------
        // ルート限定チェック
        //------------------------------
        if (cAnimationClipFade != null
        && cAnimationClipFade.m_GroupTopFix == true
        )
        {
            //------------------------------
            // AnimationClipFadeTopと同階層でのみ登録を認める。
            // ルートでない場合には以下の階層を辿らせないようにリターン
            //------------------------------
            if (bRoot == false)
            {
                return;
            }
        }

        //------------------------------
        // コンポーネントを記憶
        //------------------------------
        if (cAnimationClipFade != null)
        {
            AnimationClipObj cAnimationClipObj = new AnimationClipObj();
            cAnimationClipObj.m_AnimationObject = cRootObj;
            cAnimationClipObj.m_AnimationClip = cAnimationClipFade;

            cSearchObjList.Add(cAnimationClipObj);
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
            {
                continue;
            }

            SearchAnimationObj(cChildObj, false, ref cSearchObjList);
        }

        return;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	フェードアニメーション処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public bool AnimationUpdate()
    {
        m_AnimationDelta += Time.deltaTime;

        //--------------------------------
        // 初期化前にリクエストが発生してるなら再送
        //--------------------------------
        if (m_SetupAnimationClipFade == true)
        {
            if (m_SetupPrevRequest == ANIM_FADE_IN)
            {
                AnimationTriger(true, false);
                m_SetupPrevRequest = ANIM_FADE_NONE;
            }
            else if (m_SetupPrevRequest == ANIM_FADE_OUT)
            {
                AnimationTriger(false, false);
                m_SetupPrevRequest = ANIM_FADE_NONE;
            }
        }


        //--------------------------------
        // フェードアウトリクエストが出てるなら
        // 自分でアニメーション進行を見て勝手に止まる
        //--------------------------------
        if (m_AnimationSeq == ANIM_FADE_OUT)
        {
            if (AnimationFinishCheck() == true)
            {
                UnityUtil.SetObjectEnabledOnce(gameObject, false);

                m_AnimationSeq = ANIM_FADE_READY;
            }
            return false;
        }
        //--------------------------------
        // フェードインリクエストが出てるなら
        // アニメーションの完遂を待つ
        //--------------------------------
        if (m_AnimationSeq == ANIM_FADE_IN)
        {
            if (AnimationFinishCheck() == true)
            {
                m_AnimationSeq = ANIM_FADE_READY;
            }

            return false;
        }

        //--------------------------------
        // 全てのオブジェクトの待機アニメーションを再生指示
        //--------------------------------
        for (int i = 0; i < m_ChildObject.m_BufferSize; i++)
        {
            if (m_ChildObject[i].m_AnimationClip == null
            || m_ChildObject[i].m_AnimationClip.ChkAnimationPlaying() == true
            )
            {
                continue;
            }

            m_ChildObject[i].m_AnimationClip.PlayFadeAnimation(AnimationClipFade.FADE_ANIM.FADE_WAIT);
        }

        //--------------------------------
        // ランダムでランダム待機アニメーションを適用
        //--------------------------------
        int nRandWaitAccess = (int)RandManager.GetRand(0, 1000);
        if (m_ChildObject.m_BufferSize > nRandWaitAccess
        && m_ChildObject[nRandWaitAccess] != null
        )
        {
            m_ChildObject[nRandWaitAccess].m_AnimationClip.PlayFadeAnimation(AnimationClipFade.FADE_ANIM.FADE_WAIT_RAND);
        }

        //----------------------------------------
        // 更新OK
        //----------------------------------------
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	フェードアニメーション完遂チェック
	*/
    //----------------------------------------------------------------------------
    public bool AnimationFinishCheck()
    {
        if (m_SetupPrevRequest != ANIM_FADE_NONE)
        {
            return false;
        }

        for (int i = 0; i < m_ChildObject.m_BufferSize; i++)
        {
            if (m_ChildObject[i] == null
            || m_ChildObject[i].m_AnimationClip == null
            || m_ChildObject[i].m_AnimationClip.ChkAnimationPlaying() == false
            ) continue;

            if (UnityUtil.ChkObjectEnabled(m_ChildObject[i].m_AnimationObject) == false)
            {
                continue;
            }

            //----------------------------------------
            // セーフティのため、
            // 適当な時間経過したオブジェクトは
            // フェード待ちに考慮しないようにする
            //----------------------------------------
            if (m_AnimationDelta > DEF_SAFETY_TIMELIMIT)
            {
                Debug.LogError("Animation Safety - " + m_ChildObject[i].m_AnimationObject.name);
                continue;
            }

            return false;
        }
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	フェードアニメーション完遂チェック(ループ無視)
	*/
    //----------------------------------------------------------------------------
    public bool AnimationFinishCheckNeglectLoop()
    {
        if (m_SetupPrevRequest != ANIM_FADE_NONE)
        {
            return false;
        }

        for (int i = 0; i < m_ChildObject.m_BufferSize; i++)
        {
            if (m_ChildObject[i] == null
            || m_ChildObject[i].m_AnimationClip == null
            || m_ChildObject[i].m_AnimationClip.ChkAnimationPlaying() == false
            ) continue;

            // アニメーションが動いている
            // 動いているのがループアニメなら無視する
            if (m_ChildObject[i].m_AnimationClip.ChkAnimationPlaying((int)AnimationClipFade.FADE_ANIM.FADE_WAIT))
            {
                continue;
            }

            // ループオブジェクトをパス
            if (UnityUtil.ChkObjectEnabled(m_ChildObject[i].m_AnimationObject) == false)
            {
                continue;
            }

            //----------------------------------------
            // セーフティのため、
            // 適当な時間経過したオブジェクトは
            // フェード待ちに考慮しないようにする
            //----------------------------------------
            if (m_AnimationDelta > DEF_SAFETY_TIMELIMIT)
            {
                Debug.LogError("Animation Safety - " + m_ChildObject[i].m_AnimationObject.name);
                continue;
            }

            return false;
        }

        // 全アニメ終了
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え完遂チェック
	*/
    //----------------------------------------------------------------------------
    public bool FadeFinishCheck()
    {
        if (m_AnimationSeq != ANIM_FADE_READY)
        {
            return false;
        }

        if (AnimationFinishCheck() == false)
        {
            return false;
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え完遂チェック
	*/
    //----------------------------------------------------------------------------
    public bool FadeFinishCheckNeglectLoop()
    {
        if (m_AnimationSeq != ANIM_FADE_READY)
        {
            return false;
        }

        if (AnimationFinishCheckNeglectLoop() == false)
        {
            return false;
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替えメッセージ処理
		@note	ページ切り替えの際にSendMessageで呼び出される関数。
	*/
    //----------------------------------------------------------------------------
    public void AnimationObjectActive()
    {
        //--------------------------------
        // オブジェクト以下を強制的に有効化
        //--------------------------------
        UnityUtil.SetObjectEnabledOnce(gameObject, true);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替えメッセージ処理
		@note	ページ切り替えの際にSendMessageで呼び出される関数。
	*/
    //----------------------------------------------------------------------------
    public void AnimationTriger(bool bFadeIn, bool bFast)
    {
        //--------------------------------
        // 初期化前は戻る
        //--------------------------------
        if (m_AnimationSeq == ANIM_FADE_NONE)
        {
            //--------------------------------
            // 初期化後に再送できるように保持しておく
            //--------------------------------
            if (bFadeIn == true)
            {
                m_SetupPrevRequest = ANIM_FADE_IN;
            }
            else
            {
                m_SetupPrevRequest = ANIM_FADE_OUT;
            }
            return;
        }

        if (bFadeIn == true)
        {
            m_AnimationSeq = ANIM_FADE_IN;
        }
        else
        {
            m_AnimationSeq = ANIM_FADE_OUT;
        }

        m_AnimationDelta = 0.0f;

        //--------------------------------
        // 
        //--------------------------------
        if (m_AnimationSeq == ANIM_FADE_IN)
        {
            //--------------------------------
            // フェードイン指示
            //--------------------------------
            //--------------------------------
            // 全てのオブジェクトにフェードインアニメーション再生指示
            //--------------------------------
            for (int i = 0; i < m_ChildObject.m_BufferSize; i++)
            {
                if (m_ChildObject[i] == null
                || m_ChildObject[i].m_AnimationClip == null
                ) continue;

                if (UnityUtil.ChkObjectEnabled(m_ChildObject[i].m_AnimationObject) == false)
                {
                    m_ChildObject[i].m_AnimationClip.StopAnimationClip();
                    continue;
                }

                //--------------------------------
                // オブジェクトの表示ONOFFをやっていると、
                // 稀にコンポーネントが正常に立ち上がらないことがある？
                // 
                // Animationコンポーネントの再生機能が働かなくなって、
                // フェードが終わらないことがあるため、強制的に有効化指示を出す
                //--------------------------------
                UnityUtil.SetObjectEnabledOnce(m_ChildObject[i].m_AnimationObject, true);

                //--------------------------------
                // フェード再生呼出し
                //--------------------------------
                bool bRet = m_ChildObject[i].m_AnimationClip.PlayFadeAnimation(AnimationClipFade.FADE_ANIM.FADE_IN);
                if (bRet == false)
                {
                    m_ChildObject[i].m_AnimationClip.StopAnimationClip();
                }
            }
        }
        else if (m_AnimationSeq == ANIM_FADE_OUT)
        {
            //--------------------------------
            // フェードアウト指示
            //--------------------------------
            if (UnityUtil.ChkObjectEnabled(gameObject) == true)
            {
                if (bFast)
                {
                    //--------------------------------
                    // 急ぎなら即座にオブジェクト無効化
                    //--------------------------------
                    UnityUtil.SetObjectEnabledFix(gameObject, false);

                    m_AnimationSeq = ANIM_FADE_READY;
                }
                else
                {
                    //--------------------------------
                    // 全てのオブジェクトにフェードアウトアニメーション再生指示
                    //--------------------------------
                    for (int i = 0; i < m_ChildObject.m_BufferSize; i++)
                    {
                        if (m_ChildObject[i].m_AnimationClip == null)
                            continue;

                        //--------------------------------
                        // フェード再生呼出し
                        //--------------------------------
                        bool bRet = m_ChildObject[i].m_AnimationClip.PlayFadeAnimation(AnimationClipFade.FADE_ANIM.FADE_OUT);
                        if (bRet == false)
                        {
                            m_ChildObject[i].m_AnimationClip.StopAnimationClip();
                        }
                    }
                }
            }
            else
            {
                m_AnimationSeq = ANIM_FADE_READY;
            }
        }
    }
}