using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using DG.Tweening;
using System;

public class RegionDialog : MenuPartsBase
{
    private static readonly float FadeShowAlpha = 0.5f;
    private static readonly float FadeHideAlpha = 0.0f;
    private static readonly float AnimationTime = 0.25f;
    private static readonly float WindowPositionX = -10.0f;

    public MenuPartsBase Window = null;
    public GameObject ShadowPanel = null;
    public GameObject TabButtonObj = null;

    M4uProperty<Sprite> backgroundImage = new M4uProperty<Sprite>();
    public Sprite BackgroundImage { get { return backgroundImage.Value; } set { backgroundImage.Value = value; } }

    M4uProperty<List<RegionContext>> regionList = new M4uProperty<List<RegionContext>>();
    public List<RegionContext> RegionList { get { return regionList.Value; } set { regionList.Value = value; } }

    private System.Action<RegionContext> DidSelectItem = delegate { };
    private Action<bool> m_HideAction = null;

    private int m_SelectIndex = -1;
    private bool m_Ready = false;
    private bool m_Show = false;
    public RectTransform m_WindowRect;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        RegionList = new List<RegionContext>();

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを登録
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnClose);
        }

        m_WindowRect = Window.GetComponent<RectTransform>();
        Window.SetPosition(new Vector2(m_WindowRect.rect.width, m_WindowRect.anchoredPosition.y));
        Window.transform.localScale = new Vector3(0, 0, 0);
    }
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public RegionDialog SetTabButtonParent(GameObject parent)
    {
        return this;
    }

    public RegionDialog AddRegionList(MasterDataRegion[] regionArray, int selectIndex, Action<RegionContext> action, Action<RegionDialog> loadWaitAction)
    {
        if (regionArray == null)
        {
            return this;
        }

        RegionList.Clear();

        // インジケーターを表示
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.ASSETBUNDLE);
        }
        AssetBundlerMultiplier multiplier = AssetBundlerMultiplier.Create();

        for (int i = 0; i < regionArray.Length; i++)
        {
            MasterDataRegion regionMaster = regionArray[i];
            RegionContext newData = new RegionContext();
            newData.master = regionMaster;
            newData.Title = regionMaster.name;
            newData.IsSelect = (i == selectIndex);
            newData.IconImage = null;
            newData.DidSelectItem = OnSelectItem;

            // アセットバンドルの読み込み
            uint[] area_cate_ids = MainMenuUtil.CreateRegionMasterContainedAreaCategoryIDs(regionMaster);
            if (area_cate_ids.Length == 0)
            {
                // 開催期間が切れたなどで、有効なクエストが存在しない
                continue;
            }

            uint area_cate_id = area_cate_ids[area_cate_ids.Length - 1];
            if (area_cate_id != 0)
            {
                string assetBundleName = string.Format("areamapicon_{0}", area_cate_id);
                multiplier.Add(AssetBundler.Create().
                    Set(assetBundleName,
                     (o) =>
                     {
                         newData.IconImage = o.GetAsset<Sprite>();
                         newData.IconImage_mask = o.GetTexture(newData.IconImage.name + "_mask", TextureWrapMode.Clamp);
                     },
                    (s) =>
                    {
                        newData.IconImage = ResourceManager.Instance.Load("maeishoku_icon");
                    }).Load());
            }
            else
            {
                newData.IconImage = ResourceManager.Instance.Load("maeishoku_icon");
            }

            RegionList.Add(newData);
        }


        multiplier.Load(() =>
        {
            // インジケーターを閉じる
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
            }
            if (loadWaitAction != null)
            {
                loadWaitAction(this);
            }
        },
        () =>
        {
            // インジケーターを閉じる
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
            }
            if (loadWaitAction != null)
            {
                loadWaitAction(this);
            }
        });

        m_SelectIndex = selectIndex;
        DidSelectItem = action;

        return this;
    }

    public void Show(Action<bool> hideAction = null)
    {
        if (m_Show)
        {
            return;
        }
        m_Show = true;

        m_HideAction = hideAction;

        Window.SetTopAndBottomAjustStatusBar(new Vector2(27, -354));
        Window.transform.localScale = new Vector3(1, 1, 1);

        ShadowPanel.GetComponent<Image>().DOFade(FadeShowAlpha, AnimationTime);

        Window.GetComponent<RectTransform>().DOAnchorPosX(WindowPositionX, AnimationTime).OnComplete(() =>
        {
            m_Ready = true;
        });
    }

    public void Hide(bool isSelectRegion)
    {
        if (m_Ready == false)
        {
            return;
        }

        m_Show = false;

        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを解除
            AndroidBackKeyManager.Instance.StackPop(gameObject);
        }

        ShadowPanel.GetComponent<Image>().DOFade(FadeHideAlpha, AnimationTime);

        Window.GetComponent<RectTransform>().DOAnchorPosX(m_WindowRect.rect.width, AnimationTime).OnComplete(() =>
        {
            if (m_HideAction != null)
            {
                m_HideAction(isSelectRegion);
            }
            DestroyObject(gameObject);
        });
    }

    private void OnSelectItem(RegionContext context)
    {
        if (m_Ready == false || m_Show == false)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        DidSelectItem(context);

        Hide(true);
    }

    public void OnClose()
    {
        if (m_Ready == false || m_Show == false)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        Hide(false);
    }

    /*-------------------------------------------------------------------------------------*/
    /*                                                                                     */
    /*                                                                                     */
    /*                                                                                     */
    /*-------------------------------------------------------------------------------------*/
    public static RegionDialog Create()
    {
        GameObject _tmpObj = Resources.Load("Prefab/RegionDialog/RegionDialog") as GameObject;
        if (_tmpObj == null)
        {
            return null;
        }

        GameObject _newObj = Instantiate(_tmpObj) as GameObject;
        if (_newObj == null)
        {
            return null;
        }
        UnityUtil.SetObjectEnabledOnce(_newObj, true);

        return _newObj.GetComponent<RegionDialog>();
    }
}
