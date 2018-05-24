using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitStoryPanel : MenuPartsBase
{
    private M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    private M4uProperty<string> message = new M4uProperty<string>();
    public string Message { get { return message.Value; } set { message.Value = value; } }

    private void Awake()
    {
        Title = GameTextUtil.GetText("unit_status14");
        Message = "サンプルテキスト";
        GetComponent<M4uContextRoot>().Context = this;
    }

    public void setup(uint _unit_id)
    {
        MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)_unit_id);
        if (_master == null)
        {
            return;
        }
        Message = _master.detail;
    }
}
