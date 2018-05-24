using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyTweenExt;
using TMPro;

public class NewBattleSkillCutin : MonoBehaviour
{
    public GameObject m_BgObjectPrefab = null;
    public GameObject m_SkillObjectPrefab = null;

    public Material m_ShadowMaterial = null;
    public Material m_ShadowMaterialAlphaMask = null;

    public GameObject m_SkillLocateCurrent = null;
    public GameObject m_SkillLocateNext1 = null;
    public GameObject m_SkillLocateNext2 = null;
    public GameObject m_SkillLocateNext3 = null;

    public Sprite[] m_BgImages = new Sprite[(int)MasterDataDefineLabel.ElementType.MAX];

    private class CutinObjectInfo
    {
        public static readonly float ANIM_TIME_IN = 0.20f;
        public static readonly float ANIM_TIME_OUT = 0.2f;
        public static readonly float ANIM_TIME_SKILL = 0.3f;

        public GameObject m_RootObject;
        public Transform m_BGTransform;
        private Image m_BGImage;
        private Image m_BGChangeImage;	// 属性切り替えアニメ用
        private Transform m_CharaTransform;
        private BattleCharaImageViewControl m_CharaImageControl;
        private Image m_CharaImage;
        private Image m_CharaShadowImage;
        private NewBattleSkillCutin m_OwnerNewBattleSkillCutin = null;

        public CutinObjectInfo(GameObject prefab_obj, Transform parent_transform, NewBattleSkillCutin owner_battle_skill_cutin)
        {
            m_OwnerNewBattleSkillCutin = owner_battle_skill_cutin;

            GameObject cutin_object = Instantiate(prefab_obj);
            cutin_object.transform.SetParent(parent_transform, false);
            cutin_object.SetActive(false);

            m_RootObject = cutin_object;

            m_BGTransform = cutin_object.transform.Find("BG");
            m_BGImage = m_BGTransform.GetComponent<Image>();
            m_BGChangeImage = m_BGTransform.Find("BGChange").GetComponent<Image>();
            m_BGChangeImage.gameObject.SetActive(false);
            m_CharaTransform = cutin_object.transform.Find("Chara");

            m_CharaImageControl = m_CharaTransform.transform.Find("Image").GetComponent<BattleCharaImageViewControl>();
            m_CharaImage = m_CharaTransform.transform.Find("Image").GetComponent<Image>();
            m_CharaShadowImage = m_CharaTransform.transform.Find("Shadow/ImageShadow").GetComponent<Image>();
        }

        public enum Phase
        {
            NONE,
            INIT,
            APPEAR,
            SKILL,
            DISSPPEAR,
        }

        private Phase m_Phase = Phase.NONE;
        private float m_Timer = 0.0f;

        private float m_BGChangeImageAnimationTime = 0.0f;

        public Phase getPhase()
        {
            return m_Phase;
        }

        public void update(float delta_time)
        {
            if (m_Timer > 0.0f)
            {
                m_Timer -= delta_time * m_OwnerNewBattleSkillCutin.m_AnimSpeed;

                if (m_Timer <= 0.0f)
                {
                    if (m_Phase == Phase.APPEAR)
                    {
                        animSkill();
                    }
                    else
                    if (m_Phase == Phase.DISSPPEAR)
                    {
                        initAnim(0, null, true);
                    }
                }
            }

            if (m_CharaShadowImage.sprite != m_CharaImage.sprite)
            {
                m_CharaShadowImage.sprite = m_CharaImage.sprite;
                m_CharaShadowImage.transform.localPosition = m_CharaImage.transform.localPosition;
                m_CharaShadowImage.transform.localScale = m_CharaImage.transform.localScale;
                m_CharaShadowImage.SetNativeSize();
                m_CharaShadowImage.GetComponent<RectTransform>().pivot = m_CharaImage.GetComponent<RectTransform>().pivot;

                Texture mask_textrue = m_CharaImageControl.getMaskTexture();
                if (mask_textrue == null)
                {
                    m_CharaShadowImage.material = m_OwnerNewBattleSkillCutin.m_ShadowMaterial;
                }
                else
                {
                    m_CharaShadowImage.material = m_OwnerNewBattleSkillCutin.m_ShadowMaterialAlphaMask;
                    m_CharaShadowImage.material.SetTexture("_AlphaTex", mask_textrue);
                }
            }

            if (m_CharaImage.sprite != null)
            {
                m_CharaImage.enabled = true;
                m_CharaShadowImage.enabled = true;
            }
            else
            {
                m_CharaImage.enabled = false;
                m_CharaShadowImage.enabled = false;
            }

            if (m_BGChangeImageAnimationTime > 0.0f)
            {
                m_BGChangeImageAnimationTime -= delta_time;
                if (m_BGChangeImageAnimationTime <= 0.0f)
                {
                    m_BGImage.sprite = m_BGChangeImage.sprite;
                    m_BGImage.SetNativeSize();
                    m_BGChangeImage.gameObject.SetActive(false);
                }
            }
        }

        public void initAnim(int chara_id, Sprite bg_elem_sprite, bool is_enable_chara_image)
        {
            // 以前のアニメ停止
            m_RootObject.transform.killTween();
            m_BGTransform.killTween();
            m_CharaTransform.killTween();
            m_RootObject.SetActive(false);

            // アニメ初期位置
            m_BGImage.sprite = bg_elem_sprite;
            m_BGImage.SetNativeSize();

            m_CharaImageControl.clearCharaID();
            m_CharaImage.sprite = null;
            m_CharaShadowImage.sprite = null;
            if (is_enable_chara_image)
            {
                m_CharaImageControl.setCharaID(chara_id, BattleCharaImageViewControl.ImageType.CUTIN_DISP2);
            }

            m_RootObject.transform.localPosition = Vector3.zero;
            m_BGTransform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
            m_CharaTransform.localPosition = new Vector3(1000.0f, 0.0f, 0.0f);

            if (chara_id != 0)
            {
                m_RootObject.SetActive(true);
            }

            m_Phase = Phase.INIT;

            m_BGChangeImageAnimationTime = 0.0f;
            m_BGChangeImage.gameObject.SetActive(false);
        }

        public void animAppear()
        {
            m_RootObject.transform.killTween();
            m_BGTransform.killTween();
            m_CharaTransform.killTween();

            m_RootObject.transform.localPosition = Vector3.zero;
            m_BGTransform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
            m_CharaTransform.localPosition = new Vector3(1000.0f, 0.0f, 0.0f);

            // アニメ目標位置
            m_BGTransform.tween(Vector3.zero, Quaternion.identity, Vector3.one, ANIM_TIME_IN / m_OwnerNewBattleSkillCutin.m_AnimSpeed);
            m_CharaTransform.tween(Vector3.zero, Quaternion.identity, Vector3.one, ANIM_TIME_IN / m_OwnerNewBattleSkillCutin.m_AnimSpeed);

            m_RootObject.SetActive(true);

            m_Timer = ANIM_TIME_IN;
            m_Phase = Phase.APPEAR;

            m_BGChangeImageAnimationTime = 0.0f;
            m_BGChangeImage.gameObject.SetActive(false);
        }

        public void animSkill()
        {
            m_RootObject.transform.killTween(true);
            m_BGTransform.killTween(true);
            m_CharaTransform.killTween(true);

            m_RootObject.transform.localPosition = Vector3.zero;
            m_BGTransform.localScale = Vector3.one;
            m_CharaTransform.localPosition = Vector3.zero;

            // スキル発動中もキャラを少しずつスクロール
            m_CharaTransform.tween(new Vector3(-20, 0.0f, 0.0f), Quaternion.identity, Vector3.one, ANIM_TIME_SKILL * 10.0f / m_OwnerNewBattleSkillCutin.m_AnimSpeed);
            m_Phase = Phase.SKILL;
        }

        public void animDisappear()
        {
            m_RootObject.transform.killTween(true);
            m_BGTransform.killTween(true);

            m_RootObject.transform.localPosition = Vector3.zero;
            m_BGTransform.localScale = Vector3.one;

            m_RootObject.transform.tween(new Vector3(0.0f, -800.0f, 0.0f), Quaternion.identity, Vector3.one, ANIM_TIME_OUT / m_OwnerNewBattleSkillCutin.m_AnimSpeed, MyTween.EaseType.IN_QUAD);

            m_Timer = ANIM_TIME_OUT;
            m_Phase = Phase.DISSPPEAR;
        }

        /// <summary>
        /// 下地の属性変更アニメーション開始（属性が変わるときのみ再生される）
        /// </summary>
        /// <param name="bg_sprite"></param>
        /// <param name="duration"></param>
        public void startChangeElementAnimation(Sprite bg_sprite, float duration)
        {
            if (bg_sprite != m_BGImage.sprite)
            {
                m_BGChangeImage.transform.killTween(false);
                m_BGChangeImage.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
                m_BGChangeImage.sprite = bg_sprite;
                m_BGChangeImage.SetNativeSize();

                m_BGChangeImageAnimationTime = duration;
                m_BGChangeImage.gameObject.SetActive(true);

                m_BGChangeImage.transform.tween(Vector3.zero, Quaternion.identity, Vector3.one, duration);
            }
        }
    }

    private CutinObjectInfo[] m_CutinObjects = new CutinObjectInfo[2];
    private GameObject[] m_SkillObjects = new GameObject[3];

    private SkillInfo m_ResurrectInfo = null;
    private Vector3 m_ResurrectUnitPos0;
    private Vector3 m_ResurrectUnitPos1;

    private class SkillInfo
    {
        public SkillInfo(GlobalDefine.PartyCharaIndex party_member_index, string skill_name, ESKILLTYPE skill_type, MasterDataDefineLabel.ElementType skill_element, int active_skill_index, int skill_index)
        {
            m_Caster = party_member_index;
            if (m_Caster == GlobalDefine.PartyCharaIndex.GENERAL)
            {
                m_Caster = BattleParam.m_PlayerParty.getGeneralPartyMember();
            }

            m_SkillName = skill_name;
            m_SkillType = skill_type;
            m_SkillElement = skill_element;
            m_ActiveSkillIndex = active_skill_index;
            m_SkillIndex = skill_index;
            m_ResurrectFlag = 0;

            m_Key = ((int)m_Caster)
                + ((m_SkillType == ESKILLTYPE.eLINK || m_SkillType == ESKILLTYPE.eLINKPASSIVE) ? 100 : 0);
        }

        public GlobalDefine.PartyCharaIndex m_Caster;	// 発動者
        public string m_SkillName;	// スキル名
        public int m_SkillIndex = 0;
        public ESKILLTYPE m_SkillType = ESKILLTYPE.eACTIVE;
        public MasterDataDefineLabel.ElementType m_SkillElement = MasterDataDefineLabel.ElementType.NONE;
        public int m_ActiveSkillIndex = 0;	// アクティブスキルの１番目か２番目か
        public int m_ResurrectFlag = 0;
        public int m_Key;	// 下地切り替えタイミング判定用
    }

    private List<SkillInfo> m_SkillInfos = new List<SkillInfo>();
    private int m_SkillIndex = 0;
    private Transform m_SkillAreaTransform = null;

    private enum UpdateMode
    {
        NONE,	// 未実行
        AUTO,	// オートモードで実行中
        STEP,	// カットイン単位での待ちモードで実行中
        STEP_WAIT,	// カットイン単位での待ちモードで待機中
    }
    private UpdateMode m_UpdateMode = UpdateMode.NONE;
    private bool m_IsEnableCharaImage = true;
    private bool m_IsNoCutinMode = false;


    private SkillInfo m_CurrentSkillInfo = null;


    private CutinObjectInfo m_CurrentCutinObjectInfo = null;
    private CutinObjectInfo m_DisappearCutinObjectInfo = null;

    private float m_SkillTime = 0.0f;

    private bool m_IsSpeedUp = false;
    private float m_AnimSpeed = 1.0f;

    // Use this for initialization
    void Start()
    {
        for (int idx = 0; idx < m_CutinObjects.Length; idx++)
        {
            m_CutinObjects[idx] = new CutinObjectInfo(m_BgObjectPrefab, transform, this);
        }

        m_SkillAreaTransform = transform.Find("SkillArea");

        for (int obj_idx = 0; obj_idx < m_SkillObjects.Length; obj_idx++)
        {
            GameObject skill_object = Instantiate(m_SkillObjectPrefab);
            m_SkillObjects[obj_idx] = skill_object;

            skill_object.transform.SetParent(m_SkillLocateCurrent.transform, false);

            // キャラアイコンを設定
            for (int member_idx = 0; member_idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); member_idx++)
            {
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)member_idx, CharaParty.CharaCondition.EXIST);
                if (chara_once != null)
                {
                    Transform unit_obj = skill_object.transform.Find("Unit" + member_idx.ToString());
                    if (unit_obj != null
                        && unit_obj.childCount > 0
                    )
                    {
                        Transform unit_obj_child = unit_obj.GetChild(0);
                        if (unit_obj_child != null)
                        {
                            Image unit_image = unit_obj_child.GetComponent<Image>();

                            if (unit_image != null)
                            {
                                UnitIconImageProvider.Instance.Get(
                                    chara_once.m_CharaMasterDataParam.fix_id,
                                    sprite =>
                                    {
                                        unit_image.sprite = sprite;
                                    });
                            }
                        }
                    }
                }
            }

            skill_object.SetActive(false);
        }

        m_ResurrectUnitPos0 = m_SkillObjects[0].transform.Find("Unit0").localPosition;
        m_ResurrectUnitPos1 = m_SkillObjects[0].transform.Find("Unit1").localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_UpdateMode == UpdateMode.NONE)
        {
            return;
        }

        m_AnimSpeed = (m_IsSpeedUp) ? 1.75f : 1.0f;
        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
        if (cOption != null)
        {
            m_AnimSpeed *= (cOption.m_OptionSpeed != 0) ? 1.0f : 0.6f;
        }

        float delta_time = Time.deltaTime;

        for (int idx = 0; idx < m_CutinObjects.Length; idx++)
        {
            m_CutinObjects[idx].update(delta_time);
        }

        if (m_DisappearCutinObjectInfo != null)
        {
            if (m_DisappearCutinObjectInfo.getPhase() != CutinObjectInfo.Phase.DISSPPEAR)
            {
                m_DisappearCutinObjectInfo = null;
            }
        }

        if (m_CurrentCutinObjectInfo != null && m_CurrentCutinObjectInfo.getPhase() == CutinObjectInfo.Phase.APPEAR)
        {
            // スキル表示物を下地のスケールに合わせる（キャラ出現時用）
            m_SkillAreaTransform.localScale = m_CurrentCutinObjectInfo.m_BGTransform.localScale;
            return;
        }
        m_SkillAreaTransform.localScale = Vector3.one;

        updateSkillAnim(delta_time);

        if (m_UpdateMode != UpdateMode.STEP_WAIT && m_SkillTime <= 0.0f)
        {
            // 割り込みスキル
            if (m_ResurrectInfo != null)
            {
                m_SkillInfos.Insert(0, m_ResurrectInfo);
                m_ResurrectInfo = null;
            }

            // アニメーションしていないので次のスキルへ
            if (m_IsNoCutinMode == false)
            {
                if (m_SkillInfos.Count > 0)
                {
                    int old_key = -1;

                    if (m_CurrentSkillInfo != null)
                    {
                        old_key = m_CurrentSkillInfo.m_Key;
                    }

                    m_CurrentSkillInfo = m_SkillInfos[0];
                    m_SkillInfos.RemoveAt(0);

                    if (old_key != m_CurrentSkillInfo.m_Key)
                    {
                        int caster_chara_id = 0;
                        MasterDataDefineLabel.ElementType skill_element = m_CurrentSkillInfo.m_SkillElement;
                        if (m_CurrentSkillInfo.m_Caster == GlobalDefine.PartyCharaIndex.HERO)
                        {
                            caster_chara_id = 0;
                            if (UserDataAdmin.HasInstance)
                            {
                                ServerDataDefine.PacketStructHero current_hero = UserDataAdmin.Instance.getCurrentHero();
                                if (current_hero != null)
                                {
                                    MasterDataHero master_data_hero = MasterFinder<MasterDataHero>.Instance.Find(current_hero.hero_id);
                                    if (master_data_hero != null)
                                    {
                                        caster_chara_id = -current_hero.hero_id;
                                        if (skill_element == MasterDataDefineLabel.ElementType.NONE)
                                        {
                                            skill_element = master_data_hero.getElement();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(m_CurrentSkillInfo.m_Caster, CharaParty.CharaCondition.EXIST);
                            if (chara_once != null)
                            {
                                MasterDataParamChara chara_master = chara_once.m_CharaMasterDataParam;

                                if ((m_CurrentSkillInfo.m_SkillType == ESKILLTYPE.eLINK || m_CurrentSkillInfo.m_SkillType == ESKILLTYPE.eLINKPASSIVE)
                                    && chara_once.m_LinkParam != null
                                )
                                {
                                    MasterDataParamChara link_chara_master = chara_once.m_LinkParam.m_cCharaMasterDataParam;
                                    if (link_chara_master != null)
                                    {
                                        chara_master = link_chara_master;
                                    }
                                }

                                caster_chara_id = (int)chara_master.fix_id;
                                if (skill_element == MasterDataDefineLabel.ElementType.NONE)
                                {
                                    skill_element = chara_master.element;
                                }
                            }
                        }

                        if (m_CurrentCutinObjectInfo != null)
                        {
                            m_DisappearCutinObjectInfo = m_CurrentCutinObjectInfo;
                            m_CurrentCutinObjectInfo = null;
                            m_DisappearCutinObjectInfo.animDisappear();
                        }

                        if (m_DisappearCutinObjectInfo != m_CutinObjects[0])
                        {
                            m_CurrentCutinObjectInfo = m_CutinObjects[0];
                            m_CutinObjects[1].m_RootObject.transform.SetSiblingIndex(0);
                            m_CutinObjects[0].m_RootObject.transform.SetSiblingIndex(1);
                        }
                        else
                        {
                            m_CurrentCutinObjectInfo = m_CutinObjects[1];
                            m_CutinObjects[0].m_RootObject.transform.SetSiblingIndex(0);
                            m_CutinObjects[1].m_RootObject.transform.SetSiblingIndex(1);
                        }

                        m_CurrentCutinObjectInfo.initAnim(caster_chara_id, m_BgImages[(int)skill_element], m_IsEnableCharaImage);
                        m_CurrentCutinObjectInfo.animAppear();

                        // キャラ出現時用のスキル表示物を設定
                        //if (m_CurrentSkillInfo != null)
                        {
                            m_SkillObjects[0].SetActive(false);

                            setupSkillPanel(m_SkillObjects[1], m_CurrentSkillInfo, m_SkillLocateNext1.transform);
                            m_SkillObjects[1].SetActive(true);

                            if (m_SkillInfos.Count >= 1 && m_CurrentSkillInfo.m_Key == m_SkillInfos[0].m_Key)
                            {
                                setupSkillPanel(m_SkillObjects[2], m_SkillInfos[0], m_SkillLocateNext2.transform);
                                m_SkillObjects[2].SetActive(true);
                            }
                            else
                            {
                                m_SkillObjects[2].SetActive(false);
                            }
                        }

                        m_SkillAreaTransform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
                    }
                    else
                    {
                        if (m_CurrentCutinObjectInfo != null
                            && m_CurrentSkillInfo != null
                            && m_CurrentSkillInfo.m_SkillElement != MasterDataDefineLabel.ElementType.NONE
                        )
                        {
                            Sprite bg_elem_sprite = m_BgImages[(int)m_CurrentSkillInfo.m_SkillElement];
                            m_CurrentCutinObjectInfo.startChangeElementAnimation(bg_elem_sprite, CutinObjectInfo.ANIM_TIME_SKILL * 0.25f / m_AnimSpeed);
                        }
                    }

                    startSkillAnim();
                }
                else
                {
                    if (m_ResurrectInfo == null)
                    {
                        m_CurrentSkillInfo = null;
                        if (m_CurrentCutinObjectInfo != null)
                        {
                            // キャラの消滅アニメ開始
                            m_DisappearCutinObjectInfo = m_CurrentCutinObjectInfo;
                            m_CurrentCutinObjectInfo = null;
                            m_DisappearCutinObjectInfo.animDisappear();
                            m_UpdateMode = UpdateMode.AUTO;
                        }
                        else
                        {
                            // すべて終了
                            if (m_DisappearCutinObjectInfo == null)
                            {
                                m_UpdateMode = UpdateMode.NONE;
                            }
                        }
                    }
                }
            }
            else
            {
                if (m_SkillInfos.Count > 0)
                {
                    m_CurrentSkillInfo = m_SkillInfos[0];
                    m_SkillInfos.RemoveAt(0);
                    startSkillAnim();
                }
                else
                {
                    m_CurrentSkillInfo = null;
                    m_UpdateMode = UpdateMode.NONE;
                }
            }
        }
    }

    public void clearSkill()
    {
        m_SkillInfos.Clear();
        m_SkillIndex = 0;
        m_UpdateMode = UpdateMode.NONE;
        m_IsEnableCharaImage = true;
        m_IsNoCutinMode = false;
        setSpeedUp(false);
    }

    public void startCutin(bool is_step_mode, bool is_no_chara_mode, bool is_no_cutin_mode)
    {
        if (is_step_mode)
        {
            m_UpdateMode = UpdateMode.STEP;
        }
        else
        {
            m_UpdateMode = UpdateMode.AUTO;
        }

        m_IsEnableCharaImage = !is_no_chara_mode;
        m_IsNoCutinMode = is_no_cutin_mode;
    }

    public void setSpeedUp(bool is_speed_up)
    {
        m_IsSpeedUp = is_speed_up;
    }

    public int addSkill(GlobalDefine.PartyCharaIndex party_member_index, string skill_name, ESKILLTYPE skill_type, MasterDataDefineLabel.ElementType skill_element, int active_skill_index)
    {
        SkillInfo skill_info = new SkillInfo(party_member_index, skill_name, skill_type, skill_element, active_skill_index, m_SkillIndex);
        m_SkillInfos.Add(skill_info);

        int ret_val = m_SkillIndex;
        m_SkillIndex++;

        return ret_val;
    }

    public bool isUpdating()
    {
        return (m_CurrentSkillInfo != null
            || m_SkillInfos.Count > 0
            || m_CurrentCutinObjectInfo != null
            || m_DisappearCutinObjectInfo != null
            || m_SkillTime > 0.0f
            );
    }

    public int getCurrentSkillIndex()
    {
        if (m_CurrentCutinObjectInfo != null && m_CurrentCutinObjectInfo.getPhase() == CutinObjectInfo.Phase.APPEAR)
        {
            return -1;
        }

        if (m_CurrentSkillInfo != null)
        {
            return m_CurrentSkillInfo.m_SkillIndex;
        }

        return -1;
    }

    public GlobalDefine.PartyCharaIndex getCurrentSkillCaster()
    {
        if (m_CurrentSkillInfo != null)
        {
            return m_CurrentSkillInfo.m_Caster;
        }

        return GlobalDefine.PartyCharaIndex.ERROR;
    }

    public bool isWaitNextStep(bool is_exec_next_step)
    {
        bool ret_val = (m_UpdateMode == UpdateMode.STEP_WAIT);
        if (ret_val)
        {
            m_UpdateMode = UpdateMode.STEP;
        }
        return ret_val;
    }

    private void startSkillAnim()
    {
        m_SkillTime = CutinObjectInfo.ANIM_TIME_SKILL;
    }

    private void updateSkillAnim(float delta_time)
    {
        if (m_IsNoCutinMode == false)
        {
            if (m_SkillTime == CutinObjectInfo.ANIM_TIME_SKILL)
            {
                // 先頭のスキルを発動アニメーション１を再生（拡大）
                if (m_SkillObjects[1].IsActive())
                {
                    m_SkillObjects[1].transform.killTween();
                    m_SkillObjects[1].transform.tweenToParent(m_SkillLocateCurrent.transform, CutinObjectInfo.ANIM_TIME_SKILL * 0.25f / m_AnimSpeed);

                    SoundUtil.PlaySE(m_CurrentSkillInfo.m_SkillType == ESKILLTYPE.eLIMITBREAK
                        ? SEID.SE_BATLE_SKILL_LIMITBREAK_CUTIN
                        : SEID.SE_BATLE_SKILL_CUTIN);
                }

                if (BattleParam.m_PlayerParty != null)
                {
                    BattleParam.m_PlayerParty.setDispHpWait((CutinObjectInfo.ANIM_TIME_SKILL + CutinObjectInfo.ANIM_TIME_OUT * 0.1f) / m_AnimSpeed);
                }
            }

            if (m_SkillTime > 0.0f)
            {
                float old_time = m_SkillTime;
                m_SkillTime -= delta_time * m_AnimSpeed;

                if (old_time > CutinObjectInfo.ANIM_TIME_SKILL * 0.5f && m_SkillTime <= CutinObjectInfo.ANIM_TIME_SKILL * 0.5f)
                {
                    // 先頭のスキルを発動アニメーション２を再生（横へスクロールして画面外へ）
                    m_SkillObjects[0].transform.killTween();
                    m_SkillObjects[0].transform.localScale = Vector3.one;
                    m_SkillObjects[0].transform.localPosition = Vector3.zero;

                    setupSkillPanel(m_SkillObjects[0], m_CurrentSkillInfo, m_SkillLocateCurrent.transform);

                    m_SkillObjects[0].transform.tween(new Vector3(-1200.0f, 0.0f, 0.0f), Quaternion.identity, Vector3.one, CutinObjectInfo.ANIM_TIME_SKILL * 0.5f / m_AnimSpeed);
                    m_SkillObjects[0].SetActive(true);

                    if (m_ResurrectInfo == null)
                    {
                        if (m_SkillInfos.Count >= 1 && m_SkillInfos[0].m_Key == m_CurrentSkillInfo.m_Key)
                        {
                            m_SkillObjects[1].transform.killTween();
                            m_SkillObjects[1].transform.localScale = Vector3.one;
                            m_SkillObjects[1].transform.localPosition = Vector3.zero;

                            setupSkillPanel(m_SkillObjects[1], m_SkillInfos[0], m_SkillLocateNext2.transform);

                            m_SkillObjects[1].transform.tweenToParent(m_SkillLocateNext1.transform, CutinObjectInfo.ANIM_TIME_SKILL * 0.5f / m_AnimSpeed);
                            m_SkillObjects[1].SetActive(true);

                            if (m_SkillInfos.Count >= 2 && m_SkillInfos[1].m_Key == m_CurrentSkillInfo.m_Key)
                            {
                                m_SkillObjects[2].transform.killTween();
                                m_SkillObjects[2].transform.localScale = Vector3.one;
                                m_SkillObjects[2].transform.localPosition = Vector3.zero;

                                setupSkillPanel(m_SkillObjects[2], m_SkillInfos[1], m_SkillLocateNext3.transform);

                                m_SkillObjects[2].transform.tweenToParent(m_SkillLocateNext2.transform, CutinObjectInfo.ANIM_TIME_SKILL * 0.5f / m_AnimSpeed);
                                m_SkillObjects[2].SetActive(true);
                            }
                            else
                            {
                                m_SkillObjects[2].SetActive(false);
                            }
                        }
                        else
                        {
                            m_SkillObjects[1].SetActive(false);
                            m_SkillObjects[2].SetActive(false);
                        }
                    }
                    else
                    {
                        // 割り込みスキルは早めに出現したいのでここで開始
                        {
                            GameObject resurrect_obj = m_SkillObjects[1];

                            setupSkillPanel(resurrect_obj, m_ResurrectInfo, m_SkillLocateCurrent.transform);

                            resurrect_obj.transform.killTween();
                            resurrect_obj.transform.SetParent(m_SkillLocateCurrent.transform, false);
                            resurrect_obj.transform.localScale = new Vector3(1.2f, 0.0f, 1.0f);
                            resurrect_obj.transform.localPosition = Vector3.zero;
                            resurrect_obj.SetActive(true);
                            resurrect_obj.transform.tween(new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, Vector3.one, CutinObjectInfo.ANIM_TIME_SKILL * 0.5f / m_AnimSpeed);
                        }
                    }
                }

                if (m_SkillTime <= 0.0f)
                {
                    if (m_UpdateMode == UpdateMode.STEP)
                    {
                        m_UpdateMode = UpdateMode.STEP_WAIT;
                    }

                    m_SkillObjects[0].SetActive(false);

                    if (m_ResurrectInfo == null)
                    {
                        if (m_SkillInfos.Count <= 0)
                        {
                            m_SkillObjects[1].SetActive(false);
                            m_SkillObjects[2].SetActive(false);
                        }
                    }
                }
            }
        }
        else
        {
            if (m_SkillTime > 0.0f)
            {
                const float NO_CUTIN_MODE_ANIM_SPEED = 2.0f;  //カットインなしの時のアニメーション速度補正
                m_SkillTime -= delta_time * NO_CUTIN_MODE_ANIM_SPEED;
                if (m_SkillTime <= 0.0f)
                {
                    if (m_UpdateMode == UpdateMode.STEP)
                    {
                        m_UpdateMode = UpdateMode.STEP_WAIT;
                    }
                }
            }
        }
    }

    private void setupSkillPanel(GameObject dest_skill_object, SkillInfo skill_info, Transform start_transform)
    {
        TMP_Text text = dest_skill_object.transform.Find("Text").GetComponent<TMP_Text>();

        if (skill_info.m_ResurrectFlag <= 0)
        {
            // スキル表示
            text.text = skill_info.m_SkillName;
            text.alignment = TextAlignmentOptions.TopRight;

            dest_skill_object.transform.Find("Image").gameObject.SetActive(true);

            dest_skill_object.transform.Find("ImageSKILL1").gameObject.SetActive(skill_info.m_SkillType == ESKILLTYPE.eACTIVE && skill_info.m_ActiveSkillIndex == 0);
            dest_skill_object.transform.Find("ImageSKILL2").gameObject.SetActive(skill_info.m_SkillType == ESKILLTYPE.eACTIVE && skill_info.m_ActiveSkillIndex == 1);
            dest_skill_object.transform.Find("ImageLeader").gameObject.SetActive(skill_info.m_SkillType == ESKILLTYPE.eLEADER && skill_info.m_Caster == GlobalDefine.PartyCharaIndex.LEADER);
            dest_skill_object.transform.Find("ImageFriend").gameObject.SetActive(skill_info.m_SkillType == ESKILLTYPE.eLEADER && skill_info.m_Caster == GlobalDefine.PartyCharaIndex.FRIEND);
            dest_skill_object.transform.Find("ImagePassive").gameObject.SetActive(skill_info.m_SkillType == ESKILLTYPE.ePASSIVE);
            dest_skill_object.transform.Find("ImageLink").gameObject.SetActive(skill_info.m_SkillType == ESKILLTYPE.eLINK);
            dest_skill_object.transform.Find("ImageLinkPassive").gameObject.SetActive(skill_info.m_SkillType == ESKILLTYPE.eLINKPASSIVE);

            dest_skill_object.transform.Find("Unit0").gameObject.SetActive(false);
            dest_skill_object.transform.Find("Unit1").gameObject.SetActive(false);
            dest_skill_object.transform.Find("Unit2").gameObject.SetActive(false);
            dest_skill_object.transform.Find("Unit3").gameObject.SetActive(false);
            dest_skill_object.transform.Find("Unit4").gameObject.SetActive(false);
        }
        else
        {
            // 復活ユニット表示
            text.text = GameTextUtil.GetText("battle_resurrection_text");	//「復活！」
            text.alignment = TextAlignmentOptions.TopLeft;

            dest_skill_object.transform.Find("Image").gameObject.SetActive(false);

            dest_skill_object.transform.Find("ImageSKILL1").gameObject.SetActive(false);
            dest_skill_object.transform.Find("ImageSKILL2").gameObject.SetActive(false);
            dest_skill_object.transform.Find("ImageLeader").gameObject.SetActive(false);
            dest_skill_object.transform.Find("ImageFriend").gameObject.SetActive(false);
            dest_skill_object.transform.Find("ImagePassive").gameObject.SetActive(false);
            dest_skill_object.transform.Find("ImageLink").gameObject.SetActive(false);
            dest_skill_object.transform.Find("ImageLinkPassive").gameObject.SetActive(false);

            // キャラアイコンを設定
            float res_num = 0.0f;
            for (int member_idx = 0; member_idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); member_idx++)
            {
                GameObject unit_obj = dest_skill_object.transform.Find("Unit" + member_idx.ToString()).gameObject;
                if ((skill_info.m_ResurrectFlag & (1 << member_idx)) != 0)
                {
                    unit_obj.SetActive(true);
                    unit_obj.transform.localPosition = m_ResurrectUnitPos0 + (m_ResurrectUnitPos1 - m_ResurrectUnitPos0) * res_num;
                    res_num += 1.0f;
                }
                else
                {
                    unit_obj.SetActive(false);
                }
            }
        }

        dest_skill_object.transform.SetParent(start_transform, false);
        dest_skill_object.transform.localScale = Vector3.one;
        dest_skill_object.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 割り込みスキルを追加
    /// </summary>
    public void setResurrectInfo(int resurrect_info)
    {
        if (resurrect_info > 0)
        {
            m_ResurrectInfo = new SkillInfo(
                m_CurrentSkillInfo.m_Caster,
                "",
                m_CurrentSkillInfo.m_SkillType,
                m_CurrentSkillInfo.m_SkillElement,
                m_CurrentSkillInfo.m_ActiveSkillIndex,
                m_CurrentSkillInfo.m_SkillIndex);
            m_ResurrectInfo.m_ResurrectFlag = resurrect_info;
        }
        else
        {
            m_ResurrectInfo = null;
        }
    }


#if BUILD_TYPE_DEBUG
    /// <summary>
    /// デバッグ用スキルカットイン再生
    /// </summary>
    /// <param name="chara_fix_id"></param>
    /// <param name="img_2_tiling"></param>
    /// <param name="img_2_offsetX"></param>
    /// <param name="img_2_offsetY"></param>
    public void debugSkillCutin(int chara_fix_id, int img_2_tiling, int img_2_offsetX, int img_2_offsetY, int size_width, int size_height, MasterDataDefineLabel.PivotType pivot, int side_offset)
    {
        MasterDataParamChara base_chara_master = MasterFinder<MasterDataParamChara>.Instance.Find(chara_fix_id);
        if (base_chara_master != null)
        {
            // ダミーのキャラマスターを生成（面倒なのでスキルカットインで使うところだけ設定）
            MasterDataParamChara dummy_chara_master = new MasterDataParamChara();
            dummy_chara_master.fix_id = base_chara_master.fix_id;
            dummy_chara_master.element = base_chara_master.element;
            dummy_chara_master.skill_active0 = base_chara_master.skill_active0;
            dummy_chara_master.img_2_tiling = img_2_tiling;
            dummy_chara_master.img_2_offsetX = img_2_offsetX;
            dummy_chara_master.img_2_offsetY = img_2_offsetY;
            dummy_chara_master.size_width = size_width;
            dummy_chara_master.size_height = size_height;
            dummy_chara_master.pivot = pivot;
            dummy_chara_master.side_offset = side_offset;

            // ダミーのキャラマスターをバトルのキャッシュに登録
            BattleParam.m_MasterDataCache.clearCachePlayerAll();
            BattleParam.m_MasterDataCache.addCharaParam(dummy_chara_master);

            // ダミーのキャラデータを生成（バトルのキャッシュを使用して生成される）
            CharaOnce chara_once = new CharaOnce();
            bool is_success = chara_once.CharaSetupFromID((uint)chara_fix_id, 1);
            if (is_success)
            {
                string skill_name = "スキル名";
                MasterDataDefineLabel.ElementType element_type = base_chara_master.element;

                MasterDataSkillActive skill_master = MasterFinder<MasterDataSkillActive>.Instance.Find((int)chara_once.m_CharaMasterDataParam.skill_active0);
                if (skill_master != null)
                {
                    skill_name = skill_master.name;
                    element_type = skill_master.skill_element;
                }

                // ダミーのパーティを生成
                CharaOnce[] party_members = new CharaOnce[5];
                party_members[0] = chara_once;
                BattleParam.m_PlayerParty = new CharaParty();
                BattleParam.m_PlayerParty.PartySetup(party_members, true);

                // カットインを開始
                clearSkill();
                addSkill(GlobalDefine.PartyCharaIndex.LEADER, skill_name, ESKILLTYPE.eACTIVE, element_type, 0);
                startCutin(false, false, false);
            }
        }
    }
#endif
}
