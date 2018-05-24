/*==========================================================================*/
/*==========================================================================*/
/*!
    @file    ResidentParam.cs
    @brief    常駐パラメータ受け渡しクラス
    @author Developer
    @date     2012/10/08

    複数シーンをまたぐパラメータの受け渡し特化クラス
    単純なstatic変数での受け渡しのみを想定する。

    シーンをまたぐパラメータをバラバラに定義すると煩雑なコードになるため準備。
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*  Using                                                                   */
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ServerDataDefine;

/*==========================================================================*/
/*  namespace Begin                                                         */
/*==========================================================================*/
/*==========================================================================*/
/*  define                                                                  */
/*==========================================================================*/
/*==========================================================================*/
/*  macro                                                                   */
/*==========================================================================*/
/*==========================================================================*/
/*  class                                                                   */
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
    @brief    常駐パラメータ受け渡しクラス
*/
//----------------------------------------------------------------------------
static public class ResidentParam
{
    /*==========================================================================*/
    /*  var                                                                     */
    /*==========================================================================*/
    static public bool m_AreaSelectTechnicalMode;             //!< エリア選択画面挙動分岐：テクニカルエリア表示モード → インゲーム行って戻ってきても状態維持していたいらしいのでResidentPramへ移動

    static public int m_OptionBattleLayout;                  //!< オプション：戦闘情報：レイアウトパターンID
    static public int m_OptionBattlePlayType;                //!< オプション：戦闘情報：プレイヤー操作タイプ
    static public int m_OptionBattleCutin;                   //!< オプション：戦闘情報：カットイン動作タイプ
    static public int m_OptionBattleSkillChk;                //!< オプション：戦闘情報：スキル判定タイプ
    static public int m_OptionWorkLoadLevel;                 //!< オプション：負荷レベル情報：
    static public int m_OptionPlayMove;                      //!< オプション：移動制御パターン
    static public int m_OptionWall;                          //!< オプション：壁有無パターン
    static public int m_OptionLight;                         //!< オプション：ライトパターン
    static public int m_OptionCheat;                         //!< オプション：チートパターン
    static public int m_OptionMoveVariation;                 //!< オプション：移動フェーズバリエーション
    static public int m_OptionSPRecovery;                    //!< オプション：SP回復タイプ
    static public int m_OptionSPPattern;                     //!< オプション：SP消費タイプ
    static public int m_OptionBattleCostExpression;          //!< オプション：戦闘中意味ありコスト演出

    static public bool m_StartingFirstLoginBonus = false;   //!< 起動初回：ログインボーナス確認
    static public bool m_StartingFirstInformation = false;   //!< 起動初回：運営お知らせ確認
    static public int m_StartingFirstBeginnerInfo = 0;       //!< 起動初回：初心者ブースト内容確認済みID
    static public bool m_StartingFirstScratch = false;   //!< 起動初回：開催中スクラッチ確認
    static public bool m_StartingFirstPresent = false;   //!< 起動初回：プレゼント一覧確認
    // static public bool                         m_StartingFirstMonthlyBonus = false;   //!< 起動初回：月間ログインボーナス確認
    static public bool m_StartingFirstTopPage = false;   //!< 起動初回：トップページ確認

    static public bool m_BGMEnable = true;    //!< BGM有効
    static public int m_BGM_Volume = 1;       //!< BGM音量

    static public bool m_SEEnable = true;    //!< SE有効
    static public int m_SE_Volume = 1;       //!< SE音量

    static public bool m_DebugEnableMemory = false;   //!< デバッグ機能：メモリ使用量表示
    static public bool m_DebugEnableFPS = false;   //!< デバッグ機能：FPS表示

    static public bool m_QuestRestoreContinue = false;   //!< 中断復帰時成立済クエストコンティニュー
    static public bool m_QuestRestoreReset = false;   //!< 中断復帰時成立済クエストリセット

    static public bool m_OutOfMemoryError = false;   //!< メモリーエラー

    static public bool m_StoreCrashPointDialog = false;   //!< ストア処理でアプリ強制クラッシュダイアログを出すか否か

    static public bool m_ServerAPICrashPointDialog = false;   //!< サーバー通信時に強制クラッシュダイアログを出すか否か

    static public TemplateList<PacketAchievement> m_AchievementClear = null;    //!< アチーブメント情報：近々の通信で達成した
    static public int m_AchievementRewardCnt = 0;       //!< アチーブメント情報：達成報酬を獲得していないアチーブメントの数
    static public int m_AchievementNewCnt = 0;       //!< アチーブメント情報：新規発生アチーブメント

    static public string m_WebViewInfoURL = "";      //!< WebView：URL：お知らせ専用

    static public uint[] m_RegionIds = null;    //!< クエスト：カテゴリごとのリージョンID

    static public bool m_IsGoToTileWithApiError = false; //!< サーバーエラーでタイトルに戻された


    /*==========================================================================*/
    /*  func                                                                    */
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：完全リセット
    */
    //----------------------------------------------------------------------------
    static public void ParamReset()
    {
        m_AreaSelectTechnicalMode = false;    // エリア選択画面挙動分岐：テクニカルエリア表示モード

        m_OptionBattleLayout = 0;        // 戦闘情報：レイアウトパターンID
        m_OptionBattlePlayType = 1;        // 戦闘情報：プレイヤー操作タイプ
        m_OptionBattleCutin = 1;        // 戦闘情報：カットイン動作タイプ
        m_OptionBattleSkillChk = 0;        // 戦闘情報：スキル判定タイプ
        m_OptionWorkLoadLevel = 0;        // 負荷レベル情報
        m_OptionPlayMove = 1;        // 移動制御パターン
        m_OptionWall = 0;        // 壁有無パターン
        m_OptionLight = 0;        // ライトパターン
        m_OptionSPRecovery = 0;        // SP回復タイプ
        m_OptionSPPattern = 0;        // SP消費タイプ
        m_OptionBattleCostExpression = 1;        // 戦闘中意味ありコスト演出

        m_StartingFirstLoginBonus = false;    // 起動初回：ログインボーナス確認
        m_StartingFirstInformation = false;    // 起動初回：運営お知らせ確認
        m_StartingFirstBeginnerInfo = 0;        // 起動初回：初心者ブースト内容確認確認済みID
        m_StartingFirstScratch = false;    // 起動初回：開催中スクラッチ確認
        m_StartingFirstPresent = false;    // 起動初回：プレゼント一覧確認
        m_StartingFirstTopPage = false;    // 起動初回：トップページ確認

        m_QuestRestoreContinue = false;    // 中断復帰時成立済クエストコンティニュー
        m_QuestRestoreReset = false;    // 中断復帰時成立済クエストリセット

        m_OutOfMemoryError = false;    // メモリーエラー

        m_StoreCrashPointDialog = false;    // ストア処理でアプリ強制クラッシュダイアログを出すか否か

        m_ServerAPICrashPointDialog = false;    // サーバー通信時に強制クラッシュダイアログを出すか否か




        m_AchievementClear = new TemplateList<PacketAchievement>();  // アチーブメント情報：近々の通信で達成した

        m_WebViewInfoURL = MasterDataUtil.GetMasterDataGlobalParamTextFromID(GlobalDefine.WEB_LINK_INFORMATION);    // WebView：リンク先URL：お知らせ専用

        m_RegionIds = new uint[(int)MasterDataDefineLabel.REGION_CATEGORY.MAX]; //!< クエスト：カテゴリごとのリージョンID
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：ユーザー再生成時リセット
    */
    //----------------------------------------------------------------------------
    static public void ParamResetUserRenew()
    {
        m_StartingFirstLoginBonus = false;        // 起動初回：ログインボーナス確認
        m_StartingFirstInformation = false;        // 起動初回：運営お知らせ確認
        m_StartingFirstBeginnerInfo = 0;            // 起動初回：初心者ブースト内容確認確認済みID
        m_StartingFirstScratch = false;        // 起動初回：開催中スクラッチ確認
        m_StartingFirstPresent = false;        // 起動初回：プレゼント一覧確認
        m_StartingFirstTopPage = false;        // 起動初回：トップページ

        m_QuestRestoreContinue = false;        // 中断復帰時成立済クエストコンティニュー
        m_QuestRestoreReset = false;        // 中断復帰時成立済クエストリセット

        m_AchievementClear = null;         // アチーブメント情報：近々の通信で達成した
        m_AchievementRewardCnt = 0;            // アチーブメント情報：達成報酬を獲得していないアチーブメントの数;
        m_AchievementNewCnt = 0;            // アチーブメント情報：新規発生アチーブメント

        m_RegionIds = new uint[(int)MasterDataDefineLabel.REGION_CATEGORY.MAX]; //!< クエスト：カテゴリごとのリージョンID
    }

    /// <summary>
    /// パラメータ操作：タイトル時リセット
    /// </summary>
    static public void PramResetTitle()
    {
        m_RegionIds = new uint[(int)MasterDataDefineLabel.REGION_CATEGORY.MAX]; //!< クエスト：カテゴリごとのリージョンID
        m_IsGoToTileWithApiError = false;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  アチーブメント操作：新規達成アチーブメントの一覧追加
    */
    //----------------------------------------------------------------------------
    static public void AddAchievementClear(PacketAchievement[] cAchievements)
    {
        //----------------------------------------
        // 特に要素が発生していないならスルー
        //----------------------------------------
        if (cAchievements == null)
        {
            return;
        }

        //----------------------------------------
        //
        //----------------------------------------
        if (m_AchievementClear == null)
        {
            m_AchievementClear = new TemplateList<PacketAchievement>();
        }

        //----------------------------------------
        // アチーブメントの近々の達成済み一覧
        //----------------------------------------
        for (int i = 0; i < cAchievements.Length; i++)
        {
            if (m_AchievementClear.ChkInside(ChkInsidePacketAchievement, cAchievements[i]) == true)
                continue;

            //達成した種類のフラグをONにする。
            switch (cAchievements[i].achievement_category_id)
            {
                case (int)ACHIEVEMENT_CATEGORY_TYPE.DAILY:
                    UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionDaily, true);
                    break;
                case (int)ACHIEVEMENT_CATEGORY_TYPE.EVENT:
                    UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionEvent, true);
                    break;
                case (int)ACHIEVEMENT_CATEGORY_TYPE.NORMAL:
                    UserDataAdmin.Instance.SetUserFlag(UserDataAdmin.UserFlagType.GlobalMissionNormal, true);
                    break;
            }

            m_AchievementClear.Add(cAchievements[i]);
        }

        MainMenuHeader.SetupMissionClearMessage();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  アチーブメント操作：新規達成アチーブメントの検索
    */
    //----------------------------------------------------------------------------
    static public PacketAchievement GetAchievementClear(uint fix_id)
    {
        //----------------------------------------
        // 特に要素が発生していないならスルー
        //----------------------------------------
        if (m_AchievementClear == null)
        {
            return null;
        }
        //----------------------------------------
        // アチーブメントの近々の達成済み一覧
        //----------------------------------------
        for (int i = 0; i < m_AchievementClear.m_BufferSize; i++)
        {
            if (m_AchievementClear[i].fix_id == fix_id)
            {
                return m_AchievementClear[i];
            }
        }
        return null;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  アチーブメント操作：新規達成アチーブメントから要素を削除
    */
    //----------------------------------------------------------------------------
    static public void DelAchievementClear(uint fix_id)
    {
        //----------------------------------------
        // 特に要素が発生していないならスルー
        //----------------------------------------
        if (m_AchievementClear == null)
        {
            return;
        }
        PacketAchievement cDelAchievement = null;
        //----------------------------------------
        // アチーブメントの近々の達成済み一覧
        //----------------------------------------
        for (int i = 0; i < m_AchievementClear.m_BufferSize; i++)
        {
            if (m_AchievementClear[i].fix_id == fix_id)
            {
                cDelAchievement = m_AchievementClear[i];
                break;
            }
        }

        if (cDelAchievement != null)
        {
            m_AchievementClear.Remove(cDelAchievement);
            cDelAchievement = null;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  アチーブメント操作：新規達成アチーブメントから要素を削除
    */
    //----------------------------------------------------------------------------
    static public void DelAchievementClear(uint[] fix_ids)
    {
        if (fix_ids.IsNullOrEmpty() == false)
        {
            for (int i = 0; i < fix_ids.Length; ++i)
            {
                ResidentParam.DelAchievementClear(fix_ids[i]);
            }

            MainMenuHeader.SetupMissionClearMessage();
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  アチーブメント操作：新規達成アチーブメントの一覧破棄
    */
    //----------------------------------------------------------------------------
    static public void DelAchievementClear()
    {
        m_AchievementClear = null;
        MainMenuHeader.SetupMissionClearMessage();
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  内包チェック：PacketAchievement
    */
    //----------------------------------------------------------------------------
    static public int ChkInsidePacketAchievement(PacketAchievement a, PacketAchievement b)
    {
        if (a.fix_id == b.fix_id)
            return 1;
        return 0;
    }

    /// <summary>
    /// クエストに関連付けされたクリアミッションを取得
    /// </summary>
    /// <param name="_quest_id"></param>
    /// <returns></returns>
    static public PacketAchievement[] GetQuestAchivementClearList(uint _quest_id)
    {
        //----------------------------------------
        // 特に要素が発生していないならスルー
        //----------------------------------------
        if (m_AchievementClear == null)
        {
            return null;
        }

        List<PacketAchievement> retList = new List<PacketAchievement>();
        //----------------------------------------
        // アチーブメントの近々の達成済み一覧
        //----------------------------------------
        for (int i = 0; i < m_AchievementClear.m_BufferSize; i++)
        {
            if (m_AchievementClear[i].achievement_category_id == (int)ACHIEVEMENT_CATEGORY_TYPE.QUEST)
            {
                retList.Add(m_AchievementClear[i]);
            }
        }

        if (retList.Count == 0) return null;

        return retList.ToArray();
    }

    /// <summary>
    /// 新規発生アチーブメントカウント数を追加
    /// </summary>
    /// <param name="count"></param>
    static public void AddAchievementNewCnt(int count)
    {
        if (count >= 0)
        {
            ResidentParam.m_AchievementNewCnt += count;
        }
        if (count > 0)
        {
            MainMenuHeader.SetupMissionNewMessae();
        }
    }

    /// <summary>
    /// 新規発生アチーブメントカウント数を削除
    /// </summary>
    static public void DelAchievementNewCnt()
    {
        if (ResidentParam.m_AchievementNewCnt > 0)
        {
            ResidentParam.m_AchievementNewCnt = 0;
            MainMenuHeader.SetupMissionNewMessae();
        }
    }
}

