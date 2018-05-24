/**
 *  @file   SceneSortDialogTest.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/17
 */

using UnityEngine;
using System.Collections;

public class SceneSortDialogTest : SceneTest<SceneSortDialogTest> {
    SortDialog m_SortDialog;
    protected override void Awake() {
        base.Awake();
        GameObject obj = GameObject.Find("Prefab");
        if (obj != null) {
            obj.SetActive(false);
        }

        SortDialog dialog = GetComponentInChildren<SortDialog>();
        if (dialog != null) {
            dialog.gameObject.SetActive(false);
        }
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

    }
    public void OnClickCreate() {
        if (SortDialog.IsExists == true)
        {
            return;
        }

        SortDialog dialog = SortDialog.Create();
        dialog.SetDialogType(SortDialog.DIALOG_TYPE.UNIT);
        dialog.SetSortData(LocalSaveManager.Instance.LoadFuncSortFilterPartyForm());
        dialog.OnCloseAction = OnClickCloseButton;
    }

    void OnClickCloseButton(LocalSaveSortInfo sortInfo) {
        LocalSaveManager.Instance.SaveFuncSortFilterPartyForm(sortInfo);
    }

    public void OnClickDeleteSaveData() {
        LocalSaveUtil.ExecDataRemove(LocalSaveManager.LOCALSAVE_SORT_FILTER_PARTY_FORM);
    }
}
