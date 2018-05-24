using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;

public class SceneFriendListTest : Scene<SceneFriendListTest>
{
    public FriendList friendList = null;
    private bool m_bInit = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (SceneCommon.Instance.IsLoadingScene)
		{
			return;
		}

        if (!m_bInit)
        {
            m_bInit = true;
            PacketStructFriend[] helperList = UserDataAdmin.Instance.m_StructHelperList;
            if (helperList == null)
		    {
			    return;
		    }
            
            for(int i=0;i< helperList.Length; i++)
            {
                if (helperList[i] == null)
		        {
		            continue;
		        }

                FriendDataSetting newFriend = new FriendDataSetting();
                newFriend.FriendData = helperList[i];
                newFriend.MasterData = MasterFinder<MasterDataParamChara>.Instance.Find((int)helperList[i].unit.id);

                newFriend.m_Flag = FriendDataItem.FlagType.NONE;

                newFriend.DidSelectIcon = OnSelectIcon;
                newFriend.DidSelectFriend = OnSelectFriend;
                newFriend.CharaOnce = MainMenuUtil.CreateFriendCharaOnce(newFriend.FriendData);
                newFriend.setSortParamFriend(newFriend.FriendData, newFriend.CharaOnce, newFriend.MasterData);

                friendList.FriendBaseList.Body.Add(newFriend);
            }
            friendList.Init();
        }

    }

    public void OnSelectIcon( FriendDataItem _context )
    {
        Debug.Log("OnSelectIcon");
    }
    public void OnSelectFriend(FriendDataItem _context)
    {
        Debug.Log("OnSelectFriend");
    }
}
