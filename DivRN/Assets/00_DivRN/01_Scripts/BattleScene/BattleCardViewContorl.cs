using UnityEngine;

public class BattleCardViewContorl : MonoBehaviour
{
    private BattleScene.BattleCard m_BattleCard = null;

    public Sprite[] m_SpritesOn = new Sprite[(int)MasterDataDefineLabel.ElementType.MAX];
    public Sprite[] m_SpritesOff = new Sprite[(int)MasterDataDefineLabel.ElementType.MAX];

    public float m_AnimWait = 0.0f;

    private SpriteRenderer m_SpriteRenderer = null;

    private MasterDataDefineLabel.ElementType m_ElementType = MasterDataDefineLabel.ElementType.NONE;
    private bool m_IsOn = false;

    private MyTween m_MyTween = null;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_MyTween = new MyTween(transform);
    }

    // Use this for initialization
    void Start()
    {
    }

    private void OnEnable()
    {
        _updateSprite();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_BattleCard != null)
        {
            if (m_BattleCard.getPhase() == BattleScene.BattleCard.Phase.UNUSED)
            {
                m_ElementType = MasterDataDefineLabel.ElementType.NONE;
                m_IsOn = false;
            }

            if (m_BattleCard.getElementType() != m_ElementType
                || m_BattleCard.m_IsOn != m_IsOn
            )
            {
                _updateSprite();
            }

#if BUILD_TYPE_DEBUG
            if (m_BattleCard.getPhase() != BattleScene.BattleCard.Phase.UNUSED)
            {
                // 開発中はエラーであることをわかりやすくするための表示
                if (m_ElementType <= MasterDataDefineLabel.ElementType.NONE || m_ElementType >= MasterDataDefineLabel.ElementType.MAX)
                {
                    m_SpriteRenderer.enabled = true;
                    m_SpriteRenderer.sprite = m_SpritesOff[RandManager.GetRand(1, 7)];
                }
            }
#endif
        }

        // カード配りアニメーション
        if (m_AnimWait > 0.0f)
        {
            const float ANIM_TIME = 0.1f;
            m_AnimWait -= Time.deltaTime;
            if (m_AnimWait > ANIM_TIME)
            {
                m_SpriteRenderer.enabled = false;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));
            }
            else
            if (m_AnimWait > 0.0f)
            {
                m_SpriteRenderer.enabled = true;
                transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f * (1.0f - m_AnimWait / ANIM_TIME));
                transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 90.0f * (m_AnimWait / ANIM_TIME), 0.0f));
            }
            else
            {
                m_AnimWait = 0.0f;
                m_SpriteRenderer.enabled = true;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
            }
        }

        m_MyTween.update(Time.deltaTime);
    }

    public void init(BattleScene.BattleCard battle_card)
    {
        battle_card.m_ViewControl = this;
        m_BattleCard = battle_card;
    }

    public void setSnapToParent(Transform parent, float duration, float wait_time = 0.0f)
    {
        m_MyTween.tweenToParent(parent, duration, MyTween.EaseType.OUT_QUAD, wait_time);
    }

    private void _updateSprite()
    {
        if (m_BattleCard != null)
        {
            m_ElementType = m_BattleCard.getElementType();
            m_IsOn = m_BattleCard.m_IsOn;

            Sprite sprite = null;
            if (m_ElementType > MasterDataDefineLabel.ElementType.NONE && m_ElementType < MasterDataDefineLabel.ElementType.MAX)
            {
                if (m_IsOn)
                {
                    sprite = m_SpritesOn[(int)m_ElementType];
                }
                else
                {
                    sprite = m_SpritesOff[(int)m_ElementType];
                }
            }

            if (sprite != null)
            {
                m_SpriteRenderer.enabled = true;
                m_SpriteRenderer.sprite = sprite;
            }
            else
            {
                m_SpriteRenderer.enabled = false;
                m_SpriteRenderer.sprite = null;
            }
        }
        else
        {
            m_ElementType = MasterDataDefineLabel.ElementType.NONE;
            m_IsOn = false;
            m_SpriteRenderer.enabled = false;
            m_SpriteRenderer.sprite = null;
        }
    }
}
