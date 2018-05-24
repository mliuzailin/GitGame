/**
 *  @file   M4uSelectableEnableBinding.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/06/19
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M4u
{
    [AddComponentMenu("M4u/SelectableEnableBinding")]
    public class M4uSelectableEnableBinding : M4uBindingSingle
    {

        private Selectable ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<Selectable>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            ui.interactable = (bool)Values[0];
        }

        public override string ToString()
        {
            return "Selectable.interactable=" + GetBindStr(Path);
        }
    }
}