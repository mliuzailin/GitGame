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
using M4u;

public class ContinuousBattleManager : M4uContextMonoBehaviour
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public enum State : int
    {
        NONE = 0,
        INIT_IN,
        INIT_ICON,
        INIT_WAIT,
        INIT_OUT,
        START_IN,
        START_WAIT,
        START_OUT,
    };

    public enum MoveState : int
    {
        NONE = 0,
        MOVE,
        END,
    };

    public GameObject m_EnemyIconList = null;                                 //!< 敵アイコンルートオブジェクト
    public Sprite[] m_NewBattleBg = null;

    M4uProperty<Color> background_color = new M4uProperty<Color>();
    public Color Background_color { get { return background_color.Value; } set { background_color.Value = value; } }

    M4uProperty<bool> enemy_map_active = new M4uProperty<bool>();
    public bool Enemy_map_active { get { return enemy_map_active.Value; } set { enemy_map_active.Value = value; } }

    M4uProperty<float> enemy_map_y = new M4uProperty<float>();
    public float Enemy_map_y { get { return enemy_map_y.Value; } set { enemy_map_y.Value = value; } }

    M4uProperty<float> enemy_map_height = new M4uProperty<float>();
    public float Enemy_map_height { get { return enemy_map_height.Value; } set { enemy_map_height.Value = value; } }

    M4uProperty<float> enemy_list_x = new M4uProperty<float>();
    public float Enemy_list_x { get { return enemy_list_x.Value; } set { enemy_list_x.Value = value; } }

    M4uProperty<Sprite> battle_bg = new M4uProperty<Sprite>();
    public Sprite Battle_bg { get { return battle_bg.Value; } set { battle_bg.Value = value; } }

    private List<ContinuousBattleEnemyIcon> m_EnemyList = null;                                 //!< ゲーム中オブジェクト：バトルアイコンリスト
    private State m_State = State.NONE;
    private float m_ScaleWaitTime;
    private float m_WaitTime;
    private MoveState m_ListMove;
    private float m_MoveTime;
    private float m_MoveMaxTime;
    private float m_MoveSpeed;

    private float m_ScaleTime = 0.25f;
    private float m_StartDelayTime = 0.33f;
    private float m_IconDelayTime = 0.1f;
    private float m_BlinkTime = 0.1f;
    private int m_BlinkCountMax = 3;

    private int m_BattleCount;
    private int m_BlinkCount;
    private bool m_BlinkSwitch;

    public State cbm_state { get { return m_State; } }
    public List<ContinuousBattleEnemyIcon> enemyList { get { return m_EnemyList; } }

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

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
        changeBackgroundColor(false);
        setEnemyMapActive(true);
        Enemy_map_height = 0;
        Enemy_list_x = 0;
        m_ListMove = MoveState.NONE;
        m_MoveSpeed = 74.0f / 0.1f;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    void Update()
    {
        switch (m_State)
        {
            case State.INIT_IN:
                {
                    m_ScaleWaitTime += Time.deltaTime;
                    if (m_ScaleWaitTime >= m_ScaleTime)
                    {
                        m_ScaleWaitTime = m_ScaleTime;
                        for (int i = 0; i < m_EnemyList.Count; ++i)
                        {
                            m_EnemyList[i].setAlpha((float)i * m_IconDelayTime + m_StartDelayTime);
                        }
                        m_State = State.INIT_ICON;
                    }
                    Enemy_map_height = m_ScaleWaitTime / m_ScaleTime;
                }
                break;
            case State.INIT_ICON:
                {
                    bool ret = true;
                    for (int i = 0; i < m_EnemyList.Count; ++i)
                    {
                        if (m_EnemyList[i].icon_state != ContinuousBattleEnemyIcon.State.NONE)
                        {
                            ret = false;
                            break;
                        }
                        else
                        {
                            if (m_ListMove == MoveState.NONE
                            && m_EnemyList.Count > 4
                            && i == 0)
                            {
                                m_ListMove = MoveState.MOVE;
                                m_MoveTime = 0;
                                m_MoveMaxTime = (m_EnemyList.Count - 4) * 0.1f;
                            }
                        }
                    }
                    if (ret == true)
                    {
                        m_WaitTime = m_StartDelayTime;
                        m_State = State.INIT_WAIT;
                    }
                }
                break;
            case State.INIT_WAIT:
                {
                    m_WaitTime -= Time.deltaTime;
                    if (m_WaitTime <= 0)
                    {
                        m_State = State.INIT_OUT;
                    }
                }
                break;
            case State.INIT_OUT:
                {
                    m_ScaleWaitTime -= Time.deltaTime;
                    if (m_ScaleWaitTime <= 0)
                    {
                        m_ScaleWaitTime = 0;
                        m_State = State.NONE;
                    }
                    Enemy_map_height = m_ScaleWaitTime / m_ScaleTime;
                }
                break;
            case State.START_IN:
                {
                    m_ScaleWaitTime += Time.deltaTime;
                    if (m_ScaleWaitTime >= m_ScaleTime)
                    {
                        m_ScaleWaitTime = m_ScaleTime;
                        if (m_BattleCount > 0)
                        {
                            m_EnemyList[m_BattleCount - 1].changeIcon(ContinuousBattleEnemyIcon.ICON_TYPE.TYPE_CLEAR);
                        }
                        m_State = State.START_WAIT;
                    }
                    Enemy_map_height = m_ScaleWaitTime / m_ScaleTime;
                }
                break;
            case State.START_WAIT:
                {
                    m_WaitTime -= Time.deltaTime;
                    if (m_WaitTime <= 0)
                    {
                        if (m_BlinkSwitch == false)
                        {
                            m_BlinkSwitch = true;
                            m_WaitTime = m_BlinkTime;
                            m_EnemyList[m_BattleCount].changeFocus();
                        }
                        else
                        {
                            --m_BlinkCount;
                            if (m_BlinkCount > 0)
                            {
                                m_BlinkSwitch = false;
                                m_WaitTime = m_BlinkTime;
                                m_EnemyList[m_BattleCount].changeFocus();
                            }
                            else
                            {
                                m_State = State.START_OUT;
                            }
                        }
                    }
                }
                break;
            case State.START_OUT:
                {
                    m_ScaleWaitTime -= Time.deltaTime;
                    if (m_ScaleWaitTime <= 0)
                    {
                        m_ScaleWaitTime = 0;
                        m_State = State.NONE;
                    }
                    Enemy_map_height = m_ScaleWaitTime / m_ScaleTime;
                }
                break;
            default:
                break;
        }
        if (m_ListMove == MoveState.MOVE)
        {
            m_MoveTime += Time.deltaTime;
            if (m_MoveTime >= m_MoveMaxTime)
            {
                m_ListMove = MoveState.END;
                m_MoveTime = m_MoveMaxTime;
            }
            Enemy_list_x = -(m_MoveTime * m_MoveSpeed);
        }
    }

    public void changeBackgroundColor(bool bSwitch)
    {
        if (bSwitch == true)
        {
            Background_color = Color.black;
        }
        else
        {
            Background_color = Color.white;
        }
    }

    public int checkBoosBattleCount()
    {
        int ret = 1;
        ServerDataDefine.PacketStructQuest2Build build = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild;
        int boss_index = build.boss[1];
        while (build.list_battle[boss_index].chain != 0)
        {
            ++ret;
            boss_index = build.list_battle[boss_index].chain;
        }

        return ret;
    }

    public int setupEnemyMap(uint questId, int battleCount, int guerrilla)
    {
        //------------------------------
        // 敵リスト情報構築
        //------------------------------
        m_EnemyList = new List<ContinuousBattleEnemyIcon>();
        GameObject obj = null;
        GameObject new_obj = null;

        int boss_battle_count = checkBoosBattleCount();
        if (boss_battle_count <= 0) boss_battle_count = 1;

        ContinuousBattleEnemyIcon icon = null;
        int battle_max = SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_battle.Length - (boss_battle_count + 1);
        if (battle_max < 0) battle_max = 0;
        ContinuousBattleEnemyIcon.ICON_TYPE type = ContinuousBattleEnemyIcon.ICON_TYPE.TYPE_CLEAR;
        for (int i = 0; i < battle_max; ++i)
        {
            if (i == guerrilla)
            {
                type = ContinuousBattleEnemyIcon.ICON_TYPE.TYPE_GUERRILLA;
            }
            else if (i >= battleCount)
            {
                type = ContinuousBattleEnemyIcon.ICON_TYPE.TYPE_ENEMY;
            }
            obj = Resources.Load("Prefab/InGame/InGameBattle/ContinuousEnemyIcon", typeof(GameObject)) as GameObject;
            new_obj = Instantiate(obj) as GameObject;
            icon = new_obj.GetComponent<ContinuousBattleEnemyIcon>();
            if (icon != null)
            {
                new_obj.transform.parent = m_EnemyIconList.transform;
                icon.setup(type, i, SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.list_battle[i + boss_battle_count + 1].unique_id, false);
                m_EnemyList.Add(icon);
            }
        }
        obj = Resources.Load("Prefab/InGame/InGameBattle/ContinuousEnemyIcon", typeof(GameObject)) as GameObject;
        new_obj = Instantiate(obj) as GameObject;
        icon = new_obj.GetComponent<ContinuousBattleEnemyIcon>();
        type = ContinuousBattleEnemyIcon.ICON_TYPE.TYPE_BOSS;
        if (battleCount == m_EnemyList.Count) type = ContinuousBattleEnemyIcon.ICON_TYPE.TYPE_BOSS_FOCUS;
        if (icon != null)
        {
            new_obj.transform.parent = m_EnemyIconList.transform;
            icon.setup(type, battle_max, SceneGoesParam.Instance.m_SceneGoesParamToQuest2Build.m_QuestBuild.boss[1], true);
            m_EnemyList.Add(icon);
        }

        return m_EnemyList.Count;
    }

    public void setEnemyMapActive(bool active_sw)
    {
        Enemy_map_active = active_sw;
    }

    public void setEnemyMapInit()
    {
        Enemy_map_y = -80;
        Enemy_map_height = 0;
        m_State = State.INIT_IN;
        m_ScaleWaitTime = 0;
    }

    public void setEnemyMapStart(int battleCount)
    {
        for (int i = 0; i < m_EnemyList.Count; ++i)
        {
            m_EnemyList[i].Icon_alpha = 1;
        }
        Enemy_map_y = 0;
        if ((m_EnemyList.Count - battleCount) > 6)
        {
            Enemy_list_x = 37 - (battleCount * 74);
        }
        else
        {
            if (m_EnemyList.Count <= 6)
            {
                Enemy_list_x = 37;
            }
        }
        Enemy_map_height = 0;
        m_State = State.START_IN;
        m_ScaleWaitTime = 0;
        m_BattleCount = battleCount;
        m_BlinkSwitch = false;
        m_BlinkCount = m_BlinkCountMax;
        m_WaitTime = m_BlinkTime;
    }
}
