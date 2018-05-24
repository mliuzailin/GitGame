/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	SceneObjReferGameMain.cs
	@brief	非アクティブオブジェクト一元化クラス
	@author Developer
	@date 	2012/10/04
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
	@brief	非アクティブオブジェクト一元化クラス
*/
//----------------------------------------------------------------------------

public class InGamePlayerParty : SingletonComponent<InGamePlayerParty>
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    //------------------------------------------------------------------------
    // フィールドUI
    //------------------------------------------------------------------------
    public GameObject m_PlayerPartyRoot;
    public GameObject m_HeroRoot;
    public GameObject[] m_UnitBase;
    public GameObject[] m_UnitSkillGauge;
    public GameObject m_HeroBase;
    public Sprite[] m_SkillElementSprite;
    public Sprite m_EmptySprite;
    public Sprite[] m_HandsSprite;
    public Sprite[] m_HandsNumSprite;
    public Sprite[] m_IconHeart;
    public Sprite[] m_HpBar;
    public Sprite[] m_Balloon2;
    public Sprite[] m_AilmentIcon;
    public InGamePartyManager m_InGamePartyManager;
    public Sprite m_HeroSprite;
    public Texture m_HeroSprite_mask;
    public Animation m_ReadyAnimation = null;
    public Image m_HeroGauge = null;

    [HideInInspector]
    public InGamePartyUnit[] m_PartyUnit = null;     //!< ゲーム中オブジェクト：
                                                     //----------------------------------------
                                                     // プレイヤーパラメータ関連定義。
                                                     //----------------------------------------
    [HideInInspector]
    public int m_PartyTotalSPPrv = 0;       //!< プレイヤーSP現在値。
    [HideInInspector]
    public int[] m_PartyTotalSkillPrv = null;       //!< プレイヤースキル発動カウント現在値。
    [HideInInspector]
    public int[] m_PartyTotalSkillMaxPrv = null;        //!< プレイヤースキル発動カウント最大値。
    [HideInInspector]
    public int[] m_PartyHandsPrv = null;        //!< プレイヤースキル発動カウント最大値。
    [HideInInspector]
    public int m_PartyTotalHandsPrv = 0;        //!< プレイヤースキル発動カウント最大値。

    [HideInInspector]
    public bool m_HandsView = false;    //!<

    public static CharaParty m_PlayerParty = null;      //!< プレイヤーパーティー関連：パーティー情報
    public static CharaOnce[] m_PlayerPartyChara = null;        //!< プレイヤーパーティー関連：キャラ情報

    private bool m_NewBattle = false;
    private bool m_InfoOpen = false;

    private float SkillGaugeMaxRate = 0.659f;
    private int nTouchCharaID = InGameDefine.SELECT_NONE;
    private uint m_UnitNum = 0;
    private int m_TicketNum = 0;
    private uint m_CoinNum = 0;
    private int m_HeroTotalSkillPrv = 0;

    private float[] unit_pos_x =
    {
        147,     45,    -57,   -159,   -261
    };

    private float[] unit_pos_y =
    {
        512,    493,    493,    512,    535
    };

    private float[] sp_effect_pos =
    {
        0,0,
        -0.460f,-0.170f, -0.420f,-0.175f, -0.380f,-0.180f, -0.330f,-0.185f, -0.280f,-0.190f,
        -0.215f,-0.195f, -0.175f,-0.200f, -0.130f,-0.205f, -0.080f,-0.205f, -0.030f,-0.210f,
         0.030f,-0.210f,  0.080f,-0.205f,  0.130f,-0.205f,  0.175f,-0.200f,  0.215f,-0.195f,
         0.280f,-0.190f,  0.330f,-0.185f,  0.380f,-0.180f,  0.420f,-0.175f,  0.460f,-0.170f,
    };

    public enum AilmentType : int
    {
        ATTACK_UP = 0,
        ATTACK_DOWN,
        DEFENCE_UP,
        DEFENCE_DOWN,
        TIMER_UP,
        TIMER_DOWN,
        DARK,
        POISON,
    };
    private Color Balloon_gray = new Color(0.4f, 0.4f, 0.4f, 1);

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    protected override void Awake()
    {
        base.Awake();
        m_PlayerParty = null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();
        //--------------------------------
        // UI表示用パラメータ初期化。
        //--------------------------------
        m_PartyTotalSkillPrv = new int[(int)GlobalDefine.PartyCharaIndex.MAX];      //!< プレイヤーHP最大値。
        m_PartyTotalSkillMaxPrv = new int[(int)GlobalDefine.PartyCharaIndex.MAX];      //!< プレイヤーHP最大値。
        m_PartyHandsPrv = new int[(int)GlobalDefine.PartyCharaIndex.MAX];
        for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
        {
            m_PartyTotalSkillPrv[i] = new int();
            m_PartyTotalSkillPrv[i] = -1;
            m_PartyTotalSkillMaxPrv[i] = new int();
            m_PartyTotalSkillMaxPrv[i] = -1;
            m_PartyHandsPrv[i] = -1;
        }
        m_PartyTotalSPPrv = -1;
        m_PartyTotalHandsPrv = -1;
        nTouchCharaID = InGameDefine.SELECT_NONE;
        m_InfoOpen = false;
        m_HandsView = false;
        m_HeroTotalSkillPrv = -1;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    void Update()
    {
        //--------------------------------
        // プレイヤー関連ゲージの更新
        // ※パーティ情報に更新があった場合のみ更新
        //--------------------------------
        if (m_PlayerParty != null
        && m_PlayerParty.m_PartySetupOK == true
        && SceneModeContinuousBattle.Instance.m_InGameBattleInitOK == true
        && BattleParam.m_PlayerParty != null)
        {
            int total_hands = 0;
            // 保存しているSPと現在値が異なっている場合、SP表示更新。
            if (m_PartyTotalSPPrv != m_PlayerParty.m_PartyTotalSP
            && BattleSkillCutinManager.Instance != null
            && BattleSkillCutinManager.Instance.isRunning() == false)
            {
                m_PartyTotalSPPrv = m_PlayerParty.m_PartyTotalSP;

                m_InGamePartyManager.setGaugeSp(m_PartyTotalSPPrv);
            }
            // 総Hands数
            if (m_HandsView == true)
            {
                for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
                {
                    if (InGamePlayerParty.m_PlayerPartyChara[i] != null
                    && InGamePlayerParty.m_PlayerPartyChara[i].m_CharaMasterDataParam != null)
                    {
                        total_hands += BattleParam.getPartyMemberHands((GlobalDefine.PartyCharaIndex)i);
                    }
                }
                if (m_PartyTotalHandsPrv != total_hands)
                {
                    if (m_PartyTotalHandsPrv < 0 && total_hands == 0)
                    { }
                    else
                    {
                        m_PartyTotalHandsPrv = total_hands;
                        m_InGamePartyManager.setTotalHands(m_PartyTotalHandsPrv);
                    }
                }
            }
            else
            {
                m_PartyTotalHandsPrv = -1;
                if (m_InGamePartyManager.Total_hands_active == true)
                {
                    m_InGamePartyManager.setTotalHands(-1);
                }
            }
            if (BattleParam.m_PlayerParty.m_BattleHero != null
            && m_HeroTotalSkillPrv != BattleParam.m_PlayerParty.m_BattleHero.getSkillTurn())
            {
                m_HeroTotalSkillPrv = BattleParam.m_PlayerParty.m_BattleHero.getSkillTurn();


                //----------------------------------------
                // スキルターン数に合わせてUIのゲージ更新
                //----------------------------------------
                float rate = 0;
                if (BattleParam.m_PlayerParty.m_BattleHero.getSkillTurnMax() != 0)
                {
                    rate = 1 - ((float)m_HeroTotalSkillPrv / (float)BattleParam.m_PlayerParty.m_BattleHero.getSkillTurnMax());
                }
                m_HeroGauge.fillAmount = rate;
                if (BattleParam.m_PlayerParty.m_BattleHero.checkHeroSkillTurn() == false)
                {
                    m_InGamePartyManager.Hero_balloon_text = string.Format(GameTextUtil.GetText("hero_skill_hands_battle"), m_HeroTotalSkillPrv);
                    m_InGamePartyManager.Hero_balloon2_active = false;
                    if (m_InfoOpen == true
                    && m_InGamePartyManager.Hero_balloon1_active == false)
                    {
                        m_InGamePartyManager.Hero_balloon1_active = true;
                    }
                }
                else
                {
                    m_InGamePartyManager.Hero_balloon2_active = true;
                    m_InGamePartyManager.Hero_balloon1_active = false;
                    if (BattleSceneUtil.checkLimitBreak(BattleParam.m_PlayerParty, GlobalDefine.PartyCharaIndex.HERO, BattleParam.m_EnemyParam, BattleParam.m_TargetEnemyCurrent) == false)
                    {
                        m_InGamePartyManager.Hero_balloon2_color = Balloon_gray;
                    }
                    else
                    {
                        m_InGamePartyManager.Hero_balloon2_color = Color.white;
                    }
                }
            }
        }

        if (InGameQuestData.Instance != null)
        {
            //----------------------------------------
            // 金額総数を表示
            //----------------------------------------
            if (m_CoinNum != InGameQuestData.Instance.GetAcquireMoneyTotal())
            {
                m_CoinNum = InGameQuestData.Instance.GetAcquireMoneyTotal();
                m_InGamePartyManager.Coin_num = m_CoinNum.ToString();
            }
            //----------------------------------------
            // 取得チケット数を表示
            //----------------------------------------
            if (m_TicketNum != InGameQuestData.Instance.GetAcquireTicketTotal())
            {
                m_TicketNum = InGameQuestData.Instance.GetAcquireTicketTotal();
                m_InGamePartyManager.Ticket_num = m_TicketNum.ToString();
            }
            //----------------------------------------
            // 取得ユニット数数を表示
            //----------------------------------------
            if (m_UnitNum != InGameQuestData.Instance.GetAcquireUnitTotal())
            {
                m_UnitNum = InGameQuestData.Instance.GetAcquireUnitTotal();
                m_InGamePartyManager.Unit_num = m_UnitNum.ToString();
            }
        }

        // 「Ready」アニメーション制御
        if (m_InGamePartyManager.Hero_balloon2_active
        && (InGameMenuManagerQuest2.Instance != null && InGameMenuManagerQuest2.Instance.isSkillMenuActive == false))
        {
            bool animStop = false;
            if (BattleParam.getBattlePhase() == BattleParam.BattlePhase.INPUT_HANDLING
                || BattleParam.isCountDown()
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
		@brief	パーティー情報設定処理：
	*/
    //----------------------------------------------------------------------------
    public void setup(CharaParty playerParty, CharaOnce[] playerPartyChara, bool newBattle, bool[] balloon)
    {
        m_NewBattle = newBattle;
        if (m_PartyUnit.IsNullOrEmpty())
        {
            m_PartyUnit = new InGamePartyUnit[(int)GlobalDefine.PartyCharaIndex.MAX];
        }
        //------------------------------
        //	パーティ全員分処理
        //------------------------------
        if (playerPartyChara != null)
        {
            InGamePlayerParty.m_PlayerPartyChara = playerPartyChara;

            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {
                m_PartyUnit[i] = m_UnitBase[i].GetComponent<InGamePartyUnit>() as InGamePartyUnit;
                if (m_PartyUnit[i] != null)
                {
                    m_PartyUnit[i].SetCharaID(InGamePlayerParty.m_PlayerPartyChara[i], newBattle, unit_pos_x[i], unit_pos_y[i], (GlobalDefine.PartyCharaIndex)i, balloon[i]);
                    if (LocalSaveManager.Instance.LoadFuncOption().m_OptionBattleSkillTurn == (int)LocalSaveDefine.OptionBattleSkillTurn.ON)
                    {
                        m_PartyUnit[i].openASTurnInfo();
                    }
                    if (LocalSaveManager.Instance.LoadFuncOption().m_OptionBattleSkillCost == (int)LocalSaveDefine.OptionBattleSkillCost.ON)
                    {
                        m_PartyUnit[i].openNormalSkillInfo();
                    }
                }
            }
        }
        if (playerParty != null)
        {
            InGamePlayerParty.m_PlayerParty = playerParty;
        }
        m_InGamePartyManager.setupHero();
        if (LocalSaveManager.Instance.LoadFuncOption().m_OptionBattleSkillTurn == (int)LocalSaveDefine.OptionBattleSkillTurn.ON)
        {
            m_InfoOpen = true;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	情報開閉ボタンタッチ処理：
	*/
    //----------------------------------------------------------------------------
    public void OnInfoTouch()
    {
        // メニューオープンタイミングのチェック
        bool isOpen = InGameUtil.ChkInGameOpenWindowTiming();
        if (isOpen == false)
        {
            return;
        }

        if (BattleParam.isEnbaleOptionButton() == false)
        {
            return;
        }

        if (InGameMenuManagerQuest2.Instance != null)
        {
            InGameMenuManagerQuest2.Instance.OnOption();
        }
    }

    public void OnAutoPlayButton()
    {
        if (BattleParam.IsTutorial())
        {
            return; // チュートリアル中は押せなくする
        }

        switch (BattleParam.getAutoPlayState())
        {
            case BattleParam.AutoPlayState.OFF:
                SoundUtil.PlaySE(SEID.SE_MENU_OK);
                BattleParam.setAutoPlayState(BattleParam.AutoPlayState.ON);
                break;

            case BattleParam.AutoPlayState.ON:
                SoundUtil.PlaySE(SEID.SE_MENU_OK);
                BattleParam.setAutoPlayState(BattleParam.AutoPlayState.OFF);
                break;
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	主人公スキル発動タッチ処理：
	*/
    //----------------------------------------------------------------------------
    public void OnHeroToutch()
    {
        // メニューオープンタイミングのチェック
        bool isOpen = InGameUtil.ChkInGameOpenWindowTiming();
        if (isOpen == false)
        {
            return;
        }
        // チュートリアル中の発動禁止処理.
        if (BattleParam.IsTutorial())
        {
            if (BattleSceneManager.Instance.isTutorialForbidLimitBreak(GlobalDefine.PartyCharaIndex.HERO))
            {
                return;
            }
        }

        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        if (cOption.m_OptionConfirmAS == (int)LocalSaveDefine.OptionConfirmAS.ON)
        {
            // ヒーロースキル発動確認ウィンドウを開く
            if (InGameMenuManagerQuest2.Instance != null)
            {
                InGameMenuManagerQuest2.Instance.OpenSkillMenu(GlobalDefine.PartyCharaIndex.HERO, 0, m_HeroTotalSkillPrv);
            }
        }
        else
        {
            // ヒーロースキルを即時発動
            BattleParam.RequestLBS(GlobalDefine.PartyCharaIndex.HERO);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	主人公情報長押しタッチ処理：
	*/
    //----------------------------------------------------------------------------
    public void OnHeroLongToutch()
    {
        // メニューオープンタイミングのチェック
        bool isOpen = InGameUtil.ChkInGameOpenWindowTiming();
        if (isOpen == false)
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
        SceneModeContinuousBattle.Instance.HeroWindowOpen();
    }

    public void setHeroScale(float scale)
    {
        m_InGamePartyManager.HeroScale = scale;
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
        if (m_HeroTotalSkillPrv == 0)
        {
            return true;
        }
        return false;
    }

    public void ASTurnInfo(bool sw)
    {
        if (sw == true)
        {
            m_InfoOpen = true;
            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {
                if (m_PartyUnit[i] != null)
                {
                    m_PartyUnit[i].openASTurnInfo();
                }
            }
            if (m_HeroTotalSkillPrv != 0)
            {
                m_InGamePartyManager.Hero_balloon1_active = true;
            }
            LocalSaveManager.Instance.LoadFuncOption().m_OptionBattleSkillTurn = (int)LocalSaveDefine.OptionBattleSkillTurn.ON; //TODO:表示切り替えをOPTIONで行うようになったら不要なので削除する
        }
        else
        {
            m_InfoOpen = false;
            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {
                if (m_PartyUnit[i] != null)
                {
                    m_PartyUnit[i].closeASTurnInfo();
                }
            }
            m_InGamePartyManager.Hero_balloon1_active = false;
            LocalSaveManager.Instance.LoadFuncOption().m_OptionBattleSkillTurn = (int)LocalSaveDefine.OptionBattleSkillTurn.OFF;    //TODO:表示切り替えをOPTIONで行うようになったら不要なので削除する
        }
    }

    public void NormalSkillInfo(bool sw)
    {
        if (sw == true)
        {
            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {
                if (m_PartyUnit[i] != null)
                {
                    m_PartyUnit[i].openNormalSkillInfo();
                }
            }
            LocalSaveManager.Instance.LoadFuncOption().m_OptionBattleSkillCost = (int)LocalSaveDefine.OptionBattleSkillCost.ON; //TODO:表示切り替えをOPTIONで行うようになったら不要なので削除する
        }
        else
        {
            for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; i++)
            {
                if (m_PartyUnit[i] != null)
                {
                    m_PartyUnit[i].closeNormalSkillInfo();
                }
            }
            LocalSaveManager.Instance.LoadFuncOption().m_OptionBattleSkillCost = (int)LocalSaveDefine.OptionBattleSkillCost.OFF;    //TODO:表示切り替えをOPTIONで行うようになったら不要なので削除する
        }
    }
}
