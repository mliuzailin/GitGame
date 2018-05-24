using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using M4u;
using ServerDataDefine;
using UnityEngine;
using UnityEngine.UI;

public class GetMaster2TestContext : M4uContext
{
    private EMASTERDATA emasterData;

    public GetMaster2TestContext(EMASTERDATA e)
    {
        this.emasterData = e;
        Title = e.ToString();
    }

    public M4uProperty<string> title = new M4uProperty<string>();

    public string Title
    {
        get
        {
            return title.Value;
        }
        set
        {
            title.Value = value;
        }
    }


    public void OnClick()
    {
 #if BUILD_TYPE_DEBUG
       Debug.Log("CALL OnClick:" + Title);
#endif
        Dictionary<EMASTERDATA, uint> dict = new Dictionary<EMASTERDATA, uint>();
        dict.Add(emasterData, 0);

        ServerDataUtilSend.SendPacketAPI_GetMasterDataAll2(dict).
            setSuccessAction(
                _data =>
                {
                    RecvMasterDataAll2Value result = _data.GetResult<RecvMasterDataAll2>().result;
                    foreach (FieldInfo fi in typeof(RecvMasterDataAll2Value).GetFields())
                    {
                        object obj = fi.GetValue(result);
                        try
                        {
                            List<int> l = SQLiteClient.Instance.Reflect(obj);
                            if (l.Count >= 2 &&
                                l[1] > 0)
                            {
#if BUILD_TYPE_DEBUG
                                Debug.Log(string.Format("REFLECT {0} u_or_i:{1}件 d:{2}件 \n", DivRNUtil.GetTableName(obj.GetType().ToString()), l[0], l[1]));
#endif
                            }
                        }
                        catch (Exception e)
                        {
#if BUILD_TYPE_DEBUG
                            Debug.LogError("ERROR:" + e.Message + "::" + obj.GetType().ToString());
#endif
                        }
                    }
                }).
            setErrorAction(
                _data =>
                {
#if BUILD_TYPE_DEBUG
                    Debug.LogError("ERROR");
#endif
                }).
            SendStart();
    }
}

public class SceneGetMaster2Test : Scene<SceneGetMaster2Test>
{
    public GridLayoutGroup grid;
    public M4uProperty<List<GetMaster2TestContext>> collection = new M4uProperty<List<GetMaster2TestContext>>();

    public float interval;

    public void OnClickAll()
    {
        StartCoroutine(_CallSeaquence());
    }

    IEnumerator _CallSeaquence()
    {
        foreach (GetMaster2TestContext context in Collection)
        {
            context.OnClick();
            yield return new WaitForSeconds(interval);
        }
        Debug.LogError("FINISH!");
    }

    public List<GetMaster2TestContext> Collection
    {
        get
        {
            return collection.Value;
        }
        set
        {
            collection.Value = value;
        }
    }

    protected override void Start()
    {
        ServerDataUtilSend.SendPacketAPI_UserAuthentication().
            setSuccessAction(
                _data =>
                {
                    //----------------------------------------
                    // ユーザー認証APIでのユーザー認証要請のイレギュラー対応を除去。
                    // ここで外しておかないと他のAPIのセッション切れがスルーされるので確実に切っておく
                    //----------------------------------------
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED);
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2);

                    UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvUserAuthentication>((PacketStructPlayer) UserDataAdmin.Instance.m_StructPlayer);
                    UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvUserAuthentication>().result.hero_list;
                    UserDataAdmin.Instance.ConvertPartyAssing();
                    Collection = new List<GetMaster2TestContext>();
                    foreach (EMASTERDATA e in Enum.GetValues(typeof(EMASTERDATA)))
                    {
                        GetMaster2TestContext context = new GetMaster2TestContext(e);

                        Collection.Add(context);
                    }
                }).
            setErrorAction(
                _data =>
                {
                    Debug.LogError("AUTH_ERROR");
                    //----------------------------------------
                    // ユーザー認証APIでのユーザー認証要請のイレギュラー対応を除去。
                    // ここで外しておかないと他のAPIのセッション切れがスルーされるので確実に切っておく
                    //----------------------------------------
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED);
                    ServerDataManager.Instance.ResultCodeDelIrregular(SERVER_API.SERVER_API_USER_AUTHENTICATION, API_CODE.API_CODE_USER_AUTH_REQUIRED2);
                }).
            SendStart();
    }
}
