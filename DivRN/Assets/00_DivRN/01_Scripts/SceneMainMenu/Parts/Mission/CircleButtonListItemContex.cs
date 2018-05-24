using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;


public class CircleButtonListItemContex : M4uContext
{
    public Action<CircleButtonListItemContex> DidSelectItem = delegate { };

    //    public int DataType { get; set; }
    public int Id { get; set; }
    public uint UnitId { get; set; }

    // アイコンイメージ
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    // 名称
    protected M4uProperty<string> nameText = new M4uProperty<string>("");
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    // 上部中央の文字列
    protected M4uProperty<string> captionText = new M4uProperty<string>("");
    public string CaptionText { get { return captionText.Value; } set { captionText.Value = value; } }

    // 右下の文字列
    protected M4uProperty<int> count = new M4uProperty<int>(0);
    public int Count { get { return count.Value; } set { count.Value = value; } }

    // アイテムの消費個数
    protected M4uProperty<int> usedCount = new M4uProperty<int>(0);
    public int UsedCount { get { return usedCount.Value; } set { usedCount.Value = value; } }
    //private CircleButtonListItemContex(){}

    M4uProperty<bool> isEnableSelect = new M4uProperty<bool>(true);
    public bool IsEnableSelect { get { return isEnableSelect.Value; } set { isEnableSelect.Value = value; } }


    // ボタンフィードバック
    public void OnClickedButton()
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        // カタログ表示
        MainMenuManager.Instance.OpenUnitDetailInfoCatalog(UnitId);
    }

    public CircleButtonListItemContex(int id, uint unit_id)
    {
        Id = id;
        UnitId = unit_id;
        UnitIconImageProvider.Instance.Get(
            UnitId,
            sprite =>
            {
                IconImage = sprite;
            });
    }
}
