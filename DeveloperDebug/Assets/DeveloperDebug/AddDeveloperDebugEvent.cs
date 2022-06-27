using UnityEngine.Events;

namespace DeveloperDebug
{
    using UnityEngine;

    public class AddDeveloperDebugEvent : MonoBehaviour
    {
        public DeveloperDebugSettingData dataAdd;
        public UnityEvent debugEvent;

        private void OnEnable()
        {
#if !UNITY_EDITOR
            if(dataAdd.editorOnly) return;
#endif
            if (debugEvent.GetPersistentEventCount() == 0) return;
            if (!string.IsNullOrEmpty(dataAdd.keyCode))
            {
                DeveloperDebugExtension.RegisterKeyCode(dataAdd.keyCode, () => debugEvent.Invoke(),dataAdd.editorOnly);
            }

            if (!string.IsNullOrEmpty(dataAdd.touchCode))
            {
                DeveloperDebugExtension.RegisterTouchDebug(dataAdd.touchCode, () => debugEvent.Invoke(),dataAdd.editorOnly);
            }
        }

        private void OnDisable()
        {
            DeveloperDebugExtension.UnregisterKeyCode(dataAdd.keyCode, dataAdd.editorOnly);
            DeveloperDebugExtension.UnregisterTouchDebug(dataAdd.touchCode, dataAdd.editorOnly);
        }
    }
}