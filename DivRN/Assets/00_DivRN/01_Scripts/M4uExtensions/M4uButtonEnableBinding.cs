using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M4u
{
    [AddComponentMenu("M4u/ButtonEnableBinding")]
    public class M4uButtonEnableBinding : M4uBindingSingle
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

            ui.interactable = (bool)Values[0];
        }

        public override string ToString()
        {
            return "Button.interactable=" + GetBindStr(Path);
        }
    }
}