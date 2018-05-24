/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreBuyTip.cs
	@brief	ストア画面でのチップ購入
	@author Developer
	@date 	

*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using System;
using UnityEngine;
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
	@brief	ストア画面でのチップ購入
*/
//----------------------------------------------------------------------------
public class StoreBuyTip
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    /// 選択商品
    private StoreProduct m_SelectProduct = null;
    /// 魔法石購入処理：年齢区分別予算上限
    private int m_SelectAgeBudget = 0;
    /// 魔法石購入処理：年齢区分
    private AGE_TYPE m_SelectAgeType = AGE_TYPE.AGE_TYPE_NONE;

    private Action<bool> finishAction;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    //----------------------------------------------------------------------------
    /*!
		@brief	チップ購入処理開始
	*/
    //----------------------------------------------------------------------------
    public void StartProcess(MainMenuShop cMenuSeqShopRoot, Action<bool> _finishAction)
    {

        this.finishAction = _finishAction;

        if (StoreManager.Instance.m_UserStoreEnable)
        {
            //イベント情報取得処理へ
            SendShopProduct();
        }
        else
        {
            // 購入がロックされている
            var cRequestResult = new StoreRequest();
            cRequestResult.m_WorkResult = STORE_RESULT.FINISH_NG_LOCK;
            ExecStepStoreExec(cRequestResult);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	イベント情報取得処理
	*/
    //----------------------------------------------------------------------------
    private void SendShopProduct()
    {
        ServerDataUtilSend.SendPacketAPI_GetStoreProductEvent()
        .setSuccessAction(_data =>
        {
            RecvGetStoreProductEventValue result = _data.GetResult<RecvGetStoreProductEvent>().result;
            UserDataAdmin.Instance.m_StructProductEvent = result.store_product_event;
            UserDataAdmin.Instance.m_StructPlayer.pay_month = result.player.pay_month;

            //----------------------------------------
            // 現在時の状態で、StoreParam内のストア商品一覧を固める
            //----------------------------------------
            StoreParam.SetupStoreProductListNow();

            //アイテム選択へ
            SendStepItemSelect();
        })
        .setErrorAction(_data =>
        {
            finishAction(false);
        })
        .SendStart();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アイテム選択の処理
		@note	
	*/
    //----------------------------------------------------------------------------
    private void SendStepItemSelect()
    {
        DialogManager.OpenShopBuySelect((DialogTipItem item) =>
        {
            //MULTI01 - MULTI06 
#if BUILD_TYPE_DEBUG
            Debug.Log("DialogButtonEventType: " + item.buttonType.ToString());
#endif
            //選択商品
            m_SelectProduct = item.product;

            //最大値確認へ
            SendStepBuyStoneMax();
        },
        () =>
        {
            //OK 終了
            finishAction(false);
        });
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	購入最大値チェック
		@note	
	*/
    //----------------------------------------------------------------------------
    private void SendStepBuyStoneMax()
    {
        // プロダクト獲得
        StoreProduct cProduct = m_SelectProduct;
        // プロダクトの有無
        if (cProduct == null)
        {
            //-----------------------
            // 想定外エラー
            //-----------------------
            DialogManager.Open1B("ERROR_MSG_USER_TITLE", "ERROR_MSG_USER", "common_button7", true, true).
            SetOkEvent(() =>
            {
                SoundUtil.PlaySE(SEID.SE_MENU_RET);
                finishAction(false);
            });
        }
        else
        {
            //-----------------------
            // 商品購入で最大超えしないかチェック
            //-----------------------
            uint unStoneTotal = (uint)cProduct.product_num;
            unStoneTotal += UserDataAdmin.Instance.m_StructPlayer.have_stone_pay;

            if (unStoneTotal > GlobalDefine.VALUE_HAVE_MAX_STONE_PAID)
            {
                //-----------------------
                // 最大超え確定
                // →最大超えなことをダイアログで示してショップルートへ戻る
                //-----------------------
                DialogManager.Open1B("error_response_title46", "error_response_content46", "common_button1", true, true).
                SetOkEvent(() =>
                {
                    SoundUtil.PlaySE(SEID.SE_MENU_RET);
                    finishAction(false);
                });
            }
            else
            {
                //年齢確認へ
                SendStepChkAge();
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	年齢確認処理
		@note	
	*/
    //----------------------------------------------------------------------------
    private void SendStepChkAge()
    {
        //-----------------------
        // 年齢確認ダイアログを表示する
        //-----------------------
        DialogManager.OpenAgeVerification((DialogButtonEventType type) =>
        {
            //--------------------------------
            // 押したボタンで処理分岐
            //
            // ボタンの並びは以下
            // MULTI01: [ 16歳未満	] 
            // MULTI02: [ 16歳～19歳 ]
            // MULTI03: [ 20歳以上	]
            //--------------------------------
            switch (type)
            {
                case DialogButtonEventType.MULTI01:
                    m_SelectAgeBudget = StoreDefine.PAY_BORDER_TYPE1;
                    m_SelectAgeType = AGE_TYPE.AGE_TYPE_00_15;
                    SendStepChkAgeUnder();
                    break;
                case DialogButtonEventType.MULTI02:
                    m_SelectAgeBudget = StoreDefine.PAY_BORDER_TYPE2;
                    m_SelectAgeType = AGE_TYPE.AGE_TYPE_16_19;
                    SendStepChkAgeUnder();
                    break;
                case DialogButtonEventType.MULTI03:
                    m_SelectAgeBudget = StoreDefine.PAY_BORDER_TYPE3;
                    m_SelectAgeType = AGE_TYPE.AGE_TYPE_20_xx;
                    SendStepChkOverBudget();
                    break;
                case DialogButtonEventType.OK:
                    finishAction(false);
                    break;
            }
        },
        () =>
        {
            //OK 終了
            finishAction(false);
        });
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	未成年おゆるし確認の処理
		@note	
	*/
    //----------------------------------------------------------------------------
    private void SendStepChkAgeUnder()
    {
        DialogManager.OpenAgeVerificationUnder(() =>
        {
            //YES
            SendStepChkOverBudget();
        },
        () =>
        {
            //NO
            finishAction(false);
        }).SetStrongYes();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	月間予算超えチェックシーケンスへの移行処理
		@note	
	*/
    //----------------------------------------------------------------------------
    private void SendStepChkOverBudget()
    {
        // プロダクト獲得
        StoreProduct cProduct = m_SelectProduct;
        // プロダクトの有無
        if (cProduct == null)
        {
            //-----------------------
            // 想定外エラー
            //-----------------------
            DialogManager.Open1B("ERROR_MSG_USER_TITLE", "ERROR_MSG_USER", "common_button7", true, true).
            SetOkEvent(() =>
            {
                finishAction(false);
            });
        }
        else
        {
            //-----------------------
            // 商品購入で予算超えしないかチェック
            //-----------------------
            uint unProductPriceTotal = (uint)cProduct.product_price;
            unProductPriceTotal += UserDataAdmin.Instance.m_StructPlayer.pay_month;

            if (m_SelectAgeBudget < unProductPriceTotal)
            {
                //-----------------------
                // 予算超え確定
                // →予算超えなことをダイアログで示してショップルートへ戻る
                //-----------------------
                DialogManager.Open1B("sh120q_title", "sh120q_content", "common_button1", true, true).
                SetOkEvent(() =>
                {
                    finishAction(false);
                });
            }
            else
            {
                Patcher.Instance.IsUpdateMoveTitle(
                    () =>
                    {
                        finishAction(false);
                    },
                    () =>
                    {
                        //-----------------------
                        // 予算内
                        // →購入可能なので購入処理へ
                        //-----------------------

                        SendStepStartRequest();
                    }
                );
            }
        }
    }

    private void SendStepStartRequest()
    {
        //-----------------------
        // 購入リクエストを発行
        //-----------------------
        StoreProduct cProduct = m_SelectProduct;

        StoreManager.Instance.AddRequest(STORE_REQUEST.PAY,
                                                 cProduct.product_id,
                                                 cProduct.event_id,
                                                 m_SelectAgeType,
                                                 cProduct.product_num,
                                                 (cProduct.kind == (uint)StoreProduct.KIND_TYPE.KIND_PRESET));
        StoreManager.Instance.sefFinshAction(ExecStepStoreExec);
        StoreManager.Instance.StartRequest();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	購入シーケンス：ストア処理完了ダイアログ
		@note	
	*/
    //----------------------------------------------------------------------------
    private void ExecStepStoreExec(StoreRequest cRequestResult)
    {
        if (cRequestResult.m_WorkResult == STORE_RESULT.FINISH_OK)
        {
            // 成功時プレゼントの付与があった場合はプレゼントリスト獲得通信を行う
            if (cRequestResult.m_AddPresent == true)
            {
                //-----------------------
                // プレゼントリストリクエストを発行
                //-----------------------
                ServerDataUtilSend.SendPacketAPI_PresentListGet()
                .setSuccessAction(_data =>
                {
                    //----------------------------------------
                    // 情報反映
                    //----------------------------------------
                    var list = UserDataAdmin.PresentListClipTimeLimit(_data.GetResult<RecvPresentListGet>().result.present);
                    UserDataAdmin.Instance.m_StructPresentList = list;

                    //----------------------------------------
                    // 処理終了。
                    //----------------------------------------
                    // プレゼント付きダイアログ表示
                    uint nProductNum = cRequestResult.m_WorkResultAddTip;
                    string strDialogTitle = GameTextUtil.GetText("MM_SHOP_BUY_SUCCESS_TITLE");
                    string strDialogMsg = GameTextUtil.GetText("MM_SHOP_BUY_SUCCESS_ADD_PRESENT");
                    strDialogMsg = string.Format(strDialogMsg, nProductNum, UserDataAdmin.Instance.m_StructPlayer.have_stone);
                    DialogManager.Open1B_Direct(strDialogTitle, strDialogMsg, "common_button7", true, true).
                    SetOkEvent(() =>
                    {
                        finishAction(true);
                    });
                })
                .setErrorAction(_data =>
                {
                    //----------------------------------------
                    // 何らかの不具合により通信失敗したら戻る
                    //----------------------------------------
                    DialogManager.Open1B("ERROR_MSG_WIDEUSE_TITLE", "ERROR_MSG_WIDEUSE", "common_button7", true, true).
                    SetOkEvent(() =>
                    {
                        finishAction(false);
                    });

                    SoundUtil.PlaySE(SEID.SE_MENU_RET);
                })
                .SendStart();
            }
            else
            {
                // 通常購入
                uint nProductNum = cRequestResult.m_WorkResultAddTip;
                string strDialogTitle = GameTextUtil.GetText("sh123q_title");
                string strDialogMsg = GameTextUtil.GetText("sh123q1_content");
                strDialogMsg = string.Format(strDialogMsg, nProductNum, UserDataAdmin.Instance.m_StructPlayer.have_stone);

                DialogManager.Open1B_Direct(strDialogTitle, strDialogMsg, "common_button1", true, true).
                SetOkEvent(() =>
                {
                    finishAction(false);
                });
            }
        }
        else if (cRequestResult.m_WorkResult == STORE_RESULT.FINISH_NG)
        {
            if (cRequestResult.m_PacketCode == API_CODE.API_CODE_Android_QueryInventoryFailed_ERROR ||
                cRequestResult.m_PacketCode == API_CODE.API_CODE_Android_ConsumePurchaseFailed_ERROR ||
                cRequestResult.m_PacketCode == API_CODE.API_CODE_Android_OnPurchaseReceipt_ERROR
                )
            {
                //下記のエラーコード対応のため
                // API_CODE.API_CODE_Android_QueryInventoryFailed_ERROR:
                // API_CODE.API_CODE_Android_ConsumePurchaseFailed_ERROR:
                // API_CODE.API_CODE_Android_OnPurchaseReceipt_ERROR:
                string strDialogTitle = GameTextUtil.GetText("ERROR_MSG_CANCEL_TITLE");
                string strDialogMsg = GameTextUtil.GetText("ERROR_MSG_CANCEL");
                strDialogMsg = string.Format(strDialogMsg, (uint)cRequestResult.m_PacketCode);

                DialogManager.Open1B_Direct(strDialogTitle, strDialogMsg, "common_button7", true, true).
                SetOkEvent(() =>
                {
                    finishAction(false);
                });
            }
            else
            {
                //それ以外のエラーときはServerApiがダイアログを表示しているので終了処理のみ行う
                finishAction(false);
            }
        }
        else if (cRequestResult.m_WorkResult == STORE_RESULT.FINISH_NG_LOCK)
        {
            //-----------------------
            // 課金機能がロックされている
            //-----------------------
            DialogManager.Open1B("ERROR_STORE_LOCK_TITLE", "ERROR_STORE_LOCK", "common_button7", true, true).
            SetOkEvent(() =>
            {
                finishAction(false);
            });
        }
        else if (cRequestResult.m_WorkResult == STORE_RESULT.FINISH_NG_INIT)
        {
            //-----------------------
            // 初期化に失敗している
            //-----------------------
            DialogManager.Open1B("ERROR_PAY_INIT_TITLE", "ERROR_PAY_INIT", "common_button7", true, true).
            SetOkEvent(() =>
            {
                finishAction(false);
            });
        }
        else if (cRequestResult.m_WorkResult == STORE_RESULT.FINISH_CANCEL)
        {
            //-----------------------
            // ユーザー操作でキャンセルされた
            //-----------------------
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
            finishAction(false);
        }
        else if (cRequestResult.m_WorkResult == STORE_RESULT.FINISH_TRANSACTION)
        {
            //-----------------------
            // 前回購入処理が完了していない
            // キャンセルダイアログに乗せてボタンを押したら最初に戻す
            //-----------------------
            DialogManager.Open1B("ERROR_PAY_TRANSACTION_TITLE", "ERROR_PAY_TRANSACTION", "common_button7", true, true).
            SetOkEvent(() =>
            {
                finishAction(false);
            });

        }
        else if (cRequestResult.m_WorkResult == STORE_RESULT.FINISH_RESTORE)
        {
            //-----------------------
            // 購入しようとしたら補填処理が走った
            // キャンセルダイアログに乗せてボタンを押したら最初に戻す
            //-----------------------
            DialogManager.Open1B("ERROR_PAY_RESTORE_TITLE", "ERROR_PAY_RESTORE", "common_button7", true, true).
            SetOkEvent(() =>
            {
                finishAction(false);
            });
        }
        else
        {
            //-----------------------
            // 想定外エラー
            //-----------------------
            DialogManager.Open1B("ERROR_MSG_USER_TITLE", "ERROR_MSG_USER", "common_button7", true, true).
            SetOkEvent(() =>
            {
                finishAction(false);
            });
        }
    }
}
