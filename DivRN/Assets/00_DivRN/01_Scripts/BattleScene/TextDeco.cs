using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テキストを輪郭・影で装飾する(NGUIのOutlineの真似)
/// v5.4.0でTextMeshPro置き換えたときに使わなくなったと思う
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class TextDeco : MonoBehaviour
{
    public bool m_ShowOutline = true;
    public Vector2 m_OutlinePosition = new Vector2(0.1f, 0.1f);
    public Color m_OutlineColor = Color.magenta;

    public bool m_ShowShadow = true;
    public Vector2 m_ShadowPosition = new Vector2(0.3f, -0.3f);
    public Color m_ShadowColor = Color.black;

    private TextMesh m_BaseText = null;
    private GameObject[] m_Decos = new GameObject[5];
    private TextMesh[] m_DecosText = new TextMesh[5];

    private Vector2 m_CurrentOutlinePosition = new Vector2(0.0f, 0.0f);
    private Color m_CurrentOutlineColor = Color.magenta;
    private Vector2 m_CurrentShadowPosition = new Vector2(0.0f, 0.0f);
    private Color m_CurrentShadowColor = Color.magenta;
    private string m_CurrentText = null;
    private int m_CurrentFontSize = -1;
    private int m_CurrentLayer = -1;

    // Use this for initialization
    void Start()
    {
    }

    private void OnEnable()
    {
        GameObject deco_obj = new GameObject();
        deco_obj.layer = gameObject.layer;
        deco_obj.name = "TextDeco";
        deco_obj.transform.localPosition = Vector3.zero;
        deco_obj.SetActive(false);
        m_BaseText = GetComponent<TextMesh>();

        {
            MeshRenderer base_mesh_renderer = GetComponent<MeshRenderer>();
            MeshRenderer mesh_renderer = deco_obj.AddComponent<MeshRenderer>();
            mesh_renderer.shadowCastingMode = base_mesh_renderer.shadowCastingMode;
            mesh_renderer.receiveShadows = base_mesh_renderer.receiveShadows;
            mesh_renderer.motionVectorGenerationMode = base_mesh_renderer.motionVectorGenerationMode;
            mesh_renderer.materials = base_mesh_renderer.materials;
            mesh_renderer.lightProbeUsage = base_mesh_renderer.lightProbeUsage;
            mesh_renderer.reflectionProbeUsage = base_mesh_renderer.reflectionProbeUsage;
            mesh_renderer.probeAnchor = base_mesh_renderer.probeAnchor;
        }

        {
            TextMesh text_mesh = deco_obj.AddComponent<TextMesh>();
            text_mesh.text = m_BaseText.text;
            text_mesh.offsetZ = m_BaseText.offsetZ;
            text_mesh.characterSize = m_BaseText.characterSize;
            text_mesh.lineSpacing = m_BaseText.lineSpacing;
            text_mesh.anchor = m_BaseText.anchor;
            text_mesh.alignment = m_BaseText.alignment;
            text_mesh.tabSize = m_BaseText.tabSize;
            text_mesh.fontSize = m_BaseText.fontSize;
            text_mesh.fontStyle = m_BaseText.fontStyle;
            text_mesh.richText = m_BaseText.richText;
            text_mesh.font = m_BaseText.font;
            text_mesh.color = m_BaseText.color;
        }

        m_CurrentOutlinePosition = deco_obj.transform.localPosition;
        m_CurrentShadowPosition = deco_obj.transform.localPosition;
        m_CurrentLayer = deco_obj.layer;
        m_CurrentOutlineColor = m_BaseText.color;
        m_CurrentShadowColor = m_BaseText.color;
        m_CurrentText = m_BaseText.text;
        m_CurrentFontSize = m_BaseText.fontSize;

        m_Decos[0] = deco_obj;
        for (int idx = 1; idx < m_Decos.Length; idx++)
        {
            m_Decos[idx] = Instantiate(deco_obj);
        }

        for (int idx = 0; idx < m_Decos.Length; idx++)
        {
            m_DecosText[idx] = m_Decos[idx].GetComponent<TextMesh>();
            m_Decos[idx].transform.SetParent(transform, false);
        }
    }

    private void OnDisable()
    {
        for (int idx = 0; idx < m_Decos.Length; idx++)
        {
            Destroy(m_Decos[idx]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int layer = gameObject.layer;
        if (m_CurrentLayer != layer)
        {
            m_CurrentLayer = gameObject.layer;
            for (int idx = 0; idx < m_Decos.Length; idx++)
            {
                m_Decos[idx].layer = layer;
            }
        }

        if (m_ShowShadow)
        {
            if (m_ShadowPosition != m_CurrentShadowPosition)
            {
                m_CurrentShadowPosition = m_ShadowPosition;
                m_Decos[0].transform.localPosition = new Vector3(m_CurrentShadowPosition.x, m_CurrentShadowPosition.y, 0.4f);
            }

            if (m_ShadowColor != m_CurrentShadowColor)
            {
                m_CurrentShadowColor = m_ShadowColor;
                m_DecosText[0].color = m_CurrentShadowColor;
            }
        }
        if (m_Decos[0].IsActive() != m_ShowShadow)
        {
            m_Decos[0].SetActive(m_ShowShadow);
        }

        if (m_ShowOutline)
        {
            if (m_OutlinePosition != m_CurrentOutlinePosition)
            {
                m_CurrentOutlinePosition = m_OutlinePosition;
                m_Decos[1].transform.localPosition = new Vector3(m_CurrentOutlinePosition.x, m_CurrentOutlinePosition.y, 0.2f);
                m_Decos[2].transform.localPosition = new Vector3(-m_CurrentOutlinePosition.x, m_CurrentOutlinePosition.y, 0.2f);
                m_Decos[3].transform.localPosition = new Vector3(m_CurrentOutlinePosition.x, -m_CurrentOutlinePosition.y, 0.2f);
                m_Decos[4].transform.localPosition = new Vector3(-m_CurrentOutlinePosition.x, -m_CurrentOutlinePosition.y, 0.2f);
            }

            if (m_OutlineColor != m_CurrentOutlineColor)
            {
                m_CurrentOutlineColor = m_OutlineColor;
                m_DecosText[1].color = m_CurrentOutlineColor;
                m_DecosText[2].color = m_CurrentOutlineColor;
                m_DecosText[3].color = m_CurrentOutlineColor;
                m_DecosText[4].color = m_CurrentOutlineColor;
            }
        }
        if (m_Decos[1].IsActive() != m_ShowOutline)
        {
            m_Decos[1].SetActive(m_ShowOutline);
            m_Decos[2].SetActive(m_ShowOutline);
            m_Decos[3].SetActive(m_ShowOutline);
            m_Decos[4].SetActive(m_ShowOutline);
        }

        string base_text = m_BaseText.text; //m_BaseText.text.get は string が生成されそれが GC 対象になる.
        if (base_text != m_CurrentText)
        {
            m_CurrentText = base_text;
            for (int idx = 0; idx < m_Decos.Length; idx++)
            {
                m_DecosText[idx].text = m_CurrentText;
            }
        }

        int font_size = m_BaseText.fontSize;
        if (font_size != m_CurrentFontSize)
        {
            m_CurrentFontSize = font_size;
            for (int idx = 0; idx < m_Decos.Length; idx++)
            {
                m_DecosText[idx].fontSize = m_CurrentFontSize;
            }
        }
    }
}
