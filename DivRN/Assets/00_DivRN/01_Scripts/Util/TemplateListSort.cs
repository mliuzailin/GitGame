﻿/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	TemplateListSort.cs
	@brief	TemplateListで使用するソート関数まとめたクラス
	@author Developer
	@date 	2013/05/13
	
	
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
	@brief	TemplateListで使用するソート関数まとめたクラス
*/
//----------------------------------------------------------------------------
static public class TemplateListSort
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	内包チェック：
	*/
    //----------------------------------------------------------------------------
    static public int ChkInsideString(string a, string b)
    {
        if (a == b)
            return 1;
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	内包チェック：
	*/
    //----------------------------------------------------------------------------
    static public int ChkInsideUint(uint a, uint b)
    {
        if (a == b)
            return 1;
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	内包チェック：
	*/
    //----------------------------------------------------------------------------
    static public int ChkInsideInt(long a, long b)
    {
        if (a == b)
            return 1;
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	内包チェック：
	*/
    //----------------------------------------------------------------------------
    static public int ChkInsideLong(long a, long b)
    {
        if (a == b)
            return 1;
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用ソート処理：深度
	*/
    //----------------------------------------------------------------------------
    static public int SortDepth(Vector3 a, Vector3 b)
    {
        if (a.z < b.z) return 1;
        if (a.z > b.z) return -1;
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用ソート処理：uint
	*/
    //----------------------------------------------------------------------------
    static public int SortUint(uint a, uint b)
    {
        if (a < b) return 1;
        if (a > b) return -1;
        return 0;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	汎用ソート処理：int
	*/
    //----------------------------------------------------------------------------
    static public int SortInt(int a, int b)
    {
        if (a < b) return 1;
        if (a > b) return -1;
        return 0;
    }


}
