using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSpriteListItem : ListItem<BossSpriteContext>
{
    private readonly float SCALE_RATE = 0.00125f;
    private readonly float SCALE_MIN = 0.0f;
    private readonly float SCALE_MAX = 3.0f;
    private readonly float ALPHA_RATE = 0.003125f;

    public RectTransform ImageRectTrans = null;
    public Image BossImage = null;

    private Camera m_MainCamera = null;

    private void Start()
    {
        m_MainCamera = Camera.main;
    }
    private void Update()
    {
        if (!Context.IsSetup ||
            m_MainCamera == null)
        {
            return;
        }
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_MainCamera, gameObject.transform.position);
        float tmpX = (screenPos.x - (Screen.width / 2.0f));

        //拡大率計算
        float scale = 0.8f + (tmpX * SCALE_RATE);

        if (scale < SCALE_MIN)
        {
            scale = SCALE_MIN;
        }
        if (scale > SCALE_MAX)
        {
            scale = SCALE_MAX;
        }

        ImageRectTrans.localScale = new Vector3(scale, scale, 1.0f);

        //透過率計算
        float alpha = 1.0f;
        if (tmpX != 0.0f)
        {
            alpha = 1.0f - (Mathf.Abs(tmpX) * ALPHA_RATE);
        }
        if (alpha < 0.0f)
        {
            alpha = 0.0f;
        }
        if (alpha > 1.0f)
        {
            alpha = 1.0f;
        }
        BossImage.color = new Color(1, 1, 1, alpha);
    }
}
