using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class UnitNamePanel : MenuPartsBase
{
    public Toggle togglePremium = null;

    public Action DidSelectIcon = delegate { };
    public Action<bool> DidSelectPremium = delegate { };

    M4uProperty<bool> isViewIcon = new M4uProperty<bool>();
    public bool IsViewIcon { get { return isViewIcon.Value; } set { isViewIcon.Value = value; } }

    M4uProperty<Sprite> icon = new M4uProperty<Sprite>();
    public Sprite Icon { get { return icon.Value; } set { icon.Value = value; } }

    M4uProperty<Color> iconColor = new M4uProperty<Color>();
    public Color IconColor { get { return iconColor.Value; } set { iconColor.Value = value; } }

    M4uProperty<Sprite> iconSelect = new M4uProperty<Sprite>();
    public Sprite IconSelect { get { return iconSelect.Value; } set { iconSelect.Value = value; } }

    M4uProperty<string> charaName = new M4uProperty<string>();
    public string CharaName { get { return charaName.Value; } set { charaName.Value = value; } }

    M4uProperty<string> charaNo = new M4uProperty<string>();
    public string CharaNo { get { return charaNo.Value; } set { charaNo.Value = value; } }

    M4uProperty<uint> rarity = new M4uProperty<uint>();
    public uint Rarity { get { return rarity.Value; } set { rarity.Value = value; } }

    M4uProperty<string> raceLabel = new M4uProperty<string>();
    public string RaceLabel { get { return raceLabel.Value; } set { raceLabel.Value = value; } }
    //M4uProperty<string> race = new M4uProperty<string>();
    //public string Race { get { return race.Value; } set { race.Value = value; } }

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

    M4uProperty<Sprite> attributeImage = new M4uProperty<Sprite>();
    public Sprite AttributeImage
    {
        get { return attributeImage.Value; }
        set
        {
            attributeImage.Value = value;
            IsActiveAttribute = (value != null);
        }
    }

    M4uProperty<Color> attributeImageColor = new M4uProperty<Color>(Color.white);
    public Color AttributeImageColor { get { return attributeImageColor.Value; } set { attributeImageColor.Value = value; } }

    M4uProperty<bool> isActiveAttribute = new M4uProperty<bool>();
    public bool IsActiveAttribute { get { return isActiveAttribute.Value; } set { isActiveAttribute.Value = value; } }

    M4uProperty<bool> isViewPremiumButton = new M4uProperty<bool>();
    public bool IsViewPremiumButton { get { return isViewPremiumButton.Value; } set { isViewPremiumButton.Value = value; } }

    M4uProperty<bool> isViewName = new M4uProperty<bool>(true);
    public bool IsViewName { get { return isViewName.Value; } set { isViewName.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        RaceLabel = GameTextUtil.GetText("unit_status2");
        AttributeLabel = GameTextUtil.GetText("unit_status3");
        IsViewIcon = true;
        IsViewPremiumButton = false;
        reset();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setup(MasterDataParamChara _master)
    {
        IconSelect = MainMenuUtil.GetElementCircleSprite(_master.element);
		UnitIconImageProvider.Instance.Get(
            _master.fix_id,
            sprite =>
            {
                Icon = sprite;
            });

        CharaName = _master.name;
        string noFormat = GameTextUtil.GetText("unit_status1");
        CharaNo = string.Format(noFormat, _master.draw_id);
        Rarity = (uint)_master.rare + 1;

        RaceImage = MainMenuUtil.GetTextKindSprite(_master.kind, false);
        if (_master.sub_kind != MasterDataDefineLabel.KindType.NONE)
        {
            SubRaceImage = MainMenuUtil.GetTextKindSprite(_master.sub_kind, false);
        }
        else
        {
            SubRaceImage = null;
        }

        AttributeImage = MainMenuUtil.GetTextElementSprite(_master.element);
        AttributeImageColor = ColorUtil.GetElementLabelColor(_master.element);
    }

    public void reset()
    {
        CharaName = "";
        Rarity = 0;
        RaceImage = null;
        SubRaceImage = null;
        AttributeImage = null;
        Icon = null;
        IconColor = new Color(1.0f, 1.0f, 1.0f);
    }

    public void OnSelectIcon()
    {
        DidSelectIcon();
    }

    public void OnSelectPremium(bool bFlag)
    {
        DidSelectPremium(togglePremium.isOn);
    }
}
