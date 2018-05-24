/**
 *  @file   M4uButtonColorBlockBinding.cs
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
    [AddComponentMenu("M4u/ButtonColorBlockBinding")]
    public class M4uButtonColorBlockBinding : M4uBindingSingle
    {
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

            ui.colors = (ColorBlock)Values[0];
        }

        public override string ToString()
        {
            return "Button.colors=" + GetBindStr(Path);
        }
    }
}

