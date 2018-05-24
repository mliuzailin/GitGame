using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class ToggleImage : MonoBehaviour
{
    public Graphic offGraphic = null;
    public Graphic onGraphic = null;
    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    void Start()
    {
        toggle.onValueChanged.AddListener((value) =>
        {
            OnValueChanged(value);
        });
        //初期状態を反映
        offGraphic.enabled = !toggle.isOn;
        toggle.targetGraphic = (toggle.isOn) ? onGraphic : offGraphic;
    }

    void OnValueChanged(bool value)
    {
        if (offGraphic != null)
        {
            offGraphic.enabled = !value;
        }

        toggle.targetGraphic = (value) ? onGraphic : offGraphic;
    }
}
