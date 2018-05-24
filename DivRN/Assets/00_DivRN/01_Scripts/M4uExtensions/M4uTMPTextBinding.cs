//----------------------------------------------
// MVVM 4 uGUI
// © 2015 yedo-factory
//----------------------------------------------
using TMPro;
using UnityEngine;

namespace M4u
{
    /// <summary>
    /// M4uTMPTextBinding. Bind Text
    /// </summary>
    [AddComponentMenu("M4u/TMPTextBinding")]
    public class M4uTMPTextBinding : M4uBindingSingle
    {
        public string Format = "";

        public bool NewLine = false;

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
            if (ui != null)
            {
                ui.text = string.Format(Format, Values[0]);

                // Newlineがtrueの場合は、改行が有効
                ui.text = NewLine ? ui.text + "\n" : ui.text;
            }
            else
            {
                Debug.LogError("M4uTMPTextBinding ui == null: " + string.Format(Format, Values[0]));
            }
        }

        public override string ToString()
        {
            return "TMPText.text=" + string.Format(Format, GetBindStr(Path));
        }
    }
}