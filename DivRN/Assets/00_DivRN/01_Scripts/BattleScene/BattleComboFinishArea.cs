using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleComboFinishArea : MonoBehaviour
{
    public GameObject[] m_FinishObjs = new GameObject[4];
    public GameObject m_FinishAnimObj = null;

    private static SEID[] m_SEIDs =
    {
        SEID.VOICE_INGAME_QUEST_NICE,	// NICE!
        SEID.VOICE_INGAME_QUEST_COOL,	// COOL!
        SEID.VOICE_INGAME_QUEST_GREAT,	// GREAT!
        SEID.VOICE_INGAME_QUEST_EXCELLENT,	// EXCELLENT!!
        SEID.VOICE_INGAME_QUEST_UNBELIEVABLE,	// UNBELIEVABLE!!!
        SEID.VOICE_INGAME_QUEST_MARVELOUS,	// MARVELOUS!!!!
        SEID.VOICE_INGAME_QUEST_DIVINE,	// DIVINE!!!!!
    };


    private int m_TextIndex = -1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_TextIndex >= 0)
        {
            Animation anim = m_FinishAnimObj.GetComponent<Animation>();
            if (anim == null || anim.isPlaying == false)
            {
                m_TextIndex = -1;
                m_FinishAnimObj.SetActive(false);
            }
        }
    }

    public bool setComboFinish(int combo_count)
    {
        m_TextIndex = -1;
        if (combo_count < 1)
        {
            m_FinishAnimObj.SetActive(false);
            return false;
        }

        if (combo_count >= 25)
        {
            // DIVINE!!!!!
            m_TextIndex = 6;
        }
        else if (combo_count >= 20)
        {
            // MARVELOUS!!!!
            m_TextIndex = 5;
        }
        else if (combo_count >= 15)
        {
            // UNBELIEVABLE!!!
            m_TextIndex = 4;
        }
        else if (combo_count >= 10)
        {
            // EXCELLENT!!
            m_TextIndex = 3;
        }
        else if (combo_count >= 7)
        {
            // GREAT!
            m_TextIndex = 2;
        }
        else if (combo_count >= 4)
        {
            // COOL!
            m_TextIndex = 1;
        }
        else if (combo_count >= 1)
        {
            // NICE!
            m_TextIndex = 0;
        }

        for (int idx = 0; idx < m_FinishObjs.Length; idx++)
        {
            m_FinishObjs[idx].SetActive(idx == m_TextIndex);
        }

        m_FinishAnimObj.SetActive(true);

        SoundUtil.PlaySE(m_SEIDs[m_TextIndex]);

        return true;
    }

    public bool isUpdating()
    {
        return (m_TextIndex >= 0);
    }
}
