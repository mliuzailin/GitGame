#if STORE_SIMULATION
#define STORE_WINDOWS_ANDROID
//	#define STORE_IPHONE
#else
#if UNITY_EDITOR
#define STORE_WINDOWS_ANDROID
#elif UNITY_ANDROID
#define STORE_ANDROID
#elif UNITY_IPHONE
#define STORE_IPHONE
#elif UNITY_STANDALONE_WIN
#define STORE_WINDOWS_ANDROID
#endif
#endif

#if BUILD_TYPE_DEBUG
#define ACTIVE_STORE_LOGGER
#endif

/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreManager.cs
	@brief	ストア課金管理クラス
	@author Developer
	@date 	
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using ServerDataDefine;
using Prime31;
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

public class StoreManager : SingletonComponent<StoreManager>
{
#if STORE_WINDOWS_ANDROID
    const string STORE_SIMULATE_KEY_RECEIPT = "STORE_SIMULATE_RECEIPT";     //!< 補填用情報：レシート
    const string STORE_SIMULATE_KEY_SIGNED = "STORE_SIMULATE_SIGNED";       //!< 補填用情報：サイン
    const string STORE_SIMULATE_KEY_PRODUCT = "STORE_SIMULATE_PRODUCT";     //!< 補填用情報：商品ID
#endif

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public bool m_UserStoreEnable = false;  //!< ストアを使用できるか？
    private List<StoreRequest> m_RequestParam = null;	//!< リクエスト情報：リクエストパラメータ

    private Action<StoreRequest> finishAction = null;
    private Action<bool> successAction = null;

#if STORE_ANDROID
	private List< GooglePurchase > m_AndPurchases = null;
#elif STORE_IPHONE
	private bool  firstSkipAwaitingConfirmationEvent = true;
	private List< StoreKitProduct > m_iOSProductList = null;
	private	List< StoreKitTransaction >	m_iOSProductTransaction	= null;
#endif

#if STORE_WINDOWS_ANDROID
    //----------------------------------------------------------------------------
    /*!
		@brief	開発時用捏造レシート
	*/
    //----------------------------------------------------------------------------
    const string ANDROID_DMY_SIGN = "DUMMY SIGN";
    const string ANDROID_DMY2_RECEIPT_TIP_PRODUCTID_FRONT = "{\"packageName\":\"jp.example.dgrn\",\"orderId\":\"00000000000000000000.0000000000000000\",\"productId\":\"";
    const string ANDROID_DMY2_RECEIPT_TIP_PRODUCTID_BACK = "\",\"developerPayload\":\"DEVELOP_PAYLOAD\",\"type\":null,\"purchaseTime\":1393525871769,\"purchaseState\":0,\"purchaseToken\":\"xxxxxxxxxxxxxxxxxxxxxxxx.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\"}";
#endif

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	ストア処理コールバックイベント設定
	*/
    //----------------------------------------------------------------------------
    private void SetupCallBack()
    {
#if STORE_ANDROID
		//--------------------------------
		// ストア処理イベントコールバックを登録
		// ここで登録したコールバックが各種イベント時に自動で呼び出される
		// ※ものによっては非同期なので注意
		//--------------------------------
		GoogleIABManager.billingSupportedEvent						+= CB_Android_BillingSupported;			// ストアコールバック：初期化完了コールバック（アプリ内課金可能時）
		GoogleIABManager.billingNotSupportedEvent					+= CB_Android_BillingSupportedNone;		// ストアコールバック：初期化完了コールバック（アプリ内課金不可時）
		GoogleIABManager.queryInventorySucceededEvent				+= CB_Android_QueryInventorySucceeded;	// ストアコールバック：購入済み情報の取得成功
		GoogleIABManager.queryInventoryFailedEvent					+= CB_Android_QueryInventoryFailed;		// ストアコールバック：購入済み情報の取得失敗
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent	+= CB_Android_PurchaseCompleteAwaiting;	// ストアコールバック：購入時のレシート通知
		GoogleIABManager.purchaseSucceededEvent						+= CB_Android_PurchaseSucceeded;		// ストアコールバック：購入処理正常終了
		GoogleIABManager.purchaseFailedEvent						+= CB_Android_PurchaseFailed;			// ストアコールバック：購入処理エラー（キャンセル含む）
		GoogleIABManager.consumePurchaseSucceededEvent				+= CB_Android_ConsumePurchaseSucceeded;	// ストアコールバック：消費リクエスト成功
		GoogleIABManager.consumePurchaseFailedEvent					+= CB_Android_ConsumePurchaseFailed;	// ストアコールバック：消費リクエスト失敗

#if ACTIVE_STORE_LOGGER
		Debug.Log("[StoreManager] ANDROID");
#endif
#elif STORE_IPHONE
		//--------------------------------
		// ストア処理イベントコールバックを登録
		// ここで登録したコールバックが各種イベント時に自動で呼び出される
		// ※ものによっては非同期なので注意
		//--------------------------------
		StoreKitManager.purchaseSuccessfulEvent						+= CB_iOS_PurchaseSuccessful;			// ストアコールバック：購入処理正常終了
		StoreKitManager.purchaseCancelledEvent						+= CB_iOS_PurchaseCancelled;			// ストアコールバック：購入処理キャンセル
		StoreKitManager.purchaseFailedEvent							+= CB_iOS_PurchaseFailed;				// ストアコールバック：購入処理エラー

		StoreKitManager.productListReceivedEvent					+= CB_iOS_ProductListReceive;			// ストアコールバック：商品一覧取得
		StoreKitManager.productListRequestFailedEvent				+= CB_iOS_ProductListFailed;			// ストアコールバック：商品一覧取得失敗

		StoreKitManager.productPurchaseAwaitingConfirmationEvent	+= CB_iOS_AwaitingConfirmationEvent;	// ストアコールバック：トランザクション完遂待ち

//		StoreKitManager.restoreTransactionsFailedEvent				+= CB_iOS_RestoreTransactionsFailed;	// ストアコールバック：リストアエラー
//		StoreKitManager.restoreTransactionsFinishedEvent			+= CB_iOS_RestoreTransactionsFinished;	// ストアコールバック：リストア処理完遂
//		StoreKitManager.paymentQueueUpdatedDownloadsEvent			+= CB_iOS_PaymentQueueUpdatedDownloads;	// ストアコールバック：ホスティングシステムのDL状況
//		StoreKitManager.receiptValidationSuccessful					+= CB_iOS_ReceiptValidationSuccessful;	// ストアコールバック：レシート検証完了
//		StoreKitManager.receiptValidationRawResponseReceived		+= CB_iOS_ReceiptValidationRawResponse;	// ストアコールバック：レシート検証レスポンス
//		StoreKitManager.receiptValidationFailed						+= CB_iOS_ReceiptValidationFailed;		// ストアコールバック：レシート検証エラー

#if ACTIVE_STORE_LOGGER
		Debug.Log("[StoreManager] STORE_IPHONE");
#endif
#else
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] STORE_WINDOWS_ANDROID");
#endif
#endif

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ストア処理コールバックイベント破棄
	*/
    //----------------------------------------------------------------------------
    private void DeleteCallBack()
    {
#if STORE_ANDROID
		//--------------------------------
		// ストア処理イベントコールバックを解除
		//--------------------------------
		GoogleIABManager.billingSupportedEvent						-= CB_Android_BillingSupported;			// ストアコールバック：初期化完了コールバック（アプリ内課金可能時）
		GoogleIABManager.billingNotSupportedEvent					-= CB_Android_BillingSupportedNone;		// ストアコールバック：初期化完了コールバック（アプリ内課金不可時）
		GoogleIABManager.queryInventorySucceededEvent				-= CB_Android_QueryInventorySucceeded;	// ストアコールバック：購入済み情報の取得成功
		GoogleIABManager.queryInventoryFailedEvent					-= CB_Android_QueryInventoryFailed;		// ストアコールバック：購入済み情報の取得失敗
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent	-= CB_Android_PurchaseCompleteAwaiting;	// ストアコールバック：購入時のレシート通知
		GoogleIABManager.purchaseSucceededEvent						-= CB_Android_PurchaseSucceeded;		// ストアコールバック：購入処理正常終了
		GoogleIABManager.purchaseFailedEvent						-= CB_Android_PurchaseFailed;			// ストアコールバック：購入エラー（キャンセル含む）
		GoogleIABManager.consumePurchaseSucceededEvent				-= CB_Android_ConsumePurchaseSucceeded;	// ストアコールバック：消費リクエスト成功
		GoogleIABManager.consumePurchaseFailedEvent					-= CB_Android_ConsumePurchaseFailed;	// ストアコールバック：消費リクエスト失敗
#elif STORE_IPHONE
		//--------------------------------
		// ストア処理イベントコールバックを解除
		//--------------------------------
		StoreKitManager.purchaseSuccessfulEvent						-= CB_iOS_PurchaseSuccessful;			// ストアコールバック：購入処理正常終了
		StoreKitManager.purchaseCancelledEvent						-= CB_iOS_PurchaseCancelled;			// ストアコールバック：購入処理キャンセル
		StoreKitManager.purchaseFailedEvent							-= CB_iOS_PurchaseFailed;				// ストアコールバック：購入処理エラー

		StoreKitManager.productListReceivedEvent					-= CB_iOS_ProductListReceive;			// ストアコールバック：商品一覧取得
		StoreKitManager.productListRequestFailedEvent				-= CB_iOS_ProductListFailed;			// ストアコールバック：商品一覧取得失敗
		
		StoreKitManager.productPurchaseAwaitingConfirmationEvent	-= CB_iOS_AwaitingConfirmationEvent;	// ストアコールバック：トランザクション完遂待ち

//		StoreKitManager.restoreTransactionsFailedEvent				-= CB_iOS_RestoreTransactionsFailed;	// ストアコールバック：リストアエラー
//		StoreKitManager.restoreTransactionsFinishedEvent			-= CB_iOS_RestoreTransactionsFinished;	// ストアコールバック：リストア処理完遂
//		StoreKitManager.paymentQueueUpdatedDownloadsEvent			-= CB_iOS_PaymentQueueUpdatedDownloads;	// ストアコールバック：ホスティングシステムのDL状況
//		StoreKitManager.receiptValidationSuccessful					-= CB_iOS_ReceiptValidationSuccessful;	// ストアコールバック：レシート検証完了
//		StoreKitManager.receiptValidationRawResponseReceived		-= CB_iOS_ReceiptValidationRawResponse;	// ストアコールバック：レシート検証レスポンス
//		StoreKitManager.receiptValidationFailed						-= CB_iOS_ReceiptValidationFailed;		// ストアコールバック：レシート検証エラー
#endif
    }


    public void fsmNext()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] fsmNext");
#endif
        StoreManagerFSM.Instance.SendFsmEvent("DO_PURHCASE_NEXT", 0);
    }

    private void fsmEnd()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] fsmEnd");
#endif
        StoreManagerFSM.Instance.SendFsmEvent("DO_PURCHASE_END", 0);
    }

    private void fsmRestore()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] fsmRestore");
#endif
        StoreManagerFSM.Instance.SendFsmEvent("DO_PURCHASE_RESTORE", 0);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：インスタンス制御関連：インスタンス破棄時に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void OnDestroy()
    {
        DeleteCallBack();

        base.OnDestroy();
    }

    public void OnInitalize()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnInitalize");
#endif

        SetupCallBack();

#if STORE_ANDROID
		//--------------------------------
		// ストア処理初期化
		//--------------------------------
		GoogleIAB.init( StoreDefine.APP_PUBLIC_KEY_ANDROID );
		//GoogleIAB.enableLogging( true );

		m_UserStoreEnable = false;

		// CB_Android_BillingSupported
		// CB_Android_BillingSupportedNone
		// 2パターンでSendFsmNextEventを呼ぶ
#elif STORE_IPHONE
		//--------------------------------
		// ストア処理初期化
		//--------------------------------
		StoreKitManager.autoConfirmTransactions = false; // トランザクションの自動解決が走らないようにしておく
		StoreKitBinding.setShouldSendTransactionUpdateEvents( false );

		m_UserStoreEnable = StoreKitBinding.canMakePayments();
		m_iOSProductTransaction = new List< StoreKitTransaction >();

		fsmNext();
#else //STORE_WINDOWS_ANDROID
        //--------------------------------
        // ストア処理初期化
        //--------------------------------
        m_UserStoreEnable = true;
        fsmNext();
#endif

    }

    public void OnStoreList()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnStoreList");
#endif
        // 実際の取得開始はstartStoreListで行う
    }

    public void startStoreList(Action<bool> _successAction)
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] startStoreList");
#endif
        setSuccessAction(_successAction);

#if STORE_IPHONE
		//商品リストを別途取得する必要がある
		List< string > cProductIDList = new List< string >();
		var masterDataStoreProduct = MasterFinder<MasterDataStoreProduct>.Instance.GetAll();
		for( int i = 0;i < masterDataStoreProduct.Length;i++ )
		{
			if( masterDataStoreProduct[i].platform != MasterDataDefineLabel.StoreType.IOS )
			{
				continue;
			}

			var productid = masterDataStoreProduct[i].id;
			cProductIDList.Add( productid );

#if ACTIVE_STORE_LOGGER
			Debug.Log( "[StoreManager] startStoreList: " + productid );
#endif
		}
		
		//--------------------------------
		// 一覧取得リクエスト発行
		//--------------------------------
		StoreKitBinding.requestProductData( cProductIDList.ToArray() );

		// CB_iOS_ProductListReceive
		// CB_iOS_ProductListFailed
		// 2パターンでSendFsmNextEventを呼ぶ
#else
        fsmNext();
#endif
    }

    public void OnStoreListWait()
    {
        bool sucess = false;
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnStoreListWait");
#endif

        try
        {
#if STORE_IPHONE
            //リスト取得できたか？
            if(m_iOSProductList == null)
            {
                sucess = false;
            }
            else
            {
                sucess = true;
            }
#else
            sucess = true;

            //ストア初期化できない場合は失敗
            sucess = m_UserStoreEnable;
#endif
            successAction(sucess);
        }
        catch (Exception e)
        {
            TitleFSM.Instance.SendFsmNextEvent();
        }
        finally
        {
            fsmNext();
        }
    }

    public void OnPurchaseRestore()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseRestore");
#endif
        // 実際の取得開始はstartPurchaseRestoreで行う
    }

    public void startPurchaseRestore(Action<bool> _successAction)
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] startPurchaseRestore");
#endif
        setSuccessAction(_successAction);
#if STORE_ANDROID
		List< string > cProductIDList = new List< string >();
		var masterDataStoreProduct = MasterFinder<MasterDataStoreProduct>.Instance.GetAll();
		for( int i = 0 ;i < masterDataStoreProduct.Length ; i++ )
		{
			if( masterDataStoreProduct[i].platform != MasterDataDefineLabel.StoreType.ANDROID )
			{
				continue;
			}
			var pucheseid =  masterDataStoreProduct[i].id;
			cProductIDList.Add( pucheseid );
#if ACTIVE_STORE_LOGGER
			Debug.Log( "[StoreManager] startPurchaseRestore: " + pucheseid);
#endif
		}
		GoogleIAB.queryInventory( cProductIDList.ToArray() );
#elif STORE_IPHONE
		fsmNext();
#else
        fsmNext();
#endif
    }

    public void OnPurchaseRestoreWait()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseRestoreWait");
#endif

        try
        {
#if STORE_ANDROID
            if(m_AndPurchases != null && m_AndPurchases.Count > 0)
            {
                fsmRestore();
                return;
            }
#elif STORE_IPHONE
            if(m_iOSProductTransaction != null && m_iOSProductTransaction.Count > 0)
            {
                fsmRestore();
                return;
            }
#endif
            //リストアイテムがない＆Windows
            successAction(true);
        }
        catch (Exception e)
        {
            TitleFSM.Instance.SendFsmNextEvent();
        }
        finally
        {
            fsmNext();
        }
    }

    public void OnPurchaseWait()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseWait");
#endif
        // 購入処理を待つ AddRequest→StartRequest
        // アイテムが積まれていたら進める
    }

    public void OnPurchaseStart()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseStart");
#endif

        if (m_RequestParam == null || m_RequestParam.Count <= 0)
        {
            purchaseError(STORE_RESULT.FINISH_CANCEL);
            return;
        }

        StoreRequest storerequest = m_RequestParam.First();

#if STORE_IPHONE
		ServerDataUtilSend.SendPacketAPI_StorePayPrev_ios(	storerequest.m_ProductId ,
															storerequest.m_RequestEeventID ,
															storerequest.m_RequestAgeType)
#else
        ServerDataUtilSend.SendPacketAPI_StorePayPrev_android(storerequest.m_ProductId,
                                                              storerequest.m_RequestEeventID,
                                                              storerequest.m_RequestAgeType)
#endif
        .setSuccessAction(_data =>
        {

#if ACTIVE_STORE_LOGGER
            Debug.Log("[StoreManager] OnPurchaseStart success: ");
#endif

            //--------------------------------
            // 	購入事前API正常受理！
            //--------------------------------
            fsmNext();
        })
        .setErrorAction(_data =>
        {
            //--------------------------------
            // 	購入事前APIエラー発生！
            //--------------------------------
            API_CODE code = _data.m_PacketCode;

#if ACTIVE_STORE_LOGGER
            Debug.Log("[StoreManager] OnPurchaseStart error code: " + code);
#endif

            purchaseError(STORE_RESULT.FINISH_NG, code);
        })
        .SendStart();
    }

    public void OnPurchaseBefor()
    {
        if (m_RequestParam.IsNullOrEmpty())
        {

            purchaseError(STORE_RESULT.FINISH_CANCEL);
            return;
        }

        StoreRequest storerequest = m_RequestParam.First();

#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseBefor m_ProductId: " + storerequest.m_ProductId);
#endif

#if STORE_ANDROID
		//--------------------------------
		// ストアダイアログオープン！
		// 
		// 第２引数は自由に使っていいらしい
		// Googleアカウントを複数の端末に入れた場合の挙動のセーフティのため、この端末の識別情報としてユーザーIDを添付する
		//--------------------------------
		GoogleIAB.purchaseProduct( storerequest.m_ProductId , UserDataAdmin.Instance.m_StructPlayer.user.user_id.ToString() );
#elif STORE_IPHONE
		//--------------------------------
		// ストアダイアログオープン！
		//--------------------------------
		firstSkipAwaitingConfirmationEvent = false;
		StoreKitBinding.purchaseProduct( storerequest.m_ProductId , 1 );
		fsmNext();
#else //STORE_WINDOWS_ANDROID

        //--------------------------------
        // ストアダイアログの分岐っぽい処理
        //--------------------------------
        Dialog dialog = null;
        dialog = DialogManager.Open2B_Direct("Buy?", "Buy or Cancel?", "common_button4", "common_button5", true, true).
        SetYesEvent(() =>
        {
            dialog.Hide();
            fsmNext();
        }).
        SetNoEvent(() =>
        {
            dialog.Hide();
            purchaseError(STORE_RESULT.FINISH_CANCEL);
        }).SetStrongYes();
#endif
    }

    public void OnPurchaseStore()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseStore");
#endif

        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.SERVER_API);

#if STORE_ANDROID
		var productList = new List< string >();
		StoreRequest storerequest = m_RequestParam.First();
		productList.Add(storerequest.m_ProductId);
		GoogleIAB.queryInventory( productList.ToArray() );
		// CB_Android_QueryInventorySucceeded
		// CB_Android_QueryInventoryFailed
		// 2パターンでSendFsmNextEventを呼ぶ
#elif STORE_IPHONE
		// CB_iOS_AwaitingConfirmationEvent を待つ
#else //STORE_WINDOWS_ANDROID
        //--------------------------------
        // 送信する捏造レシート設定
        //--------------------------------
        string strReceipt = "";
        // プロダクトIDだけ可変で他は同じ
        StoreRequest storerequest = m_RequestParam.First();

        if (storerequest.m_ProductId != null)
        {
            strReceipt = ANDROID_DMY2_RECEIPT_TIP_PRODUCTID_FRONT + storerequest.m_ProductId + ANDROID_DMY2_RECEIPT_TIP_PRODUCTID_BACK;
        }

        strReceipt = strReceipt.Replace("DEVELOP_PAYLOAD", UserDataAdmin.Instance.m_StructPlayer.user.user_id.ToString());
        PlayerPrefs.SetString(STORE_SIMULATE_KEY_RECEIPT, strReceipt);
        PlayerPrefs.SetString(STORE_SIMULATE_KEY_SIGNED, ANDROID_DMY_SIGN);
        PlayerPrefs.SetString(STORE_SIMULATE_KEY_PRODUCT, storerequest.m_ProductId);
        PlayerPrefs.Save();

        fsmNext();
#endif
    }

    public void OnPurchaseRestoreMain()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseRestoreMain");
#endif

#if BUILD_TYPE_DEBUG
        //memo 通常処理

        OnPurchaseReceipt();

        //memo 強制決済リストア処理
        // CB_Android_QueryInventorySucceededで処理するキューに追加している
        // prodct nameと証明書が決済に使用したものか確認する
        // 決済用のGoogleアカウントに注意する（決済に使用したアカウントじゃにと通らない）
        // StoreDefine　→　APP_PUBLIC_KEY_ANDROIDのKEYをアプリのライセンスキーにする必要がある
        //
        // 復帰処理はOnPurchaseRestoreMainないの処理を書き換える
        // 1.OnPurchaseReceipt()をコメントアウトする。（実行されないようにする）
        // 2.recoverParchece()のコメントを解除する。（実行されるようにする）
        // 3.APKを作成して実行する（STORE_SIMULATIONを外す、署名に注意する jp.example.dgrn.dev or jp.example.dg）
        // 4.APKをインストールして立ち上げる。各種ダウンロードが終わった後で実行される
        //  adb logcat で [StoreManager]を追加するとログが表示される
        //  [StoreManager] recoverParcheceが出て決済処理のログが続いてが通っていれば問題ない
        // 5.OnPurchaseReceipt()のコメントアウトを解除する。（実行されるようにする）
        // 6.recoverParchece()をコメントアウトする。（実行されないようにする）
        // 
        // エラーコード
        // 表 1. In-app Billing Version 3 API からの呼び出しに対するレスポンス コードの一覧
        // https://developer.android.com/google/play/billing/billing_reference.html?hl=ja

        //recoverParchece();
#else
		OnPurchaseReceipt();
#endif //BUILD_TYPE_DEBUG
    }

    /*
	購入情報強制消化
	*/
#if BUILD_TYPE_DEBUG
    public void recoverParchece()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] recoverParchece");
#endif

#if STORE_ANDROID
		// StoreDefine　→　APP_PUBLIC_KEY_ANDROIDのKEYをアプリのライセンスキーにする必要がある
		GooglePurchase purchesData = null;

		if(m_AndPurchases != null)
		{
			purchesData 	= m_AndPurchases.First();
			GoogleIAB.consumeProduct( purchesData.productId ); //productId
		}
		else
		{
			fsmNext();
		}
#else
        OnPurchaseReceipt();
#endif
    }
#endif //BUILD_TYPE_DEBUG

    public void OnPurchaseRestoreCheck()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseRestoreCheck");
#endif

#if STORE_ANDROID
		if(m_AndPurchases.Count > 0)
		{
			m_AndPurchases.RemoveAt(0);
		}

		if(m_AndPurchases.Count > 0)
		{
			fsmNext();
		}
		else
		{
			fsmEnd();		
		}
#elif STORE_IPHONE
		if(m_iOSProductTransaction.Count > 0)
		{
			m_iOSProductTransaction.RemoveAt(0);
		}

		if(m_iOSProductTransaction.Count > 0)
		{
			fsmNext();
		}
		else
		{
			fsmEnd();		
		}
#else //STORE_WINDOWS_ANDROID
        fsmEnd();
#endif
    }

    public void OnPurchaseReceipt()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseReceipt");
#endif

        string strProdictid = null;
        string strReceiptData = null;
        string strSignData = null;

#if STORE_ANDROID
		GooglePurchase purchesData = null;

		if(m_AndPurchases != null){
			purchesData 	= m_AndPurchases.First();
			strProdictid	= purchesData.productId;
			strReceiptData	= purchesData.originalJson;
			strSignData		= purchesData.signature;			
		}
		else
		{
			purchaseError(STORE_RESULT.FINISH_NG, API_CODE.API_CODE_Android_OnPurchaseReceipt_ERROR);			
			return;
		}
#elif STORE_IPHONE
		StoreKitTransaction purchesData = null;
		if(m_iOSProductTransaction != null){
			purchesData		= m_iOSProductTransaction.First();
			strProdictid	= purchesData.transactionIdentifier;
			strReceiptData	= purchesData.base64EncodedTransactionReceipt;
			strSignData		= "";			
		}
		else
		{
			purchaseError(STORE_RESULT.FINISH_NG, API_CODE.API_CODE_Android_OnPurchaseReceipt_ERROR);			
			return;
		}
#else //STORE_WINDOWS_ANDROID
        strProdictid = "dummey";
        strReceiptData = PlayerPrefs.GetString(STORE_SIMULATE_KEY_RECEIPT, "");
        strSignData = PlayerPrefs.GetString(STORE_SIMULATE_KEY_SIGNED, "");
#endif 

#if STORE_IPHONE
        ServerDataUtilSend.SendPacketAPI_StorePay_ios( strReceiptData )
#else
        ServerDataUtilSend.SendPacketAPI_StorePay_android(strReceiptData, strSignData)
#endif
        .setSuccessAction(_data =>
        {
            if (UserDataAdmin.Instance != null
            && UserDataAdmin.Instance.m_StructPlayer != null)
            {
#if ACTIVE_STORE_LOGGER
                Debug.Log("[StoreManager] OnPurchaseReceipt success: ");
#endif

#if STORE_IPHONE
				RecvStorePay_ios _recv = _data.GetResult<RecvStorePay_ios>();
#else
                RecvStorePay_android _recv = _data.GetResult<RecvStorePay_android>();
#endif
                if (_recv != null)
                {
                    UserDataAdmin.Instance.m_StructPlayer.have_stone = _recv.result.stone_ct;
                    UserDataAdmin.Instance.m_StructPlayer.have_stone_free = _recv.result.stone_ct_free;
                    UserDataAdmin.Instance.m_StructPlayer.have_stone_pay = _recv.result.stone_ct_pay;
                    UserDataAdmin.Instance.m_StructPlayer.pay_total = _recv.result.pay_total;
                    UserDataAdmin.Instance.m_StructPlayer.pay_month = _recv.result.pay_month;
                    UserDataAdmin.Instance.updatePlayerParam();
                }
            }

            arrangedStore(strProdictid);
        })
        .setErrorAction(_data =>
        {
            API_CODE code = _data.m_PacketCode;

#if ACTIVE_STORE_LOGGER
            Debug.Log("[StoreManager] OnPurchaseReceipt error code: " + code);
#endif
            // API_CODE_PAY_WIDE_USER_NOT_MATCH
            // ユーザー切り替えなどで消化できないことがある
#if STORE_IPHONE
			if(code == ServerDataDefine.API_CODE.API_CODE_PAY_IOS_RECEIPT_PURCHASE){
				// iOSは成功
				arrangedStore( strProdictid );
			}else
#endif
            {
                purchaseError(STORE_RESULT.FINISH_NG, code);
            }
        })
        .SendStart();
    }

    public void OnPurchaseFinish()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseStoreFinish");
#endif

#if STORE_ANDROID
		fsmNext();
#elif STORE_IPHONE
		fsmNext();
#else //STORE_WINDOWS_ANDROID
        PlayerPrefs.DeleteKey(STORE_SIMULATE_KEY_RECEIPT);
        PlayerPrefs.DeleteKey(STORE_SIMULATE_KEY_SIGNED);
        PlayerPrefs.DeleteKey(STORE_SIMULATE_KEY_PRODUCT);
        PlayerPrefs.Save();

        //アイテム付き判定
        purchaseSuccess();
#endif
    }

    public void OnPurchaseEnd()
    {
        LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.SERVER_API);

#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] OnPurchaseEnd");
#endif

#if STORE_IPHONE
		if(m_iOSProductTransaction != null && m_iOSProductTransaction.Count > 0)
		{
			m_iOSProductTransaction.RemoveAt(0);
		}
#endif

        if (m_RequestParam != null && m_RequestParam.Count > 0)
        {
            m_RequestParam.RemoveAt(0);
        }

        fsmNext();
    }

    private void purchaseSuccess()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] purchaseSuccess");
#endif
        if (finishAction != null)
        {
            //var cRequestResult = new StoreRequest();
            StoreRequest cRequestResult = m_RequestParam.First();

            if (cRequestResult != null)
            {
                cRequestResult.m_WorkResult = STORE_RESULT.FINISH_OK;
                cRequestResult.m_PacketCode = ServerDataDefine.API_CODE.API_CODE_SUCCESS;
                finishAction(cRequestResult);
            }
            finishAction = null;
        }

        fsmNext();
    }

    private void purchaseError(STORE_RESULT result, API_CODE code = ServerDataDefine.API_CODE.API_CODE_SUCCESS)
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] purchaseError: " + code);
#endif
        if (finishAction != null)
        {
            StoreRequest cRequestResult = m_RequestParam.First();

            if (cRequestResult != null)
            {
                cRequestResult.m_WorkResult = result;
                cRequestResult.m_PacketCode = code;
                finishAction(cRequestResult);
            }
            finishAction = null;
        }

        fsmEnd();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ストア関連処理リクエスト
		@note	リクエストはリストに積む
	*/
    //----------------------------------------------------------------------------
    public void AddRequest(STORE_REQUEST eRequestType,
                            string strProductId,
                            uint strRequestEeventID,
                            AGE_TYPE eAgeType,
                            uint addTip,
                            bool bPresent)
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] AddRequest");
#endif
        if (m_RequestParam == null)
        {
            m_RequestParam = new List<StoreRequest>();
            m_RequestParam.Clear();
        }

        //--------------------------------
        // 問題ないならリクエストをバッファに積む
        //--------------------------------
        StoreRequest storerequest = new StoreRequest();
        storerequest.m_RequestType = eRequestType;
        storerequest.m_ProductId = strProductId;
        storerequest.m_RequestAgeType = (int)eAgeType;
        storerequest.m_WorkResult = STORE_RESULT.NONE;
        storerequest.m_WorkResultAddTip = addTip;
        storerequest.m_AddPresent = bPresent;
        m_RequestParam.Add(storerequest);

        if (strProductId != null)
        {
            storerequest.m_RequestEeventID = strRequestEeventID;
        }
    }

    public void sefFinshAction(Action<StoreRequest> _finishAction)
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] sefFinshAction");
#endif
        this.finishAction = _finishAction;
    }

    public void setSuccessAction(Action<bool> _successAction)
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] setSuccessAction");
#endif
        this.successAction = _successAction;
    }

    public void StartRequest()
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] StartRequest");
#endif

        fsmNext();
    }

    private void arrangedStore(string productId)
    {
#if ACTIVE_STORE_LOGGER
        Debug.Log("[StoreManager] arrangedStore: " + productId);
#endif

#if STORE_ANDROID
		//ストアの消費処理処理
		if(productId != null)
		{
			GoogleIAB.consumeProduct( productId ); //productId
			//ストアの消費処理処理
			// CB_Android_ConsumePurchaseSucceeded	→　purchaseSuccess()
			// CB_Android_ConsumePurchaseFailed		→　purchaseError ()	
		}
		else
		{
			fsmNext();				
		}
#elif STORE_IPHONE
		//ストアの消費処理処理
		if(productId != null)
		{
			StoreKitBinding.finishPendingTransaction( productId ); //strTransactionID
			// CB_iOS_PurchaseSuccessful purchaseSuccess()
			// CB_iOS_PurchaseFailed purchaseError()
		}
		else
		{
			fsmNext();				
		}
#else //STORE_WINDOWS_ANDROID
        fsmNext();
#endif
    }

    static public MasterDataDefineLabel.StoreType StorePlatform()
    {
#if STORE_IPHONE
		return MasterDataDefineLabel.StoreType.IOS;
#else // STORE_ANDROID STORE_WINDOWS_ANDROID
        return MasterDataDefineLabel.StoreType.ANDROID;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	商品情報を取得
	*/
    //----------------------------------------------------------------------------
    public StoreProduct GetProductFromNum(int nAccess)
    {
#if STORE_IPHONE
		if( m_iOSProductList == null ){
			return null;
		}
#endif
        //--------------------------------
        // 総当たりでチェック
        //--------------------------------
        int nChkCt = 0;
        MasterDataDefineLabel.StoreType nStorePlatform = StorePlatform();
        MasterDataStoreProduct[] nMasterDataStoreProduct = MasterFinder<MasterDataStoreProduct>.Instance.GetAll();
        for (int i = 0; i < nMasterDataStoreProduct.Length; i++)
        {
            if (nMasterDataStoreProduct[i].platform != nStorePlatform)
            {
                continue;
            }

            if (nChkCt == nAccess)
            {
                //--------------------------------
                // アクセサクラスを噛ませて返す
                //--------------------------------
                StoreProduct cProduct = new StoreProduct();
                cProduct.product = nMasterDataStoreProduct[i];
                cProduct.product_price_format = string.Format(GameTextUtil.GetText("sh119q_content5"), nMasterDataStoreProduct[i].price);

#if STORE_IPHONE
				//--------------------------------
				// iOSの場合はストアから取得した現在価格を反映。
				// 2年に一度くらいの頻度で商品価格が勝手に変わることがあるため
				//--------------------------------
				for( int j = 0 ; j < m_iOSProductList.Count ; j++ )
				{
					if( m_iOSProductList[j] == null 
					||	m_iOSProductList[j].productIdentifier != nMasterDataStoreProduct[i].id)
					{
						continue;
					}
						
					if ( m_iOSProductList[j].countryCode == @"JP" )
					{
						cProduct.product_price_format = string.Format( "￥{0}", m_iOSProductList[j].price );
					}
					else
					{
						cProduct.product_price_format = m_iOSProductList[j].formattedPrice;
					}
					break;
				}
#endif
                return cProduct;
            }
            nChkCt++;
        }
        return null;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	商品情報を取得：商品ID指定
	*/
    //----------------------------------------------------------------------------
    public StoreProduct GetProductFromID(string strProductID)
    {
#if STORE_IPHONE
		if( m_iOSProductList == null ){
			return null;
		}
#endif
        //--------------------------------
        // 総当たりでチェック
        //--------------------------------
        MasterDataDefineLabel.StoreType nStorePlatform = StorePlatform();
        MasterDataStoreProduct[] nMasterDataStoreProduct = MasterFinder<MasterDataStoreProduct>.Instance.GetAll();

        for (int i = 0; i < nMasterDataStoreProduct.Length; i++)
        {
            if (nMasterDataStoreProduct[i].platform != MasterDataDefineLabel.StoreType.ALL
            && nMasterDataStoreProduct[i].platform != nStorePlatform)
            {
                continue;
            }

            if (nMasterDataStoreProduct[i].id != strProductID)
            {
                continue;
            }

            //--------------------------------
            // アクセサクラスを噛ませて返す
            //--------------------------------
            StoreProduct cProduct = new StoreProduct();
            cProduct.product = nMasterDataStoreProduct[i];
            cProduct.product_price_format = string.Format(GameTextUtil.GetText("sh119q_content5"), nMasterDataStoreProduct[i].price);

#if STORE_IPHONE
			//--------------------------------
			// iOSの場合はストアから取得した現在価格を反映。
			// 2年に一度くらいの頻度で商品価格が勝手に変わることがあるため
			//--------------------------------
			for( int j = 0 ; j < m_iOSProductList.Count ; j++ )
			{
				if( m_iOSProductList[j] == null 
				||	m_iOSProductList[j].productIdentifier != nMasterDataStoreProduct[i].id )
				{
					continue;
				}

				if ( m_iOSProductList[j].countryCode == @"JP" )
				{
					cProduct.product_price_format = string.Format( "￥{0}", m_iOSProductList[j].price );
				}
				else
				{
					cProduct.product_price_format = m_iOSProductList[j].formattedPrice;
				}
				break;
			}
#endif

            return cProduct;
        }
        return null;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	イベントプロダクトの置き換え
		@note	指定の商品を別のものに置き換える場合はインスタンスが返る
	*/
    //----------------------------------------------------------------------------
    public StoreProduct ReplaceEventProduct(StoreProduct cProduct)
    {
        // リターンデータ
        StoreProduct cReplaceProduct = null;
        MasterDataStoreProductEvent cStoreEvent = GetStoreProductEvent(cProduct);//今までの処理はこの中で行うようにしました。

        if (cStoreEvent != null)
        {
            //残り時間だけここで計算して貰う
            DateTime cTimeEnd = TimeUtil.GetDateTime(cStoreEvent.timing_end);
            // 終了時間に分を足す
            double add_min = (double)cStoreEvent.timing_ed_m;
            if (add_min >= 60.0f) add_min = 0.0f;
            cTimeEnd = cTimeEnd.AddMinutes(add_min);

            TimeSpan cCountDown = cTimeEnd - TimeManager.Instance.m_TimeNow;

            // リターンデータを入れ込み
            cReplaceProduct = GetProductFromID(cStoreEvent.item_after);
            cReplaceProduct.event_id = cStoreEvent.event_id;
            cReplaceProduct.add_chip = cStoreEvent.add_chip;
            cReplaceProduct.remaining_time = cCountDown;
            cReplaceProduct.event_chip_count = cStoreEvent.event_chip_count;
            // 初回購入用データ追加
            cReplaceProduct.kind = (uint)cStoreEvent.kind;
            cReplaceProduct.event_caption = cStoreEvent.event_caption_text;
            cReplaceProduct.event_text = cStoreEvent.event_text;
        }

        return cReplaceProduct;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	MasterDataStoreProductEvent取得
		@note	有効なMasterDataStoreProductEventが欲しかったので追加
	*/
    //----------------------------------------------------------------------------
    public MasterDataStoreProductEvent GetStoreProductEvent(StoreProduct cProduct)
    {
#if STORE_IPHONE
		//--------------------------------
		// 製品情報が取得できていないなら変化後も無し
		//--------------------------------
		if( m_iOSProductList == null )
		{
			return null;
		}
#endif

        //--------------------------------
        // 
        //--------------------------------
        if (cProduct == null)
        {
            return null;
        }
        if (UserDataAdmin.Instance.m_StructProductEvent == null)
        {
            return null;
        }

        //--------------------------------
        // マスターデータ内には
        // iOS,Androidの商品リストが一緒に入っているので選別用IDを取得
        //--------------------------------
        MasterDataDefineLabel.StoreType nStorePlatform = StorePlatform();

        // リターンデータ
        MasterDataStoreProductEvent cReturnStoreEvent = null;
        //--------------------------------
        // 全てのイベント情報を総当たりでチェック
        //--------------------------------
        for (int j = 0; j < UserDataAdmin.Instance.m_StructProductEvent.Length; j++)
        {
            //--------------------------------
            // 有効チェック
            //--------------------------------
            MasterDataStoreProductEvent cStoreEvent = UserDataAdmin.Instance.m_StructProductEvent[j];

            if (cStoreEvent.active != MasterDataDefineLabel.BoolType.ENABLE)
                continue;
            if (cStoreEvent.item_before != cProduct.product_id)
                continue;
            if (cStoreEvent.platform != MasterDataDefineLabel.StoreType.ALL
            && cStoreEvent.platform != nStorePlatform
            )
            {
                continue;
            }

            //--------------------------------
            // 日付チェック
            //--------------------------------
            DateTime cTimeStart = TimeUtil.GetDateTime(cStoreEvent.timing_start);
            // 開始時間に分を足す.
            double add_min = (double)cStoreEvent.timing_st_m;
            if (add_min >= 60.0f) add_min = 0.0f;
            cTimeStart = cTimeStart.AddMinutes(add_min);

            DateTime cTimeEnd = TimeUtil.GetDateTime(cStoreEvent.timing_end);
            // 終了時間に分を足す
            add_min = (double)cStoreEvent.timing_ed_m;
            if (add_min >= 60.0f) add_min = 0.0f;
            cTimeEnd = cTimeEnd.AddMinutes(add_min);

            if (!(cTimeStart <= TimeManager.Instance.m_TimeNow && TimeManager.Instance.m_TimeNow <= cTimeEnd))
            {
                continue;
            }

            //--------------------------------
            // ストアイベント情報が存在することが確定！
            // 
            // パラメータを商品情報として加工して返す
            //--------------------------------
            // リターンデータを入れ込み.
            // 今登録されているイベントの優先度より新規イベントの優先度の方が大きい場合
            if (cReturnStoreEvent == null ||
                CheckPriorityStoreEvent(cReturnStoreEvent.kind, cStoreEvent.kind))
            {
                cReturnStoreEvent = cStoreEvent;
            }
        }
        return cReturnStoreEvent;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	どちらのストアイベントが優先されるのかチェック
		@note	既存のイベントが優先されるならfalse
				新規のイベントが優先されるならtrue
				同じならfalseを返す
	*/
    //----------------------------------------------------------------------------
    public bool CheckPriorityStoreEvent(MasterDataDefineLabel.EventType uNowEventKind, MasterDataDefineLabel.EventType uCheckEventKind)
    {
        MasterDataDefineLabel.EventType[] uEventPriority = new MasterDataDefineLabel.EventType[]{
            MasterDataDefineLabel.EventType.FIRST,	// 初回購入イベント
			MasterDataDefineLabel.EventType.NORMAL,	// 通常イベント
			MasterDataDefineLabel.EventType.PRESENT	// プレゼントつきイベント
		};

        int nNowPriority = uEventPriority.Length;
        int nCheckPriority = uEventPriority.Length;

        for (int i = 0; i < uEventPriority.Length; i++)
        {
            if (uEventPriority[i] == uNowEventKind) nNowPriority = i;
            if (uEventPriority[i] == uCheckEventKind) nCheckPriority = i;
        }
        // 判定
        return (nNowPriority > nCheckPriority);
    }

    /********
    コールバック
    **********/

#if STORE_ANDROID
	//--------------------------------
	// ■課金フロー
	// 	３．その商品が既に購入済でないかチェックし、購入済状態なら消費リクエストを発行
	// 		本来は補填処理が先に走るので不要。購入済だと課金できなくなるのでセーフティの意味合いが強い
	// 			[ GoogleIAB.queryInventory ]
	// 				→ CB[ GoogleIABManager.queryInventorySucceededEvent	] ※購入済み情報の取得成功
	// 				→ CB[ GoogleIABManager.queryInventoryFailedEvent		] ※購入済み情報の取得失敗
	// 	４．購入済で消費前の商品があるなら消費処理
	// 			[ GoogleIAB.consumeProduct ]
	// 				→ CB[ GoogleIABManager.consumePurchaseSucceededEvent	] ※消費成立
	// 				→ CB[ GoogleIABManager.consumePurchaseFailedEvent		] ※消費不成立
	// 	５．購入処理前APIをゲームサーバーに発行
	// 			[ ServerDataUtilSend.SendPacketAPI_StorePayPrev_android ]
	// 	６．リクエストの商品IDをもとにストアをオープン
	// 		ストア側の処理が勝手に働いて結果が帰ってくる
	// 			[ GoogleIAB.purchaseProduct ]
	// 				→ CB[ GoogleIABManager.purchaseCompleteAwaitingVerificationEvent	] ※購入時のレシート通知
	// 						※ハング時補填用にレシートは届いたらローカル保存しておく
	// 				→ CB[ GoogleIABManager.purchaseSucceededEvent	] ※購入処理正常終了
	// 				→ CB[ GoogleIABManager.purchaseFailedEvent		] ※購入処理エラー（キャンセル含む）
	// 	７．購入が成立した場合、商品の購入結果をゲームサーバーへ送信
	// 			[ ServerDataUtilSend.SendPacketAPI_StorePay_android ]
	// 	８．商品の消費を実行
	// 			[ GoogleIAB.consumeProduct ]
	// 				→ CB[ GoogleIABManager.consumePurchaseSucceededEvent	] ※消費成立
	// 				→ CB[ GoogleIABManager.consumePurchaseFailedEvent		] ※消費不成立
	// 	９．ローカルに保存しているレシート情報を完全破棄
	//--------------------------------

	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：初期化完了コールバック（アプリ内課金可能時）
	*/
	//----------------------------------------------------------------------------
	void CB_Android_BillingSupported()
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_BillingSupported" );
#endif

		m_UserStoreEnable = true;
		fsmNext();
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：初期化完了コールバック（アプリ内課金不可時）
	*/
	//----------------------------------------------------------------------------
	void CB_Android_BillingSupportedNone( string error )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_BillingSupportedNone - " + error );
#endif
		fsmNext();
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：購入済み情報の取得成功
		@note	起動時に前回終了時にストア処理が半端で終わっている場合を考慮して取得する。
				ここでリストに何か入っている場合、それは課金後に付与されていない可能性があるので補てんする
	*/
	//----------------------------------------------------------------------------
	void CB_Android_QueryInventorySucceeded( List<GooglePurchase> purchases, List<GoogleSkuInfo> skus )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_QueryInventorySucceeded start" );
#endif
		m_AndPurchases = null;

		if(purchases != null &&	purchases.Count > 0)
		{
			m_AndPurchases = new List<GooglePurchase>();

			for( int i = 0;i < purchases.Count;i++ )
			{
				GooglePurchase item = purchases[i];
#if ACTIVE_STORE_LOGGER
				Debug.Log( "[StoreManager] CB_Android_QueryInventorySucceeded find: " + item.packageName );
#endif

				if( item == null
				||	item.purchaseState != GooglePurchase.GooglePurchaseState.Purchased )
				{
						continue;
				}

				m_AndPurchases.Add( item );
#if ACTIVE_STORE_LOGGER
				Debug.Log( "[StoreManager] CB_Android_QueryInventorySucceeded add: " + item );
#endif
			}
		}
		else{
#if ACTIVE_STORE_LOGGER
				Debug.Log( "[StoreManager] CB_Android_QueryInventorySucceeded purchases.Count == 0 or null " );
#endif
		}
	
		fsmNext();
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：購入済み情報の取得失敗
	*/
	//----------------------------------------------------------------------------
	void CB_Android_QueryInventoryFailed( string error )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_QueryInventoryFailed - " + error );
#endif
		m_AndPurchases = null;
		purchaseError(STORE_RESULT.FINISH_NG, API_CODE.API_CODE_Android_QueryInventoryFailed_ERROR);
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：購入時のレシート通知
	*/
	//----------------------------------------------------------------------------
	void CB_Android_PurchaseCompleteAwaiting( string purchaseData, string signature )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_PurchaseCompleteAwaiting - \n" + purchaseData + "\n\n" + signature );
#endif
		// Androidは補填処理と購入処理をprime31情報を引っ張ってくるようになっているのでレシートを端末保存したりしない
		// レシートを受け取るだけの処理なので特に何も必要ないはず
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：購入処理正常終了
	*/
	//----------------------------------------------------------------------------
	void CB_Android_PurchaseSucceeded( GooglePurchase purchase )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_PurchaseSucceeded" );
#endif

		// Androidは補填処理と購入処理をprime31情報を引っ張ってくるようになっているのでレシートを端末保存したりしない
		fsmNext();
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：購入処理エラー（キャンセル含む）
	*/
	//----------------------------------------------------------------------------
	void CB_Android_PurchaseFailed( string error, int err_code )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_PurchaseFailed - " + error + " " + err_code );
#endif

		purchaseError(STORE_RESULT.FINISH_CANCEL);
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：消費リクエスト成功
		@note	※Prime31では消費型のアイテムは消費リクエストを発行しないと再度購入できなくなる
	*/
	//----------------------------------------------------------------------------
	void CB_Android_ConsumePurchaseSucceeded( GooglePurchase purchase )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_ConsumePurchaseSucceeded" );
#endif

		purchaseSuccess();
	}
	
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：消費リクエスト失敗
	*/
	//----------------------------------------------------------------------------
	void CB_Android_ConsumePurchaseFailed( string error )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_Android_ConsumePurchaseFailed - " + error );
#endif
		//想定外エラー
		purchaseError(STORE_RESULT.FINISH_NG, API_CODE.API_CODE_Android_ConsumePurchaseFailed_ERROR);
	}

#endif   // STORE_ANDROID


#if STORE_IPHONE
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：商品一覧取得
	*/
	//----------------------------------------------------------------------------
	void CB_iOS_ProductListReceive( List<StoreKitProduct> productList )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_iOS_ProductListReceive" );
#endif
		m_iOSProductList = new List< StoreKitProduct >();

		for( int i = 0 ; i < productList.Count ; i++ )
		{
			m_iOSProductList.Add( productList[i] );
#if ACTIVE_STORE_LOGGER
			Debug.Log( "[StoreManager] CB_iOS_ProductListReceive: " + productList[i].productIdentifier + " / price = " + productList[i].price );
#endif
		}

		fsmNext();
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：商品一覧取得失敗
	*/
	//----------------------------------------------------------------------------
	void CB_iOS_ProductListFailed( string error )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "CB_iOS_ProductListFailed - " + error );
#endif
		m_iOSProductList = null;

		fsmNext();
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：購入処理エラー
	*/
	//----------------------------------------------------------------------------
	void CB_iOS_PurchaseFailed( string error )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_iOS_PurchaseFailed - " + error );
#endif

		purchaseError(STORE_RESULT.FINISH_CANCEL);
	}
	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：購入処理キャンセル
	*/
	//----------------------------------------------------------------------------
	void CB_iOS_PurchaseCancelled( string error )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_iOS_PurchaseCancelled - " + error );
#endif

		purchaseError(STORE_RESULT.FINISH_CANCEL);
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	ストアコールバック：購入処理正常終了
	*/
	//----------------------------------------------------------------------------
	void CB_iOS_PurchaseSuccessful( StoreKitTransaction transaction )
	{
#if ACTIVE_STORE_LOGGER
		Debug.Log( "[StoreManager] CB_iOS_PurchaseSuccessful" );
#endif

		purchaseSuccess();
	}
	
	//----------------------------------------------------------------------------
	/*!
	    @brief	ストアコールバック：購入処理トランザクション完遂待ち
	*/
	//----------------------------------------------------------------------------
	void CB_iOS_AwaitingConfirmationEvent( StoreKitTransaction transaction )
	{
#if ACTIVE_STORE_LOGGER
		//Debug.Log( "[StoreManager] CB_iOS_AwaitingConfirmationEvent - \n" + transaction.productIdentifier);
		Debug.Log( "[StoreManager] CB_iOS_AwaitingConfirmationEvent - \n" + transaction.productIdentifier + " , " + transaction.transactionIdentifier + " , " + transaction.transactionState + "\n" + transaction.base64EncodedTransactionReceipt );
#endif

		if(m_iOSProductTransaction == null)
		{
			m_iOSProductTransaction = new List< StoreKitTransaction >();
		}

		m_iOSProductTransaction.Add( transaction );

		if(firstSkipAwaitingConfirmationEvent == false)
		{
			fsmNext();
		}
	}
#endif

}
