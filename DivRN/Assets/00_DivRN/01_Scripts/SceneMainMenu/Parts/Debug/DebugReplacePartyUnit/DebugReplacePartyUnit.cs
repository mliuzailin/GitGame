/**
 *  @file   DebugReplacePartyUnit.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/12
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;
using M4u;

public class DebugReplacePartyUnit : M4uContextMonoBehaviour
{
    const int UNIT_NUM = 4;

    M4uProperty<List<DebugReplacePartyUnitListItemContext>> partyUnits = new M4uProperty<List<DebugReplacePartyUnitListItemContext>>(new List<DebugReplacePartyUnitListItemContext>());
    public List<DebugReplacePartyUnitListItemContext> PartyUnits
    {
        get
        {
            return partyUnits.Value;
        }
        set
        {
            partyUnits.Value = value;
        }
    }

    int m_UnitListNum = 0;
    PacketStructUnit[] m_DebugReplaceUnit = new PacketStructUnit[(int)GlobalDefine.PartyCharaIndex.FRIEND * 2];
    int m_ReplaceNum = 0;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.fitTopAndBottom(GetComponent<RectTransform>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateList()
    {
        PartyUnits.Clear();

        for (int i = 0; i < m_DebugReplaceUnit.Length; ++i)
        {
            m_DebugReplaceUnit[i] = new PacketStructUnit();
        }

        DebugReplacePartyUnitListItemContext unit;
        for (int i = 0; i < UNIT_NUM; i++)
        {
            unit = new DebugReplacePartyUnitListItemContext();
            unit.BaseUnitData.level = 1;
            unit.LinkUnitData.level = 1;
            unit.BaseUnitData.link_info = (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE;
            unit.LinkUnitData.link_info = (uint)CHARALINK_TYPE.CHARALINK_TYPE_NONE;

            PartyUnits.Add(unit);
        }
    }

    public void OnClick()
    {
        PacketStructPlayer cPlayerInfo = UserDataAdmin.Instance.m_StructPlayer;

        if (cPlayerInfo == null)
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogText(DialogTextType.Title, "ReplaceError");
            newDialog.SetDialogText(DialogTextType.MainText, "NotChangeUnit");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
            newDialog.Show();
            return;
        }

        // リーダーが入っていない場合は処理しない
        if (PartyUnits[0].BaseUnitData.id == 0)
        {
            Dialog newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogText(DialogTextType.Title, "ReplaceError");
            newDialog.SetDialogText(DialogTextType.MainText, "NotChangeLeaderUnit");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
            newDialog.Show();
            return;
        }

        // 追加されたユニットがいる場合
        int nDelCnt = 0;
        int nUnitListMax = cPlayerInfo.unit_list.Length;
        if (m_UnitListNum > 0)
        {
            for (int num = m_UnitListNum; num < nUnitListMax; ++num)
            {
                // 削除
                cPlayerInfo.unit_list[num] = null;
                ++nDelCnt;
            }
        }

        // 所持数を取得
        m_UnitListNum = cPlayerInfo.unit_list.Length - nDelCnt;

        for (int nUnitNum = (int)GlobalDefine.PartyCharaIndex.LEADER; nUnitNum < (int)GlobalDefine.PartyCharaIndex.FRIEND; ++nUnitNum)
        {
            //----------------------------------------
            // ベースユニット設定
            //----------------------------------------

            // ベースキャラが設定されている場合
            if (PartyUnits[nUnitNum].BaseUnitData.id > 0)
            {
                PartyUnits[nUnitNum].BaseUnitData.unique_id = m_UnitListNum + nUnitNum;
            }

            //----------------------------------------
            // リンクユニット設定
            //----------------------------------------
            if (PartyUnits[nUnitNum].LinkUnitData.link_point > CharaLinkUtil.LINK_POINT_MAX)
            {
                PartyUnits[nUnitNum].LinkUnitData.link_point = CharaLinkUtil.LINK_POINT_MAX;
            }

            // リンクキャラが設定されている場合
            if (PartyUnits[nUnitNum].LinkUnitData.id > 0)
            {
                PartyUnits[nUnitNum].LinkUnitData.unique_id = m_UnitListNum + nUnitNum + (int)GlobalDefine.PartyCharaIndex.FRIEND;
                PartyUnits[nUnitNum].LinkUnitData.link_unique_id = m_UnitListNum + nUnitNum;
                PartyUnits[nUnitNum].LinkUnitData.link_info = (uint)CHARALINK_TYPE.CHARALINK_TYPE_LINK;

                PartyUnits[nUnitNum].BaseUnitData.link_point = PartyUnits[nUnitNum].LinkUnitData.link_point;
                PartyUnits[nUnitNum].LinkUnitData.link_point = 0;
                PartyUnits[nUnitNum].BaseUnitData.link_unique_id = PartyUnits[nUnitNum].LinkUnitData.unique_id;
                PartyUnits[nUnitNum].BaseUnitData.link_info = (uint)CHARALINK_TYPE.CHARALINK_TYPE_BASE;
            }

            //----------------------------------------
            // キャラ置き換え
            //----------------------------------------
            ReplaceUnit(PartyUnits[nUnitNum].BaseUnitData, PartyUnits[nUnitNum].LinkUnitData);

        }

        //----------------------------------------
        // 置き換えユニット数をカウント
        //----------------------------------------
        int nReplaceUnitNum = 0;
        int nReplaceUnitMax = m_DebugReplaceUnit.Length;
        for (int num = 0; num < nReplaceUnitMax; ++num)
        {
            if (m_DebugReplaceUnit[num].id == 0)
            {
                continue;
            }
            ++nReplaceUnitNum;
        }

        //----------------------------------------
        // リスト構築
        //----------------------------------------
        PacketStructUnit[] cReplaceUnitList = new PacketStructUnit[m_UnitListNum + nReplaceUnitNum];
        int nListIdx = 0;
        // 所持ユニット分を取得
        for (int num = 0; num < m_UnitListNum; ++num)
        {
            cReplaceUnitList[nListIdx] = cPlayerInfo.unit_list[num];
            ++nListIdx;
        }
        // 置き換えユニット分を取得
        for (int num = 0; num < nReplaceUnitMax; ++num)
        {
            if (m_DebugReplaceUnit[num].id == 0)
            {
                continue;
            }
            cReplaceUnitList[nListIdx] = m_DebugReplaceUnit[num];
            ++nListIdx;
        }

        // ユニットリストを上書き
        cPlayerInfo.unit_list = cReplaceUnitList;

        //----------------------------------------
        // パーティ置き換え
        //----------------------------------------
        // 1stパーティ情報を置き換える
        cPlayerInfo.unit_party_assign[0].unit0_unique_id = m_DebugReplaceUnit[0].unique_id;
        cPlayerInfo.unit_party_assign[0].unit1_unique_id = m_DebugReplaceUnit[1].unique_id;
        cPlayerInfo.unit_party_assign[0].unit2_unique_id = m_DebugReplaceUnit[2].unique_id;
        cPlayerInfo.unit_party_assign[0].unit3_unique_id = m_DebugReplaceUnit[3].unique_id;


        // 1stパーティアサインユニットを置き換える
        UserDataAdmin.Instance.ConvertPartyAssing();

        Dialog replaceOKDialog = Dialog.Create(DialogType.DialogOK);
        replaceOKDialog.SetDialogText(DialogTextType.Title, "1stPartyReplace");
        replaceOKDialog.SetDialogText(DialogTextType.MainText, "UnitChangeOK");
        replaceOKDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button7");
        replaceOKDialog.Show();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ユニットのステータス置き換え
	*/
    //----------------------------------------------------------------------------
    private void ReplaceUnit(PacketStructUnit repunit, PacketStructUnit linkUnit)
    {
        // 最大8体追加する
        int nReplaceNum = m_ReplaceNum;
        m_DebugReplaceUnit[nReplaceNum].id = repunit.id;
        m_DebugReplaceUnit[nReplaceNum].level = repunit.level;
        m_DebugReplaceUnit[nReplaceNum].add_hp = repunit.add_hp;
        m_DebugReplaceUnit[nReplaceNum].add_pow = repunit.add_pow;
        m_DebugReplaceUnit[nReplaceNum].limitbreak_lv = repunit.limitbreak_lv;
        m_DebugReplaceUnit[nReplaceNum].unique_id = repunit.unique_id;
        m_DebugReplaceUnit[nReplaceNum].link_point = repunit.link_point;
        m_DebugReplaceUnit[nReplaceNum].link_unique_id = repunit.link_unique_id;
        m_DebugReplaceUnit[nReplaceNum].link_info = repunit.link_info;

        nReplaceNum += (int)GlobalDefine.PartyCharaIndex.FRIEND;
        m_DebugReplaceUnit[nReplaceNum].id = linkUnit.id;
        m_DebugReplaceUnit[nReplaceNum].level = linkUnit.level;
        m_DebugReplaceUnit[nReplaceNum].add_hp = linkUnit.add_hp;
        m_DebugReplaceUnit[nReplaceNum].add_pow = linkUnit.add_pow;
        m_DebugReplaceUnit[nReplaceNum].unique_id = linkUnit.unique_id;
        m_DebugReplaceUnit[nReplaceNum].link_unique_id = linkUnit.link_unique_id;
        m_DebugReplaceUnit[nReplaceNum].link_info = linkUnit.link_info;

        ++m_ReplaceNum;
    }

}
