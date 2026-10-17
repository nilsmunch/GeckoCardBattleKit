using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LizardCards.Network
{
    public interface INetworkController : IGameController
    {
        Task<JObject> PostJsonAsync(
            string uri,
            HttpContent posting,
            CancellationToken ct = default);

        Task<JObject> FetchJsonAsync(string uri, CancellationToken ct = default);
    }

    public interface IGameController
    {
        void Initialize();
    }
}