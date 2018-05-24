/**
 *  @file   DebugCharaMesh.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/05
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;
using System;
using UnityEngine.UI;

public class DebugCharaMesh : M4uContextMonoBehaviour
{
    int m_Index = -1;
    MasterDataParamChara m_CurrentCharaMaster;
    [SerializeField]
    InputField m_IdInputField;
    [SerializeField]
    InputField m_NoInputField;

    MasterDataParamChara[] m_CharaMasterArray;


    M4uProperty<string> nameText = new M4uProperty<string>();
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    M4uProperty<Sprite> charaIcon = new M4uProperty<Sprite>();
    public Sprite CharaIcon { get { return charaIcon.Value; } set { charaIcon.Value = value; } }

    M4uProperty<bool> isUncompressed = new M4uProperty<bool>();
    public bool IsUncompressed { get { return isUncompressed.Value; } set { isUncompressed.Value = value; } }

    [SerializeField]
    AssetAutoSetCharaImage m_AssetAutoSetCharaImage;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }


    // Use this for initialization
    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.fitTopAndBottom(GetComponent<RectTransform>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Create()
    {
        var charaMasterList = MasterFinder<MasterDataParamChara>.Instance.FindAll();
        charaMasterList.RemoveAll((v) => v.fix_id == 0);
        m_CharaMasterArray = charaMasterList.ToArray();
    }

    public void SetUpParamChara(MasterDataParamChara charaMaster)
    {
        if (charaMaster == null)
        {
            SetUpParamChara(0);
        }
        SetUpParamChara(Array.IndexOf(m_CharaMasterArray, charaMaster));
    }


    public void SetUpParamChara(int index)
    {
        //--------------------------------
        // アクセス番号決定
        //--------------------------------
        if (index >= m_CharaMasterArray.Length)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = m_CharaMasterArray.Length - 1;
        }

        MasterDataParamChara charaMaster = m_CharaMasterArray[index];
        m_CurrentCharaMaster = charaMaster;
        m_Index = index;

        NameText = charaMaster.name;
        m_IdInputField.text = charaMaster.fix_id.ToString();
        m_NoInputField.text = charaMaster.draw_id.ToString();

        m_AssetAutoSetCharaImage.SetCharaID(charaMaster.fix_id, true);
        UnitIconImageProvider.Instance.Get(
            m_CurrentCharaMaster.fix_id,
            sprite =>
            {
                CharaIcon = sprite;
            });
    }

    public void OnClickNext()
    {
        SetUpParamChara(m_Index + 1);
    }

    public void OnClickPrev()
    {
        SetUpParamChara(m_Index - 1);
    }

    public void OnEndEditID(string value)
    {
        uint unitID;
        uint.TryParse(value, out unitID);
        SetUpParamChara(MasterDataUtil.GetCharaParamFromID(unitID));
    }

    public void OnEndEditNo(string value)
    {
        uint unitNo;
        uint.TryParse(value, out unitNo);
        SetUpParamChara(MasterDataUtil.GetCharaParamFromDrawID(unitNo));
    }

    public void OnClickUncompressed()
    {
        if (m_AssetAutoSetCharaImage.useUncompressed == true)
        {
            m_AssetAutoSetCharaImage.useUncompressed = false;
        }
        else
        {
            m_AssetAutoSetCharaImage.useUncompressed = true;
        }
        IsUncompressed = m_AssetAutoSetCharaImage.useUncompressed;
        SetUpParamChara(m_Index);
    }

}
