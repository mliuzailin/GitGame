using UnityEngine;
using UnityEngine.UI;

public class PartySelectGroupUnitListItemIconView : PartySelectGroupUnitListItemPartsView
{
    [SerializeField]
    private Image m_iconImage;

    private static readonly string AppearArrowAnimationName = "party_select_icon_arrow_appear";
    private static readonly string DisappearArrowAnimationName = "party_select_icon_arrow_disappear";

    private static readonly string GrayScaleMaterialPath = "UIData/Material/GrayMaterialWithMask";

    private Material m_defaultMaterial = null;

    void Awake()
    {
        AppearAnimationName = "party_select_icon_appear";
        DefaultAnimationName = "party_select_icon_loop";
        SelectAnimationName = "party_select_icon_select";

        m_defaultMaterial = m_iconImage.material;
    }

    public void ShowArrow()
    {
        DefaultColor();
        PlayAnimation(AppearArrowAnimationName, () =>
        {
            PlayAnimation(DefaultAnimationName);
        });
    }

    public void HideArrow()
    {
        PlayAnimation(DisappearArrowAnimationName, () =>
        {
            GrayScale();
            PlayAnimation(DefaultAnimationName);
        });
    }

    public void GrayScale()
    {
        ImageUtil.GetMaterial(GrayScaleMaterialPath, (Material material) =>
        {
            m_iconImage.material = material;
        });
    }

    public void DefaultColor()
    {
        m_iconImage.material = m_defaultMaterial;
    }
}
