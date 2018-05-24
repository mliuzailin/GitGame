using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUnitCatalogTest : Scene<SceneUnitCatalogTest>
{
    public UnitCatalog unitCatalog = null;
    public Sprite testData = null;

    protected override void Start()
    {
        base.Start();
        MasterDataParamChara[] charaArray = MasterFinder<MasterDataParamChara>.Instance.GetAll();
        for(int i = 0; i < charaArray.Length; i++)
        {
            if (charaArray[i].fix_id == 0)
		    {
		        continue;
		    }
            
            UnitCatalogItemContext _newItem = new UnitCatalogItemContext();
            _newItem.IconImage = testData;
            _newItem.Index = (int)charaArray[i].draw_id;
            unitCatalog.MasterCatalogList.Add(_newItem);
        }
        unitCatalog.Init();
    }

    public void Update()
    {

    }
}
