using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOldAreaSelectTest : Scene<SceneOldAreaSelectTest>
{
    public OldAreaSelect oldAreaSelect = null;
    public Sprite sampleSprite = null;

    private bool m_bInit = false;
    private int m_LayoutUpdateCount = 0;

    public void Update()
    {
        if (SceneCommon.Instance.IsLoadingScene)
		{
			return;
		}

        if (m_LayoutUpdateCount!=0)
        {
            oldAreaSelect.updateLayout();
            m_LayoutUpdateCount--;
        }

        if (!m_bInit)
        {
            m_bInit = true;
            m_LayoutUpdateCount = 5;

            OldAreaGroupContext newGroup = new OldAreaGroupContext();
            newGroup.Title = "ストーリー";
            newGroup.OldAreaList = new List<OldAreaContext>();

            for (int i = 0; i < 9; i++)
            {
                OldAreaContext newArea = new OldAreaContext();
                newArea.Title = "エリア" + i.ToString();
                newArea.AreaImage = sampleSprite;
                newGroup.OldAreaList.Add(newArea);
            }
            oldAreaSelect.OldAreaGroupList.Add(newGroup);
            oldAreaSelect.OldAreaGroupList.Add(new OldAreaGroupContext());
        }
    }
}
