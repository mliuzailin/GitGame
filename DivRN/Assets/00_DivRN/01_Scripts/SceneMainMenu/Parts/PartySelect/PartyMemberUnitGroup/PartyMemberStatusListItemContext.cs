/**
 *  @file   PartyMemberStatusListItemContext.cs
 *  @brief
 *  @author Developer
 *  @date   2017/04/05
 */

using UnityEngine;
using System.Collections;
using M4u;

public class PartyMemberStatusListItemContext : M4uContext
{
    M4uProperty<GlobalDefine.PartyCharaIndex> partyCharaIndex = new M4uProperty<GlobalDefine.PartyCharaIndex>();
    public int Cost;
    public double Charm;
    /// <summary>パーティキャラタイプ</summary>
    public GlobalDefine.PartyCharaIndex PartyCharaIndex
    {
        get
        {
            return partyCharaIndex.Value;
        }
        set
        {
            partyCharaIndex.Value = value;
        }
    }

    M4uProperty<string> hpText = new M4uProperty<string>("");
    public string HpText
    {
        get
        {
            return hpText.Value;
        }
        set
        {
            hpText.Value = value;
        }
    }

    M4uProperty<string> atkText = new M4uProperty<string>("");
    public string AtkText
    {
        get
        {
            return atkText.Value;
        }
        set
        {
            atkText.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost1 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost1
    {
        get
        {
            return isActiveSkill1Cost1.Value;
        }
        set
        {
            isActiveSkill1Cost1.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost2 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost2
    {
        get
        {
            return isActiveSkill1Cost2.Value;
        }
        set
        {
            isActiveSkill1Cost2.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost3 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost3
    {
        get
        {
            return isActiveSkill1Cost3.Value;
        }
        set
        {
            isActiveSkill1Cost3.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost4 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost4
    {
        get
        {
            return isActiveSkill1Cost4.Value;
        }
        set
        {
            isActiveSkill1Cost4.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill1Cost5 = new M4uProperty<bool>();
    private bool IsActiveSkill1Cost5
    {
        get
        {
            return isActiveSkill1Cost5.Value;
        }
        set
        {
            isActiveSkill1Cost5.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost1 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost1
    {
        get
        {
            return isActiveSkill2Cost1.Value;
        }
        set
        {
            isActiveSkill2Cost1.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost2 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost2
    {
        get
        {
            return isActiveSkill2Cost2.Value;
        }
        set
        {
            isActiveSkill2Cost2.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost3 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost3
    {
        get
        {
            return isActiveSkill2Cost3.Value;
        }
        set
        {
            isActiveSkill2Cost3.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost4 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost4
    {
        get
        {
            return isActiveSkill2Cost4.Value;
        }
        set
        {
            isActiveSkill2Cost4.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Cost5 = new M4uProperty<bool>();
    private bool IsActiveSkill2Cost5
    {
        get
        {
            return isActiveSkill2Cost5.Value;
        }
        set
        {
            isActiveSkill2Cost5.Value = value;
        }
    }

    M4uProperty<Sprite> skill1Cost1 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost1
    {
        get
        {
            return skill1Cost1.Value;
        }
        set
        {
            skill1Cost1.Value = value;
            IsActiveSkill1Cost1 = (value != null);
        }
    }

    M4uProperty<Sprite> skill1Cost2 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost2
    {
        get
        {
            return skill1Cost2.Value;
        }
        set
        {
            skill1Cost2.Value = value;
            IsActiveSkill1Cost2 = (value != null);
        }
    }

    M4uProperty<Sprite> skill1Cost3 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost3
    {
        get
        {
            return skill1Cost3.Value;
        }
        set
        {
            skill1Cost3.Value = value;
            IsActiveSkill1Cost3 = (value != null);
        }
    }

    M4uProperty<Sprite> skill1Cost4 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost4
    {
        get
        {
            return skill1Cost4.Value;
        }
        set
        {
            skill1Cost4.Value = value;
            IsActiveSkill1Cost4 = (value != null);
        }
    }

    M4uProperty<Sprite> skill1Cost5 = new M4uProperty<Sprite>();
    public Sprite Skill1Cost5
    {
        get
        {
            return skill1Cost5.Value;
        }
        set
        {
            skill1Cost5.Value = value;
            IsActiveSkill1Cost5 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost1 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost1
    {
        get
        {
            return skill2Cost1.Value;
        }
        set
        {
            skill2Cost1.Value = value;
            IsActiveSkill2Cost1 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost2 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost2
    {
        get
        {
            return skill2Cost2.Value;
        }
        set
        {
            skill2Cost2.Value = value;
            IsActiveSkill2Cost2 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost3 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost3
    {
        get
        {
            return skill2Cost3.Value;
        }
        set
        {
            skill2Cost3.Value = value;
            IsActiveSkill2Cost3 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost4 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost4
    {
        get
        {
            return skill2Cost4.Value;
        }
        set
        {
            skill2Cost4.Value = value;
            IsActiveSkill2Cost4 = (value != null);
        }
    }

    M4uProperty<Sprite> skill2Cost5 = new M4uProperty<Sprite>();
    public Sprite Skill2Cost5
    {
        get
        {
            return skill2Cost5.Value;
        }
        set
        {
            skill2Cost5.Value = value;
            IsActiveSkill2Cost5 = (value != null);
        }
    }


    M4uProperty<bool> isActiveSkill1Empty = new M4uProperty<bool>(true);
    public bool IsActiveSkill1Empty
    {
        get
        {
            return isActiveSkill1Empty.Value;
        }
        set
        {
            isActiveSkill1Empty.Value = value;
        }
    }

    M4uProperty<bool> isActiveSkill2Empty = new M4uProperty<bool>(true);
    public bool IsActiveSkill2Empty
    {
        get
        {
            return isActiveSkill2Empty.Value;
        }
        set
        {
            isActiveSkill2Empty.Value = value;
        }
    }

    M4uProperty<Sprite> skill1Color = new M4uProperty<Sprite>();
    public Sprite Skill1Color
    {
        get
        {
            return skill1Color.Value;
        }
        set
        {
            skill1Color.Value = value;
        }
    }

    M4uProperty<Sprite> skill2Color = new M4uProperty<Sprite>();
    public Sprite Skill2Color
    {
        get
        {
            return skill2Color.Value;
        }
        set
        {
            skill2Color.Value = value;
        }
    }
}
