using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using System;
using ServerDataDefine;
using DG.Tweening;

public class LoginBonusDetailPanel : MenuPartsBase
{
    private static readonly string PrefaPath = "Prefab/LoginBonus/LoginBonusDetailPanel";

    private static readonly string AppearAnimationName = "login_bonus_detail_panel_appear";
    private static readonly string LoopAnimationName = "login_bonus_detail_panel_loop";

    [SerializeField]
    GameObject m_PanelRoot;
    [SerializeField]
    GameObject m_Mask;
    [SerializeField]
    GameObject m_TopLine;
    [SerializeField]
    GameObject m_BottomLine;
    [SerializeField]
    LoginBonusCloseButton m_CloseButton;
    [SerializeField]
    public Vector2 m_MinPanelSize;
    [SerializeField]
    public float m_MaxPanelHeight = 500;


    M4uProperty<List<LoginBonusPresentListContext>> presents = new M4uProperty<List<LoginBonusPresentListContext>>(new List<LoginBonusPresentListContext>());
    public List<LoginBonusPresentListContext> Presents { get { return presents.Value; } set { presents.Value = value; } }

    M4uProperty<Sprite> countImage1 = new M4uProperty<Sprite>();
    public Sprite CountImage1 { get { return countImage1.Value; } set { countImage1.Value = value; } }

    M4uProperty<bool> isViewCountImage1 = new M4uProperty<bool>();
    public bool IsViewCountImage1 { get { return isViewCountImage1.Value; } set { isViewCountImage1.Value = value; } }

    M4uProperty<Sprite> countImage2 = new M4uProperty<Sprite>();
    public Sprite CountImage2 { get { return countImage2.Value; } set { countImage2.Value = value; } }

    M4uProperty<bool> isViewCountImage2 = new M4uProperty<bool>();
    public bool IsViewCountImage2 { get { return isViewCountImage2.Value; } set { isViewCountImage2.Value = value; } }

    M4uProperty<string> cautionText = new M4uProperty<string>();
    /// <summary>注意書き</summary>
    public string CautionText { get { return cautionText.Value; } set { cautionText.Value = value; } }

    M4uProperty<int> presentItemCount = new M4uProperty<int>();
    public int PresentItemCount { get { return presentItemCount.Value; } set { presentItemCount.Value = value; } }

    private int m_LastUpdateCount = 0;

    public Action CloseAction;

    bool isShow = false;
    bool isClosing = false;

    public static LoginBonusDetailPanel Attach(GameObject parent)
    {
        return Attach<LoginBonusDetailPanel>(PrefaPath, parent);
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_PanelRoot.transform.localScale = new Vector3(1, 0, 1);
        //RectTransform rect = GetComponent<RectTransform>();
        //m_MinPanelSize = new Vector2(rect.rect.width, rect.rect.height);
        isShow = false;
        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
        m_LastUpdateCount = 5;
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
            SetUpPanelHeight();
            updateLayout();
        }
    }

    void SetUpButtons()
    {
        var closeButtonModel = new ButtonModel();
        m_CloseButton.SetModel(closeButtonModel);

        closeButtonModel.OnClicked += () =>
        {
            OnClickClose();
        };

        closeButtonModel.Appear();
        closeButtonModel.SkipAppearing();
    }

    public void OnClickClose()
    {
        Hide();
    }

    public void SetCount(uint login_count)
    {
        IsViewCountImage1 = false;
        IsViewCountImage2 = false;

        List<uint> nums = TextUtil.GetNumberList((int)login_count);
        for (int i = 0; i < nums.Count; ++i)
        {
            Sprite sprite = ResourceManager.Instance.Load(string.Format("login_bonus_{0}", nums[i]), ResourceType.Common);

            if (i == 0)
            {
                CountImage1 = sprite;
                IsViewCountImage1 = true;
            }
            else if (i == 1)
            {
                CountImage2 = sprite;
                IsViewCountImage2 = true;
            }
        }
    }

    /// <summary>
    /// パネルの高さを設定する
    /// </summary>
    public void SetUpPanelHeight()
    {
        RectTransform rect = GetComponent<RectTransform>();
        ScrollRect scroll = GetComponentInChildren<ScrollRect>();

        if (rect == null || scroll == null)
        {
            return;
        }

        float contentHeight = scroll.content.rect.height;
        float windowHeight = m_MinPanelSize.y + contentHeight;
        if (windowHeight > m_MaxPanelHeight)
        {
            windowHeight = m_MaxPanelHeight;
        }
        rect.sizeDelta = new Vector2(m_MinPanelSize.x, windowHeight);

        float height = scroll.viewport.rect.height;
        if (contentHeight <= rect.rect.height - m_MinPanelSize.y)
        {
            // コンテンツよりスクロールエリアのほうが広いので、スクロールしなくてもすべて表示されている
            scroll.verticalNormalizedPosition = 1;
            scroll.vertical = false;
        }
        else
        {
            scroll.vertical = true;
        }
    }

    /// <summary>
    /// リストの作成
    /// </summary>
    /// <param name="periodLoginMaster"></param>
    public void SetUpList(LoginBonusRecordListContext item)
    {
        SetCount(item.date_count);
        Presents.Clear();
        for (int i = 0; i < item.present_ids.Length; ++i)
        {
            int presentID = item.present_ids[i];
            MasterDataPresent presentMaster = MasterDataUtil.GetPresentParamFromID((uint)presentID);

            LoginBonusPresentListContext present = new LoginBonusPresentListContext();
            MainMenuUtil.GetPresentIcon(presentMaster, sprite => {
                present.IconImage = sprite;
            });
            present.NameText = MasterDataUtil.GetPresentName(presentMaster);
            present.NumText = string.Format(GameTextUtil.GetText("loginbonus_amount"), MasterDataUtil.GetPresentCount(presentMaster));
            present.NumRate = GameTextUtil.GetText("loginbonus_multipled");
            Presents.Add(present);
        }

        // 先頭のアイテムの線を非表示にする
        LoginBonusPresentListContext selectItem = Presents.First();
        if (selectItem != null)
        {
            selectItem.IsViewBorder = false;
        }


        PresentItemCount = Presents.Count;
    }

    /// <summary>
    /// ダイアログを開く
    /// </summary>
    /// <param name="callback"></param>
    public void Show(Action callback = null)
    {
        if (isShow == true)
        {
            return;
        }

        isShow = true;

        m_Mask.SetActive(true);
        m_PanelRoot.transform.DOScaleY(1, 0.25f).OnComplete(() =>
        {
            m_Mask.SetActive(false);
            PlayAnimation(LoopAnimationName);
            if (callback != null)
            {
                callback();
            }

            CautionText = GameTextUtil.GetText("Login_bonus_01");
        });
    }

    public void Hide()
    {
        if (EnableClose() == false)
        {
            return;
        }

        isClosing = true;
        CautionText = "";
        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        if (CloseAction != null)
        {
            CloseAction();
        }

        m_PanelRoot.transform.DOScaleY(0f, 0.25f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    /// <summary>
    /// ダイアログを閉じられる状態かどうか
    /// </summary>
    /// <returns></returns>
    public bool EnableClose()
    {
        if (isClosing == true
            || m_Mask.IsActive() == true)
        {

            return false;
        }

        return true;
    }


    void OnChangedList()
    {
        // 線の表示順を変更
        m_TopLine.transform.SetAsFirstSibling();
        m_BottomLine.transform.SetAsLastSibling();
    }

}
