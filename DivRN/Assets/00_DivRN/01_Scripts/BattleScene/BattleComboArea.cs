using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleComboArea : MonoBehaviour
{
    public Sprite[] m_HitNumberSprites = new Sprite[10];
    public Sprite[] m_RateNumberSprites = new Sprite[10];

    public SpriteRenderer m_Hit100 = null;
    public SpriteRenderer m_Hit010 = null;
    public SpriteRenderer m_Hit001 = null;

    public SpriteRenderer m_Rate1000 = null;
    public SpriteRenderer m_Rate0100 = null;
    public SpriteRenderer m_Rate0010 = null;
    public SpriteRenderer m_Rate0001 = null;

    // Use this for initialization
    void Start()
    {

    }

    public void setEnable(bool is_enable)
    {
        gameObject.SetActive(is_enable);
    }

    public void setHands(int hands)
    {
        int ratex = (int)(InGameUtilBattle.GetSkillCountRate(hands) * 100.0f);
        if (ratex > 9999)
        {
            ratex = 9999;
        }

        m_Rate1000.sprite = m_RateNumberSprites[(ratex / 1000) % 10];
        m_Rate0100.sprite = m_RateNumberSprites[(ratex / 100) % 10];
        m_Rate0010.sprite = m_RateNumberSprites[(ratex / 10) % 10];
        m_Rate0001.sprite = m_RateNumberSprites[(ratex / 1) % 10];
    }

    public void setHitCount(int hit_count)
    {
        if (hit_count > 999)
        {
            hit_count = 999;
        }

        if (hit_count >= 100)
        {
            m_Hit100.sprite = m_HitNumberSprites[(hit_count / 100) % 10];
        }
        else
        {
            m_Hit100.sprite = null;
        }
        m_Hit010.sprite = m_HitNumberSprites[(hit_count / 10) % 10];
        m_Hit001.sprite = m_HitNumberSprites[hit_count % 10];
    }
}
