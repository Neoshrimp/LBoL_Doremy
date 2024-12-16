using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.VanillaTweaks
{
    public sealed class DC_VanillaLocHelperDef : DStatusEffectDef
    {

        public override Sprite LoadSprite() => null;
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            return con;
        }
    }

    [EntityLogic(typeof(DC_VanillaLocHelperDef))]
    public sealed class DC_VanillaLocHelper : DStatusEffect
    {
        public static string GetLoc(string key, object[] toFormat = null)
        {
            var rez = TypeFactory<StatusEffect>.LocalizeProperty(nameof(DC_VanillaLocHelper), key, true, true);
            if (rez == null)
                return "";
            if (toFormat != null)
                rez = string.Format(rez, toFormat);
            return rez;
        }
    }
}
