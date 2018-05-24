using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDebugMapIconView : MainMenuSeq
{

	DebugMapIconView m_DebugMapIconView;

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

		m_DebugMapIconView = m_CanvasObj.GetComponentInChildren<DebugMapIconView>();
		m_DebugMapIconView.setup();
	}

	public override bool PageSwitchEventDisableBefore()
	{
		AssetBundlerResponse.clearAssetBundleChash();
		return base.PageSwitchEventDisableBefore();
	}
}
