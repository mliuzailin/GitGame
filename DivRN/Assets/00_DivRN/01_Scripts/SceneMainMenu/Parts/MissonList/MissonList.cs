/**
 *  @file   MissonList.cs
 *  @brief  クエストのミッションリスト
 *  @author Developer
 *  @date   2016/11/14
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class MissonList : M4uContextMonoBehaviour
{
    /// <summary>
    /// ミッションアイテムリスト
    /// </summary>
    M4uProperty<List<MissonListItemContext>> missons = new M4uProperty<List<MissonListItemContext>>(new List<MissonListItemContext>());
    public List<MissonListItemContext> Missons
    {
        get
        {
            return missons.Value;
        }
        set
        {
            missons.Value = value;
        }
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ミッションアイテムを選択したとき
    /// </summary>
    /// <param name="misson"></param>
    public void SelectedMissonListItem(MissonListItemContext misson)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("SelectedMissonListItem");
#endif
    }

    /// <summary>
    /// ソートボタンを押したとき
    /// </summary>
    public void OnClickSortButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClickSortButton");
#endif
    }

    /// <summary>
    /// 戻るボタンを押したとき
    /// </summary>
    public void OnClickBackButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClickBackButton");
#endif
    }

    /// <summary>
    /// 一括受け取りボタンを押したとき
    /// </summary>
    public void OnClickAllGetButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClickAllGetButton");
#endif
    }

}
