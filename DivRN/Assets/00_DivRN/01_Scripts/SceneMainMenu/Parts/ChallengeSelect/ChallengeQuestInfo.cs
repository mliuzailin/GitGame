using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ChallengeQuestInfo : MenuPartsBase
{
    //ボス
    M4uProperty<string> bossLabel = new M4uProperty<string>();
    public string BossLabel { get { return bossLabel.Value; } set { bossLabel.Value = value; } }

    M4uProperty<string> bossName = new M4uProperty<string>();
    public string BossName { get { return bossName.Value; } set { bossName.Value = value; } }

    //バトルカウント
    M4uProperty<string> countLabel = new M4uProperty<string>();
    public string CountLabel { get { return countLabel.Value; } set { countLabel.Value = value; } }

    M4uProperty<string> countValue = new M4uProperty<string>();
    public string CountValue { get { return countValue.Value; } set { countValue.Value = value; } }

    //経験値
    M4uProperty<string> expLabel = new M4uProperty<string>();
    public string ExpLabel { get { return expLabel.Value; } set { expLabel.Value = value; } }

    M4uProperty<string> expValue = new M4uProperty<string>();
    public string ExpValue { get { return expValue.Value; } set { expValue.Value = value; } }

    //コイン
    M4uProperty<string> coinLabel = new M4uProperty<string>();
    public string CoinLabel { get { return coinLabel.Value; } set { coinLabel.Value = value; } }

    M4uProperty<string> coinValue = new M4uProperty<string>();
    public string CoinValue { get { return coinValue.Value; } set { coinValue.Value = value; } }

    //スコア
    M4uProperty<string> scoreLabel = new M4uProperty<string>();
    public string ScoreLabel { get { return scoreLabel.Value; } set { scoreLabel.Value = value; } }

    M4uProperty<string> scoreValue = new M4uProperty<string>();
    public string ScoreValue { get { return scoreValue.Value; } set { scoreValue.Value = value; } }

    //スタミナ
    M4uProperty<string> staminaLabel = new M4uProperty<string>();
    public string StaminaLabel { get { return staminaLabel.Value; } set { staminaLabel.Value = value; } }

    M4uProperty<string> staminaValue = new M4uProperty<string>();
    public string StaminaValue { get { return staminaValue.Value; } set { staminaValue.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    private void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.adjustanchoredPosition(GetComponent<RectTransform>());
        }
    }
}
