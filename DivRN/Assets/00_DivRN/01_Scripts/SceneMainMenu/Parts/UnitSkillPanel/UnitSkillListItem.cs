using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSkillListItem : ListItem<UnitSkillContext>
{

    public GameObject TurnTitleObject = null;
    public GameObject TurnNumObject = null;
	public RectTransform MessageRectTransform = null;

    void Start()
    {
        if (Context.IsTurnLabelBlackColor)
        {
            //-------------------------------------------------
            // ターンの色を黒にする
            //-------------------------------------------------
            Color color = Color.black;
            if (TurnTitleObject != null)
            {
                Text titleText = TurnTitleObject.GetComponent<Text>();
                if (titleText != null)
                {
                    titleText.color = color;
                }
            }

            if (TurnNumObject != null)
            {
                Text titleText = TurnNumObject.GetComponent<Text>();
                if (titleText != null)
                {
                    titleText.color = color;
                }
            }

        }
    }
}