using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneUnitGridTest : SceneMainMenu {
    public UnitGridComplex m_UnitGrid;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        // 確認用
        List<UnitGridContext> unitList = new List<UnitGridContext>();
        for (int i = 0; i < 320; ++i)
        {
            var model = new ListItemModel((uint)i);
            UnitGridContext unit = new UnitGridContext(model);
            unitList.Add(unit);

            model.OnClicked += () =>
            {
            };
            model.OnLongPressed += () =>
            {
            };
        }
        m_UnitGrid.CreateList(unitList);

    }

    // Update is called once per frame
    void Update () {
	
	}
}
