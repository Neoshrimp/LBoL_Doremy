using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Presentation;
using LBoL.Presentation.UI.Widgets;
using LBoL_Doremy.Actions;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.Cards.Rare;
using LBoL_Doremy.DoremyChar.Keywords;
using LBoL_Doremy.RootTemplates;
using LBoLEntitySideloader.CustomHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LBoL_Doremy.DoremyChar.DreamManagers
{
    public class NaturalDreamLayerCard : DCard
    {


        protected int DreamLevel => this.GetCustomKeyword<DLKeyword>(DoremyKw.dLId).DreamLevel;

        public override void Initialize()
        {
            base.Initialize();
            this.AddCustomKeyword(DoremyKw.NewDreamLayer);
            this.AddCustomKeyword(DoremyKw.NewDLKeyword);

        }


        public virtual void OnDLChanged(DreamLevelArgs args) { }



        static HashSet<string> allDreamLayerCards;
        public static HashSet<string> AllDreamLayerCards
        {
            get
            {
                if (allDreamLayerCards == null)
                {
                    allDreamLayerCards = new HashSet<string>();
                    var ass = typeof(NaturalDreamLayerCard).Assembly;
                    ass.ExportedTypes.Where(t => t.IsSubclassOf(typeof(NaturalDreamLayerCard))).Select(t => t.Name).Do(id => allDreamLayerCards.Add(id));
                }
                return allDreamLayerCards;
            }
        }
    }
}
