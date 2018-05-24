using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	EffectUtil.cs
    @brief	エフェクトユーティリティークラス
    @author Developer
    @date 	2013/05/16
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/
/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
    @brief	エフェクトユーティリティークラス
*/
//----------------------------------------------------------------------------
public class EffectUtil
{
    //----------------------------------------------------------------------------
    /*!
        @brief	エフェクト再生中チェック
        @note	ParticleSystemは階層構造で連続動作することがあるため、再帰でチェック
    */
    //----------------------------------------------------------------------------
    public static bool ChkParticleSystemPlaying(string handle_name)
    {
        return EffectManager.Instance.isPlayingEffect(handle_name);
    }
} // class EffectUtil


/// <summary>
/// 旧エフェクトマネージャっぽい方法でエフェクトを再生するためのクラス.
/// </summary>
public static class EffectManager_OLD
{
    private const float EFFECT_RESIZE_SCREENSCALE_DEFAULT = 480.0f;

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public enum EEFFECT_ID
    {
        eID_MAP_USE_SP,                 //!< UIエフェクト：SP消費。
        eID_MAP_USE_SP_EMPTY,           //!< UIエフェクト：SP消費（枯渇時）。
        eID_BATTLE_COSTPLUS,            //!< UIエフェクト：戦闘時：コスト追加演出。
        eID_BATTLE_SKILL_OK,            //!< UIエフェクト：スキル成立。
        eID_BATTLE_TURNCHANGE,          //!< UIエフェクト：ターン変化。

        eID_BATTLE_SKILL_NAUGHT_SA_00,  //!< スキルエフェクト：単体：無：物理：弱。
        eID_BATTLE_SKILL_NAUGHT_SA_01,  //!< スキルエフェクト：単体：無：物理：中。
        eID_BATTLE_SKILL_NAUGHT_SA_02,  //!< スキルエフェクト：単体：無：物理：強。
        eID_BATTLE_SKILL_NAUGHT_SM_00,  //!< スキルエフェクト：単体：無：魔法：弱。
        eID_BATTLE_SKILL_NAUGHT_SM_01,  //!< スキルエフェクト：単体：無：魔法：中。
        eID_BATTLE_SKILL_NAUGHT_SM_02,  //!< スキルエフェクト：単体：無：魔法：強。

        eID_BATTLE_SKILL_FIRE_SA_00,    //!< スキルエフェクト：単体：炎：物理：弱。
        eID_BATTLE_SKILL_FIRE_SA_01,    //!< スキルエフェクト：単体：炎：物理：中。
        eID_BATTLE_SKILL_FIRE_SA_02,    //!< スキルエフェクト：単体：炎：物理：強。
        eID_BATTLE_SKILL_FIRE_SM_00,    //!< スキルエフェクト：単体：炎：魔法：弱。
        eID_BATTLE_SKILL_FIRE_SM_01,    //!< スキルエフェクト：単体：炎：魔法：中。
        eID_BATTLE_SKILL_FIRE_SM_02,    //!< スキルエフェクト：単体：炎：魔法：強。

        eID_BATTLE_SKILL_WATER_SA_00,   //!< スキルエフェクト：単体：水：物理：弱。
        eID_BATTLE_SKILL_WATER_SA_01,   //!< スキルエフェクト：単体：水：物理：中。
        eID_BATTLE_SKILL_WATER_SA_02,   //!< スキルエフェクト：単体：水：物理：強。
        eID_BATTLE_SKILL_WATER_SM_00,   //!< スキルエフェクト：単体：水：魔法：弱。
        eID_BATTLE_SKILL_WATER_SM_01,   //!< スキルエフェクト：単体：水：魔法：中。
        eID_BATTLE_SKILL_WATER_SM_02,   //!< スキルエフェクト：単体：水：魔法：強。

        eID_BATTLE_SKILL_WIND_SA_00,    //!< スキルエフェクト：単体：風：物理：弱。
        eID_BATTLE_SKILL_WIND_SA_01,    //!< スキルエフェクト：単体：風：物理：中。
        eID_BATTLE_SKILL_WIND_SA_02,    //!< スキルエフェクト：単体：風：物理：強。
        eID_BATTLE_SKILL_WIND_SM_00,    //!< スキルエフェクト：単体：風：魔法：弱。
        eID_BATTLE_SKILL_WIND_SM_01,    //!< スキルエフェクト：単体：風：魔法：中。
        eID_BATTLE_SKILL_WIND_SM_02,    //!< スキルエフェクト：単体：風：魔法：強。

        eID_BATTLE_SKILL_LIGHT_SA_00,   //!< スキルエフェクト：単体：光：物理：弱。
        eID_BATTLE_SKILL_LIGHT_SA_01,   //!< スキルエフェクト：単体：光：物理：中。
        eID_BATTLE_SKILL_LIGHT_SA_02,   //!< スキルエフェクト：単体：光：物理：強。
        eID_BATTLE_SKILL_LIGHT_SM_00,   //!< スキルエフェクト：単体：光：魔法：弱。
        eID_BATTLE_SKILL_LIGHT_SM_01,   //!< スキルエフェクト：単体：光：魔法：中。
        eID_BATTLE_SKILL_LIGHT_SM_02,   //!< スキルエフェクト：単体：光：魔法：強。

        eID_BATTLE_SKILL_DARK_SA_00,    //!< スキルエフェクト：単体：闇：物理：弱。
        eID_BATTLE_SKILL_DARK_SA_01,    //!< スキルエフェクト：単体：闇：物理：中。
        eID_BATTLE_SKILL_DARK_SA_02,    //!< スキルエフェクト：単体：闇：物理：強。
        eID_BATTLE_SKILL_DARK_SM_00,    //!< スキルエフェクト：単体：闇：魔法：弱。
        eID_BATTLE_SKILL_DARK_SM_01,    //!< スキルエフェクト：単体：闇：魔法：中。
        eID_BATTLE_SKILL_DARK_SM_02,    //!< スキルエフェクト：単体：闇：魔法：強。

        eID_MAX,
    };

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief	エフェクト生成：3Dエフェクト
    */
    //----------------------------------------------------------------------------
    public static GameObject CreateEffect3D(ref Vector3 vPos, GameObject cOriginObject, GameObject cParentObject, string handle_name = null)
    {
        return EffectManager.Instance.playEffect2(cOriginObject, vPos, Vector3.zero, null, cParentObject, -1.0f, handle_name);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	エフェクト生成：2Dエフェクト
    */
    //----------------------------------------------------------------------------
    public static GameObject CreateEffect2D(ref Vector3 vPos, GameObject cOriginObject, GameObject cParentObject, string handle_name = null)
    {
        return EffectManager.Instance.playEffect2(cOriginObject, vPos, Vector3.zero, null, cParentObject, -1.0f);
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	エフェクト生成：2Dエフェクト
    */
    //----------------------------------------------------------------------------
    public static GameObject CreateEffect2D_NGUI(ref Vector3 vPos, GameObject cOriginObject, GameObject cParentObject, string handle_name = null)
    {
        if (cOriginObject == null)
        {
            Debug.LogError("Original Effect None!");
            return null;
        }


        if (cParentObject != null)
        {
            GameObject cUIObjRootSearch = cParentObject;
            GameObject cUIObjRoot = null;
            for (; cUIObjRootSearch != null;)
            {
                if (cUIObjRootSearch.transform.parent == null)
                {
                    break;
                }
                cUIObjRootSearch = cUIObjRootSearch.transform.parent.gameObject;
            }

            return CreateEffect2D(ref vPos, cOriginObject, cParentObject, handle_name, cUIObjRoot);
        }

        Debug.LogError("Parent Object Instance None!");
        return null;

    }

    //----------------------------------------------------------------------------
    /*!
        @brief	エフェクト生成：2Dエフェクト
    */
    //----------------------------------------------------------------------------
    public static GameObject CreateEffect2D(ref Vector3 vPos, GameObject cOriginObject, GameObject cParentObject, string handle_name, GameObject cUIRoot)
    {
        return EffectManager.Instance.playEffect2(cOriginObject, vPos, Vector3.zero, null, cParentObject, getScaleNGUI(cUIRoot), handle_name);
    }

    public static float getScaleNGUI(GameObject cUIRootObj)
    {
        if (null == cUIRootObj)
        {
            return 1.0f;
        }

        float scale = cUIRootObj.gameObject.transform.localScale.x;     // xyzすべて同じscale
        float curScale = (1.0f / scale);
        if (EFFECT_RESIZE_SCREENSCALE_DEFAULT >= curScale)
        {
            return 1.0f;
        }

        //-----------------------------------------------------------------
        // Default倍率を計算した倍率で割り、正しいスケール値を計算する
        //-----------------------------------------------------------------
        float meshScale = EFFECT_RESIZE_SCREENSCALE_DEFAULT / curScale;

        return meshScale;
    }
}
