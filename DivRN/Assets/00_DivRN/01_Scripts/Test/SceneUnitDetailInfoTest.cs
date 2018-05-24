using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUnitDetailInfoTest : Scene<SceneUnitDetailInfoTest>
{
    public Camera mainCamera = null;
    public uint CharaId = 1;
    private bool m_bCreate = false;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update () {
        if (SceneCommon.Instance.IsLoadingScene)
		{
			return;
		}

        if(!m_bCreate)
        {
            UnitDetailInfo _info = UnitDetailInfo.Create(mainCamera);
            if (_info != null) _info.SetCharaID(CharaId);
            m_bCreate = true;
        }
    }
}
