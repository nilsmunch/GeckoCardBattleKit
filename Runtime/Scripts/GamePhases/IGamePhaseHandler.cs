using Newtonsoft.Json.Linq;

namespace LizardCards.GamePhases
{
    public interface IGamePhaseHandler
    {
        public void HandleGamePhase(JObject gameData, JObject playerData, JObject phaseData);
        
        public void OnChangeState(string state);
        
        public string ResponsibleForPhase();
    }
}