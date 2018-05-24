using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M4u
{
    /// <summary>
    /// M4uEnableBinding. Bind Behaviour.enabled
    /// </summary>
	[AddComponentMenu("M4u/EnableOnceBinding")]
    public class M4uEnableOnceBinding : M4uBindingBool
    {
        public GameObject target = null;
        public override void Start()
        {
            base.Start();

            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            SetEnable();
        }

        private void SetEnable()
        {
            if (target != null)
            {
                target.SetActive(IsCheck());
            }
        }

        public override string ToString()
        {
            return "GameObject.enabled=" + base.ToString();
        }
    }
}
