/**
 *  @file   SceneStoryViewTest.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/06
 */

using UnityEngine;
using System.Collections;

public class SceneStoryViewTest : SceneTest<SceneStoryViewTest> {
	public StoryView m_StoryView;
	public uint m_StoryID = 83;

	// Use this for initialization
	protected override void Start() {
        base.Start();
		m_StoryView.gameObject.SetActive(false);

	}

    // Update is called once per frame
    void Update() {

    }

    public override void OnInitialized() {
        base.OnInitialized();
	}

	public void OnClick() {
		// カットインイベントの表示
		StoryView cutin = StoryView.Create();
		cutin.SetScenario(m_StoryID);
		cutin.Show(() => {
			Debug.Log("StoryView Completed");
		});
	}

}
