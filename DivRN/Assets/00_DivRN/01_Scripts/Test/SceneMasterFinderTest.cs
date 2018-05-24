using UnityEngine;
using System.Collections;

public class SceneMasterFinderTest : SceneTest<SceneMasterFinderTest>
{
    protected override void Start() {
        base.Start();

    }

    public override void OnInitialized ()
	{
		base.OnInitialized ();

//		string error = "";
//		LoadedAssetBundle ab = AssetBundleManager.GetLoadedAssetBundle ("area0000", out error);
//
//		if (error.IsNOTNullOrEmpty ()) {
//			Debug.LogError ("ERROR:" + error);
//			return;
//		}
//		Debug.Log("AB:"+ ab.m_AssetBundle);
//
//		MasterDataArea area = (ab.m_AssetBundle.mainAsset as MasterDataArea);


//		MasterDataArea area = MasterFinder<MasterDataArea>.Instance.Find (1);

//		Debug.Log ("AREA:" + area.name);

		foreach (MasterDataArea a in MasterFinder<MasterDataArea>.Instance.FindAll()) {
				
			Debug.Log ("AREA_NAME:" + a.area_name);
		}
	}
}
