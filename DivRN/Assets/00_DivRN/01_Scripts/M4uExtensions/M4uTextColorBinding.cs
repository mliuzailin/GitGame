//----------------------------------------------
// MVVM 4 uGUI
// © 2015 yedo-factory
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace M4u
{
    /// <summary>
    /// TextColorBinding. Bind TextColor
    /// </summary>
	[AddComponentMenu("M4u/TextColorBinding")]
    public class M4uTextColorBinding : M4uBindingSingle
    {
        private Text ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<Text>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            ui.color = (Color)Values[0];
        }

        public override string ToString()
        {
            return "Text.color=" + GetBindStr(Path);
        }
    }
}