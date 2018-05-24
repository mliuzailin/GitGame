using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class InGameSkillWindowTag : M4uContextMonoBehaviour
{

	M4uProperty<bool> link_tag_active = new M4uProperty<bool>();
	public bool Link_tag_active { get { return link_tag_active.Value; } set { link_tag_active.Value = value; } }

	M4uProperty<Sprite> status_tag = new M4uProperty<Sprite>();
	public Sprite Status_tag { get { return status_tag.Value; } set { status_tag.Value = value; } }

	M4uProperty<Sprite> link_tag = new M4uProperty<Sprite>();
	public Sprite Link_tag { get { return link_tag.Value; } set { link_tag.Value = value; } }

	M4uProperty<Sprite> skill_tag = new M4uProperty<Sprite>();
	public Sprite Skill_tag { get { return skill_tag.Value; } set { skill_tag.Value = value; } }

	M4uProperty<bool> tab_active = new M4uProperty<bool>();
	public bool Tab_active { get { return tab_active.Value; } set { tab_active.Value = value; } }

	M4uProperty<Color> statusColor = new M4uProperty<Color>();
	public Color StatusColor { get { return statusColor.Value; } set { statusColor.Value = value; } }

	M4uProperty<Color> linkColor = new M4uProperty<Color>();
	public Color LinkColor { get { return linkColor.Value; } set { linkColor.Value = value; } }

	M4uProperty<Color> skillColor = new M4uProperty<Color>();
	public Color SkillColor { get { return skillColor.Value; } set { skillColor.Value = value; } }

	public System.Action m_StatusClickAction = delegate { };
	public System.Action m_LinkClickAction = delegate { };
	public System.Action m_SkillClickAction = delegate { };


	void Awake()
	{
		gameObject.GetComponent<M4uContextRoot>().Context = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnStatus()
	{
		if (m_StatusClickAction != null)
		{
			m_StatusClickAction();
		}
	}

	public void OnLink()
	{
		if (m_LinkClickAction != null)
		{
			m_LinkClickAction();
		}
	}

	public void OnSkill()
	{
		if (m_SkillClickAction != null)
		{
			m_SkillClickAction();
		}
	}
}
