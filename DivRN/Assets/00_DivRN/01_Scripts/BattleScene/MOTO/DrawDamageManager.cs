/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	DrawDamageManager.cs
    @brief	ダメージ表示管理クラス
    @author Developer
    @date 	2012/12/05

    数値のUI表示に特化
*/
/*==========================================================================*/
/*==========================================================================*/

/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using TMPro;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/

//----------------------------------------------------------------------------
/*!
    @brief	ダメージタイプ
*/
//----------------------------------------------------------------------------

/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
    @brief	ダメージ表示管理クラス
*/
//----------------------------------------------------------------------------
public class DrawDamageManager : SingletonComponent<DrawDamageManager>
{
    public Sprite m_CriticalSprite = null;
    public Sprite m_WeakSprite = null;
    public Sprite m_GuardSprite = null;

    public GameObject m_DamageNumberPrefab = null;

    private class DamageViewInfo
    {
        public GameObject m_InstanceObject;
        public Animation m_TextAnimation;
        public TextMeshPro m_TextMesh;
        public Animation m_SpriteAnimation;
        public SpriteRenderer m_WeakSprite;
        public float m_ShowTimer;
    }

    private const int DAMAGE_VIEW_COUNT_MAX = 16;

    private DamageViewInfo[] m_DamageViewInfots = new DamageViewInfo[DAMAGE_VIEW_COUNT_MAX];
    private int m_CurrentIndex = 0;

    private Vector3 m_InitPrefabPosition;
    private Quaternion m_InitPrefabRotation;
    private Vector3 m_InitPrefabScale;

    private const string TEXT_ANIM_NAME_DAMAGE = "BattleDamageValue";
    private const string TEXT_ANIM_NAME_DAMAGE_GUARD = "BattleDamageValueGuard";

    private const string SPRITE_ANIM_NAME = "BattleDamageIconAnim";
    private const string SPRITE_ANIM_NAME_CRITICAL = "BattleDamageIconCriticalAnim";

    private bool m_IsShowing = false;

    private static Color[] MOJI_COLORS = new Color[(int)EDAMAGE_TYPE.eDAMAGE_TYPE_MAX]
    {
        Color.white,
        Color.white,
        Color.white,
        new Color32(248, 141, 227, 255),
        Color.white
    };

    private static Color[] MOJI_SHADOW_COLORS = new Color[(int)EDAMAGE_TYPE.eDAMAGE_TYPE_MAX]
    {
        Color.red,
        Color.red,
        Color.blue,
        Color.black,
        Color.red
    };

    private static float[] MOJI_SCALES = new float[(int)EDAMAGE_TYPE.eDAMAGE_TYPE_MAX]
    {
        0.8f,
        0.8f,
        0.8f,
        0.7f,
        1.2f
    };

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：初期化処理	※インスタンス生成時呼出し
    */
    //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
    */
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        m_InitPrefabPosition = m_DamageNumberPrefab.transform.localPosition;
        m_InitPrefabRotation = m_DamageNumberPrefab.transform.localRotation;
        m_InitPrefabScale = m_DamageNumberPrefab.transform.localScale;

        for (int idx = 0; idx < m_DamageViewInfots.Length; idx++)
        {
            DamageViewInfo damage_view_info = new DamageViewInfo();
            setupDamageViewInfo(damage_view_info);
            m_DamageViewInfots[idx] = damage_view_info;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	Unity固有処理：更新処理	※定期処理
    */
    //----------------------------------------------------------------------------
    void Update()
    {
        m_IsShowing = false;
        for (int idx = 0; idx < m_DamageViewInfots.Length; idx++)
        {
            DamageViewInfo damage_view_info = m_DamageViewInfots[idx];

            if (damage_view_info != null
                && damage_view_info.m_ShowTimer > 0.0f)
            {
                GameObject instance_object = damage_view_info.m_InstanceObject;
                if (instance_object != null)
                {
                    damage_view_info.m_ShowTimer -= Time.deltaTime;
                    if (damage_view_info.m_ShowTimer <= 0.0f)
                    {
                        instance_object.SetActive(false);
                        instance_object.transform.SetParent(transform, false);
                        instance_object.transform.localPosition = m_InitPrefabPosition;
                        instance_object.transform.localRotation = m_InitPrefabRotation;
                        instance_object.transform.localScale = m_InitPrefabScale;
                    }
                    else
                    {
                        m_IsShowing = true;
                    }
                }
                else
                {
                    damage_view_info.m_ShowTimer = 0.0f;
                }
            }
        }
    }

    public static bool isShowing()
    {
        if (Instance != null)
        {
            return Instance.m_IsShowing;
        }

        return false;
    }

    public static void showDamage(Transform parent, int damage_value, EDAMAGE_TYPE damage_type, float scale = 1.0f, float offset_y = 0.0f, float type_scale = 1.0f)
    {
        if (Instance != null)
        {
            Instance._showDamage(parent, damage_value, damage_type, scale, offset_y, type_scale);
        }
    }

    private void _showDamage(Transform parent, int damage_value, EDAMAGE_TYPE damage_type, float scale, float offset_y, float type_scale)
    {
        DamageViewInfo damage_view_info = m_DamageViewInfots[m_CurrentIndex];

        if (damage_view_info.m_InstanceObject == null)
        {
            // 表示中にライド先の削除に巻き込まれて消える場合があるので.その場合インスタンスを作り直し.
            setupDamageViewInfo(damage_view_info);
        }

        damage_view_info.m_ShowTimer = 1.0f;

        damage_view_info.m_InstanceObject.transform.SetParent(parent, false);

        if (damage_view_info.m_TextMesh != null)
        {
            string anim_name = null;
            Sprite weak_sprite = null;

            Color moji_color = MOJI_COLORS[(int)damage_type];
            Color moji_shadow_color = MOJI_SHADOW_COLORS[(int)damage_type];
            float moji_scale = MOJI_SCALES[(int)damage_type];

            switch (damage_type)
            {
                case EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL:
                default:
                    {
                        anim_name = TEXT_ANIM_NAME_DAMAGE;
                        weak_sprite = null;
                    }
                    break;

                case EDAMAGE_TYPE.eDAMAGE_TYPE_WEEK:
                    {
                        anim_name = TEXT_ANIM_NAME_DAMAGE;
                        weak_sprite = m_WeakSprite;
                    }
                    break;

                case EDAMAGE_TYPE.eDAMAGE_TYPE_GUARD:
                    {
                        anim_name = TEXT_ANIM_NAME_DAMAGE_GUARD;
                        weak_sprite = m_GuardSprite;
                    }
                    break;

                case EDAMAGE_TYPE.eDAMAGE_TYPE_HEAL:
                    {
                        anim_name = TEXT_ANIM_NAME_DAMAGE_GUARD;
                        weak_sprite = null;
                    }
                    break;

                case EDAMAGE_TYPE.eDAMAGE_TYPE_CRITICAL:
                    {
                        anim_name = TEXT_ANIM_NAME_DAMAGE;
                        weak_sprite = m_CriticalSprite;
                    }
                    break;
            }

            damage_view_info.m_InstanceObject.transform.localPosition = new Vector3(0.0f, offset_y, 0.0f);

            damage_view_info.m_TextMesh.text = damage_value.ToString();
            damage_view_info.m_TextMesh.color = moji_color;

            if (damage_view_info.m_TextMesh != null)
            {
                damage_view_info.m_TextMesh.fontMaterial.SetColor("_UnderlayColor", moji_shadow_color);
            }

            damage_view_info.m_TextMesh.gameObject.GetComponent<RectTransform>().localScale = Vector3.one * (moji_scale * 0.05f * scale);

            const float WEAK_SPRITE_BASE_SCALE = 0.5f;
            damage_view_info.m_WeakSprite.transform.localScale = new Vector3(type_scale * WEAK_SPRITE_BASE_SCALE, type_scale * WEAK_SPRITE_BASE_SCALE, type_scale * WEAK_SPRITE_BASE_SCALE);
            damage_view_info.m_WeakSprite.sprite = weak_sprite;

            damage_view_info.m_InstanceObject.SetActive(true);

            damage_view_info.m_TextAnimation.Stop();
            damage_view_info.m_TextAnimation.Play(anim_name);

            if (weak_sprite != null)
            {
                damage_view_info.m_SpriteAnimation.Stop();
                if (weak_sprite == m_CriticalSprite)
                {
                    damage_view_info.m_SpriteAnimation.Play(SPRITE_ANIM_NAME_CRITICAL);
                }
                else
                {
                    damage_view_info.m_SpriteAnimation.Play(SPRITE_ANIM_NAME);
                }
            }
        }

        m_CurrentIndex = (m_CurrentIndex + 1) % m_DamageViewInfots.Length;

        m_IsShowing = true;
    }

    private void setupDamageViewInfo(DamageViewInfo damage_view_info)
    {
        damage_view_info.m_InstanceObject = Instantiate(m_DamageNumberPrefab);

        Transform text_anim_trans = damage_view_info.m_InstanceObject.transform.Find("TextAnim");
        if (text_anim_trans != null)
        {
            damage_view_info.m_TextAnimation = text_anim_trans.GetComponent<Animation>();
        }

        Transform text_trans = damage_view_info.m_InstanceObject.transform.Find("TextAnim/TextObject");
        if (text_trans != null)
        {
            damage_view_info.m_TextMesh = text_trans.GetComponent<TextMeshPro>();
        }

        Transform sprite_anim_trans = damage_view_info.m_InstanceObject.transform.Find("SpriteAnim");
        if (sprite_anim_trans != null)
        {
            damage_view_info.m_SpriteAnimation = sprite_anim_trans.GetComponent<Animation>();
        }

        Transform weak_sprite_trans = damage_view_info.m_InstanceObject.transform.Find("SpriteAnim/WeakSprite");
        if (weak_sprite_trans != null)
        {
            damage_view_info.m_WeakSprite = weak_sprite_trans.GetComponent<SpriteRenderer>();
        }

        damage_view_info.m_InstanceObject.transform.SetParent(transform, false);
        damage_view_info.m_InstanceObject.SetActive(false);
    }
}

