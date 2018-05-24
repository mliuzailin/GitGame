using M4u;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LineUpListItemContex : SortObjectBase
{
    public Action<LineUpListItemContex> DidSelectItem = delegate { };

    // ボタンID
    public int Id { get; set; }

    // ユニットID
    public int UnitId { get; set; }

    // ユニットアイコン画像 UnitIdから該当する画像を表示
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    // 名称 UnitIdから該当する画像を表示
    M4uProperty<string> nameText = new M4uProperty<string>("");
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    // レアリボン表示（/RARE/）
    M4uProperty<bool> rareFlag = new M4uProperty<bool>(false);
    public bool RareFlag { get { return rareFlag.Value; } set { rareFlag.Value = value; } }

    // レア表示（★） UnitIdから該当する画像を表示
    M4uProperty<int> rareStar = new M4uProperty<int>();
    public int RareStar { get { return rareStar.Value; } set { rareStar.Value = value; } }

    // 確率　超絶○○○UPの元情報
    M4uProperty<int> ratio = new M4uProperty<int>(0);
    public int Ratio { get { return ratio.Value; } set { ratio.Value = value; } }

    // 超絶○○○UP画像 Ratioから該当する画像を表示
    M4uProperty<Sprite> ratioImage = new M4uProperty<Sprite>();
    public Sprite RatioImage { get { return ratioImage.Value; } private set { ratioImage.Value = value; } }

    // 確率　表記
    M4uProperty<string> ratioText = new M4uProperty<string>("");
    public string RatioText { get { return ratioText.Value; } set { ratioText.Value = value; } }

    // ボタンフィードバック
    public void OnClickedButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClick Button buttonId:" + Id + " UnitId:" + UnitId);
#endif
    }
    public LineUpListItemContex(LineUpListItemContex _data)
    {
        Copy(_data);
    }

    public LineUpListItemContex(int id, ServerDataDefine.PacketStructGachaLineup lineUp)
    {
        Id = id;
        UnitId = lineUp.id;
        Ratio = lineUp.rate_up_icon;
        RatioText = lineUp.rate.Length > 0
                        ? String.Format(GameTextUtil.GetText("Gacha_step_06"), lineUp.rate)
                        : "";

        // レアリボン表示フラグ
        RareFlag = lineUp.limit_icon != 0 ? true : false;

        MasterDataParamChara master = MasterFinder<MasterDataParamChara>.Instance.Find((int)UnitId);

        // レア数
        RareStar = (int)master.rare + 1;

        // ユニット名
        string template = "{0}";
        template = lineUp.limit_icon != 0
                        ? GameTextUtil.GetText("scratch_rare01")
                        : GameTextUtil.GetText("scratch_normal01");
        NameText = String.Format(template, master.name);

        IconImage = null;

        RatioImage = null;

        setSortParamLineUp(lineUp, master);
    }

    public void setupResource()
    {
        // ユニット画像取得
        UnitIconImageProvider.Instance.Get(
            (uint)UnitId,
            sprite =>
            {
                IconImage = sprite;
            });

        // 超絶○UP
        string result = ratioSpriteName();
        if (result.Length > 0)
        {
            RatioImage = ResourceManager.Instance.Load(result);
        }
    }

    // 超絶UP画像の取得
    // アトラス整理後に改修予定
    private string ratioSpriteName()
    {
        string result = "";

        if (Ratio >= 1 && Ratio <= 15)
        {
            result = string.Format("cyo-{0}", Ratio);
        }

        return result;
    }

    public void Copy(LineUpListItemContex _data)
    {
        Id = _data.Id;
        UnitId = _data.UnitId;
        Ratio = _data.Ratio;
        RareFlag = _data.RareFlag;
        RareStar = _data.RareStar;
        NameText = _data.NameText;
        RatioText = _data.RatioText;
    }
}
