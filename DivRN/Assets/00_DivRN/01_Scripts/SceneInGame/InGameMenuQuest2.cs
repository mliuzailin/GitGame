using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class InGameMenuQuest2 : M4uContextMonoBehaviour
{
    private bool m_IsUpdateOptionButtionView = true;

    M4uProperty<bool> menu_active = new M4uProperty<bool>();
    public bool Menu_active { get { return menu_active.Value; } set { menu_active.Value = value; } }

    M4uProperty<bool> quest_info_active = new M4uProperty<bool>();
    public bool Quest_info_active { get { return quest_info_active.Value; } set { quest_info_active.Value = value; } }

    M4uProperty<bool> option_active = new M4uProperty<bool>();
    public bool Option_active { get { return option_active.Value; } set { option_active.Value = value; } }

    M4uProperty<bool> retire_active = new M4uProperty<bool>();
    public bool Retire_active { get { return retire_active.Value; } set { retire_active.Value = value; } }

    M4uProperty<bool> back_active = new M4uProperty<bool>();
    public bool Back_active { get { return back_active.Value; } set { back_active.Value = value; } }

    M4uProperty<bool> back_button_enable = new M4uProperty<bool>();
    public bool Back_button_enable { get { return back_button_enable.Value2; } set { back_button_enable.Value2 = value; } }

    M4uProperty<Color> back_text_color = new M4uProperty<Color>();
    public Color Back_text_color { get { return back_text_color.Value2; } set { back_text_color.Value2 = value; } }

    M4uProperty<string> title_text = new M4uProperty<string>();
    public string Title_text { get { return title_text.Value; } set { title_text.Value = value; } }

    M4uProperty<string> wave_text = new M4uProperty<string>();
    public string Wave_text { get { return wave_text.Value; } set { wave_text.Value = value; } }

    M4uProperty<string> icon_rare01_num = new M4uProperty<string>();
    public string Icon_rare01_num { get { return icon_rare01_num.Value; } set { icon_rare01_num.Value = value; } }

    M4uProperty<string> icon_rare02_num = new M4uProperty<string>();
    public string Icon_rare02_num { get { return icon_rare02_num.Value; } set { icon_rare02_num.Value = value; } }

    M4uProperty<string> icon_rare03_num = new M4uProperty<string>();
    public string Icon_rare03_num { get { return icon_rare03_num.Value; } set { icon_rare03_num.Value = value; } }

    M4uProperty<string> icon_rare04_num = new M4uProperty<string>();
    public string Icon_rare04_num { get { return icon_rare04_num.Value; } set { icon_rare04_num.Value = value; } }

    M4uProperty<string> icon_rare05_num = new M4uProperty<string>();
    public string Icon_rare05_num { get { return icon_rare05_num.Value; } set { icon_rare05_num.Value = value; } }

    M4uProperty<string> icon_rare06_num = new M4uProperty<string>();
    public string Icon_rare06_num { get { return icon_rare06_num.Value; } set { icon_rare06_num.Value = value; } }

    M4uProperty<Sprite> bgm_switch = new M4uProperty<Sprite>();
    public Sprite Bgm_switch { get { return bgm_switch.Value; } set { bgm_switch.Value = value; } }

    M4uProperty<Sprite> se_switch = new M4uProperty<Sprite>();
    public Sprite Se_switch { get { return se_switch.Value; } set { se_switch.Value = value; } }

    M4uProperty<Sprite> guide_switch = new M4uProperty<Sprite>();
    public Sprite Guide_switch { get { return guide_switch.Value; } set { guide_switch.Value = value; } }

    M4uProperty<Sprite> tips_switch = new M4uProperty<Sprite>();
    public Sprite Tips_switch { get { return tips_switch.Value; } set { tips_switch.Value = value; } }

    M4uProperty<Sprite> voice_switch = new M4uProperty<Sprite>();
    public Sprite Voice_switch { get { return voice_switch.Value; } set { voice_switch.Value = value; } }

    M4uProperty<Sprite> speed_switch = new M4uProperty<Sprite>();
    public Sprite Speed_switch { get { return speed_switch.Value; } set { speed_switch.Value = value; } }

    M4uProperty<Sprite> skill_turn_switch = new M4uProperty<Sprite>();
    public Sprite Skill_turn_switch { get { return skill_turn_switch.Value; } set { skill_turn_switch.Value = value; } }

    M4uProperty<Sprite> confirm_as_switch = new M4uProperty<Sprite>();
    public Sprite Confirm_as_switch { get { return confirm_as_switch.Value; } set { confirm_as_switch.Value = value; } }

    M4uProperty<Sprite> skill_cost_switch = new M4uProperty<Sprite>();
    public Sprite Skill_cost_switch { get { return skill_cost_switch.Value; } set { skill_cost_switch.Value = value; } }

    M4uProperty<Sprite> battle_achieve_switch = new M4uProperty<Sprite>();
    public Sprite Battle_achieve_switch { get { return battle_achieve_switch.Value; } set { battle_achieve_switch.Value = value; } }

    M4uProperty<Sprite> quest_end_tips_switch = new M4uProperty<Sprite>();
    public Sprite Quest_end_tips_switch { get { return quest_end_tips_switch.Value; } set { quest_end_tips_switch.Value = value; } }

    M4uProperty<Sprite> auto_play_stop_boss_switch = new M4uProperty<Sprite>();
    public Sprite Auto_play_stop_boss_switch { get { return auto_play_stop_boss_switch.Value; } set { auto_play_stop_boss_switch.Value = value; } }

    M4uProperty<Sprite> auto_play_use_as_switch = new M4uProperty<Sprite>();
    public Sprite Auto_play_use_as_switch { get { return auto_play_use_as_switch.Value; } set { auto_play_use_as_switch.Value = value; } }

    M4uProperty<bool> bgm_switch_enable = new M4uProperty<bool>();
    public bool Bgm_switch_enable { get { return bgm_switch_enable.Value2; } set { bgm_switch_enable.Value2 = value; } }

    M4uProperty<bool> se_switch_enable = new M4uProperty<bool>();
    public bool Se_switch_enable { get { return se_switch_enable.Value2; } set { se_switch_enable.Value2 = value; } }

    M4uProperty<bool> guide_switch_enable = new M4uProperty<bool>();
    public bool Guide_switch_enable { get { return guide_switch_enable.Value2; } set { guide_switch_enable.Value2 = value; } }

    M4uProperty<bool> tips_switch_enable = new M4uProperty<bool>();
    public bool Tips_switch_enable { get { return tips_switch_enable.Value2; } set { tips_switch_enable.Value2 = value; } }

    M4uProperty<bool> voice_switch_enable = new M4uProperty<bool>();
    public bool Voice_switch_enable { get { return voice_switch_enable.Value2; } set { voice_switch_enable.Value2 = value; } }

    M4uProperty<bool> speed_switch_enable = new M4uProperty<bool>();
    public bool Speed_switch_enable { get { return speed_switch_enable.Value2; } set { speed_switch_enable.Value2 = value; } }

    M4uProperty<bool> skill_turn_switch_enable = new M4uProperty<bool>();
    public bool Skill_turn_switch_enable { get { return skill_turn_switch_enable.Value2; } set { skill_turn_switch_enable.Value2 = value; } }

    M4uProperty<bool> confirm_as_switch_enable = new M4uProperty<bool>();
    public bool Confirm_as_switch_enable { get { return confirm_as_switch_enable.Value2; } set { confirm_as_switch_enable.Value2 = value; } }

    M4uProperty<bool> skill_cost_switch_enable = new M4uProperty<bool>();
    public bool Skill_cost_switch_enable { get { return skill_cost_switch_enable.Value2; } set { skill_cost_switch_enable.Value2 = value; } }

    M4uProperty<bool> battle_achieve_switch_enable = new M4uProperty<bool>();
    public bool Battle_achieve_switch_enable { get { return battle_achieve_switch_enable.Value2; } set { battle_achieve_switch_enable.Value2 = value; } }

    M4uProperty<bool> quest_end_tips_switch_enable = new M4uProperty<bool>();
    public bool Quest_end_tips_switch_enable { get { return quest_end_tips_switch_enable.Value2; } set { quest_end_tips_switch_enable.Value2 = value; } }

    M4uProperty<bool> auto_play_stop_boss_switch_enable = new M4uProperty<bool>();
    public bool Auto_play_stop_boss_switch_enable { get { return auto_play_stop_boss_switch_enable.Value2; } set { auto_play_stop_boss_switch_enable.Value2 = value; } }

    M4uProperty<bool> auto_play_use_as_switch_enable = new M4uProperty<bool>();
    public bool Auto_play_use_as_switch_enable { get { return auto_play_use_as_switch_enable.Value2; } set { auto_play_use_as_switch_enable.Value2 = value; } }

    M4uProperty<List<UnitSkillContext>> skillList = new M4uProperty<List<UnitSkillContext>>(new List<UnitSkillContext>());
    public List<UnitSkillContext> SkillList { get { return skillList.Value; } set { skillList.Value = value; } }

    M4uProperty<bool> skill_menu_active = new M4uProperty<bool>();
    public bool Skill_menu_active { get { return skill_menu_active.Value; } set { skill_menu_active.Value = value; } }

    M4uProperty<string> retire_message = new M4uProperty<string>();
    public string Retire_message { get { return retire_message.Value; } set { retire_message.Value = value; } }

    M4uProperty<string> retire_yes = new M4uProperty<string>();
    public string Retire_yes { get { return retire_yes.Value; } set { retire_yes.Value = value; } }

    M4uProperty<string> retire_no = new M4uProperty<string>();
    public string Retire_no { get { return retire_no.Value; } set { retire_no.Value = value; } }

    M4uProperty<string> option_text = new M4uProperty<string>();
    public string Option_text { get { return option_text.Value; } set { option_text.Value = value; } }

    M4uProperty<string> retire_text = new M4uProperty<string>();
    public string Retire_text { get { return retire_text.Value; } set { retire_text.Value = value; } }

    M4uProperty<string> back_text = new M4uProperty<string>();
    public string Back_text { get { return back_text.Value; } set { back_text.Value = value; } }

    M4uProperty<string> option_bgm = new M4uProperty<string>();
    public string Option_bgm { get { return option_bgm.Value; } set { option_bgm.Value = value; } }

    M4uProperty<string> option_se = new M4uProperty<string>();
    public string Option_se { get { return option_se.Value; } set { option_se.Value = value; } }

    M4uProperty<string> option_guide = new M4uProperty<string>();
    public string Option_guide { get { return option_guide.Value; } set { option_guide.Value = value; } }

    M4uProperty<string> option_tips = new M4uProperty<string>();
    public string Option_tips { get { return option_tips.Value; } set { option_tips.Value = value; } }

    M4uProperty<string> option_voice = new M4uProperty<string>();
    public string Option_voice { get { return option_voice.Value; } set { option_voice.Value = value; } }

    M4uProperty<string> option_speed = new M4uProperty<string>();
    public string Option_speed { get { return option_speed.Value; } set { option_speed.Value = value; } }

    M4uProperty<string> option_skill_turn = new M4uProperty<string>();
    public string Option_skill_turn { get { return option_skill_turn.Value; } set { option_skill_turn.Value = value; } }

    M4uProperty<string> option_confirm_as = new M4uProperty<string>();
    public string Option_confirm_as { get { return option_confirm_as.Value; } set { option_confirm_as.Value = value; } }

    M4uProperty<string> option_skill_cost = new M4uProperty<string>();
    public string Option_skill_cost { get { return option_skill_cost.Value; } set { option_skill_cost.Value = value; } }

    M4uProperty<string> option_battle_achieve = new M4uProperty<string>();
    public string Option_battle_achieve { get { return option_battle_achieve.Value; } set { option_battle_achieve.Value = value; } }

    M4uProperty<string> option_quest_end_tips = new M4uProperty<string>();
    public string Option_quest_end_tips { get { return option_quest_end_tips.Value; } set { option_quest_end_tips.Value = value; } }

    M4uProperty<string> option_auto_play_title = new M4uProperty<string>();
    public string Option_auto_play_title { get { return option_auto_play_title.Value; } set { option_auto_play_title.Value = value; } }

    M4uProperty<string> option_auto_play_stop_boss = new M4uProperty<string>();
    public string Option_auto_play_stop_boss { get { return option_auto_play_stop_boss.Value; } set { option_auto_play_stop_boss.Value = value; } }

    M4uProperty<string> option_auto_play_use_as = new M4uProperty<string>();
    public string Option_auto_play_use_as { get { return option_auto_play_use_as.Value; } set { option_auto_play_use_as.Value = value; } }

    M4uProperty<string> skill_no_text = new M4uProperty<string>();
    public string Skill_no_text { get { return skill_no_text.Value; } set { skill_no_text.Value = value; } }

    M4uProperty<string> skill_yes_text = new M4uProperty<string>();
    public string Skill_yes_text { get { return skill_yes_text.Value; } set { skill_yes_text.Value = value; } }

    M4uProperty<string> skill_back_text = new M4uProperty<string>();
    public string Skill_back_text { get { return skill_back_text.Value; } set { skill_back_text.Value = value; } }

    M4uProperty<string> skill_title_text = new M4uProperty<string>();
    public string Skill_title_text { get { return skill_title_text.Value; } set { skill_title_text.Value = value; } }

    M4uProperty<bool> menu_bg_active = new M4uProperty<bool>();
    public bool Menu_bg_active { get { return menu_bg_active.Value; } set { menu_bg_active.Value = value; } }

    M4uProperty<string> play_score_text = new M4uProperty<string>();
    public string Play_score_text { get { return play_score_text.Value2; } set { play_score_text.Value2 = value; } }

    M4uProperty<bool> play_score_is_show = new M4uProperty<bool>();
    public bool Play_score_is_show { get { return play_score_is_show.Value2; } set { play_score_is_show.Value2 = value; } }

    M4uProperty<string> no_play_score_text = new M4uProperty<string>();
    public string No_play_score_text { get { return no_play_score_text.Value2; } set { no_play_score_text.Value2 = value; } }

    M4uProperty<bool> no_play_score_is_show = new M4uProperty<bool>();
    public bool No_play_score_is_show { get { return no_play_score_is_show.Value2; } set { no_play_score_is_show.Value2 = value; } }

    M4uProperty<bool> isBack = new M4uProperty<bool>();
    public bool IsBack { get { return isBack.Value2; } set { isBack.Value2 = value; } }

    M4uProperty<Color> skillMenuOkColor = new M4uProperty<Color>();
    public Color SkillMenuOkColor { get { return skillMenuOkColor.Value2; } set { skillMenuOkColor.Value2 = value; } }

    M4uProperty<bool> isScreenBgTouch = new M4uProperty<bool>();
    public bool IsScreenBgTouch { get { return isScreenBgTouch.Value2; } set { isScreenBgTouch.Value2 = value; } }

    M4uProperty<bool> questEndButtonActive = new M4uProperty<bool>();
    public bool QuestEndButtonActive { get { return questEndButtonActive.Value2; } set { questEndButtonActive.Value2 = value; } }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
    */
    //----------------------------------------------------------------------------
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
        Icon_rare01_num = "00";
        Icon_rare02_num = "00";
        Icon_rare03_num = "00";
        Icon_rare04_num = "00";
        Icon_rare05_num = "00";
        Icon_rare06_num = "00";

        updateOptionButtonView();

        Retire_message = GameTextUtil.GetText("mb60q_content");
        Retire_yes = GameTextUtil.GetText("common_button4");
        Retire_no = GameTextUtil.GetText("common_button5");
        Option_text = GameTextUtil.GetText("mb57q_button1");
        Retire_text = GameTextUtil.GetText("mb57q_button2");
        Back_text = GameTextUtil.GetText("common_button6");
        Option_bgm = GameTextUtil.GetText("option_display1");
        Option_se = GameTextUtil.GetText("option_display2");
        Option_guide = GameTextUtil.GetText("option_display3");
        Option_tips = GameTextUtil.GetText("option_display4");
        Option_voice = GameTextUtil.GetText("option_display5");
        Option_speed = GameTextUtil.GetText("option_display6");
        Option_skill_turn = GameTextUtil.GetText("battle_text01");
        Option_confirm_as = GameTextUtil.GetText("option_display17");
        Option_skill_cost = GameTextUtil.GetText("battle_text02");
        Option_battle_achieve = GameTextUtil.GetText("battle_text03");
        Option_quest_end_tips = GameTextUtil.GetText("option_display18");
        Option_auto_play_title = GameTextUtil.GetText("option_display19");
        Option_auto_play_stop_boss = GameTextUtil.GetText("option_display20");
        Option_auto_play_use_as = GameTextUtil.GetText("option_display21");
        Skill_no_text = GameTextUtil.GetText("common_button5");
        Skill_yes_text = GameTextUtil.GetText("common_button4");
        Skill_back_text = GameTextUtil.GetText("common_button6");
        Skill_title_text = GameTextUtil.GetText("battle_infotext6");
        Play_score_text = "";
        Play_score_is_show = false;
        No_play_score_text = GameTextUtil.GetText("battle_score_noget");
        No_play_score_is_show = false;

        Back_button_enable = true;
        Back_text_color = Color.white;
        Bgm_switch_enable = true;
        Se_switch_enable = true;
        Guide_switch_enable = true;
        Tips_switch_enable = true;
        Voice_switch_enable = true;
        Speed_switch_enable = true;
        Skill_turn_switch_enable = true;
        Confirm_as_switch_enable = true;
        Skill_cost_switch_enable = true;
        Battle_achieve_switch_enable = true;
        Quest_end_tips_switch_enable = true;
        Auto_play_stop_boss_switch_enable = true;
        Auto_play_use_as_switch_enable = true;
        IsScreenBgTouch = false;
        QuestEndButtonActive = false;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：更新処理	※定期処理
    */
    //----------------------------------------------------------------------------
    void Update()
    {
        // オプションが表示されるときにボタン表示を更新
        if (Menu_active && Option_active)
        {
            if (m_IsUpdateOptionButtionView)
            {
                m_IsUpdateOptionButtionView = false;
                updateOptionButtonView();
            }
        }
        else
        {
            m_IsUpdateOptionButtionView = true;
        }

        updatePlayScore();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：更新処理	※定期処理
    */
    //----------------------------------------------------------------------------
    public void setOptionSwitch(int index, bool sw)
    {
        Sprite button = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
        if (sw == false)
        {
            button = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
        }
        switch (index)
        {
            case 0:
                Bgm_switch = button;
                break;
            case 1:
                Se_switch = button;
                break;
            case 2:
                Guide_switch = button;
                break;
            case 3:
                Tips_switch = button;
                break;
            case 4:
                Voice_switch = button;
                break;
            case 5:
                Speed_switch = button;
                break;
            case 6:
                Skill_turn_switch = button;
                break;
            case 7:
                Confirm_as_switch = button;
                break;
            case 8:
                Skill_cost_switch = button;
                break;
            case 9:
                Battle_achieve_switch = button;
                break;
            case 10:
                Quest_end_tips_switch = button;
                break;
            case 11:
                Auto_play_stop_boss_switch = button;
                break;
            case 12:
                Auto_play_use_as_switch = button;
                break;
            default:
                break;
        }
    }


    private void updatePlayScore()
    {
        if (BattleParam.m_PlayScoreInfo != null)
        {
            if (BattleParam.m_PlayScoreInfo.is_enable_score)
            {
                int play_score = BattleParam.m_PlayScoreInfo.play_score;
                string text = play_score.ToString("#,0");
                Play_score_text = text;

                Play_score_is_show = true;
                No_play_score_is_show = false;
            }
            else
            {
                if (BattleParam.m_PlayScoreInfo.isPlayScoreQuest())
                {
                    // プレイスコア計算対象クエストだが計算しない表示
                    Play_score_is_show = false;
                    No_play_score_is_show = true;
                }
                else
                {
                    Play_score_is_show = false;
                    No_play_score_is_show = false;
                }
            }
        }
        else
        {
            Play_score_is_show = false;
            No_play_score_is_show = false;
        }
    }

    private void updateOptionButtonView()
    {
        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        if (cOption != null)
        {
            if (cOption.m_OptionBGM == (int)LocalSaveDefine.OptionBGM.ON)
            {
                Bgm_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Bgm_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionSE == (int)LocalSaveDefine.OptionSE.ON)
            {
                Se_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Se_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionGuide == (int)LocalSaveDefine.OptionGuide.ON)
            {
                Guide_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Guide_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionTIPS == (int)LocalSaveDefine.OptionTips.ON)
            {
                Tips_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Tips_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionVoice == (int)LocalSaveDefine.OptionVoice.ON)
            {
                Voice_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Voice_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionSpeed == (int)LocalSaveDefine.OptionSpeed.ON)
            {
                Speed_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Speed_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionBattleSkillTurn == (int)LocalSaveDefine.OptionBattleSkillTurn.ON)
            {
                Skill_turn_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Skill_turn_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionConfirmAS == (int)LocalSaveDefine.OptionConfirmAS.ON)
            {
                Confirm_as_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Confirm_as_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionBattleSkillCost == (int)LocalSaveDefine.OptionBattleSkillCost.ON)
            {
                Skill_cost_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Skill_cost_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionBattleAchieve == (int)LocalSaveDefine.OptionBattleAchieve.ON)
            {
                Battle_achieve_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Battle_achieve_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionQuestEndTips == (int)LocalSaveDefine.OptionQuestEndTips.ON)
            {
                Quest_end_tips_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Quest_end_tips_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionAutoPlayStopBoss == (int)LocalSaveDefine.OptionAutoPlayStopBoss.ON)
            {
                Auto_play_stop_boss_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Auto_play_stop_boss_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }

            if (cOption.m_OptionAutoPlayUseAS == (int)LocalSaveDefine.OptionAutoPlayUseAS.ON)
            {
                Auto_play_use_as_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[0];
            }
            else
            {
                Auto_play_use_as_switch = InGameMenuManagerQuest2.Instance.m_OptionButton[1];
            }
        }
    }
}
