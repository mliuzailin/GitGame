/*==========================================================================*/
/*==========================================================================*/
/*!
    @file	SceneGoesParamToQuest2.cs
    @brief	シーン間のパラメータ受け渡しクラス：新探索への受け渡し用
    @author Developer
    @date 	2013/02/08
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
    @brief	受け渡し情報：メインメニュー→ゲームメイン

    @note	クエスト開始情報
*/
//----------------------------------------------------------------------------
public class SceneGoesParamToQuest2
{
    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------

    public uint m_QuestAreaID = 0; //!< クエスト情報：エリアID
    public int m_QuestAreaAmendCoin = 100; //!< クエスト情報：エリア補正倍率：コイン		（％表記）
    public int m_QuestAreaAmendDrop = 100; //!< クエスト情報：エリア補正倍率：ドロップ率	（％表記）
    public int m_QuestAreaAmendExp = 100; //!< クエスト情報：エリア補正倍率：経験値		（％表記）
    public uint m_QuestMissionID = 0; //!< クエスト情報：クエストID
    public uint m_QuestRandSeed = 0; //!<  クエスト情報：乱数シード
    public bool m_IsUsedAutoPlay = false;   //!< オートプレイを使用したかどうか

    public MasterDataGuerrillaBoss m_QuestGuerrillaBoss = null; //!< クエスト情報：ゲリラボス

    public UserDataUnitParam m_PartyChara0Param = null; //!< パーティキャラ選択：キャラ情報	※リーダー兼ねる
    public UserDataUnitParam m_PartyChara1Param = null; //!< パーティキャラ選択：キャラ情報
    public UserDataUnitParam m_PartyChara2Param = null; //!< パーティキャラ選択：キャラ情報
    public UserDataUnitParam m_PartyChara3Param = null; //!< パーティキャラ選択：キャラ情報
    public UserDataUnitParam m_PartyChara4Param = null; //!< パーティキャラ選択：キャラ情報	※フレンド枠

    public PacketStructFriend m_PartyFriend = null; //!< パーティキャラ選択：フレンド情報

    public uint m_NextAreaCleard = 0;


    public void SetParam(int index, UserDataUnitParam v)
    {
        switch (index)
        {
            case 0: m_PartyChara0Param = v; break;
            case 1: m_PartyChara1Param = v; break;
            case 2: m_PartyChara2Param = v; break;
            case 3: m_PartyChara3Param = v; break;
            case 4: m_PartyChara4Param = v; break;
        }
    }
};
