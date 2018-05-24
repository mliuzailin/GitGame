using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using LitJson;






public class buttonList
{
    //ボタンテキスト
    public string button_text;
    //ボタン実行処理
    public Action func_action;
}

public class SceneDebugSqliteTest : Scene<SceneDebugSqliteTest>
{
    [SerializeField]
    public GameObject buttonPrefab;
    public Canvas localcanvas;
    public uint GetCharaParamFromDrawID_drawID = 1;
    public int GetLimitOverFromID_limit_over_type = 1;
    public string GetTextDefinitionTextFromKey_strKey = "MM_UNDERMSG_QUEST_AREA";
    public uint unQuestID = 5;
    public uint unAreaID = 10;
    public uint unEventID = 1;

    public List<buttonList> allButtonAdd(List<buttonList> button_lists)
    {
        button_lists.Add(new buttonList { button_text = "GetCharaParamFromDrawID", func_action = GetCharaParamFromDrawID });
        button_lists.Add(new buttonList { button_text = "GetLimitOverFromID", func_action = GetLimitOverFromID });
        button_lists.Add(new buttonList { button_text = "GetTextDefinitionTextFromKey", func_action = GetTextDefinitionTextFromKey });
        button_lists.Add(new buttonList { button_text = "GetQuestSerialNumber", func_action = GetQuestSerialNumber });
        button_lists.Add(new buttonList { button_text = "GetMasterDataEventFromID", func_action = GetMasterDataEventFromID });
        button_lists.Add(new buttonList { button_text = "SelectArea", func_action = SelectArea });

        return button_lists;
    }
    protected override void Start()
    {
        base.Start();

        List<buttonList> button_lists = new List<buttonList>();
        List<buttonList> retButtonLists = allButtonAdd(button_lists);
        foreach (buttonList buttonval in retButtonLists)
        {
            GameObject buttonGO = (GameObject)Instantiate(this.buttonPrefab);
            buttonGO.transform.SetParent(localcanvas.transform, false);
            Button button = buttonGO.GetComponent<Button>();
            button.transform.Find("Text").GetComponent<Text>().text = buttonval.button_text;
            button.onClick.AddListener(() => {
                buttonval.func_action();
            });
        }
    }
     // Update is called once per frame
    void Update () {
		
	}

    public void GetCharaParamFromDrawID()
    {
        MasterDataParamChara masterDataParamCharaTmp = MasterDataUtil.GetCharaParamFromDrawID(GetCharaParamFromDrawID_drawID);
        Debug.Log("GetCharaParamFromDrawID(" + GetCharaParamFromDrawID_drawID + ") " + System.Text.RegularExpressions.Regex.Unescape(JsonMapper.ToJson(masterDataParamCharaTmp)));
    }
    public void GetLimitOverFromID()
    {
        MasterDataLimitOver resMasterDataLimitOver = MasterDataUtil.GetLimitOverFromID(GetLimitOverFromID_limit_over_type);
        Debug.Log("GetLimitOverFromID(" + GetLimitOverFromID_limit_over_type + ") " + System.Text.RegularExpressions.Regex.Unescape(JsonMapper.ToJson(resMasterDataLimitOver)));
    }
    public void GetTextDefinitionTextFromKey()
    {
        string resGetTextDefinitionTextFromKey = MasterDataUtil.GetTextDefinitionTextFromKey(GetTextDefinitionTextFromKey_strKey);
        Debug.Log("GetTextDefinitionTextFromKey(" + GetTextDefinitionTextFromKey_strKey + ") " + resGetTextDefinitionTextFromKey);
    }
    public void GetQuestSerialNumber()
    {
        uint unGetSerialNum = 0;
        uint unGetSerialTotal = 0;
        MasterDataUtil.GetQuestSerialNumber( unQuestID,  unAreaID, ref unGetSerialNum, ref unGetSerialTotal);
        Debug.Log("GetQuestSerialNumber(" +  unQuestID.ToString() + " , " + unAreaID.ToString() + ") " + unGetSerialNum.ToString() +" , "+ unGetSerialTotal.ToString());
    }
    public void GetMasterDataEventFromID()
    {
        MasterDataEvent resGetMasterDataEventFromID = MasterDataUtil.GetMasterDataEventFromID( unEventID);
        Debug.Log("GetMasterDataEventFromID(" + unEventID + ") " + System.Text.RegularExpressions.Regex.Unescape(JsonMapper.ToJson(resGetMasterDataEventFromID)));
    }
    public void SelectArea()
    {
        MasterDataDefineLabel.AreaCategory[] areaCategoryArray = new MasterDataDefineLabel.AreaCategory[] { MasterDataDefineLabel.AreaCategory.RN_STORY, MasterDataDefineLabel.AreaCategory.STORY };
        string sqlInString = string.Join(",", Array.ConvertAll<MasterDataDefineLabel.AreaCategory, string>(areaCategoryArray, o => o.ToString("D")));
        MasterDataAreaCategory[] areaCategoryMasterArray = MasterFinder<MasterDataAreaCategory>.Instance.SelectWhere(string.Format(" where area_cate_type in( {0} )", sqlInString)).ToArray();
        Debug.Log("GetMasterDataEventFromID(" + unEventID + ") " + System.Text.RegularExpressions.Regex.Unescape(JsonMapper.ToJson(areaCategoryMasterArray)));
    }
    
}
