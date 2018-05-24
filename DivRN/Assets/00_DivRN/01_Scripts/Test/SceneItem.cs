using UnityEngine;
using System;
using System.Collections;

public class SceneItem : Scene<SceneItem>
{
	public Item Bind;

    protected override void Start()
    {
        base.Start();

        // ナビゲーションバー
        string[] text = {"スタミナ", "報酬UP", "ガチャ券", "その他", "タブの割り振りは仮です" };
		
		Bind.NaviText0 = text[0];
		Bind.NaviText1 = text[1];
		Bind.NaviText2 = text[2];

        // マスターデータからすべて追加
        MasterDataUseItem [] item_masters = MasterFinder<MasterDataUseItem>.Instance.GetAll();

        // レコード追加
        for (int id = 0; id < item_masters.Length; id++)
        {
            int item_count = 100;

            Bind.AddRecord( Item.ItemType.Stamina, item_masters[id], item_count, 0, OnSelectItem);
        }
        Bind.setup();
    }

    private void OnSelectItem(ItemDataContext _item)
    {

    }
}
