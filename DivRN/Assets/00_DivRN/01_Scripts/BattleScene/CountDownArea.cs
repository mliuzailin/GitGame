using UnityEngine;
using System.Collections;

public class CountDownArea : MonoBehaviour
{
    public GameObject[] m_CountNum = new GameObject[9];
    public GameObject m_CountNumAnimObj = null;
    public GameObject m_BgAnimObj = null;
    public Color32[] m_GaugeColor = new Color32[9];
    public SpriteRenderer m_GaugeSpriteRenderer = null;

    private Animation m_CountNumAnim = null;
    private Animation m_BgAnim = null;

    private int m_CurrentTime = -1;
    private int m_OldTime = -1;
    private static SEID[] se_array =
    {
        SEID.SE_BATLE_COUNTDOWN_1,
        SEID.SE_BATLE_COUNTDOWN_2,
        SEID.SE_BATLE_COUNTDOWN_3,
        SEID.SE_BATLE_COUNTDOWN_4,
        SEID.SE_BATLE_COUNTDOWN_5,
        SEID.SE_BATLE_COUNTDOWN_6,
        SEID.SE_BATLE_COUNTDOWN_7,
        SEID.SE_BATLE_COUNTDOWN_8,
        SEID.SE_BATLE_COUNTDOWN_9,
    };

    // Use this for initialization
    void Start()
    {
        if (m_CountNumAnimObj != null)
        {
            m_CountNumAnim = m_CountNumAnimObj.GetComponent<Animation>();
        }

        if (m_BgAnimObj != null)
        {
            m_BgAnim = m_BgAnimObj.GetComponent<Animation>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentTime > 0)
        {
            {
                int old_sec = m_OldTime / 10;
                int current_sec = m_CurrentTime / 10;
                if (current_sec >= 0
                    && current_sec < 9
                    && current_sec + 1 == old_sec)
                {
                    // スプライトの切り替えでなくオブジェクトを切り替えている理由：画像の中心位置が数字毎に異なっている場合があるため.
                    for (int idx = 0; idx < m_CountNum.Length; idx++)
                    {
                        m_CountNum[idx].SetActive(idx == current_sec);
                    }

                    if (m_CountNumAnimObj != null)
                    {
                        m_CountNumAnimObj.SetActive(true);
                    }
                    if (m_CountNumAnim != null)
                    {
                        m_CountNumAnim.Stop();
                        m_CountNumAnim.Play(InGameDefine.ANIM_COUNTDOWN);
                    }

                    if (m_BgAnimObj != null)
                    {
                        m_BgAnimObj.SetActive(true);
                    }
                    if (m_BgAnim != null)
                    {
                        m_BgAnim.Stop();
                        m_BgAnim.Play(InGameDefine.ANIM_COUNTDOWNBACK);
                        if (m_GaugeSpriteRenderer != null)
                        {
                            m_GaugeSpriteRenderer.color = m_GaugeColor[current_sec];
                        }
                    }

                    SoundUtil.PlaySE(se_array[current_sec]);
                }

                m_OldTime = m_CurrentTime;
            }
        }
        else
        {
            if (m_CountNumAnimObj != null)
            {
                m_CountNumAnimObj.SetActive(false);
            }

            if (m_BgAnimObj != null)
            {
                m_BgAnimObj.SetActive(false);
            }
        }
    }

    public void initTime(float time)
    {
        m_CurrentTime = (int)(time * 10.0f);
        m_OldTime = m_CurrentTime + 1;
    }

    public void setTime(float time)
    {
        m_CurrentTime = (int)(time * 10.0f);
    }
}
