using UnityEngine;
using System.Collections;

public class SceneOthersInfoTest : SceneTest<SceneOthersInfoTest> {
	OthersInfo m_OthersInfo = null;

	// Use this for initialization
	protected override void Start() {
		base.Start();
	}

	// Update is called once per frame
	void Update() {

	}

	public override void OnInitialized() {
		base.OnInitialized();
		m_OthersInfo = GetComponentInChildren<OthersInfo>();

		m_OthersInfo.Infos.Clear();
		for (int i = 0; i < 20; i++) {
			OthersInfoListContext info = new OthersInfoListContext();
			info.CaptionText = "お助け";

			m_OthersInfo.Infos.Add(info);
		}
		

	}
}
