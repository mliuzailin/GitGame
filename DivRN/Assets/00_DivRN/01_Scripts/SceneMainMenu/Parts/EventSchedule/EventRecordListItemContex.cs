using UnityEngine;
using System;
using System.Collections;
using M4u;

public class EventRecordListItemContex : M4uContext
{
    public Action<EventRecordListItemContex> DidSelectItem = delegate { };

    // ID
    //public int Id { get; private set; }

    // 識別ID、表示のソート基準
    public int FixId { get; set; }
    public int EventId { get; set; }

    // active
    M4uProperty<bool> active = new M4uProperty<bool>(true);
    public bool Active { get { return active.Value; } set { active.Value = value; } }

    // 分類
    M4uProperty<int> category = new M4uProperty<int>(0);
    public int Category { get { return category.Value; } set { category.Value = value; } }

    // アイコン枠のカラーパレット
    M4uProperty<Color> iconColor = new M4uProperty<Color>();
    public Color IconColor { get { return iconColor.Value; } set { iconColor.Value = value; } }
    // 全体枠のカラーパレット
    M4uProperty<Color> frameColor = new M4uProperty<Color>();
    public Color FrameColor { get { return frameColor.Value; } set { frameColor.Value = value; } }

    // バッジ「New!」表示
    M4uProperty<bool> isNewBadge = new M4uProperty<bool>(false);
    public bool IsNewBadge { get { return isNewBadge.Value; } set { isNewBadge.Value = value; } }

    // バッジ「予約」表示
    M4uProperty<bool> isReserveBadge = new M4uProperty<bool>(false);
    public bool IsReserveBadge { get { return isReserveBadge.Value; } set { isReserveBadge.Value = value; } }

    // テキスト０　アイテム個数　アイコン下部の文字
    M4uProperty<string> captionText00 = new M4uProperty<string>("");
    public string CaptionText00 { get { return captionText00.Value; } set { captionText00.Value = value; } }
    // テキスト１　タイトル
    M4uProperty<string> captionText01 = new M4uProperty<string>("");
    public string CaptionText01 { get { return captionText01.Value; } set { captionText01.Value = value; } }
    // テキスト２　残り日数
    M4uProperty<string> captionText02 = new M4uProperty<string>("");
    public string CaptionText02 { get { return captionText02.Value; } set { captionText02.Value = value; } }


    // 追加パラメータ（後で分割予定）
    public DateTime OpenDate { get; set; }
    public DateTime CloseDate { get; set; }


    // アイコンイメージ
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    // 受領ボタンイメージ
    M4uProperty<Sprite> buttonImage = new M4uProperty<Sprite>();
    public Sprite ButtonImage { get { return buttonImage.Value; } set { buttonImage.Value = value; } }

    public MasterDataDefineLabel.QuestType questType = MasterDataDefineLabel.QuestType.NORMAL;

    public EventRecordListItemContex()
    {
    }

    // 日付情報から期間を所定のフォーマットで返却する
    public string EventDateText()
    {
        string openText = OpenDate.ToString("yyyy/MM/dd");
        string closeText = CloseDate.ToString("yyyy/MM/dd");

        return string.Format("{0}-{1}", openText, closeText);
    }

    // 日付情報から残り時間を所定のフォーマットで返却する
    public string TimeLeftText(DateTime date)
    {
        string result;

        // 現在時刻との差分
        TimeSpan ts = date - System.DateTime.Now;
        //        TimeSpan ts = date - TimeManager.Instance.m_TimeNow;

#if BUILD_TYPE_DEBUG
        //DateTime date_now = TimeManager.Instance.m_TimeNow;
        //Debug.Log("-----m_TimeNow---------" + date_now.Year + "/" + date_now.Month + "/" + date_now.Day);
#endif
        DateTime now = System.DateTime.Now;
#if BUILD_TYPE_DEBUG
        //Debug.Log("---System.DateTime.Now-----" + now.Year + "/" + now.Month + "/" + now.Day);
#endif
        // 期間内
        if (TimeSpan.Zero < ts)
        {
            if (1 < ts.Days)
            {
                // 期間まで、残り1日（24時間）以上
                result = "あと " + ts.Days.ToString() + "日";
            }
            else
            {
                // 期間まで、残り1日（24時間）未満
                result = "あと " + ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00");
            }
        }
        else
        {
            // 期限切れ
            result = "CLOSED";
        }

        // デバッグ用
        return result;
    }

    // イベント開催中か否か
    public int IsEventTime()
    {
        // 現在時刻との差分
        DateTime now = System.DateTime.Now;
        //        DateTime now = TimeManager.Instance.m_TimeNow;
        TimeSpan open_ts = OpenDate - now;
        TimeSpan close_ts = CloseDate - now;

        // 期間内
        if ((open_ts < TimeSpan.Zero) && (TimeSpan.Zero < close_ts))
        {
            return 0;
        }

        else
        {
            if (TimeSpan.Zero < open_ts)
            {
                // 期間前
                return -1;
            }
            else
            {
                // 期間後
                return 1;
            }
        }
    }

    public void ReservedBadge(bool flag)
    {
        IsReserveBadge = flag;
    }

    // ボタンフィードバック
    public void OnClickedButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClickedButton - Id:" + FixId);
#endif
    }
}
