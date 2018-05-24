using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleSceneStartCharaUI : MonoBehaviour
{
    private int m_CharaID0 = 0;
    private int m_CharaID = 0;
    private int m_CharaLevel = 0;
    private int m_CharaLevelMax = 0;

    // Update is called once per frame
    void Update()
    {
        int chara_id = 0;
        if (transform.Find("InputField").GetComponent<InputField>().text != "")
        {
            chara_id = transform.Find("InputField").GetComponent<InputField>().text.ToInt(0);
        }

        if (m_CharaID0 != chara_id)
        {
            m_CharaID0 = chara_id;
            if (chara_id < 0)
            {
                MasterDataParamChara[] master_data_array = MasterFinder<MasterDataParamChara>.Instance.GetAll();
                for (int idx = 0; idx < master_data_array.Length; idx++)
                {
                    MasterDataParamChara aaa = master_data_array[idx];
                    if (aaa.draw_id == -chara_id)
                    {
                        chara_id = (int)aaa.fix_id;
                        break;
                    }
                }
            }


            m_CharaID = chara_id;

            MasterDataParamChara master_data = BattleParam.m_MasterDataCache.useCharaParam((uint)m_CharaID);
            if (master_data != null)
            {
                m_CharaLevelMax = master_data.level_max;

                transform.Find("TextCharaName").GetComponent<Text>().text = master_data.name;
            }
            else
            {
                m_CharaLevelMax = 0;
                transform.Find("TextCharaName").GetComponent<Text>().text = "(null)";
            }
        }

        if (m_CharaLevelMax > 1)
        {
            m_CharaLevel = (int)(transform.Find("Slider").GetComponent<Slider>().value * (m_CharaLevelMax - 1)) + 1;
            transform.Find("TextLevel").GetComponent<Text>().text = "LV:" + m_CharaLevel.ToString() + "/" + m_CharaLevelMax.ToString();
        }
        else if (m_CharaLevelMax == 1)
        {
            m_CharaLevel = 1;
            transform.Find("TextLevel").GetComponent<Text>().text = "LV:1/1";
        }
        else
        {
            transform.Find("TextLevel").GetComponent<Text>().text = "LV:-/-";
        }
    }

    public void setOn(bool is_on)
    {
        transform.Find("Toggle").GetComponent<Toggle>().isOn = is_on;
    }

    public bool isOn()
    {
        return transform.Find("Toggle").GetComponent<Toggle>().isOn;
    }

    public void setup(int fix_id, int chara_level)
    {
        MasterDataParamChara master_data = BattleParam.m_MasterDataCache.useCharaParam((uint)fix_id);
        if (master_data != null)
        {
            m_CharaID = (int)master_data.fix_id;
            m_CharaLevel = chara_level;
            m_CharaLevelMax = master_data.level_max;

            transform.Find("InputField").GetComponent<InputField>().text = m_CharaID.ToString();
            transform.Find("TextCharaName").GetComponent<Text>().text = master_data.name;
            transform.Find("TextLevel").GetComponent<Text>().text = "LV:" + m_CharaLevel.ToString() + "/" + m_CharaLevelMax.ToString();
            transform.Find("Slider").GetComponent<Slider>().value = (m_CharaLevel - 1) / (float)(m_CharaLevelMax - 1);
        }
    }

    public int getCharaID()
    {
        return m_CharaID;
    }

    public int getCharaLevel()
    {
        return m_CharaLevel;
    }


    public void OnPushButtonPrev()
    {
        int prev_id = 0;

        MasterDataParamChara[] master_data_array = MasterFinder<MasterDataParamChara>.Instance.GetAll();
        List<MasterDataParamChara> aaa = new List<MasterDataParamChara>(master_data_array);
        aaa.Sort((a, b) => (int)a.fix_id - (int)b.fix_id);
        master_data_array = aaa.ToArray();
        for (int idx = master_data_array.Length - 1; idx >= 0; idx--)
        {
            MasterDataParamChara master_data = master_data_array[idx];
            if (master_data.fix_id < m_CharaID)
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

        MasterDataParamChara[] master_data_array = MasterFinder<MasterDataParamChara>.Instance.GetAll();
        List<MasterDataParamChara> aaa = new List<MasterDataParamChara>(master_data_array);
        aaa.Sort((a, b) => (int)a.fix_id - (int)b.fix_id);
        master_data_array = aaa.ToArray();
        for (int idx = 0; idx < master_data_array.Length; idx++)
        {
            MasterDataParamChara master_data = master_data_array[idx];
            if (master_data.fix_id > m_CharaID)
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
