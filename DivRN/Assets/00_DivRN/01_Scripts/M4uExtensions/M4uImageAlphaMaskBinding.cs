using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M4u
{
    [RequireComponent(typeof(Image))]
    public class M4uImageAlphaMaskBinding : M4uBindingSingle
    {
        public Material overrideSourceMaterial;

        private Image ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<Image>();
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
            if (overrideSourceMaterial == null)
            {
                overrideSourceMaterial = Resources.Load<Material>("Material/AlphaMaskMaterial");
            }

            ui.material = new Material(overrideSourceMaterial);
            ui.material.SetTexture("_AlphaTex", Values[0] as Texture);
        }

        public override string ToString()
        {
            return "Image.sprite=" + GetBindStr(Path);
        }
    }
}
