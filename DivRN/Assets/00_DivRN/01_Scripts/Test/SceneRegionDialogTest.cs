using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRegionDialogTest : MonoBehaviour
{

	// Use this for initialization
	void Start () {
        int count = 10;
        MasterDataRegion[] tmpRegionArray = new MasterDataRegion[count];
        for(int i = 0; i < count; i++)
        {
            tmpRegionArray[i] = new MasterDataRegion();
            tmpRegionArray[i].name = "TEST_" + i.ToString();
        }

        RegionDialog.Create()
            .AddRegionList(tmpRegionArray, -1, OnSelect, null)
            .Show();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnSelect( RegionContext context )
    {

    }
}
