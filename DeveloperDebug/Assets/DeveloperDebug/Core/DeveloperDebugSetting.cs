using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DeveloperDebug.Core
{
    [Serializable]
    public class DeveloperDebugSetting : ScriptableObject
    {
        public float waitingTimeForEachPress = 1;
        public int numberOfTouchesRequiredToEnterDebugMode = 5;
        public float longestTimeWaitingForNextTouchCheck = 1.5f;
        public float timeHoldingTouch = 0.3f;
        public List<DeveloperDebugSettingData> debugData;
        private Dictionary<string, Action> m_KeyCodeData;
        private Dictionary<int, Action> m_TouchCodeData;
        
        private void OnEnable()
        {
            m_KeyCodeData = new Dictionary<string, Action>();
            m_TouchCodeData = new Dictionary<int, Action>();
            var methods = typeof(DeveloperData).GetMethods(BindingFlags.Static | BindingFlags.Public);
            for (var i = methods.Length - 1; i >= 0; i--)
            {
                var method = methods[i];
                var developerFuncData = debugData.Find(item => string.Equals(item.functionName,method.Name));
                if (developerFuncData != null)
                {
                    if(!developerFuncData.enable) continue;
#if !UNITY_EDITOR
                    if(developerFuncData.editorOnly) continue;
#endif
                    Action action = null;
                    // if (!string.IsNullOrEmpty(developerFuncData.keyCode))
                    // {
                    //     action = (Action) Delegate.CreateDelegate(typeof(Action), method);
                    //     m_KeyCodeData.Add(developerFuncData.keyCode,action);
                    // }
                    //
                    // if (!string.IsNullOrEmpty(developerFuncData.touchCode))
                    // {
                    //     if(action == null) action = (Action) Delegate.CreateDelegate(typeof(Action), method);
                    //     m_TouchData.Add(developerFuncData.touchCode,action);
                    // }
                }
            }
        }

        public Dictionary<string,Action> GetKeyCodeData()
        {
            return m_KeyCodeData;
        }

        public Dictionary<int,Action> GetTouchData()
        {
            return new Dictionary<int, Action>
            {
                {2234, () =>
                {
                    Debug.LogError("RUn");
                }}
            };
            return m_TouchCodeData;
        }
    }

    [Serializable]
    public class DeveloperDebugSettingData
    {
        public bool enable;
        public string functionName;
        public string keyCode;
        public string touchCode;
        public bool editorOnly;
        public bool editTouchCode;

        public DeveloperDebugSettingData(string functionName)
        {
            this.functionName = functionName;
        }
    }
}
