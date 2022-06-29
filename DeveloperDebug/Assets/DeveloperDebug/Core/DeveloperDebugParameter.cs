namespace DeveloperDebug.Core
{
    using UnityEngine;

    public class DeveloperDebugParameter : MonoBehaviour
    {
        private string m_StringInput;
        private Rect m_PopupRect;
        private void Start()
        {
            var _windowWidth = 400;
            var _windowHeight = 120;
            m_PopupRect = new Rect((float)(Screen.width - _windowWidth) / 2, (float)(Screen.height - _windowHeight) / 2,
                _windowWidth, _windowHeight);
        }

        private void OnGUI()
        {
            GUI.Window(0, m_PopupRect, WindowFunc, "Developer Debug Input");
        }

        private void WindowFunc(int i)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Input", GUILayout.MaxWidth(30));
            m_StringInput = GUILayout.TextField(m_StringInput, GUILayout.MaxWidth(350),GUILayout.MaxHeight(50));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel",GUILayout.MaxWidth(120),GUILayout.MaxHeight(60)))
            {
                
            }

            if (GUILayout.Button("Enter",GUILayout.MaxWidth(120),GUILayout.MaxHeight(60)))
            {
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}