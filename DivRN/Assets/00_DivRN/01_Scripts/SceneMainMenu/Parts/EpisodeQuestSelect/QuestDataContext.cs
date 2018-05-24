using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class QuestDataContext : M4uContext
{
    public enum ExecType
    {
        None = -1,
        Quest = 0,      //旧クエスト
        Quest2,         //新クエスト
        Event,          //イベント
        Max,
    }

    private ListItemModel m_model;
    public ListItemModel model { get { return m_model; } }

    public QuestDataContext(ListItemModel listItemModel)
    {
        m_model = listItemModel;
    }

    public uint area_category_id { get; set; }

    //    M4uProperty<Texture> backGroundTexture = new M4uProperty<Texture>();
    //    public Texture BackGroundTexture { get { return backGroundTexture.Value; } set { backGroundTexture.Value = value; } }

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<uint> index = new M4uProperty<uint>();
    public uint Index { get { return index.Value; } set { index.Value = value; } }

    M4uProperty<bool> isActivePoint = new M4uProperty<bool>();
    public bool IsActivePoint { get { return isActivePoint.Value; } set { isActivePoint.Value = value; } }

    M4uProperty<string> pointLabel = new M4uProperty<string>();
    public string PointLabel { get { return pointLabel.Value; } set { pointLabel.Value = value; } }

    M4uProperty<string> point = new M4uProperty<string>();
    public string Point { get { return point.Value; } set { point.Value = value; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<Sprite> selectImage = new M4uProperty<Sprite>();
    public Sprite SelectImage { get { return selectImage.Value; } set { selectImage.Value = value; } }

    M4uProperty<string> iconLabel = new M4uProperty<string>();
    public string IconLabel { get { return iconLabel.Value; } set { iconLabel.Value = value; } }

    M4uProperty<bool> isActiveFlag = new M4uProperty<bool>();
    public bool IsActiveFlag { get { return isActiveFlag.Value; } set { isActiveFlag.Value = value; } }

    M4uProperty<Sprite> flagImage = new M4uProperty<Sprite>();
    public Sprite FlagImage { get { return flagImage.Value; } set { flagImage.Value = value; } }

    M4uProperty<bool> isActiveNew = new M4uProperty<bool>();
    public bool IsActiveNew { get { return isActiveNew.Value; } set { isActiveNew.Value = value; } }

    M4uProperty<string> amendText = new M4uProperty<string>("");
    public string AmendText { get { return amendText.Value; } set { amendText.Value = value; } }

    public System.Action<uint> DidSelected = delegate { };

    public uint m_QuestId = 0;
    public uint m_Point = 0;
    public ExecType m_QuestType = ExecType.None;
    public MasterDataQuest2 master = null;
    public MasterDataGuerrillaBoss boss = null;

    /// <summary>
    /// フラグの設定
    /// </summary>
    /// <param name="quest_id"></param>
    public void SetFlag(uint quest_id)
    {
        IsActiveNew = false;
        IsActiveFlag = false;
        bool isQuestClear = ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_clear, quest_id);
        bool isMissonComplete = ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_mission_complete, quest_id);
        bool isNew = ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_check, quest_id);

        if (isQuestClear && isMissonComplete)
        {
            // コンプリートフラグの表示
            IsActiveFlag = true;
            FlagImage = ResourceManager.Instance.Load("comp_flag_q");
        }
        else if (isQuestClear)
        {
            // クリアフラグの表示
            IsActiveFlag = true;
            FlagImage = ResourceManager.Instance.Load("clear_flag_q");
        }
        else if (!isNew && m_QuestType != ExecType.Event)
        {
            // NEWフラグの表示
            IsActiveNew = true;
        }
    }
}
