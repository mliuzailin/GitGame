/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	InGameShopManager.cs
	@brief	インゲーム中ショップ関連クラス
	@author Developer
	@date 	2013/06/17
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
using M4u;

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
	@brief	インゲーム中ショップ関連クラス
*/
//----------------------------------------------------------------------------
public class InGameShopManager : SingletonComponent<InGameShopManager>
{
    //----------------------------------------------------------------------------
    /*!
		@brief	ショップ関連処理リザルト
	*/
    //----------------------------------------------------------------------------
    public enum INGAME_SHOP_RESAULT
    {
        RESULT_NONE,        //!< 処理前
        RESULT_WORKING,     //!< 購入処理中
        RESULT_OK,          //!< 購入成立		（意図的に購入処理を行い、魔法石の消費も成立済）
        RESULT_NG,          //!< 購入不成立		（意図的に購入処理を断念。魔法石の消費も不成立）
    };

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public INGAME_SHOP_RESAULT m_ShopResult = INGAME_SHOP_RESAULT.RESULT_NONE;  //!< リザルト
    private uint m_ShopWorkBuyCt = 0;								//!< 作業情報：課金回数カウント
    private StoreBuyTip m_StoreBuyTip = null;                               //!< チップ購入
    private bool m_IsUsedAutoPlay;   //!< オートプレイを使用したかどうか

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	課金フローリクエスト：コンティニュー課金

		@param[in]	uint				( unBuyCt	) 購入回数	※課金した回数をサーバーに送ってストアの処理が重複して発生しないようにするため
		@retval[ true	]	リクエスト成立
		@retval[ false	]	リクエスト不成立
	*/
    //----------------------------------------------------------------------------
    public void RequestShopWorkContinue(uint unBuyCt, bool is_auto_play)
    {
        //--------------------------------
        // 作業中は重複発行はできない。結果待ってからリクエスト投げてもらう
        //--------------------------------
        if (m_ShopResult == INGAME_SHOP_RESAULT.RESULT_WORKING)
        {
            Debug.LogError("Shop Working Error!!");
            return;
        }

        //--------------------------------
        // 状態を初期化。
        // これでUpdate関数が動き始める
        //--------------------------------
        m_ShopResult = INGAME_SHOP_RESAULT.RESULT_WORKING;
        m_ShopWorkBuyCt = unBuyCt;
        m_IsUsedAutoPlay = is_auto_play;

        WorkStepStart();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	処理ステップ：「コンティニュー or リセット しますか？」
	*/
    //----------------------------------------------------------------------------
    private void WorkStepStart()
    {
        //--------------------------------
        // 遷移初回処理
        //--------------------------------
        int nStoneCt = 0;
        if (UserDataAdmin.Instance != null
        && UserDataAdmin.Instance.m_StructPlayer != null
        )
        {
            nStoneCt = (int)UserDataAdmin.Instance.m_StructPlayer.have_stone;
        }

        DialogManager.OpenInGameContinue(nStoneCt).
        SetYesEvent(() =>
        {
            //--------------------------------
            // 肯定確定！
            //--------------------------------

            //--------------------------------
            // 魔法石を持ってるかチェック
            //--------------------------------
            bool bStoneOK = false;
            if (UserDataAdmin.Instance != null
            && UserDataAdmin.Instance.m_StructPlayer != null
            && UserDataAdmin.Instance.m_StructPlayer.have_stone > 0)
            {
                bStoneOK = true;
            }

            if (bStoneOK == true)
            {
                //--------------------------------
                // 魔法石を持ってる
                // 　　→即消費フローへ
                //--------------------------------
                WorkStepShopSendUse();
            }
            else
            {
                //--------------------------------
                // 魔法石を持ってない
                // 　　→魔法石購入フローへ
                //--------------------------------
                WorkStepShopShort();
            }

        }).
        SetNoEvent(() =>
        {
            //キャンセル
            m_ShopResult = INGAME_SHOP_RESAULT.RESULT_NG;
        }).
        DisableBackKey();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	処理ステップ：「魔法石が不足してます。買いますか？」
	*/
    //----------------------------------------------------------------------------
    private void WorkStepShopShort()
    {
        DialogManager.OpenShopShort().
       SetYesEvent(() =>
       {
           //----------------------------------------
           // 例外対応。
           // スペックが低くてストアを使うとハングする可能性の高い端末での課金阻止対応
           //----------------------------------------
           if (SystemUtil.GetStoreOpen() == false)
           {
               DialogManager.Open1B("ERROR_STORE_DISABLE_TITLE", "ERROR_STORE_DISABLE", "common_button7", true, true).
               SetOkEvent(() =>
               {
                   WorkStepStart();
               });
           }
           else
           {
               m_StoreBuyTip = new StoreBuyTip();
               m_StoreBuyTip.StartProcess(null, (bool isComplete) =>
               {
                   if (isComplete)
                   {
                       WorkStepShopSendUse();
                   }
                   else
                   {
                       WorkStepStart();
                   }

                   m_StoreBuyTip = null;
               });
           }

       }).
       SetNoEvent(() =>
       {
           WorkStepStart();
       });
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	処理ステップ：魔法石消費通信（コンティニュー or リセット）
	*/
    //----------------------------------------------------------------------------
    private void WorkStepShopSendUse()
    {
        switch (MasterDataUtil.GetQuestType(BattleParam.m_QuestMissionID))
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {
                    ServerDataUtilSend.SendPacketAPI_QuestContinue(m_ShopWorkBuyCt, m_IsUsedAutoPlay)
                    .setSuccessAction(_data =>
                    {
                        //----------------------------------------
                        // ここまできたら処理完遂。
                        //
                        // 魔法石の消費も終わったのでリザルトを完遂状態にして終了。
                        //----------------------------------------
                        successContinue();
                    })
                    .setErrorAction(data =>
                    {
                        WorkStepStart();
                    })
                    .SendStart();
                }
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    ServerDataUtilSend.SendPacketAPI_ChallengeQuestContinue(m_ShopWorkBuyCt, m_IsUsedAutoPlay)
                    .setSuccessAction(_data =>
                    {
                        //----------------------------------------
                        // ここまできたら処理完遂。
                        //
                        // 魔法石の消費も終わったのでリザルトを完遂状態にして終了。
                        //----------------------------------------
                        successContinue();
                    })
                    .setErrorAction(data =>
                    {
                        WorkStepStart();
                    })
                    .SendStart();
                }
                break;
            default:
                //
                WorkStepStart();
                break;
        }
    }

    private void successContinue()
    {
        m_ShopResult = INGAME_SHOP_RESAULT.RESULT_OK;

        if (UserDataAdmin.Instance.m_StructPlayer.have_stone > 0)
        {
            UserDataAdmin.Instance.m_StructPlayer.have_stone--;
            // ダイアログ情報の為、それぞれの個数も減らす
            // 無料チップがある場合はそちらから消す
            if (UserDataAdmin.Instance.m_StructPlayer.have_stone_free > 0)
            {
                UserDataAdmin.Instance.m_StructPlayer.have_stone_free--;
            }
            else if (UserDataAdmin.Instance.m_StructPlayer.have_stone_pay > 0)
            {
                UserDataAdmin.Instance.m_StructPlayer.have_stone_pay--;
            }
            else
            {
                Debug.LogError("Local Stone Error!");
            }
        }
        else
        {
            Debug.LogError("Local Stone Error!");
        }
    }
}

