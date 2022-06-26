using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeveloperDebug
{
    [Serializable]
    public class DeveloperDebugSetting : ScriptableObject
    {
        public bool enableForBuild;
        public DeveloperDebugSettingData[] data;
        private Dictionary<string, Action> m_KeyCodeData = new Dictionary<string, Action>();
        private Dictionary<string, Action> m_TouchData = new Dictionary<string, Action>();
        
        private void OnEnable()
        {
            var methods = typeof(DeveloperData).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList();
            for (var i = data.Length - 1; i >= 0; i--)
            {
                var developerFuncData = data[i];
#if !UNITY_EDITOR
                if(developerFuncData.editorOnly) return;
#endif
                var method = methods.Find(item => item.Name.Equals(developerFuncData.methodName));
                if(method == null) return;
                Action action = null;
                if (!string.IsNullOrEmpty(developerFuncData.keyCode))
                {
                    action = (Action) Delegate.CreateDelegate(typeof(Action), method);
                    m_KeyCodeData.Add(developerFuncData.keyCode,action);
                }

                if (!string.IsNullOrEmpty(developerFuncData.touch))
                {
                    if(action == null) action = (Action) Delegate.CreateDelegate(typeof(Action), method);
                    m_TouchData.Add(developerFuncData.touch,action);
                }
            }
        }

        public Dictionary<string,Action> GetKeyCodeData()
        {
            return m_KeyCodeData;
        }

        public Dictionary<string,Action>  GetTouchData()
        {
            return m_TouchData;
        }
    }

    [Serializable]
    public class DeveloperDebugSettingData
    {
        public string methodName;
        public string keyCode;
        public string touch;
        public bool editorOnly;
    }
}
