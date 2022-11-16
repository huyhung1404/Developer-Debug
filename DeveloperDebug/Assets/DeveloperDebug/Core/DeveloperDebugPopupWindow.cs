using System.Collections;

namespace DeveloperDebug.Core
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public enum TypePopupData
    {
        Button,
        Input,
        Custom
    }

    public class DeveloperDebugPopupWindow : MonoBehaviour
    {
        #region Static

        public static DeveloperDebugPopupWindow Instance;
        public static GUIStyle StyleTextField;
        public static GUIStyle StyleLabel;
        public static GUIStyle StyleButton;
        private static bool m_SetUpLayout;
        private List<Data> m_Actions;

        #endregion

        #region Layout Const

        private const float FONT_SIZE = 0.02f;
        private const float HORIZONTAL_SIZE = 0.005f;
        private const float VERTICAL_SIZE = 0.01f;
        private const float CONTENT_HEIGHT = 0.03f;

        #endregion
        
        private string m_FocusText;
        private bool m_IsStartFocus;

        public class Data
        {
            public TypePopupData type;
            public string stringInput;
            public Action<string> onEnter;
            public Action onCancel;
            public bool autoClose;
            public string title;
            public Action<Data> GUIDraw;
        }

        public void Register(Data data)
        {
            m_Actions.Add(data);
            m_IsStartFocus = false;
            m_FocusText = data.title;
            switch (data.type)
            {
                default:
                case TypePopupData.Button:
                    data.GUIDraw = DrawButton;
                    break;
                case TypePopupData.Input:
                    data.GUIDraw = DrawInput;
                    break;
                case TypePopupData.Custom:
                    break;
            }
        }

        public void Unregister(Data data)
        {
            StartCoroutine(IEUnregister(data));
        }

        private IEnumerator IEUnregister(Data data)
        {
            yield return new WaitForEndOfFrame();
            m_Actions.Remove(data);
            if (m_Actions.Count != 0) yield break;
            Destroy(gameObject);
            Instance = null;
        }

        private void Awake()
        {
            m_Actions = new List<Data>();
        }

        private void OnGUI()
        {
            SetLayoutGUI();
            foreach (var action in m_Actions)
            {
                action.GUIDraw?.Invoke(action);
                GUILayout.Space(Screen.height * VERTICAL_SIZE);
            }

            FocusAction();
        }

        private void SetLayoutGUI()
        {
            if (m_SetUpLayout) return;
            var fontSize = (int)(Screen.height * FONT_SIZE);
            var inputHeight = CONTENT_HEIGHT * Screen.height;
            m_SetUpLayout = true;
            StyleTextField = new GUIStyle(GUI.skin.textField)
            {
                fontSize = fontSize,
                fixedHeight = inputHeight
            };
            StyleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = inputHeight
            };
            StyleButton = new GUIStyle(GUI.skin.button)
            {
                fontSize = fontSize,
                fixedHeight = inputHeight
            };
        }

        private void DrawInput(Data data)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(data.title, StyleLabel, GUILayout.Width(0.2f * Screen.width));
            GUILayout.Space(Screen.width * HORIZONTAL_SIZE);
            GUI.SetNextControlName(data.title);
            data.stringInput = GUILayout.TextField(data.stringInput, StyleTextField, GUILayout.Width(0.25f * Screen.width));
            if (!m_IsStartFocus && m_FocusText == data.title)
            {
                m_IsStartFocus = true;
                GUI.FocusControl(data.title);
            }

            GUILayout.Space(Screen.width * HORIZONTAL_SIZE);
            if (GUILayout.Button("Enter", StyleButton, GUILayout.Width(0.15f * Screen.width)))
            {
                data.onEnter?.Invoke(data.stringInput);
                if (data.autoClose)
                {
                    StartCoroutine(IEUnregister(data));
                }
            }

            GUILayout.Space(Screen.width * HORIZONTAL_SIZE);

            if (GUILayout.Button("X", StyleButton, GUILayout.Width(CONTENT_HEIGHT * Screen.height)))
            {
                data.onCancel?.Invoke();
                StartCoroutine(IEUnregister(data));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawButton(Data data)
        {
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName(data.title);
            if (GUILayout.Button(data.title, StyleButton, GUILayout.Width(0.25f * Screen.width)))
            {
                data.onEnter?.Invoke(null);
                if (data.autoClose)
                {
                    StartCoroutine(IEUnregister(data));
                }
            }

            if (!m_IsStartFocus && m_FocusText == data.title)
            {
                m_IsStartFocus = true;
                GUI.FocusControl(data.title);
            }

            GUILayout.Space(Screen.width * HORIZONTAL_SIZE);

            if (GUILayout.Button("X", StyleButton, GUILayout.Width(CONTENT_HEIGHT * Screen.height)))
            {
                data.onCancel?.Invoke();
                StartCoroutine(IEUnregister(data));
            }

            GUILayout.EndHorizontal();
        }

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        private void FocusAction()
        {
#if (DEVELOPER_DEBUG && !UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            if (!Event.current.isKey) return;
            if (m_Actions.Count == 0)
            {
                Event.current.Use();
                return;
            }

            var focusText = GUI.GetNameOfFocusedControl();
            var currentAction = m_Actions.Find(item => string.Equals(item.title, focusText));
            if (currentAction == null)
            {
                Event.current.Use();
                return;
            }

            switch (Event.current.keyCode)
            {
                case KeyCode.Escape:
                    currentAction.onCancel?.Invoke();
                    StartCoroutine(IEUnregister(currentAction));
                    Event.current.Use();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    currentAction.onEnter?.Invoke(currentAction.stringInput);
                    if (currentAction.autoClose)
                    {
                        StartCoroutine(IEUnregister(currentAction));
                    }

                    Event.current.Use();
                    break;
            }
#endif
        }
    }
}