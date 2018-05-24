using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AreaSelectSwitchButton : ButtonView
{
    [SerializeField]
    private TextMeshProUGUI m_labelText = null;

    [SerializeField]
    private Button m_button = null;
    [SerializeField]
    private Image m_buttonImageActive;
    [SerializeField]
    private Image m_buttonImageInactive;
    [SerializeField]
    private Animation m_buttonLabelAnimation;

    public static readonly string PrefabPath = "Prefab/AreaSelect/AreaSelectSwitchButton";
    private readonly string AreaSelectBufAnimationName = "mainmenu_area_select_buf_loop";
    private readonly string AreaSelectBaseAnimationName = "mainmenu_area_select_loop";

    private AreaSelectSwitchButtonModel m_model = null;
    private bool m_bufEvent = false;
    public bool m_bufEventAnimationStart = false;

    public static AreaSelectSwitchButton Attach(GameObject parent)
    {
        return ButtonView.Attach<AreaSelectSwitchButton>(PrefabPath, parent);
    }

    public AreaSelectSwitchButton SetModel(AreaSelectSwitchButtonModel model, bool bufEvent)
    {
        m_model = model;
        m_bufEvent = bufEvent;

        m_model.OnUpdated += () =>
        {
            SetLabelText(m_model.labelText);
            UpdateButtonStatus();
        };

        base.SetModel<AreaSelectSwitchButtonModel>(m_model);

        RegisterKeyEventCallback("next", () => { ShowNext(); });

        return this;
    }

    void Awake()
    {
        AppearAnimationName = "area_select_switch_bubtton_appear";
        DefaultAnimationName = "area_select_switch_bubtton_loop";
        m_bufEventAnimationStart = false;
    }

    public void ShowNext()
    {
        m_model.ShowNext();
    }

    private void SetLabelText(string text)
    {
        if (m_labelText == null)
        {
            return;
        }

        m_labelText.text = text;
    }
    private void SetButtonImage(string imageFile)
    {

    }

    private void UpdateButtonStatus()
    {
        if (m_button == null)
            return;

        m_button.image = m_model.isSelected
                        ? m_buttonImageActive
                        : m_buttonImageInactive;
        m_buttonImageActive.gameObject.SetActive(m_model.isSelected);
        m_buttonImageInactive.gameObject.SetActive(!m_model.isSelected);
    }

    public void setBufEvent(bool sw, float startTime = 0)
    {
        if (m_bufEventAnimationStart == false)
        {
            return;
        }
        if (m_bufEvent == true)
        {
            if (sw == true)
            {
                m_buttonLabelAnimation[AreaSelectBufAnimationName].time = startTime;
                m_buttonLabelAnimation.Play(AreaSelectBufAnimationName);
            }
            else
            {
                m_buttonLabelAnimation.Play(AreaSelectBaseAnimationName);
            }
        }
    }
}
