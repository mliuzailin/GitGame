using UnityEngine;
using System.Collections;

public class SceneFriendSearchTest : SceneMainMenu {
    public FriendSearch m_FriendSearch;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        m_FriendSearch.SelfIDText = "111.111.111";
    }

    // Update is called once per frame
    void Update() {

    }
}
