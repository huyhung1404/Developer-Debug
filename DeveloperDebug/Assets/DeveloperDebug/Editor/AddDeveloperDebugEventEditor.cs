using System;
using System.Collections.Generic;
using DeveloperDebug.Core;
using UnityEngine;

namespace DeveloperDebug.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(AddDeveloperDebugEvent))]
    public class AddDeveloperDebugEventEditor : Editor
    {
        private List<DeveloperDebugSettingData> m_ListData;
        public override void OnInspectorGUI()
        {
            var data = (AddDeveloperDebugEvent) target;
            serializedObject.Update();
            if(m_ListData == null ) m_ListData = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting").debugData;
            DrawData(data.dataAdd,data.enabled,data.debugEvent.GetPersistentEventCount());
            if(!GUI.changed) return;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void DrawData(DeveloperDebugSettingData data,bool enable,int eventCount)
        {
            GUI.enabled = enable;
            data.editorOnly = EditorGUILayout.Toggle("Editor Only", data.editorOnly);
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("KeyCode",GUICustomStyle.MiddleLeftBoldMiniLabel);
            data.keyCode = EditorGUILayout.TextField(data.keyCode,GUICustomStyle.EditTextFieldStyle);
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("TouchCode",GUICustomStyle.MiddleLeftBoldMiniLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            data.touchCode = DeveloperDebugSettingEditor.ChangeGraphicToTouch(EditorGUILayout.TextField(DeveloperDebugSettingEditor.ChangeTouchCodeToGraphic(data.touchCode),GUICustomStyle.EditTextFieldStyle));
            GUI.enabled = enable;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            DeveloperDebugSettingEditor.DrawButtonTouch(data);
            var debugEvent = serializedObject.FindProperty("debugEvent");
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(debugEvent);
            EditorGUILayout.Space(5);
            GUI.enabled = true;
            if (enable) DeveloperDebugSettingEditor.CheckCorrect(data.keyCode, data.touchCode,m_ListData,eventCount);
            EditorGUILayout.Space(10);
        }
        
    }
}
