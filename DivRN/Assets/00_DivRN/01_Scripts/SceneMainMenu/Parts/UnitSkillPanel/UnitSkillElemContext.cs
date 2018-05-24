using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitSkillElemContext : M4uContext
{
    M4uProperty<Sprite> elementImage = new M4uProperty<Sprite>();
    public Sprite ElementImage { get { return elementImage.Value; } set { elementImage.Value = value; } }

    public UnitSkillElemContext(MasterDataDefineLabel.ElementType _type)
    {
        string _sprite_name = "";
        switch (_type)
        {
            case MasterDataDefineLabel.ElementType.NAUGHT:
                _sprite_name = "s_mu";
                break;
            case MasterDataDefineLabel.ElementType.FIRE:
                _sprite_name = "s_hi";
                break;
            case MasterDataDefineLabel.ElementType.WATER:
                _sprite_name = "s_mizu";
                break;
            case MasterDataDefineLabel.ElementType.LIGHT:
                _sprite_name = "s_hikari";
                break;
            case MasterDataDefineLabel.ElementType.DARK:
                _sprite_name = "s_yami";
                break;
            case MasterDataDefineLabel.ElementType.WIND:
                _sprite_name = "s_kaze";
                break;
            case MasterDataDefineLabel.ElementType.HEAL:
                _sprite_name = "s_kaifuku";
                break;
        }
        ElementImage = ResourceManager.Instance.Load(_sprite_name, ResourceType.Common);
    }

}
