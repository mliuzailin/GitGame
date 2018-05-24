/**
 *  @file   SceneTutorialDialogTest.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/01
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SceneTutorialDialogTest : SceneTest<SceneTutorialDialogTest> {
	public int test;
	public int Ans;
	public string stAns;
	public TutorialDialog.FLAG_TYPE m_SelectFlagType;

	protected override void Awake() {
		base.Awake();
		GetComponentInChildren<TutorialDialog>().gameObject.SetActive(false);
		GetComponentInChildren<TutorialDialogPointListItem>().gameObject.SetActive(false);
		GetComponentInChildren<TutorialDialogListItem>().gameObject.SetActive(false);
	}

	// Use this for initialization
	protected override void Start() {
		base.Start();
	}

	// Update is called once per frame
	void Update() {

	}

	public override void OnInitialized() {
		base.OnInitialized();

	}

	public void OnClick() {
		TutorialDialog dialog = TutorialDialog.Create();

		dialog.SetTutorialType(TutorialDialog.FLAG_TYPE.UNIT_PARTY_SELECT);
		dialog.Show(() => {
			Debug.Log("TutorialCarousel Completed");
		});
	}

	public void OnTest() {
		++test;
		Ans = 1 << test;
		Debug.Log(string.Format("TEST{0}:", 19 >> 1));
		stAns = Convert.ToString(Ans, 2);
	}

	public void OnDeleteLocalSave() {
		LocalSaveUtil.ExecDataRemove(LocalSaveManager.LOCALSAVE_TUTORIAL_DIALOG);
	}

	/// <summary>
	/// フラグ数値を出す
	/// </summary>
	public void OnCheckFlag() {
//		string strSaveString = LitJson.JsonMapper.ToJson(LocalSaveManager.Instance.LoadFuncTutorialDialog());
//		Debug.Log(string.Format("Flag:{0}", strSaveString));
	}

	/// <summary>
	/// 指定フラグを立てる
	/// </summary>
	public void OnActiveFlag() {
//		TutorialDialogManager.HideFlag(m_SelectFlagType);
//		string strSaveString = LitJson.JsonMapper.ToJson(LocalSaveManager.Instance.LoadFuncTutorialDialog());
//		Debug.Log(string.Format("Flag:{0}", strSaveString));
	}

	/// <summary>
	/// 指定フラグをチェックする
	/// </summary>
	public void OnCheckFlagType() {
//		Debug.Log(string.Format("Flag:{0}", TutorialDialogManager.CheckFlag(m_SelectFlagType) ? "TRUE" : "FALSE"));
	}

}
