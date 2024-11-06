using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL_Doremy.DoremyChar.SE;
using LBoL_Doremy.RootTemplates;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public class NightmareArgs : GameEventArgs
    {
        public Unit source;
        public Unit target;
        public NightmareInfo level;
        public float occupationTime;

        public DC_NightmareSE appliedNightmare;


        public override string GetBaseDebugString()
        {
            return $"{source?.Name} applying {level} Nightmare to {target?.Name}";
        }
    }
}