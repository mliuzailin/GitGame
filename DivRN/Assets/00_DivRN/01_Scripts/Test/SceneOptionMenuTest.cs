using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOptionMenuTest : Scene<SceneOptionMenuTest>
{
    public OptionMenu optionMenu = null;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        if (optionMenu != null)
        {
            string cMessage = "……デフォルト値は　ＯＮです";
            optionMenu.GetOptionItem(OptionMenu.ItemType.BGM).setup(OptionMenu.ItemType.BGM, "BGM", cMessage, true);
            optionMenu.GetOptionItem(OptionMenu.ItemType.SE).setup(OptionMenu.ItemType.SE, "SE", cMessage, true);
            optionMenu.GetOptionItem(OptionMenu.ItemType.GUIDE).setup(OptionMenu.ItemType.GUIDE, "GUIDE", cMessage, true);
            optionMenu.GetOptionItem(OptionMenu.ItemType.TIPS).setup(OptionMenu.ItemType.TIPS, "TIPS", cMessage, true);
            optionMenu.GetOptionItem(OptionMenu.ItemType.VOICE).setup(OptionMenu.ItemType.VOICE, "VOICE", cMessage, true);
            optionMenu.GetOptionItem(OptionMenu.ItemType.SPEED).setup(OptionMenu.ItemType.SPEED, "SPEED", cMessage, true);

            optionMenu.GetOptionItem(OptionMenu.ItemType.NOTIFICATION).setup(OptionMenu.ItemType.NOTIFICATION, "アプリケーションの通知設定", cMessage, false);

            optionMenu.GetOptionItem(OptionMenu.ItemType.NOT_EVENT).setup(OptionMenu.ItemType.NOT_EVENT, "イベント", cMessage, false);
            optionMenu.GetOptionItem(OptionMenu.ItemType.NOT_STAMINA).setup(OptionMenu.ItemType.NOT_STAMINA, "スタミナ", cMessage, false);
            //optionMenu.GetOptionItem(OptionMenu.ItemType.NOT_CASINO).setup(OptionMenu.ItemType.NOT_CASINO, "カジノ", cMessage, false);
            //optionMenu.GetOptionItem(OptionMenu.ItemType.NOT_SEISEKI).setup(OptionMenu.ItemType.NOT_SEISEKI, "聖石殿", cMessage, false);
            //optionMenu.GetOptionItem(OptionMenu.ItemType.NOT_BONUS).setup(OptionMenu.ItemType.NOT_BONUS, "ボーナス", cMessage, false);
            for (int i = (int)OptionMenu.ItemType.NOT_EVENT; i < (int)OptionMenu.ItemType.MAX; i++)
            {
                UnityUtil.SetObjectEnabledOnce(optionMenu.GetOptionItem((OptionMenu.ItemType)i).gameObject, false);
            }

            for (int i = 0; i < (int)OptionMenu.ItemType.MAX; i++)
            {
                optionMenu.GetOptionItem((OptionMenu.ItemType)i).DidSelectItem = OnSelect;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelect(OptionMenu.ItemType _type)
    {
        if (_type == OptionMenu.ItemType.NOTIFICATION)
        {
            if (optionMenu.GetOptionItem(OptionMenu.ItemType.NOTIFICATION).IsSwitch())
            {
                for (int i = (int)OptionMenu.ItemType.NOT_EVENT; i < (int)OptionMenu.ItemType.MAX; i++)
                {
                    optionMenu.GetOptionItem((OptionMenu.ItemType)i).SetSwitch(true);
                    UnityUtil.SetObjectEnabledOnce(optionMenu.GetOptionItem((OptionMenu.ItemType)i).gameObject, true);
                }
            }
            else
            {
                for (int i = (int)OptionMenu.ItemType.NOT_EVENT; i < (int)OptionMenu.ItemType.MAX; i++)
                {
                    optionMenu.GetOptionItem((OptionMenu.ItemType)i).SetSwitch(false);
                    UnityUtil.SetObjectEnabledOnce(optionMenu.GetOptionItem((OptionMenu.ItemType)i).gameObject, false);
                }
            }
        }
    }
}
