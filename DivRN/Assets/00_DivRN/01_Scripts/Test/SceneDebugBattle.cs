using UnityEngine;
using System.Collections;

public class SceneDebugBattle : Scene<SceneDebugBattle> {

	// Use this for initialization
	protected override void Start () {
	
		base.Start();
		//----------------------------------------
		// パッチの再読み込み発行
		//----------------------------------------
//		if (Patcher.Instance != null)
//		{
//			Patcher.Instance.PatchUpdateRequest(true, true);
//		}
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
