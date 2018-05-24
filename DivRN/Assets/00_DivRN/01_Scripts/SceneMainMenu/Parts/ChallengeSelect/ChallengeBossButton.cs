using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ChallengeBossButton : ButtonView
{
    public static readonly string PrefabPath = "Prefab/ChallengeSelect/ChallengeBossButton";

    private ButtonModel m_model = null;

    private M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    private M4uProperty<Texture> iconImageMask = new M4uProperty<Texture>();
    public Texture IconImageMask { get { return iconImageMask.Value; } set { iconImageMask.Value = value; } }

    private M4uProperty<Color> iconColor = new M4uProperty<Color>();
    public Color IconColor { get { return iconColor.Value; } set { iconColor.Value = value; } }

    public static ChallengeBossButton Attach(GameObject parent)
    {
        return ButtonView.Attach<ChallengeBossButton>(PrefabPath, parent);
    }


    public ChallengeBossButton SetModel(ButtonModel model)
    {
        m_model = model;

        base.SetModel<ButtonModel>(m_model);

        return this;
    }
    void Awake()
    {
        AppearAnimationName = "mainmenu_home_challenge_appear";
        DefaultAnimationName = "mainmenu_home_challenge_loop";

        GetComponent<M4uContextRoot>().Context = this;

        IconImage = null;
        IconColor = new Color(1, 1, 1, 0);
    }

    public void LoadIcon(uint area_category_id)
    {
        // アセットバンドルの読み込み
        string assetBundleName = string.Format("areamapicon_{0}", area_category_id);
        AssetBundler.Create()
            .Set(
            assetBundleName,
            (o) =>
            {//Success
                IconImage = o.GetAsset<Sprite>();
                IconImageMask = o.GetTexture(IconImage.name + "_mask", TextureWrapMode.Clamp);
                IconColor = new Color(1, 1, 1, 1);
            },
            (s) =>
            {//Error
                IconImage = ResourceManager.Instance.Load("maeishoku_icon");
                IconImageMask = null;
                IconColor = new Color(1, 1, 1, 1);
            })
            .Load();

    }
}
