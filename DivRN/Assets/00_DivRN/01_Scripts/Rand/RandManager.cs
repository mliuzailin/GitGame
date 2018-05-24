﻿/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	RandManager.cs
	@brief	乱数生成クラス
	@author Developer
	@date 	2012/10/08

	static な乱数生成クラス。
	Randクラスはクラスインスタンスを生成して乱数を返すため、
	乱数シードを指定した処理はRandクラスを使用。
	再現性が必要ない汎用的な乱数システムとしてはstaticなこちらを使用する
	 
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
	@brief	乱数生成クラス
*/
//----------------------------------------------------------------------------
public class RandManager : SingletonComponent<RandManager>
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    static private Rand m_Rand = new Rand();

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief
	*/
    //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();


        //		DateTime cTime = DateTime.Now;
        //		TimeSpan cTimeSpan = cTime - DateTime.MinValue;
        //		uint unRandSeed = (uint)cTimeSpan.TotalSeconds;
        uint unRandSeed = (uint)(TimeUtil.ConvertLocalTimeToServerTime(DateTime.Now) % 0xffffffff);

        m_Rand.SetRandSeed(unRandSeed);
#if BUILD_TYPE_DEBUG
        Debug.Log("RandSeed - [ " + DateTime.Now + " , " + unRandSeed);
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	乱数取得
	*/
    //----------------------------------------------------------------------------
    static public uint GetRand()
    {
        if (m_Rand == null)
        {
            return (uint)UnityEngine.Random.Range(0, 10000);
        }
        return m_Rand.GetRand();
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	乱数取得
	*/
    //----------------------------------------------------------------------------
    static public uint GetRand(uint nMin, uint nMax)
    {
        return m_Rand.GetRand(nMin, nMax);
    }
}

