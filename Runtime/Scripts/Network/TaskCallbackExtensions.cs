using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LizardCards.Network
{
    public static class TaskCallbackExtensions
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public static async void Then(
            this Task<JObject> task,
            Action<JObject> onSuccess,
            Action<Exception> onError = null)
        {
            try
            {
                var result = await task;
                onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }
    }
}