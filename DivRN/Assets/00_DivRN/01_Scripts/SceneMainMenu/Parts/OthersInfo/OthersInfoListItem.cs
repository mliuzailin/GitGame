/**
 *  @file   OthersInfoListItem.cs
 *  @brief
 *  @author Developer
 *  @date   2017/03/06
 */

using UnityEngine;
using System.Collections;

public class OthersInfoListItem : ListItem<OthersInfoListContext>
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        if (Context.DidSelectItem != null)
        {
            Context.DidSelectItem(Context);
        }
        else
        {
            switch (Context.Type)
            {
                case OthersInfoListContext.InfoType.NONE:
                    break;
                case OthersInfoListContext.InfoType.DIALOG:
                    OnClickDialog(Context);
                    break;
                case OthersInfoListContext.InfoType.SCROLL_DIALOG:
                    OnClickScrollDialog(Context);
                    break;
                case OthersInfoListContext.InfoType.TUTORIAL_DIALOG:
                    OnClickTutorialDialog(Context);
                    break;
                case OthersInfoListContext.InfoType.WEB_VIEW:
                    OnClickWebView(Context);
                    break;
                case OthersInfoListContext.InfoType.MOVIE:
                    OnClickMovie(Context);
                    break;
                case OthersInfoListContext.InfoType.GENERAL_WINDOW:
                    OnClickGeneralWindow(Context);
                    break;
            }
        }
    }

    void OnClickDialog(OthersInfoListContext item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        Dialog newDialog = Dialog.Create(DialogType.DialogOK);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, item.HelpTitleKey);
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, item.HelpDetailKey);
        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    void OnClickScrollDialog(OthersInfoListContext item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        Dialog newDialog = Dialog.Create(DialogType.DialogScroll);
        newDialog.SetDialogTextFromTextkey(DialogTextType.Title, item.HelpTitleKey);
        newDialog.SetDialogTextFromTextkey(DialogTextType.MainText, item.HelpDetailKey);

        newDialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
        newDialog.SetDialogEvent(DialogButtonEventType.OK, new System.Action(() =>
        {
        }));
        newDialog.EnableFadePanel();
        newDialog.DisableCancelButton();
        newDialog.Show();
    }

    void OnClickTutorialDialog(OthersInfoListContext item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);

        TutorialDialog.Create().SetTutorialType(item.TutorialDialogType).DisableSaveShowFlag().Show();
    }

    void OnClickWebView(OthersInfoListContext item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        URLManager.OpenURL(item.URL);
    }

    void OnClickGeneralWindow(OthersInfoListContext item)
    {
        SoundUtil.PlaySE(SEID.SE_MENU_OK2);
        GeneralWindowDialog dialog = GeneralWindowDialog.Create();
        dialog.SetGroupID(item.ID);
        dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.CLOSE, () =>
        {

        });

        dialog.Show();
    }

    void OnClickMovie(OthersInfoListContext item)
    {
        GameObject _obj = Resources.Load("Prefab/MoviePrefab") as GameObject;
        if (_obj != null)
        {
            Instantiate(_obj);
        }
        if (MovieManager.Instance != null)
        {
            AndroidBackKeyManager.Instance.StackPush(gameObject, () =>
            {
                if (MovieManager.Instance != null)
                {
                    SoundManager.Instance.PlaySE(SEID.SE_MENU_OK);
                    MovieManager.Instance.finishMovie();
                    AndroidBackKeyManager.Instance.StackPop(gameObject);
                }
            });
            MovieManager.Instance.play(item.MovieName, true, false, false, true, BGMManager.EBGM_ID.eBGM_2_1, true);
            MovieManager.Instance.skipButtonAction = () =>
            {
                AndroidBackKeyManager.Instance.StackPop(gameObject);
            };
        }
    }
}
