namespace DeveloperDebug.Editor
{
    using System;
    using System.Collections.Generic;
    using Core;
    using UnityEditor;
    using UnityEngine;

    public class DeveloperDebugEditorWindow : EditorWindow
    {
        private static DeveloperDebugEditorWindow m_Window;
        private string m_TextCode;
        private bool m_FocusTextField;
        private Vector2 m_ScrollPos;
        private DeveloperDebugSetting m_Setting;
        private Dictionary<string, Action> m_KeyCodeData;

        [MenuItem("Window/Developer Debug %#/")]
        public static void OpenPopupWindow()
        {
            m_Window = GetWindow<DeveloperDebugEditorWindow>("Developer Debug");
            m_Window.maxSize = new Vector2(600, 160);
            m_Window.minSize = new Vector2(600, 55);
            m_Window.Init();
        }

        private void Init()
        {
            m_TextCode = string.Empty;
            m_FocusTextField = false;
            m_Setting = Resources.Load<DeveloperDebugSetting>("DeveloperDebugSetting");
            m_KeyCodeData = m_Setting.GetKeyCodeData();
        }

        private void OnGUI()
        {
            if (m_FocusTextField)
            {
                m_TextCode = EditorGUILayout.TextField("Developer Code: ", m_TextCode);
            }
            else
            {
                GUI.SetNextControlName("textCodeField");
                m_TextCode = EditorGUILayout.TextField("Developer Code: ", m_TextCode);
                EditorGUI.FocusTextInControl("textCodeField");
                m_FocusTextField = true;
            }

            GUI.SetNextControlName("buttonSetting");
            if (GUILayout.Button("Go To Developer Debug Setting"))
            {
                Selection.activeObject = m_Setting;
                m_Window.Close();
            }

            DrawKeyValue();

            if (!Event.current.isKey) return;
            switch (Event.current.keyCode)
            {
                case KeyCode.Tab:
                    EditorGUI.FocusTextInControl("buttonSetting");
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    ExecuteDeveloperCode();
                    m_Window.Close();
                    Event.current.Use();
                    break;
            }
        }

        private void DrawKeyValue()
        {
            EditorGUILayout.LabelField($"Has {m_KeyCodeData.Count} key code", EditorStyles.centeredGreyMiniLabel);
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(600), GUILayout.Height(100));
            var _index = 0;
            foreach (var KeyCode in m_KeyCodeData)
            {
                var _residuals = _index % 3;
                if (_residuals == 0) EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(KeyCode.Key,GUICustomStyle.StandardButtonStyle,GUILayout.MinWidth(180)))
                {
                    KeyCode.Value.Invoke();
                    m_Window.Close();
                }

                if (_residuals == 2) EditorGUILayout.EndHorizontal();
                _index++;
            }
            EditorGUILayout.EndScrollView();
        }

        private void ExecuteDeveloperCode()
        {
            if (string.IsNullOrEmpty(m_TextCode)) return;
            if (m_KeyCodeData.ContainsKey(m_TextCode))
            {
                m_KeyCodeData[m_TextCode]?.Invoke();
                Debug.Log("Execute");
                return;
            }
            Debug.LogError("Key does not exist");
        }
    }
}