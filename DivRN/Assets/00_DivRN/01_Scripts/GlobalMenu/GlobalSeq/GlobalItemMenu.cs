using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class GlobalItemMenu : GlobalMenuSeq
{
    private Item m_Item = null;
    private Dialog m_PacketErrDialog = null;

    private int m_UpdateCount = 0;

    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (m_UpdateCount != 0)
        {
            m_UpdateCount--;
            if (m_UpdateCount < 0)
            {
                m_UpdateCount = 0;
            }
            if (m_Item != null)
            {
                m_Item.updateLayout();
            }
        }
        if (PageSwitchUpdate() == false)
        {
            return;
        }
        //----------------------------------------
        // エラーダイアログが出てるならダイアログ処理
        //----------------------------------------
        if (m_PacketErrDialog != null)
        {
            // 確認ボタン押下
            if (m_PacketErrDialog.PushButton != DialogButtonEventType.NONE)
            {
                m_PacketErrDialog.Hide();
                m_PacketErrDialog = null;
            }
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
        if (m_Item == null)
        {
            m_Item = GetComponentInChildren<Item>();
            m_Item.SetPositionAjustStatusBar(new Vector2(0, -6), new Vector2(-20, -351));
            m_Item.DidSelectTab = OnSelectTab;

            // ナビゲーションバー
            m_Item.NaviText0 = GameTextUtil.GetText("item_tab1");
            m_Item.NaviText1 = GameTextUtil.GetText("item_tab2");
            m_Item.NaviText2 = GameTextUtil.GetText("item_tab3");

            m_Item.PointLabel = GameTextUtil.GetText("item_title1");
            m_Item.TicketLabel = GameTextUtil.GetText("item_title2");
            m_Item.KeyLabel = GameTextUtil.GetText("item_title3");

            m_Item.EmptyLabel = GameTextUtil.GetText("common_not_list");
        }

        //
        m_Item.ResetRecordAll();


        //アイテムリスト
        setupItem();

        //ポイント関連
        setupPoint();

        //クエストキー
        setupKey();

        m_Item.setup();
        m_UpdateCount = 5;
    }

    private void setupPoint()
    {
        if (UserDataAdmin.Instance.m_StructPlayer == null)
        {
            return;
        }

        List<ItemKeyContext> tmpList = new List<ItemKeyContext>();
        PacketStructUseItem[] items = UserDataAdmin.Instance.m_StructPlayer.item_list;
        // レコード追加
        for (int id = 0; id < items.Length; id++)
        {
            //所持していない
            if (items[id].item_cnt == 0)
            {
                continue;
            }

            //マスターに存在するか
            MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find((int)items[id].item_id);
            if (itemMaster == null)
            {
                continue;
            }

            //チケットかどうか
            if (itemMaster.gacha_event_id == 0)
            {
                continue;
            }

            //対象の有効なガチャが存在する
            MasterDataGacha gachaMaster = MasterDataUtil.GetActiveItemPointGachaMaster(itemMaster.fix_id);
            if (gachaMaster == null)
            {
                continue;
            }

            //イベントマスター取得
            MasterDataEvent eventMaster = MasterDataUtil.GetMasterDataEventFromID(itemMaster.gacha_event_id);
            if (eventMaster == null)
            {
                continue;
            }

            //期間外
            if (TimeEventManager.Instance.ChkEventActive(eventMaster.event_id) == false)
            {
                continue;
            }


            //ガチャチケット
            ItemKeyContext _newTicket = new ItemKeyContext();
            _newTicket.Category = ItemKeyContext.CategoryType.ScratchTicket;
            _newTicket.Name = itemMaster.item_name;
            _newTicket.Count = items[id].item_cnt.ToString();

            _newTicket.setupIcon(itemMaster);

            _newTicket.itemMaster = itemMaster;
            _newTicket.DidSelectItemKey = OnSelectKey;

            _newTicket.IsViewTime = false;
            _newTicket.Time = "";
            _newTicket.timing_end = eventMaster.timing_end;
            if (eventMaster.timing_end != 0)
            {
                string timeFormat = GameTextUtil.GetText("common_expirationdate");
                DateTime endTime = TimeUtil.GetDateTime(eventMaster.timing_end).SubtractAMinute();
                _newTicket.Time = string.Format(timeFormat, endTime.ToString("yyyy/MM/dd (HH:mm)"));
                _newTicket.IsViewTime = true;
                m_Item.PointList.Add(_newTicket);
            }
            else
            {
                //期間が無限のチケットは最後に追加するので一時退避
                tmpList.Add(_newTicket);
            }

        }

        //期間でソート
        m_Item.PointList.Sort((a, b) => (int)a.timing_end - (int)b.timing_end);

        //友情ポイント・ユニットポイント追加
        {
            ItemKeyContext _newFP = new ItemKeyContext();
            _newFP.Category = ItemKeyContext.CategoryType.FriendPoint;
            _newFP.IconImage = ResourceManager.Instance.Load("friend_point_icon", ResourceType.Common);
            _newFP.Name = GameTextUtil.GetText("common_text1");
            _newFP.Count = UserDataAdmin.Instance.m_StructPlayer.have_friend_pt.ToString();
            _newFP.IsViewTime = false;
            _newFP.Time = "";
            _newFP.DidSelectItemKey = OnSelectKey;
            m_Item.PointList.Add(_newFP);

            ItemKeyContext _newUP = new ItemKeyContext();
            _newUP.Category = ItemKeyContext.CategoryType.UnitPoint;
            _newUP.IconImage = ResourceManager.Instance.Load("mm_item_unitpoint", ResourceType.Common);
            _newUP.Name = GameTextUtil.GetText("common_text2");
            _newUP.Count = UserDataAdmin.Instance.m_StructPlayer.have_unit_point.ToString();
            _newUP.IsViewTime = false;
            _newUP.Time = "";
            _newUP.DidSelectItemKey = OnSelectKey;
            m_Item.PointList.Add(_newUP);
        }

        //期間が無限のチケット追加
        m_Item.PointList.AddRange(tmpList);
    }

    private void setupItem()
    {
        if (UserDataAdmin.Instance.m_StructPlayer == null)
        {
            return;
        }

        // ユーザーの所持アイテムリスト
        //MasterDataUseItem[] item_masters = MasterFinder<MasterDataUseItem>.Instance.GetAll();
        PacketStructUseItem[] items = UserDataAdmin.Instance.m_StructPlayer.item_list;

        // レコード追加
        for (int id = 0; id < items.Length; id++)
        {
            if (items[id].item_cnt == 0)
            {
                continue;
            }

            MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find((int)items[id].item_id);
            if (itemMaster == null)
            {
                continue;
            }

            if (MasterDataUtil.ChkUseItemTypeStaminaRecovery(itemMaster))
            {
                //スタミナ系アイテム
                m_Item.AddRecord(Item.ItemType.Stamina, itemMaster, (int)items[id].item_cnt, items[id].use_timing, OnSelectItemData);
            }
            else if (MasterDataUtil.ChkUseItemTypeAmend(itemMaster))
            {
                //報酬増加アイテム
                m_Item.AddRecord(Item.ItemType.RewardUp, itemMaster, (int)items[id].item_cnt, items[id].use_timing, OnSelectItemData);
            }
            else if (itemMaster.gacha_event_id != 0)
            {
                //ガチャチケットはポイント関連に追加するのでここでは何もしない
            }
            else
            {
                //その他
                m_Item.AddRecord(Item.ItemType.Other, itemMaster, (int)items[id].item_cnt, items[id].use_timing, OnSelectItemData);
            }

        }

    }

    private void setupKey()
    {

        //----------------------------------------
        // プレイヤー情報に持っているクエストキーリストを取得
        // ※サーバーで有効期限の近い順でソートしてくれている
        //----------------------------------------
        PacketStructQuestKey[] cKeyList = UserDataAdmin.Instance.m_StructPlayer.quest_key_list;

        //--------------------------------
        // ぬるチェック
        //--------------------------------
        if (cKeyList == null
        || cKeyList.Length <= 0)
        {
            return;
        }

        //--------------------------------
        // 所持キーリストをアイテムリストに追加
        // ※クエストキーはサーバーでソートしてくれてるのでそのまま追加
        //--------------------------------
        for (int iKey = 0; iKey < cKeyList.Length; iKey++)
        {
            if (cKeyList[iKey] == null
            || cKeyList[iKey].quest_key_cnt <= 0)
            {
                continue;
            }

            //クエストキーマスタ取得
            MasterDataQuestKey cQuestKeyMaster = MasterFinder<MasterDataQuestKey>.Instance.Find((int)cKeyList[iKey].quest_key_id);
            if (cQuestKeyMaster == null)
            {
                Debug.LogError("QuestKey MasterData None! - " + cKeyList[iKey].quest_key_id);
                continue;
            }

            //クエストキーマスタに設定あり、クエストキー定義単位で表示
            bool bRet = MainMenuUtil.ChkQuestKeyPlayableFromId(cQuestKeyMaster);
            if (bRet == false)
            {
                continue;
            }

            UIAtlas cIconAtlas = SceneObjReferMainMenu.Instance.m_MainMenuAtlas;
            string strSpriteName = "mm_quest_key";

            //----------------------------------------
            // ここまできたらチェックOK。表示対象としてリストに登録
            //----------------------------------------
            ItemKeyContext _newKey = new ItemKeyContext();
            _newKey.Category = ItemKeyContext.CategoryType.QuestKey;
            _newKey.IconImage = cIconAtlas.GetSprite(strSpriteName);
            _newKey.keyMaster = cQuestKeyMaster;
            _newKey.Name = cQuestKeyMaster.key_name;
            _newKey.Count = cKeyList[iKey].quest_key_cnt.ToString();
            _newKey.IsViewTime = false;
            _newKey.Time = "";
            _newKey.timing_end = cQuestKeyMaster.timing_end;
            if (cQuestKeyMaster.timing_end != 0)
            {
                string timeFormat = GameTextUtil.GetText("common_expirationdate");
                DateTime endTime = TimeUtil.GetDateTime(cQuestKeyMaster.timing_end).SubtractAMinute();
                _newKey.Time = string.Format(timeFormat, endTime.ToString("yyyy/MM/dd (HH:mm)"));
                _newKey.IsViewTime = true;
            }
            _newKey.DidSelectItemKey = OnSelectKey;

            //リストに登録
            m_Item.KeyList.Add(_newKey);
        }

        //期間でソート
        m_Item.KeyList.Sort((a, b) => (int)a.timing_end - (int)b.timing_end);
    }

    private void OnSelectTab()
    {
        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
        m_UpdateCount = 5;
    }

    private void OnSelectItemData(ItemDataContext _item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        if (_item.Count == 0)
        {
            //アイテムない
            Dialog _newDialog = Dialog.Create(DialogType.DialogYesNo);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mm53q_title");
            string mainFormat = GameTextUtil.GetText("mm53q_content");
            _newDialog.SetDialogText(DialogTextType.MainText, string.Format(mainFormat, _item.ItemMaster.item_name, ""));
            _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
            {
                //ポイントショップへ
                MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_SHOP_POINT, false, false);
                MainMenuManager.Instance.CloseGlobalMenu();
            });
            _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
            _newDialog.DisableCancelButton();
            _newDialog.Show();

        }
        else if (MasterDataUtil.ChkUseItemTypeStaminaRecovery(_item.ItemMaster) && UserDataAdmin.Instance.IsStaminaMax())
        {
            //スタミナが満タン
            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetTitleFromTextKey("mm56q_title");
            _newDialog.SetMainFromTextKey("mm56q_content");
            _newDialog.SetDialogText(DialogTextType.OKText, Dialog.CONFIRM_BUTTON_TITLE);
            _newDialog.DisableCancelButton();
            _newDialog.Show();
        }
        else if (MainMenuUtil.IsItemEffectValid(_item.ItemMaster))
        {
            //使用中
            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetTitleFromTextKey("mm61q_title");
            _newDialog.SetMainFromTextKey("mm61q_content");
            _newDialog.SetDialogText(DialogTextType.OKText, Dialog.CONFIRM_BUTTON_TITLE);
            _newDialog.DisableCancelButton();
            _newDialog.Show();
        }
        else
        {
            string _item_name = _item.ItemMaster.item_name;
            string _item_effect_time = "";
            if (_item.ItemMaster.effect_span_m != 0)
            {
                _item_effect_time = string.Format("({0}分)", _item.ItemMaster.effect_span_m);
            }
            Dialog _newDialog = Dialog.Create(DialogType.DialogYesNo);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mm52q_title");
            string mainFormat = GameTextUtil.GetText("mt52q_content");

            if (MasterDataUtil.ChkUseItemTypeStaminaRecovery(_item.ItemMaster))
            {
                //回復後のスタミナ値を表示.
                int result_stamina = UserDataAdmin.Instance.GetUseItemStamina(_item.ItemMaster);
                mainFormat += "\n\n" + string.Format(GameTextUtil.GetText("sh132q_content3"), result_stamina, UserDataAdmin.Instance.m_StructPlayer.stamina_max);
                // オーバー回復になるかどうか
                if (result_stamina > UserDataAdmin.Instance.m_StructPlayer.stamina_max)
                {
                    mainFormat += "\n\n" + GameTextUtil.GetText("sh132q_content5");
                }
            }
            _newDialog.SetDialogText(DialogTextType.MainText, string.Format(mainFormat, _item_name, _item_effect_time));
            _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
            {
                useItem(_item.ItemMaster.fix_id);
            });
            _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
            _newDialog.DisableCancelButton();
            _newDialog.Show();
        }
    }

    private void useItem(uint fix_id)
    {
        ServerDataUtilSend.SendPacketAPI_ItemUse(fix_id)
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvItemUse>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            //アイテム情報更新
            updateItemAll();

            MasterDataUseItem itemMaster = MasterFinder<MasterDataUseItem>.Instance.Find((int)fix_id);
            if (itemMaster != null)
            {
                // スタミナ回復ダイアログを表示
                if (MasterDataUtil.ChkUseItemTypeStaminaRecovery(itemMaster))
                {
                    DialogManager.Open1B("sh135q_title", "sh135q_content", "common_button1", true, true)
                        .SetOkEvent(() =>
                        {
                            StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
                        });
                }
                else
                {
                    StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
                }
            }
        })
        .setErrorAction(_data =>
        {
            int i = 0;
            i++;
        })
        .SendStart();
    }

    private void OnSelectKey(ItemKeyContext _key)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        string item_name = "";
        string move_pos = "";
        switch (_key.Category)
        {
            case ItemKeyContext.CategoryType.FriendPoint:
                item_name = GameTextUtil.GetText("common_text1");
                move_pos = GameTextUtil.GetText("common_text7");
                break;
            case ItemKeyContext.CategoryType.UnitPoint:
                item_name = GameTextUtil.GetText("common_text2");
                move_pos = GameTextUtil.GetText("common_text8");
                break;
            case ItemKeyContext.CategoryType.ScratchTicket:
                item_name = _key.itemMaster.item_name;
                move_pos = GameTextUtil.GetText("common_text7");
                break;
            case ItemKeyContext.CategoryType.QuestKey:
                item_name = _key.keyMaster.key_name;
                move_pos = GameTextUtil.GetText("common_text9");
                break;
        }
        Dialog _newDialog = Dialog.Create(DialogType.DialogYesNo);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "mm54q_title");
        string mainFormat = GameTextUtil.GetText("mm54q_content1");
        _newDialog.SetDialogText(DialogTextType.MainText, string.Format(mainFormat, item_name, move_pos));
        _newDialog.SetDialogEvent(DialogButtonEventType.YES, () =>
        {
            switch (_key.Category)
            {
                case ItemKeyContext.CategoryType.FriendPoint:
                    MainMenuParam.m_GachaMaster = MasterDataUtil.GetActiveFriendGachaMaster();
                    MainMenuManager.Instance.SetResetSubTabFlag();
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_GACHA_MAIN, false, false);
                    MainMenuManager.Instance.CloseGlobalMenu();
                    break;
                case ItemKeyContext.CategoryType.UnitPoint:
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_SHOP_POINT, false, false);
                    MainMenuManager.Instance.CloseGlobalMenu();
                    break;
                case ItemKeyContext.CategoryType.ScratchTicket:
                    MainMenuParam.m_GachaMaster = MasterDataUtil.GetActiveItemPointGachaMaster(_key.itemMaster.fix_id);
                    MainMenuManager.Instance.SetResetSubTabFlag();
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_GACHA_MAIN, false, false);
                    MainMenuManager.Instance.CloseGlobalMenu();
                    break;
                case ItemKeyContext.CategoryType.QuestKey:
                    MainMenuParam.SetQuestSelectParam(_key.keyMaster.key_area_category_id);
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false, false);
                    MainMenuManager.Instance.CloseGlobalMenu();
                    break;
            }
        });
        _newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        _newDialog.DisableCancelButton();
        _newDialog.Show();
    }

    private void updateItemAll()
    {
        List<ItemDataContext> removeList = new List<ItemDataContext>();
        for (int i = 0; i < m_Item.Records0.Count; i++)
        {
            PacketStructUseItem _item = UserDataAdmin.Instance.SearchUseItem(m_Item.Records0[i].ItemMaster.fix_id);
            if (_item == null)
            {
                continue;
            }

            m_Item.Records0[i].Count = (int)_item.item_cnt;
            m_Item.Records0[i].use_timing = _item.use_timing;
            if (m_Item.Records0[i].Count == 0) removeList.Add(m_Item.Records0[i]);
        }
        for (int i = 0; i < removeList.Count; i++)
        {
            m_Item.Records0.Remove(removeList[i]);
        }

        removeList.Clear();
        for (int i = 0; i < m_Item.Records1.Count; i++)
        {
            PacketStructUseItem _item = UserDataAdmin.Instance.SearchUseItem(m_Item.Records1[i].ItemMaster.fix_id);
            if (_item == null)
            {
                continue;
            }

            m_Item.Records1[i].Count = (int)_item.item_cnt;
            m_Item.Records1[i].use_timing = _item.use_timing;
            if (m_Item.Records1[i].Count == 0) removeList.Add(m_Item.Records1[i]);
        }
        for (int i = 0; i < removeList.Count; i++)
        {
            m_Item.Records1.Remove(removeList[i]);
        }

        removeList.Clear();
        for (int i = 0; i < m_Item.Records2.Count; i++)
        {
            PacketStructUseItem _item = UserDataAdmin.Instance.SearchUseItem(m_Item.Records2[i].ItemMaster.fix_id);
            if (_item == null)
            {
                continue;
            }

            m_Item.Records2[i].Count = (int)_item.item_cnt;
            m_Item.Records2[i].use_timing = _item.use_timing;
            if (m_Item.Records2[i].Count == 0) removeList.Add(m_Item.Records2[i]);
        }
        for (int i = 0; i < removeList.Count; i++)
        {
            m_Item.Records2.Remove(removeList[i]);
        }

        m_Item.setup();
        m_UpdateCount = 5;
    }

}
