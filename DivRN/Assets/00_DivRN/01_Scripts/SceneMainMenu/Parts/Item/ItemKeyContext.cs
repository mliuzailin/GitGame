using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ItemKeyContext : M4uContext
{
    public enum CategoryType
    {
        None = 0,
        FriendPoint,
        UnitPoint,
        ScratchTicket,
        QuestKey,
        Max
    }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<string> name = new M4uProperty<string>();
    public string Name { get { return name.Value; } set { name.Value = value; } }

    M4uProperty<string> count = new M4uProperty<string>();
    public string Count { get { return count.Value; } set { count.Value = value; } }

    M4uProperty<bool> isViewTime = new M4uProperty<bool>();
    public bool IsViewTime { get { return isViewTime.Value; } set { isViewTime.Value = value; } }

    M4uProperty<string> time = new M4uProperty<string>();
    public string Time { get { return time.Value; } set { time.Value = value; } }

    public System.Action<ItemKeyContext> DidSelectItemKey = delegate { };

    public CategoryType Category { get; set; }

    public MasterDataUseItem itemMaster = null;
    public MasterDataQuestKey keyMaster = null;

    //ソート用
    public uint timing_end = 0;

    public void setupIcon(MasterDataUseItem _itemMaster)
    {
        MasterDataUtil.GetItemIcon(
            _itemMaster,
            sprite =>
            {
                IconImage = sprite;
            });
    }
}
