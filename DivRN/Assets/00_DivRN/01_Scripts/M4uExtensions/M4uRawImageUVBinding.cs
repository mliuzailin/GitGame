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
    /// M4uRawImageBinding. Bind RawImage
    /// </summary>
	[AddComponentMenu("M4u/RawImageUVBinding")]
    public class M4uRawImageUVBinding : M4uBindingSingle
    {
        private RawImage ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<RawImage>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();


            ui.uvRect = new Rect((Rect)Values[0]);
        }

        public override string ToString()
        {
            return "RawImage.uvRect=" + GetBindStr(Path);
        }
    }
}