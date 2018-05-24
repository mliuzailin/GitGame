using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using DG.Tweening;
using System;

public class StepUpScratchDialog : MenuPartsBase
{
    private static readonly float FadeShowAlpha = 0.5f;
    private static readonly float FadeHideAlpha = 0.0f;
    private static readonly float AnimationTime = 0.25f;

    [SerializeField]
    MenuPartsBase m_Window = null;
    [SerializeField]
    GameObject m_ShadowPanel = null;
    [SerializeField]
    ButtonView m_CloseButton = null;
    [SerializeField]
    Canvas m_Canvas = null;
    [SerializeField]
    ScrollRect m_ScrollRect = null;

    M4uProperty<List<StepUpDetailListContext>> records = new M4uProperty<List<StepUpDetailListContext>>(new List<StepUpDetailListContext>());
    public List<StepUpDetailListContext> Records { get { return records.Value; } set { records.Value = value; } }

    M4uProperty<string> titleText = new M4uProperty<string>();
    public string TitleText { get { return titleText.Value; } set { titleText.Value = value; } }

    M4uProperty<string> closeText = new M4uProperty<string>();
    public string CloseText { get { return closeText.Value; } set { closeText.Value = value; } }

    public RectTransform m_WindowRect;

    private bool m_Ready = false;
    private bool m_Show = false;
    private int m_LastUpdateCount = 0;

    private Action m_HideAction = null;

    uint m_GachaId;

    MasterDataGacha m_GachaMaster;
    List<MasterDataStepUpGachaManage> m_StepManageList;

    public static StepUpScratchDialog Create()
    {
        GameObject _tmpObj = Resources.Load("Prefab/Scratch/StepUpScratchDialog") as GameObject;
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

        return _newObj.GetComponent<StepUpScratchDialog>();
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを登録
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnClose);
        }

        m_WindowRect = m_Window.GetComponent<RectTransform>();
        m_Window.SetPosition(new Vector2(m_WindowRect.rect.width, m_WindowRect.anchoredPosition.y));
        m_Window.transform.localScale = new Vector3(0, 0, 0);
        m_ScrollRect.transform.localScale = new Vector3(0, 0, 0);
        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
        TitleText = GameTextUtil.GetText("gacha_info_title1");
        CloseText = GameTextUtil.GetText("common_button1");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (m_LastUpdateCount != 0)
        {
            m_LastUpdateCount--;
            if (m_LastUpdateCount < 0)
            {
                m_LastUpdateCount = 0;
            }
            updateLayout();
        }
    }

    void SetUpButtons()
    {
        var closeButtonModel = new ButtonModel();
        m_CloseButton.SetModel(closeButtonModel);

        closeButtonModel.OnClicked += () =>
        {
            OnClose();
        };

        closeButtonModel.Appear();
        closeButtonModel.SkipAppearing();
    }

    public void SetGachaID(uint fix_id)
    {
        m_GachaId = fix_id;
    }

    void setup()
    {
        Records.Clear();

        string detailtext = "";
        m_GachaMaster = MasterFinder<MasterDataGacha>.Instance.Find((int)m_GachaId);
        MasterDataStepUpGacha _stepUpGachaMaster = MasterDataUtil.GetMasterDataStepUpGachaFromGachaID(m_GachaMaster.fix_id);
        if (_stepUpGachaMaster != null)
        {
            detailtext = MasterDataUtil.GetGachaText(EMASTERDATA_SERVER.STEP_UP_GACHA,
                                                    _stepUpGachaMaster.fix_id,
                                                    MasterDataDefineLabel.GachaTextRefType.DETAIL);
        }

        // TOP項目の作成
        StepUpDetailListContext top = new StepUpDetailListContext();
        top.Banner_url = m_GachaMaster.url_img;
        top.DetailText = detailtext;
        top.IsStep = false;
        top.FinishLoadImageAction = () =>
        {
            m_LastUpdateCount = 5;
        };
        Records.Add(top);

        m_StepManageList = MasterFinder<MasterDataStepUpGachaManage>.Instance.SelectWhere("where gacha_id = ? ORDER BY step_num ASC", m_GachaId);

        // ステップ項目の作成
        if (m_StepManageList != null)
        {
            for (int i = 0; i < m_StepManageList.Count; ++i)
            {
                MasterDataStepUpGachaManage stepManage = m_StepManageList[i];

                detailtext = MasterDataUtil.GetGachaText(EMASTERDATA_SERVER.STEP_UP_GACHA_MANAGE,
                                                        stepManage.fix_id,
                                                        MasterDataDefineLabel.GachaTextRefType.DETAIL);

                StepUpDetailListContext item = new StepUpDetailListContext();
                item.StepManageID = stepManage.fix_id;
                item.Banner_url = stepManage.url_img;
                item.DetailText = detailtext;
                item.LotExecText = GameTextUtil.GetText("Gacha_LottNum") + stepManage.total_lot_exec.ToString();
                item.IsViewBornusLabel = (stepManage.present_enable == MasterDataDefineLabel.BoolType.ENABLE);
                item.IsViewLineUpNormal = (stepManage.normal1_assign_id > 0);
                item.IsViewLineUpRainbow = (stepManage.special_assign_id > 0);

                // ステップ数の表示
                if (stepManage.step_num == 0)
                {
                    // 初回ステップ
                    item.TitleText = GameTextUtil.GetText("Gacha_step_04");
                }
                else
                {
                    item.TitleText = string.Format(GameTextUtil.GetText("Gacha_StepNum"), stepManage.step_num);
                }

                // 値段の表示
                string priceText = GameTextUtil.GetText("Price_Chip");
                priceText += stepManage.price;

                // 有料チップの場合
                if (_stepUpGachaMaster.paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                {
                    priceText += GameTextUtil.GetText("Gacha_step_03");
                }
                item.PriceText = priceText;

                // おまけの表示
                MasterDataPresent[] presentArray = MasterDataUtil.GetPresentMasterFromGroupID(stepManage.present_group_id);
                string present_message = "";
                if (presentArray != null)
                {
                    for (int present_count = 0; present_count < presentArray.Length; ++present_count)
                    {
                        if (present_count > 0)
                        {
                            present_message += "\n";
                        }

                        string presentName = MasterDataUtil.GetPresentName(presentArray[present_count]);
                        int presentCount = MasterDataUtil.GetPresentCount(presentArray[present_count]);

                        present_message += presentName + " ×" + presentCount;
                    }
                }
                item.BonusText = string.Format(GameTextUtil.GetText("Gacha_step_01"), present_message);

                //
                item.LinUpNormalButtonModel.OnClicked += () =>
                {
                    OpenLineUp(stepManage, stepManage.normal1_assign_id);
                };
                item.LinUpRainbowButtonModel.OnClicked += () =>
                {
                    OpenLineUp(stepManage, stepManage.special_assign_id);
                };
                item.FinishLoadImageAction = () =>
                {
                    m_LastUpdateCount = 5;
                };
                Records.Add(item);
            }
        }
    }

    public void Show(Action hideAction = null)
    {
        if (m_Show)
        {
            return;
        }
        m_Show = true;

        m_HideAction = hideAction;

        m_Window.SetPositionAjustStatusBar(new Vector2(0, 0));
        m_Window.transform.localScale = new Vector3(1, 0, 1);

        m_ShadowPanel.GetComponent<Image>().DOFade(FadeShowAlpha, AnimationTime);

        m_Window.transform.DOScaleY(1, AnimationTime).OnComplete(() =>
        {
            setup();
            StartCoroutine(WaitScrollContent(() =>
            {
                m_ScrollRect.transform.localScale = new Vector3(1, 1, 1);
                m_Ready = true;

            }));
        });
    }

    IEnumerator WaitScrollContent(Action finishAction)
    {
        while (m_ScrollRect.content.rect.height == 0)
        {
            yield return null;
        }

        if (finishAction != null)
        {
            finishAction();
        }
    }

    public void Hide()
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

        m_ShadowPanel.GetComponent<Image>().DOFade(FadeHideAlpha, AnimationTime);

        m_Window.transform.DOScaleY(0f, AnimationTime).OnComplete(() =>
        {
            if (m_HideAction != null)
            {
                m_HideAction();
            }
            DestroyObject(gameObject);
        });
    }

    void OpenLineUp(MasterDataStepUpGachaManage stepupgachamanage, uint assign_id)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        m_Canvas.enabled = false;

        string title = GameTextUtil.GetText("gacha_lineup_title1");
        if (stepupgachamanage.special_assign_id != 0 &&
            stepupgachamanage.special_assign_id == assign_id)
        {
            title = GameTextUtil.GetText("gacha_lineup_title2");
        }

        // タイトルにステップ数追加
        if (stepupgachamanage.step_num == 0)
        {
            // 初回ステップ
            title += GameTextUtil.GetText("gacha_lineup_step1");
        }
        else
        {
            title += string.Format(GameTextUtil.GetText("gacha_lineup_step2"), stepupgachamanage.step_num);
        }

        ScratchLineUpDialog lineupdialog = ScratchLineUpDialog.Create();
        lineupdialog.SetCamera(SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>());
        lineupdialog.setup(m_GachaMaster, stepupgachamanage, assign_id, true, title);
        lineupdialog.m_CloseAction = () =>
        {
            m_Canvas.enabled = true;
        };
    }

    public void OnClose()
    {
        if (m_Ready == false || m_Show == false)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        Hide();
    }

    void OnChangedRecords()
    {
        m_LastUpdateCount = 5;
    }
}
