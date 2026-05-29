using Newtonsoft.Json.Linq;

namespace SunfallRaiders.Network
{
    public interface IGamePhaseHandler
    {
        public void HandleGamePhase(JObject gameData, JObject playerData, JObject phaseData);

        public abstract string ResponsibleForPhase();
    }
}