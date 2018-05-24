using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCaptionControl : MonoBehaviour
{

    public GameObject m_ActiveSkillPrefab;
    public GameObject m_PassiveSkillPrefab;
    public GameObject m_LeaderSkillPrefab;
    public GameObject m_LBSSkillPrefab;
    public GameObject m_BoostSkillPrefab;
    public GameObject m_LinkSkillPrefab;
    public GameObject m_LinkPassiveSkillPrefab;
    public GameObject m_PlayerPhasePrefab;
    public GameObject m_EnemyPhasePrefab;

    public Sprite m_AchieveBaseSprite = null;
    public Sprite[] m_AchieveOnSprites = new Sprite[(int)CharaParty.BattleAchive.AchieveType.MAX];
    public Sprite[] m_AchieveOffSprites = new Sprite[(int)CharaParty.BattleAchive.AchieveType.MAX];

    public enum CaptionType : int
    {
        NONE,
        ACTIVE_SKILL,
        PASSIVE_SKILL,
        LEADER_SKILL,
        LBS_SKILL,
        BOOST_SKILL,
        LINK_SKILL,
        LINK_PASSIVE,
        PLAYER_PHASE,
        ENEMY_PHASE,
        PLAYER_HOLD_CARD_PHASE,

        MAX
    }

    private CaptionType m_CurrentCaption = CaptionType.NONE;
    private CaptionType m_RequestCaption = CaptionType.NONE;
    private bool m_RequestCaptionAnim = false;

    private GameObject[] m_CaptionObjs = null;

    private GameObject m_BattleAchieveBase = null;
    private SpriteRenderer[] m_BattleAchieveSpriteRenderer = null;

    // Use this for initialization
    void Start()
    {
        m_CaptionObjs = new GameObject[(int)CaptionType.MAX];
        m_CaptionObjs[(int)CaptionType.ACTIVE_SKILL] = Instantiate(m_ActiveSkillPrefab);
        m_CaptionObjs[(int)CaptionType.PASSIVE_SKILL] = Instantiate(m_PassiveSkillPrefab);
        m_CaptionObjs[(int)CaptionType.LEADER_SKILL] = Instantiate(m_LeaderSkillPrefab);
        m_CaptionObjs[(int)CaptionType.LBS_SKILL] = Instantiate(m_LBSSkillPrefab);
        m_CaptionObjs[(int)CaptionType.BOOST_SKILL] = Instantiate(m_BoostSkillPrefab);
        m_CaptionObjs[(int)CaptionType.LINK_SKILL] = Instantiate(m_LinkSkillPrefab);
        m_CaptionObjs[(int)CaptionType.LINK_PASSIVE] = Instantiate(m_LinkPassiveSkillPrefab);
        m_CaptionObjs[(int)CaptionType.PLAYER_PHASE] = Instantiate(m_PlayerPhasePrefab);
        m_CaptionObjs[(int)CaptionType.ENEMY_PHASE] = Instantiate(m_EnemyPhasePrefab);

        for (int idx = 0; idx < m_CaptionObjs.Length; idx++)
        {
            GameObject obj = m_CaptionObjs[idx];
            if (obj != null)
            {
                obj.transform.SetParent(transform, false);
                obj.SetActive(false);
            }
        }

        // バトルアチーブ
        if (m_CaptionObjs[(int)CaptionType.PLAYER_PHASE] != null)
        {
            Transform achieve_base_trans = m_CaptionObjs[(int)CaptionType.PLAYER_PHASE].transform.Find("AchieveBase");
            if (achieve_base_trans != null)
            {
                m_BattleAchieveBase = achieve_base_trans.transform.gameObject;

                m_BattleAchieveSpriteRenderer = new SpriteRenderer[(int)CharaParty.BattleAchive.AchieveType.MAX];
                for (int idx = 0; idx < m_BattleAchieveSpriteRenderer.Length; idx++)
                {
                    Transform wrk_trans = achieve_base_trans.Find("Achieve" + (idx + 1).ToString());
                    if (wrk_trans != null)
                    {
                        m_BattleAchieveSpriteRenderer[idx] = wrk_trans.GetComponent<SpriteRenderer>();
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentCaption != m_RequestCaption)
        {
            GameObject current_caption = getCaptionObject(m_CurrentCaption);
            if (current_caption != null)
            {
                // 表示中のものがあるので消す.
                if (m_RequestCaptionAnim == false)
                {
                    Animation anim = current_caption.GetComponent<Animation>();
                    if (anim != null)
                    {
                        anim.Play(InGameDefine.ANIM_CAPTION_OUT);
                    }
                    m_RequestCaptionAnim = true;
                }
                else
                {
                    if (current_caption.GetComponent<Animation>().isPlaying == false)
                    {
                        UnityUtil.SetObjectEnabledOnce(current_caption, false);
                        m_CurrentCaption = CaptionType.NONE;
                        m_RequestCaptionAnim = false;
                    }
                }
            }
            else
            {
                // キャプション出現.
                GameObject request_caption = getCaptionObject(m_RequestCaption);
                if (request_caption != null)
                {
                    if (m_RequestCaptionAnim == false)
                    {
                        UnityUtil.SetObjectEnabledOnce(request_caption, true);
                        Animation anim = request_caption.GetComponent<Animation>();
                        if (anim != null)
                        {
                            anim.Play(InGameDefine.ANIM_CAPTION_IN);
                            m_RequestCaptionAnim = true;
                        }
                    }
                    else
                    {
                        UnityUtil.SetObjectEnabledOnce(request_caption, true);
                        Animation anim = request_caption.GetComponent<Animation>();
                        if (anim == null || anim.isPlaying == false)
                        {
                            m_CurrentCaption = m_RequestCaption;
                            m_RequestCaptionAnim = false;
                        }
                    }
                }
                else
                {
                    m_CurrentCaption = m_RequestCaption;
                }

                // テンポアップ対応 add by tomioka.
                if (m_RequestCaption == CaptionType.PLAYER_PHASE
                    || m_RequestCaption == CaptionType.PLAYER_HOLD_CARD_PHASE)
                {
                    m_CurrentCaption = m_RequestCaption;
                    m_RequestCaptionAnim = false;
                }
            }
        }
    }

    public void requestCaption(CaptionType caption_type)
    {
        if (m_RequestCaption != caption_type)
        {
            if (m_CurrentCaption != m_RequestCaption)
            {
                // 以前のリクエストが完了していなかったら、以前のリクエストを中止
                GameObject request_caption = getCaptionObject(m_RequestCaption);
                if (request_caption != null)
                {
                    if (m_RequestCaptionAnim)
                    {
                        m_RequestCaptionAnim = false;

                        Animation anim = request_caption.GetComponent<Animation>();
                        if (anim != null)
                        {
                            anim.Stop();
                        }
                    }

                    request_caption.SetActive(false);
                }
            }

            m_RequestCaption = caption_type;
        }
    }

    public CaptionType getRequestCaption()
    {
        return m_RequestCaption;
    }

    public CaptionType getCurrentCaption()
    {
        return m_CurrentCaption;
    }

    // 非表示にする.
    public void hideCaption()
    {
        requestCaption(CaptionType.NONE);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		キャプションを強制的に非表示にする
    */
    //----------------------------------------------------------------------------
    public void outForce()
    {
        // 例外処理:PlayerPhaseCaption非表示化
        GameObject caption = getCaptionObject(m_CurrentCaption);

        if (caption != null)
        {
            caption.GetComponent<Animation>().Stop();
            caption.GetComponent<Animation>().Play(InGameDefine.ANIM_CAPTION_OUT);
        }

        m_CurrentCaption = CaptionType.NONE;
        m_RequestCaption = CaptionType.NONE;
        m_RequestCaptionAnim = false;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		キャプションの表示チェック
    */
    //----------------------------------------------------------------------------
    public bool isDisplayCaption()
    {
        if (m_CurrentCaption != 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 表示更新中か
    /// </summary>
    /// <returns></returns>
    public bool isUpdating()
    {
        return m_RequestCaptionAnim;
    }


    private GameObject getCaptionObject(CaptionType caption_type)
    {
        return m_CaptionObjs[(int)caption_type];
    }


    public void setShowAchieve(bool is_show)
    {
        if (m_BattleAchieveBase != null)
        {
            m_BattleAchieveBase.SetActive(is_show);
        }
    }

    public void updateAchieve()
    {
        if (m_BattleAchieveSpriteRenderer != null)
        {
            for (int idx = 0; idx < (int)CharaParty.BattleAchive.AchieveType.MAX; idx++)
            {
                if (m_BattleAchieveSpriteRenderer[(int)idx] != null)
                {
                    if (BattleParam.m_PlayerParty.m_BattleAchive.isAchieved((CharaParty.BattleAchive.AchieveType)idx))
                    {
                        // 達成したら暗くする
                        m_BattleAchieveSpriteRenderer[idx].sprite = m_AchieveOffSprites[idx];
                    }
                    else
                    {
                        // 達成していないときは明るく表示
                        m_BattleAchieveSpriteRenderer[idx].sprite = m_AchieveOnSprites[idx];
                    }
                }
            }
        }
    }
}
