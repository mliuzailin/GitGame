using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
	@class		InGameSkillWindowUnitIcon
	@brief		スキルウィンドウ
*/
//----------------------------------------------------------------------------
public class InGameSkillWindowUnitIcon : M4uContextMonoBehaviour
{

	M4uProperty<float> iconScale = new M4uProperty<float>();
	public float IconScale { get { return iconScale.Value; } set { iconScale.Value = value; } }

	M4uProperty<Color> iconColor = new M4uProperty<Color>();
	public Color IconColor { get { return iconColor.Value; } set { iconColor.Value = value; } }

	M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
	public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

	M4uProperty<Sprite> iconAttribute = new M4uProperty<Sprite>();
	public Sprite IconAttribute { get { return iconAttribute.Value; } set { iconAttribute.Value = value; } }

	private GlobalDefine.PartyCharaIndex m_CharaIndex = GlobalDefine.PartyCharaIndex.ERROR;
	public System.Action<GlobalDefine.PartyCharaIndex> m_ClickAction = delegate { };

	void Awake()
	{
		gameObject.GetComponent<M4uContextRoot>().Context = this;
		IconScale = 1;
		IconColor = Color.white;
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setup(Sprite icon, Sprite circle, GlobalDefine.PartyCharaIndex index,bool select)
	{
		IconImage = icon;
		IconAttribute = circle;
		m_CharaIndex = index;
		setSelect(select);
	}

	public void setSelect(bool select)
	{
		Color gray = new Color(0.7f, 0.7f, 0.7f);
		if (select == true)
		{
			IconScale = 1.2f;
			IconColor = Color.white;
		}
		else
		{
			IconScale = 1;
			IconColor = gray;
		}
	}

	public void OnClick()
	{
		if (m_ClickAction != null)
		{
			m_ClickAction(m_CharaIndex);
		}
	}
}
