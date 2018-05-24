using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneDialogTest : Scene<SceneDialogTest>
{
	private bool bOpenDialog = false;
	protected override void Start()
	{
		base.Start();

		//Dialog.Create(DialogType.DialogOK).Show();
		//Dialog.Create(DialogType.DialogYesNo).Show();

	}

	void Update()
	{
		if (SceneCommon.Instance.IsLoadingScene)
		{
			return;
		}

		if (!bOpenDialog)
		{
			Dialog newDialog = Dialog.Create(DialogType.DialogScrollMenu);
			List<DialogMenuItem> menuList = new List<DialogMenuItem>();
			menuList.Add(new DialogMenuItem("ユニット情報", "ユニットダイアログ", OnUnitInfo));
			menuList.Add(new DialogMenuItem("ソート", "ソートダイアログ", OnSortMenu));
			menuList.Add(new DialogMenuItem("スクロール", "スクロールダイアログ", OnScrollText));
            menuList.Add(new DialogMenuItem("スクロール情報", "スクロール情報ダイアログ", OnScrollInfo));
            menuList.Add(new DialogMenuItem("アイコンリスト", "アイコンリストダイアログ", OnIconList));
            newDialog.SetMeneList(menuList);
			newDialog.SetDialogText(DialogTextType.Title, "ダイアログテスト");
			newDialog.Show();
			bOpenDialog = true;
		}
	}



	public void OnSelectQuality(DialogMenuItem _item )
	{

	}

	public void OnUnitInfo(DialogMenuItem _item )
	{
		Dialog newDialog = Dialog.Create(DialogType.DialogUnit);
		newDialog.SetDialogText(DialogTextType.Title, "ユニット情報");
#if UNITY_EDITOR && BUILD_TYPE_DEBUG 
		if (DebugOption.Instance.AutoLoginUser)
		{
			newDialog.setUnitInfo(DialogUnitInfoType.PLAYER, 0);
			//newDialog.setUnitInfo(DialogUnitInfoType.HELPER, 0);
		}
#endif
		newDialog.Show();
	}

    public void OnSortMenu(DialogMenuItem _item ) {
        Dialog newDialog = Dialog.Create(DialogType.DialogSort);
        MainMenuParam.m_DialogSelectSortType = new MAINMENU_SORT_SEQ[]{
                                                                        MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FILTER,
                                                                        MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_DEFAULT,    MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE,	// ID
                                                                        MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LOGIN_TIME, MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RANK,		// 体力
                                                                        MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ELEMENT,    MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND,
                                                                        MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK,     MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP,
                                                                        MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL,      MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE,
                                                                        MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER, MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM
                                                                    };
        newDialog.SetSortListCurrentSortType(MainMenuParam.m_DialogSelectSortType, MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL, OnClickSort);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "SORT_TITLE");
        newDialog.Show();
    }

    void OnClickSort(MAINMENU_SORT_SEQ sortType) {

    }

	void OnScrollText(DialogMenuItem _item) {
		Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
		newDialog.SetDialogText(DialogTextType.Title, "スクロールテキスト");
		newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "HELP_009_DETAIL");
		newDialog.Show();
	}

    void OnScrollInfo(DialogMenuItem _item)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogScrollInfo);
        newDialog.SetDialogText(DialogTextType.Title, "スクロール情報");
        newDialog.AddScrollInfoImage("http://sample/sample.png");
        newDialog.AddScrollInfoText("サンプルテキスト\n\rさんぷるだよ");
        newDialog.AddScrollInfoIconList("たいとるだよ", new uint[] { 2, 3, 4, 5, 6, 7, 8, 9 });
        newDialog.Show();
    }

    void OnIconList(DialogMenuItem _item)
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogIconList);
        newDialog.SetDialogText(DialogTextType.Title, "アイコンリスト");
        for(int i = 0; i < 16; i++)
        {
            DialogIconItem item = new DialogIconItem();
            newDialog.IconList.Add(item);
        }
        newDialog.SetDialogText(DialogTextType.YesText, "はい");
        newDialog.SetDialogText(DialogTextType.NoText, "いいえ");
        newDialog.Show();

    }
}
