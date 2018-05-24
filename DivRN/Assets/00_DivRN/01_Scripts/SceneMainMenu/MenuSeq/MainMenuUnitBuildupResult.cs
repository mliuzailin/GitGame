/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuSeqPartyBuildUpResult.cs
	@brief	メインメニューシーケンス：合成枠：強化合成でのリザルト演出
	@author Developer
	@date 	2013/07/03
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
	@brief	メインメニューシーケンス：合成枠：強化合成でのリザルト演出
*/
//----------------------------------------------------------------------------
public class MainMenuUnitBuildupResult : MainMenuSeq
{
    //----------------------------------------------------------------------------
    /*!
		@brief	カットイン情報
	*/
    //----------------------------------------------------------------------------
    const int STEP_000_START = 0;   //!< 強化演出ステップ：
    const int STEP_010_WINDOW_BLACK_IN = 1; //!< 強化演出ステップ：「黒ウィンドウ」イン
    const int STEP_020_FRIEND_IN = 2;   //!< 強化演出ステップ：「Friend Unit」イン
    const int STEP_030_FRIEND_CUTIN = 3;    //!< 強化演出ステップ：フレンドユニットカットイン
    const int STEP_040_MATERIAL_IN = 4; //!< 強化演出ステップ：「Material Unit」イン
    const int STEP_050_MATERIAL_CUTIN = 5;  //!< 強化演出ステップ：素材ユニットカットイン
    const int STEP_060_CUTIN_ANIM_OUT = 6;  //!< 強化演出ステップ：ユニットカットイン終了アニメーション
    const int STEP_070_CUTIN_OUT = 7;   //!< 強化演出ステップ：カットインアウト
    const int STEP_080_WINDOW_STATUS_IN = 8;    //!< 強化演出ステップ：ステータスウィンドウを有効化
    const int STEP_090_LEVELUP = 9; //!< 強化演出ステップ：経験値反映とレベルアップ
    const int STEP_100_LEVELUP_SKILL = 10;  //!< 強化演出ステップ：スキルレベルアップ
    const int STEP_110_LIMIT_OVER = 11; //!< 強化演出ステップ：限界突破
    const int STEP_120_SET_AFTER_STATUS = 12;   //!< 強化演出ステップ：強化後のステータス反映
    const int STEP_999_MAX = 13;    //!< 強化演出ステップ：

    const int LAYER_WINDOW_EMPTY = 0;   //!< レイヤー：ウィンドウ：非表示
    const int LAYER_WINDOW_BLACK = 1;   //!< レイヤー：ウィンドウ：黒地
    const int LAYER_WINDOW_MAX = 2; //!< レイヤー：ウィンドウ：

    const int LAYER_CATEGORY_EMPTY = 0; //!< レイヤー：カテゴリ：非表示
                                        //	const	int		LAYER_CATEGORY_FRIEND		= 1;	//!< レイヤー：カテゴリ：フレンドユニット
    const int LAYER_CATEGORY_MATERIAL = 1;  //!< レイヤー：カテゴリ：素材ユニット
    const int LAYER_CATEGORY_MAX = 2;   //!< レイヤー：カテゴリ：

    const int LAYER_LOGO_EMPTY = 0; //!< レイヤー：ロゴ表示：非表示
    const int LAYER_LOGO_LEVELUP = 1;   //!< レイヤー：ロゴ表示：レベルアップ
    const int LAYER_LOGO_LEVELUP_SKILL = 2; //!< レイヤー：ロゴ表示：スキルレベルアップ
    const int LAYER_LOGO_LIMIT_OVER = 3;    //!< レイヤー：ロゴ表示：限界突破
    const int LAYER_LOGO_MAX = 4;   //!< レイヤー：ロゴ表示：

    const int OBJECT_CHARA_MESH = 0;    // 参照オブジェクト：キャラ情報：キャラメッシュ
    const int OBJECT_WINDOW_ROOT = 1;   // 参照オブジェクト：ウィンドウルート
    const int OBJECT_CUTIN_PARENT = 2;  // 参照オブジェクト：カットインルート
    const int OBJECT_MAX = 3;	// 

    const float EFFECT_SCALE = 3.0f; // 

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    //	private TemplateList< AssetBundleResource >	m_AssetResourceList	= null;	// 事前のAssetBundleダウンロード用リクエストリスト
    private AssetAutoSetCharaMesh m_AssetAutoSet = null;
    // 参照オブジェクト系リンカ
    private GameObject[] m_ReferObject = new GameObject[MainMenuUnitBuildupResult.OBJECT_MAX];
    private MainMenuCutin[] m_CutinParts = null;
    private uint m_LastPartsIdx = 0;
    // シーン遷移関連
    private uint m_WorkStep = STEP_000_START;
    private uint m_WorkStepCount = 0;
    private float m_WorkStepDelta = 0;
    private bool m_WorkStepTriger = true;
    private float m_WorkStepWait = 0.0f;

    private LayoutSwitchManager m_LayoutWindow = null;      //!< レイアウト切り替え管理：ウィンドウ
    private LayoutSwitchManager m_LayoutCategory = null;        //!< レイアウト切り替え管理：カテゴリ

    private AnimationClipShot[] m_AnimationShotLogo = new AnimationClipShot[LAYER_LOGO_MAX];

    private PacketStructUnit m_UnitBefore = null;       //!< ユニット情報：強化前
    private PacketStructUnit m_UnitAfter = null;        //!< ユニット情報：強化後
    private MasterDataParamChara m_UnitMasterData = null;       //!< ユニット情報：マスターデータ

    private bool m_GaugeSEPlayed = false;
    private bool m_GaugeSkipped = false;
    private bool m_LimitOverSkipped = false;


    private float m_EffectPosY = 0.0f;		//!< エフェクト表示位置の高さ

    private UnitNamePanel m_UnitNamePanel = null;
    private UnitResultButton m_UnitResultButton = null;

    /*=======================================================
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

        //----------------------------------------
        // ユニットモデル
        m_ReferObject[OBJECT_CHARA_MESH] = UnityUtil.GetChildNode(gameObject, "CharaMesh");
        // ウィンドウルート
        m_ReferObject[OBJECT_WINDOW_ROOT] = UnityUtil.GetChildNode(gameObject, "WindowBaseRoot");
        //カットインルート
        m_ReferObject[OBJECT_CUTIN_PARENT] = UnityUtil.GetChildNode(gameObject, "CutinParent");
        m_EffectPosY = 0.0f;
        //-------------------------
        // 階層をレイアウト切り替え管理クラスに登録
        // 最初のレイアウトは非表示にしておく
        //-------------------------
        m_LayoutWindow = gameObject.AddComponent<LayoutSwitchManager>();
        m_LayoutWindow.SetLayoutMax(LAYER_WINDOW_MAX);
        m_LayoutWindow.SetLayoutSwitch(LAYER_WINDOW_EMPTY, UnityUtil.GetChildNode(gameObject, "LayerWindowEmpty"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutWindow.SetLayoutSwitch(LAYER_WINDOW_BLACK, UnityUtil.GetChildNode(gameObject, "LayerWindowBlack"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutWindow.AddSwitchRequest(LAYER_WINDOW_EMPTY, true);

        //-------------------------
        // 階層をレイアウト切り替え管理クラスに登録
        // 最初のレイアウトは非表示にしておく
        //-------------------------
        m_LayoutCategory = gameObject.AddComponent<LayoutSwitchManager>();
        m_LayoutCategory.SetLayoutMax(LAYER_CATEGORY_MAX);
        m_LayoutCategory.SetLayoutSwitch(LAYER_CATEGORY_EMPTY, UnityUtil.GetChildNode(gameObject, "LayerCateEmpty"), false, null, typeof(MainMenuDialogSeqWait));
        //m_LayoutCategory.SetLayoutSwitch( LAYER_CATEGORY_FRIEND		, UnityUtil.GetChildNode( gameObject , "LayerCateFriend"	) , false	, null	, typeof(MainMenuDialogSeqWait) );
        m_LayoutCategory.SetLayoutSwitch(LAYER_CATEGORY_MATERIAL, UnityUtil.GetChildNode(gameObject, "LayerCateMaterial"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutCategory.AddSwitchRequest(LAYER_CATEGORY_EMPTY, true);


        GameObject cObjectLogoLevelUp = UnityUtil.GetChildNode(gameObject, "LayerLogoLevelUp");
        GameObject cObjectLogoLevelUpSkill = UnityUtil.GetChildNode(gameObject, "LayerLogoSkillLevelUp");
        GameObject cObjectLogoLimitOver = UnityUtil.GetChildNode(gameObject, "LayerLogoLimitOver");
        m_AnimationShotLogo[LAYER_LOGO_LEVELUP] = cObjectLogoLevelUp.GetComponent<AnimationClipShot>();
        m_AnimationShotLogo[LAYER_LOGO_LEVELUP_SKILL] = cObjectLogoLevelUpSkill.GetComponent<AnimationClipShot>();
        m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER] = cObjectLogoLimitOver.GetComponent<AnimationClipShot>();

        Vector3 vLogoRootPos = new Vector3(0.0f, m_EffectPosY, 0.0f);
        GameObject cObjLogoRoot = UnityUtil.GetChildNode(gameObject, "LayerLogoRoot");
        cObjLogoRoot.transform.position = vLogoRootPos;

        m_UnitNamePanel = m_CanvasObj.GetComponentInChildren<UnitNamePanel>();
        m_UnitResultButton = m_CanvasObj.GetComponentInChildren<UnitResultButton>();
        m_UnitResultButton.Button1Text = "続けて強化する";
        m_UnitResultButton.IsActiveButton1 = false;
        m_UnitResultButton.DidSelectButton1 = SwitchBuildup;
        m_UnitResultButton.Button2Text = "別のユニットを強化";
        m_UnitResultButton.IsActiveButton2 = false;
        m_UnitResultButton.DidSelectButton2 = SwitchUnitSelect;
        m_UnitResultButton.IsActiveSkip = true;
        m_UnitResultButton.DidSelectSkip = ProductionAllSkip;

        //----------------------------------------
        // パッチ処理を行わないようにする.
        //----------------------------------------
        MainMenuManager.Instance.m_ResumePatchUpdateRequest = false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    public new void Update()
    {
        //--------------------------------
        // マネージャが動作可能状態になったらレイアウト管理クラスに開始許可を出す
        //--------------------------------
        if (m_LayoutWindow != null
        && m_LayoutWindow.m_LayoutStartOK == false
        )
        {
            m_LayoutWindow.SetLayoutStartOK();
        }
        if (m_LayoutCategory != null
        && m_LayoutCategory.m_LayoutStartOK == false
        )
        {
            m_LayoutCategory.SetLayoutStartOK();
        }

        //----------------------------------------
        // 固定処理
        // 管理側からの更新許可やフェード待ち含む。
        //----------------------------------------
        if (PageSwitchUpdate() == false)
        {
            return;
        }

        //----------------------------------------
        // ステップで処理分岐
        //----------------------------------------
        uint unStepNow = m_WorkStep;
        switch (m_WorkStep)
        {
            case STEP_000_START: ExecStep_000_Start(); break;   // 強化演出ステップ：
            case STEP_010_WINDOW_BLACK_IN: ExecStep_010_WindowBlackIn(); break; // 強化演出ステップ：「黒ウィンドウ」イン
            case STEP_020_FRIEND_IN: ExecStep_020_FriendIn(); break;    // 強化演出ステップ：「Friend Unit」イン
            case STEP_030_FRIEND_CUTIN: ExecStep_030_FriendCutin(); break;  // 強化演出ステップ：フレンドユニットカットイン
            case STEP_040_MATERIAL_IN: ExecStep_040_MaterialIn(); break;    // 強化演出ステップ：「Material Unit」イン
            case STEP_050_MATERIAL_CUTIN: ExecStep_050_MaterialCutin(); break;  // 強化演出ステップ：素材ユニットカットイン
            case STEP_060_CUTIN_ANIM_OUT: ExecStep_060_CutinUnitAnimOut(); break;   // 強化演出ステップ：ユニットカットイン終了アニメーション
            case STEP_070_CUTIN_OUT: ExecStep_070_CutinOut(); break;    // 強化演出ステップ：カットインアウト
            case STEP_080_WINDOW_STATUS_IN: ExecStep_080_WindowStatusIn(); break;   // 強化演出ステップ：ステータスウィンドウを有効化
            case STEP_090_LEVELUP: ExecStep_090_LevelUp(); break;   // 強化演出ステップ：経験値反映とレベルアップ
            case STEP_100_LEVELUP_SKILL: ExecStep_090_LevelUpSkill(); break;    // 強化演出ステップ：スキルレベルアップ
            case STEP_110_LIMIT_OVER: ExecStep_110_LimitOver(); break;  // 強化演出ステップ：限界突破
            case STEP_120_SET_AFTER_STATUS: ExecStep_120_SetAfterStatus(); break;   // 強化演出ステップ：強化後のステータス反映
            case STEP_999_MAX: ExecStep_999_Max(); break;   // 強化演出ステップ：
        }
        if (m_WorkStep != unStepNow)
        {
            m_WorkStepTriger = true;
            m_WorkStepCount = 0;
            m_WorkStepDelta = 0;
        }
        else
        {
            m_WorkStepTriger = false;
        }


        //--------------------------------
        // スキップボタンを押したら演出をスキップする
        //--------------------------------
        //ProductionAllSkip();

    }


    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：MainMenuSeq：ページ無効化直後に走るイベント
		@note	処理中を返す間中はページを次のページが発生せずに処理を続ける
		@retval	[ true:処理中 / false:処理完遂 ]
	*/
    //----------------------------------------------------------------------------
    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        //---------------------
        // 基底呼出し
        //---------------------
        base.PageSwitchEventDisableAfter(eNextMainMenuSeq);

        //----------------------------------------
        // リソース破棄
        //----------------------------------------
        if (m_CutinParts != null)
        {
            for (int i = 0; i < m_CutinParts.Length; i++)
            {

                Destroy(m_CutinParts[i].cutinObj);
                m_CutinParts[i] = null;
            }
            m_CutinParts = null;
        }

        //---------------------
        // オブジェクト破棄
        //---------------------
        if (m_AssetAutoSet != null)
        {
            m_AssetAutoSet.DestroyCharaMesh();
        }

        //---------------------
        // リソース破棄
        // 
        // 演出で大量のキャラクターの読み込みが走っているため、
        // 明示的に破棄しないとメモリ不足でハングする危険性がある
        //---------------------
        {
            //			AssetBundleManager.Instance.ClearAssetBundleRequest();
            UnityUtil.ResourceRefresh();
        }
        return false;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	基底継承：MainMenuSeq：ページ有効化直前に走るイベント
		@note	処理中を返す間中はページを次のページが移行せずに処理を続ける
		@retval	[ true:処理中 / false:処理完遂 ]
	*/
    //----------------------------------------------------------------------------
    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        //--------------------------------
        // 基底呼出し
        //--------------------------------
        base.PageSwitchEventEnableBefore();

        //--------------------------------
        // 関連するリソースをダウンロードしてキャッシュに載せる。
        // 
        // キャッシュに載りさえすれば表示までの負荷が収まるので、
        // キャッシュに載せる作業が終わったらリクエストクラスごと破棄する。
        //
        // 実際のメッシュの読み込みと表示処理に関しては、
        // ページを開く処理側で行うことで作業を切り分けている
        //--------------------------------

        //--------------------------------
        // 関連するリソースを読み込みリクエスト発行
        //--------------------------------
        //		if( m_AssetResourceList == null )
        //		{
        //--------------------------------
        // 関連するキャラのIDリストを生成
        //--------------------------------
        TemplateList<uint> cUnitIDList = new TemplateList<uint>();
        if (MainMenuParam.m_BlendBuildUpParts != null)
        {
            for (int i = 0; i < MainMenuParam.m_BlendBuildUpParts.m_BufferSize; i++)
            {
                cUnitIDList.Add(MainMenuParam.m_BlendBuildUpParts[i].id);
            }
        }
        return false;
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

        //--------------------------------
        // 情報破棄
        //
        // リソースの参照が残ると破棄処理で抜けが生じるので
        // ページを無効にしたタイミングでも破棄するよう対応してる
        //--------------------------------
        //--------------------------------
        // オブジェクト破棄
        //--------------------------------
        if (m_AssetAutoSet != null)
        {
            m_AssetAutoSet.DestroyCharaMesh();
        }

        //--------------------------------
        // 進行情報クリア
        //--------------------------------
        m_WorkStep = STEP_000_START;
        m_WorkStepDelta = 0;
        m_WorkStepTriger = true;


        //--------------------------------
        // v340 Bugweb:5925 
        // アチーブメント達成演出をスキップされる恐れがあるのでアチーブメント取得フラグONにしとく。
        //  フラグをONにすることで、次回、ミッション画面に遷移したとき、
        //  ローカルで保持してるアチーブメントマスタのリスト情報を破棄し、新しく一からマスタを取得し直すようになる。
        //--------------------------------
        if (MainMenuParam.m_AchievementListGet == false)
        {
            MainMenuParam.m_AchievementListGet = true;
        }

        //----------------------------------------
        // 強化対象ユニットを保持
        //----------------------------------------
        MainMenuParam.m_BuildupBaseUnitUniqueId = MainMenuParam.m_BlendBuildUpUnitPrev.unique_id;

        //--------------------------------
        // 強化前後の情報を保持しておく
        //--------------------------------
        m_UnitBefore = new PacketStructUnit();
        m_UnitBefore.Copy(MainMenuParam.m_BlendBuildUpUnitPrev);
        m_UnitAfter = new PacketStructUnit();
        m_UnitAfter.Copy(MainMenuParam.m_BlendBuildUpUnitAfter);
        m_UnitMasterData = MasterDataUtil.GetCharaParamFromID(m_UnitAfter.id);

        m_UnitNamePanel.setup(m_UnitMasterData);

        m_UnitBefore.add_hp = m_UnitAfter.add_hp;
        m_UnitBefore.add_pow = m_UnitAfter.add_pow;

        #region ==== 最大スキルレベルチェック ====
        MasterDataSkillLimitBreak cSkillLimitBreak = MasterDataUtil.GetLimitBreakSkillParamFromID(m_UnitMasterData.skill_limitbreak);
        if (cSkillLimitBreak != null)
        {
            if (cSkillLimitBreak.level_max <= m_UnitAfter.limitbreak_lv)
            {
                m_UnitAfter.limitbreak_lv = (uint)cSkillLimitBreak.level_max;
            }
        }
        #endregion

        //--------------------------------
        // リンク先のユニット情報を取得する
        //--------------------------------
        PacketStructUnit cLinkUnit = CharaLinkUtil.GetLinkUnit(m_UnitBefore.link_unique_id);
        //----------------------------------------
        // 初期情報としてリンク前のユニット情報を表示
        //----------------------------------------
        // 表示ユニット情報を設定
        MainMenuParam.SetCharaDetailParam(m_UnitBefore, cLinkUnit);

        //----------------------------------------
        // キャラメッシュ生成
        //----------------------------------------
        {
            if (m_AssetAutoSet == null)
            {
                m_AssetAutoSet = m_ReferObject[OBJECT_CHARA_MESH].AddComponent<AssetAutoSetCharaMesh>();
                m_AssetAutoSet.m_AssetBundleMeshScaleUp = true;
                m_AssetAutoSet.m_AssetBundleMeshPosition = true;
            }
            m_AssetAutoSet.SetCharaID(m_UnitMasterData.fix_id, true);
        }
        if (MainMenuParam.m_BlendBuildUpParts != null
        && MainMenuParam.m_BlendBuildUpParts.m_BufferSize > 0
        )
        {
            m_CutinParts = new MainMenuCutin[MainMenuParam.m_BlendBuildUpParts.m_BufferSize];
            for (int i = 0; i < MainMenuParam.m_BlendBuildUpParts.m_BufferSize; i++)
            {
                m_CutinParts[i] = new MainMenuCutin();
                m_CutinParts[i].Setup(m_ReferObject[OBJECT_CUTIN_PARENT], MainMenuParam.m_BlendBuildUpParts[i].id, MainMenuDefine.CUTIN_OBJ_TYPE_COST);
            }
        }
        //--------------------------------
        // カットイン演出をスキップする対応
        // ポイントショップの限界突破対応の為追加
        //--------------------------------
        if (MainMenuParam.m_BlendBuildUpParts == null
        || MainMenuParam.m_BlendBuildUpParts.m_BufferSize <= 0
        )
        {
            // ステータスウィンドウ表示から始める
            m_WorkStep = STEP_080_WINDOW_STATUS_IN;
        }

        //----------------------------------------
        // とりあえず非表示へ
        //----------------------------------------
        m_LayoutCategory.SetLayoutAllDisable();
        m_LayoutWindow.SetLayoutAllDisable();

        m_UnitResultButton.IsActiveButton1 = false;
        m_UnitResultButton.IsActiveButton2 = false;

        for (int i = 0; i < m_AnimationShotLogo.Length; i++)
        {
            if (m_AnimationShotLogo[i] == null)
                continue;
            UnityUtil.SetObjectEnabled(m_AnimationShotLogo[i].gameObject, false);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	演出ALLスキップ機能
		@note	
	*/
    //----------------------------------------------------------------------------
    void ProductionAllSkip()
    {
        //if( m_DetailBase.SkipButtonUpdate() == MainMenuPartsCharaDetailBase.UPDATE_RESULT.RESULT_SKIP )
        {
            // 経験値ゲージエフェクト削除
            //EffectManager.Instance.ReleaseEffect( ref m_GaugeEffect );
            //m_GaugeEffect = null;

            // カテゴリ非表示
            m_LayoutCategory.AddSwitchRequest(LAYER_CATEGORY_EMPTY, false);

            m_LayoutWindow.AddSwitchRequest(LAYER_WINDOW_EMPTY, false);

            // カットイン用オブジェクトを非表示
            m_ReferObject[OBJECT_CUTIN_PARENT].SetActive(false);

            //--------------------------------
            // リンク先のユニット情報を取得する
            //--------------------------------
            PacketStructUnit cLinkUnit = CharaLinkUtil.GetLinkUnit(m_UnitAfter.link_unique_id);

            //----------------------------------------
            // 初期情報としてリンク前のユニット情報を表示
            //----------------------------------------
            // 表示ユニット情報を設定
            MainMenuParam.SetCharaDetailParam(m_UnitAfter, cLinkUnit);

            // 強化後のステータス反映ステップへ移行
            m_WorkStep = STEP_120_SET_AFTER_STATUS;
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_000_Start()
    {
        //----------------------------------------
        // 特にやることないんで無条件で次へ
        //----------------------------------------
        m_WorkStep++;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：「黒ウィンドウ」イン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_010_WindowBlackIn()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            m_LayoutWindow.AddSwitchRequest(LAYER_WINDOW_BLACK, false);
        }

        //-------------------
        // 更新完遂待ち
        //-------------------
        m_WorkStepDelta += Time.deltaTime;
        if (m_WorkStepDelta >= MainMenuDefine.UPDATE_WAIT_TIME)
        {
            //新仕様ではフレンドがいないので素材演出へ
            m_WorkStep = STEP_040_MATERIAL_IN;

        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：「Friend Unit」イン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_020_FriendIn()
    {
        //-------------------
        // 更新完遂待ち
        //-------------------
        m_WorkStepDelta += Time.deltaTime;
        if (m_WorkStepDelta >= MainMenuDefine.UPDATE_WAIT_TIME)
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：フレンドユニットカットイン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_030_FriendCutin()
    {
        m_WorkStep++;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：「Material Unit」イン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_040_MaterialIn()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            m_LayoutCategory.AddSwitchRequest(LAYER_CATEGORY_MATERIAL, false);
        }

        //-------------------
        // 更新完遂待ち
        //-------------------
        m_WorkStepDelta += Time.deltaTime;
        if (m_WorkStepDelta >= MainMenuDefine.UPDATE_WAIT_TIME)
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：素材ユニットカットイン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_050_MaterialCutin()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
        }

        //----------------------------------------
        // AssetBundleの反映前チェック
        //----------------------------------------
        bool bAssetBundleAssignEnd = true;
        for (int i = 0; i < m_CutinParts.Length; i++)
        {
            if (m_CutinParts[i].isTexture)
            {
                bAssetBundleAssignEnd = false;
                break;
            }
        }
        if (bAssetBundleAssignEnd == false)
        {
            //----------------------------------------
            // カットイン準備
            //----------------------------------------
            for (int i = 0; i < m_CutinParts.Length; i++)
            {
                if (m_CutinParts[i].SetupCutin(true) == false)
                {
                    return;
                }
            }
        }

        //-------------------
        // 次が発行可能かチェック
        //-------------------
        bool bCutinAnimationEnd = true;
        for (int i = 0; i < m_CutinParts.Length; i++)
        {
            if (m_CutinParts[i].isBaseAnimPlay == false)
            {
                continue;
            }

            bCutinAnimationEnd = false;
            break;
        }
        bool bNextCutin = bCutinAnimationEnd;
#if true
        {
            float fCutinCycle = 0.5f;
            int nCyclePrev = (int)(m_WorkStepDelta / fCutinCycle);
            m_WorkStepDelta += Time.deltaTime;
            int nCycleAfter = (int)(m_WorkStepDelta / fCutinCycle);
            if (nCycleAfter != nCyclePrev)
            {
                bNextCutin = true;
            }
        }
#endif
        if (bNextCutin == false)
        {
            return;
        }


        //-------------------
        // 次の素材がいるならカットイン呼び出し
        //-------------------
        if (m_WorkStepCount < m_CutinParts.Length)
        {
            // 1つ前のキャラカットイン終了アニメーション
            int nWorkStepPrev = (int)m_WorkStepCount - 1;
            if (nWorkStepPrev >= 0)
            {
                m_CutinParts[nWorkStepPrev].StartAnimUnit(MainMenuDefine.ANIM_COST_UNIT_OUT);
            }

            m_CutinParts[m_WorkStepCount].StartAnimAll(MainMenuDefine.ANIM_COST_UNIT_IN);

            // 最後のアクセス番号を保存
            m_LastPartsIdx = m_WorkStepCount;

            m_WorkStepCount++;

            //----------------------------------------
            // エフェクトを発生
            //----------------------------------------
            Vector3 vEffectPosition = m_ReferObject[OBJECT_CHARA_MESH].transform.position;
            vEffectPosition.y = m_EffectPosY;
            vEffectPosition.z += GlobalDefine.UNITDETAIL_EFFECT_OFFSET_Z;
            GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vEffectPosition, SceneObjReferMainMenu.Instance.m_EffectEvolCutinParts, gameObject);
            UnityUtil.SetSortingOrder(_effObj, "EFFECT");
            _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);

            //----------------------------------------
            // SE再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.SE_MM_D02_MATERIAL_UNIT);
        }
        else
        {
            //-------------------
            // 次の素材がいないなら完遂
            //-------------------
            if (bCutinAnimationEnd == true)
            {
                m_WorkStep++;
                return;
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：ユニットカットイン：終了アニメーション
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_060_CutinUnitAnimOut()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            m_CutinParts[m_LastPartsIdx].StartAnimUnit(MainMenuDefine.ANIM_COST_UNIT_OUT);
        }

        //-------------------
        // アニメーション終了待ち
        //-------------------
        if (m_CutinParts[m_LastPartsIdx].isUnitAnimPlay == false)
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リンク演出ステップ：カットインアウト
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_070_CutinOut()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {


            for (int i = 0; i < m_CutinParts.Length; i++)
            {
                UnityUtil.SetObjectEnabled(m_CutinParts[i].cutinObj, false);
            }

            m_LayoutCategory.AddSwitchRequest(LAYER_CATEGORY_EMPTY, false);
        }

        //-------------------
        // 更新完遂待ち
        //-------------------
        m_WorkStepDelta += Time.deltaTime;
        if (m_WorkStepDelta >= MainMenuDefine.UPDATE_WAIT_TIME)
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：ステータスウィンドウを有効化
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_080_WindowStatusIn()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            m_LayoutWindow.AddSwitchRequest(LAYER_WINDOW_EMPTY, false);
        }

        if (m_LayoutWindow.ChkLayoutAnimationFinish(LAYER_WINDOW_EMPTY) == true
        )
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：経験値反映とレベルアップ
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_090_LevelUp()
    {
        //----------------------------------------
        // １．演出中なら待つ
        //		※演出中とか関係なしにスキップ操作は可能（即時スキップではなくフラグ保持程度）
        //		
        // ２．演出が終わったら次のサイクル更新へ
        //　　	１．既に補間処理が終わってるなら次のステップへ移行
        //				※レベルが最大に達した
        //				※経験値の補間が完遂した
        //		２．補間処理が終わってないなら今回フレームでの更新処理を実行
        //				１．今回フレームでの更新量を算出して加算
        //				２．UI見た目更新
        //				３．更新によってレベルが上がるなら演出発行
        //----------------------------------------


        //----------------------------------------
        // トリガー処理
        //----------------------------------------
        if (m_WorkStepTriger == true)
        {
            //----------------------------------------
            // SE再生前状態を指定
            //----------------------------------------
            m_GaugeSEPlayed = false;

            //----------------------------------------
            // スキップフラグをオフ
            //----------------------------------------
            m_GaugeSkipped = false;
        }

        //----------------------------------------
        // アニメーション待ち
        //----------------------------------------
        if (m_AnimationShotLogo[LAYER_LOGO_LEVELUP] != null
        && m_AnimationShotLogo[LAYER_LOGO_LEVELUP].ChkAnimationPlaying() == true
        )
        {
            return;
        }

        //----------------------------------------
        // 経験値総量の補間残量を算出
        //----------------------------------------
        int nRemainEXP = (int)(m_UnitAfter.exp - m_UnitBefore.exp);

        //----------------------------------------
        // 経験値反映の補間処理が完遂しているかチェック
        //----------------------------------------
        {
            bool bLevelUpStepFinish = false;
            //----------------------------------------
            // 経験値量が補間完遂の状態と一致するなら補間完了とみなして次へ
            //----------------------------------------
            if (nRemainEXP <= 0)
            {
                bLevelUpStepFinish = true;
            }
            //----------------------------------------
            // レベルがMAXなら処理終わりとみなして次へ
            //----------------------------------------
            if (m_UnitBefore.level >= m_UnitMasterData.level_max)
            {
                bLevelUpStepFinish = true;
            }
            //----------------------------------------
            // スキップボタンを押してたら次へ
            //----------------------------------------
            if (m_GaugeSkipped == true)
            {
                bLevelUpStepFinish = true;
            }

            //----------------------------------------
            // なんらかの理由で処理が完遂しているならUI調整して次のステップへ
            //----------------------------------------
            if (bLevelUpStepFinish == true)
            {
                // 強化後の値に変更する
                m_UnitBefore.level = m_UnitAfter.level;
                m_UnitBefore.exp = m_UnitAfter.exp;
                //----------------------------------------
                // 強化後のユニット情報を表示
                //----------------------------------------
                MainMenuParam.m_CharaDetailCharaLevel = m_UnitBefore.level;         //!< キャラ詳細：選択キャラレベル
                MainMenuParam.m_CharaDetailCharaExp = m_UnitBefore.exp;             //!< キャラ詳細：選択キャラ経験値

                //----------------------------------------
                // 表示は切っておく
                //----------------------------------------
                if (m_AnimationShotLogo[LAYER_LOGO_LEVELUP] != null)
                {
                    UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LEVELUP].gameObject, false);
                }

                //----------------------------------------
                // スキップボタンを表示オフ
                //----------------------------------------
                //m_DetailBase.SetSkipButtonEnable( false );

                //----------------------------------------
                // 次のステップへ遷移
                //----------------------------------------
                m_WorkStep++;

                return;
            }
        }

        //----------------------------------------
        // 次のレベルまでの情報を算出
        //----------------------------------------
        int nExpLevelNow = CharaUtil.GetStatusValue(m_UnitMasterData, (int)m_UnitBefore.level + 0, CharaUtil.VALUE.EXP);
        int nExpLevelNext = CharaUtil.GetStatusValue(m_UnitMasterData, (int)m_UnitBefore.level + 1, CharaUtil.VALUE.EXP);
        int nExpToNext = nExpLevelNext - nExpLevelNow;
        //int nExpToNextCycle = (int)((nExpToNext / 15.0f) * TimeUtil.GetDeltaTimeRate());

        //----------------------------------------
        // 経験値反映処理が最後付近かを調べる
        //----------------------------------------
        uint nLevelUpCnt = 0;
        if (nExpToNext > 0)
        {
            // 経験値の設定によっては0除算が発生する為nExpToNextが0以上の場合のみ値を入れる
            nLevelUpCnt = (uint)(nRemainEXP / nExpToNext);
        }
        uint nExpToNextCycle = 0;

        if (nLevelUpCnt <= 1)
        {
            //１サイクルをこまかくする
            nExpToNextCycle = (uint)((nExpToNext / 15.0f) * TimeUtil.GetDeltaTimeRate());
        }
        else
        {
            //１サイクルを大きくする
            nExpToNextCycle = (uint)((nExpToNext / 2.0f) * TimeUtil.GetDeltaTimeRate());
        }

        if (nExpToNextCycle <= 0) { nExpToNextCycle = 1; }

        //----------------------------------------
        // １サイクル分の経験値を有しているかどうかで処理分岐
        //----------------------------------------
        bool bLevelUp = false;
        {
            if (nExpToNextCycle <= nRemainEXP)
            {
                //----------------------------------------
                // 次のレベルまでの経験値を有している場合
                // →１サイクル分ずつ成長。今回のサイクルでレベルが上がりそうならレベルが上がるちょうどの数値で寸止めする
                //----------------------------------------
                if (m_UnitBefore.exp + nExpToNextCycle >= nExpLevelNext)
                {
                    //----------------------------------------
                    // レベルアップ成立
                    //----------------------------------------
                    m_UnitBefore.exp = (uint)nExpLevelNext;
                    m_UnitBefore.level++;
                    bLevelUp = true;
                }
                else
                {
                    m_UnitBefore.exp += (uint)nExpToNextCycle;
                }
            }
            else
            {
                //----------------------------------------
                // 次のレベルまでの経験値を有していない場合
                // →残量を足してレベルがあがるかだけチェック
                //----------------------------------------
                if (m_UnitBefore.exp + nRemainEXP >= nExpLevelNext)
                {
                    //----------------------------------------
                    // レベルアップ成立
                    //----------------------------------------
                    m_UnitBefore.exp = (uint)nExpLevelNext;
                    m_UnitBefore.level++;
                    bLevelUp = true;
                }
                else
                {
                    m_UnitBefore.exp += (uint)nRemainEXP;
                }
            }
        }

        //----------------------------------------
        // ゲージと数値の表示を更新
        //----------------------------------------
        if (bLevelUp == true)
        {
            //----------------------------------------
            // 強化後のユニット情報を表示
            //----------------------------------------
            MainMenuParam.m_CharaDetailCharaLevel = m_UnitBefore.level;         //!< キャラ詳細：選択キャラレベル
            MainMenuParam.m_CharaDetailCharaExp = m_UnitBefore.exp;             //!< キャラ詳細：選択キャラ経験値

            //----------------------------------------
            // レベルアップのロゴ表示
            //----------------------------------------
            if (m_AnimationShotLogo[LAYER_LOGO_LEVELUP] != null)
            {
                UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LEVELUP].gameObject, true);
                m_AnimationShotLogo[LAYER_LOGO_LEVELUP].PlayAnimation(AnimationClipShot.SHOT_ANIM.PLAY);
            }

            //----------------------------------------
            // エフェクトを出す
            //----------------------------------------
            if (SceneObjReferMainMenu.Instance.m_EffectLevelUp != null)
            {
                Vector3 vPos = new Vector3(0, m_EffectPosY, 0);
                GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vPos, SceneObjReferMainMenu.Instance.m_EffectLevelUp, gameObject);
                UnityUtil.SetSortingOrder(_effObj, "EFFECT");
                _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);
            }

            //----------------------------------------
            // レベルアップボイス再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_LEVELUP);

            //----------------------------------------
            // SE再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.SE_MM_D04_LEVEL_UP);

            //----------------------------------------
            // ゲージ音が再生されるようにクリア
            //----------------------------------------
            m_GaugeSEPlayed = false;
        }
        else
        {
            //int		nExpToNextLVRemain	= (int)( nExpLevelNext - m_UnitBefore.exp	);
            //int		nExpToNextLVMax		= (int)( nExpLevelNext - nExpLevelNow		);
            //float	fExpToNextLVRate	= (float)( (float)( nExpToNextLVMax - nExpToNextLVRemain ) / (float)nExpToNextLVMax );

            //----------------------------------------
            // ゲージSEとゲージエフェクトを再生
            //----------------------------------------
            if (m_GaugeSEPlayed == false)
            {
                m_GaugeSEPlayed = true;

                //----------------------------------------
                // SE再生
                //----------------------------------------
                SoundUtil.PlaySE(SEID.SE_MM_B01_EXP_GAUGE);
            }
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：スキルレベルアップ
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_090_LevelUpSkill()
    {
        //----------------------------------------
        // トリガー処理
        //----------------------------------------
        if (m_WorkStepTriger == true)
        {
            //----------------------------------------
            // 適当に待つ
            //----------------------------------------
            m_WorkStepWait = 0.5f;
        }

        //-------------------
        // アニメーション待ち
        //-------------------
        if (m_AnimationShotLogo[LAYER_LOGO_LEVELUP_SKILL] != null
        && m_AnimationShotLogo[LAYER_LOGO_LEVELUP_SKILL].ChkAnimationPlaying() == true
        )
        {
            return;
        }

        //-------------------
        // 強化前後の経験値差を算出
        //-------------------
        int nRemainSkillLevel = (int)(m_UnitAfter.limitbreak_lv - m_UnitBefore.limitbreak_lv);
        if (nRemainSkillLevel < 0)
        {
            Debug.LogError("SkillLevel Remain Error!");
        }
        if (nRemainSkillLevel == 0)
        {
            //-------------------
            // 処理終わってるなら次へ
            //-------------------
            m_WorkStep++;

            //-------------------
            // 表示は切っておく
            //-------------------
            if (m_AnimationShotLogo[LAYER_LOGO_LEVELUP_SKILL] != null)
            {
                UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LEVELUP_SKILL].gameObject, false);
            }
            return;
        }

        //-------------------
        // 待ち時間待つ
        //-------------------
        if (m_WorkStepWait > 0.0f)
        {
            m_WorkStepWait -= Time.deltaTime;
            return;
        }

        //-------------------
        // スキルレベルアップ確定
        //-------------------
        {
            m_UnitBefore.limitbreak_lv++;

            //-------------------
            // 表示するユニット情報を更新
            //-------------------
            MainMenuParam.m_CharaDetailCharaLBSkillLV = m_UnitBefore.limitbreak_lv; //!< キャラ詳細：選択キャラリミットブレイクスキルレベル

            //-------------------
            // レベルアップ演出
            //-------------------
            UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LEVELUP_SKILL].gameObject, true);
            m_AnimationShotLogo[LAYER_LOGO_LEVELUP_SKILL].PlayAnimation(AnimationClipShot.SHOT_ANIM.PLAY);

            //----------------------------------------
            // エフェクトを出す
            //----------------------------------------
            if (SceneObjReferMainMenu.Instance.m_EffectSkillLevelUp != null)
            {
                Vector3 vPos = new Vector3(0, m_EffectPosY, 0);
                GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vPos, SceneObjReferMainMenu.Instance.m_EffectSkillLevelUp, gameObject);
                UnityUtil.SetSortingOrder(_effObj, "EFFECT");
                _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);
            }

            //----------------------------------------
            // レベルアップボイス再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_SKILLUP);

            //----------------------------------------
            // SE再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.SE_MM_D04_LEVEL_UP);

        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：限界突破
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_110_LimitOver()
    {
        //----------------------------------------
        // トリガー処理
        //----------------------------------------
        if (m_WorkStepTriger == true)
        {
            //----------------------------------------
            // 適当に待つ
            //----------------------------------------
            m_WorkStepWait = 0.5f;
        }

        //-------------------
        // 強化前後の経験値差を算出
        //-------------------
        int nRemainLimitOverLevel = (int)(m_UnitAfter.limitover_lv - m_UnitBefore.limitover_lv);
        if (nRemainLimitOverLevel < 0)
        {
            Debug.LogError("LimitOver Remain Error!");
        }

        // 限界突破演出中にスキップボタンが押された場合の処理
        if (m_LimitOverSkipped == true)
        {
            m_LimitOverSkipped = false;

            // 強化後の値に変更する
            m_UnitBefore.limitover_lv = m_UnitAfter.limitover_lv;
            //----------------------------------------
            // 強化後のユニット情報を表示
            //----------------------------------------
            MainMenuParam.m_CharaDetailCharaLOverLV = m_UnitBefore.limitover_lv;            //!< キャラ詳細：選択キャラ限界突破レベル
                                                                                            // ユニットデータ設定
                                                                                            //			m_DetailBase.SetupUnitData( MainMenuPartsCharaDetailBase.SETUP_TYPE.SETUP_STATUS );

            //----------------------------------------
            // 表示は切っておく
            //----------------------------------------
            if (m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER] != null)
            {
                UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER].gameObject, false);
            }

            //----------------------------------------
            // スキップボタンを表示オフ
            //----------------------------------------
            //m_DetailBase.SetSkipButtonEnable( false );

            //----------------------------------------
            // 次のステップへ遷移
            //----------------------------------------
            m_WorkStep++;

            return;
        }

        //-------------------
        // アニメーション待ち
        //-------------------
        if (m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER] != null
        && m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER].ChkAnimationPlaying() == true
        )
        {
            return;
        }

        if (nRemainLimitOverLevel == 0)
        {
            //-------------------
            // 処理終わってるなら次へ
            //-------------------
            m_WorkStep++;

            MainMenuParam.m_CharaDetailCharaLOverLV = m_UnitAfter.limitover_lv;
            m_LimitOverSkipped = false;

            //-------------------
            // 表示は切っておく
            //-------------------
            if (m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER] != null)
            {
                UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER].gameObject, false);
            }
            return;
        }

        //-------------------
        // 待ち時間待つ
        //-------------------
        if (m_WorkStepWait > 0.0f)
        {
            m_WorkStepWait -= Time.deltaTime;
            return;
        }

        //-------------------
        // 限界突破
        //-------------------
        {
            m_UnitBefore.limitover_lv++;
            //-------------------
            // 表示するユニット情報を更新
            //-------------------
            MainMenuParam.m_CharaDetailCharaLOverLV = m_UnitBefore.limitover_lv;    //!< キャラ詳細：選択キャラリミットブレイクスキルレベル

            //-------------------
            // レベルアップ演出
            //-------------------
            UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER].gameObject, true);
            m_AnimationShotLogo[LAYER_LOGO_LIMIT_OVER].PlayAnimation(AnimationClipShot.SHOT_ANIM.PLAY);

            //----------------------------------------
            // エフェクトを出す
            //----------------------------------------
            if (SceneObjReferMainMenu.Instance.m_EffectSkillLevelUp != null)
            {
                Vector3 vPos = new Vector3(0, m_EffectPosY, 0);
                GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vPos, SceneObjReferMainMenu.Instance.m_EffectSkillLevelUp, gameObject);
                UnityUtil.SetSortingOrder(_effObj, "EFFECT");
                _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);
            }

            //----------------------------------------
            // レベルアップボイス再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_LIMITOVER);

            //----------------------------------------
            // SE再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.SE_MM_D04_LEVEL_UP);
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：強化後のステータス反映
		@note	強化後のステータスとリンクポイントの値を反映させる
	*/
    //----------------------------------------------------------------------------
    void ExecStep_120_SetAfterStatus()
    {
        // リンクポイントを最新にする為に、親をベースにしている場合は更新する
        if (m_UnitAfter.link_info == (uint)ServerDataDefine.CHARALINK_TYPE.CHARALINK_TYPE_BASE)
        {
            MainMenuParam.m_CharaDetailCharaLinkPoint = m_UnitAfter.link_point;
        }
        // ユニットデータ設定
        //		m_DetailBase.SetStatusType( MainMenuPartsCharaDetailBase.STATUS_TYPE.STATUS_DEFAULT );
        //		m_DetailBase.SetupUnitData( MainMenuPartsCharaDetailBase.SETUP_TYPE.SETUP_ALL );
        //-------------------
        // 処理終わってるなら次へ
        //-------------------
        m_WorkStep++;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強化演出ステップ：入力待ち
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_999_Max()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            //----------------------------------------
            // スキップボタンを無効化
            //----------------------------------------
            m_UnitResultButton.IsActiveButton1 = true;
            m_UnitResultButton.IsActiveButton2 = true;
            m_UnitResultButton.IsActiveSkip = false;
        }

        // スキップボタン非表示
        //m_DetailBase.SetSkipButtonEnable( false );
        //        bool ButtonClipFinish = false;
    }


    private void SwitchBuildup()
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false, true);
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
    }

    private void SwitchUnitSelect()
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuParam.m_BuildupBaseUnitUniqueId = 0;
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false, true);
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
    }
}

