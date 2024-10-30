using LBoL.Core;
using LBoL_Doremy.DoremyChar.DoremyPU;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.Utils
{
    public static class DColorUtils
    {
        static public string DL { get => $"<color={DoremyCavalierDef.Color}>DL</color>"; }

        static string uiBlueHex = ColorUtility.ToHtmlStringRGB(GlobalConfig.UiBlue);


        static public string LightBlue { get => $"<color=#B2FFFF>"; }

        static public string UIBlue { get => $"<color=#{uiBlueHex}>"; }

        static public string CC { get => "</color>"; }
    }
}
