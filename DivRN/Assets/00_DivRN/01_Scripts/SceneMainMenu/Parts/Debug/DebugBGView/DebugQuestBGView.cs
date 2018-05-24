using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using UnityEngine.UI;

public class DebugQuestBGView : M4uContextMonoBehaviour
{

    M4uProperty<string> title = new M4uProperty<string>();
    public string Title { get { return title.Value; } set { title.Value = value; } }

    M4uProperty<Sprite> questBg = new M4uProperty<Sprite>();
    public Sprite QuestBg { get { return questBg.Value; } set { questBg.Value = value; } }

    public InputField idInputField;
    public Sprite newBattleBg;
    public UGUIHoldPress beforeHoldPress;
    public UGUIHoldPress afterHoldPress;

    private List<MasterDataQuest2> masters;
    private int currentIndex;
    private uint currentId;
    private bool reading;
    private bool isHold;
    private float longTapWaitTime;
    private const float firstLongTapTime = 0.5f;
    private bool firstHold;


    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        reading = false;
        isHold = false;
        longTapWaitTime = firstLongTapTime;
        firstHold = true;
    }

    // Use this for initialization
    void Start()
    {
        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.fitTopAndBottom(GetComponent<RectTransform>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setup()
    {
        currentIndex = 0;
        reading = false;
        masters = MasterFinder<MasterDataQuest2>.Instance.FindAll();
        setupBg();
    }

    private void setupBg()
    {
        MasterDataArea areaMaster = MasterFinder<MasterDataArea>.Instance.Find((int)masters[currentIndex].area_id);
        if (areaMaster != null)
        {
            Title = areaMaster.area_name + "\n" + masters[currentIndex].quest_name;
            idInputField.text = masters[currentIndex].fix_id.ToString();
            string backgroundname = MasterDataUtil.GetMasterDataQuestBackgroundName(masters[currentIndex].background);
            currentId = masters[currentIndex].fix_id;
            reading = true;
            // インジケーターを表示
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.ASSETBUNDLE);
            }
            AssetBundler.Create().Set(backgroundname, backgroundname,
                (o) =>
                {
                    QuestBg = o.AssetBundle.LoadAsset<Sprite>(backgroundname);
                    reading = false;
                    // インジケーターを閉じる
                    if (LoadingManager.Instance != null)
                    {
                        LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
                    }
                },
                (s) =>
                {
                    QuestBg = newBattleBg;
                    reading = false;
                    // インジケーターを閉じる
                    if (LoadingManager.Instance != null)
                    {
                        LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
                    }
                }).Load();
        }
    }

    public void OnBeforeButton()
    {
        if (isHold == true)
        {
            isHold = false;
            firstHold = true;
            longTapWaitTime = firstLongTapTime;
            return;
        }
        if (reading == true)
        {
            return;
        }
        changeBg(-1);
    }

    public void OnBeforeHoldButton()
    {
        if (firstHold == true)
        {
            longTapWaitTime -= beforeHoldPress.m_IntervalHoldPress;
            if (longTapWaitTime > 0)
            {
                return;
            }
            firstHold = false;
        }
        if (reading == true)
        {
            return;
        }
        changeBg(-1);
        isHold = true;
    }

    public void OnAfterButton()
    {
        if (isHold == true)
        {
            isHold = false;
            firstHold = true;
            longTapWaitTime = firstLongTapTime;
            return;
        }
        if (reading == true)
        {
            return;
        }
        changeBg(1);
    }

    public void OnAfterHoldButton()
    {
        if (firstHold == true)
        {
            longTapWaitTime -= afterHoldPress.m_IntervalHoldPress;
            if (longTapWaitTime > 0)
            {
                return;
            }
            firstHold = false;
        }
        if (reading == true)
        {
            return;
        }
        changeBg(1);
        isHold = true;
    }

    public void OnEndEdit(string value)
    {
        if (reading == true)
        {
            return;
        }
        for (int i = 0; i < masters.Count; ++i)
        {
            if (masters[i].fix_id == (uint)value.ToInt(0))
            {
                currentIndex = i;
                setupBg();
                return;
            }
        }
        idInputField.text = currentId.ToString();
    }

    private void changeBg(int plusIndex)
    {
        currentIndex += plusIndex;
        if (currentIndex < 0)
        {
            currentIndex = masters.Count - 1;
        }
        if (currentIndex >= masters.Count)
        {
            currentIndex = 0;
        }
        setupBg();
    }

    public void OnClickBack()
    {
        bool bSE = false;
        try
        {
            MainMenuSeq pageNow = MainMenuManager.Instance.MainMenuSeqPageNow;
            if (MainMenuManager.Instance.IsPageSwitch() ||          //ページ切り替え中
                ServerApi.IsExists ||                               //通信中
                (pageNow != null && pageNow.IsSuspendReturn))     //戻るボタン抑制中
            {
                return;
            }

            if (MainMenuParam.m_PageBack.Count == 0)
            {
                return;
            }


            if (MainMenuManager.HasInstance)
            {
                MAINMENU_SEQ eNextPage = MainMenuParam.m_PageBack.Pop();
                if (MainMenuManager.Instance.AddSwitchRequest(eNextPage, false, true) == false)
                {
                    MainMenuParam.m_PageBack.Push(eNextPage);
                }
                else
                {
                    bSE = true;
                    AssetBundlerResponse.clearAssetBundleChash();
                }
            }
            return;
        }
        finally
        {
            if (bSE) SoundUtil.PlaySE(SEID.SE_MENU_RET);
        }
    }
}
