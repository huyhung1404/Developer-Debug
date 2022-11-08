namespace DeveloperDebug.Core
{
    using UnityEngine;
    using System;

    public class DeveloperDebugParameter : MonoBehaviour
    {
        private string m_StringInput;
        private Rect m_PopupRect;
        private int m_WindowWidth;
        private int m_WindowHeight;
        private Action<string> m_OnEnter;
        private Action m_OnCancel;
        private bool m_FocusText;
        private static readonly GUIStyle m_Style;

        static DeveloperDebugParameter()
        {
            m_Style = new GUIStyle();
        }

        public void SetUp(Action<string> onEnter, Action onCancel)
        {
            m_OnEnter = onEnter;
            m_OnCancel = onCancel;
        }

        private void Start()
        {
            var _isPortrait = Screen.width < Screen.height;
            m_WindowWidth = (int)(Screen.width * (_isPortrait ? 0.75 : 0.45));
            m_WindowHeight = (int)(Screen.height * (_isPortrait ? 0.12 : 0.2));
            m_PopupRect = new Rect((float)(Screen.width - m_WindowWidth) / 2, Screen.height - m_WindowHeight, m_WindowWidth, m_WindowHeight);
            m_FocusText = false;
            m_Style.fontSize = (int)(0.2f * m_WindowHeight);
        }

        private void OnGUI()
        {
            GUI.Window(0, m_PopupRect, WindowFunc, string.Empty);
        }

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        private void WindowFunc(int i)
        {
            GUILayout.Space(0.015f * m_WindowHeight);
            GUI.SetNextControlName("Input");
            m_StringInput = GUILayout.TextField(m_StringInput, m_Style, GUILayout.Height(0.3f * m_WindowHeight));
            if (!m_FocusText)
            {
                m_FocusText = true;
                GUI.FocusControl("Input");
            }

            GUILayout.Space(0.01f * m_WindowHeight);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", m_Style, GUILayout.Width(0.35f * m_WindowWidth), GUILayout.Height(0.25f * m_WindowHeight)))
            {
                m_OnCancel?.Invoke();
                Destroy(gameObject);
                GUIUtility.ExitGUI();
            }

            GUILayout.Space(0.02f * m_WindowHeight);
            if (GUILayout.Button("Enter", m_Style, GUILayout.Width(0.35f * m_WindowWidth), GUILayout.Height(0.25f * m_WindowHeight)))
            {
                m_OnEnter?.Invoke(m_StringInput);
                Destroy(gameObject);
                GUIUtility.ExitGUI();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
#if UNITY_EDITOR || (DEVELOPER_DEBUG && !UNITY_IOS && !UNITY_ANDROID)
            if (!Event.current.isKey) return;
            switch (Event.current.keyCode)
            {
                case KeyCode.Escape:
                    m_OnCancel?.Invoke();
                    Destroy(gameObject);
                    GUIUtility.ExitGUI();
                    Event.current.Use();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    m_OnEnter?.Invoke(m_StringInput);
                    Destroy(gameObject);
                    GUIUtility.ExitGUI();
                    Event.current.Use();
                    break;
            }
#endif
        }
    }
}