/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	InGamePartyUnit.cs
	@brief	非アクティブオブジェクト一元化クラス
	@author Developer
	@date 	2012/10/04
*/
/*==========================================================================*/
/*==========================================================================*/

/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using M4u;

public class InGamePartyManager : M4uContextMonoBehaviour
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public Sprite[] m_InfoButton;
    public Image m_GaugeSp;

    M4uProperty<Sprite> info_button = new M4uProperty<Sprite>();
    public Sprite Info_button { get { return info_button.Value2; } set { info_button.Value2 = value; } }

    M4uProperty<string> total_hands_num = new M4uProperty<string>();
    public string Total_hands_num { get { return total_hands_num.Value2; } set { total_hands_num.Value2 = value; } }

    M4uProperty<bool> total_hands_active = new M4uProperty<bool>();
    public bool Total_hands_active { get { return total_hands_active.Value2; } set { total_hands_active.Value2 = value; } }

    M4uProperty<string> unit_num = new M4uProperty<string>();
    public string Unit_num { get { return unit_num.Value2; } set { unit_num.Value2 = value; } }

    M4uProperty<string> coin_num = new M4uProperty<string>();
    public string Coin_num { get { return coin_num.Value2; } set { coin_num.Value2 = value; } }

    M4uProperty<string> ticket_num = new M4uProperty<string>();
    public string Ticket_num { get { return ticket_num.Value2; } set { ticket_num.Value2 = value; } }

    M4uProperty<bool> timer_active = new M4uProperty<bool>();
    public bool Timer_active { get { return timer_active.Value2; } set { timer_active.Value2 = value; } }

    M4uProperty<string> timer_count = new M4uProperty<string>();
    public string Timer_count { get { return timer_count.Value2; } set { timer_count.Value2 = value; } }

    M4uProperty<bool> timer_minus_active = new M4uProperty<bool>();
    public bool Timer_minus_active { get { return timer_minus_active.Value2; } set { timer_minus_active.Value2 = value; } }

    M4uProperty<string> timer_minus_count = new M4uProperty<string>();
    public string Timer_minus_count { get { return timer_minus_count.Value2; } set { timer_minus_count.Value2 = value; } }

    M4uProperty<bool> skill_Invalid_active = new M4uProperty<bool>();
    public bool Skill_Invalid_active { get { return skill_Invalid_active.Value2; } set { skill_Invalid_active.Value2 = value; } }

    M4uProperty<string> skill_Invalid_count = new M4uProperty<string>();
    public string Skill_Invalid_count { get { return skill_Invalid_count.Value2; } set { skill_Invalid_count.Value2 = value; } }

    M4uProperty<Sprite> hero_face = new M4uProperty<Sprite>();
    public Sprite Hero_face { get { return hero_face.Value2; } set { hero_face.Value2 = value; } }

    M4uProperty<Texture> hero_face_mask = new M4uProperty<Texture>();
    public Texture Hero_face_mask { get { return hero_face_mask.Value; } set { hero_face_mask.Value = value; } }

    M4uProperty<bool> hero_balloon1_active = new M4uProperty<bool>();
    public bool Hero_balloon1_active { get { return hero_balloon1_active.Value2; } set { hero_balloon1_active.Value2 = value; } }

    M4uProperty<string> hero_balloon_text = new M4uProperty<string>();
    public string Hero_balloon_text { get { return hero_balloon_text.Value2; } set { hero_balloon_text.Value2 = value; } }

    M4uProperty<bool> hero_balloon2_active = new M4uProperty<bool>();
    public bool Hero_balloon2_active { get { return hero_balloon2_active.Value2; } set { hero_balloon2_active.Value2 = value; } }

    M4uProperty<Sprite> hero_balloon = new M4uProperty<Sprite>();
    public Sprite Hero_balloon { get { return hero_balloon.Value2; } set { hero_balloon.Value2 = value; } }

    M4uProperty<Color> hero_balloon2_color = new M4uProperty<Color>();
    public Color Hero_balloon2_color { get { return hero_balloon2_color.Value2; } set { hero_balloon2_color.Value2 = value; } }

    M4uProperty<float> heroScale = new M4uProperty<float>();
    public float HeroScale { get { return heroScale.Value2; } set { heroScale.Value2 = value; } }

    M4uProperty<Color> heroColor = new M4uProperty<Color>();
    public Color HeroColor { get { return heroColor.Value2; } set { heroColor.Value2 = value; } }

    M4uProperty<Color> spColor = new M4uProperty<Color>();
    public Color SpColor { get { return spColor.Value2; } set { spColor.Value2 = value; } }

    M4uProperty<float> heroBalloonY = new M4uProperty<float>();
    public float HeroBalloonY { get { return heroBalloonY.Value2; } set { heroBalloonY.Value2 = value; } }

    M4uProperty<bool> isActiveAutoPlayOnButton = new M4uProperty<bool>();
    public bool IsActiveAutoPlayOnButton { get { return isActiveAutoPlayOnButton.Value2; } set { isActiveAutoPlayOnButton.Value2 = value; } }

    M4uProperty<bool> isActiveAutoPlayOffButton = new M4uProperty<bool>();
    public bool IsActiveAutoPlayOffButton { get { return isActiveAutoPlayOffButton.Value2; } set { isActiveAutoPlayOffButton.Value2 = value; } }

    M4uProperty<bool> isActiveAutoPlayNgButton = new M4uProperty<bool>();
    public bool IsActiveAutoPlayNgButton { get { return isActiveAutoPlayNgButton.Value2; } set { isActiveAutoPlayNgButton.Value2 = value; } }

    [SerializeField]
    private GameObject m_AutoButtonOn;

    private Animation m_AutoButtonOnAnimation;

    private float m_WaitTime;
    private float[] gauge_sp_amount =
    {
        0.000f, 0.047f, 0.096f, 0.143f, 0.191f, 0.248f, 0.304f,0.351f,  0.399f, 0.446f,0.493f,  0.557f, 0.604f, 0.651f, 0.699f, 0.760f, 0.812f, 0.859f, 0.906f, 0.954f, 1.000f
    };

    public enum PartyAilmentType : int
    {
        TimePlus = 0,
        TimeMinus,
        HandChance,

        Max
    }

    private int[] m_PartyAilmentTurn = new int[(int)PartyAilmentType.Max];
    private int m_PartyAilmentIndex;
    private float m_PartyAilmentTime;
    private float PARTY_AILMENT_TIME = 1.0f;

    private BattleParam.AutoPlayState m_AutoPlayButtonState = BattleParam.AutoPlayState.NONE;
    private float m_AutoPlayButtonCancelTimer = 0.0f;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    void Awake()
    {
        gameObject.GetComponent<M4uContextRoot>().Context = this;
        m_AutoButtonOnAnimation = m_AutoButtonOn.GetComponent<Animation>();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    void Start()
    {
        Info_button = m_InfoButton[0];
        Unit_num = "0";
        Coin_num = "0";
        Ticket_num = "0";
        m_WaitTime = 0;
        Hero_balloon2_color = Color.white;
        m_PartyAilmentIndex = -1;
        m_PartyAilmentTime = PARTY_AILMENT_TIME;
        HeroScale = 1;
        HeroColor = Color.white;
        SpColor = Color.white;
        HeroBalloonY = 630;
        IsActiveAutoPlayOnButton = false;
        IsActiveAutoPlayOffButton = false;
        IsActiveAutoPlayNgButton = true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    void Update()
    {
        if (m_WaitTime > 0)
        {
            m_WaitTime -= Time.deltaTime;
            if (m_WaitTime <= 0)
            {
                m_WaitTime = 0;
                Total_hands_active = false;
            }
        }
        if (m_PartyAilmentIndex >= 0)
        {
            Timer_active = false;
            Timer_minus_active = false;
            Skill_Invalid_active = false;
            bool change_index = false;
            if (m_PartyAilmentTurn[m_PartyAilmentIndex] > 0)
            {
                m_PartyAilmentTime -= Time.deltaTime;
                if (m_PartyAilmentTime <= 0)
                {
                    m_PartyAilmentTime += PARTY_AILMENT_TIME;
                    change_index = true;
                }
            }
            else
            {
                change_index = true;
            }
            if (change_index == true)
            {
                int index = m_PartyAilmentIndex;
                do
                {
                    ++index;
                    if (index >= (int)PartyAilmentType.Max) index = 0;
                    if (m_PartyAilmentTurn[index] > 0)
                    {
                        break;
                    }
                } while (index != m_PartyAilmentIndex);
                if (m_PartyAilmentIndex != index)
                {
                    m_PartyAilmentIndex = index;
                }
                else
                {
                    if (m_PartyAilmentTurn[(int)m_PartyAilmentIndex] <= 0)
                    {
                        m_PartyAilmentIndex = -1;
                    }
                }
            }
            switch ((PartyAilmentType)m_PartyAilmentIndex)
            {
                case PartyAilmentType.TimePlus:
                    {
                        Timer_active = true;
                        Timer_count = m_PartyAilmentTurn[m_PartyAilmentIndex].ToString("00");
                    }
                    break;
                case PartyAilmentType.TimeMinus:
                    {
                        Timer_minus_active = true;
                        Timer_minus_count = m_PartyAilmentTurn[m_PartyAilmentIndex].ToString("00");
                    }
                    break;
                case PartyAilmentType.HandChance:
                    {
                        Skill_Invalid_active = true;
                        Skill_Invalid_count = m_PartyAilmentTurn[m_PartyAilmentIndex].ToString("00");
                    }
                    break;
                default:
                    break;
            }
        }

        // オートプレイボタンの表示を更新
        {
            BattleParam.AutoPlayState auto_play_state = BattleParam.getAutoPlayState();

            if (auto_play_state == BattleParam.AutoPlayState.CANCEL)
            {
                if (m_AutoPlayButtonCancelTimer <= 0.0f)
                {
                    m_AutoPlayButtonCancelTimer = 0.5f;
                }
                else
                {
                    m_AutoPlayButtonCancelTimer -= Time.deltaTime;
                    if (m_AutoPlayButtonCancelTimer <= 0.0f)
                    {
                        m_AutoPlayButtonCancelTimer = 0.0f;
                        BattleParam.setAutoPlayState(BattleParam.AutoPlayState.OFF);
                        auto_play_state = BattleParam.AutoPlayState.OFF;
                    }
                }
            }

            if (auto_play_state != m_AutoPlayButtonState)
            {
                m_AutoPlayButtonState = auto_play_state;

                switch (m_AutoPlayButtonState)
                {
                    case BattleParam.AutoPlayState.ON:
                        IsActiveAutoPlayOnButton = true;
                        IsActiveAutoPlayOffButton = true;
                        IsActiveAutoPlayNgButton = false;
                        if (m_AutoButtonOnAnimation.isPlaying == false)
                        {
                            m_AutoButtonOnAnimation.Play();
                        }

                        break;

                    case BattleParam.AutoPlayState.OFF:
                        IsActiveAutoPlayOnButton = false;
                        IsActiveAutoPlayOffButton = true;
                        IsActiveAutoPlayNgButton = false;
                        if (m_AutoButtonOnAnimation.isPlaying)
                        {
                            m_AutoButtonOnAnimation.Stop();
                        }
                        break;

                    default:
                        IsActiveAutoPlayOnButton = false;
                        IsActiveAutoPlayOffButton = false;
                        IsActiveAutoPlayNgButton = true;
                        break;
                }
            }
        }
    }

    public void setupHero()
    {
        Hero_balloon = InGamePlayerParty.Instance.m_Balloon2[0];
        Hero_face = InGamePlayerParty.Instance.m_HeroSprite;
        Hero_face_mask = InGamePlayerParty.Instance.m_HeroSprite_mask;
    }

    public void changeInfoButton(bool sw)
    {
        if (sw == false)
        {
            Info_button = m_InfoButton[0];
        }
        else
        {
            Info_button = m_InfoButton[1];
        }
    }

    public void setGaugeSp(int num)
    {
        if (num < 0 || num > 20)
        {
            return;
        }

        m_GaugeSp.fillAmount = gauge_sp_amount[num];
    }

    public void setTotalHands(int num)
    {
        if (num < 0)
        {
            Total_hands_active = false;
        }
        else
        {
            Total_hands_active = true;
            Total_hands_num = num.ToString();
            if (num == 0)
            {
                m_WaitTime = 0.5f;
            }
        }
    }

    public void ailmentTurnListClear()
    {
        for (int i = 0; i < (int)PartyAilmentType.Max; ++i)
        {
            m_PartyAilmentTurn[i] = 0;
        }
    }

    public void setAilmentTurn(PartyAilmentType type, int turn)
    {
        m_PartyAilmentTurn[(int)type] = turn;
        if (m_PartyAilmentIndex < 0)
        {
            m_PartyAilmentIndex = (int)type;
        }
    }
}
