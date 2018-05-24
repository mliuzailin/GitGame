using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインメニューの基本コード
/// </summary>
public class MainMenuScratch : MainMenuSeq
{
    private Scratch m_Scratch = null;
    private MasterDataGacha m_MasterDataGacha = null;
    private int m_UpdateLayoutCount = 0;
    private bool m_bReturnHome = false;

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

        if (m_UpdateLayoutCount != 0)
        {
            m_UpdateLayoutCount--;
            if (m_UpdateLayoutCount < 0)
            {
                m_UpdateLayoutCount = 0;
            }

            m_Scratch.updateLayout();
        }

        if (m_bReturnHome)
        {
            if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false))
            {
                m_bReturnHome = false;
            }
        }
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        if (m_Scratch == null)
        {
            m_Scratch = GetComponentInChildren<Scratch>();
            m_Scratch.SetPositionAjustStatusBar(new Vector2(0, -67.5f), new Vector2(0, -135));
        }

        // 看板イメージ（ATLAS未使用）
        setupScratch();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.GACHA;
    }

    private void setupScratch()
    {
        m_MasterDataGacha = MainMenuParam.m_GachaMaster;
        if (m_MasterDataGacha == null)
        {
            Debug.LogError("MasterData Not Found!");
            UnityUtil.SetObjectEnabledOnce(m_Scratch.gameObject, false);
            Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
            _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "sc149q_title");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "sc149q_content");
            _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
            _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
            {
                m_bReturnHome = true;
            });
            _newDialog.DisableCancelButton();
            _newDialog.Show();
            return;
        }
        UnityUtil.SetObjectEnabledOnce(m_Scratch.gameObject, true);
        m_Scratch.setup(m_MasterDataGacha);
        m_UpdateLayoutCount = 5;

        m_Scratch.OnClickNextButtonAction = OnClickFormNextButton;
        m_Scratch.OnClickPreviousButtonAction = OnClickFormPreviousButton;
    }

    public void changeScratch(int index)
    {
        m_Scratch.changeScratch(index);
    }

    /// <summary>
    /// 戻るボタンを押したとき
    /// </summary>
    void OnClickFormPreviousButton()
    {
        if (m_Scratch.m_CarouselRotator != null && m_Scratch.m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            m_Scratch.Step(false);
        }
    }

    /// <summary>
    /// 次ボタンを押したとき
    /// </summary>
    void OnClickFormNextButton()
    {
        if (m_Scratch.m_CarouselRotator != null && m_Scratch.m_CarouselToggler.moving == false)
        {
            SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);
            m_Scratch.Step(true);
        }
    }
}
