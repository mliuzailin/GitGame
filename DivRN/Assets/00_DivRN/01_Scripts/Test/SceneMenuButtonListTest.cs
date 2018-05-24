using UnityEngine;
using System.Collections;

public class SceneMenuButtonListTest : Scene<SceneMenuButtonListTest>
{
	public MAINMENU_CATEGORY menuCategory = MAINMENU_CATEGORY.UNIT;

	public MenuButtonList buttonList = null;
	private bool bSetup = false;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	void Update()
	{
		if( !bSetup )
		{
			buttonList.setupMenuList(menuCategory);
			bSetup = true;
		}
	}
}
