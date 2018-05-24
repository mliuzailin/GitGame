using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@class		AnimationArray
	@brief		アニメーションをスクリプトから配列でアクセスできるようにしたかった
*/
//----------------------------------------------------------------------------
public class AnimationArray : MonoBehaviour
{

    private Animation m_Animation = null;           //!< Animation Component
    public AnimationClip[] m_Clips = null;          //!< Animation Clips

    private float m_Speed = GlobalDefine.GAME_SPEED_UP_OFF; //!< 速度

    //------------------------------------------------------------------------
    /*!
		@brief		起動時再生
	*/
    //------------------------------------------------------------------------
    void Awake()
    {
        m_Animation = gameObject.GetComponent<Animation>();
        if (m_Animation == null)
        {
            m_Animation = gameObject.AddComponent<Animation>();
        }

        // 事前登録しないと無理ぽい
        for (int i = 0; i < m_Clips.Length; i++)
        {
            m_Animation.AddClip(m_Clips[i], m_Clips[i].name);
        }
    }

    //------------------------------------------------------------------------
    /*!
		@brief		再生
		@param[in]	uint		(idx)		クリップインデックス
		@param[in]	bool		(force)		強制再生
	*/
    //------------------------------------------------------------------------
    private void Play(uint idx, bool force)
    {
        if (m_Animation == null)
        {
            return;
        }

        if (idx >= m_Clips.Length)
        {
            return;
        }

        if (force == false && m_Animation.isPlaying == true)
        {
            return;
        }
        else if (force == true && m_Animation.isPlaying == true)
        {
            m_Animation.Stop();
        }

        //速度設定
        m_Animation[m_Clips[idx].name].speed = m_Speed;

        //		m_Animation.clip = m_Clips[ idx ];
        m_Animation.Play(m_Clips[idx].name);
    }

    //------------------------------------------------------------------------
    /*!
		@brief		再生
		@param[in]	uint		(idx)		クリップインデックス
	*/
    //------------------------------------------------------------------------
    public void Play(uint idx)
    {
        m_Speed = GlobalDefine.GAME_SPEED_UP_OFF;
        Play(idx, false);
    }

    //------------------------------------------------------------------------
    /*!
		@brief		強制再生
		@param[in]	uint		(idx)		クリップインデックス
	*/
    //------------------------------------------------------------------------
    public void PlayForce(uint idx)
    {
        m_Speed = GlobalDefine.GAME_SPEED_UP_OFF;
        Play(idx, true);
    }

    //------------------------------------------------------------------------
    /*!
		@brief		強制再生＆速度指定
		@param[in]	uint		(idx)		クリップインデックス
		@param[in]	float		(fSpeed)	アニメーション速度
	*/
    //------------------------------------------------------------------------
    public void PlayForce(uint idx, float fSpeed)
    {
        m_Speed = fSpeed;
        Play(idx, true);
    }

    //------------------------------------------------------------------------
    /*!
		@brief		停止
	*/
    //------------------------------------------------------------------------
    private void Stop()
    {
        m_Animation.Stop();
    }

} // class AnimationArray
