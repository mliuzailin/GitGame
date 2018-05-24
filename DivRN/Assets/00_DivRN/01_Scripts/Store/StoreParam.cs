#if BUILD_TYPE_DEBUG
#define STORE_BUY_TIP_LOG
#endif

/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreParam.cs
	@brief	ストア系のパラメータ保持クラス
	@author Developer
	@date 	2013/08/03
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
#if STORE_BUY_TIP_LOG
using UnityEngine;
#endif
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
/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	ストア操作用パラメータ：
*/
//----------------------------------------------------------------------------
static public class StoreParam
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    static public bool m_StoreProductEventRenew = true;     //!< ストアイベント一覧取得の実行リクエストフラグ

    static public StoreProduct[] m_StoreProductBase = null;     //!< ストアイベント表示：ストア商品一覧のイベント無視基本状態
    static public StoreProduct[] m_StoreProductChanged = null;      //!< ストアイベント表示：ストア商品一覧のイベント中置き換え状態

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	現在時間で有効なストアイベントを考慮した商品一覧を構築
		@note	現在時で一度データを固めることで、色んな場所でストアイベントをチェックして時間差で不具合が生じるのを防ぐ。各処理はここで固めた状態を参照する
	*/
    //----------------------------------------------------------------------------
    static public void SetupStoreProductListNow()
    {
#if STORE_BUY_TIP_LOG
        if (TimeManager.Instance != null)
        {
            Debug.Log("StoreProductList Setup Now!! - " + TimeManager.Instance.m_TimeNow);
        }
#endif

        //----------------------------------------
        // 一旦クリア
        //----------------------------------------
        m_StoreProductBase = null;
        m_StoreProductChanged = null;

        //----------------------------------------
        // 置き換え前の通常商品リストを構築
        //----------------------------------------
        m_StoreProductBase = new StoreProduct[6];
        m_StoreProductBase[0] = StoreUtil.GetProductFromNum(0);
        m_StoreProductBase[1] = StoreUtil.GetProductFromNum(1);
        m_StoreProductBase[2] = StoreUtil.GetProductFromNum(2);
        m_StoreProductBase[3] = StoreUtil.GetProductFromNum(3);
        m_StoreProductBase[4] = StoreUtil.GetProductFromNum(4);
        m_StoreProductBase[5] = StoreUtil.GetProductFromNum(5);

        //----------------------------------------
        // イベントを考慮した状態の商品リストを構築
        //----------------------------------------
        m_StoreProductChanged = new StoreProduct[6];
        m_StoreProductChanged[0] = StoreUtil.GetReplaceEventProduct(m_StoreProductBase[0]);
        m_StoreProductChanged[1] = StoreUtil.GetReplaceEventProduct(m_StoreProductBase[1]);
        m_StoreProductChanged[2] = StoreUtil.GetReplaceEventProduct(m_StoreProductBase[2]);
        m_StoreProductChanged[3] = StoreUtil.GetReplaceEventProduct(m_StoreProductBase[3]);
        m_StoreProductChanged[4] = StoreUtil.GetReplaceEventProduct(m_StoreProductBase[4]);
        m_StoreProductChanged[5] = StoreUtil.GetReplaceEventProduct(m_StoreProductBase[5]);

    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ストア商品取得：イベント考慮外
	*/
    //----------------------------------------------------------------------------
    static public StoreProduct GetStoreProductBase(int nAccess)
    {
        if (m_StoreProductBase == null
        || m_StoreProductBase.Length < nAccess)
        {
            return null;
        }
        else
        {
            return m_StoreProductBase[nAccess];

        }
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ストア商品取得：イベント考慮
	*/
    //----------------------------------------------------------------------------
    static public StoreProduct GetStoreProductChanged(int nAccess)
    {
        if (m_StoreProductChanged == null
        || m_StoreProductChanged.Length < nAccess)
        {
            return null;
        }
        else
        {
            return m_StoreProductChanged[nAccess];
        }
    }
}

