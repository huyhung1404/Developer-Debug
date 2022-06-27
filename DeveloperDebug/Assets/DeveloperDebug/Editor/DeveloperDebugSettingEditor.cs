using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeveloperDebug.Editor
{
    using UnityEditor;
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
            DrawEnableForBuild();
            if (m_MethodInfo == null)
            {
                UpdateDictionary();
            }
            DrawDictionary(m_Setting.debugData);
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
        private void DrawEnableForBuild()
        {
            var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            var enableForBuild = scriptingDefineSymbols.Contains("DEVELOPER_DEBUG");
            var storeEnableValue = enableForBuild;
            enableForBuild = EditorGUILayout.Toggle("Enable For Build", enableForBuild);
            GUILayout.Space(10);
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

            EditorGUILayout.LabelField("Enable",GUICustomStyle.StandardButtonStyle);
            EditorGUILayout.Space(10);
            var count = m_EnableList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(funcData[m_EnableList[i]]);
            }
            
            EditorGUILayout.LabelField("Editor Only",GUICustomStyle.StandardButtonStyle);
            EditorGUILayout.Space(10);
            count = m_EditorOnlyList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(funcData[m_EditorOnlyList[i]]);
            }
            
            EditorGUILayout.LabelField("Disable",GUICustomStyle.StandardButtonStyle);
            EditorGUILayout.Space(10);
            count = m_DisableList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(funcData[m_DisableList[i]]);
            }
        }

        private void DrawData(DeveloperDebugSettingData data)
        {
            GUICustomStyle.GuiLine();
            EditorGUILayout.LabelField(data.functionName,GUICustomStyle.CenteredBigLabel);
            EditorGUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();
            data.enable = EditorGUILayout.ToggleLeft("Enable", data.enable, GUILayout.MaxWidth(80));
            GUI.enabled = data.enable;
            data.editorOnly = EditorGUILayout.ToggleLeft("Editor Only", data.editorOnly, GUILayout.MaxWidth(80));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("KeyCode",GUICustomStyle.MiddleLeftBoldMiniLabel);
            data.keyCode = EditorGUILayout.TextField(data.keyCode,GUICustomStyle.EditTextFieldStyle);
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("TouchCode",GUICustomStyle.MiddleLeftBoldMiniLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            
            data.touchCode = ChangeGraphicToTouch(EditorGUILayout.TextField(ChangeTouchCodeToGraphic(data.touchCode),GUICustomStyle.EditTextFieldStyle));
            
            GUI.enabled = data.enable;
            if (GUILayout.Button("Edit", GUICustomStyle.EditButtonStyle))
            {
                data.editTouchCode = !data.editTouchCode;
            }
            EditorGUILayout.EndHorizontal();
            if(data.editTouchCode) DrawButtonTouch(data);
            GUI.enabled = true;
            if (data.enable) CheckCorrect(data.keyCode, data.touchCode);
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

        private void CheckCorrect(string keyCode, string touchCode)
        {
            var keyCodeLength = string.IsNullOrEmpty(keyCode) ? 0 : keyCode.Length;
            var touchCodeLenght = string.IsNullOrEmpty(touchCode) ? 0 : touchCode.Length;
            if (keyCodeLength == 0 && touchCodeLenght == 0)
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

            if (keyCodeLength >= 5 && m_Setting.debugData.Count(item => item.enable && string.Equals(item.keyCode,keyCode)) > 1)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("This key code has been used", MessageType.Error);
            }

            if (touchCodeLenght >= 4 && m_Setting.debugData.Count(item => item.enable && string.Equals(item.touchCode,touchCode)) > 1)
            {
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("This touch code has been used", MessageType.Error);
            }
        }

        private void DrawButtonTouch(DeveloperDebugSettingData data)
        {
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("←", GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += 'L';
            }
            if(GUILayout.Button("→",GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += 'R';
            }
            if(GUILayout.Button("↑",GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += 'U';
            }
            if(GUILayout.Button("↓",GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode += 'D';
            }
            if(GUILayout.Button("X",GUICustomStyle.ArrowButtonStyle))
            {
                data.touchCode = string.Empty;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private string ChangeTouchCodeToGraphic(string touchCode)
        {
            return string.IsNullOrEmpty(touchCode) ? null : touchCode.Replace('L', '←').Replace('R', '→').Replace('U', '↑').Replace('D', '↓');
        }
        
        private string ChangeGraphicToTouch(string graphic)
        {
            return string.IsNullOrEmpty(graphic) ? null : graphic.Replace('←','L').Replace('→','R').Replace('↑','U').Replace('↓','D');
        }
    }
}
