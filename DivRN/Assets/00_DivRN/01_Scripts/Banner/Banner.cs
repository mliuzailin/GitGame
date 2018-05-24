using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class BannerData : M4uContext
{
    public string banner;
    public int priority;
    public string link;
    public M4uProperty<Texture2D> texture = new M4uProperty<Texture2D>();
    public M4uProperty<Sprite> spriteImage = new M4uProperty<Sprite>();
    public M4uProperty<bool> isTexture = new M4uProperty<bool>();
    public System.Action<bool, int> changeBanner = delegate { };


    public string JumpToInApp_Place
    {
        get
        {
            return link.Split(':')[0];
        }
    }

    public uint JumpToInApp_Id
    {
        get
        {
            return (uint)link.Split(':')[1].ExtractInt();
        }
    }

    public Texture2D Texture
    {
        get
        {
            return texture.Value;
        }
        set
        {
            texture.Value = value;
        }
    }

    public Sprite SpriteImage { get { return spriteImage.Value; } set { spriteImage.Value = value; } }
    public bool IsTexture { get { return isTexture.Value; } set { isTexture.Value = value; } }

    public void LoadTexture(Action action)
    {
        WebResource.Instance.GetTexture2D(banner,
                        (Texture2D texture) =>
                        {
                            Texture = texture;
                            IsTexture = true;
                            action();
                        },
                        () =>
                        {
                            Texture = null;
                            IsTexture = true;
                            action();
                        });
    }


    public static BannerData Create(JsonDAO jdao, ListItemModel listItemModel)
    {
        BannerData d = new BannerData(listItemModel);
        d.banner = String.Format("{0}/{1}", GlobalDefine.GetBaseBannerUrl(), jdao.GetStr("banner"));
        d.priority = jdao.GetInt("priority");
        d.link = jdao.GetStr("link");

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL BatchData#Create banner:" + d.banner + " priority:" + d.priority);
#endif
        return d;
    }
    public static BannerData CreateTitleDefault(ListItemModel listItemModel)
    {
        BannerData d = new BannerData(listItemModel);
        d.banner = "";
        d.priority = 0;
        d.link = "http://divine.example.com/member/";
        d.SpriteImage = ResourceManager.Instance.Load("dummy_banner", ResourceType.Common);
        d.IsTexture = false;
        return d;
    }

    public BannerData(ListItemModel listItemModel)
    {
        m_model = listItemModel;
    }

    private ListItemModel m_model = null;
    public ListItemModel model { get { return m_model; } }
}

public class Banner : View
{
    public enum Type
    {
        Title = 0,
        Menu,
        Quest,
    };

    enum Platform
    {
        NONE = -1,
        IOS,
        ANDROID,
        WINDOWS,
        ALL,
    };

    enum Location
    {
        NONE = 0,
        HOME,
        QUEST,
    };

    public M4uProperty<List<BannerData>> collection = new M4uProperty<List<BannerData>>();
    public AsPerSpec.CarouselRotator carouselRotator;

    private List<ListItemModel> m_banners = new List<ListItemModel>();

    private WaitTimer m_waitTimer = null;

    public List<BannerData> Collection
    {
        get
        {
            return collection.Value;
        }
        set
        {
            collection.Value = value;
        }
    }

    private bool m_bError = false;
    public bool IsError { get { return m_bError; } }

    void Update()
    {
        if (m_waitTimer != null)
        {
            m_waitTimer.Tick(Time.deltaTime);
        }
    }

    public void LoadAndShow(Type _type, System.Action setupFinish = null)
    {
        StartCoroutine(_LoadAndShow(_type, setupFinish));
    }

    IEnumerator _LoadAndShow(Type _type, System.Action setupFinish)
    {
        string url = null;
        System.Action errorAction = null;

        switch (_type)
        {
            case Type.Title:
                url = GlobalDefine.GetTapBannerListUrl();
                errorAction = errorBannerTitle;
                break;
            case Type.Menu:
            case Type.Quest:
                url = GlobalDefine.GetMainMenuBannerListUrl();
                errorAction = errorBannerMenu;
                break;
            default:
                errorAction = errorBannerMenu;
                break;
        }

        if (url == null)
        {
            if (errorAction != null)
            {
                errorAction();
            }

            yield break;
        }

        WWW www = new WWW(url);
        yield return www;
#if BUILD_TYPE_DEBUG
        Debug.Log("url:" + url);
        Debug.Log("JSON:" + www.text);
#endif


        if (www.error.IsNOTNullOrEmpty())
        {
            if (errorAction != null)
            {
                errorAction();
            }
            yield break;
        }

        if (www.text.IsNullOrEmpty())
        {
            if (errorAction != null)
            {
                errorAction();
            }
            yield break;
        }

        List<BannerData> list = new List<BannerData>();
        List<object> objList = MiniJSON.Json.Deserialize(www.text) as List<object>;
        if (objList.Count == 0)
        {
            if (errorAction != null)
            {
                errorAction();
            }
            yield break;
        }

        int count = 0;
        int objCount = 0;

#if UNITY_IPHONE
		Platform bannerPlatform = Platform.IOS;
#elif UNITY_ANDROID
        Platform bannerPlatform = Platform.ANDROID;
#elif UNITY_WINDOWS
		Platform bannerPlatform = Platform.WINDOWS;
#else
        Platform bannerPlatform = Platform.NONE;
#endif

        object[] banners = objList.ToArray();
        for (int bannercount = 0; bannercount < banners.Length; bannercount++)
        {
            JsonDAO jdao = JsonDAO.Create(banners[bannercount]);
            var timing_start = jdao.GetInt("timing_start");
            var timing_end = jdao.GetInt("timing_end");
            var platform = (Platform)jdao.GetInt("platform", (int)Platform.NONE);
            var location = jdao.GetList("location");

            if (platform != bannerPlatform &&
                platform != Platform.ALL)
            {
                continue;
            }

            if (timing_start != 0 &&
                timing_end != 0)
            {
                if (TimeManager.Instance.CheckWithinTime((uint)timing_start, (uint)timing_end) == false)
                {
                    continue;
                }
            }

            bool bCheckScreen = true;
            if (location.Count != 0)
            {
                bCheckScreen = false;
                for (int i = 0; i < location.Count; ++i)
                {
                    switch ((Location)System.Convert.ToInt32(location[i]))
                    {
                        case Location.HOME:
                            if (_type == Type.Menu)
                            {
                                bCheckScreen = true;
                            }
                            break;
                        case Location.QUEST:
                            if (_type == Type.Quest)
                            {
                                bCheckScreen = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (bCheckScreen == false)
            {
                continue;
            }

            var model = new ListItemModel((uint)objCount);
            BannerData d = BannerData.Create(jdao, model);
            m_banners.Add(model);
            objCount++;

            d.LoadTexture(() =>
            {
                count++;

                if (d.Texture != null)
                {
                    list.Add(d);
                }
                if (count == objCount)
                {
                    if (list.Count == 0)
                    {
                        if (errorAction != null)
                        {
                            errorAction();
                        }
                    }
                    else
                    {
                        carouselRotator.ResetWaitTimer();

                        list.Sort((a, b) =>
                        {
                            int ret = a.priority - b.priority;
                            return ret != 0 ? ret : (int)a.model.index - (int)b.model.index;
                        });

                        Collection = list;

                        if (setupFinish != null)
                        {
                            setupFinish();
                        }
                    }
                }
            });
        }
    }

    public void ShowCollection()
    {
        // JSONや画像の読み込みでViewの生成が遅れることがあるのでStart()を待つ
        const float WAIT_INTERVAL_SECOND = 0.5f;
        System.Action SetTimer = null;

        System.Func<bool> IsAllStarted = () =>
        {
            foreach (var banner in m_banners)
                if (!banner.isStarted)
                    return false;

            return true;
        };

        SetTimer = () =>
        {
            if (m_banners.Count == 0
                || !IsAllStarted())
            {
                m_waitTimer = new WaitTimer(WAIT_INTERVAL_SECOND, SetTimer);
                return;
            }

            foreach (var banner in m_banners)
            {
                banner.Appear();
            }
            m_waitTimer = null;
        };

        SetTimer();
    }


    private void errorBannerTitle()
    {
        m_bError = true;
        //デフォルトのバナーを作成して追加
        List<BannerData> list = new List<BannerData>();
        var model = new ListItemModel((uint)0);
        BannerData d = BannerData.CreateTitleDefault(model);
        m_banners.Add(model);
        list.Add(d);
        carouselRotator.ResetWaitTimer();
        Collection = list;
    }

    private void errorBannerMenu()
    {
        m_bError = true;
    }
}
