using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class BannerListItem : ListItem<BannerData>
{
    public Toggle toggle;

    void Awake()
    {
        AppearAnimationName = "banner_item_appear";
    }
    void Start()
    {
        SetModel(Context.model);

        toggle.group = transform.GetComponentInParent<ToggleGroup>();
        toggle.isOn = (Index == 1);
        if (Context.IsTexture)
        {
            transform.localPosition = new Vector3(Context.Texture.width * (Index - 1), 0, 0);
            GetComponent<RectTransform>().sizeDelta = new Vector2(Context.Texture.width, Context.Texture.height);
        }
        toggle.onValueChanged.AddListener(_flag => { Context.changeBanner(_flag, Index); });

        m_listItemModel.Start();
    }


    public void OnClick()
    {
        if (ButtonBlocker.Instance.IsActive())
        {
            return;
        }

        SoundUtil.PlaySE(SEID.SE_MENU_OK);

#if BUILD_TYPE_DEBUG
        Debug.Log("CALL Banner#OnClick:" + gameObject.name + " link:" + Context.link);
#endif
        if (Context.link.StartsWith("http"))
        {
            URLManager.OpenURL(Context.link);
            return;
        }
        else
        {
            switch (Context.JumpToInApp_Place)
            {
                case "areamap":
                    MainMenuParam.m_RegionID = MasterDataUtil.GetRegionIDFromCategory(MasterDataDefineLabel.REGION_CATEGORY.STORY);
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT_AREA_STORY, false, false);
                    break;
                case "scratch":
                    MainMenuParam.m_GachaMaster = MasterDataUtil.GetActiveGachaMaster().FirstOrDefault(g => g.fix_id == Context.JumpToInApp_Id);
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_GACHA_MAIN, false, false);
                    break;
                case "area":
                    MainMenuParam.SetQuestSelectParam(Context.JumpToInApp_Id);
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_QUEST_SELECT, false, false, false);
                    break;
                case "challenge":
                    MainMenuParam.SetChallengeSelectParamFromEventID(Context.JumpToInApp_Id);
                    MainMenuManager.Instance.AddSwitchRequest(MAINMENU_SEQ.SEQ_CHALLENGE_SELECT, false, false, false);
                    break;
            }

            return;
        }
    }
}
