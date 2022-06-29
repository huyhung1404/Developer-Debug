using DeveloperDebug.Core;

namespace DeveloperDebug.Editor
{
    using UnityEditor;
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
                EditorGUILayout.LabelField("Key Code Config",GUICustomStyle.CenteredBigLabel);
                EditorGUILayout.LabelField("Waiting Time For Each Press");
                m_Setting.waitingTimeForEachPress = EditorGUILayout.FloatField(m_Setting.waitingTimeForEachPress);
                GUILayout.Space(8);
                EditorGUILayout.LabelField("Touch Code Config",GUICustomStyle.CenteredBigLabel);
                EditorGUILayout.LabelField("Number Of Touches Required To Enter Debug Mode");
                m_Setting.numberOfTouchesRequiredToEnterDebugMode = EditorGUILayout.IntField(m_Setting.numberOfTouchesRequiredToEnterDebugMode);
                EditorGUILayout.LabelField("Longest Time Waiting For Next Touch Check");
                m_Setting.longestTimeWaitingForNextTouchCheck = EditorGUILayout.FloatField(m_Setting.longestTimeWaitingForNextTouchCheck);
                EditorGUILayout.LabelField("Longest Time Holding Touch");
                m_Setting.longestTimeHoldingTouch = EditorGUILayout.FloatField(m_Setting.longestTimeHoldingTouch);
                EditorGUILayout.EndVertical();   
            }
            GUILayout.Space(5);
            if (m_MethodInfo == null)
            {
                UpdateDictionary();
            }
            DrawDictionary(m_Setting.debugData);
            if(!GUI.changed) return;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
        private void DrawEnableForBuild()
        {
            var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            var enableForBuild = scriptingDefineSymbols.Contains("DEVELOPER_DEBUG");
            var storeEnableValue = enableForBuild;
            enableForBuild = EditorGUILayout.ToggleLeft("Enable For Build", enableForBuild);
            if (storeEnableValue == enableForBuild) return;
            if (enableForBuild)
            {
                scriptingDefineSymbols.Add("DEVELOPER_DEBUG");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,scriptingDefineSymbols.ToArray());
                return;
            }
            scriptingDefineSymbols.Remove("DEVELOPER_DEBUG");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,scriptingDefineSymbols.ToArray());
        }

        private void DrawDictionary(List<DeveloperDebugSettingData> funcData)
        {
            m_EnableList.Clear();
            m_EditorOnlyList.Clear();
            m_DisableList.Clear();
            var _count = funcData.Count;
            for (var i = 0; i < _count; i++)
            {
                var developerDebugFunc = funcData[i];
                if (!developerDebugFunc.enable)
                {
                    m_DisableList.Add(i);
                    continue;
                }
            
                if (developerDebugFunc.editorOnly)
                {
                    m_EditorOnlyList.Add(i);
                    continue;
                }
                m_EnableList.Add(i);
            }
            
            var count = m_EnableList.Count;
            if (GUILayout.Button($"Enable ({count})", GUICustomStyle.StandardButtonStyle))
            {
                m_Setting.showEnableList = !m_Setting.showEnableList;
            }
            EditorGUILayout.Space(10);
            if (m_Setting.showEnableList)
            {
                for (var i = 0; i < count; i++)
                {
                    DrawData(funcData[m_EnableList[i]]);
                }
            }

            count = m_EditorOnlyList.Count;
            if (GUILayout.Button($"Editor Only ({count})", GUICustomStyle.StandardButtonStyle))
            {
                m_Setting.showEditorOnlyList = !m_Setting.showEditorOnlyList;
            }
            EditorGUILayout.Space(10);
            if (m_Setting.showEditorOnlyList)
            {
                for (var i = 0; i < count; i++)
                {
                    DrawData(funcData[m_EditorOnlyList[i]]);
                }
            }

            count = m_DisableList.Count;
            if (GUILayout.Button($"Disable ({count})", GUICustomStyle.StandardButtonStyle))
            {
                m_Setting.showDisableList = !m_Setting.showDisableList;
            }
            EditorGUILayout.Space(10);
            if (m_Setting.showDisableList)
            {
                for (var i = 0; i < count; i++)
                {
                    DrawData(funcData[m_DisableList[i]]);
                }
            }
        }

        private void DrawData(DeveloperDebugSettingData data)
        {
            EditorGUILayout.BeginVertical(GUICustomStyle.BorderAreaStyle);
            EditorGUILayout.LabelField(data.functionName,GUICustomStyle.CenteredBigLabel);
            EditorGUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();
            data.enable = EditorGUILayout.ToggleLeft("Enable", data.enable,GUICustomStyle.MiddleLeftBoldMiniLabel, GUILayout.MaxWidth(80));
            GUI.enabled = data.enable;
            data.editorOnly = EditorGUILayout.ToggleLeft("Editor Only", data.editorOnly, GUICustomStyle.MiddleLeftBoldMiniLabel,GUILayout.MaxWidth(80));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("KeyCode",GUICustomStyle.MiddleLeftBoldMiniLabel);
            data.keyCode = EditorGUILayout.TextField(data.keyCode,GUICustomStyle.EditTextFieldStyle);
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("TouchCode",GUICustomStyle.MiddleLeftBoldMiniLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            
            data.touchCode = EditorGUILayout.TextField(data.touchCode,GUICustomStyle.EditTextFieldStyle);
            
            GUI.enabled = data.enable;
            if (GUILayout.Button("Edit", GUICustomStyle.EditButtonStyle))
            {
                data.editTouchCode = !data.editTouchCode;
            }
            EditorGUILayout.EndHorizontal();
            if(data.editTouchCode) DrawButtonTouch(data);
            GUI.enabled = true;
            if (data.enable) CheckCorrect(data.keyCode, data.touchCode,m_Setting.debugData);
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
            var oldData = ((DeveloperDebugSetting) target).debugData;
            var newData = new List<DeveloperDebugSettingData>();
            var methodCount = m_MethodInfo.Length;
            for (var i = 0; i < methodCount; i++)
            {
                var data = oldData.Find(item => string.Equals(item.functionName,m_MethodInfo[i].Name));
                if (data != null)
                {
                    newData.Add(data);
                    continue;
                }
                newData.Add(new DeveloperDebugSettingData(m_MethodInfo[i].Name));
            }
            
            ((DeveloperDebugSetting)target).debugData = newData;
        }

        public static void CheckCorrect(string keyCode, string touchCode,List<DeveloperDebugSettingData> data,int eventCount = 1)
        {
            var keyCodeLength = string.IsNullOrEmpty(keyCode) ? 0 : keyCode.Length;
            var touchCodeLenght = string.IsNullOrEmpty(touchCode) ? 0 : touchCode.Length;
            if (keyCodeLength == 0 && touchCodeLenght == 0 || eventCount == 0)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("This function will not be called", MessageType.Warning);
                return;
            }

            if (keyCodeLength > 0 && keyCodeLength < 5)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("The key code is case sensitive and at least 5 characters", MessageType.Error);
            }
            
            if (touchCodeLenght > 0 && touchCodeLenght < 4)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("The touch code is at least 4 characters", MessageType.Error);
            }

            if (keyCodeLength >= 5 && data.Count(item => item.enable && string.Equals(item.keyCode,keyCode)) > 1)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("This key code has been used", MessageType.Error);
            }

            if (touchCodeLenght >= 4 && data.Count(item => item.enable && string.Equals(item.touchCode,touchCode)) > 1)
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
            if(GUILayout.Button("2",GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += '2';
            }
            if(GUILayout.Button("3",GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += '3';
            }
            if(GUILayout.Button("4",GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += '4';
            }
            if(GUILayout.Button("X",GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode = string.Empty;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}
