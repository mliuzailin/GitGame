using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScratchStepButton : ButtonView
{
    [SerializeField]
    Image LotImage;
    [SerializeField]
    Image LotNumImage;
    [SerializeField]
    Image StepImage;
    [SerializeField]
    Image StepNumImage;
    [SerializeField]
    LayoutGroup TextList;

    private int updateLayoutCount = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (updateLayoutCount != 0)
        {
            updateLayoutCount--;
            if (updateLayoutCount < 0)
            {
                updateLayoutCount = 0;
            }
            updateLayout();
        }
    }

    public void SetStepImage(uint stepNum)
    {
        if (stepNum == 0)
        {
            // 初回限定
            StepImage.sprite = ResourceManager.Instance.Load("limit_step", ResourceType.Menu);
            StepNumImage.gameObject.SetActive(false);
        }
        else
        {
            // ステップ○
            StepImage.sprite = ResourceManager.Instance.Load("step", ResourceType.Menu);
            StepNumImage.gameObject.SetActive(true);
            StepNumImage.sprite = ResourceManager.Instance.Load("step_num_" + stepNum, ResourceType.Menu);
        }

        StepImage.SetNativeSize();
        StepNumImage.SetNativeSize();
        updateLayoutCount = 5;
    }

    public void SetLotImage(uint lotNum)
    {
        if (lotNum <= 1)
        {
            // めくる
            LotImage.sprite = ResourceManager.Instance.Load("mekuru", ResourceType.Menu);
            TextList.childAlignment = TextAnchor.MiddleCenter;
            LotNumImage.gameObject.SetActive(false);
        }
        else if (lotNum == 2)
        {
            // 連続でめくる
            TextList.childAlignment = TextAnchor.MiddleCenter;
            LotImage.sprite = ResourceManager.Instance.Load("renzoku_mekuru", ResourceType.Menu);
            LotNumImage.gameObject.SetActive(false);
        }
        else
        {
            // ○連続でめくる
            TextList.childAlignment = TextAnchor.MiddleLeft;
            LotImage.sprite = ResourceManager.Instance.Load("renzoku_mekuru", ResourceType.Menu);
            LotNumImage.sprite = ResourceManager.Instance.Load("btn_num_" + lotNum, ResourceType.Menu);
            LotNumImage.gameObject.SetActive(true);
        }

        LotImage.SetNativeSize();
        LotNumImage.SetNativeSize();
        updateLayoutCount = 5;
    }

    /// <summary>
    /// レイアウト更新
    /// </summary>
    public void updateLayout()
    {
        LayoutGroup[] layoutGroups = GetComponentsInChildren<LayoutGroup>();
        for (int i = 0; i < layoutGroups.Length; i++)
        {
            LayoutRebuilder.MarkLayoutForRebuild(layoutGroups[i].transform as RectTransform);
        }
    }

}
