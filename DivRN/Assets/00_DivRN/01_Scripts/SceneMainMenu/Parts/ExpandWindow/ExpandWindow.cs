//#define DVGAN_2094_DETAIL_BUTTON

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using M4u;

public class ExpandWindow : MenuPartsBase
{

    [SerializeField]
    private GameObject m_buttonRoot;

    private static readonly string ButtonPrefabPath = "Prefab/ExpandWindow/ExpandWindowButton";


    public GameObject Content = null;
    public System.Action DidSelectButton = delegate { };

    M4uProperty<float> viewHeight = new M4uProperty<float>();
    public float ViewHeight { get { return viewHeight.Value; } set { viewHeight.Value = value; } }

    M4uProperty<float> arrowRotateZ = new M4uProperty<float>();
    public float ArrowRotateZ { get { return arrowRotateZ.Value; } set { arrowRotateZ.Value = value; } }

    M4uProperty<Sprite> tabButtonImage = new M4uProperty<Sprite>();
    private Sprite TabButtonImage { get { return tabButtonImage.Value; } set { tabButtonImage.Value = value; } }

    private bool m_bOpen = false;
    public bool isOpen { get { return m_bOpen; } }

    private bool m_bAutoWindow = true;
    public bool AutoWindow { get { return m_bAutoWindow; } set { m_bAutoWindow = value; } }

    private float m_ViewHeightSize = 400.0f;
    public float ViewHeightSize { get { return m_ViewHeightSize; } set { m_ViewHeightSize = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        ViewHeight = 0.0f;
        ArrowRotateZ = 180.0f;

#if DVGAN_2094_DETAIL_BUTTON // 詳細ボタン一旦削除（復活の可能性ありの為コメント、削除確定したら消す）
		SetUpButtons();
#endif
        ChangeTabImage();
    }

    public void OnSelectButton()
    {
        if (m_bOpen == false)
        {
            SetBackKey(true);
        }
        else
        {
            SetBackKey(false);
        }

        if (m_bAutoWindow) changeWindow(!m_bOpen);
        DidSelectButton();
    }

    private void OnBackKeyInfoButton()
    {
        AndroidBackKeyManager.Instance.StackPop(gameObject);
#if DVGAN_2094_DETAIL_BUTTON // 詳細ボタン一旦削除（復活の可能性ありの為コメント、削除確定したら消す）
		changeWindow(!m_bOpen);
#endif
        DidSelectButton();
    }

    private void changeWindow(bool bFlag, bool bFast = false)
    {
        if (m_bOpen == bFlag)
        {
            return;
        }

        if (!m_bOpen)
        {
            if (bFast)
            {
                ViewHeight = m_ViewHeightSize;
                ChangeTabImage();
            }
            else
            {
                DOTween.To(() => ViewHeight, h => ViewHeight = h, m_ViewHeightSize, 0.3f)
                    .Play()
                    .OnComplete(() =>
                    {
                        ChangeTabImage();
                    });
            }
            ArrowRotateZ = 0.0f;
        }
        else
        {
            if (bFast)
            {
                ViewHeight = 0;
                ChangeTabImage();
            }
            else
            {
                DOTween.To(() => ViewHeight, h => ViewHeight = h, 0, 0.3f)
                    .Play()
                    .OnComplete(() =>
                    {
                        ChangeTabImage();
                    });
            }
            ArrowRotateZ = 180.0f;
        }
        m_bOpen = bFlag;
    }

    void ChangeTabImage()
    {
        if (m_bOpen)
        {
            TabButtonImage = ResourceManager.Instance.Load("menu_icon_back", ResourceType.Common);
        }
        else
        {
            TabButtonImage = ResourceManager.Instance.Load("menu_icon_info", ResourceType.Common);
        }

    }

    public void Open(bool bFase = false)
    {
        changeWindow(true, bFase);
    }

    public void Close(bool bFast = false)
    {
        changeWindow(false, bFast);
    }

    public void SetBackKey(bool isSet)
    {
        if (isSet == true)
        {
            AndroidBackKeyManager.Instance.StackPush(gameObject, OnBackKeyInfoButton);
        }
        else
        {
            AndroidBackKeyManager.Instance.StackPop(gameObject);
        }
    }


#if DVGAN_2094_DETAIL_BUTTON // 詳細ボタン一旦削除（復活の可能性ありの為コメント、削除確定したら消す）
	private void SetUpButtons()
    {
        var model = new ButtonModel();
        ButtonView
            .Attach<ButtonView>(ButtonPrefabPath, m_buttonRoot)
            .SetModel<ButtonModel>(model);

        model.OnClicked += () =>
        {
            OnSelectButton();
        };


        // TODO : 演出を入れるならそこに移動
        model.Appear();
        model.SkipAppearing();
    }
#endif
}
