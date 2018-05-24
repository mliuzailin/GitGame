using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using ServerDataDefine;

public class ScoreEventContext : M4uContext
{
    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<string> time = new M4uProperty<string>();
    public string Time { get { return time.Value; } set { time.Value = value; } }

    M4uProperty<string> timeEnd = new M4uProperty<string>();
    public string TimeEnd { get { return timeEnd.Value; } set { timeEnd.Value = value; } }

    M4uProperty<bool> isTimeEnd = new M4uProperty<bool>();
    public bool IsTimeEnd { get { return isTimeEnd.Value; } set { isTimeEnd.Value = value; } }

    M4uProperty<string> areaMessage = new M4uProperty<string>();
    public string AreaMessage { get { return areaMessage.Value; } set { areaMessage.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> hiScoreLabel = new M4uProperty<string>();
    public string HiScoreLabel { get { return hiScoreLabel.Value; } set { hiScoreLabel.Value = value; } }

    M4uProperty<string> hiScore = new M4uProperty<string>();
    public string HiScore { get { return hiScore.Value; } set { hiScore.Value = value; } }

    M4uProperty<string> hiScorePt = new M4uProperty<string>();
    public string HiScorePt { get { return hiScorePt.Value; } set { hiScorePt.Value = value; } }

    M4uProperty<string> nextHiLabel = new M4uProperty<string>();
    public string NextHiLabel { get { return nextHiLabel.Value; } set { nextHiLabel.Value = value; } }

    M4uProperty<string> nextHiScore = new M4uProperty<string>();
    public string NextHiScore { get { return nextHiScore.Value; } set { nextHiScore.Value = value; } }

    M4uProperty<bool> isHiScoreReward = new M4uProperty<bool>();
    public bool IsHiScoreReward { get { return isHiScoreReward.Value; } set { isHiScoreReward.Value = value; } }

    M4uProperty<string> totalScoreLabel = new M4uProperty<string>();
    public string TotalScoreLabel { get { return totalScoreLabel.Value; } set { totalScoreLabel.Value = value; } }

    M4uProperty<string> totalScore = new M4uProperty<string>();
    public string TotalScore { get { return totalScore.Value; } set { totalScore.Value = value; } }

    M4uProperty<string> totalScorePt = new M4uProperty<string>();
    public string TotalScorePt { get { return totalScorePt.Value; } set { totalScorePt.Value = value; } }

    M4uProperty<string> nextTotalLabel = new M4uProperty<string>();
    public string NextTotalLabel { get { return nextTotalLabel.Value; } set { nextTotalLabel.Value = value; } }

    M4uProperty<string> nextTotalScore = new M4uProperty<string>();
    public string NextTotalScore { get { return nextTotalScore.Value; } set { nextTotalScore.Value = value; } }

    M4uProperty<bool> isTotalScoreReward = new M4uProperty<bool>();
    public bool IsTotalScoreReward { get { return isTotalScoreReward.Value; } set { isTotalScoreReward.Value = value; } }

    public System.Action<ScoreEventContext> DidSelectItem = delegate { };
    public PacketStructUserScoreInfo ScoreInfo = null;
}
