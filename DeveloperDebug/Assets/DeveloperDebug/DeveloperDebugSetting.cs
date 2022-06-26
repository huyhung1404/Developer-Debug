using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DeveloperDebug
{
    [Serializable]
    public class DeveloperDebugSetting : ScriptableObject
    {
        public bool enableForBuild;
        public List<DeveloperDebugSettingData> debugData;
        private Dictionary<string, Action> m_KeyCodeData;
        private Dictionary<string, Action> m_TouchData;
        
        private void OnEnable()
        {
            m_KeyCodeData = new Dictionary<string, Action>();
            m_TouchData = new Dictionary<string, Action>();
            var methods = typeof(DeveloperData).GetMethods(BindingFlags.Static | BindingFlags.Public);
            for (var i = methods.Length - 1; i >= 0; i--)
            {
                var method = methods[i];
                var developerFuncData = debugData.Find(item => item.functionName.Equals(method.Name));
                if (developerFuncData != null)
                {
                    if(!developerFuncData.enable) continue;
#if !UNITY_EDITOR
                    if(developerFuncData.editorOnly) continue;
#endif
                    Action action = null;
                    if (!string.IsNullOrEmpty(developerFuncData.keyCode))
                    {
                        action = (Action) Delegate.CreateDelegate(typeof(Action), method);
                        m_KeyCodeData.Add(developerFuncData.keyCode,action);
                    }

                    if (!string.IsNullOrEmpty(developerFuncData.touchCode))
                    {
                        if(action == null) action = (Action) Delegate.CreateDelegate(typeof(Action), method);
                        m_TouchData.Add(developerFuncData.touchCode,action);
                    }
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
