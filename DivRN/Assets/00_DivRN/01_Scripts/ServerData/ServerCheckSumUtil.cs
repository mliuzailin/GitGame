/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	ServerCheckSumUtil.cs
	@brief	サーバー通信チェックサム関連ユーティリティ
	@author Developer
	@date 	2014/02/04
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using LitJson;

using ServerDataDefine;
using System.Text;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/


public class ServerCheckSumInfo
{
    public uint packet_unique_num = 0;
    public long local_check_sum = 0;
    public long server_check_sum = 0;

    public void Copy(ServerCheckSumInfo cSrc)
    {
        packet_unique_num = cSrc.packet_unique_num;
        local_check_sum = cSrc.local_check_sum;
        server_check_sum = cSrc.server_check_sum;
    }
}

//----------------------------------------------------------------------------
/*!
	@brief	サーバー通信チェックサム関連ユーティリティ
*/
//----------------------------------------------------------------------------
static public class ServerCheckSumUtil
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public static List<ServerCheckSumInfo> check_sum_info = new List<ServerCheckSumInfo>();

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	セキュリティ用チェックサムの構築
	*/
    //----------------------------------------------------------------------------
    static public bool CheckPacketCheckSum(string strRecvPacketJson, string strRecvCheckSum, PacketData packData = null, string Uuid = "")
    {
        //--------------------------------
        // チェックサムのカラムが入ってない状態でサーバー側はチェックサムを作成している。
        // 求める前に文字列を加工して、サーバー側がチェックサムを求めたタイミングと同じフォーマットに変換する
        //--------------------------------
        {
            {
                string strCheckSumPosKey = ",\"csum\":";
                string strCheckSumSample = ",\"csum\":\"FFFFFFFFF";

                int nCheckSumIndex = strRecvPacketJson.IndexOf(strCheckSumPosKey);
                int nCheckSumIndexAfter = strRecvPacketJson.IndexOf(strCheckSumPosKey) + strCheckSumSample.Length + 1;
                if (nCheckSumIndex > 0)
                {
                    //					Debug.LogError( "CheckSumConvert Prev - \n" + strRecvPacketJson );
                    strRecvPacketJson = strRecvPacketJson.Substring(0, nCheckSumIndex)
                                        + strRecvPacketJson.Substring(nCheckSumIndexAfter, strRecvPacketJson.Length - nCheckSumIndexAfter);
                    //					Debug.LogError( "CheckSumConvert After - \n" + strRecvPacketJson );
                }
            }
        }

        //--------------------------------
        //
        //--------------------------------
        if (strRecvCheckSum == null
        || strRecvCheckSum.Length <= 0
        )
        {
#if BUILD_TYPE_DEBUG
            Debug.Log("Recv CheckSum None...");
#endif
            return true;
        }

        {
            //sha1
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            //ユーザー作成時は正常終了後にuuidセットしてるので送ったjsonからuuid取り出す
            if (Uuid.Length <= 0 && packData.m_PacketAPI == SERVER_API.SERVER_API_USER_CREATE)
            {
                //JsonData型にパース
                JsonData sendMessageJsonData = JsonMapper.ToObject(packData.m_PacketSendMessage);
                //パース後のJsonData型にuuidがあるか？
                if (true == (sendMessageJsonData as IDictionary).Contains("uuid"))
                {
                    //jsonのuuidを設定
                    Uuid = (string)sendMessageJsonData["uuid"];
                }
            }
            var sha1Bs = sha1.ComputeHash(new UTF8Encoding().GetBytes(strRecvPacketJson + Uuid));
            strRecvPacketJson = BitConverter.ToString(sha1Bs).ToLower().Replace("-", "");
        }

        ServerCheckSumInfo checksumInfo = new ServerCheckSumInfo();
        checksumInfo.packet_unique_num = packData.m_PacketUniqueNum;
        checksumInfo.local_check_sum = 0;
        checksumInfo.server_check_sum = 0;
        check_sum_info.Add(checksumInfo);

#if BUILD_TYPE_DEBUG
        //Debug.Log(string.Format("checksum|{0}|{1}|{2}", Uuid, strRecvCheckSum, strRecvPacketJson));
#endif
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	セキュリティ用チェックサムの構築
	*/
    //----------------------------------------------------------------------------
    static public string CreatePacketCheckSum(string strPacketJson,
                                              SERVER_API packetAPI = SERVER_API.SERVER_API_NONE,
                                              string uuid = null)
    {
        {
            //sha1
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();

            string checkSumUuid = (uuid != null) ? uuid : LocalSaveManager.Instance.LoadFuncUUID();
            //ユーザー作成時は正常終了後にuuidセットしてるので送ったjsonからuuid取り出す
            if (packetAPI == SERVER_API.SERVER_API_USER_CREATE)
            {
                //JsonData型にパース
                JsonData sendMessageJsonData = JsonMapper.ToObject(strPacketJson);
                //パース後のJsonData型にuuidがあるか？
                if (true == (sendMessageJsonData as IDictionary).Contains("uuid"))
                {
                    //jsonのuuidを設定
                    checkSumUuid = (string)sendMessageJsonData["uuid"];
                }
            }

            var sha1Bs = sha1.ComputeHash(new UTF8Encoding().GetBytes(strPacketJson + checkSumUuid));
            strPacketJson = BitConverter.ToString(sha1Bs).ToLower().Replace("-", "");
        }

        return "";
    }
}

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
