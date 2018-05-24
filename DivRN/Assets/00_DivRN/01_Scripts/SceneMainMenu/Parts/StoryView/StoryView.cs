/**
 *  @file   StoryView.cs
 *  @brief
 *  @author Developer
 *  @date   2017/02/06
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using M4u;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using ServerDataDefine;
using TMPro;

public class StoryView : MonoBehaviour
{

    public enum STORY_FINISH_TYPE
    {
        SUCCESS = 1,
        SKIP,
        ERROR_IMPORT,
        ERROR_QUEST_START,
    }

    /// <summary>キャラクターのカットインスピード</summary>
    const float CHARACTER_CUT_IN_DURATION = 0.25f;
    /// <summary>キャラクターのフォーカススピード</summary>
    const float CHARACTER_FOCUS_DURATION = 0.25f;
    /// <summary>文字送りアニメーション操作用のID</summary>
    enum TWEEN_OBJECT_ID
    {
        NONE = 100,
        TEXT_BOX,
        TEXT_BOX_CURSOR,
        CHARACTER_SLIDE_01,
        CHARACTER_SLIDE_02,
        CHARACTER_SLIDE_03,
        CHARACTER_SLIDE_04,
        CHARACTER_FOCUS_01,
        CHARACTER_FOCUS_02,
        CHARACTER_FOCUS_03,
        CHARACTER_FOCUS_04,
        MAIN_CONTENT,
        SCENARIO_CONTENT,
        BACK_GROUND,
        TOP_MASK,
        BOTTOM_MASK,
    }

    [SerializeField]
    private GameObject m_TopMask;
    [SerializeField]
    private GameObject m_BottomMask;
    [SerializeField]
    private GameObject m_SelectButtons;

    /// <summary>表示中のイベントの数</summary>
    public static int CutInCounter = 0;

    public StoryViewContext Context = new StoryViewContext();

    /// <summary>1文字の表示にかかる時間.</summary>
    public float m_IntervalForCharacterDisplay = 0.1f;
    /// <summary>現在の読み込み行数.</summary>
    int m_CurrentLine = 0;

    /// <summary>ストーリービューマスターデータ配列</summary>
    MasterDataStory[] m_StoryMasterDataArray;
    /// <summary>今のマスターデータ</summary>
    MasterDataStory m_CurrentStoryMasterData = new MasterDataStory();
    /// <summary>前マスターデータ</summary>
    MasterDataStory m_PrevStoryMasterData = new MasterDataStory();

    /// <summary>アセットバンドルリソースリスト</summary>
    List<AssetLoadStoryViewResource> m_AssetResourceList = new List<AssetLoadStoryViewResource>();

    [SerializeField]
    CanvasSetting m_CanvasSetting;
    [SerializeField]
    RectTransform m_ContentRect;
    /// <summary>ポーズカーソル</summary>
    [SerializeField]
    GameObject m_PauseCursor;
    [SerializeField]
    TextMeshProUGUI m_Text;
    [SerializeField]
    TextMeshProUGUI m_SelectText;
    int cutInID = -1;

    /// <summary>イベントが終わったときのアクション</summary>
    Action m_CloseAction;
    /// <summary>クエストリスト更新する時のアクション</summary>
    Action m_QuestReloadAction;

    /// <summary>シナリオを読み込む前に再生していたBGM</summary>
    BGMPlayData[] m_TmpPrevPlayDatas;

    StoryViewCharaPanel[] CharaPanels = new StoryViewCharaPanel[4];

    public MasterDataStoryChara m_DebugStoryCharaMaster = null;
    /// <summary>シナリオの終了状態</summary>
    public STORY_FINISH_TYPE m_FinishType;

    string m_HeroName;
    private bool autoDestroy;
    private bool returnableBGM;
    private bool loadingMask;
    private uint m_QuestID;

    /// <summary>commandとlabel処理</summary>
    public class ADVselect
    {
        public class ADVbranch
        {
            /// <summary>選択肢No.</summary>
            public int no = -1;
            /// <summary>選択肢ボタンテキスト</summary>
            public string text = string.Empty;
            /// <summary>ジャンプ先ラベル</summary>
            public string label = string.Empty;
            /// <summary>選択肢枠タイプ</summary>
            public int wakuType = 0;
        }
        /// <summary>選択肢リスト</summary>
        public List<ADVbranch> node = new List<ADVbranch>();
        /// <summary>分岐集約先ラベル</summary>
        public string restartLabel = string.Empty;
    }

    /// <summary>選択肢最大数</summary>
    private readonly int ADV_SELECT_BRANCH_MAX = 2;
    /// <summary>全ラベル(ラベル,行番号)</summary>
    private Dictionary<string, int> m_ADVcommandLabelTable = new Dictionary<string, int>();
    /// <summary>全選択肢(行番号,選択肢データ)</summary>
    private Dictionary<int, ADVselect> m_ADVselectList = new Dictionary<int, ADVselect>();
    /// <summary>全ダイアログ(行番号,ダイアログ番号?)</summary>
    private Dictionary<int, int> m_ADVdialogList = new Dictionary<int, int>();
    /// <summary>選択ボタンリスト</summary>
    private List<StoryViewSelectButton> m_ADVselectButtonList = new List<StoryViewSelectButton>();
    /// <summary>分岐先行番号</summary>
    private int restartNo = -1;
    /// <summary>最後に選んだ選択肢</summary>
    public ADVselect.ADVbranch m_lastSelected = new ADVselect.ADVbranch();

    void ContexAlphaZero()
    {
        Context.MainContentAlpha = 0;
        Context.ScenerioContentAlpha = 0;
        Context.BackGroundAlpha = 0;
        Context.TopMaskAlpha = 0;
        Context.BottomMaskAlpha = 0;
    }

    void Awake()
    {
        autoDestroy = true;
        returnableBGM = true;
        loadingMask = false;
        GetComponent<M4uContextRoot>().Context = Context;
        StoryViewCharaPanel[] charaPanels = GetComponentsInChildren<StoryViewCharaPanel>();
        if (charaPanels != null)
        {
            for (int i = 0; i < charaPanels.Length; ++i)
            {
                CharaPanels[i] = charaPanels[i];
                CharaPanels[i].SetUpHidePosition();
            }
        }

        ContexAlphaZero();

        //　カメラの設定
        Camera camera = SceneObjReferMainMenu.Instance.m_MainMenuGroupCamera.GetComponent<Camera>();
        Canvas[] canvasList = gameObject.GetComponentsInChildren<Canvas>();
        for (int i = 0; i < canvasList.Length; i++)
        {
            canvasList[i].worldCamera = camera;
        }

        // 選択ボタンリスト
        GetComponent<M4uContextRoot>().Context = Context;
        StoryViewSelectButton[] selectButtons = GetComponentsInChildren<StoryViewSelectButton>();
        if (selectButtons != null)
        {
            for (int i = 0; i < selectButtons.Length; ++i)
            {
                m_ADVselectButtonList.Add(selectButtons[i]);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.enebleMask(m_TopMask, m_BottomMask);
            SafeAreaControl.Instance.addLocalYPos(m_SelectButtons.transform, true);
        }

        SizeToFitPosition();
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnClickSkip);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (Input.GetKey(KeyCode.G))
        {
            // スキップダイアログの表示
            if (!Dialog.HasDialog())
            {
                Debug.Log("STORY_VIEW_FSM_STATE:" + StoryViewFSM.Instance.ActiveStateName);
                OnClickSkip();
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {

        }
#endif
    }

    /// <summary>
    /// 画面サイズに合わせて表示位置を変える
    /// </summary>
    public void SizeToFitPosition()
    {
        RectTransform rect = m_CanvasSetting.gameObject.GetComponent<RectTransform>();
        float height = rect.rect.height;

        if (m_ContentRect != null)
        {
            m_ContentRect.anchoredPosition = new Vector2(0, (height < 1020) ? 30 : 0);

            // 選択肢表示位置調整
            RectTransform bt = m_CanvasSetting.transform.Find("Content/CanvasGroup/SelectButtons").GetComponent<RectTransform>();
            if (bt != null)
            {
                var y = (m_ContentRect.sizeDelta.y - height) / 2 + 30;
                bt.anchoredPosition -= new Vector2(0, y > 0 ? y : 0);
            }
        }
    }

    /// <summary>
    /// ストーリービューオブジェクトの作成
    /// </summary>
    /// <returns></returns>
    public static StoryView Create()
    {
        int cutInCcount = GetStoryView().Length;

        GameObject _tmpObj = Resources.Load("Prefab/StoryView/StoryView") as GameObject;
        if (_tmpObj == null) return null;

        GameObject _newObj = Instantiate(_tmpObj) as GameObject;
        if (_newObj == null) return null;
        UnityUtil.SetObjectEnabledOnce(_newObj, true);

        StoryView cutIn = _newObj.GetComponent<StoryView>();
        if (cutIn == null) return null;

        cutIn.cutInID = CutInCounter;
        if (cutInCcount != 0)
        {
            cutIn.m_CanvasSetting.ChangeSortingOrder(cutInCcount);
        }
        CutInCounter++;

        return cutIn;
    }

    public static StoryView[] GetStoryView()
    {
        GameObject[] cutInArray = GameObject.FindGameObjectsWithTag("StoryView");
        StoryView[] _ret = new StoryView[cutInArray.Length];
        for (int i = 0; i < cutInArray.Length; i++)
        {
            _ret[i] = cutInArray[i].GetComponent<StoryView>();
        }
        return _ret;
    }

    /// <summary>
    /// ストーリービューを削除する
    /// </summary>
    public static void HideAll()
    {
        StoryView[] cutInArray = GetStoryView();
        foreach (StoryView dlg in cutInArray)
        {
            if (dlg != null) dlg.Hide();
        }
    }

    /// <summary>
    /// ストーリービューがあるかどうか
    /// </summary>
    /// <returns></returns>
    public static bool HasCutIn()
    {
        StoryView[] cutInArray = GetStoryView();
        if (cutInArray.Length == 0) return false;
        return true;
    }

    /// <summary>
    /// 読み込むシナリオを取得
    /// </summary>
    /// <param name="story_id">ストーリーID</param>
    /// <param name="quest_id">クエストID</param>
    public void SetScenario(uint story_id, uint quest_id = 0)
    {
        //----------------------------------------
        // 初期化
        //----------------------------------------
        m_QuestID = quest_id;
        m_CurrentLine = -1;
        m_CurrentStoryMasterData = new MasterDataStory();
        m_PrevStoryMasterData = new MasterDataStory();
        m_Text.text = "";
        m_SelectText.text = "";
        Context.PauseCursorColor = Context.PauseCursorColor.WithAlpha(0);

        ContexAlphaZero();

        CharaPanels[0].Context.CharaImagePosX = CharaPanels[0].Context.m_CharaHidePosition.x;
        CharaPanels[1].Context.CharaImagePosX = CharaPanels[1].Context.m_CharaHidePosition.x;
        CharaPanels[2].Context.CharaImagePosX = CharaPanels[2].Context.m_CharaHidePosition.x;
        CharaPanels[3].Context.CharaImagePosX = CharaPanels[3].Context.m_CharaHidePosition.x;
        m_AssetResourceList.Clear();
        m_DebugStoryCharaMaster = null;
        m_FinishType = STORY_FINISH_TYPE.SUCCESS;

        //----------------------------------------
        // ストーリマスタの取得
        //----------------------------------------
        m_StoryMasterDataArray = MasterFinder<MasterDataStory>.Instance.SelectWhere(" where story_id = ? ", story_id).ToArray();

        //----------------------------------------
        // 主人公の名前を取得
        //----------------------------------------
        m_HeroName = "";
        PacketStructHero heroData = Array.Find(UserDataAdmin.Instance.m_StructHeroList, v => v.unique_id == UserDataAdmin.Instance.m_StructPlayer.current_hero_id);
        if (heroData != null)
        {
            MasterDataHero heroMaster = MasterFinder<MasterDataHero>.Instance.Find(heroData.hero_id);
            if (heroMaster != null)
            {
                m_HeroName = heroMaster.name;
            }
        }
    }

    /// <summary>
    /// デバッグ用に入れるデータ
    /// </summary>
    /// <param name="storyMasterArray"></param>
    /// <param name="storyCharaMaster">デバッグ用にUV設定が入っているデータ</param>
    public void SetDebugStoryData(MasterDataStory[] storyMasterArray, MasterDataStoryChara storyCharaMaster)
    {

        SetScenario(0);
        m_StoryMasterDataArray = storyMasterArray;
        m_DebugStoryCharaMaster = storyCharaMaster;
    }


    public StoryView DisableAutoDestroy()
    {
        autoDestroy = false;
        return this;
    }

    public void DisableReturnableBGM()
    {
        returnableBGM = false;
    }

    public void EnableLoadingMask()
    {
        loadingMask = true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public StoryView SetReloadQuestListEvent(Action action)
    {
        m_QuestReloadAction = action;
        return this;
    }


    /// <summary>
    /// ストーリービューを削除する
    /// </summary>
    public void Hide()
    {
        ContexAlphaZero();

        // アニメーションの削除
        foreach (TWEEN_OBJECT_ID objID in Enum.GetValues(typeof(TWEEN_OBJECT_ID)))
        {
            DOTween.Complete(objID);
            DOTween.Kill(objID);
        }

        SoundUtil.StopBGM(false);
        if (m_TmpPrevPlayDatas != null
        && returnableBGM == true)
        {
            foreach (BGMPlayData playData in m_TmpPrevPlayDatas)
            {
                SoundUtil.PlayBGM(playData);
            }
        }

        if (m_CloseAction != null)
        {
            m_CloseAction();
            m_CloseAction = null;
        }
        AndroidBackKeyManager.Instance.StackPop(gameObject);
        if (autoDestroy)
        {
            DestroyObject(gameObject);
        }
    }

    /// <summary>
    /// シナリオ開始
    /// </summary>
    public void Show()
    {
        Show(null);
    }

    /// <summary>
    /// シナリオ開始
    /// </summary>
    /// <param name="closeAction"></param>
    public void Show(Action closeAction)
    {
        m_CloseAction = closeAction;
        if (BGMManager.HasInstance)
        {
            // 前のイベンドのBGMを退避する
            m_TmpPrevPlayDatas = BGMManager.Instance.GetPlayingBGM();
        }

        DOTween.To(() => Context.MainContentAlpha, (x) => Context.MainContentAlpha = x, 1, 0.25f)
        .SetId(TWEEN_OBJECT_ID.MAIN_CONTENT)
        .OnStepComplete(() =>
        {
            LoadingManager.Instance.setOverLayMask(false);
            StoryViewFSM.Instance.SendFsmEvent("START");
        });
    }

    /// <summary>
    /// ミッション報酬の表示
    /// </summary>
    /// <param name="result"></param>
    /// <param name="finishAction"></param>
    void ShowMissionClearDialog(RecvQuest2StoryClearValue result, Action finishAction)
    {
        List<QuestMissionContext> missionList = GetMissionList();
        if (missionList.IsNullOrEmpty() == true)
        {
            if (finishAction != null)
            {
                finishAction();
            }
            return;
        }

        new SerialProcess().Add(
                        (System.Action next) =>
                        {
                            Dialog dialog = Dialog.Create(DialogType.DialogMissionList);
                            dialog.SetDialogTextFromTextkey(DialogTextType.Title, "story_clear_title");
                            dialog.SetMissionList(missionList);
                            dialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                            dialog.SetDialogEvent(DialogButtonEventType.OK, () =>
                            {
                                next();
                            });
                            dialog.SetDialogEvent(DialogButtonEventType.CANCEL, () =>
                            {
                                next();
                            });

                            dialog.DisableFadePanel();
                            dialog.EnableCancel();
                            dialog.Show();
                        }).Add(
                        (System.Action next) =>
                        {
                            if (result != null && result.reward_limit_list != null)
                            {
                                Dialog dialog = Dialog.Create(DialogType.DialogOK);
                                dialog.SetDialogTextFromTextkey(DialogTextType.Title, "mm38q_title2");
                                dialog.SetDialogTextFromTextkey(DialogTextType.MainText, "mm37q_content9");
                                dialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                                dialog.SetDialogEvent(DialogButtonEventType.OK, () =>
                                {
                                    next();
                                });
                                dialog.SetDialogEvent(DialogButtonEventType.CANCEL, () =>
                                {
                                    next();
                                });
                                dialog.DisableFadePanel();
                                dialog.EnableCancel();
                                dialog.Show();
                            }
                            else
                            {
                                next();
                            }
                        }
                        ).Add(
                        (System.Action next) =>
                        {
                            // 表示したアチーブメントを削除
                            for (int i = 0; i < missionList.Count; ++i)
                            {
                                ResidentParam.DelAchievementClear(missionList[i].fix_id);
                            }

                            if (finishAction != null)
                            {
                                finishAction();
                            }
                        }).Flush();
    }

    public List<QuestMissionContext> GetMissionList()
    {
        PacketAchievement[] _clearList = ResidentParam.GetQuestAchivementClearList(m_QuestID);
        List<QuestMissionContext> missionList = new List<QuestMissionContext>();
        if (_clearList != null && _clearList.Length != 0)
        {
            for (int i = 0; i < _clearList.Length; ++i)
            {
                if (_clearList[i].present_ids.IsNullOrEmpty() == false)
                {
                    missionList.Add(new QuestMissionContext(_clearList[i]));
                }
            }
        }

        return missionList;
    }


    #region Button OnClick
    /// <summary>
    /// 画面が押されたとき
    /// </summary>
    public void OnClick()
    {
        StoryViewFSM.Instance.SendFsmEvent("CLICK");
    }

    /// <summary>
    /// スキップボタンが押されたとき
    /// </summary>
    public void OnClickSkip()
    {
        if (Context.IsEnableSelectButton == false) { return; }

        // アニメーションの停止
        foreach (TWEEN_OBJECT_ID objID in Enum.GetValues(typeof(TWEEN_OBJECT_ID)))
        {
            DOTween.Pause(objID);
        }

        Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "storydialog_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "storydialog_content");
        newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            Context.IsEnableSelectButton = false;
            StoryViewFSM.Instance.SendFsmSkipEvent();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            // アニメーションの再開
            foreach (TWEEN_OBJECT_ID objID in Enum.GetValues(typeof(TWEEN_OBJECT_ID)))
            {
                DOTween.Play(objID);
            }
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.CANCEL, new System.Action(() =>
        {
            // アニメーションの再開
            foreach (TWEEN_OBJECT_ID objID in Enum.GetValues(typeof(TWEEN_OBJECT_ID)))
            {
                DOTween.Play(objID);
            }
        }));
        newDialog.Show();

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    /// <summary>
    /// 選択肢ボタンが押されたとき
    /// </summary>
    public void OnClickSelect(GameObject obj)
    {
        for (var i = 0; i < m_ADVselectButtonList.Count; i++)
        {
            m_ADVselectButtonList[i].Hide();
        }
        var btn = obj.GetComponent<StoryViewSelectButton>();
        UnityEngine.Debug.Assert(btn != null, "Button select ERROR !!!");
        if (btn)
        {
            restartNo = m_ADVcommandLabelTable[m_ADVselectList[m_CurrentLine].restartLabel];
            m_lastSelected = btn.GetParam();
            var next = m_ADVcommandLabelTable[m_lastSelected.label];
            m_CurrentLine = next - 1;
            OnHasNextLineMain();
        }
        else
        {
            // ここには来ないはず
            OnHasNextLineMain();
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    #endregion

    #region Play Maker Method
    /// <summary>
    /// シナリオ開始待機
    /// </summary>
    void OnWait()
    {

    }

    /// <summary>
    /// ストーリークエスト受注
    /// </summary>
    void OnSendStoryQuestStart()
    {
        if (m_QuestID == 0)
        {
            // クエストではない
            StoryViewFSM.Instance.SendFsmNextEvent();
            return;
        }

        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.TO_HOME);

        ServerDataUtilSend.SendPacketAPI_Quest2StoryStart(m_QuestID)
        .setSuccessAction(_data =>
        {
            StoryViewFSM.Instance.SendFsmNextEvent();
        })
        .setErrorAction(_data =>
        {
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.TO_HOME);
            m_FinishType = STORY_FINISH_TYPE.ERROR_QUEST_START;
            StoryViewFSM.Instance.SendFsmErrorEvent();
        })
        .SendStart();
    }

    /// <summary>
    /// 前のシーンのBGM停止待ち
    /// </summary>
    /// <returns></returns>
    IEnumerator OnLoopFadePrevBGM()
    {
        SoundUtil.StopBGM(false);
        if (BGMManager.HasInstance)
        {
            while (BGMManager.Instance.isFadeOut)
            {
                yield return null;
            }
        }
        SoundUtil.PlayBGM(BGMManager.EBGM_ID.eBGM_Story, false);

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// 必要なデータをロード開始する
    /// </summary>
    /// <returns></returns>
    IEnumerator OnLoopImportScenarioRequest()
    {
        if (m_StoryMasterDataArray.Length == 0)
        {
            m_FinishType = STORY_FINISH_TYPE.ERROR_IMPORT;
            StoryViewFSM.Instance.SendFsmErrorEvent();
            yield break;
        }

        // インジケーターを表示
        LoadingManager.Instance.RequestLoadingFinish(); // 受注時のインジケーターを非表示
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.DATA_DOWNLOAD);

        //----------------------------------------
        // 表示するキャラIDの取得
        //----------------------------------------
        List<uint> charaMasterIDList = new List<uint>();
        List<uint> backImgIDList = new List<uint>();
        List<uint> bgmIDList = new List<uint>();

        foreach (MasterDataStory master in m_StoryMasterDataArray)
        {

            if (!charaMasterIDList.Contains(master.show_character_01))
            {
                charaMasterIDList.Add(master.show_character_01);
            }

            if (!charaMasterIDList.Contains(master.show_character_02))
            {
                charaMasterIDList.Add(master.show_character_02);
            }

            if (!charaMasterIDList.Contains(master.show_character_03))
            {
                charaMasterIDList.Add(master.show_character_03);
            }

            if (!charaMasterIDList.Contains(master.show_character_04))
            {
                charaMasterIDList.Add(master.show_character_04);
            }

            //----------------------------------------
            // 表示する背景IDの取得
            //----------------------------------------
            if (!backImgIDList.Contains(master.back_ground_res))
            {
                backImgIDList.Add(master.back_ground_res);
            }

            //----------------------------------------
            // 再生するBGMIDの取得
            //----------------------------------------
            if (master.bgm_active == MasterDataDefineLabel.BoolType.ENABLE)
            {
                bgmIDList.Add((uint)master.bgm_res);
            }
        }

        charaMasterIDList.Sort();
        backImgIDList.Sort();
        bgmIDList.Sort();

        AssetBundlerMultiplier abMultiplier = AssetBundlerMultiplier.Create();

        //----------------------------------------
        // AssetBundleのキャラテクスチャ読み込みリクエスト発行
        //----------------------------------------
        for (int i = 0; i < charaMasterIDList.Count; i++)
        {
            AssetLoadStoryViewResource assetResource = new AssetLoadStoryViewResource();
            assetResource.SetStoryCharaId(charaMasterIDList[i]);
            m_AssetResourceList.Add(assetResource);
        }

        //----------------------------------------
        // AssetBundleの背景テクスチャ読み込みリクエスト発行
        //----------------------------------------
        for (int i = 0; i < backImgIDList.Count; i++)
        {
            AssetLoadStoryViewResource assetResource = new AssetLoadStoryViewResource();
            assetResource.SetMapID(backImgIDList[i]);
            m_AssetResourceList.Add(assetResource);
        }

        //----------------------------------------
        // AssetBundleのBGM読み込みリクエスト発行
        //----------------------------------------
        for (int i = 0; i < bgmIDList.Count; i++)
        {
            AssetLoadStoryViewResource assetResource = new AssetLoadStoryViewResource();
            assetResource.SetBgmID(bgmIDList[i]);
            m_AssetResourceList.Add(assetResource);
        }

        m_AssetResourceList.ForEach((v) =>
        {
            if (v.m_AssetBundler != null)
            {
                abMultiplier.Add(v.m_AssetBundler);
            }
        });

        abMultiplier.RegisterProgressFilesAction(
            (float count, float max) =>
            {
                LoadingManager.Instance.ProgressFiles(count, max);
            });

        abMultiplier.Load(() =>
        {
            // デバッグ用のUV情報を設定
            if (m_DebugStoryCharaMaster != null)
            {
                for (int i = 0; i < m_AssetResourceList.Count; i++)
                {
                    if (m_AssetResourceList[i].Type == AssetLoadStoryViewResource.ResourceType.CHARA)
                    {
                        if (m_AssetResourceList[i].CharaMasterData == null) { continue; }
                        m_AssetResourceList[i].CharaMasterData.img_offset_x = m_DebugStoryCharaMaster.img_offset_x;
                        m_AssetResourceList[i].CharaMasterData.img_offset_y = m_DebugStoryCharaMaster.img_offset_y;
                        m_AssetResourceList[i].CharaMasterData.img_tiling = m_DebugStoryCharaMaster.img_tiling;
                    }
                }
            }

            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.DATA_DOWNLOAD);
            StoryViewFSM.Instance.SendFsmNextEvent();
        },
        () =>
        {
#if BUILD_TYPE_DEBUG
            Debug.LogError("キャラテクスチャ読み込み失敗");
#endif
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.DATA_DOWNLOAD);
            StoryViewFSM.Instance.SendFsmNextEvent();
        });

        // コマンド解析
        m_ADVcommandLabelTable.Clear();
        m_ADVselectList.Clear();
        m_ADVdialogList.Clear();
        for (int i = 0; i < m_StoryMasterDataArray.Length; i++)
        {
            // ラベル
            var label = m_StoryMasterDataArray[i].label;
            if (label.IsNullOrEmpty() == false)
            {
                if (m_ADVcommandLabelTable.ContainsKey(label))
                {
#if BUILD_TYPE_DEBUG
                    Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV label multiple definition ERROR !!! (" + label + ")");
                    Dialog.Create(DialogType.DialogOK).
                        SetDialogText(DialogTextType.Title, "labelエラー：ラベルが重複しています").
                        SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV label:(" + label + ")").
                        SetDialogEvent(DialogButtonEventType.OK, () => { }).
                    Show();
#endif
                }
                else
                {
                    m_ADVcommandLabelTable.Add(label, i);
                }
            }
        }

        for (int i = 0; i < m_StoryMasterDataArray.Length; i++)
        {
            // コマンド
            var command = m_StoryMasterDataArray[i].command;
            if (command.IsNullOrEmpty() == false)
            {
#if BUILD_TYPE_DEBUG
                if (m_StoryMasterDataArray[i].command.Contains("”") == true)
                {
                    Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV command quotation double byte char ERROR !!!");
                    Dialog.Create(DialogType.DialogOK).
                        SetDialogText(DialogTextType.Title, "commandエラー：全角の”があります").
                        SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id + "\n" + command).
                        SetDialogEvent(DialogButtonEventType.OK, () => { }).
                    Show();
                    break;
                }
#endif
                try
                {
                    JsonDAO jdao = JsonDAO.Create(m_StoryMasterDataArray[i].command);
#if BUILD_TYPE_DEBUG
                    if (jdao.Dict == null)
                    {
                        Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV command parse ERROR !!!");
                        Dialog.Create(DialogType.DialogOK).
                            SetDialogText(DialogTextType.Title, "commandエラー：構文が間違ってます").
                            SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id + "\n" + command).
                            SetDialogEvent(DialogButtonEventType.OK, () => { }).
                        Show();
                    }
#endif

                    if (jdao.HasKey("select") == true)
                    {
                        ADVselect data = new ADVselect();
                        // 集約先ラベル指定
                        data.restartLabel = jdao.GetStr("restart");
#if BUILD_TYPE_DEBUG
                        if (m_ADVcommandLabelTable.ContainsKey(data.restartLabel) == false)
                        {
                            Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV select restart LABEL(" + data.restartLabel + ") not found ERROR !!!");
                            Dialog.Create(DialogType.DialogOK).
                                SetDialogText(DialogTextType.Title, "commandエラー：集約先のラベルがありません").
                                SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id + " LABEL:(" + data.restartLabel + ")").
                                SetDialogEvent(DialogButtonEventType.OK, () => { }).
                            Show();
                        }
#endif
                        // 選択肢情報
                        var sel = jdao.GetList("select");
                        for (int no = 0; no < sel.Count; no++)
                        {
                            ADVselect.ADVbranch dt = new ADVselect.ADVbranch();
                            var dao = JsonDAO.Create(sel[no]);
#if BUILD_TYPE_DEBUG
                            if (dao.HasKey("text") == false)
                            {
                                Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV select TEXT not found ERROR !!!");
                                Dialog.Create(DialogType.DialogOK).
                                    SetDialogText(DialogTextType.Title, "commandエラー：選択肢の文言がありません").
                                    SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id).
                                    SetDialogEvent(DialogButtonEventType.OK, () => { }).
                                Show();
                            }
#endif
#if BUILD_TYPE_DEBUG
                            if (dao.HasKey("jump") == false)
                            {
                                Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV select JUMP not found ERROR !!!");
                                Dialog.Create(DialogType.DialogOK).
                                    SetDialogText(DialogTextType.Title, "commandエラー：選択肢のラベル指定がありません").
                                    SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id).
                                    SetDialogEvent(DialogButtonEventType.OK, () => { }).
                                Show();
                            }
#endif
                            dt.text = dao.GetStr("text");
                            dt.label = dao.GetStr("jump");
                            dt.wakuType = dao.GetInt("type");
#if BUILD_TYPE_DEBUG
                            if (m_ADVcommandLabelTable.ContainsKey(dt.label) == false)
                            {
                                Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV select jump LABEL(" + dt.label + ") not found ERROR !!!");
                                Dialog.Create(DialogType.DialogOK).
                                    SetDialogText(DialogTextType.Title, "commandエラー：分岐先のラベルがありません").
                                    SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id + " LABEL:(" + dt.label + ")").
                                    SetDialogEvent(DialogButtonEventType.OK, () => { }).
                                Show();
                            }
                            else if (m_ADVcommandLabelTable[dt.label] <= i || m_ADVcommandLabelTable[dt.label] >= m_ADVcommandLabelTable[data.restartLabel])
                            {
                                Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV select jump LABEL(" + dt.label + ") out of REGION ERROR !!!");
                                Dialog.Create(DialogType.DialogOK).
                                    SetDialogText(DialogTextType.Title, "commandエラー：分岐先が制限の範囲を超えています").
                                    SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id + " LABEL:(" + dt.label + ")").
                                    SetDialogEvent(DialogButtonEventType.OK, () => { }).
                                Show();
                            }
#endif
#if BUILD_TYPE_DEBUG
                            if (dt.wakuType < 0 || dt.wakuType > 1)
                            {
                                Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV select WAKU TYPE(" + dt.wakuType + ") out of range ERROR !!!");
                                Dialog.Create(DialogType.DialogOK).
                                    SetDialogText(DialogTextType.Title, "commandエラー：選択肢枠タイプの指定が範囲外です").
                                    SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id + " type:(" + dt.wakuType + ")").
                                    SetDialogEvent(DialogButtonEventType.OK, () => { }).
                                Show();
                            }
#endif
                            dt.no = no;
                            data.node.Add(dt);
                        }
#if BUILD_TYPE_DEBUG
                        if (data.node.Count <= 0)
                        {
                            Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV select branch ZERO !!!");
                            Dialog.Create(DialogType.DialogOK).
                                SetDialogText(DialogTextType.Title, "commandエラー：選択肢の数がゼロです").
                                SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id).
                                SetDialogEvent(DialogButtonEventType.OK, () => { }).
                            Show();
                        }
#endif
#if BUILD_TYPE_DEBUG
                        if (data.node.Count > ADV_SELECT_BRANCH_MAX)
                        {
                            Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV select branch num OVER !!! (" + (data.node.Count + 1).ToString() + ")");
                            Dialog.Create(DialogType.DialogOK).
                                SetDialogText(DialogTextType.Title, "commandエラー：選択肢の数が" + ADV_SELECT_BRANCH_MAX + "以上あります").
                                SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id).
                                SetDialogEvent(DialogButtonEventType.OK, () => { }).
                            Show();
                        }
#endif
                        m_ADVselectList.Add(i, data);
                    }
                    else if (jdao.HasKey("dialog") == true) // ダイアログより選択肢優先
                    {
                        var id = jdao.GetInt("dialog");
                        m_ADVdialogList.Add(i, id);
                    }
                }
                catch (Exception e)
                {
#if BUILD_TYPE_DEBUG
                    Debug.LogError("fix_id:" + m_StoryMasterDataArray[i].fix_id + " ADV command parse ERROR !!!");
                    Dialog.Create(DialogType.DialogOK).
                        SetDialogText(DialogTextType.Title, "commandエラー：構文が間違ってます").
                        SetDialogText(DialogTextType.MainText, "fix_id:" + m_StoryMasterDataArray[i].fix_id + "\n" + command).
                        SetDialogEvent(DialogButtonEventType.OK, () => { }).
                    Show();
#endif
                }
            }
        }
    }

    /// <summary>
    /// シナリオの次の配列があるかチェック
    /// </summary>
    void OnHasNextLine()
    {
        if (m_ADVdialogList.ContainsKey(m_CurrentLine + 1) == true)
        {
            Context.IsEnableSelectButton = false;
            var id = (uint)m_ADVdialogList[m_CurrentLine + 1];

            GeneralWindowDialog dialog = GeneralWindowDialog.Create();
            dialog.SetGroupID(id);
            dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.CLOSE, () =>
            {
                ++m_CurrentLine;
                OnHasNextLine();
            });
            dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.YES, () =>
            {
                ++m_CurrentLine;
                OnHasNextLine();
            });
            dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.NO, () =>
            {
                ++m_CurrentLine;
                OnHasNextLine();
            });
            dialog.Show();
        }
        else if (m_ADVselectList.ContainsKey(m_CurrentLine) == true)
        {
            Context.IsEnableSelectButton = false;
            Context.IsEnableSelectMode = true;

            for (var i = 0; i < m_ADVselectList[m_CurrentLine].node.Count; i++)
            {
                m_ADVselectButtonList[i].SetParam(m_ADVselectList[m_CurrentLine].node[i]);
            }
        }
        else
        {
            OnHasNextLineMain();
        }
    }

    void OnHasNextLineMain()
    {
        Context.IsEnableSelectMode = false;
        ++m_CurrentLine;
        if (restartNo >= 0)
        {
            var label = m_StoryMasterDataArray[m_CurrentLine].label;
            if (label.IsNullOrEmpty() == false && (label != m_lastSelected.label || restartNo <= m_CurrentLine))
            {
                m_CurrentLine = restartNo;
                restartNo = -1;
            }
        }
        if (m_StoryMasterDataArray.Length > m_CurrentLine
        && m_StoryMasterDataArray[m_CurrentLine] != null)
        {
            if (Context.IsEnableSelectButton == false) { Context.IsEnableSelectButton = true; }

            StoryViewFSM.Instance.SendFsmPositiveEvent();
        }
        else
        {
            Context.IsEnableSelectButton = false;
            StoryViewFSM.Instance.SendFsmNegativeEvent();
#if BUILD_TYPE_DEBUG
            if (m_StoryMasterDataArray.Length == 0)
            {
                Debug.LogWarning("MasterDataが無い");
            }
#endif
        }
    }

    /// <summary>
    /// シナリオの次の配列を読み込む
    /// </summary>
    void OnLoadNextLine()
    {
        // テキストカーソルを非表示
        DOTween.Kill(TWEEN_OBJECT_ID.TEXT_BOX_CURSOR); // テキストカーソルの点滅をやめる
        Context.PauseCursorColor = Context.PauseCursorColor.WithAlpha(0);
        m_Text.text = "";
        m_SelectText.text = "";

        m_PrevStoryMasterData = m_CurrentStoryMasterData;
        m_CurrentStoryMasterData = m_StoryMasterDataArray[m_CurrentLine];

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// 背景の設定
    /// </summary>
    void OnSetBackGround()
    {
        if (m_CurrentStoryMasterData.back_ground_res > 0)
        {
            AssetLoadStoryViewResource assetResource = m_AssetResourceList.Find(v => v.m_fix_id == m_CurrentStoryMasterData.back_ground_res
                                 && v.Type == AssetLoadStoryViewResource.ResourceType.MAP);
            Context.BackGroundImage = (assetResource != null) ? assetResource.GetSprite() : null;
        }

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// フェードイン・フェードアウトの設定
    /// </summary>
    /// <returns></returns>
    IEnumerator OnLoopFadeView()
    {
        if (m_CurrentLine == 0 &&
            m_CurrentStoryMasterData.effect_fade_view != MasterDataDefineLabel.BoolType.DISABLE)
        {
            // 初回はフェードインさせる
            DOTween.To(() => Context.ScenerioContentAlpha, (x) => Context.ScenerioContentAlpha = x, 1, 0.25f)
            .SetId(TWEEN_OBJECT_ID.SCENARIO_CONTENT);

            DOTween.To(() => Context.BackGroundAlpha, (x) => Context.BackGroundAlpha = x, 1, 0.25f)
            .SetId(TWEEN_OBJECT_ID.BACK_GROUND);

            if (SafeAreaControl.HasInstance &&
                SafeAreaControl.Instance.bottom_space_height > 0)
            {
                DOTween.To(() => Context.TopMaskAlpha, (x) => Context.TopMaskAlpha = x, 1, 0.25f)
                .SetId(TWEEN_OBJECT_ID.TOP_MASK);
                DOTween.To(() => Context.BottomMaskAlpha, (x) => Context.BottomMaskAlpha = x, 1, 0.25f)
                .SetId(TWEEN_OBJECT_ID.BOTTOM_MASK);

                while (DOTween.IsTweening(TWEEN_OBJECT_ID.TOP_MASK))
                {
                    yield return null;
                }

                while (DOTween.IsTweening(TWEEN_OBJECT_ID.BOTTOM_MASK))
                {
                    yield return null;
                }
            }
        }
        else
        {
            if (m_CurrentStoryMasterData.effect_fade_view == MasterDataDefineLabel.BoolType.ENABLE)
            {
                Context.ScenerioContentAlpha = 0;
                DOTween.To(() => Context.ScenerioContentAlpha, (x) => Context.ScenerioContentAlpha = x, 1, 0.25f)
                .SetId(TWEEN_OBJECT_ID.SCENARIO_CONTENT);

                Context.BackGroundAlpha = 0;
                DOTween.To(() => Context.BackGroundAlpha, (x) => Context.BackGroundAlpha = x, 1, 0.25f)
                .SetId(TWEEN_OBJECT_ID.BACK_GROUND);
            }
            else if (m_CurrentStoryMasterData.effect_fade_view == MasterDataDefineLabel.BoolType.DISABLE)
            {
                Context.ScenerioContentAlpha = 1;
                DOTween.To(() => Context.ScenerioContentAlpha, (x) => Context.ScenerioContentAlpha = x, 0, 0.25f)
                .SetId(TWEEN_OBJECT_ID.SCENARIO_CONTENT);

                Context.BackGroundAlpha = 1;
                DOTween.To(() => Context.BackGroundAlpha, (x) => Context.BackGroundAlpha = x, 0, 0.25f)
                .SetId(TWEEN_OBJECT_ID.BACK_GROUND);
            }

        }

        while (DOTween.IsTweening(TWEEN_OBJECT_ID.SCENARIO_CONTENT))
        {
            yield return null;
        }

        while (DOTween.IsTweening(TWEEN_OBJECT_ID.BACK_GROUND))
        {
            yield return null;
        }

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// キャラクターカットインの表示処理
    /// </summary>
    void OnShowCaracter()
    {
        //-------------------------------------------
        // TODO: 枠画像の表示
        //-------------------------------------------

        //-------------------------------------------
        // 表示・非表示
        //-------------------------------------------
        // 1
        if (m_CurrentStoryMasterData.show_character_01_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            CharaPanels[0].Context.IsActiveCharaImage = true;
        }
        else if (m_CurrentStoryMasterData.show_character_01_active == MasterDataDefineLabel.BoolType.DISABLE)
        {
            CharaPanels[0].Context.IsActiveCharaImage = false;
        }

        // 2
        if (m_CurrentStoryMasterData.show_character_02_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            CharaPanels[1].Context.IsActiveCharaImage = true;
        }
        else if (m_CurrentStoryMasterData.show_character_02_active == MasterDataDefineLabel.BoolType.DISABLE)
        {
            CharaPanels[1].Context.IsActiveCharaImage = false;
        }

        // 3
        if (m_CurrentStoryMasterData.show_character_03_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            CharaPanels[2].Context.IsActiveCharaImage = true;
        }
        else if (m_CurrentStoryMasterData.show_character_03_active == MasterDataDefineLabel.BoolType.DISABLE)
        {
            CharaPanels[2].Context.IsActiveCharaImage = false;
        }

        // 4
        if (m_CurrentStoryMasterData.show_character_04_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            CharaPanels[3].Context.IsActiveCharaImage = true;
        }
        else if (m_CurrentStoryMasterData.show_character_04_active == MasterDataDefineLabel.BoolType.DISABLE)
        {
            CharaPanels[3].Context.IsActiveCharaImage = false;
        }

        //-------------------------------------------
        // キャラ画像の設定
        //-------------------------------------------
        // 1
        CharaPanels[0].SetUpCharImage(m_CurrentStoryMasterData.show_character_01, m_AssetResourceList);

        // 2
        CharaPanels[1].SetUpCharImage(m_CurrentStoryMasterData.show_character_02, m_AssetResourceList);

        // 3
        CharaPanels[2].SetUpCharImage(m_CurrentStoryMasterData.show_character_03, m_AssetResourceList);

        // 4
        CharaPanels[3].SetUpCharImage(m_CurrentStoryMasterData.show_character_04, m_AssetResourceList);

        //-------------------------------------------
        // スライドイン・アウトの設定
        //-------------------------------------------
        // 1
        CharaPanels[0].DoSlidePanel(m_CurrentStoryMasterData.show_character_01_slide, CHARACTER_CUT_IN_DURATION, TWEEN_OBJECT_ID.CHARACTER_SLIDE_01);

        // 2
        CharaPanels[1].DoSlidePanel(m_CurrentStoryMasterData.show_character_02_slide, CHARACTER_CUT_IN_DURATION, TWEEN_OBJECT_ID.CHARACTER_SLIDE_02);

        // 3
        CharaPanels[2].DoSlidePanel(m_CurrentStoryMasterData.show_character_03_slide, CHARACTER_CUT_IN_DURATION, TWEEN_OBJECT_ID.CHARACTER_SLIDE_03);

        // 4
        CharaPanels[3].DoSlidePanel(m_CurrentStoryMasterData.show_character_04_slide, CHARACTER_CUT_IN_DURATION, TWEEN_OBJECT_ID.CHARACTER_SLIDE_04);

        //-------------------------------------------
        // 名前の設定
        //-------------------------------------------
        // 1
        if (!m_CurrentStoryMasterData.name_01.IsNullOrEmpty())
        {
            CharaPanels[0].Context.CharaNameText = m_CurrentStoryMasterData.name_01;
        }

        // 2
        if (!m_CurrentStoryMasterData.name_02.IsNullOrEmpty())
        {
            CharaPanels[1].Context.CharaNameText = m_CurrentStoryMasterData.name_02;
        }

        // 3
        if (!m_CurrentStoryMasterData.name_03.IsNullOrEmpty())
        {
            CharaPanels[2].Context.CharaNameText = m_CurrentStoryMasterData.name_03;
        }

        // 4
        if (!m_CurrentStoryMasterData.name_04.IsNullOrEmpty())
        {
            CharaPanels[3].Context.CharaNameText = m_CurrentStoryMasterData.name_04;
        }

        // 1
        if (m_CurrentStoryMasterData.name_01_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            CharaPanels[0].Context.IsActiveCharaName = true;
        }
        else if (m_CurrentStoryMasterData.name_01_active == MasterDataDefineLabel.BoolType.DISABLE)
        {
            CharaPanels[0].Context.IsActiveCharaName = false;
        }

        // 2
        if (m_CurrentStoryMasterData.name_02_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            CharaPanels[1].Context.IsActiveCharaName = true;
        }
        else if (m_CurrentStoryMasterData.name_02_active == MasterDataDefineLabel.BoolType.DISABLE)
        {
            CharaPanels[1].Context.IsActiveCharaName = false;
        }

        // 3
        if (m_CurrentStoryMasterData.name_03_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            CharaPanels[2].Context.IsActiveCharaName = true;
        }
        else if (m_CurrentStoryMasterData.name_03_active == MasterDataDefineLabel.BoolType.DISABLE)
        {
            CharaPanels[2].Context.IsActiveCharaName = false;
        }

        // 4
        if (m_CurrentStoryMasterData.name_04_active == MasterDataDefineLabel.BoolType.ENABLE)
        {
            CharaPanels[3].Context.IsActiveCharaName = true;
        }
        else if (m_CurrentStoryMasterData.name_04_active == MasterDataDefineLabel.BoolType.DISABLE)
        {
            CharaPanels[3].Context.IsActiveCharaName = false;
        }

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// キャラクターカットインの表示待ち
    /// </summary>
    void OnUpdateShowCharacter()
    {
        if (!DOTween.IsTweening(TWEEN_OBJECT_ID.CHARACTER_SLIDE_01)
        && !DOTween.IsTweening(TWEEN_OBJECT_ID.CHARACTER_SLIDE_02)
        && !DOTween.IsTweening(TWEEN_OBJECT_ID.CHARACTER_SLIDE_03)
        && !DOTween.IsTweening(TWEEN_OBJECT_ID.CHARACTER_SLIDE_04)
        )
        {
            StoryViewFSM.Instance.SendFsmNextEvent();
        }
    }

    void OnShowCharacterFinished()
    {
        // 途中のアニメーションを終了させる
        DOTween.Complete(TWEEN_OBJECT_ID.CHARACTER_SLIDE_01);
        DOTween.Complete(TWEEN_OBJECT_ID.CHARACTER_SLIDE_02);
        DOTween.Complete(TWEEN_OBJECT_ID.CHARACTER_SLIDE_03);
        DOTween.Complete(TWEEN_OBJECT_ID.CHARACTER_SLIDE_04);

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// BGMの再生
    /// </summary>
    void OnPlayBGM()
    {
        if (m_CurrentStoryMasterData.bgm_active == MasterDataDefineLabel.BoolType.ENABLE
        && m_CurrentStoryMasterData.bgm_res != BGMManager.EBGM_ID.eBGM_INIT
        && m_CurrentStoryMasterData.bgm_res != m_PrevStoryMasterData.bgm_res)
        {
            SoundUtil.StopBGM(false);
            AssetLoadStoryViewResource assetResource = m_AssetResourceList.Find(v => (v.m_fix_id == (uint)m_CurrentStoryMasterData.bgm_res
                                                                                     && v.Type == AssetLoadStoryViewResource.ResourceType.BGM));
            if (assetResource.BGMData != null)
            {
                BGMManager.PlayStoryBGM(assetResource.BGMData);
            }
        }

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// SEの再生
    /// </summary>
    void OnPlaySE()
    {
        SoundUtil.PlaySE(m_CurrentStoryMasterData.se_res);
        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// キャラクターのフォーカス設定
    /// </summary>
    void OnFocusCharacter()
    {
        //-------------------------------------------
        // フォーカスの設定
        //-------------------------------------------
        switch (m_CurrentStoryMasterData.focus)
        {
            case MasterDataDefineLabel.StoryCharFocus.CHARACTER_01:
                for (int i = 0; i < CharaPanels.Length; ++i)
                {
                    if (CharaPanels[i] == null) { continue; }
                    CharaPanels[i].Context.IsActiveFocus = (i == 0);
                }
                break;
            case MasterDataDefineLabel.StoryCharFocus.CHARACTER_02:
                for (int i = 0; i < CharaPanels.Length; ++i)
                {
                    if (CharaPanels[i] == null) { continue; }
                    CharaPanels[i].Context.IsActiveFocus = (i == 1);
                }
                break;
            case MasterDataDefineLabel.StoryCharFocus.CHARACTER_03:
                for (int i = 0; i < CharaPanels.Length; ++i)
                {
                    if (CharaPanels[i] == null) { continue; }
                    CharaPanels[i].Context.IsActiveFocus = (i == 2);
                }
                break;
            case MasterDataDefineLabel.StoryCharFocus.CHARACTER_04:
                for (int i = 0; i < CharaPanels.Length; ++i)
                {
                    if (CharaPanels[i] == null) { continue; }
                    CharaPanels[i].Context.IsActiveFocus = (i == 3);
                }
                break;
            case MasterDataDefineLabel.StoryCharFocus.CHARACTER_ALL:
                for (int i = 0; i < CharaPanels.Length; ++i)
                {
                    if (CharaPanels[i] == null) { continue; }
                    CharaPanels[i].Context.IsActiveFocus = true;
                }
                break;
            case MasterDataDefineLabel.StoryCharFocus.CHARACTER_NOTHING:
                for (int i = 0; i < CharaPanels.Length; ++i)
                {
                    if (CharaPanels[i] == null) { continue; }
                    CharaPanels[i].Context.IsActiveFocus = false;
                }
                break;
            default:
                break;
        }

        if (m_CurrentStoryMasterData.focus != MasterDataDefineLabel.StoryCharFocus.NONE)
        {
            Context.TextBoxFocus = m_CurrentStoryMasterData.focus;
        }

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// 文字送りの開始
    /// </summary>
    void OnChangingCaption()
    {
        //----------------------------------------
        // 吹き出しの変更
        //----------------------------------------
        if (m_CurrentStoryMasterData.balloon_type > 0)
        {
            Context.TextBoxNum = (int)m_CurrentStoryMasterData.balloon_type;
        }
        else if (Context.TextBoxNum == 0)
        {
            Context.TextBoxNum = 1;
        }

        //----------------------------------------
        // テキスト
        //----------------------------------------
        string currentText = string.Format(m_CurrentStoryMasterData.text, m_HeroName);
        float duration = 0;
        int textLength = 0;
        if (currentText != null)
        {
            duration = m_IntervalForCharacterDisplay * currentText.Length; // 表示にかかる時間
            textLength = currentText.Length;
        }

        string viewText = currentText.ReplaceSpaceTag(29).NoLineBreakTag();

        m_SelectText.text = viewText;
        // 文字送り
        m_Text.text = viewText;
        m_Text.maxVisibleCharacters = 0;
        m_Text.DOMaxVisibleCharacters(textLength, duration)
            .SetId(TWEEN_OBJECT_ID.TEXT_BOX)
            .SetEase(Ease.Linear)
            .OnStepComplete(() =>
            {
                StoryViewFSM.Instance.SendFsmNextEvent();
            });
    }

    /// <summary>
    /// 文字送りが終わって待機状態
    /// </summary>
    void OnWaitCaption()
    {
        DOTween.Complete(TWEEN_OBJECT_ID.TEXT_BOX); // 文字送りをやめてすべて表示する
        if (m_CurrentStoryMasterData.text.IsNullOrEmpty() && m_Text.text.IsNullOrEmpty())
        {
            StoryViewFSM.Instance.SendFsmNextEvent();
        }

        // ポーズカーソルのアニメーション
        Context.PauseCursorColor = Context.PauseCursorColor.WithAlpha(1);
        DOTween.ToAlpha(() => Context.PauseCursorColor, (x) => Context.PauseCursorColor = x, 0, 0.5f)
        .SetId(TWEEN_OBJECT_ID.TEXT_BOX_CURSOR)
        .SetEase(Ease.InQuint)
        .SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// スキップされた
    /// </summary>
    void OnSkip()
    {
        m_FinishType = STORY_FINISH_TYPE.SKIP;
        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// シナリオをすべて読み込み終わった
    /// </summary>
    IEnumerator OnLoopCompletingScenario()
    {
        if (loadingMask)
        {
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.TO_BATTLE);
        }

        // アニメーションの削除
        foreach (TWEEN_OBJECT_ID objID in Enum.GetValues(typeof(TWEEN_OBJECT_ID)))
        {
            DOTween.Kill(objID);
        }

        // シナリオのフェードアウト
        DOTween.To(() => Context.ScenerioContentAlpha, (x) => Context.ScenerioContentAlpha = x, 0, 0.25f)
        .SetId(TWEEN_OBJECT_ID.SCENARIO_CONTENT);

        SoundUtil.StopBGM(false);

        while (DOTween.IsTweening(TWEEN_OBJECT_ID.SCENARIO_CONTENT))
        {
            yield return null;
        }

        if (BGMManager.HasInstance)
        {
            while (BGMManager.Instance.isFadeOut)
            {
                yield return null;
            }
        }

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    /// <summary>
    /// ストーリクエストクリア
    /// </summary>
    void OnSendStoryQuestClear()
    {
        if (m_QuestID == 0)
        {
            // クエストではない
            StoryViewFSM.Instance.SendFsmNextEvent();
            return;
        }

        if (m_FinishType != STORY_FINISH_TYPE.SUCCESS)
        {
            // 正常に終了していない場合

            StoryViewFSM.Instance.SendFsmNextEvent();
            return;
        }

        bool isQuestClear = ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_clear, m_QuestID);
        bool isMissonComplete = ServerDataUtil.ChkRenewBitFlag(ref UserDataAdmin.Instance.m_StructPlayer.flag_renew_quest_mission_complete, m_QuestID);
        if (isQuestClear && isMissonComplete)
        {
            // すでにフラグがついている
            StoryViewFSM.Instance.SendFsmNextEvent();
            return;
        }

        ServerDataUtilSend.SendPacketAPI_Quest2StoryClear(m_QuestID)
        .setSuccessAction(_data =>
        {
            RecvQuest2StoryClearValue result = _data.GetResult<RecvQuest2StoryClear>().result;
            UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvQuest2StoryClear>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
            ShowMissionClearDialog(result, () =>
            {
                StoryViewFSM.Instance.SendFsmNextEvent();
            });
        })
        .setErrorAction(_data =>
        {
            StoryViewFSM.Instance.SendFsmNextEvent();
        })
        .SendStart();
    }

    /// <summary>
    /// フェードアウト処理
    /// </summary>
    /// <returns></returns>
    IEnumerator OnLoopFadeOut()
    {
        // ストーリークエストの場合はリストを更新する
        if (m_QuestID > 0)
        {
            if (m_QuestReloadAction != null)
            {
                m_QuestReloadAction();
            }
        }


        DOTween.To(() => Context.MainContentAlpha, (x) => Context.MainContentAlpha = x, 0, 0.25f)
        .SetId(TWEEN_OBJECT_ID.MAIN_CONTENT);

        while (DOTween.IsTweening(TWEEN_OBJECT_ID.MAIN_CONTENT))
        {
            yield return null;
        }

        StoryViewFSM.Instance.SendFsmNextEvent();
    }

    void OnCompleteScenario()
    {
        Hide();
    }

    /// <summary>
    /// エラーになったとき
    /// </summary>
    void OnErrorScenario()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogText(DialogTextType.MainText, "シナリオを読み込めませんでした");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            StoryViewFSM.Instance.SendFsmNextEvent();
        }));
        newDialog.SetDialogEvent(DialogButtonEventType.CANCEL, new System.Action(() =>
        {
            StoryViewFSM.Instance.SendFsmNextEvent();
        }));
        newDialog.Show();
    }
    #endregion
}
