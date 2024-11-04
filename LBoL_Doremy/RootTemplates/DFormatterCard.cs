using JetBrains.Annotations;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Normal;
using LBoL_Doremy.DoremyChar.SE;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.RootTemplates
{

    public struct NightmareInfo
    {
        public int toApply;

        public static implicit operator int(NightmareInfo nightmareInfo) => nightmareInfo.toApply;

        public static implicit operator NightmareInfo(int _int) => new NightmareInfo(_int);


        public NightmareInfo(int toApply)
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
                    return WrappedFormatNumber(nm, nm, format);

                var target = card.PendingTarget;
                if (target == null)
                {
                    target = FakeUnit;
                }


                var nightmare = Library.CreateStatusEffect<DC_NightmareSE>();
                nightmare.Level = nm;
                var statusEffectArgs = new StatusEffectApplyEventArgs()
                {
                    Effect = nightmare,
                    Unit = target,
                    ActionSource = card,
                    Cause = LBoL.Core.Battle.ActionCause.OnlyCalculate
                };

                target.StatusEffectAdding.Execute(statusEffectArgs);


                return WrappedFormatNumber(nm, statusEffectArgs.Effect.Level, format);

            }


            return cardFormatWrapper.FormatArgument(arg, format);
        }
    }
}
