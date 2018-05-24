/**
 *  @file   QuestDetailDebugMenu.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/07/26
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using M4u;
using System;

public class QuestDetailDebugMenu : MenuPartsBase
{
    const int BUTTON_MOVE_RATE = 1;

    [SerializeField]
    InputField m_IdInputField;
    [SerializeField]
    InputField m_NoInputField;
    [SerializeField]
    InputField m_OffsetYInputField;

    RectTransform m_CharaImageRect = null;
    public QuestDetailBG m_QuestDetailBG = null;
    public uint m_InitCharaID;

    private int m_Y;
    private int m_adjustY;
    int m_Index = -1;

    MasterDataParamChara[] m_CharaMasterArray = null;
    MasterDataParamChara m_CurrentCharaMaster;

    M4uProperty<string> nameText = new M4uProperty<string>();
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    M4uProperty<bool> isActiveBossOffsetPanel = new M4uProperty<bool>(false);
    private bool IsActiveBossOffsetPanel { get { return isActiveBossOffsetPanel.Value; } set { isActiveBossOffsetPanel.Value = value; } }

    M4uProperty<Color> offsetYTextColor = new M4uProperty<Color>(Color.black);
    private Color OffsetYTextColor { get { return offsetYTextColor.Value; } set { offsetYTextColor.Value = value; } }

    static public QuestDetailDebugMenu Create(Transform _transform)
    {
#if BUILD_TYPE_DEBUG
        GameObject debugMenuPrefab = Resources.Load<GameObject>("Prefab/Debug/QuestDetailDebugMenu");
        if (debugMenuPrefab != null)
        {
            GameObject debugMenuObj = Instantiate(debugMenuPrefab);
            if (debugMenuObj != null)
            {
                debugMenuObj.transform.SetParent(_transform, false);
                return debugMenuObj.GetComponent<QuestDetailDebugMenu>();
            }
        }
#endif
        return null;
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        m_CharaMasterArray = MasterFinder<MasterDataParamChara>.Instance.GetAll();
        AssetAutoSetCharaImage charaImage = m_QuestDetailBG.GetComponentInChildren<AssetAutoSetCharaImage>();
        m_CharaImageRect = charaImage.gameObject.GetComponent<RectTransform>();

        m_Y
        = m_adjustY
        = (int)m_CharaImageRect.anchoredPosition.y;
        m_OffsetYInputField.text = m_Y.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUpParamChara(MasterDataParamChara charaMaster)
    {
        if (charaMaster == null)
        {
            SetUpParamChara(0);
        }
        SetUpParamChara(Array.IndexOf(m_CharaMasterArray, charaMaster));
    }

    public void SetUpParamChara(int index)
    {
        //--------------------------------
        // アクセス番号決定
        //--------------------------------
        if (index >= m_CharaMasterArray.Length)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = m_CharaMasterArray.Length - 1;
        }

        MasterDataParamChara charaMaster = m_CharaMasterArray[index];
        m_CurrentCharaMaster = charaMaster;
        m_Index = index;

        NameText = charaMaster.name;
        m_IdInputField.text = charaMaster.fix_id.ToString();
        m_NoInputField.text = charaMaster.draw_id.ToString();

        m_QuestDetailBG.setupChara(charaMaster.fix_id);
        StartCoroutine(m_QuestDetailBG.LoopCheckBossImageLoad(() =>
        {

        }));

    }

    /// <summary>
    /// 変更前の値と違う場合はInputFieldの色を変える
    /// </summary>
    public void CheckAdjustUV()
    {
        OffsetYTextColor = (m_Y == m_adjustY) ? Color.black : Color.red;
    }

    public void SetUpPosition()
    {
        m_CharaImageRect.anchoredPosition = new Vector2(m_CharaImageRect.anchoredPosition.x, m_Y);
    }

    public void OnClickNext()
    {
        SetUpParamChara(m_Index + 1);
    }

    public void OnClickPrev()
    {
        SetUpParamChara(m_Index - 1);
    }

    public void OnClickClear()
    {
        m_Y = m_adjustY;
        m_OffsetYInputField.text = m_Y.ToString();

        CheckAdjustUV();
        SetUpPosition();
        SetUpParamChara(MasterDataUtil.GetCharaParamFromID(m_InitCharaID));
    }

    public void OnClickUp()
    {
        m_Y += BUTTON_MOVE_RATE;
        m_OffsetYInputField.text = m_Y.ToString();
        CheckAdjustUV();
        SetUpPosition();
    }

    public void OnClickDown()
    {
        m_Y -= BUTTON_MOVE_RATE;
        m_OffsetYInputField.text = m_Y.ToString();
        CheckAdjustUV();
        SetUpPosition();
    }

    public void OnChangedDebug(bool value)
    {
        IsActiveBossOffsetPanel = value;
    }

    #region テキスト入力
    public void OnEndEditID(string value)
    {
        uint u;
        uint.TryParse(value, out u);
        SetUpParamChara(MasterDataUtil.GetCharaParamFromID(u));
    }

    public void OnEndEditNo(string value)
    {
        uint u;
        uint.TryParse(value, out u);
        SetUpParamChara(MasterDataUtil.GetCharaParamFromDrawID(u));
    }

    public void OnEndEditOffsetY(string value)
    {
        int.TryParse(value, out m_Y);
        m_OffsetYInputField.text = m_Y.ToString();
        CheckAdjustUV();
        SetUpPosition();
    }
    #endregion
}
