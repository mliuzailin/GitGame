using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScoreEventDialogTest : MonoBehaviour
{
    public Camera mainCamera = null;
	// Use this for initialization
	void Start () {
        ScoreEventDialog newDialog = ScoreEventDialog.Create(mainCamera);

        ScoreEventContext scoreEvent1 = new ScoreEventContext();
        scoreEvent1.Title = "テストイベントです";
        scoreEvent1.Time = "開催期間：2017/10/10 11:00～10/24 10:59";
        scoreEvent1.HiScoreLabel = "ハイスコア";
        scoreEvent1.HiScore = "123123123";

        newDialog.EventList.Add(scoreEvent1);
        newDialog.EventList.Add(scoreEvent1);
        newDialog.EventList.Add(scoreEvent1);
        newDialog.EventList.Add(scoreEvent1);

        newDialog.Show();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
