using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class DebugDispEnemy : M4uContextMonoBehaviour
{

    public InputField m_InputFieldEnemyId;

    public Toggle m_TogglePos;
    public InputField m_InputFieldPosX;
    public InputField m_InputFieldPosY;

    public Dropdown m_DropdwonHpType;
    public InputField m_InputFieldHpX;
    public InputField m_InputFieldHpY;

    public Toggle m_ToggleTgt;
    public InputField m_InputFieldTgtX;
    public InputField m_InputFieldTgtY;

    public Camera m_CameraBattle = null;
    public GameObject m_EnemyAreaPrefab = null;

    public GameObject m_TextLoading = null;


    private BattleDispEnemy m_BattleDispEnemy = null;

    private MasterDataParamEnemy[] m_MasterDataParamEnemys = null;
    private BattleEnemy[] m_BattleEnemys = null;

    private int m_CallCount = 0;    // 二重呼び出し禁止判定用

    private class EnemyDispInfo
    {
        public MasterDataParamEnemy m_OriginalMasterData;

        public bool m_IsUsePosition;    // 敵表示位置の座標指定を使用するかどうか（使用しない場合は自動計算）
        public int m_PositionX;
        public int m_PositionY;

        public MasterDataDefineLabel.HPGaugeType m_HpGaugeType;
        public int m_HpGaugeX;
        public int m_HpGaugeY;

        public bool m_IsUseTargetPosition;  // ターゲットカーソル位置の指定を行うかどうか
        public int m_TargetX;
        public int m_TargetY;
    }

    private EnemyDispInfo[] m_EnemyDispInfos;

    private BattleEnemy m_CurrentBattleEnemy = null;
    private EnemyDispInfo m_CurrentEnemyDispInfo = null;


    M4uProperty<string> textEnemyName = new M4uProperty<string>();
    public string TextEnemyName { get { return textEnemyName.Value; } }

    private enum Phase
    {
        NONE,
        LOAD_MASTER,
        INIT_MASTER,
        UPDATE,
        ERROR,
    }
    private Phase m_Phase = Phase.NONE;

    // Use this for initialization
    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.fitTopAndBottom(GetComponent<RectTransform>());
        }
    }

    private void OnEnable()
    {
        m_Phase = Phase.NONE;
    }

    private void OnDisable()
    {
        m_MasterDataParamEnemys = null;
        if (m_BattleDispEnemy != null)
        {
            GameObject obj = m_BattleDispEnemy.gameObject;
            m_BattleDispEnemy = null;
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_Phase)
        {
            case Phase.NONE:
                m_Phase = Phase.LOAD_MASTER;
                loadMasterData();
                if (m_TextLoading != null)
                {
                    m_TextLoading.SetActive(true);
                }
                break;

            case Phase.LOAD_MASTER:
                break;

            case Phase.INIT_MASTER:
                initMasterData();
                initEnemyArea();
                if (m_TextLoading != null)
                {
                    m_TextLoading.SetActive(false);
                }
                m_Phase = Phase.UPDATE;
                break;

            case Phase.UPDATE:
                //m_TouchInput.Update(Time.deltaTime);
                break;

            case Phase.ERROR:
                break;
        }
    }


    public void OnUiEvent(string event_name)
    {
        if (m_Phase != Phase.UPDATE)
        {
            return;
        }

        if (m_CallCount > 0)
        {
            return;
        }

        switch (event_name)
        {
            case "InputFieldEnemyID":
            case "ButtonEnemyIDRight":
            case "ButtonEnemyIDLeft":
                {
                    int enemy_id = int.Parse(m_InputFieldEnemyId.text);
                    int index = searchEnemyIndex(enemy_id);

                    switch (event_name)
                    {
                        case "ButtonEnemyIDRight":
                            index++;
                            break;
                        case "ButtonEnemyIDLeft":
                            index--;
                            break;
                    }

                    index = (index + m_MasterDataParamEnemys.Length) % m_MasterDataParamEnemys.Length;
                    enemy_id = (int)m_MasterDataParamEnemys[index].fix_id;
                    m_InputFieldEnemyId.text = enemy_id.ToString();
                    initEnemy(enemy_id);
                    //updateWorkEnemyMaster();
                }
                break;

            case "InputFieldPosX":
            case "InputFieldPosY":
            case "ButtonPosUp":
            case "ButtonPosDown":
            case "ButtonPosLeft":
            case "ButtonPosRight":
                {
                    float fx = float.Parse(m_InputFieldPosX.text);
                    float fy = float.Parse(m_InputFieldPosY.text);

                    int x = (int)(fx * 1000.1f);
                    int y = (int)(fy * 1000.1f);

                    switch (event_name)
                    {
                        case "ButtonPosUp":
                            y++;
                            break;
                        case "ButtonPosDown":
                            y--;
                            break;
                        case "ButtonPosLeft":
                            x--;
                            break;
                        case "ButtonPosRight":
                            x++;
                            break;
                    }

                    fx = x / 1000.0f;
                    fy = y / 1000.0f;

                    m_InputFieldPosX.text = fx.ToString();
                    m_InputFieldPosY.text = fy.ToString();
                    updateFromInterface();
                }
                break;

            case "InputFieldHpX":
            case "InputFieldHpY":
            case "ButtonHpUp":
            case "ButtonHpDown":
            case "ButtonHpLeft":
            case "ButtonHpRight":
                {
                    float fx = float.Parse(m_InputFieldHpX.text);
                    float fy = float.Parse(m_InputFieldHpY.text);

                    int x = (int)(fx * 1000.1f);
                    int y = (int)(fy * 1000.1f);

                    switch (event_name)
                    {
                        case "ButtonHpUp":
                            y++;
                            break;
                        case "ButtonHpDown":
                            y--;
                            break;
                        case "ButtonHpLeft":
                            x--;
                            break;
                        case "ButtonHpRight":
                            x++;
                            break;
                    }

                    fx = x / 1000.0f;
                    fy = y / 1000.0f;

                    m_InputFieldHpX.text = fx.ToString();
                    m_InputFieldHpY.text = fy.ToString();
                    updateFromInterface();
                }
                break;

            case "InputFieldTgtX":
            case "InputFieldTgtY":
            case "ButtonTgtUp":
            case "ButtonTgtDown":
            case "ButtonTgtLeft":
            case "ButtonTgtRight":
                {
                    float fx = float.Parse(m_InputFieldTgtX.text);
                    float fy = float.Parse(m_InputFieldTgtY.text);

                    int x = (int)(fx * 1000.1f);
                    int y = (int)(fy * 1000.1f);

                    switch (event_name)
                    {
                        case "ButtonTgtUp":
                            y++;
                            break;
                        case "ButtonTgtDown":
                            y--;
                            break;
                        case "ButtonTgtLeft":
                            x--;
                            break;
                        case "ButtonTgtRight":
                            x++;
                            break;
                    }

                    fx = x / 1000.0f;
                    fy = y / 1000.0f;

                    m_InputFieldTgtX.text = fx.ToString();
                    m_InputFieldTgtY.text = fy.ToString();
                    updateFromInterface();
                }
                break;

            case "TogglePos":
            case "DropdownHp":
            case "ToggleTgt":
                updateFromInterface();
                break;

            case "ButtonPosReset":
                if (m_CurrentEnemyDispInfo != null)
                {
                    MasterDataParamEnemy enemy_master = m_CurrentEnemyDispInfo.m_OriginalMasterData;    // 元のマスターデータ
                    m_CurrentEnemyDispInfo.m_IsUsePosition = (enemy_master.pos_absolute == MasterDataDefineLabel.BoolType.ENABLE);
                    m_CurrentEnemyDispInfo.m_PositionX = enemy_master.posx_offset;
                    m_CurrentEnemyDispInfo.m_PositionY = enemy_master.posy_offset;
                    setInfoToInteface();

                    updateFromInterface();
                }
                break;

            case "ButtonHpReset":
                if (m_CurrentEnemyDispInfo != null)
                {
                    MasterDataParamEnemy enemy_master = m_CurrentEnemyDispInfo.m_OriginalMasterData;    // 元のマスターデータ
                    m_CurrentEnemyDispInfo.m_HpGaugeType = enemy_master.hp_gauge_type;
                    m_CurrentEnemyDispInfo.m_HpGaugeX = enemy_master.hp_posx_offset;
                    m_CurrentEnemyDispInfo.m_HpGaugeY = enemy_master.hp_posy_offset;
                    setInfoToInteface();

                    updateFromInterface();
                }
                break;

            case "ButtonTgtReset":
                if (m_CurrentEnemyDispInfo != null)
                {
                    MasterDataParamEnemy enemy_master = m_CurrentEnemyDispInfo.m_OriginalMasterData;    // 元のマスターデータ
                    m_CurrentEnemyDispInfo.m_IsUseTargetPosition = (enemy_master.target_pos_absolute == (int)MasterDataDefineLabel.BoolType.ENABLE);
                    m_CurrentEnemyDispInfo.m_TargetX = enemy_master.target_posx_offset;
                    m_CurrentEnemyDispInfo.m_TargetY = enemy_master.target_posy_offset;
                    setInfoToInteface();

                    updateFromInterface();
                }
                break;

        }
    }

    /// <summary>
    /// インターフェイスによる変更をデータへ反映
    /// </summary>
    private void updateFromInterface()
    {
        setInterfaceToInfo();
        setInfoToWorkMaster();
    }

    /// <summary>
    /// デバッグ機能に必要なマスターデータを取得
    /// </summary>
    private void loadMasterData()
    {
#if true
        if (m_MasterDataParamEnemys == null)
        {
            Dictionary<EMASTERDATA, uint> dict = new Dictionary<EMASTERDATA, uint>();
            dict.Add(EMASTERDATA.eMASTERDATA_PARAM_ENEMY, 0);
            ServerDataUtilSend.SendPacketAPI_Debug_GetMasterDataAll2(dict).
                setSuccessAction(
                    _data =>
                    {
                        ServerDataDefine.RecvMasterDataAll2Value result = _data.GetResult<ServerDataDefine.RecvMasterDataAll2>().result;
                        m_MasterDataParamEnemys = result.master_array_enemy.upd_list;
                        m_Phase = Phase.INIT_MASTER;
                    }
                ).
                setErrorAction(
                    _date =>
                    {
                        m_Phase = Phase.ERROR;
#if BUILD_TYPE_DEBUG
                        Debug.Log("ERROR");
#endif
                    }
                ).
                SendStart();
        }
#else
		if (m_MasterDataParamEnemys == null)
		{
			BattleParam.m_IsUseDebugJsonMasterData = true;
			m_MasterDataParamEnemys = BattleParam.m_MasterDataCache.getAllEnemyParam();
			m_Phase = Phase.INIT_MASTER;
		}
#endif
    }

    private void initMasterData()
    {
        // sort
        if (m_MasterDataParamEnemys != null)
        {
            List<MasterDataParamEnemy> aaa = new List<MasterDataParamEnemy>(m_MasterDataParamEnemys);
            aaa.Sort((a, b) => (int)a.fix_id - (int)b.fix_id);
            m_MasterDataParamEnemys = aaa.ToArray();
        }

        // 位置補正情報を初期化
        if (m_MasterDataParamEnemys != null)
        {
            m_EnemyDispInfos = new EnemyDispInfo[m_MasterDataParamEnemys.Length];
            for (int idx = 0; idx < m_EnemyDispInfos.Length; idx++)
            {
                MasterDataParamEnemy enemy_master = m_MasterDataParamEnemys[idx];
                if (enemy_master != null)
                {
                    EnemyDispInfo disp_info = new EnemyDispInfo();
                    disp_info.m_OriginalMasterData = enemy_master;

                    disp_info.m_IsUsePosition = (enemy_master.pos_absolute == MasterDataDefineLabel.BoolType.ENABLE);
                    disp_info.m_PositionX = enemy_master.posx_offset;
                    disp_info.m_PositionY = enemy_master.posy_offset;

                    disp_info.m_HpGaugeType = enemy_master.hp_gauge_type;
                    disp_info.m_HpGaugeX = enemy_master.hp_posx_offset;
                    disp_info.m_HpGaugeY = enemy_master.hp_posy_offset;

                    disp_info.m_IsUseTargetPosition = (enemy_master.target_pos_absolute == (int)MasterDataDefineLabel.BoolType.ENABLE);
                    disp_info.m_TargetX = enemy_master.target_posx_offset;
                    disp_info.m_TargetY = enemy_master.target_posy_offset;

                    m_EnemyDispInfos[idx] = disp_info;
                }
            }
        }
    }

    /// <summary>
    /// 敵マスターをサーチ（enemy_idぴったりのIDがない場合は近い値の物を取得）
    /// </summary>
    /// <param name="enemy_id"></param>
    /// <returns></returns>
    private int searchEnemyIndex(int enemy_id)
    {
        int ret_val = 0;

        if (m_MasterDataParamEnemys != null)
        {
            for (int idx = 0; idx < m_MasterDataParamEnemys.Length; idx++)
            {
                MasterDataParamEnemy enemy_master = m_MasterDataParamEnemys[idx];
                if (enemy_master != null && enemy_master.fix_id >= enemy_id)
                {
                    ret_val = idx;
                    break;
                }
            }
        }

        return ret_val;
    }

    private MasterDataParamEnemy searchEnemy(int enemy_id)
    {
        MasterDataParamEnemy ret_val = null;

        int index = searchEnemyIndex(enemy_id);
        ret_val = m_MasterDataParamEnemys[index];

        return ret_val;
    }


    private void initEnemyArea()
    {
        if (m_EnemyAreaPrefab != null)
        {
            GameObject enemy_area_object = GameObject.Instantiate(m_EnemyAreaPrefab);
            if (enemy_area_object != null)
            {
                enemy_area_object.transform.SetParent(m_CameraBattle.transform, false);
                m_BattleDispEnemy = enemy_area_object.GetComponent<BattleDispEnemy>();

                float camera_fov = 0.0f;
                Quaternion camera_rot = Quaternion.identity;
                BattleSceneUtil.getAdjustScreenInfo(ref camera_fov, ref camera_rot);
                m_CameraBattle.fieldOfView = camera_fov;
                m_CameraBattle.transform.localRotation = camera_rot;

                if (BattleTouchInput.HasInstance == false)
                {
                    BattleTouchInput battle_touch_input = gameObject.AddComponent<BattleTouchInput>();
                    battle_touch_input.setCamera(m_CameraBattle);
                }
                m_BattleDispEnemy.init(m_CameraBattle, transform.parent.GetComponent<Canvas>());
            }
        }
    }

    private void initEnemy(int enemy_id)
    {
        if (m_BattleDispEnemy != null && m_MasterDataParamEnemys != null)
        {
            //MasterDataParamEnemy enemy_master = searchEnemy(enemy_id);
            int index = searchEnemyIndex(enemy_id);
            MasterDataParamEnemy enemy_master = m_MasterDataParamEnemys[index];

            if (enemy_master != null)
            {
                MasterDataParamChara chara_master = MasterFinder<MasterDataParamChara>.Instance.Find((int)enemy_master.chara_id);
                if (chara_master != null)
                {
                    MasterDataParamEnemy wrk_enemy_master = new MasterDataParamEnemy();
                    wrk_enemy_master.Copy(enemy_master); //元のマスターデータに影響が出ないようにコピーを作成
                    BattleEnemy battle_enemy = null;
                    {
                        battle_enemy = new BattleEnemy();
                        battle_enemy.setMasterData(wrk_enemy_master, chara_master);
                        battle_enemy.m_EnemyAttack = enemy_master.status_pow;
                        battle_enemy.m_EnemyDefense = enemy_master.status_def;
                        battle_enemy.m_EnemyHP = enemy_master.status_hp;
                        battle_enemy.m_EnemyHPMax = enemy_master.status_hp;
                        battle_enemy.m_EnemyDrop = 0;
                        battle_enemy.m_StatusAilmentChara = new StatusAilmentChara(StatusAilmentChara.OwnerType.ENEMY);
                        battle_enemy.setAcquireDropUnit(false);
                    }
                    m_CurrentBattleEnemy = battle_enemy;
                    m_CurrentEnemyDispInfo = m_EnemyDispInfos[index];

                    setInfoToInteface();
                    setInfoToWorkMaster();

                    m_BattleEnemys = new BattleEnemy[1];
                    m_BattleEnemys[0] = battle_enemy;

                    m_BattleDispEnemy.instanceObject(m_BattleEnemys);

                    m_BattleDispEnemy.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BATTLE"));  // レイヤーを設定し直し

                    m_BattleDispEnemy.showTargetCursor(0);  // ターゲットカーソルを表示
                }
            }
            else
            {
                m_BattleDispEnemy.instanceObject(null);
            }
        }
    }

    private void setInfoToInteface()
    {
        m_CallCount++;
        if (m_CurrentBattleEnemy != null)
        {
            MasterDataParamChara chara_master = m_CurrentBattleEnemy.getMasterDataParamChara();
            textEnemyName.Value = chara_master.name + "(FixID:" + chara_master.fix_id.ToString() + ")(DrawID:" + chara_master.draw_id.ToString() + ")";
        }

        if (m_CurrentEnemyDispInfo != null)
        {
            m_TogglePos.isOn = m_CurrentEnemyDispInfo.m_IsUsePosition;
            m_InputFieldPosX.text = (m_CurrentEnemyDispInfo.m_PositionX / 1000.0f).ToString();
            m_InputFieldPosY.text = (m_CurrentEnemyDispInfo.m_PositionY / 1000.0f).ToString();

            m_DropdwonHpType.value = (int)m_CurrentEnemyDispInfo.m_HpGaugeType;
            m_InputFieldHpX.text = (m_CurrentEnemyDispInfo.m_HpGaugeX / 1000.0f).ToString();
            m_InputFieldHpY.text = (m_CurrentEnemyDispInfo.m_HpGaugeY / 1000.0f).ToString();

            m_ToggleTgt.isOn = m_CurrentEnemyDispInfo.m_IsUseTargetPosition;
            m_InputFieldTgtX.text = (m_CurrentEnemyDispInfo.m_TargetX / 1000.0f).ToString();
            m_InputFieldTgtY.text = (m_CurrentEnemyDispInfo.m_TargetY / 1000.0f).ToString();

        }
        m_CallCount--;
    }

    private void setInterfaceToInfo()
    {
        m_CallCount++;
        if (m_CurrentEnemyDispInfo != null)
        {
            m_CurrentEnemyDispInfo.m_IsUsePosition = m_TogglePos.isOn;
            m_CurrentEnemyDispInfo.m_PositionX = (int)(float.Parse(m_InputFieldPosX.text) * 1000.1f);
            m_CurrentEnemyDispInfo.m_PositionY = (int)(float.Parse(m_InputFieldPosY.text) * 1000.1f);

            m_CurrentEnemyDispInfo.m_HpGaugeType = (MasterDataDefineLabel.HPGaugeType)m_DropdwonHpType.value;
            m_CurrentEnemyDispInfo.m_HpGaugeX = (int)(float.Parse(m_InputFieldHpX.text) * 1000.1f);
            m_CurrentEnemyDispInfo.m_HpGaugeY = (int)(float.Parse(m_InputFieldHpY.text) * 1000.1f);

            m_CurrentEnemyDispInfo.m_IsUseTargetPosition = m_ToggleTgt.isOn;
            m_CurrentEnemyDispInfo.m_TargetX = (int)(float.Parse(m_InputFieldTgtX.text) * 1000.1f);
            m_CurrentEnemyDispInfo.m_TargetY = (int)(float.Parse(m_InputFieldTgtY.text) * 1000.1f);
        }
        m_CallCount--;
    }

    private void setInfoToWorkMaster()
    {
        m_CallCount++;
        if (m_CurrentBattleEnemy != null && m_CurrentEnemyDispInfo != null)
        {
            MasterDataParamEnemy enemy_master = m_CurrentBattleEnemy.getMasterDataParamEnemy(); // このBattleEnemyの敵マスターは一時コピーなので書き換えてＯＫ.

            enemy_master.pos_absolute = (m_CurrentEnemyDispInfo.m_IsUsePosition ? MasterDataDefineLabel.BoolType.ENABLE : MasterDataDefineLabel.BoolType.NONE);
            enemy_master.posx_offset = m_CurrentEnemyDispInfo.m_PositionX;
            enemy_master.posy_offset = m_CurrentEnemyDispInfo.m_PositionY;

            enemy_master.hp_gauge_type = m_CurrentEnemyDispInfo.m_HpGaugeType;
            enemy_master.hp_posx_offset = m_CurrentEnemyDispInfo.m_HpGaugeX;
            enemy_master.hp_posy_offset = m_CurrentEnemyDispInfo.m_HpGaugeY;

            enemy_master.target_pos_absolute = (m_CurrentEnemyDispInfo.m_IsUseTargetPosition ? (int)MasterDataDefineLabel.BoolType.ENABLE : (int)MasterDataDefineLabel.BoolType.NONE);
            enemy_master.target_posx_offset = m_CurrentEnemyDispInfo.m_TargetX;
            enemy_master.target_posy_offset = m_CurrentEnemyDispInfo.m_TargetY;
        }
        m_CallCount--;
    }
}
