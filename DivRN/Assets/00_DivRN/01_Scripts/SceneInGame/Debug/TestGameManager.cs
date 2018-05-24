using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class TestGameManager : M4uContextMonoBehaviour
{
	M4uProperty<string> area_name = new M4uProperty<string>();
	public string Area_name { get { return area_name.Value; } set { area_name.Value = value; } }

	M4uProperty<string> area_detail = new M4uProperty<string>();
	public string Area_detail { get { return area_detail.Value; } set { area_detail.Value = value; } }

	M4uProperty<string> quest_name = new M4uProperty<string>();
	public string Quest_name { get { return quest_name.Value; } set { quest_name.Value = value; } }

	M4uProperty<Sprite> quest2_switch = new M4uProperty<Sprite>();
	public Sprite Quest2_switch { get { return quest2_switch.Value; } set { quest2_switch.Value = value; } }

	M4uProperty<Sprite> restore_switch = new M4uProperty<Sprite>();
	public Sprite Restore_switch { get { return restore_switch.Value; } set { restore_switch.Value = value; } }

	M4uProperty<Sprite> api_switch = new M4uProperty<Sprite>();
	public Sprite Api_switch { get { return api_switch.Value; } set { api_switch.Value = value; } }

	M4uProperty<string> message_text = new M4uProperty<string>();
	public string Message_text { get { return message_text.Value; } set { message_text.Value = value; } }


	void Awake()
	{
		gameObject.GetComponent<M4uContextRoot>().Context = this;
		Message_text = "Wait…";
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
