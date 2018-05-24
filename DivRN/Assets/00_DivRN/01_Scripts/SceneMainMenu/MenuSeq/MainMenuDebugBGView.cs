using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDebugBGView : MainMenuSeq
{

	DebugBGView m_DebugBGView;

	// Use this for initialization
	protected override void Start()
	{
		base.Start();
	}

	// Update is called once per frame
	public new void Update()
	{
		if (PageSwitchUpdate() == false)
		{
			return;
		}

	}

	protected override void PageSwitchSetting(bool initalize)
	{
		base.PageSwitchSetting(initalize);

		m_DebugBGView = m_CanvasObj.GetComponentInChildren<DebugBGView>();
		m_DebugBGView.setup();
	}
}
