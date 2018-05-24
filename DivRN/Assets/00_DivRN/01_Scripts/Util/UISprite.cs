using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class UISprite : MonoBehaviour
{
    public Sprite[] m_Sprites;
    [HideInInspector] [SerializeField] string m_SpriteName = "";
    [HideInInspector] [SerializeField] Image m_Image = null;

    // Use this for initialization
    void Start()
    {
        m_Image = this.gameObject.GetComponent<Image>();
        if (m_Image)
        {
            m_SpriteName = m_Image.sprite.name;
        }
    }

    public string spriteName
    {
        get
        {
            return m_SpriteName;
        }
        set
        {
            if (m_Sprites.Length != 0)
            {
                foreach (Sprite s in m_Sprites)
                {
                    if (!string.IsNullOrEmpty(s.name) && value == s.name)
                    {
                        m_SpriteName = s.name;
                        if (m_Image == null)
                        {
                            m_Image = this.gameObject.GetComponent<Image>();
                        }
                        if (m_Image)
                        {
                            m_Image.sprite = s;
                        }
                        return;
                    }
                }
            }
        }
    }

    void Update()
    {

    }
}
