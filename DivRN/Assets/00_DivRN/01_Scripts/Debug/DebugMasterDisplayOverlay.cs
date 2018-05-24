using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using M4u;
using Prime31;
using UnityEngine;
using UnityEngine.UI;

public class DebugMasterDisplayOverlay : Overlay<DebugMasterDisplayOverlay>
{
    public M4uProperty<string> content = new M4uProperty<string>();

    public string Content
    {
        get
        {
            return content.Value;
        }
        set
        {
            content.Value = value;
        }
    }

    private EMASTERDATA EMasterData
    {
        get
        {
            return (EMASTERDATA)masterDropDown.value;
        }
    }


    public Dropdown masterDropDown;

    public InputField fixIdInputField;


    protected override void Start()
    {
        base.Start();

        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.fitTopAndBottom(GetComponent<RectTransform>());
        }
    }

    public void setup()
    {
        masterDropDown.ClearOptions();


        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();

        foreach (EMASTERDATA e in Enum.GetValues(typeof(EMASTERDATA)))
        {

            Dropdown.OptionData o = new Dropdown.OptionData(e.ToString());
            list.Add(o);
        }
        list.Sort((a, b) => string.Compare(a.text, b.text));

        masterDropDown.AddOptions(list);
        Refresh();
        fixIdInputField.text = "1";
        SetContent(Convert.ToUInt32(fixIdInputField.text));
    }

    //inputfield入力イベント
    public void OnFixIdInputFieldValueChanged(string v)
    {
        if (Convert.ToUInt32(fixIdInputField.text) > 0)
        {
            SetContent(Convert.ToUInt32(fixIdInputField.text));
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnFixIdInputFieldValueChanged:" + fixIdInputField.text);
#endif
    }

    //プルダウンイベント
    public void OnMasterDropDownValueChanged()
    {
        OnClickFirst();
        string tableName = GetSelectTableName();
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnMasterDropDownValueChanged:" + masterDropDown.value + " TABLE_NAME:" + tableName);
#endif
    }
    public void SetContent(uint fixId)
    {
        string tableName = GetSelectTableName();
        SimpleSQL.SimpleDataTable result = MasterFinder<Master>.Instance.QueryGeneric(string.Format("select * from {0} where fix_id = ? ", tableName), fixId);
        String ContentTmp = "";
        for (int r = 0; r < result.rows.Count; r++)
        {
            for (int c = 0; c < result.columns.Count; c++)
            {
                ContentTmp += result.columns[c].name + " :\n" + result.rows[r][c].ToString() + "\n\n";
            }
        }
        Content = ContentTmp;
        fixIdInputField.text = fixId.ToString();
    }

    public void OnClickNext()
    {
        string tableName = GetSelectTableName();
        SimpleSQL.SimpleDataTable retNextFixId = MasterFinder<Master>.Instance.QueryGeneric(string.Format("select MIN(fix_id) as next_fix_id from {0} where fix_id > ? ", tableName), Convert.ToUInt32(fixIdInputField.text));
        if (retNextFixId != null && retNextFixId.rows[0][0] != null)
        {
            SetContent(Convert.ToUInt32(retNextFixId.rows[0][0]));
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnClickNext");
#endif
    }

    public void OnClickPrev()
    {
        if (Convert.ToUInt32(fixIdInputField.text) > 0)
        {
            string tableName = GetSelectTableName();
            SimpleSQL.SimpleDataTable retPrevtFixId = MasterFinder<Master>.Instance.QueryGeneric(string.Format("select MAX(fix_id) as prevt_fix_id from {0} where fix_id < ? ", tableName), Convert.ToUInt32(fixIdInputField.text));
            if (retPrevtFixId != null && retPrevtFixId.rows[0][0] != null)
            {
                SetContent(Convert.ToUInt32(retPrevtFixId.rows[0][0]));
            }
        }

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnClickPrev");
#endif
    }

    public void OnClickFirst()
    {
        string tableName = GetSelectTableName();
        SimpleSQL.SimpleDataTable retMaxFixId = MasterFinder<Master>.Instance.QueryGeneric(string.Format("select MIN(fix_id) as fix_id from {0}  ", tableName), "");
        if (retMaxFixId != null)
        {
            SetContent(Convert.ToUInt32(retMaxFixId.rows[0][0]));
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnClickFirst");
#endif
    }

    public void OnClickLast()
    {
        string tableName = GetSelectTableName();
        SimpleSQL.SimpleDataTable retMaxFixId = MasterFinder<Master>.Instance.QueryGeneric(string.Format("select MAX(fix_id) as fix_id from {0}  ", tableName), "");
        if (retMaxFixId != null)
        {
            SetContent(Convert.ToUInt32(retMaxFixId.rows[0][0]));
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnClickLast");
#endif
    }

    public void Refresh()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL Refrsesh");
#endif
        Content = "OnCl\nickFirst";
    }

    public void OnClose()
    {
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_DEBUG_MENU, false, false);
    }

    private string GetSelectTableName()
    {
        int select = masterDropDown.value;
        EMASTERDATA emaster = (EMASTERDATA)Enum.Parse(typeof(EMASTERDATA), masterDropDown.options[select].text);
        return emaster.GetTableName();
    }
}
