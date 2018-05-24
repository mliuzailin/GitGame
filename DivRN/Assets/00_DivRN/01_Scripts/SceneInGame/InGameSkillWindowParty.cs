using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class InGameSkillWindowParty : M4uContextMonoBehaviour
{

	M4uProperty<float> heroScale = new M4uProperty<float>();
	public float HeroScale { get { return heroScale.Value; } set { heroScale.Value = value; } }

	M4uProperty<Sprite> heroImage = new M4uProperty<Sprite>();
	public Sprite HeroImage { get { return heroImage.Value; } set { heroImage.Value = value; } }

	M4uProperty<Color> heroColor = new M4uProperty<Color>();
	public Color HeroColor { get { return heroColor.Value; } set { heroColor.Value = value; } }

	M4uProperty<Texture> heroImageMask = new M4uProperty<Texture>();
	public Texture HeroImageMask { get { return heroImageMask.Value; } set { heroImageMask.Value = value; } }

	public System.Action<GlobalDefine.PartyCharaIndex> m_ClickAction = delegate { };
	public GameObject m_UnitIconRoot = null;
	private InGameSkillWindowUnitIcon[] m_UnitIcon = null;

	void Awake()
	{
		gameObject.GetComponent<M4uContextRoot>().Context = this;
		if (m_UnitIcon == null)
		{
			m_UnitIcon = new InGameSkillWindowUnitIcon[5];
			for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; ++i)
			{
				GameObject _tmpObj = Resources.Load("Prefab/InGame/InGameUI/Menu/InGameSkillWindowUnitIcon") as GameObject;
				if (_tmpObj != null)
				{
					GameObject _newObj = Instantiate(_tmpObj);
					if (_newObj != null)
					{
						_newObj.transform.SetParent(m_UnitIconRoot.transform, false);
						_newObj.GetComponent<RectTransform>().position = new Vector3(147 - (i * 102), 0, 0);
						m_UnitIcon[i] = _newObj.GetComponent<InGameSkillWindowUnitIcon>();
					}
				}
			}
		}
		HeroScale = 1;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setup(GlobalDefine.PartyCharaIndex index, System.Action<GlobalDefine.PartyCharaIndex> action)
	{
		for (int i = 0; i < (int)GlobalDefine.PartyCharaIndex.MAX; ++i)
		{
			m_UnitIcon[i].setup(InGamePlayerParty.Instance.m_PartyUnit[i].Unit_base, InGamePlayerParty.Instance.m_PartyUnit[i].Attribute_circle, (GlobalDefine.PartyCharaIndex)i, i == (int)index);
			m_UnitIcon[i].m_ClickAction = action;
		}
		HeroImage = InGamePlayerParty.Instance.m_InGamePartyManager.Hero_face;
		HeroImageMask = InGamePlayerParty.Instance.m_InGamePartyManager.Hero_face_mask;
		changeHero(index == GlobalDefine.PartyCharaIndex.HERO);
	}

	public void changeUnit(GlobalDefine.PartyCharaIndex prevIndex, GlobalDefine.PartyCharaIndex nextIndex)
	{
		if (prevIndex != GlobalDefine.PartyCharaIndex.HERO)
		{
			m_UnitIcon[(int)prevIndex].setSelect(false);
		}
		if (nextIndex != GlobalDefine.PartyCharaIndex.HERO)
		{
			m_UnitIcon[(int)nextIndex].setSelect(true);
		}
	}

	public void changeHero(bool select)
	{
		Color gray = new Color(0.7f, 0.7f, 0.7f);
		if (select == true)
		{
			HeroColor = Color.white;
		}
		else
		{
			HeroColor = gray;
		}
	}

	public void OnClick()
	{
		if (m_ClickAction != null)
		{
			m_ClickAction(GlobalDefine.PartyCharaIndex.HERO);
		}
	}
}
