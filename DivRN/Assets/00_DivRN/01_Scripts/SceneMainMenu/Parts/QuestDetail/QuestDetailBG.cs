using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using System;

public class QuestDetailBG : MenuPartsBase
{
    [SerializeField]
    private GameObject m_confirmButtonRoot;
    [SerializeField]
    private Animation m_charaAnimation;

    [SerializeField]
    private GameObject m_windowRoot;
    [SerializeField]
    private GameObject m_questInfoRoot;
    [SerializeField]
    private GameObject m_windowPanelRoot;
    [SerializeField]
    private Image m_charaUIImage;

    [SerializeField]
    private RectTransform m_windowPanel = null;
    private Vector2 m_defaultWindowPanelSize;

    private static readonly string ConfirmButtonPrefabPath = "Prefab/QuestDetail/QuestDetailConfirmButton";

    private static readonly string AppearAnimationName = "quest_detail_bg_appear";
    private static readonly string DefaultAnimationName = "quest_detail_bg_default";

    private static readonly string CharaAppearAnimationName = "quest_detail_bg_chara_appear";


    private System.Action _onAppeared = null;


    private AssetAutoSetCharaImage m_CharaImage = null;


    M4uProperty<string> questIdLabel = new M4uProperty<string>();
    public string QuestIdLabel { get { return questIdLabel.Value; } set { questIdLabel.Value = value; } }

    M4uProperty<int> questId = new M4uProperty<int>();
    public int QuestId { get { return questId.Value; } set { questId.Value = value; } }

    M4uProperty<string> questTitle = new M4uProperty<string>();
    public string QuestTitle { get { return questTitle.Value; } set { questTitle.Value = value; } }

    M4uProperty<string> areaTitle = new M4uProperty<string>();
    public string AreaTitle { get { return areaTitle.Value; } set { areaTitle.Value = value; } }

    M4uProperty<string> areaCategoryTitle = new M4uProperty<string>();
    public string AreaCategoryTitle { get { return areaCategoryTitle.Value; } set { areaCategoryTitle.Value = value; } }

    M4uProperty<string> buttonTitle = new M4uProperty<string>();
    public string ButtonTitle { get { return buttonTitle.Value; } set { buttonTitle.Value = value; } }

    M4uProperty<Sprite> bossImage = new M4uProperty<Sprite>();
    public Sprite BossImage { get { return bossImage.Value; } set { bossImage.Value = value; } }

	M4uProperty<Sprite> iconSelect = new M4uProperty<Sprite>();
	public Sprite IconSelect { get { return iconSelect.Value; } set { iconSelect.Value = value; } }

	public System.Action DidSelectButton = delegate { };

    public AssetAutoSetEpisodeBackgroundTexture assetAutoSetEpisodeBackgroundTexture;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_CharaImage = GetComponentInChildren<AssetAutoSetCharaImage>();

        SetUpButtons();

        m_defaultWindowPanelSize = m_windowPanel.sizeDelta;
    }

    public void Load(uint areaCategoryId)
    {
        assetAutoSetEpisodeBackgroundTexture.Create(areaCategoryId).Load();
    }

    void Update()
    {
        if (m_sizeTo != null)
        {
            m_sizeTo.Tick(Time.deltaTime);
        }
    }

    public void OnSelectButton()
    {
        DidSelectButton();
    }

    public void setupChara(uint _charId)
    {
        if (m_CharaImage == null)
        {
            return;
        }
        m_CharaImage.SetCharaID(_charId, true);
    }

    public void Show(System.Action callback)
    {
        _onAppeared = callback;

        PlayAnimation(AppearAnimationName, () =>
        {
            PlayAnimation(DefaultAnimationName);
        });

        RegisterKeyEventCallback("appeared", () =>
        {
            if (_onAppeared != null)
            {
                _onAppeared();
            }

            _onAppeared = null;
        });
    }
    public void Hide()
    {
        m_windowRoot.SetActive(false);
        m_questInfoRoot.SetActive(false);
        m_windowPanelRoot.SetActive(false);
        m_windowPanel.sizeDelta = m_defaultWindowPanelSize;
    }

    public void ShowChara(Action finishAction)
    {
        if (m_charaAnimation == null)
        {
            return;
        }

        m_charaAnimation.Stop();
        StartCoroutine(LoopCheckBossImageLoad(() =>
        {
            m_charaAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
            m_charaAnimation.PlayQueued(CharaAppearAnimationName, QueueMode.PlayNow);

            if (finishAction != null)
            {
                finishAction();
            }
        }));
    }

    /// <summary>
    /// ボス画像の読み込み待ち
    /// </summary>
    /// <param name="finishAction"></param>
    /// <returns></returns>
    public IEnumerator LoopCheckBossImageLoad(Action finishAction)
    {
        while (m_CharaImage.Ready == false)
        {
            yield return null;
        }

        if (finishAction != null)
        {
            finishAction();
        }
    }


    public void HideChara()
    {
        m_charaUIImage.enabled = false;
    }

    private void SetUpButtons()
    {
        var confirmButtonModel = new ButtonModel();
        ButtonView
            .Attach<ButtonView>(ConfirmButtonPrefabPath, m_confirmButtonRoot)
            .SetModel<ButtonModel>(confirmButtonModel);
        confirmButtonModel.OnClicked += () =>
        {
            OnSelectButton();
        };

        // TODO : 演出を入れるならその場所に移動
        confirmButtonModel.Appear();
        confirmButtonModel.SkipAppearing();
    }

    public void Change(float height, float duration, System.Action callback = null)
    {
        Debug.Assert(m_windowPanel != null, "rect transform is not set(uses for a change animation).");

        m_sizeTo = new SizeTo(m_windowPanel)
            .SetDestination(new Vector2
                                (
                                    m_windowPanel.sizeDelta.x,
                                    height
                                ))
            .SetDuration(duration)
            .RegisterOnFinishedCallback(() =>
            {
                if (callback != null)
                {
                    callback();
                }

                m_sizeTo = null;
            });
    }
    private SizeTo m_sizeTo = null;


    // TODO : 別ファイルに
    class SizeTo
    {
        private RectTransform m_targetRectTransform;
        private Vector2 m_original;
        private Vector2 m_destination;
        private float m_duration;
        private float m_elaspedTime = 0;
        private System.Action m_onFinished = null;

        public SizeTo(RectTransform targetRectTransform)
        {
            m_targetRectTransform = targetRectTransform;
            m_original = m_destination = targetRectTransform.sizeDelta;
        }

        public SizeTo SetDestination(Vector2 size)
        {
            m_destination = size;
            return this;
        }

        public SizeTo SetDuration(float duration)
        {
            m_duration = duration;
            return this;
        }

        public SizeTo RegisterOnFinishedCallback(System.Action callback)
        {
            m_onFinished = callback;

            return this;
        }

        public void Tick(float delta)
        {
            if (m_elaspedTime >= m_duration)
            {
                return;
            }

            m_elaspedTime += delta;

            float ratio = m_elaspedTime / m_duration;
            m_targetRectTransform.sizeDelta =
                m_original + (m_destination - m_original) * ratio;

            if (m_elaspedTime >= m_duration)
            {
                m_targetRectTransform.sizeDelta = m_destination;

                if (m_onFinished != null)
                {
                    m_onFinished();
                }

            }
        }
    }
}
