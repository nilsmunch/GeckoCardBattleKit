using LizardKit.Scaffolding;
using Newtonsoft.Json.Linq;

namespace LizardCards.SessionManagement
{
    public abstract class CoreSessionController<T> : BaseManager<T>
        where T : CoreSessionController<T>
    {
        public abstract void UpdateFromFeed(JObject jObject);

    }
}