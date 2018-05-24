using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ScratchPanel : M4uContextMonoBehaviour
{
    public enum PanelSeq
    {
        SEQ_NONE = 0,
        SEQ_SELECT,
        SEQ_OPEN,
    };

    M4uProperty<Sprite> frontImage = new M4uProperty<Sprite>();
    public Sprite FrontImage { get { return frontImage.Value; } set { frontImage.Value = value; } }

    M4uProperty<Sprite> backImage = new M4uProperty<Sprite>();
    public Sprite BackImage { get { return backImage.Value; } set { backImage.Value = value; } }

    M4uProperty<bool> isViewPlus = new M4uProperty<bool>();
    public bool IsViewPlus { get { return isViewPlus.Value; } set { isViewPlus.Value = value; } }

    public AnimationClipScratch animClipScrath { get; private set; }

    public PanelSeq panelSeq = PanelSeq.SEQ_NONE;

    // 0 1 2
    // 3 4 5
    // 6 7 8
    public int Index = -1;
    public bool IsCenter()
    {
        return Index == 4;
    }

    public int touchIndex = -1;

    public System.Action<ScratchPanel> DidSelectPanel = delegate { };

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        animClipScrath = GetComponentInChildren<AnimationClipScratch>();
    }

    public void OnSelectPanel()
    {
        DidSelectPanel(this);
    }
}
