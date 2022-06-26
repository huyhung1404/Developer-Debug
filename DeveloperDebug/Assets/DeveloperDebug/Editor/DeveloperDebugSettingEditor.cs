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
            DrawEnableForBuild();
            if (m_MethodInfo == null)
            {
                UpdateDictionary();
            }
            DrawDictionary(m_Setting.debugData);
        }
        private void DrawEnableForBuild()
        {
            var lastEnable = m_Setting.enableForBuild;
            m_Setting.enableForBuild = EditorGUILayout.Toggle("Enable For Build", m_Setting.enableForBuild);
            if (lastEnable != m_Setting.enableForBuild)
            {
                
            }
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

            EditorGUILayout.LabelField("Enable");
            var count = m_EnableList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(funcData[m_EnableList[i]]);
            }
            
            EditorGUILayout.LabelField("Editor Only");
            count = m_EditorOnlyList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(funcData[m_EditorOnlyList[i]]);
            }
            
            EditorGUILayout.LabelField("Disable");
            count = m_DisableList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(funcData[m_DisableList[i]]);
            }
        }

        private void DrawData(DeveloperDebugSettingData data)
        {
            EditorGUILayout.LabelField(data.functionName);
            data.enable = EditorGUILayout.Toggle("Enable", data.enable);
            GUI.enabled = data.enable;    
            data.keyCode = EditorGUILayout.TextField("KeyCode", data.keyCode);
            data.touchCode = EditorGUILayout.TextField("TouchCode", data.touchCode);
            data.editorOnly = EditorGUILayout.Toggle("Editor Only", data.editorOnly);
            GUI.enabled = true;
            if (data.enable) CheckCorrect(data.keyCode, data.touchCode);
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
                var data = oldData.Find(item => item.functionName.Equals(m_MethodInfo[i].Name));
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
            var keyCodeLength = keyCode.Length;
            var touchCodeLenght = touchCode.Length;
            if (keyCodeLength == 0 && touchCodeLenght == 0)
            {
                EditorGUILayout.HelpBox("This function will not be called", MessageType.Warning);
                return;
            }

            if (keyCodeLength > 0 && keyCodeLength < 5)
            {
                EditorGUILayout.HelpBox("The key code is case sensitive and at least 5 characters", MessageType.Error);
            }
            
            if (touchCodeLenght > 0 && touchCodeLenght < 5)
            {
                EditorGUILayout.HelpBox("The touch code is at least 4 characters", MessageType.Error);
            }

            if (keyCodeLength >= 5 && m_Setting.debugData.Count(item => item.enable && item.keyCode.Equals(keyCode)) > 1)
            {
                EditorGUILayout.HelpBox("This key code has been used", MessageType.Error);
            }

            if (touchCodeLenght >= 4 && m_Setting.debugData.Count(item => item.enable && item.touchCode.Equals(touchCode)) > 1)
            {
                EditorGUILayout.HelpBox("This touch code has been used", MessageType.Error);
            }
        }
    }
}
