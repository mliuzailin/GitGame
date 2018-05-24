using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace M4u
{

    [AddComponentMenu("M4u/RectTransformSizeBinding")]
    public class M4uRectTransformSizeBinding : M4uBindingSingle
    {
        public enum SizeType
        {
            Width,
            Height,
        };

        public SizeType sizeType = SizeType.Width;

        private RectTransform ui = null;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<RectTransform>();
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            float value = float.Parse(Values[0].ToString());
            switch (sizeType)
            {
                case SizeType.Width:
                    ui.sizeDelta = new Vector2(value, ui.sizeDelta.y);
                    break;
                case SizeType.Height:
                    ui.sizeDelta = new Vector2(ui.sizeDelta.x, value);
                    break;
            }
        }

        public override string ToString()
        {
            switch (sizeType)
            {
                case SizeType.Width:
                    return "RectTransform.sizeDelta.x=" + GetBindStr(Path);
                case SizeType.Height:
                    return "RectTransform.sizeDelta.y=" + GetBindStr(Path);
            }
            return "";
        }
    }
}
