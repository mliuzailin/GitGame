using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using M4u;

public class FooterSubMenu : MenuPartsBase
{
    private static readonly string AppearAnimationName = "footer_sub_menu_appear";
    private static readonly string DefaultAnimationName = "footer_sub_menu_loop";
    private static readonly string DisappearAnimationName = "footer_sub_menu_disappear";

    M4uProperty<Sprite> titleImage = new M4uProperty<Sprite>();
    public Sprite TitleImage { get { return titleImage.Value; } set { titleImage.Value = value; } }

    M4uProperty<List<FooterSubMenuItem>> itemList = new M4uProperty<List<FooterSubMenuItem>>();
    public List<FooterSubMenuItem> ItemList { get { return itemList.Value; } set { itemList.Value = value; } }

    private MAINMENU_CATEGORY m_Category = MAINMENU_CATEGORY.NONE;
    public MAINMENU_CATEGORY Category { get { return m_Category; } set { m_Category = value; } }

    public System.Action DidSelectClose = delegate { };
    public System.Action DidSelectCancel = delegate { };


    private List<ListItemModel> m_buttons = new List<ListItemModel>();

    private WaitTimer m_waitTimer = null;


    private string[] titleImageNameArray = new string[(int)MAINMENU_CATEGORY.MAX]
    {
        "ft_window_UNIT",
        "",
        "ft_window_SHOP",
        "HELP",
        "",
        "",
        "",
        ""
    };

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        ItemList = new List<FooterSubMenuItem>();
    }

    private void Update()
    {
        if (m_waitTimer != null)
            m_waitTimer.Tick(Time.deltaTime);
    }

    public void setup(MAINMENU_CATEGORY _category)
    {
        m_Category = _category;

        MasterMenuButtonItem[] masterArray = Resources.LoadAll<MasterMenuButtonItem>("MasterData/MenuButtonItem");

        TitleImage = ResourceManager.Instance.Load(titleImageNameArray[(int)_category]);

        List<MasterMenuButtonItem> tmpList = new List<MasterMenuButtonItem>();
        foreach (MasterMenuButtonItem _item in masterArray)
        {
            if (_item.categoryType != _category)
            {
                continue;
            }

#if BUILD_TYPE_DEBUG
            if (_item.buttonType == MAINMENU_BUTTON.HELP_DEBUG &&
                DebugOption.Instance.disalbeDebugMenu == true)
            {
                continue;
            }
#else
            if (_item.buttonType == MAINMENU_BUTTON.HELP_DEBUG)
            {
                continue;
            }
#endif

            tmpList.Add(_item);
        }

        //ソート
        tmpList.Sort((a, b) => b.sortindex - a.sortindex);

        ItemList.Clear();

        int index = 0;
        foreach (MasterMenuButtonItem _item in tmpList)
        {
            var model = new ListItemModel((uint)index++);
            FooterSubMenuItem newItem = new FooterSubMenuItem(model);
            newItem.buttonType = _item.buttonType;
            newItem.switchSeqType = _item.switchSeqType;
            newItem.IconImage = ResourceManager.Instance.Load(_item.iconImageName);
            newItem.TextImage = ResourceManager.Instance.Load(_item.textImageName);
            newItem.IsViewFlag = false;

            //強化イベントチェック
            if (_item.buttonType == MAINMENU_BUTTON.UNIT_BUILDUP &&
                MainMenuParam.m_BlendBuildEventSLV != 0)
            {
                newItem.IsViewFlag = true;
				newItem.FlagImage = ResourceManager.Instance.Load("flag_skill");
				switch (MainMenuParam.m_BlendBuildEventSLV)
				{
					case GlobalDefine.SLV_EVENT_ID_x0150:
						newItem.FlagRate = "1.5";
						break;
					case GlobalDefine.SLV_EVENT_ID_x0200:
						newItem.FlagRate = "2";
						break;
					case GlobalDefine.SLV_EVENT_ID_x0250:
						newItem.FlagRate = "2.5";
						break;
					case GlobalDefine.SLV_EVENT_ID_x0300:
						newItem.FlagRate = "3";
						break;
					case GlobalDefine.SLV_EVENT_ID_x0400:
						newItem.FlagRate = "4";
						break;
					case GlobalDefine.SLV_EVENT_ID_x0500:
						newItem.FlagRate = "5";
						break;
					case GlobalDefine.SLV_EVENT_ID_x1000:
						newItem.FlagRate = "10";
						break;
					default:
						break;
				}
            }

            ItemList.Add(newItem);
            m_buttons.Add(model);

            model.OnClicked += () =>
            {
                OnSelectButton(newItem);
            };
        }
#if BUILD_TYPE_DEBUG
        int debugIndex = 0;
        //デバッグメニューボタン追加
        if (_category == MAINMENU_CATEGORY.SHOP &&
            DebugOption.Instance.disalbeDebugMenu == false)
        {
            var model = new ListItemModel((uint)debugIndex++);
            FooterSubMenuItem newItem = new FooterSubMenuItem(model);
            newItem.buttonType = MAINMENU_BUTTON.HELP_DEBUG;
            newItem.switchSeqType = MAINMENU_SEQ.SEQ_DEBUG_MENU;
            newItem.IconImage = ResourceManager.Instance.Load("btn_zukan");
            newItem.TextImage = ResourceManager.Instance.Load("debug");
            ItemList.Add(newItem);
            m_buttons.Add(model);

            model.OnClicked += () =>
            {
                if (MainMenuManager.Instance.CheckMenuControlNG()
                    || MainMenuManager.Instance.IsPageSwitch())
                    return;

                OnSelectButton(newItem);
            };
        }
#endif
    }

    public void Show()
    {
        ButtonBlocker.Instance.Block("footer_sub_menu_show");
        PlayAnimation(AppearAnimationName, () =>
        {
            ButtonBlocker.Instance.Unblock("footer_sub_menu_show");
            PlayAnimation(DefaultAnimationName);
        });

        RegisterKeyEventCallback("showButtons", () =>
        {
            const float WAIT_INTERVAL_SECOND = 0.5f;
            System.Action SetTimer = null;

            System.Func<bool> IsAllStarted = () =>
            {
                foreach (var button in m_buttons)
                    if (!button.isStarted)
                        return false;

                return true;
            };

            SetTimer = () =>
            {
                if (m_buttons.Count == 0
                    || !IsAllStarted())
                {
                    m_waitTimer = new WaitTimer(WAIT_INTERVAL_SECOND, SetTimer);
                    return;
                }

                foreach (var button in m_buttons)
                {
                    button.Appear();
                }
                m_waitTimer = null;
            };

            SetTimer();
        });
    }

    //　直接呼ばずMainMenuFooterで登録した、DidSelectCloseを使うこと
    public void close()
    {
        ButtonBlocker.Instance.Block("footer_sub_menu_close");
        PlayAnimation(DisappearAnimationName, () =>
        {
            ButtonBlocker.Instance.Unblock("footer_sub_menu_close");
            Destroy(gameObject);
        });

        foreach (var button in m_buttons)
        {
            button.Close();
        }
    }

    private void OnSelectButton(FooterSubMenuItem _item)
    {
        MAINMENU_SEQ _next = MAINMENU_SEQ.SEQ_NONE;
        switch (_item.buttonType)
        {
            case MAINMENU_BUTTON.UNIT_BUILDUP:
            case MAINMENU_BUTTON.UNIT_POINT_LIMITOVER:
                MainMenuParam.m_BuildupBaseUnitUniqueId = 0;
                _next = _item.switchSeqType;
                break;
            case MAINMENU_BUTTON.UNIT_EVOLUTION:
            case MAINMENU_BUTTON.UNIT_POINT_EVOLUTION:
                MainMenuParam.m_EvolveBaseUnitUniqueId = 0;
                _next = _item.switchSeqType;
                break;
            case MAINMENU_BUTTON.UNIT_LINK:
                MainMenuParam.m_LinkBaseUnitUniqueId = 0;
                MainMenuParam.m_LinkTargetUnitUniqueId = 0;
                _next = _item.switchSeqType;
                break;
            case MAINMENU_BUTTON.SHOP_CHIP:
                if (StoreDialogManager.HasInstance) StoreDialogManager.Instance.OpenBuyStone();
                _next = MAINMENU_SEQ.SEQ_NONE;
                break;
            case MAINMENU_BUTTON.SHOP_UNIT_EXTEND:
                if (StoreDialogManager.HasInstance) StoreDialogManager.Instance.OpenDialogUnitExtend();
                _next = MAINMENU_SEQ.SEQ_NONE;
                break;
            case MAINMENU_BUTTON.SHOP_FRIEND_EXTEND:
                if (StoreDialogManager.HasInstance) StoreDialogManager.Instance.OpenDialogFriendExtend();
                _next = MAINMENU_SEQ.SEQ_NONE;
                break;
            case MAINMENU_BUTTON.SHOP_STAMINA_RECOVERY:
                if (StoreDialogManager.HasInstance) StoreDialogManager.Instance.OpenDialogStaminaRecovery();
                _next = MAINMENU_SEQ.SEQ_NONE;
                break;
            default:
                _next = _item.switchSeqType;
                break;

        }
        if (_next != MAINMENU_SEQ.SEQ_NONE && MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.AddSwitchRequest(_next, false, false);
        }

        DidSelectClose();
    }

    public void OnBeginDrag(BaseEventData _data)
    {
        if (ButtonBlocker.Instance.IsActive())
            return;

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        DidSelectCancel();
    }
}
