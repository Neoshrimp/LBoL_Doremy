using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.StaticResources
{
    internal static class Loc
    {
        internal static DirectorySource locSource = new DirectorySource(LBoL_Doremy.PInfo.GUID, "loc");

        public static string Cards = "Cards";
        public static string Exhibits = "Exhibits";
        public static string PlayerUnit = "PlayerUnit";
        public static string EnemiesUnit = "EnemyUnit";
        public static string UnitModel = "UnitModel";
        public static string UltimateSkills = "UltimateSkills";
        public static string StatusEffects = "StatusEffects";

        public static BatchLocalization CardsBatchLoc = new BatchLocalization(locSource, typeof(CardTemplate), Cards);
        public static BatchLocalization ExhibitsBatchLoc = new BatchLocalization(locSource, typeof(ExhibitTemplate), Exhibits);
        public static BatchLocalization PlayerUnitBatchLoc = new BatchLocalization(locSource, typeof(PlayerUnitTemplate), PlayerUnit);
        public static BatchLocalization EnemiesUnitBatchLoc = new BatchLocalization(locSource, typeof(EnemyUnitTemplate), EnemiesUnit);
        public static BatchLocalization UnitModelBatchLoc = new BatchLocalization(locSource, typeof(UnitModelTemplate), UnitModel);
        public static BatchLocalization UltimateSkillsBatchLoc = new BatchLocalization(locSource, typeof(UltimateSkillTemplate), UltimateSkills);
        public static BatchLocalization StatusEffectsBatchLoc = new BatchLocalization(locSource, typeof(StatusEffectTemplate), StatusEffects);

    }
}
