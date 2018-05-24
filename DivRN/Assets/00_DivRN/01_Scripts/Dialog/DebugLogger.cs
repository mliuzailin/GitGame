/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	DebugLogger.cs
	@brief	デバッグロガー
	@author Developer
	@date 	2013/02/27
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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
	@brief	デバッグロガー
*/
//----------------------------------------------------------------------------
public class DebugLoggerParam
{
    public DateTime cLogInputTime;      //!< ログ発行時間
    public string strLogMessage;        //!< ログメッセージ
    public float fLogInputTime;
};

//----------------------------------------------------------------------------
/*!
	@brief	デバッグロガー
*/
//----------------------------------------------------------------------------
public class DebugLogger : SingletonComponent<DebugLogger>
{
    const int DEF_LOG_MAX = 4;
    const float DELETE_INTERVAL_TIME = 5.0f;

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private DebugLoggerParam[] m_LoggerParamList = null;        //!< ログ情報のリングバッファ
    private int m_LoggerParamInputed = 0;       //!< ログ情報のリングバッファ入力数

    private GameObject m_LoggerObject = null;       //!< 表示オブジェクト
    private TextMeshProUGUI m_LoggerLabel = null;      //!< 表示オブジェクト

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

#if BUILD_TYPE_DEBUG
        //--------------------------------
        // 関連オブジェクトの参照取得
        //--------------------------------
        m_LoggerObject = UnityUtil.GetChildNode(gameObject, "LoggerText");
        if (m_LoggerObject != null)
        {
            m_LoggerLabel = m_LoggerObject.GetComponent<TextMeshProUGUI>();
        }
        if (m_LoggerLabel == null)
        {
            Debug.LogError("Logger Label None!");
        }
#endif
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public void Update()
    {
#if BUILD_TYPE_DEBUG
        //--------------------------------
        // リクエスト発行されるまでスルー
        //--------------------------------
        if (m_LoggerParamList == null
        || m_LoggerParamInputed <= 0
        )
        {
            return;
        }

        //--------------------------------
        // リクエストの履歴から表示テキストを求める
        //--------------------------------
        bool bFirstItem = true;
        string strLogMessage = "";
        for (int i = 0; i < m_LoggerParamList.Length; i++)
        {
            //--------------------------------
            // 最後に入力した要素から辿る
            //--------------------------------
            int nAccess = (m_LoggerParamInputed - 1 - i) % m_LoggerParamList.Length;
            if (nAccess < 0)
            {
                break;
            }
            if (m_LoggerParamList[nAccess] == null)
            {
                break;
            }

            //--------------------------------
            // 経過時間が一定時間を越えたら、
            // 移行の要素は賞味期限切れとして無視
            //--------------------------------
            //			TimeSpan cTimeSpan = TimeManager.Instance.m_TimeNow - m_LoggerParamList[ nAccess ].cLogInputTime;
            //			if( cTimeSpan.TotalSeconds >= DELETE_INTERVAL_TIME )
            m_LoggerParamList[nAccess].fLogInputTime -= Time.deltaTime;
            if (m_LoggerParamList[nAccess].fLogInputTime <= 0)
            {
                m_LoggerParamList[nAccess].fLogInputTime = 0;
                break;
            }

            //--------------------------------
            // 最初の要素だけ白字で表示
            //--------------------------------
            if (bFirstItem == true)
            {
                strLogMessage += "<color=#aaffffff>" + m_LoggerParamList[nAccess].strLogMessage + "</color>" + "\n";
            }
            else
            {
                strLogMessage += m_LoggerParamList[nAccess].strLogMessage + "\n";
            }

            bFirstItem = false;
        }

        //--------------------------------
        // 求めた表示テキストを反映
        //--------------------------------
        if (strLogMessage.Length > 0)
        {
            //--------------------------------
            // ログ情報がある場合、
            // テキスト反映して見た目更新
            //--------------------------------
            if (m_LoggerLabel != null)
            {
                m_LoggerLabel.text = strLogMessage;
            }
        }
        else
        {
            //--------------------------------
            // ログが全て消費された場合、
            // オブジェクトを無効化
            //
            // 次のログリクエストまで表示や更新が一切行なわれない
            //--------------------------------
            UnityUtil.SetObjectEnabledOnce(m_LoggerObject, false);
        }

#endif

    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ログ破棄
	*/
    //----------------------------------------------------------------------------
    static public void StatClear()
    {
#if BUILD_TYPE_DEBUG
        if (DebugLogger.Instance == null)
            return;

        DebugLogger.Instance.Clear();
#endif

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ログ破棄
	*/
    //----------------------------------------------------------------------------
    private void Clear()
    {
        m_LoggerParamInputed = 0;

        if (m_LoggerLabel != null)
        {
            m_LoggerLabel.text = " ";
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ログ行追加
	*/
    //----------------------------------------------------------------------------
    static public void StatAdd(string strText)
    {
#if BUILD_TYPE_DEBUG
        if (DebugLogger.Instance == null)
        {
            Debug.LogError("DebugLogger Instance None!");
            return;
        }

        DebugLogger.Instance.Add(strText);
#endif
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	ログ行追加
	*/
    //----------------------------------------------------------------------------
    private void Add(string strText)
    {
#if BUILD_TYPE_DEBUG
        //--------------------------------
        // 文字列を記憶
        //--------------------------------
        if (m_LoggerParamList == null)
        {
            m_LoggerParamList = new DebugLoggerParam[DEF_LOG_MAX];
            for (int i = 0; i < m_LoggerParamList.Length; i++)
            {
                m_LoggerParamList[i] = new DebugLoggerParam();
            }
        }

        int nAccessNum = m_LoggerParamInputed % m_LoggerParamList.Length;

        m_LoggerParamList[nAccessNum].strLogMessage = strText;
        m_LoggerParamList[nAccessNum].cLogInputTime = TimeManager.Instance.m_TimeNow;
        m_LoggerParamList[nAccessNum].fLogInputTime = DELETE_INTERVAL_TIME;
        m_LoggerParamInputed++;

        //--------------------------------
        // オブジェクトを有効化
        // ここで有効化しないとUpdateが走らない
        //--------------------------------
        if (UnityUtil.ChkObjectEnabled(m_LoggerObject) == false)
        {
            UnityUtil.SetObjectEnabled(m_LoggerObject, true);
        }
#endif

    }
}

