/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuSeqResultEvol.cs
	@brief	メインメニューシーケンス：リザルト枠：進化クエストでのリザルト演出
	@author Developer
	@date 	2013/07/02
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
	@brief	メインメニューシーケンス：リザルト枠：進化クエストでのリザルト演出
*/
//----------------------------------------------------------------------------
public class MainMenuUnitEvolveResult : MainMenuSeq
{
    //----------------------------------------------------------------------------
    /*!
		@brief	カットイン情報
	*/
    //----------------------------------------------------------------------------
    const int STEP_000_START = 0;   //!< 進化演出ステップ：
    const int STEP_010_WINDOW_BLACK_IN = 1; //!< 進化演出ステップ：「黒ウィンドウ」イン
    const int STEP_020_FRIEND_IN = 2;   //!< 進化演出ステップ：「Friend Unit」イン
    const int STEP_030_FRIEND_CUTIN = 3;    //!< 進化演出ステップ：フレンドユニットカットイン
    const int STEP_040_MATERIAL_IN = 4; //!< 進化演出ステップ：「Material Unit」イン
    const int STEP_050_MATERIAL_CUTIN = 5;  //!< 進化演出ステップ：素材ユニットカットイン
    const int STEP_060_CUTIN_ANIM_OUT = 6;  //!< 進化演出ステップ：ユニットカットイン終了アニメーション
    const int STEP_070_CUTIN_OUT = 7;   //!< 進化演出ステップ：カットインアウト
    const int STEP_080_WINDOW_BLACK_OUT = 8;    //!< 進化演出ステップ：「黒ウィンドウ」アウト
    const int STEP_090_CHARA_SHADOW = 9;    //!< 進化演出ステップ：キャラシルエット化
    const int STEP_100_CHARA_SWITCH = 10;   //!< 進化演出ステップ：キャラを進化後シルエットへ
    const int STEP_110_EVOLVE_MSG = 11; //!< 進化演出ステップ：「Evolve！」を表示
    const int STEP_120_CHARA_SWITCH_FIX = 12;   //!< 進化演出ステップ：キャラを進化後実体へ
    const int STEP_130_WINDOW_STATUS_IN = 13;   //!< 進化演出ステップ：ステータスウィンドウを有効化
    const int STEP_140_BACK_BUTTON_IN = 14; //!< 進化演出ステップ：戻るボタンを有効化
    const int STEP_999_MAX = 15;    //!< 進化演出ステップ：

    const int LAYER_WINDOW_EMPTY = 0;   //!< レイヤー：ウィンドウ：非表示
    const int LAYER_WINDOW_BLACK = 1;   //!< レイヤー：ウィンドウ：黒地
    const int LAYER_WINDOW_MAX = 2; //!< レイヤー：ウィンドウ：

    const int LAYER_CATEGORY_EMPTY = 0; //!< レイヤー：カテゴリ：非表示
    const int LAYER_CATEGORY_FRIEND = 1;    //!< レイヤー：カテゴリ：フレンドユニット
    const int LAYER_CATEGORY_MATERIAL = 2;  //!< レイヤー：カテゴリ：素材ユニット
    const int LAYER_CATEGORY_MAX = 3;   //!< レイヤー：カテゴリ：

    const int LAYER_EVOLVE_EMPTY = 0;   //!< レイヤー：Evolve表示：非表示
    const int LAYER_EVOLVE_ACTIVE = 1;  //!< レイヤー：Evolve表示：表示
    const int LAYER_EVOLVE_MAX = 2; //!< レイヤー：Evolve表示：

    const int OBJECT_CHARA_MESH_ROOT = 0;   // 参照オブジェクト：キャラ情報：キャラメッシュ
    const int OBJECT_CHARA_MESH_BEFORE = 1; // 参照オブジェクト：キャラ情報：キャラメッシュ
    const int OBJECT_CHARA_MESH_AFTER = 2;  // 参照オブジェクト：キャラ情報：キャラメッシュ
    const int OBJECT_WINDOW_ROOT = 3;   // 参照オブジェクト：ウィンドウルート
    const int OBJECT_CUTIN_PARENT = 4;  // 参照オブジェクト：カットインルート

    const int OBJECT_MAX = 5;	// 

    const string EVOL_EFF_NAME = "EvolEffect";
    const float EFFECT_SCALE = 3.0f;

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    //	private TemplateList< AssetBundleResource >	m_AssetResourceList	= null;		// 事前のAssetBundleダウンロード用リクエストリスト
    // 参照オブジェクト系リンカ
    private GameObject[] m_ReferObject = new GameObject[MainMenuUnitEvolveResult.OBJECT_MAX];
    // 進化カットイン用パーツ
    private MainMenuCutin[] m_EvolCutinParts = null;
    private int m_LastPartsIdx = 0;
    // ユニットメッシュ関連
    private AnimationClipResultEvol m_CharaMeshAnim = null;
    private AssetAutoSetCharaMesh m_AssetAutoSetBefore = null;
    private AssetAutoSetCharaMesh m_AssetAutoSetAfter = null;
    // シーン遷移関連
    private uint m_WorkStep = STEP_000_START;
    private float m_WorkStepDelta = 0;
    private bool m_WorkStepTriger = true;

    private LayoutSwitchManager m_LayoutWindow = null;      //!< レイアウト切り替え管理：ウィンドウ
    private LayoutSwitchManager m_LayoutCategory = null;        //!< レイアウト切り替え管理：カテゴリ
    private LayoutSwitchManager m_LayoutEvolve = null;      //!< レイアウト切り替え管理：進化表示

    private float m_EffectPosY = 0.0f;		//!< エフェクト表示位置の高さ

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
        m_ReferObject[OBJECT_CHARA_MESH_ROOT] = UnityUtil.GetChildNode(gameObject, "CharaMeshRoot", "CharaMesh");
        m_ReferObject[OBJECT_CHARA_MESH_BEFORE] = UnityUtil.GetChildNode(m_ReferObject[OBJECT_CHARA_MESH_ROOT], "CharaMeshBefore");
        m_ReferObject[OBJECT_CHARA_MESH_AFTER] = UnityUtil.GetChildNode(m_ReferObject[OBJECT_CHARA_MESH_ROOT], "CharaMeshAfter");
        // ウィンドウルート
        m_ReferObject[OBJECT_WINDOW_ROOT] = UnityUtil.GetChildNode(gameObject, "WindowBaseRoot");
        // カットインルートオブジェクト
        m_ReferObject[OBJECT_CUTIN_PARENT] = UnityUtil.GetChildNode(gameObject, "CutinParent");
        // エフェクト表示位置の高さ
        //m_EffectPosY = UnityUtil.GetChildNode( gameObject, "CharaBackGround" ).transform.position.y;
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
        m_LayoutCategory.SetLayoutSwitch(LAYER_CATEGORY_FRIEND, UnityUtil.GetChildNode(gameObject, "LayerCateFriend"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutCategory.SetLayoutSwitch(LAYER_CATEGORY_MATERIAL, UnityUtil.GetChildNode(gameObject, "LayerCateMaterial"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutCategory.AddSwitchRequest(LAYER_CATEGORY_EMPTY, true);

        //-------------------------
        // 階層をレイアウト切り替え管理クラスに登録
        // 最初のレイアウトは非表示にしておく
        //-------------------------
        m_LayoutEvolve = gameObject.AddComponent<LayoutSwitchManager>();
        m_LayoutEvolve.SetLayoutMax(LAYER_EVOLVE_MAX);
        m_LayoutEvolve.SetLayoutSwitch(LAYER_EVOLVE_EMPTY, UnityUtil.GetChildNode(gameObject, "LayerEvolveEmpty"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutEvolve.SetLayoutSwitch(LAYER_EVOLVE_ACTIVE, UnityUtil.GetChildNode(gameObject, "LayerEvolve"), false, null, typeof(MainMenuDialogSeqWait));
        m_LayoutEvolve.AddSwitchRequest(LAYER_EVOLVE_EMPTY, true);


        //-------------------------
        // ユニットアニメーションのコンポーネント取得
        //-------------------------
        if (m_ReferObject[OBJECT_CHARA_MESH_ROOT] != null)
        {
            m_CharaMeshAnim = m_ReferObject[OBJECT_CHARA_MESH_ROOT].GetComponent<AnimationClipResultEvol>();
        }

        Vector3 vLogoRootPos = new Vector3(0.0f, m_EffectPosY, 0.0f);
        GameObject cObjLogoRoot = UnityUtil.GetChildNode(gameObject, "LayerLogoRoot");
        cObjLogoRoot.transform.position = vLogoRootPos;

        m_UnitNamePanel = m_CanvasObj.GetComponentInChildren<UnitNamePanel>();

        m_UnitResultButton = m_CanvasObj.GetComponentInChildren<UnitResultButton>();
        m_UnitResultButton.Button1Text = "このユニットを強化する";
        m_UnitResultButton.IsActiveButton1 = false;
        m_UnitResultButton.DidSelectButton1 = SwitchBuildup;
        m_UnitResultButton.Button2Text = "別のユニットを進化";
        m_UnitResultButton.IsActiveButton2 = false;
        m_UnitResultButton.DidSelectButton2 = SwitchUnitSelect;

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
        if (m_LayoutEvolve != null
        && m_LayoutEvolve.m_LayoutStartOK == false
        )
        {
            m_LayoutEvolve.SetLayoutStartOK();
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
            case STEP_000_START: ExecStep_000_Start(); break;   // 進化演出ステップ：
            case STEP_010_WINDOW_BLACK_IN: ExecStep_010_WindowBlackIn(); break; // 進化演出ステップ：「黒ウィンドウ」イン
            case STEP_020_FRIEND_IN: ExecStep_020_FriendIn(); break;    // 進化演出ステップ：「Friend Unit」イン
            case STEP_030_FRIEND_CUTIN: ExecStep_030_FriendCutin(); break;  // 進化演出ステップ：フレンドユニットカットイン
            case STEP_040_MATERIAL_IN: ExecStep_040_MaterialIn(); break;    // 進化演出ステップ：「Material Unit」イン
            case STEP_050_MATERIAL_CUTIN: ExecStep_050_MaterialCutin(); break;  // 進化演出ステップ：素材ユニットカットイン
            case STEP_060_CUTIN_ANIM_OUT: ExecStep_060_CutinUnitAnimOut(); break;   // 進化演出ステップ：ユニットカットイン終了アニメーション
            case STEP_070_CUTIN_OUT: ExecStep_070_CutinOut(); break;    // 進化演出ステップ：カットインアウト
            case STEP_080_WINDOW_BLACK_OUT: ExecStep_080_WindowBlackOut(); break;   // 進化演出ステップ：「黒ウィンドウ」アウト
            case STEP_090_CHARA_SHADOW: ExecStep_090_CharaShadow(); break;  // 進化演出ステップ：キャラシルエット化
            case STEP_100_CHARA_SWITCH: ExecStep_100_CharaSwitch(); break;  // 進化演出ステップ：キャラを進化後シルエットへ
            case STEP_110_EVOLVE_MSG: ExecStep_110_EvolveMsg(); break;  // 進化演出ステップ：「Evolve！」を表示
            case STEP_120_CHARA_SWITCH_FIX: ExecStep_120_CharaSwitchFix(); break;   // 進化演出ステップ：キャラを進化後実体へ
            case STEP_130_WINDOW_STATUS_IN: ExecStep_130_WindowStatusIn(); break;   // 進化演出ステップ：ステータスウィンドウを有効化
            case STEP_140_BACK_BUTTON_IN: ExecStep_140_BackButtonIn(); break;   // 進化演出ステップ：戻るボタンを有効化
            case STEP_999_MAX: ExecStep_999_Max(); break;   // 進化演出ステップ：
        }
        if (m_WorkStep != unStepNow)
        {
            m_WorkStepTriger = true;
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
        if (m_EvolCutinParts != null)
        {
            for (int i = 0; i < m_EvolCutinParts.Length; i++)
            {

                Destroy(m_EvolCutinParts[i].cutinObj);
                m_EvolCutinParts[i] = null;
            }
            m_EvolCutinParts = null;
        }

        //---------------------
        // オブジェクト破棄
        //---------------------
        if (m_AssetAutoSetBefore != null)
        {
            m_AssetAutoSetBefore.DestroyCharaMesh();
        }
        if (m_AssetAutoSetAfter != null)
        {
            m_AssetAutoSetAfter.DestroyCharaMesh();
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
        if (m_AssetAutoSetBefore != null)
        {
            m_AssetAutoSetBefore.DestroyCharaMesh();
        }
        if (m_AssetAutoSetAfter != null)
        {
            m_AssetAutoSetAfter.DestroyCharaMesh();
        }

        //--------------------------------
        // 進行情報クリア
        //--------------------------------
        m_WorkStep = STEP_000_START;
        m_WorkStepDelta = 0;
        m_WorkStepTriger = true;
        m_CharaMeshAnim.ResetAnimation();

        //----------------------------------------
        // パッチ処理を行わないようにする.
        //----------------------------------------
        MainMenuManager.Instance.m_ResumePatchUpdateRequest = false;

        //--------------------------------
        // 進化前後のユニット情報を取得
        //--------------------------------
        PacketStructUnit cEvolUnit = MainMenuParam.m_EvolveBaseAfter;
        PacketStructUnit cEvolUnitPrev = MainMenuParam.m_EvolveBaseBefore;

        if (cEvolUnit == null
        || cEvolUnitPrev == null
        )
        {
            Debug.LogError("Evol Unit Error!!");
            //とりあえずホームへ
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false);
            return;
        }

        //--------------------------------
        // 選択されているユニット情報を選定
        //--------------------------------
        uint unCharaMasterDataID = cEvolUnit.id;            // 進化後
        uint unCharaMasterDataIDPrev = cEvolUnitPrev.id;     // 新化前

        MasterDataParamChara cCharaMasterData = MasterDataUtil.GetCharaParamFromID(unCharaMasterDataID);
        MasterDataParamChara cCharaMasterDataPrev = MasterDataUtil.GetCharaParamFromID(unCharaMasterDataIDPrev);
        if (cCharaMasterData == null || cCharaMasterDataPrev == null)
        {
            Debug.LogError("SelectUnit MasterData Is None! - " + unCharaMasterDataID);
            return;
        }
        // 表示ユニット情報を設定
        //MainMenuParam.SetCharaDetailParam( cEvolUnit );
        m_UnitNamePanel.setup(cCharaMasterDataPrev);

        // リンクユニット情報取得
        PacketStructUnit cLinkUnit = CharaLinkUtil.GetLinkUnit(cEvolUnit);
        // 表示ユニット情報を設定
        MainMenuParam.SetCharaDetailParam(cEvolUnit, cLinkUnit);

        //----------------------------------------
        // キャラメッシュ生成
        //----------------------------------------
        {
            // 新化前ユニット
            if (m_AssetAutoSetBefore == null)
            {
                m_AssetAutoSetBefore = m_ReferObject[OBJECT_CHARA_MESH_BEFORE].AddComponent<AssetAutoSetCharaMesh>();
                m_AssetAutoSetBefore.m_AssetBundleMeshScaleUp = true;
                m_AssetAutoSetBefore.m_AssetBundleMeshPosition = true;
            }
            m_AssetAutoSetBefore.SetCharaID(unCharaMasterDataIDPrev, true);
            // 進化後ユニット
            if (m_AssetAutoSetAfter == null)
            {
                m_AssetAutoSetAfter = m_ReferObject[OBJECT_CHARA_MESH_AFTER].AddComponent<AssetAutoSetCharaMesh>();
                m_AssetAutoSetAfter.m_AssetBundleMeshScaleUp = true;
                m_AssetAutoSetAfter.m_AssetBundleMeshPosition = true;
            }
            m_AssetAutoSetAfter.SetCharaID(unCharaMasterDataID, true);
        }

        {
            TemplateList<uint> cPartsUnitIDList = new TemplateList<uint>();
            for (int i = 0; i < MainMenuParam.m_EvolveParts.m_BufferSize; i++)
            {
                cPartsUnitIDList.Add(MainMenuParam.m_EvolveParts[i].id);
            }

            m_EvolCutinParts = new MainMenuCutin[cPartsUnitIDList.m_BufferSize];
            for (int i = 0; i < cPartsUnitIDList.m_BufferSize; i++)
            {
                m_EvolCutinParts[i] = new MainMenuCutin();
                m_EvolCutinParts[i].Setup(m_ReferObject[OBJECT_CUTIN_PARENT], cPartsUnitIDList[i], MainMenuDefine.CUTIN_OBJ_TYPE_COST);
            }
        }

        //----------------------------------------
        // とりあえず非表示へ
        //----------------------------------------
        m_LayoutWindow.SetLayoutAllDisable();
        m_LayoutEvolve.SetLayoutAllDisable();
        m_LayoutCategory.SetLayoutAllDisable();

        //遷移ボタン非表示
        m_UnitResultButton.IsActiveButton1 = false;
        m_UnitResultButton.IsActiveButton2 = false;
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
		@brief	進化演出ステップ：
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_000_Start()
    {
        {
            //----------------------------------------
            // 特にやることないんで無条件で次へ
            //----------------------------------------
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：「黒ウィンドウ」イン
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
            //m_WorkStep++;
            m_WorkStep = STEP_040_MATERIAL_IN;

        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：「Friend Unit」イン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_020_FriendIn()
    {
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：フレンドユニットカットイン
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_030_FriendCutin()
    {

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：「Material Unit」イン
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
		@brief	進化演出ステップ：素材ユニットカットイン
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
        for (int i = 0; i < m_EvolCutinParts.Length; i++)
        {
            if (m_EvolCutinParts[i].isTexture)
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
            for (int i = 0; i < m_EvolCutinParts.Length; i++)
            {
                if (m_EvolCutinParts[i].SetupCutin(true) == false)
                {
                    return;
                }
            }
        }

        //-------------------
        // 次が発行可能かチェック
        //-------------------
        bool bNextCutin = true;
        for (int i = 0; i < m_EvolCutinParts.Length; i++)
        {
            if (m_EvolCutinParts[i].isBaseAnimPlay == false)
            {
                continue;
            }

            bNextCutin = false;
            break;
        }
        if (bNextCutin == false)
        {
            return;
        }


        //-------------------
        // 次の素材がいないなら完遂
        //-------------------
        int nPartsNum = (int)(m_WorkStepDelta + 0.1f);
        if (nPartsNum >= m_EvolCutinParts.Length)
        {
            m_WorkStep++;
            return;
        }

        //-------------------
        // 次の素材がいるならカットイン呼び出し
        //-------------------
        {
            // 1つ前のキャラカットイン終了アニメーション
            int nWorkStepPrev = nPartsNum - 1;
            if (nWorkStepPrev >= 0)
            {
                m_EvolCutinParts[nWorkStepPrev].StartAnimUnit(MainMenuDefine.ANIM_COST_UNIT_OUT);
            }

            m_EvolCutinParts[nPartsNum].StartAnimAll(MainMenuDefine.ANIM_COST_UNIT_IN);

            // 最後のアクセス番号を保存
            m_LastPartsIdx = nPartsNum;

            m_WorkStepDelta += 1.0f;

            //----------------------------------------
            // エフェクトを発生
            //----------------------------------------
            Vector3 vEffectPosition = m_ReferObject[OBJECT_CHARA_MESH_ROOT].transform.position;
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

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	リンク演出ステップ：カットイン終了アニメーション
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_060_CutinUnitAnimOut()
    {
        //-------------------
        // アニメーション終了待ち
        //-------------------
        if (m_EvolCutinParts[m_LastPartsIdx].isUnitAnimPlay == false)
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：カットインアウト
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
            for (int i = 0; i < m_EvolCutinParts.Length; i++)
            {
                UnityUtil.SetObjectEnabled(m_EvolCutinParts[i].cutinObj, false);
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
		@brief	進化演出ステップ：「黒ウィンドウ」アウト
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_080_WindowBlackOut()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            m_LayoutWindow.AddSwitchRequest(LAYER_WINDOW_EMPTY, false);
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
		@brief	進化演出ステップ：キャラシルエット化
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_090_CharaShadow()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            //----------------------------------------
            // エフェクトを発生
            //----------------------------------------
            Vector3 vEffectPosition = m_ReferObject[OBJECT_CHARA_MESH_ROOT].transform.position;
            vEffectPosition.y = m_EffectPosY;
            vEffectPosition.z += GlobalDefine.UNITDETAIL_EFFECT_OFFSET_Z;
            if (EffectManager.Instance.isPlayingEffect(EVOL_EFF_NAME))
            {
                EffectManager.Instance.stopEffect(EVOL_EFF_NAME);
            }
            GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vEffectPosition, SceneObjReferMainMenu.Instance.m_EffectEvolFix, gameObject, EVOL_EFF_NAME);
            UnityUtil.SetSortingOrder(_effObj, "EFFECT");
            _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);

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
		@brief	進化演出ステップ：キャラを進化後シルエットへ
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_100_CharaSwitch()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            if (m_CharaMeshAnim != null)
            {
                m_CharaMeshAnim.PlayAnimation(AnimationClipResultEvol.EVOL_ANIM.CHARA_SWITCH_AFTER);
            }

            //----------------------------------------
            // SE再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.SE_MM_D09_EVOLVE_ROLL);

        }

        //-------------------
        // 更新完遂待ち
        //-------------------
        if (m_CharaMeshAnim != null
        && m_CharaMeshAnim.ChkAnimationPlaying() == true
        ) return;

        m_CharaMeshAnim.StopAnimationClip();
        m_WorkStep++;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：「Evolve！」を表示
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_110_EvolveMsg()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            m_LayoutEvolve.AddSwitchRequest(LAYER_EVOLVE_ACTIVE, false);

            //----------------------------------------
            // 進化ボイス再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_EVOLVE);


            //----------------------------------------
            // SE再生
            //----------------------------------------
            SoundUtil.PlaySE(SEID.SE_MM_D10_EVOLVE_COMP);

        }

        //-------------------
        // 更新完遂待ち
        //-------------------
        m_WorkStepDelta += Time.deltaTime;
        if (m_WorkStepDelta >= 0.5f)
        {
            m_LayoutEvolve.AddSwitchRequest(LAYER_EVOLVE_EMPTY, true);
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：キャラを進化後実体へ
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_120_CharaSwitchFix()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            //UnityUtil.SetObjectEnabled( m_AssetAutoSetAfter.m_AssetBundleObject , true );

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
		@brief	進化演出ステップ：ステータスウィンドウを有効化
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_130_WindowStatusIn()
    {
        //-------------------
        // トリガー処理
        //-------------------
        if (m_WorkStepTriger == true)
        {
            m_LayoutWindow.AddSwitchRequest(LAYER_WINDOW_EMPTY, false);

            MasterDataParamChara cCharaMasterData = MasterDataUtil.GetCharaParamFromID(MainMenuParam.m_EvolveBaseAfter.id);
            m_UnitNamePanel.setup(cCharaMasterData);
        }

        //-------------------
        // 更新完遂待ち
        //-------------------
        if (m_LayoutWindow.ChkLayoutAnimationFinish(LAYER_WINDOW_EMPTY) == true
        )
        {
            m_WorkStep++;
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：戻るボタンを有効化
		@note	
	*/
    //----------------------------------------------------------------------------
    void ExecStep_140_BackButtonIn()
    {
        //-------------------
        // 更新完遂待ち
        //-------------------
        m_WorkStep++;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	進化演出ステップ：入力待ち
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
            //遷移ボタン表示
            m_UnitResultButton.IsActiveButton1 = true;
            m_UnitResultButton.IsActiveButton2 = true;
        }
    }

    private void SwitchBuildup()
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuParam.m_BuildupBaseUnitUniqueId = MainMenuParam.m_EvolveBaseAfter.unique_id;
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_BUILDUP, false, false, true);
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
    }

    private void SwitchUnitSelect()
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuParam.m_EvolveBaseUnitUniqueId = 0;
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_UNIT_EVOLVE, false, false);
            SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
    }
}

