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

        [MenuItem("Window/DeveloperDebug")]
        public static void OpenPopupWindow()
        {
            m_Window = GetWindow<DeveloperDebugEditorWindow>("Developer Debug");
            m_Window.maxSize = new Vector2(600, 100);
            m_Window.minSize = new Vector2(600, 35);
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
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(600), GUILayout.Height(80));
            var totalRow = (int)Math.Ceiling(m_KeyCodeData.Count / 3f);
            for (var i = 0; i < totalRow; i++)
            {
                DrawRow(i);
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawRow(int i)
        {
            // EditorGUILayout.BeginHorizontal();
            // var keyIndex = _keyData.Count - 1;
            // EditorGUILayout.LabelField(i * 3 <= keyIndex ? _keyData[i * 3] : "");
            // EditorGUILayout.LabelField(i * 3 + 1 <= keyIndex ? _keyData[i * 3 + 1] : "");
            // EditorGUILayout.LabelField(i * 3 + 2 <= keyIndex ? _keyData[i * 3 + 2] : "");
            // EditorGUILayout.EndHorizontal();
        }

        private void ExecuteDeveloperCode()
        {
            // if (string.IsNullOrEmpty(_textCode)) return;
            // // var dic = DeveloperDebugKeyCode.GetData();
            // // if (dic.ContainsKey(_textCode))
            // // {
            // //     dic[_textCode]?.Invoke();
            // //     Debug.Log("Execute");
            // //     return;
            // // }
            //
            // Debug.LogError("Key does not exist");
        }
    }
}