using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class PointShop : MenuPartsBase
{
    // メインイメージ
    M4uProperty<Sprite> mainImage = new M4uProperty<Sprite>();
    public Sprite MainImage
    {
        get { return mainImage.Value; }
        set
        {
            mainImage.Value = value;
            IsActiveMainImage = (value != null);
        }
    }
    M4uProperty<bool> isActiveMainImage = new M4uProperty<bool>();
    public bool IsActiveMainImage { get { return isActiveMainImage.Value; } set { isActiveMainImage.Value = value; } }

    // ユニットポイント
    M4uProperty<uint> unitPoint = new M4uProperty<uint>(0);
    public uint UnitPoint
    {
        get { return unitPoint.Value; }
        set
        {
            unitPoint.Value = value;
        }
    }

    // カテゴリ別タイトル文字列
    M4uProperty<string> title0 = new M4uProperty<string>("");
    M4uProperty<string> title1 = new M4uProperty<string>("");
    M4uProperty<string> title2 = new M4uProperty<string>("");
    M4uProperty<string> title3 = new M4uProperty<string>("");
    M4uProperty<string> title4 = new M4uProperty<string>("");
    public string Title0 { get { return title0.Value; } set { title0.Value = value; } }
    public string Title1 { get { return title1.Value; } set { title1.Value = value; } }
    public string Title2 { get { return title2.Value; } set { title2.Value = value; } }
    public string Title3 { get { return title3.Value; } set { title3.Value = value; } }
    public string Title4 { get { return title4.Value; } set { title4.Value = value; } }

    // レコードの追加先
    M4uProperty<List<ProductsListItemContex>> records0 = new M4uProperty<List<ProductsListItemContex>>(new List<ProductsListItemContex>());
    public List<ProductsListItemContex> Records0 { get { return records0.Value; } set { records0.Value = value; } }
    M4uProperty<List<ProductsListItemContex>> records1 = new M4uProperty<List<ProductsListItemContex>>(new List<ProductsListItemContex>());
    public List<ProductsListItemContex> Records1 { get { return records1.Value; } set { records1.Value = value; } }
    M4uProperty<List<ProductsListItemContex>> records2 = new M4uProperty<List<ProductsListItemContex>>(new List<ProductsListItemContex>());
    public List<ProductsListItemContex> Records2 { get { return records2.Value; } set { records2.Value = value; } }
    M4uProperty<List<ProductsListItemContex>> records3 = new M4uProperty<List<ProductsListItemContex>>(new List<ProductsListItemContex>());
    public List<ProductsListItemContex> Records3 { get { return records3.Value; } set { records3.Value = value; } }
    M4uProperty<List<ProductsListItemContex>> records4 = new M4uProperty<List<ProductsListItemContex>>(new List<ProductsListItemContex>());
    public List<ProductsListItemContex> Records4 { get { return records4.Value; } set { records4.Value = value; } }

    M4uProperty<bool> isActiveRecords0 = new M4uProperty<bool>();
    public bool IsActiveRecords0 { get { return isActiveRecords0.Value; } set { isActiveRecords0.Value = value; } }
    M4uProperty<bool> isActiveRecords1 = new M4uProperty<bool>();
    public bool IsActiveRecords1 { get { return isActiveRecords1.Value; } set { isActiveRecords1.Value = value; } }
    M4uProperty<bool> isActiveRecords2 = new M4uProperty<bool>();
    public bool IsActiveRecords2 { get { return isActiveRecords2.Value; } set { isActiveRecords2.Value = value; } }
    M4uProperty<bool> isActiveRecords3 = new M4uProperty<bool>();
    public bool IsActiveRecords3 { get { return isActiveRecords3.Value; } set { isActiveRecords3.Value = value; } }
    M4uProperty<bool> isActiveRecords4 = new M4uProperty<bool>();
    public bool IsActiveRecords4 { get { return isActiveRecords4.Value; } set { isActiveRecords4.Value = value; } }

    private int m_LastUpdateCount = 0;
    private bool m_bReady = false;
    public bool IsReady { get { return (m_bReady && m_LastUpdateCount == 0); } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
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

    public void AddRecord(int id, MasterDataPointShopProduct product)
    {
        var model = new ListItemModel((uint)id);
        var contex = new ProductsListItemContex(id, product, model);

        model.OnClicked += () =>
        {
            OnClickedRecordButton(contex);
        };
        model.OnLongPressed += () =>
        {
            contex.OnClickedButton();
        };

        // レコードの追加先を指定
        switch (product.product_type)
        {
            case MasterDataDefineLabel.PointShopType.ITEM:
                Records0.Add(contex);
                break;
            case MasterDataDefineLabel.PointShopType.UNIT:
                Records1.Add(contex);
                break;
            case MasterDataDefineLabel.PointShopType.UNIT_BUILDUP:
                Records2.Add(contex);
                break;
            case MasterDataDefineLabel.PointShopType.UNIT_EVOL:
                Records3.Add(contex);
                break;
        }
    }

    public void ClearRecord()
    {
        Records0.Clear();
        Records1.Clear();
        Records2.Clear();
        Records3.Clear();
        Records4.Clear();
    }

    // 戻るボタン
    public void OnClickedBackButton()
    {
    }

    // レコードボタンのフィードバック
    public void OnClickedRecordButton(ProductsListItemContex contex)
    {
        // 値段（ユニットポイント）チェック
        if (contex.Price > UnitPoint)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "sh126q_title");
            newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("sh126q_content2"), contex.NameText));
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("OK");
#endif
            }));
            newDialog.Show();

            return;
        }

        //-----------------------------------------------------------
        // アイテム所持数チェック
        //-----------------------------------------------------------
        if (contex.Product.product_type == MasterDataDefineLabel.PointShopType.ITEM)
        {
            PacketStructUseItem[] items = UserDataAdmin.Instance.m_StructPlayer.item_list;
            PacketStructUseItem item = Array.Find(items, (v) => v.item_id == contex.Product.product_param1);

            if (item != null && item.item_cnt >= GlobalDefine.VALUE_MAX_ITEM)
            {
                SoundUtil.PlaySE(SEID.SE_MENU_RET);
                Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "point_upperlimit_item_title");
                newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "point_upperlimit_item_content");
                newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
                {
                }));
                newDialog.Show();

                return;
            }
        }

        //-----------------------------------------------------------
        // ユニット所持数チェック
        //-----------------------------------------------------------
        if (contex.Product.product_type == MasterDataDefineLabel.PointShopType.UNIT
        || contex.Product.product_type == MasterDataDefineLabel.PointShopType.UNIT_EVOL
        || contex.Product.product_type == MasterDataDefineLabel.PointShopType.UNIT_BUILDUP
        || contex.Product.product_type == MasterDataDefineLabel.PointShopType.UNIT_LINK)
        {
            int unitMax = (int)UserDataAdmin.Instance.m_StructPlayer.total_unit;
            int unitTotal = 0;
            if (UserDataAdmin.Instance.m_StructPlayer.unit_list != null)
            {
                unitTotal = UserDataAdmin.Instance.m_StructPlayer.unit_list.Length;
            }

            if (unitTotal >= unitMax)
            {
                bool bUnitFull = (UserDataAdmin.Instance.m_StructPlayer.total_unit >= MasterDataUtil.GetUserUnitMax()); // ユニット所持枠上限
                if (bUnitFull == false)
                {
                    // 有料拡張分も見る
                    bUnitFull = (UserDataAdmin.Instance.m_StructPlayer.extend_unit >= MasterDataUtil.GetMasterDataGlobalParamFromID(GlobalDefine.GLOBALPARAMS_UNIT_MAX_EXTEND));
                }

                SoundUtil.PlaySE(SEID.SE_MENU_RET);

                if (bUnitFull == false)
                {
                    // ユニット所持枠購入上限内
                    Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                    newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "point_upperlimit_unit_title");
                    newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "point_upperlimit_unit_content");
                    newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                    newDialog.SetDialogObjectEnabled(DialogObjectType.VerticalButtonList, true);
                    newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_01"), () =>
                    {
                        //ユニット枠拡張
                        StoreDialogManager.Instance.OpenDialogUnitExtend();
                    });
                    newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_02"), () =>
                     {
                         if (MainMenuManager.HasInstance)
                         {
                             MainMenuParam.m_BuildupBaseUnitUniqueId = 0;
                             MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false);
                         }
                     });
                    newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_03"), () =>
                    {
                        if (MainMenuManager.HasInstance)
                        {
                            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_SALE, false, false);
                        }
                    });
                    newDialog.Show();
                }
                else
                {
                    Dialog newDialog = Dialog.Create(DialogType.DialogOK);
                    newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "point_upperlimit_unit_title");
                    newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "sc256q_content2");
                    newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                    newDialog.SetDialogObjectEnabled(DialogObjectType.VerticalButtonList, true);
                    newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_02"), () =>
                    {
                        if (MainMenuManager.HasInstance)
                        {
                            MainMenuParam.m_BuildupBaseUnitUniqueId = 0;
                            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false);
                        }
                    });
                    newDialog.addVerticalButton(GameTextUtil.GetText("icon_button_03"), () =>
                    {
                        if (MainMenuManager.HasInstance)
                        {
                            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_SALE, false, false);
                        }
                    });
                    newDialog.Show();
                }
                return;
            }
        }

        possible(contex);       // 購入可能

    }

    // 購入確認ダイアログボックス表示
    private void possible(ProductsListItemContex contex)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "sh126q_title");
        string mainFormat = GameTextUtil.GetText("sh126q_content1");
        newDialog.SetDialogText(DialogTextType.MainText, string.Format(mainFormat, contex.NameText, "", string.Format("{0:#,0}", contex.Price), string.Format("{0:#,0}", UnitPoint)));
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");

        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("はい");
#endif
            purchase(contex);       // 購入処理
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("いいえ");
#endif
        }));
        newDialog.Show();
    }

    // 購入処理
    private void purchase(ProductsListItemContex contex)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("購入 SendPacketAPI_PointShopPurchase");
#endif

        MasterDataDefineLabel.PointShopType _itemType = contex.Product.product_type;
        // レコード
        var serverApi = ServerDataUtilSend.SendPacketAPI_PointShopPurchase(contex.Product.fix_id);

        // SendStartの成功時の振る舞い
        serverApi.setSuccessAction(_data =>
        {
            var purchase = _data.GetResult<ServerDataDefine.RecvPointShopPurchase>().result;
            if (purchase == null)
            {
                // 購入失敗
#if BUILD_TYPE_DEBUG
                Debug.Log("購入失敗");
#endif
                return;
            }

#if BUILD_TYPE_DEBUG
            Debug.Log("購入処理");
#endif

            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvPointShopPurchase>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            UserDataAdmin.Instance.ConvertPartyAssing();

            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "sh127q_title");
            string mainText = GameTextUtil.GetText("sh127q_content1");
            if (_itemType == MasterDataDefineLabel.PointShopType.ITEM) mainText += "\n\r" + GameTextUtil.GetText("sh127q_content2");
            _newDialog.SetDialogText(DialogTextType.MainText, mainText);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            _newDialog.SetOkEvent(() =>
            {
                StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
            });
            _newDialog.Show();

            // ユニットポイント表示の更新
            UnitPoint = UserDataAdmin.Instance.m_StructPlayer.have_unit_point;
        });

        // SendStartの失敗時の振る舞い
        serverApi.setErrorAction(_date =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("MASTER_HASH_GET:Error");
#endif
        });

        serverApi.SendStart();
    }

    // 購入不能
    private void notPossible(ProductsListItemContex contex)
    {
    }


    // SceneStart後に実行
    public void SceneStart()
    {
        m_bReady = false;
        MainImage = ResourceManager.Instance.Load("point_shop_top");
        Title0 = GameTextUtil.GetText("pointshop_display3");
        Title1 = GameTextUtil.GetText("pointshop_display7");
        Title2 = GameTextUtil.GetText("pointshop_display4");
        Title3 = GameTextUtil.GetText("pointshop_display5");

        UnitPoint = UserDataAdmin.Instance.m_StructPlayer.have_unit_point;

        // レコード
        var serverApi = ServerDataUtilSend.SendPacketAPI_GetPointShopProduct();
        var products = new List<MasterDataPointShopProduct>();

#if BUILD_TYPE_DEBUG
        //Debug.Log("products");
#endif

        // SendStartの成功時の振る舞い
        serverApi.setSuccessAction(_data =>
        {
            var shop_product = _data.GetResult<ServerDataDefine.RecvGetPointShopProduct>().result.shop_product.ToList();
            products.AddRange(shop_product);
            products.Sort((a, b) => a.priority - b.priority);
#if BUILD_TYPE_DEBUG
            //Debug.Log("MASTER_HASH_GET:Success");
#endif

            ClearRecord();
            for (int id = 0; id < products.Count; id++)
            {
                if (products[id].fix_id == 0)
                {
                    continue;
                }
                AddRecord(id, products[id]);
            }
            // シーンの最後に呼び出す
            PostSceneStart();

        });

        // SendStartの失敗時の振る舞い
        serverApi.setErrorAction(_date =>
        {
            // シーンの最後に呼び出す
            PostSceneStart();
#if BUILD_TYPE_DEBUG
            //Debug.Log("MASTER_HASH_GET:Error");
#endif
        });

        serverApi.SendStart();
    }

    // SceneStart後に実行
    public void PostSceneStart()
    {
        IsActiveRecords0 = (Records0.Count > 0);
        IsActiveRecords1 = (Records1.Count > 0);
        IsActiveRecords2 = (Records2.Count > 0);
        IsActiveRecords3 = (Records3.Count > 0);
        IsActiveRecords4 = (Records4.Count > 0);
        m_LastUpdateCount = 5;
        m_bReady = true;
    }
}
