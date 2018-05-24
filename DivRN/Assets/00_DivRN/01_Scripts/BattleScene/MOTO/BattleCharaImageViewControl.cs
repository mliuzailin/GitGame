using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharaImageViewControl : MonoBehaviour
{
    public enum ImageType
    {
        ENEMY_DISP, // 敵表示用
        FACE,   // （※使用不可）顔部分のみ
        CUTIN_DISP, // 新カットインで表示（顔が中心に来るように補正）
        CUTIN_DISP2,    // CUTIN_DISPと同じ動作（ゲームオブジェクトからこの値が設定されている可能性があるので残しています。）
    }

    public int m_CharaIdRequest = 0;
    public ImageType m_ImageType = ImageType.ENEMY_DISP;

    private int m_CharaIdCurrent = 0;
    private const float m_MeterPerDot = 128.0f;

    private ImageState m_State = ImageState.NO_IMAGE;


    private Sprite m_MainSprite = null;
    private Texture2D m_TextureMask = null;
    private Vector2 m_TextureSize = Vector2.zero;

    private Vector2 m_EnemyDispMeshSize;    //キャラの表示サイズ
    private MasterDataDefineLabel.PivotType m_PivotType;    //表示原点
    private float m_CharaWidth;         //キャラの幅（キャラの横位置調整用）
    private float m_CharaTall;          //キャラの身長（画像の高さ）
    private float m_CharaWaistHeight;   //キャラの腰の高さの（ターゲットカーソルの表示位置）
    private Vector3 m_EnemyDispOffset;

    private Vector2 m_FaceCenterUv; //顔の中心位置
    private float m_FaceUvSize; //顔の半径

    private SpriteRenderer m_SpriteRenderer = null;
    private MeshFilter m_MeshFilter = null;
    private MeshRenderer m_MeshRenderer = null;
    private Image m_ImageComponent = null;
    private enum CompnentType
    {
        NONE,
        SPRITE_RENDERER,
        MESH_RENDERER,
        UI_IMAGE,
    };
    private CompnentType m_CompnentType = CompnentType.NONE;

    private Mesh m_Mesh = null;

    private Color m_Color = Color.white;

    private bool m_IsChanged = false;
    private bool m_IsChangeTexture = false;
    private bool m_IsChangeColor = false;

    private Material m_OriginalMaterial = null;
    private Material m_AlphaMaskMaterial = null;

    public enum ImageState
    {
        NO_IMAGE,
        LOADING,
        READY_IMAGE,
        ERROR_IMAGE,
    }

    private float m_CharaScaleAdjust = 1.0f;    // キャラサイズ補正

    private Vector3 m_SpriteOffsetPosition = Vector3.zero;  // スプライト表示位置補正
    private float m_SpriteScale = 1.0f; // スプライト表示サイズ補正値
    private Vector3 m_BaseLocalPostion; // 基準位置
    private float m_BaseLocalScale; // 基準スケール

    // Use this for initialization
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        if (m_SpriteRenderer != null)
        {
            m_SpriteRenderer.enabled = false;
            m_CompnentType = CompnentType.SPRITE_RENDERER;

            m_OriginalMaterial = m_SpriteRenderer.material;
        }

        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        if (m_MeshRenderer != null)
        {
            m_MeshRenderer.enabled = false;
            m_CompnentType = CompnentType.MESH_RENDERER;

            m_OriginalMaterial = m_MeshRenderer.material;
        }

        m_ImageComponent = GetComponent<Image>();
        if (m_ImageComponent != null)
        {
            m_ImageComponent.enabled = false;
            m_CompnentType = CompnentType.UI_IMAGE;

            m_OriginalMaterial = m_ImageComponent.material;
        }

        Material material_prefab = Resources.Load<Material>("Material/AlphaMaskMaterial");
        m_AlphaMaskMaterial = new Material(material_prefab);

        m_BaseLocalPostion = transform.localPosition;
        m_BaseLocalScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CharaIdCurrent != m_CharaIdRequest)
        {
            if (m_CharaIdRequest != 0)
            {
                m_CharaIdCurrent = m_CharaIdRequest;

                bool is_load = false;

                if (m_CharaIdCurrent > 0)
                {
                    // 通常ユニット
                    MasterDataParamChara master_chara = BattleParam.m_MasterDataCache.useCharaParam((uint)m_CharaIdCurrent);
                    if (master_chara != null)
                    {
                        float uv_off_x = master_chara.img_2_offsetX * 0.001f;
                        float uv_off_y = master_chara.img_2_offsetY * 0.001f;
                        float uv_size = master_chara.img_2_tiling * 0.001f;
                        uv_size *= 0.5f;
                        m_FaceCenterUv.x = uv_off_x + uv_size;
                        m_FaceCenterUv.y = uv_off_y + uv_size;
                        m_FaceUvSize = uv_size * 0.5f * 1.2f;

                        m_CharaScaleAdjust = 1.0f;
                        if (m_ImageType == ImageType.CUTIN_DISP || m_ImageType == ImageType.CUTIN_DISP2)
                        {
                            // スキルカットインの時はキャラ表示サイズの大小の差を緩和（小さいものも大きめに表示）
                            m_CharaScaleAdjust = 1.0f + (640.0f - master_chara.size_width) * (0.5f / 640.0f);
                        }

                        m_EnemyDispMeshSize.x = master_chara.size_width * m_CharaScaleAdjust / m_MeterPerDot;
                        m_EnemyDispMeshSize.y = master_chara.size_height * m_CharaScaleAdjust / m_MeterPerDot;
                        if (m_ImageType != ImageType.FACE)
                        {
                            m_PivotType = master_chara.pivot;
                        }
                        else
                        {
                            m_PivotType = MasterDataDefineLabel.PivotType.NONE;
                        }
                        m_CharaWidth = master_chara.side_offset * 2.0f / m_MeterPerDot;
                        m_CharaTall = master_chara.size_height / m_MeterPerDot; // 表示基準位置から上方向の高さ
                        if (m_PivotType == MasterDataDefineLabel.PivotType.CENTER)
                        {
                            if (m_CharaWidth >= 4.9f)
                            {
                                m_CharaTall *= 0.8f;
                            }
                            m_CharaTall *= 0.5f;
                        }

                        is_load = true;
                    }
                    else
                    {
                        m_MainSprite = null;
                        m_TextureMask = null;
                        m_TextureSize = Vector2.zero;
                        m_Mesh = null;
                        m_State = ImageState.ERROR_IMAGE;
                        m_IsChangeTexture = true;
                    }
                }
                else
                {
                    // 主人公
                    m_FaceCenterUv.x = 0.5f;
                    m_FaceCenterUv.y = 0.5f;
                    m_FaceUvSize = 0.1f;

                    m_EnemyDispMeshSize.x = 5.0f * 1.6f;
                    m_EnemyDispMeshSize.y = 5.0f * 1.6f;
                    m_PivotType = MasterDataDefineLabel.PivotType.NONE;
                    m_CharaWidth = m_EnemyDispMeshSize.x;

                    is_load = true;
                }

                if (is_load)
                {
                    // テクスチャロード
                    m_State = ImageState.LOADING;
                    if (BattleUnitTextureCache.HasInstance)
                    {
                        BattleUnitTextureCache.Instance.loadTexture(m_CharaIdCurrent, false);
                    }
                    else
                    {
                        AssetBundler.Create().
                            SetAsUnitTexture((uint)m_CharaIdCurrent,
                                (o) =>
                                {
                                    Texture2D texture = o.GetTexture2D(TextureWrapMode.Clamp);
                                    m_MainSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.one * 0.5f, 128, 0, SpriteMeshType.FullRect);
                                    m_TextureMask = null;
                                    if (m_MainSprite != null)
                                    {
                                        m_State = ImageState.READY_IMAGE;
                                        m_IsChangeTexture = true;

                                        calcSpriteAdjustment();
                                    }
                                    else
                                    {
                                        m_State = ImageState.ERROR_IMAGE;
                                        m_IsChangeTexture = true;
                                    }
                                },
                                (s) =>
                                {
                                    m_State = ImageState.ERROR_IMAGE;
                                }
                            ).Load();
                    }
                }
                else
                {
                    m_State = ImageState.ERROR_IMAGE;
                    m_IsChangeTexture = true;
                }
            }
            else
            {
                clearCharaID();
            }
        }

        if (m_State == ImageState.LOADING
            && BattleUnitTextureCache.HasInstance
        )
        {
            BattleUnitTextureCache.Status tex_status = BattleUnitTextureCache.Instance.getStatus(m_CharaIdCurrent);
            if (tex_status == BattleUnitTextureCache.Status.READY)
            {
                m_MainSprite = BattleUnitTextureCache.Instance.getSprite(m_CharaIdCurrent);
                m_TextureMask = BattleUnitTextureCache.Instance.getTexture(m_CharaIdCurrent, true);
                m_State = ImageState.READY_IMAGE;
                m_IsChangeTexture = true;

                calcSpriteAdjustment();
            }
            else
            if (tex_status != BattleUnitTextureCache.Status.LOADING)
            {
                m_MainSprite = null;
                m_TextureMask = null;
                m_State = ImageState.ERROR_IMAGE;
                m_IsChangeTexture = true;
            }
        }

        if (m_IsChangeTexture)
        {
            switch (m_CompnentType)
            {
                case CompnentType.SPRITE_RENDERER:
                    {
                        m_SpriteRenderer.sprite = m_MainSprite;

                        if (m_MainSprite != null)
                        {
                            if (m_TextureMask == null)
                            {
                                m_SpriteRenderer.material = m_OriginalMaterial;
                            }
                            else
                            {
                                m_SpriteRenderer.material = m_AlphaMaskMaterial;
                                m_SpriteRenderer.material.SetTexture("_AlphaTex", m_TextureMask);
                            }
                            transform.localPosition = m_SpriteOffsetPosition;
                            transform.localScale = new Vector3(m_SpriteScale, m_SpriteScale, m_SpriteScale);

                            m_SpriteRenderer.enabled = true;
                        }
                        else
                        {
                            m_SpriteRenderer.enabled = false;
                        }
                    }
                    break;

                case CompnentType.MESH_RENDERER:
                    break;

                case CompnentType.UI_IMAGE:
                    {
                        m_ImageComponent.sprite = m_MainSprite;
                        if (m_MainSprite != null)
                        {
                            m_ImageComponent.SetNativeSize();

                            if (m_TextureMask == null)
                            {
                                m_ImageComponent.material = m_OriginalMaterial;
                            }
                            else
                            {
                                m_ImageComponent.material = m_AlphaMaskMaterial;
                                m_ImageComponent.material.SetTexture("_AlphaTex", m_TextureMask);
                            }
                            transform.localPosition = m_SpriteOffsetPosition;
                            transform.localScale = new Vector3(m_SpriteScale, m_SpriteScale, m_SpriteScale);

                            m_ImageComponent.enabled = true;
                        }
                        else
                        {
                            m_ImageComponent.enabled = false;
                        }
                    }
                    break;
            }
        }

        if (m_IsChangeColor || m_IsChangeTexture)
        {
            switch (m_CompnentType)
            {
                case CompnentType.SPRITE_RENDERER:
                    {
                        m_IsChangeColor = false;
                        m_SpriteRenderer.color = m_Color;
                    }
                    break;

                case CompnentType.MESH_RENDERER:
                    {
                        m_IsChangeColor = false;
                        m_MeshRenderer.material.color = m_Color;
                    }
                    break;

                case CompnentType.UI_IMAGE:
                    {
                        m_IsChangeColor = false;
                        m_ImageComponent.color = m_Color;
                    }
                    break;
            }
        }

        m_IsChanged = m_IsChangeTexture;
        m_IsChangeTexture = false;
    }

    public ImageState getState()
    {
        return m_State;
    }

    public void clearCharaID()
    {
        m_CharaIdCurrent = 0;
        m_CharaIdRequest = 0;
        m_State = ImageState.NO_IMAGE;
        m_IsChangeTexture = true;
        m_MainSprite = null;
        m_TextureMask = null;
        m_TextureSize = Vector2.zero;
        m_Mesh = null;
    }

    /// <summary>
    /// キャラＩＤを設定
    /// </summary>
    /// <param name="chara_id">キャラFixID  マイナスの値を指定した場合は主人公FixIDを意味する</param>
    /// <param name="image_type"></param>
    public void setCharaID(int chara_id, ImageType image_type)
    {
        m_CharaIdRequest = chara_id;
        m_ImageType = image_type;

        if (m_CharaIdRequest == 0)
        {
            clearCharaID();
        }
    }

    public bool isChange()
    {
        return m_IsChanged;
    }

    public Vector2 getTextureSize()
    {
        return m_TextureSize;
    }

    public Vector2 getMeshSize()
    {
        return m_EnemyDispMeshSize;
    }

    /// <summary>
    /// キャラの横幅（横位置調整用）
    /// </summary>
    /// <returns></returns>
    public float getCharaWidth()
    {
        return m_CharaWidth;
    }

    /// <summary>
    /// キャラの身長(画像の高さ)
    /// </summary>
    /// <returns></returns>
    public float getCharaTall()
    {
        return m_CharaTall;
    }

    /// <summary>
    /// キャラの腰の高さ（ターゲットカーソル表示位置）
    /// </summary>
    /// <returns></returns>
    public float getCharaWaistHeight()
    {
        return m_CharaWaistHeight;
    }

    public Vector3 getEnemyDispOffset()
    {
        return m_EnemyDispOffset;
    }

    /// <summary>
    /// マスクテクスチャを取得
    /// </summary>
    /// <returns></returns>
    public Texture2D getMaskTexture()
    {
        return m_TextureMask;
    }

    public void setColor(Color color)
    {
        if (m_Color != color)
        {
            m_Color = color;
            m_IsChangeColor = true;
        }
    }

    /// <summary>
    /// スプライトの補正値を計算
    /// </summary>
    private void calcSpriteAdjustment()
    {
        m_Mesh = null;

        if (m_MainSprite == null)
        {
            return;
        }

        m_TextureSize = new Vector2(m_MainSprite.rect.width, m_MainSprite.rect.height);

        m_EnemyDispOffset = Vector3.zero;
        m_CharaWaistHeight = 0.0f;
        switch (m_PivotType)
        {
            case MasterDataDefineLabel.PivotType.CENTER:
                m_EnemyDispOffset.y = 155.0f / m_MeterPerDot;   //155.0fは旧版から引き継いだ数値
                m_CharaWaistHeight = (-0.5f + 0.5f) * 0.5f;
                break;

            case MasterDataDefineLabel.PivotType.BOTTOM:
                m_CharaWaistHeight = (0.0f + 1.0f) * 0.5f;
                break;

            default:
                break;
        }

        const float CUTIN_SPRITE_SCALE = 1.3f;
        const float CENTER_X_OFFSET_FREE_RANGE = 0.5f;
        if (m_CharaIdCurrent > 0)
        {
            MasterDataParamChara master_chara = BattleParam.m_MasterDataCache.useCharaParam((uint)m_CharaIdCurrent);
            if (master_chara != null)
            {
                switch (m_ImageType)
                {
                    case ImageType.ENEMY_DISP:
                        {
                            m_SpriteScale = master_chara.size_width / m_TextureSize.x;

                            if (master_chara.pivot == MasterDataDefineLabel.PivotType.CENTER)
                            {
                                m_SpriteOffsetPosition = Vector3.zero;
                            }
                            else
                            {
                                m_SpriteOffsetPosition = new Vector3(0.0f, master_chara.size_height / m_MeterPerDot * 0.5f, 0.0f);
                            }
                        }
                        break;

                    case ImageType.CUTIN_DISP:
                    case ImageType.CUTIN_DISP2:
                        {
                            // スキルカットインの時はキャラ表示サイズの大小の差を緩和（小さいものも大きめに表示）
                            float sprite_size = master_chara.size_width * m_CharaScaleAdjust * CUTIN_SPRITE_SCALE;
                            m_SpriteScale = sprite_size / m_TextureSize.x;

                            // 顔が画面外にならないように位置調整
                            float face_offset_x = 0.0f;
                            {
                                float face_center_x = (m_FaceCenterUv.x - 0.5f) * sprite_size;
                                float face_radius = m_FaceUvSize * sprite_size;

                                float limit_left = -CENTER_X_OFFSET_FREE_RANGE * (m_MeterPerDot * 2.0f) + face_radius;
                                float limit_right = CENTER_X_OFFSET_FREE_RANGE * (m_MeterPerDot * 2.0f) - face_radius;
                                float center_x_offset = 0.0f;

                                if (limit_left >= limit_right)
                                {
                                    center_x_offset = -face_center_x;
                                }
                                else
                                {
                                    if (face_center_x < limit_left)
                                    {
                                        center_x_offset = limit_left - face_center_x;
                                    }
                                    if (face_center_x > limit_right)
                                    {
                                        center_x_offset = limit_right - face_center_x;
                                    }
                                }

                                face_offset_x = center_x_offset;
                            }

                            m_SpriteOffsetPosition = m_BaseLocalPostion;

                            switch (master_chara.pivot)
                            {
                                case MasterDataDefineLabel.PivotType.BOTTOM:
                                    m_SpriteOffsetPosition.y += 0.25f * m_TextureSize.y * m_SpriteScale / CUTIN_SPRITE_SCALE;
                                    break;
                            }

                            m_SpriteOffsetPosition.x += face_offset_x;
                        }
                        break;
                }
            }
        }
        else
        {
            switch (m_ImageType)
            {
                case ImageType.ENEMY_DISP:
                    {
                        m_SpriteScale = 1.0f;
                        m_SpriteOffsetPosition = Vector3.zero;
                    }
                    break;

                case ImageType.CUTIN_DISP:
                case ImageType.CUTIN_DISP2:
                    {
                        // 主人公の時
                        m_SpriteScale = CUTIN_SPRITE_SCALE;
                        m_SpriteOffsetPosition = m_BaseLocalPostion;
                        m_SpriteOffsetPosition.y += -160.0f;
                    }
                    break;
            }
        }
    }
}
