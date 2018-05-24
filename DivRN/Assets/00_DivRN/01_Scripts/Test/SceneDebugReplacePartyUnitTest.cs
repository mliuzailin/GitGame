/**
 *  @file   SceneDebugReplacePartyUnitTest.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/12
 */

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class SceneDebugReplacePartyUnitTest : SceneTest<SceneDebugReplacePartyUnitTest>
{
    public DebugReplacePartyUnit m_PartyUnit;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnInitialized()
    {
        base.OnInitialized();
        m_PartyUnit.CreateList();
    }

    public void LoadSceneGame()
    {
        SceneCommon.Instance.ChangeScene(SceneType.SceneQuest2);
    }
}
