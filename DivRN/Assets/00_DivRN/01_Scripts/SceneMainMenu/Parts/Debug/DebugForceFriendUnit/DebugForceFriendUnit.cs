using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class DebugForceFriendUnit : MenuPartsBase
{
    public enum InputType
    {
        BaseID,
        BaseLevel,
        BaseAtkPlus,
        BaseHpPlus,
        BaseSkillLevel,
        BaseLimitOver,
        LinkID,
        LinkLevel,
        LinkAtkPlus,
        LinkHpPlus,
        LinkPoint,
        LinkLimitOver,
        Max,
    };

    public InputField[] fieldList = null;

    M4uProperty<string> baseName = new M4uProperty<string>();
    public string BaseName { get { return baseName.Value; } set { baseName.Value = value; } }

    M4uProperty<Sprite> baseImage = new M4uProperty<Sprite>();
    public Sprite BaseImage { get { return baseImage.Value; } set { baseImage.Value = value; } }

    M4uProperty<string> linkName = new M4uProperty<string>();
    public string LinkName { get { return linkName.Value; } set { linkName.Value = value; } }

    M4uProperty<Sprite> linkImage = new M4uProperty<Sprite>();
    public Sprite LinkImage { get { return linkImage.Value; } set { linkImage.Value = value; } }

    M4uProperty<bool> isForce = new M4uProperty<bool>();
    public bool IsForce { get { return isForce.Value; } set { isForce.Value = value; } }

    public System.Action DidResetBase = delegate { };
    public System.Action DidResetLink = delegate { };
    public System.Action DidForce = delegate { };
    public System.Action<InputType, string> DidSetParam = delegate { };

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    private void Start()
    {
        for (int i = 0; i < (int)InputType.Max; i++)
        {
            InputType type = (InputType)i;
            fieldList[i].onEndEdit.AddListener((data) =>
            {
                SetParam(type, data);
            });
        }
    }

    public void SetParam(InputType _type, string _data)
    {
        DidSetParam(_type, _data);

        switch (_type)
        {
            case InputType.BaseID:
                {
                    BaseName = "";
                    BaseImage = null;
                    int unit_id = _data.ToInt(0);
                    if (unit_id != 0)
                    {
#if BUILD_TYPE_DEBUG
                        Debug.Log("ID:" + unit_id);
#endif
                        MasterDataParamChara master = SearchUnit(unit_id);
                        if (master != null)
                        {
                            BaseName = master.name;
                            UnitIconImageProvider.Instance.Get(
                                master.fix_id,
                                sprite =>
                                {
                                    BaseImage = sprite;
                                });
                        }
                    }
                }
                break;
            case InputType.LinkID:
                {
                    LinkName = "";
                    LinkImage = null;
                    int unit_id = _data.ToInt(0);
                    if (unit_id != 0)
                    {
#if BUILD_TYPE_DEBUG
                        Debug.Log("ID:" + unit_id);
#endif
                        MasterDataParamChara master = SearchUnit(unit_id);
                        if (master != null)
                        {
                            LinkName = master.name;
                            UnitIconImageProvider.Instance.Get(
                                master.fix_id,
                                sprite =>
                                {
                                    LinkImage = sprite;
                                });
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public MasterDataParamChara SearchUnit(int unit_id)
    {
        if (unit_id == 0) return null;
        MasterDataParamChara master = MasterFinder<MasterDataParamChara>.Instance.SelectWhere("where draw_id = ?", unit_id).First();
        return master;
    }

    public void OnResetBase()
    {
        DidResetBase();
    }

    public void setupBase(PacketStructUnit _unit)
    {
        if (_unit == null)
        {
            return;
        }

        MasterDataParamChara master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit.id);
        if (master == null)
        {
            return;
        }

        fieldList[0].text = master.draw_id.ToString();
        fieldList[1].text = _unit.level.ToString();
        fieldList[2].text = _unit.add_pow.ToString();
        fieldList[3].text = _unit.add_hp.ToString();
        fieldList[4].text = _unit.limitbreak_lv.ToString();
        fieldList[5].text = _unit.limitover_lv.ToString();

        BaseName = master.name;
        UnitIconImageProvider.Instance.Get(
            master.fix_id,
            sprite =>
            {
                BaseImage = sprite;
            });
    }

    public void setupLink(PacketStructUnit _unit)
    {
        if (_unit == null)
        {
            return;
        }

        MasterDataParamChara master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit.id);
        if (master == null)
        {
            return;
        }

        fieldList[6].text = master.draw_id.ToString();
        fieldList[7].text = _unit.level.ToString();
        fieldList[8].text = _unit.add_pow.ToString();
        fieldList[9].text = _unit.add_hp.ToString();
        fieldList[10].text = _unit.limitbreak_lv.ToString();
        fieldList[11].text = _unit.limitover_lv.ToString();

        LinkName = master.name;
        UnitIconImageProvider.Instance.Get(
            master.fix_id,
            sprite =>
            {
                LinkImage = sprite;
            });
    }

    public void resetBaseParam(bool bAll = true)
    {
        if (bAll)
        {
            fieldList[0].text = "";
            BaseName = "";
            BaseImage = null;
        }
        fieldList[1].text = "";
        fieldList[2].text = "";
        fieldList[3].text = "";
        fieldList[4].text = "";
        fieldList[5].text = "";
    }

    public void OnResetLink()
    {
        DidResetLink();
    }

    public void resetLinkParam(bool bAll = true)
    {
        if (bAll)
        {
            fieldList[6].text = "";
            LinkName = "";
            LinkImage = null;
        }
        fieldList[7].text = "";
        fieldList[8].text = "";
        fieldList[9].text = "";
        fieldList[10].text = "";
        fieldList[11].text = "";
    }

    public void setInputText(InputType _type, string _text)
    {
        fieldList[(int)_type].text = _text;
    }

    public void OnForce()
    {
        IsForce = !IsForce;
        DidForce();
    }
}
