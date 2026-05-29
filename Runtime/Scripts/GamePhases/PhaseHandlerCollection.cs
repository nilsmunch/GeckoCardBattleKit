using System.Collections.Generic;

namespace LizardCards.GamePhases
{
    public static class PhaseHandlerCollection
    {
        public static List<IGamePhaseHandler> PhaseControllers = new();

        public static void Register(IGamePhaseHandler handler)
        {
            PhaseControllers.Add(handler);
        }

    }
}