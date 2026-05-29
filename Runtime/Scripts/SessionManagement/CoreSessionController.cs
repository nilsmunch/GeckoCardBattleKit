using LizardCards.GamePhases;
using LizardKit.Scaffolding;
using Newtonsoft.Json.Linq;

namespace LizardCards.SessionManagement
{
    public abstract class CoreSessionController<T> : BaseManager<T>
        where T : CoreSessionController<T>
    {
        public virtual void UpdateFromFeed(JObject jObject)
        {
            var gameData = jObject["game"] as JObject;
            var playerData = jObject["player"] as JObject;
            var phaseData = jObject["phase"] as JObject;
            
            var currentPhase = gameData!.Value<string>("phase");
            
            var handler = PhaseHandlerCollection.PhaseControllers.Find(a => a.ResponsibleForPhase() == currentPhase);
            if (handler == null)
            {
                LogError("No handler found for " + currentPhase);
                return;
            }

            handler.HandleGamePhase(gameData, playerData, phaseData);
        }

        protected void InformStateChange(string state)
        {
            foreach (var phaseHandler in PhaseHandlerCollection.PhaseControllers)
            {
                phaseHandler.OnChangeState(state);
            }
        }
    }
}