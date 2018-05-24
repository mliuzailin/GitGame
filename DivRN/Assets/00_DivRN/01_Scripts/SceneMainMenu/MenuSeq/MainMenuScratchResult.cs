
#define DEF_SCRATCH_MEKURI_WAIT_NONE	// めくり中のモーション待ち無効
#define DEF_SCRATCH_WAIT_NONE			// 待ち処理軽減
#define DEF_SCRATCH_ANSWER_SKIP			// 答え合わせ無し

/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	MainMenuSeqGachaMain.cs
	@brief	メインメニューシーケンス：ガチャ枠：スクラッチメイン処理
	@author Developer
	@date 	2013/02/15
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
	@brief	メインメニューシーケンス：ガチャ枠：スクラッチメイン処理
*/
//----------------------------------------------------------------------------
public class MainMenuScratchResult : MainMenuSeq
{
    public enum GachaSeq
    {
        NONE = 0,       //!< ガチャ演出シーケンス：選ばせる→選んだとこが駒の絵に置き換わる
        SELECT,         //!< ガチャ演出シーケンス：選ばせる→選んだとこが駒の絵に置き換わる
        SELECT_OPEN,    //!< ガチャ演出シーケンス：選び終わったら１つずつ開いてく→見たことないならキャラ詳細へ
        END_WAIT,       //!< ガチャ演出シーケンス：入力あるまで待ち
    }

    const int SCRATCH_CENTER = 4;       //!< リンカID：スクラッチ
    const int SCRATCH_MAX = 9;        //!< リンカID：

    const float EFFECT_SCALE = 300.0f;     //!< エフェクトサイズ

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    private ScratchResult m_ScratchResult = null;
    private ScratchPanel[] m_ScratchPanel { get { return m_ScratchResult.scratchPanelList; } }

    private int[] m_ScratchTouchNumList = new int[SCRATCH_MAX]; //!< スクラッチ情報：タッチした位置情報
    private int m_ScratchTouchCt = 0;                       //!< スクラッチ情報：タッチした駒化した数
    private int m_ScratchTouchOpenCt = 0;                       //!< スクラッチ情報：タッチしてキャラ化した数
    private float m_ScratchWait = 0.0f;                     //!< スクラッチ情報：待ち時間
    private int m_ScratchDetailChkCt = 0;                       //!< スクラッチ情報：キャラ詳細へ飛ばしチェック済みの数

    private GachaSeq m_ScratchSeq = GachaSeq.NONE;          //!< スクラッチ情報：更新シーケンス
    private bool m_ScratchSeqTriger = true;                     //!< スクラッチ情報：更新シーケンストリガー

    private int[] m_ScratchRareList = new int[SCRATCH_MAX]; //!< スクラッチ情報：レア度リスト
    private int m_ScratchRareOver5Num = 0;                      //!< スクラッチ情報：まだ引いていない☆5以上の数
    private bool m_ScratchCenterOpen = false;                   //!< スクラッチ情報：9連ガチャ時に真ん中を引いたかのフラグ

    private long[] m_GachaTkOpenUIdList = new long[SCRATCH_MAX];    //!< スクラッチカード一括開封対応：ユニークIDリスト
    private bool m_GachaPlayMore = false;                   //!< ガチャ情報：ガチャ一括開封9連越えフラグ v400

    private GameObject m_GachaEffectPrefab = null;

    private int m_updateLayoutCount = 0;

    private bool m_bEnableCharaDetail = false;

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
    public override void Update()
    {
        //----------------------------------------
        // 固定処理
        // 管理側からの更新許可やフェード待ち含む。
        //----------------------------------------
        if (PageSwitchUpdate() == false)
        {
            return;
        }

        if (m_updateLayoutCount > 0)
        {
            m_updateLayoutCount--;
            if (m_updateLayoutCount <= 0)
            {
                m_updateLayoutCount = 0;
                m_ScratchResult.omakeBG.alpha = 1.0f;
            }
            m_ScratchResult.updateLayout();
        }

        //----------------------------------------
        // シーケンスに合わせて処理分岐
        //----------------------------------------
        bool bSeqSuccess = false;
        GachaSeq nGachaSeq = m_ScratchSeq;
        switch (m_ScratchSeq)
        {
            case GachaSeq.SELECT: bSeqSuccess = ScratchSeqSelect(); break;  // ガチャ演出シーケンス：選ばせる→選んだとこが駒の絵に置き換わる
            case GachaSeq.SELECT_OPEN: bSeqSuccess = ScratchSeqSelectOpen(); break; // ガチャ演出シーケンス：選び終わったら１つずつ開いてく→見たことないならキャラ詳細へ
            case GachaSeq.END_WAIT: bSeqSuccess = ScratchSeqEndWait(); break;   // ガチャ演出シーケンス：入力あるまで待ち
            default: bSeqSuccess = false; break;
        }
        if (bSeqSuccess == false)
        {
            //----------------------------------------
            // エラー検知したら安全なページに戻る
            //----------------------------------------
            if (MainMenuManager.HasInstance) MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_GACHA_MAIN, false, false);
        }
        if (nGachaSeq != m_ScratchSeq)
        {
            m_ScratchSeqTriger = true;
        }
        else
        {
            m_ScratchSeqTriger = false;
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

        //--------------------------------
        // スクラッチのシーケンス初期化。
        // ガチャデータの存在が確認できない等で初期化に失敗した場合に
        // エラーで戻れるように一旦Noneを入れておく
        //--------------------------------
        m_ScratchSeq = GachaSeq.NONE;

        //--------------------------------
        // 引くガチャのマスターデータを取得
        // 2016/07/06 中断復帰改修
        // 引く前に保持していたマスターデータを使用する
        // →中断復帰時に落ちる前の状態を再現できるように
        //--------------------------------
        MasterDataGacha cGachaMasterData = null;
        if (MainMenuParam.m_GachaMaster != null)
        {
            cGachaMasterData = MainMenuParam.m_GachaMaster;
        }
        else
        {
            // データが無い場合はIDから判断
            cGachaMasterData = MasterDataUtil.GetGachaParamFromID(MainMenuParam.m_GachaID);
        }
        if (cGachaMasterData == null)
        {
            Debug.LogError("Gacha None! - " + MainMenuParam.m_GachaID);
        }

        //--------------------------------
        // ガチャ結果データの不具合チェック１
        //--------------------------------
        if (MainMenuParam.m_GachaUnitUniqueID == null)
        {
            Debug.LogError("GachaUnit GetData Error!");
            return;
        }

        if (cGachaMasterData != null)
        {
            //--------------------------------
            // 演出ループチェック
            // v400 スクラッチカード一括開封対応 Developer add
            //--------------------------------
            m_GachaPlayMore = false;
            if (cGachaMasterData.type == MasterDataDefineLabel.GachaType.TICKET
            && MainMenuParam.m_GachaUnitUniqueID.Length > SCRATCH_MAX)
            {
                //９連越えフラグ
                m_GachaPlayMore = true;

                //これから開封予定のユニットを決定
                m_GachaTkOpenUIdList = new long[SCRATCH_MAX];
                for (int nOpen = 0; nOpen < SCRATCH_MAX; nOpen++)
                {
                    if (MainMenuParam.m_GachaUnitUniqueID[nOpen] == 0)
                    {
                        continue;
                    }

                    //memo:９連虹確定演出の場合は、9連分の最後に虹確定用のユニットが設定されている

                    m_GachaTkOpenUIdList[nOpen] = MainMenuParam.m_GachaUnitUniqueID[nOpen];
                }

                //※ここにきたときにはスクラッチチケットかつ９連確定している
                //※MainMenuParam.m_GachaUnitUniqueIDから該当のユニットIDを削除するのはすべてめくり終わってから
            }
        }

        //--------------------------------
        // ガチャ結果データの不具合チェック２
        //--------------------------------
        int nUnitTotalResult = MainMenuParam.m_GachaUnitUniqueID.Length;
        if (MainMenuParam.m_GachaBlankUnitID != null)
        {
            nUnitTotalResult += MainMenuParam.m_GachaBlankUnitID.Length;
        }
        if (m_GachaPlayMore == false
        && nUnitTotalResult != SCRATCH_MAX)
        {
            Debug.LogError("GachaUnit GetData Total Error!");
            return;
        }
        if (m_GachaPlayMore == true
        && (nUnitTotalResult % SCRATCH_MAX) != 0)
        {
            //トータルが９で割り切れない値の場合エラー
            Debug.LogError("GachaUnit GetData Total Error_ 9renOver");
            return;
        }

        //--------------------------------
        // ガチャ結果データが正常！
        //--------------------------------

        //--------------------------------
        // スクラッチのシーケンス初期化
        //--------------------------------
        m_ScratchSeq = GachaSeq.SELECT;
        m_ScratchTouchCt = 0;
        m_ScratchTouchOpenCt = 0;
        m_ScratchWait = 0.0f;
        m_ScratchDetailChkCt = 0;

        if (m_ScratchResult == null)
        {
            m_ScratchResult = m_CanvasObj.GetComponentInChildren<ScratchResult>();
            m_ScratchResult.DidSelectSkip = OnSelectSkip;
            m_ScratchResult.DidSelectReturn = OnReturnScratch;
            m_ScratchResult.ButtonLabel = GameTextUtil.GetText("scratch_display4");
            m_ScratchResult.UnitDetailLabel = GameTextUtil.GetText("scratch_display3");
        }


        for (int i = 0; i < m_ScratchTouchNumList.Length; i++)
        {
            m_ScratchTouchNumList[i] = -1;
        }
        for (int i = 0; i < m_ScratchPanel.Length; i++)
        {
            m_ScratchPanel[i].panelSeq = ScratchPanel.PanelSeq.SEQ_NONE;
            m_ScratchPanel[i].Index = i;
            m_ScratchPanel[i].touchIndex = -1;
            m_ScratchPanel[i].DidSelectPanel = OnTouchPanel;

        }

        //--------------------------------
        // ガチャタイプで分岐してUI切り替え
        //--------------------------------
        settinGacha(cGachaMasterData);

        //--------------------------------
        // プラス値は付くやつにしか付かないので一旦無効化
        //--------------------------------
        setPlusFlagPanelAll(false);

        //--------------------------------
        //レア度リストの設定
        //--------------------------------
        m_ScratchRareList = new int[MainMenuParam.m_GachaUnitUniqueID.Length];
        m_ScratchRareOver5Num = 0;
        m_ScratchCenterOpen = false;

        for (int i = 0; i < MainMenuParam.m_GachaUnitUniqueID.Length; i++)
        {
            // v400 スクラッチカード一括開封対応 Developer add
            if (m_GachaPlayMore)
            {
                //開封対象かをチェック
                if (ChkOpenGachaTicket(MainMenuParam.m_GachaUnitUniqueID[i]) == false)
                {
                    continue;
                }
            }

            m_ScratchRareList[i] = (int)GetRareFromUniqueID(UserDataAdmin.Instance.SearchChara(MainMenuParam.m_GachaUnitUniqueID[i]).id);
            if (m_ScratchRareList[i] >= (int)MasterDataDefineLabel.RarityType.STAR_5)
            {
                m_ScratchRareOver5Num++;
            }
        }

        //--------------------------------
        // 虹確定イベント用設定
        //--------------------------------
        if (MainMenuParam.m_GachaRainbowDecide != 0)
        {
            if (m_GachaPlayMore)
            {
                // v400 スクラッチカード一括開封対応 Developer add
                setImagePanelF(SCRATCH_CENTER, "open_sp");
            }
            else
            {
                //--------------------------------
                //9連ガチャなら真ん中の色を変える
                //--------------------------------
                if (MainMenuParam.m_GachaUnitUniqueID.Length == SCRATCH_MAX)
                {
                    setImagePanelF(SCRATCH_CENTER, "open_sp");
                }
            }
        }

        //--------------------------------
        // おまけ
        //--------------------------------
        if (MainMenuParam.m_GachaOmakeID != null)
        {
            m_ScratchResult.IsViewSubPanel = true;
            m_ScratchResult.OmakeList.Clear();
            for (int i = 0; i < MainMenuParam.m_GachaOmakeID.Length; i++)
            {
                OmakeDataContext _newData = new OmakeDataContext();
                MainMenuUtil.GetPresentIcon(
                    MasterDataUtil.GetPresentParamFromID(MainMenuParam.m_GachaOmakeID[i]),
                    sprite =>
                    {
                        _newData.IconImage = sprite;
                    });
                _newData.Count = MasterDataUtil.GetPresentCount(MasterDataUtil.GetPresentParamFromID(MainMenuParam.m_GachaOmakeID[i]));
                m_ScratchResult.OmakeList.Add(_newData);
            }
            m_ScratchResult.omakeBG.alpha = 0.0f;
            m_updateLayoutCount = 5;
        }

        //--------------------------------
        // キャラアイコンキャッシュ処理
        //--------------------------------
        for (int i = 0; i < MainMenuParam.m_GachaUnitUniqueID.Length; i++)
        {
            PacketStructUnit unit = UserDataAdmin.Instance.SearchChara(MainMenuParam.m_GachaUnitUniqueID[i]);
            if (unit == null)
            {
                continue;
            }

            UnitIconImageProvider.Instance.Get(unit.id,
                sprite =>
                {
                    //キャッシュするだけなので設定する必要がない
                }
                , true);
        }

        if (m_GachaEffectPrefab == null)
        {
            GameObject _tmpObj = Resources.Load("Prefab/ScratchResult/SceneObjReferMainMenuScratchEffect") as GameObject;
            m_GachaEffectPrefab = Instantiate(_tmpObj) as GameObject;
        }
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        base.PageSwitchEventDisableAfter(eNextMainMenuSeq);

        Destroy(m_GachaEffectPrefab);
        m_GachaEffectPrefab = null;

        return false;

    }

    //-----------------------------
    //キャラクターのレア度を取得
    //-----------------------------
    private MasterDataDefineLabel.RarityType GetRareFromUniqueID(uint fix_id)
    {
        MasterDataParamChara _master = MasterFinder<MasterDataParamChara>.Instance.Find((int)fix_id);
        if (_master == null) return MasterDataDefineLabel.RarityType.STAR_1;
        return _master.rare;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	スクラッチカード開封リスト管理番号検索
		@note	開封対象リスト内に番号あったらtrue返却
				v400 スクラッチカード一括開封対応 Developer add
	*/
    //----------------------------------------------------------------------------
    private bool ChkOpenGachaTicket(long nUniqueId)
    {
        bool bRet = false;

        if (m_GachaTkOpenUIdList == null)
        {
            return bRet;
        }

        for (int nNum = 0; nNum < m_GachaTkOpenUIdList.Length; nNum++)
        {
            if (m_GachaTkOpenUIdList[nNum] == nUniqueId)
            {
                bRet = true;
                break;
            }
        }

        return bRet;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	タッチしたパネルに紐づくユニットのユニークIDを取得
		@note	v400 スクラッチカード一括開封対応 Developer add
	*/
    //----------------------------------------------------------------------------
    private long GetTouchScratchUniqueId(int nTouchNo)
    {
        long unUniqueId = 0;
        if (m_GachaPlayMore)
        {
            //9連越えの場合、専用の保存領域から取得
            unUniqueId = m_GachaTkOpenUIdList[nTouchNo];
        }
        else
        {
            unUniqueId = MainMenuParam.m_GachaUnitUniqueID[nTouchNo];
        }

        return unUniqueId;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ガチャ演出シーケンス：選ばせる→選んだとこが駒の絵に置き換わる
	*/
    //----------------------------------------------------------------------------
    private bool ScratchSeqSelect()
    {

#if DEF_SCRATCH_MEKURI_WAIT_NONE
        //--------------------------------
        // なにも待たない
        //--------------------------------
#elif true
		//--------------------------------
		// アニメーション待ち
		//--------------------------------
		if(	ChkPlayingAnimation()	== true )
			return true;
#endif

        //----------------------------------------
        // アニメーション終わった子のプラス値を表示
        //----------------------------------------
        for (int i = 0; i < m_ScratchPanel.Length; i++)
        {
            if (m_ScratchPanel[i].touchIndex >= 0
            && m_ScratchPanel[i].touchIndex < m_ScratchTouchCt
            )
            {
                //v400 Developer
                //PacketStructUnit cPacketUnit = UserDataAdmin.Instance.SearchChara( MainMenuParam.m_GachaUnitUniqueID[ m_ScratchPanel[ i ].m_ScratchTouchNum ] );
                PacketStructUnit cPacketUnit = UserDataAdmin.Instance.SearchChara(GetTouchScratchUniqueId(m_ScratchPanel[i].touchIndex));

                if (cPacketUnit != null)
                {
                    if (cPacketUnit.add_pow > 0
                    || cPacketUnit.add_hp > 0
                    )
                    {
                        setPlusFlagPanel(i, true);
                    }
                }
            }
        }

        //--------------------------------
        // スクラッチをめくった数が課金数に達したら
        // スクラッチの結果を開くシーケンスへ移行
        //--------------------------------
        int nCnt = 0;
        if (m_GachaPlayMore)
        {
            // v400 スクラッチカード一括開封対応 Developer add
            nCnt = m_GachaTkOpenUIdList.Length;
        }
        else
        {
            nCnt = MainMenuParam.m_GachaUnitUniqueID.Length;
        }
        if (m_ScratchTouchCt >= nCnt)
        {
            m_ScratchSeq = GachaSeq.SELECT_OPEN;
#if DEF_SCRATCH_WAIT_NONE
            m_ScratchWait = 0.1f;
#else
			m_ScratchWait	= 0.3f;
#endif
            return true;
        }

        return true;
    }

    //
    public void OnTouchPanel(ScratchPanel _panel)
    {
        if (m_ScratchSeq == GachaSeq.SELECT)
        {
            SelectPanel(_panel);
        }
        else if (m_ScratchSeq == GachaSeq.END_WAIT &&
                  m_bEnableCharaDetail)
        {
            if (_panel.touchIndex != -1)
                TouchOpenedPanel(_panel);
        }
    }

    private void SelectPanel(ScratchPanel _panel)
    {
        //----------------------------------------
        // 既に選択済みならスルー
        //----------------------------------------
        if (_panel.touchIndex != -1)
        {
            return;
        }

        //----------------------------------------
        // １枚でもめくるとスクラッチの中断復帰情報はそこで破棄。
        // 全部解消するまで残しておくと、
        // 連続スクラッチで違うとこめくっても同じ順番で出ることで苦情が来そうなので子の対応にしている
        //----------------------------------------
        LocalSaveManager.Instance.SaveFuncScratch(null);

        //----------------------------------------
        // スクラッチが選択されたならそこを駒化
        // レア度によってパネルの模様を更新
        //----------------------------------------
        {
            //--------------------------------
            // 虹確定イベント
            //--------------------------------
            if (MainMenuParam.m_GachaRainbowDecide != 0)
            {
                // v400 スクラッチチケットで９連越えのパターンが増えたため条件を増やしました
                if ((m_GachaPlayMore && m_ScratchCenterOpen == false)
                || (MainMenuParam.m_GachaUnitUniqueID.Length == SCRATCH_MAX && m_ScratchCenterOpen == false))
                {
                    // 真ん中をタッチした場合
                    if (_panel.Index == 4)
                    {
                        // 抽選データの最後にあるデータ(9連で最後にひかれたイベント用のテーブルから排出されたユニット)
                        // を次に表示されるユニットと入れ替える
                        long lTemp = 0;
                        if (m_GachaPlayMore)
                        {
                            // v400 スクラッチチケットで９連越えの場合
                            lTemp = m_GachaTkOpenUIdList[m_ScratchTouchCt];
                            m_GachaTkOpenUIdList[m_ScratchTouchCt] = m_GachaTkOpenUIdList[SCRATCH_MAX - 1];
                            m_GachaTkOpenUIdList[SCRATCH_MAX - 1] = lTemp;
                        }
                        else
                        {
                            lTemp = MainMenuParam.m_GachaUnitUniqueID[m_ScratchTouchCt];
                            MainMenuParam.m_GachaUnitUniqueID[m_ScratchTouchCt] = MainMenuParam.m_GachaUnitUniqueID[SCRATCH_MAX - 1];
                            MainMenuParam.m_GachaUnitUniqueID[SCRATCH_MAX - 1] = lTemp;
                        }

                        int iTemp = m_ScratchRareList[m_ScratchTouchCt];
                        m_ScratchRareList[m_ScratchTouchCt] = m_ScratchRareList[SCRATCH_MAX - 1];
                        m_ScratchRareList[SCRATCH_MAX - 1] = iTemp;
                        m_ScratchCenterOpen = true;
                    }
                }
            }

            //--------------------------------
            // 指定番目のキャラ情報を取得
            //--------------------------------
            PacketStructUnit cPacketUnit = UserDataAdmin.Instance.SearchChara(GetTouchScratchUniqueId(m_ScratchTouchCt));

            if (cPacketUnit == null)
            {
                //--------------------------------
                // パケット情報が取得できないのは想定外なのでエラー
                //--------------------------------
                Debug.LogError("PacketData None!");
                return;
            }
            MasterDataParamChara cCharaMasterData = MasterDataUtil.GetCharaParamFromID(cPacketUnit.id);
            if (cCharaMasterData == null)
            {
                //--------------------------------
                // マスターデータが取得できないのは想定外なのでエラー
                //--------------------------------
                Debug.LogError("MasterData None!");
                return;
            }

            //--------------------------------
            // キャラのアイコンに差替え
            //--------------------------------
            ScratchPanelToPiece(_panel, cCharaMasterData, false, m_ScratchTouchCt);

            m_ScratchTouchNumList[m_ScratchTouchCt] = _panel.Index;
            m_ScratchTouchCt++;
        }
    }

    private void TouchOpenedPanel(ScratchPanel _panel)
    {
        Debug.Assert(_panel.touchIndex == -1, "The argument of TouchOpenedPanel() must be the opend panel");

        long _unique_id = MainMenuParam.m_GachaUnitUniqueID[_panel.touchIndex];
        PacketStructUnit _unit = UserDataAdmin.Instance.SearchChara(_unique_id);
        if (MainMenuManager.HasInstance)
        {
            UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo();
            if (_info == null) return;
            PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(_unit);
            _info.SetUnitFavorite(_unit, _subUnit, null);

            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	ガチャ演出シーケンス：選び終わったら１つずつ開いてく→見たことないならキャラ詳細へ
	*/
    //----------------------------------------------------------------------------
    private bool ScratchSeqSelectOpen()
    {
        if (m_ScratchSeqTriger == true)
        {
            //----------------------------------------
            // アニメーション終わった子のプラス値を表示
            //----------------------------------------
            for (int i = 0; i < m_ScratchPanel.Length; i++)
            {
                if (m_ScratchPanel[i].touchIndex >= 0
                && m_ScratchPanel[i].touchIndex < m_ScratchTouchCt
                )
                {
                    //v400 Developer
                    //PacketStructUnit cChkUnit = UserDataAdmin.Instance.SearchChara( MainMenuParam.m_GachaUnitUniqueID[ m_ScratchPanel[ i ].m_ScratchTouchNum ] );
                    PacketStructUnit cChkUnit = UserDataAdmin.Instance.SearchChara(GetTouchScratchUniqueId(m_ScratchPanel[i].touchIndex));

                    if (cChkUnit != null)
                    {
                        if (cChkUnit.add_pow > 0
                        || cChkUnit.add_hp > 0
                        )
                        {
                            setPlusFlagPanel(i, true);
                        }
                    }
                }
            }
        }

        //--------------------------------
        // 再生待ちチェック
        //--------------------------------
        //		if( ChkPlayingEffect()		== true
        //		||	ChkPlayingAnimation()	== true )
        if (ChkPlayingAnimation() == true)
        {
            return true;
        }
        //--------------------------------
        // 演出を時間差でやるため待ち
        //--------------------------------
        m_ScratchWait -= Time.deltaTime;
        if (m_ScratchWait >= 0.0f)
        {
            return true;
        }
#if DEF_SCRATCH_WAIT_NONE
        m_ScratchWait = 0.1f;
#else
		m_ScratchWait = 0.3f;
#endif

        //--------------------------------
        // 入手したユニットのキャラ詳細へ移行
        //--------------------------------
        if (m_ScratchDetailChkCt < m_ScratchTouchOpenCt)
        {
            //v400 Developer
            //PacketStructUnit cPacketUnitPrev = UserDataAdmin.Instance.SearchChara( MainMenuParam.m_GachaUnitUniqueID[ m_ScratchDetailChkCt ] );
            PacketStructUnit cPacketUnitPrev = UserDataAdmin.Instance.SearchChara(GetTouchScratchUniqueId(m_ScratchDetailChkCt));

            if (cPacketUnitPrev != null)
            {
                MasterDataParamChara cCharaMasterDataPrev = MasterDataUtil.GetCharaParamFromID(cPacketUnitPrev.id);
                if (cCharaMasterDataPrev != null)
                {
                    bool bDetailEnable = false;

                    //--------------------------------
                    // 新規入手ユニットなら詳細へ
                    //
                    // ※２回連続で同じ新規ユニットを引いた時に重複で詳細へ飛ばないようにフラグを立てておく
                    //--------------------------------
                    if (ServerDataUtil.ChkBitFlag(ref MainMenuParam.m_GachaPrevUnitGetFlag, cCharaMasterDataPrev.fix_id) == false)
                    {
                        ServerDataUtil.AddBitFlag(ref MainMenuParam.m_GachaPrevUnitGetFlag, cCharaMasterDataPrev.fix_id);
                        bDetailEnable = true;
                    }
                    //--------------------------------
                    // １連続ガチャの場合は無条件で詳細へ
                    //--------------------------------
                    if (MainMenuParam.m_GachaGetUnitNum <= SCRATCH_MAX
                    && MainMenuParam.m_GachaUnitUniqueID.Length == 1)
                    {
                        bDetailEnable = true;
                    }

                    //詳細演出設定OFF
                    if (!m_ScratchResult.IsUnitDetail)
                    {
                        bDetailEnable = false;
                    }

                    //チュートリアル中は強制演出
                    if (TutorialManager.IsExists)
                    {
                        bDetailEnable = true;
                    }

                    if (bDetailEnable == true)
                    {
                        // ユニット詳細用データ設定
                        m_ScratchWait = 0.5f;
                        m_ScratchDetailChkCt = m_ScratchTouchOpenCt;
                        if (MainMenuManager.HasInstance)
                        {
                            UnitDetailInfo _info = MainMenuManager.Instance.OpenUnitDetailInfo(false, true);
                            if (_info != null)
                            {
                                PacketStructUnit _subUnit = UserDataAdmin.Instance.SearchLinkUnit(cPacketUnitPrev);
                                _info.SetUnitFavorite(cPacketUnitPrev, _subUnit, null);
                                _info.IsViewCharaCount = false;
                                return true;
                            }
                        }
                    }
                }
            }
            m_ScratchDetailChkCt = m_ScratchTouchOpenCt;
        }

        //--------------------------------
        // 開く操作がタッチ数以上になってるなら作業完遂のため答え合わせへ遷移
        //--------------------------------
        if (m_ScratchTouchOpenCt >= m_ScratchTouchCt)
        {
#if DEF_SCRATCH_ANSWER_SKIP

            //----------------------------------------
            // 答え合わせを行わない版の実装。
            //----------------------------------------
            m_ScratchSeq = GachaSeq.END_WAIT;

#else
            m_ScratchWait	= 0.5f;
			m_ScratchSeq	= GACHA_SEQ_ANSWER;
#endif
            return true;
        }

        //--------------------------------
        //
        //--------------------------------
        int nScratchNum = m_ScratchTouchNumList[m_ScratchTouchOpenCt];

        //--------------------------------
        // 指定番目のキャラ情報を取得
        //--------------------------------
        //PacketStructUnit cPacketUnit = UserDataAdmin.Instance.SearchChara( MainMenuParam.m_GachaUnitUniqueID[ m_ScratchTouchOpenCt ] );
        PacketStructUnit cPacketUnit = UserDataAdmin.Instance.SearchChara(GetTouchScratchUniqueId(m_ScratchTouchOpenCt));

        if (cPacketUnit == null)
        {
            //--------------------------------
            // パケット情報が取得できないのは想定外なのでエラー
            //--------------------------------
            Debug.LogError("PacketData None!");
            return false;
        }
        MasterDataParamChara cCharaMasterData = MasterDataUtil.GetCharaParamFromID(cPacketUnit.id);
        if (cCharaMasterData == null)
        {
            //--------------------------------
            // マスターデータが取得できないのは想定外なのでエラー
            //--------------------------------
            Debug.LogError("MasterData None!");
            return false;
        }
        //--------------------------------
        // 指定パネルをキャラアイコン化
        //--------------------------------
        ScratchPanelToChara(nScratchNum, cCharaMasterData, false);

        //--------------------------------
        // 次の更新でキャラ詳細へ飛ぶならちょっとディレイを長めにとる
        //--------------------------------
        bool bDetailEnable2 = false;
        switch (cCharaMasterData.rare)
        {
            case MasterDataDefineLabel.RarityType.STAR_1: bDetailEnable2 = false; break;
            case MasterDataDefineLabel.RarityType.STAR_2: bDetailEnable2 = false; break;
            case MasterDataDefineLabel.RarityType.STAR_3: bDetailEnable2 = false; break;
            case MasterDataDefineLabel.RarityType.STAR_4: bDetailEnable2 = true; break;
            case MasterDataDefineLabel.RarityType.STAR_5: bDetailEnable2 = true; break;
            case MasterDataDefineLabel.RarityType.STAR_6: bDetailEnable2 = true; break;
            case MasterDataDefineLabel.RarityType.STAR_7: bDetailEnable2 = true; break;
        }
        if (bDetailEnable2 == true)
        {
            m_ScratchWait = 0.5f;
        }

        //----------------------------------------
        // SE再生
        //----------------------------------------
        switch (cCharaMasterData.rare)
        {
            case MasterDataDefineLabel.RarityType.STAR_1: SoundUtil.PlaySE(SEID.SE_MM_C01_SCRATCH_1_3); break;
            case MasterDataDefineLabel.RarityType.STAR_2: SoundUtil.PlaySE(SEID.SE_MM_C01_SCRATCH_1_3); break;
            case MasterDataDefineLabel.RarityType.STAR_3: SoundUtil.PlaySE(SEID.SE_MM_C01_SCRATCH_1_3); break;
            case MasterDataDefineLabel.RarityType.STAR_4: SoundUtil.PlaySE(SEID.SE_MM_C02_SCRATCH_4); break;
            case MasterDataDefineLabel.RarityType.STAR_5: SoundUtil.PlaySE(SEID.SE_MM_C03_SCRATCH_5_6); break;
            case MasterDataDefineLabel.RarityType.STAR_6: SoundUtil.PlaySE(SEID.SE_MM_C03_SCRATCH_5_6); break;
            case MasterDataDefineLabel.RarityType.STAR_7: SoundUtil.PlaySE(SEID.SE_MM_C03_SCRATCH_5_6); break;
        }

        //--------------------------------
        // パネルを開く
        //--------------------------------
        m_ScratchTouchOpenCt++;

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	ガチャ演出シーケンス：入力あるまで待ち
	*/
    //----------------------------------------------------------------------------
    private bool ScratchSeqEndWait()
    {
        if (m_ScratchSeqTriger)
        {
            m_ScratchResult.IsViewSubPanel = false;
            m_ScratchResult.IsViewSkipButton = false;

            // 戻るボタンの表示
            setupReturnButton();

            //フッターを復帰させる
            if (MainMenuManager.HasInstance)
            {
                // TODO : 各ページごとの演出をまとめる
                MainMenuManager.Instance.SetMenuHeaderActive(true);
                MainMenuManager.Instance.ShowFooter();
                EffectProcessor.Instance.PlayOrderly(new List<string> { "MainMenuFooter" });
            }

            //キャラクタ詳細への遷移有効化
            m_bEnableCharaDetail = true;

            //
            StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
        }

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	スクラッチパネル操作：駒化
		@param[in]	ScratchPanel    		( _panel        		) スクラッチパネル
		@param[in]	MasterDataParamChara	( cMasterDataCharaParam	) スクラッチパネルに割り当てられたキャラマスターデータ
		@param[in]	bool					( bAnswer				) 答え合わせフラグ
		@param[out]	bool					(						) 成否
	*/
    //----------------------------------------------------------------------------
    private bool ScratchPanelToPiece(ScratchPanel _panel, MasterDataParamChara cMasterDataCharaParam, bool bAnswer, int nTouchNum)
    {

        //--------------------------------
        // 割り当てられたキャラに設定されているレア度によって
        // 駒の絵を分岐して設定
        //--------------------------------
        switch (cMasterDataCharaParam.rare)
        {
            case MasterDataDefineLabel.RarityType.STAR_1: _panel.BackImage = ResourceManager.Instance.Load("chara_icon_rare01"); break;
            case MasterDataDefineLabel.RarityType.STAR_2: _panel.BackImage = ResourceManager.Instance.Load("chara_icon_rare02"); break;
            case MasterDataDefineLabel.RarityType.STAR_3: _panel.BackImage = ResourceManager.Instance.Load("chara_icon_rare03"); break;
            case MasterDataDefineLabel.RarityType.STAR_4: _panel.BackImage = ResourceManager.Instance.Load("chara_icon_rare04"); break;
            case MasterDataDefineLabel.RarityType.STAR_5: _panel.BackImage = ResourceManager.Instance.Load("chara_icon_rare05"); break;
            case MasterDataDefineLabel.RarityType.STAR_6: _panel.BackImage = ResourceManager.Instance.Load("chara_icon_rare06"); break;
            case MasterDataDefineLabel.RarityType.STAR_7: _panel.BackImage = ResourceManager.Instance.Load("chara_icon_rare07"); break;
        }

        //--------------------------------
        // パネルにアニメーション再生指示
        //--------------------------------
        if (_panel.animClipScrath != null)
        {
            AnimationClipScratch.SCRATCH_ANIM eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.MAX;
            if (bAnswer == true)
            {
                switch (cMasterDataCharaParam.rare)
                {
                    case MasterDataDefineLabel.RarityType.STAR_1: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_PIECE_1; break;
                    case MasterDataDefineLabel.RarityType.STAR_2: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_PIECE_2; break;
                    case MasterDataDefineLabel.RarityType.STAR_3: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_PIECE_3; break;
                    case MasterDataDefineLabel.RarityType.STAR_4: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_PIECE_4; break;
                    case MasterDataDefineLabel.RarityType.STAR_5: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_PIECE_5; break;
                    case MasterDataDefineLabel.RarityType.STAR_6: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_PIECE_6; break;
                    case MasterDataDefineLabel.RarityType.STAR_7: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_PIECE_6; break;
                }
            }
            else
            {
                switch (cMasterDataCharaParam.rare)
                {
                    case MasterDataDefineLabel.RarityType.STAR_1: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_PIECE_1; break;
                    case MasterDataDefineLabel.RarityType.STAR_2: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_PIECE_2; break;
                    case MasterDataDefineLabel.RarityType.STAR_3: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_PIECE_3; break;
                    case MasterDataDefineLabel.RarityType.STAR_4: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_PIECE_4; break;
                    case MasterDataDefineLabel.RarityType.STAR_5: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_PIECE_5; break;
                    case MasterDataDefineLabel.RarityType.STAR_6: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_PIECE_6; break;
                    case MasterDataDefineLabel.RarityType.STAR_7: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_PIECE_6; break;
                }
            }
            _panel.animClipScrath.PlayScratchAnimation(eScratchAnim);
        }

        //--------------------------------
        // 演出としてパネルの位置にエフェクトを出す
        //--------------------------------
        if (EffectManager.Instance != null)
        {
            //--------------------------------
            // レア度でエフェクト分岐
            //--------------------------------
            GameObject cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare1;
            if (bAnswer == false)
            {
                switch (cMasterDataCharaParam.rare)
                {
                    case MasterDataDefineLabel.RarityType.STAR_1: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare1; break;
                    case MasterDataDefineLabel.RarityType.STAR_2: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare2; break;
                    case MasterDataDefineLabel.RarityType.STAR_3: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare3; break;
                    case MasterDataDefineLabel.RarityType.STAR_4: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare4; break;
                    case MasterDataDefineLabel.RarityType.STAR_5: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare5; break;
                    case MasterDataDefineLabel.RarityType.STAR_6: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare6; break;
                    case MasterDataDefineLabel.RarityType.STAR_7: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare6; break;
                }
            }
            else
            {
                cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaAnswerToPiece;
            }
            if (cEffectObj != null)
            {
                Vector3 vPos = new Vector3(0, 0, 0);
                GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vPos, cEffectObj, _panel.gameObject);
                _effObj.transform.SetParent(_panel.gameObject.transform, false);
                UnityUtil.SetSortingOrder(_effObj, "EFFECT");
                _effObj.transform.localPosition = new Vector3(0, 0, 0);
                _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);
            }
        }


        //--------------------------------
        // めくりSE再生
        //--------------------------------
        SoundUtil.PlaySE(IsSpecialPanel(_panel)
                    ? SEID.SE_INGAME_PANEL_MEKURI_SPECIAL
                    : SEID.SE_INGAME_PANEL_MEKURI_NORMAL);

        //--------------------------------
        // パネルの状態更新
        //--------------------------------
        _panel.panelSeq = ScratchPanel.PanelSeq.SEQ_SELECT;
        _panel.touchIndex = nTouchNum;
        return true;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	スクラッチパネル操作：キャラ化
		@param[in]	int						( nPanelAccessNum		) スクラッチパネルのアクセス番号
		@param[in]	MasterDataParamChara	( cMasterDataCharaParam	) スクラッチパネルに割り当てられたキャラマスターデータ
		@param[in]	bool					( bAnswer				) 答え合わせフラグ
		@param[out]	bool					(						) 成否
	*/
    //----------------------------------------------------------------------------
    private bool ScratchPanelToChara(int nPanelAccessNum, MasterDataParamChara cMasterDataCharaParam, bool bAnswer)
    {
        //--------------------------------
        // 指定パネルをキャラアイコン化
        //--------------------------------
        UnitIconImageProvider.Instance.Get(
            cMasterDataCharaParam.fix_id,
            sprite =>
            {
                m_ScratchPanel[nPanelAccessNum].FrontImage = sprite;
            }
            , true);

        //--------------------------------
        // パネルにアニメーション再生指示
        //--------------------------------
        if (m_ScratchPanel[nPanelAccessNum].animClipScrath != null)
        {
            AnimationClipScratch.SCRATCH_ANIM eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.MAX;
            if (bAnswer == true)
            {
                switch (cMasterDataCharaParam.rare)
                {
                    case MasterDataDefineLabel.RarityType.STAR_1: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_CHARA_1; break;
                    case MasterDataDefineLabel.RarityType.STAR_2: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_CHARA_2; break;
                    case MasterDataDefineLabel.RarityType.STAR_3: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_CHARA_3; break;
                    case MasterDataDefineLabel.RarityType.STAR_4: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_CHARA_4; break;
                    case MasterDataDefineLabel.RarityType.STAR_5: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_CHARA_5; break;
                    case MasterDataDefineLabel.RarityType.STAR_6: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_CHARA_6; break;
                    case MasterDataDefineLabel.RarityType.STAR_7: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.ANSWER_TO_CHARA_6; break;
                }
            }
            else
            {
                switch (cMasterDataCharaParam.rare)
                {
                    case MasterDataDefineLabel.RarityType.STAR_1: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_CHARA_1; break;
                    case MasterDataDefineLabel.RarityType.STAR_2: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_CHARA_2; break;
                    case MasterDataDefineLabel.RarityType.STAR_3: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_CHARA_3; break;
                    case MasterDataDefineLabel.RarityType.STAR_4: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_CHARA_4; break;
                    case MasterDataDefineLabel.RarityType.STAR_5: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_CHARA_5; break;
                    case MasterDataDefineLabel.RarityType.STAR_6: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_CHARA_6; break;
                    case MasterDataDefineLabel.RarityType.STAR_7: eScratchAnim = AnimationClipScratch.SCRATCH_ANIM.SELECT_TO_CHARA_6; break;
                }
            }

            m_ScratchPanel[nPanelAccessNum].animClipScrath.PlayScratchAnimation(eScratchAnim);
        }

        //--------------------------------
        // 演出としてパネルの位置にエフェクトを出す
        //--------------------------------
        if (EffectManager.Instance != null)
        {
            //--------------------------------
            // レア度でエフェクト分岐
            //--------------------------------
            GameObject cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaRare1;
            if (bAnswer == false)
            {
                // 本来は新規入手ユニットか否かで分岐するのが正しい。
                // セーブデータができるまでは暫定対応
                cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaToUnit;
                switch (cMasterDataCharaParam.rare)
                {
                    case MasterDataDefineLabel.RarityType.STAR_1: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaToUnit; break;
                    case MasterDataDefineLabel.RarityType.STAR_2: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaToUnit; break;
                    case MasterDataDefineLabel.RarityType.STAR_3: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaToUnit; break;
                    case MasterDataDefineLabel.RarityType.STAR_4: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaToUnitNew; break;
                    case MasterDataDefineLabel.RarityType.STAR_5: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaToUnitNew; break;
                    case MasterDataDefineLabel.RarityType.STAR_6: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaToUnitNew; break;
                    case MasterDataDefineLabel.RarityType.STAR_7: cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaToUnitNew; break;
                }
            }
            else
            {
                cEffectObj = SceneObjReferMainMenuScratchEffect.Instance.m_EffectPrefabGachaAnswerToUnit;
            }
            if (cEffectObj != null)
            {
                Vector3 vEffectPos = new Vector3(0, 0, 0);
                GameObject _effObj = EffectManager_OLD.CreateEffect2D(ref vEffectPos, cEffectObj, m_ScratchPanel[nPanelAccessNum].gameObject);
                _effObj.transform.SetParent(m_ScratchPanel[nPanelAccessNum].gameObject.transform, false);
                UnityUtil.SetSortingOrder(_effObj, "EFFECT");
                _effObj.transform.localPosition = new Vector3(0, 0, 0);
                _effObj.transform.localScale = new Vector3(EFFECT_SCALE, EFFECT_SCALE, EFFECT_SCALE);
            }
        }

        //--------------------------------
        // パネルの状態更新
        //--------------------------------
        m_ScratchPanel[nPanelAccessNum].panelSeq = ScratchPanel.PanelSeq.SEQ_OPEN;
        return true;
    }


    private bool IsSpecialPanel(ScratchPanel panel)
    {
        return MainMenuParam.m_GachaRainbowDecide != 0
            && panel.IsCenter();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	再生中確認：アニメーション
		@param[out]	bool					(						) 終了フラグ
	*/
    //----------------------------------------------------------------------------
    private bool ChkPlayingAnimation()
    {
        //----------------------------------------
        // モーション再生完遂待ち
        //----------------------------------------
        bool bAnimationPlaying = false;
        for (int i = 0; i < m_ScratchPanel.Length; i++)
        {
            if (m_ScratchPanel[i] == null
            || m_ScratchPanel[i].animClipScrath == null
            || m_ScratchPanel[i].animClipScrath.ChkAnimationPlaying() == false
            )
            {
                continue;
            }

            bAnimationPlaying = true;
            break;
        }
        if (bAnimationPlaying == true)
        {
            return true;
        }
        return false;
    }

    private void setImagePanelAll(string _name)
    {
        for (int i = 0; i < m_ScratchResult.scratchPanelList.Length; i++)
        {
            setImagePanelF(i, _name);
            setImagePanelB(i, _name);
        }
    }

    private void setImagePanelF(int _index, string _name)
    {
        m_ScratchPanel[_index].FrontImage = ResourceManager.Instance.Load(_name);
    }
    private void setImagePanelB(int _index, string _name)
    {
        m_ScratchPanel[_index].BackImage = ResourceManager.Instance.Load(_name);
    }

    private void setPlusFlagPanelAll(bool bFlag)
    {
        for (int i = 0; i < m_ScratchResult.scratchPanelList.Length; i++)
        {
            setPlusFlagPanel(i, bFlag);
        }
    }
    private void setPlusFlagPanel(int _index, bool bFlag)
    {
        m_ScratchPanel[_index].IsViewPlus = bFlag;
    }

    private void settinGacha(MasterDataGacha _master)
    {
        if (_master == null)
        {
            return;
        }

        //
        m_ScratchResult.IsUnitDetail = LocalSaveManager.Instance.LoadFuncScratchDetailSkip();
        m_ScratchResult.IsViewReturnButton = false;
        m_ScratchResult.IsViewSkipButton = true;
        //チュートリアルガチャはスキップできないように
        if (_master.type == MasterDataDefineLabel.GachaType.TUTORIAL)
        {
            m_ScratchResult.IsViewSkipButton = false;
        }

        switch (_master.type)
        {
            case MasterDataDefineLabel.GachaType.NORMAL:
            case MasterDataDefineLabel.GachaType.LUNCH:
            default:
                {
                    m_ScratchResult.MainPanel = ResourceManager.Instance.Load("silver_h");
                    m_ScratchResult.SubPanel = ResourceManager.Instance.Load("silver_o");
                    m_ScratchResult.SubLabel = ResourceManager.Instance.Load("silver_omake");
                    setImagePanelAll("open_nrm");
                }
                break;
            case MasterDataDefineLabel.GachaType.RARE:
            case MasterDataDefineLabel.GachaType.EVENT:
            case MasterDataDefineLabel.GachaType.TUTORIAL:
            case MasterDataDefineLabel.GachaType.TICKET:
            case MasterDataDefineLabel.GachaType.EVENT_POINT:
            case MasterDataDefineLabel.GachaType.ITEM_POINT:
            case MasterDataDefineLabel.GachaType.STEP_UP:
                {
                    m_ScratchResult.MainPanel = ResourceManager.Instance.Load("gold_h");
                    m_ScratchResult.SubPanel = ResourceManager.Instance.Load("gold_o");
                    m_ScratchResult.SubLabel = ResourceManager.Instance.Load("gold_omake");
                    setImagePanelAll("open_rare");
                }
                break;
        }
    }

    private void OnSelectSkip()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK);
        m_ScratchResult.IsUnitDetail = m_ScratchResult.IsUnitDetail ? false : true;
        LocalSaveManager.Instance.SaveFuncScratchDetailSkip(m_ScratchResult.IsUnitDetail);
    }

    /// <summary>
    /// スクラッチトップ画面に戻る
    /// </summary>
    void OnReturnScratch()
    {
        if (TutorialManager.IsExists == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);

        if (MasterDataUtil.CheckActiveGachaMaster(MainMenuParam.m_GachaMaster) == false)
        {
            // 現在のスクラッチが有効ではない場合、先頭のを設定
            MasterDataGacha[] gachaArray = MasterDataUtil.GetActiveGachaMaster();
            if (gachaArray != null && gachaArray.Length > 0)
            {
                MainMenuParam.m_GachaMaster = gachaArray[0];
            }
        }

        AndroidBackKeyManager.Instance.StackPop(m_CanvasObj.gameObject);
        MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_GACHA_MAIN, false, false);
    }

    void setupReturnButton()
    {
        m_ScratchResult.IsViewReturnButton = true;

        //Homeに戻るバックキー有効化
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.EnableBackKey();
        }
        if (AndroidBackKeyManager.HasInstance)
        {
            //バックキーが押された時のアクションを登録
            AndroidBackKeyManager.Instance.StackPush(m_CanvasObj.gameObject, OnReturnScratch);
        }
    }
}


