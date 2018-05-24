//----------------------------------------------
// FillAmountBinding
//   M4U 追加コンポーネント 
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M4u
{
    /// <summary>
    /// FillAmountBinding. Bind Image.fillAmount
    /// </summary>
	[AddComponentMenu("M4u/FillAmountBinding")]
    public class FillAmountBinding : M4uBindingSingle
    {
        private Image image = null;

        public override void Start()
        {
            base.Start();

            image = GetComponent<Image>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();
            float value = float.Parse(Values[0].ToString());
            image.fillAmount = value;
        }

        public override string ToString()
        {
            return "Image.fillAmount=" + GetBindStr(Path);
        }
    }
}
