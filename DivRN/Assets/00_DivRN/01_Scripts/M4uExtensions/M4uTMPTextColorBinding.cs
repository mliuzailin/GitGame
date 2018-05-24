//----------------------------------------------
// MVVM 4 uGUI
// © 2015 yedo-factory
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace M4u
{
    /// <summary>
    /// TMPTextColorBinding. Bind TextColor
    /// </summary>
    [AddComponentMenu("M4u/TMPTextColorBinding")]
    public class M4uTMPTextColorBinding : M4uBindingSingle
    {
        private TMP_Text ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<TMP_Text>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            ui.color = (Color)Values[0];
        }

        public override string ToString()
        {
            return "TMPText.color=" + GetBindStr(Path);
        }
    }
}