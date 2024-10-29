using LBoL.Core;
using LBoL.Core.Cards;

namespace LBoL_Doremy.Actions
{
    public class DreamLevelArgs : GameEventArgs
    {
        public Card target;
        public int dreamLevelDelta = 1;

        public bool isEndOfTurnBounce = false;

        public DreamLevelArgs() { }

        public DreamLevelArgs(bool isEndOfTurnBounce)
        {
            this.isEndOfTurnBounce = isEndOfTurnBounce;
        }

        public override string GetBaseDebugString()
        {
            return $"Applying {dreamLevelDelta} Dream Level to {target.Name}";
        }
    }
}