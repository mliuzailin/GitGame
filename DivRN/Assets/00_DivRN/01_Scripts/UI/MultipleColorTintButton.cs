/**
 * 	@file	MultipleColorTintButton.cs
 *	@brief	すべてのグラフィックのカラーをColorTintに合わせる
 *	@author Developer
 *	@date	
 */

using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[RequireComponent(typeof(Image))]
public class MultipleColorTintButton : Button
{
    [SerializeField]
    Graphic[] TargetGraphics;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if (transition != Transition.ColorTint) { return; }

        Color color;
        switch (state)
        {
            case SelectionState.Normal:
                color = colors.normalColor;
                break;
            case SelectionState.Highlighted:
                color = colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                color = colors.pressedColor;
                break;
            case SelectionState.Disabled:
                color = colors.disabledColor;
                break;
            default:
                color = Color.black;
                break;
        }
        if (gameObject.activeInHierarchy)
        {
            switch (transition)
            {
                case Transition.ColorTint:
                    ColorTween(color * colors.colorMultiplier, instant);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    private void ColorTween(Color targetColor, bool instant)
    {
        //if (targetGraphic == null) { return; }
        if (TargetGraphics == null) { return; }
        foreach (Graphic g in TargetGraphics)
        {
            if (g == null) { continue; }
            g.CrossFadeColor(targetColor, (!instant) ? colors.fadeDuration : 0f, true, true);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MultipleColorTintButton))]
    public class MultipleColorTintButtonEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif

}