using LBoL.Core;
using LBoL_Doremy.Actions;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public class DoremyEvents
    {
        public DLEvents dLEvents = new DLEvents();
    }

    public class DLEvents
    {
        public GameEvent<DreamLevelArgs> applyingDL = new GameEvent<DreamLevelArgs>();
        public GameEvent<DreamLevelArgs> appliedDL = new GameEvent<DreamLevelArgs>();

    }

}
