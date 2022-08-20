namespace DeveloperDebug.Core
{
    using UnityEngine;
    using UnityEngine.Events;

    public class AddDeveloperDebugEvent : MonoBehaviour
    {
        public static bool UseDefaultTouchCodeForKeyCode;
        public DeveloperDebugSettingData dataAdd;
        public UnityEvent debugEvent;

        private void Start()
        {
#if !UNITY_EDITOR
            if(dataAdd.editorOnly) return;
#endif
            if (debugEvent.GetPersistentEventCount() == 0) return;
#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            if (!string.IsNullOrEmpty(dataAdd.keyCode))
            {
                DeveloperDebugExtension.RegisterKeyCode(dataAdd.keyCode,ActionExecute);
                return;
            }
            if (!UseDefaultTouchCodeForKeyCode || string.IsNullOrEmpty(dataAdd.touchCode)) return;
            DeveloperDebugExtension.RegisterTouchCode(int.Parse(dataAdd.touchCode),ActionExecute);
#elif ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS))
            DeveloperDebugExtension.RegisterTouchCode(int.Parse(dataAdd.touchCode),ActionExecute);
#endif
        }

        private void OnDisable()
        {
            DeveloperDebugExtension.UnregisterKeyCode(dataAdd.keyCode);
            DeveloperDebugExtension.UnregisterTouchCode(int.Parse(dataAdd.touchCode));
        }

        private void ActionExecute()
        {
            debugEvent.Invoke();
        }
    }
}