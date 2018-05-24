using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerDataDefine;
using M4u;

public class ScratchLineUpDialog : M4uContextMonoBehaviour, IRecyclableItemsScrollContentDataProvider
{
    private readonly int LINE_COUNT = 1;

    [SerializeField]
    RecyclableItemsScrollContent scrollContent = null;
    [SerializeField]
    GameObject itemPrefab = null;

    // スクラッチタイトル
    M4uProperty<string> titleText = new M4uProperty<string>("");
    public string TitleText { get { return titleText.Value; } set { titleText.Value = value; } }

    // 販売期間
    M4uProperty<string> dateText = new M4uProperty<string>("");
    public string DateText { get { return dateText.Value; } set { dateText.Value = value; } }

    // 確率Webボタン表示非表示
    M4uProperty<bool> isProbability = new M4uProperty<bool>(false);
    public bool IsProbability { get { return isProbability.Value; } set { isProbability.Value = value; } }

    // 確率表記テキスト
    M4uProperty<string> probabilityText = new M4uProperty<string>("");
    public string ProbabilityText { get { return probabilityText.Value; } set { probabilityText.Value = value; } }

    // 出現ユニット一覧
    M4uProperty<List<LineUpListItemContex>> records = new M4uProperty<List<LineUpListItemContex>>(new List<LineUpListItemContex>());
    public List<LineUpListItemContex> Records { get { return records.Value; } set { records.Value = value; } }

    // チップ使用フラグ
    M4uProperty<bool> isUsedTip = new M4uProperty<bool>(false);
    bool IsUsedTip { get { return isUsedTip.Value; } set { isUsedTip.Value = value; } }

    private string m_ProbabilityUrl = "";
    private string m_GuidLineText = "";

    private Canvas m_Canvas = null;

    private List<LineUpListItemContex> m_RecvGachaLineup = null;

    public Action m_CloseAction = null;

    public static ScratchLineUpDialog Create()
    {
        GameObject _tmpObj = Resources.Load("Prefab/Scratch/ScratchLineUpDialog") as GameObject;
        if (_tmpObj == null)
        {
            return null;
        }

        GameObject _newObj = Instantiate(_tmpObj) as GameObject;
        if (_newObj == null)
        {
            return null;
        }
        UnityUtil.SetObjectEnabledOnce(_newObj, true);

        return _newObj.GetComponent<ScratchLineUpDialog>();
    }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_Canvas = GetComponentInChildren<Canvas>();
    }

    // Use this for initialization
    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            float bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
            if (bottom_space_height > 0)
            {
                RectTransform rect = m_Canvas.transform.Find("Window").gameObject.GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y + bottom_space_height);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetCamera(Camera camera)
    {
        m_Canvas.worldCamera = camera;
    }

    public void setup(MasterDataGacha gachaMaster,
                      MasterDataStepUpGachaManage stepUpManageMaster,
                      uint assgin_id,
                      bool usedTip,
                      string title)
    {
        if (gachaMaster == null)
        {
            return;
        }

        MasterDataGacha _master = gachaMaster;

        if (_master.type != MasterDataDefineLabel.GachaType.STEP_UP &&
            _master.rainbow_decide == assgin_id)
        {
            // 確定ラインナップの場合
            _master = MasterFinder<MasterDataGacha>.Instance.Find(_master.rainbow_decide);

            if (_master == null)
            {
                return;
            }
        }

        TitleText = title;

        uint startTimeU = _master.timing_start;
        uint endTimeU = _master.timing_end;

        // 販売日程(終了日が無期限のときは表示しない)
        DateText = "";
        if (endTimeU != 0)
        {
            string dateFormat = GameTextUtil.GetText("scratch_display2");
            DateTime startTime = TimeUtil.GetDateTime(startTimeU);
            DateTime endTime = TimeUtil.GetDateTime(endTimeU).SubtractAMinute();
            DateText = String.Format(dateFormat,
                        startTime.ToString("yyyy/MM/dd(HH:mm)"),
                        endTime.ToString("yyyy/MM/dd(HH:mm)"));
        }

        var guide_master_type = EMASTERDATA_SERVER.GACHA_GROUP;
        var gachatextreftype = MasterDataDefineLabel.GachaTextRefType.NONE;

        uint gacha_fix_id = 0;
        uint _step_id = 0;
        uint _assgin_id = 0;

        if (_master.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            guide_master_type = EMASTERDATA_SERVER.STEP_UP_GACHA;

            var _stepUpGachaMaster = MasterDataUtil.GetMasterDataStepUpGachaFromGachaID(_master.fix_id);
            if (_stepUpGachaMaster != null)
            {
                gacha_fix_id = _stepUpGachaMaster.fix_id;
            }
            else
            {
#if BUILD_TYPE_DEBUG
                string text = String.Format("StepUpGachaMaster.gacha_idに\n「{0}」が未定義です", _master.fix_id);
                DialogManager.Open1B_Direct("ScratchLineUpDialog",
                                            text,
                                            "common_button7", true, true).
                SetOkEvent(() =>
                {
                });
#endif
            }

            _step_id = stepUpManageMaster.fix_id;

            _assgin_id = assgin_id;

            if (stepUpManageMaster.normal1_assign_id == assgin_id)
            {
                gachatextreftype = MasterDataDefineLabel.GachaTextRefType.NOMAL1_RATE_URL;
            }
            else if (stepUpManageMaster.normal2_assign_id == assgin_id)
            {
                gachatextreftype = MasterDataDefineLabel.GachaTextRefType.NOMAL2_RATE_URL;
            }
            else if (stepUpManageMaster.special_assign_id == assgin_id)
            {
                gachatextreftype = MasterDataDefineLabel.GachaTextRefType.SPECIAL_RATE_URL;
            }

            if (gachatextreftype != MasterDataDefineLabel.GachaTextRefType.NONE)
            {
                m_ProbabilityUrl = MasterDataUtil.GetGachaText(EMASTERDATA_SERVER.STEP_UP_GACHA_MANAGE,
                                                                _step_id,
                                                                gachatextreftype);
            }
        }
        else
        {
            guide_master_type = EMASTERDATA_SERVER.GACHA_GROUP;
            gacha_fix_id = _master.gacha_group_id;

            gachatextreftype = MainMenuParam.m_GachaRainbowDecide == assgin_id
                                ? MasterDataDefineLabel.GachaTextRefType.SPECIAL_RATE_URL
                                : MasterDataDefineLabel.GachaTextRefType.NOMAL1_RATE_URL;

            m_ProbabilityUrl = MasterDataUtil.GetGachaText(guide_master_type,
                                                        gacha_fix_id,
                                                        gachatextreftype);
        }

        //ガイドライン
        m_GuidLineText = MasterDataUtil.GetGachaText(guide_master_type,
                                                    gacha_fix_id,
                                                    MasterDataDefineLabel.GachaTextRefType.GUIDLINE);


        //確率URL
        if (m_ProbabilityUrl.Length > 0)
        {
            ProbabilityText = GameTextUtil.GetText("Gacha_step_05");
            IsProbability = true;
        }
        else
        {
            IsProbability = false;
        }

        // ガイドラインボタン表示フラグ
        IsUsedTip = usedTip;

        // 排出ユニット一覧
        ServerDataUtilSend.SendPacketAPI_GetGachaLineup((int)_master.fix_id, _step_id, _assgin_id)
        .setSuccessAction(_data =>
        {
            SortList<LineUpListItemContex> tmpLineup = new SortList<LineUpListItemContex>();
            PacketStructGachaLineup[] lineup = _data.GetResult<RecvGetGachaLineup>().result.gacha_assign_unit_list;
            for (int i = 0; i < lineup.Length; i++)
            {
                LineUpListItemContex newItem = new LineUpListItemContex(i, lineup[i]);
                //newItem.DidSelectItem += OnSelectItem;
                tmpLineup.Body.Add(newItem);
            }
            tmpLineup.AddSortInfo(SORT_PARAM.RATIO_UP, SORT_ORDER.DESCENDING);
            tmpLineup.AddSortInfo(SORT_PARAM.LIMITED, SORT_ORDER.DESCENDING);
            tmpLineup.AddSortInfo(SORT_PARAM.GROUP_ID, SORT_ORDER.ASCENDING);
            tmpLineup.AddSortInfo(SORT_PARAM.RARITY, SORT_ORDER.DESCENDING);
            tmpLineup.AddSortInfo(SORT_PARAM.ID, SORT_ORDER.ASCENDING);

            m_RecvGachaLineup = tmpLineup.Exec(SORT_OBJECT_TYPE.LINUP_LIST);

            scrollContent.Initialize(this);
        })
        .setErrorAction(_date =>
        {
            Close();
        })
        .SendStart();

        //BackKey設定
        AndroidBackKeyManager.Instance.StackPush(gameObject, OnCloseButton);
    }

    // ダイアログ ガイドライン
    public void OnClickGuideLine()
    {
        if (ServerApi.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
        newDialog.SetDialogText(DialogTextType.Title, GameTextUtil.GetText("scratch_guideline_title"));
        newDialog.SetDialogText(DialogTextType.MainText, m_GuidLineText);
        newDialog.SetDialogObjectEnabled(DialogObjectType.OneButton, true);
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    // ダイアログ ガイドライン
    public void OnClickProbability()
    {
        if (ServerApi.IsExists)
        {
            return;
        }

        URLManager.OpenURL(m_ProbabilityUrl);
    }

    public void OnSelectItem(LineUpListItemContex _item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        UnitDetailInfo unitDetail = MainMenuManager.Instance.OpenUnitDetailInfo(TutorialManager.IsExists ? false : true);
        if (unitDetail != null)
        {
            unitDetail.SetCharaID((uint)_item.UnitId);
            unitDetail.SetCloseAction(OnUnitDetailClose);
            m_Canvas.enabled = false;
        }
    }

    public void OnUnitDetailClose()
    {
        m_Canvas.enabled = true;
    }

    // クリック時のフィードバック
    public void OnCloseButton()
    {
        if (ServerApi.IsExists)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_RET);
        if (m_CloseAction != null)
        {
            m_CloseAction();
        }
        Close();
    }

    public void Close()
    {
        //BackKey解除
        AndroidBackKeyManager.Instance.StackPop(gameObject);

        DestroyObject(gameObject);
    }

    #region IRecyclableItemsScrollContentDataProvider methods.
    public int DataCount
    {
        get
        {
            int _ret = m_RecvGachaLineup.Count / LINE_COUNT;
            if (m_RecvGachaLineup.Count % LINE_COUNT != 0) _ret++;
            return _ret;
        }
    }


    public float GetItemScale(int index)
    {
        return itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    public RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        // indexの位置にあるセルを読み込む処理

        if (null == recyclableItem)
        {
            // 初回ロード時はinstantateItemCountで指定した回数分だけitemがnullで来るので、ここで生成してあげる
            // 以降はitemが再利用されるため、Reflesh()しない限りnullは来ない
            recyclableItem = Instantiate(itemPrefab).GetComponent<RectTransform>();
        }

        // セルの内容書き換え
        {
            ScratchLineUpLine _line = recyclableItem.GetComponent<ScratchLineUpLine>();
            _line.ItemList.Clear();
            int start_id = index * LINE_COUNT;
            for (int i = 0; i < LINE_COUNT; i++)
            {
                if (start_id + i >= m_RecvGachaLineup.Count)
                {
                    continue;
                }

                int id = start_id + i;
                LineUpListItemContex _newItem = new LineUpListItemContex(m_RecvGachaLineup[id]);
                _newItem.setupResource();
                _newItem.DidSelectItem += OnSelectItem;
                _line.ItemList.Add(_newItem);
            }
        }

        return recyclableItem;
    }

    #endregion
}
