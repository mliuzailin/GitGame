#if STORE_SIMULATION
#define STORE_WINDOWS_ANDROID
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


/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreDefine.cs
	@brief	ストア課金関連定義
	@author Developer
	@date 	2013/08/03
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using System;
using ServerDataDefine;

/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/

//----------------------------------------------------------------------------
/*!
	@brief	ストア処理結果
*/
//----------------------------------------------------------------------------
public enum STORE_RESULT
{
    /// ストア処理結果：初期状態
    NONE,
    /// ストア処理結果：作業中
    DOING,
    /// ストア処理結果：作業終了（成立）
    FINISH_OK,
    /// ストア処理結果：作業終了（不成立）
    FINISH_NG,
    /// ストア処理結果：作業終了（キャンセル）
    FINISH_CANCEL,
    /// ストア処理結果：作業終了（課金の仕組みがロックされている）
    FINISH_NG_LOCK,
    /// ストア処理結果：作業終了（初期化に失敗している）
    FINISH_NG_INIT,
    /// ストア処理結果：作業終了（補填処理成立）
    FINISH_RESTORE,
    /// ストア処理結果：作業終了（トランザクション処理中）
    FINISH_TRANSACTION,
};

//----------------------------------------------------------------------------
/*!
	@brief	ストア処理リクエストタイプ
*/
//----------------------------------------------------------------------------
public enum STORE_REQUEST
{
    /// ストア処理リクエスト：強制終了補てん	※起動時にローカルセーブに課金中断情報が存在するなら課金処理復帰
    RESTORE,
    /// ストア処理リクエスト：課金購入
    PAY,
};

/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	ストア課金関連定義
*/
//----------------------------------------------------------------------------
static public class StoreDefine
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	アプリのパブリックキー
	*/
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    // パブリックキー：Android用
    //----------------------------------------------------------------------------
#if STORE_ANDROID
    //MEMO Androidの課金キー
#if _MASTER_BUILD
    // jp.example.dg
	public	const	string		APP_PUBLIC_KEY_ANDROID		= "";
#else
    // jp.example.dgrn.dev
	public	const	string		APP_PUBLIC_KEY_ANDROID		= "";
    // jp.example.dgrn（復旧用・サーバに設定されていないので使用しないこと）
#endif

#endif //STORE_ANDROID

    //----------------------------------------------------------------------------
    /*!
		@brief	年齢区分別の課金上限額
	*/
    //----------------------------------------------------------------------------
    /// 課金上限額：16歳未満
    public const int PAY_BORDER_TYPE1 = 5000;
    /// 課金上限額：16歳～19歳
    public const int PAY_BORDER_TYPE2 = 20000;
    /// 課金上限額：20歳以上
    public const int PAY_BORDER_TYPE3 = 99999999;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
};

//----------------------------------------------------------------------------
/*!
	@brief	ストア商品情報
	@note	Storeフォルダの外でプラットフォーム分岐してコードを書かなくていいように外部からの参照にはアクセサクラスを噛ませる。
			商品情報をコンバートしての対応も検討したけど、ローカル保存すると更新タイミングとか諸々怖いのでアクセサのような動きに留める。
*/
//----------------------------------------------------------------------------
public class StoreProduct
{
    public enum KIND_TYPE
    {
        KIND_NOMAL = 0,     // 0:通常イベント
        KIND_FIRST,         // 1:初回購入イベント
        KIND_PRESET,        // 2:プレゼント付きイベント
        KIND_MAX,
    };

    private MasterDataStoreProduct m_Product = null;

    public string product_price_format;

    /// 商品情報
    public MasterDataStoreProduct product
    {
        set { m_Product = value; }
    }
    /// 商品情報が有効か否か
    public bool product_active
    {
        get { return (m_Product != null) ? true : false; }
    }
    /// 商品情報取得：商品ID
    public string product_id
    {
        get { return (m_Product != null) ? m_Product.id : ""; }
    }
    /// 商品情報取得：商品名
    public string product_title
    {
        get { return (m_Product != null) ? m_Product.name : ""; }
    }
    /// 商品情報取得：商品説明
    public string product_detail
    {
        get { return (m_Product != null) ? m_Product.name : ""; }
    }
    /// 商品情報取得：価格
    public uint product_price
    {
        get { return (m_Product != null) ? m_Product.price : 0; }
    }
    /// 商品情報取得：内包個数
    public uint product_num
    {
        get { return (m_Product != null) ? m_Product.point : 1; }
    }
    /// イベントID
    public uint event_id;
    /// 初回加算枚数
    public int add_chip = 0;
    /// 残り時間
    public TimeSpan remaining_time;
    /// イベントでのチップ購入回数
    public int event_chip_count;
    /// イベントタイプ [ 0:通常イベント ][ 1:初回購入イベント ][ 2:プレゼント付きイベント ]
    public uint kind;
    /// イベントテキスト(上)
    public string event_caption;
    /// イベントテキスト(下)
    public string event_text;

    public override string ToString()
    {
        return string.Format(
            "product_active ={0}  product_id={1}  product_title={2} product_price={3} product_num={4} product_price_format={5} event_id={6} add_chip={7} remaining_time={8} event_chip_count{9} kind={10} event_text={11}"
            , product_active
            , product_id
            , product_title
            , product_price
            , product_num
            , product_price_format
            , event_id

            , add_chip
            , remaining_time
            , event_chip_count
            , kind
            , event_text
        );
    }

};


//----------------------------------------------------------------------------
/*!
	@brief	ストア処理リクエスト
*/
//----------------------------------------------------------------------------
public class StoreRequest
{
    /// リクエスト情報：リクエストタイプ
    public STORE_REQUEST m_RequestType;
    /// リクエスト情報：リクエストパラメータ
    public string m_ProductId;
    /// リクエスト情報：イベントID
    public uint m_RequestEeventID;
    /// リクエスト情報：年齢区分
    public int m_RequestAgeType;

    /// リザルト情報
    public STORE_RESULT m_WorkResult;
    /// パケットリザルト情報：リザルトコード
    public API_CODE m_PacketCode = ServerDataDefine.API_CODE.API_CODE_SUCCESS;
    /// リザルト情報：購入補填枚数
    public uint m_WorkResultAddTip;
    /// リザルト情報：プレゼントの付与情報
    public bool m_AddPresent;

    public override string ToString()
    {
        return string.Format(
            "m_RequestType ={0}  m_ProductId={1} m_RequestEeventID={2} m_WorkResult={3} m_PacketCode={4}"
            , m_RequestType
            , m_ProductId
            , m_RequestEeventID
            , m_WorkResult
            , m_PacketCode
        );
    }

};

/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/
