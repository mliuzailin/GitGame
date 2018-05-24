/**
 *  @file   MainMenuOthersKiyaku.cs
 *  @brief
 *  @author Developer
 *  @date   2017/03/06
 */

using UnityEngine;
using System.Collections;
using ServerDataDefine;

public class MainMenuOthersKiyaku : MainMenuSeq
{
    OthersInfo m_OthersInfo;

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

        m_OthersInfo = GetComponentInChildren<OthersInfo>();
        m_OthersInfo.SetTopAndBottomAjustStatusBar(new Vector2(-8, -348));
        m_OthersInfo.LabelText = GameTextUtil.GetText("he172p_title");

        CreateInfoList();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.NONE;
    }

    void CreateInfoList()
    {
        m_OthersInfo.Infos.Clear();

        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateWebViewWithCaptionKey("he172p_buttontitle1", Patcher.Instance.GetAgreementUrl(), null));
        string kessai_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_SHIKIN_KESSAI);
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateWebViewWithCaptionKey("he172p_buttontitle2", kessai_url, null));
        string torihiki_url = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_TOKUTEI_TORIHIKI);
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateWebViewWithCaptionKey("he172p_buttontitle3", torihiki_url, null));
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateWebViewWithCaptionKey("he172p_buttontitle4", Patcher.Instance.GetPolicyUrl(), null));
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateScrollDialogInfo("he172p_buttontitle5", "credit_text", null));
        m_OthersInfo.Infos.Add(OthersInfoListContext.CreateScrollDialogInfo("he172p_buttontitle6", "license_text", null));
    }
}
