#if UNITY_EDITOR || DEVELOPER_DEBUG
using System;
using System.Collections.Generic;

namespace DeveloperDebug
{
    public static class DeveloperData
    {
        /// <summary>
        /// The key is case sensitive and at least 5 characters
        /// </summary>
        public static readonly Dictionary<string, Action> KEYCODE = new Dictionary<string, Action>
        {
        
        };
    
        /// <summary>
        /// 0nly 4 characters are valid
        /// U | D | L | R  means swipe UP | DOWN | LEFT | RIGHT
        /// characters are capital letters
        /// </summary>
        public static readonly Dictionary<string, Action> TOUCH = new Dictionary<string, Action>
        {

        };
    
        static DeveloperData()
        {
#if UNITY_EDITOR
            foreach (var editorKeyCodeData in OnlyEditorDeveloperData.ONLY_EDITOR_KEYCODE)
            {
                KEYCODE.Add(editorKeyCodeData.Key,editorKeyCodeData.Value);
            }
            foreach (var editorTouchData in OnlyEditorDeveloperData.ONLY_EDITOR_TOUCH)
            {
                TOUCH.Add(editorTouchData.Key,editorTouchData.Value);
            }
#endif
        }
    }
}
#endif
