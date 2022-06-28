namespace DeveloperDebug.Core
{
    public class DeveloperDebugExtension
    {
//         /// <summary>
//         /// Register key code debug
//         /// </summary>
//         /// <param name="key">The key is case sensitive and at least 5 characters</param>
//         /// <param name="action">Executable code</param>
//         /// <param name="runOnEditorOnly">Only running in Unity Editor</param>
//         public static void RegisterKeyCode(string key, Action action, bool runOnEditorOnly)
//         {
//             if (runOnEditorOnly)
//             {
// #if UNITY_EDITOR
//                 KeyCodeDebug.Register(key, action);
// #endif
//                 return;
//             }
//
// #if UNITY_EDITOR || DEVELOPER_DEBUG
//             KeyCodeDebug.Register(key, action);
// #endif
//         }
//
//         public static void UnregisterKeyCode(string key)
//         {
// #if UNITY_EDITOR || DEVELOPER_DEBUG
//             if(string.IsNullOrEmpty(key)) return;
//             KeyCodeDebug.Unregister(key);
// #endif
//         }
//
//         /// <summary>
//         /// Register touch debug
//         /// </summary>
//         /// <param name="key">0nly 4 characters are valid 'U' | 'D' | 'L' | 'R'  means swipe 'UP' | 'DOWN' | 'LEFT' |'RIGHT' characters are capital letters</param>
//         /// <param name="action">Executable code</param>
//         /// /// <param name="runOnEditorOnly">Only running in Unity Editor</param>
//         public static void RegisterTouchDebug(string key, Action action, bool runOnEditorOnly)
//         {
//             if (runOnEditorOnly)
//             {
// #if UNITY_EDITOR
//                 TouchDebug.Register(key.ToUpper(), action);
// #endif
//                 return;
//             }
//
// #if UNITY_EDITOR || DEVELOPER_DEBUG
//             TouchDebug.Register(key.ToUpper(), action);
// #endif
//         }
//
//         public static void UnregisterTouchDebug(string key)
//         {
// #if UNITY_EDITOR || DEVELOPER_DEBUG
//             if(string.IsNullOrEmpty(key)) return;
//             TouchDebug.Unregister(key.ToUpper());
// #endif
        // }
    }
}
