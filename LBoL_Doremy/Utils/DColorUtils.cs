using LBoL.Core;
using LBoL.Core.Cards;
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

        public const string common = "#a0a1a0";
        public const string uncommon = "#3277d1";
        public const string rare = "#d6cd28";

        public const string basicAndCommon = "#898e89";
        public const string misfortune = "#8a3baf";
        public const string status = "#b25eee";
        public const string tool = "#f8f8ff";





        public static string ColorName(this Card card, string defaultColor = "#000000")
        {
            var name = card.Name + (card.IsUpgraded ? " +" : "");
            if (card.IsBasic && card.Config.Rarity == LBoL.Base.Rarity.Common)
                return name.WrapHex(basicAndCommon);
            switch (card.CardType)
            {
                case LBoL.Base.CardType.Status:
                    return name.WrapHex(status);
                case LBoL.Base.CardType.Misfortune:
                    return name.WrapHex(misfortune);
                case LBoL.Base.CardType.Tool:
                    return name.WrapHex(tool);
                default:
                    break;
            }

            switch (card.Config.Rarity)
            {
                case LBoL.Base.Rarity.Common:
                    return name.WrapHex(common);
                case LBoL.Base.Rarity.Uncommon:
                    return name.WrapHex(uncommon);
                case LBoL.Base.Rarity.Shining:
                case LBoL.Base.Rarity.Mythic:
                case LBoL.Base.Rarity.Rare:
                    return name.WrapHex(rare);
                default:
                    break;
            }

            return name.WrapHex(defaultColor);
        }

        public static string WrapHex(this string text, string hex)
        {
            return string.Concat(new string[]
            {
                "<color=",
                hex,
                ">",
                text,
                "</color>"
            });
        }

    }
}


