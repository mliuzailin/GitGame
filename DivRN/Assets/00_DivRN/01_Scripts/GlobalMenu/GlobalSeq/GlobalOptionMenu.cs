using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalOptionMenu : GlobalMenuSeq
{

    private OptionMenu m_OptionMenu = null;

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

    protected override void PageSwitchSetting(bool bActive, bool bBack)
    {
        base.PageSwitchSetting(bActive, bBack);

        //--------------------------------
        // 以下は有効になったタイミングでの処理なので、
        // フェードアウト指示の場合にはスルー
        //--------------------------------
        if (bActive == false)
        {
            return;
        }

        //--------------------------------
        // 戻り処理の場合は再構築スルー
        //--------------------------------
        if (bBack == true)
        {
            return;
        }

        //ページ初期化処理
        if (m_OptionMenu == null)
        {
            m_OptionMenu = GetComponentInChildren<OptionMenu>();
            m_OptionMenu.SetPositionAjustStatusBar(new Vector2(0, 4), new Vector2(-40, -372));

            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();

            string cMessageON = GameTextUtil.GetText("option_display7");
            string cMessageOFF = GameTextUtil.GetText("option_display16");
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.BGM, GameTextUtil.GetText("option_display1"), cMessageON, (cOption.m_OptionBGM == (int)LocalSaveDefine.OptionBGM.ON));
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.SE, GameTextUtil.GetText("option_display2"), cMessageON, (cOption.m_OptionSE == (int)LocalSaveDefine.OptionSE.ON));
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.GUIDE, GameTextUtil.GetText("option_display3"), cMessageON, (cOption.m_OptionGuide == (int)LocalSaveDefine.OptionGuide.ON));
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.SPEED, GameTextUtil.GetText("option_display6"), cMessageOFF, (cOption.m_OptionSpeed == (int)LocalSaveDefine.OptionSpeed.ON));
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.SKILL_TURN, GameTextUtil.GetText("battle_text01"), cMessageOFF, (cOption.m_OptionBattleSkillTurn == (int)LocalSaveDefine.OptionBattleSkillTurn.ON));
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.CONFIRM_AS, GameTextUtil.GetText("option_display17"), cMessageON, (cOption.m_OptionConfirmAS == (int)LocalSaveDefine.OptionConfirmAS.ON));
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.SKILL_COST, GameTextUtil.GetText("battle_text02"), cMessageOFF, (cOption.m_OptionBattleSkillCost == (int)LocalSaveDefine.OptionBattleSkillCost.ON));
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.BATTLE_ACHIEVE, GameTextUtil.GetText("battle_text03"), cMessageOFF, (cOption.m_OptionBattleAchieve == (int)LocalSaveDefine.OptionBattleAchieve.ON));
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.QUEST_END_TIPS, GameTextUtil.GetText("option_display18"), cMessageON, (cOption.m_OptionQuestEndTips == (int)LocalSaveDefine.OptionQuestEndTips.ON));

            m_OptionMenu.AddSpace().SetHight(10); // スペース
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.NONE, GameTextUtil.GetText("option_display19"), null, false).SetShowSwitch(false);
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.AUTO_PLAY_STOP_BOSS, GameTextUtil.GetText("option_display20"), cMessageON, (cOption.m_OptionAutoPlayStopBoss == (int)LocalSaveDefine.OptionAutoPlayStopBoss.ON)).SetIndent(1);
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.AUTO_PLAY_USE_AS, GameTextUtil.GetText("option_display21"), cMessageON, (cOption.m_OptionAutoPlayUseAS == (int)LocalSaveDefine.OptionAutoPlayUseAS.ON)).SetIndent(1);

            m_OptionMenu.AddSpace().SetHight(10); // スペース

            //------------------------------------
            // 通知設定
            //------------------------------------
            bool bNotification = (cOption.m_OptionNotification == (int)LocalSaveDefine.OptionNotification.ON);
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.NOTIFICATION, GameTextUtil.GetText("option_display8"), cMessageON, bNotification);
            if (bNotification == false)
            {
                // アプリケーションの通知設定がOFFの場合は、他の通知設定もOFFにする
                cOption.m_NotificationEvent = (int)LocalSaveDefine.OptionNotificationEvent.OFF;
                cOption.m_NotificationStaminaMax = (int)LocalSaveDefine.OptionNotificationStaminaMax.OFF;
            }
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.NOT_EVENT, GameTextUtil.GetText("option_display10"), cMessageON, (cOption.m_NotificationEvent == (int)LocalSaveDefine.OptionNotificationEvent.ON)).SetIndent(1);
            m_OptionMenu.AddItem().setup(OptionMenu.ItemType.NOT_STAMINA, GameTextUtil.GetText("option_display11"), cMessageON, (cOption.m_NotificationStaminaMax == (int)LocalSaveDefine.OptionNotificationStaminaMax.ON)).SetIndent(1);

            //------------------------------------
            // ボタンのイベント
            //------------------------------------
            for (int i = 0; i < (int)OptionMenu.ItemType.MAX; i++)
            {
                OptionMenuItem item = m_OptionMenu.GetOptionItem((OptionMenu.ItemType)i);
                if (item != null)
                {
                    item.DidSelectItem = OnSelect;
                }
            }

            LocalSaveManager.Instance.SaveFuncOption(cOption); // 状態を保存する
        }
    }

    public void OnSelect(OptionMenu.ItemType _type)
    {
        OptionMenuItem selectItem = m_OptionMenu.GetOptionItem(_type);
        switch (_type)
        {
            case OptionMenu.ItemType.NOTIFICATION:
                {
                    bool isSwitch = m_OptionMenu.GetOptionItem(_type).IsSwitch();

                    m_OptionMenu.GetOptionItem(OptionMenu.ItemType.NOT_EVENT).SetSwitch(isSwitch); // イベント通知設定を変更
                    m_OptionMenu.GetOptionItem(OptionMenu.ItemType.NOT_STAMINA).SetSwitch(isSwitch); // スタミナ通知設定を変更
                }
                break;
            case OptionMenu.ItemType.NOT_EVENT:
            case OptionMenu.ItemType.NOT_STAMINA:
                {
                    if (m_OptionMenu.GetOptionItem(_type).IsSwitch() == true)
                    {
                        // 通知設定がONになった場合は、アプリケーションの通知設定もONにする
                        m_OptionMenu.GetOptionItem(OptionMenu.ItemType.NOTIFICATION).SetSwitch(true);
                    }
                }
                break;
        }


        //-----------------------------------------------
        // 状態を保存する
        //-----------------------------------------------
        LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();

        cOption.m_OptionBGM = CheckSwitch(OptionMenu.ItemType.BGM) ? (int)LocalSaveDefine.OptionBGM.ON : (int)LocalSaveDefine.OptionBGM.OFF;
        cOption.m_OptionSE = CheckSwitch(OptionMenu.ItemType.SE) ? (int)LocalSaveDefine.OptionSE.ON : (int)LocalSaveDefine.OptionSE.OFF;
        cOption.m_OptionGuide = CheckSwitch(OptionMenu.ItemType.GUIDE) ? (int)LocalSaveDefine.OptionGuide.ON : (int)LocalSaveDefine.OptionGuide.OFF;
        cOption.m_OptionSpeed = CheckSwitch(OptionMenu.ItemType.SPEED) ? (int)LocalSaveDefine.OptionSpeed.ON : (int)LocalSaveDefine.OptionSpeed.OFF;
        cOption.m_OptionBattleSkillTurn = CheckSwitch(OptionMenu.ItemType.SKILL_TURN) ? (int)LocalSaveDefine.OptionBattleSkillTurn.ON : (int)LocalSaveDefine.OptionBattleSkillTurn.OFF;
        cOption.m_OptionConfirmAS = CheckSwitch(OptionMenu.ItemType.CONFIRM_AS) ? (int)LocalSaveDefine.OptionConfirmAS.ON : (int)LocalSaveDefine.OptionConfirmAS.OFF;
        cOption.m_OptionBattleSkillCost = CheckSwitch(OptionMenu.ItemType.SKILL_COST) ? (int)LocalSaveDefine.OptionBattleSkillCost.ON : (int)LocalSaveDefine.OptionBattleSkillCost.OFF;
        cOption.m_OptionBattleAchieve = CheckSwitch(OptionMenu.ItemType.BATTLE_ACHIEVE) ? (int)LocalSaveDefine.OptionBattleAchieve.ON : (int)LocalSaveDefine.OptionBattleAchieve.OFF;
        cOption.m_OptionQuestEndTips = CheckSwitch(OptionMenu.ItemType.QUEST_END_TIPS) ? (int)LocalSaveDefine.OptionQuestEndTips.ON : (int)LocalSaveDefine.OptionQuestEndTips.OFF;
        cOption.m_OptionAutoPlayStopBoss = CheckSwitch(OptionMenu.ItemType.AUTO_PLAY_STOP_BOSS) ? (int)LocalSaveDefine.OptionAutoPlayStopBoss.ON : (int)LocalSaveDefine.OptionAutoPlayStopBoss.OFF;
        cOption.m_OptionAutoPlayUseAS = CheckSwitch(OptionMenu.ItemType.AUTO_PLAY_USE_AS) ? (int)LocalSaveDefine.OptionAutoPlayUseAS.ON : (int)LocalSaveDefine.OptionAutoPlayUseAS.OFF;

        cOption.m_OptionNotification = CheckSwitch(OptionMenu.ItemType.NOTIFICATION) ? (int)LocalSaveDefine.OptionNotification.ON : (int)LocalSaveDefine.OptionNotification.OFF;

        cOption.m_NotificationEvent = CheckSwitch(OptionMenu.ItemType.NOT_EVENT) ? (int)LocalSaveDefine.OptionNotificationEvent.ON : (int)LocalSaveDefine.OptionNotificationEvent.OFF;
        cOption.m_NotificationStaminaMax = CheckSwitch(OptionMenu.ItemType.NOT_STAMINA) ? (int)LocalSaveDefine.OptionNotificationStaminaMax.ON : (int)LocalSaveDefine.OptionNotificationStaminaMax.OFF;

        LocalSaveManager.Instance.SaveFuncOption(cOption);
    }

    public bool CheckSwitch(OptionMenu.ItemType _type)
    {
        return m_OptionMenu.GetOptionItem(_type).IsSwitch();
    }
}
