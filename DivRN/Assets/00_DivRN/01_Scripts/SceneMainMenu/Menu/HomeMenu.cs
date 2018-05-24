using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class HomeMenu : MenuPartsBase
{
    [SerializeField]
    protected GameObject[] m_elementRoots;

    public System.Action DidSelectStory = delegate { };
    public System.Action DidSelectUnitInfo = delegate { };
    public System.Action DidSelectBossInfo = delegate { };
    public System.Action DidSelectGoodInfo = delegate { };
    public System.Action DidSelectMaster = delegate { };
    public System.Action DidSelectBanner = delegate { };
    public System.Action DidSelectScoreInfo = delegate { };
    public System.Action DidSelectMission = delegate { };
    public System.Action DidSelectPresent = delegate { };
    public System.Action DidSelectChallenge = delegate { };

    public static readonly string AppearAnimationName = "mainmenu_home_appear";
    public static readonly string AppearShortAnimationName = "mainmenu_home_appear_short";
    public static readonly string DisappearAnimationName = "mainmenu_home_disappear";
    public static readonly string DefaultAnimationName = "mainmenu_home_loop";

    private List<ButtonModel> m_elements = new List<ButtonModel>();

    private ChallengeBossButton m_ChallengeButton = null;
    public ChallengeBossButton ChallengeButton { get { return m_ChallengeButton; } }


    M4uProperty<Sprite> heroImage = new M4uProperty<Sprite>();
    /// <summary>主人公画像</summary>
    public Sprite HeroImage
    {
        get { return heroImage.Value; }
        set
        {
            heroImage.Value = value;
            IsActiveHeroImage = (value != null);
        }
    }

    M4uProperty<Texture> heroImage_mask = new M4uProperty<Texture>();
    /// <summary>主人公画像</summary>
    public Texture HeroImage_mask
    {
        get { return heroImage_mask.Value; }
        set
        {
            heroImage_mask.Value = value;
        }
    }

    M4uProperty<bool> isActiveHeroImage = new M4uProperty<bool>(false);
    /// <summary>主人公画像の表示・非表示</summary>
    public bool IsActiveHeroImage { get { return isActiveHeroImage.Value; } set { isActiveHeroImage.Value = value; } }

    M4uProperty<bool> isActiveEventFlag = new M4uProperty<bool>();
    public bool IsActiveEventFlag { get { return isActiveEventFlag.Value; } set { isActiveEventFlag.Value = value; } }

    M4uProperty<bool> isActiveMissionFlag = new M4uProperty<bool>();
    public bool IsActiveMissionFlag { get { return isActiveMissionFlag.Value; } set { isActiveMissionFlag.Value = value; } }

    M4uProperty<bool> isActivePresentFlag = new M4uProperty<bool>();
    public bool IsActivePresentFlag { get { return isActivePresentFlag.Value; } set { isActivePresentFlag.Value = value; } }

    M4uProperty<bool> isViewScoreInfo = new M4uProperty<bool>();
    public bool IsViewScoreInfo { get { return isViewScoreInfo.Value; } set { isViewScoreInfo.Value = value; } }

    M4uProperty<bool> isViewChallenge = new M4uProperty<bool>();
    public bool IsViewChallenge { get { return isViewChallenge.Value; } set { isViewChallenge.Value = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        IsViewScoreInfo = false;
        IsViewChallenge = false;

        SetUpElements();
    }

    public void Initialize()
    {
        SetUpAppearEffect();
    }

    public void OnSelectStory()
    {
        DidSelectStory();
    }

    public void OnSelectUnitInfo()
    {
        DidSelectUnitInfo();
    }


    public void OnSelectGoodInfo()
    {
        DidSelectGoodInfo();
    }

    public void OnSelectMaster()
    {
        DidSelectMaster();
    }

    public void OnSelectBanner()
    {
        DidSelectBanner();
    }

    public void OnSelectScoreInfo()
    {
        DidSelectScoreInfo();
    }

    public void OnSelectMission()
    {
        DidSelectMission();
    }

    public void OnSelectPresent()
    {
        DidSelectPresent();
    }

    public void OnSelectChallenge()
    {
        DidSelectChallenge();
    }


    private void SetUpElements()
    {
        var elementActoinMap = new List<System.Action>
        {
            OnSelectMaster,
            //OnSelectGoodInfo,
            OnSelectUnitInfo,
            OnSelectScoreInfo,
            OnSelectMission,
            OnSelectPresent,
            OnSelectChallenge,
        };

        var elementCreateViewMap = new List<System.Action<GameObject, ButtonModel>>
        {
            (GameObject parent, ButtonModel model)=> { HomeMenuCharacterChange.Attach(parent).SetModel(model); },
            //(GameObject parent, ButtonModel model)=> { HomeMenuGoodInfo.Attach(parent).SetModel(model); },
            (GameObject parent, ButtonModel model)=> { HomeMenuUnitInfo.Attach(parent).SetModel(model); },
            (GameObject parent, ButtonModel model)=> { HomeMenuScoreInfo.Attach(parent).SetModel(model); },
            (GameObject parent, ButtonModel model)=> { HomeMenuMission.Attach(parent).SetModel(model); },
            (GameObject parent, ButtonModel model)=> { HomeMenuPresent.Attach(parent).SetModel(model); },
            (GameObject parent, ButtonModel model)=> { m_ChallengeButton = ChallengeBossButton.Attach(parent).SetModel(model); },
        };

        UnityEngine.Debug.Assert(elementActoinMap.Count == elementCreateViewMap.Count, "The const maps are invalid.");

        int size = m_elementRoots.Length;
        for (int i = 0; i < size; i++)
        {
            int index = i;
            if (m_elementRoots[index] == null)
            {
                continue;
            }

            var model = new ButtonModel();
            model.OnClicked += () =>
            {
                elementActoinMap[index]();
            };
            elementCreateViewMap[index](m_elementRoots[index], model);
            m_elements.Add(model);
        }
    }


    // TODO : 整理
    private static bool s_isAppearEffectAlreadyShowed = false;
    private void SetUpAppearEffect()
    {
        EffectProcessor.Instance.Register("HomeMenu", (System.Action finish) =>
        {
            bool isReady = false;

            RegisterKeyEventCallback("next",
                () =>
                {
                    isReady = true;
                    finish();
                });

            PlayAnimation(s_isAppearEffectAlreadyShowed
                        ? AppearShortAnimationName
                        : AppearAnimationName,
                        () =>
                        {
                            isReady = true;
                        });

            InputLayer.Instance.OnAnyTouchBeganCallbackOnce = (Vector3 touchPosition) =>
            {
                if (isReady)
                    return;

                PlayAnimation(DefaultAnimationName);
                FinishAnimation(AppearAnimationName);

                foreach (var element in m_elements)
                    element.SkipAppearing();

                finish();
            };

            s_isAppearEffectAlreadyShowed = true;
        });
    }

    public void ShowElement(int index)
    {
        UnityEngine.Debug.Assert(index < m_elements.Count, "Called ShowElement() with a invalid argument");

        m_elements[index].Appear();
    }

    public void ShowElementEnd(int index)
    {
        if (index >= m_elements.Count)
        {
            return;
        }
        for (int i = index; i < m_elements.Count; i++)
        {
            m_elements[i].Appear();
        }
    }
}
