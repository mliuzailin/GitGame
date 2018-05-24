using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SceneAssetBundlerTest : SceneTest<SceneAssetBundlerTest>
{
    public Image image01;
    public Image image02;
    public Image image03;
    public Image image04;
    public Image image05;
    public Image image06;
    public RawImage rawImage;
    public List<Image> imageList;

    public QualitySetting qualitySetting;

    public GameObject buttons;
    public uint unitId;
    public uint heroId;
    public uint episodeId;

    public HeroTestContext context;


    protected override void Awake()
    {
        base.Awake();
        buttons.SetActive(false);
    }


    public override void OnInitialized()
    {
        base.OnInitialized();
        SceneCommon.Instance.LoadResidentResource(() => { buttons.SetActive(true); });
    }

    private void Reset()
    {
        image01.sprite = null;
        image02.sprite = null;
        rawImage.texture = null;
    }


    //シンプル
    public void OnClickEpisode()
    {
        Reset();

        LocalSaveManagerRN.Instance.QualitySetting = qualitySetting;
        LocalSaveManagerRN.Instance.Save();

//        image01.sprite = ResidentResourceUtil.GetCharaIcon(unitId);
        AssetBundler.Create().Set("Areamap_" + episodeId, "Background", (o) =>
        {
//            Sprite[] sprites = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("tex_episode_perform_l_000"+episodeId);
//            image01.sprite = sprites[0];
            foreach (Object obj in o.AssetBundle.LoadAllAssets())
            {
                Debug.LogError("OBK:" + obj);
            }

        rawImage.texture = o.AssetBundle.LoadAsset<Texture>("Background");
            
        Debug.LogError("SPRIH:"+ o.AssetBundle.LoadAsset<Sprite>("icon01"));
            
        image01.overrideSprite = o.AssetBundle.LoadAsset<Sprite>("icon01");
        image02.sprite = o.AssetBundle.LoadAsset<Sprite>("icon02");
        image03.sprite = o.AssetBundle.LoadAsset<Sprite>("icon03");
        image04.sprite = o.AssetBundle.LoadAsset<Sprite>("icon04");
        image05.sprite = o.AssetBundle.LoadAsset<Sprite>("icon05");
        image06.sprite = o.AssetBundle.LoadAsset<Sprite>("icon06");
            
        }).Load();
    }

    //シンプル
    public void OnClickHero()
    {
        Reset();

        LocalSaveManagerRN.Instance.QualitySetting = qualitySetting;
        LocalSaveManagerRN.Instance.Save();

//        image01.sprite = ResidentResourceUtil.GetCharaIcon(unitId);
        AssetBundler.Create().Set("hero_000" + heroId, "Hero_0" + heroId, (o) =>
        {
//            Sprite[] sprites = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("tex_hero_perform_l_000"+heroId);
//            image01.sprite = sprites[0];
            foreach (Object obj in o.AssetBundle.LoadAllAssets())
            {
                Debug.LogError("OBK:" + obj);
            }

//            image01.sprite = o.AssetBundle.LoadAsset<Sprite>("Hero_0" + heroId);
//            context.Hero = o.AssetBundle.LoadAsset<Sprite>("Hero_0" + heroId);
//            context.Hero_mask = o.AssetBundle.LoadAsset<Texture>("Hero_0" + heroId  +"_mask");

            string assetName = string.Format("tex_hero_perform_l_{0:D4}" , (int)heroId);
            
            context.Hero = o.AssetBundle.LoadAsset<Sprite>(assetName);
            context.Hero_mask = o.GetTexture(assetName+"_mask", TextureWrapMode.Clamp);
            
        }).Load();
    }

    public string any;

    public void OnClickAny()
    {
        Reset();

        LocalSaveManagerRN.Instance.QualitySetting = qualitySetting;
        LocalSaveManagerRN.Instance.Save();

        AssetBundler.Create().Set(any.ToLower(),any.ToLower(),
            (o) =>
            {
                int index = 0;
                foreach (Sprite obj in o.AssetBundle.LoadAllAssets<Sprite>())
                {
                    Debug.LogError("OBK:" + obj);
                    imageList[index].overrideSprite = obj;

                    ++index;
                }
        }).Load();
    }

    public void OnClickUnit()
    {
        Reset();

        LocalSaveManagerRN.Instance.QualitySetting = qualitySetting;
        LocalSaveManagerRN.Instance.Save();

        UnitIconImageProvider.Instance.Get(
            unitId,
            sprite => 
            {
                image01.sprite = sprite;
            });

        AssetBundler.Create().SetAsUnitTexture(unitId,
            (o) =>
            {
            foreach (Object obj in o.AssetBundle.LoadAllAssets())
            {
                Debug.LogError("OBK:" + obj);
            }
                rawImage.texture = o.GetTexture2D(TextureWrapMode.Clamp);
                image02.sprite = o.GetAssetAll<Sprite>().FirstOrDefault(s => s.name.Contains("uncompressed"));
            }).Load();
    }

    //シンプル
    public void OnClickSimple()
    {
        Reset();
        AssetBundler.Create().Set("iconpackpanel",
            (o) =>
            {
                image01.sprite = o.GetSprites()[0];
//                    image01.sprite = o.GetAsset<>()
            }).Load();

        AssetBundler.Create().SetAsUnitTexture(unitId,
            (o) => { rawImage.texture = o.GetTexture2D("tex_chara_l_001", TextureWrapMode.Clamp); }).Load();
    }

    //複数のAssetBundleを並行して取得する場合
    public void OnClickMultiple()
    {
        Reset();
        AssetBundlerMultiplier multiplier = AssetBundlerMultiplier.Create();


        multiplier.Add(AssetBundler.Create().Set("iconpackpanel",
            (o) =>
            {
                image01.sprite = o.GetSprites()[0];
//                    image01.sprite = o.GetAsset<>()
            })).Load();

        multiplier.Add(AssetBundler.Create().SetAsUnitTexture(unitId,
            (o) => { rawImage.texture = o.GetTexture2D(TextureWrapMode.Clamp); })).Load();
        multiplier.Load(() =>
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("FINISH");
#endif
            },
            () =>
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("ERROR");
#endif
            });
    }

    private AssetBundler aber;

    //ステータス変化を検知してAssetを取得したい場合
    //※基本、これはソースが汚くなるので使わない
    //※既存ソースにこっちの方が組み込みやすい場合のみ使用する
    public void OnClickWaitChangeStatus()
    {
        if (aber != null)
        {
            return;
        }
        Reset();

        aber = AssetBundler.Create();
        aber.Set("iconpackpanel");
        aber.DisableAutoDestoryOnSuccess();
        aber.Load();
    }

    void Update()
    {
        if (aber == null)
        {
            return;
        }

        if (!aber.IsSuccess)
        {
            return;
        }

        image01.sprite = aber.Response.GetSprites()[0];
        aber.Destroy();
        aber = null;
    }
}
