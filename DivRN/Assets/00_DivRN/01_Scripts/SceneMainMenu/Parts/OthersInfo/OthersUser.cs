using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class OthersUser : MenuPartsBase
{
    public Action OnRenameClickAction = delegate { };
    public Action OnPasswordClickAction = delegate { };
    public Action OnGameDataDeleteClickAction = delegate { };
    public Action OnAchievementClickAction = delegate { };
    public Action OnLeaderboardClickAction = delegate { };

    M4uProperty<string> labelText = new M4uProperty<string>();
    public string LabelText { get { return labelText.Value; } set { labelText.Value = value; } }

    M4uProperty<string> nameText = new M4uProperty<string>();
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    M4uProperty<string> userNameText = new M4uProperty<string>();
    public string UserNameText { get { return userNameText.Value; } set { userNameText.Value = value; } }

    M4uProperty<string> userIDLabel = new M4uProperty<string>();
    public string UserIDLabel { get { return userIDLabel.Value; } set { userIDLabel.Value = value; } }

    M4uProperty<string> userIDText = new M4uProperty<string>();
    public string UserIDText { get { return userIDText.Value; } set { userIDText.Value = value; } }

    M4uProperty<string> renameText = new M4uProperty<string>();
    public string RenameText { get { return renameText.Value; } set { renameText.Value = value; } }

    M4uProperty<string> gameDataText = new M4uProperty<string>();
    public string GameDataText { get { return gameDataText.Value; } set { gameDataText.Value = value; } }

    M4uProperty<string> passwordText = new M4uProperty<string>();
    public string PasswordText { get { return passwordText.Value; } set { passwordText.Value = value; } }

    M4uProperty<string> gameDataDeleteText = new M4uProperty<string>();
    public string GameDataDeleteText { get { return gameDataDeleteText.Value; } set { gameDataDeleteText.Value = value; } }

    M4uProperty<string> purchaseHistoryText = new M4uProperty<string>();
    public string PurchaseHistoryText { get { return purchaseHistoryText.Value; } set { purchaseHistoryText.Value = value; } }

    M4uProperty<string> tipText = new M4uProperty<string>();
    public string TipText { get { return tipText.Value; } set { tipText.Value = value; } }

    M4uProperty<string> tollText = new M4uProperty<string>();
    public string TollText { get { return tollText.Value; } set { tollText.Value = value; } }

    M4uProperty<string> tollTipNum = new M4uProperty<string>();
    public string TollTipNum { get { return tollTipNum.Value; } set { tollTipNum.Value = value; } }

    M4uProperty<string> freeText = new M4uProperty<string>();
    public string FreeText { get { return freeText.Value; } set { freeText.Value = value; } }

    M4uProperty<string> freeTipNum = new M4uProperty<string>();
    public string FreeTipNum { get { return freeTipNum.Value; } set { freeTipNum.Value = value; } }

    M4uProperty<string> detailText = new M4uProperty<string>();
    public string DetailText { get { return detailText.Value; } set { detailText.Value = value; } }

    M4uProperty<string> achievementText = new M4uProperty<string>();
    public string AchievementText { get { return achievementText.Value; } set { achievementText.Value = value; } }

    M4uProperty<string> leaderboardText = new M4uProperty<string>();
    public string LeaderboardText { get { return leaderboardText.Value; } set { leaderboardText.Value = value; } }

    M4uProperty<bool> playGameActive = new M4uProperty<bool>();
    public bool PlayGameActive { get { return playGameActive.Value; } set { playGameActive.Value = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    void Start()
    {
        LabelText = GameTextUtil.GetText("he173p_title");
        NameText = GameTextUtil.GetText("he173p_text1");
        RenameText = GameTextUtil.GetText("he173p_text9");
        UserIDLabel = GameTextUtil.GetText("he173p_text10");
        GameDataText = GameTextUtil.GetText("he173p_text2");
        GameDataDeleteText = GameTextUtil.GetText("he173p_button2_2");
        PurchaseHistoryText = GameTextUtil.GetText("he173p_text3");
        TipText = GameTextUtil.GetText("he173p_text4");
        TollText = GameTextUtil.GetText("he173p_text5");
        FreeText = GameTextUtil.GetText("he173p_text7");
        DetailText = GameTextUtil.GetText("he173p_text8");
        AchievementText = GameTextUtil.GetText("he173p_button3");
        LeaderboardText = GameTextUtil.GetText("he173p_button4");
#if UNITY_ANDROID
        PlayGameActive = true;
#else
		PlayGameActive = false;
#endif
    }

    public void OnRenameClick()
    {
        OnRenameClickAction();
    }

    public void OnPasswordClick()
    {
        OnPasswordClickAction();
    }

    public void OnGameDataDeleteClick()
    {
        OnGameDataDeleteClickAction();
    }

    public void OnAchievementClick()
    {
        OnAchievementClickAction();
    }

    public void OnLeaderboardClick()
    {
        OnLeaderboardClickAction();
    }

}
