using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class MenuBanner : MenuPartsBase
{
    public Banner banner = null;
    M4uProperty<List<MenuBannerContext>> itemList = new M4uProperty<List<MenuBannerContext>>();
    public List<MenuBannerContext> ItemList { get { return itemList.Value; } set { itemList.Value = value; } }

    M4uProperty<Sprite> mainBannerImage = new M4uProperty<Sprite>();
    public Sprite MainBannerImage { get { return mainBannerImage.Value; } set { mainBannerImage.Value = value; } }

    public static readonly string AppearAnimationName = "mainmenu_banner_appear";
    public static readonly string DisappearAnimationName = "mainmenu_banner_disappear";
    public static readonly string DefaultAnimationName = "mainmenu_banner_loop";

    private bool m_bQuest;

    private void Awake()
    {
        Debug.Assert(banner != null, "The banner was not assigned.");

        GetComponent<M4uContextRoot>().Context = this;
        ItemList = new List<MenuBannerContext>();

    }

    void Start()
    {
    }

    void Update()
    {
        if (banner.IsError)
        {
            if (UnityUtil.ChkObjectEnabled(gameObject)) UnityUtil.SetObjectEnabledOnce(gameObject, false);
        }
        else
        {
            if (!UnityUtil.ChkObjectEnabled(gameObject)) UnityUtil.SetObjectEnabledOnce(gameObject, true);
        }
    }

    public void bannerSetup(bool Quest = false)
    {
        m_bQuest = Quest;
        banner.LoadAndShow(Quest ? Banner.Type.Quest : Banner.Type.Menu,
                          bannerSetupFinish);

        // TODO : バナーのデータを読み込み終わってから出現させるなら逐次処理に加える
        EffectProcessor.Instance.Register("MenuBanner", (System.Action finish) =>
        {
            PlayAnimation(AppearAnimationName,
                () =>
                {
                });
            banner.ShowCollection();

            finish();
        });
    }


    void bannerSetupFinish()
    {
        if (banner == null || banner.Collection == null)
        {
            return;
        }

        ItemList.Clear();
        for (int i = 0; i < banner.Collection.Count; i++)
        {
            MenuBannerContext _newItem = new MenuBannerContext();
            ItemList.Add(_newItem);
        }
        StartCoroutine(WaitSetIndex(banner));
    }

    public void OnSelectLeft()
    {
        if (banner == null)
        {
            return;
        }

        banner.carouselRotator.Step(false);
        banner.carouselRotator.ResetWaitTimer();
    }

    public void OnSelectRight()
    {
        if (banner == null)
        {
            return;
        }

        banner.carouselRotator.Step(true);
        banner.carouselRotator.ResetWaitTimer();
    }

    override public void FinishAnimation(string animationName)
    {
        var animationEventMap = new Dictionary<string, System.Action>
        {
            {
                AppearAnimationName ,
                ()=>
                {

                }
            },
            {
                DisappearAnimationName ,
                ()=>
                {

                }
            }
        };

        if (animationEventMap.ContainsKey(animationName))
            animationEventMap[animationName]();

        base.FinishAnimation(animationName);
    }

    IEnumerator WaitSetIndex(Banner _banner)
    {
        while (_banner != null && _banner.carouselRotator.IsToggles() == false)
        {
            yield return null;
        }

        int index = MainMenuParam.m_BannerLastIndexHome;
        if (m_bQuest == true)
        {
            index = MainMenuParam.m_BannerLastIndexQuest;
        }

        if (_banner != null)
        {
            _banner.carouselRotator.SetIndex(index);
        }
    }
}
