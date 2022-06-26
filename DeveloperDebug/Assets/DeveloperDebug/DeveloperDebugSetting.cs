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
        public List<DeveloperDebugSettingData> data;
        private Dictionary<string, Action> m_KeyCodeData;
        private Dictionary<string, Action> m_TouchData;
        
//         private void OnEnable()
//         {
//             if (data == null) data = new Dictionary<string, DeveloperDebugSettingData>();
//             m_KeyCodeData = new Dictionary<string, Action>();
//             m_TouchData = new Dictionary<string, Action>();
//             var methods = typeof(DeveloperData).GetMethods(BindingFlags.Static | BindingFlags.Public);
//             for (var i = methods.Length - 1; i >= 0; i--)
//             {
//                 var method = methods[i];
//                 if (data.ContainsKey(method.Name))
//                 {
//                     var developerFuncData = data[method.Name];
// #if !UNITY_EDITOR
//                     if(developerFuncData.editorOnly) return;
// #endif
//                     Action action = null;
//                     if (!string.IsNullOrEmpty(developerFuncData.keyCode))
//                     {
//                         action = (Action) Delegate.CreateDelegate(typeof(Action), method);
//                         m_KeyCodeData.Add(developerFuncData.keyCode,action);
//                     }
//
//                     if (!string.IsNullOrEmpty(developerFuncData.touchCode))
//                     {
//                         if(action == null) action = (Action) Delegate.CreateDelegate(typeof(Action), method);
//                         m_TouchData.Add(developerFuncData.touchCode,action);
//                     }
//                 }
//             }
//         }

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
        public string actionName;
        public string keyCode;
        public string touchCode;
        public bool editorOnly;
    }
}
