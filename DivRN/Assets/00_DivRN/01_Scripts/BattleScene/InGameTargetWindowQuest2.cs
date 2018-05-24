using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;
using M4u;

//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
    @class		InGameTargetWindowQuest2
    @brief		ターゲットウィンドウクラス(新探索版)
*/
//----------------------------------------------------------------------------
public class InGameTargetWindowQuest2 : M4uContextMonoBehaviour
{
    enum eTargetWindowStep : int
    {
        NONE = 0,
        OPEN_INIT,
        OPEN,
        CLOSE,
    };

    M4uProperty<float> window_y = new M4uProperty<float>();
    public float Window_y { get { return window_y.Value; } set { window_y.Value = value; } }

    M4uProperty<string> label_text = new M4uProperty<string>();
    public string Label_text { get { return label_text.Value; } set { label_text.Value = value; } }

    M4uProperty<string> closeText = new M4uProperty<string>();
    public string CloseText { get { return closeText.Value; } set { closeText.Value = value; } }

    public GameObject windowPanelRoot = null;
    public UnitNamePanel unitNamePanel = null;
    public UnitAilmentPanel unitAilmentPanel = null;

    private uint m_CharaId = 0;
    private eTargetWindowStep m_TargetWindowStep = eTargetWindowStep.NONE;
    private bool m_Open = false;
    public bool isOpen { get { return m_Open; } }
    private bool m_Close = false;
    public bool isClose { get { return m_Close; } }

    void Awake()
    {
        gameObject.GetComponent<M4uContextRoot>().Context = this;
        unitNamePanel = InGameSkillWindowQuest2.setupPrefab<UnitNamePanel>("Prefab/UnitNamePanel/UnitNamePanel", windowPanelRoot);
        unitAilmentPanel = InGameSkillWindowQuest2.setupPrefab<UnitAilmentPanel>("Prefab/InGame/InGameUI/Menu/UnitAilmentPanel", windowPanelRoot);
        CloseText = GameTextUtil.GetText("common_button6");
    }


    //------------------------------------------------------------------------
    /*!
        @brief		更新前処理
    */
    //------------------------------------------------------------------------
    protected void Start()
    {
        Window_y = 960;
        m_Close = false;
        m_Open = false;
        Open((uint)BattleParam.m_EnemyParam[BattleParam.m_TargetEnemyWindow].getMasterDataParamChara().fix_id);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		更新処理
    */
    //------------------------------------------------------------------------
    void Update()
    {
        switch (m_TargetWindowStep)
        {
            case eTargetWindowStep.OPEN_INIT:
                {
                    if (unitAilmentPanel.Ailment_obj_list.Count >= 2)
                    {
                        unitAilmentPanel.setupCharaAilmentInfo(BattleParam.m_EnemyParam[BattleParam.m_TargetEnemyWindow].m_StatusAilmentChara);
                        m_TargetWindowStep = eTargetWindowStep.OPEN;
                    }
                }
                break;
            case eTargetWindowStep.OPEN:
                {
                    Window_y -= (3840 * Time.deltaTime);
                    if (Window_y <= 0)
                    {
                        Window_y = 0;
                        m_TargetWindowStep = eTargetWindowStep.NONE;
                    }
                }
                break;
            case eTargetWindowStep.CLOSE:
                {
                    Window_y += (3840 * Time.deltaTime);
                    if (Window_y >= 960)
                    {
                        m_TargetWindowStep = eTargetWindowStep.NONE;
                        m_Close = true;
                        Destroy(this.gameObject);
                    }
                }
                break;
            default:
                break;
        }
    }


    //------------------------------------------------------------------------
    /*!
        @brief		更新処理
    */
    //------------------------------------------------------------------------
    public void Open(uint id)
    {
        SoundUtil.PlaySE(SEID.SE_BATLE_UI_OPEN);
        m_CharaId = id;
        m_Open = true;

        initPanel();

        Label_text = GameTextUtil.GetText("battle_infotext4");

        //「ENEMY INFO」テキスト差し替え
        {
            uint area_category_id = ReplaceAssetManager.getAreaCategoryIDFromQuestID(BattleParam.m_QuestMissionID); //MainMenuParam.m_RegionIDからは直接は取らない（中断復帰時対策）
            MasterDataQuestAppearance[] questAppearance = MasterFinder<MasterDataQuestAppearance>.Instance.SelectWhere("where area_category_id = ?", area_category_id).ToArray();
            if (questAppearance.IsNullOrEmpty() == false)
            {
                //Label_text = GameTextUtil.GetText(questAppearance[0].enemy_info_text_key);
                // テキストキーではなく直接テキストが入っている
                Label_text = questAppearance[0].enemy_info_text_key;
            }
        }

        m_TargetWindowStep = eTargetWindowStep.OPEN_INIT;
    }

    //------------------------------------------------------------------------
    /*!
        @brief		更新処理
    */
    //------------------------------------------------------------------------
    private void initPanel()
    {
        MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)m_CharaId);

        // 名前パネル設定
        unitNamePanel.setup(_master);

        // 状態異常パネル設定
        unitAilmentPanel.setupEnemyAbilityInfo(BattleParam.m_EnemyParam[BattleParam.m_TargetEnemyWindow]);
    }

    //------------------------------------------------------------------------
    /*!
        @brief		更新処理
    */
    //------------------------------------------------------------------------
    public void OnClose()
    {
        if (m_TargetWindowStep != eTargetWindowStep.CLOSE)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            m_TargetWindowStep = eTargetWindowStep.CLOSE;
        }
    }

}
