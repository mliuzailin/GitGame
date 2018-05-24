using UnityEngine;
using System.Collections;

public class SceneMasterAssetBundleTest : Scene<SceneMasterAssetBundleTest> {

    protected override void Start() {
        base.Start();
        MasterDataArea area = MasterFinder<MasterDataArea>.Instance.Find(0);
        Debug.LogError("ARREA:" + area.timing_public);
    }

    public override void OnInitialized() {
        base.OnInitialized();


    }
}
