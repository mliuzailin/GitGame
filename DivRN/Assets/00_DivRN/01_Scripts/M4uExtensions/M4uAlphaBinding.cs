/**
 *  @file   M4uAlphaBinding.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/09
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace M4u
{
    /// <summary>
    /// M4uAlphaBinding. Bind ColorAlpha
    /// </summary>
    [AddComponentMenu("M4u/AlphaBinding")]
    public class M4uAlphaBinding : M4uBindingSingle
    {
        private Transform ui = null;

        public override void Start()
        {
            base.Start();

            ui = transform;
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            SetColor(ui, (float)Values[0]);
        }

        private void SetColor(Transform t, float alpha)
        {
            var g = t.GetComponent<Graphic>();
            if (g != null)
            {
                g.color = new Color(g.color.r, g.color.g, g.color.b, alpha);
            }

            for (int i = 0; i < t.childCount; i++)
            {
                SetColor(t.GetChild(i), alpha);
            }
        }

        public override string ToString()
        {
            return "Graphic.color.a=" + GetBindStr(Path);
        }
    }
}