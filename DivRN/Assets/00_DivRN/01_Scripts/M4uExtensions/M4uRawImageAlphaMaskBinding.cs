using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M4u
{
    [RequireComponent(typeof(RawImage))]
    public class M4uRawImageAlphaMaskBinding : M4uBindingSingle
    {
        private RawImage ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<RawImage>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();
            if (Values[0] == null)
            {
                ui.material = null;
                return;
            }


            ui.material = new Material(Resources.Load<Material>("Material/AlphaMaskMaterial"));
            ui.material.SetTexture("_AlphaTex", Values[0] as Texture);
        }

        public override string ToString()
        {
            return "RawImage.texture=" + GetBindStr(Path);
        }
    }
}
