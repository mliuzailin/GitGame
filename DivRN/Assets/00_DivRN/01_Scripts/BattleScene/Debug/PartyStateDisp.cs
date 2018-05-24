using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PartyStateDisp : MonoBehaviour
{
    public GameObject m_PartyHpObject = null;
    public GameObject m_PartyHpDamageLocale = null;
    public GameObject m_PartySpObject = null;
    public GameObject[] m_MemberObject = new GameObject[0];

    public Sprite[] m_SkillCostElements = new Sprite[(int)MasterDataDefineLabel.ElementType.MAX];

    private int m_PartyHp = -1;
    private int m_PartyHpMax = -1;
    private int m_PartySp = -1;
    private int m_PartySpMax = -1;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (BattleParam.m_PlayerParty != null && BattleParam.isActiveBattle())
        {
            BattleSceneUtil.MultiInt damage_value = BattleParam.m_PlayerParty.getDispDamageValue();
            BattleSceneUtil.MultiInt heal_value = BattleParam.m_PlayerParty.getDispRecoveryValue();

            if (BattleParam.m_PlayerParty.m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MAX) != m_PartyHp
                || BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MAX) != m_PartyHpMax
                )
            {
                m_PartyHp = BattleParam.m_PlayerParty.m_HPCurrent.getValue(GlobalDefine.PartyCharaIndex.MAX);
                m_PartyHpMax = BattleParam.m_PlayerParty.m_HPMax.getValue(GlobalDefine.PartyCharaIndex.MAX);
                if (m_PartyHpObject != null)
                {
                    TextMesh text_mesh = m_PartyHpObject.GetComponent<TextMesh>();
                    if (text_mesh != null)
                    {
                        text_mesh.text = "HP:" + m_PartyHp.ToString() + "/" + m_PartyHpMax.ToString();
                    }
                }
            }

            if (damage_value != null)
            {
                int hp_delta = damage_value.getValue(GlobalDefine.PartyCharaIndex.MAX);
                if (hp_delta > 0)
                {
                    DrawDamageManager.showDamage(m_PartyHpDamageLocale.transform, hp_delta, EDAMAGE_TYPE.eDAMAGE_TYPE_WEEK);
                }
            }

            if (heal_value != null)
            {
                int hp_delta = heal_value.getValue(GlobalDefine.PartyCharaIndex.MAX);
                if (hp_delta > 0)
                {
                    DrawDamageManager.showDamage(m_PartyHpDamageLocale.transform, hp_delta, EDAMAGE_TYPE.eDAMAGE_TYPE_HEAL);
                    EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_03, new Vector3(2.0f * 0.05f, -0.9609375f * 0.05f, 0.0f), Vector3.zero, m_PartyHpObject.transform, null, 0.15f);
                }
            }

            if (BattleParam.m_PlayerParty.m_PartyTotalSP != m_PartySp
                || BattleParam.m_PlayerParty.m_PartyTotalSPMax != m_PartySpMax
                )
            {
                int old_sp = m_PartySp;
                m_PartySp = BattleParam.m_PlayerParty.m_PartyTotalSP;
                m_PartySpMax = BattleParam.m_PlayerParty.m_PartyTotalSPMax;
                if (m_PartySpObject != null)
                {
                    TextMesh text_mesh = m_PartySpObject.GetComponent<TextMesh>();
                    if (text_mesh != null)
                    {
                        text_mesh.text = "SP:" + m_PartySp.ToString() + "/" + m_PartySpMax.ToString();
                    }
                    if (m_PartySp > old_sp)
                    {
                        EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_SP, new Vector3(2.0f * 0.05f, -0.4f * 0.05f, 0.0f), Vector3.zero, m_PartySpObject.transform, null, 0.125f);
                    }
                }
            }

            // パーティ状態異常
            {
                Transform ailment_object = transform.Find("PartyAilment");
                if (ailment_object != null)
                {
                    InGameAilmentIcon ailment_icon = ailment_object.GetComponent<InGameAilmentIcon>();
                    if (ailment_icon != null)
                    {
                        StatusAilmentChara ailment_info = BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX);
                        ailment_icon.SetStatus(ailment_info);
                    }
                }
            }

            bool is_control = false;
            if (BattleParam.getBattlePhase() == BattleParam.BattlePhase.INPUT)
            {
                is_control = true;
            }

            // ヒーロースキル発動ボタン
            {
                Transform hero_skill_button_trans = transform.Find("Canvas/ButtonHeroSkill");
                if (hero_skill_button_trans != null)
                {
                    Button button = hero_skill_button_trans.GetComponent<Button>();
                    if (button != null)
                    {
                        bool is_button_enable = (is_control && BattleParam.m_PlayerParty.m_BattleHero.checkHeroSkillTurn());
                        button.interactable = is_button_enable;

                        Transform btn_txt = hero_skill_button_trans.Find("Text");
                        if (btn_txt != null)
                        {
                            Text txt = btn_txt.GetComponent<Text>();
                            if (txt != null)
                            {
                                txt.text = BattleParam.m_PlayerParty.m_BattleHero.getSkillTurn().ToString() + "/HERO";
                            }
                        }
                    }
                }
            }

            GlobalDefine.PartyCharaIndex provoke_target = BattleParam.m_PlayerParty._getProvokeTarget();
            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
            {
                string chara_name = "";
                int chara_id = 0;
                int link_chara_id = 0;
                string hp_text = "";
                string skill_name = "";
                bool is_active_skill_button = false;

                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
                bool is_alive = false;
                if (chara_once != null)
                {
                    MasterDataParamChara chara_master = chara_once.m_CharaMasterDataParam;

                    int hp = BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx);
                    float hp_rate = hp / (float)BattleParam.m_PlayerParty.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx);

                    string hp_color = "white";
                    if (hp_rate >= 0.5f)
                    {
                        hp_color = "white";
                        is_alive = true;
                    }
                    else
                    if (hp_rate >= 0.2f)
                    {
                        hp_color = "yellow";
                        is_alive = true;
                    }
                    else
                    if (hp_rate > 0.0f)
                    {
                        hp_color = "red";
                        is_alive = true;
                    }
                    else
                    {
                        hp_color = "#808080";
                    }

                    // キャラ名
                    chara_name = "<color=" + hp_color + ">" + chara_master.name + "</color>";

                    // キャライメージ
                    chara_id = (int)chara_master.fix_id;

                    // リンクキャライメージ
                    if (chara_once.m_LinkParam != null)
                    {
                        link_chara_id = (int)chara_once.m_LinkParam.m_CharaID;
                    }

                    int atk_percent = (int)(InGameUtil.getCharaAttakPowScale(chara_once, BattleParam.m_PlayerParty.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx)) * 100.0f);
                    int hp_percent = (int)(BattleParam.m_PlayerParty.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx) / (float)BattleParam.m_PlayerParty.m_HPBase.getValue((GlobalDefine.PartyCharaIndex)idx) * 100.0f);

                    // キャラ個別ＨＰ
                    hp_text = "<color=" + hp_color + ">"
                        + BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx)
                        + "/" + BattleParam.m_PlayerParty.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx)
                        + "</color>";

                    if (BattleParam.IsKobetsuHP)
                    {
                        hp_text += " <color=#a0a0a0>HP" + hp_percent.ToString() + "%\n";
                        if (BattleParam.m_PlayerParty.m_Hate_ProvokeTurn.getValue((GlobalDefine.PartyCharaIndex)idx) > 0)
                        {
                            if (provoke_target == (GlobalDefine.PartyCharaIndex)idx)
                            {
                                hp_text += "[挑発]:";
                            }
                            else
                            {
                                hp_text += "挑発:";
                            }
                            hp_text += BattleParam.m_PlayerParty.m_Hate_ProvokeTurn.getValue((GlobalDefine.PartyCharaIndex)idx);
                        }
                        else
                        {
                            hp_text += "Hate:" + BattleParam.m_PlayerParty.m_Hate.getValue((GlobalDefine.PartyCharaIndex)idx);
                        }

                        hp_text += " ATK" + atk_percent.ToString() + "%</color>";
                    }

                    // スキルターン
                    int skill_turn = chara_once.GetTrunToLimitBreak();
                    MasterDataSkillLimitBreak skill_limit_break = BattleParam.m_MasterDataCache.useSkillLimitBreak(chara_master.skill_limitbreak);
                    if (skill_limit_break != null)
                    {
                        skill_name = skill_turn.ToString() + "/" + skill_limit_break.name;
                    }

                    // ボタンのアクティブ・非アクティブ
                    {
                        is_active_skill_button = is_control && BattleParam.IsEnableLBS((GlobalDefine.PartyCharaIndex)idx);
                    }
                }

                // 表示
                {
                    // キャラ名
                    {
                        Transform chara_name_trans = m_MemberObject[idx].transform.Find("CharaName");
                        if (chara_name_trans != null)
                        {
                            Text text_component = chara_name_trans.GetComponent<Text>();
                            if (text_component != null)
                            {
                                text_component.text = chara_name;
                            }
                        }
                    }
                }

                // スキル発動条件表示
                {
                    {
                        MasterDataDefineLabel.ElementType[,] skill_costs = new MasterDataDefineLabel.ElementType[2, 5];

                        if (is_alive)
                        {
                            MasterDataParamChara chara_master = chara_once.m_CharaMasterDataParam;
                            if (chara_master.skill_active0 != 0)
                            {
                                MasterDataSkillActive skill_active0 = BattleParam.m_MasterDataCache.useSkillActive(chara_master.skill_active0);
                                if (skill_active0 != null)
                                {
                                    skill_costs[0, 0] = skill_active0.cost1;
                                    skill_costs[0, 1] = skill_active0.cost2;
                                    skill_costs[0, 2] = skill_active0.cost3;
                                    skill_costs[0, 3] = skill_active0.cost4;
                                    skill_costs[0, 4] = skill_active0.cost5;
                                }
                            }
                            if (chara_master.skill_active1 != 0)
                            {
                                MasterDataSkillActive skill_active1 = BattleParam.m_MasterDataCache.useSkillActive(chara_master.skill_active1);
                                if (skill_active1 != null)
                                {
                                    skill_costs[1, 0] = skill_active1.cost1;
                                    skill_costs[1, 1] = skill_active1.cost2;
                                    skill_costs[1, 2] = skill_active1.cost3;
                                    skill_costs[1, 3] = skill_active1.cost4;
                                    skill_costs[1, 4] = skill_active1.cost5;
                                }
                            }
                        }

                        for (int skill_idx = 0; skill_idx < 2; skill_idx++)
                        {
                            for (int cost_idx = 0; cost_idx < 5; cost_idx++)
                            {
                                Transform cost_trans = m_MemberObject[idx].transform.Find("SkillInfo/Skill" + skill_idx.ToString() + cost_idx.ToString());
                                if (cost_trans != null)
                                {
                                    MasterDataDefineLabel.ElementType element_type = skill_costs[skill_idx, cost_idx];
                                    if (element_type != MasterDataDefineLabel.ElementType.NONE)
                                    {
                                        Image img = cost_trans.GetComponent<Image>();
                                        if (img != null)
                                        {
                                            img.sprite = m_SkillCostElements[(int)element_type];
                                            cost_trans.gameObject.SetActive(true);
                                        }
                                        else
                                        {
                                            cost_trans.gameObject.SetActive(false);
                                        }
                                    }
                                    else
                                    {
                                        cost_trans.gameObject.SetActive(false);
                                    }
                                }
                            }
                        }
                    }

                    // キャライメージ
                    {
                        Transform chara_image_trans = m_MemberObject[idx].transform.Find("Image");
                        if (chara_image_trans != null)
                        {
                            BattleCharaImageViewControl chara_view_control = chara_image_trans.GetComponent<BattleCharaImageViewControl>();
                            if (chara_view_control != null)
                            {
                                chara_view_control.setCharaID(chara_id, BattleCharaImageViewControl.ImageType.FACE);
                            }
                        }
                    }

                    // リンクキャライメージ
                    {
                        Transform chara_image_trans = m_MemberObject[idx].transform.Find("ImageLink");
                        if (chara_image_trans != null)
                        {
                            BattleCharaImageViewControl chara_view_control = chara_image_trans.GetComponent<BattleCharaImageViewControl>();
                            if (chara_view_control != null)
                            {
                                chara_view_control.setCharaID(link_chara_id, BattleCharaImageViewControl.ImageType.FACE);
                            }
                        }
                    }

                    // キャラ個別ＨＰ
                    {
                        Transform chara_hp_trans = m_MemberObject[idx].transform.Find("HP");
                        if (chara_hp_trans != null)
                        {
                            Text text_component = chara_hp_trans.GetComponent<Text>();
                            if (text_component != null)
                            {
                                text_component.text = hp_text;
                            }
                        }

                        if (damage_value != null)
                        {
                            int hp_delta = damage_value.getValue((GlobalDefine.PartyCharaIndex)idx);
                            if (hp_delta > 0)
                            {
                                DrawDamageManager.showDamage(chara_hp_trans, hp_delta, EDAMAGE_TYPE.eDAMAGE_TYPE_WEEK, 50.0f);
                            }
                        }

                        if (heal_value != null)
                        {
                            int hp_delta = heal_value.getValue((GlobalDefine.PartyCharaIndex)idx);
                            if (hp_delta > 0)
                            {
                                DrawDamageManager.showDamage(chara_hp_trans, hp_delta, EDAMAGE_TYPE.eDAMAGE_TYPE_HEAL, 50.0f);
                                EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_03, new Vector3(0.01975f, -0.0246875f, 0.0f), Vector3.zero, chara_hp_trans, null, 50.0f * 3.0f);
                            }
                        }
                    }

                    // スキルターン
                    Transform skill_turn_trans = m_MemberObject[idx].transform.Find("SkillButton");
                    if (skill_turn_trans != null)
                    {
                        Text text_component = skill_turn_trans.GetChild(0).GetComponent<Text>();
                        if (text_component != null)
                        {
                            text_component.text = skill_name;
                        }

                        // ボタンのアクティブ・非アクティブ
                        {
                            Button button = skill_turn_trans.GetComponent<Button>();
                            button.interactable = is_active_skill_button;
                        }
                    }
                }

                // 敵にターゲットされているかどうかの表示
                {
                    Transform lock_on_object = m_MemberObject[idx].transform.Find("ImageLockOn");
                    if (lock_on_object != null)
                    {
                        lock_on_object.gameObject.SetActive(BattleParam.m_EnemyToPlayerTarget == (GlobalDefine.PartyCharaIndex)idx);
                    }
                }

                // 状態異常
                if (chara_once != null)
                {
                    Transform ailment_object = m_MemberObject[idx].transform.Find("Ailment");
                    if (ailment_object != null)
                    {
                        InGameAilmentIcon ailment_icon = ailment_object.GetComponent<InGameAilmentIcon>();
                        if (ailment_icon != null)
                        {
                            StatusAilmentChara ailment_info = BattleParam.m_PlayerParty.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx);
                            ailment_icon.SetStatus(ailment_info);   //毎フレーム設定する必要はないがとりあえず設定しておく
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// リミットブレイクスキル発動ボタンが押された
    /// </summary>
    /// <param name="my_owner_object"></param>
    public void OnPushLimitBreakSkillButton(GameObject my_owner_object)
    {
        for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
        {
            GameObject wrk_button = m_MemberObject[idx];
            if (wrk_button == my_owner_object)
            {
                BattleParam.RequestLBS((GlobalDefine.PartyCharaIndex)idx);

                break;
            }
        }
    }

    /// <summary>
    /// ヒーロースキル（必殺技）発動ボタンが押された
    /// </summary>
    public void OnPushHeroSkillButton()
    {
        BattleParam.RequestLBS(GlobalDefine.PartyCharaIndex.HERO);
    }

    /// <summary>
    /// パーティのスキルターンを増加ボタンが押された
    /// </summary>
    public void OnPushSkillTurnPPButton()
    {
        // パーティのスキルターンを増加.
        for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
        {
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.SKILL_TURN1);
            if (chara_once != null)
            {
                chara_once.AddCharaLimitBreak(1);
            }
        }
    }
}
