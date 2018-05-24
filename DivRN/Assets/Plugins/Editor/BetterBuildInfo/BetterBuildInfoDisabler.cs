using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

[InitializeOnLoad]
public static class BuildInfoDisabler
{
    static BuildInfoDisabler()
    {
        BetterBuildInfo.ForceDisabled = false;
#if BETTERBUILDINFO_DISABLED
        BetterBuildInfo.ForceDisabled = true;
#endif
    }
}