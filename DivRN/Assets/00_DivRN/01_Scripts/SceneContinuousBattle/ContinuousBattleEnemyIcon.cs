/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	InGamePartyUnit.cs
	@brief	非アクティブオブジェクト一元化クラス
	@author Developer
	@date 	2012/10/04
*/
/*==========================================================================*/
/*==========================================================================*/

/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ContinuousBattleEnemyIcon : M4uContextMonoBehaviour
{
	/*==========================================================================*/
	/*		var																	*/
	/*==========================================================================*/

	public enum State : int
	{
		NONE = 0,
		MOVE,
		BLINK,
		DEAD,
		ALPHA,
	};

	public enum ICON_TYPE : int
	{
		TYPE_ENEMY = 0,
		TYPE_ENEMY_FOCUS,
		TYPE_GUERRILLA,
		TYPE_GUERRILLA_FOCUS,
		TYPE_BOSS,
		TYPE_BOSS_FOCUS,
		TYPE_CLEAR,
		TYPE_MAX,
	};

	public enum ICON_FOCUS_TYPE : int
	{
		OFF = 0,
		ON,
	};

	public Sprite[]							m_HexSprite;														//!< 倒した敵アイコン
	public Sprite[]							m_BattleNum;                                                       //!< 存在してる敵アイコン
	public Sprite[]							m_IconSprite;

	M4uProperty<bool> stage_active = new M4uProperty<bool>();
	public bool Stage_active { get { return stage_active.Value; } set { stage_active.Value = value; } }

	M4uProperty<bool> guerrilla_active = new M4uProperty<bool>();
	public bool Guerrilla_active { get { return guerrilla_active.Value; } set { guerrilla_active.Value = value; } }

	M4uProperty<bool> boss_active = new M4uProperty<bool>();
	public bool Boss_active { get { return boss_active.Value; } set { boss_active.Value = value; } }

	M4uProperty<bool> clear_active = new M4uProperty<bool>();
	public bool Clear_active { get { return clear_active.Value; } set { clear_active.Value = value; } }

	M4uProperty<Sprite> stage_hex = new M4uProperty<Sprite>();
	public Sprite Stage_hex { get { return stage_hex.Value; } set { stage_hex.Value = value; } }

	M4uProperty<Sprite> stage_text = new M4uProperty<Sprite>();
	public Sprite Stage_text { get { return stage_text.Value; } set { stage_text.Value = value; } }

	M4uProperty<Sprite> stage_num0 = new M4uProperty<Sprite>();
	public Sprite Stage_num0 { get { return stage_num0.Value; } set { stage_num0.Value = value; } }

	M4uProperty<Sprite> stage_num1 = new M4uProperty<Sprite>();
	public Sprite Stage_num1 { get { return stage_num1.Value; } set { stage_num1.Value = value; } }

	M4uProperty<Sprite> warning_text = new M4uProperty<Sprite>();
	public Sprite Warning_text { get { return warning_text.Value; } set { warning_text.Value = value; } }

	M4uProperty<Sprite> boss_text = new M4uProperty<Sprite>();
	public Sprite Boss_text { get { return boss_text.Value; } set { boss_text.Value = value; } }

	M4uProperty<float> icon_alpha = new M4uProperty<float>();
	public float Icon_alpha { get { return icon_alpha.Value; } set { icon_alpha.Value = value; } }

	[HideInInspector] public int			m_BattleUnique;														//!< 敵情報リストのユニークID
	[HideInInspector] public bool			m_Boss;																//!< ボスフラグ

	private State							m_State;															//!< 制御ステータス																		
	private State							m_PreState;															//!< 制御ステータス																		
	private int								m_Index;															//!< 表示位置数
	private Animation						m_BlinkAnim;														//!< 点滅用アニメーション
	private Vector3							m_Position;
	private float							m_MoveTime;
	
	private float							m_IconMoveDistance;
	private float							m_IconMoveSpeed;
	private float							m_IcomMoveTime;
	private float							m_IcomAlphaTime;
	private float							m_IcomAlphaDelayTime;
	private ICON_TYPE						m_IconType;

	private float							m_AlphaFadeItme			= 0.5f;

	public State							icon_state { get { return m_State; } }

	void Awake()
	{
		gameObject.GetComponent<M4uContextRoot>().Context = this;
		m_State = State.NONE;
		m_BlinkAnim = gameObject.GetComponent<Animation>();
		m_IconMoveDistance = 74.0f;
		m_IcomMoveTime = 0.5f;
		m_IconMoveSpeed = m_IconMoveDistance / m_IcomMoveTime;
		Icon_alpha = 0;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		switch(m_State)
		{
			case State.MOVE:
				{
					m_MoveTime += Time.deltaTime;
					if(m_MoveTime > m_IcomMoveTime)
					{
						m_MoveTime = m_IcomMoveTime;
						m_State = m_PreState;
					}
					Vector3 pos = m_Position;
					pos.x -= (m_MoveTime * m_IconMoveSpeed);
					gameObject.transform.localPosition = pos;
					if (m_State != State.MOVE)
					{
						m_Position = pos;
					}
				}
				break;
			case State.BLINK:
				{

				}
				break;
			case State.DEAD:
				{

				}
				break;
			case State.ALPHA:
				{
					if(m_IcomAlphaDelayTime > 0)
					{
						m_IcomAlphaDelayTime -= Time.deltaTime;
					}
					if(m_IcomAlphaDelayTime <= 0)
					{
						m_IcomAlphaTime += Time.deltaTime;
						if (m_IcomAlphaTime >= m_AlphaFadeItme)
						{
							m_IcomAlphaTime = m_AlphaFadeItme;
							m_State = State.NONE;
						}
						Icon_alpha = m_IcomAlphaTime / m_AlphaFadeItme;
					}
				}
				break;
			default:
				break;
		}
	}

	public void setup(ICON_TYPE type,int index,int battle_unique,bool boss)
	{
		m_Index = index;
		changeIcon(type);
		if(m_Index >= 0)
		{
			m_Position = gameObject.transform.localPosition;
			m_Position.x = 84.0f + m_Index * m_IconMoveDistance;
			if((m_Index % 2) == 0 )
				m_Position.y = 12;
			else
				m_Position.y = -32;
			m_Position.z = 0;
			gameObject.transform.localPosition = m_Position;
			Vector3 scale = gameObject.transform.localScale;
			scale.x = 1;
			scale.y = 1;
			scale.z = 1;
			gameObject.transform.localScale = scale;
		}
		m_BattleUnique = battle_unique;
		m_Boss = boss;
		m_IconType = type;
	}

	public void changeIcon(ICON_TYPE type)
	{
		m_IconType = type;
		Stage_active = false;
		Guerrilla_active = false;
		Boss_active = false;
		Clear_active = false;
		int num0 = (m_Index + 1) % 10;
		int num1 = (m_Index + 1) / 10;
		switch (type)
		{
			case ICON_TYPE.TYPE_CLEAR:
				{
					Clear_active = true;
				}
				break;
			case ICON_TYPE.TYPE_ENEMY:
				{
					Stage_active = true;
					Stage_hex = m_HexSprite[(int)ICON_FOCUS_TYPE.OFF];
					Stage_text = m_IconSprite[0];
					Stage_num0 = m_BattleNum[num0];
					Stage_num1 = m_BattleNum[num1];
				}
				break;
			case ICON_TYPE.TYPE_ENEMY_FOCUS:
				{
					Stage_active = true;
					Stage_hex = m_HexSprite[(int)ICON_FOCUS_TYPE.ON];
					Stage_text = m_IconSprite[3];
					Stage_num0 = m_BattleNum[num0 + 10];
					Stage_num1 = m_BattleNum[num1 + 10];
				}
				break;
			case ICON_TYPE.TYPE_GUERRILLA:
				{
					Guerrilla_active = true;
					Warning_text = m_IconSprite[1];
				}
				break;
			case ICON_TYPE.TYPE_GUERRILLA_FOCUS:
				{
					Guerrilla_active = true;
					Warning_text = m_IconSprite[4];
				}
				break;
			case ICON_TYPE.TYPE_BOSS:
				{
					Boss_active = true;
					Boss_text = m_IconSprite[2];
				}
				break;
			case ICON_TYPE.TYPE_BOSS_FOCUS:
				{
					Boss_active = true;
					Boss_text = m_IconSprite[5];
				}
				break;
			default:
				break;
		}
	}

	public void setBlink(bool flag)
	{
		if( flag == true )
		{
			if( m_State != State.BLINK)
			{
				m_BlinkAnim.Play("EnemyIconBlink");
				m_State = State.BLINK;
			}
		}
		else
		{
			if (m_State == State.BLINK)
			{
				m_BlinkAnim.Stop("EnemyIconBlink");
				m_BlinkAnim.Play("EnemyIconBlinkStop");
				m_State = State.NONE;
			}
		}
	}

	public void setMove()
	{
		m_MoveTime = 0;
		m_PreState = m_State;
		m_State = State.MOVE;
	}

	public void setAlpha(float delay)
	{
		m_State = State.ALPHA;
		Icon_alpha = 0;
		m_IcomAlphaTime = 0;
		m_IcomAlphaDelayTime = delay;
	}

	public void changeFocus()
	{
		int num0 = (m_Index + 1) % 10;
		int num1 = (m_Index + 1) / 10;
		switch (m_IconType)
		{
			case ICON_TYPE.TYPE_ENEMY:
				{
					m_IconType = ICON_TYPE.TYPE_ENEMY_FOCUS;
					Stage_hex = m_HexSprite[(int)ICON_FOCUS_TYPE.ON];
					Stage_text = m_IconSprite[3];
					Stage_num0 = m_BattleNum[num0 + 10];
					Stage_num1 = m_BattleNum[num1 + 10];
				}
				break;
			case ICON_TYPE.TYPE_ENEMY_FOCUS:
				{
					m_IconType = ICON_TYPE.TYPE_ENEMY;
					Stage_hex = m_HexSprite[(int)ICON_FOCUS_TYPE.OFF];
					Stage_text = m_IconSprite[0];
					Stage_num0 = m_BattleNum[num0];
					Stage_num1 = m_BattleNum[num1];
				}
				break;
			case ICON_TYPE.TYPE_GUERRILLA:
				{
					m_IconType = ICON_TYPE.TYPE_GUERRILLA_FOCUS;
					Warning_text = m_IconSprite[4];
				}
				break;
			case ICON_TYPE.TYPE_GUERRILLA_FOCUS:
				{
					m_IconType = ICON_TYPE.TYPE_GUERRILLA;
					Warning_text = m_IconSprite[1];
				}
				break;
			case ICON_TYPE.TYPE_BOSS:
				{
					m_IconType = ICON_TYPE.TYPE_BOSS_FOCUS;
					Boss_text = m_IconSprite[5];
				}
				break;
			case ICON_TYPE.TYPE_BOSS_FOCUS:
				{
					m_IconType = ICON_TYPE.TYPE_BOSS;
					Boss_text = m_IconSprite[2];
				}
				break;
			default:
				break;
		}
	}
}
