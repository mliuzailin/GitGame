/**
 *  @file   MissonGroupList.cs
 *  @brief  クエストのミッショングループリスト
 *  @author Developer
 *  @date   2016/11/14
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;
using M4u;

public class MissonGroupList : M4uContextMonoBehaviour
{
    /// <summary>
    /// ミッショングループアイテムリスト
    /// </summary>
    M4uProperty<List<MissonGroupListItemContext>> missons = new M4uProperty<List<MissonGroupListItemContext>>(new List<MissonGroupListItemContext>());
    public List<MissonGroupListItemContext> Missons
    {
        get
        {
            return missons.Value;
        }
        set
        {
            missons.Value = value;
        }
    }

    M4uProperty<bool> isActiveEmptyText = new M4uProperty<bool>(false);
    /// <summary>
    /// アイテムがなかった場合のテキストの表示・非表示
    /// </summary>
    public bool IsActiveEmptyText
    {
        get
        {
            return isActiveEmptyText.Value;
        }
        set
        {
            isActiveEmptyText.Value = value;
        }
    }

    /// <summary>タブの切替制御</summary> 
    public MissonGroupListTabSwitcher m_TabSwitcher;

    public Action<MissonGroupListItemContext> OnSelectItemAction = delegate { };
    public Action OnClickSortButtonAction = delegate { };
    public Action OnClickAllGetButtonAction = delegate { };
    public Action<bool> OnValueChangedDefaultAction = delegate { };
    public Action<bool> OnValueChangedLimitedTimeAction = delegate { };

    /// <summary>ソートタイプ</summary> 
    public MAINMENU_SORT_SEQ m_SortType;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        m_TabSwitcher = GetComponentInChildren<MissonGroupListTabSwitcher>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateList(PacketStructAchievementGroup[] packetGroupList)
    {
        if (packetGroupList.Length == 0) { return; }

        Missons.Clear(); // アイテムを空にする

        foreach (PacketStructAchievementGroup packetGroup in packetGroupList)
        {

            //----------------------------------------
            // ミッション表示ステータス決定
            //----------------------------------------
            //初期値は「NEW」にしておく
            MasterDataDefineLabel.AchievementState cState = MasterDataDefineLabel.AchievementState.NEW;
            string strState = "";
            Color cColor = ColorUtil.GetColor(APP_COLOR.LABEL_YELLOW);

            //string stateText 
            if (packetGroup.clear_unacquired_cnt > 0)
            { // （分子＝分母且つ、ステータス２+３のミッション数＞０の場合、または分子≠分母且つ、ステータス２+３のアチーブメント数＞０の場合）
                cState = MasterDataDefineLabel.AchievementState.GET_REWARD; // 報酬未取得の場合「GET REWARD!!」表示
                strState = GameTextUtil.GetText("ACHIEVEMENT_GP_STATE_GET_REWARD");
                cColor = ColorUtil.GetColor(APP_COLOR.LABEL_LIGHT_BLUE);
            }
            else if (packetGroup.list_cnt != packetGroup.clear_cnt)
            { // 分子≠分母且つ、ステータス２+３のミッション数＝０の場合
                cState = MasterDataDefineLabel.AchievementState.NEW; // 「NEW!」表示
                strState = GameTextUtil.GetText("ACHIEVEMENT_GP_STATE_NEW");
                cColor = ColorUtil.GetColor(APP_COLOR.LABEL_YELLOW);
            }
            else if (packetGroup.list_cnt == packetGroup.clear_cnt)
            { //分子＝分母且つ、ステータス２+３のミッション数＝０の場合
                cState = MasterDataDefineLabel.AchievementState.CLEAR; // 「CLEAR」表示
                strState = GameTextUtil.GetText("ACHIEVEMENT_GP_STATE_CLEAR");
                cColor = ColorUtil.GetColor(APP_COLOR.LABEL_PURPLE);
            }


            // 情報設定
            MissonGroupListItemContext group = new MissonGroupListItemContext();
            group.AchievementState = cState;
            group.AchievementStateText = strState;
            group.AchievementStateColor = cColor;
            group.GroupListData = packetGroup;
            group.DetailText = packetGroup.draw_msg;
            group.MissonClearCount = packetGroup.clear_cnt;
            group.MissonMaxCount = packetGroup.list_cnt;
            group.ProgressText = string.Format(GameTextUtil.GetText("MISSION_GROUP_CNT")
                                            , packetGroup.clear_cnt
                                            , packetGroup.list_cnt);
            group.TextColor = (cState == MasterDataDefineLabel.AchievementState.CLEAR)
                            ? ColorUtil.COLOR_GRAY : ColorUtil.COLOR_WHITE;
            group.DidSelectItem += SelectedMissonGroupListItem;
            Missons.Add(group);

        }

        IsActiveEmptyText = (Missons.Count == 0);
    }


    #region ListItem Delegate
    public void SelectedMissonGroupListItem(MissonGroupListItemContext misson)
    {
        OnSelectItemAction(misson);
    }

    #endregion

    #region Button OnClick
    /// <summary>
    /// ソートボタンを押したとき
    /// </summary>
    public void OnClickSortButton()
    {
        OnClickSortButtonAction();
    }

    /// <summary>
    /// 一括受け取りボタンを押したとき
    /// </summary>
    public void OnClickAllGetButton()
    {
        OnClickAllGetButtonAction();
    }
    #endregion

    #region ToggleButton OnValueChanged
    /// <summary>
    /// デフォルトのタブを押したとき
    /// </summary>
    /// <param name="value"></param>
    public void OnValueChangedDefault(bool value)
    {
        OnValueChangedDefaultAction(value);
    }

    /// <summary>
    /// 期間限定のタブを押したとき
    /// </summary>
    /// <param name="value"></param>
    public void OnValueChangedLimitedTime(bool value)
    {
        OnValueChangedLimitedTimeAction(value);
    }
    #endregion

}
