using System;
using System.Threading.Tasks;
using LizardCards.SessionManagement;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LizardCards.Network
{
    public static class TaskCallbackExtensions
    {
        public static void UpdateGame<TController>(this Task<JObject> task)
            where TController : CoreSessionController<TController>
        {
            task.Then(response =>
            {
                CoreSessionController<TController>.Instance.UpdateFromFeed(response);
            });
        }
        
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
                Debug.LogError(ex);
                onError?.Invoke(ex);
            }
        }
    }
}