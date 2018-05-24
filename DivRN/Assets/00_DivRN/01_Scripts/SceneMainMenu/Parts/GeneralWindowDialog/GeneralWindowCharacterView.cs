using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralWindowCharacterView : View
{
    static readonly string PrefabPath = "Prefab/GeneralWindowDialog/GeneralWindowCharacterView";
    static readonly string AssetBundleName = "general_window_common";

    [SerializeField]
    Image m_Image = null;

    int m_fix_id;

    public static GeneralWindowCharacterView Attach(GameObject parent)
    {
        return Attach<GeneralWindowCharacterView>(PrefabPath, parent);
    }

    void Awake()
    {
        m_fix_id = -1;
        m_Image.color = m_Image.color.WithAlpha(0);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUpData(MasterDataGeneralWindow master)
    {
        int fix_id = (int)master.fix_id;
        m_fix_id = fix_id;
        string assetName = master.char_img;

        if (m_Image == null)
        {
            return;
        }

        m_Image.color = m_Image.color.WithAlpha(0);

        RectTransform ImageRect = m_Image.GetComponent<RectTransform>();
        ImageRect.anchoredPosition = new Vector2(master.char_offset_x, master.char_offset_y);

        AssetBundler asset = AssetBundler.Create().Set(AssetBundleName,
                        (o) =>
                        {
                            SetSprite(o.GetTexture2D(assetName, TextureWrapMode.Clamp),
                                    o.GetTexture(assetName + "_mask", TextureWrapMode.Clamp),
                                    fix_id);
                        },
                        (str) =>
                        {

                        })
                        .Load();


    }

    public void SetSprite(Texture2D texture, Texture mask = null, int fix_id = -1)
    {
        if (m_Image == null)
        {
            return;
        }

        if (fix_id >= 0 && fix_id != m_fix_id)
        {
            return;
        }

        m_Image.sprite = null;

        if (texture == null)
        {
            m_Image.color = m_Image.color.WithAlpha(0);
        }
        else
        {
            m_Image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            m_Image.SetNativeSize();
            m_Image.color = m_Image.color.WithAlpha(1);
        }

        if (mask != null)
        {
            m_Image.material = new Material(Resources.Load<Material>("Material/AlphaMaskMaterial"));
            m_Image.material.SetTexture("_AlphaTex", mask);
        }
        else
        {
            m_Image.material = null;
        }
    }
}
