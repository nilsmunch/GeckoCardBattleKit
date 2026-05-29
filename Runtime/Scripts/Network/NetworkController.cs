using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LizardKit.DebugButton;
using LizardKit.Scaffolding;
using Newtonsoft.Json.Linq;
using UnityEngine;

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
        public void Test()
        {
            FetchJsonAsync("/ping", s => Log(s.ToString()));
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

        // ReSharper disable Unity.PerformanceAnalysis
        public async void FetchJsonAsync(string uri, Action<JObject> onSuccess, Action<Exception> onError = null, CancellationToken ct = default)
        {
            if (uri == _currentFetchCall) return;
            _currentFetchCall = uri;
            var url = GetBaseUrl() + uri;

            try
            {
                var obj = await FetchJson(url, ct);
                onSuccess?.Invoke(obj);
                _currentFetchCall = null;
            }
            catch (ApiException ex)
            {
                LogError($"{url} Status: {ex.StatusCode}");
                LogError($"Message: {ex.Message}");
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
        
        

        // ReSharper disable Unity.PerformanceAnalysis
        public async void PostJsonAsync(string uri, HttpContent posting,Action<JObject> onSuccess, Action<Exception> onError = null, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(uri) || uri == _currentPostCall) return;
            _currentPostCall = uri;
            var url = GetBaseUrl() + FillUriDetails(uri);
            try
            {
                var obj = await PostJson(url,
                    posting, ct);
                onSuccess?.Invoke(obj);
            }
            catch (ApiException ex)
            {
                LogError($"{url} Status: {ex.StatusCode}");
                LogError($"Message: {ex.Message}");
            }
            _currentPostCall = null;
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
