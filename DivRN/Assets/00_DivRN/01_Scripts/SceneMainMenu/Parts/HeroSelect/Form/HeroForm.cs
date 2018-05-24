/**
 *  @file   HeroForm.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/20
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M4u;
using AsPerSpec;

public class HeroForm : MenuPartsBase
{
    [SerializeField]
    private GameObject m_previousButtonRoot;
    [SerializeField]
    private GameObject m_nextButtonRoot;
    [SerializeField]
    private GameObject m_decisionButton;
    private static readonly string PreviousButtonPrefabPath = "Prefab/HeroSelect/HeroSelectPreviousButton";
    private static readonly string NextButtonPrefabPath = "Prefab/HeroSelect/HeroSelectNextButton";


    private ButtonView m_previousButtonView = null;
    private ButtonView m_nextButtonView = null;
    private ButtonModel m_previousButton = null;
    private ButtonModel m_nextButton = null;

    enum Hero_0001_Sprite_Tag : int
    {
        Birthday = 0,
        Name,
        Face,
        No,
        Select,
    };
    enum Hero_0002_Sprite_Tag : int
    {
        Birthday = 0,
        Name,
        Face,
        No,
        Select,
    };
    enum Hero_0003_Sprite_Tag : int
    {
        Birthday = 0,
        No,
        Name,
        Face,
        Select,
    };
    enum Hero_0004_Sprite_Tag : int
    {
        Birthday = 0,
        Name,
        No,
        Face,
        Select,
    };
    enum Hero_0005_Sprite_Tag : int
    {
        Birthday = 0,
        Name,
        Face,
        No,
        Select,
    };
    enum Hero_0006_Sprite_Tag : int
    {
        Birthday = 0,
        Name,
        Face,
        No,
        Select,
    };


    M4uProperty<List<HeroFormListContext>> formDatas = new M4uProperty<List<HeroFormListContext>>(new List<HeroFormListContext>());
    /// <summary>学生証リスト</summary>
    List<HeroFormListContext> FormDatas
    {
        get
        {
            return formDatas.Value;
        }
        set
        {
            formDatas.Value = value;
        }
    }

    M4uProperty<List<PagePointListItemContext>> points = new M4uProperty<List<PagePointListItemContext>>(new List<PagePointListItemContext>());
    /// <summary>ページコントロールアイコンリスト</summary>
    List<PagePointListItemContext> Points
    {
        get
        {
            return points.Value;
        }
        set
        {
            points.Value = value;
        }
    }

    M4uProperty<bool> isActivePrevButton = new M4uProperty<bool>();
    /// <summary>矢印ボタンの表示・非表示</summary>
    bool IsActivePrevButton { get { return isActivePrevButton.Value; } set { isActivePrevButton.Value = value; } }

    M4uProperty<bool> isActiveNextButton = new M4uProperty<bool>();
    /// <summary>矢印ボタンの表示・非表示</summary>
    bool IsActiveNextButton { get { return isActiveNextButton.Value; } set { isActiveNextButton.Value = value; } }


    M4uProperty<bool> isScrollMoving = new M4uProperty<bool>();
    public bool IsScrollMoving { get { return isScrollMoving.Value; } set { isScrollMoving.Value = value; } }

    List<GameObject> formDataList = new List<GameObject>();
    public List<GameObject> FormDataList { get { return formDataList; } set { formDataList = value; } }

    /// <summary>前に戻るボタンを押したときのアクション</summary>
    public Action OnClickPreviousButtonAction = delegate { };
    /// <summary>次に進むボタンを押したときのアクション</summary>
    public Action OnClickNextButtonAction = delegate { };
    /// <summary>決定ボタンを押したときのアクション</summary>
    public Action OnClickDecisionButtonAction = delegate { };

    /// <summary>現在の表示位置</summary>
    public int m_CurrentIndex = 0;

    private static Action finishSelectHero = delegate { };

    [SerializeField]
    public ScrollRect m_ScrollRect;

    /// <summary>拡縮するビューのRect</summary>
    RectTransform m_ContentRect;

    public CarouselRotator m_CarouselRotator = null;
    public CarouselToggler m_CarouselToggler = null;


    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_CarouselRotator = m_ScrollRect.gameObject.GetComponent<CarouselRotator>();
        m_CarouselToggler = m_ScrollRect.gameObject.GetComponent<CarouselToggler>();
        m_ContentRect = m_ScrollRect.gameObject.GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start()
    {
        SizeToFitForm();

        UnityUtil.SetObjectEnabledOnce(m_decisionButton.gameObject, false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (IsScrollMoving != m_CarouselToggler.moving)
        {
            IsScrollMoving = m_CarouselToggler.moving;
        }
    }

    void OnEnable()
    {
        SizeToFitForm();
    }

    public void SetFormDatas(List<HeroFormListContext> datas, int index)
    {
        FormDatas = datas;
        m_CurrentIndex = index;
        // ページコントロールの設定
        List<PagePointListItemContext> points = new List<PagePointListItemContext>();
        for (int i = 0; i < datas.Count; ++i)
        {
            var model = new ListItemModel((uint)i);
            PagePointListItemContext point = new PagePointListItemContext(model);
            point.IsSelect = (i == m_CurrentIndex);
            points.Add(point);

            model.OnClicked += () =>
            {
                OnClickPagePoint(point);
            };

        }
        Points = points;

        SetUpButtons();
    }

    /// <summary>
    /// リストを切り替える
    /// </summary>
    /// <param name="forward">true:進む false:戻る</param>
    public void Step(bool forward)
    {
        if (m_CarouselRotator != null && m_CarouselToggler.moving == false)
        {
            m_CarouselRotator.Step(forward);
        }
    }

    /// <summary>
    /// 任意のインデクスにリストを切り替える
    /// </summary>
    /// <param name="index"></param>
    public void ChangeForm(int index)
    {
        if (m_CarouselToggler.moving)
        {
            return;
        }

        if (index >= 0 && index < FormDataList.Count)
        {
            Toggle toggle = FormDataList[index].GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = true;
                m_CarouselToggler.CenterOnToggled();
            }
        }
    }

    /// <summary>
    /// 画面サイズに合わせて学生証のサイズを拡縮する
    /// </summary>
    public void SizeToFitForm()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        float width = rect.rect.width;
        float height = rect.rect.height;

        float contenWidth = m_ContentRect.rect.width;
        float contentHeight = m_ContentRect.rect.height;

        float aspect = height / width;
        float contentAcpect = contentHeight / contenWidth;

        float scale;
        if (aspect < contentAcpect)
        {
            scale = height / contentHeight;
        }
        else
        {
            scale = width / contenWidth;
        }

        m_ContentRect.transform.localScale = new Vector3(scale, scale, scale);

        //--------------------------------------
        // リストビューの空白幅を変更する
        //--------------------------------------
        float space = width - (contenWidth * scale);
        space = (space / scale) * 0.5f;
        HorizontalLayoutGroup horizon = m_ContentRect.GetComponentInChildren<HorizontalLayoutGroup>();
        horizon.spacing = space;
    }

    /// <summary>
    /// 主人公リストの設定
    /// </summary>
    static public List<HeroFormListContext> CreateFormDatas(Action<HeroFormListContext> _OnClickFaceImageAction,
                                                            Action<List<HeroFormListContext>> finishLoadAssetBundleAction,
                                                            Action _finishSelectHero)
    {
        List<HeroFormListContext> datas = new List<HeroFormListContext>();
        for (int i = 0; i < 6; i++)
        {
            HeroFormListContext data = new HeroFormListContext();
            data.OnClickFaceImageAction = _OnClickFaceImageAction;
            datas.Add(data);
        }

        finishSelectHero = _finishSelectHero;

        //-----------------------------------------------------------
        // 職種
        //-----------------------------------------------------------
        datas[0].Job = HeroFormListContext.JobType.STUDENT;
        datas[1].Job = HeroFormListContext.JobType.STUDENT;
        datas[2].Job = HeroFormListContext.JobType.STUDENT;
        datas[3].Job = HeroFormListContext.JobType.STUDENT;
        datas[4].Job = HeroFormListContext.JobType.TEACHER;
        datas[5].Job = HeroFormListContext.JobType.STUDENT;

        //-----------------------------------------------------------
        // アセットバンドルの読み込み
        //-----------------------------------------------------------
        // インジケーターを表示
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.ASSETBUNDLE);
        }

        AssetBundlerMultiplier multiplier = AssetBundlerMultiplier.Create();
        multiplier.Add(AssetBundler.Create().Set("hero_0001", "hero_0001", (o) =>
        {
            Sprite[] sprite = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("hero_0001");
            datas[0].FaceImage = sprite[(int)Hero_0001_Sprite_Tag.Face];
            datas[0].NameImage = sprite[(int)Hero_0001_Sprite_Tag.Name];
            datas[0].BirthdayImage = sprite[(int)Hero_0001_Sprite_Tag.Birthday];
            datas[0].NumberImage = sprite[(int)Hero_0001_Sprite_Tag.No];
            Texture maskTextue = o.GetTexture("hero_0001_mask", TextureWrapMode.Clamp);
            datas[0].NameImage_mask = maskTextue;
            datas[0].BirthdayImage_mask = maskTextue;
            datas[0].NumberImage_mask = maskTextue;
            datas[0].HeroIndex = 1;
        }));
        multiplier.Add(AssetBundler.Create().Set("hero_0002", "hero_0002", (o) =>
        {
            Sprite[] sprite = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("hero_0002");
            datas[1].FaceImage = sprite[(int)Hero_0002_Sprite_Tag.Face];
            datas[1].NameImage = sprite[(int)Hero_0002_Sprite_Tag.Name];
            datas[1].BirthdayImage = sprite[(int)Hero_0002_Sprite_Tag.Birthday];
            datas[1].NumberImage = sprite[(int)Hero_0002_Sprite_Tag.No];
            Texture maskTextue = o.GetTexture("hero_0002_mask", TextureWrapMode.Clamp);
            datas[1].NameImage_mask = maskTextue;
            datas[1].BirthdayImage_mask = maskTextue;
            datas[1].NumberImage_mask = maskTextue;
            datas[1].HeroIndex = 2;
        }));
        multiplier.Add(AssetBundler.Create().Set("hero_0003", "hero_0003", (o) =>
        {
            Sprite[] sprite = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("hero_0003");
            datas[2].FaceImage = sprite[(int)Hero_0003_Sprite_Tag.Face];
            datas[2].NameImage = sprite[(int)Hero_0003_Sprite_Tag.Name];
            datas[2].BirthdayImage = sprite[(int)Hero_0003_Sprite_Tag.Birthday];
            datas[2].NumberImage = sprite[(int)Hero_0003_Sprite_Tag.No];
            Texture maskTextue = o.GetTexture("hero_0003_mask", TextureWrapMode.Clamp);
            datas[2].NameImage_mask = maskTextue;
            datas[2].BirthdayImage_mask = maskTextue;
            datas[2].NumberImage_mask = maskTextue;
            datas[2].HeroIndex = 3;
        }));
        multiplier.Add(AssetBundler.Create().Set("hero_0004", "hero_0004", (o) =>
        {
            Sprite[] sprite = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("hero_0004");
            datas[3].FaceImage = sprite[(int)Hero_0004_Sprite_Tag.Face];
            datas[3].NameImage = sprite[(int)Hero_0004_Sprite_Tag.Name];
            datas[3].BirthdayImage = sprite[(int)Hero_0004_Sprite_Tag.Birthday];
            datas[3].NumberImage = sprite[(int)Hero_0004_Sprite_Tag.No];
            Texture maskTextue = o.GetTexture("hero_0004_mask", TextureWrapMode.Clamp);
            datas[3].NameImage_mask = maskTextue;
            datas[3].BirthdayImage_mask = maskTextue;
            datas[3].NumberImage_mask = maskTextue;
            datas[3].HeroIndex = 4;
        }));
        multiplier.Add(AssetBundler.Create().Set("hero_0005", "hero_0005", (o) =>
        {
            Sprite[] sprite = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("hero_0005");
            datas[4].FaceImage = sprite[(int)Hero_0005_Sprite_Tag.Face];
            datas[4].NameImage = sprite[(int)Hero_0005_Sprite_Tag.Name];
            datas[4].BirthdayImage = sprite[(int)Hero_0005_Sprite_Tag.Birthday];
            datas[4].NumberImage = sprite[(int)Hero_0005_Sprite_Tag.No];
            Texture maskTextue = o.GetTexture("hero_0005_mask", TextureWrapMode.Clamp);
            datas[4].NameImage_mask = maskTextue;
            datas[4].BirthdayImage_mask = maskTextue;
            datas[4].NumberImage_mask = maskTextue;
            datas[4].HeroIndex = 5;
        }));
        multiplier.Add(AssetBundler.Create().Set("hero_0006", "hero_0006", (o) =>
        {
            Sprite[] sprite = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("hero_0006");
            datas[5].FaceImage = sprite[(int)Hero_0006_Sprite_Tag.Face];
            datas[5].NameImage = sprite[(int)Hero_0006_Sprite_Tag.Name];
            datas[5].BirthdayImage = sprite[(int)Hero_0006_Sprite_Tag.Birthday];
            datas[5].NumberImage = sprite[(int)Hero_0006_Sprite_Tag.No];
            Texture maskTextue = o.GetTexture("hero_0006_mask", TextureWrapMode.Clamp);
            datas[5].NameImage_mask = maskTextue;
            datas[5].BirthdayImage_mask = maskTextue;
            datas[5].NumberImage_mask = maskTextue;
            datas[5].HeroIndex = 6;
        }));
        multiplier.Load(() =>
        {
            if (finishLoadAssetBundleAction != null)
            {
                finishLoadAssetBundleAction(datas);
            }

            // インジケーターを閉じる
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
            }
        },
        () =>
        {
            // インジケーターを閉じる
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
            }
        });

        //-----------------------------------------------------------
        // 学年
        //-----------------------------------------------------------
        datas[0].SchoolYearImage = ResourceManager.Instance.Load("gakunen_2");
        datas[1].SchoolYearImage = ResourceManager.Instance.Load("gakunen_2");
        datas[2].SchoolYearImage = ResourceManager.Instance.Load("gakunen_3");
        datas[3].SchoolYearImage = ResourceManager.Instance.Load("gakunen_1");
        //datas[4].SchoolYearImage = ResourceManager.Instance.Load("");
        datas[5].SchoolYearImage = ResourceManager.Instance.Load("gakunen_3");

        return datas;
    }

    /// <summary>
    /// 矢印ボタンの表示・非表示状態を変更する
    /// </summary>
    void ChangeArrowButtonActive()
    {
        int index = -1;
        for (int i = 0; i < FormDataList.Count; i++)
        {
            Toggle toggle = FormDataList[i].GetComponentInChildren<Toggle>();
            if (toggle == null) { continue; }

            if (toggle.isOn == true)
            {
                index = i;
                break;
            }
        }

        if (FormDataList.Count <= 1)
        {
            // リストが1個以下のとき
            IsActiveNextButton = false;
            IsActivePrevButton = false;
        }
        else if (index == 0)
        {
            // リストが先頭の場合
            IsActiveNextButton = true;
            IsActivePrevButton = false;
        }
        else if (index == FormDataList.Count - 1)
        {
            // リストが最後尾の場合
            IsActiveNextButton = false;
            IsActivePrevButton = true;
        }
        else
        {
            // その他の場合
            IsActiveNextButton = true;
            IsActivePrevButton = true;
        }
    }

    /// <summary>
    /// ページが切り替わったときに呼ばれる
    /// </summary>
    /// <param name="form"></param>
    public void OnChangedForm(bool isOn)
    {
        if (isOn == true)
        {

#if BUILD_TYPE_DEBUG
            Debug.Log("OnChangedForm");
#endif

            // ページコントロールの選択位置を変える
            for (int i = 0; i < FormDatas.Count; ++i)
            {
                if (FormDatas[i].Toggle == null)
                {
                    continue;
                }
                if (FormDatas[i].Toggle.isOn == true)
                {
                    if (i != m_CurrentIndex)
                    {
                        m_CurrentIndex = i;

                        for (int j = 0; j < Points.Count; ++j)
                        {
                            Points[j].IsSelect = (j == i);
                        }
                    }

                    break;
                }
            }

            ChangeArrowButtonActive();
        }
    }

    /// <summary>
    /// 前に戻るボタンが押された
    /// </summary>
    public void OnClickPreviousButton()
    {
        if (OnClickPreviousButtonAction != null)
        {
            OnClickPreviousButtonAction();
        }
    }

    /// <summary>
    /// 次に進むボタンを押された
    /// </summary>
    public void OnClickNextButton()
    {
        if (OnClickNextButtonAction != null)
        {
            OnClickNextButtonAction();
        }
    }

    /// <summary>
    /// 決定ボタンが押された
    /// </summary>
    public void OnClickDecisionButton()
    {
        if (OnClickDecisionButtonAction != null)
        {
            OnClickDecisionButtonAction();
        }
    }

    /// <summary>
    /// ページボタンが押された
    /// </summary>
    /// <param name="point"></param>
    void OnClickPagePoint(PagePointListItemContext point)
    {
        int index = Points.IndexOf(point);
        if (index != m_CurrentIndex)
        {
            ChangeForm(index);
        }
    }

    /// <summary>
    /// CollectionBindingのデータが変更されたとき
    /// </summary>
    void OnChangedFormDataList()
    {
        if (FormDataList.Count > 0)
        {
            //ScrollContentが機能した後に実行する
            StartCoroutine(DelayScrollContent(() =>
            {
                // リストの表示位置設定
                if (m_CurrentIndex >= 0 && m_CurrentIndex < FormDataList.Count)
                {
                    ChangeForm(m_CurrentIndex);
                }
                else
                {
                    m_CurrentIndex = 0;
                    ChangeForm(0);
                }
            }));
        }
    }

    /// <summary>
    /// ScrollRect.contentが機能した後に実行する
    /// </summary>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayScrollContent(Action action)
    {
        ScrollRect scrollRect = m_CarouselToggler.GetComponentInChildren<ScrollRect>();

        while (scrollRect.content.rect.width == 0 || FormDatas.Any(q => q.Toggle == null))
        {
            yield return null;
        }

        action();

        while (m_CarouselToggler.moving == true)
        {
            yield return null;
        }

        if (finishSelectHero != null)
        {
            finishSelectHero();
        }
    }

    private void SetUpButtons()
    {
        if (FormDatas.Count <= 1)
        {
            return;
        }

        if (m_previousButtonView != null)
        {
            m_previousButtonView.Detach();
        }

        if (m_nextButtonView != null)
        {
            m_nextButtonView.Detach();
        }

        m_previousButton = new ButtonModel();
        m_nextButton = new ButtonModel();
        m_previousButtonView = ButtonView.Attach<ButtonView>(PreviousButtonPrefabPath, m_previousButtonRoot);
        m_previousButtonView.SetModel<ButtonModel>(m_previousButton);
        m_nextButtonView = ButtonView.Attach<ButtonView>(NextButtonPrefabPath, m_nextButtonRoot);
        m_nextButtonView.SetModel<ButtonModel>(m_nextButton);

        m_previousButton.OnClicked += () =>
        {
            OnClickPreviousButton();
        };
        m_nextButton.OnClicked += () =>
        {
            OnClickNextButton();
        };

        UnityUtil.SetObjectEnabledOnce(m_decisionButton.gameObject, true);

        // TODO : 演出を入れるならその場所に移動
        m_previousButton.Appear();
        m_previousButton.SkipAppearing();
        m_nextButton.Appear();
        m_nextButton.SkipAppearing();
    }

    public void OnChangedSnapCarousel(bool isChange)
    {
        if (isChange == true)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
        }
    }
}
