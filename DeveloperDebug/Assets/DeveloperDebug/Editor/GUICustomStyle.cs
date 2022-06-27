namespace DeveloperDebug.Editor
{
    using UnityEditor;
    using UnityEngine;

    public static class GUICustomStyle
    {
        public static GUIStyle MiddleLeftBoldMiniLabel { get; }
        public static GUIStyle CenteredBigLabel { get; }
        public static GUIStyle StandardButtonStyle { get; }
        public static GUIStyle ArrowButtonStyle { get; }
        public static GUIStyle EditButtonStyle { get; }
        public static GUIStyle EditTextFieldStyle { get; }

        static GUICustomStyle()
        {
            var isDaskSkin = EditorGUIUtility.isProSkin;
            MiddleLeftBoldMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 12,
                normal =
                {
                    textColor = isDaskSkin ? Color.white : Color.black,
                    background = null
                },
                alignment = TextAnchor.MiddleLeft
            };

            CenteredBigLabel = new GUIStyle(MiddleLeftBoldMiniLabel)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal
            };

            StandardButtonStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button)
            {
                fixedHeight = 25,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(5, 0, 0, 0),
                fontSize = 13,
                normal =
                {
                    textColor = isDaskSkin ? new Color(1f, 1f, 1f, 0.85f) : new Color(0f,0f,0f,0.85f),
                    background = null
                }
            };

            ArrowButtonStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button)
            {
                fixedWidth = 30,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = 
                {
                    textColor = isDaskSkin ? Color.white : Color.black,
                    background = null
                },
                hover = 
                {
                    textColor = isDaskSkin ? new Color(1f, 1f, 1f, 0.75f) : new Color(0f,0f,0f,0.75f),
                    background = null
                },
                focused = 
                {
                    textColor = isDaskSkin ? new Color(1f, 1f, 1f, 0.85f) : new Color(0f,0f,0f,0.85f),
                    background = null
                }
            };
            
            EditButtonStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button)
            {
                fixedWidth = 40,
                fixedHeight = 25,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = 
                {
                    textColor = isDaskSkin ? Color.white : Color.black,
                    background = null
                },
                hover = 
                {
                    textColor = isDaskSkin ? new Color(1f, 1f, 1f, 0.75f) : new Color(0f,0f,0f,0.75f),
                    background = null
                },
                focused = 
                {
                    textColor = isDaskSkin ? new Color(1f, 1f, 1f, 0.85f) : new Color(0f,0f,0f,0.85f),
                    background = null
                }
            };

            EditTextFieldStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).textField)
            {
                fixedHeight = 25,
                fontSize = 15,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10,0,0,0),
                normal = 
                {
                    textColor = isDaskSkin ? Color.white : Color.black,
                    background = null
                },
                hover = 
                {
                    textColor = isDaskSkin ? new Color(1f, 1f, 1f, 0.75f) : new Color(0f,0f,0f,0.75f),
                    background = null
                },
                focused = 
                {
                    textColor = isDaskSkin ? new Color(1f, 1f, 1f, 0.85f) : new Color(0f,0f,0f,0.85f),
                    background = null
                }
            };
        }

        public static void GuiLine(int i_height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}