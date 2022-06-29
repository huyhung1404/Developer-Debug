namespace DeveloperDebug.Editor
{
    using UnityEditor;
    using Core;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    [CustomEditor(typeof(DeveloperDebugSetting))]
    public class DeveloperDebugSettingEditor : Editor
    {
        private DeveloperDebugSetting m_Setting;
        private MethodInfo[] m_MethodInfo;
        private readonly List<int> m_EnableList = new List<int>();
        private readonly List<int> m_EditorOnlyList = new List<int>();
        private readonly List<int> m_DisableList = new List<int>();

        public override void OnInspectorGUI()
        {
            m_Setting = (DeveloperDebugSetting) target;
            serializedObject.Update();
            GUILayout.Space(5);
            if (GUILayout.Button("Config", GUICustomStyle.StandardButtonStyle))
            {
                m_Setting.showConfig = !m_Setting.showConfig;
            }

            if (m_Setting.showConfig)
            {
                EditorGUILayout.BeginVertical(GUICustomStyle.BorderAreaStyle);
                DrawEnableForBuild();
                m_Setting.useDefaultTouchCodeForKeyCode = EditorGUILayout.ToggleLeft("Use Default Touch Code For Key Code", m_Setting.useDefaultTouchCodeForKeyCode);
                GUILayout.Space(8);
                EditorGUILayout.LabelField("Key Code Config", GUICustomStyle.CenteredBigLabel);
                EditorGUILayout.LabelField("Waiting Time For Each Press");
                m_Setting.waitingTimeForEachPress = EditorGUILayout.FloatField(m_Setting.waitingTimeForEachPress);
                EditorGUILayout.LabelField("Min Length Key Code");
                m_Setting.minLengthKeyCode = EditorGUILayout.IntField(m_Setting.minLengthKeyCode);
                GUILayout.Space(8);
                EditorGUILayout.LabelField("Touch Code Config", GUICustomStyle.CenteredBigLabel);
                EditorGUILayout.LabelField("Number Of Touches Required To Enter Debug Mode");
                m_Setting.numberOfTouchesRequiredToEnterDebugMode = EditorGUILayout.IntField(m_Setting.numberOfTouchesRequiredToEnterDebugMode);
                EditorGUILayout.LabelField("Longest Time Waiting For Next Touch Check");
                m_Setting.longestTimeWaitingForNextTouchCheck = EditorGUILayout.FloatField(m_Setting.longestTimeWaitingForNextTouchCheck);
                EditorGUILayout.LabelField("Longest Time Holding Touch");
                m_Setting.longestTimeHoldingTouch = EditorGUILayout.FloatField(m_Setting.longestTimeHoldingTouch);
                EditorGUILayout.LabelField("Min Length Touch Code");
                m_Setting.minLengthTouchCode = EditorGUILayout.IntField(m_Setting.minLengthTouchCode);
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);
            if (m_MethodInfo == null) UpdateDictionary();
            DrawDictionary(m_Setting.debugData);
            if (!GUI.changed) return;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void DrawEnableForBuild()
        {
            var _scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            var _enableForBuild = _scriptingDefineSymbols.Contains("DEVELOPER_DEBUG");
            var _storeEnableValue = _enableForBuild;
            _enableForBuild = EditorGUILayout.ToggleLeft("Enable For Build", _enableForBuild);
            if (_storeEnableValue == _enableForBuild) return;
            if (_enableForBuild)
            {
                _scriptingDefineSymbols.Add("DEVELOPER_DEBUG");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,_scriptingDefineSymbols.ToArray());
                return;
            }

            _scriptingDefineSymbols.Remove("DEVELOPER_DEBUG"); PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,_scriptingDefineSymbols.ToArray());
        }

        private void DrawDictionary(List<DeveloperDebugSettingData> funcData)
        {
            m_EnableList.Clear();
            m_EditorOnlyList.Clear();
            m_DisableList.Clear();
            var _funcCount = funcData.Count;
            for (var i = 0; i < _funcCount; i++)
            {
                var _developerDebugFunc = funcData[i];
                if (!_developerDebugFunc.enable)
                {
                    m_DisableList.Add(i);
                    continue;
                }

                if (_developerDebugFunc.editorOnly)
                {
                    m_EditorOnlyList.Add(i);
                    continue;
                }

                m_EnableList.Add(i);
            }

            var _count = m_EnableList.Count;
            if (GUILayout.Button($"Enable ({_count})", GUICustomStyle.StandardButtonStyle))
            {
                m_Setting.showEnableList = !m_Setting.showEnableList;
            }

            EditorGUILayout.Space(10);
            if (m_Setting.showEnableList)
            {
                for (var i = 0; i < _count; i++)
                {
                    DrawData(funcData[m_EnableList[i]]);
                }
            }

            _count = m_EditorOnlyList.Count;
            if (GUILayout.Button($"Editor Only ({_count})", GUICustomStyle.StandardButtonStyle))
            {
                m_Setting.showEditorOnlyList = !m_Setting.showEditorOnlyList;
            }

            EditorGUILayout.Space(10);
            if (m_Setting.showEditorOnlyList)
            {
                for (var i = 0; i < _count; i++)
                {
                    DrawData(funcData[m_EditorOnlyList[i]]);
                }
            }

            _count = m_DisableList.Count;
            if (GUILayout.Button($"Disable ({_count})", GUICustomStyle.StandardButtonStyle))
            {
                m_Setting.showDisableList = !m_Setting.showDisableList;
            }

            EditorGUILayout.Space(10);
            if (m_Setting.showDisableList)
            {
                for (var i = 0; i < _count; i++)
                {
                    DrawData(funcData[m_DisableList[i]]);
                }
            }
        }

        private void DrawData(DeveloperDebugSettingData data)
        {
            EditorGUILayout.BeginVertical(GUICustomStyle.BorderAreaStyle);
            EditorGUILayout.LabelField(data.functionName, GUICustomStyle.CenteredBigLabel);
            EditorGUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();
            data.enable = EditorGUILayout.ToggleLeft("Enable", data.enable, GUICustomStyle.MiddleLeftBoldMiniLabel, GUILayout.MaxWidth(80));
            GUI.enabled = data.enable;
            data.editorOnly = EditorGUILayout.ToggleLeft("Editor Only", data.editorOnly,GUICustomStyle.MiddleLeftBoldMiniLabel, GUILayout.MaxWidth(80));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("KeyCode", GUICustomStyle.MiddleLeftBoldMiniLabel);
            data.keyCode = EditorGUILayout.TextField(data.keyCode, GUICustomStyle.EditTextFieldStyle);
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("TouchCode", GUICustomStyle.MiddleLeftBoldMiniLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;

            data.touchCode = EditorGUILayout.TextField(data.touchCode, GUICustomStyle.EditTextFieldStyle);

            GUI.enabled = data.enable;
            if (GUILayout.Button("Edit", GUICustomStyle.EditButtonStyle))
            {
                data.editTouchCode = !data.editTouchCode;
            }

            EditorGUILayout.EndHorizontal();
            if (data.editTouchCode) DrawButtonTouch(data);
            GUI.enabled = true;
            if (data.enable) CheckCorrect(data.keyCode, data.touchCode, m_Setting.debugData, m_Setting.minLengthKeyCode,m_Setting.minLengthTouchCode);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        private void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += UpdateDictionary;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= UpdateDictionary;
        }

        private void UpdateDictionary()
        {
            m_MethodInfo = typeof(DeveloperData).GetMethods(BindingFlags.Static | BindingFlags.Public);
            var _oldData = ((DeveloperDebugSetting) target).debugData;
            var _newData = new List<DeveloperDebugSettingData>();
            var _methodCount = m_MethodInfo.Length;
            for (var i = 0; i < _methodCount; i++)
            {
                var _data = _oldData.Find(item => string.Equals(item.functionName, m_MethodInfo[i].Name));
                if (_data != null)
                {
                    _newData.Add(_data);
                    continue;
                }

                _newData.Add(new DeveloperDebugSettingData(m_MethodInfo[i].Name));
            }

            ((DeveloperDebugSetting) target).debugData = _newData;
        }

        public static void CheckCorrect(string keyCode, string touchCode, List<DeveloperDebugSettingData> data,int minLengthKeyCode,int minLengthTouchCode, int eventCount = 1)
        {
            var _keyCodeLength = string.IsNullOrEmpty(keyCode) ? 0 : keyCode.Length;
            var _touchCodeLenght = string.IsNullOrEmpty(touchCode) ? 0 : touchCode.Length;
            if (_keyCodeLength == 0 && _touchCodeLenght == 0 || eventCount == 0)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("This function will not be called", MessageType.Warning);
                return;
            }

            if (_keyCodeLength > 0 && _keyCodeLength < minLengthKeyCode)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox($"The key code is case sensitive and at least {minLengthKeyCode} characters", MessageType.Error);
            }

            if (_touchCodeLenght > 0 && _touchCodeLenght < minLengthTouchCode)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox($"The touch code is at least {minLengthTouchCode} characters", MessageType.Error);
            }

            if (_keyCodeLength >= minLengthKeyCode && data.Count(item => item.enable && string.Equals(item.keyCode, keyCode)) > 1)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("This key code has been used", MessageType.Error);
            }

            if (_touchCodeLenght >= minLengthTouchCode && data.Count(item => item.enable && string.Equals(item.touchCode, touchCode)) > 1)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("This touch code has been used", MessageType.Error);
            }
        }

        public static void DrawButtonTouch(DeveloperDebugSettingData data)
        {
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("1", GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += '1';
            }

            if (GUILayout.Button("2", GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += '2';
            }

            if (GUILayout.Button("3", GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += '3';
            }

            if (GUILayout.Button("4", GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += '4';
            }

            if (GUILayout.Button("X", GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode = string.Empty;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}