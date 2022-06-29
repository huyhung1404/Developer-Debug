namespace DeveloperDebug.Core
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine;
    
    public static class DeveloperDebugExtension
    {
        private static readonly Regex m_TouchCodeRegex = new Regex(@"^[1234]*$");
        
        public static void RegisterKeyCode(string key, Action action)
        {
#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            if(string.IsNullOrEmpty(key)) return;
            DeveloperDebugKeyCode.Register(key, action);
#endif
        }

        public static void RegisterKeyCodeRunOnEditorOnly(string key, Action action)
        {
#if UNITY_EDITOR
            if(string.IsNullOrEmpty(key)) return;
            DeveloperDebugKeyCode.Register(key, action);
#endif
        }

        public static void UnregisterKeyCode(string key)
        {
#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            if(string.IsNullOrEmpty(key)) return;
            DeveloperDebugKeyCode.Unregister(key);
#endif
        }
        
        public static void RegisterTouchCode(int key, Action action)
        {
#if UNITY_EDITOR
            RegisterTouchCodeRunOnEditorOnly(key,action);
#elif ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS))
            if (!m_TouchCodeRegex.IsMatch(key.ToString()))
            {
                Debug.LogError("Touch code is not in the correct format");
                return;
            }
            DeveloperDebugTouchCode.Register(key, action);
#endif
        }

        public static void RegisterTouchCodeRunOnEditorOnly(int key, Action action)
        {
#if UNITY_EDITOR
            if (!m_TouchCodeRegex.IsMatch(key.ToString()))
            {
                Debug.LogError("Touch code is not in the correct format");
                return;
            }
            DeveloperDebugKeyCode.Register(key.ToString(), action);
#endif
        }

        public static void UnregisterTouchCode(int key)
        {
#if UNITY_EDITOR
            DeveloperDebugKeyCode.Unregister(key.ToString());
#elif ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS))
            DeveloperDebugTouchCode.Unregister(key);
#endif
        }
    }
}