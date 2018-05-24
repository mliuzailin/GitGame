using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionBadge : MonoBehaviour
{
    private TextMeshProUGUI numText;
    private Image image;


    void Awake()
    {
        numText = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();

        Set(0);
    }


    public void Set(int num)
    {
        if (num <= 0)
        {
            numText.text = "";
            image.enabled = false;
            return;
        }
        numText.text = num.ToString();
        image.enabled = true;
    }
}
