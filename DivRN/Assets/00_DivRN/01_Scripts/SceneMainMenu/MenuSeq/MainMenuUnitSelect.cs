using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class MainMenuUnitSelect : MainMenuSeq
{
    private UnitGridComplex m_UnitGrid = null;

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

        //ページ初期化処理
        if (m_UnitGrid == null)
        {
            //ユニットグリッド取得
            m_UnitGrid = m_CanvasObj.GetComponentInChildren<UnitGridComplex>();
            //サイズ設定
            m_UnitGrid.SetPosition(new Vector2(0, -90), new Vector2(-48, -360));
            //ボタン
            m_UnitGrid.AttchUnitGrid<UnitGridView>(UnitGridView.Create());
        }

        updateUnitList();
    }

    private void updateUnitGrid()
    {
    }

    public void updateUnitList()
    {
        PacketStructUnit[] unitlist = UserDataAdmin.Instance.m_StructPlayer.unit_list;

        List<UnitGridContext> unitList = new List<UnitGridContext>();

        for (int i = 0; i < unitlist.Length; i++)
        {
            var model = new ListItemModel((uint)i);
            UnitGridContext unit = new UnitGridContext(model);
            unit.CharaMasterData = MasterFinder<MasterDataParamChara>.Instance.Find((int)unitlist[i].id);
            unit.UnitData = unitlist[i];
            unit.SetUnitParam(unit.UnitData, unit.CharaMasterData);
            if (CheckUnit(unitlist[i].unique_id))
            {
                unit.UnitIconType = MasterDataDefineLabel.UnitIconType.NONE;
            }
            else
            {
                unit.UnitIconType = MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON;
            }

            if (unitlist[i].link_info != (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
            {
                unit.LinkMark = MainMenuUtil.GetLinkMark(unitlist[i], null);
            }

            unitList.Add(unit);
        }

        m_UnitGrid.CreateList(unitList);
        m_UnitGrid.ClickUnitAction = SelectUnit;
        m_UnitGrid.LongPressUnitAction = SelectUnitLongPress;
    }

    private bool CheckUnit(long _unique_id)
    {
        switch (MainMenuParam.m_UnitSelectType)
        {
            case MAINMENU_UNIT_SELECT_TYPE.BILDUP:
                return CheckBildupUnit(_unique_id);
            case MAINMENU_UNIT_SELECT_TYPE.EVOLVE:
                return CheckEvolveUnit(_unique_id);
            case MAINMENU_UNIT_SELECT_TYPE.LINK_BASE:
                return CheckLinkBaseUnit(_unique_id);
            case MAINMENU_UNIT_SELECT_TYPE.LINK_TARGET:
                return CheckLinkTargetUnit(_unique_id);
        }

        return true;
    }

    private bool CheckBildupUnit(long _unique_id)
    {
        //PacketStructUnit _unit = UserDataAdmin.Instance.SearchChara(_unique_id);
        return true;
    }

    private bool CheckEvolveUnit(long _unique_id)
    {
        PacketStructUnit _unit = UserDataAdmin.Instance.SearchChara(_unique_id);
        //リンクしている・されている
        if (_unit.link_info != (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE) return false;
        //進化先があるか？
        MasterDataParamCharaEvol _masterEvol = MasterDataUtil.GetCharaEvolParamFromCharaID(_unit.id);
        if (_masterEvol == null) return false;

        return true;
    }

    private bool CheckLinkBaseUnit(long _unique_id)
    {
        return true;
    }

    private bool CheckLinkTargetUnit(long _unique_id)
    {
        PacketStructUnit _baseUnit = UserDataAdmin.Instance.SearchChara(MainMenuParam.m_LinkBaseUnitUniqueId);
        PacketStructUnit _targetUnit = UserDataAdmin.Instance.SearchChara(_unique_id);
        //ベースに設定されている
        if (_unique_id == MainMenuParam.m_LinkBaseUnitUniqueId) return false;
        //ベースと同じユニットID
        if (_baseUnit.id == _targetUnit.id) return false;
        // パーティチェック
        if (MainMenuUtil.ChkUnitPartyAssign(_unique_id)) return false;
        //リンクしている・されている
        if (_targetUnit.link_info != (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE) return false;
        return true;
    }

    private void SelectUnit(UnitGridContext _unit)
    {
        if (!MainMenuManager.HasInstance)
        {
            return;
        }

        if (!CheckUnit(_unit.UnitData.unique_id))
        {
            return;
        }

        switch (MainMenuParam.m_UnitSelectType)
        {
            case MAINMENU_UNIT_SELECT_TYPE.BILDUP:
                {
                    MainMenuParam.m_BuildupBaseUnitUniqueId = _unit.UnitData.unique_id;
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false);
                }
                break;
            case MAINMENU_UNIT_SELECT_TYPE.EVOLVE:
                {
                    MainMenuParam.m_EvolveBaseUnitUniqueId = _unit.UnitData.unique_id;
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_EVOLVE, false, false);
                }
                break;
            case MAINMENU_UNIT_SELECT_TYPE.LINK_BASE:
                {
                    MainMenuParam.m_LinkBaseUnitUniqueId = _unit.UnitData.unique_id;
                    PacketStructUnit _baseUnit = UserDataAdmin.Instance.SearchChara(MainMenuParam.m_LinkBaseUnitUniqueId);
                    //リンク情報があるユニットを選択
                    if (_baseUnit.link_info != (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
                    {
                        //リンク解除なのでターゲットも設定

                        //ベースかどうか？
                        if (_baseUnit.link_info == (int)CHARALINK_TYPE.CHARALINK_TYPE_BASE)
                        {
                            //ベースの場合はターゲットにIDを設定
                            MainMenuParam.m_LinkTargetUnitUniqueId = _baseUnit.link_unique_id;
                        }
                        else
                        {
                            //ターゲットの場合はベースのIDを変更
                            MainMenuParam.m_LinkBaseUnitUniqueId = _baseUnit.link_unique_id;
                            MainMenuParam.m_LinkTargetUnitUniqueId = _baseUnit.unique_id;
                        }
                    }
                    else
                    {
                        if (MainMenuParam.m_LinkTargetUnitUniqueId != 0)
                        {
                            PacketStructUnit _targetUnit = UserDataAdmin.Instance.SearchChara(MainMenuParam.m_LinkTargetUnitUniqueId);
                            if (_targetUnit != null)
                            {
                                if (_targetUnit.link_info != (int)CHARALINK_TYPE.CHARALINK_TYPE_NONE)
                                {
                                    MainMenuParam.m_LinkTargetUnitUniqueId = 0;
                                }
                            }
                            else
                            {
                                MainMenuParam.m_LinkTargetUnitUniqueId = 0;
                            }
                        }

                    }
                    //リンクターゲットに設定されているユニット？
                    if (MainMenuParam.m_LinkBaseUnitUniqueId == MainMenuParam.m_LinkTargetUnitUniqueId)
                    {
                        //ターゲットから外す
                        MainMenuParam.m_LinkTargetUnitUniqueId = 0;
                    }
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_LINK, false, false);
                }
                break;
            case MAINMENU_UNIT_SELECT_TYPE.LINK_TARGET:
                {
                    MainMenuParam.m_LinkTargetUnitUniqueId = _unit.UnitData.unique_id;
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_LINK, false, false);
                }
                break;
        }
    }
    private void SelectUnitLongPress(UnitGridContext _unit)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        if (MainMenuManager.HasInstance) MainMenuManager.Instance.OpenUnitDetailInfoPlayer(_unit.UnitData);
    }
}
