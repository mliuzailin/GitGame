/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	BattleManager.cs
    @brief	バトル管理クラス
    @author Developer
    @date 	2012/10/08
*/

//	用語メモ：一般に「ノーマルスキル」と呼ばれているものはプログラム内では AvticeSkill, SkillActive 等と記述されています.
//	用語メモ：一般に「アクティブスキル」と呼ばれているものはプログラム内では LimitBreakSkill, SkillLimitBreak, LBS 等と記述されています.
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;

using MyTweenExt;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/**
 *	バトル管理クラス
 */
//----------------------------------------------------------------------------
public class BattleSceneManager : SingletonComponent<BattleSceneManager>
{
    public enum BattleScenePhase
    {
        NOT_BATTLE,
        IN_BATTLE,
    }

    public Camera m_BattleCameraFar = null;
    public Camera m_BattleCameraNear = null;

    public GameObject m_PartsGroup1 = null;
    public GameObject m_PartsGroup2 = null;

    public GameObject m_BattleCardArea = null;
    public GameObject m_SkillIconArea = null;
    public GameObject m_CountDownArea = null;
    public GameObject m_CountFinishArea = null;
    public GameObject m_CaptionArea = null;
    public GameObject m_TextCutinArea = null;
    public GameObject m_EnemyArea = null;
    public GameObject m_ComboArea = null;
    public GameObject m_ComboFinishArea = null;
    public GameObject m_PlayerPartyView = null;

    public GameObject m_UICanvas = null;

    public GameObject m_CardMask = null;
    public GameObject m_UIMask = null;

    public GameObject m_TopRightAnchor = null;
    public GameObject m_BottomRightAnchor = null;

    public GameObject m_EnemyDetailWindowPrefab = null;

    private BattleLogic m_BattleLogic = null;
    private BattleScenePhase m_BattleScenePhase = BattleScenePhase.NOT_BATTLE;
    private bool m_IsChangeBattleScenePhase = false;

    private GameObject m_EnemyDetailWindowObject = null;
    private bool m_IsShowEnemyDetailWindow = false;

    private bool m_IsShowHandArea = false;

    private int m_InputEnableFrameCounter = 0;	// 操作可能になってからの経過フレーム数
    private int m_GuideDispMode = -1;
    private Transform m_GuideDispTransform = null;

    private BattleTutorialManager m_BattleTutorialManager = null;


    private GameObject m_OverrideTouchHand = null;  // 操作乗っ取り時に表示される手の画像
    private bool m_IsOverrideTouchMode = false;
    private Vector2 m_OverrideTouchHandPosition;

    private BattleAutoPlay m_BattleAutoPlay = new BattleAutoPlay(); // オートプレイ
    public BattleAutoPlay AutoPlay
    {
        get
        {
            return m_BattleAutoPlay;
        }
    }

    // Unity関数.
    protected override void Awake()
    {
        base.Awake();
        BattleTouchInput battle_touch_input = GetComponent<BattleTouchInput>();
        if (battle_touch_input != null)
        {
            battle_touch_input.setCamera(m_BattleCameraNear);
        }

        m_EnemyArea.GetComponent<BattleDispEnemy>().init(m_BattleCameraNear, m_UICanvas.GetComponent<Canvas>());

        m_BattleLogic = GetComponent<BattleLogic>();
        m_BattleLogic.init(
            m_BattleCardArea.GetComponent<BattleCardArea>().m_BattleCardManager,
            m_SkillIconArea.GetComponent<SkillIconArea>(),
            m_CountDownArea.GetComponent<CountDownArea>(),
            m_CountFinishArea.GetComponent<CountFinishArea>(),
            m_CaptionArea.GetComponent<BattleCaptionControl>(),
            m_TextCutinArea.GetComponent<TextCutinViewControl>(),
            m_EnemyArea.GetComponent<BattleDispEnemy>(),
            m_ComboArea.GetComponent<BattleComboArea>(),
            m_ComboFinishArea.GetComponent<BattleComboFinishArea>(),
            m_PlayerPartyView.GetComponent<BattlePlayerPartyViewControl>()
        );

        m_BattleTutorialManager = new BattleTutorialManager();

        m_BattleAutoPlay.init(m_BattleCardArea.GetComponent<BattleCardArea>().m_BattleCardManager);
    }

    protected override void Start()
    {
        base.Start();

        if (SafeAreaControl.HasInstance)
        {
            RectTransform rect = m_CardMask.GetComponent<RectTransform>();
            if (rect != null)
            {
                float bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
                if (bottom_space_height > 0)
                {
                    rect.AddLocalPositionY((bottom_space_height / 2) * -1);
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x,
                                                    rect.sizeDelta.y + bottom_space_height);

                }
            }
        }

        SceneCommon.initalizeGameFps();

        m_PartsGroup1.SetActive(false);
        m_PartsGroup2.SetActive(false);
        m_BattleScenePhase = BattleScenePhase.NOT_BATTLE;
        m_IsChangeBattleScenePhase = true;

        {
            float camera_fov = m_BattleCameraNear.fieldOfView;
            Quaternion camera_rot = m_BattleCameraNear.transform.localRotation;
            BattleSceneUtil.getAdjustScreenInfo(ref camera_fov, ref camera_rot);

            m_BattleCameraNear.fieldOfView = camera_fov;
            m_BattleCameraNear.transform.localRotation = camera_rot;

            m_BattleCameraFar.fieldOfView = camera_fov;
            m_BattleCameraFar.transform.localRotation = camera_rot;
        }

        m_GuideDispTransform = m_UICanvas.transform.Find("ImageGuide");

        GameObject teacher_hand_prefab = Resources.Load<GameObject>("Prefab/BattleScene/TutorialHand");
        if (teacher_hand_prefab != null)
        {
            m_OverrideTouchHand = GameObject.Instantiate(teacher_hand_prefab);
            if (m_OverrideTouchHand != null)
            {
                m_OverrideTouchHand.transform.SetParent(m_UICanvas.transform, false);
                m_OverrideTouchHand.SetActive(false);
            }
        }

#if BUILD_TYPE_DEBUG
        {
            GameObject debug_menu_prefab = Resources.Load<GameObject>("Prefab/Debug/BattleDebugMenu");
            if (debug_menu_prefab != null)
            {
                GameObject debug_menu_object = GameObject.Instantiate(debug_menu_prefab);
                if (debug_menu_object != null)
                {
                    debug_menu_object.transform.SetParent(transform, false);
                    Transform canvas_trans = debug_menu_object.transform.Find("Canvas");
                    if (canvas_trans != null)
                    {
                        Canvas canvas = canvas_trans.GetComponent<Canvas>();
                        if (canvas != null)
                        {
                            canvas.renderMode = RenderMode.ScreenSpaceCamera;
                            canvas.worldCamera = m_BattleCameraNear;
                            canvas.planeDistance = 0.5f;
                        }
                    }
                }
            }
        }
#endif
    }

    void Update()
    {
        // 画面位置調整（アスペクト比）
        if (m_BattleLogic.m_InitialPhase >= BattleLogic.InitialPhase.QUEST_INIT)
        {
            adjustScreen();
        }

        if (m_TopRightAnchor != null)
        {
            if (SafeAreaControl.HasInstance)
            {
                RectTransform rect_transform = m_TopRightAnchor.GetComponent<RectTransform>();
                if (rect_transform != null)
                {
                    rect_transform.anchoredPosition = new Vector2(320.0f, -SafeAreaControl.Instance.bar_height);
                }
            }
        }

        // チュートリアル更新
        m_BattleTutorialManager.updateTutorial(Time.deltaTime);

        // オートプレイ
        m_BattleAutoPlay.update(Time.deltaTime);

        // タッチ乗っ取り時の手の表示
        _updateOverrideTouchHand();

        // パーティＨＰ表示更新
        if (BattleParam.m_PlayerParty != null)
        {
            BattleParam.m_PlayerParty.updateDispHp(Time.deltaTime);
        }

        switch (m_BattleScenePhase)
        {
            case BattleScenePhase.NOT_BATTLE:
                {
                    if (m_IsChangeBattleScenePhase)
                    {
                        m_IsChangeBattleScenePhase = false;
                    }

                    // 手札エリアはオブジェクトの初期化が完了したら表示できる.
                    bool is_show_hand_area = m_IsShowHandArea && m_BattleLogic.m_InitialPhase >= BattleLogic.InitialPhase.QUEST_INIT;
                    m_PartsGroup1.SetActive(is_show_hand_area);
                    m_PartsGroup2.SetActive(false);
                }
                return;
            //break;

            case BattleScenePhase.IN_BATTLE:
                if (m_IsChangeBattleScenePhase)
                {
                    m_IsChangeBattleScenePhase = false;
                }

                m_PartsGroup1.SetActive(true);
                m_PartsGroup2.SetActive(true);
                break;
        }

        if (gameObject.IsActive())
        {
            // エネミー詳細ウィンドウの制御
            if (BattleParam.m_TargetEnemyWindow != InGameDefine.SELECT_NONE)
            {
                if (m_IsShowEnemyDetailWindow == false)
                {
                    m_IsShowEnemyDetailWindow = true;
                    if (m_EnemyDetailWindowObject == null)
                    {
                        if (m_EnemyDetailWindowPrefab != null)
                        {
                            m_EnemyDetailWindowObject = GameObject.Instantiate(m_EnemyDetailWindowPrefab);
                            BattleSceneUtil.setRide(m_EnemyDetailWindowObject.transform, SceneObjReferGameMain.Instance.m_UIInstanceTargetWindowPosition.transform);
                        }
                    }

                    if (m_EnemyDetailWindowObject != null)
                    {
                        m_EnemyDetailWindowObject.SetActive(true);
                    }
                }
                else
                {
                    if (m_EnemyDetailWindowObject == null || m_EnemyDetailWindowObject.IsNullOrInactive())
                    {
                        BattleParam.m_TargetEnemyWindow = InGameDefine.SELECT_NONE;
                        m_IsShowEnemyDetailWindow = false;
                        m_EnemyDetailWindowObject = null;
                    }
                }
            }


            // スキル成立時プレイヤーへ向かって飛んでいくエフェクト
            MasterDataDefineLabel.ElementType[,] elements = m_BattleLogic.m_SkillIconArea.getNewSkillElementInfo();
            if (elements != null)
            {
                for (int field_idx = 0; field_idx < elements.GetLength(0); field_idx++)
                {
                    for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                    {
                        MasterDataDefineLabel.ElementType element = elements[field_idx, idx];
                        if (element != MasterDataDefineLabel.ElementType.NONE)
                        {
                            Transform start_trans = m_BattleCardArea.GetComponent<BattleCardArea>().getEffectPosition(field_idx, BattleScene._BattleCardManager.EffectInfo.EffectPosition.FIELD_AREA);
                            Transform goal_trans = m_BattleLogic.m_BattlePlayerPartyViewControl.getEffectTransform((GlobalDefine.PartyCharaIndex)idx);
                            GameObject effect_obj = EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_SkillSetupNaught, Vector3.zero, Vector3.zero, start_trans, null);

                            if (effect_obj != null)
                            {
                                effect_obj.transform.tweenToParent(goal_trans, 0.3f);
                            }
                        }
                    }
                }
            }
            m_BattleLogic.m_SkillIconArea.clearNewSkillElementInfo();
        }

        // ユーザー操作可能になってからの経過フレーム数
        {
            BattleParam.BattlePhase phase = BattleParam.getBattlePhase();
            if (phase == BattleParam.BattlePhase.INPUT
                || phase == BattleParam.BattlePhase.INPUT_HANDLING
            )
            {
                m_InputEnableFrameCounter++;
            }
            else
            {
                m_InputEnableFrameCounter = 0;
            }
        }

        // 手札領域を暗くするマスクのオンオフ
        if (m_InputEnableFrameCounter >= 2)
        {
            m_CardMask.SetActive(false);
        }
        else
        {
            m_CardMask.SetActive(true);
        }

        // パーティＵＩの拡縮に応じたマスクコリジョンの制御
        {
            float offset_y = 0.0f;
            if (BattleParam.IsKobetsuHP)
            {
                if (BattleParam.isShowPartyInterfaceSkillCost())
                {
                    offset_y += 0.1f;
                }
                if (BattleParam.isShowPartyInterfaceSkillTurn())
                {
                    offset_y += 0.1f;
                }
            }
            m_UIMask.transform.localPosition = new Vector3(0.0f, offset_y, 0.0f);
        }

        // ガイド表示
        if (m_GuideDispTransform != null)
        {
            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
            if (cOption != null)
            {
                bool is_change = false;
                if (m_InputEnableFrameCounter >= 2)
                {
                    if (m_GuideDispMode != cOption.m_OptionGuide)
                    {
                        is_change = true;
                        m_GuideDispMode = cOption.m_OptionGuide;
                    }
                }
                else
                {
                    if (m_GuideDispMode != -1)
                    {
                        is_change = true;
                        m_GuideDispMode = -1;
                    }
                }

                if (is_change)
                {
                    m_GuideDispTransform.gameObject.SetActive(m_GuideDispMode > 0);
                }
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SceneCommon.initalizeMenuFps();

        BattleParam.clearValues();
    }

    public void setBattleScenePhase(BattleScenePhase battle_scene_phase)
    {
        if (m_BattleScenePhase != battle_scene_phase)
        {
            m_BattleScenePhase = battle_scene_phase;
            m_IsChangeBattleScenePhase = true;
        }
    }

    public bool isFadeing()
    {
        if (m_PartsGroup2 != null && m_PartsGroup2.IsActive())
        {
            Animation anim = m_PartsGroup2.GetComponent<Animation>();
            if (anim != null
                && anim.isPlaying
                )
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 手札エリアを表示し続けるかどうか
    /// </summary>
    /// <param name="is_show"></param>
    public void setShowHandAreaAlways(bool is_show)
    {
        m_IsShowHandArea = is_show;
    }

    // リファクタリング作業中でプライベート領域へアクセスしたい場合用.
    // 後にこの変数はなくなります.
    public BattleLogic PRIVATE_FIELD
    {
        get { return m_BattleLogic; }
    }

    /// <summary>
    /// 発動したノーマルスキルの属性種類数を取得.
    /// </summary>
    /// <returns>The active skill element count.</returns>
    public int getActiveSkillElementCount()
    {
        int elemColorNum = 0;
        for (int num = 0; num < m_BattleLogic.m_BattleLogicSkill.m_ActiveSkillElement.Length; ++num)
        {
            // 回復属性はカウントしない
            if (num == (int)MasterDataDefineLabel.ElementType.HEAL)
            {
                continue;
            }

            if (m_BattleLogic.m_BattleLogicSkill.m_ActiveSkillElement[num] == false)
            {
                continue;
            }

            ++elemColorNum;
        }

        return elemColorNum;
    }

    /// <summary>
    /// 発動したスキルによるハンズ数を取得.
    /// </summary>
    /// <returns>The hands count.</returns>
    public int getActiveSkillComboCount()
    {
        return m_BattleLogic.m_BattleLogicSkill.m_SkillComboCountCalc;
    }

    /// <summary>
    /// 手札に指定属性のカードが何枚あるかを取得.
    /// </summary>
    /// <returns>The hand card element count.</returns>
    /// <param name="element_type">Element_type.</param>
    public int getHandCardElementCount(MasterDataDefineLabel.ElementType element_type)
    {
        int ret_val = m_BattleCardArea.GetComponent<BattleCardArea>().m_BattleCardManager.m_HandArea.countElement(element_type);
        return ret_val;
    }

    public bool isTargetWindowOpen()
    {
        if (m_EnemyDetailWindowObject == null) return false;
        InGameTargetWindowQuest2 targetWindow = m_EnemyDetailWindowObject.GetComponent<InGameTargetWindowQuest2>();
        if (targetWindow == null) return false;
        return targetWindow.isOpen;
    }

    public void TargetWindowClose()
    {
        if (m_EnemyDetailWindowObject == null)
        {
            return;
        }

        InGameTargetWindowQuest2 targetWindow = m_EnemyDetailWindowObject.GetComponent<InGameTargetWindowQuest2>();
        if (targetWindow == null)
        {
            return;
        }

        targetWindow.OnClose();
    }

    /// <summary>
    /// タッチ入力を乗っ取る
    /// </summary>
    /// <param name="touch_screen_position"></param>
    /// <param name="is_touching"></param>
    public void setOverrideTouchMode(Vector2 touch_screen_position, bool is_touching)
    {
        float min_dist_sqr = 1.0e+16f;
        int near_x = -1;
        int near_y = -1;
        for (int idx_y = 0; idx_y < 2; idx_y++)
        {
            for (int idx_x = 0; idx_x < 5; idx_x++)
            {
                Vector2 col_pos = getCardFieldScreenPos(idx_x, idx_y);
                Vector2 vec = col_pos - touch_screen_position;
                float dist_sqr = vec.sqrMagnitude;
                if (dist_sqr < min_dist_sqr)
                {
                    min_dist_sqr = dist_sqr;
                    near_x = idx_x;
                    near_y = idx_y;
                }
            }
        }
        int hand_area_touch_index = ((near_y == 1) ? near_x : -1);
        int field_area_touch_index = ((near_y == 0) ? near_x : -1);

        BattleTouchInput.Instance.setOverrideTouchMode(touch_screen_position, is_touching, hand_area_touch_index, field_area_touch_index);

        m_OverrideTouchHandPosition = touch_screen_position;    // タッチしていないときも手の表示用座標を更新したいので別途保存している.
    }

    /// <summary>
    /// タッチ入力を乗っ取っている時の手を表示
    /// </summary>
    private void _updateOverrideTouchHand()
    {
        if (m_OverrideTouchHand != null)
        {
            bool is_override_mode = BattleTouchInput.Instance.isOverrideTouchMode();
            if (m_IsOverrideTouchMode != is_override_mode)
            {
                m_IsOverrideTouchMode = is_override_mode;
                m_OverrideTouchHand.SetActive(m_IsOverrideTouchMode);
            }

            if (m_IsOverrideTouchMode)
            {
                bool is_touch = BattleTouchInput.Instance.isTouching();

                // スクリーン座標をキャンバス座標へ変換
                Vector2 canvas_hand_pos = convertScreenPositionToCanvasPosition(m_OverrideTouchHandPosition);

                m_OverrideTouchHand.transform.localPosition = new Vector3(canvas_hand_pos.x, canvas_hand_pos.y, 0.0f);
                float rot = -15.0f;
                if (is_touch)
                {
                    rot = 0.0f;
                }
                m_OverrideTouchHand.transform.Find("Image").transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rot);
            }
        }
    }

    /// <summary>
    /// 手札・場札のスクリーン位置を取得
    /// </summary>
    /// <param name="x">横方向位置(0.0f:左端のカードの中心 4.0f:右端のカードの中心)</param>
    /// <param name="y">縦方向位置(0.0f:場札のカードの中心 1.0f:手札のカードの中心)</param>
    /// <returns>スクリーン位置</returns>
    public Vector2 getCardFieldScreenPos(float x, float y)
    {
        BattleCardArea battle_card_area = m_BattleCardArea.GetComponent<BattleCardArea>();
        Vector3 pos_hand_l = BattleTouchInput.Instance.getScreenPosition(battle_card_area.getEffectPosition(0, BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA).position);
        Vector3 pos_hand_r = BattleTouchInput.Instance.getScreenPosition(battle_card_area.getEffectPosition(4, BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA).position);
        Vector3 pos_field_l = BattleTouchInput.Instance.getScreenPosition(battle_card_area.getEffectPosition(0, BattleScene._BattleCardManager.EffectInfo.EffectPosition.FIELD_AREA).position);
        Vector3 pos_field_r = BattleTouchInput.Instance.getScreenPosition(battle_card_area.getEffectPosition(4, BattleScene._BattleCardManager.EffectInfo.EffectPosition.FIELD_AREA).position);

        Vector3 vec_x = ((pos_hand_r + pos_field_r) * 0.5f - (pos_hand_l + pos_field_l) * 0.5f) / 4.0f;
        Vector3 vec_y = (pos_hand_l + pos_hand_r) * 0.5f - (pos_field_l + pos_field_r) * 0.5f;

        Vector3 pos = pos_field_l + vec_x * x + vec_y * y;

        return new Vector2(pos.x, pos.y);
    }

    /// <summary>
    /// スクリーン位置をキャンバス位置へ変換
    /// </summary>
    /// <param name="screen_position"></param>
    /// <returns></returns>
    public Vector2 convertScreenPositionToCanvasPosition(Vector2 screen_position)
    {
        Vector2 canvas_position;
        {
            // スクリーン座標をキャンバス座標へ変換
            canvas_position.x = screen_position.x - Screen.width * 0.5f;
            canvas_position.y = screen_position.y - Screen.height * 0.5f;
            Canvas canvas = BattleSceneManager.Instance.m_UICanvas.GetComponent<Canvas>();
            canvas_position /= canvas.scaleFactor;
        }

        return canvas_position;
    }


    public void setTutorialPhase(BattleTutorialManager.TutorialBattlePhase tutorial_phase)
    {
        m_BattleTutorialManager.setTutorialPhase(tutorial_phase);
    }

    public bool isWaitTutorial()
    {
        return m_BattleTutorialManager.isWaitTutorial();
    }

    public MasterDataDefineLabel.ElementType getTutorialCard()
    {
        return m_BattleTutorialManager.getTutorialCard();
    }

    public bool isTutorialNoDeadEnemy()
    {
        return m_BattleTutorialManager.isNoDeadEnemy();
    }

    public bool isTutorialForbidLimitBreak(GlobalDefine.PartyCharaIndex caster_index)
    {
        return m_BattleTutorialManager.isForbidLimitBreak(caster_index);
    }

    public bool isTutorialEnableUnitInfoWindow()
    {
        return m_BattleTutorialManager.isEnableUnitInfoWindow();
    }

    public bool isTutorialEnableOptionButton()
    {
        return m_BattleTutorialManager.isEnableOptionButton();
    }

    public BattleTutorialManager.TutorialOptionMenuPhase getTutorialOptionMenuPhase()
    {
        return m_BattleTutorialManager.getTutorialOptionMenuPhase();
    }

    public bool isTutorialAllDeadEnemy(bool is_reset_flag)
    {
        return m_BattleTutorialManager.isAllDeadEnemy(is_reset_flag);
    }

    private enum AdjustScreenPhase
    {
        ADJUST_FOV, // 画角調整
        ADJUST_ROT_X,   // 上下位置調整
        ADJUST_ROT_X_CONT,  // 上下位置調整
        FINISH, // 画面調整完了
    }
    private AdjustScreenPhase m_AdjustScreenPhase = AdjustScreenPhase.FINISH;
    private float m_AdjstCanvasAspectRatio = 0.0f;
    private struct AdjsutScreenInfo
    {
        public float m_AspectRatio;
        public float m_Fov;
        public float m_RotX;

        public AdjsutScreenInfo(float aspect_ratio, float fov, float rot_x)
        {
            m_AspectRatio = aspect_ratio;
            m_Fov = fov;
            if (rot_x < 180.0f)
            {
                m_RotX = rot_x;
            }
            else
            {
                m_RotX = rot_x - 360.0f;
            }
        }
    }
    private static AdjsutScreenInfo[] m_AdjsutScreenInfos =
    {
        new AdjsutScreenInfo(640.0f / 1316.0f, 39.5648f, 354.4105f),
        new AdjsutScreenInfo(640.0f / 1280.0f, 38.45184f, 354.9672f),
        new AdjsutScreenInfo(640.0f / 1204.0f, 36.1552f, 356.0727f),
        new AdjsutScreenInfo(640.0f / 960.0f, 28.59984f, 359.8883f),
        new AdjsutScreenInfo(640.0f / 854.0f, 25.41776f, 1.449447f),
    };
    private void adjustScreen()
    {
        // Unity の fieldOfView の基準が不明なため計算で最適値を求めるのが難しい（アスペクト比、ステータスバーの影響を受ける？）
        // 基準位置オブジェクト(m_BottomRightAnchor)を配置してそれが画面の指定した位置（右下）へ近づくようにカメラを調整を行うという方法を行っています
        switch (m_AdjustScreenPhase)
        {
            case AdjustScreenPhase.FINISH:
                {
                    RectTransform rect_transform = m_UICanvas.GetComponent<RectTransform>();
                    if (rect_transform != null)
                    {
                        float aspect_ratio = rect_transform.rect.width / rect_transform.rect.height;
                        if (aspect_ratio != m_AdjstCanvasAspectRatio)
                        {
                            m_AdjstCanvasAspectRatio = aspect_ratio;
                            m_AdjustScreenPhase = AdjustScreenPhase.ADJUST_FOV;

                            // 影響を受ける表示物を消しておく
                            m_PartsGroup1.transform.Find("AppearAnim").gameObject.SetActive(false);

                            // 最適値が求まるまでのフレーム数を少なくするために、最適値に近い値をあらかじめ設定しておく
                            {
                                int idx = 0;
                                if (m_AdjstCanvasAspectRatio < m_AdjsutScreenInfos[0].m_AspectRatio)
                                {
                                    idx = 0;
                                }
                                else
                                if (m_AdjstCanvasAspectRatio >= m_AdjsutScreenInfos[m_AdjsutScreenInfos.Length - 1].m_AspectRatio)
                                {
                                    idx = m_AdjsutScreenInfos.Length - 2;
                                }
                                else
                                {
                                    for (idx = 0; idx <= m_AdjsutScreenInfos.Length - 2; idx++)
                                    {
                                        if (m_AdjstCanvasAspectRatio < m_AdjsutScreenInfos[idx + 1].m_AspectRatio)
                                        {
                                            break;
                                        }
                                    }
                                }

                                float t = Mathf.Clamp01((aspect_ratio - m_AdjsutScreenInfos[idx].m_AspectRatio) / (m_AdjsutScreenInfos[idx + 1].m_AspectRatio - m_AdjsutScreenInfos[idx].m_AspectRatio));

                                float fov = Mathf.Lerp(m_AdjsutScreenInfos[idx].m_Fov, m_AdjsutScreenInfos[idx + 1].m_Fov, t);
                                float rot_x = Mathf.Lerp(m_AdjsutScreenInfos[idx].m_RotX, m_AdjsutScreenInfos[idx + 1].m_RotX, t);
                                if (rot_x < 0.0f)
                                {
                                    rot_x += 360.0f;
                                }

                                m_BattleCameraFar.fieldOfView = fov;
                                Vector3 rot = m_BattleCameraFar.transform.localEulerAngles;
                                rot.x = rot_x;
                                m_BattleCameraFar.transform.localEulerAngles = rot;
                            }
                        }
                    }
                }
                break;

            case AdjustScreenPhase.ADJUST_FOV:
                // m_BottomRightAnchor の横位置の調整（FOVの調整）
                {
                    const float FOV_SPEED = 0.01f;
                    Vector3 screen_position = m_BattleCameraFar.WorldToScreenPoint(m_BottomRightAnchor.transform.position);
                    Canvas canvas = m_UICanvas.GetComponent<Canvas>();
                    const float CANVAS_MAX_WIDTH = 640.0f;  // アプリが使用するCanvasの幅は常に640になるように調整されている.
                    float canvas_x = (screen_position.x - Screen.width * 0.5f) / canvas.scaleFactor;
                    float xx = canvas_x - CANVAS_MAX_WIDTH * 0.5f;  // 現在位置と目標位置の差を取得
                    xx *= FOV_SPEED;

                    if (xx * xx > 0.00001f)
                    {
                        m_BattleCameraFar.fieldOfView = Mathf.Clamp(m_BattleCameraFar.fieldOfView + xx, 1.0f, 89.0f);
                        m_AdjustScreenPhase = AdjustScreenPhase.ADJUST_ROT_X_CONT;
                    }
                    else
                    {
                        m_AdjustScreenPhase = AdjustScreenPhase.ADJUST_ROT_X;
                    }
                }
                break;

            case AdjustScreenPhase.ADJUST_ROT_X:
            case AdjustScreenPhase.ADJUST_ROT_X_CONT:
                // m_BottomRightAnchor の縦位置の調整（x軸回転の調整）
                {
                    const float ROT_SPEED = -5.0f;
                    Vector3 viewport_position = m_BattleCameraFar.WorldToViewportPoint(m_BottomRightAnchor.transform.position);
                    Vector3 rot = m_BattleCameraFar.transform.localEulerAngles;

                    float yy = viewport_position.y;

                    if (SafeAreaControl.HasInstance
                        && SafeAreaControl.Instance.bottom_space_height != 0
                    )
                    {
                        if (m_UICanvas != null)
                        {
                            Canvas canvas = m_UICanvas.GetComponent<Canvas>();
                            if (canvas != null)
                            {
                                float bottom_space_offset_y = SafeAreaControl.Instance.bottom_space_height / canvas.pixelRect.height * canvas.scaleFactor;
                                yy -= bottom_space_offset_y;
                            }
                        }
                    }

                    yy *= ROT_SPEED;

                    if (yy * yy > 0.00001f)
                    {
                        yy = Mathf.Clamp(yy, -1.0f, 1.0f);

                        rot.x += yy;
                        m_BattleCameraFar.transform.localEulerAngles = rot;

                        m_AdjustScreenPhase = AdjustScreenPhase.ADJUST_FOV;
                    }
                    else
                    {
                        if (m_AdjustScreenPhase == AdjustScreenPhase.ADJUST_ROT_X_CONT)
                        {
                            m_AdjustScreenPhase = AdjustScreenPhase.ADJUST_FOV;
                        }
                        else
                        {
                            m_AdjustScreenPhase = AdjustScreenPhase.FINISH;
                            m_BattleCameraNear.fieldOfView = m_BattleCameraFar.fieldOfView;
                            m_BattleCameraNear.transform.localEulerAngles = m_BattleCameraFar.transform.localEulerAngles;

                            // 影響を受ける表示物を消していたのを復活(出現アニメーションあり)
                            m_PartsGroup1.transform.Find("AppearAnim").gameObject.SetActive(true);
                        }
                    }
                }
                break;
        }
    }
}
