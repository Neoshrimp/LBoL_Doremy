using HarmonyLib;
using JetBrains.Annotations;
using LBoL.Base.Extensions;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Normal;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoL_Doremy.DoremyChar.Actions;
using LBoL_Doremy.DoremyChar.SE;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.RootTemplates
{

    public struct NightmareInfo
    {
        public float toApply;

        public static explicit operator int(NightmareInfo nightmareInfo) => nightmareInfo.toApply.RoundToInt(MidpointRounding.AwayFromZero);


        public static implicit operator float(NightmareInfo nightmareInfo) => nightmareInfo.toApply;

        public static implicit operator NightmareInfo(int _int) => new NightmareInfo(_int);

        public static implicit operator NightmareInfo(float _float) => new NightmareInfo(_float);



        public NightmareInfo(float toApply)
        {
            this.toApply = toApply;
        }

        public override string ToString()
        {
            return toApply.ToString();
        }
    }

    public class DFormatterCard : GameEntityFormatWrapper
    {

        readonly Card.CardFormatWrapper cardFormatWrapper = null;
        readonly Card card;

        static Unit _fakeUnit;
        public static Unit FakeUnit
        {
            get
            {
                if (_fakeUnit == null)
                    _fakeUnit = Library.CreateEnemyUnit<WhiteFairy>();
                return _fakeUnit;
            }
        }

        public DFormatterCard(Card card, Card.CardFormatWrapper ogWrapper) : base(card)
        {
            cardFormatWrapper = ogWrapper;
            this.card = card;
        }

        public DFormatterCard(Card card) : base(card)
        {
            cardFormatWrapper = new Card.CardFormatWrapper(card);
            this.card = card;
        }



        public override string FormatArgument(object arg, string format)
        {
            if (arg is NightmareInfo nm)
            {
                var battle = card.Battle;
                if (battle == null)
                    return WrappedFormatNumber((int)nm, (int)nm, format);

                var target = card.PendingTarget;
                if (target == null)
                {
                    target = FakeUnit;
                }


                var nigtmareEventArgs = new NightmareArgs()
                {
                    source = battle.Player,
                    target = target,
                    level = nm,
                };
                nigtmareEventArgs.ActionSource = card;
                nigtmareEventArgs.Cause = LBoL.Core.Battle.ActionCause.OnlyCalculate;

                EventManager.GetDoremyEvents(battle).nightmareEvents.nigtmareApplying.Execute(nigtmareEventArgs);

                return WrappedFormatNumber((int)nm, (int)nigtmareEventArgs.level, format);

            }


            return cardFormatWrapper.FormatArgument(arg, format);
        }
    }
}
