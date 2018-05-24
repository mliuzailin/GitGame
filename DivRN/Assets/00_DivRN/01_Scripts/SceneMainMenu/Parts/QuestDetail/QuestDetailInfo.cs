using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class QuestDetailInfo : MenuPartsBase
{
    [SerializeField]
    private GameObject m_panelRoot;

    private static readonly string AppearAnimationName = "quest_detail_info_appear";
    private static readonly string DefaultAnimationName = "quest_detail_info_default";


    private System.Action _onAppeared = null;

    private QuestDetailModel m_model = null;
    public void SetModel(QuestDetailModel model)
    {
        m_model = model;
        SetUpLayout();
    }

    private QuestDetailTab m_QuestDetailTab = null;
    public QuestDetailTab tab
    {
        get { return m_QuestDetailTab; }
        set
        {
            m_QuestDetailTab = value;
        }
    }


    M4uProperty<string> countLabel = new M4uProperty<string>();
    public string CountLabel { get { return countLabel.Value; } set { countLabel.Value = value; } }

    M4uProperty<string> countValue = new M4uProperty<string>();
    public string CountValue { get { return countValue.Value; } set { countValue.Value = value; } }

    M4uProperty<string> bossLabel = new M4uProperty<string>();
    public string BossLabel { get { return bossLabel.Value; } set { bossLabel.Value = value; } }

    M4uProperty<string> bossName = new M4uProperty<string>();
    public string BossName { get { return bossName.Value; } set { bossName.Value = value; } }

    M4uProperty<string> expLabel = new M4uProperty<string>();
    public string ExpLabel { get { return expLabel.Value; } set { expLabel.Value = value; } }

    M4uProperty<string> expValue = new M4uProperty<string>();
    public string ExpValue { get { return expValue.Value; } set { expValue.Value = value; } }

    M4uProperty<string> coinLabel = new M4uProperty<string>();
    public string CoinLabel { get { return coinLabel.Value; } set { coinLabel.Value = value; } }

    M4uProperty<string> coinValue = new M4uProperty<string>();
    public string CoinValue { get { return coinValue.Value; } set { coinValue.Value = value; } }



    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    private void Start()
    {
        if (SafeAreaControl.Instance)
        {
            SafeAreaControl.Instance.addLocalYPos(transform);
        }
    }

    public void Show(System.Action callback)
    {
        _onAppeared = callback;

        PlayAnimation(AppearAnimationName, () =>
        {
            if (_onAppeared != null)
                _onAppeared();
            PlayAnimation(DefaultAnimationName);
        });


        RegisterKeyEventCallback("show_buttons", () =>
        {
            tab.Show();
        });
    }

    public void Hide()
    {
        m_panelRoot.SetActive(false);

        tab.Hide();
    }


    private void SetUpLayout()
    {
        Debug.Assert(m_model != null, "model not set.");
        var rectTransform = GetComponent<RectTransform>();
        Debug.Assert(rectTransform != null, "RectTransform not set.");
        m_model.SetInfoPanelHeight(rectTransform.rect.height);
    }
}
