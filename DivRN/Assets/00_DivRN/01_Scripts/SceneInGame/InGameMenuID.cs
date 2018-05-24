using UnityEngine;
using System.Collections;

public class InGameMenuID : MonoBehaviour {

	// ゲームメニューID定義 
	public const int        GAMEMENU_NONE            = 0;			//!< 無し 
	public const int        GAMEMENU_CLOSE           = 1;			//!< 閉じる
	public const int        GAMEMENU_QUESTINFO       = 2;			//!< 取得情報画面 
	public const int        GAMEMENU_ITEMINFO		 = 3;			//!< 使用中アイテム画面
	public const int        GAMEMENU_OPTION          = 4;			//!< オプション画面 
	public const int        GAMEMENU_RETIRE          = 5;			//!< リタイア画面 
	public const int        GAMEMENU_MAX             = 6;			//!< 最大値 
}
