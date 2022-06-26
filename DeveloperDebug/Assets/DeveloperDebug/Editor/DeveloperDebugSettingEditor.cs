using System;
using System.Collections.Generic;
using System.Reflection;
using Codice.Client.BaseCommands.BranchExplorer;
using UnityEditor.UIElements;
using UnityEngine;

namespace DeveloperDebug.Editor
{
    using UnityEditor;
    [CustomEditor(typeof(DeveloperDebugSetting))]
    public class DeveloperDebugSettingEditor : Editor
    {
        private MethodInfo[] m_MethodInfo;
        private List<string> m_EnableList = new List<string>();
        private List<string> m_EditorOnlyList = new List<string>();
        private List<string> m_DisableList = new List<string>();

        public override void OnInspectorGUI()
        {
            var setting = (DeveloperDebugSetting) target;
            // serializedObject.Update();
            DrawEnableForBuild(setting);
            if (m_MethodInfo == null)
            {
                UpdateDictionary();
            }
            DrawDictionary(setting.data);
            // serializedObject.ApplyModifiedProperties();
        }
        private void DrawEnableForBuild(DeveloperDebugSetting setting)
        {
            var lastEnable = setting.enableForBuild;
            setting.enableForBuild = EditorGUILayout.Toggle("Enable For Build", setting.enableForBuild);
            if (lastEnable != setting.enableForBuild)
            {
                
            }
        }

        private void DrawDictionary(List<DeveloperDebugSettingData> funcData)
        {
            m_EnableList.Clear();
            m_EditorOnlyList.Clear();
            m_DisableList.Clear();
            foreach (var developerDebugFunc in funcData)
            {
                if (string.IsNullOrEmpty(developerDebugFunc.Value.keyCode) &&
                    string.IsNullOrEmpty(developerDebugFunc.Value.touchCode))
                {
                    m_DisableList.Add(developerDebugFunc.Key);
                    continue;
                }

                if (developerDebugFunc.Value.editorOnly)
                {
                    m_EditorOnlyList.Add(developerDebugFunc.Key);
                    continue;
                }
                m_EnableList.Add(developerDebugFunc.Key);
            }
            
            EditorGUILayout.LabelField("Enable");
            var count = m_EnableList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(m_EnableList[i], funcData[m_EnableList[i]]);
            }
            
            EditorGUILayout.LabelField("Editor Only");
            count = m_EditorOnlyList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(m_EditorOnlyList[i], funcData[m_EditorOnlyList[i]]);
            }
            
            EditorGUILayout.LabelField("Disable");
            count = m_DisableList.Count;
            for (var i = 0; i < count; i++)
            {
                DrawData(m_DisableList[i], funcData[m_DisableList[i]]);
            }
        }

        private void DrawData(string label ,DeveloperDebugSettingData data)
        {
            EditorGUILayout.LabelField(label);
            data.keyCode = EditorGUILayout.TextField("KeyCode", data.keyCode);
            data.touchCode = EditorGUILayout.TextField("TouchCode", data.touchCode);
            data.editorOnly = EditorGUILayout.Toggle("Editor Only", data.editorOnly);
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
            var oldDictionary = ((DeveloperDebugSetting) target).data;
            var newDictionary = new Dictionary<string, DeveloperDebugSettingData>();
            foreach (var method in m_MethodInfo)
            {
                if (oldDictionary.ContainsKey(method.Name))
                {
                    newDictionary.Add(method.Name,oldDictionary[method.Name]);
                    continue;
                }
                newDictionary.Add(method.Name,new DeveloperDebugSettingData());
            }

            ((DeveloperDebugSetting)target).data = newDictionary;
        }
    }
}
