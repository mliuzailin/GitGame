using UnityEngine;
using System.Collections;

namespace BattleScene
{
    public class FieldCollision : MonoBehaviour
    {
        // 最後にタッチしたコリジョン
        private static FieldCollision m_LastTouch = null;

        public bool IsMouseOver
        {
            get
            {
                bool ret_val = (m_LastTouch == this);
                return ret_val;
            }
        }

        private void _off()
        {
            if (m_LastTouch == this)
            {
                m_LastTouch = null;
            }
        }

        private void OnDestroy()
        {
            _off();
        }

        void OnMouseOver()
        {
            m_LastTouch = this;
        }

        void OnMouseExit()
        {
            _off();
        }

        void OnMouseUp()
        {
            _off();
        }
    }
}
