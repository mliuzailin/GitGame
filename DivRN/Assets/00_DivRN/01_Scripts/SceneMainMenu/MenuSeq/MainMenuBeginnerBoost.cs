using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインメニューの基本コード
/// </summary>
public class MainMenuBeginnerBoost : MainMenuSeq
{
    BeginnerBoost m_BeginnerBoost;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    //ページ初期化処理
    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        // 表示をダイアログと同じにする
        CanvasSetting canvasSetting = m_CanvasObj.GetComponent<CanvasSetting>();
        if (canvasSetting != null)
        {
            canvasSetting.ChangeLayerType(CanvasSetting.LayerType.DIALOG);
        }

        m_BeginnerBoost = GetComponentInChildren<BeginnerBoost>();
        //m_BeginnerBoost.SetSizeParfect(new Vector2(0, -395));

        // シーンの最後に呼び出す
        m_BeginnerBoost.PostSceneStart();
    }
}
