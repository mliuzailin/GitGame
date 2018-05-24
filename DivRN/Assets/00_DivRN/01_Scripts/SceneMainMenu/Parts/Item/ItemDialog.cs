using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ItemDialog : M4uContextMonoBehaviour
{
    // 文字列
    M4uProperty<string> title = new M4uProperty<string>("");
    M4uProperty<string> caption = new M4uProperty<string>("");
    M4uProperty<string> text0 = new M4uProperty<string>("");
    M4uProperty<string> text1 = new M4uProperty<string>("");
    public string Title { get { return title.Value; } set { title.Value = value; } }
    public string Caption { get { return caption.Value; } set { caption.Value = value; } }
    public string Text0 { get { return text0.Value; } set { text0.Value = value; } }
    public string Text1 { get { return text1.Value; } set { text1.Value = value; } }

    // アイコンイメージ
    M4uProperty<Sprite> image0 = new M4uProperty<Sprite>();
    M4uProperty<Sprite> image1 = new M4uProperty<Sprite>();
    public Sprite Image0 { get { return image0.Value; } set { image0.Value = value; } }
    public Sprite Image1 { get { return image1.Value; } set { image1.Value = value; } }

    // ボタンの背景画像
    public Sprite[] ButtonImages;

    // アイテム使用可能
    bool isEnableItem;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {

    }


#if true
    // アイテム使用ボタン
    public void OnClickUsedItem(int id, string dateText, int item_num, int used_num)
    {
        usedItem(id, dateText, item_num, used_num);
    }

    // アイテム使用
    public void usedItem(int id, string dateText, int item_num, int used_num)
    {
        string format = "使用したアイテムIDは{0}です" + "\n"
                        + "所持個数は{1}個で、１度に{2}個消費します" + "\n"
                        + "減らないのは消費0個のアイテムです" + "\n"
                        + "使用期限の{3}迄に{4}回の使用が可能です" + "\n"
                        + "" + "\n"
                        + "本当に使用しますか？";

        string mainText = string.Format(format,
                                id,         // アイテムID
                                item_num,   // 所持数
                                used_num,   // 消費個数
                                dateText,   // 使用期限(文字列)
                                used_num == 0 ? "無限" : (item_num / used_num).ToString());	// 使用回数

        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo);
        newDialog.SetDialogText(DialogTextType.Title, "確認");
        newDialog.SetDialogText(DialogTextType.MainText, mainText);
        newDialog.SetDialogText(DialogTextType.YesText, "OK");
        newDialog.SetDialogText(DialogTextType.NoText, "CANCEL");

        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("OK");
#endif
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("CANCEL");
#endif
        }));
        newDialog.Show();
    }

#else

    // ダイアログのメッセージテキストをセット（デバッグ用）
    public void SetMessageDebug(int id, string dateText, int item_num, int used_num)
	{
		isEnableItem = used_num <= item_num ? true : false;
		
		Title = "確認";
		
		string format = "使用したアイテムIDは{0}です" + "\n"
						+ "所持個数は{1}個で、１度に{2}個消費します" + "\n"
						+ "減らないのは消費0個のアイテムです" + "\n"
						+ "使用期限の{3}迄に{4}回の使用が可能です" + "\n"
						+ "" + "\n"
						+ "本当に使用しますか？";
		
		Caption = string.Format (format,
								id,			// アイテムID
								item_num,	// 所持数
								used_num,	// 消費個数
								dateText,	// 使用期限(文字列)
								used_num　== 0 ? "無限" : (item_num / used_num).ToString());	// 使用回数
								
		Text0 = "OK";
		Text1 = "CANCEL";
		
		Image0 = ButtonImages[0];
		Image1 = ButtonImages[1];
	}
#endif


    // ダイアログのメッセージテキストをセット
    public void Message(int id, string dateText, int item_num, int used_num)
    {
        isEnableItem = used_num <= item_num ? true : false;

        // アイテム個数判定
        if (isEnableItem)
            MessageEnable();
        else
            MessageDisnable();
    }

    public void MessageEnable()
    {
        Title = "アイテム使用確認";

        Caption = "アイテムを使用します。" + "\n"
                        + "\n"
                        + "アイテムによっては効果を上書きしたり、回復があふれる場合があるかもしれません。" + "\n"
                        + "もしくは、期待した効果が得られない場合もあるかと思います。" + "\n"
                        + "\n"
                        + "用法容量を守って、正しくお使いください。";

        Text0 = "いいえ";
        Text1 = "はい";

        Image0 = ButtonImages[0];
        Image1 = ButtonImages[1];
    }

    // ダイアログのメッセージテキストをセット
    public void MessageDisnable()
    {
        Title = "アイテム使用に関するメッセージ";

        Caption = "アイテムが使用できません" + "\n"
                        + "\n"
                        + "お買いものに行きますか？";

        Text0 = "もどる";
        Text1 = "ショップ";

        Image0 = ButtonImages[0];
        Image1 = ButtonImages[2];
    }

    // ダイアログのメッセージテキストをセット
    public void Message(string title, string caption, string text0 = "")
    {
        Title = title;
        Caption = caption;
        Text0 = text0;
        Text1 = "";
        Image0 = ButtonImages[0];
    }

    // クリック時のフィードバック
    public void OnClickButton(int i)
    {
        // 現状はどちらも閉じるのみ
        Destroy(gameObject);
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClick DialogButton:" + i);
#endif
    }
}
