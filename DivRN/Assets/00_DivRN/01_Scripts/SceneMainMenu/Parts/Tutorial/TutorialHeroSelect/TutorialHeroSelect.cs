/**
 *  @file   TutorialHeroSelect.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/20
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using AsPerSpec;
using ServerDataDefine;

public class TutorialHeroSelect : MenuPartsBase
{
    [SerializeField]
    private GameObject m_TopMask;
    [SerializeField]
    private GameObject m_BottomMask;
    [SerializeField]
    private GameObject m_BackGround;
    [SerializeField]
    private GameObject m_HeroSelect;

    [SerializeField]
    private Animation m_SelectHeroInAnim;

    private bool m_select = true;   // 主人公選択チェック
    private bool m_decision = false;

    private bool m_finishDecision = false;
    public bool isfinishDecision
    {
        get { return m_finishDecision; }
    }

    private Animation m_HeroInAnimation = null;

    [HideInInspector]
    public Sprite[] m_HeroImage = null;
    [HideInInspector]
    public Texture[] m_HeroImage_mask = null;

    public bool isSelect
    {
        get { return m_select; }
    }


    M4uProperty<List<HeroSelectListContext>> selectDatas = new M4uProperty<List<HeroSelectListContext>>(new List<HeroSelectListContext>());
    /// <summary>学生証リスト</summary>
    public List<HeroSelectListContext> SelectDatas { get { return selectDatas.Value; } set { selectDatas.Value = value; } }

    M4uProperty<string> hero_select_msg = new M4uProperty<string>();
    /// <summary>学生番号</summary>
    public string Hero_select_msg { get { return hero_select_msg.Value; } set { hero_select_msg.Value = value; } }

    M4uProperty<string> decision_text = new M4uProperty<string>();
    /// <summary>学生番号</summary>
    public string Decision_text { get { return decision_text.Value; } set { decision_text.Value = value; } }

    List<GameObject> selectDataList = new List<GameObject>();
    public List<GameObject> SelectDataList { get { return selectDataList; } set { selectDataList = value; } }

    M4uProperty<string> hero_name = new M4uProperty<string>();
    /// <summary>学生番号</summary>
    public string Hero_name { get { return hero_name.Value; } set { hero_name.Value = value; } }

    M4uProperty<string> hero_subname = new M4uProperty<string>();
    /// <summary>学生番号</summary>
    public string Hero_subname { get { return hero_subname.Value; } set { hero_subname.Value = value; } }

    M4uProperty<Sprite> hero_image = new M4uProperty<Sprite>();
    public Sprite Hero_image { get { return hero_image.Value; } set { hero_image.Value = value; } }

    M4uProperty<Texture> hero_image_mask = new M4uProperty<Texture>();
    public Texture Hero_image_mask { get { return hero_image_mask.Value; } set { hero_image_mask.Value = value; } }

    M4uProperty<Sprite> tutorial_hero01 = new M4uProperty<Sprite>();
    public Sprite Tutorial_hero01 { get { return tutorial_hero01.Value; } set { tutorial_hero01.Value = value; } }

    M4uProperty<Sprite> tutorial_hero02 = new M4uProperty<Sprite>();
    public Sprite Tutorial_hero02 { get { return tutorial_hero02.Value; } set { tutorial_hero02.Value = value; } }

    M4uProperty<Sprite> tutorial_hero03 = new M4uProperty<Sprite>();
    public Sprite Tutorial_hero03 { get { return tutorial_hero03.Value; } set { tutorial_hero03.Value = value; } }

    M4uProperty<Sprite> tutorial_hero04 = new M4uProperty<Sprite>();
    public Sprite Tutorial_hero04 { get { return tutorial_hero04.Value; } set { tutorial_hero04.Value = value; } }

    M4uProperty<Sprite> tutorial_hero05 = new M4uProperty<Sprite>();
    public Sprite Tutorial_hero05 { get { return tutorial_hero05.Value; } set { tutorial_hero05.Value = value; } }

    M4uProperty<Sprite> tutorial_hero06 = new M4uProperty<Sprite>();
    public Sprite Tutorial_hero06 { get { return tutorial_hero06.Value; } set { tutorial_hero06.Value = value; } }

    M4uProperty<Color> hero01_color = new M4uProperty<Color>();
    public Color Hero01_color { get { return hero01_color.Value; } set { hero01_color.Value = value; } }

    M4uProperty<Color> hero02_color = new M4uProperty<Color>();
    public Color Hero02_color { get { return hero02_color.Value; } set { hero02_color.Value = value; } }

    M4uProperty<Color> hero03_color = new M4uProperty<Color>();
    public Color Hero03_color { get { return hero03_color.Value; } set { hero03_color.Value = value; } }

    M4uProperty<Color> hero04_color = new M4uProperty<Color>();
    public Color Hero04_color { get { return hero04_color.Value; } set { hero04_color.Value = value; } }

    M4uProperty<Color> hero05_color = new M4uProperty<Color>();
    public Color Hero05_color { get { return hero05_color.Value; } set { hero05_color.Value = value; } }

    M4uProperty<Color> hero06_color = new M4uProperty<Color>();
    public Color Hero06_color { get { return hero06_color.Value; } set { hero06_color.Value = value; } }

    /// <summary>前に戻るボタンを押したときのアクション</summary>
    public Action OnClickPreviousButtonAction = delegate { };
    /// <summary>次に進むボタンを押したときのアクション</summary>
    public Action OnClickNextButtonAction = delegate { };
    /// <summary>現在の表示位置</summary>
    public int m_CurrentIndex = 0;
    /// <summary>拡縮するビューのRect</summary>
    [SerializeField]
    RectTransform m_ContentRect;

    public Button m_DecisionButton = null;

    public Action OnClickDecisionButtonAction = delegate { };

    private Color m_Gray = new Color(0.4f, 0.4f, 0.4f, 1);

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        Decision_text = GameTextUtil.GetText("common_button7");
        Hero_select_msg = GameTextUtil.GetText("mastr_select");

        if (SafeAreaControl.HasInstance)
        {
            SafeAreaControl.Instance.enebleMask(m_TopMask, m_BottomMask);

            int bottom_space_height = SafeAreaControl.Instance.bottom_space_height;
            int bar_height = SafeAreaControl.Instance.bar_height;
            if (bottom_space_height > 0)
            {
                int space = bottom_space_height + bar_height;

                RectTransform rect = m_BottomMask.GetComponent<RectTransform>();
                rect.AddLocalPositionY(space * -1);

                rect = GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y + space);

                rect = m_BackGround.GetComponent<RectTransform>();
                rect.AddLocalPositionY((space / 2) * -1);

                rect = m_HeroSelect.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y - 120);
            }
        }
    }

    private void OnDestroy()
    {
        m_DecisionButton = null;
        OnClickDecisionButtonAction = null;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
    }

    /// <summary>
    /// 前に戻るボタンが押された
    /// </summary>
    public void OnClickPreviousButton()
    {
        if (OnClickPreviousButtonAction != null)
        {
            OnClickPreviousButtonAction();
        }
    }

    /// <summary>
    /// 次に進むボタンを押された
    /// </summary>
    public void OnClickNextButton()
    {
        if (OnClickNextButtonAction != null)
        {
            OnClickNextButtonAction();
        }
    }

    /// <summary>
    /// ページボタンが押された
    /// </summary>
    /// <param name="point"></param>
    void OnClickDecisionButton()
    {
        if (OnClickDecisionButtonAction != null)
        {
            OnClickDecisionButtonAction();
        }
    }


    /// <summary>
    /// CollectionBindingのデータが変更されたとき
    /// </summary>
    void OnChangeSelectDataList()
    {
    }

    private void SetUpButtons()
    {
        if (SelectDatas.Count <= 1)
        {
            return;
        }

        m_DecisionButton.onClick.AddListener(() =>
        {
            SoundUtil.PlaySE(SEID.SE_MENU_OK2);
            OnClickDecisionButton();
        });

    }

    public void OnInitWait()
    {
        //MainMenuTutorialHeroSelect→LoadHeroDataの完了トリガーで動作
    }

    public void OnHeroSelectInWait()
    {
        for (int i = 0; i < SelectDatas.Count; ++i)
        {
            for (int n = 0; n < SelectDatas[i].Units.Count; ++n)
            {
                SelectDatas[i].Units[n].model.Appear();
            }

            setHeroColor(i, false);
        }

        m_HeroInAnimation = null;
        m_CurrentIndex = 0;

        if (UserDataAdmin.Instance != null)
        {
            if (UserDataAdmin.Instance.HeroId > 0)
            {
                m_CurrentIndex = UserDataAdmin.Instance.HeroId - 1;
            }
        }

        setHero(m_CurrentIndex);
        setHeroColor(m_CurrentIndex, true);

        TutorialHeroSelectFSM.Instance.SendFsmNextEvent();
    }

    public void OnHeroInWait()
    {
        if (m_HeroInAnimation != null)
        {
            if (m_SelectHeroInAnim.isPlaying == true ||
                m_HeroInAnimation.isPlaying == true)
            {
                return;
            }
        }

        setHeroName(m_CurrentIndex);

#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (DebugOption.Instance != null &&
            DebugOption.Instance.tutorialDO.forceTutorialPart == TutorialPart.NONE)
        {
#endif
            if (UserDataAdmin.Instance.m_StructPlayer.renew_first_select_num >= 0)
            {
                TutorialHeroSelectFSM.Instance.SendFsmNegativeEvent();
            }
#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        }
#endif

        TutorialHeroSelectFSM.Instance.SendFsmPositiveEvent();
    }

    public void OnHeroSelectGuide()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "tutorial_master_result_01_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "tutorial_master_result_01_main");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            TutorialManager.SendStep(
                4,
                () =>
                {
                    TutorialDialog.Create().SetTutorialType(TutorialDialog.FLAG_TYPE.UNIT_HERO).Show();
                    m_select = false;
                    SetUpButtons();

                    TutorialHeroSelectFSM.Instance.SendFsmNextEvent();

                });
        }));
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    public void Decision(MasterDataHero master, Action finishDecision = null, Action cancelAction = null)
    {
        m_decision = true;
        TutorialManager.SendStep((int)TutorialStep.HERO_SELECT,
            () =>
            {
                Dialog newDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
                newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "tutorial_master_result_02_title");
                newDialog.SetDialogText(DialogTextType.MainText, string.Format(GameTextUtil.GetText("tutorial_master_result_02_main"), master.name));
                newDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
                newDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
                newDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
                {
                    ServerDataUtilSend.SendPacketAPI_SelectDefParty((uint)master.default_party_id).
                    setSuccessAction(_data =>
                    {
                        // DG0-2733 Tutorial時、StructPlayer.renew_tutorial_step == 100 に更新される 
                        UserDataAdmin.Instance.m_StructPlayer = _data.UpdateStructPlayer<RecvSelectDefParty>((PacketStructPlayer)UserDataAdmin.Instance.m_StructPlayer);
                        UserDataAdmin.Instance.m_StructHeroList = _data.GetResult<RecvSelectDefParty>().result.hero_list;
                        UserDataAdmin.Instance.ConvertPartyAssing();

                        if (finishDecision != null)
                        {
                            finishDecision();
                        }

                        m_finishDecision = true;

                        //旧チュートリアルでパーティ選択しているか
                        PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;
                        if (player.first_select_num == TutorialManager.FirstSelectNone)
                        {
                            //選択していない　[新規ユーザー]
                            TutorialManager.PP.TutorialUserType = TutorialUserType.NEW;
                        }
                        else
                        {
                            //選択している　[既存ユーザー]
                            TutorialManager.PP.TutorialUserType = TutorialUserType.ALREADY;
                        }
                        TutorialManager.PP.Save();

                        TutorialHeroSelectFSM.Instance.SendFsmPositiveEvent();
                    }).
                    setErrorAction(data =>
                    {
                        if (cancelAction != null)
                        {
                            cancelAction();
                        }
                        SoundUtil.PlaySE(SEID.SE_MENU_NG);
                        //----------------------------------------
                        // ここでエラーが発生すると
                        // シャーディングの都合上サーバー側のセーブデータが使用不可能になるらしい。
                        // 
                        // 例外対応として、UUIDを破棄して最初からゲームをやり直すようにする。
                        // ダイアログを出してボタン待ちとかやるとユーザーの操作で抜けが発生するので、
                        // そのままセーブ破棄してQuitを呼んでアプリを強制的に落とす
                        //----------------------------------------
                        DialogManager.Open1B("ERROR_MSG_USER_TITLE",
                                             "ERROR_MSG_USER",
                                             "common_button7", true, true).
                                            SetOkEvent(() =>
                                            {
                                                PacketStructPlayer player = UserDataAdmin.Instance.m_StructPlayer;
                                                if (player.first_select_num == TutorialManager.FirstSelectNone)
                                                {
                                                    //選択していない　[新規ユーザー]
                                                    LocalSaveManager.Instance.SaveFuncUUID("");
                                                }
                                                Application.Quit();
                                            });
                    }).
                    SendStart();
                }));
                newDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
                {
                    if (cancelAction != null)
                    {
                        cancelAction();
                    }
                    m_decision = false;
                    TutorialHeroSelectFSM.Instance.SendFsmNegativeEvent();
                }));
                newDialog.EnableFadePanel();
                newDialog.DisableCancelButton();
                newDialog.Show();
                TutorialHeroSelectFSM.Instance.SendFsmPositiveEvent();
            });
    }

    public void OnHeroSelectWait()
    {
        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, "tutorial_master_result_03_title");
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "tutorial_master_result_03_main");
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
            TutorialHeroSelectFSM.Instance.SendFsmNextEvent();
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    public void OnHeroSelectEnd()
    {
        Dialog nextDialog = Dialog.Create(DialogType.DialogYesNo).SetStrongYes();
        nextDialog.SetDialogTextFromTextkey(DialogTextType.Title, "tutorial_master_result_04_title");
        nextDialog.SetDialogTextFromTextkey(DialogTextType.MainText, "tutorial_master_result_04_main");
        nextDialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
        nextDialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
        nextDialog.SetDialogEvent(DialogButtonEventType.NO, new System.Action(() =>
        {
            TutorialManager.SendStep((int)TutorialStep.SCRATCH_START,
                () =>
                {
                    TutorialManager.Instance.Skip();
                });
        }));
        nextDialog.SetDialogEvent(DialogButtonEventType.YES, new System.Action(() =>
        {
            TutorialManager.SendStep((int)TutorialStep.NOMAL01_END,
                () =>
                {
                    TutorialFSM.Instance.SendFsmNextEvent();
                });
        }));
        nextDialog.EnableFadePanel();
        nextDialog.DisableCancelButton();
        nextDialog.DisableBackKey();
        nextDialog.Show();
    }

    public void setHeroName(int index)
    {
        switch (index)
        {
            case 0:
                {
                    Hero_name = GameTextUtil.GetText("tutorial_selecttext1");
                    Hero_subname = GameTextUtil.GetText("tutorial_selecttext7");
                }
                break;
            case 1:
                {
                    Hero_name = GameTextUtil.GetText("tutorial_selecttext2");
                    Hero_subname = GameTextUtil.GetText("tutorial_selecttext8");
                }
                break;
            case 2:
                {
                    Hero_name = GameTextUtil.GetText("tutorial_selecttext3");
                    Hero_subname = GameTextUtil.GetText("tutorial_selecttext9");
                }
                break;
            case 3:
                {
                    Hero_name = GameTextUtil.GetText("tutorial_selecttext4");
                    Hero_subname = GameTextUtil.GetText("tutorial_selecttext10");
                }
                break;
            case 4:
                {
                    Hero_name = GameTextUtil.GetText("tutorial_selecttext5");
                    Hero_subname = GameTextUtil.GetText("tutorial_selecttext11");
                }
                break;
            case 5:
                {
                    Hero_name = GameTextUtil.GetText("tutorial_selecttext6");
                    Hero_subname = GameTextUtil.GetText("tutorial_selecttext12");
                }
                break;
            default:
                {
                    Hero_name = "";
                    Hero_subname = "";
                }
                break;
        }
    }

    public void OnHeroSelectOut()
    {
        if (m_HeroInAnimation.isPlaying == true ||
            m_SelectHeroInAnim.isPlaying == true)
        {
            return;
        }

        setHero(m_CurrentIndex);

        TutorialHeroSelectFSM.Instance.SendFsmNextEvent();
    }

    public void OnHeroSelectIn()
    {
        if (m_HeroInAnimation.isPlaying == true ||
            m_SelectHeroInAnim.isPlaying == true)
        {
            return;
        }

        setHeroName(m_CurrentIndex);

        m_select = false;

        TutorialHeroSelectFSM.Instance.SendFsmNextEvent();
    }

    public void OnHeroSelect(int index)
    {
        if (m_decision == true)
        {
            return;
        }

        if (m_select == true)
        {
            return;
        }

        if (m_CurrentIndex == index)
        {
            return;
        }

        if (m_finishDecision == true)
        {
            return;
        }

        m_select = true;

        int fix_id = index + 1;
        string hero_name = String.Format("hero_{0:D4}", fix_id);
        string hero_perfom_name = String.Format("tex_hero_perform_l_{0:D4}", fix_id);
        string hero_mask_name = String.Format("tex_hero_perform_l_{0:D4}_mask", fix_id);
        AssetBundler.Create().Set(hero_name, hero_perfom_name, (o) =>
        {
            Sprite[] herosprites = o.AssetBundle.LoadAssetWithSubAssets<Sprite>(hero_perfom_name);
            Texture maskTextue = o.GetTexture(hero_mask_name, TextureWrapMode.Clamp);
            m_HeroImage[index] = herosprites[0];
            m_HeroImage_mask[index] = maskTextue;

            m_HeroInAnimation.Play("FreamOut");
            m_SelectHeroInAnim.Play("SelectHeroOut");
            setHeroColor(m_CurrentIndex, false);

            m_CurrentIndex = index;
            setHeroColor(m_CurrentIndex, true);
            setHeroName(-1);

            TutorialHeroSelectFSM.Instance.SendFsmNegativeEvent();
        },
        (str) =>
        {
        })
        .Load();

        SoundUtil.PlaySE(SEID.SE_MENU_OK);
    }

    private void setHero(int index)
    {
        HeroSelectListItem list = SelectDataList[index].GetComponent<HeroSelectListItem>();
        if (list != null)
        {
            m_HeroInAnimation = list.m_HeroInAnim;
            m_HeroInAnimation.Play("FreamIn");
        }

        Hero_image = m_HeroImage[index];
        Hero_image_mask = m_HeroImage_mask[index];
        m_SelectHeroInAnim.Play("SelectHeroIn");
    }

    public void setHeroColor(int index, bool active)
    {
        Color col = Color.white;
        if (active == false)
        {
            col = m_Gray;
        }

        switch (index)
        {
            case 0:
                Hero01_color = col;
                break;
            case 1:
                Hero02_color = col;
                break;
            case 2:
                Hero03_color = col;
                break;
            case 3:
                Hero04_color = col;
                break;
            case 4:
                Hero05_color = col;
                break;
            case 5:
                Hero06_color = col;
                break;
            default:
                break;
        }
    }
}
