/**
 * 	@file	UnitGridContext.cs
 *	@brief	ユニット一覧のリストアイテム用のM4uContext
 *	@author Developer
 *	@date	2016/10/31
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;

public class UnitGridContext : SortObjectBase
{
    private static string unit_status18;
    private static string uniticon_flag2;
    private static string uniticon_flag3;

    static public void ResetTextValues()
    {
        unit_status18 = GameTextUtil.GetText("unit_status18");
        uniticon_flag2 = GameTextUtil.GetText("uniticon_flag2");
        uniticon_flag3 = GameTextUtil.GetText("uniticon_flag3");
    }

    public UnitGridContext(ListItemModel model)
    {
        m_model = model;
    }

    private ListItemModel m_model = null;
    public ListItemModel model { get { return m_model; } }

    public void Copy(UnitGridContext data)
    {
        unique_id = data.UnitData.unique_id;
        CharaMasterData = data.CharaMasterData;
        UnitData = data.UnitData;
        UnitParam = data.UnitParam;
        UnitIconType = data.UnitIconType;
        UnitName = data.UnitName;
        //UnitImage = data.UnitImage;
        IsSelectedUnit = data.IsSelectedUnit;
        IsActiveLeader = data.IsActiveLeader;
        IsActivePartyAssign = data.IsActivePartyAssign;
        IsActiveFavoriteImage = data.IsActiveFavoriteImage;
        LinkMark = data.LinkMark;
        PlusPointTextColor = data.PlusPointTextColor;
        Point = data.Point;

        plus = data.Plus;
        level = data.Level;
        setParamValue();

        IsActivePoint = data.IsActivePoint;

        UpdateGameObjectName();
    }

    /// <summary>キャラ関連のマスターデータ</summary>
    public MasterDataParamChara CharaMasterData = new MasterDataParamChara();

    /// <summary>ユニット情報</summary>
    public PacketStructUnit UnitData = new PacketStructUnit();

    public UnitGridParam UnitParam = new UnitGridParam();

    public long unique_id = 0;

    public List<string> StatusTextList;

    static Color eneble_color = ColorUtil.RGB255Color(80, 80, 80);
    static Color disable_color = ColorUtil.RGB255Color(60, 60, 60);
    static Color harf_color = ColorUtil.RGB255Color(150, 150, 150);
    static Color default_color = Color.white;

    public string m_SpriteName;

    M4uProperty<MasterDataDefineLabel.UnitIconType> unitIconType = new M4uProperty<MasterDataDefineLabel.UnitIconType>(MasterDataDefineLabel.UnitIconType.NONE);
    /// <summary>アイコンのグレーアウト状態</summary>
    public MasterDataDefineLabel.UnitIconType UnitIconType
    {
        get
        {
            return unitIconType.Value;
        }
        set
        {
            switch (value)
            {
                case MasterDataDefineLabel.UnitIconType.GRAY_OUT_ENABLE_BUTTON:
                    UnitImageColor = eneble_color;
                    break;
                case MasterDataDefineLabel.UnitIconType.GRAY_OUT_DISABLE_BUTTON:
                    UnitImageColor = disable_color;
                    break;
                case MasterDataDefineLabel.UnitIconType.ALPHA_HALF_DISABLE_BUTTON:
                    UnitImageColor = harf_color;
                    break;
                default:
                    UnitImageColor = default_color;
                    break;
            }

            unitIconType.Value = value;

            m_model.isEnabled = IsEnableSelect = IsSelectable();
        }
    }

    private bool IsSelectable()
    {
        if (UnitIconType == MasterDataDefineLabel.UnitIconType.DISABLE_BUTTON ||
           UnitIconType == MasterDataDefineLabel.UnitIconType.GRAY_OUT_DISABLE_BUTTON)
        {
            return false;
        }

        return true;
    }

    M4uProperty<Sprite> unitImage = new M4uProperty<Sprite>();
    /// <summary>ユニット画像</summary>
    public Sprite UnitImage { get { return unitImage.Value; } set { unitImage.Value = value; } }


    public delegate void SpriteEventHandler(Sprite sprite);
    public event SpriteEventHandler OnIconImageUpdated;
    public void SetIconImageDirectly(Sprite sprite)
    {
        if (OnIconImageUpdated != null)
        {
            OnIconImageUpdated(sprite);
        }
        UnitImage = sprite;
    }

    public delegate void EventHandler();
    public event EventHandler OnUpdateGameObjectName;
    public void UpdateGameObjectName()
    {
        if (OnUpdateGameObjectName != null)
        {
            OnUpdateGameObjectName();
        }
    }


    M4uProperty<Color> unitImageColor = new M4uProperty<Color>(Color.white);
    /// <summary>ユニット画像の色</summary>
    Color UnitImageColor { get { return unitImageColor.Value; } set { unitImageColor.Value = value; } }

    M4uProperty<string> unitName = new M4uProperty<string>();
    /// <summary>ユニット名</summary>
    public string UnitName { get { return unitName.Value; } set { unitName.Value = value; } }

    M4uProperty<bool> isSelectedUnit = new M4uProperty<bool>();
    /// <summary>選択状態かどうか</summary>
    public bool IsSelectedUnit
    {
        get
        {
            return isSelectedUnit.Value;
        }

        set
        {
            isSelectedUnit.Value = value;
        }
    }

    M4uProperty<bool> isActiveLeader = new M4uProperty<bool>();
    /// <summary>リーダーかどうか</summary>
    public bool IsActiveLeader { get { return isActiveLeader.Value; } set { isActiveLeader.Value = value; } }

    M4uProperty<bool> isActivePartyAssign = new M4uProperty<bool>(false);
    /// <summary>パーティーに入っているかどうか</summary>
    public bool IsActivePartyAssign { get { return isActivePartyAssign.Value; } set { isActivePartyAssign.Value = value; } }

    M4uProperty<bool> isActiveFavoriteImage = new M4uProperty<bool>(false);
    /// <summary>お気に入りアイコン表示・非表示</summary>
    public bool IsActiveFavoriteImage { get { return isActiveFavoriteImage.Value; } set { isActiveFavoriteImage.Value = value; } }

    M4uProperty<Sprite> linkMark = new M4uProperty<Sprite>();
    /// <summary>リンク</summary>
    public Sprite LinkMark { get { return linkMark.Value; } set { IsActiveLinkMarkImage = (value != null); linkMark.Value = value; } }

    M4uProperty<bool> isActiveLinkMarkImage = new M4uProperty<bool>(false);
    /// <summary>リンクアイコンの表示・非表示</summary>
    private bool IsActiveLinkMarkImage { get { return isActiveLinkMarkImage.Value; } set { isActiveLinkMarkImage.Value = value; } }

    M4uProperty<Color> plusPointTextColor = new M4uProperty<Color>(ColorUtil.COLOR_LIGHT_BLUE);
    /// <summary>プラス値の色</summary>
    public Color PlusPointTextColor { get { return plusPointTextColor.Value; } set { plusPointTextColor.Value = value; } }

    M4uProperty<int> point = new M4uProperty<int>();
    /// <summary>ポイント</summary>
    public int Point { get { return point.Value; } set { point.Value = value; } }

    M4uProperty<bool> isActivePoint = new M4uProperty<bool>(false);
    /// <summary>ポイントの表示・非表示</summary>
    public bool IsActivePoint { get { return isActivePoint.Value; } set { isActivePoint.Value = value; } }

    M4uProperty<string> statusText = new M4uProperty<string>();
    /// <summary>ステータス情報</summary>
    public string StatusText { get { return statusText.Value; } set { statusText.Value = value; } }

    M4uProperty<bool> isView = new M4uProperty<bool>(true);
    public bool IsView { get { return isView.Value; } set { isView.Value = value; } }

    M4uProperty<bool> isEnableSelect = new M4uProperty<bool>(true);
    public bool IsEnableSelect { get { return isEnableSelect.Value; } set { isEnableSelect.Value = value; } }

    private uint plus = 0;
    public uint Plus
    {
        get { return plus; }
        set
        {
            //更新後にsetParamValueを呼ぶこと
            plus = value;
        }
    }
    private uint level = 0;
    public uint Level
    {
        get { return level; }
        set
        {
            //更新後にsetParamValueを呼ぶこと
            level = value;
        }
    }

    M4uProperty<string> paramValue = new M4uProperty<string>();
    public string ParamValue { get { return paramValue.Value; } set { paramValue.Value = value; } }

    /// <summary>
    /// ステータスの切り替え
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void ChangeSwitchStatus(int count)
    {
        if (StatusTextList == null || StatusTextList.Count == 0)
        {
            return;
        }

        int index = count % StatusTextList.Count;
        StatusText = StatusTextList[index];
    }

    /// <summary>
    /// ステータス表示の設定
    /// </summary>
    /// <param name="item"></param>
    /// <param name="sortType"></param>
    public void SetStatus(MAINMENU_SORT_SEQ sortType, MAINMENU_SORT_SEQ[] favoriteSortTypes)
    {
        if (CharaMasterData == null)
        {
            return;
        }

        if (UnitData == null)
        {
            return;
        }

        if (StatusTextList == null)
        {
            StatusTextList = new List<string>();
        }
        else
        {
            StatusTextList.Clear();
        }

        //-----------------------------------------
        // テキストの追加
        //-----------------------------------------
        if (sortType != MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_FAVORITE_SORT)
        {
            // 通常ソート
            AddStatusText(sortType);
        }
        else
        {
            // お好みソート
            if (favoriteSortTypes != null)
            {
                for (int i = 0; i < favoriteSortTypes.Length; i++)
                {
                    AddStatusText(favoriteSortTypes[i]);
                }
            }
        }

        if (StatusTextList.Count == 0)
        {
            // 取得できなかった場合はとりあえずレベルにする
            AddStatusText(MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL);
        }

        if (StatusTextList.Count > 0)
        {
            StatusText = StatusTextList[0];
        }
        else
        {
            StatusText = "";
        }
    }


    /// <summary>
    /// ステータスの設定：テキストリストの追加
    /// </summary>
    /// <param name="sortType"></param>
    void AddStatusText(MAINMENU_SORT_SEQ sortType)
    {
        switch (sortType)
        {
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_COST:
                StatusTextList.Add(UnitParam.cost.ToString());
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_RARE:
                StatusTextList.Add(GameTextUtil.GetSortDialogRareText(CharaMasterData.rare));
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_KIND:
                StatusTextList.Add(GameTextUtil.GetSortDialogKindText(CharaMasterData.kind));
                if (CharaMasterData.sub_kind == MasterDataDefineLabel.KindType.NONE)
                {
                    StatusTextList.Add(GameTextUtil.GetSortDialogKindText(CharaMasterData.kind));
                }
                else
                {
                    StatusTextList.Add(GameTextUtil.GetSortDialogKindText(CharaMasterData.sub_kind));
                }
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_HP:
                StatusTextList.Add(UnitParam.hp.ToString());
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ATTACK:
                StatusTextList.Add(UnitParam.pow.ToString());
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_ID:
                StatusTextList.Add(string.Format(GameTextUtil.GetText("unit_status1"), CharaMasterData.draw_id));
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LIMIT_OVER:
                StatusTextList.Add(UnitData.limitover_lv.ToString());
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_CHARM:
                StatusTextList.Add(string.Format("{0:0.0}", UnitParam.charm));
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_PLUS:
                StatusTextList.Add(string.Format("{0}", UnitData.add_hp + UnitData.add_pow));
                break;
            case MAINMENU_SORT_SEQ.SEQ_SORT_TYPE_LEVEL:
                if (CharaMasterData != null)
                {
                    if (UnitData.level >= CharaMasterData.level_max)
                    {
                        StatusTextList.Add(unit_status18);
                    }
                    else
                    {
                        StatusTextList.Add(UnitData.level.ToString());
                    }
                }
                else
                {
                    StatusTextList.Add(UnitData.level.ToString());
                }
                break;
        }
    }


    public void SetUnitParam(PacketStructUnit unit_data, MasterDataParamChara chara_master)
    {
        UnitParam = Array.Find(UserDataAdmin.Instance.m_UnitGridParamList, (v) => v.unique_id == unit_data.unique_id);
        if (UnitParam == null)
        {
            UnitParam = new UnitGridParam(unit_data, chara_master);
        }

        setSortParamUnit(UnitParam);
    }

    public override void setSortParamUnit(UnitGridParam unit_param)
    {
        base.setSortParamUnit(unit_param);
        UnitParam = unit_param;
    }

    public void updateSortParam()
    {
        base.setSortParamUnit(UnitParam);
    }

    public void setParamValue()
    {
        string str_level = (level >= CharaMasterData.level_max) ?
                            unit_status18 :
                            string.Format(uniticon_flag2, level); // レベル
        if (plus != 0)
        {
            str_level = str_level + string.Format(uniticon_flag3, plus);
        }

        ParamValue = str_level;
    }
}