using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using M4u;


public class FooterSubMenuItem : M4uContext
{
    public FooterSubMenuItem(ListItemModel model)
    {
        m_model = model;
    }

    private ListItemModel m_model = null;
    public ListItemModel model { get { return m_model; } }

    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage { get { return iconImage.Value; } set { iconImage.Value = value; } }

    M4uProperty<Sprite> textImage = new M4uProperty<Sprite>();
    public Sprite TextImage { get { return textImage.Value; } set { textImage.Value = value; } }

    M4uProperty<bool> isViewFlag = new M4uProperty<bool>();
    public bool IsViewFlag { get { return isViewFlag.Value; } set { isViewFlag.Value = value; } }

	M4uProperty<Sprite> flagImage = new M4uProperty<Sprite>();
	public Sprite FlagImage { get { return flagImage.Value; } set { flagImage.Value = value; } }

	M4uProperty<string> flagRate = new M4uProperty<string>();
	public string FlagRate { get { return flagRate.Value; } set { flagRate.Value = value; } }

	public MAINMENU_BUTTON buttonType = MAINMENU_BUTTON.NONE;
    public MAINMENU_SEQ switchSeqType = MAINMENU_SEQ.SEQ_NONE;

    public Action<FooterSubMenuItem> DelSelectSubMenu = delegate { };
}