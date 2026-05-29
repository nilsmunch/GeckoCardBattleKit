using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LizardKit.DebugButton;
using LizardKit.Scaffolding;
using Newtonsoft.Json.Linq;

namespace LizardCards.Network
{
    public abstract class NetworkController : BaseManager<NetworkController>
    {
        private readonly HttpClient _client = new();
        
        protected abstract string BaseUrlLocal { get; }
        protected abstract string BaseUrlProd { get; }

        public ServiceOutlet serviceOutlet = ServiceOutlet.LOCAL;

        private string _currentFetchCall;
        private string _currentPostCall;
        
        [Button]
        public async Task Test()
        {
            var ping = await FetchJsonAsync("/ping");
            Log(ping.ToString());
        }

        protected virtual string FillUriDetails(string uri)
        {
            return uri;
        }

        protected override void Awake()
        {
            base.Awake();
#if !UNITY_EDITOR 
            serviceOutlet = ServiceOutlet.PRODUCTION;
#endif
            var accessToken = AccessTokenController.AccessToken;
            _client.DefaultRequestHeaders.Add("access_token", accessToken);
        }

        public async Task<JObject> FetchJsonAsync(string uri, CancellationToken ct = default)
        {
            if (uri == _currentFetchCall) return null;
            _currentFetchCall = uri;
            var url = GetBaseUrl() + FillUriDetails(uri);
            try
            {
                var obj = await FetchJson(url, ct);
                return obj;
            }
            catch (ApiException ex)
            {
                LogError($"{url} Status: {ex.StatusCode} Message: {ex.Message}");
                return null;
            }
            finally
            {
                _currentFetchCall = null;
            }
        }

        private string GetBaseUrl()
        {
            return serviceOutlet switch
            {
                ServiceOutlet.LOCAL => BaseUrlLocal,
                ServiceOutlet.PRODUCTION => BaseUrlProd,
                _ => BaseUrlLocal
            };
        }

        private async Task<JObject> FetchJson(string uri, CancellationToken ct = default) 
        {
            var response = await _client.GetAsync(uri, ct);
            var body = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    (int)response.StatusCode,
                    body
                );
            }
            
            return string.IsNullOrEmpty(body) ? null : JObject.Parse(body);
        }
        
        

        public async Task<JObject> PostJsonAsync(
            string uri, HttpContent posting, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(uri)) throw new ArgumentException("URI cannot be null or empty.", nameof(uri));

            if (uri == _currentPostCall) throw new InvalidOperationException($"Already posting to '{uri}'");

            _currentPostCall = uri;

            var url = GetBaseUrl() + FillUriDetails(uri);

            try
            {
                return await PostJson(url, posting, ct);
            }
            catch (ApiException ex)
            {
                LogError($"{url} Status: {ex.StatusCode} Message: {ex.Message}");
                throw;
            }
            finally
            {
                _currentPostCall = null;
            }
        }
        
        private async Task<JObject> PostJson(string uri, HttpContent posting, CancellationToken ct = default) 
        {
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            var response = await _client.PostAsync(uri, posting, ct);
            var body = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    (int)response.StatusCode,
                    body
                );
            }
            var parsedobject = JObject.Parse(body);
            return parsedobject;
        }
    }

    internal class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public enum ServiceOutlet
    {
        LOCAL,
        STAGING,
        PRODUCTION
    }
}
