using UnityEngine;
using System.Collections;
using M4u;

public class SceneTextMeshProTest : M4uContextMonoBehaviour
{
    M4uProperty<string> bindingText = new M4uProperty<string>();
    public string BindingText { get { return bindingText.Value; } set { bindingText.Value = value; } }

    M4uProperty<Color> bindingTextColor = new M4uProperty<Color>();
    public Color BindingTextColor { get { return bindingTextColor.Value; } set { bindingTextColor.Value = value; } }

    M4uProperty<Color> bindingColor = new M4uProperty<Color>();
    public Color BindingColor { get { return bindingColor.Value; } set { bindingColor.Value = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
        BindingText = "TextBindingTest<color=#00ffffff>あいうえお</color>";
        BindingTextColor = Color.cyan;
        BindingColor = HexColor.ToColor("#00ffff77");
    }

    // Update is called once per frame

    void Update()
    {

    }



}
