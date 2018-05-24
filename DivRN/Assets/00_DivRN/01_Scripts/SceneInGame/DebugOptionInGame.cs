//#if BUILD_TYPE_DEBUG
//	#if UNITY_EDITOR
//		#define Quest2fromJson
//	#endif


using UnityEngine;
using System.Collections;

using ServerDataDefine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR && BUILD_TYPE_DEBUG 

public class DebugOptionInGame : SingletonComponent<DebugOptionInGame>
{
	[HideInInspector] public InGameDebugDO inGameDebugDO;
	[HideInInspector] public int m_QuestIndex;										// デバッグ用クエストテーブルインデックス
	[HideInInspector] public bool m_InGmaeStart = false;			// 
	[HideInInspector] public bool m_InGmaeFirst = true;				// 単体起動初回フラグ
	[HideInInspector] public bool m_InGmaeAPI = false;				// ユーザーオートログイン終了フラグ

	[System.Serializable]
	public class InGameDebugDO
	{
		public bool m_InGameDebug = false;							// 探索単体起動フラグ
		public bool m_BattleSkip = true;							// 探索バトルスキップフラグ
		public bool m_AutoPlay = false;								// 探索オートプレイフラグ
		public bool m_AutoPlayAllMode = true;						// 探索オートプレイ全クエストモードフラグ
		public bool m_AllPanel = true;                              // 探索オートプレイ全パネルオープンフラグ
		public bool m_DebugParty = false;                           // 探索デフォルトパーティー使用フラグ
		public bool m_Restore = true;
		public bool m_UseAPI = true;                                // 探索単体起動時QuestStartAPI呼び出しフラグ
		//public MasterDataQuest m_MasterDataQuest = null;// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
		public MasterDataQuest2 m_MasterDataQuest2 = null;
		public MasterDataArea m_MasterDataArea = null;
		public MasterDataDefaultParty m_MasterDataDefaultParty = null;
		public uint m_QuestId;										// デバッグ用クエストID
		public int m_DefaultPartyId;								// デバッグ用デフォルトパーティーID
	}

	[HideInInspector] public bool m_Quest2fromJson = false;
	[HideInInspector] public bool m_TestMode = false;
	[HideInInspector] public PacketStructQuestBuild m_QuestBuild = null;
	[HideInInspector] public PacketStructQuest2Build m_Quest2Build = null;
	private MasterDataQuest2[] m_MasterDataQuest2 = null;

	private float m_debugTime;
	private bool m_debugTimeSetup = false;
	public MasterDataQuest2[] masterDataQuest2
	{
		get { return m_MasterDataQuest2; }
	}

	protected override void Awake()
	{
		base.Awake();
		if (inGameDebugDO.m_UseAPI == true)
		{
			m_InGmaeFirst = true;
		}
		else
		{
			m_InGmaeFirst = false;
		}
		m_InGmaeAPI = false;
		if (inGameDebugDO.m_InGameDebug == true)
		{
			if (inGameDebugDO.m_AutoPlay == true)
			{
				m_QuestIndex = 0;
				inGameDebugDO.m_QuestId = 0;
			}
		}

#if Quest2fromJson
		string json_text = System.IO.File.ReadAllText("Assets/00_DivRN/03_Master/JsonData/00_Json/MasterDataQuest2.txt");
		if (json_text != null)
		{
			m_MasterDataQuest2 = LitJson.JsonMapper.ToObject<MasterDataQuest2[]>(json_text);    // マスターデータ実体：クエスト関連：クエスト情報
		}
		m_Quest2fromJson = true;
#endif
	}

	protected override void Start()
	{
		base.Start();
	}
#if false// TODO: MasterDataQuestをコメントアウトする。問題がなければ削除する。
    public bool NextQuestIndex()
	{
		if (MasterFinder<MasterDataQuest>.Instance != null)
		{
			MasterDataQuest[] master = MasterFinder<MasterDataQuest>.Instance.GetAll();
			while (true)
			{
				if (m_QuestIndex >= (uint)(master.Length - 1)) return false;
				++m_QuestIndex;
				inGameDebugDO.m_MasterDataQuest = master[m_QuestIndex];
				inGameDebugDO.m_QuestId = inGameDebugDO.m_MasterDataQuest.fix_id;
				if (inGameDebugDO.m_MasterDataQuest.active == MasterDataDefineLabel.BoolType.ENABLE) break;
//				Debug.LogErrorFormat("[AutoPlay] Quest Error [QUEST RESERVE] : QuestId = {0}", DebugOptionInGame.Instance.inGameDebugDO.m_QuestId.ToString());
			}
			return true;
		}
		return false;
	}
#endif
    public void InitQuest()
	{
		m_InGmaeFirst = true;
		m_QuestIndex = 0;
		inGameDebugDO.m_QuestId = 0;
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	エネミーグループパラメータ取得：ID指定
	*/
	//----------------------------------------------------------------------------
	public MasterDataQuest2 GetQuest2ParamFromID(uint unID)
	{
		if (m_MasterDataQuest2.IsNullOrEmpty() == true)
		{
			return null;
		}

		//----------------------------------------
		// 0番は不整合調整用データなので無視
		//----------------------------------------
		if (unID == 0)
		{
			return null;
		}

		//----------------------------------------
		// 指定IDのデータを返す
		//----------------------------------------
		for (int i = 0; i < m_MasterDataQuest2.Length; ++i)
		{
			if (m_MasterDataQuest2[i].fix_id == unID) return m_MasterDataQuest2[i];
		}
		return null;
	}

	public void setupDebugTime()
	{
		m_debugTime = Time.realtimeSinceStartup;
		m_debugTimeSetup = true;
	}

	public void calcDebugTime(string msg)
	{
		if (m_debugTimeSetup == true)
		{
			Debug.LogError(msg + " : " + (Time.realtimeSinceStartup - m_debugTime) + "Sec");
			m_debugTime = Time.realtimeSinceStartup;
		}
	}
}
#endif