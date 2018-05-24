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
    /// ImageMaterialBinding. Bind ImageMaterial
    /// </summary>
	[AddComponentMenu("M4u/ImageMaterialBinding")]
    public class M4uImageMaterialBinding : M4uBindingSingle
    {
        private Image ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<Image>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            ui.material = (Material)Values[0];
        }

        public override string ToString()
        {
            return "Image.material=" + GetBindStr(Path);
        }
    }
}