using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneUnitSaleListTest : Scene<SceneUnitSaleListTest>
{
	public UnitSaleList unitSaleList = null;

	private bool m_Init = false;

	protected override void Awake()
	{
		base.Awake();
	}

	private void Update()
	{
		if (SceneCommon.Instance.IsLoadingScene)
		{
			return;
		}

		if( !m_Init )
		{
#if false
            unitSaleList.UnitList = new List<UnitSaleContext>();
			for(int i = 0; i < 16; i++)
			{
				UnitSaleContext newContext = new UnitSaleContext();
				newContext.UnitImage = ResidentResourceUtil.GetCharaIconEmpty();
				unitSaleList.UnitList.Add(newContext);
			}
#endif
			m_Init = true;
		}
	}

}
