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
    /// M4uTextBinding. Bind Text
    /// </summary>
	[AddComponentMenu("M4u/TextBinding")]
	public class M4uTextBinding : M4uBindingSingle
	{
		public string Format = "";

        public bool NewLine = false;

		private Text ui = null;

		public override void Start ()
		{
			base.Start ();

			ui = GetComponent<Text> ();
			OnChange ();
		}

		public override void OnChange ()
		{
			base.OnChange ();
			if(ui != null)
			{
	            ui.text = string.Format (Format, Values[0]);

    	        // Newlineがtrueの場合は、改行が有効
        	    ui.text = NewLine ? ui.text + "\n" : ui.text;
			}
			else
			{
				Debug.LogError("M4uTextBinding ui == null: " + string.Format (Format, Values[0]));
			}
        }

        public override string ToString()
        {
            return "Text.text=" + string.Format(Format, GetBindStr(Path));
        }
	}
}