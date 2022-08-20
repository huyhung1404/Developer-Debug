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

        public void SetUp(Action<string> onEnter,Action onCancel)
        {
            m_OnEnter = onEnter;
            m_OnCancel = onCancel;
        }
        
        private void Start()
        {
            var _isPortrait = Screen.width < Screen.height;
            m_WindowWidth = (int) (Screen.width * (_isPortrait ? 0.75 : 0.45));
            m_WindowHeight = (int) (Screen.height * (_isPortrait ? 0.12 : 0.2));
            m_PopupRect = new Rect((float) (Screen.width - m_WindowWidth) / 2, Screen.height - m_WindowHeight, m_WindowWidth, m_WindowHeight);
        }

        private void OnGUI()
        {
            GUI.Window(0, m_PopupRect, WindowFunc, "Developer Debug Input");
        }

        private void WindowFunc(int i)
        {
            GUILayout.Space(5);
            m_StringInput = GUILayout.TextArea(m_StringInput, GUILayout.Height(0.4f * m_WindowHeight));
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(0.35f * m_WindowWidth), GUILayout.Height(0.25f * m_WindowHeight)))
            {
                m_OnCancel?.Invoke();
                Destroy(gameObject);
                GUIUtility.ExitGUI();
            }

            GUILayout.Space(15);
            if (GUILayout.Button("Enter", GUILayout.Width(0.35f * m_WindowWidth), GUILayout.Height(0.25f * m_WindowHeight)))
            {
                m_OnEnter?.Invoke(m_StringInput);
                Destroy(gameObject);
                GUIUtility.ExitGUI();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}