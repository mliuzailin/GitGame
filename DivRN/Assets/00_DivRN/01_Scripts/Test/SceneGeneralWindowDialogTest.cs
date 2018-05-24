using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneGeneralWindowDialogTest : SceneTest<SceneGeneralWindowDialogTest>
{
    [SerializeField]
    uint GeneralWindowGroupID = 1;
    [SerializeField]
    GameObject GeneralWindow = null;

    protected override void Awake()
    {
        base.Awake();
        if (GeneralWindow != null)
        {
            GeneralWindow.SetActive(false);
        }
    }

    public void OnClick()
    {
        GeneralWindowDialog dialog = GeneralWindowDialog.Create();
        dialog.SetGroupID(GeneralWindowGroupID);
        dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.YES, () =>
        {
            Debug.Log("Send Yes");
        });
        dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.NO, () =>
        {
            Debug.Log("Send No");
        });
        dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.CLOSE, () =>
        {
            Debug.Log("Send Close");
        });

        dialog.Show();
    }
}
