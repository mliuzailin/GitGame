/*==========================================================================*/
/*==========================================================================*/
/*!
    @file  MainMenuParam.cs
    @brief  メインメニューパラメータ受け渡しクラス
    @author Developer
    @date   2012/11/27

    メインメニュー関連のパラメータ受け渡しに特化。
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*    Using                                */
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using ServerDataDefine;

/*==========================================================================*/
/*    namespace Begin                           */
/*==========================================================================*/
/*==========================================================================*/
/*    define                                */
/*==========================================================================*/
/*==========================================================================*/
/*    macro                                */
/*==========================================================================*/
/*==========================================================================*/
/*    class                                */
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
    @brief  メインメニューパラメータ受け渡しクラス
*/
//----------------------------------------------------------------------------
static public class MainMenuParam
{
    /*==========================================================================*/
    /*    var                                  */
    /*==========================================================================*/

    static public uint m_RegionID;                         //!< クエスト：表示するリージョンID

    static public uint m_QuestSelectAreaCateID;            //!< 通常クエスト発行関連：エリアカテゴリID(v300対応)
    static public uint m_QuestSelectAreaID;             //!< 通常クエスト発行関連：エリアID
    static public int m_QuestSelectAreaAmendStamina;        //!< 通常クエスト発行関連：エリア補正：スタミナ
    static public int m_QuestSelectAreaAmendEXP;            //!< 通常クエスト発行関連：エリア補正：経験値
    static public int m_QuestSelectAreaAmendCoin;      //!< 通常クエスト発行関連：エリア補正：コイン
    static public int m_QuestSelectAreaAmendDrop;      //!< 通常クエスト発行関連：エリア補正：ドロップ率
    static public int m_QuestSelectAreaAmendTicket;    //!< 通常クエスト発行関連：エリア補正：チケット
    static public int m_QuestSelectAreaAmendGuerrillaBoss;//!< 通常クエスト発行関連：エリア補正：ゲリラボス
    static public int m_QuestSelectAreaAmendLinkPoint;  //!< 通常クエスト発行関連：エリア補正：リンクポイント
    static public uint m_QuestSelectMissionID;              //!< 通常クエスト発行関連：クエストID
    static public PacketStructFriend m_QuestHelper;                     //!< 通常クエスト発行関連：助っ人ユーザーID：
    static public uint m_QuestEventFP;                      //!< 通常クエスト発行関連：発生イベント：フレンドポイント増加
    static public uint m_QuestAddMoney;                 //!< 通常クエスト発行関連：入手コイン
    static public TemplateList<MasterDataAreaAmend> m_QuestAreaAmendList;               //!< 通常クエスト発行関連：エリア補正リスト
#if BUILD_TYPE_DEBUG
    static public int m_DebugBattleNumber;        //!< 通常クエスト発行関連：デバッグ：バトル選択
#endif
    static private uint m_challengeQuestMissionID;            //!< 成長ボス：選択クエストID
    static public uint m_ChallengeQuestMissionID { get { return m_challengeQuestMissionID; } }
    static public int m_ChallengeQuestLevel;                  //!< 成長ボスクエスト発行関連：クエストレベル
    static public bool m_bChallengeQuestSkip;                 //!< 成長ボスクエスト発行関連：スキップフラグ

    static private uint m_jmpQuestSelectAreaCateID;
    static public uint m_JmpQuestSelectAreaCateID { get { return m_jmpQuestSelectAreaCateID; } }
    static private uint m_jmpQuestSelectAreaID;
    static public uint m_JmpQuestSelectAreaID { get { return m_jmpQuestSelectAreaID; } }

    static private MasterDataDefineLabel.QuestType m_SaveSelectQuestType;       // 選択保存：クエストタイプ
    static private uint m_SaveSelectAreaCategoryID;                             // 選択保存：エリアカテゴリーID
    static private uint m_SaveSelectAreaID;                                     // 選択保存：エリアID
    static private uint m_SaveSelectChallengeQuestID;                           // 選択保存；成長ボスクエストID

    static public MAINMENU_UNIT_SELECT_TYPE m_UnitSelectType;           //!< ユニット選択タイプ

    static public long m_BuildupBaseUnitUniqueId;   //!< 強化ベースユニットユニークID
    static public PacketStructUnit m_BlendBuildUpUnitPrev;      //!< 強化合成発行関連：強化前パラメータ
    static public PacketStructUnit m_BlendBuildUpUnitAfter; //!< 強化合成発行関連：強化後パラメータ
    static public TemplateList<PacketStructUnit> m_BlendBuildUpParts;       //!< 強化合成発行関連：パーツ
    static public uint m_BlendBuildEventSLV;        //!< 強化合成発行関連：発生イベント：スキルレベルアップ確率上昇

    static public long m_EvolveBaseUnitUniqueId;    //!< 進化ベースユニットユニークID
    static public PacketStructUnit m_EvolveBaseBefore;          //!< 進化発行関連：ベースユニット：進化前パラメータ
    static public PacketStructUnit m_EvolveBaseAfter;           //!< 進化発行関連：ベースユニット：進化後パラメータ
    static public TemplateList<PacketStructUnit> m_EvolveParts;             //!< 進化発行関連：パーツ

    static public long m_LinkBaseUnitUniqueId;      //!< リンクベースユニットユニークID
    static public long m_LinkTargetUnitUniqueId;    //!< リンクターゲットユニットユニークID
    static public PacketStructUnit m_LinkBaseBefore;            //!< リンク発行関連：ベースユニット：リンク前パラメータ
    static public PacketStructUnit m_LinkBaseAfter;         //!< リンク発行関連：ベースユニット：リンク後パラメータ
    static public TemplateList<PacketStructUnit> m_LinkParts;               //!< リンク発行関連：素材ユニット
    static public PacketStructUnit m_LinkUnit;                 //!< リンク発行関連：リンクユニット

    static public uint m_CharaDetailCharaID;                //!< キャラ詳細：選択キャラID
    static public uint m_CharaDetailCharaLevel;         //!< キャラ詳細：選択キャラレベル
    static public uint m_CharaDetailCharaExp;               //!< キャラ詳細：選択キャラ経験値
    static public uint m_CharaDetailCharaAddPow;            //!< キャラ詳細：選択キャラパラメータ補正値：攻撃力
    static public uint m_CharaDetailCharaAddHP;         //!< キャラ詳細：選択キャラパラメータ補正値：体力
    static public uint m_CharaDetailCharaLBSkillLV;    //!< キャラ詳細：選択キャラリミットブレイクスキルレベル
    static public uint m_CharaDetailCharaLOverLV;           //!< キャラ詳細：選択キャラ限界突破レベル

    static public uint m_CharaDetailCharaLinkTYPE;          //!< キャラ詳細：選択キャラ：リンク：親か子か
    static public uint m_CharaDetailCharaLinkID;            //!< キャラ詳細：選択キャラ：リンク：キャラID
    static public uint m_CharaDetailCharaLinkLv;            //!< キャラ詳細：選択キャラ：リンク：キャラレベル
    static public uint m_CharaDetailCharaLinkAddPow;        //!< キャラ詳細：選択キャラ：リンク：パラメータ補正値：攻撃力
    static public uint m_CharaDetailCharaLinkAddHP;     //!< キャラ詳細：選択キャラ：リンク：パラメータ補正値：体力
    static public uint m_CharaDetailCharaLinkPoint;    //!< キャラ詳細：選択キャラ：リンク：ポイント(リンクスキル発動率に影響)
    static public uint m_CharaDetailCharaLinkLimitOver; //!< キャラ詳細：選択キャラ：リンク：限界突破

    static public Stack<MAINMENU_SEQ> m_PageBack;                       //!< 戻るボタン：戻り先


    static public uint m_ResultParamActive;             //!< リザルト情報：リザルト情報有無フラグ
    static public uint m_ResultTicket;                      //!< リザルト情報：入手チケット
    static public uint m_ResultExp;                     //!< リザルト情報：入手経験値
    static public uint m_ResultMoney;            //!< リザルト情報：入手コイン
    static public uint m_ResultFriendPt;                   //!< リザルト情報：入手フレンドポイント
    static public uint m_ResultStone;                       //!< リザルト情報：クエストクリア報酬：入手魔法石
    static public long m_ResultClearUnit;                   //!< リザルト情報：クエストクリア報酬：ユニット
    static public uint m_ResultClearItem;                   //!< リザルト情報：クエストクリア報酬：アイテム
    static public uint m_ResultClearQuestKey;               //!< リザルト情報：クエストクリア報酬：クエストキー
    static public long[] m_ResultUnit;                      //!< リザルト情報：ユニットユニークID
    static public uint m_ResultQuestID;                 //!< リザルト情報：クエストID
    static public PacketStructFloorBonus[] m_ResultFloorBonus;                  //!< リザルト情報：フロアボーナス入手ユニット
    static public PacketStructResultScore[] m_ResultScores;                     //!< リザルト情報：スコア情報
    static public PacketStructResultChallenge m_ResultChallenge;              //!< リザルト情報：成長ボス情報

    static public int[] m_ResultRewardLimit;                //!< リザルト情報：報酬超過アチーブメントID

    static public PacketStructUnit[] m_ResultPrevUnit;                  //!< リザルト情報：リザルト申請前情報：ユニットリスト
    static public PacketStructFriend m_ResultPrevFriend;                    //!< リザルト情報：リザルト申請前情報：フレンド
    static public uint m_ResultPrevRank;                    //!< リザルト情報：リザルト申請前情報：ランク
    static public uint m_ResultPrevExp;                 //!< リザルト情報：リザルト申請前情報：経験値
    static public uint m_ResultPrevMoney;                   //!< リザルト情報：リザルト申請前情報：コイン
    static public uint m_ResultPrevStone;                   //!< リザルト情報：リザルト申請前情報：魔法石報酬
    static public uint m_ResultPrevTicket;                  //!< リザルト情報：リザルト申請前情報：チケット
    static public uint m_ResultPrevFriendPoint;         //!< リザルト情報：リザルト申請前情報：フレンドポイント
    static public byte[] m_ResultPrevUnitGetFlag;      //!< リザルト情報：リザルト申請前情報：ユニット入手状況
    static public bool m_ResultQuest2;                     //!< リザルト情報：リザルト申請前情報：新クエストかどうか
    static public PacketStructHero m_ResultPrevHero;                   //!< リザルト情報：リザルト申請前情報：ヒーロー情報

    static public uint m_ResultAreaID;                   //!< リザルト情報：エリアID
    static public uint m_ResultNextArea;                   //!< リザルト情報：次のエリアのオープン情報

    static public uint m_GachaID;                           //!< ガチャ情報：ガチャID
    static public MasterDataGacha m_GachaMaster;                        //!< ガチャ情報：ガチャマスター
    static public int m_GachaGetUnitNum;                  //!< ガチャ情報：ガチャで取得したユニット数
    static public long[] m_GachaUnitUniqueID;               //!< ガチャ情報：ガチャで入手したユニットユニークID
    static public uint[] m_GachaBlankUnitID;          //!< ガチャ情報：ガチャでハズレ枠に表示するユニットのキャラID
    static public uint[] m_GachaOmakeID;                     //!< ガチャ情報：ガチャおまけプレゼントID
    static public byte[] m_GachaPrevUnitGetFlag;                //!< ガチャ情報：ガチャ引く前のユニット入手フラグ
    static public uint m_GachaRainbowDecide;                //!< ガチャ情報：虹確定アサインID


    static public bool m_LoginActive;                       //!< ログイン情報：ログイン情報有効
    static public uint m_LoginTotal;                        //!< ログイン情報：総合ログイン日数
    static public uint m_LoginChain;                        //!< ログイン情報：連続ログイン日数
    static public PacketStructLoginBonus[] m_LoginBonus;                        //!< ログイン情報：ログインボーナス
    static public uint m_LoginFriendPointNow;               //!< ログイン情報：フレンド：フレンドポイント：現在値
    static public uint m_LoginFriendPointGet;               //!< ログイン情報：フレンド：フレンドポイント：今回取得分
    static public uint m_LoginFriendHelpCt;             //!< ログイン情報：フレンド：助っ人として助けた人数


    static public MAINMENU_SORT_SEQ[] m_DialogSelectSortType;               //!< ダイアログ受け渡し：選択可能なソートのタイプ

    static public string m_DialogWebViewURL;                    //!< ダイアログ受け渡し：WebView：URL

    static public DateTime m_ReturnTitleTime = DateTime.MaxValue;                 //!< タイトルに戻る日時
    static public DateTime m_DayStraddleTime = DateTime.MaxValue;                 //!< 日跨ぎAPIを呼ぶ時間

    static public int m_LunchTimeStart;                 //!< ランチタイム期間：開始時間（1130）時分のみ
    static public int m_LunchTimeEnd;                     //!< ランチタイム期間：終了時間（1330）時分のみ

    static public PacketStructBoxGachaStock[] m_BoxGachaStock;                  //!< BOXガチャのストック情報
    static public uint m_BoxGachaStockID;                   //!< m_BoxGachaStockのガチャID

    static public MasterDataBeginnerBoost m_BeginnerBoost;                  //!< 適用する初心者ブースト

    static public float m_QuestStaminaAmend;                //!< クエスト消費スタミナ補正倍率
    static public uint m_QuestStamina;            //!< クエスト消費スタミナ
    static public uint m_QuestKey;                          //!< クエスト消費キー
    static public uint m_QuestTicket;                       //!< クエスト消費チケット

#if BUILD_TYPE_DEBUG
    static public bool m_UseTransferGooglePathDebug;        //!< ダイアログ受け渡し：デバッグ：Google認証をパスするか
#endif

    static public bool m_AchievementListGet = false;    //!< アチーブメント：全アチーブメントマスタ再取得要否

    static public PacketAchievement m_AchievementShowData;  //!< アチーブメントグループ：ミッションで表示するデータ

    static public DATE_CHANGE_TYPE m_DateChangeType;        //!< 日付変更タイプ

    static public uint m_PointShopLimitOverProductID = 0;   //!< ポイントショップ限界突破：限界突破のポイントショップ商品ID
    static public uint m_PointShopEvolProductID = 0;        //!< ポイントショップ進化：進化のポイントショップ商品ID

    static public MAINMENU_SEQ m_PartyAssignPrevPage;               //!< ユニット編成：表示ページ管理：遷移元ページ

    static public int m_HeroCurrentInex = -1;        //!< 主人公選択：選択ユニット
    static public bool m_HeroSelectReturn = false;         //!< 主人公選択：主人公詳細などから主人公選択に戻て来たかどうか

    static public MasterDataEvent m_DialogEventScheduleData = null; //!< イベントスケジュール：直接詳細画面を表示するときに使う

    static public int m_BannerLastIndexHome;
    static public int m_BannerLastIndexQuest;

    static public bool m_PartySelectIsShowLinkUnit = false; //!< パーティー選択：リンクユニットを表示・非表示状態
    static public bool m_PartySelectShowedLinkUnit = false; //!< パーティー選択：リンクユニットを表示していたかどうか

    static public bool m_IsEnableQuestFriendReload = true;       //!< フレンド選択：更新可能かどうか

    static public int[] m_AmendFlagCheckTime = null;            //!< エリア補間フラグチェック時間
    static public bool[] m_AmendFlagCheckResult = null;         //!< エリア補間フラグチェックリザルト

    /*==========================================================================*/
    /*    func                                */
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：完全リセット
    */
    //----------------------------------------------------------------------------
    static public void ParamReset()
    {
        m_QuestSelectAreaCateID = 0;
        m_QuestSelectAreaID = 0;
        m_QuestSelectMissionID = 0;
        m_QuestHelper = null;
        m_QuestEventFP = 0;

        m_BuildupBaseUnitUniqueId = 0;  //!< 強化ベースユニットユニークID
        m_BlendBuildUpUnitPrev = null;                                          // 強化合成発行関連：強化前パラメータ
        m_BlendBuildUpUnitAfter = null;                                         // 強化合成発行関連：強化後パラメータ
        m_BlendBuildUpParts = new TemplateList<PacketStructUnit>();      // 強化合成発行関連：パーツ
        m_BlendBuildEventSLV = 0;

        m_EvolveBaseUnitUniqueId = 0;
        m_EvolveBaseAfter = null;
        m_EvolveBaseBefore = null;
        m_EvolveParts = new TemplateList<PacketStructUnit>();

        m_LinkBaseUnitUniqueId = 0;
        m_LinkTargetUnitUniqueId = 0;
        m_LinkBaseBefore = null;                                            // リンク発行関連：ベースユニット：リンク前パラメータ
        m_LinkBaseAfter = null;                                         // リンク発行関連：ベースユニット：リンク後パラメータ
        m_LinkParts = new TemplateList<PacketStructUnit>();         // リンク発行関連：素材ユニット
        m_LinkUnit = null;                      // リンク発行関連：リンクユニット

        // ユニット詳細用データクリア
        ClearCharaDetailParam();
        // ユニット詳細用データ(リンク用データ)クリア
        ClearCharaDetailLinkParam();

        m_PageBack = new Stack<MAINMENU_SEQ>();
        m_PageBack.Clear();

        m_ResultParamActive = 0;
        m_ResultTicket = 0;
        m_ResultExp = 0;
        m_ResultMoney = 0;
        m_ResultFriendPt = 0;
        m_ResultUnit = null;
        m_ResultQuestID = 0;
        m_ResultAreaID = 0;

        m_ResultFloorBonus = null;

        m_ResultRewardLimit = null;

        m_ResultScores = null;

        m_LoginActive = false;          // ログイン情報：ログイン情報有効
        m_LoginTotal = 0;               // ログイン情報：総合ログイン日数
        m_LoginChain = 0;               // ログイン情報：連続ログイン日数
        m_LoginBonus = null;                // ログイン情報：ログインボーナス
        m_LoginFriendPointNow = 0;              // ログイン情報：フレンド：フレンドポイント：現在値
        m_LoginFriendPointGet = 0;              // ログイン情報：フレンド：フレンドポイント：今回取得分
        m_LoginFriendHelpCt = 0;                // ログイン情報：フレンド：助っ人として助けた人数

        m_BeginnerBoost = null;

        m_QuestStamina = 0;

        m_BoxGachaStock = null;             //!< BOXガチャのストック情報
        m_BoxGachaStockID = 0;              //!< m_BoxGachaStockのガチャID

        m_PartyAssignPrevPage = 0;

        m_BannerLastIndexHome = 0;
        m_BannerLastIndexQuest = 0;
        m_PartySelectShowedLinkUnit = false;

        m_ChallengeQuestLevel = 0;
        m_bChallengeQuestSkip = false;
        m_challengeQuestMissionID = 0;

        m_IsEnableQuestFriendReload = true;

        m_AmendFlagCheckTime = new int[4];
        m_AmendFlagCheckResult = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            m_AmendFlagCheckTime[i] = -1;
            m_AmendFlagCheckResult[i] = false;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：ユニット詳細用データ設定：キャラIDとレベルの指定
    */
    //----------------------------------------------------------------------------
    static public void SetCharaDetailParam(uint uID, uint uLevel)
    {
        m_CharaDetailCharaID = uID;     //!< キャラ詳細：選択キャラID
        m_CharaDetailCharaLevel = uLevel;   //!< キャラ詳細：選択キャラレベル
        m_CharaDetailCharaExp = 0;      //!< キャラ詳細：選択キャラ経験値
        m_CharaDetailCharaAddPow = 0;       //!< キャラ詳細：選択キャラパラメータ補正値：攻撃力
        m_CharaDetailCharaAddHP = 0;        //!< キャラ詳細：選択キャラパラメータ補正値：体力
        m_CharaDetailCharaLBSkillLV = 0;        //!< キャラ詳細：選択キャラリミットブレイクスキルレベル
        m_CharaDetailCharaLOverLV = 0;      //!< キャラ詳細：選択キャラ限界突破レベル
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：ユニット詳細用データ設定：選択ユニットのみ指定版
    */
    //----------------------------------------------------------------------------
    static public void SetCharaDetailParam(PacketStructUnit cUnit)
    {
        m_CharaDetailCharaID = cUnit.id;                //!< キャラ詳細：選択キャラID
        m_CharaDetailCharaLevel = cUnit.level;          //!< キャラ詳細：選択キャラレベル
        m_CharaDetailCharaExp = cUnit.exp;          //!< キャラ詳細：選択キャラ経験値
        m_CharaDetailCharaAddPow = cUnit.add_pow;       //!< キャラ詳細：選択キャラパラメータ補正値：攻撃力
        m_CharaDetailCharaAddHP = cUnit.add_hp;         //!< キャラ詳細：選択キャラパラメータ補正値：体力
        m_CharaDetailCharaLBSkillLV = cUnit.limitbreak_lv;  //!< キャラ詳細：選択キャラリミットブレイクスキルレベル
        m_CharaDetailCharaLOverLV = cUnit.limitover_lv; //!< キャラ詳細：選択キャラ限界突破レベル
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：ユニット詳細用データ設定：リンクしているユニット指定版
    */
    //----------------------------------------------------------------------------
    static public void SetCharaDetailParam(PacketStructUnit cUnit, PacketStructUnit cLinkUnit)
    {
        // 選択ユニット情報がnullなら全データクリアして戻る
        if (cUnit == null)
        {
            ClearCharaDetailParam();
            ClearCharaDetailLinkParam();
            return;
        }
        // 選択ユニット情報指定
        SetCharaDetailParam(cUnit);

        if (cLinkUnit != null)
        {
            // 選択↓ユニットとリンクしているユニット情報指定
            m_CharaDetailCharaLinkTYPE = cLinkUnit.link_info;       //!< キャラ詳細：選択キャラ：リンク：親か子か
            m_CharaDetailCharaLinkID = cLinkUnit.id;                //!< キャラ詳細：選択キャラ：リンク：キャラID
            m_CharaDetailCharaLinkLv = cLinkUnit.level;         //!< キャラ詳細：選択キャラ：リンク：キャラレベル
            m_CharaDetailCharaLinkAddPow = cLinkUnit.add_pow;       //!< キャラ詳細：選択キャラ：リンク：パラメータ補正値：攻撃力
            m_CharaDetailCharaLinkAddHP = cLinkUnit.add_hp;         //!< キャラ詳細：選択キャラ：リンク：パラメータ補正値：体力
            m_CharaDetailCharaLinkLimitOver = cLinkUnit.limitover_lv;   //!< キャラ詳細：選択キャラ：リンク：限界突破

            //!< キャラ詳細：選択キャラ：リンク：ポイント(リンクスキル発動率に影響)
            // 基本的にリンクポイントはベースユニットに入る為、ベースユニットの情報を入れる
            if (cLinkUnit.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_LINK)
            {
                m_CharaDetailCharaLinkPoint = cUnit.link_point;
            }
            else
            {
                m_CharaDetailCharaLinkPoint = cLinkUnit.link_point;
            }
        }
        else
        {
            // ユニット詳細用データ(リンク用データ)クリア
            ClearCharaDetailLinkParam();
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：ユニット詳細用データ設定：フレンド情報からの構築版
        @note  フレンド情報の場合。ベースユニットと小ユニットが明確に判別されて送られるので、別処理で用意
    */
    //----------------------------------------------------------------------------
    static public void SetCharaDetailParam(PacketStructFriend cFriend)
    {
        m_CharaDetailCharaID = cFriend.unit.id;                 //!< キャラ詳細：選択キャラID
        m_CharaDetailCharaLevel = cFriend.unit.level;               //!< キャラ詳細：選択キャラレベル
        m_CharaDetailCharaExp = cFriend.unit.exp;                   //!< キャラ詳細：選択キャラ経験値
        m_CharaDetailCharaAddPow = cFriend.unit.add_pow;                //!< キャラ詳細：選択キャラパラメータ補正値：攻撃力
        m_CharaDetailCharaAddHP = cFriend.unit.add_hp;              //!< キャラ詳細：選択キャラパラメータ補正値：体力
        m_CharaDetailCharaLBSkillLV = cFriend.unit.limitbreak_lv;       //!< キャラ詳細：選択キャラリミットブレイクスキルレベル
        m_CharaDetailCharaLOverLV = cFriend.unit.limitover_lv;      //!< キャラ詳細：選択キャラ限界突破レベル

        // リンクポイントはベースユニットの情報を用いる
        m_CharaDetailCharaLinkPoint = cFriend.unit.link_point;          //!< キャラ詳細：選択キャラ：リンク：ポイント(リンクスキル発動率に影響)

        m_CharaDetailCharaLinkTYPE = cFriend.unit_link.link_info;       //!< キャラ詳細：選択キャラ：リンク：親か子か
        m_CharaDetailCharaLinkID = cFriend.unit_link.id;                //!< キャラ詳細：選択キャラ：リンク：キャラID
        m_CharaDetailCharaLinkLv = cFriend.unit_link.level;         //!< キャラ詳細：選択キャラ：リンク：キャラレベル
        m_CharaDetailCharaLinkAddPow = cFriend.unit_link.add_pow;       //!< キャラ詳細：選択キャラ：リンク：パラメータ補正値：攻撃力
        m_CharaDetailCharaLinkAddHP = cFriend.unit_link.add_hp;         //!< キャラ詳細：選択キャラ：リンク：パラメータ補正値：体力
        m_CharaDetailCharaLinkLimitOver = cFriend.unit_link.limitover_lv;   //!< キャラ詳細：選択キャラ：リンク：限界突破
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：ユニット詳細用データクリア
    */
    //----------------------------------------------------------------------------
    static public void ClearCharaDetailParam()
    {
        m_CharaDetailCharaID = 0;   //!< キャラ詳細：選択キャラID
        m_CharaDetailCharaLevel = 0;    //!< キャラ詳細：選択キャラレベル
        m_CharaDetailCharaExp = 0;  //!< キャラ詳細：選択キャラ経験値
        m_CharaDetailCharaAddPow = 0;   //!< キャラ詳細：選択キャラパラメータ補正値：攻撃力
        m_CharaDetailCharaAddHP = 0;    //!< キャラ詳細：選択キャラパラメータ補正値：体力
        m_CharaDetailCharaLBSkillLV = 0;    //!< キャラ詳細：選択キャラリミットブレイクスキルレベル
        m_CharaDetailCharaLOverLV = 0;  //!< キャラ詳細：選択キャラ限界突破レベル
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ操作：ユニット詳細用データクリア：リンクユニット版
    */
    //----------------------------------------------------------------------------
    static public void ClearCharaDetailLinkParam()
    {
        m_CharaDetailCharaLinkTYPE = 0; //!< キャラ詳細：選択キャラ：リンク：親か子か
        m_CharaDetailCharaLinkID = 0;   //!< キャラ詳細：選択キャラ：リンク：キャラID
        m_CharaDetailCharaLinkLv = 0;   //!< キャラ詳細：選択キャラ：リンク：キャラレベル
        m_CharaDetailCharaLinkAddPow = 0;   //!< キャラ詳細：選択キャラ：リンク：パラメータ補正値：攻撃力
        m_CharaDetailCharaLinkAddHP = 0;    //!< キャラ詳細：選択キャラ：リンク：パラメータ補正値：体力
        m_CharaDetailCharaLinkPoint = 0;    //!< キャラ詳細：選択キャラ：リンク：ポイント(リンクスキル発動率に影響)
        m_CharaDetailCharaLinkLimitOver = 0;    //!< キャラ詳細：選択キャラ：リンク：限界突破
    }

    //----------------------------------------------------------------------------
    /*!
        @brief  パラメータ取得：BOXガチャ情報：BOXガチャの現在在庫と最大在庫取得
    */
    //----------------------------------------------------------------------------
    static public void GetBoxGachaStockInfo(ref int nNowStock, ref int nMaxStock)
    {
        nNowStock = 0;
        nMaxStock = 0;
        // データが無かったら0を返す
        if (m_BoxGachaStock == null
        || m_BoxGachaStockID == 0)
        {
            return;
        }
        // カウントしていく
        for (int StockCount = 0; StockCount < m_BoxGachaStock.Length; StockCount++)
        {
            nNowStock += m_BoxGachaStock[StockCount].now_stock;
            nMaxStock += m_BoxGachaStock[StockCount].max_stock;
        }
    }

    /// <summary>
    /// クエスト選択画面へ遷移するときのパラメータ設定
    /// </summary>
    /// <param name="_areaCategoryId"></param>
    /// <param name="_areaId"></param>
    static public void SetQuestSelectParam(uint _areaCategoryId, uint _areaId = 0)
    {
        m_jmpQuestSelectAreaCateID = _areaCategoryId;
        m_jmpQuestSelectAreaID = _areaId;
    }

    /// <summary>
    /// 成長ボス選択画面へ遷移するときのパラメータ設定
    /// </summary>
    /// <param name="challengeQuestID"></param>
    static public void SetChallengeSelectParam(uint challengeQuestID, bool bSkip = false, int skipLevel = 0)
    {
        m_challengeQuestMissionID = 0;
        m_ChallengeQuestLevel = 0;
        m_bChallengeQuestSkip = false;
        if (MasterDataUtil.GetQuestType(challengeQuestID) != MasterDataDefineLabel.QuestType.CHALLENGE)
        {
            return;
        }
        m_challengeQuestMissionID = challengeQuestID;
        m_ChallengeQuestLevel = skipLevel;
        m_bChallengeQuestSkip = bSkip;
    }

    /// <summary>
    /// 成長ボス選択画面へ遷移するときのパラメータ設定(イベントID指定)
    /// </summary>
    /// <param name="event_id"></param>
    static public void SetChallengeSelectParamFromEventID(uint event_id)
    {
        MasterDataChallengeQuest master = MasterFinder<MasterDataChallengeQuest>.Instance.SelectFirstWhere("where event_id = ? ", event_id);
        SetChallengeSelectParam((master != null ? master.fix_id : 0));
    }

    static public bool MoveQuestDetail(MasterDataQuest2 quest2Master)
    {
        if (quest2Master == null)
        {
            return false;
        }

        MasterDataArea areaMaster = MasterFinder<MasterDataArea>.Instance.Find((int)quest2Master.area_id);
        MainMenuQuestSelect.AreaAmendParam areaAmendParam = MainMenuUtil.CreateAreaParamAmend(areaMaster);

        MainMenuParam.m_QuestStamina = 0;
        MainMenuParam.m_QuestKey = 0;
        MainMenuParam.m_QuestTicket = 0;
        switch (quest2Master.consume_type)
        {
            case 1:
                if (areaAmendParam.m_QuestSelectAreaAmendStamina == 100)
                {
                    MainMenuParam.m_QuestStamina = (uint)quest2Master.consume_value;
                }
                else
                {
                    uint point = (uint)((float)quest2Master.consume_value * ((float)areaAmendParam.m_QuestSelectAreaAmendStamina / 100.0f));
                    MainMenuParam.m_QuestStamina = point;
                }
                break;
            case 2:
                MainMenuParam.m_QuestKey = (uint)quest2Master.consume_value;
                break;
            case 3:
                MainMenuParam.m_QuestTicket = (uint)quest2Master.consume_value;
                break;
        }

        if (MainMenuManager.HasInstance)
        {
            MainMenuParam.m_QuestSelectAreaID = areaMaster.fix_id;
            MainMenuParam.m_QuestSelectMissionID = quest2Master.fix_id;
            MainMenuParam.m_QuestAreaAmendList = areaAmendParam.m_AreaMasterDataAmendList;
            MainMenuParam.m_QuestStaminaAmend = (float)areaAmendParam.m_QuestSelectAreaAmendStamina / 100.0f;
            MainMenuParam.m_QuestSelectAreaCateID = areaMaster.area_cate_id;
            m_jmpQuestSelectAreaCateID = areaMaster.area_cate_id;
            m_jmpQuestSelectAreaID = areaMaster.fix_id;

            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_DETAIL, false, false, true);
        }

        return true;
    }

    static public bool FindPageBack(MAINMENU_SEQ type)
    {
        MAINMENU_SEQ[] array_sequence = MainMenuParam.m_PageBack.ToArray();
        for (int i = 0; i < array_sequence.Length; i++)
        {
            if (array_sequence[i] == type)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// 選択保存：保存データリセット
    /// </summary>
    static public void ResetSaveSelect()
    {
        m_SaveSelectQuestType = MasterDataDefineLabel.QuestType.NONE;
        m_SaveSelectAreaCategoryID = 0;
        m_SaveSelectAreaID = 0;
        m_SaveSelectChallengeQuestID = 0;
    }

    /// <summary>
    /// 選択保存：通常クエスト選択情報保存
    /// </summary>
    /// <param name="areaCategoryID"></param>
    /// <param name="areaID"></param>
    static public void SetSaveSelectNormal(uint areaCategoryID, uint areaID)
    {
        m_SaveSelectQuestType = MasterDataDefineLabel.QuestType.NORMAL;
        m_SaveSelectAreaCategoryID = areaCategoryID;
        m_SaveSelectAreaID = areaID;
    }

    /// <summary>
    /// 選択保存：成長ボス選択情報保存
    /// </summary>
    /// <param name="challengeQuestID"></param>
    static public void SetSaveSelectChallenge(uint challengeQuestID)
    {
        m_SaveSelectQuestType = MasterDataDefineLabel.QuestType.CHALLENGE;
        m_SaveSelectChallengeQuestID = challengeQuestID;
    }

    /// <summary>
    /// 選択保存：選択情報から遷移を設定
    /// </summary>
    /// <returns>設定されなかったら[false]を返す</returns>
    static public bool SetupSaveSelect()
    {
        bool bRet = false;
        switch (m_SaveSelectQuestType)
        {
            case MasterDataDefineLabel.QuestType.NORMAL:
                {
                    //有効なエリアカテゴリかどうかチェック
                    if (MainMenuUtil.CheckRNAreaCategory(m_SaveSelectAreaCategoryID))
                    {
                        //クエスト選択画面へ
                        SetQuestSelectParam(m_SaveSelectAreaCategoryID, m_SaveSelectAreaID);
                        bRet = MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false);
                    }
                    else
                    {
                        //有効でない場合は属しているマップに遷移
                        MasterDataDefineLabel.REGION_CATEGORY caegory = MainMenuUtil.GetRegionCategory(m_SaveSelectAreaCategoryID);
                        MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(caegory);
                        bRet = MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY, false, false);
                    }
                }
                break;
            case MasterDataDefineLabel.QuestType.CHALLENGE:
                {
                    //有効な成長ボスイベントリスト取得
                    List<MasterDataChallengeEvent> eventList = MasterDataUtil.GetActiveChallengeEvent();
                    if (eventList.Count > 0)
                    {
                        //成長ボス選択画面へ
                        SetChallengeSelectParam(m_SaveSelectChallengeQuestID);
                        bRet = MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_CHALLENGE_SELECT, false, false);
                    }
                    else
                    {
                        //表示できる成長ボスイベントが１つもない場合はイベントマップへ遷移
                        MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY.EVENT);
                        bRet = MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY, false, false);
                    }
                }
                break;
        }
        return bRet;
    }
}

