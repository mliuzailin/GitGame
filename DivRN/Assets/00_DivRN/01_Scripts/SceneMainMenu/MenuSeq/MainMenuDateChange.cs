/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuSeqDateChange.cs
	@brief	メインメニューシーケンス：特別枠：日時変更呼び出し
	@author Developer
	@date 	2014/01/23
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
	@brief	メインメニューシーケンス：特別枠：日時変更呼び出し
*/
//----------------------------------------------------------------------------
public class MainMenuDateChange : MainMenuSeq
{

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private Dialog m_Dialog = null;
    private bool m_bReturnHome = false;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public new void Update()
    {
        //----------------------------------------
        // 固定処理
        // 管理側からの更新許可やフェード待ち含む。
        //----------------------------------------
        if (PageSwitchUpdate() == false)
        {
            return;
        }

        //----------------------------------------
        // エラーダイアログ処理
        //----------------------------------------
        if (m_Dialog != null)
        {
            if (m_Dialog.PushButton != DialogButtonEventType.NONE)
            {
                m_Dialog.Hide();
                m_Dialog = null;

                //----------------------------------------
                // タイトルへ戻る
                //----------------------------------------
                SceneCommon.Instance.GameToTitle();

                //----------------------------------------
                // SE再生
                //----------------------------------------
                SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            }
            return;
        }

        if(m_bReturnHome)
        {
            if(MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false))
            {
                m_bReturnHome = false;
            }
        }

    }


    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：MainMenuSeq：ページ切り替えにより有効化された際に呼ばれる関数
		@note	ページのレイアウト再構築を兼ねる
	*/
    //----------------------------------------------------------------------------
    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        m_Dialog = null;
        m_bReturnHome = false;

        switch (MainMenuParam.m_DateChangeType)
        {
            case DATE_CHANGE_TYPE.RETURN_TITLE:
                {
                    //--------------------------------
                    // タイトルへ
                    //--------------------------------
                    m_Dialog = DialogManager.Open1B("CHANGE_DAY", "CHANGE_DAY_DETAIL", "common_button1", true, false);
                }
                break;
            case DATE_CHANGE_TYPE.LOGIN:
                {
                    //ローカルに保存してあるWebリソースを全削除
                    WebResource.Instance.RemoveAll();

                    //ログインボーナスシーケンスへ
                    MainMenuManagerFSM.Instance.SendFsmEvent("REQUEST_LOGIN_BONUS", 1.0f);
                }
                break;
            case DATE_CHANGE_TYPE.DAY_STRADDLE:
                {
                    //次の日跨ぎ通信の時間を設定
                    MainMenuParam.m_DayStraddleTime = MainMenuParam.m_DayStraddleTime.AddDays(1);
                    //日またぎ通信
                    ServerDataUtilSend.SendPacketAPI_DayStraddle()
                    .setSuccessAction((data) =>
                    {
                        RecvDayStraddleValue result = data.GetResult<RecvDayStraddle>().result;
                        if (result != null)
                        {
                            //プレイヤー情報更新
                            UserDataAdmin.Instance.m_StructPlayer = data.UpdateStructPlayer<RecvDayStraddle>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                            UserDataAdmin.Instance.ConvertPartyAssing();

                            //ガチャ情報更新
                            if (result.gacha_status != null)
                            {
                                UserDataAdmin.Instance.UpdateGachaStatusList(result.gacha_status);
                            }

                            //Homeへ
                            m_bReturnHome = true;
                        }

                    })
                    .setErrorAction((data) =>
                    {
                        //Homeへ
                        m_bReturnHome = true;
                    })
                    .SendStart();
                }
                break;
            default:
                //ここに来ることはないはず
                //Homeへ
                m_bReturnHome = true;
                break;
        }
    }

}




