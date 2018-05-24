using UnityEngine;
using System.Collections;

public class SceneScratch : Scene<SceneScratch>
{
    public Scratch Bind;

    public override void OnInitialized()
    {
        base.OnInitialized();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void OnClickButton() {
        StepUpScratchDialog dialog = StepUpScratchDialog.Create();
        dialog.SetGachaID(0);
        dialog.Show();

    }
}
