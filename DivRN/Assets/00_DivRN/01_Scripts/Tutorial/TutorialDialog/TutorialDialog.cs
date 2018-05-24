/**
 *  @file   TutorialDialog.cs
 *  @brief
 *  @author Developer
 *  @date   2017/03/01
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using M4u;
using AsPerSpec;

public static class TutorialPartExtension
{
    public static TutorialDialog.FLAG_TYPE ConvertToTutorialFlagType(this TutorialPart part, int no = 1)
    {
        TutorialDialog.FLAG_TYPE flagType = TutorialDialog.FLAG_TYPE.NONE;
        switch (part)
        {
            case TutorialPart.EDIT:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_PARTY_SELECT;
                break;
            case TutorialPart.NORMAL02:
                flagType = TutorialDialog.FLAG_TYPE.HOW_TO_PLAY;
                break;
            case TutorialPart.BUILDUP:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_PARTY_BUILDUP;
                break;
        }
        return flagType;
    }
}

public static class MainMenuSeqExtension
{
    /// <summary>
    /// MainMenuqのページ切り替え時に表示する機能チュートリアルの種類を取得する
    /// </summary>
    public static TutorialDialog.FLAG_TYPE ConvertToTutorialFlagType(this MAINMENU_SEQ seq)
    {
        TutorialDialog.FLAG_TYPE flagType = TutorialDialog.FLAG_TYPE.NONE;

        switch (seq)
        {
            case MAINMENU_SEQ.SEQ_HOME_MENU:
                flagType = TutorialDialog.FLAG_TYPE.HOME_MENU;
                break;
            case MAINMENU_SEQ.SEQ_FRIEND_LIST:
                flagType = TutorialDialog.FLAG_TYPE.FRIEND_LIST;
                break;
            case MAINMENU_SEQ.SEQ_HERO_FORM:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_HERO;
                break;
            case MAINMENU_SEQ.SEQ_UNIT_PARTY_SELECT:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_PARTY_SELECT;
                break;
            case MAINMENU_SEQ.SEQ_UNIT_EVOLVE:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_EVOLVE;
                break;
            case MAINMENU_SEQ.SEQ_UNIT_BUILDUP:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_PARTY_BUILDUP;
                break;
            case MAINMENU_SEQ.SEQ_UNIT_LINK:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_LINK;
                break;
            case MAINMENU_SEQ.SEQ_UNIT_SALE:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_SALE;
                break;
            case MAINMENU_SEQ.SEQ_UNIT_LIST: break;
            case MAINMENU_SEQ.SEQ_UNIT_CATALOG:
                flagType = TutorialDialog.FLAG_TYPE.UNIT_CATALOG;
                break;
            case MAINMENU_SEQ.SEQ_GACHA_MAIN:
                flagType = TutorialDialog.FLAG_TYPE.GACHA;
                break;
            case MAINMENU_SEQ.SEQ_QUEST_SELECT_ACHIEVEMENT_GP_LIST:
                flagType = TutorialDialog.FLAG_TYPE.ACHIEVEMENT_GP_LIST;
                break;
            case MAINMENU_SEQ.SEQ_CHALLENGE_SELECT:
                flagType = TutorialDialog.FLAG_TYPE.CHALLENGE;
                break;
        }
        return flagType;
    }
}

public static class GloabalMainMenuSeqExtension
{
    /// <summary>
    /// GloabalMainMenuSeqのページ切り替え時に表示する機能チュートリアルの種類を取得する
    /// </summary>
    public static TutorialDialog.FLAG_TYPE ConvertToTutorialFlagType(this GLOBALMENU_SEQ seq)
    {
        TutorialDialog.FLAG_TYPE flagType;
        switch (seq)
        {
            //case GLOBALMENU_SEQ.NONE:
            //    break;
            //case GLOBALMENU_SEQ.TOP_MENU:
            //    break;
            //case GLOBALMENU_SEQ.OPTION:
            //    break;
            case GLOBALMENU_SEQ.MISSION:
                flagType = TutorialDialog.FLAG_TYPE.ACHIEVEMENT_GP_LIST;
                break;
            //case GLOBALMENU_SEQ.PRESENT:
            //    break;
            //case GLOBALMENU_SEQ.ITEM:
            //    break;
            case GLOBALMENU_SEQ.EVENTSCHEDULE:
                flagType = TutorialDialog.FLAG_TYPE.EVENT_SCHEDULE_LIST;
                break;
            //case GLOBALMENU_SEQ.MAX:
            //    break;
            default:
                flagType = TutorialDialog.FLAG_TYPE.NONE;
                break;
        }

        return flagType;
    }
}

public class TutorialDialog : M4uContextMonoBehaviour
{
    public enum FLAG_TYPE
    {
        NONE = 0,
        UNIT_PARTY_SELECT = 1,  //!< 編成
        UNIT_PARTY_BUILDUP = 2,  //!< 強化
        UNIT_HERO = 3,  //!< マスター
        HOME_MENU = 4,  //!< MENU
        GACHA = 5,  //!< スクラッチ
        UNIT_EVOLVE = 6,  //!< 進化
        UNIT_LINK = 7,  //!< リンク
        UNIT_SALE = 8,  //!< 売却
        UNIT_CATALOG = 9,  //!< 図鑑
        EVENT_SCHEDULE_LIST = 10, //!< イベントスケジュール
        FRIEND_LIST = 11, //!< フレンド
        ACHIEVEMENT_GP_LIST = 12, //!< ミッション
        UNIT_REBIRTH_EVOLVE = 13, //!< 再生進化
        UNIT_LIMIT_OVER = 14, //!< 限界突破
        UNIT_STATUS = 15, //!< ステータス
        ELEMENT_DAMAGE = 16, //!< 属性相性
        GUERRILLA_BOSS = 17, //!< ゲリラボス
        KEY_QUEST = 18, //!< キークエスト
        BATTLE = 19, //!< バトル画面 バトル１からバトル４に分割されたので未使用
        LEGACY_QUEST = 20, //!< 旧ディバインゲート
        HOW_TO_PLAY = 21, //!< 遊び方まとめ
        SCORE = 22, //!< スコア
        BATTLE1 = 23, //!< バトル１
        BATTLE2 = 24, //!< バトル２
        BATTLE3 = 25, //!< バトル３
        BATTLE4 = 26, //!< バトル4
        // BATTLE5 = 27, //!< バトル5
        CHALLENGE = 28, //!< 魔影回廊
        AUTO_PLAY = 29, //!< オートプレイ
        ALL,
    }

    [SerializeField]
    private GameObject m_previousButtonRoot;
    [SerializeField]
    private GameObject m_nextButtonRoot;

    private static readonly string PreviousButtonPrefabPath = "Prefab/HeroSelect/HeroSelectPreviousButton";
    private static readonly string NextButtonPrefabPath = "Prefab/HeroSelect/HeroSelectNextButton";


    private ButtonView m_previousButtonView = null;
    private ButtonView m_nextButtonView = null;
    private ButtonModel m_previousButton = null;
    private ButtonModel m_nextButton = null;

    private FLAG_TYPE type;
    M4uProperty<List<TutorialDialogListContext>> carousels = new M4uProperty<List<TutorialDialogListContext>>(new List<TutorialDialogListContext>());

    /// <summary>カルーセルリスト</summary>
    List<TutorialDialogListContext> Carousels
    {
        get
        {
            return carousels.Value;
        }
        set
        {
            carousels.Value = value;
        }
    }

    M4uProperty<List<TutorialDialogPointContext>> points = new M4uProperty<List<TutorialDialogPointContext>>(new List<TutorialDialogPointContext>());

    /// <summary>ページの●</summary>
    List<TutorialDialogPointContext> Points
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

    M4uProperty<bool> isActiveCloseButton = new M4uProperty<bool>();

    public bool IsActiveCloseButton
    {
        get
        {
            return isActiveCloseButton.Value;
        }
        set
        {
            isActiveCloseButton.Value = value;
        }
    }

    M4uProperty<bool> isScrollMoving = new M4uProperty<bool>();
    public bool IsScrollMoving { get { return isScrollMoving.Value; } set { isScrollMoving.Value = value; } }

    List<GameObject> carouselsList = new List<GameObject>();
    public List<GameObject> CarouselsList { get { return carouselsList; } set { carouselsList = value; } }

    M4uProperty<bool> isActivePrevButton = new M4uProperty<bool>();
    /// <summary>矢印ボタンの表示・非表示</summary>
    bool IsActivePrevButton { get { return isActivePrevButton.Value; } set { isActivePrevButton.Value = value; } }

    M4uProperty<bool> isActiveNextButton = new M4uProperty<bool>();
    /// <summary>矢印ボタンの表示・非表示</summary>
    bool IsActiveNextButton { get { return isActiveNextButton.Value; } set { isActiveNextButton.Value = value; } }

    public CarouselRotator m_CarouselRotator = null;
    public CarouselToggler m_CarouselToggler = null;


    private CanvasSetting m_CanvasSetting;

    /// <summary>イベントが終わったときのアクション</summary>
    private Action m_CloseAction;

    private int m_CurrentIndex = 0;


    /// <summary>表示中のイベントの数</summary>
    public static int CarouselCounter = 0;


    private int CarouselID = -1;

    private bool m_IsCloseSE = true;
    private bool m_IsSave = true;

    public bool IsLoading
    {
        get;
        private set;
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_CanvasSetting = GetComponentInChildren<CanvasSetting>();
        m_CarouselRotator = GetComponentInChildren<CarouselRotator>();
        m_CarouselToggler = GetComponentInChildren<CarouselToggler>();

        AndroidBackKeyManager.Instance.StackPush(gameObject, OnClickCloseButton);
    }

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
        // セルサイズをリストビューのサイズに合わせる
        RectTransform rect = m_CarouselToggler.GetComponent<RectTransform>();
        GridLayoutGroup layout = m_CarouselToggler.GetComponentInChildren<GridLayoutGroup>();
        layout.cellSize = new Vector2(rect.rect.width, rect.rect.height);
    }

    /// <summary>
    /// オブジェクトの作成
    /// </summary>
    /// <returns></returns>
    public static TutorialDialog Create()
    {
        int carouselCcount = GetCarousel().Length;

        GameObject _tmpObj = Resources.Load("Prefab/TutorialDialog/TutorialDialog") as GameObject;
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

        TutorialDialog carousel = _newObj.GetComponent<TutorialDialog>();
        if (carousel == null)
        {
            return null;
        }

        carousel.CarouselID = CarouselCounter;
        if (carouselCcount != 0)
        {
            carousel.m_CanvasSetting.ChangeSortingOrder(carouselCcount);
        }

        CarouselCounter++;

        return carousel;
    }

    public static TutorialDialog[] GetCarousel()
    {
        GameObject[] carouselArray = GameObject.FindGameObjectsWithTag("TutorialDialog");
        TutorialDialog[] _ret = new TutorialDialog[carouselArray.Length];
        for (int i = 0; i < carouselArray.Length; i++)
        {
            _ret[i] = carouselArray[i].GetComponent<TutorialDialog>();
        }
        return _ret;
    }

    public static void HideAll()
    {
        TutorialDialog[] carouselArray = GetCarousel();
        foreach (TutorialDialog carousel in carouselArray)
        {
            if (carousel != null)
                carousel.Hide();
        }
    }

    public void Hide()
    {
        if (m_IsSave)
        {
            LocalSaveManagerRN.Instance.SetIsShowTutorialDialog(type, true);
            LocalSaveManagerRN.Instance.Save();
        }

        if (m_IsCloseSE)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }

        if (m_CloseAction != null)
        {
            m_CloseAction();
            m_CloseAction = null;
        }

        AndroidBackKeyManager.Instance.StackPop(gameObject);
        DestroyImmediate(gameObject);
    }


    public void SetSprits(Sprite[] sprits)
    {
        Carousels.Clear();
        Points.Clear();

        for (int i = 0; i < sprits.Length; ++i)
        {
            TutorialDialogListContext carousel = new TutorialDialogListContext();
            TutorialDialogPointContext point = new TutorialDialogPointContext();

            carousel.Image = sprits[i];
            Carousels.Add(carousel);

            point.IsSelect = (i == 0);
            Points.Add(point);
        }
    }

    public TutorialDialog SetTutorialType(FLAG_TYPE _type)
    {
        this.type = _type;
        SetLoading(true);

        AssetBundler.Create().
            Set(string.Format("tutorial_{0:D4}", (int)type),
            (o) =>
            {
                Sprite[] sprits = o.GetAssetAll<Sprite>();
                if (sprits != null && sprits.Length > 0)
                {
                    SetSprits(sprits);
                }
                else
                {
                    Error();
                }
                SetLoading(false);
                SetUpButtons();
            },
            (s) =>
            {
                SetLoading(false);
                Error();
            }).
            Load();

        return this;
    }

    private void SetLoading(bool b)
    {
        IsLoading = b;
        IsActiveCloseButton = !b;

        if (b)
        {
            // インジケーターを表示
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.ASSETBUNDLE);
        }
        else
        {
            // インジケーターを閉じる
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
        }
    }

    private void Error()
    {
        Hide();
    }

    /// <summary>
    /// ページが切り替わったときに呼ばれる
    /// </summary>
    /// <param name="form"></param>
    public void OnChangedCarousel(bool isOn)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnChangedCarousel:" + isOn);
#endif
        if (!isOn)
        {
            return;
        }

        // ページコントロールの選択位置を変える
        for (int i = 0; i < Points.Count; ++i)
        {
            if (Carousels[i].Toggle.isOn == true)
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

    public void Show()
    {
        Show(null);
    }

    public void Show(Action closeAction)
    {
        m_CloseAction = closeAction;
        if (type == FLAG_TYPE.NONE)
        {
            Hide();
        }
    }

    public void OnClickCloseButton()
    {
        Hide();
    }

    public void OnChangedSnapCarousel(bool isChange)
    {
        if (isChange == true)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
        }
    }

    /// <summary>
    /// 非表示時のサウンドをしないようにする
    /// </summary>
    /// <returns></returns>
    public TutorialDialog DisableCloseSE()
    {
        m_IsCloseSE = false;
        return this;
    }

    /// <summary>
    /// 表示フラグを保存しないようにする
    /// </summary>
    /// <returns></returns>
    public TutorialDialog DisableSaveShowFlag()
    {
        this.m_IsSave = false;
        return this;
    }

    /// <summary>
    /// 前に戻るボタンが押された
    /// </summary>
    public void OnClickPreviousButton()
    {
        Step(false);
    }

    /// <summary>
    /// 次に進むボタンを押された
    /// </summary>
    public void OnClickNextButton()
    {
        Step(true);
    }

    private void SetUpButtons()
    {
        if (Carousels.Count <= 1)
            return;

        if (m_previousButtonView != null)
            m_previousButtonView.Detach();
        if (m_nextButtonView != null)
            m_nextButtonView.Detach();

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

        // TODO : 演出を入れるならその場所に移動
        m_previousButton.Appear();
        m_previousButton.SkipAppearing();
        m_nextButton.Appear();
        m_nextButton.SkipAppearing();
    }

    /// <summary>
    /// リストを切り替える
    /// </summary>
    /// <param name="forward">true:進む false:戻る</param>
    private void Step(bool forward)
    {
        if (m_CarouselRotator != null && m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

            m_CarouselRotator.Step(forward);
        }
    }

    /// <summary>
    /// 矢印ボタンの表示・非表示状態を変更する
    /// </summary>
    void ChangeArrowButtonActive()
    {
        int index = -1;
        for (int i = 0; i < CarouselsList.Count; i++)
        {
            Toggle toggle = CarouselsList[i].GetComponentInChildren<Toggle>();
            if (toggle == null) { continue; }

            if (toggle.isOn == true)
            {
                index = i;
                break;
            }
        }

        if (CarouselsList.Count <= 1)
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
        else if (index == CarouselsList.Count - 1)
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
    /// CollectionBindingのデータが変更されたとき
    /// </summary>
    void OnChangedCarouselsList()
    {
        if (CarouselsList.Count > 0)
        {
            //ScrollContentが機能した後に実行する
            StartCoroutine(DelayScrollContent(() =>
            {
                Toggle toggle = CarouselsList[0].GetComponentInChildren<Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = true;
                    m_CarouselToggler.CenterOnToggled();
                }
                ChangeArrowButtonActive();
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

        while (scrollRect.content.rect.width == 0 || Carousels.Any(q => q.Toggle == null))
        {
            yield return null;
        }

        action();
    }

}
