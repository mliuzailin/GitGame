using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class UnitResultEvolve : UnitResultBase
{
    [SerializeField]
    private GameObject m_unitEffectRoot;
    [SerializeField]
    private Image m_beforeUnitImage;
    [SerializeField]
    private RectTransform m_beforeUnitRectTransform;

    [SerializeField]
    private Image m_afterUnitImage;
    [SerializeField]
    private RectTransform m_afterUnitRectTransform;
    [SerializeField]
    private Image m_afterUnitEffectImage;
    [SerializeField]
    private RectTransform m_afterUnitEffectRectTransform;

    [SerializeField]
    private Image[] m_materialImages;

    [SerializeField]
    private GameObject m_materialEffectRoot;
    [SerializeField]
    private RectTransform[] m_materialRectTransforms;

    // RectTransformだとworld座標変換自前・・・？
    [SerializeField]
    private RectTransform[] m_materialRotRectTransforms;
    [SerializeField]
    private RectTransform[] m_materialMoveRectTransforms;


    private static readonly string AnimationName = "unit_result_evolve";

    public AssetAutoSetCharaMesh Before = null;
    public AssetAutoSetCharaMesh After = null;

    M4uProperty<Sprite> material1 = new M4uProperty<Sprite>();
    public Sprite Material1 { get { return material1.Value; } set { material1.Value = value; } }

    M4uProperty<Sprite> material2 = new M4uProperty<Sprite>();
    public Sprite Material2 { get { return material2.Value; } set { material2.Value = value; } }

    M4uProperty<Sprite> material3 = new M4uProperty<Sprite>();
    public Sprite Material3 { get { return material3.Value; } set { material3.Value = value; } }


    private uint m_BeforCharaId = 0;
    private uint m_AfterCharaId = 0;
    private uint[] m_MaterialCharaIds = null;

    private void Update()
    {
        if (!m_Ready)
            updateLoadWait();
    }

    public void Initialize(
        uint chara_before_id,
        uint chara_after_id,
        uint[] chara_material_ids,
        System.Action callback)
    {
        m_BeforCharaId = chara_before_id;
        m_AfterCharaId = chara_after_id;
        m_MaterialCharaIds = chara_material_ids;

        initLoadWait();

        if (callback != null)
            callback();
    }

    public void Show(System.Action finish)
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_EVOLVE);
        PlayAnimation(AnimationName, () =>
        {
            if (finish != null)
                finish();
        });

        RegisterKeyEventCallback("se_roll", () =>
        {
            SoundUtil.PlaySE(SEID.SE_MM_D09_EVOLVE_ROLL);
        });

        RegisterKeyEventCallback("se_comp", () =>
        {
            SoundUtil.PlaySE(SEID.SE_MM_D10_EVOLVE_COMP);
        });
    }

    public void AttachCharaAfterImage()
    {
        AfterImageEffect
            .Attach(m_unitEffectRoot)
            .SetImage(m_beforeUnitImage.sprite)
            .SetPosition(m_beforeUnitRectTransform.localPosition)
            .Show();
    }
    public void AttachMaterialsAfterImage()
    {
        for (int i = 0; i < m_materialImages.Length; i++)
        {
            if (!m_materialImages[i].enabled)
                continue;

            // アイコンの表示位置が該当のTransformではなくMove(GameObject)とRot(GameObject)で設定してある前提
            float distance = m_materialMoveRectTransforms[i].anchoredPosition.y;
            float theta = m_materialRotRectTransforms[i].localRotation.eulerAngles.z + 90;
            float radian = Mathf.PI * theta / 180;
            var position = new Vector2(
                Mathf.Cos(radian) * distance,
                Mathf.Sin(radian) * distance
                );

            AfterImageEffect
                .Attach(m_materialEffectRoot)
                .SetImage(m_materialImages[i].sprite)
                .SetPosition(position)
                .SetSize(m_materialRectTransforms[i].sizeDelta)
                .Show();
        }
    }

    private bool initLoadWait()
    {
        Before.SetCharaID(m_BeforCharaId, true);
        After.SetCharaID(m_AfterCharaId, true);

        m_materialImages[0].enabled = false;
        m_materialImages[1].enabled = false;
        m_materialImages[2].enabled = false;

        SetMaterial(0, m_MaterialCharaIds[0]);
        SetMaterial(1, m_MaterialCharaIds[1]);
        SetMaterial(2, m_MaterialCharaIds[2]);

        return false;
    }

    private void SetMaterial(int _index, uint _chara_id)
    {
        Sprite image = null;
        if (_chara_id != 0)
        {
            UnitIconImageProvider.Instance.Get(
                _chara_id,
                sprite =>
                {
                    image = sprite;
                    switch (_index)
                    {
                        case 0:
                            Material1 = image;
                            break;
                        case 1:
                            Material2 = image;
                            break;
                        case 2:
                            Material3 = image;
                            break;
                    }
                    m_materialImages[_index].enabled = image != null;
                });
        }
    }

    private void updateLoadWait()
    {
        if (Before.Ready && After.Ready)
        {
            m_afterUnitEffectImage.sprite = m_afterUnitImage.sprite;
            m_afterUnitEffectRectTransform.sizeDelta = m_afterUnitRectTransform.sizeDelta;
            m_Ready = true;
        }
    }
}
