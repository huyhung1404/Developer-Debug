#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace DeveloperDebug
{
    /// <summary>
    /// Only Run In Editor
    /// </summary>
    public static class OnlyEditorDeveloperData
    {
        public static readonly Dictionary<string, Action> ONLY_EDITOR_KEYCODE = new Dictionary<string, Action>
        {
        };

        public static readonly Dictionary<string, Action> ONLY_EDITOR_TOUCH = new Dictionary<string, Action>
        {
        };
    }
}

#endif