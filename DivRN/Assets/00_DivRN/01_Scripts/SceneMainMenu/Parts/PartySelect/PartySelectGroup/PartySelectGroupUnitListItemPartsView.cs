using UnityEngine;

public class PartySelectGroupUnitListItemPartsView : View
{
    protected string AppearAnimationName = "party_select_name_appear";
    protected string DefaultAnimationName = "party_select_name_loop";
    protected string SelectAnimationName = "party_select_name_select";

    private bool m_isShowed = false;


    public void Show(
                    System.Action onNextCallback = null,
                    System.Action onFinishedCallback = null)
    {
        PlayAnimation(AppearAnimationName, () =>
        {
            PlayAnimation(DefaultAnimationName);

            if (onFinishedCallback != null)
                onFinishedCallback();
        });

        RegisterKeyEventCallback("next", () =>
        {
            m_isShowed = true;

            if (onNextCallback != null)
                onNextCallback();
        });
    }

    public void Select(System.Action onFinishedCallback = null)
    {
        PlayAnimation(SelectAnimationName, () =>
        {
            PlayAnimation(DefaultAnimationName);

            if (onFinishedCallback != null)
                onFinishedCallback();
        });
    }

    public bool isShowed { get { return m_isShowed; } }
}
