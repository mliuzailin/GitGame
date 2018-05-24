using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerDataDefine;
using M4u;

public class FriendDataItem : View
{
    [SerializeField]
    private GameObject m_iconButtonRoot;
    [SerializeField]
    private GameObject m_selectButtonRoot;
    [SerializeField]
    Image m_RaceImage;
    [SerializeField]
    GameObject m_SlashText;
    [SerializeField]
    Image m_SubRaceImage;
    [SerializeField]
    Image m_AttributeImage;
    [SerializeField]
    Image m_FlagImage;

    public enum FlagType
    {
        NONE = -1,
        FRIEND,
        HELPER,
        MAX,
    }

    public enum ParamType
    {
        NONE = -1,
        SKILL,
        NAME,
        MAX,
    }
    public GameObject paramListObj = null;
    public GameObject skillListObj = null;

    public Image SelectButtonImage = null;

    //CharaIcon
    public Sprite IconImage
    {
        set
        {
            if (m_IconButton != null && m_IconButton.IconImage != null)
            {
                m_IconButton.IconImage.sprite = value;
            }
        }
    }

    //Element
    public Sprite ElementImage
    {
        set
        {
            if (m_IconButton != null && m_IconButton.ElementImage != null)
            {
                m_IconButton.ElementImage.sprite = value;
            }
        }
    }

    //Link
    M4uProperty<Sprite> linkImage = new M4uProperty<Sprite>();
    public Sprite LinkImage { get { return linkImage.Value; } set { linkImage.Value = value; } }

    M4uProperty<bool> isActiveLink = new M4uProperty<bool>();
    public bool IsActiveLink { get { return isActiveLink.Value; } set { isActiveLink.Value = value; } }

    //Status
    M4uProperty<string> statusValue = new M4uProperty<string>();
    public string StatusValue { get { return statusValue.Value; } set { statusValue.Value = value; } }

    //BGColor
    public Color BgColor
    {
        set
        {
            if (m_IconButton != null && m_IconButton.BGImage != null)
            {
                m_IconButton.BGImage.color = value;
            }
        }
    }

    //Rank
    M4uProperty<string> rank = new M4uProperty<string>();
    public string Rank { get { return rank.Value; } set { rank.Value = value; } }

    //Name
    new M4uProperty<string> name = new M4uProperty<string>();
    public string Name { get { return name.Value; } set { name.Value = value; } }

    M4uProperty<string> time = new M4uProperty<string>();
    public string Time { get { return time.Value; } set { time.Value = value; } }

    M4uProperty<bool> isViewFriendPoint = new M4uProperty<bool>(false);
    /// <summary>フレンドポイントの表示・非表示</summary>
    public bool IsViewFriendPoint { get { return isViewFriendPoint.Value; } set { isViewFriendPoint.Value = value; } }

    M4uProperty<string> friendPointText = new M4uProperty<string>();
    /// <summary>フレンドポイント</summary>
    public string FriendPointText { get { return friendPointText.Value; } set { friendPointText.Value = value; } }

    // Charm
    M4uProperty<bool> isViewCharm = new M4uProperty<bool>(false);
    public bool IsViewCharm { get { return isViewCharm.Value; } set { isViewCharm.Value = value; } }

    M4uProperty<string> charmLabel = new M4uProperty<string>();
    private string CharmLabel { get { return charmLabel.Value; } set { charmLabel.Value = value; } }

    M4uProperty<string> charmValue = new M4uProperty<string>();
    public string CharmValue { get { return charmValue.Value; } set { charmValue.Value = value; } }

    //Flag
    public Sprite FlagImage
    {
        set
        {
            if (m_FlagImage != null)
            {
                m_FlagImage.sprite = value;
            }
        }
    }

    public bool IsViewFlag
    {
        set
        {
            if (m_FlagImage != null)
            {
                m_FlagImage.gameObject.SetActive(value);
            }
        }
    }

    //Lock
    M4uProperty<bool> isActiveLock = new M4uProperty<bool>();
    public bool IsActiveLock { get { return isActiveLock.Value; } set { isActiveLock.Value = value; } }

    M4uProperty<bool> isEnableButton = new M4uProperty<bool>(false);
    public bool IsEnableButton { get { return isEnableButton.Value; } set { isEnableButton.Value = value; } }

    M4uProperty<string> charaName = new M4uProperty<string>();
    public string CharaName { get { return charaName.Value; } set { charaName.Value = value; } }

    M4uProperty<uint> rarity = new M4uProperty<uint>();
    public uint Rarity { get { return rarity.Value; } set { rarity.Value = value; } }

    M4uProperty<string> raceLabel = new M4uProperty<string>();
    public string RaceLabel { get { return raceLabel.Value; } set { raceLabel.Value = value; } }

    M4uProperty<Sprite> raceImage = new M4uProperty<Sprite>();
    public Sprite RaceImage
    {
        get { return raceImage.Value; }
        set
        {
            raceImage.Value = value;
            IsActiveRace = (value != null);
        }
    }

    M4uProperty<bool> isActiveRace = new M4uProperty<bool>(false);
    bool IsActiveRace { get { return isActiveRace.Value; } set { isActiveRace.Value = value; } }

    M4uProperty<Sprite> subRaceImage = new M4uProperty<Sprite>();
    public Sprite SubRaceImage
    {
        get { return subRaceImage.Value; }
        set
        {
            subRaceImage.Value = value;
            IsActiveSubRace = (value != null);
        }
    }

    M4uProperty<bool> isActiveSubRace = new M4uProperty<bool>(false);
    bool IsActiveSubRace { get { return isActiveSubRace.Value; } set { isActiveSubRace.Value = value; } }

    M4uProperty<string> attributeLabel = new M4uProperty<string>();
    public string AttributeLabel { get { return attributeLabel.Value; } set { attributeLabel.Value = value; } }

    public Sprite AttributeImage
    {
        set
        {
            if (m_AttributeImage != null)
            {
                m_AttributeImage.sprite = value;
                m_AttributeImage.gameObject.SetActive(value != null);
            }
        }
    }

    public Color AttributeImageColor
    {
        set
        {
            if (m_AttributeImage != null)
            {
                m_AttributeImage.color = value;
            }
        }
    }

    public int Index = 0;
    public FriendDataSetting friendDataSetting = null;
    public PacketStructFriend FriendData { get { return friendDataSetting.FriendData; } }
    public MasterDataParamChara MasterData { get { return friendDataSetting.MasterData; } }

    FriendListIconButton m_IconButton;
    FriendListSelectButton m_SelectButton;
    private string m_SpriteName = string.Empty;
    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        BgColor = new Color(0.5f, 0.5f, 0.5f, 1);
        setFlag(FlagType.NONE);
        IsActiveLock = false;
        IsActiveLink = false;

        SetUpButtons();
    }

    public void setFlag(FlagType _type)
    {
        switch (_type)
        {
            case FlagType.NONE:
                IsViewFlag = false;
                break;
            case FlagType.FRIEND:
                IsViewFlag = true;
                FlagImage = ResourceManager.Instance.Load("flag_furendo", ResourceType.Menu);
                break;
            case FlagType.HELPER:
                IsViewFlag = true;
                FlagImage = ResourceManager.Instance.Load("suketto2", ResourceType.Menu);
                break;
        }
    }

    public void setup(int _index, FriendDataSetting _friend, ParamType _type, bool bCheckLock)
    {
        Index = _index;

        friendDataSetting = _friend;

        m_SpriteName = string.Empty;
        // アイコン
        UnitIconImageProvider.Instance.Get(
            MasterData.fix_id,
            ref m_SpriteName,
            sprite =>
            {
                if (MainMenuUtil.IsWriteIcon(ref m_SpriteName, sprite))
                {
                    IconImage = sprite;
                }

            });

        RaceLabel = GameTextUtil.GetText("unit_status2");
        AttributeLabel = GameTextUtil.GetText("unit_status3");

        Name = FriendData.user_name; // ユーザーネーム
        string rankFormat = GameTextUtil.GetText("questfriend_text1");
        Rank = string.Format(rankFormat, FriendData.user_rank);// ランク

        //お気に入りチェック
        IsActiveLock = false;
        if (bCheckLock)
        {
            IsActiveLock = MainMenuUtil.ChkFavoridFriend(FriendData.user_id);
        }

        StatusValue = (FriendData.unit.level >= MasterData.level_max) ?
            GameTextUtil.GetText("uniticon_flag1") :
            string.Format(GameTextUtil.GetText("uniticon_flag2"), FriendData.unit.level); // レベル
        uint plusPoint = FriendData.unit.add_hp + FriendData.unit.add_pow; // プラス値の計算
        if (plusPoint != 0) StatusValue += string.Format(GameTextUtil.GetText("uniticon_flag3"), plusPoint);

        //属性
        ElementImage = MainMenuUtil.GetElementCircleSprite(MasterData.element);

        IsActiveLink = (FriendData.unit_link.id == 0) ? false : true;
        if (IsActiveLink)
        {
            LinkImage = MainMenuUtil.GetLinkMark(FriendData.unit, FriendData.unit_link);
        }

        Rarity = (uint)MasterData.rare + 1;

        RaceImage = MainMenuUtil.GetTextKindSprite(MasterData.kind, false);
        if (MasterData.sub_kind != MasterDataDefineLabel.KindType.NONE)
        {
            SubRaceImage = MainMenuUtil.GetTextKindSprite(MasterData.sub_kind, false);
        }
        else
        {
            SubRaceImage = null;
        }

        AttributeImage = MainMenuUtil.GetTextElementSprite(MasterData.element);
        AttributeImageColor = ColorUtil.GetElementLabelColor(MasterData.element);

        CharaName = MasterData.name;

        changeInfo(_type);

        //--------------------------------
        // ユーザーのログイン時間を設定
        //--------------------------------
        {
            DateTime cTimeLastPlay = TimeUtil.ConvertServerTimeToLocalTime(FriendData.last_play);
            DateTime cTimeNow = TimeManager.Instance.m_TimeNow;
            TimeSpan cTimeSpan = cTimeNow - cTimeLastPlay;
            string strTimeValue = "";
            if ((int)cTimeSpan.TotalDays > 0) { strTimeValue = string.Format(GameTextUtil.GetText("questfriend_text3"), (int)cTimeSpan.TotalDays); }
            else if ((int)cTimeSpan.TotalHours > 0) { strTimeValue = string.Format(GameTextUtil.GetText("questfriend_text2"), (int)cTimeSpan.TotalHours); }
            //else if ((int)cTimeSpan.TotalMinutes > 0) { strTimeValue = string.Format(GetText("LAST_PLAY_MINUTE"), (int)cTimeSpan.TotalMinutes); }
            else { strTimeValue = string.Format(GameTextUtil.GetText("questfriend_text2"), 0); }
            Time = strTimeValue;
        }

        setFlag(friendDataSetting.m_Flag);

        IsEnableButton = friendDataSetting.IsEnableButton;

        //---------------------------------------
        // フレンドポイントの設定
        //---------------------------------------
        IsViewFriendPoint = friendDataSetting.IsViewFriendPoint;
        if (friendDataSetting.IsViewFriendPoint)
        {
            switch (friendDataSetting.m_Flag)
            {
                case FlagType.FRIEND:
                    FriendPointText = "20";
                    break;
                case FlagType.HELPER:
                    FriendPointText = "5";
                    break;
            }

            //---------------------------------------
            // フレンドポイント増加の設定
            //---------------------------------------
            switch (MainMenuParam.m_QuestEventFP)
            {
                case GlobalDefine.FP_EVENT_ID_x0150: FriendPointText += "×1.5"; break; // フレンドポイント増加イベントID：1.5倍
                case GlobalDefine.FP_EVENT_ID_x0200: FriendPointText += "×2"; break;   // フレンドポイント増加イベントID：2.0倍
                case GlobalDefine.FP_EVENT_ID_x0250: FriendPointText += "×2.5"; break; // フレンドポイント増加イベントID：2.5倍
                case GlobalDefine.FP_EVENT_ID_x0300: FriendPointText += "×3"; break;   // フレンドポイント増加イベントID：3.0倍
                case GlobalDefine.FP_EVENT_ID_x0400: FriendPointText += "×4"; break;   // フレンドポイント増加イベントID：4.0倍
                case GlobalDefine.FP_EVENT_ID_x0500: FriendPointText += "×5"; break;   // フレンドポイント増加イベントID：5.0倍
                case GlobalDefine.FP_EVENT_ID_x1000: FriendPointText += "×10"; break;  // フレンドポイント増加イベントID：10.0倍
            }
        }

        //---------------------------------------
        // CHARMの設定
        //---------------------------------------
        CharmLabel = GameTextUtil.GetText("unit_status9");
        if (friendDataSetting.CharaOnce != null)
        {
            CharmValue = string.Format(GameTextUtil.GetText("kyouka_text1"), friendDataSetting.CharaOnce.m_CharaCharm.ToString("F1"));
            CharmValue = string.Format(GameTextUtil.GetText("value_colorset"), CharmValue);
        }
    }

    public void changeInfo(ParamType _type)
    {
        switch (_type)
        {
            case ParamType.NAME:
                UnityUtil.SetObjectEnabledOnce(paramListObj, true);
                UnityUtil.SetObjectEnabledOnce(skillListObj, false);
                break;
        }
    }

    public T setupPrefab<T>(GameObject _obj)
    {
        if (_obj != null)
        {
            GameObject _newObj = Instantiate(_obj);
            if (_newObj != null)
            {
                _newObj.transform.SetParent(paramListObj.transform, false);
                return _newObj.GetComponent<T>();
            }
        }
        return default(T);
    }
    public T setupPrefab<T>(string _prefabName)
    {
        GameObject _obj = Resources.Load<GameObject>(_prefabName);
        if (_obj != null)
        {
            GameObject _newObj = Instantiate(_obj);
            if (_newObj != null)
            {
                _newObj.transform.SetParent(paramListObj.transform, false);
                return _newObj.GetComponent<T>();
            }
        }
        return default(T);
    }

    public void OnSelectIcon()
    {
        if (ServerApi.IsExists == true)
        {
            return;
        }

        friendDataSetting.DidSelectIcon(this);
    }

    public void OnSelectFriend()
    {
        if (ServerApi.IsExists == true)
        {
            return;
        }

        friendDataSetting.DidSelectFriend(this);
    }


    private void SetUpButtons()
    {
        var iconButtonModel = new ButtonModel();
        m_IconButton = FriendListIconButton.Attach(m_iconButtonRoot);
        m_IconButton.SetModel<ButtonModel>(iconButtonModel);
        iconButtonModel.OnClicked += () =>
        {
            OnSelectIcon();
        };

        var selectButtonModel = new ButtonModel();
        m_SelectButton = FriendListSelectButton.Attach(m_selectButtonRoot);
        m_SelectButton.SetModel<ButtonModel>(selectButtonModel);
        selectButtonModel.OnClicked += () =>
        {
            OnSelectFriend();
        };
        // ボタンの選択イメージ設定
        m_SelectButton.SetButtonTarget(SelectButtonImage);

        // TODO : 演出を入れるならその場所に移動
        iconButtonModel.Appear();
        iconButtonModel.SkipAppearing();
        selectButtonModel.Appear();
        selectButtonModel.SkipAppearing();
    }
}
