using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAreaSelectTest : Scene<SceneAreaSelectTest>
{
    public AreaSelect areaSelect = null;

    protected override void Start()
    {
        base.Start();
        if (areaSelect)
        {
            areaSelect.BackGroundImage = ResourceManager.Instance.Load("BG_24ku");
            areaSelect.SwitchImage0 = ResourceManager.Instance.Load("btn_map_on");
            areaSelect.SwitchTitle0 = "コウイキMAP";
            areaSelect.SwitchImage1 = ResourceManager.Instance.Load("btn_map_off");
            areaSelect.SwitchTitle1 = "ガクエンMAP";
            areaSelect.SwitchImage2 = ResourceManager.Instance.Load("btn_map_off");
            areaSelect.SwitchTitle2 = "イベントMAP";

            Sprite areaimg = ResourceManager.Instance.Load("btn_24ku1");

            areaSelect.AddAreaData(0, "テスト01", areaimg, new Vector2(116,-630), OnSelectArea);
            areaSelect.AddAreaData(1, "テスト02", areaimg, new Vector2(416,-525), OnSelectArea);
            areaSelect.AddAreaData(2, "テスト03", areaimg, new Vector2(226,-451), OnSelectArea);
            areaSelect.AddAreaData(3, "テスト04", areaimg, new Vector2(316,-958), OnSelectArea);
        }
    }

    public void OnSelectArea( uint _index )
    {

    }
}
