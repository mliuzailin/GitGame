/**
 *  @file   ScenePartySelectTest2.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/13
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenePartySelectTest2 : SceneTest<ScenePartySelectTest2> {
	[SerializeField]
	GameObject m_ListItemParefab;
	[SerializeField]
	PartySelectGroup m_PartySelectGroup;
	[SerializeField]
	PartyParamPanel m_PartyParamPanel;
	[SerializeField]
	PartyMemberUnitGroup m_PartyMemberUnitGroup;
    PartyMemberStatusPanel m_PartyMemberStatusPanel;

    public int m_PartyCount;

	protected override void Awake() {
		base.Awake();
		m_ListItemParefab.SetActive(false);
	}


	// Use this for initialization
	protected override void Start() {
		base.Start();
	}

	// Update is called once per frame
	void Update() {

	}

    public override void OnInitialized() {
        base.OnInitialized();

        m_PartyMemberStatusPanel = GetComponentInChildren<PartyMemberStatusPanel>();

        // マスターデータ
        List<MasterDataParamChara> charaMasterDatas = MasterFinder<MasterDataParamChara>.Instance.FindAll();


        // パーティ
        for (int i = 0; i < m_PartyCount; ++i) {
            var model = new PartySelectGroupUnitListItemModel((uint)i);

            PartySelectGroupUnitContext party = new PartySelectGroupUnitContext(model);
            UnitIconImageProvider.Instance.Get(
                charaMasterDatas[Random.Range(0, 1800)].fix_id,
                sprite => { party.UnitImage = sprite; });
            party.NameText = string.Format(GameTextUtil.GetText("questlast_tub"), i + 1);
            m_PartySelectGroup.AddData(party);
            model.OnClicked += () => {
                m_PartySelectGroup.ChangePartyItemSelect(party.Index); // パーティ選択状態を変更
            };

        }

        // パーティメンバー
        for (int i = 0; i < 4; ++i) {
            var unitDataModel = new PartyMemberUnitListItemModel((uint)i);

            PartyMemberUnitContext unit = new PartyMemberUnitContext(unitDataModel);
            UnitIconImageProvider.Instance.Get(
                charaMasterDatas[Random.Range(0, 1800)].fix_id,
                sprite => { unit.UnitImage = sprite; });
            m_PartyMemberUnitGroup.Units.Add(unit);
            unitDataModel.OnClicked += () => {
                unit.DidSelectItem(unit); // TODO : DidSelectItem()の内容と差し替え
            };
            unitDataModel.OnLongPressed += () => {
                unit.DidLongPressItem(unit); // TODO : DidLongPressItem()の内容と差し替え
            };

        }

        // クエストパーティメンバー
        for (int i = 0; i < 5; ++i) {
            var unitDataModel = new PartyMemberUnitListItemModel((uint)i);
            
            PartyMemberUnitContext unit = new PartyMemberUnitContext(unitDataModel);
            UnitIconImageProvider.Instance.Get(
                charaMasterDatas[Random.Range(0, 1800)].fix_id,
                sprite => { unit.UnitImage = sprite; });
            //m_PartyParamQuestPartyPanel.Units.Add(unit);
            unitDataModel.OnClicked += () => {
                unit.DidSelectItem(unit); // TODO : DidSelectItem()の内容と差し替え
            };
            unitDataModel.OnLongPressed += () => {
                unit.DidLongPressItem(unit); // TODO : DidLongPressItem()の内容と差し替え
            };

        }

        for (int i = 0; i < 4; ++i) {
            PartyMemberStatusListItemContext status = new PartyMemberStatusListItemContext();
            m_PartyMemberStatusPanel.UnitStatusParams.Add(status);
        }

	}

}
