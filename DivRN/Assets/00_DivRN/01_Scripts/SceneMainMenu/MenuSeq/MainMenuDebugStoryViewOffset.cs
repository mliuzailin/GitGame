/**
 *  @file   MainMenuDebugStoryViewOffset.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/24
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainMenuDebugStoryViewOffset : MainMenuSeq
{
    DebugPartyOffset m_DebugPartyOffset;
    MasterDataStoryChara[] m_StoryCharaMasterArray;
    float[] m_TilingArray;
    float[] m_OffsetXArray;
    float[] m_OffsetYArray;

    int m_Index = -1;

    MasterDataStoryChara m_CurrentStoryCharaMaster;
    AssetLoadStoryViewResource m_CharaResource;

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

        m_DebugPartyOffset.m_NoInputField.interactable = false;
        m_DebugPartyOffset.IsActiveStoryViewButton = true;
        m_DebugPartyOffset.ClickNextAction = OnClickNext;
        m_DebugPartyOffset.ClickPrevAction = OnClickPrev;
        m_DebugPartyOffset.ClickStoryViewAction = OnClickStoryView;
        m_DebugPartyOffset.ClickClearAction = OnClickClear;
        m_DebugPartyOffset.ChangedIDAction = OnChangedID;
        m_DebugPartyOffset.ChangedNumberAction = OnChangedNo;
        m_DebugPartyOffset.ChangedOffsetXAction = OnChangedOffsetX;
        m_DebugPartyOffset.ChangedOffsetYAction = OnChangedOffsetY;
        m_DebugPartyOffset.ChangedTilingAction = OnChangedTiling;

        CreateParam();
    }

    void CreateParam()
    {
        var storyCharaMasterList = MasterFinder<MasterDataStoryChara>.Instance.FindAll();
        storyCharaMasterList.RemoveAll((v) => v.fix_id == 0);
        m_StoryCharaMasterArray = storyCharaMasterList.ToArray();

        m_TilingArray = new float[m_StoryCharaMasterArray.Length];
        m_OffsetXArray = new float[m_StoryCharaMasterArray.Length];
        m_OffsetYArray = new float[m_StoryCharaMasterArray.Length];

        for (int i = 0; i < m_StoryCharaMasterArray.Length; ++i)
        {
            m_TilingArray[i] = m_StoryCharaMasterArray[i].img_tiling * 0.001f;
            m_OffsetXArray[i] = m_StoryCharaMasterArray[i].img_offset_x * 0.001f;
            m_OffsetYArray[i] = m_StoryCharaMasterArray[i].img_offset_y * 0.001f;
        }

        SetupStoryCharaMaster(0);
    }

    void SetupStoryCharaMaster(MasterDataStoryChara master)
    {
        if (master == null)
        {
            SetupStoryCharaMaster(0);
        }
        SetupStoryCharaMaster(Array.IndexOf(m_StoryCharaMasterArray, master));
    }

    void SetupStoryCharaMaster(int index)
    {
        //--------------------------------
        // アクセス番号決定
        //--------------------------------
        if (index >= m_StoryCharaMasterArray.Length)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = m_StoryCharaMasterArray.Length - 1;
        }

        MasterDataStoryChara storyCharaMaster = m_StoryCharaMasterArray[index];
        m_CurrentStoryCharaMaster = storyCharaMaster;
        m_Index = index;

        m_DebugPartyOffset.CheckAdjustUV();

        m_CharaResource = new AssetLoadStoryViewResource();

        m_DebugPartyOffset.NameText = "MasterDataStoryChara";
        m_DebugPartyOffset.m_IdInputField.text = storyCharaMaster.fix_id.ToString();
        m_DebugPartyOffset.SetupUV(m_OffsetXArray[m_Index]
                            , m_OffsetYArray[m_Index]
                            , m_TilingArray[m_Index]
                          );
        m_DebugPartyOffset.SetupAdjustUV(storyCharaMaster.img_offset_x * 0.001f, storyCharaMaster.img_offset_y * 0.001f, storyCharaMaster.img_tiling * 0.001f);
        m_DebugPartyOffset.CheckAdjustUV();

        m_CharaResource.SetStoryCharaId(storyCharaMaster.fix_id);
        StartCoroutine(LoopAssetLoad(() =>
        {
            m_DebugPartyOffset.ThirdUnitMaterial.SetTexture("_MainTex", m_CharaResource.GetTexture());
            SetUpUVParam();
        }));
    }

    void SetUpUVParam()
    {
        m_DebugPartyOffset.ThirdUnitMaterial.SetTextureOffset("_MainTex", new Vector2(m_OffsetXArray[m_Index], m_OffsetYArray[m_Index]));
        m_DebugPartyOffset.ThirdUnitMaterial.SetTextureScale("_MainTex", new Vector2(m_TilingArray[m_Index], m_TilingArray[m_Index]));
    }

    /// <summary>
    /// AssetBundleの読み込み待ち
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator LoopAssetLoad(Action action)
    {
        while (m_CharaResource.IsLoading == false)
        {
            yield return null;
        }

        action();
    }

    void OnClickNext()
    {
        SetupStoryCharaMaster(m_Index + 1);
    }

    void OnClickPrev()
    {
        SetupStoryCharaMaster(m_Index - 1);
    }

    void OnClickStoryView()
    {
        MasterDataStoryChara storycharaData = new MasterDataStoryChara();
        storycharaData.img_tiling = (int)(m_TilingArray[m_Index] * 1000);
        storycharaData.img_offset_x = (int)(m_OffsetXArray[m_Index] * 1000);
        storycharaData.img_offset_y = (int)(m_OffsetYArray[m_Index] * 1000);

        List<MasterDataStory> storyList = new List<MasterDataStory>();
        MasterDataStory s1 = new MasterDataStory();
        s1.show_character_01 = m_CurrentStoryCharaMaster.fix_id;
        s1.show_character_02 = m_CurrentStoryCharaMaster.fix_id;
        s1.show_character_03 = m_CurrentStoryCharaMaster.fix_id;
        s1.show_character_04 = m_CurrentStoryCharaMaster.fix_id;
        s1.show_character_01_slide = MasterDataDefineLabel.BoolType.ENABLE;
        s1.show_character_02_slide = MasterDataDefineLabel.BoolType.ENABLE;
        s1.show_character_03_slide = MasterDataDefineLabel.BoolType.ENABLE;
        s1.show_character_04_slide = MasterDataDefineLabel.BoolType.ENABLE;
        s1.text = string.Format("Tiling:{0} \nOffsetX:{1} \nOffsetY:{2}", m_TilingArray[m_Index].ToString("F3")
                                                                        , m_OffsetXArray[m_Index].ToString("F3")
                                                                        , m_OffsetYArray[m_Index].ToString("F3"));
        storyList.Add(s1);

        StoryView story = StoryView.Create();
        story.SetDebugStoryData(storyList.ToArray(), storycharaData);
        story.Show(() =>
        {

        });
    }

    void OnClickClear()
    {
        m_TilingArray[m_Index] = m_DebugPartyOffset.m_adjustT;
        m_OffsetXArray[m_Index] = m_DebugPartyOffset.m_adjustX;
        m_OffsetYArray[m_Index] = m_DebugPartyOffset.m_adjustY;
        SetUpUVParam();
    }

    void OnChangedID(uint value)
    {
        SetupStoryCharaMaster(MasterFinder<MasterDataStoryChara>.Instance.Find((int)value));
    }

    void OnChangedNo(uint value)
    {

    }

    void OnChangedOffsetX(float value)
    {
        m_OffsetXArray[m_Index] = value;
        SetUpUVParam();
    }

    void OnChangedOffsetY(float value)
    {
        m_OffsetYArray[m_Index] = value;
        SetUpUVParam();
    }

    void OnChangedTiling(float value)
    {
        m_TilingArray[m_Index] = value;
        SetUpUVParam();
    }
}
