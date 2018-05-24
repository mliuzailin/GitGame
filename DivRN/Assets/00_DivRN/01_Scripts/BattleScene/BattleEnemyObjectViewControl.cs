using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BattleEnemyObjectViewControl : MonoBehaviour
{
    public int m_EnemyCharaID = 0;
    public Color m_ShadowColor = Color.white;
    public int m_Hp = 100;
    public int m_HpMax = 100;
    public int m_AttackWaitTurn = 0;
    public int m_TextMeshAttackWaitTurn = -1;
    public bool m_TurnChangeEffect = false;

    public GameObject m_CenterObject = null;
    public GameObject m_ImageObjectAnim = null;
    public GameObject m_ImageObject = null;
    public GameObject m_ImageObjectShadow = null;
    public GameObject m_HitPointLocate = null;
    public GameObject m_HpGaugeScaleObject = null;
    public GameObject m_HpLeftObject = null;
    public GameObject m_HpRightObject = null;
    public GameObject m_HitPointObject = null;
    public GameObject m_AttackWaitObject = null;
    public GameObject m_TargetCursorObject = null;
    public GameObject m_DamageNumberLocale = null;
    public GameObject m_AilmentObject = null;

    private bool m_IsBlink = false;
    private float m_BlinkTimer = 0.0f;

    private float m_DamageDispTimer = 0.0f;

    private MasterDataDefineLabel.HPGaugeType m_HPGaugeType = MasterDataDefineLabel.HPGaugeType.NONE;
    private Vector3 m_HpGaugeOffset = Vector3.zero;
    private Vector3 m_TargetCursorOffset = Vector3.zero;

    private BattleCharaImageViewControl m_BattleCharaImageViewControl = null;
    private SpriteRenderer m_SpriteRenderer_ImageObject = null;
    private SpriteRenderer m_SpriteRenderer_ImageObjectShadow = null;

    private Animation m_Animation = null;

    private void Awake()
    {
        m_BattleCharaImageViewControl = m_ImageObject.GetComponent<BattleCharaImageViewControl>();
        m_SpriteRenderer_ImageObject = m_ImageObject.GetComponent<SpriteRenderer>();
        m_SpriteRenderer_ImageObjectShadow = m_ImageObjectShadow.GetComponent<SpriteRenderer>();

        if (m_ImageObjectAnim != null)
        {
            m_Animation = m_ImageObjectAnim.GetComponent<Animation>();
        }

        if (m_HitPointLocate != null)
        {
            m_HpGaugeOffset = m_HitPointLocate.transform.localPosition;
        }

        if (m_TargetCursorObject != null)
        {
            m_TargetCursorOffset = m_TargetCursorObject.transform.localPosition;
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        m_BattleCharaImageViewControl.setCharaID(m_EnemyCharaID, BattleCharaImageViewControl.ImageType.ENEMY_DISP);

        if (m_BattleCharaImageViewControl.isChange())
        {
            // ターゲットカーソル位置を調整（敵キャラの表示サイズ・位置に合わせる）
            if (m_CenterObject != null)
            {
                m_CenterObject.transform.localPosition = new Vector3(0.0f, m_BattleCharaImageViewControl.getCharaWaistHeight(), 0.0f);
            }
        }

        if (m_ImageObject != null)
        {
            Color color = Color.white;

            // HPゼロの時は暗く表示
            if (m_Hp <= 0)
            {
                color.r *= 0.5f;
                color.g *= 0.5f;
                color.b *= 0.5f;
            }

            m_BattleCharaImageViewControl.setColor(color);
        }

        if (m_SpriteRenderer_ImageObjectShadow != null)
        {
            m_BlinkTimer += Time.deltaTime;
            if (m_BlinkTimer > 1.0f)
            {
                m_BlinkTimer -= 1.0f;
            }

            Color color = m_ShadowColor;

            // HPゼロの時は暗く表示
            if (m_Hp <= 0)
            {
                color.r *= 0.5f;
                color.g *= 0.5f;
                color.b *= 0.5f;
            }

            // このターンで攻撃してくるときは点滅
            if (m_IsBlink
                && m_AttackWaitTurn <= 1
            )
            {
                float b;
                if (m_BlinkTimer < 0.5f)
                {
                    b = m_BlinkTimer * 2.0f;
                }
                else
                {
                    b = (1.0f - m_BlinkTimer) * 2.0f;
                }
                b = b * 0.5f + 0.5f;
                color.r *= b;
                color.g *= b;
                color.b *= b;
            }

            if (m_BattleCharaImageViewControl.isChange())
            {
                m_SpriteRenderer_ImageObjectShadow.sprite = m_SpriteRenderer_ImageObject.sprite;
                m_SpriteRenderer_ImageObjectShadow.transform.localPosition = m_SpriteRenderer_ImageObject.transform.localPosition;
                m_SpriteRenderer_ImageObjectShadow.transform.localScale = m_SpriteRenderer_ImageObject.transform.localScale;
            }
            if (m_SpriteRenderer_ImageObjectShadow.color.Equals(color) == false)
            {
                m_SpriteRenderer_ImageObjectShadow.color = color;
            }
        }

        // HPゲージの制御
        {
            {
                // ＨＰゲージ位置を調整（敵キャラの表示サイズ・位置に合わせる）
                if (m_HitPointLocate != null)
                {
                    m_HitPointLocate.transform.localPosition = new Vector3(0.0f, m_BattleCharaImageViewControl.getEnemyDispOffset().y * -0.75f, 0.0f)
                        + m_HpGaugeOffset;
                }
            }

            // HPゲージの大きさを選択
            const float HP_GAUGE_SMALL = 2.2f;
            const float HP_GAUGE_MIDDLE = 3.5f;
            const float HP_GAUGE_LEARGE = 6.3f;
            const float HP_GAUGE_BOSS = 7.0f;
            float hp_gauge_scale = HP_GAUGE_SMALL;
            switch (m_HPGaugeType)
            {
                case MasterDataDefineLabel.HPGaugeType.S:
                    hp_gauge_scale = HP_GAUGE_SMALL;
                    break;

                case MasterDataDefineLabel.HPGaugeType.M:
                    hp_gauge_scale = HP_GAUGE_MIDDLE;
                    break;

                case MasterDataDefineLabel.HPGaugeType.L:
                    hp_gauge_scale = HP_GAUGE_LEARGE;
                    break;

                case MasterDataDefineLabel.HPGaugeType.BOSS:
                    hp_gauge_scale = HP_GAUGE_BOSS;
                    break;

                default:
                    {
                        float mesh_width = m_BattleCharaImageViewControl.getMeshSize().x;
                        if (mesh_width >= 270.0f / 128.0f)
                        {
                            hp_gauge_scale = HP_GAUGE_LEARGE;
                        }
                        else if (mesh_width >= 170.0f / 128.0f)
                        {
                            hp_gauge_scale = HP_GAUGE_MIDDLE;
                        }
                    }
                    break;
            }

            // HPゲージに左側に表示されるものの位置調整
            if (m_HpLeftObject != null)
            {
                m_HpLeftObject.transform.localPosition = new Vector3(-hp_gauge_scale, 0.0f, 0.0f);
            }

            // HPゲージに右側に表示されるものの位置調整
            if (m_HpRightObject != null)
            {
                m_HpRightObject.transform.localPosition = new Vector3(hp_gauge_scale, 0.0f, 0.0f);
            }

            // HPゲージのバーのスケール
            if (m_HpGaugeScaleObject != null)
            {
                m_HpGaugeScaleObject.transform.localScale = new Vector3(hp_gauge_scale, 1.0f, 1.0f);
            }

            // HPゲージの残りＨＰを更新
            if (m_HitPointObject != null)
            {
                float rate = m_Hp / (float)m_HpMax;
                m_HitPointObject.transform.localScale = new Vector3(rate, 1.0f, 1.0f);
            }

            // ターン更新エフェクト
            if (m_TurnChangeEffect)
            {
                m_TurnChangeEffect = false;
                EffectManager.Instance.playEffect2(SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DTurnChange, new Vector3(0.63f, 3.5f, -1.0f), Vector3.zero, m_HpRightObject, null, 20.0f);
            }
        }

        if (m_TextMeshAttackWaitTurn != m_AttackWaitTurn)
        {
            m_TextMeshAttackWaitTurn = m_AttackWaitTurn;

            if (m_AttackWaitObject != null)
            {
                TextMeshPro text_mesh = m_AttackWaitObject.GetComponent<TextMeshPro>();
                if (text_mesh != null)
                {
                    if (m_TextMeshAttackWaitTurn < 99)
                    {
                        text_mesh.text = m_TextMeshAttackWaitTurn.ToString();
                    }
                    else
                    {
                        text_mesh.text = "99";
                    }

                    if (m_TextMeshAttackWaitTurn > 1)
                    {
                        text_mesh.color = Color.white;
                    }
                    else
                    {
                        text_mesh.color = Color.yellow;
                    }
                }
            }
        }

        if (m_TargetCursorObject != null)
        {
            m_TargetCursorObject.transform.localPosition = m_TargetCursorOffset;
        }

        if (m_DamageDispTimer > 0.0f)
        {
            m_DamageDispTimer -= Time.deltaTime;
            if (m_DamageDispTimer <= 0.0f)
            {
                m_DamageNumberLocale.SetActive(false);
            }
        }
    }

    public bool isLoadingTexture()
    {
        BattleCharaImageViewControl chara_image_control = m_ImageObject.GetComponent<BattleCharaImageViewControl>();
        if (chara_image_control.getState() == BattleCharaImageViewControl.ImageState.READY_IMAGE
            || chara_image_control.getState() == BattleCharaImageViewControl.ImageState.ERROR_IMAGE
            )
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// テクスチャ内で実際にキャラの絵が描かれている部分の幅
    /// </summary>
    /// <returns></returns>
    public float getCharaWidth()
    {
        return m_BattleCharaImageViewControl.getCharaWidth();
    }

    public float getCharaTall()
    {
        return m_BattleCharaImageViewControl.getCharaTall();
    }

    /// <summary>
    /// 敵の表示位置のオフセット
    /// </summary>
    /// <returns></returns>
    public Vector3 getEnemyDispOffset()
    {
        return m_BattleCharaImageViewControl.getEnemyDispOffset();
    }

    /// <summary>
    /// 敵にダメージ数値を表示
    /// </summary>
    /// <param name="damage_value"></param>
    /// <param name="damage_type"></param>
    public void setDmageDisp(int damage_value, EDAMAGE_TYPE damage_type)
    {
        if (m_DamageNumberLocale != null)
        {
            DrawDamageManager.showDamage(m_DamageNumberLocale.transform, damage_value, damage_type);
        }
    }

    /// <summary>
    /// 敵に状態異常表示を設定
    /// </summary>
    /// <param name="ailment_chara"></param>
    public void setAilment(StatusAilmentChara ailment_chara)
    {
        m_AilmentObject.GetComponent<InGameAilmentIcon>().SetStatus(ailment_chara);
    }

    /// <summary>
    /// ＨＰゲージの種類を設定
    /// </summary>
    /// <param name="hp_gauge_type"></param>
    public void setHpGaugeType(MasterDataDefineLabel.HPGaugeType hp_gauge_type)
    {
        m_HPGaugeType = hp_gauge_type;
    }

    public void setHpGaugeOffest(Vector2 offset)
    {
        m_HpGaugeOffset.x = offset.x;
        m_HpGaugeOffset.y = offset.y;
    }

    public void setTargetCursorOffset(Vector2 offset)
    {
        m_TargetCursorOffset.x = offset.x;
        m_TargetCursorOffset.y = offset.y;
    }

    public void playAnim(string anim_name)
    {
        if (m_Animation != null)
        {
            m_Animation.Stop();
            m_Animation.Play(anim_name);
        }
    }

    public bool isPlayingAnim()
    {
        bool ret_val = false;
        if (m_Animation != null)
        {
            ret_val = m_Animation.isPlaying;
        }

        return ret_val;
    }
}
