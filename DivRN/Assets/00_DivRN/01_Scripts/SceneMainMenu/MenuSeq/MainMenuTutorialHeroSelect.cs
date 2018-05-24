/**
 *  @file   MainMenuTutorialHeroSelect.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/20
 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using ServerDataDefine;

public class MainMenuTutorialHeroSelect : MainMenuSeq
{
    TutorialHeroSelect m_TutorialHeroSelect = null;

    List<HeroSelectListContext> m_HeroSelectList = null;

    public bool isSelect
    {
        get { return m_TutorialHeroSelect.isSelect; }
    }

    private bool LoadHeroLoding = true;

    public bool isfinishDecision
    {
        get
        {
            if (m_TutorialHeroSelect == null)
            {
                return false;
            }
            else
            {
                return m_TutorialHeroSelect.isfinishDecision;
            }
        }
    }

    private bool inDecision;

    public bool isDecision
    {
        get { return inDecision; }
    }

    private void Awake()
    {
        GameObject canvasObj = UnityUtil.GetChildNode(gameObject, "Canvas");
        if (canvasObj != null)
        {
            CanvasGroup canvasGroup = canvasObj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.0f;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }

    public override bool PageSwitchEventEnableBefore(bool bBack = false)
    {
        bool bEnable = base.PageSwitchEventEnableBefore();

        return bEnable;
    }

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //--------------------------------
        //	オブジェクトの取得
        //--------------------------------
        m_TutorialHeroSelect = m_CanvasObj.GetComponentInChildren<TutorialHeroSelect>();

        m_TutorialHeroSelect.OnClickDecisionButtonAction = OnClickFormDecisionButton;

        LoadHeroData();
    }

    public override bool PageSwitchEventDisableAfter(MAINMENU_SEQ eNextMainMenuSeq)
    {
        base.PageSwitchEventDisableAfter(eNextMainMenuSeq);

        return false;
    }

    public override bool PageSwitchEventEnableAfter()
    {
        bool enable = base.PageSwitchEventEnableAfter();

        if (LoadHeroLoding)
        {
            return true;
        }

        return enable;
    }

    /// <summary>
    /// 次ボタンを押したとき
    /// </summary>
    void OnClickFormDecisionButton()
    {
        if (m_TutorialHeroSelect.isSelect)
        {
            return;
        }

        if (UnitDetailInfo.GetUnitDetailInfo() != null)
        {
            return;
        }

        if (m_TutorialHeroSelect.isfinishDecision)
        {
            return;
        }

        if (inDecision)
        {
            return;
        }

        inDecision = true;
        MasterDataHero[] master = MasterFinder<MasterDataHero>.Instance.GetAll();

        m_TutorialHeroSelect.Decision(master[m_TutorialHeroSelect.m_CurrentIndex], () =>
        {
            inDecision = false;
        }, () =>
        {
            inDecision = false;
        });
    }


    IEnumerator LoadHeroDataWait()
    {
        UnitIconImageProvider.Instance.Tick();

        yield return null;

        while (m_HeroSelectList == null)
        {
            yield return null;
        }

        // パーティ一覧の描画
        m_TutorialHeroSelect.SelectDatas = m_HeroSelectList;

        while (m_TutorialHeroSelect.SelectDatas.Count < m_HeroSelectList.Count)
        {
            yield return null;
        }

        int count = m_TutorialHeroSelect.SelectDatas.Count - 1;
        while (m_TutorialHeroSelect.SelectDatas[count].UnitList.Count == 0 ||
                m_TutorialHeroSelect.SelectDatas[count].SkillListDatas.Count == 0)
        {
            yield return null;
        }

        // インジケーターを閉じる
        LoadingManager.Instance.RequestLoadingFinish();

        TutorialHeroSelectFSM.Instance.SendFsmNextEvent();

        LoadHeroLoding = false;
    }

    void LoadHeroData()
    {
        m_HeroSelectList = new List<HeroSelectListContext>();

        MasterDataHero[] masters = MasterFinder<MasterDataHero>.Instance.SelectWhere("ORDER BY fix_id").ToArray();
        m_TutorialHeroSelect.m_HeroImage = new Sprite[masters.Length];
        m_TutorialHeroSelect.m_HeroImage_mask = new Texture[masters.Length];

        // アセットバンドルの読み込み
        AssetBundlerMultiplier multiplier = AssetBundlerMultiplier.Create();
        for (int i = 0; i < masters.Length; i++)
        {

            MasterDataHero heromaster = masters[i];
            string hero_name = String.Format("hero_{0:D4}", heromaster.fix_id);
            string hero_perfom_name = String.Format("tex_hero_perform_l_{0:D4}", heromaster.fix_id);
            string hero_mask_name = String.Format("tex_hero_perform_l_{0:D4}_mask", heromaster.fix_id);
            int index = i;

            multiplier.Add(AssetBundler.Create().Set(hero_name, hero_perfom_name, (o) =>
            {
                Sprite[] sprites = o.AssetBundle.LoadAssetWithSubAssets<Sprite>(hero_name);

                Sprite[] herosprites = o.AssetBundle.LoadAssetWithSubAssets<Sprite>(hero_perfom_name);
                Texture maskTextue = o.GetTexture(hero_mask_name, TextureWrapMode.Clamp);
                m_TutorialHeroSelect.m_HeroImage[index] = herosprites[0];
                m_TutorialHeroSelect.m_HeroImage_mask[index] = maskTextue;

                switch (index)
                {
                    case 0:
                        m_TutorialHeroSelect.Tutorial_hero01 = sprites[4];
                        break;
                    case 1:
                        m_TutorialHeroSelect.Tutorial_hero02 = sprites[4];
                        break;
                    case 2:
                        m_TutorialHeroSelect.Tutorial_hero03 = sprites[4];
                        break;
                    case 3:
                        m_TutorialHeroSelect.Tutorial_hero04 = sprites[4];
                        break;
                    case 4:
                        m_TutorialHeroSelect.Tutorial_hero05 = sprites[4];
                        break;
                    case 5:
                        m_TutorialHeroSelect.Tutorial_hero06 = sprites[4];
                        break;
                }

                HeroSelectListContext heroSelect = new HeroSelectListContext();
                heroSelect.CreatePartyParam(heromaster.default_party_id, this);
                heroSelect.CreateSkillList(heromaster);
                heroSelect.Tutorial_text = GameTextUtil.GetText("tutorial_selecttext13");
                heroSelect.Profile_text = heromaster.detail;

                m_HeroSelectList.Add(heroSelect);
            }));

        }

        multiplier.Load(() =>
        {
            StartCoroutine(LoadHeroDataWait());
        },
        () =>
        {
            StartCoroutine(LoadHeroDataWait());
        });
    }
}
