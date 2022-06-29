namespace DeveloperDebug.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    
    [Serializable]
    public class DeveloperDebugSetting : ScriptableObject
    {
        public float waitingTimeForEachPress = 1;
        public int numberOfTouchesRequiredToEnterDebugMode = 5;
        public float longestTimeWaitingForNextTouchCheck = 2f;
        public float longestTimeHoldingTouch = 0.75f;
        public bool useDefaultTouchCodeForKeyCode;
        public List<DeveloperDebugSettingData> debugData;
        public bool showConfig = true;
        public bool showEnableList = true;
        public bool showEditorOnlyList = true;
        public bool showDisableList = true;
        
#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
        public Dictionary<string,Action> GetKeyCodeData()
        {
            var _data = new Dictionary<string, Action>();
            var _methods = typeof(DeveloperData).GetMethods(BindingFlags.Static | BindingFlags.Public);
            for (var i = _methods.Length - 1; i >= 0; i--)
            {
                var _method = _methods[i];
                var _debugData = debugData.Find(item => string.Equals(item.functionName,_method.Name));
                if (_debugData != null)
                {
                    if(!_debugData.enable) continue;
#if !UNITY_EDITOR
                    if(_debugData.editorOnly) continue;
#endif
                    Action _action;
                    if (!string.IsNullOrEmpty(_debugData.keyCode))
                    {
                        if(_debugData.keyCode.Length < 5) continue;
                        _action = (Action) Delegate.CreateDelegate(typeof(Action), _method);
                        _data.Add(_debugData.keyCode,_action);
                        continue;
                    }

                    if (!useDefaultTouchCodeForKeyCode || string.IsNullOrEmpty(_debugData.touchCode) || int.Parse(_debugData.touchCode) <= 1000) continue;
                    _action = (Action) Delegate.CreateDelegate(typeof(Action), _method);
                    _data.Add(_debugData.touchCode,_action);
                }
            }
            return _data;
        }
#endif
        
#if ((DEVELOPER_DEBUG && UNITY_ANDROID) || (DEVELOPER_DEBUG && UNITY_IOS)) && !UNITY_EDITOR
        public Dictionary<int,Action> GetTouchCodeData()
        {
            var _data = new Dictionary<int, Action>();
            var _methods = typeof(DeveloperData).GetMethods(BindingFlags.Static | BindingFlags.Public);
            for (var i = _methods.Length - 1; i >= 0; i--)
            {
                var _method = _methods[i];
                var _debugData = debugData.Find(item => string.Equals(item.functionName,_method.Name));
                if (_debugData != null)
                {
                    if(!_debugData.enable) continue;
#if !UNITY_EDITOR
                    if(_debugData.editorOnly) continue;
#endif
                    if(string.IsNullOrEmpty(_debugData.touchCode)) continue;
                    var _code = int.Parse(_debugData.touchCode);
                    if (_code <= 1000) continue;
                    var _action = (Action) Delegate.CreateDelegate(typeof(Action), _method);
                    _data.Add(_code,_action);
                }
            }
            return _data;
        }
#endif
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
