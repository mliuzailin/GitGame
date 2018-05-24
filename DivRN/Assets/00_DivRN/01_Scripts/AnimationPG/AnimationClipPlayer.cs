/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	AnimationClipPlayer.cs
	@brief	アニメーションクリップ再生クラス
	@note	このクラスを継承しての使用を想定する
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
	@brief	アニメーションクリップ再生クラス
	@note	このクラスを継承しての使用を想定する
*/
//----------------------------------------------------------------------------
public class AnimationClipPlayer : MonoBehaviour
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private Animation m_Animation = null;       //!< アニメーション再生コンポーネント
    private AnimationClip[] m_AnimationClips = null;        //!< アニメーションクリップリスト
    private float[] m_AnimationClipsPriority = null;        //!< アニメーションクリップリスト優先度
    private bool m_AnimationSetup = false;  //!< アニメーション初期化フラグ
    private int m_AnimationPlayNum = -1;        //!< アニメーション再生中番号
    private bool m_AnimationResetFlg = false;   //!< アニメーションをリセット


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※インスタンス生成時呼出し
	*/
    //----------------------------------------------------------------------------
    protected virtual void Awake()
    {

        //--------------------------------
        // 関連アニメーションの登録を実行。
        // 関数のオーバーライド想定。
        //--------------------------------
        AnimationSetup();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected virtual void Start()
    {
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    protected virtual void Update()
    {
        if (m_AnimationResetFlg == true)
        {
            m_Animation.Stop();
            m_AnimationResetFlg = false;
        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：AnimationClipPlayer：関連するアニメーションをAnimationクリップに追加する
	*/
    //----------------------------------------------------------------------------
    protected virtual void AnimationSetup()
    {
        //--------------------------------
        // 再生に使用するアニメーションコンポーネントの取得
        //--------------------------------
        m_Animation = gameObject.GetComponent<Animation>();
        if (m_Animation == null)
        {
            m_Animation = gameObject.AddComponent<Animation>();
        }

        //--------------------------------
        // 自動再生が働かないように止めておく
        //--------------------------------
        m_Animation.playAutomatically = false;
        m_Animation.Stop();

        m_AnimationSetup = true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：最初のフレームに戻す
	*/
    //----------------------------------------------------------------------------
    protected virtual void ResetAnimationClip(AnimationClip cAnimationClip)
    {
        if (m_Animation != null)
        {
            m_Animation.Play(cAnimationClip.name);
            m_AnimationResetFlg = true;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：最終フレームに設定する
	*/
    //----------------------------------------------------------------------------
    protected virtual void EndFrame(AnimationClip cAnimationClip)
    {
        if (m_Animation != null)
        {
            AnimationState cAnimationState = m_Animation[cAnimationClip.name];

            if (cAnimationState != null)
            {
                cAnimationState.time = cAnimationState.length;
                m_Animation.Play();
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーションクリップ登録最大数を設定
	*/
    //----------------------------------------------------------------------------
    protected void SetAnimationClipMax(int nAccessNum)
    {
        m_AnimationClips = null;
        m_AnimationClips = new AnimationClip[nAccessNum];
        m_AnimationClipsPriority = new float[nAccessNum];
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーションクリップ追加
	*/
    //----------------------------------------------------------------------------
    protected bool SetAnimationClip(int nAccessNum, AnimationClip cAnimationClip, float fPriority)
    {
        //--------------------------------
        // 不具合チェック
        //--------------------------------
        if (m_Animation == null)
        {
            Debug.LogError("Animation Component None!");
            return false;
        }
        if (m_AnimationClips == null
        || m_AnimationClips.Length < nAccessNum
        )
        {
            Debug.LogError("Animation Clip List Over!");
            return false;
        }

        //--------------------------------
        // 変化チェック
        //--------------------------------
        if (m_AnimationClips[nAccessNum] == cAnimationClip)
        {
            return false;
        }

        //--------------------------------
        // アニメーションコンポーネントを追加登録
        //--------------------------------
        m_AnimationClipsPriority[nAccessNum] = fPriority;
        m_AnimationClips[nAccessNum] = cAnimationClip;
        if (m_AnimationClips[nAccessNum] != null)
        {
            m_Animation.AddClip(m_AnimationClips[nAccessNum], m_AnimationClips[nAccessNum].name);
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーションクリップ追加
	*/
    //----------------------------------------------------------------------------
    protected bool SetAnimationClip(int nAccessNum, AnimationClip cAnimationClip)
    {
        return SetAnimationClip(nAccessNum, cAnimationClip, 0.0f);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーションクリップ再生
	*/
    //----------------------------------------------------------------------------
    protected bool PlayAnimationClip(int nAccessNum, bool bForce)
    {
        //--------------------------------
        // 初期化前なら初期化する
        //--------------------------------
        if (m_AnimationSetup == false)
        {
            AnimationSetup();
        }

        //--------------------------------
        // 不具合チェック
        //--------------------------------
        if (m_AnimationClips == null
        || m_AnimationClips.Length < nAccessNum
        || m_AnimationClips[nAccessNum] == null
        ) return false;

        //--------------------------------
        // 強制発行でない場合、
        // なんらかのアニメーションを再生中であれば失敗を返す
        //--------------------------------
        if (bForce == false)
        {
            //--------------------------------
            // プレイ中であり、プレイ中のアクションが優先度的に高いものならリクエストをスルー
            //--------------------------------
            if (m_Animation.isPlaying == true)
            {
                bool bRequestSkip = false;

                if (m_AnimationPlayNum >= 0
                && m_AnimationPlayNum < m_AnimationClipsPriority.Length
                )
                {
                    if (m_AnimationClipsPriority[m_AnimationPlayNum] > m_AnimationClipsPriority[nAccessNum])
                    {
                        bRequestSkip = true;
                    }
                }

                if (bRequestSkip == true)
                {
                    return false;
                }
            }
        }

        //--------------------------------
        // アニメーション再生発行
        // 
        // アニメーションコンポーネントがenabled立ってても再生しないことがある
        //--------------------------------
        m_Animation.enabled = false;
        m_Animation.enabled = true;
        m_Animation.Play(m_AnimationClips[nAccessNum].name);
        m_AnimationPlayNum = nAccessNum;

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーションクリップ再生停止
	*/
    //----------------------------------------------------------------------------
    public void StopAnimationClip()
    {
        if (m_Animation == null)
            return;

        m_Animation.Stop();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーション再生中チェック
	*/
    //----------------------------------------------------------------------------
    public bool ChkAnimationPlaying()
    {
        if (m_Animation == null)
        {
            return false;
        }

        if (m_Animation.isPlaying == true)
        {
#if false
			string strAnimationClipState = gameObject.name + "\n";
			for( int i = 0;i < m_AnimationClips.Length;i++ )
			{
				if( m_AnimationClips[ i ] == null )
					continue;
				AnimationState cAnimationState = m_Animation[ m_AnimationClips[ i ].name ];
				strAnimationClipState = strAnimationClipState + cAnimationState.name + " , " + cAnimationState.enabled + " , " + cAnimationState.time + "\n";
			}
			Debug.LogError( strAnimationClipState );
#endif
            return true;
        }
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーションループチェック
	*/
    //----------------------------------------------------------------------------
    public bool ChkAnimationPlaying(int nAccessNum)
    {
        if (m_Animation == null)
        {
            return false;
        }
        // 再生中なら番号から判断する
        if (m_Animation.isPlaying == true)
        {
            return (m_AnimationPlayNum == nAccessNum);
        }
        // 動いていないならそのまま
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション操作：アニメーション速度設定
	*/
    //----------------------------------------------------------------------------
    public bool SetAnimationSpeed(int nAccessNum, float fSpeed)
    {
        if (m_Animation == null)
        {
            return false;
        }
        //速度変更
        m_Animation[m_AnimationClips[nAccessNum].name].speed = fSpeed;
        return true;
    }
}

