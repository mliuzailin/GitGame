/**
 *  @file   OthersInfoListContext.cs
 *  @brief
 *  @author Developer
 *  @date   2017/03/06
 */

using UnityEngine;
using System;
using System.Collections;
using M4u;

public class OthersInfoListContext : M4uContext
{
    public enum InfoType
    {
        NONE = 0,
        DIALOG,
        SCROLL_DIALOG,
        TUTORIAL_DIALOG,
        WEB_VIEW,
        MOVIE,
        GENERAL_WINDOW,
    }
    /// <summary>アイテムを選択したときのアクション</summary>
    public Action<OthersInfoListContext> DidSelectItem = delegate { };

    /// <summary>タイトルのキー</summary>
    public string HelpTitleKey;

    /// <summary>ダイアログの内容のキー</summary>
    public string HelpDetailKey;

    /// <summary>URL</summary>
    public string URL;

    /// <summary>チュートリアルダイアログのタイプ</summary>
    public TutorialDialog.FLAG_TYPE TutorialDialogType = TutorialDialog.FLAG_TYPE.NONE;

    /// <summary>ムービー名</summary>
    public string MovieName;

    public uint ID;

    public InfoType Type;

    M4uProperty<string> captionText = new M4uProperty<string>();
    /// <summary>表示テキスト</summary>
    public string CaptionText
    {
        get
        {
            return captionText.Value;
        }
        set
        {
            captionText.Value = value;
        }
    }

    public OthersInfoListContext()
    {

    }

    /// <summary>
    /// ダイアログ表示用初期化
    /// </summary>
    /// <param name="strTitleKey"></param>
    /// <param name="strDetailKey"></param>
    /// <param name="onSlectAction"></param>
    public static OthersInfoListContext CreateDialogInfo(string strTitleKey, string strDetailKey, Action<OthersInfoListContext> onSlectAction)
    {
        OthersInfoListContext info = new OthersInfoListContext();
        info.Type = InfoType.DIALOG;
        info.HelpTitleKey = strTitleKey;
        info.HelpDetailKey = strDetailKey;
        info.DidSelectItem = onSlectAction;

        info.CaptionText = UnityUtil.GetText(strTitleKey);

        return info;
    }

    /// <summary>
    /// ダイアログ表示用初期化
    /// </summary>
    /// <param name="strTitleKey"></param>
    /// <param name="strDetailKey"></param>
    /// <param name="onSlectAction"></param>
    public static OthersInfoListContext CreateScrollDialogInfo(string strTitleKey, string strDetailKey, Action<OthersInfoListContext> onSlectAction)
    {
        OthersInfoListContext info = new OthersInfoListContext();
        info.Type = InfoType.SCROLL_DIALOG;
        info.HelpTitleKey = strTitleKey;
        info.HelpDetailKey = strDetailKey;
        info.DidSelectItem = onSlectAction;

        info.CaptionText = UnityUtil.GetText(strTitleKey);

        return info;
    }

    /// <summary>
    /// リンク表示用初期化
    /// </summary>
    /// <param name="captionKey"></param>
    /// <param name="url"></param>
    /// <param name="onSlectAction"></param>
    /// <returns></returns>
    public static OthersInfoListContext CreateWebViewWithCaptionKey(string captionKey, string url, Action<OthersInfoListContext> onSlectAction)
    {
        OthersInfoListContext info = new OthersInfoListContext();
        info.Type = InfoType.WEB_VIEW;
        info.CaptionText = UnityUtil.GetText(captionKey);
        info.URL = url;
        info.DidSelectItem = onSlectAction;

        return info;
    }

    /// <summary>
    /// 機能チュートリアル表示用初期化
    /// </summary>
    /// <param name="captionKey"></param>
    /// <param name="type"></param>
    /// <param name="onSlectAction"></param>
    /// <returns></returns>
    public static OthersInfoListContext CreateTutorialDialog(string captionKey, TutorialDialog.FLAG_TYPE type, Action<OthersInfoListContext> onSlectAction)
    {
        OthersInfoListContext info = new OthersInfoListContext();
        info.Type = InfoType.TUTORIAL_DIALOG;
        info.CaptionText = UnityUtil.GetText(captionKey);
        info.TutorialDialogType = type;
        info.DidSelectItem = onSlectAction;

        return info;
    }

    /// <summary>
    /// ムービー表示用初期化
    /// </summary>
    /// <param name="captionKey"></param>
    /// <param name="type"></param>
    /// <param name="onSlectAction"></param>
    /// <returns></returns>
    public static OthersInfoListContext CreateMovieInfo(string captionKey, string movie_name, Action<OthersInfoListContext> onSlectAction)
    {
        OthersInfoListContext info = new OthersInfoListContext();
        info.Type = InfoType.MOVIE;
        info.CaptionText = UnityUtil.GetText(captionKey);
        info.MovieName = movie_name;
        info.DidSelectItem = onSlectAction;

        return info;
    }

    /// <summary>
    /// 汎用ウィンドウ表示用初期化
    /// </summary>
    /// <param name="captionKey"></param>
    /// <param name="group_id"></param>
    /// <param name="onSlectAction"></param>
    /// <returns></returns>
    public static OthersInfoListContext CreateGeneralWindowInfo(string captionKey, uint group_id, Action<OthersInfoListContext> onSlectAction)
    {
        OthersInfoListContext info = new OthersInfoListContext();
        info.Type = InfoType.GENERAL_WINDOW;
        info.CaptionText = UnityUtil.GetText(captionKey);
        info.ID = group_id;
        info.DidSelectItem = onSlectAction;

        return info;
    }

}
