using UnityEngine;
using System;
using System.Collections;
using M4u;

using ServerDataDefine;

public class ServerApiMenuItem : M4uContext {

	M4uProperty<string> title = new M4uProperty<string>();
	public string Title { get { return title.Value; } set { title.Value = value; } }

	public SERVER_API api = SERVER_API.SERVER_API_MAX;

	public Action<SERVER_API> DelSelectSubMenu = delegate { };

	public ServerApiMenuItem( string _title, SERVER_API _api, Action<SERVER_API> _action )
	{
		Title = _title;
		api = _api;
		DelSelectSubMenu = _action;
	}

}
