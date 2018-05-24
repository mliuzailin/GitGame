using System.Linq;
using System.Collections;
using System.Collections.Generic;
using M4u;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionPager : M4uContextMonoBehaviour
{
    private static int PER_PAGE_COUNT = 30;

    public TextMeshProUGUI pageText;

    public GameObject leftArrowGO;
    public GameObject rightArrowGO;

    public M4uProperty<int> pageNo = new M4uProperty<int>();

    public int PageNo
    {
        get
        {
            return pageNo.Value;
        }
        set
        {
            pageNo.Value = value;
        }
    }


    public M4uProperty<int> maxPageNo = new M4uProperty<int>();

    public int MaxPageNo
    {
        get
        {
            return maxPageNo.Value;
        }
        set
        {
            maxPageNo.Value = value;
        }
    }


    public delegate void OnClickArrow();

    public OnClickArrow onClickRightArrow;
    public OnClickArrow onClickLeftArrow;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    public void OnClickRightArrow()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnClickRightArrow");
#endif
        onClickRightArrow();
    }

    public void OnClickLeftArrow()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL OnClickLeftArrow");
#endif
        onClickLeftArrow();
    }

    public void Refresh(int pageNo, uint totalRecordCount)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL MissionPager#Refresh:" + pageNo + " totalRecordCount:" + totalRecordCount);
#endif
        leftArrowGO.SetActive(false);
        rightArrowGO.SetActive(false);
        pageText.enabled = false;

        PageNo = pageNo;
        MaxPageNo = Mathf.CeilToInt((float)totalRecordCount / (float)PER_PAGE_COUNT);
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL MissionPager#Refresh:" + pageNo + " MaxPageNo:" + MaxPageNo);
#endif
        if (MaxPageNo <= 1)
        {
            return;
        }

        pageText.enabled = true;

        if (PageNo < MaxPageNo)
        {
            rightArrowGO.SetActive(true);
        }

        if (PageNo > 1)
        {
            leftArrowGO.SetActive(true);
        }
    }
}
