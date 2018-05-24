//----------------------------------------------
// MVVM 4 uGUI
// © 2015 yedo-factory
//----------------------------------------------
using UnityEngine;
using TMPro;

namespace M4u
{
    /// <summary>
    /// M4uTextBindings. Bind Text
    /// </summary>
	[AddComponentMenu("M4u/TMPTextBindings")]
    public class M4uTMPTextBindings : M4uBindingMultiple
    {
        public string Format = "";

        private TextMeshProUGUI ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<TextMeshProUGUI>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            ui.text = string.Format(Format, Values);
        }

        public override string ToString()
        {
            string str = "TMPText.text=";
            if (Path != null && Path.Length > 0)
            {
                str += string.Format(Format, GetBindStrs(Path));
            }
            return str;
        }
    }
}