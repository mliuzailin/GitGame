using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class LoginBonusRecordListContext : M4uContext
{
    const float COUNT_SCALE_NORMAL = 0.5f;
    const float COUNT_SCALE_TODAY = 0.65f;
    static readonly Color PREVIOUS_DAY_ICON_COLOR = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    static readonly Color NOT_RECEIVED_COUNT_COLOR = ColorUtil.RGB255Color(80.0f, 80.0f, 80.0f);

    public Action<LoginBonusRecordListContext> DidSelectItem = null;
    public bool IsToday = false;
    public uint date_count = 0;
    public int[] present_ids = null;
    public string message;

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    /// <summary>アイコンイメージ</summary>
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<Color> iconImageColor = new M4uProperty<Color>(Color.white);
    public Color IconImageColor { get { return iconImageColor.Value; } set { iconImageColor.Value = value; } }

    M4uProperty<Sprite> countImage1 = new M4uProperty<Sprite>();
    /// <summary>日数の1桁目の数字</summary>
    public Sprite CountImage1 { get { return countImage1.Value; } set { countImage1.Value = value; } }

    M4uProperty<bool> isViewCountImage1 = new M4uProperty<bool>();
    public bool IsViewCountImage1 { get { return isViewCountImage1.Value; } set { isViewCountImage1.Value = value; } }

    M4uProperty<Sprite> countImage2 = new M4uProperty<Sprite>();
    /// <summary>日数の2桁目の数字</summary>
    public Sprite CountImage2 { get { return countImage2.Value; } set { countImage2.Value = value; } }

    M4uProperty<Vector3> countScale = new M4uProperty<Vector3>();
    public Vector3 CountScale { get { return countScale.Value; } set { countScale.Value = value; } }

    M4uProperty<Color> countColor = new M4uProperty<Color>(Color.white);
    /// <summary>日数の色</summary>
    public Color CountColor { get { return countColor.Value; } set { countColor.Value = value; } }

    M4uProperty<bool> isViewCountImage2 = new M4uProperty<bool>();
    public bool IsViewCountImage2 { get { return isViewCountImage2.Value; } set { isViewCountImage2.Value = value; } }

    /// <summary>
    /// 月間ログインボーナスの表示設定
    /// </summary>
    /// <param name="date"></param>
    /// <param name="login_list"></param>
    public void SetMonthlyLoginState(uint date, uint[] login_list, uint today_date)
    {
        IsToday = (date == today_date);

        //------------------------------------
        // 数字の表示
        //------------------------------------
        float count_scale = COUNT_SCALE_NORMAL;
        List<uint> nums = TextUtil.GetNumberList((int)date_count);
        for (int i = 0; i < nums.Count; ++i)
        {
            Sprite sprite = ResourceManager.Instance.Load(string.Format("login_bonus_{0}", nums[i]), ResourceType.Common);

            if (i == 0)
            {
                CountImage1 = sprite;
                IsViewCountImage1 = true;
            }
            else if (i == 1)
            {
                CountImage2 = sprite;
                IsViewCountImage2 = true;
            }
        }


        if (date > today_date)
        {
            // 今後のアイテムの場合

            IconImageColor = Color.white;
        }
        else if (IsToday)
        {
            // アイコンが当日のものの場合

            IconImageColor = Color.white;
            count_scale = COUNT_SCALE_TODAY;
        }
        else
        {
            // アイコンが当日以前のものの場合

            IconImageColor = PREVIOUS_DAY_ICON_COLOR;

            int index = Array.IndexOf(login_list, date);
            if (index >= 0)
            {
                // すでに受け取っているアイテム
                CountColor = Color.white;
            }
            else
            {
                // 受け取れなかったアイテム
                CountColor = NOT_RECEIVED_COUNT_COLOR;
            }
        }

        CountScale = new Vector3(count_scale, count_scale, count_scale);
    }

    /// <summary>
    /// 特別ログインボーナスの表示設定
    /// </summary>
    /// <param name="date_count"></param>
    /// <param name="login_count"></param>
    public void SetPeriodLoginState(uint date_count, uint login_count)
    {
        IsToday = false;

        if (date_count > login_count)
        {
            //------------------------------------
            // 今後のアイテムの場合、日数は表示しない
            //------------------------------------
            IsViewCountImage1 = false;
            IsViewCountImage2 = false;
            IconImageColor = Color.white;
        }
        else
        {
            // すでに受け取っているアイテム

            //------------------------------------
            // 数字の表示
            //------------------------------------
            List<uint> nums = TextUtil.GetNumberList((int)date_count);
            for (int i = 0; i < nums.Count; ++i)
            {
                Sprite sprite = ResourceManager.Instance.Load(string.Format("login_bonus_{0}", nums[i]), ResourceType.Common);

                if (i == 0)
                {
                    CountImage1 = sprite;
                    IsViewCountImage1 = true;
                }
                else if (i == 1)
                {
                    CountImage2 = sprite;
                    IsViewCountImage2 = true;
                }
            }

            if (date_count == login_count)
            {
                // アイコンが当日のものの場合

                IsToday = true;
                IconImageColor = Color.white;
                float scale = COUNT_SCALE_TODAY;
                CountScale = new Vector3(scale, scale, scale);
            }
            else
            {
                // アイコンが当日以前のものの場合

                IconImageColor = PREVIOUS_DAY_ICON_COLOR;
                float scale = COUNT_SCALE_NORMAL;
                CountScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
