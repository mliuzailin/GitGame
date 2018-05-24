using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniExtensions;
using ServerDataDefine;

public class SceneSQLiteTest : Scene<SceneSQLiteTest>
{
    protected override void Start()
    {
        base.Start();
//        this.ExecuteLater(
//            0.5f,
//            () =>
//            {
//                SimpleSQL.SimpleDataTable dt = SQLiteClient.Instance.dbManager.QueryGeneric("select * from event_cut_in_master");
//                Debug.LogError("TT:" + dt.rows[0][2].ToString());

        // output the list of weapons
//        // note that we can reference the field/column by number (the order in the SELECT list, starting with zero) or by name
//        outputText.text = "Weapons\n\n";
//        int rowIndex = 0;
//        foreach (SimpleSQL.SimpleDataRow dr in dt.rows)
//        {
//            outputText.text += "Name: '" + dr[1].ToString() + "' " +
//                                "Damage:" + dr["Damage"].ToString() + " " +
//                                "Cost:" + dr[3].ToString() + " " +
//                                "Weight:" + dr["Weight"].ToString() + " " +
//                                "Type:" + dr[6] + "\n";
//
//            rowIndex++;
//        }
//                MasterFinder<MasterDataArea>.Instance.FindAll();
//                Debug.LogError("G:" + MasterFinder<MasterDataGuerrillaBoss>.Instance.Find(0));
//                foreach (MasterDataGuerrillaBoss b in MasterFinder<MasterDataGuerrillaBoss>.Instance.FindAll())
//                {
//                    Debug.LogError("B:" + b.timing_start);
//                }
//            });
        this.ExecuteLater(
            1f,
            () =>
            {
                Dictionary<EMASTERDATA, uint> dict = SQLiteClient.Instance.GetMaxTagIdDict();

                foreach (EMASTERDATA d in dict.Keys)
                {
                    Debug.LogError("K:" + d.ToString());
                    Debug.LogError("K:" + d.ToString() + " V:" + dict[d]);
                }
            });
    }

    public override void OnInitialized()
    {
        base.OnInitialized();
    }
}
