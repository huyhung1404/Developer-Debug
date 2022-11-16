namespace DeveloperDebug.Core
{
    using UnityEngine;

    public static class DeveloperDebugManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialization()
        {
            if(PlayerPrefs.GetInt("Developer",0) == 0) return;
#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            DeveloperDebugKeyCode.Initialization();
#elif ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS)) && !UNITY_EDITOR
            DeveloperDebugTouchCode.Initialization();
#endif
        }

        public static void BecomeDeveloper()
        {
            PlayerPrefs.SetInt("Developer",1);
#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            var debugCore = Object.FindObjectOfType<DeveloperDebugKeyCode>();
            if(debugCore == null) DeveloperDebugKeyCode.Initialization();
#elif ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS)) && !UNITY_EDITOR
            var debugCoreTouch = Object.FindObjectOfType<DeveloperDebugTouchCode>();            
            if(debugCoreTouch == null) DeveloperDebugTouchCode.Initialization();
#endif
        }

        public static void UnsubscribeDeveloper()
        {
            PlayerPrefs.SetInt("Developer",0);
#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            var debugCore = Object.FindObjectOfType<DeveloperDebugKeyCode>();
            if(debugCore != null) Object.Destroy(debugCore.gameObject);
#elif ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS)) && !UNITY_EDITOR
            var debugCoreTouch = Object.FindObjectOfType<DeveloperDebugTouchCode>();            
            if(debugCoreTouch != null) Object.Destroy(debugCoreTouch.gameObject);
#endif
        }
    }
}