using UnityEngine;
using System.Collections;

/// <summary>
/// SceneObjReferGameMain からエフェクトのプレハブを分離
/// </summary>
public class SceneObjReferGameMainEffect : MonoBehaviour
{
    // アセットバンドル内の AnimationController をロードするために必要（AnimationControllerコンポーネントはアプリ内にあらかじめ一つは持っておかないとエンジンコードがストリップされてしまうため）
    public RuntimeAnimatorController m_DefaultAnimationController = null;


    public GameObject m_2DBattleCostPlus;       //!< 参照オブジェクト：UIエフェクト：戦闘時：コスト追加演出
    public GameObject m_2DUnitDrop;             //!< 参照オブジェクト：UIエフェクト：ユニットドロップ
    public GameObject[] m_2DPlayerDamage;           //!< 参照オブジェクト：UIエフェクト：プレイヤー被ダメ
    public GameObject m_2DSkill;                    //!< 参照オブジェクト：UIエフェクト：スキル成立
    public GameObject m_2DTurnChange;           //!< 参照オブジェクト：UIエフェクト：ターン変化
    public GameObject m_2DGameClear;                //!< 参照オブジェクト：UIエフェクト：ゲームクリア
    public GameObject[] m_2DSkillPowUP;         //!< 参照オブジェクト：UIエフェクト：スキルパワーアップ
    public GameObject[] m_2DSkillStanby;            //!< 参照オブジェクト：UIエフェクト：LBS発動準備完了
    public GameObject m_2DEnemyEvol;                //!< 参照オブジェクト：UIエフェクト：進化エフェクト00
    public GameObject m_2DEnemyEvol2;           //!< 参照オブジェクト：UIエフェクト：進化エフェクト01

    public GameObject m_EnemyDeath;                 //!< 通常死亡エフェクト
    public GameObject m_EnemyDeath_BOSS;            //!< ボス死亡エフェクト

    public GameObject m_Naught_MA_00;               //!< 無
    public GameObject m_Naught_MA_01;
    public GameObject m_Naught_MM_00;
    public GameObject m_Naught_MM_01;
    public GameObject m_Naught_SA_00;
    public GameObject m_Naught_SA_01;
    public GameObject m_Naught_SA_02;
    public GameObject m_Naught_SM_00;
    public GameObject m_Naught_SM_01;
    public GameObject m_Naught_SM_02;

    public GameObject m_Fire_MA_00;                 //!< 火
    public GameObject m_Fire_MA_01;
    public GameObject m_Fire_MM_00;
    public GameObject m_Fire_MM_01;
    public GameObject m_Fire_SA_00;
    public GameObject m_Fire_SA_01;
    public GameObject m_Fire_SA_02;
    public GameObject m_Fire_SM_00;
    public GameObject m_Fire_SM_01;
    public GameObject m_Fire_SM_02;

    public GameObject m_Water_MA_00;                //!< 水
    public GameObject m_Water_MA_01;
    public GameObject m_Water_MM_00;
    public GameObject m_Water_MM_01;
    public GameObject m_Water_SA_00;
    public GameObject m_Water_SA_01;
    public GameObject m_Water_SA_02;
    public GameObject m_Water_SM_00;
    public GameObject m_Water_SM_01;
    public GameObject m_Water_SM_02;

    public GameObject m_Wind_MA_00;                 //!< 風
    public GameObject m_Wind_MA_01;
    public GameObject m_Wind_MM_00;
    public GameObject m_Wind_MM_01;
    public GameObject m_Wind_SA_00;
    public GameObject m_Wind_SA_01;
    public GameObject m_Wind_SA_02;
    public GameObject m_Wind_SM_00;
    public GameObject m_Wind_SM_01;
    public GameObject m_Wind_SM_02;

    public GameObject m_Light_MA_00;                //!< 光
    public GameObject m_Light_MA_01;
    public GameObject m_Light_MM_00;
    public GameObject m_Light_MM_01;
    public GameObject m_Light_SA_00;
    public GameObject m_Light_SA_01;
    public GameObject m_Light_SA_02;
    public GameObject m_Light_SM_00;
    public GameObject m_Light_SM_01;
    public GameObject m_Light_SM_02;

    public GameObject m_Dark_MA_00;                 //!< 闇
    public GameObject m_Dark_MA_01;
    public GameObject m_Dark_MM_00;
    public GameObject m_Dark_MM_01;
    public GameObject m_Dark_SA_00;
    public GameObject m_Dark_SA_01;
    public GameObject m_Dark_SA_02;
    public GameObject m_Dark_SM_00;
    public GameObject m_Dark_SM_01;
    public GameObject m_Dark_SM_02;

    public GameObject m_HM_M_Fire;                  //!< 初音エフェクト：炎：全体
    public GameObject m_HM_M_Water;             //!< 初音エフェクト：水：全体
    public GameObject m_HM_M_Wind;                  //!< 初音エフェクト：風：全体
    public GameObject m_HM_M_Light;             //!< 初音エフェクト：光：全体
    public GameObject m_HM_M_Dark;                  //!< 初音エフェクト：闇：全体
    public GameObject m_HM_M_Naught;                //!< 初音エフェクト：無：全体

    public GameObject m_HM_S_Fire;                  //!< 初音エフェクト：炎：単体
    public GameObject m_HM_S_Water;             //!< 初音エフェクト：水：単体
    public GameObject m_HM_S_Wind;                  //!< 初音エフェクト：風：単体
    public GameObject m_HM_S_Light;             //!< 初音エフェクト：光：単体
    public GameObject m_HM_S_Dark;                  //!< 初音エフェクト：闇：単体
    public GameObject m_HM_S_Naught;                //!< 初音エフェクト：無：単体

    public GameObject m_Heal_00;                    //!< 通常回復	弱
    public GameObject m_Heal_01;                    //!< 通常回復	中
    public GameObject m_Heal_02;                    //!< 通常回復	強
    public GameObject m_Heal_03;                    //!< 通常回復	完全回復

    public GameObject m_Heal_SP;                    //!< 回復：SP
    public GameObject m_Blood;                      //!< 吸血
    public GameObject m_Poison;                     //!< 毒

    public GameObject m_Buff;                       //!< バフ：出現位置-プレイヤー
    public GameObject m_Debuff;                     //!< デバフ：出現位置-敵

    public GameObject m_EnemySkillBuff;             //!< 敵スキル：バフ：出現位置-敵
    public GameObject m_EnemySkillDebuff;           //!< 敵スキル：デバフ：出現位置-プレイヤー

    public GameObject m_EnemySkillFire00;           //!< 敵スキル：炎：全体
    public GameObject m_EnemySkillWind00;           //!< 敵スキル：風：全体
    public GameObject m_EnemySkillWater00;          //!< 敵スキル：水：全体
    public GameObject m_EnemySkillLight00;          //!< 敵スキル：光：全体
    public GameObject m_EnemySkillDark00;           //!< 敵スキル：闇：全体
    public GameObject m_EnemySkillNaught00;         //!< 敵スキル：無：全体

    public GameObject m_EnemySkillHeal_S;           //!< 敵スキル：回復：単体
    public GameObject m_EnemySkillHeal_M;           //!< 敵スキル：回復：全体

    public GameObject m_HandCardTransform;          //!< 手札変化エフェクト：変化
    public GameObject m_HandCardDestroy;            //!< 手札変化エフェクト：破壊

    public GameObject m_PlayerSkillBuff;            //!< プレイヤー側に表示：バフ
    public GameObject m_PlayerSkillDebuff;          //!< プレイヤー側に表示：デバフ


    public GameObject m_SkillSetupNaught;   /// スキル成立時プレイヤーへ向かって飛んでいくエフェクト


    public void init()
    {
        if (EffectManager.HasInstance)
        {
            EffectManager.Instance.unpoolAll();

            EffectManager effect_manager = EffectManager.Instance;

            effect_manager.poolEffect(m_2DBattleCostPlus, 2, 2);    // カード２枚目持った時の飛び散るエフェクト
            effect_manager.poolEffect(m_2DSkill, 5, 5); // スキル成立時のパネルエフェクト
            effect_manager.poolEffect(m_2DTurnChange, 8);   // 敵のターン変化エフェクト

            // 物理・魔法でエフェクトの区別がなくなったので物理のみプール
            effect_manager.poolEffect(m_Naught_MA_00, 0, 2);
            effect_manager.poolEffect(m_Naught_MA_01, 0, 2);
            effect_manager.poolEffect(m_Naught_SA_00, 2, 2);
            effect_manager.poolEffect(m_Naught_SA_01, 2, 2);
            effect_manager.poolEffect(m_Naught_SA_02, 2, 2);

            effect_manager.poolEffect(m_Fire_MA_00, 0, 2);
            effect_manager.poolEffect(m_Fire_MA_01, 0, 2);
            effect_manager.poolEffect(m_Fire_SA_00, 2, 2);
            effect_manager.poolEffect(m_Fire_SA_01, 2, 2);
            effect_manager.poolEffect(m_Fire_SA_02, 2, 2);

            effect_manager.poolEffect(m_Water_MA_00, 0, 2);
            effect_manager.poolEffect(m_Water_MA_01, 0, 2);
            effect_manager.poolEffect(m_Water_SA_00, 2, 2);
            effect_manager.poolEffect(m_Water_SA_01, 2, 2);
            effect_manager.poolEffect(m_Water_SA_02, 2, 2);

            effect_manager.poolEffect(m_Wind_MA_00, 0, 2);
            effect_manager.poolEffect(m_Wind_MA_01, 0, 2);
            effect_manager.poolEffect(m_Wind_SA_00, 2, 2);
            effect_manager.poolEffect(m_Wind_SA_01, 2, 2);
            effect_manager.poolEffect(m_Wind_SA_02, 2, 2);

            effect_manager.poolEffect(m_Light_MA_00, 0, 2);
            effect_manager.poolEffect(m_Light_MA_01, 0, 2);
            effect_manager.poolEffect(m_Light_SA_00, 2, 2);
            effect_manager.poolEffect(m_Light_SA_01, 2, 2);
            effect_manager.poolEffect(m_Light_SA_02, 2, 2);

            effect_manager.poolEffect(m_Dark_MA_00, 0, 2);
            effect_manager.poolEffect(m_Dark_MA_01, 0, 2);
            effect_manager.poolEffect(m_Dark_SA_00, 2, 2);
            effect_manager.poolEffect(m_Dark_SA_01, 2, 2);
            effect_manager.poolEffect(m_Dark_SA_02, 2, 2);

            effect_manager.poolEffect(m_HM_M_Fire, 0, 2);
            effect_manager.poolEffect(m_HM_M_Water, 0, 2);
            effect_manager.poolEffect(m_HM_M_Wind, 0, 2);
            effect_manager.poolEffect(m_HM_M_Light, 0, 2);
            effect_manager.poolEffect(m_HM_M_Dark, 0, 2);
            effect_manager.poolEffect(m_HM_M_Naught, 0, 2);

            effect_manager.poolEffect(m_HM_S_Fire, 0, 2);
            effect_manager.poolEffect(m_HM_S_Water, 0, 2);
            effect_manager.poolEffect(m_HM_S_Wind, 0, 2);
            effect_manager.poolEffect(m_HM_S_Light, 0, 2);
            effect_manager.poolEffect(m_HM_S_Dark, 0, 2);
            effect_manager.poolEffect(m_HM_S_Naught, 0, 2);

            effect_manager.poolEffect(m_SkillSetupNaught, 5, 5);    // スキル成立時のパネルへ飛んでいくエフェクト
            effect_manager.poolEffect(m_Heal_00, 5, 5); // プレイヤー回復エフェクト
        }
    }

    public void destroy()
    {
        if (EffectManager.HasInstance)
        {
            EffectManager.Instance.stopEffectAll();
            EffectManager.Instance.unpoolAll();
        }
    }

    private void OnDestroy()
    {
        //destroy();
    }
}
