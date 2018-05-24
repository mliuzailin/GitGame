using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ServerDataDefine;
using LitJson;
using System;
using StrOpe = StringOperationUtil.OptimizedStringOperation;

public class ServerApi : MonoBehaviour
{
    private PacketData m_PackData = new PacketData();
    private ResultCodeType m_Result = null;

    public class ResultData
    {
        public uint m_PacketUniqueNum;      //!< パケット情報：パケット固有ユニークID
        public SERVER_API m_PacketAPI;          //!< パケット情報：パケットAPIタイプ

        public API_CODE m_PacketCode;           //!< パケットリザルト情報：リザルトコード
        private RecvDataBase m_PacketResult = null;    //!< リザルトデータ
        private ICollection m_PacketResultPlayerColumnList = null; //!< リザルトPlayerデータ項目配列
        public T GetResult<T>() where T : RecvDataBase
        {
            return (T)m_PacketResult;
        }

        public void SetResult<T>(T _data) where T : RecvDataBase
        {
            m_PacketResult = _data;
        }

        //UpdateStructPlayer用
        public ICollection GetResultPlayerColumnList()
        {
            return (ICollection)m_PacketResultPlayerColumnList;
        }

        public void SetResultPlayerColumnList(ICollection _data)
        {
            m_PacketResultPlayerColumnList = _data;
        }
        public void SetPlayerResult<T>(string jsonText) where T : RecvDataBase
        {
            //初期化
            SetResultPlayerColumnList(null);
            //普通にjsonをパースしてm_PacketResultに設定
            SetResult<T>(JsonMapper.ToObject<T>(jsonText));
            //T型にplayerオブジェクトがあるか？
            if (null != typeof(T).GetField("result") &&
                null != typeof(T).GetField("result").GetValue(m_PacketResult).GetType().GetField("player"))
            {
                //JsonData型にパース
                JsonData playerColumnList = JsonMapper.ToObject(jsonText);
                //パース後のJsonData型にplayerがあるか？
                if (true == (playerColumnList as IDictionary).Contains("result") &&
                    true == (playerColumnList["result"] as IDictionary).Contains("player") &&
                    playerColumnList["result"]["player"] != null)
                {
                    //jsonのplayerのカラム名配列を設定
                    SetResultPlayerColumnList((playerColumnList["result"]["player"] as IDictionary).Keys);
                }
                //リザルトデータのplayer上書き
                //typeof(T).GetField("result").GetValue(GetResult<T>()).GetType().GetField("player").SetValue(typeof(T).GetField("result").GetValue(GetResult<T>()), UpdateStructPlayer<T>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer));
            }
        }
        public PacketStructPlayer UpdateStructPlayer<T>(PacketStructPlayer structPlayer) where T : RecvDataBase
        {
            if (null == structPlayer)
            {
                structPlayer = new PacketStructPlayer();
            }

            //レシーブplayerオブジェクトのカラム名配列取得
            ICollection PlayerColumnList = GetResultPlayerColumnList();
            if (null == PlayerColumnList)
            {
                //更新データないから何もしない
                return (PacketStructPlayer)structPlayer;
            }

            //<T>にplayerオブジェクトがあるか？
            if (null == typeof(T).GetField("result") &&
                null == typeof(T).GetField("result").GetValue(GetResult<T>()).GetType().GetField("player"))
            {
                //playerがない
                return (PacketStructPlayer)structPlayer;
            }

            //playerオブジェクト取得
            var result = typeof(T).GetField("result").GetValue(GetResult<T>());
            PacketStructPlayer playerValue = (PacketStructPlayer)result.GetType().GetField("player").GetValue(result);

            //カラム名が存在するものだけAPIから受け取った値に更新する
            foreach (string PlayerColumnName in PlayerColumnList)
            {
                //PacketStructPlayer型に存在するカラム名か？
                FieldInfo PlayerFieldInfo = typeof(PacketStructPlayer).GetField(PlayerColumnName);
                if (null != PlayerFieldInfo)
                {
                    //存在するカラム名なのでstructPlayerの値をm_PacketResultの値に更新する
                    PlayerFieldInfo.SetValue(structPlayer, PlayerFieldInfo.GetValue(playerValue));

                    //unit追加、更新、
                    if (PlayerColumnName == "unit_list")
                    {
                        UserDataAdmin.Instance.m_cureate_unit_list = true;
                    }

                    if (PlayerColumnName == "add_unit_list")
                    {
                        //unit追加
                        PacketStructUnit[] add_unit_list = (PacketStructUnit[])PlayerFieldInfo.GetValue(playerValue);
                        PacketStructUnit[] merged_unit_list = new PacketStructUnit[structPlayer.unit_list.Length + add_unit_list.Length];
                        structPlayer.unit_list.CopyTo(merged_unit_list, 0);
                        add_unit_list.CopyTo(merged_unit_list, structPlayer.unit_list.Length);

                        FieldInfo PlayerFieldInfoUnitList = typeof(PacketStructPlayer).GetField("unit_list");
                        PlayerFieldInfoUnitList.SetValue(structPlayer, merged_unit_list);

                        // パラーメータ作成用
                        long[] add_unit_uni_id_list = new long[add_unit_list.Length];
                        for (int i = 0; i < add_unit_list.Length; ++i)
                        {
                            if (add_unit_list[i] == null)
                            {
                                continue;
                            }

                            add_unit_uni_id_list[i] = add_unit_list[i].unique_id;
                        }

                        UserDataAdmin.Instance.m_add_unit_uni_id_list = add_unit_uni_id_list;
                    }

                    if (PlayerColumnName == "update_unit_list")
                    {
                        //unit更新
                        PacketStructUnit[] update_unit_list = (PacketStructUnit[])PlayerFieldInfo.GetValue(playerValue);
                        for (int i = 0; i < update_unit_list.Length; ++i)
                        {
                            PacketStructUnit update_unit = update_unit_list[i];

                            for (int j = 0; j < structPlayer.unit_list.Length; j++)
                            {
                                if (structPlayer.unit_list[j].unique_id == update_unit.unique_id)
                                {
                                    structPlayer.unit_list[j].Copy(update_unit);
                                    break;
                                }
                            }
                        }

                        // パラーメータ作成用
                        long[] update_unit_uni_id_list = new long[update_unit_list.Length];
                        for (int k = 0; k < update_unit_list.Length; ++k)
                        {
                            if (update_unit_list[k] == null)
                            {
                                continue;
                            }

                            update_unit_uni_id_list[k] = update_unit_list[k].unique_id;
                        }

                        UserDataAdmin.Instance.m_update_unit_uni_id_list = update_unit_uni_id_list;
                    }
                    if (PlayerColumnName == "delete_unit_list")
                    {
                        //unit削除
                        List<PacketStructUnit> player_unit_list_tmp = new List<PacketStructUnit>(structPlayer.unit_list);
                        PacketStructUnit[] delete_unit_list = (PacketStructUnit[])PlayerFieldInfo.GetValue(playerValue);

                        for (int i = 0; i < delete_unit_list.Length; ++i)
                        {
                            PacketStructUnit delete_unit = delete_unit_list[i];

                            for (int j = 0; j < player_unit_list_tmp.Count; j++)
                            {
                                if (player_unit_list_tmp[j].unique_id == delete_unit.unique_id)
                                {
                                    player_unit_list_tmp.Remove(player_unit_list_tmp[j]);
                                    break;
                                }
                            }
                        }

                        structPlayer.unit_list = player_unit_list_tmp.ToArray();

                        // パラーメータ作成用
                        long[] delete_unit_uni_id_list = new long[delete_unit_list.Length];
                        for (int k = 0; k < delete_unit_list.Length; ++k)
                        {
                            if (delete_unit_list[k] == null)
                            {
                                continue;
                            }

                            delete_unit_uni_id_list[k] = delete_unit_list[k].unique_id;
                        }

                        UserDataAdmin.Instance.m_delete_unit_uni_id_list = delete_unit_uni_id_list;
                    }
                }
            }

            return (PacketStructPlayer)structPlayer;
        }
    };
    private ResultData m_PackResult = null;
    public ResultData resultData { get { return m_PackResult; } }

    private System.Action<ResultData> m_successAction = delegate { };
    private System.Action<ResultData> m_errorAction = delegate { };

    private WWW m_WWW = null;

    // WWW.textがアクセスのたびにstringを生成しているので巨大なjsonの場合パフォーマンスが落ちる対策
    private string m_wwwText = "";

    private PlayMakerFSM playmakerFSM = null;
    private bool m_bFinished = false;
    public bool IsFinished { get { return m_bFinished; } }


    private Semaphore m_semaphore = new Semaphore();


    void Awake()
    {
        playmakerFSM = GetComponent<PlayMakerFSM>();
    }

    void Update()
    {
        m_semaphore.Tick();
    }

    void OnDestroy()
    {
        if (m_WWW != null)
        {
            m_WWW.Dispose();
            m_WWW = null;
        }
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    /// <param name="eAPIType"></param>
    /// <param name="strSendMessage"></param>
    /// <param name="unUniqueID"></param>
    public void setupApi(SERVER_API eAPIType, string strSendMessage, uint unUniqueID)
    {
        gameObject.name = "ServerApi_" + eAPIType.ToString();
        m_PackData.m_PacketAPI = eAPIType;
        m_PackData.m_PacketSendMessage = strSendMessage;
        m_PackData.m_PacketUniqueNum = unUniqueID;
    }

    /// <summary>
    /// 成功アクション設定
    /// </summary>
    /// <param name="_action"></param>
    public ServerApi setSuccessAction(System.Action<ResultData> _action)
    {
        m_successAction = _action;
        return this;
    }

    /// <summary>
    /// エラーアクション設定
    /// </summary>
    /// <param name="_action"></param>
    public ServerApi setErrorAction(System.Action<ResultData> _action)
    {
        m_errorAction = _action;
        return this;
    }

    public string createPacketAddress()
    {
        string strAddressIP = LocalSaveManager.Instance.LoadFuncServerAddressIP();
        string strPacketAddress = ServerDataUtilSend.CreateAddress(m_PackData.m_PacketAPI, strAddressIP);

        return strPacketAddress;
    }

    /// <summary>
    /// 通信開始
    /// </summary>
    public void SendStart()
    {
        playmakerFSM.SendEvent("DO_START");
    }

    /// <summary>
    /// 通信処理
    /// </summary>
    /// <returns></returns>
    public void RequestPatchUpdate()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("[checkResponse]RequestPatchUpdate start");
#endif
        if (MainMenuManager.Instance == null)
        {
            playmakerFSM.SendEvent("DO_NEXT");
            return;
        }

        //--------------------------------
        // チュートリアル中は、処理しない
        //--------------------------------
        if (TutorialManager.IsExists)
        {
            playmakerFSM.SendEvent("DO_NEXT");
            return;
        }

        switch (m_PackData.m_PacketAPI)
        {
            case SERVER_API.SERVER_API_STONE_USE_FRIEND:
            case SERVER_API.SERVER_API_STONE_USE_STAMINA:
            case SERVER_API.SERVER_API_STONE_USE_UNIT:
            case SERVER_API.SERVER_API_POINT_SHOP_LIMITOVER:
            case SERVER_API.SERVER_API_POINT_SHOP_EVOL:
            case SERVER_API.SERVER_API_UNIT_LINK_CREATE:
            case SERVER_API.SERVER_API_UNIT_LINK_DELETE:
            case SERVER_API.SERVER_API_EVOL_QUEST_START:
            case SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP:
            case SERVER_API.SERVER_API_USE_ITEM:
            case SERVER_API.SERVER_API_GACHA_PLAY:
            case SERVER_API.SERVER_API_UNIT_SALE:
            case SERVER_API.SERVER_API_POINT_SHOP_PURCHASE:
            case SERVER_API.SERVER_API_GET_LOGIN_PACK:
                {
                    playmakerFSM.SendEvent("DO_PATCH");
                }
                return;

            case SERVER_API.SERVER_API_STONE_PAY_PREV_ANDROID:
            case SERVER_API.SERVER_API_STONE_PAY_ANDROID:
            case SERVER_API.SERVER_API_STONE_PAY_PREV_IOS:
            case SERVER_API.SERVER_API_STONE_PAY_IOS:
            // このタイミングで行うと状態遷移がおかしくなるので除外
            // StoreBuyTip→SendStepChkOverBudget　でチェックを行う
            default:
                playmakerFSM.SendEvent("DO_NEXT");
                return;
        }

    }

    /// <summary>
    /// 通信処理
    /// </summary>
    /// <returns></returns>
    public void OnIsPatchUpdated()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("[checkResponse]OnIsPatchUpdated start");
#endif
        // メインメニュー中か
        if (MainMenuManager.Instance == null)
        {
            playmakerFSM.SendEvent("DO_POSITIVE");
            return;
        }

        // ユーザー追い出し禁止か
        if (MainMenuManager.Instance.m_ResumePatchUpdateRequest == false)
        {
            playmakerFSM.SendEvent("DO_POSITIVE");
            return;
        }

        if (Patcher.Instance.IsLoaded == true)
        {
            Patcher.Instance.IsUpdateMoveTitle(
                () =>
                {
                    playmakerFSM.SendEvent("DO_NEGATIVE");
                },
                () =>
                {
                    playmakerFSM.SendEvent("DO_POSITIVE");
                });
        }
        else
        {
            playmakerFSM.SendEvent("DO_POSITIVE");
            return;
        }
    }

    private void CreateSendApiRequest()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("[checkResponse]CreateSendApiRequest start");
#endif
        ServerDialogUpdate(m_PackData.m_PacketAPI, true);

        try
        {
            //--------------------------------
            // WWWクラスを使用してサーバーにAPIを送信。
            //--------------------------------
            WWWForm cWWWForm = new WWWForm();
            Dictionary<string, string> cHashtable = new Dictionary<string, string>();

            //送信データ
            cWWWForm.AddField("request", m_PackData.m_PacketSendMessage);

            string csum = ServerCheckSumUtil.CreatePacketCheckSum(m_PackData.m_PacketSendMessage,
                                                                                m_PackData.m_PacketAPI,
                                                                                strCheckSumUUID);
            //チェックサム
            cWWWForm.AddField("csum", csum);

            //
            cWWWForm.AddField("ott", ServerDataParam.m_ServerAPICheckRandOtt.ToString());

            //--------------------------------
            // PSPの固定フォーマット記述
            //
            // ユーザー生成とユーザー認証のAPIは
            // サーバー接続前に送られるパケットなので
            // 例外的にセッションIDを含めない。
            //--------------------------------
            if (m_PackData.m_PacketAPI == SERVER_API.SERVER_API_USER_CREATE ||
                m_PackData.m_PacketAPI == SERVER_API.SERVER_API_USER_AUTHENTICATION)
            {
                cHashtable["User-Agent"] = GlobalDefine.SERVER_USER_AGENT;
            }
            else
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                // Unity 2017.3.0p1でAndroid実機向けビルドでWWWを使用するとCookieがオートでセットされる
                // そのためセットしないように変更
#else
                cHashtable["Cookie"] = ServerDataParam.m_ServerSessionIDCookie + "; " +
                                        GlobalDefine.SERVER_COOKIE_SESSIONID + "=" + ServerDataParam.m_ServerSessionID;
#endif
                cHashtable["User-Agent"] = GlobalDefine.SERVER_USER_AGENT;
            }

            //--------------------------------
            // 通信実行
            //--------------------------------
            string address = createPacketAddress();

            if (m_WWW != null)
            {
                m_WWW.Dispose();
                m_WWW = null;
                m_Result = null;
            }

            m_WWW = new WWW(address, cWWWForm.data, cHashtable);
            m_WWW.threadPriority = ThreadPriority.Low;

#if BUILD_TYPE_DEBUG && UNITY_EDITOR
            Debug.Log("WWW address --------------  \n" + address);
#endif
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR:" + e.Message);

            m_Result = ResultCodeToCodeType(m_PackData.m_PacketAPI, API_CODE.API_CODE_CLIENT_RETRY);
            CheckResponseResult();
            return;
        }

        playmakerFSM.SendEvent("DO_NEXT");
    }

    private void CreateNextSendCookie()
    {
        //--------------------------------
        // レスポンスヘッダの検証
        // ここで解析した情報を次のAPI送信時に使用する
        //--------------------------------
        Dictionary<string, string> cResponseHeader = m_WWW.responseHeaders;
        foreach (KeyValuePair<string, string> kvPair in cResponseHeader)
        {
            //--------------------------------
            // クッキー以外の情報は無視
            //--------------------------------
            if (kvPair.Key.ToLower().Equals("set-cookie") == false)
            {
                continue;
            }

            //--------------------------------
            // [;]で区切って一覧化して総当たりチェック。
            // 特定キーに必要な情報が関連付けられているのでそこだけ抽出
            //--------------------------------
            string strCookie = kvPair.Value.Replace(",", ";");
            strCookie = strCookie.Replace("; ", ";");
            strCookie = strCookie.Replace(";", "; ");
            string[] astCookie = strCookie.Split(new string[] { "; " }, System.StringSplitOptions.None);

            string stCookie = string.Empty;
            for (int j = 0; j < astCookie.Length; j++)
            {
                stCookie = astCookie[j];

                if (stCookie.Length < 7 ||
                    stCookie.Substring(0, 7).ToLower().Equals("awselb=") == false)
                {
                    continue;
                }

                ServerDataParam.m_ServerSessionIDCookie = stCookie;
                break;
            }
        }
    }

    /// <summary>
    /// 通信処理
    /// </summary>
    /// <returns></returns>
    IEnumerator OnSendApi()
    {
        yield return m_WWW;

        m_wwwText = m_WWW.text;

#if BUILD_TYPE_DEBUG && UNITY_EDITOR
        //--------------------------------
        // レスポンスヘッダを全て洗ってログとして出力
        //--------------------------------
        string strResponseHeader = "";
        foreach (KeyValuePair<string, string> cKeyValuePair in m_WWW.responseHeaders)
        {
            strResponseHeader += cKeyValuePair.Key + " : " + cKeyValuePair.Value + "\n";
        }
        Debug.Log("Resv Response Header-------------- \n" + strResponseHeader);

        //レスポンスデータ
        try
        {
            Debug.Log(string.Format("[{0}] RESPONSE:{1}",
                                    m_PackData.m_PacketAPI.ToString(),
                                    m_wwwText.Length > 0 ? m_wwwText.JsonPrettyFormat() : ""));
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e);
            string strAddressIP = LocalSaveManager.Instance.LoadFuncServerAddressIP();
            string strPacketAddress = ServerDataUtilSend.CreateAddress(m_PackData.m_PacketAPI, strAddressIP);
            string message = strPacketAddress + "\n\n" + m_wwwText;

            Dialog m_OdinDialog = Dialog.Create(DialogType.DialogScrollInfo);
            m_OdinDialog.SetDialogText(DialogTextType.Title, "サーバJSONエラー");
            m_OdinDialog.AddScrollInfoText(message);
            m_OdinDialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText("common_button7"));
            m_OdinDialog.EnableFadePanel();
            m_OdinDialog.DisableAutoHide();
            m_OdinDialog.SetOkEvent(() => { });
            m_OdinDialog.Show();
        }
#endif

        CreateNextSendCookie();

        playmakerFSM.SendEvent("DO_NEXT");
    }

    /// <summary>
    /// レスポンス処理
    /// </summary>
    public void checkResponse()
    {
        if (m_WWW.isDone == false ||
            m_wwwText.Length <= 0)
        {
            CheckResponseResult();
            return;
        }

#if BUILD_TYPE_DEBUG
        Debug.Log("[checkResponse]RecvPacketAPI start");
#endif
        //--------------------------------
        // 正常に受理できている場合
        //--------------------------------
        m_Result = ServerDataUtilRecv.RecvPacketAPI(ref m_PackData, m_wwwText, m_WWW.error, false);

#if BUILD_TYPE_DEBUG
        Debug.Log("[checkResponse]CheckSum start");
#endif
        //--------------------------------
        // APIのチェックサム確認
        //--------------------------------
        CheckSum((bool seccess) =>
        {
            if (!seccess)
            {
                //--------------------------------
                // チェックサムが不一致の場合、チートの可能性から強制的に再起動を促す。
                //
                // 実装が誤作動した場合のセーフティとして、
                // パッチでエラーに飛ばす機能を無効化できるようにしている。
                // ※ここが無効でも基本的にチート操作はサーバー側処理で弾くはず
                //--------------------------------
                m_Result = ResultCodeToCodeType(m_PackData.m_PacketAPI, API_CODE.API_CODE_CLIENT_CSUM_ERROR);
            }

            CheckResponseResult();
        });
    }

    /// <summary>
    /// チェックサム
    /// </summary>
    // checksum失敗した場合false
    private void CheckSum(System.Action<bool> callback)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("[checkResponse]CheckSum run");
#endif

        switch (m_Result.m_Code)
        {
            //メンテナンス時はチェックサムが「0000000000」のため例外的にスキップする
            case API_CODE.API_CODE_WIDE_ERROR_MENTENANCE:
            //再認証時はチェックサムが「0000000000」のため例外的にスキップする
            case API_CODE.API_CODE_USER_AUTH_REQUIRED2:
                callback(true);
                return;
        }

        m_semaphore.Lock(() =>
        {
            bool bCheck = true;
            ServerCheckSumInfo info = ServerCheckSumUtil.check_sum_info.Find((v) => v.packet_unique_num == m_PackData.m_PacketUniqueNum);
            if (info != null)
            {
                ServerCheckSumUtil.check_sum_info.Remove(info);
                if (info.local_check_sum == info.server_check_sum)
                {
                    bCheck = true;
                }
            }

#if BUILD_TYPE_DEBUG && UNITY_EDITOR
            if (bCheck)
            {
                callback(bCheck);
            }
            else
            {
                string mainText = "";
                mainText += strCheckSumUUID;
                if (info != null)
                {
                    mainText += "\n local:" + info.local_check_sum.ToString();
                    mainText += "\n server:" + info.server_check_sum.ToString();
                }
                else
                {
                    mainText += "\n <color=#ff0000>Not Found CheckSumData</color>";
                }
                mainText += "\n result:" + bCheck.ToString();
                mainText += "\n" + m_PackData.m_PacketSendMessage;
                Dialog dlg = Dialog.Create(DialogType.DialogScrollInfo);
                dlg.SetDialogText(DialogTextType.Title, "Client Check UUID");
                dlg.AddScrollInfoText(mainText);
                dlg.SetDialogText(DialogTextType.OKText, "閉じる");
                dlg.SetDialogEvent(DialogButtonEventType.OK, () =>
                {
                    callback(bCheck);
                });
                dlg.Show();
            }
#else
            callback(bCheck);
#endif
        });
        new System.Threading.Thread(() =>
        {
            try
            {
                ServerCheckSumUtil.CheckPacketCheckSum(m_wwwText,
                                                        ServerDataParam.m_ServerAPICheckSum,
                                                        m_PackData,
                                                        strCheckSumUUID);
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR:" + e.Message);
            }
            finally
            {
                m_semaphore.Unlock();
            }
        }).Start();
    }

    /// <summary>
    /// リザルトデータチェック
    /// </summary>
    private void CheckResponseResult()
    {
        //--------------------------------
        // 正常に受理できていない場合
        // →再送したいのでリザルトを再送へ強制
        //--------------------------------
        if (m_Result == null)
        {
            m_Result = ResultCodeToCodeType(m_PackData.m_PacketAPI, API_CODE.API_CODE_CLIENT_RETRY);
        }

        ServerDialogUpdate(m_PackData.m_PacketAPI, false);

#if BUILD_TYPE_DEBUG
        Debug.Log("[checkResponse] SendEvent start");
        Debug.Log("[checkResponse] SendEvent m_CodeType =" + m_Result.m_CodeType.ToString());
#endif

        switch (m_Result.m_CodeType)
        {
            case API_CODETYPE.API_CODETYPE_USUALLY:
                playmakerFSM.SendEvent("SUCCESS");
                break;
            case API_CODETYPE.API_CODETYPE_RETRY:
                playmakerFSM.SendEvent("RETRY");
                break;
            case API_CODETYPE.API_CODETYPE_MENTENANCE:
                playmakerFSM.SendEvent("MENTENANCE");
                break;
            case API_CODETYPE.API_CODETYPE_VERSION_UP:
                playmakerFSM.SendEvent("VERSION_UP");
                break;
            case API_CODETYPE.API_CODETYPE_RESTART:
                playmakerFSM.SendEvent("RESTART");
                break;
            case API_CODETYPE.API_CODETYPE_AUTH_REQUIRED:
                playmakerFSM.SendEvent("AUTH_REQUEST");
                break;
            case API_CODETYPE.API_CODETYPE_INVALID_USER:
                playmakerFSM.SendEvent("INVALID_USER");
                break;
            case API_CODETYPE.API_CODETYPE_TRANSFER_MOVED:
                playmakerFSM.SendEvent("TRANSFER_USER");
                break;
        }

        switch (m_Result.m_Code)
        {
            case API_CODE.API_CODE_SUCCESS:
            case API_CODE.API_CODE_WIDE_ERROR_MENTENANCE:
                break;

            case API_CODE.API_CODE_CLIENT_CSUM_ERROR:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 終了アクション
    /// </summary>
    public void GoTitle()
    {
        m_bFinished = true;

        if (StoreDialogManager.Instance != null)
        {
            StoreDialogManager.Instance.ResetStatus();
        }
        if (ButtonBlocker.Instance != null)
        {
            // 売却時の追い出しでボタンがロック状態になるからアンロックする
            if (ButtonBlocker.Instance.IsActive(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG) == true)
            {
                ButtonBlocker.Instance.Unblock(MainMenuDefine.UNIT_DECIDE_BUTTON_BLOCK_TAG);
            }
        }
        DestroyObject(gameObject);
    }

    /// <summary>
    /// 終了アクション
    /// </summary>
    public void Finished()
    {
        m_bFinished = true;

        m_PackResult = new ResultData();
        m_PackResult.m_PacketAPI = m_PackData.m_PacketAPI;
        m_PackResult.m_PacketCode = m_Result.m_Code;
        m_PackResult.m_PacketUniqueNum = m_PackData.m_PacketUniqueNum;

        if (m_Result.m_Code == API_CODE.API_CODE_SUCCESS)
        {
            FinishedAsSuccess(m_PackResult);
        }
        else
        {
            FinishedAsFailure(m_PackResult);
        }
    }

    private void FinishedAsSuccess(ResultData resultData)
    {
        m_semaphore.Lock(() =>
        {
            m_successAction(resultData);
            DestroyObject(gameObject);
        });
        new System.Threading.Thread(() =>
        {
            try
            {
                parseJson();
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR:" + e.Message);
            }
            finally
            {
                m_semaphore.Unlock();
            }
        }).Start();
    }

    private void FinishedAsFailure(ResultData resultData)
    {
        Debug.LogError("ERROR:" + m_Result.m_Code + " GO:" + gameObject.name);

        ErrDialog(resultData, () =>
        {
            //エラーでもリプライデータがあるAPIの特別処理
            if (resultData.m_PacketAPI == SERVER_API.SERVER_API_GACHA_PLAY &&
                resultData.m_PacketCode == API_CODE.API_CODE_STEP_UP_GACHA_REST_TIME_NOW)
            {
                m_PackResult.SetPlayerResult<RecvGachaPlay>(m_wwwText);
            }
            m_errorAction(resultData);
            DestroyObject(gameObject);
        });
    }

    /// <summary>
    /// エラーダイアログ
    /// </summary>
    public void ErrDialog(ResultData data, Action error_action)
    {
        //エラーダイアログを表示せずに終了
        if (SERVER_API.SERVER_API_RENEW_TUTORIAL == data.m_PacketAPI)
        {
            switch (data.m_PacketCode)
            {
                case API_CODE.API_CODE_USER_AUTH_REQUIRED:
                case API_CODE.API_CODE_USER_AUTH_REQUIRED2:
                case API_CODE.API_CODE_CLIENT_RETRY:
                    error_action();
                    return;

                default:
                    break;
            }
        }

#if BUILD_TYPE_DEBUG
        API_CODE[] serverApiCode = {
                API_CODE.API_CODE_CLIENT_PARSE_FAILED,
                API_CODE.API_CODE_CLIENT_PARSE_FAILED_CODE,
                API_CODE.API_CODE_CLIENT_CSUM_ERROR,
                API_CODE.API_CODE_POST_LOG_UPLOAD_ERROR,
        };
        int indexserver = Array.IndexOf(serverApiCode, data.m_PacketCode);
        if (indexserver > 0)
        {
            OpenResponsDialog(data.m_PacketAPI, data.m_PacketCode, error_action);
            return;
        }
#endif

        API_CODE[] apiCode = {
              API_CODE.API_CODE_WIDE_ERROR_SERVER
            , API_CODE.API_CODE_WIDE_ERROR_VERSION
            , API_CODE.API_CODE_USER_AUTH_REQUIRED
            , API_CODE.API_CODE_USER_AUTH_REQUIRED2
            , API_CODE.API_CODE_FRIEND_REQUEST_ALREADY_STATE
            , API_CODE.API_CODE_DEBUG_ERROR_PERMISSION
            , API_CODE.API_CODE_WIDE_ERROR_USER_INVALID
            , API_CODE.API_CODE_QUEST_START_MASTER_NONE
            , API_CODE.API_CODE_QUEST_RESULT_INJUSTICE
            , API_CODE.API_CODE_QUEST_RESULT_ORDER_NONE
            , API_CODE.API_CODE_QUEST_RESULT_ORDER_ERR
            , API_CODE.API_CODE_QUEST_INGAME_RESET_CT_ZERO
            , API_CODE.API_CODE_QUEST_INGAME_RESET_CT_ERR
            , API_CODE.API_CODE_QUEST_INGAME_CONTINUE_CT_ZERO
            , API_CODE.API_CODE_QUEST_INGAME_CONTINUE_CT_ERR
            , API_CODE.API_CODE_WIDE_API_SHORT_COIN
            , API_CODE.API_CODE_WIDE_API_SHORT_FP
            , API_CODE.API_CODE_WIDE_API_SHORT_TIP
            , API_CODE.API_CODE_PAY_IOS_RECEIPT_ERR
            , API_CODE.API_CODE_PAY_IOS_PRODUCT_ERR
            , API_CODE.API_CODE_PAY_IOS_RECEIPT_PURCHASE
            , API_CODE.API_CODE_PAY_ANDROID_RECEIPT_ERR
            , API_CODE.API_CODE_PAY_ANDROID_SIGNED_ERR
            , API_CODE.API_CODE_WIDE_API_UNIT_NONE
            , API_CODE.API_CODE_WIDE_ERROR_BAD_REQUEST2
            , API_CODE.API_CODE_PAY_WIDE_PRODUCT_ERR
            , API_CODE.API_CODE_PAY_WIDE_PRODUCT_EVENT_NONE
            , API_CODE.API_CODE_BEGINNER_BOOST_FIXID_ERROR
            , API_CODE.API_CODE_BEGINNER_BOOST_RANK_ERROR
            , API_CODE.API_CODE_BEGINNER_BOOST_PERIOD_OUTSIDE
            , API_CODE.API_CODE_GACHA_INVALID_RANK_ERROR
            , API_CODE.API_CODE_MAKEPASSWORD_INVALID_RANK_ERROR
            , API_CODE.API_CODE_MASTERDATA_HASH_DIFFERENT
            , API_CODE.API_CODE_WIDE_ERROR_SYSTEM
            , API_CODE.API_CODE_WIDE_ERROR_DB
            , API_CODE.API_CODE_WIDE_ERROR_MASTER
            , API_CODE.API_CODE_WIDE_ERROR_VERSION2
            , API_CODE.API_CODE_CLIENT_PARSE_FAILED
            , API_CODE.API_CODE_USER_CREATE_UUID_ERR
            , API_CODE.API_CODE_WIDE_DEAD_LOCK
            , API_CODE.API_CODE_WIDE_ERROR_USER_FREEZE
            , API_CODE.API_CODE_WIDE_ERROR_BAD_REQUEST
        };

        int index = Array.IndexOf(apiCode, data.m_PacketCode);
        if (index > 0)
        {
            //デフォルトエラーダイアログ
            OpenDialog(data.m_PacketCode,
                        true,
                        "error_reject_common_title",
                        "error_reject_common_contents",
                        "common_button1",
                        null,
                        CHANGE_SCENE.CHANGE_SCENE_TITLE,
                        DIALOG_TYPE.DIALOG_TYPE_1,
            () =>
            {
                error_action();
            });
            return;
        }

        Dictionary<API_CODE, ApiErrorDialogTextKey> DialogTextTable = new Dictionary<API_CODE, ApiErrorDialogTextKey>()
        {
            {API_CODE.API_CODE_FRIEND_SELECT_ME, new ApiErrorDialogTextKey{ title_key = "fr163q_title",  contents_key = "fr163q_content2", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_TITLE, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_SELECT_NOT, new ApiErrorDialogTextKey{ title_key = "error_response_title1",  contents_key = "error_response_content1", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_SELECT_USER_NONE, new ApiErrorDialogTextKey{ title_key = "error_response_title2",  contents_key = "error_response_content2", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_SELECT_NOT2, new ApiErrorDialogTextKey{ title_key = "error_response_title3",  contents_key = "error_response_content3", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_SEARCH_NONE, new ApiErrorDialogTextKey{ title_key = "fr163q_title",  contents_key = "fr163q_content2", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_REQUEST_ALREADY_FRIEND, new ApiErrorDialogTextKey{ title_key = "fr166q_title",  contents_key = "fr_error_text01", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_REQUEST_ALREADY_STATE2, new ApiErrorDialogTextKey{ title_key = "fr166q_title",  contents_key = "fr166q_content", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_REQUEST_FULL_ME, new ApiErrorDialogTextKey{ title_key = "error_response_title72",  contents_key = "error_response_content72", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_REQUEST_FULL_HIM, new ApiErrorDialogTextKey{ title_key = "fr158q_title",  contents_key = "fr158q_content2", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_REQUEST_RECEIVED, new ApiErrorDialogTextKey{ title_key = "fr165q_title",  contents_key = "fr165q_content", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_FRIEND_REQUEST_CANCELED, new ApiErrorDialogTextKey{ title_key = "fr158q_title",  contents_key = "fr_error_text02", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_TRANSFER_USER_ID, new ApiErrorDialogTextKey{ title_key = "mt18q_title",  contents_key = "mt18q_content1", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_TRANSFER_PASSWORD, new ApiErrorDialogTextKey{ title_key = "mt18q_title",  contents_key = "mt18q_content2", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_TRANSFER_MOVED, new ApiErrorDialogTextKey{ title_key = "mt18q_title",  contents_key = "mt18q_content5", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_TRANSFER_TIME_LIMIT, new ApiErrorDialogTextKey{ title_key = "mt18q_title",  contents_key = "mt18q_content4", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_RENEW_ERR_UUID_INVALID, new ApiErrorDialogTextKey{ title_key = "mt18q_title",  contents_key = "mt18q_content3", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_RENEW_ERR_USER_ID_BEFORE, new ApiErrorDialogTextKey{ title_key = "mt18q_title",  contents_key = "mt18q_content3", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_EVOL_UNIT_AFTER_NONE, new ApiErrorDialogTextKey{ title_key = "error_response_title4",  contents_key = "error_response_content4", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.定義なし, new ApiErrorDialogTextKey{ title_key = "error_response_title5",  contents_key = "error_response_content5", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_EVOL_UNIT_LEVEL_SHORT2, new ApiErrorDialogTextKey{ title_key = "error_response_title6",  contents_key = "error_response_content6", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_UNIT_COST_OVER2   , new ApiErrorDialogTextKey{ title_key = "error_response_title7",  contents_key = "error_response_content7", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_WIDE_API_UNIT_EXCESS, new ApiErrorDialogTextKey{ title_key = "error_response_title8",  contents_key = "error_response_content8", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_WIDE_API_UNIT_OWNER_ERR, new ApiErrorDialogTextKey{ title_key = "error_response_title9",  contents_key = "error_response_content9", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_TITLE, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_LINKSYSTEM_SALE_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title10",  contents_key = "error_response_content10", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_LINKSYSTEM_COMPOSE_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title11",  contents_key = "error_response_content11", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_LINKSYSTEM_FORMUNIT_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title12",  contents_key = "error_response_content12", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_LINKSYSTEM_UNIT_DISABLE_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title13",  contents_key = "error_response_content13", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_LINKSYSTEM_EVOL_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title14",  contents_key = "error_response_content14", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_WIDE_API_UNIT_SAME, new ApiErrorDialogTextKey{ title_key = "error_response_title15",  contents_key = "error_response_content15", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_TITLE, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_WIDE_API_UNIT_PARTY_ASSIGNED, new ApiErrorDialogTextKey{ title_key = "error_response_title16",  contents_key = "error_response_content16", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_SALE_COIN_MAX, new ApiErrorDialogTextKey{ title_key = "error_response_title17",  contents_key = "error_response_content17", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_EVOL_UNIT_LEVEL_SHORT, new ApiErrorDialogTextKey{ title_key = "error_response_title18",  contents_key = "error_response_content18", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_WIDE_API_SHORT_TICKET, new ApiErrorDialogTextKey{ title_key = "error_response_title19",  contents_key = "error_response_content19", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_REQUIRE_QUEST_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title20",  contents_key = "error_response_content20", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_AREA_AMEND_FIXID_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title21",  contents_key = "error_response_content21", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_TITLE, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_AREA_AMEND_AREA_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title22",  contents_key = "error_response_content22", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_TITLE, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_AREA_AMEND_PERIOD_OUTSIDE, new ApiErrorDialogTextKey{ title_key = "error_response_title23",  contents_key = "error_response_content23", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_CONTINUE_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title24",  contents_key = "error_response_content24", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_START_BAD_CONDITION, new ApiErrorDialogTextKey{ title_key = "error_response_title25",  contents_key = "error_response_content25", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_WIDE_API_SHORT_STAMINA, new ApiErrorDialogTextKey{ title_key = "kk109q_title",  contents_key = "kk109q_content1", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_WIDE_API_SHORT_STAMINA_MAX, new ApiErrorDialogTextKey{ title_key = "kk109q_title",  contents_key = "kk109q_content2", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_KEY_COUNT_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title26",  contents_key = "error_response_content26", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_KEY_AREA_CLOSE, new ApiErrorDialogTextKey{ title_key = "error_response_title27",  contents_key = "error_response_content27", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_START_HELPER_ME, new ApiErrorDialogTextKey{ title_key = "error_response_title28",  contents_key = "error_response_content28", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_TITLE, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_START_INVALID, new ApiErrorDialogTextKey{ title_key = "error_response_title29",  contents_key = "error_response_content29", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_TITLE, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_WIDE_API_EVENT_ERR_FP, new ApiErrorDialogTextKey{ title_key = "error_response_title30",  contents_key = "error_response_content30", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_WIDE_API_EVENT_ERR_SLV, new ApiErrorDialogTextKey{ title_key = "error_response_title31",  contents_key = "error_response_content31", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_START_OVER_UNIT, new ApiErrorDialogTextKey{ title_key = "error_response_title71",  contents_key = "error_response_content71", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_START_OVER_PARTY_COST, new ApiErrorDialogTextKey{ title_key = "kk110q_title",  contents_key = "kk110q_content", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_QUEST_START_OVER_UNIT2, new ApiErrorDialogTextKey{ title_key = "error_response_title71",  contents_key = "error_response_content71", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_GACHA_STOP_STATUS_STOP, new ApiErrorDialogTextKey{ title_key = "ERROR_GETBOX_GACHA_STATUS_STOP_TITLE",  contents_key = "ERROR_GETBOX_GACHA_STATUS_STOP", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_GACHA_STOP_STATUS_DISABLE, new ApiErrorDialogTextKey{ title_key = "ERROR_GETBOX_GACHA_STATUS_DISABLE_TITLE",  contents_key = "ERROR_GETBOX_GACHA_STATUS_DISABLE", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_TITLE, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_GACHA_EVENT_OUTSIDE, new ApiErrorDialogTextKey{ title_key = "error_response_title34",  contents_key = "error_response_content34", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_GACHA_FIXID_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title35",  contents_key = "error_response_content35", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.定義なし, new ApiErrorDialogTextKey{ title_key = "error_response_title36",  contents_key = "error_response_content36", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_ACHIEVEMENT_REWARD_MAX, new ApiErrorDialogTextKey{ title_key = "error_response_title37",  contents_key = "error_response_content37", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_ACHIEVEMENT_REWARD_MAX_ALL, new ApiErrorDialogTextKey{ title_key = "error_response_title38",  contents_key = "error_response_content38", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_ACHIEVEMENT_REWARD_ERR_1_KEY, new ApiErrorDialogTextKey{ title_key = "error_response_title39",  contents_key = "error_response_content39", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_ACHIEVEMENT_REWARD_ERR_2_KEY, new ApiErrorDialogTextKey{ title_key = "error_response_title40",  contents_key = "error_response_content40", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_ACHIEVEMENT_REWARD_ERR_INVALID_KEY, new ApiErrorDialogTextKey{ title_key = "error_response_title41",  contents_key = "error_response_content41", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_ACHIEVEMENT_GP_FIXID_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title42",  contents_key = "error_response_content42", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_PAY_WIDE_AGE_BUDGET_OVER, new ApiErrorDialogTextKey{ title_key = "error_response_title43",  contents_key = "error_response_content43", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_POINT_SHOP_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title44",  contents_key = "error_response_content44", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_POINT_SHOP_TIMING_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title45",  contents_key = "error_response_content45", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_PAY_WIDE_TIP_MAX, new ApiErrorDialogTextKey{ title_key = "error_response_title46",  contents_key = "error_response_content46", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //購入画面処理２重チェック,{API_CODE.API_CODE_WIDE_API_SHORT_TIP2, new ApiErrorDialogTextKey{ title_key = "error_response_title47",  contents_key = "error_response_content47", button_key1 = "common_button4", button_key2 = "common_button5", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY_OR_ACTION, dialog_type = DIALOG_TYPE.DIALOG_TYPE_2 } }
            ,{API_CODE.API_CODE_STORE_MAX_STAMINA, new ApiErrorDialogTextKey{ title_key = "error_response_title48",  contents_key = "error_response_content48", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STORE_MAX_FRIEND, new ApiErrorDialogTextKey{ title_key = "error_response_title49",  contents_key = "error_response_content49", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_PAY_WIDE_PRODUCT_EVENT_ALREADY, new ApiErrorDialogTextKey{ title_key = "error_response_title50",  contents_key = "error_response_content50", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_BUY_TIP, new ApiErrorDialogTextKey{ title_key = "error_response_title51",  contents_key = "error_response_content51", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_USE_TIP_CONTINUE, new ApiErrorDialogTextKey{ title_key = "error_response_title52",  contents_key = "error_response_content52", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_USE_TIP_RETRY, new ApiErrorDialogTextKey{ title_key = "error_response_title53",  contents_key = "error_response_content53", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_USE_TIP_UNIT, new ApiErrorDialogTextKey{ title_key = "error_response_title54",  contents_key = "error_response_content54", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_USE_TIP_FRIEND, new ApiErrorDialogTextKey{ title_key = "error_response_title55",  contents_key = "error_response_content55", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_USE_TIP_STAMINA, new ApiErrorDialogTextKey{ title_key = "error_response_title56",  contents_key = "error_response_content56", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_USE_TIP_PREMIUM, new ApiErrorDialogTextKey{ title_key = "error_response_title57",  contents_key = "error_response_content57", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_USE_TIP_EVOL, new ApiErrorDialogTextKey{ title_key = "error_response_title58",  contents_key = "error_response_content58", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STOP_AREA, new ApiErrorDialogTextKey{ title_key = "error_response_title59",  contents_key = "error_response_content59", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_PAY_WIDE_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title60",  contents_key = "error_response_content60", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STORE_MAX_UNIT, new ApiErrorDialogTextKey{ title_key = "error_response_title61",  contents_key = "error_response_content61", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_PAY_WIDE_APP_STORE_503, new ApiErrorDialogTextKey{ title_key = "error_response_title62",  contents_key = "error_response_content62", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_ITEM_NOTHING_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title63",  contents_key = "error_response_content63", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_ITEM_OVERLAP_ERROR, new ApiErrorDialogTextKey{ title_key = "error_response_title64",  contents_key = "error_response_content64", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_PRESENT_ERROR    , new ApiErrorDialogTextKey{ title_key = "error_response_title65",  contents_key = "error_response_content65", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE., new ApiErrorDialogTextKey{ title_key = "error_response_title66",  contents_key = "error_response_content66", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_PRESENT_NONE, new ApiErrorDialogTextKey{ title_key = "error_response_title67",  contents_key = "error_response_content67", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            //,{API_CODE.API_CODE_CLIENT_RETRY, new ApiErrorDialogTextKey{ title_key = "error_response_title68",  contents_key = "error_response_content68", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_USER_RENAME_NG_WORD, new ApiErrorDialogTextKey{ title_key = "error_response_title69",  contents_key = "error_response_content69", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_PAY_WIDE_USER_NOT_MATCH, new ApiErrorDialogTextKey{ title_key = "error_response_title70",  contents_key = "error_response_content70", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_PAY_WIDE_PRODUCT_TIMEOUT, new ApiErrorDialogTextKey{ title_key = "error_event_not_found_title",  contents_key = "error_event_not_found_text", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_SCORE_REWARD_INVALID_EVENT, new ApiErrorDialogTextKey{ title_key = "error_response_title73",  contents_key = "error_response_content73", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_HOME, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            ,{API_CODE.API_CODE_STEP_UP_GACHA_REST_TIME_NOW, new ApiErrorDialogTextKey{ title_key = "Gacha_ResetCheck_Title",  contents_key = "Gacha_ResetCheck", button_key1 = "common_button1", change_scene = CHANGE_SCENE.CHANGE_SCENE_STAY, dialog_type = DIALOG_TYPE.DIALOG_TYPE_1 } }
            };

        if (!DialogTextTable.ContainsKey(data.m_PacketCode))
        {
            //エラーコード設定なし。デフォルトエラーダイアログ
            OpenDialog(data.m_PacketCode,
                        true,
                        "error_reject_common_title",
                        "error_reject_common_contents",
                        "common_button1",
                        null,
                        CHANGE_SCENE.CHANGE_SCENE_STAY,
                        DIALOG_TYPE.DIALOG_TYPE_1,
            () =>
            {
                error_action();
            });
            return;
        }

        OpenDialog(data.m_PacketCode,
                    false,
                    DialogTextTable[data.m_PacketCode].title_key,
                    DialogTextTable[data.m_PacketCode].contents_key,
                    DialogTextTable[data.m_PacketCode].button_key1,
                    DialogTextTable[data.m_PacketCode].button_key2,
                    DialogTextTable[data.m_PacketCode].change_scene,
                    DialogTextTable[data.m_PacketCode].dialog_type,
                    () =>
                    {
                        error_action();
                    });
    }

    /// <summary>
    /// ダイアログ
    /// </summary>
    public void OpenDialog(API_CODE api_code, bool isViewApiCode, string strTitleKey, string strMsgKey, string strBtnKey1, string strBtnKey2, CHANGE_SCENE change_scene, DIALOG_TYPE dialog_type, Action error_action)
    {
        Dialog newDialog = null;
        if (strBtnKey2 != null && strBtnKey2.Length > 0)
        {
            newDialog = Dialog.Create(DialogType.DialogYesNo);
            newDialog.SetDialogText(DialogTextType.YesText, GameTextUtil.GetText(strBtnKey1));
            newDialog.SetDialogText(DialogTextType.NoText, GameTextUtil.GetText(strBtnKey2));
        }
        else
        {
            newDialog = Dialog.Create(DialogType.DialogOK);
            newDialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText(strBtnKey1));
        }

        string title = GameTextUtil.GetText(strTitleKey);
        if (isViewApiCode)
        {
            title += " (" + api_code.ToString("d") + ")";
        }
        else
        {
#if BUILD_TYPE_DEBUG
            DebugLogger.StatAdd("SERVER_ERROR_CODE:" + api_code.ToString("d"));
#endif
        }

        newDialog.SetDialogText(DialogTextType.Title, title);
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, strMsgKey);

        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            StartCoroutine(ErrorActionDelayMethod(3.0f, 2.0f, () =>
            {
                ErrorActionChangeScene(change_scene);
                error_action();
            }));
        }));

        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            StartCoroutine(ErrorActionDelayMethod(3.0f, 2.0f, () =>
            {
                error_action();
            }));
        }));

        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            StartCoroutine(ErrorActionDelayMethod(3.0f, 2.0f, () =>
            {
                error_action();
            }));
        }));

        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    /// <summary>
    /// ErrorActionChangeScene
    /// </summary>
    private void ErrorActionChangeScene(CHANGE_SCENE change_scene)
    {
        switch (change_scene)
        {
            case CHANGE_SCENE.CHANGE_SCENE_TITLE:
                ResidentParam.m_IsGoToTileWithApiError = true;
                SceneCommon.Instance.GameToTitle();
                break;
            case CHANGE_SCENE.CHANGE_SCENE_HOME:
                SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
                break;
            case CHANGE_SCENE.CHANGE_SCENE_STAY:
                break;
            case CHANGE_SCENE.CHANGE_SCENE_STAY_OR_ACTION:
                SceneCommon.Instance.ChangeScene(SceneType.SceneMainMenu);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Json解析
    /// </summary>
    private void parseJson()
    {
        if (m_PackResult == null)
        {
            return;
        }

        switch (m_PackResult.m_PacketAPI)
        {
            case SERVER_API.SERVER_API_USER_CREATE: { m_PackResult.SetPlayerResult<RecvCreateUser>(m_wwwText); ServerDataParam.m_ServerConnectOK = true; break; }
            case SERVER_API.SERVER_API_USER_AUTHENTICATION: { m_PackResult.SetPlayerResult<RecvUserAuthentication>(m_wwwText); ServerDataParam.m_ServerConnectOK = true; break; }
            case SERVER_API.SERVER_API_USER_RENAME: { m_PackResult.SetResult<RecvRenameUser>(JsonMapper.ToObject<RecvRenameUser>(m_wwwText)); ServerDataParam.m_ServerConnectOK = true; break; }
            case SERVER_API.SERVER_API_USER_SELECT_DEF_PARTY: { m_PackResult.SetPlayerResult<RecvSelectDefParty>(m_wwwText); break; }
            case SERVER_API.SERVER_API_USER_RENEW: { m_PackResult.SetPlayerResult<RecvRenewUser>(m_wwwText); break; }
            case SERVER_API.SERVER_API_USER_RENEW_CHECK: { m_PackResult.SetResult<RecvCheckRenewUser>(JsonMapper.ToObject<RecvCheckRenewUser>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_MASTER_HASH_GET: { m_PackResult.SetResult<RecvGetMasterHash>(JsonMapper.ToObject<RecvGetMasterHash>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL: { m_PackResult.SetResult<RecvMasterDataAll>(JsonMapper.ToObject<RecvMasterDataAll>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_MASTER_DATA_GET_ALL2: { m_PackResult.SetResult<RecvMasterDataAll2>(JsonMapper.ToObject<RecvMasterDataAll2>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_GET_ACHIEVEMENT_GP: { m_PackResult.SetResult<RecvAchievementGroup>(JsonMapper.ToObject<RecvAchievementGroup>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_GET_ACHIEVEMENT: { m_PackResult.SetResult<RecvMasterDataAchievement>(JsonMapper.ToObject<RecvMasterDataAchievement>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_GET_LOGIN_PACK: { m_PackResult.SetResult<RecvLoginPack>(JsonMapper.ToObject<RecvLoginPack>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_GET_LOGIN_PARAM: { m_PackResult.SetResult<RecvLoginParam>(JsonMapper.ToObject<RecvLoginParam>(m_wwwText)); break; }

            case SERVER_API.SERVER_API_FRIEND_LIST_GET: { m_PackResult.SetResult<RecvFriendListGet>(JsonMapper.ToObject<RecvFriendListGet>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_FRIEND_REQUEST: { m_PackResult.SetResult<RecvFriendRequest>(JsonMapper.ToObject<RecvFriendRequest>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_FRIEND_CONSENT: { m_PackResult.SetResult<RecvFriendConsent>(JsonMapper.ToObject<RecvFriendConsent>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_FRIEND_REFUSAL: { m_PackResult.SetResult<RecvFriendRefusal>(JsonMapper.ToObject<RecvFriendRefusal>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_FRIEND_SEARCH: { m_PackResult.SetResult<RecvFriendSearch>(JsonMapper.ToObject<RecvFriendSearch>(m_wwwText)); break; }

            case SERVER_API.SERVER_API_UNIT_PARTY_ASSIGN: { m_PackResult.SetPlayerResult<RecvUnitPartyAssign>(m_wwwText); break; }
            case SERVER_API.SERVER_API_UNIT_SALE: { m_PackResult.SetPlayerResult<RecvUnitSale>(m_wwwText); break; }
            case SERVER_API.SERVER_API_UNIT_BLEND_BUILDUP: { m_PackResult.SetPlayerResult<RecvUnitBlendBuildUp>(m_wwwText); break; }
            case SERVER_API.SERVER_API_UNIT_BLEND_EVOL: { m_PackResult.SetResult<RecvUnitBlendEvol>(JsonMapper.ToObject<RecvUnitBlendEvol>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_UNIT_LINK_CREATE: { m_PackResult.SetPlayerResult<RecvUnitLink>(m_wwwText); break; }
            case SERVER_API.SERVER_API_UNIT_LINK_DELETE: { m_PackResult.SetPlayerResult<RecvUnitLink>(m_wwwText); break; }

            case SERVER_API.SERVER_API_QUEST_HELPER_GET: { m_PackResult.SetResult<RecvQuestHelperGet>(JsonMapper.ToObject<RecvQuestHelperGet>(m_wwwText)); break; }
            case SERVER_API.SERVER_API_QUEST_HELPER_GET_EVOL: { m_PackResult.SetResult<RecvQuestHelperGetEvol>(JsonMapper.ToObject<RecvQuestHelperGetEvol>(m_wwwText)); break; }    // APIタイプ：クエスト操作：助っ人一覧取得（進化合成向き）
            case SERVER_API.SERVER_API_QUEST_HELPER_GET_BUILD: { m_PackResult.SetResult<RecvQuestHelperGetBuild>(JsonMapper.ToObject<RecvQuestHelperGetBuild>(m_wwwText)); break; } // APIタイプ：クエスト操作：助っ人一覧取得（強化合成向き）
            case SERVER_API.SERVER_API_QUEST_START: { m_PackResult.SetResult<RecvQuestStart>(JsonMapper.ToObject<RecvQuestStart>(m_wwwText)); break; }  // APIタイプ：クエスト操作：クエスト開始
            case SERVER_API.SERVER_API_QUEST_CLEAR: { m_PackResult.SetPlayerResult<RecvQuestClear>(m_wwwText); break; } // APIタイプ：クエスト操作：クエストクリア
            case SERVER_API.SERVER_API_QUEST_RETIRE: { m_PackResult.SetPlayerResult<RecvQuestRetire>(m_wwwText); break; }   // APIタイプ：クエスト操作：クエストリタイア
            case SERVER_API.SERVER_API_QUEST_ORDER_GET: { m_PackResult.SetResult<RecvQuestOrderGet>(JsonMapper.ToObject<RecvQuestOrderGet>(m_wwwText)); break; }    // APIタイプ：クエスト操作：クエスト受託情報取得
            case SERVER_API.SERVER_API_QUEST_CONTINUE: { m_PackResult.SetResult<RecvQuestContinue>(JsonMapper.ToObject<RecvQuestContinue>(m_wwwText)); break; } // APIタイプ：インゲーム中：コンティニュー
            case SERVER_API.SERVER_API_QUEST_RESET: { m_PackResult.SetResult<RecvQuestReset>(JsonMapper.ToObject<RecvQuestReset>(m_wwwText)); break; }  // APIタイプ：インゲーム中：リセット
            case SERVER_API.SERVER_API_EVOL_QUEST_START: { m_PackResult.SetPlayerResult<RecvEvolQuestStart>(m_wwwText); break; }    // APIタイプ：クエスト操作：進化クエスト開始
            case SERVER_API.SERVER_API_EVOL_QUEST_CLEAR: { m_PackResult.SetResult<RecvEvolQuestClear>(JsonMapper.ToObject<RecvEvolQuestClear>(m_wwwText)); break; }	// APIタイプ：クエスト操作：進化クエストクリア

            case SERVER_API.SERVER_API_QUEST2_START: { m_PackResult.SetResult<RecvQuest2Start>(JsonMapper.ToObject<RecvQuest2Start>(m_wwwText)); break; }	// APIタイプ：クエスト操作：クエスト開始
            case SERVER_API.SERVER_API_QUEST2_START_TUTORIAL: { m_PackResult.SetResult<RecvQuest2Start>(JsonMapper.ToObject<RecvQuest2Start>(m_wwwText)); break; }	// APIタイプ：クエスト操作：クエスト開始
            case SERVER_API.SERVER_API_QUEST2_CLEAR: { m_PackResult.SetPlayerResult<RecvQuest2Clear>(m_wwwText); break; }	                                            // APIタイプ：クエスト操作：クエストクリア
            case SERVER_API.SERVER_API_QUEST2_ORDER_GET: { m_PackResult.SetResult<RecvQuest2OrderGet>(JsonMapper.ToObject<RecvQuest2OrderGet>(m_wwwText)); break; }	// APIタイプ：クエスト操作：クエスト受託情報取得
            case SERVER_API.SERVER_API_QUEST2_CESSAION_QUEST: { m_PackResult.SetResult<RecvQuest2CessaionQuest>(JsonMapper.ToObject<RecvQuest2CessaionQuest>(m_wwwText)); break; }	// APIタイプ：クエスト操作：新クエスト中断データ破棄通知
            case SERVER_API.SERVER_API_QUEST2_STORY_START: { m_PackResult.SetPlayerResult<RecvQuest2StoryStart>(m_wwwText); break; }                                               // APIタイプ：クエスト操作：ストーリクエスト開始
            case SERVER_API.SERVER_API_QUEST2_STORY_CLEAR: { m_PackResult.SetPlayerResult<RecvQuest2StoryClear>(m_wwwText); break; }                                               // APIタイプ：クエスト操作：ストーリクエストクリア

            case SERVER_API.SERVER_API_INJUSTICE: { m_PackResult.SetResult<RecvInjustice>(JsonMapper.ToObject<RecvInjustice>(m_wwwText)); break; }  // APIタイプ：不正検出関連：不正検出送信
            case SERVER_API.SERVER_API_TUTORIAL: { m_PackResult.SetResult<RecvTutorialStep>(JsonMapper.ToObject<RecvTutorialStep>(m_wwwText)); break; } // APIタイプ：チュートリアル関連：進行集計
            case SERVER_API.SERVER_API_STONE_USE_UNIT: { m_PackResult.SetPlayerResult<RecvStoneUsedUnit>(m_wwwText); break; }   // APIタイプ：魔法石使用：ユニット枠増設
            case SERVER_API.SERVER_API_STONE_USE_FRIEND: { m_PackResult.SetPlayerResult<RecvStoneUsedFriend>(m_wwwText); break; }   // APIタイプ：魔法石使用：フレンド枠増設
            case SERVER_API.SERVER_API_STONE_USE_STAMINA: { m_PackResult.SetPlayerResult<RecvStoneUsedStamina>(m_wwwText); break; } // APIタイプ：魔法石使用：スタミナ回復
            case SERVER_API.SERVER_API_GACHA_PLAY: { m_PackResult.SetPlayerResult<RecvGachaPlay>(m_wwwText); break; }   // APIタイプ：ガチャ操作：ガチャ実行
            case SERVER_API.SERVER_API_GACHA_TICKET_PLAY: { m_PackResult.SetPlayerResult<RecvGachaTicketPlay>(m_wwwText); break; }  // APIタイプ：ガチャ操作：ガチャチケット実行
            case SERVER_API.SERVER_API_STONE_PAY_PREV_IOS: { m_PackResult.SetResult<RecvStorePayPrev_ios>(JsonMapper.ToObject<RecvStorePayPrev_ios>(m_wwwText)); break; }   // APIタイプ：課金操作：魔法石購入直前処理( iOS … AppStore )
            case SERVER_API.SERVER_API_STONE_PAY_PREV_ANDROID: { m_PackResult.SetResult<RecvStorePayPrev_android>(JsonMapper.ToObject<RecvStorePayPrev_android>(m_wwwText)); break; }   // APIタイプ：課金操作：魔法石購入直前処理( Android … GooglePlay )
            case SERVER_API.SERVER_API_STONE_PAY_IOS: { m_PackResult.SetResult<RecvStorePay_ios>(JsonMapper.ToObject<RecvStorePay_ios>(m_wwwText)); break; }    // APIタイプ：課金操作：魔法石購入反映処理( iOS … AppStore )
            case SERVER_API.SERVER_API_STONE_PAY_ANDROID: { m_PackResult.SetResult<RecvStorePay_android>(JsonMapper.ToObject<RecvStorePay_android>(m_wwwText)); break; }    // APIタイプ：課金操作：魔法石購入反映処理( Android … GooglePlay )
            case SERVER_API.SERVER_API_REVIEW_PRESENT: { m_PackResult.SetResult<RecvReviewPresent>(JsonMapper.ToObject<RecvReviewPresent>(m_wwwText)); break; } // APIタイプ：ユーザーレビュー関連：レビュー遷移報酬
            case SERVER_API.SERVER_API_PRESENT_LIST_GET: { m_PackResult.SetResult<RecvPresentListGet>(JsonMapper.ToObject<RecvPresentListGet>(m_wwwText)); break; } // APIタイプ：プレゼント関連：プレゼント一覧取得
            case SERVER_API.SERVER_API_PRESENT_OPEN: { m_PackResult.SetPlayerResult<RecvPresentOpen>(m_wwwText); break; }   // APIタイプ：プレゼント関連：プレゼント開封
            case SERVER_API.SERVER_API_TRANSFER_ORDER: { m_PackResult.SetResult<RecvTransferOrder>(JsonMapper.ToObject<RecvTransferOrder>(m_wwwText)); break; } // APIタイプ：セーブ移行関連：パスワード発行
            case SERVER_API.SERVER_API_TRANSFER_EXEC: { m_PackResult.SetResult<RecvTransferExec>(JsonMapper.ToObject<RecvTransferExec>(m_wwwText)); break; }    // APIタイプ：セーブ移行関連：移行実行
#if BUILD_TYPE_DEBUG
            case SERVER_API.SERVER_API_DEBUG_EDIT_USER: { m_PackResult.SetResult<RecvDebugEditUser>(JsonMapper.ToObject<RecvDebugEditUser>(m_wwwText)); break; }    // APIタイプ：デバッグ機能関連：ユーザーデータ更新
                                                                                                                                                                    //case SERVER_API.SERVER_API_DEBUG_UNIT_GET:			{ m_PackResult.SetResult<RecvDebugUnitGet>(JsonMapper.ToObject< RecvDebugUnitGet			>(m_wwwText)); break; }    // APIタイプ：デバッグ機能関連：ユニット取得
            case SERVER_API.SERVER_API_DEBUG_UNIT_GET: { m_PackResult.SetPlayerResult<RecvDebugUnitGet>(m_wwwText); break; }  // APIタイプ：デバッグ機能関連：ユニット取得
            case SERVER_API.SERVER_API_DEBUG_QUEST_CLEAR: { m_PackResult.SetResult<RecvDebugQuestClear>(JsonMapper.ToObject<RecvDebugQuestClear>(m_wwwText)); break; }	// APIタイプ：デバッグ機能関連：クエストクリア情報改変
            case SERVER_API.SERVER_API_DEBUG_SEND_BATTLE_LOG: { m_PackResult.SetResult<RecvDebugBattleLog>(JsonMapper.ToObject<RecvDebugBattleLog>(m_wwwText)); break; }    // APIタイプ：デバッグ機能関連：バトルログアップロード
            case SERVER_API.SERVER_API_DEBUG_MASTER_DATA_GET_ALL2: { m_PackResult.SetResult<RecvMasterDataAll2>(JsonMapper.ToObject<RecvMasterDataAll2>(m_wwwText)); break; }
#endif
            case SERVER_API.SERVER_API_GET_STORE_EVENT: { m_PackResult.SetResult<RecvGetStoreProductEvent>(JsonMapper.ToObject<RecvGetStoreProductEvent>(m_wwwText)); break; }  // APIタイプ：イベントストア一覧取得
            case SERVER_API.SERVER_API_STORE_PAY_CANCEL: { m_PackResult.SetResult<RecvStorePayCancel>(JsonMapper.ToObject<RecvStorePayCancel>(m_wwwText)); break; } // APIタイプ：課金操作：魔法石購入キャンセル
            case SERVER_API.SERVER_API_ACHIEVEMENT_OPEN: { m_PackResult.SetPlayerResult<RecvOpenAchievement>(m_wwwText); break; }   // APIタイプ：アチーブメント開封
            case SERVER_API.SERVER_API_CHECK_SNS_LINK: { m_PackResult.SetResult<RecvCheckSnsLink>(JsonMapper.ToObject<RecvCheckSnsLink>(m_wwwText)); break; }   //!< APIタイプ：SNSIDとの紐付け確認
            case SERVER_API.SERVER_API_SET_SNS_LINK: { m_PackResult.SetResult<RecvSetSnsLink>(JsonMapper.ToObject<RecvSetSnsLink>(m_wwwText)); break; } //!< APIタイプ：SNSIDとの紐付け
            case SERVER_API.SERVER_API_MOVE_SNS_SAVE_DATA: { m_PackResult.SetResult<RecvMoveSnsSaveData>(JsonMapper.ToObject<RecvMoveSnsSaveData>(m_wwwText)); break; } //!< APIタイプ：SNSIDを使用したデータ移行

            case SERVER_API.SERVER_API_TUTORIAL_SKIP: { m_PackResult.SetPlayerResult<RecvSkipTutorial>(m_wwwText); break; } //!< APIタイプ：チュートリアルスキップ

            case SERVER_API.SERVER_API_GET_SNS_ID: { m_PackResult.SetResult<RecvGetSnsID>(JsonMapper.ToObject<RecvGetSnsID>(m_wwwText)); break; }   //!< APIタイプ：SNSID作成

            case SERVER_API.SERVER_API_GET_POINT_SHOP_PRODUCT: { m_PackResult.SetResult<RecvGetPointShopProduct>(JsonMapper.ToObject<RecvGetPointShopProduct>(m_wwwText)); break; } //!< APIタイプ：ポイントショップ：ショップ商品情報を取得		get_point_shop_product
            case SERVER_API.SERVER_API_POINT_SHOP_PURCHASE: { m_PackResult.SetPlayerResult<RecvPointShopPurchase>(m_wwwText); break; }  //!< APIタイプ：ポイントショップ：商品購入					point_shop_purchase
            case SERVER_API.SERVER_API_POINT_SHOP_LIMITOVER: { m_PackResult.SetPlayerResult<RecvPointShopLimitOver>(m_wwwText); break; }    //!< APIタイプ：ポイントショップ：限界突破
            case SERVER_API.SERVER_API_POINT_SHOP_EVOL: { m_PackResult.SetPlayerResult<RecvPointShopEvol>(m_wwwText); break; }    //!< APIタイプ：ポイントショップ：進化

            case SERVER_API.SERVER_API_USE_ITEM: { m_PackResult.SetPlayerResult<RecvItemUse>(m_wwwText); break; }   //!< APIタイプ：消費アイテム：アイテム使用

            case SERVER_API.SERVER_API_GET_BOX_GACHA_STOCK: { m_PackResult.SetResult<RecvGetBoxGachaStock>(JsonMapper.ToObject<RecvGetBoxGachaStock>(m_wwwText)); break; }  //!< APIタイプ：BOXガチャ在庫状況取得
            case SERVER_API.SERVER_API_RESET_BOX_GACHA_STOCK: { m_PackResult.SetResult<RecvResetBoxGachaStock>(JsonMapper.ToObject<RecvResetBoxGachaStock>(m_wwwText)); break; }    //!< APIタイプ：BOXガチャ在庫状況リセット
            case SERVER_API.SERVER_API_SET_CURRENT_HERO: { m_PackResult.SetPlayerResult<RecvSetCurrentHero>(m_wwwText); break; }  //!< APIタイプ：主人公選択：主人公選択
            case SERVER_API.SERVER_API_EVOLVE_UNIT: { m_PackResult.SetPlayerResult<RecvEvolveUnit>(m_wwwText); break; }  //!< APIタイプ：ユニット：ユニット強化
            case SERVER_API.SERVER_API_GET_GUERRILLA_BOSS_INFO: { m_PackResult.SetResult<RecvGetGuerrillaBossInfo>(JsonMapper.ToObject<RecvGetGuerrillaBossInfo>(m_wwwText)); break; }    //!< APIタイプ：ゲリラボス情報取得
            case SERVER_API.SERVER_API_GET_GACHA_LINEUP: { m_PackResult.SetResult<RecvGetGachaLineup>(JsonMapper.ToObject<RecvGetGachaLineup>(m_wwwText)); break; }    //!< APIタイプ：ガチャ：ガチャラインナップ詳細
            case SERVER_API.SERVER_API_RENEW_TUTORIAL: { m_PackResult.SetResult<RecvRenewTutorialStep>(JsonMapper.ToObject<RecvRenewTutorialStep>(m_wwwText)); break; }	// APIタイプ：リニューアルチュートリアル関連：進行集計
            case SERVER_API.SERVER_API_GET_TOPIC_INFO: { m_PackResult.SetResult<RecvGetTopicInfo>(JsonMapper.ToObject<RecvGetTopicInfo>(m_wwwText)); break; }	// APIタイプ：ホームページのトピック : ニュース情報取得
            case SERVER_API.SERVER_API_GET_PRESENT_OPEN_LOG: { m_PackResult.SetResult<RecvGetPresentOpenLog>(JsonMapper.ToObject<RecvGetPresentOpenLog>(m_wwwText)); break; }    // APIタイプ：プレゼント関連：プレゼント開封ログ取得
            case SERVER_API.SERVER_API_GET_USER_SCORE_INFO: { m_PackResult.SetResult<RecvGetUserScoreInfo>(JsonMapper.ToObject<RecvGetUserScoreInfo>(m_wwwText)); break; }    // APIタイプ：スコア関連：ユーザースコア情報取得
            case SERVER_API.SERVER_API_GET_SCORE_REWARD: { m_PackResult.SetResult<RecvGetScoreReward>(JsonMapper.ToObject<RecvGetScoreReward>(m_wwwText)); break; }    // APIタイプ：スコア関連：ユーザースコア情報取得

            case SERVER_API.SERVER_API_GET_CHALLENGE_INFO: { m_PackResult.SetResult<RecvGetChallengeInfo>(JsonMapper.ToObject<RecvGetChallengeInfo>(m_wwwText)); break; }              //!< APIタイプ：成長ボスイベント情報取得
            case SERVER_API.SERVER_API_GET_CHALLENGE_REWARD: { m_PackResult.SetResult<RecvGetChallengeReward>(JsonMapper.ToObject<RecvGetChallengeReward>(m_wwwText)); break; }            //!< APIタイプ：成長ボスイベント報酬取得
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_START: { m_PackResult.SetResult<RecvChallengeQuestStart>(JsonMapper.ToObject<RecvChallengeQuestStart>(m_wwwText)); break; }           //!< APIタイプ：クエスト操作：成長ボスクエスト開始
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_CLEAR: { m_PackResult.SetPlayerResult<RecvChallengeQuestClear>(m_wwwText); break; }           //!< APIタイプ：クエスト操作：成長ボスクエストクリア
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_ORDER_GET: { m_PackResult.SetResult<RecvQuest2OrderGet>(JsonMapper.ToObject<RecvQuest2OrderGet>(m_wwwText)); break; }       //!< APIタイプ：クエスト操作：成長ボスクエスト受託情報取得
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_CESSAION_QUEST: { m_PackResult.SetResult<RecvQuest2CessaionQuest>(JsonMapper.ToObject<RecvQuest2CessaionQuest>(m_wwwText)); break; }  //!< APIタイプ：クエスト操作：成長ボスクエスト中断データ破棄通知
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_CONTINUE: { m_PackResult.SetResult<RecvQuestContinue>(JsonMapper.ToObject<RecvQuestContinue>(m_wwwText)); break; }        //!< APIタイプ：クエスト操作：コンティニュー
            case SERVER_API.SERVER_API_CHALLENGE_QUEST_RETIRE: { m_PackResult.SetPlayerResult<RecvQuestRetire>(m_wwwText); break; }          //!< APIタイプ：クエスト操作：クエストリタイア

            case SERVER_API.SERVER_API_DAY_STRADDLE: { m_PackResult.SetPlayerResult<RecvDayStraddle>(m_wwwText); break; }   //!< APIタイプ：日付変更情報取得
            default:
                Debug.LogError("parseJson() ApiType Error!!");
                break;
        }
    }

    /// <summary>
    /// リザルトコード生成
    /// </summary>
    /// <param name="eAPI"></param>
    /// <param name="eCode"></param>
    /// <returns></returns>
    private ResultCodeType ResultCodeToCodeType(SERVER_API eAPI, API_CODE eCode)
    {
        return ServerDataManager.Instance.ResultCodeToCodeType(eAPI, eCode);
    }

    /// <summary>
    /// リトライダイアログ
    /// </summary>
    public void openRetryDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);

        if (m_Result.m_Code == API_CODE.API_CODE_PAY_WIDE_APP_STORE_503)
        {
            // AppleStoreの処理が詰まってる場合は例外文言のダイアログを出す
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "error_response_title62");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "error_response_content62");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        }
        else
        {
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_RETRY_LIMIT_TITLE");
            newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_RETRY_LIMIT");
            newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "BTN_RECON");
        }

        newDialog.SetDialogEvent(DialogButtonEventType.OK,
        new System.Action(() =>
        {
            StartCoroutine(ErrorActionDelayMethod(3.0f, 2.0f, () =>
            {
                playmakerFSM.SendEvent("DO_NEXT");
            }));
        }));

        newDialog.Show();
    }

    /// <summary>
    /// ストアダイアログ
    /// </summary>
    public void openStoreDialog()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_VERSION_UP_TITLE");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "ERROR_VERSION_UP");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
#if UNITY_IPHONE
            Application.OpenURL(GlobalDefine.WEB_LINK_APPSTORE);
#elif UNITY_ANDROID
            Application.OpenURL(GlobalDefine.WEB_LINK_PLAYSTORE);
#else
			Application.OpenURL( GlobalDefine.WEB_LINK_PLAYSTORE );
#endif
            openStoreDialog();
        }));
        newDialog.Show();
    }

    /// <summary>
    /// ユーザー認証リクエスト
    /// </summary>
    public void authRequest()
    {
        ServerDataUtilSend.SendPacketAPI_UserAuthentication()
        .setSuccessAction(_data =>
        {
            UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvUserAuthentication>().result.hero_list;
            playmakerFSM.SendEvent("DO_NEXT");
        })
        .setErrorAction(_data =>
        {
            playmakerFSM.SendEvent("DO_NEXT");
        })
        .SendStart();
    }

    /// <summary>
    /// メンテナンスダイアログ
    /// </summary>
    public void openMentenanceDialog()
    {
        //リザルトデータ生成
        m_PackResult = new ResultData();
        m_PackResult.m_PacketAPI = m_PackData.m_PacketAPI;
        m_PackResult.m_PacketCode = m_Result.m_Code;
        m_PackResult.m_PacketUniqueNum = m_PackData.m_PacketUniqueNum;

        OpenDialog(0,
                    false,
                    "ERROR_MENTENANCE_TITLE",
                    "ERROR_MENTENANCE",
                    "common_button1",
                    null,
                    CHANGE_SCENE.CHANGE_SCENE_TITLE,
                    DIALOG_TYPE.DIALOG_TYPE_1,
                    () =>
                    {
                        m_errorAction(resultData);
                    });
    }

    /// <summary>
    /// アプリケーション終了
    /// </summary>
    public void appQuit()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);

        int code = -1;
        if (m_Result != null)
        {
            code = (int)m_Result.m_Code;
        }

        string title = String.Format(GameTextUtil.GetText("error_epplication_quit_title"), code);
        newDialog.SetDialogText(DialogTextType.Title, title);
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "error_epplication_quit_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            Application.Quit();
        }));
        newDialog.Show();

#if BUILD_TYPE_DEBUG
        OpenResponsDialog(m_PackData.m_PacketAPI, m_Result.m_Code);
#endif
    }

    /// <summary>
    /// 不正ユーザー
    /// </summary>
    public void invalidUser()
    {
        string strUserID = "???";
        if (LocalSaveManager.HasInstance)
        {
            uint unUserID = LocalSaveManager.Instance.LoadFuncUserID();
            strUserID = UnityUtil.CreateDrawUserID(unUserID);
        }

        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        // APIコードがフリーズの場合テキストが変わる
        {
            newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "ERROR_USER_INVALID_TITLE");
            newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("ERROR_USER_INVALID"), strUserID));
        }
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            invalidUser();
        }));
        newDialog.Show();
    }

    /// <summary>
    /// ユーザーデータ破棄
    /// </summary>
    public void deleteUserData()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "OTHERS_TRANSFER_QUIT_TITLE");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "OTHERS_TRANSFER_QUIT_DETAIL");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            //--------------------------------
            // 次回起動時にユーザー生成からやり直すため、
            // ここではセーブデータを全て破棄する
            //
            // 完全新規ユーザーとして作り直すため、
            // 無条件のセーブデータ全破棄を行う
            //--------------------------------
            LocalSaveUtil.ExecDataDelete();

            //--------------------------------
            //
            //--------------------------------
#if BUILD_TYPE_DEBUG
            Debug.Log("SaveData Moved Quit!");
#endif
            Application.Quit();
        }));
        newDialog.Show();

    }

    public void ServerDialogUpdate(SERVER_API eAPI, bool bEnable)
    {
        if (eAPI == SERVER_API.SERVER_API_TUTORIAL || eAPI == SERVER_API.SERVER_API_RENEW_TUTORIAL)
        {
            return;
        }

        if (bEnable)
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);
        }
        else
        {
            //--------------------------------
            // ダイアログ表示中などにローディング表示を強制的にオフる
            //--------------------------------
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.SERVER_API);
        }
    }

    /// <summary>
    /// Notice表示ダイアログ
    /// </summary>

    private void OpenResponsDialog(SERVER_API api, API_CODE code, Action action = null)
    {
#if BUILD_TYPE_DEBUG
        Dialog dialog = Dialog.Create(DialogType.DialogScrollInfo);
        dialog.SetDialogText(DialogTextType.Title, string.Format("ERROR:{0}", code));
        dialog.AddScrollInfoText("codeを確認してGoogleドライブのDGRN内で「APIエラー一覧（クライアント定義）」検索。資料でエラー内容を確認。サーバ担当者に問い合わせてください。\n\n" +
                                "Noticeと書かれている英文の場合は、サーバ担当者に画面キャプチャーを渡す。もしくは、直接見せて確認してください。\n\n" +
                                api + "\n\n" +
                                m_wwwText);
        dialog.SetDialogText(DialogTextType.OKText, GameTextUtil.GetText("common_button7"));
        dialog.EnableFadePanel();
        dialog.DisableAutoHide();
        dialog.SetOkEvent(() =>
        {
            if (action != null)
            {
                action();
            }
        });
        dialog.Show();
#endif
    }

    private IEnumerator ErrorActionDelayMethod(float waitTime, float addRange, Action action)
    {
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.RETRY_WAIT);
        }

        float wait = waitTime + UnityEngine.Random.Range(0.0f, addRange);

#if BUILD_TYPE_DEBUG
        Debug.Log("[checkResponse] ErrorActionDelayMethod waittime: " + wait);
#endif

        yield return new WaitForSeconds(wait);

        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.RETRY_WAIT);
        }

        action();
    }


    static string strCheckSumUUID = null;

    /// <summary>
    ///
    /// </summary>
    /// <param name="eAPIType"></param>
    /// <param name="strSendMessage"></param>
    /// <param name="unUniqueID"></param>
    /// <returns></returns>
    public static ServerApi Create(SERVER_API eAPIType,
                                    string strSendMessage,
                                     uint unUniqueID,
                                     string uuid = null)
    {
        strCheckSumUUID = (uuid != null) ? uuid : LocalSaveManager.Instance.LoadFuncUUID();

        GameObject _loadObj = Resources.Load("Prefab/ServerApi") as GameObject;

#if BUILD_TYPE_DEBUG && UNITY_EDITOR
        Debug.Log("API:" + eAPIType.ToString() + "\n REQUEST:" + strSendMessage.JsonPrettyFormat());
#endif
        GameObject _insObj = Instantiate(_loadObj);
        UnityUtil.SetObjectEnabledOnce(_insObj, true);

        ServerApi serverApi = _insObj.GetComponent<ServerApi>();
        serverApi.setupApi(eAPIType, strSendMessage, unUniqueID);

        return serverApi;
    }

    public static bool IsExists
    {
        get
        {
            GameObject obj = GameObject.FindGameObjectWithTag("ServerApi");
            if (obj == null) return false;
            ServerApi api = obj.GetComponent<ServerApi>();
            return (api != null);
        }
    }
}

