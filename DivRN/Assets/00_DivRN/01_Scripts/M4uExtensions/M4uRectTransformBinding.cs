using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace M4u
{
    /// <summary>
    /// RectTransformBinding. Bind RectTransform
    /// </summary>
	[AddComponentMenu("M4u/RectTransformBinding")]
    public class M4uRectTransformBinding : M4uBindingSingle
    {
        public TransformType Type = TransformType.Px;

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
            switch (Type)
            {
                case TransformType.Px:
                    ui.anchoredPosition3D = new Vector3(value, ui.anchoredPosition3D.y, ui.anchoredPosition3D.z);
                    break;
                case TransformType.Py:
                    ui.anchoredPosition3D = new Vector3(ui.anchoredPosition3D.x, value, ui.anchoredPosition3D.z);
                    break;
                case TransformType.Pz:
                    ui.anchoredPosition3D = new Vector3(ui.anchoredPosition3D.x, ui.anchoredPosition3D.y, value);
                    break;

                case TransformType.Rx:
                    ui.localRotation = Quaternion.Euler(value, ui.localRotation.y, ui.localRotation.z);
                    break;
                case TransformType.Ry:
                    ui.localRotation = Quaternion.Euler(ui.localRotation.x, value, ui.localRotation.z);
                    break;
                case TransformType.Rz:
                    ui.localRotation = Quaternion.Euler(ui.localRotation.x, ui.localRotation.y, value);
                    break;

                case TransformType.Sx:
                    ui.localScale = new Vector3(value, ui.localScale.y, ui.localScale.z);
                    break;
                case TransformType.Sy:
                    ui.localScale = new Vector3(ui.localScale.x, value, ui.localScale.z);
                    break;
                case TransformType.Sz:
                    ui.localScale = new Vector3(ui.localScale.x, ui.localScale.y, value);
                    break;
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case TransformType.Px:
                    return "RectTransform.anchoredPosition3D.x=" + GetBindStr(Path);
                case TransformType.Py:
                    return "RectTransform.anchoredPosition3D.y=" + GetBindStr(Path);
                case TransformType.Pz:
                    return "RectTransform.anchoredPosition3D.z=" + GetBindStr(Path);

                case TransformType.Rx:
                    return "Transform.localRotation.x=" + GetBindStr(Path);
                case TransformType.Ry:
                    return "Transform.localRotation.y=" + GetBindStr(Path);
                case TransformType.Rz:
                    return "Transform.localRotation.z=" + GetBindStr(Path);

                case TransformType.Sx:
                    return "Transform.localScale.x=" + GetBindStr(Path);
                case TransformType.Sy:
                    return "Transform.localScale.y=" + GetBindStr(Path);
                case TransformType.Sz:
                    return "Transform.localScale.z=" + GetBindStr(Path);
            }
            return "";
        }
    }
}