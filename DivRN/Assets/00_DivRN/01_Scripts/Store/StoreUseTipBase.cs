/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreUseTipBase.cs
	@brief	ストア画面でのチップの共通処理
	@author Developer
	@date 	2015/09/11
 
	新UIへの変更にともない、MainMenuSeqShopUseTipStaminaではなく
	MainMenuSeqShopRoot画面上に出るようになった為追加
	MainMenuSeqShopUseTipStaminaの中身を持ってきて修正する
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using ServerDataDefine;

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
	@brief	ストア画面でベースクラス
*/
//----------------------------------------------------------------------------
public class StoreUseTipBase
{
    //----------------------------------------------------------------------------
    /*!
        @brief	ストア シーケンス処理
    */
    //----------------------------------------------------------------------------
    public enum STORE_TIP_SEQUENCE
    {
        /// シーケンス：処理なし状態
        NONE = 0,
        /// シーケンス：使用意思確認
        CHOICE,
        /// シーケンス：使用リクエスト
        REQUEST,
        /// シーケンス：使用確定
        REQUEST_WAIT,
        /// シーケンス：処理終了
        FINISH,
        /// シーケンス：エラー対応
        ERROR,
        /// シーケンス：チップ購入へ
        NEXT_BUY_TIP,
    };

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    
    /// チップ使用処理：ステップ
    protected STORE_TIP_SEQUENCE m_BuyStep = STORE_TIP_SEQUENCE.NONE;
    /// チップの仕様確認ダイアログ
    protected Dialog m_DialogChoice = null;
    /// 終了処理ダイアログ
    protected Dialog m_FinishDialog = null;

    // @add Developer 2016/04/20 v340 パッチ取得対応
    /// 呼び出し元メニュー
    protected MainMenuShop m_MenuSeqShopRoot = null;

    protected bool m_bNextBuyTip = false;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	処理開始
	*/
    //----------------------------------------------------------------------------
    public virtual void StartProcess(MainMenuShop cMenuSeqShopRoot)
    {
        //--------------------------------
        // ステップがNONE以外の場合は何もしない
        //--------------------------------
        if (m_BuyStep != STORE_TIP_SEQUENCE.NONE)
        {
            return;
        }
        m_DialogChoice = null;

        //--------------------------------
        // パッチ取得用
        // @add Developer 016/04/20 v340
        //--------------------------------
        m_MenuSeqShopRoot = cMenuSeqShopRoot;

        m_FinishDialog = null;

        m_bNextBuyTip = false;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	更新処理：処理中はtrueを返す
		@note	MainMenuSeqShopRoot.Update内で毎回呼び出す
	*/
    //----------------------------------------------------------------------------
    public bool UpdateProcess()
    {
        //-----------------------
        // ステップ依存処理
        //-----------------------
        bool bReturn = false;
        switch (m_BuyStep)
        {
            case STORE_TIP_SEQUENCE.NONE:
                bReturn = ExecStepNone();
                break;
            case STORE_TIP_SEQUENCE.CHOICE:
                bReturn = ExecStepChoice();
                break;
            case STORE_TIP_SEQUENCE.REQUEST:
                bReturn = ExecStepRequest();
                break;
            case STORE_TIP_SEQUENCE.REQUEST_WAIT:
                bReturn = ExecStepRequestWait();
                break;
            case STORE_TIP_SEQUENCE.FINISH:
                bReturn = ExecStepFinish();
                break;
            case STORE_TIP_SEQUENCE.ERROR:
                bReturn = ExecStepError();
                break;
            case STORE_TIP_SEQUENCE.NEXT_BUY_TIP:
                bReturn = ExecStepNextBuyTip();
                break;
        }

        return bReturn;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：処理なし
		@note	常にfalse(=処理していない状態)を返す
	*/
    //----------------------------------------------------------------------------
    private bool ExecStepNone()
    {
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：使用意思確認
		@note	仕様確認ダイアログの結果を見て結果によって終了or使用確定処理を行わせる
	*/
    //----------------------------------------------------------------------------
    private bool ExecStepChoice()
    {
        //----------------------------------------
        // ダイアログボタン操作
        // ダイアログボタン操作が来た場合
        //----------------------------------------
        if (m_DialogChoice != null
        && m_DialogChoice.PushButton != DialogButtonEventType.NONE)
        {
            if (m_DialogChoice.PushButton == DialogButtonEventType.YES)
            {
				//--------------------------------
				// 使用するボタンを押した場合は、パッチをリクエスト
				// @change Developer 016/04/20 v340
				//--------------------------------
				// ServerApi.csに移動
#if false
				if (MainMenuManager.Instance != null)
                {
					MainMenuManager.Instance.RequestPatchUpdate(true);
                }
#endif
                // SEは自前で鳴らす
                //SoundUtil.PlaySE(SEID.SE_MENU_OK);
                // リクエスト処理へ
                m_BuyStep = STORE_TIP_SEQUENCE.REQUEST;
            }
            else
            {
                //----------------------------------------
                // 戻るボタンを押した場合は戻る
                //----------------------------------------
                // SEは自前で鳴らす
                //SoundUtil.PlaySE(SEID.SE_MENU_RET);
                // 処理終了へ
                m_BuyStep = STORE_TIP_SEQUENCE.FINISH;
            }
            // ダイアログ参照外しておく
            m_DialogChoice.Hide();
            m_DialogChoice = null;
        }
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：使用リクエスト
		@note	
		@add	Developer 2016/04/21 v340
	*/
    //----------------------------------------------------------------------------
    protected virtual bool ExecStepRequest()
    {
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：使用確定
		@note	使用した場合の通信待ち及び結果ダイアログ表示を行い、処理を終了する
	*/
    //----------------------------------------------------------------------------
    private bool ExecStepRequestWait()
    {
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：処理終了
		@note	終了ダイアログが無くなっていればそのまま処理終了に遷移、ダイアログがあった場合は終了まで待ち
	*/
    //----------------------------------------------------------------------------
    private bool ExecStepFinish()
    {
        //-----------------------
        // 終了ダイアログがNULLの場合終了させる
        //-----------------------
        if (m_FinishDialog == null)
        {
            m_BuyStep = STORE_TIP_SEQUENCE.NONE;

            if (m_DialogChoice != null)
            {
                m_DialogChoice.Hide();
                m_DialogChoice = null;
            }
        }
        else
        {
            if (m_FinishDialog.PushButton != DialogButtonEventType.NONE)
            {
                // 何かボタンが押されていたらNULLにして処理終了へ
                m_FinishDialog.Hide();
                m_FinishDialog = null;
            }
        }
        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	シーケンス：エラーダイアログ処理
	*/
    //----------------------------------------------------------------------------
    protected bool ExecStepError()
    {
        //----------------------------------------
        // ダイアログボタン操作
        //----------------------------------------
        if (m_DialogChoice != null)
        {
            // ダイアログボタン操作が来た場合
            if (m_DialogChoice.PushButton != DialogButtonEventType.NONE)
            {
                // ダイアログ参照外しておく
                m_DialogChoice.Hide();
                m_DialogChoice = null;
                // ステップを処理なしに変更する
                m_BuyStep = STORE_TIP_SEQUENCE.NONE;
            }
            // ダイアログが居る限り処理中とする
            return true;
        }
        // ダイアログが無ければ処理終了
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected bool ExecStepNextBuyTip()
    {
        //----------------------------------------
        // ダイアログボタン操作
        //----------------------------------------
        if (m_DialogChoice != null)
        {
            // ダイアログボタン操作が来た場合
            if (m_DialogChoice.PushButton != DialogButtonEventType.NONE)
            {
                if (m_DialogChoice.PushButton == DialogButtonEventType.YES)
                {
                    m_bNextBuyTip = true;
                }
                // ダイアログ参照外しておく
                m_DialogChoice.Hide();
                m_DialogChoice = null;
                // ステップを処理なしに変更する
                m_BuyStep = STORE_TIP_SEQUENCE.NONE;
            }
            // ダイアログが居る限り処理中とする
            return true;
        }
        // ダイアログが無ければ処理終了
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsNextBuyTip()
    {
        return m_bNextBuyTip;
    }

	public void ResetBuyStep()
	{
		m_BuyStep = STORE_TIP_SEQUENCE.NONE;
	}
}
