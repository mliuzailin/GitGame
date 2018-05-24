/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuUnitLinkResult.cs
	@brief	メインメニューシーケンス：リンク枠：リンク演出
	@author Developer
	@date 	2015/10/05
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
	@brief	メインメニューシーケンス：リンク枠：リンクでのリザルト演出
*/
//----------------------------------------------------------------------------
public class MainMenuUnitLinkResult : MainMenuSeq
{
    //----------------------------------------------------------------------------
    /*!
		@brief	カットイン情報
	*/
    //----------------------------------------------------------------------------
    const int STEP_000_START = 0;   //!< 演出ステップ：
    const int STEP_010_WINDOW_BLACK_IN = 1; //!< 演出ステップ：「黒ウィンドウ」イン
    const int STEP_020_LINK_IN = 2; //!< 演出ステップ：「Link Unit」イン
    const int STEP_030_LINK_CUTIN = 3;  //!< 演出ステップ：リンクユニットカットイン
    const int STEP_040_MATERIAL_IN = 4; //!< 演出ステップ：「Material Unit」イン
    const int STEP_050_MATERIAL_CUTIN = 5;  //!< 演出ステップ：素材ユニットカットイン
    const int STEP_060_CUTIN_ANIM_OUT = 6;  //!< 演出ステップ：ユニットカットイン終了アニメーション
    const int STEP_070_CUTIN_OUT = 7;   //!< 演出ステップ：カットインアウト
    const int STEP_080_WINDOW_STATUS_IN = 8;    //!< 演出ステップ：ステータスウィンドウを有効化
    const int STEP_090_LINKON = 9;  //!< 演出ステップ：リンク
    const int STEP_999_MAX = 10;    //!< 演出ステップ：

    const int LAYER_WINDOW_EMPTY = 0;   //!< レイヤー：ウィンドウ：非表示
    const int LAYER_WINDOW_BLACK = 1;   //!< レイヤー：ウィンドウ：黒地
    const int LAYER_WINDOW_MAX = 2; //!< レイヤー：ウィンドウ：

    const int LAYER_CATEGORY_EMPTY = 0; //!< レイヤー：カテゴリ：非表示
    const int LAYER_CATEGORY_LINK = 1;  //!< レイヤー：カテゴリ：リンクユニット
    const int LAYER_CATEGORY_MATERIAL = 2;  //!< レイヤー：カテゴリ：素材ユニット
    const int LAYER_CATEGORY_MAX = 3;   //!< レイヤー：カテゴリ：

    const int LAYER_LOGO_EMPTY = 0; //!< レイヤー：ロゴ表示：非表示
    const int LAYER_LOGO_LINKON = 1;    //!< レイヤー：ロゴ表示：リンクON
    const int LAYER_LOGO_MAX = 2;   //!< レイヤー：ロゴ表示：

    const int OBJECT_CHARA_MESH = 0;    // 参照オブジェクト：キャラ情報：キャラメッシュ
    const int OBJECT_WINDOW_ROOT = 1;   // 参照オブジェクト：ウィンドウルート
    const int OBJECT_CUTIN_PARENT = 2;  // 参照オブジェクト：カットインルート
    const int OBJECT_MAX = 3;	// 

    const float EFFECT_SCALE = 3.0f; // 

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private AssetAutoSetCharaMesh m_AssetAutoSet = null;
    // 参照オブジェクト系リンカ
    private GameObject[] m_ReferObject = new GameObject[MainMenuUnitLinkResult.OBJECT_MAX];
    // リンクカットイン用パーツ
    private MainMenuCutin m_CutinLink = null;
    private MainMenuCutin[] m_CutinParts = null;
    private uint m_LastPartsIdx = 0;
    // シーン遷移関連
    private uint m_WorkStep = STEP_000_START;
    private uint m_WorkStepCount = 0;
    private float m_WorkStepDelta = 0;
    private bool m_WorkStepTriger = true;

    private LayoutSwitchManager m_LayoutWindow = null;      //!< レイアウト切り替え管理：ウィンドウ
    private LayoutSwitchManager m_LayoutCategory = null;        //!< レイアウト切り替え管理：カテゴリ

    private AnimationClipShot[] m_AnimationShotLogo = new AnimationClipShot[LAYER_LOGO_MAX];

    private float m_EffectPosY = 0.0f;	//!< エフェクト表示位置の高さ

    private UnitNamePanel m_UnitNamePanel = null;
    private UnitResultButton m_UnitResultButton = null;

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
        // エフェクト表示位置の高さ
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
        m_LayoutCategory.SetLayoutSwitch(LAYER_CATEGORY_LINK, UnityUtil.GetChildNode(gameObject, "LayerCateLink"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutCategory.SetLayoutSwitch(LAYER_CATEGORY_MATERIAL, UnityUtil.GetChildNode(gameObject, "LayerCateMaterial"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutCategory.AddSwitchRequest(LAYER_CATEGORY_EMPTY, true);


        GameObject cObjectLinkOn = UnityUtil.GetChildNode(gameObject, "LayerLogoLinkOn");
        m_AnimationShotLogo[LAYER_LOGO_LINKON] = cObjectLinkOn.GetComponent<AnimationClipShot>();

        Vector3 vLogoRootPos = new Vector3(0.0f, m_EffectPosY, 0.0f);
        GameObject cObjLogoRoot = UnityUtil.GetChildNode(gameObject, "LayerLogoRoot");
        cObjLogoRoot.transform.position = vLogoRootPos;

        m_UnitNamePanel = m_CanvasObj.GetComponentInChildren<UnitNamePanel>();
        m_UnitResultButton = m_CanvasObj.GetComponentInChildren<UnitResultButton>();
        m_UnitResultButton.Button1Text = "別のユニットをリンク";
        m_UnitResultButton.IsActiveButton1 = false;
        m_UnitResultButton.DidSelectButton1 = SwitchUnitSelect;
        m_UnitResultButton.IsActiveButton2 = false;

        //----------------------------------------
        // パッチ処理を行わないようにする
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
        && m_LayoutWindow.m_LayoutStartOK == false)
        {
            m_LayoutWindow.SetLayoutStartOK();
        }
        if (m_LayoutCategory != null
        && m_LayoutCategory.m_LayoutStartOK == false)
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
            case STEP_000_START: ExecStep_000_Start(); break;   // 演出ステップ：
            case STEP_010_WINDOW_BLACK_IN: ExecStep_010_WindowBlackIn(); break; // 演出ステップ：「黒ウィンドウ」イン
            case STEP_020_LINK_IN: ExecStep_020_LinkIn(); break;    // 演出ステップ：「Link Unit」イン
            case STEP_030_LINK_CUTIN: ExecStep_030_LinkCutin(); break;  // 演出ステップ：リンクユニットカットイン
            case STEP_040_MATERIAL_IN: ExecStep_040_MaterialIn(); break;    // 演出ステップ：「Material Unit」イン
            case STEP_050_MATERIAL_CUTIN: ExecStep_050_MaterialCutin(); break;  // 演出ステップ：素材ユニットカットイン
            case STEP_060_CUTIN_ANIM_OUT: ExecStep_060_CutinUnitAnimOut(); break;   // 演出ステップ：ユニットカットイン終了アニメーション
            case STEP_070_CUTIN_OUT: ExecStep_070_CutinOut(); break;    // 演出ステップ：カットインアウト
            case STEP_080_WINDOW_STATUS_IN: ExecStep_080_WindowStatusIn(); break;   // 演出ステップ：ステータスウィンドウを有効化
            case STEP_090_LINKON: ExecStep_090_LinkOn(); break; // 演出ステップ：リンク
            case STEP_999_MAX: ExecStep_999_Max(); break;   // 演出ステップ：
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
        ProductionAllSkip();

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
        if (m_CutinLink != null)
        {
            Destroy(m_CutinLink.cutinObj);
            m_CutinLink = null;
        }
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
        UnityUtil.ResourceRefresh();

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
        // リンク情報クリア
        //--------------------------------
        m_WorkStep = STEP_000_START;
        m_WorkStepDelta = 0;
        m_WorkStepTriger = true;


        //--------------------------------
        // Bugweb:5925 関連
        // アチーブメント達成演出をスキップされる可能性を考慮しアチーブメント取得フラグONにしとく。
        //  フラグをONにすることで、次回、ミッション画面に遷移したとき、
        //  ローカルで保持してるアチーブメントマスタのリスト情報を破棄し、新しく一からマスタを取得し直すようになる。
        //--------------------------------
        if (MainMenuParam.m_AchievementListGet == false)
        {
            MainMenuParam.m_AchievementListGet = true;
        }

        //--------------------------------
        // リンク前後の情報を保持しておく
        //--------------------------------
        PacketStructUnit UnitBefore = new PacketStructUnit();   //!< ユニット情報：リンク前
        PacketStructUnit UnitAfter = new PacketStructUnit();    //!< ユニット情報：リンク後
        UnitBefore.Copy(MainMenuParam.m_LinkBaseBefore);
        UnitAfter.Copy(MainMenuParam.m_LinkBaseAfter);
        MasterDataParamChara m_UnitMasterData = MasterDataUtil.GetCharaParamFromID(UnitAfter.id);	//!< ユニット情報：マスターデータ

        m_UnitNamePanel.setup(m_UnitMasterData);


        UnitBefore.add_hp = UnitAfter.add_hp;
        UnitBefore.add_pow = UnitAfter.add_pow;
        UnitBefore.link_unique_id = UnitAfter.link_unique_id;

        //--------------------------------
        // リンク先のユニット情報を取得する
        //--------------------------------
        PacketStructUnit cLinkUnit = CharaLinkUtil.GetLinkUnit(UnitBefore.link_unique_id);

        //----------------------------------------
        // 初期情報としてリンク前のユニット情報を表示
        //----------------------------------------
        // 表示ユニット情報を設定
        MainMenuParam.SetCharaDetailParam(UnitBefore, cLinkUnit);

        //----------------------------------------
        // キャラメッシュ生成
        //----------------------------------------
        if (m_AssetAutoSet == null)
        {
            m_AssetAutoSet = m_ReferObject[OBJECT_CHARA_MESH].AddComponent<AssetAutoSetCharaMesh>();
            m_AssetAutoSet.m_AssetBundleMeshScaleUp = true;
            m_AssetAutoSet.m_AssetBundleMeshPosition = true;
        }
        m_AssetAutoSet.SetCharaID(m_UnitMasterData.fix_id, true);

        //----------------------------------------
        // カットインオブジェクト生成
        //----------------------------------------
        //--------------------------------
        // リンクユニット
        //--------------------------------
        m_CutinLink = new MainMenuCutin();
        m_CutinLink.Setup(m_ReferObject[OBJECT_CUTIN_PARENT], MainMenuParam.m_LinkUnit.id, MainMenuDefine.CUTIN_OBJ_TYPE_FIX);

        //--------------------------------
        // 素材ユニット
        //--------------------------------
        if (MainMenuParam.m_LinkParts != null
        && MainMenuParam.m_LinkParts.m_BufferSize > 0)
        {
            m_CutinParts = new MainMenuCutin[MainMenuParam.m_LinkParts.m_BufferSize];
            for (int i = 0; i < MainMenuParam.m_LinkParts.m_BufferSize; i++)
            {
                m_CutinParts[i] = new MainMenuCutin();
                m_CutinParts[i].Setup(m_ReferObject[OBJECT_CUTIN_PARENT], MainMenuParam.m_LinkParts[i].id, MainMenuDefine.CUTIN_OBJ_TYPE_COST);
            }
        }

        //----------------------------------------
        // とりあえず非表示へ
        //----------------------------------------
        m_LayoutCategory.SetLayoutAllDisable();
        m_LayoutWindow.SetLayoutAllDisable();

        //遷移ボタンOFF
        m_UnitResultButton.IsActiveButton1 = false;
        m_UnitResultButton.IsActiveButton2 = false;

        // ロゴ情報
        for (int i = 0; i < m_AnimationShotLogo.Length; i++)
        {
            if (m_AnimationShotLogo[i] == null)
            {
                continue;
            }
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
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リンク演出ステップ：
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
		@brief	リンク演出ステップ：「黒ウィンドウ」イン
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
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リンク演出ステップ：「Link Unit」イン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_020_LinkIn()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            m_LayoutCategory.AddSwitchRequest(LAYER_CATEGORY_LINK, false);

        }

        //-------------------
        // 更新完遂待ち
        //-------------------
        m_WorkStepDelta += Time.deltaTime;
        if (m_WorkStepDelta >= MainMenuDefine.UPDATE_WAIT_TIME &&
            m_CutinLink.isTexture)
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リンク演出ステップ：リンクユニットカットイン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_030_LinkCutin()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            //----------------------------------------
            // カットイン準備
            //----------------------------------------
            m_CutinLink.SetupCutin();

            //---------------------------------------
            // オブジェクトのセットアップが完了したので、アニメーション発行
            //----------------------------------------
            m_CutinLink.StartAnimAll(MainMenuDefine.ANIM_FIX_UNIT_IN);

            //----------------------------------------
            // エフェクトを発生
            //----------------------------------------
            Vector3 vEffectPosition = m_ReferObject[OBJECT_CHARA_MESH].transform.position;
            vEffectPosition.y = m_EffectPosY;
            vEffectPosition.z += GlobalDefine.UNITDETAIL_EFFECT_OFFSET_Z;
            GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vEffectPosition, SceneObjReferMainMenu.Instance.m_EffectEvolCutinFriend, gameObject);
            UnityUtil.SetSortingOrder(_effObj, "EFFECT");
            _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);

            //----------------------------------------
            // SE再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.SE_MM_D01_FRIEND_UNIT);
        }


        //-------------------
        // 更新完遂待ち
        //-------------------
        if (m_CutinLink.isBaseAnimPlay == true)
        {
            return;
        }

        m_WorkStep++;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リンク演出ステップ：「Material Unit」イン
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
		@brief	リンク演出ステップ：素材ユニットカットイン
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
            // 素材を必要としない場合
            if (m_CutinParts == null)
            {
                m_WorkStep++;
                return;
            }
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
        float fCutinCycle = 0.5f;
        int nCyclePrev = (int)(m_WorkStepDelta / fCutinCycle);
        m_WorkStepDelta += Time.deltaTime;
        int nCycleAfter = (int)(m_WorkStepDelta / fCutinCycle);
        if (nCycleAfter != nCyclePrev)
        {
            bNextCutin = true;
        }

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
		@brief	リンク演出ステップ：ユニットカットイン：終了アニメーション
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
            m_CutinLink.StartAnimUnit(MainMenuDefine.ANIM_FIX_UNIT_OUT);
            if (m_CutinParts != null)
            {
                m_CutinParts[m_LastPartsIdx].StartAnimUnit(MainMenuDefine.ANIM_COST_UNIT_OUT);
            }
        }

        //-------------------
        // アニメーション終了待ち
        //-------------------
        if (m_CutinLink.isUnitAnimPlay == true)
        {
            return;
        }
        if (m_CutinParts != null
        && m_CutinParts[m_LastPartsIdx].isUnitAnimPlay == true)
        {
            return;
        }

        m_WorkStep++;
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

            UnityUtil.SetObjectEnabled(m_CutinLink.cutinObj, false);

            if (m_CutinParts != null)
            {
                for (int i = 0; i < m_CutinParts.Length; i++)
                {
                    UnityUtil.SetObjectEnabled(m_CutinParts[i].cutinObj, false);
                }
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
		@brief	リンク演出ステップ：ステータスウィンドウを有効化
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
        /*&&	!m_DetailBase.IsPlayWindowAnimation()*/ )
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リンク演出ステップ：リンク
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_090_LinkOn()
    {
        //----------------------------------------
        // トリガー処理
        //----------------------------------------
        if (m_WorkStepTriger == true)
        {
            //----------------------------------------
            // リンクのロゴ表示
            //----------------------------------------
            if (m_AnimationShotLogo[LAYER_LOGO_LINKON] != null)
            {
                UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LINKON].gameObject, true);
                m_AnimationShotLogo[LAYER_LOGO_LINKON].PlayAnimation(AnimationClipShot.SHOT_ANIM.PLAY);
            }

            //----------------------------------------
            // エフェクトを出す
            //----------------------------------------
            if (SceneObjReferMainMenu.Instance.m_EffectLinkOn != null)
            {
                Vector3 vPos = new Vector3(0, m_EffectPosY, 0);
                GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vPos, SceneObjReferMainMenu.Instance.m_EffectLinkOn, gameObject);
                UnityUtil.SetSortingOrder(_effObj, "EFFECT");
                _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);
            }

            //----------------------------------------
            // 「LINK ON」ボイス再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_LINK_ON);

            //----------------------------------------
            // SE再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.SE_MM_D10_EVOLVE_COMP);
        }

        //----------------------------------------
        // アニメーション待ち
        //----------------------------------------
        if (m_AnimationShotLogo[LAYER_LOGO_LINKON] != null
        && m_AnimationShotLogo[LAYER_LOGO_LINKON].ChkAnimationPlaying() == true)
        {
            return;
        }


        //----------------------------------------
        // 表示は切っておく
        //----------------------------------------
        if (m_AnimationShotLogo[LAYER_LOGO_LINKON] != null)
        {
            UnityUtil.SetObjectEnabled(m_AnimationShotLogo[LAYER_LOGO_LINKON].gameObject, false);
        }

        //----------------------------------------
        // 次のステップへ遷移
        //----------------------------------------
        m_WorkStep++;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リンク演出ステップ：入力待ち
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
            // リンクタブのアニメーション実行
            //----------------------------------------

            //遷移ボタンON
            m_UnitResultButton.IsActiveButton1 = true;
        }
    }

    private void SwitchUnitSelect()
    {
        //ユニット選択へ
        if (MainMenuManager.HasInstance)
        {
            MainMenuParam.m_LinkBaseUnitUniqueId = 0;
            MainMenuParam.m_LinkTargetUnitUniqueId = 0;
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_LINK, false, false, true);
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
    }

}
///////////////////////////////////////EOF///////////////////////////////////////
