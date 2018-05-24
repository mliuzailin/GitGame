using UnityEngine;
using System.Collections;

public class MainMenuShop : MainMenuSeq
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

    public enum SHOP_MENU_STEP
    {
        TOP,
        STAMINA_RECOVERY,
        FRIEND_EXT,
        UNIT_EXT,
    }


    private SHOP_MENU_STEP m_Step = SHOP_MENU_STEP.TOP;

    // スタミナ回復ルーチン
    private StoreUseTipStamina m_UseTipStamina = new StoreUseTipStamina();
    // フレンド枠拡張ルーチン
    private StoreUseTipFriendExt m_UseTipFriendExt = new StoreUseTipFriendExt();
    // 所持ユニット枠拡張ルーチン
    private StoreUseTipUnitExt m_UseTipUnitExt = new StoreUseTipUnitExt();

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

        switch (m_Step)
        {
            case SHOP_MENU_STEP.TOP:
                break;
            case SHOP_MENU_STEP.STAMINA_RECOVERY:
                if (m_UseTipStamina.UpdateProcess() != true)
                {
                    // 処理が終わったらステップを元に戻す
                    m_Step = SHOP_MENU_STEP.TOP;
                }
                break;
            case SHOP_MENU_STEP.FRIEND_EXT:
                if (m_UseTipFriendExt.UpdateProcess() != true)
                {
                    // 処理が終わったらステップを元に戻す
                    m_Step = SHOP_MENU_STEP.TOP;
                }
                break;
            case SHOP_MENU_STEP.UNIT_EXT:
                if (m_UseTipUnitExt.UpdateProcess() != true)
                {
                    // 処理が終わったらステップを元に戻す
                    m_Step = SHOP_MENU_STEP.TOP;
                }
                break;
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        m_ButtonList.setupMenuList(MAINMENU_CATEGORY.SHOP);
        m_ButtonList.setMenuAction(MAINMENU_BUTTON.SHOP_UNIT_EXTEND, openDialogUnitExtend);
        m_ButtonList.setMenuAction(MAINMENU_BUTTON.SHOP_FRIEND_EXTEND, openDialogFriendExtend);
        m_ButtonList.setMenuAction(MAINMENU_BUTTON.SHOP_STAMINA_RECOVERY, openDialogStaminaRecovery);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.SHOP;
    }

    private void openDialogUnitExtend()
    {
        m_UseTipUnitExt.StartProcess(this);
        // ページステップも変える
        m_Step = SHOP_MENU_STEP.UNIT_EXT;
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

    }
    private void openDialogFriendExtend()
    {
        m_UseTipFriendExt.StartProcess(this);
        // ページステップも変える
        m_Step = SHOP_MENU_STEP.FRIEND_EXT;
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }
    private void openDialogStaminaRecovery()
    {
        m_UseTipStamina.StartProcess(this);
        // ページステップも変える
        m_Step = SHOP_MENU_STEP.STAMINA_RECOVERY;
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }
}
