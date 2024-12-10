using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using LBoL.Core.Battle.BattleActions;
using LBoL_Doremy.DoremyChar.BattleTracking;
using LBoL_Doremy.CreatedCardTracking;
using System.Threading.Tasks.Sources;
using System.Linq;
using LBoL.Core.Cards;

namespace LBoL_Doremy.DoremyChar.Cards.Uncommon
{
    public sealed class DoremyNightmareAoEDef : DCardDef
    {
        public override CardConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Rarity = Rarity.Uncommon;

            con.Type = LBoL.Base.CardType.Attack;
            con.TargetType = TargetType.AllEnemies;

            con.Colors = new List<ManaColor>() { ManaColor.White };
            con.Cost = new ManaGroup() { Any = 1, White = 1 };
            con.UpgradedCost = new ManaGroup() { Any = 1};


            con.Damage = 6;
            con.UpgradedDamage = 8;


            con.Keywords = Keyword.Exile;
            con.UpgradedKeywords = Keyword.Exile | Keyword.Accuracy;

            con.RelativeEffects = new List<string>() { nameof(DC_NightmareSE) };
            con.UpgradedRelativeEffects = new List<string>() { nameof(DC_NightmareSE) };


            return con;
        }
    }


    [EntityLogic(typeof(DoremyNightmareAoEDef))]
    public sealed class DoremyNightmareAoE : DCard
    {
        public int DmgEstimate
        {
            get
            {
                if (RealBattle == null)
                    return 0;

                return RealBattle.AllAliveUnits.Select(u =>
                    {
                        if (u.TryGetStatusEffect<DC_NightmareSE>(out var nightmareSE))
                            return nightmareSE.Level;
                        return 0;

                    }).Sum();   
            }
        }

        public override DamageInfo Damage
        {
            get
            {
                return base.Damage;
            }
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            int consumedNM = 0;
            foreach (var u in Battle.AllAliveUnits)
                if (u.TryGetStatusEffect<DC_NightmareSE>(out var nightmareSE))
                {
                    consumedNM += nightmareSE.Level;
                    yield return new RemoveStatusEffectAction(nightmareSE);
                }
                    
            yield return AttackAction(selector, Damage.IncreaseBy(consumedNM));

        }
    }
}
