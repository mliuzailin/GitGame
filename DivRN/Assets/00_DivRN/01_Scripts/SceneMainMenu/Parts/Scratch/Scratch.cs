using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;
using AsPerSpec;

public class Scratch : MenuPartsBase
{
    [SerializeField]
    private GameObject m_previousButtonRoot;
    [SerializeField]
    private GameObject m_nextButtonRoot;
    [SerializeField]
    public ScrollRect m_ScrollRect;

    public CarouselRotator m_CarouselRotator = null;
    public CarouselToggler m_CarouselToggler = null;

    // スクラッチ可能回数の最大値
    const int ScratchMax = 9;

    public enum ButtonId : int
    {
        Close = 0,
        LineUp_A = 10,  // ラインナップ 前半
        LineUp_B = 11,  // ラインナップ 後半
        LineUp_Rainbow = 12,  // ラインナップ 確定
        LineUp_Normal = 13,  // ラインナップ 通常
        ScratchOne = 20,  // 1回ガチャ
        ScratchMax = 21, // 最大9回ガチャ
        ScratchStepUp = 22, // ステップアップガチャ
        //GideLine = 500, // ガイドライン
        Info = 999,     // 詳細
        End
    }

    public enum LineUp : int
    {
        Normal = 0,
        A,
        B,
        Rainbow,
        Max
    }

    M4uProperty<bool> isActivePrevButton = new M4uProperty<bool>();
    /// <summary>矢印ボタンの表示・非表示</summary>
    bool IsActivePrevButton { get { return isActivePrevButton.Value; } set { isActivePrevButton.Value = value; } }

    M4uProperty<bool> isActiveNextButton = new M4uProperty<bool>();
    /// <summary>矢印ボタンの表示・非表示</summary>
    bool IsActiveNextButton { get { return isActiveNextButton.Value; } set { isActiveNextButton.Value = value; } }

    M4uProperty<List<ScratchListItemContext>> scratchDatas = new M4uProperty<List<ScratchListItemContext>>();
    public List<ScratchListItemContext> ScratchDatas { get { return scratchDatas.Value; } set { scratchDatas.Value = value; } }

    List<GameObject> scratchDataList = new List<GameObject>();
    public List<GameObject> ScratchDataList { get { return scratchDataList; } set { scratchDataList = value; } }


    // ■サーバー側、ガチャ種別定義
    // 0：　ゲーム内マネー
    // 1：　フレンドポイントガチャ
    // 2：　レアガチャ（チップ使用）
    // 3：　コラボガチャ（チップ使用）
    // 4：　チュートリアル専用ガチャ
    // 5：　一日一回ガチャ（フレンドポイント使用）
    // 8：　ガチャチケット（プレゼント送付して使う 期日、ありなし含む）
    // 9：　イベントポイント
    // 10：　ガチャチケット（新仕様チケット、個数・回数という概念がある）
    // 6：　不明
    // 7：　不明

    private MasterDataGacha m_Master = null;
    private int m_CurrentIndex;

    private static readonly string PreviousButtonPrefabPath = "Prefab/HeroSelect/HeroSelectPreviousButton";
    private static readonly string NextButtonPrefabPath = "Prefab/HeroSelect/HeroSelectNextButton";


    private ButtonView m_previousButtonView = null;
    private ButtonView m_nextButtonView = null;
    private ButtonModel m_previousButton = null;
    private ButtonModel m_nextButton = null;

    /// <summary>前に戻るボタンを押したときのアクション</summary>
    public Action OnClickPreviousButtonAction = delegate { };
    /// <summary>次に進むボタンを押したときのアクション</summary>
    public Action OnClickNextButtonAction = delegate { };

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_CarouselRotator = m_ScrollRect.gameObject.GetComponent<CarouselRotator>();
        m_CarouselToggler = m_ScrollRect.gameObject.GetComponent<CarouselToggler>();
    }

    void Start()
    {

    }

    private void Update()
    {

    }

    public void setup(MasterDataGacha _master)
    {
        m_Master = _master;
        ScratchDatas = new List<ScratchListItemContext>();
        MasterDataGacha[] gachaArray;
        if (TutorialManager.IsExists == true)
        {
            gachaArray = new MasterDataGacha[1];
            gachaArray[0] = _master;
        }
        else
        {
            gachaArray = MasterDataUtil.GetActiveGachaMaster();
        }
        m_CurrentIndex = 0;
        for (int i = 0; i < gachaArray.Length; ++i)
        {
            ScratchListItemContext scratch = new ScratchListItemContext();
            scratch.setup(gachaArray[i]);
            ScratchDatas.Add(scratch);
            if (gachaArray[i].fix_id == m_Master.fix_id)
            {
                m_CurrentIndex = i;
            }
        }
        StartCoroutine(setFirstPage());
        SetUpButtons();
    }

    IEnumerator setFirstPage()
    {
        ScrollRect scrollRect = m_CarouselToggler.GetComponentInChildren<ScrollRect>();
        while (scrollRect.content.rect.width == 0)
        {
            yield return null;
        }
        while (scratchDataList.Count == 0)
        {
            yield return null;
        }
        while (ScratchDatas.Any(q => q.Toggle == null) || ScratchDatas.Any(q => q.m_StepButton == null))
        {
            yield return null;
        }
        for (int i = 0; i < ScratchDatas.Count; ++i)
        {
            ScratchDatas[i].updateScratchButton();
        }
        ScratchDatas[m_CurrentIndex].Toggle.isOn = true;
        m_CarouselToggler.CenterOnToggled();
        while (m_CarouselToggler.moving == true)
        {
            yield return null;
        }
    }

    public void changeScratch(int index)
    {
        if (m_CarouselToggler.moving == true)
        {
            return;
        }
        ScratchDatas[index].Toggle.isOn = true;
        m_CarouselToggler.CenterOnToggled();
        m_CurrentIndex = index;
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
            Debug.Log("OnChangedScratch");
#endif

            // ページコントロールの選択位置を変える
            for (int i = 0; i < ScratchDatas.Count; ++i)
            {
                if (ScratchDatas[i].Toggle == null)
                {
                    continue;
                }
                if (ScratchDatas[i].Toggle.isOn == true)
                {
                    if (i != m_CurrentIndex)
                    {
                        m_CurrentIndex = i;
                        MainMenuManager.Instance.SubTab.changeSelectTub(m_CurrentIndex);
                    }

                    break;
                }
            }
            ChangeArrowButtonActive();
        }
    }

    private void SetUpButtons()
    {
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

        // TODO : 演出を入れるならその場所に移動
        m_previousButton.Appear();
        m_previousButton.SkipAppearing();
        m_nextButton.Appear();
        m_nextButton.SkipAppearing();
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
    /// 矢印ボタンの表示・非表示状態を変更する
    /// </summary>
    void ChangeArrowButtonActive()
    {
        if (ScratchDatas.Count <= 1)
        {
            // リストが1個以下のとき
            IsActiveNextButton = false;
            IsActivePrevButton = false;
        }
        else if (m_CurrentIndex == 0)
        {
            // リストが先頭の場合
            IsActiveNextButton = true;
            IsActivePrevButton = false;
        }
        else if (m_CurrentIndex == ScratchDatas.Count - 1)
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

    public void OnChangedSnapCarousel(bool isChange)
    {
        if (isChange == true)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
        }
    }
}
