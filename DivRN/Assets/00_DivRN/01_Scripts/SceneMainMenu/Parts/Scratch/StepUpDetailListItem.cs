using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepUpDetailListItem : ListItem<StepUpDetailListContext>
{
    [SerializeField]
    ButtonView LinUpNormalButton;
    [SerializeField]
    ButtonView LinUpRainbowButton;
    [SerializeField]
    Image BannerImage;

    private RectTransform m_ImageRect;
    private void Awake()
    {
        BannerImage.color = new Color(1, 1, 1, 0);
        m_ImageRect = BannerImage.GetComponent<RectTransform>();
        m_ImageRect.sizeDelta = new Vector2(m_ImageRect.sizeDelta.x, 0);
    }

    // Use this for initialization
    void Start()
    {
        SetUpButtons();

        if (Context.Banner_url.IsNullOrEmpty() == true)
        {
            BannerImage.gameObject.SetActive(false);
        }
        else
        {
            string url = string.Format(GlobalDefine.GetScratchBoadhUrl(), Context.Banner_url);
            WebResource.Instance.GetSprite(url,
                (Sprite sprite) =>
                {
                    if (BannerImage != null)
                    {
                        BannerImage.sprite = sprite;
                        BannerImage.color = Color.white;
                        SetImageSize();
                    }
                },
                () =>
                {
                    if (BannerImage != null)
                    {
                        BannerImage.sprite = ResourceManager.Instance.Load("dummy_scratch_banner", ResourceType.Common);
                        BannerImage.color = Color.white;
                        SetImageSize();
                    }
                });
        }
    }

    void SetImageSize()
    {
        if (BannerImage.sprite == null)
        {
            return;
        }

        // 横幅を固定で画像比率を合わせる
        float widthImageRatio = m_ImageRect.sizeDelta.x / BannerImage.sprite.rect.width;
        float afterHeight = BannerImage.sprite.rect.height * widthImageRatio;
        m_ImageRect.sizeDelta = new Vector2(m_ImageRect.sizeDelta.x, afterHeight);

        if (Context.FinishLoadImageAction != null)
        {
            Context.FinishLoadImageAction();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetUpButtons()
    {
        LinUpNormalButton.SetModel<ButtonModel>(Context.LinUpNormalButtonModel);
        Context.LinUpNormalButtonModel.Appear();
        Context.LinUpNormalButtonModel.SkipAppearing();

        LinUpRainbowButton.SetModel<ButtonModel>(Context.LinUpRainbowButtonModel);
        Context.LinUpRainbowButtonModel.Appear();
        Context.LinUpRainbowButtonModel.SkipAppearing();

    }
}
