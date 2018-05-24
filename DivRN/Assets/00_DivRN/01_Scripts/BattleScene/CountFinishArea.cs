using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountFinishArea : MonoBehaviour
{
    public Sprite[] m_NumberSprites = new Sprite[10];

    public SpriteRenderer m_Num100 = null;
    public SpriteRenderer m_Num010 = null;
    public SpriteRenderer m_Num001 = null;

    private Animation m_Anim = null;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Anim.isPlaying == false)
        {
            gameObject.SetActive(false);
        }
    }

    public void setHands(int hands)
    {
        if (hands > 999)
        {
            hands = 999;
        }

        if (hands >= 100)
        {
            m_Num100.sprite = m_NumberSprites[(hands / 100) % 10];
        }
        else
        {
            m_Num100.sprite = null;
        }
        m_Num010.sprite = m_NumberSprites[(hands / 10) % 10];
        m_Num001.sprite = m_NumberSprites[hands % 10];
    }

    public void play()
    {
        if (m_Anim == null)
        {
            m_Anim = GetComponent<Animation>();
        }

        if (m_Anim != null)
        {
            m_Anim.Stop();
            m_Anim.Play();
            gameObject.SetActive(true);

            SoundUtil.PlaySE(SEID.SE_BATLE_SKILL_HANDS);
        }
    }

    public bool isPlaying()
    {
        if (m_Anim != null)
        {
            return m_Anim.isPlaying;
        }
        return false;
    }
}
