using System.Collections;
using System.Collections.Generic;

public class TipSet
{
    public enum DialogTipsText
    {
        PANEL = 0,
        BATTLE2,
        BATTLE3,
        NORMALSKILL1,
        NORMALSKILL2,
        NORMALSKILL3,
        LIMITTIME,
        ENEMYATTACK1,
        ENEMYATTACK2,
        SIXELEMENTS1,
        SIXELEMENTS2,
        RETRY,
        EVOLVEQUEST1,
        EVOLVEQUEST2,
        BOOST,
        SKILLCHECK,
        PASSIVESKILL1,
        PASSIVESKILL2,
        ACTIVESKILL1,
        ACTIVESKILL2,
        ACTIVESKILL3,
        FIRSTATTACK1,
        FIRSTATTACK2,
        BACKATTACK1,
        BACKATTACK2,
        PARTYORGANIZE,
        BUILDPARTNER,
        EVOLVE1,
        EVOLVE2,
        FAVORITE,
        YOURID,
        APPDELETE,
        PLUSUNIT1,
        PLUSUNIT2,
        PANELRECAP1,
        PANELRECAP2,
        SKILLRECAP1,
        SKILLRECAP2,
        SKILLRECAP3,
        PREMIUMFRIEND,
        CASINO,
        VIPAREA,
        GUERRILLABOSS1,
        GUERRILLABOSS2,
        COUNTDOWNSKIP,
        MAX,
    }


    public string title = "";
    public string detail = "";

    public TipSet()
    {
        DialogTipsText rand = (DialogTipsText)UnityEngine.Random.Range(0, (int)DialogTipsText.MAX);
        Initialize((int)rand + 1);
    }

    public TipSet(int index)
    {
        Initialize(index);
    }

    public TipSet AsReadOnly()
    {
        return new TipSet
        {
            title = title,
            detail = detail
        };
    }

    private void Initialize(int index)
    {
        string tmpTitle = string.Format("TIPS_{0:000}_TITLE", index);
        string tmpDetail = string.Format("TIPS_{0:000}_DETAIL", index);
        title = UnityUtil.GetText(tmpTitle);
        detail = UnityUtil.GetText(tmpDetail);
    }
}