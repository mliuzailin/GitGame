using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEpisodeQuestSelectTest : Scene<SceneEpisodeQuestSelectTest>
{
    public EpisodeQuestSelect episodeQuestSelect = null;
    public Sprite BGSprite = null;
    public int AreaCount = 7;
    public int QuestCount = 10;

    protected override void Start()
    {
        base.Start();

        episodeQuestSelect.SetPosition(new Vector2(0, -44), new Vector2(0, -251));

//        episodeQuestSelect.BackGroundImage = BGSprite;
        episodeQuestSelect.EpisodeTitle = "エピソード名";
        episodeQuestSelect.AreaTitle = "エリア名";

        Sprite select1 = ResourceManager.Instance.Load("icon_circle_1");
        Sprite select2 = ResourceManager.Instance.Load("icon_circle_2");

        for (int i = 0; i < AreaCount; i++)
        {
            uint index = (uint)i;
            var model = new EpisodeDataListItemModel(index);
            model.OnClicked += () =>
            {

            };

            EpisodeDataContext newEpisode = new EpisodeDataContext(model);
            if (i == 0)
            {
                newEpisode.SelectImage = select2;
                newEpisode.IsSelected = true;
            }
            else
            {
                newEpisode.SelectImage = select1;
                newEpisode.IsSelected = false;
            }
            newEpisode.m_EpisodeId = index;
            episodeQuestSelect.EpisodeList.Add(newEpisode);
        }

        for (int i = 0; i < QuestCount; i++)
        {
            uint index = (uint)i;
            var model = new ListItemModel(index);
            model.OnClicked += () =>
            {

            };

            QuestDataContext newQuest = new QuestDataContext(model);
            //cnewQuest.BackGroundTexture = BGSprite.texture;
            newQuest.Title = "クエスト" + index.ToString();
            newQuest.Index = index;
            newQuest.Point = ((uint)(index * 5) + 5).ToString();
            newQuest.SelectImage = select2;
            newQuest.IconLabel = "BOSS";
            newQuest.m_QuestId = index;
            episodeQuestSelect.QuestList.Add(newQuest);

            // TODO : 演出を入れるならその場所に移動
            model.Appear();
            model.SkipAppearing();
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
