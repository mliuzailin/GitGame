/**
 *  @file   MainMenuHeroPreview.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/21
 */

using UnityEngine;
using System;
using System.Collections;
using ServerDataDefine;

public class MainMenuHeroPreview : MainMenuSeq
{
    HeroPreview m_HeroPreview = null;

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

    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        //--------------------------------
        // オブジェクトの取得
        //--------------------------------
        m_HeroPreview = m_CanvasObj.GetComponentInChildren<HeroPreview>();
        m_HeroPreview.SetSizeParfect(new Vector2(0, -254));
        m_HeroPreview.OnClickViewAction = OnClickPreview;

        int currentHeroID = 0;

        if (UserDataAdmin.Instance.m_StructHeroList.Length > MainMenuParam.m_HeroCurrentInex)
        {
            PacketStructHero heroData = UserDataAdmin.Instance.m_StructHeroList[MainMenuParam.m_HeroCurrentInex];
            currentHeroID = heroData.hero_id;
        }

        // アセットバンドルの読み込み
        LoadingManager.Instance.RequestLoadingStart(LOADING_TYPE.ASSETBUNDLE);

        AssetBundler.Create().
        Set(string.Format("hero_{0:D4}", currentHeroID),
        (o) =>
        {
            Sprite[] sprites = o.AssetBundle.LoadAssetWithSubAssets<Sprite>(string.Format("tex_hero_stand_l_{0:D4}", currentHeroID));
            Texture maskTextue = o.GetTexture(string.Format("tex_hero_stand_l_{0:D4}_mask", currentHeroID), TextureWrapMode.Clamp);
            m_HeroPreview.UnitImage = ImageUtil.GetSprite(sprites, "body");
            m_HeroPreview.UnitImage_mask = maskTextue;

            // インジケーターを閉じる
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
        },
        (str) =>
        {
            // インジケーターを閉じる
            LoadingManager.Instance.RequestLoadingFinish(LOADING_TYPE.ASSETBUNDLE);
        }).
        Load();
    }

    void OnClickPreview()
    {
        if (MainMenuManager.HasInstance)
        {
            MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_HERO_FORM, false, true);
        }
    }
}
