using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スプライト差し替えコンポーネント
/// 差し替え対象のスプライトを持つゲームオブジェクト（SpriteRendererかImageコンポーネントを持っている必要がある）
/// にこのコンポーネントを追加すると、差し替え情報に基づいてスプライトを置き換えます。
/// </summary>
public class ReplaceAssetSprite : MonoBehaviour
{
    [SerializeField]
    private bool m_IsHideInLoading = false;	// 差し替え情報ロード中は表示を消すかどうかの設定

    // チェック対象のコンポーネント。SpriteRenderer と Image が同時に使われることはないはず。
    private SpriteRenderer m_SpriteRendererComponent = null;
    private Image m_ImageComponent = null;

    private Sprite m_SrcSprite = null;  // 差し替え元のスプライト
    private Sprite m_DstSprite = null;  // 差し替え後のスプライト
    private Material m_BaseMaterial = null; // コンポーネントにもともと設定されていたマテリアル（差し替え対象外のスプライトの場合にこれに戻す）

    private int m_SerialNo = 0;

    private bool m_IsHiding = false;    // 非表示中かどうか

    private void Awake()
    {
        m_SpriteRendererComponent = GetComponent<SpriteRenderer>();
        if (m_SpriteRendererComponent != null)
        {
            m_BaseMaterial = m_SpriteRendererComponent.material;
        }

        m_ImageComponent = GetComponent<Image>();
        if (m_ImageComponent != null)
        {
            m_BaseMaterial = m_ImageComponent.material;
        }

        _update();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _update();
    }

       private void OnDisable()
    {
        _revert();
    }

    private void _update()
    {
        int serial_no = ReplaceAssetManager.Instance.getSerialNo();
        if (m_SerialNo != serial_no)
        {
            m_SerialNo = serial_no;
            _revert();
            m_DstSprite = null;
        }

        if (ReplaceAssetManager.Instance.getStatus() == ReplaceAssetManager.Status.REPLACE_MODE)
        {
            if (m_SpriteRendererComponent != null
                && m_SpriteRendererComponent.sprite != m_DstSprite
            )
            {
                Sprite sprite = null;
                bool is_replace = ReplaceAssetManager.Instance.getReplaceSprite(m_SpriteRendererComponent.sprite.name, out sprite);
                if (is_replace)
                {
                    m_SrcSprite = m_SpriteRendererComponent.sprite;
                    m_SpriteRendererComponent.sprite = sprite;

                    // マテリアルはデフォルトの物を設定する(差し替えテクスチャのテクスチャアトラス化には未対応)
                    m_SpriteRendererComponent.material = ReplaceAssetManager.Instance.m_SpriteDefaultMaterial;
                }
                else
                {
                    // 置き換え対象外のスプライトなのでもともとのマテリアルに戻す.
                    m_SpriteRendererComponent.material = m_BaseMaterial;
                }

                m_DstSprite = m_SpriteRendererComponent.sprite;
            }

            if (m_ImageComponent != null
                && m_ImageComponent.sprite != m_DstSprite
            )
            {
                Sprite sprite = null;
                bool is_replace = ReplaceAssetManager.Instance.getReplaceSprite(m_ImageComponent.sprite.name, out sprite);
                if (is_replace)
                {
                    m_SrcSprite = m_ImageComponent.sprite;
                    m_ImageComponent.sprite = sprite;

                    // マテリアルはデフォルトの物を設定する(差し替えテクスチャのテクスチャアトラス化には未対応)
                    m_ImageComponent.material = null;

                    m_ImageComponent.SetNativeSize();
                }
                else
                {
                    // 置き換え対象外のスプライトなのでもともとのマテリアルに戻す.
                    m_ImageComponent.material = m_BaseMaterial;
                }

                m_DstSprite = m_ImageComponent.sprite;
            }
        }
        else
        {
            if (m_SrcSprite != null)
            {
                _revert();
            }
        }

        if (m_IsHideInLoading)
        {
            // 差し替え情報ロード中は表示を消す処理
            if (ReplaceAssetManager.Instance.getStatus() == ReplaceAssetManager.Status.LOADING)
            {
                if (m_IsHiding == false)
                {
                    m_IsHiding = true;
                    if (m_SpriteRendererComponent != null)
                    {
                        m_SpriteRendererComponent.enabled = false;
                    }
                    if (m_ImageComponent != null)
                    {
                        m_ImageComponent.enabled = false;
                    }
                }
            }
            else
            {
                if (m_IsHiding)
                {
                    m_IsHiding = false;
                    if (m_SpriteRendererComponent != null)
                    {
                        m_SpriteRendererComponent.enabled = true;
                    }
                    if (m_ImageComponent != null)
                    {
                        m_ImageComponent.enabled = true;
                    }
                }
            }
        }
    }

    private void _revert()
    {
        if (m_SrcSprite != null)
        {
            if (m_SpriteRendererComponent != null)
            {
                m_SpriteRendererComponent.sprite = m_SrcSprite;
                m_SpriteRendererComponent.material = m_BaseMaterial;
                m_DstSprite = null;
            }

            if (m_ImageComponent != null)
            {
                m_ImageComponent.sprite = m_SrcSprite;
                m_ImageComponent.material = m_BaseMaterial;
                m_DstSprite = null;
            }

            m_SrcSprite = null;
        }
    }
}
