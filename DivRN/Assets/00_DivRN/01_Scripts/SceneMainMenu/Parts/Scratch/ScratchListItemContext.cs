using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class ScratchListItemContext : M4uContext
{
    public Image m_BoadImage = null;
    public ScratchStepButton m_StepButton = null;

    // スクラッチ可能回数の最大値
    const int ScratchMax = 9;

    public Toggle Toggle = null;

    // 開催期間　※販売期間とは別
    M4uProperty<string> dateText = new M4uProperty<string>("");
    public string DateText { get { return dateText.Value; } set { dateText.Value = value; } }

    //
    M4uProperty<bool> isLineUpNormal = new M4uProperty<bool>(false);
    bool IsLineUpNormal { get { return isLineUpNormal.Value; } set { isLineUpNormal.Value = value; } }

    //
    M4uProperty<bool> isLineUpA = new M4uProperty<bool>(false);
    bool IsLineUpA { get { return isLineUpA.Value; } set { isLineUpA.Value = value; } }

    //
    M4uProperty<bool> isLineUpB = new M4uProperty<bool>(false);
    bool IsLineUpB { get { return isLineUpB.Value; } set { isLineUpB.Value = value; } }

    //
    M4uProperty<bool> isLineUpRainbow = new M4uProperty<bool>(false);
    bool IsLineUpRainbow { get { return isLineUpRainbow.Value; } set { isLineUpRainbow.Value = value; } }

    M4uProperty<bool> isInfo = new M4uProperty<bool>();
    public bool IsInfo { get { return isInfo.Value; } set { isInfo.Value = value; } }

    // チップ使用フラグ
    M4uProperty<bool> isUsedTip = new M4uProperty<bool>(false);
    public bool IsUsedTip { get { return isUsedTip.Value; } set { isUsedTip.Value = value; } }

    // ポイント
    M4uProperty<string> pointText = new M4uProperty<string>();
    public string PointText { get { return pointText.Value; } set { pointText.Value = value; } }

    M4uProperty<bool> isViewMaxButton = new M4uProperty<bool>();
    public bool IsViewMaxButton { get { return isViewMaxButton.Value; } set { isViewMaxButton.Value = value; } }

    M4uProperty<Sprite> maxButtonImage = new M4uProperty<Sprite>();
    public Sprite MaxButtonImage { get { return maxButtonImage.Value; } set { maxButtonImage.Value = value; } }

    M4uProperty<Sprite> maxNumImage = new M4uProperty<Sprite>();
    public Sprite MaxNumImage { get { return maxNumImage.Value; } set { maxNumImage.Value = value; } }

    M4uProperty<bool> isViewOneButton = new M4uProperty<bool>(false);
    public bool IsViewOneButton { get { return isViewOneButton.Value; } set { isViewOneButton.Value = value; } }

    M4uProperty<Sprite> oneButtonImage = new M4uProperty<Sprite>();
    public Sprite OneButtonImage { get { return oneButtonImage.Value; } set { oneButtonImage.Value = value; } }

    M4uProperty<bool> isViewMaxNum = new M4uProperty<bool>(false);
    public bool IsViewMaxNum { get { return isViewMaxNum.Value; } set { isViewMaxNum.Value = value; } }

    M4uProperty<bool> isViewStepUpButton = new M4uProperty<bool>(false);
    public bool IsViewStepUpButton { get { return isViewStepUpButton.Value; } set { isViewStepUpButton.Value = value; } }

    M4uProperty<string> bonusText = new M4uProperty<string>();
    public string BonusText { get { return bonusText.Value; } set { bonusText.Value = value; } }

    M4uProperty<bool> isViewBonusLabel = new M4uProperty<bool>(false);
    public bool IsViewBonusLabel { get { return isViewBonusLabel.Value; } set { isViewBonusLabel.Value = value; } }

    M4uProperty<Sprite> boadImage = new M4uProperty<Sprite>();
    public Sprite BoadImage { get { return boadImage.Value; } set { boadImage.Value = value; } }

    M4uProperty<float> boadHeight = new M4uProperty<float>(300);
    public float BoadHeight { get { return boadHeight.Value; } set { boadHeight.Value = value; } }

    M4uProperty<float> boadAlpha = new M4uProperty<float>(0);
    public float BoadAlpha { get { return boadAlpha.Value; } set { boadAlpha.Value = value; } }

    private MasterDataGacha m_Master = null;
    private MasterDataGachaGroup m_GroupMaster = null;
    private MasterDataStepUpGacha m_StepMaster = null;
    private MasterDataStepUpGachaManage m_StepManageMaster = null;
    private uint[] m_LineupMaster = new uint[(int)Scratch.LineUp.Max];
    private string m_DetailText = "";
    public MasterDataGacha gachaMaster { get { return m_Master; } }
    public uint[] lineupMaster { get { return m_LineupMaster; } }
    public string detailText { get { return m_DetailText; } }
    public bool m_IsMoveScratchResult = false;

    public void setup(MasterDataGacha _master)
    {
        m_IsMoveScratchResult = false;
        m_Master = _master;
        m_GroupMaster = MasterFinder<MasterDataGachaGroup>.Instance.Find((int)m_Master.gacha_group_id);

        if (m_Master.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            m_StepMaster = MasterDataUtil.GetMasterDataStepUpGachaFromGachaID(m_Master.fix_id);
            m_StepManageMaster = MasterDataUtil.GetCurrentStepUpGachaManageMaster(m_Master.fix_id);
        }

        uint startTimeU = 0;
        uint endTimeU = 0;
        if (m_GroupMaster != null)
        {
            startTimeU = m_GroupMaster.sale_period_start;
            endTimeU = m_GroupMaster.sale_period_end;
        }
        else
        {
            startTimeU = m_Master.timing_start;
            endTimeU = m_Master.timing_end;
        }

        DateText = "";
        if (endTimeU != 0)
        {
            DateTime startTime = TimeUtil.GetDateTime(startTimeU);
            DateTime endTime = TimeUtil.GetDateTime(endTimeU);
            endTime = endTime.SubtractAMinute();
            // 看板テキスト
            string kikanFormat = GameTextUtil.GetText("scratch_display1");
            DateText = string.Format(kikanFormat, startTime.ToString("yyyy/MM/dd(HH:mm)"), endTime.ToString("yyyy/MM/dd(HH:mm)"));
        }

        // 看板イメージ
        BoadImage = null;
        BoadAlpha = 0;
        if (m_Master.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            // ステップアップスクラッチの場合は看板サイズ変更
            BoadHeight = 234;
        }

        // ラインナップボタンの設定
        setupLineupButton();

        // 詳細ボタンの表示・非表示
        m_DetailText = "";
        if (m_GroupMaster != null &&
            m_Master.type != MasterDataDefineLabel.GachaType.STEP_UP)
        {
            m_DetailText = MasterDataUtil.GetGachaText(EMASTERDATA_SERVER.GACHA_GROUP,
                                                        m_Master.gacha_group_id,
                                                        MasterDataDefineLabel.GachaTextRefType.DETAIL);
        }

        if (m_DetailText.IsNullOrEmpty() == false ||
            m_Master.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            IsInfo = true;
        }
        else
        {
            IsInfo = false;
        }

        IsUsedTip = false;
        IsViewMaxButton = true;

        updatePointText();

        // おまけの表示
        if (m_Master.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            IsViewBonusLabel = (m_StepManageMaster.present_enable == MasterDataDefineLabel.BoolType.ENABLE);
            string presentName = "";
            int presentNum = 0;
            MasterDataPresent[] presentArray = MasterDataUtil.GetPresentMasterFromGroupID(m_StepManageMaster.present_group_id);
            if (presentArray.IsNullOrEmpty() == false)
            {
                presentName = MasterDataUtil.GetPresentName(presentArray[0]);
                presentNum = MasterDataUtil.GetPresentCount(presentArray[0]);
            }

            string present_message = "「" + presentName + "」" + " ×" + presentNum;
            BonusText = string.Format(GameTextUtil.GetText("Gacha_Bonus"), present_message);
        }

        LoadBoadImage();
    }

    void setupLineupButton()
    {
        // 通常ガチャ判定
        m_LineupMaster[(int)Scratch.LineUp.Normal] = m_Master.fix_id;
        m_LineupMaster[(int)Scratch.LineUp.Rainbow] = (uint)m_Master.rainbow_decide;
        m_LineupMaster[(int)Scratch.LineUp.A] = 0;
        m_LineupMaster[(int)Scratch.LineUp.B] = 0;

        //前後半ガチャ判定
        MasterDataGacha _secondMaster = MasterDataUtil.GetGroupGachaMaster(m_Master);
        if (_secondMaster != null)
        {
            DateTime startA = TimeUtil.GetDateTime(m_Master.timing_start);
            DateTime startB = TimeUtil.GetDateTime(_secondMaster.timing_start);
            if (startA < startB)
            {
                m_LineupMaster[(int)Scratch.LineUp.A] = m_Master.fix_id;
                m_LineupMaster[(int)Scratch.LineUp.B] = _secondMaster.fix_id;
            }
            else
            {
                m_LineupMaster[(int)Scratch.LineUp.A] = _secondMaster.fix_id;
                m_LineupMaster[(int)Scratch.LineUp.B] = m_Master.fix_id;
            }

            m_LineupMaster[(int)Scratch.LineUp.Normal] = 0; // 通常ラインナップを削除
        }

        //ステップアップガチャ判定
        if (m_Master.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            m_LineupMaster[(int)Scratch.LineUp.Normal] = m_StepManageMaster.normal1_assign_id;
            m_LineupMaster[(int)Scratch.LineUp.Rainbow] = m_StepManageMaster.special_assign_id;
        }

        // ボタンの表示設定
        IsLineUpNormal = (m_LineupMaster[(int)Scratch.LineUp.Normal] > 0);
        IsLineUpA = (m_LineupMaster[(int)Scratch.LineUp.A] > 0);
        IsLineUpB = (m_LineupMaster[(int)Scratch.LineUp.B] > 0);
        IsLineUpRainbow = (m_LineupMaster[(int)Scratch.LineUp.Rainbow] > 0);
    }

    public void uodateGachaRainbowDecide()
    {
        //虹確定アサインID更新
        MainMenuParam.m_GachaRainbowDecide = (IsLineUpRainbow == true) ? m_LineupMaster[(int)Scratch.LineUp.Rainbow] : 0;
    }

    public void updateScratchButton()
    {
        if (m_Master == null)
        {
            return;
        }

        if (m_Master.type == MasterDataDefineLabel.GachaType.STEP_UP)
        {
            IsViewStepUpButton = true;
            IsViewMaxButton = false;
            IsViewOneButton = false;
            m_StepButton.SetStepImage(m_StepManageMaster.step_num);
            m_StepButton.SetLotImage(m_StepManageMaster.total_lot_exec);
        }
        else
        {
            IsViewStepUpButton = false;
            IsViewOneButton = true;

            int count = (int)MasterDataUtil.GetGachaCountFromMaster(m_Master);
            // 連続でめくるの非表示設定
            if (m_Master.type == MasterDataDefineLabel.GachaType.TUTORIAL)
            {
                IsViewMaxButton = false;
            }
            else if (m_Master.type == MasterDataDefineLabel.GachaType.ITEM_POINT)
            {
                if (count <= 1)
                {
                    IsViewMaxButton = false;
                }
            }

            //初回無料チェック
            if (MasterDataUtil.IsFirstTimeFree(m_Master))
            {
                OneButtonImage = ResourceManager.Instance.Load("otameshi");
            }
            else
            {
                OneButtonImage = ResourceManager.Instance.Load("btn_once");
            }

            if (IsViewMaxButton == false)
            {
                return;
            }

            //
            if (count <= 1)
            {
                MaxButtonImage = ResourceManager.Instance.Load("btn_continuously");
                IsViewMaxNum = false;
                MaxNumImage = null;
            }
            else
            {
                MaxButtonImage = ResourceManager.Instance.Load("btn_continuously2");
                IsViewMaxNum = true;
                MaxNumImage = ResourceManager.Instance.Load("btn_num_" + count.ToString());
            }
        }
    }

    public void updatePointText()
    {
        if (m_Master == null)
        {
            return;
        }

        string pointFormat = GameTextUtil.GetText("scratch_pt_text");
        switch (m_Master.type)
        {
            case MasterDataDefineLabel.GachaType.NONE:
                //Money使用
                break;
            case MasterDataDefineLabel.GachaType.RARE:
            case MasterDataDefineLabel.GachaType.EVENT:
            case MasterDataDefineLabel.GachaType.TUTORIAL:
                {
                    // チップ使用
                    IsUsedTip = true;

                    if (MasterDataUtil.IsFirstTimeFree(m_Master))
                    {
                        //初回無料
                        PointText = GameTextUtil.GetText("gacha_first free");
                        IsViewMaxButton = false;
                    }
                    else
                    {
                        string name = "";
                        uint point = 0;
                        if (m_Master.paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                        {
                            //有料のみ
                            name = GameTextUtil.GetText("common_text13");
                            point = UserDataAdmin.Instance.m_StructPlayer.have_stone_pay;
                            IsViewMaxButton = false;
                        }
                        else
                        {
                            name = GameTextUtil.GetText("common_text6");
                            point = UserDataAdmin.Instance.m_StructPlayer.have_stone;
                        }
                        PointText = string.Format(pointFormat, name, m_Master.price, point);
                    }
                }
                break;
            case MasterDataDefineLabel.GachaType.NORMAL:
                {
                    // フレンドポイント
                    string name = GameTextUtil.GetText("common_text1");
                    uint point = UserDataAdmin.Instance.m_StructPlayer.have_friend_pt;
                    PointText = string.Format(pointFormat, name, m_Master.price, point);
                }
                break;
            case MasterDataDefineLabel.GachaType.LUNCH:
            case MasterDataDefineLabel.GachaType.TICKET:
            case MasterDataDefineLabel.GachaType.EVENT_POINT:
                break;
            case MasterDataDefineLabel.GachaType.ITEM_POINT:
                {
                    //  アイテムポイント使用
                    string name = MasterDataUtil.GetItemNameFromGachaMaster(m_Master);
                    uint point = UserDataAdmin.Instance.GetItemPoint(m_Master.cost_item_id);
                    PointText = string.Format(pointFormat, name, m_Master.price, point);
                }
                break;
            case MasterDataDefineLabel.GachaType.STEP_UP:
                {
                    // チップ使用
                    IsUsedTip = true;
                    string name = "";
                    uint point = 0;
                    if (m_StepMaster.paid_tip_only == MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        //有料のみ
                        name = GameTextUtil.GetText("common_text13");
                        point = UserDataAdmin.Instance.m_StructPlayer.have_stone_pay;
                        IsViewMaxButton = false;
                    }
                    else
                    {
                        name = GameTextUtil.GetText("common_text6");
                        point = UserDataAdmin.Instance.m_StructPlayer.have_stone;
                    }
                    PointText = string.Format(pointFormat, name, m_StepManageMaster.price, point);
                }
                break;
            default:
                break;
        }
    }

    public void LoadBoadImage()
    {
        if (m_Master != null)
        {
            string url = "";

            if (m_Master.type == MasterDataDefineLabel.GachaType.STEP_UP)
            {
                url = string.Format(GlobalDefine.GetScratchBoadhUrl(), m_StepManageMaster.url_img);
            }
            else
            {
                if (m_Master.url_img == string.Empty)
                {
                    url = string.Format(GlobalDefine.GetScratchBoadhUrl(), m_Master.fix_id);
                }
                else
                {
                    url = string.Format(GlobalDefine.GetScratchBoadhUrl(), m_Master.url_img);
                }
            }

            WebResource.Instance.GetSprite(url,
                        (Sprite sprite) =>
                        {
                            BoadImage = sprite;
                            BoadAlpha = 1;
                        },
                        () =>
                        {
                            BoadImage = ResourceManager.Instance.Load("dummy_scratch_banner");
                            BoadAlpha = 1;
                        });

        }
    }
}
