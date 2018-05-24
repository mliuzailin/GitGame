using UnityEngine;
using System.Collections;

public class MainMenuUnit : MainMenuSeq
{
    MenuButtonList m_buttonList;
    public MenuButtonList m_ButtonList
    {
        get
        {
            if (m_buttonList == null)
            {
                m_buttonList = GetComponentInChildren<MenuButtonList>();
            }
            return m_buttonList;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        m_ButtonList.setupMenuList(MAINMENU_CATEGORY.UNIT);
        m_ButtonList.setMenuAction(MAINMENU_BUTTON.UNIT_BUILDUP, openUnitSelectBildup);
        m_ButtonList.setMenuAction(MAINMENU_BUTTON.UNIT_EVOLUTION, openUnitSelectEvolution);
        m_ButtonList.setMenuAction(MAINMENU_BUTTON.UNIT_LINK, openUnitSelectLink);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.UNIT;
    }

    private void openUnitSelectBildup()
    {
        MainMenuParam.m_BuildupBaseUnitUniqueId = 0;
        MainMenuParam.m_UnitSelectType = MAINMENU_UNIT_SELECT_TYPE.BILDUP;
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_SELECT, false, false);
    }
    private void openUnitSelectEvolution()
    {
        MainMenuParam.m_EvolveBaseUnitUniqueId = 0;
        MainMenuParam.m_UnitSelectType = MAINMENU_UNIT_SELECT_TYPE.EVOLVE;
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_SELECT, false, false);
    }
    private void openUnitSelectLink()
    {
        MainMenuParam.m_LinkBaseUnitUniqueId = 0;
        MainMenuParam.m_LinkTargetUnitUniqueId = 0;
        MainMenuParam.m_UnitSelectType = MAINMENU_UNIT_SELECT_TYPE.LINK_BASE;
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_SELECT, false, false);
    }

}
