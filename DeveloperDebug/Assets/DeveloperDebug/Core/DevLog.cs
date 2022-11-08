using System.Diagnostics;
using UnityEngine;

namespace DeveloperDebug.Core
{
    public static class DevLog
    {
        [Conditional("DEV_LOG")]
        public static void Log<T>(T target)
        {
            UnityEngine.Debug.Log(JsonUtility.ToJson(target));
        }
        [Conditional("DEV_LOG")]
        public static void LogWarning<T>(T target)
        {
            UnityEngine.Debug.LogWarning(JsonUtility.ToJson(target));
        }
        [Conditional("DEV_LOG")]
        public static void LogError<T>(T target)
        {
            UnityEngine.Debug.LogError(JsonUtility.ToJson(target));
        }
    }
}
