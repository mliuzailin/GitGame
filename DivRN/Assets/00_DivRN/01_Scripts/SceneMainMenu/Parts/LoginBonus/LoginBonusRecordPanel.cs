/// <summary>
///
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using System;
using TMPro;

public class LoginBonusRecordPanel : MenuPartsBase
{
    [SerializeField]
    TextMeshProUGUI m_DetailText;
    [SerializeField]
    Image m_BGImage;
    [SerializeField]
    Image m_ButtonImage;
    [SerializeField]
    TextMeshProUGUI m_ButtonText;
    [SerializeField]
    LoginBonusCloseButton m_CloseButton;

    M4uProperty<string> detailText = new M4uProperty<string>();
    public string DetailText { get { return detailText.Value; } set { detailText.Value = value; } }

    M4uProperty<List<LoginBonusRecordListContext>> records = new M4uProperty<List<LoginBonusRecordListContext>>(new List<LoginBonusRecordListContext>());
    public List<LoginBonusRecordListContext> Records { get { return records.Value; } set { records.Value = value; } }

    M4uProperty<bool> isViewRecordList = new M4uProperty<bool>(true);
    /// <summary>トップの画面の表示・非表示</summary>
    public bool IsViewRecordList { get { return isViewRecordList.Value; } set { isViewRecordList.Value = value; } }

    public int m_LastUpdateCount = 0;

    public Action CloseAction;

    /// <summary>ログインした日アイテムの番号</summary>
    public uint m_LoginBonusIndex = 0;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        // 表示の一瞬アニメーションしていない間があるので、ここで設定
        m_DetailText.gameObject.SetActive(false);
        m_BGImage.color = new Color(1, 1, 1, 0);
        m_ButtonImage.color = new Color(1, 1, 1, 0);
        m_ButtonText.color = new Color(1, 1, 1, 0);

        SetUpButtons();
    }

    // Use this for initialization
    void Start()
    {
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
            SetScrollPosition(m_LoginBonusIndex);
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
        if (CloseAction != null)
        {
            CloseAction();
        }
    }

    /// <summary>
    /// 指定したインデクスの位置にスクロールする
    /// </summary>
    /// <param name="index"></param>
    bool SetScrollPosition(uint index)
    {
        GridLayoutGroup grid = GetComponentInChildren<GridLayoutGroup>();
        ScrollRect scroll = GetComponentInChildren<ScrollRect>();

        if (grid == null || scroll == null)
        {
            return false;
        }

        float height = scroll.viewport.rect.height;
        float contentHeight = scroll.content.rect.height;
        if (contentHeight <= height)
        {
            // コンテンツよりスクロールエリアのほうが広いので、スクロールしなくてもすべて表示されている
            scroll.verticalNormalizedPosition = 1;
            scroll.vertical = false;
            return true;
        }
        scroll.vertical = true;

        int row = (int)index / grid.constraintCount; // インデクスがある列の値
        int count = Records.Count / grid.constraintCount; // 列の総数
        int rest = Records.Count % grid.constraintCount;
        if (rest > 0)
        {
            ++count;
        }

        float pivot = 0.0f;
        float y = (row + pivot) / count;  // 要素の中心座標
        float scrollY = y - pivot * height / contentHeight;
        float ny = scrollY / (1 - height / contentHeight);  // ScrollRect用に正規化した座標

        float value = Mathf.Clamp(1 - ny, 0, 1);  // Y軸は下から上なので反転してやる
        scroll.verticalNormalizedPosition = value;

        return true;
    }
}
