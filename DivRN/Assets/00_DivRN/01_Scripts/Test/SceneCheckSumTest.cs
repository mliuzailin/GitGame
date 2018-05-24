using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneCheckSumTest : MonoBehaviour
{

    [SerializeField]
    InputField uuid_input;

    [SerializeField]
    InputField checksum_input;

    [SerializeField]
    InputField response_input;

    [SerializeField]
    Text Answer;

    [SerializeField]
    Button button;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        //TD→access→uuidは[xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx]形式に変換すること
        uuid_input.text = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
        checksum_input.text = "000000000";
        response_input.text = "{\"header\":{\"code\":4096,\"achievement_new\":null,\"achievement_clear\":null,\"achievement_counter\":null,\"achievement_clear_data\":null,\"achievement_reward_count\":-1,\"achievement_new_count\":-1,\"session_id\":\"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\",\"api_version\":\"5.3.0\",\"packet_unique_id\":000,\"server_time\":1517458101,\"ott\":00000000,\"csum\":\"xxxxxxxxx\"},\"result\":null}";

        PacketData packData = new PacketData();
        packData.m_PacketAPI = ServerDataDefine.SERVER_API.SERVER_API_USER_AUTHENTICATION;
        bool bflag = ServerCheckSumUtil.CheckPacketCheckSum(response_input.text,
                                                            checksum_input.text,
                                                            packData,
                                                            uuid_input.text);

        ServerCheckSumInfo info = ServerCheckSumUtil.check_sum_info.Find((v) => v.packet_unique_num == packData.m_PacketUniqueNum);
        string ret = "";
        if (info != null)
        {
            ServerCheckSumUtil.check_sum_info.Remove(info);
            ret = string.Format("{0}\n local_check_sum = {1} \n server_check_sum = {2}", bflag ? "OK" : "NG", info.local_check_sum, info.server_check_sum);
        }
        else
        {
            ret = "Not Found CheckSumData";
        }
#if BUILD_TYPE_DEBUG
        Debug.Log(ret);
#endif

        Answer.text = ret;
    }
}
