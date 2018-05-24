/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	LayoutSwitchManager.cs
	@brief	レイアウト切り替え管理クラス
	@author Developer
	@date 	2013/06/24
	
	「複数のレイアウトを切り替えること」に特化したクラス。
	 シングルトンでないので、管理クラスとしてレイアウトのグループ単位でマネージャを生成してレイアウトを登録しておけば、
	容易に内包するレイアウトが重複しないように切り替えて表示することができる
  
  
  
 
	UIは基本的に
	・「A」が出てる間は「B」が出ない
	・「B」が出てくる前に「A」が引っ込んで「B」が出てくる
	
	∴何も表示しない場合を、「何も表示しないものが出ている」と置き換えると、
		『UIのレイアウトは切り替えだけで動いている』
		『レイアウトは特定のグループ内で切り替わり、ほかのグループに対しては影響しない』
	
	
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
	@brief	レイアウト単体管理クラス
*/
//----------------------------------------------------------------------------
public class LayoutSwitchObj
{
    public bool m_LayoutInstantiate = false;    //!< レイアウト単体情報：複製で生まれたか否か
    public GameObject m_LayoutGameObj = null;       //!< レイアウト単体情報：ゲームオブジェクト
    public LayoutSwitchSeq m_LayoutSwitchSeq = null;        //!< レイアウト単体情報：処理シーケンス
    public int m_LayoutID = -1;     //!< レイアウト単体情報：ID

    public Type m_OriginComponent = null;       //!< 複製元情報：オリジナルオブジェクト
    public GameObject m_OriginObject = null;        //!< 複製元情報：接続コンポーネント
    public string m_OriginObjectPath = null;        //!< 複製元情報：接続コンポーネントパス
    public GameObject m_PrentObject = null;		//!< 複製元情報：接続先親オブジェクト
}

//----------------------------------------------------------------------------
/*!
	@brief	レイアウト切り替え管理クラス
*/
//----------------------------------------------------------------------------
[System.Serializable]
public class LayoutSwitchManager : MonoBehaviour
{
    public enum SwitchState
    {
        REQUEST_CHK = 0,                    //!< 切り替え状態：リクエスト判断
        OLD_FADE_OUT_BEFORE,                //!< 切り替え状態：旧ページフェードアウト前処理
        OLD_FADE_OUT,                       //!< 切り替え状態：旧ページフェードアウト
        OLD_FADE_OUT_AFTER,                 //!< 切り替え状態：旧ページフェードアウト後処理
        NEW_FADE_IN_BEFORE,                 //!< 切り替え状態：新ページフェードイン前処理
        NEW_FADE_IN,                        //!< 切り替え状態：新ページフェードイン
        NEW_FADE_IN_AFTER,                  //!< 切り替え状態：新ページフェードイン後処理
    };

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public int m_LayoutLayerMask = 0;
    public int m_LayoutId = 0;

    protected TemplateList<LayoutSwitchObj> m_LaytoutList = new TemplateList<LayoutSwitchObj>();        //!< レイアウトリスト

    protected int m_SwitchRequestInput = 0;                                             //!< ページ切り替え情報：リクエスト情報：入力番号
    protected int m_SwitchRequestID = -1;                                               //!< ページ切り替え情報：リクエスト情報：リクエスト番号
    protected bool m_SwitchRequestFast = false;                                         //!< ページ切り替え情報：リクエスト情報：即時切替

    [SerializeField]
    protected SwitchState m_WorkSwitchState = SwitchState.REQUEST_CHK;                          //!< ページ切り替え作業情報：切り替え状態
    protected bool m_WorkSwitchTriger = false;                                          //!< ページ切り替え作業情報：切り替え状態変化直後トリガー
    protected int m_WorkSwitchFinish = 0;                                               //!< ページ切り替え作業情報：リクエスト情報：アクセス番号
    protected int m_WorkSwitchAccess = 0;                                               //!< ページ切り替え作業情報：リクエスト情報：アクセス番号
    protected int m_WorkSwitchID = 0;                                               //!< ページ切り替え作業情報：リクエスト情報：アクセス番号
    protected bool m_WorkSwitchFast = false;                                            //!< ページ切り替え作業情報：リクエスト情報：即時切替
    protected int m_WorkSwitchPagePrev = -1;                                                //!< ページ切り替え作業情報：最後に開いてたページ
    protected int m_WorkSwitchPageNow = -1;                                             //!< ページ切り替え作業情報：現在開いているページ

    public bool m_LayoutStartOK = false;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※インスタンス生成時呼出し
	*/
    //----------------------------------------------------------------------------
    protected virtual void Awake()
    {
        //--------------------------------
        // レイヤーを設定
        //--------------------------------
        if (m_LayoutLayerMask == 0)
        {
            m_LayoutLayerMask = LayerMask.NameToLayer("GUI");
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected virtual void Start()
    {

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：インスタンス制御関連：インスタンス破棄時に呼出し
	*/
    //----------------------------------------------------------------------------
    protected virtual void OnDestroy()
    {
        //--------------------------------
        // オブジェクトを全破棄
        //--------------------------------
        for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
        {
            if (m_LaytoutList[i] == null)
            {
                continue;
            }

            //--------------------------------
            // 複製か否かで破棄処理分岐
            //--------------------------------
            if (m_LaytoutList[i].m_LayoutInstantiate == false)
            {
                //--------------------------------
                // 複製でないなら破棄しない。
                // 
                // 後付けで追加したコンポーネントだけを破棄する
                //--------------------------------
                DestroyImmediate(m_LaytoutList[i].m_LayoutSwitchSeq, true);
            }
            else
            {
                //--------------------------------
                // 複製ならオブジェクトごと破棄
                //--------------------------------
                Destroy(m_LaytoutList[i].m_LayoutGameObj);
            }

            m_LaytoutList[i].m_LayoutGameObj = null;
        }
        m_LaytoutList.Release();
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	
	*/
    //----------------------------------------------------------------------------
    public void SetLayoutStartOK()
    {
        if (m_LayoutStartOK == true)
        {
            return;
        }
        m_LayoutStartOK = true;

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void Update()
    {
        if (SceneCommon.Instance == null
        || SceneCommon.Instance.IsLoadingScene == true)
        {
            return;
        }

        //--------------------------------
        // レイアウト更新処理
        //--------------------------------
        LayoutUpdate();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト更新
		@note	Update関数 , AddRequest で呼ばれる可能性がある
	*/
    //----------------------------------------------------------------------------
    public void LayoutUpdate()
    {
        //--------------------------------
        // 準備前なら処理しない
        // 毎フレチェックする意味は無いのでシーケンス分けるとかで対応する？
        //--------------------------------
        if (m_LayoutStartOK == false)
        {
            return;
        }

        //--------------------------------
        // ステップで処理分岐
        //--------------------------------
        bool bStepFinish = false;
        switch (m_WorkSwitchState)
        {
            case SwitchState.REQUEST_CHK: bStepFinish = LayoutSwitchSeqRequestChk(); break; // 切り替え状態：リクエスト判断
            case SwitchState.OLD_FADE_OUT_BEFORE: bStepFinish = LayoutSwitchSeqOldFadeOutBefore(); break;   // 切り替え状態：旧ページフェードアウト前処理
            case SwitchState.OLD_FADE_OUT: bStepFinish = LayoutSwitchSeqOldFadeOut(); break;    // 切り替え状態：旧ページフェードアウト
            case SwitchState.OLD_FADE_OUT_AFTER: bStepFinish = LayoutSwitchSeqOldFadeOutAfter(); break; // 切り替え状態：旧ページフェードアウト後処理
            case SwitchState.NEW_FADE_IN_BEFORE: bStepFinish = LayoutSwitchSeqNewFadeInBefore(); break; // 切り替え状態：新ページフェードイン前処理
            case SwitchState.NEW_FADE_IN: bStepFinish = LayoutSwitchSeqNewFadeIn(); break;  // 切り替え状態：新ページフェードイン
            case SwitchState.NEW_FADE_IN_AFTER: bStepFinish = LayoutSwitchSeqNewFadeInAfter(); break;   // 切り替え状態：新ページフェードイン後処理
        }
        if (bStepFinish == true)
        {
            //--------------------------------
            // 処理完遂したら次のステップへ
            //--------------------------------
            m_WorkSwitchTriger = true;
            switch (m_WorkSwitchState)
            {
                case SwitchState.REQUEST_CHK: m_WorkSwitchState = SwitchState.OLD_FADE_OUT_BEFORE; break;   // 切り替え状態：リクエスト判断
                case SwitchState.OLD_FADE_OUT_BEFORE: m_WorkSwitchState = SwitchState.OLD_FADE_OUT; break;  // 切り替え状態：旧ページフェードアウト前処理
                case SwitchState.OLD_FADE_OUT: m_WorkSwitchState = SwitchState.OLD_FADE_OUT_AFTER; break;   // 切り替え状態：旧ページフェードアウト
                case SwitchState.OLD_FADE_OUT_AFTER: m_WorkSwitchState = SwitchState.NEW_FADE_IN_BEFORE; break; // 切り替え状態：旧ページフェードアウト後処理
                case SwitchState.NEW_FADE_IN_BEFORE: m_WorkSwitchState = SwitchState.NEW_FADE_IN; break;    // 切り替え状態：新ページフェードイン前処理
                case SwitchState.NEW_FADE_IN: m_WorkSwitchState = SwitchState.NEW_FADE_IN_AFTER; break; // 切り替え状態：新ページフェードイン
                case SwitchState.NEW_FADE_IN_AFTER: m_WorkSwitchState = SwitchState.REQUEST_CHK; break; // 切り替え状態：新ページフェードイン後処理
            }
        }
        else
        {
            m_WorkSwitchTriger = false;
        }

    }



    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え処理：リクエスト有無チェック中
		@note	ここでリクエストを見つけたら古いページのフェードアウトを行う
		@retval[ true	] : 処理完遂
		@retval[ false	] : 処理中
	*/
    //----------------------------------------------------------------------------
    public bool LayoutSwitchSeqRequestChk()
    {
        //		Debug.LogError( gameObject.name );

        //--------------------------------
        // ページ切り替え待機中
        // 切り替え処理を実行していないので、次のリクエストがないかチェック。
        // 
        // 無いなら次のフレームもチェック処理を行うため処理中を返す。
        //--------------------------------
        if (m_WorkSwitchAccess == m_SwitchRequestInput)
        {
            return false;
        }

        //--------------------------------
        // 現在開いているページを設定
        // ※フェード中は例外コード
        //--------------------------------
        m_WorkSwitchPagePrev = m_WorkSwitchID;
        m_WorkSwitchPageNow = -1;

        //--------------------------------
        // 入力情報を保持
        //--------------------------------
        m_WorkSwitchAccess = m_SwitchRequestInput;
        m_WorkSwitchID = m_SwitchRequestID;
        m_WorkSwitchFast = m_SwitchRequestFast;

        //		Debug.LogError( "aaaaaaaaaaaaaa" + gameObject.name );

        //--------------------------------
        // リクエストがあるらしいので処理開始
        //--------------------------------
        return true;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え処理：古いページのフェードアウト前イベント
		@retval[ true	] : 処理完遂
		@retval[ false	] : 処理中
	*/
    //----------------------------------------------------------------------------
    public bool LayoutSwitchSeqOldFadeOutBefore()
    {
        //--------------------------------
        // ページ閉じ前のイベント実行
        //--------------------------------
        bool bEventPlaying = false;
        LayoutSwitchObj cPrevLayoutObj = GetLayoutSwitchObj(m_WorkSwitchPagePrev);
        if (cPrevLayoutObj != null
        && cPrevLayoutObj.m_LayoutSwitchSeq != null
        )
        {
            bool bRet = cPrevLayoutObj.m_LayoutSwitchSeq.LayoutSwitchEventDisableBefore();
            if (bRet == true)
            {
                bEventPlaying = true;
            }
        }
        if (bEventPlaying == true)
        {
            return false;
        }

        //		Debug.LogError( "bbbbbbbbbbbbbb" + gameObject.name );
        //--------------------------------
        // 処理完遂
        //--------------------------------
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え処理：古いページがフェードアウト処理中
		@retval[ true	] : 処理完遂
		@retval[ false	] : 処理中
	*/
    //----------------------------------------------------------------------------
    public bool LayoutSwitchSeqOldFadeOut()
    {
        //--------------------------------
        // トリガーでフェードアウト再生指示
        //--------------------------------
        if (m_WorkSwitchTriger == true)
        {
            //--------------------------------
            // 切り替えリクエストを受理。
            // 一旦全てのページをフェードアウトさせ、
            // フェードアウト後にリクエストページのフェードインを実行することを期待する
            //--------------------------------
            for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
            {
                if (m_LaytoutList[i] == null
                || m_LaytoutList[i].m_LayoutGameObj == null
                || m_LaytoutList[i].m_LayoutSwitchSeq == null
                ) continue;

                if (UnityUtil.ChkObjectEnabled(m_LaytoutList[i].m_LayoutGameObj) == false)
                    continue;

                m_LaytoutList[i].m_LayoutSwitchSeq.LayoutSwitchTriger(false, m_WorkSwitchFast);
                m_LaytoutList[i].m_LayoutSwitchSeq.m_LayoutExecOK = false;
            }
        }

        //--------------------------------
        // フェード処理の完遂待ち
        //--------------------------------
        bool bSwitchFinish = true;
        for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
        {
            if (m_LaytoutList[i] == null
            || m_LaytoutList[i].m_LayoutSwitchSeq == null
            ) continue;

            if (UnityUtil.ChkObjectEnabled(m_LaytoutList[i].m_LayoutGameObj) == false)
                continue;

            bSwitchFinish &= m_LaytoutList[i].m_LayoutSwitchSeq.LayoutSwitchFinishCheck();
        }
        if (bSwitchFinish == false)
        {
            return false;
        }

        //		Debug.LogError( "cccccccccccccccc" + gameObject.name );
        //--------------------------------
        // 処理完遂
        //--------------------------------
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え処理：古いページのフェードアウト後イベント
		@note	この時点では画面上にページが一切表示されていない
		@retval[ true	] : 処理完遂
		@retval[ false	] : 処理中
	*/
    //----------------------------------------------------------------------------
    public bool LayoutSwitchSeqOldFadeOutAfter()
    {
        //--------------------------------
        // ページ閉じ後のイベント実行
        //--------------------------------
        bool bEventPlaying = false;
        LayoutSwitchObj cPrevLayoutObj = GetLayoutSwitchObj(m_WorkSwitchPagePrev);
        if (cPrevLayoutObj != null
        && cPrevLayoutObj.m_LayoutSwitchSeq != null
        )
        {
            bool bRet = cPrevLayoutObj.m_LayoutSwitchSeq.LayoutSwitchEventDisableAfter();
            if (bRet == true)
            {
                bEventPlaying = true;
            }
        }
        if (bEventPlaying == true)
        {
            return false;
        }

        //--------------------------------
        // ページ閉じ後のイベントが完遂したら次へ
        //--------------------------------
        return true;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え処理：新しいページのフェードイン前イベント
		@note	この時点では画面上にページが一切表示されていない
		@retval[ true	] : 処理完遂
		@retval[ false	] : 処理中
	*/
    //----------------------------------------------------------------------------
    public bool LayoutSwitchSeqNewFadeInBefore()
    {
        //--------------------------------
        // ページ開き前のイベント実行
        //--------------------------------
        bool bEventPlaying = false;
        LayoutSwitchObj cNewLayoutObj = GetLayoutSwitchObj(m_WorkSwitchID);
        if (cNewLayoutObj != null)
        {
            //--------------------------------
            // 新しいページのオブジェクト存在チェック。
            // ページの実体がまだ作られていないなら実体作成
            //--------------------------------
            if (cNewLayoutObj.m_LayoutGameObj == null)
            {
                LayoutInstantiate(m_WorkSwitchID);
                return false;
            }
            if (cNewLayoutObj.m_LayoutSwitchSeq != null
            && cNewLayoutObj.m_LayoutSwitchSeq.m_LayoutStartOK == false
            )
            {
                if (UnityUtil.ChkObjectEnabled(cNewLayoutObj.m_LayoutGameObj) == false)
                {
                    UnityUtil.SetObjectEnabled(cNewLayoutObj.m_LayoutGameObj, true);
                }
                return false;
            }

            //--------------------------------
            // 
            //--------------------------------
            if (cNewLayoutObj.m_LayoutSwitchSeq != null)
            {
                bool bRet = cNewLayoutObj.m_LayoutSwitchSeq.LayoutSwitchEventEnableBefore();
                if (bRet == true)
                {
                    bEventPlaying = true;
                }
            }
        }

        if (bEventPlaying == true)
        {
            return false;
        }

        //--------------------------------
        // 処理完遂
        //--------------------------------
        return true;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え処理：新しいページのフェードイン処理中
		@retval[ true	] : 処理完遂
		@retval[ false	] : 処理中
	*/
    //----------------------------------------------------------------------------
    public bool LayoutSwitchSeqNewFadeIn()
    {
        //--------------------------------
        // 開く対象の場合には有効化操作呼出し
        //--------------------------------
        if (m_WorkSwitchTriger == true)
        {
            LayoutSwitchObj cNewLayoutObj = GetLayoutSwitchObj(m_WorkSwitchID);
            if (cNewLayoutObj != null
            && cNewLayoutObj.m_LayoutSwitchSeq != null
            )
            {
                //--------------------------------
                // 開く対象の場合には有効化操作呼出し
                //--------------------------------
                cNewLayoutObj.m_LayoutSwitchSeq.LayoutSwitchTriger(true, m_WorkSwitchFast);
                cNewLayoutObj.m_LayoutSwitchSeq.m_LayoutExecOK = true;
            }

            //--------------------------------
            // 現在開いているページを設定
            //--------------------------------
            m_WorkSwitchPageNow = m_WorkSwitchID;
            m_LayoutId = m_WorkSwitchID;

            //--------------------------------
            // 初期化中のレイアウトを見られたくないので
            // 一時的に表示しないレイヤーに設定している。
            //
            // そのままだと表示されないのでGUIレイヤーに書き換えて描画有効化
            //--------------------------------
            if (cNewLayoutObj != null
            && cNewLayoutObj.m_LayoutGameObj != null
            && cNewLayoutObj.m_LayoutGameObj.layer != m_LayoutLayerMask
            )
            {
                UnityUtil.SetObjectLayer(cNewLayoutObj.m_LayoutGameObj, m_LayoutLayerMask);
            }

        }



        //--------------------------------
        // フェード処理の完遂待ち
        //--------------------------------
        bool bSwitchFinish = true;
        for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
        {
            if (m_LaytoutList[i] == null
            || m_LaytoutList[i].m_LayoutSwitchSeq == null
            || m_LaytoutList[i].m_LayoutGameObj == null
            ) continue;

            if (UnityUtil.ChkObjectEnabled(m_LaytoutList[i].m_LayoutGameObj) == false)
                continue;

            bSwitchFinish &= m_LaytoutList[i].m_LayoutSwitchSeq.LayoutSwitchFinishCheck();
        }
        if (bSwitchFinish == false)
        {
            return false;
        }

        //--------------------------------
        // 処理完遂
        //--------------------------------
        return true;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ページ切り替え処理：新しいページのフェードイン後イベント
		@retval[ true	] : 処理完遂
		@retval[ false	] : 処理中
	*/
    //----------------------------------------------------------------------------
    public bool LayoutSwitchSeqNewFadeInAfter()
    {
        //--------------------------------
        // ページ開き後のイベント実行
        //--------------------------------
        bool bEventPlaying = false;
        LayoutSwitchObj cNewLayoutObj = GetLayoutSwitchObj(m_WorkSwitchPageNow);
        if (cNewLayoutObj != null
        && cNewLayoutObj.m_LayoutGameObj != null
        && cNewLayoutObj.m_LayoutSwitchSeq != null
        )
        {
            bool bRet = cNewLayoutObj.m_LayoutSwitchSeq.LayoutSwitchEventEnableAfter();
            if (bRet == true)
            {
                bEventPlaying = true;
            }
        }
        if (bEventPlaying == true)
        {
            return false;
        }

        //--------------------------------
        // ページ閉じ後のイベントが完遂したら次へ
        //--------------------------------
        m_WorkSwitchFinish = m_WorkSwitchAccess;
        return true;
    }







    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト初期化登録：レイアウト最大数登録
		@param	int			( nLayoutMax		)	: レイアウト最大数
		@retval[ true	]	: 登録成立
		@retval[ true	]	: 登録不成立
	*/
    //----------------------------------------------------------------------------
    public bool SetLayoutMax(int nLayoutMax)
    {
        //--------------------------------
        // オブジェクトを全破棄
        //--------------------------------
        {
            for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
            {
                if (m_LaytoutList[i] == null)
                {
                    continue;
                }

                //--------------------------------
                // 複製か否かで破棄処理分岐
                //--------------------------------
                if (m_LaytoutList[i].m_LayoutInstantiate == false)
                {
                    //--------------------------------
                    // 複製でないなら破棄しない。
                    // 
                    // 後付けで追加したコンポーネントだけを破棄する
                    //--------------------------------
                    DestroyImmediate(m_LaytoutList[i].m_LayoutSwitchSeq, true);
                }
                else
                {
                    //--------------------------------
                    // 複製ならオブジェクトごと破棄
                    //--------------------------------
                    Destroy(m_LaytoutList[i].m_LayoutGameObj);
                }

                m_LaytoutList[i].m_LayoutGameObj = null;
            }
            m_LaytoutList.Release();
        }


        //--------------------------------
        // リストを再構築
        //--------------------------------
        m_LaytoutList.Alloc(nLayoutMax);
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト初期化登録：レイアウト追加
		@param	int				( nLayoutID			)	: レイアウトID
		@param	GameObject		( cOriginObj		)	: レイアウトに登録するオブジェクト
		@param	bool			( bInstantiate		)	: レイアウトに登録するオブジェクトの複製フラグ
		@param	GameObject		( cParentObj		)	: レイアウトの親になるオブジェクト
		@param	Type			( cComponentType	)	: レイアウトに接続するコンポーネント（LayoutSwitchSeq継承限定）
	*/
    //----------------------------------------------------------------------------
    public bool SetLayoutSwitch(int nLayoutID, GameObject cOriginObj, bool bInstantiate, GameObject cParentObj, Type cComponentType)
    {
        //--------------------------------
        // エラーチェック
        //--------------------------------
        if (cOriginObj == null)
        {
            Debug.LogError("Layout Add Error! - " + nLayoutID);
            return false;
        }

        //--------------------------------
        // 同じレイアウトIDのオブジェクトがすでに登録されてないかチェック
        //--------------------------------
        for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
        {
            if (m_LaytoutList[i] == null
            || m_LaytoutList[i].m_LayoutID != nLayoutID
            ) continue;

            Debug.LogError("LayoutID Error! - " + nLayoutID);
            return false;
        }


        //--------------------------------
        // 
        //--------------------------------
        LayoutSwitchObj cLayoutSwitchObj = new LayoutSwitchObj();
        if (bInstantiate == true)
        {
            //--------------------------------
            // 複製指定の場合はオブジェクトを複製する
            // 
            // 起動時間短縮のため、複製の場合はその場で複製せず、表示が有効になった瞬間に実体を生成する
            //--------------------------------
            cLayoutSwitchObj.m_PrentObject = cParentObj;
            cLayoutSwitchObj.m_OriginComponent = cComponentType;
            cLayoutSwitchObj.m_OriginObject = cOriginObj;
            cLayoutSwitchObj.m_OriginObjectPath = "";
            cLayoutSwitchObj.m_LayoutGameObj = null;
            cLayoutSwitchObj.m_LayoutSwitchSeq = null;
            //			cLayoutSwitchObj.m_LayoutGameObj		= Instantiate( cOriginObj ) as GameObject;
            //			cLayoutSwitchObj.m_LayoutSwitchSeq		= cLayoutSwitchObj.m_LayoutGameObj.AddComponent( cComponentType.Name ) as LayoutSwitchSeq;
            cLayoutSwitchObj.m_LayoutID = nLayoutID;
            cLayoutSwitchObj.m_LayoutInstantiate = true;
        }
        else
        {
            //--------------------------------
            // 複製しない場合はオブジェクト自体を追加登録する
            //--------------------------------
            cLayoutSwitchObj.m_PrentObject = cParentObj;
            cLayoutSwitchObj.m_OriginComponent = null;
            cLayoutSwitchObj.m_OriginObject = null;
            cLayoutSwitchObj.m_OriginObjectPath = "";
            cLayoutSwitchObj.m_LayoutGameObj = cOriginObj;
            cLayoutSwitchObj.m_LayoutSwitchSeq = cLayoutSwitchObj.m_LayoutGameObj.AddComponent(cComponentType) as LayoutSwitchSeq;
            cLayoutSwitchObj.m_LayoutID = nLayoutID;
            cLayoutSwitchObj.m_LayoutInstantiate = false;


            //--------------------------------
            // オブジェクトの親としてNGUI階層を設定
            //--------------------------------
            if (cParentObj != null)
            {
                Vector3 vLocalPos = cLayoutSwitchObj.m_LayoutGameObj.transform.localPosition;
                Vector3 vLocalScale = cLayoutSwitchObj.m_LayoutGameObj.transform.localScale;

                cLayoutSwitchObj.m_LayoutGameObj.transform.SetParent(cParentObj.transform, false);
                //cLayoutSwitchObj.m_LayoutGameObj.transform.parent			= cParentObj.transform;
                //cLayoutSwitchObj.m_LayoutGameObj.transform.localPosition	= vLocalPos;
                //cLayoutSwitchObj.m_LayoutGameObj.transform.localScale		= vLocalScale;
            }

            //--------------------------------
            // 初期化中のレイアウトを見られたくないので
            // 一時的に表示しないレイヤーに設定する
            //--------------------------------
            UnityUtil.SetObjectLayer(cLayoutSwitchObj.m_LayoutGameObj, LayerMask.NameToLayer("DRAW_CLIP"));
        }

        /*
                //--------------------------------
                // 有効化しないとStart関数まで行きつかないらしい。
                // 黒フェードで隠すことを想定して最初は全て有効化する
                //--------------------------------
                UnityUtil.SetObjectEnabledFix( cLayoutSwitchObj.m_LayoutGameObj , true );
        */
        //--------------------------------
        // オブジェクトを登録
        //--------------------------------
        m_LaytoutList.Add(cLayoutSwitchObj);
        return true;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト初期化登録：レイアウト追加
		@param	int				( nLayoutID			)	: レイアウトID
		@param	GameObject		( cOriginObj		)	: レイアウトに登録するオブジェクト
		@param	bool			( bInstantiate		)	: レイアウトに登録するオブジェクトの複製フラグ
		@param	GameObject		( cParentObj		)	: レイアウトの親になるオブジェクト
		@param	Type			( cComponentType	)	: レイアウトに接続するコンポーネント（LayoutSwitchSeq継承限定）
	*/
    //----------------------------------------------------------------------------
    public bool SetLayoutSwitchFromName(int nLayoutID, string strOriginObjPath, bool bInstantiate, GameObject cParentObj, Type cComponentType)
    {
        //--------------------------------
        // 同じレイアウトIDのオブジェクトがすでに登録されてないかチェック
        //--------------------------------
        for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
        {
            if (m_LaytoutList[i] == null
            || m_LaytoutList[i].m_LayoutID != nLayoutID
            ) continue;

            Debug.LogError("LayoutID Error! - " + nLayoutID);
            return false;
        }

        //--------------------------------
        // 
        //--------------------------------
        LayoutSwitchObj cLayoutSwitchObj = new LayoutSwitchObj();
        if (bInstantiate == true)
        {
            //--------------------------------
            // 複製指定の場合はオブジェクトを複製する
            // 
            // 起動時間短縮のため、複製の場合はその場で複製せず、表示が有効になった瞬間に実体を生成する
            //--------------------------------
            cLayoutSwitchObj.m_PrentObject = cParentObj;
            cLayoutSwitchObj.m_OriginComponent = cComponentType;
            cLayoutSwitchObj.m_OriginObject = null;
            cLayoutSwitchObj.m_OriginObjectPath = strOriginObjPath;
            cLayoutSwitchObj.m_LayoutGameObj = null;
            cLayoutSwitchObj.m_LayoutSwitchSeq = null;
            //			cLayoutSwitchObj.m_LayoutGameObj		= Instantiate( cOriginObj ) as GameObject;
            //			cLayoutSwitchObj.m_LayoutSwitchSeq		= cLayoutSwitchObj.m_LayoutGameObj.AddComponent( cComponentType.Name ) as LayoutSwitchSeq;
            cLayoutSwitchObj.m_LayoutID = nLayoutID;
            cLayoutSwitchObj.m_LayoutInstantiate = true;
        }
        else
        {
            //--------------------------------
            // 名前指定の場合には複製タイプしか認めない
            //--------------------------------
            Debug.LogError("NameType Is Instantiate Only!!");
            return false;
        }

        //--------------------------------
        // オブジェクトを登録
        //--------------------------------
        m_LaytoutList.Add(cLayoutSwitchObj);
        return true;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ページの読み込み処理
		@note	シーン読み込み直後に全ページを一括で読み込むと待ち時間が長くなるので、必要なタイミングで読み込むように対応
		@retval[ true	]	:ページの準備完了
		@retval[ false	]	:ページの見た目複製と初期化中
	*/
    //----------------------------------------------------------------------------
    private bool LayoutInstantiate(int nLayoutID)
    {
        //--------------------------------
        // インスタンスを生成してないなら生成実行
        // 
        // オリジナルオブジェクト(Prefab)を複製して実体化する
        //--------------------------------
        LayoutSwitchObj cLayoutObj = GetLayoutSwitchObj(nLayoutID);
        if (cLayoutObj == null)
        {
            Debug.LogError("LayoutObject None! - " + nLayoutID);
            return false;
        }

        {
            //--------------------------------
            // オリジナルオブジェクトのインスタンスがまだ読まれてないなら読み込んで適用
            //--------------------------------
            if (cLayoutObj.m_LayoutGameObj == null
            && cLayoutObj.m_OriginObject == null
            && cLayoutObj.m_OriginObjectPath.Length > 0
            )
            {
#if BUILD_TYPE_DEBUG
                //				Debug.Log( "Create Layout - " + cLayoutObj.m_OriginObjectPath + " , " + nLayoutID );
#endif
                cLayoutObj.m_OriginObject = Resources.Load(cLayoutObj.m_OriginObjectPath) as GameObject;
            }

            //--------------------------------
            // オブジェクト複製
            //--------------------------------
            if (cLayoutObj.m_LayoutGameObj == null
            && cLayoutObj.m_OriginObject != null
            )
            {
                //--------------------------------
                // 複製フラグが立ってるなら複製、立ってないならそのまま実体参照
                //--------------------------------
                if (cLayoutObj.m_LayoutInstantiate == true)
                {
                    cLayoutObj.m_LayoutGameObj = Instantiate(cLayoutObj.m_OriginObject) as GameObject;
                }
                else
                {
                    cLayoutObj.m_LayoutGameObj = cLayoutObj.m_OriginObject;
                }

                //--------------------------------
                // 
                //--------------------------------
                if (cLayoutObj.m_LayoutGameObj != null)
                {
                    //--------------------------------
                    // オブジェクトの親としてNGUI階層を設定
                    //--------------------------------
                    if (cLayoutObj.m_PrentObject != null)
                    {
                        cLayoutObj.m_LayoutGameObj.transform.SetParent(cLayoutObj.m_PrentObject.transform, false);
                        //cLayoutObj.m_LayoutGameObj.transform.parent = cLayoutObj.m_PrentObject.transform;
                        //cLayoutObj.m_LayoutGameObj.transform.localPosition	= cLayoutObj.m_OriginObject.transform.localPosition;
                        //cLayoutObj.m_LayoutGameObj.transform.localScale		= cLayoutObj.m_OriginObject.transform.localScale;
                    }
                    else
                    {
                        Debug.LogError("Layout Parent Is Null!");
                    }

                    //--------------------------------
                    // 初期化中のレイアウトを見られたくないので
                    // 一時的に表示しないレイヤーに設定する
                    //--------------------------------
                    UnityUtil.SetObjectLayer(cLayoutObj.m_LayoutGameObj, LayerMask.NameToLayer("DRAW_CLIP"));

                    //--------------------------------
                    // 有効化しないとStart関数まで行きつかないらしい。
                    // 
                    // 表示レイヤーが違うことで準備中のものが描画されることはないので一旦無条件で有効化しておく
                    //--------------------------------
                    UnityUtil.SetObjectEnabledFix(cLayoutObj.m_LayoutGameObj, true);


                    //					Debug.LogError( "Layout Instantiate - " + cLayoutObj.m_LayoutGameObj.name );
                }
                else
                {
                    Debug.LogError("LayoutGameObj Error!");
                }

            }

            if (cLayoutObj.m_LayoutSwitchSeq == null
            && cLayoutObj.m_LayoutGameObj != null
            )
            {
                cLayoutObj.m_LayoutSwitchSeq = cLayoutObj.m_LayoutGameObj.AddComponent(cLayoutObj.m_OriginComponent) as LayoutSwitchSeq;
                cLayoutObj.m_LayoutSwitchSeq.m_LayoutSwitchManager = this;
                cLayoutObj.m_LayoutGameObj.name = cLayoutObj.m_OriginComponent.ToString();
            }
        }

        //--------------------------------
        // 
        //--------------------------------
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト全無効化
		@note	最初に一部レイアウトが見えてしまうことがあるので、強制無効化対応
	*/
    //----------------------------------------------------------------------------
    public void SetLayoutAllDisable()
    {
        for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
        {
            UnityUtil.SetObjectEnabled(m_LaytoutList[i].m_LayoutGameObj, false);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウトオブジェクト取得
		@retval[ LayoutSwitchObj ]	IDに対応したレイアウト
	*/
    //----------------------------------------------------------------------------
    public LayoutSwitchObj GetLayoutSwitchObj(int nID)
    {
        for (int i = 0; i < m_LaytoutList.m_BufferSize; i++)
        {
            if (m_LaytoutList[i] == null
            || m_LaytoutList[i].m_LayoutID != nID
            ) continue;

            return m_LaytoutList[i];
        }
        return null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト切り替えリクエスト関連：リクエスト発行
	*/
    //----------------------------------------------------------------------------
    public bool AddSwitchRequest(int nNextLayout, bool bFast)
    {
        m_SwitchRequestID = nNextLayout;
        m_SwitchRequestFast = bFast;
        m_SwitchRequestInput++;

        //--------------------------------
        // リクエスト発生直後に切り替えが可能ならさっさと切り替える
        //--------------------------------
        LayoutUpdate();

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	カレントなレイアウトIDを取得
		@return int	カレントなレイアウトID
	*/
    //----------------------------------------------------------------------------
    public int GetLayoutCurrent()
    {
        return m_WorkSwitchID;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	レイアウト更新を止めていいか否か
	*/
    //----------------------------------------------------------------------------
    public bool GetLayoutPauseOK()
    {
        if (m_WorkSwitchFinish != m_SwitchRequestInput)
            return false;

        return true;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	メニュー操作禁止状態チェック
		@retval[ true	]	操作禁止状態
		@retval[ false	]	操作可能状態
	*/
    //----------------------------------------------------------------------------
    public bool ChkLayoutRequestNG()
    {
        //--------------------------------
        // シーン切り替え中ならNG
        //--------------------------------
        if (SceneCommon.Instance == null
        || SceneCommon.Instance.IsLoadingScene == true
        )
        {
            return true;
        }

        //--------------------------------
        // ページ切り替え中ならNG
        //--------------------------------
        if (m_WorkSwitchState != SwitchState.REQUEST_CHK)
        {
            return true;
        }

        //--------------------------------
        // ページ切り替えリクエストが存在するならNG
        //--------------------------------
        if (m_SwitchRequestInput != m_WorkSwitchFinish)
        {
            return true;
        }

        return false;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ページアクティブチェック
		@retval[ true	]	アクティブ
		@retval[ false	]	非アクティブ
	*/
    //----------------------------------------------------------------------------
    public bool ChkLayoutActive(int nPageID)
    {
        //--------------------------------
        // リクエストが指定番号のページなら「後にアクティブになるやつ」としてアクティブ判定
        //--------------------------------
        if (m_SwitchRequestID == nPageID)
        {
            return true;
        }

        //--------------------------------
        // エラーチェック
        //--------------------------------
        LayoutSwitchObj cLayoutSwitchObj = GetLayoutSwitchObj(nPageID);
        if (cLayoutSwitchObj == null)
        {
            Debug.LogError("Layout Index Error! - " + nPageID);
            return false;
        }

        //--------------------------------
        // レイアウトの有効無効チェック
        //--------------------------------
        return UnityUtil.ChkObjectEnabled(cLayoutSwitchObj.m_LayoutGameObj);
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション終了チェック
		@retval[ true	]	終了
		@retval[ false	]	再生中
	*/
    //----------------------------------------------------------------------------
    public bool ChkLayoutAnimationFinish(int nPageID)
    {
        //--------------------------------
        // ページ切り替え中なら再生中とみなす
        //--------------------------------
        if (m_WorkSwitchState != SwitchState.REQUEST_CHK)
        {
            return false;
        }

        //--------------------------------
        // ページ切り替えリクエストが存在するなら再生中とみなす
        //--------------------------------
        if (m_SwitchRequestInput != m_WorkSwitchFinish)
        {
            return false;
        }

        //--------------------------------
        // エラーチェック
        //--------------------------------
        LayoutSwitchObj cLayoutSwitchObj = GetLayoutSwitchObj(nPageID);
        if (cLayoutSwitchObj == null)
        {
            Debug.LogError("Layout Index Error! - " + nPageID);
            return true;
        }

        return cLayoutSwitchObj.m_LayoutSwitchSeq.LayoutSwitchFinishCheck();
    }

}

