using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;
using ServerApiTest;
using LitJson;
using System;
using System.Linq;
using System.Reflection;

public class SceneAchievementApiTest : Scene<SceneAchievementApiTest>
{
    protected override void Start()
    {
        base.Start();
    }

    public uint groupId;
    public uint page;
    public int sortType;

    public override void OnInitialized()
    {
        base.OnInitialized();
        Debug.LogError("INITIALIZED");
    }

    public void OnClick()
    {
        OnSendGetAchivement();
    }

    public uint[] aunAchievementOpen;
    public uint[] aunAchievementGroupOpen;

    public void OnSendAchievementOpen()
    {
        ServerDataUtilSend.SendPacketAPI_AchievementOpen(aunAchievementOpen, aunAchievementGroupOpen).
            setSuccessAction((_data) =>
            {
                Debug.LogError("FINISH");
                RecvOpenAchievementValue result = _data.GetResult<RecvOpenAchievement>().result;
            }).
            setErrorAction((_data) =>
            {
                Debug.LogError("ERROR:" + _data.m_PacketCode);
            }).
            SendStart();
    }

    public void OnSendGetAchivement()
    {
        ServerDataUtilSend.SendPacketAPI_GetMasterDataAchievement(groupId, page, sortType).
            setSuccessAction(_data =>
            {
                Debug.LogError("FINISH");
                RecvMasterDataAchievementValue recvMasterDataAchievementValueTest = (RecvMasterDataAchievementValue) _data.GetResult<RecvMasterDataAchievement>().result;
            }).
            setErrorAction(_data =>
            {
                Debug.LogError("ERROR:" + _data.m_PacketCode);
            }).
            SendStart();
    }
}
