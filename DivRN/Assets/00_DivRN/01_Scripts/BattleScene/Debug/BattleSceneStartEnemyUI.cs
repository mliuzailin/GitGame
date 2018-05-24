using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleSceneStartEnemyUI : MonoBehaviour
{
    private int m_EnemyGroupID = 0;

    // Update is called once per frame
    void Update()
    {
        int enemy_group_id = 0;
        if (transform.Find("InputField").GetComponent<InputField>().text != "")
        {
            enemy_group_id = transform.Find("InputField").GetComponent<InputField>().text.ToInt(0);
        }

        if (m_EnemyGroupID != enemy_group_id)
        {
            m_EnemyGroupID = enemy_group_id;

            MasterDataEnemyGroup master_data = null;
            if (m_EnemyGroupID > 0)
            {
                master_data = BattleParam.m_MasterDataCache.useEnemyGroup((uint)m_EnemyGroupID);
            }
            if (master_data != null)
            {
                transform.Find("TextCharaName").GetComponent<Text>().color = Color.white;
            }
            else
            {
                transform.Find("TextCharaName").GetComponent<Text>().color = Color.gray;
            }
        }
    }

    public void setActive(bool is_active)
    {
        transform.Find("Toggle").GetComponent<Toggle>().isOn = is_active;
    }

    public MasterDataEnemyGroup getEnemyGroup()
    {
        if (transform.Find("Toggle").GetComponent<Toggle>().isOn)
        {
            MasterDataEnemyGroup master_data = null;
            if (m_EnemyGroupID > 0)
            {
                master_data = BattleParam.m_MasterDataCache.useEnemyGroup((uint)m_EnemyGroupID);
                return master_data;
            }
        }
        return null;
    }

    public int getEnemyGroupID()
    {
        if (transform.Find("Toggle").GetComponent<Toggle>().isOn)
        {
            return m_EnemyGroupID;
        }
        return 0;
    }


    public void OnPushButtonPrev()
    {
        int prev_id = 0;

        MasterDataEnemyGroup[] master_data_array = BattleParam.m_MasterDataCache.getAllEnemyGroup();
        List<MasterDataEnemyGroup> aaa = new List<MasterDataEnemyGroup>(master_data_array);
        aaa.Sort((a, b) => (int)a.fix_id - (int)b.fix_id);
        master_data_array = aaa.ToArray();
        for (int idx = master_data_array.Length - 1; idx >= 0; idx--)
        {
            MasterDataEnemyGroup master_data = master_data_array[idx];
            if (master_data.fix_id < m_EnemyGroupID)
            {
                prev_id = (int)master_data.fix_id;
                break;
            }
        }

        if (prev_id == 0)
        {
            prev_id = (int)master_data_array[master_data_array.Length - 1].fix_id;
        }

        transform.Find("InputField").GetComponent<InputField>().text = prev_id.ToString();
    }

    public void OnPushButtonNext()
    {
        int next_id = 0;

        MasterDataEnemyGroup[] master_data_array = BattleParam.m_MasterDataCache.getAllEnemyGroup();
        List<MasterDataEnemyGroup> aaa = new List<MasterDataEnemyGroup>(master_data_array);
        aaa.Sort((a, b) => (int)a.fix_id - (int)b.fix_id);
        master_data_array = aaa.ToArray();
        for (int idx = 0; idx < master_data_array.Length; idx++)
        {
            MasterDataEnemyGroup master_data = master_data_array[idx];
            if (master_data.fix_id > m_EnemyGroupID)
            {
                next_id = (int)master_data.fix_id;
                break;
            }
        }

        if (next_id == 0)
        {
            next_id = (int)master_data_array[0].fix_id;
        }

        transform.Find("InputField").GetComponent<InputField>().text = next_id.ToString();
    }
}
