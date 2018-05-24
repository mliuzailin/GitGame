using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuQuestOldArea : MainMenuSeq
{
    private OldAreaSelect m_OldAreaSelect = null;
    private int m_UpdateCount = 0;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (m_UpdateCount != 0)
        {
            m_UpdateCount--;
            if (m_OldAreaSelect) m_OldAreaSelect.updateLayout();
        }

        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        if (m_OldAreaSelect == null)
        {
            m_OldAreaSelect = m_CanvasObj.GetComponentInChildren<OldAreaSelect>();
            setupArea();
        }
    }

    private void setupArea()
    {
        //MasterDataAreaCategory[] categorieDatas = MasterFinder<MasterDataAreaCategory>.Instance.FindAll().FindAll(masterDataAreaCategory => masterDataAreaCategory.area_cate_type == MasterDataDefineLabel.AreaCategory.STORY).ToArray();
        MasterDataAreaCategory[] categorieDatas = MasterFinder<MasterDataAreaCategory>.Instance.SelectWhere(" where area_cate_type = ? ", MasterDataDefineLabel.AreaCategory.STORY).ToArray();


        OldAreaGroupContext newGroup = new OldAreaGroupContext();
        newGroup.Title = "ストーリー";
        newGroup.OldAreaList = new List<OldAreaContext>();
        for (int i = 0; i < categorieDatas.Length; i++)
        {
            if (categorieDatas[i] == null)
            {
                continue;
            }
            //MasterDataArea[] areaDatas = MasterFinder<MasterDataArea>.Instance.FindAll().FindAll(masterDataArea => masterDataArea.area_cate_id == categorieDatas[i].fix_id).ToArray();
            MasterDataArea[] areaDatas = MasterFinder<MasterDataArea>.Instance.SelectWhere(" where area_cate_id = ? ", categorieDatas[i].fix_id).ToArray();
            if (areaDatas.Length == 0)
            {
                continue;
            }

            OldAreaContext newArea = new OldAreaContext();
            newArea.Title = categorieDatas[i].area_cate_name;
            newArea.m_AreaId = categorieDatas[i].fix_id;
            newArea.AreaImage = SceneObjReferMainMenu.Instance.m_MainMenuAtlasArea.GetSprite(areaDatas[0].res_map_icon);
            newArea.DidSelectArea = SelectArea;

            newGroup.OldAreaList.Add(newArea);
        }

        m_OldAreaSelect.OldAreaGroupList.Add(newGroup);

        m_UpdateCount = 5;
    }

    private void SelectArea(uint area_id)
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuParam.SetQuestSelectParam(area_id);
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false);
        }
    }

    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        base.PageSwitchEventEnableBefore();
        //ページ初期化処理
        if (m_OldAreaSelect == null)
        {
            m_OldAreaSelect = m_CanvasObj.GetComponentInChildren<OldAreaSelect>();
            setupArea();
        }

        if (m_UpdateCount != 0) return true;
        return false;
    }
}
