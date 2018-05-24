using M4u;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class TextMeshProReplacer
{
    /// <summary>
    /// シーン内を置き換える
    /// </summary>
    [MenuItem("Tools/Text Mesh Replacer/Replace Current Scene")]
    internal static void ReplaceCurrentScene()
    {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < rootGameObjects.Length; i++)
        {
            GameObject root = rootGameObjects[i];
            for (int j = 0; j < root.transform.childCount; j++)
            {
                ReplaceUnityText(root.transform.GetChild(j).GetComponentsInChildren<Text>(true));
            }
        }
    }

    /// <summary>
    /// 選択したディレクトリ以下のプレハブを置き換える
    /// </summary>
    [MenuItem("Assets/DivRN/Text Mesh Replacer/Replace All Prefab")]
    internal static void ReplaceAllScene()
    {
        string[] selectAssetList = Selection.assetGUIDs;
        string[] selectAssetListPath = new string[selectAssetList.Length];
        for (int i = 0; i < selectAssetList.Length; i++)
        {
            selectAssetListPath[i] = AssetDatabase.GUIDToAssetPath(selectAssetList[i]);
        }

        string[] assetList = AssetDatabase.FindAssets("t:Prefab", selectAssetListPath);

        for (int i = 0; i < assetList.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetList[i]);
            GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            EditorUtility.DisplayProgressBar("Replacing in Prefab...", gameObj.name, (float)i / assetList.Length);

            ReplaceUnityText(gameObj.GetComponent<Text>());
            ReplaceUnityText(gameObj.GetComponentsInChildren<Text>(true));
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// ヒエラルキー内の選択したオブジェクト以下を置き換える
    /// </summary>
    [MenuItem("GameObject/DivRN/Text Mesh Replacer - Object", false, 16)]
    internal static void ReplaceGameObject()
    {
        if (Selection.activeGameObject != null)
        {
            ReplaceUnityText(Selection.activeGameObject.GetComponent<Text>());
            ReplaceUnityText(Selection.activeGameObject.GetComponentsInChildren<Text>(true));
        }
    }

    /// <summary>
    /// ヒエラルキー内の選択したオブジェクト以下を置き換える
    /// </summary>
    [MenuItem("GameObject/DivRN/Text Mesh InputField Replacer - Object", false, 16)]
    internal static void ReplaceInputFieldGameObject()
    {
        if (Selection.activeGameObject != null)
        {
            ReplaceUnityInputField(Selection.activeGameObject.GetComponent<InputField>());
        }
    }

    internal static void ReplaceUnityText(Text[] unityTexts)
    {
        TMP_FontAsset[] fonts = FindAssets<TMP_FontAsset>();
        if (fonts.Length == 0)
        {
            return;
        }
        List<Font> missingFonts = new List<Font>();
        for (int i = 0; i < unityTexts.Length; i++)
        {
            Text text = unityTexts[i];

            if (!missingFonts.Contains(text.font))
            {
                TMP_FontAsset font = GetTMPFont(text.font, fonts);
                if (font != null)
                {
                    ReplaceUnityText(text, font);
                }
                else
                {
                    missingFonts.Add(text.font);
                }
            }
        }
        //log missing fonts if any
        if (missingFonts.Count == 0)
        {
            return;
        }
        Debug.LogWarningFormat("Missing {0} fonts", missingFonts.Count);
        for (int i = 0; i < missingFonts.Count; i++)
        {
            Debug.LogWarningFormat("Text Mesh pro Font {0} is missing", missingFonts[i].name);
        }
    }

    /// <summary>
    /// TextコンポーネントをTextMesh Proに置き換える
    /// </summary>
    /// <param name="unityText"></param>
    internal static void ReplaceUnityText(Text unityText)
    {
        if (unityText == null)
        {
            return;
        }

        TMP_FontAsset[] fonts = FindAssets<TMP_FontAsset>();
        if (fonts.Length == 0)
        {
            return;
        }
        TMP_FontAsset font = GetTMPFont(unityText.font, fonts);
        if (font != null)
        {
            ReplaceUnityText(unityText, font);
        }
        else
        {
            Debug.LogWarningFormat("Text Mesh pro Font {0} is missing", unityText.font.name);
        }
    }

    private static void ReplaceUnityText(Text unityText, TMP_FontAsset font)
    {
        Text currentText = unityText;
        //check if tmpro text already exist, if not dont replace

        //for some reason adding tmpro component messed up the rect transform size
        //this will fix it
        Vector2 size = currentText.rectTransform.sizeDelta;

        TextData textData = new TextData();
        GameObject obj = unityText.gameObject;
        textData.write(currentText, font);

        //Selection.activeObject = obj;
        Undo.DestroyObjectImmediate(currentText);


        TextMeshProUGUI tmproText = Undo.AddComponent<TextMeshProUGUI>(obj);
        tmproText.autoSizeTextContainer = false;
        textData.read(tmproText);
        tmproText.rectTransform.sizeDelta = size;

        ReplaceTextEffect(tmproText);
        ReplaceTextBinding(tmproText);

        EditorUtility.SetDirty(tmproText);
    }

    /// <summary>
    /// M4uBindingの置き換え
    /// </summary>
    /// <param name="text"></param>
    private static void ReplaceTextBinding(TextMeshProUGUI text)
    {
        GameObject obj = text.gameObject;
        M4uTextBinding textBinding = text.GetComponent<M4uTextBinding>();
        if (textBinding != null)
        {
            string path = textBinding.Path;
            string format = textBinding.Format;
            //bool newLine = textBinding.NewLine;
            bool newLine = false;

            Undo.DestroyObjectImmediate(textBinding);

            M4uTMPTextBinding tmproTextBinding = Undo.AddComponent<M4uTMPTextBinding>(obj);
            tmproTextBinding.Path = path;
            tmproTextBinding.Format = format;
            tmproTextBinding.NewLine = newLine;
        }

        M4uTextColorBinding textColorBinding = text.GetComponent<M4uTextColorBinding>();
        if (textColorBinding != null)
        {
            string path = textColorBinding.Path;

            Undo.DestroyObjectImmediate(textColorBinding);

            M4uTMPTextColorBinding tmproTextColorBinding = Undo.AddComponent<M4uTMPTextColorBinding>(obj);
            tmproTextColorBinding.Path = path;
        }

        M4uTextBindings textBindings = text.GetComponent<M4uTextBindings>();
        if (textBindings != null)
        {
            string[] path = new string[textBindings.Path.Length];
            for (int i = 0; i < textBindings.Path.Length; ++i)
            {
                path[i] = textBindings.Path[i];
            }
            string format = textBindings.Format;

            Undo.DestroyObjectImmediate(textBindings);

            M4uTMPTextBindings tmproTextBinding = Undo.AddComponent<M4uTMPTextBindings>(obj);
            tmproTextBinding.Path = path;
            tmproTextBinding.Format = format;
        }

    }

    /// <summary>
    /// Shadow・Outlineの置き換え
    /// </summary>
    /// <param name="text"></param>
    private static void ReplaceTextEffect(TextMeshProUGUI text)
    {
        Outline outline = null;
        Shadow shadow = null;

        Shadow[] effects = text.GetComponents<Shadow>();
        for (int i = 0; i < effects.Length; ++i)
        {
            if (outline == null && effects[i].GetType() == typeof(Outline))
            {
                outline = (Outline)effects[i];
            }
            if (shadow == null && effects[i].GetType() == typeof(Shadow))
            {
                shadow = effects[i];
            }
        }

        Material material = null;
        string fontMaterialName = "Font/TextMeshPro/" + text.font.name;

        if (outline != null && shadow != null)
        {
            int shadowX = (int)shadow.effectDistance.x;
            int shadowY = (int)shadow.effectDistance.y;
            int num = 1;
            if (shadowX == 1 && shadowY == -2)
            {
                num = 4;
            }
            else
            {
                num = shadowX;
            }

            material = Resources.Load<Material>(fontMaterialName + " Default" + num.ToString("D2"));

            if (material == null)
            {
                material = Resources.Load<Material>(fontMaterialName + " Default01");
            }
        }
        else if (outline != null)
        {
            int outlineNum = (int)outline.effectDistance.x;

            material = Resources.Load<Material>(fontMaterialName + " Outline" + outlineNum.ToString("D2"));

            if (material == null)
            {
                material = Resources.Load<Material>(fontMaterialName + " Outline01");
            }
        }
        else if (shadow != null)
        {
            int shadowNum = (int)shadow.effectDistance.x;

            material = Resources.Load<Material>(fontMaterialName + " Shadow" + shadowNum.ToString("D2"));

            if (material == null)
            {
                material = Resources.Load<Material>(fontMaterialName + " Shadow01");
            }
        }

        if (outline != null)
        {
            Undo.DestroyObjectImmediate(outline);
        }
        if (shadow != null)
        {
            Undo.DestroyObjectImmediate(shadow);
        }

        if (material != null)
        {
            text.fontMaterial = material;
        }
    }

    internal static void ReplaceUnityInputField(InputField unityInputField)
    {
        if (unityInputField == null)
        {
            return;
        }

        Graphic _targetGraphic = unityInputField.targetGraphic;
        ColorBlock _colors = unityInputField.colors;
        Color _selectionColor = unityInputField.selectionColor;
        TMP_InputField.ContentType _contentType = (TMP_InputField.ContentType)unityInputField.contentType;
        TMP_InputField.LineType _lineType = (TMP_InputField.LineType)unityInputField.lineType;
        int _characterLimit = unityInputField.characterLimit;


        GameObject obj = unityInputField.gameObject;
        Undo.DestroyObjectImmediate(unityInputField);

        TMP_InputField tmproInputField = Undo.AddComponent<TMP_InputField>(obj);
        tmproInputField.targetGraphic = _targetGraphic;
        tmproInputField.colors = _colors;
        tmproInputField.selectionColor = _selectionColor;
        tmproInputField.contentType = _contentType;
        tmproInputField.lineType = _lineType;
        tmproInputField.characterLimit = _characterLimit;

        tmproInputField.targetGraphic.GetComponent<Image>();

    }

    private static T[] FindAssets<T>() where T : Object
    {
        List<T> result = new List<T>();
        string[] assetList = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name));
        for (int i = 0; i < assetList.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetList[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                result.Add(asset);
            }
        }
        return result.ToArray();
    }

    private static TMP_FontAsset GetTMPFont(Font m_font, TMP_FontAsset[] fonts)
    {
        if (m_font == null)
        {
            return TMP_FontAsset.defaultFontAsset;
        }

        for (int i = 0; i < fonts.Length; i++)
        {

            TMP_FontAsset fontAsset = fonts[i];
            string fontname = ConvFontName(m_font);
            if (fontAsset.name.Equals(fontname, System.StringComparison.OrdinalIgnoreCase))
            {
                return fontAsset;
            }

            if (System.Array.Exists(m_font.fontNames, (s) => s.Equals(fontAsset.fontInfo.Name, System.StringComparison.OrdinalIgnoreCase)))
            {
                return fontAsset;
            }
        }
        return TMP_FontAsset.defaultFontAsset;
    }

    private static string ConvFontName(Font font)
    {

        string fontName = (font != null) ? font.name : "";
        string name = fontName;

        switch (fontName)
        {
            case "rodin":
                name = "FOT-RodinNTLGPro-EB";
                break;
            case "Arial":
            case "rodin_db":
                name = "FOT-RodinNTLGPro-DB";
                break;
            case "lucky7":
                name = "FOT-Lucky7";
                break;
            case "pqdmImpact":
                name = "impact";
                break;
        }

        return name + " SDF";
    }
}

internal struct TextData
{
    private string text;
    private TextAlignmentOptions anchor;
    private TMP_FontAsset tmProfont;
    private Color color;
    private float fontSize;
    private FontStyles fontStyle;
    private bool autoResize;
    private Vector2 minMaxSize;
    private bool raycastTarget;
    float ratioFontSize;
    bool enable;

    public void write(Text textObject, TMP_FontAsset tmpFont)
    {
        color = textObject.color;
        if (textObject.fontSize > 0)
        {
            fontSize = textObject.fontSize;
        }
        else if (textObject.font != null)
        {
            fontSize = textObject.font.lineHeight;
        }
        fontStyle = GetFontStyle(textObject.fontStyle);
        tmProfont = tmpFont;
        anchor = GetTextAlignment(textObject.alignment);
        autoResize = textObject.resizeTextForBestFit;
        minMaxSize = new Vector2(textObject.resizeTextMinSize, textObject.resizeTextMaxSize);
        raycastTarget = textObject.raycastTarget;
        enable = textObject.enabled;
        if (textObject.fontStyle == FontStyle.BoldAndItalic)
        {
            text = string.Format("<i>{0}</i>", textObject.text);
        }
        else
        {
            text = textObject.text;
        }

        ratioFontSize = GetRatioFontSize(textObject.font);
    }

    public void read(TextMeshProUGUI textObject)
    {
        textObject.text = text;
        textObject.color = color;

        textObject.fontSize = (int)System.Math.Round(fontSize * ratioFontSize, System.MidpointRounding.AwayFromZero);
        textObject.fontStyle = fontStyle;

        textObject.alignment = anchor;

        textObject.enableAutoSizing = autoResize;
        textObject.fontSizeMin = minMaxSize.x;
        textObject.fontSizeMax = minMaxSize.y;
        textObject.raycastTarget = raycastTarget;
        textObject.enabled = enable;
        if (tmProfont != null)
        {
            textObject.font = tmProfont;
        }
    }

    private FontStyles GetFontStyle(FontStyle style)
    {
        switch (style)
        {
            case FontStyle.Bold:
                return FontStyles.Bold;
            case FontStyle.Italic:
                return FontStyles.Italic;
            case FontStyle.BoldAndItalic:
                //this might not work because it's protected inside tmpro internal
                //and i'm too lazy too look it up :P
                //so i added <i> tags around it
                return FontStyles.Bold;
        }
        return FontStyles.Normal;
    }
    private TextAlignmentOptions GetTextAlignment(TextAnchor m_anchor)
    {
        switch (m_anchor)
        {
            case TextAnchor.LowerCenter:
                return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerLeft:
                return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerRight:
                return TextAlignmentOptions.BottomRight;
            case TextAnchor.MiddleCenter:
                return TextAlignmentOptions.Center;
            case TextAnchor.MiddleLeft:
                return TextAlignmentOptions.MidlineLeft;
            case TextAnchor.MiddleRight:
                return TextAlignmentOptions.MidlineRight;
            case TextAnchor.UpperCenter:
                return TextAlignmentOptions.Top;
            case TextAnchor.UpperLeft:
                return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperRight:
                return TextAlignmentOptions.TopRight;
            default:
                return TextAlignmentOptions.Baseline;
        }
    }

    /// <summary>
    /// 変換する時サイズが変わるので、おおよその比率で変更する
    /// </summary>
    /// <param name="font_name"></param>
    /// <returns></returns>
    private float GetRatioFontSize(Font font)
    {
        float ratio = 1.0f;
        string fontName = (font != null) ? font.name : "";

        switch (fontName)
        {
            case "rodin":
                ratio = 0.79f;
                break;
            case "rodin_db":
                ratio = 0.79f;
                break;
            case "lucky7":
                ratio = 0.895f;
                break;
            case "pqdmImpact":
                ratio = 1.09f;
                break;
            case "Arial":
                ratio = 0.92f;
                break;
        }

        return ratio;
    }
}