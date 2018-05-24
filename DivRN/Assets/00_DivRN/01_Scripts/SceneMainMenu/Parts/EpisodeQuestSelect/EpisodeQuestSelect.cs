using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class EpisodeQuestSelect : MenuPartsBase
{
    private static readonly string AppearAnimationName = "episode_quest_select_window_appear";
    private static readonly string DefaultAnimationName = "episode_quest_select_window_loop";

    public enum MaskType
    {
        Full,
        Extension,
    }

    public GameObject viewMask1 = null;
    public GameObject viewMask2 = null;
    [SerializeField]
    ScrollRect EpisodeScrollRect = null;
    [SerializeField]
    GameObject EpisodeListLineObject = null;
    [SerializeField]
    public GameObject DetailButtonFullRoot = null;
    [SerializeField]
    public GameObject DetailButtonRoot = null;

    M4uProperty<string> episodeTitle = new M4uProperty<string>();
    public string EpisodeTitle { get { return episodeTitle.Value; } set { episodeTitle.Value = value; } }

    M4uProperty<string> areaTitle = new M4uProperty<string>();
    public string AreaTitle { get { return areaTitle.Value; } set { areaTitle.Value = value; } }

    M4uProperty<List<EpisodeDataContext>> episodeList = new M4uProperty<List<EpisodeDataContext>>();
    public List<EpisodeDataContext> EpisodeList { get { return episodeList.Value; } set { episodeList.Value = value; } }

    M4uProperty<List<QuestDataContext>> questList = new M4uProperty<List<QuestDataContext>>();
    public List<QuestDataContext> QuestList { get { return questList.Value; } set { questList.Value = value; } }

    List<GameObject> questObjList = new List<GameObject>();
    public List<GameObject> QuestObjList { get { return questObjList; } set { questObjList = value; } }

    M4uProperty<bool> isFullMask = new M4uProperty<bool>();
    public bool IsFullMask { get { return isFullMask.Value; } set { isFullMask.Value = value; } }

    M4uProperty<bool> isViewDetailButton = new M4uProperty<bool>(true);
    public bool IsViewDetailButton { get { return isViewDetailButton.Value; } set { isViewDetailButton.Value = value; } }

    M4uProperty<float> questListAlpha = new M4uProperty<float>(0);
    public float QuestListAlpha { get { return questListAlpha.Value; } set { questListAlpha.Value = value; } }

    private Action m_UpdateUVAction = null;
    public bool isEndShowList = false;
    public MasterDataEvent m_EventMaster = null;

    private int updateLayoutCount = 0;

    private bool m_bEpisodeFirst = false;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        EpisodeList = new List<EpisodeDataContext>();
        QuestList = new List<QuestDataContext>();
        resetMask();
    }

    private void Update()
    {
        if (updateLayoutCount != 0)
        {
            updateLayoutCount--;
            if (updateLayoutCount < 0)
            {
                updateLayoutCount = 0;
            }
            SetScrollSettings();
            updateLayout();
        }
    }

    private void LateUpdate()
    {
        if (isEndShowList == false
        && QuestObjList != null)
        {
            UpdateUV();
        }
    }

    public AssetAutoSetEpisodeBackgroundTexture assetAutoSetEpisodeBackgroundTexture;

    public void Show(uint areaCategoryFixId, System.Action callback, System.Action finishLoadAction)
    {
        Action action = () =>
        {
            if (finishLoadAction != null)
            {
                finishLoadAction();
            }

            PlayAnimation(AppearAnimationName, () =>
            {
                PlayAnimation(DefaultAnimationName);

                if (callback != null)
                {
                    callback();
                }
            });
        };

        Action itemAction = () =>
        {
            // Cellの背景取得
            // M4Uで接続されるまで時間がかかるので専用処理
            AssetBundlerMultiplier multiplier2 = AssetBundlerMultiplier.Create();
            if (viewMask2 != null)
            {
                GameObject gobj = viewMask2.transform.Find("Content").gameObject;
                QuestDataListItem[] items = gobj.GetComponentsInChildren<QuestDataListItem>();
                for (int i = 0; i < items.Length; i++)
                {
                    multiplier2.Add(items[i].CreateBg());
                }

                multiplier2.Load(() =>
                {
                    if (action != null)
                    {
                        action();
                    }
                },
                () =>
                {
                    if (action != null)
                    {
                        action();
                    }
                });
            }
            else
            {
                if (action != null)
                {
                    action();
                }
            }
        };

        AssetBundlerMultiplier multiplier = AssetBundlerMultiplier.Create();
        multiplier.Add(assetAutoSetEpisodeBackgroundTexture.Create(areaCategoryFixId));
        if (m_bEpisodeFirst == false)
        {
            for (int i = 0; i < EpisodeList.Count; i++)
            {
                EpisodeDataContext context = EpisodeList[i];
                multiplier.Add(context.CreatetIcon());
            }
            m_bEpisodeFirst = true;
        }

        multiplier.Load(() =>
        {
            if (itemAction != null)
            {
                itemAction();
            }
        },
        () =>
        {
            if (itemAction != null)
            {
                itemAction();
            }
        });
    }

    public void resetMask()
    {
        IsFullMask = false;
        RectTransform trans = viewMask2.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(trans.sizeDelta.x, 12);
    }

    public void checkMask()
    {
        RectTransform trans1 = viewMask1.GetComponent<RectTransform>();
        RectTransform trans2 = viewMask2.GetComponent<RectTransform>();
        if (trans1.rect.height > trans2.rect.height)
        {
            IsFullMask = false;
            viewMask1.GetComponent<Mask>().enabled = false;
            viewMask1.GetComponent<Image>().enabled = false;
            viewMask2.GetComponent<Mask>().enabled = true;
            viewMask2.GetComponent<Image>().enabled = true;
        }
        else
        {
            IsFullMask = true;
            viewMask1.GetComponent<Mask>().enabled = true;
            viewMask1.GetComponent<Image>().enabled = true;
            viewMask2.GetComponent<Mask>().enabled = false;
            viewMask2.GetComponent<Image>().enabled = false;
        }
    }

    void SetScrollSettings()
    {
        float episodeHeight = EpisodeScrollRect.viewport.rect.height;
        float episodeContentHeight = EpisodeScrollRect.content.rect.height;
        if (episodeContentHeight <= episodeHeight)
        {
            EpisodeScrollRect.vertical = false;
            EpisodeListLineObject.SetActive(false);
        }
        else
        {
            EpisodeScrollRect.vertical = true;
            EpisodeListLineObject.SetActive(true);
        }
    }

    /// <summary>
    /// リストのUVを更新する
    /// </summary>
    public void UpdateUV()
    {
        if (m_UpdateUVAction != null)
        {
            m_UpdateUVAction();
        }
    }

    public void OnValueChangedScroll()
    {
        UpdateUV();
    }

    void OnChangedEpiSodeList()
    {
        updateLayoutCount = 5;
    }

    void OnChangedQuestList()
    {
        m_UpdateUVAction = delegate { };
        if (QuestObjList != null)
        {
            for (int i = 0; i < QuestObjList.Count; ++i)
            {
                AutoUVSetting uvSetting = QuestObjList[i].GetComponentInChildren<AutoUVSetting>();
                if (uvSetting != null)
                {
                    m_UpdateUVAction += uvSetting.UpdateUV;
                }
            }
        }
    }
}
