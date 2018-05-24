using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuQuestStory : MainMenuSeq
{
    private const uint EXCLUDE_EFFECT_MIN_ID = 1320000;
    private const uint EXCLUDE_EFFECT_MAX_ID = 1329999;

    private AreaSelect m_AreaSelect = null;
    private MenuBanner m_MenuBanner = null;

    private List<AreaSelectListItemModel> m_areaIcons = new List<AreaSelectListItemModel>();
    private MasterDataRegion m_CurrentRegionMaster = null;
    private List<MasterDataRegion> m_RegionMasterList = null;
    private MasterDataDefineLabel.AreaCategory[] m_AreaCategory = null;
    private MasterDataAreaCategory m_ChallengeAreaCategory = null;

    private bool m_bReturnHome = false;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }

        if (m_bReturnHome)
        {
            if (MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HOME_MENU, false, false))
            {
                m_bReturnHome = false;
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (m_AreaSelect != null && m_AreaSelect.BackGroundImage != null)
        {
            m_AreaSelect.BackGroundImage = null;
        }
    }



    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //ページ初期化処理
        if (m_AreaSelect == null)
        {
            m_AreaSelect = m_CanvasObj.GetComponentInChildren<AreaSelect>();
            m_AreaSelect.SetPositionAjustStatusBar(new Vector2(0, -46.25f), new Vector2(0, -172.5f));
        }
        if (m_MenuBanner == null)
        {
            m_MenuBanner = m_CanvasObj.GetComponentInChildren<MenuBanner>();
            m_MenuBanner.SetPositionAjustStatusBar(new Vector2(-125, -217));
            m_MenuBanner.bannerSetup(true);
        }

        m_bReturnHome = false;

        m_ChallengeAreaCategory = null;
        List<MasterDataChallengeEvent> eventList = MasterDataUtil.GetActiveChallengeEvent();
        if (eventList != null &&
            eventList.Count > 0)
        {
            m_ChallengeAreaCategory = MasterDataUtil.GetChallengeAreaCategoryMaster(eventList[0].event_id);
        }

        SettingArea();

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.QUEST;

        StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        MainMenuParam.m_BannerLastIndexQuest = m_MenuBanner.banner.carouselRotator.CurrentIndex;
        return base.PageSwitchEventDisableAfter(eNextMainMenuSeq);
    }

    private void SettingArea()
    {
        if (m_AreaSelect == null)
        {
            return;
        }

        m_AreaSelect.IsActiveScroll = false;
        m_AreaSelect.DidSelectSwitch0 += OnSwitch0;
        m_AreaSelect.DidSelectSwitch1 += OnSwitch1;
        m_AreaSelect.DidSelectSwitch2 += OnSwitch2;
        m_AreaSelect.SwitchTitle0 = GameTextUtil.GetText("map_tab1");
        m_AreaSelect.SwitchTitle1 = GameTextUtil.GetText("map_tab2");
        m_AreaSelect.SwitchTitle2 = GameTextUtil.GetText("map_tab3");
        m_AreaSelect.DidSelectMap += OnSelectMapButton;

        SelectArea();
    }

    void SelectArea(/*MasterDataDefineLabel.AreaCategory[] areaCategoryArray*/)
    {
        m_CurrentRegionMaster = MasterFinder<MasterDataRegion>.Instance.Find((int)MainMenuParam.m_RegionID);
        if (m_CurrentRegionMaster != null)
        {
            m_RegionMasterList = MainMenuUtil.CreateRegionList(m_CurrentRegionMaster.category);
            if (m_RegionMasterList.IsNullOrEmpty() == false)
            {
                if (m_RegionMasterList.Contains(m_CurrentRegionMaster) == false)
                {
                    // 開催期間が切れた等で有効なデータが無い場合
                    ResidentParam.m_RegionIds[(int)m_CurrentRegionMaster.category] = 0;
                    openWarningRegionDialog();
                    return;
                }
            }
        }

        MasterDataAreaCategory[] areaCategoryMasterArray = null;
        List<AreaDataContext> areaDataList = new List<AreaDataContext>();

        new SerialProcess()
            .Add(next =>
            {
                // インジケーターを表示
                LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.ASSETBUNDLE);

                if (m_CurrentRegionMaster != null)
                {
                    m_AreaSelect.IsViewRegionButton = (m_RegionMasterList != null && m_RegionMasterList.Count > 1);
                    ResidentParam.m_RegionIds[(int)m_CurrentRegionMaster.category] = m_CurrentRegionMaster.fix_id;
                    areaCategoryMasterArray = MasterFinder<MasterDataAreaCategory>.Instance.SelectWhere("where region_id = ?", MainMenuParam.m_RegionID).ToArray();

                    // Region ID毎の背景画像をAssetBundleからセットする.
                    // AssetBundlePathMasterに該当のAssetBundleがあるか確認し, なければデフォルト画像を表示する.
                    SetAreaBackground(m_CurrentRegionMaster.category, m_CurrentRegionMaster.fix_id, next);

                    switch (m_CurrentRegionMaster.category)
                    {
                        case MasterDataDefineLabel.REGION_CATEGORY.STORY:
                            m_AreaSelect.currentCategory = MasterDataDefineLabel.AreaCategory.RN_STORY;
                            MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk99p_description"));
                            break;
                        case MasterDataDefineLabel.REGION_CATEGORY.MATERIAL:
                            m_AreaSelect.currentCategory = MasterDataDefineLabel.AreaCategory.RN_SCHOOL;
                            MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk101p_description"));
                            break;
                        case MasterDataDefineLabel.REGION_CATEGORY.EVENT:
                            m_AreaSelect.currentCategory = MasterDataDefineLabel.AreaCategory.RN_EVENT;
                            MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk100p_description"));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    //MasterDataRegionがなかった時のセーフティ(データが設定されていないサーバーに接続した場合)
#if BUILD_TYPE_DEBUG
                    string messageText = "MasterDataRegionに有効なデータがありませんでした。\n"
                                        + "プランナーさんにマスターデータ設定が\n間違っていないか確認しください。\n"
                                        + "\n不明な場合は\nクライアントプログラマに報告してください。";
                    Dialog dloalog = DialogManager.Open1B_Direct("No MasterDataRegion", messageText, "common_button7", true, true)
                        .SetOkEvent(() => { });
#endif
                    m_AreaSelect.IsViewRegionButton = false;
                    if (m_AreaCategory == null)
                    {
                        m_AreaCategory = new MasterDataDefineLabel.AreaCategory[] { MasterDataDefineLabel.AreaCategory.RN_STORY };
                    }

                    string sqlInString = string.Join(",", Array.ConvertAll<MasterDataDefineLabel.AreaCategory, string>(m_AreaCategory, o => o.ToString("D")));
                    areaCategoryMasterArray = MasterFinder<MasterDataAreaCategory>.Instance.SelectWhere(string.Format(" where area_cate_type in( {0} )", sqlInString)).ToArray();

                    // 背景のリソース設定
                    if (0 <= Array.IndexOf(m_AreaCategory, MasterDataDefineLabel.AreaCategory.RN_STORY))
                    {
                        m_AreaSelect.BackGroundImage = m_AreaSelect.backgroundSprites[0];
                        m_AreaSelect.currentCategory = MasterDataDefineLabel.AreaCategory.RN_STORY;
                        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk99p_description"));
                    }
                    else if (0 <= Array.IndexOf(m_AreaCategory, MasterDataDefineLabel.AreaCategory.RN_SCHOOL))
                    {
                        m_AreaSelect.BackGroundImage = m_AreaSelect.backgroundSprites[1];
                        m_AreaSelect.currentCategory = MasterDataDefineLabel.AreaCategory.RN_SCHOOL;
                        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk101p_description"));
                    }
                    else if (0 <= Array.IndexOf(m_AreaCategory, MasterDataDefineLabel.AreaCategory.RN_EVENT))
                    {
                        m_AreaSelect.BackGroundImage = m_AreaSelect.backgroundSprites[2];
                        m_AreaSelect.currentCategory = MasterDataDefineLabel.AreaCategory.RN_EVENT;
                        MainMenuHeader.UnderMsgRequest(GameTextUtil.GetText("kk100p_description"));
                    }
                    // エリアアイコン読み込みに遷移.
                    next();
                }
            }).Add(next =>
            {
                // エリアカテゴリリストの生成
                m_AreaSelect.ClearAreaData();
                m_areaIcons.Clear();

                AssetBundlerMultiplier multiplier = AssetBundlerMultiplier.Create();
                if (areaCategoryMasterArray != null)
                {
                    for (int i = 0; i < areaCategoryMasterArray.Length; ++i)
                    {
                        string area_title = areaCategoryMasterArray[i].area_cate_name;
                        uint area_cate_id = areaCategoryMasterArray[i].fix_id;
                        Vector2 area_pos = new Vector2(areaCategoryMasterArray[i].btn_posx_offset, areaCategoryMasterArray[i].btn_posy_offset * -1);

                        int index = m_areaIcons.Count; // areaCategoryMasterArrayのうち不採用のデータがあるので・・・

                        var model = makeAreaSelectModel(index, area_cate_id);

                        AreaDataContext newArea = MainMenuUtil.CreateRNAreaCategory(area_cate_id, model);
                        if (newArea != null)
                        {
                            newArea.Title = area_title;
                            newArea.PosX = area_pos.x;
                            newArea.PosY = area_pos.y;

                            // アセットバンドルの読み込み
                            string assetBundleName = string.Format("areamapicon_{0}", area_cate_id);
                            multiplier.Add(AssetBundler.Create().
                                Set(assetBundleName,
                                 (o) =>
                                 {
                                     newArea.IconImage = o.GetAsset<Sprite>();
                                     newArea.IconImage_mask = o.GetTexture(newArea.IconImage.name + "_mask", TextureWrapMode.Clamp);
                                 },
                                (s) =>
                                {
                                    newArea.IconImage = ResourceManager.Instance.Load("maeishoku_icon");
                                }).Load());

                            areaDataList.Add(newArea);
                            m_areaIcons.Add(model);
                        }
                    }

                    //成長ボスアイコン
                    if (m_CurrentRegionMaster.category == MasterDataDefineLabel.REGION_CATEGORY.EVENT &&
                        m_ChallengeAreaCategory != null)
                    {
                        uint area_cate_id = m_ChallengeAreaCategory.fix_id;
                        Vector2 area_pos = new Vector2(m_ChallengeAreaCategory.btn_posx_offset, m_ChallengeAreaCategory.btn_posy_offset * -1);
                        int index = m_areaIcons.Count; // areaCategoryMasterArrayのうち不採用のデータがあるので・・・
                        var model = makeAreaSelectModel(index, area_cate_id, true);
                        model.isChallenge = true;

                        AreaDataContext newArea = new AreaDataContext(model);
                        newArea.m_AreaIndex = m_ChallengeAreaCategory.fix_id;
                        newArea.IsViewFlag = false;
                        //newArea.FlagImage = ResourceManager.Instance.Load("completed");
                        //newArea.FlagImage = ResourceManager.Instance.Load("clear");
                        newArea.IsAreaNew = false;

                        newArea.Title = GameTextUtil.GetText("challenge_quest_title");
                        newArea.PosX = area_pos.x;
                        newArea.PosY = area_pos.y;
                        newArea.m_bufEvent = MainMenuUtil.IsCheckChallengeAmend();

                        // アセットバンドルの読み込み
                        string assetBundleName = string.Format("areamapicon_{0}", area_cate_id);
                        multiplier.Add(AssetBundler.Create().
                            Set(assetBundleName,
                             (o) =>
                             {
                                 newArea.IconImage = o.GetAsset<Sprite>();
                                 newArea.IconImage_mask = o.GetTexture(newArea.IconImage.name + "_mask", TextureWrapMode.Clamp);
                             },
                            (s) =>
                            {
                                newArea.IconImage = ResourceManager.Instance.Load("maeishoku_icon");
                            }).Load());

                        areaDataList.Add(newArea);
                        m_areaIcons.Add(model);
                    }
                }
#if BUILD_TYPE_DEBUG
                else if (m_CurrentRegionMaster != null)
                {
                    // MasterDataAreaCategory がなかった時のエラーダイアログ.
                    string messageText = "MasterDataAreaCategory に\n有効なデータがありませんでした。\n"
                                        + "プランナーさんにマスターデータ設定が\n間違っていないか確認してください。\n"
                                        + "\n不明な場合は\nクライアントプログラマに報告してください。";
                    Dialog dloalog = DialogManager.Open1B_Direct("No MasterDataAreaCategory", messageText, "common_button7", true, true)
                        .SetOkEvent(() => { });
                }
#endif

                multiplier.Load(() =>
                {
                    next();
                },
                () =>
                {
                    // エリアアイコンで読み込み出来ないものがあった場合も、
                    // 仮アイコンが設定されるため、アイコンリストを表示する.(進行不能対策)
                    next();
                });

            }).Add(next =>
            {
                SetAreaIconRoutine(areaDataList);
                m_AreaSelect.checkAnimationFinish();
                // インジケーターを閉じる
                LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
            }).Flush();
    }

    private AreaSelectListItemModel makeAreaSelectModel(int _index, uint _area_cate_id, bool bChallenge = false)
    {
        int index = _index;
        uint area_cate_id = _area_cate_id;

        System.Func<bool> IsLastestArea = () =>
        {
            return index == m_areaIcons.Count - 1;
        };

        var model = new AreaSelectListItemModel((uint)index);
        if (bChallenge)
        {
            model.OnClicked += () =>
            {
                OnSelectChallenge();
            };
        }
        else
        {
            model.OnClicked += () =>
            {
                OnSelectArea(area_cate_id);
            };
        }
        model.OnShowedNext += () =>
        {
            if (IsLastestArea())
            {
                m_AreaSelect.m_iconAnimationFinish = true;
                return;
            }

            m_areaIcons[index + 1].Appear();
        };
        model.OnAppeared += () =>
        {
            if (!IsLastestArea())
                return;

            foreach (var icon in m_areaIcons)
                icon.ShowTitle();
        };

        model.isActive = false;

        return model;
    }

    /// <summary>
    /// MAP背景画像を設定する.
    /// </summary>
    /// <param name="category">リージョンカテゴリ</param>
    /// <param name="fixId">MasterDataRegionのfix_id</param>
    private void SetAreaBackground(MasterDataDefineLabel.REGION_CATEGORY category, uint fixId, Action action)
    {
        if (m_AreaSelect == null)
        {
            return;
        }

        // デフォルトの背景を設定する.
        Sprite defaultSprite = null;
        switch (m_CurrentRegionMaster.category)
        {
            case MasterDataDefineLabel.REGION_CATEGORY.STORY:
                defaultSprite = m_AreaSelect.backgroundSprites[0];
                break;
            case MasterDataDefineLabel.REGION_CATEGORY.MATERIAL:
                defaultSprite = m_AreaSelect.backgroundSprites[1];
                break;
            case MasterDataDefineLabel.REGION_CATEGORY.EVENT:
                defaultSprite = m_AreaSelect.backgroundSprites[2];
                break;
        }

        var assetBundleName = string.Format("AreaBackGround_{0}", fixId);
        // 該当ファイルがAssetBundlePathMasterに存在しない場合は、カテゴリ毎のデフォルト画像を設定する.
        var assetBundlePath = MasterDataUtil.GetMasterDataAssetBundlePath(assetBundleName);
        if (assetBundlePath != null)
        {
            AssetBundler.Create().Set(
                assetBundleName, (o) =>
                {
                    m_AreaSelect.BackGroundImage = o.GetAsset<Sprite>();
                    if (action != null)
                    {
                        action();
                    }
                },
                (s) =>
                {
                    m_AreaSelect.BackGroundImage = defaultSprite;
                    if (action != null)
                    {
                        action();
                    }
                }
                ).Load();
        }
        else
        {
            m_AreaSelect.BackGroundImage = defaultSprite;
            if (action != null)
            {
                action();
            }
        }
    }

    /// <summary>
    /// アイコンを設定
    /// </summary>
    /// <param name="areaDataList"></param>
    /// <returns></returns>
    private void SetAreaIconRoutine(List<AreaDataContext> areaDataList)
    {
        m_AreaSelect.AppearSwitchButton();

        // ユーザーデータにストーリー進行度は存在しない。
        // area_cate_idが「最後尾のもの＆クリアフラグなし」を進行中のエリアと判定する。（指定範囲を除く）
        if (m_AreaSelect.currentCategory == MasterDataDefineLabel.AreaCategory.RN_STORY)
        {
            // 指定範囲内は除外する
            int selectAreaDataIndex = areaDataList.FindLastIndex((v) => v.m_AreaIndex < EXCLUDE_EFFECT_MIN_ID
                                                           || v.m_AreaIndex > EXCLUDE_EFFECT_MAX_ID);
            if (selectAreaDataIndex >= 0 && areaDataList[selectAreaDataIndex].FlagImage == null)
            {
                m_areaIcons[selectAreaDataIndex].isActive = true;
            }
        }

        // 読み込みの完了したアイコンを設定する.
        for (int i = 0; i < areaDataList.Count; i++)
        {
            m_AreaSelect.AddAreaData(areaDataList[i]);
        }
    }

    private void OnSwitch0()
    {
        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

        MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY.STORY);
        m_AreaCategory = new MasterDataDefineLabel.AreaCategory[] { MasterDataDefineLabel.AreaCategory.RN_STORY };
        SelectArea();
    }

    private void OnSwitch1()
    {
        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

        MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY.MATERIAL);
        m_AreaCategory = new MasterDataDefineLabel.AreaCategory[] { MasterDataDefineLabel.AreaCategory.RN_SCHOOL };
        SelectArea();
    }

    private void OnSwitch2()
    {
        SoundUtil.PlaySE(SEID.SE_MM_A03_TAB);

        MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY.EVENT);
        m_AreaCategory = new MasterDataDefineLabel.AreaCategory[] { MasterDataDefineLabel.AreaCategory.RN_EVENT };
        SelectArea();
    }

    /// <summary>
    /// MAPのボタンを押したとき
    /// </summary>
    /// <param name="area_cate_id"></param>
    private void OnSelectArea(uint area_cate_id)
    {
        if (MainMenuManager.HasInstance)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            MainMenuParam.SetQuestSelectParam(area_cate_id);
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false);
        }

    }

    private void OnSelectChallenge()
    {
        if (MainMenuManager.HasInstance)
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            MainMenuParam.SetChallengeSelectParam(0);
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_CHALLENGE_SELECT, false, false);
        }
    }

    private void OnSelectMapButton()
    {
        if (m_CurrentRegionMaster == null)
        {
            return;
        }
        if (m_RegionMasterList.IsNullOrEmpty() == true)
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

        int selectIndex = m_RegionMasterList.IndexOf(m_CurrentRegionMaster);

        RegionDialog.Create()
            .AddRegionList(m_RegionMasterList.ToArray(), selectIndex, OnSelectRegion, (RegionDialog dialog) =>
            {
                dialog.Show(OnHideRegionDialog);
                m_AreaSelect.IsViewRegionButton = false;
            });
    }

    private void OnSelectRegion(RegionContext context)
    {
        MainMenuParam.m_RegionID = context.master.fix_id;
    }

    void OnHideRegionDialog(bool isSelectRegion)
    {
        if (isSelectRegion == true)
        {
            SelectArea();
        }
        else
        {
            m_AreaSelect.IsViewRegionButton = true;
        }
    }

    public void openWarningRegionDialog()
    {
        // タブ切り替え中に期限切れになった場合、非表示になっては困るので、背景の有無で判断する
        if (m_AreaSelect.BackGroundImage == null)
        {
            //表示OFF
            UnityUtil.SetObjectEnabledOnce(m_AreaSelect.gameObject, false);
            UnityUtil.SetObjectEnabledOnce(m_MenuBanner.gameObject, false);
        }

        //Homeへ戻る
        Dialog _newDialog = Dialog.Create(DialogType.DialogOK);
        _newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "kk111q_title");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "kk111q_content");
        _newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        _newDialog.SetDialogEvent(DialogButtonEventType.OK, () =>
        {
            m_bReturnHome = true;
        });
        _newDialog.DisableCancelButton();
        _newDialog.Show();
    }

}
