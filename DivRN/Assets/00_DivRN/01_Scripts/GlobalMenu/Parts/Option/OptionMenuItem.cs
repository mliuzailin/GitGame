using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class OptionMenuItem : MenuPartsBase
{
    const int INDENT_SIZE = 60;


    [SerializeField]
    GameObject m_Title = null;

    public System.Action<OptionMenu.ItemType> DidSelectItem = delegate { };

    private M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    private M4uProperty<bool> isViewMessage = new M4uProperty<bool>();
    public bool IsViewMessage { get { return isViewMessage.Value; } set { isViewMessage.Value = value; } }

    private M4uProperty<string> message = new M4uProperty<string>();
    public string Message { get { return message.Value; } set { message.Value = value; } }

    M4uProperty<Sprite> switchImage = new M4uProperty<Sprite>();
    public Sprite SwitchImage { get { return switchImage.Value; } set { switchImage.Value = value; } }

    M4uProperty<bool> isShowSwitch = new M4uProperty<bool>();
    public bool IsShowSwitch { get { return isShowSwitch.Value; } set { isShowSwitch.Value = value; } }

    public OptionMenu.ItemType m_Type = OptionMenu.ItemType.NONE;

    private Vector2 m_InitTitlePosition;
    private bool m_IsOn;

    private void Awake()
    {
        IsViewMessage = true;
        GetComponent<M4uContextRoot>().Context = this;

        RectTransform titleRect = m_Title.GetComponent<RectTransform>();
        m_InitTitlePosition = titleRect.anchoredPosition;
        IsShowSwitch = true;
    }

    private void Start()
    {
        name = m_Type.ToString();
    }

    public OptionMenuItem setup(OptionMenu.ItemType _type, string _title, string _message, bool bSwitch)
    {
        SetSwitch(bSwitch);
        m_Type = _type;
        Title = _title;
        Message = _message;

        return this;
    }

    /// <summary>
    /// テキスト位置をずらす
    /// </summary>
    /// <param name="_num"></param>
    /// <returns></returns>
    public OptionMenuItem SetIndent(int _num)
    {
        RectTransform titleRect = m_Title.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(m_InitTitlePosition.x + (INDENT_SIZE * _num), m_InitTitlePosition.y);

        return this;
    }

    public void OnClickSwitch()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        SetSwitch(!m_IsOn);
        DidSelectItem(m_Type);
    }

    public bool IsSwitch()
    {
        return m_IsOn;
    }

    public void SetSwitch(bool isOn)
    {
        m_IsOn = isOn;
        if (isOn == true)
        {
            SwitchImage = ResourceManager.Instance.Load("btn_sw_on", ResourceType.Common);
        }
        else
        {
            SwitchImage = ResourceManager.Instance.Load("btn_sw_off", ResourceType.Common);
        }
    }

    public void SetShowSwitch(bool isShow)
    {
        IsShowSwitch = isShow;
    }
}
