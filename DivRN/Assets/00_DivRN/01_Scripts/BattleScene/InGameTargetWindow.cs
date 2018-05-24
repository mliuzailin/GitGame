/*==========================================================================*/
/*!
    @file		InGameTargetWindow.cs
    @brief		インゲームターゲットウィンドウ
*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;


//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
    @class		InGameTargetWindow
    @brief		ターゲットウィンドウクラス
*/
//----------------------------------------------------------------------------
public class InGameTargetWindow : M4uContextMonoBehaviour
{
    public GameObject m_UIInstanceTargetWindow = null;
    public GameObject m_UIInstanceTargetWindowPosition = null;
    public GameObject m_UIInstanceTargetWindowCollision = null;
    public GameObject m_UIInstanceTargetWindowCharaIcon = null;

    private const string TEXTKEY_ITEM_00 = "";
    private const string TEXTKEY_ITEM_01 = "";
    private const string TEXTKEY_ITEM_02 = "";
    private const string TEXTKEY_ITEM_03 = "";
    private const string TEXTKEY_ITEM_04 = "";

    private const int INFO_LIST_BASE = 0;				//!< 情報：基本オブジェクト
    private const int INFO_LIST_AILMENT = 1;				//!< 情報：状態異常オブジェクト
    private const int INFO_LIST_ABILITY = 2;				//!< 情報：特性オブジェクト
    private const int INFO_LIST_MAX = 3;				//!< 情報

    private const int AILMENT_LABEL_EMPTY = 0;				//!< 情報：状態異常：EMPTY文言
    private const int AILMENT_LABEL_NAME = 1;				//!< 情報：状態異常：名称
    private const int AILMENT_LABEL_EXPLAN = 2;				//!< 情報：状態異常：説明
    private const int AILMENT_LABEL_TURN = 3;				//!< 情報：状態異常：ターン
    private const int AILMENT_LABEL_MAX = 4;				//!< 情報：状態異常

    private const int ABILITY_LABEL_EMPTY = 0;				//!< 情報：特性：EMPTY文言
    private const int ABILITY_LABEL = 1;				//!< 情報：特性：ラベル
    private const int ABILITY_LABEL_MAX = 2;				//!< 情報：特性

    private const int ABILITY_DRAGGABLEPANEL_ENABLE_LINE = 7;		//!< UIDraggablePanelスクロールONにする特性文字列行数。

#if true
    private const float AILMENT_BUTTON_BASE_X = (-220.0f);	//!< 状態異常ボタン：基準位置X
    private const float AILMENT_BUTTON_BASE_Y = (102.0f);	//!< 状態異常ボタン：基準位置Y
    private const float AILMENT_BUTTON_BASE_Z = (-100.0f);	//!< 状態異常ボタン：基準位置Z
    private const float AILMENT_SPACE_VALUE_X = (110.0f);	//!< 状態異常ボタン：次のボタンまでの間隔X
    private const float AILMENT_SPACE_VALUE_Y = (-86.5f);	//!< 状態異常ボタン：次のボタンまでの間隔Y
    private const int AILMENT_BUTTON_IN_LINE = (5);	//!< 状態異常ボタン：１行に表示するアイコンの数。
#else
    private const float				AILMENT_BUTTON_BASE_X	= ( -262.0f );		//!< 状態異常ボタン：基準位置X
    private const float				AILMENT_BUTTON_BASE_Y	= ( -105.0f	);		//!< 状態異常ボタン：基準位置Y
    private const float				AILMENT_BUTTON_BASE_Z	= ( -100.0f	);		//!< 状態異常ボタン：基準位置Z
    private const float				AILMENT_SPACE_VALUE_X	= ( 58.0f	);		//!< 状態異常ボタン：次のボタンまでの間隔X
#endif


    public enum ESTATE
    {
        eSTATE_NULL = 0,														//!< 状態：無効。
        eSTATE_PAGE_BASIC,														//!< 状態：基本情報。
        eSTATE_PAGE_STATUS,														//!< 状態：状態異常情報。
        eSTATE_PAGE_ABILITY,													//!< 状態：特性情報。
    };


    private int m_TargetID = InGameDefine.SELECT_NONE;	//!< キャラID
    private GameObject m_Window;									//!< ウィンドウ
    private GameObject m_WindowPos;								//!<
    private GameObject m_Collision;								//!< コリジョン
    private GameObject m_CharaIcon;
    private GameObject m_ButtonTargetOff = null;
    private GameObject m_ButtonBaseInfo = null;
    private GameObject m_ButtonAbilityInfo = null;
    private GameObject m_ButtonStatusInfo = null;
    private GameObject[] m_InfoList = new GameObject[INFO_LIST_MAX];                              //!< 情報：要素一覧
    M4uProperty<bool> empty_label = new M4uProperty<bool>();
    public bool Empty_label { get { return empty_label.Value; } set { empty_label.Value = value; } }
    M4uProperty<string> ailment_name = new M4uProperty<string>();
    public string Ailment_name { get { return ailment_name.Value; } set { ailment_name.Value = value; } }
    M4uProperty<string> ailment_explan = new M4uProperty<string>();
    public string Ailment_explan { get { return ailment_explan.Value; } set { ailment_explan.Value = value; } }
    M4uProperty<string> ailment_turn = new M4uProperty<string>();
    public string Ailment_turn { get { return ailment_turn.Value; } set { ailment_turn.Value = value; } }
    M4uProperty<bool> turn_active = new M4uProperty<bool>();
    public bool Turn_active { get { return turn_active.Value; } set { turn_active.Value = value; } }
    private GameObject[] m_AilmentButtonSet = new GameObject[StatusAilmentChara.get_STATUSAILMENT_MAX()];	//!< 状態異常ボタンセット
    private GameObject[] m_AilmentIcon = new GameObject[StatusAilmentChara.get_STATUSAILMENT_MAX()];	//!< 状態異常アイコン
    private GameObject[] m_AilmentBalloon = new GameObject[StatusAilmentChara.get_STATUSAILMENT_MAX()];	//!< 状態異常バルーン
    private GameObject[] m_AilmentBalloonText = new GameObject[StatusAilmentChara.get_STATUSAILMENT_MAX()];	//!< 状態異常バルーンテキスト
    private GameObject[] m_AilmentButton = new GameObject[StatusAilmentChara.get_STATUSAILMENT_MAX()];	//!< 状態異常ボタン一覧
    private GameObject[] m_AbilityDetailObj = new GameObject[ABILITY_LABEL_MAX];							//!< 特性：説明パーツ
    private GameObject m_AbilityDraggablePanel = null;                                                         //!< 特性：NGUIのDraggablePanel
    M4uProperty<string> caption = new M4uProperty<string>();
    public string Caption { get { return caption.Value; } set { caption.Value = value; } }
    M4uProperty<string> item_race = new M4uProperty<string>();
    public string Item_race { get { return item_race.Value; } set { item_race.Value = value; } }
    M4uProperty<string> item_race_title = new M4uProperty<string>();
    public string Item_race_title { get { return item_race_title.Value; } set { item_race_title.Value = value; } }
    M4uProperty<string> item_type = new M4uProperty<string>();
    public string Item_type { get { return item_type.Value; } set { item_type.Value = value; } }
    M4uProperty<string> item_type_title = new M4uProperty<string>();
    public string Item_type_title { get { return item_type_title.Value; } set { item_type_title.Value = value; } }
    M4uProperty<string> item_week = new M4uProperty<string>();
    public string Item_week { get { return item_week.Value; } set { item_week.Value = value; } }
    M4uProperty<string> item_week_title = new M4uProperty<string>();
    public string Item_week_title { get { return item_week_title.Value; } set { item_week_title.Value = value; } }
    M4uProperty<string> item_rare = new M4uProperty<string>();
    public string Item_rare { get { return item_rare.Value; } set { item_rare.Value = value; } }
    M4uProperty<string> item_rare_title = new M4uProperty<string>();
    public string Item_rare_title { get { return item_rare_title.Value; } set { item_rare_title.Value = value; } }
    M4uProperty<Sprite> chara_icon = new M4uProperty<Sprite>();
    public Sprite Chara_icon { get { return chara_icon.Value; } set { chara_icon.Value = value; } }
    StatusAilmentChara m_AilmentChara = null;
    private int[] m_AilmentStatusID = new int[StatusAilmentChara.get_STATUSAILMENT_MAX()];	//!< 表示している状態異常ID
    private int m_TouchButton = -1;														//!< 選択中のボタン
    private bool m_SetUpInfo = false;													//!< 表示準備フラグ
    private GameObject m_cMaskCollision;							//!< 背景マスクのコリジョン。
    private GameObject m_cMaskCollisionWindowBelow;				//!< 背景マスクのコリジョン。ウィンドウの下側に置くやつ。

    private GameObject m_DragNGUIDragPanelObj = null;				//!< NGUIのDraggablePanel監視関連：スクロール用

    private Color m_ColorWhite;
    private Color m_ColorGray;
    private Color m_ColorGrayTab;
    private Color m_ColorRed;
    private Color m_ColorLightBlue;

    private ESTATE m_eState = ESTATE.eSTATE_NULL;

    private bool m_bTouchReleaseAfterWindowOpen = false;

    private Vector3 m_DispPos;                                              //!< 表示する位置

    void Awake()
    {
        gameObject.GetComponent<M4uContextRoot>().Context = this;
    }
    void Start()
    {
        m_Window = m_UIInstanceTargetWindow;
        m_WindowPos = m_UIInstanceTargetWindowPosition;
        m_Collision = m_UIInstanceTargetWindowCollision;
        m_CharaIcon = m_UIInstanceTargetWindowCharaIcon;
        UnityUtil.SetObjectEnabledOnce(m_WindowPos, true);
        UnityUtil.SetObjectEnabledOnce(m_Collision, true);
        UnityUtil.SetObjectEnabledOnce(this.gameObject, false);

        // 各種ゲームオブジェクトを取得(アクセス用)
        m_InfoList[INFO_LIST_BASE] = UnityUtil.GetChildNode(m_WindowPos, "InfoBase");
        m_InfoList[INFO_LIST_AILMENT] = UnityUtil.GetChildNode(m_WindowPos, "InfoAilment");
        m_InfoList[INFO_LIST_ABILITY] = UnityUtil.GetChildNode(m_WindowPos, "InfoAbility");

        m_ButtonTargetOff = UnityUtil.GetChildNode(m_InfoList[INFO_LIST_BASE], "button:target_off");
        m_ButtonBaseInfo = UnityUtil.GetChildNode(UnityUtil.GetChildNode(m_WindowPos, "tab_basic"), "Collision");
        m_ButtonStatusInfo = UnityUtil.GetChildNode(UnityUtil.GetChildNode(m_WindowPos, "tab_status"), "Collision");
        m_ButtonAbilityInfo = UnityUtil.GetChildNode(UnityUtil.GetChildNode(m_WindowPos, "tab_ability"), "Collision");

        m_AbilityDraggablePanel = UnityUtil.GetChildNode(m_InfoList[INFO_LIST_ABILITY], "UIDraggablePanel");

        // ラベルを設定
        UnityUtil.SetUILabelValue(UnityUtil.GetChildNode(m_InfoList[INFO_LIST_BASE], "item:target"), GameTextUtil.GetText("BTN_TARGET"));
        UnityUtil.SetUILabelValue(m_ButtonTargetOff, GameTextUtil.GetText("BTN_OFF"));

        // GraggablePanel(スクロール要素)を取得
        m_DragNGUIDragPanelObj = UnityUtil.GetChildNode(m_InfoList[INFO_LIST_ABILITY], "MsgList");

        // 表示OFF
        UnityUtil.SetObjectEnabledOnce(m_InfoList[INFO_LIST_AILMENT], false);

        // カラーを取得
        m_ColorWhite = ColorUtil.GetColor(APP_COLOR.LABEL_WHITE);
        m_ColorGray = ColorUtil.GetColor(APP_COLOR.LABEL_GRAY);
        m_ColorGrayTab = ColorUtil.GetColor(APP_COLOR.LABEL_GRAY_TAB);
        m_ColorRed = ColorUtil.GetColor(APP_COLOR.LABEL_RED);
        m_ColorLightBlue = ColorUtil.GetColor(APP_COLOR.LABEL_LIGHT_BLUE);

        // ウィンドウ外コリジョンを取得。
        m_cMaskCollision = UnityUtil.GetChildNode(m_Window, "collision");
        m_cMaskCollisionWindowBelow = UnityUtil.GetChildNode(m_Window, "collision_window_below");

        // 表示位置を画面外へ(ダイアログ初回のみ、表示物が初期値のまま表示されてしまうため)
        m_DispPos = gameObject.transform.localPosition;	// 表示位置を保存
        gameObject.transform.localPosition = new Vector3(1000.0f, 1.0f, 1.0f);	// 表示情報が確定するまで見られたくないので、画面の遠くへ配置

        //-----------------------------------------------------------------
        // タブをグレーアウト。
        //-----------------------------------------------------------------
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_basic"), m_ColorGrayTab);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_basic"), "Text"), m_ColorGray);
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_status"), m_ColorGrayTab);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_status"), "Text"), m_ColorGray);
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_ability"), m_ColorGrayTab);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_ability"), "Text"), m_ColorGray);
    }

    void Update()
    {
        //----------------------------------------
        // インスタンスのチェック
        //----------------------------------------
        if (SceneObjReferGameMain.Instance == null)
        {
            return;
        }

        if (m_Window == null
        || m_WindowPos == null
        || m_CharaIcon == null
        //		||   m_Collision   == null
        //		||   m_Caption     == null
        //		||   m_Race        == null
        //		||   m_Type        == null
        //		||   m_Week        == null
        //		||   m_Rare        == null
        //		||   m_LabelTarget == null
        )
        {
            return;
        }

        //----------------------------------------
        // ターゲットマークがついている敵なのかチェック
        //----------------------------------------
        bool isTarget = false;
        if (BattleParam.m_TargetEnemyWindow == BattleParam.m_TargetEnemyCurrent)
        {
            isTarget = true;
        }

        //----------------------------------------
        // ターゲットウィンドウに表示する対象が設定されているかチェック
        //----------------------------------------
        if (m_TargetID == InGameDefine.SELECT_NONE)
        {
            //----------------------------------------
            // ターゲットウィンドウを閉じている時の処理
            //----------------------------------------
            m_TargetID = BattleParam.m_TargetEnemyWindow;
            if (m_TargetID != InGameDefine.SELECT_NONE
            && m_SetUpInfo == false)
            {
                BattleEnemy enemy = BattleParam.m_EnemyParam[m_TargetID];
                if (enemy != null)
                {
                    MasterDataParamChara charaParam = enemy.getMasterDataParamChara();
                    if (charaParam != null)
                    {
                        // 名前
                        Caption = charaParam.name;
                        // 種族
                        Item_race_title = GameTextUtil.GetText("WORD_UNIT_KIND");
                        Item_race = GameTextUtil.GetKindToText(enemy.getKind(), enemy.getKindSub());
                        // 属性
                        Item_type_title = GameTextUtil.GetText("WORD_UNIT_ELEMENT");
                        Item_type = GameTextUtil.GetText(GameTextUtil.GetElemToTextKey(charaParam.element));
                        // 弱点
                        Item_week_title = GameTextUtil.GetText("WORD_UNIT_WEEK");
                        MasterDataDefineLabel.ElementType week = InGameUtilBattle.GetElementAffinity(charaParam.element);
                        Item_week = GameTextUtil.GetText(GameTextUtil.GetElemToTextKey(week));
                        // レア
                        Item_rare_title = GameTextUtil.GetText("WORD_RARE_JP");
                        Item_rare = GameTextUtil.GetText(GameTextUtil.GetRareToTextKey(charaParam.rare));

                        // 状態異常アイコンの設定
                        SetAilmentIcon(enemy);

                        // キャラアイコンの設定
                        UnitIconImageProvider.Instance.Get(
                            charaParam.fix_id,
                            sprite => { Chara_icon = sprite; });
                    }

                    // 特性情報の設定
                    SetAbilityInformation(enemy);

                    // ターゲットボタンの状態を設定
                    SetTargetButton(isTarget);

                    // ウィンドウの有効化
                    UnityUtil.SetObjectEnabledOnce(m_WindowPos, true);

                    m_SetUpInfo = true;

                    // 表示位置へ移動
                    gameObject.transform.localPosition = m_DispPos;

                    m_bTouchReleaseAfterWindowOpen = false;

                    //-----------------------------------------------------
                    // 初期状態：基本情報から開始。
                    //-----------------------------------------------------
                    ChangeState(ESTATE.eSTATE_PAGE_BASIC);
                    return;
                }
            }
        }
        else
        {
            //----------------------------------------
            // ターゲットウィンドウを開いている時の処理
            //----------------------------------------


            //-------------------------------------------------------------
            // 閉じるボタン押下によるウィンドウ閉じる処理。
            //-------------------------------------------------------------
            bool dispChange = false;

            // 閉じる。
            if (dispChange == true)
            {
                //---------------------------------------------------------
                // ページ情報クリアのため、状態解除。
                //---------------------------------------------------------
                ChangeState(ESTATE.eSTATE_NULL);

                UnityUtil.SetUILabelValue(m_AbilityDetailObj[ABILITY_LABEL], "");		// ここでクリアしておかないと、別の敵でスクロールバーの表示判定がうまく機能しない
                m_SetUpInfo = false;
                m_TargetID = InGameDefine.SELECT_NONE;
                UnityUtil.SetObjectEnabledOnce(m_WindowPos, false);
                UnityUtil.SetObjectEnabledOnce(m_Collision, false);
                UnityUtil.SetObjectEnabledOnce(this.gameObject, false);
                return;
            }


            //-------------------------------------------------------------
            // 状態更新。
            //-------------------------------------------------------------
            switch (m_eState)
            {
                case ESTATE.eSTATE_PAGE_BASIC: State_PageBasicUpdate(); break;
                case ESTATE.eSTATE_PAGE_STATUS: State_PageStatusAilmentUpdate(); break;
                case ESTATE.eSTATE_PAGE_ABILITY: State_PageAbilityUpdate(); break;
            }

        }
    }


    //------------------------------------------------------------------------
    /*!
        @brief		状態異常アイコンの設定
        @param[in]	BattleEnemy	(enemy)		対象の敵
    */
    //------------------------------------------------------------------------
    private void SetAilmentIcon(BattleEnemy enemy)
    {
        // 初めに全てクリア
        m_AilmentChara = null;
        for (int num = 0; num < m_AilmentButtonSet.Length; ++num)
        {
            m_AilmentStatusID[num] = 0;
            UnityUtil.SetObjectEnabledOnce(m_AilmentButtonSet[num], false);
        }

        // 状態異常管理を取得
        m_AilmentChara = enemy.m_StatusAilmentChara;
        //----------------------------------------
        // エラーチェック
        //----------------------------------------
        if (m_AilmentChara == null)
        {
            return;
        }


        string spriteName = "";
        int count = 0;
        int turn = 0;
        bool empty = true;

        for (int num = 0; num < (int)MasterDataDefineLabel.AilmentType.MAX; ++num)
        {
            if (m_AilmentButtonSet[count] == null
            || m_AilmentIcon[count] == null
            || m_AilmentBalloon[count] == null
            || m_AilmentBalloonText[count] == null
            || m_AilmentChara.GetStatus()[num] == false)
            {
                continue;
            }

            // Sprite名を取得
            spriteName = StatusAilmentIconUtil.GetResourceName((MasterDataDefineLabel.AilmentType)Enum.ToObject(typeof(MasterDataDefineLabel.AilmentType), num));
            if (spriteName == "")
            {
                continue;
            }

            // Sprite切替
            //UnityUtil.SetUIAtlasSprite( m_AilmentIcon[ count ], spriteName );

            // 残りターン数を取得
            for (int n = 0; n < m_AilmentChara.GetAilmentCount(); ++n)
            {
                if (m_AilmentChara.GetAilment(n) == null
                || m_AilmentChara.GetAilment(n).bUsed == false)
                {
                    continue;
                }

                int type = (int)m_AilmentChara.GetAilment(n).nType;
                if (type != num)
                {
                    continue;
                }

                // 状態異常IDを取得
                m_AilmentStatusID[count] = n;

                // 残りターン数を取得
                turn = m_AilmentChara.GetAilment(m_AilmentStatusID[count]).nLife;

                // 残りターン数が、0でない場合
                if (turn > 0)
                {
                    // 表示
                    UnityUtil.SetUILabelValue(m_AilmentBalloonText[count], turn.ToString("00"));

                    UnityUtil.SetObjectEnabledOnce(m_AilmentBalloon[count], true);		// バルーン
                    UnityUtil.SetObjectEnabledOnce(m_AilmentBalloonText[count], true);		// バルーンテキスト
                }
                else
                {
                    // 非表示
                    UnityUtil.SetObjectEnabledOnce(m_AilmentBalloon[count], false);	// バルーン
                    UnityUtil.SetObjectEnabledOnce(m_AilmentBalloonText[count], false);	// バルーンテキスト
                }

            }

            // EMPTYフラグOFF
            empty = false;

            // 表示ON
            UnityUtil.SetObjectEnabledOnce(m_AilmentButtonSet[count], true);

            ++count;
            // 表示限界チェック
            if (count >= m_AilmentButtonSet.Length)
            {
                break;
            }

        }

        // EMPTY文言表示有無
        if (empty == true)
        {
            Empty_label = true;
        }
        else
        {
            Empty_label = false;
        }
    }


    //------------------------------------------------------------------------
    /*!
        @brief		ボタン切り替え確認
    */
    //------------------------------------------------------------------------
    private void CheckSwitchButton()
    {
    }


    //------------------------------------------------------------------------
    /*!
        @brief		ターゲットボタンの状態を設定
    */
    //------------------------------------------------------------------------
    private void SetTargetButton(bool isTarget)
    {
        // ターゲット状態に応じてボタンを変化させる
        if (isTarget == true)
        {
            // 選択ボタンの明暗
            UnityUtil.SetUILabelColor(m_ButtonTargetOff, m_ColorGray);
            UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(m_ButtonTargetOff, "Image"), m_ColorGray);
        }
        else
        {
            // 選択ボタンの明暗
            UnityUtil.SetUILabelColor(m_ButtonTargetOff, m_ColorWhite);
            UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(m_ButtonTargetOff, "Image"), m_ColorWhite);
        }

    }

    //------------------------------------------------------------------------
    /*!
        @brief		特性情報を表示
    */
    //------------------------------------------------------------------------
    private void SetAbilityInformation(BattleEnemy enemy)
    {
        MasterDataEnemyAbility abilityMaster = null;
        string abilityLabel = "";

        // 特性を配列化
        uint[] enemyAbility = enemy.getEnemyAbilitys();

        //--------------------------------
        // ラベルをクリア(正常動作の場合、上書きされるのでクリアしなくてもいいが念のため)
        //--------------------------------
        UnityUtil.SetUILabelValue(m_AbilityDetailObj[ABILITY_LABEL], "");

        //--------------------------------
        // 所持特性を全チェック
        //--------------------------------
        int nLineCnt = 0;
        for (int num = 0; num < enemyAbility.Length; ++num)
        {
            if (enemyAbility[num] == 0)
            {
                continue;
            }

            // 特性マスターを取得
            abilityMaster = MasterDataUtil.GetEnemyAbilityParamFromID((int)enemyAbility[num]);
            if (abilityMaster == null)
            {
                continue;
            }

            // 表示文を取得
            if (abilityMaster.name != "")
            {
                abilityLabel += abilityMaster.name + "\r\n";
                ++nLineCnt;
            }
            if (abilityMaster.detail != "")
            {
                abilityLabel += abilityMaster.detail + "\r\n";
                ++nLineCnt;
                if (abilityMaster.detail.Contains("\r\n"))
                {
                    ++nLineCnt;
                }
            }
        }

        // 特性情報ボタンの有効無効チェック
        if (abilityLabel != "")
        {
            // 末尾の改行コードを削除
            abilityLabel = abilityLabel.Substring(0, (abilityLabel.Length - 2));

            // ラベルを設定
            UnityUtil.SetUILabelValue(m_AbilityDetailObj[ABILITY_LABEL], abilityLabel);

            // EMPTY文言非表示
            UnityUtil.SetObjectEnabledOnce(m_AbilityDetailObj[AILMENT_LABEL_EMPTY], false);
#if false
            // ボタンを有効化
            UnityUtil.SetObjectEnabledOnce( m_ButtonAbilityInfo, true );
#endif

            //#if true
            //    // 一定行数以上であれば、スクロールバーの表示をON。
            //    UIDraggablePanel cUIDraggablePanel = m_DragNGUIDragPanelObj.GetComponent( typeof(UIDraggablePanel) ) as UIDraggablePanel;
            //    if( null != cUIDraggablePanel )
            //    {
            //        cUIDraggablePanel.enabled = ( nLineCnt > ABILITY_DRAGGABLEPANEL_ENABLE_LINE );
            //    };
            //#else
            //    // スクロールロック判定(250は、MsgListのUIPanelサイズ)
            //    UnityUtil.SetDraggablePanelAutoLockUILabelHeight( m_DragNGUIDragPanelObj, m_AbilityDetailObj[ ABILITY_LABEL ], 250 );
            //#endif

            //スクロール設定
            //			UnityUtil.SetDraggablePanelAutoLockUILabelHeight(m_DragNGUIDragPanelObj, m_AbilityDetailObj[ABILITY_LABEL], 250, false);
        }
        else
        {
            // EMPTY文言表示
            UnityUtil.SetObjectEnabledOnce(m_AbilityDetailObj[AILMENT_LABEL_EMPTY], true);
        }
#if false
        else
        {
            // ボタンを無効化
            UnityUtil.SetObjectEnabledOnce( m_ButtonAbilityInfo, false );
        }
#endif
    }


    #region <STATE>
    //---------------------------------------------------------------------
    /**
     *	@brief	状態遷移。
     */
    //---------------------------------------------------------------------
    private void ChangeState(ESTATE eState)
    {
        if (eState == m_eState)
        {
            return;
        }


        //-----------------------------------------------------------------
        // 現在の状態退場。
        //-----------------------------------------------------------------
        switch (m_eState)
        {
            case ESTATE.eSTATE_PAGE_BASIC: State_PageBasicExit(); break;
            case ESTATE.eSTATE_PAGE_STATUS: State_PageStatusAilmentExit(); break;
            case ESTATE.eSTATE_PAGE_ABILITY: State_PageAbilityExit(); break;
        }


        //-----------------------------------------------------------------
        // 状態変数更新。
        //-----------------------------------------------------------------
        m_eState = eState;


        //-----------------------------------------------------------------
        // 新しい状態入場。
        //-----------------------------------------------------------------
        switch (m_eState)
        {
            case ESTATE.eSTATE_PAGE_BASIC: State_PageBasicEnter(); break;
            case ESTATE.eSTATE_PAGE_STATUS: State_PageStatusAilmentEnter(); break;
            case ESTATE.eSTATE_PAGE_ABILITY: State_PageAbilityEnter(); break;
        }
    }


    //---------------------------------------------------------------------
    /**
     *	@brief	状態：基本情報。
     */
    //---------------------------------------------------------------------
    private void State_PageBasicEnter()
    {
        //-----------------------------------------------------------------
        // 基本情報表示ON。
        //-----------------------------------------------------------------
        UnityUtil.SetObjectEnabledOnce(m_InfoList[INFO_LIST_BASE], true);
        //-----------------------------------------------------------------
        // タブのハイライト切り替え。
        //-----------------------------------------------------------------
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_basic"), m_ColorWhite);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_basic"), "Text"), m_ColorWhite);
    }
    //---------------------------------------------------------------------
    private void State_PageBasicExit()
    {
        //-----------------------------------------------------------------
        // 基本情報表示OFF。
        //-----------------------------------------------------------------
        UnityUtil.SetObjectEnabledOnce(m_InfoList[INFO_LIST_BASE], false);
        //-----------------------------------------------------------------
        // タブのハイライト切り替え。
        //-----------------------------------------------------------------
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_basic"), m_ColorGrayTab);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_basic"), "Text"), m_ColorGray);
    }
    //---------------------------------------------------------------------
    private void State_PageBasicUpdate()
    {
    }


    //---------------------------------------------------------------------
    /**
     *	@brief	状態：状態異常情報。
     */
    //---------------------------------------------------------------------
    private void State_PageStatusAilmentEnter()
    {
        //-----------------------------------------------------------------
        // 状態異常情報表示ON。
        //-----------------------------------------------------------------
        UnityUtil.SetObjectEnabledOnce(m_InfoList[INFO_LIST_AILMENT], true);
        //-----------------------------------------------------------------
        // タブのハイライト切り替え。
        //-----------------------------------------------------------------
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_status"), m_ColorWhite);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_status"), "Text"), m_ColorWhite);
        //-----------------------------------------------------------------
        // 状態異常説明テキストクリア。
        //-----------------------------------------------------------------
        Ailment_name = "";
        Ailment_explan = "";
        Ailment_turn = "";
    }
    //---------------------------------------------------------------------
    private void State_PageStatusAilmentExit()
    {
        //-----------------------------------------------------------------
        // 状態異常情報表示OFF。
        //-----------------------------------------------------------------
        UnityUtil.SetObjectEnabledOnce(m_InfoList[INFO_LIST_AILMENT], false);
        //-----------------------------------------------------------------
        // タブのハイライト切り替え。
        //-----------------------------------------------------------------
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_status"), m_ColorGrayTab);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_status"), "Text"), m_ColorGray);

        //-----------------------------------------------------------------
        // 選択中の状態異常ボタンを暗くする。
        //-----------------------------------------------------------------
        if (m_TouchButton >= 0)
        {
            UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(m_AilmentButton[m_TouchButton], "Icon"), m_ColorGray);
            // @change Developer v300 UI変更により削除
            //			UnityUtil.SetUISpriteColor( UnityUtil.GetChildNode( m_AilmentButton[ m_TouchButton ], "Back" ), m_ColorGray );

            // 選択中の状態異常ボタンのクリア
            m_TouchButton = -1;
        }
    }
    //---------------------------------------------------------------------
    private void State_PageStatusAilmentUpdate()
    {
    }


    //---------------------------------------------------------------------
    /**
     *	@brief	状態：特性情報。
     */
    //---------------------------------------------------------------------
    private void State_PageAbilityEnter()
    {
        //-----------------------------------------------------------------
        // 特性報表示ON。
        //-----------------------------------------------------------------
        UnityUtil.SetObjectEnabledOnce(m_InfoList[INFO_LIST_ABILITY], true);
        //-----------------------------------------------------------------
        // タブのハイライト切り替え。
        //-----------------------------------------------------------------
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_ability"), m_ColorWhite);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_ability"), "Text"), m_ColorWhite);
    }
    //---------------------------------------------------------------------
    private void State_PageAbilityExit()
    {
        //-----------------------------------------------------------------
        // 特性情報表示OFF。
        //-----------------------------------------------------------------
        UnityUtil.SetObjectEnabledOnce(m_InfoList[INFO_LIST_ABILITY], false);
        //-----------------------------------------------------------------
        // タブのハイライト切り替え。
        //-----------------------------------------------------------------
        UnityUtil.SetUISpriteColor(UnityUtil.GetChildNode(gameObject, "tab_ability"), m_ColorGrayTab);
        UnityUtil.SetUILabelColor(UnityUtil.GetChildNode(UnityUtil.GetChildNode(gameObject, "tab_ability"), "Text"), m_ColorGray);
        //-----------------------------------------------------------------
        // 特性情報スクロール状態をリセット。
        //-----------------------------------------------------------------
        //UnityUtil.SetDraggablePanelReset( m_DragNGUIDragPanelObj );
    }
    //---------------------------------------------------------------------
    private void State_PageAbilityUpdate()
    {
    }
    #endregion // <STATE>

    public void OnCancel()
    {
        //---------------------------------------------------------
        // ページ情報クリアのため、状態解除。
        //---------------------------------------------------------
        ChangeState(ESTATE.eSTATE_NULL);

        //		UnityUtil.SetUILabelValue(m_AbilityDetailObj[ABILITY_LABEL], "");       // ここでクリアしておかないと、別の敵でスクロールバーの表示判定がうまく機能しない
        m_SetUpInfo = false;
        m_TargetID = InGameDefine.SELECT_NONE;
        UnityUtil.SetObjectEnabledOnce(m_WindowPos, false);
        //		UnityUtil.SetObjectEnabledOnce(m_Collision, false);
        UnityUtil.SetObjectEnabledOnce(this.gameObject, false);
    }

    public void OnBasicInfo()
    {
        if (ESTATE.eSTATE_PAGE_BASIC != m_eState)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            ChangeState(ESTATE.eSTATE_PAGE_BASIC);
        }
    }

    public void OnStatusAilment()
    {
        if (ESTATE.eSTATE_PAGE_STATUS != m_eState)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            ChangeState(ESTATE.eSTATE_PAGE_STATUS);
        }
    }

    public void OnAbility()
    {
        if (ESTATE.eSTATE_PAGE_ABILITY != m_eState)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            ChangeState(ESTATE.eSTATE_PAGE_ABILITY);
        }
    }

    public void OnTargetOff()
    {
        SetTargetButton(false);
    }

    public void OnTargetOn()
    {
        SetTargetButton(true);
    }
}; // class InGameTargetWindow

