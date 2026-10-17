using UnityEngine;

namespace LizardCards.Network
{
    public class VirtualSingleton<T>
        where T : VirtualSingleton<T>, new()
    {
        private static T _sInstance;

        public bool DebugLog;

        public static T Instance
        {
            get
            {
                if (_sInstance == null) CreateAndLoad();
                return _sInstance;
            }
        }

        private static void CreateAndLoad()
        {
            if (!CardCore.Initialized()) return;
        
            _sInstance = new T();
            _sInstance.Prepare();
        }

        protected virtual void Prepare()
        {
            
        }
        
        
        #region LOGGING

        // ReSharper disable Unity.PerformanceAnalysis
        public void Log(string log)
        {
#if UNITY_EDITOR
            if (DebugLog) Debug.Log("<color=#00FFFF><b>"+typeof(T)+"</b></color>: "+log);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void LogError(string log)
        {
#if UNITY_EDITOR
            if (DebugLog) Debug.LogError("<color=#00FFFF><b>"+typeof(T)+"</b></color>: "+log);
#endif
        }
        

        #endregion
    }
}