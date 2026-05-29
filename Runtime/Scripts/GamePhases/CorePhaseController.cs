using LizardCards.Network;
using LizardCards.SessionManagement;
using LizardKit.Scaffolding;
using Newtonsoft.Json.Linq;
using SunfallRaiders.Network;

namespace LizardCards.GamePhases
{
    public abstract class CorePhaseController<T, TGame> : BaseManager<T>
        where T : CorePhaseController<T, TGame>, IGamePhaseHandler
        where TGame : CoreSessionController<TGame>
    {
        protected JObject PhaseData;
        public JObject PlayerData;
        
        protected virtual void UpdateView()
        {
            //view.UpdateView();
        }
        protected virtual string PhaseName => GetType().Name.Replace("PhaseController", "");

        public virtual string ResponsibleForPhase() => PhaseName;
        
        public virtual void HandleGamePhase(JObject gameData, JObject playerData, JObject phaseData)
        {
            Log(phaseData.ToString());
            PhaseData = phaseData;
            PlayerData = playerData;
            UpdateView();
        }
        
        public virtual void OnChangeState(string state)
        {
            
        }
        
        public virtual void ProcessUrl(string url)
        {
            Log(url);
            NetworkController.Instance.PostJsonAsync(url,
                    null)
                .Then(response =>
                    {
                        CoreSessionController<TGame>.Instance.UpdateFromFeed(response);
                    },
                    e =>
                    {
                        LogError("Error in feed "+e);
                        UpdateView();
                    });
        }

    }
}