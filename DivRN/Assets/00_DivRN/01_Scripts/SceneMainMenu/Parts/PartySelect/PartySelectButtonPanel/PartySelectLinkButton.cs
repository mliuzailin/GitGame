using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySelectLinkButton : ButtonView
{
    [SerializeField]
    public Image ButtonImage;

    [SerializeField]
    public Image TextImage;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// リンクボタンの選択状態を設定
    /// </summary>
    /// <param name="isSelect"></param>
    public void SetSelected(bool isSelect)
    {
        if (isSelect == true)
        {
            ButtonImage.sprite = ResourceManager.Instance.Load("btn_link_down", ResourceType.Common);
            TextImage.color = Color.cyan;
        }
        else
        {
            ButtonImage.sprite = ResourceManager.Instance.Load("btn_link", ResourceType.Common);
            TextImage.color = Color.white;
        }
    }
}
