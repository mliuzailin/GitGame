using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugRichText : MenuPartsBase
{
    static readonly string m_FontPath = "Font/TextMeshPro";
    static readonly string[] m_ExcludeMaterials = new string[] {
        "impact SDF BDV_Shadow"
    };


    [SerializeField]
    TMP_InputField m_TextArea = null;
    [SerializeField]
    TMP_Dropdown m_FontSelect = null;
    [SerializeField]
    TMP_Dropdown m_MaterialSelect = null;
    [SerializeField]
    Toggle m_TogleMaterial = null;
    [SerializeField]
    Toggle m_TogleLineBreak = null;

    private string[] m_MaterialNameArray = null;

    // Use this for initialization
    void Start()
    {
        SetPositionAjustStatusBar(Vector2.zero);

        List<string> fontNameArray = Resources.LoadAll<TMP_FontAsset>(m_FontPath).Select((v) => v.name).ToList();
        m_MaterialNameArray = Resources.LoadAll<Material>(m_FontPath).Select((v) => v.name).ToArray();

        int index = fontNameArray.FindIndex((v) => v == m_TextArea.fontAsset.name);
        m_FontSelect.ClearOptions();
        m_FontSelect.AddOptions(fontNameArray);
        m_FontSelect.value = index;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetFont(string fontName)
    {
        m_TextArea.fontAsset = Resources.Load<TMP_FontAsset>(string.Format("{0}/{1}", m_FontPath, fontName));
    }

    void SetTag(string tag)
    {
        if (m_TogleLineBreak != null && m_TogleLineBreak.isOn)
        {
            string text = TextUtil.RemoveNewLine(m_TextArea.text);
            string[] strs = text.Split('\n');

            text = "";
            for (int i = 0; i < strs.Length; ++i)
            {
                if (i < strs.Length - 1)
                {
                    text += strs[i];
                }
                else
                {
                    text += string.Format(tag, strs[i]);
                }
            }
        }
        else
        {
            string lineBreak = (m_TextArea.text != string.Empty) ? "\n" : "";
            m_TextArea.text += lineBreak + string.Format(tag, "New Text");
        }

    }

    public void OnChangeValueFontSelect(int result)
    {
        string fontName = m_FontSelect.captionText.text;

        // マテリアル
        List<string> names = new List<string>();
        for (int i = 0; i < m_MaterialNameArray.Length; ++i)
        {
            string materialName = m_MaterialNameArray[i];
            if (materialName.StartsWith(fontName))
            {
                if (m_ExcludeMaterials.Contains(materialName) == false)
                {
                    names.Add(materialName);
                }
            }
        }
        int index = names.FindIndex((v) => (v == string.Format("{0} material", fontName)));
        m_MaterialSelect.ClearOptions();
        m_MaterialSelect.AddOptions(names);
        m_MaterialSelect.value = index;
    }

    public void OnClickAddFont()
    {
        string tag = "{0}";
        if (m_TogleMaterial.isOn == true)
        {
            tag = tag.FontTag(m_FontSelect.captionText.text, m_MaterialSelect.captionText.text);

        }
        else
        {
            tag = tag.FontTag(m_FontSelect.captionText.text);
        }
        SetTag(tag);
    }



}