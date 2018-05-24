using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUnitCatalog : MainMenuSeq
{
    private Color WhiteColor = new Color(1.0f, 1.0f, 1.0f, 1f);
    private Color GrayColor = new Color(0.25f, 0.25f, 0.25f, 1f);

    private UnitCatalog m_UnitCatalog = null;

    private int m_TotalCount = 0;

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

        if (ChkUserDataUpdate())
        {
            updateUnitCatalog();
        }
    }

    //ページ初期化処理
    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);


        if (m_UnitCatalog == null)
        {
            m_UnitCatalog = m_CanvasObj.GetComponentInChildren<UnitCatalog>();
            m_UnitCatalog.SetPositionAjustStatusBar(new Vector2(0, 27), new Vector2(0, -325));
        }

        updateUnitCatalog();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.HOME;
    }

    private void updateUnitCatalog()
    {
        m_TotalCount = 0;
        if (m_UnitCatalog.MasterCatalogList.Count == 0)
        {
            //sqlite対応　チェック済み　そのまま残す
            MasterDataParamChara[] charaArray = MasterFinder<MasterDataParamChara>.Instance.GetAll();
            for (int i = 0; i < charaArray.Length; i++)
            {
                if (charaArray[i] == null ||
                    charaArray[i].fix_id == 0)
                {
                    continue;
                }

                bool bGet = false;

                //GETしてるかチェック
                if (IsGetUnit(charaArray[i].fix_id))
                {
                    bGet = true;
                }

                if (bGet) m_TotalCount++;

                UnitCatalogItemContext item = new UnitCatalogItemContext();
                item.Index = (int)charaArray[i].draw_id;
                item.master = charaArray[i];

                if (!bGet)
                {
                    item.IconColor = GrayColor;
                }
                else
                {
                    item.IconColor = WhiteColor;
                }
                /*
                                UnitIconImageProvider.Instance.Get(
                                    charaArray[i].fix_id,
                                    sprite => 
                                    {
                                        item.IconImage = sprite;
                                    });
                */
                item.DidSelectItem = SelectItem;

                m_UnitCatalog.MasterCatalogList.Add(item);
            }
        }
        else
        {
            for (int i = 0; i < m_UnitCatalog.MasterCatalogList.Count; i++)
            {
                int index = i;

                var data = m_UnitCatalog.MasterCatalogList[index];

                bool bGet = false;

                //GETしてるかをチェック
                if (IsGetUnit(data.master.fix_id))
                {
                    bGet = true;
                }

                if (bGet) m_TotalCount++;
                /*
                                UnitIconImageProvider.Instance.Get(
                                    data.master.fix_id,
                                    sprite => 
                                    {
                                        m_UnitCatalog.MasterCatalogList[index].IconImage = sprite;
                                    });
                */
                if (!bGet)
                {
                    data.IconColor = GrayColor;
                }
                else
                {
                    data.IconColor = WhiteColor;
                }
            }
        }

        m_UnitCatalog.Init();

        // ユニット所持数表示
        m_UnitCatalog.CountText = string.Format(GameTextUtil.GetText("zukan_counttext"),
            m_TotalCount,
            m_UnitCatalog.MasterCatalogList.Count);
    }

    private bool IsGetUnit(uint _id)
    {
        if (ServerDataUtil.ChkBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_unit_get, _id) == true) return true;
        return false;
    }

    private bool IsLookUnit(uint _id)
    {
        if (ServerDataUtil.ChkBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_unit_check, _id) == true) return true;
        return false;
    }

    private void SelectItem(uint _id)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.OpenUnitDetailInfoCatalog(_id);
        }
    }
}
