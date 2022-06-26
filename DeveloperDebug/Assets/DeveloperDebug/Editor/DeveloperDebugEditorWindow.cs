using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DeveloperDebug.Editor
{
    public class DeveloperDebugEditorWindow : EditorWindow
    {
        private static DeveloperDebugEditorWindow _window;
        private string _textCode;
        private bool _focusTextField;
        private Vector2 _scrollPos;
        private List<string> _keyData = new List<string>();
        private int _keyCount;

        [MenuItem("Window/DeveloperDebug")]
        public static void OpenPopupWindow()
        {
            _window = GetWindow<DeveloperDebugEditorWindow>("Developer Debug");
            _window.maxSize = new Vector2(600, 100);
            _window.minSize = new Vector2(600, 35);
            _window.Init();
        }

        private void Init()
        {
            _textCode = string.Empty;
            _focusTextField = false;
            _keyData.Clear();
            _keyData = KeyCodeDebug.GetKeyData();
            _keyData.AddRange(TouchDebug.GetTouchData());
            _keyCount = _keyData.Count;
        }

        private void OnGUI()
        {
            if (_focusTextField)
            {
                _textCode = EditorGUILayout.TextField("Developer Code: ", _textCode);
            }
            else
            {
                GUI.SetNextControlName("textCodeField");
                _textCode = EditorGUILayout.TextField("Developer Code: ", _textCode);
                EditorGUI.FocusTextInControl("textCodeField");
                _focusTextField = true;
            }

            DrawKeyValue();

            if (!Event.current.isKey) return;
            switch (Event.current.keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    ExecuteDeveloperCode();
                    _window.Close();
                    Event.current.Use();
                    break;
            }
        }

        private void DrawKeyValue()
        {
            EditorGUILayout.LabelField($"Has {_keyCount} key code", EditorStyles.centeredGreyMiniLabel);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(600), GUILayout.Height(80));
            var totalRow = (int)Math.Ceiling(_keyCount / 3f);
            for (var i = 0; i < totalRow; i++)
            {
                DrawRow(i);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawRow(int i)
        {
            EditorGUILayout.BeginHorizontal();
            var keyIndex = _keyData.Count - 1;
            EditorGUILayout.LabelField(i * 3 <= keyIndex ? _keyData[i * 3] : "");
            EditorGUILayout.LabelField(i * 3 + 1 <= keyIndex ? _keyData[i * 3 + 1] : "");
            EditorGUILayout.LabelField(i * 3 + 2 <= keyIndex ? _keyData[i * 3 + 2] : "");
            EditorGUILayout.EndHorizontal();
        }

        private void ExecuteDeveloperCode()
        {
            if (string.IsNullOrEmpty(_textCode)) return;
            var dic = KeyCodeDebug.GetData();
            if (dic.ContainsKey(_textCode))
            {
                dic[_textCode]?.Invoke();
                return;
            }

            dic = TouchDebug.GetData();
            if (dic.ContainsKey(_textCode))
            {
                dic[_textCode]?.Invoke();
                return;
            }

            Debug.LogError("Key does not exist");
        }
    }
}