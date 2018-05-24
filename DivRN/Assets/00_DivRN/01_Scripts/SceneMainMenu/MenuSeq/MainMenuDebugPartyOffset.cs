/**
 *  @file   MainMenuDebugPartyOffset.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/02
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainMenuDebugPartyOffset : MainMenuSeq
{
    DebugPartyOffset m_DebugPartyOffset;

    GlobalDefine.PartyCharaIndex currentUVTarget = GlobalDefine.PartyCharaIndex.LEADER;

    GlobalDefine.PartyCharaIndex CurrentUVTarget
    {
        get
        {
            return currentUVTarget;
        }
        set
        {
            currentUVTarget = value;
        }

    }

    int m_Index = -1;
    float[,] m_TilingArray;
    float[,] m_OffsetXArray;
    float[,] m_OffsetYArray;

    MasterDataParamChara[] m_CharaMasterArray;

#if BUILD_TYPE_DEBUG
    NewBattleSkillCutin m_NewBattleSkillCutin = null;   // スキルカットイン表示テスト用
#endif

    MasterDataParamChara m_CurrentCharaMaster;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        m_DebugPartyOffset = m_CanvasObj.GetComponentInChildren<DebugPartyOffset>();
        if (m_DebugPartyOffset == null) { return; }

        m_DebugPartyOffset.IsActiveSkillCutinButton = true;
        m_DebugPartyOffset.ClickNextAction = OnClickNext;
        m_DebugPartyOffset.ClickPrevAction = OnClickPrev;
        m_DebugPartyOffset.ClickSkillCutinAction = OnClickSkillCutin;
        m_DebugPartyOffset.ClickClearAction = OnClickClear;
        m_DebugPartyOffset.ChangedIDAction = OnChangedID;
        m_DebugPartyOffset.ChangedNumberAction = OnChangedNo;
        m_DebugPartyOffset.ChangedOffsetXAction = OnChangedOffsetX;
        m_DebugPartyOffset.ChangedOffsetYAction = OnChangedOffsetY;
        m_DebugPartyOffset.ChangedTilingAction = OnChangedTiling;
        Create();
        SetUpParamChara(0);
    }


    public void Create()
    {
        var charaMasterList = MasterFinder<MasterDataParamChara>.Instance.FindAll();
        charaMasterList.RemoveAll((v) => v.fix_id == 0);
        m_CharaMasterArray = charaMasterList.ToArray();

        m_TilingArray = new float[m_CharaMasterArray.Length, Enum.GetNames(typeof(GlobalDefine.PartyCharaIndex)).Length];
        m_OffsetXArray = new float[m_CharaMasterArray.Length, Enum.GetNames(typeof(GlobalDefine.PartyCharaIndex)).Length];
        m_OffsetYArray = new float[m_CharaMasterArray.Length, Enum.GetNames(typeof(GlobalDefine.PartyCharaIndex)).Length];
        CurrentUVTarget = GlobalDefine.PartyCharaIndex.MOB_2;

        for (int i = 0; i < m_CharaMasterArray.Length; i++)
        {
            MasterDataParamChara charaMaster = m_CharaMasterArray[i];

            m_OffsetXArray[i, (int)GlobalDefine.PartyCharaIndex.MOB_2] = (float)charaMaster.img_2_offsetX * 0.001f;
            m_OffsetYArray[i, (int)GlobalDefine.PartyCharaIndex.MOB_2] = (float)charaMaster.img_2_offsetY * 0.001f;
            m_TilingArray[i, (int)GlobalDefine.PartyCharaIndex.MOB_2] = (float)charaMaster.img_2_tiling * 0.001f;
        }
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

        m_DebugPartyOffset.NameText = charaMaster.name;
        m_DebugPartyOffset.m_IdInputField.text = charaMaster.fix_id.ToString();
        m_DebugPartyOffset.m_NoInputField.text = charaMaster.draw_id.ToString();

        m_DebugPartyOffset.SetupUV(m_OffsetXArray[m_Index, (int)CurrentUVTarget]
                                    , m_OffsetYArray[m_Index, (int)CurrentUVTarget]
                                    , m_TilingArray[m_Index, (int)CurrentUVTarget]
                                  );

        m_DebugPartyOffset.SetupAdjustUV(charaMaster.img_2_offsetX * 0.001f, charaMaster.img_2_offsetY * 0.001f, charaMaster.img_2_tiling * 0.001f);
        m_DebugPartyOffset.CheckAdjustUV();
        AssetBundler.Create().SetAsUnitTexture(charaMaster.fix_id,
            (o) =>
            {
                Texture2D tex = o.GetTexture2D(TextureWrapMode.Clamp);
                m_DebugPartyOffset.ThirdUnitMaterial.SetTexture("_MainTex", tex);
                foreach (GlobalDefine.PartyCharaIndex charaIndex in Enum.GetValues(typeof(GlobalDefine.PartyCharaIndex)))
                {
                    SetUpUVParam(charaIndex);
                }
            },
            (s) =>
            {
                m_DebugPartyOffset.ThirdUnitMaterial.SetTexture("_MainTex", null);
            }).Load();

    }

    void SetUpUVParam(GlobalDefine.PartyCharaIndex charaIndex)
    {
        int index = (int)charaIndex;
        if (index < 0 || m_OffsetXArray.GetLength(1) <= index) { return; }
        Rect rect = new Rect(m_OffsetXArray[m_Index, index],
                         m_OffsetYArray[m_Index, index],
                         m_TilingArray[m_Index, index],
                         m_TilingArray[m_Index, index]);
        SetUpUVParam(charaIndex, rect);

    }

    void SetUpUVParam(GlobalDefine.PartyCharaIndex charaIndex, Rect rect)
    {
        if (charaIndex == GlobalDefine.PartyCharaIndex.MOB_2)
        {
            m_DebugPartyOffset.ThirdUnitMaterial.SetTextureOffset("_MainTex", rect.position);
            m_DebugPartyOffset.ThirdUnitMaterial.SetTextureScale("_MainTex", rect.size);
        }
    }

    public void OnClickSkillCutin()
    {
#if BUILD_TYPE_DEBUG
        if (m_NewBattleSkillCutin == null)
        {
            GameObject skill_cutin_prefab = Resources.Load<GameObject>("Prefab/BattleScene/NewBattleSkillCutin");
            if (skill_cutin_prefab != null)
            {
                GameObject skill_cutin_object = Instantiate(skill_cutin_prefab);
                if (skill_cutin_object != null)
                {
                    m_NewBattleSkillCutin = skill_cutin_object.GetComponent<NewBattleSkillCutin>();
                    if (m_NewBattleSkillCutin != null)
                    {
                        skill_cutin_object.transform.SetParent(m_DebugPartyOffset.transform.parent, false);
                    }
                    else
                    {
                        Destroy(skill_cutin_object);
                    }
                }
            }
        }

        if (m_NewBattleSkillCutin != null
        && m_CurrentCharaMaster != null)
        {
            int fix_id = (int)m_CurrentCharaMaster.fix_id;
            int img_2_offsetX = (int)(m_OffsetXArray[m_Index, (int)CurrentUVTarget] * 1000);
            int img_2_offsetY = (int)(m_OffsetYArray[m_Index, (int)CurrentUVTarget] * 1000);
            int img_2_tiling = (int)(m_TilingArray[m_Index, (int)CurrentUVTarget] * 1000);
            int size_width = m_CurrentCharaMaster.size_width;
            int size_height = m_CurrentCharaMaster.size_height;
            MasterDataDefineLabel.PivotType pivot_type = m_CurrentCharaMaster.pivot;
            int side_offset = m_CurrentCharaMaster.side_offset;
            m_NewBattleSkillCutin.debugSkillCutin(fix_id, img_2_tiling, img_2_offsetX, img_2_offsetY, size_width, size_height, pivot_type, side_offset);
        }
#endif
    }

    public void OnClickNext()
    {
        SetUpParamChara(m_Index + 1);
    }

    public void OnClickPrev()
    {
        SetUpParamChara(m_Index - 1);
    }

    public void OnClickClear()
    {
        float x = 0;
        float y = 0;
        float t = 0;

        x = (float)m_CurrentCharaMaster.img_2_offsetX * 0.001f;
        y = (float)m_CurrentCharaMaster.img_2_offsetY * 0.001f;
        t = (float)m_CurrentCharaMaster.img_2_tiling * 0.001f;

        m_OffsetXArray[m_Index, (int)CurrentUVTarget] = x;
        m_OffsetYArray[m_Index, (int)CurrentUVTarget] = y;
        m_TilingArray[m_Index, (int)CurrentUVTarget] = t;
        SetUpUVParam(CurrentUVTarget);
    }

    public void OnChangedID(uint value)
    {
        SetUpParamChara(MasterDataUtil.GetCharaParamFromID(value));
    }

    public void OnChangedNo(uint value)
    {
        SetUpParamChara(MasterDataUtil.GetCharaParamFromDrawID(value));
    }

    public void OnChangedOffsetY(float value)
    {
        m_OffsetYArray[m_Index, (int)CurrentUVTarget] = value;
        SetUpUVParam(CurrentUVTarget);
    }

    public void OnChangedOffsetX(float value)
    {
        m_OffsetXArray[m_Index, (int)CurrentUVTarget] = value;
        SetUpUVParam(CurrentUVTarget);
    }

    public void OnChangedTiling(float value)
    {
        m_TilingArray[m_Index, (int)CurrentUVTarget] = value;
        SetUpUVParam(CurrentUVTarget);
    }
}
