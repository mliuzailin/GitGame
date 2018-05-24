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
using M4u;

public class InGamePartyUnit : M4uContextMonoBehaviour
{

    enum HP_SPRITE_INDEX : int
    {
        HP_SPRITE_100 = 0,
        HP_SPRITE_50_99,
        HP_SPRITE_0_49,
        HP_SPRITE_POISON,
    }

    enum BALLOON2_TYPE : int
    {
        READY = 0,
        READY_MUTE,
        HUKIDASHI_MUTE,
    }

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public Animation m_HandsAnimation = null;               //!<
    public Image m_UnitSkillGauge = null;              //!<
    public Image m_UnitHPGauge = null;
    public Animation m_HpNumAnimation = null;
    public Animation m_ReadyAnimation = null;
    [SerializeField]
    private Transform m_UnitRoot = null;
    [SerializeField]
    private Transform m_UnitSelectRoot = null;
    [SerializeField]
    private Transform m_UnitObject = null;

    M4uProperty<Sprite> unit_base = new M4uProperty<Sprite>();
    public Sprite Unit_base { get { return unit_base.Value2; } set { unit_base.Value2 = value; } }

    M4uProperty<Sprite> skill_color1_1 = new M4uProperty<Sprite>();
    public Sprite Skill_color1_1 { get { return skill_color1_1.Value2; } set { skill_color1_1.Value2 = value; } }

    M4uProperty<bool> skill_col1_1_active = new M4uProperty<bool>();
    public bool Skill_col1_1_active { get { return skill_col1_1_active.Value2; } set { skill_col1_1_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color1_2 = new M4uProperty<Sprite>();
    public Sprite Skill_color1_2 { get { return skill_color1_2.Value2; } set { skill_color1_2.Value2 = value; } }

    M4uProperty<bool> skill_col1_2_active = new M4uProperty<bool>();
    public bool Skill_col1_2_active { get { return skill_col1_2_active.Value2; } set { skill_col1_2_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color1_3 = new M4uProperty<Sprite>();
    public Sprite Skill_color1_3 { get { return skill_color1_3.Value2; } set { skill_color1_3.Value2 = value; } }

    M4uProperty<bool> skill_col1_3_active = new M4uProperty<bool>();
    public bool Skill_col1_3_active { get { return skill_col1_3_active.Value2; } set { skill_col1_3_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color1_4 = new M4uProperty<Sprite>();
    public Sprite Skill_color1_4 { get { return skill_color1_4.Value2; } set { skill_color1_4.Value2 = value; } }

    M4uProperty<bool> skill_col1_4_active = new M4uProperty<bool>();
    public bool Skill_col1_4_active { get { return skill_col1_4_active.Value2; } set { skill_col1_4_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color1_5 = new M4uProperty<Sprite>();
    public Sprite Skill_color1_5 { get { return skill_color1_5.Value2; } set { skill_color1_5.Value2 = value; } }

    M4uProperty<bool> skill_col1_5_active = new M4uProperty<bool>();
    public bool Skill_col1_5_active { get { return skill_col1_5_active.Value2; } set { skill_col1_5_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color2_1 = new M4uProperty<Sprite>();
    public Sprite Skill_color2_1 { get { return skill_color2_1.Value2; } set { skill_color2_1.Value2 = value; } }

    M4uProperty<bool> skill_col2_1_active = new M4uProperty<bool>();
    public bool Skill_col2_1_active { get { return skill_col2_1_active.Value2; } set { skill_col2_1_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color2_2 = new M4uProperty<Sprite>();
    public Sprite Skill_color2_2 { get { return skill_color2_2.Value2; } set { skill_color2_2.Value2 = value; } }

    M4uProperty<bool> skill_col2_2_active = new M4uProperty<bool>();
    public bool Skill_col2_2_active { get { return skill_col2_2_active.Value2; } set { skill_col2_2_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color2_3 = new M4uProperty<Sprite>();
    public Sprite Skill_color2_3 { get { return skill_color2_3.Value2; } set { skill_color2_3.Value2 = value; } }

    M4uProperty<bool> skill_col2_3_active = new M4uProperty<bool>();
    public bool Skill_col2_3_active { get { return skill_col2_3_active.Value2; } set { skill_col2_3_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color2_4 = new M4uProperty<Sprite>();
    public Sprite Skill_color2_4 { get { return skill_color2_4.Value2; } set { skill_color2_4.Value2 = value; } }

    M4uProperty<bool> skill_col2_4_active = new M4uProperty<bool>();
    public bool Skill_col2_4_active { get { return skill_col2_4_active.Value2; } set { skill_col2_4_active.Value2 = value; } }

    M4uProperty<Sprite> skill_color2_5 = new M4uProperty<Sprite>();
    public Sprite Skill_color2_5 { get { return skill_color2_5.Value2; } set { skill_color2_5.Value2 = value; } }

    M4uProperty<bool> skill_col2_5_active = new M4uProperty<bool>();
    public bool Skill_col2_5_active { get { return skill_col2_5_active.Value2; } set { skill_col2_5_active.Value2 = value; } }

    M4uProperty<bool> balloon1_active = new M4uProperty<bool>();
    public bool Balloon1_active { get { return balloon1_active.Value2; } set { balloon1_active.Value2 = value; } }

    M4uProperty<string> balloon_text = new M4uProperty<string>();
    public string Balloon_text { get { return balloon_text.Value2; } set { balloon_text.Value2 = value; } }

    M4uProperty<bool> balloon2_active = new M4uProperty<bool>();
    public bool Balloon2_active { get { return balloon2_active.Value2; } set { balloon2_active.Value2 = value; } }

    M4uProperty<bool> skill_tips_active = new M4uProperty<bool>();
    public bool Skill_tips_active { get { return skill_tips_active.Value2; } set { skill_tips_active.Value2 = value; } }

    M4uProperty<bool> hands_active = new M4uProperty<bool>();
    public bool Hands_active { get { return hands_active.Value2; } set { hands_active.Value2 = value; } }

    M4uProperty<Sprite> hands_text = new M4uProperty<Sprite>();
    public Sprite Hands_text { get { return hands_text.Value2; } set { hands_text.Value2 = value; } }

    M4uProperty<Sprite> hands_num1 = new M4uProperty<Sprite>();
    public Sprite Hands_num1 { get { return hands_num1.Value2; } set { hands_num1.Value2 = value; } }

    M4uProperty<Sprite> hands_num2 = new M4uProperty<Sprite>();
    public Sprite Hands_num2 { get { return hands_num2.Value2; } set { hands_num2.Value2 = value; } }

    M4uProperty<bool> unit_base_active = new M4uProperty<bool>();
    public bool Unit_base_active { get { return unit_base_active.Value2; } set { unit_base_active.Value2 = value; } }

    M4uProperty<bool> empty_active = new M4uProperty<bool>();
    public bool Empty_active { get { return empty_active.Value2; } set { empty_active.Value2 = value; } }

    M4uProperty<float> unit_base_x = new M4uProperty<float>();
    public float Unit_base_x { get { return unit_base_x.Value2; } set { unit_base_x.Value2 = value; } }

    M4uProperty<float> unit_base_y = new M4uProperty<float>();
    public float Unit_base_y { get { return unit_base_y.Value2; } set { unit_base_y.Value2 = value; } }

    M4uProperty<Sprite> skill_color1_head = new M4uProperty<Sprite>();
    public Sprite Skill_color1_head { get { return skill_color1_head.Value2; } set { skill_color1_head.Value2 = value; } }

    M4uProperty<Sprite> skill_color2_head = new M4uProperty<Sprite>();
    public Sprite Skill_color2_head { get { return skill_color2_head.Value2; } set { skill_color2_head.Value2 = value; } }

    M4uProperty<bool> skill_empty1 = new M4uProperty<bool>();
    public bool Skill_empty1 { get { return skill_empty1.Value2; } set { skill_empty1.Value2 = value; } }

    M4uProperty<bool> skill_empty2 = new M4uProperty<bool>();
    public bool Skill_empty2 { get { return skill_empty2.Value2; } set { skill_empty2.Value2 = value; } }

    M4uProperty<float> hands_y = new M4uProperty<float>();
    public float Hands_y { get { return hands_y.Value2; } set { hands_y.Value2 = value; } }

    M4uProperty<bool> attack_active = new M4uProperty<bool>();
    public bool Attack_active { get { return attack_active.Value2; } set { attack_active.Value2 = value; } }

    M4uProperty<bool> defense_active = new M4uProperty<bool>();
    public bool Defense_active { get { return defense_active.Value2; } set { defense_active.Value2 = value; } }

    M4uProperty<Sprite> attack_icon = new M4uProperty<Sprite>();
    public Sprite Attack_icon { get { return attack_icon.Value2; } set { attack_icon.Value2 = value; } }

    M4uProperty<Sprite> defense_icon = new M4uProperty<Sprite>();
    public Sprite Defense_icon { get { return defense_icon.Value2; } set { defense_icon.Value2 = value; } }

    M4uProperty<bool> poison_active = new M4uProperty<bool>();
    public bool Poison_active { get { return poison_active.Value2; } set { poison_active.Value2 = value; } }

    M4uProperty<string> poison_count = new M4uProperty<string>();
    public string Poison_count { get { return poison_count.Value2; } set { poison_count.Value2 = value; } }

    M4uProperty<bool> dead_active = new M4uProperty<bool>();
    public bool Dead_active { get { return dead_active.Value2; } set { dead_active.Value2 = value; } }

    M4uProperty<Sprite> icon_heart = new M4uProperty<Sprite>();
    public Sprite Icon_heart { get { return icon_heart.Value2; } set { icon_heart.Value2 = value; } }

    M4uProperty<Sprite> unit_hp_bar = new M4uProperty<Sprite>();
    public Sprite Unit_hp_bar { get { return unit_hp_bar.Value2; } set { unit_hp_bar.Value2 = value; } }

    M4uProperty<Sprite> balloon2_image = new M4uProperty<Sprite>();
    public Sprite Balloon2_image { get { return balloon2_image.Value2; } set { balloon2_image.Value2 = value; } }

    M4uProperty<Color> balloon2_color = new M4uProperty<Color>();
    public Color Balloon2_color { get { return balloon2_color.Value2; } set { balloon2_color.Value2 = value; } }

    M4uProperty<Sprite> attribute_circle = new M4uProperty<Sprite>();
    public Sprite Attribute_circle { get { return attribute_circle.Value2; } set { attribute_circle.Value2 = value; } }

    M4uProperty<string> hp_num = new M4uProperty<string>();
    public string Hp_num { get { return hp_num.Value2; } set { hp_num.Value2 = value; } }

    M4uProperty<bool> dark_active = new M4uProperty<bool>();
    public bool Dark_active { get { return dark_active.Value2; } set { dark_active.Value2 = value; } }

    M4uProperty<string> dark_count = new M4uProperty<string>();
    public string Dark_count { get { return dark_count.Value2; } set { dark_count.Value2 = value; } }

    M4uProperty<bool> not_heal_active = new M4uProperty<bool>();
    public bool Not_heal_active { get { return not_heal_active.Value2; } set { not_heal_active.Value2 = value; } }

    M4uProperty<string> not_heal_count = new M4uProperty<string>();
    public string Not_heal_count { get { return not_heal_count.Value2; } set { not_heal_count.Value2 = value; } }

    M4uProperty<bool> isSelectEnable = new M4uProperty<bool>();
    public bool IsSelectEnable { get { return isSelectEnable.Value2; } set { isSelectEnable.Value2 = value; } }

    M4uProperty<float> unitIconScale = new M4uProperty<float>();
    public float UnitIconScale { get { return unitIconScale.Value2; } set { unitIconScale.Value2 = value; } }

    M4uProperty<Color> unitIconColor = new M4uProperty<Color>();
    public Color UnitIconColor { get { return unitIconColor.Value2; } set { unitIconColor.Value2 = value; } }

    M4uProperty<bool> nonAS_active = new M4uProperty<bool>();
    public bool NonAS_active { get { return nonAS_active.Value2; } set { nonAS_active.Value2 = value; } }

    M4uProperty<float> unitBallonY = new M4uProperty<float>();
    public float UnitBallonY { get { return unitBallonY.Value2; } set { unitBallonY.Value2 = value; } }

    private CharaOnce m_PartyChara = null;                      //!<
    private Sprite[] m_Skill_cost = new Sprite[2 * 5];      //!<
    private bool[] m_Skill_cost_active = new bool[2 * 5];       //!<
    private float m_ClosePosY = 0;                      //!<
    private float m_OpenPosY = 0;                       //!<
    private float m_WaitTime;                                           //!<
    private bool m_SkillWindow = false;
    private GlobalDefine.PartyCharaIndex m_CharaIndex = GlobalDefine.PartyCharaIndex.ERROR;
    private int m_Dead = 0;
    private BALLOON2_TYPE m_Balloon2 = BALLOON2_TYPE.READY;
    private BALLOON2_TYPE m_PreBalloon2 = BALLOON2_TYPE.READY;
    private HP_SPRITE_INDEX m_HpBar = HP_SPRITE_INDEX.HP_SPRITE_100;

    //----------------------------------------
    // プレイヤーパラメータ関連定義。
    //----------------------------------------
    private int m_PartyTotalHPPrv = -1;     //!< プレイヤーHP現在値。
    private int m_PartyTotalHPMaxPrv = -1;      //!< プレイヤーHP最大値。
    private int m_PartyTotalSPPrv = -1;     //!< プレイヤーSP現在値。
    private int m_PartyTotalSkillPrv = -1;      //!< プレイヤースキル発動カウント現在値。
    private int m_PartyTotalSkillMaxPrv = -1;       //!< プレイヤースキル発動カウント最大値。
    private int m_PartyHandsPrv = -1;       //!< プレイヤースキル発動カウント最大値。
    private int m_PartyTotalHandsPrv = -1;      //!< プレイヤースキル発動カウント最大値。
    private bool m_DeadBalloon1 = false;
    private bool m_DeadBalloon2 = false;
    private bool m_ASTurnInfoOpen = false;
    private bool m_NormalSkillInfoOpen = false;

    private Color poison_color = new Color(0.65f, 0, 1);
    private Color Balloon_gray = new Color(0.4f, 0.4f, 0.4f, 1);

    public enum CharaAilmentType : int
    {
        Poison = 0,
        NotHeal,

        Max
    }

    private int[] m_CharaAilmentTurn = new int[(int)CharaAilmentType.Max];
    private int m_CharaAilmentIndex;
    private float m_CharaAilmentTime;
    private float CHARA_AILMENT_TIME = 1.0f;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    public void SetCharaID(CharaOnce partyChara, bool newBattle, float posx, float posy, GlobalDefine.PartyCharaIndex index, bool balloon_active)
    {
        m_PartyChara = partyChara;
        if (m_PartyChara != null
        && m_PartyChara.m_bHasCharaMasterDataParam)
        {
            // キャラアイコンの設定
            UnitIconImageProvider.Instance.Get(
                m_PartyChara.m_CharaMasterDataParam.fix_id,
                sprite =>
                {
                    Unit_base = sprite;
                });

            MasterDataSkillActive cActiveSkill = null;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0) { cActiveSkill = MasterDataUtil.GetActiveSkillParamFromID(m_PartyChara.m_CharaMasterDataParam.skill_active0); }
                else if (i == 1)
                {
                    MasterDataSkillPassive cPassiveSkill = MasterDataUtil.GetPassiveSkillParamFromID(m_PartyChara.m_CharaMasterDataParam.skill_passive);
                    if (cPassiveSkill == null)
                    {
                        cActiveSkill = MasterDataUtil.GetActiveSkillParamFromID(m_PartyChara.m_CharaMasterDataParam.skill_active1);
                    }
                    else
                    {
                        cActiveSkill = null;
                    }
                }
                MasterDataDefineLabel.ElementType[] anCostElement = new MasterDataDefineLabel.ElementType[5];
                if (cActiveSkill != null)
                {
                    anCostElement[0] = cActiveSkill.cost1;
                    anCostElement[1] = cActiveSkill.cost2;
                    anCostElement[2] = cActiveSkill.cost3;
                    anCostElement[3] = cActiveSkill.cost4;
                    anCostElement[4] = cActiveSkill.cost5;
                    if (i == 0)
                    {
                        Skill_color1_head = MainMenuUtil.GetSkillElementColor("S1", cActiveSkill.skill_element);
                    }
                    else
                    {
                        Skill_color2_head = MainMenuUtil.GetSkillElementColor("S2", cActiveSkill.skill_element);
                    }
                }
                else
                {
                    anCostElement[0] = MasterDataDefineLabel.ElementType.NONE;
                    anCostElement[1] = MasterDataDefineLabel.ElementType.NONE;
                    anCostElement[2] = MasterDataDefineLabel.ElementType.NONE;
                    anCostElement[3] = MasterDataDefineLabel.ElementType.NONE;
                    anCostElement[4] = MasterDataDefineLabel.ElementType.NONE;
                    if (i == 0)
                    {
                        Skill_empty1 = true;
                    }
                    else
                    {
                        Skill_empty2 = true;
                    }
                }
                int skill_idx = 0;
                for (int j = 4; j >= 0; j--)
                {
                    if (anCostElement[j] != MasterDataDefineLabel.ElementType.NONE)
                    {
                        m_Skill_cost[i * 5 + skill_idx] = InGamePlayerParty.Instance.m_SkillElementSprite[(int)anCostElement[j]];
                        m_Skill_cost_active[i * 5 + skill_idx] = true;
                        ++skill_idx;
                    }
                }
            }
            setSkillCost();
            Hands_text = InGamePlayerParty.Instance.m_HandsSprite[(int)m_PartyChara.m_CharaMasterDataParam.element];
            Unit_base_active = true;
            Attribute_circle = MainMenuUtil.GetElementCircleSprite(m_PartyChara.m_CharaMasterDataParam.element);
            IsSelectEnable = true;
        }
        else
        {
            Empty_active = true;
            Skill_empty1 = true;
            Skill_empty2 = true;
            Attribute_circle = ResourceManager.Instance.Load("icon_circle_deco", ResourceType.Common);
            IsSelectEnable = false;
        }
        m_ClosePosY = posy;
        m_OpenPosY = posy + 38;
        Unit_base_y = m_ClosePosY;
        Unit_base_x = posx;
        Balloon_text = "・・・";
        Hands_y = 70;
        m_CharaIndex = index;
        m_Dead = 0;
        m_Balloon2 = BALLOON2_TYPE.READY;
        m_HpBar = HP_SPRITE_INDEX.HP_SPRITE_100;
        Icon_heart = InGamePlayerParty.Instance.m_IconHeart[m_Dead];
        Unit_hp_bar = InGamePlayerParty.Instance.m_HpBar[(int)m_HpBar];
        Balloon2_image = InGamePlayerParty.Instance.m_Balloon2[(int)m_Balloon2];
        if (balloon_active == true)
        {
            Balloon2_color = Color.white;
        }
        else
        {
            Balloon2_color = Balloon_gray;
        }
        UnitIconScale = 1;
        UnitIconColor = Color.white;
        NonAS_active = false;
        UnitBallonY = 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リソース破棄
	*/
    //----------------------------------------------------------------------------
    public void DestroyTexture()
    {
    }

    void Awake()
    {
        gameObject.GetComponent<M4uContextRoot>().Context = this;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    void Start()
    {
        m_WaitTime = 0;
        m_SkillWindow = false;
        m_CharaAilmentIndex = -1;
        m_CharaAilmentTime = CHARA_AILMENT_TIME;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    void Update()
    {
        if (m_PartyChara == null)
        {
            return;
        }

        if (m_PartyChara.m_CharaMasterDataParam == null)
        {
            return;
        }

        if (BattleParam.m_PlayerParty != null)
        {
            m_PreBalloon2 = m_Balloon2;
            // 保存しているHPと現在値が異なっている場合、HP表示更新。
            if (m_PartyTotalHPPrv != BattleParam.m_PlayerParty.getDispHP().getValue(m_CharaIndex) ||
                m_PartyTotalHPMaxPrv != BattleParam.m_PlayerParty.m_HPMax.getValue(m_CharaIndex) ||
                BattleParam.m_PlayerParty.getDispDamageValue().getValue(m_CharaIndex) > 0 ||
                BattleParam.m_PlayerParty.getDispRecoveryValue().getValue(m_CharaIndex) > 0
            )
            {
                if (m_PartyTotalHPPrv >= 0)
                {
                    Hp_num = BattleParam.m_PlayerParty.getDispHP().getValue(m_CharaIndex).ToString();
                    m_HpNumAnimation.Stop();
                    m_HpNumAnimation.Play("hp_num");
                }
                m_PartyTotalHPPrv = BattleParam.m_PlayerParty.getDispHP().getValue(m_CharaIndex);
                m_PartyTotalHPMaxPrv = BattleParam.m_PlayerParty.m_HPMax.getValue(m_CharaIndex);

                //----------------------------------------
                // HP残量に合わせてUIのゲージ更新
                //----------------------------------------
                {
                    float fHPRate = 0.0f;
                    if (m_PartyTotalHPMaxPrv > 0.0f)
                    {
                        fHPRate = (float)m_PartyTotalHPPrv / (float)m_PartyTotalHPMaxPrv;
                        if (fHPRate > 0 && fHPRate < 0.03f)
                        {
                            fHPRate = 0.03f;
                        }
                    }

                    m_UnitHPGauge.fillAmount = fHPRate;
                    //                    Unit_hp_width = 63.0f * fHPRate;
                    if (fHPRate == 1)
                    {
                        m_HpBar = HP_SPRITE_INDEX.HP_SPRITE_100;
                    }
                    else if (fHPRate >= 0.5f)
                    {
                        m_HpBar = HP_SPRITE_INDEX.HP_SPRITE_50_99;
                    }
                    else
                    {
                        m_HpBar = HP_SPRITE_INDEX.HP_SPRITE_0_49;
                    }
                    Unit_hp_bar = InGamePlayerParty.Instance.m_HpBar[(int)m_HpBar];
                }

                checkDead(m_PartyTotalHPPrv);
            }
            //----------------------------------------
            // スキルターン数のゲージ更新チェック
            //----------------------------------------
            {
                if (m_PartyChara.m_CharaMasterDataParam.skill_limitbreak != 0)
                {
                    if (m_PartyTotalSkillPrv != m_PartyChara.GetTrunToLimitBreak() ||
                        m_PartyTotalSkillMaxPrv != m_PartyChara.GetMaxTurn())
                    {
                        m_PartyTotalSkillPrv = m_PartyChara.GetTrunToLimitBreak();
                        m_PartyTotalSkillMaxPrv = m_PartyChara.GetMaxTurn();


                        //----------------------------------------
                        // スキルターン数に合わせてUIのゲージ更新
                        //----------------------------------------
                        {
                            float rate = 1.0f - ((float)m_PartyTotalSkillPrv / (float)m_PartyTotalSkillMaxPrv);
                            m_UnitSkillGauge.fillAmount = rate;
                        }
                        {
                            if (m_PartyTotalSkillPrv != 0)
                            {
                                Balloon_text = string.Format(GameTextUtil.GetText("unit_skill_turn_battle"), m_PartyTotalSkillPrv);
                                Balloon2_active = false;
                                if (m_ASTurnInfoOpen == false)
                                {
                                    Hands_y = 70;
                                    Balloon1_active = false;
                                }
                                else
                                {
                                    if (Balloon1_active == false)
                                    {
                                        Balloon1_active = true;
                                    }
                                }
                            }
                            if (m_PartyTotalSkillPrv == 0
                            || m_Balloon2 != BALLOON2_TYPE.READY)
                            {
                                Balloon2_active = true;
                                Balloon1_active = false;
                                Hands_y = 118;
                                if (BattleSceneManager.Instance != null
                                && BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleActive == true)
                                {
                                    if (BattleSceneUtil.checkLimitBreak(BattleParam.m_PlayerParty, m_CharaIndex, BattleParam.m_EnemyParam, BattleParam.m_TargetEnemyCurrent) == false
                                    && m_Balloon2 == BALLOON2_TYPE.READY)
                                    {
                                        Balloon2_color = Balloon_gray;
                                    }
                                    else
                                    {
                                        Balloon2_color = Color.white;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_PartyTotalSkillPrv == 0)
                        {
                            if (Balloon2_active == true)    // 死んでるときとか非表示時は処理しない
                            {
                                if (BattleSceneManager.Instance != null
                                && BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleActive == true)    // 戦闘開始までは初期値または中断復帰時の色で表示
                                {
                                    bool check = BattleSceneUtil.checkLimitBreak(BattleParam.m_PlayerParty, m_CharaIndex, BattleParam.m_EnemyParam, BattleParam.m_TargetEnemyCurrent);
                                    if (check != SceneModeContinuousBattle.Instance.m_Balloon_active[(int)m_CharaIndex])
                                    {
                                        SceneModeContinuousBattle.Instance.m_Balloon_active[(int)m_CharaIndex] = check;
                                        if (check == false
                                        && m_Balloon2 == BALLOON2_TYPE.READY)
                                        {
                                            Balloon2_color = Balloon_gray;
                                        }
                                        else
                                        {
                                            Balloon2_color = Color.white;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // スキル発動状態じゃない
                            if (m_Balloon2 == BALLOON2_TYPE.READY)    // スキル封印じゃなければReadyを消す
                            {
                                Balloon2_active = false;
                                if (m_ASTurnInfoOpen == true
                                && Balloon1_active == false)
                                {
                                    Balloon1_active = true;
                                }
                            }
                        }
                    }
                }
            }
            //----------------------------------------
            // Hands数の更新チェック
            //----------------------------------------
            if (InGamePlayerParty.Instance.m_HandsView == true)
            {
                {
                    if (m_PartyHandsPrv != BattleParam.getPartyMemberHands(m_CharaIndex))
                    {
                        if (m_PartyHandsPrv < 0 && BattleParam.getPartyMemberHands(m_CharaIndex) == 0)
                        { }
                        else
                        {
                            m_PartyHandsPrv = BattleParam.getPartyMemberHands(m_CharaIndex);
                            {
                                setHands(m_PartyHandsPrv);
                                m_HandsAnimation.Stop();
                                m_HandsAnimation.Play("Hands");
                            }
                        }
                    }
                }
            }
            else
            {
                m_PartyHandsPrv = -1;
                {
                    if (Hands_active == true)
                    {
                        Hands_active = false;
                    }
                }
            }
            //----------------------------------------
            // 状態以上チェック
            //----------------------------------------
            {
                AilmentUpdate();
            }

            bool wrk_not_heal_active = false;
            bool wrk_poison_active = false;
            if (m_CharaAilmentIndex >= 0)
            {
                bool change_index = false;
                if (m_CharaAilmentTurn[m_CharaAilmentIndex] > 0)
                {
                    m_CharaAilmentTime -= Time.deltaTime;
                    if (m_CharaAilmentTime <= 0)
                    {
                        m_CharaAilmentTime += CHARA_AILMENT_TIME;
                        change_index = true;
                    }
                }
                else
                {
                    change_index = true;
                }
                if (change_index == true)
                {
                    int index = m_CharaAilmentIndex;
                    do
                    {
                        ++index;
                        if (index >= (int)CharaAilmentType.Max) index = 0;
                        if (m_CharaAilmentTurn[index] > 0)
                        {
                            break;
                        }
                    } while (index != m_CharaAilmentIndex);
                    if (m_CharaAilmentIndex != index)
                    {
                        m_CharaAilmentIndex = index;
                    }
                    else
                    {
                        if (m_CharaAilmentTurn[(int)m_CharaAilmentIndex] <= 0)
                        {
                            m_CharaAilmentIndex = -1;
                        }
                    }
                }
                switch ((CharaAilmentType)m_CharaAilmentIndex)
                {
                    case CharaAilmentType.NotHeal:
                        {
                            wrk_not_heal_active = true;
                            Not_heal_count = m_CharaAilmentTurn[m_CharaAilmentIndex].ToString("00");
                        }
                        break;
                    case CharaAilmentType.Poison:
                        {
                            wrk_poison_active = true;
                            Poison_count = m_CharaAilmentTurn[m_CharaAilmentIndex].ToString("00");
                        }
                        break;
                    default:
                        break;
                }
            }
            Not_heal_active = wrk_not_heal_active;
            Poison_active = wrk_poison_active;
        }

        //----------------------------------------
        // Hands非表示チェック
        //----------------------------------------
        if (m_WaitTime > 0)
        {
            m_WaitTime -= Time.deltaTime;
            if (m_WaitTime <= 0)
            {
                m_WaitTime = 0;
                Hands_active = false;
            }
        }

        // 「Ready」アニメーション制御
        if (Balloon2_active
        && (InGameMenuManagerQuest2.Instance != null && InGameMenuManagerQuest2.Instance.isSkillMenuActive == false))
        {
            bool animStop = false;
            if (BattleParam.getBattlePhase() == BattleParam.BattlePhase.INPUT_HANDLING
                || BattleParam.isCountDown()
                || (m_PartyTotalSkillPrv != 0 && m_Balloon2 != BALLOON2_TYPE.READY)
                || m_PartyChara.m_CharaMasterDataParam.skill_limitbreak == 0
            )
            {
                // プレイヤー操作中・カウントダウン中はアニメーションしない。「Ready」表示状態で止める。
                animStop = true;
            }
            setReadyAnimation(animStop);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void setHands(int hands)
    {
        if (hands < 0)
        {
            Hands_active = false;
        }
        else
        {
            Hands_active = true;
            int index_base = 10 * ((int)m_PartyChara.m_CharaMasterDataParam.element - 1);
            Hands_num1 = InGamePlayerParty.Instance.m_HandsNumSprite[index_base + (hands % 10)];
            Hands_num2 = InGamePlayerParty.Instance.m_HandsNumSprite[index_base + (hands / 10)];
            if (hands == 0)
            {
                m_WaitTime = 0.5f;
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void openASTurnInfo()
    {
        Hands_y = 118;
        if (Empty_active == false)
        {
            if (m_PartyTotalSkillPrv != 0)
            {
                Balloon1_active = true;
            }
        }
        m_ASTurnInfoOpen = true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void openNormalSkillInfo()
    {
        Unit_base_y = m_OpenPosY;
        Skill_tips_active = true;
        m_NormalSkillInfoOpen = true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void closeASTurnInfo()
    {
        if (Balloon2_active == false)
            Hands_y = 70;
        else
            Hands_y = 118;
        if (Empty_active == false)
        {
            Balloon1_active = false;
        }
        m_ASTurnInfoOpen = false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void closeNormalSkillInfo()
    {
        Unit_base_y = m_ClosePosY;
        Skill_tips_active = false;
        m_NormalSkillInfoOpen = false;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    private void setSkillCost()
    {
        Skill_color1_1 = m_Skill_cost[0];
        Skill_color1_2 = m_Skill_cost[1];
        Skill_color1_3 = m_Skill_cost[2];
        Skill_color1_4 = m_Skill_cost[3];
        Skill_color1_5 = m_Skill_cost[4];
        Skill_color2_1 = m_Skill_cost[5];
        Skill_color2_2 = m_Skill_cost[6];
        Skill_color2_3 = m_Skill_cost[7];
        Skill_color2_4 = m_Skill_cost[8];
        Skill_color2_5 = m_Skill_cost[9];
        Skill_col1_1_active = m_Skill_cost_active[0];
        Skill_col1_2_active = m_Skill_cost_active[1];
        Skill_col1_3_active = m_Skill_cost_active[2];
        Skill_col1_4_active = m_Skill_cost_active[3];
        Skill_col1_5_active = m_Skill_cost_active[4];
        Skill_col2_1_active = m_Skill_cost_active[5];
        Skill_col2_2_active = m_Skill_cost_active[6];
        Skill_col2_3_active = m_Skill_cost_active[7];
        Skill_col2_4_active = m_Skill_cost_active[8];
        Skill_col2_5_active = m_Skill_cost_active[9];
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：状態異常更新処理
	*/
    //----------------------------------------------------------------------------
    private void AilmentUpdate()
    {
        StatusAilmentChara ailmentChara = BattleParam.m_PlayerParty.m_Ailments.getAilment(m_CharaIndex);
        bool wrk_attack_active = false;
        bool wrk_defense_active = false;
        bool wrk_dark_active = false;

        ailmentTurnListClear();
        if (Dead_active == false)
        {
            InGamePlayerParty.Instance.m_InGamePartyManager.ailmentTurnListClear();
        }
        else
        {
            m_CharaAilmentIndex = -1;
        }
        HP_SPRITE_INDEX hpBar = m_HpBar;
        m_Balloon2 = BALLOON2_TYPE.READY;

        if (Dead_active == false)
        {
            float atk_rate;
            if (ailmentChara.GetOffenceRateTotal(out atk_rate, m_PartyChara.m_CharaMasterDataParam.element, m_PartyChara.m_CharaMasterDataParam.kind, m_PartyChara.m_CharaMasterDataParam.sub_kind))
            {
                wrk_attack_active = true;
                if (atk_rate >= 1.0f)
                {
                    Attack_icon = InGamePlayerParty.Instance.m_AilmentIcon[(int)InGamePlayerParty.AilmentType.ATTACK_UP];
                }
                else
                {
                    Attack_icon = InGamePlayerParty.Instance.m_AilmentIcon[(int)InGamePlayerParty.AilmentType.ATTACK_DOWN];
                }
            }

            float def_damage_rate;
            if (ailmentChara.GetDefenceDamageRateMaxForPlayer(out def_damage_rate))
            {
                wrk_defense_active = true;
                if (def_damage_rate <= 1.0f)
                {
                    // 被ダメージが小さくなるので防御アップ表示
                    Defense_icon = InGamePlayerParty.Instance.m_AilmentIcon[(int)InGamePlayerParty.AilmentType.DEFENCE_UP];
                }
                else
                {
                    // 被ダメージが大きくなるので防御ダウン表示
                    Defense_icon = InGamePlayerParty.Instance.m_AilmentIcon[(int)InGamePlayerParty.AilmentType.DEFENCE_DOWN];
                }
            }
        }

        for (int i = 0; i < (int)MasterDataDefineLabel.AilmentType.MAX; i++)
        {
            if (ailmentChara.IsHavingAilment((MasterDataDefineLabel.AilmentType)i) == false)
            {
                continue;
            }
            StatusAilment status_ailment = null;
            for (int n = 0; n < ailmentChara.GetAilmentCount(); n++)
            {
                status_ailment = ailmentChara.GetAilment(n);
                if (status_ailment == null)
                {
                    continue;
                }

                MasterDataDefineLabel.AilmentType type = status_ailment.nType;
                if (type != (MasterDataDefineLabel.AilmentType)i)
                {
                    continue;
                }
                break;
            }
            MasterDataDefineLabel.AilmentGroup group = StatusAilment.getAilmentGroupFromAilmentType((MasterDataDefineLabel.AilmentType)i);
            switch (group)
            {
                case MasterDataDefineLabel.AilmentGroup.ATK_UP:
                case MasterDataDefineLabel.AilmentGroup.ATK_DOWN:
                case MasterDataDefineLabel.AilmentGroup.BARRIER:
                case MasterDataDefineLabel.AilmentGroup.DAMAGE_TAKEN_UP:
                    break;
                case MasterDataDefineLabel.AilmentGroup.DEF_DOWN:
                    {
                        if (Dead_active == false)
                        {
                            wrk_defense_active = true;
                            Defense_icon = InGamePlayerParty.Instance.m_AilmentIcon[(int)InGamePlayerParty.AilmentType.DEFENCE_DOWN];
                        }
                    }
                    break;
                case MasterDataDefineLabel.AilmentGroup.POISON:
                    {
                        if (Dead_active == false)
                        {
                            hpBar = HP_SPRITE_INDEX.HP_SPRITE_POISON;
                            setAilmentTurn(CharaAilmentType.Poison, status_ailment.nLife);
                        }
                    }
                    break;
                case MasterDataDefineLabel.AilmentGroup.NON_RECOVERY:
                    {
                        if (Dead_active == false)
                        {
                            setAilmentTurn(CharaAilmentType.NotHeal, status_ailment.nLife);
                        }
                    }
                    break;
                case MasterDataDefineLabel.AilmentGroup.SEALED:
                    {
                        if (m_PartyTotalSkillPrv <= 0)
                        {
                            m_Balloon2 = BALLOON2_TYPE.READY_MUTE;
                        }
                        else
                        {
                            m_Balloon2 = BALLOON2_TYPE.HUKIDASHI_MUTE;
                            if (m_PreBalloon2 != BALLOON2_TYPE.HUKIDASHI_MUTE
                            && m_ReadyAnimation.isPlaying == false)
                            {
                                m_ReadyAnimation.gameObject.GetComponent<Image>().enabled = true;
                            }
                        }
                        Balloon2_active = true;
                        Balloon1_active = false;
                        Balloon2_color = Color.white;
                        Hands_y = 118;
                    }
                    break;
                case MasterDataDefineLabel.AilmentGroup.DARK:
                    {
                        if (Dead_active == false)
                        {
                            wrk_dark_active = true;
                            Dark_count = status_ailment.nLife.ToString("00");
                        }
                    }
                    break;
                case MasterDataDefineLabel.AilmentGroup.TIME_PLUS:
                    {
                        InGamePlayerParty.Instance.m_InGamePartyManager.setAilmentTurn(InGamePartyManager.PartyAilmentType.TimePlus, status_ailment.nLife);
                    }
                    break;
                case MasterDataDefineLabel.AilmentGroup.DELAY:
                case MasterDataDefineLabel.AilmentGroup.TIME_MINUS:
                    {
                        InGamePlayerParty.Instance.m_InGamePartyManager.setAilmentTurn(InGamePartyManager.PartyAilmentType.TimeMinus, status_ailment.nLife);
                    }
                    break;
                case MasterDataDefineLabel.AilmentGroup.HAND_CHANCE:
                    {
                        InGamePlayerParty.Instance.m_InGamePartyManager.setAilmentTurn(InGamePartyManager.PartyAilmentType.HandChance, status_ailment.nLife);
                    }
                    break;
                default:
                    break;
            }
        }
        Unit_hp_bar = InGamePlayerParty.Instance.m_HpBar[(int)hpBar];
        Balloon2_image = InGamePlayerParty.Instance.m_Balloon2[(int)m_Balloon2];
        if (m_PreBalloon2 != m_Balloon2 &&
            m_Balloon2 == BALLOON2_TYPE.READY &&
            (Dead_active == true ||
             BattleSceneUtil.checkLimitBreak(BattleParam.m_PlayerParty, m_CharaIndex, BattleParam.m_EnemyParam, BattleParam.m_TargetEnemyCurrent) == false))
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(m_CharaIndex, CharaParty.CharaCondition.SKILL_LEADER);
            if (chara_once.m_CharaMasterDataParam.skill_limitbreak != 0)
            {
                Balloon2_color = Balloon_gray;
            }
            else
            {
                Balloon2_active = false;
                if (m_ASTurnInfoOpen == true
                && Balloon1_active == false)
                {
                    Balloon1_active = true;
                }
            }
        }

        Attack_active = wrk_attack_active;
        Defense_active = wrk_defense_active;
        Dark_active = wrk_dark_active;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：キャラタッチ時間チェック
	*/
    //----------------------------------------------------------------------------
    public void OnClick()
    {
        // メニューオープンタイミングのチェック
        bool isOpen = InGameUtil.ChkInGameOpenWindowTiming();
        if (isOpen == false)
        {
            return;
        }
        if (IsSetUnit() == false)
        {
            return;
        }
        if (m_CharaIndex == GlobalDefine.PartyCharaIndex.ERROR)
        {
            return;
        }
        if (Dead_active == true)
        {
            return;
        }
        if (m_PartyChara.m_CharaMasterDataParam.skill_limitbreak == 0)
        {
            return;
        }
        // チュートリアル中の発動禁止処理.
        if (BattleParam.IsTutorial())
        {
            if (BattleSceneManager.Instance.isTutorialForbidLimitBreak(m_CharaIndex))
            {
                return;
            }
        }

        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        if (cOption.m_OptionConfirmAS == (int)LocalSaveDefine.OptionConfirmAS.ON)
        {
            // スキル発動確認ウィンドウ表示
            if (InGameMenuManagerQuest2.Instance != null)
            {
                InGameMenuManagerQuest2.Instance.OpenSkillMenu(m_CharaIndex, SceneModeContinuousBattle.Instance.m_PlayerPartyChara[(int)m_CharaIndex].m_CharaMasterDataParam.fix_id, m_PartyTotalSkillPrv, m_Balloon2 != BALLOON2_TYPE.READY);
            }
        }
        else
        {
            // リミットブレイクスキルを即時発動
            BattleParam.RequestLBS(m_CharaIndex);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：キャラタッチ時間チェック
	*/
    //----------------------------------------------------------------------------
    public void OnLongPress()
    {
        // メニューオープンタイミングのチェック
        bool isOpen = InGameUtil.ChkInGameOpenWindowTiming();
        if (isOpen == false)
        {
            return;
        }
        if (IsSetUnit() == false)
        {
            return;
        }
        if (m_CharaIndex == GlobalDefine.PartyCharaIndex.ERROR)
        {
            return;
        }
        if (InGameMenuManagerQuest2.Instance != null)
        {
            if (InGameMenuManagerQuest2.Instance.isSkillMenuActive == true)
            {
                return;
            }
        }
        // チュートリアル中の禁止処理
        if (BattleParam.IsTutorial())
        {
            if (BattleSceneManager.Instance.isTutorialEnableUnitInfoWindow() == false)
            {
                return;
            }
        }

        m_SkillWindow = true;
        SceneModeContinuousBattle.Instance.SkillWindowOpen(m_CharaIndex);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：キャラ詳細ウィンドウ表示中か？
	*/
    //----------------------------------------------------------------------------
    public bool isSkillWindow()
    {
        bool ret = m_SkillWindow;
        m_SkillWindow = false;
        return ret;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：キャラ死亡チェック
	*/
    //----------------------------------------------------------------------------
    public void checkDead(int hp)
    {
        if (hp == 0)
        {
            Dead_active = true;
            Icon_heart = InGamePlayerParty.Instance.m_IconHeart[1];
            if (Balloon2_active == true)
            {
                Balloon2_color = Balloon_gray;
            }
        }
        else
        {
            if (Dead_active == true)
            {
                Dead_active = false;
                Icon_heart = InGamePlayerParty.Instance.m_IconHeart[0];
                if (Balloon2_active == true)
                {
                    if (BattleSceneUtil.checkLimitBreak(BattleParam.m_PlayerParty, (GlobalDefine.PartyCharaIndex)m_CharaIndex, BattleParam.m_EnemyParam, BattleParam.m_TargetEnemyCurrent) == true)
                        Balloon2_color = Color.white;
                    else
                        Balloon2_color = Balloon_gray;
                }
            }
        }
    }

    private void ailmentTurnListClear()
    {
        for (int i = 0; i < (int)CharaAilmentType.Max; ++i)
        {
            m_CharaAilmentTurn[i] = 0;
        }
    }

    private void setAilmentTurn(CharaAilmentType type, int turn)
    {
        m_CharaAilmentTurn[(int)type] = turn;
        if (m_CharaAilmentIndex < 0)
        {
            m_CharaAilmentIndex = (int)type;
        }
    }

    public void setScale(float scale)
    {
        UnitIconScale = scale;
    }

    public void setReadyAnimation(bool stop)
    {
        if (stop == true)
        {
            // プレイヤー操作中・カウントダウン中はアニメーションしない。「Ready」表示状態で止める。
            m_ReadyAnimation.wrapMode = WrapMode.Once;
        }
        else
        {
            if (m_ReadyAnimation.isPlaying == false)
            {
                m_ReadyAnimation.wrapMode = WrapMode.Loop;
                m_ReadyAnimation["Ready"].time = SceneModeContinuousBattle.Instance.m_ReadyAnimation["Ready"].time;
                m_ReadyAnimation.Play("Ready");
            }
        }
    }

    public bool IsSkillActivate()
    {
        if (m_PartyTotalSkillPrv != 0)
        {
            return false;
        }
        return true;
    }

    public void CheckASMask(bool sw)
    {
        if (IsSetUnit() == false)
        {
            return;
        }
        if (m_PartyChara.m_CharaMasterDataParam.skill_limitbreak != 0)
        {
            return;
        }
        if (Dead_active == true)
        {
            return;
        }


        if (sw == true)
        {
            NonAS_active = true;
        }
        else
        {
            NonAS_active = false;
        }
    }

    public bool IsSetUnit()
    {
        if (m_PartyChara == null)
        {
            return false;
        }
        if (m_PartyChara.m_bHasCharaMasterDataParam == false)
        {
            return false;
        }
        return true;
    }

    public void SetUnitRoot(bool select)
    {
        if (select == false)
        {
            m_UnitObject.SetParent(m_UnitRoot, false);
            UnitBallonY = 0;
        }
        else
        {
            m_UnitObject.SetParent(m_UnitSelectRoot, false);
            UnitBallonY = 32;
        }
    }
}
