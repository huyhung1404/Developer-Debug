using System.Collections.Generic;
using DeveloperDebug.Core;
using UnityEngine;

namespace DeveloperDebug.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(AddDeveloperDebugEvent))]
    public class AddDeveloperDebugEventEditor : Editor
    {
        private DeveloperDebugSetting m_Setting;

        public override void OnInspectorGUI()
        {
            var data = (AddDeveloperDebugEvent) target;
            serializedObject.Update();
            if (m_Setting == null)
                m_Setting = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting");
            DrawData(data.dataAdd, data.enabled, data.debugEvent.GetPersistentEventCount());
            if (!GUI.changed) return;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void DrawData(DeveloperDebugSettingData data, bool enable, int eventCount)
        {
            GUI.enabled = enable;
            data.editorOnly = EditorGUILayout.Toggle("Editor Only", data.editorOnly);
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("KeyCode", GUICustomStyle.MiddleLeftBoldMiniLabel);
            data.keyCode = EditorGUILayout.TextField(data.keyCode, GUICustomStyle.EditTextFieldStyle);
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("TouchCode", GUICustomStyle.MiddleLeftBoldMiniLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            data.touchCode = EditorGUILayout.TextField(data.touchCode, GUICustomStyle.EditTextFieldStyle);
            GUI.enabled = enable;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            DeveloperDebugSettingEditor.DrawButtonTouch(data);
            var debugEvent = serializedObject.FindProperty("debugEvent");
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(debugEvent);
            EditorGUILayout.Space(5);
            GUI.enabled = true;
            if (enable) DeveloperDebugSettingEditor.CheckCorrect(data.keyCode, data.touchCode, m_Setting.debugData,m_Setting.minLengthKeyCode,m_Setting.minLengthTouchCode,eventCount);
            EditorGUILayout.Space(10);
        }
    }
}