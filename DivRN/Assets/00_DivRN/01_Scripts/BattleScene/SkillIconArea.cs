using UnityEngine;
using System.Collections;

public class SkillIconArea : MonoBehaviour
{
    public GameObject m_SkillIconPanelPrefab = null;
    [Tooltip("無属性スキルアイコン（単体攻撃・全体攻撃・回復・特殊）")]
    public Sprite[] m_SpritesNaught = new Sprite[4];

    [Tooltip("火属性スキルアイコン（単体攻撃・全体攻撃・回復・特殊）")]
    public Sprite[] m_SpritesFire = new Sprite[4];

    [Tooltip("水属性スキルアイコン（単体攻撃・全体攻撃・回復・特殊）")]
    public Sprite[] m_SpritesWater = new Sprite[4];

    [Tooltip("光属性スキルアイコン（単体攻撃・全体攻撃・回復・特殊）")]
    public Sprite[] m_SpritesLight = new Sprite[4];

    [Tooltip("闇属性スキルアイコン（単体攻撃・全体攻撃・回復・特殊）")]
    public Sprite[] m_SpritesDark = new Sprite[4];

    [Tooltip("風属性スキルアイコン（単体攻撃・全体攻撃・回復・特殊）")]
    public Sprite[] m_SpritesWind = new Sprite[4];

    [Tooltip("回復属性スキルアイコン（１・２・３・４）")]
    public Sprite[] m_SpritesHeal = new Sprite[4];

    [Tooltip("復活スキルアイコン（１・２・３・４）")]
    public Sprite[] m_SpritesResurr = new Sprite[4];

    private int m_FieldAreaCount;
    private int m_SkillCountMax;

    // ゲームオブジェクト・コンポーネントへのポインタ（毎フレーム、オブジェクト検索やGetComponentすると重くなる可能性があるので）
    private GameObject[,] m_IconObjects = null;
    private SpriteRenderer[,] mSpriteRenderers = null;

    private uint[,] m_SkillIDs = null;
    private GlobalDefine.PartyCharaIndex[,] m_SkillCasters = null;
    private int[] m_SkillCounts = null;

    private MasterDataDefineLabel.ElementType[,] m_NewSkillElements = null;

    private int m_ComboSoundIndex = 0;

    private struct WorkSkillInfo
    {
        public uint m_SkillID;
        public int m_FieldIndex;
        public GlobalDefine.PartyCharaIndex m_Caster;
    }

    private WorkSkillInfo[] m_WorkAppearSkillInfos = new WorkSkillInfo[155];

    // Use this for initialization
    void Start()
    {

        init(5, 35);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IconObjects != null)
        {
            for (int field_idx = 0; field_idx < m_FieldAreaCount; field_idx++)
            {
                for (int skill_idx = 0; skill_idx < m_SkillCounts[field_idx]; skill_idx++)
                {
                    Sprite sprite = null;
                    MasterDataSkillActive master_data_skill_active = BattleParam.m_MasterDataCache.useSkillActive((uint)m_SkillIDs[field_idx, skill_idx]);
                    if (master_data_skill_active != null)
                    {
                        Sprite[] element_sprite = null;
                        switch (master_data_skill_active.skill_element)
                        {
                            case MasterDataDefineLabel.ElementType.NAUGHT:
                                element_sprite = m_SpritesNaught;
                                break;

                            case MasterDataDefineLabel.ElementType.FIRE:
                                element_sprite = m_SpritesFire;
                                break;

                            case MasterDataDefineLabel.ElementType.WATER:
                                element_sprite = m_SpritesWater;
                                break;

                            case MasterDataDefineLabel.ElementType.LIGHT:
                                element_sprite = m_SpritesLight;
                                break;

                            case MasterDataDefineLabel.ElementType.DARK:
                                element_sprite = m_SpritesDark;
                                break;

                            case MasterDataDefineLabel.ElementType.WIND:
                                element_sprite = m_SpritesWind;
                                break;

                            case MasterDataDefineLabel.ElementType.HEAL:
                                element_sprite = m_SpritesHeal;
                                break;
                        }

                        if (element_sprite != null)
                        {
                            switch (master_data_skill_active.skill_type)
                            {
                                case MasterDataDefineLabel.SkillType.ATK_ONCE:
                                    sprite = element_sprite[0];
                                    break;

                                case MasterDataDefineLabel.SkillType.ATK_ALL:
                                    sprite = element_sprite[1];
                                    break;

                                case MasterDataDefineLabel.SkillType.HEAL:
                                    sprite = m_SpritesHeal[1];  // 属性に関係なく回復アイコンを出す.
                                    break;

                                case MasterDataDefineLabel.SkillType.SUPPORT:
                                    sprite = element_sprite[1];
                                    break;
                            }
                        }

                        ResurrectInfo resurrent_info = master_data_skill_active.getResurrectInfo();
                        if (resurrent_info != null)
                        {
                            int recov_count = resurrent_info.m_FixCount + resurrent_info.m_AddCount;
                            if (recov_count >= 2)
                            {
                                sprite = m_SpritesResurr[1];
                            }
                            else
                            {
                                sprite = m_SpritesResurr[0];
                            }
                        }
                    }

                    if (sprite != null)
                    {
                        mSpriteRenderers[field_idx, skill_idx].sprite = sprite;
                        if (m_IconObjects[field_idx, skill_idx].IsActive() == false)
                        {
                            m_IconObjects[field_idx, skill_idx].SetActive(true);
                        }
                    }
                    else
                    {
                        if (m_IconObjects[field_idx, skill_idx].IsActive())
                        {
                            m_IconObjects[field_idx, skill_idx].SetActive(false);
                        }
                    }
                }

                for (int skill_idx = m_SkillCounts[field_idx]; skill_idx < m_SkillCountMax; skill_idx++)
                {
                    if (m_IconObjects[field_idx, skill_idx].IsActive())
                    {
                        m_IconObjects[field_idx, skill_idx].SetActive(false);
                    }
                }
            }
        }
    }

    public void init(int field_area_count, int skill_count_max)
    {
        m_FieldAreaCount = field_area_count;
        m_SkillCountMax = skill_count_max;

        m_IconObjects = new GameObject[m_FieldAreaCount, m_SkillCountMax];
        mSpriteRenderers = new SpriteRenderer[m_FieldAreaCount, m_SkillCountMax];
        m_SkillIDs = new uint[m_FieldAreaCount, m_SkillCountMax];
        m_SkillCasters = new GlobalDefine.PartyCharaIndex[m_FieldAreaCount, m_SkillCountMax];
        m_SkillCounts = new int[m_FieldAreaCount];

        for (int field_idx = 0; field_idx < m_FieldAreaCount; field_idx++)
        {
            for (int skill_idx = 0; skill_idx < m_SkillCountMax; skill_idx++)
            {
                GameObject icon_object = Instantiate(m_SkillIconPanelPrefab);

                BattleSceneUtil.setRide(icon_object.transform, gameObject.transform);
                icon_object.transform.localPosition = new Vector3(field_idx - (m_IconObjects.GetLength(0) - 1) * 0.5f + ((skill_idx % 10) - 4.5f) * 0.08f, (skill_idx / 10) * 0.1f, 0.0f);
                icon_object.SetActive(false);

                m_IconObjects[field_idx, skill_idx] = icon_object;

                GameObject child = icon_object.transform.GetChild(0).gameObject;
                SpriteRenderer spr = child.GetComponent<SpriteRenderer>();
                mSpriteRenderers[field_idx, skill_idx] = spr;
            }
        }

        m_NewSkillElements = new MasterDataDefineLabel.ElementType[m_FieldAreaCount, (int)GlobalDefine.PartyCharaIndex.MAX];
    }

    public int setSkillInfos(BattleSkillReq[] skill_infos, int skill_info_count)
    {
        int ret_val = -1;

        if (skill_infos == null || skill_info_count == 0)
        {
            for (int field_idx = 0; field_idx < m_FieldAreaCount; field_idx++)
            {
                for (int skill_idx = 0; skill_idx < m_SkillCountMax; skill_idx++)
                {
                    m_SkillIDs[field_idx, skill_idx] = 0;
                    m_SkillCasters[field_idx, skill_idx] = GlobalDefine.PartyCharaIndex.ERROR;
                }
                m_SkillCounts[field_idx] = 0;
            }

            m_ComboSoundIndex = 0;
        }
        else
        {
            // スキルの増減を調べる
            int appear_skill_count = 0;

            {
                bool[,] is_cheked = new bool[m_FieldAreaCount, m_SkillCountMax];

                for (int req_idx = 0; req_idx < skill_info_count; req_idx++)
                {
                    BattleSkillReq battle_skill_req = skill_infos[req_idx];
                    if (battle_skill_req != null && battle_skill_req.m_SkillReqState != BattleSkillReq.State.NONE && battle_skill_req.m_SkillParamSkillID != 0)
                    {
                        uint skill_id = battle_skill_req.m_SkillParamSkillID;
                        int field_idx = battle_skill_req.m_SkillParamFieldNum;
                        GlobalDefine.PartyCharaIndex caster_idx = battle_skill_req.m_SkillParamCharaNum;

                        bool is_exsit = false;
                        for (int idx = 0; idx < m_SkillCounts[field_idx]; idx++)
                        {
                            if (is_cheked[field_idx, idx] == false
                                && m_SkillIDs[field_idx, idx] == skill_id
                                && m_SkillCasters[field_idx, idx] == caster_idx
                            )
                            {
                                is_cheked[field_idx, idx] = true;
                                is_exsit = true;
                                break;
                            }
                        }

                        if (is_exsit == false)
                        {
                            m_WorkAppearSkillInfos[appear_skill_count].m_SkillID = skill_id;
                            m_WorkAppearSkillInfos[appear_skill_count].m_FieldIndex = field_idx;
                            m_WorkAppearSkillInfos[appear_skill_count].m_Caster = caster_idx;
                            appear_skill_count++;
                        }
                    }
                }

            }

            for (int field_idx = 0; field_idx < m_FieldAreaCount; field_idx++)
            {
                m_SkillCounts[field_idx] = 0;
            }

            skill_info_count = Mathf.Min(skill_info_count, skill_infos.Length);
            for (int idx = 0; idx < skill_info_count; idx++)
            {
                BattleSkillReq battle_skill_req = skill_infos[idx];
                if (battle_skill_req != null && battle_skill_req.m_SkillReqState != BattleSkillReq.State.NONE && battle_skill_req.m_SkillParamSkillID != 0)
                {
                    int field_idx = battle_skill_req.m_SkillParamFieldNum;

                    int skill_idx = m_SkillCounts[field_idx];
                    if (skill_idx < m_SkillCountMax)
                    {
                        m_SkillIDs[field_idx, skill_idx] = battle_skill_req.m_SkillParamSkillID;
                        m_SkillCasters[field_idx, skill_idx] = battle_skill_req.m_SkillParamCharaNum;
                        m_SkillCounts[field_idx]++;
                    }
                }
            }

            if (appear_skill_count > 0)
            {
                SEID seID;

                switch (m_ComboSoundIndex)
                {
                    case 0: seID = SEID.SE_SKILL_COMBO_00; break;
                    case 1: seID = SEID.SE_SKILL_COMBO_01; break;
                    case 2: seID = SEID.SE_SKILL_COMBO_02; break;
                    case 3: seID = SEID.SE_SKILL_COMBO_03; break;
                    case 4: seID = SEID.SE_SKILL_COMBO_04; break;
                    case 5: seID = SEID.SE_SKILL_COMBO_05; break;
                    case 6: seID = SEID.SE_SKILL_COMBO_06; break;
                    case 7: seID = SEID.SE_SKILL_COMBO_07; break;
                    case 8: seID = SEID.SE_SKILL_COMBO_08; break;
                    default: seID = SEID.SE_SKILL_COMBO_MORE_THAN_08; break;
                }
                SoundUtil.PlaySE(seID);

                m_ComboSoundIndex++;
            }

            if (appear_skill_count > 0)
            {
                clearNewSkillElementInfo();

                for (int idx = 0; idx < appear_skill_count; idx++)
                {
                    int field_idx = m_WorkAppearSkillInfos[idx].m_FieldIndex;
                    GlobalDefine.PartyCharaIndex caster_idx = m_WorkAppearSkillInfos[idx].m_Caster;
                    if (caster_idx == GlobalDefine.PartyCharaIndex.GENERAL)
                    {
                        caster_idx = BattleParam.m_PlayerParty.getGeneralPartyMember();
                    }

                    if (m_NewSkillElements[field_idx, (int)caster_idx] == MasterDataDefineLabel.ElementType.NONE)
                    {
                        MasterDataSkillActive master_data_skill_active = BattleParam.m_MasterDataCache.useSkillActive(m_WorkAppearSkillInfos[idx].m_SkillID);
                        if (master_data_skill_active != null)
                        {
                            m_NewSkillElements[field_idx, (int)caster_idx] = master_data_skill_active.skill_element;
                        }
                    }
                }

                ret_val = m_WorkAppearSkillInfos[0].m_FieldIndex;
            }
        }

        return ret_val;
    }

    public void clearNewSkillElementInfo()
    {
        for (int field_idx = 0; field_idx < m_FieldAreaCount; field_idx++)
        {
            for (int member_idx = 0; member_idx < (int)GlobalDefine.PartyCharaIndex.MAX; member_idx++)
            {
                m_NewSkillElements[field_idx, member_idx] = MasterDataDefineLabel.ElementType.NONE;
            }
        }
    }

    /// <summary>
    /// 新しく成立したスキル情報
    /// </summary>
    /// <returns>１次元目が場の番号、２次元目がパーティメンバー番号</returns>
    public MasterDataDefineLabel.ElementType[,] getNewSkillElementInfo()
    {
        return m_NewSkillElements;
    }


}
