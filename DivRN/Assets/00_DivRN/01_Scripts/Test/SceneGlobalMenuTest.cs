using UnityEngine;
using System.Collections;

public class SceneGlobalMenuTest : Scene<SceneGlobalMenuTest>
{
	public GLOBALMENU_TYPE MenuType = GLOBALMENU_TYPE.MAIN_MENU;
    public Camera mainCamera = null;

	private GlobalMenu m_GlobalMenu = null;
	protected override void Start()
	{
		base.Start();

	}

	private void Update()
	{
		if (SceneCommon.Instance.IsLoadingScene)
		{
			return;
		}

		if( m_GlobalMenu == null)
		{
			m_GlobalMenu = GlobalMenu.Create(MenuType, mainCamera);
			m_GlobalMenu.Show();
		}

	}
}
