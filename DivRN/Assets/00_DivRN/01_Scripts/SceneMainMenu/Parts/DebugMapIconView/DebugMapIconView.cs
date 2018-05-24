using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using UnityEngine.UI;

public class DebugMapIconView : M4uContextMonoBehaviour
{

    M4uProperty<List<AreaDataContext>> areaDataList = new M4uProperty<List<AreaDataContext>>();
    public List<AreaDataContext> AreaDataList { get { return areaDataList.Value; } set { areaDataList.Value = value; } }

    M4uProperty<bool> isIconView = new M4uProperty<bool>();
    public bool IsIconView { get { return isIconView.Value; } set { isIconView.Value = value; } }

    public InputField idInputField;
    public UGUIHoldPress beforeHoldPress;
    public UGUIHoldPress afterHoldPress;

    private List<MasterDataAreaCategory> masters;
    private int currentIndex;
    private uint currentId;
    private float longTapWaitTime;
    private const float firstLongTapTime = 0.5f;
    private bool firstHold;

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        longTapWaitTime = firstLongTapTime;
        firstHold = true;
        AreaDataList = new List<AreaDataContext>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setup()
    {
        currentIndex = 0;
        masters = MasterFinder<MasterDataAreaCategory>.Instance.FindAll();
        IsIconView = true;
        setupIcon();
    }

    public void setupIcon()
    {
        AreaDataList.Clear();
        var model = new AreaSelectListItemModel(0);
        model.OnAppeared += () =>
        {
            model.ShowTitle();
        };

        if (IsIconView == false)
        {
            model.isActive = true;
        }
        else
        {
            model.isActive = false;
        }

        AreaDataContext newArea = new AreaDataContext(model);
        if (newArea != null)
        {
            newArea.m_AreaIndex = masters[currentIndex].fix_id;
            newArea.IsViewFlag = false;
            newArea.IsAreaNew = false;

            newArea.Title = masters[currentIndex].area_cate_name;
            newArea.PosX = 0;
            newArea.PosY = 0;

            // アセットバンドルの読み込み
            string assetBundleName = string.Format("areamapicon_{0}", masters[currentIndex].fix_id);
            // インジケーターを表示
            LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.GUARD);
            AssetBundler.Create().
                Set(assetBundleName,
                (o) =>
                {
                    newArea.IconImage = o.GetAsset<Sprite>();
                    newArea.IconImage_mask = o.GetTexture(newArea.IconImage.name + "_mask", TextureWrapMode.Clamp);
                    LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.GUARD);
                    AreaDataList.Add(newArea);
                },
                (s) =>
                {
                    newArea.IconImage = ResourceManager.Instance.Load("maeishoku_icon");
                    LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.GUARD);
                }).Load();
        }
        else
        {
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.GUARD);
        }

        idInputField.text = masters[currentIndex].fix_id.ToString();
    }

    public void OnBeforeButton()
    {
        changeBg(-1);
    }

    public void OnBeforeHoldButton()
    {
        changeBg(-1);
    }

    public void OnAfterButton()
    {
        changeBg(1);
    }

    public void OnAfterHoldButton()
    {
        changeBg(1);
    }

    public void OnEndEdit(string value)
    {
        for (int i = 0; i < masters.Count; ++i)
        {
            if (masters[i].fix_id == (uint)value.ToInt(0))
            {
                currentIndex = i;
                setupIcon();
                return;
            }
        }
        idInputField.text = currentId.ToString();
    }

    private void changeBg(int plusIndex)
    {
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.GUARD);

        currentIndex += plusIndex;
        if (currentIndex < 0)
        {
            currentIndex = masters.Count - 1;
        }

        if (currentIndex >= masters.Count)
        {
            currentIndex = 0;
        }

        setupIcon();
    }

    public void changeIconAnimetion()
    {
        if (IsIconView == true)
        {
            IsIconView = false;
            AreaDataList[0].model.isActive = true;
        }
        else
        {
            IsIconView = true;
            AreaDataList[0].model.isActive = false;
        }
    }

    public void OnIconTouch()
    {
        setupIcon();
    }
}
