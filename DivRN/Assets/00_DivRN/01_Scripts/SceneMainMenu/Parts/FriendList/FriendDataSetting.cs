using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class FriendDataSetting : SortObjectBase
{
    public PacketStructFriend FriendData = null;
    public MasterDataParamChara MasterData = null;
    public CharaOnce CharaOnce = null;

    public System.Action<FriendDataItem> DidSelectIcon = delegate { };
    public System.Action<FriendDataItem> DidSelectFriend = delegate { };

    public FriendDataItem.FlagType m_Flag = FriendDataItem.FlagType.NONE;

    public bool IsEnableButton = false;

    public bool IsActiveLock = false;

    public bool IsViewFriendPoint = false;
}
