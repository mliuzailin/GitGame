/**
 *  @file   M4uButtonColorBinding.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/16
 */
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace M4u
{
    [AddComponentMenu("M4u/ButtonColorBinding")]
    public class M4uButtonColorBinding : M4uBindingSingle
    {
        public enum SelectionState
        {
            Normal = 0,
            Highlighted = 1,
            Pressed = 2,
            Disabled = 3
        }

        public SelectionState Type = SelectionState.Normal;
        private Button ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<Button>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            var value = (Color)Values[0];
            var tmp = ui.colors;
            switch (Type)
            {
                case SelectionState.Normal:
                    tmp.normalColor = value;
                    break;
                case SelectionState.Highlighted:
                    tmp.highlightedColor = value;
                    break;
                case SelectionState.Pressed:
                    tmp.pressedColor = value;
                    break;
                case SelectionState.Disabled:
                    tmp.disabledColor = value;
                    break;
            }

            ui.colors = tmp;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case SelectionState.Normal:
                    return "Button.colors.normalColor=" + GetBindStr(Path);
                case SelectionState.Highlighted:
                    return "Button.colors.highlightedColor=" + GetBindStr(Path);
                case SelectionState.Pressed:
                    return "Button.colors.pressedColor=" + GetBindStr(Path);
                case SelectionState.Disabled:
                    return "Button.colors.disabledColor=" + GetBindStr(Path);
            }
            return "";
        }
    }
}

