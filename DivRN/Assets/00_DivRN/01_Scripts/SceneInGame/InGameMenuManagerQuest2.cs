/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	InGameMenuManager.cs
	@brief	インゲーム中のメニュー管理クラス
	@author Developer
	@date 	2013/1/22
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;
using TMPro;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@class		InGameMenuManager
	@brief		インゲーム中のメニュー管理クラス
*/
//----------------------------------------------------------------------------
public class InGameMenuManagerQuest2 : SingletonComponent<InGameMenuManagerQuest2>
{

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public Sprite[] m_OptionButton = null;
    public InGameMenuQuest2 m_InGameMenuQuest2 = null;
    public GameObject m_PartyRoot = null;
    public GameObject m_SkillMenuRoot = null;
    public Image m_MenuButton = null;
    public GameObject m_MenuButtonMask = null;
    public GameObject m_HeroRoot = null;
    public GameObject m_SkillMenuHeroRoot = null;

    // 処理ステップ
    const int GAMEMENU_STEP_NONE = 0;                                   //!< 無し
    const int GAMEMENU_STEP_ACTIVATE = 1;                                   //!< 起動
    const int GAMEMENU_STEP_UPDATE = 2;                                 //!< 更新
    const int GAMEMENU_STEP_DIACTIVATE = 3;                                 //!< 終了
    const int GAMEMENU_STEP_END = 4;                                    //!< 終了待機
    const int GAMEMENU_STEP_MAX = 5;                                    //!< 最大値

    private int m_CurrentMenu = InGameMenuID.GAMEMENU_NONE;                 //!< 現在のメニューID
    private int m_NextMenu = InGameMenuID.GAMEMENU_NONE;                    //!< 次回のメニューID
    private int m_CurrentStep = InGameMenuManagerQuest2.GAMEMENU_STEP_NONE; //!< 現在の処理段階.
    private bool m_OpenFrame = false;
    private string m_QuestName = "";
    private UnitSkillContext m_newSkill = null;

    private GlobalDefine.PartyCharaIndex m_CharaIdx = GlobalDefine.PartyCharaIndex.ERROR;

    private GlobalMenu m_GlobalMenu = null;
    private bool m_SkillMenuActive = false;
    private bool m_BackKey = false;
    private bool m_OkDisable = false;

    private Color m_MenuButtonGray = new Color(0.596f, 0.596f, 0.596f);

    public bool isSkillMenuActive { get { return m_SkillMenuActive; } }

    //------------------------------------------------------------------------
    /*!
		@brief		起動時呼出し
	*/
    //------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
    }

    //------------------------------------------------------------------------
    /*!
		@brief		初期化処理
	*/
    //------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();
        if (SceneModeContinuousBattle.Instance != null)
        {
            MasterDataQuest2 master = MasterDataUtil.GetQuest2ParamFromID(SceneModeContinuousBattle.Instance.m_QuestMissionID);
            if (master != null)
            {
                if (m_InGameMenuQuest2 != null)
                {
                    m_QuestName = master.quest_name;
                }

                if (master.enable_autoplay != MasterDataDefineLabel.BoolType.ENABLE)
                {
                    // オートプレイ禁止でなければ、オートプレイボタンの初期状態を設定（出撃画面での設定値を反映）
                    LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
                    bool is_auto_play = (cOption.m_OptionAutoPlayEnable == (int)LocalSaveDefine.OptionAutoPlayEnable.ON);
                    if (is_auto_play)
                    {
                        BattleParam.setAutoPlayState(BattleParam.AutoPlayState.ON);
                    }
                    else
                    {
                        BattleParam.setAutoPlayState(BattleParam.AutoPlayState.OFF);
                    }
                }
                else
                {
                    // オートプレイ禁止
                    BattleParam.setAutoPlayState(BattleParam.AutoPlayState.NONE);
                }
            }
        }
    }

    //------------------------------------------------------------------------
    /*!
		@brief			更新処理
	*/
    //------------------------------------------------------------------------
    void Update()
    {
        // メニューオープンタイミングのチェック
        bool isOpen = InGameUtil.ChkInGameOpenWindowTiming();
        if (isOpen == false && m_CurrentMenu != InGameMenuID.GAMEMENU_NONE)
        {
            // メニューを閉じる
            CloseInGameMenu();
            return;
        }

        // リタイアリクエストが来ていたら終了
        if (InGameQuestData.Instance && InGameQuestData.Instance.m_InGameRetire)
        {
            m_CurrentMenu = InGameMenuID.GAMEMENU_NONE;
            return;
        }

        m_OpenFrame = false;

        if (BattleParam.IsTutorial())
        {
            updateForTutorial();
        }
    }

    //------------------------------------------------------------------------
    /*!
		@brief		メニュー画面起動
	*/
    //------------------------------------------------------------------------
    public void OpenInGameMenu()
    {
        OpenInGameMenu(InGameMenuID.GAMEMENU_QUESTINFO);
    }

    public void OpenInGameMenu(int menuID, bool backkey = false)
    {
        if (m_InGameMenuQuest2 == null)
        {
            return;
        }
        m_CurrentMenu = menuID;
        switch (m_CurrentMenu)
        {
            case InGameMenuID.GAMEMENU_QUESTINFO:
                {
                    m_InGameMenuQuest2.IsScreenBgTouch = true;
                    OpenQuestInfo();
                }
                break;
            case InGameMenuID.GAMEMENU_OPTION:
                {
                    m_InGameMenuQuest2.IsScreenBgTouch = true;
                    OpenOption();
                }
                break;
            case InGameMenuID.GAMEMENU_RETIRE:
                {
                    m_InGameMenuQuest2.IsScreenBgTouch = false;
                    OpenRetire();
                }
                break;
            default:
                break;
        }

        m_InGameMenuQuest2.Menu_active = true;
        m_InGameMenuQuest2.Menu_bg_active = true;
        m_OpenFrame = true;
        m_BackKey = backkey;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		メニュー画面終了
	*/
    //------------------------------------------------------------------------
    public void CloseInGameMenu()
    {
        m_CurrentMenu = InGameMenuID.GAMEMENU_NONE;
        m_InGameMenuQuest2.Menu_active = false;
        m_InGameMenuQuest2.Menu_bg_active = false;
        m_InGameMenuQuest2.IsScreenBgTouch = false;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		メニュー画面が開かれているか確認
		@retval		bool		[開いてる/開いていない]
	*/
    //------------------------------------------------------------------------
    public bool IsOpenGameMenu()
    {
        if (m_CurrentMenu != InGameMenuID.GAMEMENU_NONE)
        {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	メニューボタンタッチ処理
	*/
    //----------------------------------------------------------------------------
    public void OnMenuButton()
    {
        if (BattleParam.IsTutorial() == true)
        {
            return;
        }

        if (IsOpenGameMenu())
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            CloseInGameMenu();
        }
        else
        {
            if (InGameUtil.ChkInGameOpenWindowTiming())
                SoundUtil.PlaySE(SEID.SE_BATLE_UI_OPEN);
            OpenInGameMenu();
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	クエスト情報メニューOPEN
	*/
    //----------------------------------------------------------------------------
    private void OpenQuestInfo()
    {
        m_InGameMenuQuest2.Quest_info_active = true;
        m_InGameMenuQuest2.Option_active = false;
        m_InGameMenuQuest2.Retire_active = false;
        m_InGameMenuQuest2.Back_active = true;

        m_InGameMenuQuest2.Title_text = m_QuestName;
        m_InGameMenuQuest2.Wave_text = SceneModeContinuousBattle.Instance.GetWaveCount();
        m_InGameMenuQuest2.Icon_rare01_num = InGameQuestData.Instance.GetAcquireUnitRare(MasterDataDefineLabel.RarityType.STAR_1).ToString("00");
        m_InGameMenuQuest2.Icon_rare02_num = InGameQuestData.Instance.GetAcquireUnitRare(MasterDataDefineLabel.RarityType.STAR_2).ToString("00");
        m_InGameMenuQuest2.Icon_rare03_num = InGameQuestData.Instance.GetAcquireUnitRare(MasterDataDefineLabel.RarityType.STAR_3).ToString("00");
        m_InGameMenuQuest2.Icon_rare04_num = InGameQuestData.Instance.GetAcquireUnitRare(MasterDataDefineLabel.RarityType.STAR_4).ToString("00");
        m_InGameMenuQuest2.Icon_rare05_num = InGameQuestData.Instance.GetAcquireUnitRare(MasterDataDefineLabel.RarityType.STAR_5).ToString("00");
        m_InGameMenuQuest2.Icon_rare06_num = InGameQuestData.Instance.GetAcquireUnitRare(MasterDataDefineLabel.RarityType.STAR_6).ToString("00");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オプションメニューOPEN
	*/
    //----------------------------------------------------------------------------
    private void OpenOption()
    {
        m_InGameMenuQuest2.Quest_info_active = false;
        m_InGameMenuQuest2.Option_active = true;
        m_InGameMenuQuest2.Retire_active = false;
        m_InGameMenuQuest2.Back_active = true;

        m_InGameMenuQuest2.Title_text = GameTextUtil.GetText("mb58q_title");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リタイアメニューOPEN
	*/
    //----------------------------------------------------------------------------
    private void OpenRetire()
    {
        m_InGameMenuQuest2.Quest_info_active = false;
        m_InGameMenuQuest2.Option_active = false;
        m_InGameMenuQuest2.Retire_active = true;
        m_InGameMenuQuest2.Back_active = false;

        m_InGameMenuQuest2.Title_text = GameTextUtil.GetText("mb60q_title");
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オプションメニューボタンタッチ処理
	*/
    //----------------------------------------------------------------------------
    public void OnOption()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        OpenInGameMenu(InGameMenuID.GAMEMENU_OPTION);
        m_MenuButton.color = m_MenuButtonGray;
        UnityUtil.SetObjectEnabled(m_MenuButtonMask, true);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リタイアメニューボタンタッチ処理
	*/
    //----------------------------------------------------------------------------
    public void OnRetire()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        OpenInGameMenu(InGameMenuID.GAMEMENU_RETIRE);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	戻るボタンタッチ処理
	*/
    //----------------------------------------------------------------------------
    public void OnBack()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        CloseInGameMenu();
        if (UnityUtil.ChkObjectEnabled(m_MenuButtonMask) == true)
        {
            m_MenuButton.color = Color.white;
            UnityUtil.SetObjectEnabled(m_MenuButtonMask, false);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リタイアボタンタッチ処理
	*/
    //----------------------------------------------------------------------------
    public void OnRetire(int sw)
    {
        if (sw == 0)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK);
            m_InGameMenuQuest2.Retire_active = false;   // ここでは CloseInGameMenu() はしない。Closeすると一瞬ゲームを操作できてしまう。
            SceneModeContinuousBattleFSM.Instance.SendFsmEvent("INGAME_STEP_GAME_RETIRE");
        }
        else
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            if (m_BackKey == false)
            {
                OpenInGameMenu(InGameMenuID.GAMEMENU_QUESTINFO);
            }
            else
            {
                CloseInGameMenu();
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	オプション項目タッチ処理
	*/
    //----------------------------------------------------------------------------
    public void OnOption(int index)
    {
        bool sw = false;
        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        switch (index)
        {
            case 0:
                {
                    if (cOption.m_OptionBGM == (int)LocalSaveDefine.OptionBGM.OFF)
                    {
                        cOption.m_OptionBGM = (int)LocalSaveDefine.OptionBGM.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionBGM = (int)LocalSaveDefine.OptionBGM.OFF;
                    }
                }
                break;
            case 1:
                {
                    if (cOption.m_OptionSE == (int)LocalSaveDefine.OptionSE.OFF)
                    {
                        cOption.m_OptionSE = (int)LocalSaveDefine.OptionSE.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionSE = (int)LocalSaveDefine.OptionSE.OFF;
                    }
                }
                break;
            case 2:
                {
                    if (cOption.m_OptionGuide == (int)LocalSaveDefine.OptionGuide.OFF)
                    {
                        cOption.m_OptionGuide = (int)LocalSaveDefine.OptionGuide.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionGuide = (int)LocalSaveDefine.OptionGuide.OFF;
                    }
                }
                break;
            case 3:
                {
                    if (cOption.m_OptionTIPS == (int)LocalSaveDefine.OptionTips.OFF)
                    {
                        cOption.m_OptionTIPS = (int)LocalSaveDefine.OptionTips.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionTIPS = (int)LocalSaveDefine.OptionTips.OFF;
                    }
                }
                break;
            case 4:
                {
                    if (cOption.m_OptionVoice == (int)LocalSaveDefine.OptionVoice.OFF)
                    {
                        cOption.m_OptionVoice = (int)LocalSaveDefine.OptionVoice.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionVoice = (int)LocalSaveDefine.OptionVoice.OFF;
                    }
                }
                break;
            case 5:
                {
                    if (cOption.m_OptionSpeed == (int)LocalSaveDefine.OptionSpeed.OFF)
                    {
                        cOption.m_OptionSpeed = (int)LocalSaveDefine.OptionSpeed.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionSpeed = (int)LocalSaveDefine.OptionSpeed.OFF;
                    }
                }
                break;
            case 6:
                {
                    if (cOption.m_OptionBattleSkillTurn == (int)LocalSaveDefine.OptionBattleSkillTurn.OFF)
                    {
                        cOption.m_OptionBattleSkillTurn = (int)LocalSaveDefine.OptionBattleSkillTurn.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionBattleSkillTurn = (int)LocalSaveDefine.OptionBattleSkillTurn.OFF;
                    }
                    InGamePlayerParty.Instance.ASTurnInfo(sw);
                }
                break;
            case 7:
                {
                    if (cOption.m_OptionConfirmAS == (int)LocalSaveDefine.OptionConfirmAS.OFF)
                    {
                        cOption.m_OptionConfirmAS = (int)LocalSaveDefine.OptionConfirmAS.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionConfirmAS = (int)LocalSaveDefine.OptionConfirmAS.OFF;
                    }
                }
                break;
            case 8:
                {
                    if (cOption.m_OptionBattleSkillCost == (int)LocalSaveDefine.OptionBattleSkillCost.OFF)
                    {
                        cOption.m_OptionBattleSkillCost = (int)LocalSaveDefine.OptionBattleSkillCost.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionBattleSkillCost = (int)LocalSaveDefine.OptionBattleSkillCost.OFF;
                    }
                    InGamePlayerParty.Instance.NormalSkillInfo(sw);
                }
                break;
            case 9:
                {
                    if (cOption.m_OptionBattleAchieve == (int)LocalSaveDefine.OptionBattleAchieve.OFF)
                    {
                        cOption.m_OptionBattleAchieve = (int)LocalSaveDefine.OptionBattleAchieve.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionBattleAchieve = (int)LocalSaveDefine.OptionBattleAchieve.OFF;
                    }
                }
                break;
            case 10:
                {
                    if (cOption.m_OptionQuestEndTips == (int)LocalSaveDefine.OptionQuestEndTips.OFF)
                    {
                        cOption.m_OptionQuestEndTips = (int)LocalSaveDefine.OptionQuestEndTips.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionQuestEndTips = (int)LocalSaveDefine.OptionQuestEndTips.OFF;
                    }
                }
                break;
            case 11:
                {
                    if (cOption.m_OptionAutoPlayStopBoss == (int)LocalSaveDefine.OptionAutoPlayStopBoss.OFF)
                    {
                        cOption.m_OptionAutoPlayStopBoss = (int)LocalSaveDefine.OptionAutoPlayStopBoss.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionAutoPlayStopBoss = (int)LocalSaveDefine.OptionAutoPlayStopBoss.OFF;
                    }
                }
                break;
            case 12:
                {
                    if (cOption.m_OptionAutoPlayUseAS == (int)LocalSaveDefine.OptionAutoPlayUseAS.OFF)
                    {
                        cOption.m_OptionAutoPlayUseAS = (int)LocalSaveDefine.OptionAutoPlayUseAS.ON;
                        sw = true;
                    }
                    else
                    {
                        cOption.m_OptionAutoPlayUseAS = (int)LocalSaveDefine.OptionAutoPlayUseAS.OFF;
                    }
                }
                break;
            default:
                break;
        }
        LocalSaveManager.Instance.SaveFuncOption(cOption);
        m_InGameMenuQuest2.setOptionSwitch(index, sw);
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	スキル発動ウィンドウOPEN
		@param[in]	GlobalDefine.PartyCharaIndex	(charaIdx)		キャラパーティインデックス
		@param[in]	uint							(charaId)		キャラID
	*/
    //----------------------------------------------------------------------------
    public void OpenSkillMenu(GlobalDefine.PartyCharaIndex charaIdx, uint charaId, int turn, bool bSealed = false)
    {
        if (m_InGameMenuQuest2 == null)
        {
            return;
        }
        if (charaIdx == GlobalDefine.PartyCharaIndex.ERROR)
        {
            return;
        }
        // 既に開いてる場合
        if (m_SkillMenuActive == true)
        {
            // 一旦初期化する
            if (InGameUtil.isQuest2() == true)
            {
                changePartyUnitParent(false);
            }
            if (m_CharaIdx == GlobalDefine.PartyCharaIndex.HERO)
                m_newSkill.setupHeroSkill(0);
            else
                m_newSkill.setupLimitBreakSkill(0, 0);
            m_InGameMenuQuest2.Skill_menu_active = false;
            m_InGameMenuQuest2.Menu_bg_active = false;
        }

        m_SkillMenuActive = true;
        m_CharaIdx = charaIdx;
        uint skill_limitbreak = 0;
        uint limitbreak_lv = 0;
        if (charaIdx == GlobalDefine.PartyCharaIndex.HERO)
        {
            int unique_id = UserDataAdmin.Instance.m_StructPlayer.current_hero_id;
            for (int i = 0; i < UserDataAdmin.Instance.m_StructHeroList.Length; ++i)
            {
                if (UserDataAdmin.Instance.m_StructHeroList[i].unique_id == unique_id)
                {
                    skill_limitbreak = (uint)UserDataAdmin.Instance.m_StructHeroList[i].current_skill_id;
                }
            }
        }
        else
        {
            MasterDataParamChara _master = InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaMasterDataParam;
            skill_limitbreak = _master.skill_limitbreak;
            limitbreak_lv = (uint)InGamePlayerParty.m_PlayerPartyChara[(int)m_CharaIdx].m_CharaLBSLv;
        }

        if (m_newSkill == null)
        {
            m_newSkill = new UnitSkillContext();
            m_InGameMenuQuest2.SkillList.Add(m_newSkill);
        }
        if (charaIdx == GlobalDefine.PartyCharaIndex.HERO)
            m_newSkill.setupHeroSkill(skill_limitbreak);
        else
            m_newSkill.setupLimitBreakSkill(skill_limitbreak, limitbreak_lv);

        m_InGameMenuQuest2.Skill_menu_active = true;
        m_InGameMenuQuest2.Menu_bg_active = true;

        changePartyUnitParent(true);

        SoundUtil.PlaySE(SEID.SE_BATLE_UI_OPEN);
        m_MenuButton.color = m_MenuButtonGray;
        m_OkDisable = false;
        m_InGameMenuQuest2.SkillMenuOkColor = Color.white;
        if (turn == 0)
        {
            if (bSealed == true)
            {
                m_InGameMenuQuest2.Skill_title_text = GameTextUtil.GetText("unit_action_01");
                m_InGameMenuQuest2.IsBack = true;
            }
            else
            {
                m_InGameMenuQuest2.Skill_title_text = GameTextUtil.GetText("battle_infotext6");
                m_InGameMenuQuest2.IsBack = false;
                if (charaIdx != GlobalDefine.PartyCharaIndex.HERO)
                {
                    if (BattleSceneUtil.checkLimitBreak(BattleParam.m_PlayerParty, m_CharaIdx, BattleParam.m_EnemyParam, BattleParam.m_TargetEnemyCurrent) == false)
                    {
                        m_OkDisable = true;
                        m_InGameMenuQuest2.SkillMenuOkColor = Color.gray;
                    }
                }
            }
        }
        else
        {
            if (charaIdx == GlobalDefine.PartyCharaIndex.HERO)
            {
                m_InGameMenuQuest2.Skill_title_text = string.Format(GameTextUtil.GetText("hero_skill_hands_battle"), turn);
            }
            else
            {
                m_InGameMenuQuest2.Skill_title_text = string.Format(GameTextUtil.GetText("unit_skill_turn_battle"), turn);
            }
            m_InGameMenuQuest2.IsBack = true;
        }
        UnityUtil.SetObjectEnabled(m_MenuButtonMask, true);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	スキル発動OKボタンタッチ処理
	*/
    //----------------------------------------------------------------------------
    public void OnSkillYes()
    {
        if (m_OkDisable == true)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_NG);
            return;
        }
        if (InGameUtil.isQuest2() == true)
        {
            changePartyUnitParent(false);
        }
        if (m_CharaIdx == GlobalDefine.PartyCharaIndex.HERO)
            m_newSkill.setupHeroSkill(0);
        else
            m_newSkill.setupLimitBreakSkill(0, 0);
        m_InGameMenuQuest2.Skill_menu_active = false;
        m_InGameMenuQuest2.Menu_bg_active = false;
        m_SkillMenuActive = false;
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        m_MenuButton.color = Color.white;
        UnityUtil.SetObjectEnabled(m_MenuButtonMask, false);

        BattleParam.RequestLBS(m_CharaIdx);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	スキル発動NOボタンタッチ処理
	*/
    //----------------------------------------------------------------------------
    public void OnSkillNo()
    {
        if (BattleParam.IsTutorial())
        {
            return; // チュートリアル中は押せなくする
        }

        if (InGameUtil.isQuest2() == true)
        {
            changePartyUnitParent(false);
        }
        if (m_CharaIdx == GlobalDefine.PartyCharaIndex.HERO)
            m_newSkill.setupHeroSkill(0);
        else
            m_newSkill.setupLimitBreakSkill(0, 0);
        m_InGameMenuQuest2.Skill_menu_active = false;
        m_InGameMenuQuest2.Menu_bg_active = false;
        m_SkillMenuActive = false;
        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        m_MenuButton.color = Color.white;
        UnityUtil.SetObjectEnabled(m_MenuButtonMask, false);
    }

    public void OnQuestEndButton()
    {
        if (SceneModeContinuousBattle.Instance != null)
        {
            SceneModeContinuousBattle.Instance.OnQuestButtonPush();
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	パーティーアイコン表示プライオリティ変更処理
	*/
    //----------------------------------------------------------------------------
    private void changePartyUnitParent(bool front)
    {
        GameObject parentUnit = m_PartyRoot;
        GameObject parentHero = m_HeroRoot;
        float scale = 1;
        float heroScale = 1;
        Color nonSelectColor = new Color(0.7f, 0.7f, 0.7f);
        Color spColorGray = new Color(0.6f, 0.6f, 0.6f);
        Color color = Color.white;
        Color spColor = Color.white;
        bool animStop = false;
        float balloonY = 630;
        if (front == true)
        {
            parentUnit = m_SkillMenuRoot;
            parentHero = m_SkillMenuHeroRoot;
            scale = 1.4f;
            heroScale = 1.3f;
            color = nonSelectColor;
            animStop = true;
            spColor = spColorGray;
        }

        for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; ++i)
        {
            if (i != (int)m_CharaIdx)
            {
                InGamePlayerParty.Instance.m_UnitBase[i].transform.SetParent(parentUnit.transform, false);
                InGamePlayerParty.Instance.m_PartyUnit[i].UnitIconColor = color;
                if (InGamePlayerParty.Instance.m_PartyUnit[i].IsSkillActivate() == true)
                {
                    InGamePlayerParty.Instance.m_PartyUnit[i].setReadyAnimation(animStop);
                }
                InGamePlayerParty.Instance.m_PartyUnit[i].SetUnitRoot(false);
            }
            InGamePlayerParty.Instance.m_PartyUnit[i].CheckASMask(front);
        }
        if (m_CharaIdx != GlobalDefine.PartyCharaIndex.HERO)
        {
            InGamePlayerParty.Instance.m_HeroBase.transform.SetParent(parentHero.transform, false);
            InGamePlayerParty.Instance.m_UnitBase[(int)m_CharaIdx].transform.SetParent(parentUnit.transform, false);
            InGamePlayerParty.Instance.m_PartyUnit[(int)m_CharaIdx].setScale(scale);
            InGamePlayerParty.Instance.m_InGamePartyManager.HeroColor = color;
            if (InGamePlayerParty.Instance.IsSkillActivate() == true)
            {
                InGamePlayerParty.Instance.setReadyAnimation(animStop);
            }
            if (front == true)
            {
                InGamePlayerParty.Instance.m_PartyUnit[(int)m_CharaIdx].SetUnitRoot(true);
            }
            else
            {
                InGamePlayerParty.Instance.m_PartyUnit[(int)m_CharaIdx].SetUnitRoot(false);
            }
        }
        else
        {
            InGamePlayerParty.Instance.m_HeroBase.transform.SetParent(parentHero.transform, false);
            InGamePlayerParty.Instance.setHeroScale(heroScale);
            if (front == true)
            {
                balloonY = 675;
            }
        }
        InGamePlayerParty.Instance.m_InGamePartyManager.SpColor = spColor;
        InGamePlayerParty.Instance.m_InGamePartyManager.HeroBalloonY = balloonY;
    }

    public bool IsSkillMenuActiveUnit(GlobalDefine.PartyCharaIndex index)
    {
        if (index == m_CharaIdx)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// チュートリアル向けの特殊処理
    /// </summary>
    private void updateForTutorial()
    {
        if (BattleParam.IsTutorial()
            && m_InGameMenuQuest2.Menu_active != false
            && m_InGameMenuQuest2.Option_active != false
        )
        {
            BattleTutorialManager.TutorialOptionMenuPhase tutorial_option_menu_phase = BattleParam.getTutorialOptionMenuPhase();
            switch (tutorial_option_menu_phase)
            {
                case BattleTutorialManager.TutorialOptionMenuPhase.INIT:
                    {
                        // チュートリアルで説明する項目を初期化
                        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
                        if (cOption != null)
                        {
                            if (cOption.m_OptionBattleSkillTurn != (int)LocalSaveDefine.OptionBattleSkillTurn.OFF)
                            {
                                OnOption(6);
                            }
                            if (cOption.m_OptionBattleSkillCost != (int)LocalSaveDefine.OptionBattleSkillCost.OFF)
                            {
                                OnOption(8);
                            }
                        }
                    }
                    break;

                case BattleTutorialManager.TutorialOptionMenuPhase.SKILL_TURN:
                    {
                        m_InGameMenuQuest2.Bgm_switch_enable = false;
                        m_InGameMenuQuest2.Se_switch_enable = false;
                        m_InGameMenuQuest2.Guide_switch_enable = false;
                        m_InGameMenuQuest2.Tips_switch_enable = false;
                        m_InGameMenuQuest2.Voice_switch_enable = false;
                        m_InGameMenuQuest2.Speed_switch_enable = false;
                        m_InGameMenuQuest2.Skill_turn_switch_enable = true;
                        m_InGameMenuQuest2.Confirm_as_switch_enable = false;
                        m_InGameMenuQuest2.Skill_cost_switch_enable = false;
                        m_InGameMenuQuest2.Battle_achieve_switch_enable = false;
                        m_InGameMenuQuest2.Quest_end_tips_switch_enable = false;
                        m_InGameMenuQuest2.Auto_play_stop_boss_switch_enable = false;
                        m_InGameMenuQuest2.Auto_play_use_as_switch_enable = false;
                        m_InGameMenuQuest2.Back_button_enable = false;
                        m_InGameMenuQuest2.Back_text_color = new Color32(255, 255, 255, 128);
                        m_InGameMenuQuest2.IsScreenBgTouch = false;
                    }
                    break;

                case BattleTutorialManager.TutorialOptionMenuPhase.SKILL_COST:
                    {
                        m_InGameMenuQuest2.Bgm_switch_enable = false;
                        m_InGameMenuQuest2.Se_switch_enable = false;
                        m_InGameMenuQuest2.Guide_switch_enable = false;
                        m_InGameMenuQuest2.Tips_switch_enable = false;
                        m_InGameMenuQuest2.Voice_switch_enable = false;
                        m_InGameMenuQuest2.Speed_switch_enable = false;
                        m_InGameMenuQuest2.Skill_turn_switch_enable = false;
                        m_InGameMenuQuest2.Confirm_as_switch_enable = false;
                        m_InGameMenuQuest2.Skill_cost_switch_enable = true;
                        m_InGameMenuQuest2.Battle_achieve_switch_enable = false;
                        m_InGameMenuQuest2.Quest_end_tips_switch_enable = false;
                        m_InGameMenuQuest2.Auto_play_stop_boss_switch_enable = false;
                        m_InGameMenuQuest2.Auto_play_use_as_switch_enable = false;
                        m_InGameMenuQuest2.Back_button_enable = false;
                        m_InGameMenuQuest2.Back_text_color = new Color32(255, 255, 255, 128);
                        m_InGameMenuQuest2.IsScreenBgTouch = false;
                    }
                    break;

                case BattleTutorialManager.TutorialOptionMenuPhase.BACK_BUTTON:
                    {
                        m_InGameMenuQuest2.Bgm_switch_enable = false;
                        m_InGameMenuQuest2.Se_switch_enable = false;
                        m_InGameMenuQuest2.Guide_switch_enable = false;
                        m_InGameMenuQuest2.Tips_switch_enable = false;
                        m_InGameMenuQuest2.Voice_switch_enable = false;
                        m_InGameMenuQuest2.Speed_switch_enable = false;
                        m_InGameMenuQuest2.Skill_turn_switch_enable = false;
                        m_InGameMenuQuest2.Confirm_as_switch_enable = false;
                        m_InGameMenuQuest2.Skill_cost_switch_enable = false;
                        m_InGameMenuQuest2.Battle_achieve_switch_enable = false;
                        m_InGameMenuQuest2.Quest_end_tips_switch_enable = false;
                        m_InGameMenuQuest2.Auto_play_stop_boss_switch_enable = false;
                        m_InGameMenuQuest2.Auto_play_use_as_switch_enable = false;
                        m_InGameMenuQuest2.Back_button_enable = true;
                        m_InGameMenuQuest2.Back_text_color = Color.white;
                        m_InGameMenuQuest2.IsScreenBgTouch = true;
                    }
                    break;

                case BattleTutorialManager.TutorialOptionMenuPhase.ALL_ON:
                default:
                    {
                        m_InGameMenuQuest2.Bgm_switch_enable = true;
                        m_InGameMenuQuest2.Se_switch_enable = true;
                        m_InGameMenuQuest2.Guide_switch_enable = true;
                        m_InGameMenuQuest2.Tips_switch_enable = true;
                        m_InGameMenuQuest2.Voice_switch_enable = true;
                        m_InGameMenuQuest2.Speed_switch_enable = true;
                        m_InGameMenuQuest2.Skill_turn_switch_enable = true;
                        m_InGameMenuQuest2.Confirm_as_switch_enable = true;
                        m_InGameMenuQuest2.Skill_cost_switch_enable = true;
                        m_InGameMenuQuest2.Battle_achieve_switch_enable = true;
                        m_InGameMenuQuest2.Quest_end_tips_switch_enable = true;
                        m_InGameMenuQuest2.Auto_play_stop_boss_switch_enable = true;
                        m_InGameMenuQuest2.Auto_play_use_as_switch_enable = true;
                        m_InGameMenuQuest2.Back_button_enable = true;
                        m_InGameMenuQuest2.Back_text_color = Color.white;
                        m_InGameMenuQuest2.IsScreenBgTouch = true;
                    }
                    break;
            }
        }
    }
} // class InGameMenuManager
