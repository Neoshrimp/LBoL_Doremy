using LBoL.Core;
using LBoL_Doremy.Actions;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public class DoremyEvents
    {
        public DLEvents dLEvents = new DLEvents();
        public NightmareEvents nightmareEvents = new NightmareEvents();
    }

    public class DLEvents
    {
        public GameEvent<DreamLevelArgs> applyingDL = new GameEvent<DreamLevelArgs>();
        public GameEvent<DreamLevelArgs> appliedDL = new GameEvent<DreamLevelArgs>();

    }

    public class NightmareEvents
    {
        public GameEvent<NightmareArgs> nigtmareApplying = new GameEvent<NightmareArgs>();
        public GameEvent<NightmareArgs> nigtmareApplied = new GameEvent<NightmareArgs>();
    }

}
