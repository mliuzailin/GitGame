using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCutinViewControl : MonoBehaviour
{

    public Sprite m_FirstAttackSprite = null;
    public Sprite m_BackAttackSprite = null;
    public Sprite m_StandReadySprite = null;
    public Sprite m_EvolveSprite = null;

    public string m_FirstAttackAnim = null;
    public string m_BackAttackAnim = null;
    public string m_StandReadyAnim = null;
    public string m_EvolveAnim = null;

    private GameObject m_Obj = null;
    private Animation m_Animation = null;
    private Image m_Image = null;
    private bool m_IsPlaying = false;

    public enum TitleType
    {
        NONE,
        FIRST_ATTACK,
        BACK_ATTACK,
        STAND_READY,
        EVOLVE,
    }

    // Use this for initialization
    void Start()
    {
        m_Obj = transform.GetChild(0).gameObject;
        m_Animation = m_Obj.GetComponent<Animation>();
        m_Image = m_Obj.transform.GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        m_IsPlaying = false;
        if (m_Obj.IsActive())
        {
            if (m_Animation.isPlaying)
            {
                m_IsPlaying = true;
            }
            else
            {
                m_Obj.SetActive(false);
            }
        }
    }

    public void startAnim(TitleType title_type)
    {
        Sprite sprite = null;
        string anim_name = null;
        switch (title_type)
        {
            case TitleType.FIRST_ATTACK:
                sprite = m_FirstAttackSprite;
                anim_name = m_FirstAttackAnim;
                break;

            case TitleType.BACK_ATTACK:
                sprite = m_BackAttackSprite;
                anim_name = m_BackAttackAnim;
                break;

            case TitleType.STAND_READY:
                sprite = m_StandReadySprite;
                anim_name = m_StandReadyAnim;
                break;

            case TitleType.EVOLVE:
                sprite = m_EvolveSprite;
                anim_name = m_EvolveAnim;
                break;
        }

        m_Image.sprite = sprite;
        m_Image.SetNativeSize();

        {
            Transform trans = m_Animation.gameObject.transform;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }
        {
            Transform trans = m_Animation.gameObject.transform.GetChild(0);
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        m_Obj.SetActive(true);
        m_Animation.Play(anim_name);
        m_IsPlaying = true;
    }

    public bool isPlaying()
    {
        return m_IsPlaying;
    }
}
