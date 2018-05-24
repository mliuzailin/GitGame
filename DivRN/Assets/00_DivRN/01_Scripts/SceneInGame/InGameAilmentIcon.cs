using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
	@class		InGameAilmentIcon
	@brief		状態異常アイコンの更新スクリプト
*/
//----------------------------------------------------------------------------
public class InGameAilmentIcon : MonoBehaviour
{

    private const float DURATIONTIME = 1.0f;                                //!< 継続時間

    public GameObject m_Icon = null;                                //!< 管理スプライト
    public GameObject m_Balloon = null;                             //!< 吹き出し
    public GameObject m_BalloonText = null;                             //!< 吹き出しテキスト
    private float m_DeltaTime = DURATIONTIME;                       //!< 内部時間
    private StatusAilmentChara m_AilmentChara;

    private int m_SpriteIndex = 0;

    public Sprite m_SpriteAtkUp;
    public Sprite m_SpriteAtkDown;
    public Sprite m_SpriteDefUp;
    public Sprite m_SpriteDefDown;
    public Sprite m_SpritePoison;
    public Sprite m_SpriteDeadlyPoison;
    public Sprite m_SpriteTimerUp;
    public Sprite m_SpriteTimerDown;
    public Sprite m_SpriteDark;
    public Sprite m_SpriteMute;

    private Sprite[] m_Sprites;
    private int[] m_Turns;

    //------------------------------------------------------------------------
    /*!
		@brief		状態異常のパラメータの設定
		@param[in]	StatusAilmentChara		(charaAilmentChara)		キャラ状態異常管理
	*/
    //------------------------------------------------------------------------
    public void SetStatus(StatusAilmentChara charaAilmentChara)
    {
        if (charaAilmentChara == null)
        {
            return;
        }
        m_AilmentChara = charaAilmentChara;
    }

    //------------------------------------------------------------------------
    /*!
		@brief		開始処理
	*/
    //------------------------------------------------------------------------
    protected void Start()
    {
        UnityUtil.SetObjectEnabledOnce(m_Icon, false);
        UnityUtil.SetObjectEnabledOnce(m_Balloon, false);
        UnityUtil.SetObjectEnabledOnce(m_BalloonText, false);

        m_Sprites = new Sprite[]
        {
            m_SpriteAtkUp,
            m_SpriteAtkDown,
            m_SpriteDefUp,
            m_SpriteDefDown,
            m_SpritePoison,
            m_SpriteDeadlyPoison,
            m_SpriteTimerUp,
            m_SpriteTimerDown,
            m_SpriteDark,
            m_SpriteMute,
        };
        m_Turns = new int[m_Sprites.Length];
    }

    //------------------------------------------------------------------------
    /*!
		@brief		定期更新処理
	*/
    //------------------------------------------------------------------------
    protected void Update()
    {
        if (m_AilmentChara == null)
        {
            return;
        }

        // 定期更新
        m_DeltaTime -= Time.deltaTime;
        if (m_DeltaTime > 0.0f)
        {
            return;
        }

        // 継続時間設定
        m_DeltaTime = DURATIONTIME;

        updateAilment();

        int turn = m_Turns[m_SpriteIndex];
        if (turn < 0)
        {
            UnityUtil.SetObjectEnabledOnce(m_Icon, false);
            UnityUtil.SetObjectEnabledOnce(m_Balloon, false);
            UnityUtil.SetObjectEnabledOnce(m_BalloonText.gameObject, false);
            return;
        }

        Sprite sprite = m_Sprites[m_SpriteIndex];

        // アイコン表示
        if (sprite != null)
        {
            SpriteRenderer sprite_renderer = m_Icon.GetComponent<SpriteRenderer>();
            if (sprite_renderer != null)
            {
                sprite_renderer.sprite = sprite;
                UnityUtil.SetObjectEnabledOnce(m_Icon, true);
            }
        }

        // 残りターン数書き換え
        if (m_BalloonText != null)
        {
            // 残りのターン数が０でなければターン数を表示
            if (turn > 0)
            {
                TextMeshPro text_mesh = m_BalloonText.GetComponent<TextMeshPro>();
                if (text_mesh != null)
                {
                    text_mesh.text = turn.ToString("00");
                    UnityUtil.SetObjectEnabledOnce(m_Balloon, true);
                    UnityUtil.SetObjectEnabledOnce(m_BalloonText.gameObject, true);
                }
                else
                {
                    UnityUtil.SetObjectEnabledOnce(m_Balloon, false);
                    UnityUtil.SetObjectEnabledOnce(m_BalloonText.gameObject, false);
                }
            }
            else
            {
                // 非表示
                UnityUtil.SetObjectEnabledOnce(m_Balloon, false);
                UnityUtil.SetObjectEnabledOnce(m_BalloonText.gameObject, false);
            }
        }
    }

    //------------------------------------------------------------------------
    /*!
		@brief		カレントIDを次に表示するアイコンIDに更新
		@param[in]	bool[]		(array)		ステータス配列
	*/
    //------------------------------------------------------------------------
    private void updateAilment()
    {
        for (int idx = 0; idx < m_Turns.Length; idx++)
        {
            m_Turns[idx] = -1;
        }

        if (m_AilmentChara != null)
        {
            for (int idx = 0; idx < m_AilmentChara.GetAilmentCount(); idx++)
            {
                StatusAilment ailment = m_AilmentChara.GetAilment(idx);
                if (ailment != null)
                {
                    Sprite sprite = getSprite(ailment.nType);
                    if (sprite != null)
                    {
                        for (int spr_idx = 0; spr_idx < m_Sprites.Length; spr_idx++)
                        {
                            if (sprite == m_Sprites[spr_idx])
                            {
                                if (ailment.nLife > m_Turns[spr_idx])
                                {
                                    m_Turns[spr_idx] = ailment.nLife;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        int next_index = (m_SpriteIndex + 1) % m_Sprites.Length;
        while (next_index != m_SpriteIndex)
        {
            if (m_Turns[next_index] >= 0)
            {
                m_SpriteIndex = next_index;
                break;
            }

            next_index = (next_index + 1) % m_Sprites.Length;
        }
    }

    public Sprite getSprite(MasterDataDefineLabel.AilmentType ailment_type)
    {
        Sprite ret_val = null;

        switch (ailment_type)
        {
            case MasterDataDefineLabel.AilmentType.DEF_UP: ret_val = m_SpriteDefUp; break;
            case MasterDataDefineLabel.AilmentType.DEF_DOWN: ret_val = m_SpriteDefDown; break;
            case MasterDataDefineLabel.AilmentType.POISON: ret_val = m_SpritePoison; break;
            case MasterDataDefineLabel.AilmentType.PANIC: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.HAPPY_TRESURE: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.BATTLE_MONEY_UP: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.FEAR: ret_val = m_SpriteTimerDown; break;
            case MasterDataDefineLabel.AilmentType.TIMER: ret_val = m_SpriteTimerUp; break;
            case MasterDataDefineLabel.AilmentType.TORCH: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.DARK: ret_val = m_SpriteDark; break;
            case MasterDataDefineLabel.AilmentType.ALARM: ret_val = null; break;

            case MasterDataDefineLabel.AilmentType.ATK_UP: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_FIRE: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_WATER: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_LIGHT: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_DARK: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_WIND: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_HEAL: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_NAUGHT: ret_val = m_SpriteAtkUp; break;

            case MasterDataDefineLabel.AilmentType.DEF_UP_FIRE: ret_val = m_SpriteDefUp; break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_WATER: ret_val = m_SpriteDefUp; break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_LIGHT: ret_val = m_SpriteDefUp; break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_DARK: ret_val = m_SpriteDefUp; break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_WIND: ret_val = m_SpriteDefUp; break;
            case MasterDataDefineLabel.AilmentType.DEF_UP_HEAL: ret_val = null; break; //!< 防御力アップ[回復属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_NAUGHT: ret_val = m_SpriteDefUp; break;

            case MasterDataDefineLabel.AilmentType.ATK_UP_HUMAN: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_DRAGON: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_GOD: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_DEMON: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_CREATURE: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_BEAST: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_MACHINE: ret_val = m_SpriteAtkUp; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_EGG: ret_val = null; break;

            case MasterDataDefineLabel.AilmentType.PNL_HOLD_FIRE: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_WATER: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_LIGHT: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_DARK: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_WIND: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_HEAL: ret_val = null; break;
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_NAUGHT: ret_val = null; break;

            case MasterDataDefineLabel.AilmentType.SKILL_NOTUSE: ret_val = m_SpriteMute; break;
            case MasterDataDefineLabel.AilmentType.SKILL_HEALING: ret_val = null; break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FIRE: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WATER: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_LIGHT: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DARK: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WIND: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_NAUGHT: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HEAL: ret_val = null; break;

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_GOD: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_BEAST: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DEMON: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DRAGON: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FAIRY: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HUMAN: ret_val = m_SpriteAtkDown; break;
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_MACHINE: ret_val = m_SpriteAtkDown; break;

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP: ret_val = m_SpriteDefDown; break;
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_FIRE: ret_val = m_SpriteDefDown; break;
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_WATER: ret_val = m_SpriteDefDown; break;
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_LIGHT: ret_val = m_SpriteDefDown; break;
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_DARK: ret_val = m_SpriteDefDown; break;
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_WIND: ret_val = m_SpriteDefDown; break;
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_NAUGHT: ret_val = m_SpriteDefDown; break;
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_HEAL: ret_val = null; break;

            case MasterDataDefineLabel.AilmentType.HANDCARD_DEFAULT: ret_val = null; break;

            case MasterDataDefineLabel.AilmentType.POISON_MAXHP: ret_val = m_SpriteDeadlyPoison; break; //!< 毒[最大HP割合]

            case MasterDataDefineLabel.AilmentType.NON_RECOVERY_ALL: ret_val = null; break;

            case MasterDataDefineLabel.AilmentType.AUTO_PLAY_SKILL: ret_val = null; break;

            default: break;

        }

        return ret_val;
    }

} // class InGameAilmentIcon

///----------------------------------------------------------------------------
/*!
	@class		StatusAilmentIconUtil
	@brief		状態異常アイコンユーティリティー
*/
//----------------------------------------------------------------------------
class StatusAilmentIconUtil
{
    //------------------------------------------------------------------------
    /*!
		@brief		アイコンリソースの名前を取得
		@param[in]	EAILMENT_TYPE		(eIcon)		状態異常ID
		@return		string				[アイコンリソース名]
	*/
    //------------------------------------------------------------------------
    //	static public string GetResourceName( EAILMENT_TYPE eIcon ) {
    static public string GetResourceName(MasterDataDefineLabel.AilmentType ailment_type)
    {

        string resName = "";

        switch (ailment_type)
        {
            case MasterDataDefineLabel.AilmentType.DEF_UP: resName = "defIncrease"; break;
            case MasterDataDefineLabel.AilmentType.DEF_DOWN: resName = "defDecrease"; break;
            case MasterDataDefineLabel.AilmentType.POISON: resName = "poison"; break;
            case MasterDataDefineLabel.AilmentType.PANIC: resName = "panic"; break;
            case MasterDataDefineLabel.AilmentType.HAPPY_TRESURE: resName = "happyTreasure"; break;
            case MasterDataDefineLabel.AilmentType.BATTLE_MONEY_UP: resName = "battleMoneyUP"; break;
            case MasterDataDefineLabel.AilmentType.FEAR: resName = "fear"; break;
            case MasterDataDefineLabel.AilmentType.TIMER: resName = "timer"; break;
            case MasterDataDefineLabel.AilmentType.TORCH: resName = "torch"; break;
            case MasterDataDefineLabel.AilmentType.DARK: resName = "dark"; break;
            case MasterDataDefineLabel.AilmentType.ALARM: resName = "alarm"; break;

            case MasterDataDefineLabel.AilmentType.ATK_UP: resName = "atkUp"; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_FIRE: resName = "atkUpFire"; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_WATER: resName = "atkUpWater"; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_LIGHT: resName = "atkUpLight"; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_DARK: resName = "atkUpDark"; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_WIND: resName = "atkUpWind"; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_HEAL: resName = ""; break;
            case MasterDataDefineLabel.AilmentType.ATK_UP_NAUGHT: resName = "atkUpNone"; break;

            case MasterDataDefineLabel.AilmentType.DEF_UP_FIRE: resName = "Damage mitigation_fire"; break;  //!< 防御力アップ[炎属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_WATER: resName = "Damage mitigation_water"; break;    //!< 防御力アップ[水属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_LIGHT: resName = "Damage mitigation_light"; break;    //!< 防御力アップ[光属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_DARK: resName = "Damage mitigation_dark"; break;  //!< 防御力アップ[闇属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_WIND: resName = "Damage mitigation_wind"; break;  //!< 防御力アップ[風属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_HEAL: resName = ""; break; //!< 防御力アップ[回復属性]
            case MasterDataDefineLabel.AilmentType.DEF_UP_NAUGHT: resName = "Damage mitigation_none"; break;    //!< 防御力アップ[無属性]

            case MasterDataDefineLabel.AilmentType.ATK_UP_HUMAN: resName = "atkUpHuman"; break; //!< 攻撃力アップ[人間]
            case MasterDataDefineLabel.AilmentType.ATK_UP_DRAGON: resName = "atkUpDragon"; break; //!< 攻撃力アップ[ドラゴン]
            case MasterDataDefineLabel.AilmentType.ATK_UP_GOD: resName = "atkUpGod"; break; //!< 攻撃力アップ[神]
            case MasterDataDefineLabel.AilmentType.ATK_UP_DEMON: resName = "atkUpDemon"; break; //!< 攻撃力アップ[魔物]
            case MasterDataDefineLabel.AilmentType.ATK_UP_CREATURE: resName = "atkUpFairy"; break; //!< 攻撃力アップ[妖精]
            case MasterDataDefineLabel.AilmentType.ATK_UP_BEAST: resName = "atkUpBeast"; break; //!< 攻撃力アップ[獣]
            case MasterDataDefineLabel.AilmentType.ATK_UP_MACHINE: resName = "atkUpMachine"; break; //!< 攻撃力アップ[機械]
            case MasterDataDefineLabel.AilmentType.ATK_UP_EGG: resName = ""; break; //!< 攻撃力アップ[強化合成用]

            case MasterDataDefineLabel.AilmentType.PNL_HOLD_FIRE: resName = "panelFixFire"; break; //!< 属性固定[炎]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_WATER: resName = "panelFixWater"; break; //!< 属性固定[水]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_LIGHT: resName = "panelFixLight"; break; //!< 属性固定[光]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_DARK: resName = "panelFixDark"; break; //!< 属性固定[闇]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_WIND: resName = "panelFixWind"; break; //!< 属性固定[風]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_HEAL: resName = "panelFixHeal"; break; //!< 属性固定[回復]
            case MasterDataDefineLabel.AilmentType.PNL_HOLD_NAUGHT: resName = "panelFixNone"; break; //!< 属性固定[無]

            case MasterDataDefineLabel.AilmentType.SKILL_NOTUSE: resName = "AS_Prohibition"; break; //!< スキル使用禁止
            case MasterDataDefineLabel.AilmentType.SKILL_HEALING: resName = ""; break; //!< スキル使用禁止

            case MasterDataDefineLabel.AilmentType.ATK_DOWN: resName = "atkDown"; break; //!< 攻撃力ダウン
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FIRE: resName = "atkDownFire"; break; //!< 攻撃力ダウン【炎】
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WATER: resName = "atkDownWater"; break; //!< 攻撃力ダウン【水】
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_LIGHT: resName = "atkDownLight"; break; //!< 攻撃力ダウン【光】
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DARK: resName = "atkDownDark"; break; //!< 攻撃力ダウン【闇】
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_WIND: resName = "atkDownWind"; break; //!< 攻撃力ダウン【風】
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_NAUGHT: resName = "atkDownNaught"; break; //!< 攻撃力ダウン【無】
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HEAL: resName = ""; break; //!< 攻撃力ダウン【回復】

            case MasterDataDefineLabel.AilmentType.ATK_DOWN_GOD: resName = "atkDownGod"; break; //!< 攻撃力ダウン[神]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_BEAST: resName = "atkDownBeast"; break; //!< 攻撃力ダウン[獣]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DEMON: resName = "atkDownDemon"; break; //!< 攻撃力ダウン[魔物]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_DRAGON: resName = "atkDownDragon"; break; //!< 攻撃力ダウン[ドラゴン]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_FAIRY: resName = "atkDownFairy"; break; //!< 攻撃力ダウン[妖精]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_HUMAN: resName = "atkDownHuman"; break; //!< 攻撃力ダウン[人間]
            case MasterDataDefineLabel.AilmentType.ATK_DOWN_MACHINE: resName = "atkDownMachine"; break; //!< 攻撃力ダウン[機械]

            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP: resName = "damageUp"; break; //!< 被ダメアップ
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_FIRE: resName = "damageUpFire"; break; //!< 被ダメアップ【炎】
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_WATER: resName = "damageUpWater"; break; //!< 被ダメアップ【水】
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_LIGHT: resName = "damageUpLight"; break; //!< 被ダメアップ【光】
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_DARK: resName = "damageUpDark"; break; //!< 被ダメアップ【闇】
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_WIND: resName = "damageUpWind"; break; //!< 被ダメアップ【風】
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_NAUGHT: resName = "damageUpNaught"; break; //!< 被ダメアップ【無】
            case MasterDataDefineLabel.AilmentType.DEF_DMG_UP_HEAL: resName = ""; break; //!< 被ダメアップ【回復】

            case MasterDataDefineLabel.AilmentType.HANDCARD_DEFAULT: resName = "handCardDefault"; break; //!< 手札出現率デフォルト

            case MasterDataDefineLabel.AilmentType.POISON_MAXHP: resName = "poisonDeadly"; break; //!< 毒[最大HP割合]

            case MasterDataDefineLabel.AilmentType.NON_RECOVERY_ALL: resName = "nonRecoveryAll"; break; //!< 回復不可[全]

            case MasterDataDefineLabel.AilmentType.AUTO_PLAY_SKILL: resName = ""; break;

            default: break;

        }

        return resName;
    }
} // class StatusAilmentIconUtil


