using UnityEngine;
using System.Collections;

public class SceneMenuSeqTest : Scene<SceneMenuSeqTest>
{
	enum PageStatus
	{
		PageSwitchEnableBefore,
		PageSwitchEnable,
		PageSwitchEnableAfter,
		PageSwitchWait,
		Max
	}
	public GameObject seqObj = null;
	public MasterMainMenuSeq testSeq = null;

	private MainMenuSeq m_MenuSeq = null;
	private PageStatus m_PageStatus = PageStatus.PageSwitchEnableBefore;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	public void Update()
	{
		if (m_MenuSeq == null)
		{
			return;
		}
		
		if (!m_MenuSeq.m_MainMenuSeqStartOK)
		{
			return;
		}

		switch(m_PageStatus)
		{
			case PageStatus.PageSwitchEnableBefore:
				{
					if(!m_MenuSeq.PageSwitchEventEnableBefore())
					{
						m_PageStatus = PageStatus.PageSwitchEnable;
					}
				}
				break;
			case PageStatus.PageSwitchEnable:
				{
					m_MenuSeq.PageSwitchTriger();
					m_PageStatus = PageStatus.PageSwitchEnableAfter;

				}
				break;
			case PageStatus.PageSwitchEnableAfter:
				{
					if( !m_MenuSeq.PageSwitchEventEnableAfter() )
					{
						m_PageStatus = PageStatus.PageSwitchWait;
					}

				}
				break;
			case PageStatus.PageSwitchWait:
				break;

		}
	}

	public override void OnInitialized()
	{
		base.OnInitialized();
		GameObject objCanvas = UnityUtil.GetChildNode(seqObj, "Canvas");
		if (objCanvas != null)
		{
			foreach(MasterMainMenuSeq.SequenceObj _obj in testSeq.SequenceObjArray)
			{
				string object_path = "Prefab/" + _obj.object_name;
				GameObject originObj = Resources.Load( object_path ) as GameObject;
				if( originObj != null)
				{
					GameObject _insObj = Instantiate(originObj) as GameObject;
                    _insObj.transform.SetParent(objCanvas.transform, false);

				}
			}
		}
		UnityUtil.SetObjectLayer( seqObj , LayerMask.NameToLayer( "DRAW_CLIP" ) );

		UnityUtil.SetObjectEnabledOnce( seqObj , true );

		System.Type sequenceType = System.Type.GetType(testSeq.SequenceName);
		m_MenuSeq = seqObj.AddComponent( sequenceType ) as MainMenuSeq;
		seqObj.name = testSeq.SequenceName;

	}
}
