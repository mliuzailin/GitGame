/**
 *  @file   M4uCanvasGroupAlphaBinding.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/21
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace M4u
{
    /// <summary>
    /// M4uImageBinding. Bind Image
    /// </summary>
    [AddComponentMenu("M4u/CanvasGroupAlphaBinding")]
    public class M4uCanvasGroupAlphaBinding : M4uBindingSingle
    {
        private CanvasGroup ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<CanvasGroup>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            ui.alpha = (float)Values[0];
        }

        public override string ToString()
        {
            return "CanvasGroup.alpha=" + GetBindStr(Path);
        }
    }
}